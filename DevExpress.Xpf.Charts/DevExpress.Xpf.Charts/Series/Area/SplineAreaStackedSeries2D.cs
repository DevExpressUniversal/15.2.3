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

using System.ComponentModel;
using System.Windows;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts {
	[
	TemplatePart(Name = "PART_AdditionalGeometryHolder", Type = typeof(ChartContentPresenter)),
	TemplatePart(Name = "PART_PointsPanel", Type = typeof(SimplePanel)),
	]
	public class SplineAreaStackedSeries2D : AreaStackedSeries2D, IStackedSplineView, IGeometryStripCreator {
		const double defaultLineTension = 0.8;
		double actualLineTension = defaultLineTension;
		public static readonly DependencyProperty LineTensionProperty = DependencyProperty.Register(
			"LineTension", typeof(double), typeof(SplineAreaStackedSeries2D), new PropertyMetadata(defaultLineTension, OnLineTensionChange));
		static void OnLineTensionChange(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			SplineAreaStackedSeries2D control = obj as SplineAreaStackedSeries2D;
			if (control != null) {
				control.ActualLineTension = (double)e.NewValue;
			}
			ChartElementHelper.Update(obj, e);
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SplineAreaStackedSeries2DLineTension"),
#endif
		Category(Categories.Appearance),
		XtraSerializableProperty
		]
		public double LineTension {
			get {
				return (double)GetValue(LineTensionProperty);
			}
			set {
				SetValue(LineTensionProperty, value);
			}
		}
		double ActualLineTension {
			get {
				return actualLineTension;
			}
			set {
				if (!double.IsNaN(value) && 0 <= value && value <= 1)
					actualLineTension = value;
				else
					actualLineTension = defaultLineTension;
			}
		}
		public SplineAreaStackedSeries2D() {
			DefaultStyleKey = typeof(SplineAreaStackedSeries2D);
		}
		protected override void AssignForBinding(Series series) {
			base.AssignForBinding(series);
			SplineAreaStackedSeries2D spline = series as SplineAreaStackedSeries2D;
			if (spline != null)
				CopyPropertyValueHelper.CopyPropertyValue(this, spline, LineTensionProperty);
		}
		protected override Series CreateObjectForClone() {
			return new SplineAreaStackedSeries2D();
		}
		protected override GeometryStripCreator CreateStripCreator() {
			return new SplineStackedAreaGeometryStripCreator(ActualLineTension);
		}
		protected internal override AdditionalLineSeriesGeometry CreateAdditionalGeometry() {
			return new AdditionalSplineStackedAreaSeriesGeometry(this);
		}
		#region ISplineView
		int ISplineView.LineTensionPercent {
			get {
				return (int)(ActualLineTension * 100);
			}
			set {
				ActualLineTension = ((double)value) / 100;
			}
		}
		#endregion
		#region IGeometryStripCreator
		IGeometryStrip IGeometryStripCreator.CreateStrip() {
			return new BezierRangeStrip(ActualLineTension);
		}
		#endregion
	}
}
