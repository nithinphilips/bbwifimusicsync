﻿/**********************************************************************
 * WifiMusicSync
 * Copyright (C) 2011 Nithin Philips <nithin@nithinphilips.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 **********************************************************************/

namespace WifiSyncDesktop.Threading
{
    public class FileOperation
    {
        public FileOperationType OperationType { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public string Size { get; set; }

        public override string ToString()
        {
            return string.Format("{0}: {1}", OperationType, Source);
        }
    }
}
