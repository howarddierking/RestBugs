namespace RestBugs.Services
{
    public interface ILinkProvider
    {
        LinkInfo GetLinkInfo(string namedChild = null);
    }
}