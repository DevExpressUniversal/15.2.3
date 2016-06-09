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
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Resources;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel.Design.Serialization;
#if !DXPORTABLE
using System.Drawing.Design;
#endif
namespace DevExpress.XtraGauges.Core.Drawing {
#if !DXPORTABLE
	[Editor("DevExpress.XtraGauges.Design.BrushObjectTypeEditor, " + AssemblyInfo.SRAssemblyGaugesDesignWin, typeof(UITypeEditor))]
#endif
	[TypeConverter(typeof(BrushObjectTypeConverter)), RefreshProperties(RefreshProperties.All)]
	public abstract class BrushObject : BaseObjectEx, ISupportColorShading, ISupportAssign<BrushObject>, IFormattable {
		public static readonly BrushObject Empty;
		public static readonly RectangleF2D DefaultRectangle;
		float penWidth;
		Pen cachedPenCore;
		RectangleF2D brushRect;
		Matrix transformCore;
		Brush cachedBrushCore;
		static BrushObject() {
			DefaultRectangle = new RectangleF2D(0, 0, 10, 10);
			Empty = new EmptyBrushObject();
		}
		protected BrushObject() { }
		protected BrushObject(string dataTag) {
			BeginUpdate();
			Assign(dataTag);
			EndUpdate();
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.penWidth = 0;
		}
		protected override void OnDispose() {
			Ref.Dispose(ref cachedPenCore);
			Ref.Dispose(ref cachedBrushCore);
			Ref.Dispose(ref transformCore);
			base.OnDispose();
		}
		protected override void OnUnlockUpdateCore() {
			OnBrushChanged();
		}
		protected override void CopyToCore(BaseObject clone) {
			BrushObject cloneBrush = clone as BrushObject;
			if(cloneBrush != null) {
				cloneBrush.PenWidth = this.PenWidth;
				cloneBrush.BrushRect = this.BrushRect;
			}
		}
		public abstract bool IsDifferFrom(BrushObject source);
		protected internal void AssignInternal(BrushObject source) {
			BeginUpdate();
			AssignCore(source);
			CancelUpdate();
		}
		public void Assign(BrushObject source) {
			BeginUpdate();
			AssignCore(source);
			EndUpdate();
		}
		protected virtual void AssignCore(BrushObject source) {
			this.PenWidth = source.PenWidth;
			this.BrushRect = source.BrushRect;
		}
		protected Pen CachedPen {
			get { return cachedPenCore; }
		}
		protected Brush CachedBrush {
			get { return cachedBrushCore; }
		}
		protected internal float PenWidth {
			get { return penWidth; }
			set {
				if(PenWidth == value) return;
				penWidth = value;
				OnPenChanged();
			}
		}
		protected internal abstract bool ShouldSerialize();
		internal bool ShouldSerializeBrushRect() {
			return !BrushRect.IsEmpty;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Matrix Transform {
			get {
				if(transformCore == null)
					transformCore = new Matrix();
				return transformCore;
			}
			set {
				if(value.IsIdentity)
					transformCore = null;
				else
					transformCore = value;
				OnBrushChanged();
			}
		}
		public RectangleF2D BrushRect {
			get { return brushRect; }
			set {
				if(BrushRect == value) return;
				brushRect = value;
				OnBrushChanged();
			}
		}
		public Pen GetPen(float width) {
			PenWidth = width;
			if(CachedPen == null) 
				this.cachedPenCore = CreatePenCore();
			return CachedPen;
		}
		public virtual Brush GetBrush(RectangleF2D rect) {
			if(!rect.IsEmpty) BrushRect = rect;
			if(CachedBrush == null)
				this.cachedBrushCore = CreateBrushCore();
			return CachedBrush;
		}
		protected virtual Pen CreatePenCore() {
			Brush penBrush = GetBrush(BrushRect.IsEmpty ? DefaultRectangle : BrushRect);
			return new Pen(penBrush, PenWidth);
		}
		protected abstract Brush CreateBrushCore();
		protected void OnBrushChanged() {
			if(IsDisposing || IsUpdateLocked) return;
			RaiseChanged(EventArgs.Empty);
			Ref.Dispose(ref cachedPenCore);
			Ref.Dispose(ref cachedBrushCore);
		}
		protected void OnPenChanged() {
			Ref.Dispose(ref cachedPenCore);
		}
		public void Accept(IColorShader shader) {
			Color[] colors = ToColors();
			shader.Process(ref colors);
			FromColors(colors);
		}
		static Color[] EmptyColors = new Color[0];
		protected virtual Color[] ToColors() {
			return EmptyColors;
		}
		protected virtual void FromColors(Color[] colors) { }
		public bool IsEmpty {
			get { return this is EmptyBrushObject; }
		}
		class EmptyBrushObject : BrushObject {
			protected override Brush CreateBrushCore() {
				return new SolidBrush(Color.Empty);
			}
			protected override BaseObject CloneCore() {
				return this;
			}
			public override bool IsDifferFrom(BrushObject source) {
				return (source == null) ? true : !source.IsEmpty;
			}
			protected internal override bool ShouldSerialize() {
				return false;
			}
			protected override string GetBrushTypeTag() {
				return "Empty";
			}
			protected internal override string GetBrushDataTag() {
				return string.Empty;
			}
			protected internal override void Assign(string brushData) { }
		}
		string IFormattable.ToString(string format, IFormatProvider provider) {
			string dataTag = GetBrushDataTag();
			return (string.IsNullOrEmpty(dataTag)) ? String.Format("<BrushObject Type=\"{0}\"/>", GetBrushTypeTag()) :
				String.Format("<BrushObject Type=\"{0}\" Data=\"{1}\"/>", GetBrushTypeTag(), dataTag);
		}
		protected abstract string GetBrushTypeTag();
		protected internal abstract string GetBrushDataTag();
		protected internal abstract void Assign(string brushData);
	}
	public class SolidBrushObject : BrushObject {
		Color colorCore;
		public SolidBrushObject() { }
		public SolidBrushObject(Color brushColor) {
			this.colorCore = brushColor;
		}
		public SolidBrushObject(string dataTag)
			: base(dataTag) {
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.colorCore = Color.White;
		}
		[XtraSerializableProperty]
		public Color Color {
			get { return colorCore; }
			set {
				if(colorCore == value) return;
				colorCore = value;
				OnBrushChanged();
			}
		}
		protected override Brush CreateBrushCore() {
			return new SolidBrush(Color);
		}
		protected override BaseObject CloneCore() {
			return new SolidBrushObject(Color);
		}
		protected override void AssignCore(BrushObject source) {
			base.AssignCore(source);
			SolidBrushObject brush = source as SolidBrushObject;
			if(brush != null) {
				this.Color = brush.Color;
			}
		}
		public override bool IsDifferFrom(BrushObject source) {
			SolidBrushObject brush = source as SolidBrushObject;
			return (brush == null) ? true : (Color != brush.Color);
		}
		protected override Color[] ToColors() {
			return new Color[] { Color };
		}
		protected override void FromColors(Color[] colors) {
			if(colors.Length > 0) {
				Color = colors[0];
			}
		}
		internal bool ShouldSerializeColor() {
			return Color != Color.Empty;
		}
		internal void ResetColor() {
			Color = Color.Empty;
		}
		protected internal override bool ShouldSerialize() {
			return ShouldSerializeColor();
		}
		protected override string GetBrushTypeTag() {
			return "Solid";
		}
		protected internal override string GetBrushDataTag() {
			string colorData = String.Format("Color:{0}", ARGBColorTranslator.ToHtml(Color));
			return ShouldSerializeColor() ? colorData : string.Empty;
		}
		protected internal override void Assign(string brushData) {
			if(string.IsNullOrEmpty(brushData)) return;
			MatchCollection matches = Regex.Matches(brushData, "Color:(?<color>.*?)\\z");
			if(matches.Count == 1) Color = ARGBColorTranslator.FromHtml(matches[0].Groups["color"].Value);
		}
	}
	public class PenBrushObject : SolidBrushObject {
		float[] penDashPatternCore;
		public PenBrushObject()
			: base() {
		}
		public PenBrushObject(Color brushColor)
			: base(brushColor) {
		}
		public PenBrushObject(Color brushColor, float[] pattern)
			: base(brushColor) {
			this.penDashPatternCore = pattern;
		}
		[XtraSerializableProperty]
		public float[] PenPattern {
			get { return penDashPatternCore; }
			set {
				if(PenPattern == value) return;
				penDashPatternCore = value;
				OnPenChanged();
			}
		}
		protected override BaseObject CloneCore() {
			return new PenBrushObject(Color, PenPattern);
		}
		public override bool IsDifferFrom(BrushObject source) {
			PenBrushObject brush = source as PenBrushObject;
			return (brush == null) ? true : (Color != brush.Color) || (PenPattern != brush.PenPattern);
		}
		protected override Pen CreatePenCore() {
			Pen pen = base.CreatePenCore();
			if(PenPattern != null) pen.DashPattern = PenPattern;
			return pen;
		}
		protected override string GetBrushTypeTag() {
			return "Pen";
		}
	}
	public abstract class BaseGradientBrushObject : BrushObject {
		Color startColorCore = Color.Empty;
		Color endColorCore = Color.Empty;
		GradientStopCollection gradientStopsCore;
		protected BaseGradientBrushObject() : base() { }
		protected BaseGradientBrushObject(string dataTag) : base(dataTag) { }
		protected override void OnCreate() {
			base.OnCreate();
			this.gradientStopsCore = new GradientStopCollection();
			this.gradientStopsCore.CollectionChanged += OnGradientStopsChanged;
		}
		protected override void OnDispose() {
			if(GradientStops != null) {
				this.gradientStopsCore.CollectionChanged -= OnGradientStopsChanged;
				GradientStops.Clear();
				gradientStopsCore = null;
			}
			base.OnDispose();
		}
		void OnGradientStopsChanged(CollectionChangedEventArgs<GradientStop> ea) {
			OnBrushChanged();
		}
		[XtraSerializableProperty]
		public Color StartColor {
			get {
				if(GradientStops.Count > 0) return GradientStops[0].Color;
				else return startColorCore;
			}
			set {
				if(StartColor == value) return;
				if(GradientStops.Count > 0) GradientStops[0].Color = value;
				else startColorCore = value;
				OnBrushChanged();
			}
		}
		[XtraSerializableProperty]
		public Color EndColor {
			get {
				if(GradientStops.Count > 1) return GradientStops[GradientStops.Count - 1].Color;
				else return endColorCore;
			}
			set {
				if(EndColor == value) return;
				if(GradientStops.Count > 1) GradientStops[GradientStops.Count - 1].Color = value;
				else endColorCore = value;
				OnBrushChanged();
			}
		}
		public GradientStopCollection GradientStops {
			get { return gradientStopsCore; }
		}
		protected void CalcBlend(PointF2D start, PointF2D end, out float[] pos, out float[] vals, out float angle) {
			angle = CalcGradientAngle(start, end);
			pos = new float[4];
			float isx1, isy1, isx2, isy2;
			bool rezult = LineRectIntersect.Calc2(start.X, start.Y, end.X, end.Y, 0, 0, 1, 1, out isx1, out isy1, out isx2, out isy2);
			if(!rezult || isx1 > isx2 || isx1 == isx2) {
				isx1 = 0;
				isx2 = 1;
			}
			vals = new float[4] { 0, 0, 1, 1 };
			pos = new float[4] { 0, isx1, isx2, 1 };
		}
		protected float CalcGradientAngle(PointF2D start, PointF2D end) {
			float angle = 0;
			float kat1 = end.Y - start.Y;
			float kat2 = end.X - start.X;
			double hyp = Math.Sqrt(kat1 * kat1 + kat2 * kat2);
			if(kat1 == 0) angle = 0;
			else {
				if(kat2 == 0) angle = 90;
				else {
					angle = (float)(Math.Asin(kat1 / hyp) * 180.0 / Math.PI);
					if(angle < 0) angle += 270;
				}
			}
			return angle;
		}
		protected LinearGradientBrush CreateLinearBrush(LinearGradientMode mode, RectangleF brushRect) {
			return new LinearGradientBrush(brushRect, StartColor, EndColor, mode);
		}
		protected virtual ICollection SortGradientStops(GradientStopCollection collection) {
			ArrayList list = new ArrayList(collection);
			list.Sort(new GradientStopsComparer());
			return list;
		}
		public class GradientStopsComparer : IComparer {
			public int Compare(object x, object y) {
				GradientStop gs1 = x as GradientStop;
				GradientStop gs2 = y as GradientStop;
				if(gs1 != null && gs2 != null) {
					if(gs1.Offset == gs2.Offset) return 0;
					if(gs1.Offset > gs2.Offset) return 1;
					else return -1;
				}
				return -1;
			}
		}
		protected ColorBlend CalcColorBlend() {
			GradientStopCollection grStops = new GradientStopCollection();
			grStops.AddRange(
					new GradientStop[] { new GradientStop(StartColor, 0), new GradientStop(EndColor, 1) }
				);
			ICollection sortedGradients = SortGradientStops(GradientStops.Count > 0 ? GradientStops : grStops);
			ColorBlend cBlend = new ColorBlend();
			Color[] colors = new Color[sortedGradients.Count + 2];
			float[] offsets = new float[sortedGradients.Count + 2];
			int counter = 1;
			Color leftColor = Color.Empty, rightColor = Color.Empty;
			float minPos = float.MaxValue;
			float maxPos = float.MinValue;
			foreach(GradientStop gs in sortedGradients) {
				if(gs.Offset < minPos) {
					minPos = gs.Offset;
					leftColor = gs.Color;
				}
				if(gs.Offset > maxPos) {
					maxPos = gs.Offset;
					rightColor = gs.Color;
				}
				colors[counter] = gs.Color;
				offsets[counter] = gs.Offset;
				counter++;
			}
			colors[0] = leftColor;
			offsets[0] = 0;
			colors[sortedGradients.Count + 1] = rightColor;
			offsets[sortedGradients.Count + 1] = 1;
			cBlend.Colors = colors;
			cBlend.Positions = offsets;
			return cBlend;
		}
		protected LinearGradientBrush CreateLinearBrush(PointF2D startPoint, PointF2D endPoint, RectangleF brushRect) {
			brushRect.Inflate(20, 20);
			PointF sp = new PointF(startPoint.X * brushRect.Width, startPoint.Y * brushRect.Height);
			PointF ep = new PointF(endPoint.X * brushRect.Width, endPoint.Y * brushRect.Height);
			LinearGradientBrush brush;
			try {
				brush = new LinearGradientBrush(sp, ep, StartColor, EndColor);
			}
			catch {
				brush = new LinearGradientBrush(new Rectangle(0, 0, 1, 1), Color.Transparent, Color.Transparent, 0.0f);
			}
			if(Transform != null && !Transform.IsIdentity) {
				Matrix m2 = brush.Transform;
				Matrix m1 = Transform.Clone();
				PointF[] offset = { new PointF(m2.Elements[4], m2.Elements[5]), new PointF(m1.Elements[4], m1.Elements[5]) };
				m1.TransformPoints(offset);
				m1.Multiply(m2);
				m1 = new Matrix(
					m1.Elements[0],
					m1.Elements[1],
					m1.Elements[2],
					m1.Elements[3],
					brushRect.X + offset[0].X + +Transform.OffsetX,
					brushRect.Y + offset[0].Y + +Transform.OffsetY);
				brush.Transform = m1;
				m1.Dispose();
			}
			else {
				Matrix m1 = brush.Transform;
				Matrix m2 = new Matrix(
					m1.Elements[0],
					m1.Elements[1],
					m1.Elements[2],
					m1.Elements[3],
					brushRect.X + m1.Elements[4],
					brushRect.Y + m1.Elements[5]
					);
				brush.Transform = m2;
				m2.Dispose();
			}
			ColorBlend cb = CalcColorBlend();
			if(cb != null) brush.InterpolationColors = cb;
			return brush;
		}
		protected PathGradientBrush CreateEllipticalPathBrush(PointF2D center, float rx, float ry, RectangleF2D brushRect) {
			PathGradientBrush brush = null;
			using(GraphicsPath gPath = new GraphicsPath()) {
				RectangleF rect;
				if(center.X == 0 && center.Y == 0)
					rect = new RectangleF((center.X - rx) + brushRect.X, (center.Y - ry) + brushRect.Y, brushRect.Width, brushRect.Height);
				else
					rect = new RectangleF((center.X - rx) * brushRect.Width + brushRect.X, (center.Y - ry) * brushRect.Height + brushRect.Y, rx * 2.0f * brushRect.Width, ry * 2.0f * brushRect.Height);
				gPath.AddEllipse(rect);
				brush = CreatePathGradientBrush(gPath, rect);
			}
			return brush;
		}
		protected PathGradientBrush CreateRectangularPathBrush(RectangleF rect, RectangleF brushRect) {
			PathGradientBrush brush = null;
			using(GraphicsPath gPath = new GraphicsPath()) {
				gPath.AddRectangle(rect);
				brush = CreatePathGradientBrush(gPath, rect);
			}
			return brush;
		}
		PathGradientBrush CreatePathGradientBrush(GraphicsPath path, RectangleF rect) {
			PathGradientBrush brush;
			try {
				brush = new PathGradientBrush(path);
			}
			catch {
				brush = new PathGradientBrush(new Point[] { new Point(0, 0), new Point(1, 1), new Point(2, 2) });
			}
			ColorBlend cb = CalcColorBlend();
			for(int i = 0; i < cb.Positions.Length; i++) {
				cb.Positions[i] = 1 - cb.Positions[i];
			}
			ArrayList temp = new ArrayList(cb.Colors);
			temp.Reverse();
			cb.Colors = (Color[])temp.ToArray(typeof(Color));
			temp = new ArrayList(cb.Positions);
			temp.Reverse();
			cb.Positions = (float[])temp.ToArray(typeof(float));
			brush.InterpolationColors = cb;
			brush.CenterPoint = GetCenterPoint(rect);
			return brush;
		}
		Color[] GetSurroundColors() {
			return new Color[] { EndColor };
		}
		PointF GetCenterPoint(RectangleF rect) {
			return new PointF(rect.Left + rect.Width / 2.0f, rect.Top + rect.Height / 2.0f);
		}
		protected override void CopyToCore(BaseObject clone) {
			base.CopyToCore(clone);
			BaseGradientBrushObject clonedBrush = clone as BaseGradientBrushObject;
			if(clonedBrush != null) {
				GradientStop[] stops = new GradientStop[GradientStops.Count];
				for(int i = 0; i < stops.Length; i++) {
					stops[i] = new GradientStop(GradientStops[i].Color, GradientStops[i].Offset);
				}
				clonedBrush.GradientStops.AddRange(stops);
				clonedBrush.startColorCore = StartColor;
				clonedBrush.endColorCore = EndColor;
			}
		}
		protected override void AssignCore(BrushObject source) {
			base.AssignCore(source);
			BaseGradientBrushObject brush = source as BaseGradientBrushObject;
			if(brush != null) {
				GradientStop[] stops = new GradientStop[GradientStops.Count];
				for(int i = 0; i < stops.Length; i++) {
					stops[i] = new GradientStop(brush.GradientStops[i].Color, brush.GradientStops[i].Offset);
				}
				GradientStops.AddRange(stops);
				StartColor = brush.StartColor;
				EndColor = brush.EndColor;
			}
		}
		public override bool IsDifferFrom(BrushObject source) {
			BaseGradientBrushObject brush = source as BaseGradientBrushObject;
			return (brush == null) ? true :
				(StartColor != brush.StartColor) ||
				(EndColor != brush.EndColor);
		}
		protected override Color[] ToColors() {
			Color[] colors = new Color[GradientStops.Count > 0 ? GradientStops.Count : 2];
			if(GradientStops.Count > 0) {
				for(int i = 0; i < colors.Length; i++) colors[i] = GradientStops[i].Color;
			}
			else {
				colors[0] = StartColor;
				colors[1] = EndColor;
			}
			return colors;
		}
		protected override void FromColors(Color[] colors) {
			if(colors.Length < 2) return;
			BeginUpdate();
			if(GradientStops.Count > 0) {
				for(int i = 0; i < colors.Length; i++) GradientStops[i].Color = colors[i];
			}
			else {
				StartColor = colors[0];
				EndColor = colors[1];
			}
			EndUpdate();
		}
		internal bool ShouldSerializeStartColor() {
			return StartColor != Color.Empty;
		}
		internal bool ShouldSerializeEndColor() {
			return EndColor != Color.Empty;
		}
		internal void ResetStartColor() {
			StartColor = Color.Empty;
		}
		internal void ResetEndColor() {
			EndColor = Color.Empty;
		}
		protected internal override bool ShouldSerialize() {
			return ShouldSerializeStartColor() || ShouldSerializeEndColor();
		}
		protected internal override void Assign(string brushData) {
			if(string.IsNullOrEmpty(brushData)) return;
			MatchCollection geomMatches = Regex.Matches(brushData, @"Geometry\[(?<geometry>.*?)\]");
			if(geomMatches.Count == 1) ParseGeometry(geomMatches[0].Groups["geometry"].Value);
			MatchCollection colorMatches = Regex.Matches(brushData, @"Colors\[(?<colors>.*?)\]");
			if(colorMatches.Count == 1) ParseColors(colorMatches[0].Groups["colors"].Value);
			MatchCollection gsMatches = Regex.Matches(brushData, @"GradientStops\[(?<gs>.*?)\]");
			if(gsMatches.Count == 1) ParseGradientStops(gsMatches[0].Groups["gs"].Value);
		}
		protected virtual void ParseGeometry(string str) { }
		protected void ParseColors(string str) {
			MatchCollection m = Regex.Matches(str, @"Start:(?<startColor>.*?);End:(?<endColor>.*?)$");
			if(m.Count == 1) {
				StartColor = ARGBColorTranslator.FromHtml(m[0].Groups["startColor"].Value);
				EndColor = ARGBColorTranslator.FromHtml(m[0].Groups["endColor"].Value);
			}
		}
		protected void ParseGradientStops(string str) {
			string[] stops = str.Split(new char[] { ';' });
			GradientStops.Clear();
			for(int i = 0; i < stops.Length; i++) {
				MatchCollection m = Regex.Matches(stops[i], @"(?<offset>.*?)/(?<color>.*?)$");
				GradientStop stop = new GradientStop();
				if(m.Count == 1) {
					stop.Offset = float.Parse(m[0].Groups["offset"].Value, CultureInfo.InvariantCulture);
					stop.Color = ARGBColorTranslator.FromHtml(m[0].Groups["color"].Value);
				}
				GradientStops.Add(stop);
			}
		}
		protected internal override string GetBrushDataTag() {
			string colors = ColorsToString();
			string gradients = GradientStopsToString();
			string[] colorData = new string[] { GeometryToString(), (gradients != null) ? gradients : colors };
			return ShouldSerialize() ? String.Join(" ", colorData) : string.Empty;
		}
		protected string ColorsToString() {
			return "Colors[" + String.Format("Start:{0}", ARGBColorTranslator.ToHtml(StartColor)) + ";" +
			String.Format("End:{0}", ARGBColorTranslator.ToHtml(EndColor)) + "]";
		}
		protected virtual string GeometryToString() {
			return null;
		}
		protected string GradientStopsToString() {
			string[] gradientStops = new string[GradientStops.Count];
			for(int gs = 0; gs < GradientStops.Count; gs++) {
				gradientStops[gs] = String.Format(CultureInfo.InvariantCulture, "{0}/{1}",
					GradientStops[gs].Offset, ARGBColorTranslator.ToHtml(GradientStops[gs].Color));
			}
			return (gradientStops.Length > 0) ? ("GradientStops[" + String.Join(";", gradientStops) + "]") : null;
		}
	}
	public class SimpleLinearGradientBrushObject : BaseGradientBrushObject {
		LinearGradientMode modeCore;
		public SimpleLinearGradientBrushObject()
			: base() {
			this.modeCore = LinearGradientMode.Horizontal;
		}
		public SimpleLinearGradientBrushObject(LinearGradientMode mode)
			: base() {
			this.modeCore = mode; ;
		}
		[XtraSerializableProperty]
		public LinearGradientMode Mode {
			get { return modeCore; }
			set {
				if(Mode == value) return;
				modeCore = value;
				OnBrushChanged();
			}
		}
		protected override Brush CreateBrushCore() {
			return CreateLinearBrush(Mode, (BrushRect.IsEmpty) ? DefaultRectangle : BrushRect);
		}
		protected override void CopyToCore(BaseObject clone) {
			base.CopyToCore(clone);
			SimpleLinearGradientBrushObject clonedBrush = clone as SimpleLinearGradientBrushObject;
			if(clonedBrush != null) {
				clonedBrush.modeCore = Mode;
			}
		}
		protected override BaseObject CloneCore() {
			return new SimpleLinearGradientBrushObject();
		}
		public override bool IsDifferFrom(BrushObject source) {
			SimpleLinearGradientBrushObject brush = source as SimpleLinearGradientBrushObject;
			return (brush == null) ? true : base.IsDifferFrom(brush) || (Mode != brush.Mode);
		}
		protected override string GetBrushTypeTag() {
			return "SimpleLinearGradient";
		}
		protected internal override string GetBrushDataTag() {
			return String.Format("Mode={0}", Mode.ToString());
		}
		protected internal override void Assign(string brushData) {
			if(string.IsNullOrEmpty(brushData)) return;
		}
	}
	public class LinearGradientBrushObject : BaseGradientBrushObject {
		PointF2D startPointCore;
		PointF2D endPointCore;
		public LinearGradientBrushObject()
			: base() {
		}
		public LinearGradientBrushObject(string dataTag) : base(dataTag) { }
		public LinearGradientBrushObject(PointF2D startPoint, PointF2D endPoint)
			: base() {
			this.startPointCore = startPoint;
			this.endPointCore = endPoint;
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.startPointCore = PointF2D.Empty;
			this.endPointCore = new PointF2D(DefaultRectangle.Bottom, DefaultRectangle.Right);
		}
		[XtraSerializableProperty]
		public PointF2D StartPoint {
			get { return startPointCore; }
			set {
				if(StartPoint == value) return;
				startPointCore = value;
				OnBrushChanged();
			}
		}
		[XtraSerializableProperty]
		public PointF2D EndPoint {
			get { return endPointCore; }
			set {
				if(EndPoint == value) return;
				endPointCore = value;
				OnBrushChanged();
			}
		}
		protected override Brush CreateBrushCore() {
			return CreateLinearBrush(StartPoint, EndPoint, (BrushRect.IsEmpty) ? DefaultRectangle : BrushRect);
		}
		protected override void CopyToCore(BaseObject clone) {
			base.CopyToCore(clone);
			LinearGradientBrushObject clonedBrush = clone as LinearGradientBrushObject;
			if(clonedBrush != null) {
				clonedBrush.StartPoint = StartPoint;
				clonedBrush.EndPoint = EndPoint;
			}
		}
		protected override void AssignCore(BrushObject source) {
			base.AssignCore(source);
			LinearGradientBrushObject brush = source as LinearGradientBrushObject;
			if(brush != null) {
				StartPoint = brush.StartPoint;
				EndPoint = brush.EndPoint;
			}
		}
		protected override BaseObject CloneCore() {
			return new LinearGradientBrushObject();
		}
		public override bool IsDifferFrom(BrushObject source) {
			LinearGradientBrushObject brush = source as LinearGradientBrushObject;
			return (brush == null) ? true : base.IsDifferFrom(brush) ||
				(StartPoint != brush.StartPoint) || (EndPoint != brush.EndPoint);
		}
		internal bool ShouldSerializeStartPoint() {
			return StartPoint != DefaultRectangle.Location;
		}
		internal bool ShouldSerializeEndPoint() {
			return StartPoint != new PointF2D(DefaultRectangle.Right, DefaultRectangle.Bottom);
		}
		protected override string GetBrushTypeTag() {
			return "LinearGradient";
		}
		protected override string GeometryToString() {
			PointF2DConverter converter = new PointF2DConverter();
			return "Geometry[" +
				String.Format("Start:{0}", converter.ConvertToInvariantString(StartPoint)) + ";" +
				String.Format("End:{0}", converter.ConvertToInvariantString(EndPoint)) + "]";
		}
		protected override void ParseGeometry(string str) {
			MatchCollection m = Regex.Matches(str, @"Start:(?<startPoint>.*?);End:(?<endPoint>.*?)$");
			if(m.Count == 1) {
				PointF2DConverter converter = new PointF2DConverter();
				StartPoint = (PointF2D)converter.ConvertFromInvariantString(m[0].Groups["startPoint"].Value);
				EndPoint = (PointF2D)converter.ConvertFromInvariantString(m[0].Groups["endPoint"].Value);
			}
		}
	}
	public class EllipticalGradientBrushObject : BaseGradientBrushObject {
		float radiusXCore, radiusYCore;
		PointF2D centerPointCore;
		public EllipticalGradientBrushObject()
			: base() {
		}
		public EllipticalGradientBrushObject(string dataTag) : base(dataTag) { }
		public EllipticalGradientBrushObject(PointF2D center, float rx, float ry)
			: base() {
			this.centerPointCore = center;
			this.radiusXCore = rx;
			this.radiusYCore = ry;
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.centerPointCore = PointF2D.Empty;
			this.radiusXCore = DefaultRectangle.Width / 2f;
			this.radiusYCore = DefaultRectangle.Height / 2f;
		}
		[XtraSerializableProperty]
		public PointF2D Center {
			get { return centerPointCore; }
			set {
				if(Center == value) return;
				centerPointCore = value;
				OnBrushChanged();
			}
		}
		[XtraSerializableProperty]
		public float RadiusX {
			get { return radiusXCore; }
			set {
				if(RadiusX == value) return;
				radiusXCore = value;
				OnBrushChanged();
			}
		}
		[XtraSerializableProperty]
		public float RadiusY {
			get { return radiusYCore; }
			set {
				if(RadiusY == value) return;
				radiusYCore = value;
				OnBrushChanged();
			}
		}
		protected override Brush CreateBrushCore() {
			return CreateEllipticalPathBrush(Center, RadiusX, RadiusY, (BrushRect.IsEmpty) ? DefaultRectangle : BrushRect);
		}
		protected override void CopyToCore(BaseObject clone) {
			base.CopyToCore(clone);
			EllipticalGradientBrushObject clonedBrush = clone as EllipticalGradientBrushObject;
			if(clonedBrush != null) {
				clonedBrush.Center = Center;
				clonedBrush.RadiusX = RadiusX;
				clonedBrush.RadiusY = RadiusY;
			}
		}
		protected override void AssignCore(BrushObject source) {
			base.AssignCore(source);
			EllipticalGradientBrushObject brush = source as EllipticalGradientBrushObject;
			if(brush != null) {
				Center = brush.Center;
				RadiusX = brush.RadiusX;
				RadiusY = brush.RadiusY;
			}
		}
		protected override BaseObject CloneCore() {
			return new EllipticalGradientBrushObject();
		}
		public override bool IsDifferFrom(BrushObject source) {
			EllipticalGradientBrushObject brush = source as EllipticalGradientBrushObject;
			return (brush == null) ? true : base.IsDifferFrom(brush) ||
				(Center != brush.Center) ||
				(RadiusX != brush.RadiusX) ||
				(RadiusY != brush.RadiusY);
		}
		internal bool ShouldSerializeCenter() {
			return Center != PointF2D.Empty;
		}
		protected override string GeometryToString() {
			PointF2DConverter converter = new PointF2DConverter();
			return "Geometry[" + String.Format("Center:{0}", converter.ConvertToInvariantString(Center)) + ";" +
					String.Format(CultureInfo.InvariantCulture, "RadiusX:{0}", RadiusX) + ";" +
					String.Format(CultureInfo.InvariantCulture, "RadiusY:{0}", RadiusY) + "]";
		}
		protected override string GetBrushTypeTag() {
			return "EllipticalGradient";
		}
		protected override void ParseGeometry(string str) {
			MatchCollection m = Regex.Matches(str, @"Center:(?<center>.*?);RadiusX:(?<rx>.*?);RadiusY:(?<ry>.*?)$");
			if(m.Count == 1) {
				PointF2DConverter converter = new PointF2DConverter();
				Center = (PointF2D)converter.ConvertFromInvariantString(m[0].Groups["center"].Value);
				RadiusX = float.Parse(m[0].Groups["rx"].Value, CultureInfo.InvariantCulture);
				RadiusY = float.Parse(m[0].Groups["ry"].Value, CultureInfo.InvariantCulture);
			}
		}
	}
	public class BrushObjectTypeConverter : ExpandableObjectConverter {
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			return FilterPropertiesHelper.FilterProperties(base.GetProperties(context, value, attributes), GetExpectedProperties(value));
		}
		static string[] GetExpectedProperties(object value) {
			BrushObject baseBrush = value as BrushObject;
			if(baseBrush == null || baseBrush.IsEmpty) return new string[0];
			ArrayList filteredProperties = new ArrayList();
			if(baseBrush is SolidBrushObject) {
				filteredProperties.AddRange(new string[] { "Color" });
			}
			if(baseBrush is BaseGradientBrushObject) {
				filteredProperties.AddRange(new string[] { "BrushRect", "StartColor", "EndColor", "GradientStops" });
			}
			if(baseBrush is LinearGradientBrushObject) {
				filteredProperties.AddRange(new string[] { "StartPoint", "EndPoint" });
			}
			if(baseBrush is EllipticalGradientBrushObject) {
				filteredProperties.AddRange(new string[] { "Center", "RadiusX", "RadiusY" });
			}
			return (string[])filteredProperties.ToArray(typeof(string));
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destType) {
			return (destType == typeof(string)) || (destType == typeof(InstanceDescriptor));
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType == typeof(string);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType) {
			if(value is BrushObject) {
				if(destType == typeof(string)) {
					return ((IFormattable)value).ToString(null, culture);
				}
				if(destType == typeof(InstanceDescriptor)) {
					ConstructorInfo constructor = value.GetType().GetConstructor(new Type[] { typeof(string) });
					if(constructor != null) return new InstanceDescriptor(constructor, new object[] { ((BrushObject)value).GetBrushDataTag() });
				}
			}
			return base.ConvertTo(context, culture, value, destType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string str = value as string;
			if(str == null) return base.ConvertFrom(context, culture, value);
			str = str.Trim();
			if(str.Length == 0) return null;
			string pattern = "<BrushObject\\s+Type\\s*=\\s*\"(?<brushType>.*?)\"(\\s+(Data\\s*=\\s*\"(?<brushData>.*?)\"\\s?)?)?/>";
			MatchCollection matches = Regex.Matches(str, pattern);
			BrushObject brush = null;
			if(matches.Count == 1) {
				brush = CreateBrush(matches[0].Groups["brushType"].Value);
				if(brush != null) {
					brush.BeginUpdate();
					brush.Assign(matches[0].Groups["brushData"].Value);
					brush.EndUpdate();
				}
			}
			return brush;
		}
		protected BrushObject CreateBrush(string type) {
			switch(type) {
				case "Empty": return BrushObject.Empty;
				case "Solid": return new SolidBrushObject();
				case "LinearGradient": return new LinearGradientBrushObject();
				case "EllipticalGradient": return new EllipticalGradientBrushObject();
				default: return null;
			}
		}
	}
	public enum BrushObjectType { Empty, Solid, LinearGradient, EllipseGradient }
	public static class ARGBColorTranslator {
		public static string ToHtml(Color c) {
			if (c.A == 255 || c.IsEmpty) return DevExpress.Utils.DXColor.ToHtml(c); 
			else return ("#" + c.A.ToString("X2", null) + c.R.ToString("X2", null) + c.G.ToString("X2", null) + c.B.ToString("X2", null));
		}
		public static Color FromHtml(string htmlColor) {
			if (!string.IsNullOrEmpty(htmlColor) && htmlColor.Length == 9 && htmlColor[0] == '#') {
				return Color.FromArgb(
						Convert.ToInt32(htmlColor.Substring(1, 2), 0x10),
						Convert.ToInt32(htmlColor.Substring(3, 2), 0x10),
						Convert.ToInt32(htmlColor.Substring(5, 2), 0x10),
						Convert.ToInt32(htmlColor.Substring(7, 2), 0x10)
					);
			}
			else return DevExpress.Utils.DXColor.FromHtml(htmlColor); 
		}
	}
}
