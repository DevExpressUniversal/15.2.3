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
using System.Windows.Controls.Primitives;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.WindowsUI.Navigation {
	public interface INavigationHandler {
		void RegisterSender(DependencyObject sender, object navigateTo, object param);
		void UnregisterSender(DependencyObject sender);
		void RegisterTarget(object target);
		void UnregisterTarget(object target);
	}
	public abstract class NavigationHandlerBase : INavigationHandler {
		protected abstract void RegisterSender(DependencyObject sender, object navigateTo, object navigationParameter);
		protected abstract void UnregisterSender(DependencyObject sender);
		protected void RegisterTarget(object target) {
			Target = target;
		}
		protected virtual void UnregisterTarget(object traget) {
			Target = null;
			Navigator = null;
		}
		protected abstract bool IsSenderSupported(DependencyObject target);
		NavigationTargetFactory NavigationTargetFactory = new NavigationTargetFactory();
		protected virtual INavigationTarget ResolveNavigationHost(object target) {
			return NavigationTargetFactory.Resolve(target);
		}
		INavigationTarget Navigator;
		object Target;
		protected virtual void InvokeNavigation(object sender, object navigatedTo, object navigationParameter) {
			if(navigatedTo == null) return;
			if(Target == null) {
				FrameworkElement dObj = sender as FrameworkElement;
				Target = NavigationHelper.GetNavigationContainer(dObj);
#if !SILVERLIGHT
				if(Target == null) Target = NavigationHelper.GetFrame(dObj);
#endif
			}
			if(Target == null) return;
			if(Navigator == null)
				Navigator = ResolveNavigationHost(Target);
			Navigator.Navigate(Target, navigatedTo, navigationParameter);
		}
		protected INavigationContainer Container;
		#region INavigationHandler Members
		void INavigationHandler.RegisterSender(DependencyObject sender, object navigateTo, object navigationParameter) {
			if(!IsSenderSupported(sender)) throw new NotSupportedException();
			RegisterSender(sender, navigateTo, navigationParameter);
		}
		void INavigationHandler.UnregisterSender(DependencyObject sender) {
			if(!IsSenderSupported(sender)) throw new NotSupportedException();
			UnregisterSender(sender);
		}
		void INavigationHandler.RegisterTarget(object target) {
			RegisterTarget(target);
		}
		void INavigationHandler.UnregisterTarget(object target) {
			UnregisterTarget(target);
		}
		#endregion
	}
	public class ClickableNavigationHandler : NavigationHandlerBase {
		EventHandler Handler;
		protected override void RegisterSender(DependencyObject sender, object navigateTo, object navigationParameter) {
			IClickable clickable = sender as IClickable;
			Handler = (s, e) => { InvokeNavigation(s, navigateTo, navigationParameter); };
			clickable.Click += Handler;
		}
		protected override void UnregisterSender(DependencyObject sender) {
			IClickable clickable = sender as IClickable;
			clickable.Click -= Handler;
		}
		protected override bool IsSenderSupported(DependencyObject target) {
			return target is IClickable;
		}
	}
	public class ButtonNavigationHandler : NavigationHandlerBase {
		RoutedEventHandler Handler;
		protected override void RegisterSender(DependencyObject sender, object navigateTo, object navigationParameter) {
			ButtonBase clickable = sender as ButtonBase;
			Handler = (s, e) => { InvokeNavigation(s, navigateTo, navigationParameter); };
			clickable.Click += Handler;
		}
		protected override void UnregisterSender(DependencyObject sender) {
			ButtonBase clickable = sender as ButtonBase;
			if(Handler != null)
				clickable.Click -= Handler;
		}
		protected override bool IsSenderSupported(DependencyObject target) {
			return target is ButtonBase;
		}
	}
}
