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

namespace DevExpress.DashboardWin.Native
{
	partial class AxisXSettingsControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AxisXSettingsControl));
			this.panelAxis = new DevExpress.XtraEditors.PanelControl();
			this.ceReverse = new DevExpress.XtraEditors.CheckEdit();
			this.ceTitleVisible = new DevExpress.XtraEditors.CheckEdit();
			this.ceVisible = new DevExpress.XtraEditors.CheckEdit();
			this.panelTitle = new DevExpress.XtraEditors.PanelControl();
			this.seVisiblePointsCount = new DevExpress.XtraEditors.SpinEdit();
			this.cbLimitVisiblePoints = new DevExpress.XtraEditors.CheckEdit();
			this.cbZoomEnabled = new DevExpress.XtraEditors.CheckEdit();
			this.teTitleText = new DevExpress.XtraEditors.TextEdit();
			this.panelTitleText = new DevExpress.XtraEditors.PanelControl();
			this.rbCustomTitleText = new DevExpress.XtraEditors.CheckEdit();
			this.rbDefaultTitleText = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.panelAxis)).BeginInit();
			this.panelAxis.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ceReverse.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ceTitleVisible.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ceVisible.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelTitle)).BeginInit();
			this.panelTitle.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.seVisiblePointsCount.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbLimitVisiblePoints.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbZoomEnabled.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.teTitleText.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelTitleText)).BeginInit();
			this.panelTitleText.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.rbCustomTitleText.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rbDefaultTitleText.Properties)).BeginInit();
			this.SuspendLayout();
			this.panelAxis.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelAxis.Controls.Add(this.ceReverse);
			this.panelAxis.Controls.Add(this.ceTitleVisible);
			this.panelAxis.Controls.Add(this.ceVisible);
			resources.ApplyResources(this.panelAxis, "panelAxis");
			this.panelAxis.Name = "panelAxis";
			resources.ApplyResources(this.ceReverse, "ceReverse");
			this.ceReverse.Name = "ceReverse";
			this.ceReverse.Properties.Caption = resources.GetString("ceReverse.Properties.Caption");
			this.ceReverse.CheckedChanged += new System.EventHandler(this.OnReverseCheckedChanged);
			resources.ApplyResources(this.ceTitleVisible, "ceTitleVisible");
			this.ceTitleVisible.Name = "ceTitleVisible";
			this.ceTitleVisible.Properties.Caption = resources.GetString("ceTitleVisible.Properties.Caption");
			this.ceTitleVisible.CheckedChanged += new System.EventHandler(this.OnTitleVisibleCheckedChanged);
			resources.ApplyResources(this.ceVisible, "ceVisible");
			this.ceVisible.Name = "ceVisible";
			this.ceVisible.Properties.Caption = resources.GetString("ceVisible.Properties.Caption");
			this.ceVisible.CheckedChanged += new System.EventHandler(this.OnVisibleCheckedChanged);
			this.panelTitle.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelTitle.Controls.Add(this.seVisiblePointsCount);
			this.panelTitle.Controls.Add(this.cbLimitVisiblePoints);
			this.panelTitle.Controls.Add(this.cbZoomEnabled);
			this.panelTitle.Controls.Add(this.teTitleText);
			this.panelTitle.Controls.Add(this.panelTitleText);
			resources.ApplyResources(this.panelTitle, "panelTitle");
			this.panelTitle.Name = "panelTitle";
			resources.ApplyResources(this.seVisiblePointsCount, "seVisiblePointsCount");
			this.seVisiblePointsCount.Name = "seVisiblePointsCount";
			this.seVisiblePointsCount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("seVisiblePointsCount.Properties.Buttons"))))});
			this.seVisiblePointsCount.Properties.Mask.EditMask = resources.GetString("seVisiblePointsCount.Properties.Mask.EditMask");
			this.seVisiblePointsCount.Properties.MaxValue = new decimal(new int[] {
			1000000,
			0,
			0,
			0});
			this.seVisiblePointsCount.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.seVisiblePointsCount.Properties.NullText = resources.GetString("seVisiblePointsCount.Properties.NullText");
			this.seVisiblePointsCount.EditValueChanged += new System.EventHandler(this.onVisiblePointsCountEditValueChanged);
			resources.ApplyResources(this.cbLimitVisiblePoints, "cbLimitVisiblePoints");
			this.cbLimitVisiblePoints.Name = "cbLimitVisiblePoints";
			this.cbLimitVisiblePoints.Properties.Caption = resources.GetString("cbLimitVisiblePoints.Properties.Caption");
			this.cbLimitVisiblePoints.CheckedChanged += new System.EventHandler(this.onLimitVisiblePointsCheckedChanged);
			resources.ApplyResources(this.cbZoomEnabled, "cbZoomEnabled");
			this.cbZoomEnabled.Name = "cbZoomEnabled";
			this.cbZoomEnabled.Properties.Caption = resources.GetString("cbZoomEnabled.Properties.Caption");
			this.cbZoomEnabled.CheckedChanged += new System.EventHandler(this.onZoomEnabledCheckedChanged);
			resources.ApplyResources(this.teTitleText, "teTitleText");
			this.teTitleText.Name = "teTitleText";
			this.teTitleText.EditValueChanged += new System.EventHandler(this.OnTitleTextEditValueChanged);
			this.panelTitleText.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("panelTitleText.Appearance.BackColor")));
			this.panelTitleText.Appearance.Options.UseBackColor = true;
			this.panelTitleText.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelTitleText.Controls.Add(this.rbCustomTitleText);
			this.panelTitleText.Controls.Add(this.rbDefaultTitleText);
			resources.ApplyResources(this.panelTitleText, "panelTitleText");
			this.panelTitleText.Name = "panelTitleText";
			resources.ApplyResources(this.rbCustomTitleText, "rbCustomTitleText");
			this.rbCustomTitleText.Name = "rbCustomTitleText";
			this.rbCustomTitleText.Properties.Caption = resources.GetString("rbCustomTitleText.Properties.Caption");
			this.rbCustomTitleText.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.rbCustomTitleText.CheckedChanged += new System.EventHandler(this.OnTitleCustomTextCheckedChanged);
			resources.ApplyResources(this.rbDefaultTitleText, "rbDefaultTitleText");
			this.rbDefaultTitleText.Name = "rbDefaultTitleText";
			this.rbDefaultTitleText.Properties.Caption = resources.GetString("rbDefaultTitleText.Properties.Caption");
			this.rbDefaultTitleText.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.rbDefaultTitleText.CheckedChanged += new System.EventHandler(this.OnTitleDefaultTextCheckedChanged);
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panelTitle);
			this.Controls.Add(this.panelAxis);
			this.Name = "AxisXSettingsControl";
			((System.ComponentModel.ISupportInitialize)(this.panelAxis)).EndInit();
			this.panelAxis.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ceReverse.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ceTitleVisible.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ceVisible.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelTitle)).EndInit();
			this.panelTitle.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.seVisiblePointsCount.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbLimitVisiblePoints.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbZoomEnabled.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.teTitleText.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelTitleText)).EndInit();
			this.panelTitleText.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.rbCustomTitleText.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rbDefaultTitleText.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraEditors.PanelControl panelAxis;
		private DevExpress.XtraEditors.CheckEdit ceVisible;
		private DevExpress.XtraEditors.PanelControl panelTitle;
		private DevExpress.XtraEditors.PanelControl panelTitleText;
		private DevExpress.XtraEditors.CheckEdit rbCustomTitleText;
		private DevExpress.XtraEditors.CheckEdit rbDefaultTitleText;
		private DevExpress.XtraEditors.TextEdit teTitleText;
		private DevExpress.XtraEditors.CheckEdit ceTitleVisible;
		private XtraEditors.CheckEdit ceReverse;
		private XtraEditors.SpinEdit seVisiblePointsCount;
		private XtraEditors.CheckEdit cbLimitVisiblePoints;
		private XtraEditors.CheckEdit cbZoomEnabled;
	}
}
