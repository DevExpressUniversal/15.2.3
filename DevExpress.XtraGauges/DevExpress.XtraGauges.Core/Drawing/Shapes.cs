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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Localization;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
namespace DevExpress.XtraGauges.Core.Drawing {
	public class DrawImageHelper {
		public static Rectangle GetImageBounds(Rectangle bounds, Size imageSize, ImageLayoutMode imageLayout) {
			Rectangle res = bounds;
			if(imageLayout == ImageLayoutMode.Stretch)
				return bounds;
			if(imageLayout == ImageLayoutMode.ZoomInside || imageLayout == ImageLayoutMode.ZoomOutside) {
				float scaleX = ((float)bounds.Width) / imageSize.Width;
				float scaleY = ((float)bounds.Height) / imageSize.Height;
				float currScale;
				if(imageLayout == ImageLayoutMode.ZoomInside)
					currScale = scaleX > scaleY ? scaleY : scaleX;
				else
					currScale = scaleX > scaleY ? scaleX : scaleY;
				res.Width = (int)(imageSize.Width * currScale);
				res.Height = (int)(imageSize.Height * currScale);
				return CenterRectangle(bounds, res);
			}
			res.Width = imageSize.Width;
			res.Height = imageSize.Height;
			if(imageLayout == ImageLayoutMode.TopLeft) {
				return res;
			}
			if(imageLayout == ImageLayoutMode.TopCenter || imageLayout == ImageLayoutMode.MiddleCenter || imageLayout == ImageLayoutMode.BottomCenter) {
				res.X = AlignCenter(bounds.X, bounds.Width, res.Width);
			}
			if(imageLayout == ImageLayoutMode.TopRight || imageLayout == ImageLayoutMode.MiddleRight || imageLayout == ImageLayoutMode.BottomRight) {
				res.X = AlignRightBottom(bounds.X, bounds.Width, res.Width);
			}
			if(imageLayout == ImageLayoutMode.MiddleCenter || imageLayout == ImageLayoutMode.MiddleLeft || imageLayout == ImageLayoutMode.MiddleRight) {
				res.Y = AlignCenter(bounds.Y, bounds.Height, res.Height);
			}
			if(imageLayout == ImageLayoutMode.BottomLeft || imageLayout == ImageLayoutMode.BottomCenter || imageLayout == ImageLayoutMode.BottomRight) {
				res.Y = AlignRightBottom(bounds.Y, bounds.Height, res.Height);
			}
			if(imageLayout == ImageLayoutMode.StretchHorizontal) {
				res.Width = bounds.Width;
				res.X = bounds.X;
				res.Y = AlignCenter(bounds.Y, bounds.Height, res.Height);
			}
			if(imageLayout == ImageLayoutMode.StretchVertical) {
				res.Height = bounds.Height;
				res.Y = bounds.Y;
				res.X = AlignCenter(bounds.X, bounds.Width, res.Width);
			}
			return res;
		}
		static int AlignCenter(int boundStartValue, int boundValue, int val) {
			return boundStartValue + (boundValue - val) / 2;
		}
		static int AlignRightBottom(int boundStartValue, int boundValue, int val) {
			return boundStartValue + (boundValue - val);
		}
		static Rectangle CenterRectangle(Rectangle bounds, Rectangle rect) {
			Rectangle res = rect;
			res.X = bounds.X + (bounds.Width - rect.Width) / 2;
			res.Y = bounds.Y + (bounds.Height - rect.Height) / 2;
			return res;
		}
	}
	public enum ImageLayoutMode {
		TopLeft = 0,
		TopCenter = 1,
		TopRight = 2,
		MiddleLeft = 3,
		MiddleCenter = 4,
		MiddleRight = 5,
		BottomLeft = 6,
		BottomCenter = 7,
		BottomRight = 8,
		Stretch = 9,
		ZoomInside = 10,
		ZoomOutside = 11,
		StretchHorizontal = 12,
		StretchVertical = 13,
		Default = 14,
	}
	public class ShapePoint {
		PointF pointCore;
		PathPointType pointTypeCore;
		public ShapePoint(PointF point, PathPointType pointType) {
			this.pointCore = point;
			this.pointTypeCore = pointType;
		}
		public PointF Point {
			get { return pointCore; }
		}
		public void SetPoint(PointF point) {
			pointCore = point;
		}
		public PathPointType PointType {
			get { return pointTypeCore; }
			set { pointTypeCore = value; }
		}
	}
	public class BaseShapeCollection : BaseReadOnlyDictionary<BaseShape>, IEnumerable<BaseShape> {
		IEnumerator<BaseShape> enumerator;
		public BaseShapeCollection() : base() { }
		public void Add(BaseShape shape) {
			if(String.IsNullOrEmpty(shape.Name) || Collection.ContainsKey(shape.Name)) return;
			Collection[shape.Name] = shape;
			enumerator = null;
		}
		IEnumerator<BaseShape> IEnumerable<BaseShape>.GetEnumerator() {
			if(enumerator == null)
				enumerator = Collection.Values.GetEnumerator();
			else {
				try {
					enumerator.Reset();
				}
				catch(InvalidOperationException) {
					enumerator = Collection.Values.GetEnumerator();
				}
			}
			return enumerator;
		}
		public void AddRange(BaseShape[] shapes) {
			foreach(BaseShape s in shapes) Add(s);
		}
		public void Remove(BaseShape shape) {
			Collection.Remove(shape.Name);
			enumerator = null;
		}
	}
	[TypeConverter(typeof(ReadonlyShapeObjectTypeConverter))]
	public abstract class BaseShape : BaseObjectEx, ISupportVisitor<BaseShape>, ISupportColorShading, ISupportLockUpdate, ISupportAcceptOrder {
		protected const double DegreeToRadian = Math.PI / 180d;
		[ThreadStatic]
		static BaseShape emptyShapeCore;
		[ThreadStatic]
		static GraphicsPath emptyPathCore;
		protected static GraphicsPath EmptyPath {
			get {
				if(emptyPathCore == null)
					emptyPathCore = new GraphicsPath();
				return emptyPathCore;
			}
		}
		internal static bool IsEmptyPath(GraphicsPath path) {
			return object.ReferenceEquals(path, emptyPathCore);
		}
		internal static GraphicsPath SaveEmptyPathForFinalizerThread() {
			return EmptyPath;
		}
		internal static void EnsureEmptyPathFromFinalizerThread(GraphicsPath emptyPath) {
			if(emptyPathCore == null) emptyPathCore = emptyPath;
		}
		public static BaseShape Empty {
			get {
				if(emptyShapeCore == null)
					emptyShapeCore = new EmptyShape();
				return emptyShapeCore;
			}
		}
		class EmptyShape : BaseShape {
			protected override void OnCreate() {
				base.OnCreate();
				this.nameCore = "EmptyShape";
			}
			protected override GraphicsPath OnCreatePath() {
				return BaseShape.EmptyPath;
			}
			public override event EventHandler Changed {
				add {}
				remove {}
			}
			protected override void CopyToCore(BaseObject clone) {
			}
			protected override void OnCollectPoints() {
			}
			protected override BaseObject CloneCore() {
				return this;
			}
			protected override void AssignCore(BaseShape shape) {
			}
			public override void Accept(VisitDelegate<BaseShape> visit) {
			}
			public override void Accept(IVisitor<BaseShape> visitor) {
			}
			protected override void OnShapeChangedCore() {
			}
			public override void Render(Graphics g, IStringPainter stringPainter) {
			}
		}
		int acceptOrderCore;
		string nameCore;
		Matrix transformCore;
		RectangleF2D boundsCore;
		RectangleF textBoundsCore;
		RectangleF boundBoxCore;
		BaseShapeAppearance appearanceCore;
		BaseTextAppearance appearanceTextCore;
		List<ShapePoint> shapePointsCore;
		GraphicsPath cachedPath;
		ShadingFlags shadingFlagsCore;
		bool allowHtmlStringCore;
		protected BaseShape()
			: base() {
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.allowHtmlStringCore = false;
			this.shadingFlagsCore = ShadingFlags.Normal;
			this.appearanceCore = new BaseShapeAppearance();
			if(!IsEmpty)
				Appearance.Changed += OnAppearanceChanged;
			this.boundsCore = RectangleF2D.Empty;
			this.textBoundsCore = RectangleF.Empty;
			this.boundBoxCore = RectangleF.Empty;
			this.cachedPath = null;
		}
		void OnAppearanceChanged(object sender, EventArgs e) {
			if(IsUpdateLocked || IsDisposing) return;
			OnPropertiesChanged();
		}
		protected override void OnDispose() {
			if(appearanceCore != null) {
				Appearance.Changed -= OnAppearanceChanged;
				Appearance.Dispose();
				appearanceCore = null;
			}
			if(appearanceTextCore != null) {
				AppearanceText.Changed -= OnAppearanceChanged;
				AppearanceText.Dispose();
				appearanceTextCore = null;
			}
			if(shapePointsCore != null) {
				shapePointsCore = null;
			}
			Ref.Dispose(ref transformCore);
			DestroyCachedPath();
			base.OnDispose();
		}
		public void Accept(IColorShader shader) {
			BeginUpdate();
			Accept(
					delegate(BaseShape shape) {
						if((ShadingFlags & ShadingFlags.NoShading) != 0) return;
						shape.Appearance.Accept(shader);
						shape.AppearanceText.Accept(shader);
					}
				);
			EndUpdate();
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int AcceptOrder {
			get { return acceptOrderCore; }
			set { acceptOrderCore = value; }
		}
		public new BaseShape Clone() {
			return (BaseShape)base.Clone();
		}
		protected override void CopyToCore(BaseObject clone) {
			BaseShape clonedShape = clone as BaseShape;
			if(clonedShape != null) {
				clonedShape.BeginUpdate();
				clonedShape.transformCore = transformCore != null ? Transform.Clone() : null;
				clonedShape.Name = this.Name;
				clonedShape.Bounds = this.Bounds;
				clonedShape.ShadingFlags = this.ShadingFlags;
				clonedShape.AllowHtmlString = this.AllowHtmlString;
				clonedShape.Appearance.AssignInternal(this.Appearance);
				if(appearanceTextCore != null) clonedShape.AppearanceText.AssignInternal(this.AppearanceText);
				clonedShape.EndUpdate();
			}
		}
		[DefaultValue(false)]
		public bool AllowHtmlString {
			get { return allowHtmlStringCore; }
			set {
				if(AllowHtmlString == value) return;
				allowHtmlStringCore = value;
				OnPropertiesChanged();
			}
		}
		public virtual string DisplayText {
			get { return String.Empty; }
		}
		protected void DestroyCachedPath() {
			if(!IsEmptyPath(cachedPath))
				Ref.Dispose(ref cachedPath);
			cachedPath = null;
		}
		[Browsable(false)]
		public bool IsEmpty {
			get { return this is EmptyShape; }
		}
		[Browsable(false)]
		public string Name {
			get { return nameCore; }
			set { nameCore = value; }
		}
		internal bool ShouldSerializeTransform() {
			return transformCore != null && !Transform.IsIdentity;
		}
		[DefaultValue(ShadingFlags.Normal)]
		public ShadingFlags ShadingFlags {
			get { return shadingFlagsCore; }
			set {
				if(ShadingFlags == value) return;
				shadingFlagsCore = value;
				OnPropertiesChanged();
			}
		}
		[Browsable(false)]
		public Matrix Transform {
			get {
				if(transformCore == null)
					transformCore = new Matrix();
				return transformCore;
			}
			set {
				if(transformCore != null)
					transformCore.Dispose();
				if(value.IsIdentity)
					transformCore = null;
				else
					transformCore = value;
				DestroyCachedPath();
				OnPropertiesChanged();
			}
		}
		public virtual RectangleF2D Bounds {
			get { return boundsCore; }
			set {
				if(Bounds == value) return;
				boundsCore = value;
				OnPropertiesChanged();
			}
		}
		[Browsable(false)]
		public virtual RectangleF TextBounds {
			get { return textBoundsCore; }
		}
		[Browsable(false)]
		public RectangleF BoundingBox {
			get {
				if(boundBoxCore == RectangleF.Empty)
					boundBoxCore = CalcBounds();
				return boundBoxCore;
			}
		}
		protected internal List<ShapePoint> ShapePoints {
			get {
				if(shapePointsCore == null) {
					shapePointsCore = new List<ShapePoint>();
					BeginUpdate();
					OnCollectPoints();
					CancelUpdate();
				}
				return shapePointsCore;
			}
		}
		protected override sealed void OnUnlockUpdateCore() {
			OnShapeChanged();
		}
		protected void Add(ShapePoint point) {
			ShapePoints.Add(point);
			OnShapeChanged();
		}
		protected void AddRange(ShapePoint[] points) {
			ShapePoints.AddRange(points);
			OnShapeChanged();
		}
		internal bool ShouldSerializeAppearance() {
			return Appearance.ShouldSerialize();
		}
		internal bool ShouldSerializeAppearanceText() {
			return appearanceTextCore != null && AppearanceText.ShouldSerialize();
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public BaseShapeAppearance Appearance {
			get { return appearanceCore; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public BaseTextAppearance AppearanceText {
			get {
				if(appearanceTextCore == null) {
					appearanceTextCore = new BaseTextAppearance();
					if(!IsEmpty)
						appearanceTextCore.Changed += OnAppearanceChanged;
				}
				return appearanceTextCore;
			}
		}
		protected internal void ForceShapeChanged() {
			OnShapeChangedCore();
		}
		protected void OnShapeChanged() {
			if(IsDisposing || IsUpdateLocked) return;
			OnShapeChangedCore();
		}
		protected virtual void OnShapeChangedCore() {
			DestroyCachedPath();
			this.boundBoxCore = RectangleF.Empty;
			this.textBoundsCore = CalcTextBounds();
			RaiseChanged(EventArgs.Empty);
		}
		protected void OnPropertiesChanged() {
			BeginUpdate();
			shapePointsCore = null;
			EndUpdate();
		}
		protected abstract void OnCollectPoints();
		protected virtual void AssignCore(BaseShape shape) {
			this.Appearance.Assign(shape.Appearance);
		}
		public void Assign(BaseShape shape) {
			if(shape != null) {
				BeginUpdate();
				AssignCore(shape);
				EndUpdate();
			}
		}
		protected internal void AssignInternal(BaseShape shape) {
			if(shape != null) {
				BeginUpdate();
				AssignCore(shape);
				CancelUpdate();
			}
		}
		[Browsable(false)]
		public GraphicsPath Path {
			get {
				if(cachedPath == null) {
					cachedPath = OnCreatePath();
					if(transformCore != null && !Transform.IsIdentity) {
						if(!IsEmptyPath(cachedPath))
							cachedPath.Transform(Transform);
					}
				}
				return cachedPath;
			}
		}
		protected abstract GraphicsPath OnCreatePath();
		public bool HitTest(Point pt) {
			return Path.IsVisible(pt);
		}
		protected virtual RectangleF CalcBounds() {
			return ShapeHelper.GetShapeBounds(this);
		}
		protected virtual RectangleF CalcTextBounds() {
			return RectangleF.Empty;
		}
		public virtual void Accept(IVisitor<BaseShape> visitor) {
			if(visitor != null) visitor.Visit(this);
		}
		public virtual void Accept(VisitDelegate<BaseShape> visit) {
			if(visit != null) visit(this);
		}
		public virtual void Render(Graphics g, IStringPainter stringPainter) {
			RenderCore(g);
		}
		protected void RenderCore(Graphics g) {
			Appearance.BeginUpdate();
			if(!Appearance.ContentBrush.IsEmpty) {
				g.FillPath(Appearance.ContentBrush.GetBrush(Bounds), Path);
			}
			if(!Appearance.BorderBrush.IsEmpty)
				g.DrawPath(Appearance.BorderBrush.GetPen(Appearance.BorderWidth), Path);
			Appearance.CancelUpdate();
		}
	}
	public class PathShape : BaseShape {
		ShapePoint[] pathPoints;
		static ShapePoint[] EmptyPoints = new ShapePoint[0];
		public PathShape()
			: base() {
		}
		public PathShape(ShapePoint[] points)
			: base() {
			Points = points;
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.pathPoints = EmptyPoints;
		}
		public ShapePoint[] Points {
			get { return pathPoints; }
			set {
				this.pathPoints = value;
				OnPropertiesChanged();
			}
		}
		protected override void OnCollectPoints() {
			AddRange(Points);
		}
		protected override GraphicsPath OnCreatePath() {
			PointF[] points = ShapeHelper.GetPoints(this, false);
			if(points.Length == 0)
				return EmptyPath;
			GraphicsPath path = new GraphicsPath(points, ShapeHelper.GetPointTypes(this));
			if(path.PointCount != points.Length) throw new Exception(GaugesCoreLocalizer.GetString(GaugesCoreStringId.MsgPathCreationError));
			return path;
		}
		protected override BaseObject CloneCore() {
			return new PathShape();
		}
		protected override void CopyToCore(BaseObject clone) {
			base.CopyToCore(clone);
			PathShape clonedShape = clone as PathShape;
			if(clonedShape != null) {
				clonedShape.BeginUpdate();
				clonedShape.Points = Points == EmptyPoints ? EmptyPoints : (ShapePoint[])this.Points.Clone();
				clonedShape.EndUpdate();
			}
		}
	}
	public class PolygonShape : BaseShape {
		PointF[] polygonPoints;
		public PolygonShape() : this(new PointF[0]) { }
		public PolygonShape(PointF[] points)
			: base() {
			Points = points;
		}
		protected override void OnCollectPoints() {
			if(Points.Length >= 2) {
				for(int i = 0; i < Points.Length; i++) {
					PathPointType type = PathPointType.Line;
					if(i == Points.Length - 1) type = PathPointType.Line | PathPointType.CloseSubpath;
					Add(new ShapePoint(Points[i], type));
				}
			}
		}
		public PointF[] Points {
			get { return polygonPoints; }
			set {
				this.polygonPoints = value;
				OnPropertiesChanged();
			}
		}
		protected override GraphicsPath OnCreatePath() {
			if(Points.Length == 0)
				return EmptyPath;
			GraphicsPath path = new GraphicsPath();
			path.AddPolygon(Points);
			return path;
		}
		protected override BaseObject CloneCore() {
			return new PolygonShape();
		}
		protected override void CopyToCore(BaseObject clone) {
			base.CopyToCore(clone);
			PolygonShape clonedShape = clone as PolygonShape;
			if(clonedShape != null) {
				clonedShape.BeginUpdate();
				clonedShape.Points = (PointF[])this.Points.Clone();
				clonedShape.EndUpdate();
			}
		}
	}
	public class PolylineShape : PolygonShape {
		public PolylineShape() : base() { }
		public PolylineShape(PointF[] points)
			: base() {
			Points = points;
		}
		protected override GraphicsPath OnCreatePath() {
			GraphicsPath path = new GraphicsPath();
			if(Points.Length > 0) path.AddLines(Points);
			return path;
		}
		protected override BaseObject CloneCore() {
			return new PolylineShape();
		}
		protected override void OnCollectPoints() {
			if(Points.Length >= 2) {
				Add(new ShapePoint(Points[0], PathPointType.Start));
				for(int i = 1; i < Points.Length; i++) {
					Add(new ShapePoint(Points[i], PathPointType.Line));
				}
			}
		}
	}
	public class BoxShape : BaseShape {
		RectangleF2D boxCore;
		public BoxShape()
			: base() {
		}
		public BoxShape(RectangleF2D box)
			: base() {
			Box = box;
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.boxCore = RectangleF2D.Empty;
		}
		protected override void OnCollectPoints() {
			AddRange(new ShapePoint[]{
					new ShapePoint(new PointF(Box.Left,Box.Top), PathPointType.Start),
					new ShapePoint(new PointF(Box.Left + Box.Width,Box.Top), PathPointType.Line),
					new ShapePoint(new PointF(Box.Left + Box.Width,Box.Top + Box.Height), PathPointType.Line),
					new ShapePoint(new PointF(Box.Left,Box.Top + Box.Height), PathPointType.Line | PathPointType.CloseSubpath)
				}
			);
		}
		public RectangleF2D Box {
			get { return boxCore; }
			set {
				if(Box == value) return;
				this.boxCore = value;
				OnPropertiesChanged();
			}
		}
		protected override GraphicsPath OnCreatePath() {
			GraphicsPath path = new GraphicsPath();
			if(!Box.IsEmpty) path.AddRectangle(Box);
			return path;
		}
		protected override BaseObject CloneCore() {
			return new BoxShape();
		}
		protected override void CopyToCore(BaseObject clone) {
			base.CopyToCore(clone);
			BoxShape clonedShape = clone as BoxShape;
			if(clonedShape != null) {
				clonedShape.BeginUpdate();
				clonedShape.Box = this.Box;
				clonedShape.EndUpdate();
			}
		}
	}
	public class EllipseShape : BoxShape {
		public EllipseShape() : this(RectangleF2D.Empty) { }
		public EllipseShape(RectangleF2D box)
			: base() {
			Box = box;
		}
		protected override GraphicsPath OnCreatePath() {
			if(Box.IsEmpty)
				return EmptyPath;
			GraphicsPath path = new GraphicsPath();
			path.AddEllipse(Box);
			return path;
		}
		protected override void OnCollectPoints() {
			float start = 0;
			float d_range = 360f / 18f;
			for(int i = 0; i < 18; i++) {
				float x = Box.Left + Box.Width / 2f;
				float y = Box.Top + Box.Height / 2f;
				double angle = (start + d_range * i) * DegreeToRadian;
				float dx = (Box.Width / 2f) * (float)Math.Cos(angle);
				float dy = (Box.Height / 2f) * (float)Math.Sin(angle);
				PathPointType type = PathPointType.Bezier;
				if(i == 0) type = PathPointType.Start;
				if(i == 17) type = PathPointType.Bezier | PathPointType.CloseSubpath;
				Add(new ShapePoint(new PointF(x + dx, y + dy), type));
			}
		}
		protected override BaseObject CloneCore() {
			return new EllipseShape();
		}
	}
	public class ArcShape : BoxShape {
		float startAngleCore;
		float endAngleCore;
		float minimumAngleCore;
		float maximumAngleCore;
		public ArcShape() : base() { }
		public ArcShape(RectangleF2D box, float startAngle, float endAngle)
			: base() {
			BeginUpdate();
			Box = box;
			StartAngle = startAngle;
			EndAngle = endAngle;
			EndUpdate();
		}
		protected override void OnCreate() {
			base.OnCreate();
			startAngleCore = 0f;
			endAngleCore = 360f;
		}
		public float StartAngle {
			get { return startAngleCore; }
			set {
				if(StartAngle == value) return;
				this.startAngleCore = value;
				OnPropertiesChanged();
			}
		}
		public float EndAngle {
			get { return endAngleCore; }
			set {
				if(EndAngle == value) return;
				this.endAngleCore = value;
				OnPropertiesChanged();
			}
		}
		public float MaximumAngle {
			get { return maximumAngleCore; }
			set {
				if(MaximumAngle == value) return;
				this.maximumAngleCore = value;
				OnPropertiesChanged();
			}
		}
		public float MinimumAngle {
			get { return minimumAngleCore; }
			set {
				if(MinimumAngle == value) return;
				this.minimumAngleCore = value;
				OnPropertiesChanged();
			}
		}
		protected override GraphicsPath OnCreatePath() {
			if(Box.IsEmpty)
				return EmptyPath;
			GraphicsPath path = new GraphicsPath();
			path.AddArc(Box, StartAngle, EndAngle - StartAngle);
			return path;
		}
		protected override RectangleF CalcBounds() {
			float start = Math.Min(StartAngle, EndAngle);
			float d_range = (Math.Max(StartAngle, EndAngle) - start) / 12f;
			float rx = Box.Width / 2f;
			float ry = Box.Height / 2f;
			float x = Box.Left + rx;
			float y = Box.Top + ry;
			float left = float.MaxValue,
				right = float.MinValue,
				top = float.MaxValue,
				bottom = float.MinValue;
			for(int i = 0; i <= 12; i++) {
				double angle = (start + d_range * i) * DegreeToRadian;
				float dx = rx * (float)Math.Cos(angle);
				float dy = ry * (float)Math.Sin(angle);
				left = Math.Min(left, x + dx);
				right = Math.Max(right, x + dx);
				top = Math.Min(top, y + dy);
				bottom = Math.Max(bottom, y + dy);
			}
			return new RectangleF(left, top, right - left, bottom - top);
		}
		protected override void OnCollectPoints() {
			float start = Math.Min(StartAngle, EndAngle);
			float d_range = (Math.Max(StartAngle, EndAngle) - start) / 12f;
			float x = Box.Left + Box.Width / 2f;
			float y = Box.Top + Box.Height / 2f;
			for(int i = 0; i <= 12; i++) {
				double angle = (start + d_range * i) * DegreeToRadian;
				float dx = (Box.Width / 2f) * (float)Math.Cos(angle);
				float dy = (Box.Height / 2f) * (float)Math.Sin(angle);
				PathPointType type = PathPointType.Bezier;
				if(i == 0) type = PathPointType.Start;
				Add(new ShapePoint(new PointF(x + dx, y + dy), type));
			}
		}
		protected override BaseObject CloneCore() {
			return new ArcShape();
		}
		protected override void CopyToCore(BaseObject clone) {
			base.CopyToCore(clone);
			ArcShape clonedShape = clone as ArcShape;
			if(clonedShape != null) {
				clonedShape.BeginUpdate();
				clonedShape.StartAngle = this.StartAngle;
				clonedShape.EndAngle = this.EndAngle;
				clonedShape.EndUpdate();
			}
		}
	}
	public class PieShape : ArcShape {
		public PieShape()
			: base() {
		}
		public PieShape(RectangleF2D box, float startAngle, float endAngle)
			: base(box, startAngle, endAngle) {
		}
		protected override GraphicsPath OnCreatePath() {
			if(Box.IsEmpty)
				return EmptyPath;
			GraphicsPath path = new GraphicsPath();
			float start = Math.Min(StartAngle, EndAngle);
			float range = Math.Max(StartAngle, EndAngle) - start;
			path.AddPie(Box.Left, Box.Top, Box.Width, Box.Height, start, range);
			return path;
		}
		protected override void OnCollectPoints() {
			float start = Math.Min(StartAngle, EndAngle);
			float d_range = (Math.Max(StartAngle, EndAngle) - start) / 12f;
			float x = Box.Left + Box.Width / 2f;
			float y = Box.Top + Box.Height / 2f;
			Add(new ShapePoint(new PointF(x, y), PathPointType.Start));
			for(int i = 0; i <= 12; i++) {
				double angle = (start + d_range * i) * DegreeToRadian;
				float dx = (Box.Width / 2f) * (float)Math.Cos(angle);
				float dy = (Box.Height / 2f) * (float)Math.Sin(angle);
				Add(new ShapePoint(new PointF(x + dx, y + dy), PathPointType.Bezier));
			}
		}
		protected override BaseObject CloneCore() {
			return new PieShape();
		}
	}
	public class SectorShape : ArcShape {
		float internalRadiusCore;
		public SectorShape()
			: base() {
		}
		public SectorShape(RectangleF2D box, float startAngle, float endAngle)
			: base(box, startAngle, endAngle) {
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.roundedCapsCore = false;
			this.internalRadiusCore = 0.5f;
			appearanceRangeBarCore = new RangeBarAppearance();
		}
		bool showBackgroundCore;
		public bool ShowBackground {
			get { return showBackgroundCore; }
			set {
				if(ShowBackground == value) return;
				showBackgroundCore = value;
			}
		}
		bool roundedCapsCore;
		public bool RoundedCaps {
			get { return roundedCapsCore; }
			set {
				if(RoundedCaps == value) return;
				roundedCapsCore = value;
			}
		}
		public float InternalRadius {
			get { return internalRadiusCore; }
			set {
				if(InternalRadius == value) return;
				this.internalRadiusCore = value;
				OnPropertiesChanged();
			}
		}
		RangeBarAppearance appearanceRangeBarCore;
		public RangeBarAppearance AppearanceRangeBar {
			get { return appearanceRangeBarCore; }
		}
		protected override GraphicsPath OnCreatePath() {
			if(Box.IsEmpty)
				return EmptyPath;
			GraphicsPath path = new GraphicsPath();
			float rx = Box.Width / 2f;
			float ry = Box.Height / 2f;
			float rx0 = Box.Width / 2f * InternalRadius;
			float ry0 = Box.Height / 2f * InternalRadius;
			SizeF offset = new SizeF(Box.Left + rx, Box.Top + ry);
			PointF pt0 = MathHelper.GetRadiusVector(rx0, ry0, StartAngle) + offset;
			PointF pt1 = MathHelper.GetRadiusVector(rx, ry, StartAngle) + offset;
			PointF pt2 = MathHelper.GetRadiusVector(rx, ry, EndAngle) + offset;
			PointF pt3 = MathHelper.GetRadiusVector(rx0, ry0, EndAngle) + offset;
			RectangleF r = new RectangleF((Box.Left + rx) - rx0, (Box.Top + ry) - ry0, rx0 * 2f, ry0 * 2f);
			path.AddArc(Box, StartAngle, EndAngle - StartAngle);
			int directionModifier = StartAngle >= EndAngle ? -1 : 1;
			if(!r.IsEmpty) {
				if(RoundedCaps) {
					path.FillMode = FillMode.Winding;
					if(r.Width < Box.Width)
						path.AddArc(CalcArcBox(pt2, pt3), EndAngle, directionModifier * 180);
					else
						path.AddArc(CalcArcBox(pt2, pt3), EndAngle - 180, directionModifier * -180);
				}
				path.AddArc(r, EndAngle, StartAngle - EndAngle);
				if(RoundedCaps) {
					if(r.Width < Box.Width)
						path.AddArc(CalcArcBox(pt0, pt1), StartAngle - 180, directionModifier * 180);
					else
						path.AddArc(CalcArcBox(pt0, pt1), StartAngle, directionModifier * -180);
				}
			}
			else
				path.AddLine(pt2, pt3);
			return path;
		}
		public override void Render(Graphics g, IStringPainter stringPainter) {
			if(ShowBackground)
				g.FillPath(AppearanceRangeBar.BackgroundBrush.GetBrush(Bounds), GetBackgroundPath());
			base.Render(g, stringPainter);
		}
		public GraphicsPath GetBackgroundPath() {
			if(Box.IsEmpty)
				return EmptyPath;
			GraphicsPath backgroundPath = new GraphicsPath();
			float rx = Box.Width / 2f;
			float ry = Box.Height / 2f;
			float rx0 = Box.Width / 2f * InternalRadius;
			float ry0 = Box.Height / 2f * InternalRadius;
			SizeF offset = new SizeF(Box.Left + rx, Box.Top + ry);
			PointF pt0 = MathHelper.GetRadiusVector(rx0, ry0, MinimumAngle) + offset;
			PointF pt1 = MathHelper.GetRadiusVector(rx, ry, MinimumAngle) + offset;
			PointF pt2 = MathHelper.GetRadiusVector(rx, ry, MaximumAngle) + offset;
			PointF pt3 = MathHelper.GetRadiusVector(rx0, ry0, MaximumAngle) + offset;
			RectangleF r = new RectangleF((Box.Left + rx) - rx0, (Box.Top + ry) - ry0, rx0 * 2f, ry0 * 2f);
			backgroundPath.AddArc(Box, MinimumAngle, MaximumAngle - MinimumAngle);
			if(!r.IsEmpty) {
				if(RoundedCaps) {
					if(r.Width < Box.Width)
						backgroundPath.AddArc(CalcArcBox(pt2, pt3), MaximumAngle, 180);
					else
						backgroundPath.AddArc(CalcArcBox(pt2, pt3), MaximumAngle - 180, -180);
				}
				backgroundPath.AddArc(r, MaximumAngle, MinimumAngle - MaximumAngle);
				if(RoundedCaps) {
					if(r.Width < Box.Width)
						backgroundPath.AddArc(CalcArcBox(pt0, pt1), MinimumAngle - 180, 180);
					else
						backgroundPath.AddArc(CalcArcBox(pt0, pt1), MinimumAngle, -180);
				}
			}
			else
				backgroundPath.AddLine(pt2, pt3);
			return backgroundPath;
		}
		private RectangleF CalcArcBox(PointF pt0, PointF pt1) {
			RectangleF result = new RectangleF();
			result.Width = (float)Math.Sqrt((pt1.X - pt0.X) * (pt1.X - pt0.X) + (pt1.Y - pt0.Y) * (pt1.Y - pt0.Y));
			result.Width = result.Width == 0 ? 1 : result.Width;
			result.Height = result.Width;
			PointF coords = new PointF((pt0.X + pt1.X) / 2, (pt0.Y + pt1.Y) / 2);
			coords.X -= result.Width / 2;
			coords.Y -= result.Height / 2;
			result.Location = coords;
			return result;
		}
		protected override BaseObject CloneCore() {
			return new SectorShape();
		}
		protected override void CopyToCore(BaseObject clone) {
			base.CopyToCore(clone);
			SectorShape clonedShape = clone as SectorShape;
			if(clonedShape != null) {
				clonedShape.BeginUpdate();
				clonedShape.InternalRadius = this.InternalRadius;
				clonedShape.EndUpdate();
			}
		}
	}
	public class SectorRangeShape : ArcShape {
		float startThicknessCore;
		float endThicknessCore;
		public SectorRangeShape()
			: base() {
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.startThicknessCore = 0f;
			this.endThicknessCore = 1f;
		}
		public float StartThickness {
			get { return startThicknessCore; }
			set {
				if(StartThickness == value) return;
				startThicknessCore = value;
				OnPropertiesChanged();
			}
		}
		public float EndThickness {
			get { return endThicknessCore; }
			set {
				if(EndThickness == value) return;
				endThicknessCore = value;
				OnPropertiesChanged();
			}
		}
		protected override GraphicsPath OnCreatePath() {
			if(Box.IsEmpty)
				return EmptyPath;
			GraphicsPath path = new GraphicsPath();
			float rx = Box.Width / 2f;
			float ry = Box.Height / 2f;
			float rx0 = rx * (1f - StartThickness);
			float ry0 = ry * (1f - StartThickness);
			float rx1 = rx * (1f - EndThickness);
			float ry1 = ry * (1f - EndThickness);
			SizeF offset = new SizeF(Box.Left + rx, Box.Top + ry);
			PointF pt0 = MathHelper.GetRadiusVector(rx0, ry0, StartAngle) + offset;
			PointF pt1 = MathHelper.GetRadiusVector(rx, ry, StartAngle) + offset;
			PointF pt2 = MathHelper.GetRadiusVector(rx, ry, EndAngle) + offset;
			PointF pt3 = MathHelper.GetRadiusVector(rx1, ry1, EndAngle) + offset;
			PointF[] ptsTopCurve; PointF[] ptsBottomCurve;
			CalcSectorCurves(rx, ry, offset, out ptsTopCurve, out ptsBottomCurve);
			path.AddLine(pt0, pt1);
			path.AddCurve(ptsTopCurve);
			path.AddLine(pt2, pt3);
			path.AddCurve(ptsBottomCurve);
			return path;
		}
		const int CurvePrecesion = 25;
		void CalcSectorCurves(float rx, float ry, SizeF offset, out PointF[] ptsTopCurve, out PointF[] ptsBottomCurve) {
			ptsTopCurve = new PointF[CurvePrecesion];
			ptsBottomCurve = new PointF[CurvePrecesion];
			float step = 1f / ((float)CurvePrecesion - 1f);
			float thicknessRange = EndThickness - StartThickness;
			float angleRange = EndAngle - StartAngle;
			if(float.IsNaN(thicknessRange) || float.IsInfinity(thicknessRange)) thicknessRange = 0;
			if(float.IsNaN(angleRange) || float.IsInfinity(angleRange)) angleRange = 0;
			for(int i = 0; i < CurvePrecesion; i++) {
				float angle = StartAngle + angleRange * step * (float)i;
				float sina = (float)Math.Sin(angle * DegreeToRadian);
				float cosa = (float)Math.Cos(angle * DegreeToRadian);
				ptsTopCurve[i] = new PointF(rx * cosa + offset.Width, ry * sina + offset.Height);
				float thickness = 1f - (StartThickness + thicknessRange * step * (float)i);
				ptsBottomCurve[CurvePrecesion - (i + 1)] = new PointF(rx * thickness * cosa + offset.Width, ry * thickness * sina + offset.Height);
			}
		}
		protected override BaseObject CloneCore() {
			return new SectorRangeShape();
		}
		protected override void CopyToCore(BaseObject clone) {
			base.CopyToCore(clone);
			SectorRangeShape clonedShape = clone as SectorRangeShape;
			if(clonedShape != null) {
				clonedShape.BeginUpdate();
				clonedShape.StartThickness = this.StartThickness;
				clonedShape.EndThickness = this.EndThickness;
				clonedShape.EndUpdate();
			}
		}
	}
	public class TextShape : BoxShape {
		string textCore;
		public string Text {
			get { return textCore; }
			set {
				if(Text == value) return;
				textCore = value;
				OnPropertiesChanged();
			}
		}
		public override string DisplayText {
			get { return Text; }
		}
		public override RectangleF TextBounds {
			get { return CalcRectWithSpacing(Box, AppearanceText.Spacing); }
		}
		RectangleF CalcRectWithSpacing(RectangleF2D rect, TextSpacing spacing) {
			float h = Math.Max(rect.Height - spacing.Height, 0);
			float w = Math.Max(rect.Width - spacing.Width, 0);
			return new RectangleF(rect.Left + spacing.Left, rect.Top + spacing.Top, w, h);
		}
		protected override BaseObject CloneCore() {
			return new TextShape();
		}
		protected override void CopyToCore(BaseObject clone) {
			base.CopyToCore(clone);
			TextShape clonedShape = clone as TextShape;
			if(clonedShape != null) {
				clonedShape.BeginUpdate();
				clonedShape.Text = this.Text;
				clonedShape.EndUpdate();
			}
		}
		public override void Render(Graphics g, IStringPainter stringPainter) {
			RenderCore(g);
			if(TextBounds.IsEmpty || string.IsNullOrEmpty(DisplayText)) return;
			AppearanceText.BeginUpdate();
			using(new ShapeTextPainterHelper(g, this)) {
				if(AllowHtmlString) RenderHTMLString(g, stringPainter);
				else RenderString(g, stringPainter);
			}
			AppearanceText.CancelUpdate();
		}
		SolidBrush defaultBrush = new SolidBrush(Color.Black);
		protected void RenderString(Graphics g, IStringPainter stringPainter) {
			if(AppearanceText.TextBrush.IsEmpty) return;
			var brush = AppearanceText.TextBrush.GetBrush(Bounds);
			if(AppearanceText.TextBrush == BrushObject.Empty)
				brush = defaultBrush;
			g.DrawString(DisplayText, AppearanceText.Font, AppearanceText.TextBrush.GetBrush(Bounds), TextBounds, AppearanceText.Format.NativeFormat);
		}
		protected void RenderHTMLString(Graphics g, IStringPainter stringPainter) {
			Utils_StringInfo si = stringPainter.Calculate(g, AppearanceText, DisplayText, Rectangle.Ceiling(TextBounds));
			stringPainter.SetGraphics(g, AppearanceText.TextBrush.GetBrush(Bounds));
			stringPainter.DrawString(g, si);
			stringPainter.SetGraphics(null, null);
		}
	}
	public class ImageIndicatorShape : BoxShape {
		Image imageCore;
		object imageCollectionCore;
		int imageIndexCore;
		ImageLayoutMode imageLayoutModeCore;
		public Image Image {
			get { return imageCore; }
			set {
				if(Image == value) return;
				imageCore = value;
				OnPropertiesChanged();
			}
		}
		public ImageLayoutMode ImageLayoutMode {
			get { return imageLayoutModeCore; }
			set {
				if(ImageLayoutMode == value) return;
				imageLayoutModeCore = value;
				OnPropertiesChanged();
			}
		}
		public int ImageIndex {
			get { return imageIndexCore; }
			set {
				if(ImageIndex == value) return;
				imageIndexCore = value;
				OnPropertiesChanged();
			}
		}
		public object ImageCollection {
			get { return imageCollectionCore; }
			set {
				if(ImageCollection == value) return;
				imageCollectionCore = value;
				OnPropertiesChanged();
			}
		}
		public RectangleF ImageBounds {
			get { return CalcRectWithSpacing(Box, AppearanceText.Spacing); }
		}
		RectangleF CalcRectWithSpacing(RectangleF2D rect, TextSpacing spacing) {
			float h = Math.Max(rect.Height - spacing.Height, 0);
			float w = Math.Max(rect.Width - spacing.Width, 0);
			return new RectangleF(rect.Left + spacing.Left, rect.Top + spacing.Top, w, h);
		}
		protected override BaseObject CloneCore() {
			return new ImageIndicatorShape();
		}
		protected override void CopyToCore(BaseObject clone) {
			base.CopyToCore(clone);
			ImageIndicatorShape clonedShape = clone as ImageIndicatorShape;
			if(clonedShape != null) {
				clonedShape.BeginUpdate();
				clonedShape.Image = this.Image;
				clonedShape.ImageLayoutMode = this.ImageLayoutMode;
				clonedShape.EndUpdate();
			}
		}
		public override void Render(Graphics g, IStringPainter stringPainter) {
			RenderCore(g);
			if(ImageBounds.IsEmpty) return;
			AppearanceText.BeginUpdate();
			RenderImage(g);
			AppearanceText.CancelUpdate();
		}
		protected void RenderImage(Graphics g) {
			if(Image == null) return;
			using(new ShapeImageIndicatorPainterHelper(g, this)) {
				g.DrawImage(Image, DrawImageHelper.GetImageBounds((Rectangle)Box, Image.Size, imageLayoutModeCore));
			}
		}
	}
	public class ComplexShape : BaseShape {
		BaseShapeCollection collectionCore;
		protected override void OnCollectPoints() {
			foreach(BaseShape shape in Collection) ShapePoints.AddRange(shape.ShapePoints.ToArray());
		}
		protected override RectangleF CalcBounds() {
			return base.CalcBounds();
		}
		protected override GraphicsPath OnCreatePath() {
			return EmptyPath;
		}
		protected override void OnCreate() {
			base.OnCreate();
		}
		protected override void OnDispose() {
			if(collectionCore != null) {
				Collection.DisposeItems();
				collectionCore = null;
			}
			base.OnDispose();
		}
		public ComplexShape() {
		}
		public ComplexShape(BaseShapeCollection collection) {
			collectionCore = collection;
		}
		public BaseShapeCollection Collection {
			get {
				if(collectionCore == null)
					collectionCore = new BaseShapeCollection();
				return collectionCore;
			}
		}
		public void Add(BaseShape shape) {
			BeginUpdate();
			AddCore(shape);
			OnPropertiesChanged();
			EndUpdate();
		}
		protected void AddCore(BaseShape shape) {
			bool nameIsEmpty = String.IsNullOrEmpty(shape.Name);
			if(nameIsEmpty) shape.Name = UniqueNameHelper.GetShapeUniqueName(Collection);
			Collection.Add(shape);
		}
		public void AddRange(BaseShape[] shapes) {
			BeginUpdate();
			for(int i = 0; i < shapes.Length; i++) AddCore(shapes[i]);
			OnPropertiesChanged();
			EndUpdate();
		}
		public override void Accept(IVisitor<BaseShape> visitor) {
			if(visitor == null) return;
			base.Accept(visitor);
			foreach(BaseShape shape in Collection) shape.Accept(visitor);
		}
		public override void Accept(VisitDelegate<BaseShape> visit) {
			if(visit == null) return;
			base.Accept(visit);
			foreach(BaseShape shape in Collection) shape.Accept(visit);
		}
		protected override BaseObject CloneCore() {
			return new ComplexShape();
		}
		protected override void CopyToCore(BaseObject clone) {
			base.CopyToCore(clone);
			ComplexShape clonedShape = clone as ComplexShape;
			if(clonedShape != null) {
				clonedShape.BeginUpdate();
				BaseShape[] clonedShapes = new BaseShape[Collection.Count];
				int i = 0;
				foreach(BaseShape shape in Collection) clonedShapes[i++] = shape.Clone();
				clonedShape.AddRange(clonedShapes);
				clonedShape.EndUpdate();
			}
		}
		public override void Render(Graphics g, IStringPainter stringPainter) {
			foreach(BaseShape shape in Collection)
				shape.Render(g, stringPainter);
		}
	}
	public class ReadonlyShapeObjectTypeConverter : ExpandableObjectConverter {
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			return FilterPropertiesHelper.FilterProperties(base.GetProperties(context, value, attributes), GetExpectedProperties(value));
		}
		static string[] GetExpectedProperties(object value) {
			BaseShape baseShape = value as BaseShape;
			if(baseShape == null || baseShape.IsEmpty) return new string[0];
			ArrayList filteredProperties = new ArrayList();
			filteredProperties.AddRange(new string[] { "Appearance" });
			if(baseShape is TextShape) {
				filteredProperties.AddRange(new string[] { "AppearanceText" });
			}
			return (string[])filteredProperties.ToArray(typeof(string));
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destType) {
			return destType == typeof(string);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType) {
			return "<BaseShape>";
		}
	}
}
