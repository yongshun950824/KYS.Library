using System;
using System.Text.Json;
using KYS.EFCore.Library.DBContext;
using KYS.EFCore.Library.DBContext.Partials;
using KYS.EFCore.Library.Tests.Domain.Entities;
using KYS.EFCore.Library.Tests.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static KYS.Library.Helpers.FormattingHelper;

namespace KYS.EFCore.Library.Tests;

public class AuditEntryUnitTests
{
    private static readonly JsonSerializerOptions _snakeCaseJsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        WriteIndented = false
    };

    [Test]
    public void ToActionLog_WithInvalidFormatting_ShouldReturnFailure()
    {
        // Arrange
        var auditEntry = new AuditEntry
        {
            TableName = "table",
            Action = AuditAction.Insert,
            UserId = Guid.NewGuid(),
            KeyValues = { { "Id", 1 } },
            NewValues = { { "ColumnName", "NewValue" } }
        };

        // Act
        var result = auditEntry.ToActionLog((Formatting)999);

        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.IsTrue(result.Error.Contains("Failed to convert table name"));
    }

    [Test]
    public void ToActionLog_InsertOperation_ShouldReturnSuccess()
    {
        // Arrange
        var auditEntry = new AuditEntry
        {
            TableName = "table",
            ColumnName = "ColumnName",
            Action = AuditAction.Insert,
            UserId = Guid.NewGuid(),
            KeyValues = { { "Id", 1 } },
            NewValues = { { "ColumnName", "NewValue" } }
        };

        // Act
        var result = auditEntry.ToActionLog();

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(auditEntry.TableName, result.Value.ReferenceTable);
        Assert.AreEqual(auditEntry.ColumnName, result.Value.ColumnName);
        Assert.AreEqual(
            JsonSerializer.Serialize(new { Id = 1, ColumnName = "NewValue" }),
            result.Value.RecordValue);
        Assert.IsNull(result.Value.ColumnOldValue);
        Assert.IsNull(result.Value.ColumnNewValue);
        Assert.AreEqual(auditEntry.Action, result.Value.ActionType);
        Assert.AreEqual(auditEntry.UserId, result.Value.CreatedBy);
        Assert.AreNotEqual(default(DateTime), result.Value.CreatedDate);
    }

    [Test]
    public void ToActionLog_InsertOperationWithSnakeCaseFormatting_ShouldReturnSuccess()
    {
        // Arrange
        var auditEntry = new AuditEntry
        {
            TableName = "Table",
            ColumnName = "ColumnName",
            Action = AuditAction.Insert,
            UserId = Guid.NewGuid(),
            KeyValues = { { "Id", 1 } },
            NewValues = { { "ColumnName", "NewValue" } }
        };

        // Act
        var result = auditEntry.ToActionLog(Formatting.SnakeCase);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("table", result.Value.ReferenceTable);
        Assert.AreEqual("column_name", result.Value.ColumnName);
        Assert.AreEqual(
            JsonSerializer.Serialize(new { ID = 1, ColumnName = "NewValue" }, _snakeCaseJsonSerializerOptions),
            result.Value.RecordValue);
        Assert.IsNull(result.Value.ColumnOldValue);
        Assert.IsNull(result.Value.ColumnNewValue);
        Assert.AreEqual(auditEntry.Action, result.Value.ActionType);
        Assert.AreEqual(auditEntry.UserId, result.Value.CreatedBy);
        Assert.AreNotEqual(default(DateTime), result.Value.CreatedDate);
    }

    [Test]
    public void ToActionLog_UpdateOperation_ShouldReturnSuccess()
    {
        // Arrange
        var auditEntry = new AuditEntry
        {
            TableName = "Table",
            ColumnName = "ColumnName",
            Action = AuditAction.Update,
            UserId = Guid.NewGuid(),
            KeyValues = { { "Id", 1 } },
            OldValues = { { "ColumnName", "OldValue" } },
            NewValues = { { "ColumnName", "NewValue" } }
        };

        // Act
        var result = auditEntry.ToActionLog();

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(auditEntry.TableName, result.Value.ReferenceTable);
        Assert.AreEqual(auditEntry.ColumnName, result.Value.ColumnName);
        Assert.IsNull(result.Value.RecordValue);
        Assert.AreEqual(auditEntry.OldValues[auditEntry.ColumnName], result.Value.ColumnOldValue);
        Assert.AreEqual(auditEntry.NewValues[auditEntry.ColumnName], result.Value.ColumnNewValue);
        Assert.AreEqual(auditEntry.Action, result.Value.ActionType);
        Assert.AreEqual(auditEntry.UserId, result.Value.CreatedBy);
        Assert.AreNotEqual(default(DateTime), result.Value.CreatedDate);
    }

    [Test]
    public void ToActionLog_DeleteOperation_ShouldReturnSuccess()
    {
        // Arrange
        var auditEntry = new AuditEntry
        {
            TableName = "Table",
            ColumnName = "ColumnName",
            Action = AuditAction.Delete,
            UserId = Guid.NewGuid(),
            KeyValues = { { "Id", 1 } },
            OldValues = { { "ColumnName", "OldValue" } }
        };

        // Act
        var result = auditEntry.ToActionLog();

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(auditEntry.TableName, result.Value.ReferenceTable);
        Assert.AreEqual(auditEntry.ColumnName, result.Value.ColumnName);
        Assert.AreEqual(
            JsonSerializer.Serialize(new { Id = 1, ColumnName = "OldValue" }),
            result.Value.RecordValue);
        Assert.IsNull(result.Value.ColumnOldValue);
        Assert.IsNull(result.Value.ColumnNewValue);
        Assert.AreEqual(auditEntry.Action, result.Value.ActionType);
        Assert.AreEqual(auditEntry.UserId, result.Value.CreatedBy);
        Assert.AreNotEqual(default(DateTime), result.Value.CreatedDate);
    }

    [Test]
    public void ToActionLog_DeleteOperationWithSnakeCaseFormatting_ShouldReturnSuccess()
    {
        // Arrange
        var auditEntry = new AuditEntry
        {
            TableName = "Table",
            ColumnName = "ColumnName",
            Action = AuditAction.Delete,
            UserId = Guid.NewGuid(),
            KeyValues = { { "Id", 1 } },
            OldValues = { { "ColumnName", "OldValue" } }
        };

        // Act
        var result = auditEntry.ToActionLog(Formatting.SnakeCase);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("table", result.Value.ReferenceTable);
        Assert.AreEqual("column_name", result.Value.ColumnName);
        Assert.AreEqual(
            JsonSerializer.Serialize(new { Id = 1, ColumnName = "OldValue" }, _snakeCaseJsonSerializerOptions),
            result.Value.RecordValue);
        Assert.IsNull(result.Value.ColumnOldValue);
        Assert.IsNull(result.Value.ColumnNewValue);
        Assert.AreEqual(auditEntry.Action, result.Value.ActionType);
        Assert.AreEqual(auditEntry.UserId, result.Value.CreatedBy);
        Assert.AreNotEqual(default(DateTime), result.Value.CreatedDate);
    }

    [Test]
    public void HasTemporaryProperties_WhenEmpty_ShouldReturnFalse()
    {
        // Arrange
        var auditEntry = new AuditEntry { TableName = "Product" };

        // Act & Assert
        Assert.IsFalse(auditEntry.HasTemporaryProperties);
    }

    [Test]
    public void HasTemporaryProperties_WhenPropertyAdded_ShouldReturnTrue()
    {
        // Arrange
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(connection)
            .Options;

        using var context = new TestDbContext(options);
        context.Database.EnsureCreated();

        // Create and track an entity to get a real PropertyEntry
        var category = new Category { Name = "Test" };
        context.Categories.Add(category);
        context.SaveChanges();

        // Modify the category to get a changed property entry
        category.Name = "Updated";
        var entry = context.Entry(category);
        var propertyEntry = entry.Property(p => p.Name);

        var auditEntry = new AuditEntry { TableName = "Category" };

        // Act
        auditEntry.TemporaryProperties.Add(propertyEntry);

        // Assert
        Assert.IsTrue(auditEntry.HasTemporaryProperties);
        Assert.AreEqual(1, auditEntry.TemporaryProperties.Count);
    }
}
