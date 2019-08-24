using MorseCode.ITask;
using System;
using System.Threading.Tasks;

namespace GraphQlSchema
{
    public interface IResolver<out T>
    {
        ITask<T> Resolve();
    }
}
