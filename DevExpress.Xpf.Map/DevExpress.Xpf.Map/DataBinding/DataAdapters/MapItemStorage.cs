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

using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	[ContentProperty("Items")]
	public class MapItemStorage : MapDataAdapterBase {
		static readonly DependencyPropertyKey ItemsPropertyKey = DependencyPropertyManager.RegisterReadOnly("Items",
			typeof(MapVectorItemCollection), typeof(MapItemStorage), new PropertyMetadata());
		public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;
		[Category(Categories.Data)]
		public MapVectorItemCollection Items { get { return (MapVectorItemCollection)GetValue(ItemsProperty); } }
		protected internal override MapVectorItemCollection ItemsCollection { get { return Items; } }
		public MapItemStorage() {
			this.SetValue(ItemsPropertyKey, new MapVectorItemCollection());
		}
		public override object GetItemSourceObject(MapItem item) {
			return item;
		}
		protected override MapDependencyObject CreateObject() {
			return new MapItemStorage();
		}
		protected override void LoadDataCore() {
			if(Layer != null)
				Layer.OnDataLoaded();
		}
		protected internal override bool IsCSCompatibleTo(MapCoordinateSystem coordinateSystem) {
			return true;
		}
	}
	public class MapVectorItemCollection : MapDependencyObjectCollection<MapItem> {
		VirtualMapItemCollection virtualizingCollection = new VirtualMapItemCollection();
		bool updateLocked = false;
		bool addToVirtualEnabled = true;
		VectorLayerBase Layer { get { return Owner as VectorLayerBase; } }
		internal VirtualMapItemCollection VirtualizingCollection { get { return virtualizingCollection; } set { virtualizingCollection = value; } }
		void RemoveVirtualItem(MapItem item) {
			virtualizingCollection.RemoveMapItem(item);
			RemoveConnectedItem(item);
		}
		void RemoveConnectedItem(MapItem item) {
			MapShapeBase shape = item as MapShapeBase;
			if (shape != null) {
				CommonUtils.SetItemOwner(shape.Title, null);
				virtualizingCollection.RemoveMapItem(shape.Title);
			}
		}
		void AddConnectedItem(MapItem item) {
			MapShapeBase shape = item as MapShapeBase;
			if (shape != null) {
				CommonUtils.SetItemOwner(shape.Title, Layer);
				virtualizingCollection.PushMapItem(shape.Title);
			}
		}
		void RemoveItemOwner(MapItem item) {
			((IOwnedElement)item).Owner = null;
			MapShapeBase shape = item as MapShapeBase;
			if (shape != null)
				((IOwnedElement)shape.Title).Owner = null;
		}
		void AddVirtualItem(MapItem item, int index) {
			virtualizingCollection.PushMapItem(item, index);
			AddConnectedItem(item);
		}
		void ColorizeItems() {
			if (Layer != null && !updateLocked) {
				Layer.ResetColors();
				Layer.ColorizeItems();
				Layer.UpdateLegends();
			}
		}
		protected override void InsertItem(int index, MapItem item) {
			base.InsertItem(index, item);
			if(this.addToVirtualEnabled) {
				AddVirtualItem(item, index);
				ColorizeItems();
			}
		}
		protected override void RemoveItem(int index) {
			MapItem itemToRemove = this[index];
			base.RemoveItem(index);
			if(this.addToVirtualEnabled) {
				RemoveVirtualItem(itemToRemove);
				ColorizeItems();
			}
		}
		protected override void SetItem(int index, MapItem item) {
			MapItem itemToRemove = this[index];
			base.SetItem(index, item);
			if(this.addToVirtualEnabled) {
				AddVirtualItem(item, index);
				RemoveVirtualItem(itemToRemove);
				ColorizeItems();
			}
		}
		protected override void ClearItems() {
			foreach (MapItem item in this)
				RemoveItemOwner(item);
			base.ClearItems();
			if(this.addToVirtualEnabled) {
				virtualizingCollection.Clear();
				ColorizeItems();
			}
		}
		protected override void MoveItem(int oldIndex, int newIndex) {
			MapItem item = this[oldIndex];
			base.MoveItem(oldIndex, newIndex);
			if(this.addToVirtualEnabled) {
				virtualizingCollection.RemoveMapItem(item);
				virtualizingCollection.PushMapItem(item, newIndex);
				ColorizeItems();
			}
		}
		internal void FillVirtualizingCollection() {
			for (int i = 0; i < Count; i++) {
				AddVirtualItem(this[i], i);
			}
		}
		internal void ClearVirtualizingCollection(bool clearClusters) {
			if(clearClusters)
				virtualizingCollection.ClearClusters();
			else
				virtualizingCollection.Clear();
		}
		internal void BeginUpdate(ISupportVirtualizationData supportVirtualizationData) {
			BeginUpdate();
			addToVirtualEnabled = supportVirtualizationData != null ? !supportVirtualizationData.IsPreprocessingData : true;
		}
		internal void EndUpdate(bool unlockOnly) {
			updateLocked = false;
			addToVirtualEnabled = true;
		}
		public void BeginUpdate() {
			updateLocked = true;
		}
		public void EndUpdate() {
			EndUpdate(true);
			ColorizeItems();
		}
	}
}
