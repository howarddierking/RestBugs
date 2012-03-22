using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;

namespace RestBugs.ConsoleClient
{
    class BugsMediaTypeConstants
    {
        public const string ID_FORMS = "forms";
        public const string ID_BUGS = "bugs";
        public const string CLASS_ALL = "all";
        public const string ID_LINKS = "links";
        public const string CLASS_NEW = "new";
        public const string CLASS_MOVE = "move";
        public const string CLASS_NEXT = "next";
        public const string CLASS_TITLE = "title";
        public const string CLASS_DESCRIPTION = "description";
        public const string REL_BACKLOG = "backlog";
        public const string ENTRY_PATH = "bugs";
    }

    class Program
    {
        // for bookmarks, I may want to have a bookmarking service that is a map of rel to url
        // can create a client cache that is even persistable - talk about this in the section on self-describing messages (control data)

        static HttpClient _client;

        static void Main() {
            var clientHandler = new HttpClientHandler();
            //_client = new HttpClient(clientHandler) { BaseAddress = new Uri("http://ipv4.fiddler:9200") };
            _client = new HttpClient(clientHandler) { BaseAddress = new Uri("http://localhost:9200") };
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

            //loop is as follows
            //----------------------
            //get a representation
            //display represenation
            //process a command (depends on the type of command [a|form])
            //get another represenation
            //...

            Console.WriteLine("Welcome to RESTBugs \"on command\"!\n");
            Console.WriteLine("Enter a command per the instructions below, or enter 'q' to quit.");

            Go();
        }

        static void Go() {
            var resp = _client.GetAsync(BugsMediaTypeConstants.ENTRY_PATH).Result;
            //TODO: factory logic to select the correct represenation impl based on content-type header
            IRepresentation representation = new HtmlBugRepresentation(resp);
            representation.Display();

            while(true) {
                try {
                    Console.Write("\nCommand: ");
                    var command = Console.ReadLine();
                    if (command == "q")
                        break;

                    var newReq = representation.ProcessCommand(command);

                    var newResp = _client.SendAsync(newReq).Result;

                    //add logic to check for failure codes before automatically overwriting the context variable
                    resp = newResp;
                    representation = new HtmlBugRepresentation(resp);
                    representation.Display();
                }
                catch(Exception ex) {
                    Console.WriteLine("ERROR: " + ex.Message);
                }
            }
        }
    }

    class HtmlBugRepresentation : IRepresentation
    {
        public static Regex CommandParamsRegex = new Regex(
            "\"(?<param>[\\w\\s]+)\"|(?<param>\\w+)",
            RegexOptions.CultureInvariant
            | RegexOptions.Compiled);

        readonly XDocument _document;

        public HtmlBugRepresentation(HttpResponseMessage resp) {
            _document = XDocument.Load(resp.Content.ReadAsStreamAsync().Result);
        }

        public void Display() {
            DisplayCommands();
            DisplayNavigation();
            DisplayBugs();
        }

        void DisplayBugs() {
            Console.WriteLine("\nAvailable Bugs");
            Console.WriteLine("To run one of the available commands for an item,\nenter 'bug' [command name] [item number]");
            Console.WriteLine("An asterisk next to a command indicates that command as the optimum workflow path");
            Console.WriteLine("-------------------------------------------------\n");
            Console.WriteLine("\tTitle\tDescription\t\t\tAvailable Commands");
            Console.WriteLine("\t-----\t-----------\t\t\t------------------");

            var bugElements =
                _document.XPathSelectElements(String.Format("//div[@id='{0}']/ul[@class='{1}']/li",
                                                                 BugsMediaTypeConstants.ID_BUGS,
                                                                 BugsMediaTypeConstants.CLASS_ALL));
            var c = 0;
            foreach (var bugElement in bugElements)
            {
                var titleEl = bugElement.XPathSelectElement(String.Format("span[@class='{0}']", BugsMediaTypeConstants.CLASS_TITLE));
                var descEl = bugElement.XPathSelectElement(String.Format("span[@class='{0}']", BugsMediaTypeConstants.CLASS_DESCRIPTION));
                var transitions = bugElement.XPathSelectElements(String.Format("form[contains(@class, '{0}')]", BugsMediaTypeConstants.CLASS_MOVE));

                var commands = CreateCommandsString(transitions);

                Console.WriteLine("[{0}]\t{1}\t{2}\t{3}",
                    c,
                    titleEl == null ? String.Empty : titleEl.Value,
                    descEl == null ? String.Empty : descEl.Value,
                    commands);
                ++c;
            }
        }

        string CreateCommandsString(IEnumerable<XElement> transitions) {
            //don't display the 'next' hint class
            var sb = new StringBuilder();
            foreach (var transition in transitions)
            {
                var classAttr = transition.Attribute("class").Value;
                if (classAttr.Contains(BugsMediaTypeConstants.CLASS_NEXT))
                    classAttr = "*" + classAttr.Replace(BugsMediaTypeConstants.CLASS_NEXT, String.Empty).Trim();
                
                sb.Append(", " + classAttr);
            }
            sb.Remove(0, 1);
            return sb.ToString();
        }

        void DisplayNavigation() {
            Console.WriteLine("\nAvailable Links");
            Console.WriteLine("Enter 'link [resource]' to navigate");
            Console.WriteLine("----------------------------------------");
            
            var navigationElements = _document.XPathSelectElements(String.Format("//div[@id='{0}']/a", BugsMediaTypeConstants.ID_LINKS));
            foreach (var navigationElement in navigationElements) {
                Console.WriteLine("{0}", navigationElement.Attribute("rel").Value);
            }
        }

        void DisplayCommands() {
            Console.WriteLine("\nAvailable Forms");
            Console.WriteLine("Enter 'form [command name] [param1 [paramN]]' to execute.");
            Console.WriteLine("---------------------------------------------------------");
            var formElements = _document.XPathSelectElements(String.Format("//div[@id='{0}']/form", BugsMediaTypeConstants.ID_FORMS));
            foreach (var formElement in formElements) {
                var className = formElement.Attribute("class").Value;
                //get text inputs 
                //NOTE: this part would be more complicated depending on the media type
                var dataTransferElements = formElement.XPathSelectElements("input[@type='text']");
                var inputsSb = new StringBuilder();
                foreach (var dataTransferElement in dataTransferElements) {
                    var name = dataTransferElement.Attribute("name").Value;
                    inputsSb.AppendFormat("[{0}] ", name);
                }
                Console.WriteLine("\"{0}\" {1}", className, inputsSb.ToString().Trim());
            }
        }

        public HttpRequestMessage ProcessCommand(string command) {
            if (command.StartsWith("bug", StringComparison.InvariantCultureIgnoreCase))
                return CreateBugRequest(command.Substring(4));
            if (command.StartsWith("link", StringComparison.InvariantCultureIgnoreCase))
                return CreateLinkRequest(command.Substring(5));
            if (command.StartsWith("form", StringComparison.InvariantCultureIgnoreCase))
                return CreateFormRequest(command.Substring(5));
            throw new ArgumentException("Unknown command.");
        }

        HttpRequestMessage CreateFormRequest(string formCommand) {
            var parameters = ParseParamters(formCommand);

            //get the form element referenced by the command
            var formElement =
                _document.XPathSelectElement(string.Format("//div[@id='{0}']/form[contains(@class, '{1}')]",
                                                           BugsMediaTypeConstants.ID_FORMS,
                                                           parameters[0]));
            var action = formElement.Attribute("action").Value;

            //set the values of the form's data transfer elements based on the parameters provided from the command line
            //this logic, while limited, replicates the logic in 'DisplayCommands' so at this point, consistentcy is best
            var paramsIter = 1;
            var dataTransferInputElements = formElement.XPathSelectElements("input[@type='text']");
            foreach (var dataTransferElement in dataTransferInputElements) {
                dataTransferElement.Attribute("value").SetValue(parameters[paramsIter++]);
            }

            var content = GetFormUrlEncodedDataFrom(formElement);
            var newReq = new HttpRequestMessage(HttpMethod.Post, action) {Content = content};
            return newReq;
        }

        FormUrlEncodedContent GetFormUrlEncodedDataFrom(XElement formElement) {
            //populate the kvp structure
            //yes, I realize that this is innefficient - will clean up
            var data = new List<KeyValuePair<string, string>>();
            var dataTransferElements = formElement.XPathSelectElements("input");
            foreach (var dataTransferElement in dataTransferElements)
            {
                var kvp = new KeyValuePair<string, string>(
                    dataTransferElement.Attribute("name").Value,
                    dataTransferElement.Attribute("value").Value);
                data.Add(kvp);
            }

            var content = new FormUrlEncodedContent(data);
            return content;
        }

        List<string> ParseParamters(string args) {
            var matches = CommandParamsRegex.Matches(args);

            var parameters = new List<string>();
            foreach (Match match in matches) {
                parameters.Add(match.Groups["param"].Captures[0].Value);
            }
            
            return parameters;
        }

        HttpRequestMessage CreateLinkRequest(string linkCommand) {
            //get the element specified by the command
            var specifiedLinkElement =
                _document.XPathSelectElement(String.Format("//div[@id='{0}']/a[@rel='{1}']",
                                                           BugsMediaTypeConstants.ID_LINKS, linkCommand));
            if(specifiedLinkElement==null)
                throw new Exception("Cannot find a link with rel=" + linkCommand);

            var linkValue = specifiedLinkElement.Attribute("href").Value;
            var newReq = new HttpRequestMessage(HttpMethod.Get, linkValue);
            return newReq;
        }

        HttpRequestMessage CreateBugRequest(string bugCommand) {
            var parameters = ParseParamters(bugCommand);
            var className = parameters[0];
            var bugnumber = int.Parse(parameters[1]);  //note this isn't the same as bug id - just the position of the bug in the current representation

            var formElement =
                _document.XPathSelectElement(
                    string.Format("//div[@id='{0}']/ul[@class='{1}']/li[{2}]/form[contains(@class, '{3}')]",
                                  BugsMediaTypeConstants.ID_BUGS,
                                  BugsMediaTypeConstants.CLASS_ALL,
                                  bugnumber + 1,
                                  className));

            var action = formElement.Attribute("action").Value;
            var newReq = new HttpRequestMessage(HttpMethod.Post, action)
                         {Content = GetFormUrlEncodedDataFrom(formElement)};
            return newReq;
        }
    }

    interface IRepresentation
    {
        void Display();
        HttpRequestMessage ProcessCommand(string command);
    }
}