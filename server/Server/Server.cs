using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Drawing;
class Server
{
   const string serverIPAddress1 = "127.0.0.1"; const int serverPort1 = 11000;

   static void Main(string[] args)
   {
      Server server1 = new Server(serverIPAddress1, serverPort1);

      while(true) // Realtime log of connected clients
      {
         System.Console.Write("Listening...\r");
         Thread.Sleep(250);
      }
   }
   public Server(string serverIPAddress, int serverPort){
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
            Thread thread = new Thread(
               new ParameterizedThreadStart(ReceiveSendOnStream)
            );
            thread.Start(tCPClient);
         }
      }
      catch
      {
         serverTCPListener.Stop();
      }
   }
   public void ReceiveSendOnStream(Object obj)
   {
      TcpClient tCPClient = (TcpClient)obj;
      NetworkStream stream = tCPClient.GetStream();

      string clientID = null; 
      try
      {
         // Initial Connection to register client
         clientID = ReceiveSendOnStreamConnect(stream);

         // Receiving image
         Image clientImage = ReadImageStream(stream);
         string filename = TimeStamp() + "_" + clientID + ".jpg";
         clientImage.Save(".\\images\\" + filename);
         System.Console.WriteLine(TimeStamp() + ", " + clientID + " image saved as: " + filename);

         // Send file completion confirmation
         WriteStream(stream, "<EOF>");
//TimeStamp() + ", " + clientID + " file received."
      }
      catch
      {
         tCPClient.Close();
         System.Console.WriteLine(TimeStamp() + ", " + clientID + " forcibly closed connection");
      }

   }
   public static string TimeStamp()
   {
      return DateTime.Now.ToString("yyyyMMddHHmmssffff");
   }
   static string ReceiveSendOnStreamConnect(NetworkStream stream)
   {
      // Received from client
      string clientID = ReadStream(stream);

      // Send back to client
      string serverMessageString = TimeStamp() + ", " + clientID + " connected.";
      Console.WriteLine(serverMessageString);
      WriteStream(stream, serverMessageString);
      return clientID;
   }
   static string ReadStream(NetworkStream stream, int byteArraySize = 1024) // Text
   {
      Byte[] byteArray = new Byte[byteArraySize];
      int bytes = stream.Read(byteArray, 0, byteArray.Length);
      return System.Text.Encoding.ASCII.GetString(byteArray, 0, bytes);
   }
   static void WriteStream(NetworkStream stream, string message)
   {
      Byte[] byteArray = System.Text.Encoding.ASCII.GetBytes(message);
      stream.Write(byteArray, 0, byteArray.Length);
   }
   static Image ReadImageStream(NetworkStream stream, int byteArraySize = 1024000) // Image
   {
      Byte[] byteArray = new Byte[byteArraySize];
      int bytes = stream.Read(byteArray, 0, byteArray.Length);
      return byteArrayToImage(byteArray);
   }
   static public Image byteArrayToImage(byte[] byteArrayIn)
   {
      MemoryStream ms = new MemoryStream(byteArrayIn);
      Image returnImage = Image.FromStream(ms);
      return returnImage;
   }
   public byte[] imageToByteArray(Image imageIn)
   {
      MemoryStream ms = new MemoryStream();
      imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
      return ms.ToArray();
   }
}