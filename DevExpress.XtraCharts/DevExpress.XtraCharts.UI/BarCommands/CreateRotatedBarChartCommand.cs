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
	public class CreateRotatedBarChartCommand : CreateRotatedBarChartCommandBase {
		public CreateRotatedBarChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateRotatedBarChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateRotatedBarChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateRotatedBarChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateRotatedBarChart; } }
		protected override ViewType ChartViewType { get { return ViewType.Bar; } }
	}
	public class CreateRotatedFullStackedBarChartCommand : CreateRotatedBarChartCommandBase {
		public CreateRotatedFullStackedBarChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateRotatedFullStackedBarChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateRotatedFullStackedBarChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateRotatedFullStackedBarChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateRotatedFullStackedBarChart; } }
		protected override ViewType ChartViewType { get { return ViewType.FullStackedBar; } }
	}
	public class CreateRotatedSideBySideFullStackedBarChartCommand : CreateRotatedBarChartCommandBase {
		public CreateRotatedSideBySideFullStackedBarChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateRotatedSideBySideFullStackedBarChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateRotatedSideBySideFullStackedBarChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateRotatedSideBySideFullStackedBarChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateRotatedSideBySideFullStackedBarChart; } }
		protected override ViewType ChartViewType { get { return ViewType.SideBySideFullStackedBar; } }
	}
	public class CreateRotatedSideBySideStackedBarChartCommand : CreateRotatedBarChartCommandBase {
		public CreateRotatedSideBySideStackedBarChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateRotatedSideBySideStackedBarChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateRotatedSideBySideStackedBarChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateRotatedSideBySideStackedBarChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateRotatedSideBySideStackedBarChart; } }
		protected override ViewType ChartViewType { get { return ViewType.SideBySideStackedBar; } }
	}
	public class CreateRotatedStackedBarChartCommand : CreateRotatedBarChartCommandBase {
		public CreateRotatedStackedBarChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateRotatedStackedBarChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateRotatedStackedBarChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateRotatedStackedBarChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateRotatedStackedBarChart; } }
		protected override ViewType ChartViewType { get { return ViewType.StackedBar; } }
	}
}
