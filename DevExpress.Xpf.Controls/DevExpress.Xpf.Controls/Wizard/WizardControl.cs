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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Controls.Native;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Controls {
	[TemplatePart(Name = PART_Root, Type = typeof(FrameworkElement))]
	public class Wizard : Control {
		public static readonly DependencyProperty IsButtonBackProperty = DependencyProperty.RegisterAttached("IsButtonBack", typeof(bool), typeof(Wizard),
			new PropertyMetadata(false, (d, e) => OnIsWizardCommandChanged(d, (bool)e.OldValue, (bool)e.NewValue, WizardCommand.Back)));
		public static readonly DependencyProperty IsButtonNextProperty = DependencyProperty.RegisterAttached("IsButtonNext", typeof(bool), typeof(Wizard),
			new PropertyMetadata(false, (d, e) => OnIsWizardCommandChanged(d, (bool)e.OldValue, (bool)e.NewValue, WizardCommand.Next)));
		public static readonly DependencyProperty IsButtonCancelProperty = DependencyProperty.RegisterAttached("IsButtonCancel", typeof(bool), typeof(Wizard),
			new PropertyMetadata(false, (d, e) => OnIsWizardCommandChanged(d, (bool)e.OldValue, (bool)e.NewValue, WizardCommand.Cancel)));
		public static readonly DependencyProperty IsButtonFinishProperty = DependencyProperty.RegisterAttached("IsButtonFinish", typeof(bool), typeof(Wizard),
			new PropertyMetadata(false, (d, e) => OnIsWizardCommandChanged(d, (bool)e.OldValue, (bool)e.NewValue, WizardCommand.Finish)));
		public static bool GetIsButtonBack(DependencyObject obj) { return (bool)obj.GetValue(IsButtonBackProperty); }
		public static void SetIsButtonBack(DependencyObject obj, bool value) { obj.SetValue(IsButtonBackProperty, value); }
		public static bool GetIsButtonNext(DependencyObject obj) { return (bool)obj.GetValue(IsButtonNextProperty); }
		public static void SetIsButtonNext(DependencyObject obj, bool value) { obj.SetValue(IsButtonNextProperty, value); }
		public static bool GetIsButtonCancel(DependencyObject obj) { return (bool)obj.GetValue(IsButtonCancelProperty); }
		public static void SetIsButtonCancel(DependencyObject obj, bool value) { obj.SetValue(IsButtonCancelProperty, value); }
		public static bool GetIsButtonFinish(DependencyObject obj) { return (bool)obj.GetValue(IsButtonFinishProperty); }
		public static void SetIsButtonFinish(DependencyObject obj, bool value) { obj.SetValue(IsButtonFinishProperty, value); }
		static void OnIsWizardCommandChanged(DependencyObject sender, bool oldValue, bool newValue, WizardCommand buttonType) {
			if(!newValue) {
				var b = Interaction.GetBehaviors(sender).OfType<WizardCommandBehavior>().ToList();
				b.ForEach(x => Interaction.GetBehaviors(sender).Remove(x));
			} else {
				WizardCommandBehavior b = new WizardCommandBehavior() { ButtonType = buttonType };
				Interaction.GetBehaviors(sender).Add(b);
			}
		}
		public const string PART_Root = "Root";
		public static readonly object NavigationServiceKey = new object();
		public static readonly object CurrentDialogServiceKey = new object();
		static readonly DependencyPropertyKey ControllerPropertyKey;
		public static readonly DependencyProperty ControllerProperty;
		public static readonly DependencyProperty StartPageViewTypeProperty;
		public static readonly DependencyProperty StartPageViewModelProperty;
		public static readonly DependencyProperty ParameterProperty;
		public static readonly DependencyProperty ParentViewModelProperty;
		public static readonly DependencyProperty ViewTemplateProperty;
		public static readonly DependencyProperty ViewTemplateSelectorProperty;
		public static readonly DependencyProperty ViewLocatorProperty;
		public static readonly DependencyProperty FooterTemplateProperty;
		static Wizard() {
			DependencyPropertyRegistrator<Wizard>.New()
				.RegisterReadOnly(d => d.Controller, out ControllerPropertyKey, out ControllerProperty, null)
				.Register(d => d.StartPageViewType, out StartPageViewTypeProperty, null)
				.Register(d => d.StartPageViewModel, out StartPageViewModelProperty, null)
				.Register(d => d.Parameter, out ParameterProperty, null)
				.Register(d => d.ParentViewModel, out ParentViewModelProperty, null)
				.Register(d => d.ViewTemplate, out ViewTemplateProperty, null)
				.Register(d => d.ViewTemplateSelector, out ViewTemplateSelectorProperty, null)
				.Register(d => d.ViewLocator, out ViewLocatorProperty, null)
				.Register(d => d.FooterTemplate, out FooterTemplateProperty, null)
			;
		}
		public Wizard() {
			this.SetDefaultStyleKey(typeof(Wizard));
			Loaded += OnLoaded;
		}
		public DataTemplate ViewTemplate {
			get { return (DataTemplate)GetValue(ViewTemplateProperty); }
			set { SetValue(ViewTemplateProperty, value); }
		}
		public DataTemplateSelector ViewTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ViewTemplateSelectorProperty); }
			set { SetValue(ViewTemplateSelectorProperty, value); }
		}
		public IViewLocator ViewLocator {
			get { return (IViewLocator)GetValue(ViewLocatorProperty); }
			set { SetValue(ViewLocatorProperty, value); }
		}
		public DataTemplate FooterTemplate {
			get { return (DataTemplate)GetValue(FooterTemplateProperty); }
			set { SetValue(FooterTemplateProperty, value); }
		}
		public WizardController Controller {
			get { return (WizardController)GetValue(ControllerProperty); }
			private set { SetValue(ControllerPropertyKey, value); }
		}
		public string StartPageViewType {
			get { return (string)GetValue(StartPageViewTypeProperty); }
			set { SetValue(StartPageViewTypeProperty, value); }
		}
		public object StartPageViewModel {
			get { return GetValue(StartPageViewModelProperty); }
			set { SetValue(StartPageViewModelProperty, value); }
		}
		public object Parameter {
			get { return GetValue(ParameterProperty); }
			set { SetValue(ParameterProperty, value); }
		}
		public object ParentViewModel {
			get { return GetValue(ParentViewModelProperty); }
			set { SetValue(ParentViewModelProperty, value); }
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			Loaded -= OnLoaded;
			Controller = CreateController();
			Initialize();
		}
		protected virtual WizardController CreateController() {
			return new WizardController(() => NavigationService, () => CurrentDialogService, Parameter, ParentViewModel);
		}
		protected virtual void Initialize() {
			if(StartPageViewType != null && StartPageViewModel != null)
				throw new InvalidOperationException();
			if(StartPageViewType != null)
				Controller.NavigateToPage(StartPageViewType);
			else if(StartPageViewModel != null)
				Controller.NavigateTo(StartPageViewModel);
		}
		protected INavigationService NavigationService { get; private set; }
		protected ICurrentDialogService CurrentDialogService { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			var root = (FrameworkElement)GetTemplateChild(PART_Root);
			NavigationService = root.With(x => (INavigationService)x.Resources[NavigationServiceKey]);
			CurrentDialogService = root.With(x => (ICurrentDialogService)x.Resources[CurrentDialogServiceKey]);
		}
	}
}
