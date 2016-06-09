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

namespace DevExpress.XtraCharts.Wizard {
	partial class ImageControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageControl));
			this.pnlImageSettings = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlStretch = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chStretch = new DevExpress.XtraEditors.CheckEdit();
			this.btnClear = new DevExpress.XtraEditors.SimpleButton();
			this.sepInage = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.beImage = new DevExpress.XtraEditors.ButtonEdit();
			((System.ComponentModel.ISupportInitialize)(this.pnlImageSettings)).BeginInit();
			this.pnlImageSettings.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlStretch)).BeginInit();
			this.pnlStretch.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chStretch.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepInage)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.beImage.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.pnlImageSettings, "pnlImageSettings");
			this.pnlImageSettings.BackColor = System.Drawing.Color.Transparent;
			this.pnlImageSettings.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlImageSettings.Controls.Add(this.pnlStretch);
			this.pnlImageSettings.Controls.Add(this.btnClear);
			this.pnlImageSettings.Name = "pnlImageSettings";
			this.pnlStretch.BackColor = System.Drawing.Color.Transparent;
			this.pnlStretch.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlStretch.Controls.Add(this.chStretch);
			resources.ApplyResources(this.pnlStretch, "pnlStretch");
			this.pnlStretch.Name = "pnlStretch";
			resources.ApplyResources(this.chStretch, "chStretch");
			this.chStretch.Name = "chStretch";
			this.chStretch.Properties.AutoHeight = ((bool)(resources.GetObject("chStretch.Properties.AutoHeight")));
			this.chStretch.Properties.Caption = resources.GetString("chStretch.Properties.Caption");
			this.chStretch.CheckedChanged += new System.EventHandler(this.chStretch_CheckedChanged);
			resources.ApplyResources(this.btnClear, "btnClear");
			this.btnClear.Name = "btnClear";
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
			this.sepInage.BackColor = System.Drawing.Color.Transparent;
			this.sepInage.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepInage, "sepInage");
			this.sepInage.Name = "sepInage";
			resources.ApplyResources(this.beImage, "beImage");
			this.beImage.Name = "beImage";
			this.beImage.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.beImage.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.beImage.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.beImage_ButtonClick);
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.pnlImageSettings);
			this.Controls.Add(this.sepInage);
			this.Controls.Add(this.beImage);
			this.Name = "ImageControl";
			((System.ComponentModel.ISupportInitialize)(this.pnlImageSettings)).EndInit();
			this.pnlImageSettings.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlStretch)).EndInit();
			this.pnlStretch.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chStretch.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepInage)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.beImage.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private ChartPanelControl pnlImageSettings;
		private DevExpress.XtraEditors.SimpleButton btnClear;
		private ChartPanelControl sepInage;
		private DevExpress.XtraEditors.ButtonEdit beImage;
		private ChartPanelControl pnlStretch;
		private DevExpress.XtraEditors.CheckEdit chStretch;
	}
}
