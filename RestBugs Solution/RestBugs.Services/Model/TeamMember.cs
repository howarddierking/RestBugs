namespace RestBugs.Services.Model
{
    public class TeamMember : ILinkProvider {
        public string Name { get; set; }

        public int Id { get; set; }

        public LinkInfo GetLinkInfo(string namedChild = null) {
            if (namedChild == "bugs")
                return new LinkInfo(string.Format("/team/{0}/bugs", Id), string.Format("{0}'s Bugs", Name));
            return new LinkInfo(string.Format("/team/{0}", Id), Name);
        }
    }
}