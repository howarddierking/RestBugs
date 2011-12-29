using System.Text;
using System.IO;

namespace RestBugs.Services.Infrastructure
{
    /// <summary>
    /// Adapted from: http://blog.andrewnurse.net/2010/11/16/HostingRazorOutsideOfASPNetRevisedForMVC3RC.aspx
    /// </summary>
    public abstract class TemplateBase
    {
        public StringBuilder Buffer { get; set; }
        public StringWriter Writer { get; set; }

        public TemplateBase()
        {
            Buffer = new StringBuilder();
            Writer = new StringWriter(Buffer);
        }

        public abstract void Execute();

        public virtual void Write(object value)
        {
            WriteLiteral(value);
        }

        public virtual void WriteLiteral(object value)
        {
            Buffer.Append(value);
        }

        public dynamic Model { get; set; }

        public string FormatXmlForHtml(string text)
        {
            var result = System.Web.HttpUtility.HtmlEncode(text);
            return result;
            //return result.Replace("\n", "<br/>&nbsp&nbsp&nbsp&nbsp&nbsp");//.Replace(" ", "&nbsp&nbsp&nbsp&nbsp&nbsp");
        }

        string FormattedLink(LinkInfo linkData, string rel) {
            return string.Format("<a href=\"{0}\" rel=\"{1}\">{2}</a>",
                    linkData.Link,
                    rel,
                    linkData.Description);
        }

        public string LinkFromEntity(string namedChild, object entity, string rel)
        {
            if (entity != null && entity is ILinkProvider)
            {
                var linkData = ((ILinkProvider)entity).GetLinkInfo(namedChild);
                return FormattedLink(linkData, rel);
            }

            return null;
        }

        public string LinkFromEntity(object entity, string rel)
        {
            if (entity != null && entity is ILinkProvider)
            {
                var linkData = ((ILinkProvider)entity).GetLinkInfo();
                return FormattedLink(linkData, rel);

            }

            return null;
        }
    }
}