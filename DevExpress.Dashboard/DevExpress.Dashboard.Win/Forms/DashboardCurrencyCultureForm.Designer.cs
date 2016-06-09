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
	partial class DashboardCurrencyCultureForm {
		#region Windows Form Designer generated code
		#endregion
		private CurrencyCultureControl currencyChooser;
		private NumericFormatPreviewControl currencyPreview;
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DashboardCurrencyCultureForm));
			this.currencyChooser = new DevExpress.DashboardWin.Native.CurrencyCultureControl();
			this.currencyPreview = new DevExpress.DashboardWin.Native.NumericFormatPreviewControl();
			this.panelOkCancel = new DevExpress.XtraEditors.PanelControl();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.currencyPreview.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelOkCancel)).BeginInit();
			this.panelOkCancel.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.currencyChooser, "currencyChooser");
			this.currencyChooser.Name = "currencyChooser";
			resources.ApplyResources(this.currencyPreview, "currencyPreview");
			this.currencyPreview.Name = "currencyPreview";
			this.currencyPreview.Properties.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("currencyPreview.Properties.Appearance.Font")));
			this.currencyPreview.Properties.Appearance.Options.UseFont = true;
			this.currencyPreview.Properties.Appearance.Options.UseTextOptions = true;
			this.currencyPreview.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.currencyPreview.Properties.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
			this.currencyPreview.Properties.ReadOnly = true;
			this.currencyPreview.Properties.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.currencyPreview.TabStop = false;
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
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.panelOkCancel);
			this.Controls.Add(this.currencyChooser);
			this.Controls.Add(this.currencyPreview);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DashboardCurrencyCultureForm";
			this.ShowIcon = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			((System.ComponentModel.ISupportInitialize)(this.currencyPreview.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelOkCancel)).EndInit();
			this.panelOkCancel.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private XtraEditors.PanelControl panelOkCancel;
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.SimpleButton btnOk;
	}
}
