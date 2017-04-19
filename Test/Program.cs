using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Linklaget;

namespace Test
{
    class Program
    {
        const byte DELIMITER = (byte)'A';
        static void Main(string[] args)
        {
            //Link myLink = new Link(100, "test");
            byte[] toSend = Encoding.ASCII.GetBytes("GHTJAPDB");
            //myLink.send(toSend, 8);
            Test myTest = new Test();
            myTest.send(toSend, 8);

        }
        
    }

    class Test
    {
        const byte DELIMITER = (byte)'A';
        public void send(byte[] buf, int size)
        {
            // TO DO Your own code
            int i = 1;
            char c;
            byte[] sendBuf = new byte[size * 2 + 2];

            sendBuf[0] = DELIMITER;

            foreach (var bufferByte in buf)
            {
                if (bufferByte == (byte)'A')
                {
                    sendBuf[i] = (byte)'B';
                    i++;
                    sendBuf[i] = (byte)'C';
                }
                else if (bufferByte == (byte)'B')
                {
                    sendBuf[i] = (byte)'B';
                    i++;
                    sendBuf[i] = (byte)'D';
                }
                else
                {
                    sendBuf[i] = bufferByte;
                }

                i++;
            }
            sendBuf[i] = DELIMITER;

            foreach (var x in buf)
            {
                Console.Write(Convert.ToChar(x));
            }
            Console.WriteLine();
            foreach (var x in sendBuf)
            {
                Console.Write(Convert.ToChar(x));
            }
            Console.WriteLine();
        }
    }
}
