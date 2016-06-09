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
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.Windows.Input;
namespace DevExpress.Xpf.Controls.Native {
	public enum WizardCommand { Back, Next, Cancel, Finish }
	public class WizardCommandBehavior : Behavior<DependencyObject> {
		public static readonly DependencyProperty ButtonTypeProperty = DependencyProperty.Register("ButtonType", typeof(WizardCommand), typeof(WizardCommandBehavior), new PropertyMetadata(null));
		public static readonly DependencyProperty CommandPropertyNameProperty = DependencyProperty.Register("CommandPropertyName", typeof(string), typeof(WizardCommandBehavior), new PropertyMetadata("Command"));
		public WizardCommand ButtonType { get { return (WizardCommand)GetValue(ButtonTypeProperty); } set { SetValue(ButtonTypeProperty, value); } }
		public string CommandPropertyName { get { return (string)GetValue(CommandPropertyNameProperty); } set { SetValue(CommandPropertyNameProperty, value); } }
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty CommandProperty;
		static WizardCommandBehavior() {
			DependencyPropertyRegistrator<WizardCommandBehavior>.New()
				.Register(d => d.Command, out CommandProperty, null, d => d.OnCommandChanged());
			;
		}
		Wizard wizard;
		ICommand Command {
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}
		protected override void OnAttached() {
			base.OnAttached();
			UpdateCommand();
			if(wizard == null) SubsribeAssociatedObject(AssociatedObject);
		}
		protected override void OnDetaching() {
			UnsubsribeAssociatedObject(AssociatedObject);
			Command = null;
			wizard = null;
			base.OnDetaching();
		}
		void SubsribeAssociatedObject(object obj) {
			FrameworkElement AssociatedFrameworkElement = obj as FrameworkElement;
			FrameworkContentElement AssociatedFrameworkContentElement = obj as FrameworkContentElement;
			AssociatedFrameworkElement.Do(x => x.Loaded += OnAssociatedObjectLoaded);
			AssociatedFrameworkElement.Do(x => x.LayoutUpdated += OnAssociatedObjectLayoutUpdated);
			AssociatedFrameworkContentElement.Do(x => x.Loaded += OnAssociatedObjectLoaded);
		}
		void UnsubsribeAssociatedObject(object obj) {
			FrameworkElement AssociatedFrameworkElement = obj as FrameworkElement;
			FrameworkContentElement AssociatedFrameworkContentElement = obj as FrameworkContentElement;
			AssociatedFrameworkElement.Do(x => x.Loaded -= OnAssociatedObjectLoaded);
			AssociatedFrameworkElement.Do(x => x.LayoutUpdated -= OnAssociatedObjectLayoutUpdated);
			AssociatedFrameworkContentElement.Do(x => x.Loaded -= OnAssociatedObjectLoaded);
		}
		void OnAssociatedObjectLoaded(object sender, RoutedEventArgs e) {
			UpdateCommand();
			if(wizard != null)
				UnsubsribeAssociatedObject(AssociatedObject);
		}
		void OnAssociatedObjectLayoutUpdated(object sender, EventArgs e) {
			UpdateCommand();
			if(wizard != null)
				UnsubsribeAssociatedObject(AssociatedObject);
		}
		void UpdateCommand() {
			wizard = FindWizard();
			if(wizard == null)
				Command = null;
			else
				BindingOperations.SetBinding(this, CommandProperty, new Binding("Controller." + ButtonType.ToString() + "Command") { Source = wizard, Mode = BindingMode.OneWay });
		}
		void OnCommandChanged() {
			Type associatedObjectType = AssociatedObject.GetType();
			PropertyInfo commandPropertyInfo = associatedObjectType.GetProperty(CommandPropertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
			if(commandPropertyInfo == null) return;
			commandPropertyInfo.SetValue(AssociatedObject, Command, null);
		}
		Wizard FindWizard() {
			return LinqExtensions.Unfold<DependencyObject>(AssociatedObject, x => LayoutHelper.GetParent(x, true), x => x == null).OfType<Wizard>().FirstOrDefault();
		}
	}
}
