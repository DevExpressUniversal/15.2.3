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

using DevExpress.Utils.Text;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraBars.Navigation {
	public partial class NavigationOptionsForm : XtraForm {
		public NavigationOptionsForm() {
			InitializeComponent();
			this.ceCompact.Visible = false;
			this.KeyPreview = true;
			spinNumber.Location = new Point(labelControl1.Bounds.Right + 3, spinNumber.Location.Y);
			PrepareForTouchUI();
		}
		void PrepareForTouchUI() {
			if(LookAndFeel.GetTouchUI()) {
				Scale(new SizeF(1.4f + LookAndFeel.GetTouchScaleFactor() / 10.0f, 1.4f + LookAndFeel.GetTouchScaleFactor() / 10.0f));
			}
		}
		public List<NavigationBarItem> ResultItems { get { return GetItems(); } }
		List<NavigationBarItem> GetItems() {
			List<NavigationBarItem> res = new List<NavigationBarItem>();
			foreach(ItemInfo iinfo in list.Items)
				res.Add(iinfo.Item);
			return res;
		}
		public bool ResultIsCompact { get { return ceCompact.Checked; } }
		public int ResultVisibleCount { get { return (int)spinNumber.Value; } }
		OfficeNavigationBar offnavbar;
		public OfficeNavigationBar OfficeNavigationBar {
			get { return offnavbar; }
			set {
				if(offnavbar == value) return;
				offnavbar = value;
				OnOwnerChanged();
			}
		}
		private void OnOwnerChanged() {
			if(OfficeNavigationBar == null) return;
			Init();
		}
		protected override void OnShown(EventArgs e) {
			ClearList();
			Init();
		}
		private void ClearList() {
			list.Items.Clear();
		}
		void Init() {
			spinNumber.Properties.MaxValue = OfficeNavigationBar.Items.Count;
			spinNumber.Value = OfficeNavigationBar.MaxItemCount > -1 ? OfficeNavigationBar.MaxItemCount : spinNumber.Properties.MaxValue;
			PopulateItems();
		}
		void PopulateItems() {
			foreach(NavigationBarItem item in OfficeNavigationBar.Items)
				list.Items.Add(new ItemInfo() { Item = item });
		}
		class ItemInfo {
			public NavigationBarItem Item { get; set; }
			public string Text {
				get { return GetItemText(Item); }
			}
			string GetItemText(NavigationBarItem item) {
				if(!string.IsNullOrEmpty(item.CustomizationText))
					return item.CustomizationText;
				if(string.IsNullOrEmpty(item.Text))
					return string.IsNullOrEmpty(Item.Name) ? "<empty>" : Item.Name;
				string value = item.Text;
				if(Item.Collection.Owner.AllowHtmlDraw) {
					string result = string.Empty;
					var blocks = StringParser.Parse(1f, value, true);
					foreach(var s in blocks)
						result += s.Text;
					value = result;
				}
				return value;
			}
			public override string ToString() { return Text; }
		}
		private void btnUp_Click(object sender, EventArgs e) {
			MoveItem(true);
		}
		private void btnDown_Click(object sender, EventArgs e) {
			MoveItem(false);
		}
		void MoveItem(bool moveUp) {
			if(list.SelectedItem == null) return;
			var itemInfo = list.SelectedItem as ItemInfo;
			int index = list.SelectedIndex;
			list.Items.Remove(itemInfo);
			index = moveUp ? index - 1 : index + 1;
			index = Math.Max(0, Math.Min(list.Items.Count, index));
			list.Items.Insert(index, itemInfo);
			list.SelectedIndex = index;
		}
		private void btnOk_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.OK;
		}
		private void btnCancel_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.Cancel;
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			if(e.KeyCode == Keys.Escape)
				DialogResult = DialogResult.Cancel;
		}
	}
}
