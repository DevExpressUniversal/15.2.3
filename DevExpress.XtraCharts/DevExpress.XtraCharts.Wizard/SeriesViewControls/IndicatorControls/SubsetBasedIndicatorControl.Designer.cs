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

namespace DevExpress.XtraCharts.Wizard.SeriesViewControls {
	internal partial class SubsetBasedIndicatorControl : SingleLevelIndicatorControl {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SubsetBasedIndicatorControl));
			this.panelDaysCount = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtPointsCount = new DevExpress.XtraEditors.SpinEdit();
			this.labelPointsCount = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.panelValueLevel)).BeginInit();
			this.panelValueLevel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbValueLevel.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelName)).BeginInit();
			this.panelName.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkVisible.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkShowInLegend.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkCheckableInLegend.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkCheckedInLegend.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelDaysCount)).BeginInit();
			this.panelDaysCount.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtPointsCount.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.panelValueLevel, "panelValueLevel");
			resources.ApplyResources(this.panelName, "panelName");
			resources.ApplyResources(this.chkVisible, "chkVisible");
			resources.ApplyResources(this.chkShowInLegend, "chkShowInLegend");
			resources.ApplyResources(this.chkCheckableInLegend, "chkCheckableInLegend");
			resources.ApplyResources(this.chkCheckedInLegend, "chkCheckedInLegend");
			resources.ApplyResources(this.panelDaysCount, "panelDaysCount");
			this.panelDaysCount.BackColor = System.Drawing.Color.Transparent;
			this.panelDaysCount.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelDaysCount.Controls.Add(this.txtPointsCount);
			this.panelDaysCount.Controls.Add(this.labelPointsCount);
			this.panelDaysCount.Name = "panelDaysCount";
			resources.ApplyResources(this.txtPointsCount, "txtPointsCount");
			this.txtPointsCount.Name = "txtPointsCount";
			this.txtPointsCount.Properties.Appearance.Options.UseTextOptions = true;
			this.txtPointsCount.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
			this.txtPointsCount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtPointsCount.Properties.IsFloatValue = false;
			this.txtPointsCount.Properties.Mask.EditMask = resources.GetString("txtPointsCount.Properties.Mask.EditMask");
			this.txtPointsCount.Properties.Mask.IgnoreMaskBlank = ((bool)(resources.GetObject("txtPointsCount.Properties.Mask.IgnoreMaskBlank")));
			this.txtPointsCount.Properties.Mask.ShowPlaceHolders = ((bool)(resources.GetObject("txtPointsCount.Properties.Mask.ShowPlaceHolders")));
			this.txtPointsCount.Properties.MaxValue = new decimal(new int[] {
			100,
			0,
			0,
			0});
			this.txtPointsCount.Properties.MinValue = new decimal(new int[] {
			2,
			0,
			0,
			0});
			this.txtPointsCount.EditValueChanged += new System.EventHandler(this.txtPointsCount_EditValueChanged);
			resources.ApplyResources(this.labelPointsCount, "labelPointsCount");
			this.labelPointsCount.Name = "labelPointsCount";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.panelDaysCount);
			this.Name = "SubsetBasedIndicatorControl";
			this.Controls.SetChildIndex(this.chkCheckableInLegend, 0);
			this.Controls.SetChildIndex(this.chkCheckedInLegend, 0);
			this.Controls.SetChildIndex(this.panelName, 0);
			this.Controls.SetChildIndex(this.panelValueLevel, 0);
			this.Controls.SetChildIndex(this.panelDaysCount, 0);
			this.Controls.SetChildIndex(this.chkVisible, 0);
			this.Controls.SetChildIndex(this.chkShowInLegend, 0);
			((System.ComponentModel.ISupportInitialize)(this.panelValueLevel)).EndInit();
			this.panelValueLevel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbValueLevel.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelName)).EndInit();
			this.panelName.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkVisible.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkShowInLegend.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkCheckableInLegend.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkCheckedInLegend.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelDaysCount)).EndInit();
			this.panelDaysCount.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtPointsCount.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected ChartPanelControl panelDaysCount;
		protected DevExpress.XtraEditors.LabelControl labelPointsCount;
		protected DevExpress.XtraEditors.SpinEdit txtPointsCount;
	}
}
