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
using System.IO;
using System.Xml;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		#region ScatterChartSTyleTable
		internal static Dictionary<ScatterChartStyle, string> ScatterChartStyleTable = CreateScatterChartStyleTable();
		static Dictionary<ScatterChartStyle, string> CreateScatterChartStyleTable() {
			Dictionary<ScatterChartStyle, string> result = new Dictionary<ScatterChartStyle, string>();
			result.Add(ScatterChartStyle.Line, "line");
			result.Add(ScatterChartStyle.LineMarker, "lineMarker");
			result.Add(ScatterChartStyle.Marker, "marker");
			result.Add(ScatterChartStyle.None, "none");
			result.Add(ScatterChartStyle.Smooth, "smooth");
			result.Add(ScatterChartStyle.SmoothMarker, "smoothMarker");
			return result;
		}
		#endregion
		internal void GenerateScatterChartView(ScatterChartView view) {
			WriteStartElement("scatterChart", DrawingMLChartNamespace);
			try {
				GenerateChartSimpleStringAttributeTag("scatterStyle", ScatterChartStyleTable[GetScatterStyle(view)]);
				GenerateChartSimpleBoolAttributeTag("varyColors", view.VaryColors);
				GenerateChartViewSeries(view);
				GenerateDataLabels(view.DataLabels);
				GenerateAxisGroupRef(view);
			}
			finally {
				WriteEndElement();
			}
		}
		ScatterChartStyle GetScatterStyle(ScatterChartView view) {
			switch (view.ScatterStyle) {
				case ScatterChartStyle.Smooth:
				case ScatterChartStyle.SmoothMarker:
					return ScatterChartStyle.SmoothMarker;
				case ScatterChartStyle.Line:
				case ScatterChartStyle.LineMarker:
				case ScatterChartStyle.Marker:
				default:
					return ScatterChartStyle.LineMarker;
			}
		}
	}
}
