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
using System.CodeDom.Compiler;
using System.Collections;
namespace DevExpress.XtraRichEdit.API.Word {
	#region Field
	[GeneratedCode("Suppress FxCop check", "")]
	public interface Field : IWordObject {
		Range Code { get; set; }
		WdFieldType Type { get; }
		bool Locked { get; set; }
		WdFieldKind Kind { get; }
		Range Result { get; set; }
		string Data { get; set; }
		Field Next { get; }
		Field Previous { get; }
		int Index { get; }
		bool ShowCodes { get; set; }
		LinkFormat LinkFormat { get; }
		OLEFormat OLEFormat { get; }
		InlineShape InlineShape { get; }
		void Select();
		bool Update();
		void Unlink();
		void UpdateSource();
		void DoClick();
		void Copy();
		void Cut();
		void Delete();
	}
	#endregion
	#region Fields
	[GeneratedCode("Suppress FxCop check", "")]
	public interface Fields : IWordObject, IEnumerable {
		int Count { get; }
		int Locked { get; set; }
		Field this[int Index] { get; }
		void ToggleShowCodes();
		int Update();
		void Unlink();
		void UpdateSource();
		Field Add(Range Range, ref object Type, ref object Text, ref object PreserveFormatting);
	}
	#endregion
	#region WdFieldKind
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdFieldKind {
		wdFieldKindNone,
		wdFieldKindHot,
		wdFieldKindWarm,
		wdFieldKindCold
	}
	#endregion
	#region WdFieldType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdFieldType {
		wdFieldAddin = 0x51,
		wdFieldAddressBlock = 0x5d,
		wdFieldAdvance = 0x54,
		wdFieldAsk = 0x26,
		wdFieldAuthor = 0x11,
		wdFieldAutoNum = 0x36,
		wdFieldAutoNumLegal = 0x35,
		wdFieldAutoNumOutline = 0x34,
		wdFieldAutoText = 0x4f,
		wdFieldAutoTextList = 0x59,
		wdFieldBarCode = 0x3f,
		wdFieldBibliography = 0x61,
		wdFieldBidiOutline = 0x5c,
		wdFieldCitation = 0x60,
		wdFieldComments = 0x13,
		wdFieldCompare = 80,
		wdFieldCreateDate = 0x15,
		wdFieldData = 40,
		wdFieldDatabase = 0x4e,
		wdFieldDate = 0x1f,
		wdFieldDDE = 0x2d,
		wdFieldDDEAuto = 0x2e,
		wdFieldDocProperty = 0x55,
		wdFieldDocVariable = 0x40,
		wdFieldEditTime = 0x19,
		wdFieldEmbed = 0x3a,
		wdFieldEmpty = -1,
		wdFieldExpression = 0x22,
		wdFieldFileName = 0x1d,
		wdFieldFileSize = 0x45,
		wdFieldFillIn = 0x27,
		wdFieldFootnoteRef = 5,
		wdFieldFormCheckBox = 0x47,
		wdFieldFormDropDown = 0x53,
		wdFieldFormTextInput = 70,
		wdFieldFormula = 0x31,
		wdFieldGlossary = 0x2f,
		wdFieldGoToButton = 50,
		wdFieldGreetingLine = 0x5e,
		wdFieldHTMLActiveX = 0x5b,
		wdFieldHyperlink = 0x58,
		wdFieldIf = 7,
		wdFieldImport = 0x37,
		wdFieldInclude = 0x24,
		wdFieldIncludePicture = 0x43,
		wdFieldIncludeText = 0x44,
		wdFieldIndex = 8,
		wdFieldIndexEntry = 4,
		wdFieldInfo = 14,
		wdFieldKeyWord = 0x12,
		wdFieldLastSavedBy = 20,
		wdFieldLink = 0x38,
		wdFieldListNum = 90,
		wdFieldMacroButton = 0x33,
		wdFieldMergeField = 0x3b,
		wdFieldMergeRec = 0x2c,
		wdFieldMergeSeq = 0x4b,
		wdFieldNext = 0x29,
		wdFieldNextIf = 0x2a,
		wdFieldNoteRef = 0x48,
		wdFieldNumChars = 0x1c,
		wdFieldNumPages = 0x1a,
		wdFieldNumWords = 0x1b,
		wdFieldOCX = 0x57,
		wdFieldPage = 0x21,
		wdFieldPageRef = 0x25,
		wdFieldPrint = 0x30,
		wdFieldPrintDate = 0x17,
		wdFieldPrivate = 0x4d,
		wdFieldQuote = 0x23,
		wdFieldRef = 3,
		wdFieldRefDoc = 11,
		wdFieldRevisionNum = 0x18,
		wdFieldSaveDate = 0x16,
		wdFieldSection = 0x41,
		wdFieldSectionPages = 0x42,
		wdFieldSequence = 12,
		wdFieldSet = 6,
		wdFieldShape = 0x5f,
		wdFieldSkipIf = 0x2b,
		wdFieldStyleRef = 10,
		wdFieldSubject = 0x10,
		wdFieldSubscriber = 0x52,
		wdFieldSymbol = 0x39,
		wdFieldTemplate = 30,
		wdFieldTime = 0x20,
		wdFieldTitle = 15,
		wdFieldTOA = 0x49,
		wdFieldTOAEntry = 0x4a,
		wdFieldTOC = 13,
		wdFieldTOCEntry = 9,
		wdFieldUserAddress = 0x3e,
		wdFieldUserInitials = 0x3d,
		wdFieldUserName = 60
	}
	#endregion
}
