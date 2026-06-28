using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                using (var stream = new NamedPipeClientStream("NamedPipe"))
                {
                    await stream.ConnectAsync();

                    using (var reader = new StreamReader(stream))
                    {
                        while (stream.IsConnected)
                        {
                            var str = await reader.ReadLineAsync();
                            Console.WriteLine("Data :" + str);
                            Thread.Sleep(100);
                        }
                    }
                }
            }).Wait();
        }
    }
}
