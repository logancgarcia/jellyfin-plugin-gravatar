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
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.Gravatar.Configuration;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Gravatar
{
    public class GravatarManager
    {
        private const string GravatarUrl = "https://www.gravatar.com/avatar/{0}.png?d={1}&r={2}&s=200";

        private readonly IProviderManager _providerManager;
        private readonly ILogger _logger;

        public GravatarManager(
            IProviderManager providerManager,
            ILogger logger)
        {
            _providerManager = providerManager;
            _logger = logger;
        }

        private GravatarOptions GetOptions(User user)
            => Plugin.Instance.Configuration.Options
                .FirstOrDefault(i => string.Equals(
                    i.UserId,
                    user.Id.ToString("N"),
                    StringComparison.OrdinalIgnoreCase));

        private static string GetMd5Hash(string input)
        {
            var md5Hasher = new MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(
                Encoding.Default.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        public bool IsEnabledForUser(User user)
        {
            var options = GetOptions(user);

            return options != null && options.Enabled;
        }

        public async Task Post(
            User user,
            CancellationToken cancellationToken)
        {
            if (!IsEnabledForUser(user))
            {
                _logger.LogError(
                    "Gravatar support is not enabled for this user",
                    user.Name);
                return;
            }

            var imageType = (ImageType)Enum.Parse(
                typeof(ImageType),
                "Primary",
                true);
            var options = GetOptions(user);
            var hash = options.Email;

            if (string.IsNullOrEmpty(hash))
            {
                hash = user.Name;
            }

            hash = hash.Trim();
            hash = hash.ToLower();
            hash = GetMd5Hash(hash);

            var url = string.Format(
                GravatarUrl,
                hash,
                options.DefaultAvatar,
                options.Rating);

            await _providerManager.SaveImage(
                user,
                url,
                imageType,
                null,
                cancellationToken).ConfigureAwait(false);

            user.UpdateToRepository(
                ItemUpdateType.ImageUpdate,
                cancellationToken);
        }
    }
}
