using KYS.Library.Extensions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace KYS.Library.Tests.ExtensionsUnitTests
{
    internal class IQueryableExtensionsUnitTest
    {
        private List<Vehicle> _vehicles;

        [SetUp]
        public void Setup()
        {
            _vehicles = new List<Vehicle>
            {
                new Vehicle("Toyota", "Camry", 1800, VehicleType.Car),
                new Vehicle("Yamaha", "Z15R", 1500, VehicleType.Bike),
                new Vehicle("Scania", "DC07", 6700, VehicleType.Truck)
            };
        }

        [Test]
        public void WhereIf_WithPredicateIsFalse_ShouldReturnFullList()
        {
            // Arrange
            bool isCarOnly = false;

            // Act
            var filteredVehicle = _vehicles.AsQueryable()
                .WhereIf(isCarOnly, x => x.VehicleType == VehicleType.Car)
                .ToList();

            // Assert
            Assert.AreEqual(_vehicles.Count, filteredVehicle.Count);
        }

        [Test]
        public void WhereIf_WithPredicateIsTrue_ShouldReturnFilteredList()
        {
            // Arrange
            bool isCarOnly = true;
            var expectedVehicles = _vehicles.AsQueryable()
                .Where(x => x.VehicleType == VehicleType.Car)
                .ToList();

            // Act
            var filteredVehicle = _vehicles.AsQueryable()
                .WhereIf(isCarOnly, x => x.VehicleType == VehicleType.Car)
                .ToList();

            // Assert
            Assert.AreEqual(expectedVehicles.Count, filteredVehicle.Count);
        }

        private record Vehicle(string Brand, string ModelName, int EngineCc, VehicleType VehicleType);

        enum VehicleType
        {
            Car,
            Truck,
            Bike
        }
    }
}
