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
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Customization.Controls;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
namespace DevExpress.XtraGrid.Views.Layout.Designer {
	public partial class TemplateCardCustomizationControl : UserControl {			  
		LayoutControl targetLayoutControlCore = null;
		LayoutViewCustomizer customizerCore = null;
		bool fDisposing = false;
		DesignerLayoutTreeView designerTreeView;
		XtraEditors.SearchControl designerSearchControlForTreeView;
		DesignerHiddenItemsList designerHiddenItemsList;
		DesignerPropertyGrid designerPropertyGrid;
		LayoutControlItem searchControlForTreeViewLayoutItem;
		LayoutControlItem treeViewLayoutItem;
		LayoutControlItem hiddenItemsListLayoutItem;
		LayoutControlItem propertyGridLayoutItem;
		bool showTreeViewGroup;
		bool showHiddenItemsGroup;
		bool showPropertyGridGroup;
		public TemplateCardCustomizationControl(LayoutViewCustomizer customizer, LayoutControl targetLayoutControl) {
			this.customizerCore = customizer;
			this.targetLayoutControlCore = targetLayoutControl;
			InitializeComponent();
			layoutControl.BeginUpdate();
			CalcGroupsVisibility();
			CreateCustomizationControls();
			SetComponentsText();
			SetGroupsVisibility();
			ConfigureGroupVisibility(customizationGroup, false);
			SubscribeEvents();
			this.BorderStyle = BorderStyle.None;
			this.layoutControl.Root.GroupBordersVisible = false;
			layoutControl.EndUpdate();
			if(showTreeViewGroup) 
				treeViewGroup.Expanded = false;
		}
		protected internal LayoutViewCustomizer Customizer { get { return customizerCore; } }
		protected internal LayoutControl TargetLayoutControl { get { return targetLayoutControlCore; } }
		public bool IsDisposingInProgress { get { return fDisposing; } }
		protected virtual void DoDispose() {
			UnSubscribeEvents();
			targetLayoutControlCore = null;
			customizerCore = null;
			if(designerHiddenItemsList != null) {
				designerHiddenItemsList.Dispose();
				designerHiddenItemsList = null;
			}
			if(designerTreeView != null) {
				designerTreeView.Dispose();
				designerTreeView = null;
			}
			if(designerSearchControlForTreeView != null) {
				designerSearchControlForTreeView.Dispose();
				designerSearchControlForTreeView = null;
			}
			if(designerPropertyGrid != null) {
				designerPropertyGrid.Dispose();
				designerPropertyGrid = null;
			}
		}
		protected void SubscribeEvents() {
			TargetLayoutControl.ShowCustomization+=new EventHandler(targetLayoutControl_ShowCustomization);
			TargetLayoutControl.HideCustomization+=new EventHandler(targetLayoutControl_HideCustomization);
		}
		protected void UnSubscribeEvents() {
			TargetLayoutControl.ShowCustomization-=new EventHandler(targetLayoutControl_ShowCustomization);
			TargetLayoutControl.HideCustomization-=new EventHandler(targetLayoutControl_HideCustomization);
		}
		protected void SetComponentsText() {
			customizationGroup.Text = Customizer.Localizer.StringByID(CustomizationStringID.GroupCustomization);
			treeViewGroup.Text = Customizer.Localizer.StringByID(CustomizationStringID.GroupTreeView);
			hiddenItemsGroup.Text = Customizer.Localizer.StringByID(CustomizationStringID.GroupHiddenItems);
			propertyGridGroup.Text = Customizer.Localizer.StringByID(CustomizationStringID.GroupPropertyGrid);
		}
		protected void SetGroupsVisibility() {
			if(Customizer != null && Customizer.EditingLayoutView != null) {
				hiddenItemsGroup.ExpandOnDoubleClick = true;
				treeViewGroup.ExpandOnDoubleClick = true;
				propertyGridGroup.ExpandOnDoubleClick = true;
				ConfigureGroupVisibility(hiddenItemsGroup, showHiddenItemsGroup);
				ConfigureGroupVisibility(treeViewGroup, showTreeViewGroup);
				ConfigureGroupVisibility(propertyGridGroup, showPropertyGridGroup);
			}
		}
		protected void CalcGroupsVisibility() {
			if(Customizer!=null && Customizer.EditingLayoutView!=null) {
				showTreeViewGroup = CalcCustomizationVisibilityTreeViewGroup();
				showHiddenItemsGroup = CalcCustomizationVisibilityHiddenItemsGroup();
				showPropertyGridGroup = CalcCustomizationVisibilityPropertyGridGroup();
			}		
		}
		protected bool CalcCustomizationVisibilityTreeViewGroup() {
			bool result = Customizer.EditingLayoutView.OptionsCustomization.UseAdvancedRuntimeCustomization 
				? Customizer.EditingLayoutView.OptionsCustomization.ShowGroupLayoutTreeView : false;
			return result || Customizer.EditingLayoutView.IsDesignMode;
		}
		protected bool CalcCustomizationVisibilityHiddenItemsGroup() {
			bool result = Customizer.EditingLayoutView.OptionsCustomization.UseAdvancedRuntimeCustomization 
				? Customizer.EditingLayoutView.OptionsCustomization.ShowGroupHiddenItems : true;
			return result || Customizer.EditingLayoutView.IsDesignMode;
		}
		protected bool CalcCustomizationVisibilityPropertyGridGroup() {
			return Customizer.EditingLayoutView.IsDesignMode;
		}
		protected void ConfigureGroupVisibility(LayoutControlGroup group, bool visible) {
			if(visible) {
				group.Visibility = LayoutVisibility.Always;
				group.Expanded = group.GroupBordersVisible = true;
			} else {
				group.Expanded = group.GroupBordersVisible = false;
				group.Visibility = LayoutVisibility.Never;
			}
		}
		public bool AllGroupsHidden {
			get { return !showTreeViewGroup && !showHiddenItemsGroup && !showPropertyGridGroup; }
		}
		protected void targetLayoutControl_HideCustomization(object sender, EventArgs e) {
			ConfigureGroupVisibility(customizationGroup, false);
			if(Customizer.designerSplitContainer2!=null) {
				Customizer.designerSplitContainer2.PanelVisibility = AllGroupsHidden ? SplitPanelVisibility.Panel1 : SplitPanelVisibility.Both;
			}
		}
		protected void targetLayoutControl_ShowCustomization(object sender, EventArgs e) {
			ConfigureGroupVisibility(customizationGroup, showHiddenItemsGroup || showTreeViewGroup );
			if(customizationGroup.Visibility == LayoutVisibility.Always) {
				customizationGroup.Expanded = true;
				customizationGroup.GroupBordersVisible = false;
				if(hiddenItemsGroup.Visibility == LayoutVisibility.Always && hiddenItemsGroup.Expanded) {
					UpdateLayout();
				}
			}
			if(Customizer.designerSplitContainer2!=null) {
				Customizer.designerSplitContainer2.PanelVisibility =  AllGroupsHidden ? SplitPanelVisibility.Panel1 : SplitPanelVisibility.Both;
			}
		}
		protected void CreateCustomizationControls() {
			customizationGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(4);
			TargetLayoutControl.OptionsCustomizationForm.ShowLoadButton = false;
			TargetLayoutControl.OptionsCustomizationForm.ShowSaveButton = false;
			if(showTreeViewGroup) {
				designerTreeView = new DesignerLayoutTreeView(this);
				designerSearchControlForTreeView = new XtraEditors.SearchControl();
				designerSearchControlForTreeView.Client = designerTreeView;
				treeViewGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0);
				searchControlForTreeViewLayoutItem = treeViewGroup.AddItem("DesignerLayoutTreeView", designerSearchControlForTreeView);
				searchControlForTreeViewLayoutItem.TextVisible = false;
				treeViewLayoutItem = treeViewGroup.AddItem("DesignerLayoutTreeView", designerTreeView);
				treeViewLayoutItem.TextVisible = false;
				treeViewLayoutItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0);
				treeViewLayoutItem.Spacing = new DevExpress.XtraLayout.Utils.Padding(0);
			}
			if(showHiddenItemsGroup) {
				designerHiddenItemsList = new DesignerHiddenItemsList(this);
				hiddenItemsListLayoutItem = hiddenItemsGroup.AddItem("DesignerHiddenItemsList", designerHiddenItemsList);
				hiddenItemsGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0);
				hiddenItemsListLayoutItem.TextVisible = false;
				hiddenItemsListLayoutItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0);
				hiddenItemsListLayoutItem.Spacing = new DevExpress.XtraLayout.Utils.Padding(0);
				designerHiddenItemsList.StyleController = null;
			}
			if(showPropertyGridGroup) {
				designerPropertyGrid = new DesignerPropertyGrid(this);
				propertyGridLayoutItem =  propertyGridGroup.AddItem("DesignerPropertyGrid", designerPropertyGrid);
				propertyGridGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0);
				propertyGridLayoutItem.TextVisible = false;
				propertyGridLayoutItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0);
				propertyGridLayoutItem.Spacing = new DevExpress.XtraLayout.Utils.Padding(0);
			}
		}
		protected internal void UpdateLayout() {
			if(IsDisposingInProgress) return;
			layoutControl.BeginUpdate();
			if(hiddenItemsGroup.Visibility == LayoutVisibility.Always) hiddenItemsGroup.Update();
			if(treeViewGroup.Visibility == LayoutVisibility.Always) treeViewGroup.Update();
			(layoutControl as ILayoutControl).ShouldResize = true;
			(layoutControl as ILayoutControl).ShouldUpdateConstraints = true;
			layoutControl.EndUpdate();
		}
	}
	internal class DesignerPropertyGrid : CustomizationPropertyGrid {
		ILayoutControl designedControl;
		LayoutView editingView;
		public DesignerPropertyGrid(TemplateCardCustomizationControl manager)
			: base() {
			this.designedControl = manager.TargetLayoutControl;
			this.editingView = (manager.Customizer != null) ? manager.Customizer.EditingLayoutView : null;
			Register();
			this.MinimumSize = new Size(100, 400);
			this.MaximumSize = new Size(0, 400);
			if(OwnerControl!=null) UpdateColors(OwnerControl.LookAndFeel);
		}
		protected void UpdateColors(UserLookAndFeel LookAndFeel) {
			Color backColor = GetBackColor(LookAndFeel);
			this.HelpBackColor = backColor;
			this.BackColor = backColor;
			this.CommandsBackColor = backColor;
			this.ViewBackColor = LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.Window);
			this.LineColor = LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.Control);
			SetHotCommandsBackColor(LookAndFeel);
		}
		private Color GetBackColor(UserLookAndFeel LookAndFeel) {
			return LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.Control);
		}
		private Control GetControl(string typeName) {
			foreach(Control control in this.Controls) {
				if(string.Compare(control.GetType().Name, typeName, true) == 0) return control;
			}
			return null;
		}
		private void SetHotCommandsBackColor(UserLookAndFeel LookAndFeel) {
			Control hotCommands = this.GetControl("HotCommands");
			if(hotCommands != null) {
				foreach(Control control in hotCommands.Controls) {
					control.BackColor = GetBackColor(LookAndFeel);
				}
			}
		}
		protected override ILayoutControl GetOwnerControl() { return designedControl; }
		protected override object GetService(Type service) {
			object resultService = base.GetService(service);
			if(resultService == null && editingView != null && editingView.Site != null) {
				resultService = editingView.Site.GetService(service);
			}
			return resultService;
		}
	}
	internal class DesignerLayoutTreeView : LayoutTreeView {
		ILayoutControl designedControl;
		TemplateCardCustomizationControl managerCore = null;
		public DesignerLayoutTreeView(TemplateCardCustomizationControl manager)
			: base() {
			this.managerCore = manager;
			this.designedControl = manager.TargetLayoutControl;
			Register();
			ShowHiddenItemsInTreeView = false;
			this.BorderStyle = BorderStyle.None;
		}
		protected override ILayoutControl GetOwnerControl() { return designedControl; }
		protected override void OwnerSelectionChanged(object sender, EventArgs e) {
			BaseLayoutItem item = sender as BaseLayoutItem;
			if(item!=null && item.IsHidden) return;
			base.OwnerSelectionChanged(sender, e);
		}
		protected override void UpdateCoreTreeView(object baseItem, bool includeHiddenItems) {
			if(IsDisposingInProgress) return;
			base.UpdateCoreTreeView(baseItem, includeHiddenItems);
			ResizeByContent();
			managerCore.UpdateLayout();
		}
		protected virtual void ResizeByContent() {
			if(Nodes.Count==0) return;
			int h = GetTreeViewContentHeight(Nodes[0]);
			if(ShowHiddenItemsInTreeView && Nodes.Count>1) h += GetTreeViewContentHeight(Nodes[1]);
			h = Math.Min(h+30, 400);
			MinimumSize = new Size(100, h);
			MaximumSize = new Size(0, h);
		}
		protected override void OnGotFocus(EventArgs e) {
		}
	}
	internal class DesignerHiddenItemsList : HiddenItemsList {
		ILayoutControl designedControl;
		TemplateCardCustomizationControl managerCore = null;
		public DesignerHiddenItemsList(TemplateCardCustomizationControl manager)
			: base() {
			this.managerCore = manager;
			this.designedControl = manager.TargetLayoutControl;
			Register();
			this.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		}
		protected override ILayoutControl GetOwnerControl() { return designedControl; }
		public override void UpdateContent() {
			if(IsDisposing) return;
			base.UpdateContent();
			ResizeByContent();
			managerCore.UpdateLayout();
		}
		protected virtual void ResizeByContent() {
			int h = (ItemCount+2)*ItemHeight;
			h = Math.Min(h, 400);
			MinimumSize = new Size(100, h);
			MaximumSize = new Size(0, h);
		}
		protected override DevExpress.XtraEditors.Drawing.BaseControlPainter CreatePainter() {
			return new DesignerHiddenItemsListPainter();
		}
	}
	internal class DesignerHiddenItemsListPainter : BaseListBoxPainter {
		protected override void DrawBorder(ControlGraphicsInfoArgs info) {
		}
	}
}
