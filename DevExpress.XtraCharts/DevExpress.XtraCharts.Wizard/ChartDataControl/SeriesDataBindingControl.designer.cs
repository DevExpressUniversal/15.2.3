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

using DevExpress.XtraEditors;
namespace DevExpress.XtraCharts.Wizard {
	partial class SeriesDataBindingControl {
		protected override void Dispose( bool disposing )
		{
			if (disposing) {
				if (components != null)
					components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Designer generated code
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SeriesDataBindingControl));
			this.splitContainerControl = new DevExpress.XtraEditors.SplitContainerControl();
			this.lvSeries = new DevExpress.XtraCharts.Wizard.SeriesListBoxControl();
			this.panel = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.dataBindingControl = new DevExpress.XtraCharts.Wizard.DataBindingControl();
			this.pnlBindingOffset = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.grSeriesTemplate = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlDataFilters = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.beFilters = new DevExpress.XtraEditors.ButtonEdit();
			this.lblDataFilters = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.pnlDataFiltersOffset = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlDataMember = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbChartDataMember = new DevExpress.XtraEditors.PopupContainerEdit();
			this.lblDataMember = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.pnlDataMemberOffset = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlDataSource = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbDataSource = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblDataSource = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.pnlLeftMargin = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlRightMargin = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.seriesImages = new System.Windows.Forms.ImageList(this.components);
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl)).BeginInit();
			this.splitContainerControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panel)).BeginInit();
			this.panel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlBindingOffset)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grSeriesTemplate)).BeginInit();
			this.grSeriesTemplate.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlDataFilters)).BeginInit();
			this.pnlDataFilters.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.beFilters.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlDataFiltersOffset)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlDataMember)).BeginInit();
			this.pnlDataMember.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbChartDataMember.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlDataMemberOffset)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlDataSource)).BeginInit();
			this.pnlDataSource.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbDataSource.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlLeftMargin)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlRightMargin)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.splitContainerControl, "splitContainerControl");
			this.splitContainerControl.Name = "splitContainerControl";
			this.splitContainerControl.Panel1.Controls.Add(this.lvSeries);
			this.splitContainerControl.Panel2.Controls.Add(this.panel);
			this.splitContainerControl.SplitterPosition = 320;
			resources.ApplyResources(this.lvSeries, "lvSeries");
			this.lvSeries.Name = "lvSeries";
			this.lvSeries.SelectedSeriesChanged += new System.EventHandler(this.lvSeries_SelectedIndexChanged);
			this.panel.BackColor = System.Drawing.Color.Transparent;
			this.panel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panel.Controls.Add(this.dataBindingControl);
			this.panel.Controls.Add(this.pnlBindingOffset);
			this.panel.Controls.Add(this.grSeriesTemplate);
			resources.ApplyResources(this.panel, "panel");
			this.panel.Name = "panel";
			resources.ApplyResources(this.dataBindingControl, "dataBindingControl");
			this.dataBindingControl.Name = "dataBindingControl";
			this.dataBindingControl.ValuableBindingChanged += new System.EventHandler(this.dataBindingControl_ValuableBindingChanged);
			this.pnlBindingOffset.BackColor = System.Drawing.Color.Transparent;
			this.pnlBindingOffset.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.pnlBindingOffset, "pnlBindingOffset");
			this.pnlBindingOffset.Name = "pnlBindingOffset";
			resources.ApplyResources(this.grSeriesTemplate, "grSeriesTemplate");
			this.grSeriesTemplate.BackColor = System.Drawing.Color.Transparent;
			this.grSeriesTemplate.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.grSeriesTemplate.Controls.Add(this.pnlDataFilters);
			this.grSeriesTemplate.Controls.Add(this.pnlDataFiltersOffset);
			this.grSeriesTemplate.Controls.Add(this.pnlDataMember);
			this.grSeriesTemplate.Controls.Add(this.pnlDataSource);
			this.grSeriesTemplate.Controls.Add(this.pnlLeftMargin);
			this.grSeriesTemplate.Controls.Add(this.pnlRightMargin);
			this.grSeriesTemplate.Name = "grSeriesTemplate";
			resources.ApplyResources(this.pnlDataFilters, "pnlDataFilters");
			this.pnlDataFilters.BackColor = System.Drawing.Color.Transparent;
			this.pnlDataFilters.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlDataFilters.Controls.Add(this.beFilters);
			this.pnlDataFilters.Controls.Add(this.lblDataFilters);
			this.pnlDataFilters.Name = "pnlDataFilters";
			resources.ApplyResources(this.beFilters, "beFilters");
			this.beFilters.Name = "beFilters";
			this.beFilters.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.beFilters.Properties.ReadOnly = true;
			this.beFilters.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.beFilters.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.beFilters_ButtonClick);
			resources.ApplyResources(this.lblDataFilters, "lblDataFilters");
			this.lblDataFilters.Name = "lblDataFilters";
			this.pnlDataFiltersOffset.BackColor = System.Drawing.Color.Transparent;
			this.pnlDataFiltersOffset.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.pnlDataFiltersOffset, "pnlDataFiltersOffset");
			this.pnlDataFiltersOffset.Name = "pnlDataFiltersOffset";
			resources.ApplyResources(this.pnlDataMember, "pnlDataMember");
			this.pnlDataMember.BackColor = System.Drawing.Color.Transparent;
			this.pnlDataMember.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlDataMember.Controls.Add(this.cbChartDataMember);
			this.pnlDataMember.Controls.Add(this.lblDataMember);
			this.pnlDataMember.Controls.Add(this.pnlDataMemberOffset);
			this.pnlDataMember.Name = "pnlDataMember";
			resources.ApplyResources(this.cbChartDataMember, "cbChartDataMember");
			this.cbChartDataMember.EnterMoveNextControl = true;
			this.cbChartDataMember.Name = "cbChartDataMember";
			this.cbChartDataMember.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbChartDataMember.Properties.Buttons"))))});
			this.cbChartDataMember.Properties.ReadOnly = true;
			this.cbChartDataMember.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cbChartDataMember_CustomDisplayText);
			resources.ApplyResources(this.lblDataMember, "lblDataMember");
			this.lblDataMember.Name = "lblDataMember";
			this.pnlDataMemberOffset.BackColor = System.Drawing.Color.Transparent;
			this.pnlDataMemberOffset.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.pnlDataMemberOffset, "pnlDataMemberOffset");
			this.pnlDataMemberOffset.Name = "pnlDataMemberOffset";
			resources.ApplyResources(this.pnlDataSource, "pnlDataSource");
			this.pnlDataSource.BackColor = System.Drawing.Color.Transparent;
			this.pnlDataSource.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlDataSource.Controls.Add(this.cbDataSource);
			this.pnlDataSource.Controls.Add(this.lblDataSource);
			this.pnlDataSource.Name = "pnlDataSource";
			resources.ApplyResources(this.cbDataSource, "cbDataSource");
			this.cbDataSource.Name = "cbDataSource";
			this.cbDataSource.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbDataSource.Properties.Buttons"))))});
			this.cbDataSource.Properties.Items.AddRange(new object[] {
			resources.GetString("cbDataSource.Properties.Items"),
			resources.GetString("cbDataSource.Properties.Items1"),
			resources.GetString("cbDataSource.Properties.Items2")});
			this.cbDataSource.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbDataSource.SelectedIndexChanged += new System.EventHandler(this.cbDataSource_SelectedIndexChanged);
			resources.ApplyResources(this.lblDataSource, "lblDataSource");
			this.lblDataSource.Name = "lblDataSource";
			this.pnlLeftMargin.BackColor = System.Drawing.Color.Transparent;
			this.pnlLeftMargin.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.pnlLeftMargin, "pnlLeftMargin");
			this.pnlLeftMargin.Name = "pnlLeftMargin";
			this.pnlRightMargin.BackColor = System.Drawing.Color.Transparent;
			this.pnlRightMargin.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.pnlRightMargin, "pnlRightMargin");
			this.pnlRightMargin.Name = "pnlRightMargin";
			this.seriesImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
			resources.ApplyResources(this.seriesImages, "seriesImages");
			this.seriesImages.TransparentColor = System.Drawing.Color.Magenta;
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Magenta;
			this.imageList.Images.SetKeyName(0, "up.png");
			this.imageList.Images.SetKeyName(1, "down.png");
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.splitContainerControl);
			this.Name = "SeriesDataBindingControl";
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl)).EndInit();
			this.splitContainerControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panel)).EndInit();
			this.panel.ResumeLayout(false);
			this.panel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlBindingOffset)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grSeriesTemplate)).EndInit();
			this.grSeriesTemplate.ResumeLayout(false);
			this.grSeriesTemplate.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlDataFilters)).EndInit();
			this.pnlDataFilters.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.beFilters.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlDataFiltersOffset)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlDataMember)).EndInit();
			this.pnlDataMember.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbChartDataMember.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlDataMemberOffset)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlDataSource)).EndInit();
			this.pnlDataSource.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbDataSource.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlLeftMargin)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlRightMargin)).EndInit();
			this.ResumeLayout(false);
		}
#endregion
		System.Windows.Forms.ImageList imageList;
		System.ComponentModel.IContainer components;
		System.Windows.Forms.ImageList seriesImages;
		ChartPanelControl panel;
		private DataBindingControl dataBindingControl;
		private ChartPanelControl pnlBindingOffset;
		private ChartPanelControl grSeriesTemplate;
		private ChartPanelControl pnlDataFilters;
		private ButtonEdit beFilters;
		private ChartLabelControl lblDataFilters;
		private ChartPanelControl pnlDataFiltersOffset;
		private ChartPanelControl pnlDataMember;
		private PopupContainerEdit cbChartDataMember;
		private ChartLabelControl lblDataMember;
		private ChartPanelControl pnlDataMemberOffset;
		private ChartPanelControl pnlDataSource;
		private ComboBoxEdit cbDataSource;
		private ChartLabelControl lblDataSource;
		private ChartPanelControl pnlLeftMargin;
		private ChartPanelControl pnlRightMargin;
		private SplitContainerControl splitContainerControl;
		private SeriesListBoxControl lvSeries;
	}
}
