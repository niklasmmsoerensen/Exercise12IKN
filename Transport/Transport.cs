using System;
using Linklaget;

/// <summary>
/// Transport.
/// </summary>
namespace Transportlaget
{
	/// <summary>
	/// Transport.
	/// </summary>
	public class Transport
	{
		/// <summary>
		/// The link.
		/// </summary>
		private Link link;
		/// <summary>
		/// The 1' complements checksum.
		/// </summary>
		private Checksum checksum;
		/// <summary>
		/// The buffer.
		/// </summary>
		private byte[] buffer;
		/// <summary>
		/// The seq no.
		/// </summary>
		private byte seqNo;
		/// <summary>
		/// The old_seq no.
		/// </summary>
		private byte old_seqNo;
		/// <summary>
		/// The error count.
		/// </summary>
		private int errorCount;
		/// <summary>
		/// The DEFAULT_SEQNO.
		/// </summary>
		private const int DEFAULT_SEQNO = 2;
		/// <summary>
		/// The data received. True = received data in receiveAck, False = not received data in receiveAck
		/// </summary>
		private bool dataReceived;
		/// <summary>
		/// The number of data the recveived.
		/// </summary>
		private int recvSize = 0;

		/// <summary>
		/// Initializes a new instance of the <see cref="Transport"/> class.
		/// </summary>
		public Transport (int BUFSIZE, string APP)
		{
			link = new Link(BUFSIZE+(int)TransSize.ACKSIZE, APP);
			checksum = new Checksum();
			buffer = new byte[BUFSIZE+(int)TransSize.ACKSIZE];
			seqNo = 1;
			old_seqNo = DEFAULT_SEQNO;
			errorCount = 0;
			dataReceived = false;
		}

		/// <summary>
		/// Receives the ack.
		/// </summary>
		/// <returns>
		/// The ack.
		/// </returns>
		private bool receiveAck()
		{
			recvSize = link.receive(ref buffer);
			dataReceived = true;

			if (recvSize == (int)TransSize.ACKSIZE) {
				dataReceived = false;
				bool checksumResult = checksum.checkChecksum (buffer, (int)TransSize.ACKSIZE);
				if (!checksumResult ||
				  buffer [(int)TransCHKSUM.SEQNO] != seqNo ||
				  buffer [(int)TransCHKSUM.TYPE] != (int)TransType.ACK)
				{
					return false;
				}
				seqNo = (byte)((buffer[(int)TransCHKSUM.SEQNO] + 1) % 2);
			}
 
			return true;
		}

		/// <summary>
		/// Sends the ack.
		/// </summary>
		/// <param name='ackType'>
		/// Ack type.
		/// </param>
		private void sendAck (bool ackType)
		{
            byte[] ackBuf = new byte[(int)TransSize.ACKSIZE];
            ackBuf[(int)TransCHKSUM.SEQNO] = (byte)
                (ackType ? (byte)buffer[(int)TransCHKSUM.SEQNO] : (byte)(buffer[(int)TransCHKSUM.SEQNO] + 1) % 2);
            ackBuf[(int)TransCHKSUM.TYPE] = (byte)(int)TransType.ACK;
            checksum.calcChecksum(ref ackBuf, (int)TransSize.ACKSIZE);

            /*
            // bitfejl
            if (++errorCount == 2)
            {
                ackBuf[1]++;
                Console.WriteLine("FEJL I ACK");
            }
            */

            link.send(ackBuf, (int)TransSize.ACKSIZE);
        }

		/// <summary>
		/// Send the specified buffer and size.
		/// </summary>
		/// <param name='buffer'>
		/// Buffer.
		/// </param>
		/// <param name='size'>
		/// Size.
		/// </param>
		public void send(byte[] buf, int size)
		{
            // TO DO Your own code
            int counter = 0;

            // Remember buffer length
            int lengthOfBuffer = buffer.Length;
            do
            {
                // reset buffer length (so it is not only 4)
                buffer = new byte[lengthOfBuffer];

                buffer[(int)TransCHKSUM.SEQNO] = (byte)seqNo;
                buffer[(int)TransCHKSUM.TYPE] = (byte)TransType.DATA;
                Array.Copy(buf, 0, buffer, 4, buf.Length);

                checksum.calcChecksum(ref buffer, size);

                /*
                // bitfejl
                if (++errorCount == 2)
                {
                    buffer[1]++;
                    Console.WriteLine("NOISE! - Byte #2 is damaged in the first transmission!");
                }
                */

                link.send(buffer, size);
                counter++;

            } while (!receiveAck() && counter < 5);
            if (counter > 5)
            { //timeout
                Console.WriteLine("Timeout from send, counter > 5");
            }
            old_seqNo = seqNo;
            
            // reset buffer length (so it is not only 4)
            buffer = new byte[lengthOfBuffer];
        }

        /// <summary>
        /// Receive the specified buffer.
        /// </summary>
        /// <param name='buffer'>
        /// Buffer.
        /// </param>
        public int receive(ref byte[] buf)
        {
            // receieve data selv med brug af link laget
            // K�r data igennem checkchecksum funktion
            // kald funktionen sendack og brug returv�rdien fra checkchecksum som parameter
            // muligvis noget loop

            bool status = false;
            do
            {
                int size = link.receive(ref buffer);
                status = checksum.checkChecksum(buffer, size);

                sendAck(status);

                if (status == true)
                {
                    Array.Copy(buffer, 4, buf, 0, buffer.Length - 4);
                    //buf = buffer;
                    return size;
                }

            } while (status == false);

            return 1;
        }
    }
}