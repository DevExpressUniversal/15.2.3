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

namespace DevExpress.XtraCharts.Wizard.AnnotationControls {
	partial class AnnotationImageContentControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnnotationImageContentControl));
			this.grpImage = new DevExpress.XtraEditors.GroupControl();
			this.imageControl = new DevExpress.XtraCharts.Wizard.ImageControl();
			this.sepImage = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlSideModeBack = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlSizeMode = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbSizeMode = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblSizeMode = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			((System.ComponentModel.ISupportInitialize)(this.grpImage)).BeginInit();
			this.grpImage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.sepImage)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlSideModeBack)).BeginInit();
			this.pnlSideModeBack.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlSizeMode)).BeginInit();
			this.pnlSizeMode.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbSizeMode.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.grpImage, "grpImage");
			this.grpImage.Controls.Add(this.imageControl);
			this.grpImage.Name = "grpImage";
			resources.ApplyResources(this.imageControl, "imageControl");
			this.imageControl.Name = "imageControl";
			this.sepImage.BackColor = System.Drawing.Color.Transparent;
			this.sepImage.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepImage, "sepImage");
			this.sepImage.Name = "sepImage";
			resources.ApplyResources(this.pnlSideModeBack, "pnlSideModeBack");
			this.pnlSideModeBack.BackColor = System.Drawing.Color.Transparent;
			this.pnlSideModeBack.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlSideModeBack.Controls.Add(this.pnlSizeMode);
			this.pnlSideModeBack.Name = "pnlSideModeBack";
			resources.ApplyResources(this.pnlSizeMode, "pnlSizeMode");
			this.pnlSizeMode.BackColor = System.Drawing.Color.Transparent;
			this.pnlSizeMode.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlSizeMode.Controls.Add(this.cbSizeMode);
			this.pnlSizeMode.Controls.Add(this.lblSizeMode);
			this.pnlSizeMode.Name = "pnlSizeMode";
			resources.ApplyResources(this.cbSizeMode, "cbSizeMode");
			this.cbSizeMode.Name = "cbSizeMode";
			this.cbSizeMode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbSizeMode.Properties.Buttons"))))});
			this.cbSizeMode.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbSizeMode.SelectedIndexChanged += new System.EventHandler(this.cbSizeMode_SelectedIndexChanged);
			resources.ApplyResources(this.lblSizeMode, "lblSizeMode");
			this.lblSizeMode.Name = "lblSizeMode";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.pnlSideModeBack);
			this.Controls.Add(this.sepImage);
			this.Controls.Add(this.grpImage);
			this.Name = "AnnotationImageContentControl";
			((System.ComponentModel.ISupportInitialize)(this.grpImage)).EndInit();
			this.grpImage.ResumeLayout(false);
			this.grpImage.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.sepImage)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlSideModeBack)).EndInit();
			this.pnlSideModeBack.ResumeLayout(false);
			this.pnlSideModeBack.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlSizeMode)).EndInit();
			this.pnlSizeMode.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbSizeMode.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.GroupControl grpImage;
		private ImageControl imageControl;
		private ChartPanelControl sepImage;
		private ChartPanelControl pnlSideModeBack;
		private ChartPanelControl pnlSizeMode;
		private ChartLabelControl lblSizeMode;
		private DevExpress.XtraEditors.ComboBoxEdit cbSizeMode;
	}
}
