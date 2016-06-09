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

using DevExpress.Export.Xl;
using System;
namespace DevExpress.XtraSpreadsheet.Model {
	#region Cell : ICellAlignmentInfo
	partial class Cell {
		public ICellAlignmentInfo Alignment { get { return this; } }
		public IActualCellAlignmentInfo ActualAlignment { get { return this; } } 
		public virtual IActualCellAlignmentInfo InnerActualAlignment { get { return FormatInfo.ActualAlignment; } }
		#region ICellAlignmentInfo Members
		#region ICellAlignmentInfo.WrapText
		bool ICellAlignmentInfo.WrapText {
			get { return FormatInfo.Alignment.WrapText; }
			set {
				SetAlignmentPropertyValue(SetAlignmentWrapText, value);
				sheet.WebRanges.ChangeRange(sheet.Rows[RowIndex].GetCellIntervalRange());
			}
		}
		DocumentModelChangeActions SetAlignmentWrapText(FormatBase info, bool value) {
			info.Alignment.WrapText = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.JustifyLastLine
		bool ICellAlignmentInfo.JustifyLastLine {
			get { return FormatInfo.Alignment.JustifyLastLine; }
			set {
				SetAlignmentPropertyValue(SetAlignmentJustifyLastLine, value);
			}
		}
		DocumentModelChangeActions SetAlignmentJustifyLastLine(FormatBase info, bool value) {
			info.Alignment.JustifyLastLine = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.ShrinkToFit
		bool ICellAlignmentInfo.ShrinkToFit {
			get { return FormatInfo.Alignment.ShrinkToFit; }
			set {
				SetAlignmentPropertyValue(SetAlignmentShrinkToFit, value);
			}
		}
		DocumentModelChangeActions SetAlignmentShrinkToFit(FormatBase info, bool value) {
			info.Alignment.ShrinkToFit = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.TextRotation
		int ICellAlignmentInfo.TextRotation {
			get { return FormatInfo.Alignment.TextRotation; }
			set {
				SetAlignmentPropertyValue(SetAlignmentTextRotation, value);
			}
		}
		DocumentModelChangeActions SetAlignmentTextRotation(FormatBase info, int value) {
			info.Alignment.TextRotation = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.Indent
		byte ICellAlignmentInfo.Indent {
			get { return FormatInfo.Alignment.Indent; }
			set {
				SetAlignmentPropertyValue(SetAlignmentIndent, value);
			}
		}
		DocumentModelChangeActions SetAlignmentIndent(FormatBase info, byte value) {
			info.Alignment.Indent = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.RelativeIndent
		int ICellAlignmentInfo.RelativeIndent {
			get { return FormatInfo.Alignment.RelativeIndent; }
			set {
				SetAlignmentPropertyValue(SetAlignmentRelativeIndent, value);
			}
		}
		DocumentModelChangeActions SetAlignmentRelativeIndent(FormatBase info, int value) {
			info.Alignment.RelativeIndent = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.Horizontal
		XlHorizontalAlignment ICellAlignmentInfo.Horizontal {
			get { return FormatInfo.Alignment.Horizontal; }
			set {
				SetAlignmentPropertyValue(SetAlignmentHorizontal, value);
			}
		}
		DocumentModelChangeActions SetAlignmentHorizontal(FormatBase info, XlHorizontalAlignment value) {
			info.Alignment.Horizontal = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.Vertical
		XlVerticalAlignment ICellAlignmentInfo.Vertical {
			get { return FormatInfo.Alignment.Vertical; }
			set {
				SetAlignmentPropertyValue(SetAlignmentVertical, value);
			}
		}
		DocumentModelChangeActions SetAlignmentVertical(FormatBase info, XlVerticalAlignment value) {
			info.Alignment.Vertical = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.XlReadingOrder
		XlReadingOrder ICellAlignmentInfo.ReadingOrder {
			get { return FormatInfo.Alignment.ReadingOrder; }
			set {
				SetAlignmentPropertyValue(SetAlignmentReadingOrder, value);
			}
		}
		DocumentModelChangeActions SetAlignmentReadingOrder(FormatBase info, XlReadingOrder value) {
			info.Alignment.ReadingOrder = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		protected internal virtual void SetAlignmentPropertyValue<U>(SetPropertyValueDelegate<U> setter, U newValue) {
			DocumentModel.BeginUpdate();
			try {
				FormatBase info = GetInfoForModification();
				info.SetActualAlignment(ActualAlignment);
				DocumentModelChangeActions changeActions = setter(info, newValue);
				ReplaceInfo(info, changeActions);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		#endregion
		#region IActualCellAlignmentInfo Members
		bool IActualCellAlignmentInfo.WrapText { get { return GetActualAlignmentWrapText(); } }
		bool IActualCellAlignmentInfo.JustifyLastLine { get { return GetActualAlignmentJustifyLastLine(); } }
		bool IActualCellAlignmentInfo.ShrinkToFit { get { return GetActualAlignmentShrinkToFit(); } }
		int IActualCellAlignmentInfo.TextRotation { get { return GetActualAlignmentTextRotation(); } }
		byte IActualCellAlignmentInfo.Indent { get { return GetActualAlignmentIndent(); } }
		int IActualCellAlignmentInfo.RelativeIndent { get { return GetActualAlignmentRelativeIndent(); } }
		XlHorizontalAlignment IActualCellAlignmentInfo.Horizontal { get { return GetActualAlignmentHorizontal(); } }
		XlVerticalAlignment IActualCellAlignmentInfo.Vertical { get { return GetActualAlignmentVertical(); } }
		XlReadingOrder IActualCellAlignmentInfo.ReadingOrder { get { return GetActualAlignmentReadingOrder(); } }
		public XlHorizontalAlignment ActualHorizontalAlignment {
			get {
				if (HasFormula && Worksheet.ActiveView.ShowFormulas)
					return XlHorizontalAlignment.Left;
				return GetActualHorizontalAlignment(Value, ActualAlignment.Horizontal, ActualFormat.IsText);
			}
		}
		#endregion
		#region GetActualAlignmentValue
		protected T GetActualAlignmentValue<T>(T cellFormatActualValue, DifferentialFormatPropertyDescriptor propertyDescriptor) {
			return GetActualFormatValue(cellFormatActualValue, ActualApplyInfo.ApplyAlignment, propertyDescriptor);
		}
		protected virtual bool GetActualAlignmentWrapText() {
			return GetActualAlignmentValue(InnerActualAlignment.WrapText, DifferentialFormatPropertyDescriptor.AlignmentWrapText);
		}
		protected virtual bool GetActualAlignmentJustifyLastLine() {
			return GetActualAlignmentValue(InnerActualAlignment.JustifyLastLine, DifferentialFormatPropertyDescriptor.AlignmentJustifyLastLine);
		}
		protected virtual bool GetActualAlignmentShrinkToFit() {
			return GetActualAlignmentValue(InnerActualAlignment.ShrinkToFit, DifferentialFormatPropertyDescriptor.AlignmentShrinkToFit);
		}
		protected virtual int GetActualAlignmentTextRotation() {
			return GetActualAlignmentValue(InnerActualAlignment.TextRotation, DifferentialFormatPropertyDescriptor.AlignmentTextRotation);
		}
		protected virtual byte GetActualAlignmentIndent() {
			return GetActualAlignmentValue(InnerActualAlignment.Indent, DifferentialFormatPropertyDescriptor.AlignmentIndent);
		}
		protected virtual int GetActualAlignmentRelativeIndent() {
			return GetActualAlignmentValue(InnerActualAlignment.RelativeIndent, DifferentialFormatPropertyDescriptor.AlignmentRelativeIndent);
		}
		protected virtual XlHorizontalAlignment GetActualAlignmentHorizontal() {
			return GetActualAlignmentValue(InnerActualAlignment.Horizontal, DifferentialFormatPropertyDescriptor.AlignmentHorizontal);
		}
		protected virtual XlVerticalAlignment GetActualAlignmentVertical() {
			return GetActualAlignmentValue(InnerActualAlignment.Vertical, DifferentialFormatPropertyDescriptor.AlignmentVertical);
		}
		protected virtual XlReadingOrder GetActualAlignmentReadingOrder() {
			return GetActualAlignmentValue(InnerActualAlignment.ReadingOrder, DifferentialFormatPropertyDescriptor.AlignmentReadingOrder);
		}
		#endregion
		static XlHorizontalAlignment GetActualHorizontalAlignment(VariantValue value, XlHorizontalAlignment horizontalAlignment, bool isTextFormatApplied) {
			if (horizontalAlignment == XlHorizontalAlignment.General)
				return CalculateValueHorizontalAlignment(value.Type, isTextFormatApplied);
			else
				return horizontalAlignment;
		}
		static protected internal XlHorizontalAlignment CalculateValueHorizontalAlignment(VariantValueType valueType, bool isTextFormatApplied) {
			switch (valueType) {
				case VariantValueType.Boolean:
				case VariantValueType.Error:
					return XlHorizontalAlignment.Center;
				case VariantValueType.Numeric:
					if (isTextFormatApplied)
						return XlHorizontalAlignment.Left;
					else
						return XlHorizontalAlignment.Right;
				default:
					return XlHorizontalAlignment.Left;
			}
		}
	}
	#endregion
}
