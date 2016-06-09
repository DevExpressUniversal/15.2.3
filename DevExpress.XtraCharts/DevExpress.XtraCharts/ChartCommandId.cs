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
using System.Runtime.InteropServices;
using DevExpress.Utils;
namespace DevExpress.XtraCharts.Commands {
	[Serializable, StructLayout(LayoutKind.Sequential), ComVisible(false)]
	public struct ChartCommandId : IConvertToInt<ChartCommandId>, IEquatable<ChartCommandId> {
		public static readonly ChartCommandId None = new ChartCommandId(0);
		public static readonly ChartCommandId CreateBarChart = new ChartCommandId(1);
		public static readonly ChartCommandId CreateBar3DChart = new ChartCommandId(2);
		public static readonly ChartCommandId CreateFullStackedBarChart = new ChartCommandId(3);
		public static readonly ChartCommandId CreateFullStackedBar3DChart = new ChartCommandId(4);
		public static readonly ChartCommandId CreateSideBySideFullStackedBarChart = new ChartCommandId(5);
		public static readonly ChartCommandId CreateSideBySideFullStackedBar3DChart = new ChartCommandId(6);
		public static readonly ChartCommandId CreateSideBySideStackedBarChart = new ChartCommandId(7);
		public static readonly ChartCommandId CreateSideBySideStackedBar3DChart = new ChartCommandId(8);
		public static readonly ChartCommandId CreateStackedBarChart = new ChartCommandId(9);
		public static readonly ChartCommandId CreateStackedBar3DChart = new ChartCommandId(10);
		public static readonly ChartCommandId CreateManhattanBarChart = new ChartCommandId(11);
		public static readonly ChartCommandId CreateBarChartPlaceHolder = new ChartCommandId(12);
		public static readonly ChartCommandId CreatePieChart = new ChartCommandId(13);
		public static readonly ChartCommandId CreatePie3DChart = new ChartCommandId(14);
		public static readonly ChartCommandId CreateDoughnutChart = new ChartCommandId(15);
		public static readonly ChartCommandId CreateDoughnut3DChart = new ChartCommandId(16);
		public static readonly ChartCommandId CreatePieChartPlaceHolder = new ChartCommandId(17);
		public static readonly ChartCommandId CreateAreaChart = new ChartCommandId(18);
		public static readonly ChartCommandId CreateArea3DChart = new ChartCommandId(19);
		public static readonly ChartCommandId CreateFullStackedAreaChart = new ChartCommandId(20);
		public static readonly ChartCommandId CreateFullStackedArea3DChart = new ChartCommandId(21);
		public static readonly ChartCommandId CreateFullStackedSplineAreaChart = new ChartCommandId(22);
		public static readonly ChartCommandId CreateFullStackedSplineArea3DChart = new ChartCommandId(23);
		public static readonly ChartCommandId CreateSplineAreaChart = new ChartCommandId(24);
		public static readonly ChartCommandId CreateSplineArea3DChart = new ChartCommandId(25);
		public static readonly ChartCommandId CreateStackedAreaChart = new ChartCommandId(26);
		public static readonly ChartCommandId CreateStackedArea3DChart = new ChartCommandId(27);
		public static readonly ChartCommandId CreateStackedSplineAreaChart = new ChartCommandId(28);
		public static readonly ChartCommandId CreateStackedSplineArea3DChart = new ChartCommandId(29);
		public static readonly ChartCommandId CreateStepAreaChart = new ChartCommandId(30);
		public static readonly ChartCommandId CreateStepArea3DChart = new ChartCommandId(31);
		public static readonly ChartCommandId CreateAreaChartPlaceHolder = new ChartCommandId(32);
		public static readonly ChartCommandId CreateLineChart = new ChartCommandId(33);
		public static readonly ChartCommandId CreateLine3DChart = new ChartCommandId(34);
		public static readonly ChartCommandId CreateFullStackedLineChart = new ChartCommandId(35);
		public static readonly ChartCommandId CreateFullStackedLine3DChart = new ChartCommandId(36);
		public static readonly ChartCommandId CreateScatterLineChart = new ChartCommandId(37);
		public static readonly ChartCommandId CreateSplineChart = new ChartCommandId(38);
		public static readonly ChartCommandId CreateSpline3DChart = new ChartCommandId(39);
		public static readonly ChartCommandId CreateStackedLineChart = new ChartCommandId(40);
		public static readonly ChartCommandId CreateStackedLine3DChart = new ChartCommandId(41);
		public static readonly ChartCommandId CreateStepLineChart = new ChartCommandId(42);
		public static readonly ChartCommandId CreateStepLine3DChart = new ChartCommandId(43);
		public static readonly ChartCommandId CreateLineChartPlaceHolder = new ChartCommandId(44);
		public static readonly ChartCommandId CreatePointChart = new ChartCommandId(45);
		public static readonly ChartCommandId CreateBubbleChart = new ChartCommandId(46);
		public static readonly ChartCommandId CreateFunnelChart = new ChartCommandId(47);
		public static readonly ChartCommandId CreateFunnel3DChart = new ChartCommandId(48);
		public static readonly ChartCommandId CreateRangeBarChart = new ChartCommandId(49);
		public static readonly ChartCommandId CreateSideBySideRangeBarChart = new ChartCommandId(50);
		public static readonly ChartCommandId CreateRangeAreaChart = new ChartCommandId(51);
		public static readonly ChartCommandId CreateRangeArea3DChart = new ChartCommandId(52);
		public static readonly ChartCommandId CreateRadarPointChart = new ChartCommandId(53);
		public static readonly ChartCommandId CreateRadarLineChart = new ChartCommandId(54);
		public static readonly ChartCommandId CreateRadarAreaChart = new ChartCommandId(55);
		public static readonly ChartCommandId CreatePolarPointChart = new ChartCommandId(56);
		public static readonly ChartCommandId CreatePolarLineChart = new ChartCommandId(57);
		public static readonly ChartCommandId CreatePolarAreaChart = new ChartCommandId(58);
		public static readonly ChartCommandId CreateStockChart = new ChartCommandId(59);
		public static readonly ChartCommandId CreateCandleStickChart = new ChartCommandId(60);
		public static readonly ChartCommandId CreateGanttChart = new ChartCommandId(61);
		public static readonly ChartCommandId CreateSideBySideGanttChart = new ChartCommandId(62);
		public static readonly ChartCommandId CreateOtherSeriesTypesChartPlaceHolder = new ChartCommandId(63);
		public static readonly ChartCommandId CreateRotatedBarChart = new ChartCommandId(64);
		public static readonly ChartCommandId CreateRotatedFullStackedBarChart = new ChartCommandId(65);
		public static readonly ChartCommandId CreateRotatedSideBySideFullStackedBarChart = new ChartCommandId(66);
		public static readonly ChartCommandId CreateRotatedSideBySideStackedBarChart = new ChartCommandId(67);
		public static readonly ChartCommandId CreateRotatedStackedBarChart = new ChartCommandId(68);
		public static readonly ChartCommandId CreateRotatedBarChartPlaceHolder = new ChartCommandId(69);
		public static readonly ChartCommandId ChangeAppearance = new ChartCommandId(70);
		public static readonly ChartCommandId ChangePalette = new ChartCommandId(71);
		public static readonly ChartCommandId ChangePalettePlaceHolder = new ChartCommandId(72);
		public static readonly ChartCommandId RunWizard = new ChartCommandId(73);
		public static readonly ChartCommandId SaveAsTemplate = new ChartCommandId(74);
		public static readonly ChartCommandId LoadTemplate = new ChartCommandId(75);
		public static readonly ChartCommandId PrintPreview = new ChartCommandId(76);
		public static readonly ChartCommandId Print = new ChartCommandId(77);
		public static readonly ChartCommandId ExportPlaceHolder = new ChartCommandId(78);
		public static readonly ChartCommandId ExportToPDF = new ChartCommandId(79);
		public static readonly ChartCommandId ExportToHTML = new ChartCommandId(80);
		public static readonly ChartCommandId ExportToMHT = new ChartCommandId(81);
		public static readonly ChartCommandId ExportToXLS = new ChartCommandId(82);
		public static readonly ChartCommandId ExportToXLSX = new ChartCommandId(83);
		public static readonly ChartCommandId ExportToRTF = new ChartCommandId(84);
		public static readonly ChartCommandId ExportToImagePlaceHolder = new ChartCommandId(85);
		public static readonly ChartCommandId ExportToBMP = new ChartCommandId(86);
		public static readonly ChartCommandId ExportToGIF = new ChartCommandId(87);
		public static readonly ChartCommandId ExportToJPEG = new ChartCommandId(88);
		public static readonly ChartCommandId ExportToPNG = new ChartCommandId(89);
		public static readonly ChartCommandId ExportToTIFF = new ChartCommandId(90);
		public static readonly ChartCommandId Column2DGroupPlaceHolder = new ChartCommandId(91);
		public static readonly ChartCommandId Column3DGroupPlaceHolder = new ChartCommandId(92);
		public static readonly ChartCommandId ColumnCylinderGroupPlaceHolder = new ChartCommandId(93);
		public static readonly ChartCommandId ColumnConeGroupPlaceHolder = new ChartCommandId(94);
		public static readonly ChartCommandId ColumnPyramidGroupPlaceHolder = new ChartCommandId(95);
		public static readonly ChartCommandId Line2DGroupPlaceHolder = new ChartCommandId(96);
		public static readonly ChartCommandId Line3DGroupPlaceHolder = new ChartCommandId(97);
		public static readonly ChartCommandId Pie2DGroupPlaceHolder = new ChartCommandId(98);
		public static readonly ChartCommandId Pie3DGroupPlaceHolder = new ChartCommandId(99);
		public static readonly ChartCommandId Bar2DGroupPlaceHolder = new ChartCommandId(100);
		public static readonly ChartCommandId Area2DGroupPlaceHolder = new ChartCommandId(101);
		public static readonly ChartCommandId Area3DGroupPlaceHolder = new ChartCommandId(102);
		public static readonly ChartCommandId PointGroupPlaceHolder = new ChartCommandId(103);
		public static readonly ChartCommandId FunnelGroupPlaceHolder = new ChartCommandId(104);
		public static readonly ChartCommandId FinancialGroupPlaceHolder = new ChartCommandId(105);
		public static readonly ChartCommandId RadarGroupPlaceHolder = new ChartCommandId(106);
		public static readonly ChartCommandId PolarGroupPlaceHolder = new ChartCommandId(107);
		public static readonly ChartCommandId RangeGroupPlaceHolder = new ChartCommandId(108);
		public static readonly ChartCommandId GanttGroupPlaceHolder = new ChartCommandId(109);
		public static readonly ChartCommandId CreateConeBar3DChart = new ChartCommandId(110);
		public static readonly ChartCommandId CreateConeFullStackedBar3DChart = new ChartCommandId(111);
		public static readonly ChartCommandId CreateConeSideBySideFullStackedBar3DChart = new ChartCommandId(112);
		public static readonly ChartCommandId CreateConeSideBySideStackedBar3DChart = new ChartCommandId(113);
		public static readonly ChartCommandId CreateConeStackedBar3DChart = new ChartCommandId(114);
		public static readonly ChartCommandId CreateConeManhattanBarChart = new ChartCommandId(115);
		public static readonly ChartCommandId CreateCylinderBar3DChart = new ChartCommandId(116);
		public static readonly ChartCommandId CreateCylinderFullStackedBar3DChart = new ChartCommandId(117);
		public static readonly ChartCommandId CreateCylinderSideBySideFullStackedBar3DChart = new ChartCommandId(118);
		public static readonly ChartCommandId CreateCylinderSideBySideStackedBar3DChart = new ChartCommandId(119);
		public static readonly ChartCommandId CreateCylinderStackedBar3DChart = new ChartCommandId(120);
		public static readonly ChartCommandId CreateCylinderManhattanBarChart = new ChartCommandId(121);
		public static readonly ChartCommandId CreatePyramidBar3DChart = new ChartCommandId(122);
		public static readonly ChartCommandId CreatePyramidFullStackedBar3DChart = new ChartCommandId(123);
		public static readonly ChartCommandId CreatePyramidSideBySideFullStackedBar3DChart = new ChartCommandId(124);
		public static readonly ChartCommandId CreatePyramidSideBySideStackedBar3DChart = new ChartCommandId(125);
		public static readonly ChartCommandId CreatePyramidStackedBar3DChart = new ChartCommandId(126);
		public static readonly ChartCommandId CreatePyramidManhattanBarChart = new ChartCommandId(127);
		public static readonly ChartCommandId ChangeAppearancePlaceHolder = new ChartCommandId(128);
		public static readonly ChartCommandId CreateNestedDoughnutChart = new ChartCommandId(129);
		public static readonly ChartCommandId CreateScatterRadarLineChart = new ChartCommandId(130);
		public static readonly ChartCommandId CreateScatterPolarLineChart = new ChartCommandId(131);
		public static readonly ChartCommandId RunDesigner = new ChartCommandId(132);
		readonly int m_value;
		public ChartCommandId(int value) {
			m_value = value;
		}
		public override bool Equals(object obj) {
			return ((obj is ChartCommandId) && (this.m_value == ((ChartCommandId)obj).m_value));
		}
		public override int GetHashCode() {
			return m_value.GetHashCode();
		}
		public override string ToString() {
			return m_value.ToString();
		}
		public static bool operator ==(ChartCommandId id1, ChartCommandId id2) {
			return id1.m_value == id2.m_value;
		}
		public static bool operator !=(ChartCommandId id1, ChartCommandId id2) {
			return id1.m_value != id2.m_value;
		}
		#region IConvertToInt<RichEditCommandId> Members
		int IConvertToInt<ChartCommandId>.ToInt() {
			return m_value;
		}
		ChartCommandId IConvertToInt<ChartCommandId>.FromInt(int value) {
			return new ChartCommandId(value);
		}
		#endregion
		#region IEquatable<RichEditCommandId> Members
		public bool Equals(ChartCommandId other) {
			return this.m_value == other.m_value;
		}
		#endregion
	}
}
