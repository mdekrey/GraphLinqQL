using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GraphLinqQL
{
    public interface IFinalizer
    {
        public Task<object?> GetValue(FinalizerContext context);
    }
}
