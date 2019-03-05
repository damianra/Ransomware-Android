using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            RecData();
            Console.ReadKey();
        }
        //Metodo que recive los datos por medio de Socket
        public static void RecData()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //se configura la ip y el puerto utilizando la ip de esta maquina
            IPEndPoint miDireccion = new IPEndPoint(IPAddress.Any, 4444);
            //Array de byte a recibir
            byte[] reciveBytes = new byte[1024];
            //si la creacion del socket y la recepcion de los datos resulta exitosa
            //deserializa el contenio y lo muestra por pantalla
            //si hay algun tipo de falla salda un error de conexion.
            try
            {
                socket.Bind(miDireccion); // Asociamos el socket a miDireccion
                socket.Listen(1); // Lo ponemos a escucha
                Console.WriteLine("Listen.....");
                Socket listen = socket.Accept();
                Console.WriteLine("Recive data......");
                int a = listen.Receive(reciveBytes, 0, reciveBytes.Length, 0);
                Array.Resize(ref reciveBytes, a);
                string objJson = Encoding.Default.GetString(reciveBytes);
                List<Contact> contact = JsonConvert.DeserializeObject<List<Contact>>(objJson);
                foreach(var con in contact)
                {
                    Console.WriteLine(con.id + "--" + con.name + "--" + con.phone);
                    Console.WriteLine("------------------------------------------------");
                }
                Console.WriteLine("Fin de la conexion");
            }
            catch(Exception ex)
            {
                Console.WriteLine("Problemas en la conexion: " + ex.Message);
            }
        }
    }
}
