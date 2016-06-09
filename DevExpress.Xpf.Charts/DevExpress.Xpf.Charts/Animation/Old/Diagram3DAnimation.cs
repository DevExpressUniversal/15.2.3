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
using System.Windows;
using System.Windows.Media.Media3D;
using DevExpress.Utils;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public sealed class Diagram3DAnimation : ChartAnimation {
		static readonly DependencyPropertyKey ActionsPropertyKey = DependencyProperty.RegisterReadOnly("Actions", 
			typeof(Diagram3DAnimationActionCollection), typeof(Diagram3DAnimation), new PropertyMetadata());
		public static readonly DependencyProperty ActionsProperty = ActionsPropertyKey.DependencyProperty;
		public static readonly DependencyProperty InitialPerspectiveAngleProperty = DependencyProperty.Register("InitialPerspectiveAngle", 
			typeof(double), typeof(Diagram3DAnimation), new PropertyMetadata(Double.NaN), new ValidateValueCallback(ValidatePerspectiveAngle));
		public static readonly DependencyProperty InitialZoomPercentProperty = DependencyProperty.Register("InitialZoomPercent", 
			typeof(double), typeof(Diagram3DAnimation), new PropertyMetadata(Double.NaN), new ValidateValueCallback(ValidateZoomPercent));
		public static readonly DependencyProperty InitialVerticalScrollPercentProperty = DependencyProperty.Register("InitialVerticalScrollPercent",
			typeof(double), typeof(Diagram3DAnimation), new PropertyMetadata(Double.NaN), new ValidateValueCallback(ValidateScrollPercent));
		public static readonly DependencyProperty InitialHorizontalScrollPercentProperty = DependencyProperty.Register("InitialHorizontalScrollPercent",
			typeof(double), typeof(Diagram3DAnimation), new PropertyMetadata(Double.NaN), new ValidateValueCallback(ValidateScrollPercent));
		public static readonly DependencyProperty InitialRotationProperty = DependencyProperty.Register("InitialRotation", 
			typeof(AnimationRotation), typeof(Diagram3DAnimation), new PropertyMetadata(new AnimationRotation()));
		internal static bool ValidatePerspectiveAngle(object value) {
			return Double.IsNaN((double)value) || Diagram3D.ValidatePerspectiveAngle(value);
		}
		internal static bool ValidateZoomPercent(object value) {
			return Double.IsNaN((double)value) || Diagram3D.ValidateZoomPercent(value);
		}
		internal static bool ValidateScrollPercent(object value) {
			return Double.IsNaN((double)value) || Diagram3D.ValidateScrollPercent(value);
		}
		double initialPerspectiveAngle;
		double initialZoomPercent = Diagram3D.minZoomPercent;
		double initualVerticalScrollPercent;
		double initialHorizontalScrollPercent;
		Transform3D initialContentTransform;
		internal Diagram3D Diagram { get { return (Diagram3D)AnimationRecord.ChartControl.Diagram; } }
		internal double ActualInitialPerspectiveAngle { get { return initialPerspectiveAngle; } }
		internal double ActialInitialZoomPercent { get { return initialZoomPercent; } }
		internal double ActualInitialVerticalScrollPercent { get { return initualVerticalScrollPercent; } }
		internal double ActualInitialHorizontalScrollPercent { get { return initialHorizontalScrollPercent; } }
		internal Transform3D ActualInitialContentTransform { get { return initialContentTransform; } }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("Diagram3DAnimationActions"),
#endif
		Category(Categories.Elements),
		]
		public Diagram3DAnimationActionCollection Actions {
			get { return (Diagram3DAnimationActionCollection)GetValue(ActionsProperty); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("Diagram3DAnimationInitialPerspectiveAngle"),
#endif
		Category(Categories.Behavior)
		]
		public double InitialPerspectiveAngle {
			get { return (double)GetValue(InitialPerspectiveAngleProperty); }
			set { SetValue(InitialPerspectiveAngleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("Diagram3DAnimationInitialZoomPercent"),
#endif
		Category(Categories.Behavior)
		]
		public double InitialZoomPercent {
			get { return (double)GetValue(InitialZoomPercentProperty); }
			set { SetValue(InitialZoomPercentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("Diagram3DAnimationInitialVerticalScrollPercent"),
#endif
		Category(Categories.Behavior)
		]
		public double InitialVerticalScrollPercent {
			get { return (double)GetValue(InitialVerticalScrollPercentProperty); }
			set { SetValue(InitialVerticalScrollPercentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("Diagram3DAnimationInitialHorizontalScrollPercent"),
#endif
		Category(Categories.Behavior)
		]
		public double InitialHorizontalScrollPercent {
			get { return (double)GetValue(InitialHorizontalScrollPercentProperty); }
			set { SetValue(InitialHorizontalScrollPercentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("Diagram3DAnimationInitialRotation"),
#endif
		Category(Categories.Behavior)
		]
		public AnimationRotation InitialRotation {
			get { return (AnimationRotation)GetValue(InitialRotationProperty); }
			set { SetValue(InitialRotationProperty, value); }
		}
		public Diagram3DAnimation() {
			SetValue(ActionsPropertyKey, ChartElementHelper.CreateInstance<Diagram3DAnimationActionCollection>(this));
		}
		protected internal override void Initialize() {
			Diagram3D diagram = Diagram;
			if (!Double.IsNaN(InitialPerspectiveAngle))
				diagram.PerspectiveAngle = InitialPerspectiveAngle;
			if (!Double.IsNaN(InitialZoomPercent))
				diagram.ZoomPercent = InitialZoomPercent;
			if (!Double.IsNaN(InitialVerticalScrollPercent))
				diagram.VerticalScrollPercent = InitialVerticalScrollPercent;
			if (!Double.IsNaN(InitialHorizontalScrollPercent))
				diagram.HorizontalScrollPercent = InitialHorizontalScrollPercent;
			initialContentTransform = InitialRotation.Order == AnimationRotationOrder.None ? diagram.ActualContentTransform : null;
			initialPerspectiveAngle = diagram.PerspectiveAngle;
			initialZoomPercent = diagram.ZoomPercent;
			initualVerticalScrollPercent = diagram.VerticalScrollPercent;
			initialHorizontalScrollPercent = diagram.HorizontalScrollPercent;
		}
		protected internal override void PerformAnimation(double value) {
			foreach (Diagram3DAnimationAction action in Actions)
				action.PerformAnimation(value);
		}
	}
	public class Diagram3DAnimationActionCollection : ChartElementCollection<Diagram3DAnimationAction> { }
	public abstract class Diagram3DAnimationAction : ChartElement {
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool Focusable { get { return base.Focusable; } set { base.Focusable = value; } }
		protected Diagram3DAnimation Animation { get { return (Diagram3DAnimation)((IChartElement)this).Owner; } }
		protected Diagram3D Diagram { get { return Animation.Diagram; } }
		protected internal abstract void PerformAnimation(double value);
	}
	public sealed class Diagram3DPerspectiveAction : Diagram3DAnimationAction {
		public static readonly DependencyProperty FinalPerspectiveAngleProperty = DependencyProperty.Register("FinalPerspectiveAngle", typeof(double),
			typeof(Diagram3DPerspectiveAction), new PropertyMetadata(Double.NaN), new ValidateValueCallback(Diagram3DAnimation.ValidatePerspectiveAngle));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("Diagram3DPerspectiveActionFinalPerspectiveAngle"),
#endif
		Category(Categories.Behavior)
		]
		public double FinalPerspectiveAngle {
			get { return (double)GetValue(FinalPerspectiveAngleProperty); }
			set { SetValue(FinalPerspectiveAngleProperty, value); }
		}
		protected internal override void PerformAnimation(double value) {
			if (!Double.IsNaN(FinalPerspectiveAngle))
				Diagram.PerspectiveAngle = Animation.ActualInitialPerspectiveAngle + (FinalPerspectiveAngle - Animation.ActualInitialPerspectiveAngle) * value;
		}
	}
	public sealed class Diagram3DZoomAction : Diagram3DAnimationAction {
		public static readonly DependencyProperty FinalZoomPercentProperty = DependencyProperty.Register("FinalZoomPercent", typeof(double),
			typeof(Diagram3DZoomAction), new PropertyMetadata(Double.NaN), new ValidateValueCallback(Diagram3DAnimation.ValidateZoomPercent));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("Diagram3DZoomActionFinalZoomPercent"),
#endif
		Category(Categories.Behavior)
		]
		public double FinalZoomPercent {
			get { return (double)GetValue(FinalZoomPercentProperty); }
			set { SetValue(FinalZoomPercentProperty, value); }
		}
		protected internal override void PerformAnimation(double value) {
			if (!Double.IsNaN(FinalZoomPercent))
				Diagram.ZoomPercent = Animation.ActialInitialZoomPercent + (FinalZoomPercent - Animation.ActialInitialZoomPercent) * value;
		}
	}
	public sealed class Diagram3DVerticalScrollPercentAction : Diagram3DAnimationAction {
		public static readonly DependencyProperty FinalVerticalScrollPercentProperty = DependencyProperty.Register("FinalVerticalScrollPercent", typeof(double), 
			typeof(Diagram3DVerticalScrollPercentAction), new PropertyMetadata(Double.NaN), new ValidateValueCallback(Diagram3DAnimation.ValidateScrollPercent));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("Diagram3DVerticalScrollPercentActionFinalVerticalScrollPercent"),
#endif
		Category(Categories.Behavior)
		]
		public double FinalVerticalScrollPercent {
			get { return (double)GetValue(FinalVerticalScrollPercentProperty); }
			set { SetValue(FinalVerticalScrollPercentProperty, value); }
		}
		protected internal override void PerformAnimation(double value) {
			if (!Double.IsNaN(FinalVerticalScrollPercent))
				Diagram.VerticalScrollPercent = Animation.ActualInitialVerticalScrollPercent + (FinalVerticalScrollPercent - Animation.ActualInitialVerticalScrollPercent) * value;
		}
	}
	public sealed class Diagram3DHorizontalScrollPercentAction : Diagram3DAnimationAction {
		public static readonly DependencyProperty FinalHorizontalScrollPercentProperty = DependencyProperty.Register("FinalHorizontalScrollPercent", 
			typeof(double), typeof(Diagram3DHorizontalScrollPercentAction), new PropertyMetadata(Double.NaN), new ValidateValueCallback(Diagram3DAnimation.ValidateScrollPercent));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("Diagram3DHorizontalScrollPercentActionFinalHorizontalScrollPercent"),
#endif
		Category(Categories.Behavior)
		]
		public double FinalHorizontalScrollPercent {
			get { return (double)GetValue(FinalHorizontalScrollPercentProperty); }
			set { SetValue(FinalHorizontalScrollPercentProperty, value); }
		}
		protected internal override void PerformAnimation(double value) {
			if (!Double.IsNaN(FinalHorizontalScrollPercent))
				Diagram.HorizontalScrollPercent = Animation.ActualInitialHorizontalScrollPercent + (FinalHorizontalScrollPercent - Animation.ActualInitialHorizontalScrollPercent) * value;
		}
	}
	public sealed class Diagram3DRotationAction : Diagram3DAnimationAction, IRotationAnimation {
		public static readonly DependencyProperty RotationProperty = DependencyProperty.Register("Rotation", 
			typeof(AnimationRotation), typeof(Diagram3DRotationAction), new PropertyMetadata(new AnimationRotation()));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("Diagram3DRotationActionRotation"),
#endif
		Category(Categories.Behavior)
		]
		public AnimationRotation Rotation {
			get { return (AnimationRotation)GetValue(RotationProperty); }
			set { SetValue(RotationProperty, value); }
		}
		#region IRotationAnimation implementation
		Transform3D IRotationAnimation.InitialTransform { get { return Animation.ActualInitialContentTransform; } }
		Transform3D IRotationAnimation.Transform { set { Diagram.ContentTransform = value; } }
		AnimationRotation IRotationAnimation.InitialRotation { get { return Animation.InitialRotation; } }
		AnimationRotation IRotationAnimation.Rotation { get { return Rotation; } }
		#endregion
		protected internal override void PerformAnimation(double value) {
			RotationAnimationHelper.PerformAnimation(this, value);
		}
	}
}
