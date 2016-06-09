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
	partial class MovePivotTableForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MovePivotTableForm));
			this.lblLocation = new DevExpress.XtraEditors.LabelControl();
			this.editLocation = new DevExpress.XtraSpreadsheet.ReferenceEditControl();
			this.rgrpWorksheets = new DevExpress.XtraEditors.RadioGroup();
			this.lblPivotTablePlace = new DevExpress.XtraEditors.LabelControl();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.editLocation.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rgrpWorksheets.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.lblLocation, "lblLocation");
			this.lblLocation.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblLocation.Name = "lblLocation";
			this.editLocation.Activated = false;
			this.editLocation.EditValuePrefix = null;
			this.editLocation.IncludeSheetName = false;
			resources.ApplyResources(this.editLocation, "editLocation");
			this.editLocation.Name = "editLocation";
			this.editLocation.PositionType = DevExpress.XtraSpreadsheet.Model.PositionType.Relative;
			this.editLocation.Properties.AccessibleName = resources.GetString("editLocation.Properties.AccessibleName");
			this.editLocation.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.editLocation.Properties.MaxLength = 255;
			this.editLocation.SpreadsheetControl = null;
			this.editLocation.SuppressActiveSheetChanging = false;
			resources.ApplyResources(this.rgrpWorksheets, "rgrpWorksheets");
			this.rgrpWorksheets.Name = "rgrpWorksheets";
			this.rgrpWorksheets.Properties.AccessibleName = resources.GetString("rgrpWorksheets.Properties.AccessibleName");
			this.rgrpWorksheets.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.RadioButton;
			this.rgrpWorksheets.Properties.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("rgrpWorksheets.Properties.Appearance.BackColor")));
			this.rgrpWorksheets.Properties.Appearance.Options.UseBackColor = true;
			this.rgrpWorksheets.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.rgrpWorksheets.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgrpWorksheets.Properties.Items"))), resources.GetString("rgrpWorksheets.Properties.Items1")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgrpWorksheets.Properties.Items2"))), resources.GetString("rgrpWorksheets.Properties.Items3"))});
			resources.ApplyResources(this.lblPivotTablePlace, "lblPivotTablePlace");
			this.lblPivotTablePlace.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblPivotTablePlace.LineVisible = true;
			this.lblPivotTablePlace.Name = "lblPivotTablePlace";
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.lblLocation);
			this.Controls.Add(this.editLocation);
			this.Controls.Add(this.rgrpWorksheets);
			this.Controls.Add(this.lblPivotTablePlace);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.btnCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "MovePivotTableForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			((System.ComponentModel.ISupportInitialize)(this.editLocation.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rgrpWorksheets.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.LabelControl lblLocation;
		private ReferenceEditControl editLocation;
		private XtraEditors.RadioGroup rgrpWorksheets;
		private XtraEditors.LabelControl lblPivotTablePlace;
		private XtraEditors.SimpleButton btnOk;
		private XtraEditors.SimpleButton btnCancel;
	}
}
