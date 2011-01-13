/**********************************************************************
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

using System.IO;

namespace WifiSyncDesktop.Model
{
    public class DirectorySizeCalculator
    {
        public static long CalculateSize(string path)
        {
            if (!Directory.Exists(path)) return 0;

            long totalSize = 0;
            //TODO: Handle Access denied exceptions
            string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                totalSize += fi.Length;
            }
            System.Diagnostics.Debug.WriteLine(totalSize);
            return totalSize;
        }
    }
}
