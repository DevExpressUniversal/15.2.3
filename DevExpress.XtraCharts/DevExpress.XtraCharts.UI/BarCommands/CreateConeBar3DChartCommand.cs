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
	public class CreateConeBar3DChartCommand : CreateConeBar3DChartCommandBase {
		public CreateConeBar3DChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateConeBar3DChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateConeBar3DChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateConeBar3DChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateConeBar3DChart; } }
		protected override ViewType ChartViewType { get { return ViewType.Bar3D; } }
	}
	public class CreateConeFullStackedBar3DChartCommand : CreateConeBar3DChartCommandBase {
		public CreateConeFullStackedBar3DChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateConeFullStackedBar3DChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateConeFullStackedBar3DChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateConeFullStackedBar3DChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateConeFullStackedBar3DChart; } }
		protected override ViewType ChartViewType { get { return ViewType.FullStackedBar3D; } }
	}
	public class CreateConeSideBySideFullStackedBar3DChartCommand : CreateConeBar3DChartCommandBase {
		public CreateConeSideBySideFullStackedBar3DChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateConeSideBySideFullStackedBar3DChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateConeSideBySideFullStackedBar3DChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateConeSideBySideFullStackedBar3DChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateConeSideBySideFullStackedBar3DChart; } }
		protected override ViewType ChartViewType { get { return ViewType.SideBySideFullStackedBar3D; } }
	}
	public class CreateConeSideBySideStackedBar3DChartCommand : CreateConeBar3DChartCommandBase {
		public CreateConeSideBySideStackedBar3DChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateConeSideBySideStackedBar3DChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateConeSideBySideStackedBar3DChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateConeSideBySideStackedBar3DChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateConeSideBySideStackedBar3DChart; } }
		protected override ViewType ChartViewType { get { return ViewType.SideBySideStackedBar3D; } }
	}
	public class CreateConeStackedBar3DChartCommand : CreateConeBar3DChartCommandBase {
		public CreateConeStackedBar3DChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateConeStackedBar3DChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateConeStackedBar3DChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateConeStackedBar3DChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateConeStackedBar3DChart; } }
		protected override ViewType ChartViewType { get { return ViewType.StackedBar3D; } }
	}
	public class CreateConeManhattanBarChartCommand : CreateConeBar3DChartCommandBase {
		public CreateConeManhattanBarChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateConeManhattanBarChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateConeManhattanBarChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateConeManhattanBarChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateConeManhattanBarChart; } }
		protected override ViewType ChartViewType { get { return ViewType.ManhattanBar; } }
	}
}
