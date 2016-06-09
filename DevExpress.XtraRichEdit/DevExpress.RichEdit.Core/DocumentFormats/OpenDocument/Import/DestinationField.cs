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
using DevExpress.XtraRichEdit.Export.OpenDocument;
using System.Xml;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office;
using System.Globalization;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Import.OpenDocument {
	public static class FieldHandlers {
		#region Simple Field Names
		static string[] documentFieldNames = {"chapter", "file-name", "template-name", "author-name", "author-initials" };
		static string[] documentSenderFieldNames = { "sender-company", "sender-firstname", "sender-lastname", "sender-initials", "sender-street", "sender-country", "sender-postal-code", "sender-city", "sender-title", "sender-position", "sender-phone-private", "sender-phone-work", "sender-fax", "sender-email", "sender-state-or-province" };
		static string[] documentStatisticFieldNames = {"paragraph-count", "word-count", "character-count", "table-count", "image-count", "object-count" };
		static string[] documentInformationFieldNames = { "description", "initial-creator", "creator", "keywords", "printed-by", "editing-cycles", "subject", "title", "editing-duration" };
		static string[] referencesFieldNames = { "reference-ref", "bookmark-ref" };
		static string[] functionFieldNames = { "drop-down", "text-input", "execute-macro", "placeholder" };
		static string[] variableFieldNames = { "variable-get","page-variable-get", "user-field-get"};
		#endregion
		public static void AddFieldHandlers(ElementHandlerTable table) {
			AddDocumentFieldHandlers(table);
			AddDocumentInformationFieldHandlers(table);
			AddCrossReferenceFieldHandlers(table);
			AddFunctionFieldHandlers(table);
			AddVariableFieldHandlers(table);
			AddDataBaseFieldHandlers(table);
		}
		static void AddDocumentFieldHandlers(ElementHandlerTable table) {
			table.Add("date", OnDateTime);
			table.Add("time", OnDateTime);
			table.Add("page-number", OnPageNumberDestination);
			table.Add("page-count", OnPagesCountDestination);
			AddSimpleFieldsHandlers(table, documentFieldNames);
			AddSimpleFieldsHandlers(table, documentSenderFieldNames);
			AddSimpleFieldsHandlers(table, documentStatisticFieldNames);
		}
		static void AddDocumentInformationFieldHandlers(ElementHandlerTable table) {
			table.Add("creation-date", OnCreationDateTime);
			table.Add("creation-time", OnCreationDateTime);
			table.Add("print-date", OnDateTime);
			table.Add("print-time", OnDateTime);
			table.Add("modification-date", OnDateTime);
			table.Add("modification-time", OnDateTime);
			AddSimpleFieldsHandlers(table, documentInformationFieldNames);
		}
		static void AddCrossReferenceFieldHandlers(ElementHandlerTable table) {
			AddSimpleFieldsHandlers(table, referencesFieldNames);
		}
		static void AddFunctionFieldHandlers(ElementHandlerTable table) {
			table.Add("hidden-text", OnHiddenText);
			table.Add("conditional-text", OnConditionalText);
			AddSimpleFieldsHandlers(table, functionFieldNames);
		}
		static void AddVariableFieldHandlers(ElementHandlerTable table) {
			table.Add("variable-input", OnVariableSetter);
			table.Add("variable-set", OnVariableSetter);
			table.Add("expression", OnExpressionDestination);
			table.Add("sequence", OnSequenceDestination);
			AddSimpleFieldsHandlers(table, variableFieldNames);
		}
		static void AddDataBaseFieldHandlers(ElementHandlerTable table) {
			table.Add("database-display", OnDataBaseDisplay);
		}
		static void AddSimpleFieldsHandlers(ElementHandlerTable table, string[] fieldNames) {
			int count = fieldNames.Length;
			for (int i = 0; i < count; i++)
				table.Add(fieldNames[i], OnSimpleField);
		}
		static Destination OnDateTime(OpenDocumentTextImporter importer, XmlReader reader) {
			return new FieldDateDestination(importer);
		}
		static Destination OnCreationDateTime(OpenDocumentTextImporter importer, XmlReader reader) {
			return new FieldCreateionDateDestination(importer);
		}
		static Destination OnSimpleField(OpenDocumentTextImporter importer, XmlReader reader) {
			return new SimpleFieldDestination(importer);
		}
		static Destination OnHiddenText(OpenDocumentTextImporter importer, XmlReader reader) {
			return new FieldHiddenTextDestination(importer);
		}
		static Destination OnConditionalText(OpenDocumentTextImporter importer, XmlReader reader) {
			return new FieldConditionalTextDestination(importer);
		}
		static Destination OnVariableSetter(OpenDocumentTextImporter importer, XmlReader reader) {
			return new FieldVariableSetterDestination(importer);
		}
		static Destination OnExpressionDestination(OpenDocumentTextImporter importer, XmlReader reader) {
			return new FieldExpressionDestination(importer);
		}
		static Destination OnSequenceDestination(OpenDocumentTextImporter importer, XmlReader reader) {
			return new FieldSequenceDestination(importer);
		}
		static Destination OnDataBaseDisplay(OpenDocumentTextImporter importer, XmlReader reader) {
			return new FieldDatabaseDisplayDestination(importer);
		}
		static Destination OnPageNumberDestination(OpenDocumentTextImporter importer, XmlReader reader) {
			return new FieldPageNumberDestination(importer);
		}
		static Destination OnPagesCountDestination(OpenDocumentTextImporter importer, XmlReader reader) {
			return new FieldPagesCountDestination(importer);
		}
	}
	public class SimpleFieldDestination : ElementDestination {
		#region Fields
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		string value = String.Empty;
		string code = String.Empty;
		bool hidden = false;
		#endregion
		public SimpleFieldDestination(OpenDocumentTextImporter importer)
			: base(importer) {			
		}
		protected internal override ElementHandlerTable ElementHandlerTable {
			get { return handlerTable; }
		}
		internal bool Hidden { get { return hidden; } set { hidden = value; } }
		internal string Value { get { return value; } set { this.value = value; } }
		internal string Code { get { return code; } set { code = value; } }
		static ElementHandlerTable CreateElementHandlerTable() {
			return new ElementHandlerTable();
		}
		public override bool ProcessText(XmlReader reader) {
			string fieldValue = ReadFieldValue(reader);
			if (String.IsNullOrEmpty(this.code))
				this.code = fieldValue;
			return true;
		}
		protected internal virtual string ReadFieldValue(XmlReader reader) {
			string readerValue = reader.Value;
			if (readerValue == null)
				return String.Empty;
			this.value = Hidden ? String.Empty : readerValue;
			return readerValue;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Importer.FieldInfoStack.Push(new ImportFieldInfo(Importer.PieceTable));
		}
		public override void ProcessElementClose(XmlReader reader) {
			ImportFieldInfo fieldInfo = Importer.FieldInfoStack.Pop();
			ImportFieldHelper helper = new ImportFieldHelper(Importer.PieceTable);
			helper.ProcessFieldBegin(fieldInfo, Importer.InputPosition);
			InsertFieldText(Code);
			helper.ProcessFieldSeparator(fieldInfo, Importer.InputPosition);
			InsertFieldText(Value);
			helper.ProcessFieldEnd(fieldInfo, Importer.InputPosition);
			if (Importer.FieldInfoStack.Count > 0)
				fieldInfo.Field.Parent = Importer.FieldInfoStack.Peek().Field;
			if (Importer.FieldInfoStack.Count == 0) 
				Importer.DocumentModel.CheckIntegrity();
		}
		protected internal void InsertFieldText(string text) {
			if (String.IsNullOrEmpty(text))
				return;
			Importer.PieceTable.InsertTextCore(Importer.InputPosition, text);
		}
	}	
	public class FieldDateDestination : SimpleFieldDestination {
		public FieldDateDestination(OpenDocumentTextImporter importer)
			: base(importer) {
			Code = "DATE";
		}
	}
	public class FieldCreateionDateDestination : SimpleFieldDestination {
		public FieldCreateionDateDestination(OpenDocumentTextImporter importer)
			: base(importer) {
			Code = "CREATEDATE";
		}
	}
	public class FieldConditionalTextDestination : SimpleFieldDestination {
		public FieldConditionalTextDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string condition = GetCondition(reader);
			string trueValue = ImportHelper.GetTextStringAttribute(reader, "string-value-if-true");
			string falseValue = ImportHelper.GetTextStringAttribute(reader, "string-value-if-false");
			Code = String.Format("IF \"{0}\" \"{1}\" \"{2}\"", condition, trueValue, falseValue);
			base.ProcessElementOpen(reader);
		}
		internal string GetCondition(XmlReader reader) {
			string attributeValue = ImportHelper.GetTextStringAttribute(reader, "condition");
			int prefixEnd = attributeValue.IndexOf(':');
			return attributeValue.Substring(prefixEnd + 1);
		}
	}
	public class FieldHiddenTextDestination : SimpleFieldDestination {
		public FieldHiddenTextDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}		
		public override void ProcessElementOpen(XmlReader reader) {
			string hiddenValue = ImportHelper.GetTextStringAttribute(reader, "is-hidden").ToLower(CultureInfo.InvariantCulture);
			this.Hidden = hiddenValue == "true" ? false : true;	
			base.ProcessElementOpen(reader);
		}		
	}
	public class FieldVariableSetterDestination : SimpleFieldDestination {
		public FieldVariableSetterDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string display = ImportHelper.GetTextStringAttribute(reader, "display").ToLower(CultureInfo.InvariantCulture);
			this.Hidden = display == "none" ? true : false;
			base.ProcessElementOpen(reader);
		}
	}
	public class FieldDatabaseDisplayDestination : SimpleFieldDestination {
		public FieldDatabaseDisplayDestination(OpenDocumentTextImporter importer)
			: base(importer) {			
		}		
		public override void ProcessElementOpen(XmlReader reader) {
			string columnName = ImportHelper.GetTextStringAttribute(reader, "column-name");
			this.Code = String.Format("MERGEFIELD {0}", columnName);
			base.ProcessElementOpen(reader);
		}		
	}
	public class FieldExpressionDestination : SimpleFieldDestination {
		public FieldExpressionDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string code = GetCode(reader);
			Code = code;
			base.ProcessElementOpen(reader);
		}
		protected internal virtual string GetCode(XmlReader reader) {
			string formula = ImportHelper.GetTextStringAttribute(reader, "formula");
			int prefixEnd = formula.IndexOf(':');
			return formula.Substring(prefixEnd + 1);
		}
	}
	public class FieldSequenceDestination : SimpleFieldDestination {
		public FieldSequenceDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string code = GetCode(reader);
			Code = code;
			base.ProcessElementOpen(reader);
		}
		protected internal virtual string GetCode(XmlReader reader) {
			string formula = ImportHelper.GetTextStringAttribute(reader, "formula");
			int prefixEnd = formula.IndexOf(':');
			return formula.Substring(prefixEnd + 1);
		}
	}
	public class FieldPageNumberDestination : SimpleFieldDestination {
		public FieldPageNumberDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Code = "PAGE";
			base.ProcessElementOpen(reader);
		}
	}
	public class FieldPagesCountDestination : SimpleFieldDestination {
		public FieldPagesCountDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Code = "NUMPAGES";
			base.ProcessElementOpen(reader);
		}
	}
}
