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
using System.Windows.Controls;
using System.Windows.Data;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public abstract class PointModel : ChartDependencyObject {
		protected internal abstract ModelControl CreateModelControl();
		protected internal virtual Point CorrectCenterLabelLocation(Point location) {
			return location;
		}
		protected internal virtual Point CorrectOutsideLabelLocation(Point location, bool isDownwardBar) {
			return location;
		}
		protected internal virtual Rect CalculateCorrectedBounds(Rect viewport, Rect initialBounds, bool rotated) {
			return Rect.Empty;
		}
	}
	public abstract class Bar2DModel : PointModel {
		public static IEnumerable<Bar2DKind> GetPredefinedKinds() {
			return Bar2DKind.List;
		}
		protected internal abstract bool ActualOverlapBars { get; }
		protected internal abstract bool ActualFlipNegativeBars { get; }
		protected internal virtual bool ActualInFrontOfAxes { get { return false; } }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("Bar2DModelModelName")
#else
	Description("")
#endif
		]
		public abstract string ModelName { get; }
		protected abstract Bar2DModel CreateObjectForClone();
		protected virtual void Assign(Bar2DModel model) {
		}
		protected internal Bar2DModel CloneModel() {
			Bar2DModel model = CreateObjectForClone();
			model.Assign(this);
			return model;
		}		
	}
	public abstract class PredefinedBar2DModel : Bar2DModel {
		internal PredefinedBar2DModel() { }
		protected internal virtual int CorrectionSize { get { return 0; } }
		protected internal virtual int ReversedCorrectionSize { get { return 0; } }
	}
	public abstract class SymmetricBar2DModel : PredefinedBar2DModel {
		internal SymmetricBar2DModel() {
		}
	}
	public class FlatBar2DModel : SymmetricBar2DModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("FlatBar2DModelModelName")]
#endif
		public override string ModelName { get { return "Flat Bar"; } }
		protected internal override bool ActualOverlapBars { get { return true; } }
		protected internal override bool ActualFlipNegativeBars { get { return true; } }
		protected override Bar2DModel CreateObjectForClone() {
			return new FlatBar2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new FlatBar2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new FlatBar2DModel();
		}
	}
	public class FlatGlassBar2DModel : SymmetricBar2DModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("FlatGlassBar2DModelModelName")]
#endif
		public override string ModelName { get { return "Flat Glass Bar"; } }
		protected internal override bool ActualOverlapBars { get { return true; } }
		protected internal override bool ActualFlipNegativeBars { get { return true; } }
		protected override Bar2DModel CreateObjectForClone() {
			return new FlatGlassBar2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new FlatGlassBar2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new FlatGlassBar2DModel();
		}
	}
	public class SteelColumnBar2DModel : SymmetricBar2DModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("SteelColumnBar2DModelModelName")]
#endif
		public override string ModelName { get { return "Steel Column"; } }
		protected internal override bool ActualOverlapBars { get { return true; } }
		protected internal override bool ActualFlipNegativeBars { get { return true; } }
		protected override Bar2DModel CreateObjectForClone() {
			return new SteelColumnBar2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new SteelColumnBar2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new SteelColumnBar2DModel();
		}
	}
	public class TransparentBar2DModel : SymmetricBar2DModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("TransparentBar2DModelModelName")]
#endif
		public override string ModelName { get { return "Transparent Bar"; } }
		protected internal override bool ActualOverlapBars { get { return false; } }
		protected internal override bool ActualFlipNegativeBars { get { return true; } }
		protected override Bar2DModel CreateObjectForClone() {
			return new TransparentBar2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new TransparentBar2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new TransparentBar2DModel();
		}
	}
	public class Quasi3DBar2DModel : PredefinedBar2DModel {
		internal const int Depth = 8;
		static bool ValueWithinRange(double value) {
			return value < Depth && value >= 0;
		}
#if !SL
	[DevExpressXpfChartsLocalizedDescription("Quasi3DBar2DModelModelName")]
#endif
		public override string ModelName { get { return "Quasi-3D Bar"; } }
		protected internal override bool ActualOverlapBars { get { return false; } }
		protected internal override bool ActualInFrontOfAxes { get { return true; } }
		protected internal override bool ActualFlipNegativeBars { get { return false; } }
		protected override Bar2DModel CreateObjectForClone() {
			return new Quasi3DBar2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new Quasi3DBar2DModelControl();
		}
		protected internal override Point CorrectCenterLabelLocation(Point location) {
			return new Point(location.X - Depth, location.Y - Depth);
		}
		protected internal override Point CorrectOutsideLabelLocation(Point location, bool isDownwardBar) {
			return isDownwardBar ? new Point(location.X - Depth, location.Y - Depth) : location;
		}
		protected internal override Rect CalculateCorrectedBounds(Rect viewport, Rect initialBounds, bool rotated) {
			if (initialBounds.IsEmpty)
				return initialBounds;
			double left = ValueWithinRange(initialBounds.Left - viewport.Left) ? initialBounds.Left - Depth : viewport.Left;
			double top = ValueWithinRange(initialBounds.Top - viewport.Top) ? initialBounds.Top - Depth : viewport.Top;
			double right = rotated ? viewport.Bottom : viewport.Right;
			double bottom = rotated ? viewport.Right : viewport.Bottom;
			return new Rect(new Point(left, top), new Point(right, bottom));
		}
		protected internal override int CorrectionSize { get { return Depth; } }
		protected override ChartDependencyObject CreateObject() {
			return new Quasi3DBar2DModel();
		}
	}
	public class GlassCylinderBar2DModel : PredefinedBar2DModel {
		internal const int CoverHalfHeight = 9;
		static bool ValueWithinRange(double value) {
			return value < CoverHalfHeight && value >= 0;
		}
#if !SL
	[DevExpressXpfChartsLocalizedDescription("GlassCylinderBar2DModelModelName")]
#endif
		public override string ModelName { get { return "Glass Cylinder"; } }
		protected internal override bool ActualOverlapBars { get { return false; } }
		protected internal override bool ActualInFrontOfAxes { get { return true; } }
		protected internal override bool ActualFlipNegativeBars { get { return false; } }
		protected override Bar2DModel CreateObjectForClone() {
			return new GlassCylinderBar2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new GlassCylinderBar2DModelControl();
		}
		protected internal override Point CorrectCenterLabelLocation(Point location) {
			return new Point(location.X, location.Y - CoverHalfHeight);
		}
		protected internal override Rect CalculateCorrectedBounds(Rect viewport, Rect initialBounds, bool rotated) {
			if (initialBounds.IsEmpty)
				return initialBounds;
			double left = viewport.Left;
			double top = ValueWithinRange(initialBounds.Top - viewport.Top) ? initialBounds.Top - CoverHalfHeight : viewport.Top;
			double right = rotated ? viewport.Bottom : viewport.Right;
			double bottom;
			if (rotated)
				bottom = ValueWithinRange(viewport.Right - initialBounds.Bottom) ? initialBounds.Bottom + CoverHalfHeight : viewport.Right;
			else
				bottom = ValueWithinRange(viewport.Bottom - initialBounds.Bottom) ? initialBounds.Bottom + CoverHalfHeight : viewport.Bottom;
			return new Rect(new Point(left, top), new Point(right, bottom));
		}
		protected internal override int CorrectionSize { get { return CoverHalfHeight; } }
		protected internal override int ReversedCorrectionSize { get { return CoverHalfHeight; } }
		protected override ChartDependencyObject CreateObject() {
			return new GlassCylinderBar2DModel();
		}
	}
	public class GradientBar2DModel : SymmetricBar2DModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("GradientBar2DModelModelName")]
#endif
		public override string ModelName { get { return "Gradient Bar"; } }
		protected internal override bool ActualOverlapBars { get { return false; } }
		protected internal override bool ActualFlipNegativeBars { get { return true; } }
		protected override Bar2DModel CreateObjectForClone() {
			return new GradientBar2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new GradientBar2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new GradientBar2DModel();
		}
	}
	public class BorderlessGradientBar2DModel : SymmetricBar2DModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("BorderlessGradientBar2DModelModelName")]
#endif
		public override string ModelName { get { return "Gradient Borderless Bar"; } }
		protected internal override bool ActualOverlapBars { get { return false; } }
		protected internal override bool ActualFlipNegativeBars { get { return true; } }
		protected override Bar2DModel CreateObjectForClone() {
			return new BorderlessGradientBar2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new BorderlessGradientBar2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new BorderlessGradientBar2DModel();
		}
	}
	public class OutsetBar2DModel : SymmetricBar2DModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("OutsetBar2DModelModelName")]
#endif
		public override string ModelName { get { return "Outset Bar"; } }
		protected internal override bool ActualOverlapBars { get { return true; } }
		protected internal override bool ActualFlipNegativeBars { get { return true; } }
		protected override Bar2DModel CreateObjectForClone() {
			return new OutsetBar2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new OutsetBar2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new OutsetBar2DModel();
		}
	}
	public class SimpleBar2DModel : SymmetricBar2DModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("SimpleBar2DModelModelName")]
#endif
		public override string ModelName { get { return "Simple Bar"; } }
		protected internal override bool ActualOverlapBars { get { return false; } }
		protected internal override bool ActualFlipNegativeBars { get { return true; } }
		protected override Bar2DModel CreateObjectForClone() {
			return new SimpleBar2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new SimpleBar2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new SimpleBar2DModel();
		}
	}
	public class BorderlessSimpleBar2DModel : SymmetricBar2DModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("BorderlessSimpleBar2DModelModelName")]
#endif
		public override string ModelName { get { return "Borderless Simple Bar"; } }
		protected internal override bool ActualOverlapBars { get { return false; } }
		protected internal override bool ActualFlipNegativeBars { get { return true; } }
		protected override Bar2DModel CreateObjectForClone() {
			return new BorderlessSimpleBar2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new BorderlessSimpleBar2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new BorderlessSimpleBar2DModel();
		}
	}
	public class CustomBar2DModel : Bar2DModel {
		public static readonly DependencyProperty OverlapBarsProperty = DependencyPropertyManager.Register("OverlapBars",
			typeof(bool), typeof(CustomBar2DModel), new PropertyMetadata(NotifyPropertyChanged));
		public static readonly DependencyProperty FlipNegativeBarsProperty = DependencyPropertyManager.Register("FlipNegativeBars",
			typeof(bool), typeof(CustomBar2DModel), new PropertyMetadata(true, NotifyPropertyChanged));
		public static readonly DependencyProperty InFrontOfAxesProperty = DependencyPropertyManager.Register("InFrontOfAxes",
			typeof(bool), typeof(CustomBar2DModel), new PropertyMetadata(false, NotifyPropertyChanged));
		public static readonly DependencyProperty PointTemplateProperty = DependencyPropertyManager.Register("PointTemplate",
			typeof(ControlTemplate), typeof(CustomBar2DModel));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CustomBar2DModelOverlapBars"),
#endif
		Category(Categories.Behavior)
		]
		public bool OverlapBars {
			get { return (bool)GetValue(OverlapBarsProperty); }
			set { SetValue(OverlapBarsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CustomBar2DModelFlipNegativeBars"),
#endif
		Category(Categories.Behavior)
		]
		public bool FlipNegativeBars {
			get { return (bool)GetValue(FlipNegativeBarsProperty); }
			set { SetValue(FlipNegativeBarsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CustomBar2DModelInFrontOfAxes"),
#endif
		Category(Categories.Behavior)
		]
		public bool InFrontOfAxes {
			get { return (bool)GetValue(InFrontOfAxesProperty); }
			set { SetValue(InFrontOfAxesProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CustomBar2DModelPointTemplate"),
#endif
		Category(Categories.Presentation)
		]
		public ControlTemplate PointTemplate {
			get { return (ControlTemplate)GetValue(PointTemplateProperty); }
			set { SetValue(PointTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfChartsLocalizedDescription("CustomBar2DModelModelName")]
#endif
		public override string ModelName { get { return "Custom"; } }
		protected internal override bool ActualOverlapBars { get { return OverlapBars; } }
		protected internal override bool ActualInFrontOfAxes { get { return InFrontOfAxes; } }
		protected internal override bool ActualFlipNegativeBars { get { return FlipNegativeBars; } }
		protected override Bar2DModel CreateObjectForClone() {
			return new CustomBar2DModel();
		}
		protected override void Assign(Bar2DModel model) {
			base.Assign(model);
			CustomBar2DModel customBar2DModel = model as CustomBar2DModel;
			if (customBar2DModel != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, customBar2DModel, InFrontOfAxesProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, customBar2DModel, OverlapBarsProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, customBar2DModel, FlipNegativeBarsProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, customBar2DModel, PointTemplateProperty);
			}
		}
		protected internal override ModelControl CreateModelControl() {
			CustomModelControl modelControl = new CustomModelControl();
			modelControl.SetBinding(CustomModelControl.TemplateProperty, new Binding("PointTemplate") { Source = this });
			return modelControl;
		}
		protected override ChartDependencyObject CreateObject() {
			return new CustomBar2DModel();
		}
	}
}
