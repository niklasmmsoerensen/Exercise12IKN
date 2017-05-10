using System;
using System.ComponentModel;
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

        //const int PORT = 9000;


        /// <summary>
        /// Initializes a new instance of the <see cref="file_server"/> class.
        /// </summary>
        private file_server ()
		{
		    Console.WriteLine("Starting server...");
            Transport t = new Transport(BUFSIZE, APP);

		    while (true)
		    {
                Console.WriteLine("Waiting for filename...");
                byte[] clientFileBuffer = new byte[BUFSIZE];
                t.receive(ref clientFileBuffer);
                string clientFile = Encoding.ASCII.GetString(clientFileBuffer);
                // Nedestående linje var udkommenteret i TCP koden
                //clientFile = LIB.extractFileName(clientFile);
                Console.WriteLine("File requested: " + clientFile);

		        if (LIB.check_File_Exists(clientFile) != 0)
		        {
		            Console.WriteLine("Sending filesize to client...");
		            string fileSize = LIB.check_File_Exists(clientFile).ToString();
                    byte[] fileSizeBytes = Encoding.ASCII.GetBytes(fileSize);
                    t.send(fileSizeBytes, fileSizeBytes.Length);

		            Console.WriteLine("Sending file to client...");
                    sendFile(clientFile, long.Parse(fileSize), t);
		        }
		        else
		        {
                    Console.WriteLine("File does not exist!");
                    string error = "Error: File does not exist on server.";
                    byte[] errorBytes = Encoding.ASCII.GetBytes(error);
                    t.send(errorBytes, errorBytes.Length);
                }

		    }
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
                byte[] data = new byte[BUFSIZE];
                int count = 0;

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
            /*
            // Applikationslag test
            new file_server();
            */
            


            /*
            // Transportlag test
            var transportLag = new Transport(1000, "betyderikkenoget");
            var toReceive = new byte[1000];


            transportLag.receive(ref toReceive);
            foreach (var b in toReceive)
            {
                Console.Write((char)b);
            }

            transportLag.receive(ref toReceive);
            foreach (var b in toReceive)
            {
                Console.Write((char)b);
            }

            transportLag.receive(ref toReceive);
            foreach (var b in toReceive)
            {
                Console.Write((char)b);
            }
            */



            /*
            // Link lag test
      		var linkLag = new Link(1000, "betyderikkenoget");

		    var toReceive = new byte[1000];

		    linkLag.receive(ref toReceive);

		    foreach (var b in toReceive)
		    {
		        Console.Write((char)b);
		    }

		    Console.WriteLine("\n" + toReceive.Length);
			*/
        }
    }
}
 