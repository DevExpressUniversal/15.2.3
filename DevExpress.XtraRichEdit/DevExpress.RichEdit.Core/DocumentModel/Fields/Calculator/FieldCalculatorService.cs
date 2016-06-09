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
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Native;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Fields.Expression;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Fields {
	public delegate CalculatedFieldBase CreateFieldDelegate();
	public class Token {
		readonly int kind;
		readonly DocumentLogPosition position;
		int length;
		string tokenValue;
		Token next;
		public Token()
			: this(TokenKind.Eof, new DocumentLogPosition(0), null) {
		}
		public Token(TokenKind kind, DocumentLogPosition position, string value, int length) {
			this.kind = (int)kind;
			this.position = position;
			this.length = length;
			this.tokenValue = value;
		}
		public Token(TokenKind kind, DocumentLogPosition position, string value)
			: this(kind, position, value, 1) {
		}
		public int Kind { get { return kind; } }
		public TokenKind ActualKind { get { return (TokenKind)Kind; } }
		public string Value { get { return tokenValue; } set { tokenValue = value; } }
		public DocumentLogPosition Position { get { return position; } }
		public int Length { get { return length; } set { length = value; } }
		public Token Next { get { return next; } set { next = value; } }
	}
	public enum TokenKind {
		Eof = 0,
		OpEQ,
		OpNEQ,
		OpLOW,
		OpLOWEQ,
		OpHI,
		OpHIEQ,
		OpPLUS,
		OpMINUS,
		OpMUL,
		OpDIV,
		OpPOW,
		OpenParenthesis,
		CloseParenthesis,
		Simple,
		DocPropertyInfoCommon,
		DocPropertyCategory,
		DocProperty,
		DocumentInformation,
		Eq,
		DateAndTimeFormattingSwitchBegin,
		GeneralFormattingSwitchBegin,
		NumbericFormattingSwitchBegin,
		CommonStringFormatSwitchBegin,
		Text,
		QuotedText,
		FieldSwitchCharacter,
		Constant,
		Percent,
		SeparatorChar,
		FunctionName,
		Template,
		Invalid = 0xFF
	}	
	public class FieldCalculatorService : IFieldCalculatorService {
		#region static
		static readonly Dictionary<string, CreateFieldDelegate> fieldTypes;
		static FieldCalculatorService() {
			fieldTypes = new Dictionary<string, CreateFieldDelegate>();
			RegisterFieldType(MergefieldField.FieldType, MergefieldField.Create);
			RegisterFieldType(DateField.FieldType, DateField.Create);
			RegisterFieldType(TimeField.FieldType, TimeField.Create);
			RegisterFieldType(CreatedateField.FieldType, CreatedateField.Create);
			RegisterFieldType(IncludepictureField.FieldType, IncludepictureField.Create);
			RegisterFieldType(IfField.FieldType, IfField.Create);
			RegisterFieldType(HyperlinkField.FieldType, HyperlinkField.Create);
			RegisterFieldType(PageField.FieldType, PageField.Create);
			RegisterFieldType(NumPagesField.FieldType, NumPagesField.Create);
			RegisterFieldType(TocField.FieldType, TocField.Create);
			RegisterFieldType(TocEntryField.FieldType, TocEntryField.Create);
			RegisterFieldType(PageRefField.FieldType, PageRefField.Create);
			RegisterFieldType(DocVariableField.FieldType, DocVariableField.Create);
			RegisterFieldType(SequenceField.FieldType, SequenceField.Create);
			RegisterFieldType(ShapeField.FieldType, ShapeField.Create);
			RegisterFieldType(SymbolField.FieldType, SymbolField.Create);
		}
		protected static void RegisterFieldType(string fieldType, CreateFieldDelegate creator) {
			if(!fieldTypes.ContainsKey(fieldType))
				fieldTypes.Add(fieldType, creator);
		}
		internal static CalculatedFieldBase CreateField(string fieldType) {
			CreateFieldDelegate creator;
			if (fieldTypes.TryGetValue(fieldType, out creator))
				return creator();
			else
				return null;
		}
		#endregion
		FieldUpdateOnLoadOptions updateOnLoadOptions;
		public FieldCalculatorService() {
		}
		public virtual void BeginUpdateFieldsOnLoad(FieldUpdateOnLoadOptions options) {
			this.updateOnLoadOptions = options;
		}
		public virtual void EndUpdateFieldsOnLoad(){
			this.updateOnLoadOptions = null;
		}
		#region IFieldCalculatorService Members
		public CalculateFieldResult CalculateField(PieceTable pieceTable, Field field, MailMergeDataMode mailMergeDataMode, UpdateFieldOperationType updateType) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			Guard.ArgumentNotNull(field, "field");
			DocumentFieldIterator iterator = new DocumentFieldIterator(pieceTable, field);
			FieldScanner scanner = new FieldScanner(iterator, pieceTable.DocumentModel.MaxFieldSwitchLength, pieceTable.DocumentModel.EnableFieldNames, pieceTable.SupportFieldCommonStringFormat);
			Token token = scanner.Scan();
			switch (token.ActualKind) {
				case TokenKind.OpEQ:
					return CalculateExpressionValue(pieceTable, scanner, field);
				case TokenKind.Eq:
					Debug.Assert(false);
					return CalculateInvalidField(pieceTable);
				default:
					return CalculateSimpleField(pieceTable, token, scanner, mailMergeDataMode, field, updateType);
			}
		}
		#endregion
		public void PrepareField(PieceTable pieceTable, Field field, UpdateFieldOperationType updateType) {
			DocumentFieldIterator iterator = new DocumentFieldIterator(pieceTable, field);
			FieldScanner scanner = new FieldScanner(iterator, pieceTable.DocumentModel.MaxFieldSwitchLength, pieceTable.DocumentModel.EnableFieldNames, pieceTable.SupportFieldCommonStringFormat);
			Token token = scanner.Scan();
			if(token.ActualKind != TokenKind.OpEQ && token.ActualKind != TokenKind.Eq)
				PrepareSimpleField(pieceTable, token, scanner, field, updateType);
		}
		protected virtual void PrepareSimpleField(PieceTable pieceTable, Token firstToken, FieldScanner scanner, Field documentField, UpdateFieldOperationType updateType) {
			try {
				CalculatedFieldBase field = CreateInitializedCalculatedField(pieceTable, firstToken, scanner);
				if (field == null || !field.CanPrepare)
					return;				
				if ((updateType & field.GetAllowedUpdateFieldTypes(updateOnLoadOptions)) == 0)
					return;
				field.BeforeCalculateFields(pieceTable, documentField);
				documentField.PreparedCalculatedField = field;
			}
			catch {
			}
		}
		protected virtual CalculatedFieldBase CreateInitializedCalculatedField(PieceTable pieceTable, Token firstToken, FieldScanner scanner) {
			CalculatedFieldBase field = CreateField(firstToken.Value);
			if (field == null)
				return null;
			InstructionCollection instructions = ParseInstructions(scanner, field);
			field.Initialize(pieceTable, instructions);
			return field;
		}
		public virtual CalculatedFieldBase ParseField(PieceTable pieceTable, Field field) {
			try {
				DocumentFieldIterator iterator = new DocumentFieldIterator(pieceTable, field);
				FieldScanner scanner = new FieldScanner(iterator, pieceTable.DocumentModel.MaxFieldSwitchLength, pieceTable.DocumentModel.EnableFieldNames, pieceTable.SupportFieldCommonStringFormat);
				Token token = scanner.Scan();
				if (token.ActualKind != TokenKind.OpEQ && token.ActualKind != TokenKind.Eq)
					return CreateInitializedCalculatedField(pieceTable, token, scanner);
			} catch {
			}
			return null;
		}
		protected virtual CalculateFieldResult CalculateSimpleField(PieceTable pieceTable, Token firstToken, FieldScanner scanner, MailMergeDataMode mailMergeDataMode, Field documentField, UpdateFieldOperationType updateType) {
			CalculatedFieldBase field;
			if (documentField.PreparedCalculatedField != null)
				field = documentField.PreparedCalculatedField;
			else
				field = CreateInitializedCalculatedField(pieceTable, firstToken, scanner);
			if (documentField.Locked && pieceTable.DocumentModel.FieldOptions.UpdateLockedFields != UpdateLockedFields.Always) {
				ShapeField shapeField = field as ShapeField;
				if (shapeField == null) {
					if (pieceTable.DocumentModel.FieldOptions.UpdateLockedFields == UpdateLockedFields.DocVariableOnly) {
						DocVariableField docVariableField = field as DocVariableField;
						if (docVariableField == null)
							return new CalculateFieldResult(new CalculatedFieldValue(null, FieldResultOptions.KeepOldResult), UpdateFieldOperationType.Normal);
					}
					else {
						return new CalculateFieldResult(new CalculatedFieldValue(null, FieldResultOptions.KeepOldResult), UpdateFieldOperationType.Normal);
					}
				}
			}
			documentField.PreparedCalculatedField = null;
			if (field == null)
				return null;
			if ((updateType & field.GetAllowedUpdateFieldTypes(updateOnLoadOptions)) == 0)
				return null;
			if (updateType == UpdateFieldOperationType.Copy && !pieceTable.DocumentModel.FieldOptions.UpdateFieldsOnPaste)
				return null;
			if (field is DocVariableField) {
				DocumentModel model = pieceTable.DocumentModel;
				bool updateBeforePrint = GetActualUpdateDocVariablesBeforePrint(model, updateType);
				if (!updateBeforePrint)
					return null;
				if (!model.FieldOptions.UpdateDocVariablesBeforeCopy && updateType == UpdateFieldOperationType.Copy)
					return null;
			}
			CalculatedFieldValue result = field.Update(pieceTable, mailMergeDataMode, documentField);
			return new CalculateFieldResult(result, field.GetAllowedUpdateFieldTypes(updateOnLoadOptions));
		}
		protected virtual bool GetActualUpdateDocVariablesBeforePrint(DocumentModel model, UpdateFieldOperationType updateType) {
			switch (model.DocumentProperties.UpdateDocVariablesBeforePrint) {
				case UpdateDocVariablesBeforePrint.Never:
					return false;
				case UpdateDocVariablesBeforePrint.Always:
					return true;
				default:
					if (!model.PrintingOptions.UpdateDocVariablesBeforePrint && updateType == UpdateFieldOperationType.CreateModelForExport)
						return false;
					if (!model.FieldOptions.UpdateDocVariablesBeforePrint && (updateType == UpdateFieldOperationType.CreateModelForExport || updateType == UpdateFieldOperationType.Copy))
						return false;
					return true;
			}
		}
		protected virtual CalculateFieldResult CalculateExpressionValue(PieceTable pieceTable, FieldScanner scanner, Field field) {
			RichEditFieldExpressionParser parser = new RichEditFieldExpressionParser(new ExpressionScanner(scanner));
			parser.Parse();
			ExpressionFieldBase richEditField = parser.GetResult();
			double calcResult = 0;
			if (richEditField.ExpressionTree.Root.BeginDataIteration(pieceTable, field)) {
				try {
					calcResult = richEditField.ExpressionTree.Root.GetValue();
				}
				finally {
					richEditField.ExpressionTree.Root.EndDataIteration();
				}
			}
			DocumentFieldIterator iterator = new DocumentFieldIterator(pieceTable, field);
			scanner = new FieldScanner(iterator, pieceTable.DocumentModel.MaxFieldSwitchLength, pieceTable.DocumentModel.EnableFieldNames, pieceTable.SupportFieldCommonStringFormat);
			ExpressionCalculatedField fakeField = new ExpressionCalculatedField(calcResult);
			InstructionCollection switches = ParseInstructions(scanner, fakeField);
			fakeField.Initialize(pieceTable, switches);
			return new CalculateFieldResult(fakeField.Update(pieceTable, MailMergeDataMode.None, field), UpdateFieldOperationType.Normal);
		}
		protected virtual CalculateFieldResult CalculateInvalidField(PieceTable pieceTable) {
			return new CalculateFieldResult(CalculatedFieldValue.Null, UpdateFieldOperationType.Normal);
		}
		public virtual Token GetFirstToken(PieceTable pieceTable, Field field) {
			try {
				DocumentFieldIterator iterator = new DocumentFieldIterator(pieceTable, field);
				FieldScanner scanner = new FieldScanner(iterator, pieceTable.DocumentModel.MaxFieldSwitchLength, pieceTable.DocumentModel.EnableFieldNames, pieceTable.SupportFieldCommonStringFormat);
				return scanner.Scan();
			} catch {
			}
			return null;
		}
		protected internal static InstructionCollection ParseInstructions(FieldScanner scanner, CalculatedFieldBase field) {
			InstructionCollection instructions = new InstructionCollection();
			for (; ; ) {
				Token token = scanner.Scan();
				if (token.ActualKind == TokenKind.Eof)
					break;
				if (IsFieldArgument(token.ActualKind))
					instructions.AddArgument(token);
				else {					
					if (IsSwitchWithArgument(field, token)) {
						if (field.IsSwitchArgumentField(token.Value)) {
							if (scanner.IsFieldStart()) {
								Token fieldArgument = scanner.ScanEntireFieldToken();
								DocumentLogInterval interval = new DocumentLogInterval(token.Position, fieldArgument.Position - token.Position);
								instructions.AddSwitch(token.Value, interval, fieldArgument);
							} else {
								Token nextToken = scanner.Peek();
								if (IsFieldArgument(nextToken.ActualKind))
									scanner.Scan();
							}
						} else {
							Token nextToken = scanner.Peek();
							if (IsFieldArgument(nextToken.ActualKind)) {
								Token fieldArgument = scanner.Scan();
								DocumentLogInterval interval = new DocumentLogInterval(token.Position, fieldArgument.Position - token.Position);
								instructions.AddSwitch(token.Value, interval, fieldArgument);
							}
							else if (field.CanUseSwitchWithoutArgument(token.Value)) {
								DocumentLogInterval interval = new DocumentLogInterval(token.Position, nextToken.Position - token.Position);
								instructions.AddSwitch(token.Value, interval);
							}
						}
					}
					else {
						Token nextToken = scanner.Peek();
						DocumentLogInterval interval = new DocumentLogInterval(token.Position, nextToken.Position - token.Position);
						instructions.AddSwitch(token.Value, interval);
					}
				}
			}
			return instructions;
		}
		static bool IsSwitchWithArgument(CalculatedFieldBase field, Token token) {
			TokenKind kind = token.ActualKind;
			return kind == TokenKind.NumbericFormattingSwitchBegin ||				
				kind == TokenKind.GeneralFormattingSwitchBegin ||
				kind == TokenKind.DateAndTimeFormattingSwitchBegin ||
				kind == TokenKind.CommonStringFormatSwitchBegin ||
				field.IsSwitchWithArgument(token.Value);
		}
		static bool IsFieldArgument(TokenKind tokenKind) {
			return tokenKind == TokenKind.Text || tokenKind == TokenKind.QuotedText;
		}
	}
}
