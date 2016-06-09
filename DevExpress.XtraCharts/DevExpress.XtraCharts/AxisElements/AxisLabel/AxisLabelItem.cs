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
using System.Drawing;
using System.Collections.Generic;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.Utils;
namespace DevExpress.XtraCharts {
	[RuntimeObject]
	public abstract class AxisLabelItemBase : ITextPropertiesProvider, IXYDiagramLabelLayout {
		readonly AxisLabel label;
		readonly int gridIndex;
		Point basePoint;
		readonly double axisValue;
		readonly bool textItemVisible;
		bool visible = true;
		string text;
		Color textColor;
		Color backColor;
		Font font;
		DefaultBoolean enableAntialiasing;
		TextPainterBase painter;
		SizeF textSize;
		RectangleFillStyle fillStyle;
		RectangularBorder border;
		protected AxisLabel Label { get { return label; } }
		protected internal virtual bool Rotated { get { return false; } }
		internal bool Visible { get { return visible && textItemVisible; } }
		internal bool LabelVisible { get { return visible; } set { visible = value; } }
		internal Point BasePoint { get { return basePoint; } set { basePoint = value; } }
		internal RectangleF Bounds { get { return painter.Bounds; } }
		internal Rectangle RoundedBounds { get { return painter.RoundedBounds; } }
		internal TextPainterBase TextPainter { get { return painter; } }
		internal SizeF TextSize { get { return textSize; } }
		internal int GridIndex { get { return gridIndex; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("AxisLabelItemBaseAxis")]
#endif
		public AxisBase Axis { get { return (AxisBase)label.Owner; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("AxisLabelItemBaseAxisValue")]
#endif
		public object AxisValue { get { return ((IAxisData)Axis).AxisScaleTypeMap.InternalToNative(axisValue); } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("AxisLabelItemBaseAxisValueInternal")]
#endif
		public double AxisValueInternal { get { return axisValue; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("AxisLabelItemBaseText")]
#endif
		public string Text { 
			get { return text; } 
			set { text = value; }
		}
#if !SL
	[DevExpressXtraChartsLocalizedDescription("AxisLabelItemBaseTextColor")]
#endif
		public Color TextColor {
			get { return textColor; }
			set { textColor = value; }
		}
#if !SL
	[DevExpressXtraChartsLocalizedDescription("AxisLabelItemBaseFont")]
#endif
		public Font Font { 
			get { return font; } 
			set { font = value; }
		}
#if !SL
	[DevExpressXtraChartsLocalizedDescription("AxisLabelItemBaseEnableAntialiasing")]
#endif
		public DefaultBoolean EnableAntialiasing { 
			get { return enableAntialiasing; } 
			set { enableAntialiasing = value; }
		}
#if !SL
	[DevExpressXtraChartsLocalizedDescription("AxisLabelItemBaseBackColor")]
#endif
		public Color BackColor {
			get { return backColor; }
			set { backColor = value; }
		}
		[
		Obsolete("This property is now obsolete. Use the EnableAntialiasing property instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public bool Antialiasing { 
			get { return DefaultBooleanUtils.ToBoolean(enableAntialiasing, Rotated); } 
			set { EnableAntialiasing = (value) ? DefaultBoolean.True : DefaultBoolean.False; }
		}
		protected AxisLabelItemBase(AxisLabel label, Point basePoint, AxisTextItem textItem) {
			this.label = label;
			this.basePoint = basePoint;
			this.gridIndex = textItem.GridIndex;
			textItemVisible = textItem.Visible;
			axisValue = textItem.Value;
			object content = textItem.Content;
			text = content == null ? String.Empty : content.ToString();
			CustomAxisLabel customLabel = textItem.CustomAxisLabel as CustomAxisLabel;
			textColor = GetActualTextColor(customLabel, label);
			font = GetActualFont(customLabel, label);
			backColor = customLabel != null && customLabel.BackColor != Color.Empty ? customLabel.BackColor : label.BackColor;
			fillStyle = customLabel != null && customLabel.FillStyle.ShouldSerialize() ? customLabel.FillStyle : label.FillStyle;
			border = customLabel != null && customLabel.Border.ShouldSerialize() ? customLabel.Border : label.Border;
			enableAntialiasing = label.EnableAntialiasing;
		}
		#region IHitTest implementation
		object IHitTest.Object { get { return Axis; } }
		HitTestState IHitTest.State { get { return ((IHitTest)Axis).State; } }
		#endregion
		#region ITextPropertiesProvider implementation
		Color ITextPropertiesProvider.BackColor { get { return backColor; } }
		RectangleFillStyle ITextPropertiesProvider.FillStyle { get { return fillStyle; } }
		RectangularBorder ITextPropertiesProvider.Border { get { return border; } }
		Shadow ITextPropertiesProvider.Shadow { get { return null; } }
		StringAlignment ITextPropertiesProvider.Alignment { get { return Label.TextAlignment; } }
		bool ITextPropertiesProvider.ChangeSelectedBorder { get { return true; } }
		bool ITextPropertiesProvider.CorrectBoundsByBorder { get { return true; } }
		Color ITextPropertiesProvider.GetTextColor(Color color) { return textColor; }
		Color ITextPropertiesProvider.GetBorderColor(Color color) { return BorderHelper.CalculateBorderColor(border, Color.Empty, ((IHitTest)label).State); }
		#endregion
		#region ILabelLayout implementation
		GRect2D ILabelLayout.LabelBounds { get { return GraphicUtils.ConvertRect(RoundedBounds); } set { } }
		bool ILabelLayout.Visible { get { return visible; } set { visible = value; } }
		#endregion
		#region IXYDiagramLabelLayout implementation
		GPoint2D IXYDiagramLabelLayout.AnchorPoint { get { return new GPoint2D(MathUtils.StrongRound(basePoint.X), MathUtils.StrongRound(basePoint.Y)); } }
		GRect2D IXYDiagramLabelLayout.ExcludedRectangle { get { return GRect2D.Empty; } }
		ResolveOverlappingModeCore IXYDiagramLabelLayout.ResolveOverlappingMode { get { return ResolveOverlappingModeCore.None; } }
		GRect2D IXYDiagramLabelLayout.ValidRectangle { get { return GRect2D.Empty; } }
		#endregion
		#region ISupportTextAntialiasing implementation
		bool ISupportTextAntialiasing.DefaultAntialiasing { get { return false; } }
		bool ISupportTextAntialiasing.Rotated { get { return Rotated; } }
		Color ISupportTextAntialiasing.TextBackColor { get { return backColor; } }
		RectangleFillStyle ISupportTextAntialiasing.TextBackFillStyle { get { return fillStyle; } }
		ChartElement ISupportTextAntialiasing.BackElement { get { return label.BackElement; } }
		#endregion
		Color GetActualTextColor(CustomAxisLabel customLabel, AxisLabel label) {
			return customLabel != null && customLabel.TextColor != Color.Empty ? customLabel.TextColor : label.ActualTextColor;
		}
		Font GetActualFont(CustomAxisLabel customLabel, AxisLabel label) {
			return customLabel != null && customLabel.Font != null ? customLabel.Font : label.Font;
		}
		internal void Render(IRenderer renderer, HitTestController hitTestController) {
			painter.Render(renderer, hitTestController, this);
		}
		internal void CreatePainter(TextMeasurer textMeasurer) {
			textSize = textMeasurer.MeasureString(Text, Font);
			painter = CreatePainterInternal(textSize, textMeasurer);
		}
		protected abstract TextPainterBase CreatePainterInternal(SizeF textSize, TextMeasurer textMeasurer);
	}
	public class AxisLabelItem : AxisLabelItemBase, IAxisLabelLayout {
		readonly bool isCustomLabel;
		int angle;
		GRealPoint2D offset;
		GRealPoint2D limitedOffset;
		protected internal override bool Rotated { get { return Angle % 90 != 0; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("AxisLabelItemAngle")]
#endif
		public int Angle { 
			get { return angle; }
			set { angle = value; }
		}
		internal AxisLabelItem(AxisLabel label, Point basePoint, AxisTextItem textItem) : base(label, basePoint, textItem) {
			angle = label.Angle;
			isCustomLabel = textItem.IsCustomLabel;
		}
		void ApplyOffset(GRealPoint2D oldOffset, GRealPoint2D newOffset) {
			BasePoint = new Point(BasePoint.X - (int)oldOffset.X, BasePoint.Y - (int)oldOffset.Y);
			if (TextPainter != null)
				TextPainter.Offset(-oldOffset.X, -oldOffset.Y);
			BasePoint = new Point(BasePoint.X + (int)newOffset.X, BasePoint.Y + (int)newOffset.Y);
			if (TextPainter != null)
				TextPainter.Offset(newOffset.X, newOffset.Y);
		}
		protected override TextPainterBase CreatePainterInternal(SizeF textSize, TextMeasurer textMeasurer) {
			return new RotatedTextPainterNearLine(BasePoint, Text, textSize, this, ((Axis2D)Label.Owner).GetNearTextPosition(), angle, false, false, textMeasurer, Label.MaxWidth, Label.MaxLineCount, false);
		}
		#region IAxisLabelLayout Members
		GRealSize2D IAxisLabelLayout.Size {
			get { return new GRealSize2D(TextPainter.Width, TextPainter.Height); }
		}
		double IAxisLabelLayout.Angle {
			get {
				return Angle;
			}
			set {
				Angle = (int)value;
				if (TextPainter != null)
					TextPainter.Rotate(value);
			}
		}
		GRealPoint2D IAxisLabelLayout.Pivot {
			get {
				return new GRealPoint2D(BasePoint.X, BasePoint.Y);
			}
		}
		bool IAxisLabelLayout.Visible {
			get {
				return LabelVisible;
			}
			set {
				LabelVisible = value;
			}
		}
		string IAxisLabelLayout.Text {
			get {
				return Text;
			}
		}
		GRealPoint2D IAxisLabelLayout.Offset {
			get { return offset; }
			set {
				ApplyOffset(offset, value);
				offset = value;
			}
		}
		GRealPoint2D IAxisLabelLayout.LimitsOffset {
			get { return limitedOffset; }
			set {
				ApplyOffset(limitedOffset, value);
				limitedOffset = value;
			}
		}
		GRealRect2D IAxisLabelLayout.Bounds {
			get { return Visible ? Bounds.ToGRealRect2D() : GRealRect2D.Empty; }
		}
		int IAxisLabelLayout.GridIndex { get { return this.GridIndex; } }
		bool IAxisLabelLayout.IsCustomLabel { get { return isCustomLabel; } }
		#endregion
	}
	public class AxisLabel3DItem : AxisLabelItemBase {
		int angle;
		NearTextPosition textPosition;
#if !SL
	[DevExpressXtraChartsLocalizedDescription("AxisLabel3DItemAngle")]
#endif
		public int Angle {
			get { return angle; }
			set { angle = value; }
		}
#if !SL
	[DevExpressXtraChartsLocalizedDescription("AxisLabel3DItemTextPosition")]
#endif
		public AxisLabel3DPosition TextPosition {
			get { return (AxisLabel3DPosition)textPosition; }
			set { textPosition = (NearTextPosition)value; }
		}
		internal AxisLabel3DItem(XYDiagram3DCoordsCalculator coordsCalculator, AxisLabel label, Point basePoint, AxisTextItem textItem) : base(label, basePoint, textItem) {
			angle = label.Angle;
			textPosition = ((Axis3D)label.Owner).GetNearTextPosition(coordsCalculator);
		}
		protected override TextPainterBase CreatePainterInternal(SizeF textSize, TextMeasurer textMeasurer) {
			return new RotatedTextPainterNearLine(BasePoint, Text, textSize, this, textPosition, angle, false, false, textMeasurer, Label.MaxWidth, Label.MaxLineCount, false);
		}
	}
	public class RadarAxisXLabelItem : AxisLabelItemBase, IAxisLabelLayout {
		readonly float angleOnCircleDegree;
		protected internal override bool Rotated {
			get {
				RadarAxisXLabel radarLabel = Label as RadarAxisXLabel;
				if (radarLabel != null)
					return (radarLabel.TextDirection == RadarAxisXLabelTextDirection.Radial) || (radarLabel.TextDirection == RadarAxisXLabelTextDirection.Tangent);
				return false;
			}
		}
		internal RadarAxisXLabelItem(AxisLabel label, Point basePoint, AxisTextItem textItem, float angleOnCircleDegree) : base(label, basePoint, textItem) {
			this.angleOnCircleDegree = angleOnCircleDegree;
		}
		protected override TextPainterBase CreatePainterInternal(SizeF textSize, TextMeasurer textMeasurer) {
			switch (((RadarAxisXLabel)Label).TextDirection) {
				case RadarAxisXLabelTextDirection.LeftToRight:
					return new RotatedTextPainterNearCircleLeftToRight(angleOnCircleDegree, BasePoint, Text, textSize, this, false, false, textMeasurer, Label.MaxWidth, Label.MaxLineCount);
				case RadarAxisXLabelTextDirection.TopToBottom:
					return new RotatedTextPainterNearCircleTopToBottom(angleOnCircleDegree, BasePoint, Text, textSize, this, false, false, textMeasurer, Label.MaxWidth, Label.MaxLineCount);
				case RadarAxisXLabelTextDirection.BottomToTop:
					return new RotatedTextPainterNearCircleBottomToTop(angleOnCircleDegree, BasePoint, Text, textSize, this, false, false, textMeasurer, Label.MaxWidth, Label.MaxLineCount);
				case RadarAxisXLabelTextDirection.Radial:
					return new RotatedTextPainterNearCircleRadial(angleOnCircleDegree, BasePoint, Text, textSize, this, false, false, textMeasurer, Label.MaxWidth, Label.MaxLineCount);
				case RadarAxisXLabelTextDirection.Tangent:
					return new RotatedTextPainterNearCircleTangent(angleOnCircleDegree, BasePoint, Text, textSize, this, false, false, textMeasurer, Label.MaxWidth, Label.MaxLineCount);
				default:
					ChartDebug.Fail("Unknown text direction.");
					return new RotatedTextPainterNearCircleTangent(angleOnCircleDegree, BasePoint, Text, textSize, this, false, false, textMeasurer, Label.MaxWidth, Label.MaxLineCount);
			}
		}
		#region IAxisLabelLayout Members
		GRealSize2D IAxisLabelLayout.Size { get { return new GRealSize2D(TextPainter.Width, TextPainter.Height); } }
		double IAxisLabelLayout.Angle {
			get { return 0; }
			set { }
		}
		GRealPoint2D IAxisLabelLayout.Pivot { get { return new GRealPoint2D(Bounds.X, Bounds.Y); } }
		GRealPoint2D IAxisLabelLayout.LimitsOffset {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		bool IAxisLabelLayout.Visible {
			get { return LabelVisible; }
			set { LabelVisible = value; }
		}
		string IAxisLabelLayout.Text { get { return Text; } }
		GRealPoint2D IAxisLabelLayout.Offset {
			get { return new GRealPoint2D(); }
			set { }
		}
		GRealRect2D IAxisLabelLayout.Bounds { get { return GRealRect2D.Empty; } }
		bool IAxisLabelLayout.IsCustomLabel { get { return false; } }
		int IAxisLabelLayout.GridIndex { get { return this.GridIndex; } }
		#endregion
	}
	public class RadarAxisYLabelItem : AxisLabelItemBase, IAxisLabelLayout {
		readonly float angle;
		readonly NearTextPosition textPosition;
		protected internal override bool Rotated { get { return angle % 90 != 0; } }
		internal RadarAxisYLabelItem(AxisLabel label, Point basePoint, AxisTextItem textItem, float angle, NearTextPosition textPosition) : base(label, basePoint, textItem) {
			this.angle = angle;
			this.textPosition = textPosition;
		}
		protected override TextPainterBase CreatePainterInternal(SizeF textSize, TextMeasurer textMeasurer) {
			return new RotatedTextPainterNearRotatedLine(angle, textPosition, BasePoint, Text, textSize, this, false);
		}
		#region IAxisLabelLayout Members
		GRealSize2D IAxisLabelLayout.Size { get { return new GRealSize2D(TextPainter.Width, TextPainter.Height); } }
		double IAxisLabelLayout.Angle {
			get { return 0; }
			set { }
		}
		GRealPoint2D IAxisLabelLayout.Pivot { get { return new GRealPoint2D(BasePoint.X, BasePoint.Y); } }
		GRealPoint2D IAxisLabelLayout.LimitsOffset {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		bool IAxisLabelLayout.Visible {
			get { return LabelVisible; }
			set { LabelVisible = value; }
		}
		string IAxisLabelLayout.Text { get { return Text; } }
		GRealPoint2D IAxisLabelLayout.Offset {
			get { return new GRealPoint2D(); }
			set { }
		}
		GRealRect2D IAxisLabelLayout.Bounds { get { return GRealRect2D.Empty; } }
		bool IAxisLabelLayout.IsCustomLabel { get { return false; } }
		int IAxisLabelLayout.GridIndex { get { return this.GridIndex; } }
		#endregion
	}
}
namespace DevExpress.XtraCharts.Native {
	public class AxisLabelItemList : List<AxisLabelItemBase> {
		public AxisLabelItemList(int count) : base(count) {
		}
		public void AdjustPropertiesAndCreatePainters(AxisBase axis, TextMeasurer textMeasurer) {
			axis.ChartContainer.Chart.DataContainer.PivotGridDataSourceOptions.UpdateAxisLabelItems(axis, this);
			if (axis.ContainerAdapter.ShouldCustomDrawAxisLabels)
				RaiseCustomDrawEvents(axis);
			foreach (AxisLabelItemBase item in this)
				item.CreatePainter(textMeasurer);
		}
		public Rectangle GetLabelsOffsetRect(bool isVertical) {
			Rectangle maxRect = Rectangle.Empty;
			foreach (AxisLabelItemBase item in this) {
				RectangleF actualBounds = item.Bounds;
				actualBounds.Offset(-item.BasePoint.X, -item.BasePoint.Y);
				maxRect = Rectangle.Union(maxRect, GraphicUtils.RoundRectangle(actualBounds));
			}
			return maxRect;
		}
		public void UpdateCorrection(RectangleCorrection correction) {
			foreach (AxisLabelItemBase item in this) {
				if (item.Visible)
					correction.Update(item.RoundedBounds);
			}
		}
		public void UpdateHeight(RectangleCorrection correction) {
			foreach (AxisLabelItemBase item in this) {
				if (item.Visible)
					correction.UpdateHeight(item.RoundedBounds);
			}
		}
		public bool IsRotated() {
			bool rotated = false;
			foreach (AxisLabelItemBase item in this) {
				if (item.Rotated) {
					rotated = true;
					break;
				}
			}
			return rotated;
		}
		public Rectangle GetMaxLabelRect(bool isVertical, AxisAlignment alignment) {
			int minOffset = int.MaxValue;
			int maxOffset = int.MinValue;
			foreach (AxisLabelItemBase item in this) {
				Point basePoint = item.BasePoint;
				int pointOffset = isVertical ? basePoint.X : basePoint.Y;
				minOffset = Math.Min(minOffset, pointOffset);
				maxOffset = Math.Max(maxOffset, pointOffset);
			}
			Rectangle maxRect = Rectangle.Empty;
			foreach (AxisLabelItemBase item in this) {
				Point basePoint = item.BasePoint;
				int offsetX = -basePoint.X;
				int offsetY = -basePoint.Y;
				if (isVertical) {
					offsetX -= basePoint.X;
					offsetX += alignment == AxisAlignment.Far ? maxOffset : minOffset;
				}
				else {
					offsetY -= basePoint.Y;
					offsetY += alignment == AxisAlignment.Far ? minOffset : maxOffset;
				}
				Rectangle bounds = MathUtils.StrongRound(item.Bounds);
				bounds.Offset(offsetX, offsetY);
				if (maxRect.IsEmpty)
					maxRect = bounds;
				else
					maxRect = Rectangle.Union(maxRect, bounds);
			}
			return maxRect;
		}
		public void Render(IRenderer renderer, HitTestController hitTestController, Rectangle previousPrimaryItemBounds, Rectangle previousStaggeredItemBounds) {
			if (Count == 0)
				return;
			foreach (AxisLabelItemBase item in this)
				if (item.Visible) {
					if (!previousPrimaryItemBounds.IsEmpty) {
						Rectangle bounds = previousPrimaryItemBounds;
						previousPrimaryItemBounds = Rectangle.Empty;
						if (bounds.IntersectsWith(item.RoundedBounds))
							continue;
					}
					if (!previousStaggeredItemBounds.IsEmpty) {
						Rectangle bounds = previousStaggeredItemBounds;
						previousStaggeredItemBounds = Rectangle.Empty;
						if (bounds.IntersectsWith(item.RoundedBounds))
							continue;
					}
					item.Render(renderer, hitTestController);
				}
		}
		public void Render(IRenderer renderer, HitTestController hitTestController) {
			 Render(renderer, hitTestController, Rectangle.Empty, Rectangle.Empty);
		}
		void RaiseCustomDrawEvents(AxisBase axis) {
			ChartContainerAdapter containerAdapter = axis.ContainerAdapter;
			foreach (AxisLabelItemBase item in this)
				containerAdapter.OnCustomDrawAxisLabel(new CustomDrawAxisLabelEventArgs(item));
		}
	}
}
