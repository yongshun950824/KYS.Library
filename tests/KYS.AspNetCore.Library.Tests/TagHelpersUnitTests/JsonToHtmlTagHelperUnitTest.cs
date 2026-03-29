using System.Text.Json;
using System.Text.RegularExpressions;
using KYS.AspNetCore.Library.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NUnit.Framework;

namespace KYS.AspNetCore.Library.Tests.TagHelpersUnitTests;

public class JsonToHtmlTagHelperUnitTest
{
    [Test]
    public async Task ProcessAsync_WithJsonObject_ShouldGenerateHtmlTable()
    {
        // Arrange
        var jsonInput = new People("Alice", 28);
        var helper = new JsonToHtmlTagHelper
        {
            Json = JsonSerializer.Serialize(jsonInput),
            AsBootstrapGrid = false,
            AsFlatten = false
        };

        var context = CreateTagHelperContext();
        var output = CreateTagHelperOutput("json-to-html");

        // Act
        await helper.ProcessAsync(context, output);

        // Assert
        var content = output.Content.GetContent();
        AssertContent(content,
            "<table",
            "</table>",
            $"<tr><th>{nameof(People.Name)}</th><td>{jsonInput.Name}</td></tr>",
            $"<tr><th>{nameof(People.Age)}</th><td>{jsonInput.Age}</td></tr>"
        );
    }

    [Test]
    public async Task ProcessAsync_WithBootstrapGrid_ShouldGenerateBootstrapGrid()
    {
        // Arrange
        var jsonInput = new People("Yuta", 16);
        var helper = new JsonToHtmlTagHelper
        {
            Json = JsonSerializer.Serialize(jsonInput),
            AsBootstrapGrid = true
        };

        var context = CreateTagHelperContext();
        var output = CreateTagHelperOutput("json-to-html");

        // Act
        await helper.ProcessAsync(context, output);

        // Assert
        var content = output.Content.GetContent();
        AssertContent(content,
            "<div class='container-fluid'>",
            "</div>",
            @$"<div class='row'>
                <div class='col-4 col-md-4'>{nameof(People.Name)}</div>
                <div class='col-8 col-md-8'>{jsonInput.Name}</div>
            </div>",
            @$"<div class='row'>
                <div class='col-4 col-md-4'>{nameof(People.Age)}</div>
                <div class='col-8 col-md-8'>{jsonInput.Age}</div>
            </div>"
        );
    }

    [Test]
    public async Task ProcessAsync_WithJsonArray_ShouldGenerateHtmlTable()
    {
        // Arrange
        var jsonInput = new People[]
        {
            new("Alice", 28),
            new("Yuta", 16)
        };
        var helper = new JsonToHtmlTagHelper
        {
            Json = JsonSerializer.Serialize(jsonInput)
        };

        var context = CreateTagHelperContext();
        var output = CreateTagHelperOutput("json-to-html");

        // Act
        await helper.ProcessAsync(context, output);

        // Assert
        var content = output.Content.GetContent();
        AssertContent(content,
            "<table border='1' class='table'><tr><th>Index</th><th>Value</th></tr>",
            "</table>",
            @$"<tr>
                <td>{0}</td>
                <td>
                    <table border='1' class='table'>
                        <tr><th>{nameof(People.Name)}</th><td>{jsonInput[0].Name}</td></tr>
                        <tr><th>{nameof(People.Age)}</th><td>{jsonInput[0].Age}</td></tr>
                    </table>
                </td>
            </tr>",
            @$"<tr>
                <td>{1}</td>
                <td>
                    <table border='1' class='table'>
                        <tr><th>{nameof(People.Name)}</th><td>{jsonInput[1].Name}</td></tr>
                        <tr><th>{nameof(People.Age)}</th><td>{jsonInput[1].Age}</td></tr>
                    </table>
                </td>
            </tr>"
        );
    }

    [Test]
    public async Task ProcessAsync_WithJsonArrayAndBootstrapGrid_ShouldGenerateBootstrapGrid()
    {
        // Arrange
        var jsonInput = new People[]
        {
            new("Alice", 28),
            new("Yuta", 16)
        };
        var helper = new JsonToHtmlTagHelper
        {
            Json = JsonSerializer.Serialize(jsonInput),
            AsBootstrapGrid = true
        };

        var context = CreateTagHelperContext();
        var output = CreateTagHelperOutput("json-to-html");

        // Act
        await helper.ProcessAsync(context, output);

        // Assert
        var content = output.Content.GetContent();
        AssertContent(content,
            "<div class='container-fluid'>",
            "</div>",
            @$"<div class='row'><div class='col-4 col-md-4'>{0}</div>
                <div class='col-8 col-md-8'>
                    <div class='row'>
                        <div class='col-4 col-md-4'>{nameof(People.Name)}</div>
                        <div class='col-8 col-md-8'>{jsonInput[0].Name}</div>
                    </div>
                    <div class='row'>
                        <div class='col-4 col-md-4'>{nameof(People.Age)}</div>
                        <div class='col-8 col-md-8'>{jsonInput[0].Age}</div>
                    </div>
                </div>
            </div>",
            @$"<div class='row'>
                <div class='col-4 col-md-4'>{1}</div>
                <div class='col-8 col-md-8'>
                    <div class='row'>
                        <div class='col-4 col-md-4'>{nameof(People.Name)}</div>
                        <div class='col-8 col-md-8'>{jsonInput[1].Name}</div>
                    </div>
                    <div class='row'>
                        <div class='col-4 col-md-4'>{nameof(People.Age)}</div>
                        <div class='col-8 col-md-8'>{jsonInput[1].Age}</div>
                    </div>
                </div>
            </div>"
        );
    }

    [Test]
    public async Task ProcessAsync_WithFlattenJsonArray_ShouldGenerateHtmlTable()
    {
        // Arrange
        var jsonInput = new People[]
        {
            new("Alice", 28),
            new("Yuta", 16)
        };
        var helper = new JsonToHtmlTagHelper
        {
            Json = JsonSerializer.Serialize(jsonInput),
            AsFlatten = true
        };

        var context = CreateTagHelperContext();
        var output = CreateTagHelperOutput("json-to-html");

        // Act
        await helper.ProcessAsync(context, output);

        // Assert
        var content = output.Content.GetContent();
        AssertContent(content,
            "<table border='1' class='table'>",
            "</table>",
            @$"<tr><th>[0].{nameof(People.Name)}</th><td>{jsonInput[0].Name}</td></tr>",
            @$"<tr><th>[0].{nameof(People.Age)}</th><td>{jsonInput[0].Age}</td></tr>",
            @$"<tr><th>[1].{nameof(People.Name)}</th><td>{jsonInput[1].Name}</td></tr>",
            @$"<tr><th>[1].{nameof(People.Age)}</th><td>{jsonInput[1].Age}</td></tr>"
        );
    }

    #region Helper method for TagHelper
    private TagHelperContext CreateTagHelperContext()
    {
        return new TagHelperContext(
            [],
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));
    }

    private TagHelperOutput CreateTagHelperOutput(string tagName)
    {
        return new TagHelperOutput(
            tagName,
            [],
            (useCachedResult, encoder) =>
                Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
    }
    #endregion

    #region Helper for method and test class 
    public static string NormalizeHtml(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;

        // 1. Convert all tabs and newlines to spaces
        // 2. Collapse multiple spaces into one
        // 3. Trim leading/trailing whitespace
        string clean = Regex.Replace(input, @"\s+", " ").Trim();

        // 4. Optional: Remove spaces between HTML tags (e.g., "> <" becomes "><")
        // This prevents tests from failing just because of indentation between <tr> and <td>
        clean = Regex.Replace(clean, @">\s+<", "><");

        return clean;
    }

    private static void AssertContent(string actualContent,
        string expectedStartContent,
        string expectedEndContent,
        params string[] expectedInnerContents)
    {
        // Normalize HTML as only validate the structure without beautify before asserting
        actualContent = NormalizeHtml(actualContent);
        expectedInnerContents = expectedInnerContents.Select(x => NormalizeHtml(x))
            .ToArray();

        Assert.True(actualContent.StartsWith(expectedStartContent));
        foreach (var innerContent in expectedInnerContents)
        {
            Assert.True(actualContent.Contains(innerContent));
        }

        Assert.True(actualContent.ReplaceLineEndings(String.Empty).EndsWith(expectedEndContent));
    }

    private record People(string Name, int Age);
    #endregion
}
