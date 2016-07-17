using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLaDOSbot
{
    class Message
    {
        public Message(string message)
        {
            DisplayMessage(message);
        }

        public void DisplayMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
