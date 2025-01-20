using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MkNinja
{
    // dll that is suppused to be thogeder with the .exe file
    public class MkNinja
    {
        public string NinjaDLLWorcks(int ul_reason_for_call, string data)
        {
            switch (ul_reason_for_call)
            {
                case 1:

                case 2:
                    return "banana";
                case 3:
                    return "apple";
                default:
                    return "eh";
            }
        }

    }
}
