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
	public class CreateFunnelChartCommand : CreateSimpleDiagramChartCommandBase {
		public CreateFunnelChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateFunnelChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateFunnelChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateFunnelChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateFunnelChart; } }
		protected override ViewType ChartViewType { get { return ViewType.Funnel; } }
	}
	public class CreateFunnel3DChartCommand : CreateSimpleDiagramChartCommandBase {
		public CreateFunnel3DChartCommand(IChartContainer control)
			: base(control) {
		}
		public override string ImageName { get { return "CreateFunnel3DChart"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdCreateFunnel3DChartDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdCreateFunnel3DChartMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.CreateFunnel3DChart; } }
		protected override ViewType ChartViewType { get { return ViewType.Funnel3D; } }
	}
}
