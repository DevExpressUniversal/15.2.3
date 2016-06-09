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
using DevExpress.XtraSpreadsheet.Model;
using System.Collections.Generic;
using System.Globalization;
using DevExpress.Office.Utils;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		#region Translation tables
		internal static Dictionary<SparklineGroupType, string> SparklineGroupTypeTable = CreateSparklineGroupTypeTable();
		internal static Dictionary<SparklineAxisScaling, string> SparklineAxisScalingTable = CreateSparklineAxisScalingTable();
		static Dictionary<SparklineGroupType, string> CreateSparklineGroupTypeTable() {
			Dictionary<SparklineGroupType, string> result = new Dictionary<SparklineGroupType, string>();
			result.Add(SparklineGroupType.Line, "line");
			result.Add(SparklineGroupType.Column, "column");
			result.Add(SparklineGroupType.Stacked, "stacked");
			return result;
		}
		static Dictionary<SparklineAxisScaling, string> CreateSparklineAxisScalingTable() {
			Dictionary<SparklineAxisScaling, string> result = new Dictionary<SparklineAxisScaling, string>();
			result.Add(SparklineAxisScaling.Individual, "individual");
			result.Add(SparklineAxisScaling.Group, "group");
			result.Add(SparklineAxisScaling.Custom, "custom");
			return result;
		}
		#endregion
		#region SparklineGroupsExt
		protected internal virtual void GenerateSparklineGroupsExt() {
			SparklineGroupCollection sparklineGroups = ActiveSheet.SparklineGroups;
			WriteShStartElement("ext");
			try {
				WriteStringAttr("xmlns", "x14", null, x14NamespaceReference);
				WriteStringValue("uri", SparklineGroupsExtUri);
				WriteStartElement("sparklineGroups", x14NamespaceReference);
				try {
					WriteStringAttr("xmlns", "xm", null, xmNamespaceReference);
					sparklineGroups.ForEach(GenerateSparklineGroup);
				}
				finally {
					WriteShEndElement();
				}
			}
			finally {
				WriteShEndElement();
			}
		}
		bool ShouldGenerateSparklineGroupsExt() {
			if (!Workbook.DocumentCapabilities.SparklinesAllowed || ActiveSheet == null)
				return false;
			SparklineGroupCollection sparklineGroups = ActiveSheet.SparklineGroups;
			return sparklineGroups.Count > 0 && HasSparklines(sparklineGroups);
		}
		bool HasSparklines(SparklineGroupCollection sparklineGroups) {
			int result = 0;
			foreach (SparklineGroup sparklineGroup in sparklineGroups)
				result += sparklineGroup.Sparklines.Count;
			return result > 0;
		}
		protected internal void GenerateSparklineGroup(SparklineGroup sparklineGroup) {
			SparklineCollection sparklines = sparklineGroup.Sparklines;
			if (sparklines.Count == 0)
				return;
			SparklineGroupInfo defaultInfo = sparklineGroup.Sheet.Workbook.Cache.SparklineGroupInfoCache.DefaultItem;
			WriteStartElement("sparklineGroup", x14NamespaceReference);
			try {
				WriteSparklineGroupAttributes(sparklineGroup, defaultInfo);
				WriteSparklineColors(sparklineGroup);
				if (sparklineGroup.DateRange != null)
					WriteString("f", xmNamespaceReference, PrepareFormula(sparklineGroup.Expression));
				GenerateSparklines(sparklines);
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteSparklineGroupAttributes(SparklineGroup sparklineGroup, SparklineGroupInfo defaultInfo) {
			if (sparklineGroup.MaxAxisScaleType == SparklineAxisScaling.Custom)
				WriteDoubleValue("manualMax", sparklineGroup.MaxAxisValue);
			if (sparklineGroup.MinAxisScaleType == SparklineAxisScaling.Custom)
				WriteDoubleValue("manualMin", sparklineGroup.MinAxisValue);
			if (sparklineGroup.LineWeightInPoints != defaultInfo.LineWeightInPoints)
				WriteDoubleValue("lineWeight", sparklineGroup.LineWeightInPoints);
			if (sparklineGroup.Type != defaultInfo.Type)
				WriteShStringValue("type", SparklineGroupTypeTable[sparklineGroup.Type]);
			if (sparklineGroup.UseDateAxis)
				WriteBoolValue("dateAxis", sparklineGroup.UseDateAxis);
			if (sparklineGroup.DisplayBlanksAs != defaultInfo.DisplayBlanksAs)
				WriteShStringValue("displayEmptyCellsAs", DisplayBlanksAsTable[sparklineGroup.DisplayBlanksAs]);
			if (sparklineGroup.ShowMarkers)
				WriteBoolValue("markers", sparklineGroup.ShowMarkers);
			if (sparklineGroup.ShowHighest)
				WriteBoolValue("high", sparklineGroup.ShowHighest);
			if (sparklineGroup.ShowLowest)
				WriteBoolValue("low", sparklineGroup.ShowLowest);
			if (sparklineGroup.ShowFirst)
				WriteBoolValue("first", sparklineGroup.ShowFirst);
			if (sparklineGroup.ShowLast)
				WriteBoolValue("last", sparklineGroup.ShowLast);
			if (sparklineGroup.ShowNegative)
				WriteBoolValue("negative", sparklineGroup.ShowNegative);
			if (sparklineGroup.ShowXAxis)
				WriteBoolValue("displayXAxis", sparklineGroup.ShowXAxis);
			if (sparklineGroup.ShowHidden)
				WriteBoolValue("displayHidden", sparklineGroup.ShowHidden);
			if (sparklineGroup.MinAxisScaleType != defaultInfo.MinAxisScaleType)
				WriteShStringValue("minAxisType", SparklineAxisScalingTable[sparklineGroup.MinAxisScaleType]);
			if (sparklineGroup.MaxAxisScaleType != defaultInfo.MaxAxisScaleType)
				WriteShStringValue("maxAxisType", SparklineAxisScalingTable[sparklineGroup.MaxAxisScaleType]);
			if (sparklineGroup.RightToLeft)
				WriteBoolValue("rightToLeft", sparklineGroup.RightToLeft);
		}
		void WriteSparklineColors(SparklineGroup sparklineGroup) {
			WriteSparklineColor(sparklineGroup, SparklineColorType.Series, "colorSeries");
			WriteSparklineColor(sparklineGroup, SparklineColorType.Negative, "colorNegative");
			WriteSparklineColor(sparklineGroup, SparklineColorType.Axis, "colorAxis");
			WriteSparklineColor(sparklineGroup, SparklineColorType.Markers, "colorMarkers");
			WriteSparklineColor(sparklineGroup, SparklineColorType.First, "colorFirst");
			WriteSparklineColor(sparklineGroup, SparklineColorType.Last, "colorLast");
			WriteSparklineColor(sparklineGroup, SparklineColorType.Highest, "colorHigh");
			WriteSparklineColor(sparklineGroup, SparklineColorType.Lowest, "colorLow");
		}
		void WriteSparklineColor(SparklineGroup sparklineGroup, SparklineColorType colorType, string tag) {
			WriteColor(sparklineGroup.GetColorInfo(colorType), tag, x14NamespaceReference);
		}
		string PrepareFormula(ParsedExpression expression) {
			return expression.BuildExpressionString(ActiveSheet.DataContext);
		}
		void GenerateSparklines(SparklineCollection sparklines) {
			WriteStartElement("sparklines", x14NamespaceReference);
			try {
				sparklines.ForEach(GenerateSparkline);
			}
			finally {
				WriteShEndElement();
			}
		}
		void GenerateSparkline(Sparkline sparkline) {
			WriteStartElement("sparkline", x14NamespaceReference);
			try {
				if (sparkline.SourceDataRange != null)
					WriteString("f", xmNamespaceReference, PrepareFormula(sparkline.Expression));
				WriteString("sqref", xmNamespaceReference, CellReferenceParser.ToString(sparkline.Position));
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
	}
}
