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

using DevExpress.Map;
using DevExpress.XtraMap.Drawing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
namespace DevExpress.XtraMap.Native {
	public partial class MultiScaleTile : MultiScaleTileBase, IRenderItemProvider {
		const double constOverlayFadeTime = 1.0;
		BitmapTile face;
		BitmapTile[,] mask;
		TileIndex currentTileIndex;
		MapPoint renderStretchFactor = new MapPoint(1.0, 1.0);
		public MapPoint RenderStretchFactor { get { return renderStretchFactor; } }
		public MultiScaleTile(MultiScaleViewport viewport, int x, int y)
			: base(viewport, x, y) {
			InitPieces();
			UpdateLocation();
		}
		void InitPieces() {
			face = new BitmapTile(this);
			MapSize size = new MapSize(Viewport.TileSourceWidth, Viewport.TileSourceHeight);
			mask = new BitmapTile[2, 2];
			mask[0, 0] = CreateBitmapTile(size);
			mask[1, 0] = CreateBitmapTile(size);
			mask[0, 1] = CreateBitmapTile(size);
			mask[1, 1] = CreateBitmapTile(size);
		}
		BitmapTile CreateBitmapTile(MapSize size) {
			return new BitmapTile(this) { Size = size };
		}
		void UpdateBitmaps() {
			if(currentTileIndex == TileIndex.Invalid) {
				face.Source = null;
				mask[0, 0].Source = null;
				mask[1, 0].Source = null;
				mask[0, 1].Source = null;
				mask[1, 1].Source = null;
				return;
			}
			face.Source = Viewport.GetTileImageSource(currentTileIndex);
			if(face.Source == null || face.Source.Source == null) {
				UpdatePiece(0, 0);
				UpdatePiece(1, 0);
				UpdatePiece(0, 1);
				UpdatePiece(1, 1);
			}
		}
		void UpdateTransform() {
			Size initialSize = new Size(Viewport.TileSourceWidth, Viewport.TileSourceHeight);
			MapRect transformedRect = new MapRect(Location.X, Location.Y, initialSize.Width, initialSize.Height);
			MapPoint alignedPosition = new MapPoint(transformedRect.Left, transformedRect.Top);
			MapSize alignedSize = new MapSize(transformedRect.Width, transformedRect.Height);
			MapSize halfAlignedSize = new MapSize(alignedSize.Width / 2.0, Math.Ceiling(alignedSize.Height / 2.0));
			if(face != null) {
				face.Position = alignedPosition;
				face.Size = alignedSize;
			}
			mask[0, 0].Position = face.Position;
			mask[1, 0].Position = new MapPoint(face.Position.X + halfAlignedSize.Width, face.Position.Y);
			mask[0, 1].Position = new MapPoint(face.Position.X, face.Position.Y + halfAlignedSize.Height);
			mask[1, 1].Position = new MapPoint(face.Position.X + halfAlignedSize.Width, face.Position.Y + halfAlignedSize.Height);
			mask[0, 0].Size = halfAlignedSize;
			mask[1, 0].Size = halfAlignedSize;
			mask[0, 1].Size = halfAlignedSize;
			mask[1, 1].Size = halfAlignedSize;
		}
		TileIndex DownwardIndex(int x, int y) {
			return new TileIndex(currentTileIndex.X * 2 + x, currentTileIndex.Y * 2 + y, currentTileIndex.Level + 1);
		}
		TileIndex UpwardIndex(int x, int y, int level, out MapPoint offset, out double scale) {
			int distance = (int)Math.Pow(2, currentTileIndex.Level + 1 - level);
			int pieceX = (currentTileIndex.X * 2) + x;
			int pieceY = (currentTileIndex.Y * 2) + y;
			int upX = (int)Math.Floor((double)pieceX / distance);
			int upY = (int)Math.Floor((double)pieceY / distance);
			offset = new MapPoint
			(
				((double)pieceX / distance) - upX,
				((double)pieceY / distance) - upY
			);
			scale = distance;
			return new TileIndex(upX, upY, level);
		}
		MapRect GetOverlayPieceClip(MapPoint offset, double scale) {
			return new MapRect
			(
				Math.Ceiling(offset.X * Viewport.TileSourceWidth),
				Math.Ceiling(offset.Y * Viewport.TileSourceHeight),
				Math.Ceiling(Viewport.TileSourceWidth / scale),
				Math.Ceiling(Viewport.TileSourceHeight / scale)
			);
		}
		void UpdatePiece(int x, int y) {
			BitmapTile tile = mask[x, y];
			TileIndex downIndex = DownwardIndex(x, y);
			if(LookupCache(tile, downIndex)) {
				MapRect pieceRect = new MapRect(0, 0, Viewport.TileSourceWidth, Viewport.TileSourceHeight);
				tile.SetTileClipRect(pieceRect);
			}
			else {
				if(currentTileIndex.Level > 0) {
					for(int level = currentTileIndex.Level - 1; level >= 0; level--) {
						MapPoint offset;
						double scale;
						TileIndex upward = UpwardIndex(x, y, level, out offset, out scale);
						if(LookupCache(tile, upward)) {
							MapRect rect = GetOverlayPieceClip(offset, scale);
							tile.SetTileClipRect(rect);
							break;
						}
					}
				}
				else
					tile.Source = null;
			}
		}
		bool LookupCache(BitmapTile tile, TileIndex index) {
			TileImageSource source = Viewport.PeekTileImageSource(index);
			if(source != null) {
				tile.Source = source;
				return true;
			}
			return false;
		}
		bool IRenderItemProvider.IsReady { 
			get {
				return !IsInvalid && IsTileLoaded(face);
			} 
		}
		void IRenderItemProvider.OnRenderComplete() { 
		}
		MapColorizer IRenderItemProvider.GetColorizer(IRenderItem item) { return null; }
		double IRenderItemProvider.RenderScaleFactorX {
			get { return 1.0; }
		}
		double IRenderItemProvider.RenderScaleFactorY {
			get { return 1.0; }
		}
		MapPoint IRenderItemProvider.RenderOffset {
			get { return MapPoint.Empty; }
		}
		double IRenderItemProvider.RenderScale {
			get { return 1.0; }
		}
		IEnumerable<IRenderItem> IRenderItemProvider.RenderItems {
			get { return CreateRenderItems(); }
		}
		void IRenderItemProvider.PrepareData() {
		}
		IEnumerable<IRenderItem> CreateRenderItems() {
			List<IRenderItem> result = new List<IRenderItem>();
			if(IsTileLoaded(face)) {
				result.Add(face);
			}
			else {
				if(IsTileLoaded(mask[0, 0]))
					result.Add(mask[0, 0]);
				if(IsTileLoaded(mask[0, 1]))
					result.Add(mask[0, 1]);
				if(IsTileLoaded(mask[1, 0]))
					result.Add(mask[1, 0]);
				if(IsTileLoaded(mask[1, 1]))
					result.Add(mask[1, 1]);
			}
			return result;
		}
		bool IsTileLoaded(BitmapTile tile) {
			return tile != null && tile.Image != null;
		}
		MapPoint[] IRenderItemProvider.GeometryToScreenPoints(MapUnit[] geometry) {
			return new MapPoint[0];
		}
		Rectangle IRenderItemProvider.RenderClipBounds {
			get { return Rectangle.Empty; }
		}
		protected override void OnUpdate() {
			if(IsInvalid) {
				face.Source = null;
				mask[0, 0].Source = null;
				mask[1, 0].Source = null;
				mask[0, 1].Source = null;
				mask[1, 1].Source = null;
				IsInvalid = false;
			}
		}
		protected override void DisposeOverride() {
			face.Dispose();
			face = null;
			mask[0, 0].Dispose();
			mask[1, 0].Dispose();
			mask[0, 1].Dispose();
			mask[1, 1].Dispose();
		}
		protected void UpdateLocation() {
			if(face != null)
				face.Position = Location;
			UpdateMaskPosition();
		}
		internal override void UpdateRenderParams(MapPoint scaleFactor) {
			UpdateTransform();
			this.renderStretchFactor = scaleFactor;
			currentTileIndex = Index;
			UpdateBitmaps();
		}
		internal void UpdateMaskPosition() {
			if(mask[0, 0] == null)
				return;
			MapSize initialSize = new MapSize(Viewport.TileSourceWidth, Viewport.TileSourceHeight);
			MapRect transformedRect = new MapRect(Location.X, Location.Y, initialSize.Width, initialSize.Height);
			MapSize alignedSize = new MapSize(transformedRect.Width, transformedRect.Height);
			MapSize halfAlignedSize = new MapSize(alignedSize.Width / 2.0, Math.Ceiling(alignedSize.Height / 2.0));
			mask[0, 0].Position = face.Position;
			mask[1, 0].Position = new MapPoint(face.Position.X + halfAlignedSize.Width, face.Position.Y);
			mask[0, 1].Position = new MapPoint(face.Position.X, face.Position.Y + halfAlignedSize.Height);
			mask[1, 1].Position = new MapPoint(face.Position.X + halfAlignedSize.Width, face.Position.Y + halfAlignedSize.Height);
		}
		internal void OnTileSourceLoaded() {
			RedrawVisualContent();
		}
		public void UpdateRenderParameters(IRenderContext renderContext) {
		}
	}
	public class BitmapTile : IDisposable, IRenderItem, ILocatableRenderItem {
		MultiScaleTile owningTile;
		RectangleF clipImageRect;
		MapSize size;
		TileImageSource source;
		Image image;
		MapPoint position;
		IRenderItemResourceHolder resourceHolder;
		bool visible = true;
		object updateLocker = new object();
		byte Transparency { get { return this.owningTile.Viewport.MultiScaleImage.Transparency; } }
		internal Image Image { get { return image; } }
		public bool IsValid { get { return image != null; } }
		public TileImageSource Source {
			get {
				return source;
			}
			set {
				if(source != value) {
					TileImageSource oldSource = source;
					source = value;
					OnSourceChanged(oldSource, value);
				}
			}
		}
		public MapPoint Position {
			get {
				return position;
			}
			set {
				if(position != value) {
					position = value;
				}
			}
		}
		public MapSize Size {
			get {
				return size;
			}
			set {
				if(size != value) {
					size = value;
				}
			}
		}
		public BitmapTile(MultiScaleTile tile) {
			this.clipImageRect = RectangleF.Empty;
			this.owningTile = tile;
		}
		void OnSourceChanged(TileImageSource oldSource, TileImageSource newSource) {
			Image imageSource = null;
			if(oldSource != null) {
				oldSource.ImageLoaded -= SourceImageLoaded;
				oldSource.ImageDisposed -= SourceImageDisposed;
			}
			if(newSource != null) {
				newSource.ImageLoaded += SourceImageLoaded;
				newSource.ImageDisposed += SourceImageDisposed;
				if(newSource.Status == TileStatus.Ready)
					imageSource = newSource.Source;
			}
			else
				imageSource = null;
			UpdateImage(imageSource);
		}
		void SourceImageLoaded(object sender, EventArgs e) {
			TileImageSource tileImageSource = sender as TileImageSource;
			UpdateImage(tileImageSource.Source);
			owningTile.OnTileSourceLoaded();
		}
		void SourceImageDisposed(object sender, EventArgs e) {
			TileImageSource tileImageSource = sender as TileImageSource;
			UpdateImage(tileImageSource.Source);
		}
		void UpdateImage(Image source) {
			this.image = source;
		}
		MapRect GetBounds() {
			return image != null ? new MapRect(Position.X, Position.Y, this.Size.Width, this.Size.Height) : MapRect.Empty;
		}
		public void SetTileClipRect(MapRect clipRect) {
			clipImageRect = clipRect != MapRect.Empty ? clipRect.ToRectangleF() : RectangleF.Empty;
		}
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				Source = null;
				owningTile = null;
				ReleaseResourceHolder();
			}
		}
		void ReleaseResourceHolder() {
			if(resourceHolder != null) {
				resourceHolder.Dispose();
				resourceHolder = null;
			}
		}
		~BitmapTile() {
			Dispose(false);
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
		#region IRenderItem Members
		IRenderShapeTitle IRenderItem.Title { get { return null; } }
		IMapItemGeometry IRenderItem.Geometry {
			get { 
				MapRect bounds = GetBounds();
				ImageGeometry result = new ImageGeometry() {
					Bounds = bounds,
					ClipRect = clipImageRect,
					ImageRect = new RectangleF(0, 0, (float)bounds.Width, (float)bounds.Height),
					Transparency = this.Transparency,
					CachePriority = ImageCachePriority.Low
				};
				result.SetImage(this.image, false);
				return result;
			}
		}
		IRenderItemStyle IRenderItem.Style {
			get { return null; }
		}
		IRenderItemResourceHolder IRenderItem.ResourceHolder {
			get { return resourceHolder != null ? resourceHolder : RenderItemResourceHolder.Empty; }
		}
		object IRenderItem.UpdateLocker { get { return updateLocker; } }
		bool IRenderItem.ForceUpdateResourceHolder { get { return false; } set { } }
		bool IRenderItem.UseAntiAliasing { get { return true; } }
		void IRenderItem.PrepareGeometry() {
		}
		bool IRenderItem.Visible {
			get { return visible; }
		}
		void IRenderItem.OnRender() {
		}
		bool IRenderItem.CanExport() { return true; }
		void IRenderItem.SetResourceHolder(IRenderer renderer, IRenderItemProvider provider) {
			lock(updateLocker) {
				ReleaseResourceHolder();
				this.resourceHolder = renderer.CreateResourceHolder(provider, this);
			}
		}
		#endregion
		#region IFixableRenderItem Members
		MapUnit ILocatableRenderItem.Location {
			get { return new MapUnit(position.X, position.Y); }
		}
		Size ILocatableRenderItem.SizeInPixels {
			get { return new Size((int)this.Size.Width, (int)this.Size.Height); }
		}
		MapPoint ILocatableRenderItem.Origin { get { return MapPoint.Empty; } }
		MapPoint ILocatableRenderItem.StretchFactor {
			get { return owningTile.RenderStretchFactor; }
		}
		void ILocatableRenderItem.ResetLocation() {
		}
		#endregion
	}
}
