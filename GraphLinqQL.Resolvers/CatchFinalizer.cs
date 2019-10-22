using GraphLinqQL.Resolution;
using System;
using System.Threading.Tasks;

namespace GraphLinqQL
{
    internal class CatchFinalizer : IFinalizer
    {
        private readonly FieldContext fieldContext;
        private readonly Func<object> valueAccessor;

        internal CatchFinalizer(FieldContext fieldContext, Func<object> valueAccessor)
        {
            this.fieldContext = fieldContext;
            this.valueAccessor = valueAccessor;
        }

        public async Task<object?> GetValue(FinalizerContext context)
        {
            try
            {
                return await context.UnrollResults(valueAccessor(), context).ConfigureAwait(false);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                var errors = ex.HasGraphQlErrors(out var e) ? e : new[] { new GraphQlError(WellKnownErrorCodes.UnhandledError) };
                foreach (var error in errors)
                {
                    error.Fixup(fieldContext);
                }
                // TODO - log the exception
                context.Errors.AddRange(errors);
                return null;
            }
        }
    }
}
