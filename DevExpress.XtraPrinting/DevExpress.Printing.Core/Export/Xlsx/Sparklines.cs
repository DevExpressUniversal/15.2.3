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
using System.Globalization;
using System.Text;
using DevExpress.Export.Xl;
using DevExpress.XtraExport.Implementation;
using DevExpress.Utils;
namespace DevExpress.XtraExport.Xlsx {
	partial class XlsxDataAwareExporter {
		#region Translation tables
		static Dictionary<XlSparklineType, string> sparklineTypeTable = CreateSparklineTypeTable();
		static Dictionary<XlSparklineType, string> CreateSparklineTypeTable() {
			Dictionary<XlSparklineType, string> result = new Dictionary<XlSparklineType, string>();
			result.Add(XlSparklineType.Line, "line");
			result.Add(XlSparklineType.Column, "column");
			result.Add(XlSparklineType.WinLoss, "stacked");
			return result;
		}
		static Dictionary<XlSparklineAxisScaling, string> sparklineAxisScalingTable = CreateSparklineAxisScalingTable();
		static Dictionary<XlSparklineAxisScaling, string> CreateSparklineAxisScalingTable() {
			Dictionary<XlSparklineAxisScaling, string> result = new Dictionary<XlSparklineAxisScaling, string>();
			result.Add(XlSparklineAxisScaling.Individual, "individual");
			result.Add(XlSparklineAxisScaling.Group, "group");
			result.Add(XlSparklineAxisScaling.Custom, "custom");
			return result;
		}
		static Dictionary<XlDisplayBlanksAs, string> displayBlanksAsTable = CreateDisplayBlanksAsTable();
		static Dictionary<XlDisplayBlanksAs, string> CreateDisplayBlanksAsTable() {
			Dictionary<XlDisplayBlanksAs, string> result = new Dictionary<XlDisplayBlanksAs, string>();
			result.Add(XlDisplayBlanksAs.Gap, "gap");
			result.Add(XlDisplayBlanksAs.Span, "span");
			result.Add(XlDisplayBlanksAs.Zero, "zero");
			return result;
		}
		#endregion
		const string SparklineGroupsExtUri = "{05C60535-1F16-4fd2-B633-F4F36F0B64E0}";
		bool ShouldExportSparklineGroupsExt(IList<XlSparklineGroup> sparklineGroups) {
			foreach(XlSparklineGroup group in sparklineGroups)
				if(group.Sparklines.Count > 0)
					return true;
			return false;
		}
		void GenerateSparklineGroupsExt(IXlSheet sheet) {
			WriteShStartElement("ext");
			try {
				WriteStringAttr("xmlns", "x14", null, x14NamespaceReference);
				WriteStringValue("uri", SparklineGroupsExtUri);
				WriteStartElement("sparklineGroups", x14NamespaceReference);
				try {
					WriteStringAttr("xmlns", "xm", null, xmNamespaceReference);
					foreach(XlSparklineGroup group in sheet.SparklineGroups)
						GenerateSparklineGroup(sheet, group);
				}
				finally {
					WriteShEndElement();
				}
			}
			finally {
				WriteShEndElement();
			}
		}
		void ApplySheetName(IXlSheet sheet, XlSparklineGroup sparklineGroup) {
			if(sparklineGroup.DateRange != null && string.IsNullOrEmpty(sparklineGroup.DateRange.SheetName))
				sparklineGroup.DateRange.SheetName = sheet.Name;
			foreach(XlSparkline sparkline in sparklineGroup.Sparklines) {
				if(string.IsNullOrEmpty(sparkline.DataRange.SheetName))
					sparkline.DataRange.SheetName = sheet.Name;
			}
		}
		void GenerateSparklineGroup(IXlSheet sheet, XlSparklineGroup sparklineGroup) {
			if(sparklineGroup.Sparklines.Count == 0)
				return;
			ApplySheetName(sheet, sparklineGroup);
			WriteStartElement("sparklineGroup", x14NamespaceReference);
			try {
				WriteSparklineGroupAttributes(sparklineGroup);
				if(!sparklineGroup.ColorSeries.IsAutoOrEmpty)
					WriteColor(sparklineGroup.ColorSeries, "colorSeries", x14NamespaceReference);
				if(!sparklineGroup.ColorNegative.IsAutoOrEmpty)
					WriteColor(sparklineGroup.ColorNegative, "colorNegative", x14NamespaceReference);
				if(!sparklineGroup.ColorAxis.IsAutoOrEmpty)
					WriteColor(sparklineGroup.ColorAxis, "colorAxis", x14NamespaceReference);
				if(!sparklineGroup.ColorMarker.IsAutoOrEmpty && sparklineGroup.SparklineType == XlSparklineType.Line)
					WriteColor(sparklineGroup.ColorMarker, "colorMarkers", x14NamespaceReference);
				if(!sparklineGroup.ColorFirst.IsAutoOrEmpty)
					WriteColor(sparklineGroup.ColorFirst, "colorFirst", x14NamespaceReference);
				if(!sparklineGroup.ColorLast.IsAutoOrEmpty)
					WriteColor(sparklineGroup.ColorLast, "colorLast", x14NamespaceReference);
				if(!sparklineGroup.ColorHigh.IsAutoOrEmpty)
					WriteColor(sparklineGroup.ColorHigh, "colorHigh", x14NamespaceReference);
				if(!sparklineGroup.ColorLow.IsAutoOrEmpty)
					WriteColor(sparklineGroup.ColorLow, "colorLow", x14NamespaceReference);
				if(sparklineGroup.DateRange != null)
					WriteString("f", xmNamespaceReference, sparklineGroup.DateRange.ToString());
				GenerateSparklines(sparklineGroup.Sparklines);
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteSparklineGroupAttributes(XlSparklineGroup sparklineGroup) {
			if(sparklineGroup.MaxScaling == XlSparklineAxisScaling.Custom)
				WriteDoubleValue("manualMax", sparklineGroup.ManualMax);
			if(sparklineGroup.MinScaling == XlSparklineAxisScaling.Custom)
				WriteDoubleValue("manualMin", sparklineGroup.ManualMin);
			if(sparklineGroup.LineWeight != 0.75)
				WriteDoubleValue("lineWeight", sparklineGroup.LineWeight);
			if(sparklineGroup.SparklineType != XlSparklineType.Line)
				WriteShStringValue("type", sparklineTypeTable[sparklineGroup.SparklineType]);
			if(sparklineGroup.DateRange != null)
				WriteBoolValue("dateAxis", true);
			if(sparklineGroup.DisplayBlanksAs != XlDisplayBlanksAs.Zero)
				WriteShStringValue("displayEmptyCellsAs", displayBlanksAsTable[sparklineGroup.DisplayBlanksAs]);
			if(sparklineGroup.DisplayMarkers && !sparklineGroup.ColorMarker.IsAutoOrEmpty)
				WriteBoolValue("markers", true);
			if(sparklineGroup.HighlightHighest && !sparklineGroup.ColorHigh.IsAutoOrEmpty)
				WriteBoolValue("high", true);
			if(sparklineGroup.HighlightLowest && !sparklineGroup.ColorLow.IsAutoOrEmpty)
				WriteBoolValue("low", true);
			if(sparklineGroup.HighlightFirst && !sparklineGroup.ColorFirst.IsAutoOrEmpty)
				WriteBoolValue("first", true);
			if(sparklineGroup.HighlightLast && !sparklineGroup.ColorLast.IsAutoOrEmpty)
				WriteBoolValue("last", true);
			if(sparklineGroup.HighlightNegative && !sparklineGroup.ColorNegative.IsAutoOrEmpty)
				WriteBoolValue("negative", true);
			if(sparklineGroup.DisplayXAxis && !sparklineGroup.ColorAxis.IsAutoOrEmpty)
				WriteBoolValue("displayXAxis", true);
			if(sparklineGroup.DisplayHidden)
				WriteBoolValue("displayHidden", true);
			if(sparklineGroup.MinScaling != XlSparklineAxisScaling.Individual)
				WriteShStringValue("minAxisType", sparklineAxisScalingTable[sparklineGroup.MinScaling]);
			if(sparklineGroup.MaxScaling != XlSparklineAxisScaling.Individual)
				WriteShStringValue("maxAxisType", sparklineAxisScalingTable[sparklineGroup.MaxScaling]);
			if(sparklineGroup.RightToLeft)
				WriteBoolValue("rightToLeft", true);
		}
		void GenerateSparklines(IList<XlSparkline> sparklines) {
			WriteStartElement("sparklines", x14NamespaceReference);
			try {
				foreach(XlSparkline sparkline in sparklines)
					GenerateSparkline(sparkline);
			}
			finally {
				WriteShEndElement();
			}
		}
		void GenerateSparkline(XlSparkline sparkline) {
			WriteStartElement("sparkline", x14NamespaceReference);
			try {
				WriteString("f", xmNamespaceReference, sparkline.DataRange.ToString());
				WriteString("sqref", xmNamespaceReference, sparkline.Location.ToString());
			}
			finally {
				WriteShEndElement();
			}
		}
	}
}
