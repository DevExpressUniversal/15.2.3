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
using System.Collections;
using System.Windows;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.Xpf.TreeMap.Native;
using DevExpress.TreeMap.Core;
using DevExpress.Data;
using System.ComponentModel;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.TreeMap {
	public class TreeMapHierarchicalDataAdapter : TreeMapDataAdapterBase, ISupportNativeSource {
		public static readonly DependencyProperty DataSourceProperty = DependencyProperty.Register("DataSource",
			typeof(object), typeof(TreeMapHierarchicalDataAdapter), new PropertyMetadata(null, UpdateData));
		public static readonly DependencyProperty ValueDataMemberProperty = DependencyProperty.Register("ValueDataMember",
			typeof(string), typeof(TreeMapHierarchicalDataAdapter), new PropertyMetadata(string.Empty, UpdateData));
		static void UpdateData(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			TreeMapHierarchicalDataAdapter adapter = d as TreeMapHierarchicalDataAdapter;
			if (adapter != null)
				adapter.UpdateData();
		}
		public object DataSource {
			get { return GetValue(DataSourceProperty); }
			set { SetValue(DataSourceProperty, value); }
		}
		public string ValueDataMember {
			get { return (string)GetValue(ValueDataMemberProperty); }
			set { SetValue(ValueDataMemberProperty, value); }
		}
		readonly TreeMapItemCollection items;
		readonly TreeMapItemsCollector collector;
		readonly HierarhicalDataController controller;
		protected internal override TreeMapItemCollection ItemsCollection { get { return items; } }
		public TreeMapHierarchicalDataAdapter() {
			items = new TreeMapItemCollection();
			collector = new TreeMapItemsCollector(items);
			controller = new HierarhicalDataController(this);
		}
		IEnumerable ISupportNativeSource.GetNativeSource() {
			return DataSource as IEnumerable;
		}
		TreeMapItem INativeItemsCollector.ProcessNativeItem(object item, object parent) {
			double value = Convert.ToDouble(GetValue(item, ValueDataMember));
			controller.SubscribePropertiesChanged(item);
			return collector.ProcessNativeItem(item, parent, value);
		}
		void UpdateData() {
			collector.Reset();
		}		
		object GetValue(object item, string propertyName) {
			if (item != null) {
				Type type = item.GetType();
				PropertyInfo property = type.GetProperty(propertyName);
				if (property != null)
					return property.GetValue(item, null);
			}
			return null;
		}
		protected override TreeMapDependencyObject CreateObject() {
			return new TreeMapHierarchicalDataAdapter();
		}
		internal void UpdateItemValue(object nativeItem, double value) {
			collector.UpdateItemValue(nativeItem, value);
		}
	}
}
namespace DevExpress.Xpf.TreeMap.Native {
	public interface INativeItemsCollector {
		TreeMapItem ProcessNativeItem(object item, object parent);
	}
	public interface ISupportNativeSource : INativeItemsCollector {
		IEnumerable GetNativeSource();
	}
	public class TreeMapItemsCollector {	  
		readonly TreeMapItemCollection treeMapItems;
		Dictionary<object, TreeMapItem> mapping = new Dictionary<object, TreeMapItem>();
		public TreeMapItemsCollector(TreeMapItemCollection treeMapItems) {
			this.treeMapItems = treeMapItems;
		}
		internal void UpdateItemValue(object nativeItem, double value) {
			mapping[nativeItem].Value = value;
		}
		public TreeMapItem ProcessNativeItem(object item, object parent, double value) {
			TreeMapItem treeMapItem = new TreeMapItem() { Tag = item, Value = value };
			if (parent == null) {
				if (!mapping.ContainsKey(item)) {
					mapping.Add(item, treeMapItem);
					treeMapItems.Add(treeMapItem);
				}
			}
			else {
				TreeMapItem parentMapItem;
				if (mapping.TryGetValue(parent, out parentMapItem)) {
					parentMapItem.Children.Add(treeMapItem);
					mapping.Add(item, treeMapItem);
				}
			}
			return treeMapItem;
		}
		public void Reset() {
			mapping.Clear();
			treeMapItems.Clear();
		}
	}
	public class HierarhicalDataController : IWeakEventListener {
		TreeMapHierarchicalDataAdapter adapter;
		public HierarhicalDataController(TreeMapHierarchicalDataAdapter adapter) {
			this.adapter = adapter;
		}
		bool ProcessWeakEvent(object sender) {
			Type type = sender.GetType();
			PropertyInfo property = type.GetProperty(adapter.ValueDataMember);
			if (property != null) {
				adapter.UpdateItemValue(sender, Convert.ToDouble(property.GetValue(sender, null)));				
				return true;
			}
			return false;
		}
		internal void SubscribePropertiesChanged(object item) {
			if (item == null) return;
			INotifyPropertyChanged pc = item as INotifyPropertyChanged;
			if (pc != null) {
				PropertyChangedWeakEventManager.RemoveListener(pc, this);
				PropertyChangedWeakEventManager.AddListener(pc, this);
			}
		}
		public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			PropertyChangedEventArgs propertyChangedEventArgs = e as PropertyChangedEventArgs;
			if (propertyChangedEventArgs != null && propertyChangedEventArgs.PropertyName == adapter.ValueDataMember)
				return managerType == typeof(PropertyChangedWeakEventManager) && ProcessWeakEvent(sender);
			return false;
		}
	}
}
