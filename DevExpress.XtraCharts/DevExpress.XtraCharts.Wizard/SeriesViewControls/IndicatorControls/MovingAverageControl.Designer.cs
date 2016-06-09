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

namespace DevExpress.XtraCharts.Wizard.SeriesViewControls 
{ 
	partial class MovingAverageControl
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
		#region Windows Form Designer generated code
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MovingAverageControl));
			this.panelKind = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbKind = new DevExpress.XtraEditors.ComboBoxEdit();
			this.labelKind = new DevExpress.XtraEditors.LabelControl();
			this.lineStyleControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.LineStyleControl();
			this.panelColor = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.ceColor = new DevExpress.XtraEditors.ColorEdit();
			this.labelColor = new DevExpress.XtraEditors.LabelControl();
			this.pageEnvelopeAppearance = new DevExpress.XtraTab.XtraTabPage();
			this.envelopeLineStyleControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.LineStyleControl();
			this.panelEnvelopeColor = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.ceEnvelopeColor = new DevExpress.XtraEditors.ColorEdit();
			this.labelEnvelopeColor = new DevExpress.XtraEditors.LabelControl();
			this.panelEnvelopePercent = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtEnvelopePercent = new DevExpress.XtraEditors.SpinEdit();
			this.labelEnvelopePercent = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.panelDaysCount)).BeginInit();
			this.panelDaysCount.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtPointsCount.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelValueLevel)).BeginInit();
			this.panelValueLevel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbValueLevel.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl)).BeginInit();
			this.xtraTabControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelName)).BeginInit();
			this.panelName.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkVisible.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkShowInLegend.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkCheckableInLegend.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkCheckedInLegend.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelKind)).BeginInit();
			this.panelKind.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbKind.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelColor)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ceColor.Properties)).BeginInit();
			this.pageEnvelopeAppearance.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelEnvelopeColor)).BeginInit();
			this.panelEnvelopeColor.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ceEnvelopeColor.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelEnvelopePercent)).BeginInit();
			this.panelEnvelopePercent.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtEnvelopePercent.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.panelDaysCount, "panelDaysCount");
			this.txtPointsCount.Properties.Appearance.Options.UseTextOptions = true;
			this.txtPointsCount.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
			this.txtPointsCount.Properties.Mask.EditMask = resources.GetString("txtPointsCount.Properties.Mask.EditMask");
			this.txtPointsCount.Properties.Mask.IgnoreMaskBlank = ((bool)(resources.GetObject("txtPointsCount.Properties.Mask.IgnoreMaskBlank")));
			this.txtPointsCount.Properties.Mask.ShowPlaceHolders = ((bool)(resources.GetObject("txtPointsCount.Properties.Mask.ShowPlaceHolders")));
			resources.ApplyResources(this.panelValueLevel, "panelValueLevel");
			resources.ApplyResources(this.xtraTabControl, "xtraTabControl");
			this.xtraTabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.pageEnvelopeAppearance});
			this.xtraTabControl.Controls.SetChildIndex(this.pageEnvelopeAppearance, 0);
			resources.ApplyResources(this.panelName, "panelName");
			resources.ApplyResources(this.chkVisible, "chkVisible");
			resources.ApplyResources(this.chkShowInLegend, "chkShowInLegend");
			resources.ApplyResources(this.chkCheckableInLegend, "chkCheckableInLegend");
			resources.ApplyResources(this.chkCheckedInLegend, "chkCheckedInLegend");
			resources.ApplyResources(this.panelKind, "panelKind");
			this.panelKind.BackColor = System.Drawing.Color.Transparent;
			this.panelKind.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelKind.Controls.Add(this.cbKind);
			this.panelKind.Controls.Add(this.labelKind);
			this.panelKind.Name = "panelKind";
			resources.ApplyResources(this.cbKind, "cbKind");
			this.cbKind.Name = "cbKind";
			this.cbKind.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbKind.Properties.Buttons"))))});
			this.cbKind.Properties.Items.AddRange(new object[] {
			resources.GetString("cbKind.Properties.Items"),
			resources.GetString("cbKind.Properties.Items1"),
			resources.GetString("cbKind.Properties.Items2")});
			this.cbKind.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbKind.SelectedIndexChanged += new System.EventHandler(this.cbKind_SelectedIndexChanged);
			resources.ApplyResources(this.labelKind, "labelKind");
			this.labelKind.Name = "labelKind";
			resources.ApplyResources(this.lineStyleControl, "lineStyleControl");
			this.lineStyleControl.Name = "lineStyleControl";
			resources.ApplyResources(this.panelColor, "panelColor");
			this.panelColor.BackColor = System.Drawing.Color.Transparent;
			this.panelColor.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelColor.Name = "panelColor";
			resources.ApplyResources(this.ceColor, "ceColor");
			this.ceColor.Name = "ceColor";
			this.ceColor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("ceColor.Properties.Buttons"))))});
			this.ceColor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
			this.ceColor.EditValueChanged += new System.EventHandler(this.ceColor_EditValueChanged);
			resources.ApplyResources(this.labelColor, "labelColor");
			this.labelColor.Name = "labelColor";
			this.pageEnvelopeAppearance.Controls.Add(this.envelopeLineStyleControl);
			this.pageEnvelopeAppearance.Controls.Add(this.panelEnvelopeColor);
			this.pageEnvelopeAppearance.Name = "pageEnvelopeAppearance";
			resources.ApplyResources(this.pageEnvelopeAppearance, "pageEnvelopeAppearance");
			resources.ApplyResources(this.envelopeLineStyleControl, "envelopeLineStyleControl");
			this.envelopeLineStyleControl.Name = "envelopeLineStyleControl";
			resources.ApplyResources(this.panelEnvelopeColor, "panelEnvelopeColor");
			this.panelEnvelopeColor.BackColor = System.Drawing.Color.Transparent;
			this.panelEnvelopeColor.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelEnvelopeColor.Controls.Add(this.ceEnvelopeColor);
			this.panelEnvelopeColor.Controls.Add(this.labelEnvelopeColor);
			this.panelEnvelopeColor.Name = "panelEnvelopeColor";
			resources.ApplyResources(this.ceEnvelopeColor, "ceEnvelopeColor");
			this.ceEnvelopeColor.Name = "ceEnvelopeColor";
			this.ceEnvelopeColor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("ceEnvelopeColor.Properties.Buttons"))))});
			this.ceEnvelopeColor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
			this.ceEnvelopeColor.EditValueChanged += new System.EventHandler(this.ceEnvelopeColor_EditValueChanged);
			resources.ApplyResources(this.labelEnvelopeColor, "labelEnvelopeColor");
			this.labelEnvelopeColor.Name = "labelEnvelopeColor";
			resources.ApplyResources(this.panelEnvelopePercent, "panelEnvelopePercent");
			this.panelEnvelopePercent.BackColor = System.Drawing.Color.Transparent;
			this.panelEnvelopePercent.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelEnvelopePercent.Controls.Add(this.txtEnvelopePercent);
			this.panelEnvelopePercent.Controls.Add(this.labelEnvelopePercent);
			this.panelEnvelopePercent.Name = "panelEnvelopePercent";
			resources.ApplyResources(this.txtEnvelopePercent, "txtEnvelopePercent");
			this.txtEnvelopePercent.Name = "txtEnvelopePercent";
			this.txtEnvelopePercent.Properties.Appearance.Options.UseTextOptions = true;
			this.txtEnvelopePercent.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
			this.txtEnvelopePercent.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtEnvelopePercent.Properties.Mask.IgnoreMaskBlank = ((bool)(resources.GetObject("txtEnvelopePercent.Properties.Mask.IgnoreMaskBlank")));
			this.txtEnvelopePercent.Properties.Mask.ShowPlaceHolders = ((bool)(resources.GetObject("txtEnvelopePercent.Properties.Mask.ShowPlaceHolders")));
			this.txtEnvelopePercent.Properties.MaxValue = new decimal(new int[] {
			100,
			0,
			0,
			0});
			this.txtEnvelopePercent.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.txtEnvelopePercent.EditValueChanged += new System.EventHandler(this.txtEnvelopePercent_EditValueChanged);
			resources.ApplyResources(this.labelEnvelopePercent, "labelEnvelopePercent");
			this.labelEnvelopePercent.Name = "labelEnvelopePercent";
			this.Controls.Add(this.panelEnvelopePercent);
			this.Controls.Add(this.panelKind);
			this.Name = "MovingAverageControl";
			resources.ApplyResources(this, "$this");
			this.Controls.SetChildIndex(this.panelName, 0);
			this.Controls.SetChildIndex(this.panelValueLevel, 0);
			this.Controls.SetChildIndex(this.panelDaysCount, 0);
			this.Controls.SetChildIndex(this.panelKind, 0);
			this.Controls.SetChildIndex(this.panelEnvelopePercent, 0);
			this.Controls.SetChildIndex(this.chkVisible, 0);
			this.Controls.SetChildIndex(this.chkShowInLegend, 0);
			this.Controls.SetChildIndex(this.chkCheckableInLegend, 0);
			this.Controls.SetChildIndex(this.chkCheckedInLegend, 0);
			this.Controls.SetChildIndex(this.sepAppearance, 0);
			this.Controls.SetChildIndex(this.xtraTabControl, 0);
			((System.ComponentModel.ISupportInitialize)(this.panelDaysCount)).EndInit();
			this.panelDaysCount.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtPointsCount.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelValueLevel)).EndInit();
			this.panelValueLevel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbValueLevel.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl)).EndInit();
			this.xtraTabControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelName)).EndInit();
			this.panelName.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkVisible.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkShowInLegend.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkCheckableInLegend.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkCheckedInLegend.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelKind)).EndInit();
			this.panelKind.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbKind.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelColor)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ceColor.Properties)).EndInit();
			this.pageEnvelopeAppearance.ResumeLayout(false);
			this.pageEnvelopeAppearance.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelEnvelopeColor)).EndInit();
			this.panelEnvelopeColor.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ceEnvelopeColor.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelEnvelopePercent)).EndInit();
			this.panelEnvelopePercent.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtEnvelopePercent.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected ChartPanelControl panelKind;
		private DevExpress.XtraEditors.ComboBoxEdit cbKind;
		private DevExpress.XtraEditors.LabelControl labelKind;
		private DevExpress.XtraTab.XtraTabPage pageEnvelopeAppearance;
		private ChartPanelControl panelColor;
		private DevExpress.XtraEditors.ColorEdit ceColor;
		private DevExpress.XtraEditors.LabelControl labelColor;
		private LineStyleControl lineStyleControl;
		private LineStyleControl envelopeLineStyleControl;
		private ChartPanelControl panelEnvelopeColor;
		private DevExpress.XtraEditors.ColorEdit ceEnvelopeColor;
		private DevExpress.XtraEditors.LabelControl labelEnvelopeColor;
		protected ChartPanelControl panelEnvelopePercent;
		private DevExpress.XtraEditors.SpinEdit txtEnvelopePercent;
		private DevExpress.XtraEditors.LabelControl labelEnvelopePercent;
	}
}
