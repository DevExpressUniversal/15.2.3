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
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Interop;
using System.Windows.Documents;
using DevExpress.Utils.Text;
using DevExpress.Utils.Text.Internal;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;
namespace DevExpress.NoteHint
{
  public partial class NoteWindow : Window
  {
	const string StrFadeInAnimationName = "uxFadeInAnimation";
	Storyboard moveAnimation;
	Rect startSnapRect;
	DragHelper dragHelper;
	Point snapPoint;
	Point startScreenPoint;
	bool useAnimationInLocation;
	delegate void RefreshDelegate();
	public NoteWindow()
	{
	  InitializeComponent();
	  Loaded += ehLoaded;
	  DragEnter += ehDragEnter;
	  this.SizeChanged += ehSizeChanged;
	  dragHelper = new DragHelper();
	  dragHelper.Start(this, hintContainer, hintContainer);
	  dragHelper.StopDragging += ehStopDragging;
	  hintContainer.ShapeChanged += ehHintContainerShapeChanged;
	}
	void StartFadeInAnimation()
	{
	  Storyboard uxFadeInAnimation = FindResource(StrFadeInAnimationName) as Storyboard;
	  if (uxFadeInAnimation != null)
	  {
		Storyboard.SetTarget(uxFadeInAnimation, this);
		uxFadeInAnimation.Completed += ehFadeInAnimationCompleted;
		uxFadeInAnimation.Begin();
	  }
	}
	void StopFadeInAnimation()
	{
	  Storyboard uxFadeInAnimation = FindResource(StrFadeInAnimationName) as Storyboard;
	  if (uxFadeInAnimation != null)
	  {
		uxFadeInAnimation.Completed -= ehFadeInAnimationCompleted;
		uxFadeInAnimation.Stop();
		Opacity = 1.0;
	  }
	}
	Rect GetWindowBounds()
	{
	  Point pos = new Point(Left, Top);
	  return new Rect(pos.X, pos.Y, ActualWidth, ActualHeight);
	}
	Rect ScreenBounds()
	{
	  Point screenLocation = PointToScreen(new Point(0, 0));
	  return new Rect(screenLocation, RenderSize);
	}
	Rect TransformRect(Rect rect, Matrix transform)
	{
	  Point topLeft = rect.TopLeft;
	  Point bottomRight = rect.BottomRight;
	  Point topLeftTransformed = transform.Transform(topLeft);
	  Point bottomRightTransformed = transform.Transform(bottomRight);
	  double width = bottomRightTransformed.X - topLeftTransformed.X;
	  double height = bottomRightTransformed.Y - topLeftTransformed.Y;
	  return new Rect(topLeftTransformed, new Size(width, height));
	}
	Point DeviceToWPF(Point point)
	{
	  using (var source = new HwndSource(new HwndSourceParameters()))
	  {
		Matrix matrix = source.CompositionTarget.TransformFromDevice;
		return matrix.Transform(point);
	  }
	}
	Rect DeviceToWPF(Rect rect)
	{
	  using (var source = new HwndSource(new HwndSourceParameters()))
	  {
		return DeviceToWPF(ref rect, source);
	  }
	}
	Rect DeviceToWPF(ref Rect rect, HwndSource source)
	{
	  Matrix matrix = source.CompositionTarget.TransformFromDevice;
	  return TransformRect(rect, matrix);
	}
	Point WPFToDevice(Point point)
	{
	  using (var source = new HwndSource(new HwndSourceParameters()))
	  {
		return WPFToDevice(ref point, source);
	  }
	}
	Point WPFToDevice(ref Point point, HwndSource source)
	{
	  Matrix matrix = source.CompositionTarget.TransformToDevice;
	  return matrix.Transform(point);
	}
	System.Windows.Forms.Screen GetStartScreen()
	{
	  Point pt = WPFToDevice(startScreenPoint);
	  System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.FromPoint(new System.Drawing.Point((int)pt.X, (int)pt.Y));
	  return screen;
	}
	Rect GetStartScreenBounds()
	{
	  System.Windows.Forms.Screen screen = GetStartScreen();
	  if (screen == null)
		return Rect.Empty;
	  Rect screenBounds = DeviceToWPF(new Rect(screen.WorkingArea.X, screen.WorkingArea.Y, screen.WorkingArea.Width, screen.WorkingArea.Height));
	  return screenBounds;
	}
	Point CalcTargetLocation(Point location, bool centerOverTarget)
	{
	  Point targetLocation = location;
	  Rect windowBounds = this.GetWindowBounds();
	  if (centerOverTarget)
		targetLocation = new Point(location.X - windowBounds.Width / 2, location.Y - windowBounds.Height / 2);
	  return targetLocation;
	}
	bool HasScreenIntersection()
	{
	  bool snapPointIsValid = snapPoint.X != 0 || snapPoint.Y != 0;
	  if (!snapPointIsValid)
		return false;
	  Rect bounds = GetHintBounds(snapPoint, HintPosition);
	  System.Windows.Forms.Screen screen = GetStartScreen();
	  if (screen == null)
		return false;
	  Rect screenBounds = DeviceToWPF(new Rect(screen.WorkingArea.X, screen.WorkingArea.Y, screen.WorkingArea.Width, screen.WorkingArea.Height));
	  return !screenBounds.Contains(bounds);
	}
	void StopMoveAnimation()
	{
	  if (moveAnimation == null)
		return;
	  moveAnimation.Completed -= ehStoryboardCompleted;
	  moveAnimation.Stop();
	  moveAnimation = null;
	}
	void AnimatioHintMotion(Point location)
	{
	  if (moveAnimation != null)
		StopMoveAnimation();
	  QuarticEase easefall = new QuarticEase();
	  easefall.EasingMode = EasingMode.EaseOut;
	  moveAnimation = new Storyboard();
	  DoubleAnimation xAn = new DoubleAnimation(this.Left, location.X, new Duration(TimeSpan.FromMilliseconds(500)));
	  xAn.EasingFunction = easefall;
	  Storyboard.SetTarget(xAn, this);
	  Storyboard.SetTargetProperty(xAn, new PropertyPath("Left"));
	  moveAnimation.Children.Add(xAn);
	  DoubleAnimation yAn = new DoubleAnimation(this.Top, location.Y, new Duration(TimeSpan.FromMilliseconds(500)));
	  yAn.EasingFunction = easefall;
	  Storyboard.SetTarget(yAn, this);
	  Storyboard.SetTargetProperty(yAn, new PropertyPath("Top"));
	  moveAnimation.Children.Add(yAn);
	  moveAnimation.Completed += ehStoryboardCompleted;
	  moveAnimation.Begin();
	}
	public void RefreshContainer()
	{
	  hintContainer.Refresh();
	}
	void LocateHint() { LocateHint(false); }
	void LocateHint(bool firstRun)
	{
	  if (!startSnapRect.IsEmpty && HasScreenIntersection())
	  {		
		UpdateSnapPoint(startSnapRect, false, ref snapPoint);
		startSnapRect = Rect.Empty;
		Dispatcher.BeginInvoke(new RefreshDelegate(RefreshContainer), null);					
	  }
	  Point location = snapPoint;
	  Point targetLocation = CalcTargetLocation(location, HintPosition == NoteHintPosition.Centered);
	  SetLocation(targetLocation, true);		
	}
	bool CanMoveHorz(Line line, Point point, double moveDistance)
	{
	  double x = point.X + moveDistance;
	  return line.X1 <= x && line.X2 >= x;
	}
	bool CanMoveVert(Line line, Point point, double moveDistance)
	{
	  double y = point.Y + moveDistance;
	  return line.Y1 <= y && line.Y2 >= y;
	}
	Point GetHintTargetPoint(NoteHintPosition hintPosition)
	{
	  Rect rect = this.ScreenBounds();
	  Point point = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
	  Point targetLocation = NoteHintContainer.GetTargetLocation(rect.Size, HintOffset, new Size(30, 30), ArrowOffsetRatio, hintPosition);
	  if (hintPosition != NoteHintPosition.Centered)
	  {
		point.X = rect.X + targetLocation.X;
		point.Y = rect.Y + targetLocation.Y;
	  }
	  return point;
	}
	Rect GetHintBounds(Point location, NoteHintPosition hintPosition)
	{
	  Rect bounds = GetWindowBounds();
	  Point formLocation = location;
	  if (HintPosition == NoteHintPosition.Centered)
		formLocation = new Point(location.X - bounds.Width / 2.0, location.Y - bounds.Height / 2.0);
	  else
	  {
		Point targetLocation = NoteHintContainer.GetTargetLocation(bounds.Size, HintOffset, new Size(30, 30), ArrowOffsetRatio, hintPosition);
		formLocation = new Point(location.X - targetLocation.X, location.Y - targetLocation.Y);
	  }
	  return new Rect(formLocation.X, formLocation.Y, bounds.Width, bounds.Height);
	}
	bool ValidateLeftIntersection(Rect screenBounds, Rect hintBounds, Point targetPoint, Line line, out double offset)
	{
	  offset = 0;
	  if (hintBounds.X < screenBounds.X)
	  {
		offset = screenBounds.X - hintBounds.X;
		if (!CanMoveHorz(line, targetPoint, offset))
		  return false;		
	  }
	  return true;
	}
	bool ValidateRightIntersection(Rect screenBounds, Rect hintBounds, Point targetPoint, Line line, out double offset)
	{
	  offset = 0;
	  if (hintBounds.Right > screenBounds.Right)
	  {
		offset = screenBounds.Right - hintBounds.Right;
		if (!CanMoveHorz(line, targetPoint, offset))
		  return false;
	  }
	  return true;	  
	}
	bool ValidateUpIntersection(Rect screenBounds, Rect hintBounds, Point targetPoint, Line line, out double offset)
	{
	  offset = 0;
	  if (hintBounds.Y < screenBounds.Y)
	  {
		offset = screenBounds.Y - hintBounds.Y;
		if (!CanMoveVert(line, targetPoint, offset))
		  return false;
	  }
	  return true;	 
	}
	bool ValidateDownIntersection(Rect screenBounds, Rect hintBounds, Point targetPoint, Line line, out double offset)
	{
	  offset = 0;
	  if (hintBounds.Bottom < screenBounds.Bottom)
	  {
		offset = screenBounds.Bottom - hintBounds.Bottom;
		if (!CanMoveVert(line, targetPoint, offset))
		  return false;
	  }
	  return true;  
	}
	Point CorrectScreenIntersectionsAlongLine(Line line, Point location, NoteHintPosition hintPosition, out bool canCorrect)
	{
	  canCorrect = true;
	  System.Windows.Forms.Screen screen = GetStartScreen();
	  if (screen == null)
		return location;
	  Rect formBounds = GetHintBounds(location, hintPosition);
	  Rect screenBounds = GetStartScreenBounds();
	  if (screenBounds.Contains(formBounds))
		return location;
	  canCorrect = false;
	  double moveDistance;
	  if (hintPosition == NoteHintPosition.Up || hintPosition == NoteHintPosition.Down)
	  {
		if (!ValidateLeftIntersection(screenBounds, formBounds, location, line, out moveDistance))
		  return location;
		formBounds.X += moveDistance;
		location.X += moveDistance;
		if (!ValidateRightIntersection(screenBounds, formBounds, location, line, out moveDistance))
		  return location;
		formBounds.X += moveDistance;
		location.X += moveDistance;
	  }
	  else if (hintPosition == NoteHintPosition.Left || hintPosition == NoteHintPosition.Right)
	  {
		if (!ValidateUpIntersection(screenBounds, formBounds, location, line, out moveDistance))
		  return location;
		formBounds.Y += moveDistance;
		location.Y += moveDistance;
		if (!ValidateDownIntersection(screenBounds, formBounds, location, line, out moveDistance))
		  return location;
		formBounds.Y += moveDistance;
		location.Y += moveDistance;	   
	  }
	  canCorrect = screenBounds.Contains(formBounds);
	  return location;
	}
	Point CorrectScreenIntersections(Point location, Rect bounds)
	{
	  System.Windows.Forms.Screen screen = GetStartScreen();
	  if (screen == null)
		return location;
	  Point result = location;
	  Rect screenBounds = GetStartScreenBounds();
	  if (result.X < screenBounds.X)
		result.X = screenBounds.X;
	  if ((result.X + bounds.Width) > screenBounds.Right)
		result.X -= (result.X + bounds.Width) - screenBounds.Right;
	  if (result.Y < screenBounds.Y)
		result.Y = screenBounds.Y;
	  if ((result.Y + bounds.Height) > screenBounds.Bottom)
		result.Y -= (result.Y + bounds.Height) - screenBounds.Bottom;
	  return result;
	}
	Point GetSnapPoint(Rect snapRect, out NoteHintPosition hintPosition)
	{
	  hintPosition = NoteHintPosition.Centered;
	  Dictionary<Line, NoteHintPosition> lines = new Dictionary<Line, NoteHintPosition>();
	  lines.Add(new Line(snapRect.Left, snapRect.Top, snapRect.Right, snapRect.Top), NoteHintPosition.Up);
	  lines.Add(new Line(snapRect.Right, snapRect.Top, snapRect.Right, snapRect.Bottom), NoteHintPosition.Right);
	  lines.Add(new Line(snapRect.Left, snapRect.Bottom, snapRect.Right, snapRect.Bottom), NoteHintPosition.Down);
	  lines.Add(new Line(snapRect.Left, snapRect.Top, snapRect.Left, snapRect.Bottom), NoteHintPosition.Left);
	  double minDistance = -1;
	  Point snapPoint = new Point(0, 0);
	  foreach (Line line in lines.Keys)
	  {
		NoteHintPosition lineHintPosition = lines[line];
		Point point = GetHintTargetPoint(lineHintPosition);
		Point intersectPoint;
		double distance = line.GetDistance(point, out intersectPoint);
		if (!line.ContainsPoint(intersectPoint))
		  distance = line.GetDistanceToCenterPoint(point, out intersectPoint);
		bool canCorrect;
		intersectPoint = CorrectScreenIntersectionsAlongLine(line, intersectPoint, lineHintPosition, out canCorrect);
		if (!canCorrect)
		  continue;
		distance = Line.GetLength(point.X, point.Y, intersectPoint.X, intersectPoint.Y);
		if (minDistance == -1 || distance < minDistance)
		{
		  hintPosition = lines[line];
		  minDistance = distance;
		  snapPoint = intersectPoint;
		}
	  }
	  return snapPoint;
	}	
	void UpdateSnapPoint(Rect snapRect, bool staysAtPos, ref Point snapPoint)
	{
	  if (staysAtPos)
	  {
		snapPoint = snapRect.Location;
		snapPoint.Offset(hintContainer.ArrowTargetLocation.X, hintContainer.ArrowTargetLocation.Y);
	  }
	  else
	  {
		NoteHintPosition hintPosition;
		snapPoint = GetSnapPoint(snapRect, out hintPosition);
		HintPosition = hintPosition;
	  }
	}
	void SetLocation(Point location, bool correctIntersections)
	{
	  Point newLocation = location;
	  if (!(HintPosition == NoteHintPosition.Centered))
		newLocation = new Point(location.X - hintContainer.ArrowTargetLocation.X, location.Y - hintContainer.ArrowTargetLocation.Y);
	  if (correctIntersections)
	  {		
		Rect windowBounds = GetWindowBounds();
		newLocation = CorrectScreenIntersections(newLocation, windowBounds);
	  }
	  if (useAnimationInLocation)
		AnimatioHintMotion(newLocation);
	  else
	  {
		Left = newLocation.X;
		Top = newLocation.Y;
	  }
	}
	void Snap(Rect snapRect, bool staysAtPos)
	{
	  if (HintPosition == NoteHintPosition.Centered || HintPosition == NoteHintPosition.None)
		return;
	  Point snapPoint = snapRect.Location;
	  UpdateSnapPoint(snapRect, staysAtPos, ref snapPoint);
	  useAnimationInLocation = true;
	  this.snapPoint = snapPoint;
	  hintContainer.Refresh();
	}
	Point GetFirstSnapPoint(Rect snapRect)
	{
	  switch (HintPosition)
	  {
		case NoteHintPosition.Left:
		  return new Point(snapRect.Left, snapRect.Top + snapRect.Height / 2);
		case NoteHintPosition.Right:
		  return new Point(snapRect.Right, snapRect.Top + snapRect.Height / 2);
		case NoteHintPosition.Up:
		  return new Point(snapRect.Left + snapRect.Width / 2, snapRect.Top);
		case NoteHintPosition.Down:
		  return new Point(snapRect.Left + snapRect.Width / 2, snapRect.Bottom);
		case NoteHintPosition.Centered:
		  return new Point(snapRect.Left + snapRect.Width / 2, snapRect.Top + snapRect.Height / 2);
		case NoteHintPosition.None:
		default:
		  return snapRect.Location;
	  }
	}
	void ehSizeChanged(object sender, SizeChangedEventArgs e) {
		SizeChanged -= ehSizeChanged;
		LocateHint(true);
	}
	void ehLoaded(object sender, RoutedEventArgs e)
	{
	  Loaded -= ehLoaded;
	  LocateHint();
	  StartFadeInAnimation();
	}
	void ehDragEnter(object sender, DragEventArgs e)
	{
	  StopMoveAnimation();
	}
	void ehStopDragging(object sender, EventArgs e)
	{
	  Snap(startSnapRect, false);
	}
	void ehFadeInAnimationCompleted(object sender, EventArgs e)
	{
	  StopFadeInAnimation();
	}
	void ehHintContainerShapeChanged(object sender, EventArgs e)
	{
	  LocateHint();
	}
	void ehStoryboardCompleted(object sender, EventArgs e)
	{
	  if (moveAnimation == null)
		return;
	  moveAnimation.Completed -= ehStoryboardCompleted;
	  moveAnimation.Stop();
	}
	protected override void OnClosed(EventArgs e)
	{
	  DragEnter -= ehDragEnter;
	  if (dragHelper != null)
		dragHelper.Stop();
	  base.OnClosed(e);
	}
	public void Show(System.Drawing.Rectangle snapRect) {
		Show(new Rect(snapRect.X, snapRect.Y, snapRect.Width, snapRect.Height));
	}
	public void Show(Rect snapRect)
	{
	  startSnapRect = snapRect;
	  snapPoint = GetFirstSnapPoint(snapRect);
	  startScreenPoint = snapPoint;
	  Show();
	}
	public bool ShowHtmlCloseButton { get; set; }
	public void SetHtmlContent(string text, IStringImageProvider imageProvider) {
		StackPanel sp = new StackPanel();
		TextBlock tb = new TextBlock();
		tb.Inlines.AddRange(Parse(text, imageProvider));
		sp.Children.Add(tb);
		if(ShowHtmlCloseButton) {
			var button = new Button() { Content = "Hide", HorizontalAlignment = System.Windows.HorizontalAlignment.Right, Width = 50 };
			button.Click += (s, e) => {
				Hide();
			};
			sp.Children.Add(button);
		}
		HintContent = sp;
	}
	public object HintContent
	{
	  get { return hintContainer.Content; }
	  set { hintContainer.Content = value; }
	}
	public double CornerRadius
	{
	  get { return hintContainer.CornerRadius; }
	  set { hintContainer.CornerRadius = value; }
	}
	public NoteHintPosition HintPosition
	{
	  get { return hintContainer.HintPosition; }
	  set { hintContainer.HintPosition = value; }
	}
	public double ArrowScale
	{
	  get { return hintContainer.ArrowScale; }
	  set { hintContainer.ArrowScale = value; }
	}
	public double ArrowOffsetRatio
	{
	  get { return hintContainer.ArrowOffsetRatio; }
	  set { hintContainer.ArrowOffsetRatio = value; }
	}
	public Point HintOffset
	{
	  get { return hintContainer.HintOffset; }
	  set { hintContainer.HintOffset = value; }
	}
	public NoteHintStyle NoteHintStyle
	{
	  get { return hintContainer.HintStyle; }
	  set { hintContainer.HintStyle = value; }
	}
	public Brush Fill
	{
	  get { return hintContainer.Background; }
	  set { hintContainer.Background = value; }
	}
	public Brush Stroke
	{
	  get { return hintContainer.Stroke; }
	  set { hintContainer.Stroke = value; }
	}
	public double StrokeThickness
	{
	  get { return hintContainer.StrokeThickness; }
	  set { hintContainer.StrokeThickness = value; }
	}
	List<Inline> Parse(string text, IStringImageProvider imageProvider) {
		List<Inline> res = new List<Inline>();
		var list = StringParser.Parse(12, text, true);
		int lastLineNumber = 0;
		foreach (var block in list) {
			if (block.LineNumber != lastLineNumber) res.Add(new LineBreak());
			lastLineNumber = block.LineNumber;
			Inline run = null;
			switch (block.Type) {
				case StringBlockType.Text:
					run = CreateTextBlock(block);
					break;
				case StringBlockType.Link:
					run = CreateLink(block);
					break;
				case StringBlockType.Image:
					run = CreateImageBlock(block, imageProvider);
					break;
			}
			if (run != null) res.Add(run);
		}
		return res;
	}
	Inline CreateImageBlock(StringBlock block, IStringImageProvider imageProvider) {
		if(imageProvider == null) return null;
		var image = (System.Drawing.Bitmap)imageProvider.GetImage(block.ImageName);
		if(image == null) return null;
		IntPtr handle = image.GetHbitmap();
		var source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
		   handle, 
		   IntPtr.Zero, 
		   System.Windows.Int32Rect.Empty, 
		   BitmapSizeOptions.FromEmptyOptions());
		Image i = new Image();
		i.Stretch = Stretch.None;
		i.SnapsToDevicePixels = true;
		i.Source = source;
		i.Width = source.Width + 1;
		i.Height = source.Height + 1;
		RenderOptions.SetBitmapScalingMode(i, BitmapScalingMode.NearestNeighbor);
		InlineUIContainer container = new InlineUIContainer(i);
		DevExpress.Utils.Drawing.Helpers.NativeMethods.DeleteObject(handle);
		return container;
	}
	protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
		e.Cancel = true;
		Hide();
	}
	Inline CreateLink(StringBlock block) {
		var link = new Hyperlink(new Run(block.Text)) { NavigateUri = new System.Uri(block.Link) };
		Inline res = link;
		link.RequestNavigate += (s, e) => {
			OpenLink(block.Link);
		};
		return res;
	}
	void OpenLink(string link) {
		using (Process process = new Process()) {
			process.StartInfo = new ProcessStartInfo(link);
			process.Start();
		}
	}
	private Inline CreateTextBlock(StringBlock block) {
		if(block.Text == "\r") return new Run();
		Inline res = null;
		Run textRun = new Run(block.Text);
		res = textRun;
		res.FontSize = block.FontSettings.Size;
		var c = block.FontSettings.Color;
		if (c != System.Drawing.Color.Empty) {
			res.Foreground = new SolidColorBrush(Color.FromArgb(c.A, c.R, c.G, c.B));
		}
		if ((block.FontSettings.Style & System.Drawing.FontStyle.Bold) == System.Drawing.FontStyle.Bold) {
			res = new Bold(res);
		}
		if ((block.FontSettings.Style & System.Drawing.FontStyle.Italic) == System.Drawing.FontStyle.Italic) {
			res = new Italic(res);
		}
		if ((block.FontSettings.Style & System.Drawing.FontStyle.Underline) == System.Drawing.FontStyle.Underline) {
			res = new Underline(res);
		}
		if ((block.FontSettings.Style & System.Drawing.FontStyle.Underline) == System.Drawing.FontStyle.Strikeout) {
			res = new Underline(res);
		}
		return res;
	}
  }
}
