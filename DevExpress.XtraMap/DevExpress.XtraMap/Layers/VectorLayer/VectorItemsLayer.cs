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
using System.ComponentModel;
using System.Drawing.Design;
using DevExpress.XtraMap.Native;
using DevExpress.Map.Native;
using DevExpress.XtraMap.Drawing;
namespace DevExpress.XtraMap {
	public class VectorItemsLayer : MapItemsLayerBase, ILayerDataManagerProvider {
		public const MapItemType MapItemTypeDefault = MapItemType.Unknown;
		CoordBounds boundingRect = CoordBounds.Empty;
		protected override int DefaultZIndex { get { return 100; } }
		protected override bool IsReadyForRender { get { return Data != null ? Data.IsReady : true; } }
		protected internal override CoordBounds BoundingRect { get { return boundingRect; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("VectorItemsLayerColorizer"),
#endif
		Category(SRCategoryNames.Appearance), DefaultValue(null),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		TypeConverter("DevExpress.XtraMap.Design.ExpandableNoneStringSupportedTypeConverter," + AssemblyInfo.SRAssemblyMapDesign),
		Editor("DevExpress.XtraMap.Design.ColorizerPickerEditor," + AssemblyInfo.SRAssemblyMapDesign, typeof(UITypeEditor)),
		RefreshProperties(RefreshProperties.All)
		]
		public new MapColorizer Colorizer {
			get { return base.Colorizer; }
			set { base.Colorizer = value; }
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("VectorItemsLayerData"),
#endif
		Category(SRCategoryNames.Data), DefaultValue(null),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		TypeConverter("DevExpress.XtraMap.Design.ExpandableNoneStringSupportedTypeConverter," + AssemblyInfo.SRAssemblyMapDesign),
		Editor("DevExpress.XtraMap.Design.MapDataAdapterPickerEditor," + AssemblyInfo.SRAssemblyMapDesign, typeof(UITypeEditor)),
		RefreshProperties(RefreshProperties.All)
		]
		public IMapDataAdapter Data {
			get { return DataAdapter; }
			set {
				if (object.Equals(DataAdapter, value))
					return;
				SetDataAdapterInternal(value);
			}
		}
		#region ILayerDataManagerProvider implementation
		LayerDataManager ILayerDataManagerProvider.DataManager {
			get {
				ILayerDataManagerProvider provider = Data as ILayerDataManagerProvider;
				return provider != null ? provider.DataManager : null;
			}
		}
		#endregion
		protected override bool ShouldLoadData() {
			bool result = base.ShouldLoadData() &&  (IsValidData && !Data.IsReady);
			return result;
		}
		protected override IMapDataAdapter CreateDataAdapter() {
			return null;
		}
		protected override void PrepareForRendering() {
			IColorizerLegendDataProvider provider = Colorizer as IColorizerLegendDataProvider;
			if(provider != null) provider.StartColorize();
		}
		internal override void AfterRender() {
			if(Map != null) {
				IColorizerLegendDataProvider provider = Colorizer as IColorizerLegendDataProvider;
				if(provider != null) provider.EndColorize();
			}
			base.AfterRender();
		}
		protected override IList<MapLegendItemBase> CreateLegendItems(MapLegendBase legend) {
			ILegendDataProvider provider = Data as ILegendDataProvider;
			if (provider != null && legend is SizeLegend) {
				IList<MapLegendItemBase> result = provider.CreateItems(legend);
				ColorizeLegendItems(result);
				return result;
			}
			return base.CreateLegendItems(legend);
		}
		internal void DataBind() {
			if(Map != null) Map.DataBind(this);
		}	 
		internal void ColorizeLegendItems(IList<MapLegendItemBase> items) {
			if(MapUtils.IsColorEmpty(ItemStyle.Fill))
				return;
			foreach(MapLegendItemBase item in items)
				item.Color = ItemStyle.Fill;
		}		
		internal void ResetBoundingRect() {
			this.boundingRect = CoordBounds.Empty;
		}
		internal void AppendBoundingRect(CoordBounds nativeBounds) {
			this.boundingRect = CoordBounds.Union(boundingRect, nativeBounds);
		}
		protected internal override void LoadData() {
			if (IsValidData) {
				IMapItemFactory factory = Map != null ? Map.MapItemFactory : DefaultMapItemFactory.Instance;
				Data.LoadData(factory);
				if (EnableSelection) SelectionController.ApplySelection();
				ShouldRaiseDataLoadedEvent = true;
			}
		}
		protected internal override void EnsureBoundingRect() {
			CoordBounds oldBoundingRect = CoordBounds.Empty;
			CoordBounds newBoundingRect;
			ISupportBoundingRectAdapter boundingRectAdapter = Data as ISupportBoundingRectAdapter;
			if (boundingRectAdapter == null) {
				oldBoundingRect = BoundingRect;
				ResetBoundingRect();
				lock (UpdateLocker) {
					EnumerateDataItems(mapItem => mapItem.UpdateNativeBounds(), true);
				}
				newBoundingRect = BoundingRect;
			}
			else {
				newBoundingRect = boundingRectAdapter.BoundingRect;
				lock (UpdateLocker) {
					EnumerateDataItems(mapItem => mapItem.ResetUnitLocation(), true);
				}
				this.boundingRect = newBoundingRect;
			}
			if (newBoundingRect != oldBoundingRect && View != null)
				View.CoordinateSystem.SetNeedUpdateBoundingBox(true);
			base.EnsureBoundingRect();
		}
		public override string ToString() {
			return "(VectorItemsLayer)";
		}
		public object GetItemSourceObject(MapItem item) {
			object sourceItem = Data != null ? Data.GetItemSourceObject(item) : null;
			return sourceItem != null ? sourceItem : item;
		}
		public MapItem GetMapItemBySourceObject(object sourceObject) {
			return Data != null ? Data.GetItemBySourceObject(sourceObject) : null;
		}
	}
}
