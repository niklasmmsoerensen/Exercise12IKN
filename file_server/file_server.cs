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

        //const int PORT = 9000;


        /// <summary>
        /// Initializes a new instance of the <see cref="file_server"/> class.
        /// </summary>
        private file_server ()
		{
            Transport t = new Transport(BUFSIZE, APP); 
            

         //   sendFile(clientFile.ToString(), LIB.check_File_Exists (clientFile),t);

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
<<<<<<< HEAD
            //new file_server();
			var transportLag = new Transport(1000, "test");
			var toReceive = new byte[1000];
=======
            new file_server();
>>>>>>> 835efeef2e2fc3f8fa575cb6c9c750ee6b10b326

			transportLag.receive (ref toReceive);
			foreach (var b in toReceive) {
				Console.Write ((char)b);
			}

			transportLag.receive (ref toReceive);
			foreach (var b in toReceive) {
				Console.Write ((char)b);
			}

			transportLag.receive (ref toReceive);
			foreach (var b in toReceive) {
				Console.Write ((char)b);
			}


			/*
            // Link lag test
      //      var linkLag = new Link(1000, "server");

		    //var toReceive = new byte[10];

		    //linkLag.receive(ref toReceive);

<<<<<<< HEAD
		    foreach (var b in toReceive)
		    {
		        Console.Write((char)b);
		    }
			*/
=======
		    //foreach (var b in toReceive)
		    //{
		    //    Console.Write((char)b);
		    //}
>>>>>>> 835efeef2e2fc3f8fa575cb6c9c750ee6b10b326

		}
	}
}
 