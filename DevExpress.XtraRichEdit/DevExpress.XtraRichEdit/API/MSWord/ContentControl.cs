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
	#region ContentControl
	[GeneratedCode("Suppress FxCop check", "")]
	public interface ContentControl : IWordObject {
		Range Range { get; }
		bool LockContentControl { get; set; }
		bool LockContents { get; set; }
		WdContentControlType Type { get; set; }
		void Copy();
		void Cut();
		void Delete(bool DeleteContents);
		ContentControlListEntries DropdownListEntries { get; }
		string Title { get; set; }
		string DateDisplayFormat { get; set; }
		bool MultiLine { get; set; }
		ContentControl ParentContentControl { get; }
		bool Temporary { get; set; }
		string ID { get; }
		bool ShowingPlaceholderText { get; }
		WdContentControlDateStorageFormat DateStorageFormat { get; set; }
		WdBuildingBlockTypes BuildingBlockType { get; set; }
		string BuildingBlockCategory { get; set; }
		WdLanguageID DateDisplayLocale { get; set; }
		void Ungroup();
		object DefaultTextStyle { get; set; }
		WdCalendarType DateCalendarType { get; set; }
		string Tag { get; set; }
	}
	#endregion
	#region ContentControls
	[GeneratedCode("Suppress FxCop check", "")]
	public interface ContentControls : IWordObject, IEnumerable {
		int Count { get; }
		ContentControl this[object Index] { get; } 
		ContentControl Add(WdContentControlType Type, ref object Range);
	}
	#endregion
	#region ContentControlListEntry
	[GeneratedCode("Suppress FxCop check", "")]
	public interface ContentControlListEntry : IWordObject {
		string Text { get; set; }
		string Value { get; set; }
		int Index { get; set; }
		void Delete();
		void MoveUp();
		void MoveDown();
		void Select();
	}
	#endregion
	#region ContentControlListEntries
	[GeneratedCode("Suppress FxCop check", "")]
	public interface ContentControlListEntries : IWordObject, IEnumerable {
		int Count { get; }
		void Clear();
		ContentControlListEntry this[int Index] { get; }
		ContentControlListEntry Add(string Text, string Value, int Index);
	}
	#endregion
	#region WdContentControlType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdContentControlType {
		wdContentControlRichText,
		wdContentControlText,
		wdContentControlPicture,
		wdContentControlComboBox,
		wdContentControlDropdownList,
		wdContentControlBuildingBlockGallery,
		wdContentControlDate,
		wdContentControlGroup
	}
	#endregion
	#region WdBuildingBlockTypes
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdBuildingBlockTypes {
		wdTypeAutoText = 9,
		wdTypeBibliography = 0x22,
		wdTypeCoverPage = 2,
		wdTypeCustom1 = 0x1d,
		wdTypeCustom2 = 30,
		wdTypeCustom3 = 0x1f,
		wdTypeCustom4 = 0x20,
		wdTypeCustom5 = 0x21,
		wdTypeCustomAutoText = 0x17,
		wdTypeCustomBibliography = 0x23,
		wdTypeCustomCoverPage = 0x10,
		wdTypeCustomEquations = 0x11,
		wdTypeCustomFooters = 0x12,
		wdTypeCustomHeaders = 0x13,
		wdTypeCustomPageNumber = 20,
		wdTypeCustomPageNumberBottom = 0x1a,
		wdTypeCustomPageNumberPage = 0x1b,
		wdTypeCustomPageNumberTop = 0x19,
		wdTypeCustomQuickParts = 15,
		wdTypeCustomTableOfContents = 0x1c,
		wdTypeCustomTables = 0x15,
		wdTypeCustomTextBox = 0x18,
		wdTypeCustomWatermarks = 0x16,
		wdTypeEquations = 3,
		wdTypeFooters = 4,
		wdTypeHeaders = 5,
		wdTypePageNumber = 6,
		wdTypePageNumberBottom = 12,
		wdTypePageNumberPage = 13,
		wdTypePageNumberTop = 11,
		wdTypeQuickParts = 1,
		wdTypeTableOfContents = 14,
		wdTypeTables = 7,
		wdTypeTextBox = 10,
		wdTypeWatermarks = 8
	}
	#endregion
	#region WdCalendarType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdCalendarType {
		wdCalendarWestern,
		wdCalendarArabic,
		wdCalendarHebrew,
		wdCalendarChina,
		wdCalendarJapan,
		wdCalendarThai,
		wdCalendarKorean,
		wdCalendarSakaEra,
		wdCalendarTranslitEnglish,
		wdCalendarTranslitFrench
	}
	#endregion
	#region WdContentControlDateStorageFormat
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdContentControlDateStorageFormat {
		wdContentControlDateStorageText,
		wdContentControlDateStorageDate,
		wdContentControlDateStorageDateTime
	}
	#endregion
}
