namespace RestBugs.Services
{
    public class LinkInfo
    {
        public LinkInfo(string link, string description) {
            Link = link;
            Description = description;
        }

        public string Link { get; set; }

        public string Description { get; set; }
    }
}