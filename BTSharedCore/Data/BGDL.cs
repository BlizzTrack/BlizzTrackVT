using System.Threading.Tasks;
using BTSharedCore.Services;
using MongoDB.Driver;

namespace BTSharedCore.Data
{
    public class BGDL : AbstractData<Models.BGDL>
    {
        public BGDL(Mongo database) : base(database)
        {
        }

        public override async Task<Models.BGDL> Get(string product, int seqn)
        {
            return await Collection.Find(x => x.Product.ToLower() == product.ToLower() && x.Seqn == seqn).FirstOrDefaultAsync();
        }

        public override async Task<Models.BGDL> Latest(string product)
        {
            return await Collection.Find(x => x.Product.ToLower() == product.ToLower()).SortByDescending(x => x.Id).FirstOrDefaultAsync();
        }

        public override async Task<Models.BGDL> Previous(string product, int current)
        {
            return await Collection.Find(x => x.Product.ToLower() == product.ToLower() && x.Seqn != current).SortByDescending(x => x.Id).FirstOrDefaultAsync();
        }

        public override async Task Insert(params Models.BGDL[] item)
        {
            await Collection.InsertManyAsync(item);
        }
    }
}
