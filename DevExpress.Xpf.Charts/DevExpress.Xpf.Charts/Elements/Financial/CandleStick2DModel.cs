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
	public abstract class CandleStick2DModel : PointModel {
		public static IEnumerable<CandleStick2DKind> GetPredefinedKinds() {
			return CandleStick2DKind.List;
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CandleStick2DModelModelName")
#else
	Description("")
#endif
		]
		public abstract string ModelName { get; }
		protected abstract CandleStick2DModel CreateObjectForClone();
		protected virtual void Assign(CandleStick2DModel model) { }
		protected internal CandleStick2DModel CloneModel() {
			CandleStick2DModel model = CreateObjectForClone();
			model.Assign(this);
			return model;
		}
	}
	public abstract class PredefinedCandleStick2DModel : CandleStick2DModel {
	}
	public class BorderCandleStick2DModel : PredefinedCandleStick2DModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("BorderCandleStick2DModelModelName")]
#endif
		public override string ModelName { get { return "Border"; } }
		protected override CandleStick2DModel CreateObjectForClone() {
			return new BorderCandleStick2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new BorderCandleStick2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new BorderCandleStick2DModel();
		}
	}
	public class ThinCandleStick2DModel : PredefinedCandleStick2DModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ThinCandleStick2DModelModelName")]
#endif
		public override string ModelName { get { return "Thin"; } }
		protected override CandleStick2DModel CreateObjectForClone() {
			return new ThinCandleStick2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new ThinCandleStick2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new ThinCandleStick2DModel();
		}
	}
	public class FlatCandleStick2DModel : PredefinedCandleStick2DModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("FlatCandleStick2DModelModelName")]
#endif
		public override string ModelName { get { return "Flat"; } }
		protected override CandleStick2DModel CreateObjectForClone() {
			return new FlatCandleStick2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new FlatCandleStick2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new FlatCandleStick2DModel();
		}
	}
	public class GlassCandleStick2DModel : PredefinedCandleStick2DModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("GlassCandleStick2DModelModelName")]
#endif
		public override string ModelName { get { return "Glass"; } }
		protected override CandleStick2DModel CreateObjectForClone() {
			return new GlassCandleStick2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new GlassCandleStick2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new GlassCandleStick2DModel();
		}
	}
	public class SimpleCandleStick2DModel : PredefinedCandleStick2DModel {
#if !SL
	[DevExpressXpfChartsLocalizedDescription("SimpleCandleStick2DModelModelName")]
#endif
		public override string ModelName { get { return "Simple"; } }
		protected override CandleStick2DModel CreateObjectForClone() {
			return new SimpleCandleStick2DModel();
		}
		protected internal override ModelControl CreateModelControl() {
			return new SimpleCandleStick2DModelControl();
		}
		protected override ChartDependencyObject CreateObject() {
			return new SimpleCandleStick2DModel();
		}
	}
	public class CustomCandleStick2DModel : CandleStick2DModel {
		static ControlTemplate CreateDefaultPointTemplate() {
			string template = @"<local:CandleStick2DModelPanel xmlns:local=""http://schemas.devexpress.com/winfx/2008/xaml/charts"">
                                    <ContentPresenter local:CandleStick2DModelPanel.Elements=""Candle"" Content=""{Binding }""
                                        ContentTemplate=""{Binding Path=CandleTemplate, RelativeSource={RelativeSource TemplatedParent}}""/>
                                    <ContentPresenter local:CandleStick2DModelPanel.Elements=""InvertedCandle"" Content=""{Binding }""
                                        ContentTemplate=""{Binding Path=InvertedCandleTemplate, RelativeSource={RelativeSource TemplatedParent}}""/>
                                    <ContentPresenter local:CandleStick2DModelPanel.Elements=""TopStick"" Content=""{Binding }""
                                        ContentTemplate=""{Binding Path=TopStickTemplate, RelativeSource={RelativeSource TemplatedParent}}""/>
                                    <ContentPresenter local:CandleStick2DModelPanel.Elements=""BottomStick"" Content=""{Binding }""
                                        ContentTemplate=""{Binding Path=BottomStickTemplate, RelativeSource={RelativeSource TemplatedParent}}""/>
                                </local:CandleStick2DModelPanel>";
			return XamlHelper.GetControlTemplate(template);
		}
		public static readonly DependencyProperty TopStickTemplateProperty = DependencyPropertyManager.Register("TopStickTemplate",
			typeof(DataTemplate), typeof(CustomCandleStick2DModel), new PropertyMetadata(null));
		public static readonly DependencyProperty BottomStickTemplateProperty = DependencyPropertyManager.Register("BottomStickTemplate",
			typeof(DataTemplate), typeof(CustomCandleStick2DModel), new PropertyMetadata(null));
		public static readonly DependencyProperty CandleTemplateProperty = DependencyPropertyManager.Register("CandleTemplate",
			typeof(DataTemplate), typeof(CustomCandleStick2DModel), new PropertyMetadata(null));
		public static readonly DependencyProperty InvertedCandleTemplateProperty = DependencyPropertyManager.Register("InvertedCandleTemplate",
			typeof(DataTemplate), typeof(CustomCandleStick2DModel), new PropertyMetadata(null));
		public static readonly DependencyProperty PointTemplateProperty = DependencyPropertyManager.Register("PointTemplate",
			typeof(ControlTemplate), typeof(CustomCandleStick2DModel), new PropertyMetadata(CreateDefaultPointTemplate()));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CustomCandleStick2DModelTopStickTemplate"),
#endif
		Category(Categories.Common)
		]
		public DataTemplate TopStickTemplate {
			get { return (DataTemplate)GetValue(TopStickTemplateProperty); }
			set { SetValue(TopStickTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CustomCandleStick2DModelBottomStickTemplate"),
#endif
		Category(Categories.Common)
		]
		public DataTemplate BottomStickTemplate {
			get { return (DataTemplate)GetValue(BottomStickTemplateProperty); }
			set { SetValue(BottomStickTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CustomCandleStick2DModelCandleTemplate"),
#endif
		Category(Categories.Common)
		]
		public DataTemplate CandleTemplate {
			get { return (DataTemplate)GetValue(CandleTemplateProperty); }
			set { SetValue(CandleTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CustomCandleStick2DModelInvertedCandleTemplate"),
#endif
		Category(Categories.Common)
		]
		public DataTemplate InvertedCandleTemplate {
			get { return (DataTemplate)GetValue(InvertedCandleTemplateProperty); }
			set { SetValue(InvertedCandleTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CustomCandleStick2DModelPointTemplate"),
#endif
		Category(Categories.Common)
		]
		public ControlTemplate PointTemplate {
			get { return (ControlTemplate)GetValue(PointTemplateProperty); }
			set { SetValue(PointTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfChartsLocalizedDescription("CustomCandleStick2DModelModelName")]
#endif
		public override string ModelName { get { return "Custom"; } }
		protected override CandleStick2DModel CreateObjectForClone() {
			return new CustomCandleStick2DModel();
		}
		protected override void Assign(CandleStick2DModel model) {
			base.Assign(model);
			CustomCandleStick2DModel customCandleStick2DModel = model as CustomCandleStick2DModel;
			if (customCandleStick2DModel != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, customCandleStick2DModel, CandleTemplateProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, customCandleStick2DModel, InvertedCandleTemplateProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, customCandleStick2DModel, TopStickTemplateProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, customCandleStick2DModel, BottomStickTemplateProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, customCandleStick2DModel, PointTemplateProperty);
			}
		}
		protected internal override ModelControl CreateModelControl() {
			CustomCandleStickModelControl modelControl = new CustomCandleStickModelControl();
			modelControl.SetBinding(CustomCandleStickModelControl.CandleTemplateProperty, new Binding("CandleTemplate") { Source = this });
			modelControl.SetBinding(CustomCandleStickModelControl.InvertedCandleTemplateProperty, new Binding("InvertedCandleTemplate") { Source = this });
			modelControl.SetBinding(CustomCandleStickModelControl.TopStickTemplateProperty, new Binding("TopStickTemplate") { Source = this });
			modelControl.SetBinding(CustomCandleStickModelControl.BottomStickTemplateProperty, new Binding("BottomStickTemplate") { Source = this });
			modelControl.SetBinding(CustomCandleStickModelControl.TemplateProperty, new Binding("PointTemplate") { Source = this });
			return modelControl;
		}
		protected override ChartDependencyObject CreateObject() {
			return new CustomCandleStick2DModel();
		}
	}
}
