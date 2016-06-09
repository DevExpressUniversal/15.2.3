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
using System.Windows.Shapes;
namespace DevExpress.NoteHint
{
  public enum NoteHintPosition
  {
	None,
	Left,
	Right,
	Up,
	Down,
	Centered
  }
  public enum NoteHintStyle
  { 
	WithArrow,
	Simple
  }
  public delegate void ContentChangedHandler(object oldContent, object newContent);
  public class NoteHintContainer : ContentControl
  {
	public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
	  "CornerRadius", typeof(double), typeof(NoteHintContainer), new PropertyMetadata(7.0));
	public static readonly DependencyProperty HintPositionProperty = DependencyProperty.Register(
	  "HintPosition", typeof(NoteHintPosition), typeof(NoteHintContainer), new PropertyMetadata(NoteHintPosition.Up));
	public static readonly DependencyProperty ArrowScaleProperty = DependencyProperty.Register(
	  "ArrowScale", typeof(double), typeof(NoteHintContainer), new PropertyMetadata(0.5));
	public static readonly DependencyProperty ArrowOffsetRatioProperty = DependencyProperty.Register(
	  "ArrowOffsetRatio", typeof(double), typeof(NoteHintContainer), new PropertyMetadata(0.2));
	public static readonly DependencyProperty HintOffsetProperty = DependencyProperty.Register(
	  "HintOffset", typeof(Point), typeof(NoteHintContainer), new PropertyMetadata(new Point(5.0, 5.0)));
	public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
	  "Stroke", typeof(Brush), typeof(NoteHintContainer), null);
	public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
	  "StrokeThickness", typeof(double), typeof(NoteHintContainer), new PropertyMetadata(1.0));
	public static readonly DependencyProperty NoteHintStyleProperty = DependencyProperty.Register(
	  "NoteHintStyle", typeof(NoteHintStyle), typeof(NoteHintContainer), new PropertyMetadata(NoteHintStyle.WithArrow));
	FrameworkElement rootElement;
	Path hintPath;
	ContentPresenter hintPresenter;
	Geometry hintShapeGeometry;
	Point hintContentLocation;
	Point hintArrowTargetLocation;
	public NoteHintContainer()
	{
	  DefaultStyleKey = typeof(NoteHintContainer);
	  AssignColors();
	}
	static LinearGradientBrush GetFillBrush()
	{
	  GradientStopCollection fillStops = new GradientStopCollection();
	  fillStops.Add(new GradientStop(Color.FromRgb(0xFF, 0xFE, 0xF7), 0.0));
	  fillStops.Add(new GradientStop(Color.FromRgb(0xFF, 0xFD, 0xF2), 0.05));
	  fillStops.Add(new GradientStop(Color.FromRgb(0xFA, 0xFB, 0xF8), 0.5));
	  fillStops.Add(new GradientStop(Color.FromRgb(0xF7, 0xF6, 0xEB), 0.95));
	  fillStops.Add(new GradientStop(Color.FromRgb(0xE5, 0xE3, 0xD7), 1.0));
	  return new LinearGradientBrush(fillStops, new Point(0.5, 0), new Point(0.5, 1.0));
	}
	static LinearGradientBrush GetOutlineBrush()
	{
	  GradientStopCollection strokeStops = new GradientStopCollection();
	  strokeStops.Add(new GradientStop(Color.FromRgb(0xC2, 0xC1, 0xB3), 0.0));
	  strokeStops.Add(new GradientStop(Color.FromRgb(0x66, 0x65, 0x58), 1.0));
	  return new LinearGradientBrush(strokeStops, new Point(0.5, 0), new Point(0.5, 1.0));
	}
	void AssignColors()
	{			
			Background = GetFillBrush();
			Stroke = GetOutlineBrush();
	}
	bool DoubleIsValid(double value)
	{
	  return !double.IsNaN(value) && !double.IsInfinity(value) && value != 0.0;
	}
	bool SizeIsValid(Size size)
	{
	  return DoubleIsValid(size.Width) && DoubleIsValid(size.Height);
	}
	Size GetRectangleSize(Size contentSize, double cornerRadius)
	{
	  return new Size(contentSize.Width + 2 * cornerRadius, contentSize.Height + 2 * cornerRadius);
	}
	Size GetContentSize()
	{
	  if (hintPresenter == null)
		return new Size(100, 100);
	  Size size = hintPresenter.DesiredSize;
	  if (!SizeIsValid(size))
		size = new Size(hintPresenter.ActualWidth, hintPresenter.ActualHeight);
	  if (!SizeIsValid(size))
		size = new Size(100, 100);
	  return size;
	}
	Geometry GetBaseArrowGeomentry()
	{
	  string geometryString = "M 40.001144 0.01965503 19.97359 39.994846 0.00113897 -5.784127e-4 z";
	  PathFigureCollectionConverter figuresConverter = new PathFigureCollectionConverter();
	  PathFigureCollection figures = figuresConverter.ConvertFromString(geometryString) as PathFigureCollection;
	  PathGeometry pathGeomentry = new PathGeometry();
	  pathGeomentry.Figures = figures;
	  pathGeomentry.FillRule = FillRule.Nonzero;
	  return pathGeomentry;
	}
	Geometry GetArrowGeometry(double horzOffset, double vertOffset, double scale, NoteHintPosition hintPosition,
							  double lineThickness, out Size bounds)
	{
	  Geometry arrowGeomentry = GetBaseArrowGeomentry();
	  bounds = arrowGeomentry.Bounds.Size;
	  TransformGroup transformGroup = new TransformGroup();
	  switch (hintPosition)
	  {
		case NoteHintPosition.Up:
		  transformGroup.Children.Add(new TranslateTransform(0, -2 * lineThickness));
		  break;
		case NoteHintPosition.Down:
		  transformGroup.Children.Add(new RotateTransform(180, bounds.Width / 2, bounds.Height));
		  transformGroup.Children.Add(new TranslateTransform(0, -bounds.Height));
		  break;
		case NoteHintPosition.Right:
		  transformGroup.Children.Add(new RotateTransform(90, bounds.Width / 2, bounds.Height));
		  transformGroup.Children.Add(new TranslateTransform(-bounds.Width / 2.0, -bounds.Height / 2.0));
		  break;
		case NoteHintPosition.Left:
		  transformGroup.Children.Add(new RotateTransform(-90, bounds.Width / 2, bounds.Height));
		  transformGroup.Children.Add(new TranslateTransform(bounds.Width / 2.0, -bounds.Height / 2.0));
		  transformGroup.Children.Add(new TranslateTransform(-2 * lineThickness, 0));
		  break;
	  }
	  transformGroup.Children.Add(new ScaleTransform(scale, scale));
	  transformGroup.Children.Add(new TranslateTransform(horzOffset, vertOffset));
	  arrowGeomentry.Transform = transformGroup;
	  bounds = arrowGeomentry.Bounds.Size;
	  return arrowGeomentry;
	}
	Geometry GetArrowGeometry(Size rectangleSize, out Size arrowSize)
	{
	  double arrowHorzOffset = 0;
	  double arrowVertOffset = 0;
	  if (HintStyle == NoteHintStyle.Simple || HintPosition == NoteHintPosition.None || HintPosition == NoteHintPosition.Centered)
	  {
		arrowSize = new Size(0, 0);
		return null;
	  }
	  switch (HintPosition)
	  {
		case NoteHintPosition.Up:
		  arrowHorzOffset = rectangleSize.Width * ArrowOffsetRatio;
		  arrowVertOffset = rectangleSize.Height;
		  break;
		case NoteHintPosition.Down:
		  arrowHorzOffset = rectangleSize.Width * ArrowOffsetRatio;
		  arrowVertOffset = 0;
		  break;
		case NoteHintPosition.Left:
		  arrowHorzOffset = rectangleSize.Width;
		  arrowVertOffset = rectangleSize.Height * ArrowOffsetRatio;
		  break;
		case NoteHintPosition.Right:
		  arrowHorzOffset = 0;
		  arrowVertOffset = rectangleSize.Height * ArrowOffsetRatio;
		  break;
	  }
	  double lineThickness = hintPath != null ? hintPath.StrokeThickness : 0;
	  return GetArrowGeometry(arrowHorzOffset, arrowVertOffset, ArrowScale, HintPosition, lineThickness, out arrowSize);
	}
	RectangleGeometry GetRectangleGeometry(double horzOffset, double vertOffset, Size size, double cornerRadius)
	{
	  RectangleGeometry rectangleGeometry = new RectangleGeometry(new Rect(0, 0, size.Width, size.Height),
																  cornerRadius, cornerRadius);
	  TranslateTransform translateTransform = new TranslateTransform(horzOffset, vertOffset);
	  rectangleGeometry.Transform = translateTransform;
	  return rectangleGeometry;
	}
	RectangleGeometry GetRectangleGeometry(Size arrowSize, Size contentSize)
	{
	  double rectHorzOffset = 0;
	  double rectVertOffset = 0;
	  double lineThickness = hintPath != null ? hintPath.StrokeThickness : 0;
	  if (this.HintStyle == NoteHintStyle.WithArrow)
		switch (HintPosition)
		{
		  case NoteHintPosition.Down:
			rectVertOffset += arrowSize.Height - lineThickness;
			break;
		  case NoteHintPosition.Right:
			rectHorzOffset += arrowSize.Width - lineThickness;
			break;
		}
	  return GetRectangleGeometry(rectHorzOffset, rectVertOffset, contentSize, CornerRadius);
	}
	Point GetContentLocation(Size arrowSize)
	{
	  double thicknessOffset = hintPath != null ? hintPath.StrokeThickness / 2 : 0;
	  switch (HintPosition)
	  {
		case NoteHintPosition.Up:
		case NoteHintPosition.Left:
		  return new Point(CornerRadius + thicknessOffset, CornerRadius + thicknessOffset);
		case NoteHintPosition.Down:
		  return new Point(CornerRadius + thicknessOffset, CornerRadius + arrowSize.Height - thicknessOffset);
		case NoteHintPosition.Right:
		  return new Point(CornerRadius + arrowSize.Width - thicknessOffset, CornerRadius + thicknessOffset);
	  }
	  return new Point(CornerRadius + thicknessOffset, CornerRadius + thicknessOffset);
	}
	Size GetHintSize(NoteHintPosition hintPosition, Size rectangleSize, Size arrowSize)
	{
	  Size hintSize = rectangleSize;
	  switch (hintPosition)
	  {
		case NoteHintPosition.Up:
		case NoteHintPosition.Down:
		  hintSize = new Size(rectangleSize.Width, rectangleSize.Height + arrowSize.Height);
		  break;
		case NoteHintPosition.Left:
		case NoteHintPosition.Right:
		  hintSize = new Size(rectangleSize.Width + arrowSize.Width, rectangleSize.Height);
		  break;
	  }
	  return hintSize;
	}
	Point GetTargetLocation(Rect hintBounds, Size arrowSize)
	{
	  return GetTargetLocation(hintBounds.Size, HintOffset, arrowSize, ArrowOffsetRatio, HintPosition);
	}
	CombinedGeometry GetShapeGeometry(out Point contentLocation, out Point targetLocation)
	{
	  Size contentSize = GetContentSize();
	  Size rectangleSize = GetRectangleSize(contentSize, CornerRadius);
	  Size arrowSize;
	  Geometry arrowGeometry = GetArrowGeometry(rectangleSize, out arrowSize);	  
	  Geometry rectangleGeometry = GetRectangleGeometry(arrowSize, rectangleSize);
	  contentLocation = GetContentLocation(arrowSize);
	  CombinedGeometry geom = new CombinedGeometry();
	  geom.GeometryCombineMode = GeometryCombineMode.Union;
	  geom.Geometry1 = rectangleGeometry;
	  geom.Geometry2 = arrowGeometry;
	  targetLocation = GetTargetLocation(geom.Bounds, arrowSize);
	  return geom;
	}
	void ValidateShapeGeomentry()
	{
	  if (hintShapeGeometry == null)
	  {
		hintShapeGeometry = GetShapeGeometry(out hintContentLocation, out hintArrowTargetLocation);
		if (hintPath != null)
		  hintPath.Data = hintShapeGeometry;
		OnShapeChanged();
	  }
	}
	protected override Size ArrangeOverride(Size arrangeBounds)
	{
	  Size size = base.ArrangeOverride(arrangeBounds);
	  double lineThickness = hintPath != null ? hintPath.StrokeThickness : 0;
	  if (hintPresenter != null)
	  {
		Rect contentLocation = new Rect(hintContentLocation.X, hintContentLocation.Y,
										hintPresenter.DesiredSize.Width, hintPresenter.DesiredSize.Height);
		hintPresenter.Arrange(contentLocation);
	  }
	  if (hintPath != null)
	  {
		Rect contentLocation = new Rect(lineThickness / 2.0, lineThickness / 2.0,
										hintPath.DesiredSize.Width, hintPath.DesiredSize.Height);
		hintPath.Arrange(contentLocation);
	  }
	  return size;
	}
	protected override Size MeasureOverride(Size constraint)
	{
	  Size size = base.MeasureOverride(constraint);
	  if (hintPresenter == null)
		return size;
	  hintPresenter.Measure(constraint);
	  ValidateShapeGeomentry();
	  if (hintPath != null)
		hintPath.Measure(constraint);
	  Rect bounds = hintShapeGeometry.Bounds;
	  double lineThickness = hintPath != null ? hintPath.StrokeThickness : 0;
	  return new Size(bounds.Width + lineThickness, bounds.Height + lineThickness);
	}
	protected override void OnContentChanged(object oldContent, object newContent)
	{
	  base.OnContentChanged(oldContent, newContent);
	  if (ContentChanged != null)
		ContentChanged(oldContent, newContent);
	}
	protected virtual void OnShapeChanged()
	{
	  if (ShapeChanged != null)
		ShapeChanged(this, EventArgs.Empty);
	}   
	public override void OnApplyTemplate()
	{
	  base.OnApplyTemplate();
	  hintPath = GetTemplateChild("path") as Path;
	  rootElement = GetTemplateChild("root") as FrameworkElement;
	  hintPresenter = GetTemplateChild("presenter") as ContentPresenter;
	}
	public void Refresh()
	{
	  hintShapeGeometry = null;
	  InvalidateMeasure();
	  InvalidateArrange();
	  InvalidateVisual();
	}
	public static Point GetTargetLocation(Size hintSize, Point hintOffset, Size arrowSize, double arrowOffsetRatio, NoteHintPosition hintPosition)
	{
	  switch (hintPosition)
	  {
		case NoteHintPosition.Up:
		  return new Point(hintSize.Width * arrowOffsetRatio + arrowSize.Width / 2.0, hintSize.Height + hintOffset.Y);
		case NoteHintPosition.Left:
		  return new Point(hintSize.Width + hintOffset.X, hintSize.Height * arrowOffsetRatio + arrowSize.Height / 2.0);
		case NoteHintPosition.Down:
		  return new Point(hintSize.Width * arrowOffsetRatio + arrowSize.Width / 2.0, -hintOffset.Y);
		case NoteHintPosition.Right:
		  return new Point(-hintOffset.X, hintSize.Height * arrowOffsetRatio + arrowSize.Height / 2.0);
	  }
	  return new Point(hintSize.Width * arrowOffsetRatio, hintSize.Height);
	}
	public Point ArrowTargetLocation
	{
	  get 
	  {
		return hintArrowTargetLocation; 
	  }
	}
	public Brush Stroke
	{
	  get { return (Brush)this.GetValue(StrokeProperty); }
	  set { this.SetValue(StrokeProperty, value); }
	}
	public double StrokeThickness
	{
	  get { return (double)this.GetValue(StrokeThicknessProperty); }
	  set { this.SetValue(StrokeThicknessProperty, value); }
	}
	public double CornerRadius
	{
	  get { return (double)this.GetValue(CornerRadiusProperty); }
	  set { this.SetValue(CornerRadiusProperty, value); }
	}
	public NoteHintPosition HintPosition
	{
	  get { return (NoteHintPosition)this.GetValue(HintPositionProperty); }
	  set { this.SetValue(HintPositionProperty, value); }
	}
	public double ArrowScale
	{
	  get { return (double)this.GetValue(ArrowScaleProperty); }
	  set { this.SetValue(ArrowScaleProperty, value); }
	}
	public double ArrowOffsetRatio
	{
	  get { return (double)this.GetValue(ArrowOffsetRatioProperty); }
	  set { this.SetValue(ArrowOffsetRatioProperty, value); }
	}
	public Point HintOffset
	{
	  get { return (Point)this.GetValue(HintOffsetProperty); }
	  set { this.SetValue(HintOffsetProperty, value); }
	}
	public NoteHintStyle HintStyle
	{
	  get { return (NoteHintStyle)this.GetValue(NoteHintStyleProperty); }
	  set { this.SetValue(NoteHintStyleProperty, value); }
	}
	public event ContentChangedHandler ContentChanged;
	public event EventHandler ShapeChanged;	
  }
}
