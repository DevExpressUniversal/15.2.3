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
using System.Windows.Media.Media3D;
using DevExpress.Utils;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public sealed class SeriesPointAnimation : ChartAnimation {
		static readonly DependencyPropertyKey ActionsPropertyKey = DependencyProperty.RegisterReadOnly("Actions", 
			typeof(SeriesPointAnimationActionCollection), typeof(SeriesPointAnimation), new PropertyMetadata());
		public static readonly DependencyProperty ActionsProperty = ActionsPropertyKey.DependencyProperty;
		public static readonly DependencyProperty TargetSeriesPointProperty = DependencyProperty.Register("TargetSeriesPoint", 
			typeof(SeriesPoint), typeof(SeriesPointAnimation), new PropertyMetadata(null, new PropertyChangedCallback(TargetSeriesPointChanged)));
		public static readonly DependencyProperty InitialNumericalArgumentProperty = DependencyProperty.Register("InitialNumericalArgument", 
			typeof(double?), typeof(SeriesPointAnimation), new PropertyMetadata(null));
		public static readonly DependencyProperty InitialNumericalValueProperty = DependencyProperty.Register("InitialNumericalValue", 
			typeof(double?), typeof(SeriesPointAnimation), new PropertyMetadata(null));
		public static readonly DependencyProperty InitialRotationProperty = DependencyProperty.Register("InitialRotation", 
			typeof(AnimationRotation), typeof(SeriesPointAnimation), new PropertyMetadata(new AnimationRotation()));
		static void TargetSeriesPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((SeriesPointAnimation)d).Initialize((SeriesPoint)e.NewValue);
		}
		SeriesPoint actualSeriesPoint;
		double? initialNumericalArgument;
		double? initialNumericalValue;
		Transform3D initialTransform;
		internal SeriesPoint ActualSeriesPoint { get { return actualSeriesPoint; } }
		internal double? ActualInitialNumericalArgument { get { return initialNumericalArgument; } }
		internal double? ActualInitialNumericalValue { get { return initialNumericalValue; } }
		internal Transform3D ActualInitialTransform { get { return initialTransform; } }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesPointAnimationActions"),
#endif
		Category(Categories.Elements)
		]
		public SeriesPointAnimationActionCollection Actions {
			get { return (SeriesPointAnimationActionCollection)GetValue(ActionsProperty); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesPointAnimationTargetSeriesPoint"),
#endif
		Category(Categories.Behavior)
		]
		public SeriesPoint TargetSeriesPoint {
			get { return (SeriesPoint)GetValue(TargetSeriesPointProperty); }
			set { SetValue(TargetSeriesPointProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesPointAnimationInitialNumericalArgument"),
#endif
		Category(Categories.Behavior),
		NonTestableProperty
		]
		public double? InitialNumericalArgument {
			get { return (double?)GetValue(InitialNumericalArgumentProperty); }
			set { SetValue(InitialNumericalArgumentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesPointAnimationInitialNumericalValue"),
#endif
		Category(Categories.Behavior),
		NonTestableProperty
		]
		public double? InitialNumericalValue {
			get { return (double?)GetValue(InitialNumericalValueProperty); }
			set { SetValue(InitialNumericalValueProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesPointAnimationInitialRotation"),
#endif
		Category(Categories.Behavior)
		]
		public AnimationRotation InitialRotation {
			get { return (AnimationRotation)GetValue(InitialRotationProperty); }
			set { SetValue(InitialRotationProperty, value); }
		}
		public SeriesPointAnimation() {
			SetValue(ActionsPropertyKey, ChartElementHelper.CreateInstance<SeriesPointAnimationActionCollection>(this));
		}
		protected internal override void Initialize() {
			Initialize(TargetSeriesPoint);
		}
		protected internal override void PerformAnimation(double value) {
			foreach (SeriesPointAnimationAction action in Actions)
				action.PerformAnimation(value);
		}
		void Initialize(SeriesPoint seriesPoint) {
			if (seriesPoint != actualSeriesPoint)
				PerformAnimation(1.0);
			actualSeriesPoint = seriesPoint;
			if (seriesPoint == null)
				return;
			if (seriesPoint.Series.ActualArgumentScaleType == ScaleType.Numerical) {
				initialNumericalArgument = InitialNumericalArgument;
				if (initialNumericalArgument == null)
					initialNumericalArgument = ((ISeriesPoint)seriesPoint).NumericalArgument;
			}
			else
				initialNumericalArgument = null;
			initialNumericalValue = InitialNumericalValue;
			if (initialNumericalValue == null)
				initialNumericalValue = ((ISeriesPoint)seriesPoint).UserValues[0];
			if (InitialRotation.Order == AnimationRotationOrder.None) {
				initialTransform = SeriesPointModel3D.GetTransform(seriesPoint);
				if (initialTransform == null)
					initialTransform = new TranslateTransform3D();
			}
			else
				initialTransform = null;
		}
	}
	public class SeriesPointAnimationActionCollection : ChartElementCollection<SeriesPointAnimationAction> { }
	public abstract class SeriesPointAnimationAction : ChartElement {
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool Focusable { get { return base.Focusable; } set { base.Focusable = value; } }
		protected SeriesPointAnimation Animation {
			get { return (SeriesPointAnimation)((IChartElement)this).Owner; }
		}
		protected SeriesPoint SeriesPoint { get { return Animation.ActualSeriesPoint; } }
		protected internal abstract void PerformAnimation(double value);
	}
	public sealed class SeriesPointRotationAction : SeriesPointAnimationAction, IRotationAnimation {
		public static readonly DependencyProperty RotationProperty = DependencyProperty.Register("Rotation", 
			typeof(AnimationRotation), typeof(SeriesPointRotationAction), new PropertyMetadata(new AnimationRotation()));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesPointRotationActionRotation"),
#endif
		Category(Categories.Behavior)
		]
		public AnimationRotation Rotation {
			get { return (AnimationRotation)GetValue(RotationProperty); }
			set { SetValue(RotationProperty, value); }
		}
		#region IRotationAnimation implementation
		Transform3D IRotationAnimation.InitialTransform { get { return Animation.ActualInitialTransform; } }
		Transform3D IRotationAnimation.Transform { set { SeriesPointModel3D.SetTransform(SeriesPoint, value); } }
		AnimationRotation IRotationAnimation.InitialRotation { get { return Animation.InitialRotation; } }
		AnimationRotation IRotationAnimation.Rotation { get { return Rotation; } }
		#endregion
		protected internal override void PerformAnimation(double value) {
			if (SeriesPoint != null)
				RotationAnimationHelper.PerformAnimation(this, value);
		}
	}
}
