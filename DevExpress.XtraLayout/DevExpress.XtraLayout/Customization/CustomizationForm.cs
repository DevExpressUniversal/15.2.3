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
using System.Reflection;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;
using System.ComponentModel;
using System.Globalization;
using System.ComponentModel.Design.Serialization;
using System.ComponentModel.Design;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraLayout.Handlers;
using DevExpress.XtraLayout.HitInfo;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Controls;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraEditors.Customization;
using DevExpress.LookAndFeel;
using DevExpress.XtraLayout.Localization;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.XtraLayout.Customization.Controls;
namespace DevExpress.XtraLayout.Customization {
	[DesignTimeVisible(false)]
	public class CustomizationForm : UserCustomizationForm {
		public LayoutControl layoutControl1;
		public LayoutTreeView layoutTreeView1;
		public ButtonsPanel buttonsPanel1;
		public LayoutControlGroup layoutControlGroup1;
		public LayoutControlItem buttonsPanelItem;
		public TabbedControlGroup tabbedControlGroup1;
		public LayoutControlGroup layoutTreeViewGroup;
		public LayoutControlItem layoutTreeViewItem;
		public HiddenItemsList hiddenItemsList1;
		public LayoutControlItem hiddenItemsListItem;															   
		public CustomizationPropertyGrid customizationPropertyGrid1;
		public LayoutControlItem propertyGridItem;
		public SplitterItem splitterItem1;
		public LayoutControlGroup hiddenItemsGroup;
		public LayoutControlItem panelTemplateItem;
		public LayoutControlGroup templateListGroup;
		public TemplateItemsList templateList;
		public LayoutControlItem templatesListLCI;
		public SearchControl searchControlForHiddenItems;
		public LayoutControlItem searchControlForHiddenItemsItem;
		public SearchControl searchControlForLayoutTreeView;
		public LayoutControlItem searchControlForLayoutTreeViewItem;
		public SearchControl searchControlForTemplateList;
		public LayoutControlItem searchControlForTemplateListItem;
		public Panel panelTemplate;
		public CustomizationForm() {
			InitializeComponent();
			SetElementsText();
			CreatePanelTemplate();
		}
		protected virtual void CreatePanelTemplate() {
		 if(panelTemplateItem == null) {
				panelTemplateItem = new LayoutControlItem();
				panelTemplateItem.Text = "PanelTemplateItem";
				panelTemplate = new Panel();
				panelTemplateItem.Control = panelTemplate;
				panelTemplateItem.TextVisible = false;
				layoutControlGroup1.AddItem(panelTemplateItem, propertyGridItem, InsertType.Top);
				panelTemplateItem.ControlAlignment = ContentAlignment.MiddleCenter;
			}
		}
		public override void ResetActiveControl() {
			layoutControl1.ActiveControl = ((ILayoutControl)layoutControl1).FakeFocusContainer;
		}
		protected void SetElementsText() {
			layoutTreeViewGroup.Text = LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.TreeViewPageTitle);
			hiddenItemsGroup.Text = LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.HiddenItemsPageTitle);
		}
		protected override void InitCustomizationForm() {
			if (OwnerControl != null) {
				if(!OwnerControl.DesignMode) templateListGroup.Visibility = LayoutVisibility.Never;
				if (!OwnerControl.OptionsCustomizationForm.ShowLayoutTreeView) {
					layoutTreeViewGroup.Visibility = LayoutVisibility.Never;
				}
				if (!OwnerControl.OptionsCustomizationForm.ShowPropertyGrid) {
					propertyGridItem.Visibility = LayoutVisibility.Never;
					splitterItem1.Visibility = LayoutVisibility.Never;
				}
				if(!OwnerControl.OptionsCustomizationForm.ShowButtonsPanel) buttonsPanelItem.Visibility = LayoutVisibility.Never;
				UpdateCustomiztionPropertyGridOrPanel();
			}
			base.InitCustomizationForm();
		}
		void UpdateCustomiztionPropertyGridOrPanel() {
			CustomizationForm form = this;
			if(form.panelTemplateItem != null) form.panelTemplateItem.Visibility = LayoutVisibility.Never;
			if(!Visible && form.propertyGridItem != null) {
								if(!OwnerControl.OptionsCustomizationForm.ShowPropertyGrid) {
					form.propertyGridItem.Visibility = LayoutVisibility.Never;
					form.splitterItem1.Visibility = LayoutVisibility.Never;
				}
				if(OwnerControl.OptionsCustomizationForm.ShowPropertyGrid) {
					form.propertyGridItem.Visibility = LayoutVisibility.Always;
					form.splitterItem1.Visibility = LayoutVisibility.Always;
				}
				return;
			}
			if(!OwnerControl.OptionsCustomizationForm.ShowPropertyGrid) {
				form.propertyGridItem.Visibility = LayoutVisibility.Never;
				form.splitterItem1.Visibility = LayoutVisibility.Never;
			}
			if(OwnerControl.OptionsCustomizationForm.ShowPropertyGrid) {
				form.propertyGridItem.Visibility = LayoutVisibility.Always;
				form.splitterItem1.Visibility = LayoutVisibility.Always;
			}
		}
		void InitializeComponent() {
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.searchControlForTemplateList = new DevExpress.XtraEditors.SearchControl();
			this.searchControlForLayoutTreeView = new DevExpress.XtraEditors.SearchControl();
			this.layoutTreeView1 = new DevExpress.XtraLayout.Customization.Controls.LayoutTreeView();
			this.searchControlForHiddenItems = new DevExpress.XtraEditors.SearchControl();
			this.hiddenItemsList1 = new DevExpress.XtraLayout.Customization.Controls.HiddenItemsList();
			this.customizationPropertyGrid1 = new DevExpress.XtraLayout.Customization.Controls.CustomizationPropertyGrid();
			this.buttonsPanel1 = new DevExpress.XtraLayout.Customization.Controls.ButtonsPanel();
			this.templateList = new DevExpress.XtraLayout.Customization.Controls.TemplateItemsList();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.buttonsPanelItem = new DevExpress.XtraLayout.LayoutControlItem();
			this.tabbedControlGroup1 = new DevExpress.XtraLayout.TabbedControlGroup();
			this.hiddenItemsGroup = new DevExpress.XtraLayout.LayoutControlGroup();
			this.hiddenItemsListItem = new DevExpress.XtraLayout.LayoutControlItem();
			this.searchControlForHiddenItemsItem = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutTreeViewGroup = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutTreeViewItem = new DevExpress.XtraLayout.LayoutControlItem();
			this.searchControlForLayoutTreeViewItem = new DevExpress.XtraLayout.LayoutControlItem();
			this.templateListGroup = new DevExpress.XtraLayout.LayoutControlGroup();
			this.templatesListLCI = new DevExpress.XtraLayout.LayoutControlItem();
			this.searchControlForTemplateListItem = new DevExpress.XtraLayout.LayoutControlItem();
			this.propertyGridItem = new DevExpress.XtraLayout.LayoutControlItem();
			this.splitterItem1 = new DevExpress.XtraLayout.SplitterItem();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.searchControlForTemplateList.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.searchControlForLayoutTreeView.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.searchControlForHiddenItems.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.hiddenItemsList1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.templateList)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.buttonsPanelItem)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tabbedControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.hiddenItemsGroup)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.hiddenItemsListItem)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.searchControlForHiddenItemsItem)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutTreeViewGroup)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutTreeViewItem)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.searchControlForLayoutTreeViewItem)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.templateListGroup)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.templatesListLCI)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.searchControlForTemplateListItem)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.propertyGridItem)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitterItem1)).BeginInit();
			this.SuspendLayout();
			this.layoutControl1.AllowCustomization = false;
			this.layoutControl1.Controls.Add(this.searchControlForTemplateList);
			this.layoutControl1.Controls.Add(this.searchControlForLayoutTreeView);
			this.layoutControl1.Controls.Add(this.searchControlForHiddenItems);
			this.layoutControl1.Controls.Add(this.customizationPropertyGrid1);
			this.layoutControl1.Controls.Add(this.hiddenItemsList1);
			this.layoutControl1.Controls.Add(this.layoutTreeView1);
			this.layoutControl1.Controls.Add(this.buttonsPanel1);
			this.layoutControl1.Controls.Add(this.templateList);
			this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControl1.Location = new System.Drawing.Point(0, 0);
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(-807, 188, 657, 522);
			this.layoutControl1.Root = this.layoutControlGroup1;
			this.layoutControl1.Size = new System.Drawing.Size(611, 337);
			this.layoutControl1.TabIndex = 0;
			this.layoutControl1.Text = "layoutControl1";
			this.searchControlForTemplateList.Client = this.templateList;
			this.searchControlForTemplateList.Location = new System.Drawing.Point(24, 75);
			this.searchControlForTemplateList.Name = "searchControlForTemplateList";
			this.searchControlForTemplateList.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Repository.ClearButton(),
			new DevExpress.XtraEditors.Repository.SearchButton()});
			this.searchControlForTemplateList.Properties.Client = this.templateList;
			this.searchControlForTemplateList.Size = new System.Drawing.Size(228, 20);
			this.searchControlForTemplateList.StyleController = this.layoutControl1;
			this.searchControlForTemplateList.TabIndex = 22;
			this.searchControlForLayoutTreeView.Client = this.layoutTreeView1;
			this.searchControlForLayoutTreeView.Location = new System.Drawing.Point(24, 75);
			this.searchControlForLayoutTreeView.Name = "searchControlForLayoutTreeView";
			this.searchControlForLayoutTreeView.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Repository.ClearButton(),
			new DevExpress.XtraEditors.Repository.SearchButton()});
			this.searchControlForLayoutTreeView.Properties.Client = this.layoutTreeView1;
			this.searchControlForLayoutTreeView.Size = new System.Drawing.Size(228, 20);
			this.searchControlForLayoutTreeView.StyleController = this.layoutControl1;
			this.searchControlForLayoutTreeView.TabIndex = 16;
			this.layoutTreeView1.Location = new System.Drawing.Point(24, 99);
			this.layoutTreeView1.Name = "layoutTreeView1";
			this.layoutTreeView1.Role = DevExpress.XtraLayout.Customization.Controls.TreeViewRoles.LayoutTreeView;
			this.layoutTreeView1.ShowHiddenItemsInTreeView = true;
			this.layoutTreeView1.Size = new System.Drawing.Size(228, 214);
			this.searchControlForHiddenItems.Client = this.hiddenItemsList1;
			this.searchControlForHiddenItems.Location = new System.Drawing.Point(24, 75);
			this.searchControlForHiddenItems.MinimumSize = new System.Drawing.Size(80, 0);
			this.searchControlForHiddenItems.Name = "searchControlForHiddenItems";
			this.searchControlForHiddenItems.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Repository.ClearButton(),
			new DevExpress.XtraEditors.Repository.SearchButton()});
			this.searchControlForHiddenItems.Properties.Client = this.hiddenItemsList1;
			this.searchControlForHiddenItems.Size = new System.Drawing.Size(228, 20);
			this.searchControlForHiddenItems.StyleController = this.layoutControl1;
			this.searchControlForHiddenItems.TabIndex = 10;
			this.hiddenItemsList1.Location = new System.Drawing.Point(24, 99);
			this.hiddenItemsList1.Name = "hiddenItemsList1";
			this.hiddenItemsList1.Size = new System.Drawing.Size(228, 214);
			this.customizationPropertyGrid1.Location = new System.Drawing.Point(273, 41);
			this.customizationPropertyGrid1.Name = "customizationPropertyGrid1";
			this.customizationPropertyGrid1.Size = new System.Drawing.Size(326, 284);
			this.buttonsPanel1.Location = new System.Drawing.Point(12, 12);
			this.buttonsPanel1.Name = "buttonsPanel1";
			this.buttonsPanel1.Size = new System.Drawing.Size(100, 25);
			this.templateList.Location = new System.Drawing.Point(24, 99);
			this.templateList.Name = "templateList";
			this.templateList.Size = new System.Drawing.Size(228, 214);
			this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.buttonsPanelItem,
			this.tabbedControlGroup1,
			this.propertyGridItem,
			this.splitterItem1});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "Root";
			this.layoutControlGroup1.Size = new System.Drawing.Size(611, 337);
			this.layoutControlGroup1.TextVisible = false;
			this.buttonsPanelItem.Control = this.buttonsPanel1;
			this.buttonsPanelItem.CustomizationFormText = "layoutControlItem1";
			this.buttonsPanelItem.Location = new System.Drawing.Point(0, 0);
			this.buttonsPanelItem.Name = "buttonsPanelItem";
			this.buttonsPanelItem.Size = new System.Drawing.Size(591, 29);
			this.buttonsPanelItem.TextSize = new System.Drawing.Size(0, 0);
			this.buttonsPanelItem.TextVisible = false;
			this.tabbedControlGroup1.CustomizationFormText = "tabbedControlGroup1";
			this.tabbedControlGroup1.Location = new System.Drawing.Point(0, 29);
			this.tabbedControlGroup1.Name = "tabbedControlGroup1";
			this.tabbedControlGroup1.SelectedTabPage = this.hiddenItemsGroup;
			this.tabbedControlGroup1.SelectedTabPageIndex = 0;
			this.tabbedControlGroup1.Size = new System.Drawing.Size(256, 288);
			this.tabbedControlGroup1.TabPages.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.hiddenItemsGroup,
			this.layoutTreeViewGroup,
			this.templateListGroup});
			this.hiddenItemsGroup.CustomizationFormText = "layoutControlGroup2";
			this.hiddenItemsGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.hiddenItemsListItem,
			this.searchControlForHiddenItemsItem});
			this.hiddenItemsGroup.Location = new System.Drawing.Point(0, 0);
			this.hiddenItemsGroup.Name = "hiddenItemsGroup";
			this.hiddenItemsGroup.Size = new System.Drawing.Size(232, 242);
			this.hiddenItemsGroup.Text = "Hidden Items";
			this.hiddenItemsListItem.Control = this.hiddenItemsList1;
			this.hiddenItemsListItem.CustomizationFormText = "layoutControlItem3";
			this.hiddenItemsListItem.Location = new System.Drawing.Point(0, 24);
			this.hiddenItemsListItem.Name = "hiddenItemsListItem";
			this.hiddenItemsListItem.Size = new System.Drawing.Size(232, 218);
			this.hiddenItemsListItem.TextSize = new System.Drawing.Size(0, 0);
			this.hiddenItemsListItem.TextVisible = false;
			this.searchControlForHiddenItemsItem.Control = this.searchControlForHiddenItems;
			this.searchControlForHiddenItemsItem.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			this.searchControlForHiddenItemsItem.FillControlToClientArea = false;
			this.searchControlForHiddenItemsItem.Location = new System.Drawing.Point(0, 0);
			this.searchControlForHiddenItemsItem.Name = "searchControlForHiddenItemsItem";
			this.searchControlForHiddenItemsItem.Size = new System.Drawing.Size(232, 24);
			this.searchControlForHiddenItemsItem.TextSize = new System.Drawing.Size(0, 0);
			this.searchControlForHiddenItemsItem.TextVisible = false;
			this.searchControlForHiddenItemsItem.TrimClientAreaToControl = false;
			this.layoutTreeViewGroup.CustomizationFormText = "layoutControlGroup3";
			this.layoutTreeViewGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutTreeViewItem,
			this.searchControlForLayoutTreeViewItem});
			this.layoutTreeViewGroup.Location = new System.Drawing.Point(0, 0);
			this.layoutTreeViewGroup.Name = "layoutTreeViewGroup";
			this.layoutTreeViewGroup.Size = new System.Drawing.Size(232, 242);
			this.layoutTreeViewGroup.Text = "Layout Tree View";
			this.layoutTreeViewItem.Control = this.layoutTreeView1;
			this.layoutTreeViewItem.CustomizationFormText = "layoutControlItem2";
			this.layoutTreeViewItem.Location = new System.Drawing.Point(0, 24);
			this.layoutTreeViewItem.Name = "layoutTreeViewItem";
			this.layoutTreeViewItem.Size = new System.Drawing.Size(232, 218);
			this.layoutTreeViewItem.TextSize = new System.Drawing.Size(0, 0);
			this.layoutTreeViewItem.TextVisible = false;
			this.searchControlForLayoutTreeViewItem.Control = this.searchControlForLayoutTreeView;
			this.searchControlForLayoutTreeViewItem.Location = new System.Drawing.Point(0, 0);
			this.searchControlForLayoutTreeViewItem.Name = "searchControlForLayoutTreeViewItem";
			this.searchControlForLayoutTreeViewItem.Size = new System.Drawing.Size(232, 24);
			this.searchControlForLayoutTreeViewItem.TextSize = new System.Drawing.Size(0, 0);
			this.searchControlForLayoutTreeViewItem.TextVisible = false;
			this.templateListGroup.CustomizationFormText = "Templates";
			this.templateListGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.templatesListLCI,
			this.searchControlForTemplateListItem});
			this.templateListGroup.Location = new System.Drawing.Point(0, 0);
			this.templateListGroup.Name = "templateListGroup";
			this.templateListGroup.Size = new System.Drawing.Size(232, 242);
			this.templateListGroup.Text = "Templates";
			this.templatesListLCI.Control = this.templateList;
			this.templatesListLCI.CustomizationFormText = "layoutControlItem3";
			this.templatesListLCI.Location = new System.Drawing.Point(0, 24);
			this.templatesListLCI.Name = "templatesListLCI";
			this.templatesListLCI.Size = new System.Drawing.Size(232, 218);
			this.templatesListLCI.Text = "hiddenItemsListItem";
			this.templatesListLCI.TextSize = new System.Drawing.Size(0, 0);
			this.templatesListLCI.TextVisible = false;
			this.searchControlForTemplateListItem.Control = this.searchControlForTemplateList;
			this.searchControlForTemplateListItem.Location = new System.Drawing.Point(0, 0);
			this.searchControlForTemplateListItem.Name = "searchControlForTemplateListItem";
			this.searchControlForTemplateListItem.Size = new System.Drawing.Size(232, 24);
			this.searchControlForTemplateListItem.TextSize = new System.Drawing.Size(0, 0);
			this.searchControlForTemplateListItem.TextVisible = false;
			this.propertyGridItem.Control = this.customizationPropertyGrid1;
			this.propertyGridItem.CustomizationFormText = "layoutControlItem4";
			this.propertyGridItem.Location = new System.Drawing.Point(261, 29);
			this.propertyGridItem.Name = "propertyGridItem";
			this.propertyGridItem.Size = new System.Drawing.Size(330, 288);
			this.propertyGridItem.TextLocation = DevExpress.Utils.Locations.Top;
			this.propertyGridItem.TextSize = new System.Drawing.Size(0, 0);
			this.propertyGridItem.TextVisible = false;
			this.splitterItem1.AllowHotTrack = true;
			this.splitterItem1.CustomizationFormText = "LayoutItem8";
			this.splitterItem1.Location = new System.Drawing.Point(256, 29);
			this.splitterItem1.Name = "LayoutItem8";
			this.splitterItem1.Size = new System.Drawing.Size(5, 288);
			this.ClientSize = new System.Drawing.Size(611, 337);
			this.Controls.Add(this.layoutControl1);
			this.MinimumSize = new System.Drawing.Size(200, 97);
			this.Name = "CustomizationForm";
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.searchControlForTemplateList.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.searchControlForLayoutTreeView.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.searchControlForHiddenItems.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.hiddenItemsList1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.templateList)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.buttonsPanelItem)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tabbedControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.hiddenItemsGroup)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.hiddenItemsListItem)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.searchControlForHiddenItemsItem)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutTreeViewGroup)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutTreeViewItem)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.searchControlForLayoutTreeViewItem)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.templateListGroup)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.templatesListLCI)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.searchControlForTemplateListItem)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.propertyGridItem)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.splitterItem1)).EndInit();
			this.ResumeLayout(false);
		}
	}
}
