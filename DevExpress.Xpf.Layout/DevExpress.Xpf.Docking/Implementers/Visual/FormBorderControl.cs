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
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
namespace DevExpress.Xpf.Docking.VisualElements {
	public class FormBorderControl : psvControl {
		#region static
		public static readonly DependencyProperty SingleBorderTemplateProperty;
		public static readonly DependencyProperty FormBorderTemplateProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty ActualShadowMarginProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty ActualContentMarginProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty ActualBorderMarginProperty;
		public static readonly DependencyProperty BorderStyleProperty;
		public static readonly DependencyProperty IsMaximizedProperty;
		public static readonly DependencyProperty EmptyBorderTemplateProperty;
		static FormBorderControl() {
			var dProp = new DependencyPropertyRegistrator<FormBorderControl>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("EmptyBorderTemplate", ref EmptyBorderTemplateProperty, (ControlTemplate)null,
				(dObj, e) => ((FormBorderControl)dObj).SelectContainerTemplate());
			dProp.Register("SingleBorderTemplate", ref SingleBorderTemplateProperty, (ControlTemplate)null,
				(dObj, e) => ((FormBorderControl)dObj).SelectContainerTemplate());
			dProp.Register("FormBorderTemplate", ref FormBorderTemplateProperty, (ControlTemplate)null,
				(dObj, e) => ((FormBorderControl)dObj).SelectContainerTemplate());
			dProp.Register("BorderStyle", ref BorderStyleProperty, FloatGroupBorderStyle.Form,
				(dObj, e) => ((FormBorderControl)dObj).OnBorderStyleChanged((FloatGroupBorderStyle)e.OldValue, (FloatGroupBorderStyle)e.NewValue));
			dProp.Register("IsMaximized", ref IsMaximizedProperty, false,
				(dObj, e) => ((FormBorderControl)dObj).OnIsMaximizedChanged((bool)e.OldValue, (bool)e.NewValue));
			dProp.Register("ActualBorderMargin", ref ActualBorderMarginProperty, new Thickness());
			dProp.Register("ActualShadowMargin", ref ActualShadowMarginProperty, new Thickness());
			dProp.Register("ActualContentMargin", ref ActualContentMarginProperty, new Thickness());
		}
		#endregion
		public FormBorderControl() {
			DefaultStyleKey = typeof(FormBorderControl);
		}
		protected override void OnDispose() {
			if(PartItemsControl != null) {
				PartItemsControl.Dispose();
				PartItemsControl = null;
			}
			base.OnDispose();
		}
		protected virtual void OnBorderStyleChanged(FloatGroupBorderStyle oldValue, FloatGroupBorderStyle newValue) {
			SelectContainerTemplate();
		}
		protected virtual void OnIsMaximizedChanged(bool oldValue, bool newValue) {
			UpdateVisualState();
		}
		void UpdateVisualState() {
			VisualStateManager.GoToState(this, IsMaximized ? "Maximized" : "EmptySizeState", false);
		}
		void SelectContainerTemplate() {
#if !SILVERLIGHT
			if(!IsInitialized) return;
#endif
			switch(BorderStyle) {
				case FloatGroupBorderStyle.Empty:
					Template = EmptyBorderTemplate;
					break;
				case DevExpress.Xpf.Docking.FloatGroupBorderStyle.Single:
					Template = SingleBorderTemplate;
					break;
				case DevExpress.Xpf.Docking.FloatGroupBorderStyle.Form:
					Template = FormBorderTemplate;
					break;
			}
		}
		protected LayoutItemsControl PartItemsControl { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if(PartItemsControl != null && !LayoutItemsHelper.IsTemplateChild(PartItemsControl, this))
				PartItemsControl.Dispose();
			var contentControl = LayoutItemsHelper.GetTemplateChild<FormBorderContentControl>(this);
			PartItemsControl = contentControl != null ? contentControl.Content as LayoutItemsControl :
				LayoutItemsHelper.GetTemplateChild<LayoutItemsControl>(this);
			UpdateVisualState();
		}
		protected override void OnInitialized() {
			base.OnInitialized();
			SelectContainerTemplate();
		}
		protected virtual void OnIsActiveChanged(bool value) {
			UpdateVisualState();
		}
		public ControlTemplate FormBorderTemplate {
			get { return (ControlTemplate)GetValue(FormBorderTemplateProperty); }
			set { SetValue(FormBorderTemplateProperty, value); }
		}
		public ControlTemplate SingleBorderTemplate {
			get { return (ControlTemplate)GetValue(SingleBorderTemplateProperty); }
			set { SetValue(SingleBorderTemplateProperty, value); }
		}
		public bool IsMaximized {
			get { return (bool)GetValue(IsMaximizedProperty); }
			set { SetValue(IsMaximizedProperty, value); }
		}
		public FloatGroupBorderStyle BorderStyle {
			get { return (FloatGroupBorderStyle)GetValue(BorderStyleProperty); }
			set { SetValue(BorderStyleProperty, value); }
		}
		public ControlTemplate EmptyBorderTemplate {
			get { return (ControlTemplate)GetValue(EmptyBorderTemplateProperty); }
			set { SetValue(EmptyBorderTemplateProperty, value); }
		}
	}
	public class FormBorderContentControl : ContentControl {
		#region static
		public static readonly DependencyProperty IsActiveProperty;
		static FormBorderContentControl() {
			var dProp = new DependencyPropertyRegistrator<FormBorderContentControl>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("IsActive", ref IsActiveProperty, false,
				(dObj, e) => ((FormBorderContentControl)dObj).OnIsActiveChanged((bool)e.OldValue, (bool)e.NewValue));
		}
		#endregion
		public FormBorderContentControl() {
#if SILVERLIGHT
			DefaultStyleKey = typeof(FormBorderContentControl);
#endif
		}
		protected virtual void OnIsActiveChanged(bool oldValue, bool newValue) {
			UpdateVisualState();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateVisualState();
		}
		protected virtual void UpdateVisualState() {
		}
		public bool IsActive {
			get { return (bool)GetValue(IsActiveProperty); }
			set { SetValue(IsActiveProperty, value); }
		}
	}
	public class DocumentFormBorderContentControl : FormBorderContentControl {
		protected override void UpdateVisualState() {
			VisualStateManager.GoToState(this, IsActive ? "Active" : "Inactive", false);
		}
	}
	public class ResizeBoundsControl : Control {
		#region static
		static ResizeBoundsControl() {
			var dProp = new DependencyPropertyRegistrator<ResizeBoundsControl>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
		}
		#endregion
		public ResizeBoundsControl() {
#if SILVERLIGHT
			DefaultStyleKey = typeof(ResizeBoundsControl);
#endif
		}
	}
	public class FloatingDragWidget : Border {
		public FloatingDragWidget() {
		}
	}
}
