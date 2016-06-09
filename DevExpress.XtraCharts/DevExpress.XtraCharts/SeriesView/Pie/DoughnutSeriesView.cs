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
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.ModelSupport;
namespace DevExpress.XtraCharts {
   	[
	TypeConverter(typeof(DoughnutSeriesViewTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class DoughnutSeriesView : PieSeriesView, IDoughnutSeriesView {
		const int DefaultHolePercent = 60;
		int holeRadiusPercent;
		protected internal override string StringId { get { return ChartLocalizer.GetString(ChartStringId.SvnDoughnut); } }
		protected internal override int HolePercent { get { return holeRadiusPercent; } }
		protected internal override int LegendHolePercent { get { return DefaultHolePercent; } }
		protected virtual int DefaultHoleRadiusPercent { get { return DefaultHolePercent; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("DoughnutSeriesViewHoleRadiusPercent"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.DoughnutSeriesView.HoleRadiusPercent"),
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
					RaiseControlChanged(new SeriesGroupsInteractionUpdateInfo(this));
				}
			}
		}
		public DoughnutSeriesView() : base() {
			Initialize();
		}
		public DoughnutSeriesView(int[] explodedPointIds) : base(explodedPointIds) {
			Initialize();
		}
		void Initialize() {
			holeRadiusPercent = DefaultHoleRadiusPercent;
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
		DiagramPoint? CalculateAchorPoint(SeriesPointLayout pointLayout) {
			PieSeriesPointLayout pieLayout = pointLayout as PieSeriesPointLayout;
			if (pieLayout == null)
				return null;
			RefinedPointData pointData = pieLayout.PointData;
			IPiePoint pointInfo = pointData.RefinedPoint;
			if (pieLayout == null || pointInfo == null || pointInfo.NormalizedValue == 0)
				return null;
			RectangleF pieBounds = pieLayout.PieBounds;
			if (!pieBounds.AreWidthAndHeightPositive())
				return null;
			Ellipse actualEllipse = new Ellipse(pieLayout.Pie.CalculateCenter(pieLayout.BasePoint), pieBounds.Width / 2, pieBounds.Height / 2);
			return (DiagramPoint)actualEllipse.CalcEllipsePoint(pieLayout.Pie.HalfAngle);
		}
		protected override DiagramPoint? CalculateAnnotationAchorPoint(ISimpleDiagramDomain domain, SeriesPointLayout pointLayout) {
			return CalculateAchorPoint(pointLayout);
		}
		protected override ChartElement CreateObjectForClone() {
			return new DoughnutSeriesView();
		}
		protected internal override DiagramPoint? CalculateRelativeToolTipPosition(SeriesPointLayout pointLayout) {
			return CalculateAchorPoint(pointLayout);
		} 
		protected internal override SeriesLabelBase CreateSeriesLabel() {
			return new DoughnutSeriesLabel();
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
