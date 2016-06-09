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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public abstract class Pie2DModel : PointModel {
		public static IEnumerable<Pie2DKind> GetPredefinedKinds() {
			return Pie2DKind.List;
		}
#if !SL
	[DevExpressXpfChartsLocalizedDescription("Pie2DModelModelName")]
#endif
		public abstract string ModelName { get; }
		protected abstract Pie2DModel CreateObjectForClone();
		protected virtual void Assign(Pie2DModel model) { }
		protected internal Pie2DModel CloneModel() {
			Pie2DModel model = CreateObjectForClone();
			model.Assign(this);
			return model;
		}
	}
	public abstract class PredefinedPie2DModel : Pie2DModel {
	}
	public class CupidPie2DModel : PredefinedPie2DModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("CupidPie2DModelModelName")]
#endif
		public override string ModelName { get { return "Cupid"; } }
		protected override Pie2DModel CreateObjectForClone() {
			return new CupidPie2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new CupidPie2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new CupidPie2DModel();
		}
	}
	public class GlarePie2DModel : PredefinedPie2DModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("GlarePie2DModelModelName")]
#endif
		public override string ModelName { get { return "Glare"; } }
		protected override Pie2DModel CreateObjectForClone() {
			return new GlarePie2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new GlarePie2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new GlarePie2DModel();
		}
	}
	public class GlassPie2DModel : PredefinedPie2DModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("GlassPie2DModelModelName")]
#endif
		public override string ModelName { get { return "Glass"; } }
		protected override Pie2DModel CreateObjectForClone() {
			return new GlassPie2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new GlassPie2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new GlassPie2DModel();
		}
	}
	public class GlossyPie2DModel : PredefinedPie2DModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("GlossyPie2DModelModelName")]
#endif
		public override string ModelName { get { return "Glossy"; } }
		protected override Pie2DModel CreateObjectForClone() {
			return new GlossyPie2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new GlossyPie2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new GlossyPie2DModel();
		}
	}
	public class SimplePie2DModel : PredefinedPie2DModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("SimplePie2DModelModelName")]
#endif
		public override string ModelName { get { return "Simple"; } }
		protected override Pie2DModel CreateObjectForClone() {
			return new SimplePie2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new SimplePie2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new SimplePie2DModel();
		}
	}
	public class FlatPie2DModel : PredefinedPie2DModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("FlatPie2DModelModelName")]
#endif
		public override string ModelName { get { return "Flat"; } }
		protected override Pie2DModel CreateObjectForClone() {
			return new FlatPie2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new FlatPie2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new FlatPie2DModel();
		}
	}
	public class BorderlessFlatPie2DModel : PredefinedPie2DModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("BorderlessFlatPie2DModelModelName")]
#endif
		public override string ModelName { get { return "Borderless Flat"; } }
		protected override Pie2DModel CreateObjectForClone() {
			return new BorderlessFlatPie2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new BorderlessFlatPie2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new BorderlessFlatPie2DModel();
		}
	}
	public class CustomPie2DModel : Pie2DModel {
		public static readonly DependencyProperty PointTemplateProperty = DependencyPropertyManager.Register("PointTemplate",
			typeof(ControlTemplate), typeof(CustomPie2DModel));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CustomPie2DModelPointTemplate"),
#endif
		Category(Categories.Presentation)
		]
		public ControlTemplate PointTemplate {
			get { return (ControlTemplate)GetValue(PointTemplateProperty); }
			set { SetValue(PointTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfChartsLocalizedDescription("CustomPie2DModelModelName")]
#endif
		public override string ModelName { get { return "Custom"; } }
		protected override Pie2DModel CreateObjectForClone() {
			return new CustomPie2DModel();
		}
		protected override void Assign(Pie2DModel model) {
			base.Assign(model);
			CustomPie2DModel customModel = model as CustomPie2DModel;
			if (customModel != null)
				CopyPropertyValueHelper.CopyPropertyValue(this, customModel, PointTemplateProperty);
		}
		protected internal override ModelControl CreateModelControl() {
			CustomPie2DModelControl modelControl = new CustomPie2DModelControl();
			modelControl.SetBinding(CustomModelControl.TemplateProperty, new Binding("PointTemplate") { Source = this });
			return modelControl;
		}
		protected override ChartDependencyObject CreateObject() {
			return new CustomPie2DModel();
		}
	}
}
