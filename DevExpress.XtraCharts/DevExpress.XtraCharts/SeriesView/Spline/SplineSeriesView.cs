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
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Collections.Generic;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(SplineSeriesViewTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class SplineSeriesView : LineSeriesView, ISplineSeriesView {
		const int DefaultLineTensionPercent = 80;
		int lineTensionPercent = DefaultLineTensionPercent;
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SplineSeriesViewLineTensionPercent"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SplineSeriesView.LineTensionPercent"),
		XtraSerializableProperty
		]
		public int LineTensionPercent {
			get { return lineTensionPercent; }
			set {
				if (value > 100 || value < 0)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectLineTensionPercent));
				if (lineTensionPercent != value) {
					SendNotification(new ElementWillChangeNotification(this));
					lineTensionPercent = value;
					RaiseControlChanged();
				}
			}
		}
		protected internal override string StringId { get { return ChartLocalizer.GetString(ChartStringId.SvnSpline); } }
		double LineTension { get { return (double)lineTensionPercent / 100; } }
		public SplineSeriesView() : base() {
		}
		#region ISplineSeriesView
		bool ISplineSeriesView.ShouldCorrectRanges { get { return true; } }
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			return propertyName == "LineTensionPercent" ? ShouldSerializeLineTensionPercent() : base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeLineTensionPercent() {
			return lineTensionPercent != DefaultLineTensionPercent;
		}
		void ResetLineTensionPercent() {
			LineTensionPercent = DefaultLineTensionPercent;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeLineTensionPercent();
		}
		#endregion
		protected override IGeometryStrip CreateStripInternal() {
			return new BezierStrip(LineTension);
		}
		protected override GeometryStripCreator CreateStripCreator() {
			return new SplineGeometryStripCreator(LineTension, AxisX.ScaleTypeMap.Transformation, AxisY.ScaleTypeMap.Transformation);
		}
		protected override ChartElement CreateObjectForClone() {
			return new SplineSeriesView();
		}
		protected override PointSeriesViewPainter CreatePainter() {
			return new SplineSeriesViewPainter(this);
		}
		protected override LineWholeSeriesLayout CreateWholeSeriesLayout(SeriesLayout seriesLayout, List<IGeometryStrip> strips, int lineThickness, Rectangle bounds) {
			return new SplineWholeSeriesLayout(seriesLayout, strips, lineThickness, bounds, ((XYDiagramSeriesLayout)seriesLayout).SingleLayout);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			ISplineSeriesView view = obj as ISplineSeriesView;
			if (view == null)
				return;
			lineTensionPercent = view.LineTensionPercent;
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class SplineWholeSeriesLayout : LineWholeSeriesLayout {
		public SplineWholeSeriesLayout(SeriesLayout seriesLayout, List<IGeometryStrip> strips, int lineThickness, Rectangle bounds, bool optimizeHitTesting)
			: base(seriesLayout, strips, lineThickness, bounds, optimizeHitTesting) {
		}
		protected override void AddStrips(GraphicsPath path) {
			foreach (BezierStrip strip in Strips) {
				List<GRealPoint2D> points = strip.GetPointsForDrawing(true, true);
				ZPlaneRectangle boundingRectangle = ZPlaneRectangle.MakeRectangle(points);
				if (boundingRectangle.Width >= 0.5 || boundingRectangle.Height >= 0.5)
					path.AddBeziers(StripsUtils.Convert(points));
			}
		}
	}
}
