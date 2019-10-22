using GraphLinqQL.Resolution;
using System;
using System.Threading.Tasks;

namespace GraphLinqQL
{
    public class TaskFinalizerFactory
    {
        private readonly FieldContext fieldContext;

        public TaskFinalizerFactory(FieldContext fieldContext)
        {
            this.fieldContext = fieldContext;
        }

        public IFinalizer Invoke(Func<Task> taskFactory)
        {
            return new TaskFinalizer(fieldContext, taskFactory);
        }
    }
}
