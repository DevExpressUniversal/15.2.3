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
	partial class WizardPageView
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
			this.components = new System.ComponentModel.Container();
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.buttonPrevious = new DevExpress.XtraEditors.SimpleButton();
			this.buttonFinish = new DevExpress.XtraEditors.SimpleButton();
			this.buttonNext = new DevExpress.XtraEditors.SimpleButton();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
			this.wizardBaseGroup = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItemBtnPrev = new DevExpress.XtraLayout.LayoutControlItem();
			this.barAndDockingController1 = new DevExpress.XtraBars.BarAndDockingController(this.components);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.wizardBaseGroup)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItemBtnPrev)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).BeginInit();
			this.SuspendLayout();
			this.layoutControl1.AllowCustomization = false;
			this.layoutControl1.Controls.Add(this.buttonPrevious);
			this.layoutControl1.Controls.Add(this.buttonFinish);
			this.layoutControl1.Controls.Add(this.buttonNext);
			this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControl1.Location = new System.Drawing.Point(0, 0);
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(842, 199, 1055, 679);
			this.layoutControl1.OptionsView.UseParentAutoScaleFactor = true;
			this.layoutControl1.Root = this.layoutControlGroup1;
			this.layoutControl1.Size = new System.Drawing.Size(754, 502);
			this.layoutControl1.TabIndex = 0;
			this.layoutControl1.Text = "layoutControl1";
			this.buttonPrevious.Location = new System.Drawing.Point(490, 468);
			this.buttonPrevious.Name = "buttonPrevious";
			this.buttonPrevious.Size = new System.Drawing.Size(80, 22);
			this.buttonPrevious.StyleController = this.layoutControl1;
			this.buttonPrevious.TabIndex = 7;
			this.buttonPrevious.Text = "Back";
			this.buttonFinish.Location = new System.Drawing.Point(662, 468);
			this.buttonFinish.Name = "buttonFinish";
			this.buttonFinish.Size = new System.Drawing.Size(80, 22);
			this.buttonFinish.StyleController = this.layoutControl1;
			this.buttonFinish.TabIndex = 6;
			this.buttonFinish.Text = "Finish";
			this.buttonNext.Location = new System.Drawing.Point(576, 468);
			this.buttonNext.Name = "buttonNext";
			this.buttonNext.Size = new System.Drawing.Size(80, 22);
			this.buttonNext.StyleController = this.layoutControl1;
			this.buttonNext.TabIndex = 5;
			this.buttonNext.Text = "Next";
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.emptySpaceItem1,
			this.layoutControlItem2,
			this.layoutControlItem3,
			this.wizardBaseGroup,
			this.layoutControlItemBtnPrev});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "Root";
			this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroup1.Size = new System.Drawing.Size(754, 502);
			this.layoutControlGroup1.TextVisible = false;
			this.emptySpaceItem1.AllowHotTrack = false;
			this.emptySpaceItem1.Location = new System.Drawing.Point(0, 455);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Size = new System.Drawing.Size(488, 47);
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.Control = this.buttonNext;
			this.layoutControlItem2.Location = new System.Drawing.Point(574, 455);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 3, 13, 2);
			this.layoutControlItem2.Size = new System.Drawing.Size(85, 47);
			this.layoutControlItem2.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 10);
			this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.TextVisible = false;
			this.layoutControlItem3.Control = this.buttonFinish;
			this.layoutControlItem3.Location = new System.Drawing.Point(659, 455);
			this.layoutControlItem3.Name = "layoutControlItem3";
			this.layoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 2, 13, 2);
			this.layoutControlItem3.Size = new System.Drawing.Size(95, 47);
			this.layoutControlItem3.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 10, 0, 10);
			this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem3.TextVisible = false;
			this.wizardBaseGroup.GroupBordersVisible = false;
			this.wizardBaseGroup.Location = new System.Drawing.Point(0, 0);
			this.wizardBaseGroup.Name = "wizardBaseGroup";
			this.wizardBaseGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.wizardBaseGroup.Size = new System.Drawing.Size(754, 455);
			this.layoutControlItemBtnPrev.Control = this.buttonPrevious;
			this.layoutControlItemBtnPrev.Location = new System.Drawing.Point(488, 455);
			this.layoutControlItemBtnPrev.Name = "layoutControlItemBtnPrev";
			this.layoutControlItemBtnPrev.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 4, 13, 2);
			this.layoutControlItemBtnPrev.Size = new System.Drawing.Size(86, 47);
			this.layoutControlItemBtnPrev.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 10);
			this.layoutControlItemBtnPrev.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItemBtnPrev.TextVisible = false;
			this.barAndDockingController1.PropertiesBar.AllowLinkLighting = false;
			this.barAndDockingController1.PropertiesBar.DefaultGlyphSize = new System.Drawing.Size(16, 16);
			this.barAndDockingController1.PropertiesBar.DefaultLargeGlyphSize = new System.Drawing.Size(32, 32);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.layoutControl1);
			this.Name = "WizardPageView";
			this.Size = new System.Drawing.Size(754, 502);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.wizardBaseGroup)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItemBtnPrev)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected XtraLayout.LayoutControl layoutControl1;
		protected XtraEditors.SimpleButton buttonFinish;
		protected XtraEditors.SimpleButton buttonNext;
		protected XtraLayout.LayoutControlGroup layoutControlGroup1;
		protected XtraLayout.EmptySpaceItem emptySpaceItem1;
		protected XtraLayout.LayoutControlItem layoutControlItem2;
		protected XtraLayout.LayoutControlItem layoutControlItem3;
		protected XtraBars.BarAndDockingController barAndDockingController1;
		protected XtraLayout.LayoutControlGroup wizardBaseGroup;
		protected XtraEditors.SimpleButton buttonPrevious;
		protected XtraLayout.LayoutControlItem layoutControlItemBtnPrev;
	}
}
