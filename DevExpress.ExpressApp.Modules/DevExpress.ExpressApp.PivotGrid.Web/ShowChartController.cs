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
using System.Text;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.PivotGrid.Web {
	public class ShowChartController : ViewController<ListView> {
		private SimpleAction showChartAction;
		private void UpdateShowChartActionCaption() {
			string showChartMenuCaptionId = PivotSettings.ShowChart ? "HideChart" : "ShowChart";
			showChartAction.Caption = CaptionHelper.GetLocalizedText(PivotGridModule.LocalizationGroup, showChartMenuCaptionId);
			showChartAction.ToolTip = showChartAction.Caption;
			showChartAction.ImageName = PivotSettings.ShowChart ? "Action_Hide_Chart" : "Action_View_Chart";
		}
		private void showChartAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			PivotSettings.ShowChart = !PivotSettings.ShowChart;
			((ASPxPivotGridListEditor)(View.Editor)).UpdateChartVisibility(PivotSettings.ShowChart);
			UpdateShowChartActionCaption();
		}
		private IModelPivotSettings PivotSettings {
			get { return ((IModelPivotListView)(View.Model)).PivotSettings; }
		}
		protected override void OnViewChanging(View view) {
			base.OnViewChanging(view);
			if(view is ListView) {
				Active["ByEditorType"] = ((IModelListView)(view.Model)).EditorType == typeof(ASPxPivotGridListEditor);
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			UpdateShowChartActionCaption();
		}
		public ShowChartController() {
			showChartAction = new SimpleAction(this, "ShowChartAction", PredefinedCategory.Edit);
			showChartAction.ImageName = "Action_PivotChart_Data_Bind";
			showChartAction.Execute += new SimpleActionExecuteEventHandler(showChartAction_Execute);
		}
	}
}
