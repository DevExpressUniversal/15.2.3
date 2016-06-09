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

#if !WINRT && !WP
using DevExpress.Xpf.WindowsUI.Base;
using System.Windows.Controls;
using System.Windows.Shapes;
#else
using DevExpress.Core.Native;
using DevExpress.UI.Xaml.Controls.Native;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endif
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.BarCode;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Drawing.Drawing2D;
using DevExpress.XtraPrinting.Native;
#if !WINRT && !WP
namespace DevExpress.Xpf.Controls.Internal {
#else
namespace DevExpress.UI.Xaml.Controls.Native {
#endif
	public class BarCodePainter : Control, IGraphicsBase {
#if WINRT || WP
		public static readonly DependencyProperty OwnerFlowDirectionProperty;
#endif
		public static readonly DependencyProperty SymbologyProperty;
		public static readonly DependencyProperty BarCodeDataProperty;
		static BarCodePainter() {
			var dProp = new DependencyPropertyRegistrator<BarCodePainter>();
			dProp.Register<SymbologyBase>("Symbology", ref SymbologyProperty, null, (d,e) => ((BarCodePainter)d).SymbologyChanged(e));
			dProp.Register<IFullBarCodeData>("BarCodeData", ref BarCodeDataProperty, null);
#if WINRT || WP
			dProp.Register("OwnerFlowDirection", ref OwnerFlowDirectionProperty, FlowDirection.LeftToRight);
#endif
		}
		Grid grid;
		Canvas canvas;
		Path path;
		Path backgroundPath;
		public SymbologyBase Symbology {
			get { return (SymbologyBase)GetValue(SymbologyProperty); }
			set { SetValue(SymbologyProperty, value); }
		}
		public IFullBarCodeData BarCodeData {
			get { return (IFullBarCodeData)GetValue(BarCodeDataProperty); }
			set { SetValue(BarCodeDataProperty, value); }
		}
#if WINRT || WP
		public FlowDirection OwnerFlowDirection {
			get { return (FlowDirection)GetValue(OwnerFlowDirectionProperty); }
			set { SetValue(OwnerFlowDirectionProperty, value); }
		}
#endif
		public BarCodePainter() {
			GraphicsBase.PageUnit = System.Drawing.GraphicsUnit.Pixel;
			DefaultStyleKey = typeof(BarCodePainter);
			this.SizeChanged += BarCodePainter_SizeChanged;
#if WINRT || WP
			this.FlowDirection = Windows.UI.Xaml.FlowDirection.LeftToRight;
#endif
		}
		void SymbologyChanged(DependencyPropertyChangedEventArgs e) {
			InvalidateMeasure();
		}
#if !WINRT && !WP
		public 
#else
		protected
#endif
		override void OnApplyTemplate() {
			base.OnApplyTemplate();
			grid = (Grid)GetTemplateChild("Part_Grid");
			canvas = (Canvas)GetTemplateChild("Part_Canvas");
			InvalidateBarCodePainter();
		}
		public virtual void InvalidateBarCodePainter() {
			if(Symbology == null || grid == null)
				return;
			grid.Children.Clear();
			path = new Path() { Data = new GeometryGroup() { FillRule = FillRule.Nonzero}, Fill = WindowsFormsHelper.ConvertColorToBrush(this.BarCodeData.Style.ForeColor), };
			backgroundPath = new Path() { Data = new GeometryGroup(), Fill = WindowsFormsHelper.ConvertColorToBrush(this.BarCodeData.Style.BackColor) };
			grid.Children.Add(backgroundPath);
			grid.Children.Add(path);
			canvas.Children.Clear();
			UpdateSymbology();
			Symbology.GeneratorBase.DrawContent(this, GetPainterRect(), BarCodeData);
		}
		void BarCodePainter_SizeChanged(object sender, SizeChangedEventArgs e) {
			InvalidateBarCodePainter();
		}
		System.Drawing.RectangleF GetPainterRect() {
			return new System.Drawing.RectangleF(0, 0, (float)ActualWidth, (float)ActualHeight);
		}
		public virtual void UpdateSymbology() {
			BarCode2DGenerator generator = Symbology.GeneratorBase as BarCode2DGenerator;
			if(generator != null) {
				try {
				generator.Update(BarCodeData.Text, BarCodeData.BinaryData);
				} catch(Exception) { }
			}
		}
		protected virtual void FillRectangle(System.Drawing.Brush brush, float x, float y, float width, float height) {
			Color color = WindowsFormsHelper.ConvertBrushToColor(brush);
			Rect rect = new Rect(Math.Floor(x), Math.Floor(y), Math.Ceiling(width), Math.Ceiling(height));
			if(color == ((SolidColorBrush)backgroundPath.Fill).Color) {
				backgroundPath.Data = new RectangleGeometry() { Rect = rect };
			} else {
				GeometryGroup.Children.Add(new RectangleGeometry() { Rect = rect });
			}
		}
#if WINRT || WP
		protected virtual System.Drawing.StringAlignment ConvertTextAlignment(System.Drawing.StringAlignment stringAlignment) {
			if(OwnerFlowDirection == Windows.UI.Xaml.FlowDirection.LeftToRight || stringAlignment == System.Drawing.StringAlignment.Center)
				return stringAlignment;
			else if(stringAlignment == System.Drawing.StringAlignment.Far)
				return System.Drawing.StringAlignment.Near;
			else 
				return System.Drawing.StringAlignment.Far;
		}
#endif
		protected virtual void DrawText(string text, System.Drawing.RectangleF bounds, System.Drawing.StringAlignment stringAlignment) {
			FormattedText formattedText = GetFormattedText(text);
			var textBlock = new TextBlock() { Text = formattedText.Text, Foreground = new SolidColorBrush(Colors.Black) };
			Canvas.SetLeft(textBlock, bounds.X);
			Canvas.SetTop(textBlock, bounds.Y);
			textBlock.Width = bounds.Width;
			textBlock.Height = bounds.Height;
#if WINRT || WP
			textBlock.TextAlignment = WindowsFormsHelper.ConvertStringToTextAlignment(ConvertTextAlignment(stringAlignment));
#else
			textBlock.TextAlignment = WindowsFormsHelper.ConvertStringToTextAlignment(stringAlignment);
#endif
			canvas.Children.Add(textBlock);
		}
		FormattedText GetFormattedText(string s) {
			Typeface typeFace = new Typeface(this.FontFamily.Source);
			return new FormattedText(s, System.Globalization.CultureInfo.CurrentUICulture, FlowDirection, typeFace, FontSize, Foreground);
		}
		protected override Size MeasureOverride(Size availableSize) {
			var baseSize = base.MeasureOverride(availableSize);
			if(Symbology == null)
				return baseSize;
			var generator = Symbology.GeneratorBase;
			var barCodeData = new BarCodeData(BarCodeData);
			barCodeData.AutoModule = false;
			var viewIfno = new BarCodeDrawingViewInfo(new System.Drawing.RectangleF(0, 0, float.PositiveInfinity, float.PositiveInfinity), barCodeData);
			generator.CalculateDrawingViewInfo(viewIfno, this);
			double maxWidth = Math.Max(viewIfno.BestSize.Width, viewIfno.BestSize.Height / 3);
			double maxHeight = Math.Max(viewIfno.BestSize.Height, viewIfno.BestSize.Width / 3);
			double width = double.IsInfinity(availableSize.Width) ? maxWidth : Math.Min(availableSize.Width, maxWidth);
			double height = double.IsInfinity(availableSize.Height) ? maxHeight : Math.Min(availableSize.Height, maxHeight);
			return new Size(width, height);
		}		
		#region IGraphicsBase
		System.Drawing.RectangleF IGraphicsBase.ClipBounds { get { return new System.Drawing.RectangleF(0, 0, (float)Width, (float)Height); } set { } }
		IGraphicsBase GraphicsBase { get { return (IGraphicsBase)this; } }
		void IGraphicsBase.DrawString(string text, System.Drawing.Font font, System.Drawing.Brush brush, System.Drawing.RectangleF bounds, System.Drawing.StringFormat format) {
			DrawText(text, bounds, format.Alignment);
		}
		GeometryGroup GeometryGroup { get { return (GeometryGroup)path.Data; } }
		void IGraphicsBase.FillRectangle(System.Drawing.Brush brush, float x, float y, float width, float height) {
			FillRectangle(brush, x, y, width, height);
		}
		System.Drawing.Brush IGraphicsBase.GetBrush(System.Drawing.Color color) {
			return new System.Drawing.SolidBrush(color);
		}
		System.Drawing.SizeF MesureString(string text) {
			var textBlock = new TextBlock() { Text = text };
			textBlock.Measure(new Size(double.MaxValue, double.MaxValue));
#if !WINRT && !WP
			return new System.Drawing.SizeF((float)textBlock.DesiredSize.Width, (float)textBlock.DesiredSize.Height);
#else
			return new System.Drawing.SizeF((float)(textBlock.ActualWidth * 2), (float)(textBlock.ActualHeight * 2));
#endif
		}
		System.Drawing.SizeF IGraphicsBase.MeasureString(string text, System.Drawing.Font font, float width, System.Drawing.StringFormat stringFormat, System.Drawing.GraphicsUnit graphicsUnit) {
			return MesureString(text);
		}
		System.Drawing.GraphicsUnit IGraphicsBase.PageUnit { get; set; }
		void IGraphicsBase.ResetTransform() { }
		System.Drawing.SizeF IGraphicsBase.MeasureString(string text, System.Drawing.Font font, System.Drawing.GraphicsUnit graphicsUnit) {
			return MesureString(text);
		}
		void IGraphicsBase.ApplyTransformState(System.Drawing.Drawing2D.MatrixOrder order, bool removeState) { }
		void IGraphicsBase.SaveTransformState() { }
		void IGraphicsBase.RotateTransform(float angle) { }
		void IGraphicsBase.TranslateTransform(float dx, float dy, MatrixOrder matrixOrder) { }
		System.Drawing.SizeF IGraphicsBase.MeasureString(string text, System.Drawing.Font font, System.Drawing.PointF location, System.Drawing.StringFormat stringFormat, System.Drawing.GraphicsUnit graphicsUnit) {
			return MesureString(text);
		}
#if !WINRT && !WP
		void IGraphicsBase.DrawCheckBox(System.Drawing.RectangleF rect, System.Windows.Forms.CheckState state) {
			throw new NotImplementedException();
		}
		void IGraphicsBase.DrawEllipse(System.Drawing.Pen pen, float x, float y, float width, float height) {
			throw new NotImplementedException();
		}
		void IGraphicsBase.DrawEllipse(System.Drawing.Pen pen, System.Drawing.RectangleF rect) {
			throw new NotImplementedException();
		}
		void IGraphicsBase.DrawImage(System.Drawing.Image image, System.Drawing.Point point) {
			throw new NotImplementedException();
		}
		void IGraphicsBase.DrawImage(System.Drawing.Image image, System.Drawing.RectangleF rect, System.Drawing.Color underlyingColor) {
			throw new NotImplementedException();
		}
		void IGraphicsBase.DrawImage(System.Drawing.Image image, System.Drawing.RectangleF rect) {
			throw new NotImplementedException();
		}
		void IGraphicsBase.DrawLine(System.Drawing.Pen pen, float x1, float y1, float x2, float y2) {
			throw new NotImplementedException();
		}
		void IGraphicsBase.DrawLine(System.Drawing.Pen pen, System.Drawing.PointF pt1, System.Drawing.PointF pt2) {
			throw new NotImplementedException();
		}
		void IGraphicsBase.DrawLines(System.Drawing.Pen pen, System.Drawing.PointF[] points) {
			throw new NotImplementedException();
		}
		void IGraphicsBase.DrawPath(System.Drawing.Pen pen, System.Drawing.Drawing2D.GraphicsPath path) {
			throw new NotImplementedException();
		}
		void IGraphicsBase.DrawRectangle(System.Drawing.Pen pen, System.Drawing.RectangleF bounds) {
			throw new NotImplementedException();
		}
		void IGraphicsBase.DrawString(string s, System.Drawing.Font font, System.Drawing.Brush brush, System.Drawing.PointF point) {
			throw new NotImplementedException();
		}
		void IGraphicsBase.DrawString(string s, System.Drawing.Font font, System.Drawing.Brush brush, System.Drawing.PointF point, System.Drawing.StringFormat format) {
			throw new NotImplementedException();
		}
		void IGraphicsBase.DrawString(string s, System.Drawing.Font font, System.Drawing.Brush brush, System.Drawing.RectangleF bounds) {
			throw new NotImplementedException();
		}
		void IGraphicsBase.FillEllipse(System.Drawing.Brush brush, float x, float y, float width, float height) {
			throw new NotImplementedException();
		}
		void IGraphicsBase.FillEllipse(System.Drawing.Brush brush, System.Drawing.RectangleF rect) {
			throw new NotImplementedException();
		}
		void IGraphicsBase.FillPath(System.Drawing.Brush brush, System.Drawing.Drawing2D.GraphicsPath path) {
			throw new NotImplementedException();
		}
		void IGraphicsBase.FillRectangle(System.Drawing.Brush brush, System.Drawing.RectangleF bounds) {
			throw new NotImplementedException();
		}
		System.Drawing.SizeF IGraphicsBase.MeasureString(string text, System.Drawing.Font font, System.Drawing.SizeF size, System.Drawing.StringFormat stringFormat, System.Drawing.GraphicsUnit graphicsUnit) {
			throw new NotImplementedException();
		}		
		void IGraphicsBase.RotateTransform(float angle, System.Drawing.Drawing2D.MatrixOrder order) {
			throw new NotImplementedException();
		}
		void IGraphicsBase.ScaleTransform(float sx, float sy, System.Drawing.Drawing2D.MatrixOrder order) {
			throw new NotImplementedException();
		}
		void IGraphicsBase.ScaleTransform(float sx, float sy) {
			throw new NotImplementedException();
		}
		System.Drawing.Drawing2D.SmoothingMode IGraphicsBase.SmoothingMode {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		void IGraphicsBase.TranslateTransform(float dx, float dy) {
			throw new NotImplementedException();
		}
#endif
		#endregion
	}
}
