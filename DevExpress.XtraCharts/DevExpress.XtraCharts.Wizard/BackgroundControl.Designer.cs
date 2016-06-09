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
	partial class BackgroundControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BackgroundControl));
			this.pnlBackgroundColor = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.grpBackgroundColor = new DevExpress.XtraEditors.GroupControl();
			this.clreColor = new DevExpress.XtraEditors.ColorEdit();
			this.lblColor = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.sepColor = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlFillStyles = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.grpFillStyle = new DevExpress.XtraEditors.GroupControl();
			this.fillStylesControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.FillStylesControl();
			this.sepFillStyle = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlBackgroundImage = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.grpBackgroundImage = new DevExpress.XtraEditors.GroupControl();
			this.imageControl = new DevExpress.XtraCharts.Wizard.ImageControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlBackgroundColor)).BeginInit();
			this.pnlBackgroundColor.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.grpBackgroundColor)).BeginInit();
			this.grpBackgroundColor.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.clreColor.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepColor)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlFillStyles)).BeginInit();
			this.pnlFillStyles.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.grpFillStyle)).BeginInit();
			this.grpFillStyle.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.sepFillStyle)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlBackgroundImage)).BeginInit();
			this.pnlBackgroundImage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.grpBackgroundImage)).BeginInit();
			this.grpBackgroundImage.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.pnlBackgroundColor, "pnlBackgroundColor");
			this.pnlBackgroundColor.BackColor = System.Drawing.Color.Transparent;
			this.pnlBackgroundColor.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlBackgroundColor.Controls.Add(this.grpBackgroundColor);
			this.pnlBackgroundColor.Name = "pnlBackgroundColor";
			resources.ApplyResources(this.grpBackgroundColor, "grpBackgroundColor");
			this.grpBackgroundColor.Controls.Add(this.clreColor);
			this.grpBackgroundColor.Controls.Add(this.lblColor);
			this.grpBackgroundColor.Name = "grpBackgroundColor";
			resources.ApplyResources(this.clreColor, "clreColor");
			this.clreColor.Name = "clreColor";
			this.clreColor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("clreColor.Properties.Buttons"))))});
			this.clreColor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
			this.clreColor.EditValueChanged += new System.EventHandler(this.clreColor_EditValueChanged);
			resources.ApplyResources(this.lblColor, "lblColor");
			this.lblColor.Name = "lblColor";
			this.sepColor.BackColor = System.Drawing.Color.Transparent;
			this.sepColor.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepColor, "sepColor");
			this.sepColor.Name = "sepColor";
			resources.ApplyResources(this.pnlFillStyles, "pnlFillStyles");
			this.pnlFillStyles.BackColor = System.Drawing.Color.Transparent;
			this.pnlFillStyles.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlFillStyles.Controls.Add(this.grpFillStyle);
			this.pnlFillStyles.Name = "pnlFillStyles";
			resources.ApplyResources(this.grpFillStyle, "grpFillStyle");
			this.grpFillStyle.Controls.Add(this.fillStylesControl);
			this.grpFillStyle.Name = "grpFillStyle";
			resources.ApplyResources(this.fillStylesControl, "fillStylesControl");
			this.fillStylesControl.Name = "fillStylesControl";
			this.sepFillStyle.BackColor = System.Drawing.Color.Transparent;
			this.sepFillStyle.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepFillStyle, "sepFillStyle");
			this.sepFillStyle.Name = "sepFillStyle";
			resources.ApplyResources(this.pnlBackgroundImage, "pnlBackgroundImage");
			this.pnlBackgroundImage.BackColor = System.Drawing.Color.Transparent;
			this.pnlBackgroundImage.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlBackgroundImage.Controls.Add(this.grpBackgroundImage);
			this.pnlBackgroundImage.Name = "pnlBackgroundImage";
			resources.ApplyResources(this.grpBackgroundImage, "grpBackgroundImage");
			this.grpBackgroundImage.Controls.Add(this.imageControl);
			this.grpBackgroundImage.Name = "grpBackgroundImage";
			resources.ApplyResources(this.imageControl, "imageControl");
			this.imageControl.Name = "imageControl";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.pnlBackgroundImage);
			this.Controls.Add(this.sepFillStyle);
			this.Controls.Add(this.pnlFillStyles);
			this.Controls.Add(this.sepColor);
			this.Controls.Add(this.pnlBackgroundColor);
			this.Name = "BackgroundControl";
			((System.ComponentModel.ISupportInitialize)(this.pnlBackgroundColor)).EndInit();
			this.pnlBackgroundColor.ResumeLayout(false);
			this.pnlBackgroundColor.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.grpBackgroundColor)).EndInit();
			this.grpBackgroundColor.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.clreColor.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepColor)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlFillStyles)).EndInit();
			this.pnlFillStyles.ResumeLayout(false);
			this.pnlFillStyles.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.grpFillStyle)).EndInit();
			this.grpFillStyle.ResumeLayout(false);
			this.grpFillStyle.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.sepFillStyle)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlBackgroundImage)).EndInit();
			this.pnlBackgroundImage.ResumeLayout(false);
			this.pnlBackgroundImage.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.grpBackgroundImage)).EndInit();
			this.grpBackgroundImage.ResumeLayout(false);
			this.grpBackgroundImage.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraCharts.Wizard.ChartPanelControl pnlBackgroundColor;
		private DevExpress.XtraEditors.GroupControl grpBackgroundColor;
		private DevExpress.XtraCharts.Wizard.ChartLabelControl lblColor;
		private DevExpress.XtraEditors.ColorEdit clreColor;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl sepColor;
		private DevExpress.XtraCharts.Wizard.ChartPanelControl pnlFillStyles;
		private DevExpress.XtraEditors.GroupControl grpFillStyle;
		private DevExpress.XtraCharts.Wizard.SeriesViewControls.FillStylesControl fillStylesControl;
		private ChartPanelControl sepFillStyle;
		private ChartPanelControl pnlBackgroundImage;
		private DevExpress.XtraEditors.GroupControl grpBackgroundImage;
		private ImageControl imageControl;
	}
}
