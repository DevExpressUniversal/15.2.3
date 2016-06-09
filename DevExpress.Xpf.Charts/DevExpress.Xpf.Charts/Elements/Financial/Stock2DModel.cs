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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public abstract class Stock2DModel : PointModel {
		public static IEnumerable<Stock2DKind> GetPredefinedKinds() {
			return Stock2DKind.List;
		}
#if !SL
	[DevExpressXpfChartsLocalizedDescription("Stock2DModelModelName")]
#endif
		public abstract string ModelName { get; }
		protected virtual void Assign(Stock2DModel model) { 
		}
		protected abstract Stock2DModel CreateObjectForClone();
		protected internal Stock2DModel CloneModel() {
			Stock2DModel model = CreateObjectForClone();
			model.Assign(this);
			return model;
		}
	}
	public abstract class PredefinedStock2DModel : Stock2DModel {
	}
	public class ThinStock2DModel : PredefinedStock2DModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ThinStock2DModelModelName")]
#endif
		public override string ModelName { get { return "Thin"; } }
		protected override Stock2DModel CreateObjectForClone() {
			return new ThinStock2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new ThinStock2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new ThinStock2DModel();
		}
	}
	public class FlatStock2DModel : PredefinedStock2DModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("FlatStock2DModelModelName")]
#endif
		public override string ModelName { get { return "Flat"; } }
		protected override Stock2DModel CreateObjectForClone() {
			return new FlatStock2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new FlatStock2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new FlatStock2DModel();
		}
	}
	public class DropsStock2DModel : PredefinedStock2DModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("DropsStock2DModelModelName")]
#endif
		public override string ModelName { get { return "Drops"; } }
		protected override Stock2DModel CreateObjectForClone() {
			return new DropsStock2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new DropsStock2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new DropsStock2DModel();
		}
	}
	public class ArrowsStock2DModel : PredefinedStock2DModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ArrowsStock2DModelModelName")]
#endif
		public override string ModelName { get { return "Arrows"; } }
		protected override Stock2DModel CreateObjectForClone() {
			return new ArrowsStock2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new ArrowsStock2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new ArrowsStock2DModel();
		}
	}
	public class CustomStock2DModel : Stock2DModel {
		static ControlTemplate CreateDefaultPointTemplate() {
			string template = @"<local:Stock2DModelPanel xmlns:local=""http://schemas.devexpress.com/winfx/2008/xaml/charts"" x:Name=""rootPanel"">
                                    <ContentPresenter local:Stock2DModelPanel.Elements=""CenterLine"" Content=""{Binding }""
                                        ContentTemplate=""{Binding Path=CenterLineTemplate, RelativeSource={RelativeSource TemplatedParent}}""/>
                                    <ContentPresenter local:Stock2DModelPanel.Elements=""OpenLine"" Content=""{Binding }""
                                        ContentTemplate=""{Binding Path=OpenLineTemplate, RelativeSource={RelativeSource TemplatedParent}}""/>
                                    <ContentPresenter local:Stock2DModelPanel.Elements=""CloseLine"" Content=""{Binding }""
                                        ContentTemplate=""{Binding Path=CloseLineTemplate, RelativeSource={RelativeSource TemplatedParent}}""/>
                                </local:Stock2DModelPanel>";
			return XamlHelper.GetControlTemplate(template);
		}
		public static readonly DependencyProperty OpenLineTemplateProperty = DependencyPropertyManager.Register("OpenLineTemplate",
			typeof(DataTemplate), typeof(CustomStock2DModel), new PropertyMetadata(null));
		public static readonly DependencyProperty CloseLineTemplateProperty = DependencyPropertyManager.Register("CloseLineTemplate",
			typeof(DataTemplate), typeof(CustomStock2DModel), new PropertyMetadata(null));
		public static readonly DependencyProperty CenterLineTemplateProperty = DependencyPropertyManager.Register("CenterLineTemplate",
			typeof(DataTemplate), typeof(CustomStock2DModel), new PropertyMetadata(null));
		public static readonly DependencyProperty PointTemplateProperty = DependencyPropertyManager.Register("PointTemplate",
			typeof(ControlTemplate), typeof(CustomStock2DModel), new PropertyMetadata(CreateDefaultPointTemplate()));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CustomStock2DModelOpenLineTemplate"),
#endif
		Category(Categories.Common)
		]
		public DataTemplate OpenLineTemplate {
			get { return (DataTemplate)GetValue(OpenLineTemplateProperty); }
			set { SetValue(OpenLineTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CustomStock2DModelCloseLineTemplate"),
#endif
		Category(Categories.Common)
		]
		public DataTemplate CloseLineTemplate {
			get { return (DataTemplate)GetValue(CloseLineTemplateProperty); }
			set { SetValue(CloseLineTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CustomStock2DModelCenterLineTemplate"),
#endif
		Category(Categories.Common)
		]
		public DataTemplate CenterLineTemplate {
			get { return (DataTemplate)GetValue(CenterLineTemplateProperty); }
			set { SetValue(CenterLineTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CustomStock2DModelPointTemplate"),
#endif
		Category(Categories.Common)
		]
		public ControlTemplate PointTemplate {
			get { return (ControlTemplate)GetValue(PointTemplateProperty); }
			set { SetValue(PointTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfChartsLocalizedDescription("CustomStock2DModelModelName")]
#endif
		public override string ModelName { get { return "Custom"; } }
		protected override Stock2DModel CreateObjectForClone() {
			return new CustomStock2DModel();
		}
		protected override void Assign(Stock2DModel model) {
			base.Assign(model);
			CustomStock2DModel customStock2DModel = model as CustomStock2DModel;
			if (customStock2DModel != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, customStock2DModel, CenterLineTemplateProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, customStock2DModel, OpenLineTemplateProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, customStock2DModel, CloseLineTemplateProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, customStock2DModel, PointTemplateProperty);
			}
		}
		protected internal override ModelControl CreateModelControl() {
			CustomStockModelControl modelControl = new CustomStockModelControl();
			modelControl.SetBinding(CustomStockModelControl.CenterLineTemplateProperty, new Binding("CenterLineTemplate") { Source = this });
			modelControl.SetBinding(CustomStockModelControl.OpenLineTemplateProperty, new Binding("OpenLineTemplate") { Source = this });
			modelControl.SetBinding(CustomStockModelControl.CloseLineTemplateProperty, new Binding("CloseLineTemplate") { Source = this });
			modelControl.SetBinding(CustomStockModelControl.TemplateProperty, new Binding("PointTemplate") { Source = this });
			return modelControl;
		}
		protected override ChartDependencyObject CreateObject() {
			return new CustomStock2DModel();
		}
	}
}
