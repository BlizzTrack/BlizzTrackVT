using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BTSharedCore.Services;
using MongoDB.Driver;

namespace BTSharedCore.Data
{
    public abstract class AbstractData<T>
    {
        protected IMongoCollection<T> Collection;
        protected AbstractData(Mongo database)
        {
            Collection = database.Collection<T>();
        }

        public abstract Task<T> Get(string product, int seqn);
        public abstract Task<T> Latest(string product);
        public abstract Task<T> Previous(string product, int current);
        public abstract Task Insert(params T[] item);
    }
}
