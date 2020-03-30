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
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Net;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Services;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Gravatar.Api
{
    /// <summary>
    /// Class PostGravatarImage
    /// </summary>
    [Route("/Gravatar/{UserId}", "POST")]
    [Authenticated]
    public class PostGravatar : IReturnVoid
    {
        /// <summary>
        /// Sets the user id.
        /// </summary>
        /// <value>The user id.</value>
        [ApiMember(Name = "UserId", Description = "User ID", IsRequired = true, DataType = "string", ParameterType = "path", Verb = "POST")]
        public Guid UserId { get; set; }
    }

    public class ServerApiEndpoints : IService
    {
        private readonly IProviderManager _providerManager;
        private readonly IUserManager _userManager;
        private readonly ILogger _logger;

        public ServerApiEndpoints(
            IProviderManager providerManager,
            IUserManager userManager,
            ILogger logger)
        {
            _providerManager = providerManager;
            _userManager = userManager;
            _logger = logger;
        }

        public Task Post(PostGravatar request)
        {
            return new GravatarManager(_providerManager, _logger).Post(
                _userManager.GetUserById(request.UserId),
                CancellationToken.None);
        }
    }
}
