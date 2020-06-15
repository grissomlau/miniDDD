using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MiniDDD;
using SqlSugar;

namespace DDD.Simple.Model
{
    [Table("User")]
    [SugarTable("User")]
    public class User : IEFModel
    {
        [SugarColumn(IsPrimaryKey = true)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
