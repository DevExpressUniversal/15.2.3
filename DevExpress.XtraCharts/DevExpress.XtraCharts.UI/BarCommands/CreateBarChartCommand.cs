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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Commands {
	public class CreateBarChartCommand : CreateChartCommandBase {
		public CreateBarChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateBarChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateBarChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateBarChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateBarChart; } }
		protected override ViewType ChartViewType { get { return ViewType.Bar; } }
	}
	public class CreateBar3DChartCommand : CreateBar3DChartCommandBase {
		public CreateBar3DChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateBar3DChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateBar3DChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateBar3DChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateBar3DChart; } }
		protected override ViewType ChartViewType { get { return ViewType.Bar3D; } }
	}
	public class CreateFullStackedBarChartCommand : CreateChartCommandBase {
		public CreateFullStackedBarChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateFullStackedBarChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateFullStackedBarChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateFullStackedBarChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateFullStackedBarChart; } }
		protected override ViewType ChartViewType { get { return ViewType.FullStackedBar; } }
	}
	public class CreateFullStackedBar3DChartCommand : CreateBar3DChartCommandBase {
		public CreateFullStackedBar3DChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateFullStackedBar3DChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateFullStackedBar3DChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateFullStackedBar3DChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateFullStackedBar3DChart; } }
		protected override ViewType ChartViewType { get { return ViewType.FullStackedBar3D; } }
	}
	public class CreateSideBySideFullStackedBarChartCommand : CreateChartCommandBase {
		public CreateSideBySideFullStackedBarChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateSideBySideFullStackedBarChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateSideBySideFullStackedBarChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateSideBySideFullStackedBarChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateSideBySideFullStackedBarChart; } }
		protected override ViewType ChartViewType { get { return ViewType.SideBySideFullStackedBar; } }
	}
	public class CreateSideBySideFullStackedBar3DChartCommand : CreateBar3DChartCommandBase {
		public CreateSideBySideFullStackedBar3DChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateSideBySideFullStackedBar3DChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateSideBySideFullStackedBar3DChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateSideBySideFullStackedBar3DChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateSideBySideFullStackedBar3DChart; } }
		protected override ViewType ChartViewType { get { return ViewType.SideBySideFullStackedBar3D; } }
	}
	public class CreateSideBySideStackedBarChartCommand : CreateChartCommandBase {
		public CreateSideBySideStackedBarChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateSideBySideStackedBarChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateSideBySideStackedBarChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateSideBySideStackedBarChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateSideBySideStackedBarChart; } }
		protected override ViewType ChartViewType { get { return ViewType.SideBySideStackedBar; } }
	}
	public class CreateSideBySideStackedBar3DChartCommand : CreateBar3DChartCommandBase {
		public CreateSideBySideStackedBar3DChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateSideBySideStackedBar3DChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateSideBySideStackedBar3DChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateSideBySideStackedBar3DChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateSideBySideStackedBar3DChart; } }
		protected override ViewType ChartViewType { get { return ViewType.SideBySideStackedBar3D; } }
	}
	public class CreateStackedBarChartCommand : CreateChartCommandBase {
		public CreateStackedBarChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateStackedBarChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateStackedBarChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateStackedBarChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateStackedBarChart; } }
		protected override ViewType ChartViewType { get { return ViewType.StackedBar; } }
	}
	public class CreateStackedBar3DChartCommand : CreateBar3DChartCommandBase {
		public CreateStackedBar3DChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateStackedBar3DChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateStackedBar3DChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateStackedBar3DChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateStackedBar3DChart; } }
		protected override ViewType ChartViewType { get { return ViewType.StackedBar3D; } }
	}
	public class CreateManhattanBarChartCommand : CreateBar3DChartCommandBase {
		public CreateManhattanBarChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateManhattanBarChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateManhattanBarChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateManhattanBarChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateManhattanBarChart; } }
		protected override ViewType ChartViewType { get { return ViewType.ManhattanBar; } }
	}
	public class CreateRangeBarChartCommand : CreateChartCommandBase {
		public CreateRangeBarChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateRangeBarChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateRangeBarChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateRangeBarChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateRangeBarChart; } }
		protected override ViewType ChartViewType { get { return ViewType.RangeBar; } }
	}
	public class CreateSideBySideRangeBarChartCommand : CreateChartCommandBase {
		public CreateSideBySideRangeBarChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateSideBySideRangeBarChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateSideBySideRangeBarChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateSideBySideRangeBarChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateSideBySideRangeBarChart; } }
		protected override ViewType ChartViewType { get { return ViewType.SideBySideRangeBar; } }
	}
}
