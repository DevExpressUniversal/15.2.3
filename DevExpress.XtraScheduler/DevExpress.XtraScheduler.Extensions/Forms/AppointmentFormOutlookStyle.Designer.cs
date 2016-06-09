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

namespace DevExpress.XtraScheduler.UI
{
	partial class AppointmentFormOutlookStyle {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AppointmentFormOutlookStyle));
			DevExpress.XtraSpellChecker.OptionsSpelling optionsSpelling1 = new DevExpress.XtraSpellChecker.OptionsSpelling();
			DevExpress.XtraSpellChecker.OptionsSpelling optionsSpelling2 = new DevExpress.XtraSpellChecker.OptionsSpelling();
			DevExpress.XtraSpellChecker.OptionsSpelling optionsSpelling3 = new DevExpress.XtraSpellChecker.OptionsSpelling();
			this.barManager = new DevExpress.XtraBars.BarManager();
			this.brTools = new DevExpress.XtraBars.Bar();
			this.btnSaveAndClose = new DevExpress.XtraBars.BarButtonItem();
			this.btnRecurrence = new DevExpress.XtraBars.BarButtonItem();
			this.btnDelete = new DevExpress.XtraBars.BarButtonItem();
			this.btnSpelling = new DevExpress.XtraBars.BarButtonItem();
			this.brMainMenu = new DevExpress.XtraBars.Bar();
			this.mActions = new DevExpress.XtraBars.BarSubItem();
			this.btnSave = new DevExpress.XtraBars.BarButtonItem();
			this.btnClose = new DevExpress.XtraBars.BarButtonItem();
			this.barAndDockingController = new DevExpress.XtraBars.BarAndDockingController();
			this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
			this.spellChecker = new DevExpress.XtraSpellChecker.SpellChecker();
			this.tbLocation = new DevExpress.XtraEditors.TextEdit();
			this.layoutCtrl = new DevExpress.XtraLayout.LayoutControl();
			this.edtResources = new DevExpress.XtraScheduler.UI.AppointmentResourcesEdit();
			this.lblHorzSeparator1 = new DevExpress.XtraEditors.LabelControl();
			this.lblInfo = new DevExpress.XtraEditors.LabelControl();
			this.lblHorzSeparator2 = new DevExpress.XtraEditors.LabelControl();
			this.tbSubject = new DevExpress.XtraEditors.TextEdit();
			this.edtLabel = new DevExpress.XtraScheduler.UI.AppointmentLabelEdit();
			this.edtStartDate = new DevExpress.XtraEditors.DateEdit();
			this.cbReminder = new DevExpress.XtraScheduler.UI.DurationEdit();
			this.edtResource = new DevExpress.XtraScheduler.UI.AppointmentResourceEdit();
			this.edtShowTimeAs = new DevExpress.XtraScheduler.UI.AppointmentStatusEdit();
			this.edtEndDate = new DevExpress.XtraEditors.DateEdit();
			this.tbDescription = new DevExpress.XtraEditors.MemoEdit();
			this.edtStartTime = new DevExpress.XtraScheduler.UI.SchedulerTimeEdit();
			this.edtEndTime = new DevExpress.XtraScheduler.UI.SchedulerTimeEdit();
			this.chkAllDay = new DevExpress.XtraEditors.CheckEdit();
			this.chkReminder = new DevExpress.XtraEditors.CheckEdit();
			this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutLocation = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutSubject = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutLabel = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutDescription = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutShowTimeAs = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutAllDay = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutInfo = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutReminderGroup = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutChkReminder = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutReminder = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutStartGroup = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutStartDate = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutStartTime = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutEndGroup = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutEndDate = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutEndTime = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutResourcesGroup = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutResources = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutResource = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutHorzSeparator1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutHorzSeparator2 = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.tpAppointment = new DevExpress.XtraTab.XtraTabPage();
			this.tabControl = new DevExpress.XtraTab.XtraTabControl();
			((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbLocation.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutCtrl)).BeginInit();
			this.layoutCtrl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.edtResources.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtResources.ResourcesCheckedListBoxControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbSubject.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtLabel.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtStartDate.Properties.CalendarTimeProperties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtStartDate.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbReminder.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtResource.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtShowTimeAs.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtEndDate.Properties.CalendarTimeProperties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtEndDate.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbDescription.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtStartTime.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtEndTime.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkAllDay.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkReminder.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutLocation)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutSubject)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutLabel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutDescription)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutShowTimeAs)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutAllDay)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutInfo)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutReminderGroup)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutChkReminder)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutReminder)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutStartGroup)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutStartDate)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutStartTime)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutEndGroup)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutEndDate)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutEndTime)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutResourcesGroup)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutResources)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutResource)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutHorzSeparator1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutHorzSeparator2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			this.tpAppointment.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.tabControl)).BeginInit();
			this.tabControl.SuspendLayout();
			this.SuspendLayout();
			this.barManager.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
			this.brTools,
			this.brMainMenu});
			this.barManager.Controller = this.barAndDockingController;
			this.barManager.DockControls.Add(this.barDockControlTop);
			this.barManager.DockControls.Add(this.barDockControlBottom);
			this.barManager.DockControls.Add(this.barDockControlLeft);
			this.barManager.DockControls.Add(this.barDockControlRight);
			this.barManager.Form = this;
			this.barManager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
			this.mActions,
			this.btnSave,
			this.btnClose,
			this.btnSaveAndClose,
			this.btnDelete,
			this.btnRecurrence,
			this.btnSpelling});
			this.barManager.MainMenu = this.brMainMenu;
			this.barManager.MaxItemId = 8;
			this.brTools.BarName = "Tools";
			this.brTools.DockCol = 0;
			this.brTools.DockRow = 1;
			this.brTools.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
			this.brTools.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnSaveAndClose, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnRecurrence, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnDelete, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.Standard),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnSpelling, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)});
			resources.ApplyResources(this.brTools, "brTools");
			resources.ApplyResources(this.btnSaveAndClose, "btnSaveAndClose");
			this.btnSaveAndClose.Id = 4;
			this.btnSaveAndClose.Name = "btnSaveAndClose";
			resources.ApplyResources(this.btnRecurrence, "btnRecurrence");
			this.btnRecurrence.Id = 6;
			this.btnRecurrence.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G));
			this.btnRecurrence.Name = "btnRecurrence";
			resources.ApplyResources(this.btnDelete, "btnDelete");
			this.btnDelete.Id = 5;
			this.btnDelete.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D));
			this.btnDelete.Name = "btnDelete";
			resources.ApplyResources(this.btnSpelling, "btnSpelling");
			this.btnSpelling.Id = 7;
			this.btnSpelling.ItemShortcut = new DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.F7);
			this.btnSpelling.Name = "btnSpelling";
			this.brMainMenu.BarName = "Main menu";
			this.brMainMenu.DockCol = 0;
			this.brMainMenu.DockRow = 0;
			this.brMainMenu.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
			this.brMainMenu.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.mActions)});
			this.brMainMenu.OptionsBar.AllowQuickCustomization = false;
			this.brMainMenu.OptionsBar.MultiLine = true;
			this.brMainMenu.OptionsBar.UseWholeRow = true;
			resources.ApplyResources(this.brMainMenu, "brMainMenu");
			resources.ApplyResources(this.mActions, "mActions");
			this.mActions.Id = 0;
			this.mActions.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.btnSave),
			new DevExpress.XtraBars.LinkPersistInfo(this.btnDelete),
			new DevExpress.XtraBars.LinkPersistInfo(this.btnRecurrence),
			new DevExpress.XtraBars.LinkPersistInfo(this.btnSpelling),
			new DevExpress.XtraBars.LinkPersistInfo(this.btnClose)});
			this.mActions.Name = "mActions";
			resources.ApplyResources(this.btnSave, "btnSave");
			this.btnSave.Id = 2;
			this.btnSave.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
			this.btnSave.Name = "btnSave";
			resources.ApplyResources(this.btnClose, "btnClose");
			this.btnClose.Id = 3;
			this.btnClose.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4));
			this.btnClose.Name = "btnClose";
			this.barAndDockingController.PropertiesBar.DefaultGlyphSize = new System.Drawing.Size(16, 16);
			this.barAndDockingController.PropertiesBar.DefaultLargeGlyphSize = new System.Drawing.Size(32, 32);
			this.barDockControlTop.CausesValidation = false;
			resources.ApplyResources(this.barDockControlTop, "barDockControlTop");
			this.barDockControlBottom.CausesValidation = false;
			resources.ApplyResources(this.barDockControlBottom, "barDockControlBottom");
			this.barDockControlLeft.CausesValidation = false;
			resources.ApplyResources(this.barDockControlLeft, "barDockControlLeft");
			this.barDockControlRight.CausesValidation = false;
			resources.ApplyResources(this.barDockControlRight, "barDockControlRight");
			this.spellChecker.CheckAsYouTypeOptions.CheckControlsInParentContainer = true;
			this.spellChecker.Culture = new System.Globalization.CultureInfo("en-US");
			this.spellChecker.ParentContainer = this;
			this.spellChecker.SpellCheckMode = DevExpress.XtraSpellChecker.SpellCheckMode.AsYouType;
			resources.ApplyResources(this.tbLocation, "tbLocation");
			this.tbLocation.MenuManager = this.barManager;
			this.tbLocation.Name = "tbLocation";
			this.spellChecker.SetShowSpellCheckMenu(this.tbLocation, true);
			this.spellChecker.SetSpellCheckerOptions(this.tbLocation, optionsSpelling1);
			this.tbLocation.StyleController = this.layoutCtrl;
			this.layoutCtrl.Controls.Add(this.edtResources);
			this.layoutCtrl.Controls.Add(this.tbLocation);
			this.layoutCtrl.Controls.Add(this.lblHorzSeparator1);
			this.layoutCtrl.Controls.Add(this.lblInfo);
			this.layoutCtrl.Controls.Add(this.lblHorzSeparator2);
			this.layoutCtrl.Controls.Add(this.tbSubject);
			this.layoutCtrl.Controls.Add(this.edtLabel);
			this.layoutCtrl.Controls.Add(this.edtStartDate);
			this.layoutCtrl.Controls.Add(this.cbReminder);
			this.layoutCtrl.Controls.Add(this.edtResource);
			this.layoutCtrl.Controls.Add(this.edtShowTimeAs);
			this.layoutCtrl.Controls.Add(this.edtEndDate);
			this.layoutCtrl.Controls.Add(this.tbDescription);
			this.layoutCtrl.Controls.Add(this.edtStartTime);
			this.layoutCtrl.Controls.Add(this.edtEndTime);
			this.layoutCtrl.Controls.Add(this.chkAllDay);
			this.layoutCtrl.Controls.Add(this.chkReminder);
			resources.ApplyResources(this.layoutCtrl, "layoutCtrl");
			this.layoutCtrl.MenuManager = this.barManager;
			this.layoutCtrl.Name = "layoutCtrl";
			this.layoutCtrl.Root = this.layoutControlGroup;
			resources.ApplyResources(this.edtResources, "edtResources");
			this.edtResources.MenuManager = this.barManager;
			this.edtResources.Name = "edtResources";
			this.edtResources.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtResources.Properties.Buttons"))))});
			this.edtResources.ResourcesCheckedListBoxControl.Location = ((System.Drawing.Point)(resources.GetObject("edtResources.ResourcesCheckedListBoxControl.Location")));
			this.edtResources.ResourcesCheckedListBoxControl.Name = "";
			this.edtResources.ResourcesCheckedListBoxControl.TabIndex = ((int)(resources.GetObject("edtResources.ResourcesCheckedListBoxControl.TabIndex")));
			this.edtResources.StyleController = this.layoutCtrl;
			this.lblHorzSeparator1.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Horizontal;
			this.lblHorzSeparator1.LineVisible = true;
			resources.ApplyResources(this.lblHorzSeparator1, "lblHorzSeparator1");
			this.lblHorzSeparator1.Name = "lblHorzSeparator1";
			this.lblHorzSeparator1.StyleController = this.layoutCtrl;
			this.lblInfo.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("lblInfo.Appearance.BackColor")));
			resources.ApplyResources(this.lblInfo, "lblInfo");
			this.lblInfo.Name = "lblInfo";
			this.lblInfo.StyleController = this.layoutCtrl;
			this.lblHorzSeparator2.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Horizontal;
			this.lblHorzSeparator2.LineVisible = true;
			resources.ApplyResources(this.lblHorzSeparator2, "lblHorzSeparator2");
			this.lblHorzSeparator2.Name = "lblHorzSeparator2";
			this.lblHorzSeparator2.StyleController = this.layoutCtrl;
			resources.ApplyResources(this.tbSubject, "tbSubject");
			this.tbSubject.MenuManager = this.barManager;
			this.tbSubject.Name = "tbSubject";
			this.spellChecker.SetShowSpellCheckMenu(this.tbSubject, true);
			this.spellChecker.SetSpellCheckerOptions(this.tbSubject, optionsSpelling2);
			this.tbSubject.StyleController = this.layoutCtrl;
			resources.ApplyResources(this.edtLabel, "edtLabel");
			this.edtLabel.MenuManager = this.barManager;
			this.edtLabel.Name = "edtLabel";
			this.edtLabel.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtLabel.Properties.Buttons"))))});
			this.edtLabel.StyleController = this.layoutCtrl;
			resources.ApplyResources(this.edtStartDate, "edtStartDate");
			this.edtStartDate.MenuManager = this.barManager;
			this.edtStartDate.Name = "edtStartDate";
			this.edtStartDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtStartDate.Properties.Buttons"))))});
			this.edtStartDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.edtStartDate.StyleController = this.layoutCtrl;
			resources.ApplyResources(this.cbReminder, "cbReminder");
			this.cbReminder.MenuManager = this.barManager;
			this.cbReminder.Name = "cbReminder";
			this.cbReminder.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbReminder.Properties.Buttons"))))});
			this.cbReminder.Properties.ShowEmptyItem = false;
			this.cbReminder.StyleController = this.layoutCtrl;
			resources.ApplyResources(this.edtResource, "edtResource");
			this.edtResource.MenuManager = this.barManager;
			this.edtResource.Name = "edtResource";
			this.edtResource.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtResource.Properties.Buttons"))))});
			this.edtResource.StyleController = this.layoutCtrl;
			resources.ApplyResources(this.edtShowTimeAs, "edtShowTimeAs");
			this.edtShowTimeAs.MenuManager = this.barManager;
			this.edtShowTimeAs.Name = "edtShowTimeAs";
			this.edtShowTimeAs.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtShowTimeAs.Properties.Buttons"))))});
			this.edtShowTimeAs.StyleController = this.layoutCtrl;
			resources.ApplyResources(this.edtEndDate, "edtEndDate");
			this.edtEndDate.MenuManager = this.barManager;
			this.edtEndDate.Name = "edtEndDate";
			this.edtEndDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtEndDate.Properties.Buttons"))))});
			this.edtEndDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.edtEndDate.StyleController = this.layoutCtrl;
			resources.ApplyResources(this.tbDescription, "tbDescription");
			this.tbDescription.MenuManager = this.barManager;
			this.tbDescription.Name = "tbDescription";
			this.spellChecker.SetShowSpellCheckMenu(this.tbDescription, true);
			this.spellChecker.SetSpellCheckerOptions(this.tbDescription, optionsSpelling3);
			this.tbDescription.StyleController = this.layoutCtrl;
			this.tbDescription.UseOptimizedRendering = true;
			resources.ApplyResources(this.edtStartTime, "edtStartTime");
			this.edtStartTime.MenuManager = this.barManager;
			this.edtStartTime.Name = "edtStartTime";
			this.edtStartTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.edtStartTime.StyleController = this.layoutCtrl;
			resources.ApplyResources(this.edtEndTime, "edtEndTime");
			this.edtEndTime.MenuManager = this.barManager;
			this.edtEndTime.Name = "edtEndTime";
			this.edtEndTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.edtEndTime.StyleController = this.layoutCtrl;
			resources.ApplyResources(this.chkAllDay, "chkAllDay");
			this.chkAllDay.Name = "chkAllDay";
			this.chkAllDay.Properties.Caption = resources.GetString("chkAllDay.Properties.Caption");
			this.chkAllDay.StyleController = this.layoutCtrl;
			resources.ApplyResources(this.chkReminder, "chkReminder");
			this.chkReminder.Name = "chkReminder";
			this.chkReminder.Properties.Caption = resources.GetString("chkReminder.Properties.Caption");
			this.chkReminder.StyleController = this.layoutCtrl;
			resources.ApplyResources(this.layoutControlGroup, "layoutControlGroup");
			this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutLocation,
			this.layoutSubject,
			this.layoutLabel,
			this.layoutDescription,
			this.layoutShowTimeAs,
			this.layoutAllDay,
			this.layoutInfo,
			this.layoutReminderGroup,
			this.layoutStartGroup,
			this.layoutEndGroup,
			this.layoutResourcesGroup,
			this.layoutHorzSeparator1,
			this.layoutHorzSeparator2,
			this.emptySpaceItem1});
			this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup.Name = "layoutControlGroup";
			this.layoutControlGroup.Size = new System.Drawing.Size(546, 341);
			this.layoutControlGroup.TextVisible = false;
			this.layoutLocation.Control = this.tbLocation;
			resources.ApplyResources(this.layoutLocation, "layoutLocation");
			this.layoutLocation.Location = new System.Drawing.Point(0, 42);
			this.layoutLocation.Name = "layoutLocation";
			this.layoutLocation.Size = new System.Drawing.Size(303, 24);
			this.layoutLocation.TextSize = new System.Drawing.Size(67, 13);
			this.layoutSubject.Control = this.tbSubject;
			resources.ApplyResources(this.layoutSubject, "layoutSubject");
			this.layoutSubject.Location = new System.Drawing.Point(0, 18);
			this.layoutSubject.Name = "layoutSubject";
			this.layoutSubject.Size = new System.Drawing.Size(526, 24);
			this.layoutSubject.TextSize = new System.Drawing.Size(67, 13);
			this.layoutLabel.Control = this.edtLabel;
			resources.ApplyResources(this.layoutLabel, "layoutLabel");
			this.layoutLabel.Location = new System.Drawing.Point(303, 42);
			this.layoutLabel.Name = "layoutLabel";
			this.layoutLabel.Size = new System.Drawing.Size(223, 24);
			this.layoutLabel.TextSize = new System.Drawing.Size(67, 13);
			this.layoutDescription.Control = this.tbDescription;
			resources.ApplyResources(this.layoutDescription, "layoutDescription");
			this.layoutDescription.Location = new System.Drawing.Point(0, 196);
			this.layoutDescription.Name = "layoutDescription";
			this.layoutDescription.Size = new System.Drawing.Size(526, 125);
			this.layoutDescription.TextSize = new System.Drawing.Size(0, 0);
			this.layoutDescription.TextToControlDistance = 0;
			this.layoutDescription.TextVisible = false;
			this.layoutShowTimeAs.Control = this.edtShowTimeAs;
			resources.ApplyResources(this.layoutShowTimeAs, "layoutShowTimeAs");
			this.layoutShowTimeAs.Location = new System.Drawing.Point(0, 172);
			this.layoutShowTimeAs.Name = "layoutShowTimeAs";
			this.layoutShowTimeAs.Size = new System.Drawing.Size(303, 24);
			this.layoutShowTimeAs.TextSize = new System.Drawing.Size(67, 13);
			this.layoutAllDay.Control = this.chkAllDay;
			resources.ApplyResources(this.layoutAllDay, "layoutAllDay");
			this.layoutAllDay.Location = new System.Drawing.Point(303, 131);
			this.layoutAllDay.Name = "layoutAllDay";
			this.layoutAllDay.Size = new System.Drawing.Size(102, 24);
			this.layoutAllDay.TextSize = new System.Drawing.Size(0, 0);
			this.layoutAllDay.TextToControlDistance = 0;
			this.layoutAllDay.TextVisible = false;
			this.layoutInfo.Control = this.lblInfo;
			resources.ApplyResources(this.layoutInfo, "layoutInfo");
			this.layoutInfo.Location = new System.Drawing.Point(0, 0);
			this.layoutInfo.Name = "layoutInfo";
			this.layoutInfo.Size = new System.Drawing.Size(526, 18);
			this.layoutInfo.TextLocation = DevExpress.Utils.Locations.Top;
			this.layoutInfo.TextSize = new System.Drawing.Size(0, 0);
			this.layoutInfo.TextToControlDistance = 0;
			this.layoutInfo.TextVisible = false;
			resources.ApplyResources(this.layoutReminderGroup, "layoutReminderGroup");
			this.layoutReminderGroup.GroupBordersVisible = false;
			this.layoutReminderGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutChkReminder,
			this.layoutReminder});
			this.layoutReminderGroup.Location = new System.Drawing.Point(303, 172);
			this.layoutReminderGroup.Name = "layoutReminderGroup";
			this.layoutReminderGroup.Size = new System.Drawing.Size(223, 24);
			this.layoutChkReminder.Control = this.chkReminder;
			resources.ApplyResources(this.layoutChkReminder, "layoutChkReminder");
			this.layoutChkReminder.Location = new System.Drawing.Point(0, 0);
			this.layoutChkReminder.Name = "layoutChkReminder";
			this.layoutChkReminder.Size = new System.Drawing.Size(84, 24);
			this.layoutChkReminder.TextSize = new System.Drawing.Size(0, 0);
			this.layoutChkReminder.TextToControlDistance = 0;
			this.layoutChkReminder.TextVisible = false;
			this.layoutReminder.Control = this.cbReminder;
			resources.ApplyResources(this.layoutReminder, "layoutReminder");
			this.layoutReminder.Location = new System.Drawing.Point(84, 0);
			this.layoutReminder.Name = "layoutReminder";
			this.layoutReminder.Size = new System.Drawing.Size(139, 24);
			this.layoutReminder.TextSize = new System.Drawing.Size(0, 0);
			this.layoutReminder.TextToControlDistance = 0;
			this.layoutReminder.TextVisible = false;
			resources.ApplyResources(this.layoutStartGroup, "layoutStartGroup");
			this.layoutStartGroup.GroupBordersVisible = false;
			this.layoutStartGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutStartDate,
			this.layoutStartTime});
			this.layoutStartGroup.Location = new System.Drawing.Point(0, 83);
			this.layoutStartGroup.Name = "layoutStartGroup";
			this.layoutStartGroup.Size = new System.Drawing.Size(303, 48);
			this.layoutStartDate.Control = this.edtStartDate;
			resources.ApplyResources(this.layoutStartDate, "layoutStartDate");
			this.layoutStartDate.Location = new System.Drawing.Point(0, 0);
			this.layoutStartDate.Name = "layoutStartDate";
			this.layoutStartDate.Size = new System.Drawing.Size(206, 48);
			this.layoutStartDate.TextSize = new System.Drawing.Size(67, 13);
			this.layoutStartTime.Control = this.edtStartTime;
			resources.ApplyResources(this.layoutStartTime, "layoutStartTime");
			this.layoutStartTime.Location = new System.Drawing.Point(206, 0);
			this.layoutStartTime.Name = "layoutStartTime";
			this.layoutStartTime.Size = new System.Drawing.Size(97, 48);
			this.layoutStartTime.TextSize = new System.Drawing.Size(0, 0);
			this.layoutStartTime.TextToControlDistance = 0;
			this.layoutStartTime.TextVisible = false;
			resources.ApplyResources(this.layoutEndGroup, "layoutEndGroup");
			this.layoutEndGroup.GroupBordersVisible = false;
			this.layoutEndGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutEndDate,
			this.layoutEndTime});
			this.layoutEndGroup.Location = new System.Drawing.Point(0, 131);
			this.layoutEndGroup.Name = "layoutEndGroup";
			this.layoutEndGroup.Size = new System.Drawing.Size(303, 24);
			this.layoutEndDate.Control = this.edtEndDate;
			resources.ApplyResources(this.layoutEndDate, "layoutEndDate");
			this.layoutEndDate.Location = new System.Drawing.Point(0, 0);
			this.layoutEndDate.Name = "layoutEndDate";
			this.layoutEndDate.Size = new System.Drawing.Size(206, 24);
			this.layoutEndDate.TextSize = new System.Drawing.Size(67, 13);
			this.layoutEndTime.Control = this.edtEndTime;
			resources.ApplyResources(this.layoutEndTime, "layoutEndTime");
			this.layoutEndTime.Location = new System.Drawing.Point(206, 0);
			this.layoutEndTime.Name = "layoutEndTime";
			this.layoutEndTime.Size = new System.Drawing.Size(97, 24);
			this.layoutEndTime.TextSize = new System.Drawing.Size(0, 0);
			this.layoutEndTime.TextToControlDistance = 0;
			this.layoutEndTime.TextVisible = false;
			resources.ApplyResources(this.layoutResourcesGroup, "layoutResourcesGroup");
			this.layoutResourcesGroup.GroupBordersVisible = false;
			this.layoutResourcesGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutResources,
			this.layoutResource});
			this.layoutResourcesGroup.Location = new System.Drawing.Point(303, 83);
			this.layoutResourcesGroup.Name = "layoutResourcesGroup";
			this.layoutResourcesGroup.Size = new System.Drawing.Size(223, 48);
			this.layoutResources.Control = this.edtResources;
			resources.ApplyResources(this.layoutResources, "layoutResources");
			this.layoutResources.Location = new System.Drawing.Point(0, 0);
			this.layoutResources.Name = "layoutResources";
			this.layoutResources.Size = new System.Drawing.Size(223, 24);
			this.layoutResources.TextSize = new System.Drawing.Size(67, 13);
			this.layoutResource.Control = this.edtResource;
			resources.ApplyResources(this.layoutResource, "layoutResource");
			this.layoutResource.Location = new System.Drawing.Point(0, 24);
			this.layoutResource.Name = "layoutResource";
			this.layoutResource.Size = new System.Drawing.Size(223, 24);
			this.layoutResource.TextSize = new System.Drawing.Size(67, 13);
			this.layoutHorzSeparator1.Control = this.lblHorzSeparator1;
			resources.ApplyResources(this.layoutHorzSeparator1, "layoutHorzSeparator1");
			this.layoutHorzSeparator1.Location = new System.Drawing.Point(0, 66);
			this.layoutHorzSeparator1.Name = "layoutHorzSeparator1";
			this.layoutHorzSeparator1.Size = new System.Drawing.Size(526, 17);
			this.layoutHorzSeparator1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutHorzSeparator1.TextToControlDistance = 0;
			this.layoutHorzSeparator1.TextVisible = false;
			this.layoutHorzSeparator2.Control = this.lblHorzSeparator2;
			resources.ApplyResources(this.layoutHorzSeparator2, "layoutHorzSeparator2");
			this.layoutHorzSeparator2.Location = new System.Drawing.Point(0, 155);
			this.layoutHorzSeparator2.Name = "layoutHorzSeparator2";
			this.layoutHorzSeparator2.Size = new System.Drawing.Size(526, 17);
			this.layoutHorzSeparator2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutHorzSeparator2.TextToControlDistance = 0;
			this.layoutHorzSeparator2.TextVisible = false;
			this.emptySpaceItem1.AllowHotTrack = false;
			resources.ApplyResources(this.emptySpaceItem1, "emptySpaceItem1");
			this.emptySpaceItem1.Location = new System.Drawing.Point(405, 131);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Size = new System.Drawing.Size(121, 24);
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			this.tpAppointment.Controls.Add(this.layoutCtrl);
			this.tpAppointment.Name = "tpAppointment";
			resources.ApplyResources(this.tpAppointment, "tpAppointment");
			resources.ApplyResources(this.tabControl, "tabControl");
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedTabPage = this.tpAppointment;
			this.tabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.tpAppointment});
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.barDockControlLeft);
			this.Controls.Add(this.barDockControlRight);
			this.Controls.Add(this.barDockControlBottom);
			this.Controls.Add(this.barDockControlTop);
			this.Name = "AppointmentFormOutlookStyle";
			this.ShowInTaskbar = false;
			this.Activated += new System.EventHandler(this.AppointmentFormOutlookStyle_Activated);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AppointmentFormOutlookStyle_FormClosing);
			this.Load += new System.EventHandler(this.AppointmentFormOutlookStyle_Load);
			((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbLocation.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutCtrl)).EndInit();
			this.layoutCtrl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.edtResources.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtResources.ResourcesCheckedListBoxControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbSubject.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtLabel.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtStartDate.Properties.CalendarTimeProperties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtStartDate.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbReminder.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtResource.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtShowTimeAs.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtEndDate.Properties.CalendarTimeProperties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtEndDate.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbDescription.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtStartTime.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtEndTime.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkAllDay.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkReminder.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutLocation)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutSubject)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutLabel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutDescription)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutShowTimeAs)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutAllDay)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutInfo)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutReminderGroup)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutChkReminder)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutReminder)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutStartGroup)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutStartDate)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutStartTime)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutEndGroup)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutEndDate)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutEndTime)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutResourcesGroup)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutResources)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutResource)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutHorzSeparator1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutHorzSeparator2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			this.tpAppointment.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.tabControl)).EndInit();
			this.tabControl.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		protected DevExpress.XtraSpellChecker.SpellChecker spellChecker;
		protected DevExpress.XtraTab.XtraTabControl tabControl;
		protected DevExpress.XtraTab.XtraTabPage tpAppointment;
		protected internal DevExpress.XtraBars.BarAndDockingController barAndDockingController;
		protected DevExpress.XtraEditors.CheckEdit chkReminder;
		protected DevExpress.XtraEditors.CheckEdit chkAllDay;
		protected DevExpress.XtraEditors.DateEdit edtEndDate;
		protected DevExpress.XtraEditors.DateEdit edtStartDate;
		protected DevExpress.XtraScheduler.UI.SchedulerTimeEdit edtStartTime;
		protected DevExpress.XtraScheduler.UI.SchedulerTimeEdit edtEndTime;
		protected DevExpress.XtraScheduler.UI.AppointmentLabelEdit edtLabel;
		protected DevExpress.XtraScheduler.UI.AppointmentStatusEdit edtShowTimeAs;
		protected DevExpress.XtraEditors.TextEdit tbSubject;
		protected DevExpress.XtraEditors.TextEdit tbLocation;
		protected DevExpress.XtraScheduler.UI.AppointmentResourcesEdit edtResources;
		protected DevExpress.XtraScheduler.UI.AppointmentResourceEdit edtResource;
		protected DevExpress.XtraEditors.MemoEdit tbDescription;
		protected DevExpress.XtraScheduler.UI.DurationEdit cbReminder;
		protected DevExpress.XtraEditors.LabelControl lblInfo;
		protected DevExpress.XtraLayout.LayoutControl layoutCtrl;
		protected DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
		protected DevExpress.XtraLayout.LayoutControlItem layoutResources;
		protected DevExpress.XtraLayout.LayoutControlItem layoutLocation;
		protected DevExpress.XtraLayout.LayoutControlItem layoutSubject;
		protected DevExpress.XtraLayout.LayoutControlItem layoutLabel;
		protected DevExpress.XtraLayout.LayoutControlItem layoutEndDate;
		protected DevExpress.XtraLayout.LayoutControlItem layoutStartDate;
		protected DevExpress.XtraLayout.LayoutControlItem layoutReminder;
		protected DevExpress.XtraLayout.LayoutControlItem layoutDescription;
		protected DevExpress.XtraLayout.LayoutControlItem layoutShowTimeAs;
		protected DevExpress.XtraLayout.LayoutControlItem layoutStartTime;
		protected DevExpress.XtraLayout.LayoutControlItem layoutEndTime;
		protected DevExpress.XtraLayout.LayoutControlItem layoutChkReminder;
		protected DevExpress.XtraLayout.LayoutControlItem layoutAllDay;
		protected DevExpress.XtraLayout.LayoutControlItem layoutInfo;
		protected DevExpress.XtraLayout.LayoutControlItem layoutResource;
		protected DevExpress.XtraLayout.LayoutControlGroup layoutReminderGroup;
		protected DevExpress.XtraLayout.LayoutControlGroup layoutStartGroup;
		protected DevExpress.XtraLayout.LayoutControlGroup layoutEndGroup;
		protected DevExpress.XtraLayout.LayoutControlGroup layoutResourcesGroup;
		protected DevExpress.XtraEditors.LabelControl lblHorzSeparator1;
		protected DevExpress.XtraLayout.LayoutControlItem layoutHorzSeparator1;
		protected DevExpress.XtraEditors.LabelControl lblHorzSeparator2;
		protected DevExpress.XtraLayout.LayoutControlItem layoutHorzSeparator2;
		protected DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
		protected DevExpress.XtraBars.BarManager barManager;
		protected DevExpress.XtraBars.Bar brMainMenu;
		protected DevExpress.XtraBars.BarDockControl barDockControlTop;
		protected DevExpress.XtraBars.BarDockControl barDockControlBottom;
		protected DevExpress.XtraBars.BarDockControl barDockControlLeft;
		protected DevExpress.XtraBars.BarDockControl barDockControlRight;
		protected DevExpress.XtraBars.Bar brTools;
		protected DevExpress.XtraBars.BarSubItem mActions;
		protected DevExpress.XtraBars.BarButtonItem btnSave;
		protected DevExpress.XtraBars.BarButtonItem btnClose;
		protected DevExpress.XtraBars.BarButtonItem btnSaveAndClose;
		protected DevExpress.XtraBars.BarButtonItem btnDelete;
		protected DevExpress.XtraBars.BarButtonItem btnRecurrence;
		protected DevExpress.XtraBars.BarButtonItem btnSpelling;
	}
}
