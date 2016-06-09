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

using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace DevExpress.Xpf.Spreadsheet.Internal {
	public class PageMarginsPreviewControl : Control {
		public static readonly DependencyProperty TopMarginProperty;
		public static readonly DependencyProperty BottomMarginProperty;
		public static readonly DependencyProperty LeftMarginProperty;
		public static readonly DependencyProperty RightMarginProperty;
		public static readonly DependencyProperty HeaderMarginProperty;
		public static readonly DependencyProperty FooterMarginProperty;
		public static readonly DependencyProperty CenterHorizontallyProperty;
		public static readonly DependencyProperty CenterVerticallyProperty;
		public static readonly DependencyProperty OrientationPortraitProperty;
		public static readonly DependencyProperty OrientationLandscapeProperty;
		static PageMarginsPreviewControl() {
			Type ownerType = typeof(PageMarginsPreviewControl);
			TopMarginProperty = DependencyProperty.Register("TopMargin", typeof(float), ownerType,
				new FrameworkPropertyMetadata(default(float), FrameworkPropertyMetadataOptions.AffectsArrange));
			BottomMarginProperty = DependencyProperty.Register("BottomMargin", typeof(float), ownerType,
				new FrameworkPropertyMetadata(default(float), FrameworkPropertyMetadataOptions.AffectsArrange));
			LeftMarginProperty = DependencyProperty.Register("LeftMargin", typeof(float), ownerType,
				new FrameworkPropertyMetadata(default(float), FrameworkPropertyMetadataOptions.AffectsArrange));
			RightMarginProperty = DependencyProperty.Register("RightMargin", typeof(float), ownerType,
				new FrameworkPropertyMetadata(default(float), FrameworkPropertyMetadataOptions.AffectsArrange));
			HeaderMarginProperty = DependencyProperty.Register("HeaderMargin", typeof(float), ownerType,
				new FrameworkPropertyMetadata(default(float), FrameworkPropertyMetadataOptions.AffectsArrange));
			FooterMarginProperty = DependencyProperty.Register("FooterMargin", typeof(float), ownerType,
				new FrameworkPropertyMetadata(default(float), FrameworkPropertyMetadataOptions.AffectsArrange));
			CenterHorizontallyProperty = DependencyProperty.Register("CenterHorizontally", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange));
			CenterVerticallyProperty = DependencyProperty.Register("CenterVertically", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange));
			OrientationPortraitProperty = DependencyProperty.Register("OrientationPortrait", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange));
			OrientationLandscapeProperty = DependencyProperty.Register("OrientationLandscape", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange));
		}
		#region Fields
		SpinEdit edtTopMargin;
		SpinEdit edtBottomMargin;
		SpinEdit edtLeftMargin;
		SpinEdit edtRightMargin;
		SpinEdit edtHeaderMargin;
		SpinEdit edtFooterMargin;
		#endregion
		public PageMarginsPreviewControl() {
			DefaultStyleKey = typeof(PageMarginsPreviewControl);
		}
		public float TopMargin {
			get { return (float)GetValue(TopMarginProperty); }
			set { SetValue(TopMarginProperty, value); }
		}
		public float BottomMargin {
			get { return (float)GetValue(BottomMarginProperty); }
			set { SetValue(BottomMarginProperty, value); }
		}
		public float LeftMargin {
			get { return (float)GetValue(LeftMarginProperty); }
			set { SetValue(LeftMarginProperty, value); }
		}
		public float RightMargin {
			get { return (float)GetValue(RightMarginProperty); }
			set { SetValue(RightMarginProperty, value); }
		}
		public float HeaderMargin {
			get { return (float)GetValue(HeaderMarginProperty); }
			set { SetValue(HeaderMarginProperty, value); }
		}
		public float FooterMargin {
			get { return (float)GetValue(FooterMarginProperty); }
			set { SetValue(FooterMarginProperty, value); }
		}
		public bool CenterHorizontally {
			get { return (bool)GetValue(CenterHorizontallyProperty); }
			set { SetValue(CenterHorizontallyProperty, value); }
		}
		public bool CenterVertically {
			get { return (bool)GetValue(CenterVerticallyProperty); }
			set { SetValue(CenterVerticallyProperty, value); }
		}
		public bool OrientationPortrait {
			get { return (bool)GetValue(OrientationPortraitProperty); }
			set { SetValue(OrientationPortraitProperty, value); }
		}
		PageMarginsDrawingControl previewControlPortrait;
		PageMarginsDrawingControl previewControlLandscape;
		public override void OnApplyTemplate() {
			UnsubscribeEvents();
			base.OnApplyTemplate();
			edtTopMargin = LayoutHelper.FindElementByName(this, "edtTopMargin") as SpinEdit;
			edtBottomMargin = LayoutHelper.FindElementByName(this, "edtBottomMargin") as SpinEdit;
			edtLeftMargin = LayoutHelper.FindElementByName(this, "edtLeftMargin") as SpinEdit;
			edtRightMargin = LayoutHelper.FindElementByName(this, "edtRightMargin") as SpinEdit;
			edtHeaderMargin = LayoutHelper.FindElementByName(this, "edtHeaderMargin") as SpinEdit;
			edtFooterMargin = LayoutHelper.FindElementByName(this, "edtFooterMargin") as SpinEdit;
			PageMarginsDrawingControlContainer portraitContainer = LayoutHelper.FindElementByName(this, "portraitOrientationContainer") as PageMarginsDrawingControlContainer;
			if (portraitContainer != null) {
				previewControlPortrait = portraitContainer.Content as PageMarginsDrawingControl;
				InvalidateArrange();
			}
			PageMarginsDrawingControlContainer landscapeContainer = LayoutHelper.FindElementByName(this, "landscapeOrientationContainer") as PageMarginsDrawingControlContainer;
			if (landscapeContainer != null) {
				previewControlLandscape = landscapeContainer.Content as PageMarginsDrawingControl;
				InvalidateArrange();
			}
			SubscribeEvents();
		}
		protected override Size ArrangeOverride(Size arrangeBounds) {
			if (previewControlPortrait != null) {
				FlagsInfo();
				previewControlPortrait.Invalidate();
			}
			if (previewControlLandscape != null) {
				FlagsInfo();
				previewControlLandscape.Invalidate();
			}
			return base.ArrangeOverride(arrangeBounds);
		}
		void UnsubscribeEvents() {
			if (edtTopMargin != null)
				edtTopMargin.GotFocus -= edtTopMarginGotFocus;
			if (edtBottomMargin != null)
				edtBottomMargin.GotFocus -= edtBottomMarginGotFocus;
			if (edtLeftMargin != null)
				edtLeftMargin.GotFocus -= edtLeftMarginGotFocus;
			if (edtRightMargin != null)
				edtRightMargin.GotFocus -= edtRightMarginGotFocus;
			if (edtHeaderMargin != null)
				edtHeaderMargin.GotFocus -= edtHeaderMarginGotFocus;
			if (edtFooterMargin != null)
				edtFooterMargin.GotFocus -= edtFooterMarginGotFocus;
			if (edtTopMargin != null)
				edtTopMargin.LostFocus -= edtTopMarginLostFocus;
			if (edtBottomMargin != null)
				edtBottomMargin.LostFocus -= edtBottomMarginLostFocus;
			if (edtLeftMargin != null)
				edtLeftMargin.LostFocus -= edtLeftMarginLostFocus;
			if (edtRightMargin != null)
				edtRightMargin.LostFocus -= edtRightMarginLostFocus;
			if (edtHeaderMargin != null)
				edtHeaderMargin.LostFocus -= edtHeaderMarginLostFocus;
			if (edtFooterMargin != null)
				edtFooterMargin.LostFocus -= edtFooterMarginLostFocus;
		}
		void SubscribeEvents() {
			if (edtTopMargin != null)
				edtTopMargin.GotFocus += edtTopMarginGotFocus;
			if (edtBottomMargin != null)
				edtBottomMargin.GotFocus += edtBottomMarginGotFocus;
			if (edtLeftMargin != null)
				edtLeftMargin.GotFocus += edtLeftMarginGotFocus;
			if (edtRightMargin != null)
				edtRightMargin.GotFocus += edtRightMarginGotFocus;
			if (edtHeaderMargin != null)
				edtHeaderMargin.GotFocus += edtHeaderMarginGotFocus;
			if (edtFooterMargin != null)
				edtFooterMargin.GotFocus += edtFooterMarginGotFocus;
			if (edtTopMargin != null)
				edtTopMargin.LostFocus += edtTopMarginLostFocus;
			if (edtBottomMargin != null)
				edtBottomMargin.LostFocus += edtBottomMarginLostFocus;
			if (edtLeftMargin != null)
				edtLeftMargin.LostFocus += edtLeftMarginLostFocus;
			if (edtRightMargin != null)
				edtRightMargin.LostFocus += edtRightMarginLostFocus;
			if (edtHeaderMargin != null)
				edtHeaderMargin.LostFocus += edtHeaderMarginLostFocus;
			if (edtFooterMargin != null)
				edtFooterMargin.LostFocus += edtFooterMarginLostFocus;
		}
		void edtTopMarginGotFocus(object sender, RoutedEventArgs e) {
			previewControlPortrait.TopMarginColor = Colors.Red;
			previewControlPortrait.Invalidate();
			previewControlLandscape.TopMarginColor = Colors.Red;
			previewControlLandscape.Invalidate();
		}
		void edtBottomMarginGotFocus(object sender, RoutedEventArgs e) {
			previewControlPortrait.BottomMarginColor = Colors.Red;
			previewControlPortrait.Invalidate();
			previewControlLandscape.BottomMarginColor = Colors.Red;
			previewControlLandscape.Invalidate();
		}
		void edtLeftMarginGotFocus(object sender, RoutedEventArgs e) {
			previewControlPortrait.LeftMarginColor = Colors.Red;
			previewControlPortrait.Invalidate();
			previewControlLandscape.LeftMarginColor = Colors.Red;
			previewControlLandscape.Invalidate();
		}
		void edtRightMarginGotFocus(object sender, RoutedEventArgs e) {
			previewControlPortrait.RightMarginColor = Colors.Red;
			previewControlPortrait.Invalidate();
			previewControlLandscape.RightMarginColor = Colors.Red;
			previewControlLandscape.Invalidate();
		}
		void edtHeaderMarginGotFocus(object sender, RoutedEventArgs e) {
			previewControlPortrait.HeaderMarginColor = Colors.Red;
			previewControlPortrait.Invalidate();
			previewControlLandscape.HeaderMarginColor = Colors.Red;
			previewControlLandscape.Invalidate();
		}
		void edtFooterMarginGotFocus(object sender, RoutedEventArgs e) {
			previewControlPortrait.FooterMarginColor = Colors.Red;
			previewControlPortrait.Invalidate();
			previewControlLandscape.FooterMarginColor = Colors.Red;
			previewControlLandscape.Invalidate();
		}
		void edtTopMarginLostFocus(object sender, RoutedEventArgs e) {
			previewControlPortrait.TopMarginColor = Colors.Black;
			previewControlPortrait.Invalidate();
			previewControlLandscape.TopMarginColor = Colors.Black;
			previewControlLandscape.Invalidate();
		}
		void edtBottomMarginLostFocus(object sender, RoutedEventArgs e) {
			previewControlPortrait.BottomMarginColor = Colors.Black;
			previewControlPortrait.Invalidate();
			previewControlLandscape.BottomMarginColor = Colors.Black;
			previewControlLandscape.Invalidate();
		}
		void edtLeftMarginLostFocus(object sender, RoutedEventArgs e) {
			previewControlPortrait.LeftMarginColor = Colors.Black;
			previewControlPortrait.Invalidate();
			previewControlLandscape.LeftMarginColor = Colors.Black;
			previewControlLandscape.Invalidate();
		}
		void edtRightMarginLostFocus(object sender, RoutedEventArgs e) {
			previewControlPortrait.RightMarginColor = Colors.Black;
			previewControlPortrait.Invalidate();
			previewControlLandscape.RightMarginColor = Colors.Black;
			previewControlLandscape.Invalidate();
		}
		void edtHeaderMarginLostFocus(object sender, RoutedEventArgs e) {
			previewControlPortrait.HeaderMarginColor = Colors.Black;
			previewControlPortrait.Invalidate();
			previewControlLandscape.HeaderMarginColor = Colors.Black;
			previewControlLandscape.Invalidate();
		}
		void edtFooterMarginLostFocus(object sender, RoutedEventArgs e) {
			previewControlPortrait.FooterMarginColor = Colors.Black;
			previewControlPortrait.Invalidate();
			previewControlLandscape.FooterMarginColor = Colors.Black;
			previewControlLandscape.Invalidate();
		}
		void FlagsInfo() {
			previewControlPortrait.IsCenterHorizontally = CenterHorizontally;
			previewControlPortrait.IsCenterVertically = CenterVertically;
			previewControlPortrait.Portrait = OrientationPortrait;
			previewControlLandscape.IsCenterHorizontally = CenterHorizontally;
			previewControlLandscape.IsCenterVertically = CenterVertically;
			previewControlLandscape.Portrait = OrientationPortrait;
		}
	}
	public class PageMarginsDrawingControlContainer : BackgroundPanel { }
	public class PageMarginsDrawingControl : Control {
		public Color TopMarginColor { get; set; }
		public Color BottomMarginColor { get; set; }
		public Color LeftMarginColor { get; set; }
		public Color RightMarginColor { get; set; }
		public Color HeaderMarginColor { get; set; }
		public Color FooterMarginColor { get; set; }
		public bool IsCenterHorizontally { get; set; }
		public bool IsCenterVertically { get; set; }
		public bool Portrait { get; set; }
		public bool Landscape { get; set; }
		internal void Invalidate() {
			InvalidateVisual();
		}
		protected override void OnRender(DrawingContext dc) {
			base.OnRender(dc);
			DrawMargins(dc);
		}
		void DrawMargins(DrawingContext dc) {
			Pen penCells = new Pen(new SolidColorBrush(Colors.Black), 1);
			Pen penTopMargin = new Pen(new SolidColorBrush(GetNotEmptyColor(TopMarginColor)), 1);
			Pen penBottomMargin = new Pen(new SolidColorBrush(GetNotEmptyColor(BottomMarginColor)), 1);
			Pen penLeftMargin = new Pen(new SolidColorBrush(GetNotEmptyColor(LeftMarginColor)), 1);
			Pen penRightMargin = new Pen(new SolidColorBrush(GetNotEmptyColor(RightMarginColor)), 1);
			Pen penHeaderMargin = new Pen(new SolidColorBrush(GetNotEmptyColor(HeaderMarginColor)), 1);
			Pen penFooterMargin = new Pen(new SolidColorBrush(GetNotEmptyColor(FooterMarginColor)), 1);
			if (Portrait) {
				DrawCellsPortraitOrientation(dc, penCells);
				DrawTopMarginPortraitOrientation(dc, penTopMargin);
				DrawBottomMarginPortraitOrientation(dc, penBottomMargin);
				DrawLeftMarginPortraitOrientation(dc, penLeftMargin);
				DrawRightMarginPortraitOrientation(dc, penRightMargin);
				DrawHeaderMarginPortraitOrientation(dc, penHeaderMargin);
				DrawFooterMarginPortraitOrientation(dc, penFooterMargin);
			}
			else {
				DrawCellsLandscapeOrientation(dc, penCells);
				DrawTopMarginLandscapeOrientation(dc, penTopMargin);
				DrawBottomMarginLandscapeOrientation(dc, penBottomMargin);
				DrawLeftMarginLandscapeOrientation(dc, penLeftMargin);
				DrawRightMarginLandscapeOrientation(dc, penRightMargin);
				DrawHeaderMarginLandscapeOrientation(dc, penHeaderMargin);
				DrawFooterMarginLandscapeOrientation(dc, penFooterMargin);
			}
		}
		Color GetNotEmptyColor(Color color) {
			if (color.A == 0 && color.R == 0 && color.G == 0 && color.B == 0) {
				return Colors.Black;
			}
			return color;
		}
		void DrawCellsPortraitOrientation(DrawingContext dc, Pen pen) {
			int coordStartX = (int)MarginsLayoutManagerPortraitOrientation.TopPointLeftMargins.X;
			int coordStartY = (int)MarginsLayoutManagerPortraitOrientation.LeftPointTopMargins.Y;
			if (IsCenterHorizontally && !IsCenterVertically)
				coordStartX = 30;
			else if (IsCenterVertically && !IsCenterHorizontally)
				coordStartY = 30;
			else if (IsCenterHorizontally && IsCenterVertically) {
				coordStartX = 30;
				coordStartY = 30;
			}
			for (int i = 0; i < 15; i++) {
				for (int j = 0; j < 5; j++)
					DrawLine(dc, pen, new Point(coordStartX + j * 10, coordStartY), new Point(coordStartX + j * 10, coordStartY + 70));
				DrawLine(dc, pen, new Point(coordStartX, coordStartY + i * 5), new Point(coordStartX + 40, coordStartY + i * 5));
			}
		}
		void DrawHeaderMarginPortraitOrientation(DrawingContext dc, Pen pen) {
			DrawLine(dc, pen, MarginsLayoutManagerPortraitOrientation.LeftPointHeaderMargins, MarginsLayoutManagerPortraitOrientation.RightPointHeaderMargins);
		}
		void DrawFooterMarginPortraitOrientation(DrawingContext dc, Pen pen) {
			DrawLine(dc, pen, MarginsLayoutManagerPortraitOrientation.LeftPointFooterMargins, MarginsLayoutManagerPortraitOrientation.RightPointFooterMargins);
		}
		void DrawTopMarginPortraitOrientation(DrawingContext dc, Pen pen) {
			DrawLine(dc, pen, MarginsLayoutManagerPortraitOrientation.LeftPointTopMargins, MarginsLayoutManagerPortraitOrientation.RightPointTopMargins);
		}
		void DrawBottomMarginPortraitOrientation(DrawingContext dc, Pen pen) {
			DrawLine(dc, pen, MarginsLayoutManagerPortraitOrientation.LeftPointBottomMargins, MarginsLayoutManagerPortraitOrientation.RightPointBottomMargins);
		}
		void DrawLeftMarginPortraitOrientation(DrawingContext dc, Pen pen) {
			DrawLine(dc, pen, MarginsLayoutManagerPortraitOrientation.TopPointLeftMargins, MarginsLayoutManagerPortraitOrientation.BottomPointLeftMargins);
		}
		void DrawRightMarginPortraitOrientation(DrawingContext dc, Pen pen) {
			DrawLine(dc, pen, MarginsLayoutManagerPortraitOrientation.TopPointRightMargins, MarginsLayoutManagerPortraitOrientation.BottomPointRightMargins);
		}
		void DrawCellsLandscapeOrientation(DrawingContext dc, Pen pen) {
			int coordStartX = (int)MarginsLayoutManagerLandscapeOrientation.TopPointLeftMargins.X;
			int coordStartY = (int)MarginsLayoutManagerLandscapeOrientation.LeftPointTopMargins.Y;
			if (IsCenterHorizontally && !IsCenterVertically)
				coordStartX = 30;
			else if (IsCenterVertically && !IsCenterHorizontally)
				coordStartY = 30;
			else if (IsCenterHorizontally && IsCenterVertically) {
				coordStartX = 30;
				coordStartY = 30;
			}
			for (int i = 0; i < 9; i++) {
				for (int j = 0; j < 8; j++)
					DrawLine(dc, pen, new Point(coordStartX + j * 10, coordStartY), new Point(coordStartX + j * 10, coordStartY + 40));
				DrawLine(dc, pen, new Point(coordStartX, coordStartY + i * 5), new Point(coordStartX + 70, coordStartY + i * 5));
			}
		}
		void DrawHeaderMarginLandscapeOrientation(DrawingContext dc, Pen pen) {
			DrawLine(dc, pen, MarginsLayoutManagerLandscapeOrientation.LeftPointHeaderMargins, MarginsLayoutManagerLandscapeOrientation.RightPointHeaderMargins);
		}
		void DrawFooterMarginLandscapeOrientation(DrawingContext dc, Pen pen) {
			DrawLine(dc, pen, MarginsLayoutManagerLandscapeOrientation.LeftPointFooterMargins, MarginsLayoutManagerLandscapeOrientation.RightPointFooterMargins);
		}
		void DrawTopMarginLandscapeOrientation(DrawingContext dc, Pen pen) {
			DrawLine(dc, pen, MarginsLayoutManagerLandscapeOrientation.LeftPointTopMargins, MarginsLayoutManagerLandscapeOrientation.RightPointTopMargins);
		}
		void DrawBottomMarginLandscapeOrientation(DrawingContext dc, Pen pen) {
			DrawLine(dc, pen, MarginsLayoutManagerLandscapeOrientation.LeftPointBottomMargins, MarginsLayoutManagerLandscapeOrientation.RightPointBottomMargins);
		}
		void DrawLeftMarginLandscapeOrientation(DrawingContext dc, Pen pen) {
			DrawLine(dc, pen, MarginsLayoutManagerLandscapeOrientation.TopPointLeftMargins, MarginsLayoutManagerLandscapeOrientation.BottomPointLeftMargins);
		}
		void DrawRightMarginLandscapeOrientation(DrawingContext dc, Pen pen) {
			DrawLine(dc, pen, MarginsLayoutManagerLandscapeOrientation.TopPointRightMargins, MarginsLayoutManagerLandscapeOrientation.BottomPointRightMargins);
		}
		void DrawLine(DrawingContext dc, Pen pen, Point from, Point to) {
			DrawLineCore(dc, pen, from, to, false);
		}
		void DrawLineCore(DrawingContext dc, Pen pen, Point from, Point to, bool isDouble) {
			if (!isDouble && (pen.Thickness == 1 || pen.Thickness == 3))
				PatchPoints(ref from, ref to);
			dc.DrawLine(pen, from, to);
		}
		void PatchPoints(ref Point from, ref Point to) {
			double patch = 0.5;
			bool isVertical = IsLineVertical(from, to);
			to = new Point(to.X - patch, to.Y - patch);
			from = new Point(from.X - patch, from.Y - patch);
		}
		bool IsLineVertical(Point from, Point to) {
			return from.Y != to.Y;
		}
	}
	#region MarginsLayoutManagerPortraitOrientation
	public static class MarginsLayoutManagerPortraitOrientation {
		#region Fields
		static int heightDrawPanel = 130;
		static int widthDrawPanel = 100;
		static int horizontalLeftRightOffset = 15;
		static int verticalHeaderFooterOffset = 7;
		static int verticalTopBottomOffset = 17;
		static Point leftPointTopMargins = new Point(0, verticalTopBottomOffset);
		static Point rightPointTopMargins = new Point(widthDrawPanel, verticalTopBottomOffset);
		static Point leftPointBottomMargins = new Point(0, heightDrawPanel - verticalTopBottomOffset);
		static Point rightPointBottomMargins = new Point(widthDrawPanel, heightDrawPanel - verticalTopBottomOffset);
		static Point topPointLeftMargins = new Point(horizontalLeftRightOffset, 0);
		static Point bottomPointLeftMargins = new Point(horizontalLeftRightOffset, heightDrawPanel);
		static Point topPointRightMargins = new Point(widthDrawPanel - horizontalLeftRightOffset, 0);
		static Point bottomPointRightMargins = new Point(widthDrawPanel - horizontalLeftRightOffset, heightDrawPanel);
		static Point leftPointHeaderMargins = new Point(0, verticalHeaderFooterOffset);
		static Point rightPointHeaderMargins = new Point(widthDrawPanel, verticalHeaderFooterOffset);
		static Point leftPointFooterMargins = new Point(0, heightDrawPanel - verticalHeaderFooterOffset);
		static Point rightPointFooterMargins = new Point(widthDrawPanel, heightDrawPanel - verticalHeaderFooterOffset);
		#endregion
		#region Properties
		public static Point LeftPointTopMargins { get { return leftPointTopMargins; } }
		public static Point RightPointTopMargins { get { return rightPointTopMargins; } }
		public static Point LeftPointBottomMargins { get { return leftPointBottomMargins; } }
		public static Point RightPointBottomMargins { get { return rightPointBottomMargins; } }
		public static Point TopPointLeftMargins { get { return topPointLeftMargins; } }
		public static Point BottomPointLeftMargins { get { return bottomPointLeftMargins; } }
		public static Point TopPointRightMargins { get { return topPointRightMargins; } }
		public static Point BottomPointRightMargins { get { return bottomPointRightMargins; } }
		public static Point LeftPointHeaderMargins { get { return leftPointHeaderMargins; } }
		public static Point RightPointHeaderMargins { get { return rightPointHeaderMargins; } }
		public static Point LeftPointFooterMargins { get { return leftPointFooterMargins; } }
		public static Point RightPointFooterMargins { get { return rightPointFooterMargins; } }
		#endregion
	}
	#endregion
	#region MarginsLayoutManagerLandscapeOrientation
	static class MarginsLayoutManagerLandscapeOrientation {
		#region Fields
		static int heightDrawPanel = 100;
		static int widthDrawPanel = 130;
		static int horizontalLeftRightOffset = 15;
		static int verticalHeaderFooterOffset = 7;
		static int verticalTopBottomOffset = 17;
		static Point leftPointTopMargins = new Point(0, verticalTopBottomOffset);
		static Point rightPointTopMargins = new Point(widthDrawPanel, verticalTopBottomOffset);
		static Point leftPointBottomMargins = new Point(0, heightDrawPanel - verticalTopBottomOffset);
		static Point rightPointBottomMargins = new Point(widthDrawPanel, heightDrawPanel - verticalTopBottomOffset);
		static Point topPointLeftMargins = new Point(horizontalLeftRightOffset, 0);
		static Point bottomPointLeftMargins = new Point(horizontalLeftRightOffset, heightDrawPanel);
		static Point topPointRightMargins = new Point(widthDrawPanel - horizontalLeftRightOffset, 0);
		static Point bottomPointRightMargins = new Point(widthDrawPanel - horizontalLeftRightOffset, heightDrawPanel);
		static Point leftPointHeaderMargins = new Point(0, verticalHeaderFooterOffset);
		static Point rightPointHeaderMargins = new Point(widthDrawPanel, verticalHeaderFooterOffset);
		static Point leftPointFooterMargins = new Point(0, heightDrawPanel - verticalHeaderFooterOffset);
		static Point rightPointFooterMargins = new Point(widthDrawPanel, heightDrawPanel - verticalHeaderFooterOffset);
		#endregion
		#region Properties
		public static Point LeftPointTopMargins { get { return leftPointTopMargins; } }
		public static Point RightPointTopMargins { get { return rightPointTopMargins; } }
		public static Point LeftPointBottomMargins { get { return leftPointBottomMargins; } }
		public static Point RightPointBottomMargins { get { return rightPointBottomMargins; } }
		public static Point TopPointLeftMargins { get { return topPointLeftMargins; } }
		public static Point BottomPointLeftMargins { get { return bottomPointLeftMargins; } }
		public static Point TopPointRightMargins { get { return topPointRightMargins; } }
		public static Point BottomPointRightMargins { get { return bottomPointRightMargins; } }
		public static Point LeftPointHeaderMargins { get { return leftPointHeaderMargins; } }
		public static Point RightPointHeaderMargins { get { return rightPointHeaderMargins; } }
		public static Point LeftPointFooterMargins { get { return leftPointFooterMargins; } }
		public static Point RightPointFooterMargins { get { return rightPointFooterMargins; } }
		#endregion
	}
	#endregion
}
