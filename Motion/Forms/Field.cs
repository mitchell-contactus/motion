using System;
using System.Collections.Generic;

namespace Motion.Forms
{
    public class Field
    {
        public int ID { get; }
        public string Name { get; internal set; }
        public string Type { get; internal set; }
        public int? DependentOn { get; internal set; }
        public string Hint { get; internal set; }
        public int SortOrder { get; internal set; }
        public bool Required { get; internal set; }

        public List<string> Options { get; set; }

        public Field(int id)
        {
            ID = id;
        }
    }
}
