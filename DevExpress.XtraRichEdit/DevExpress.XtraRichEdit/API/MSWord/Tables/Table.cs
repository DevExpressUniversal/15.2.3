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
	#region Table
	[GeneratedCode("Suppress FxCop check", "")]
	public interface Table : IWordObject {
		Range Range { get; }
		Columns Columns { get; }
		Rows Rows { get; }
		Borders Borders { get; set; }
		Shading Shading { get; }
		bool Uniform { get; }
		int AutoFormatType { get; }
		void Select();
		void Delete();
		void SortOld(ref object ExcludeHeader, ref object FieldNumber, ref object SortFieldType, ref object SortOrder, ref object FieldNumber2, ref object SortFieldType2, ref object SortOrder2, ref object FieldNumber3, ref object SortFieldType3, ref object SortOrder3, ref object CaseSensitive, ref object LanguageID);
		void SortAscending();
		void SortDescending();
		void AutoFormat(ref object Format, ref object ApplyBorders, ref object ApplyShading, ref object ApplyFont, ref object ApplyColor, ref object ApplyHeadingRows, ref object ApplyLastRow, ref object ApplyFirstColumn, ref object ApplyLastColumn, ref object AutoFit);
		void UpdateAutoFormat();
		Range ConvertToTextOld(ref object Separator);
		Cell Cell(int Row, int Column);
		Table Split(ref object BeforeRow);
		Range ConvertToText(ref object Separator, ref object NestedTables);
		void AutoFitBehavior(WdAutoFitBehavior Behavior);
		void Sort(ref object ExcludeHeader, ref object FieldNumber, ref object SortFieldType, ref object SortOrder, ref object FieldNumber2, ref object SortFieldType2, ref object SortOrder2, ref object FieldNumber3, ref object SortFieldType3, ref object SortOrder3, ref object CaseSensitive, ref object BidiSort, ref object IgnoreThe, ref object IgnoreKashida, ref object IgnoreDiacritics, ref object IgnoreHe, ref object LanguageID);
		Tables Tables { get; }
		int NestingLevel { get; }
		bool AllowPageBreaks { get; set; }
		bool AllowAutoFit { get; set; }
		float PreferredWidth { get; set; }
		WdPreferredWidthType PreferredWidthType { get; set; }
		float TopPadding { get; set; }
		float BottomPadding { get; set; }
		float LeftPadding { get; set; }
		float RightPadding { get; set; }
		float Spacing { get; set; }
		WdTableDirection TableDirection { get; set; }
		string ID { get; set; }
		object Style { get; set; }
		bool ApplyStyleHeadingRows { get; set; }
		bool ApplyStyleLastRow { get; set; }
		bool ApplyStyleFirstColumn { get; set; }
		bool ApplyStyleLastColumn { get; set; }
		bool ApplyStyleRowBands { get; set; }
		bool ApplyStyleColumnBands { get; set; }
		void ApplyStyleDirectFormatting(string StyleName);
	}
	#endregion
	#region Tables
	[GeneratedCode("Suppress FxCop check", "")]
	public interface Tables : IWordObject, IEnumerable {
		int Count { get; }
		Table this[int Index] { get; }
		Table AddOld(Range Range, int NumRows, int NumColumns);
		Table Add(Range Range, int NumRows, int NumColumns, ref object DefaultTableBehavior, ref object AutoFitBehavior);
		int NestingLevel { get; }
	}
	#endregion
	#region WdTableDirection
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdTableDirection {
		wdTableDirectionRtl,
		wdTableDirectionLtr
	}
	#endregion
	#region WdAutoFitBehavior
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdAutoFitBehavior {
		wdAutoFitFixed,
		wdAutoFitContent,
		wdAutoFitWindow
	}
	#endregion
}
