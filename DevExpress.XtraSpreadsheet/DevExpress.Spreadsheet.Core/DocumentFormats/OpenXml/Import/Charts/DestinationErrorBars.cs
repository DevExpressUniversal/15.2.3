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
using System.Xml;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region ErrorBars >>>
	#region ErrorBarsDestination
	public class ErrorBarsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static
		internal static Dictionary<string, ErrorBarDirection> errorBarDirectionTable = DictionaryUtils.CreateBackTranslationTable(DevExpress.XtraSpreadsheet.Export.OpenXml.OpenXmlExporter.ErrorBarDirectionTable);
		internal static Dictionary<string, ErrorBarType> errorBarTypeTable = DictionaryUtils.CreateBackTranslationTable(DevExpress.XtraSpreadsheet.Export.OpenXml.OpenXmlExporter.ErrorBarTypeTable);
		internal static Dictionary<string, ErrorValueType> errorValueTypeTable = DictionaryUtils.CreateBackTranslationTable(DevExpress.XtraSpreadsheet.Export.OpenXml.OpenXmlExporter.ErrorValueTypeTable);
		#endregion
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("errDir", OnErrorBarDirection);
			result.Add("errBarType", OnErrorBarType);
			result.Add("errValType", OnErrorBarValueType);
			result.Add("noEndCap", OnNoEndCap);
			result.Add("plus", OnPlus);
			result.Add("minus", OnMinus);
			result.Add("val", OnErrorBarValue);
			result.Add("spPr", OnShapeProperties);
			return result;
		}
		static ErrorBarsDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ErrorBarsDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly ErrorBars errorBars;
		#endregion
		public ErrorBarsDestination(SpreadsheetMLBaseImporter importer, ErrorBars errorBars)
			: base(importer) {
			this.errorBars = errorBars;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			errorBars.BeginUpdate();
		}
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			errorBars.EndUpdate();
			if (errorBars.ValueType != ErrorValueType.Custom && 
				(errorBars.Plus.GetReferenceType() != DataReferenceType.None || 
				errorBars.Minus.GetReferenceType() != DataReferenceType.None))
				Importer.ThrowInvalidFile();
		}
		#region Handlers
		static Destination OnShapeProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ChartShapePropertiesDestination(importer, GetThis(importer).errorBars.ShapeProperties);
		}
		static Destination OnErrorBarDirection(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ErrorBars errorBars = GetThis(importer).errorBars;
			return new EnumValueDestination<ErrorBarDirection>(importer,
				errorBarDirectionTable,
				delegate(ErrorBarDirection value) { errorBars.BarDirection = value; },
				"val",
				ErrorBarDirection.X);
		}
		static Destination OnErrorBarType(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ErrorBars errorBars = GetThis(importer).errorBars;
			return new EnumValueDestination<ErrorBarType>(importer,
				errorBarTypeTable,
				delegate(ErrorBarType value) { errorBars.BarType = value; },
				"val",
				ErrorBarType.Both);
		}
		static Destination OnErrorBarValueType(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ErrorBars errorBars = GetThis(importer).errorBars;
			return new EnumValueDestination<ErrorValueType>(importer,
				errorValueTypeTable,
				delegate(ErrorValueType value) { errorBars.ValueType = value; },
				"val",
				ErrorValueType.FixedValue);
		}
		static Destination OnNoEndCap(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ErrorBars errorBars = GetThis(importer).errorBars;
			return new OnOffValueDestination(importer,
				delegate(bool value) { errorBars.NoEndCap = value; },
				"val", true);
		}
		static Destination OnPlus(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ErrorBars errorBars = GetThis(importer).errorBars;
			return new ErrorBarValueDestination(importer, delegate(IDataReference value) { errorBars.Plus = value; });
		}
		static Destination OnMinus(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ErrorBars errorBars = GetThis(importer).errorBars;
			return new ErrorBarValueDestination(importer, delegate(IDataReference value) { errorBars.Minus = value; });
		}
		static Destination OnErrorBarValue(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ErrorBars errorBars = GetThis(importer).errorBars;
			return new FloatValueDestination(importer, delegate(float value) { errorBars.SetValueCore(value); }, "val");
		}
		#endregion
	}
	#endregion
	#region ErrorBarValueDestination
	public class ErrorBarValueDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("numRef", OnNumberReference);
			result.Add("numLit", OnNumberLiteral);
			return result;
		}
		static ErrorBarValueDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ErrorBarValueDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly Action<IDataReference> setterMethod;
		IDataReference dataReference;
		IOpenXmlChartDataReference openXmlNumberReference;
		#endregion
		public ErrorBarValueDestination(SpreadsheetMLBaseImporter importer, Action<IDataReference> setterMethod)
			: base(importer) {
			this.setterMethod = setterMethod;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			if (openXmlNumberReference != null)
				dataReference = openXmlNumberReference.ToDataReference(Importer.DocumentModel, ChartViewSeriesDirection.Vertical, true); 
			if (dataReference == null)
				Importer.ThrowInvalidFile();
			setterMethod(dataReference);
		}
		#region Handlers
		static Destination OnNumberReference(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ErrorBarValueDestination thisDestination = GetThis(importer);
			if (thisDestination.dataReference != null)
				importer.ThrowInvalidFile();
			OpenXmlNumberReference numberReference = new OpenXmlNumberReference();
			thisDestination.openXmlNumberReference = numberReference;
			return new ChartNumberReferenceDestination(importer, numberReference);
		}
		static Destination OnNumberLiteral(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ErrorBarValueDestination thisDestination = GetThis(importer);
			if (thisDestination.dataReference != null)
				importer.ThrowInvalidFile();
			OpenXmlNumberLiteral numberLiteral = new OpenXmlNumberLiteral();
			thisDestination.openXmlNumberReference = numberLiteral;
			return new ChartNumberLiteralDestination(importer, numberLiteral);
		}
		#endregion
	}
	#endregion
	#endregion
}
