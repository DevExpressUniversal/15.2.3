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
using System.Drawing.Printing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Printing;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Printing.Native;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraEditors.Native;
using DevExpress.XtraScheduler.Internal.Diagnostics;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.PageSetupForm.tabFormatControl")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.PageSetupForm.btnOk")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.PageSetupForm.btnCancel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.PageSetupForm.btnPrintPreview")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.PageSetupForm.xtraTabControl")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.PageSetupForm.paper")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.PageSetupForm.panelControl1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.PageSetupForm.format")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.PageSetupForm.btnPrint")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.PageSetupForm.tabPaperControl")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.PageSetupForm.tabPageResources")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.PageSetupForm.tabResources")]
#endregion
namespace DevExpress.XtraScheduler.Forms {
	[System.Runtime.InteropServices.ComVisible(false)]
	public partial class PageSetupForm : DevExpress.XtraEditors.XtraForm {
		#region Fields
		SchedulerPrintStyleCollection editedPrintStyles;
		SchedulerPrinter currentPrinter;
		SchedulerPrintStyle currentEditedPrintStyle;
		SchedulerControl control;
		bool designTimeMode = false;
		Form form;
		#endregion
		PageSetupForm() {
			InitializeComponent();
		}
		public PageSetupForm(SchedulerControl control) {
			Guard.ArgumentNotNull(control, "control");
			Guard.ArgumentNotNull(control.Storage, "control.Storage");
			InitializeComponent();
			this.control = control;
			CheckPrintingSystem();
			CreateEditedPrintStyles();
			SchedulerPrintStyleKind printStyleKind = GetEditedPrintStyleKind();
			tabFormatControl.SetEditedPrintStyles(editedPrintStyles, printStyleKind);
			currentEditedPrintStyle = tabFormatControl.CurrentPrintStyle;
			tabResources.Storage = control.Storage;
			SetResourceOptionsToResourcesTab();
			tabPaperControl.PageSettings = currentEditedPrintStyle.PageSettings;
			SubscribeEvents();
		}
		#region Properties
		internal SchedulerControl Control { get { return control; } }
		internal SchedulerPrintStyleCollection EditedPrintStyles { get { return editedPrintStyles; } set { editedPrintStyles = value; } }
		protected SchedulerPrinter CurrentPrinter { 
			get {
				if (currentPrinter == null)
					UpdateCurrentPrinter();
				return currentPrinter; } 
		}
		#endregion
		protected override void Dispose(bool disposing) {
			if (disposing) {
				UnsubscribeEvents();
				DisposeCurrentPrinter();
				if (components != null) {
					components.Dispose();
					components = null;
				}
			}
			base.Dispose(disposing);
		}
		public virtual void SetMenuManager(Utils.Menu.IDXMenuManager menuManager) {
			MenuManagerUtils.SetMenuManager(Controls, menuManager);
			if (this.tabFormatControl != null)
				this.tabFormatControl.SetMenuManager(menuManager);
		}
		protected internal virtual void CheckPrintingSystem() {
			if (ComponentPrinterBase.IsPrintingAvailable(false))
				return;
			btnPrint.Enabled = false;
			btnPrintPreview.Enabled = false;
		}
		protected internal virtual void SubscribeEvents() {
			tabFormatControl.CurrentPrintStyleChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.OnCurrentPrintStyleChanging);
			tabFormatControl.CurrentPrintStyleChanged += new System.EventHandler(this.OnCurrentPrintStyleChanged);
			tabFormatControl.SelectionPrintStyleChanged += new EventHandler(OnSelectionPrintStyleChanged);
			btnOk.Click += new System.EventHandler(this.OnButtonOkClick);
			btnCancel.Click += new System.EventHandler(this.OnCancel);
			btnPrintPreview.Click += new System.EventHandler(this.OnPrintPreview);
			btnPrint.Click += new System.EventHandler(this.OnPrint);
		}
		protected internal virtual void UnsubscribeEvents() {
			tabFormatControl.CurrentPrintStyleChanging -= new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.OnCurrentPrintStyleChanging);
			tabFormatControl.CurrentPrintStyleChanged -= new System.EventHandler(this.OnCurrentPrintStyleChanged);
			tabFormatControl.SelectionPrintStyleChanged -= new EventHandler(OnSelectionPrintStyleChanged);
			btnOk.Click -= new System.EventHandler(this.OnButtonOkClick);
			btnCancel.Click -= new System.EventHandler(this.OnCancel);
			btnPrintPreview.Click -= new System.EventHandler(this.OnPrintPreview);
			btnPrint.Click -= new System.EventHandler(this.OnPrint);
		}
		protected internal virtual void SubscribePrintingSystemChanged(IPrintingSystem ps) {
			if (ps == null)
				return;
			ps.AfterChange += new DevExpress.XtraPrinting.ChangeEventHandler(OnPrintingSystemChanged);
		}
		[Obsolete("You should use the 'SubscribePrintingSystemChanged(IPrintingSystem)' instead.", false)]
		protected internal virtual void SubscribePrintingSystemChanged() {
		}
		protected internal virtual void UnsubscribePrintingSystemChanged(IPrintingSystem ps) {
			if (ps == null)
				return;
			ps.AfterChange -= new DevExpress.XtraPrinting.ChangeEventHandler(OnPrintingSystemChanged);
		}
		[Obsolete("You should use the 'UnsubscribePrintingSystemChanged(IPrintingSystem)' instead.", false)]
		protected internal virtual void UnsubscribePrintingSystemChanged() {
		}
		protected internal virtual void OnPrintingSystemChanged(object sender, DevExpress.XtraPrinting.ChangeEventArgs e) {
			if(((PrintingSystemBase)sender).PageCount == 0)
				return;
			switch (e.EventName) {
				case DevExpress.XtraPrinting.SR.PageSettingsChanged:
					currentEditedPrintStyle.PageSettings = CurrentPrinter.PageSettings;
					tabPaperControl.PageSettings = currentEditedPrintStyle.PageSettings;
					break;
				case DevExpress.XtraPrinting.SR.AfterMarginsChange:
					currentEditedPrintStyle.PageSettings.Margins = CurrentPrinter.PrintingSystemBase.PageMargins;
					currentEditedPrintStyle.PageSettings = currentEditedPrintStyle.PageSettings; 
					tabPaperControl.PageSettings = currentEditedPrintStyle.PageSettings;
					break;
			}
		}
		public void SwitchToDesignTimeMode() {
			btnPrintPreview.Visible = false;
			btnPrint.Visible = false;
			tabResources.SwitchToDesignTimeMode();
			designTimeMode = true;
		}
		protected internal virtual void CreateEditedPrintStyles() {
			SchedulerPrintStyleCollection sourcePrintStyles = control.PrintStyles;
			editedPrintStyles = new SchedulerPrintStyleCollection(false);
			tabFormatControl.BeginUpdate();
			sourcePrintStyles.ForEach(AddEditedPrintStyle);
			tabFormatControl.EndUpdate();
		}
		protected internal virtual void AddEditedPrintStyle(SchedulerPrintStyle sourcePrintStyle) {
			SchedulerPrintStyle printStyle = (SchedulerPrintStyle)sourcePrintStyle.Clone();
			SetDefaultRangeIfNecessary(printStyle);
			editedPrintStyles.Add(printStyle);
		}
		protected internal virtual void SetDefaultRangeIfNecessary(SchedulerPrintStyle printStyle) {
			PrintStyleWithDateRange printStyleWithDateRange = printStyle as PrintStyleWithDateRange;
			if (printStyleWithDateRange != null) {
				TimeIntervalCollection visibleIntervals = control.ActiveView.GetVisibleIntervals();
				printStyleWithDateRange.SetDefaultRange(visibleIntervals[0].Start.Date,
					visibleIntervals[visibleIntervals.Count - 1].Start.Date);
			}
		}
		protected internal virtual SchedulerPrintStyleKind GetEditedPrintStyleKind() {
			if (control.ActiveView is MonthView)
				return SchedulerPrintStyleKind.Monthly;
			if (control.ActiveView is WeekView)
				return SchedulerPrintStyleKind.Weekly;
			return SchedulerPrintStyleKind.Daily;
		}
		protected internal virtual void OnButtonOkClick(object sender, System.EventArgs e) {
			if (OnApplyAllPrintStyle()) {
				ApplyEditedPrintStyles();
				DialogResult = DialogResult.OK;
				this.Close();
			}
		}
		protected internal virtual void ApplyEditedPrintStyles() {
			SchedulerPrintStyleCollection sourcePrintStyles = control.PrintStyles;
			sourcePrintStyles.DisposeCollectionElements();
			int count = editedPrintStyles.Count;
			for (int i = 0; i < count; i++) {
				SchedulerPrintStyle editedStyle = editedPrintStyles[i];
				SchedulerPrintStyle cloneStyle = (SchedulerPrintStyle)editedStyle.Clone();
				if (designTimeMode) {
					PrintStyleWithDateRange style = cloneStyle as PrintStyleWithDateRange;
					if (style != null && style.UseDefaultRange) {
						style.StartRangeDate = new DateTime((long)0);
						style.EndRangeDate = new DateTime((long)0);
						style.UseDefaultRange = true;
					}
				}
				sourcePrintStyles.Add(cloneStyle);
			}
		}
		protected internal virtual void OnCurrentPrintStyleChanged(object sender, System.EventArgs e) {
		}
		protected internal virtual void OnSelectionPrintStyleChanged(object sender, EventArgs e) {
			currentEditedPrintStyle = tabFormatControl.CurrentPrintStyle;
			ChangeFormTitle();
			SetResourceOptionsToResourcesTab();
			SetPageSettingsToPaperTab();
		}
		protected virtual void UpdateCurrentPrinter() {
			DisposeCurrentPrinter();
			this.currentPrinter = control.CreateSchedulerPrinter(currentEditedPrintStyle);
			SubscribePrintingSystemChanged(currentPrinter.PrintingSystemBase);
		}
		private void DisposeCurrentPrinter() {
			if (currentPrinter != null) {
				UnsubscribePrintingSystemChanged(currentPrinter.PrintingSystemBase);
				currentPrinter.Dispose();
				currentPrinter = null;
			}
			if(form != null && !form.IsDisposed) {
				form.Dispose();
				form = null;
			}
		}
		protected void SetPageSettingsToPaperTab() {
			if (currentEditedPrintStyle.PageSettings == null) {
				tabPaperControl.Enabled = false;
				return;
			}
			tabPaperControl.Enabled = true;
			tabPaperControl.PageSettings = currentEditedPrintStyle.PageSettings;
		}
		protected internal virtual void ChangeFormTitle() {
			if (currentEditedPrintStyle != null)
				this.Text = String.Format(SchedulerLocalizer.GetString(SchedulerStringId.PrintPageSetupFormCaption), currentEditedPrintStyle.DisplayName);
		}
		protected internal virtual void SetResourceOptionsToResourcesTab() {
			PrintStyleWithResourceOptions printStyle = currentEditedPrintStyle as PrintStyleWithResourceOptions;
			if (printStyle != null) {
				tabPageResources.PageEnabled = true;
				tabResources.ResourceOptions = printStyle.ResourceOptions;
			}
			else {
				tabPageResources.PageEnabled = false;
				tabResources.ResourceOptions = null;
			}
		}
		protected internal virtual void OnPrintPreview(object sender, System.EventArgs e) {
			if(currentEditedPrintStyle != CurrentPrinter.PrintStyle)
				UpdateCurrentPrinter();
			if(form == null || form.IsDisposed)
				form = Control.ShowPrintPreviewCore(CurrentPrinter);
			else {
				Control.CreateDocument(CurrentPrinter);
				ActivatePreviewForm();
			}
		}
		protected void ActivatePreviewForm() {
			if (!form.Visible)
				form.Visible = true;
			if (form.WindowState == FormWindowState.Minimized)
				form.WindowState = FormWindowState.Normal;
			form.Activate();
		}
		protected internal virtual void OnPrint(object sender, System.EventArgs e) {
			if (PrinterSettings.InstalledPrinters.Count > 0) {
				control.Print(currentEditedPrintStyle);
			}
			else
				XtraEditors.XtraMessageBox.Show(SchedulerLocalizer.GetString(SchedulerStringId.PrintNoPrintersInstalled), "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
		protected internal virtual void OnCancel(object sender, EventArgs e) {
			DialogResult = DialogResult.Cancel;
			this.Close();
		}
		protected internal virtual void OnCurrentPrintStyleChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e) {
			e.Cancel = !OnApplyAllPrintStyle();
		}
		protected internal virtual bool OnApplyAllPrintStyle() {
			if (!tabPaperControl.ApplySettingsToAllStyles)
				return true;
			DialogResult result = ShowApplySettingsMessageBox();
			switch (result) {
				case DialogResult.Cancel:
					return false;
				case DialogResult.No:
					tabPaperControl.ApplySettingsToAllStyles = false;
					return true;
				case DialogResult.Yes:
					tabPaperControl.ApplySettingsToAllStyles = false;
					ApplyPageSettingsToAllStyles();
					return true;
			}
			return true;
		}
		protected internal virtual DialogResult ShowApplySettingsMessageBox() {
			DialogResult result = XtraMessageBox.Show(SchedulerLocalizer.GetString(SchedulerStringId.Msg_ApplyToAllStyles),
												  Application.ProductName,
												  MessageBoxButtons.YesNoCancel,
												  MessageBoxIcon.Question);
			return result;
		}
		protected internal virtual void ApplyPageSettingsToAllStyles() {
			XtraSchedulerDebug.Assert(editedPrintStyles != null);
			XtraSchedulerDebug.Assert(currentEditedPrintStyle != null);
			int count = editedPrintStyles.Count;
			for (int i = 0; i < count; i++) {
				editedPrintStyles[i].PageSettings = (PageSettings)currentEditedPrintStyle.PageSettings.Clone();
			}
		}
	}
}
