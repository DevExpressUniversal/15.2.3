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
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Model {
	#region FieldType
	public enum FieldType {
		None,
		CreateDate,
		Date,
		EditTime,
		PrintDate,
		SaveDate,
		Time,
		Compare,
		DocVariable,
		GotoButton,
		If,
		MacroButton,
		Print,
		Author,
		Comments,
		DocProperty,
		FileName,
		FileSize,
		Info,
		Keywords,
		LastSavedBy,
		NumChars,
		NumPages,
		NumWords,
		Subject,
		Template,
		Title,
		Formula,
		Advance,
		Eq,
		Symbol,
		Index,
		RD,
		TA,
		TC,
		TOA,
		TOC,
		XE,
		AutoText,
		AutoTextList,
		Bibliography,
		Citation,
		Hyperlink,
		IncludePicture,
		IncludeText,
		Link,
		NoteRef,
		PageRef,
		Quote,
		Ref,
		StyleRef,
		AddressBlock,
		Ask,
		DataBase,
		FillIn,
		GreetingLine,
		MergeField,
		MergeRec,
		MergeSeq,
		Next,
		NextIf,
		Set,
		SkipIf,
		AutoNum,
		AutoNumLGL,
		AutoNumOut,
		BarCode,
		ListNum,
		Page,
		RevNum,
		Section,
		SectionPages,
		Seq,
		UserAddress,
		UserInitials,
		UserName,
		FormCheckbox,
		FormDropdown,
		FormText
	}
	#endregion
	public class RichEditFieldInfo {
		FieldType type = FieldType.None;
		string argument = String.Empty;
		string bookmarkName = String.Empty;
		readonly Dictionary<char, string> specificFormattingSwitches;
		readonly List<string> generalFormattingSwitches;
		public RichEditFieldInfo() {
			this.specificFormattingSwitches = new Dictionary<char, string>();
			this.generalFormattingSwitches = new List<string>();
		}
		public FieldType Type { get { return type; } set { type = value; } }
		public string Argument { get { return argument; } set { argument = value; } }
		public string BookmarkName { get { return bookmarkName; } set { bookmarkName = value; } }
		public Dictionary<char, string> SpecificFormattingSwitches { get { return specificFormattingSwitches; } }
		public List<string> GeneralFormattingSwitches { get { return generalFormattingSwitches; } }
	}
	public enum FieldReadingStates {
		Type,
		Argument,
		FormattingSwitch,
		GeneralSwitches,
		SpecificSwitches
	}
	#region RichEditFieldInfoCalculator
	public class RichEditFieldInfoCalculator {
		readonly PieceTable pieceTable;
		public RichEditFieldInfoCalculator(PieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
		}
		public RichEditFieldInfo Calculate(Field field) {
			string fieldCode = field.Code.GetText(this.pieceTable);
			RichEditFieldInfo result = new RichEditFieldInfo();
			if (String.IsNullOrEmpty(fieldCode))
				return result;
			int length = fieldCode.Length;
			for (int i = 0; i < length; i++) {
			}
			return null;
		}
		protected internal void ChangeState(FieldReadingStates state) {
		}
	}
	#endregion
	public abstract class FieldInfoReadingStateBase {
		readonly RichEditFieldInfoCalculator calculator;
		protected FieldInfoReadingStateBase(RichEditFieldInfoCalculator calculator) {
			Guard.ArgumentNotNull(calculator, "calculator");
			this.calculator = calculator;
		}
		public RichEditFieldInfoCalculator Calculator { get { return calculator; } }
		public abstract void ProcessChar(char ch, RichEditFieldInfo fieldInfo);
	}
	#region FieldTypeReadingState
	public class FieldTypeReadingState : FieldInfoReadingStateBase {
		static Dictionary<string, FieldType> FieldTypeTable = CreateFieldTypeTable();
		#region CreateFieldTypeTable
		static Dictionary<string, FieldType> CreateFieldTypeTable() {
			Dictionary<string, FieldType> result = new Dictionary<string, FieldType>();
			result.Add("ADDRESSBLOCK", FieldType.AddressBlock);
			result.Add("ADVANCE", FieldType.Advance);
			result.Add("ASK", FieldType.Ask);
			result.Add("AUTHOR", FieldType.Author);
			result.Add("AUTONUM", FieldType.AutoNum);
			result.Add("AUTONUMLGL", FieldType.AutoNumLGL);
			result.Add("AUTONUMOUT", FieldType.AutoNumOut);
			result.Add("AUTOTEXT", FieldType.AutoText);
			result.Add("AUTOTEXTLIST", FieldType.AutoTextList);
			result.Add("BARCODE", FieldType.BarCode);
			result.Add("BIBLIOGRAPHY", FieldType.Bibliography);
			result.Add("CITATION", FieldType.Citation);
			result.Add("COMMENTS", FieldType.Comments);
			result.Add("COMPARE", FieldType.Compare);
			result.Add("CREATEDATE", FieldType.CreateDate);
			result.Add("DATABASE", FieldType.DataBase);
			result.Add("DATE", FieldType.Date);
			result.Add("DOCPROPERTY", FieldType.DocProperty);
			result.Add("DOCVARIABLE", FieldType.DocVariable);
			result.Add("EDITTIME", FieldType.EditTime);
			result.Add("EQ", FieldType.Eq);
			result.Add("FILENAME", FieldType.FileName);
			result.Add("FILESIZE", FieldType.FileSize);
			result.Add("FILLIN", FieldType.FillIn);
			result.Add("FORMCHECKBOX", FieldType.FormCheckbox);
			result.Add("FORMDROPDOWN", FieldType.FormDropdown);
			result.Add("FORMTEXT", FieldType.FormText);
			result.Add("FORMULA", FieldType.Formula);
			result.Add("GOTOBUTTON", FieldType.GotoButton);
			result.Add("GREETINGLINE", FieldType.GreetingLine);
			result.Add("HYPERLINK", FieldType.Hyperlink);
			result.Add("IF", FieldType.If);
			result.Add("INCLUDEPICTIRE", FieldType.IncludePicture);
			result.Add("INCLUDETEXT", FieldType.IncludeText);
			result.Add("INDEX", FieldType.Index);
			result.Add("INFO", FieldType.Info);
			result.Add("KEYWORDS", FieldType.Keywords);
			result.Add("LASTSAVEDBY", FieldType.LastSavedBy);
			result.Add("LINK", FieldType.Link);
			result.Add("LISTNUM", FieldType.ListNum);
			result.Add("MACROBUTTON", FieldType.MacroButton);
			result.Add("MERGEFIELD", FieldType.MergeField);
			result.Add("MERGEREC", FieldType.MergeRec);
			result.Add("MERGESEQ", FieldType.MergeSeq);
			result.Add("NEXT", FieldType.Next);
			result.Add("NEXTIF", FieldType.NextIf);
			result.Add("NOTEREF", FieldType.NoteRef);
			result.Add("NUMCHARS", FieldType.NumChars);
			result.Add("NUMPAGES", FieldType.NumPages);
			result.Add("NUMWORDS", FieldType.NumWords);
			result.Add("PAGE", FieldType.Page);
			result.Add("PAGEREF", FieldType.PageRef);
			result.Add("PRINT", FieldType.Print);
			result.Add("PRINTDATE", FieldType.PrintDate);
			result.Add("QUOTE", FieldType.Quote);
			result.Add("RD", FieldType.RD);
			result.Add("REF", FieldType.Ref);
			result.Add("REVNUM", FieldType.RevNum);
			result.Add("SAVEDATE", FieldType.SaveDate);
			result.Add("SECTION", FieldType.Section);
			result.Add("SECTIONPAGES", FieldType.SectionPages);
			result.Add("SEQ", FieldType.Seq);
			result.Add("SET", FieldType.Set);
			result.Add("SKIPIF", FieldType.SkipIf);
			result.Add("STYLEREF", FieldType.StyleRef);
			result.Add("SUBJECT", FieldType.Subject);
			result.Add("SYMBOL", FieldType.Symbol);
			result.Add("TA", FieldType.TA);
			result.Add("TC", FieldType.TC);
			result.Add("TEMPLATE", FieldType.Template);
			result.Add("TIME", FieldType.Time);
			result.Add("TITLE", FieldType.Title);
			result.Add("TOA", FieldType.TOA);
			result.Add("TOC", FieldType.TOC);
			result.Add("USERADDRESS", FieldType.UserAddress);
			result.Add("USERINITIALS", FieldType.UserInitials);
			result.Add("USERNAME", FieldType.UserName);
			result.Add("XE", FieldType.XE);
			return result;
		}
		#endregion
		StringBuilder typeName;
		public FieldTypeReadingState(RichEditFieldInfoCalculator calculator)
			: base(calculator) {
			this.typeName = new StringBuilder();
		}
		public override void ProcessChar(char ch, RichEditFieldInfo fieldInfo) {
			if (Char.IsWhiteSpace(ch)) {
				string type = this.typeName.ToString().ToUpper(CultureInfo.InvariantCulture);
				if (FieldTypeTable.ContainsKey(type))
					fieldInfo.Type = FieldTypeTable[type];
				else
					fieldInfo.BookmarkName = type;
				Calculator.ChangeState(FieldReadingStates.Argument);
			}
			else
				this.typeName.Append(ch);
		}
	}
	#endregion
	public class FieldArgumentReadingState : FieldInfoReadingStateBase {
		bool endByDoubleQuotes;
		readonly StringBuilder argument;
		public FieldArgumentReadingState(RichEditFieldInfoCalculator calculator)
			: base(calculator) {
			this.argument = new StringBuilder();
		}
		public override void ProcessChar(char ch, RichEditFieldInfo fieldInfo) {
			if (ch == '"' && this.argument.Length == 0) {
				this.endByDoubleQuotes = true;
			}
			if (this.endByDoubleQuotes) {
			}
		}
	}
}
