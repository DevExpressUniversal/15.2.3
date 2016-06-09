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
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public enum AnimationRotationOrder { None = 0, XYZ, XZY, YXZ, YZX, ZXY, ZYX }
	public struct AnimationRotation {
		AnimationRotationOrder order;
		double angleX;
		double angleY;
		double angleZ;
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("AnimationRotationOrder"),
#endif
		Category(Categories.Behavior)
		]
		public AnimationRotationOrder Order {
			get { return order; }
			set { order = value; }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("AnimationRotationAngleX"),
#endif
		Category(Categories.Behavior)
		]
		public double AngleX {
			get { return angleX; }
			set { angleX = value; }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("AnimationRotationAngleY"),
#endif
		Category(Categories.Behavior)
		]
		public double AngleY {
			get { return angleY; }
			set { angleY = value; }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("AnimationRotationAngleZ"),
#endif
		Category(Categories.Behavior)
		]
		public double AngleZ {
			get { return angleZ; }
			set { angleZ = value; }
		}
	}
	internal interface IRotationAnimation {
		Transform3D InitialTransform { get; }
		Transform3D Transform { set; }
		AnimationRotation InitialRotation { get; }
		AnimationRotation Rotation { get; }
	}
	internal static class RotationAnimationHelper {
		public static void PerformAnimation(IRotationAnimation animation, double value) {
			AxisAngleRotation3D axisXRotation = new AxisAngleRotation3D(new Vector3D(1, 0, 0), 
				animation.Rotation.AngleX * value);
			AxisAngleRotation3D axisYRotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 
				animation.Rotation.AngleY * value);
			AxisAngleRotation3D axisZRotation = new AxisAngleRotation3D(new Vector3D(0, 0, 1), 
				animation.Rotation.AngleZ * value);
			AnimationRotationOrder rotationOrder = AnimationRotationOrder.None;
			Transform3DGroup transform = new Transform3DGroup();
			if (animation.InitialTransform == null) {
				rotationOrder = animation.InitialRotation.Order;
				axisXRotation.Angle += animation.InitialRotation.AngleX;
				axisYRotation.Angle += animation.InitialRotation.AngleY;
				axisZRotation.Angle += animation.InitialRotation.AngleZ;
			}
			else {
				rotationOrder = animation.Rotation.Order;
				transform.Children.Add(animation.InitialTransform);
			}
			RotateTransform3D axisXTransform = new RotateTransform3D(axisXRotation);
			RotateTransform3D axisYTransform = new RotateTransform3D(axisYRotation);
			RotateTransform3D axisZTransform = new RotateTransform3D(axisZRotation);
			switch (rotationOrder) {
				case AnimationRotationOrder.XYZ:
					transform.Children.Add(axisXTransform);
					transform.Children.Add(axisYTransform);
					transform.Children.Add(axisZTransform);
					break;
				case AnimationRotationOrder.XZY:
					transform.Children.Add(axisXTransform);
					transform.Children.Add(axisZTransform);
					transform.Children.Add(axisYTransform);
					break;
				case AnimationRotationOrder.YXZ:
					transform.Children.Add(axisYTransform);
					transform.Children.Add(axisXTransform);
					transform.Children.Add(axisZTransform);
					break;
				case AnimationRotationOrder.YZX:
					transform.Children.Add(axisYTransform);
					transform.Children.Add(axisZTransform);
					transform.Children.Add(axisXTransform);
					break;
				case AnimationRotationOrder.ZXY:
					transform.Children.Add(axisZTransform);
					transform.Children.Add(axisXTransform);
					transform.Children.Add(axisYTransform);
					break;
				case AnimationRotationOrder.ZYX:
					transform.Children.Add(axisZTransform);
					transform.Children.Add(axisYTransform);
					transform.Children.Add(axisXTransform);
					break;
				default:
					return;
			}
			animation.Transform = transform;
		}
	}
	public class ChartAnimationCollection : ChartElementCollection<ChartAnimation> {}
	public class ChartAnimationRecordCollection : ChartElementCollection<ChartAnimationRecord> {}
	public abstract class ChartAnimation : ChartElement {
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool Focusable { get { return base.Focusable; } set { base.Focusable = value; } }
		internal ChartAnimationRecord AnimationRecord { 
			get { return (ChartAnimationRecord)((IChartElement)this).Owner; } 
		}
		protected internal abstract void Initialize();
		protected internal abstract void PerformAnimation(double value);
	}
	public class ChartAnimationRecord : ChartElement {
		static readonly DependencyPropertyKey AnimationsPropertyKey = DependencyPropertyManager.RegisterReadOnly("Animations", 
			typeof(ChartAnimationCollection), typeof(ChartAnimationRecord), new PropertyMetadata(null));
		public static readonly DependencyProperty AnimationsProperty = AnimationsPropertyKey.DependencyProperty;
		public static readonly DependencyProperty ProgressProperty = DependencyPropertyManager.Register("Progress", 
			typeof(double), typeof(ChartAnimationRecord), new PropertyMetadata(Double.NaN, ProgressChanged, CoerceProgress));
		static void ProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			double newValue = (double)e.NewValue;
			if (!Double.IsNaN(newValue)) {
				double oldValue = (double)e.OldValue; 
				ChartAnimationRecord record = (ChartAnimationRecord)d;
				if (Double.IsNaN(oldValue)) {
					record.Initialize();
					oldValue = 0.0;
				}
				if (newValue != oldValue)
					record.PerformAnimation(newValue);
			}
		}
		static object CoerceProgress(DependencyObject d, object value) {
			double progress = (double)value;
			if (Double.IsNaN(progress))
				return progress;
			if (progress < 0.0)
				return 0.0;
			if (progress > 1.0)
				return 1.0;
			return progress;
		}
		internal ChartControl ChartControl { get { return (ChartControl)((IChartElement)this).Owner; } }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ChartAnimationRecordAnimations"),
#endif
		Category(Categories.Elements),
		]
		public ChartAnimationCollection Animations { get { return (ChartAnimationCollection)GetValue(AnimationsProperty); } }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ChartAnimationRecordProgress"),
#endif
		Category(Categories.Behavior)
		]
		public double Progress {
			get { return (double)GetValue(ProgressProperty); }
			set { SetValue(ProgressProperty, value); }
		}
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool Focusable { get { return base.Focusable; } set { base.Focusable = value; } }
		public ChartAnimationRecord() {
			this.SetValue(AnimationsPropertyKey, ChartElementHelper.CreateInstance<ChartAnimationCollection>(this));
		}
		void Initialize() {
			foreach (ChartAnimation animation in Animations)
				animation.Initialize();
		}
		void PerformAnimation(double value) {
			foreach (ChartAnimation animation in Animations)
				animation.PerformAnimation(value);
		}
	}
}
