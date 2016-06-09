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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
namespace DevExpress.Xpf.Map {
	public abstract class GenericLegendItemCollection<TLegendItem> : ObservableCollection<TLegendItem> where TLegendItem : MapLegendItemBase {
	}
	public class ColorLegendItemCollection : GenericLegendItemCollection<ColorLegendItem> {
	}
	public class SizeLegendItemCollection : GenericLegendItemCollection<SizeLegendItem> {
	}
}
namespace DevExpress.Xpf.Map.Native {
	public class MapLegendItemCollection : ObservableCollection<MapLegendItemBase> {
	}
	public class ActualLegendItemCollection : ObservableCollection<MapLegendItemBase> {
		readonly MapLegendBase legend;
		public ActualLegendItemCollection(MapLegendBase legend) {
			this.legend = legend;
		}
		void SubscribeCollectionChangedEvent(object collection) {
			INotifyCollectionChanged notificationCollection = collection as INotifyCollectionChanged;
			if (notificationCollection != null)
				notificationCollection.CollectionChanged += CustomItemsCollectionChanged;
		}
		void UnsubscribeCollectionChangedEvent(object collection) {
			INotifyCollectionChanged notificationCollection = collection as INotifyCollectionChanged;
			if (notificationCollection != null)
				notificationCollection.CollectionChanged -= CustomItemsCollectionChanged;
		}
		void AddCustomItems(IEnumerable<MapLegendItemBase> customItems) {
			if (customItems != null) {
				foreach (MapLegendItemBase legendItem in customItems) {
					this.Add(legendItem);
				}
			}
		}
		void CustomItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			UpdateCustomItems(legend.CustomItemsInternal);
		}
		void UpdateCustomItems(IEnumerable<MapLegendItemBase> customItems) {
			int indexOffset = legend.InnerItems.Count;
			int totalCount = this.Count;
			for (int i = totalCount - 1; i >= indexOffset; i--) {
				this.RemoveAt(i);
			}
			AddCustomItems(customItems);
		}
		public void CustomItemsChanged(object oldValue, object newValue) {
			UnsubscribeCollectionChangedEvent(oldValue);
			SubscribeCollectionChangedEvent(newValue);
			UpdateCustomItems(newValue as IEnumerable<MapLegendItemBase>);
		}
		public void Update() {
			this.Clear();
			foreach (MapLegendItemBase legendItem in legend.InnerItems) {
				this.Add(legendItem);
			}
			AddCustomItems(legend.CustomItemsInternal);
		}
	}
}
