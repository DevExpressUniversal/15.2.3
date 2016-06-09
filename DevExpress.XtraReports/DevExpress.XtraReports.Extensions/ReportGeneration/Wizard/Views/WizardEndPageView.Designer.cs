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
	partial class WizardEndPageView
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
			this.textEdit1 = new DevExpress.XtraEditors.TextEdit();
			this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.simpleLabelItem1 = new DevExpress.XtraLayout.SimpleLabelItem();
			this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.wizardBaseGroup)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItemBtnPrev)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
			this.SuspendLayout();
			this.layoutControl1.Controls.Add(this.checkBox1);
			this.layoutControl1.Controls.Add(this.textEdit1);
			this.layoutControl1.HiddenItems.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem4});
			this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(443, 195, 1467, 806);
			this.layoutControl1.OptionsView.UseParentAutoScaleFactor = true;
			this.layoutControl1.Controls.SetChildIndex(this.buttonPrevious, 0);
			this.layoutControl1.Controls.SetChildIndex(this.buttonNext, 0);
			this.layoutControl1.Controls.SetChildIndex(this.buttonFinish, 0);
			this.layoutControl1.Controls.SetChildIndex(this.textEdit1, 0);
			this.layoutControl1.Controls.SetChildIndex(this.checkBox1, 0);
			this.buttonFinish.Click += new System.EventHandler(this.finishButton_Click);
			this.barAndDockingController1.PropertiesBar.AllowLinkLighting = false;
			this.barAndDockingController1.PropertiesBar.DefaultGlyphSize = new System.Drawing.Size(16, 16);
			this.barAndDockingController1.PropertiesBar.DefaultLargeGlyphSize = new System.Drawing.Size(32, 32);
			this.wizardBaseGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.emptySpaceItem2,
			this.emptySpaceItem3,
			this.layoutControlItem1,
			this.simpleLabelItem1});
			this.textEdit1.Location = new System.Drawing.Point(135, 188);
			this.textEdit1.Name = "textEdit1";
			this.textEdit1.Properties.Appearance.Font = new System.Drawing.Font("Nirmala UI", 12.5F);
			this.textEdit1.Properties.Appearance.Options.UseFont = true;
			this.textEdit1.Size = new System.Drawing.Size(484, 30);
			this.textEdit1.StyleController = this.layoutControl1;
			this.textEdit1.TabIndex = 7;
			this.emptySpaceItem2.AllowHotTrack = false;
			this.emptySpaceItem2.Location = new System.Drawing.Point(0, 0);
			this.emptySpaceItem2.Name = "emptySpaceItem2";
			this.emptySpaceItem2.Size = new System.Drawing.Size(754, 86);
			this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.AppearanceItemCaption.Font = new System.Drawing.Font("Nirmala UI", 8.25F);
			this.layoutControlItem1.AppearanceItemCaption.Options.UseFont = true;
			this.layoutControlItem1.Control = this.textEdit1;
			this.layoutControlItem1.Location = new System.Drawing.Point(0, 185);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(135, 135, 3, 3);
			this.layoutControlItem1.Size = new System.Drawing.Size(754, 36);
			this.layoutControlItem1.TextLocation = DevExpress.Utils.Locations.Top;
			this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.TextVisible = false;
			this.simpleLabelItem1.AllowHotTrack = false;
			this.simpleLabelItem1.AppearanceItemCaption.Font = new System.Drawing.Font("Nirmala UI", 12F);
			this.simpleLabelItem1.AppearanceItemCaption.Options.UseFont = true;
			this.simpleLabelItem1.Location = new System.Drawing.Point(0, 86);
			this.simpleLabelItem1.Name = "simpleLabelItem1";
			this.simpleLabelItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(135, 2, 72, 6);
			this.simpleLabelItem1.Size = new System.Drawing.Size(754, 99);
			this.simpleLabelItem1.Text = "Report Name";
			this.simpleLabelItem1.TextSize = new System.Drawing.Size(93, 21);
			this.emptySpaceItem3.AllowHotTrack = false;
			this.emptySpaceItem3.Location = new System.Drawing.Point(0, 221);
			this.emptySpaceItem3.Name = "emptySpaceItem3";
			this.emptySpaceItem3.Size = new System.Drawing.Size(754, 234);
			this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
			this.checkBox1.Font = new System.Drawing.Font("Nirmala UI", 8.25F);
			this.checkBox1.Location = new System.Drawing.Point(135, 239);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(617, 20);
			this.checkBox1.TabIndex = 8;
			this.checkBox1.Text = "Save To File";
			this.checkBox1.UseVisualStyleBackColor = true;
			this.checkBox1.Visible = false;
			this.layoutControlItem4.Control = this.checkBox1;
			this.layoutControlItem4.Location = new System.Drawing.Point(0, 211);
			this.layoutControlItem4.Name = "layoutControlItem4";
			this.layoutControlItem4.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 1, 2);
			this.layoutControlItem4.Size = new System.Drawing.Size(754, 50);
			this.layoutControlItem4.Spacing = new DevExpress.XtraLayout.Utils.Padding(135, 0, 27, 0);
			this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem4.TextVisible = false;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.Name = "WizardEndPageView";
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.wizardBaseGroup)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItemBtnPrev)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private XtraEditors.TextEdit textEdit1;
		private XtraLayout.EmptySpaceItem emptySpaceItem2;
		private XtraLayout.LayoutControlItem layoutControlItem1;
		private XtraLayout.SimpleLabelItem simpleLabelItem1;
		private System.Windows.Forms.CheckBox checkBox1;
		private XtraLayout.EmptySpaceItem emptySpaceItem3;
		private XtraLayout.LayoutControlItem layoutControlItem4;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
	}
}
