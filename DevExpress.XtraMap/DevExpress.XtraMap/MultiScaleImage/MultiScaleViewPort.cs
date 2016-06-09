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
using System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.Utils.Drawing;
using DevExpress.XtraMap.Drawing;
using System.Collections.Generic;
namespace DevExpress.XtraMap.Native {
	public enum VisibleTilesStatus { CheckIsRequired, Loaded };
	public class MultiScaleViewport : MapDisposableObject {
		protected internal const MultiScaleTileSource DefaultTileSource = null;
		MapSize contentSize;
		MapPoint contentOrigin;
		MapPoint scale = new MapPoint(1, 1);
		MapPoint offset;
		Size viewSize;
		TileRange tileRange;
		IntSize arrangedTiles;
		int zoomLevelTileCount;
		int zoomLevel;
		MultiScaleTileSource tileSource = DefaultTileSource;
		MultiScaleTileBase[,] tiles;
		IMultiScaleImage tileFactory;
		int inflateCount;
		int lockUpdate = 0;
		VisibleTilesStatus visibleTilesStatus;
		bool isLoadCompleted;
		bool shouldRaiseDataLoadedEvent;
		bool shouldRaiseRequestDataLoadingEvent;
		object tilesUpdatelocker = new object();
		protected internal VisibleTilesStatus VisibleTilesStatus { get { return visibleTilesStatus; } }
		protected internal bool ShouldRaiseDataLoadedEvent { 
			get { return shouldRaiseDataLoadedEvent; } 
			set { shouldRaiseDataLoadedEvent = value; } 
		}
		protected internal bool ShouldRaiseRequestDataLoadingEvent { 
			get { return shouldRaiseRequestDataLoadingEvent; } 
			set { shouldRaiseRequestDataLoadingEvent = value; } 
		}
		public bool IsLoadCompleted { get { return isLoadCompleted; } }
		public object TilesUpdateLocker { get { return tilesUpdatelocker; } }
		public MapPoint ContentOrigin {
			get { return contentOrigin; }
			set {
				if(contentOrigin != value) {
					contentOrigin = value;
					RecalculateViewport();
				}
			}
		}
		public Size ViewSize {
			get { return viewSize; }
			set {
				if(viewSize != value) {
					viewSize = value;
					RecalculateViewport();
				}
			}
		}
		public MapSize ContentSize {
			get { return contentSize; }
			set {
				if(contentSize == value)
					return;
				contentSize = value;
				RecalculateViewport();
			}
		}
		public MapPoint Scale { get { return scale; } }
		public MapPoint Offset { get { return offset; } }
		public TileRange TileRange { get { return tileRange; } }
		public IntSize ArrangedTiles { get { return arrangedTiles; } }
		public IntSize InflatedTiles { get { return new IntSize(arrangedTiles.Width + (inflateCount << 1), arrangedTiles.Height + (inflateCount << 1)); } }
		public int ZoomLevelTileCount { get { return zoomLevelTileCount; } }
		public int MaxZoomLevel { get { return (TileSource != null) ? TileSource.MaxZoomLevel : 0; } }
		public virtual bool IsUpdateLocked { get { return this.lockUpdate != 0; } }
		public int ZoomLevel { get { return zoomLevel; } }
		public MultiScaleTileSource TileSource {
			get { return tileSource; }
			set {
				if(tileSource != value) {
					tileSource = value;
					RecalculateViewport();
					InvalidateAllTiles();
				}
			}
		}
		public MultiScaleTileBase[,] Tiles {
			get { return tiles; }
		}
		public IMultiScaleImage MultiScaleImage {
			get { return tileFactory; }
		}
		public int TileSourceHeight { get { return TileSource != null ? TileSource.TileHeight : 0; } }
		public int TileSourceWidth { get { return TileSource != null ? TileSource.TileWidth : 0; } }
		public MultiScaleViewport(IMultiScaleImage tileFactory, int inflateCount) {
			this.tileFactory = tileFactory;
			this.inflateCount = inflateCount;
		}
		public virtual void BeginUpdate() {
			this.lockUpdate++;
		}
		public virtual void EndUpdate(bool forceUpdate) {
			this.lockUpdate--;
			if(forceUpdate && !IsUpdateLocked) {
				RecalculateViewport();
				UpdateTiles();
			}
		}
		protected override void DisposeOverride() {
			if(tiles != null) {
				DisposeTiles();
				tiles = null;
			}
			if(tileSource != null) {
				tileSource.Dispose();
				tileSource = null;
			}
		}
		public void DisposeTiles() {
			IntSize inflatedSize = InflatedTiles;
			for(int y = 0; y < inflatedSize.Height; y++)
				for(int x = 0; x < inflatedSize.Width; x++)
					tiles[x, y].Dispose();
		}
		void ConfirmTiles(IntSize size) {
			if((tiles == null) || (arrangedTiles != size)) {
				IntSize inflatedSize = new IntSize(size.Width + inflateCount * 2, size.Height + inflateCount * 2);
				MultiScaleTileBase[,] temp = new MultiScaleTileBase[inflatedSize.Width, inflatedSize.Height];
				if(arrangedTiles.Area != 0) {
					for(int y = 0; y < InflatedTiles.Height; y++) {
						for(int x = 0; x < InflatedTiles.Width; x++) {
							if((x >= inflatedSize.Width) || (y >= inflatedSize.Height))
								tiles[x, y].Dispose();
							else
								temp[x, y] = tiles[x, y];
						}
					}
				}
				for(int y = 0; y < inflatedSize.Height; y++)
					for(int x = 0; x < inflatedSize.Width; x++)
						if(temp[x, y] == null)
							temp[x, y] = MultiScaleImage.CreateTile(this, x - inflateCount, y - inflateCount);
				tiles = temp;
				arrangedTiles = size;
			}
		}
		void ProjectRange(TileRange range) {
			if((tiles != null) && (range != tileRange)) {
				IntSize inflatedSize = InflatedTiles;
				for(int y = 0; y < inflatedSize.Height; y++)
					for(int x = 0; x < inflatedSize.Width; x++) {
						int projX = range.Min.X + x - inflateCount;
						int projY = range.Min.Y + y - inflateCount;
						if((projX >= zoomLevelTileCount) || (projY >= zoomLevelTileCount) || (projX < 0) || (projY < 0))
							tiles[x, y].Index = TileIndex.Invalid;
						else
							tiles[x, y].Index = new TileIndex(projX, projY, range.Min.Level);
					}
				tileRange = range;
				ShouldRaiseRequestDataLoadingEvent = true;
				visibleTilesStatus = VisibleTilesStatus.CheckIsRequired;
			}
		}
		internal void RecalculateViewport() {
			if(IsUpdateLocked)
				return;
			if(viewSize.Width == 0.0 || contentSize.Width == 0.0)
				return;
			if(TileSource == null || TileSource.TileWidth == 0.0 || TileSource.TileHeight == 0.0)
				return;
			lock(TilesUpdateLocker) {
				double tileWidth = TileSource.TileWidth;
				double tileHeight = TileSource.TileHeight;
				MapSize tileSize = new MapSize((viewSize.Width / tileWidth), (viewSize.Height / tileHeight));
				zoomLevel = Math.Min(Math.Max((int)(Math.Floor(Math.Log((Math.Ceiling(tileSize.Width) / contentSize.Width), 2.0))), 1), tileSource.MaxZoomLevel);
				zoomLevelTileCount = (int)Math.Pow(2.0, zoomLevel);
				scale = new MapPoint(tileSize.Width / contentSize.Width / (double)ZoomLevelTileCount, tileSize.Height / contentSize.Height / (double)ZoomLevelTileCount);
				double scaleX = Math.Min(scale.X, 1);
				double scaleY = Math.Min(scale.Y, 1);
				ConfirmTiles(new IntSize((int)Math.Ceiling(tileSize.Width / scaleX), (int)Math.Ceiling(tileSize.Height / scaleY)));
				TileIndex min = new TileIndex((int)Math.Max(Math.Floor(contentOrigin.X * zoomLevelTileCount), 0),
					(int)Math.Max(Math.Floor(contentOrigin.Y * zoomLevelTileCount), 0), zoomLevel);
				TileIndex max = new TileIndex((int)Math.Min(Math.Ceiling((contentOrigin.X + contentSize.Width) * zoomLevelTileCount / scaleX), zoomLevelTileCount),
					(int)Math.Min(Math.Ceiling((contentOrigin.Y + contentSize.Height) * zoomLevelTileCount / scaleY), zoomLevelTileCount * tileSource.TileCountRatio), zoomLevel);
				ProjectRange(new TileRange(min, max));
				offset = new MapPoint(tileRange.Min.X * tileWidth - (contentOrigin.X * tileWidth * zoomLevelTileCount),
					tileRange.Min.Y * tileHeight - (contentOrigin.Y * tileHeight * zoomLevelTileCount));
			}
		}
		public void RedrawVisualContent() {
			MultiScaleImage.RedrawVisualContent();
		}
		protected internal void ForceUpdate() {
			RecalculateViewport();
			UpdateTiles();
		}
		void PerformActionForEach(Action<MultiScaleTileBase> action) {
			if(tiles == null)
				return;
			for(int y = 0; y < InflatedTiles.Height; y++)
				for(int x = 0; x < InflatedTiles.Width; x++)
					action(tiles[x, y]);
		}
		void DoUpdateTile(MultiScaleTileBase tile) { tile.Update(); }
		void DoInvalidateUpdateTile(MultiScaleTileBase tile) {
			tile.Invalidate();
			tile.Update();
		}
		void InvalidateAllTiles() {
			PerformActionForEach(DoInvalidateUpdateTile);
		}
		public void UpdateTiles() {
			PerformActionForEach(DoUpdateTile);
		}
		public TileImageSource PeekTileImageSource(TileIndex index) {
			return TileSource != null ? TileSource.PeekTileImageSource(index) : null;
		}
		public TileImageSource GetTileImageSource(TileIndex index) {
			return TileSource != null ? TileSource.GetTileImageSource(index) : null;
		}
		internal void UpdateTilesLoadCompleted() {
			if(visibleTilesStatus == VisibleTilesStatus.CheckIsRequired && IsLoadCompleted) {
				visibleTilesStatus = VisibleTilesStatus.Loaded;
				ShouldRaiseDataLoadedEvent = true;
			}
		}
		internal IEnumerable<IRenderItem> CreateRenderItems() {
			List<IRenderItem> result = new List<IRenderItem>();
			if(Tiles == null)
				return result;
			bool facesAreLoaded = true;
			for(int y = 0; y < InflatedTiles.Height; y++)
				for(int x = 0; x < InflatedTiles.Width; x++) {
					MultiScaleTileBase tile = Tiles[x, y];
					if(tile.Index == TileIndex.Invalid)
						continue;
					tile.UpdateRenderParams(Scale);
					IRenderItemProvider provider = tile as IRenderItemProvider;
					if (!provider.IsReady) 
						facesAreLoaded = false;
					result.AddRange(provider.RenderItems);
				}
			this.isLoadCompleted = facesAreLoaded;
			return result;
		}
		internal MapPoint CalculateRenderOffset(Point point) {
			double x = (double)point.X / Scale.X;
			double y = (double)point.Y / Scale.Y;
			return new MapPoint(Offset.X + x, Offset.Y + y);
		}
	}
}
