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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.Charts.Native;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile)
	]
	public enum MovingAverageKind {
		MovingAverage = MovingAverageKindInternal.MovingAverage,
		Envelope = MovingAverageKindInternal.Envelope,
		MovingAverageAndEnvelope = MovingAverageKindInternal.MovingAverageAndEnvelope
	}
	public abstract class MovingAverage : SubsetBasedIndicator {
		const MovingAverageKind DefaultKind = MovingAverageKind.MovingAverage;
		const double DefaultEnvelopePercent = 3.0;
		static readonly Color DefaultEnvelopeColor = Color.Empty;
		MovingAverageKind kind = DefaultKind;
		double envelopePercent = DefaultEnvelopePercent;
		Color envelopeColor = DefaultEnvelopeColor;
		LineStyle envelopeLineStyle;
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("MovingAverageKind"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.MovingAverage.Kind"),
		RefreshProperties(RefreshProperties.All),
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public MovingAverageKind Kind {
			get { return kind; }
			set {
				if (value != kind) {
					SendNotification(new ElementWillChangeNotification(this));
					kind = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("MovingAverageEnvelopePercent"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.MovingAverage.EnvelopePercent"),
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public double EnvelopePercent {
			get { return envelopePercent; }
			set {
				if (value != envelopePercent) {
					if (value <= 0.0 || value > 100.0)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectEnvelopePercent));
					SendNotification(new ElementWillChangeNotification(this));
					envelopePercent = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("MovingAverageEnvelopeColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.MovingAverage.EnvelopeColor"),
		Category(Categories.Appearance),
		XtraSerializableProperty
		]
		public Color EnvelopeColor {
			get { return envelopeColor; }
			set {
				if (value != envelopeColor) {
					SendNotification(new ElementWillChangeNotification(this));
					envelopeColor = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("MovingAverageEnvelopeLineStyle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.MovingAverage.EnvelopeLineStyle"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Appearance),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public LineStyle EnvelopeLineStyle { get { return envelopeLineStyle; } }
		public MovingAverage(string name, ValueLevel valueLevel) : base(name, valueLevel) {
			InitializeEnvelopeLineStyle();
		}
		public MovingAverage(string name) : base(name) {
			InitializeEnvelopeLineStyle();
		}
		#region ShouldSerialize & Reset
		bool ShouldSerializeKind() {
			return kind != DefaultKind;
		}
		void ResetKind() {
			Kind = DefaultKind;
		}
		bool ShouldSerializeEnvelopePercent() {
			return envelopePercent != DefaultEnvelopePercent;
		}
		void ResetEnvelopePercent() {
			EnvelopePercent = DefaultEnvelopePercent;
		}
		bool ShouldSerializeEnvelopeColor() {
			return envelopeColor != DefaultEnvelopeColor;
		}
		void ResetEnvelopeColor() {
			EnvelopeColor = DefaultEnvelopeColor;
		}
		bool ShouldSerializeEnvelopeLineStyle() {
			return envelopeLineStyle.ShouldSerialize();
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "Kind":
					return ShouldSerializeKind();
				case "EnvelopePercent":
					return ShouldSerializeEnvelopePercent();
				case "EnvelopeColor":
					return ShouldSerializeEnvelopeColor();
				case "EnvelopeLineStyle":
					return ShouldSerializeEnvelopeLineStyle();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		void InitializeEnvelopeLineStyle() {
			envelopeLineStyle = new LineStyle(this, 1, true, DashStyle.Solid);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			MovingAverage movingAverage = obj as MovingAverage;
			if (movingAverage != null) {
				this.kind = movingAverage.kind;
				this.envelopePercent = movingAverage.envelopePercent;
				this.envelopeColor = movingAverage.envelopeColor;
				this.envelopeLineStyle.Assign(movingAverage.envelopeLineStyle);
			}
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public abstract class MovingAverageIndicatorBehavior : IndicatorBehavior {
		LineStrip movingAverageData;
		LineStrip upperEnvelopeData;
		LineStrip lowerEnvelopeData;
		MovingAverage MovingAverage { get { return (MovingAverage)Indicator; } }
		Color EnvelopeColor {
			get {
				Color envelopeColor = MovingAverage.EnvelopeColor;
				return envelopeColor.IsEmpty ? Color : envelopeColor;
			}
		}
		public override Color LegendColor { get { return (MovingAverage.Kind == MovingAverageKind.Envelope) ? EnvelopeColor : base.LegendColor; } }
		public override LineStyle LegendLineStyle {
			get {
				MovingAverage movingAverage = MovingAverage;
				return (movingAverage.Kind == MovingAverageKind.Envelope) ? movingAverage.EnvelopeLineStyle : base.LegendLineStyle;
			}
		}
		protected MovingAverageIndicatorBehavior(MovingAverage movingAverage) : base(movingAverage) {
		}
		protected void SetMovingAverageData(List<GRealPoint2D> movingAverageData) {
			this.movingAverageData = new LineStrip(movingAverageData);
		}
		protected abstract void CalculateInternal(IRefinedSeries seriesInfo);
		protected override void Calculate(IRefinedSeries seriesInfo) {
			movingAverageData = null;
			upperEnvelopeData = null;
			lowerEnvelopeData = null;
			CalculateInternal(seriesInfo);
			MovingAverage movingAverage = MovingAverage;
			if (movingAverage.Kind != MovingAverageKind.MovingAverage && Visible && movingAverageData != null) {
				int size = movingAverageData.Count;
				double factor = movingAverage.EnvelopePercent / 100.0;
				upperEnvelopeData = new LineStrip(size);
				lowerEnvelopeData = new LineStrip(size);
				foreach (GRealPoint2D point in movingAverageData) {
					double x = point.X;
					double y = point.Y;
					double dy = y * factor;
					upperEnvelopeData.Add(new GRealPoint2D(x, y + dy));
					lowerEnvelopeData.Add(new GRealPoint2D(x, y - dy));
				}
			}
		}
		public override IndicatorLayout CreateLayout(XYDiagramMappingBase diagramMapping, TextMeasurer textMeasurer) {
			MovingAverage movingAverage = Indicator as MovingAverage;
			if (!Visible || movingAverageData == null || movingAverage == null)
				return null;
			List<LineStripItem> lineStrips = new List<LineStripItem>();
			if (movingAverage.Kind != MovingAverageKind.Envelope) {
				LineStyle lineStyle = movingAverage.LineStyle;
				LineStripItem item = new LineStripItem(StripsUtils.MapStrip(diagramMapping, movingAverageData),
					GraphicUtils.CorrectColorByHitTestState(Color, ((IHitTest)movingAverage).State), lineStyle,
					GraphicUtils.CorrectThicknessByHitTestState(lineStyle.Thickness, ((IHitTest)movingAverage).State, 1));
				lineStrips.Add(item);
			}
			if (movingAverage.Kind != MovingAverageKind.MovingAverage) {
				Color color = GraphicUtils.CorrectColorByHitTestState(EnvelopeColor, ((IHitTest)movingAverage).State);
				LineStyle lineStyle = movingAverage.EnvelopeLineStyle;
				int thickness = GraphicUtils.CorrectThicknessByHitTestState(lineStyle.Thickness, ((IHitTest)movingAverage).State, 1);
				if (upperEnvelopeData != null)
					lineStrips.Add(new LineStripItem(StripsUtils.MapStrip(diagramMapping, upperEnvelopeData), color, lineStyle, thickness));
				if (lowerEnvelopeData != null)
					lineStrips.Add(new LineStripItem(StripsUtils.MapStrip(diagramMapping, lowerEnvelopeData), color, lineStyle, thickness));
			}
			return new LineStripsIndicatorLayout(movingAverage, lineStrips);
		}
	}
	public class LineStripItem {
		readonly LineStrip lineStrip;
		readonly Color color;
		readonly LineStyle lineStyle;
		readonly int thickness;
		public LineStrip LineStrip { get { return lineStrip; } }
		public Color Color { get { return color; } }
		public LineStyle LineStyle { get { return lineStyle; } }
		public int Thickness { get { return thickness; } }
		public LineStripItem(LineStrip lineStrip, Color color, LineStyle lineStyle, int thickness) {
			this.lineStrip = lineStrip;
			this.color = color;
			this.lineStyle = lineStyle;
			this.thickness = thickness;
		}
	}
	public class LineStripsIndicatorLayout : IndicatorLayout {
		readonly IEnumerable<LineStripItem> items;
		public LineStripsIndicatorLayout(Indicator indicator, IEnumerable<LineStripItem> items) : base(indicator) {
			this.items = items;
		}
		public override void Render(IRenderer renderer) {
			foreach (LineStripItem item in items) {
				renderer.EnableAntialiasing(item.LineStyle.AntiAlias);
				renderer.DrawLines(item.LineStrip, item.Color, item.Thickness, item.LineStyle, LineCap.Flat);
				renderer.RestoreAntialiasing();
			}
		}
		public override GraphicsPath CalculateHitTestGraphicsPath() {
			GraphicsPath path = new GraphicsPath();
			try {
				foreach (LineStripItem item in items) {
					GraphicsPath itemPath = StripsUtils.GetPath(item.LineStrip, item.Thickness, Indicator.LineStyle);
					if (itemPath.PointCount == 0)
						itemPath.Dispose();
					else
						path.AddPath(itemPath, false);
				}
			}
			catch {
				path.Dispose();
				throw;
			}
			return path;
		}
	}
	public class MultilineIndicatorLayout : IndicatorLayout {
		LineStrip lineStrip;
		public MultilineIndicatorLayout(Indicator indicator, LineStrip multiline) 
			: base(indicator) {
			this.lineStrip = multiline;
		}
		public override GraphicsPath CalculateHitTestGraphicsPath() {
			GraphicsPath path = new GraphicsPath();
			if (this.lineStrip == null)
				return null;
			try {
				GraphicsPath itemPath = StripsUtils.GetPath(this.lineStrip, Thickness, Indicator.LineStyle);
				if (itemPath.PointCount == 0)
					itemPath.Dispose();
				else
					path.AddPath(itemPath, false);
			}
			catch {
				path.Dispose();
				throw;
			}
			return path;
		}
		public override void Render(IRenderer renderer) {
			renderer.EnableAntialiasing(Indicator.LineStyle.AntiAlias);
			renderer.DrawLines(this.lineStrip, Color, Thickness, Indicator.LineStyle, LineCap.Flat);
			renderer.RestoreAntialiasing();
		}
	}
}
