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

using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public abstract class RangeBar2DModel : PointModel {
		public static IEnumerable<RangeBar2DKind> GetPredefinedKinds() {
			return RangeBar2DKind.List;
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("RangeBar2DModelModelName")
#else
	Description("")
#endif
		]
		public abstract string ModelName { get; }
		protected abstract RangeBar2DModel CreateObjectForClone();
		protected virtual void Assign(RangeBar2DModel model) {
		}
		protected internal RangeBar2DModel CloneModel() {
			RangeBar2DModel model = CreateObjectForClone();
			model.Assign(this);
			return model;
		}
	}
	public abstract class PredefinedRangeBar2DModel : RangeBar2DModel {
		internal PredefinedRangeBar2DModel() {
		}
	}
	public class FlatRangeBar2DModel : PredefinedRangeBar2DModel {
		public override string ModelName { get { return "Flat Bar"; } }
		protected override RangeBar2DModel CreateObjectForClone() {
			return new FlatRangeBar2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new FlatRangeBar2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new FlatRangeBar2DModel();
		}
	}
	public class FlatGlassRangeBar2DModel : PredefinedRangeBar2DModel {
		public override string ModelName { get { return "Flat Glass Bar"; } }
		protected override RangeBar2DModel CreateObjectForClone() {
			return new FlatGlassRangeBar2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new FlatGlassRangeBar2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new FlatGlassRangeBar2DModel();
		}
	}
	public class TransparentRangeBar2DModel : PredefinedRangeBar2DModel {
		public override string ModelName { get { return "Transparent Bar"; } }
		protected override RangeBar2DModel CreateObjectForClone() {
			return new TransparentRangeBar2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new TransparentRangeBar2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new TransparentRangeBar2DModel();
		}
	}
	public class GradientRangeBar2DModel : PredefinedRangeBar2DModel {
		public override string ModelName { get { return "Gradient Bar"; } }
		protected override RangeBar2DModel CreateObjectForClone() {
			return new GradientRangeBar2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new GradientRangeBar2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new GradientRangeBar2DModel();
		}
	}
	public class BorderlessGradientRangeBar2DModel : PredefinedRangeBar2DModel {
		public override string ModelName { get { return "Borderless Gradient Bar"; } }
		protected override RangeBar2DModel CreateObjectForClone() {
			return new BorderlessGradientRangeBar2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new BorderlessGradientRangeBar2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new BorderlessGradientRangeBar2DModel();
		}
	}
	public class OutsetRangeBar2DModel : PredefinedRangeBar2DModel {
		public override string ModelName { get { return "Outset Bar"; } }
		protected override RangeBar2DModel CreateObjectForClone() {
			return new OutsetRangeBar2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new OutsetRangeBar2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new OutsetRangeBar2DModel();
		}
	}
	public class SimpleRangeBar2DModel : PredefinedRangeBar2DModel {
		public override string ModelName { get { return "Simple Bar"; } }
		protected override RangeBar2DModel CreateObjectForClone() {
			return new SimpleRangeBar2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new SimpleRangeBar2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new SimpleRangeBar2DModel();
		}
	}
	public class BorderlessSimpleRangeBar2DModel : PredefinedRangeBar2DModel {
		public override string ModelName { get { return "Borderless Simple Bar"; } }
		protected override RangeBar2DModel CreateObjectForClone() {
			return new BorderlessSimpleRangeBar2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new BorderlessSimpleRangeBar2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new BorderlessSimpleRangeBar2DModel();
		}
	}
	public class CustomRangeBar2DModel : RangeBar2DModel {
		public static readonly DependencyProperty PointTemplateProperty = DependencyPropertyManager.Register("PointTemplate",
			typeof(ControlTemplate), typeof(CustomRangeBar2DModel));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CustomRangeBar2DModelPointTemplate"),
#endif
		Category(Categories.Presentation)
		]
		public ControlTemplate PointTemplate {
			get { return (ControlTemplate)GetValue(PointTemplateProperty); }
			set { SetValue(PointTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfChartsLocalizedDescription("CustomRangeBar2DModelModelName")]
#endif
		public override string ModelName { get { return "Custom"; } }
		protected override RangeBar2DModel CreateObjectForClone() {
			return new CustomRangeBar2DModel();
		}
		protected override void Assign(RangeBar2DModel model) {
			base.Assign(model);
			CustomRangeBar2DModel customBar2DModel = model as CustomRangeBar2DModel;
			if (customBar2DModel != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, customBar2DModel, PointTemplateProperty);
			}
		}
		protected internal override ModelControl CreateModelControl() {
			CustomModelControl modelControl = new CustomModelControl();
			modelControl.SetBinding(CustomModelControl.TemplateProperty, new Binding("PointTemplate") { Source = this });
			return modelControl;
		}
		protected override ChartDependencyObject CreateObject() {
			return new CustomRangeBar2DModel();
		}
	}
}
