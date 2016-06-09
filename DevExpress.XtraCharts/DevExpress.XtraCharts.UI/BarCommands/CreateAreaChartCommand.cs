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
	public class CreateAreaChartCommand : CreateChartCommandBase {
		public CreateAreaChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateAreaChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateAreaChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateAreaChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateAreaChart; } }
		protected override ViewType ChartViewType { get { return ViewType.Area; } }
	}
	public class CreateArea3DChartCommand : CreateChartCommandBase {
		public CreateArea3DChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateArea3DChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateArea3DChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateArea3DChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateArea3DChart; } }
		protected override ViewType ChartViewType { get { return ViewType.Area3D; } }
	}
	public class CreateFullStackedAreaChartCommand : CreateChartCommandBase {
		public CreateFullStackedAreaChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateFullStackedAreaChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateFullStackedAreaChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateFullStackedAreaChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateFullStackedAreaChart; } }
		protected override ViewType ChartViewType { get { return ViewType.FullStackedArea; } }
	}
	public class CreateFullStackedArea3DChartCommand : CreateChartCommandBase {
		public CreateFullStackedArea3DChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateFullStackedArea3DChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateFullStackedArea3DChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateFullStackedArea3DChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateFullStackedArea3DChart; } }
		protected override ViewType ChartViewType { get { return ViewType.FullStackedArea3D; } }
	}
	public class CreateFullStackedSplineAreaChartCommand : CreateChartCommandBase {
		public CreateFullStackedSplineAreaChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateFullStackedSplineAreaChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateFullStackedSplineAreaChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateFullStackedSplineAreaChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateFullStackedSplineAreaChart; } }
		protected override ViewType ChartViewType { get { return ViewType.FullStackedSplineArea; } }
	}
	public class CreateFullStackedSplineArea3DChartCommand : CreateChartCommandBase {
		public CreateFullStackedSplineArea3DChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateFullStackedSplineArea3DChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateFullStackedSplineArea3DChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateFullStackedSplineArea3DChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateFullStackedSplineArea3DChart; } }
		protected override ViewType ChartViewType { get { return ViewType.FullStackedSplineArea3D; } }
	}
	public class CreateSplineAreaChartCommand : CreateChartCommandBase {
		public CreateSplineAreaChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateSplineAreaChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateSplineAreaChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateSplineAreaChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateSplineAreaChart; } }
		protected override ViewType ChartViewType { get { return ViewType.SplineArea; } }
	}
	public class CreateSplineArea3DChartCommand : CreateChartCommandBase {
		public CreateSplineArea3DChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateSplineArea3DChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateSplineArea3DChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateSplineArea3DChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateSplineArea3DChart; } }
		protected override ViewType ChartViewType { get { return ViewType.SplineArea3D; } }
	}
	public class CreateStackedAreaChartCommand : CreateChartCommandBase {
		public CreateStackedAreaChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateStackedAreaChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateStackedAreaChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateStackedAreaChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateStackedAreaChart; } }
		protected override ViewType ChartViewType { get { return ViewType.StackedArea; } }
	}
	public class CreateStackedArea3DChartCommand : CreateChartCommandBase {
		public CreateStackedArea3DChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateStackedArea3DChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateStackedArea3DChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateStackedArea3DChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateStackedArea3DChart; } }
		protected override ViewType ChartViewType { get { return ViewType.StackedArea3D; } }
	}
	public class CreateStackedSplineAreaChartCommand : CreateChartCommandBase {
		public CreateStackedSplineAreaChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateStackedSplineAreaChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateStackedSplineAreaChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateStackedSplineAreaChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateStackedSplineAreaChart; } }
		protected override ViewType ChartViewType { get { return ViewType.StackedSplineArea; } }
	}
	public class CreateStackedSplineArea3DChartCommand : CreateChartCommandBase {
		public CreateStackedSplineArea3DChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateStackedSplineArea3DChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateStackedSplineArea3DChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateStackedSplineArea3DChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateStackedSplineArea3DChart; } }
		protected override ViewType ChartViewType { get { return ViewType.StackedSplineArea3D; } }
	}
	public class CreateStepAreaChartCommand : CreateChartCommandBase {
		public CreateStepAreaChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateStepAreaChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateStepAreaChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateStepAreaChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateStepAreaChart; } }
		protected override ViewType ChartViewType { get { return ViewType.StepArea; } }
	}
	public class CreateStepArea3DChartCommand : CreateChartCommandBase {
		public CreateStepArea3DChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateStepArea3DChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateStepArea3DChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateStepArea3DChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateStepArea3DChart; } }
		protected override ViewType ChartViewType { get { return ViewType.StepArea3D; } }
	}
	public class CreateRangeAreaChartCommand : CreateChartCommandBase {
		public CreateRangeAreaChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateRangeAreaChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateRangeAreaChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateRangeAreaChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateRangeAreaChart; } }
		protected override ViewType ChartViewType { get { return ViewType.RangeArea; } }
	}
	public class CreateRangeArea3DChartCommand : CreateChartCommandBase {
		public CreateRangeArea3DChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateRangeArea3DChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateRangeArea3DChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateRangeArea3DChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateRangeArea3DChart; } }
		protected override ViewType ChartViewType { get { return ViewType.RangeArea3D; } }
	}
}
