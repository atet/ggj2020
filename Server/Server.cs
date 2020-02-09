using System;
using System.Net;
using System.Net.Sockets;

namespace Server
{
   class Server
   {
       const string serverHostName = "ggj.atetkao.com"; const int serverPort = 11000;
      // //const string saveDir1 = @".\images\"; const string saveDir2 = @"..\html\images\"; // WINDOWS SLASH
      const string saveDir1 = "./images/"; const string saveDir2 = "../html/images/"; // LINUX SLASH
      // Random random = new Random();
      static int currentImageCounter = 1;
      static int maxImageCount = 30;

      public static int Main(String[] args)
      {
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
               string filePathName = $"{ saveDir2 }{ currentImageCounter }.jpg";
               currentImageCounter++; if(currentImageCounter > maxImageCount){ currentImageCounter = 1; }
               ReadSendFile(handlerSocket, filePathName);

               // Release the socket
               handlerSocket.Shutdown(SocketShutdown.Both); handlerSocket.Close(); System.Console.WriteLine(TimeStamp() + " | Gracefully closed connection.");
            }
            catch
            {
               // Release the socket
               handlerSocket.Shutdown(SocketShutdown.Both); handlerSocket.Close(); System.Console.WriteLine(TimeStamp() + " | Forcibly closed connection!");
            }
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
         //imageByteLength = handlerSocket.Receive(imageByteArray);

         var total = 0;
         do
         {
               var read = handlerSocket.Receive(imageByteArray, total, imageByteLength - total, SocketFlags.None);
               //Console.WriteLine("Client recieved {0} bytes", total);
               if (read == 0)
               {
                  //If it gets here and you received 0 bytes it means that the Socket has Disconnected gracefully (without throwing exception) so you will need to handle that here
               }
               total += read;
               //If you have sent 1024 bytes and Receive only 512 then it wil continue to recieve in the correct index thus when total is equal to 1024 you will have recieved all the bytes
         }
         while(total != imageByteLength);

         string clientResponse = $"{ TimeStamp()} | File received ({ imageByteLength.ToString() } bytes), saved as { filePath }";
         handlerSocket.Send(System.Text.Encoding.ASCII.GetBytes(clientResponse));
         System.Console.WriteLine(clientResponse);

         System.IO.File.WriteAllBytes(filePath, imageByteArray);
      }

      // ### HELPER FUNCTIONS ###
      public static string TimeStamp()
      {
         return DateTime.Now.ToString("yyyyMMddHHmmssffff");
      }
   }
}
