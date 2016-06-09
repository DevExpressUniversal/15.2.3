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
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(Pie3DSeriesViewTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class Pie3DSeriesView : PieSeriesViewBase, ISimpleDiagram3DSeriesView {
		const int DefaultDepth = 15;
		int depth = DefaultDepth;
		PolygonFillStyle3D fillStyle;
		Pie3DSeriesViewAppearance Appearance {
			get {
				IChartAppearance actualAppearance = CommonUtils.GetActualAppearance(this); 
				return actualAppearance.Pie3DSeriesViewAppearance;
			}
		}
		protected override bool Is3DView { get { return true; } }
		protected override Type PointInterfaceType {
			get {
				return typeof(IValuePoint);
			}
		}
		internal PolygonFillStyle3D ActualFillStyle {
			get {
				if (fillStyle.FillMode != FillMode3D.Empty)
					return fillStyle;
				else
					return Appearance.FillStyle;
			}
		}		
		protected internal override string StringId { get { return ChartLocalizer.GetString(ChartStringId.SvnPie3D); } }
		protected internal override bool HitTestingSupportedForLegendMarker { get { return false; } }
		protected internal override int DepthPercent { get { return depth; } }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public override Type DiagramType { get { return typeof(SimpleDiagram3D); } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new Color Color { get { return Color.Empty; } set {} }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Pie3DSeriesViewDepth"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Pie3DSeriesView.Depth"),
		Category("Appearance"),
		XtraSerializableProperty
		]
		public int Depth { 
			get { return depth; }
			set { 
				if (value != depth) {
					if (value <= 0 || value > 100)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPieDepth));
					SendNotification(new ElementWillChangeNotification(this));
					depth = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Pie3DSeriesViewSizeAsPercentage"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Pie3DSeriesView.SizeAsPercentage"),
		]
		public new double SizeAsPercentage { 
			get { return base.SizeAsPercentage; }
			set { base.SizeAsPercentage = value; }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Pie3DSeriesViewPieFillStyle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Pie3DSeriesView.PieFillStyle"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public PolygonFillStyle3D PieFillStyle { get { return this.fillStyle; } }
		public Pie3DSeriesView() : base() {
			Initialize();
		}
		public Pie3DSeriesView(int[] explodedPointIds) : base(explodedPointIds) {
			Initialize();
		}
		#region ISimpleDiagram3DSeriesView implementation
		double ISimpleDiagram3DSeriesView.DepthFactor { get { return depth / 100.0; } }
		double ISimpleDiagram3DSeriesView.HeightToWidthRatio { get { return 1.0; } }
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if(propertyName == "Depth")
				return ShouldSerializeDepth();
			if(propertyName == "PieFillStyle")
				return ShouldSerializePieFillStyle();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeDepth() {
			return depth != DefaultDepth;
		}
		void ResetDepth() {
			Depth = DefaultDepth;
		}
		bool ShouldSerializePieFillStyle() {
			return PieFillStyle.ShouldSerialize();
		}
		new bool ShouldSerializeSizeAsPercentage() {
			return base.ShouldSerializeSizeAsPercentage();
		}
		new void ResetSizeAsPercentage() {
			base.ResetSizeAsPercentage();
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeDepth() ||
				ShouldSerializePieFillStyle();
		}
		#endregion
		void Initialize() {
			this.fillStyle = new PolygonFillStyle3D(this);
		}
		protected override DiagramPoint? CalculateAnnotationAchorPoint(ISimpleDiagramDomain domain, SeriesPointLayout pointLayout) {
			PieSeriesPointLayout pieLayout = pointLayout as PieSeriesPointLayout;
			if (pieLayout == null)
				return null;
			RefinedPointData pointData = pieLayout.PointData;
			IPiePoint pointInfo = pointData.RefinedPoint;
			SimpleDiagram3DDomain domain3D = domain as SimpleDiagram3DDomain;
			if (pointInfo == null || pointInfo.NormalizedValue == 0 || domain3D == null)
				return null;
			float inflateSize = -pieLayout.Pie.MajorSemiaxis * Math.Min(PieGraphicsCommand.FacetPercent, 1.0f - pieLayout.Pie.HoleFraction);
			RectangleF rect = GraphicUtils.InflateRect(pieLayout.PieBounds, inflateSize, inflateSize);
			if (!rect.AreWidthAndHeightPositive())
				return null;
			Ellipse realEllipse = new Ellipse(pieLayout.Pie.CalculateCenter(pieLayout.BasePoint), rect.Width / 2, rect.Height / 2);
			GRealPoint2D finishPoint = realEllipse.CalcEllipsePoint(pieLayout.Pie.HalfAngle);
			DiagramPoint labelPoint = new DiagramPoint(finishPoint.X - realEllipse.Center.X, realEllipse.Center.Y - finishPoint.Y);
			labelPoint.X += (realEllipse.Center.X - pieLayout.BasePoint.X);
			labelPoint.Y -= (realEllipse.Center.Y - pieLayout.BasePoint.Y);
			DiagramPoint p1 = domain3D.Project(new DiagramPoint(labelPoint.X, labelPoint.Y));
			DiagramPoint p2 = domain3D.Project(new DiagramPoint(labelPoint.X, labelPoint.Y, 1.0));
			double depth = pieLayout.Pie.MajorSemiaxis * DepthPercent * 0.02;
			double z = p2.Z < p1.Z ? depth * 0.5 : -depth * 0.5;
			return domain3D.Project(new DiagramPoint(labelPoint.X, labelPoint.Y, z));			
		}
		protected override SimpleDiagramSeriesLayoutBase CreateSimpleDiagramSeriesLayout() {
			SimpleDiagram3DSeriesLayout layout;
			using (TextMeasurer textMeasurer = new TextMeasurer()) {
				RefinedSeriesData seriesData = new RefinedSeriesData(Chart.ViewController.FindRefinedSeries(Series), textMeasurer, null);
				GRect2D displayBounds = GraphicUtils.ConvertRect(Chart.ContainerAdapter.DisplayBounds);
				layout = new SimpleDiagram3DSeriesLayout(seriesData, textMeasurer, displayBounds);
			}
			return layout;
		}
		protected override DrawOptions CreateSeriesDrawOptionsInternal() {
			return new Pie3DDrawOptions(this);
		}
		protected override ChartElement CreateObjectForClone() {
			return new Pie3DSeriesView();
		}
		protected override GraphicsCommand CreateGraphicsCommand(Pie pie, PointF basePoint, bool negativeValuesPresents, SelectionState selectionState, int borderThickness, SimpleDiagramDrawOptionsBase drawOptions) {
			Pie3DDrawOptions pieDrawOptions = drawOptions as Pie3DDrawOptions;
			if (pieDrawOptions == null || !pie.ShouldCreateGraphicsCommand())
				return null;
			Color color = pieDrawOptions.Color;
			if (negativeValuesPresents) {
				color = HitTestColors.MixColors(Color.FromArgb(150, 255, 255, 255), pieDrawOptions.Color);
				pieDrawOptions.FillStyle.FillMode = FillMode3D.Solid;
			}
			return pie.CreateGraphicsCommand(color, pieDrawOptions.ActualColor2, pieDrawOptions.FillStyle, PointF.Empty);
		}
		protected override double GetRefinedPointMax(RefinedPoint point) {
			return ((IValuePoint)point).Value;
		}
		protected override double GetRefinedPointMin(RefinedPoint point) {
			return ((IValuePoint)point).Value;
		}
		protected override double GetRefinedPointAbsMin(RefinedPoint point) {
			return Math.Abs(((IValuePoint)point).Value);
		}
		protected override void Render(IRenderer renderer, Pie pie, PointF basePoint, bool negativeValuesPresents, SelectionState selectionState, int borderThickness, SimpleDiagramDrawOptionsBase drawOptions) {
			Pie3DDrawOptions pieDrawOptions = (Pie3DDrawOptions)drawOptions;
			Color color = pieDrawOptions.Color;
			if (negativeValuesPresents) {
				color = HitTestColors.MixColors(Color.FromArgb(150, 255, 255, 255), pieDrawOptions.Color);
				pieDrawOptions.FillStyle.FillMode = FillMode3D.Solid;
			}
			pie.Render(renderer, color, pieDrawOptions.ActualColor2, pieDrawOptions.FillStyle, PointF.Empty);
		}
		protected internal override SeriesLabelBase CreateSeriesLabel() {
			return new Pie3DSeriesLabel();
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			Pie3DSeriesView view = obj as Pie3DSeriesView;
			if(view == null)
				return;
			depth = view.depth;
			fillStyle.Assign(view.fillStyle);
		}
		public override bool Equals(object obj) {
			if(!base.Equals(obj))
				return false;
			Pie3DSeriesView view = (Pie3DSeriesView)obj;
			return
				this.depth == view.depth &&
				this.fillStyle.Equals(view.fillStyle);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
