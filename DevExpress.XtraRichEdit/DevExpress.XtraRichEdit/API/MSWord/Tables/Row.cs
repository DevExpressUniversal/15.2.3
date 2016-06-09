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
	#region Row
	[GeneratedCode("Suppress FxCop check", "")]
	public interface Row : IWordObject {
		Range Range { get; }
		int AllowBreakAcrossPages { get; set; }
		WdRowAlignment Alignment { get; set; }
		int HeadingFormat { get; set; }
		float SpaceBetweenColumns { get; set; }
		float Height { get; set; }
		WdRowHeightRule HeightRule { get; set; }
		float LeftIndent { get; set; }
		bool IsLast { get; }
		bool IsFirst { get; }
		int Index { get; }
		Cells Cells { get; }
		Borders Borders { get; set; }
		Shading Shading { get; }
		Row Next { get; }
		Row Previous { get; }
		void Select();
		void Delete();
		void SetLeftIndent(float LeftIndent, WdRulerStyle RulerStyle);
		void SetHeight(float RowHeight, WdRowHeightRule HeightRule);
		Range ConvertToTextOld(ref object Separator);
		Range ConvertToText(ref object Separator, ref object NestedTables);
		int NestingLevel { get; }
		string ID { get; set; }
	}
	#endregion
	#region Rows
	[GeneratedCode("Suppress FxCop check", "")]
	public interface Rows : IWordObject, IEnumerable {
		int Count { get; }
		int AllowBreakAcrossPages { get; set; }
		WdRowAlignment Alignment { get; set; }
		int HeadingFormat { get; set; }
		float SpaceBetweenColumns { get; set; }
		float Height { get; set; }
		WdRowHeightRule HeightRule { get; set; }
		float LeftIndent { get; set; }
		Row First { get; }
		Row Last { get; }
		Borders Borders { get; set; }
		Shading Shading { get; }
		Row this[int Index] { get; }
		Row Add(ref object BeforeRow);
		void Select();
		void Delete();
		void SetLeftIndent(float LeftIndent, WdRulerStyle RulerStyle);
		void SetHeight(float RowHeight, WdRowHeightRule HeightRule);
		Range ConvertToTextOld(ref object Separator);
		void DistributeHeight();
		Range ConvertToText(ref object Separator, ref object NestedTables);
		int WrapAroundText { get; set; }
		float DistanceTop { get; set; }
		float DistanceBottom { get; set; }
		float DistanceLeft { get; set; }
		float DistanceRight { get; set; }
		float HorizontalPosition { get; set; }
		float VerticalPosition { get; set; }
		WdRelativeHorizontalPosition RelativeHorizontalPosition { get; set; }
		WdRelativeVerticalPosition RelativeVerticalPosition { get; set; }
		int AllowOverlap { get; set; }
		int NestingLevel { get; }
		WdTableDirection TableDirection { get; set; }
	}
	#endregion
	#region WdRowAlignment
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdRowAlignment {
		wdAlignRowLeft,
		wdAlignRowCenter,
		wdAlignRowRight
	}
	#endregion
	#region WdRowHeightRule
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdRowHeightRule {
		wdRowHeightAuto,
		wdRowHeightAtLeast,
		wdRowHeightExactly
	}
	#endregion
}
