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
	public abstract class Marker2DModel : PointModel {
		public static IEnumerable<Marker2DKind> GetPredefinedKinds() {
			return Marker2DKind.List;
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("Marker2DModelModelName")
#else
	Description("")
#endif
		]
		public abstract string ModelName { get; }
		protected abstract Marker2DModel CreateObjectForClone();
		protected virtual void Assign(Marker2DModel model) { }
		protected internal Marker2DModel CloneModel() {
			Marker2DModel model = CreateObjectForClone();
			model.Assign(this);
			return model;
		}
	}
	public abstract class PredefinedMarker2DModel : Marker2DModel {
	}
	public class CircleMarker2DModel : PredefinedMarker2DModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("CircleMarker2DModelModelName")]
#endif
		public override string ModelName { get { return "Circle"; } }
		protected override Marker2DModel CreateObjectForClone() {
			return new CircleMarker2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new CircleMarker2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new CircleMarker2DModel();
		}
	}
	public class CrossMarker2DModel : PredefinedMarker2DModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("CrossMarker2DModelModelName")]
#endif
		public override string ModelName { get { return "Cross"; } }
		protected override Marker2DModel CreateObjectForClone() {
			return new CrossMarker2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new CrossMarker2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new CrossMarker2DModel();
		}
	}
	public class DollarMarker2DModel : PredefinedMarker2DModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("DollarMarker2DModelModelName")]
#endif
		public override string ModelName { get { return "Dollar"; } }
		protected override Marker2DModel CreateObjectForClone() {
			return new DollarMarker2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new DollarMarker2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new DollarMarker2DModel();
		}
	}
	public class PolygonMarker2DModel : PredefinedMarker2DModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("PolygonMarker2DModelModelName")]
#endif
		public override string ModelName { get { return "Polygon"; } }
		protected override Marker2DModel CreateObjectForClone() {
			return new PolygonMarker2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new PolygonMarker2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new PolygonMarker2DModel();
		}
	}
	public class RingMarker2DModel : PredefinedMarker2DModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("RingMarker2DModelModelName")]
#endif
		public override string ModelName { get { return "Ring"; } }
		protected override Marker2DModel CreateObjectForClone() {
			return new RingMarker2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new RingMarker2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new RingMarker2DModel();
		}
	}
	public class SquareMarker2DModel : PredefinedMarker2DModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("SquareMarker2DModelModelName")]
#endif
		public override string ModelName { get { return "Square"; } }
		protected override Marker2DModel CreateObjectForClone() {
			return new SquareMarker2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new SquareMarker2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new SquareMarker2DModel();
		}
	}
	public class StarMarker2DModel : PredefinedMarker2DModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("StarMarker2DModelModelName")]
#endif
		public override string ModelName { get { return "Star"; } }
		protected override Marker2DModel CreateObjectForClone() {
			return new StarMarker2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new StarMarker2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new StarMarker2DModel();
		}
	}
	public class TriangleMarker2DModel : PredefinedMarker2DModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("TriangleMarker2DModelModelName")]
#endif
		public override string ModelName { get { return "Triangle"; } }
		protected override Marker2DModel CreateObjectForClone() {
			return new TriangleMarker2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new TriangleMarker2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new TriangleMarker2DModel();
		}
	}
	public class CustomMarker2DModel : Marker2DModel {
		public static readonly DependencyProperty PointTemplateProperty = DependencyPropertyManager.Register("PointTemplate",
			typeof(ControlTemplate), typeof(CustomMarker2DModel));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CustomMarker2DModelPointTemplate"),
#endif
		Category(Categories.Presentation)
		]
		public ControlTemplate PointTemplate {
			get { return (ControlTemplate)GetValue(PointTemplateProperty); }
			set { SetValue(PointTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfChartsLocalizedDescription("CustomMarker2DModelModelName")]
#endif
		public override string ModelName { get { return "Custom"; } }
		protected override Marker2DModel CreateObjectForClone() {
			return new CustomMarker2DModel();
		}
		protected override void Assign(Marker2DModel model) {
			base.Assign(model);
			CustomMarker2DModel customModel = model as CustomMarker2DModel;
			if (customModel != null)
				CopyPropertyValueHelper.CopyPropertyValue(this, customModel, PointTemplateProperty);
		}
		protected internal override ModelControl CreateModelControl() {
			CustomModelControl modelControl = new CustomModelControl();
			modelControl.SetBinding(CustomModelControl.TemplateProperty, new Binding("PointTemplate") { Source = this });
			return modelControl;
		}
		protected override ChartDependencyObject CreateObject() {
			return new CustomMarker2DModel();
		}
	}
}
