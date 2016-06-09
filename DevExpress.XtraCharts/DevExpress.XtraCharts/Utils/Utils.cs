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
using System.ComponentModel;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.GLGraphics;
using DevExpress.XtraCharts.Localization;
using System.Reflection;
using System.Security;
using DevExpress.Charts.NotificationCenter;
namespace DevExpress.XtraCharts.Native {
	public interface IOwnedElement {
		IOwnedElement Owner { get; }
		IChartContainer ChartContainer { get; }
	}
	public interface IChartElementWizardAccess {
		void RaiseControlChanging();
		void RaiseControlChanged();
	}
	public class ChartElementUpdateInfo : ChartUpdateInfoBase {
		ChartElementChange flags;
		public ChartElementChange Flags { get { return flags; } set { flags = value; } }
		public ChartElementUpdateInfo(object sender, ChartElementChange flags)
			: base(sender) {
			this.flags = flags;
		}
	}
	public class ChartUpdateInfoHelper {
		public static bool IsSpecificUpdateInfo(ChartUpdateInfoBase changeInfo) {
			ChartElementUpdateInfo chartElementUpdateInfo = changeInfo as ChartElementUpdateInfo;
			if (chartElementUpdateInfo != null && chartElementUpdateInfo.Flags == ChartElementChange.NonSpecific)
				return false;
			return true;
		}
	}
	public static class SerializationUtils {
		public static Assembly ExecutingAssembly { get { return Assembly.GetExecutingAssembly(); } }
		public static string PublicNamespace { get { return "DevExpress.XtraCharts."; } }
	}
	public class ChartElementSynchronizer {
		ChartElement source;
		ChartElement destination;
		bool synchronize;
		public ChartElement Source {
			get {
				return source;
			}
			set {
				if (source != value) {
					source = value;
					Synchronize();
				}
			}
		}
		public ChartElement Destination {
			get {
				return destination;
			}
			set {
				if (destination != value) {
					destination = value;
					Synchronize();
				}
			}
		}
		public bool IsSynchronized {
			get {
				return synchronize;
			}
			set {
				if (synchronize != value) {
					synchronize = value;
					if (synchronize)
						Synchronize();
				}
			}
		}
		public ChartElementSynchronizer() {
			synchronize = true;
		}
		void Synchronize() {
			if (synchronize && (Source != null) && (Destination != null) && !Source.Loading && !Destination.Loading)
				Destination.Assign(source);
		}
		public void ElementChanged(ChartElement element) {
			if (element != null && synchronize) {
				if (element == Destination)
					synchronize = false;
				if (element == Source)
					Synchronize();
			}
		}
		public void EndLoading() {
			Synchronize();
		}
	}
}
namespace DevExpress.XtraCharts {
	internal class ResFinder {
	}
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum ChartImageSizeMode {
		AutoSize = 0,
		Stretch = 1,
		Zoom = 2,
		Tile = 3
	}
	[ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum SeriesPointKey {
		Argument = SeriesPointKeyNative.Argument,
		Value_1 = SeriesPointKeyNative.Value_1,
		Value_2 = SeriesPointKeyNative.Value_2,
		Value_3 = SeriesPointKeyNative.Value_3,
		Value_4 = SeriesPointKeyNative.Value_4
	}
	[Obsolete("This enumeration is now obsolete. Use the SeriesPointKey enumeration instead.")]
	public enum SortingKey {
		Argument = 0,
		Value_1 = 1,
		Value_2 = 2,
		Value_3 = 3,
		Value_4 = 4
	}
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum SortingMode {
		None = SortMode.None,
		Ascending = SortMode.Ascending,
		Descending = SortMode.Descending
	}
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum RotationType {
		UseMouseStandard,
		UseMouseAdvanced,
		UseAngles
	}
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum RotationOrder {
		XYZ,
		XZY,
		YXZ,
		YZX,
		ZXY,
		ZYX
	}
}
namespace DevExpress.XtraCharts.Native {
	[Flags]
	public enum ChartElementChange {
		ClearTextureCache = 1,
		KeepZoomCache = 2,
		SeriesPointCollectionChanged = 8,
		DataBindingChanged = 16,
		SmartBindingChanged = 32,
		SeriesPointFiltersChanged = 64,
		RangeChanged = 128,
		LimitsChanged = 256,
		NonSpecific = 512,
		RangeControlChanged = 1024
	}
	public delegate bool? GetLoadingDelegate();
	public delegate void SetLoadingDelegate(bool loading);
	public class WebLoadingHelper {
		[ThreadStatic]
		static bool globalLoadingStatic = false;
		public static bool GlobalLoadingStatic {
			get { return globalLoadingStatic; }
			set { globalLoadingStatic = value; }
		}
		public static bool GlobalLoading {
			get { return Helper.GetLoading(); }
			set { Helper.SetLoading(value); }
		}
		static WebLoadingHelper helper;
		static WebLoadingHelper Helper {
			get {
				if (helper == null)
					helper = new WebLoadingHelper();
				return helper;
			}
		}
		public static void Initialize(GetLoadingDelegate getLoadingCallback, SetLoadingDelegate setLoadingCallback) {
			Helper.getLoadingCallback = getLoadingCallback;
			Helper.setLoadingCallback = setLoadingCallback;
		}
		GetLoadingDelegate getLoadingCallback;
		SetLoadingDelegate setLoadingCallback;
		WebLoadingHelper() {
		}
		bool GetLoading() {
			bool? loading = getLoadingCallback != null ? getLoadingCallback() : null;
			return loading ?? globalLoadingStatic;
		}
		void SetLoading(bool loading) {
			if (setLoadingCallback != null)
				setLoadingCallback(loading);
		}
	}
	public class IntMinMaxValues {
		int minValue;
		int maxValue;
		public int MinValue {
			get { return minValue; }
			set { minValue = value; }
		}
		public int MaxValue {
			get { return maxValue; }
			set { maxValue = value; }
		}
		public IntMinMaxValues(int minValue, int maxValue) {
			this.minValue = minValue;
			this.maxValue = maxValue;
		}
	}
	public static class NameGenerator {
		static bool TestName(IList<string> nameList, string name) {
			for (int i = 0; i < nameList.Count; i++)
				if (nameList[i] == name)
					return false;
			return true;
		}
		public static string UniqueName(string baseName, IList<string> nameList) {
			string result = null;
			int i = 1;
			do result = baseName + Convert.ToString(i++);
			while (!TestName(nameList, result));
			return result;
		}
	}
	public enum SplitRectangleMode {
		Vertical,
		Horizontal
	}
	public static class GraphicUtils {
		static byte CalcGradientColorComponent(byte comp1, byte comp2, double ratio) {
			byte delta = (byte)Math.Round(Math.Abs(comp1 - comp2) * ratio);
			return comp1 < comp2 ? (byte)(comp1 + delta) : (byte)(comp1 - delta);
		}
		public static Color CalcGradientColor(Color color1, Color color2, double ratio) {
			if (ratio < 0)
				ratio = 0;
			if (ratio > 1)
				ratio = 1;
			byte R = CalcGradientColorComponent(color1.R, color2.R, ratio);
			byte G = CalcGradientColorComponent(color1.G, color2.G, ratio);
			byte B = CalcGradientColorComponent(color1.B, color2.B, ratio);
			byte A = CalcGradientColorComponent(color1.A, color2.A, ratio);
			return Color.FromArgb(A, R, G, B);
		}
		public static Color GetColor(Color color, HitTestState hitTestState) {
			Color c = hitTestState.ActualColor;
			return c == Color.Empty ? color : c;
		}
		public static GPoint2D ConvertPoint(Point point) {
			return new GPoint2D(point.X, point.Y);
		}
		public static GRect2D ConvertRect(Rectangle rect) {
			return new GRect2D(rect.Left, rect.Top, rect.Width, rect.Height);
		}
		public static GRect2D ConvertRect(RectangleF rect) {
			return ConvertRect(RoundRectangle(rect));
		}
		public static Rectangle RoundRectangle(RectangleF rect) {
			Point topLeft = new Point(MathUtils.StrongRound(rect.Left), MathUtils.StrongRound(rect.Top));
			Point bottomRight = new Point(MathUtils.StrongRound(rect.Right), MathUtils.StrongRound(rect.Bottom));
			Size size = new Size(bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
			return new Rectangle(topLeft, size);
		}
		public static Rectangle MakeRectangle(Point corner1, Point corner2) {
			int x = corner1.X < corner2.X ? corner1.X : corner2.X;
			int y = corner1.Y < corner2.Y ? corner1.Y : corner2.Y;
			int width = Math.Abs(corner1.X - corner2.X) + 1;
			int height = Math.Abs(corner1.Y - corner2.Y) + 1;
			return new Rectangle(x, y, width, height);
		}
		public static Rectangle[] SplitRectangle(Rectangle rect, SplitRectangleMode mode, bool weld) {
			int seam = weld ? 1 : 0;
			switch (mode) {
				case SplitRectangleMode.Vertical:
					int halfHeight = rect.Height >> 1;
					return new Rectangle[] { 
						new Rectangle(rect.X, rect.Y, rect.Width, halfHeight), 
						new Rectangle(rect.X, rect.Y + halfHeight - seam, rect.Width, rect.Height - halfHeight + seam) 
					};
				case SplitRectangleMode.Horizontal:
					int halfWidth = rect.Width >> 1;
					return new Rectangle[] { 
						new Rectangle(rect.X, rect.Y, halfWidth, rect.Height), 
						new Rectangle(rect.X + halfWidth - seam, rect.Y, rect.Width - halfWidth + seam, rect.Height) 
					};
			}
			return new Rectangle[] { Rectangle.Empty, Rectangle.Empty };
		}
		public static RectangleF[] SplitRectangle(RectangleF rect, SplitRectangleMode mode, bool weld) {
			int seam = weld ? 1 : 0;
			switch (mode) {
				case SplitRectangleMode.Vertical:
					float halfHeight = (float)Math.Floor(rect.Height / 2);
					return new RectangleF[] { 
						new RectangleF(rect.X, rect.Y, rect.Width, halfHeight), 
						new RectangleF(rect.X, rect.Y + halfHeight - seam, rect.Width, rect.Height - halfHeight + seam) 
					};
				case SplitRectangleMode.Horizontal:
					float halfWidth = (float)Math.Floor(rect.Width / 2);
					return new RectangleF[] { 
						new RectangleF(rect.X, rect.Y, halfWidth, rect.Height), 
						new RectangleF(rect.X + halfWidth - seam, rect.Y, rect.Width - halfWidth + seam, rect.Height) 
					};
			}
			return new RectangleF[] { RectangleF.Empty, RectangleF.Empty };
		}
		public static RectangleF MakeRectangle(PointF corner1, PointF corner2) {
			float x = corner1.X < corner2.X ? corner1.X : corner2.X;
			float y = corner1.Y < corner2.Y ? corner1.Y : corner2.Y;
			float width = Math.Abs(corner1.X - corner2.X);
			float height = Math.Abs(corner1.Y - corner2.Y);
			return new RectangleF(x, y, width, height);
		}
		public static bool CheckIsSizePositive(Size size) {
			return size.Width > 0 && size.Height > 0;
		}
		public static bool CheckIsSizePositive(SizeF size) {
			return size.Width > 0.0F && size.Height > 0.0F;
		}
		public static double CalcRadius(double diameter, bool roundToInteger) {
			double radius = diameter / 2.0;
			if (roundToInteger)
				radius = Math.Floor(radius);
			if (radius < 1.0)
				radius = 1.0;
			return radius;
		}
		public static GRealPoint2D CalcCenter(ZPlaneRectangle rect, bool roundToInteger) {
			GRealPoint2D center = new GRealPoint2D(rect.Left + rect.Width / 2.0, rect.Bottom + rect.Height / 2.0);
			return roundToInteger ? new GRealPoint2D(Math.Floor(center.X), Math.Floor(center.Y)) : center;
		}
		public static PointF CalcCenter(RectangleF rect, bool roundToInteger) {
			PointF center = new PointF(rect.Left + rect.Width / 2.0f, rect.Bottom - rect.Height / 2.0f);
			return roundToInteger ? new PointF((float)Math.Floor(center.X), (float)Math.Floor(center.Y)) : center;
		}
		public static Point Central2Screen(RectangleF bounds, PointF point) {
			return new Point((int)Math.Round(bounds.X + bounds.Width / 2 + point.X), (int)Math.Round(bounds.Y + bounds.Height / 2 - point.Y));
		}
		public static Region GetEmptyRegion() {
			Region rgn = new Region();
			rgn.MakeEmpty();
			return rgn;
		}
		public static float CalcFontEmSize(TextMeasurer textMeasurer, RectangleF bounds, string text, FontFamily family, StringAlignment alignment, StringAlignment lineAlignment) {
			if (text == "") return 0;
			float emSize = 300;
			using (Font font = new Font(family, emSize)) {
				SizeF stringSize = textMeasurer.MeasureString(text, font, alignment, lineAlignment);
				if (bounds.Height / bounds.Width > stringSize.Height / stringSize.Width)
					emSize = emSize * bounds.Width / stringSize.Width;
				else
					emSize = emSize * bounds.Height / stringSize.Height;
			}
			return emSize;
		}
		public static RectangleF InflateRect(RectangleF rect, float dx, float dy) {
			rect.Offset(-dx, -dy);
			float width = rect.Width + dx * 2;
			float height = rect.Height + dy * 2;
			rect.Width = width > 0 ? width : 0;
			rect.Height = height > 0 ? height : 0;
			return rect;
		}
		public static Rectangle InflateRect(Rectangle rect, int dx, int dy) {
			rect.Offset(-dx, -dy);
			int width = rect.Width + dx * 2;
			int height = rect.Height + dy * 2;
			rect.Width = width > 0 ? width : 0;
			rect.Height = height > 0 ? height : 0;
			return rect;
		}
		public static int CalcAlignedTextureSize(int size) {
			int n = MathUtils.Ceiling(Math.Log(size, 2));
			return (int)Math.Pow(2, n);
		}
		public static int CalcAlignedTextureSize(int size, int maxSize) {
			int texSize = CalcAlignedTextureSize(size);
			return texSize > maxSize ? maxSize : texSize;
		}
		public static IHitRegion MakeHitRegion(IPolygon polygon) {
			return
				polygon != null ?
				new HitRegion(polygon.GetPath()) :
				new HitRegion();
		}
		public static GRealPoint2D CalcCirclePoint(GRealPoint2D center, double radius, double angle) {
			double x = radius * Math.Cos(angle);
			double y = radius * Math.Sin(angle);
			return new GRealPoint2D(center.X + x, center.Y + y);
		}
		public static LineStrip CalcPolygon(double startAngle, int verticesCount, GRealPoint2D center, float radius) {
			double angleStep = Math.PI * 2.0 / verticesCount;
			LineStrip points = new LineStrip(verticesCount);
			for (int i = 0; i < verticesCount; i++)
				points.Add(CalcCirclePoint(center, radius, startAngle + angleStep * i));
			return points;
		}
		public static RectangleF BoundsFromPointsArray(IList<GRealPoint2D> points) {
			if (points.Count == 0)
				return RectangleF.Empty;
			double left = double.MaxValue;
			double top = double.MaxValue;
			double right = double.MinValue;
			double bottom = double.MinValue;
			foreach (GRealPoint2D point in points) {
				left = Math.Min(left, point.X);
				top = Math.Min(top, point.Y);
				right = Math.Max(right, point.X);
				bottom = Math.Max(bottom, point.Y);
			}
			return RectangleF.FromLTRB((float)left, (float)top, (float)(right), (float)(bottom));
		}
		static void AddAnchorPoint(LineStrip anchorPoints, GRealPoint2D point) {
			if (!Double.IsNaN(point.X) && !Double.IsNaN(point.Y))
				anchorPoints.Add(point);
		}
		public static LineStrip FillAnchorPoints(IEllipse ellipse, double startAngle, double sweepAngle, bool addCenter) {
			LineStrip anchorPoints = new LineStrip();
			AddAnchorPoint(anchorPoints, ellipse.CalcEllipsePoint(startAngle));
			AddAnchorPoint(anchorPoints, ellipse.CalcEllipsePoint(startAngle + sweepAngle));
			if (addCenter)
				anchorPoints.Add(ellipse.Center);
			double normalizedStartAngle = GeometricUtils.NormalizeRadian(startAngle);
			double normalizedFinishAngle = normalizedStartAngle + sweepAngle;
			if (normalizedFinishAngle < normalizedStartAngle) {
				double temp = normalizedStartAngle;
				normalizedStartAngle = normalizedFinishAngle;
				normalizedFinishAngle = temp;
			}
			double beginAngle = -4 * Math.PI;
			double endAngle = 4 * Math.PI;
			for (int multiplier = 0; ; multiplier++) {
				double angle = beginAngle + multiplier * Math.PI / 2;
				if (angle > endAngle)
					break;
				if (angle > normalizedStartAngle && angle < normalizedFinishAngle)
					AddAnchorPoint(anchorPoints, ellipse.CalcEllipsePoint(angle));
			}
			return anchorPoints;
		}
		public static void CalculateStartFinishPoints(GRealPoint2D center, float majorSemiaxis, float minorSemiaxis, float startAngle, float sweepAngle, out GRealPoint2D startPoint, out GRealPoint2D finishPoint) {
			Ellipse ellipse = new Ellipse(center, majorSemiaxis, minorSemiaxis);
			startPoint = ellipse.CalcEllipsePoint(-MathUtils.Degree2Radian(startAngle));
			finishPoint = ellipse.CalcEllipsePoint(-MathUtils.Degree2Radian(startAngle + sweepAngle));
		}
		public static GraphicsPath CreatePieGraphicsPath(GRealPoint2D center, float majorSemiaxis, float minorSemiaxis, float holePercent, float startAngle, float sweepAngle) {
			GraphicsPath path = new GraphicsPath();
			try {
				Rectangle emptyRect = new Rectangle((int)Math.Round(center.X), (int)Math.Round(center.Y), 0, 0);
				Rectangle rect = Rectangle.Inflate(emptyRect, Convert.ToInt32(majorSemiaxis), Convert.ToInt32(minorSemiaxis));
				if (!rect.AreWidthAndHeightPositive())
					return path;
				bool shouldAddLines = sweepAngle != -360.0;
				float innerMajorSemiaxis = majorSemiaxis * holePercent;
				float innerMinorSemiaxis = minorSemiaxis * holePercent;
				if (innerMajorSemiaxis >= 1.0f && innerMinorSemiaxis >= 1.0f) {
					Rectangle innerRect = Rectangle.Inflate(emptyRect, Convert.ToInt32(innerMajorSemiaxis), Convert.ToInt32(innerMinorSemiaxis));
					GRealPoint2D startPoint = new GRealPoint2D(), finishPoint = new GRealPoint2D();
					GRealPoint2D innerStartPoint = new GRealPoint2D(), innerFinishPoint = new GRealPoint2D();
					if (shouldAddLines) {
						CalculateStartFinishPoints(center, majorSemiaxis, minorSemiaxis, startAngle, sweepAngle, out startPoint, out finishPoint);
						CalculateStartFinishPoints(center, innerMajorSemiaxis, innerMinorSemiaxis, startAngle, sweepAngle, out innerStartPoint, out innerFinishPoint);
					}
					path.AddArc(rect, startAngle, sweepAngle);
					if (shouldAddLines)
						path.AddLine(new PointF((float)finishPoint.X, (float)finishPoint.Y), new PointF((float)innerFinishPoint.X, (float)innerFinishPoint.Y));
					path.AddArc(innerRect, startAngle + sweepAngle, -sweepAngle);
					if (shouldAddLines)
						path.AddLine(new PointF((float)startPoint.X, (float)startPoint.Y), new PointF((float)innerStartPoint.X, (float)innerStartPoint.Y));
				} else if (shouldAddLines)
					path.AddPie(rect, startAngle, sweepAngle);
				else
					path.AddEllipse(rect);
			} catch {
				path.Dispose();
				throw;
			}
			return path;
		}
		static bool IsGrayComponent(int colorComponent) {
			return colorComponent >= 110 && colorComponent <= 150;
		}
		static bool IsGrayColor(Color c) {
			return IsGrayComponent(c.R) && IsGrayComponent(c.G) && IsGrayComponent(c.B);
		}
		public static Color XorColor(Color color) {
			if (IsGrayColor(color))
				return Color.White;
			else
				return Color.FromArgb(255, color.R ^ 255, color.G ^ 255, color.B ^ 255);
		}
		public static int CorrectThicknessBySelectionState(int thickness, SelectionState selectionState) {
			return CorrectThicknessBySelectionState(thickness, selectionState, 2);
		}
		public static int CorrectThicknessBySelectionState(int thickness, SelectionState selectionState, int increment) {
			if (selectionState != SelectionState.Normal)
				thickness += 2;
			return thickness;
		}
		public static int CorrectThicknessByHitTestState(int thickness, HitTestState hitState) {
			return CorrectThicknessByHitTestState(thickness, hitState, 2);
		}
		public static int CorrectThicknessByHitTestState(int thickness, HitTestState hitState, int increment) {
			if (!hitState.Normal)
				thickness += increment;
			return thickness;
		}
		public static Color CorrectColorByHitTestState(Color color, HitTestState hitState) {
			if (hitState.Hot)
				return HitTestColors.MixColors(Color.FromArgb(100, 255, 255, 255), color);
			if (hitState.Select)
				return HitTestColors.MixColors(Color.FromArgb(75, 0, 0, 0), color);
			return color;
		}
		public static Color CorrectColorBySelectionState(Color color, SelectionState state) {
			switch (state) {
				case SelectionState.Selected:
					return HitTestColors.MixColors(Color.FromArgb(75, 0, 0, 0), color);
				case SelectionState.HotTracked:
					return HitTestColors.MixColors(Color.FromArgb(100, 255, 255, 255), color);
				case SelectionState.Normal:
				default:
					return color;
			}
		}
		public static RectangleF CalcBoundRectangle(GRealPoint2D center, float semiAxis1, float semiAxis2) {
			return RectangleF.Inflate(new RectangleF((float)center.X, (float)center.Y, 0, 0), semiAxis1, semiAxis2);
		}
	}
	public class TextureInfo {
		static int CalcTexSize(int texSize, int maxTexSize) {
			if (texSize <= maxTexSize)
				return texSize;
			while (texSize > maxTexSize)
				texSize /= 2;
			return texSize;
		}
		[System.Security.SecuritySafeCritical]
		static TextureInfo[] CalcTextureInfos(BitmapData data, int frameWidth, int frameHeight) {
			ArrayList list = new ArrayList();
			for (int y = 0; y < data.Height; y += frameHeight) {
				for (int x = 0; x < data.Width; x += frameWidth) {
					byte[] pixels = new byte[frameWidth * frameHeight * 4];
					for (int frameLine = 0; frameLine < frameHeight; frameLine++) {
						int offset = y * data.Width * 4 + x * 4 + frameLine * data.Width * 4;
						long ptr = IntPtr.Size == 4 ? data.Scan0.ToInt32() : data.Scan0.ToInt64();
						System.Runtime.InteropServices.Marshal.Copy(
							(IntPtr)(ptr + offset),
							pixels,
							frameLine * frameWidth * 4,
							frameWidth * 4);
					}
					list.Add(new TextureInfo(IntPtr.Zero, pixels, x, y, frameWidth, frameHeight, data.Width, data.Height));
				}
			}
			return (TextureInfo[])list.ToArray(typeof(TextureInfo));
		}
		static TextureInfo[] CalcTextureInfos(byte[] texPixels, int frameWidth, int frameHeight, int texWidth, int texHeight) {
			ArrayList list = new ArrayList();
			for (int y = 0; y < texHeight; y += frameHeight) {
				for (int x = 0; x < texWidth; x += frameWidth) {
					byte[] pixels = new byte[frameWidth * frameHeight * 4];
					for (int frameLine = 0; frameLine < frameHeight; frameLine++) {
						int offset = y * texWidth * 4 + x * 4 + frameLine * texWidth * 4;
						Array.Copy(texPixels, offset, pixels, frameLine * frameWidth * 4, frameWidth * 4);
					}
					list.Add(new TextureInfo(IntPtr.Zero, pixels, x, y, frameWidth, frameHeight, texWidth, texHeight));
				}
			}
			return (TextureInfo[])list.ToArray(typeof(TextureInfo));
		}
		public static TextureInfo[] CalcTextureInfos(System.Drawing.Image image, int maxTexSize, Size bounds) {
			int width = image.Width > bounds.Width ? bounds.Width : image.Width;
			int height = image.Height > bounds.Height ? bounds.Height : image.Height;
			int texWidth = GraphicUtils.CalcAlignedTextureSize(width);
			int texHeight = GraphicUtils.CalcAlignedTextureSize(height);
			byte[] pixels = new byte[texWidth * texHeight * 4];
			using (Bitmap bitmap = new Bitmap(image)) {
				BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
				GLU.ScaleImage(GL.BGRA_EXT, bitmap.Width, bitmap.Height, GL.UNSIGNED_BYTE, data.Scan0, texWidth, texHeight, GL.UNSIGNED_BYTE, pixels);
				bitmap.UnlockBits(data);
			}
			int frameWidth = CalcTexSize(texWidth, maxTexSize);
			int frameHeight = CalcTexSize(texHeight, maxTexSize);
			return CalcTextureInfos(pixels, frameWidth, frameHeight, texWidth, texHeight);
		}
		public static TextureInfo[] CalcTextureInfos(BitmapData data, int maxTexSize) {
			int frameWidth = CalcTexSize(data.Width, maxTexSize);
			int frameHeight = CalcTexSize(data.Height, maxTexSize);
			if (frameWidth == data.Width && frameHeight == data.Height)
				return new TextureInfo[] { new TextureInfo(data.Scan0, null, 0, 0, frameWidth, frameHeight, data.Width, data.Height) };
			return CalcTextureInfos(data, frameWidth, frameHeight);
		}
		IntPtr texture;
		byte[] pixels;
		int x, y, width, height, texWidth, texHeight;
		public IntPtr TexturePointer { get { return texture; } }
		public byte[] TextureArray { get { return pixels; } }
		public int X { get { return x; } }
		public int Y { get { return y; } }
		public int Width { get { return width; } }
		public int Height { get { return height; } }
		public int TexWidth { get { return texWidth; } }
		public int TexHeight { get { return texHeight; } }
		public TextureInfo(IntPtr texture, byte[] pixels, int x, int y, int width, int height, int texWidth, int texHeight) {
			this.texture = texture;
			this.pixels = pixels;
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
			this.texWidth = texWidth;
			this.texHeight = texHeight;
		}
	}
	public static class ThicknessUtils {
		public const int Correction = 1;
		public static void SplitToHalfs(int thickness, out int firstHalf, out int secondHalf) {
			int offset = ThicknessToOffset(thickness);
			if (offset > 0) {
				firstHalf = offset % 2 == 0 ? offset / 2 : MathUtils.Ceiling((double)offset / 2);
				secondHalf = offset - firstHalf;
			} else {
				firstHalf = 0;
				secondHalf = 0;
			}
		}
		public static void SplitToHalfs(int thickness, HitTestState hitState, out int firstHalf, out int secondHalf) {
			int correctedThickness = CorrectThicknessByHitState(thickness, hitState);
			SplitToHalfs(correctedThickness, out firstHalf, out secondHalf);
		}
		public static int ThicknessToOffset(int thickness) {
			if (thickness == 0)
				throw new ArgumentException("thickness must be > 0");
			return thickness - 1;
		}
		public static int ThicknessToOffset(int thickness, HitTestState hitState) {
			int correctedThickness = CorrectThicknessByHitState(thickness, hitState);
			return ThicknessToOffset(correctedThickness);
		}
		public static int CorrectThicknessByHitState(int thickness, HitTestState hitState) {
			if (thickness < 0)
				throw new ArgumentException("thickness must be >= 0");
			if (hitState == null)
				throw new ArgumentNullException("hitState");
			return hitState.Normal ? thickness : thickness + Correction;
		}
	}
	public sealed class StringUtils {
		public static string StringArrayToString(string[] array) {
			if (array == null || array.Length <= 0)
				return String.Empty;
			System.Text.StringBuilder sb = new System.Text.StringBuilder(array[0]);
			int count = array.Length;
			for (int i = 1; i < count; i++) {
				sb.Append("\r\n");
				sb.Append(array[i]);
			}
			return sb.ToString();
		}
		public static string[] StringToStringArray(string str) {
			return (str != null) ? System.Text.RegularExpressions.Regex.Split(str, "\r\n") : new string[] { };
		}
		StringUtils() {
		}
	}
	public static class CommonUtils {
		static bool IsSeriesVisible(Series series) {
			return series.Visible && series.View.IsSupportedCrosshair &&
				!series.ChartContainer.Chart.ViewController.SeriesIncompatibilityStatistics.IsSeriesIncompatible(series);
		}
		static void ValidateIndocator(Indicator indicator, Series series) {
			IRefinedSeries refinedSeries = series.Chart != null ? series.Chart.ViewController.FindRefinedSeries(series) : null;
			indicator.Validate((XYDiagram2DSeriesViewBase)series.View, refinedSeries);
		}
		static List<XYDiagram2DSeriesViewBase> FindXYDiagram2DSeriesViews(Chart chart, Predicate<XYDiagram2DSeriesViewBase> viewFound) {
			List<XYDiagram2DSeriesViewBase> views = new List<XYDiagram2DSeriesViewBase>();
			foreach (Series series in chart.Series) {
				XYDiagram2DSeriesViewBase xyView = series.View as XYDiagram2DSeriesViewBase;
				if (xyView != null && viewFound(xyView))
					views.Add(xyView);
			}
			return views;
		}
		public static Chart FindOwnerChart(ChartElement element) {
			if (element == null)
				return null;
			if (element is Chart)
				return (Chart)element;
			return FindOwnerChart(element.Owner);
		}
		public static IChartContainer FindChartContainer(ChartElement element) {
			Chart chart = FindOwnerChart(element);
			return chart != null ? chart.Container : null;
		}
		public static IChartAppearance GetActualAppearance(ChartElement element) {
			Chart chart = FindOwnerChart(element);
			return chart == null ? null : chart.Appearance;
		}
		public static bool CheckSortingKey(SeriesPointKey key, int pointDimension) {
			return (int)key <= pointDimension;
		}
		public static List<CrosshairValueItem> GetCrosshairSortedData(Series series) {
			Chart chart = series.Chart;
			if (chart != null && chart.ActualCrosshairEnabled && chart.Diagram != null) {
				IRefinedSeries refinedSeries = chart.ViewController.FindRefinedSeries(series);
				XYDiagram2D XYDiagram2D = chart.Diagram as XYDiagram2D;
				if (XYDiagram2D != null)
					return XYDiagram2D.GetCrosshairSortedData(refinedSeries);
			}
			return null;
		}
		public static IList<RefinedPoint> GetDisplayPoints(Series series) {
			IRefinedSeries refinedSeries = series.Chart != null ? series.Chart.ViewController.FindRefinedSeries(series) : null;
			if (refinedSeries != null)
				return refinedSeries.Points;
			return new List<RefinedPoint>();
		}
		public static Rectangle GetLabelBounds(XYDiagramPaneBase pane, Axis2D axis) {
			return axis.LabelBounds != null && axis.LabelBounds.ContainsKey(pane) ? axis.LabelBounds[pane] : Rectangle.Empty;
		}
		public static Color? GetCrosshairMarkerColor(Series series) {
			if (IsSeriesVisible(series))
				return series.View.ActualColorEach ? null : new Color?(series.View.ActualColor);
			return null;
		}
		public static Color? GetCrosshairMarkerColor(Series series, int pointIndex, int pointsCount) {
			if (IsSeriesVisible(series))
				return new Color?(series.View.GetPointColor(pointIndex, pointsCount));
			return null;
		}
		public static bool GetActualMarkerVisible(SeriesViewBase view, MarkerBase marker) {
			RangeAreaSeriesView rangeAreaView = view as RangeAreaSeriesView;
			if (rangeAreaView != null) {
				if (marker == rangeAreaView.Marker1)
					return rangeAreaView.ActualMarker1Visible;
				if (marker == rangeAreaView.Marker2)
					return rangeAreaView.ActualMarker2Visible;
			} else {
				LineSeriesView lineView = view as LineSeriesView;
				if (lineView != null)
					return lineView.ActualMarkerVisible;
				else {
					RadarLineSeriesView radarView = view as RadarLineSeriesView;
					if (radarView != null)
						return radarView.ActualMarkerVisible;
					else {
						RangeBarSeriesView rangeBarView = view as RangeBarSeriesView;
						if (rangeBarView != null) {
							if (marker == rangeBarView.MinValueMarker)
								return rangeBarView.ActualMinValueMarkerVisible;
							if (marker == rangeBarView.MaxValueMarker)
								return rangeBarView.ActualMaxValueMarkerVisible;
						}
					}
				}
			}
			return false;
		}
		public static bool GetActualSeriesLabelLineVisibility(SeriesLabelBase label) {
			return label.ActualLineVisible;
		}
		public static bool GetActualBorderVisibility(BorderBase border) {
			return border.ActualVisibility;
		}
		public static bool GetSeriesActualCrosshairLabelVisibility(Series series) {
			return series.ActualCrosshairLabelVisibility;
		}
		public static bool GetSeriesActualCrosshairEnabled(Series series) {
			return series.ActualCrosshairEnabled;
		}
		public static bool GetSeriesActualToolTipEnabled(Series series) {
			return series.ActualToolTipEnabled;
		}
		public static XYDiagram2D GetXYDiagram2D(ChartElement element) {
			if (element == null || element.ChartContainer == null)
				return null;
			Chart chart = element.ChartContainer.Chart as Chart;
			return chart != null ? chart.Diagram as XYDiagram2D : null;
		}
		public static void CheckDockTarget(ChartElement value, ChartElement owner) {
			if (!(value is IDockTarget))
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectFreePositionDockTarget));
			XYDiagram2D diagram = GetXYDiagram2D(owner);
			XYDiagramPaneBase pane = value as XYDiagramPaneBase;
			if (diagram != null && pane != null && pane != diagram.DefaultPane && !diagram.Panes.Contains(pane))
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectFreePositionDockTarget));
		}
		public static ScaleMode GetDateTimeScaleMode(AxisBase axis) {
			return axis.ActualDateTimeScaleMode;
		}
		public static SeriesIncompatibilityStatistics GetSeriesIncompatibilityStatistics(Chart chart) {
			return chart.ViewController.SeriesIncompatibilityStatistics;
		}
		public static SummaryFunctionsStorage GetSummaryFunctions(Chart chart) {
			if (chart == null)   
				return null;
			return chart.SummaryFunctions;
		}
		public static bool IsUnboundMode(Series series) {
			return series.UnboundMode;
		}
		public static int AddToSecondaryAxisCollection(SecondaryAxisCollection secondaryAxisCollection, Axis2D axis) {
			return secondaryAxisCollection.AddInternal(axis);
		}
		public static bool IsShouldFilterZeroAlignment(Axis2D axis) {
			return axis.ShouldFilterZeroAlignment;
		}
		public static int AddWithoutChanged(ChartCollectionBase chartCollectionBase, ChartElement item) {
			return chartCollectionBase.AddWithoutChanged(item);
		}
		public static void Validate(Indicator indicator, XYDiagram2DSeriesViewBase view) {
			if (view.Owner is Series)
				ValidateIndocator(indicator, view.Series);
			else if (view.Owner is SeriesBase)
				foreach (Series series in view.Chart.DataContainer.AutocreatedSeries)
					ValidateIndocator(indicator, series);
		}
		public static void CopySettings(SeriesViewBase dest, SeriesViewBase src) {
			dest.CopySettings(src);
		}
		public static List<XYDiagram2DSeriesViewBase> FindViewsByPane(XYDiagramPaneBase pane, Chart chart) {
			return FindXYDiagram2DSeriesViews(chart, (XYDiagram2DSeriesViewBase view) => { return view.ActualPane == pane; });
		}
		public static List<XYDiagram2DSeriesViewBase> FindViewsByAxisX(Axis2D axisX, Chart chart) {
			return FindXYDiagram2DSeriesViews(chart, (XYDiagram2DSeriesViewBase view) => { return view.ActualAxisX == axisX; });
		}
		public static List<XYDiagram2DSeriesViewBase> FindViewsByAxisY(Axis2D axisY, Chart chart) {
			return FindXYDiagram2DSeriesViews(chart, (XYDiagram2DSeriesViewBase view) => { return view.ActualAxisY == axisY; });
		}
	}
	public static class Constants {
		public const string SaveLoadLayoutFileDialogFilter = "XML files(*.xml)|*.xml|All files (*.*)|*.*";
		public const int AxisXDefaultMinorCount = 4;
		public const int AxisXGridSpacingFactor = 50;
		public const int AxisYDefaultMinorCount = 2;
		public const int AxisYGridSpacingFactor = 30;
	}
	public class NonTestablePropertyAttribute : Attribute {
		public NonTestablePropertyAttribute()
			: base() {
		}
	}
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public class RuntimeObjectAttribute : Attribute {
	}
	public class Locker {
		int lockCount = 0;
		public bool IsLocked { get { return this.lockCount > 0; } }
		public void Lock() {
			this.lockCount++;
		}
		public void Unlock() {
			if (IsLocked)
				this.lockCount--;
		}
	}
	public static class XYDiagramMappingHelper {
		static double CorrectAngleRotated(XYDiagramMappingBase diagramMapping, double angle) {
			double correctedAngle = Math.PI / 2 - angle;
			if (diagramMapping.XReverse)
				correctedAngle = -correctedAngle;
			if (diagramMapping.YReverse)
				correctedAngle = Math.PI - correctedAngle;
			return correctedAngle;
		}
		static double CorrectAngleNotRotated(XYDiagramMappingBase diagramMapping, double angle) {
			double correctedAngle = angle;
			if (diagramMapping.XReverse)
				correctedAngle = Math.PI - correctedAngle;
			if (diagramMapping.YReverse)
				correctedAngle = -correctedAngle;
			return correctedAngle;
		}
		public static DiagramPoint ApplyYOffsetToPoint(XYDiagramMappingBase diagramMapping, DiagramPoint point, int offset) {
			DiagramPoint pointWithOffset = point;
			if (diagramMapping.Rotated)
				pointWithOffset.X += diagramMapping.YReverse ? -offset : offset;
			else
				pointWithOffset.Y += diagramMapping.YReverse ? offset : -offset;
			return pointWithOffset;
		}
		public static double CorrectAngle(XYDiagramMappingBase diagramMapping, double angle) {
			if (diagramMapping.Rotated)
				return CorrectAngleRotated(diagramMapping, angle);
			else
				return CorrectAngleNotRotated(diagramMapping, angle);
		}
		public static bool PointInMappingBounds(Rectangle mappingBounds, GPoint2D point) {
			return point.X >= mappingBounds.Left && point.X <= mappingBounds.Right &&
				point.Y >= mappingBounds.Top && point.Y <= mappingBounds.Bottom;
		}
		public static MappingTransform CreateTransform(Rectangle mappingBounds, bool rotated, bool xReverse, bool yReverse) {
			bool horizontalReverse = rotated ? yReverse : xReverse;
			bool verticalReverse = rotated ? xReverse : yReverse;
			return new MappingTransform(mappingBounds, rotated, horizontalReverse, verticalReverse);
		}
		public static DiagramPoint InterimPointToScreenPoint(DiagramPoint interimPoint, XYDiagramMappingContainer mappingContainer) {
			DiagramPoint diagramPoint = mappingContainer.Transform.Map(interimPoint);
			return MatrixTransform.Project(diagramPoint, mappingContainer.MappingBounds);
		}
	}
	public sealed class NativeUtilsImport {
		[DllImport("user32.dll")]
		public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
	}
	public sealed class NativeUtils {
		public const int LVM_FIRST = 0x1000;
		public const int LVM_SETCOLUMNWIDTH = LVM_FIRST + 30;
		public const int LVSCW_AUTOSIZE = -1;
		public const int LVSCW_AUTOSIZE_USEHEADER = -2;
		[SecuritySafeCritical]
		public static IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam) {
			return NativeUtilsImport.SendMessage(hWnd, Msg, wParam, lParam);
		}
	}
	public static class SerializingUtils {
		const char separator = ';';
		public const string SerializableDateTimeFormat = "MM/dd/yyyy HH:mm:ss.fff";
		public static string StringArrayToString(string[] values) {
			return values.Length > 0 ? string.Join(new string(separator, 1), values) : "";
		}
		public static string[] StringToStringArray(string value) {
			value.Trim();
			if (value == "")
				return new string[0];
			string[] array = value.Split(separator);
			for (int i = 0; i < array.Length; i++)
				array[i].Trim();
			return array;
		}
		public static string ConvertToSerializable(object value) {
			if (value is double)
				return ((double)value).ToString(NumberFormatInfo.InvariantInfo);
			else if (value is DateTime)
				return ((DateTime)value).ToString(SerializableDateTimeFormat, DateTimeFormatInfo.InvariantInfo);
			else if (value is string)
				return (string)value;
			else
				return string.Empty;
		}
		public static object ConvertFromSerializable(string value) {
			try {
				return Convert.ToDouble(value, NumberFormatInfo.InvariantInfo);
			} catch {
				try {
					return Convert.ToDateTime(value, DateTimeFormatInfo.InvariantInfo);
				} catch {
				}
			}
			return value;
		}
	}
	public static class Categories {
		public const string Behavior = "Behavior";
		public const string Appearance = "Appearance";
		public const string Elements = "Elements";
		public const string Data = "Data";
		public const string Layout = "Layout";
	}
	public static class DefaultFonts {
		public static readonly Font Tahoma8 = new Font("Tahoma", 8);
		public static readonly Font Tahoma10 = new Font("Tahoma", 10);
		public static readonly Font Tahoma12 = new Font("Tahoma", 12);
		public static readonly Font Tahoma18 = new Font("Tahoma", 18);
	}
	public static class DefaultBooleanUtils {
		public static bool ToBoolean(DefaultBoolean value, bool defaultValue) {
			return value == DefaultBoolean.Default ? defaultValue : value == DefaultBoolean.True ? true : false;
		}
		public static DefaultBoolean ToDefaultBoolean(bool value) {
			return value ? DefaultBoolean.True : DefaultBoolean.False;
		}
	}
	public static class DateTimeFormatUtils {
		public static void GetDateTimeFormat(DateTimeMeasureUnit measureUnit, out DateTimeFormat format, out string formatString) {
			DateTimeFormatParts formatParts = DateTimeUtilsExt.SelectFormat((DateTimeGridAlignmentNative)measureUnit);
			format = (DateTimeFormat)formatParts.Format;
			formatString = formatParts.FormatString;
		}
	}
	public class NestedTagPropertyAttribute : Attribute {
	}
	public static class ToolTipUtils {
		public static double GetPointPercentValue(RefinedPoint point, Series series) {
			if (series.View is PieSeriesViewBase) {
				return ((IPiePoint)point).NormalizedValue;
			} else if (series.View is FunnelSeriesViewBase) {
				return ((IFunnelPoint)point).NormalizedValue;
			} else {
				double min = ((IStackedPoint)point).MinValue;
				double max = ((IStackedPoint)point).MaxValue;
				if (double.IsNaN(min) || double.IsNaN(max))
					return double.NaN;
				return max - min;
			}
		}
	}
	public class ChartElementFactory {
		public static Series CreateEmptySeries(string name, DataContainer owner, SeriesViewBase view) {
			Series series = new Series(name, SeriesViewFactory.GetViewType(view));
			series.Owner = owner;
			return series;
		}
		public static Series CreateEmptySeries(string name, Chart chart, SeriesViewBase view) {
			return CreateEmptySeries(name, chart.DataContainer, view);
		}
		public static Axis2D CreateNewAxis(SecondaryAxisCollection axesCollection) {
			return axesCollection.CreateNewAxis();
		}
		public static RectangleFillStyle CreateRectangleFillStyle() {
			return new RectangleFillStyle(null);
		}
		public static RectangleFillStyle3D CreateRectangleFillStyle3D() {
			return new RectangleFillStyle3D(null);
		}
		public static BackgroundImage CreateBackgroundImage() {
			return new BackgroundImage(null);
		}
	}
	public static class PatternEditorUtils {
		const int RandomPointsCount = 1;
		static IPatternValuesSource CreateValuesSourceFromAxis(IAxisData axis) {
			return new PatternEditorValuesSource(axis.VisualRange.MaxValue);
		}
		static IPatternValuesSource CreateValuesSourceFromRangeColorizer() {
			return new PatternEditorValuesSource(0, 100);
		}
		static IPatternValuesSource CreateValuesSourceFromRangeKeyColorColorizer() {
			return new PatternEditorValuesSource("key");
		}
		static IPatternValuesSource CreateValuesSourceFromSeries(SeriesBase series) {
			Series cloneSeries = null;
			if (series is Series)
				cloneSeries = series as Series;
			else if (series is SeriesBase) {
				cloneSeries = series.CreateSeries(ChartLocalizer.GetString(ChartStringId.SeriesNamePatternDescription));
				cloneSeries.Owner = series.Owner;
				cloneSeries.Points.Clear();
			}
			ISideBySideStackedBarSeriesView stackedGroup = cloneSeries.View as ISideBySideStackedBarSeriesView;
			object seriesGroup = stackedGroup != null ? stackedGroup.StackedGroup : ChartLocalizer.GetString(ChartStringId.StackedGroupPrefix);
			PatternEditorValuesSource valuesSource = new PatternEditorValuesSource(GetPointArgument(cloneSeries), GetPointValue(cloneSeries), cloneSeries.Name, seriesGroup);
			return valuesSource;
		}
		static IPatternValuesSource CreateValuesSourceFromCrosshairOptions(CrosshairOptions options) {
			Chart chart = options.ChartContainer.Chart;
			if (chart.Series.Count > 0)
				return new PatternEditorValuesSource(GetPointArgument(chart.Series[0]), GetPointValue(chart.Series[0]));
			else {
				Series tempSeries = chart.DataContainer.CreateSeries(ChartLocalizer.GetString(ChartStringId.SeriesPrefix));
				return new PatternEditorValuesSource(GetPointArgument(tempSeries), GetPointValue(tempSeries));
			}
		}
		static IPatternValuesSource CreateValuesSourceFromSeriesLabel(SeriesLabelBase label) {
			return CreateValuesSourceFromSeries(label.SeriesBase);
		}
		static object GetPointArgument(Series series) {
			object argument = null;
			switch (series.ActualArgumentScaleType) {
				case ScaleType.Numerical:
					argument = 12.3456789;
					break;
				case ScaleType.DateTime:
					argument = DateTime.Now;
					break;
				case ScaleType.Qualitative:
					if (series.Points.Count > 0)
						argument = series.Points[0].Argument;
					else
						argument = "A";
					break;
				default:
					argument = null;
					break;
			}
			return argument;
		}
		static object GetPointValue(Series series) {
			object value = null;
			switch (series.ValueScaleType) {
				case ScaleType.Numerical:
					value = 12.3456789;
					break;
				case ScaleType.DateTime:
					value = DateTime.Now;
					break;
				default:
					value = null;
					break;
			}
			return value;
		}
		public static string GetDescriptionForPatternPlaceholder(string pattern) {
			switch (pattern) {
				case ToolTipPatternUtils.ArgumentPattern: return ChartLocalizer.GetString(ChartStringId.ArgumentPatternDescription);
				case ToolTipPatternUtils.ValuePattern: return ChartLocalizer.GetString(ChartStringId.ValuePatternDescription);
				case ToolTipPatternUtils.SeriesNamePattern: return ChartLocalizer.GetString(ChartStringId.SeriesNamePatternDescription);
				case ToolTipPatternUtils.StackedGroupPattern: return ChartLocalizer.GetString(ChartStringId.StackedGroupPatternDescription);
				case ToolTipPatternUtils.Value1Pattern: return ChartLocalizer.GetString(ChartStringId.Value1PatternDescription);
				case ToolTipPatternUtils.Value2Pattern: return ChartLocalizer.GetString(ChartStringId.Value2PatternDescription);
				case ToolTipPatternUtils.WeightPattern: return ChartLocalizer.GetString(ChartStringId.WeightPatternDescription);
				case ToolTipPatternUtils.HighValuePattern: return ChartLocalizer.GetString(ChartStringId.HighValuePatternDescription);
				case ToolTipPatternUtils.LowValuePattern: return ChartLocalizer.GetString(ChartStringId.LowValuePatternDescription);
				case ToolTipPatternUtils.OpenValuePattern: return ChartLocalizer.GetString(ChartStringId.OpenValuePatternDescription);
				case ToolTipPatternUtils.CloseValuePattern: return ChartLocalizer.GetString(ChartStringId.CloseValuePatternDescription);
				case ToolTipPatternUtils.PercentValuePattern: return ChartLocalizer.GetString(ChartStringId.PercentValuePatternDescription);
				case ToolTipPatternUtils.PointHintPattern: return ChartLocalizer.GetString(ChartStringId.PointHintPatternDescription);
				case ToolTipPatternUtils.ValueDurationPattern: return ChartLocalizer.GetString(ChartStringId.ValueDurationPatternDescription);
				default: return "Unknown pattern format!";
			}
		}
		public static string GetPatternPlaceholderForDescription(string patternDescription) {
			if (patternDescription == ChartLocalizer.GetString(ChartStringId.ArgumentPatternDescription))
				return ToolTipPatternUtils.ArgumentPattern;
			if (patternDescription == ChartLocalizer.GetString(ChartStringId.ValuePatternDescription))
				return ToolTipPatternUtils.ValuePattern;
			if (patternDescription == ChartLocalizer.GetString(ChartStringId.SeriesNamePatternDescription))
				return ToolTipPatternUtils.SeriesNamePattern;
			if (patternDescription == ChartLocalizer.GetString(ChartStringId.StackedGroupPatternDescription))
				return ToolTipPatternUtils.StackedGroupPattern;
			if (patternDescription == ChartLocalizer.GetString(ChartStringId.Value1PatternDescription))
				return ToolTipPatternUtils.Value1Pattern;
			if (patternDescription == ChartLocalizer.GetString(ChartStringId.Value2PatternDescription))
				return ToolTipPatternUtils.Value2Pattern;
			if (patternDescription == ChartLocalizer.GetString(ChartStringId.WeightPatternDescription))
				return ToolTipPatternUtils.WeightPattern;
			if (patternDescription == ChartLocalizer.GetString(ChartStringId.HighValuePatternDescription))
				return ToolTipPatternUtils.HighValuePattern;
			if (patternDescription == ChartLocalizer.GetString(ChartStringId.LowValuePatternDescription))
				return ToolTipPatternUtils.LowValuePattern;
			if (patternDescription == ChartLocalizer.GetString(ChartStringId.OpenValuePatternDescription))
				return ToolTipPatternUtils.OpenValuePattern;
			if (patternDescription == ChartLocalizer.GetString(ChartStringId.CloseValuePatternDescription))
				return ToolTipPatternUtils.CloseValuePattern;
			if (patternDescription == ChartLocalizer.GetString(ChartStringId.PercentValuePatternDescription))
				return ToolTipPatternUtils.PercentValuePattern;
			if (patternDescription == ChartLocalizer.GetString(ChartStringId.PointHintPatternDescription))
				return ToolTipPatternUtils.PointHintPattern;
			if (patternDescription == ChartLocalizer.GetString(ChartStringId.ValueDurationPatternDescription))
				return ToolTipPatternUtils.ValueDurationPattern;
			return "";
		}
		public static string[] GetAvailablePlaceholdersForRangeColorizerLegendItem(RangeColorizer colorizer) {
			return colorizer != null ? colorizer.GetAvailablePatternPlaceholders() : new string[0];
		}
		public static string[] GetAvailablePlaceholdersForRangeColorizerLegendItem(KeyColorColorizer colorizer) {
			return colorizer != null ? colorizer.GetAvailablePatternPlaceholders() : new string[0];
		}
		public static string[] GetAvailablePlaceholdersForPoint(SeriesBase series) {
			return series.View != null ? series.View.GetAvailablePointPatternPlaceholders() : new string[0];
		}
		public static string[] GetAvailableToolTipPlaceholdersForPoint(SeriesBase series) {
			if (series.View != null) {
				string[] basePatterns = series.View.GetAvailablePointPatternPlaceholders();
				string[] toolTipPaterns = new string[basePatterns.Length + 1];
				basePatterns.CopyTo(toolTipPaterns, 0);
				toolTipPaterns[basePatterns.Length] = ToolTipPatternUtils.PointHintPattern;
				return toolTipPaterns;
			} else
				return new string[0];
		}
		public static string[] GetAvailableToolTipPlaceholdersForSeries(SeriesBase series) {
			return series.View != null ? series.View.GetAvailableSeriesPatternPlaceholders() : new string[0];
		}
		public static string[] GetAvailablePlaceholdersForCrosshairAxisLabel(CrosshairAxisLabelOptions labelOptions) {
			return labelOptions.Axis.GetAvailablePatternPlaceholders();
		}
		public static string[] GetAvailablePlaceholdersForCrosshairGroupHeader(CrosshairOptions crosshairOptions) {
			return crosshairOptions.GetAvailablePatternPlaceholders();
		}
		public static string[] GetAvailablePlaceholdersForLabel(SeriesLabelBase label) {
			return GetAvailablePlaceholdersForPoint(label.SeriesBase);
		}
		public static string[] GetAvailablePlaceholdersForAxisLabel(AxisLabel axisLabel) {
			return axisLabel.Axis.GetAvailablePatternPlaceholders();
		}
		public static IPatternValuesSource CreatePatternValuesSource(object instance) {
			if (instance is SeriesBase)
				return CreateValuesSourceFromSeries((SeriesBase)instance);
			if (instance is CrosshairOptions)
				return CreateValuesSourceFromCrosshairOptions((CrosshairOptions)instance);
			if (instance is CrosshairAxisLabelOptions)
				return CreateValuesSourceFromAxis(((CrosshairAxisLabelOptions)instance).Axis);
			if (instance is SeriesLabelBase)
				return CreateValuesSourceFromSeriesLabel((SeriesLabelBase)instance);
			if (instance is AxisLabel)
				return CreateValuesSourceFromAxis(((AxisLabel)instance).Axis);
			if (instance is RangeColorizer)
				return CreateValuesSourceFromRangeColorizer();
			if (instance is KeyColorColorizer)
				return CreateValuesSourceFromRangeKeyColorColorizer();
			return null;
		}
		public static string GetPatternText(string pattern, IPatternValuesSource valuesSource) {
			IPatternHolder patternHolder = valuesSource as IPatternHolder;
			PatternParser patternParser = new PatternParser(pattern, patternHolder);
			patternParser.SetContext(valuesSource);
			return patternParser.GetText();
		}
		public static string GetActualPattern(ChartElement element) {
			if (element is SeriesBase)
				return ((SeriesBase)element).ActualLegendTextPattern;
			if (element is SeriesLabelBase)
				return ((SeriesLabelBase)element).ActualTextPattern;
			if (element is AxisLabel)
				return ((AxisLabel)element).ActualTextPattern;
			return string.Empty;
		}
	}
}
