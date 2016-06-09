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
	public class SplineSeries2D : LineSeries2D {
		const double defaultLineTension = 0.8;
		double actualLineTension = defaultLineTension;
		public static readonly DependencyProperty LineTensionProperty = DependencyProperty.Register(
			"LineTension", typeof(double), typeof(SplineSeries2D), new PropertyMetadata(defaultLineTension, OnLineTensionChange));
		static void OnLineTensionChange(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			SplineSeries2D control = obj as SplineSeries2D;
			if (control != null) {
				control.ActualLineTension = (double)e.NewValue;
			}
			ChartElementHelper.Update(obj, e);
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SplineSeries2DLineTension"),
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
		public SplineSeries2D() {
			DefaultStyleKey = typeof(SplineSeries2D);
		}
		protected override void AssignForBinding(Series series) {
			base.AssignForBinding(series);
			SplineSeries2D spline = series as SplineSeries2D;
			if (spline != null)
				CopyPropertyValueHelper.CopyPropertyValue(this, spline, LineTensionProperty);
		}
		protected override Series CreateObjectForClone() {
			return new SplineSeries2D();
		}
		protected override GeometryStripCreator CreateStripCreator() {
			return new SplineGeometryStripCreator(ActualLineTension, ((IAxisData)this.ActualAxisX).AxisScaleTypeMap.Transformation, ((IAxisData)this.ActualAxisY).AxisScaleTypeMap.Transformation);
		}
		protected internal override AdditionalLineSeriesGeometry CreateAdditionalGeometry() {
			return new AdditionalSplineSeriesGeometry(this);
		}
	}
}
