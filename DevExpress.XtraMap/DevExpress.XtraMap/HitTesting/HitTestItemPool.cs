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
using System.Collections;
namespace DevExpress.XtraMap.Native {
	public class HitTestItemPool {
		Dictionary<long, HitTestItem> unitGeometryItems;
		Dictionary<long, HitTestItem> screenGeometryItems;
		public int ScreenGeometryCount { get { return screenGeometryItems.Count;  } }
		public int UnitGeometryCount { get { return unitGeometryItems.Count; } }
		public HitTestItemPool() {
			this.unitGeometryItems = new Dictionary<long, HitTestItem>();
			this.screenGeometryItems = new Dictionary<long,HitTestItem>();
		}
		protected Dictionary<long, HitTestItem> SelectGeometryList(HitTestKey key) {
			bool isScreen = key.IsScreenGeometry();
			return isScreen ? this.screenGeometryItems : unitGeometryItems;
		}
		protected HitTestItem CreateItem(IHitTestableElement element) {
			return new HitTestItem(element);
		}
		public void RegisterItem(IHitTestableElement element) {
			HitTestKey key = element.Key;
			Dictionary<long, HitTestItem> items = SelectGeometryList(key);
			long value = key.Value;
			if(items.ContainsKey(value))
				return;
			HitTestItem item = CreateItem(element);
			items.Add(value, item);
		}
		public void UnregisterItem(IHitTestableElement element) {
			HitTestKey key = element.Key;
			Dictionary<long, HitTestItem> items = SelectGeometryList(key);
			items.Remove(key.Value);
		}
		public IHitTestableElement Search(HitTestKey key) {
			Dictionary<long, HitTestItem> items = SelectGeometryList(key);
			HitTestItem item;
			if(items.TryGetValue(key.Value, out item))
				return item != null ? item.Element : null;
			return null;
		}
		public void ClearItems() {
			screenGeometryItems.Clear();
			unitGeometryItems.Clear();
		}
		public void ClearItems(MapItemsLayerBase layer) {
			if(layer == null)
				return;
			foreach(MapItem item in layer.DataItems)
				UnregisterItem(item);
		}
		protected internal HitTestItem GetUnitGeometryItem(int key) {
			HitTestItem item;
			if(unitGeometryItems.TryGetValue(key, out item))
				return item;
			return null;
		}
		protected internal HitTestItem GetScreentGeometryItem(int key) {
			HitTestItem item;
			if(screenGeometryItems.TryGetValue(key, out item))
				return item;
			return null;
		}
	}
}
