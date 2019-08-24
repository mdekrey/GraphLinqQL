using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MorseCode.ITask;

namespace GraphQlSchema
{
    class Resolver<T> : IResolver<T>
    {
        private Func<Task<T>> factory;

        public Resolver(Func<Task<T>> factory)
        {
            this.factory = factory;
        }

        public ITask<T> Resolve()
        {
            return factory().AsITask();
        }
    }

    public static class Resolver
    {
        public static IResolver<T> Of<T>(Func<T> factory)
        {
            return new Resolver<T>(() => Task.FromResult(factory()));
        }

        public static IResolver<T> Of<T>(Func<Task<T>> factory)
        {
            return new Resolver<T>(factory);
        }
    }
}
