using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    private static TcpClient client;
    private static object lockObject = new object();

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

        var tcpListener = new TcpListener(ip, port);
        Console.Write("Грузим говнокод");
        for (int i = 0; i < 10; i++)
        {
            Thread.Sleep(500);
            Console.Write(".");
        }
        Console.WriteLine("\nЗагрузка говнокода завершена!");

        tcpListener.Start();
        Console.WriteLine("Сервер запущен, ожидание подключений...");

        while (true)
        {
            client = tcpListener.AcceptTcpClient();
            var thread = new Thread(new ParameterizedThreadStart(HandleClient));
            thread.Start(client);
        }
    }

    static void HandleClient(object obj)
    {
        var client = (TcpClient)obj;
        var stream = client.GetStream();
        var buffer = new byte[256];

        var readThread = new Thread(() =>
        {
            while (true)
            {
                var bytes = stream.Read(buffer, 0, buffer.Length);
                if (bytes == 0) break;
                var data = Encoding.UTF8.GetString(buffer, 0, bytes);
                lock (lockObject)
                {
                    Console.WriteLine($"Получено сообщение : {data}");
                }
            }
        });
        readThread.Start();

        var writeThread = new Thread(() =>
        {
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    lock (lockObject)
                    {

                        var message = Console.ReadLine();
                        var response = Encoding.UTF8.GetBytes(message);
                        stream.Write(response, 0, response.Length);
                    }
                }
            }
        });
        writeThread.Start();

        readThread.Join();
        writeThread.Join();

        client.Close();
    }
}
