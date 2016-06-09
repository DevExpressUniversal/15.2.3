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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.ComponentModel;
namespace DevExpress.Web.Design {
	public class CollectionEditorForm : ItemsEditorFormBase {
		const int CollectionEditorItemsPanelMinimizeWidth = 153;
		private ListBoxItemPainter fListBoxItemPainter = new ListBoxItemPainter(true);
		ListBox itemsListBox;
		protected override int LeftPanelMinimizeWidth {
			get { return CollectionEditorItemsPanelMinimizeWidth; }
		}
		protected ListBox ItemsListBox { get { return itemsListBox; } }
		public CollectionEditorForm(object component, ITypeDescriptorContext context,
			IServiceProvider provider, object propertyValue)
			: base(component, context, provider, propertyValue) {
		}
		protected override void RemoveItem() {
			int index = ItemsListBox.SelectedIndex;
			List<CollectionItem> list = new List<CollectionItem>();
			foreach(object item in ItemsListBox.SelectedItems)
				list.Add((CollectionItem)item);			
			foreach(CollectionItem item in list) {
				Collection.Remove(item);
				ItemsListBox.Items.Remove(item);
			}
			if(Collection.Count == 0)
				PropertyGrid.SelectedObject = null;
			ItemsListBox.SelectedIndex = Math.Min(index, ItemsListBox.Items.Count - 1);			
			base.RemoveItem();
		}
		protected override void AssignControls() {
			FillListBox(ItemsListBox);
			if(ItemsListBox.Items.Count > 0)
				ItemsListBox.SelectedIndex = 0;
			base.AssignControls();
		}
		protected override void FocusItemViewer() {
			ItemsListBox.Focus();
		}
		protected override object CreateNewItem() {
			CollectionItem item = Collection.CreateKnownTypeItem(0);
			return item;
		}
		protected override System.Windows.Forms.Control CreateItemViewer() {
			this.itemsListBox = fListBoxItemPainter.CreateListBox(Font);
			ItemsListBox.ContextMenuStrip = PopupMenu;
			ItemsListBox.Dock = DockStyle.Fill;
			ItemsListBox.Margin = new Padding(0);
			ItemsListBox.SelectedIndexChanged += new EventHandler(OnSelectedIndexChanged);
			ItemsListBox.MouseDown += new MouseEventHandler(OnListBoxMouseDown);
			ItemsListBox.SelectionMode = SelectionMode.MultiExtended;
			return ItemsListBox;
		}
		protected override string GetPropertyStorePathPrefix() {
			return "CollectionEditorForm";
		}
		protected override void OnAddNewItem(object item) {
			Collection.Add(item as CollectionItem);
			ItemsListBox.Items.Add(item as CollectionItem);
			ItemsListBox.SelectedItems.Clear();
			ItemsListBox.SelectedItem = item;
			ItemsListBox.Update();
		}
		protected override void OnInsertItem(object item) {
			Collection.Insert(ItemsListBox.SelectedIndex, item as CollectionItem);
			ItemsListBox.Items.Insert(ItemsListBox.SelectedIndex, item as CollectionItem);
			ItemsListBox.SelectedItems.Clear();
			ItemsListBox.SelectedItem = item;
			ItemsListBox.Update();
		}
		protected override bool IsInsertButtonEnable() {
			return base.IsInsertButtonEnable() && ItemsListBox.SelectedIndex != -1;
		}
		protected override bool IsRemoveButtonEnable() {
			return base.IsRemoveButtonEnable() && ItemsListBox.SelectedIndex != -1;
		}
		protected override bool IsMoveDownButtonEnabled() {
			return (ItemsListBox.SelectedIndex < ItemsListBox.Items.Count - 1)
				&& (ItemsListBox.SelectedIndex != -1);
		}
		protected override bool IsMoveUpButtonEnabled() {
			return ItemsListBox.SelectedIndex > 0;
		}
		protected override IList GetParentViewerItemCollection() { return ItemsListBox.Items; }
		protected override void MoveViewerItem(int oldIndex, int newIndex) {
			IList collection = GetParentViewerItemCollection();
			object item = collection[oldIndex];
			collection.Remove(item);
			collection.Insert(newIndex, item);
		}
		protected override void MoveUpViewerItem() {
			int index = ItemsListBox.SelectedIndex;
			MoveViewerItem(index, index - 1);
			ItemsListBox.SelectedIndex = index - 1;
			ItemsListBox.Update();
		}
		protected override void MoveDownViewerItem() {
			int index = ItemsListBox.SelectedIndex;
			MoveViewerItem(index, index + 1);
			ItemsListBox.SelectedIndex = index + 1;
			ItemsListBox.Update();
		}
		protected override void MoveUpItem() {
			if(IsMoveUpButtonEnabled()) {
				int index = ItemsListBox.SelectedIndex;
				Collection.Move(index, index - 1);
				base.MoveUpItem();
			}
		}
		protected override void MoveDownItem() {
			if(IsMoveDownButtonEnabled()) {
				int index = ItemsListBox.SelectedIndex;
				Collection.Move(index, index + 1);
				base.MoveDownItem();
			}
		}
		protected override void RemoveAllItems() {
			Collection.Clear();
			ItemsListBox.Items.Clear();
		}
		protected override void SelectAll() {
			for(int i = 0; i < ItemsListBox.Items.Count; i++)
				ItemsListBox.SetSelected(i, true);
		}
		protected override void SetTabIndexes() {
			base.SetTabIndexes();
			ItemsListBox.TabStop = true;
			ItemsListBox.TabIndex = TabOrder[0];
		}
		protected override void SetVisiblePropertyInViewer() {
			ItemsListBox.Refresh();
		}
		protected object[] GetSelectedItems() {
			ArrayList objects = new ArrayList(ItemsListBox.SelectedItems);
			return objects.ToArray();
		}
		private void FillListBox(ListBox listBox) {
			listBox.Items.Clear();
			foreach(CollectionItem item in Collection) {
				listBox.Items.Add(item);
			}
		}
		private void OnSelectedIndexChanged(object sender, EventArgs e) {
			PropertyGrid.SelectedObjects = GetSelectedItems();
			UpdateToolStrip();
		}
		private void OnListBoxMouseDown(object sender, MouseEventArgs e) {
			if(e.Button == MouseButtons.Right) {
				if(ItemsListBox.Items.Count != 0) {
					int index = GetListBoxItemIndex(e.Y);
					ItemsListBox.SelectedItems.Clear();
					ItemsListBox.SelectedItem = index < ItemsListBox.Items.Count ? ItemsListBox.Items[index] : ItemsListBox.Items[ItemsListBox.Items.Count - 1];
					UpdateTools();
				}
			}
		}
		int GetListBoxItemIndex(int mouseDownY) {
			ScrollInfo info = GetListBoxVerticalScrollInfo();
			if(info.nPos == 0)
				return mouseDownY / ItemsListBox.ItemHeight;
			int scrollPositionHeight = ItemsListBox.PreferredHeight / info.nMax;
			return (mouseDownY + info.nPos * scrollPositionHeight) / ItemsListBox.ItemHeight;
		}
		#region API
		const uint SIF_POS = 0x1;
		const uint SIF_RANGE = 0x4;
		ScrollInfo GetListBoxVerticalScrollInfo() {
			ScrollInfo info = new ScrollInfo();
			info.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(info);
			info.fMask = SIF_POS | SIF_RANGE;
			GetScrollInfo(ItemsListBox.Handle, 1, ref info);
			return info;
		}
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		static extern bool GetScrollInfo(IntPtr hwnd, int fnBar, ref ScrollInfo scrollInfo);
		[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
		struct ScrollInfo {
			public int cbSize;
			public uint fMask;
			public int nMin;
			public int nMax;
			public uint nPage;
			public int nPos;
			public int nTrackPos;
		}
		#endregion
	}
}
