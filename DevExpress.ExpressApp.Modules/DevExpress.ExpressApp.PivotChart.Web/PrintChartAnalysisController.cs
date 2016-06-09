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
namespace DevExpress.ExpressApp.PivotChart.Web {
	public partial class PrintChartAnalysisController : AnalysisViewControllerBase {
		protected const string NotSupportedKey = "NotSupported";
		protected override void UpdateActionState() {
			base.UpdateActionState();
			printChartAction.Enabled.SetItemValue("AnalysisEditor presents", analysisEditor != null);
			printChartAction.Enabled.SetItemValue("Data source is ready", IsDataSourceReady);
		}
		private void printChartAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			if(analysisEditor != null && analysisEditor.Control != null) {
			((AnalysisControlWeb)analysisEditor.Control).UpdateChartDataSource();
			analysisEditor.Control.Chart.Print(DevExpress.XtraCharts.Printing.PrintSizeMode.Stretch);
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			printChartAction.Enabled.SetItemValue("The XtraPrinting Library is available", ComponentPrinterDynamic.IsPrintingAvailable(false));
		}
		public PrintChartAnalysisController() {
			InitializeComponent();
			RegisterActions(components);
			Active[NotSupportedKey] = false;
		}
		public SimpleAction PrintChartAction {
			get { return printChartAction; }
		}
	}
}
