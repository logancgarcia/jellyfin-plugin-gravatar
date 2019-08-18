// Jellyfin Gravatar Plugin
// Copyright (C) 2019  Logan Garcia
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.Gravatar;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Tasks;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Gravatar.ScheduledTasks
{
    public class GravatarScheduledTask : IScheduledTask, IConfigurableScheduledTask
    {
        private readonly ILogger _logger;
        private readonly IServerConfigurationManager _config;
        private readonly IProviderManager _providerManager;
        private readonly IUserManager _userManager;

        public GravatarScheduledTask(
            ILogger logger,
            IServerConfigurationManager config,
            IProviderManager providerManager,
            IUserManager userManager)
        {
            _logger = logger;
            _config = config;
            _providerManager = providerManager;
            _userManager = userManager;
        }

        public async Task Execute(CancellationToken cancellationToken, IProgress<double> progress)
        {
            var gravatarManager = new GravatarManager(_providerManager, _logger);
            var users = _userManager.Users.ToList();
            var percentPerUser = 100 / users.Count;
            var numComplete = 0;
            double currentProgress = 0;

            foreach (var user in users)
            {
                await gravatarManager.Post(user, CancellationToken.None).ConfigureAwait(false);

                numComplete++;
                currentProgress = percentPerUser * numComplete;
                progress.Report(currentProgress);
            }
        }

        /// <summary>
        /// Creates the triggers that define when the task will run
        /// </summary>
        /// <returns>IEnumerable{BaseTaskTrigger}.</returns>
        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            return new[] {

                // Every so often
                new TaskTriggerInfo
                {
                    Type = TaskTriggerInfo.TriggerInterval,
                    IntervalTicks = TimeSpan.FromDays(1).Ticks
                }
            };
        }

        public string Name => "Update profile images";

        public string Description => "Updates profile images of users who have Gravatar enabled.";

        public string Category => "Gravatar";

        public string Key => "Gravatar";

        public bool IsHidden { get { return false; } }

        public bool IsEnabled { get { return true; } }

        public bool IsLogged { get { return false; } }
    }
}
