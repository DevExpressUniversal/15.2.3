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

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.ViewInfo;
using System.Diagnostics;
namespace DevExpress.XtraEditors {
	public class TreeComboBoxItem : ImageComboBoxItem {
		internal static bool HasItemChildren(ITreeSelectableItem item) {
			return item.Children != null && item.Children.Count > 0;
		}
		internal static int GetLevel(ITreeSelectableItem item) {
			int level = 0;
			ITreeSelectableItem parent = item.Parent;
			while(parent != null) {
				level++;
				parent = parent.Parent;
			}
			return level;
		}
		internal static string GetItemFullText(ITreeSelectableItem item, int rootLevel, int level) {
			string text = item.Text;
			ITreeSelectableItem parent = item.Parent;
			while (parent != null && level > rootLevel) {
				text = parent.Text + '.' + text;
				parent = parent.Parent;
				level--;
			}
			return text;
		}
		internal bool IsExpanded { get; set; }
		string drawText = null;
		int level = -1;
		const int SpaceByLevel = 4;
		int rootLevel;
		public TreeComboBoxItem(ITreeSelectableItem value, int rootLevel) : base(value) {
			this.rootLevel = rootLevel;
			Description = GetItemFullText(Item, RootLevel, Level);
		}
		public bool HasChildren { get { return HasItemChildren(Item); } }
		public ITreeSelectableItem Item { get { return (ITreeSelectableItem)Value; } }
		public int Level {
			get {
				if(level < 0) {
					level = GetLevel(Item);
				}
				return level;
			}
		}
		public string DrawText {
			get {
				if(string.IsNullOrEmpty(drawText)) {
					drawText = Item.Text;
				}
				return drawText;
			}
		}
		protected int RootLevel { get { return rootLevel; } }
		internal int AdditionalWidth { get; set; }
		public int ExpanderButtonWidth { get; set; }
	}
	[ToolboxItem(false)]
	public class TreeComboBoxEdit : ImageComboBoxEdit {
		List<ITreeSelectableItem> rootItems;
		int rootLevel = -1;
		private static readonly object popupListBoxSelectedIndexChanged = new object();
		public TreeComboBoxEdit() {
		}
		public override object SelectedItem {
			get { return base.SelectedItem; }
			set {
				if(SelectedItem == value) return;
				base.SelectedItem = value;
			}
		}
		internal delegate void OnCollapsedDelegate(TreeComboBoxItem item);
		internal event OnCollapsedDelegate OnCollapsed;
		void RaiseOnCollapsed(TreeComboBoxItem item) {
			if (OnCollapsed != null) {
				OnCollapsed(item);
			}
		}
		internal void CollapseItem(TreeComboBoxItem item) {
			RemoveChildren(item);
			item.IsExpanded = false;
			RaiseOnCollapsed(item);
		}
		internal void ExpandItem(TreeComboBoxItem item) {
			AddChildren(item);
			item.IsExpanded = true;
		}
		private void RemoveChildren(TreeComboBoxItem treeItem) {
			if (treeItem == null || !treeItem.HasChildren)
				return;
			PropertiesBeginUpdate();
			string address = treeItem.ToString();
			for (int i = Properties.Items.Count - 1; i >= 0; --i) {
				if (Properties.Items[i].ToString().StartsWith(address + ".") &&
					Properties.Items[i].ToString() != address)
					Properties.Items.RemoveAt(i);
			}
			PropertiesEndUpdate();
		}
		protected override void OnEditorKeyDown(KeyEventArgs e) {
			base.OnEditorKeyDown(e);
			switch (e.KeyCode) {
				case Keys.Right:
				case Keys.Add:
				case Keys.Decimal:
				case Keys.OemPeriod:
					ExpandItem(CurrentSelectedTreeItem);
					break;
				case Keys.Left:
				case Keys.Subtract:
					CollapseItem(CurrentSelectedTreeItem);
					break;
			}
		}
		public ITreeSelectableItem FocusedTreeItem {
			get {
				if(SelectedTreeItem == null) return null;
				ITreeSelectableItem item = SelectedTreeItem.Item;
				return GetFirstFocusableChildrenItem(item);
			}
			set {
				if(value == null) {
					SelectedItem = null;
					return;
				}
				AddChildrenByParents(GetItemParents(value));
				SelectedItem = FindItem(value);
			}
		}
		public void SetItemByDescription(string description) {
			if(string.IsNullOrEmpty(description)) return;
			string[] list = description.Split('.');
			int index = -1;
			string text = string.Empty;
			for(int i = 0; i < list.Length; i++) {
				text += list[i];
				index = FindItem(text, 0);
				if(i < list.Length - 1) {
					text += '.';
				}
				if(index < 0) break;
				ExpandItem(Properties.Items[index] as TreeComboBoxItem);
			}
			if(index > -1) {
				SelectedIndex = index;
			}
		}
		protected void AddChildrenByParents(List<ITreeSelectableItem> parents) {
			foreach(ITreeSelectableItem parent in parents) {
				TreeComboBoxItem item = FindItem(parent);
				if(item == null) break;
				ExpandItem(item);
			}
		}
		protected TreeComboBoxItem FindItem(ITreeSelectableItem treeItem) {
			string itemFullText = TreeComboBoxItem.GetItemFullText(treeItem, RootLevel, TreeComboBoxItem.GetLevel(treeItem));
			int index = FindItem(itemFullText, false, 0);
			if(index < 0) {
				index = FindItem(itemFullText, true, 0);
			}
			return index >= 0 ? Properties.Items[index] as TreeComboBoxItem : null;
		}
		protected override void FindUpdateEditValue(int itemIndex, bool jopened) {
			base.FindUpdateEditValue(itemIndex, jopened);
			if(itemIndex > -1) {
				ExpandItem(Properties.Items[itemIndex] as TreeComboBoxItem);
			}
		}
		protected TreeComboBoxItem SelectedTreeItem { get { return SelectedItem as TreeComboBoxItem; } }
		protected TreeComboBoxItem CurrentSelectedTreeItem {
			get {
				return CurrentSelectedIndex >= 0 ? Properties.Items[CurrentSelectedIndex] as TreeComboBoxItem : null;
			}
		}
		protected int CurrentSelectedIndex {
			get { return IsPopupOpen ? PopupForm.ListBox.SelectedIndex : SelectedIndex; }
			set {
				if(IsPopupOpen) {
					PopupForm.ListBox.SelectedIndex = value;
				} else {
					SelectedIndex = value;
				}
			}
		}
		public List<ITreeSelectableItem> RootItems {
			get { return rootItems; }
			set {
				if(RootItems == value) return;
				this.rootLevel = -1;
				rootItems = value;
				RebuildItems();
			}
		}
		public int RootLevel {
			get {
				if(rootLevel < 0) {
					ITreeSelectableItem item = RootItems.Count > 0 ? RootItems[0].Parent : null;
					this.rootLevel = 0;
					while(item != null) {
						item = item.Parent;
						this.rootLevel++;
					}
				}
				return rootLevel;
			}
		}
		List<ITreeSelectableItem> GetItemParents(ITreeSelectableItem item) {
			List<ITreeSelectableItem> parents = new List<ITreeSelectableItem>();
			while(item != null && item.Parent != null) {
				parents.Insert(0, item.Parent);
				item = item.Parent;
			}
			return parents;
		}
		ITreeSelectableItem GetFirstFocusableChildrenItem(ITreeSelectableItem item) {
			if(AllowSelectItem(item)) return item;
			foreach(ITreeSelectableItem childItem in item.Children) {
				if(AllowSelectItem(childItem)) return childItem;
			}
			return null;
		}
		bool AllowSelectItem(ITreeSelectableItem item) {
			return item != null && (item.AllowSelect || item.Children == null || item.Children.Count == 0);
		}
		protected internal override void ClosePopup(PopupCloseMode closeMode) {
			if (closeMode == PopupCloseMode.Cancel || closeMode == PopupCloseMode.Immediate) {
				PopupCloseMode newCloseMode = closeMode;
				if (IsPopupOpen) {
					newCloseMode = PopupCloseMode.Cancel;
				}
				base.ClosePopup(newCloseMode);
			} else {
				ITreeSelectableItem newItem = null;
				if (PopupForm != null && PopupForm.ListBox != null) {
					if (PopupForm.ListBox.SelectedIndex > -1) {
						TreeComboBoxItem item = Properties.Items[PopupForm.ListBox.SelectedIndex] as TreeComboBoxItem;
						if (item != null) {
							newItem = GetFirstFocusableChildrenItem(item.Item);
						}
					}
				}
				base.ClosePopup(closeMode);
				if (newItem != null) {
					FocusedTreeItem = newItem;
				}
			}
		}
		protected override DevExpress.XtraEditors.Popup.PopupBaseForm CreatePopupForm() {
			return new PopupFilterPropertiesComboBoxEditListBoxForm(this);
		}
		void AddChildren(TreeComboBoxItem treeItem) {
			if(treeItem == null || !treeItem.HasChildren) return;
			int index = Properties.Items.IndexOf(treeItem);
			if(index < 0) return;
			index++;
			TreeComboBoxItem nextTreeItem = index < Properties.Items.Count ? Properties.Items[index] as TreeComboBoxItem : null;
			if(nextTreeItem == null || nextTreeItem.Level <= treeItem.Level) {
				AddChildren(treeItem.Item.Children, index, false);
			}
		}
		void RebuildItems() {
			AddChildren(RootItems, 0, true);
		}
		void AddChildren(List<ITreeSelectableItem> items, int index, bool clearAll) {
			Debug.Assert(index <= Properties.Items.Count);
			PropertiesBeginUpdate();
			if(clearAll) {
				Properties.Items.Clear();
			}			
			if(items != null) {
				foreach(ITreeSelectableItem item in items) {
					Properties.Items.Insert(index, new TreeComboBoxItem(item, RootLevel));
					index++;
				}
			}
			PropertiesEndUpdate();
		}
		int CompareTreeItems(ITreeSelectableItem item1, ITreeSelectableItem item2) {
			return Comparer<string>.Default.Compare(item1.Text, item2.Text);
		}
		void PropertiesBeginUpdate() {
			Properties.BeginUpdate();
		}
		void PropertiesEndUpdate() {
			if(IsPopupOpen) {
				Properties.CancelUpdate();
				RefreshPopup();
			} else {
				Properties.EndUpdate();
			}
		}
	}
	[ToolboxItem(false)]
	internal class PopupFilterPropertiesComboBoxEditListBoxForm : PopupImageComboBoxEditListBoxForm {
		internal TreeComboBoxEdit ImageComboBoxEdit { get; private set; }
		public PopupFilterPropertiesComboBoxEditListBoxForm(TreeComboBoxEdit owner)
			: base(owner) {
			ImageComboBoxEdit = owner;
		}
		protected override PopupListBox CreateListBox() {			
			return new PopupFilterPropertiesComboBoxEditListBox(this);
		}
	}
	[ToolboxItem(false)]
	internal class PopupFilterPropertiesComboBoxEditListBox : PopupListBox {		
		PopupFilterPropertiesComboBoxEditListBoxForm ownerForm;
		Size expanderSize;
		public PopupFilterPropertiesComboBoxEditListBox(PopupFilterPropertiesComboBoxEditListBoxForm ownerForm)
			: base(ownerForm) {
			this.ownerForm = ownerForm;
		}
		protected int RootLevel { get { return ((TreeComboBoxEdit)OwnerEdit).RootLevel; } }
		IEnumerable<BaseListBoxViewInfo.ItemInfo> VisibleLines {
			get {
				foreach(object obj in ViewInfo.VisibleItems) {
					if(obj is BaseListBoxViewInfo.ItemInfo &&
						((BaseListBoxViewInfo.ItemInfo)obj).Item is TreeComboBoxItem)
						yield return (BaseListBoxViewInfo.ItemInfo)obj;
				}
			}
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			if (e.Button != MouseButtons.Left)
				return;
			foreach(BaseListBoxViewInfo.ItemInfo itemInfo in VisibleLines) {
				if(itemInfo.Bounds.Contains(e.X, e.Y)) {
					var item = (TreeComboBoxItem)itemInfo.Item;
					if(!item.HasChildren || item.ExpanderButtonWidth < e.X)
						continue;
					if(item.IsExpanded) {
						ownerForm.ImageComboBoxEdit.CollapseItem(item);
					} else {
						ownerForm.ImageComboBoxEdit.ExpandItem(item);
					}
					return;
				}
			}
			base.OnMouseUp(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if(ViewInfo.HotItemIndex > -1) {
				SelectedIndex = ViewInfo.HotItemIndex;
				RaiseSelectedIndexChanged();
			}
		}
		protected internal override string GetDrawItemText(int index) {
			TreeComboBoxItem item = Properties.Items[index] as TreeComboBoxItem;
			return item != null ? item.DrawText : base.GetDrawItemText(index);
		}
		protected override string GetItemText(object item) {
			return ((TreeComboBoxItem)item).DrawText;
		}
		public override int CalcItemWidth(GraphicsInfo gInfo, object item) {
			expanderSize = CalcExpanderSize(); 
			return base.CalcItemWidth(gInfo, item) + CalcAdditionalWidth(item as TreeComboBoxItem);
		}		
		protected internal override void RaiseDrawItem(ListBoxDrawItemEventArgs e) {			
			TreeComboBoxItem item = e.Item as TreeComboBoxItem;
			if (item != null && !item.Item.AllowSelect) {
				e.Appearance.ForeColor = Color.Gray;
			}
			BaseListBoxViewInfo viewInfo = e.ViewInfo;
			viewInfo.ListBoxItemInfoArgs.Cache = e.Cache;
			viewInfo.ListBoxItemInfoArgs.AssignFromItemInfo((BaseListBoxViewInfo.ItemInfo)e.GetItemInfo());
			try {
				Rectangle r = viewInfo.ListBoxItemInfoArgs.TextRect;
				int additionalWidth = CalcAdditionalWidth(item);
				r.X += additionalWidth;
				viewInfo.ListBoxItemInfoArgs.TextRect = r;
				viewInfo.ListBoxItemPainter.DrawObject(viewInfo.ListBoxItemInfoArgs);
				if (item.HasChildren) {
					int x = additionalWidth - expanderSize.Width;
					int y = r.Y +(r.Height - expanderSize.Height) / 2;
					Size extendedExpanderSize = (Size)expanderSize;
					Rectangle expanderRectangle = new Rectangle(new Point(x, y), (Size)expanderSize);					
					OpenCloseButtonInfoArgs infoArgs = new OpenCloseButtonInfoArgs(e.Cache, expanderRectangle, item.IsExpanded, e.Appearance, ObjectState.Normal);
					LookAndFeelPainterHelper.GetPainter(LookAndFeel.ActiveStyle).OpenCloseButton.DrawObject(infoArgs);					
					item.ExpanderButtonWidth = expanderRectangle.Width + x;
					if (!item.Item.AllowSelect)
						item.ExpanderButtonWidth = int.MaxValue;
				}
			}
			finally {				
				viewInfo.ListBoxItemInfoArgs.Cache = null;
				viewInfo.ListBoxItemInfoArgs.PaintAppearance = null;
			}
			e.Handled = true;
		}
		private Size CalcExpanderSize() {
			OpenCloseButtonInfoArgs infoArgs = new OpenCloseButtonInfoArgs(null);
			return LookAndFeelPainterHelper.GetPainter(LookAndFeel.ActiveStyle).OpenCloseButton.CalcObjectMinBounds(infoArgs).Size;
		}
		private int CalcAdditionalWidth(TreeComboBoxItem item) {
			int levelWidth = expanderSize.Width;
			int additionalWidth = 0;
			additionalWidth += levelWidth * item.Level;
			bool shouldHaveLeftPadding = false;
			foreach (TreeComboBoxItem child in Properties.Items) {
				if (child.Level == 0 && child.HasChildren) {
					shouldHaveLeftPadding = true;
					break;
				}
			}
			if (shouldHaveLeftPadding)
				additionalWidth += levelWidth;
			return additionalWidth;
		}
	}
}
