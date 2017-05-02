using System;
using System.IO;
using System.Text;
using Transportlaget;
using Library;
using Linklaget;

namespace Application
{
	class file_server
	{
		/// <summary>
		/// The BUFSIZE
		/// </summary>
		private const int BUFSIZE = 1000;
		private const string APP = "FILE_SERVER";

		/// <summary>
		/// Initializes a new instance of the <see cref="file_server"/> class.
		/// </summary>
		private file_server ()
		{
			Link myLink = new Link(100, "FILE_SERVER");
			byte[] toSend = Encoding.ASCII.GetBytes("GHTJAPDB");

			myLink.send(toSend, 8);
		}

		/// <summary>
		/// Sends the file.
		/// </summary>
		/// <param name='fileName'>
		/// File name.
		/// </param>
		/// <param name='fileSize'>
		/// File size.
		/// </param>
		/// <param name='tl'>
		/// Tl.
		/// </param>
		private void sendFile(String fileName, long fileSize, Transport transport)
		{
            using (FileStream SourceStream = File.Open(fileName, FileMode.Open))
            {
                long sentBytes = 0;
                byte[] data = new byte[1000];
                int count = 0;
                Console.WriteLine("Sending file to client...");
                while (fileSize > sentBytes)
                {
                    count = SourceStream.Read(data, 0, data.Length);
                    transport.send(data,count);
                    sentBytes += count;
                    Console.WriteLine(sentBytes);
                }
                Console.WriteLine("File sent! Bytes sent: {0}", sentBytes);
            }
		}

        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name='args'>
        /// The command-line arguments.
        /// </param>
        public static void Main (string[] args)
		{
            //new file_server();

            // Link lag test
            var linkLag = new Link(1000, "server");

		    var toReceive = new byte[10];

		    linkLag.receive(ref toReceive);

		    foreach (var b in toReceive)
		    {
		        Console.Write((char)b);
		    }

		}
	}
}