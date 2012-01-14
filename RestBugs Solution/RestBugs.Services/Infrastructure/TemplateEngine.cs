using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Razor;
using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.CSharp;
using System.IO;

namespace RestBugs.Services.Infrastructure
{
    /// <summary>
    /// This code adapted from: http://blog.andrewnurse.net/2010/11/16/HostingRazorOutsideOfASPNetRevisedForMVC3RC.aspx
    /// </summary>
    public class TemplateEngine
    {
        private static readonly RazorTemplateEngine Engine = InitializeTemplateEngine();
        private static readonly Dictionary<string, Type> ModelTemplateTypeCache = new Dictionary<string, Type>();
        private static readonly object CacheLock = new object();
        
        public TemplateBase CreateTemplateForType(Type modelType, string templateFile = null)
        {
            Type templateType;
            var templateKey = templateFile ?? modelType.Name;
            if (!ModelTemplateTypeCache.TryGetValue(templateKey, out templateType))
            {
                lock (CacheLock)
                {
                    if (!ModelTemplateTypeCache.TryGetValue(templateKey, out templateType))
                    {
                        templateType = InitializeTemplateType(templateKey);
                        ModelTemplateTypeCache.Add(templateKey, templateType);
                    }
                }
            }
            return Activator.CreateInstance(templateType) as TemplateBase;
        }

        private static Type InitializeTemplateType(string templateKey)
        {
            // Generate code for the template
            GeneratorResults razorResult;
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            var fileName = string.Format(@"Templates\{0}.cshtml", templateKey);
            var templateText = File.ReadAllText(Path.Combine(baseDirectory, fileName));
            using (TextReader rdr = new StringReader(templateText))
            {
                razorResult = Engine.GenerateCode(rdr);
            }

            var codeProvider = new CSharpCodeProvider();

            using (var sw = new StringWriter())
            {
                codeProvider.GenerateCodeFromCompileUnit(razorResult.GeneratedCode, sw, new CodeGeneratorOptions());
            }

            // Compile the generated code into an assembly
            string outputAssemblyName = Path.Combine(baseDirectory, String.Format("Temp_{0}.dll", Guid.NewGuid().ToString("N")));
            var compilerParameters = new CompilerParameters(new[] { typeof(TemplateEngine).Assembly.CodeBase.Replace("file:///", "").Replace("/", "\\") }, outputAssemblyName);
            compilerParameters.ReferencedAssemblies.Add("System.Core.dll");
            compilerParameters.ReferencedAssemblies.Add("Microsoft.CSharp.dll"); //<- needed to bind to dynamic type

            CompilerResults results = codeProvider.CompileAssemblyFromDom(compilerParameters, razorResult.GeneratedCode);

            if (results.Errors.HasErrors)
            {
                CompilerError err = results.Errors.OfType<CompilerError>().Where(ce => !ce.IsWarning).First();
                throw new InvalidOperationException(string.Format("Error Compiling Template: ({0}, {1}) {2}", err.Line, err.Column, err.ErrorText));
            }

            // Load the assembly
            Assembly asm = Assembly.LoadFrom(outputAssemblyName);
            if (asm == null)
            {
                throw new InvalidOperationException("Error loading template assembly");
            }
            // Get the template type
            return asm.GetType("RazorOutput.Template");
        }

        private static RazorTemplateEngine InitializeTemplateEngine()
        {
            // Set up the hosting environment
            // a. Use the C# language (you could detect this based on the file extension if you want to)
            var host = new RazorEngineHost(new CSharpRazorCodeLanguage())
            {
                DefaultBaseClass = typeof (TemplateBase).FullName,
                DefaultNamespace = "RazorOutput",
                DefaultClassName = "Template"
            };

            // b. Set the base class

            // c. Set the output namespace and type name

            // d. Add default imports
            host.NamespaceImports.Add("System");

            // Create the template engine using this host
            return new RazorTemplateEngine(host);
        }
    }
}