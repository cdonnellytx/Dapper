using BenchmarkDotNet.Attributes;
using Dapper.Database.Extensions;
using System.ComponentModel;
using System.Linq;


namespace Dapper.Tests.Performance.DapperDotDatabase
{
    /// <summary>
    /// Benchmarks for the <a href="https://github.com/dallasbeek/Dapper.Database">Dapper.Database</a> extension library built on top of Dapper.
    /// </summary>
    [Description("Dapper.Database")]
    public class DapperDotDatabaseBenchmarks : BenchmarkBase
    {
        [GlobalSetup]
        public void Setup()
        {
            BaseSetup();
        }

        [Benchmark(Description = "GetList<T>")]
        public Post GetList()
        {
            Step();
            return _connection.GetList<Post>("select * from Posts where Id = @Id", new { Id = i }).First();
        }

        [Benchmark(Description = "GetList<dynamic>")]
        public dynamic GetListDynamic()
        {
            Step();
            return _connection.GetList<dynamic>("select * from Posts where Id = @Id", new { Id = i }).First();
        }

        [Benchmark(Description = "GetFirst<T>")]
        public Post GetFirst()
        {
            Step();
            return _connection.GetFirst<Post>("select * from Posts where Id = @Id", new { Id = i });
        }

        [Benchmark(Description = "GetFirst<dynamic>")]
        public dynamic GetFirstDynamic()
        {
            Step();
            return _connection.GetFirst<dynamic>("select * from Posts where Id = @Id", new { Id = i });
        }

        [Benchmark(Description = "Get<T>")]
        public Post Get()
        {
            Step();
            return _connection.Get<Post>(i);
        }
    }
}
