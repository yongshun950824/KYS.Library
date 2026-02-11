using System.Net;
using KYS.AspNetCore.Library.Models;
using Newtonsoft.Json;
using NUnit.Framework;

namespace KYS.AspNetCore.Library.Tests.ModelsUnitTests;

public class ApiResponseModelUnitTest
{
    [Test]
    public void Constructor_WithStatusCode_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var status = HttpStatusCode.NoContent;

        // Act
        var model = new ApiReponseModel<string>(status);

        // Assert
        Assert.AreEqual(status, model.StatusCode);
        Assert.IsNull(model.Data);
        Assert.IsNull(model.ResponseMessage);
    }

    [Test]
    public void Constructor_WithStatusCodeAndData_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var status = HttpStatusCode.OK;
        var data = "Test Data";

        // Act
        var model = new ApiReponseModel<string>(status, data: data);

        // Assert
        Assert.AreEqual(status, model.StatusCode);
        Assert.AreEqual(data, model.Data);
        Assert.IsNull(model.ResponseMessage);
    }

    [Test]
    public void Constructor_WithStatusCodeAndResponseMessage_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var status = HttpStatusCode.InternalServerError;
        var message = "Internal Server Error";

        // Act
        var model = new ApiReponseModel<string>(status, responseMessage: message);

        // Assert
        Assert.AreEqual(status, model.StatusCode);
        Assert.IsNull(model.Data);
        Assert.AreEqual(message, model.ResponseMessage);
    }

    [Test]
    public void Constructor_WithThreeParams_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var status = HttpStatusCode.BadRequest;
        var message = "Bad Request";
        var data = new ErrorModel
        {
            Errors = new Dictionary<string, string>
            {
                { "name", "Name is required!" }
            }
        };

        // Act
        var model = new ApiReponseModel<ErrorModel>(status, message, data);

        // Assert
        Assert.AreEqual(status, model.StatusCode);
        Assert.AreEqual(JsonConvert.SerializeObject(data), JsonConvert.SerializeObject(model.Data));
        Assert.AreEqual(message, model.ResponseMessage);
    }

    private class ErrorModel
    {
        public required Dictionary<string, string> Errors { get; set; }
    }
}
