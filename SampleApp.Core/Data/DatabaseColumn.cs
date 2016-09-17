using System;

namespace SampleApp.Core.Data
{
    public class DatabaseColumn
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public Type DataType { get; set; }
    }
}