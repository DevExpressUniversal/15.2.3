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

using DevExpress.XtraSpreadsheet.Model;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		#region Tables
		#region ChartOfPieTypeTable
		internal static Dictionary<ChartOfPieType, string> ChartOfPieTypeTable = CreateChartOfPieTypeTable();
		static Dictionary<ChartOfPieType, string> CreateChartOfPieTypeTable() {
			Dictionary<ChartOfPieType, string> result = new Dictionary<ChartOfPieType, string>();
			result.Add(ChartOfPieType.Bar, "bar");
			result.Add(ChartOfPieType.Pie, "pie");
			return result;
		}
		#endregion
		#region OfPieSplitTypeTable
		internal static Dictionary<OfPieSplitType, string> OfPieSplitTypeTable = CreateOfPieSplitTypeTable();
		static Dictionary<OfPieSplitType, string> CreateOfPieSplitTypeTable() {
			Dictionary<OfPieSplitType, string> result = new Dictionary<OfPieSplitType, string>();
			result.Add(OfPieSplitType.Auto, "auto");
			result.Add(OfPieSplitType.Custom, "cust");
			result.Add(OfPieSplitType.Percent, "percent");
			result.Add(OfPieSplitType.Position, "pos");
			result.Add(OfPieSplitType.Value, "val");
			return result;
		}
		#endregion
		#endregion
		internal void GenerateOfPieChartView(OfPieChartView view) {
			WriteStartElement("ofPieChart", DrawingMLChartNamespace);
			try {
				GenerateChartSimpleStringAttributeTag("ofPieType", ChartOfPieTypeTable[view.OfPieType]);
				GeneratePieChartViewShared(view);
				GenerateChartSimpleIntAttributeTag("gapWidth", view.GapWidth);
				if (view.SplitType != OfPieSplitType.Auto) {
					GenerateChartSimpleStringAttributeTag("splitType", OfPieSplitTypeTable[view.SplitType]);
					if (view.SplitType != OfPieSplitType.Custom)
						GenerateChartSimpleDoubleAttributeTag("splitPos", view.SplitPos);
					else
						GenerateOfPieCustomSplit(view.SecondPiePoints);
				}
				GenerateChartSimpleIntAttributeTag("secondPieSize", view.SecondPieSize);
				foreach (ShapeProperties shapeProperties in view.SeriesLines)
					GenerateSeriesLines(shapeProperties);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateOfPieCustomSplit(SecondPiePointCollection secondPiePoints) {
			WriteStartElement("custSplit", DrawingMLChartNamespace);
			try {
				foreach (int point in secondPiePoints)
					GenerateChartSimpleIntAttributeTag("secondPiePt", point);
			}
			finally {
				WriteEndElement();
			}
		}
	}
}
