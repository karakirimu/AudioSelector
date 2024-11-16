using System;
using System.IO.Pipes;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace AudioSelector
{

    internal class MultiInstanceHandler
    {
        private const string PipeName = "AudioSelector.{E2D88EB5-CF16-4367-B7DB-EB3F2A10986D}";
        private const string CommandMultiInstance = "MultiInstance.{F6F3222F-D815-4323-A566-5B574404CBA4}";

        public delegate void OnAnotherAppLaunched();
        public event OnAnotherAppLaunched AnotherAppLaunched;
        CancellationTokenSource cts;


        public bool Start()
        {
            bool isServerInstance;

            try
            {
                using NamedPipeClientStream client = new(".", PipeName, PipeDirection.Out);
                client.Connect(1000);
                isServerInstance = false;
            }
            catch (Exception)
            {
                isServerInstance = true;
            }

            if (!isServerInstance)
            {
                using NamedPipeClientStream client = new(".", PipeName, PipeDirection.Out);
                client.Connect();
                using StreamWriter writer = new(client);
                writer.WriteLine(CommandMultiInstance);
                return false;
            }

            // NamedPipeサーバーを開始
            StartPipeServer();
            return true;
        }

        public void Stop()
        {
            cts?.Cancel();
        }

        private void StartPipeServer()
        {
            cts = new CancellationTokenSource();

            Task.Run(() =>
            {
                while (!cts.IsCancellationRequested)
                {
                    try
                    {
                        using NamedPipeServerStream server = new(PipeName, PipeDirection.In);
                        server.WaitForConnection();
                        using StreamReader reader = new(server);
                        string message = reader.ReadLine();

                        if (CommandMultiInstance == message)
                        {
                            AnotherAppLaunched?.Invoke();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        break;
                    }
                }
            }, cts.Token);
        }
    }
}
