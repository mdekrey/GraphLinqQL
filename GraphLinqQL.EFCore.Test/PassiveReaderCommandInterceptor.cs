using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace GraphLinqQL
{
#nullable disable warnings
    class PassiveReaderCommandInterceptor : IDbCommandInterceptor
    {
        private readonly ConcurrentDictionary<Guid, ConcurrentBag<string>> executedQueries = new ConcurrentDictionary<Guid, ConcurrentBag<string>>();

        public virtual InterceptionResult<DbCommand> CommandCreating(
            CommandCorrelatedEventData eventData,
            InterceptionResult<DbCommand> result)
        {
            return result;
        }

        public virtual DbCommand CommandCreated(
            CommandEndEventData eventData,
            DbCommand result)
        {
            return result;
        }

        public virtual InterceptionResult<DbDataReader> ReaderExecuting(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result)
        {
            Assert.False(eventData.IsAsync);
            RecordQuery(eventData?.Context?.ContextId.InstanceId, command.CommandText);

            return result;
        }

        public virtual InterceptionResult<object> ScalarExecuting(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<object> result)
        {
            Assert.False(eventData.IsAsync);
            RecordQuery(eventData?.Context?.ContextId.InstanceId, command.CommandText);

            return result;
        }

        public virtual InterceptionResult<int> NonQueryExecuting(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<int> result)
        {
            Assert.False(eventData.IsAsync);
            RecordQuery(eventData?.Context?.ContextId.InstanceId, command.CommandText);

            return result;
        }

        public virtual Task<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = default)
        {
            Assert.True(eventData.IsAsync);
            RecordQuery(eventData?.Context?.ContextId.InstanceId, command.CommandText);

            return Task.FromResult(result);
        }

        public virtual Task<InterceptionResult<object>> ScalarExecutingAsync(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<object> result,
            CancellationToken cancellationToken = default)
        {
            Assert.True(eventData.IsAsync);
            RecordQuery(eventData?.Context?.ContextId.InstanceId, command.CommandText);

            return Task.FromResult(result);
        }

        public virtual Task<InterceptionResult<int>> NonQueryExecutingAsync(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            Assert.True(eventData.IsAsync);
            RecordQuery(eventData?.Context?.ContextId.InstanceId, command.CommandText);

            return Task.FromResult(result);
        }

        public virtual DbDataReader ReaderExecuted(
            DbCommand command,
            CommandExecutedEventData eventData,
            DbDataReader result)
        {
            Assert.False(eventData.IsAsync);

            return result;
        }

        public virtual object ScalarExecuted(
            DbCommand command,
            CommandExecutedEventData eventData,
            object result)
        {
            Assert.False(eventData.IsAsync);

            return result;
        }

        public virtual int NonQueryExecuted(
            DbCommand command,
            CommandExecutedEventData eventData,
            int result)
        {
            Assert.False(eventData.IsAsync);

            return result;
        }

        public virtual Task<DbDataReader> ReaderExecutedAsync(
            DbCommand command,
            CommandExecutedEventData eventData,
            DbDataReader result,
            CancellationToken cancellationToken = default)
        {
            Assert.True(eventData.IsAsync);

            return Task.FromResult(result);
        }

        public virtual Task<object> ScalarExecutedAsync(
            DbCommand command,
            CommandExecutedEventData eventData,
            object result,
            CancellationToken cancellationToken = default)
        {
            Assert.True(eventData.IsAsync);

            return Task.FromResult(result);
        }

        public virtual Task<int> NonQueryExecutedAsync(
            DbCommand command,
            CommandExecutedEventData eventData,
            int result,
            CancellationToken cancellationToken = default)
        {
            Assert.True(eventData.IsAsync);

            return Task.FromResult(result);
        }

        public void CommandFailed(
            DbCommand command,
            CommandErrorEventData eventData)
        {
            Assert.False(eventData.IsAsync);
        }

        public Task CommandFailedAsync(
            DbCommand command,
            CommandErrorEventData eventData,
            CancellationToken cancellationToken = default)
        {
            Assert.True(eventData.IsAsync);

            return Task.CompletedTask;
        }

        public InterceptionResult DataReaderDisposing(
            DbCommand command,
            DataReaderDisposingEventData eventData,
            InterceptionResult result)
        {
            Assert.NotNull(eventData.DataReader);

            return result;
        }

        private void RecordQuery(Guid? contextId, string commandText)
        {
            if (contextId == null)
            {
                return;
            }
            var bag = this.executedQueries.GetOrAdd(contextId.Value, _ => new ConcurrentBag<string>());
            bag.Add(commandText);
        }

        internal IEnumerable<string> GetSql(Guid contextId)
        {
            return this.executedQueries.TryGetValue(contextId, out var bag) ? bag.ToArray() : Array.Empty<string>();
        }
    }
}
