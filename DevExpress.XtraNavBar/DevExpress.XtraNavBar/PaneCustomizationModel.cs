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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System.Collections;
namespace DevExpress.XtraNavBar {
	public class PaneCustomizationHelper {
		public virtual void Customize(NavBarControl navBar, NavBarCustomizationInfo info) {
			Restore(navBar, info);
		}
		public virtual Font GetGroupFont(NavBarGroup group) {
			if(group == null || group.ItemLinks.Count < 1)
				return null;
			return group.ItemLinks[0].Item.Appearance.Font;
		}
		public virtual NavBarCustomizationInfo Fill(NavBarControl navBar) {
			NavBarCustomizationInfo res = new NavBarCustomizationInfo();
			foreach(NavBarGroup group in navBar.Groups) {
				NavBarGroupCustomizationInfo si = new NavBarGroupCustomizationInfo(navBar.Groups.IndexOf(group), group.IsVisible);
				si.Font = GetGroupFont(group);
				res.Add(group, si);
			}
			res.IsInitialized = true;
			return res;
		}
		public virtual NavBarCustomizationInfo Fill(CheckedListBoxControl listbox) {
			NavBarCustomizationInfo res = new NavBarCustomizationInfo();
			for(int i = 0; i < listbox.Items.Count; i++) {
				GroupListBoxItemInfo listItem = (GroupListBoxItemInfo)listbox.Items[i];
				NavBarGroupCustomizationInfo si = new NavBarGroupCustomizationInfo(i, listItem.CheckState == CheckState.Checked);
				si.Font = listItem.Font;
				res.Add(listItem.Group, si);
			}
			return res;
		}
		public virtual void Restore(CheckedListBoxControl listbox, NavBarCustomizationInfo info) {
			if(!info.IsInitialized)
				return;
			listbox.Items.Clear();
			listbox.BeginUpdate();
			try {
				foreach(KeyValuePair<NavBarGroup, NavBarGroupCustomizationInfo> pair in info) {
					GroupListBoxItemInfo item = new GroupListBoxItemInfo(pair.Key);
					item.CheckState = pair.Value.IsVisible ? CheckState.Checked : CheckState.Unchecked;
					item.Font = pair.Value.Font;
					listbox.Items.Add(item);
				}
				if(listbox.Items.Count > 0) listbox.SelectedIndex = 0;
			}
			finally {
				listbox.EndUpdate();
			}
		}
		public virtual void Restore(NavBarControl navBar, NavBarCustomizationInfo info) {
			int resPos = 0;
			navBar.BeginUpdate();
			try {
				foreach(KeyValuePair<NavBarGroup, NavBarGroupCustomizationInfo> pair in info) {
					int srcPos = navBar.Groups.IndexOf(pair.Key);
					navBar.Groups.Move(srcPos, resPos++);
					pair.Key.Visible = pair.Value.IsVisible;
					ApplyGroupFont(navBar, pair.Key, pair.Value.Font);
				}
			}
			finally {
				navBar.EndUpdate();
			}
		}
		protected virtual void ApplyGroupFont(NavBarControl navBar, NavBarGroup group, Font font) {
			if(font == null) return;
			foreach(NavBarItemLink link in group.ItemLinks)
				link.Item.Appearance.Font = link.Item.AppearanceDisabled.Font = link.Item.AppearanceHotTracked.Font = link.Item.AppearancePressed.Font = font;
			RaisePaneOptionsApplyGroupFont(navBar, group, font);
		}
		protected virtual void RaisePaneOptionsApplyGroupFont(NavBarControl navBar, NavBarGroup group, Font font) {
			NavPaneOptionsApplyGroupFontEventArgs e = new NavPaneOptionsApplyGroupFontEventArgs(group, font);
			navBar.RaiseNavPaneOptionsApplyGroupFont(e);
		}
	}
	public class NavBarCustomizationInfo : IEnumerable<KeyValuePair<NavBarGroup, NavBarGroupCustomizationInfo>>, IDisposable {
		Dictionary<NavBarGroup, NavBarGroupCustomizationInfo> table;
		public NavBarCustomizationInfo() {
			this.IsInitialized = false;
			this.table = new Dictionary<NavBarGroup, NavBarGroupCustomizationInfo>();
		}
		~NavBarCustomizationInfo() {
			Dispose(false);
		}
		public void Add(NavBarGroup group, NavBarGroupCustomizationInfo groupInfo) {
			table.Add(group, groupInfo);
		}
		public bool IsInitialized { get; set; }
		protected virtual void Dispose(bool disposing) {
			table.Clear();
		}
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		#region Equals & GetHashCode
		public override bool Equals(object obj) {
			NavBarCustomizationInfo sample = obj as NavBarCustomizationInfo;
			if(sample == null)
				return false;
			if(table.Count != sample.table.Count)
				return false;
			foreach(KeyValuePair<NavBarGroup, NavBarGroupCustomizationInfo> pair in table) {
				NavBarGroupCustomizationInfo secondValue;
				if(!sample.table.TryGetValue(pair.Key, out secondValue)) return false;
				if(!pair.Value.Equals(secondValue)) return false;
			}
			return true;
		}
		public override int GetHashCode() {
			return table.GetHashCode();
		}
		#endregion
		#region IEnumerable<TKey, TValue>
		public IEnumerator<KeyValuePair<NavBarGroup, NavBarGroupCustomizationInfo>> GetEnumerator() {
			return table.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		#endregion
	}
	public class NavBarGroupCustomizationInfo {
		public NavBarGroupCustomizationInfo(int groupPos, bool isVisible) {
			this.Pos = groupPos;
			this.Font = null;
			this.IsVisible = isVisible;
		}
		public int Pos { get; set; }
		public Font Font { get; set; }
		public bool IsVisible { get; set; }
		#region Equals & GetHashCode
		public override bool Equals(object obj) {
			NavBarGroupCustomizationInfo sample = obj as NavBarGroupCustomizationInfo;
			if(sample == null) return false;
			if(Pos != sample.Pos || IsVisible != sample.IsVisible)
				return false;
			return Font != null ? Font.Equals(sample.Font) : sample.Font == null;
		}
		public override int GetHashCode() {
			return Pos.GetHashCode() ^ Font.GetHashCode() ^ IsVisible.GetHashCode();
		}
		#endregion
	}
	public class GroupListBoxItemInfo : CheckedListBoxItem {
		public GroupListBoxItemInfo(NavBarGroup group) : this(group, null) { }
		public GroupListBoxItemInfo(NavBarGroup group, Font font) {
			this.Group = group;
			this.Font = font;
		}
		public override string ToString() {
			return Group.Caption;
		}
		public Font Font { get; set; }
		public NavBarGroup Group { get; private set; }
	}
}
