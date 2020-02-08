using System;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace Client
{
   public class Client
   {

      public static int Main(String[] args)
      {
         string serverHostName = "ggj.atetkao.com"; // string serverHostName = "localhost";
         int serverPort = 11000;

         string filePathName = @".\images\BENCHMARK_CLIENT.jpg";
         Send(serverHostName, serverPort, filePathName);
         return 0;         
      }

      public static void Send(string serverHostName, int serverPort, string filePathName)
      {
         // Connect to server
         IPAddress serverIPAddress = Dns.GetHostEntry(serverHostName).AddressList[0];
         IPEndPoint serverEndPoint = new IPEndPoint(serverIPAddress, serverPort);
         Socket serverSocket = new Socket(serverIPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
         serverSocket.Connect(serverEndPoint);
         try
         {
            // 1. Send clientID
            SendReadString(serverSocket, GetClientID());

            // 2. Send fileByteArray
            //SendReadFile(serverSocket, filePathName);

            // Release the socket
            serverSocket.Shutdown(SocketShutdown.Both); serverSocket.Close(); System.Console.WriteLine(TimeStamp() + " | Gracefully closed connection.");
         }
         catch
         {
            // Release the socket
            serverSocket.Shutdown(SocketShutdown.Both); serverSocket.Close(); System.Console.WriteLine(TimeStamp() + " | Forcibly closed connection!");
         }
      }
      public static void SendReadString(Socket serverSocket, string clientMessage, int maxByteLength = 1024)
      {
         serverSocket.Send(System.Text.Encoding.ASCII.GetBytes(clientMessage));
         byte[] serverMessageByteArray = new byte[maxByteLength];
         int byteLength = serverSocket.Receive(serverMessageByteArray);
         System.Console.WriteLine(System.Text.Encoding.ASCII.GetString(serverMessageByteArray, 0, byteLength));
      }


      public static void SendReadFile(Socket serverSocket, string filePath, int maxByteLength = 1024)
      {
         byte[] imageByteArray = System.IO.File.ReadAllBytes(filePath);
         // Determine file's byte length
         SendReadString(serverSocket, imageByteArray.Length.ToString());
         // Send file
         serverSocket.SendFile(filePath);

         // Received confirmation
         byte[] serverMessageByteArray = new byte[maxByteLength];
         int byteLength = serverSocket.Receive(serverMessageByteArray);
         System.Console.WriteLine(System.Text.Encoding.ASCII.GetString(serverMessageByteArray, 0, byteLength));
      }


      // const string serverIPAddress = "ggj.atetkao.com"; const int serverPort = 11000; // const string serverIPAddress = "127.0.0.1"; const int serverPort = 11000;
      // static void Main(string[] args)
      // {
      //    Send(@".\images\BENCHMARK_CLIENT.jpg");

      // }

      // public static void Send(string filePathName)
      // {
      //    // filePathName: Where do you want image to be saved, remember Windows filepath slashes:  ".\\images\\1.jpg"
      //    TcpClient client = new TcpClient(serverIPAddress, serverPort);
      //    NetworkStream stream = client.GetStream();
      //    try
      //    {
      //       SendReadOnStreamString(stream, "<SEND>", 1024);
      //       SendReadOnStreamImage(stream, filePathName);
      //       stream.Close(); client.Close(); System.Console.WriteLine(TimeStamp() + " | Gracefully closed connection.");
      //    }
      //    catch
      //    {
      //       stream.Close(); client.Close(); System.Console.WriteLine(TimeStamp() + " | Forcibly closed connection!");
      //    }
      // }
      // static void SendReadOnStreamImage(NetworkStream stream, string filePath)
      // {
      //    // Send clientID to server, receive confirmation
      //    SendReadOnStreamString(stream, GetClientID(), 1024);

      //    // Read in image locally
      //    byte[] imageByteArray = System.IO.File.ReadAllBytes(filePath);
      //    // Determine byte length
      //    int imageByteLength = imageByteArray.Length;
      //    // Send byte length to server, receive confirmation
      //    SendReadOnStreamString(stream, imageByteLength.ToString(), 1024);
         
      //    // Send image to server
      //    WriteStreamByteArray(stream, imageByteArray);
      //    System.Console.WriteLine(TimeStamp() + " | Sent: " + filePath);
      //    // Received from server
      //    string serverMessageString = ReadStreamString(stream, 1024);
      //    System.Console.WriteLine(serverMessageString);
      // }
      // static void SendReadOnStreamString(NetworkStream stream, string clientMessageString, int byteArraySize)
      // {
      //    // Send to server
      //    WriteStreamString(stream, clientMessageString);
      //    System.Console.WriteLine(TimeStamp() + " | Sent: " + clientMessageString);
      //    // Received from server
      //    string serverMessageString = ReadStreamString(stream, byteArraySize);
      //    System.Console.WriteLine(serverMessageString);
      // }
      // static string ReadStreamString(NetworkStream stream, int byteArraySize)
      // {
      //    Byte[] byteArray = new Byte[byteArraySize];
      //    int bytes = stream.Read(byteArray, 0, byteArray.Length);
      //    return System.Text.Encoding.ASCII.GetString(byteArray, 0, bytes);
      // }
      // static byte[] ReadStreamByteArray(NetworkStream stream, int byteArraySize)
      // {
      //    Byte[] byteArray = new Byte[byteArraySize];
      //    int bytes = stream.Read(byteArray, 0, byteArray.Length);
      //    return byteArray;
      // }
      // static void WriteStreamString(NetworkStream stream, string message)
      // {
      //    Byte[] byteArray = System.Text.Encoding.ASCII.GetBytes(message);
      //    stream.Write(byteArray, 0, byteArray.Length);
      // }
      // static void WriteStreamByteArray(NetworkStream stream, byte[] byteArray)
      // {
      //    stream.Write(byteArray, 0, byteArray.Length);
      // }

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
