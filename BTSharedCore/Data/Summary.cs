using System.Threading.Tasks;
using BTSharedCore.Services;
using MongoDB.Driver;

namespace BTSharedCore.Data
{
    public class Summary : AbstractData<Models.Summary>
    {
        public Summary(Mongo database) : base(database)
        {
        }

        public override async Task<Models.Summary> Get(string product, int seqn)
        {
            return await Collection.Find(x => x.Seqn == seqn).SortByDescending(x => x.Id).FirstOrDefaultAsync();
        }

        public override async Task<Models.Summary> Latest(string product = null)
        {
            return await Collection.Find(_ => true).SortByDescending(x => x.Id).FirstOrDefaultAsync();
        }

        public override async Task<Models.Summary> Previous(string product, int current)
        {
            return await Collection.Find(x => x.Seqn < current).SortByDescending(x => x.Id).FirstOrDefaultAsync();
        }

        public override async Task Insert(params Models.Summary[] item)
        {
            await Collection.InsertManyAsync(item);
        }
    }
}
