using System;
using System.Net.Sockets;
using System.Drawing;
using System.IO;
using System.Threading;

namespace Client
{
   class Client
   {
      const int maxByteArray = 102400; 
      //const string serverIPAddress = "127.0.0.1"; const int serverPort = 11000;
      const string serverIPAddress = "ggj.atetkao.com"; const int serverPort = 11000;
      string clientID = "client1";
      string levelID = "01";
      string byteArrayLength = null;
      static void Main(string[] args)
      {
         //Send();
         Read();
      }

      static void Send()
      {
         TcpClient client = new TcpClient(serverIPAddress, serverPort);
         NetworkStream stream = client.GetStream();
         try
         {
            SendReadOnStreamString(stream, "<SEND>", maxByteArray);
            //SendReadOnStreamString(stream, "PACKAGE FROM CLIENT", maxByteArray);
            SendReadOnStreamImage(stream, ".\\1.jpg");
            //SendReadOnStreamImage(stream, ".\\images\\1.jpg");
            stream.Close(); client.Close(); System.Console.WriteLine(TimeStamp() + " | Gracefully closed connection.");
         }
         catch
         {
            stream.Close(); client.Close(); System.Console.WriteLine(TimeStamp() + " | Forcibly closed connection!");
         }
      }
      static void Read()
      {
         TcpClient client = new TcpClient(serverIPAddress, serverPort);
         NetworkStream stream = client.GetStream();
         try
         {
            SendReadOnStreamString(stream, "<READ>", maxByteArray);
            //ReadSendOnStreamString(stream, maxByteArray);
            ReadSendOnStreamImage(stream, maxByteArray);
            stream.Close(); client.Close(); System.Console.WriteLine(TimeStamp() + " | Gracefully closed connection.");
         }
         catch
         {
            stream.Close(); client.Close(); System.Console.WriteLine(TimeStamp() + " | Forcibly closed connection!");
         }
      }
      static void ReadSendOnStreamImage(NetworkStream stream, int byteArraySize)
      {
         
         WriteStreamString(stream, "<GO>");
         
         // Received from server
         Byte[] byteArray = new Byte[byteArraySize];
         int bytes = stream.Read(byteArray, 0, byteArray.Length);
         MemoryStream ms = new MemoryStream(byteArray);
         Image serverImage = Image.FromStream(ms);

         // Save out
         string filePath = ".//" + TimeStamp() + "_IMAGE_FROM_SERVER.jpg";
         serverImage.Save(filePath);

         // Send back to server
         string clientMessageString = TimeStamp() + " | Received image save as: " + filePath;
         WriteStreamString(stream, clientMessageString);
         Console.WriteLine(clientMessageString);
      }
      static void SendReadOnStreamString(NetworkStream stream, string clientMessageString, int byteArraySize)
      {
         // Send to server
         WriteStreamString(stream, clientMessageString);
         System.Console.WriteLine(TimeStamp() + " | Sent: " + clientMessageString);
         // Received from server
         string serverMessageString = ReadStreamString(stream, byteArraySize);
         System.Console.WriteLine(serverMessageString);
         //return serverMessageString;
      }
      static void SendReadOnStreamImage(NetworkStream stream, string filePath)
      {
         // Read in image locally
         MemoryStream ms = new MemoryStream();
         Image.FromFile(filePath).Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
         Byte[] imageByteArray = ms.ToArray();

         System.Console.WriteLine("Image Byte lenght: " + imageByteArray.Length.ToString());

         // Send to server
         WriteStreamByteArray(stream, imageByteArray);
         System.Console.WriteLine(TimeStamp() + " | Sent: " + filePath);
         // Received from server
         string serverMessageString = ReadStreamString(stream, maxByteArray);
         System.Console.WriteLine(serverMessageString);
         //return serverMessageString;
      }
      static void ReadSendOnStreamString(NetworkStream stream, int byteArraySize)
      {
         // // Send start notice
         // WriteStreamString(stream, "<START>");
         // Received from server
         string serverMessageString = ReadStreamString(stream, byteArraySize);
         // Send back to server
         string clientMessageString = TimeStamp() + " | Received: " + serverMessageString;
         WriteStreamString(stream, clientMessageString);
         Console.WriteLine(clientMessageString);
         //return clientMessageString;
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



   }
}
