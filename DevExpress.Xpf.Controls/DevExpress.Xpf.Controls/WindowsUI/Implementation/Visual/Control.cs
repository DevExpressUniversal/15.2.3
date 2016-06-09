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
	public class veControlBase : Control, IDisposable {
		protected static DependencyProperty RegisterPropertyListener(string propertyName) {
			return DependencyProperty.Register(propertyName + "Listener", typeof(object), typeof(veContentControlBase),
				new PropertyMetadata(
					delegate(DependencyObject o, DependencyPropertyChangedEventArgs e) {
						var control = (veControlBase)o;
						if(!control._IsInitializingPropertyListener)
							control.OnPropertyChanged(e.Property, e.OldValue, e.NewValue);
					}));
		}
		public veControlBase() {
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
		protected virtual void ClearContainer() { }
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
	}
	[TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
	[TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
	[TemplateVisualState(Name = "Pressed", GroupName = "CommonStates")]
	[TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
	public class veControl : veControlBase, IControl {
		public veControl() {
			Controller = CreateController();
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
		public sealed override void OnApplyTemplate() {
			ClearTemplateChildren();
			base.OnApplyTemplate();
			GetTemplateChildren();
			OnApplyTemplateComplete();
		}
		protected virtual void ClearTemplateChildren() { }
		protected virtual void GetTemplateChildren() { }
		protected virtual void OnApplyTemplateComplete() {
			UpdateState(false);
		}
		void veContentControl_Loaded(object sender, RoutedEventArgs e) {
			OnLoaded();
		}
		void veContentControl_Unloaded(object sender, RoutedEventArgs e) {
			OnUnloaded();
		}
		void veContentControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) {
			OnIsEnabledChanged();
		}
		protected virtual void OnLoaded() { }
		protected virtual void OnUnloaded() { }
		protected virtual void OnIsEnabledChanged() { }
		public ControlControllerBase Controller { get; private set; }
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
