using System;
using System.Net.Sockets;
using System.Drawing;
using System.IO;

class Client
{ 
   static void Main(string[] args)
   {
      string dirRead = ".\\imageRead\\";
      // RequestImageSaveToFile(dirRead, "01", "165.227.54.194");
      RequestImageSaveToFile(dirRead, "01", "127.0.0.1");

      // string filePathSend;
      // filePathSend = ".\\imageSend\\01.jpg";
      // // SendImageFromFile(filePathSend, "client1", "165.227.54.194");
      // SendImageFromFile(filePathSend, "client1", "127.0.0.1");

      // filePathSend = ".\\imageSend\\01.jpg";
      // SendImageFromFile(filePathSend, "client1");

      // filePathSend = ".\\imageSend\\02.jpg";
      // SendImageFromFile(filePathSend, "client1");

      // filePathSend = ".\\imageSend\\03.jpg";
      // SendImageFromFile(filePathSend, "client1");

      // filePathSend = ".\\imageSend\\04.jpg";
      // SendImageFromFile(filePathSend, "client1");

      // filePathSend = ".\\imageSend\\05.jpg";
      // SendImageFromFile(filePathSend, "client1");

      // filePathSend = ".\\imageSend\\06.jpg";
      // SendImageFromFile(filePathSend, "client1");

      // filePathSend = ".\\imageSend\\07.jpg";
      // SendImageFromFile(filePathSend, "client1");

      // filePathSend = ".\\imageSend\\08.jpg";
      // SendImageFromFile(filePathSend, "client1");

      // filePathSend = ".\\imageSend\\09.jpg";
      // SendImageFromFile(filePathSend, "client1");

      // filePathSend = ".\\imageSend\\10.jpg";
      // SendImageFromFile(filePathSend, "client1");

      // filePathSend = ".\\imageSend\\11.jpg";
      // SendImageFromFile(filePathSend, "client1");

      // filePathSend = ".\\imageSend\\12.jpg";
      // SendImageFromFile(filePathSend, "client1");

      // string dirRead = ".\\imageRead\\";
      // RequestImageSaveToFile(dirRead, "01");
      // RequestImageSaveToFile(dirRead, "01");
      // RequestImageSaveToFile(dirRead, "02");
      // RequestImageSaveToFile(dirRead, "02");
      // RequestImageSaveToFile(dirRead, "03");
      // RequestImageSaveToFile(dirRead, "03");
   }

   public static void SendImageFromFile(string filePathSend, string clientID, string serverIPAddress = "127.0.0.1", int serverPort = 11000)
   {
      // ONLY ALPHANUMERIC FOR clientID

      // levelID is the filename, e.g. 01, 02, 03,...
      string levelID = Path.GetFileNameWithoutExtension(filePathSend);
      //System.Console.WriteLine(levelID);

      // Connect to server and establish stream
      TcpClient client = new TcpClient(serverIPAddress, serverPort);
      NetworkStream stream = client.GetStream();

      // Send command
      SendReadOnStream(stream, System.Text.Encoding.ASCII.GetBytes("<SEND>"));

      // Send clientID
      SendReadOnStream(stream, System.Text.Encoding.ASCII.GetBytes(clientID));

      // Send levelID
      SendReadOnStream(stream, System.Text.Encoding.ASCII.GetBytes(levelID));

      // Body of communication, Wait to confirm receipt
      SendReadOnStream(stream, imageToByteArray(Image.FromFile(filePathSend)));

      
      // Close stream communication
      stream.Close();
      client.Close();

      System.Console.WriteLine("F");
   }
   // public static void SendImage(Image imageSend, string clientID, string levelID, string serverIPAddress = "127.0.0.1", int serverPort = 11000)
   // {
   //    // ONLY ALPHANUMERIC FOR clientID

   //    // levelID is the filename, e.g. 01, 02, 03,...
   //    System.Console.WriteLine(levelID);

   //    // Connect to server and establish stream
   //    TcpClient client = new TcpClient(serverIPAddress, serverPort);
   //    NetworkStream stream = client.GetStream();

   //    // Send command
   //    SendReadOnStream(stream, System.Text.Encoding.ASCII.GetBytes("<SEND>"));

   //    // Send clientID
   //    SendReadOnStream(stream, System.Text.Encoding.ASCII.GetBytes(clientID));

   //    // Send levelID
   //    SendReadOnStream(stream, System.Text.Encoding.ASCII.GetBytes(levelID));

   //    // Body of communication, Wait to confirm receipt
   //    SendReadOnStream(stream, imageToByteArray(imageSend));

   //    // Close stream communication
   //    stream.Close();
   //    client.Close();
   // }
   public static void RequestImageSaveToFile(string dirRead, string levelID, string serverIPAddress = "127.0.0.1", int serverPort = 11000)
   {
      // Connect to server and establish stream
      TcpClient client = new TcpClient(serverIPAddress, serverPort);
      NetworkStream stream = client.GetStream();

      // Send command
      SendReadOnStream(stream, System.Text.Encoding.ASCII.GetBytes("<REQUEST>"));

      // Send levelID
      SendReadOnStream(stream, System.Text.Encoding.ASCII.GetBytes(levelID));

      // Receiving image
      Image clientImage = ReadImageStream(stream);
      string filename = levelID + ".jpg";
      clientImage.Save(dirRead + filename);

      System.Console.WriteLine("Received Image.");

      // Close stream communication
      stream.Close();
      client.Close();
   }
   // public static Image RequestImage(string levelID, string serverIPAddress = "127.0.0.1", int serverPort = 11000)
   // {
   //    // Connect to server and establish stream
   //    TcpClient client = new TcpClient(serverIPAddress, serverPort);
   //    NetworkStream stream = client.GetStream();

   //    // Send command
   //    SendReadOnStream(stream, System.Text.Encoding.ASCII.GetBytes("<REQUEST>"));

   //    // Send levelID
   //    SendReadOnStream(stream, System.Text.Encoding.ASCII.GetBytes(levelID));

   //    // Receiving image
   //    Image clientImage = ReadImageStream(stream);

   //    System.Console.WriteLine("Received Image.");

   //    // Close stream communication
   //    stream.Close();
   //    client.Close();

   //    return clientImage;
   // }
   static void SendReadOnStream(NetworkStream stream, Byte[] clientMessageByteArray, int byteArraySize = 1024000)
   {
      System.Console.WriteLine("A " + clientMessageByteArray.Length.ToString());
      stream.Write(clientMessageByteArray, 0, clientMessageByteArray.Length);
            System.Console.WriteLine("B");
      Byte[] serverMessageByteArray = new Byte[byteArraySize];
            System.Console.WriteLine("C");
      int serverMessageBytes = stream.Read(serverMessageByteArray, 0, serverMessageByteArray.Length);
            System.Console.WriteLine("D " + serverMessageBytes.ToString());
      System.Console.WriteLine(System.Text.Encoding.ASCII.GetString(serverMessageByteArray, 0, serverMessageBytes));
            System.Console.WriteLine("A");
   }
   static public Image byteArrayToImage(byte[] byteArrayIn)
   {
      MemoryStream ms = new MemoryStream(byteArrayIn);
      Image returnImage = Image.FromStream(ms);
      return returnImage;
   }
   public static byte[] imageToByteArray(Image imageIn)
   {
      MemoryStream ms = new MemoryStream();
      imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
      return ms.ToArray();
   }
   public static Image ReadImageStream(NetworkStream stream, int byteArraySize = 1024000) // Image
   {
      Byte[] byteArray = new Byte[byteArraySize];
      int bytes = stream.Read(byteArray, 0, byteArray.Length);
      return byteArrayToImage(byteArray);
   }
}