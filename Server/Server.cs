﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Drawing;

namespace Server
{
   class Server
   {
      const int maxByteArray = 102400;
      const string serverIPAddress = "0.0.0.0"; const int serverPort = 11000;
      //const string saveDir1 = ".\\images\\"; const string saveDir2 = "..\\html\\images\\"; // WINDOWS SLASH
      //const string saveDir1 = "./images/"; const string saveDir2 = "../html/images/"; // LINUX SLASH
      Random random = new Random();
      static int currentImageCounter = 1;
      static int maxImageCount = 30;

      public void ReceiveSendOnStream(Object obj)
      {
         TcpClient client = (TcpClient)obj;
         NetworkStream stream = client.GetStream();
         try
         {
            string command = ReadSendOnStreamString(stream, maxByteArray);
            if(command == "<SEND>")
            {
               ReadSendOnStreamImage(stream, maxByteArray);
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
         // Received from client
         string clientMessageString = ReadStreamString(stream, byteArraySize);
         // Send back to client
         string serverMessageString = TimeStamp() + " | Received: " + clientMessageString;
         WriteStreamString(stream, serverMessageString);
         Console.WriteLine(serverMessageString);
         return clientMessageString;
      }
      static void ReadSendOnStreamImage(NetworkStream stream, int byteArraySize)
      {
         // Receive clientID
         string clientID = ReadSendOnStreamString(stream, 1024);
         // Receive imageByteLength
         int imageByteLength = Int32.Parse(ReadSendOnStreamString(stream, 1024));
         // Receive image
         Byte[] byteArray = new Byte[imageByteLength];
         stream.Read(byteArray, 0, imageByteLength);

         // Save image locally
         string filePath1 = $"./images/{ TimeStamp() }_{ clientID }.jpg";
         System.IO.File.WriteAllBytes(filePath1, byteArray);

         string filePath2 = $"../html/images/{ currentImageCounter }.jpg";
         currentImageCounter++; if(currentImageCounter > maxImageCount){ currentImageCounter = 1; }
         System.IO.File.WriteAllBytes(filePath2, byteArray);

         // Send back to client
         string serverMessageString = $"{ TimeStamp() } | Received image saved as: { filePath1 }";
         WriteStreamString(stream, serverMessageString);
         Console.WriteLine(serverMessageString);
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
