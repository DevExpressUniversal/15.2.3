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
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Design;
using System.ComponentModel.Design;
namespace DevExpress.XtraBars.Ribbon.Design {
	public partial class KeyTipManager : DevExpress.XtraEditors.Designer.Utils.XtraPGFrame {
		private IDesignerHost designerHost = null;
		private IComponentChangeService componentChangeService = null;
		public KeyTipManager() { }
		public RibbonControl Ribbon { get { return EditingComponent as RibbonControl; } }
		protected virtual TreeList TreeList { get { return treeList1; } }
		protected virtual void PopulateApplicationMenu(TreeListNode parentNode) {
			PopupMenu menu = Ribbon.ApplicationButtonDropDownControl as PopupMenu;
			TreeListNode node;
			if(Ribbon.FindForm() is RibbonForm) {
				if(menu == null)
					node = TreeList.AppendNode(new object[] { "ApplicationMenu", "ApplicationMenu", "ApplicationMenu", Ribbon.ApplicationButtonKeyTip, "" }, parentNode);
				else
					node = TreeList.AppendNode(new object[] { menu.Name, GetObjectName(menu.GetType().ToString()), "ApplicationMenu", Ribbon.ApplicationButtonKeyTip, "" }, parentNode);
				node.Tag = Ribbon;
			}
		}
		protected virtual ISupportRibbonKeyTip GetButtonDropDownKeyTip(BarItemLink itemLink, BaseKeyTipManager keyTipManager) {
			BarButtonItemLink button = itemLink as BarButtonItemLink;
			if (button == null || keyTipManager == null) return null;
			int buttonIndex = keyTipManager.Items.IndexOf(button) + 1;
			if(buttonIndex <= 0 || buttonIndex >= keyTipManager.Items.Count || !keyTipManager.Items[buttonIndex - 1].HasDropDownButton) return null;
			return keyTipManager.Items[buttonIndex]; 
		}
		protected virtual void PopulateItemLinks(BarItemLinkCollection itemLinks, BaseKeyTipManager keyTipManager, TreeListNode parentNode) {
			foreach(BarItemLink itemLink in itemLinks) {
				if(itemLink is BarButtonGroupLink) {
					PopulateItemLinks((itemLink as BarButtonGroupLink).Item.ItemLinks, keyTipManager, parentNode);
					continue;
				}
				TreeList.AppendNode(new object[] { itemLink.Item.Name, GetObjectName(itemLink.GetType().ToString()), itemLink.Caption, itemLink.KeyTip, (itemLink as ISupportRibbonKeyTip).ItemKeyTip}, parentNode).Tag = itemLink;
				ISupportRibbonKeyTip buttonDropDown = GetButtonDropDownKeyTip(itemLink, keyTipManager);
				if (buttonDropDown == null) continue;
				TreeList.AppendNode(new object[] { itemLink.Item.Name + ".DropDownArrow", GetObjectName(itemLink.GetType().ToString()), itemLink.Caption, buttonDropDown.ItemUserKeyTip, buttonDropDown.ItemKeyTip }, parentNode).Tag = null;				
			}			
		}
		protected virtual void PopulateBackstageViewItems(BackstageViewControlItemCollecton items, BaseKeyTipManager baseKeyTipManager, TreeListNode parentNode) {
			foreach(BackstageViewItemBase item in items){
				if(item as BackstageViewItem != null){
					TreeList.AppendNode(new object[] { item.Name, GetObjectName(item.GetType().ToString()), ((BackstageViewItem)item).Caption, ((BackstageViewItem)item).KeyTip, (item as ISupportRibbonKeyTip).ItemKeyTip }, parentNode).Tag = item;
				}
			}			
		}
		protected virtual void PopulatePageContent(RibbonPage page, TreeListNode parentNode) {
			GenerateKeyTips(parentNode);
			foreach(RibbonPageGroup group in page.Groups) {
				PopulateItemLinks(group.ItemLinks, Ribbon.KeyTipManager, parentNode);
				TreeList.AppendNode(new object[] { group.Name, GetObjectName(group.GetType().ToString()), group.Text, group.KeyTip, (group as ISupportRibbonKeyTip).ItemKeyTip }, parentNode).Tag = group;
			}
		}
		protected virtual void PopulatePagesContent() {
			foreach(RibbonPage page in Ribbon.TotalPageCategory.Pages) {
				TreeListNode node = TreeList.AppendNode(new object[] { page.Text + " (" + page.Name + ")", null, null, null, null }, null);
				node.Tag = page;
				PopulatePageContent(page, node);	
			}
		}
		protected virtual void PopulateRibbonPages(TreeListNode parentNode) {
			foreach(RibbonPage page in Ribbon.TotalPageCategory.Pages) {
				TreeList.AppendNode(new object[] { page.Name, GetObjectName(page.GetType().ToString()), page.Text, page.KeyTip, (page as ISupportRibbonKeyTip).ItemKeyTip }, parentNode).Tag = page;
			}
		}
		protected virtual void PopulateUpperRegionItems(TreeListNode parentNode) {
			BaseKeyTipManager keyTipManager = GenerateKeyTips(parentNode);
			PopulateApplicationMenu(parentNode);
			PopulateItemLinks(Ribbon.Toolbar.ItemLinks, Ribbon.KeyTipManager, parentNode);
			PopulateRibbonPages(parentNode);
			UpdateItemKeyTips(parentNode, keyTipManager);
		}
		protected virtual void PopulateMenu(PopupMenu menu) {
			TreeListNode node = TreeList.AppendNode(new object[] { "PopupMenu (" + menu.Name + ")", null, null, null, null }, null);
			node.Tag = menu;
			PopulateItemLinks(menu.ItemLinks, GenerateKeyTips(node), node);
		}
		protected virtual void PopulateBackstageView(BackstageViewControl bsv) {
			TreeListNode node = TreeList.AppendNode(new object[] { "BackstageView (" + bsv.Name + ")", null, null, null, null }, null);
			node.Tag = bsv;
			PopulateBackstageViewItems(bsv.Items, GenerateKeyTips(node), node);
		}
		protected virtual void PopulateMenus() {
			foreach(Object obj in Ribbon.Container.Components) {
				PopupMenu menu = obj as PopupMenu;
				if (menu != null) PopulateMenu(menu);
				BackstageViewControl bsv = obj as BackstageViewControl;
				if (bsv != null) PopulateBackstageView(bsv);
			}
		}
		protected virtual void PopulateLowerRegionItems() {
			PopulatePagesContent();
			PopulateMenus();
		}
		protected virtual void InitializeTreeList() {
			TreeListNode node = TreeList.AppendNode(new object[] {"ApplicationButton, QuickAccessToolbarItems, RibbonPages", null, null, null, null}, null);
			node.Tag = Ribbon;
			PopulateUpperRegionItems(node);
			PopulateLowerRegionItems();
		}
		public KeyTipManagerToolbar Toolbar { get { return keyTipManagerToolbar1; } }
		protected virtual void CorrectRibbonPanelSize() {
			pnlControl.Height = Toolbar.Height;
		}
		protected virtual void InitializeToolbarEvents() {
			Toolbar.GenerateUserKeysTipInGroup.ItemClick += new ItemClickEventHandler(OnGenerateUserKeysTipInGroupClick);
			Toolbar.GenerateAllUserKeyTips.ItemClick += new ItemClickEventHandler(OnGenerateAllUserKeyTipsClick);
			Toolbar.ClearUserKeyTipsInGroup.ItemClick += new ItemClickEventHandler(OnClearUserKeyTipsInGroupClick);
			Toolbar.ClearAllUserKeyTips.ItemClick += new ItemClickEventHandler(OnClearAllUserKeyTipsClick);
		}
		protected virtual void ClearToolbarEvents() {
			Toolbar.GenerateUserKeysTipInGroup.ItemClick -= new ItemClickEventHandler(OnGenerateUserKeysTipInGroupClick);
			Toolbar.GenerateAllUserKeyTips.ItemClick -= new ItemClickEventHandler(OnGenerateAllUserKeyTipsClick);
			Toolbar.ClearUserKeyTipsInGroup.ItemClick -= new ItemClickEventHandler(OnClearUserKeyTipsInGroupClick);
			Toolbar.ClearAllUserKeyTips.ItemClick -= new ItemClickEventHandler(OnClearAllUserKeyTipsClick);
		}
		public override void InitComponent() {
			base.InitComponent();
			InitializeComponent();
			InitializeTreeList();
			InitializeToolbarEvents();
			InitializePropertyGridEvents();	
		}
		void ClearPropertyGridEvents() {
			pgMain.PropertyValueChanged -= new PropertyValueChangedEventHandler(OnObjectPropertiesChanged);
		}
		void InitializePropertyGridEvents() {
			pgMain.PropertyValueChanged += new PropertyValueChangedEventHandler(OnObjectPropertiesChanged);
		}
		protected virtual void DrawVerticalRightLine(CustomDrawNodeCellEventArgs e) {
			e.Graphics.DrawLine(TreeList.ViewInfo.PaintAppearance.VertLine.GetForePen(e.Cache), new Point(e.Bounds.Right, e.Bounds.Top), new Point(e.Bounds.Right, e.Bounds.Bottom));
		}
		protected virtual void OnObjectPropertiesChanged(object sender, PropertyValueChangedEventArgs e) {
			UpdateItemsKeyTips();
		}
		protected virtual void DrawVerticalLeftLine(CustomDrawNodeCellEventArgs e) {
			e.Graphics.DrawLine(TreeList.ViewInfo.PaintAppearance.VertLine.GetForePen(e.Cache), e.Bounds.Location, new Point(e.Bounds.Left, e.Bounds.Bottom));
		}
		protected virtual void DrawVerticalLines(CustomDrawNodeCellEventArgs e) {
			if(e.Column == nameColumn) DrawVerticalLeftLine(e);
			if(e.Column == finalKeyTipColumn || e.Node.ParentNode != null) DrawVerticalRightLine(e);
		}
		private void treeList1_CustomDrawNodeCell(object sender, CustomDrawNodeCellEventArgs e) {
			if(e.Column == userKeyTipColumn || e.Column == finalKeyTipColumn) {
				string userKeyTip = e.Node[userKeyTipColumn] as string, finalKeyTip = e.Node[finalKeyTipColumn] as string;
				if( userKeyTip == null || userKeyTip == "") Appearance.FillRectangle(e.Cache, e.Bounds);
				else if(userKeyTip != finalKeyTip) e.Cache.FillRectangle(e.Cache.GetSolidBrush(Color.Pink), e.Bounds);
			}
			if(e.Node.ParentNode == null) {
				if(e.Column == nameColumn) e.Appearance.DrawString(e.Cache, e.CellText, new Rectangle(new Point(e.Bounds.Left + 3, e.Bounds.Top), TreeList.ViewInfo.RowsInfo[e.Node].Bounds.Size));
				e.Handled = true;
			}
			DrawVerticalLines(e);
		}
		protected virtual string GetObjectName(string name) {
			return name.Substring(name.LastIndexOf('.') + 1, name.Length - name.LastIndexOf('.') - 1);
		}
		private void treeList1_ShowingEditor(object sender, CancelEventArgs e) {
			if(TreeList.FocusedNode.ParentNode == null) e.Cancel = true;
			if((TreeList.FocusedNode.Tag == null || TreeList.FocusedNode.Tag is PopupControl) && TreeList.FocusedColumn == captionColumn) e.Cancel = true;
		}
		private void treeList1_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e) {
			if(e.Node.ParentNode != null) {
				BarItemLink link = e.Node.Tag as BarItemLink;
				if(link != null) {
					BarLinkInfoProvider.SetLinkInfo(link.Item, link);
					pgMain.SelectedObject = link.Item;
				}
				else 
					pgMain.SelectedObject = e.Node.Tag;
			}
			else pgMain.SelectedObject = null;
		}
		protected virtual void ChangeObjectCaption(object obj, string caption) {
			if(caption == null) return;
			if(obj is PopupControl) return;
			BarItemLink link = obj as BarItemLink;
			RibbonPage page = obj as RibbonPage;
			RibbonPageGroup group = obj as RibbonPageGroup;
			if(link != null) link.Caption = caption;
			if(page != null) page.Text = caption;
			if(group != null) group.Text = caption;
			ComponentChangeService.OnComponentChanged(obj, null, null, null);
		}
		protected virtual void ChangeDropDownArrowKeyTip(TreeListNode node, string keyTip) {
			BarButtonItemLink link = node.Tag as BarButtonItemLink;
			if (link != null)
			{
				link.DropDownKeyTip = keyTip;
				ComponentChangeService.OnComponentChanged(link.LinkedObject, null, null, null);
			}
		}
		protected virtual void ChangeApplicationButtonKeyTip(TreeListNode node, string keyTip) {
			RibbonControl rc = node.Tag as RibbonControl;
			if(rc == null) return;
			rc.ApplicationButtonKeyTip = keyTip;
			ComponentChangeService.OnComponentChanged(rc, null, null, null);
		}
		protected virtual void ChangeObjectKeyTip(TreeListNode node, string keyTip){
			if(node.Tag == null) ChangeDropDownArrowKeyTip(node.PrevNode, keyTip);
			else if(node.Tag is RibbonControl) ChangeApplicationButtonKeyTip(node, keyTip);
			else {
				ISupportRibbonKeyTip item = node.Tag as ISupportRibbonKeyTip;
				if(keyTip == null) return;
				if(item == null) return;
				item.ItemUserKeyTip = keyTip;
				ComponentChangeService.OnComponentChanged(item, null, null, null);
			}
		}
		protected virtual ISupportRibbonKeyTip GetApplicationButonKeyTipItem()
		{
			foreach (ISupportRibbonKeyTip item in Ribbon.KeyTipManager.Items) {
				RibbonApplicationButtonKeyTipItem appButtonItem = item as RibbonApplicationButtonKeyTipItem;
				if (appButtonItem != null) return item;
			}
			return null;
		}
		protected virtual void UpdateApplicationButtonNode(TreeListNode node) {
			RibbonControl rc = node.Tag as RibbonControl;
			if(rc == null) return;
			node[userKeyTipColumn] = rc.ApplicationButtonKeyTip;
			ISupportRibbonKeyTip buttonItem = GetApplicationButonKeyTipItem();
			if (buttonItem != null) node[finalKeyTipColumn] = buttonItem.ItemKeyTip;
		}
		protected virtual void UpdateDropDownNode(TreeListNode node, BaseKeyTipManager keyTipManager) { 
			BarButtonItemLink button = node.PrevNode.Tag as BarButtonItemLink;
			if(button == null) return ;
			node[userKeyTipColumn] = button.DropDownKeyTip;
			node[captionColumn] = button.Caption;
			if (keyTipManager == null || node.PrevNode == null) return;
			ISupportRibbonKeyTip item = GetButtonDropDownKeyTip(node.PrevNode.Tag as BarItemLink, keyTipManager);
			if (item == null) return;
			node[finalKeyTipColumn] = item.ItemKeyTip;
		}
		protected virtual void UpdateNode(TreeListNode node, BaseKeyTipManager keyTipManager) {
			if(node.ParentNode != null && node.Tag == null) UpdateDropDownNode(node, keyTipManager);
			else if(node.Tag is RibbonControl) UpdateApplicationButtonNode(node);
			else {
				ISupportRibbonKeyTip item = node.Tag as ISupportRibbonKeyTip;
				if(item == null || node.ParentNode == null) return;
				node[captionColumn] = item.ItemCaption;
				node[userKeyTipColumn] = item.ItemUserKeyTip;
				node[finalKeyTipColumn] = item.ItemKeyTip;
			}
		}
		private void treeList1_CellValueChanged(object sender, CellValueChangedEventArgs e) {
			if(e.Column == captionColumn) ChangeObjectCaption(e.Node.Tag, e.Value.ToString());
			else if(e.Column == userKeyTipColumn) {
				ChangeObjectKeyTip(e.Node, e.Value.ToString());
			}
			UpdateItemKeyTips(e.Node, GenerateKeyTips(e.Node));
		}
		[Browsable(false)]
		protected virtual ISite ComponentSite { get { return Ribbon.Site; } }
		[Browsable(false)]
		protected virtual IDesignerHost DesignerHost
		{
			get
			{
				if (designerHost == null) designerHost = ComponentSite.GetService(typeof(IDesignerHost)) as IDesignerHost;
				return designerHost;
			}
		}
		[Browsable(false)]
		protected internal virtual IComponentChangeService ComponentChangeService
		{
			get
			{
				if (componentChangeService == null) componentChangeService = DesignerHost.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
				return componentChangeService;
			}
		}
		protected virtual BaseKeyTipManager GenerateKeyTips(TreeListNode node) {
			if (node == null) return null;
			if (node.ParentNode != null) node = node.ParentNode;
			RibbonControl rc = node.Tag as RibbonControl;
			if (rc != null)
			{
				Ribbon.KeyTipManager.GeneratePageKeyTips();
				return Ribbon.KeyTipManager;
			}
			RibbonPage page = node.Tag as RibbonPage;
			if (page != null)
			{
				Ribbon.KeyTipManager.GeneratePanelKeyTipsInDesignTime(page);
				return Ribbon.KeyTipManager;
			}
			PopupMenu menu = node.Tag as PopupMenu;
			if (menu != null) {
				ContainerKeyTipManager keyTip = new ContainerKeyTipManager(Ribbon, null, menu.ItemLinks);
				keyTip.GenerateContainerKeyTips();
				return keyTip;
			}
			BackstageViewControl bsv = node.Tag as BackstageViewControl;
			if (bsv != null) {
				bsv.FillKeyTipItems();
				bsv.KeyTipManager.GenerateKeyTips();
				return bsv.KeyTipManager;
			}
			return null;
		}
		protected virtual void UpdateItemKeyTips(TreeListNode node, BaseKeyTipManager keyTipManager) {
			if (node == null) return;
			if (node.ParentNode != null) node = node.ParentNode;
			TreeList.BeginUpdate();
			foreach (TreeListNode nd in node.Nodes) {
				UpdateNode(nd, keyTipManager);
			}
			TreeList.EndUpdate();
		}
		protected virtual void SerializeNode(TreeListNode node) {
			if(node.Tag == null) {
				if(node.ParentNode != null) ComponentChangeService.OnComponentChanged(node.PrevNode.Tag, null, null, null);
			}
			else {
				ComponentChangeService.OnComponentChanged(node.Tag, null, null, null);
			} 
		}
		protected virtual void SerializeNodes(TreeListNode parentNode) {
			foreach(TreeListNode node in parentNode.Nodes) {
				SerializeNode(node);
			}
		}
		protected virtual void UpdateItemsKeyTips() {
			foreach(TreeListNode node in TreeList.Nodes) {
				UpdateItemKeyTips(node, GenerateKeyTips(node));
			}
		}
		protected virtual void GenerateUserKeyTipsInGroup(TreeListNode node, bool clear) {
			BaseKeyTipManager keyTipManager = GenerateKeyTips(node);
			if(keyTipManager == null) return;
			foreach(ISupportRibbonKeyTip item in keyTipManager.Items) {
				item.ItemUserKeyTip = clear? "": item.ItemKeyTip;
			}
			SerializeNodes(node.ParentNode != null ? node.ParentNode: node);
			UpdateItemKeyTips(node, GenerateKeyTips(node));
		}
		protected virtual void OnGenerateUserKeysTipInGroupClick(object sender, ItemClickEventArgs e) {
			GenerateUserKeyTipsInGroup(TreeList.FocusedNode, false);
		}
		protected virtual void OnGenerateAllUserKeyTipsClick(object sender, ItemClickEventArgs e) {
			foreach(TreeListNode node in TreeList.Nodes) {
				GenerateUserKeyTipsInGroup(node, false);		
			}
		}
		protected virtual void OnClearUserKeyTipsInGroupClick(object sender, ItemClickEventArgs e) {
			GenerateUserKeyTipsInGroup(TreeList.FocusedNode, true);			
		}
		protected virtual void OnClearAllUserKeyTipsClick(object sender, ItemClickEventArgs e) {
			foreach(TreeListNode node in TreeList.Nodes) {
				GenerateUserKeyTipsInGroup(node, true);
			}	   
		}
	}
}
