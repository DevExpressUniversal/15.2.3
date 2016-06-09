#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

namespace DevExpress.DashboardWin.Native {
	partial class NumericFormatForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NumericFormatForm));
			this.panelNumericFormat = new DevExpress.XtraEditors.PanelControl();
			this.currencyChooser = new DevExpress.DashboardWin.Native.CurrencyCultureControl();
			this.cheIncludeGroupSeparator = new DevExpress.XtraEditors.CheckEdit();
			this.sePrecision = new DevExpress.XtraEditors.SpinEdit();
			this.cbeUnit = new DevExpress.XtraEditors.ComboBoxEdit();
			this.cbeFormatType = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblPrecision = new DevExpress.XtraEditors.LabelControl();
			this.lblUnit = new DevExpress.XtraEditors.LabelControl();
			this.lblFormatType = new DevExpress.XtraEditors.LabelControl();
			this.mePreview = new DevExpress.DashboardWin.Native.NumericFormatPreviewControl();
			this.panelOkCancel = new DevExpress.XtraEditors.PanelControl();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.panelNumericFormat)).BeginInit();
			this.panelNumericFormat.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cheIncludeGroupSeparator.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sePrecision.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbeUnit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbeFormatType.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.mePreview.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelOkCancel)).BeginInit();
			this.panelOkCancel.SuspendLayout();
			this.SuspendLayout();
			this.panelNumericFormat.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelNumericFormat.Controls.Add(this.currencyChooser);
			this.panelNumericFormat.Controls.Add(this.cheIncludeGroupSeparator);
			this.panelNumericFormat.Controls.Add(this.sePrecision);
			this.panelNumericFormat.Controls.Add(this.cbeUnit);
			this.panelNumericFormat.Controls.Add(this.cbeFormatType);
			this.panelNumericFormat.Controls.Add(this.lblPrecision);
			this.panelNumericFormat.Controls.Add(this.lblUnit);
			this.panelNumericFormat.Controls.Add(this.lblFormatType);
			this.panelNumericFormat.Controls.Add(this.mePreview);
			resources.ApplyResources(this.panelNumericFormat, "panelNumericFormat");
			this.panelNumericFormat.Name = "panelNumericFormat";
			resources.ApplyResources(this.currencyChooser, "currencyChooser");
			this.currencyChooser.Name = "currencyChooser";
			this.currencyChooser.CurrencyCultureChanged += new System.EventHandler(this.NumericFormatPropertyChanged);
			resources.ApplyResources(this.cheIncludeGroupSeparator, "cheIncludeGroupSeparator");
			this.cheIncludeGroupSeparator.Name = "cheIncludeGroupSeparator";
			this.cheIncludeGroupSeparator.Properties.Caption = resources.GetString("cheIncludeGroupSeparator.Properties.Caption");
			this.cheIncludeGroupSeparator.CheckedChanged += new System.EventHandler(this.NumericFormatPropertyChanged);
			resources.ApplyResources(this.sePrecision, "sePrecision");
			this.sePrecision.Name = "sePrecision";
			this.sePrecision.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.sePrecision.Properties.Mask.EditMask = resources.GetString("sePrecision.Properties.Mask.EditMask");
			this.sePrecision.Properties.MaxValue = new decimal(new int[] {
			15,
			0,
			0,
			0});
			this.sePrecision.EditValueChanged += new System.EventHandler(this.NumericFormatPropertyChanged);
			resources.ApplyResources(this.cbeUnit, "cbeUnit");
			this.cbeUnit.Name = "cbeUnit";
			this.cbeUnit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbeUnit.Properties.Buttons"))))});
			this.cbeUnit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbeUnit.SelectedIndexChanged += new System.EventHandler(this.NumericFormatPropertyChanged);
			resources.ApplyResources(this.cbeFormatType, "cbeFormatType");
			this.cbeFormatType.Name = "cbeFormatType";
			this.cbeFormatType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbeFormatType.Properties.Buttons"))))});
			this.cbeFormatType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbeFormatType.SelectedIndexChanged += new System.EventHandler(this.NumericFormatPropertyChanged);
			resources.ApplyResources(this.lblPrecision, "lblPrecision");
			this.lblPrecision.Name = "lblPrecision";
			resources.ApplyResources(this.lblUnit, "lblUnit");
			this.lblUnit.Name = "lblUnit";
			resources.ApplyResources(this.lblFormatType, "lblFormatType");
			this.lblFormatType.Name = "lblFormatType";
			resources.ApplyResources(this.mePreview, "mePreview");
			this.mePreview.Name = "mePreview";
			this.mePreview.Properties.AllowFocused = false;
			this.mePreview.Properties.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("mePreview.Properties.Appearance.Font")));
			this.mePreview.Properties.Appearance.Options.UseFont = true;
			this.mePreview.Properties.Appearance.Options.UseTextOptions = true;
			this.mePreview.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.mePreview.Properties.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
			this.mePreview.Properties.ReadOnly = true;
			this.mePreview.Properties.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.mePreview.TabStop = false;
			resources.ApplyResources(this.panelOkCancel, "panelOkCancel");
			this.panelOkCancel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelOkCancel.Controls.Add(this.btnCancel);
			this.panelOkCancel.Controls.Add(this.btnOk);
			this.panelOkCancel.Name = "panelOkCancel";
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.ButtonOKClick);
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.panelOkCancel);
			this.Controls.Add(this.panelNumericFormat);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.ShowIcon = false;
			this.Name = "NumericFormatForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			((System.ComponentModel.ISupportInitialize)(this.panelNumericFormat)).EndInit();
			this.panelNumericFormat.ResumeLayout(false);
			this.panelNumericFormat.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.cheIncludeGroupSeparator.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sePrecision.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbeUnit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbeFormatType.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.mePreview.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelOkCancel)).EndInit();
			this.panelOkCancel.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraEditors.PanelControl panelNumericFormat;
		private DevExpress.XtraEditors.PanelControl panelOkCancel;
		private DevExpress.XtraEditors.SimpleButton btnCancel;
		private DevExpress.XtraEditors.SimpleButton btnOk;
		private DevExpress.XtraEditors.LabelControl lblPrecision;
		private DevExpress.XtraEditors.LabelControl lblUnit;
		private DevExpress.XtraEditors.LabelControl lblFormatType;
		private DevExpress.XtraEditors.CheckEdit cheIncludeGroupSeparator;
		private DevExpress.XtraEditors.SpinEdit sePrecision;
		private DevExpress.XtraEditors.ComboBoxEdit cbeUnit;
		private DevExpress.XtraEditors.ComboBoxEdit cbeFormatType;
		private NumericFormatPreviewControl mePreview;
		private CurrencyCultureControl currencyChooser;
	}
}
