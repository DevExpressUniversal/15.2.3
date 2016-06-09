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

namespace DevExpress.XtraSpreadsheet.Forms.Design {
	partial class FormatNumberDecimalControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormatNumberDecimalControl));
			this.lbDecimalPlaces = new DevExpress.XtraEditors.LabelControl();
			this.edtDecimalPlaces = new DevExpress.XtraEditors.SpinEdit();
			this.chkUseThousandSeparator = new DevExpress.XtraEditors.CheckEdit();
			this.lblNegativeNumbers = new DevExpress.XtraEditors.LabelControl();
			this.lblSymbol = new DevExpress.XtraEditors.LabelControl();
			this.edtSymbol = new DevExpress.XtraEditors.LookUpEdit();
			this.lBoxNegativeNumbers = new DevExpress.XtraEditors.ListBoxControl();
			((System.ComponentModel.ISupportInitialize)(this.edtDecimalPlaces.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkUseThousandSeparator.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtSymbol.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lBoxNegativeNumbers)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.lbDecimalPlaces, "lbDecimalPlaces");
			this.lbDecimalPlaces.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lbDecimalPlaces.Name = "lbDecimalPlaces";
			resources.ApplyResources(this.edtDecimalPlaces, "edtDecimalPlaces");
			this.edtDecimalPlaces.Name = "edtDecimalPlaces";
			this.edtDecimalPlaces.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.edtDecimalPlaces.Properties.IsFloatValue = false;
			this.edtDecimalPlaces.Properties.Mask.EditMask = resources.GetString("edtDecimalPlaces.Properties.Mask.EditMask");
			this.edtDecimalPlaces.Properties.Mask.MaskType = ((DevExpress.XtraEditors.Mask.MaskType)(resources.GetObject("edtDecimalPlaces.Properties.Mask.MaskType")));
			this.edtDecimalPlaces.Properties.MaxLength = 2;
			this.edtDecimalPlaces.Properties.MaxValue = new decimal(new int[] {
			30,
			0,
			0,
			0});
			this.chkUseThousandSeparator.CausesValidation = false;
			resources.ApplyResources(this.chkUseThousandSeparator, "chkUseThousandSeparator");
			this.chkUseThousandSeparator.Name = "chkUseThousandSeparator";
			this.chkUseThousandSeparator.Properties.AccessibleName = resources.GetString("chkUseThousandSeparator.Properties.AccessibleName");
			this.chkUseThousandSeparator.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.CheckButton;
			this.chkUseThousandSeparator.Properties.AutoWidth = true;
			this.chkUseThousandSeparator.Properties.Caption = resources.GetString("chkUseThousandSeparator.Properties.Caption");
			resources.ApplyResources(this.lblNegativeNumbers, "lblNegativeNumbers");
			this.lblNegativeNumbers.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblNegativeNumbers.Name = "lblNegativeNumbers";
			resources.ApplyResources(this.lblSymbol, "lblSymbol");
			this.lblSymbol.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblSymbol.Name = "lblSymbol";
			resources.ApplyResources(this.edtSymbol, "edtSymbol");
			this.edtSymbol.Name = "edtSymbol";
			this.edtSymbol.Properties.AccessibleName = resources.GetString("edtSymbol.Properties.AccessibleName");
			this.edtSymbol.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.edtSymbol.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtSymbol.Properties.Buttons"))))});
			this.edtSymbol.Properties.DropDownRows = 6;
			this.edtSymbol.Properties.NullText = resources.GetString("edtSymbol.Properties.NullText");
			this.edtSymbol.Properties.ShowFooter = false;
			this.edtSymbol.Properties.ShowHeader = false;
			resources.ApplyResources(this.lBoxNegativeNumbers, "lBoxNegativeNumbers");
			this.lBoxNegativeNumbers.AccessibleRole = System.Windows.Forms.AccessibleRole.List;
			this.lBoxNegativeNumbers.Name = "lBoxNegativeNumbers";
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lBoxNegativeNumbers);
			this.Controls.Add(this.edtSymbol);
			this.Controls.Add(this.lblSymbol);
			this.Controls.Add(this.lblNegativeNumbers);
			this.Controls.Add(this.chkUseThousandSeparator);
			this.Controls.Add(this.edtDecimalPlaces);
			this.Controls.Add(this.lbDecimalPlaces);
			this.Name = "FormatNumberDecimalControl";
			((System.ComponentModel.ISupportInitialize)(this.edtDecimalPlaces.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkUseThousandSeparator.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtSymbol.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lBoxNegativeNumbers)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.LabelControl lbDecimalPlaces;
		private XtraEditors.SpinEdit edtDecimalPlaces;
		private XtraEditors.CheckEdit chkUseThousandSeparator;
		private XtraEditors.LabelControl lblNegativeNumbers;
		private XtraEditors.LabelControl lblSymbol;
		private XtraEditors.LookUpEdit edtSymbol;
		private XtraEditors.ListBoxControl lBoxNegativeNumbers;
	}
}
