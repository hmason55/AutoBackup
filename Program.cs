using AutoBackup.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace AutoBackup;

public class Program
{
    public static string[] ARGS;
    public const string APPLICATION_NAME = "AutoBackup";

    public static void Main(string[] args)
    {
        try
        {
            ARGS = args;
            if(args.Length != 0)
            {
                CreateHostBuilder(args).UseWindowsService().Build().Run();
                return;
            }

            Console.WriteLine($"If you have not configured your settings, please modify the configuration located at:\n{Path.Combine(Environment.CurrentDirectory, Config.CONFIG_PATH)}\n");
            Console.WriteLine("Press any key to proceed with the installation...");
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("Installing AutoBackup service...\n");

            Process process = new()
            {
                StartInfo =
                {
                    FileName = "cmd.exe",
                    WorkingDirectory = Environment.CurrentDirectory,
                    Arguments = $"/C sc create {APPLICATION_NAME} binpath=\"{Path.Combine(Environment.CurrentDirectory, APPLICATION_NAME)}.exe {Path.Combine(Environment.CurrentDirectory, Config.CONFIG_PATH)}\" start=auto & sc start {APPLICATION_NAME} & pause"
                }
            };

            process.Start();
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<Worker>();
            });
}
