using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvHelperTutorial
{
    public class Foo
    {
        [Name("id")]
        [Index(0)]
        public int Id { get; set; }
        [Name("name")]
        [Index(1)]
        public string Name { get; set; }
    }
}
