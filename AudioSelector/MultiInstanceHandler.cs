using System;
using System.IO.Pipes;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Text;

namespace AudioSelector
{

    internal class MultiInstanceHandler
    {
        private const string MutexName = "AudioSelector.{6E309A77-21F8-49F2-B57D-AE6E0D4940DD}";
        private const string PipeName = "AudioSelector.{E2D88EB5-CF16-4367-B7DB-EB3F2A10986D}";
        private const string CommandMultiInstance = "MultiInstance.{F6F3222F-D815-4323-A566-5B574404CBA4}";

        public delegate void OnAnotherAppLaunched();
        public event OnAnotherAppLaunched AnotherAppLaunched;
        private CancellationTokenSource cts;
        private Mutex instanceMutex;
        private Task serverTask;
        private int stopRequested;


        public bool Start()
        {
            instanceMutex = new Mutex(false, MutexName, out bool createdNew);

            if (!createdNew)
            {
                NotifyExistingInstance();
                instanceMutex.Dispose();
                instanceMutex = null;
                return false;
            }

            StartPipeServer();
            return true;
        }

        public void Stop()
        {
            if (Interlocked.Exchange(ref stopRequested, 1) != 0)
            {
                return;
            }

            cts?.Cancel();

            try
            {
                serverTask?.Wait(TimeSpan.FromSeconds(2));
            }
            catch (AggregateException ex)
            {
                foreach (Exception inner in ex.InnerExceptions)
                {
                    Debug.WriteLine(inner.Message);
                }
            }

            cts?.Dispose();
            cts = null;
            serverTask = null;

            if (instanceMutex != null)
            {
                instanceMutex.Dispose();
                instanceMutex = null;
            }
        }

        private void StartPipeServer()
        {
            cts = new CancellationTokenSource();
            stopRequested = 0;

            serverTask = Task.Run(async () =>
            {
                while (!cts.IsCancellationRequested)
                {
                    NamedPipeServerStream server = null;

                    try
                    {
                        server = CreateServerStream();
                        await server.WaitForConnectionAsync(cts.Token);
                        using StreamReader reader = new(server, Encoding.UTF8, leaveOpen: false);
                        server = null;
                        string message = await reader.ReadLineAsync();

                        if (CommandMultiInstance == message)
                        {
                            AnotherAppLaunched?.Invoke();
                        }
                    }
                    catch (OperationCanceledException) when (cts.IsCancellationRequested)
                    {
                        break;
                    }
                    catch (IOException ex) when (!cts.IsCancellationRequested)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);

                        if (cts.IsCancellationRequested)
                        {
                            break;
                        }
                    }
                    finally
                    {
                        server?.Dispose();
                    }
                }
            }, cts.Token);
        }

        private void NotifyExistingInstance()
        {
            try
            {
                using NamedPipeClientStream client = new(".", PipeName, PipeDirection.Out);
                client.Connect(1000);
                using StreamWriter writer = new(client, Encoding.UTF8) { AutoFlush = true };
                writer.WriteLine(CommandMultiInstance);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private static NamedPipeServerStream CreateServerStream()
        {
            return new NamedPipeServerStream(
                PipeName,
                PipeDirection.In,
                1,
                PipeTransmissionMode.Byte,
                PipeOptions.Asynchronous);
        }
    }
}
