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
using System.Xml;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.Office.Model;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region SparklineGroupsDestination
	public class SparklineGroupsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static Members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("sparklineGroup", OnSparklineGroup);
			return result;
		}
		static Destination OnSparklineGroup(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new SparklineGroupDestination(importer);
		}
		#endregion
		public SparklineGroupsDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementClose(XmlReader reader) {
			if (Importer.CurrentWorksheet.SparklineGroups.Count < 1)
				Importer.ThrowInvalidFile("Zero sparkline groups in collection");
		}
	}
	#endregion
	#region SparklineGroupDestination
	public class SparklineGroupDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static Members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("colorSeries", OnColorSeries);
			result.Add("colorNegative", OnColorNegative);
			result.Add("colorAxis", OnColorAxis);
			result.Add("colorMarkers", OnColorMarkers);
			result.Add("colorFirst", OnColorFirst);
			result.Add("colorLast", OnColorLast);
			result.Add("colorHigh", OnColorHigh);
			result.Add("colorLow", OnColorLow);
			result.Add("f", OnFormula);
			result.Add("sparklines", OnSparklines);
			return result;
		}
		static SparklineGroupDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (SparklineGroupDestination)importer.PeekDestination();
		}
		static Destination OnColorSeries(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ColorDestination(importer, GetThis(importer).seriesColorInfo);
		}
		static Destination OnColorNegative(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ColorDestination(importer, GetThis(importer).negativeColorInfo);
		}
		static Destination OnColorAxis(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ColorDestination(importer, GetThis(importer).axisColorInfo);
		}
		static Destination OnColorMarkers(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ColorDestination(importer, GetThis(importer).markersColorInfo);
		}
		static Destination OnColorFirst(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ColorDestination(importer, GetThis(importer).firstColorInfo);
		}
		static Destination OnColorLast(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ColorDestination(importer, GetThis(importer).lastColorInfo);
		}
		static Destination OnColorHigh(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ColorDestination(importer, GetThis(importer).highColorInfo);
		}
		static Destination OnColorLow(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ColorDestination(importer, GetThis(importer).lowColorInfo);
		}
		static Destination OnFormula(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			SparklineGroupDestination destination = GetThis(importer);
			return new StringValueTagDestination(importer, delegate(string value) { destination.dataRangeFormula = value; return true; });
		}
		static Destination OnSparklines(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new SparklinesDestination(importer, GetThis(importer).sparklineGroup.Sparklines);
		}
		#endregion
		#region Fields
		readonly SparklineGroup sparklineGroup;
		readonly ColorModelInfo seriesColorInfo;
		readonly ColorModelInfo negativeColorInfo;
		readonly ColorModelInfo axisColorInfo;
		readonly ColorModelInfo markersColorInfo;
		readonly ColorModelInfo firstColorInfo;
		readonly ColorModelInfo lastColorInfo;
		readonly ColorModelInfo highColorInfo;
		readonly ColorModelInfo lowColorInfo;
		string dataRangeFormula;
		#endregion
		public SparklineGroupDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
			sparklineGroup = new SparklineGroup(importer.CurrentWorksheet);
			seriesColorInfo = new ColorModelInfo();
			negativeColorInfo = new ColorModelInfo();
			axisColorInfo = new ColorModelInfo();
			markersColorInfo = new ColorModelInfo();
			firstColorInfo = new ColorModelInfo();
			lastColorInfo = new ColorModelInfo();
			highColorInfo = new ColorModelInfo();
			lowColorInfo = new ColorModelInfo();
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		SparklineGroupInfo DefaultInfo { get { return Importer.CurrentWorksheet.Workbook.Cache.SparklineGroupInfoCache.DefaultItem; } }
		public override void ProcessElementOpen(XmlReader reader) {
			sparklineGroup.BeginUpdate();
			sparklineGroup.MaxAxisValue = Importer.GetWpDoubleValue(reader, "manualMax", DefaultInfo.MaxAxisValue);
			sparklineGroup.MinAxisValue = Importer.GetWpDoubleValue(reader, "manualMin", DefaultInfo.MinAxisValue);
			sparklineGroup.LineWeightInPoints = Importer.GetWpDoubleValue(reader, "lineWeight", DefaultInfo.LineWeightInPoints);
			sparklineGroup.Type = Importer.GetWpEnumValue(reader, "type", OpenXmlExporter.SparklineGroupTypeTable, DefaultInfo.Type);
			sparklineGroup.UseDateAxis = Importer.GetWpSTOnOffValue(reader, "dateAxis", DefaultInfo.UseDateAxis);
			sparklineGroup.DisplayBlanksAs = Importer.GetWpEnumValue(reader, "displayEmptyCellsAs", OpenXmlExporter.DisplayBlanksAsTable , DefaultInfo.DisplayBlanksAs);
			sparklineGroup.ShowMarkers = Importer.GetWpSTOnOffValue(reader, "markers", DefaultInfo.ShowMarkers);
			sparklineGroup.ShowHighest = Importer.GetWpSTOnOffValue(reader, "high", DefaultInfo.ShowHighest);
			sparklineGroup.ShowLowest = Importer.GetWpSTOnOffValue(reader, "low", DefaultInfo.ShowLowest);
			sparklineGroup.ShowFirst = Importer.GetWpSTOnOffValue(reader, "first", DefaultInfo.ShowFirst);
			sparklineGroup.ShowLast = Importer.GetWpSTOnOffValue(reader, "last", DefaultInfo.ShowLast);
			sparklineGroup.ShowNegative = Importer.GetWpSTOnOffValue(reader, "negative", DefaultInfo.ShowNegative);
			sparklineGroup.ShowXAxis = Importer.GetWpSTOnOffValue(reader, "displayXAxis", DefaultInfo.ShowXAxis);
			sparklineGroup.ShowHidden = Importer.GetWpSTOnOffValue(reader, "displayHidden", DefaultInfo.ShowHidden);
			sparklineGroup.MinAxisScaleType = Importer.GetWpEnumValue(reader, "minAxisType", OpenXmlExporter.SparklineAxisScalingTable, DefaultInfo.MinAxisScaleType);
			sparklineGroup.MaxAxisScaleType = Importer.GetWpEnumValue(reader, "maxAxisType", OpenXmlExporter.SparklineAxisScalingTable, DefaultInfo.MaxAxisScaleType);
			sparklineGroup.RightToLeft = Importer.GetWpSTOnOffValue(reader, "rightToLeft", DefaultInfo.RightToLeft);
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (sparklineGroup.Sparklines.Count < 1)
				Importer.ThrowInvalidFile("Zero sparklines in collection");
			SetColorIndexes();
			if (!String.IsNullOrEmpty(dataRangeFormula))
				if (!sparklineGroup.TrySetDataRange(dataRangeFormula))
					Importer.ThrowInvalidFile("Invalid data range formula");
			sparklineGroup.EndUpdate();
			Importer.CurrentWorksheet.SparklineGroups.AddCore(sparklineGroup);
		}
		void SetColorIndexes() {
			sparklineGroup.SetColorIndexCore(seriesColorInfo, SparklineColorType.Series);
			sparklineGroup.SetColorIndexCore(negativeColorInfo, SparklineColorType.Negative);
			sparklineGroup.SetColorIndexCore(axisColorInfo, SparklineColorType.Axis);
			sparklineGroup.SetColorIndexCore(markersColorInfo, SparklineColorType.Markers);
			sparklineGroup.SetColorIndexCore(firstColorInfo, SparklineColorType.First);
			sparklineGroup.SetColorIndexCore(lastColorInfo, SparklineColorType.Last);
			sparklineGroup.SetColorIndexCore(highColorInfo, SparklineColorType.Highest);
			sparklineGroup.SetColorIndexCore(lowColorInfo, SparklineColorType.Lowest);
		}
	}
	#endregion
	#region SparklinesDestination
	public class SparklinesDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static Members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("sparkline", OnSparkline);
			return result;
		}
		static SparklinesDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (SparklinesDestination)importer.PeekDestination();
		}
		static Destination OnSparkline(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new SparklineDestination(importer, GetThis(importer).sparklines);
		}
		#endregion
		SparklineCollection sparklines;
		public SparklinesDestination(SpreadsheetMLBaseImporter importer, SparklineCollection sparklines)
			: base(importer) {
			this.sparklines = sparklines;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region SparklinesDestination
	public class SparklineDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static Members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("f", OnFormula);
			result.Add("sqref", OnSqRef);
			return result;
		}
		static SparklineDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (SparklineDestination)importer.PeekDestination();
		}
		static Destination OnFormula(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			SparklineDestination destination = GetThis(importer);
			return new StringValueTagDestination(importer, delegate(string value) { destination.dataRangeFormula = value; return true; });
		}
		static Destination OnSqRef(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			SparklineDestination destination = GetThis(importer);
			return new StringValueTagDestination(importer, delegate(string value) { destination.position = value; return true; });
		}
		#endregion
		#region Fields
		SparklineCollection sparklines;
		string dataRangeFormula;
		string position;
		#endregion
		public SparklineDestination(SpreadsheetMLBaseImporter importer, SparklineCollection sparklines)
			: base(importer) {
			this.sparklines = sparklines;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementClose(XmlReader reader) {
			Sparkline sparkline = new Sparkline(Importer.CurrentWorksheet);
			if (!String.IsNullOrEmpty(dataRangeFormula))
				if (!sparkline.TrySetDataRange(dataRangeFormula))
					Importer.ThrowInvalidFile("Invalid data range formula");
			if (String.IsNullOrEmpty(position))
				Importer.ThrowInvalidFile("Sparkline position is missing");
			CellPosition cellPosition = CellPosition.TryCreate(position);
			if (!cellPosition.IsValid)
				Importer.ThrowInvalidFile("Invalid cell position");
			sparkline.Position = cellPosition;
			sparklines.AddCore(sparkline);
		}
	}
	#endregion
}
