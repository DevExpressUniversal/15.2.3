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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	public abstract class GaugeControlBase : Control, IModelSupported {
		const double defaultWidth = 250.0;
		const double defaultHeight = 250.0;
		public static readonly DependencyProperty EnableAnimationProperty = DependencyPropertyManager.Register("EnableAnimation",
			typeof(bool), typeof(GaugeControlBase), new PropertyMetadata(false, EnableAnimationPropertyChanged));
		internal static readonly DependencyPropertyKey ElementsPropertyKey = DependencyPropertyManager.RegisterReadOnly("Elements",
			typeof(ObservableCollection<IElementInfo>), typeof(GaugeControlBase), new PropertyMetadata());
		public static readonly DependencyProperty ElementsProperty = ElementsPropertyKey.DependencyProperty;
		[
		Category(Categories.Animation)
		]
		public bool EnableAnimation {
			get { return (bool)GetValue(EnableAnimationProperty); }
			set { SetValue(EnableAnimationProperty, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public ObservableCollection<IElementInfo> Elements {
			get { return (ObservableCollection<IElementInfo>)GetValue(ElementsProperty); }
		}
		static void EnableAnimationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			GaugeControlBase gauge = d as GaugeControlBase;
			if (gauge != null)
				gauge.Animate();
		}
		#region Hidden properties
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new HorizontalAlignment HorizontalContentAlignment { get { return base.HorizontalContentAlignment; } set { base.HorizontalContentAlignment = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new VerticalAlignment VerticalContentAlignment { get { return base.VerticalContentAlignment; } set { base.VerticalContentAlignment = value; } }
		#endregion
		UIElement baseLayoutElement = null;
		internal UIElement BaseLayoutElement { get { return baseLayoutElement; } }
		public GaugeControlBase() {
			Loaded += new RoutedEventHandler(GaugeLoaded);
			Unloaded += new RoutedEventHandler(GaugeUnloaded);
			this.SetValue(ElementsPropertyKey, new ObservableCollection<IElementInfo>());
		}
		#region IModelSupported implementation
		void IModelSupported.UpdateModel() {
			UpdateModel();
		}
		#endregion
		public override void OnApplyTemplate() {
			baseLayoutElement = GetTemplateChild("PART_BaseLayoutElement") as UIElement;
			base.OnApplyTemplate();
		}
		protected virtual void GaugeLoaded(object sender, RoutedEventArgs e) { }
		protected virtual void GaugeUnloaded(object sender, RoutedEventArgs e) { }
		protected internal abstract void Animate();
		protected abstract void UpdateModel();
		protected abstract IEnumerable<IElementInfo> GetElements();
		protected override Size MeasureOverride(Size constraint) {
			double constraintWidth = double.IsInfinity(constraint.Width) ? defaultWidth : constraint.Width;
			double constraintHeight = double.IsInfinity(constraint.Height) ? defaultHeight : constraint.Height;
			return base.MeasureOverride(new Size(constraintWidth, constraintHeight));			
		}
		internal void UpdateElements() {
			Elements.Clear();
			foreach (IElementInfo element in GetElements())
				Elements.Add(element);
		}
	}
	public abstract class AnalogGaugeControl : GaugeControlBase, ILogicalParent {
		public static readonly DependencyProperty ScalePanelTemplateProperty = DependencyPropertyManager.Register("ScalePanelTemplate",
			typeof(ItemsPanelTemplate), typeof(AnalogGaugeControl), new PropertyMetadata());
		public static readonly DependencyProperty ValueIndicatorProperty = DependencyPropertyManager.RegisterAttached("ValueIndicator",
			typeof(ValueIndicatorBase), typeof(AnalogGaugeControl), new PropertyMetadata(null, StateIndicatorControl.ValueIndicatorPropertyChanged));
		[
		Category(Categories.Common)
		]
		public static ValueIndicatorBase GetValueIndicator(StateIndicatorControl stateControl) {
			return (ValueIndicatorBase)stateControl.GetValue(ValueIndicatorProperty);
		}
		public static void SetValueIndicator(StateIndicatorControl stateControl, ValueIndicatorBase value) {
			stateControl.SetValue(ValueIndicatorProperty, value);
		}
		[
		Category(Categories.Presentation)
		]
		public ItemsPanelTemplate ScalePanelTemplate {
			get { return (ItemsPanelTemplate)GetValue(ScalePanelTemplateProperty); }
			set { SetValue(ScalePanelTemplateProperty, value); }
		}
		readonly NavigationController navigationController;
		readonly List<object> logicalChildren = new List<object>();
		protected override IEnumerator LogicalChildren { get { return logicalChildren.GetEnumerator(); } }
		public AnalogGaugeControl() {
			navigationController = new NavigationController(this);
			MouseLeftButtonUp += new MouseButtonEventHandler(navigationController.MouseLeftButtonUp);
			MouseLeftButtonDown += new MouseButtonEventHandler(navigationController.MouseLeftButtonDown);
			MouseMove += new MouseEventHandler(navigationController.MouseMove);
			MouseLeave += new MouseEventHandler(navigationController.MouseLeave);
			PreviewTouchUp += new EventHandler<TouchEventArgs>(navigationController.PreviewTouchUp);
			PreviewTouchDown += new EventHandler<TouchEventArgs>(navigationController.PreviewTouchDown);
			LostTouchCapture += new EventHandler<TouchEventArgs>(navigationController.LostTouchCapture);
		}
		#region ILogicalParent implementation
		void ILogicalParent.AddChild(object child) {
			if (!logicalChildren.Contains(child)) {
				logicalChildren.Add(child);
				AddLogicalChild(child);
			}
		}
		void ILogicalParent.RemoveChild(object child) {
			if (logicalChildren.Contains(child)) {
				logicalChildren.Remove(child);
				RemoveLogicalChild(child);
			}
		}
		#endregion
	}
}
