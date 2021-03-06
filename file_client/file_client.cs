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

        // følgende er tilføjet af os
	    private string fileSize;

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
            string fileName = args[0];

            Console.WriteLine("Starting client...");

            Transport t = new Transport(BUFSIZE, APP);

            Console.WriteLine("Sending filename to server...");
	        byte[] fileNameBytes = Encoding.ASCII.GetBytes(fileName);
            t.send(fileNameBytes, fileNameBytes.Length);

	        Console.WriteLine("Waiting for filesize...");
	        var fileSizeBuffer = new byte[BUFSIZE];
	        t.receive(ref fileSizeBuffer);
            fileSize = Encoding.ASCII.GetString(fileSizeBuffer);

	        if (fileSize.Contains("Error"))
	        {
	            Console.WriteLine(fileSize);
	            return;
	        }
	        Console.WriteLine("File size: " + fileSize);

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
		    Console.WriteLine("Receiving file from server...");
            FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);

            long receivedBytes = 0;
		    long fileSizeLong = Int32.Parse(fileSize);
            int count = 0;
            byte[] data = new byte[BUFSIZE];

            while (fileSizeLong > receivedBytes)
            {
                count = transport.receive(ref data);
                fs.Write(data, 0, count);
                receivedBytes += count;
                Console.WriteLine(receivedBytes);
            }

            Console.WriteLine("File received, " + receivedBytes + " bytes");
        }



        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name='args'>
        /// First argument: Filname
        /// </param>
        public static void Main (string[] args)
		{
            ///*
            // Applikationslag test
            new file_client(args);
            //*/


            string besked = "Gik det her okay?";
            string besked2 = " la la la la la la la la la la la";
            byte[] toSend = Encoding.ASCII.GetBytes(besked);
            byte[] toSend2 = Encoding.ASCII.GetBytes(besked2);


            /*
            // Transportlag test

            var transportLag = new Transport(1000, "test");

            System.Threading.Thread.Sleep(1000);

            transportLag.send(toSend, toSend.Length);

            System.Threading.Thread.Sleep(1000);

            transportLag.send(toSend2, toSend2.Length);

            System.Threading.Thread.Sleep(1000);

            transportLag.send(toSend, toSend.Length);
            //*/

            /*
		    // Linklag test
		    //var linkLag = new Link(1000, "client");
		    //linkLag.send(toSend, toSend.Length);
            */
        }
    }
}