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
	public class CreateRadarPointChartCommand : CreateChartCommandBase {
		public CreateRadarPointChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateRadarPointChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateRadarPointChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateRadarPointChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateRadarPointChart; } }
		protected override ViewType ChartViewType { get { return ViewType.RadarPoint; } }
	}
	public class CreateRadarLineChartCommand : CreateChartCommandBase {
		public CreateRadarLineChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateRadarLineChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateRadarLineChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateRadarLineChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateRadarLineChart; } }
		protected override ViewType ChartViewType { get { return ViewType.RadarLine; } }
	}
	public class CreateScatterRadarLineChartCommand : CreateChartCommandBase {
		public CreateScatterRadarLineChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateScatterRadarLineChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateScatterRadarLineChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateScatterRadarLineChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateScatterRadarLineChart; } }
		protected override ViewType ChartViewType { get { return ViewType.ScatterRadarLine; } }
	}
	public class CreateRadarAreaChartCommand : CreateChartCommandBase {
		public CreateRadarAreaChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateRadarAreaChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateRadarAreaChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateRadarAreaChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateRadarAreaChart; } }
		protected override ViewType ChartViewType { get { return ViewType.RadarArea; } }
	}
	public class CreatePolarPointChartCommand : CreatePolarChartCommandBase {
		public CreatePolarPointChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreatePolarPointChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreatePolarPointChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreatePolarPointChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreatePolarPointChart; } }
		protected override ViewType ChartViewType { get { return ViewType.PolarPoint; } }
	}
	public class CreatePolarLineChartCommand : CreatePolarChartCommandBase {
		public CreatePolarLineChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreatePolarLineChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreatePolarLineChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreatePolarLineChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreatePolarLineChart; } }
		protected override ViewType ChartViewType { get { return ViewType.PolarLine; } }
	}
	public class CreateScatterPolarLineChartCommand : CreatePolarChartCommandBase {
		public CreateScatterPolarLineChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateScatterPolarLineChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateScatterPolarLineChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateScatterPolarLineChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateScatterPolarLineChart; } }
		protected override ViewType ChartViewType { get { return ViewType.ScatterPolarLine; } }
	}
	public class CreatePolarAreaChartCommand : CreatePolarChartCommandBase {
		public CreatePolarAreaChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreatePolarAreaChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreatePolarAreaChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreatePolarAreaChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreatePolarAreaChart; } }
		protected override ViewType ChartViewType { get { return ViewType.PolarArea; } }
	}
}
