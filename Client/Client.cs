using System;
using System.Net.Sockets;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace Client
{
   public class Client
   {
      const string serverIPAddress = "ggj.atetkao.com"; const int serverPort = 11000; // const string serverIPAddress = "127.0.0.1"; const int serverPort = 11000;
      static void Main(string[] args)
      {
         Send(@".\images\BENCHMARK_CLIENT.jpg");

      }

      public static void Send(string filePathName)
      {
         // filePathName: Where do you want image to be saved, remember Windows filepath slashes:  ".\\images\\1.jpg"
         TcpClient client = new TcpClient(serverIPAddress, serverPort);
         NetworkStream stream = client.GetStream();
         try
         {
            SendReadOnStreamString(stream, "<SEND>", 1024);
            SendReadOnStreamImage(stream, filePathName);
            stream.Close(); client.Close(); System.Console.WriteLine(TimeStamp() + " | Gracefully closed connection.");
         }
         catch
         {
            stream.Close(); client.Close(); System.Console.WriteLine(TimeStamp() + " | Forcibly closed connection!");
         }
      }
      static void SendReadOnStreamImage(NetworkStream stream, string filePath)
      {
         // Send clientID to server, receive confirmation
         SendReadOnStreamString(stream, GetClientID(), 1024);

         // Read in image locally
         byte[] imageByteArray = System.IO.File.ReadAllBytes(filePath);
         // Determine byte length
         int imageByteLength = imageByteArray.Length;
         // Send byte length to server, receive confirmation
         SendReadOnStreamString(stream, imageByteLength.ToString(), 32);
         
         // Send image to server
         WriteStreamByteArray(stream, imageByteArray);
         System.Console.WriteLine(TimeStamp() + " | Sent: " + filePath);
         // Received from server
         string serverMessageString = ReadStreamString(stream, 1024);
         System.Console.WriteLine(serverMessageString);
      }
      static void SendReadOnStreamString(NetworkStream stream, string clientMessageString, int byteArraySize)
      {
         // Send to server
         WriteStreamString(stream, clientMessageString);
         System.Console.WriteLine(TimeStamp() + " | Sent: " + clientMessageString);
         // Received from server
         string serverMessageString = ReadStreamString(stream, byteArraySize);
         System.Console.WriteLine(serverMessageString);
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

      // ### HELPER FUNCTIONS ###
      public static string TimeStamp()
      {
         return DateTime.Now.ToString("yyyyMMddHHmmssffff");
      }
      public static string GetClientID()
      {
         string hostName = Dns.GetHostName(); // Retrive the Name of HOST    
         string myIP = Dns.GetHostAddresses(hostName)[0].ToString();

         string macAddress = "";
         foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
         {
            if (nic.OperationalStatus == OperationalStatus.Up)
            {
               macAddress += nic.GetPhysicalAddress().ToString();
               break;
            }
         }
         string clientID = Regex.Replace(macAddress + "_" + myIP, @"[^A-Za-z0-9_]+", "");
         return clientID;
      }
   }
}
