#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.IO;
using System.Windows.Media.Imaging;
namespace DevExpress.Xpf.Map.Native {
	public class FileSystemTileCache {
		const char replaceSymbol = '_';
		readonly char[] unallowedSymbols = new char[] { ':' };
		readonly MultiScaleImageTileCache owner;
		public FileSystemTileCache(MultiScaleImageTileCache owner) {
			this.owner = owner;
		}
		bool IsTileExpired(string filePath, TimeSpan keepInterval) {
			if (File.Exists(filePath) && (DateTime.UtcNow - File.GetLastWriteTimeUtc(filePath) < keepInterval))
				return false;
			return true;
		}
		string ConvertFileName(string fileName, TilePositionData tilePosition) {
			return owner.MapKind.ToString() + "_" + tilePosition.Level.ToString() + "_" + tilePosition.PositionX.ToString() + "_" + tilePosition.PositionY.ToString() + GetFileType(fileName);
		}
		string GetProviderFolder(string fileName) {
			string fullDomainName = fileName.Split(new string[] { "/" }, 3, StringSplitOptions.RemoveEmptyEntries)[1];
			foreach (char symbol in unallowedSymbols)
				fullDomainName = fullDomainName.Replace(symbol, replaceSymbol);
			string[] domainNameParts = fullDomainName.Split(new char[] { '.' });
			string mainDomainName = domainNameParts[0];
			if (domainNameParts.Length > 1)
				mainDomainName = domainNameParts[domainNameParts.Length - 2] + "." + domainNameParts[domainNameParts.Length - 1];
			if (mainDomainName.Length > 0)
				mainDomainName = "\\" + mainDomainName;
			return mainDomainName;
		}
		string RemoveProviderDomain(string fileName) {
			string fullDomainName = fileName.Split(new string[] { "/" }, 3, StringSplitOptions.RemoveEmptyEntries)[1];
			return fileName.Replace(fullDomainName, "");
		}
		string GetFileType(string fileName) {
			if (fileName.Contains(".jpg") || fileName.Contains(".jpeg"))
				return ".jpg";
			if (fileName.Contains(".png"))
				return ".png";
			if (fileName.Contains(".gif"))
				return ".gif";
			if (fileName.Contains(".bmp"))
				return ".bmp";
			if (fileName.Contains(".tiff"))
				return ".tiff";
			return ".jpg";
		}
		BitmapEncoder GetEncoder(string fileName) {
			switch (GetFileType(fileName)) {
				case ".jpg": return new JpegBitmapEncoder();
				case ".png": return new PngBitmapEncoder();
				case ".gif": return new GifBitmapEncoder();
				case ".bmp": return new BmpBitmapEncoder();
				case ".tiff": return new TiffBitmapEncoder();
				default: return new JpegBitmapEncoder();
			}
		}
		public bool TryGetTile(string fileName, string cachePath, TilePositionData tilePosition, TimeSpan keepInterval, out Uri tileUri) {
			tileUri = null;
			cachePath += GetProviderFolder(fileName) + "\\";
			string actualFileName = ConvertFileName(fileName, tilePosition);
			if (!IsTileExpired(cachePath + actualFileName, keepInterval)) {
				string filePath = "file://" + cachePath.Replace('\\', '/') + actualFileName;
				tileUri = new Uri(filePath);
				return true;
			}
			return false;
		}
		public void SaveTile(string fileName, BitmapSource bitmapSource, TilePositionData tilePosition, string cachePath, TimeSpan keepInterval) {
			string filePath = cachePath + GetProviderFolder(fileName);
			if (!Directory.Exists(filePath))
				Directory.CreateDirectory(filePath);
			filePath += "\\" + ConvertFileName(fileName, tilePosition);
			if (IsTileExpired(filePath, keepInterval)) {
				string tempFileName = filePath + DateTime.Now.Ticks;
				using (var fileStream = new FileStream(tempFileName, FileMode.OpenOrCreate)) {
					BitmapEncoder encoder = GetEncoder(fileName);
					encoder.Frames.Add(BitmapFrame.Create(new CachedBitmap(bitmapSource, BitmapCreateOptions.IgnoreImageCache, BitmapCacheOption.None)));
					encoder.Save(fileStream);
				}
				if (File.Exists(filePath))
					File.Delete(filePath);
				File.Move(tempFileName, filePath);
			}
		}
	}
}
