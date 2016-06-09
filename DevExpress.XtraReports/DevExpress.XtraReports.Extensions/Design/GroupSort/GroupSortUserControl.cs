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
using System.Text;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.Design.Tools;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Localization;
using System.Windows.Forms;
using System.ComponentModel;
using System.Windows.Forms.Design;
using DevExpress.XtraBars;
using System.Drawing;
using DevExpress.LookAndFeel.DesignService;
namespace DevExpress.XtraReports.Design.GroupSort {
	[System.ComponentModel.ToolboxItem(false)]
	public class GroupSortUserControl : XtraUserControl, ISupportController {
		private System.ComponentModel.IContainer components = null;
		GroupSortTreeList groupSortTreeList;
		DevExpress.XtraBars.BarManager barManager;
		const int barButtonIdAddGroup = 0;
		const int barButtonIdAddSort = 1;
		const int barButtonIdMoveUp = 2;
		const int barButtonIdMoveDown = 3;
		const int barButtonIdDelete = 4;
		public GroupSortUserControl() {
			groupSortTreeList = new GroupSortTreeList();
			groupSortTreeList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Controls.Add(groupSortTreeList);
			InitializeBar();
			groupSortTreeList.MenuManager = barManager;
			UpdateButtonsEnabled();
			groupSortTreeList.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(groupSortTreeList_FocusedNodeChanged);
			groupSortTreeList.NodesReloaded += new EventHandler(groupSortTreeList_NodesReloaded);
			groupSortTreeList.CustomDrawEmptyArea += new DevExpress.XtraTreeList.CustomDrawEmptyAreaEventHandler(groupSortTreeList_CustomDrawEmptyArea);
		}
		public TreeListController ActiveController {
			get {
				return groupSortTreeList.ActiveController;
			}
			set {
				groupSortTreeList.ActiveController = value;
			}
		}
		GroupSortController GroupSortController {
			get {
				return (GroupSortController)ActiveController;
			}
		}
		internal DevExpress.XtraTreeList.TreeList GroupSortTreeList {
			get { 
				return this.groupSortTreeList; 
			}
		}
		public void SetLookAndFeel(IServiceProvider serviceProvider) {
			DesignLookAndFeelHelper.SetParentLookAndFeel(this.groupSortTreeList, serviceProvider);
			DesignLookAndFeelHelper.SetLookAndFeel(this.barManager, serviceProvider);
		}
		void InitializeBar() {
			this.components = new System.ComponentModel.Container();
			DevExpress.XtraBars.BarItemAppearance barItemAppearance = new DevExpress.XtraBars.BarItemAppearance();
			barManager = new RuntimeBarManager(this.components);
			DevExpress.XtraBars.Bar bar = new DevExpress.XtraBars.Bar();
			DevExpress.XtraBars.BarButtonItem barButtonAddGroup = new DevExpress.XtraBars.BarButtonItem();
			DevExpress.XtraBars.BarButtonItem barButtonAddSort = new DevExpress.XtraBars.BarButtonItem();
			DevExpress.XtraBars.BarButtonItem barButtonMoveUp = new DevExpress.XtraBars.BarButtonItem();
			DevExpress.XtraBars.BarButtonItem barButtonMoveDown = new DevExpress.XtraBars.BarButtonItem();
			DevExpress.XtraBars.BarButtonItem barButtonDelete = new DevExpress.XtraBars.BarButtonItem();
			((System.ComponentModel.ISupportInitialize)(barManager)).BeginInit();
			barManager.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
			bar});
			barManager.AllowCustomization = false;
			barManager.AllowShowToolbarsPopup = false;
			barManager.Form = this;
			barManager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
			barButtonAddGroup,
			barButtonAddSort,
			barButtonMoveUp,
			barButtonMoveDown,
			barButtonDelete});
			barManager.MaxItemId = 4;
			barManager.Controller = new BarAndDockingController();
			barManager.Controller.LookAndFeel.ParentLookAndFeel = this.LookAndFeel;
			bar.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Top;
			bar.BarName = "Tools";
			bar.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
			bar.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(barButtonAddGroup),
			new DevExpress.XtraBars.LinkPersistInfo(barButtonAddSort),
			new DevExpress.XtraBars.LinkPersistInfo(barButtonDelete),
			new DevExpress.XtraBars.LinkPersistInfo(barButtonMoveUp, true),
			new DevExpress.XtraBars.LinkPersistInfo(barButtonMoveDown)
			});
			bar.OptionsBar.AllowQuickCustomization = false;
			bar.OptionsBar.DrawDragBorder = false;
			bar.OptionsBar.UseWholeRow = true;
			bar.Text = "Tools";
			IntitButtonItem(barButtonAddGroup, barItemAppearance, null, ReportStringId.GroupSort_AddGroup, barButtonIdAddGroup, "AddGroup");
			barButtonAddGroup.ActAsDropDown = true;
			barButtonAddGroup.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
			barButtonAddGroup.DropDownControl = new FieldSelectPopupControlContainer(this, components);
			barButtonAddGroup.DropDownControl.CloseUp += new EventHandler(barButtonAddGroup_DropDownControl_CloseUp);
			IntitButtonItem(barButtonAddSort, barItemAppearance, null, ReportStringId.GroupSort_AddSort, barButtonIdAddSort, "AddSort");
			barButtonAddSort.ActAsDropDown = true;
			barButtonAddSort.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
			barButtonAddSort.DropDownControl = new FieldSelectPopupControlContainer(this, components);
			barButtonAddSort.DropDownControl.CloseUp += new EventHandler(barButtonAddSort_DropDownControl_CloseUp);
			IntitButtonItem(barButtonMoveUp, barItemAppearance, barButtonMoveUp_ItemClick, ReportStringId.GroupSort_MoveUp, barButtonIdMoveUp, "MoveUp");
			IntitButtonItem(barButtonMoveDown, barItemAppearance, barButtonMoveDown_ItemClick, ReportStringId.GroupSort_MoveDown, barButtonIdMoveDown, "MoveDown");
			IntitButtonItem(barButtonDelete, barItemAppearance, barButtonDelete_ItemClick, ReportStringId.GroupSort_Delete, barButtonIdDelete, "Delete");
			((System.ComponentModel.ISupportInitialize)(barManager)).EndInit();
		}
		static void IntitButtonItem(BarButtonItem barButtonItem, BarItemAppearance barItemAppearance, ItemClickEventHandler itemClick, ReportStringId reportStringId, int id, string buttonName) {
			barButtonItem.Appearance.Assign(barItemAppearance);
			barButtonItem.Caption = ReportLocalizer.GetString(reportStringId);
			barButtonItem.Id = id;
			barButtonItem.Glyph = ResLoader.LoadBitmap(string.Concat("Images.GroupAndSort." + buttonName + ".png"), typeof(LocalResFinder), System.Drawing.Color.Empty);
			barButtonItem.Name = buttonName;
			barButtonItem.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
			if(itemClick != null)
				barButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(itemClick);
			DevExpress.Utils.SuperToolTip superToolTip = new DevExpress.Utils.SuperToolTip();
			DevExpress.Utils.ToolTipItem toolTipItem = new DevExpress.Utils.ToolTipItem();
			toolTipItem.Text = ReportLocalizer.GetString(reportStringId);
			superToolTip.Items.Add(toolTipItem);
			barButtonItem.SuperTip = superToolTip;
		}
		void barButtonAddGroup_DropDownControl_CloseUp(object sender, EventArgs e) {
			string fieldName = ((FieldSelectPopupControlContainer)sender).FieldName;
			if(!string.IsNullOrEmpty(fieldName))
				this.GroupSortController.AddGroup(fieldName);
		}
		void barButtonAddSort_DropDownControl_CloseUp(object sender, EventArgs e) {
			string fieldName = ((FieldSelectPopupControlContainer)sender).FieldName;
			if(!string.IsNullOrEmpty(fieldName))
				this.GroupSortController.AddSort(fieldName);
		}
		void barButtonMoveUp_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			this.GroupSortController.MoveSortUp();
		}
		void barButtonMoveDown_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			this.GroupSortController.MoveSortDown();
		}
		void barButtonDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			if(this.GroupSortController.CanDeleteSort())
				this.GroupSortController.DeleteSort();
		}
		void groupSortTreeList_NodesReloaded(object sender, EventArgs e) {
			UpdateButtonsEnabled();
		}
		void groupSortTreeList_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e) {
			UpdateButtonsEnabled();
		}
		void UpdateButtonsEnabled() {
			if(GroupSortController == null || GroupSortController.DesignerHostIsLoading)
				return;
			bool enabled = CanAddGroupSort();
			barManager.Items.FindById(barButtonIdAddGroup).Enabled = enabled && IsAddGroupEnabled();
			barManager.Items.FindById(barButtonIdAddSort).Enabled = enabled && IsAddSortEnabled();
			barManager.Items.FindById(barButtonIdMoveUp).Enabled = groupSortTreeList.Selection.Count > 0 && groupSortTreeList.Selection[0].ParentNode != null && IsMoveSortUpEnabled();
			barManager.Items.FindById(barButtonIdMoveDown).Enabled = groupSortTreeList.Selection.Count > 0 && groupSortTreeList.Selection[0].HasChildren && IsMoveSortDownEnabled();
			barManager.Items.FindById(barButtonIdDelete).Enabled = groupSortTreeList.FocusedNode != null && IsDeleteSortEnabled();
		}
		bool CanAddGroupSort() {
			return GroupSortController != null && GroupSortController.CanAddGroupSort();
		}
		bool IsAddGroupEnabled() {
			return GroupSortController == null || (GroupSortController != null && GroupSortController.IsAddGroupEnabled());
		}
		bool IsAddSortEnabled() {
			return GroupSortController == null || (GroupSortController != null && GroupSortController.IsAddSortEnabled());
		}
		bool IsDeleteSortEnabled() {
			return GroupSortController == null || (GroupSortController != null && GroupSortController.IsDeleteSortEnabled());
		}
		bool IsMoveSortUpEnabled() {
			return GroupSortController == null || (GroupSortController != null && GroupSortController.IsMoveSortUpEnabled());
		}
		bool IsMoveSortDownEnabled() {
			return GroupSortController == null || (GroupSortController != null && GroupSortController.IsMoveSortDownEnabled());
		}
		void groupSortTreeList_CustomDrawEmptyArea(object sender, DevExpress.XtraTreeList.CustomDrawEmptyAreaEventArgs e) {
			try {
				if(!CanAddGroupSort()) {
					DevExpress.XtraReports.Design.ErrorList.TreeListPaintHelper.DrawString(e, ReportLocalizer.GetString(ReportStringId.Msg_GroupSortNoDataSource));
					e.Handled = true;
				}
			} catch(Exception ex) {
				if(ExceptionHelper.IsCriticalException(ex))
					throw;
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			try {
				if(groupSortTreeList != null) {
					groupSortTreeList.FocusedNodeChanged -= new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(groupSortTreeList_FocusedNodeChanged);
					groupSortTreeList.NodesReloaded -= new EventHandler(groupSortTreeList_NodesReloaded);
					groupSortTreeList.CustomDrawEmptyArea -= new DevExpress.XtraTreeList.CustomDrawEmptyAreaEventHandler(groupSortTreeList_CustomDrawEmptyArea);
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		public TreeListController CreateController(IServiceProvider serviceProvider) {
			return this.groupSortTreeList.CreateController(serviceProvider);
		}
	}
	class FieldSelectPopupControlContainer : DevExpress.XtraBars.PopupControlContainer {
		ISupportController supportController;
		IPopupFieldNamePicker picker;
		string fieldName;
		public string FieldName {
			get { return fieldName; }
		}
		GroupSortController Controller {
			get { return supportController.ActiveController as GroupSortController; }
		}
		public FieldSelectPopupControlContainer(ISupportController supportController, IContainer container) 
			: base(container) {
			this.supportController = supportController;
		}
		protected override void OnPopup() {
			LookAndFeel.ParentLookAndFeel = Manager.GetController().LookAndFeel;
			GroupSortController controller = Controller;
			if(controller != null) {
				picker = controller.CreatePopupFieldNamePicker();
				picker.Start(controller, controller.Report.GetEffectiveDataSource(), controller.Report.DataMember, string.Empty, this);
			}
			fieldName = null;
			base.OnPopup();
			SubControl.Form.LayoutChanged();
		}
		protected override void OnCloseUp(DevExpress.XtraBars.Controls.CustomPopupBarControl prevControl) {
			if(picker != null) {
				fieldName = picker.EndFieldNamePicker();
				picker.Dispose();
			}
			base.OnCloseUp(prevControl);
		}
	}
}
