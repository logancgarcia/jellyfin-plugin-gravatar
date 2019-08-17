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
using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.Gravatar.Configuration
{
    /// <summary>
    /// Class PluginConfiguration
    /// </summary>
    public class PluginConfiguration : BasePluginConfiguration
    {
        public GravatarOptions[] Options { get; set; }

        public PluginConfiguration ()
        {
            Options = Array.Empty<GravatarOptions>();
        }
    }

    /// <summary>
    /// Class GravatarOptions
    /// </summary>
    public class GravatarOptions
    {
        public bool Enabled { get; set; }
        public string DefaultAvatar { get; set; }
        public string Email { get; set; }
        public string Rating { get; set; }
        public string UserId { get; set; }

        public GravatarOptions()
        {
            Enabled = false;
        }
    }
}
