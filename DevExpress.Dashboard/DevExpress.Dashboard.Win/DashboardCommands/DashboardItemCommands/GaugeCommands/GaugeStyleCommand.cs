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

using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Native;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Commands {
	public abstract class GaugeStyleCommand : DashboardItemInteractionCommand<GaugeDashboardItem> {
		protected abstract GaugeViewType ViewType { get; }
		protected GaugeStyleCommand(DashboardDesigner designer)
			: base(designer) {
		}
		protected override bool CheckDashboardItem(GaugeDashboardItem item) {
			return item.ViewType == ViewType;
		}
		protected override IHistoryItem CreateHistoryItem(GaugeDashboardItem dashboardItem, bool enabled) {
			return new GaugeStyleHistoryItem(dashboardItem, ViewType);
		}
	}
	public class GaugeStyleFullCircularCommand : GaugeStyleCommand {
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandGaugeStyleFullCircularCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandGaugeStyleFullCircularDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.GaugeStyleFullCircular; } }
		protected override GaugeViewType ViewType { get { return GaugeViewType.CircularFull; } }
		public override string ImageName { get { return "GaugeStyleFullCircular"; } }
		public GaugeStyleFullCircularCommand(DashboardDesigner designer)
			: base(designer) {
		}
	}
	public class GaugeStyleHalfCircularCommand : GaugeStyleCommand {
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandGaugeStyleHalfCircularCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandGaugeStyleHalfCircularDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.GaugeStyleHalfCircular; } }
		protected override GaugeViewType ViewType { get { return GaugeViewType.CircularHalf; } }
		public override string ImageName { get { return "GaugeStyleHalfCircular"; } }
		public GaugeStyleHalfCircularCommand(DashboardDesigner designer)
			: base(designer) {
		}
	}
	public class GaugeStyleLeftQuarterCircularCommand : GaugeStyleCommand {
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandGaugeStyleLeftQuarterCircularCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandGaugeStyleLeftQuarterCircularDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.GaugeStyleLeftQuarterCircular; } }
		protected override GaugeViewType ViewType { get { return GaugeViewType.CircularQuarterLeft; } }
		public override string ImageName { get { return "GaugeStyleLeftQuarterCircular"; } }
		public GaugeStyleLeftQuarterCircularCommand(DashboardDesigner designer)
			: base(designer) {
		}
	}
	public class GaugeStyleRightQuarterCircularCommand : GaugeStyleCommand {
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandGaugeStyleRightQuarterCircularCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandGaugeStyleRightQuarterCircularDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.GaugeStyleRightQuarterCircular; } }
		protected override GaugeViewType ViewType { get { return GaugeViewType.CircularQuarterRight; } }
		public override string ImageName { get { return "GaugeStyleRightQuarterCircular"; } }
		public GaugeStyleRightQuarterCircularCommand(DashboardDesigner designer)
			: base(designer) {
		}
	}
	public class GaugeStyleThreeFourthCircularCommand : GaugeStyleCommand {
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandGaugeStyleThreeFourthCircularCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandGaugeStyleThreeFourthCircularDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.GaugeStyleThreeFourthCircular; } }
		protected override GaugeViewType ViewType { get { return GaugeViewType.CircularThreeFourth; } }
		public override string ImageName { get { return "GaugeStyleThreeFourthCircular"; } }
		public GaugeStyleThreeFourthCircularCommand(DashboardDesigner designer)
			: base(designer) {
		}
	}
	public class GaugeStyleLinearHorizontalCommand : GaugeStyleCommand {
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandGaugeStyleLinearHorizontalCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandGaugeStyleLinearHorizontalDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.GaugeStyleLinearHorizontal; } }
		protected override GaugeViewType ViewType { get { return GaugeViewType.LinearHorizontal; } }
		public override string ImageName { get { return "GaugeStyleLinearHorizontal"; } }
		public GaugeStyleLinearHorizontalCommand(DashboardDesigner designer)
			: base(designer) {
		}
	}
	public class GaugeStyleLinearVerticalCommand : GaugeStyleCommand {
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandGaugeStyleLinearVerticalCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandGaugeStyleLinearVerticalDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.GaugeStyleLinearVertical; } }
		protected override GaugeViewType ViewType { get { return GaugeViewType.LinearVertical; } }
		public override string ImageName { get { return "GaugeStyleLinearVertical"; } }
		public GaugeStyleLinearVerticalCommand(DashboardDesigner designer)
			: base(designer) {
		}
	}
}
