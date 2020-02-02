using System;
using System.Net.Sockets;
using System.Drawing;
using System.IO;

class Client
{ 
   static void Main(string[] args)
   {
      string filePath = ".\\images\\test.jpg";
      SendImage(filePath);
   }

   static public void SendImage(string filePath, string serverIPAddress = "127.0.0.1", int serverPort = 11000, string clientID = "127001000a959d6816")
   {
      // NO SPECIAL CHARACTERS FOR CLIENTID!!!

      // Connect to server and establish stream
      TcpClient client = new TcpClient(serverIPAddress, serverPort);
      NetworkStream stream = client.GetStream();

      // Confirm stream communication
      SendReadOnStream(stream, System.Text.Encoding.ASCII.GetBytes(clientID));

      // Body of communication, Wait to confirm receipt
      SendReadOnStream(stream, imageToByteArray(Image.FromFile(filePath)));

      // Close stream communication
      stream.Close();
      client.Close();
   }
   public void RequestImages()
   {
   }
   static void SendReadOnStream(NetworkStream stream, Byte[] clientMessageByteArray, int byteArraySize = 1024000)
   {
      stream.Write(clientMessageByteArray, 0, clientMessageByteArray.Length);
      Byte[] serverMessageByteArray = new Byte[byteArraySize];
      int serverMessageBytes = stream.Read(serverMessageByteArray, 0, serverMessageByteArray.Length);
      System.Console.WriteLine(System.Text.Encoding.ASCII.GetString(serverMessageByteArray, 0, serverMessageBytes));
   }
   static void SendOnStream(NetworkStream stream, Byte[] clientMessageByteArray, int byteArraySize = 1024000)
   {
      stream.Write(clientMessageByteArray, 0, clientMessageByteArray.Length);
   }
   static string ReadOnStream(NetworkStream stream, int byteArraySize = 1024) // Text
   {
      Byte[] byteArray = new Byte[byteArraySize];
      int bytes = stream.Read(byteArray, 0, byteArray.Length);
      return System.Text.Encoding.ASCII.GetString(byteArray, 0, bytes);
   }
   static public Image byteArrayToImage(byte[] byteArrayIn)
   {
      MemoryStream ms = new MemoryStream(byteArrayIn);
      Image returnImage = Image.FromStream(ms);
      return returnImage;
   }
   static public byte[] imageToByteArray(System.Drawing.Image imageIn)
   {
      MemoryStream ms = new MemoryStream();
      imageIn.Save(ms,System.Drawing.Imaging.ImageFormat.Gif);
      return ms.ToArray();
   }
   public Client()
   {
   }
}