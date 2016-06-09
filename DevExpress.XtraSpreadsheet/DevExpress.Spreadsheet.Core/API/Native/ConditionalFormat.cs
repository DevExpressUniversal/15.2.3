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
using DevExpress.Utils;
using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using System.Collections.Generic;
using System.Text;
using DevExpress.Office;
using DevExpress.Export.Xl;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
using DevExpress.XtraSpreadsheet.API.Native;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	#region NativeConditionalFormat
	partial class NativeConditionalFormat : Formatting {
		#region Fields
		readonly Model.FormulaConditionalFormatting formulaConditionalFormatting;
		NativeAlignment alignment;
		NativeFont font;
		NativeStyleBorders borders;
		NativeConditionalFormattingFill fill;
		NativeProtection protection;
		#endregion
		public NativeConditionalFormat(Model.FormulaConditionalFormatting formulaConditionalFormatting) {
			Guard.ArgumentNotNull(formulaConditionalFormatting, "formulaConditionalFormatting");
			this.formulaConditionalFormatting = formulaConditionalFormatting;
			this.alignment = new NativeAlignment(formulaConditionalFormatting, 
				formulaConditionalFormatting.Alignment, 
				formulaConditionalFormatting.DocumentModel.UnitConverter);
			this.font = new NativeFont(formulaConditionalFormatting, formulaConditionalFormatting.Font);
			this.borders = new NativeStyleBorders(formulaConditionalFormatting, formulaConditionalFormatting.Border);
			this.fill = new NativeConditionalFormattingFill(formulaConditionalFormatting, formulaConditionalFormatting.Fill);
			this.protection = new NativeProtection(formulaConditionalFormatting.Protection);
		}
		#region Properties
		public string NumberFormat { get { return formulaConditionalFormatting.FormatString; } set { formulaConditionalFormatting.FormatString = value; } }
		Alignment Formatting.Alignment { get { return alignment; } }
		public Spreadsheet.SpreadsheetFont Font { get { return font; } }
		public Borders Borders { get { return borders; } }
		public Fill Fill { get { return fill; } }
		Protection Formatting.Protection { get { return protection; } }
		StyleFlags Formatting.Flags { get { return null; } }
		#endregion
		#region BeginUpdate
		public void BeginUpdate() {
			formulaConditionalFormatting.BeginUpdate();
		}
		#endregion
		#region EndUpdate
		public void EndUpdate() {
			formulaConditionalFormatting.EndUpdate();
		}
		#endregion
	}
	#endregion
	#region NativeConditionalAlignment
	partial class NativeConditionalAlignment : Alignment {
		Model.ICellAlignmentInfo alignment;
		DocumentModelUnitConverter unitConverter;
		public NativeConditionalAlignment(Model.ICellAlignmentInfo alignment, DocumentModelUnitConverter unitConverter) {
			Guard.ArgumentNotNull(alignment, "alignment");
			Guard.ArgumentNotNull(unitConverter, "unitConverter");
			this.alignment = alignment;
			this.unitConverter = unitConverter;
		}
		public SpreadsheetHorizontalAlignment Horizontal { get { return (SpreadsheetHorizontalAlignment)alignment.Horizontal; } set { alignment.Horizontal = (XlHorizontalAlignment)value; } }
		public SpreadsheetVerticalAlignment Vertical { get { return (SpreadsheetVerticalAlignment)alignment.Vertical; } set { alignment.Vertical = (XlVerticalAlignment)value; } }
		public bool WrapText { get { return alignment.WrapText; } set { alignment.WrapText = value; } }
		public bool ShrinkToFit { get { return alignment.ShrinkToFit; } set { alignment.ShrinkToFit = value; } }
		public int Indent { get { return alignment.Indent; } set { alignment.Indent = (byte)value; } }
		public int RotationAngle {
			get { 
				return ModelToApiRotationAngleConverter.ConvertToApiValue(alignment.TextRotation, unitConverter);
			}
			set {
				alignment.TextRotation = ModelToApiRotationAngleConverter.ConvertToModelValue(value, unitConverter);
			}
		}
	}
	#endregion
	#region NativeFill
	partial class NativeConditionalFormattingFill : Fill, IDifferentialFormatGradientFillAccessor {
		IBatchUpdateable owner;
		Model.IFillInfo fillInfo;
		NativeDifferentialFormatGradientFill gradientFill; 
		public NativeConditionalFormattingFill(IBatchUpdateable owner, Model.IFillInfo fillInfo) {
			this.owner = owner;
			this.fillInfo = fillInfo;
			this.gradientFill = new NativeDifferentialFormatGradientFill(this);
		}
		Model.IFillInfo FillInfo { get { return fillInfo; } }
		#region Fill Members
		public Color BackgroundColor {
			get {
				if (PatternType == Spreadsheet.PatternType.None)
					return DXColor.Empty;
				return FillInfo.BackColor;
			}
			set {
				try {
					value = value.RemoveTransparency();
					bool wasPatternTypeNone = FillInfo.PatternType == XlPatternType.None;
					owner.BeginUpdate();
					if (wasPatternTypeNone) {
						FillInfo.PatternType = XlPatternType.Solid;
						FillInfo.BackColor = value;
						return;
					}
					FillInfo.BackColor = value;
				}
				finally {
					owner.EndUpdate();
				}
			}
		}
		public Color PatternColor {
			get {return FillInfo.ForeColor;}
			set {
				bool wasPatternTypeNone = FillInfo.PatternType == XlPatternType.None;
				if (wasPatternTypeNone) {
					FillInfo.PatternType = XlPatternType.Solid;
					FillInfo.ForeColor = value;
					return;
				}
				FillInfo.ForeColor = value;
			}
		}
		public PatternType PatternType {
			get { return (PatternType)FillInfo.PatternType; }
			set {
				try {
					XlPatternType newValue = (XlPatternType)value;
					owner.BeginUpdate();
					FillInfo.PatternType = newValue;
				}
				finally {
					owner.EndUpdate();
				}
			}
		}
		public FillType FillType {
			get { return (FillType)FillInfo.FillType; }
			set { FillInfo.FillType = (Model.ModelFillType)value; }
		}
		public GradientFill Gradient { get { return gradientFill; } }
		#endregion
		#region IGradientFillAccessor Members
		Model.IGradientFillInfo IDifferentialFormatGradientFillAccessor.ReadWriteInfo { get { return FillInfo.GradientFill; } }
		#endregion
	}
	#endregion
	#region NativeDifferentialFormatGradientFill
	partial class NativeDifferentialFormatGradientFill : GradientFill {
		IDifferentialFormatGradientFillAccessor accessor;
		NativeDifferentialFormatGradientStopCollection stops;
		public NativeDifferentialFormatGradientFill(IDifferentialFormatGradientFillAccessor accessor) {
			this.accessor = accessor;
			this.stops = new NativeDifferentialFormatGradientStopCollection(accessor);
		}
		#region Properties
		Model.IGradientFillInfo Info { get { return accessor.ReadWriteInfo; } }
		#endregion
		#region GradientFill Members
		public GradientFillType Type {
			get { return (GradientFillType)Info.Type; }
			set { Info.Type = (Model.ModelGradientFillType)value; }
		}
		public double Degree {
			get { return Info.Degree; }
			set { Info.Degree = value; }
		}
		public float RectangleLeft {
			get { return Info.Convergence.Left; }
			set { Info.Convergence.Left = value; }
		}
		public float RectangleRight {
			get { return Info.Convergence.Right; }
			set { Info.Convergence.Right = value; }
		}
		public float RectangleTop {
			get { return Info.Convergence.Top; }
			set { Info.Convergence.Top = value; }
		}
		public float RectangleBottom {
			get { return Info.Convergence.Bottom; }
			set { Info.Convergence.Bottom = value; }
		}
		public Spreadsheet.GradientStopCollection Stops { get { return stops; } }
		#endregion
	}
	#endregion
	#region NativeDifferentialFormatGradientStopCollection
	partial class NativeDifferentialFormatGradientStopCollection : Spreadsheet.GradientStopCollection {
		IDifferentialFormatGradientFillAccessor accessor;
		public NativeDifferentialFormatGradientStopCollection(IDifferentialFormatGradientFillAccessor accessor) {
			this.accessor = accessor;
		}
		Model.IGradientStopCollection Info { get { return accessor.ReadWriteInfo.GradientStops; } }
		#region GradientStopCollection Members
		public Spreadsheet.GradientStop this[int index] { get { return new NativeGradientStop(Info[index]); } }
		public int Count { get { return Info.Count; } }
		public void Add(double position, Color color) {
			Info.Add(position, color);
		}
		public void Clear() {
			Info.Clear();
		}
		public void RemoveAt(int index) {
			Info.RemoveAt(index);
		}
		#endregion
	}
	#endregion
}
