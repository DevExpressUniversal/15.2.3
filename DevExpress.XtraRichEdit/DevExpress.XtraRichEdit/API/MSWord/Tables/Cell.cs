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
	#region Cell
	[GeneratedCode("Suppress FxCop check", "")]
	public interface Cell : IWordObject {
		Range Range { get; }
		int RowIndex { get; }
		int ColumnIndex { get; }
		float Width { get; set; }
		float Height { get; set; }
		WdRowHeightRule HeightRule { get; set; }
		WdCellVerticalAlignment VerticalAlignment { get; set; }
		Column Column { get; }
		Row Row { get; }
		Cell Next { get; }
		Cell Previous { get; }
		Shading Shading { get; }
		Borders Borders { get; set; }
		void Select();
		void Delete(ref object ShiftCells);
		void Formula(ref object Formula, ref object NumFormat);
		void SetWidth(float ColumnWidth, WdRulerStyle RulerStyle);
		void SetHeight(ref object RowHeight, WdRowHeightRule HeightRule);
		void Merge(Cell MergeTo);
		void Split(ref object NumRows, ref object NumColumns);
		void AutoSum();
		Tables Tables { get; }
		int NestingLevel { get; }
		bool WordWrap { get; set; }
		float PreferredWidth { get; set; }
		bool FitText { get; set; }
		float TopPadding { get; set; }
		float BottomPadding { get; set; }
		float LeftPadding { get; set; }
		float RightPadding { get; set; }
		string ID { get; set; }
		WdPreferredWidthType PreferredWidthType { get; set; }
	}
	#endregion
	#region Cells
	[GeneratedCode("Suppress FxCop check", "")]
	public interface Cells : IWordObject, IEnumerable {
		int Count { get; }
		float Width { get; set; }
		float Height { get; set; }
		WdRowHeightRule HeightRule { get; set; }
		WdCellVerticalAlignment VerticalAlignment { get; set; }
		Borders Borders { get; set; }
		Shading Shading { get; }
		Cell this[int Index] { get; }
		Cell Add(ref object BeforeCell);
		void Delete(ref object ShiftCells);
		void SetWidth(float ColumnWidth, WdRulerStyle RulerStyle);
		void SetHeight(ref object RowHeight, WdRowHeightRule HeightRule);
		void Merge();
		void Split(ref object NumRows, ref object NumColumns, ref object MergeBeforeSplit);
		void DistributeHeight();
		void DistributeWidth();
		void AutoFit();
		int NestingLevel { get; }
		float PreferredWidth { get; set; }
		WdPreferredWidthType PreferredWidthType { get; set; }
	}
	#endregion
	#region WdCellVerticalAlignment
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdCellVerticalAlignment {
		wdCellAlignVerticalBottom = 3,
		wdCellAlignVerticalCenter = 1,
		wdCellAlignVerticalTop = 0
	}
	#endregion
}
