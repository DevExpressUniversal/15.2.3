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
	partial class CurrencyCultureControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CurrencyCultureControl));
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.cbeCurrencyCulture = new DevExpress.XtraEditors.ComboBoxEdit();
			this.cbeCurrency = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblCurrency = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.cbeCurrencyCulture.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbeCurrency.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.Name = "labelControl1";
			resources.ApplyResources(this.cbeCurrencyCulture, "cbeCurrencyCulture");
			this.cbeCurrencyCulture.Name = "cbeCurrencyCulture";
			this.cbeCurrencyCulture.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbeCurrencyCulture.Properties.Buttons"))))});
			this.cbeCurrencyCulture.Properties.Sorted = true;
			this.cbeCurrencyCulture.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbeCurrencyCulture.SelectedIndexChanged += new System.EventHandler(this.OnCurrencyCultureSelectedIndexChanged);
			resources.ApplyResources(this.cbeCurrency, "cbeCurrency");
			this.cbeCurrency.Name = "cbeCurrency";
			this.cbeCurrency.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbeCurrency.Properties.Buttons"))))});
			this.cbeCurrency.Properties.Sorted = true;
			this.cbeCurrency.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbeCurrency.SelectedIndexChanged += new System.EventHandler(this.OnCurrencySelectedIndexChanged);
			resources.ApplyResources(this.lblCurrency, "lblCurrency");
			this.lblCurrency.Name = "lblCurrency";
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.labelControl1);
			this.Controls.Add(this.cbeCurrencyCulture);
			this.Controls.Add(this.cbeCurrency);
			this.Controls.Add(this.lblCurrency);
			this.Name = "CurrencyCultureControl";
			resources.ApplyResources(this, "$this");
			((System.ComponentModel.ISupportInitialize)(this.cbeCurrencyCulture.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbeCurrency.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.LabelControl labelControl1;
		private XtraEditors.ComboBoxEdit cbeCurrencyCulture;
		private XtraEditors.ComboBoxEdit cbeCurrency;
		private XtraEditors.LabelControl lblCurrency;
	}
}
