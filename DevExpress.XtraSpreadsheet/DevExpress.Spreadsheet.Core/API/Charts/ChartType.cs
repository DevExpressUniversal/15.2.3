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
using System.ComponentModel;
using DevExpress.Office;
namespace DevExpress.Spreadsheet.Charts {
	public enum ChartType {
		ColumnClustered = DevExpress.XtraSpreadsheet.Model.ChartType.ColumnClustered,
		ColumnStacked = DevExpress.XtraSpreadsheet.Model.ChartType.ColumnStacked,
		ColumnFullStacked = DevExpress.XtraSpreadsheet.Model.ChartType.ColumnFullStacked,
		Column3DClustered = DevExpress.XtraSpreadsheet.Model.ChartType.Column3DClustered,
		Column3DStacked = DevExpress.XtraSpreadsheet.Model.ChartType.Column3DStacked,
		Column3DFullStacked = DevExpress.XtraSpreadsheet.Model.ChartType.Column3DFullStacked,
		Column3DStandard = DevExpress.XtraSpreadsheet.Model.ChartType.Column3DStandard,
		Column3DClusteredCylinder = DevExpress.XtraSpreadsheet.Model.ChartType.Column3DClusteredCylinder,
		Column3DStackedCylinder = DevExpress.XtraSpreadsheet.Model.ChartType.Column3DStackedCylinder,
		Column3DFullStackedCylinder = DevExpress.XtraSpreadsheet.Model.ChartType.Column3DFullStackedCylinder,
		Column3DStandardCylinder = DevExpress.XtraSpreadsheet.Model.ChartType.Column3DStandardCylinder,
		Column3DClusteredCone = DevExpress.XtraSpreadsheet.Model.ChartType.Column3DClusteredCone,
		Column3DStackedCone = DevExpress.XtraSpreadsheet.Model.ChartType.Column3DStackedCone,
		Column3DFullStackedCone = DevExpress.XtraSpreadsheet.Model.ChartType.Column3DFullStackedCone,
		Column3DStandardCone = DevExpress.XtraSpreadsheet.Model.ChartType.Column3DStandardCone,
		Column3DClusteredPyramid = DevExpress.XtraSpreadsheet.Model.ChartType.Column3DClusteredPyramid,
		Column3DStackedPyramid = DevExpress.XtraSpreadsheet.Model.ChartType.Column3DStackedPyramid,
		Column3DFullStackedPyramid = DevExpress.XtraSpreadsheet.Model.ChartType.Column3DFullStackedPyramid,
		Column3DStandardPyramid = DevExpress.XtraSpreadsheet.Model.ChartType.Column3DStandardPyramid,
		Line = DevExpress.XtraSpreadsheet.Model.ChartType.Line,
		LineStacked = DevExpress.XtraSpreadsheet.Model.ChartType.LineStacked,
		LineFullStacked = DevExpress.XtraSpreadsheet.Model.ChartType.LineFullStacked,
		LineMarker = DevExpress.XtraSpreadsheet.Model.ChartType.LineMarker,
		LineStackedMarker = DevExpress.XtraSpreadsheet.Model.ChartType.LineStackedMarker,
		LineFullStackedMarker = DevExpress.XtraSpreadsheet.Model.ChartType.LineFullStackedMarker,
		Line3D = DevExpress.XtraSpreadsheet.Model.ChartType.Line3D,
		Pie = DevExpress.XtraSpreadsheet.Model.ChartType.Pie,
		Pie3D = DevExpress.XtraSpreadsheet.Model.ChartType.Pie3D,
		PieExploded = DevExpress.XtraSpreadsheet.Model.ChartType.PieExploded,
		Pie3DExploded = DevExpress.XtraSpreadsheet.Model.ChartType.Pie3DExploded,
		PieOfPie = DevExpress.XtraSpreadsheet.Model.ChartType.PieOfPie,
		BarOfPie = DevExpress.XtraSpreadsheet.Model.ChartType.BarOfPie,
		BarClustered = DevExpress.XtraSpreadsheet.Model.ChartType.BarClustered,
		BarStacked = DevExpress.XtraSpreadsheet.Model.ChartType.BarStacked,
		BarFullStacked = DevExpress.XtraSpreadsheet.Model.ChartType.BarFullStacked,
		Bar3DClustered = DevExpress.XtraSpreadsheet.Model.ChartType.Bar3DClustered,
		Bar3DStacked = DevExpress.XtraSpreadsheet.Model.ChartType.Bar3DStacked,
		Bar3DFullStacked = DevExpress.XtraSpreadsheet.Model.ChartType.Bar3DFullStacked,
		Bar3DClusteredCylinder = DevExpress.XtraSpreadsheet.Model.ChartType.Bar3DClusteredCylinder,
		Bar3DStackedCylinder = DevExpress.XtraSpreadsheet.Model.ChartType.Bar3DStackedCylinder,
		Bar3DFullStackedCylinder = DevExpress.XtraSpreadsheet.Model.ChartType.Bar3DFullStackedCylinder,
		Bar3DClusteredCone = DevExpress.XtraSpreadsheet.Model.ChartType.Bar3DClusteredCone,
		Bar3DStackedCone = DevExpress.XtraSpreadsheet.Model.ChartType.Bar3DStackedCone,
		Bar3DFullStackedCone = DevExpress.XtraSpreadsheet.Model.ChartType.Bar3DFullStackedCone,
		Bar3DClusteredPyramid = DevExpress.XtraSpreadsheet.Model.ChartType.Bar3DClusteredPyramid,
		Bar3DStackedPyramid = DevExpress.XtraSpreadsheet.Model.ChartType.Bar3DStackedPyramid,
		Bar3DFullStackedPyramid = DevExpress.XtraSpreadsheet.Model.ChartType.Bar3DFullStackedPyramid,
		Area = DevExpress.XtraSpreadsheet.Model.ChartType.Area,
		AreaStacked = DevExpress.XtraSpreadsheet.Model.ChartType.AreaStacked,
		AreaFullStacked = DevExpress.XtraSpreadsheet.Model.ChartType.AreaFullStacked,
		Area3D = DevExpress.XtraSpreadsheet.Model.ChartType.Area3D,
		Area3DStacked = DevExpress.XtraSpreadsheet.Model.ChartType.Area3DStacked,
		Area3DFullStacked = DevExpress.XtraSpreadsheet.Model.ChartType.Area3DFullStacked,
		ScatterMarkers = DevExpress.XtraSpreadsheet.Model.ChartType.ScatterMarkers,
		ScatterSmoothMarkers = DevExpress.XtraSpreadsheet.Model.ChartType.ScatterSmoothMarkers,
		ScatterSmooth = DevExpress.XtraSpreadsheet.Model.ChartType.ScatterSmooth,
		ScatterLine = DevExpress.XtraSpreadsheet.Model.ChartType.ScatterLine,
		ScatterLineMarkers = DevExpress.XtraSpreadsheet.Model.ChartType.ScatterLineMarkers,
		StockHighLowClose = DevExpress.XtraSpreadsheet.Model.ChartType.StockHighLowClose,
		StockOpenHighLowClose = DevExpress.XtraSpreadsheet.Model.ChartType.StockOpenHighLowClose,
		StockVolumeHighLowClose = DevExpress.XtraSpreadsheet.Model.ChartType.StockVolumeHighLowClose,
		StockVolumeOpenHighLowClose = DevExpress.XtraSpreadsheet.Model.ChartType.StockVolumeOpenHighLowClose,
		Surface = DevExpress.XtraSpreadsheet.Model.ChartType.Surface,
		SurfaceWireframe = DevExpress.XtraSpreadsheet.Model.ChartType.SurfaceWireframe,
		Surface3D = DevExpress.XtraSpreadsheet.Model.ChartType.Surface3D,
		Surface3DWireframe = DevExpress.XtraSpreadsheet.Model.ChartType.Surface3DWireframe,
		Doughnut = DevExpress.XtraSpreadsheet.Model.ChartType.Doughnut,
		DoughnutExploded = DevExpress.XtraSpreadsheet.Model.ChartType.DoughnutExploded,
		Bubble = DevExpress.XtraSpreadsheet.Model.ChartType.Bubble,
		Bubble3D = DevExpress.XtraSpreadsheet.Model.ChartType.Bubble3D,
		Radar = DevExpress.XtraSpreadsheet.Model.ChartType.Radar,
		RadarMarkers = DevExpress.XtraSpreadsheet.Model.ChartType.RadarMarkers,
		RadarFilled = DevExpress.XtraSpreadsheet.Model.ChartType.RadarFilled
	}
}
