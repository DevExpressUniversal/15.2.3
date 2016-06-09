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

using System.Collections.Generic;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.Native;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Commands;
using System;
using DevExpress.DashboardWin.ServiceModel;
namespace DevExpress.DashboardWin.Commands {
	public class ScatterChartLabelOptionsCommand : DashboardItemCommand<ScatterChartDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.ScatterChartLabelOptions; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandScatterChartPointLabelOptionsCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandScatterChartPointLabelOptionsDescription; } }
		public override string ImageName { get { return "ScatterChartLabelOptions"; } }
		public ScatterChartLabelOptionsCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override void ExecuteInternal(ICommandUIState state) {
			using(ScatterChartPointLabelOptionsForm form = CreateForm())
				form.ShowDialog(Control.FindForm());
		}
		ScatterChartPointLabelOptionsForm CreateForm() {
			IDashboardDesignerHistoryService historyService = Control.RequestServiceStrictly<IDashboardDesignerHistoryService>();
			ScatterChartPointLabelHistoryItem historyItem = new ScatterChartPointLabelHistoryItem(DashboardItem);
			ScatterChartLabelOptionsPresenter presenter = new ScatterChartLabelOptionsPresenter(historyService, historyItem);
			ScatterChartPointLabelOptionsForm form = new ScatterChartPointLabelOptionsForm();
			presenter.Initialize(form);
			return form;
		}
	}
}
