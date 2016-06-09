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
using DevExpress.Export.Xl;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.Spreadsheet {
	#region Borders
	public interface Borders {
		Border LeftBorder { get; }
		Border RightBorder { get; }
		Border TopBorder { get; }
		Border BottomBorder { get; }
		Border InsideHorizontalBorders { get; }
		Border InsideVerticalBorders { get; }
		Color DiagonalBorderColor { get; set; }
		BorderLineStyle DiagonalBorderLineStyle { get; set; }
		DiagonalBorderType DiagonalBorderType { get; set; }
		void SetOutsideBorders(Color color, BorderLineStyle style);
		void SetDiagonalBorders(Color color, BorderLineStyle style, DiagonalBorderType diaginalBorderType);
		void SetAllBorders(Color color, BorderLineStyle style);
		void RemoveBorders();
	}
	#endregion
	#region Border
	public interface Border {
		Color Color { get; set; }
		BorderLineStyle LineStyle { get; set; }
	}
	#endregion
	#region BorderLineStyle
	public enum BorderLineStyle {
		None = XlBorderLineStyle.None,
		Thin = XlBorderLineStyle.Thin,
		Medium = XlBorderLineStyle.Medium,
		Dashed = XlBorderLineStyle.Dashed,
		Dotted = XlBorderLineStyle.Dotted,
		Thick = XlBorderLineStyle.Thick,
		Double = XlBorderLineStyle.Double,
		Hair = XlBorderLineStyle.Hair,
		MediumDashed = XlBorderLineStyle.MediumDashed,
		DashDot = XlBorderLineStyle.DashDot,
		MediumDashDot = XlBorderLineStyle.MediumDashDot,
		DashDotDot = XlBorderLineStyle.DashDotDot,
		MediumDashDotDot = XlBorderLineStyle.MediumDashDotDot,
		SlantDashDot = XlBorderLineStyle.SlantDashDot,
	}
	#endregion
	#region DiaginalBorderType
	public enum DiagonalBorderType {
		None,
		Up,
		Down,
		UpAndDown
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	#region NativeStyleBorders
	partial class NativeStyleBorders : Borders {
		IBorderPropertiesAccessor formatAccessor;
		public NativeStyleBorders(IBatchUpdateable owner, Model.IBorderInfo borderInfo) {
			formatAccessor = new BorderStylePropertiesAccessor(owner, borderInfo);
		}
		Model.IBorderInfo BorderInfo { get { return formatAccessor.ReadOnlyBorderInfo; } }
		IBatchUpdateable Owner { get { return formatAccessor.ReadOnlyOwner; } }
		#region Properties
		#region LeftBorder
		public Border LeftBorder {
			get {
				return new NativeBorder(formatAccessor, NativeBorderType.Left);
			}
			set {
				Model.IBorderInfo border = BorderInfo;
				border.LeftColor = value.Color;
				border.LeftLineStyle = (XlBorderLineStyle)value.LineStyle;
			}
		}
		#endregion
		#region RightBorder
		public Border RightBorder {
			get {
				return new NativeBorder(formatAccessor, NativeBorderType.Right);
			}
			set {
				Model.IBorderInfo border = BorderInfo;
				border.RightColor = value.Color;
				border.RightLineStyle = (XlBorderLineStyle)value.LineStyle;
			}
		}
		#endregion
		#region TopBorder
		public Border TopBorder {
			get {
				return new NativeBorder(formatAccessor, NativeBorderType.Top);
			}
			set {
				Model.IBorderInfo border = BorderInfo;
				border.TopColor = value.Color;
				border.TopLineStyle = (XlBorderLineStyle)value.LineStyle;
			}
		}
		#endregion
		#region BottomBorder
		public Border BottomBorder {
			get {
				return new NativeBorder(formatAccessor, NativeBorderType.Bottom);
			}
			set {
				Model.IBorderInfo border = BorderInfo;
				border.BottomColor = value.Color;
				border.BottomLineStyle = (XlBorderLineStyle)value.LineStyle;
			}
		}
		#endregion
		#region DiagonalBorderColor
		public Color DiagonalBorderColor {
			get {
				return BorderInfo.DiagonalColor;
			}
			set {
				BorderInfo.DiagonalColor = value;
			}
		}
		#endregion
		#region DiagonalBorderLineStyle
		public BorderLineStyle DiagonalBorderLineStyle {
			get {
				if (DiagonalBorderType == Spreadsheet.DiagonalBorderType.Down)
					return (BorderLineStyle)BorderInfo.DiagonalDownLineStyle;
				return (BorderLineStyle)BorderInfo.DiagonalUpLineStyle;
			}
			set {
				IBatchUpdateable owner = Owner;
				Model.IBorderInfo border = BorderInfo;
				try {
					owner.BeginUpdate();
					if (DiagonalBorderType == Spreadsheet.DiagonalBorderType.None)
						DiagonalBorderType = Spreadsheet.DiagonalBorderType.UpAndDown;
					if (DiagonalBorderType == Spreadsheet.DiagonalBorderType.UpAndDown || DiagonalBorderType == Spreadsheet.DiagonalBorderType.Down)
						border.DiagonalDownLineStyle = (XlBorderLineStyle)value;
					if (DiagonalBorderType == Spreadsheet.DiagonalBorderType.UpAndDown || DiagonalBorderType == Spreadsheet.DiagonalBorderType.Up)
						border.DiagonalUpLineStyle = (XlBorderLineStyle)value;
				}
				finally {
					owner.EndUpdate();
				}
			}
		}
		#endregion
		#region DiaginalBorderType
		public DiagonalBorderType DiagonalBorderType {
			get {
				Model.IBorderInfo border = BorderInfo;
				if (border.DiagonalUpLineStyle != XlBorderLineStyle.None && border.DiagonalDownLineStyle != XlBorderLineStyle.None)
					return DiagonalBorderType.UpAndDown;
				if (border.DiagonalUpLineStyle != XlBorderLineStyle.None)
					return DiagonalBorderType.Up;
				if (border.DiagonalDownLineStyle != XlBorderLineStyle.None)
					return DiagonalBorderType.Down;
				else
					return DiagonalBorderType.None;
			}
			set {
				IBatchUpdateable owner = Owner;
				Model.IBorderInfo border = BorderInfo;
				owner.BeginUpdate();
				try {
					if (value == Spreadsheet.DiagonalBorderType.None) {
						border.DiagonalUpLineStyle = XlBorderLineStyle.None;
						border.DiagonalDownLineStyle = XlBorderLineStyle.None;
					}
					if (value == Spreadsheet.DiagonalBorderType.UpAndDown) {
						if (border.DiagonalDownLineStyle == XlBorderLineStyle.None)
							border.DiagonalDownLineStyle = XlBorderLineStyle.Thin;
						if (border.DiagonalUpLineStyle == XlBorderLineStyle.None)
							border.DiagonalUpLineStyle = XlBorderLineStyle.Thin;
					}
					if (value == Spreadsheet.DiagonalBorderType.Down) {
						if (border.DiagonalDownLineStyle == XlBorderLineStyle.None)
							border.DiagonalDownLineStyle = XlBorderLineStyle.Thin;
						border.DiagonalUpLineStyle = XlBorderLineStyle.None;
					}
					if (value == Spreadsheet.DiagonalBorderType.Up) {
						if (border.DiagonalUpLineStyle == XlBorderLineStyle.None)
							border.DiagonalUpLineStyle = XlBorderLineStyle.Thin;
						border.DiagonalDownLineStyle = XlBorderLineStyle.None;
					}
				}
				finally {
					owner.EndUpdate();
				}
			}
		}
		#endregion
		#region HorizontalBorder
		public Border InsideHorizontalBorders {
			get {
				return new NativeBorder(formatAccessor, NativeBorderType.InsideHorizontal);
			}
			set {
				Model.IBorderInfo border = BorderInfo;
				border.HorizontalColor = value.Color;
				border.HorizontalLineStyle = (XlBorderLineStyle)value.LineStyle;
			}
		}
		#endregion
		#region VerticalBorder
		public Border InsideVerticalBorders {
			get {
				return new NativeBorder(formatAccessor, NativeBorderType.InsideVertical);
			}
			set {
				Model.IBorderInfo border = BorderInfo;
				border.VerticalColor = value.Color;
				border.VerticalLineStyle = (XlBorderLineStyle)value.LineStyle;
			}
		}
		#endregion
		#region Outline
		public bool Outline {
			get {
				return BorderInfo.Outline;
			}
			set {
				BorderInfo.Outline = value;
			}
		}
		#endregion
		#endregion
		#region SetAllBorders
		public void SetAllBorders(Color color, BorderLineStyle style) {
			SetOutsideBorders(color, style);
		}
		#endregion
		#region SetOutsideBorders
		public void SetOutsideBorders(Color color, BorderLineStyle style) {
			IBatchUpdateable owner = Owner;
			Model.IBorderInfo border = BorderInfo;
			owner.BeginUpdate();
			try {
				border.TopColor = color;
				border.TopLineStyle = (XlBorderLineStyle)style;
				border.BottomColor = color;
				border.BottomLineStyle = (XlBorderLineStyle)style;
				border.LeftColor = color;
				border.LeftLineStyle = (XlBorderLineStyle)style;
				border.RightColor = color;
				border.RightLineStyle = (XlBorderLineStyle)style;
			}
			finally {
				owner.EndUpdate();
			}
		}
		#endregion
		#region SetDiagonalBorders
		public void SetDiagonalBorders(Color color, BorderLineStyle style, DiagonalBorderType diaginalBorderType) {
			if (diaginalBorderType == DiagonalBorderType.None)
				return;
			IBatchUpdateable owner = Owner;
			Model.IBorderInfo border = BorderInfo;
			owner.BeginUpdate();
			try {
				border.DiagonalColor = color;
				if (diaginalBorderType == DiagonalBorderType.UpAndDown) {
					border.DiagonalUpLineStyle = (XlBorderLineStyle)style;
					border.DiagonalDownLineStyle = (XlBorderLineStyle)style;
				}
				if (diaginalBorderType == DiagonalBorderType.Up) {
					border.DiagonalUpLineStyle = (XlBorderLineStyle)style;
					border.DiagonalDownLineStyle = XlBorderLineStyle.None;
				}
				if (diaginalBorderType == DiagonalBorderType.Down) {
					border.DiagonalUpLineStyle = XlBorderLineStyle.None;
					border.DiagonalDownLineStyle = (XlBorderLineStyle)style;
				}
			}
			finally {
				owner.EndUpdate();
			}
		}
		#endregion
		#region RemoveBorders
		public void RemoveBorders() {
			IBatchUpdateable owner = Owner;
			Model.IBorderInfo border = BorderInfo;
			owner.BeginUpdate();
			try {
				border.BottomColor = DXColor.Empty;
				border.BottomLineStyle = XlBorderLineStyle.None;
				border.DiagonalColor = DXColor.Empty;
				border.DiagonalDownLineStyle = XlBorderLineStyle.None;
				border.DiagonalUpLineStyle = XlBorderLineStyle.None;
				border.HorizontalColor = DXColor.Empty;
				border.HorizontalLineStyle = XlBorderLineStyle.None;
				border.LeftColor = DXColor.Empty;
				border.LeftLineStyle = XlBorderLineStyle.None;
				border.RightColor = DXColor.Empty;
				border.RightLineStyle = XlBorderLineStyle.None;
				border.TopColor = DXColor.Empty;
				border.TopLineStyle = XlBorderLineStyle.None;
				border.VerticalColor = DXColor.Empty;
				border.VerticalLineStyle = XlBorderLineStyle.None;
			}
			finally {
				owner.EndUpdate();
			}
		}
		#endregion
	}
	#endregion
	#region NativeActualBorders
	partial class NativeActualBorders : Borders {
		readonly IFormatBaseAccessor ownerFormatAccessor;
		readonly BorderActualPropertiesAccessor borderPropertiesAccessor;
		public NativeActualBorders(IFormatBaseAccessor ownerFormatAccessor) {
			this.ownerFormatAccessor = ownerFormatAccessor;
			borderPropertiesAccessor = new BorderActualPropertiesAccessor(ownerFormatAccessor);
		}
		Model.IFormatBaseBatchUpdateable ReadOnlyOwnerFormat { get { return ownerFormatAccessor.ReadOnlyFormat; } }
		Model.IFormatBaseBatchUpdateable ReadWriteOwnerFormat { get { return ownerFormatAccessor.ReadWriteFormat; } }
		Model.IBorderInfo ReadWriteBorderInfo { get { return ReadWriteOwnerFormat.Border; } }
		Model.IActualBorderInfo ReadOnlyActualBorderInfo { get { return ReadOnlyOwnerFormat.ActualBorder; } }
		#region Properties
		#region LeftBorder
		public Border LeftBorder {
			get {
				return new NativeActualBorder(borderPropertiesAccessor, ReadOnlyActualBorderInfo, NativeBorderType.Left);
			}
			set {
				ReadWriteBorderInfo.LeftColor = value.Color;
				ReadWriteBorderInfo.LeftLineStyle = (XlBorderLineStyle)value.LineStyle;
			}
		}
		#endregion
		#region RightBorder
		public Border RightBorder {
			get {
				return new NativeActualBorder(borderPropertiesAccessor, ReadOnlyActualBorderInfo, NativeBorderType.Right);
			}
			set {
				ReadWriteBorderInfo.RightColor = value.Color;
				ReadWriteBorderInfo.RightLineStyle = (XlBorderLineStyle)value.LineStyle;
			}
		}
		#endregion
		#region TopBorder
		public Border TopBorder {
			get {
				return new NativeActualBorder(borderPropertiesAccessor, ReadOnlyActualBorderInfo, NativeBorderType.Top);
			}
			set {
				ReadWriteBorderInfo.TopColor = value.Color;
				ReadWriteBorderInfo.TopLineStyle = (XlBorderLineStyle)value.LineStyle;
			}
		}
		#endregion
		#region BottomBorder
		public Border BottomBorder {
			get {
				return new NativeActualBorder(borderPropertiesAccessor, ReadOnlyActualBorderInfo, NativeBorderType.Bottom);
			}
			set {
				ReadWriteBorderInfo.BottomColor = value.Color;
				ReadWriteBorderInfo.BottomLineStyle = (XlBorderLineStyle)value.LineStyle;
			}
		}
		#endregion
		#region DiagonalBorderColor
		public Color DiagonalBorderColor {
			get {
				return ReadOnlyActualBorderInfo.DiagonalColor;
			}
			set {
				ReadWriteBorderInfo.DiagonalColor = value;
			}
		}
		#endregion
		#region DiagonalBorderLineStyle
		public BorderLineStyle DiagonalBorderLineStyle {
			get {
				switch (this.DiagonalBorderType) {
					case Spreadsheet.DiagonalBorderType.None:
					case Spreadsheet.DiagonalBorderType.Up:
					case Spreadsheet.DiagonalBorderType.UpAndDown:
						return (BorderLineStyle)ReadOnlyActualBorderInfo.DiagonalUpLineStyle;
					default:
						return (BorderLineStyle)ReadOnlyActualBorderInfo.DiagonalDownLineStyle;
				}
			}
			set {
				Model.IFormatBaseBatchUpdateable owner = ReadWriteOwnerFormat;
				Model.IBorderInfo border = owner.Border;
				try {
					owner.BeginUpdate();
					if (DiagonalBorderType == Spreadsheet.DiagonalBorderType.None)
						DiagonalBorderType = Spreadsheet.DiagonalBorderType.UpAndDown;
					if (DiagonalBorderType == Spreadsheet.DiagonalBorderType.UpAndDown || DiagonalBorderType == Spreadsheet.DiagonalBorderType.Down)
						border.DiagonalDownLineStyle = (XlBorderLineStyle)value;
					if (DiagonalBorderType == Spreadsheet.DiagonalBorderType.UpAndDown || DiagonalBorderType == Spreadsheet.DiagonalBorderType.Up)
						border.DiagonalUpLineStyle = (XlBorderLineStyle)value;
				}
				finally {
					owner.EndUpdate();
				}
			}
		}
		#endregion
		#region DiaginalBorderType
		public DiagonalBorderType DiagonalBorderType {
			get {
				if (ReadOnlyActualBorderInfo.DiagonalUpLineStyle != XlBorderLineStyle.None && ReadOnlyActualBorderInfo.DiagonalDownLineStyle != XlBorderLineStyle.None)
					return DiagonalBorderType.UpAndDown;
				if (ReadOnlyActualBorderInfo.DiagonalUpLineStyle != XlBorderLineStyle.None)
					return DiagonalBorderType.Up;
				if (ReadOnlyActualBorderInfo.DiagonalDownLineStyle != XlBorderLineStyle.None)
					return DiagonalBorderType.Down;
				else
					return DiagonalBorderType.None;
			}
			set {
				Model.IFormatBaseBatchUpdateable owner = ReadWriteOwnerFormat;
				Model.IBorderInfo border = owner.Border;
				owner.BeginUpdate();
				try {
					if (value == Spreadsheet.DiagonalBorderType.None) {
						border.DiagonalUpLineStyle = XlBorderLineStyle.None;
						border.DiagonalDownLineStyle = XlBorderLineStyle.None;
					}
					if (value == Spreadsheet.DiagonalBorderType.UpAndDown) {
						if (border.DiagonalDownLineStyle == XlBorderLineStyle.None)
							border.DiagonalDownLineStyle = XlBorderLineStyle.Thin;
						if (border.DiagonalUpLineStyle == XlBorderLineStyle.None)
							border.DiagonalUpLineStyle = XlBorderLineStyle.Thin;
					}
					if (value == Spreadsheet.DiagonalBorderType.Down) {
						if (border.DiagonalDownLineStyle == XlBorderLineStyle.None)
							ReadWriteBorderInfo.DiagonalDownLineStyle = XlBorderLineStyle.Thin;
						border.DiagonalUpLineStyle = XlBorderLineStyle.None;
					}
					if (value == Spreadsheet.DiagonalBorderType.Up) {
						if (border.DiagonalUpLineStyle == XlBorderLineStyle.None)
							border.DiagonalUpLineStyle = XlBorderLineStyle.Thin;
						border.DiagonalDownLineStyle = XlBorderLineStyle.None;
					}
				}
				finally {
					owner.EndUpdate();
				}
			}
		}
		#endregion
		#region HorizontalBorder
		public Border InsideHorizontalBorders {
			get {
				return new NativeActualBorder(borderPropertiesAccessor, ReadOnlyActualBorderInfo, NativeBorderType.InsideHorizontal);
			}
			set {
				ReadWriteBorderInfo.HorizontalColor = value.Color;
				ReadWriteBorderInfo.HorizontalLineStyle = (XlBorderLineStyle)value.LineStyle;
			}
		}
		#endregion
		#region VerticalBorder
		public Border InsideVerticalBorders {
			get {
				return new NativeActualBorder(borderPropertiesAccessor, ReadOnlyActualBorderInfo, NativeBorderType.InsideVertical);
			}
			set {
				ReadWriteBorderInfo.VerticalColor = value.Color;
				ReadWriteBorderInfo.VerticalLineStyle = (XlBorderLineStyle)value.LineStyle;
			}
		}
		#endregion
		#region Outline
		public bool Outline {
			get {
				return ReadOnlyActualBorderInfo.Outline;
			}
			set {
				ReadWriteBorderInfo.Outline = value;
			}
		}
		#endregion
		#endregion
		#region SetAllBorders
		public void SetAllBorders(Color color, BorderLineStyle style) {
			SetOutsideBorders(color, style);
		}
		#endregion
		#region SetOutsideBorders
		public void SetOutsideBorders(Color color, BorderLineStyle style) {
			Model.IFormatBaseBatchUpdateable owner = ReadWriteOwnerFormat;
			Model.IBorderInfo border = owner.Border;
			owner.BeginUpdate();
			try {
				border.TopColor = color;
				border.TopLineStyle = (XlBorderLineStyle)style;
				border.BottomColor = color;
				border.BottomLineStyle = (XlBorderLineStyle)style;
				border.LeftColor = color;
				border.LeftLineStyle = (XlBorderLineStyle)style;
				border.RightColor = color;
				border.RightLineStyle = (XlBorderLineStyle)style;
			}
			finally {
				owner.EndUpdate();
			}
		}
		#endregion
		#region SetDiagonalBorders
		public void SetDiagonalBorders(Color color, BorderLineStyle style, DiagonalBorderType diaginalBorderType) {
			if (diaginalBorderType == DiagonalBorderType.None)
				return;
			Model.IFormatBaseBatchUpdateable owner = ReadWriteOwnerFormat;
			Model.IBorderInfo border = owner.Border;
			owner.BeginUpdate();
			try {
				border.DiagonalColor = color;
				if (diaginalBorderType == DiagonalBorderType.UpAndDown) {
					border.DiagonalUpLineStyle = (XlBorderLineStyle)style;
					border.DiagonalDownLineStyle = (XlBorderLineStyle)style;
				}
				if (diaginalBorderType == DiagonalBorderType.Up) {
					border.DiagonalUpLineStyle = (XlBorderLineStyle)style;
					border.DiagonalDownLineStyle = XlBorderLineStyle.None;
				}
				if (diaginalBorderType == DiagonalBorderType.Down) {
					border.DiagonalUpLineStyle = XlBorderLineStyle.None;
					border.DiagonalDownLineStyle = (XlBorderLineStyle)style;
				}
			}
			finally {
				owner.EndUpdate();
			}
		}
		#endregion
		public void RemoveBorders() {
			Model.IFormatBaseBatchUpdateable owner = ReadWriteOwnerFormat;
			Model.IBorderInfo border = owner.Border;
			try {
				owner.BeginUpdate();
				SetAllBorders(DXColor.Empty, BorderLineStyle.None);
				border.DiagonalColor = DXColor.Empty;
				border.DiagonalDownLineStyle = XlBorderLineStyle.None;
				border.DiagonalUpLineStyle = XlBorderLineStyle.None;
				border.HorizontalColor = DXColor.Empty;
				border.HorizontalLineStyle = XlBorderLineStyle.None;
				border.VerticalColor = DXColor.Empty;
				border.VerticalLineStyle = XlBorderLineStyle.None;
			}
			finally {
				owner.EndUpdate();
			}
		}
	}
	#endregion
	#region NativeBorderBase
	public abstract class NativeBorderBase : Border {
		readonly IBorderPropertiesAccessor formatAccessor;
		readonly NativeBorderType borderType;
		readonly DiagonalBorderType diaginalBorderType;
		protected NativeBorderBase(IBorderPropertiesAccessor formatAccessor, NativeBorderType borderType) {
			this.formatAccessor = formatAccessor;
			this.borderType = borderType;
		}
		protected NativeBorderBase(IBorderPropertiesAccessor formatAccessor, NativeBorderType borderType, DiagonalBorderType diaginalBorderType)
			: this(formatAccessor, borderType) {
			this.diaginalBorderType = diaginalBorderType;
		}
		public abstract Color Color { get; set; }
		public abstract BorderLineStyle LineStyle { get; set; }
		protected internal IBatchUpdateable ReadOnlyOwner { get { return formatAccessor.ReadOnlyOwner; } }
		protected internal IBatchUpdateable ReadWriteOwner { get { return formatAccessor.ReadWriteOwner; } }
		protected internal Model.IBorderInfo ReadOnlyBorderInfo { get { return formatAccessor.ReadOnlyBorderInfo; } }
		protected internal Model.IBorderInfo ReadWriteBorderInfo { get { return formatAccessor.ReadWriteBorderInfo; } }
		protected IBorderPropertiesAccessor FormatAccessor { get { return formatAccessor; } }
		public NativeBorderType BorderType { get { return borderType; } }
		public DiagonalBorderType DiaginalBorderType { get { return diaginalBorderType; } }
		protected void SetColor(Color value) {
			IBatchUpdateable owner = ReadWriteOwner;
			Model.IBorderInfo border = ReadWriteBorderInfo;
			owner.BeginUpdate();
			bool changed = false;
			try {
				switch (borderType) {
					case NativeBorderType.Bottom:
						if (border.BottomColor != value) {
							border.BottomColor = value;
							changed = true;
						}
						break;
					case NativeBorderType.Diagonal:
						if (border.DiagonalColor != value) {
							border.DiagonalColor = value;
							changed = true;
						}
						break;
					case NativeBorderType.InsideHorizontal:
						if (border.HorizontalColor != value) {
							border.HorizontalColor = value;
							changed = true;
						}
						break;
					case NativeBorderType.Left:
						if (border.LeftColor != value) {
							border.LeftColor = value;
							changed = true;
						}
						break;
					case NativeBorderType.Right:
						if (border.RightColor != value) {
							border.RightColor = value;
							changed = true;
						}
						break;
					case NativeBorderType.Top:
						if (border.TopColor != value) {
							border.TopColor = value;
							changed = true;
						}
						break;
					default:
						if (border.VerticalColor != value) {
							border.VerticalColor = value;
							changed = true;
						}
						break;
				}
				if (LineStyle == BorderLineStyle.None && changed)
					SetLineStyle(BorderLineStyle.Thin);
			}
			finally {
				owner.EndUpdate();
			}
		}
		protected void SetLineStyle(BorderLineStyle value) {
			Model.IBorderInfo border = ReadWriteBorderInfo;
			switch (borderType) {
				case NativeBorderType.Bottom:
					border.BottomLineStyle = (XlBorderLineStyle)value;
					break;
				case NativeBorderType.Diagonal:
					if (diaginalBorderType == DiagonalBorderType.Up || diaginalBorderType == DiagonalBorderType.UpAndDown)
						border.DiagonalUpLineStyle = (XlBorderLineStyle)value;
					else
						border.DiagonalDownLineStyle = (XlBorderLineStyle)value;
					break;
				case NativeBorderType.InsideHorizontal:
					border.HorizontalLineStyle = (XlBorderLineStyle)value;
					break;
				case NativeBorderType.Left:
					border.LeftLineStyle = (XlBorderLineStyle)value;
					break;
				case NativeBorderType.Right:
					border.RightLineStyle = (XlBorderLineStyle)value;
					break;
				case NativeBorderType.Top:
					border.TopLineStyle = (XlBorderLineStyle)value;
					break;
				default:
					border.VerticalLineStyle = (XlBorderLineStyle)value;
					break;
			}
		}
	}
	#endregion
	#region NativeBorder
	partial class NativeBorder : NativeBorderBase {
		public NativeBorder(IBorderPropertiesAccessor formatAccessor, NativeBorderType borderType)
			: base(formatAccessor, borderType) {
		}
		public NativeBorder(IBorderPropertiesAccessor formatAccessor, NativeBorderType borderType, DiagonalBorderType diagonalBorderType)
			: base(formatAccessor, borderType, diagonalBorderType) {
		}
		public override Color Color {
			get {
				switch (BorderType) {
					case NativeBorderType.Bottom:
						return ReadOnlyBorderInfo.BottomColor;
					case NativeBorderType.Diagonal:
						return ReadOnlyBorderInfo.DiagonalColor;
					case NativeBorderType.InsideHorizontal:
						return ReadOnlyBorderInfo.HorizontalColor;
					case NativeBorderType.Left:
						return ReadOnlyBorderInfo.LeftColor;
					case NativeBorderType.Right:
						return ReadOnlyBorderInfo.RightColor;
					case NativeBorderType.Top:
						return ReadOnlyBorderInfo.TopColor;
					default:
						return ReadOnlyBorderInfo.VerticalColor;
				}
			}
			set {
				SetColor(value);
			}
		}
		public override BorderLineStyle LineStyle {
			get {
				switch (BorderType) {
					case NativeBorderType.Bottom:
						return (BorderLineStyle)ReadOnlyBorderInfo.BottomLineStyle;
					case NativeBorderType.Diagonal:
						if (DiaginalBorderType == DiagonalBorderType.Up || DiaginalBorderType == DiagonalBorderType.UpAndDown)
							return (BorderLineStyle)ReadOnlyBorderInfo.DiagonalUpLineStyle;
						else
							return (BorderLineStyle)ReadOnlyBorderInfo.DiagonalDownLineStyle;
					case NativeBorderType.InsideHorizontal:
						return (BorderLineStyle)ReadOnlyBorderInfo.HorizontalLineStyle;
					case NativeBorderType.Left:
						return (BorderLineStyle)ReadOnlyBorderInfo.LeftLineStyle;
					case NativeBorderType.Right:
						return (BorderLineStyle)ReadOnlyBorderInfo.RightLineStyle;
					case NativeBorderType.Top:
						return (BorderLineStyle)ReadOnlyBorderInfo.TopLineStyle;
					default:
						return (BorderLineStyle)ReadOnlyBorderInfo.VerticalLineStyle;
				}
			}
			set {
				SetLineStyle(value);
			}
		}
	}
	#endregion
	#region NativeActualBorder
	partial class NativeActualBorder : NativeBorderBase {
		public NativeActualBorder(BorderActualPropertiesAccessor formatAccessor, Model.IActualBorderInfo readOnlyActualBorderInfo, NativeBorderType borderType)
			: base(formatAccessor, borderType) {
		}
		Model.IActualBorderInfo ReadOnlyActualBorderInfo { get { return ((BorderActualPropertiesAccessor)this.FormatAccessor).ReadOnlyActualBorderInfo; } }
		public override Color Color {
			get {
				switch (BorderType) {
					case NativeBorderType.Bottom:
						return ReadOnlyActualBorderInfo.BottomColor;
					case NativeBorderType.Diagonal:
						return ReadOnlyActualBorderInfo.DiagonalColor;
					case NativeBorderType.InsideHorizontal:
						return ReadOnlyActualBorderInfo.HorizontalColor;
					case NativeBorderType.Left:
						return ReadOnlyActualBorderInfo.LeftColor;
					case NativeBorderType.Right:
						return ReadOnlyActualBorderInfo.RightColor;
					case NativeBorderType.Top:
						return ReadOnlyActualBorderInfo.TopColor;
					default:
						return ReadOnlyActualBorderInfo.VerticalColor;
				}
			}
			set {
				SetColor(value);
			}
		}
		public override BorderLineStyle LineStyle {
			get {
				switch (BorderType) {
					case NativeBorderType.Bottom:
						return (BorderLineStyle)ReadOnlyActualBorderInfo.BottomLineStyle;
					case NativeBorderType.Diagonal:
						if (DiaginalBorderType == DiagonalBorderType.Up || DiaginalBorderType == DiagonalBorderType.UpAndDown)
							return (BorderLineStyle)ReadOnlyActualBorderInfo.DiagonalUpLineStyle;
						else
							return (BorderLineStyle)ReadOnlyActualBorderInfo.DiagonalDownLineStyle;
					case NativeBorderType.InsideHorizontal:
						return (BorderLineStyle)ReadOnlyActualBorderInfo.HorizontalLineStyle;
					case NativeBorderType.Left:
						return (BorderLineStyle)ReadOnlyActualBorderInfo.LeftLineStyle;
					case NativeBorderType.Right:
						return (BorderLineStyle)ReadOnlyActualBorderInfo.RightLineStyle;
					case NativeBorderType.Top:
						return (BorderLineStyle)ReadOnlyActualBorderInfo.TopLineStyle;
					default:
						return (BorderLineStyle)ReadOnlyActualBorderInfo.VerticalLineStyle;
				}
			}
			set {
				SetLineStyle(value);
			}
		}
	}
	#endregion
	#region NativeBorderType
	public enum NativeBorderType {
		Left,
		Right,
		Top,
		Bottom,
		Diagonal,
		InsideHorizontal,
		InsideVertical
	}
	#endregion
	#region IBorderPropertiesAccessor
	public interface IBorderPropertiesAccessor {
		IBatchUpdateable ReadOnlyOwner { get; }
		IBatchUpdateable ReadWriteOwner { get; }
		Model.IBorderInfo ReadOnlyBorderInfo { get; }
		Model.IBorderInfo ReadWriteBorderInfo { get; }
	}
	#endregion
	public class BorderStylePropertiesAccessor : IBorderPropertiesAccessor {
		readonly IBatchUpdateable owner;
		readonly Model.IBorderInfo borderInfo;
		public BorderStylePropertiesAccessor(IBatchUpdateable owner, Model.IBorderInfo borderInfo) {
			this.owner = owner;
			this.borderInfo = borderInfo;
		}
		#region IBorderPropertiesAccessor Members
		public IBatchUpdateable ReadOnlyOwner { get { return owner; } }
		public IBatchUpdateable ReadWriteOwner { get { return owner; } }
		public Model.IBorderInfo ReadOnlyBorderInfo { get { return borderInfo; } }
		public Model.IBorderInfo ReadWriteBorderInfo { get { return borderInfo; } }
		#endregion
	}
	public class BorderActualPropertiesAccessor : IBorderPropertiesAccessor {
		readonly IFormatBaseAccessor formatBaseAccessor;
		public BorderActualPropertiesAccessor(IFormatBaseAccessor formatBaseAccessor) {
			this.formatBaseAccessor= formatBaseAccessor;
		}
		#region IBorderPropertiesAccessor Members
		public IBatchUpdateable ReadOnlyOwner { get { return formatBaseAccessor.ReadOnlyFormat; } }
		public IBatchUpdateable ReadWriteOwner { get { return formatBaseAccessor.ReadWriteFormat; } }
		public Model.IBorderInfo ReadOnlyBorderInfo { get { return formatBaseAccessor.ReadOnlyFormat.Border; } }
		public Model.IBorderInfo ReadWriteBorderInfo { get { return formatBaseAccessor.ReadWriteFormat.Border; } }
		public Model.IActualBorderInfo ReadOnlyActualBorderInfo { get { return formatBaseAccessor.ReadOnlyFormat.ActualBorder; } }
		#endregion
	}
}
