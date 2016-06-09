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
using DevExpress.Map.Native;
using DevExpress.XtraMap.Drawing;
using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
namespace DevExpress.XtraMap {
	public class ImageTilesLayer : LayerBase {
		MapDataProviderBase provider;
		MultiScaleImage multiScaleImage;
		byte transparency = ImageGeometry.DefaultTransparency;
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("ImageTilesLayerDataProvider"),
#endif
		DefaultValue(null), Category(SRCategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Editor("DevExpress.XtraMap.Design.DataProviderPickerEditor," + AssemblyInfo.SRAssemblyMapDesign, typeof(UITypeEditor))
		]
		public MapDataProviderBase DataProvider {
			get { return provider; }
			set {
				if (provider == value)
					return;
				if (provider != null) {
					UnsubscribeToTileSourceChanged(provider);
					provider.Dispose();
					MapUtils.SetOwner(provider, null);
				}
				provider = value;
				OnDataProviderChanged();
			}
		}
		[Category(SRCategoryNames.Appearance), DefaultValue(ImageGeometry.DefaultTransparency)]
		public byte Transparency {
			get { return transparency; }
			set {
				if (transparency == value)
					return;
				transparency = value;
				InvalidateRender();
			}
		}
		protected internal MultiScaleImage MultiScaleImage { get { return multiScaleImage; } } 
		protected internal ProjectionBase Projection { get { return DataProvider != null ? DataProvider.Projection : null; } }
		protected override int DefaultZIndex { get { return 300; } }
		public ImageTilesLayer() {
			this.multiScaleImage = new MultiScaleImage(this);
		}
		#region events
#if !SL
	[DevExpressXtraMapLocalizedDescription("ImageTilesLayerRequestDataLoading")]
#endif
		public event EventHandler RequestDataLoading;
		internal void RaiseRequestDataLoadingCore() {
			RequestDataLoading(this, EventArgs.Empty);
		}
		internal void RaiseRequestDataLoading() {
			if(RequestDataLoading != null)
				RaiseEventAsync(new Action(RaiseRequestDataLoadingCore));
		}
		#endregion
		internal override void AfterRender() {
			MultiScaleImage.CheckTilesLoadCompleted();
		}
		protected override bool IsReadyForRender {
			get {
				if(!ShouldLoadData()) return false;
				if(DataProvider == null || DataProvider.TileSource == null)
					return true;
				return MultiScaleImage.Viewport.VisibleTilesStatus == VisibleTilesStatus.Loaded;
			}
		}
		void OnDataProviderChanged() {
			if(provider != null) {
				MapUtils.SetOwner(provider, this);
				if (Map != null)
					Map.UpdateViewportRect(false);
				UpdateTileSource(provider.TileSource);
				SubscribeToTileSourceChanged(provider);
			}
			ResetErrorPanel();
		}
		void UnsubscribeToTileSourceChanged(MapDataProviderBase provider) {
			provider.TileSourceChanged -= OnTileSourceChanged;
		}
		void SubscribeToTileSourceChanged(MapDataProviderBase provider) {
			provider.TileSourceChanged += OnTileSourceChanged;
		}
		void OnTileSourceChanged(object sender, GenericPropertyChangedEventArgs<MapTileSourceBase> args) {
			UpdateTileSource(args.NewValue);
			ResetErrorPanel();
		}
		void ResetErrorPanel() {
			if(Map != null)
				Map.ResetErrorPanel();
		}
		void UpdateTileSource(MultiScaleTileSource source) {
			MultiScaleImage.Source = source;
		}
		void ApplyViewport() {
			MapRect viewport = AnimatedViewportRect;
			MultiScaleImage.ViewportOrigin = new MapPoint(viewport.Left, viewport.Top);
			MultiScaleImage.ViewportSize = new MapSize(viewport.Width, viewport.Height);
		}
		void DisposeInternalResources() {
			if(provider != null)
				provider.DisposeInternalResources();
		}
		void CreateInternalResources() {
			if(provider != null)
				provider.CreateInternalResources();
		}
		protected override void OnVisibleChanged() {
			ResetErrorPanel();
			base.OnVisibleChanged();
		}
		protected override void ChangeOwner(object newOwner) {
			if(newOwner == null) {
				ResetErrorPanel();
				DisposeInternalResources();
			}
			else
				CreateInternalResources();
			base.ChangeOwner(newOwner);
		}
		protected override void OwnerChanged() {
			base.OwnerChanged();
			if(Owner != null)
				ApplyViewport();
		}
		protected override void DisposeOverride() {
			if(provider != null) {
				UnsubscribeToTileSourceChanged(provider);
				provider.Dispose();
			}
			if(multiScaleImage != null) {
				multiScaleImage.Dispose();
				multiScaleImage = null;
			}
		}
		protected internal override Size GetMapBaseSizeInPixels() {
			 if(DataProvider != null)
				 return DataProvider.BaseSizeInPixels;
			 return base.GetMapBaseSizeInPixels();
		}
		protected internal override MapSize GetMapSizeInPixels(double zoomLevel) {
			if(DataProvider != null)
				return DataProvider.GetMapSizeInPixels(zoomLevel);
			else {
				double size = zoomLevel < 1.0 ? InnerMap.DefaultMapSize * zoomLevel : InnerMap.DefaultMapSize / 2.0 * Math.Pow(2.0, zoomLevel);
				return new MapSize(size, size);
			}
		}
		protected internal override void OnSetClientSize(Size size) {
			base.OnSetClientSize(size);
			MultiScaleImage.ClientSize = size;
		}
		protected internal override void ViewportUpdated() {
			base.ViewportUpdated();
			ApplyViewport();
		}
		protected bool CanShowTiles() {
			if (Map != null) 
				return Map.OperationHelper.CanShowImageTilesLayer(Projection, DataProvider.BaseSizeInPixels);
			return View != null;
		}
		protected override IEnumerable<IRenderItem> GetRenderItems() {
			if(DataProvider != null) { 
				if(CanShowTiles())
					foreach(IRenderItem item in multiScaleImage.GetRenderItems())
						yield return item;
			}
		}
		protected internal override MapPoint CalculateRenderOffset(double zoomLevel, CoordPoint centerPoint, Rectangle contentBounds) {
			return MultiScaleImage.Viewport.CalculateRenderOffset(contentBounds.Location);
		}
		protected override void PrepareForRendering() {
			if(DataProvider != null)
				DataProvider.PrepareData();			
			MultiScaleImage.StartLoadTiles();
		}
		protected override object GetUpdateLocker() {
			return MultiScaleImage.Viewport.TilesUpdateLocker;
		}
		protected internal override bool CheckVisibility() {
			return base.CheckVisibility() && DataProvider != null;
		}
		public override string ToString() {
			return "(ImageTilesLayer)";
		}
	}
}
