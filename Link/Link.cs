using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;

/// <summary>
/// Link.
/// </summary>
namespace Linklaget
{
	/// <summary>
	/// Link.
	/// </summary>
	public class Link
	{
		/// <summary>
		/// The DELIMITE for slip protocol.
		/// </summary>
		const byte DELIMITER = (byte)'A';
		/// <summary>
		/// The buffer for link.
		/// </summary>
		private byte[] buffer;
		/// <summary>
		/// The serial port.
		/// </summary>
		SerialPort serialPort;

		/// <summary>
		/// Initializes a new instance of the <see cref="link"/> class.
		/// </summary>
		public Link (int BUFSIZE, string APP)
		{
			// Create a new SerialPort object with default settings.
			#if DEBUG
				if(APP.Equals("FILE_SERVER"))
				{
					serialPort = new SerialPort("/dev/ttyS1",115200,Parity.None,8,StopBits.One);
				}
				else
				{
					serialPort = new SerialPort("/dev/ttyS1",115200,Parity.None,8,StopBits.One);
				}
			#else
				serialPort = new SerialPort("/dev/ttyS1",115200,Parity.None,8,StopBits.One);
			#endif
			if(!serialPort.IsOpen)
				serialPort.Open();

			buffer = new byte[(BUFSIZE*2)];

			// Uncomment the next line to use timeout
			//serialPort.ReadTimeout = 500;

			serialPort.DiscardInBuffer ();
			serialPort.DiscardOutBuffer ();
		}

		/// <summary>
		/// Send the specified buf and size.
		/// </summary>
		/// <param name='buf'>
		/// Buffer.
		/// </param>
		/// <param name='size'>
		/// Size.
		/// </param>
		public void send (byte[] buf, int size)
		{
            // TO DO Your own code
            int i = 1;

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

			serialPort.Write (sendBuf, 0, sendBuf.Length);

			//serialPort.Close ();
        }

        /// <summary>
        /// Receive the specified buf and size.
        /// </summary>
        /// <param name='buf'>
        /// Buffer.
        /// </param>
        /// <param name='size'>
        /// Size.
        /// </param>
        public int receive(ref byte[] buf)
        {            
            // til sammenligning
            var B = (byte)'B';
            var C = (byte)'C';
            var D = (byte)'D';

            // hvor mange bytes ligger der klar i bufferen
            var bytesToRead = serialPort.BytesToRead;

            // er der noget at læse
            while (bytesToRead == 0)
            {
                bytesToRead = serialPort.BytesToRead;
            }

            buffer = new byte[bytesToRead];
            serialPort.Read(buffer, 0, bytesToRead);

            // vi finder start af vores frame
            int start = 0;
            while (buffer[start] != DELIMITER)
            {
                start++;
            }

            // vi finder slut af vores frame
            int slut = start + 1;
            while (buffer[slut] != DELIMITER)
            {
                slut++;
            }

            // temp liste til vores "dekrypterede" besked
            var temp = new List<byte>();

            // Vi er ikke interesseret i vores delimitter
            start++;

            // array l?es igennem og "dekrypteres"
            for (; start < slut; start++)
            {
                if (buffer[start] == B)
                {
                    if (buffer[start + 1] == C)
                    {
                        temp.Add(DELIMITER); // DELIMITER = A
                        start++;
                    }
                    else if (buffer[start + 1] == D)
                    {
                        temp.Add(B);
                        start++;
                    }
                }
                else
                {
                    temp.Add(buffer[start]);
                }
            }

            // buf rettes
            buf = temp.ToArray();

            return buf.Length;
        }

    }
}
