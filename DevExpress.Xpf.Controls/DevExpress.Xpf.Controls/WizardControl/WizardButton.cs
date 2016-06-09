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

using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Windows;
namespace DevExpress.Xpf.Controls.Native {
	public enum WizardButtonKind { Back, Next, NavigateTo, Cancel, Finish }
	public class WizardButtonBehavior : Behavior<DependencyObject> {
		public static readonly DependencyProperty ButtonTypeProperty = DependencyProperty.Register("ButtonType", typeof(WizardButtonKind), typeof(WizardButtonBehavior), new PropertyMetadata(null));
		public static readonly DependencyProperty CommandPropertyNameProperty = DependencyProperty.Register("CommandPropertyName", typeof(string), typeof(WizardButtonBehavior), new PropertyMetadata("Command"));
		public WizardButtonKind ButtonType { get { return (WizardButtonKind)GetValue(ButtonTypeProperty); } set { SetValue(ButtonTypeProperty, value); } }
		public string CommandPropertyName { get { return (string)GetValue(CommandPropertyNameProperty); } set { SetValue(CommandPropertyNameProperty, value); } }
		WizardControl owner;
		WizardControl Owner {
			get { return owner; }
			set {
				if(owner == value) return;
				var oldValue = owner;
				owner = value;
				OnOwnerChanged(oldValue, value);
			}
		}
		DelegateCommand command;
		DelegateCommand Command {
			get { return command; }
			set {
				if(command == value) return;
				var oldValue = command;
				command = value;
				OnCommandChanged(oldValue, value);
			}
		}
		protected override void OnAttached() {
			base.OnAttached();
			Owner = FindOwner();
			if(Owner == null) SubsribeAssociatedObject(AssociatedObject);
		}
		protected override void OnDetaching() {
			UnsubsribeAssociatedObject(AssociatedObject);
			Command = null;
			Owner = null;
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
			Owner = FindOwner();
			if(Owner != null) UnsubsribeAssociatedObject(AssociatedObject);
		}
		void OnAssociatedObjectLayoutUpdated(object sender, EventArgs e) {
			Owner = FindOwner();
			if(Owner != null) UnsubsribeAssociatedObject(AssociatedObject);
		}
		void OnOwnerChanged(WizardControl oldValue, WizardControl newValue) {
			if(newValue == null) return;
			oldValue.Do(x => x.SelectionChanged -= OnOwnerSelectionChanged);
			newValue.Do(x => x.SelectionChanged += OnOwnerSelectionChanged);
			oldValue.Do(x => x.ItemsChanged -= OnOwnerItemsChanged);
			newValue.Do(x => x.ItemsChanged += OnOwnerItemsChanged);
			Command = new DelegateCommand(OnCommandExecute, OnCommandCanExecute, false);
		}
		void OnOwnerSelectionChanged(object sender, SelectorSelectionChangedEventArgs e) {
			Command.Do(x => x.RaiseCanExecuteChanged());
		}
		void OnOwnerItemsChanged(object sender, NotifyCollectionChangedEventArgs e) {
			Command.Do(x => x.RaiseCanExecuteChanged());
		}
		void OnCommandChanged(DelegateCommand oldValue, DelegateCommand newValue) {
			Type associatedObjectType = AssociatedObject.GetType();
			PropertyInfo commandPropertyInfo = associatedObjectType.GetProperty(CommandPropertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
			if(commandPropertyInfo == null) return;
			commandPropertyInfo.SetValue(AssociatedObject, newValue, null);
		}
		void OnCommandExecute() {
			if(Owner == null) return;
			if(ButtonType == WizardButtonKind.Back)
				Owner.Back();
			else if(ButtonType == WizardButtonKind.Next)
				Owner.Next();
			else if(ButtonType == WizardButtonKind.Cancel)
				Owner.Cancel();
			else if(ButtonType == WizardButtonKind.Finish)
				Owner.Finish();
			else if(ButtonType == WizardButtonKind.NavigateTo)
				Owner.NavigateTo(WizardControl.GetIsButtonNavigateTo(AssociatedObject));
		}
		bool OnCommandCanExecute() {
			if(Owner == null) return false;
			if(ButtonType == WizardButtonKind.Back)
				return Owner.CanBack();
			else if(ButtonType == WizardButtonKind.Next)
				return Owner.CanNext();
			else if(ButtonType == WizardButtonKind.Cancel)
				return Owner.CanCancel();
			else if(ButtonType == WizardButtonKind.Finish)
				return Owner.CanFinish();
			else if(ButtonType == WizardButtonKind.NavigateTo)
				return Owner.CanNavigateTo(WizardControl.GetIsButtonNavigateTo(AssociatedObject));
			return true;
		}
		WizardControl FindOwner() {
			return LayoutTreeHelper.GetVisualParents(AssociatedObject).OfType<WizardControl>().FirstOrDefault();
		}
	}
}
