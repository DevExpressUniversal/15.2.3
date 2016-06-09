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
using System.Linq;
using System.Text;
using DevExpress.Utils;
using System.Drawing;
using DevExpress.XtraMap.Native;
namespace DevExpress.XtraMap {
	public abstract class OwnedCollection<T> : NotificationCollection<T> where T : IOwnedElement {
		readonly object owner;
		protected OwnedCollection(object owner, DXCollectionUniquenessProviderType type)
			: base(type) {
			this.owner = owner;
		}
		protected OwnedCollection(object owner) {
			this.owner = owner;
		}
		protected object Owner { get { return owner; } }
		protected override void OnInsertComplete(int index, T value) {
			((IOwnedElement)value).Owner = owner;
			base.OnInsertComplete(index, value);
		}
		protected override void OnRemoveComplete(int index, T value) {
			((IOwnedElement)value).Owner = null;
			base.OnRemoveComplete(index, value);
		}
		protected override bool OnClear() {
			for (int i = 0; i < Count; i++)
				((IOwnedElement)this[i]).Owner = null;
			return base.OnClear();
		}
	}
	public class LayerCollection : DXNamedItemCollection<LayerBase>, ISupportSwapItems {
		readonly InnerMap map;
		protected InnerMap Map { get { return map; } }
		internal LayerCollection(InnerMap map)
			: base(DXCollectionUniquenessProviderType.None) {
			Guard.ArgumentNotNull(map, "Map");
			this.map = map;
		}
		protected override string GetItemName(LayerBase item) {
			return item != null ? item.Name : string.Empty;
		}
		protected override void InsertIfNotAlreadyInserted(int index, LayerBase obj) {
			base.InsertIfNotAlreadyInserted(index, obj.LayerInCollection);
		}
		protected override int AddIfNotAlreadyAdded(LayerBase obj) {
			return base.AddIfNotAlreadyAdded(obj.LayerInCollection);
		}
		protected override bool RemoveIfAlreadyAdded(LayerBase obj) {
			return base.RemoveIfAlreadyAdded(obj.LayerInCollection);
		}
		protected override bool OnInsert(int index, LayerBase value) {
			Map.OnLayerInsert(value);
			return base.OnInsert(index, value);
		}
		protected override bool OnRemove(int index, LayerBase value) {
			Map.OnLayerRemove(value);
			return base.OnRemove(index, value);
		}
		protected override bool OnClear() {
			Map.OnLayersClear();
			foreach (LayerBase layer in this)
				layer.NameChanged -= OnNameChanged;
			return base.OnClear();
		}
		public void Swap(int index1, int index2) {
			if (index1 == index2)
				return;
			LayerBase swapLayer = InnerList[index1];
			InnerList[index1] = InnerList[index2];
			InnerList[index2] = swapLayer;
			Map.RecreateActualLayers(this);
		}
		protected internal virtual void OnNameChanged(object sender, NameChangedEventArgs e) {
			LayerBase layer = this[e.OldName];
			NameHash.Remove(e.OldName);
			NameHash.Add(e.Name, layer);
		}
		protected override void OnInsertComplete(int index, LayerBase value) {
			base.OnInsertComplete(index, value);
			value.NameChanged += OnNameChanged;
		}
		protected override void OnRemoveComplete(int index, LayerBase value) {
			value.NameChanged -= OnNameChanged;
			base.OnRemoveComplete(index, value);
		}
	}
}
namespace DevExpress.XtraMap.Native {
	public class SortedLayerCollection : OwnedCollection<LayerBase> {
		protected new IMapView Owner { get { return (IMapView)base.Owner; } }
		public SortedLayerCollection(IMapView owner) : base(owner) {
		}
		protected override void OnInsertComplete(int index, LayerBase value) {
			base.OnInsertComplete(index, value);
			Sort(LayerZIndexComparer.Default);
		}
		protected override void OnRemoveComplete(int index, LayerBase value) {
			base.OnRemoveComplete(index, value);
			Sort(LayerZIndexComparer.Default);
		}
		protected override void OnSetComplete(int index, LayerBase oldValue, LayerBase newValue) {
			base.OnSetComplete(index, oldValue, newValue);
			Sort(LayerZIndexComparer.Default);
		}
	}
	public class LayerZIndexComparer : IComparer<LayerBase> {
		static readonly LayerZIndexComparer instance = new LayerZIndexComparer();
		public static LayerZIndexComparer Default { get { return instance; } }
		int IComparer<LayerBase>.Compare(LayerBase x, LayerBase y) {
			if (x == y)
				return 0;
			if (x == null)
				return -1;
			if (y == null)
				return 1;
			int result = y.ZIndex.CompareTo(x.ZIndex);
			if (result != 0)
				return result;
			IComparable<LayerBase> comparableX = x as IComparable<LayerBase>;
			IComparable<LayerBase> comparableY = y as IComparable<LayerBase>;
			if (comparableX != null && comparableY != null)
				return comparableY.CompareTo(x);
			else
				return 0;
		}
	}
}
