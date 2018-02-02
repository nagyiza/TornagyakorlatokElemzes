using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stream;
using System.IO;

namespace SkeletonCompare
{
    public class Program
    {
        private static Compare skeletonCompare;
        static void Main(string[] args)
        {
            skeletonCompare = new Compare();
        }
    }

}
