using System;
using NUnit.Framework;

namespace KYS.EFCore.Library.Tests;

public class EntityUnitTests
{
    [Test]
    public void AddDomainEvent_ShouldAddEventToDomainEventsList()
    {
        // Arrange
        var entity = new TestEntity();
        var notification = new TestNotification();

        // Act
        entity.AddDomainEvent(notification);

        // Assert
        Assert.IsNotNull(entity.DomainEvents);
        Assert.Contains(notification, entity.DomainEvents);
    }

    [Test]
    public void RemoveDomainEvent_ShouldRemoveEventFromDomainEventsList()
    {
        // Arrange
        var entity = new TestEntity();
        var notification = new TestNotification();
        entity.AddDomainEvent(notification);

        // Act
        entity.RemoveDomainEvent(notification);

        // Assert
        Assert.IsNotNull(entity.DomainEvents);
        Assert.IsFalse(entity.DomainEvents.Contains(notification));
    }

    [Test]
    public void RemoveDomainEvent_WithoutAddingEvent_ShouldNotThrowException()
    {
        // Arrange
        var entity = new TestEntity();
        var notification = new TestNotification();

        // Act & Assert
        Assert.DoesNotThrow(() => entity.RemoveDomainEvent(notification));
    }

    [Test]
    public void ClearDomainEvents_ShouldClearAllEventsFromDomainEventsList()
    {
        // Arrange
        var entity = new TestEntity();
        var notification1 = new TestNotification();
        var notification2 = new TestNotification();
        entity.AddDomainEvent(notification1);
        entity.AddDomainEvent(notification2);

        // Act
        entity.ClearDomainEvents();

        // Assert
        Assert.IsNotNull(entity.DomainEvents);
        Assert.IsEmpty(entity.DomainEvents);
    }

    [Test]
    public void ClearDomainEvents_WithoutAddingEvents_ShouldNotThrowException()
    {
        // Arrange
        var entity = new TestEntity();

        // Act & Assert
        Assert.DoesNotThrow(() => entity.ClearDomainEvents());
    }

    [Test]
    public void IsTransient_WithNewEntity_ShouldReturnTrue()
    {
        // Arrange & Act
        var e = new TestEntity();

        // Assert
        Assert.IsTrue(e.IsTransient());
    }

    [Test]
    public void IsTransient_WithSetId_ShouldReturnFalse()
    {
        // Arrange & Act
        var e = new TestEntity();
        e.SetId(1);

        // Assert
        Assert.IsFalse(e.IsTransient());
    }

    [Test]
    public void Equals_WithNonEntityObject_ShouldReturnFalse()
    {
        // Arrange
        var e1 = new TestEntity();
        e1.SetId(1);
        var nonEntityObject = new object();

        // Act & Assert
        Assert.IsFalse(e1.Equals(nonEntityObject));
    }

    [Test]
    public void Equals_WithEntityAsObject_ShouldReturnTrue()
    {
        // Arrange
        var e1 = new TestEntity();
        e1.SetId(1);
        var e2 = new TestEntity();
        e2.SetId(1);

        // Act & Assert
        Assert.IsTrue(e1.Equals((object)e2));
    }

    [Test]
    public void Equals_WithSameTypeAndId_ShouldReturnTrue()
    {
        // Arrange
        var e1 = new TestEntity();
        e1.SetId(1);
        var e2 = new TestEntity();
        e2.SetId(1);

        // Act & Assert
        Assert.IsTrue(e1.Equals(e2));
    }

    [Test]
    public void Equals_WithDifferentIds_ShouldReturnFalse()
    {
        // Arrange
        var e1 = new TestEntity();
        e1.SetId(1);
        var e2 = new TestEntity();
        e2.SetId(2);

        // Act & Assert
        Assert.IsFalse(e1.Equals(e2));
    }

    [Test]
    public void Equals_WithSameIdButDifferentTypes_ShouldReturnFalse()
    {
        // Arrange
        var e1 = new TestEntity();
        e1.SetId(1);
        var e2 = new OtherEntity();
        e2.SetId(1);

        // Act & Assert
        Assert.IsFalse(e1.Equals(e2));
    }

    [Test]
    public void Equals_WithSameInstance_ShouldReturnTrue()
    {
        // Arrange
        var e1 = new TestEntity();
        e1.SetId(1);
        var e2 = e1;

        // Act & Assert
        Assert.IsTrue(e1.Equals(e2));
    }

    [Test]
    public void Equals_WithCurrentTransientEntity_ShouldReturnFalse()
    {
        // Arrange
        var e1 = new TestEntity();
        var e2 = new TestEntity();
        e2.SetId(1);

        // Act & Assert
        Assert.IsFalse(e1.Equals(e2));
    }

    [Test]
    public void Equals_WithOtherTransientEntity_ShouldReturnFalse()
    {
        // Arrange
        var e1 = new TestEntity();
        e1.SetId(1);
        var e2 = new TestEntity();

        // Act & Assert
        Assert.IsFalse(e1.Equals(e2));
    }

    [Test]
    public void GetHashCode_ShouldReturnHashCode()
    {
        // Arrange
        var e = new TestEntity();
        e.SetId(1);

        // Act
        var hashCode = e.GetHashCode();

        // Assert
        Assert.AreEqual(e.Id.GetHashCode() ^ 31, hashCode);
    }

    [Test]
    public void RelationalOperators_Equal_WithEqualEntities_ShouldReturnTrue()
    {
        // Arrange
        var e1 = new TestEntity();
        e1.SetId(1);
        var e2 = new TestEntity();
        e2.SetId(1);

        // Act & Assert
        Assert.IsTrue(e1 == e2);
    }

    [Test]
    public void RelationalOperators_Equal_WithDifferentEntities_ShouldReturnFalse()
    {
        // Arrange
        var e1 = new TestEntity();
        e1.SetId(1);
        var e2 = new TestEntity();
        e2.SetId(2);

        // Act & Assert
        Assert.IsFalse(e1 == e2);
    }

    [Test]
    public void RelationalOperators_Equal_WithNullEntities_ShouldReturnTrue()
    {
        // Arrange
        TestEntity? e1 = null;
        TestEntity? e2 = null;

        // Act & Assert
        Assert.IsTrue(e1 == e2);
    }

    [Test]
    public void RelationalOperators_Equal_WithOneNullEntity_ShouldReturnFalse()
    {
        // Arrange
        TestEntity? e1 = null;
        var e2 = new TestEntity();
        e2.SetId(1);

        // Act & Assert
        Assert.IsFalse(e1 == e2);
    }

    [Test]
    public void RelationalOperators_NotEqual_WithEqualEntities_ShouldReturnFalse()
    {
        // Arrange
        var e1 = new TestEntity();
        e1.SetId(1);
        var e2 = new TestEntity();
        e2.SetId(1);

        // Act & Assert
        Assert.IsFalse(e1 != e2);
    }
}
