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

using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Core.Serialization;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
namespace DevExpress.Xpf.Core {
	public static class DXTabControlRestoreLayoutOptions {
		public static readonly DependencyProperty AddNewTabsProperty = DependencyProperty.RegisterAttached("AddNewTabs", typeof(bool), typeof(DXTabControlRestoreLayoutOptions), new PropertyMetadata(true));
		public static readonly DependencyProperty RemoveOldTabsProperty = DependencyProperty.RegisterAttached("RemoveOldTabs", typeof(bool), typeof(DXTabControlRestoreLayoutOptions), new PropertyMetadata(true));
		public static bool GetAddNewTabs(DXTabControl obj) { return (bool)obj.GetValue(AddNewTabsProperty); }
		public static void SetAddNewTabs(DXTabControl obj, bool value) { obj.SetValue(AddNewTabsProperty, value); }
		public static bool GetRemoveOldTabs(DXTabControl obj) { return (bool)obj.GetValue(RemoveOldTabsProperty); }
		public static void SetRemoveOldTabs(DXTabControl obj, bool value) { obj.SetValue(RemoveOldTabsProperty, value); }
	}
}
namespace DevExpress.Xpf.Core.Native {
	public class DXTabSerializationInfo {
		[XtraSerializableProperty]
		public string Name { get; set; }
		[XtraSerializableProperty]
		public string Header { get; set; }
		[XtraSerializableProperty]
		public string Content { get; set; }
		[XtraSerializableProperty]
		public bool IsNew { get; set; }
		[XtraSerializableProperty]
		public Visibility Visibility { get; set; }
		[XtraSerializableProperty]
		public int Index { get; set; }
		[XtraSerializableProperty]
		public TabPinMode PinMode { get; set; }
		internal DXTabItem Tab { get; private set; }
		public void Assign(DXTabItem tabItem) {
			Tab = tabItem;
		}
		public void Unassign() {
			Tab = null;
		}
		public void Apply() {
			if(Tab == null) return;
			Tab.Name = Name;
			Tab.IsNew = IsNew;
			Tab.Visibility = Visibility;
			if(!string.IsNullOrEmpty(Header))
				Tab.SetCurrentValue(DXTabItem.HeaderProperty, Header);
			if(!string.IsNullOrEmpty(Content))
				Tab.SetCurrentValue(DXTabItem.ContentProperty, Content);
			Tab.SetCurrentValue(TabControlStretchView.PinModeProperty, PinMode);
		}
		public static DXTabSerializationInfo Create(DXTabItem tabItem, int index) {
			DXTabSerializationInfo res = new DXTabSerializationInfo();
			res.Index = tabItem.VisibleIndex >= int.MaxValue || tabItem.VisibleIndex < 0 ? index : tabItem.VisibleIndex;
			res.Name = tabItem.Name;
			if(tabItem.Header is string)
				res.Header = (string)tabItem.Header;
			if(tabItem.Content is string)
				res.Content = (string)tabItem.Content;
			res.IsNew = tabItem.IsNew;
			res.Visibility = tabItem.Visibility;
			res.PinMode = TabControlStretchView.GetPinMode(tabItem);
			return res;
		}
	}
	public class DXTabControlSerializationInfo {
		[XtraSerializableProperty]
		public int SelectedIndex { get; set; }
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false)]
		public List<DXTabSerializationInfo> Infos { get; private set; }
		public DXTabControlSerializationInfo() {
			Infos = new List<DXTabSerializationInfo>();
		}
		public void Init(DXTabControl tabControl) {
			Clear();
			SelectedIndex = tabControl.SelectedIndex;
			for(int i = 0; i < tabControl.Items.Count; i++) {
				var tabItem = tabControl.GetTabItem(i);
				Infos.Add(DXTabSerializationInfo.Create(tabItem, i));
			}
		}
		public void Clear() {
			SelectedIndex = -1;
			Infos.Clear();
		}
		public void Apply(DXTabControl tabControl) {
			tabControl.SetCurrentValue(DXTabControl.SelectedIndexProperty, -1);
			AssignInfos(tabControl);
			ReorderExistingTabs(tabControl);
			if(DXTabControlRestoreLayoutOptions.GetRemoveOldTabs(tabControl))
				RemoveOldTabs(tabControl);
			if(DXTabControlRestoreLayoutOptions.GetAddNewTabs(tabControl))
				AddNewTabs(tabControl);
			ReorderExistingTabs(tabControl);
			ApplyAndUnassignInfos();
			tabControl.SetCurrentValue(DXTabControl.SelectedIndexProperty, SelectedIndex);
		}
		void AssignInfos(DXTabControl tabControl) {
			for(int i = 0; i < tabControl.Items.Count; i++) {
				var tab = tabControl.GetTabItem(i);
				if(tab == null) continue;
				var info = Infos.Find(x => x.Name == tab.Name);
				info.Do(x => x.Assign(tab));
			}
		}
		void ReorderExistingTabs(DXTabControl tabControl) {
			foreach(var info in Infos) {
				if(info.Tab == null) continue;
				MoveTab(tabControl, info.Tab, info.Index);
			}
		}
		void RemoveOldTabs(DXTabControl tabControl) {
			tabControl.ForEachTabItem(tab => {
				if(Infos.Find(info => info.Name == tab.Name) == null)
					tabControl.RemoveTabItem(tab);
			});
		}
		void AddNewTabs(DXTabControl tabControl) {
			foreach(var info in Infos) {
				if(info.Tab != null) continue;
				tabControl.AddNewTabItem();
				var tab = tabControl.GetTabItem(tabControl.Items.Count - 1);
				info.Assign(tab);
				MoveTab(tabControl, info.Tab, info.Index);
			}
		}
		void ApplyAndUnassignInfos() {
			foreach(var info in Infos) {
				info.Apply();
				info.Unassign();
			}
		}
		void MoveTab(DXTabControl tabControl, DXTabItem tab, int index) {
			if(tabControl.IndexOf(tab) == index) return;
			if(index >= tabControl.Items.Count)
				index = tabControl.Items.Count - 1;
			tab.Move(index);
		}
	}
	public class DXTabControlSerializationProvider : SerializationProvider {
		protected internal override object OnCreateCollectionItem(XtraCreateCollectionItemEventArgs e) {
			DXTabControlSerializationInfo dxTabControlSerializationInfo = null;
			if(e.CollectionName != BindableBase.GetPropertyName(() => dxTabControlSerializationInfo.Infos))
				return null;
			var item = new DXTabSerializationInfo();
			((IList)e.Collection).Add(item);
			e.CollectionItem = item;
			return item;
		}
		protected internal override void OnStartSerializing(DependencyObject dObj) {
			base.OnStartSerializing(dObj);
			DXTabControl tabControl = dObj as DXTabControl;
			if(tabControl == null) return;
			tabControl.SerializationInfo.Init(tabControl);
		}
		protected internal override void OnEndSerializing(DependencyObject dObj) {
			base.OnEndSerializing(dObj);
			DXTabControl tabControl = dObj as DXTabControl;
			if(tabControl == null) return;
			tabControl.SerializationInfo.Clear();
		}
		protected internal override void OnStartDeserializing(DependencyObject dObj, LayoutAllowEventArgs ea) {
			base.OnStartDeserializing(dObj, ea);
			DXTabControl tabControl = dObj as DXTabControl;
			if(tabControl == null) return;
			tabControl.SerializationInfo.Clear();
		}
		protected internal override void OnEndDeserializing(DependencyObject dObj, string restoredVersion) {
			base.OnEndDeserializing(dObj, restoredVersion);
			DXTabControl tabControl = dObj as DXTabControl;
			if(tabControl == null) return;
			tabControl.SerializationInfo.Apply(tabControl);
			tabControl.SerializationInfo.Clear();
		}
	}
}
