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

namespace DevExpress.ExpressApp.PivotChart.Win {
	partial class AnalysisPrintPreviewController
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
			this.printPreviewPivotGridAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);	
			this.printPreviewChartAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);			
			this.printPreviewPivotGridAction.Caption = "Print PivotGrid";
			this.printPreviewPivotGridAction.Id = "PrintPreviewPivotGrid";
			this.printPreviewPivotGridAction.Category = "View";
			this.printPreviewPivotGridAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(printPreviewPivotGridAction_Execute);
			this.printPreviewPivotGridAction.ImageName = "MenuBar_PivotGridPrintPreview";
			this.printPreviewChartAction.Caption = "Print Chart";
			this.printPreviewChartAction.Id = "PrintChart";
			this.printPreviewChartAction.Category = "View";
			this.printPreviewChartAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(printPreviewChartAction_Execute);
			this.printPreviewChartAction.ImageName = "MenuBar_ChartPrintPreview";
		}
		#endregion
		private DevExpress.ExpressApp.Actions.SimpleAction printPreviewPivotGridAction;
		private DevExpress.ExpressApp.Actions.SimpleAction printPreviewChartAction;
	}
}
