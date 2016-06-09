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
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public class LineStepSeries2D : LineSeries2D, IStepSeriesView {
		public static readonly DependencyProperty InvertedStepProperty = DependencyPropertyManager.Register("InvertedStep",
			typeof(bool), typeof(LineStepSeries2D), new PropertyMetadata(ChartElementHelper.Update));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("LineStepSeries2DInvertedStep"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool InvertedStep {
			get { return (bool)GetValue(InvertedStepProperty); }
			set { SetValue(InvertedStepProperty, value); }
		}
		public LineStepSeries2D() {
			DefaultStyleKey = typeof(LineStepSeries2D);
		}
		protected override void AssignForBinding(Series series) {
			base.AssignForBinding(series);
			LineStepSeries2D stepLine = series as LineStepSeries2D;
			if (stepLine != null)
				CopyPropertyValueHelper.CopyPropertyValue(this, stepLine, InvertedStepProperty);
		}
		protected override Series CreateObjectForClone() {
			return new LineStepSeries2D();
		}
		protected internal override AdditionalLineSeriesGeometry CreateAdditionalGeometry() {
			return new AdditionalStepLineSeriesGeometry(this);
		}
		protected override GeometryStripCreator CreateStripCreator() {
			return new StepLineGeometryStripCreator(InvertedStep);
		}
	}
}
