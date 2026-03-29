using System.Text;
using KYS.Library.Helpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json.Linq;

namespace KYS.AspNetCore.Library.TagHelpers
{
    /// <summary>
    /// A tag helper that converts JSON (Object &amp; Array) into Table. <br />
    /// <list type="bullet">
    /// <item>Support display data in flatten form.</item>
    /// <item>Support generated HTML as a <c>&lt;table&gt;</c> element or Bootstrap Grid.</item>
    /// </list>
    /// </summary>
    [HtmlTargetElement("json-to-html", Attributes = "json", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class JsonToHtmlTagHelper : TagHelper
    {
        /// <summary>
        /// Gets or sets the serialized JSON object/array.
        /// </summary>
        [HtmlAttributeName("json")]
        public string Json { get; set; }

        /// <summary>
        /// Gets or sets the indicator to flatten the JSON.
        /// </summary>
        [HtmlAttributeName("asFlatten")]
        public bool AsFlatten { get; set; }

        /// <summary>
        /// Gets or sets the indicator to generate the HTML content as Bootstrap Grid.
        /// </summary>
        [HtmlAttributeName("asBootstrapGrid")]
        public bool AsBootstrapGrid { get; set; }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagMode = TagMode.StartTagAndEndTag;

            JToken jToken = JToken.Parse(Json);
            if (AsFlatten)
            {
                Dictionary<string, object> flattenDict = JsonHelper.Flatten(jToken);
                jToken = JToken.FromObject(flattenDict);
            }

            string html = String.Empty;
            if (AsBootstrapGrid)
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("<div class='container-fluid'>");
                sb.AppendLine(GenerateHtmlWithBootstrapGrid(jToken));
                sb.AppendLine("</div>");

                html = sb.ToString();
            }
            else
            {
                html = GenerateHtml(jToken);
            }

            output.Content.AppendHtml(html);

            return Task.CompletedTask;
        }

        public static string GenerateHtml(JToken token)
        {
            StringBuilder sb = new StringBuilder();

            if (token.Type == JTokenType.Object)
            {
                sb.AppendLine("<table border='1' class='table'>");
                foreach (var property in token.Children<JProperty>())
                {
                    sb.AppendLine($"<tr><th>{property.Name}</th><td>{GenerateHtml(property.Value)}</td></tr>");
                }
                sb.AppendLine("</table>");

                return sb.ToString();
            }
            else if (token.Type == JTokenType.Array)
            {
                sb.AppendLine("<table border='1' class='table'><tr><th>Index</th><th>Value</th></tr>");
                int index = 0;
                foreach (var item in token.Children())
                {
                    sb.AppendLine($"<tr><td>{index++}</td><td>{GenerateHtml(item)}</td></tr>");
                }
                sb.AppendLine("</table>");

                return sb.ToString();
            }
            else
            {
                return token.ToString();
            }
        }

        public static string GenerateHtmlWithBootstrapGrid(JToken token)
        {
            StringBuilder sb = new StringBuilder();

            if (token.Type == JTokenType.Object)
            {
                foreach (var property in token.Children<JProperty>())
                {
                    sb.AppendLine("<div class='row'>");
                    sb.AppendLine($"<div class='col-4 col-md-4'>{property.Name}</div><div class='col-8 col-md-8'>{GenerateHtmlWithBootstrapGrid(property.Value)}</div>");
                    sb.AppendLine("</div>");
                }

                return sb.ToString();
            }
            else if (token.Type == JTokenType.Array)
            {
                int index = 0;
                foreach (var item in token.Children())
                {
                    sb.AppendLine("<div class='row'>");
                    sb.AppendLine($"<div class='col-4 col-md-4'>{index++}</div><div class='col-8 col-md-8'>{GenerateHtmlWithBootstrapGrid(item)}</div>");
                    sb.AppendLine("</div>");
                }

                return sb.ToString();
            }
            else
            {
                return token.ToString();
            }
        }
    }
}
