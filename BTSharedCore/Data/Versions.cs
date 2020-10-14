using System;
using System.Threading.Tasks;
using BTSharedCore.Services;
using MongoDB.Driver;

namespace BTSharedCore.Data
{
    public class Versions : AbstractData<Models.Version>
    {
        public Versions(Mongo database) : base(database)
        {
        }

        public override async Task<Models.Version> Get(string product, int seqn)
        {
            return await Collection.Find(x => x.Product.ToLower() == product.ToLower() && x.Seqn == seqn).FirstOrDefaultAsync();
        }

        public override async Task<Models.Version> Latest(string product)
        {
            return await Collection.Find(x => x.Product.ToLower() == product.ToLower()).SortByDescending(x => x.Id).FirstOrDefaultAsync();
        }

        public override async Task Insert(params Models.Version[] item)
        {
            await Collection.InsertManyAsync(item);
        }
    }
}
