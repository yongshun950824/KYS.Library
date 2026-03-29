using System.Net;
using KYS.AspNetCore.Library.Helpers;
using KYS.AspNetCore.Library.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace KYS.AspNetCore.Library.Tests.HelpersUnitTests;

public class ViewResultHelperUnitTest
{
    [Test]
    public async Task ToViewResultAsync_OnSuccess_ShouldReturnTargetViewWithModel()
    {
        // Arrange
        var expectedData = new TestModel { Id = 1, Name = "Test" };
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonConvert.SerializeObject(expectedData))
        };

        var mockContext = MockHttpContext();

        // Act
        var result = await ViewResultHelper.ToViewResultAsync<TestModel>(
            mockContext,
            httpResponse,
            "SuccessView",
            "ErrorView"
        );

        // Assert
        Assert.IsInstanceOf<ViewResult>(result);
        Assert.AreEqual("SuccessView", result.ViewName);

        var model = result.ViewData.Model as ApiReponseModel<TestModel>;
        Assert.NotNull(model);
        Assert.AreEqual(HttpStatusCode.OK, model!.StatusCode);
        Assert.AreEqual(expectedData.Name, model!.Data.Name);
    }

    [Test]
    public async Task ToViewResultAsync_OnFailure_ShouldReturnErrorView()
    {
        // Arrange
        var errorData = new TestModel { Name = "Error Details" };
        var httpResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            ReasonPhrase = "Internal Server Error",
            Content = new StringContent(JsonConvert.SerializeObject(errorData))
        };

        var mockContext = MockHttpContext();

        // Act
        var result = await ViewResultHelper.ToViewResultAsync<TestModel>(
            mockContext,
            httpResponse,
            "SuccessView",
            "CustomErrorView"
        );

        // Assert
        Assert.AreEqual("CustomErrorView", result.ViewName);
        var model = result.ViewData.Model as ApiReponseModel<TestModel>;
        Assert.AreEqual(HttpStatusCode.InternalServerError, model!.StatusCode);
        Assert.AreEqual(httpResponse.ReasonPhrase, model!.ResponseMessage);
    }

    private HttpContext MockHttpContext()
    {
        var mockContext = new DefaultHttpContext();
        var metadataProvider = new EmptyModelMetadataProvider();
        var mockServiceProvider = new Mock<IServiceProvider>();
        mockServiceProvider
            .Setup(sp => sp.GetService(typeof(IModelMetadataProvider)))
            .Returns(metadataProvider);

        mockContext.RequestServices = mockServiceProvider.Object;

        return mockContext;
    }

    private class TestModel
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
    }
}
