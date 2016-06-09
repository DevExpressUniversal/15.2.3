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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
namespace DevExpress.Xpf.Map.Native {
	public enum TileStatus {
		Actual,
		Substituted,
		InProgress
	}
	public struct TilePositionData {
		int level;
		int positionX;
		int positionY;
		public int Level { get { return level; } }
		public int PositionX { get { return positionX; } }
		public int PositionY { get { return positionY; } }
		public TilePositionData(int level, int positionX, int positionY) {
			this.level = level;
			this.positionX = positionX;
			this.positionY = positionY;
		}
		public override string ToString() {
			return string.Format("{0}:{1} {2}", Level, PositionX, PositionY);
		}
	}
	public class TileInfo {
		Image tile;
		Image substitutedTile;
		TileStatus status;
		bool valid;
		double left;
		double top;
		double scaleX;
		double scaleY;
		public Image Tile { get { return tile; } set { tile = value; } }
		public Image SubstitutedTile { get { return substitutedTile; } }
		public TileStatus Status { get { return status; } set { status = value; } }
		public bool Valid { get { return valid; } set { valid = value; } }
		public double Left { get { return left; } set { left = value; } }
		public double Top { get { return top; } set { top = value; } }
		public double ScaleX { get { return scaleX; } set { scaleX = value; } }
		public double ScaleY { get { return scaleY; } set { scaleY = value; } }
		public TileInfo(Image tile, Image substitutedTile, TileStatus status, double left, double top, double scaleX, double scaleY) {
			this.tile = tile;
			this.substitutedTile = substitutedTile;
			this.status = status;
			this.valid = true;
			this.left = left;
			this.top = top;
			this.scaleX = scaleX;
			this.scaleY = scaleY;
		}
	}
	public class MultiScaleTilesManager {
		readonly MultiScaleImage owner;
		readonly Dictionary<TilePositionData, TileInfo> tilesInfo;
		readonly MultiScaleImageTileCache tileCache;
		bool zoomLevelChanged;
		Canvas Canvas { get { return owner.Canvas; } }
		internal int ZoomLevel { get { return owner.ZoomLevel; } }
		internal int MaxZoomLevel { get { return owner.MaxZoomLevel; } }
		internal int TileWidth { get { return owner.Source.TileWidth; } }
		internal int TileHeight { get { return owner.Source.TileHeight; } }
		internal int StartTileX { get { return owner.StartTileX; } }
		internal int StartTileY { get { return owner.StartTileY; } }
		internal int EndTileX { get { return owner.EndTileX; } }
		internal int EndTileY { get { return owner.EndTileY; } }
		internal int MaxTile { get { return owner.MaxTile; } }
		internal CacheOptions CacheOptions { get { return owner.ActualCacheOptions; } }
		internal object MapKind { get { return owner.MapKind; } }
		public MultiScaleTilesManager(MultiScaleImage owner) {
			this.owner = owner;
			tilesInfo = new Dictionary<TilePositionData, TileInfo>();
			tileCache = new MultiScaleImageTileCache(this);
		}
		void UpdateCanvas(bool animate) {
			List<Image> updatedImages = new List<Image>();
			foreach (KeyValuePair<TilePositionData, TileInfo> pair in tilesInfo) {
				TileInfo tileInfo = pair.Value;
				Image tile = tileInfo.Tile;
				Image substitutedTile = tileInfo.SubstitutedTile;
				if (animate)
					tileInfo.Status = TileStatus.InProgress;
				if ((tile == null || tileInfo.Status != TileStatus.Actual) && substitutedTile != null) {
					UpdateTile(substitutedTile, tileInfo.Left, tileInfo.Top, tileInfo.ScaleX, tileInfo.ScaleY, false, TileStatus.Substituted);
					updatedImages.Add(substitutedTile);
				}
				if (tile != null) {
					UpdateTile(tile, tileInfo.Left, tileInfo.Top, tileInfo.ScaleX, tileInfo.ScaleY, animate, TileStatus.Actual);
					updatedImages.Add(tile);
					if (tileInfo.Status == TileStatus.InProgress && tile.Opacity == 1)
						SetTileActiveStatus(tileInfo);
				}
			}
			List<Image> invalidImages = new List<Image>();
			foreach (Image tile in Canvas.Children)
				if (!updatedImages.Contains(tile))
					invalidImages.Add(tile);
			foreach (Image removedImage in invalidImages)
				Canvas.Children.Remove(removedImage);
		}
		void UpdateTile(Image tile, double left, double top, double scaleX, double scaleY, bool animate, TileStatus status) {
			if (tile.RenderTransform as ScaleTransform == null)
				tile.RenderTransform = new ScaleTransform();
			ScaleTransform imageTransform = tile.RenderTransform as ScaleTransform;
			imageTransform.ScaleX = scaleX;
			imageTransform.ScaleY = scaleY;
			if (tile.Source != null) {
				imageTransform.ScaleX *= TileWidth / tile.Source.Width;
				imageTransform.ScaleY *= TileHeight / tile.Source.Height;
			}
			Canvas.SetLeft(tile, left);
			Canvas.SetTop(tile, top);
			if (animate) {
				tile.BeginAnimation(UIElement.OpacityProperty, null);
				tile.Opacity = 0.0;
				DoubleAnimation animation = new DoubleAnimation(1.0, owner.Source.BlendTime)
				{
					AccelerationRatio = 0.5,
					DecelerationRatio = 0.3
				};
				animation.Completed += new EventHandler(TileAnimationCompleted);
				tile.BeginAnimation(UIElement.OpacityProperty, animation);
			}
			if (!Canvas.Children.Contains(tile))
				if (status != TileStatus.Substituted)
					Canvas.Children.Add(tile);
				else
					Canvas.Children.Insert(0, tile);
		}
		void TileAnimationCompleted(object sender, EventArgs e) {
			UpdateCanvas(false);
		}
		void SetTileActiveStatus(TileInfo tileInfo) {
			tileInfo.Status = TileStatus.Actual;
			if (CheckAllTilesLoaded()) {
				if (!owner.IsDataReady)
					SetDataReady();
			}
		}
		void SetDataReady() {
			owner.SetDataReady(true);
		}
		bool CheckAllTilesLoaded() {
			if (!tileCache.IsTileQueueEmpty)
				return false;
			foreach (TileInfo tileInfo in tilesInfo.Values) {
				if (tileInfo != null)
					if (tileInfo.Status != TileStatus.Actual)
						return false;
			}
			return true;
		}
		void RemoveInvalidTiles() {
			List<TilePositionData> invalidTilesData = new List<TilePositionData>();
			foreach (KeyValuePair<TilePositionData, TileInfo> pair in tilesInfo)
				if (!pair.Value.Valid)
					invalidTilesData.Add(pair.Key);
			foreach (TilePositionData removedTile in invalidTilesData)
				tilesInfo.Remove(removedTile);
		}
		internal void OnWebRequest(MapWebRequestEventArgs e) {
			owner.OnTileWebRequest(e);
		}
		public void BeginUpdate(bool zoomLevelChanged) {
			foreach (KeyValuePair<TilePositionData, TileInfo> pair in tilesInfo)
				pair.Value.Valid = false;
		}
		public void Update(TilePositionData tileData, double left, double top, double scaleX, double scaleY, bool zoomLevelChanged, int previousLevel) {
			TileInfo tileInfo;
			tilesInfo.TryGetValue(tileData, out tileInfo);
			this.zoomLevelChanged = zoomLevelChanged;
			if (tileInfo != null) {
				tileInfo.Left = left;
				tileInfo.Top = top;
				tileInfo.ScaleX = scaleX;
				tileInfo.ScaleY = scaleY;
				tileInfo.Valid = true;
			} else {
				TileStatus tileStatus;
				Image tile = tileCache.GetTile(tileData, out tileStatus);
				Image substitutedTile = null;
				if (zoomLevelChanged)
					substitutedTile = tileCache.GetSubstitutedTile(previousLevel, tileData);
				if (tileStatus == TileStatus.Substituted) {
					if (substitutedTile == null)
						substitutedTile = tile;
					tile = null;
				}
				AddTileInfo(tileData, new TileInfo(tile, substitutedTile, tileStatus, left, top, scaleX, scaleY));  
			}
		}
		void AddTileInfo(TilePositionData tileData, TileInfo tileInfo) {
			tilesInfo.Add(tileData, tileInfo);
			owner.SetDataReady(false);
		}
		public void EndUpdate(TilePositionData centerPosition) {
			RemoveInvalidTiles();
			UpdateCanvas(zoomLevelChanged);
			tileCache.RefreshDownload(centerPosition);
		}
		public Uri GetTileUri(TilePositionData tileData) {
			return owner.Source.GetTileImageSource(tileData.Level + owner.ZoomLevelOffset, tileData.PositionX, tileData.PositionY);
		}
		public void AddDownloadedTile(TilePositionData tileData, Image tile) {
			TileInfo tileInfo;
			tilesInfo.TryGetValue(tileData, out tileInfo);
			if (tileInfo != null) {
				tileInfo.Tile = tile;
				tileInfo.Status = TileStatus.InProgress;
				UpdateTile(tile, tileInfo.Left, tileInfo.Top, tileInfo.ScaleX, tileInfo.ScaleY, true, tileInfo.Status);
			}
		}
		public void Reset() {
			tilesInfo.Clear();
			tileCache.Clear();
			owner.SetDataReady(false);
		}
	}
}
