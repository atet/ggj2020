using System;
using System.Net;
using System.Net.Sockets;

namespace Server
{
   class Server
   {
      // const string serverIPAddress = "0.0.0.0"; const int serverPort = 11000;
      // //const string saveDir1 = @".\images\"; const string saveDir2 = @"..\html\images\"; // WINDOWS SLASH
      // const string saveDir1 = "./images/"; const string saveDir2 = "../html/images/"; // LINUX SLASH
      // Random random = new Random();
      // static int currentImageCounter = 1;
      // static int maxImageCount = 30;

      public static int Main(String[] args)
      {
         string serverHostName = "ggj.atetkao.com";
         int serverPort = 11000;

         IPAddress serverIPAddress = Dns.GetHostEntry(serverHostName).AddressList[0];
         IPEndPoint serverEndPoint = new IPEndPoint(serverIPAddress, serverPort);
         Socket serverSocket = new Socket(serverIPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
         serverSocket.Bind(serverEndPoint);
         serverSocket.Listen(10);

         while(true)
         {
            // Waiting for clients to connect
            Console.Write("Listening...\r");
            Socket handlerSocket = serverSocket.Accept();
            try
            {
               // 1. Receive clientID
               string clientID = ReadSendString(handlerSocket);

               // 2. Receive fileByteArray
               //string filePathName = @".\images\BENCHMARK_CLIENT.jpg";
               string filePathName = "../html/images/1.jpg";
               ReadSendFile(handlerSocket, filePathName);

               // System.Console.WriteLine($"clientID: { clientID }");
               // System.Console.WriteLine($"imageByteLength: { imageByteLength }");

               // Release the socket
               handlerSocket.Shutdown(SocketShutdown.Both); handlerSocket.Close(); System.Console.WriteLine(TimeStamp() + " | Gracefully closed connection.");
            }
            catch
            {
               // Release the socket
               handlerSocket.Shutdown(SocketShutdown.Both); handlerSocket.Close(); System.Console.WriteLine(TimeStamp() + " | Forcibly closed connection!");
            }

            


            // // Client message
            // clientMessageBytes = handlerSocket.Receive(clientMessageByteArray);
            // clientMessageString = System.Text.Encoding.ASCII.GetString(clientMessageByteArray, 0, clientMessageBytes);
            // clientMessageString = TimeStamp() + " | " + clientMessageString;
            // Console.WriteLine(clientMessageString);

            // // Server response
            // serverMessageString = clientMessageString;
            // serverMessageByteArray = System.Text.Encoding.ASCII.GetBytes(serverMessageString);
            // handlerSocket.Send(serverMessageByteArray);
            
            

         }
      }
      public static string ReadSendString(Socket handlerSocket, int maxByteLength = 1024)
      {
         byte[] serverMessageByteArray = new byte[maxByteLength];
         int byteLength = handlerSocket.Receive(serverMessageByteArray);
         string clientMessage = System.Text.Encoding.ASCII.GetString(serverMessageByteArray, 0, byteLength);
         string clientResponse = $"{ TimeStamp()} | { clientMessage }";
         handlerSocket.Send(System.Text.Encoding.ASCII.GetBytes(clientResponse));
         System.Console.WriteLine(clientResponse);
         return clientMessage;
      }
      public static void ReadSendFile(Socket handlerSocket, string filePath, int maxByteLength = 1024)
      {
         int imageByteLength = Int32.Parse(ReadSendString(handlerSocket, maxByteLength));
         byte[] imageByteArray = new byte[imageByteLength];
         int imageByteLength2 = handlerSocket.Receive(imageByteArray);

         string clientResponse = $"{ TimeStamp()} | File received ({ imageByteLength2.ToString() } bytes)";
         handlerSocket.Send(System.Text.Encoding.ASCII.GetBytes(clientResponse), 0, maxByteLength, SocketFlags.None);
         System.Console.WriteLine(clientResponse);

         System.IO.File.WriteAllBytes(filePath, imageByteArray);
      }


      // public void ReceiveSendOnStream(Object obj)
      // {
      //    TcpClient client = (TcpClient)obj;
      //    NetworkStream stream = client.GetStream();
      //    try
      //    {
      //       string command = ReadSendOnStreamString(stream, maxByteArray);
      //       if(command == "<SEND>")
      //       {
      //          ReadSendOnStreamImage(stream, maxByteArray);
      //       }
      //       client.Close();
      //       System.Console.WriteLine(TimeStamp() + " | Gracefully closed connection.");
      //    }
      //    catch
      //    {
      //       client.Close();
      //       System.Console.WriteLine(TimeStamp() + " | Forcibly closed connection!");
      //    }
      // }
      // static string ReadSendOnStreamString(NetworkStream stream, int byteArraySize)
      // {
      //    // Received from client
      //    string clientMessageString = ReadStreamString(stream, byteArraySize);
      //    // Send back to client
      //    string serverMessageString = TimeStamp() + " | Received: " + clientMessageString;
      //    WriteStreamString(stream, serverMessageString);
      //    Console.WriteLine(serverMessageString);
      //    return clientMessageString;
      // }
      // static void ReadSendOnStreamImage(NetworkStream stream, int byteArraySize)
      // {
      //    // Receive clientID
      //    string clientID = ReadSendOnStreamString(stream, 1024);
      //    // Receive imageByteLength
      //    int imageByteLength = Int32.Parse(ReadSendOnStreamString(stream, 1024));
         
      //    // Receive image
      //    Byte[] byteArray = new Byte[imageByteLength + 1048576];
      //    stream.Read(byteArray, 0, imageByteLength + 1048576);
      //    // Send back to client
      //    string serverMessageString = $"{ TimeStamp() } | Received image of imageByteLength { imageByteLength }";
      //    WriteStreamString(stream, serverMessageString);
      //    Console.WriteLine(serverMessageString);

      //    // Save image locally
      //    string filePath1 = $"{ saveDir1 }{ TimeStamp() }_{ clientID }.jpg";
      //    System.IO.File.WriteAllBytes(filePath1, byteArray);
      //    serverMessageString = $"{ TimeStamp() } | Image saved as: { filePath1 }";
      //    Console.WriteLine(serverMessageString);

      //    string filePath2 = $"{ saveDir2 }{ currentImageCounter }.jpg";
      //    currentImageCounter++; if(currentImageCounter > maxImageCount){ currentImageCounter = 1; }
      //    System.IO.File.WriteAllBytes(filePath2, byteArray);
      //    serverMessageString = $"{ TimeStamp() } | Image saved as: { filePath2 }";
      //    Console.WriteLine(serverMessageString);
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





      // static void Main(string[] args)
      // {
      //    Server server = new Server();
      //    while(true) // Realtime log of connected clients
      //    {
      //       System.Console.Write("Listening...\r"); Thread.Sleep(250);
      //    }
      // }
      // public Server()
      // {
      //    Thread thread = new Thread(
      //       delegate()
      //       {
      //          TcpListener serverTCPListener = new TcpListener(IPAddress.Parse(serverIPAddress), serverPort);
      //          serverTCPListener.Start();
      //          StartListener(serverTCPListener);
      //       }
      //    );
      //    thread.IsBackground = true;
      //    thread.Start();
      // }
      // public void StartListener(TcpListener serverTCPListener)
      // {
      //    try
      //    {
      //       while(true)
      //       {
      //          TcpClient tCPClient = serverTCPListener.AcceptTcpClient();
      //          Thread thread = new Thread( new ParameterizedThreadStart(ReceiveSendOnStream) );
      //          thread.Start(tCPClient);
      //       }
      //    }
      //    catch
      //    {
      //       serverTCPListener.Stop();
      //    }
      // }

      // ### HELPER FUNCTIONS ###
      public static string TimeStamp()
      {
         return DateTime.Now.ToString("yyyyMMddHHmmssffff");
      }
   }
}
