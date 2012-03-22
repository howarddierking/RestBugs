using System.Collections.Generic;

namespace RestBugs.Services.Model
{
    public interface IBugRepository {
        IEnumerable<Bug> GetAll();
        Bug Get(int bugId);
        void Add(Bug bug);
    }
}