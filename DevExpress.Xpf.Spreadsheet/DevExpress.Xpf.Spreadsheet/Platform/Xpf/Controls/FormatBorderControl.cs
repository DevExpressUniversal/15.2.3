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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.XtraSpreadsheet.Model;
using DrawingColor = System.Drawing.Color;
using DevExpress.Xpf.Spreadsheet.Extensions.Internal;
using DevExpress.Export.Xl;
namespace DevExpress.Xpf.Spreadsheet.Internal {
	public class FormatBorderControl : Control {
		public static readonly DependencyProperty TopColorProperty;
		public static readonly DependencyProperty BottomColorProperty;
		public static readonly DependencyProperty LeftColorProperty;
		public static readonly DependencyProperty RightColorProperty;
		public static readonly DependencyProperty DiagonalColorProperty;
		public static readonly DependencyProperty VerticalColorProperty;
		public static readonly DependencyProperty HorizontalColorProperty;
		public static readonly DependencyProperty ShowTopBorderProperty;
		public static readonly DependencyProperty ShowBottomBorderProperty;
		public static readonly DependencyProperty ShowLeftBorderProperty;
		public static readonly DependencyProperty ShowRightBorderProperty;
		public static readonly DependencyProperty ShowDiagonalDownBorderProperty;
		public static readonly DependencyProperty ShowDiagonalUpBorderProperty;
		public static readonly DependencyProperty ShowVerticalInsideBorderProperty;
		public static readonly DependencyProperty ShowHorizontalInsideBorderProperty;
		public static readonly DependencyProperty TopBorderStyleProperty;
		public static readonly DependencyProperty BottomBorderStyleProperty;
		public static readonly DependencyProperty LeftBorderStyleProperty;
		public static readonly DependencyProperty RightBorderStyleProperty;
		public static readonly DependencyProperty DiagonalDownBorderStyleProperty;
		public static readonly DependencyProperty DiagonalUpBorderStyleProperty;
		public static readonly DependencyProperty VerticalBorderStyleProperty;
		public static readonly DependencyProperty HorizontalBorderStyleProperty;
		public static readonly DependencyProperty RangeTypeProperty;
		public static readonly DependencyProperty EnableVerticalBorderCheckStateProperty;
		public static readonly DependencyProperty EnableHorizontalBorderCheckStateProperty;
		static FormatBorderControl() {
			Type ownerType = typeof(FormatBorderControl);
			TopColorProperty = DependencyProperty.Register("TopColor", typeof(DrawingColor), ownerType,
				new FrameworkPropertyMetadata(DrawingColor.FromArgb(0, 0, 0, 0), FrameworkPropertyMetadataOptions.AffectsArrange));
			BottomColorProperty = DependencyProperty.Register("BottomColor", typeof(DrawingColor), ownerType,
				new FrameworkPropertyMetadata(DrawingColor.FromArgb(0, 0, 0, 0), FrameworkPropertyMetadataOptions.AffectsArrange));
			LeftColorProperty = DependencyProperty.Register("LeftColor", typeof(DrawingColor), ownerType,
				new FrameworkPropertyMetadata(DrawingColor.FromArgb(0, 0, 0, 0), FrameworkPropertyMetadataOptions.AffectsArrange));
			RightColorProperty = DependencyProperty.Register("RightColor", typeof(DrawingColor), ownerType,
				new FrameworkPropertyMetadata(DrawingColor.FromArgb(0, 0, 0, 0), FrameworkPropertyMetadataOptions.AffectsArrange));
			DiagonalColorProperty = DependencyProperty.Register("DiagonalColor", typeof(DrawingColor), ownerType,
				new FrameworkPropertyMetadata(DrawingColor.FromArgb(0, 0, 0, 0), FrameworkPropertyMetadataOptions.AffectsArrange));
			VerticalColorProperty = DependencyProperty.Register("VerticalColor", typeof(DrawingColor), ownerType,
				new FrameworkPropertyMetadata(DrawingColor.FromArgb(0, 0, 0, 0), FrameworkPropertyMetadataOptions.AffectsArrange));
			HorizontalColorProperty = DependencyProperty.Register("HorizontalColor", typeof(DrawingColor), ownerType,
				new FrameworkPropertyMetadata(DrawingColor.FromArgb(0, 0, 0, 0), FrameworkPropertyMetadataOptions.AffectsArrange));
			ShowTopBorderProperty = DependencyProperty.Register("ShowTopBorder", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange));
			ShowBottomBorderProperty = DependencyProperty.Register("ShowBottomBorder", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange));
			ShowLeftBorderProperty = DependencyProperty.Register("ShowLeftBorder", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange));
			ShowRightBorderProperty = DependencyProperty.Register("ShowRightBorder", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange));
			ShowDiagonalDownBorderProperty = DependencyProperty.Register("ShowDiagonalDownBorder", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange));
			ShowDiagonalUpBorderProperty = DependencyProperty.Register("ShowDiagonalUpBorder", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange));
			ShowVerticalInsideBorderProperty = DependencyProperty.Register("ShowVerticalInsideBorder", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange));
			ShowHorizontalInsideBorderProperty = DependencyProperty.Register("ShowHorizontalInsideBorder", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange));
			TopBorderStyleProperty = DependencyProperty.Register("TopBorderStyle", typeof(XlBorderLineStyle), ownerType,
				new FrameworkPropertyMetadata(XlBorderLineStyle.None, FrameworkPropertyMetadataOptions.AffectsArrange));
			BottomBorderStyleProperty = DependencyProperty.Register("BottomBorderStyle", typeof(XlBorderLineStyle), ownerType,
				new FrameworkPropertyMetadata(XlBorderLineStyle.None, FrameworkPropertyMetadataOptions.AffectsArrange));
			LeftBorderStyleProperty = DependencyProperty.Register("LeftBorderStyle", typeof(XlBorderLineStyle), ownerType,
				new FrameworkPropertyMetadata(XlBorderLineStyle.None, FrameworkPropertyMetadataOptions.AffectsArrange));
			RightBorderStyleProperty = DependencyProperty.Register("RightBorderStyle", typeof(XlBorderLineStyle), ownerType,
				new FrameworkPropertyMetadata(XlBorderLineStyle.None, FrameworkPropertyMetadataOptions.AffectsArrange));
			DiagonalDownBorderStyleProperty = DependencyProperty.Register("DiagonalDownBorderStyle", typeof(XlBorderLineStyle), ownerType,
				new FrameworkPropertyMetadata(XlBorderLineStyle.None, FrameworkPropertyMetadataOptions.AffectsArrange));
			DiagonalUpBorderStyleProperty = DependencyProperty.Register("DiagonalUpBorderStyle", typeof(XlBorderLineStyle), ownerType,
				new FrameworkPropertyMetadata(XlBorderLineStyle.None, FrameworkPropertyMetadataOptions.AffectsArrange));
			VerticalBorderStyleProperty = DependencyProperty.Register("VerticalBorderStyle", typeof(XlBorderLineStyle), ownerType,
				new FrameworkPropertyMetadata(XlBorderLineStyle.None, FrameworkPropertyMetadataOptions.AffectsArrange));
			HorizontalBorderStyleProperty = DependencyProperty.Register("HorizontalBorderStyle", typeof(XlBorderLineStyle), ownerType,
				new FrameworkPropertyMetadata(XlBorderLineStyle.None, FrameworkPropertyMetadataOptions.AffectsArrange));
			RangeTypeProperty = DependencyProperty.Register("RangeType", typeof(SelectedRangeTypeForBorderPreview), ownerType,
				new FrameworkPropertyMetadata(SelectedRangeTypeForBorderPreview.Cell, FrameworkPropertyMetadataOptions.AffectsArrange));
			EnableVerticalBorderCheckStateProperty = DependencyProperty.Register("EnableVerticalBorderCheckState", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange));
			EnableHorizontalBorderCheckStateProperty = DependencyProperty.Register("EnableHorizontalBorderCheckState", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange));
		}
		public FormatBorderControl() {
			DefaultStyleKey = typeof(FormatBorderControl);
		}
		public DrawingColor TopColor {
			get { return (DrawingColor)GetValue(TopColorProperty); }
			set { SetValue(TopColorProperty, value); }
		}
		public DrawingColor BottomColor {
			get { return (DrawingColor)GetValue(BottomColorProperty); }
			set { SetValue(BottomColorProperty, value); }
		}
		public DrawingColor LeftColor {
			get { return (DrawingColor)GetValue(LeftColorProperty); }
			set { SetValue(LeftColorProperty, value); }
		}
		public DrawingColor RightColor {
			get { return (DrawingColor)GetValue(RightColorProperty); }
			set { SetValue(RightColorProperty, value); }
		}
		public DrawingColor DiagonalColor {
			get { return (DrawingColor)GetValue(DiagonalColorProperty); }
			set { SetValue(DiagonalColorProperty, value); }
		}
		public DrawingColor VerticalColor {
			get { return (DrawingColor)GetValue(VerticalColorProperty); }
			set { SetValue(VerticalColorProperty, value); }
		}
		public DrawingColor HorizontalColor {
			get { return (DrawingColor)GetValue(HorizontalColorProperty); }
			set { SetValue(HorizontalColorProperty, value); }
		}
		public bool ShowTopBorder {
			get { return (bool)GetValue(ShowTopBorderProperty); }
			set { SetValue(ShowTopBorderProperty, value); }
		}
		public bool ShowBottomBorder {
			get { return (bool)GetValue(ShowBottomBorderProperty); }
			set { SetValue(ShowBottomBorderProperty, value); }
		}
		public bool ShowLeftBorder {
			get { return (bool)GetValue(ShowLeftBorderProperty); }
			set { SetValue(ShowLeftBorderProperty, value); }
		}
		public bool ShowRightBorder {
			get { return (bool)GetValue(ShowRightBorderProperty); }
			set { SetValue(ShowRightBorderProperty, value); }
		}
		public bool ShowDiagonalDownBorder {
			get { return (bool)GetValue(ShowDiagonalDownBorderProperty); }
			set { SetValue(ShowDiagonalDownBorderProperty, value); }
		}
		public bool ShowDiagonalUpBorder {
			get { return (bool)GetValue(ShowDiagonalUpBorderProperty); }
			set { SetValue(ShowDiagonalUpBorderProperty, value); }
		}
		public bool ShowVerticalInsideBorder {
			get { return (bool)GetValue(ShowVerticalInsideBorderProperty); }
			set { SetValue(ShowVerticalInsideBorderProperty, value); }
		}
		public bool ShowHorizontalInsideBorder {
			get { return (bool)GetValue(ShowHorizontalInsideBorderProperty); }
			set { SetValue(ShowHorizontalInsideBorderProperty, value); }
		}
		public XlBorderLineStyle? TopBorderStyle {
			get { return (XlBorderLineStyle)GetValue(TopBorderStyleProperty); }
			set { SetValue(TopBorderStyleProperty, value); }
		}
		public XlBorderLineStyle? BottomBorderStyle {
			get { return (XlBorderLineStyle)GetValue(BottomBorderStyleProperty); }
			set { SetValue(BottomBorderStyleProperty, value); }
		}
		public XlBorderLineStyle? LeftBorderStyle {
			get { return (XlBorderLineStyle)GetValue(LeftBorderStyleProperty); }
			set { SetValue(LeftBorderStyleProperty, value); }
		}
		public XlBorderLineStyle? RightBorderStyle {
			get { return (XlBorderLineStyle)GetValue(RightBorderStyleProperty); }
			set { SetValue(RightBorderStyleProperty, value); }
		}
		public XlBorderLineStyle? DiagonalDownBorderStyle {
			get { return (XlBorderLineStyle)GetValue(DiagonalDownBorderStyleProperty); }
			set { SetValue(DiagonalDownBorderStyleProperty, value); }
		}
		public XlBorderLineStyle? DiagonalUpBorderStyle {
			get { return (XlBorderLineStyle)GetValue(DiagonalUpBorderStyleProperty); }
			set { SetValue(DiagonalUpBorderStyleProperty, value); }
		}
		public XlBorderLineStyle? VerticalBorderStyle {
			get { return (XlBorderLineStyle)GetValue(VerticalBorderStyleProperty); }
			set { SetValue(VerticalBorderStyleProperty, value); }
		}
		public XlBorderLineStyle? HorizontalBorderStyle {
			get { return (XlBorderLineStyle)GetValue(HorizontalBorderStyleProperty); }
			set { SetValue(HorizontalBorderStyleProperty, value); }
		}
		public SelectedRangeTypeForBorderPreview RangeType {
			get { return (SelectedRangeTypeForBorderPreview)GetValue(RangeTypeProperty); }
			set { SetValue(RangeTypeProperty, value); }
		}
		public bool EnableVerticalBorderCheckState {
			get { return (bool)GetValue(EnableVerticalBorderCheckStateProperty); }
			set { SetValue(EnableVerticalBorderCheckStateProperty, value); }
		}
		public bool EnableHorizontalBorderCheckState {
			get { return (bool)GetValue(EnableHorizontalBorderCheckStateProperty); }
			set { SetValue(EnableHorizontalBorderCheckStateProperty, value); }
		}
		FormatBorderPreviewControl previewControl;
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			FormatBorderPreviewControlContainer container = LayoutHelper.FindElementByType(this, typeof(FormatBorderPreviewControlContainer)) as FormatBorderPreviewControlContainer;
			previewControl = container.Content as FormatBorderPreviewControl;
			InvalidateArrange();
		}
		protected override Size ArrangeOverride(Size arrangeBounds) {
			if (previewControl != null) {
				UpdateBorderInfo();
				previewControl.Invalidate();
			}
			return base.ArrangeOverride(arrangeBounds);
		}
		private void UpdateBorderInfo() {
			UpdateDiagonalCheckButtonState();
			previewControl.TopBorderInfo = CreateBorderPen(TopColor, TopBorderStyle.Value);
			previewControl.BottomBorderInfo = CreateBorderPen(BottomColor, BottomBorderStyle.Value);
			previewControl.LeftBorderInfo = CreateBorderPen(LeftColor, LeftBorderStyle.Value);
			previewControl.RightBorderInfo = CreateBorderPen(RightColor, RightBorderStyle.Value);
			previewControl.DiagonalDownBorderInfo = CreateBorderPen(DiagonalColor, DiagonalDownBorderStyle.Value);
			previewControl.DiagonalUpBorderInfo = CreateBorderPen(DiagonalColor, DiagonalUpBorderStyle.Value);
			previewControl.VerticalInsideBorderInfo = CreateBorderPen(VerticalColor, VerticalBorderStyle.Value);
			previewControl.HorizontalInsideBorderInfo = CreateBorderPen(HorizontalColor, HorizontalBorderStyle.Value);
			previewControl.RangeType = RangeType;
		}
		private void UpdateDiagonalCheckButtonState() {
			if (DiagonalDownBorderStyle.HasValue && DiagonalDownBorderStyle.Value != XlBorderLineStyle.None && !HasSpecialCase(DiagonalDownBorderStyle.Value))
				ShowDiagonalDownBorder = true;
			if (DiagonalDownBorderStyle.HasValue && DiagonalUpBorderStyle.Value != XlBorderLineStyle.None && !HasSpecialCase(DiagonalUpBorderStyle.Value))
				ShowDiagonalUpBorder = true;
		}
		private bool HasSpecialCase(XlBorderLineStyle style) {
			return style == SpecialBorderLineStyle.OutsideComplexBorder || style == SpecialBorderLineStyle.InsideComplexBorder;
		}
		private FormatBorderInfo CreateBorderPen(DrawingColor color, XlBorderLineStyle borderStyle) {
			double[] dashes = ToDoubleArray(LinePatternProvider.GetPattern(borderStyle));
			Pen pen;
			if (borderStyle != SpecialBorderLineStyle.InsideComplexBorder && borderStyle != SpecialBorderLineStyle.OutsideComplexBorder)
				pen = new Pen(new SolidColorBrush(color.ToWpfColor()), GetThickness(borderStyle));
			else
				pen = new Pen(new SolidColorBrush(Color.FromArgb(255, 211, 211, 211)), GetThickness(borderStyle));
			pen.DashStyle = new DashStyle(dashes, 0);
			return new FormatBorderInfo(pen, XlBorderLineStyle.Double == borderStyle);
		}
		private double GetThickness(XlBorderLineStyle borderStyle) {
			return borderStyle == XlBorderLineStyle.Double ? 1 : BorderInfo.LinePixelThicknessTable[borderStyle];
		}
		private double[] ToDoubleArray(float[] p) {
			double[] result = new double[p.Length];
			p.CopyTo(result, 0);
			return result;
		}
	}
	public class FormatBorderInfo {
		public FormatBorderInfo(Pen pen, bool isDouble) {
			BorderPen = pen;
			IsDouble = isDouble;
		}
		public bool IsDouble { get; private set; }
		public Pen BorderPen { get; private set; }
	}
	public class FormatBorderPreviewControlContainer : BackgroundPanel { }
	public class FormatBorderPreviewControl : Control {
		public FormatBorderInfo LeftBorderInfo { get; set; }
		public FormatBorderInfo RightBorderInfo { get; set; }
		public FormatBorderInfo TopBorderInfo { get; set; }
		public FormatBorderInfo BottomBorderInfo { get; set; }
		public FormatBorderInfo DiagonalDownBorderInfo { get; set; }
		public FormatBorderInfo DiagonalUpBorderInfo { get; set; }
		public FormatBorderInfo HorizontalInsideBorderInfo { get; set; }
		public FormatBorderInfo VerticalInsideBorderInfo { get; set; }
		public SelectedRangeTypeForBorderPreview RangeType { get; set; }
		int baseOffset = 4;
		int angleMarkerSize = 5;
		int BorderOffset { get { return baseOffset + angleMarkerSize; } }
		internal void Invalidate() {
			InvalidateVisual();
		}
		protected override void OnRender(DrawingContext dc) {
			base.OnRender(dc);
			DrawAngleMarkers(dc);
			DrawBorders(dc);
		}
		private void DrawBorders(DrawingContext dc) {
			DrawTopBorder(dc);
			DrawBottomBorder(dc);
			DrawLeftBorder(dc);
			DrawRightBorder(dc);
			DrawDiagonalDownBorder(dc);
			DrawDiagonalUpBorder(dc);
			if (RangeType == SelectedRangeTypeForBorderPreview.Row || RangeType == SelectedRangeTypeForBorderPreview.Table)
				DrawVerticalInsideBorder(dc);
			if (RangeType == SelectedRangeTypeForBorderPreview.Column || RangeType == SelectedRangeTypeForBorderPreview.Table)
				DrawHorizontalInsideBorder(dc);
		}
		private void DrawLeftBorder(DrawingContext dc) {
			if (LeftBorderInfo != null) {
				Point from = new Point(BorderOffset, BorderOffset);
				Point to = new Point(BorderOffset, ActualHeight - BorderOffset);
				if (LeftBorderInfo.IsDouble) {
					DrawDoubleLine(dc, LeftBorderInfo.BorderPen, from, to, 2);
				}
				else
					DrawLine(dc, LeftBorderInfo.BorderPen, from, to);
			}
		}
		private void DrawRightBorder(DrawingContext dc) {
			if (RightBorderInfo != null) {
				Point from = new Point(ActualWidth - BorderOffset, BorderOffset);
				Point to = new Point(ActualWidth - BorderOffset, ActualHeight - BorderOffset);
				if (RightBorderInfo.IsDouble) {
					DrawDoubleLine(dc, RightBorderInfo.BorderPen, from, to, -2);
				}
				else
					DrawLine(dc, RightBorderInfo.BorderPen, from, to);
			}
		}
		private void DrawTopBorder(DrawingContext dc) {
			if (TopBorderInfo != null) {
				Point from = new Point(BorderOffset, BorderOffset);
				Point to = new Point(ActualWidth - BorderOffset, BorderOffset);
				if (TopBorderInfo.IsDouble) {
					DrawDoubleLine(dc, TopBorderInfo.BorderPen, from, to, 2);
				}
				else
					DrawLine(dc, TopBorderInfo.BorderPen, from, to);
			}
		}
		private void DrawBottomBorder(DrawingContext dc) {
			if (BottomBorderInfo != null) {
				Point from = new Point(BorderOffset, ActualHeight - BorderOffset);
				Point to = new Point(ActualWidth - BorderOffset, ActualHeight - BorderOffset);
				if (BottomBorderInfo.IsDouble) {
					DrawDoubleLine(dc, BottomBorderInfo.BorderPen, from, to, -2);
				}
				else
					DrawLine(dc, BottomBorderInfo.BorderPen, from, to);
			}
		}
		private void DrawDiagonalDownBorder(DrawingContext dc) {
			if (RangeType == SelectedRangeTypeForBorderPreview.Cell) {
				int offset = 5;
				if (!DiagonalDownBorderInfo.IsDouble)
					DrawDiagonalBorderCore(dc, new Point(BorderOffset, BorderOffset), new Point(ActualWidth - BorderOffset, ActualHeight - BorderOffset), DiagonalDownBorderInfo, offset);
				else
					DrawDiagonalBorderCore(dc, new Point(BorderOffset - 2, BorderOffset), new Point(ActualWidth - BorderOffset - 2, ActualHeight - BorderOffset), DiagonalDownBorderInfo, offset);
			}
			if (RangeType == SelectedRangeTypeForBorderPreview.Row) {
				int offset = 3;
				DrawDiagonalBorderCore(dc, new Point(BorderOffset, BorderOffset), new Point(ActualWidth / 2, ActualHeight - BorderOffset), DiagonalDownBorderInfo, offset);
				DrawDiagonalBorderCore(dc, new Point(ActualWidth / 2, BorderOffset), new Point(ActualWidth - BorderOffset, ActualHeight - BorderOffset), DiagonalDownBorderInfo, offset);
			}
			if (RangeType == SelectedRangeTypeForBorderPreview.Column) {
				int offset = 8;
				if (!DiagonalDownBorderInfo.IsDouble) {
					DrawDiagonalBorderCore(dc, new Point(BorderOffset, BorderOffset), new Point(ActualWidth - BorderOffset, ActualHeight / 2), DiagonalDownBorderInfo, offset);
					DrawDiagonalBorderCore(dc, new Point(BorderOffset, ActualHeight / 2), new Point(ActualWidth - BorderOffset, ActualHeight - BorderOffset), DiagonalDownBorderInfo, offset);
				}
				else {
					DrawDiagonalBorderCore(dc, new Point(BorderOffset - 4, BorderOffset), new Point(ActualWidth - BorderOffset - 4, ActualHeight / 2), DiagonalDownBorderInfo, offset);
					DrawDiagonalBorderCore(dc, new Point(BorderOffset - 4, ActualHeight / 2), new Point(ActualWidth - BorderOffset - 4, ActualHeight - BorderOffset), DiagonalDownBorderInfo, offset);
				}
			}
			if (RangeType == SelectedRangeTypeForBorderPreview.Table) {
				int offset = 5;
				if (!DiagonalDownBorderInfo.IsDouble) {
					DrawDiagonalBorderCore(dc, new Point(BorderOffset, ActualHeight / 2), new Point(ActualWidth / 2, ActualHeight - BorderOffset), DiagonalDownBorderInfo, offset);
					DrawDiagonalBorderCore(dc, new Point(BorderOffset, BorderOffset), new Point(ActualWidth - BorderOffset, ActualHeight - BorderOffset), DiagonalDownBorderInfo, offset);
					DrawDiagonalBorderCore(dc, new Point(ActualWidth / 2, BorderOffset), new Point(ActualWidth - BorderOffset, ActualHeight / 2), DiagonalDownBorderInfo, offset);
				}
				else {
					DrawDiagonalBorderCore(dc, new Point(BorderOffset - 2, ActualHeight / 2), new Point(ActualWidth / 2 - 2, ActualHeight - BorderOffset), DiagonalDownBorderInfo, offset);
					DrawDiagonalBorderCore(dc, new Point(BorderOffset - 2, BorderOffset), new Point(ActualWidth - BorderOffset - 2, ActualHeight - BorderOffset), DiagonalDownBorderInfo, offset);
					DrawDiagonalBorderCore(dc, new Point(ActualWidth / 2 - 2, BorderOffset), new Point(ActualWidth - BorderOffset - 2, ActualHeight / 2), DiagonalDownBorderInfo, offset);
				}
			}
		}
		private void DrawDiagonalUpBorder(DrawingContext dc) {
			if (RangeType == SelectedRangeTypeForBorderPreview.Cell) {
				int offset = -5;
				if (!DiagonalUpBorderInfo.IsDouble)
					DrawDiagonalBorderCore(dc, new Point(BorderOffset, ActualHeight - BorderOffset), new Point(ActualWidth - BorderOffset, BorderOffset), DiagonalUpBorderInfo, offset);
				else
					DrawDiagonalBorderCore(dc, new Point(BorderOffset + 2, ActualHeight - BorderOffset), new Point(ActualWidth - BorderOffset + 2, BorderOffset), DiagonalUpBorderInfo, offset);
			}
			if (RangeType == SelectedRangeTypeForBorderPreview.Row) {
				int offset = -3;
				DrawDiagonalBorderCore(dc, new Point(BorderOffset, ActualHeight - BorderOffset), new Point(ActualWidth / 2, BorderOffset), DiagonalUpBorderInfo, offset);
				DrawDiagonalBorderCore(dc, new Point(ActualWidth / 2, ActualHeight - BorderOffset), new Point(ActualWidth - BorderOffset, BorderOffset), DiagonalUpBorderInfo, offset);
			}
			if (RangeType == SelectedRangeTypeForBorderPreview.Column) {
				int offset = -8;
				if (!DiagonalUpBorderInfo.IsDouble) {
					DrawDiagonalBorderCore(dc, new Point(BorderOffset, ActualHeight / 2), new Point(ActualWidth - BorderOffset, BorderOffset), DiagonalUpBorderInfo, offset);
					DrawDiagonalBorderCore(dc, new Point(BorderOffset, ActualHeight - BorderOffset), new Point(ActualWidth - BorderOffset, ActualHeight / 2), DiagonalUpBorderInfo, offset);
				}
				else {
					DrawDiagonalBorderCore(dc, new Point(BorderOffset + 4, ActualHeight / 2), new Point(ActualWidth - BorderOffset + 4, BorderOffset), DiagonalUpBorderInfo, offset);
					DrawDiagonalBorderCore(dc, new Point(BorderOffset + 4, ActualHeight - BorderOffset), new Point(ActualWidth - BorderOffset + 4, ActualHeight / 2), DiagonalUpBorderInfo, offset);
				}
			}
			if (RangeType == SelectedRangeTypeForBorderPreview.Table) {
				int offset = -5;
				if (!DiagonalUpBorderInfo.IsDouble) {
					DrawDiagonalBorderCore(dc, new Point(BorderOffset, ActualHeight / 2), new Point(ActualWidth / 2, BorderOffset), DiagonalUpBorderInfo, offset);
					DrawDiagonalBorderCore(dc, new Point(BorderOffset, ActualHeight - BorderOffset), new Point(ActualWidth - BorderOffset, BorderOffset), DiagonalUpBorderInfo, offset);
					DrawDiagonalBorderCore(dc, new Point(ActualWidth / 2, ActualHeight - BorderOffset), new Point(ActualWidth - BorderOffset, ActualHeight / 2), DiagonalUpBorderInfo, offset);
				}
				else {
					DrawDiagonalBorderCore(dc, new Point(BorderOffset + 2, ActualHeight / 2), new Point(ActualWidth / 2 + 2, BorderOffset), DiagonalUpBorderInfo, offset);
					DrawDiagonalBorderCore(dc, new Point(BorderOffset + 2, ActualHeight - BorderOffset), new Point(ActualWidth - BorderOffset + 2, BorderOffset), DiagonalUpBorderInfo, offset);
					DrawDiagonalBorderCore(dc, new Point(ActualWidth / 2 + 2, ActualHeight - BorderOffset), new Point(ActualWidth - BorderOffset + 2, ActualHeight / 2), DiagonalUpBorderInfo, offset);
				}
			}
		}
		private void DrawVerticalInsideBorder(DrawingContext dc) {
			if (VerticalInsideBorderInfo != null) {
				Point from = new Point(ActualWidth / 2, BorderOffset);
				Point to = new Point(ActualWidth / 2, ActualHeight - BorderOffset);
				if (VerticalInsideBorderInfo.IsDouble) {
					DrawDoubleLine(dc, VerticalInsideBorderInfo.BorderPen, from, to, 2);
				}
				else
					DrawLine(dc, VerticalInsideBorderInfo.BorderPen, from, to);
			}
		}
		private void DrawHorizontalInsideBorder(DrawingContext dc) {
			if (HorizontalInsideBorderInfo != null) {
				Point from = new Point(BorderOffset, ActualHeight / 2);
				Point to = new Point(ActualWidth - BorderOffset, ActualHeight / 2);
				if (HorizontalInsideBorderInfo.IsDouble) {
					DrawDoubleLine(dc, HorizontalInsideBorderInfo.BorderPen, from, to, -2);
				}
				else
					DrawLine(dc, HorizontalInsideBorderInfo.BorderPen, from, to);
			}
		}
		private void DrawDiagonalBorderCore(DrawingContext dc, Point firstPoint, Point secondPoint, FormatBorderInfo DiagonalBorderInfo, int offset) {
			if (DiagonalBorderInfo != null) {
				if (DiagonalBorderInfo.IsDouble) {
					DrawDoubleLine(dc, DiagonalBorderInfo.BorderPen, firstPoint, secondPoint, offset);
				}
				else
					DrawLine(dc, DiagonalBorderInfo.BorderPen, firstPoint, secondPoint);
			}
		}
		private void DrawAngleMarkers(DrawingContext dc) {
			Pen pen = new Pen(new SolidColorBrush(Colors.Gray), 1);
			DrawTopLeftMarker(dc, pen);
			DrawTopRightMarker(dc, pen);
			DrawBottomLeftMarker(dc, pen);
			DrawBottomRightMarker(dc, pen);
			if (RangeType == SelectedRangeTypeForBorderPreview.Column || RangeType == SelectedRangeTypeForBorderPreview.Table) {
				DrawLeftCenterMarker(dc, pen);
				DrawRightCenterMarker(dc, pen);
			}
			if (RangeType == SelectedRangeTypeForBorderPreview.Row || RangeType == SelectedRangeTypeForBorderPreview.Table) {
				DrawTopCenterMarker(dc, pen);
				DrawBottomCenterMarker(dc, pen);
			}
		}
		private void DrawTopLeftMarker(DrawingContext dc, Pen pen) {
			Point from = new Point(baseOffset, BorderOffset);
			Point to = new Point(BorderOffset, BorderOffset);
			DrawLine(dc, pen, from, to);
			from = new Point(BorderOffset, baseOffset);
			to = new Point(BorderOffset, BorderOffset);
			DrawLine(dc, pen, from, to);
		}
		private void DrawTopRightMarker(DrawingContext dc, Pen pen) {
			Point from = new Point(ActualWidth - baseOffset, BorderOffset);
			Point to = new Point(from.X - angleMarkerSize, BorderOffset);
			DrawLine(dc, pen, from, to);
			from = new Point(ActualWidth - BorderOffset, baseOffset);
			to = new Point(ActualWidth - BorderOffset, from.Y + angleMarkerSize);
			DrawLine(dc, pen, from, to);
		}
		private void DrawBottomRightMarker(DrawingContext dc, Pen pen) {
			Point from = new Point(ActualWidth - baseOffset, ActualHeight - BorderOffset);
			Point to = new Point(from.X - angleMarkerSize, ActualHeight - BorderOffset);
			DrawLine(dc, pen, from, to);
			from = new Point(ActualWidth - BorderOffset, ActualHeight - baseOffset);
			to = new Point(ActualWidth - BorderOffset, from.Y - angleMarkerSize);
			DrawLine(dc, pen, from, to);
		}
		private void DrawBottomLeftMarker(DrawingContext dc, Pen pen) {
			Point from = new Point(baseOffset, ActualHeight - BorderOffset);
			Point to = new Point(BorderOffset, ActualHeight - BorderOffset);
			DrawLine(dc, pen, from, to);
			from = new Point(BorderOffset, ActualHeight - baseOffset);
			to = new Point(BorderOffset, from.Y - angleMarkerSize);
			DrawLine(dc, pen, from, to);
		}
		private void DrawLeftCenterMarker(DrawingContext dc, Pen pen) {
			Point from = new Point(BorderOffset, ActualHeight / 2 - baseOffset);
			Point to = new Point(BorderOffset, ActualHeight / 2);
			DrawLine(dc, pen, from, to);
			from = new Point(baseOffset, ActualHeight / 2);
			to = new Point(BorderOffset, ActualHeight / 2);
			DrawLine(dc, pen, from, to);
			from = new Point(BorderOffset, ActualHeight / 2 + baseOffset);
			to = new Point(BorderOffset, ActualHeight / 2);
			DrawLine(dc, pen, from, to);
		}
		private void DrawRightCenterMarker(DrawingContext dc, Pen pen) {
			Point from = new Point(ActualWidth - BorderOffset, ActualHeight / 2 - baseOffset);
			Point to = new Point(ActualWidth - BorderOffset, ActualHeight / 2);
			DrawLine(dc, pen, from, to);
			from = new Point(ActualWidth - baseOffset, ActualHeight / 2);
			to = new Point(ActualWidth - BorderOffset, ActualHeight / 2);
			DrawLine(dc, pen, from, to);
			from = new Point(ActualWidth - BorderOffset, ActualHeight / 2 + baseOffset);
			to = new Point(ActualWidth - BorderOffset, ActualHeight / 2);
			DrawLine(dc, pen, from, to);
		}
		private void DrawTopCenterMarker(DrawingContext dc, Pen pen) {
			Point from = new Point(ActualWidth / 2 - baseOffset, BorderOffset);
			Point to = new Point(ActualWidth / 2, BorderOffset);
			DrawLine(dc, pen, from, to);
			from = new Point(ActualWidth / 2, baseOffset);
			to = new Point(ActualWidth / 2, BorderOffset);
			DrawLine(dc, pen, from, to);
			from = new Point(ActualWidth / 2 + baseOffset, BorderOffset);
			to = new Point(ActualWidth / 2, BorderOffset);
			DrawLine(dc, pen, from, to);
		}
		private void DrawBottomCenterMarker(DrawingContext dc, Pen pen) {
			Point from = new Point(ActualWidth / 2 - baseOffset, ActualHeight - BorderOffset);
			Point to = new Point(ActualWidth / 2, ActualHeight - BorderOffset);
			DrawLine(dc, pen, from, to);
			from = new Point(ActualWidth / 2, ActualHeight - baseOffset);
			to = new Point(ActualWidth / 2, ActualHeight - BorderOffset);
			DrawLine(dc, pen, from, to);
			from = new Point(ActualWidth / 2 + baseOffset, ActualHeight - BorderOffset);
			to = new Point(ActualWidth / 2, ActualHeight - BorderOffset);
			DrawLine(dc, pen, from, to);
		}
		private void PatchPoints(ref Point from, ref Point to) {
			double patch = 0.5;
			bool isVertical = IsLineVertical(from, to);
			to = new Point(to.X - patch, to.Y - patch);
			from = new Point(from.X - patch, from.Y - patch);
		}
		private bool IsLineVertical(Point from, Point to) {
			return from.Y != to.Y;
		}
		private void DrawDoubleLine(DrawingContext dc, Pen pen, Point from, Point to, int offset) {
			PatchPoints(ref from, ref to);
			DrawLineCore(dc, pen, from, to, true);
			if (IsLineVertical(from, to)) {
				from = new Point(from.X + offset, from.Y);
				to = new Point(to.X + offset, to.Y);
			}
			else {
				from = new Point(from.X, from.Y + offset);
				to = new Point(to.X, to.Y + offset);
			}
			DrawLineCore(dc, pen, from, to, true);
		}
		private void DrawLine(DrawingContext dc, Pen pen, Point from, Point to) {
			DrawLineCore(dc, pen, from, to, false);
		}
		private void DrawLineCore(DrawingContext dc, Pen pen, Point from, Point to, bool isDouble) {
			if (!isDouble && (pen.Thickness == 1 || pen.Thickness == 3))
				PatchPoints(ref from, ref to);
			dc.DrawLine(pen, from, to);
		}
	}
}
