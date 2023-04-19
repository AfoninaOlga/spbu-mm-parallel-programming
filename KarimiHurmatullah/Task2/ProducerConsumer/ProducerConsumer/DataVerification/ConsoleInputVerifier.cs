using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerConsumer.DataVerification
{
    public static class ConsoleInputVerifier
    {
        public static int ReadInputFromConsole(string input)
        {
            bool isParseSuccessful = int.TryParse(input, out int result);

            if (isParseSuccessful && result > 0)
            {
                return result;
            }
            else
            {
                return 0;
            }
        }
    }
}
