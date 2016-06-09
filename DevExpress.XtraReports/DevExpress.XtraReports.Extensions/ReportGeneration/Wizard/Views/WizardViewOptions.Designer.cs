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

namespace DevExpress.XtraReports.ReportGeneration.Wizard.Views
{
	partial class WizardViewOptions
	{
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent()
		{
			this.documentViewer2 = new DevExpress.XtraPrinting.Preview.DocumentViewer();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.simpleSeparator2 = new DevExpress.XtraLayout.SimpleSeparator();
			this.simpleLabelItem1 = new DevExpress.XtraLayout.SimpleLabelItem();
			this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.simpleSeparator3 = new DevExpress.XtraLayout.SimpleSeparator();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.wizardBaseGroup)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItemBtnPrev)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.simpleSeparator2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.simpleSeparator3)).BeginInit();
			this.SuspendLayout();
			this.layoutControl1.Controls.Add(this.documentViewer2);
			this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(563, 167, 1351, 835);
			this.layoutControl1.OptionsView.UseParentAutoScaleFactor = true;
			this.layoutControl1.Controls.SetChildIndex(this.buttonPrevious, 0);
			this.layoutControl1.Controls.SetChildIndex(this.buttonNext, 0);
			this.layoutControl1.Controls.SetChildIndex(this.buttonFinish, 0);
			this.layoutControl1.Controls.SetChildIndex(this.documentViewer2, 0);
			this.buttonFinish.Location = new System.Drawing.Point(663, 468);
			this.buttonFinish.Size = new System.Drawing.Size(79, 22);
			this.buttonNext.Location = new System.Drawing.Point(575, 468);
			this.buttonNext.Size = new System.Drawing.Size(82, 22);
			this.emptySpaceItem1.Location = new System.Drawing.Point(0, 456);
			this.emptySpaceItem1.Size = new System.Drawing.Size(486, 46);
			this.layoutControlItem2.Location = new System.Drawing.Point(573, 456);
			this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 3, 12, 3);
			this.layoutControlItem2.Size = new System.Drawing.Size(87, 46);
			this.layoutControlItem2.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 9);
			this.layoutControlItem3.Location = new System.Drawing.Point(660, 456);
			this.layoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 2, 12, 3);
			this.layoutControlItem3.Size = new System.Drawing.Size(94, 46);
			this.layoutControlItem3.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 10, 0, 9);
			this.barAndDockingController1.PropertiesBar.AllowLinkLighting = false;
			this.barAndDockingController1.PropertiesBar.DefaultGlyphSize = new System.Drawing.Size(16, 16);
			this.barAndDockingController1.PropertiesBar.DefaultLargeGlyphSize = new System.Drawing.Size(32, 32);
			this.wizardBaseGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem1,
			this.simpleSeparator2,
			this.simpleLabelItem1,
			this.layoutControlGroup3,
			this.simpleSeparator3});
			this.wizardBaseGroup.Size = new System.Drawing.Size(754, 456);
			this.buttonPrevious.Location = new System.Drawing.Point(488, 468);
			this.buttonPrevious.Size = new System.Drawing.Size(81, 22);
			this.layoutControlItemBtnPrev.Location = new System.Drawing.Point(486, 456);
			this.layoutControlItemBtnPrev.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 4, 12, 2);
			this.layoutControlItemBtnPrev.Size = new System.Drawing.Size(87, 46);
			this.layoutControlItemBtnPrev.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 9);
			this.documentViewer2.HorizontalScrollBarVisibility = DevExpress.XtraEditors.ViewInfo.ScrollBarVisibility.Hidden;
			this.documentViewer2.IsMetric = false;
			this.documentViewer2.Location = new System.Drawing.Point(0, 0);
			this.documentViewer2.Name = "documentViewer2";
			this.documentViewer2.Size = new System.Drawing.Size(488, 454);
			this.documentViewer2.TabIndex = 7;
			this.documentViewer2.VerticalScrollBarVisibility = DevExpress.XtraEditors.ViewInfo.ScrollBarVisibility.Hidden;
			this.layoutControlItem1.Control = this.documentViewer2;
			this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem1.Size = new System.Drawing.Size(488, 454);
			this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.TextVisible = false;
			this.simpleSeparator2.AllowHotTrack = false;
			this.simpleSeparator2.Location = new System.Drawing.Point(488, 0);
			this.simpleSeparator2.Name = "simpleSeparator2";
			this.simpleSeparator2.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 0, 2);
			this.simpleSeparator2.Size = new System.Drawing.Size(2, 454);
			this.simpleLabelItem1.AllowHotTrack = false;
			this.simpleLabelItem1.AppearanceItemCaption.Font = new System.Drawing.Font("Nirmala UI", 12F);
			this.simpleLabelItem1.AppearanceItemCaption.Options.UseFont = true;
			this.simpleLabelItem1.Location = new System.Drawing.Point(490, 0);
			this.simpleLabelItem1.Name = "simpleLabelItem1";
			this.simpleLabelItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 19, 11);
			this.simpleLabelItem1.Size = new System.Drawing.Size(264, 62);
			this.simpleLabelItem1.Spacing = new DevExpress.XtraLayout.Utils.Padding(22, 0, 0, 11);
			this.simpleLabelItem1.Text = "Options";
			this.simpleLabelItem1.TextSize = new System.Drawing.Size(55, 21);
			this.layoutControlGroup3.GroupBordersVisible = false;
			this.layoutControlGroup3.Location = new System.Drawing.Point(490, 62);
			this.layoutControlGroup3.Name = "layoutControlGroup3";
			this.layoutControlGroup3.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 10, 12);
			this.layoutControlGroup3.Size = new System.Drawing.Size(264, 392);
			this.simpleSeparator3.AllowHotTrack = false;
			this.simpleSeparator3.Location = new System.Drawing.Point(0, 454);
			this.simpleSeparator3.Name = "simpleSeparator3";
			this.simpleSeparator3.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 0);
			this.simpleSeparator3.Size = new System.Drawing.Size(754, 2);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.Name = "WizardViewOptions";
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.wizardBaseGroup)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItemBtnPrev)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.simpleSeparator2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.simpleSeparator3)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected XtraEditors.SeparatorControl separatorControl2;
		protected XtraEditors.LabelControl labelControl1;
		protected XtraLayout.SimpleSeparator simpleSeparator1;
		protected XtraPrinting.Preview.DocumentViewer documentViewer2;
		protected XtraLayout.LayoutControlItem layoutControlItem1;
		protected XtraLayout.SimpleSeparator simpleSeparator2;
		protected XtraLayout.SimpleLabelItem simpleLabelItem1;
		protected XtraLayout.LayoutControlGroup layoutControlGroup3;
		private XtraLayout.SimpleSeparator simpleSeparator3;
	}
}
