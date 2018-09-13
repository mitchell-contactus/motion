using System;
using System.Collections.Generic;

namespace Motion.Views
{
    public class View
    {
        public int ID { get; }
        public string Name { get; internal set; }
        public HashSet<int> Forms = new HashSet<int>();

        public View(int id)
        {
            ID = id;
        }
    }
}
