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

namespace DevExpress.XtraSpreadsheet.Forms {
	partial class DataValidationForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataValidationForm));
			this.xtraTabControl = new DevExpress.XtraTab.XtraTabControl();
			this.settingsPage = new DevExpress.XtraTab.XtraTabPage();
			this.edtFormula2 = new DevExpress.XtraSpreadsheet.ReferenceEditControl();
			this.edtFormula1 = new DevExpress.XtraSpreadsheet.ReferenceEditControl();
			this.chkDropdown = new DevExpress.XtraEditors.CheckEdit();
			this.chkApplyToAllCells = new DevExpress.XtraEditors.CheckEdit();
			this.lblFormula2 = new DevExpress.XtraEditors.LabelControl();
			this.lblFormula1 = new DevExpress.XtraEditors.LabelControl();
			this.chkIgnoreBlank = new DevExpress.XtraEditors.CheckEdit();
			this.edtData = new DevExpress.XtraEditors.LookUpEdit();
			this.lblData = new DevExpress.XtraEditors.LabelControl();
			this.edtType = new DevExpress.XtraEditors.LookUpEdit();
			this.lblType = new DevExpress.XtraEditors.LabelControl();
			this.lblValidationCriteria = new DevExpress.XtraEditors.LabelControl();
			this.inputMessagePage = new DevExpress.XtraTab.XtraTabPage();
			this.edtMessage = new DevExpress.XtraEditors.MemoEdit();
			this.lblMessage = new DevExpress.XtraEditors.LabelControl();
			this.edtMessageTitle = new DevExpress.XtraEditors.TextEdit();
			this.lblMessageTitle = new DevExpress.XtraEditors.LabelControl();
			this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
			this.chkShowMessage = new DevExpress.XtraEditors.CheckEdit();
			this.errorAlertPage = new DevExpress.XtraTab.XtraTabPage();
			this.iconInfo = new DevExpress.XtraEditors.PictureEdit();
			this.iconWarning = new DevExpress.XtraEditors.PictureEdit();
			this.iconStop = new DevExpress.XtraEditors.PictureEdit();
			this.edtErrorMessage = new DevExpress.XtraEditors.MemoEdit();
			this.lblErrorMessage = new DevExpress.XtraEditors.LabelControl();
			this.edtErrorTitle = new DevExpress.XtraEditors.TextEdit();
			this.lblErrorTitle = new DevExpress.XtraEditors.LabelControl();
			this.edtErrorStyle = new DevExpress.XtraEditors.LookUpEdit();
			this.lblErrorStyle = new DevExpress.XtraEditors.LabelControl();
			this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
			this.chkShowError = new DevExpress.XtraEditors.CheckEdit();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnClearAll = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl)).BeginInit();
			this.xtraTabControl.SuspendLayout();
			this.settingsPage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.edtFormula2.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtFormula1.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkDropdown.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkApplyToAllCells.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkIgnoreBlank.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtData.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtType.Properties)).BeginInit();
			this.inputMessagePage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.edtMessage.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtMessageTitle.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkShowMessage.Properties)).BeginInit();
			this.errorAlertPage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.iconInfo.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.iconWarning.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.iconStop.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtErrorMessage.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtErrorTitle.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtErrorStyle.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkShowError.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.xtraTabControl, "xtraTabControl");
			this.xtraTabControl.Name = "xtraTabControl";
			this.xtraTabControl.SelectedTabPage = this.settingsPage;
			this.xtraTabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.settingsPage,
			this.inputMessagePage,
			this.errorAlertPage});
			this.xtraTabControl.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.xtraTabControl_SelectedPageChanged);
			this.xtraTabControl.SelectedPageChanging += new DevExpress.XtraTab.TabPageChangingEventHandler(this.xtraTabControl_SelectedPageChanging);
			this.settingsPage.Controls.Add(this.edtFormula2);
			this.settingsPage.Controls.Add(this.edtFormula1);
			this.settingsPage.Controls.Add(this.chkDropdown);
			this.settingsPage.Controls.Add(this.chkApplyToAllCells);
			this.settingsPage.Controls.Add(this.lblFormula2);
			this.settingsPage.Controls.Add(this.lblFormula1);
			this.settingsPage.Controls.Add(this.chkIgnoreBlank);
			this.settingsPage.Controls.Add(this.edtData);
			this.settingsPage.Controls.Add(this.lblData);
			this.settingsPage.Controls.Add(this.edtType);
			this.settingsPage.Controls.Add(this.lblType);
			this.settingsPage.Controls.Add(this.lblValidationCriteria);
			this.settingsPage.Name = "settingsPage";
			resources.ApplyResources(this.settingsPage, "settingsPage");
			this.edtFormula2.Activated = false;
			this.edtFormula2.EditValuePrefix = null;
			this.edtFormula2.IncludeSheetName = false;
			resources.ApplyResources(this.edtFormula2, "edtFormula2");
			this.edtFormula2.Name = "edtFormula2";
			this.edtFormula2.PositionType = DevExpress.XtraSpreadsheet.Model.PositionType.Relative;
			this.edtFormula2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.edtFormula2.Properties.MaxLength = 255;
			this.edtFormula2.SpreadsheetControl = null;
			this.edtFormula1.Activated = false;
			this.edtFormula1.EditValuePrefix = null;
			this.edtFormula1.IncludeSheetName = false;
			resources.ApplyResources(this.edtFormula1, "edtFormula1");
			this.edtFormula1.Name = "edtFormula1";
			this.edtFormula1.PositionType = DevExpress.XtraSpreadsheet.Model.PositionType.Relative;
			this.edtFormula1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.edtFormula1.Properties.MaxLength = 255;
			this.edtFormula1.SpreadsheetControl = null;
			resources.ApplyResources(this.chkDropdown, "chkDropdown");
			this.chkDropdown.Name = "chkDropdown";
			this.chkDropdown.Properties.Caption = resources.GetString("chkDropdown.Properties.Caption");
			resources.ApplyResources(this.chkApplyToAllCells, "chkApplyToAllCells");
			this.chkApplyToAllCells.Name = "chkApplyToAllCells";
			this.chkApplyToAllCells.Properties.Caption = resources.GetString("chkApplyToAllCells.Properties.Caption");
			this.chkApplyToAllCells.CheckedChanged += new System.EventHandler(this.chkApplyToAllCells_CheckedChanged);
			resources.ApplyResources(this.lblFormula2, "lblFormula2");
			this.lblFormula2.Name = "lblFormula2";
			resources.ApplyResources(this.lblFormula1, "lblFormula1");
			this.lblFormula1.Name = "lblFormula1";
			resources.ApplyResources(this.chkIgnoreBlank, "chkIgnoreBlank");
			this.chkIgnoreBlank.Name = "chkIgnoreBlank";
			this.chkIgnoreBlank.Properties.Caption = resources.GetString("chkIgnoreBlank.Properties.Caption");
			resources.ApplyResources(this.edtData, "edtData");
			this.edtData.Name = "edtData";
			this.edtData.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtData.Properties.Buttons"))))});
			this.edtData.Properties.DropDownRows = 8;
			this.edtData.Properties.NullText = resources.GetString("edtData.Properties.NullText");
			this.edtData.Properties.ShowFooter = false;
			this.edtData.Properties.ShowHeader = false;
			resources.ApplyResources(this.lblData, "lblData");
			this.lblData.Name = "lblData";
			resources.ApplyResources(this.edtType, "edtType");
			this.edtType.Name = "edtType";
			this.edtType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtType.Properties.Buttons"))))});
			this.edtType.Properties.DropDownRows = 8;
			this.edtType.Properties.NullText = resources.GetString("edtType.Properties.NullText");
			this.edtType.Properties.ShowFooter = false;
			this.edtType.Properties.ShowHeader = false;
			resources.ApplyResources(this.lblType, "lblType");
			this.lblType.Name = "lblType";
			resources.ApplyResources(this.lblValidationCriteria, "lblValidationCriteria");
			this.lblValidationCriteria.LineLocation = DevExpress.XtraEditors.LineLocation.Center;
			this.lblValidationCriteria.LineVisible = true;
			this.lblValidationCriteria.Name = "lblValidationCriteria";
			this.inputMessagePage.Controls.Add(this.edtMessage);
			this.inputMessagePage.Controls.Add(this.lblMessage);
			this.inputMessagePage.Controls.Add(this.edtMessageTitle);
			this.inputMessagePage.Controls.Add(this.lblMessageTitle);
			this.inputMessagePage.Controls.Add(this.labelControl4);
			this.inputMessagePage.Controls.Add(this.chkShowMessage);
			this.inputMessagePage.Name = "inputMessagePage";
			resources.ApplyResources(this.inputMessagePage, "inputMessagePage");
			resources.ApplyResources(this.edtMessage, "edtMessage");
			this.edtMessage.Name = "edtMessage";
			this.edtMessage.Properties.MaxLength = 255;
			resources.ApplyResources(this.lblMessage, "lblMessage");
			this.lblMessage.Name = "lblMessage";
			resources.ApplyResources(this.edtMessageTitle, "edtMessageTitle");
			this.edtMessageTitle.Name = "edtMessageTitle";
			this.edtMessageTitle.Properties.MaxLength = 32;
			resources.ApplyResources(this.lblMessageTitle, "lblMessageTitle");
			this.lblMessageTitle.Name = "lblMessageTitle";
			resources.ApplyResources(this.labelControl4, "labelControl4");
			this.labelControl4.LineLocation = DevExpress.XtraEditors.LineLocation.Center;
			this.labelControl4.LineVisible = true;
			this.labelControl4.Name = "labelControl4";
			resources.ApplyResources(this.chkShowMessage, "chkShowMessage");
			this.chkShowMessage.Name = "chkShowMessage";
			this.chkShowMessage.Properties.Caption = resources.GetString("chkShowMessage.Properties.Caption");
			this.errorAlertPage.Controls.Add(this.iconInfo);
			this.errorAlertPage.Controls.Add(this.iconWarning);
			this.errorAlertPage.Controls.Add(this.iconStop);
			this.errorAlertPage.Controls.Add(this.edtErrorMessage);
			this.errorAlertPage.Controls.Add(this.lblErrorMessage);
			this.errorAlertPage.Controls.Add(this.edtErrorTitle);
			this.errorAlertPage.Controls.Add(this.lblErrorTitle);
			this.errorAlertPage.Controls.Add(this.edtErrorStyle);
			this.errorAlertPage.Controls.Add(this.lblErrorStyle);
			this.errorAlertPage.Controls.Add(this.labelControl7);
			this.errorAlertPage.Controls.Add(this.chkShowError);
			this.errorAlertPage.Name = "errorAlertPage";
			resources.ApplyResources(this.errorAlertPage, "errorAlertPage");
			resources.ApplyResources(this.iconInfo, "iconInfo");
			this.iconInfo.Name = "iconInfo";
			this.iconInfo.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.iconInfo.Properties.ReadOnly = true;
			this.iconInfo.Properties.ShowMenu = false;
			this.iconInfo.ShowToolTips = false;
			resources.ApplyResources(this.iconWarning, "iconWarning");
			this.iconWarning.Name = "iconWarning";
			this.iconWarning.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
			this.iconWarning.Properties.AllowFocused = false;
			this.iconWarning.Properties.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
			this.iconWarning.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.iconWarning.Properties.ReadOnly = true;
			this.iconWarning.Properties.ShowMenu = false;
			this.iconWarning.ShowToolTips = false;
			resources.ApplyResources(this.iconStop, "iconStop");
			this.iconStop.Name = "iconStop";
			this.iconStop.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
			this.iconStop.Properties.AllowFocused = false;
			this.iconStop.Properties.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
			this.iconStop.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.iconStop.Properties.ReadOnly = true;
			this.iconStop.Properties.ShowMenu = false;
			this.iconStop.ShowToolTips = false;
			resources.ApplyResources(this.edtErrorMessage, "edtErrorMessage");
			this.edtErrorMessage.Name = "edtErrorMessage";
			this.edtErrorMessage.Properties.MaxLength = 225;
			resources.ApplyResources(this.lblErrorMessage, "lblErrorMessage");
			this.lblErrorMessage.Name = "lblErrorMessage";
			resources.ApplyResources(this.edtErrorTitle, "edtErrorTitle");
			this.edtErrorTitle.Name = "edtErrorTitle";
			this.edtErrorTitle.Properties.MaxLength = 32;
			resources.ApplyResources(this.lblErrorTitle, "lblErrorTitle");
			this.lblErrorTitle.Name = "lblErrorTitle";
			resources.ApplyResources(this.edtErrorStyle, "edtErrorStyle");
			this.edtErrorStyle.Name = "edtErrorStyle";
			this.edtErrorStyle.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtErrorStyle.Properties.Buttons"))))});
			this.edtErrorStyle.Properties.ShowFooter = false;
			this.edtErrorStyle.Properties.ShowHeader = false;
			resources.ApplyResources(this.lblErrorStyle, "lblErrorStyle");
			this.lblErrorStyle.Name = "lblErrorStyle";
			resources.ApplyResources(this.labelControl7, "labelControl7");
			this.labelControl7.LineLocation = DevExpress.XtraEditors.LineLocation.Center;
			this.labelControl7.LineVisible = true;
			this.labelControl7.Name = "labelControl7";
			resources.ApplyResources(this.chkShowError, "chkShowError");
			this.chkShowError.Name = "chkShowError";
			this.chkShowError.Properties.Caption = resources.GetString("chkShowError.Properties.Caption");
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			resources.ApplyResources(this.btnClearAll, "btnClearAll");
			this.btnClearAll.Name = "btnClearAll";
			this.btnClearAll.Click += new System.EventHandler(this.btnClearAll_Click);
			this.AcceptButton = this.btnOk;
			this.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.btnClearAll);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.xtraTabControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "DataValidationForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl)).EndInit();
			this.xtraTabControl.ResumeLayout(false);
			this.settingsPage.ResumeLayout(false);
			this.settingsPage.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.edtFormula2.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtFormula1.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkDropdown.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkApplyToAllCells.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkIgnoreBlank.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtData.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtType.Properties)).EndInit();
			this.inputMessagePage.ResumeLayout(false);
			this.inputMessagePage.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.edtMessage.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtMessageTitle.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkShowMessage.Properties)).EndInit();
			this.errorAlertPage.ResumeLayout(false);
			this.errorAlertPage.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.iconInfo.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.iconWarning.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.iconStop.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtErrorMessage.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtErrorTitle.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtErrorStyle.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkShowError.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private XtraTab.XtraTabControl xtraTabControl;
		private XtraTab.XtraTabPage settingsPage;
		private XtraTab.XtraTabPage inputMessagePage;
		private XtraTab.XtraTabPage errorAlertPage;
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.SimpleButton btnOk;
		private XtraEditors.SimpleButton btnClearAll;
		private XtraEditors.LabelControl lblValidationCriteria;
		private XtraEditors.LabelControl lblType;
		private XtraEditors.LookUpEdit edtType;
		private XtraEditors.LookUpEdit edtData;
		private XtraEditors.LabelControl lblData;
		private XtraEditors.CheckEdit chkIgnoreBlank;
		private XtraEditors.LabelControl lblFormula1;
		private XtraEditors.LabelControl lblFormula2;
		private XtraEditors.CheckEdit chkApplyToAllCells;
		private ReferenceEditControl edtFormula2;
		private ReferenceEditControl edtFormula1;
		private XtraEditors.LabelControl labelControl4;
		private XtraEditors.CheckEdit chkShowMessage;
		private XtraEditors.LabelControl lblMessageTitle;
		private XtraEditors.LabelControl lblMessage;
		private XtraEditors.TextEdit edtMessageTitle;
		private XtraEditors.MemoEdit edtMessage;
		private XtraEditors.MemoEdit edtErrorMessage;
		private XtraEditors.LabelControl lblErrorMessage;
		private XtraEditors.TextEdit edtErrorTitle;
		private XtraEditors.LabelControl lblErrorTitle;
		private XtraEditors.LookUpEdit edtErrorStyle;
		private XtraEditors.LabelControl lblErrorStyle;
		private XtraEditors.LabelControl labelControl7;
		private XtraEditors.CheckEdit chkShowError;
		private XtraEditors.CheckEdit chkDropdown;
		private XtraEditors.PictureEdit iconStop;
		private XtraEditors.PictureEdit iconWarning;
		private XtraEditors.PictureEdit iconInfo;
	}
}
