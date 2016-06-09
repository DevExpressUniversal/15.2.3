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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Localization;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public class PointSeries2D : MarkerSeries2D, ISupportMarker2D {
		public static readonly DependencyProperty MarkerModelProperty = DependencyPropertyManager.Register("MarkerModel",
			typeof(Marker2DModel), typeof(PointSeries2D), new PropertyMetadata(ChartElementHelper.Update));
		public static readonly DependencyProperty MarkerSizeProperty = DependencyPropertyManager.Register("MarkerSize",
			typeof(int), typeof(PointSeries2D), new PropertyMetadata(10, ChartElementHelper.Update), MarkerSizeValidation);
		public static readonly DependencyProperty PointAnimationProperty = DependencyPropertyManager.Register("PointAnimation",
			typeof(Marker2DAnimationBase), typeof(PointSeries2D), new PropertyMetadata(null, PointAnimationPropertyChanged));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("PointSeries2DMarkerSize"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public int MarkerSize {
			get { return (int)GetValue(MarkerSizeProperty); }
			set { SetValue(MarkerSizeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("PointSeries2DMarkerModel"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public Marker2DModel MarkerModel {
			get { return (Marker2DModel)GetValue(MarkerModelProperty); }
			set { SetValue(MarkerModelProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("PointSeries2DPointAnimation"),
#endif
		Category(Categories.Animation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public Marker2DAnimationBase PointAnimation {
			get { return (Marker2DAnimationBase)GetValue(PointAnimationProperty); }
			set { SetValue(PointAnimationProperty, value); }
		}
		protected internal override bool ArePointsVisible { get { return true; } }
		protected internal override VisualSelectionType SupportedLegendMarkerSelectionType { get { return VisualSelectionType.Hatch | base.SupportedLegendMarkerSelectionType; } }
		protected override int PixelsPerArgument { get { return this.MarkerSize; } }
		public PointSeries2D() {
			DefaultStyleKey = typeof(PointSeries2D);
		}
		#region ISupportMarker2D implementation
		bool ISupportMarker2D.MarkerVisible {
			get { return true; }
			set {
				string message = String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPropertyUsage), "MarkerVisible", "PointSeries2D");
				throw new InvalidOperationException(message);
			}
		}
		#endregion
		protected override Series CreateObjectForClone() {
			return new PointSeries2D();
		}
		protected override void AssignForBinding(Series series) {
			base.AssignForBinding(series);
			PointSeries2D pointSeries2D = series as PointSeries2D;
			if (pointSeries2D != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, pointSeries2D, MarkerSizeProperty);
				if (CopyPropertyValueHelper.IsValueSet(pointSeries2D, MarkerModelProperty))
					if (CopyPropertyValueHelper.VerifyValues(this, pointSeries2D, MarkerModelProperty))
						MarkerModel = pointSeries2D.MarkerModel.CloneModel();
				if (CopyPropertyValueHelper.IsValueSet(pointSeries2D, PointAnimationProperty))
					if (CopyPropertyValueHelper.VerifyValues(this, pointSeries2D, PointAnimationProperty))
						PointAnimation = pointSeries2D.PointAnimation.CloneAnimation() as Marker2DAnimationBase;
			}
		}
		protected internal override PointModel GetModel(RangeValueLevel valueLevel) {
			return MarkerModel;
		}
		protected override int CalculateMarkerSize(PaneMapping mapping, RefinedPoint pointInfo) {
			return MarkerSize;
		}
		protected internal override SeriesPointAnimationBase CreateDefaultPointAnimation() {
			return new Marker2DSlideFromTopAnimation();
		}
		protected override void FillPredefinedPointAnimationKinds(List<AnimationKind> animationKinds) {
			FillMarkerAnimationKinds(animationKinds);
		}
		public override SeriesPointAnimationBase GetPointAnimation() { return PointAnimation; }
		public override void SetPointAnimation(SeriesPointAnimationBase value) {
			if (value != null && !(value is Marker2DAnimationBase))
				return;
			PointAnimation = value as Marker2DAnimationBase;
		}
	}
}
