using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaDLL
{
    public class NinjaDLL
    {
        public string NinjaDLLWorcks(int ul_reason_for_call)
        {
            switch (ul_reason_for_call)
            {
                case 1:
                    return "hello";
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
