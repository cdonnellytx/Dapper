using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dapper.Tests.Performance.DapperDotDatabase
{
    /// <summary>
    /// Dapper.Database currently has a bug where it can't handle multiple attributes named <c>TableAttribute</c> being on a class.
    /// Once this is fixed, it can use the shared class, and this instance can go away.
    /// </summary>
    [Table("Posts")]
    public class Post
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastChangeDate { get; set; }
        public int? Counter1 { get; set; }
        public int? Counter2 { get; set; }
        public int? Counter3 { get; set; }
        public int? Counter4 { get; set; }
        public int? Counter5 { get; set; }
        public int? Counter6 { get; set; }
        public int? Counter7 { get; set; }
        public int? Counter8 { get; set; }
        public int? Counter9 { get; set; }
    }
}
