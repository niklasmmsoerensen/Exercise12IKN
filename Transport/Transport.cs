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

            ///*
            // bitfejl
            if (++errorCount == 2)
            {
                ackBuf[1]++;
                Console.WriteLine("FEJL I ACK");
            }
            //*/

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
            do
            {
                buffer[(int)TransCHKSUM.SEQNO] = (byte)seqNo;
                buffer[(int)TransCHKSUM.TYPE] = (byte)TransType.DATA;
                Array.Copy(buf, 0, buffer, 4, size);

                checksum.calcChecksum(ref buffer, size);
                ///*
                if (++errorCount == 2)
                {
                    buffer[1]++;
                    Console.WriteLine("NOISE! - Byte #2 is damaged in the first transmission!");
                }
                //*/
                link.send(buffer, size+4);
            } while (!receiveAck());

		    old_seqNo = DEFAULT_SEQNO;
        }

        /// <summary>
        /// Receive the specified buffer.
        /// </summary>
        /// <param name='buffer'>
        /// Buffer.
        /// </param>
        public int receive(ref byte[] buf)
        {
            int size;
            bool status;
            do
            {

                size = link.receive(ref buffer);
                bool checkStatus = checksum.checkChecksum(buffer, size - 4);
                bool seqStatus = buffer[2] != old_seqNo;
                status = checkStatus && seqStatus;

                sendAck(status);

            } while (!status); 

            old_seqNo = buffer[2];
            Array.Copy(buffer, 4, buf, 0, size - 4);
            return size - 4;
        }
    }
}