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

using DevExpress.Utils;
using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.XtraMap {
	public class MiniMapLayerCollection : DXNamedItemCollection<MiniMapLayerBase>, ISupportSwapItems  {
		readonly MiniMap map;
		protected MiniMap Map { get { return map; } }
		internal MiniMapLayerCollection(MiniMap map)
			: base(DXCollectionUniquenessProviderType.None) {
			Guard.ArgumentNotNull(map, "Map");
			this.map = map;
		}
		protected override string GetItemName(MiniMapLayerBase item) {
			return item != null ? item.Name : string.Empty;
		}
		protected override bool OnInsert(int index, MiniMapLayerBase value) {
			Map.OnLayerInsert(value);
			return base.OnInsert(index, value);
		}
		protected override bool OnRemove(int index, MiniMapLayerBase value) {
			Map.OnLayerRemove(value);
			return base.OnRemove(index, value);
		}
		protected override bool OnClear() {
			Map.OnLayersClear();
			foreach (MiniMapLayerBase layer in this)
				layer.NameChanged -= OnNameChanged;
			return base.OnClear();
		}
		public void Swap(int index1, int index2) {
			if (index1 == index2)
				return;
			MiniMapLayerBase swapLayer = InnerList[index1];
			InnerList[index1] = InnerList[index2];
			InnerList[index2] = swapLayer;
			Map.RecreateActualLayers(this);
		}
		protected internal virtual void OnNameChanged(object sender, NameChangedEventArgs e) {
			MiniMapLayerBase layer = this[e.OldName];
			NameHash.Remove(e.OldName);
			NameHash.Add(e.Name, layer);
		}
		protected override void OnInsertComplete(int index, MiniMapLayerBase value) {
			base.OnInsertComplete(index, value);
			value.NameChanged += OnNameChanged;
			Map.OnLayerInsertComplete();
		}
		protected override void OnRemoveComplete(int index, MiniMapLayerBase value) {
			value.NameChanged -= OnNameChanged;
			base.OnRemoveComplete(index, value);
		}
	}
}
