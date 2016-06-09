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

using DevExpress.Utils;
using System.Collections.Generic;
using DevExpress.Spreadsheet;
using System;
using System.Diagnostics;
using System.Drawing;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using System.Collections;
using DevExpress.Export.Xl;
#if SL
	using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Compatibility.System.Drawing;
	using DevExpress.Export.Xl;
	#region NativeRangeFormat
	partial class NativeRangeFormat : Formatting {
		#region Fields
		NativeRange range;
		PropertyAccessor<string> numberFormat;
		NativeRangeFormatAlignmentPart alignmentPart;
		NativeRangeFormatFontPart fontPart;
		NativeRangeFormatBordersPart bordersPart;
		NativeRangeFormatFillPart fillPart;
		NativeRangeFormatProtectionPart protectionPart;
		NativeRangeFormatApplyPart flagsPart;
		bool isValid;
		#endregion
		public NativeRangeFormat(NativeRange range) {
			Guard.ArgumentNotNull(range, "range");
			this.range = range;
			this.alignmentPart = new NativeRangeFormatAlignmentPart(this);
			this.fontPart = new NativeRangeFormatFontPart(this);
			this.bordersPart = new NativeRangeFormatBordersPart(this);
			this.protectionPart = new NativeRangeFormatProtectionPart(this);
			this.fillPart = new NativeRangeFormatFillPart(this);
			this.flagsPart = new NativeRangeFormatApplyPart(this);
			this.numberFormat = new NumberFormatPropertyAccessor(range.ModelRange);
			this.isValid = true;
		}
		#region Properties
		public NativeRange Range { get { return range; } }
		public bool IsValid { get { return isValid; } set { isValid = value; } }
		public StyleFlags Flags { get { return flagsPart; } }
		#region NumberFormat
		public string NumberFormat {
			get {
				return numberFormat.GetValue();
			}
			set {
				if (numberFormat.SetValue(value))
					RaiseChanged();
			}
		}
		#endregion
		public Alignment Alignment { get { return alignmentPart; } }
		public DevExpress.Spreadsheet.SpreadsheetFont Font { get { return fontPart; } }
		public Borders Borders { get { return bordersPart; } }
		public Fill Fill { get { return fillPart; } }
		public Protection Protection { get { return protectionPart; } }
		public string Name { get { return String.Empty; } }
		public int OutlineLevel { get { return 0; } set { } }
		public bool Hidden { get { return false; } set { } }
		#endregion
		public void CopyFrom(BuiltInStyleId id) {
		}
		public void BeginUpdate() {
			this.range.ModelWorkbook.BeginUpdate();
		}
		public void EndUpdate() {
			this.range.ModelWorkbook.EndUpdate();
		}
		protected internal void RaiseChanged() {
		}
		public void CheckValid() {
			if (!isValid) {
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorUseInvalidRangeFormatObject);
			}
		}
	}
	#endregion
	#region PropertyAccessors
	#region NumberFormat
	public class NumberFormatPropertyAccessor : RangeFormatsValuePropertyAccessor<string> {
		public NumberFormatPropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<string> CreateModifierCore(string newValue) {
			return new DevExpress.XtraSpreadsheet.Model.NumberFormatPropertyModifier(newValue);
		}
		protected internal override string CalculateValueCore(Model.ICell cell, DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifierBase modifier) {
			DevExpress.XtraSpreadsheet.Model.NumberFormatPropertyModifier typedModifier = (DevExpress.XtraSpreadsheet.Model.NumberFormatPropertyModifier)modifier;
			return typedModifier.GetFormatPropertyValue(cell);
		}
		protected internal override string CalculateValueCore(Model.FormatBase format, DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifierBase modifier) {
			DevExpress.XtraSpreadsheet.Model.NumberFormatPropertyModifier typedModifier = (DevExpress.XtraSpreadsheet.Model.NumberFormatPropertyModifier)modifier;
			return typedModifier.GetFormatPropertyValue(format);
		}
		protected internal override IEnumerable<ICellBase> GetRangeExistingCellForModifyEnumerator(Model.CellRange range) {
			return range.GetExistingCellsEnumerable();
		}
		protected override IList<Model.Column> GetAffectedColumns(int firstColumnIndex, int lastColumnIndex) {
			return ModelWorksheet.Columns.GetColumnRangesEnsureExist(firstColumnIndex, lastColumnIndex);
		}
		protected override IList<Model.Row> GetAffectedRows(int first, int last) {
			return base.GetAffectedRows(first, last);
		}
	}
	#endregion
	#region  Alignment
	public class AlignmentHorizontalPropertyAccessor : RangeFormatsValuePropertyAccessor<XlHorizontalAlignment> {
		public AlignmentHorizontalPropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<XlHorizontalAlignment> CreateModifierCore(XlHorizontalAlignment newValue) {
			return new DevExpress.XtraSpreadsheet.Model.AlignmentHorizontalPropertyModifier(newValue);
		}
	}
	public class AlignmentVerticalPropertyAccessor : RangeFormatsValuePropertyAccessor<XlVerticalAlignment> {
		public AlignmentVerticalPropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<XlVerticalAlignment> CreateModifierCore(XlVerticalAlignment newValue) {
			return new DevExpress.XtraSpreadsheet.Model.AlignmentVerticalPropertyModifier(newValue);
		}
	}
	public class AlignmentRotationAnglePropertyAccessor : RangeFormatsValuePropertyAccessor<int> {
		public AlignmentRotationAnglePropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<int> CreateModifierCore(int newValue) {
			return new DevExpress.XtraSpreadsheet.Model.AlignmentRotationAnglePropertyModifier(newValue);
		}
	}
	public class AlignmentJustifyDistributedPropertyAccessor : RangeFormatsValuePropertyAccessor<bool> {
		public AlignmentJustifyDistributedPropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<bool> CreateModifierCore(bool newValue) {
			return new DevExpress.XtraSpreadsheet.Model.AlignmentJustifyDistributedPropertyModifier(newValue);
		}
	}
	public class AlignmentIndentPropertyAccessor : RangeFormatsValuePropertyAccessor<byte> {
		public AlignmentIndentPropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<byte> CreateModifierCore(byte newValue) {
			return new DevExpress.XtraSpreadsheet.Model.AlignmentIndentPropertyModifier(newValue);
		}
	}
	public class AlignmentShrinkToFitPropertyAccessor : RangeFormatsValuePropertyAccessor<bool> {
		public AlignmentShrinkToFitPropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<bool> CreateModifierCore(bool newValue) {
			return new DevExpress.XtraSpreadsheet.Model.AlignmentShrinkToFitPropertyModifier(newValue);
		}
	}
	public class AlignmentWrapTextPropertyAccessor : RangeFormatsValuePropertyAccessor<bool> {
		public AlignmentWrapTextPropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<bool> CreateModifierCore(bool newValue) {
			return new DevExpress.XtraSpreadsheet.Model.AlignmentWrapTextPropertyModifier(newValue);
		}
	}
	#endregion
	#region Font
	#region FontStylePropertyAccessor
	public class FontStylePropertyAccessor : RangeFormatsValuePropertyAccessor<DevExpress.Spreadsheet.SpreadsheetFontStyle> {
		public FontStylePropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override Model.FormatBasePropertyModifier<Spreadsheet.SpreadsheetFontStyle> CreateModifierCore(Spreadsheet.SpreadsheetFontStyle newValue) {
			return new Model.FontStylePropertyModifier(newValue);
		}
	}
	#endregion
	#region FontNamePropertyAccessor
	public class FontNamePropertyAccessor : RangeFormatsValuePropertyAccessor<string> {
		public FontNamePropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<string> CreateModifierCore(string newValue) {
			return new Model.FontNamePropertyModifier(newValue);
		}
		protected internal override string CalculateValueCore(Model.ICell cell, DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifierBase modifier) {
			DevExpress.XtraSpreadsheet.Model.FontNamePropertyModifier typedModifier = (DevExpress.XtraSpreadsheet.Model.FontNamePropertyModifier)modifier;
			return typedModifier.GetFormatPropertyValue(cell);
		}
		protected internal override string CalculateValueCore(Model.FormatBase format, DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifierBase modifier) {
			DevExpress.XtraSpreadsheet.Model.FontNamePropertyModifier typedModifier = (DevExpress.XtraSpreadsheet.Model.FontNamePropertyModifier)modifier;
			return typedModifier.GetFormatPropertyValue(format);
		}
	}
	#endregion
	#region FontBoldPropertyAccessor
	public class FontBoldPropertyAccessor : RangeFormatsValuePropertyAccessor<bool> {
		public FontBoldPropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override Model.FormatBasePropertyModifier<bool> CreateModifierCore(bool newValue) {
			return new Model.FontBoldPropertyModifier(newValue);
		}
	}
	#endregion
	#region FontItalicPropertyAccessor
	public class FontItalicPropertyAccessor : RangeFormatsValuePropertyAccessor<bool> {
		public FontItalicPropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override Model.FormatBasePropertyModifier<bool> CreateModifierCore(bool newValue) {
			return new Model.FontItalicPropertyModifier(newValue);
		}
	}
	#endregion
	#region FontOutlinePropertyAccessor
	public class FontOutlinePropertyAccessor : RangeFormatsValuePropertyAccessor<bool> {
		public FontOutlinePropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override Model.FormatBasePropertyModifier<bool> CreateModifierCore(bool newValue) {
			return new Model.FontOutlinePropertyModifier(newValue);
		}
	}
	#endregion
	#region FontScriptPropertyAccessor
	public class FontScriptPropertyAccessor : RangeFormatsValuePropertyAccessor<XlScriptType> {
		public FontScriptPropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<XlScriptType> CreateModifierCore(XlScriptType newValue) {
			return new Model.FontScriptPropertyModifier(newValue);
		}
	}
	#endregion
	#region FontSizePropertyAccessor
	public class FontSizePropertyAccessor : RangeFormatsValuePropertyAccessor<double> {
		public FontSizePropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<double> CreateModifierCore(double newValue) {
			return new Model.FontSizePropertyModifier(newValue);
		}
	}
	#endregion
	#region FontUnderlineTypePropertyAccessor
	public class FontUnderlineTypePropertyAccessor : RangeFormatsValuePropertyAccessor<XlUnderlineType> {
		public FontUnderlineTypePropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<XlUnderlineType> CreateModifierCore(XlUnderlineType newValue) {
			return new Model.FontUnderlineTypePropertyModifier(newValue);
		}
	}
	#endregion
	#region FontStrikeThroughPropertyAccessor
	public class FontStrikeThroughPropertyAccessor : RangeFormatsValuePropertyAccessor<bool> {
		public FontStrikeThroughPropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<bool> CreateModifierCore(bool newValue) {
			return new Model.FontStrikeThroughPropertyModifier(newValue);
		}
	}
	#endregion
	#region FontColorPropertyAccessor
	public class FontColorPropertyAccessor : RangeFormatsValuePropertyAccessor<Color> {
		public FontColorPropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<Color> CreateModifierCore(Color newValue) {
			return new Model.FontColorPropertyModifier(newValue);
		}
	}
	#endregion
	#endregion
	#region Protection
	#region ProtectionHiddenPropertyAccessor
	public class ProtectionHiddenPropertyAccessor : RangeFormatsValuePropertyAccessor<bool> {
		public ProtectionHiddenPropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<bool> CreateModifierCore(bool newValue) {
			return new Model.ProtectionHiddenPropertyModifier(newValue);
		}
	}
	#endregion
	#region ProtectionLockedPropertyAccessor
	public class ProtectionLockedPropertyAccessor : RangeFormatsValuePropertyAccessor<bool> {
		public ProtectionLockedPropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<bool> CreateModifierCore(bool newValue) {
			return new Model.ProtectionLockedPropertyModifier(newValue);
		}
		protected internal override bool GetDefaultPropertyValue() {
			return true;
		}
	}
	#endregion
	#endregion
	#region Fill
	#region PatternFillBackColorPropertyAccessor
	public class FillBackColorPropertyAccessor : RangeFormatsValuePropertyAccessor<Color> {
		public FillBackColorPropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<Color> CreateModifierCore(Color newValue) {
			return new Model.FillBackColorPropertyModifier(newValue);
		}
	}
	#endregion
	#region PatternFillForeColorPropertyAccessor
	public class FillForeColorPropertyAccessor : RangeFormatsValuePropertyAccessor<Color> {
		public FillForeColorPropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<Color> CreateModifierCore(Color newValue) {
			return new Model.FillForeColorPropertyModifier(newValue);
		}
	}
	#endregion
	#region PatternFillPatternTypePropertyAccessor
	public class FillPatternTypePropertyAccessor : RangeFormatsValuePropertyAccessor<XlPatternType> {
		public FillPatternTypePropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<XlPatternType> CreateModifierCore(XlPatternType newValue) {
			return new Model.FillPatternTypePropertyModifier(newValue);
		}
	}
	#endregion
	#region FillTypePropertyAccessor
	public class FillTypePropertyAccessor : RangeFormatsValuePropertyAccessor<Model.ModelFillType> {
		public FillTypePropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<Model.ModelFillType> CreateModifierCore(Model.ModelFillType newValue) {
			return new Model.FillTypePropertyModifier(newValue);
		}
	}
	#endregion
	#region GradientFillTypePropertyAccessor
	public class GradientFillTypePropertyAccessor : RangeFormatsValuePropertyAccessor<Model.ModelGradientFillType> {
		public GradientFillTypePropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<Model.ModelGradientFillType> CreateModifierCore(Model.ModelGradientFillType newValue) {
			return new Model.GradientFillTypePropertyModifier(newValue);
		}
	}
	#endregion
	#region GradientFillDegreePropertyAccessor
	public class GradientFillDegreePropertyAccessor : RangeFormatsValuePropertyAccessor<double> {
		public GradientFillDegreePropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<double> CreateModifierCore(double newValue) {
			return new Model.GradientFillDegreePropertyModifier(newValue);
		}
	}
	#endregion
	#region GradientFillRectangleLeftPropertyAccessor
	public class GradientFillRectangleLeftPropertyAccessor : RangeFormatsValuePropertyAccessor<float> {
		public GradientFillRectangleLeftPropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<float> CreateModifierCore(float newValue) {
			return new Model.GradientFillRectangleLeftPropertyModifier(newValue);
		}
	}
	#endregion
	#region GradientFillRectangleRightPropertyAccessor
	public class GradientFillRectangleRightPropertyAccessor : RangeFormatsValuePropertyAccessor<float> {
		public GradientFillRectangleRightPropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<float> CreateModifierCore(float newValue) {
			return new Model.GradientFillRectangleRightPropertyModifier(newValue);
		}
	}
	#endregion
	#region GradientFillRectangleTopPropertyAccessor
	public class GradientFillRectangleTopPropertyAccessor : RangeFormatsValuePropertyAccessor<float> {
		public GradientFillRectangleTopPropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<float> CreateModifierCore(float newValue) {
			return new Model.GradientFillRectangleTopPropertyModifier(newValue);
		}
	}
	#endregion
	#region GradientFillRectangleBottomPropertyAccessor
	public class GradientFillRectangleBottomPropertyAccessor : RangeFormatsValuePropertyAccessor<float> {
		public GradientFillRectangleBottomPropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<float> CreateModifierCore(float newValue) {
			return new Model.GradientFillRectangleBottomPropertyModifier(newValue);
		}
	}
	#endregion
	#region GradientFillStopsPropertyAccessor
	public class GradientFillStopsPropertyAccessor : RangeFormatsValuePropertyAccessor<Model.IGradientStopCollection> {
		public GradientFillStopsPropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<Model.IGradientStopCollection> CreateModifierCore(Model.IGradientStopCollection newValue) {
			return new Model.GradientFillStopsPropertyModifier(newValue);
		}
		protected internal bool AddGradientStop(double position, Color color) {
			Model.IGradientStopCollection value = GetValue();
			Model.IGradientStopCollection newValue = value.Clone();
			newValue.Add(position, color);
			return SetValue(newValue);
		}
		protected internal bool ClearGradientStops() {
			Model.IGradientStopCollection value = GetValue();
			Model.IGradientStopCollection newValue = value.Clone();
			newValue.Clear();
			return SetValue(newValue);
		}
		protected internal bool RemoveAtGradientStop(int index) {
			Model.IGradientStopCollection value = GetValue();
			Model.IGradientStopCollection newValue = value.Clone();
			newValue.RemoveAt(index);
			return SetValue(newValue);
		}
	}
	#endregion
	#endregion
	public abstract class RangeSideCellsEnumeratorAdapter : IEnumerator<Model.ICellBase> {
		readonly IEnumerator<Model.ICellBase> innerEnumerator;
		readonly CellPosition topLeft;
		protected RangeSideCellsEnumeratorAdapter(IEnumerator<Model.ICellBase> innerEnumerator, CellPosition topLeft) {
			this.innerEnumerator = innerEnumerator;
			this.topLeft = topLeft;
		}
		#region IEnumerator<T> Members
		public Model.ICellBase Current { get { return innerEnumerator.Current; } }
		#endregion
		#region IDisposable Members
		public void Dispose() {
			innerEnumerator.Dispose();
		}
		#endregion
		#region IEnumerator Members
		object IEnumerator.Current {
			get { return Current; }
		}
		public bool MoveNext() {
			bool result = innerEnumerator.MoveNext();
			while (result && Current != null && !ConditionForValidCells(Current, topLeft))
				result = innerEnumerator.MoveNext();
			return result;
		}
		public void Reset() {
			innerEnumerator.Reset();
		}
		#endregion
		protected abstract bool ConditionForValidCells(Model.ICellBase current, CellPosition topLeft);
	}
	#region Borders
	public class LeftRangeSideCellsEnumeratorAdapter : RangeSideCellsEnumeratorAdapter {
		public LeftRangeSideCellsEnumeratorAdapter(IEnumerator<Model.ICellBase> innerEnumerator, CellPosition topLeft)
			: base(innerEnumerator, topLeft) {
		}
		protected override bool ConditionForValidCells(Model.ICellBase current, CellPosition topLeft) {
			return current.ColumnIndex == topLeft.Column;
		}
	}
	public class RightRangeSideCellsEnumeratorAdapter : RangeSideCellsEnumeratorAdapter {
		readonly int lastRelativeColumnIndex;
		public RightRangeSideCellsEnumeratorAdapter(IEnumerator<Model.ICellBase> innerEnumerator, int lastRelativeColumnIndex, CellPosition topLeft)
			: base(innerEnumerator, topLeft) {
			this.lastRelativeColumnIndex = lastRelativeColumnIndex;
		}
		protected override bool ConditionForValidCells(Model.ICellBase current, CellPosition topLeft) {
			return current.ColumnIndex - topLeft.Column == lastRelativeColumnIndex;
		}
	}
	public class TopRangeSideCellsEnumeratorAdapter : RangeSideCellsEnumeratorAdapter {
		public TopRangeSideCellsEnumeratorAdapter(IEnumerator<Model.ICellBase> innerEnumerator, CellPosition topLeft)
			: base(innerEnumerator, topLeft) {
		}
		protected override bool ConditionForValidCells(Model.ICellBase current, CellPosition topLeft) {
			return current.RowIndex == topLeft.Row;
		}
	}
	public class BottomRangeSideCellsEnumeratorAdapter : RangeSideCellsEnumeratorAdapter {
		readonly int lastRelativeRowIndex;
		public BottomRangeSideCellsEnumeratorAdapter(IEnumerator<Model.ICellBase> innerEnumerator, int lastRelativeRowIndex, CellPosition topLeft)
			: base(innerEnumerator, topLeft) {
			this.lastRelativeRowIndex = lastRelativeRowIndex;
		}
		protected override bool ConditionForValidCells(Model.ICellBase current, CellPosition topLeft) {
			return current.RowIndex - topLeft.Row == lastRelativeRowIndex;
		}
	}
	public abstract class LeftRangeSidePropertyAccessorBase<T> : RangeFormatsValuePropertyAccessor<T> {
		protected LeftRangeSidePropertyAccessorBase(Model.CellRangeBase range)
			: base(range) {
		}
		int RelativeColumnIndex { get { return 0; } }
		int RelativeFirstRelativeRow { get { return 0; } }
		protected internal override IEnumerable<Model.ICellBase> GetRangeCellForModifyEnumerator(Model.CellRange range) {
			int lastRelativeRow = range.Height;
			for (int row = RelativeFirstRelativeRow; row < lastRelativeRow; row++) {
				yield return range.GetCellRelative(RelativeColumnIndex, row);
			}
		}
		protected internal override IEnumerable<ICellBase> GetRangeExistingCellForModifyEnumerator(Model.CellRange range) {
			IEnumerator<Model.ICellBase> enumerator = new LeftRangeSideCellsEnumeratorAdapter(range.GetExistingCellsEnumerator(false), range.TopLeft);
			return new Enumerable<ICellBase>(enumerator);
		}
		protected override IList<Model.Column> GetAffectedColumns(int firstColumnIndex, int lastColumnIndex) {
			return ModelWorksheet.Columns.GetColumnRangesEnsureExist(firstColumnIndex, firstColumnIndex);
		}
		protected override IList<Model.Row> GetAffectedRows(int first, int last) {
			throw new InvalidOperationException();
		}
	}
	public abstract class RightRangeSidePropertyAccessorBase<T> : RangeFormatsValuePropertyAccessor<T> {
		protected RightRangeSidePropertyAccessorBase(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override IEnumerable<Model.ICellBase> GetRangeCellForModifyEnumerator(Model.CellRange range) {
			int columnIndex = range.Width - 1;
			int firstRelativeRow = 0;
			int lastRelativeRow = range.Height;
			for (int row = firstRelativeRow; row < lastRelativeRow; row++) {
				yield return range.GetCellRelative(columnIndex, row);
			}
		}
		protected internal override IEnumerable<ICellBase> GetRangeExistingCellForModifyEnumerator(Model.CellRange range) {
			IEnumerator<Model.ICellBase> enumerator = new RightRangeSideCellsEnumeratorAdapter(range.GetExistingCellsEnumerable().GetEnumerator(), range.Width - 1, range.TopLeft);
			return new Enumerable<ICellBase>(enumerator);
		}
		protected override IList<Model.Column> GetAffectedColumns(int firstColumnIndex, int lastColumnIndex) {
			return ModelWorksheet.Columns.GetColumnRangesEnsureExist(lastColumnIndex, lastColumnIndex);
		}
		protected override IList<Model.Row> GetAffectedRows(int first, int last) {
			throw new InvalidOperationException();
		}
	}
	public abstract class TopRangeSidePropertyAccessorBase<T> : RangeFormatsValuePropertyAccessor<T> {
		protected TopRangeSidePropertyAccessorBase(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override IEnumerable<Model.ICellBase> GetRangeCellForModifyEnumerator(Model.CellRange range) {
			int rowIndex = 0;
			int firstRelativeColumn = 0;
			int lastRelativeColumn = range.Width;
			for (int column = firstRelativeColumn; column < lastRelativeColumn; column++) {
				yield return range.GetCellRelative(column, rowIndex);
			}
		}
		protected internal override IEnumerable<ICellBase> GetRangeExistingCellForModifyEnumerator(Model.CellRange range) {
			IEnumerator<Model.ICellBase> enumerator = new TopRangeSideCellsEnumeratorAdapter(range.GetExistingCellsEnumerator(false), range.TopLeft);
			return new Enumerable<ICellBase>(enumerator);
		}
		protected override IList<Model.Column> GetAffectedColumns(int firstColumnIndex, int lastColumnIndex) {
			throw new InvalidOperationException();
		}
		protected override IList<Model.Row> GetAffectedRows(int first, int last) {
			return new List<Model.Row>() { ModelWorksheet.Rows[first] };
		}
	}
	public abstract class BottomRangeSidePropertyAccessorBase<T> : RangeFormatsValuePropertyAccessor<T> {
		protected BottomRangeSidePropertyAccessorBase(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override IEnumerable<Model.ICellBase> GetRangeCellForModifyEnumerator(Model.CellRange range) {
			int rowIndex = range.Height - 1;
			int firstRelativeColumn = 0;
			int lastRelativeColumn = range.Width;
			for (int column = firstRelativeColumn; column < lastRelativeColumn; column++) {
				yield return range.GetCellRelative(column, rowIndex);
			}
		}
		protected internal override IEnumerable<ICellBase> GetRangeExistingCellForModifyEnumerator(Model.CellRange range) {
			IEnumerator<Model.ICellBase> enumerator = new BottomRangeSideCellsEnumeratorAdapter(range.GetExistingCellsEnumerable().GetEnumerator(), range.Height - 1, range.TopLeft);
			return new Enumerable<ICellBase>(enumerator);
		}
		protected override IList<Model.Column> GetAffectedColumns(int firstColumnIndex, int lastColumnIndex) {
			return null;
		}
		protected override IList<Model.Row> GetAffectedRows(int first, int last) {
			return new List<Model.Row>() { ModelWorksheet.Rows[last] };
		}
	}
	#region LeftBorderColorPropertyAccessor
	public class LeftBorderColorPropertyAccessor : LeftRangeSidePropertyAccessorBase<Color> {
		public LeftBorderColorPropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<Color> CreateModifierCore(Color newValue) {
			return new Model.LeftBorderColorPropertyModifier(newValue);
		}
	}
	#endregion
	#region LeftBorderLineStylePropertyAccessor
	public class LeftBorderLineStylePropertyAccessor : LeftRangeSidePropertyAccessorBase<XlBorderLineStyle> {
		public LeftBorderLineStylePropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<XlBorderLineStyle> CreateModifierCore(XlBorderLineStyle newValue) {
			return new Model.LeftBorderLineStylePropertyModifier(newValue);
		}
	}
	#endregion
	#region RightBorderColorPropertyAccessor
	public class RightBorderColorPropertyAccessor : RightRangeSidePropertyAccessorBase<Color> {
		public RightBorderColorPropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<Color> CreateModifierCore(Color newValue) {
			return new Model.RightBorderColorPropertyModifier(newValue);
		}
	}
	#endregion
	#region RightBorderLineStylePropertyAccessor
	public class RightBorderLineStylePropertyAccessor : RightRangeSidePropertyAccessorBase<XlBorderLineStyle> {
		public RightBorderLineStylePropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<XlBorderLineStyle> CreateModifierCore(XlBorderLineStyle newValue) {
			return new Model.RightBorderLineStylePropertyModifier(newValue);
		}
	}
	#endregion
	#region TopBorderColorPropertyAccessor
	public class TopBorderColorPropertyAccessor : TopRangeSidePropertyAccessorBase<Color> {
		public TopBorderColorPropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<Color> CreateModifierCore(Color newValue) {
			return new Model.TopBorderColorPropertyModifier(newValue);
		}
	}
	#endregion
	#region TopBorderLineStylePropertyAccessor
	public class TopBorderLineStylePropertyAccessor : TopRangeSidePropertyAccessorBase<XlBorderLineStyle> {
		public TopBorderLineStylePropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<XlBorderLineStyle> CreateModifierCore(XlBorderLineStyle newValue) {
			return new Model.TopBorderLineStylePropertyModifier(newValue);
		}
	}
	#endregion
	#region BottomBorderColorPropertyAccessor
	public class BottomBorderColorPropertyAccessor : BottomRangeSidePropertyAccessorBase<Color> {
		public BottomBorderColorPropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<Color> CreateModifierCore(Color newValue) {
			return new Model.BottomBorderColorPropertyModifier(newValue);
		}
	}
	#endregion
	#region BottomBorderLineStylePropertyAccessor
	public class BottomBorderLineStylePropertyAccessor : BottomRangeSidePropertyAccessorBase<XlBorderLineStyle> {
		public BottomBorderLineStylePropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<XlBorderLineStyle> CreateModifierCore(XlBorderLineStyle newValue) {
			return new Model.BottomBorderLineStylePropertyModifier(newValue);
		}
	}
	#endregion
	#region DiagonalBorderColorPropertyAccessor
	public class DiagonalBorderColorPropertyAccessor : RangeFormatsValuePropertyAccessor<Color> {
		public DiagonalBorderColorPropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<Color> CreateModifierCore(Color newValue) {
			return new Model.DiagonalBorderColorPropertyModifier(newValue);
		}
	}
	#endregion
	#region DiagonalUpBorderLineStylePropertyAccessor
	public class DiagonalUpBorderLineStylePropertyAccessor : RangeFormatsValuePropertyAccessor<XlBorderLineStyle> {
		public DiagonalUpBorderLineStylePropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<XlBorderLineStyle> CreateModifierCore(XlBorderLineStyle newValue) {
			return new Model.DiagonalUpBorderLineStylePropertyModifier(newValue);
		}
	}
	#endregion
	#region DiagonalDownBorderLineStylePropertyAccessor
	public class DiagonalDownBorderLineStylePropertyAccessor : RangeFormatsValuePropertyAccessor<XlBorderLineStyle> {
		public DiagonalDownBorderLineStylePropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<XlBorderLineStyle> CreateModifierCore(XlBorderLineStyle newValue) {
			return new Model.DiagonalDownBorderLineStylePropertyModifier(newValue);
		}
	}
	#endregion
	#endregion
	#region Applies
	public class ApplyAllPropertyAccessor : RangeFormatsValuePropertyAccessor<bool> {
		public ApplyAllPropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<bool> CreateModifierCore(bool newValue) {
			return new Model.ApplyAllPropertyModifier(newValue);
		}
	}
	public class ApplyNumberPropertyAccessor : RangeFormatsValuePropertyAccessor<bool> {
		public ApplyNumberPropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<bool> CreateModifierCore(bool newValue) {
			return new Model.ApplyNumberPropertyModifier(newValue);
		}
	}
	public class ApplyAlignmentPropertyAccessor : RangeFormatsValuePropertyAccessor<bool> {
		public ApplyAlignmentPropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<bool> CreateModifierCore(bool newValue) {
			return new Model.ApplyAlignmentPropertyModifier(newValue);
		}
	}
	public class ApplyFontPropertyAccessor : RangeFormatsValuePropertyAccessor<bool> {
		public ApplyFontPropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<bool> CreateModifierCore(bool newValue) {
			return new Model.ApplyFontPropertyModifier(newValue);
		}
	}
	public class ApplyBordersPropertyAccessor : RangeFormatsValuePropertyAccessor<bool> {
		public ApplyBordersPropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<bool> CreateModifierCore(bool newValue) {
			return new Model.ApplyBordersPropertyModifier(newValue);
		}
	}
	public class ApplyFillPropertyAccessor : RangeFormatsValuePropertyAccessor<bool> {
		public ApplyFillPropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<bool> CreateModifierCore(bool newValue) {
			return new Model.ApplyFillPropertyModifier(newValue);
		}
	}
	public class ApplyProtectionPropertyAccessor : RangeFormatsValuePropertyAccessor<bool> {
		public ApplyProtectionPropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<bool> CreateModifierCore(bool newValue) {
			return new Model.ApplyProtectionPropertyModifier(newValue);
		}
	}
	#endregion
	#endregion
	#region  NativeRange Parts
	#region  NativeRangeFormatPartBase (abstract)
	abstract partial class NativeRangeFormatPartBase {
		NativeRangeFormat owner;
		protected NativeRangeFormatPartBase(NativeRangeFormat owner) {
			this.owner = owner;
		}
		public NativeRangeFormat Owner { get { return owner; } set { owner = value; } }
		public NativeRange Range { get { return Owner.Range; } }
		public Model.CellRangeBase ModelRange { get { return Range.ModelRange; } }
		public DevExpress.Office.DocumentModelUnitConverter UnitConverter { get { return (this.ModelRange.Worksheet.Workbook as DocumentModel).UnitConverter; } }
	}
	#endregion
	#region NativeRangeFormatAlignmentPart
	partial class NativeRangeFormatAlignmentPart : NativeRangeFormatPartBase, Alignment {
		PropertyAccessor<XlHorizontalAlignment> horizontal;
		PropertyAccessor<XlVerticalAlignment> vertical;
		PropertyAccessor<bool> wrapText;
		PropertyAccessor<bool> shrinkToFit;
		PropertyAccessor<byte> indent;
		PropertyAccessor<bool> justifyDistributed;
		PropertyAccessor<int> rotationAngle;
		public NativeRangeFormatAlignmentPart(NativeRangeFormat owner)
			: base(owner) {
			this.horizontal = new AlignmentHorizontalPropertyAccessor(ModelRange);
			this.vertical = new AlignmentVerticalPropertyAccessor(ModelRange);
			this.wrapText = new AlignmentWrapTextPropertyAccessor(ModelRange);
			this.shrinkToFit = new AlignmentShrinkToFitPropertyAccessor(ModelRange);
			this.indent = new AlignmentIndentPropertyAccessor(ModelRange);
			this.justifyDistributed = new AlignmentJustifyDistributedPropertyAccessor(ModelRange);
			this.rotationAngle = new AlignmentRotationAnglePropertyAccessor(ModelRange);
		}
		public SpreadsheetHorizontalAlignment Horizontal {
			get {
				CheckValid();
				return (SpreadsheetHorizontalAlignment)horizontal.GetValue();
			}
			set {
				CheckValid();
				Owner.BeginUpdate();
				try {
					if (Horizontal != value) {
						if (AlignmentChangeHelper.ShouldResetIndentOnChangeHorizontalAlignment(value))
							indent.SetValue(0);
					}
					if (horizontal.SetValue((XlHorizontalAlignment)value))
						RaiseChanged();
				}
				finally {
					Owner.EndUpdate();
				}
			}
		}
		public SpreadsheetVerticalAlignment Vertical {
			get {
				CheckValid();
				return (SpreadsheetVerticalAlignment)vertical.GetValue();
			}
			set {
				CheckValid();
				if (vertical.SetValue((XlVerticalAlignment)value))
					RaiseChanged();
			}
		}
		public bool WrapText {
			get {
				CheckValid();
				return wrapText.GetValue();
			}
			set {
				CheckValid();
				if (wrapText.SetValue(value))
					RaiseChanged();
			}
		}
		public bool ShrinkToFit {
			get {
				CheckValid();
				return shrinkToFit.GetValue();
			}
			set {
				CheckValid();
				if (shrinkToFit.SetValue(value))
					RaiseChanged();
			}
		}
		public int Indent {
			get {
				CheckValid();
				byte result = indent.GetValue();
				Debug.Assert(result < 250);
				return result;
			}
			set {
				CheckValid();
				if (value < 0 || value > 250) {
					SpreadsheetExceptions.ThrowArgumentOutOfRangeException(XtraSpreadsheetStringId.Msg_ErrorIncorrectIndentValue, "value");
				}
				Owner.BeginUpdate();
				try {
					if (Indent != value && value > 0) {
						if (AlignmentChangeHelper.ShouldResetHorizontalAlignmentOnIndentChange(Horizontal))
							Horizontal = SpreadsheetHorizontalAlignment.Left;
					}
					if (indent.SetValue((byte)value))
						RaiseChanged();
				}
				finally {
					Owner.EndUpdate();
				}
			}
		}
		public bool JustifyDistributed {
			get {
				CheckValid();
				return justifyDistributed.GetValue();
			}
			set {
				CheckValid();
				if (justifyDistributed.SetValue(value))
					RaiseChanged();
			}
		}
		public int RotationAngle {
			get {
				CheckValid();
				return ModelToApiRotationAngleConverter.ConvertToApiValue(rotationAngle.GetValue(), UnitConverter);
			}
			set {
				CheckValid();
				int valueInModelUnits = ModelToApiRotationAngleConverter.ConvertToModelValue(value, UnitConverter);
				if (rotationAngle.SetValue(valueInModelUnits))
					RaiseChanged();
			}
		}
		protected void CheckValid() {
			Owner.CheckValid();
		}
		void RaiseChanged() {
			Owner.RaiseChanged();
		}
	}
	#endregion
	#region NativeRangeFormatFontPart
	partial class NativeRangeFormatFontPart : NativeRangeFormatPartBase, DevExpress.Spreadsheet.SpreadsheetFont {
		PropertyAccessor<DevExpress.Spreadsheet.SpreadsheetFontStyle> fontStyle;
		PropertyAccessor<bool> fontBold;
		PropertyAccessor<bool> fontItalic;
		PropertyAccessor<string> fontName;
		PropertyAccessor<bool> fontOutline;
		PropertyAccessor<XlScriptType> fontScript;
		PropertyAccessor<double> fontSize;
		PropertyAccessor<XlUnderlineType> fontUnderlineType;
		PropertyAccessor<bool> fontStrikeThrough;
		PropertyAccessor<Color> fontColor;
		public NativeRangeFormatFontPart(NativeRangeFormat owner)
			: base(owner) {
			this.fontStyle = new FontStylePropertyAccessor(ModelRange);
			this.fontBold = new FontBoldPropertyAccessor(ModelRange);
			this.fontItalic = new FontItalicPropertyAccessor(ModelRange);
			this.fontName = new FontNamePropertyAccessor(ModelRange);
			this.fontOutline = new FontOutlinePropertyAccessor(ModelRange);
			this.fontScript = new FontScriptPropertyAccessor(ModelRange);
			this.fontSize = new FontSizePropertyAccessor(ModelRange);
			this.fontUnderlineType = new FontUnderlineTypePropertyAccessor(ModelRange);
			this.fontStrikeThrough = new FontStrikeThroughPropertyAccessor(ModelRange);
			this.fontColor = new FontColorPropertyAccessor(ModelRange);
		}
		#region Font Members
		public Spreadsheet.SpreadsheetFontStyle FontStyle {
			get {
				CheckValid();
				return fontStyle.GetValue();
			}
			set {
				CheckValid();
				if (fontStyle.SetValue(value))
					RaiseChanged();
			}
		}
		public bool Bold {
			get {
				CheckValid();
				return fontBold.GetValue();
			}
			set {
				CheckValid();
				if (fontBold.SetValue(value))
					RaiseChanged();
			}
		}
		public bool Italic {
			get {
				CheckValid();
				return fontItalic.GetValue();
			}
			set {
				CheckValid();
				if (fontItalic.SetValue(value))
					RaiseChanged();
			}
		}
		public string Name {
			get {
				return fontName.GetValue();
			}
			set {
				if (String.IsNullOrEmpty(value))
					return;
				if (fontName.SetValue(value))
					RaiseChanged();
			}
		}
		public bool Outline {
			get {
				CheckValid();
				return fontOutline.GetValue();
			}
			set {
				CheckValid();
				if (fontOutline.SetValue(value))
					RaiseChanged();
			}
		}
		public ScriptType Script {
			get {
				CheckValid();
				return (ScriptType)fontScript.GetValue();
			}
			set {
				CheckValid();
				if (fontScript.SetValue((XlScriptType)value))
					RaiseChanged();
			}
		}
		public double Size {
			get {
				CheckValid();
				return fontSize.GetValue();
			}
			set {
				CheckValid();
				if (fontSize.SetValue(value))
					RaiseChanged();
			}
		}
		public UnderlineType UnderlineType {
			get {
				CheckValid();
				return (UnderlineType)fontUnderlineType.GetValue();
			}
			set {
				CheckValid();
				if (fontUnderlineType.SetValue((XlUnderlineType)value))
					RaiseChanged();
			}
		}
		public bool Strikethrough {
			get {
				CheckValid();
				return fontStrikeThrough.GetValue();
			}
			set {
				CheckValid();
				if (fontStrikeThrough.SetValue(value))
					RaiseChanged();
			}
		}
		public Color Color {
			get {
				CheckValid();
				return fontColor.GetValue();
			}
			set {
				CheckValid();
				if (fontColor.SetValue(value))
					RaiseChanged();
			}
		}
		#endregion
		protected void CheckValid() {
			Owner.CheckValid();
		}
		private void RaiseChanged() {
			Owner.RaiseChanged();
		}
	}
	#endregion
	#region NativeRangeFormatFillPart
	partial class NativeRangeFormatFillPart : NativeRangeFormatPartBase, Fill {
		PropertyAccessor<Color> foreColor;
		PropertyAccessor<Color> backColor;
		PropertyAccessor<XlPatternType> patternType;
		PropertyAccessor<Model.ModelFillType> fillType;
		NativeRangeFormatGradientFillPart gradientFillPart;
		public NativeRangeFormatFillPart(NativeRangeFormat owner)
			: base(owner) {
			this.foreColor = new FillForeColorPropertyAccessor(ModelRange);
			this.backColor = new FillBackColorPropertyAccessor(ModelRange);
			this.patternType = new FillPatternTypePropertyAccessor(ModelRange);
			this.fillType = new FillTypePropertyAccessor(ModelRange);
			this.gradientFillPart = new NativeRangeFormatGradientFillPart(owner);
		}
		#region BackgroundColor
		public Color BackgroundColor {
			get {
				XlPatternType pattern = patternType.GetValue();
				if (pattern == XlPatternType.None)
					return DXColor.Empty;
				return GetFillColorCore(foreColor, backColor, pattern);
			}
			set {
				XlPatternType pattern = patternType.GetValue();
				bool wasPatternTypeNone = pattern == XlPatternType.None;
				PropertyAccessor<Color> colorAccessor = wasPatternTypeNone || pattern == XlPatternType.Solid ? foreColor : backColor;
				colorAccessor.SetValue(value.RemoveTransparency());
				if (wasPatternTypeNone)
					patternType.SetValue(XlPatternType.Solid);
			}
		}
		#endregion
		#region PatternColor
		public Color PatternColor {
			get {
				return GetFillColorCore(backColor, foreColor, patternType.GetValue());
			}
			set {
				XlPatternType pattern = patternType.GetValue();
				bool wasPatternTypeNone = pattern == XlPatternType.None;
				PropertyAccessor<Color> colorAccessor = wasPatternTypeNone || pattern == XlPatternType.Solid ? backColor : foreColor;
				colorAccessor.SetValue(value.RemoveTransparency());
				if (wasPatternTypeNone)
					patternType.SetValue(XlPatternType.Solid);
			}
		}
		Color GetFillColorCore(PropertyAccessor<Color> firstAccessor, PropertyAccessor<Color> secondAccessor, XlPatternType pattern) {
			PropertyAccessor<Color> colorAccessor = pattern == XlPatternType.Solid ? firstAccessor : secondAccessor;
			return colorAccessor.GetValue();
		}
		#endregion
		#region PatternType
		public PatternType PatternType {
			get {
				CheckValid();
				return (PatternType)patternType.GetValue();
			}
			set {
				XlPatternType newValue = (XlPatternType)value;
				if (PerformSwapFillColors(patternType.GetValue(), newValue)) {
					Color tmp = backColor.GetValue();
					backColor.SetValue(foreColor.GetValue());
					foreColor.SetValue(tmp);
				}
				patternType.SetValue(newValue);
			}
		}
		bool PerformSwapFillColors(XlPatternType oldValue, XlPatternType newValue) {
			bool wasPatternTypeNone = oldValue == XlPatternType.None;
			bool newPatternTypeSolid = newValue == XlPatternType.Solid;
			return
				(!wasPatternTypeNone && oldValue == XlPatternType.Solid &&
				(newValue == XlPatternType.None || !newPatternTypeSolid)) ||
				(wasPatternTypeNone && newPatternTypeSolid);
		}
		#endregion
		#region FillType
		public FillType FillType {
			get {
				CheckValid();
				return (FillType)fillType.GetValue();
			}
			set {
				CheckValid();
				fillType.SetValue((Model.ModelFillType)value);
			}
		}
		#endregion
		#region GradientFill
		public GradientFill Gradient { get { return gradientFillPart; } }
		#endregion
		protected void CheckValid() {
			Owner.CheckValid();
		}
	}
	#endregion
	#region NativeRangeFormatGradientFillPart
	partial class NativeRangeFormatGradientFillPart : NativeRangeFormatPartBase, GradientFill {
		PropertyAccessor<Model.ModelGradientFillType> gradientType;
		PropertyAccessor<double> degree;
		PropertyAccessor<float> rectangleLeft;
		PropertyAccessor<float> rectangleRight;
		PropertyAccessor<float> rectangleTop;
		PropertyAccessor<float> rectangleBottom;
		NativeRangeFormatGradientFillStopsPart stopsPart;
		public NativeRangeFormatGradientFillPart(NativeRangeFormat owner)
			: base(owner) {
			this.gradientType = new GradientFillTypePropertyAccessor(ModelRange);
			this.degree = new GradientFillDegreePropertyAccessor(ModelRange);
			this.rectangleLeft = new GradientFillRectangleLeftPropertyAccessor(ModelRange);
			this.rectangleRight = new GradientFillRectangleRightPropertyAccessor(ModelRange);
			this.rectangleTop = new GradientFillRectangleTopPropertyAccessor(ModelRange);
			this.rectangleBottom = new GradientFillRectangleBottomPropertyAccessor(ModelRange);
			this.stopsPart = new NativeRangeFormatGradientFillStopsPart(owner);
		}
		#region GradientFill Members
		#region GradientType
		GradientFillType GradientFill.Type {
			get {
				CheckValid();
				return (GradientFillType)gradientType.GetValue();
			}
			set {
				CheckValid();
				if (gradientType.SetValue((Model.ModelGradientFillType)value))
					RaiseChanged();
			}
		}
		#endregion
		#region Degree
		double GradientFill.Degree {
			get {
				CheckValid();
				return degree.GetValue();
			}
			set {
				CheckValid();
				if (degree.SetValue(value))
					RaiseChanged();
			}
		}
		#endregion
		#region RectangleLeft
		float GradientFill.RectangleLeft {
			get {
				CheckValid();
				return rectangleLeft.GetValue();
			}
			set {
				CheckValid();
				if (rectangleLeft.SetValue(value))
					RaiseChanged();
			}
		}
		#endregion
		#region RectangleRight
		float GradientFill.RectangleRight {
			get {
				CheckValid();
				return rectangleRight.GetValue();
			}
			set {
				CheckValid();
				if (rectangleRight.SetValue(value))
					RaiseChanged();
			}
		}
		#endregion
		#region RectangleTop
		float GradientFill.RectangleTop {
			get {
				CheckValid();
				return rectangleTop.GetValue();
			}
			set {
				CheckValid();
				if (rectangleTop.SetValue(value))
					RaiseChanged();
			}
		}
		#endregion
		#region RectangleBottom
		float GradientFill.RectangleBottom {
			get {
				CheckValid();
				return rectangleBottom.GetValue();
			}
			set {
				CheckValid();
				if (rectangleBottom.SetValue(value))
					RaiseChanged();
			}
		}
		#endregion
		Spreadsheet.GradientStopCollection GradientFill.Stops { get { return stopsPart; } }
		#endregion
		protected void CheckValid() {
			Owner.CheckValid();
		}
		void RaiseChanged() {
			Owner.RaiseChanged();
		}
	}
	#endregion
	#region NativeRangeFormatGradientFillStopsPart
	partial class NativeRangeFormatGradientFillStopsPart : NativeRangeFormatPartBase, Spreadsheet.GradientStopCollection {
		GradientFillStopsPropertyAccessor stops;
		public NativeRangeFormatGradientFillStopsPart(NativeRangeFormat owner)
			: base(owner) {
			this.stops = new GradientFillStopsPropertyAccessor(ModelRange);
		}
		Model.IGradientStopCollection ModelStops { get { return stops.GetValue(); } }
		#region Spreadsheet.GradientStopCollection Members
		public int Count { get { return ModelStops.Count; } }
		public Spreadsheet.GradientStop this[int index] { get { return new NativeGradientStop(ModelStops[index]); } }
		public void Add(double position, Color color) {
			CheckValid();
			if (stops.AddGradientStop(position, color))
				RaiseChanged();
		}
		public void Clear() {
			CheckValid();
			if (stops.ClearGradientStops())
				RaiseChanged();
		}
		public void RemoveAt(int index) {
			CheckValid();
			if (stops.RemoveAtGradientStop(index))
				RaiseChanged();
		}
		#endregion
		protected void CheckValid() {
			Owner.CheckValid();
		}
		void RaiseChanged() {
			Owner.RaiseChanged();
		}
	}
	#endregion
	#region NativeRangeFormatProtectionPart
	partial class NativeRangeFormatProtectionPart : NativeRangeFormatPartBase, Protection {
		PropertyAccessor<bool> hidden;
		PropertyAccessor<bool> locked;
		public NativeRangeFormatProtectionPart(NativeRangeFormat owner)
			: base(owner) {
			this.hidden = new ProtectionHiddenPropertyAccessor(ModelRange);
			this.locked = new ProtectionLockedPropertyAccessor(ModelRange);
		}
		#region Locked
		public bool Locked {
			get {
				CheckValid();
				return locked.GetValue();
			}
			set {
				CheckValid();
				if (locked.SetValue(value))
					RaiseChanged();
			}
		}
		#endregion
		#region Hidden
		public bool Hidden {
			get {
				CheckValid();
				return hidden.GetValue();
			}
			set {
				CheckValid();
				if (hidden.SetValue(value))
					RaiseChanged();
			}
		}
		#endregion
		protected void CheckValid() {
			Owner.CheckValid();
		}
		void RaiseChanged() {
			Owner.RaiseChanged();
		}
	}
	#endregion
	partial class NativeRangeFormatApplyPart : NativeRangeFormatPartBase, StyleFlags {
		PropertyAccessor<bool> all;
		PropertyAccessor<bool> number;
		PropertyAccessor<bool> alignment;
		PropertyAccessor<bool> font;
		PropertyAccessor<bool> borders;
		PropertyAccessor<bool> fill;
		PropertyAccessor<bool> protection;
		public NativeRangeFormatApplyPart(NativeRangeFormat owner)
			: base(owner) {
			this.all = new ApplyAllPropertyAccessor(ModelRange);
			this.number = new ApplyNumberPropertyAccessor(ModelRange);
			this.alignment = new ApplyAlignmentPropertyAccessor(ModelRange);
			this.font = new ApplyFontPropertyAccessor(ModelRange);
			this.borders = new ApplyBordersPropertyAccessor(ModelRange);
			this.fill = new ApplyFillPropertyAccessor(ModelRange);
			this.protection = new ApplyProtectionPropertyAccessor(ModelRange);
		}
		#region All
		public bool All {
			get {
				CheckValid();
				return all.GetValue();
			}
			set {
				CheckValid();
				if (all.SetValue(value))
					RaiseChanged();
			}
		}
		#endregion
		#region Number
		public bool Number {
			get {
				CheckValid();
				return number.GetValue();
			}
			set {
				CheckValid();
				if (number.SetValue(value))
					RaiseChanged();
			}
		}
		#endregion
		#region Alignment
		public bool Alignment {
			get {
				CheckValid();
				return alignment.GetValue();
			}
			set {
				CheckValid();
				if (alignment.SetValue(value))
					RaiseChanged();
			}
		}
		#endregion
		#region Font
		public bool Font {
			get {
				CheckValid();
				return font.GetValue();
			}
			set {
				CheckValid();
				if (font.SetValue(value))
					RaiseChanged();
			}
		}
		#endregion
		#region Borders
		public bool Borders {
			get {
				CheckValid();
				return borders.GetValue();
			}
			set {
				CheckValid();
				if (borders.SetValue(value))
					RaiseChanged();
			}
		}
		#endregion
		#region Fill
		public bool Fill {
			get {
				CheckValid();
				return fill.GetValue();
			}
			set {
				CheckValid();
				if (fill.SetValue(value))
					RaiseChanged();
			}
		}
		#endregion
		#region Protection
		public bool Protection {
			get {
				CheckValid();
				return protection.GetValue();
			}
			set {
				CheckValid();
				if (protection.SetValue(value))
					RaiseChanged();
			}
		}
		#endregion
		protected void CheckValid() {
			Owner.CheckValid();
		}
		void RaiseChanged() {
			Owner.RaiseChanged();
		}
	}
	#region NativeRangeFormatBordersPart
	partial class NativeRangeFormatBordersPart : NativeRangeFormatPartBase, Borders {
		NativeRangeFormatLeftBorderPart leftBorder;
		NativeRangeFormatRightBorderPart rightBorder;
		NativeRangeFormatTopBorderPart topBorder;
		NativeRangeFormatBottomBorderPart bottomBorder;
		NativeRangeFormatInsideHorizontalBorderPart insideHorizontalBorder;
		NativeRangeFormatInsideVerticalBorderPart insideVerticalBorder;
		PropertyAccessor<XlBorderLineStyle> lineStyleUp;
		PropertyAccessor<XlBorderLineStyle> lineStyleDown;
		PropertyAccessor<Color> diagonalColor;
		public NativeRangeFormatBordersPart(NativeRangeFormat owner)
			: base(owner) {
			this.leftBorder = new NativeRangeFormatLeftBorderPart(owner);
			this.rightBorder = new NativeRangeFormatRightBorderPart(owner);
			this.topBorder = new NativeRangeFormatTopBorderPart(owner);
			this.bottomBorder = new NativeRangeFormatBottomBorderPart(owner);
			this.insideHorizontalBorder = new NativeRangeFormatInsideHorizontalBorderPart(owner);
			this.insideVerticalBorder = new NativeRangeFormatInsideVerticalBorderPart(owner);
			this.lineStyleUp = new DiagonalUpBorderLineStylePropertyAccessor(ModelRange);
			this.lineStyleDown = new DiagonalDownBorderLineStylePropertyAccessor(ModelRange);
			this.diagonalColor = new DiagonalBorderColorPropertyAccessor(ModelRange);
		}
		public Border LeftBorder { get { return leftBorder; } }
		public Border RightBorder { get { return rightBorder; } }
		public Border TopBorder { get { return topBorder; } }
		public Border BottomBorder { get { return bottomBorder; } }
		public Border InsideHorizontalBorders { get { return insideHorizontalBorder; } }
		public Border InsideVerticalBorders { get { return insideVerticalBorder; } }
		public Color DiagonalBorderColor { get { return diagonalColor.GetValue(); } set { diagonalColor.SetValue(value); } }
		#region DiagonalBorderLineStyle
		public BorderLineStyle DiagonalBorderLineStyle {
			get {
				switch (this.DiagonalBorderType) {
					case Spreadsheet.DiagonalBorderType.None:
					case Spreadsheet.DiagonalBorderType.Up:
					case Spreadsheet.DiagonalBorderType.UpAndDown:
						return (BorderLineStyle)lineStyleUp.GetValue();
					default:
						return (BorderLineStyle)lineStyleDown.GetValue();
				}
			}
			set {
				Owner.BeginUpdate();
				try {
					if (DiagonalBorderType == Spreadsheet.DiagonalBorderType.None)
						DiagonalBorderType = Spreadsheet.DiagonalBorderType.UpAndDown;
					if (DiagonalBorderType == Spreadsheet.DiagonalBorderType.UpAndDown || DiagonalBorderType == Spreadsheet.DiagonalBorderType.Down)
						lineStyleDown.SetValue((XlBorderLineStyle)value);
					if (DiagonalBorderType == Spreadsheet.DiagonalBorderType.UpAndDown || DiagonalBorderType == Spreadsheet.DiagonalBorderType.Up)
						lineStyleUp.SetValue((XlBorderLineStyle)value);
				}
				finally {
					Owner.EndUpdate();
				}
			}
		}
		#endregion
		#region DiagonalBorderType
		public DiagonalBorderType DiagonalBorderType {
			get {
				if (lineStyleUp.GetValue() != XlBorderLineStyle.None && lineStyleDown.GetValue() != XlBorderLineStyle.None)
					return DiagonalBorderType.UpAndDown;
				if (lineStyleUp.GetValue() != XlBorderLineStyle.None)
					return DiagonalBorderType.Up;
				if (lineStyleDown.GetValue() != XlBorderLineStyle.None)
					return DiagonalBorderType.Down;
				else
					return DiagonalBorderType.None;
			}
			set {
				Owner.BeginUpdate();
				try {
					if (value == Spreadsheet.DiagonalBorderType.None) {
						lineStyleUp.SetValue(XlBorderLineStyle.None);
						lineStyleDown.SetValue(XlBorderLineStyle.None);
					}
					if (value == Spreadsheet.DiagonalBorderType.UpAndDown) {
						if (lineStyleDown.GetValue() == XlBorderLineStyle.None)
							lineStyleDown.SetValue(XlBorderLineStyle.Thin);
						if (lineStyleUp.GetValue() == XlBorderLineStyle.None)
							lineStyleUp.SetValue(XlBorderLineStyle.Thin);
					}
					if (value == Spreadsheet.DiagonalBorderType.Down) {
						if (lineStyleDown.GetValue() == XlBorderLineStyle.None)
							lineStyleDown.SetValue(XlBorderLineStyle.Thin);
						lineStyleUp.SetValue(XlBorderLineStyle.None);
					}
					if (value == Spreadsheet.DiagonalBorderType.Up) {
						if (lineStyleUp.GetValue() == XlBorderLineStyle.None)
							lineStyleUp.SetValue(XlBorderLineStyle.Thin);
						lineStyleDown.SetValue(XlBorderLineStyle.None);
					}
				}
				finally {
					Owner.EndUpdate();
				}
			}
		}
		#endregion
		public int OutlineLevel { get { return 0; } set { } }
		#region SetOutsideBorders
		public void SetOutsideBorders(Color color, BorderLineStyle style) {
			CheckValid();
			Owner.BeginUpdate();
			try {
				XlBorderLineStyle modelLineStyle = (XlBorderLineStyle)style;
				MergedBorderInfo info = MergedBorderInfo.CreateOutlineBorders(modelLineStyle, color);
				ChangeRangeBordersCommand command = new ChangeRangeBordersCommand(ModelRange, info);
				command.Execute();
			}
			finally {
				RaiseChanged();
				Owner.EndUpdate();
			}
		}
		#endregion
		public void SetInsideRangeBorders(Color color, BorderLineStyle style) {
			Owner.BeginUpdate();
			try {
				XlBorderLineStyle modelStyle = (XlBorderLineStyle)style;
				MergedBorderInfo info = MergedBorderInfo.CreateInsideBorders(modelStyle, color);
				ChangeRangeBordersCommand command = new ChangeRangeBordersCommand(this.ModelRange, info);
				command.Execute();
			}
			finally {
				Owner.EndUpdate();
			}
		}
		public void SetAllBorders(Color color, BorderLineStyle style) {
			Owner.BeginUpdate();
			try {
				XlBorderLineStyle modelLineStyle = (XlBorderLineStyle)style;
				MergedBorderInfo info = MergedBorderInfo.CreateAllBorders(modelLineStyle, color);
				ChangeRangeBordersCommand command = new ChangeRangeBordersCommand(this.ModelRange, info);
				command.Execute();
			}
			finally {
				Owner.EndUpdate();
			}
		}
		#region SetDiagonalBorders
		public void SetDiagonalBorders(Color color, BorderLineStyle style, DiagonalBorderType diaginalBorderType) {
			CheckValid();
			try {
				Owner.BeginUpdate();
				if (diagonalColor.SetValue(color)) {
					if ((diaginalBorderType == Spreadsheet.DiagonalBorderType.UpAndDown) && lineStyleUp.SetValue((XlBorderLineStyle)style) && lineStyleDown.SetValue((XlBorderLineStyle)style))
						RaiseChanged();
					else if ((diaginalBorderType == Spreadsheet.DiagonalBorderType.Up) && lineStyleUp.SetValue((XlBorderLineStyle)style))
						RaiseChanged();
					else if ((diaginalBorderType == Spreadsheet.DiagonalBorderType.Down) && lineStyleDown.SetValue((XlBorderLineStyle)style))
						RaiseChanged();
				}
			}
			finally {
				Owner.EndUpdate();
			}
		}
		#endregion
		public void SetInsideBorders(Color color, BorderLineStyle style) {
			Owner.BeginUpdate();
			try {
				XlBorderLineStyle modelLineStyle = (XlBorderLineStyle)style;
				MergedBorderInfo info = MergedBorderInfo.CreateInsideBorders(modelLineStyle, color);
				ChangeRangeBordersCommand command = new ChangeRangeBordersCommand(ModelRange, info);
				command.Execute();
			}
			finally {
				Owner.EndUpdate();
			}
		}
		public void RemoveBorders() {
			SetAllBorders(DXColor.Empty, BorderLineStyle.None);
		}
		protected void CheckValid() {
			Owner.CheckValid();
		}
		void RaiseChanged() {
			Owner.RaiseChanged();
		}
	}
	#endregion
	#region NativeRangeFormatLeftBorderPart
	partial class NativeRangeFormatLeftBorderPart : NativeRangeFormatPartBase, Border {
		PropertyAccessor<XlBorderLineStyle> lineStyle;
		PropertyAccessor<Color> color;
		public NativeRangeFormatLeftBorderPart(NativeRangeFormat owner)
			: base(owner) {
			this.lineStyle = new LeftBorderLineStylePropertyAccessor(ModelRange);
			this.color = new LeftBorderColorPropertyAccessor(ModelRange);
		}
		#region Color
		public Color Color {
			get {
				CheckValid();
				return color.GetValue();
			}
			set {
				CheckValid();
				if (color.SetValue(value))
					RaiseChanged();
			}
		}
		#endregion
		#region LineStyle
		public BorderLineStyle LineStyle {
			get {
				CheckValid();
				return (BorderLineStyle)lineStyle.GetValue();
			}
			set {
				CheckValid();
				if (lineStyle.SetValue((XlBorderLineStyle)value))
					RaiseChanged();
			}
		}
		#endregion
		protected void CheckValid() {
			Owner.CheckValid();
		}
		void RaiseChanged() {
			Owner.RaiseChanged();
		}
	}
	#endregion
	#region NativeRangeFormatRightBorderPart
	partial class NativeRangeFormatRightBorderPart : NativeRangeFormatPartBase, Border {
		PropertyAccessor<XlBorderLineStyle> lineStyle;
		PropertyAccessor<Color> color;
		public NativeRangeFormatRightBorderPart(NativeRangeFormat owner)
			: base(owner) {
			this.lineStyle = new RightBorderLineStylePropertyAccessor(ModelRange);
			this.color = new RightBorderColorPropertyAccessor(ModelRange);
		}
		#region Color
		public Color Color {
			get {
				CheckValid();
				return color.GetValue();
			}
			set {
				CheckValid();
				if (color.SetValue(value))
					RaiseChanged();
			}
		}
		#endregion
		#region LineStyle
		public BorderLineStyle LineStyle {
			get {
				CheckValid();
				return (BorderLineStyle)lineStyle.GetValue();
			}
			set {
				CheckValid();
				if (lineStyle.SetValue((XlBorderLineStyle)value))
					RaiseChanged();
			}
		}
		#endregion
		protected void CheckValid() {
			Owner.CheckValid();
		}
		void RaiseChanged() {
			Owner.RaiseChanged();
		}
	}
	#endregion
	#region NativeRangeFormatTopBorderPart
	partial class NativeRangeFormatTopBorderPart : NativeRangeFormatPartBase, Border {
		PropertyAccessor<XlBorderLineStyle> lineStyle;
		PropertyAccessor<Color> color;
		public NativeRangeFormatTopBorderPart(NativeRangeFormat owner)
			: base(owner) {
			this.lineStyle = new TopBorderLineStylePropertyAccessor(ModelRange);
			this.color = new TopBorderColorPropertyAccessor(ModelRange);
		}
		#region Color
		public Color Color {
			get {
				CheckValid();
				return color.GetValue();
			}
			set {
				CheckValid();
				if (color.SetValue(value))
					RaiseChanged();
			}
		}
		#endregion
		#region LineStyle
		public BorderLineStyle LineStyle {
			get {
				CheckValid();
				return (BorderLineStyle)lineStyle.GetValue();
			}
			set {
				CheckValid();
				if (lineStyle.SetValue((XlBorderLineStyle)value))
					RaiseChanged();
			}
		}
		#endregion
		protected void CheckValid() {
			Owner.CheckValid();
		}
		void RaiseChanged() {
			Owner.RaiseChanged();
		}
	}
	#endregion
	#region NativeRangeFormatBottomBorderPart
	partial class NativeRangeFormatBottomBorderPart : NativeRangeFormatPartBase, Border {
		PropertyAccessor<XlBorderLineStyle> lineStyle;
		PropertyAccessor<Color> color;
		public NativeRangeFormatBottomBorderPart(NativeRangeFormat owner)
			: base(owner) {
			this.lineStyle = new BottomBorderLineStylePropertyAccessor(ModelRange);
			this.color = new BottomBorderColorPropertyAccessor(ModelRange);
		}
		#region Color
		public Color Color {
			get {
				CheckValid();
				return color.GetValue();
			}
			set {
				CheckValid();
				if (color.SetValue(value))
					RaiseChanged();
			}
		}
		#endregion
		#region LineStyle
		public BorderLineStyle LineStyle {
			get {
				CheckValid();
				return (BorderLineStyle)lineStyle.GetValue();
			}
			set {
				CheckValid();
				if (lineStyle.SetValue((XlBorderLineStyle)value))
					RaiseChanged();
			}
		}
		#endregion
		protected void CheckValid() {
			Owner.CheckValid();
		}
		void RaiseChanged() {
			Owner.RaiseChanged();
		}
	}
	#endregion
	#region NativeRangeFormatHorizontalBorderPart
	partial class NativeRangeFormatInsideHorizontalBorderPart : NativeRangeFormatPartBase, Border {
		PropertyAccessor<XlBorderLineStyle> lineStyle;
		PropertyAccessor<Color> color;
		public NativeRangeFormatInsideHorizontalBorderPart(NativeRangeFormat owner)
			: base(owner) {
			this.lineStyle = new HorizontalInsideBordersLineStylePropertyAccessor(ModelRange);
			this.color = new HorizontalInsideBordersColorPropertyAccessor(ModelRange);
		}
		#region Color
		public Color Color {
			get {
				CheckValid();
				return color.GetValue();
			}
			set {
				CheckValid();
				if (color.SetValue(value))
					RaiseChanged();
			}
		}
		#endregion
		#region LineStyle
		public BorderLineStyle LineStyle {
			get {
				CheckValid();
				return (BorderLineStyle)lineStyle.GetValue();
			}
			set {
				CheckValid();
				if (lineStyle.SetValue((XlBorderLineStyle)value))
					RaiseChanged();
			}
		}
		#endregion
		protected void CheckValid() {
			Owner.CheckValid();
		}
		void RaiseChanged() {
			Owner.RaiseChanged();
		}
	}
	#endregion
	#region NativeRangeFormatVerticalBorderPart
	partial class NativeRangeFormatInsideVerticalBorderPart : NativeRangeFormatPartBase, Border {
		PropertyAccessor<XlBorderLineStyle> lineStyle;
		PropertyAccessor<Color> color;
		public NativeRangeFormatInsideVerticalBorderPart(NativeRangeFormat owner)
			: base(owner) {
			this.lineStyle = new InsideVerticalBorderLineStylePropertyAccessor(ModelRange);
			this.color = new InsideVerticalBorderColorPropertyAccessor(ModelRange);
		}
		#region Color
		public Color Color {
			get {
				CheckValid();
				return color.GetValue();
			}
			set {
				CheckValid();
				if (color.SetValue(value))
					RaiseChanged();
			}
		}
		#endregion
		#region LineStyle
		public BorderLineStyle LineStyle {
			get {
				CheckValid();
				return (BorderLineStyle)lineStyle.GetValue();
			}
			set {
				CheckValid();
				if (lineStyle.SetValue((XlBorderLineStyle)value))
					RaiseChanged();
			}
		}
		#endregion
		protected void CheckValid() {
			Owner.CheckValid();
		}
		void RaiseChanged() {
			Owner.RaiseChanged();
		}
	}
	#endregion
	#region NativeRangeFormatDiagonalUpBorderPart
	partial class NativeRangeFormatDiagonalUpBorderPart : NativeRangeFormatPartBase, Border {
		PropertyAccessor<XlBorderLineStyle> lineStyle;
		PropertyAccessor<Color> color;
		public NativeRangeFormatDiagonalUpBorderPart(NativeRangeFormat owner)
			: base(owner) {
			this.lineStyle = new DiagonalUpBorderLineStylePropertyAccessor(ModelRange);
			this.color = new DiagonalBorderColorPropertyAccessor(ModelRange);
		}
		#region Color
		public Color Color {
			get {
				CheckValid();
				return color.GetValue();
			}
			set {
				CheckValid();
				if (color.SetValue(value))
					RaiseChanged();
			}
		}
		#endregion
		#region LineStyle
		public BorderLineStyle LineStyle {
			get {
				CheckValid();
				return (BorderLineStyle)lineStyle.GetValue();
			}
			set {
				CheckValid();
				if (lineStyle.SetValue((XlBorderLineStyle)value))
					RaiseChanged();
			}
		}
		#endregion
		protected void CheckValid() {
			Owner.CheckValid();
		}
		void RaiseChanged() {
			Owner.RaiseChanged();
		}
	}
	#endregion
	#endregion
}
