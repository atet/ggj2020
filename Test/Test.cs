using System;
using System.IO;

// using System.Drawing;

using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

class Test
{
   static void Main(string[] args)
   {



      // // Uses functions below, need non-core System.Drawing.Common
      // Byte[] imageByteArray = imageFilePathToByteArray(filePath);
      // byteArrayToImageFilePath(imageByteArray, ".\\images\\1_out.png");

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
   // public static byte[] imageFilePathToByteArray(string filePath)
   // {
   //    Image image = Image.FromFile(filePath);
   //    using (MemoryStream mStream = new MemoryStream())
   //    {
   //       image.Save(mStream, image.RawFormat);
   //       return mStream.ToArray();
   //    }  
   // }
   // public static void byteArrayToImageFilePath(byte[] byteArrayIn, string filePath)
   // {
   //    using(var ms = new MemoryStream(byteArrayIn))
   //    {
   //       Image image = Image.FromStream(ms);
   //       image.Save(filePath);
   //    }
   // }
}
