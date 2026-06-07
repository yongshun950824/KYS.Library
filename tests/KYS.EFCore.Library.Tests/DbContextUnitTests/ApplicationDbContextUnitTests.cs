using KYS.EFCore.Library.DBContext;
using KYS.EFCore.Library.Tests.Domain.Entities;
using KYS.EFCore.Library.Tests.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static KYS.Library.Helpers.FormattingHelper;

namespace KYS.EFCore.Library.Tests.DbContextUnitTests;

public class ApplicationDbContextUnitTests
{
    private SqliteConnection? _connection;
    private TestApplicationDbContext? _context;

    [SetUp]
    public void Setup()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<TestApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new TestApplicationDbContext(options);
        _context.Database.EnsureCreated();
    }

    [TearDown]
    public void TearDown()
    {
        _context?.Dispose();
        _connection?.Dispose();
    }

    [Test]
    public async Task SaveChangesWithAuditAsync_InsertOperation_ShouldCreateActionLogEntry()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var category = new Category { Name = "Electronics" };
        _context!.Categories.Add(category);

        // Act
        var result = await _context.SaveChangesWithAuditAsync(userId);

        // Assert
        Assert.Greater(result, 0);
        var actionLogs = await _context.ActionLog.ToListAsync();
        Assert.AreEqual(1, actionLogs.Count);
        Assert.AreEqual("Category", actionLogs[0].ReferenceTable);
        Assert.AreEqual(AuditAction.Insert, actionLogs[0].ActionType);
        Assert.AreEqual(userId, actionLogs[0].CreatedBy);
        Assert.AreNotEqual(default(DateTime), actionLogs[0].CreatedDate);
        Assert.IsNotEmpty(actionLogs[0].RecordValue);
    }

    [Test]
    public async Task SaveChangesWithAuditAsync_InsertOperation_NonPrimaryTemporaryProperty_ShouldCaptureNewValue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var category = new Category { Name = "Electronics" };
        _context!.Categories.Add(category);

        // Diagnostic: ensure EF treats GeneratedCode as a temporary, store-generated value
        _context.ChangeTracker.DetectChanges();
        var genProp = _context.Entry(category).Property(c => c.GeneratedCode);
        var isStoreGeneratedOnAdd = genProp.Metadata.ValueGenerated == Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAdd
                                 && genProp.CurrentValue == null;
        Assert.IsTrue(genProp.IsTemporary || isStoreGeneratedOnAdd,
            "GeneratedCode should be treated as a store-generated/temporary value before save.");

        // Act
        var result = await _context.SaveChangesWithAuditAsync(userId);

        // Assert
        Assert.Greater(result, 0);
        var actionLog = await _context.ActionLog.SingleAsync();
        Assert.AreEqual(AuditAction.Insert, actionLog.ActionType);
        Assert.AreEqual(nameof(Category), actionLog.ReferenceTable);
        Assert.AreEqual(userId, actionLog.CreatedBy);
        Assert.IsNotEmpty(actionLog.RecordValue);

        // The GeneratedCode is a non-primary, DB-generated column that should
        // be captured into the audit record's JSON payload (NewValues branch).
        Assert.IsTrue(actionLog.RecordValue.Contains("GeneratedCode"),
            "RecordValue should contain the GeneratedCode field populated by the database.");
    }

    [Test]
    public async Task SaveChangesWithAuditAsync_InsertOperationWithSnakeCaseFormatting_ShouldCreateActionLogEntry()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var category = new Category { Name = "Electronics" };
        _context!.Categories.Add(category);

        // Act
        var result = await _context.SaveChangesWithAuditAsync(userId, Formatting.SnakeCase);

        // Assert
        Assert.Greater(result, 0);
        var actionLogs = await _context.ActionLog.ToListAsync();
        Assert.AreEqual(1, actionLogs.Count);
        Assert.AreEqual("category", actionLogs[0].ReferenceTable);
        Assert.AreEqual(AuditAction.Insert, actionLogs[0].ActionType);
        Assert.AreEqual(userId, actionLogs[0].CreatedBy);
        Assert.AreNotEqual(default(DateTime), actionLogs[0].CreatedDate);
        Assert.IsNotEmpty(actionLogs[0].RecordValue);
    }

    [Test]
    public async Task SaveChangesWithAuditAsync_UpdateOperation_ShouldCreateActionLogWithOldAndNewValues()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var category = new Category { Name = "Electronics" };
        _context!.Categories.Add(category);
        await _context.SaveChangesAsync();

        // Clear action logs
        _context.ActionLog.RemoveRange(_context.ActionLog);
        await _context.SaveChangesAsync();

        // Update the entity
        category.Name = "Gadgets";
        var entry = _context.Entry(category);
        entry.Property(c => c.Name).IsModified = true;

        // Act
        var result = await _context.SaveChangesWithAuditAsync(userId);

        // Assert
        Assert.Greater(result, 0);
        var actionLogs = await _context.ActionLog.ToListAsync();
        Assert.GreaterOrEqual(actionLogs.Count, 1);
        var updateLog = actionLogs.FirstOrDefault(a => a.ActionType == AuditAction.Update);
        Assert.IsNotNull(updateLog);
        Assert.AreEqual(nameof(Category), updateLog!.ReferenceTable);
        Assert.AreEqual(nameof(Category.Name), updateLog.ColumnName);
        Assert.AreEqual("Electronics", updateLog.ColumnOldValue);
        Assert.AreEqual("Gadgets", updateLog.ColumnNewValue);
        Assert.AreEqual(userId, updateLog.CreatedBy);
    }

    [Test]
    public async Task SaveChangesWithAuditAsync_UpdateOperation_ExplicitPropertyIsModified_ShouldCreateActionLog()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var category = new Category { Name = "Books" };
        _context!.Categories.Add(category);
        await _context.SaveChangesAsync();

        // Clear action logs and detach to simulate a fresh tracked update
        _context.ActionLog.RemoveRange(_context.ActionLog);
        await _context.SaveChangesAsync();

        category.Name = "Novels";
        var entry = _context.Entry(category);
        entry.State = EntityState.Modified;
        entry.Property(c => c.Name).IsModified = true;

        // Act
        var result = await _context.SaveChangesWithAuditAsync(userId);

        // Assert
        Assert.Greater(result, 0);
        var actionLog = await _context.ActionLog.SingleAsync(a => a.ActionType == AuditAction.Update);
        Assert.AreEqual(nameof(Category), actionLog.ReferenceTable);
        Assert.AreEqual(nameof(Category.Name), actionLog.ColumnName);
        Assert.AreEqual("Books", actionLog.ColumnOldValue);
        Assert.AreEqual("Novels", actionLog.ColumnNewValue);
    }

    [Test]
    public async Task SaveChangesWithAuditAsync_UpdateOperationWithSnakeCaseFormatting_ShouldCreateActionLogWithOldAndNewValues()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var category = new Category { Name = "Electronics" };
        _context!.Categories.Add(category);
        await _context.SaveChangesAsync();

        // Clear action logs
        _context.ActionLog.RemoveRange(_context.ActionLog);
        await _context.SaveChangesAsync();

        // Update the entity
        category.Name = "Gadgets";
        _context.Categories.Update(category);

        // Act
        var result = await _context.SaveChangesWithAuditAsync(userId, Formatting.SnakeCase);

        // Assert
        Assert.Greater(result, 0);
        var actionLogs = await _context.ActionLog.ToListAsync();
        Assert.GreaterOrEqual(actionLogs.Count, 1);
        var updateLog = actionLogs.FirstOrDefault(a => a.ActionType == AuditAction.Update);
        Assert.IsNotNull(updateLog);
        Assert.AreEqual("category", updateLog!.ReferenceTable);
        Assert.AreEqual("name", updateLog.ColumnName);
        Assert.AreEqual("Electronics", updateLog.ColumnOldValue);
        Assert.AreEqual("Gadgets", updateLog.ColumnNewValue);
        Assert.AreEqual(userId, updateLog.CreatedBy);
    }

    [Test]
    public async Task SaveChangesWithAuditAsync_DeleteOperation_ShouldCreateActionLogWithDeletedRecord()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var category = new Category { Name = "Electronics" };
        _context!.Categories.Add(category);
        await _context.SaveChangesAsync();

        // Clear action logs
        _context.ActionLog.RemoveRange(_context.ActionLog);
        await _context.SaveChangesAsync();

        // Delete the entity
        _context.Categories.Remove(category);

        // Act
        var result = await _context.SaveChangesWithAuditAsync(userId);

        // Assert
        Assert.Greater(result, 0);
        var actionLogs = await _context.ActionLog.ToListAsync();
        Assert.GreaterOrEqual(actionLogs.Count, 1);
        var deleteLog = actionLogs.FirstOrDefault(a => a.ActionType == AuditAction.Delete);
        Assert.IsNotNull(deleteLog);
        Assert.AreEqual(nameof(Category), deleteLog!.ReferenceTable);
        Assert.AreEqual(userId, deleteLog.CreatedBy);
        Assert.IsNotEmpty(deleteLog.RecordValue);
    }

    [Test]
    public async Task SaveChangesWithAuditAsync_MultipleOperations_ShouldCreateMultipleActionLogs()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var category1 = new Category { Name = "Electronics" };
        var category2 = new Category { Name = "Books" };
        _context!.Categories.Add(category1);
        _context!.Categories.Add(category2);

        // Act
        var result = await _context.SaveChangesWithAuditAsync(userId);

        // Assert
        Assert.Greater(result, 0);
        var actionLogs = await _context.ActionLog.Where(a => a.ActionType == AuditAction.Insert).ToListAsync();
        Assert.GreaterOrEqual(actionLogs.Count, 2);
        Assert.IsTrue(actionLogs.All(a => a.CreatedBy == userId));
    }

    [Test]
    public async Task SaveChangesWithAuditAsync_NoChanges_ShouldNotCreateActionLog()
    {
        // Arrange
        var category = new Category { Name = "Electronics" };
        _context!.Categories.Add(category);
        await _context.SaveChangesAsync();

        // Clear action logs
        _context.ActionLog.RemoveRange(_context.ActionLog);
        await _context.SaveChangesAsync();

        // Act - no changes made
        var result = await _context.SaveChangesWithAuditAsync(Guid.NewGuid());

        // Assert
        Assert.AreEqual(0, result);
        var actionLogs = await _context.ActionLog.ToListAsync();
        Assert.AreEqual(0, actionLogs.Count);
    }

    [Test]
    public void SaveChangesWithAuditAsync_WithInvalidFormatting_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var category = new Category { Name = "Electronics" };
        _context!.Categories.Add(category);

        // Act & Assert
        var exception = Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _context.SaveChangesWithAuditAsync(Guid.NewGuid(), (Formatting)999)
        );

        Assert.IsNotNull(exception);
        Assert.IsTrue(exception!.Message.Contains("Failed to convert table name"));
    }

    [Test]
    public async Task SaveChangesWithAuditAsync_WithNullUserId_ShouldCreateActionLogWithNullCreatedBy()
    {
        // Arrange
        var category = new Category { Name = "Electronics" };
        _context!.Categories.Add(category);

        // Act
        var result = await _context.SaveChangesWithAuditAsync(null);

        // Assert
        Assert.Greater(result, 0);
        var actionLogs = await _context.ActionLog.ToListAsync();
        Assert.AreEqual(1, actionLogs.Count);
        Assert.IsNull(actionLogs[0].CreatedBy);
    }

    [Test]
    public async Task SaveChangesWithAuditAsync_IgnoresActionLogEntitiesFromAudit()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var category = new Category { Name = "Electronics" };
        _context!.Categories.Add(category);
        await _context.SaveChangesWithAuditAsync();

        // Clear action logs
        _context.ActionLog.RemoveRange(_context.ActionLog);
        await _context.SaveChangesAsync();

        // Modify the category
        category.Name = "Gadgets";
        _context.Categories.Update(category);

        // Manually add an ActionLog entry (simulating external logging)
        // This should be IGNORED from auditing
        var manualActionLog = new ActionLog
        {
            ReferenceTable = "ManualTest",
            ReferenceId = "123",
            ActionType = AuditAction.Insert,
            CreatedBy = Guid.NewGuid(),
            CreatedDate = DateTime.UtcNow,
            RecordValue = "manual"
        };
        _context.ActionLog.Add(manualActionLog);

        // Act
        await _context.SaveChangesWithAuditAsync(userId);

        // Assert
        // Should have 2 ActionLog entries:
        // 1. The audit entry for the Category update (with CreatedBy = userId)
        // 2. The manually added ActionLog (not audited, so CreatedBy != userId)
        var allLogs = await _context.ActionLog.ToListAsync();
        Assert.AreEqual(2, allLogs.Count);

        // Verify that only the Category update was audited
        var auditedLogs = allLogs.Where(a => a.CreatedBy == userId).ToList();
        Assert.AreEqual(1, auditedLogs.Count);
        Assert.AreEqual(nameof(Category), auditedLogs[0].ReferenceTable);
        Assert.AreEqual(AuditAction.Update, auditedLogs[0].ActionType);

        // Verify that the manually added ActionLog was not audited (no audit log created for it)
        var manualLogs = allLogs.Where(a => a.ReferenceTable == "ManualTest").ToList();
        Assert.AreEqual(1, manualLogs.Count);
    }

    [Test]
    public async Task SaveChangesWithAuditAsync_NoEntry_ShouldNotInsertActionLogEntry()
    {
        // Ac
        await _context!.SaveChangesWithAuditAsync();

        // Assert
        var actionLogs = await _context.ActionLog.ToListAsync();
        Assert.AreEqual(0, actionLogs.Count);
    }
}