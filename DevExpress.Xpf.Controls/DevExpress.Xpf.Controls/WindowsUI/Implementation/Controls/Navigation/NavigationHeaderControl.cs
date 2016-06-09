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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.WindowsUI.Base;
using DevExpress.Xpf.WindowsUI.Navigation;
#if SILVERLIGHT
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.WindowsUI.Internal {
	public class NavigationHeaderControl : veContentControl {
		public static readonly DependencyProperty BackCommandProperty;
		public static readonly DependencyProperty BackCommandParameterProperty;
		static readonly DependencyPropertyKey ActualBackCommandPropertyKey;
		public static readonly DependencyProperty ActualBackCommandProperty;
		public static readonly DependencyProperty ShowBackButtonProperty;
		public bool ShowBackButton {
			get { return (bool)GetValue(ShowBackButtonProperty); }
			set { SetValue(ShowBackButtonProperty, value); }
		}
		static NavigationHeaderControl() {
			var dProp = new DependencyPropertyRegistrator<NavigationHeaderControl>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("BackCommand", ref BackCommandProperty, (ICommand)null,
				(d, e) => ((NavigationHeaderControl)d).OnBackCommandChanged((ICommand)e.OldValue, (ICommand)e.NewValue));
			dProp.Register("BackCommandParameter", ref BackCommandParameterProperty, (object)null);
			dProp.RegisterReadonly("ActualBackCommand", ref ActualBackCommandPropertyKey, ref ActualBackCommandProperty, (ICommand)null);
			dProp.Register("ShowBackButton", ref ShowBackButtonProperty, true);
		}
		public NavigationHeaderControl() {
#if SILVERLIGHT
			DefaultStyleKey = typeof(NavigationHeaderControl);
#endif
			ActualBackCommand = DefaultBackCommand;
		}
		internal BackButton PartBackButton;
		public ContentPresenter PartContent { get; private set; }
		protected override void GetTemplateChildren() {
			base.GetTemplateChildren();
			PartBackButton = GetTemplateChild("PART_BackButton") as BackButton;
			PartContent = GetTemplateChild("PART_Content") as ContentPresenter;
		}
		protected override void OnApplyTemplateComplete() {
			base.OnApplyTemplateComplete();
			if(PartBackButton != null) PartBackButton.SetBinding(Control.IsEnabledProperty, new Binding("ShowBackButton") { Source = this });
		}
		public void OnBackCommandChanged(ICommand oldValue, ICommand newValue) {
			ActualBackCommand = newValue ?? DefaultBackCommand;
		}
		protected override void OnLoaded() {
			base.OnLoaded();
			DefaultBackCommand.RaiseCanExecuteChanged();
		}
		public ICommand BackCommand {
			get { return (ICommand)GetValue(BackCommandProperty); }
			set { SetValue(BackCommandProperty, value); }
		}
		public virtual double HeaderOffset {
			get {
				return PartContent != null ? PartContent.TranslatePoint(new Point(), this).X : 0d;
			}
		}
		DelegateCommand _DefaultBackCommand;
		DelegateCommand DefaultBackCommand {
			get {
				if(_DefaultBackCommand == null)
					_DefaultBackCommand = DelegateCommandFactory.Create(
						() => { NavigationHelper.GoBack(this, BackCommandParameter); },
						() => { return NavigationHelper.CanGoBack(this); }, false);
				return _DefaultBackCommand;
			}
		}
		public ICommand ActualBackCommand {
			get { return (ICommand)GetValue(ActualBackCommandProperty); }
			private set { this.SetValue(ActualBackCommandPropertyKey, value); }
		}
		public object BackCommandParameter {
			get { return GetValue(BackCommandParameterProperty); }
			set { SetValue(BackCommandParameterProperty, value); }
		}
	}
}
