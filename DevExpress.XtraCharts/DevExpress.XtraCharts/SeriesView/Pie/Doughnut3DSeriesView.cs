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
	TypeConverter(typeof(Doughnut3DSeriesViewTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class Doughnut3DSeriesView : Pie3DSeriesView, IDoughnutSeriesView {
		const int DefaultHoleRadiusPercent = 60;
		int holeRadiusPercent = DefaultHoleRadiusPercent;
		protected internal override string StringId { get { return ChartLocalizer.GetString(ChartStringId.SvnDoughnut3D); } }
		protected internal override int HolePercent { get { return holeRadiusPercent; } }
		protected internal override int LegendHolePercent { get { return DefaultHoleRadiusPercent; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Doughnut3DSeriesViewHoleRadiusPercent"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Doughnut3DSeriesView.HoleRadiusPercent"),
		XtraSerializableProperty
		]
		public int HoleRadiusPercent {
			get { return holeRadiusPercent; }
			set {
				if (value != holeRadiusPercent) {
					if (value < 0 || value > 100)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectDoughnutHolePercent));
					SendNotification(new ElementWillChangeNotification(this));
					holeRadiusPercent = value;
					RaiseControlChanged();
				}
			}
		}
		public Doughnut3DSeriesView() : base() {
		}
		public Doughnut3DSeriesView(int[] explodedPointIds) : base(explodedPointIds) {
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if(propertyName == "HoleRadiusPercent")
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
		protected override DiagramPoint? CalculateAnnotationAchorPoint(ISimpleDiagramDomain domain, SeriesPointLayout pointLayout) {
			PieSeriesPointLayout pieLayout = pointLayout as PieSeriesPointLayout;
			RefinedPointData pointData = pointLayout.PointData;
			IPiePoint pointInfo = pointData.RefinedPoint;
			SimpleDiagram3DDomain domain3D = domain as SimpleDiagram3DDomain;
			if (pieLayout == null || pointInfo == null || pointInfo.NormalizedValue == 0 || domain3D == null)
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
		protected override ChartElement CreateObjectForClone() {
			return new Doughnut3DSeriesView();
		}
		protected internal override SeriesLabelBase CreateSeriesLabel() {
			return new Doughnut3DSeriesLabel();
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			IDoughnutSeriesView view = obj as IDoughnutSeriesView;
			if (view == null)
				return;
			holeRadiusPercent = view.HoleRadiusPercent;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			IDoughnutSeriesView view = (IDoughnutSeriesView)obj;
			return holeRadiusPercent == view.HoleRadiusPercent;
		}
	}
}
