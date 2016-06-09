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

using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		#region Static
		internal static Dictionary<ChartTrendlineType, string> TrendlineTypeTable = CreateTrendlineTypeTable();
		static Dictionary<ChartTrendlineType, string> CreateTrendlineTypeTable() {
			Dictionary<ChartTrendlineType, string> result = new Dictionary<ChartTrendlineType, string>();
			result.Add(ChartTrendlineType.Exponential, "exp");
			result.Add(ChartTrendlineType.Linear, "linear");
			result.Add(ChartTrendlineType.Logarithmic, "log");
			result.Add(ChartTrendlineType.MovingAverage, "movingAvg");
			result.Add(ChartTrendlineType.Polynomial, "poly");
			result.Add(ChartTrendlineType.Power, "power");
			return result;
		}
		#endregion
		void GenerateTrendlines(TrendlineCollection trendlines) {
			foreach (Trendline item in trendlines)
				GenerateTrendline(item);
		}
		protected internal void GenerateTrendline(Trendline trendline) {
			WriteStartElement("trendline", DrawingMLChartNamespace);
			try {
				if (trendline.HasCustomName)
					GenerateChartSimpleStringTag("name", trendline.Name);
				GenerateChartShapeProperties(trendline.ShapeProperties);
				GenerateChartSimpleStringAttributeTag("trendlineType", TrendlineTypeTable[trendline.TrendlineType]);
				if (trendline.TrendlineType == ChartTrendlineType.Polynomial)
					GenerateChartSimpleIntAttributeTag("order", trendline.Order);
				if (trendline.TrendlineType == ChartTrendlineType.MovingAverage)
					GenerateChartSimpleIntAttributeTag("period", trendline.Period);
				if (trendline.Forward != 0.0)
					GenerateChartSimpleDoubleAttributeTag("forward", trendline.Forward);
				if (trendline.Backward != 0.0)
					GenerateChartSimpleDoubleAttributeTag("backward", trendline.Backward);
				if (trendline.HasIntercept)
					GenerateChartSimpleDoubleAttributeTag("intercept", trendline.Intercept);
				GenerateChartSimpleBoolAttributeTag("dispRSqr", trendline.DisplayRSquare);
				GenerateChartSimpleBoolAttributeTag("dispEq", trendline.DisplayEquation);
				if(trendline.HasLabel)
					GenerateTrendlineLabel(trendline.Label);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateTrendlineLabel(TrendlineLabel label) {
			WriteStartElement("trendlineLbl", DrawingMLChartNamespace);
			try {
				GenerateChartLayoutContent(label.Layout);
				GenerateChartTextContent(label.Text);
				GenerateNumberFormatContent(label.NumberFormat);
				GenerateChartShapeProperties(label.ShapeProperties);
				GenerateTextPropertiesContent(label.TextProperties);
			}
			finally {
				WriteEndElement();
			}
		}
	}
}
