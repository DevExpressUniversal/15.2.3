#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.XtraPrinting;
namespace DevExpress.ExpressApp.PivotChart.Win {
	public partial class AnalysisPrintPreviewController : WinAnalysisViewControllerBase {
		protected override void UpdateActionState() {
			base.UpdateActionState();
			printPreviewChartAction.Enabled.SetItemValue("AnalysisEditor presents", analysisEditor != null);
			printPreviewPivotGridAction.Enabled.SetItemValue("AnalysisEditor presents", analysisEditor != null);
			printPreviewPivotGridAction.Enabled.SetItemValue("Data source is ready", IsDataSourceReady);
			printPreviewChartAction.Enabled.SetItemValue("Data source is ready", IsDataSourceReady);
			printPreviewChartAction.Enabled.SetItemValue("Chart is visible", analysisEditor != null && analysisEditor.Control != null && ((AnalysisEditorWin)analysisEditor).Control.IsChartVisible);
		}
		private void printPreviewPivotGridAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			PreviewPivotGrid();
		}
		private void printPreviewChartAction_Execute(object sender, DevExpress.ExpressApp.Actions.SimpleActionExecuteEventArgs e) {
			PreviewChart();
		}
		protected virtual void PreviewPivotGrid() {
			((AnalysisEditorWin)analysisEditor).Control.PivotGrid.ShowPrintPreview();
		}
		protected virtual void PreviewChart() {
			((AnalysisEditorWin)analysisEditor).Control.Chart.ShowPrintPreview(DevExpress.XtraCharts.Printing.PrintSizeMode.Stretch);
		}
		protected override void OnActivated() {
			base.OnActivated();
			printPreviewChartAction.Enabled.SetItemValue("The XtraPrinting Library is available", ComponentPrinter.IsPrintingAvailable(false));
			printPreviewPivotGridAction.Enabled.SetItemValue("The XtraPrinting Library is available", ComponentPrinter.IsPrintingAvailable(false));
		}
		public AnalysisPrintPreviewController() {
			InitializeComponent();
			RegisterActions(components);
		}
		public SimpleAction PrintPreviewPivotGridAction {
			get { return this.printPreviewPivotGridAction; }
		}
		public SimpleAction PrintPreviewChartAction {
			get { return this.printPreviewChartAction; }
		}
	}
}
