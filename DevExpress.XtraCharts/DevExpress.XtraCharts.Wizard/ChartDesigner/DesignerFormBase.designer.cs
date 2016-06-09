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

using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraCharts.Wizard;
using DevExpress.XtraCharts.Designer;
namespace DevExpress.XtraCharts.Designer.Native {
	partial class DesignerFormBase {
		private IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing) {
				designerController.Dispose();
				if (components != null)
					components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DesignerFormBase));
			this.cbShowOnNextStart = new DevExpress.XtraEditors.CheckEdit();
			this.sbOK = new DevExpress.XtraEditors.SimpleButton();
			this.sbCancel = new DevExpress.XtraEditors.SimpleButton();
			this.hitTestTransparentPanelControl = new DevExpress.XtraCharts.Wizard.HitTestTransparentPanelControl();
			this.ucDockUserControl = new DevExpress.XtraGrid.Views.Grid.EditFormUserControl();
			this.ucChartPreview = new DevExpress.XtraCharts.Designer.ChartPreviewControl();
			this.ucOptionsControl = new DevExpress.XtraCharts.Designer.ChartElementsOptionsControl();
			this.ucPropertyGrid = new DevExpress.XtraCharts.Designer.ChartPropertyGridControl();
			this.ucChartTree = new DevExpress.XtraCharts.Designer.ChartStructureControl();
			this.pnlStructure = new DevExpress.XtraEditors.PanelControl();
			this.pnlActions = new DevExpress.XtraEditors.PanelControl();
			this.ucActionsControl = new DevExpress.XtraCharts.Designer.ChartActionsControl();
			this.splitterControl1 = new DevExpress.XtraCharts.Designer.Native.DesignerSplitter();
			this.pnlPreview = new DevExpress.XtraEditors.PanelControl();
			this.pnlChartActions = new DevExpress.XtraEditors.PanelControl();
			this.tcOptionsProperties = new DevExpress.XtraTab.XtraTabControl();
			this.tbpOptions = new DevExpress.XtraTab.XtraTabPage();
			this.tbpProperties = new DevExpress.XtraTab.XtraTabPage();
			this.tbpData = new DevExpress.XtraTab.XtraTabPage();
			this.chartDataControl = new DevExpress.XtraCharts.Designer.ChartDataControl();
			this.ucPointGridControl = new DevExpress.XtraCharts.Designer.SeriesPointsGridControl();
			this.splitterControl2 = new DevExpress.XtraCharts.Designer.Native.DesignerSplitter();
			this.pnlOptions = new DevExpress.XtraEditors.PanelControl();
			this.designerController = new DevExpress.XtraCharts.Designer.ChartDesignerController(this.components);
			((System.ComponentModel.ISupportInitialize)(this.cbShowOnNextStart.Properties)).BeginInit();
			this.hitTestTransparentPanelControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlStructure)).BeginInit();
			this.pnlStructure.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlActions)).BeginInit();
			this.pnlActions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlPreview)).BeginInit();
			this.pnlPreview.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlChartActions)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tcOptionsProperties)).BeginInit();
			this.tcOptionsProperties.SuspendLayout();
			this.tbpOptions.SuspendLayout();
			this.tbpProperties.SuspendLayout();
			this.tbpData.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlOptions)).BeginInit();
			this.pnlOptions.SuspendLayout();
			this.SuspendLayout();
			this.ucDockUserControl.SetBoundPropertyName(this.cbShowOnNextStart, "");
			resources.ApplyResources(this.cbShowOnNextStart, "cbShowOnNextStart");
			this.cbShowOnNextStart.Name = "cbShowOnNextStart";
			this.cbShowOnNextStart.Properties.AutoWidth = true;
			this.cbShowOnNextStart.Properties.Caption = resources.GetString("cbShowOnNextStart.Properties.Caption");
			this.cbShowOnNextStart.CheckedChanged += new System.EventHandler(this.ShowOnNextStart_CheckedChanged);
			resources.ApplyResources(this.sbOK, "sbOK");
			this.ucDockUserControl.SetBoundPropertyName(this.sbOK, "");
			this.sbOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.sbOK.Name = "sbOK";
			resources.ApplyResources(this.sbCancel, "sbCancel");
			this.ucDockUserControl.SetBoundPropertyName(this.sbCancel, "");
			this.sbCancel.DialogResult = System.Windows.Forms.DialogResult.Abort;
			this.sbCancel.Name = "sbCancel";
			this.hitTestTransparentPanelControl.BackColor = System.Drawing.Color.Transparent;
			this.ucDockUserControl.SetBoundPropertyName(this.hitTestTransparentPanelControl, "");
			this.hitTestTransparentPanelControl.Controls.Add(this.cbShowOnNextStart);
			this.hitTestTransparentPanelControl.Controls.Add(this.sbCancel);
			this.hitTestTransparentPanelControl.Controls.Add(this.sbOK);
			resources.ApplyResources(this.hitTestTransparentPanelControl, "hitTestTransparentPanelControl");
			this.hitTestTransparentPanelControl.Name = "hitTestTransparentPanelControl";
			resources.ApplyResources(this.ucDockUserControl, "ucDockUserControl");
			this.ucDockUserControl.Name = "ucDockUserControl";
			this.ucDockUserControl.SetBoundPropertyName(this.ucChartPreview, "");
			resources.ApplyResources(this.ucChartPreview, "ucChartPreview");
			this.ucChartPreview.Name = "ucChartPreview";
			this.ucDockUserControl.SetBoundPropertyName(this.ucOptionsControl, "");
			resources.ApplyResources(this.ucOptionsControl, "ucOptionsControl");
			this.ucOptionsControl.Name = "ucOptionsControl";
			this.ucOptionsControl.SelectedModel = null;
			this.ucDockUserControl.SetBoundPropertyName(this.ucPropertyGrid, "");
			resources.ApplyResources(this.ucPropertyGrid, "ucPropertyGrid");
			this.ucPropertyGrid.Name = "ucPropertyGrid";
			this.ucPropertyGrid.SelectedObject = null;
			this.ucPropertyGrid.ServiceProvider = null;
			this.ucDockUserControl.SetBoundPropertyName(this.ucChartTree, "");
			resources.ApplyResources(this.ucChartTree, "ucChartTree");
			this.ucChartTree.Name = "ucChartTree";
			this.pnlStructure.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.ucDockUserControl.SetBoundPropertyName(this.pnlStructure, "");
			this.pnlStructure.Controls.Add(this.ucChartTree);
			this.pnlStructure.Controls.Add(this.pnlActions);
			resources.ApplyResources(this.pnlStructure, "pnlStructure");
			this.pnlStructure.Name = "pnlStructure";
			this.pnlActions.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("pnlActions.Appearance.BackColor")));
			this.pnlActions.Appearance.Options.UseBackColor = true;
			this.pnlActions.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.ucDockUserControl.SetBoundPropertyName(this.pnlActions, "");
			this.pnlActions.Controls.Add(this.ucActionsControl);
			resources.ApplyResources(this.pnlActions, "pnlActions");
			this.pnlActions.Name = "pnlActions";
			this.ucDockUserControl.SetBoundPropertyName(this.ucActionsControl, "");
			resources.ApplyResources(this.ucActionsControl, "ucActionsControl");
			this.ucActionsControl.Name = "ucActionsControl";
			this.ucDockUserControl.SetBoundPropertyName(this.splitterControl1, "");
			resources.ApplyResources(this.splitterControl1, "splitterControl1");
			this.splitterControl1.Name = "splitterControl1";
			this.splitterControl1.TabStop = false;
			this.pnlPreview.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.ucDockUserControl.SetBoundPropertyName(this.pnlPreview, "");
			this.pnlPreview.Controls.Add(this.ucChartPreview);
			this.pnlPreview.Controls.Add(this.pnlChartActions);
			resources.ApplyResources(this.pnlPreview, "pnlPreview");
			this.pnlPreview.Name = "pnlPreview";
			this.pnlChartActions.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("pnlChartActions.Appearance.BackColor")));
			this.pnlChartActions.Appearance.Options.UseBackColor = true;
			this.pnlChartActions.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.ucDockUserControl.SetBoundPropertyName(this.pnlChartActions, "");
			resources.ApplyResources(this.pnlChartActions, "pnlChartActions");
			this.pnlChartActions.Name = "pnlChartActions";
			this.ucDockUserControl.SetBoundPropertyName(this.tcOptionsProperties, "");
			resources.ApplyResources(this.tcOptionsProperties, "tcOptionsProperties");
			this.tcOptionsProperties.Name = "tcOptionsProperties";
			this.tcOptionsProperties.SelectedTabPage = this.tbpOptions;
			this.tcOptionsProperties.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.tbpOptions,
			this.tbpProperties,
			this.tbpData});
			this.ucDockUserControl.SetBoundPropertyName(this.tbpOptions, "");
			this.tbpOptions.Controls.Add(this.ucOptionsControl);
			resources.ApplyResources(this.tbpOptions, "tbpOptions");
			this.tbpOptions.Name = "tbpOptions";
			this.ucDockUserControl.SetBoundPropertyName(this.tbpProperties, "");
			this.tbpProperties.Controls.Add(this.ucPropertyGrid);
			resources.ApplyResources(this.tbpProperties, "tbpProperties");
			this.tbpProperties.Name = "tbpProperties";
			this.ucDockUserControl.SetBoundPropertyName(this.tbpData, "");
			this.tbpData.Controls.Add(this.chartDataControl);
			this.tbpData.Controls.Add(this.ucPointGridControl);
			this.tbpData.Name = "tbpData";
			resources.ApplyResources(this.tbpData, "tbpData");
			this.ucDockUserControl.SetBoundPropertyName(this.chartDataControl, "");
			resources.ApplyResources(this.chartDataControl, "chartDataControl");
			this.chartDataControl.Name = "chartDataControl";
			this.ucDockUserControl.SetBoundPropertyName(this.ucPointGridControl, "");
			resources.ApplyResources(this.ucPointGridControl, "ucPointGridControl");
			this.ucPointGridControl.Name = "ucPointGridControl";
			this.ucDockUserControl.SetBoundPropertyName(this.splitterControl2, "");
			resources.ApplyResources(this.splitterControl2, "splitterControl2");
			this.splitterControl2.Name = "splitterControl2";
			this.splitterControl2.TabStop = false;
			this.pnlOptions.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.ucDockUserControl.SetBoundPropertyName(this.pnlOptions, "");
			this.pnlOptions.Controls.Add(this.tcOptionsProperties);
			resources.ApplyResources(this.pnlOptions, "pnlOptions");
			this.pnlOptions.Name = "pnlOptions";
			this.designerController.ActionsControl = this.ucActionsControl;
			this.designerController.ChartContainer = null;
			this.designerController.ChartOptionsControl = this.ucOptionsControl;
			this.designerController.ChartStructureControl = this.ucChartTree;
			this.designerController.DataControl = this.chartDataControl;
			this.designerController.PointsGridControl = this.ucPointGridControl;
			this.designerController.PreviewControl = this.ucChartPreview;
			this.designerController.PropertyGridControl = this.ucPropertyGrid;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ucDockUserControl.SetBoundPropertyName(this, "");
			this.Controls.Add(this.pnlPreview);
			this.Controls.Add(this.splitterControl2);
			this.Controls.Add(this.pnlOptions);
			this.Controls.Add(this.splitterControl1);
			this.Controls.Add(this.pnlStructure);
			this.Controls.Add(this.ucDockUserControl);
			this.Controls.Add(this.hitTestTransparentPanelControl);
			this.MinimizeBox = false;
			this.Name = "DesignerFormBase";
			this.ShowInTaskbar = false;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DesignerFormBase_FormClosing);
			this.ResizeEnd += new System.EventHandler(this.DesignerFormBase_ResizeEnd);
			((System.ComponentModel.ISupportInitialize)(this.cbShowOnNextStart.Properties)).EndInit();
			this.hitTestTransparentPanelControl.ResumeLayout(false);
			this.hitTestTransparentPanelControl.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlStructure)).EndInit();
			this.pnlStructure.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlActions)).EndInit();
			this.pnlActions.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlPreview)).EndInit();
			this.pnlPreview.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlChartActions)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tcOptionsProperties)).EndInit();
			this.tcOptionsProperties.ResumeLayout(false);
			this.tbpOptions.ResumeLayout(false);
			this.tbpProperties.ResumeLayout(false);
			this.tbpData.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlOptions)).EndInit();
			this.pnlOptions.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private SimpleButton sbOK;
		private SimpleButton sbCancel;
		private CheckEdit cbShowOnNextStart;
		private HitTestTransparentPanelControl hitTestTransparentPanelControl;
		private XtraGrid.Views.Grid.EditFormUserControl ucDockUserControl;
		private ChartPreviewControl ucChartPreview;
		private ChartDesignerController designerController;
		private ChartPropertyGridControl ucPropertyGrid;
		private ChartStructureControl ucChartTree;
		private ChartElementsOptionsControl ucOptionsControl;
		private PanelControl pnlStructure;
		private PanelControl pnlActions;
		private DesignerSplitter splitterControl1;
		private PanelControl pnlPreview;
		private PanelControl pnlChartActions;
		private XtraTab.XtraTabControl tcOptionsProperties;
		private XtraTab.XtraTabPage tbpOptions;
		private XtraTab.XtraTabPage tbpProperties;
		private DesignerSplitter splitterControl2;
		private PanelControl pnlOptions;
		private ChartActionsControl ucActionsControl;
		private XtraTab.XtraTabPage tbpData;
		private ChartDataControl chartDataControl;
		private SeriesPointsGridControl ucPointGridControl;
	}
}
