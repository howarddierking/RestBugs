using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml;

namespace RestBugs.ConsoleClient
{
    class Program
    {
        static HttpClient client;

        static void Main(string[] args) {
            string baseUri = "http://localhost:8800";
            InitializeClient();

            Start(baseUri);
            Console.ReadKey();
        }

        static void Start(string baseUri) {
            while (true) {
                Console.WriteLine("Retrieving list of reports\r\n");
                XmlDocument document = GetDocument(baseUri);

                XmlNodeList reports = OutputReports(document);

                Console.WriteLine("\r\nChoose a report (1 - {0})", reports.Count);
                ConsoleKeyInfo info = Console.ReadKey();

                int selection = int.Parse(info.KeyChar.ToString());
                string uri = string.Format("{0}/{1}", baseUri, reports[selection - 1].Attributes["href"].Value);
                Console.WriteLine("\r\n" + uri + "\r\n");

                document = GetDocument(uri);
                OutputBugs(document);

                Console.WriteLine("\r\nCommand:\r\n");
                Console.WriteLine("{Bug #} r - Resolve");
                Console.WriteLine("b - Back\r\n");

                string command = Console.ReadLine();
                HandleBugCommand(baseUri, document, command);
            }
        }

        static void HandleBugCommand(string baseUri, XmlDocument document, string command) {
            if (command == "b") {
                Start(baseUri);
                return;
            }

            string[] items = command.Split(' ');
            int id = int.Parse(items[0]);
            XmlNode bug =
                document.SelectSingleNode("//tr[@class='bug-data']/td[@class='id'][.='" + id + "']").ParentNode;
            XmlNode form = bug.SelectSingleNode("td/form[@class='resolved']");

            var formPostValues = new Dictionary<string, string>();
            formPostValues["id"] = id.ToString();
            formPostValues["comments"] = "resolved";
            var content = new FormUrlEncodedContent(formPostValues);
            string href = form.Attributes["action"].InnerText;
            string uri = string.Format("{0}{1}", baseUri, href);
            client.PostAsync(uri, content).Wait();
        }

        static XmlNodeList OutputReports(XmlDocument document) {
            XmlNodeList reports = document.SelectNodes("//a[@rel='bugs']");

            for (int i = 1; i <= reports.Count; i++) {
                XmlNode node = reports[i - 1];
                Console.WriteLine("{0} - {1}", i, node.InnerText);
            }
            return reports;
        }

        static void OutputBugs(XmlDocument document) {
            XmlNodeList bugs = document.SelectNodes("//tr[@class='bug-data']");
            Console.WriteLine("ID\tName\tStatus\t\tPriority\tRank\tAssignedTo\r\n");
            foreach (XmlElement bug in bugs) {
                string id = bug.SelectSingleNode("td[@class='id']").InnerText.Trim();
                string name = bug.SelectSingleNode("td[@class='name']").InnerText.Trim();
                string status = bug.SelectSingleNode("td[@class='status']").InnerText.Trim();
                string priority = bug.SelectSingleNode("td[@class='priority']").InnerText.Trim();
                string rank = bug.SelectSingleNode("td[@class='rank']").InnerText.Trim();
                string assignedTo = bug.SelectSingleNode("td[@class='assignedTo']").InnerText.Trim();
                Console.WriteLine("{0}\t{1}\t{2}\t\t{3}\t\t{4}\t{5}", id, name, status, priority, rank, assignedTo);
            }
        }

        static XmlDocument GetDocument(string uri) {
            HttpResponseMessage response = client.GetAsync(uri).Result;
            var document = new XmlDocument();
            document.Load(response.Content.ReadAsStreamAsync().Result);
            return document;
        }

        static void InitializeClient() {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
        }
    }
}