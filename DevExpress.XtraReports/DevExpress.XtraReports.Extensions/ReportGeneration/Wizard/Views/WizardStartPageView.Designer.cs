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
	partial class WizardStartPageView
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
			this.listBoxControl1 = new DevExpress.XtraEditors.ListBoxControl();
			this.simpleLabelItem1 = new DevExpress.XtraLayout.SimpleLabelItem();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.wizardBaseGroup)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItemBtnPrev)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.listBoxControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			this.SuspendLayout();
			this.layoutControl1.Controls.Add(this.listBoxControl1);
			this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2018, 102, 1467, 806);
			this.layoutControl1.OptionsView.UseParentAutoScaleFactor = true;
			this.layoutControl1.Controls.SetChildIndex(this.buttonPrevious, 0);
			this.layoutControl1.Controls.SetChildIndex(this.buttonNext, 0);
			this.layoutControl1.Controls.SetChildIndex(this.buttonFinish, 0);
			this.layoutControl1.Controls.SetChildIndex(this.listBoxControl1, 0);
			this.barAndDockingController1.PropertiesBar.AllowLinkLighting = false;
			this.barAndDockingController1.PropertiesBar.DefaultGlyphSize = new System.Drawing.Size(16, 16);
			this.barAndDockingController1.PropertiesBar.DefaultLargeGlyphSize = new System.Drawing.Size(32, 32);
			this.wizardBaseGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.simpleLabelItem1,
			this.layoutControlItem1});
			this.listBoxControl1.Location = new System.Drawing.Point(135, 101);
			this.listBoxControl1.Name = "listBoxControl1";
			this.listBoxControl1.Size = new System.Drawing.Size(484, 284);
			this.listBoxControl1.StyleController = this.layoutControl1;
			this.listBoxControl1.TabIndex = 7;
			this.simpleLabelItem1.AllowHotTrack = false;
			this.simpleLabelItem1.AppearanceItemCaption.Font = new System.Drawing.Font("Nirmala UI", 12F);
			this.simpleLabelItem1.AppearanceItemCaption.Options.UseFont = true;
			this.simpleLabelItem1.Location = new System.Drawing.Point(0, 0);
			this.simpleLabelItem1.Name = "simpleLabelItem1";
			this.simpleLabelItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(135, 150, 70, 0);
			this.simpleLabelItem1.Size = new System.Drawing.Size(754, 101);
			this.simpleLabelItem1.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 10);
			this.simpleLabelItem1.Text = "Select the source GridView to generate a report";
			this.simpleLabelItem1.TextSize = new System.Drawing.Size(327, 21);
			this.layoutControlItem1.Control = this.listBoxControl1;
			this.layoutControlItem1.Location = new System.Drawing.Point(0, 101);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(135, 135, 0, 70);
			this.layoutControlItem1.Size = new System.Drawing.Size(754, 354);
			this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.TextVisible = false;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.Name = "WizardStartPageView";
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.wizardBaseGroup)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItemBtnPrev)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.listBoxControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private XtraEditors.ListBoxControl listBoxControl1;
		private XtraLayout.SimpleLabelItem simpleLabelItem1;
		private XtraLayout.LayoutControlItem layoutControlItem1;
	}
}
