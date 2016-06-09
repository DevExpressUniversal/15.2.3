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

using DevExpress.Map;
using DevExpress.Map.Native;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Paint;
using DevExpress.Utils.Text;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraMap.Drawing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
namespace DevExpress.XtraMap.Native {
	public static class Exceptions {
		public static void ThrowObjectLockedException(string objectName, string propName) {
			string s = String.Format("Cannot access the '{0}' property of a locked '{1}' object", propName, objectName);
			throw new ObjectLockedException(s);
		}
		public static void ThrowIncorrectCoordPointException() { 
			string s = "Invalid coordinate point value";
			throw new IncorrectCoordPointException(s);
		}
	}
	public class MapException : Exception {
		public MapException(string message)
			: base(message) {
		}
	}
	public class ObjectLockedException : MapException {
		public ObjectLockedException(string message)
			: base(message) {
		}
	}
	public class IncorrectGeoPointException : MapException {
		public IncorrectGeoPointException(string message)
			: base(message) {
		}
	}
	public class IncorrectCoordPointException : MapException {
		public IncorrectCoordPointException(string message)
			: base(message) {
		}
	}
	[CLSCompliant(false)]
	public static class MapUtils {
		static readonly MapPoint defaultPushpinRenderOrigin = new MapPoint(0.5, 1.0);
		static readonly Point defaultPushpinTextOrigin = new Point(42, 17);
		static Image defaultPushpinImage;
		static Image defaultHighlightedPushpinImage;
		static Image defaultSelectedPushpinImage;
		static Image elementPushpinImage;
		static Graphics measureStringGraphics;
		static StringFormat ellipsisStringFormat;
		static MapCoordinateSystem svgDefaultCoordinateSystem;
		public static MapPoint ElementPushpinRenderOrigin { get { return defaultPushpinRenderOrigin; } }
		public static Point DefaultPushpinTextOrigin { get { return defaultPushpinTextOrigin; } }
		static readonly Dictionary<MiniMapAlignment, ContentAlignment> contentAlignmentDict = new Dictionary<MiniMapAlignment, ContentAlignment>();
		static MapUtils() {
			contentAlignmentDict[MiniMapAlignment.TopLeft] = ContentAlignment.TopLeft;
			contentAlignmentDict[MiniMapAlignment.TopRight] = ContentAlignment.TopRight;
			contentAlignmentDict[MiniMapAlignment.BottomLeft] = ContentAlignment.BottomLeft;
			contentAlignmentDict[MiniMapAlignment.BottomRight] = ContentAlignment.BottomRight;
		}
		[System.Diagnostics.Conditional("DEBUG")]
		public static void DebugAssert(bool condition) {
			if (!condition) {
				string trace = Environment.StackTrace.ToLower();
				if (trace.IndexOf("nunittestrunner") < 0 &&
					trace.IndexOf("nunit.core") < 0 &&
					trace.IndexOf("nunittask") < 0)
					System.Diagnostics.Debug.Assert(condition);
#if DEBUGTEST
#endif
			}
		}
		static Graphics MeasureStringGraphics {
			get {
				if (measureStringGraphics == null)
					measureStringGraphics = Graphics.FromImage(GetDefaultPushpinImage());
				return measureStringGraphics;
			}
		}
		static XPaint textPainter;
		internal static XPaint TextPainter {
			get {
				if (textPainter == null) textPainter = new XPaint();
				return textPainter;
			}
		}
		public static StringFormat EllipsisStringFormat {
			get {
				if (ellipsisStringFormat == null) {
					ellipsisStringFormat = new StringFormat(StringFormat.GenericTypographic);
					ellipsisStringFormat.Alignment = StringAlignment.Center;
					ellipsisStringFormat.LineAlignment = StringAlignment.Center;
					ellipsisStringFormat.Trimming = StringTrimming.EllipsisCharacter;
					ellipsisStringFormat.FormatFlags |= StringFormatFlags.NoWrap;
				}
				return ellipsisStringFormat;
			}
		}
		public static MapCoordinateSystem SvgDefaultCoordinateSystem {
			get {
				if(svgDefaultCoordinateSystem == null)
					svgDefaultCoordinateSystem = new CartesianMapCoordinateSystem();
				return svgDefaultCoordinateSystem;
			}
		}
		static StringAlignment GetVerticalAlignment(ContentAlignment alignment) {
			if(ContentAlignmentUtils.MiddleAligned(alignment))
				return StringAlignment.Center;
			if(ContentAlignmentUtils.BottomAligned(alignment))
				return StringAlignment.Far;
			return StringAlignment.Near;
		}
		static StringAlignment GetHorizontalAlignment(ContentAlignment alignment) {
			if(ContentAlignmentUtils.CenterAligned(alignment))
				return StringAlignment.Center;
			if(ContentAlignmentUtils.RightAligned(alignment))
				return StringAlignment.Far;
			return StringAlignment.Near;
		}
		public static StringFormat GetAlignedStringFormat(ContentAlignment alignment) {
			StringFormat format = new StringFormat(StringFormat.GenericTypographic);
			format.Alignment = GetHorizontalAlignment(alignment);
			format.LineAlignment = GetVerticalAlignment(alignment);
			return format;
		}
		public static void DataBind(VectorItemsLayer layer) {
			layer.DataBind();
		}
		public static void SetBackgroundVisibility(MapPointer pointer, ElementState state) {
			pointer.SkinBackgroundVisibility = state;
		}
		public static RenderMode GetActualRenderMode(InnerMap map) {
			return map.ActualRenderMode;
		}
		public static Image DefaultPushpinImage {
			get {
				if (defaultPushpinImage == null)
					defaultPushpinImage = GetDefaultPushpinImage();
				return defaultPushpinImage;
			}
		}
		public static Image DefaultSelectedPushpinImage {
			get {
				if(defaultSelectedPushpinImage == null)
					defaultSelectedPushpinImage = GetDefaultSelectedPushpinImage();
				return defaultSelectedPushpinImage;
			}
		}
		public static Image DefaultHighlightedPushpinImage {
			get {
				if(defaultHighlightedPushpinImage == null)
					defaultHighlightedPushpinImage = GetDefaultHighlightedPushpinImage();
				return defaultHighlightedPushpinImage;
			}
		}
		public static Image ElementPushpinImage {
			get {
				if (elementPushpinImage == null)
					elementPushpinImage = GetElementPushpinImage();
				return elementPushpinImage;
			}
		}
		public static Size CalcStringPixelSize(string text, Font font) {
			lock (MeasureStringGraphics)
				return CalcStringPixelSize(MeasureStringGraphics, text, font);
		}
		public static Size CalcStringPixelSize(string text, Font font, int width) {
			lock(MeasureStringGraphics)
				return CalcStringPixelSize(MeasureStringGraphics, text, font, width);
		}
		public static Size CalcStringPixelSize(Graphics gr, string text, Font font) {
			return CalcStringPixelSize(gr, text, font, Int32.MaxValue);
		}
		public static Size CalcStringPixelSize(Graphics gr, string text, Font font, int width) {
			return CalcStringPixelSize(gr, text, font, StringFormat.GenericTypographic, width);
		}
		public static Size CalcStringPixelSize(Graphics gr, string text, Font font, StringFormat stringFormat, int width) {
			SizeF textSize = TextPainter.CalcTextSize(gr, text, font, stringFormat, width);
			return new Size((int)Math.Ceiling(textSize.Width), (int)Math.Ceiling(textSize.Height));
		}
		public static DevExpress.Utils.Text.StringInfo CalcHtmlStringInfo(string text, AppearanceObject app) {
			lock (MeasureStringGraphics) {
				Rectangle bounds = new Rectangle(Point.Empty, new Size(int.MaxValue, int.MaxValue));
				return StringPainter.Default.Calculate(MeasureStringGraphics, app, TextOptions.DefaultOptionsMultiLine, text, bounds, TextPainter);
			}
		}
		static Image GetDefaultPushpinImage() {
			return GetResourceImage("MapPushpin.png");
		}
		static Image GetDefaultHighlightedPushpinImage() {
			return GetResourceImage("MapPushpinH.png");
		}
		static Image GetDefaultSelectedPushpinImage() {
			return GetResourceImage("MapPushpinS.png");
		}
		static Image GetElementPushpinImage() {
			return GetResourceImage("Placemark.png");
		}
		public static Image GetResourceImage(string imageName) {
			Assembly asm = Assembly.GetExecutingAssembly();
			string imgPath = "DevExpress.XtraMap.Images.";
			return ResourceImageHelper.CreateImageFromResources(imgPath + imageName, asm);
		}
		internal static bool IsOperationComplete(MapWebLoaderEventArgs e) {
			return !e.Cancelled && e.Error == null;
		}
		public static byte[] GetByteArrayFromResource(string resourceName) {
			return ResourceStreamHelper.GetBytes(resourceName, Assembly.GetExecutingAssembly());
		}
		public static bool IsColorEmpty(Color color) {
			return Color.Empty.ToArgb() == color.ToArgb();
		}
		public static bool CanDrawColor(Color color) {
			return !MapUtils.IsColorEmpty(color) && color != Color.Transparent;
		}
		public static uint ColorToUInt(Color color) {
			return (uint)((color.A << 24) | (color.R << 16) | (color.G << 8) | (color.B << 0));
		}
		public static void DisposeObjectList(IList list) {
			int count = list.Count;
			for (int i = count - 1; i >= 0; i--) {
				DisposeObject(list[i]);
				list.RemoveAt(i);
			}
		}
		public static void DisposeObject(object obj) {
			IDisposable disposable = obj as IDisposable;
			if (disposable != null) disposable.Dispose();
		}
		public static void SetOwner(object element, object owner) {
			IOwnedElement ownedElement = element as IOwnedElement;
			if (ownedElement != null) ownedElement.Owner = owner;
		}
		public static void BeginInvokeAction(ISynchronizeInvoke invoker, Action action) {
			if (invoker != null)
				invoker.BeginInvoke(action, new object[0]);
			else
				action();
		}
		public static void InvokeAction(ISynchronizeInvoke invoker, Action action) {
			if (invoker != null)
				invoker.Invoke(action, new object[0]);
			else
				action();
		}
		public static double ConvertToDouble(object val) {
			double result = 0.0f;
			string s = val as string;
			if (!String.IsNullOrEmpty(s)) {
				if (double.TryParse(s, out result))
					return result;
				return double.Parse(s, CultureInfo.InvariantCulture);
			}
			else
				return Convert.ToDouble(val);
		}
		public static bool IsFileUri(Uri uri) {
			return uri != null && uri.Scheme == "file" && uri.IsAbsoluteUri && !string.IsNullOrEmpty(uri.LocalPath);
		}
		public static TemplateGeometryType ToTemplateGeometryType(MapDotShapeKind shapeKind) {
			if (shapeKind == MapDotShapeKind.Rectangle)
				return TemplateGeometryType.Square;
			return TemplateGeometryType.Circle;
		}
		public static TemplateGeometryType ToTemplateGeometryType(MarkerType type) {
			switch (type) {
				case MarkerType.Square:
					return TemplateGeometryType.Square;
				case MarkerType.Diamond:
					return TemplateGeometryType.Diamond;
				case MarkerType.Triangle:
					return TemplateGeometryType.Triangle;
				case MarkerType.InvertedTriangle:
					return TemplateGeometryType.InvertedTriangle;
				case MarkerType.Circle:
					return TemplateGeometryType.Circle;
				case MarkerType.Plus:
					return TemplateGeometryType.Plus;
				case MarkerType.Cross:
					return TemplateGeometryType.Cross;
				case MarkerType.Star5:
					return TemplateGeometryType.Star5;
				case MarkerType.Star6:
					return TemplateGeometryType.Star6;
				case MarkerType.Star8:
					return TemplateGeometryType.Star8;
				case MarkerType.Pentagon:
					return TemplateGeometryType.Pentagon;
				case MarkerType.Hexagon:
					return TemplateGeometryType.Hexagon;
			}
			return TemplateGeometryType.Circle;
		}
		public static bool IsEmptyStream(Stream stream) {
			return stream == null || Object.Equals(stream, Stream.Null) || stream.Length == 0;
		}
		public static Rectangle GetContentBoundsWithoutBorders(IRenderStyleProvider styleProvider, Rectangle clientBounds) {
			if (RectUtils.IsBoundsEmpty(clientBounds))
				return clientBounds;
			BorderPainter painter = GetBorderPainter(styleProvider);
			using (GraphicsCache cache = new GraphicsCache(Graphics.FromHwnd(IntPtr.Zero))) {
				BorderObjectInfoArgs args = new BorderObjectInfoArgs(cache, clientBounds, null, ObjectState.Normal);
				return painter.GetObjectClientRectangle(args);
			}
		}
		public static BorderPainter GetBorderPainter(IRenderStyleProvider styleProvider) {
			BorderStyles borderStyles = styleProvider.BorderStyle;
			if (styleProvider.IsSkinActive && borderStyles != BorderStyles.NoBorder && styleProvider.BorderElement != null)
				return new MapSkinBorderPainter(styleProvider.SkinProvider) { BorderElement = styleProvider.BorderElement };
			BorderStyles styles = (borderStyles != BorderStyles.Default) ? borderStyles : BorderStyles.Flat;
			return BorderHelper.GetPainter(styles, null, null);
		}
		public static bool CheckValidPen(Pen pen) {
			return pen.Width > 0.0f && pen.Color != Color.Empty && pen.Color != Color.Transparent;
		}
		public static GeoPoint CalculateRegionCenter(GeoPoint leftTop, GeoPoint rightBottom) {
			return new GeoPoint((leftTop.Latitude + rightBottom.Latitude) / 2, (leftTop.Longitude + rightBottom.Longitude) / 2);
		}
		public static Assembly XtraMapAssembly { get { return typeof(MapControl).Assembly; } }
		public static Assembly MapCoreAssembly { get { return typeof(CoordPoint).Assembly; } }
		public static Type[] GetTypeDescendants(Assembly assembly, Type ancestorType, List<Type> ignoredTypes) {
			Type[] types = assembly.GetTypes();
			List<Type> result = new List<Type>();
			foreach (Type item in types) {
				object[] obsoleteAttributes = item.GetCustomAttributes(typeof(ObsoleteAttribute), true);
				bool itTestClass = item.Namespace != null && item.Namespace.ToLower().Contains(".test");
				if (ancestorType.IsAssignableFrom(item) && item.IsClass && !item.IsAbstract && !ignoredTypes.Contains(item) && obsoleteAttributes.Length == 0 && !itTestClass)
					result.Add(item);
			}
			return result.ToArray();
		}
		#region Vector math utils
		public static MapUnit[] RotateVectors(double rotateAngle, MapUnit anchor, MapUnit[] points) {
			MapUnit[] result = new MapUnit[points.Length];
			for (int i = 0; i < points.Length; i++) {
				double aRad = MathUtils.Degree2Radian(rotateAngle);
				double x = anchor.X + (points[i].X - anchor.X) * Math.Cos(aRad) - (points[i].Y - anchor.Y) * Math.Sin(aRad);
				double y = anchor.Y + (points[i].Y - anchor.Y) * Math.Cos(aRad) + (points[i].X - anchor.X) * Math.Sin(aRad);
				result[i] = new MapUnit(x, y);
			}
			return result;
		}
		public static MapUnit[] CalcPolygon(double startAngle, int verticesCount, MapUnit center, double radius) {
			double angleStep = Math.PI * 2.0 / verticesCount;
			MapUnit[] points = new MapUnit[verticesCount];
			for (int i = 0; i < verticesCount; i++)
				points[i] = CalcCirclePoint(center, radius, startAngle + angleStep * i);
			return points;
		}
		public static MapUnit CalcCirclePoint(MapUnit center, double radius, double angle) {
			double x = radius * Math.Cos(angle);
			double y = radius * Math.Sin(angle);
			return new MapUnit(center.X + x, center.Y + y);
		}
		#endregion
		public static IList<double> SortDoubleCollection(DoubleCollection collection) {
			if (collection == null) return new List<double>();
			List<double> list = new List<double>(collection);
			list.Sort(Comparer<double>.Default);
			return list;
		}
		public static int BinarySearch(IList<double> collection, double value) {
			if (collection == null) return -1;
			List<double> list = new List<double>(collection);
			return list.BinarySearch(value, Comparer<double>.Default);
		}
		public static int GetActualNavigationPanelHeight(InnerMap map) {
			return map != null && map.OperationHelper.CanShowNavigationPanel() ? map.NavigationPanelOptions.Height : 0;
		}
		public static PointF MapUnitToPointF(MapUnit unit) {
			return new PointF(Convert.ToSingle(unit.X), Convert.ToSingle(unit.Y));
		}
		public static PointF[] MapUnitsToPointsF(MapUnit[] units) {
			PointF[] result = new PointF[units.Count()];
			for (int i = 0; i < units.Count(); i++)
				result[i] = MapUnitToPointF(units[i]);
			return result;
		}
		public static PointF[] ConvertToPoints(MapPoint[] points) {
			int count = points.Length;
			List<PointF> result = new List<PointF>(count);
			if(count > 0) {
				PointF prev = points[0].ToPointF();
				result.Add(prev);
				for(int i = 1; i < count; i++) {
					PointF current = points[i].ToPointF();
					if(prev == current)
						continue;
					result.Add(current);
					prev = current;
				}
			}
			if(result.Count == 1)
				result.Add(result[0]); 
			return result.ToArray();
		}
		internal static Thread CreateThread(ThreadStart threadStart, string name) {
			Thread thread = new Thread(threadStart);
			thread.Name = name;
			thread.CurrentCulture = Thread.CurrentThread.CurrentCulture;
			thread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
			thread.IsBackground = true;
			return thread;
		}
		public static bool IsValidSize(Size size) {
			return !size.IsEmpty && size.Width > 0 && size.Height > 0;
		}
		static bool IsValidCoordinate(double coord) {
			return !Double.IsInfinity(coord) && !Double.IsNaN(coord);
		}
		public static bool IsValidPoint(CoordPoint point) {
			return IsValidCoordinate(point.GetX()) && IsValidCoordinate(point.GetY());
		}
		public static Bitmap TryCreateBitmap(int width, int height) {
			Bitmap bmp;
			try {
				bmp = new Bitmap(width, height);
			} catch {
				return null;
			}
			return bmp;
		}
		internal static ContentAlignment ConvertToContentAlignment(MiniMapAlignment alignment) {
			return contentAlignmentDict[alignment];
		}
		public static void UpdateImageContainer(IImageContainer imageContainer, object imageList) {
			if(imageContainer != null)
				imageContainer.UpdateImage(imageList);
		}
		public static double CalculateDistance(Point point1, Point point2) {
			double dx = point1.X - point2.X;
			double dy = point1.Y - point2.Y;
			return Math.Sqrt(dx * dx + dy * dy);
		}
		public static bool CircleHitTest(Rectangle circleBoundingRect, Point point, int circleThickness) {
			double radius = circleBoundingRect.Width / 2 + circleThickness;
			double distance = CalculateDistance(point, RectUtils.GetCenter(circleBoundingRect));
			return distance <= radius;
		}
		public static CoordPointCollection CreateOwnedPoints(object owner) {
			CoordPointCollection result = new CoordPointCollection();
			((IOwnedElement)result).Owner = owner;
			return result;
		}
	}
	public static class ImageSafeAccess {
		public static Size GetSize(Image image) {
			if (image != null) {
				lock (image) {
					return image.Size;
				}
			}
			return Size.Empty;
		}
		public static void Draw(Graphics gr, Image image, Rectangle bounds) {
			if (image != null) {
				lock (image) {
					gr.DrawImage(image, bounds);
				}
			}
		}
		public static void Draw(Graphics gr, Image image, RectangleF bounds) {
			if (image != null) {
				lock (image) {
					gr.DrawImage(image, bounds);
				}
			}
		}
		public static void Draw(Graphics gr, Image image, RectangleF bounds, ImageAttributes attributes) {
			if(image != null) {
				PointF[] tileBounds = new PointF[] { new PointF(bounds.Left, bounds.Top), new PointF(bounds.Right - 1, bounds.Top), new PointF(bounds.Left, bounds.Bottom - 1) };
				RectangleF imageBounds = new RectangleF(bounds.Left, bounds.Top, bounds.Width - 2, bounds.Height - 2);
				lock(image) {
					gr.DrawImage(image, tileBounds, imageBounds, GraphicsUnit.Pixel, attributes);
				}
			}
		}
		public static void DrawUnscaled(Graphics gr, Image image, int x, int y) {
			gr.DrawImageUnscaled(image, x, y);
		}
		public static void Draw(Graphics gr, Image image, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit) {
			if (image != null) {
				lock (image) {
					gr.DrawImage(image, destRect, srcRect, srcUnit);
				}
			}
		}
		public static void Draw(Graphics gr, Image image, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes attributes) {
			if(image != null) {
				PointF[] tileBounds = new PointF[] { new PointF(destRect.Left, destRect.Top), new PointF(destRect.Right - 1, destRect.Top), new PointF(destRect.Left, destRect.Bottom - 1) };
				RectangleF imageBounds = new RectangleF(srcRect.Left, srcRect.Top, srcRect.Width, srcRect.Height);
				lock(image) {
					gr.DrawImage(image, tileBounds, imageBounds, srcUnit, attributes);
				}
			}
		}
	}
	public class MapPathSegmentHelper {
		readonly PathSegmentHelperCore segmentHelperCore;
		public MapPathSegmentHelper(MapPath path) {
			this.segmentHelperCore = new PathSegmentHelperCore(path, path.UnitConverter.PointFactory);
		}
		internal void UpdatePointFactory(CoordObjectFactory pointFactory) {
			segmentHelperCore.UpdatePointFactory(pointFactory);
		}
		public Color GetLargestSegmentTextColor() {
			MapPathSegment segment = (MapPathSegment)segmentHelperCore.MaxSegment;
			return segment != null ? segment.GetActualTextColor() : Color.Empty;
		}
		public Color GetLargestSegmentTextGlowColor() {
			MapPathSegment segment = (MapPathSegment)segmentHelperCore.MaxSegment;
			return segment != null ? segment.GetActualTextGlowColor() : Color.Empty;
		}
		public CoordPoint GetMaxSegmentCenter() {
			return segmentHelperCore.GetMaxSegmentCenter();
		}
		public MapRect GetMaxSegmentBounds() {
			MapPathSegment segment = (MapPathSegment)segmentHelperCore.MaxSegment;
			return segment != null ? segment.Bounds : MapRect.Empty;
		}
		public void Reset() {
			segmentHelperCore.Reset();
		}
	}
	public static class CoordPointHelper {
		public static CoordBounds SelectItemBounds(MapItem item) {
			return SelectItemsBounds(new List<MapItem>() { item });
		}
		public static CoordBounds SelectLayersItemsBounds(IEnumerable<LayerBase> layers) {
			CoordBounds bounds = CoordBounds.Empty;
			foreach(LayerBase layer in layers) {
				MapItemsLayerBase itemsLayer = layer as MapItemsLayerBase;
				if(itemsLayer != null && itemsLayer.CheckVisibility()) {
					CoordBounds layerItemsBounds = SelectItemsBounds(itemsLayer.DataItems);
					bounds = CoordBounds.Union(bounds, layerItemsBounds);
				}
			}
			return bounds;
		}
		public static CoordBounds SelectItemsBounds(IEnumerable<MapItem> items) {
			IList<CoordPoint> points = items.SelectMany<MapItem, CoordPoint>(item => { return item.GetItemPoints(); }).ToList();
			if(points.Count > 0) {
				double y1 = points.Max(p => p.GetY());
				double y2 = points.Min(p => p.GetY());
				double x1 = points.Min(p => p.GetX());
				double x2 = points.Max(p => p.GetX());
				return new CoordBounds(x1, y1, x2, y2);
			}
			return CoordBounds.Empty;
		}
	}
	public static class OrthodromeHelper {
		public static IList<IList<CoordPoint>> CalculateLine(CoordPoint point1, CoordPoint point2) {
			OrthodromeCalculator calculator = new OrthodromeCalculator(GeoPointFactory.Instance);
			return calculator.CalculateLine(point1, point2);
		}
		public static IList<IList<CoordPoint>> CalculateLine(IList<CoordPoint> points) {
			OrthodromeCalculator calculator = new OrthodromeCalculator(GeoPointFactory.Instance);
			List<List<CoordPoint>> result = new List<List<CoordPoint>>();
			result.Add(new List<CoordPoint>());
			for(int i = 0; i < points.Count - 1; i++) {
				IList<IList<CoordPoint>> segmentPoints = calculator.CalculateLine(points[i], points[i + 1]);
				result[result.Count - 1].AddRange(segmentPoints[0]);
				if(segmentPoints.Count > 1) {
					result.Add(new List<CoordPoint>());
					result[result.Count - 1].AddRange(segmentPoints[1]);
				}
			}
			return result.Cast<IList<CoordPoint>>().ToList();
		}
	}
}
