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
#if SL
using System.Windows.Media;
#else
using System.Drawing;
#endif
namespace DevExpress.Spreadsheet {
	#region StyleFlags
	public interface StyleFlags {
		bool All { get; set; }
		bool Number { get; set; }
		bool Alignment { get; set; }
		bool Font { get; set; }
		bool Borders { get; set; }
		bool Fill { get; set; }
		bool Protection { get; set; }
	}
	#endregion
	#region Alignment
	public interface Alignment {
		SpreadsheetHorizontalAlignment Horizontal { get; set; }
		SpreadsheetVerticalAlignment Vertical { get; set; }
		bool WrapText { get; set; }
		bool ShrinkToFit { get; set; }
		int Indent { get; set; }
		int RotationAngle { get; set; }
	}
	#endregion
	#region Protection
	public interface Protection {
		bool Locked { get; set; }
		bool Hidden { get; set; }
	}
	#endregion
	#region BuiltInStyleId
	public enum BuiltInStyleId {
		Normal = 0, 
		Comma = 3, 
		Currency = 4, 
		Percent = 5, 
		Comma0 = 6, 
		Currency0 = 7, 
		Hyperlink = 8, 
		FollowedHyperlink = 9, 
		Note = 10,
		WarningText = 11,
		Emphasis1 = 12,
		Emphasis2 = 13,
		Emphasis3 = 14,
		Title = 15,
		Heading1 = 16,
		Heading2 = 17,
		Heading3 = 18,
		Heading4 = 19,
		Input = 20,
		Output = 21,
		Calculation = 22,
		CheckCell = 23,
		LinkedCell = 24,
		Total = 25,
		Good = 26,
		Bad = 27,
		Neutral = 28,
		Accent1 = 29,
		Accent1_20percent = 30,
		Accent1_40percent = 31,
		Accent1_60percent = 32,
		Accent2 = 33,
		Accent2_20percent = 34,
		Accent2_40percent = 35,
		Accent2_60percent = 36,
		Accent3 = 37,
		Accent3_20percent = 38,
		Accent3_40percent = 39,
		Accent3_60percent = 40,
		Accent4 = 41,
		Accent4_20percent = 42,
		Accent4_40percent = 43,
		Accent4_60percent = 44,
		Accent5 = 45,
		Accent5_20percent = 46,
		Accent5_40percent = 47,
		Accent5_60percent = 48,
		Accent6 = 49,
		Accent6_20percent = 50,
		Accent6_40percent = 51,
		Accent6_60percent = 52,
		Explanatory = 53,
		TableStyleLight1 = 54,
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Utils;
	using ModelCell = DevExpress.XtraSpreadsheet.Model.ICell;
	using DevExpress.Spreadsheet;
	using DevExpress.Office;
	using DevExpress.XtraSpreadsheet.Utils;
	using DevExpress.Export.Xl;
	using DevExpress.Compatibility.System.Drawing;
	#region NativeStyleFlags
	partial class NativeStyleFlags : StyleFlags {
		readonly IFormatBaseAccessor ownerFormatAccessor;
		public NativeStyleFlags(IFormatBaseAccessor formatAccessor) {
			this.ownerFormatAccessor = formatAccessor;
		}
		public bool All {
			get {
				return Number && Alignment && Font && Borders && Fill && Protection;
			}
			set {
				Number = value;
				Alignment = value;
				Font = value;
				Borders = value;
				Fill = value;
				Protection = value;
			}
		}
		Model.IFormatBaseBatchUpdateable ReadOnlyOwnerFormat { get { return ownerFormatAccessor.ReadOnlyFormat; } }
		Model.IFormatBaseBatchUpdateable ReadWriteOwnerFormat { get { return ownerFormatAccessor.ReadWriteFormat; } }
		public bool Number { get { return ReadOnlyOwnerFormat.ApplyNumberFormat; } set { ReadWriteOwnerFormat.ApplyNumberFormat = value; } }
		public bool Alignment { get { return ReadOnlyOwnerFormat.ApplyAlignment; } set { ReadWriteOwnerFormat.ApplyAlignment = value; } }
		public bool Font { get { return ReadOnlyOwnerFormat.ApplyFont; } set { ReadWriteOwnerFormat.ApplyFont = value; } }
		public bool Borders { get { return ReadOnlyOwnerFormat.ApplyBorder; } set { ReadWriteOwnerFormat.ApplyBorder = value; } }
		public bool Fill { get { return ReadOnlyOwnerFormat.ApplyFill; } set { ReadWriteOwnerFormat.ApplyFill = value; } }
		public bool Protection { get { return ReadOnlyOwnerFormat.ApplyProtection; } set { ReadWriteOwnerFormat.ApplyProtection = value; } }
	}
	#endregion
	#region NativeAlignment
	public struct NativeAlignment : Alignment {
		IBatchUpdateable owner;
		Model.ICellAlignmentInfo alignmentInfo;
		DocumentModelUnitConverter unitConverter;
		public NativeAlignment(IBatchUpdateable owner, Model.ICellAlignmentInfo alignmentInfo, DocumentModelUnitConverter unitConverter) {
			this.owner = owner;
			this.alignmentInfo = alignmentInfo;
			this.unitConverter = unitConverter;
		}
		Model.ICellAlignmentInfo AlignmentInfo { get { return alignmentInfo; } }
		public SpreadsheetHorizontalAlignment Horizontal {
			get { return (SpreadsheetHorizontalAlignment)AlignmentInfo.Horizontal; }
			set {
				owner.BeginUpdate();
				try {
					if (Horizontal != value) {
						if (AlignmentChangeHelper.ShouldResetIndentOnChangeHorizontalAlignment(value))
							AlignmentInfo.Indent = 0;
					}
					AlignmentInfo.Horizontal = (XlHorizontalAlignment)value;
				}
				finally {
					owner.EndUpdate();
				}
			}
		}
		public SpreadsheetVerticalAlignment Vertical { get { return (SpreadsheetVerticalAlignment)AlignmentInfo.Vertical; } set { AlignmentInfo.Vertical = (XlVerticalAlignment)value; } }
		public bool WrapText { get { return AlignmentInfo.WrapText; } set { AlignmentInfo.WrapText = value; } }
		public bool ShrinkToFit { get { return AlignmentInfo.ShrinkToFit; } set { AlignmentInfo.ShrinkToFit = value; } }
		public int Indent {
			get { return AlignmentInfo.Indent; }
			set {
				owner.BeginUpdate();
				try {
					if (Indent != value && value > 0) {
						if (AlignmentChangeHelper.ShouldResetHorizontalAlignmentOnIndentChange(Horizontal))
							Horizontal = SpreadsheetHorizontalAlignment.Left;
					}
					AlignmentInfo.Indent = (byte)value;
				}
				finally {
					owner.EndUpdate();
				}
			}
		}
		public bool JustifyDistributed {
			get { return AlignmentInfo.JustifyLastLine; }
			set {
				owner.BeginUpdate();
				try {
					if (value == true && !AlignmentInfo.JustifyLastLine) {
						Indent = 0;
						Horizontal = SpreadsheetHorizontalAlignment.Distributed;
					}
					AlignmentInfo.JustifyLastLine = value;
				}
				finally {
					owner.EndUpdate();
				}
			}
		}
		public int RotationAngle {
			get {
				return ModelToApiRotationAngleConverter.ConvertToApiValue(AlignmentInfo.TextRotation, unitConverter);
			}
			set {
				AlignmentInfo.TextRotation = ModelToApiRotationAngleConverter.ConvertToModelValue(value, unitConverter);
			}
		}
	}
	public static class AlignmentChangeHelper {
		public static bool ShouldResetIndentOnChangeHorizontalAlignment(SpreadsheetHorizontalAlignment value) {
			return (value != SpreadsheetHorizontalAlignment.Left && value != SpreadsheetHorizontalAlignment.Right && value != SpreadsheetHorizontalAlignment.Distributed);
		}
		public static bool ShouldResetHorizontalAlignmentOnIndentChange(SpreadsheetHorizontalAlignment newValue) {
			return (newValue != SpreadsheetHorizontalAlignment.Left && newValue != SpreadsheetHorizontalAlignment.Right && newValue != SpreadsheetHorizontalAlignment.Distributed);
		}
	}
	#endregion
	#region NativeActualAlignment
	public struct NativeActualAlignment : Alignment {
		readonly IFormatBaseAccessor ownerFormatAccessor;
		public NativeActualAlignment(IFormatBaseAccessor formatAccessor) {
			this.ownerFormatAccessor = formatAccessor;
		}
		Model.IFormatBaseBatchUpdateable ReadOnlyOwnerFormat { get { return ownerFormatAccessor.ReadOnlyFormat; } }
		Model.IFormatBaseBatchUpdateable ReadWriteOwnerFormat { get { return ownerFormatAccessor.ReadWriteFormat; } }
		Model.ICellAlignmentInfo ReadWriteAlignmentInfo { get { return ReadWriteOwnerFormat.Alignment; } }
		Model.IActualCellAlignmentInfo ReadOnlyActualAlignmentInfo { get { return ReadOnlyOwnerFormat.ActualAlignment; } }
		public SpreadsheetHorizontalAlignment Horizontal {
			get { return (SpreadsheetHorizontalAlignment)ReadOnlyActualAlignmentInfo.Horizontal; }
			set {
				ReadWriteOwnerFormat.BeginUpdate();
				try {
					if (Horizontal != value) {
						if (AlignmentChangeHelper.ShouldResetIndentOnChangeHorizontalAlignment(value))
							ReadWriteAlignmentInfo.Indent = 0;
					}
					ReadWriteAlignmentInfo.Horizontal = (XlHorizontalAlignment)value;
				}
				finally {
					ReadWriteOwnerFormat.EndUpdate();
				}
			}
		}
		public SpreadsheetVerticalAlignment Vertical { get { return (SpreadsheetVerticalAlignment)ReadOnlyActualAlignmentInfo.Vertical; } set { ReadWriteAlignmentInfo.Vertical = (XlVerticalAlignment)value; } }
		public bool WrapText { get { return ReadOnlyActualAlignmentInfo.WrapText; } set { ReadWriteAlignmentInfo.WrapText = value; } }
		public bool ShrinkToFit { get { return ReadOnlyActualAlignmentInfo.ShrinkToFit; } set { ReadWriteAlignmentInfo.ShrinkToFit = value; } }
		public int Indent {
			get { return ReadOnlyActualAlignmentInfo.Indent; }
			set {
				ReadWriteOwnerFormat.BeginUpdate();
				try {
					if (Indent != value && value > 0) {
						if (AlignmentChangeHelper.ShouldResetHorizontalAlignmentOnIndentChange(Horizontal))
							Horizontal = SpreadsheetHorizontalAlignment.Left;
					}
					ReadWriteAlignmentInfo.Indent = (byte)value;
				}
				finally {
					ReadWriteOwnerFormat.EndUpdate();
				}
			}
		}
		public bool JustifyDistributed {
			get { return ReadOnlyActualAlignmentInfo.JustifyLastLine; }
			set {
				ReadWriteOwnerFormat.BeginUpdate();
				try {
					if (value == true && !ReadWriteAlignmentInfo.JustifyLastLine) {
						Indent = 0;
						Horizontal = SpreadsheetHorizontalAlignment.Distributed;
					}
					ReadWriteAlignmentInfo.JustifyLastLine = value;
				}
				finally {
					ReadWriteOwnerFormat.EndUpdate();
				}
			}
		}
		public int RotationAngle {
			get {
				return ModelToApiRotationAngleConverter.ConvertToApiValue(ReadOnlyActualAlignmentInfo.TextRotation, ReadWriteOwnerFormat.DocumentModel.UnitConverter);
			}
			set {
				ReadWriteAlignmentInfo.TextRotation = ModelToApiRotationAngleConverter.ConvertToModelValue(value, ReadWriteOwnerFormat.DocumentModel.UnitConverter);
			}
		}
	}
	#endregion
	#region NativeProtection
	partial class NativeProtection : Protection {
		Model.ICellProtectionInfo protectionInfo;
		public NativeProtection(Model.ICellProtectionInfo protectionInfo) {
			this.protectionInfo = protectionInfo;
		}
		public bool Locked { get { return protectionInfo.Locked; } set { protectionInfo.Locked = value; } }
		public bool Hidden { get { return protectionInfo.Hidden; } set { protectionInfo.Hidden = value; } }
	}
	#endregion
	#region NativeProtection
	public struct NativeActualProtection : Protection {
		readonly IFormatBaseAccessor ownerFormatAccessor;
		public NativeActualProtection(IFormatBaseAccessor ownerFormatAccessor) {
			this.ownerFormatAccessor = ownerFormatAccessor;
		}
		Model.IFormatBaseBatchUpdateable ReadOnlyOwnerFormat { get { return ownerFormatAccessor.ReadOnlyFormat; } }
		Model.IFormatBaseBatchUpdateable ReadWriteOwnerFormat { get { return ownerFormatAccessor.ReadWriteFormat; } }
		Model.ICellProtectionInfo ReadWriteProtectionInfo { get { return ReadWriteOwnerFormat.Protection; } }
		Model.IActualCellProtectionInfo ReadOnlyActualProtectionInfo { get { return ReadOnlyOwnerFormat.ActualProtection; } }
		public bool Locked { get { return ReadOnlyActualProtectionInfo.Locked; } set { ReadWriteProtectionInfo.Locked = value; } }
		public bool Hidden { get { return ReadOnlyActualProtectionInfo.Hidden; } set { ReadWriteProtectionInfo.Hidden = value; } }
	}
	#endregion
	#region ActualNativeFill
	partial class NativeActualFill : Fill, IGradientFillAccessor {
		readonly IFormatBaseAccessor ownerFormatAccessor;
		NativeGradientFill gradientFill;
		public NativeActualFill(IFormatBaseAccessor ownerFormatAccessor) {
			this.ownerFormatAccessor = ownerFormatAccessor;
			this.gradientFill = new NativeGradientFill(this);
		}
		Model.IFormatBaseBatchUpdateable ReadOnlyOwnerFormat { get { return ownerFormatAccessor.ReadOnlyFormat; } }
		Model.IFormatBaseBatchUpdateable ReadWriteOwnerFormat { get { return ownerFormatAccessor.ReadWriteFormat; } }
		Model.IActualFillInfo ReadOnlyActualFillInfo { get { return ReadOnlyOwnerFormat.ActualFill; } }
		#region Fill Members
		public Color BackgroundColor {
			get {
				if (PatternType == Spreadsheet.PatternType.Solid)
					return ReadOnlyActualFillInfo.ForeColor;
				if (PatternType == Spreadsheet.PatternType.None)
					return DXColor.Empty;
				return ReadOnlyActualFillInfo.BackColor;
			}
			set {
				value = value.RemoveTransparency();
				Model.IFormatBaseBatchUpdateable owner = ReadWriteOwnerFormat;
				Model.IFillInfo fill = owner.Fill;
				owner.BeginUpdate();
				try {
					if (DXColor.IsTransparentOrEmpty(value)) {
						fill.PatternType = XlPatternType.None;
						fill.ForeColor = DXColor.Empty;
						fill.BackColor = DXColor.Empty;
						return;
					}
					if (fill.PatternType == XlPatternType.None)
						fill.PatternType = XlPatternType.Solid;
					if (fill.PatternType == XlPatternType.Solid)
						fill.ForeColor = value;
					else
						fill.BackColor = value;
				}
				finally {
					owner.EndUpdate();
				}
			}
		}
		public Color PatternColor {
			get {
				if (PatternType == Spreadsheet.PatternType.Solid)
					return ReadOnlyActualFillInfo.BackColor;
				return ReadOnlyActualFillInfo.ForeColor;
			}
			set {
				Model.IFormatBaseBatchUpdateable owner = ReadWriteOwnerFormat;
				Model.IFillInfo fill = owner.Fill;
				bool wasPatternTypeSolid = fill.PatternType == XlPatternType.Solid;
				bool wasPatternTypeNone = fill.PatternType == XlPatternType.None;
				owner.BeginUpdate();
				try {
					if (wasPatternTypeNone) {
						fill.PatternType = XlPatternType.Solid;
						fill.BackColor = value;
						return;
					}
					if (wasPatternTypeSolid)
						fill.BackColor = value;
					else
						fill.ForeColor = value;
				}
				finally {
					owner.EndUpdate();
				}
			}
		}
		public PatternType PatternType {
			get { return (PatternType)ReadOnlyActualFillInfo.PatternType; }
			set {
				Model.IFormatBaseBatchUpdateable owner = ReadWriteOwnerFormat;
				Model.IFillInfo fill = owner.Fill;
				owner.BeginUpdate();
				try {
					XlPatternType newValue = (XlPatternType)value;
					fill.PatternType = newValue;
					if (newValue != XlPatternType.None && newValue != XlPatternType.Solid) {
						Color tmp = fill.BackColor;
						fill.BackColor = fill.ForeColor;
						fill.ForeColor = tmp;
					}
				}
				finally {
					owner.EndUpdate();
				}
			}
		}
		public FillType FillType {
			get { return (FillType)ReadOnlyActualFillInfo.FillType; }
			set { ReadWriteOwnerFormat.Fill.FillType = (Model.ModelFillType)value; }
		}
		public GradientFill Gradient { get { return gradientFill; } }
		#endregion
		#region IGradientFillAccessor Members
		Model.IGradientFillInfo IDifferentialFormatGradientFillAccessor.ReadWriteInfo { get { return ReadWriteOwnerFormat.Fill.GradientFill; } }
		Model.IActualGradientFillInfo IGradientFillAccessor.ActualInfo { get { return ReadOnlyActualFillInfo.GradientFill; } }
		#endregion
	}
	#endregion
	public static class ModelToApiRotationAngleConverter {
		public static int ConvertToApiValue(int valueInModelUnits, DocumentModelUnitConverter unitConverter) {
			int value = unitConverter.ModelUnitsToDegree(valueInModelUnits);
			int corrected = (value > 90) ? 90 - value : value;
			return corrected;
		}
		public static int ConvertToModelValue(int value, DocumentModelUnitConverter unitConverter) {
			if (value < -90 || value > 90)
				SpreadsheetExceptions.ThrowArgumentOutOfRangeException(DevExpress.XtraSpreadsheet.Localization.XtraSpreadsheetStringId.Msg_ErrorIncorectRotationAngleValue, "value");
			int correctedValue = (value < 0) ? 90 - value : value;
			int valueInModelUnits = unitConverter.DegreeToModelUnits(correctedValue);
			return valueInModelUnits;
		}
	}
	#region NativeStyleCache
	partial class NativeStyleCache {
		Dictionary<int, NativeCellStyle> styleCache;
		public NativeStyleCache() {
			this.styleCache = new Dictionary<int, NativeCellStyle>();
		}
		public void Clear() {
			styleCache.Clear();
		}
	}
	#endregion
	#region IDifferentialFormatGradientFillAccessor
	public interface IDifferentialFormatGradientFillAccessor {
		Model.IGradientFillInfo ReadWriteInfo { get; }
	}
	#endregion
	#region IGradientFillAccessor
	public interface IGradientFillAccessor : IDifferentialFormatGradientFillAccessor {
		Model.IActualGradientFillInfo ActualInfo { get; }
	}
	#endregion
	#region NativeGradientFill
	partial class NativeGradientFill : GradientFill {
		IGradientFillAccessor accessor;
		NativeGradientStopCollection stops;
		public NativeGradientFill(IGradientFillAccessor accessor) {
			this.accessor = accessor;
			this.stops = new NativeGradientStopCollection(accessor);
		}
		#region Properties
		Model.IActualGradientFillInfo ActualInfo { get { return accessor.ActualInfo; } }
		Model.IGradientFillInfo ReadWriteInfo { get { return accessor.ReadWriteInfo; } }
		#endregion
		#region GradientFill Members
		public GradientFillType Type {
			get { return (GradientFillType)ActualInfo.Type; }
			set { ReadWriteInfo.Type = (Model.ModelGradientFillType)value; }
		}
		public double Degree {
			get { return ActualInfo.Degree; }
			set { ReadWriteInfo.Degree = value; }
		}
		public float RectangleLeft {
			get { return ActualInfo.Convergence.Left; }
			set { ReadWriteInfo.Convergence.Left = value; }
		}
		public float RectangleRight {
			get { return ActualInfo.Convergence.Right; }
			set { ReadWriteInfo.Convergence.Right = value; }
		}
		public float RectangleTop {
			get { return ActualInfo.Convergence.Top; }
			set { ReadWriteInfo.Convergence.Top = value; }
		}
		public float RectangleBottom {
			get { return ActualInfo.Convergence.Bottom; }
			set { ReadWriteInfo.Convergence.Bottom = value; }
		}
		public Spreadsheet.GradientStopCollection Stops { get { return stops; } }
		#endregion
	}
	#endregion
	#region NativeGradientStopCollection
	partial class NativeGradientStopCollection : GradientStopCollection {
		IGradientFillAccessor accessor;
		public NativeGradientStopCollection(IGradientFillAccessor accessor) {
			this.accessor = accessor;
		}
		#region Properties
		Model.IActualGradientStopCollection ActaulInfo { get { return accessor.ActualInfo.GradientStops; } }
		Model.IGradientStopCollection ReadWriteInfo { get { return accessor.ReadWriteInfo.GradientStops; } }
		#endregion
		#region GradientStopCollection Members
		public GradientStop this[int index] { get { return new NativeGradientStop(ActaulInfo[index]); } }
		public int Count { get { return ActaulInfo.Count; } }
		public void Add(double position, Color color) {
			ReadWriteInfo.Add(position, color);
		}
		public void Clear() {
			ReadWriteInfo.Clear();
		}
		public void RemoveAt(int index) {
			ReadWriteInfo.RemoveAt(index);
		}
		#endregion
	}
	#endregion
	#region NativeGradientStop
	partial class NativeGradientStop : GradientStop {
		readonly Model.IGradientStopInfo modelGradientStopInfo;
		public NativeGradientStop(Model.IGradientStopInfo modelGradientStopInfo) {
			this.modelGradientStopInfo = modelGradientStopInfo;
		}
		#region GradientStop Members
		public double Position { get { return modelGradientStopInfo.Position; } }
		public Color Color { get { return modelGradientStopInfo.Color; } }
		#endregion
	}
	#endregion
}
