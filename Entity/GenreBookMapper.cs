using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DdadduBot.Entity
{
    class GenreBookMapper
    {
        private static GenreBookMapper instance;
        private Dictionary<int, List<string>> GenreBookMap;

        private GenreBookMapper()
        {
            GenreBookMap.Add(1, Enumerable.Repeat(string.Empty, 20).ToList()); 
            GenreBookMap.Add(2, new List<string> { });
            GenreBookMap.Add(3, new List<string> { });
            GenreBookMap.Add(4, new List<string> { });
            GenreBookMap.Add(5, new List<string> { });
        }

        public static GenreBookMapper GetInstance()
        {
            if (instance == null)
            {
                instance = new GenreBookMapper();
            }
            return instance;
        }
    }
}
