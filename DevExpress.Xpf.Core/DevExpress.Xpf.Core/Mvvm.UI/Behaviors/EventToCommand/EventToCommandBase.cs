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
using System.Windows;
using System.Windows.Input;
#if !NETFX_CORE
using System.Windows.Controls;
using System.Windows.Threading;
#else
using Windows.UI.Xaml;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endif
namespace DevExpress.Mvvm.UI {
	public abstract class EventToCommandBase : DevExpress.Mvvm.UI.Interactivity.EventTrigger {
		public static readonly DependencyProperty CommandProperty =
		   DependencyProperty.Register("Command", typeof(ICommand), typeof(EventToCommandBase),
		   new PropertyMetadata(null, (d, e) => ((EventToCommandBase)d).OnCommandChanged()));
		public static readonly DependencyProperty CommandParameterProperty =
		   DependencyProperty.Register("CommandParameter", typeof(object), typeof(EventToCommandBase),
		   new PropertyMetadata(null, (d, e) => ((EventToCommandBase)d).OnCommandParameterChanged()));
		public static readonly DependencyProperty ProcessEventsFromDisabledEventOwnerProperty =
			DependencyProperty.Register("ProcessEventsFromDisabledEventOwner", typeof(bool), typeof(EventToCommandBase),
			new PropertyMetadata(true));
		public static readonly DependencyProperty MarkRoutedEventsAsHandledProperty =
			DependencyProperty.Register("MarkRoutedEventsAsHandled", typeof(bool), typeof(EventToCommandBase),
			new PropertyMetadata(false));
		public static readonly DependencyProperty UseDispatcherProperty =
			DependencyProperty.Register("UseDispatcher", typeof(bool?), typeof(EventToCommandBase),
			new PropertyMetadata(null));
#if !SILVERLIGHT && !NETFX_CORE
		public static readonly DependencyProperty DispatcherPriorityProperty =
			DependencyProperty.Register("DispatcherPriority", typeof(DispatcherPriority?), typeof(EventToCommandBase),
			new PropertyMetadata(null));
#endif
		public ICommand Command {
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}
		public object CommandParameter {
			get { return GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}
		public bool ProcessEventsFromDisabledEventOwner {
			get { return (bool)GetValue(ProcessEventsFromDisabledEventOwnerProperty); }
			set { SetValue(ProcessEventsFromDisabledEventOwnerProperty, value); }
		}
		public bool MarkRoutedEventsAsHandled {
			get { return (bool)GetValue(MarkRoutedEventsAsHandledProperty); }
			set { SetValue(MarkRoutedEventsAsHandledProperty, value); }
		}
		public bool? UseDispatcher {
			get { return (bool?)GetValue(UseDispatcherProperty); }
			set { SetValue(UseDispatcherProperty, value); }
		}
		protected internal bool ActualUseDispatcher {
			get {
				if(UseDispatcher == null) {
#if !SILVERLIGHT && !NETFX_CORE
					return DispatcherPriority != null;
#else
					return false;
#endif
				}
				return UseDispatcher.Value;
			}
		}
#if !SILVERLIGHT && !NETFX_CORE
		public DispatcherPriority? DispatcherPriority {
			get { return (DispatcherPriority?)GetValue(DispatcherPriorityProperty); }
			set { SetValue(DispatcherPriorityProperty, value); }
		}
		protected internal DispatcherPriority ActualDispatcherPriority {
			get { return DispatcherPriority ?? System.Windows.Threading.DispatcherPriority.Normal; }
		}
#endif
		protected override void OnEvent(object sender, object eventArgs) {
			base.OnEvent(sender, eventArgs);
			if(Command != null) {
				OnEventCore(sender, eventArgs);
				return;
			}
#if !SILVERLIGHT && !NETFX_CORE
			bool commandIsBound = System.Windows.Data.BindingOperations.GetBindingExpression(this, CommandProperty) != null;
			if(Command == null && commandIsBound) {
				Dispatcher.BeginInvoke(new Action(() => {
					OnEventCore(sender, eventArgs);
				}));
			}
#endif
		}
		void OnEventCore(object sender, object eventArgs) {
			if(Command == null) return;
			if(!CanInvoke(sender, eventArgs)) return;
			if(!ActualUseDispatcher)
				Invoke(sender, eventArgs);
			else {
#if SILVERLIGHT
				Dispatcher.BeginInvoke(new Action<object, object>(Invoke), new object[] { sender, eventArgs });
#elif NETFX_CORE
#pragma warning disable 4014
				Dispatcher.RunAsync(CoreDispatcherPriority.Low, new DispatchedHandler(() => Invoke(sender, eventArgs)));
#pragma warning restore 4014
#else
				Dispatcher.BeginInvoke(new Action<object, object>(Invoke), ActualDispatcherPriority, new object[] { sender, eventArgs });
#endif
			}
			if(MarkRoutedEventsAsHandled) {
#if SILVERLIGHT
				if(eventArgs is MouseButtonEventArgs)
					((MouseButtonEventArgs)eventArgs).Handled = true;
				if(eventArgs is KeyEventArgs)
					((KeyEventArgs)eventArgs).Handled = true;
#elif NETFX_CORE
				if(eventArgs is KeyEventArgs)
					((KeyEventArgs)eventArgs).Handled = true; 
#else
				if(eventArgs is RoutedEventArgs)
					((RoutedEventArgs)eventArgs).Handled = true;
#endif
			}
		}
		protected abstract void Invoke(object sender, object eventArgs);
		protected virtual bool CanInvoke(object sender, object eventArgs) {
#if !SILVERLIGHT && !NETFX_CORE
			FrameworkElement associatedFrameworkObject = Source as FrameworkElement;
#else
			Control associatedFrameworkObject = Source as Control;
#endif
			return ProcessEventsFromDisabledEventOwner || associatedFrameworkObject == null || associatedFrameworkObject.IsEnabled;
		}
		protected virtual void OnCommandChanged() { }
		protected virtual void OnCommandParameterChanged() { }
	}
}
