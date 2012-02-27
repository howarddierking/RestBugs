using System.Collections.Generic;

namespace RestBugs.Services.Model
{
    public interface IBugRepository {
        IEnumerable<Bug> GetAll();
        Bug Get(int bugId);
        void Add(Bug bug);
    }

    public interface IBugDtoRepository
    {
        IEnumerable<BugDTO> GetAll();
        BugDTO Get(int bugId);
        void Add(BugDTO bug);
    }
}