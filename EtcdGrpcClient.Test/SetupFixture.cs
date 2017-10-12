using NUnit.Framework;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace EtcdGrpcClient.Test
{
    [SetUpFixture]
    public class SetupFixture
    {
        Process process;

        [OneTimeSetUp]
        public async Task RunEtcd()
        {
            FindAndKillEtcdProcesses();
            process = new Process();

            process.StartInfo.FileName = @".\etcd_windows\etcd.exe";
            process.StartInfo.Arguments = @"--name infra0 --initial-advertise-peer-urls http://localhost:2380 --listen-peer-urls http://localhost:2380 --listen-client-urls http://localhost:2381 --advertise-client-urls http://localhost:2381";

            process.StartInfo.UseShellExecute = true;
            process.StartInfo.CreateNoWindow = false;

            process.Start();
        }

        [OneTimeTearDown]
        public void KillEtcd()
        {
            process.CloseMainWindow();
            process.Kill();
            process.Dispose();
        }

        private void FindAndKillEtcdProcesses()
        {
            Process[] processes = Process.GetProcessesByName("etcd");
            foreach (Process p in processes)
            {
                p.Kill();
                p.Dispose();
            }
        }
    }
}