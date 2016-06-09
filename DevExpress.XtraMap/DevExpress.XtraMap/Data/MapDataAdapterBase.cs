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
using DevExpress.Utils;
using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
namespace DevExpress.XtraMap {
	public abstract class MapDataAdapterBase : MapDisposableObject, IMapDataAdapter, ILockableObject {
		MapItemsLayerBase layer;
		readonly MapItemCollection innerItems;
		MapSupportObjectChangedListener<MapItem> itemListener;
		object updateLocker = new object();
		IMapItemFactory mapItemFactory;
		IClusterer clusterer;
		object ILockableObject.UpdateLocker { 
			get { return updateLocker; } 
		}
		protected abstract bool IsReady { get; }
		protected MapItemsLayerBase Layer {
			get { return layer; }
			private set {
				layer = value;
				OnLayerChanged();
			}
		}
		protected internal object UpdateLocker { get { return updateLocker; } }
		protected internal MapItemCollection InnerItems { get { return innerItems; } }
		protected MapDataAdapterBase() {
			this.innerItems = new MapItemCollection(this);
			itemListener = new MapSupportObjectChangedListener<MapItem>(InnerItems);
			SubscribeItemListenerEvent();
			SubscribeItemCollectionEvents();
		}
#if !SL
	[DevExpressXtraMapLocalizedDescription("MapDataAdapterBaseDataChanged")]
#endif
		public event DataAdapterChangedEventHandler DataChanged;
		[Category(SRCategoryNames.Data), DefaultValue(null),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		TypeConverter("DevExpress.XtraMap.Design.ExpandableNoneStringSupportedTypeConverter," + AssemblyInfo.SRAssemblyMapDesign),
		Editor("DevExpress.XtraMap.Design.MapClustererPickerEditor," + AssemblyInfo.SRAssemblyMapDesign, typeof(UITypeEditor)),
		RefreshProperties(RefreshProperties.All)]
		public IClusterer Clusterer {
			get { return clusterer; }
			set {
				if (clusterer == value)
					return;
				if (clusterer != null)
					clusterer.SetOwner(null);
				clusterer = value;
				if (clusterer != null)
					clusterer.SetOwner(this);
				else
					NotifyClustererDataChanged();
				if (Layer != null)
					Layer.InvalidateRender();
			}
		}
		#region IMapDataAdapter members
		int IMapDataAdapter.Count { get { return innerItems.Count; } }
		MapItemType IMapDataAdapter.DefaultMapItemType { get { return GetDefaultMapItemType(); } }
		bool IMapDataAdapter.IsReady { get { return IsReady; } }
		void IMapDataAdapter.LoadData(IMapItemFactory factory) {
			IMapItemFactory actualFactory = mapItemFactory != null ? mapItemFactory : factory;
			LoadData(actualFactory);
			Clusterize(true);
		}
		IEnumerable<MapItem> IMapDataAdapter.Items { get { return GetMapItems(); } }
		IEnumerable<MapItem> IMapDataAdapter.SourceItems { get { return InnerItems; } }
		object IMapDataAdapter.GetItemSourceObject(MapItem item) {
			return GetItemSourceObject(item);
		}
		MapItem IMapDataAdapter.GetItemBySourceObject(object sourceObject) {
			return GetItemBySourceObject(sourceObject);
		}
		MapItem IMapDataAdapter.GetItem(int index) {
			return GetItemByIndex(index);
		}
		void IMapDataAdapter.SetLayer(MapItemsLayerBase layer) {
		   SetLayer(layer);
		}
		MapItemsLayerBase IMapDataAdapter.GetLayer() { 
			return layer; 
		}
		bool IMapDataAdapter.IsCSCompatibleTo(MapCoordinateSystem mapCS) {
			return IsCSCompatibleTo(mapCS);
		}
		void IMapDataAdapter.OnViewportUpdated(MapViewport viewport) {
			ClusterizeCore(viewport, false);
		}
		void IMapDataAdapter.OnClustered() {
			NotifyClustererDataChanged();
		}
		#endregion
		DataAdapterChangedEventArgs CreateDataChangedEventArgs(MapUpdateType updateType) {
			return new DataAdapterChangedEventArgs(updateType);
		}
		void SubscribeItemListenerEvent() {
			itemListener.Changed += OnItemListenerChanged;
		}
		void UnsubscribeItemListenerEvent() {
			itemListener.Changed -= OnItemListenerChanged;
		}
		void SubscribeItemCollectionEvents() {
			if (innerItems != null)
				innerItems.CollectionChanged += OnItemCollectionChanged;
		}
		void UnsubscribeItemCollectionEvents() {
			if (innerItems != null)
				innerItems.CollectionChanged -= OnItemCollectionChanged;
		}
		void ClusterizeCore(MapViewport viewport, bool sourceChanged) {
			if (Clusterer != null)
				Clusterer.Clusterize(InnerItems, viewport, sourceChanged);
		}
		void Clusterize(bool sourceChanged) {
			if (Layer != null && Layer.View != null)
				ClusterizeCore(Layer.GetViewport(), sourceChanged);
		}
		void NotifyClustererDataChanged() {
			NotifyDataChangedCore(GetDataChangedEventUpdates() | MapUpdateType.ViewInfo);
		}
		void NotifyDataChangedCore(MapUpdateType updateType) {
			if (DataChanged != null)
				DataChanged(this, CreateDataChangedEventArgs(updateType));
		}
		IList<MapItem> GetClusterItems() {
			if(Clusterer.Items != null)
				return Clusterer.Items;
			return new List<MapItem>();
		}
		protected abstract void LoadData(IMapItemFactory factory);
		protected abstract object GetItemSourceObject(MapItem item);
		protected abstract MapItem GetItemBySourceObject(object sourceObject);
		protected abstract bool IsCSCompatibleTo(MapCoordinateSystem mapCS);
		protected virtual IList<MapItem> GetMapItems() {
			return Clusterer != null ? GetClusterItems() : InnerItems;
		}
		protected virtual MapItem GetItemByIndex(int index) {
			return InnerItems.GetItemInternal(index);
		}
		protected virtual void ClearItems() {
			InnerItems.Clear();
		}
		protected virtual void AggregateItems() {
		}
		protected virtual void OnLayerChanged() {
		}
		protected virtual void SetLayer(MapItemsLayerBase layer) {
			this.Layer = layer;
		}
		protected virtual void OnItemListenerChanged(object sender, EventArgs e) {
			NotifyDataChanged(GetDataChangedEventUpdates());
		}
		protected virtual MapUpdateType GetDataChangedEventUpdates() {
			return MapUpdateType.Data;
		}
		protected virtual void OnItemCollectionChanged(object sender, CollectionChangedEventArgs<MapItem> e) {
			if(e.Action != CollectionChangedAction.EndBatchUpdate)
				NotifyDataChanged(GetDataChangedEventUpdates());
		}
		protected override void DisposeOverride() {
			if(itemListener != null) {
				UnsubscribeItemListenerEvent();
				itemListener = null;
			}
			if(innerItems != null) {
				UnsubscribeItemCollectionEvents();
				innerItems.BeginUpdate();
				innerItems.Clear();
				innerItems.CancelUpdate();
			}
		}
		protected void NotifyDataChanged(MapUpdateType updateType) {
			NotifyDataChangedCore(updateType);
			if (updateType.HasFlag(MapUpdateType.Data))
			   Clusterize(true);
		}
		protected internal abstract MapItemType GetDefaultMapItemType();
		protected internal virtual MapItem GetItemByRowIndex(int rowIndex) {
			return InnerItems.GetItemByRowIndex(rowIndex);
		}
		public void SetMapItemFactory(IMapItemFactory factory) {
			this.mapItemFactory = factory;
		}
	}
}
