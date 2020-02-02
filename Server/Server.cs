using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Drawing;

namespace Server
{
   class Server
   {
      const string serverIPAddress = "0.0.0.0"; const int serverPort = 11000;
      const string saveDir1 = ".\\images\\"; const string saveDir2 = "..\\html\\images\\"; // WINDOWS SLASH
      //const string saveDir1 = "./images/"; const string saveDir2 = "../html/images/"; // LINUX SLASH
      Random random = new Random();

      public void ReceiveSendOnStream(Object obj)
      {
         TcpClient client = (TcpClient)obj;
         NetworkStream stream = client.GetStream();

         try
         {
            string command = ReadSendOnStreamString(stream, 1024);
            if(command == "<SEND>")
            {
               string readFromClient = ReadSendOnStreamString(stream, 1024);
            }
            if(command == "<READ>")
            {
               SendReadOnStreamString(stream, "READ FROM SERVER", 1024);
            }

            client.Close();
            System.Console.WriteLine(TimeStamp() + " | Gracefully closed connection.");
         }
         catch
         {
            client.Close();
            System.Console.WriteLine(TimeStamp() + " | Forcibly closed connection!");
         }
      }

      static string ReadSendOnStreamString(NetworkStream stream, int byteArraySize)
      {
         // // Received from client
         // string clientMessageString = ReadStreamString(stream, byteArraySize);
         // Send back to client
         string serverMessageString = TimeStamp() + " | Received: " + clientMessageString;
         WriteStreamString(stream, serverMessageString);
         Console.WriteLine(serverMessageString);
         return clientMessageString;
      }
      static void SendReadOnStreamString(NetworkStream stream, string serverMessageString, int byteArraySize)
      {
         // Start notice from client
         System.Console.WriteLine(ReadStreamString(stream, byteArraySize));
         // Send to client
         WriteStreamString(stream, serverMessageString);
         System.Console.WriteLine(TimeStamp() + " | Sent: " + serverMessageString);
         // Received from client
         string clientMessageString = ReadStreamString(stream, byteArraySize);
         System.Console.WriteLine(clientMessageString);
         //return serverMessageString;
      }
      static string ReadStreamString(NetworkStream stream, int byteArraySize)
      {
         Byte[] byteArray = new Byte[byteArraySize];
         int bytes = stream.Read(byteArray, 0, byteArray.Length);
         return System.Text.Encoding.ASCII.GetString(byteArray, 0, bytes);
      }
      static byte[] ReadStreamByteArray(NetworkStream stream, int byteArraySize)
      {
         Byte[] byteArray = new Byte[byteArraySize];
         int bytes = stream.Read(byteArray, 0, byteArray.Length);
         return byteArray;
      }
      static void WriteStreamString(NetworkStream stream, string message)
      {
         Byte[] byteArray = System.Text.Encoding.ASCII.GetBytes(message);
         stream.Write(byteArray, 0, byteArray.Length);
      }
      static void WriteStreamByteArray(NetworkStream stream, byte[] byteArray)
      {
         stream.Write(byteArray, 0, byteArray.Length);
      }










      static void Main(string[] args)
      {
         Server server = new Server();
         while(true) // Realtime log of connected clients
         {
            System.Console.Write("Listening...\r"); Thread.Sleep(250);
         }
      }
      public Server()
      {
         Thread thread = new Thread(
            delegate()
            {
               TcpListener serverTCPListener = new TcpListener(IPAddress.Parse(serverIPAddress), serverPort);
               serverTCPListener.Start();
               StartListener(serverTCPListener);
            }
         );
         thread.IsBackground = true;
         thread.Start();
      }
      public void StartListener(TcpListener serverTCPListener)
      {
         try
         {
            while(true)
            {
               TcpClient tCPClient = serverTCPListener.AcceptTcpClient();
               Thread thread = new Thread( new ParameterizedThreadStart(ReceiveSendOnStream) );
               thread.Start(tCPClient);
            }
         }
         catch
         {
            serverTCPListener.Stop();
         }
      }

      // ### HELPER FUNCTIONS ###
      public static string TimeStamp()
      {
         return DateTime.Now.ToString("yyyyMMddHHmmssffff");
      }

   }
}
