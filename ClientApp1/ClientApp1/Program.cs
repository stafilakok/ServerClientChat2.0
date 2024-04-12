using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    static void Main()
    {
        Console.WriteLine(@"
                              
           88                       ,d     
           88                       88     
 ,adPPYba, 88,dPPYba,  ,adPPYYba, MM88MMM  
a8""     ""  88P'    ""8a ""      `Y8   88     
8b         88       88 ,adPPPPP88   88     
""8a,   ,aa 88       88 88,    ,88   88,    
 `""Ybbd8'"" 88       88 `""8bbdP""Y8   ""Y888
");

        IPAddress ip;
        while (true)
        {
            Console.Write("Введите IP-адрес: ");
            if (IPAddress.TryParse(Console.ReadLine(), out ip))
                break;
            else
                Console.WriteLine("Неверный IP-адрес. Пожалуйста, введите действительный IP-адрес.");
        }

        int port;
        while (true)
        {
            Console.Write("Введите порт: ");
            if (int.TryParse(Console.ReadLine(), out port))
                break;
            else
                Console.WriteLine("Неверный порт. Пожалуйста, введите действительный порт.");
        }

        var tcpClient = new TcpClient();

        try
        {
            tcpClient.Connect(ip, port);
            tcpClient.NoDelay = true; // Отключаем алгоритм Нейгла
            Console.WriteLine("Подключение установлено.");

            var inputThread = new Thread(new ThreadStart(() => ReadInput(tcpClient)));
            inputThread.Start();

            while (true)
            {
                var data = new byte[256];
                var response = "";
                var stream = tcpClient.GetStream();
                do
                {
                    var bytes = stream.Read(data, 0, data.Length);
                    response += Encoding.UTF8.GetString(data, 0, bytes);
                }
                while (stream.DataAvailable);

                Console.WriteLine($"Получено сообщение: {response}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            tcpClient.Close();
        }
    }

    static void ReadInput(TcpClient client)
    {
        while (true)
        {

            var message = Console.ReadLine();
            var data = Encoding.UTF8.GetBytes(message);
            var stream = client.GetStream();
            stream.Write(data, 0, data.Length);
        }
    }
}
