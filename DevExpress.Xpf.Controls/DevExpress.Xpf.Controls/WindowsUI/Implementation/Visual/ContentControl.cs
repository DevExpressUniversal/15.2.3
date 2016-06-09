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
using System.Windows.Data;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.WindowsUI.Base {
	public class veContentControlBase : ContentControl, IDisposable {
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty BackgroundListener;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty ForegroundListener;
		static veContentControlBase() {
			ForegroundListener = RegisterPropertyListener("Foreground");
			BackgroundListener = RegisterPropertyListener("Background");
		}
		protected static DependencyProperty RegisterPropertyListener(string propertyName) {
			return DependencyProperty.Register(propertyName + "Listener", typeof(object), typeof(veContentControlBase),
				new PropertyMetadata(
					delegate(DependencyObject o, DependencyPropertyChangedEventArgs e) {
						var control = (veContentControlBase)o;
						if(!control._IsInitializingPropertyListener)
							control.OnPropertyChanged(e.Property, e.OldValue, e.NewValue);
					}));
		}
		public veContentControlBase() {
			SubscribeEvents();
		}
		public bool IsDisposing { get; private set; }
		void IDisposable.Dispose() {
			if(!IsDisposing) {
				IsDisposing = true;
				UnSubscribeEvents();
				OnDispose();
			}
			GC.SuppressFinalize(this);
		}
		protected virtual void SubscribeEvents() { }
		protected virtual void UnSubscribeEvents() { }
		protected virtual void OnDispose() {
			ClearContainer();
		}
		protected virtual void ClearContainer() {
			ClearValue(ContentProperty);
		}
		private bool _IsInitializingPropertyListener;
		protected void AttachPropertyListener(string propertyName, DependencyProperty propertyListener, object source = null) {
			_IsInitializingPropertyListener = true;
			try {
				SetBinding(propertyListener, new Binding(propertyName) { Source = source ?? this });
			}
			finally {
				_IsInitializingPropertyListener = false;
			}
		}
		protected void DetachPropertyListener(DependencyProperty propertyListener) {
			_IsInitializingPropertyListener = true;
			try {
				ClearValue(propertyListener);
			}
			finally {
				_IsInitializingPropertyListener = false;
			}
		}
		protected virtual void OnPropertyChanged(DependencyProperty propertyListener, object oldValue, object newValue) { }
#if SILVERLIGHT
		public bool IsMouseOver { get; private set; }
		protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e) {
			base.OnMouseEnter(e);
			IsMouseOver = true;
		}
		protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e) {
			base.OnMouseLeave(e);
			IsMouseOver = false;
		}
#endif
	}
	public class veContentControl : veContentControlBase, IDisposable, IItemContainer {
		#region static
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty ContentTemplateInternalProperty;
#if SILVERLIGHT
		public static readonly DependencyProperty ContentTemplateSelectorProperty;
		public static readonly DependencyProperty HasContentProperty;
#endif
		static veContentControl() {
			var dProp = new DependencyPropertyRegistrator<veContentControl>();
			dProp.Register("ContentTemplateInternal", ref ContentTemplateInternalProperty, (DataTemplate)null,
				(d, e) => ((veContentControl)d).OnContentTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue));
#if SILVERLIGHT
			dProp.Register("HasContent", ref HasContentProperty, (bool)false);
			dProp.Register("ContentTemplateSelector", ref ContentTemplateSelectorProperty, (DataTemplateSelector)null,
				(d, e) => ((veContentControl)d).OnContentTemplateSelectorChanged((DataTemplateSelector)e.OldValue, (DataTemplateSelector)e.NewValue));
#endif
		}
		#endregion static
		public veContentControl() {
			SetBinding(ContentTemplateInternalProperty, new Binding() { Path = new PropertyPath("ContentTemplate"), Source = this });
		}
		protected override void SubscribeEvents() {
			Loaded += veContentControl_Loaded;
			Unloaded += veContentControl_Unloaded;
			IsEnabledChanged += veContentControl_IsEnabledChanged;
		}
		protected override void UnSubscribeEvents() {
			IsEnabledChanged -= veContentControl_IsEnabledChanged;
			Loaded -= veContentControl_Loaded;
			Unloaded -= veContentControl_Unloaded;
		}
#if SILVERLIGHT
		public bool IsLoaded { get; private set; }
#endif
		public sealed override void OnApplyTemplate() {
			ClearTemplateChildren();
			base.OnApplyTemplate();
			GetTemplateChildren();
			OnApplyTemplateComplete();
		}
		protected virtual void ClearTemplateChildren() { }
		protected virtual void GetTemplateChildren() { }
		protected virtual void OnApplyTemplateComplete() {
		}
		void veContentControl_Loaded(object sender, RoutedEventArgs e) {
#if SILVERLIGHT
			this.IsLoaded = true;
#endif
			OnLoaded();
		}
		void veContentControl_Unloaded(object sender, RoutedEventArgs e) {
			OnUnloaded();
#if SILVERLIGHT
			this.IsLoaded = false;
#endif
		}
		void veContentControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) {
			OnIsEnabledChanged();
		}
		protected override void OnContentChanged(object oldContent, object newContent) {
			base.OnContentChanged(oldContent, newContent);
#if SILVERLIGHT
			SetValue(HasContentProperty, newContent != null);
#endif
		}
#if SILVERLIGHT
		protected virtual void OnContentTemplateChanged(DataTemplate oldTemplate, DataTemplate newTemplate) { }
		protected virtual void OnContentTemplateSelectorChanged(DataTemplateSelector oldTemplate, DataTemplateSelector newTemplate) { }
#endif
		protected virtual void PrepareContainer(object item, DataTemplate itemTemplate, DataTemplateSelector itemTemplateSelector) { }
		#region Properties
#if SILVERLIGHT
		public bool HasContent {
			get { return (bool)GetValue(HasContentProperty); }
		}
		public DataTemplateSelector ContentTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty); }
			set { SetValue(ContentTemplateSelectorProperty, value); }
		}
#endif
		#endregion Properties
		protected virtual void OnLoaded() { }
		protected virtual void OnUnloaded() { }
		protected virtual void OnIsEnabledChanged() { }
		protected virtual void EnsureContentElementCore(DependencyObject element) { }
		protected virtual void ReleaseContentElementCore(DependencyObject element) { }
		public static void EnsureContentElement<T>(DependencyObject element, ContentPresenter presenter) where T : veContentControl {
			if(element == null) return;
			T owner = LayoutTreeHelper.GetTemplateParent<T, veContentControl>(presenter);
			if(owner != null)
				owner.EnsureContentElementCore(element);
		}
		public static void ReleaseContentElement<T>(DependencyObject element, ContentPresenter presenter) where T : veContentControl {
			if(element == null) return;
			T owner = LayoutTreeHelper.GetTemplateParent<T, veContentControl>(presenter);
			if(owner != null)
				owner.ReleaseContentElementCore(element);
		}
		#region IItemContainer Members
		void IItemContainer.PrepareContainer(object item, DataTemplate itemTemplate, DataTemplateSelector itemTemplateSelector) {
			PrepareContainer(item, itemTemplate, itemTemplateSelector);
		}
		void IItemContainer.ClearContainer() {
			ClearContainer();
		}
		#endregion
	}
	[TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
	[TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
	[TemplateVisualState(Name = "Pressed", GroupName = "CommonStates")]
	[TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
	public abstract class veContentContainer : veContentControl, IControl {
		public ControlControllerBase Controller { get; private set; }
		protected veContentContainer() {
			Controller = CreateController();
		}
		protected override void OnApplyTemplateComplete() {
			base.OnApplyTemplateComplete();
			UpdateState(false);
		}
		protected virtual ControlControllerBase CreateController() {
			return new ControlControllerBase(this);
		}
		protected virtual void UpdateState(bool useTransitions) {
			Controller.UpdateState(false);
		}
		#region IControl
		FrameworkElement IControl.Control { get { return this; } }
		Controller IControl.Controller { get { return Controller; } }
		#endregion IControl
	}
}
