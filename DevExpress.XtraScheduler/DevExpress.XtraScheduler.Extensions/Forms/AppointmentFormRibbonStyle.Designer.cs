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

namespace DevExpress.XtraScheduler.UI {
	partial class AppointmentFormRibbonStyle {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AppointmentFormRibbonStyle));
			DevExpress.XtraSpellChecker.OptionsSpelling optionsSpelling1 = new DevExpress.XtraSpellChecker.OptionsSpelling();
			DevExpress.XtraSpellChecker.OptionsSpelling optionsSpelling2 = new DevExpress.XtraSpellChecker.OptionsSpelling();
			DevExpress.XtraSpellChecker.OptionsSpelling optionsSpelling3 = new DevExpress.XtraSpellChecker.OptionsSpelling();
			this.ribPageAppointment = new DevExpress.XtraBars.Ribbon.RibbonPage();
			this.pgrActions = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.btnSaveAndClose = new DevExpress.XtraBars.BarButtonItem();
			this.btnDelete = new DevExpress.XtraBars.BarButtonItem();
			this.pgrOptions = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.barEditShowTimeAs = new DevExpress.XtraBars.BarEditItem();
			this.repItemAppointmentStatus = new DevExpress.XtraScheduler.UI.RepositoryItemAppointmentStatus();
			this.barEditLabelAs = new DevExpress.XtraBars.BarEditItem();
			this.repItemAppointmentLabel = new DevExpress.XtraScheduler.UI.RepositoryItemAppointmentLabel();
			this.btnRecurrence = new DevExpress.XtraBars.BarButtonItem();
			this.pgrProofing = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.btnSpelling = new DevExpress.XtraBars.BarButtonItem();
			this.btnClose = new DevExpress.XtraBars.BarButtonItem();
			this.btnSave = new DevExpress.XtraBars.BarButtonItem();
			this.ribbonControl = new DevExpress.XtraBars.Ribbon.RibbonControl();
			this.applicationMenu = new DevExpress.XtraBars.Ribbon.ApplicationMenu();
			this.barAndDockingController = new DevExpress.XtraBars.BarAndDockingController();
			this.spellChecker = new DevExpress.XtraSpellChecker.SpellChecker();
			this.tbLocation = new DevExpress.XtraEditors.TextEdit();
			this.layoutCtrl = new DevExpress.XtraLayout.LayoutControl();
			this.edtResources = new DevExpress.XtraScheduler.UI.AppointmentResourcesEdit();
			this.lblHorzSeparator1 = new DevExpress.XtraEditors.LabelControl();
			this.lblInfo = new DevExpress.XtraEditors.LabelControl();
			this.lblHorzSeparator2 = new DevExpress.XtraEditors.LabelControl();
			this.tbSubject = new DevExpress.XtraEditors.TextEdit();
			this.edtStartDate = new DevExpress.XtraEditors.DateEdit();
			this.cbReminder = new DevExpress.XtraScheduler.UI.DurationEdit();
			this.edtResource = new DevExpress.XtraScheduler.UI.AppointmentResourceEdit();
			this.edtEndDate = new DevExpress.XtraEditors.DateEdit();
			this.tbDescription = new DevExpress.XtraEditors.MemoEdit();
			this.edtStartTime = new DevExpress.XtraScheduler.UI.SchedulerTimeEdit();
			this.edtEndTime = new DevExpress.XtraScheduler.UI.SchedulerTimeEdit();
			this.chkAllDay = new DevExpress.XtraEditors.CheckEdit();
			this.chkReminder = new DevExpress.XtraEditors.CheckEdit();
			this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutLocation = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutSubject = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutDescription = new DevExpress.XtraLayout.LayoutControlItem();
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
			((System.ComponentModel.ISupportInitialize)(this.repItemAppointmentStatus)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repItemAppointmentLabel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ribbonControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.applicationMenu)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbLocation.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutCtrl)).BeginInit();
			this.layoutCtrl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.edtResources.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtResources.ResourcesCheckedListBoxControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbSubject.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtStartDate.Properties.CalendarTimeProperties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtStartDate.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbReminder.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtResource.Properties)).BeginInit();
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
			((System.ComponentModel.ISupportInitialize)(this.layoutDescription)).BeginInit();
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
			this.SuspendLayout();
			this.ribPageAppointment.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
			this.pgrActions,
			this.pgrOptions,
			this.pgrProofing});
			this.ribPageAppointment.KeyTip = "AP";
			this.ribPageAppointment.Name = "ribPageAppointment";
			resources.ApplyResources(this.ribPageAppointment, "ribPageAppointment");
			this.pgrActions.ItemLinks.Add(this.btnSaveAndClose);
			this.pgrActions.ItemLinks.Add(this.btnDelete);
			this.pgrActions.KeyTip = "A";
			this.pgrActions.Name = "pgrActions";
			this.pgrActions.ShowCaptionButton = false;
			resources.ApplyResources(this.pgrActions, "pgrActions");
			resources.ApplyResources(this.btnSaveAndClose, "btnSaveAndClose");
			this.btnSaveAndClose.Id = 4;
			this.btnSaveAndClose.Name = "btnSaveAndClose";
			this.btnSaveAndClose.RibbonStyle = ((DevExpress.XtraBars.Ribbon.RibbonItemStyles)(((DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large | DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText) 
			| DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText)));
			this.btnSaveAndClose.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnSaveAndClose_Click);
			resources.ApplyResources(this.btnDelete, "btnDelete");
			this.btnDelete.Id = 5;
			this.btnDelete.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D));
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.RibbonStyle = ((DevExpress.XtraBars.Ribbon.RibbonItemStyles)(((DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large | DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText) 
			| DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText)));
			this.btnDelete.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnDelete_Click);
			this.pgrOptions.ItemLinks.Add(this.barEditShowTimeAs);
			this.pgrOptions.ItemLinks.Add(this.barEditLabelAs);
			this.pgrOptions.ItemLinks.Add(this.btnRecurrence);
			this.pgrOptions.KeyTip = "O";
			this.pgrOptions.Name = "pgrOptions";
			this.pgrOptions.ShowCaptionButton = false;
			resources.ApplyResources(this.pgrOptions, "pgrOptions");
			resources.ApplyResources(this.barEditShowTimeAs, "barEditShowTimeAs");
			this.barEditShowTimeAs.Edit = this.repItemAppointmentStatus;
			this.barEditShowTimeAs.Id = 10;
			this.barEditShowTimeAs.Name = "barEditShowTimeAs";
			resources.ApplyResources(this.repItemAppointmentStatus, "repItemAppointmentStatus");
			this.repItemAppointmentStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repItemAppointmentStatus.Buttons"))))});
			this.repItemAppointmentStatus.Name = "repItemAppointmentStatus";
			resources.ApplyResources(this.barEditLabelAs, "barEditLabelAs");
			this.barEditLabelAs.Edit = this.repItemAppointmentLabel;
			this.barEditLabelAs.Id = 11;
			this.barEditLabelAs.Name = "barEditLabelAs";
			resources.ApplyResources(this.repItemAppointmentLabel, "repItemAppointmentLabel");
			this.repItemAppointmentLabel.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repItemAppointmentLabel.Buttons"))))});
			this.repItemAppointmentLabel.Name = "repItemAppointmentLabel";
			resources.ApplyResources(this.btnRecurrence, "btnRecurrence");
			this.btnRecurrence.Id = 6;
			this.btnRecurrence.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G));
			this.btnRecurrence.Name = "btnRecurrence";
			this.btnRecurrence.RibbonStyle = ((DevExpress.XtraBars.Ribbon.RibbonItemStyles)(((DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large | DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText) 
			| DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText)));
			this.btnRecurrence.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnRecurrence_Click);
			this.pgrProofing.ItemLinks.Add(this.btnSpelling);
			this.pgrProofing.KeyTip = "P";
			this.pgrProofing.Name = "pgrProofing";
			this.pgrProofing.ShowCaptionButton = false;
			resources.ApplyResources(this.pgrProofing, "pgrProofing");
			resources.ApplyResources(this.btnSpelling, "btnSpelling");
			this.btnSpelling.Id = 7;
			this.btnSpelling.ItemShortcut = new DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.F7);
			this.btnSpelling.Name = "btnSpelling";
			this.btnSpelling.RibbonStyle = ((DevExpress.XtraBars.Ribbon.RibbonItemStyles)(((DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large | DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText) 
			| DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText)));
			this.btnSpelling.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnSpelling_Click);
			resources.ApplyResources(this.btnClose, "btnClose");
			this.btnClose.Id = 3;
			this.btnClose.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4));
			this.btnClose.Name = "btnClose";
			this.btnClose.RibbonStyle = ((DevExpress.XtraBars.Ribbon.RibbonItemStyles)(((DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large | DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText) 
			| DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText)));
			this.btnClose.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnClose_Click);
			resources.ApplyResources(this.btnSave, "btnSave");
			this.btnSave.Id = 2;
			this.btnSave.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
			this.btnSave.Name = "btnSave";
			this.btnSave.RibbonStyle = ((DevExpress.XtraBars.Ribbon.RibbonItemStyles)(((DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large | DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText) 
			| DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithoutText)));
			this.btnSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnSave_Click);
			resources.ApplyResources(this.ribbonControl, "ribbonControl");
			this.ribbonControl.ApplicationButtonDropDownControl = this.applicationMenu;
			this.ribbonControl.AutoSizeItems = true;
			this.ribbonControl.Controller = this.barAndDockingController;
			this.ribbonControl.ExpandCollapseItem.Id = 0;
			this.ribbonControl.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
			this.ribbonControl.ExpandCollapseItem,
			this.btnSave,
			this.btnClose,
			this.btnSaveAndClose,
			this.btnDelete,
			this.btnRecurrence,
			this.btnSpelling,
			this.barEditShowTimeAs,
			this.barEditLabelAs});
			this.ribbonControl.ItemsVertAlign = DevExpress.Utils.VertAlignment.Top;
			this.ribbonControl.MaxItemId = 16;
			this.ribbonControl.Name = "ribbonControl";
			this.ribbonControl.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
			this.ribPageAppointment});
			this.ribbonControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
			this.repItemAppointmentStatus,
			this.repItemAppointmentLabel});
			this.ribbonControl.Toolbar.ItemLinks.Add(this.btnSave);
			this.applicationMenu.ItemLinks.Add(this.btnSave);
			this.applicationMenu.ItemLinks.Add(this.btnDelete);
			this.applicationMenu.ItemLinks.Add(this.btnClose);
			this.applicationMenu.MenuDrawMode = DevExpress.XtraBars.MenuDrawMode.LargeImagesText;
			this.applicationMenu.Name = "applicationMenu";
			this.applicationMenu.Ribbon = this.ribbonControl;
			this.barAndDockingController.PropertiesBar.DefaultGlyphSize = new System.Drawing.Size(16, 16);
			this.barAndDockingController.PropertiesBar.DefaultLargeGlyphSize = new System.Drawing.Size(32, 32);
			this.spellChecker.CheckAsYouTypeOptions.CheckControlsInParentContainer = true;
			this.spellChecker.Culture = new System.Globalization.CultureInfo("en-US");
			this.spellChecker.ParentContainer = this;
			this.spellChecker.SpellCheckMode = DevExpress.XtraSpellChecker.SpellCheckMode.AsYouType;
			resources.ApplyResources(this.tbLocation, "tbLocation");
			this.tbLocation.MenuManager = this.ribbonControl;
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
			this.layoutCtrl.Controls.Add(this.edtStartDate);
			this.layoutCtrl.Controls.Add(this.cbReminder);
			this.layoutCtrl.Controls.Add(this.edtResource);
			this.layoutCtrl.Controls.Add(this.edtEndDate);
			this.layoutCtrl.Controls.Add(this.tbDescription);
			this.layoutCtrl.Controls.Add(this.edtStartTime);
			this.layoutCtrl.Controls.Add(this.edtEndTime);
			this.layoutCtrl.Controls.Add(this.chkAllDay);
			this.layoutCtrl.Controls.Add(this.chkReminder);
			resources.ApplyResources(this.layoutCtrl, "layoutCtrl");
			this.layoutCtrl.Name = "layoutCtrl";
			this.layoutCtrl.Root = this.layoutControlGroup;
			resources.ApplyResources(this.edtResources, "edtResources");
			this.edtResources.MenuManager = this.ribbonControl;
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
			this.tbSubject.MenuManager = this.ribbonControl;
			this.tbSubject.Name = "tbSubject";
			this.spellChecker.SetShowSpellCheckMenu(this.tbSubject, true);
			this.spellChecker.SetSpellCheckerOptions(this.tbSubject, optionsSpelling2);
			this.tbSubject.StyleController = this.layoutCtrl;
			resources.ApplyResources(this.edtStartDate, "edtStartDate");
			this.edtStartDate.MenuManager = this.ribbonControl;
			this.edtStartDate.Name = "edtStartDate";
			this.edtStartDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtStartDate.Properties.Buttons"))))});
			this.edtStartDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.edtStartDate.StyleController = this.layoutCtrl;
			resources.ApplyResources(this.cbReminder, "cbReminder");
			this.cbReminder.MenuManager = this.ribbonControl;
			this.cbReminder.Name = "cbReminder";
			this.cbReminder.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbReminder.Properties.Buttons"))))});
			this.cbReminder.Properties.ShowEmptyItem = false;
			this.cbReminder.StyleController = this.layoutCtrl;
			resources.ApplyResources(this.edtResource, "edtResource");
			this.edtResource.MenuManager = this.ribbonControl;
			this.edtResource.Name = "edtResource";
			this.edtResource.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtResource.Properties.Buttons"))))});
			this.edtResource.StyleController = this.layoutCtrl;
			resources.ApplyResources(this.edtEndDate, "edtEndDate");
			this.edtEndDate.MenuManager = this.ribbonControl;
			this.edtEndDate.Name = "edtEndDate";
			this.edtEndDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtEndDate.Properties.Buttons"))))});
			this.edtEndDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.edtEndDate.StyleController = this.layoutCtrl;
			resources.ApplyResources(this.tbDescription, "tbDescription");
			this.tbDescription.MenuManager = this.ribbonControl;
			this.tbDescription.Name = "tbDescription";
			this.spellChecker.SetShowSpellCheckMenu(this.tbDescription, true);
			this.spellChecker.SetSpellCheckerOptions(this.tbDescription, optionsSpelling3);
			this.tbDescription.StyleController = this.layoutCtrl;
			this.tbDescription.UseOptimizedRendering = true;
			resources.ApplyResources(this.edtStartTime, "edtStartTime");
			this.edtStartTime.MenuManager = this.ribbonControl;
			this.edtStartTime.Name = "edtStartTime";
			this.edtStartTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.edtStartTime.StyleController = this.layoutCtrl;
			resources.ApplyResources(this.edtEndTime, "edtEndTime");
			this.edtEndTime.MenuManager = this.ribbonControl;
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
			this.layoutDescription,
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
			this.layoutControlGroup.Size = new System.Drawing.Size(609, 392);
			this.layoutControlGroup.TextVisible = false;
			this.layoutLocation.Control = this.tbLocation;
			resources.ApplyResources(this.layoutLocation, "layoutLocation");
			this.layoutLocation.Location = new System.Drawing.Point(0, 81);
			this.layoutLocation.Name = "layoutLocation";
			this.layoutLocation.Size = new System.Drawing.Size(589, 24);
			this.layoutLocation.TextSize = new System.Drawing.Size(51, 13);
			this.layoutSubject.Control = this.tbSubject;
			resources.ApplyResources(this.layoutSubject, "layoutSubject");
			this.layoutSubject.Location = new System.Drawing.Point(0, 57);
			this.layoutSubject.Name = "layoutSubject";
			this.layoutSubject.Size = new System.Drawing.Size(589, 24);
			this.layoutSubject.TextSize = new System.Drawing.Size(51, 13);
			this.layoutDescription.Control = this.tbDescription;
			resources.ApplyResources(this.layoutDescription, "layoutDescription");
			this.layoutDescription.Location = new System.Drawing.Point(0, 235);
			this.layoutDescription.Name = "layoutDescription";
			this.layoutDescription.Size = new System.Drawing.Size(589, 137);
			this.layoutDescription.TextSize = new System.Drawing.Size(0, 0);
			this.layoutDescription.TextToControlDistance = 0;
			this.layoutDescription.TextVisible = false;
			this.layoutAllDay.Control = this.chkAllDay;
			resources.ApplyResources(this.layoutAllDay, "layoutAllDay");
			this.layoutAllDay.Location = new System.Drawing.Point(339, 170);
			this.layoutAllDay.Name = "layoutAllDay";
			this.layoutAllDay.Size = new System.Drawing.Size(102, 24);
			this.layoutAllDay.TextSize = new System.Drawing.Size(0, 0);
			this.layoutAllDay.TextToControlDistance = 0;
			this.layoutAllDay.TextVisible = false;
			this.layoutInfo.Control = this.lblInfo;
			resources.ApplyResources(this.layoutInfo, "layoutInfo");
			this.layoutInfo.Location = new System.Drawing.Point(0, 0);
			this.layoutInfo.Name = "layoutInfo";
			this.layoutInfo.Size = new System.Drawing.Size(589, 57);
			this.layoutInfo.TextLocation = DevExpress.Utils.Locations.Top;
			this.layoutInfo.TextSize = new System.Drawing.Size(0, 0);
			this.layoutInfo.TextToControlDistance = 0;
			this.layoutInfo.TextVisible = false;
			resources.ApplyResources(this.layoutReminderGroup, "layoutReminderGroup");
			this.layoutReminderGroup.GroupBordersVisible = false;
			this.layoutReminderGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutChkReminder,
			this.layoutReminder});
			this.layoutReminderGroup.Location = new System.Drawing.Point(0, 211);
			this.layoutReminderGroup.Name = "layoutReminderGroup";
			this.layoutReminderGroup.Size = new System.Drawing.Size(589, 24);
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
			this.layoutReminder.Size = new System.Drawing.Size(505, 24);
			this.layoutReminder.TextSize = new System.Drawing.Size(0, 0);
			this.layoutReminder.TextToControlDistance = 0;
			this.layoutReminder.TextVisible = false;
			resources.ApplyResources(this.layoutStartGroup, "layoutStartGroup");
			this.layoutStartGroup.GroupBordersVisible = false;
			this.layoutStartGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutStartDate,
			this.layoutStartTime});
			this.layoutStartGroup.Location = new System.Drawing.Point(0, 122);
			this.layoutStartGroup.Name = "layoutStartGroup";
			this.layoutStartGroup.Size = new System.Drawing.Size(339, 48);
			this.layoutStartDate.Control = this.edtStartDate;
			resources.ApplyResources(this.layoutStartDate, "layoutStartDate");
			this.layoutStartDate.Location = new System.Drawing.Point(0, 0);
			this.layoutStartDate.Name = "layoutStartDate";
			this.layoutStartDate.Size = new System.Drawing.Size(230, 48);
			this.layoutStartDate.TextSize = new System.Drawing.Size(51, 13);
			this.layoutStartTime.Control = this.edtStartTime;
			resources.ApplyResources(this.layoutStartTime, "layoutStartTime");
			this.layoutStartTime.Location = new System.Drawing.Point(230, 0);
			this.layoutStartTime.Name = "layoutStartTime";
			this.layoutStartTime.Size = new System.Drawing.Size(109, 48);
			this.layoutStartTime.TextSize = new System.Drawing.Size(0, 0);
			this.layoutStartTime.TextToControlDistance = 0;
			this.layoutStartTime.TextVisible = false;
			resources.ApplyResources(this.layoutEndGroup, "layoutEndGroup");
			this.layoutEndGroup.GroupBordersVisible = false;
			this.layoutEndGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutEndDate,
			this.layoutEndTime});
			this.layoutEndGroup.Location = new System.Drawing.Point(0, 170);
			this.layoutEndGroup.Name = "layoutEndGroup";
			this.layoutEndGroup.Size = new System.Drawing.Size(339, 24);
			this.layoutEndDate.Control = this.edtEndDate;
			resources.ApplyResources(this.layoutEndDate, "layoutEndDate");
			this.layoutEndDate.Location = new System.Drawing.Point(0, 0);
			this.layoutEndDate.Name = "layoutEndDate";
			this.layoutEndDate.Size = new System.Drawing.Size(230, 24);
			this.layoutEndDate.TextSize = new System.Drawing.Size(51, 13);
			this.layoutEndTime.Control = this.edtEndTime;
			resources.ApplyResources(this.layoutEndTime, "layoutEndTime");
			this.layoutEndTime.Location = new System.Drawing.Point(230, 0);
			this.layoutEndTime.Name = "layoutEndTime";
			this.layoutEndTime.Size = new System.Drawing.Size(109, 24);
			this.layoutEndTime.TextSize = new System.Drawing.Size(0, 0);
			this.layoutEndTime.TextToControlDistance = 0;
			this.layoutEndTime.TextVisible = false;
			resources.ApplyResources(this.layoutResourcesGroup, "layoutResourcesGroup");
			this.layoutResourcesGroup.GroupBordersVisible = false;
			this.layoutResourcesGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutResources,
			this.layoutResource});
			this.layoutResourcesGroup.Location = new System.Drawing.Point(339, 122);
			this.layoutResourcesGroup.Name = "layoutResourcesGroup";
			this.layoutResourcesGroup.Size = new System.Drawing.Size(250, 48);
			this.layoutResources.Control = this.edtResources;
			resources.ApplyResources(this.layoutResources, "layoutResources");
			this.layoutResources.Location = new System.Drawing.Point(0, 0);
			this.layoutResources.Name = "layoutResources";
			this.layoutResources.Size = new System.Drawing.Size(250, 24);
			this.layoutResources.TextSize = new System.Drawing.Size(51, 13);
			this.layoutResource.Control = this.edtResource;
			resources.ApplyResources(this.layoutResource, "layoutResource");
			this.layoutResource.Location = new System.Drawing.Point(0, 24);
			this.layoutResource.Name = "layoutResource";
			this.layoutResource.Size = new System.Drawing.Size(250, 24);
			this.layoutResource.TextSize = new System.Drawing.Size(51, 13);
			this.layoutHorzSeparator1.Control = this.lblHorzSeparator1;
			resources.ApplyResources(this.layoutHorzSeparator1, "layoutHorzSeparator1");
			this.layoutHorzSeparator1.Location = new System.Drawing.Point(0, 105);
			this.layoutHorzSeparator1.Name = "layoutHorzSeparator1";
			this.layoutHorzSeparator1.Size = new System.Drawing.Size(589, 17);
			this.layoutHorzSeparator1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutHorzSeparator1.TextToControlDistance = 0;
			this.layoutHorzSeparator1.TextVisible = false;
			this.layoutHorzSeparator2.Control = this.lblHorzSeparator2;
			resources.ApplyResources(this.layoutHorzSeparator2, "layoutHorzSeparator2");
			this.layoutHorzSeparator2.Location = new System.Drawing.Point(0, 194);
			this.layoutHorzSeparator2.Name = "layoutHorzSeparator2";
			this.layoutHorzSeparator2.Size = new System.Drawing.Size(589, 17);
			this.layoutHorzSeparator2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutHorzSeparator2.TextToControlDistance = 0;
			this.layoutHorzSeparator2.TextVisible = false;
			this.emptySpaceItem1.AllowHotTrack = false;
			resources.ApplyResources(this.emptySpaceItem1, "emptySpaceItem1");
			this.emptySpaceItem1.Location = new System.Drawing.Point(441, 170);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Size = new System.Drawing.Size(148, 24);
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.layoutCtrl);
			this.Controls.Add(this.ribbonControl);
			this.Name = "AppointmentFormRibbonStyle";
			this.Ribbon = this.ribbonControl;
			this.ShowInTaskbar = false;
			this.Activated += new System.EventHandler(this.AppointmentFormBase_Activated);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AppointmentFormBase_FormClosing);
			this.Load += new System.EventHandler(this.AppointmentFormBase_Load);
			((System.ComponentModel.ISupportInitialize)(this.repItemAppointmentStatus)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repItemAppointmentLabel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ribbonControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.applicationMenu)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbLocation.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutCtrl)).EndInit();
			this.layoutCtrl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.edtResources.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtResources.ResourcesCheckedListBoxControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbSubject.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtStartDate.Properties.CalendarTimeProperties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtStartDate.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbReminder.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtResource.Properties)).EndInit();
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
			((System.ComponentModel.ISupportInitialize)(this.layoutDescription)).EndInit();
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
			this.ResumeLayout(false);
		}
		#endregion
		protected internal DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl;
		protected internal DevExpress.XtraBars.BarButtonItem btnSave;
		protected internal DevExpress.XtraBars.BarButtonItem btnDelete;
		protected internal DevExpress.XtraBars.BarButtonItem btnRecurrence;
		protected internal DevExpress.XtraBars.BarButtonItem btnSpelling;
		protected internal DevExpress.XtraBars.BarButtonItem btnClose;
		protected internal DevExpress.XtraBars.BarButtonItem btnSaveAndClose;
		protected internal DevExpress.XtraBars.Ribbon.RibbonPage ribPageAppointment;
		protected internal DevExpress.XtraBars.Ribbon.RibbonPageGroup pgrActions;
		protected internal DevExpress.XtraBars.Ribbon.RibbonPageGroup pgrOptions;
		protected internal DevExpress.XtraBars.Ribbon.RibbonPageGroup pgrProofing;
		protected internal DevExpress.XtraSpellChecker.SpellChecker spellChecker;
		protected internal DevExpress.XtraBars.Ribbon.ApplicationMenu applicationMenu;
		protected internal DevExpress.XtraBars.BarAndDockingController barAndDockingController;
		protected DevExpress.XtraEditors.CheckEdit chkReminder;
		protected DevExpress.XtraEditors.CheckEdit chkAllDay;
		protected DevExpress.XtraEditors.DateEdit edtEndDate;
		protected DevExpress.XtraEditors.DateEdit edtStartDate;
		protected DevExpress.XtraScheduler.UI.SchedulerTimeEdit edtStartTime;
		protected DevExpress.XtraScheduler.UI.SchedulerTimeEdit edtEndTime;
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
		protected DevExpress.XtraLayout.LayoutControlItem layoutEndDate;
		protected DevExpress.XtraLayout.LayoutControlItem layoutStartDate;
		protected DevExpress.XtraLayout.LayoutControlItem layoutReminder;
		protected DevExpress.XtraLayout.LayoutControlItem layoutDescription;
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
		protected RepositoryItemAppointmentStatus repItemAppointmentStatus;
		protected RepositoryItemAppointmentLabel repItemAppointmentLabel;
		protected DevExpress.XtraBars.BarEditItem barEditShowTimeAs;
		protected DevExpress.XtraBars.BarEditItem barEditLabelAs;
	}
}
