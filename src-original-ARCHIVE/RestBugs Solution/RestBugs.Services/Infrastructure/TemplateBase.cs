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
    }
}