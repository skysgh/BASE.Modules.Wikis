using System.Collections;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace Tests.Modules.Wikis.Application.Helpers
{
    /// <summary>
    /// Wraps an in-memory <see cref="IQueryable{T}"/> with async enumeration and
    /// <see cref="IAsyncQueryProvider"/> support so EF Core async query operators
    /// can run against NSubstitute query seams in unit tests.
    /// </summary>
    internal sealed class TestAsyncQueryable<T> : IQueryable<T>, IAsyncEnumerable<T>
    {
        private readonly IQueryable<T> _inner;

        internal TestAsyncQueryable(IEnumerable<T> source)
        {
            this._inner = source.AsQueryable();
        }

        internal TestAsyncQueryable(IQueryable<T> source)
        {
            this._inner = source;
        }

        public Type ElementType => this._inner.ElementType;

        public Expression Expression => this._inner.Expression;

        public IQueryProvider Provider => new TestAsyncQueryProvider<T>(this._inner.Provider);

        public IEnumerator<T> GetEnumerator() => this._inner.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this._inner.GetEnumerator();

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => new TestAsyncEnumerator<T>(this._inner.GetEnumerator());
    }

    internal sealed class TestAsyncQueryProvider<T> : IQueryProvider, IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        internal TestAsyncQueryProvider(IQueryProvider inner)
        {
            this._inner = inner;
        }

        public IQueryable CreateQuery(Expression expression) => this._inner.CreateQuery(expression);

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            => new TestAsyncQueryable<TElement>(this._inner.CreateQuery<TElement>(expression));

        public object? Execute(Expression expression) => this._inner.Execute(expression);

        public TResult Execute<TResult>(Expression expression) => this._inner.Execute<TResult>(expression);

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            Type resultType = typeof(TResult);
            if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                Type elementType = resultType.GetGenericArguments()[0];
                object? result = this._inner.Execute(expression);
                return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))!
                    .MakeGenericMethod(elementType)
                    .Invoke(null, new[] { result })!;
            }

            return this._inner.Execute<TResult>(expression);
        }
    }

    internal sealed class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        internal TestAsyncEnumerator(IEnumerator<T> inner)
        {
            this._inner = inner;
        }

        public T Current => this._inner.Current;

        public ValueTask<bool> MoveNextAsync() => ValueTask.FromResult(this._inner.MoveNext());

        public ValueTask DisposeAsync()
        {
            this._inner.Dispose();
            return ValueTask.CompletedTask;
        }
    }

    internal static class TestAsyncQueryableExtensions
    {
        public static IQueryable<T> AsAsyncQueryable<T>(this IEnumerable<T> source)
        {
            return new TestAsyncQueryable<T>(source);
        }
    }
}
