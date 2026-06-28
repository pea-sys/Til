using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter a message, and then press Enter.");

            Task.Run(async () =>
            {
                try
                {
                    using (var stream = new NamedPipeServerStream("NamedPipe"))
                    {
                        await stream.WaitForConnectionAsync();

                        using (var writer = new StreamWriter(stream))
                        {
                            writer.AutoFlush = true;
                            while (true)
                            {
                                var data = Console.ReadLine();
                                await writer.WriteLineAsync(data);
                            }
                        }
                    }
                }
                catch (IOException)
                {
                    Console.WriteLine("Connection is closed.");
                }
            }).Wait();

            Console.WriteLine("End of sample.");
            Console.ReadLine();
        }
    }
}
