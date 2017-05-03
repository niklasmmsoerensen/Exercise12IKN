using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Transportlaget;
using Library;
using Linklaget;

namespace Application
{
	class file_client
	{
		/// <summary>
		/// The BUFSIZE.
		/// </summary>
		private const int BUFSIZE = 1000;
		private const string APP = "FILE_CLIENT";


        /// <summary>
        /// Initializes a new instance of the <see cref="file_client"/> class.
        /// 
        /// file_client metoden opretter en peer-to-peer forbindelse
        /// Sender en forspÃ¸rgsel for en bestemt fil om denne findes pÃ¥ serveren
        /// Modtager filen hvis denne findes eller en besked om at den ikke findes (jvf. protokol beskrivelse)
        /// Lukker alle streams og den modtagede fil
        /// Udskriver en fejl-meddelelse hvis ikke antal argumenter er rigtige
        /// </summary>
        /// <param name='args'>
        /// Filnavn med evtuelle sti.
        /// </param>
        private file_client(String[] args)
	    {
            Transport t = new Transport(BUFSIZE, APP);
	        string fileName = args[0];
            receiveFile(fileName,t);

        }

		/// <summary>
		/// Receives the file.
		/// </summary>
		/// <param name='fileName'>
		/// File name.
		/// </param>
		/// <param name='transport'>
		/// Transportlaget
		/// </param>
		private void receiveFile (String fileName, Transport transport)
		{
            FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);


      //      long receivedBytes = 0;
      //      byte[] data = new byte[BUFSIZE];

		    //int count = transport.receive(ref data);
            
		    //while (data.Length > receivedBytes) 
		    //{
      //          fs.Write(data, 0, count);
      //          receivedBytes += count;
      //          Console.WriteLine(receivedBytes);
      //      }

        }



        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name='args'>
        /// First argument: Filname
        /// </param>
        public static void Main (string[] args)
		{
			//new file_client(args);
			var toSend = new byte[7] {(byte) 'a', (byte)'y' , (byte)'y' , (byte)'l' , (byte)'m' , (byte)'a' , (byte)'o'};
			new file_client(args);

			//transportlag test
			var transportLag = new Transport (1000, "test");
			transportLag.send(toSend, 1000);
			transportLag.send(toSend, 1000);
			transportLag.send(toSend, 1000);

		}
	}
}