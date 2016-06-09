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
	#region ValueAxisDestination
	public class ValueAxisDestination : ChartAxisDestinationBase {
		#region Static
		internal static Dictionary<string, AxisCrossBetween> CrossBetweenTable = DictionaryUtils.CreateBackTranslationTable(DevExpress.XtraSpreadsheet.Export.OpenXml.OpenXmlExporter.CrossBetweenTable);
		#endregion
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("crossBetween", OnCrossBetween);
			result.Add("majorUnit", OnMajorUnit);
			result.Add("minorUnit", OnMinorUnit);
			result.Add("dispUnits", OnDisplayUnits);
			AddAxisHandlers(result);
			result.Add("crossesAt", OnCrossesAt);
			return result;
		}
		static ValueAxisDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ValueAxisDestination)importer.PeekDestination();
		}
		#endregion
		readonly ValueAxis axis;
		public ValueAxisDestination(SpreadsheetMLBaseImporter importer, ValueAxis axis, List<ChartAxisImportInfo> axisList)
			: base(importer, axisList) {
			this.axis = axis;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		protected override AxisBase Axis { get { return axis; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			axis.BeginUpdate();
		}
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			axis.EndUpdate();
		}
		#region Handlers
		static Destination OnCrossBetween(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ValueAxis axis = GetThis(importer).axis;
			return new EnumValueDestination<AxisCrossBetween>(importer,
				CrossBetweenTable,
				delegate(AxisCrossBetween value) { axis.CrossBetween = value; },
				"val",
				AxisCrossBetween.Between);
		}
		static Destination OnMajorUnit(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ValueAxis axis = GetThis(importer).axis;
			return new FloatValueDestination(importer, delegate(float value) { axis.SetMajorUnitCore(value); }, "val");
		}
		static Destination OnMinorUnit(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ValueAxis axis = GetThis(importer).axis;
			return new FloatValueDestination(importer, delegate(float value) { axis.SetMinorUnitCore(value); }, "val");
		}
		static Destination OnDisplayUnits(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ValueAxis axis = GetThis(importer).axis;
			return new AxisDisplayUnitsDestination(importer, axis.DisplayUnit);
		}
		static Destination OnCrossesAt(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ValueAxis axis = GetThis(importer).axis;
			axis.Crosses = AxisCrosses.AtValue;
			return new FloatValueDestination(importer, delegate(float value) { axis.SetCrossesValueCore(value); }, "val");
		}
		#endregion
	}
	#endregion
	#region AxisDisplayUnitsDestination
	public class AxisDisplayUnitsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static
		internal static Dictionary<string, DisplayUnitType> DisplayUnitTypeTable = DictionaryUtils.CreateBackTranslationTable(DevExpress.XtraSpreadsheet.Export.OpenXml.OpenXmlExporter.DisplayUnitTypeTable);
		#endregion
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("custUnit", OnCustomUnit);
			result.Add("builtInUnit", OnBuiltInUnit);
			result.Add("dispUnitsLbl", OnDisplayUnitsLabel);
			return result;
		}
		static AxisDisplayUnitsDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (AxisDisplayUnitsDestination)importer.PeekDestination();
		}
		#endregion
		readonly DisplayUnitOptions displayUnit;
		public AxisDisplayUnitsDestination(SpreadsheetMLBaseImporter importer, DisplayUnitOptions displayUnit)
			: base(importer) {
			this.displayUnit = displayUnit;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			displayUnit.BeginUpdate();
		}
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			displayUnit.EndUpdate();
			if (displayUnit.UnitType == DisplayUnitType.None)
				Importer.ThrowInvalidFile();
		}
		#region Handlers
		static Destination OnDisplayUnitsLabel(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DisplayUnitOptions options = GetThis(importer).displayUnit;
			options.ShowLabel = true;
			return new AxisDisplayUnitsLabelDestination(importer, options);
		}
		static Destination OnBuiltInUnit(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DisplayUnitOptions displayUnit = GetThis(importer).displayUnit;
			if (displayUnit.UnitType != DisplayUnitType.None)
				importer.ThrowInvalidFile();
			return new EnumValueDestination<DisplayUnitType>(importer,
				DisplayUnitTypeTable,
				delegate(DisplayUnitType value) { displayUnit.UnitType = value; },
				"val",
				DisplayUnitType.None);
		}
		static Destination OnCustomUnit(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DisplayUnitOptions displayUnit = GetThis(importer).displayUnit;
			if (displayUnit.UnitType != DisplayUnitType.None)
				importer.ThrowInvalidFile();
			displayUnit.UnitType = DisplayUnitType.Custom;
			return new FloatValueDestination(importer, delegate(float value) { displayUnit.CustomUnit = value; }, "val");
		}
		#endregion
	}
	#endregion
	#region AxisDisplayUnitsLabelDestination
	public class AxisDisplayUnitsLabelDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("layout", OnLayout);
			result.Add("tx", OnChartText);
			result.Add("spPr", OnShapeProperties);
			result.Add("txPr", OnTextProperties);
			return result;
		}
		static AxisDisplayUnitsLabelDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (AxisDisplayUnitsLabelDestination)importer.PeekDestination();
		}
		#endregion
		readonly DisplayUnitOptions displayUnit;
		public AxisDisplayUnitsLabelDestination(SpreadsheetMLBaseImporter importer, DisplayUnitOptions displayUnit)
			: base(importer) {
			this.displayUnit = displayUnit;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnLayout(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new LayoutDestination(importer, GetThis(importer).displayUnit.Layout);
		}
		static Destination OnChartText(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DisplayUnitOptions displayUnit = GetThis(importer).displayUnit;
			return new ChartTextDestination(importer, displayUnit.Parent, delegate(IChartText value) { displayUnit.SetTextCore(value); });
		}
		static Destination OnShapeProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ChartShapePropertiesDestination(importer, GetThis(importer).displayUnit.ShapeProperties);
		}
		static Destination OnTextProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DisplayUnitOptions displayUnit = GetThis(importer).displayUnit;
			return new TextPropertiesDestination(importer, displayUnit.TextProperties);
		}
		#endregion
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			if (this.displayUnit.Text.TextType == ChartTextType.None)
				this.displayUnit.Text = ChartText.Auto;
		}
	}
	#endregion
}
