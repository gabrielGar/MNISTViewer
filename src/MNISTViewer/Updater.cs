using System;
using NLog;
using Squirrel;
using System.Threading.Tasks;
using System.Configuration;

namespace MNISTViewer
{
    public static class Updater
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly string UpdatePath = ConfigurationManager.AppSettings["UpdateLocation"];

        public static async Task<bool> CheckUpdates()
        {
            logger.Info("Begin update check...");
            try
            {
                using (var mgr = new UpdateManager(UpdatePath))
                {
                    var updateInfo = await mgr.CheckForUpdate();
                    var update = updateInfo.CurrentlyInstalledVersion.Version < updateInfo.FutureReleaseEntry.Version;
                    logger.Info($"{updateInfo.CurrentlyInstalledVersion.Version} < {updateInfo.FutureReleaseEntry.Version} = {update}");
                    return update;
                }
            }
            catch (Exception error)
            {
                logger.Error(error, "ApplyUpdates Exception");
                return false;
            }
        }

        public static void Restart()
        {
            UpdateManager.RestartApp();
        }

        public static async Task<bool> ApplyUpdates()
        {
            if (!CanUpdate())
                return false;
            try
            {
                bool shouldRestart = false;
                using (var mgr = new UpdateManager(UpdatePath))
                {
                    var updateInfo = await mgr.CheckForUpdate();
                    if (updateInfo.CurrentlyInstalledVersion.Version < updateInfo.FutureReleaseEntry.Version)
                    {
                        logger.Info($"Upgrading to {updateInfo.FutureReleaseEntry.Version}");
                        shouldRestart = true;
                        logger.Info("Downloading release...");
                        await mgr.DownloadReleases(updateInfo.ReleasesToApply);
                        logger.Info("Applying release...");
                        await mgr.ApplyReleases(updateInfo);
                    }
                }

                if (shouldRestart)
                {
                    logger.Info("Restarting");
                    return true;
                }

                return false;
            }
            catch (Exception error)
            {
                logger.Error(error, "ApplyUpdates Exception");
                return false;
            }
        }

        private static bool CanUpdate() =>
            !string.IsNullOrWhiteSpace(UpdatePath);
    }

}
