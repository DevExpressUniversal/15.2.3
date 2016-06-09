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
using System.Xml;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region DateAxisDestination
	public class DateAxisDestination : ChartAxisDestinationBase {
		#region Static
		internal static Dictionary<string, TimeUnits> TimeUnitTable = DictionaryUtils.CreateBackTranslationTable(DevExpress.XtraSpreadsheet.Export.OpenXml.OpenXmlExporter.TimeUnitTable);
		#endregion
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("crossesAt", OnCrossesAt);
			result.Add("auto", OnAutoDateAxis);
			result.Add("lblOffset", OnLabelOffset);
			result.Add("baseTimeUnit", OnBaseTimeUnit);
			result.Add("majorUnit", OnMajorUnit);
			result.Add("minorUnit", OnMinorUnit);
			result.Add("majorTimeUnit", OnMajorTimeUnit);
			result.Add("minorTimeUnit", OnMinorTimeUnit);
			AddAxisHandlers(result);
			return result;
		}
		static DateAxisDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (DateAxisDestination)importer.PeekDestination();
		}
		#endregion
		readonly DateAxis axis;
		public DateAxisDestination(SpreadsheetMLBaseImporter importer, DateAxis axis, List<ChartAxisImportInfo> axisList)
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
		static Destination OnAutoDateAxis(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DateAxis axis = GetThis(importer).axis;
			return new OnOffValueDestination(importer,
				delegate(bool value) { axis.Auto = value; },
				"val", true);
		}
		static Destination OnLabelOffset(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DateAxis axis = GetThis(importer).axis;
			return new IntegerValueDestination(importer,
				delegate(int value) { axis.LabelOffset = value; },
				"val", 100);
		}
		static Destination OnCrossesAt(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DateAxis axis = GetThis(importer).axis;
			axis.Crosses = AxisCrosses.AtValue;
			return new FloatValueDestination(importer, delegate(float value) { axis.SetCrossesValueCore(value); }, "val");
		}
		static Destination OnMajorUnit(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DateAxis axis = GetThis(importer).axis;
			return new FloatValueDestination(importer, delegate(float value) { axis.SetMajorUnitCore(value); }, "val");
		}
		static Destination OnMinorUnit(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DateAxis axis = GetThis(importer).axis;
			return new FloatValueDestination(importer, delegate(float value) { axis.SetMinorUnitCore(value); }, "val");
		}
		static Destination OnBaseTimeUnit(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DateAxis axis = GetThis(importer).axis;
			return new EnumValueDestination<TimeUnits>(importer,
				TimeUnitTable,
				delegate(TimeUnits value) { axis.BaseTimeUnit = value; },
				"val",
				TimeUnits.Days);
		}
		static Destination OnMajorTimeUnit(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DateAxis axis = GetThis(importer).axis;
			return new EnumValueDestination<TimeUnits>(importer,
				TimeUnitTable,
				delegate(TimeUnits value) { axis.MajorTimeUnit = value; },
				"val",
				TimeUnits.Days);
		}
		static Destination OnMinorTimeUnit(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DateAxis axis = GetThis(importer).axis;
			return new EnumValueDestination<TimeUnits>(importer,
				TimeUnitTable,
				delegate(TimeUnits value) { axis.MinorTimeUnit = value; },
				"val",
				TimeUnits.Days);
		}
		#endregion
	}
	#endregion
}
