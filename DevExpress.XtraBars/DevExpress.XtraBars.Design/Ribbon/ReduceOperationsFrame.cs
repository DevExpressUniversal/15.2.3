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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils.Frames;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraBars.Ribbon.Internal;
using DevExpress.XtraBars.Ribbon.Helpers;
using System.Reflection;
namespace DevExpress.XtraBars.Ribbon.Design {
	public partial class ReduceOperationsFrame : DevExpress.XtraEditors.Designer.Utils.XtraFrame {
		public ReduceOperationsFrame() {
			InitializeComponent();
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			this.bAdd.Tag = ReduceOperationType.LargeButtons;
			barAndDockingController1.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			InitializePreviewRibbon();
			InitializeReduceOperationMenu();
			InitializeBehaviorCombo();
			InitializePagesCombo();
			UpdateButtonsState();
		}
		protected virtual void InitializePagesCombo() {
			this.cbPages.Properties.BeginUpdate();
			try {
				this.cbPages.Properties.Items.Clear();
				foreach(RibbonPage page in previewRibbon.TotalPageCategory.Pages) {
					this.cbPages.Properties.Items.Add(page);
				}
			}
			finally {
				this.cbPages.Properties.EndUpdate();
			}
			if(this.cbPages.Properties.Items.Count > 0)
				this.cbPages.SelectedIndex = 0;
		}
		PreviewRibbon previewRibbon;
		protected virtual void InitializePreviewRibbon() {
			this.previewRibbon = new PreviewRibbon(Ribbon.Manager, Ribbon);
			this.previewRibbon.Dock = System.Windows.Forms.DockStyle.None;
			this.previewRibbon.DrawGroupsBorder = true;
			this.previewRibbon.ExpandCollapseItem.Id = 0;
			this.previewRibbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
			this.previewRibbon.ExpandCollapseItem});
			this.previewRibbon.Location = new System.Drawing.Point(0, 0);
			this.previewRibbon.MaxItemId = 1;
			this.previewRibbon.Name = "previewRibbon";
			this.previewRibbon.Size = new System.Drawing.Size(427, 47);
			this.previewRibbon.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Above;
			this.previewRibbon.RibbonStyle = Ribbon.RibbonStyle;
			this.previewRibbon.PageCategories.Clear();
			this.previewRibbon.Pages.Clear();
			this.previewRibbon.DefaultPageCategory.Pages.Clear();
			foreach(RibbonPage page in Ribbon.DefaultPageCategory.Pages) { 
				this.previewRibbon.DefaultPageCategory.Pages.Add((RibbonPage)page.Clone());
				SynsReduceOperations(page, this.previewRibbon.DefaultPageCategory.Pages.GetPageByName(page.Name));
			}
			foreach(RibbonPageCategory cat in Ribbon.PageCategories) {
				this.previewRibbon.PageCategories.Add((RibbonPageCategory)cat.Clone());
				for(int i = 0; i < cat.Pages.Count; i++) {
					SynsReduceOperations(cat.Pages[i], this.previewRibbon.PageCategories[this.previewRibbon.PageCategories.Count - 1].Pages[i]);
				}
			}
			this.xtraScrollableControl1.Controls.Add(this.previewRibbon);
		}
		protected void SynsReduceOperations(RibbonPage dst, RibbonPage src) {
			for(int i = 0; i < src.ReduceOperations.Count; i++) {
				src.ReduceOperations[i].Reference = dst.ReduceOperations[i];
				dst.ReduceOperations[i].Reference = null;
			}
		}
		[Browsable(false)]
		public RibbonControl Ribbon { get { return EditingComponent as RibbonControl; } }
		void InitializeBehaviorCombo() {
			this.cbBehavior.Properties.BeginUpdate();
			try {
				this.cbBehavior.Properties.Items.Clear();
				this.cbBehavior.Properties.Items.Add(ReduceOperationBehavior.Single);
				this.cbBehavior.Properties.Items.Add(ReduceOperationBehavior.UntilAvailable);
				this.cbBehavior.SelectedIndex = 0;
			}
			finally {
				this.cbBehavior.Properties.EndUpdate();
			}
		}
		void OnReduceOperationTypeAddClick(object sender, ItemClickEventArgs e) {
			this.bAdd.Tag = e.Item.Tag;
			bAdd_ItemClick(sender, e);
		}
		void InitializeReduceOperationMenu() {
			this.bTypeGallery.Tag = ReduceOperationType.Gallery;
			this.bTypeLargeButtons.Tag = ReduceOperationType.LargeButtons;
			this.bTypeSmallButtonsWithText.Tag = ReduceOperationType.SmallButtonsWithText;
			this.bTypeButtonGroups.Tag = ReduceOperationType.ButtonGroups;
			this.bTypeCollapseGroup.Tag = ReduceOperationType.CollapseGroup;
			this.bAdd.Tag = this.bTypeLargeButtons.Tag;
		}
		private void reduceOperationsList_SelectedIndexChanged(object sender, EventArgs e) {
			SelectOperation();
			UpdatePropertiesForOperation();
			UpdateButtonsState();
			UpdateRibbon();
		}
		void SelectOperation() {
			RibbonReduceOperationHelper.Ribbon = this.previewRibbon;
			RibbonReduceOperationHelper.SelectedOperation = (ReduceOperation)this.reduceOperationsList.SelectedValue;
		}
		bool ShouldAddLinkForOperation(ReduceOperation op, BarItemLink link) {
			if(op.Operation == ReduceOperationType.LargeButtons || op.Operation == ReduceOperationType.SmallButtonsWithText)
				return !(link is RibbonGalleryBarItemLink || link is BarButtonGroupLink || link is BarEditItemLink);
			if(op.Operation == ReduceOperationType.CollapseGroup)
				return false;
			if(op.Operation == ReduceOperationType.Gallery)
				return link is RibbonGalleryBarItemLink;
			if(op.Operation == ReduceOperationType.ButtonGroups)
				return link.ActAsButtonGroup && IsFirstLinkInButtonGroups(link);
			return false;
		}
		private bool IsFirstLinkInButtonGroups(BarItemLink link) {
			int linkIndex = link.Links.IndexOf(link);
			return linkIndex == 0 || !link.Links[linkIndex - 1].ActAsButtonGroup;
		}
		void InitializeStartLinkCombo(ReduceOperation op) {
			this.cbStartLink.Properties.BeginUpdate();
			try {
				this.cbStartLink.Properties.Items.Clear();
				if(op == null || op.Group == null)
					return;
				foreach(BarItemLink link in op.Group.ItemLinks) {
					if(ShouldAddLinkForOperation(op, link))
						this.cbStartLink.Properties.Items.Add(new BarItemLinkInfo() { Link = link });
				}
				this.cbStartLink.SelectedIndex = LinkIndex2SelectedIndex(op.ItemLinkIndex);
			}
			finally {
				this.cbStartLink.Properties.EndUpdate();
			}
		}
		protected virtual int LinkIndex2SelectedIndex(int linkIndex) {
			int index = 0;
			foreach(BarItemLinkInfo linkInfo in this.cbStartLink.Properties.Items) {
				if(linkInfo.Link.Links.IndexOf(linkInfo.Link) == linkIndex)
					return index;
				index++;
			}
			return -1;
		}
		protected virtual void UpdateButtonsState() {
			ReduceOperation op = (ReduceOperation)this.reduceOperationsList.SelectedValue;
			bool isLast = op != null && this.reduceOperationsList.Items.IndexOf(this.reduceOperationsList.SelectedItem) == this.reduceOperationsList.Items.Count - 1;
			bool isFirst = op != null && this.reduceOperationsList.Items.IndexOf(this.reduceOperationsList.SelectedItem) == 0;
			this.bRemove.Enabled = op != null;
			this.bMoveDown.Enabled = op != null && !isLast;
			this.bMoveUp.Enabled = op != null && !isFirst;
		}
		protected virtual void UpdatePropertiesForOperation() {
			try {
				SuppressPropertiesChanged = true;
				ReduceOperation op = (ReduceOperation)this.reduceOperationsList.SelectedValue;
				this.cbBehavior.Enabled = op != null;
				this.cbPageGroup.Enabled = op != null;
				this.sLinkCount.Enabled = op != null && (op.Operation == ReduceOperationType.LargeButtons || op.Operation == ReduceOperationType.SmallButtonsWithText);
				this.cbStartLink.Enabled = op != null && op.Operation != ReduceOperationType.CollapseGroup;
				if(op == null) {
					this.cbBehavior.SelectedItem = ReduceOperationBehavior.Single;
					this.cbPageGroup.Text = "";
					InitializeStartLinkCombo(op);
					this.sLinkCount.Value = 3;
					return;
				}
				this.cbBehavior.SelectedItem = op.Behavior;
				this.cbPageGroup.SelectedIndex = op.Group == null? -1: ((RibbonPage)this.cbPages.SelectedItem).Groups.IndexOf(op.Group);
				InitializeStartLinkCombo(op);
				this.sLinkCount.Value = op.ItemLinksCount;
			}
			finally {
				SuppressPropertiesChanged = false;
			}
		}
		bool SuppressPropertiesChanged { get; set; }
		private void cbPages_Properties_SelectedIndexChanged(object sender, EventArgs e) {
			this.previewRibbon.SelectedPage = (RibbonPage)cbPages.SelectedItem;
			InitializePageGroupsCombo();
			InitializeReduceOpList(0);
		}
		private void InitializePageGroupsCombo() {
			this.cbPageGroup.Properties.BeginUpdate();
			try {
				this.cbPageGroup.Properties.Items.Clear();
				if(this.cbPages.SelectedItem == null)
					return;
				foreach(RibbonPageGroup group in ((RibbonPage)this.cbPages.SelectedItem).Groups) {
					this.cbPageGroup.Properties.Items.Add(group);
				}
				this.cbPageGroup.SelectedIndex = -1;
			}
			finally {
				this.cbPageGroup.Properties.EndUpdate();	
			}
		}
		private void InitializeReduceOpList(int itemIndex) {
			this.reduceOperationsList.Items.Clear();
			if(this.previewRibbon.SelectedPage == null) {
				UpdatePropertiesForOperation();
				return;
			}
			foreach(ReduceOperation op in this.previewRibbon.SelectedPage.ReduceOperations) {
				this.reduceOperationsList.Items.Add(new ImageListBoxItem() { Value = op });
			}
			this.reduceOperationsList.SelectedIndex = this.reduceOperationsList.Items.Count > itemIndex? itemIndex: -1;
		}
		private void bAdd_ItemClick(object sender, ItemClickEventArgs e) {
			ReduceOperation op = new ReduceOperation();
			op.Operation = (ReduceOperationType)this.bAdd.Tag;
			this.reduceOperationsList.Items.Add(new ImageListBoxItem() { Value = op });
			this.reduceOperationsList.SelectedIndex = this.reduceOperationsList.Items.Count - 1;
			((RibbonPage)this.cbPages.SelectedItem).ReduceOperations.Add(op);
			int pageIndex = this.previewRibbon.TotalPageCategory.Pages.IndexOf((RibbonPage)this.cbPages.SelectedItem);
			ReduceOperation cloned = (ReduceOperation)op.Clone();
			op.Reference = cloned;
			Ribbon.TotalPageCategory.Pages[pageIndex].ReduceOperations.Add(cloned);
		}
		private void cbBehavior_SelectedIndexChanged(object sender, EventArgs e) {
			ReduceOperation op = (ReduceOperation)this.reduceOperationsList.SelectedValue;
			if(op != null)
				op.Behavior = (ReduceOperationBehavior)this.cbBehavior.SelectedItem;
			UpdateRibbon();
		}
		private void tePageGroup_TextChanged(object sender, EventArgs e) {
		}
		private void cbStartLink_SelectedIndexChanged(object sender, EventArgs e) {
			ReduceOperation op = (ReduceOperation)this.reduceOperationsList.SelectedValue;
			if(op == null || op.Group == null)
				return;
			BarItemLink selLink = ((BarItemLinkInfo)this.cbStartLink.SelectedItem).Link;
			op.ItemLinkIndex = selLink.Links.IndexOf(selLink);
			UpdateRibbon();
		}
		private void sLinkCount_EditValueChanged(object sender, EventArgs e) {
			ReduceOperation op = (ReduceOperation)this.reduceOperationsList.SelectedValue;
			if(op == null)
				return;
			op.ItemLinksCount = (int)this.sLinkCount.Value;
			UpdateRibbon();
		}
		protected void UpdateRibbon() {
			this.previewRibbon.Refresh();
			this.previewRibbon.Update();
			MethodInfo mi = Ribbon.GetType().GetMethod("FireRibbonChanged", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { }, new ParameterModifier[] { });
			mi.Invoke(Ribbon, new object[] { });
		}
		private void cbPages_SelectedIndexChanged(object sender, EventArgs e) {
			InitializeReduceOpList(0);
		}
		protected void OnAddItemClick(object sender, ItemClickEventArgs e) {
			this.bAdd.Tag = e.Item.Tag;
			bAdd_ItemClick(sender, e);
		}
		private void bTypeGallery_ItemClick(object sender, ItemClickEventArgs e) {
			OnAddItemClick(sender, e);
		}
		private void bTypeButtonGroups_ItemClick(object sender, ItemClickEventArgs e) {
			OnAddItemClick(sender, e);
		}
		private void bTypeLargeButtons_ItemClick(object sender, ItemClickEventArgs e) {
			OnAddItemClick(sender, e);
		}
		private void bTypeSmallButtonsWithText_ItemClick(object sender, ItemClickEventArgs e) {
			OnAddItemClick(sender, e);
		}
		private void bTypeCollapseGroup_ItemClick(object sender, ItemClickEventArgs e) {
			OnAddItemClick(sender, e);
		}
		private void bRemove_ItemClick(object sender, ItemClickEventArgs e) {
			if(this.reduceOperationsList.Items.Count == 0)
				return;
			ReduceOperation op = (ReduceOperation)this.reduceOperationsList.SelectedValue;
			int itemIndex = this.reduceOperationsList.Items.IndexOf(this.reduceOperationsList.SelectedItem);
			((RibbonPage)this.cbPages.SelectedItem).ReduceOperations.Remove(op);
			op.Reference.OwnerPage.ReduceOperations.Remove(op.Reference);
			InitializeReduceOpList(itemIndex);
		}
		private void cbPageGroup_SelectedIndexChanged(object sender, EventArgs e) {
			ReduceOperation op = (ReduceOperation)this.reduceOperationsList.SelectedValue;
			if(op == null)
				return;
			this.cbStartLink.Properties.Items.Clear();
			op.Group = (RibbonPageGroup)this.cbPageGroup.SelectedItem;
			InitializeStartLinkCombo(op);
			UpdateRibbon();
		}
		void SwapOperations(RibbonPage page, int index1, int index2) {
			ReduceOperation op = page.ReduceOperations[index1];
			page.ReduceOperations[index1] = page.ReduceOperations[index2];
			page.ReduceOperations[index2] = op;
		}
		void SwapListBoxItems(int index1, int index2) { 
			ImageListBoxItem item = this.reduceOperationsList.Items[index1];
			this.reduceOperationsList.Items[index1] = this.reduceOperationsList.Items[index2];
			this.reduceOperationsList.Items[index2] = item;
		}
		protected void MoveSelectedItem(int direction) {
			if(this.reduceOperationsList.Items.Count == 0)
				return;
			ReduceOperation op = (ReduceOperation)this.reduceOperationsList.SelectedValue;
			int opIndex = this.previewRibbon.SelectedPage.ReduceOperations.IndexOf(op);
			direction = Math.Sign(direction);
			SwapOperations(this.previewRibbon.SelectedPage, opIndex, opIndex + direction);
			SwapOperations(Ribbon.SelectedPage, opIndex, opIndex + direction);
			SwapListBoxItems(opIndex, opIndex + direction);
			this.reduceOperationsList.SelectedIndex = opIndex + direction;
			UpdateRibbon();
		}
		private void bMoveUp_ItemClick(object sender, ItemClickEventArgs e) {
			MoveSelectedItem(-1);
		}
		private void bMoveDown_ItemClick(object sender, ItemClickEventArgs e) {
			MoveSelectedItem(+1);
		}
	}
	public class BarItemLinkInfo {
		public BarItemLink Link { get; set; }
		public override string ToString() {
			return string.IsNullOrEmpty(Link.Caption) ? Link.ToString() : Link.Caption;
		}
	}
}
