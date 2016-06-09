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
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.GLGraphics;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(Funnel3DSeriesViewTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class Funnel3DSeriesView : FunnelSeriesViewBase, ISimpleDiagram3DSeriesView {
		const int DefaultHoleRadiusPercent = 90;
		internal static DiagramPoint CalcSurfacePoint(double y, double radius, double angle) {
			return new DiagramPoint(radius * Math.Sin(angle), y, radius * Math.Cos(angle));
		}
		int holeRadiusPercent = DefaultHoleRadiusPercent;
		new Funnel3DSeriesLabel Label { get { return (Funnel3DSeriesLabel)base.Label; } }
		protected internal override string StringId { get { return ChartLocalizer.GetString(ChartStringId.SvnFunnel3D); } }
		protected internal override bool HitTestingSupportedForLegendMarker { get { return false; } }
		protected override bool Is3DView { get { return true; } }
		protected override Type PointInterfaceType {
			get {
				return typeof(IValuePoint);
			}
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public override Type DiagramType { get { return typeof(FunnelDiagram3D); } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new Color Color { get { return Color.Empty; } set { } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Funnel3DSeriesViewHoleRadiusPercent"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Funnel3DSeriesView.HoleRadiusPercent"),
		XtraSerializableProperty
		]
		public int HoleRadiusPercent {
			get { return holeRadiusPercent; }
			set {
				if (value != holeRadiusPercent) {
					if (value < 0 || value > 100)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectFunnelHolePercent));
					SendNotification(new ElementWillChangeNotification(this));
					holeRadiusPercent = value;
					RaiseControlChanged();
				}
			}
		}
		#region ISimpleDiagram3DSeriesView implementation
		double ISimpleDiagram3DSeriesView.DepthFactor { get { return 1.0; } }
		double ISimpleDiagram3DSeriesView.HeightToWidthRatio { get { return HeightToWidthRatio; } }
		#endregion
		public Funnel3DSeriesView() : base() {
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "HoleRadiusPercent")
				return ShouldSerializeHoleRadiusPercent();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeHoleRadiusPercent() {
			return holeRadiusPercent != DefaultHoleRadiusPercent;
		}
		void ResetHoleRadiusPercent() {
			HoleRadiusPercent = DefaultHoleRadiusPercent;
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeHoleRadiusPercent();
		}
		#endregion        
		Rectangle CalcLabelBounds(Rectangle bounds, int maxLabelWidth, int maxLabelHeight) {
			if (bounds.Height < bounds.Width) {
				int offset = MathUtils.StrongRound((bounds.Width - bounds.Height) / 2.0);
				bounds.Width = bounds.Height;
				bounds.Offset(offset, 0);
			}
			else {
				int offset = MathUtils.StrongRound((bounds.Height - bounds.Width) / 2.0);
				bounds.Height = bounds.Width;
				bounds.Offset(0, offset);
			}
			Rectangle result = Rectangle.Inflate(bounds, maxLabelWidth, maxLabelHeight);
			return result;			
		}
		protected override DiagramPoint? CalculateAnnotationAchorPoint(ISimpleDiagramDomain domain, SeriesPointLayout pointLayout) {
			FunnelSeriesPointLayout layout = pointLayout as FunnelSeriesPointLayout;
			SimpleDiagram3DDomain domain3D = domain as SimpleDiagram3DDomain;
			if (layout == null || domain3D == null)
				return null;
			double y = domain3D.Bounds.Height / 2 - (layout.RightUpPoint.Y + layout.RightDownPoint.Y) / 2;
			double radius = (layout.RightUpPoint.X + layout.RightDownPoint.X - layout.LeftUpPoint.X - layout.LeftDownPoint.X) / 4;
			DiagramPoint point = new DiagramPoint(0, y, -1);
			DiagramPoint axledPoint = GLHelper.InverseTransform(domain3D.ModelViewMatrix, new DiagramPoint(0, point.Y, 0));
			DiagramPoint surfacePoint = GLHelper.InverseTransform(domain3D.ModelViewMatrix, point);
			double correctionAngle = Math.Atan2(axledPoint.X - surfacePoint.X, axledPoint.Z - surfacePoint.Z);
			return domain3D.Project(CalcSurfacePoint(y, radius, correctionAngle));
		}
		protected override DrawOptions CreateSeriesDrawOptionsInternal() {
			return new Funnel3DDrawOptions(this);
		}
		protected override ChartElement CreateObjectForClone() {
			return new Funnel3DSeriesView();
		}
		protected internal override SeriesLabelBase CreateSeriesLabel() {
			return new Funnel3DSeriesLabel();
		}
		protected override GraphicsCommand CreateGraphicsCommand(FunnelSeriesPointLayout layout, SimpleDiagramDrawOptionsBase drawOptions, Rectangle mappingBounds) {
			if (layout != null) {
				Funnel3DDrawOptions funnelDrawOptions = (Funnel3DDrawOptions)drawOptions;
				Color color = funnelDrawOptions.Color;
				if (layout.IsNegativeValuePresents) 
					color = HitTestColors.MixColors(Color.FromArgb(150, 255, 255, 255), color);
				float startRadius = (float)((layout.RightUpPoint.X - layout.LeftUpPoint.X) / 2);
				if (startRadius < 1)
					startRadius = 1;
				float finishRadius = (float)((layout.RightDownPoint.X - layout.LeftDownPoint.X) / 2);
				if (finishRadius < 1)
					finishRadius = 1;
				bool startClosed = layout.PointOrder == PointOrder.First || layout.PointOrder == PointOrder.Single || PointDistance > 0;
				bool finishClosed = layout.PointOrder == PointOrder.Last || layout.PointOrder == PointOrder.Single || PointDistance > 0;
				return new ConeGraphicsCommand((float)(layout.LeftUpPoint.Y - mappingBounds.Height / 2 ), (float)(layout.LeftDownPoint.Y - mappingBounds.Height / 2), startRadius, finishRadius, color, startClosed, finishClosed, holeRadiusPercent);
			}
			return null;
		}
		protected internal override void Render(IRenderer renderer, Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
		}
		protected override void Render(IRenderer renderer, FunnelSeriesPointLayout layout, SimpleDiagramDrawOptionsBase drawOptions, Rectangle mappingBounds) {
		}
		protected override void RenderLegendMarker(IRenderer renderer, VariousPolygon polygon, SimpleDiagramDrawOptionsBase drawOptions, SelectionState selectionState) {
			Funnel3DDrawOptions funnelDrawOptions = drawOptions as Funnel3DDrawOptions;
			if (funnelDrawOptions == null)
				return;
			Color color = funnelDrawOptions.Color;
			LineStrip vertices = polygon.Vertices;
			vertices[2] = new GRealPoint2D(vertices[2].X, vertices[2].Y - 1);
			renderer.FillPolygon(vertices, color);
			renderer.EnableAntialiasing(true);
			renderer.DrawLine(new Point((int)polygon.Vertices[1].X - 1, (int)polygon.Vertices[1].Y), new Point((int)polygon.Vertices[2].X - 1, (int)polygon.Vertices[2].Y), color, 1);
			renderer.DrawLine(new Point((int)polygon.Vertices[0].X, (int)polygon.Vertices[0].Y), new Point((int)polygon.Vertices[5].X, (int)polygon.Vertices[5].Y - 1), color, 1);
			renderer.RestoreAntialiasing();
		}
		protected override PointF CalculateBasePoint(ISimpleDiagramDomain domain, Rectangle correctedBounds) {
			return new PointF(domain.ElementBounds.X + domain.ElementBounds.Width / 2.0f, correctedBounds.Y - domain.Bounds.Y);
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
		protected internal override bool CalculateBounds(RefinedSeriesData seriesData, Rectangle outerBounds, out Rectangle funnelBounds, out Rectangle labelBounds) {
			funnelBounds = outerBounds;
			labelBounds  = outerBounds;
			if (!outerBounds.AreWidthAndHeightPositive())
				return false;
			Size maximumLabelSize = Label.CalculateMaximumSizeConsiderIndent(seriesData);
			funnelBounds = Rectangle.Inflate(funnelBounds, -maximumLabelSize.Width, -maximumLabelSize.Height);
			if (!funnelBounds.AreWidthAndHeightPositive())
				return false;
			labelBounds = CalcLabelBounds(funnelBounds, maximumLabelSize.Width, maximumLabelSize.Height);
			return true;
		}		
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			Funnel3DSeriesView view = obj as Funnel3DSeriesView;
			if (view == null)
				return;
			holeRadiusPercent = view.holeRadiusPercent;			
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			Funnel3DSeriesView view = (Funnel3DSeriesView)obj;
			return holeRadiusPercent == view.holeRadiusPercent;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
