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

namespace DevExpress.XtraReports.Wizards3.Views {
	partial class ChooseReportStylePageView {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChooseReportStylePageView));
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.sampleDataLabel = new System.Windows.Forms.Label();
			this.sampleCaptionLabel = new System.Windows.Forms.Label();
			this.sampleTitleLabel = new System.Windows.Forms.Label();
			this.reportStyleGroup = new DevExpress.XtraEditors.RadioGroup();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).BeginInit();
			this.layoutControlBase.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupBase)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemFinishButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemNextButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemHeaderLabel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemSeparatorTop)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemPreviousButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelBaseContent)).BeginInit();
			this.panelBaseContent.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemBaseContentPanel)).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.reportStyleGroup.Properties)).BeginInit();
			this.SuspendLayout();
			this.layoutControlBase.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(664, 142, 749, 739);
			this.layoutControlBase.OptionsPrint.AppearanceGroupCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("layoutControlBase.OptionsPrint.AppearanceGroupCaption.BackColor")));
			this.layoutControlBase.OptionsPrint.AppearanceGroupCaption.Font = ((System.Drawing.Font)(resources.GetObject("layoutControlBase.OptionsPrint.AppearanceGroupCaption.Font")));
			this.layoutControlBase.OptionsPrint.AppearanceGroupCaption.Options.UseBackColor = true;
			this.layoutControlBase.OptionsPrint.AppearanceGroupCaption.Options.UseFont = true;
			this.layoutControlBase.OptionsPrint.AppearanceGroupCaption.Options.UseTextOptions = true;
			this.layoutControlBase.OptionsPrint.AppearanceGroupCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.layoutControlBase.OptionsPrint.AppearanceGroupCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
			this.layoutControlBase.OptionsPrint.AppearanceItemCaption.Options.UseTextOptions = true;
			this.layoutControlBase.OptionsPrint.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
			this.layoutControlBase.OptionsPrint.AppearanceItemCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
			this.layoutControlBase.Controls.SetChildIndex(this.buttonPrevious, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.separatorTop, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonFinish, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonNext, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.panelBaseContent, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.labelHeader, 0);
			resources.ApplyResources(this.labelHeader, "labelHeader");
			this.panelBaseContent.Controls.Add(this.tableLayoutPanel1);
			resources.ApplyResources(this.panelBaseContent, "panelBaseContent");
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.reportStyleGroup, 2, 1);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Controls.Add(this.sampleDataLabel);
			this.panel1.Controls.Add(this.sampleCaptionLabel);
			this.panel1.Controls.Add(this.sampleTitleLabel);
			this.panel1.Name = "panel1";
			this.tableLayoutPanel1.SetRowSpan(this.panel1, 7);
			resources.ApplyResources(this.sampleDataLabel, "sampleDataLabel");
			this.sampleDataLabel.Name = "sampleDataLabel";
			resources.ApplyResources(this.sampleCaptionLabel, "sampleCaptionLabel");
			this.sampleCaptionLabel.Name = "sampleCaptionLabel";
			resources.ApplyResources(this.sampleTitleLabel, "sampleTitleLabel");
			this.sampleTitleLabel.Name = "sampleTitleLabel";
			resources.ApplyResources(this.reportStyleGroup, "reportStyleGroup");
			this.reportStyleGroup.Name = "reportStyleGroup";
			this.reportStyleGroup.Properties.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("reportStyleGroup.Properties.Appearance.BackColor")));
			this.reportStyleGroup.Properties.Appearance.Options.UseBackColor = true;
			this.reportStyleGroup.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.reportStyleGroup.Properties.Columns = 1;
			this.reportStyleGroup.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("reportStyleGroup.Properties.Items"))), resources.GetString("reportStyleGroup.Properties.Items1")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("reportStyleGroup.Properties.Items2"))), resources.GetString("reportStyleGroup.Properties.Items3")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("reportStyleGroup.Properties.Items4"))), resources.GetString("reportStyleGroup.Properties.Items5")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("reportStyleGroup.Properties.Items6"))), resources.GetString("reportStyleGroup.Properties.Items7")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("reportStyleGroup.Properties.Items8"))), resources.GetString("reportStyleGroup.Properties.Items9"))});
			this.tableLayoutPanel1.SetRowSpan(this.reportStyleGroup, 5);
			this.reportStyleGroup.SelectedIndexChanged += new System.EventHandler(this.reportStyleGroup_SelectedIndexChanged);
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Name = "ChooseReportStylePageView";
			((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).EndInit();
			this.layoutControlBase.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupBase)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemFinishButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemNextButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemHeaderLabel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemSeparatorTop)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemPreviousButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelBaseContent)).EndInit();
			this.panelBaseContent.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutItemBaseContentPanel)).EndInit();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.reportStyleGroup.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label sampleTitleLabel;
		private XtraEditors.RadioGroup reportStyleGroup;
		private System.Windows.Forms.Label sampleDataLabel;
		private System.Windows.Forms.Label sampleCaptionLabel;
	}
}
