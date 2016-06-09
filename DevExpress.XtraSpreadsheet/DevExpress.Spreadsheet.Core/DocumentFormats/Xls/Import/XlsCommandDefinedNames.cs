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
using System.Text;
using System.Runtime.InteropServices;
using DevExpress.Office;
using DevExpress.Office.Services;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsContentDefinedNameExt
	public class XlsContentDefinedNameExt : XlsContentDefinedName {
		public void Apply(XlsContentBuilder contentBuilder) {
			ParsedExpression parsedExpression = GetParsedExpression(contentBuilder);
			if(parsedExpression.Count > 0) {
				if(SheetIndex == 0) {
					CheckParsedExpression(contentBuilder, parsedExpression);
					string description = string.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Scope_DefinedName) + " \"{0}\" ({1})",
						Name, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Scope_Workbook));
					contentBuilder.RPNContext.PushCurrentSubject(description);
					try {
						ParsedExpression expression = XlsParsedThingConverter.ToModelExpression(parsedExpression, contentBuilder.RPNContext);
						ValidateParsedExpression(contentBuilder, expression);
						if(expression.Count == 0)
							expression.Add(new ParsedThingError(VariantValue.ErrorReference.ErrorValue));
						DefinedName definedName = new DefinedName(contentBuilder.DocumentModel, Name, expression, -1);
						contentBuilder.DocumentModel.DefinedNames.AddWithoutHistoryAndNotifications(definedName);
						AssignProperties(definedName);
					}
					finally {
						contentBuilder.RPNContext.PopCurrentSubject();
					}
				}
				else if (contentBuilder.BoundSheetTable.ContainsKey(SheetIndex - 1)) {
					int index = contentBuilder.BoundSheetTable[SheetIndex - 1]; ;
					Worksheet sheet = contentBuilder.DocumentModel.Sheets[index];
					if (!sheet.DefinedNames.Contains(Name)) {
						CheckParsedExpression(contentBuilder, parsedExpression);
						string description = string.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Scope_DefinedName) + " \"{0}\" ({1})", Name, sheet.Name);
						contentBuilder.RPNContext.PushCurrentSubject(description);
						try {
							ParsedExpression expression = XlsParsedThingConverter.ToModelExpression(parsedExpression, contentBuilder.RPNContext);
							ValidateParsedExpression(contentBuilder, expression);
							if (expression.Count == 0)
								expression.Add(new ParsedThingError(VariantValue.ErrorReference.ErrorValue));
							DefinedName definedName = new DefinedName(contentBuilder.DocumentModel, Name, expression, sheet.SheetId);
							sheet.DefinedNames.AddWithoutHistoryAndNotifications(definedName);
							AssignProperties(definedName);
						}
						finally {
							contentBuilder.RPNContext.PopCurrentSubject();
						}
					}
				}
				else {
					string message = string.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_DefinedNameHasInvalidScope), Name);
					contentBuilder.LogMessage(LogCategory.Warning, message);
				}
			}
		}
		public ParsedExpression GetParsedExpression(XlsContentBuilder contentBuilder) {
			ParsedExpression result;
			contentBuilder.RPNContext.WorkbookContext.PushDefinedNameProcessing(new DefinedName(contentBuilder.DocumentModel, "fakeDefinedName", "=", -1));
			contentBuilder.RPNContext.WorkbookContext.PushCurrentCell(0, 0);
			try {
				result = contentBuilder.RPNContext.BinaryToExpression(FormulaBytes, FormulaSize);
			}
			finally {
				contentBuilder.RPNContext.WorkbookContext.PopDefinedNameProcessing();
				contentBuilder.RPNContext.WorkbookContext.PopCurrentCell();
			}
			return result;
		}
		public void SetParsedExpression(ParsedExpression parsedExpression, IRPNContext context) {
			FormulaBytes = context.ExpressionToBinary(parsedExpression);
		}
		void CheckParsedExpression(XlsContentBuilder contentBuilder, ParsedExpression parsedExpression) {
			if(!parsedExpression.IsXlsNamedFormulaCompliant()) {
				string scopeName = GetScopeName(contentBuilder);
				string message = string.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_IncompliantNamedParsedFormula), Name, scopeName);
				contentBuilder.LogMessage(LogCategory.Warning, message);
				parsedExpression.Clear();
				parsedExpression.Add(new ParsedThingError(VariantValue.ErrorValueNotAvailable.ErrorValue));
			}
		}
		void ValidateParsedExpression(XlsContentBuilder contentBuilder, ParsedExpression expression) {
			if(expression.Count == 0)
				return;
			contentBuilder.RPNContext.WorkbookContext.PushDefinedNameProcessing(new DefinedName(contentBuilder.DocumentModel, "fakeDefinedName", "=", -1));
			contentBuilder.RPNContext.WorkbookContext.PushCurrentCell(0, 0);
			try {
				if(!expression.IsValidExpression(contentBuilder)) {
					string scopeName = GetScopeName(contentBuilder);
					string message = string.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_DefinedNameHasInvalidExpression), Name, scopeName);
					contentBuilder.LogMessage(LogCategory.Warning, message);
					expression.Clear();
				}
			}
			finally {
				contentBuilder.RPNContext.WorkbookContext.PopDefinedNameProcessing();
				contentBuilder.RPNContext.WorkbookContext.PopCurrentCell();
			}
		}
		public string GetScopeName(XlsContentBuilder contentBuilder) {
			if(SheetIndex == 0)
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Scope_Workbook);
			if(contentBuilder.BoundSheetTable.ContainsKey(SheetIndex - 1)) {
				int index = contentBuilder.BoundSheetTable[SheetIndex - 1]; ;
				Worksheet sheet = contentBuilder.DocumentModel.Sheets[index];
				return sheet.Name;
			}
			return string.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Scope_SheetIndex), SheetIndex);
		}
		void AssignProperties(DefinedName definedName) {
			definedName.IsHidden = IsHidden;
			definedName.Comment = Comment;
			definedName.IsXlmMacro = IsXlmMacro;
			definedName.IsVbaMacro = IsVbaMacro;
			definedName.IsMacro = IsMacro;
			definedName.FunctionGroupId = FunctionCategory;
		}
	}
	#endregion
	#region XlsCommandDefinedName
	public class XlsCommandDefinedName : XlsCommandContentBase {
		#region Fields
		XlsContentDefinedNameExt content = new XlsContentDefinedNameExt();
		#endregion
		#region Properties
		public XlsContentDefinedNameExt Content { get { return content; } }
		#endregion
		public void SetParsedExpression(ParsedExpression parsedExpression, IRPNContext context) {
			content.SetParsedExpression(parsedExpression, context);
		}
		protected override void ApplyWorkbookGlobalsContent(XlsContentBuilder contentBuilder) {
			string name = content.Name;
			if(name.StartsWith(WorksheetFunctionBase.FUTURE_FUNCTION_PREFIX)) {
				if(!contentBuilder.RPNContext.IsRegisteredDefinedName(name, XlsDefs.NoScope))
					contentBuilder.RPNContext.RegisterDefinedName(name, XlsDefs.NoScope);
			}
			else {
				name = content.Name.TrimEnd(new char[] { '\0' });
				name = XlsDefinedNameHelper.ReplaceInvalidChars(name);
				while(!WorkbookDataContext.IsIdent(name) || contentBuilder.RPNContext.IsRegisteredDefinedName(name, content.SheetIndex))
					name = "_" + name;
				if(name != content.Name) {
					string scopeName = content.GetScopeName(contentBuilder);
					string message = string.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_DefinedNameHasBeenChanged), 
						content.Name.Replace("\0", @"\0"), scopeName, name);
					contentBuilder.LogMessage(LogCategory.Info, message);
				}
				contentBuilder.RPNContext.RegisterDefinedName(name, content.SheetIndex);
				content.Name = name;
				contentBuilder.DefinedNameItems.Add(content);
			}
		}
		public override IXlsCommand GetInstance() {
			content = new XlsContentDefinedNameExt();
			return this;
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#endregion
	#region XlsCommandDefinedNameComment
	public class XlsCommandDefinedNameComment : XlsCommandBase {
		#region Fields
		const int fixedPartSize = 16;
		XLUnicodeStringNoCch name = new XLUnicodeStringNoCch();
		XLUnicodeStringNoCch comment = new XLUnicodeStringNoCch();
		#endregion
		#region Properties
		public string Name {
			get { return name.Value; }
			set { name.Value = value; }
		}
		public string Comment {
			get { return comment.Value; }
			set { comment.Value = value; }
		}
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			FutureRecordHeader.FromStream(reader);
			int nameCharCount = reader.ReadUInt16();
			int commentCharCount = reader.ReadUInt16();
			this.name = XLUnicodeStringNoCch.FromStream(reader, nameCharCount);
			this.comment = XLUnicodeStringNoCch.FromStream(reader, commentCharCount);
		}
		protected override void ApplyWorkbookGlobalsContent(XlsContentBuilder contentBuilder) {
			if(contentBuilder.DefinedNameItems.Count > 0) {
				XlsContentDefinedNameExt content = contentBuilder.DefinedNameItems[contentBuilder.DefinedNameItems.Count - 1];
				if(content.IsBuiltIn && (Name != content.InternalName)) 
					return;
				 if(StringExtensions.CompareInvariantCultureIgnoreCase(Name, content.InternalName) != 0)
					return;
				 content.Comment = Comment;
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
			FutureRecordHeader header = new FutureRecordHeader();
			header.RecordTypeId = XlsCommandFactory.GetTypeCodeByType(typeof(XlsCommandDefinedNameComment));
			header.Write(writer);
			writer.Write((ushort)this.name.Value.Length);
			writer.Write((ushort)this.comment.Value.Length);
			this.name.Write(writer);
			this.comment.Write(writer);
		}
		protected override short GetSize() {
			return (short)(fixedPartSize + name.Length + comment.Length);
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandDefinedNameComment();
		}
	}
	#endregion
}
