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
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace DevExpress.Xpf.Map.Native {
	public struct TileCacheData {
		TileStatus tileStatus;
		TilePositionData tileData;
		public TileStatus TileStatus { get { return tileStatus; } }
		public TilePositionData TileData { get { return tileData; } }
		public TileCacheData(TilePositionData tileData)
			: this(tileData, TileStatus.Actual) {
		}
		public TileCacheData(TilePositionData tileData, TileStatus tileStatus) {
			this.tileData = tileData;
			this.tileStatus = tileStatus;
		}
	}
	public class MultiScaleImageTileCache {
		const int maxCacheCapacity = 50;
		const int maxDownloadCacheCapacity = 100;
		const int currentLevelBuffer = 2;
		const int lowLevelBuffer = 2;
		const int highLevelBuffer = 2;
		const double tileBorder = 1;
		readonly MultiScaleTilesManager owner;
		readonly List<Dictionary<TileCacheData, ImageSource>> cache = new List<Dictionary<TileCacheData, ImageSource>>();
		readonly DownloadQueue downloadQueue;
		readonly FileSystemTileCache fileCache;
		int ZoomLevel { get { return owner.ZoomLevel; } }
		int MaxZoomLevel { get { return owner.MaxZoomLevel; } }
		int TileWidth { get { return owner.TileWidth; } }
		int TileHeight { get { return owner.TileHeight; } }
		CacheOptions CacheOptions { get { return owner.CacheOptions; } }
		bool EnableCache { get { return !string.IsNullOrEmpty(CacheOptions.Directory); } }
		internal object MapKind { get { return owner.MapKind; } }
		internal bool IsTileQueueEmpty { get { return downloadQueue.IsTileQueueEmpty; } }
		public MultiScaleImageTileCache(MultiScaleTilesManager owner) {
			this.owner = owner;
			this.fileCache = new FileSystemTileCache(this);
			this.downloadQueue = new DownloadQueue();
			this.downloadQueue.TileLoaded += TileLoaded;
			this.downloadQueue.WebRequest += OnWebRequest;
		}
		void OnWebRequest(object sender, MapWebRequestEventArgs e) {
			owner.OnWebRequest(e);
		}
		void TileLoaded(object sender, TileLoadedEventArgs e) {
			Image image = new Image();
			ImageSource imageSource = CreateImageSource(e.StreamSource);
			image.BeginInit();
			image.Source = imageSource;
			image.EndInit();
			AddTile(e.TilePosition, TileStatus.Actual, imageSource);
			CacheTile(e.Uri, image.Source as BitmapSource, e.TilePosition);
			owner.AddDownloadedTile(e.TilePosition, image);
		}
		void CacheTile(string fileName, BitmapSource bitmapSource, TilePositionData tilePosition) {
			if (bitmapSource != null && EnableCache && !fileName.ToLowerInvariant().Contains(CacheOptions.Directory.ToLowerInvariant().Replace('\\', '/')))
				fileCache.SaveTile(fileName, bitmapSource, tilePosition, CacheOptions.Directory, CacheOptions.KeepInterval);
		}
		void CreateCache() {
			cache.Capacity = owner.MaxZoomLevel;
			for (int i = 0; i < owner.MaxZoomLevel; i++)
				cache.Add(new Dictionary<TileCacheData, ImageSource>());
		}
		void ClearCache() {
			for (int i = 2; i < cache.Capacity + 1; i++) {
				if ((i < ZoomLevel - 1 || i > ZoomLevel + 1) && cache[i - 1].Count > maxCacheCapacity)
					cache[i - 1].Clear();
				else {
					if (i == ZoomLevel - 1 && cache[i - 1].Count > maxCacheCapacity) {
						int highLevelStartTileX = Math.Max((int)Math.Floor(owner.StartTileX / 2.0) - currentLevelBuffer, 0);
						int highLevelStartTileY = Math.Max((int)Math.Floor(owner.StartTileY / 2.0) - currentLevelBuffer, 0);
						int highLevelEndTileX = Math.Min((int)Math.Floor(owner.StartTileX / 2.0) + currentLevelBuffer, owner.MaxTile / 2);
						int highLevelEndTileY = Math.Min((int)Math.Floor(owner.StartTileY / 2.0) + currentLevelBuffer, owner.MaxTile / 2);
						List<TileCacheData> removeList = new List<TileCacheData>();
						foreach (KeyValuePair<TileCacheData, ImageSource> pair in cache[i - 1]) {
							TilePositionData tileData = pair.Key.TileData;
							if ((tileData.PositionX < highLevelStartTileX || tileData.PositionX >= highLevelEndTileX) ||
								(tileData.PositionY < highLevelStartTileY || tileData.PositionY >= highLevelEndTileY))
								removeList.Add(pair.Key);
						}
						foreach (TileCacheData removeData in removeList)
							cache[i - 1].Remove(removeData);
					}
					if (i == ZoomLevel && cache[i - 1].Count > maxCacheCapacity) {
						int currentLevelStartTileX = Math.Max(owner.StartTileX - currentLevelBuffer, 0);
						int currentLevelStartTileY = Math.Max(owner.StartTileY - currentLevelBuffer, 0);
						int currentLevelEndTileX = Math.Min(owner.EndTileX + currentLevelBuffer, owner.MaxTile);
						int currentLevelEndTileY = Math.Min(owner.EndTileY + currentLevelBuffer, owner.MaxTile);
						List<TileCacheData> removeList = new List<TileCacheData>();
						foreach (KeyValuePair<TileCacheData, ImageSource> pair in cache[i - 1]) {
							TilePositionData tileData = pair.Key.TileData;
							if ((tileData.PositionX < currentLevelStartTileX || tileData.PositionX >= currentLevelEndTileX) ||
								(tileData.PositionY < currentLevelStartTileY || tileData.PositionY >= currentLevelEndTileY))
								removeList.Add(pair.Key);
						}
						foreach (TileCacheData removeData in removeList)
							cache[i - 1].Remove(removeData);
					}
					if (i == ZoomLevel + 1 && cache[i - 1].Count > maxCacheCapacity) {
						int lowLevelStartTileX = Math.Max(owner.StartTileX + owner.EndTileX - currentLevelBuffer, 0);
						int lowLevelStartTileY = Math.Max(owner.StartTileY + owner.EndTileY - currentLevelBuffer, 0);
						int lowLevelEndTileX = Math.Min(owner.StartTileX + owner.EndTileX + currentLevelBuffer, owner.MaxTile * 2);
						int lowLevelEndTileY = Math.Min(owner.StartTileY + owner.EndTileY + currentLevelBuffer, owner.MaxTile * 2);
						List<TileCacheData> removeList = new List<TileCacheData>();
						foreach (KeyValuePair<TileCacheData, ImageSource> pair in cache[i - 1]) {
							TilePositionData tileData = pair.Key.TileData;
							if ((tileData.PositionX < lowLevelStartTileX || tileData.PositionX >= lowLevelEndTileX) ||
								(tileData.PositionY < lowLevelStartTileY || tileData.PositionY >= lowLevelEndTileY))
								removeList.Add(pair.Key);
						}
						foreach (TileCacheData removeData in removeList)
							cache[i - 1].Remove(removeData);
					}
				}
			}
		}
		ImageSource GetHighLevelTile(int level, int positionX, int positionY) {
			ImageSource resultBitmap = null;
			for (int sourceLevel = level - 1; sourceLevel > 0; sourceLevel--) {
				int levelFactor = (int)Math.Pow(2.0, level - sourceLevel);
				int sourcePositionX = (int)Math.Floor(positionX / (double)levelFactor);
				int sourcePositionY = (int)Math.Floor(positionY / (double)levelFactor);
				int startTargetPositionX = sourcePositionX * levelFactor;
				int startTargetPositionY = sourcePositionY * levelFactor;
				ImageSource tileBitmapSource = null;
				if (cache.Capacity >= sourceLevel && cache[sourceLevel - 1] != null)
					cache[sourceLevel - 1].TryGetValue(new TileCacheData(new TilePositionData(sourceLevel, sourcePositionX, sourcePositionY)), out tileBitmapSource);
				if (tileBitmapSource != null) {
					int offsetX = (int)((positionX - startTargetPositionX) * ((double)TileWidth / (double)levelFactor));
					int offsetY = (int)((positionY - startTargetPositionY) * ((double)TileHeight / (double)levelFactor));
					if (levelFactor > TileWidth || levelFactor > TileHeight)
						levelFactor = Math.Min(TileWidth, TileHeight);
					if (tileBitmapSource != null) {
						resultBitmap = GetPartialImageSource(tileBitmapSource, levelFactor, offsetX, offsetY);
						resultBitmap.Freeze();
					}
					break;
				}
			}
			return resultBitmap;
		}
		ImageSource GetPartialImageSource(ImageSource source, int scale, int offsetX, int offsetY) {
			DrawingGroup group = new DrawingGroup();
			group.Children.Add(new ImageDrawing(source, new Rect(0, 0, TileWidth * scale + tileBorder, TileHeight * scale + tileBorder)));
			group.ClipGeometry = new RectangleGeometry(new Rect(offsetX * scale, offsetY * scale, TileWidth + tileBorder, TileHeight + tileBorder));
			group.Freeze();
			return new DrawingImage(group);
		}
		ImageSource GetLowLevelTile(int level, int positionX, int positionY) {
			ImageSource imageSource00 = null;
			ImageSource imageSource01 = null;
			ImageSource imageSource10 = null;
			ImageSource imageSource11 = null;
			if (cache.Capacity > level - 1) {
				cache[level - 1].TryGetValue(new TileCacheData(new TilePositionData(level, positionX, positionY)), out imageSource00);
				cache[level - 1].TryGetValue(new TileCacheData(new TilePositionData(level, positionX + 1, positionY)), out imageSource01);
				cache[level - 1].TryGetValue(new TileCacheData(new TilePositionData(level, positionX, positionY + 1)), out imageSource10);
				cache[level - 1].TryGetValue(new TileCacheData(new TilePositionData(level, positionX + 1, positionY + 1)), out imageSource11);
			}
			if (imageSource00 == null)
				imageSource00 = GetHighLevelTile(level, positionX, positionY);
			if (imageSource01 == null)
				imageSource01 = GetHighLevelTile(level, positionX + 1, positionY);
			if (imageSource10 == null)
				imageSource10 = GetHighLevelTile(level, positionX, positionY + 1);
			if (imageSource11 == null)
				imageSource11 = GetHighLevelTile(level, positionX + 1, positionY + 1);
			if (imageSource00 != null && imageSource01 != null && imageSource10 != null && imageSource11 != null)
				return GetMergedImageSource(imageSource00, imageSource01, imageSource10, imageSource11);
			else
				return null;
		}
		ImageSource GetMergedImageSource(ImageSource imageSource00, ImageSource imageSource01, ImageSource imageSource10, ImageSource imageSource11) {
			DrawingGroup group = new DrawingGroup();
			group.Children.Add(new ImageDrawing(imageSource00, new Rect(0, 0, imageSource00.Width / 2, imageSource00.Height / 2)));
			group.Children.Add(new ImageDrawing(imageSource01, new Rect(TileWidth / 2, 0, imageSource01.Width / 2 + tileBorder, imageSource01.Height / 2)));
			group.Children.Add(new ImageDrawing(imageSource10, new Rect(0, TileHeight / 2, imageSource10.Width / 2, imageSource10.Height / 2 + tileBorder)));
			group.Children.Add(new ImageDrawing(imageSource11, new Rect(TileWidth / 2, TileHeight / 2, imageSource11.Width / 2 + tileBorder, imageSource11.Height / 2 + tileBorder)));
			group.Freeze();
			return new DrawingImage(group);
		}
		ImageSource CreateImageSource(byte[] sourceStream) {
			try {
				BitmapDecoder bdDecoder = BitmapDecoder.Create(new MemoryStream(sourceStream), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
				return bdDecoder.Frames[0];
			}
			catch (Exception) {
				return null;
			}
		}
		public void AddTile(TilePositionData tileData, TileStatus tileStatus, ImageSource imageSource) {
			if (cache.Capacity < tileData.Level)
				cache.Capacity = tileData.Level;
			if (cache[tileData.Level - 1] == null)
				cache[tileData.Level - 1] = new Dictionary<TileCacheData, ImageSource>();
			TileCacheData tileCacheData = new TileCacheData(tileData, tileStatus);
			if (!cache[tileData.Level - 1].ContainsKey(tileCacheData)) {
				cache[tileData.Level - 1].Add(tileCacheData, imageSource);
			}
			else {
				cache[tileData.Level - 1].Remove(tileCacheData);
				cache[tileData.Level - 1].Add(tileCacheData, imageSource);
			}
			ClearCache();
		}
		public Image GetTile(TilePositionData tileData, out TileStatus tileStatus) {
			Image image = new Image();
			ImageSource imageSource;
			tileStatus = TileStatus.Substituted;
			if (cache.Capacity >= tileData.Level && cache[tileData.Level - 1] != null) {
				cache[tileData.Level - 1].TryGetValue(new TileCacheData(tileData, TileStatus.Actual), out imageSource);
				if (imageSource == null) {
					Uri sourceUri = owner.GetTileUri(tileData);
					if (sourceUri != null) {
						Uri tileUri;
						if (!(EnableCache && fileCache.TryGetTile(sourceUri.ToString(), CacheOptions.Directory, tileData, CacheOptions.KeepInterval, out tileUri)))
							tileUri = sourceUri;
						ProcessDownloadTask(tileData, tileUri);
						image.Source = GetHighLevelTile(tileData.Level, tileData.PositionX, tileData.PositionY);
					}
				}
				else {
					tileStatus = TileStatus.Actual;
					System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(new Action(delegate {
						image.BeginInit();
						image.Source = imageSource;
						image.EndInit();
					}));
				}
			}
			return image;
		}
		public Image GetSubstitutedTile(int sourceLevel, TilePositionData tileData) {
			Image resultImage = new Image();
			if (tileData.Level > sourceLevel)
				resultImage.Source = GetHighLevelTile(tileData.Level, tileData.PositionX, tileData.PositionY);
			else
				resultImage.Source = GetLowLevelTile(sourceLevel, tileData.PositionX * 2, tileData.PositionY * 2);
			return resultImage;
		}
		void ProcessDownloadTask(TilePositionData tileData, Uri bitmapUri) {
			downloadQueue.Push(bitmapUri, tileData);
		}
		public void Clear() {
			downloadQueue.Clear();
			cache.Clear();
			CreateCache();
		}
		public void RefreshDownload(TilePositionData centerPosition) {
			downloadQueue.Refresh(centerPosition);
		}
	}
}
