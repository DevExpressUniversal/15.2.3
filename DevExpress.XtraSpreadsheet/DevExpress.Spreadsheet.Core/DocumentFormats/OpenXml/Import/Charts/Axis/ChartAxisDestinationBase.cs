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
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region ChartAxisImportInfo
	public class ChartAxisImportInfo {
		public AxisBase Axis { get; set; }
		public int Id { get; set; }
		public int CrossesAxisId { get; set; }
	}
	#endregion
	#region ChartAxisDestinationBase
	public abstract class ChartAxisDestinationBase : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static
		internal static Dictionary<string, AxisPosition> AxisPositionTable = DictionaryUtils.CreateBackTranslationTable(DevExpress.XtraSpreadsheet.Export.OpenXml.OpenXmlExporter.AxisPositionTable);
		internal static Dictionary<string, AxisCrosses> AxisCrossesTable = DictionaryUtils.CreateBackTranslationTable(DevExpress.XtraSpreadsheet.Export.OpenXml.OpenXmlExporter.AxisCrossesTable);
		internal static Dictionary<string, TickMark> TickMarkTable = DictionaryUtils.CreateBackTranslationTable(DevExpress.XtraSpreadsheet.Export.OpenXml.OpenXmlExporter.TickMarkTable);
		internal static Dictionary<string, AxisOrientation> AxisOrientationTable = DictionaryUtils.CreateBackTranslationTable(DevExpress.XtraSpreadsheet.Export.OpenXml.OpenXmlExporter.AxisOrientationTable);
		internal static Dictionary<string, TickLabelPosition> TickLabelPositionTable = DictionaryUtils.CreateBackTranslationTable(DevExpress.XtraSpreadsheet.Export.OpenXml.OpenXmlExporter.TickLabelPositionTable);
		internal static Dictionary<string, LabelAlignment> LabelAlignmentTable = DictionaryUtils.CreateBackTranslationTable(DevExpress.XtraSpreadsheet.Export.OpenXml.OpenXmlExporter.LabelAlignmentTable);
		#endregion
		#region Handler Table
		protected static void AddAxisHandlers(ElementHandlerTable<SpreadsheetMLBaseImporter> table) {
			table.Add("axId", OnAxisId);
			table.Add("axPos", OnAxisPosition);
			table.Add("crossAx", OnCrossesAx);
			table.Add("crosses", OnCrosses);
			table.Add("delete", OnDelete);
			table.Add("majorGridlines", OnMajorGridLines);
			table.Add("majorTickMark", OnMajorTickMark);
			table.Add("minorGridlines", OnMinorGridLines);
			table.Add("minorTickMark", OnMinorTickMark);
			table.Add("numFmt", OnNumberFormat);
			table.Add("scaling", OnScaling);
			table.Add("tickLblPos", OnTickLabelPosition);
			table.Add("title", OnTitle);
			table.Add("spPr", OnShapeProperties);
			table.Add("txPr", OnTextProperties);
		}
		static ChartAxisDestinationBase GetThis(SpreadsheetMLBaseImporter importer) {
			return (ChartAxisDestinationBase)importer.PeekDestination();
		}
		#endregion
		readonly List<ChartAxisImportInfo> axisList;
		int axisId;
		int crossesAxisId;
		protected ChartAxisDestinationBase(SpreadsheetMLBaseImporter importer, List<ChartAxisImportInfo> axisList)
			: base(importer) {
			this.axisList = axisList;
		}
		protected abstract AxisBase Axis { get; }
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			axisList.Add(new ChartAxisImportInfo() { Axis = Axis, Id = axisId, CrossesAxisId = crossesAxisId });
		}
		#region Handlers
		static Destination OnDelete(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			AxisBase axis = GetThis(importer).Axis;
			return new OnOffValueDestination(importer,
				delegate(bool value) { axis.Delete = value; },
				"val", true);
		}
		static Destination OnAxisPosition(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			AxisBase axis = GetThis(importer).Axis;
			return new EnumValueDestination<AxisPosition>(importer,
				AxisPositionTable,
				delegate(AxisPosition value) { axis.Position = value; },
				"val",
				AxisPosition.Bottom);
		}
		static Destination OnCrosses(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			AxisBase axis = GetThis(importer).Axis;
			return new EnumValueDestination<AxisCrosses>(importer,
				AxisCrossesTable,
				delegate(AxisCrosses value) { axis.Crosses = value; },
				"val",
				AxisCrosses.AutoZero);
		}
		static Destination OnMajorGridLines(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			AxisBase axis = GetThis(importer).Axis;
			axis.ShowMajorGridlines = true;
			return new GridLinesDestination(importer, axis.MajorGridlines);
		}
		static Destination OnMinorGridLines(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			AxisBase axis = GetThis(importer).Axis;
			axis.ShowMinorGridlines = true;
			return new GridLinesDestination(importer, axis.MinorGridlines);
		}
		static Destination OnShapeProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			AxisBase axis = GetThis(importer).Axis;
			return new ChartShapePropertiesDestination(importer, axis.ShapeProperties);
		}
		static Destination OnMajorTickMark(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			AxisBase axis = GetThis(importer).Axis;
			return new EnumValueDestination<TickMark>(importer,
				TickMarkTable,
				delegate(TickMark value) { axis.MajorTickMark = value; },
				"val",
				TickMark.Cross);
		}
		static Destination OnMinorTickMark(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			AxisBase axis = GetThis(importer).Axis;
			return new EnumValueDestination<TickMark>(importer,
				TickMarkTable,
				delegate(TickMark value) { axis.MinorTickMark = value; },
				"val",
				TickMark.Cross);
		}
		static Destination OnNumberFormat(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			AxisBase axis = GetThis(importer).Axis;
			return new ChartNumberFormatDestination(importer, axis.NumberFormat);
		}
		static Destination OnScaling(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			AxisBase axis = GetThis(importer).Axis;
			return new ChartAxisScalingDestination(importer, axis.Scaling);
		}
		static Destination OnTickLabelPosition(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			AxisBase axis = GetThis(importer).Axis;
			return new EnumValueDestination<TickLabelPosition>(importer,
				TickLabelPositionTable,
				delegate(TickLabelPosition value) { axis.TickLabelPos = value; },
				"val",
				TickLabelPosition.NextTo);
		}
		static Destination OnTitle(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			AxisBase axis = GetThis(importer).Axis;
			return new ChartTitleDestination(importer, axis.Title);
		}
		static Destination OnAxisId(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ChartAxisDestinationBase thisDestination = GetThis(importer);
			return new IntegerValueDestination(importer,
				delegate(int value) { thisDestination.axisId = value; },
				"val",
				-1);
		}
		static Destination OnCrossesAx(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ChartAxisDestinationBase thisDestination = GetThis(importer);
			return new IntegerValueDestination(importer,
				delegate(int value) { thisDestination.crossesAxisId = value; },
				"val",
				-1);
		}
		static Destination OnTextProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			AxisBase axis = GetThis(importer).Axis;
			return new TextPropertiesDestination(importer, axis.TextProperties);
		}
		#endregion
	}
	#endregion
	#region GridLinesDestination
	public class GridLinesDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("spPr", OnShapeProperties);
			return result;
		}
		static GridLinesDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (GridLinesDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		ShapeProperties shapeProperties;
		#endregion
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		public GridLinesDestination(SpreadsheetMLBaseImporter importer, ShapeProperties shapeProperties)
			: base(importer) {
			this.shapeProperties = shapeProperties;
		}
		#region Handlers
		static Destination OnShapeProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ChartShapePropertiesDestination(importer, GetThis(importer).shapeProperties);
		}
		#endregion
	}
	#endregion
	#region ChartNumberFormatDestination
	public class ChartNumberFormatDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		#region Fields
		readonly NumberFormatOptions numberFormat;
		#endregion
		public ChartNumberFormatDestination(SpreadsheetMLBaseImporter importer, NumberFormatOptions numberFormat)
			: base(importer) {
			this.numberFormat = numberFormat;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			numberFormat.SetNumberFormatCodeCore(reader.GetAttribute("formatCode"));
			if (string.IsNullOrEmpty(numberFormat.NumberFormatCode))
				Importer.ThrowInvalidFile();
			numberFormat.SetSourceLinkedCore(Importer.GetWpSTOnOffValue(reader, "sourceLinked", true));
		}
	}
	#endregion
	#region ChartAxisScalingDestination
	public class ChartAxisScalingDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("logBase", OnLogarithmicBase);
			result.Add("orientation", OnOrientation);
			result.Add("max", OnMax);
			result.Add("min", OnMin);
			return result;
		}
		static ChartAxisScalingDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ChartAxisScalingDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		ScalingOptions scaling;
		#endregion
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		public ChartAxisScalingDestination(SpreadsheetMLBaseImporter importer, ScalingOptions scaling)
			: base(importer) {
			this.scaling = scaling;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			scaling.BeginUpdate();
		}
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			scaling.EndUpdate();
		}
		#region Handlers
		static Destination OnLogarithmicBase(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ScalingOptions scaling = GetThis(importer).scaling;
			scaling.LogScale = true;
			return new FloatValueDestination(importer,
				delegate(float value) { scaling.LogBase = value; },
				"val");
		}
		static Destination OnOrientation(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ScalingOptions scaling = GetThis(importer).scaling;
			return new EnumValueDestination<AxisOrientation>(importer,
				CategoryAxisDestination.AxisOrientationTable,
				delegate(AxisOrientation value) { scaling.Orientation = value; },
				"val",
				AxisOrientation.MinMax);
		}
		static Destination OnMin(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ScalingOptions scaling = GetThis(importer).scaling;
			scaling.FixedMin = true;
			return new FloatValueDestination(importer,
				delegate(float value) { scaling.Min = value; },
				"val");
		}
		static Destination OnMax(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ScalingOptions scaling = GetThis(importer).scaling;
			scaling.FixedMax = true;
			return new FloatValueDestination(importer,
				delegate(float value) { scaling.Max = value; },
				"val");
		}
		#endregion
	}
	#endregion
}
