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

using DevExpress.Web.ASPxSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Commands;
namespace DevExpress.Web.ASPxSpreadsheet {
	#region Column
	public class SRInsertChartColumnCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartColumnCommandGroup;
			}
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override RibbonItemSize DefaultItemSize {
			get {
				return RibbonItemSize.Large;
			}
		}
		protected override void FillItems() {
			Items.Add(new SRInsertChartClustered2DCommand());
			Items.Add(new SRInsertChartStacked2DCommand());
			Items.Add(new SRInsertChartPercentStacked2DCommand());
			Items.Add(new SRInsertChartClustered3DCommand());
			Items.Add(new SRInsertChartStacked3DCommand());
			Items.Add(new SRInsertChartPercentStacked3DCommand());
			Items.Add(new SRInsertChartColumn3DCommand());
			Items.Add(new SRInsertChartCylinderClusteredCommand());
			Items.Add(new SRInsertChartCylinderStackedCommand());
			Items.Add(new SRInsertChartCylinderPercentStackedCommand());
			Items.Add(new SRInsertChartCylinderCommand());
			Items.Add(new SRInsertChartConeClusteredCommand());
			Items.Add(new SRInsertChartConeStackedCommand());
			Items.Add(new SRInsertChartConePercentStackedCommand());
			Items.Add(new SRInsertChartConeCommand());
			Items.Add(new SRInsertChartPyramidClusteredCommand());
			Items.Add(new SRInsertChartPyramidStackedCommand());
			Items.Add(new SRInsertChartPyramidPercentStackedCommand());
			Items.Add(new SRInsertChartPyramidCommand());
		}
	}
	public class SRInsertChartClustered2DCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartColumnClustered2D;
			}
		}
	}
	public class SRInsertChartStacked2DCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartColumnStacked2D;
			}
		}
	}
	public class SRInsertChartPercentStacked2DCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartColumnPercentStacked2D;
			}
		}
	}
	public class SRInsertChartClustered3DCommand : SRDropDownCommandBase {
		public SRInsertChartClustered3DCommand() {
			BeginGroup = true;
		}
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartColumnClustered3D;
			}
		}
	}
	public class SRInsertChartStacked3DCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartColumnStacked3D;
			}
		}
	}
	public class SRInsertChartPercentStacked3DCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartColumnPercentStacked3D;
			}
		}
	}
	public class SRInsertChartColumn3DCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartColumn3D;
			}
		}
	}
	public class SRInsertChartCylinderClusteredCommand : SRDropDownCommandBase {
		public SRInsertChartCylinderClusteredCommand() {
			BeginGroup = true;
		}
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartCylinderClustered;
			}
		}
	}
	public class SRInsertChartCylinderStackedCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartCylinderStacked;
			}
		}
	}
	public class SRInsertChartCylinderPercentStackedCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartCylinderPercentStacked;
			}
		}
	}
	public class SRInsertChartCylinderCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartCylinder;
			}
		}
	}
	public class SRInsertChartConeClusteredCommand : SRDropDownCommandBase {
		public SRInsertChartConeClusteredCommand() {
			BeginGroup = true;
		}
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartConeClustered;
			}
		}
	}
	public class SRInsertChartConeStackedCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartConeStacked;
			}
		}
	}
	public class SRInsertChartConePercentStackedCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartConePercentStacked;
			}
		}
	}
	public class SRInsertChartConeCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartCone;
			}
		}
	}
	public class SRInsertChartPyramidClusteredCommand : SRDropDownCommandBase {
		public SRInsertChartPyramidClusteredCommand() {
			BeginGroup = true;
		}
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartPyramidClustered;
			}
		}
	}
	public class SRInsertChartPyramidStackedCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartPyramidStacked;
			}
		}
	}
	public class SRInsertChartPyramidPercentStackedCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartPyramidPercentStacked;
			}
		}
	}
	public class SRInsertChartPyramidCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartPyramid;
			}
		}
	}
	#endregion
	#region Line
	public class SRInsertChartLinesCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartLineCommandGroup;
			}
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override RibbonItemSize DefaultItemSize {
			get {
				return RibbonItemSize.Large;
			}
		}
		protected override void FillItems() {
			Items.Add(new SRInsertChartLineCommand());
			Items.Add(new SRInsertChartStackedLineCommand());
			Items.Add(new SRInsertChartPercentStackedLineCommand());
			Items.Add(new SRInsertChartLineWithMarkersCommand());
			Items.Add(new SRInsertChartStackedLineWithMarkersCommand());
			Items.Add(new SRInsertChartPercentStackedLineWithMarkersCommand());
			Items.Add(new SRInsertChartLine3DCommand());
		}
	}
	public class SRInsertChartLineCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartLine;
			}
		}
	}
	public class SRInsertChartStackedLineCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartStackedLine;
			}
		}
	}
	public class SRInsertChartPercentStackedLineCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartPercentStackedLine;
			}
		}
	}
	public class SRInsertChartLineWithMarkersCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartLineWithMarkers;
			}
		}
	}
	public class SRInsertChartStackedLineWithMarkersCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartStackedLineWithMarkers;
			}
		}
	}
	public class SRInsertChartPercentStackedLineWithMarkersCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartPercentStackedLineWithMarkers;
			}
		}
	}
	public class SRInsertChartLine3DCommand : SRDropDownCommandBase {
		public SRInsertChartLine3DCommand() {
			BeginGroup = true;
		}
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartLine3D;
			}
		}
	}
	#endregion
	#region Pie
	public class SRInsertChartPiesCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartPieCommandGroup;
			}
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override RibbonItemSize DefaultItemSize {
			get {
				return RibbonItemSize.Large;
			}
		}
		protected override void FillItems() {
			Items.Add(new SRInsertChartPie2DCommand());
			Items.Add(new SRInsertChartPieExploded2DCommand());
			Items.Add(new SRInsertChartPie3DCommand());
			Items.Add(new SRInsertChartPieExploded3DCommand());
			Items.Add(new SRInsertChartDoughnut2DCommand());
			Items.Add(new SRInsertChartDoughnutExploded2DCommand());
		}
	}
	public class SRInsertChartPie2DCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartPie2D;
			}
		}
	}
	public class SRInsertChartPieExploded2DCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartPieExploded2D;
			}
		}
	}
	public class SRInsertChartPie3DCommand : SRDropDownCommandBase {
		public SRInsertChartPie3DCommand() {
			BeginGroup = true;
		}
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartPie3D;
			}
		}
	}
	public class SRInsertChartPieExploded3DCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartPieExploded3D;
			}
		}
	}
	public class SRInsertChartDoughnut2DCommand : SRDropDownCommandBase {
		public SRInsertChartDoughnut2DCommand() {
			BeginGroup = true;
		}
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartDoughnut2D;
			}
		}
	}
	public class SRInsertChartDoughnutExploded2DCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartDoughnutExploded2D;
			}
		}
	}
	#endregion
	#region Bar
	public class SRInsertChartBarsCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartBarCommandGroup;
			}
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override RibbonItemSize DefaultItemSize {
			get {
				return RibbonItemSize.Large;
			}
		}
		protected override void FillItems() {
			Items.Add(new SRInsertChartBarClustered2DCommand());
			Items.Add(new SRInsertChartBarStacked2DCommand());
			Items.Add(new SRInsertChartBarPercentStacked2DCommand());
			Items.Add(new SRInsertChartBarClustered3DCommand());
			Items.Add(new SRInsertChartBarStacked3DCommand());
			Items.Add(new SRInsertChartBarPercentStacked3DCommand());
			Items.Add(new SRInsertChartHorizontalCylinderClusteredCommand());
			Items.Add(new SRInsertChartHorizontalCylinderStackedCommand());
			Items.Add(new SRInsertChartHorizontalCylinderPercentStackedCommand());
			Items.Add(new SRInsertChartHorizontalConeClusteredCommand());
			Items.Add(new SRInsertChartHorizontalConeStackedCommand());
			Items.Add(new SRInsertChartHorizontalConePercentStackedCommand());
			Items.Add(new SRInsertChartHorizontalPyramidClusteredCommand());
			Items.Add(new SRInsertChartHorizontalPyramidStackedCommand());
			Items.Add(new SRInsertChartHorizontalPyramidPercentStackedCommand());
		}
	}
	public class SRInsertChartBarClustered2DCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartBarClustered2D;
			}
		}
	}
	public class SRInsertChartBarStacked2DCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartBarStacked2D;
			}
		}
	}
	public class SRInsertChartBarPercentStacked2DCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartBarPercentStacked2D;
			}
		}
	}
	public class SRInsertChartBarClustered3DCommand : SRDropDownCommandBase {
		public SRInsertChartBarClustered3DCommand() {
			BeginGroup = true;
		}
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartBarClustered3D;
			}
		}
	}
	public class SRInsertChartBarStacked3DCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartBarStacked3D;
			}
		}
	}
	public class SRInsertChartBarPercentStacked3DCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartBarPercentStacked3D;
			}
		}
	}
	public class SRInsertChartHorizontalCylinderClusteredCommand : SRDropDownCommandBase {
		public SRInsertChartHorizontalCylinderClusteredCommand() {
			BeginGroup = true;
		}
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartHorizontalCylinderClustered;
			}
		}
	}
	public class SRInsertChartHorizontalCylinderStackedCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartHorizontalCylinderStacked;
			}
		}
	}
	public class SRInsertChartHorizontalCylinderPercentStackedCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartHorizontalCylinderPercentStacked;
			}
		}
	}
	public class SRInsertChartHorizontalConeClusteredCommand : SRDropDownCommandBase {
		public SRInsertChartHorizontalConeClusteredCommand() {
			BeginGroup = true;
		}
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartHorizontalConeClustered;
			}
		}
	}
	public class SRInsertChartHorizontalConeStackedCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartHorizontalConeStacked;
			}
		}
	}
	public class SRInsertChartHorizontalConePercentStackedCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartHorizontalConePercentStacked;
			}
		}
	}
	public class SRInsertChartHorizontalPyramidClusteredCommand : SRDropDownCommandBase {
		public SRInsertChartHorizontalPyramidClusteredCommand() {
			BeginGroup = true;
		}
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartHorizontalPyramidClustered;
			}
		}
	}
	public class SRInsertChartHorizontalPyramidStackedCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartHorizontalPyramidStacked;
			}
		}
	}
	public class SRInsertChartHorizontalPyramidPercentStackedCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartHorizontalPyramidPercentStacked;
			}
		}
	}
	#endregion
	#region Area
	public class SRInsertChartAreasCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartAreaCommandGroup;
			}
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override RibbonItemSize DefaultItemSize {
			get {
				return RibbonItemSize.Large;
			}
		}
		protected override void FillItems() {
			Items.Add(new SRInsertChartAreaCommand());
			Items.Add(new SRInsertChartStackedAreaCommand());
			Items.Add(new SRInsertChartPercentStackedAreaCommand());
			Items.Add(new SRInsertChartArea3DCommand());
			Items.Add(new SRInsertChartStackedArea3DCommand());
			Items.Add(new SRInsertChartPercentStackedArea3DCommand());
		}
	}
	public class SRInsertChartAreaCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartArea;
			}
		}
	}
	public class SRInsertChartStackedAreaCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartStackedArea;
			}
		}
	}
	public class SRInsertChartPercentStackedAreaCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartPercentStackedArea;
			}
		}
	}
	public class SRInsertChartArea3DCommand : SRDropDownCommandBase {
		public SRInsertChartArea3DCommand() {
			BeginGroup = true;
		}
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartArea3D;
			}
		}
	}
	public class SRInsertChartStackedArea3DCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartStackedArea3D;
			}
		}
	}
	public class SRInsertChartPercentStackedArea3DCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartPercentStackedArea3D;
			}
		}
	}
	#endregion
	#region Scatter
	public class SRInsertChartScattersCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartScatterCommandGroup;
			}
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override RibbonItemSize DefaultItemSize {
			get {
				return RibbonItemSize.Large;
			}
		}
		protected override void FillItems() {
			Items.Add(new SRInsertChartScatterMarkersCommand());
			Items.Add(new SRInsertChartScatterSmoothLinesAndMarkersCommand());
			Items.Add(new SRInsertChartScatterSmoothLinesCommand());
			Items.Add(new SRInsertChartScatterLinesAndMarkersCommand());
			Items.Add(new SRInsertChartScatterLinesCommand());
			Items.Add(new SRInsertChartBubbleCommand());
			Items.Add(new SRInsertChartBubble3DCommand());
		}
	}
	public class SRInsertChartScatterMarkersCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartScatterMarkers;
			}
		}
	}
	public class SRInsertChartScatterSmoothLinesAndMarkersCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartScatterSmoothLinesAndMarkers;
			}
		}
	}
	public class SRInsertChartScatterSmoothLinesCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartScatterSmoothLines;
			}
		}
	}
	public class SRInsertChartScatterLinesAndMarkersCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartScatterLinesAndMarkers;
			}
		}
	}
	public class SRInsertChartScatterLinesCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartScatterLines;
			}
		}
	}
	public class SRInsertChartBubbleCommand : SRDropDownCommandBase {
		public SRInsertChartBubbleCommand() {
			BeginGroup = true;
		}
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartBubble;
			}
		}
	}
	public class SRInsertChartBubble3DCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartBubble3D;
			}
		}
	}
	#endregion
	#region OtherCharts
	public class SRInsertChartOthersCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartOtherCommandGroup;
			}
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override RibbonItemSize DefaultItemSize {
			get {
				return RibbonItemSize.Large;
			}
		}
		protected override void FillItems() {
			Items.Add(new SRInsertChartStockHighLowCloseCommand());
			Items.Add(new SRInsertChartStockOpenHighLowCloseCommand());
			Items.Add(new SRInsertChartRadarCommand());
			Items.Add(new SRInsertChartRadarWithMarkersCommand());
			Items.Add(new SRInsertChartRadarFilledCommand());
		}
	}
	public class SRInsertChartStockHighLowCloseCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartStockHighLowClose;
			}
		}
	}
	public class SRInsertChartStockOpenHighLowCloseCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartStockOpenHighLowClose;
			}
		}
	}
	public class SRInsertChartRadarCommand : SRDropDownCommandBase {
		public SRInsertChartRadarCommand() {
			BeginGroup = true;
		}
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartRadar;
			}
		}
	}
	public class SRInsertChartRadarWithMarkersCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartRadarWithMarkers;
			}
		}
	}
	public class SRInsertChartRadarFilledCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertChartRadarFilled;
			}
		}
	}
	#endregion
}
