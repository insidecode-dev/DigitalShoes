using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalShoes.Domain
{
    public class StaticDetails
    {
        public enum Gender
        {
            Man = 1, 
            Woman = 2
        }

        public enum Rating
        {
            Best = 5, 
            Good = 4, 
            Normal = 3, 
            Bad = 2, 
            Worst = 1
        }

        public enum Color
        {
            Red = 1,
            Green = 2,
            Blue = 3,
            Yellow = 4,
            Orange = 5,
            Purple = 6,
            Pink = 7,
            Brown = 8,
            White = 9,
            Black = 10
        }

        public enum DataStatus
        {
            Inserted = 1, Updated = 2, Deleted = 3
        }
    }
}
