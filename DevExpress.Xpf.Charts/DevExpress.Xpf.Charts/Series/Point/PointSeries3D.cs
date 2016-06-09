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
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public class PointSeries3D : MarkerSeries3D {
		public static readonly DependencyProperty MarkerSizeProperty;
		static PointSeries3D() {
			MarkerSizeProperty = DependencyProperty.Register("MarkerSize", typeof(double), typeof(PointSeries3D),
				new FrameworkPropertyMetadata(0.6, ChartElementHelper.UpdateWithClearDiagramCache),
				new ValidateValueCallback(MarkerSizeValidation));
		}
		static bool MarkerSizeValidation(object size) {
			return (double)size > 0;
		}
		protected internal override double SeriesDepth { get { return MarkerSize; } }
		protected override double Size { get { return SeriesDepth; } }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("PointSeries3DMarkerSize"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public double MarkerSize {
			get { return (double)GetValue(MarkerSizeProperty); }
			set { SetValue(MarkerSizeProperty, value); }
		}
		public PointSeries3D() {
			DefaultStyleKey = typeof(PointSeries3D);
		}
		protected override Series CreateObjectForClone() {
			return new PointSeries3D();
		}
		protected internal override SeriesData CreateSeriesData() {
			SeriesData = new PointSeries3DData(this);
			return SeriesData;
		}
		protected override void AssignForBinding(Series series) {
			base.AssignForBinding(series);
			PointSeries3D pointSeries3D = series as PointSeries3D;
			if (pointSeries3D != null)
				CopyPropertyValueHelper.CopyPropertyValue(this, pointSeries3D, MarkerSizeProperty);
		} 
	}
}
