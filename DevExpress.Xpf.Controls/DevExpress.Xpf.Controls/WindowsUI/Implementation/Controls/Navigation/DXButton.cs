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

using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.WindowsUI.Navigation;
using System;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.WindowsUI.Base;
using System.ComponentModel;
namespace DevExpress.Xpf.WindowsUI {
#if !SILVERLIGHT
#endif
	public class NavigationButton : Button {
		#region static
		public static readonly DependencyProperty NavigateToProperty;
		public static readonly DependencyProperty NavigationParameterProperty;
		public static readonly DependencyProperty NavigationTargetProperty;
		static NavigationButton() {
			var dProp = new DependencyPropertyRegistrator<NavigationButton>();
			dProp.Register("NavigateTo", ref NavigateToProperty, (object)null,
				(d, e) => ((NavigationButton)d).OnNavigateToChanged(e.OldValue, e.NewValue));
			dProp.Register("NavigationParameter", ref NavigationParameterProperty, (object)null,
				(d, e) => ((NavigationButton)d).OnNavigationParameterChanged(e.OldValue, e.NewValue));
			dProp.Register("NavigationTarget", ref NavigationTargetProperty, (object)null,
				(d, e) => ((NavigationButton)d).OnNavigationTargetChanged(e.OldValue, e.NewValue));
		}
		#endregion
		public void OnNavigateToChanged(object oldValue, object newValue) {
			Navigation.Navigation.SetNavigateTo(this, newValue);
		}
		public void OnNavigationParameterChanged(object oldValue, object newValue) {
			Navigation.Navigation.SetNavigationParameter(this, newValue);
		}
		public void OnNavigationTargetChanged(object oldValue, object newValue) {
			Navigation.Navigation.SetNavigationTarget(this, newValue);
		}
		protected internal void PerformClick() {
			OnClick();
		}
		public object NavigateTo {
			get { return GetValue(NavigateToProperty); }
			set { SetValue(NavigateToProperty, value); }
		}
		public object NavigationParameter {
			get { return GetValue(NavigationParameterProperty); }
			set { SetValue(NavigationParameterProperty, value); }
		}
		public object NavigationTarget {
			get { return GetValue(NavigationTargetProperty); }
			set { SetValue(NavigationTargetProperty, value); }
		}
	}
}
