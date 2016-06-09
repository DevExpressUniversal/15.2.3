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
using System.Collections.Generic;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Kpi {
	public class ShowKpiAsChartController : ViewController<ListView> {
		private SimpleAction showChart;
		private void showChart_Execute(object sender, SimpleActionExecuteEventArgs e) {
			IKpiInstance kpi = e.CurrentObject as IKpiInstance;
			if(kpi != null) {
				e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
				e.ShowViewParameters.Context = TemplateContext.PopupWindow;
				DialogController dialogController = Application.CreateController<DialogController>();
				dialogController.CancelAction.Active.SetItemValue("Showing KPI", false);
				dialogController.AcceptAction.Caption = CaptionHelper.GetLocalizedText("DialogButtons", "Close", "Close");
				dialogController.AcceptAction.ToolTip = dialogController.AcceptAction.Caption;
				e.ShowViewParameters.Controllers.Add(dialogController);
				IObjectSpace os = Application.CreateObjectSpace(kpi.GetType());
				ViewCreatedEventArgs args = new ViewCreatedEventArgs(Application.CreateDetailView(os, KpiModule.KpiInstance_Chart_DetailViewID, true, os.GetObject(kpi)));
				OnChartViewCreated(args);
				e.ShowViewParameters.CreatedView = args.View;
			}
		}
		private void OnChartViewCreated(ViewCreatedEventArgs args) {
			if(ChartViewCreated != null) {
				ChartViewCreated(this, args);
			}
		}
		public ShowKpiAsChartController() {
			TargetObjectType = typeof(IKpiInstance);
			showChart = new SimpleAction(this, "ShowChart", PredefinedCategory.Edit);
			showChart.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
			showChart.Execute += new SimpleActionExecuteEventHandler(showChart_Execute);
			showChart.ImageName = "Action_Show_Chart";
		}
		public event EventHandler<ViewCreatedEventArgs> ChartViewCreated;
	}
}
