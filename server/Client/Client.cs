using System;
using System.Net.Sockets;
using System.Drawing;
using System.IO;

class Client
{
   const string serverIPAddress1 = "127.0.0.1"; const int serverPort1 = 11000;
   const string filePath = ".\\images\\test.jpg";

   static void Main(string[] args)
   {
      Client client1 = new Client(serverIPAddress1, serverPort1, "Client1");
   }

   public void SendImage()
   {

   }
   public void RequestImages()
   {

   }


   public Client(string serverIPAddress, int serverPort, string clientID)
   {
      // Connect to server and establish stream
      TcpClient client = new TcpClient(serverIPAddress, serverPort);
      NetworkStream stream = client.GetStream();

      // Confirm stream communication
      SendReceiveOnStream(stream, System.Text.Encoding.ASCII.GetBytes(clientID));

      // Body of communication
      SendReceiveOnStream(stream, imageToByteArray(Image.FromFile(filePath)));

      // Wait to confirm receipt
      string receipt = ReadStream(stream);
      if(receipt == "<EOF>")
      {
                        stream.Close();
      client.Close();
         System.Console.WriteLine(receipt);

      }



      // Close stream communication
      System.Console.WriteLine("D1");
      stream.Close();
      client.Close();
      System.Console.WriteLine("Done");
   }
   static void SendReceiveOnStream(NetworkStream stream, Byte[] clientMessageByteArray, int byteArraySize = 1024000)
   {
      stream.Write(clientMessageByteArray, 0, clientMessageByteArray.Length);
      Byte[] serverMessageByteArray = new Byte[byteArraySize];
      int serverMessageBytes = stream.Read(serverMessageByteArray, 0, serverMessageByteArray.Length);
      System.Console.WriteLine(System.Text.Encoding.ASCII.GetString(serverMessageByteArray, 0, serverMessageBytes));
   }
   static string ReadStream(NetworkStream stream, int byteArraySize = 1024) // Text
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
   public byte[] imageToByteArray(System.Drawing.Image imageIn)
   {
      MemoryStream ms = new MemoryStream();
      imageIn.Save(ms,System.Drawing.Imaging.ImageFormat.Gif);
      return ms.ToArray();
   }
}