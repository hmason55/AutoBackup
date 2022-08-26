using AutoBackup.Utilities;
using Microsoft.Extensions.Hosting;

namespace AutoBackup.Models;

public class Worker : BackgroundService
{
    private static Config _config { get; set; } = new();
    private static bool _running = true;
    private const int _pollIntervalMs = 1000;

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        if(Program.ARGS != null && Program.ARGS.Length > 0)
        {
            Config.CONFIG_PATH = Program.ARGS[0];
        }

        try
        {
            _config = Config.Load();
            Config.Save(_config);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            return;
        }

        while (_running)
        {
            DateTime updateTime = _config.LastUpdate.AddMinutes(_config.UpdateIntervalInMinutes);

            if (DateTime.Now >= updateTime)
            {
                FileHandler.Backup(_config.TrackedFiles);
                FileHandler.Clean(_config.TrackedFiles, _config.MaximumHistory);
                _config.LastUpdate = DateTime.Now;

                Config.Save(_config);
                _config = Config.Load();
            }

            await Task.Delay(_pollIntervalMs, token);
        }
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        return base.StopAsync(cancellationToken);
    }
}
