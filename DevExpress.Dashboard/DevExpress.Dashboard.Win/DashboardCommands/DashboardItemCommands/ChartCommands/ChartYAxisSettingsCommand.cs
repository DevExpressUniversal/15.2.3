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
namespace DevExpress.DashboardWin.Commands {
	public class ChartYAxisSettingsCommand : ChartAxisSettingsCommand<ChartYAxisSettingsHistoryItem, AxisYSettingsForm, ChartDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.ChartYAxisSettings; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandChartYAxisSettingsCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandChartYAxisSettingsDescription; } }
		public override string ImageName { get { return "ChartYAxisSettings"; } }
		public ChartYAxisSettingsCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override ChartYAxisSettingsHistoryItem CreateHistoryItem(ChartDashboardItem item) {
			return new ChartYAxisSettingsHistoryItem(item);
		}
		protected override AxisYSettingsForm CreateAxisSettingsForm(ChartYAxisSettingsHistoryItem historyItem, UserLookAndFeel lookAndFeel) {
			return new AxisYSettingsForm(historyItem, lookAndFeel);
		}
	}
	public abstract class ScatterChartAxisSettingsCommand : ChartAxisSettingsCommand<ScatterChartAxisSettingsHistoryItem, AxisYSettingsForm, ScatterChartDashboardItem> {
		protected ScatterChartAxisSettingsCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override AxisYSettingsForm CreateAxisSettingsForm(ScatterChartAxisSettingsHistoryItem historyItem, UserLookAndFeel lookAndFeel) {
			return new AxisYSettingsForm(historyItem, lookAndFeel);
		}
	}
	public class ScatterChartXAxisSettingsCommand : ScatterChartAxisSettingsCommand {
		public override DashboardCommandId Id { get { return DashboardCommandId.ScatterChartXAxisSettings; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandScatterChartXAxisSettingsCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandScatterChartXAxisSettingsDescription; } }
		public override string ImageName { get { return "ChartXAxisSettings"; } }
		public ScatterChartXAxisSettingsCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override ScatterChartAxisSettingsHistoryItem CreateHistoryItem(ScatterChartDashboardItem item) {
			return new ScatterChartXAxisSettingsHistoryItem(item);
		}
	}
	public class ScatterChartYAxisSettingsCommand : ScatterChartAxisSettingsCommand {
		public override DashboardCommandId Id { get { return DashboardCommandId.ScatterChartYAxisSettings; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandScatterChartYAxisSettingsCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandScatterChartYAxisSettingsDescription; } }
		public override string ImageName { get { return "ChartYAxisSettings"; } }
		public ScatterChartYAxisSettingsCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override ScatterChartAxisSettingsHistoryItem CreateHistoryItem(ScatterChartDashboardItem item) {
			return new ScatterChartYAxisSettingsHistoryItem(item);
		}
	}
}
