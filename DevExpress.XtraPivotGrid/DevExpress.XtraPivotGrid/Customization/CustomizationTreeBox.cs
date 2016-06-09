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
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Customization;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraPivotGrid.Customization.ViewInfo;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.XtraPivotGrid.Customization {
	public abstract class CustomizationTreeBox : CustomizationListBoxBase {
		readonly Dictionary<IVisualCustomizationTreeItem, CustomizationItemViewInfo> viewInfoCache = new Dictionary<IVisualCustomizationTreeItem, CustomizationItemViewInfo>();
		readonly ICustomizationFormOwner controlOwner;
		readonly CustomizationForm form;
		int itemHeight;
		protected CustomizationTreeBox(CustomizationForm form)
			: base() {
			this.form = form;
			this.controlOwner = form.ControlOwner as ICustomizationFormOwner;
			Initialize();
		}
		protected CustomizationTreeBox(ICustomizationFormOwner controlOwner)
			: base() {
			this.form = null;
			this.controlOwner = controlOwner;
			Initialize();
		}
		void Initialize() {
			SetupTooltips();
			HotTrackItems = true;
			HotTrackSelectMode = HotTrackSelectMode.SelectItemOnClick;
			HighlightedItemStyle = HighlightStyle.Skinned;
			Appearance.TextOptions.Trimming = Trimming.EllipsisCharacter;
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) 
				UnsetTooltips();
		}
		public CustomizationForm CustomizationForm { get { return form; } }
		protected ICustomizationFormOwner ControlOwner { get { return controlOwner; } }
		public new IVisualCustomizationTreeItem SelectedItem {
			get { return (IVisualCustomizationTreeItem)base.SelectedItem; }
		}
		#region Access to ListItemPainter for viewinfos
		internal new BaseListBoxViewInfo ViewInfo { get { return base.ViewInfo; } }
		internal BaseListBoxItemPainter ListItemPainter { get { return ((ListBoxViewInfoAccess)ViewInfo).ItemPainter; } }
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new ListBoxViewInfoAccess(this);
		}
		#endregion
		protected override Point PointToView(Point p) {
			if(ControlOwner != null)
				return ((Control)ControlOwner).PointToClient(PointToScreen(p));
			return p;
		}
		public override int GetItemHeight() {
			if(itemHeight == 0)
				itemHeight = CalcItemHeight();
			return itemHeight; 
		}
		public new IVisualCustomizationTreeItem GetItem(int index) {
			return (IVisualCustomizationTreeItem)base.GetItem(index);
		}
		public IVisualCustomizationTreeItem GetItem(Point location) {
			int index = IndexFromPoint(location);
			return GetItem(index);
		}
		protected override void DrawItemObject(GraphicsCache cache, int index, Rectangle bounds, DrawItemState itemState) {
			CustomizationItemViewInfo itemViewInfo = GetItemViewInfo(index);
			IVisualCustomizationTreeItem node = itemViewInfo.Item;
			bounds.X++;
			bounds.Width -= 2;
			bounds.Y++;
			bounds.Height--;
			bool hotTrack = (itemState & DrawItemState.HotLight) == DrawItemState.HotLight,
				selected = node == PressedItem || (itemState & DrawItemState.Selected) == DrawItemState.Selected;
			itemViewInfo.Paint(cache, bounds, hotTrack, selected, Focused);
		}
		protected override void OnStyleChanged(EventArgs e) {
			base.OnStyleChanged(e);
			UpdateItemHeight();
		}
		void UpdateItemHeight() {
			itemHeight = CalcItemHeight();
			if(ItemHeight != GetItemHeight())
				ItemHeight = GetItemHeight();
		}
		protected virtual int CalcItemHeight() {
			CustomizationTreeItemViewInfo itemViewInfo = new CustomizationTreeItemViewInfo(this, new CalcCustomizationItem());
			return itemViewInfo.CalcHeight() + 6;
		}
		protected internal virtual void ChangeExpanded(IVisualCustomizationTreeItem item) {
			ChangeExpanded(item, !item.IsExpanded);
		}
		protected internal virtual void ChangeExpanded(IVisualCustomizationTreeItem item, bool expanded) {
			if(item == null)
				return;
			item.IsExpanded = expanded;
			Populate();
		}
		protected override void OnMouseClick(MouseEventArgs e) {
			base.OnMouseClick(e);
			if(e.Button != MouseButtons.Left) return;
			CustomizationItemViewInfo itemViewInfo = GetItemViewInfo(e.Location);
			Rectangle bounds = GetItemRectangle(IndexFromPoint(e.Location));
			if(itemViewInfo != null && itemViewInfo.IsOpenCloseButton(bounds, e.Location)) {
				ChangeExpanded(itemViewInfo.Item);
			}
		}
		protected override void OnMouseDoubleClick(MouseEventArgs e) {
			base.OnMouseDoubleClick(e);
			if(e.Button != MouseButtons.Left) return;
			IVisualCustomizationTreeItem item = GetItem(e.Location);
			if(item != null)
				OnNodeDoubleClick(item);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			CustomizationItemViewInfo itemViewInfo = GetItemViewInfo(e.Location);
			if(itemViewInfo != null)
				itemViewInfo.MouseDown(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			CustomizationItemViewInfo itemViewInfo = GetItemViewInfo(e.Location);
			if(itemViewInfo != null)
				itemViewInfo.MouseUp(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			CustomizationItemViewInfo itemViewInfo = GetItemViewInfo(e.Location);
			if(itemViewInfo != null)
				itemViewInfo.MouseMove(e);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			switch(e.KeyCode) {
				case Keys.Right:
					OnRightArrowKeyDown(SelectedItem);
					return;
			}
			base.OnKeyDown(e);
		}
		protected virtual void OnNodeDoubleClick(IVisualCustomizationTreeItem item) {
			ChangeExpanded(item);
		}
		protected virtual void OnRightArrowKeyDown(IVisualCustomizationTreeItem item) {
			if(item == null) {
				if(Items.Count > 0)
					SelectedIndex = 0;
				return;
			}
			if(item.CanExpand && !item.IsExpanded) {
				ChangeExpanded(item, true);
				return;
			}
			if(item.CanExpand && item.IsExpanded)
				SelectedIndex++;
		}
		protected virtual void SetupTooltips() {
			if(ControlOwner == null) return;
			ToolTipController toolTipController = ControlOwner.ToolTipController;
			if(toolTipController != null)
				toolTipController.AddClientControl(this);
			else
				ToolTipController.DefaultController.AddClientControl(this);
		}
		protected virtual void UnsetTooltips() {
			if(ControlOwner == null) return;
			ToolTipController toolTipController = ControlOwner.ToolTipController;
			if(toolTipController != null)
				toolTipController.RemoveClientControl(this);
			else
				ToolTipController.DefaultController.RemoveClientControl(this);
		}
		protected override ToolTipControlInfo GetToolTipInfo(Point point) {
			CustomizationItemViewInfo itemViewInfo = GetItemViewInfo(point);
			if(itemViewInfo == null) return null;
			return itemViewInfo.GetToolTipObjectInfo();
		}
		public override void Populate() {
			UpdateItemHeight();
			int oldSelectedIndex = SelectedIndex;
			int oldTopIndex = TopIndex;
			BeginUpdate();
			try {				
				Items.Clear();
				this.viewInfoCache.Clear();
				PopulateCore();
			} finally {
				EndUpdate();
			}
			if(oldSelectedIndex >= ItemCount)
				oldSelectedIndex = ItemCount - 1;
			if(oldSelectedIndex > -1)
				SelectedIndex = oldSelectedIndex;
			if(oldTopIndex > -1)
				TopIndex = oldTopIndex;
		}
		protected abstract void PopulateCore();
		protected CustomizationItemViewInfo GetItemViewInfo(Point location) {
			int index = IndexFromPoint(location);
			return GetItemViewInfo(index);
		}
		protected CustomizationItemViewInfo GetItemViewInfo(int index) {
			IVisualCustomizationTreeItem item = GetItem(index);
			if(item == null)
				return null;
			CustomizationItemViewInfo res;
			if(!viewInfoCache.TryGetValue(item, out res)) {
				res = CreateItemViewInfo(item);
				viewInfoCache.Add(item, res);
			}
			return res;
		}
		protected virtual CustomizationItemViewInfo CreateItemViewInfo(IVisualCustomizationTreeItem item) {
			return new CustomizationTreeItemViewInfo(this, item);
		}
		protected override void WndProc(ref Message m) {
			base.WndProc(ref m);
			CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
		}
		#region CustomizationListBoxViewInfo
		class ListBoxViewInfoAccess : BaseListBoxViewInfo {
			public ListBoxViewInfoAccess(BaseListBoxControl owner)
				: base(owner) { }
			public BaseListBoxItemPainter ItemPainter {
				get { return base.ListBoxItemPainter; }
			}
		}
		#endregion
		#region class CalcCustomizationItem
		class CalcCustomizationItem : IVisualCustomizationTreeItem {
			public ICustomizationTreeItem Parent { get { return null; } }
			public virtual bool CanExpand { get { return false; } }
			public string Name { get { return string.Empty; } }
			public string Caption { get { return "Qq"; } }
			public virtual bool IsExpanded { get { return false; } set { } }
			public virtual bool IsVisible { get { return true; } }
			public virtual int Level { get { return 0; } }
			public int ImageIndex { get { return 0; } }
			public PivotFieldItemBase Field { get { return null; } }
			public IEnumerable<ICustomizationTreeItem> EnumerateChildren() { return null; }
#pragma warning disable 67
			public event EventHandler<TreeNodeExpandedChangedEventArgs> ExpandedChanged;
#pragma warning restore 67
			public ICustomizationTreeItem NextSibling { get { return null; } }
			public ICustomizationTreeItem FirstChild { get { return null; } }
		}
		#endregion
	}
	public interface ICustomizationFormOwner {
		ToolTipController ToolTipController { get; }
	}
}
