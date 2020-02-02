using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Drawing;

class Server
{
   const string serverIPAddress = "0.0.0.0"; const int serverPort = 11000;
   // const string saveDir = ".\\images\\";
   //const string saveDir = "..\\..\\html\\images\\";

   Random random = new Random();

   static void Main(string[] args)
   {
      Server server = new Server(serverIPAddress, serverPort);

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
      TcpClient.Client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.KeepAlive, true);
      NetworkStream stream = tCPClient.GetStream();

      string clientID = null;
      string levelID = null;
      try
      {
         // Get command
         string command = ReadSendOnStreamCommand(stream);
         if(command == "<SEND>")
         {
            // Initial Connection to get clientID
            clientID = ReadSendOnStreamConnect(stream);

            // Initial Connection to get clientID
            levelID = ReadSendOnStreamConnect3(stream);
            
            // Receiving image
            Image clientImage = ReadImageStream(stream);
            System.Console.WriteLine("\nD\n");
            // Saved to where images will be pulled from
            string filename = TimeStamp() + "_" + clientID + "_" + levelID + ".jpg";
            System.Console.WriteLine("\nE\n");
            //string filePath1 = ".\\images\\" + levelID + "\\" + filename; // WINDOWS SLASH
            string filePath1 = "./images/" + levelID + "/" + filename; // LINUX SLASH
            System.Console.WriteLine("\nF\n");
            System.Console.WriteLine("filePath1: " + filePath1);
            System.Console.WriteLine("\nG\n");
            clientImage.Save(filePath1);
            System.Console.WriteLine("\nH\n");

            System.Console.WriteLine(TimeStamp() + ", " + clientID + " image saved as: " + filename);

            // Saved to html
            //string filePath2 = "..\\..\\html\\images\\" + levelID + ".jpg"; // WINDOWS SLASH
            string filePath2 = "../../html/images/" + levelID + ".jpg"; // LINUX SLASH
            System.Console.WriteLine("filePath2: " + filePath2);
            clientImage.Save(filePath2);

            // Send file completion confirmation
            WriteStream(stream, TimeStamp() + ", " + clientID + " Image received.");
            tCPClient.Close();
         }
         if(command == "<REQUEST>")
         {
            // Initial Connection to get levelID for request
            levelID = ReadSendOnStreamConnect2(stream);

            //string[] filepaths = Directory.GetFiles(".\\images\\" + levelID); // WINDOWS SLASH
            string[] filepaths = Directory.GetFiles("./images/" + levelID); // LINUX SLASH
            int randIdx = random.Next(0, filepaths.Length);
            System.Console.WriteLine("randIdx = " + randIdx + ", length = " + filepaths.Length);
            string filepath = filepaths[randIdx];

            System.Console.WriteLine(filepath);
            // Sending image
            SendReadOnStream(stream, imageToByteArray(Image.FromFile(filepath)));
            WriteStream(stream, TimeStamp() + ", Image sent: " + filepath);
            tCPClient.Close();
         }
      }
      catch
      {
         tCPClient.Close();
         System.Console.WriteLine(TimeStamp() + ", " + clientID + " forcibly closed connection!");
      }
   }
   public static string TimeStamp()
   {
      return DateTime.Now.ToString("yyyyMMddHHmmssffff");
   }
   static string ReadSendOnStreamCommand(NetworkStream stream)
   {
      // Received from client
      string command = ReadStream(stream);

      // Send back to client
      string serverMessageString = TimeStamp() + ", " + command + " received.";
      Console.WriteLine(serverMessageString);
      WriteStream(stream, serverMessageString);
      return command;
   }
   static string ReadSendOnStreamConnect(NetworkStream stream)
   {
      // Received from client
      string clientID = ReadStream(stream);

      // Send back to client
      string serverMessageString = TimeStamp() + ", " + clientID + " connected.";
      Console.WriteLine(serverMessageString);
      WriteStream(stream, serverMessageString);
      return clientID;
   }
   static string ReadSendOnStreamConnect2(NetworkStream stream)
   {
      // Received from client
      string levelID = ReadStream(stream);

      // Send back to client
      string serverMessageString = TimeStamp() + ", Images from Level " + levelID + " requested.";
      Console.WriteLine(serverMessageString);
      WriteStream(stream, serverMessageString);
      return levelID;
   }
   static string ReadSendOnStreamConnect3(NetworkStream stream)
   {
      // Received from client
      string levelID = ReadStream(stream);

      // Send back to client
      string serverMessageString = TimeStamp() + ", Images from Level " + levelID + " being sent.";
      Console.WriteLine(serverMessageString);
      WriteStream(stream, serverMessageString);
      return levelID;
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
      System.Console.WriteLine("\nA\n");
      Byte[] byteArray = new Byte[byteArraySize];
      System.Console.WriteLine("\nB\n");
      int bytes = stream.Read(byteArray, 0, byteArray.Length);
      System.Console.WriteLine("\nC\n");
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
   static void SendReadOnStream(NetworkStream stream, Byte[] clientMessageByteArray, int byteArraySize = 1024000)
   {
      stream.Write(clientMessageByteArray, 0, clientMessageByteArray.Length);
      Byte[] serverMessageByteArray = new Byte[byteArraySize];
      int serverMessageBytes = stream.Read(serverMessageByteArray, 0, serverMessageByteArray.Length);
      System.Console.WriteLine(System.Text.Encoding.ASCII.GetString(serverMessageByteArray, 0, serverMessageBytes));
   }
}