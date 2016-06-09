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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Text;
using DevExpress.Utils.ToolTip.ViewInfo;
using DevExpress.Utils.ViewInfo;
using DevExpress.Utils.Win;
using DevExpress.Utils.Text.Internal;
using System.Collections.Generic;
using DevExpress.XtraEditors;
namespace DevExpress.Utils.ToolTip.ViewInfo  {
	public abstract class ToolTipViewInfoBase {
		public const bool DefaultApplyCursorOffset = true;
		bool isReady;
		Rectangle bounds;
		Rectangle cursorBounds;
		ToolTipControllerShowEventArgs showArgs;
		bool allowHtmlText = false;
		bool applyCursorOffset = DefaultApplyCursorOffset;
		public ToolTipViewInfoBase() {
			Clear();
		}
		public virtual void Clear() {
			this.allowHtmlText = false;
			this.showArgs = null;
			this.isReady = false;
			this.showArgs = null;
			bounds = cursorBounds = Rectangle.Empty;
		}
		public bool AllowHtmlText {
			get {
				if (ShowArgs.AllowHtmlText != DefaultBoolean.Default)
					return ShowArgs.AllowHtmlText == DefaultBoolean.True;
				return allowHtmlText;
			}
			set { allowHtmlText = value; }
		}
		public void Calculate(ToolTipControllerShowEventArgs showArgs, Point cursorPosition) {
			Calculate(showArgs, Size.Empty, cursorPosition);
		}
		public void Calculate(ToolTipControllerShowEventArgs showArgs, Point cursorPosition, bool allowHtmlText) {
			Calculate(showArgs, Size.Empty, cursorPosition, allowHtmlText, null);
		}
		public void Calculate(ToolTipControllerShowEventArgs showArgs, Size contentSize, Point cursorPosition) {
			Calculate(showArgs, contentSize, cursorPosition, false, null);   
		}
		public void Calculate(ToolTipControllerShowEventArgs showArgs, Size contentSize, Point cursorPosition, bool allowHtmlText, bool? applyCursorOffset) {
			Clear();
			this.allowHtmlText = allowHtmlText;
			this.showArgs = showArgs;
			this.applyCursorOffset = applyCursorOffset.HasValue ? applyCursorOffset.Value : DefaultApplyCursorOffset;
			CalculateInternal(contentSize, cursorPosition, showArgs.ObjectBounds);
			SetReady();
		}
		protected virtual void CalculateInternal(Size contentSize, Point cursorPosition, Rectangle objectBounds) {
			CalcBounds(contentSize, cursorPosition, objectBounds);
		}
		protected internal UserLookAndFeel GetLookAndFeel(ToolTipControllerShowEventArgs showArgs) {
			if(showArgs == null) return UserLookAndFeel.Default;
			ISupportLookAndFeel look = showArgs.SelectedObject as ISupportLookAndFeel;
			if(look == null) look = showArgs.SelectedControl as ISupportLookAndFeel;
			if(look != null) return look.LookAndFeel;
			IToolTipLookAndFeelProvider provider = showArgs.SelectedObject as IToolTipLookAndFeelProvider;
			if(provider != null) return provider.LookAndFeel;
			provider = showArgs.SelectedControl as IToolTipLookAndFeelProvider;
			if(provider != null) return provider.LookAndFeel;
			return UserLookAndFeel.Default;
		}
		protected void SetReady() { this.isReady = true; }
		public bool IsReady { get { return isReady; } }
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		protected internal ToolTipControllerShowEventArgs ShowArgs { get { return showArgs; } }
		protected ToolTipLocation Location { get { return ShowArgs.GetToolTipLocation(); } }
		protected ToolTipStyle ToolTipStyle { get { return ShowArgs.ToolTipStyle; } set { ShowArgs.ToolTipStyle = value; } }
		protected Rectangle CursorBounds {
			get {
				if(cursorBounds.IsEmpty) {
					cursorBounds = CursorInfo.CursorBounds;
				}
				return cursorBounds;
			}
		}
		public Point GetCursorPosition(ToolTipControllerShowEventArgs showArgs, Control control) {
			if(control == null) return Point.Empty;
			this.showArgs = showArgs;
			Point cursorPosition;
			if(!IsCursoPositionSucceedForCurrentLocation(control, out cursorPosition)) {
				showArgs.ToolTipLocation = ReverseToolTipLocation();
				if(!IsCursoPositionSucceedForCurrentLocation(control, out cursorPosition)) {
					showArgs.ToolTipLocation = ReverseToolTipLocation2();
					if(!IsCursoPositionSucceedForCurrentLocation(control, out cursorPosition)) {
						showArgs.ToolTipLocation = ReverseToolTipLocation();
						if(IsCursoPositionSucceedForCurrentLocation(control, out cursorPosition)) {
							cursorPosition = control.PointToScreen(Point.Empty);
							cursorPosition.X += control.Width / 2;
							cursorPosition.X += control.Height / 2;
						}
					}
				}
			}
			return cursorPosition;
		}
		bool IsCursoPositionSucceedForCurrentLocation(Control control, out Point cursorPosition) {
			ToolTipLocation savedLocation = Location;
			cursorPosition = GetCursorPositionByControl(control);
			CalculateInternal(Size.Empty, cursorPosition, Rectangle.Empty);
			return Location == savedLocation;
		}
		Point GetCursorPositionByControl(Control control) {
			Point pt = control.PointToScreen(Point.Empty);
			pt = OffsetPoint(pt, CalcControlLocationOffSet(control.Size));
			Size size = CalcCursorOffset();
			size.Width = -size.Width; size.Height = -size.Height;
			return OffsetPoint(pt, size);
		}
		Rectangle CalcToolTipRectangle(Point cursorLocation) {
			return CalcToolTipRectangle(Location, cursorLocation, Rectangle.Empty);
		}
		Rectangle CalcToolTipRectangle(ToolTipLocation location, Point cursorLocation) {
			return CalcToolTipRectangle(location, cursorLocation, Rectangle.Empty);
		}
		Rectangle CalcToolTipRectangle(Point cursorLocation, Rectangle objectBounds) {
			return CalcToolTipRectangle(Location, cursorLocation, objectBounds);
		}
		protected int ToolTipIndent { get { return 16; } }
		Point CalcLocationByObjectBounds(ToolTipLocation location, Size toolTipSize, Rectangle objectBounds) {
			Point pt = new Point();
			if(location == ToolTipLocation.BottomCenter || location == ToolTipLocation.BottomLeft || location == ToolTipLocation.BottomRight)
				pt.Y = objectBounds.Bottom + ToolTipIndent;
			if(location == ToolTipLocation.LeftBottom || location == ToolTipLocation.RightBottom)
				pt.Y = objectBounds.Bottom - toolTipSize.Height;
			if(location == ToolTipLocation.TopLeft || location == ToolTipLocation.TopCenter || location == ToolTipLocation.TopRight)
				pt.Y = objectBounds.Y - toolTipSize.Height - ToolTipIndent;
			if(location == ToolTipLocation.LeftTop || location == ToolTipLocation.RightTop)
				pt.Y = objectBounds.Y;
			if(location == ToolTipLocation.LeftCenter || location == ToolTipLocation.RightCenter)
				pt.Y = objectBounds.Y + (objectBounds.Height - toolTipSize.Height) / 2;
			if(location == ToolTipLocation.LeftBottom || location == ToolTipLocation.LeftCenter || location == ToolTipLocation.LeftTop)
				pt.X = objectBounds.X - toolTipSize.Width - ToolTipIndent;
			if(location == ToolTipLocation.BottomLeft || location == ToolTipLocation.TopLeft)
				pt.X = objectBounds.X;
			if(location == ToolTipLocation.RightBottom || location == ToolTipLocation.RightCenter || location == ToolTipLocation.RightTop)
				pt.X = objectBounds.Right + ToolTipIndent;
			if(location == ToolTipLocation.BottomRight || location == ToolTipLocation.TopRight) {
				pt.X = objectBounds.Right - toolTipSize.Width;
			}
			if(location == ToolTipLocation.BottomCenter || location == ToolTipLocation.TopCenter)
				pt.X = objectBounds.X + (objectBounds.Width - toolTipSize.Width) / 2;
			return pt;
		}
		Rectangle CalcToolTipRectangle(ToolTipLocation newLocation, Point cursorLocation, Rectangle objectBounds) {
			if(Location != newLocation) {
				ShowArgs.ToolTipLocation = newLocation;
				bounds.Size = ShowArgs.UseCustomSize ? CalcBoundsSize(ShowArgs.CustomSize) : CalcBoundsSize();
			}
			if(objectBounds.IsEmpty) {
				bounds.Location = cursorLocation;
				bounds.Location = OffsetPoint(bounds.Location, CalcLocationOffset());
				bounds.Location = OffsetPoint(bounds.Location, CalcCursorOffset());
			}
			else {
				bounds.Location = CalcLocationByObjectBounds(newLocation, bounds.Size, objectBounds);
			}
			return bounds;
		}
		Point OffsetPoint(Point pt, Size size) {
			pt.X += size.Width; pt.Y += size.Height;
			return pt;
		}
		bool InRange(int near, int far, int value) {
			return value >= near && value < far;
		}
		ToolTipLocation ReverseToolTipLocationHorizontal() {
			ToolTipLocation[] reverseLocation = { 
											ToolTipLocation.BottomRight, ToolTipLocation.BottomCenter, ToolTipLocation.BottomLeft, 
											ToolTipLocation.TopRight, ToolTipLocation.TopCenter, ToolTipLocation.TopLeft,
											ToolTipLocation.RightTop, ToolTipLocation.RightCenter, ToolTipLocation.RightBottom,
											ToolTipLocation.LeftTop, ToolTipLocation.LeftCenter, ToolTipLocation.LeftBottom, 
											ToolTipLocation.Default, ToolTipLocation.Fixed
										 };
			return reverseLocation[(int)Location];
		}
		ToolTipLocation ReverseToolTipLocationVertical() {
			ToolTipLocation[] reverseLocation = { 
											ToolTipLocation.TopLeft, ToolTipLocation.TopCenter, ToolTipLocation.TopRight, 
											ToolTipLocation.BottomLeft, ToolTipLocation.BottomCenter, ToolTipLocation.BottomRight,
											ToolTipLocation.LeftBottom, ToolTipLocation.LeftCenter, ToolTipLocation.LeftTop,
											ToolTipLocation.RightBottom, ToolTipLocation.RightCenter, ToolTipLocation.RightTop, 
											ToolTipLocation.Default, ToolTipLocation.Fixed
										 };
			return reverseLocation[(int)Location];
		}
		ToolTipLocation ReverseToolTipLocation() {
			ToolTipLocation[] ReverseLocation = {	
													ToolTipLocation.TopRight, ToolTipLocation.TopCenter, ToolTipLocation.BottomLeft, 
													ToolTipLocation.BottomRight, ToolTipLocation.BottomCenter, ToolTipLocation.BottomLeft,
													ToolTipLocation.RightBottom, ToolTipLocation.RightCenter, ToolTipLocation.RightTop,
													ToolTipLocation.LeftBottom, ToolTipLocation.LeftCenter, ToolTipLocation.LeftTop, ToolTipLocation.Default, ToolTipLocation.Fixed};
			return ReverseLocation[(int)Location];
		}
		ToolTipLocation ReverseToolTipLocation2() {
			ToolTipLocation[] ReverseLocation = {	
													ToolTipLocation.TopLeft, ToolTipLocation.TopCenter, ToolTipLocation.TopRight, 
													ToolTipLocation.BottomLeft, ToolTipLocation.BottomCenter, ToolTipLocation.BottomRight,
													ToolTipLocation.RightTop, ToolTipLocation.RightCenter, ToolTipLocation.RightBottom,
													ToolTipLocation.LeftTop, ToolTipLocation.LeftCenter, ToolTipLocation.LeftBottom, ToolTipLocation.Default, ToolTipLocation.Fixed};
			return ReverseLocation[(int)Location];
		}
		bool IsBottomLocation {
			get {
				return (int)Location <= (int)ToolTipLocation.BottomRight
					|| Location == ToolTipLocation.LeftBottom || Location == ToolTipLocation.RightBottom;
			}
		}
		bool IsRightLocation {
			get {
				return (int)Location >= (int)ToolTipLocation.RightTop
					|| Location == ToolTipLocation.BottomRight || Location == ToolTipLocation.TopRight;
			}
		}
		bool IsLeftLocation {
			get {
				return Location == ToolTipLocation.LeftBottom || Location == ToolTipLocation.LeftCenter || Location == ToolTipLocation.LeftTop;
			}
		}
		Size CalcCursorOffset() {
			if(Location == ToolTipLocation.Fixed || !applyCursorOffset) return Size.Empty;
			Size size = Size.Empty;
			if((int)Location >= (int)ToolTipLocation.RightTop) {
				size.Width = CursorSize.Width;
			}
			if(IsBottomLocation) {
				size.Height = CursorSize.Height;
			}
			return size;
		}
		Size CalcLocationOffset() {
			if(Location == ToolTipLocation.Fixed) return Size.Empty;
			Size size = new Size(-Bounds.Width, -Bounds.Height);
			Size offset = CalcControlLocationOffSet(Bounds.Size);
			size.Width += offset.Width;
			size.Height += offset.Height;
			return size;
		}
		Size CalcControlLocationOffSet(Size controlSize) {
			Size size = Size.Empty;
			if(IsBottomLocation) {
				size.Height = controlSize.Height;
			}
			if(IsRightLocation) {
				size.Width = controlSize.Width;
			}
			if(Location == ToolTipLocation.LeftCenter || Location == ToolTipLocation.RightCenter) {
				size.Height = controlSize.Height / 2;
			}
			if(Location == ToolTipLocation.BottomCenter || Location == ToolTipLocation.TopCenter) {
				size.Width = controlSize.Width / 2;
			}
			if(IsLeftLocation) {
			}
			return size;
		}
		protected virtual Size CursorSize {
			get {
				return CursorBounds.Size;
			}
		}
		protected virtual Size CalcBoundsSize() { return CalcBoundsSize(Size.Empty); }
		protected abstract Size CalcBoundsSize(Size contentSize);
		protected void CalcBounds(Size contentSize, Point cursorPosition, Rectangle objectBounds) {
			bounds.Size = CalcBoundsSize(contentSize);
			bounds.Location = CalcBoundsLocation(cursorPosition, objectBounds);
		}
		Point CalcBoundsLocation(Point cursorPosition, Rectangle objectBounds) {
			return GetToolTipLocation(cursorPosition, objectBounds);
		}
		Point GetToolTipLocation(Point cursorPosition, Rectangle objectBounds) {
			if(!objectBounds.IsEmpty)
				return GetToolTipLocationByBounds(objectBounds);
			return GetToolTipLocationByCursorPosition(cursorPosition);
		}
		protected virtual Point GetToolTipLocationByCursorPosition(Point cursorPosition) {
			Rectangle r = CalcToolTipRectangle(cursorPosition, Rectangle.Empty);
			Rectangle workArea = GetWorkingArea(cursorPosition);
			if(workArea.Contains(r))
				return r.Location;
			else {
				if(InRange(workArea.X, workArea.Right, r.X) && InRange(workArea.X, workArea.Right, r.Right)) {
					if(r.Bottom > workArea.Bottom)
						r.Y = workArea.Bottom - r.Height;
					else {
						if(r.Y < workArea.Y) r.Y = workArea.Y;
					}
					if(workArea.Contains(r)) return r.Location;
				}
				if(Location == ToolTipLocation.Fixed) return CalcFixedLocation(workArea, r);
				r = CalcToolTipRectangle(ReverseToolTipLocation(), cursorPosition, Rectangle.Empty);
				if(workArea.Contains(r))
					return r.Location;
				else {
					r = CalcToolTipRectangle(ReverseToolTipLocation2(), cursorPosition, Rectangle.Empty);
					if(workArea.Contains(r))
						return r.Location;
					else {
						r = CalcToolTipRectangle(ReverseToolTipLocation(), cursorPosition, Rectangle.Empty);
						if(workArea.Contains(r))
							return r.Location;
						else {
							ShowArgs.ShowBeak = false;
							r = CalcToolTipRectangle(ReverseToolTipLocation2(), cursorPosition, Rectangle.Empty);
							r.X = Math.Max(r.X, workArea.X + 5);
							if(r.Right > workArea.Right) r.X = Math.Max(workArea.X, workArea.Right - r.Width - 5);
							r.Y = Math.Max(r.Y, workArea.Y + 5);
							if(r.Bottom > workArea.Bottom) r.Y = Math.Max(workArea.Y, workArea.Bottom - r.Height - 5);
							return r.Location;
						}
					}
				}
			}
		}
		protected virtual Point GetToolTipLocationByBounds(Rectangle objectBounds) {
			Rectangle r = CalcToolTipRectangle(Point.Empty, objectBounds);
			Rectangle workArea = GetWorkingArea(objectBounds.Location);
			if(workArea.Contains(r))
				return r.Location;
			if(r.Right > workArea.Right || r.Left < workArea.Left)
				r = CalcToolTipRectangle(ReverseToolTipLocationHorizontal(), Point.Empty, objectBounds);
			if(r.Bottom > workArea.Bottom || r.Top < workArea.Top)
				r = CalcToolTipRectangle(ReverseToolTipLocationVertical(), Point.Empty, objectBounds);
			if(r.Right > workArea.Right)
				r.X = workArea.Right - r.Width;
			if(r.Left < workArea.Left)
				r.X = workArea.Left;
			if(r.Top < workArea.Top)
				r.Y = workArea.Top;
			if(r.Bottom > workArea.Bottom)
				r.Y = workArea.Bottom - r.Height;
			return r.Location;
		}
		Point CalcFixedLocation(Rectangle workArea, Rectangle bounds) {
			if(bounds.Right > workArea.Right) bounds.X = workArea.Right - bounds.Width;
			if(bounds.X < workArea.X) bounds.X = workArea.X;
			if(bounds.Bottom > workArea.Bottom) bounds.Y = workArea.Bottom - bounds.Height;
			if(bounds.Y < workArea.Y) bounds.Y = workArea.Y;
			return bounds.Location;
		}
		protected virtual Rectangle GetWorkingArea(Point pt) {
			if(SystemInformation.MonitorCount > 1) {
				if(ControlUtils.UseVirtualScreenForDropDown)
					return Screen.FromPoint(pt).Bounds;
				return Screen.FromPoint(pt).WorkingArea;
			}
			return SystemInformation.VirtualScreen;
		}
	}
	public class ToolTipViewInfo : ToolTipViewInfoBase {
		public const int ObjectsOffset = 3;
		public const int DefaultBorderSize = 3;
		public const int NativeDefaultBorderSize = 6;
		public const int DefaultOutOffHeight = 15;
		public const int DefaultOutOffWidth = 15;
		Rectangle contentBounds, imageBounds, textBounds, titleBounds;
		AppearanceObject paintAppearance;
		AppearanceObject paintAppearanceTitle;
		StringInfo htmlTitleInfo;
		StringInfo htmlTextInfo;
		public ToolTipViewInfo() {
			this.contentBounds = Rectangle.Empty;
			this.paintAppearance = new AppearanceObject();
			this.paintAppearanceTitle = new AppearanceObject();
		}
		public AppearanceObject PaintAppearance { get { return paintAppearance; } }
		public AppearanceObject PaintAppearanceTitle { get { return paintAppearanceTitle; } }
		public override void Clear() {
			base.Clear();
			imageBounds = textBounds = titleBounds = Rectangle.Empty;
		}
		public Rectangle ContentBounds { get { return contentBounds; } }
		protected Rectangle ElementBounds {
			get {
				Rectangle rect = ContentBounds;
				rect.Width -= CalcBorderSize() * 2;
				rect.Height -= CalcBorderSize() * 2;
				rect.X += CalcBorderSize();
				rect.Y += CalcBorderSize();
				return rect;
			}
		}
		public Point ContentScreenLocation { get { return new Point(Bounds.X + ContentBounds.X, Bounds.Y + ContentBounds.Y); } }
		public Rectangle ImageBounds { get { return imageBounds; } }
		public Rectangle TextBounds { get { return textBounds; } }
		public Rectangle TitleBounds { get { return titleBounds; } }
		public virtual int OutOffHeight { get { return DefaultOutOffHeight; } }
		public virtual int OutOffWidth { get { return DefaultOutOffWidth; } }
		protected string ToolTip { get { return ShowArgs.ToolTip; } }
		protected bool ShowBeak { get { return ShowArgs.ShowBeak; } }
		public StringInfo HtmlTitleInfo {
			get {
				if (htmlTitleInfo == null) 
					htmlTitleInfo = new StringInfo();
				return htmlTitleInfo;
			}
			set {
				htmlTitleInfo = value;
			}
		}
		public StringInfo HtmlTextInfo {
			get {
				if (htmlTextInfo == null)
					htmlTextInfo = new StringInfo();
				return htmlTextInfo;
			}
			set {
				htmlTextInfo = value;
			}
		}
		protected override void CalculateInternal(Size contentSize, Point cursorPosition, Rectangle objectBounds) {
			UpdatePaintAppearance();
			this.textBounds.Size = CalcToolTipSize();
			this.titleBounds.Size = CalcTitleSize();
			this.imageBounds.Size = CalcImageSize();
			if(contentSize.IsEmpty) {
				contentSize = CalcContentSize();
				contentSize.Width += CalcBorderSize() * 2;
				contentSize.Height += CalcBorderSize() * 2;
			}
			contentBounds.Size = contentSize;
			base.CalcBounds(contentSize, cursorPosition, objectBounds);
			contentBounds.Location = CalcContentBoundsLocation();	
			CalculateElementsLocation();
			TextSizeCorrect(); 
		}
		protected virtual void UpdatePaintAppearance() {
			UserLookAndFeel look = GetLookAndFeel(ShowArgs);
			AppearanceHelper.Combine(this.paintAppearance, new AppearanceObject[] { ShowArgs.Appearance }, GetDefAppearanceTextObject(look));
			AppearanceHelper.Combine(this.paintAppearanceTitle, new AppearanceObject[] { ShowArgs.AppearanceTitle }, GetDefAppearanceTitleObject(look));
			this.paintAppearance.TextOptions.RightToLeft = ShowArgs.RightToLeft;
			this.paintAppearanceTitle.TextOptions.RightToLeft = ShowArgs.RightToLeft;
		}
		protected virtual AppearanceDefault GetDefAppearanceTextObject(UserLookAndFeel look) {
			if(ToolTipController.ToolTipControllerHelper.ShouldUseVistaStyleTooltip(ToolTipStyle))
				return new AppearanceDefault(LookAndFeelHelper.GetSystemColor(look, NativeToolTipTextColor), LookAndFeelHelper.GetSystemColor(look, SystemColors.Info), Color.Black);
			return new AppearanceDefault(LookAndFeelHelper.GetSystemColor(look, SystemColors.InfoText), LookAndFeelHelper.GetSystemColor(look, SystemColors.Info), Color.Black);
		}
		protected virtual AppearanceDefault GetDefAppearanceTitleObject(UserLookAndFeel look) {
			if(ToolTipController.ToolTipControllerHelper.ShouldUseVistaStyleTooltip(ToolTipStyle))
				return new AppearanceDefault(LookAndFeelHelper.GetSystemColor(look, NativeToolTipTextColor), LookAndFeelHelper.GetSystemColor(look, SystemColors.Info), Color.Black, new Font(AppearanceObject.DefaultFont, FontStyle.Bold));
			return new AppearanceDefault(LookAndFeelHelper.GetSystemColor(look, SystemColors.InfoText), LookAndFeelHelper.GetSystemColor(look, SystemColors.Info), Color.Black, new Font(AppearanceObject.DefaultFont, FontStyle.Bold));
		}
		protected virtual Color NativeToolTipTextColor { get { return Color.FromArgb(0x37, 0x3A, 0x3D); } }
		protected virtual int CalcBorderSize() {
			int res = GetBorderSizeCore();
			if(!ShowArgs.Rounded) return res;
			return res * 2; 
		}
		protected virtual int GetBorderSizeCore() {
			if(ToolTipController.ToolTipControllerHelper.ShouldUseVistaStyleTooltip(ToolTipStyle))
				return NativeDefaultBorderSize;
			return DefaultBorderSize;
		}
		protected virtual Size CalcToolTipSize() {
			if(AllowHtmlText) {
				HtmlTextInfo = CalcHtmlText(ToolTip, PaintAppearance);
				return HtmlTextInfo.Bounds.Size;
			}
			return CalcTextSize(ToolTip, PaintAppearance);
		}
		protected virtual Size CalcTitleSize() {
			if (AllowHtmlText) {
				HtmlTitleInfo = CalcHtmlText(ShowArgs.Title, PaintAppearanceTitle);
				return HtmlTitleInfo.Bounds.Size;
			}
			return CalcTextSize(ShowArgs.Title, PaintAppearanceTitle);
		}
		protected virtual StringInfo CalcHtmlText(string text, AppearanceObject appearance) {
			if(text == string.Empty) return new StringInfo();
			GraphicsInfo.Default.AddGraphics(null);
			try {
				StringCalculateArgs args = new StringCalculateArgs(GraphicsInfo.Default.Graphics, appearance, TextOptions.DefaultOptionsMultiLine, text, new Rectangle(Point.Empty, new Size((int)GetWorkingArea(Point.Empty).Width * 2 / 3, int.MaxValue)), null);
				args.AllowBaselineAlignment = false;
				StringInfo info = StringPainter.Default.Calculate(args);
				int minX = int.MaxValue, maxX = 0;
				int minY = int.MaxValue, maxY = 0;
				if(info.BlocksBounds != null) {
					foreach(Rectangle r in info.BlocksBounds) {
						minY = Math.Min(minY, r.Y);
						maxY = Math.Max(maxY, r.Bottom);
						maxX = Math.Max(r.Right, maxX);
						minX = Math.Min(r.X, minX);
					}
				}
				args = new StringCalculateArgs(GraphicsInfo.Default.Graphics, appearance, TextOptions.DefaultOptionsMultiLine, text, new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1), null);
				args.AllowBaselineAlignment = false;
				return StringPainter.Default.Calculate(args);
			}
			finally {
				GraphicsInfo.Default.ReleaseGraphics();
			}
		}
		protected virtual Size CalcTextSize(string text, AppearanceObject appearance) {
			if(text == string.Empty) return Size.Empty;
			Size size = Size.Empty;
			GraphicsInfo.Default.AddGraphics(null);
			try {
				size = appearance.CalcTextSize(GraphicsInfo.Default.Cache, appearance.GetStringFormat(TextOptions.DefaultOptionsMultiLine), 
					text, (int)GetWorkingArea(Point.Empty).Width * 2 /3).ToSize();
			} finally {
				GraphicsInfo.Default.ReleaseGraphics();
			}
			return size;
		}
		protected virtual Size CalcImageSize() {
			ToolTipControllerImage image = new ToolTipControllerImage(ShowArgs); 
			return image.ImageSize;
		}
		Size CalcContentSize() {
			Size size = TextBounds.Size;
			Size titleSize = TitleBounds.Size;
			if(!titleSize.IsEmpty) {
				titleSize = AddImageSize(titleSize);
			} else {
				size = AddImageSize(size);
			}
			if(!titleSize.IsEmpty) {
				size.Width = Math.Max(size.Width, titleSize.Width);
				size.Height += titleSize.Height + ObjectsOffset;
			}
			return size;
		}
		Size AddImageSize(Size size) {
			if(ImageBounds.Size.IsEmpty) return size;
			size.Width += ImageBounds.Size.Width + ObjectsOffset;
			size.Height = Math.Max(size.Height, ImageBounds.Size.Height);
			return size;
		}
		protected override Size CalcBoundsSize(Size contentSize) {
			Size size = ContentBounds.Size;
			if(!ShowBeak)
				return size;
			if(IsBeakEnlargeWidth)
				size.Width += OutOffHeight;
			else size.Height += OutOffHeight;
			return size;
		}
		bool IsBeakEnlargeWidth {
			get { return (int)Location >= (int)ToolTipLocation.LeftTop; }
		}
		Point CalcContentBoundsLocation() {
			if(!ShowBeak)
				return Point.Empty;
			if((int)Location <= (int)ToolTipLocation.BottomRight)
				return new Point(0, OutOffHeight);
			if((int)Location >= (int)ToolTipLocation.RightTop)
				return new Point(OutOffHeight, 0);
			return Point.Empty;
		}
		void CalculateElementsLocation() {
			imageBounds.X = ElementBounds.X;
			int imageOffset = !ImageBounds.Size.IsEmpty ? ObjectsOffset : 0;
			if(!TitleBounds.IsEmpty) {
				int height = Math.Max(ImageBounds.Height, TitleBounds.Height);
				imageBounds.Y = ElementBounds.Y + (height - ImageBounds.Height) / 2;
				titleBounds.Y = ElementBounds.Y + (height - TitleBounds.Height) / 2;
				titleBounds.X = ElementBounds.X + ImageBounds.Width + imageOffset;
				if(PaintAppearanceTitle.TextOptions.HAlignment == HorzAlignment.Center)
					titleBounds.X += (ElementBounds.Width - ImageBounds.Width - imageOffset - TitleBounds.Width) / 2;
				if(PaintAppearanceTitle.TextOptions.HAlignment == HorzAlignment.Far) {
					titleBounds.X = ElementBounds.Right - TitleBounds.Width;
				}
				textBounds.Y = ElementBounds.Y + height + ObjectsOffset; 
				textBounds.X = ElementBounds.X;
			} else {
				textBounds.X = ElementBounds.X + ImageBounds.Width + imageOffset;
				imageBounds.Y = ElementBounds.Y + (ElementBounds.Height - ImageBounds.Height) / 2;
				textBounds.Y = ElementBounds.Y + (ElementBounds.Height - TextBounds.Height) / 2;
			}
			if(ShowArgs.RightToLeft) {
				imageBounds = ReverceBounds(imageBounds, ElementBounds);
				textBounds = ReverceBounds(textBounds, ElementBounds);
			}
			if (AllowHtmlText) {
				HtmlTitleInfo.SetLocation(TitleBounds.Location);
				HtmlTextInfo.SetLocation(TextBounds.Location);
			}
		}
		private Rectangle ReverceBounds(Rectangle itemBounds, Rectangle bounds) {
			return new Rectangle(bounds.Right - (itemBounds.X - bounds.X) - itemBounds.Width, itemBounds.Y, itemBounds.Width, itemBounds.Height);
		}
		void TextSizeCorrect() {
			if(!TitleBounds.Size.IsEmpty &&
				ElementBounds.Width > textBounds.Width &&
				(PaintAppearance.TextOptions.HAlignment == HorzAlignment.Center ||
						 PaintAppearance.TextOptions.HAlignment == HorzAlignment.Far))
				textBounds.Width = ElementBounds.Width;
		}
	}
}
namespace DevExpress.Utils {
	public class ToolTipControlInfo {
		ToolTipLocation toolTipLocation;
		Point toolTipPosition;
		bool immediateToolTip;
		object _object;
		string text;
		string title;
		DefaultBoolean allowHtmlText;
		ToolTipIconType iconType;
		int interval = -1;
		SuperToolTip superTip;
		ToolTipType toolTipType = ToolTipType.Default;
		Image toolTipImage = null;
		bool applyCursorOffset = ToolTipViewInfoBase.DefaultApplyCursorOffset;
		public ToolTipControlInfo() : this(null, "") { }
		public ToolTipControlInfo(object _object, string text, ToolTipIconType iconType) : this(_object, text, false, iconType) { }
		public ToolTipControlInfo(object _object, string text) : this(_object, text, false, ToolTipIconType.None) {}
		public ToolTipControlInfo(object _object, string text, string title, ToolTipIconType iconType) : this(_object, text, title, false, iconType) { }
		public ToolTipControlInfo(object _object, string text, string title) : this(_object, text, title, false, ToolTipIconType.None) {}
		public ToolTipControlInfo(object _object, string text, bool immediateToolTip, ToolTipIconType iconType) : this(_object, text, "", immediateToolTip, iconType) {}
		public ToolTipControlInfo(object _object, string text, string title, bool immediateToolTip, ToolTipIconType iconType) : this(_object, text, title, immediateToolTip, iconType, DefaultBoolean.Default) { }
		public ToolTipControlInfo(object _object, string text, string title, bool immediateToolTip, ToolTipIconType iconType, DefaultBoolean allowHtmlText) {
			this._object = _object;
			this.text = text;
			this.title = title;
			this.immediateToolTip = immediateToolTip;
			this.iconType = iconType;
			this.toolTipLocation = ToolTipLocation.Default;
			this.toolTipPosition = new Point(-10000, -10000);
			this.allowHtmlText = allowHtmlText;
			this.ForcedShow = DefaultBoolean.Default;
		}
		public DefaultBoolean ForcedShow { get; set; }
		public bool HideHintOnMouseMove { get; set; }
		protected internal bool ApplyCursorOffset { get { return applyCursorOffset; } set { applyCursorOffset = value; } }
#if !SL
	[DevExpressUtilsLocalizedDescription("ToolTipControlInfoToolTipImage")]
#endif
		public Image ToolTipImage {
			get { return toolTipImage; }
			set { toolTipImage = value; }
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("ToolTipControlInfoToolTipLocation")]
#endif
		public ToolTipLocation ToolTipLocation { get { return toolTipLocation; } set { toolTipLocation = value; } }
#if !SL
	[DevExpressUtilsLocalizedDescription("ToolTipControlInfoToolTipPosition")]
#endif
		public Point ToolTipPosition { get { return toolTipPosition; } set { toolTipPosition = value; } }
#if !SL
	[DevExpressUtilsLocalizedDescription("ToolTipControlInfoSuperTip")]
#endif
		public SuperToolTip SuperTip {
			get { return superTip; }
			set { superTip = value; }
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("ToolTipControlInfoInterval")]
#endif
		public int Interval { get { return interval; } set { interval = value; } }
#if !SL
	[DevExpressUtilsLocalizedDescription("ToolTipControlInfoText")]
#endif
		public string Text {
			get { return text; }
			set {
				if(value == null) value = string.Empty;
				text = value;
			}
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("ToolTipControlInfoObjectBounds")]
#endif
		public Rectangle ObjectBounds { get; set; }
#if !SL
	[DevExpressUtilsLocalizedDescription("ToolTipControlInfoToolTipType")]
#endif
		public ToolTipType ToolTipType {
			get { return toolTipType; }
			set { toolTipType = value; }
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("ToolTipControlInfoTitle")]
#endif
		public string Title {
			get { return title; }
			set {
				if(value == null) value = string.Empty;
				title = value;
			}
		}
		public virtual void Normalize() {
			Text = NormalizeSimpleText(text);
			Title = NormalizeSimpleText(title);
		}
		protected internal static string NormalizeSimpleText(string input) {
			if(input == null) return null;
			int nullPos = input.IndexOf('\0');
			if(nullPos != -1) {
				return input.Remove(nullPos);
			}
			return input;
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("ToolTipControlInfoObject")]
#endif
		public object Object { get { return _object; } set { _object = value; } }
#if !SL
	[DevExpressUtilsLocalizedDescription("ToolTipControlInfoImmediateToolTip")]
#endif
		public bool ImmediateToolTip { get { return immediateToolTip; } set { immediateToolTip = value; } }
#if !SL
	[DevExpressUtilsLocalizedDescription("ToolTipControlInfoIconType")]
#endif
		public ToolTipIconType IconType { get { return iconType; } set { iconType = value; } }
#if !SL
	[DevExpressUtilsLocalizedDescription("ToolTipControlInfoAllowHtmlText")]
#endif
		public DefaultBoolean AllowHtmlText { get { return allowHtmlText; } set { allowHtmlText = value; } }
	}
	public enum ToolTipType { Default, Standard, SuperTip }
	[System.Runtime.InteropServices.ComVisible(false)]
	public interface IToolTipControlClient {
		ToolTipControlInfo GetObjectInfo(Point point);
		bool ShowToolTips {get;}
	}
	public interface IToolTipControlClientEx : IToolTipControlClient {
		void OnBeforeShow(ToolTipControllerShowEventArgs e);
	}
	public enum ToolTipLocation {BottomLeft = 0, BottomCenter = 1, BottomRight = 2, 
		TopLeft = 3, TopCenter = 4, TopRight = 5,
		LeftTop = 6, LeftCenter = 7, LeftBottom = 8,
		RightTop = 9, RightCenter = 10, RightBottom = 11, Default = 12, Fixed = 13};
	public enum ToolTipIconType {Application = 0, Asterisk = 1, Error = 2, 
		Exclamation = 3, Hand = 4, Information = 5, Question = 6, Warning = 7, 
		WindLogo = 8, None = 9};
	public enum ToolTipIconSize {Large, Small};
	public enum ToolTipStyle { Default, WindowsXP, Windows7 };
	public class ToolTipControllerEventArgsBase : EventArgs {
		Control selectedControl;
		object selectedObject;
		public ToolTipControllerEventArgsBase() {
			this.selectedControl = null;
			this.selectedObject = null;
		}
		public ToolTipControllerEventArgsBase(Control control, object obj) {
			this.selectedControl = control;
			this.selectedObject = obj;
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("ToolTipControllerEventArgsBaseSelectedControl")]
#endif
		public Control SelectedControl { get { return this.selectedControl; } set { this.selectedControl = value; } }
#if !SL
	[DevExpressUtilsLocalizedDescription("ToolTipControllerEventArgsBaseSelectedObject")]
#endif
		public object SelectedObject { get { return this.selectedObject; } set { this.selectedObject = value; } }
	}
	public class ToolTipControllerShowEventArgs : ToolTipControllerEventArgsBase, ICloneable  {
		protected internal const ToolTipLocation DefaultToolTipLocation = ToolTipLocation.BottomRight;
		protected internal const int DefaultRoundRadius = 7;
		protected internal const int MinRoundRadius = 1;
		protected internal const int MaxRoundRadius = 15;
		string toolTip;
		string title;
		AppearanceObject appearance;
		AppearanceObject appearanceTitle;
		ToolTipLocation toolTipLocation;
		bool rounded, showBeak;
		DefaultBoolean allowHtmlText;
		int roundRadius;
		bool show = true;
		ToolTipIconType iconType;
		ToolTipIconSize iconSize;
		object imageList;
		int imageIndex;
		bool autoHide;
		ToolTipType toolTipType;
		ToolTipStyle styleCore;
		SuperToolTip superTip;
		Image toolTipImage;
		bool useCustomSize;
		Size customSize;
		protected virtual void InitProperties() {
			this.superTip = new SuperToolTip();
			this.toolTipType = ToolTipType.Default;
			this.toolTip = string.Empty;
			this.title = string.Empty;
			this.toolTipLocation = ToolTipLocation.Default;
			this.rounded = false;
			this.showBeak = false;
			this.roundRadius = DefaultRoundRadius;
			this.iconSize = ToolTipIconSize.Small;
			this.iconType = ToolTipIconType.None;
			this.imageList = null;
			this.imageIndex = -1;
			this.autoHide = true;
			this.allowHtmlText = DefaultBoolean.Default;
			this.styleCore = ToolTipStyle.Default;
		}
		protected ToolTipControllerShowEventArgs(Control control, object obj, object dummy) : base(control, obj) {
			this.appearance = new AppearanceObject();
			this.appearanceTitle = new AppearanceObject();
		}
		public ToolTipControllerShowEventArgs(): this(null, null, null) {
			InitProperties();
		}
		public ToolTipControllerShowEventArgs(Control control, object obj): this(control, obj, null) {
			InitProperties();
		}
		public ToolTipControllerShowEventArgs(Control control, object obj, string toolTip, string title, ToolTipLocation toolTipLocation, bool showBeak, bool rounded, 
			int roundRadius, ToolTipIconType iconType, ToolTipIconSize iconSize, object imageList, int imageIndex, AppearanceObject appearance, AppearanceObject appearanceTitle)
			: this(control, obj, toolTip, title, toolTipLocation, showBeak, rounded, roundRadius, ToolTipStyle.WindowsXP, iconType, iconSize, imageList, imageIndex, appearance, appearanceTitle) {
		}
		public ToolTipControllerShowEventArgs(Control control, object obj,
			string toolTip, string title, ToolTipLocation toolTipLocation, 
			bool showBeak, bool rounded, int roundRadius, ToolTipStyle style,
			ToolTipIconType iconType, ToolTipIconSize iconSize, object imageList, int imageIndex, 
			AppearanceObject appearance, AppearanceObject appearanceTitle) : this(control, obj, null) {
			if(appearance != null) this.appearance.Assign(appearance);
			if(appearanceTitle != null) this.appearanceTitle.Assign(appearanceTitle);
			this.toolTip = toolTip;
			this.title = title;
			this.toolTipLocation = toolTipLocation;
			this.showBeak = showBeak;
			this.rounded = rounded;
			this.styleCore = style;
			RoundRadius = roundRadius;
			this.iconSize = iconSize;
			this.iconType = iconType;
			this.imageList = imageList;
			this.imageIndex = imageIndex;
		}
		internal bool UseCustomSize { get { return useCustomSize; } set { useCustomSize = value; } }
		internal Size CustomSize { get { return customSize; } set { customSize = value; } }
		public Image ToolTipImage {
			get { return toolTipImage; }
			set { toolTipImage = value; }
		}
		public SuperToolTip SuperTip { 
			get { return superTip; } 
			set {
				if(value != null) value = value.Clone() as SuperToolTip;
				superTip = value; 
			} 
		}
		public ToolTipLocation GetToolTipLocation() {
			if(ToolTipLocation == ToolTipLocation.Default) return DefaultToolTipLocation;
			return ToolTipLocation;
		}
		public ToolTipType ToolTipType { get { return toolTipType; } set { toolTipType = value; } }
		public override bool Equals(object obj) {
			if(obj is ToolTipControllerShowEventArgs) {
				ToolTipControllerShowEventArgs e = obj as ToolTipControllerShowEventArgs;
				return (e.ToolTip == ToolTip) && (e.Title == Title) && (e.ToolTipLocation == ToolTipLocation) && (e.ShowBeak == ShowBeak)
					&& (e.Rounded == Rounded) && (e.RoundRadius == RoundRadius) && (e.ToolTipStyle == ToolTipStyle)
					&& (e.SelectedControl == SelectedControl) && (e.SelectedObject == e.SelectedObject)
					&& (e.IconSize == IconSize) && (e.IconType == IconType)
					&& (e.ImageList == ImageList) && (e.ImageIndex == ImageIndex)
					&& (e.ToolTipType == ToolTipType) && 
					e.ObjectBounds == ObjectBounds;
					;
			} else return base.Equals(obj);
		}
		public AppearanceObject Appearance { get { return appearance; } }
		public AppearanceObject AppearanceTitle { get { return appearanceTitle; } }
		public override int GetHashCode() { return -1; }
		public string ToolTip { 
			get { return toolTip; } 
			set { 
				if(value == null) value = String.Empty;
				toolTip = value; 
			} 
		}
		public Rectangle ObjectBounds { get; set; }
		public string Title { 
			get { return title; } 
			set { 
				if(value == null) value = String.Empty;
				title = value; 
			} 
		}
		public ToolTipLocation ToolTipLocation { get { return this.toolTipLocation; } set { this.toolTipLocation = value; } }
		public bool AutoHide { get { return this.autoHide; } set { this.autoHide = value; } }
		public bool ShowBeak { get { return this.showBeak; } set { this.showBeak = value; } }
		public DefaultBoolean AllowHtmlText { get { return this.allowHtmlText; } set { this.allowHtmlText = value; } }
		public bool Rounded { get { return this.rounded; } set { this.rounded = value; } }
		public ToolTipStyle ToolTipStyle { get { return this.styleCore; } set { this.styleCore = value; } }
		public int RoundRadius { 
			get { return this.roundRadius; } 
			set { 
				if(value > MaxRoundRadius) {
					this.roundRadius = MaxRoundRadius; 
					return;
				}
				if(value < MinRoundRadius) {
					this.roundRadius = MinRoundRadius; 
					return;
				}
				this.roundRadius = value; 
			} 
		}
		public bool Show { 
			get { return this.show; } 
			set { this.show = value; }
		}
		public ToolTipIconType IconType { get { return this.iconType; } set { this.iconType = value; } }
		public ToolTipIconSize IconSize { get { return this.iconSize; } set { this.iconSize = value; } }
		public object ImageList { get { return this.imageList; } set { this.imageList = value; } }
		public int ImageIndex { get { return this.imageIndex; } set { this.imageIndex = value; } }
		object ICloneable.Clone() {
			ToolTipControllerShowEventArgs showArgs = new ToolTipControllerShowEventArgs(SelectedControl, SelectedObject, ToolTip, Title,
				ToolTipLocation, ShowBeak, Rounded, RoundRadius, ToolTipStyle, IconType, IconSize, ImageList, ImageIndex, Appearance, AppearanceTitle);
			showArgs.AutoHide = AutoHide;
			showArgs.allowHtmlText = AllowHtmlText;
			showArgs.customSize = CustomSize;
			showArgs.useCustomSize = UseCustomSize;
			showArgs.ObjectBounds = ObjectBounds;
			return showArgs;
		}
		public bool RightToLeft { get; set; }
	}
	public class ToolTipControllerGetActiveObjectInfoEventArgs : ToolTipControllerEventArgsBase {
		ToolTipControlInfo info;
		Point controlMousePosition;
		public ToolTipControllerGetActiveObjectInfoEventArgs(Control control, object obj, ToolTipControlInfo info, Point controlMousePosition) : base(control, obj) {
			this.info = info;
			this.controlMousePosition = controlMousePosition;
		}
		public ToolTipControlInfo Info { get { return info; } set { info = value; } }
		public Point ControlMousePosition { get { return controlMousePosition; } }
	}
	public class ToolTipControllerCalcSizeEventArgs : ToolTipControllerEventArgsBase {
		string toolTip;
		string title;
		Size size;
		Point position;
		ToolTipControllerShowEventArgs showArgs;
		public ToolTipControllerCalcSizeEventArgs(Control control, object obj, ToolTipControllerShowEventArgs showArgs) : this(control, obj, showArgs.ToolTip, showArgs.Title) {
			this.showArgs = showArgs;
		}
		public ToolTipControllerCalcSizeEventArgs(Control control, object obj, string toolTip, string title) : this(control, obj, toolTip, title, Size.Empty, Point.Empty) {
		}
		public ToolTipControllerCalcSizeEventArgs(Control control, object obj,
			string toolTip, string title, Size size, Point position) : base(control, obj) {
			this.toolTip = toolTip;
			this.title = title;
			this.size = size;
			this.position = position;
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("ToolTipControllerCalcSizeEventArgsToolTip")]
#endif
		public string ToolTip { get { return this.toolTip; } }
#if !SL
	[DevExpressUtilsLocalizedDescription("ToolTipControllerCalcSizeEventArgsTitle")]
#endif
		public string Title { get { return this.title; } }
#if !SL
	[DevExpressUtilsLocalizedDescription("ToolTipControllerCalcSizeEventArgsPosition")]
#endif
		public Point Position { get { return this.position; } set { this.position = value; } }
#if !SL
	[DevExpressUtilsLocalizedDescription("ToolTipControllerCalcSizeEventArgsSize")]
#endif
		public Size Size { get { return this.size; } set { this.size = value; } }
#if !SL
	[DevExpressUtilsLocalizedDescription("ToolTipControllerCalcSizeEventArgsShowInfo")]
#endif
		public ToolTipControllerShowEventArgs ShowInfo { get { return this.showArgs; }}
	}
	public delegate void ToolTipControllerBeforeShowEventHandler(object sender, ToolTipControllerShowEventArgs e);
	public delegate void ToolTipControllerCalcSizeEventHandler(object sender, ToolTipControllerCalcSizeEventArgs e);
	public delegate void ToolTipControllerCustomDrawEventHandler(object sender, ToolTipControllerCustomDrawEventArgs e);
	public delegate void ToolTipControllerGetActiveObjectInfoEventHandler(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e);
	public delegate void HyperlinkClickEventHandler(object sender, HyperlinkClickEventArgs e);
	public class ToolTipControllerImage {
		ToolTipControllerShowEventArgs showArgs;
		public ToolTipControllerImage(ToolTipControllerShowEventArgs showArgs) {
			this.showArgs = showArgs;
		}
		public ToolTipControllerShowEventArgs ShowArgs { get { return showArgs; } }
		public Size ImageSize {
			get {
				if(!HasImage) return Size.Empty;
				if(ImageList != null) return ImageCollection.GetImageListSize(ImageList);
				if(ShowArgs.IconType != ToolTipIconType.None) return ShowArgs.IconSize == ToolTipIconSize.Large ? new Size(32, 32) : new Size(16, 16);
				return ShowArgs.ToolTipImage.Size;
			}
		}
		public bool HasImage {
			get {
				if(ShowArgs.IconType != ToolTipIconType.None) return true;
				if(ImageList != null) return ImageCollection.IsImageListImageExists(ShowArgs.ImageList, ShowArgs.ImageIndex);
				return ShowArgs.ToolTipImage != null;
			}
		}
		public void Draw(GraphicsCache cache, int x, int y) {
			if(!HasImage) return;
			Rectangle rect = new Rectangle(new Point(x, y), ImageSize);
			if(ImageList != null) {
				ImageCollection.DrawImageListImage(cache, ImageList, ShowArgs.ImageIndex, rect);
			} else {
				if(GetIcon() != null) 
					cache.Graphics.DrawIcon(GetIcon(), rect);
				else if(ShowArgs.ToolTipImage != null) 
					cache.Graphics.DrawImage(ShowArgs.ToolTipImage, rect);
			}
		}
		object ImageList { 
			get { 
				if(ImageCollection.IsImageListImageExists(ShowArgs.ImageList, ShowArgs.ImageIndex)) return ShowArgs.ImageList;
				return null;
			} 
		}
		public Icon GetIcon() {
			Icon icon;
			if(ShowArgs.IconSize == ToolTipIconSize.Small) {
				icon = SystemIconsHelper.GetSmallIcon(ShowArgs.IconType);
				if(icon != null) return icon;
			}
			icon = GetSystemIcon();
			return NativeVista.IsWindows8 ? StockIconHelper.GetWindows8AssociatedIcon(icon) : icon;			
		}
		protected Icon GetSystemIcon() {
			switch(ShowArgs.IconType) {
				case ToolTipIconType.Application: return SystemIcons.Application;
				case ToolTipIconType.Asterisk: return SystemIcons.Asterisk;
				case ToolTipIconType.Error: return SystemIcons.Error;
				case ToolTipIconType.Exclamation: return SystemIcons.Exclamation;
				case ToolTipIconType.Hand: return SystemIcons.Hand;
				case ToolTipIconType.Information: return SystemIcons.Information;
				case ToolTipIconType.Question: return SystemIcons.Question;
				case ToolTipIconType.Warning: return SystemIcons.Warning;
				case ToolTipIconType.WindLogo: return SystemIcons.WinLogo;
			}
			return null;
		}
		#region SystemIconsHelper
		class SystemIconsHelper {
			const int LR_SHARED = 0x8000;
			const int IMAGE_ICON = 1;
			const int SmallIconSize = 16;
			const int ApplicationIconId = 100, WarningIconId = 101, QuestionIconId = 102, ErrorIconId = 103, InformationIconId = 104;
			public static Icon GetSmallIcon(ToolTipIconType type) {
				IntPtr hIcon = IntPtr.Zero;
				try {
					int iconId = GetIconId(type);
					if(iconId < 0) return null;
					hIcon = NativeMethods.LoadImage(IntPtr.Zero, iconId, IMAGE_ICON, SmallIconSize, SmallIconSize, LR_SHARED);
					if(hIcon == IntPtr.Zero) return null;
					Icon icon = Icon.FromHandle(hIcon);
					return icon;
				}
				catch {
					return null;
				}
				finally {
					NativeMethods.DestroyIcon(hIcon);
				}
			}
			static int GetIconId(ToolTipIconType type) {
				switch(type) {
					case ToolTipIconType.Application:
					case ToolTipIconType.WindLogo:
						return ApplicationIconId;
					case ToolTipIconType.Error:
					case ToolTipIconType.Hand:
						return ErrorIconId;
					case ToolTipIconType.Asterisk:
					case ToolTipIconType.Information:
						return InformationIconId;
					case ToolTipIconType.Warning:
					case ToolTipIconType.Exclamation:
						return WarningIconId;
					case ToolTipIconType.Question:
						return QuestionIconId;
				}
				return -1;
			}
		}
		#endregion 
	}
	public class ToolTipControllerCustomDrawEventArgs : EventArgs {
		GraphicsCache cache;
		ToolTipControllerShowEventArgs showArgs;
		bool handled;
		Rectangle bounds;
		public ToolTipControllerCustomDrawEventArgs(GraphicsCache cache, ToolTipControllerShowEventArgs args, Rectangle bounds) {
			this.cache = cache;
			this.showArgs = args;
			this.bounds = bounds; 
			this.handled = false;
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("ToolTipControllerCustomDrawEventArgsCache")]
#endif
		public GraphicsCache Cache { get { return this.cache; }}
#if !SL
	[DevExpressUtilsLocalizedDescription("ToolTipControllerCustomDrawEventArgsShowInfo")]
#endif
		public ToolTipControllerShowEventArgs ShowInfo { get { return this.showArgs; }}
#if !SL
	[DevExpressUtilsLocalizedDescription("ToolTipControllerCustomDrawEventArgsBounds")]
#endif
		public Rectangle Bounds { get { return this.bounds; }}
#if !SL
	[DevExpressUtilsLocalizedDescription("ToolTipControllerCustomDrawEventArgsHandled")]
#endif
		public bool Handled { get { return this.handled; } set {this.handled = value; } }
	}
	[Designer("DevExpress.Utils.Design.ToolTipControllerDesigner, " + AssemblyInfo.SRAssemblyDesign),
	 ProvideProperty("ToolTip", typeof(Control)), 
	 ProvideProperty("Title", typeof(Control)),
	 ProvideProperty("ToolTipIconType", typeof(Control)),
	 ProvideProperty("SuperTip", typeof(Control)),
	 ProvideProperty("AllowHtmlText", typeof(Control)), 
	 Description("Manipulates tooltips for all Developer Express controls."),
	 ToolboxTabName(AssemblyInfo.DXTabComponents), DXToolboxItem(DXToolboxItemKind.Free),
	 ToolboxBitmap(typeof(DevExpress.Utils.ToolBoxIcons.ToolboxIconsRootNS), "DefaultToolTipController")
	]
	public class DefaultToolTipController : Component, System.ComponentModel.IExtenderProvider {
		public DefaultToolTipController(IContainer container) : this() {
			container.Add(this);
		}
		public DefaultToolTipController() {}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("DefaultToolTipControllerDefaultController"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ToolTipController DefaultController { get { return ToolTipController.DefaultController; } }
		bool IExtenderProvider.CanExtend(object target) {
			if ((target is Control) && !(target is IToolTipControlClient))
				return true;
			return false;
		}
		[DefaultValue(""), Localizable(true), Category("Tooltip")]
		public string GetToolTip(Control control) { return DefaultController.GetToolTip(control); }
		[DefaultValue(""), Localizable(true), Category("Tooltip")]
		public string GetTitle(Control control) { return DefaultController.GetTitle(control); }
		[DefaultValue(ToolTipIconType.None), Localizable(true), Category("Tooltip")]
		public ToolTipIconType GetToolTipIconType(Control control) { return DefaultController.GetToolTipIconType(control); }
		[DefaultValue(""), Localizable(true), Category("AllowHtmlText")]
		public DefaultBoolean GetAllowHtmlText(Control control) { return DefaultController.GetAllowHtmlText(control); }
		public void SetToolTip(Control control, string value) { DefaultController.SetToolTip(control, value); }
		public void SetTitle(Control control, string value) { DefaultController.SetTitle(control, value); }
		public void SetToolTipIconType(Control control, ToolTipIconType value) { DefaultController.SetToolTipIconType(control, value); }
		public void SetAllowHtmlText(Control control, DefaultBoolean value) { DefaultController.SetAllowHtmlText(control, value); }
		bool ShouldSerializeSuperTip(Control control) { return DefaultController.ShouldSerializeSuperTip(control); }
		[DefaultValue(null), Localizable(true), Category("Tooltip"),
		Editor("DevExpress.XtraEditors.Design.ToolTipContainerUITypeEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor))
		]
		public SuperToolTip GetSuperTip(Control control) { return DefaultController.GetSuperTip(control);  }
		public void SetSuperTip(Control control, SuperToolTip value) { DefaultController.SetSuperTip(control, value); }
		void ResetSuperTip(Control control) { DefaultController.SetSuperTip(control, null); }
	}
	public interface ISupportToolTipsForm {
		bool ShowToolTipsWhenInactive { get; }
		bool ShowToolTipsFor(Form form);
	}
	[Designer("DevExpress.Utils.Design.ToolTipControllerDesigner, " + AssemblyInfo.SRAssemblyDesign),
	 ProvideProperty("ToolTip", typeof(Control)), 
	 ProvideProperty("Title", typeof(Control)),
	 ProvideProperty("ToolTipIconType", typeof(Control)),
	 ProvideProperty("SuperTip", typeof(Control)),
	 ProvideProperty("AllowHtmlText", typeof(Control)),
	 Description("Manages tooltips for a specific control or controls."),
	 ToolboxTabName(AssemblyInfo.DXTabComponents), DXToolboxItem(DXToolboxItemKind.Free),
	 ToolboxBitmap(typeof(DevExpress.Utils.ToolBoxIcons.ToolboxIconsRootNS), "ToolTipController")
	]
	public class ToolTipController : Component, System.ComponentModel.IExtenderProvider {
		protected class ControlInfo {
			DefaultBoolean allowHtmlText;
			string text;
			string title;
			ToolTipIconType iconType;
			SuperToolTip superTip;
			public ControlInfo(string text) : this(text, ToolTipIconType.None) { }
			public ControlInfo(string text, ToolTipIconType iconType) : this(text, "", iconType) {}
			public ControlInfo(string text, string title, ToolTipIconType iconType) : this(text, title, iconType, DefaultBoolean.Default) {}
			public ControlInfo(string text, string title, ToolTipIconType iconType, DefaultBoolean allowHtmlText) {
				if (text == null) text = string.Empty;
				if (title == null) title = string.Empty;
				this.text = text;
				this.title = title;
				this.iconType = iconType;
				this.allowHtmlText = allowHtmlText;
			}
			public SuperToolTip SuperTip {
				get { return superTip; }
				set {
					if(value != null && value.IsEmpty) value = null;
					superTip = value;
				}
			}
			public string Text { 
				get { return text; } 
				set { 
					if(value == null) value = string.Empty;
					text = value;
				}
			}
			public string Title { 
				get { return title; } 
				set { 
					if(value == null) value = string.Empty;
					title = value;
				}
			}
			public ToolTipIconType IconType {
				get { return iconType; }
				set { iconType = value; }
			}
			public DefaultBoolean AllowHtmlText {
				get { return allowHtmlText; }
				set { allowHtmlText = value; }
			}
			public bool IsEmpty {
				get {
					return Text.Length == 0 &&
						Title.Length == 0 &&
						IconType == ToolTipIconType.None &&
						(SuperTip == null || SuperTip.IsEmpty);
				}
			}
			static ControlInfo empty;
			public static ControlInfo Empty {
				get {
					if(empty == null) empty = new ControlInfo(string.Empty);
					return empty;
				}
			}
		}
		internal const int DefaultReshowDelay = 100;
		internal const int DefaultInitialDelay = 500;
		internal const int DefaultAutoPopDelay = 5000;
		Hashtable controlClientOwners;
		SuperToolTip superTooltip;
		ToolTipControlInfo activeObjectInfo;
		ToolTipControllerBaseWindow toolWindow;
		ToolTipViewInfoBase viewInfo;
		int initialDelay, autoPopDelay, reshowDelay;
		bool active;
		Timer initialTimer, autoPopTimer;
		ClosingDelayTimer closingDelayTimer;
		Control activeControl;
		object activeObject;
		Hashtable toolTips;
		ToolTipControllerShowEventArgs showArgs;
		ToolTipControllerShowEventArgs currentShowArgs;
		Point oldCursorPosition;
		bool showShadow;
		bool allowHtmlText;
		int prevMaxWidth;
		ToolTipType toolTipType, currentToolTipType;
		[ThreadStatic]
		static ToolTipController defaultController;
		[ThreadStatic]
		static ToolTipController activeController;
		public ToolTipController(IContainer container) : this() {
			container.Add(this);
		}
		public ToolTipController() {
			this.superTooltip = new SuperToolTip();
			this.toolTipType = ToolTipType.Default;
			this.currentToolTipType = ToolTipType.SuperTip;
			this.toolTips = new Hashtable();
			this.toolWindow = null;
			this.activeObjectInfo = null;
			this.activeControl = null;
			this.activeObject = null;
			this.active = true;
			this.showArgs  = new ToolTipControllerShowEventArgs();
			this.showShadow = true;
			this.allowHtmlText = false;
			this.initialTimer = new Timer();
			this.autoPopTimer = new Timer();
			this.closingDelayTimer = new ClosingDelayTimer();
			AutoPopTimer.Tick += new EventHandler(OnAutoPopTimerTick);
			InitialTimer.Tick += new EventHandler(OnInitialTimerTick);
			ClosingDelayTimer.Interval = ClosingDelayInterval;
			this.controlClientOwners = new Hashtable();
			this.reshowDelay = DefaultReshowDelay;
			InitialDelay = DefaultInitialDelay;
			AutoPopDelay = DefaultAutoPopDelay;
			this.currentShowArgs = null;
			this.oldCursorPosition = Point.Empty;
			this.prevMaxWidth = this.superTooltip.MaxWidth;
		}
		protected virtual int ClosingDelayInterval { get { return 600; } }
		protected bool GetUseSuperTips(ToolTipControllerShowEventArgs eShow) { return GetToolTipType(eShow) == ToolTipType.SuperTip; }
		protected virtual ToolTipControllerBaseWindow CreateToolWindow() {
			if(GetUseSuperTips(CurrentShowArgs)) return new SuperToolTipWindow(SuperTooltip, ShowShadow);
			return ToolTipControllerStyledWindowBase.Create(this);
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("ToolTipControllerDefaultController")]
#endif
		static public ToolTipController DefaultController {
			get {
				if(defaultController == null)
					defaultController = new ToolTipControllerDefault();
				return defaultController;
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				this.autoPopTimer.Tick -= new EventHandler(OnAutoPopTimerTick);
				this.initialTimer.Tick -= new EventHandler(OnInitialTimerTick);
				this.initialTimer.Dispose();
				this.autoPopTimer.Dispose();
				this.closingDelayTimer.Dispose();
				DestroyToolWindow();
				if(activeController == this) {
					activeController = null;
				}
			}
			base.Dispose(disposing);
		}
		protected SuperToolTip SuperTooltip { get { return superTooltip; } }
		protected internal ToolTipViewInfoBase ViewInfo { 
			get {
				if(viewInfo == null) viewInfo = CreateViewInfo();
				return viewInfo; 
			} 
		}
		protected ToolTipType GetToolTipType() {
			if(CurrentShowArgs == null) {
				if(ToolTipType != ToolTipType.Default) return ToolTipType;
				return ToolTipType.Standard;
			}
			return GetToolTipType(CurrentShowArgs);
		}
		protected ToolTipType GetToolTipType(ToolTipControllerShowEventArgs e) {
			if (e == null) return ToolTipType.Standard;
			if(e.ToolTipType != ToolTipType.Default) return e.ToolTipType;
			if(this.ToolTipType != ToolTipType.Default) return ToolTipType;
			if(e.SuperTip != null) return ToolTipType.SuperTip;
			return ToolTipType.Standard;
		}
		protected ToolTipType CurrentToolTipType {
			get { return currentToolTipType; }
			set {
				if(CurrentToolTipType == value) return;
				currentToolTipType = value;
				DestroyToolWindow();
			}
		}
		protected virtual ToolTipViewInfoBase CreateViewInfo() {
			if(CurrentToolTipType == ToolTipType.SuperTip) return new ToolTipViewInfoSuperTip(SuperTooltip);
			return new ToolTipViewInfo();
		}
		protected virtual void DestroyToolWindow() {
			if(toolWindow != null) {
				toolWindow.Paint -= new PaintEventHandler(OnToolWindowPaint);
				toolWindow.MouseLeave -= new EventHandler(OnToolWindowMouseLeave);
				toolWindow.Dispose();
				this.toolWindow = null;
			}
			this.viewInfo = null;
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("ToolTipControllerActiveObjectInfo"),
#endif
 Browsable(false)]
		public virtual ToolTipControlInfo ActiveObjectInfo { 
			get { return activeObjectInfo; }
		}
		bool IsTheSameObjectInfo(ToolTipControlInfo value) {
			if(value != null && value.ForcedShow == DefaultBoolean.True) return false;
			if(ActiveObjectInfo == value) return true;
			if(ActiveObjectInfo != null && value != null && ActiveObjectInfo.Object == value.Object) return true;
			return false;
		}
		protected virtual void SetActiveObjectInfo(ToolTipControlInfo value) {
			if(IsTheSameObjectInfo(value))
				return;
			this.activeObjectInfo = value;
			SetActiveObject(ActiveObjectInfo != null ? ActiveObjectInfo.Object : null, value != null && value.ForcedShow == DefaultBoolean.True);
		}
		bool IExtenderProvider.CanExtend(object target) {
			if ((target is Control) && !(target is IToolTipControlClient))
				return true;
			return false;
		}
		ControlInfo GetControlInfo(Control control) {
			return (ControlInfo)this.toolTips[control];
		}
		void SetControlInfo(Control control, ControlInfo info) {
			if(info == null && GetControlInfo(control) != null) {
				control.MouseEnter -= new EventHandler(OnControlMouseEnter);
				control.MouseLeave -= new EventHandler(OnControlMouseLeave);
				control.MouseDown -= new MouseEventHandler(OnControlMouseDown);
				control.Disposed -= new EventHandler(OnControlDisposed);
			}
			if(info != null && GetControlInfo(control) == null) {
				control.Disposed += new EventHandler(OnControlDisposed);
				control.MouseEnter += new EventHandler(OnControlMouseEnter);
				control.MouseLeave += new EventHandler(OnControlMouseLeave);
				control.MouseDown += new MouseEventHandler(OnControlMouseDown);
			}
			if(info == null) 
				this.toolTips.Remove(control);
			else
			this.toolTips[control] = info;
		}
		protected internal bool IsVistaStyleTooltip {
			get {
				return ToolTipControllerHelper.ShouldUseVistaStyleTooltip(ToolTipStyle);
			}
		}
		[DefaultValue(""), Localizable(true), Category("Tooltip")]
		public string GetToolTip(Control control) {
			ControlInfo info = GetControlInfo(control);
			return info == null ? string.Empty : ToolTipControlInfo.NormalizeSimpleText(info.Text);
		}
		internal bool ShouldSerializeSuperTip(Control control) {
			SuperToolTip res = GetSuperTip(control);
			if(res == null || res.IsEmpty) return false;
			return true;
		}
		[Localizable(true), Category("Tooltip"),
		Editor("DevExpress.XtraEditors.Design.ToolTipContainerUITypeEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor))
		]
		public SuperToolTip GetSuperTip(Control control) {
			ControlInfo info = GetControlInfo(control);
			return info == null ? null : info.SuperTip;
		}
		public void SetSuperTip(Control control, SuperToolTip value) {
			ControlInfo info = GetControlInfo(control);
			if(info == null) info = new ControlInfo(string.Empty);
			info.SuperTip = value;
			SetControlInfo(control, info.IsEmpty ? null : info);
		}
		[DefaultValue(""), Localizable(true), Category("Tooltip")]
		public string GetTitle(Control control) {
			ControlInfo info = GetControlInfo(control);
			return info == null ? string.Empty : ToolTipControlInfo.NormalizeSimpleText(info.Title);
		}
		[DefaultValue(ToolTipIconType.None), Localizable(true), Category("Tooltip")]
		public ToolTipIconType GetToolTipIconType(Control control) {
			ControlInfo info = GetControlInfo(control);
			return info == null ? ToolTipIconType.None : info.IconType;
		}
		public void SetToolTip(Control control, string value) {
			ControlInfo info = GetControlInfo(control);
			if(value == null) {
				value = string.Empty;
			}
			if(info == null) info = new ControlInfo(value);
			info.Text = value;
			SetControlInfo(control, info.IsEmpty ? null : info);
		}
		public void SetTitle(Control control, string value) {
			ControlInfo info = GetControlInfo(control);
			if(value == null) {
				value = string.Empty;
			}
			if(info == null) info = new ControlInfo(string.Empty, value, ToolTipIconType.None);
			info.Title = value;
			SetControlInfo(control, info.IsEmpty ? null : info);
		}
		public void SetToolTipIconType(Control control, ToolTipIconType value) {
			ControlInfo info = GetControlInfo(control);
			if(info == null) info = new ControlInfo(string.Empty, value);
			info.IconType = value;
			SetControlInfo(control, info.IsEmpty ? null : info);
		}
		[DefaultValue(DefaultBoolean.Default), Localizable(true), Category("Tooltip")]
		public DefaultBoolean GetAllowHtmlText(Control control)
		{
			ControlInfo info = GetControlInfo(control);
			return info == null ? DefaultBoolean.Default : info.AllowHtmlText;
		}
		public void SetAllowHtmlText(Control control, DefaultBoolean value)
		{
			ControlInfo info = GetControlInfo(control);
			if (info == null) info = new ControlInfo(string.Empty, string.Empty, ToolTipIconType.None, value);
			info.AllowHtmlText = value;
			SetControlInfo(control, info.IsEmpty ? null : info);
		}
		internal bool ShouldSerializeAllowHtmlText(Control control)
		{
			DefaultBoolean res = GetAllowHtmlText(control);
			if (res == DefaultBoolean.Default) return false;
			return true;
		}
		protected Hashtable ControlClientOwners {
			get { return controlClientOwners; }
		}
		public void AddClientControl(Control control, IToolTipControlClient owner) {
			AddClientControlCore(control);
			if(ControlClientOwners.ContainsKey(control))
				ControlClientOwners[control] = owner;
			else 
				ControlClientOwners.Add(control, owner);
		}
		protected virtual void AddClientControlCore(Control control) {
			control.MouseEnter += new EventHandler(OnControlMouseEnter);
			control.MouseLeave += new EventHandler(OnControlMouseLeave);
			control.MouseMove += new MouseEventHandler(OnControlMouseMove);
			control.MouseWheel += new MouseEventHandler(OnControlMouseWheel);
			control.LocationChanged += new EventHandler(OnControlLocationChanged);
			control.MouseDown += new MouseEventHandler(OnControlMouseDown);
		}
		void OnControlLocationChanged(object sender, EventArgs e) {
			Control control = (Control)sender;
			if(control != ActiveControl || ActiveControl == null)
				return;
			Point pt = control.PointToClient(Control.MousePosition);
			MouseEventArgs m = new MouseEventArgs(MouseButtons.None, 0, pt.X, pt.Y, 0);
			OnControlMouseMove(sender, m);
		}
		public void AddClientControl(Control control) {
			if (!(control is IToolTipControlClient)) return;
			AddClientControlCore(control);
		}
		public virtual void RemoveClientControl(Control control) {
			control.MouseEnter -= new EventHandler(OnControlMouseEnter);
			control.MouseLeave -= new EventHandler(OnControlMouseLeave);
			control.MouseMove -= new MouseEventHandler(OnControlMouseMove);
			control.MouseWheel -= new MouseEventHandler(OnControlMouseWheel);
			control.LocationChanged -= new EventHandler(OnControlLocationChanged);
			control.MouseDown -= new MouseEventHandler(OnControlMouseDown);
			if(ControlClientOwners.ContainsKey(control))
				ControlClientOwners.Remove(control);
		}
		void UnsubscribeFormEvents() {
			Form frm = GetTargetForm();
			if(frm == null) return;
			frm.Deactivate -= new EventHandler(OnFormDeactivate);
		}
		Form GetTargetForm() {
			if(ActiveControl == null) return null;
			Form frm = ActiveControl.FindForm();
			if(frm == null) return null;
			if(frm.IsMdiChild && frm.MdiParent != null) frm = frm.MdiParent;
			return frm;
		}
		protected virtual void SubscribeFormEvents() {
			Form frm = GetTargetForm();
			if(frm == null) return;
			frm.Deactivate -= new EventHandler(OnFormDeactivate);
			frm.Deactivate += new EventHandler(OnFormDeactivate);
		}
		protected virtual void OnFormDeactivate(object sender, EventArgs e) {
			HideHint();	
		}
		protected void OnControlMouseEnter(object sender, EventArgs e) {
			if(sender is Control) {
				ActiveControl = sender as Control;
			}
		}
		protected bool IsMouseInTooltipForm() {
			ToolTipControllerBaseWindow tooltip = toolWindow;
			if(tooltip == null || !tooltip.Visible)
				return false;
			return tooltip.Bounds.Contains(Control.MousePosition);
		}
		protected void OnControlMouseLeave(object sender, EventArgs e) {
			if(toolWindow != null && ToolWindow.Visible && ActiveControl != null) {
				if(ToolWindow.HasHyperlink && CloseOnClick != DefaultBoolean.True) {
					ClosingDelayTimer.Start(() => {
						if(!IsMouseInTooltipForm()) ResetActiveControl();
					});
					return;
				}
				if(CloseOnClick == DefaultBoolean.True && IsMouseInActiveControl())
					return;
			}
			ResetActiveControl();
		}
		protected virtual bool IsHyperLinkAccessible { get { return toolWindow != null && toolWindow.Visible && toolWindow.HasHyperlink; } }
		protected virtual void ResetActiveControl() {
			if(toolWindow != null) {
				toolWindow.Dispose();
				toolWindow = null;
			}
			ActiveControl = null;
			if(ViewInfo.ShowArgs != null) {
				ViewInfo.ShowArgs.SelectedControl = null;
				ViewInfo.ShowArgs.SelectedObject = null;
			}
			if(SuperTooltip != null) {
				SuperTooltip.LookAndFeel = null;
			}
		}
		protected Form FindTopLevelForm(Control control) {
			Control current = control;
			while(current.Parent != null)
				current = current.Parent;
			Form frm = current as Form;
			return frm != null && frm.TopLevel ? frm : null;
		}
		protected void OnControlMouseWheel(object sender, MouseEventArgs e) {
			OnControlMouseMove(sender, e);
		}
		protected void OnControlMouseMove(object sender, MouseEventArgs e) {
			ProcessMouseMove(sender, e.Location);
		}
		public virtual void ProccessNCMouseMove(object sender, Point p){
			ProcessMouseMove(sender, p);
		}
		private void ProcessMouseMove(object sender, Point p) {
			Control active = sender as Control;
			if(active != null) {
				Form frm = FindTopLevelForm(active);
				Form activeForm = Form.ActiveForm;
				ISupportToolTipsForm activeSupportToolTips = activeForm as ISupportToolTipsForm;
				ISupportToolTipsForm inactiveSupportToolTips = frm as ISupportToolTipsForm;
				bool showToolTips = false;
				if(frm == activeForm)
					showToolTips = true;
				if(!showToolTips && inactiveSupportToolTips != null)
					showToolTips = inactiveSupportToolTips.ShowToolTipsWhenInactive;
				if(!showToolTips && activeSupportToolTips != null)
					showToolTips = activeSupportToolTips.ShowToolTipsFor(frm);
				if(!showToolTips)
					return;
				ActiveControl = active;
			}
			ToolTipControlInfo info = ActiveObjectInfo;
			IToolTipControlClient client = sender as IToolTipControlClient;
			if(ControlClientOwners.ContainsKey(sender)) {
				client = ControlClientOwners[sender] as IToolTipControlClient;
			}
			if(client != null && client.ShowToolTips) {
				info = client.GetObjectInfo(new Point(p.X, p.Y));
				if(info != null) info.Normalize();
			}
			ToolTipControllerGetActiveObjectInfoEventArgs args = new ToolTipControllerGetActiveObjectInfoEventArgs(ActiveControl, ActiveObject, info, new Point(p.X, p.Y));
			OnGetActiveObjectInfo(args);
			if(info != null) info.ForcedShow = GetForcedShow(info);
			bool isTheSameActiveObjectInfo = IsTheSameObjectInfo(args.Info);
			SetActiveObjectInfo(args.Info);
			if(ShouldHideHintByMouseMove(args.Info, isTheSameActiveObjectInfo)) {
				HideHint();
			}
		}
		protected virtual DefaultBoolean GetForcedShow(ToolTipControlInfo value) {
			if(value == null) return DefaultBoolean.False;
			if(value.ForcedShow != DefaultBoolean.Default) return value.ForcedShow;
			if(ActiveObjectInfo == null) return DefaultBoolean.False;
			if(ActiveObjectInfo.SuperTip == null || value.SuperTip == null) return DefaultBoolean.False;
			if(ActiveObjectInfo.SuperTip.Items.Count != value.SuperTip.Items.Count) return DefaultBoolean.True;
			for(int i = 0; i < value.SuperTip.Items.Count; i++) {
				if(!(value.SuperTip.Items[i] is ToolTipItem) || !(ActiveObjectInfo.SuperTip.Items[i] is ToolTipItem)) continue;
				if(((ToolTipItem)value.SuperTip.Items[i]).Text == ((ToolTipItem)ActiveObjectInfo.SuperTip.Items[i]).Text) continue;
				return DefaultBoolean.True;
			}
			return DefaultBoolean.False;
		}
		protected virtual bool ShouldHideHintByMouseMove(ToolTipControlInfo info, bool isTheSameActiveObjectInfo) {
			return isTheSameActiveObjectInfo && info != null && info.HideHintOnMouseMove && IsHintVisible && Control.MousePosition != ShowHintCursorPosition;
		}
		void OnControlDisposed(object sender, EventArgs e) {
			SetControlInfo((Control)sender, null);
		}
		protected void OnControlMouseDown(object sender, MouseEventArgs e) {
			HideHint();
		}
		protected virtual void OnInitialTimerTick(object sender, EventArgs e) {
			ShowHint();
		}
		protected virtual void OnAutoPopTimerTick(object sender, EventArgs e) {
			if (ActiveObject != null)
				HideToolWindow();
			else HideHint();
		}
		protected virtual void OnToolWindowPaint(object sender, PaintEventArgs e) {
			using(GraphicsCache cache = new GraphicsCache(e)) {
				if(GetUseSuperTips(CurrentShowArgs))
					DrawSuperToolWindow(cache);
				else
					DrawToolWindow(cache);
			}
		}
		protected virtual void DrawSuperToolWindow(GraphicsCache cache) {
		}
		protected virtual void DrawToolWindow(GraphicsCache cache) {
			ToolTipViewInfo vi = ViewInfo as ToolTipViewInfo;
			if(vi == null || CurrentShowArgs == null) return;
			ToolWindow.DrawBackground(cache, vi.PaintAppearance);
			ToolTipControllerCustomDrawEventArgs e = new ToolTipControllerCustomDrawEventArgs(cache, CurrentShowArgs, vi.ContentBounds);
			OnCustomDraw(e);
			if(e.Handled) return;
			ToolTipControllerImage image = new ToolTipControllerImage(CurrentShowArgs);
			if(image.HasImage) {
				image.Draw(cache, vi.ImageBounds.X, vi.ImageBounds.Y);
			}
			if (vi.AllowHtmlText) {
				DrawHtmlString(cache, vi.HtmlTextInfo);
				DrawHtmlString(cache, vi.HtmlTitleInfo);
			}
			else {
				DrawString(cache, CurrentShowArgs.ToolTip, vi.PaintAppearance, vi.TextBounds);
				DrawString(cache, CurrentShowArgs.Title, vi.PaintAppearanceTitle, vi.TitleBounds);
			}
		}
		protected virtual void DrawHtmlString(GraphicsCache cache, StringInfo info) {
			StringPainter.Default.DrawString(cache, info);
		}
		protected virtual void DrawString(GraphicsCache cache, string text, AppearanceObject appearance, Rectangle bounds) {
			if(text == string.Empty) return;
			cache.DrawString(text, appearance.Font, appearance.GetForeBrush(cache), bounds, this.Style.TextOptions.GetStringFormat(TextOptions.DefaultOptionsMultiLine));
		}
		protected virtual string CurrentToolTipText {
			get {
				if(ActiveControl != null) {
					if(ActiveControlClient != null) {
						if(ActiveObjectInfo != null) return ActiveObjectInfo.Text;
					}
					return GetToolTip(ActiveControl);
				}
				return string.Empty;
			}
		}
		protected virtual SuperToolTip CurrentSuperTip {
			get {
				if(ActiveControl != null) {
					if(ActiveControlClient != null) {
						if(ActiveObjectInfo != null) return ActiveObjectInfo.SuperTip;
					}
					return GetSuperTip(ActiveControl);
				}
				if(ActiveObjectInfo != null) return ActiveObjectInfo.SuperTip;
				return null;
			}
		}
		protected virtual string CurrentToolTipTitle {
			get {
				if(ActiveControl != null) {
					if(ActiveControlClient != null) {
						if(ActiveObjectInfo != null) return ActiveObjectInfo.Title;
					}
					return GetTitle(ActiveControl);
				}
				return string.Empty;
			}
		}
		protected virtual ToolTipIconType CurrentToolTipIconType {
			get {
				if(ActiveControl != null) {
					if(ActiveControlClient != null) {
						if(ActiveObjectInfo != null) return ActiveObjectInfo.IconType;
					}
					return GetToolTipIconType(ActiveControl);
				}
				return ToolTipIconType.None;
			}
		}
		protected virtual DefaultBoolean CurrentAllowHtmlText {
			get {
				if (ActiveControl != null) {
					if (ActiveControlClient != null) {
						if (ActiveObjectInfo != null) return ActiveObjectInfo.AllowHtmlText;
					}
					return GetAllowHtmlText(ActiveControl);
				}
				return DefaultBoolean.Default;
			}
		}
		protected Control ActiveControl {
			get { return this.activeControl; }
			set {
				if(ActiveControl == value) return;
				this.activeObject = null;
				if(ActiveControl != null) ActiveControl.HandleDestroyed -= new EventHandler(OnActiveControl_HandleDestroyed);
				Control prevControl = ActiveControl;
				UnsubscribeFormEvents();
				this.activeControl = value;
				this.activeObjectInfo = null;
				if(ActiveControl != null) ActiveControl.HandleDestroyed += new EventHandler(OnActiveControl_HandleDestroyed);
				ActiveObjectChanged(prevControl, ActiveObject);
				SubscribeFormEvents();
			}
		}
		protected void OnActiveControl_HandleDestroyed(object sender, EventArgs e) {
			ActiveControl = null;
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("ToolTipControllerActiveObject"),
#endif
 Browsable(false)]
		public object ActiveObject {
			get {	return this.activeObject; }
		}
		protected void SetActiveObject(object value) {
			SetActiveObject(value, false);
		}
		protected void SetActiveObject(object value, bool force) {
			if(!force && ((ActiveObject == value) || (object.Equals(ActiveObject, value)))) return;
			object prevObject = ActiveObject;
			this.activeObject = value;
			ActiveObjectChanged(ActiveControl, prevObject);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IToolTipControlClient ActiveControlClient {
			get {
				if(ActiveControl != null) {
					if(ActiveControl is IToolTipControlClient)
						return ActiveControl as IToolTipControlClient;
					else if(ControlClientOwners.ContainsKey(ActiveControl))
						return ControlClientOwners[ActiveControl] as IToolTipControlClient;
				}
				return null;
			}
		}
		void ForceHideWithoutTimer() {
			HideHintCore();
			ClosingDelayTimer.Stop();
		}
		protected virtual void ActiveObjectChanged(Control prevControl, object prevObject) {
			if(! Active) return;
			if(ActiveControl != null || ActiveObject != null) {
				if(ActiveControlClient != null || ActiveObject != null)  { 
					HideHint();
					if(ActiveObject != null) {
						if(ActiveObjectInfo != null && ActiveObjectInfo.ImmediateToolTip) {
							ForceHideWithoutTimer();
							ShowHint();
						}
						else {
							InitialTimer.Interval = prevObject != null ? ReshowDelay : InitialDelay;
							if(ActiveObjectInfo != null && ActiveObjectInfo.Interval > 0) InitialTimer.Interval = ActiveObjectInfo.Interval;
							ForceHideWithoutTimer();
							InitialTimer.Start();
						}
					} else {
						HideHint();
					}
				} else {
					InitialTimer.Interval = prevControl != null ? ReshowDelay : InitialDelay;
					InitialTimer.Start();
				}
			} else 	HideHint();
		}
		protected virtual Timer InitialTimer { get { return initialTimer; } }
		protected virtual Timer AutoPopTimer { get { return autoPopTimer; } }
		protected virtual ClosingDelayTimer ClosingDelayTimer { get { return closingDelayTimer; } }
		protected internal virtual ToolTipControllerBaseWindow ToolWindow { 
			get { 
				if(toolWindow == null || toolWindow.IsDisposed) {
					toolWindow = CreateToolWindow();
					toolWindow.Paint += new PaintEventHandler(OnToolWindowPaint);
					toolWindow.MouseLeave += new EventHandler(OnToolWindowMouseLeave);
				}
				return toolWindow; 
			} 
		}
		protected virtual bool IsMouseInActiveControl() {
			Point pt = ActiveControl.PointToClient(Control.MousePosition);
			return pt.X > 0 && pt.Y > 0 && pt.X < ActiveControl.Width && pt.Y < ActiveControl.Height;
		}
		protected virtual void OnToolWindowMouseLeave(object sender, EventArgs e) {
			if(CloseOnClick == DefaultBoolean.True && ActiveControl != null && !IsMouseInActiveControl()) {
				HideHint();
				ActiveControl = null;
			}
		}
		protected virtual void UpdateSuperTip(ToolTipControllerShowEventArgs eShow) {
			if(eShow.SuperTip == null || eShow.SuperTip.IsEmpty) {
				SuperToolTipSetupArgs args = new SuperToolTipSetupArgs();
				args.OwnerAllowHtmlText = AllowHtmlText;
				args.Title.Text = eShow.Title;
				args.Contents.Text = eShow.ToolTip;
				args.Contents.Images = eShow.ImageList;
				args.Contents.ImageIndex = eShow.ImageIndex;
				ToolTipControllerImage image = new ToolTipControllerImage(eShow);
				if(eShow.ToolTipImage != null) args.Contents.Image = eShow.ToolTipImage;	  
				args.Contents.Icon = image.GetIcon();
				RestoreSuperTipMaxWidth();
				SuperTooltip.Setup(args);
				SuperTooltip.Appearance.Assign(Appearance);
			}
			else {
				SaveSuperTipMaxWidth();
				SuperTooltip.Assign(eShow.SuperTip);
				AppearanceHelper.Combine(SuperTooltip.Appearance, new AppearanceObject[] { Appearance });
			}
			SuperTooltip.SetController(this);
			SuperTooltip.RightToLeft = eShow.RightToLeft;
			SuperTooltip.LookAndFeel = ViewInfo.GetLookAndFeel(eShow);
			SuperTooltip.AdjustSize();
		}
		bool isSuperTipMaxWidthSaved = false;
		protected void SaveSuperTipMaxWidth() {
			if(isSuperTipMaxWidthSaved)
				return;
			this.isSuperTipMaxWidthSaved = true;
			this.prevMaxWidth = SuperTooltip.MaxWidth;
		}
		protected void RestoreSuperTipMaxWidth() {
			if(!this.isSuperTipMaxWidthSaved)
				return;
			this.isSuperTipMaxWidthSaved = false;
			SuperTooltip.FixedTooltipWidth = false;
			SuperTooltip.MaxWidth = this.prevMaxWidth;
		}
		protected Point ShowHintCursorPosition { get; set; }
		protected virtual void ShowSuperTipCore() {
			if(SuperTooltip.Items.Count == 0) return;
			ToolWindow.Bounds = ViewInfo.Bounds;
			if(ToolWindow.Visible) {
				ToolWindow.Invalidate();
				ToolWindow.Update();
			}
			ToolWindow.Visible = true;
			ShowHintCursorPosition = Control.MousePosition;
		}
		protected virtual void ShowRegularTipCore(ToolTipLocation oldLocation, bool needUpdateRegion) {
			ToolTipViewInfo vi = ViewInfo as ToolTipViewInfo;
			if(vi == null) return;
			ToolWindow.Font = vi.PaintAppearance.Font;
			ToolWindow.BackColor = vi.PaintAppearance.BackColor;
			ToolWindow.ForeColor = vi.PaintAppearance.ForeColor;
			ToolWindow.Location = ViewInfo.Bounds.Location;
			if(ToolWindow.Size != ViewInfo.Bounds.Size || needUpdateRegion || (oldLocation != CurrentShowArgs.ToolTipLocation)) {
				ToolWindow.Size = ViewInfo.Bounds.Size;
				SetToolWindowRegion(CurrentShowArgs);
			}
			if(ToolWindow.Visible)
				ToolWindow.Invalidate();
			else {
				ToolWindow.Visible = true;
				ShowHintCursorPosition = Control.MousePosition;
			}
		}
		protected virtual void ShowHintCore(ToolTipControllerShowEventArgs eShow) {
			ResetAutoPopupDelay();
			InitialTimer.Stop();
			ClosingDelayTimer.Stop();
			if(DesignMode) return;
			if(eShow.Show) {
				CurrentToolTipType = GetToolTipType(eShow);
				if(activeController != null && activeController != this) {
					activeController.HideHint();
				}
				activeController = this;
				bool needUpdateRegion = CurrentShowArgs == null ? true : (CurrentShowArgs.ShowBeak != eShow.ShowBeak) || (CurrentShowArgs.Rounded != eShow.Rounded);
				ToolTipLocation oldLocation = CurrentShowArgs != null? CurrentShowArgs.GetToolTipLocation() : ToolTipControllerShowEventArgs.DefaultToolTipLocation;
				this.currentShowArgs = eShow;
				ToolWindow.CloseOnClick = CloseOnClick;
				ToolWindow.Controller = this;
				if(GetUseSuperTips(eShow))
					ShowSuperTipCore();
				else
					ShowRegularTipCore(oldLocation, needUpdateRegion);
				if(eShow.AutoHide) {
					AutoPopTimer.Interval = AutoPopDelay;
					AutoPopTimer.Start();
				}
			} else 	HideHint();
		}
		protected virtual bool IsShowArgValid(ToolTipControllerShowEventArgs eShow) {
			ToolTipType tooltipType = GetToolTipType(eShow);
			if(tooltipType != ToolTipType.Standard)
				return true;
			if(!IsHyperLinkAccessible)
				return true;
			return eShow.ToolTipImage != null || !string.IsNullOrEmpty(eShow.ToolTip);
		}
		public void ShowHint(ToolTipControllerShowEventArgs eShow, Point cursorPosition) {
			if(!IsShowArgValid(eShow)) {
				InitialTimer.Stop();
				return;
			}
			Point oldCursorPosition = this.oldCursorPosition;
			OnBeforeShow(eShow);
			ToolTipControllerCalcSizeEventArgs eCalcSize = new ToolTipControllerCalcSizeEventArgs(ActiveControl, ActiveObject, eShow);
			CurrentToolTipType = GetToolTipType(eShow);
			if(eShow.Show) {
				if(GetUseSuperTips(eShow)) {
					ToolTipViewInfoSuperTip vi = ((ToolTipViewInfoSuperTip)ViewInfo);
					UpdateSuperTip(eShow);
					vi.Calculate(eShow, Point.Empty);
					eCalcSize.Size = vi.SuperTip.ContentBounds.Size;
					Size oldSize = eCalcSize.Size;
					OnCalcSize(eCalcSize);
					bool useCustomSize = (eCalcSize.ShowInfo != null && oldSize != eCalcSize.Size);
					if(useCustomSize) {
						eCalcSize.ShowInfo.CustomSize = eCalcSize.Size;
						UpdateSuperTipSize(eShow.SuperTip, eCalcSize.Size);
					}
					eCalcSize.ShowInfo.UseCustomSize = useCustomSize;
					UpdateSuperTip(eShow);
				}
				bool? applyCursorOffset = null;
				if (ActiveObjectInfo != null)
					applyCursorOffset = ActiveObjectInfo.ApplyCursorOffset;
				ViewInfo.Calculate(eShow, eCalcSize.Size, cursorPosition, AllowHtmlText, applyCursorOffset);
				if(!GetUseSuperTips(eShow)) {
					ToolTipViewInfo vi = ViewInfo as ToolTipViewInfo;
					eCalcSize.Size = vi.ContentBounds.Size;
					eCalcSize.Position = vi.ContentScreenLocation;
					this.oldCursorPosition = cursorPosition;
					OnCalcSize(eCalcSize);
					ViewInfo.Calculate(eShow, eCalcSize.Size, cursorPosition, AllowHtmlText, applyCursorOffset);
				}
			}
			if(! eShow.Show || (CurrentShowArgs == null) || ! CurrentShowArgs.Equals(eShow)
				|| (! oldCursorPosition.Equals(cursorPosition)))
				ShowHintCore(eShow);
		}
		protected virtual void UpdateSuperTipSize(SuperToolTip superTip, Size size) {
			if(superTip == null || superTip.IsEmpty || size.IsEmpty)
				return;
			if(superTip.MaxWidth != size.Width) superTip.MaxWidth = size.Width;
		}
		public void ShowHint(ToolTipControllerShowEventArgs eShow) {
			ShowHint(eShow, Cursor.Position);
		}
		public void ShowHint(string toolTip, string title, ToolTipLocation toolTipLocation, Point cursorPosition) {
			ToolTipControllerShowEventArgs e = CloneCurrentShowArgs();
			e.ToolTip = toolTip;
			e.Title = title;
			e.ToolTipLocation = toolTipLocation;
			e.Rounded = Rounded;
			e.ToolTipStyle = ToolTipStyle;
			e.ShowBeak = ShowBeak;
			e.RoundRadius = RoundRadius;
			ShowHint(e, cursorPosition);
		}
		string GetCurrentTitle() { return CurrentShowArgs != null ? CurrentShowArgs.Title : string.Empty; }
		ToolTipLocation GetCurrentToolTipLocation() { return CurrentShowArgs != null ? CurrentShowArgs.GetToolTipLocation() : this.ToolTipLocation; }
		Point GetCurrentCursorPosition() { return CurrentShowArgs != null ? this.oldCursorPosition : Cursor.Position; }
		public void ShowHint(string toolTip, ToolTipLocation toolTipLocation, Point cursorPosition) {
			ShowHint(toolTip, GetCurrentTitle(), toolTipLocation, cursorPosition);
		}
		public void ShowHint(string toolTip, Point cursorPosition) {
			ShowHint(toolTip, GetCurrentToolTipLocation(), cursorPosition);
		}
		public void ShowHint(string toolTip, string title, Point cursorPosition) {
			ShowHint(toolTip, title, GetCurrentToolTipLocation(), cursorPosition);
		}
		public void ShowHint(string toolTip, ToolTipLocation toolTipLocation) {
			ShowHint(toolTip, toolTipLocation, GetCurrentCursorPosition());
		}
		public void ShowHint(string toolTip, string title, ToolTipLocation toolTipLocation) {
			ShowHint(toolTip, title, toolTipLocation, GetCurrentCursorPosition());
		}
		public void ShowHint(string toolTip) {
			ShowHint(toolTip, GetCurrentCursorPosition());			
		}
		public void ShowHint(string toolTip, string title) {
			ShowHint(toolTip, title, GetCurrentCursorPosition());			
		}
		public void ShowHint(string toolTip, string title, Control control, ToolTipLocation toolTipLocation) {
			ToolTipControllerShowEventArgs e = CloneCurrentShowArgs();
			e.ToolTip = toolTip;
			e.Title = title;
			e.ToolTipLocation = toolTipLocation;
			e.Rounded = Rounded;
			e.ToolTipStyle = ToolTipStyle;
			e.ShowBeak = ShowBeak;
			e.RoundRadius = RoundRadius;
			ShowHint(e, control);
		}
		public void ShowHint(string toolTip, Control control, ToolTipLocation toolTipLocation) {
			ShowHint(toolTip, GetCurrentTitle(), control, toolTipLocation);
		}
		public void ShowHint(ToolTipControllerShowEventArgs eShow, Control control) {
			ShowHint(eShow, ViewInfo.GetCursorPosition(eShow, control));
		}
		public void ShowHint(ToolTipControlInfo info) {
			SetActiveObjectInfo(info);
		}
		protected internal bool CanShowToolTip(ToolTipControllerShowEventArgs eShow) {
			bool isExistsToolTip = !string.IsNullOrEmpty(eShow.Title) || !string.IsNullOrEmpty(eShow.ToolTip);
			if(ToolTipType == ToolTipType.Standard || eShow.ToolTipType == ToolTipType.Standard) return isExistsToolTip;
			bool isExistsSuperTip = (eShow.SuperTip != null && !eShow.SuperTip.IsEmpty) || (GetUseSuperTips(eShow) && isExistsToolTip);
			if(ToolTipType == ToolTipType.SuperTip || eShow.ToolTipType == ToolTipType.SuperTip) return isExistsSuperTip;
			return isExistsSuperTip || isExistsToolTip;
		}
		protected void ShowHint() {
			Point position = Cursor.Position;
			ToolTipControllerShowEventArgs eShow = CreateShowArgs();
			eShow.RightToLeft = ActiveControl != null ? WindowsFormsSettings.GetIsRightToLeft(ActiveControl) : false;
			eShow.ObjectBounds = ActiveObjectInfo != null? ActiveObjectInfo.ObjectBounds: Rectangle.Empty;
			eShow.ToolTip = CurrentToolTipText;
			eShow.Title = CurrentToolTipTitle;
			eShow.IconType = CurrentToolTipIconType;
			eShow.SuperTip = CurrentSuperTip;
			if(eShow.SuperTip != null) {
				eShow.SuperTip.OwnerAllowHtmlText = AllowHtmlText;
			}
			eShow.AllowHtmlText = CurrentAllowHtmlText;
			eShow.ToolTipType = ActiveObjectInfo != null ? ActiveObjectInfo.ToolTipType : ToolTipType.Default;
			if(ActiveObjectInfo != null) {
				eShow.ToolTipImage = activeObjectInfo.ToolTipImage;
				if(ActiveObjectInfo.ToolTipLocation != ToolTipLocation.Default) {
					eShow.ToolTipLocation = activeObjectInfo.ToolTipLocation;
				}
				if(ActiveObjectInfo.ToolTipPosition.X > -10000) {
					position = ActiveObjectInfo.ToolTipPosition;
				}
			}
			if(ActiveObjectInfo != null) {
				if(eShow.ToolTip == string.Empty) eShow.ToolTip = ActiveObjectInfo.Text;
				if(eShow.Title == string.Empty) eShow.Title = ActiveObjectInfo.Title;
				eShow.Show = CanShowToolTip(eShow);
				eShow.SelectedControl = ActiveControl;
				eShow.SelectedObject = ActiveObjectInfo.Object;
				eShow.IconType = ActiveObjectInfo.IconType;
			} else {
				eShow.SelectedControl = ActiveControl;
				eShow.SelectedObject = ActiveObject;
			}
			ShowHint(eShow, position);
		}
		public void HideHint() {
			if(IsHyperLinkAccessible) {
				ClosingDelayTimer.Start(() => {
					if(!IsMouseInTooltipForm()) HideHintCore();
				});
			}
			else {
				HideHintCore();
			}
		}
		public void HideHintCore() {
			this.currentShowArgs = null;
			this.oldCursorPosition = Point.Empty;
			HideToolWindow();
			AutoPopTimer.Stop();
			InitialTimer.Stop();
			ClosingDelayTimer.Stop();
		}
		bool IsHintVisible {
			get {
				return this.toolWindow != null && ToolWindow.Visible;
			}
		}
		protected virtual void HideToolWindow() {
			if (this.toolWindow != null && !this.toolWindow.IsDisposed) {
				bool prevVisible = ToolWindow.Visible;
				ToolWindow.Visible = false;
			}
		}
		private static readonly object beforeShow = new object();
		private static readonly object calcSize = new object();
		private static readonly object customDraw = new object();
		private static readonly object getActiveObjectInfo = new object();
		private static readonly object hyperlinkClick = new object();
		[
#if !SL
	DevExpressUtilsLocalizedDescription("ToolTipControllerGetActiveObjectInfo"),
#endif
 Category("Events")]
		public event ToolTipControllerGetActiveObjectInfoEventHandler GetActiveObjectInfo {
			add { this.Events.AddHandler(getActiveObjectInfo, value); }
			remove { this.Events.RemoveHandler(getActiveObjectInfo, value); }
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("ToolTipControllerBeforeShow")]
#endif
		public event ToolTipControllerBeforeShowEventHandler BeforeShow {
			add { Events.AddHandler(beforeShow, value); }
			remove { Events.RemoveHandler(beforeShow, value); }
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("ToolTipControllerCalcSize")]
#endif
		public event ToolTipControllerCalcSizeEventHandler CalcSize {
			add { Events.AddHandler(calcSize, value); }
			remove { Events.RemoveHandler(calcSize, value); }
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("ToolTipControllerCustomDraw")]
#endif
		public event ToolTipControllerCustomDrawEventHandler CustomDraw {
			add { Events.AddHandler(customDraw, value); }
			remove { Events.RemoveHandler(customDraw, value); }
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("ToolTipControllerHyperlinkClick")]
#endif
		public event HyperlinkClickEventHandler HyperlinkClick {
			add { Events.AddHandler(hyperlinkClick, value); }
			remove { Events.RemoveHandler(hyperlinkClick, value); }
		}
		protected virtual void OnBeforeShow(ToolTipControllerShowEventArgs e) {
			ToolTipControllerBeforeShowEventHandler handler = (ToolTipControllerBeforeShowEventHandler)this.Events[beforeShow];
			if(handler != null) handler(this, e);
			if(ActiveControlClient is IToolTipControlClientEx) {
				((IToolTipControlClientEx)ActiveControlClient).OnBeforeShow(e);
			}
		}
		protected virtual void OnCalcSize(ToolTipControllerCalcSizeEventArgs e) {
			ToolTipControllerCalcSizeEventHandler handler = (ToolTipControllerCalcSizeEventHandler)this.Events[calcSize];
			if(handler != null) handler(this, e);
		}
		protected virtual void OnCustomDraw(ToolTipControllerCustomDrawEventArgs e) {
			ToolTipControllerCustomDrawEventHandler handler = (ToolTipControllerCustomDrawEventHandler)this.Events[customDraw];
			if(handler != null) handler(this, e);
		}
		protected virtual void OnGetActiveObjectInfo(ToolTipControllerGetActiveObjectInfoEventArgs e) {
			ToolTipControllerGetActiveObjectInfoEventHandler handler = (ToolTipControllerGetActiveObjectInfoEventHandler)this.Events[getActiveObjectInfo];
			if(handler != null) handler(this, e);
		}
		protected virtual void OnHyperlinkClick(HyperlinkClickEventArgs e) {
			HyperlinkClickEventHandler handler = (HyperlinkClickEventHandler)this.Events[hyperlinkClick];
			if(handler != null) handler(this, e);
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("ToolTipControllerToolTipType"),
#endif
 DefaultValue(ToolTipType.Default)]
		public ToolTipType ToolTipType {
			get { return toolTipType; }
			set {
				if(ToolTipType == value) return;
				toolTipType = value;
				HideHint();
				DestroyToolWindow();
				CurrentToolTipType = GetToolTipType();
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("ToolTipControllerActive"),
#endif
 DefaultValue(true)]
		public bool Active {
			get { return this.active; }
			set {
				if(value == Active) return;
				active = value;
				if(!value)
					HideHint();
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("ToolTipControllerInitialDelay"),
#endif
 DefaultValue(DefaultInitialDelay)]
		public int InitialDelay {
			get { return initialDelay; }
			set {
				if(value < 1) value = 1;
				if(InitialDelay == value) return;
				initialDelay = value;
				InitialTimer.Interval = InitialDelay;
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("ToolTipControllerReshowDelay"),
#endif
 DefaultValue(DefaultReshowDelay)]
		public int ReshowDelay {
			get { return reshowDelay; }
			set {
				if(value < 1) value = 1;
				if(ReshowDelay == value) return;
				reshowDelay = value;
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("ToolTipControllerAutoPopDelay"),
#endif
 DefaultValue(DefaultAutoPopDelay)]
		public int AutoPopDelay {
			get { return autoPopDelay; }
			set {
				if(value < 1) value = 1;
				if(AutoPopDelay == value) return;
				autoPopDelay = value;
				AutoPopTimer.Interval = AutoPopDelay;
			}
		}
		public void ResetAutoPopupDelay() {
			if(AutoPopTimer.Enabled) {
				AutoPopTimer.Enabled = false;
				AutoPopTimer.Enabled = true;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public bool UseWindowStyle {
			get { return true; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public AppearanceObject Style { 
			get { return Appearance; } 
			set { 
				if(value == null) return;
				this.Appearance.Assign(value);
			}
		}
		void ResetAppearance() { Appearance.Reset(); }
		bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("ToolTipControllerAppearance"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Appearance { get { return showArgs.Appearance; } }
		void ResetAppearanceTitle() { AppearanceTitle.Reset(); }
		bool ShouldSerializeAppearanceTitle() { return AppearanceTitle.ShouldSerialize(); }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("ToolTipControllerAppearanceTitle"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject AppearanceTitle { get { return showArgs.AppearanceTitle; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsDefaultToolTipLocation { get { return this.showArgs.ToolTipLocation == Utils.ToolTipLocation.Default; } }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("ToolTipControllerToolTipLocation"),
#endif
 DefaultValue(ToolTipControllerShowEventArgs.DefaultToolTipLocation)]
		public ToolTipLocation ToolTipLocation { get { return this.showArgs.GetToolTipLocation(); } set { this.showArgs.ToolTipLocation = value; } }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("ToolTipControllerShowBeak"),
#endif
 DefaultValue(false)]
		public bool ShowBeak { get { return this.showArgs.ShowBeak; } set { this.showArgs.ShowBeak = value; } }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("ToolTipControllerRounded"),
#endif
 DefaultValue(false)]
		public bool Rounded { get { return this.showArgs.Rounded; } set { this.showArgs.Rounded = value; } }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("ToolTipControllerToolTipStyle"),
#endif
 DefaultValue(ToolTipStyle.Default)]
		public ToolTipStyle ToolTipStyle { get { return this.showArgs.ToolTipStyle; } set { this.showArgs.ToolTipStyle = value; } }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("ToolTipControllerRoundRadius"),
#endif
 DefaultValue(ToolTipControllerShowEventArgs.DefaultRoundRadius)]
		public int RoundRadius { get { return this.showArgs.RoundRadius; } set { this.showArgs.RoundRadius = value; } }
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete(), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false),
#if !SL
	DevExpressUtilsLocalizedDescription("ToolTipControllerIconType"),
#endif
 DefaultValue(ToolTipIconType.None)]
		public ToolTipIconType IconType { get { return this.showArgs.IconType; } set { } } 
		[
#if !SL
	DevExpressUtilsLocalizedDescription("ToolTipControllerIconSize"),
#endif
 DefaultValue(ToolTipIconSize.Small)]
		public ToolTipIconSize IconSize { get { return this.showArgs.IconSize; } set { this.showArgs.IconSize = value; } }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("ToolTipControllerImageList"),
#endif
 DefaultValue(null),
		 TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))
		]
		public virtual object ImageList { get { return this.showArgs.ImageList; } set { this.showArgs.ImageList = value; } }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("ToolTipControllerImageIndex"),
#endif
 DefaultValue(-1),
		 Editor(typeof(DevExpress.Utils.Design.ImageIndexesEditor), typeof(System.Drawing.Design.UITypeEditor)),
		ImageList("ImageList")
		]
		public int ImageIndex { get { return this.showArgs.ImageIndex; } set { this.showArgs.ImageIndex = value; } }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("ToolTipControllerShowShadow"),
#endif
 DefaultValue(true)]
		public bool ShowShadow {
			get { return this.showShadow; }
			set {
				if(this.showShadow != value) {
					this.showShadow = value;
					DestroyToolWindow();
				}
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("ToolTipControllerAllowHtmlText"),
#endif
 DefaultValue(false)]
		public bool AllowHtmlText {
			get { return allowHtmlText; }
			set {
				if(AllowHtmlText != value) {
					allowHtmlText = value;
					DestroyToolWindow();
				}
			}
		}
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int SuperTipMaxWidth {
			set {
				if(SuperTooltip != null && value > 0)
					SuperTooltip.MaxWidth = value;
			}
		}
		protected ToolTipControllerShowEventArgs CurrentShowArgs { get { return this.currentShowArgs; } }
		protected ToolTipControllerShowEventArgs CloneCurrentShowArgs() {
			if(CurrentShowArgs != null)
				return (ToolTipControllerShowEventArgs)((ICloneable)CurrentShowArgs).Clone();
			else return CreateShowArgs();
		}
		public ToolTipControllerShowEventArgs CreateShowArgs() {
			return (ToolTipControllerShowEventArgs)((ICloneable)this.showArgs).Clone();
		}
		protected virtual void SetToolWindowRegion(ToolTipControllerShowEventArgs eShow) {
			ToolTipViewInfo vi = ViewInfo as ToolTipViewInfo;
			if(vi == null) return;
			if(eShow.Rounded || eShow.ShowBeak) {
				ToolTipGraphicsPath tpath = new ToolTipGraphicsPath(vi.ContentBounds, eShow.GetToolTipLocation(), 
					eShow.Rounded ? eShow.RoundRadius : 0, 
					eShow.ShowBeak ? vi.OutOffHeight : 0, eShow.ShowBeak ? vi.OutOffWidth : 0);
				ToolWindow.Path = tpath.Path;
				ToolWindow.Region = BitmapToRegion.ConvertPathToRegion(tpath.Path);
			}
			else {
				if(ToolWindow.Region != null) ToolWindow.Region = null;
				ToolWindow.Path = null;
			}
		}
		DefaultBoolean closeOnClick = DefaultBoolean.Default;
		[
#if !SL
	DevExpressUtilsLocalizedDescription("ToolTipControllerCloseOnClick"),
#endif
DefaultValue(DefaultBoolean.Default)]
		public virtual DefaultBoolean CloseOnClick {
			get { return closeOnClick; }
			set { closeOnClick = value; }
		}
		#region ToolTipControllerHelper
		internal class ToolTipControllerHelper {
			public static bool ShouldUseVistaStyleTooltip(ToolTipStyle style) {
				if(style == ToolTipStyle.WindowsXP || !NativeVista.IsVista)
					return false;
				return !IsXPVisualStyle;
			}
			internal static bool IsXPVisualStyle {
				get {
					StringBuilder name = new StringBuilder(NativeMethods.MAX_PATH, NativeMethods.MAX_PATH);
					return NativeVista.GetCurrentThemeName(name, NativeMethods.MAX_PATH, IntPtr.Zero, 0, IntPtr.Zero, 0) < 0;
				}
			}
		}
		#endregion
		internal void OnHyperlinkClick(StringBlock block) {
			OnHyperlinkClick(new HyperlinkClickEventArgs() { Text = block.Text, Link = block.Link });
		}
	}
	[ToolboxItem(false)]
	public class ClosingDelayTimer : Timer {
		MethodInvoker callback;
		public ClosingDelayTimer() {
			this.callback = null;
		}
		public void Start(MethodInvoker callback) {
			this.callback = callback;
			Start();
		}
		protected override void OnTick(EventArgs e) {
			base.OnTick(e);
			if(!Enabled)
				return;
			Stop();
			if(Callback != null)
				Callback();
			this.callback = null;
		}
		protected override void Dispose(bool disposing) {
			this.callback = null;
			base.Dispose(disposing);
		}
		public MethodInvoker Callback { get { return callback; } }
	}
	public class HyperlinkClickEventArgs : EventArgs {
		public string Text { get; set; }
		public string Link { get; set; }
		public MouseEventArgs MouseArgs {get; set;}
	}
	[ToolboxItem(false)]
	public class ToolTipControllerDefault : ToolTipController {
		[Browsable(false)]
		public override object ImageList {
			get {
				return base.ImageList;
			}
			set {
				base.ImageList = value;
			}
		}
	}
}
namespace DevExpress.Utils.Win {
	[ToolboxItem(false)]
	public class CursorInfo {
		static Point DefaultHotSpot = new Point(10, 10);
		static public Rectangle CursorBounds {
			get {
				if(Cursor.Current == null) return Rectangle.Empty;
				Size size = Cursor.Current.Size;
				if(Cursor.Current.HotSpot.IsEmpty) return new Rectangle(0, 0, size.Width - DefaultHotSpot.X, size.Height - DefaultHotSpot.Y);
				return new Rectangle(0, 0, size.Width - Cursor.Current.HotSpot.X, size.Height - Cursor.Current.HotSpot.Y);
			}
		}
	}
	[ToolboxItem(false)]
	public abstract class ToolTipControllerBaseWindow : TopFormBase {
		GraphicsPath path;
		bool dropShadow = true;
		public ToolTipControllerBaseWindow(bool dropShadow) {
			this.dropShadow = dropShadow;
			SetStyle(ControlStyles.Opaque | ControlConstants.DoubleBuffer, true);
		}
		DefaultBoolean closeOnClick = DefaultBoolean.Default;
		protected internal ToolTipController Controller { get; set; }
		public DefaultBoolean CloseOnClick {
			get { return closeOnClick; }
			set { closeOnClick = value; }
		}
		protected internal virtual bool HasHyperlink {
			get {
				ToolTipViewInfo vi = Controller.ViewInfo as ToolTipViewInfo;
				return vi != null && (vi.HtmlTitleInfo.HasHyperlink || vi.HtmlTextInfo.HasHyperlink);
			}
		}
		protected bool IsMouseOnLink(StringInfo info, Point pt) {
			return info != null && info.GetLinkByPoint(pt) != null;
		}
		protected virtual StringBlock GetLinkByMouse(Point pt) { 
			ToolTipViewInfo vi = Controller.ViewInfo as ToolTipViewInfo;
			StringBlock block = vi.HtmlTitleInfo != null? vi.HtmlTitleInfo.GetLinkByPoint(pt): null;
			if(block != null)
				return block;
			return vi.HtmlTextInfo == null? null: vi.HtmlTextInfo.GetLinkByPoint(pt);
		}
		protected virtual bool IsMouseOnLink(Point pt) {
			return GetLinkByMouse(pt) != null;
		}
		protected virtual bool AllowHtmlText { 
			get { 
				return Controller.ViewInfo.AllowHtmlText; 
			} 
		}
		protected override void WndProc ( ref System.Windows.Forms.Message m ) {
			const int WM_NCHITTEST = 0x84, HTTRANSPARENT =(-1);
			base.WndProc(ref m);
			switch(m.Msg) {
				case WM_NCHITTEST :
					if( (CloseOnClick != DefaultBoolean.True && !HasHyperlink))
						m.Result = new IntPtr(HTTRANSPARENT);
					break;
			}
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			Cursor = IsMouseOnLink(e.Location) ? Cursors.Hand : Cursor = Cursors.Default;
		}
		protected virtual void TryProcessHyperlinkClick(MouseEventArgs e) {
			StringBlock block = GetLinkByMouse(e.Location);
			if(block != null) {
				Controller.OnHyperlinkClick(block);
			}
		}
		protected override void OnMouseClick(MouseEventArgs e) {
			base.OnMouseClick(e);
			if(AllowHtmlText) {
				TryProcessHyperlinkClick(e);
			}
			Hide();
		}
		protected override CreateParams CreateParams {
			get {
				CreateParams cp = base.CreateParams;
				cp.Style = unchecked((int)0x80000000); 
				cp.ClassStyle |= 0x0800; 
				if(dropShadow && (System.Environment.OSVersion.Version.Major > 5 || (System.Environment.OSVersion.Version.Major == 5 && System.Environment.OSVersion.Version.Minor > 0))) 
					cp.ClassStyle |= 0x20000; 
				return cp;
			}
		}
		public GraphicsPath Path {
			get { return path; }
			set {
				if(path == value) return;
				if(path != null) path.Dispose();
				path = value;
			}
		}
		public abstract void DrawBackground(GraphicsCache cache, AppearanceObject apperance);
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			UpdateRegion();
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			UpdateRegion();
		}
		protected internal virtual void UpdateRegion() { }
	}
	public abstract class ToolTipControllerStyledWindowBase : ToolTipControllerBaseWindow {
		bool dropShadow;
		public ToolTipControllerStyledWindowBase(bool dropShadow) : base(dropShadow) {
			this.dropShadow = dropShadow;
		}
		public static ToolTipControllerStyledWindowBase Create(ToolTipController controller) {
			if(controller.IsVistaStyleTooltip)
				return new ToolTipControllerVistaWindow(controller.ShowShadow);
			return new ToolTipControllerXPWindow(controller.ShowShadow);
		}
		protected bool DropShadow { get { return this.dropShadow; } }
		ObjectPainter painterCore = null;
		protected ObjectPainter Painter {
			get {
				if(this.painterCore == null)
					this.painterCore = CreatePainter();
				return this.painterCore;
			}
		}
		protected abstract ObjectPainter CreatePainter();
		public override void DrawBackground(GraphicsCache cache, AppearanceObject apperance) {
			apperance.FillRectangle(cache, ClientRectangle);
			if(Path == null)
				DrawSimpleBackground(cache, apperance);
			else DrawPathBackground(cache, apperance);
		}
		protected virtual void DrawPathBackgroundCore(GraphicsCache cache, AppearanceObject appearance) {
			SmoothingMode prev = cache.Graphics.SmoothingMode;
			PixelOffsetMode prevP = cache.Graphics.PixelOffsetMode;
			cache.Graphics.SmoothingMode = SmoothingMode.HighQuality;
			cache.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
			cache.Graphics.DrawPath(GetPathPen(cache, appearance), Path);
			cache.Graphics.SmoothingMode = prev;
			cache.Graphics.PixelOffsetMode = prevP;
		}
		protected virtual Pen GetPathPen(GraphicsCache cache, AppearanceObject appearance) {
			return appearance.GetBorderPen(cache);
		}
		protected abstract void DrawSimpleBackground(GraphicsCache cache, AppearanceObject appearance);
		protected abstract void DrawPathBackground(GraphicsCache cache, AppearanceObject appearance);
	}
	public class ToolTipControllerXPWindow : ToolTipControllerStyledWindowBase {
		public ToolTipControllerXPWindow(bool dropShadow) : base(dropShadow) { }
		protected override void DrawSimpleBackground(GraphicsCache cache, AppearanceObject appearance) {
			cache.Graphics.DrawRectangle(appearance.GetBorderPen(cache), BorderRectangle);
		}
		protected override void DrawPathBackground(GraphicsCache cache, AppearanceObject appearance) {
			DrawPathBackgroundCore(cache, appearance);
		}
		protected override ObjectPainter CreatePainter() {
			return null;
		}
		protected Rectangle BorderRectangle {
			get {
				Rectangle res = ClientRectangle;
				return new Rectangle(res.X, res.Y, res.Width - 1, res.Height - 1);
			}
		}
	}
	public class ToolTipControllerVistaWindow : ToolTipControllerStyledWindowBase {
		public ToolTipControllerVistaWindow(bool dropShadow) : base(dropShadow) { }
		protected override void DrawSimpleBackground(GraphicsCache cache, AppearanceObject appearance) {
			ApplyRegionCore();
			if(DropShadow) cache.Graphics.FillRectangle(Brushes.Gray, ShadowRect);
			XPToolTipInfoArgs args = new XPToolTipInfoArgs(true);
			args.Bounds = ClientRectangle;
			ObjectPainter.DrawObject(cache, Painter, args);
		}
		protected override void DrawPathBackground(GraphicsCache cache, AppearanceObject appearance) {
			XPToolTipInfoArgs args = new XPToolTipInfoArgs(false);
			args.Bounds = ClientRectangle;
			ObjectPainter.DrawObject(cache, Painter, args);
			DrawPathBackgroundCore(cache, appearance);
			DrawPathBackgroundCore(cache, appearance);
		}
		protected override ObjectPainter CreatePainter() {
			return new XPToolTipPainter();
		}
		protected Rectangle ShadowRect {
			get {
				Rectangle rect = ClientRectangle;
				return new Rectangle(rect.Right - 2, rect.Bottom - 2, 2, 2);
			}
		}
		protected virtual void ApplyRegionCore() {
			GraphicsPath path = CreateGraphicsPathCore();
			Region = new Region(path);
		}
		protected virtual GraphicsPath CreateGraphicsPathCore() {
			GraphicsPath path = new GraphicsPath();
			Rectangle rect = ClientRectangle;
			rect.Inflate(-1, -1);
			path.AddRectangle(rect);
			path.AddRectangle(new Rectangle(1, 0, Width - 2, 1));
			path.AddRectangle(new Rectangle(0, 1, 1, Height - 2));
			path.AddRectangle(new Rectangle(1, Height - 1, Width - 2, 1));
			path.AddRectangle(new Rectangle(Width - 1, 1, 1, Height - 2));
			return path;
		}
	}
	public class ToolTipControllerWindow : ToolTipControllerXPWindow {
		public ToolTipControllerWindow(bool dropShadow) : base(dropShadow) { }
	}
	public enum TransparencyMode {
		ColorKeyTransparent,
		ColorKeyOpaque
	}
	public class BitmapToRegion {
		public static Region ConvertPathToRegion(GraphicsPath path) {
			RectangleF fb = path.GetBounds();
			Rectangle bounds = Rectangle.FromLTRB(0, 0, 1 + (int)fb.Right, 1 + (int)fb.Bottom);
			Bitmap bmp = new Bitmap(bounds.Width, bounds.Height);
			Graphics g = Graphics.FromImage(bmp);
			g.Clear(Color.Magenta);
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
			g.FillPath(Brushes.Red, path);
			g.Dispose();
			Region res = BitmapToRegion.Convert(bmp, Color.Magenta);
			return res;
		}
		public static Region Convert(Bitmap bitmap, Color colorTransparent) {
			GraphicsPath path = new GraphicsPath();
			int argb = colorTransparent.ToArgb();
			int colOpaquePixel = 0, width = bitmap.Width, height = bitmap.Height;
			for(int row = 0; row < height; row ++) {
				colOpaquePixel = 0;
				for(int col = 0; col < width; col ++) {
					if(bitmap.GetPixel(col, row).ToArgb() != argb) {
						colOpaquePixel = col;
						int colNext = col;
						for(colNext = colOpaquePixel; colNext < width; colNext++)
							if(bitmap.GetPixel(colNext, row).ToArgb() == argb) break;
						path.AddRectangle(new Rectangle(colOpaquePixel, 
							row, colNext - colOpaquePixel, 1));
						col = colNext;
					}
				}
			}
			Region reg = new Region(path);
			path.Dispose();
			return reg;
		}
	}
	public class ToolTipGraphicsPath : IDisposable {
		public enum ToolTipLocationSide {Top, Bottom, Left, Right};
		public enum ToolTipLocationPosition {Near, Center, Far};
		Rectangle bounds;
		ToolTipLocation location;
		ToolTipLocationSide locationSide;
		ToolTipLocationPosition locationPosition;
		int roundRadius;
		int outOffSize; 
		int outOffWidth;
		GraphicsPath path;
		public ToolTipGraphicsPath(Rectangle bounds, ToolTipLocation location) 
			: this(bounds, location, 0) {}
		public ToolTipGraphicsPath(Rectangle bounds, ToolTipLocation location, int roundRadius) 
			: this(bounds, location, roundRadius, 0, 0) {}
		public ToolTipGraphicsPath(Rectangle bounds, ToolTipLocation location, int roundRadius, int outOffSize, int outOffWidth) {
			this.bounds = bounds;
			this.location = location;
			this.locationSide = GetToolTipLocationSide(Location);
			this.locationPosition = GetToolTipLocationPosition(Location);
			this.roundRadius = roundRadius;
			this.outOffSize = outOffSize;
			this.outOffWidth = outOffWidth;
			this.path = null;
		}
		public void Dispose() {
			if(path != null) {
				this.path.Dispose();
				this.path = null;
			}
		}
		public Rectangle Bounds { get { return bounds; } }
		public ToolTipLocation Location  { get { return location; } }
		public ToolTipLocationSide LocationSide { get { return locationSide; } }
		public ToolTipLocationPosition LocationPosition { get { return locationPosition; } }
		public int RoundRadius  { get { return roundRadius; } }
		public bool ShowBeaks { get { return OutOffSize > 0; } } 
		public int OutOffSize  { get { return outOffSize; } }
		public int OutOffWidth  { get { return outOffWidth; } }
		public GraphicsPath Path {
			get {
				if(path == null)
					path = CreatePath();
				return path;
			}
		}
		public static ToolTipLocationSide GetToolTipLocationSide(ToolTipLocation toolTipLocation) {
			if(toolTipLocation == ToolTipLocation.TopCenter || toolTipLocation == ToolTipLocation.TopLeft || toolTipLocation == ToolTipLocation.TopRight)
				return ToolTipLocationSide.Top;
			if(toolTipLocation == ToolTipLocation.BottomCenter || toolTipLocation == ToolTipLocation.BottomLeft || toolTipLocation == ToolTipLocation.BottomRight)
				return ToolTipLocationSide.Bottom;
			if(toolTipLocation == ToolTipLocation.LeftCenter || toolTipLocation == ToolTipLocation.LeftTop || toolTipLocation == ToolTipLocation.LeftBottom)
				return ToolTipLocationSide.Left;
			return ToolTipLocationSide.Right;
		}
		public static ToolTipLocationPosition GetToolTipLocationPosition(ToolTipLocation toolTipLocation) {
			if(toolTipLocation == ToolTipLocation.BottomLeft || toolTipLocation == ToolTipLocation.TopLeft 
				|| toolTipLocation == ToolTipLocation.LeftTop || toolTipLocation == ToolTipLocation.RightTop)
				return ToolTipLocationPosition.Far;
			if(toolTipLocation == ToolTipLocation.BottomCenter || toolTipLocation == ToolTipLocation.TopCenter 
				|| toolTipLocation == ToolTipLocation.LeftCenter || toolTipLocation == ToolTipLocation.RightCenter)
				return ToolTipLocationPosition.Center;
			return ToolTipLocationPosition.Near;
		}
		Point[] ToolTipBeakSetLocationPoints() {
			if(!ShowBeaks) return null;
			Point[] points = { Point.Empty, Point.Empty, Point.Empty };
			int lineX, lineWidth;
			int beakOffset = RoundRadius > ToolTipControllerShowEventArgs.DefaultRoundRadius ? RoundRadius : ToolTipControllerShowEventArgs.DefaultRoundRadius; 
			if(LocationSide == ToolTipLocationSide.Bottom || LocationSide == ToolTipLocationSide.Top) {
				lineX = Bounds.Left; lineWidth = Bounds.Width;
			} else {
				lineX = Bounds.Top; lineWidth = Bounds.Height;
			}
			int p1 = 0, p2 = 0, p3 = 0;
			if(outOffWidth > lineWidth - 2 * beakOffset)
				outOffWidth = lineWidth - 2 * beakOffset;
			switch(LocationPosition) {
				case ToolTipLocationPosition.Near:
					p1 = lineX + beakOffset; 
					p3 = p1 + outOffWidth; p2 = p1;
					break;
				case ToolTipLocationPosition.Center:
					if(outOffWidth % 2 == 1) outOffWidth --;
					p1 = lineX + (lineWidth - outOffWidth) / 2;
					p2 = p1 + outOffWidth / 2;
					p3 = p1 + outOffWidth;
					break;
				case ToolTipLocationPosition.Far:
					p3 = lineX + lineWidth - beakOffset; 
					p1 = p3 - outOffWidth; p2 = p3;
					break;
			}
			if(LocationSide == ToolTipLocationSide.Bottom || LocationSide == ToolTipLocationSide.Top) {
				points[1].X = p2;
				if(LocationSide == ToolTipLocationSide.Top) {
					points[0].X = p3;  
					points[2].X = p1;
				} else {
					points[0].X = p1;  
					points[2].X = p3;
				}
			} else {
				points[1].Y = p2; 
				if(LocationSide == ToolTipLocationSide.Left) {
					points[0].Y = p1; 
					points[2].Y = p3;
				} else {
					points[0].Y = p3; 
					points[2].Y = p1;
				}
			}
			switch(LocationSide) {
				case ToolTipLocationSide.Bottom:
					points[0].Y = Bounds.Top; points[2].Y = points[0].Y; points[1].Y = points[0].Y - outOffSize;
					break;
				case ToolTipLocationSide.Top:
					points[0].Y = Bounds.Bottom; points[2].Y = points[0].Y; points[1].Y = points[0].Y + outOffSize;
					break;
				case ToolTipLocationSide.Right:
					points[0].X = Bounds.Left; points[2].X = points[0].X; points[1].X = points[0].X - outOffSize;
					break;
				case ToolTipLocationSide.Left:
					points[0].X = Bounds.Right; points[2].X = points[0].X; points[1].X = points[0].X + outOffSize;
					break;
			}
			return points;
		}
		GraphicsPath CreateToolTipBeakPath() {
			GraphicsPath path = new GraphicsPath();
			path.AddLines(ToolTipBeakSetLocationPoints());
			return path;
		}
		GraphicsPath CreatePath() {
			GraphicsPath path = new GraphicsPath();
			Point[] points = ToolTipBeakSetLocationPoints();
			AddArcToPath(path, Bounds.Left, Bounds.Top, RoundRadius * 2, RoundRadius * 2, 180, 90);
			AddLineToPath(path, Bounds.Left + RoundRadius, Bounds.Top, Bounds.Left + Bounds.Width - RoundRadius * 2, Bounds.Top, points);
			AddArcToPath(path, Bounds.Left + Bounds.Width - RoundRadius * 2, Bounds.Top, RoundRadius * 2, RoundRadius * 2, 270, 90);
			AddLineToPath(path, Bounds.Left + Bounds.Width, Bounds.Top + RoundRadius, Bounds.Left + Bounds.Width, Bounds.Top + Bounds.Height - RoundRadius * 2, points);
			AddArcToPath(path, Bounds.Left + Bounds.Width - RoundRadius * 2, Bounds.Top + Bounds.Height - RoundRadius * 2, RoundRadius * 2, RoundRadius * 2, 0, 90);
			AddLineToPath(path, Bounds.Left + Bounds.Width - RoundRadius * 2, Bounds.Top + Bounds.Height, Bounds.Left + RoundRadius, Bounds.Top + Bounds.Height, points);
			AddArcToPath(path, Bounds.Left, Bounds.Top + Bounds.Height - RoundRadius * 2, RoundRadius * 2, RoundRadius * 2, 90, 90);
			AddLineToPath(path, Bounds.Left, Bounds.Top + Bounds.Height - RoundRadius * 2, Bounds.Left, Bounds.Top + RoundRadius, points);
			path.CloseFigure();
			return path;
		}
		void AddLineToPath(GraphicsPath path, int x, int y, int right, int bottom, Point[] points) {
			if(IsBeakOnLine(x, y, right, bottom, points)) {
				path.AddLine(points[0], points[1]);
				path.AddLine(points[1], points[2]);
			} else path.AddLine(x, y, right, bottom);
		}
		void AddArcToPath(GraphicsPath path, int x, int y, int width, int height, float startAngle, float sweepAngle) {
			if(RoundRadius <= 0) return;
			path.AddArc(x, y, width, height, startAngle, sweepAngle);
		}
		bool IsBeakOnLine(int x, int y, int right, int bottom, Point[] points) {
			if(points == null) return false;
			return IsBeakOnXLine(x, right, points) || IsBeakOnYLine(y, bottom, points);
		}
		bool IsBeakOnXLine(int x, int right, Point[] points) {
			return IsBeakOnLine(x, right, points[0].X, points[2].X);
		}
		bool IsBeakOnYLine(int y, int bottom, Point[] points) {
			return IsBeakOnLine(y, bottom, points[0].Y, points[2].Y);
		}
		bool IsBeakOnLine(int x, int right, int p0X, int p2X) {
			return x == right && x == p0X && x == p2X;
		}
	}
	public interface IToolTipLookAndFeelProvider {
		UserLookAndFeel LookAndFeel { get; }
	}
}
