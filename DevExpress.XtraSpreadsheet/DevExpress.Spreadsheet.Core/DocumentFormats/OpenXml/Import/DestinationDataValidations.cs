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
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using System;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region DataValidationsDestination
	public class DataValidationsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("dataValidation", OnDataValidation);
			return result;
		}
		#endregion
		public DataValidationsDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		static Destination OnDataValidation(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DataValidationDestination(importer);
		}
	}
	#endregion
	#region DataValidationDestinationBase
	public abstract class DataValidationDestinationBase : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		static Dictionary<DataValidationType, string> dataValidationTypeTable = OpenXmlExporter.DataValidationTypeTable;
		static Dictionary<DataValidationErrorStyle, string> dataValidationErrorStyleTable = OpenXmlExporter.DataValidationErrorStyleTable;
		static Dictionary<DataValidationImeMode, string> dataValidationImeModeTable = OpenXmlExporter.DataValidationImeModeTable;
		static Dictionary<DataValidationOperator, string> dataValidationOperatorTable = OpenXmlExporter.DataValidationOperatorTable;
		#endregion
		protected DataValidationDestinationBase(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		#region Properties
		public DataValidation Validation { get; private set; }
		#endregion
		protected void CreateDataValidation(CellRangeBase cellRange) {
			Validation = new DataValidation(cellRange, Importer.CurrentWorksheet);
		}
		protected void ReadDataValidationAttributes(XmlReader reader) {
			DataValidation validation = Validation;
			validation.BeginUpdate();
			try {
				validation.Type = Importer.GetWpEnumValue<DataValidationType>(reader, "type", dataValidationTypeTable, DataValidationType.None);
				validation.ErrorStyle = Importer.GetWpEnumValue<DataValidationErrorStyle>(reader, "errorStyle", dataValidationErrorStyleTable, DataValidationErrorStyle.Stop);
				validation.ImeMode = Importer.GetWpEnumValue<DataValidationImeMode>(reader, "imeMode", dataValidationImeModeTable, DataValidationImeMode.NoControl);
				validation.ValidationOperator = Importer.GetWpEnumValue<DataValidationOperator>(reader, "operator", dataValidationOperatorTable, DataValidationOperator.Between);
				validation.AllowBlank = Importer.GetWpSTOnOffValue(reader, "allowBlank", false);
				validation.SuppressDropDown = Importer.GetWpSTOnOffValue(reader, "showDropDown", false);
				validation.ShowInputMessage = Importer.GetWpSTOnOffValue(reader, "showInputMessage", false);
				validation.ShowErrorMessage = Importer.GetWpSTOnOffValue(reader, "showErrorMessage", false);
				validation.ErrorTitle = DecodeXmlChars(reader, "errorTitle");
				validation.Error = DecodeXmlChars(reader, "error");
				validation.PromptTitle = DecodeXmlChars(reader, "promptTitle");
				validation.Prompt = DecodeXmlChars(reader, "prompt");
			}
			finally {
				validation.EndUpdate();
			}
		}
		string DecodeXmlChars(XmlReader reader, string tag) {
			return Importer.DecodeXmlChars(Importer.ReadAttribute(reader, tag));
		}
		protected internal static string PrepareFormula(string value) {
			if (string.IsNullOrEmpty(value))
				return value;
			if (value.Length > 1 && value.StartsWith("\"", StringComparison.Ordinal) && value.EndsWith("\"", StringComparison.Ordinal))
				return value.Substring(1, value.Length - 2).Replace("\"\"", "\"");
			else
				return "=" + value;
		}
		public override void ProcessElementClose(XmlReader reader) {
			if(Validation != null)
				Importer.CurrentWorksheet.DataValidations.AddWithoutHistoryAndNotifications(Validation);
		}
	}
	#endregion
	#region DataValidationDestination
	public class DataValidationDestination : DataValidationDestinationBase {
		#region Static members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("formula1", OnFormula1);
			result.Add("formula2", OnFormula2);
			return result;
		}
		#endregion
		#region Fields
		string formula1;
		string formula2;
		#endregion
		public DataValidationDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		static DataValidationDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (DataValidationDestination)importer.PeekDestination();
		}
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			CellRangeBase cellRange = Importer.GetWpSTSqref(reader, "sqref", Importer.CurrentWorksheet);
			if(cellRange == null)
				Importer.ThrowInvalidFile("Data validation sqref is not specified");
			CreateDataValidation(cellRange);
			ReadDataValidationAttributes(reader);
		}
		public override void ProcessElementClose(XmlReader reader) {
			Validation.SetFormulas(PrepareFormula(formula1), PrepareFormula(formula2));
			base.ProcessElementClose(reader);
		}
		static Destination OnFormula1(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DataValidationDestination destination = GetThis(importer);
			return new StringValueTagDestination(importer, delegate(string value) { destination.formula1 = value; return true; });
		}
		static Destination OnFormula2(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DataValidationDestination destination = GetThis(importer);
			return new StringValueTagDestination(importer, delegate(string value) { destination.formula2 = value; return true; });
		}
	}
	#endregion
	#region ExtDataValidationsDestination
	public class ExtDataValidationsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("dataValidation", OnDataValidation);
			return result;
		}
		#endregion
		public ExtDataValidationsDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		static Destination OnDataValidation(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ExtDataValidationDestination(importer);
		}
	}
	#endregion
	#region ExtDataValidationDestination
	public class ExtDataValidationDestination : DataValidationDestinationBase {
		#region Static members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("formula1", OnFormula1);
			result.Add("formula2", OnFormula2);
			result.Add("sqref", OnSqRef);
			return result;
		}
		#endregion
		public ExtDataValidationDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		static ExtDataValidationDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ExtDataValidationDestination)importer.PeekDestination();
		}
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			CellRangeBase cellRange = CellRangeBase.CreateRangeBase(Importer.CurrentWorksheet, "A1");
			CreateDataValidation(cellRange);
			ReadDataValidationAttributes(reader);
		}
		static Destination OnFormula1(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ExtDataValidationDestination destination = GetThis(importer);
			return new ExtDataValidationFormulaDestination(importer, destination.Validation, false);
		}
		static Destination OnFormula2(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ExtDataValidationDestination destination = GetThis(importer);
			return new ExtDataValidationFormulaDestination(importer, destination.Validation, true);
		}
		static Destination OnSqRef(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ExtDataValidationDestination destination = GetThis(importer);
			return new StringValueTagDestination(importer, 
				delegate(string value) {
					DataValidation validation = destination.Validation;
					CellRangeBase cellRange = CellRangeBase.CreateRangeBase(destination.Importer.CurrentWorksheet, value, ' ');
					string formula1 = validation.Formula1;
					string formula2 = validation.Formula2;
					validation.SetRangeCore(cellRange);
					if (validation.Expression1 != null)
						validation.Formula1 = formula1;
					if (validation.Expression2 != null)
						validation.Formula2 = formula2;
					return true; 
				});
		}
	}
	#endregion
	#region ExtDataValidationFormulaDestination
	public class ExtDataValidationFormulaDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("f", OnFormulaBody);
			return result;
		}
		#endregion
		#region Fields
		DataValidation validation;
		bool isFormula2;
		#endregion
		public ExtDataValidationFormulaDestination(SpreadsheetMLBaseImporter importer, DataValidation validation, bool isFormula2)
			: base(importer) {
			this.validation = validation;
			this.isFormula2 = isFormula2;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		static ExtDataValidationFormulaDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ExtDataValidationFormulaDestination)importer.PeekDestination();
		}
		#endregion
		static Destination OnFormulaBody(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ExtDataValidationFormulaDestination destination = GetThis(importer);
			DataValidation validation = destination.validation;
			return new StringValueTagDestination(importer,
				delegate(string value) {
					if (!string.IsNullOrEmpty(value) && validation.Type != DataValidationType.None) {
						if (!destination.isFormula2)
							validation.Formula1 = DataValidationDestinationBase.PrepareFormula(value);
						else if (MustHaveFormula2(validation))
							validation.Formula2 = DataValidationDestinationBase.PrepareFormula(value);
					}
					return true;
				});
		}
		static bool MustHaveFormula2(DataValidation validation) {
			return validation.Type != DataValidationType.None && validation.Type != DataValidationType.List && validation.Type != DataValidationType.Custom &&
				(validation.ValidationOperator == DataValidationOperator.Between || validation.ValidationOperator == DataValidationOperator.NotBetween);
		}
	}
	#endregion
}
