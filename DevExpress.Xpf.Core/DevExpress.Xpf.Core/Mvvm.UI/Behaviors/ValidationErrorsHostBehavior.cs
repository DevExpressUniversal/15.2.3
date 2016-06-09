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

using DevExpress.Mvvm.UI.Interactivity;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Mvvm.UI {
	[TargetType(typeof(FrameworkElement))]
	public class ValidationErrorsHostBehavior : Behavior<FrameworkElement> {
		#region Dependency Properties
		public static readonly DependencyProperty ErrorsProperty =
			DependencyProperty.Register("Errors", typeof(IList<ValidationError>), typeof(ValidationErrorsHostBehavior), new PropertyMetadata(null,
				(d, e) => ((ValidationErrorsHostBehavior)d).OnErrorsChanged(e),
				(d, v) => v ?? new ObservableCollection<ValidationError>()));
		public static readonly DependencyProperty HasErrorsProperty =
			DependencyProperty.Register("HasErrors", typeof(bool), typeof(ValidationErrorsHostBehavior), new PropertyMetadata(false));
		#endregion
		public ValidationErrorsHostBehavior() {
			Errors = new ObservableCollection<ValidationError>();
		}
		public IList<ValidationError> Errors {
			get { return (IList<ValidationError>)GetValue(ErrorsProperty); }
			set { SetValue(ErrorsProperty, value); }
		}
		public bool HasErrors {
			get { return (bool)GetValue(HasErrorsProperty); }
			set { SetValue(HasErrorsProperty, value); }
		}
		protected override void OnAttached() {
			base.OnAttached();
			Validation.AddErrorHandler(AssociatedObject, OnAssociatedObjectError);
		}
		protected override void OnDetaching() {
			base.OnDetaching();
			Validation.RemoveErrorHandler(AssociatedObject, OnAssociatedObjectError);
		}
		void OnAssociatedObjectError(object sender, ValidationErrorEventArgs e) {
			if(e.Action == ValidationErrorEventAction.Added)
				Errors.Add(e.Error);
			else
				Errors.Remove(e.Error);
			e.Handled = true;
			UpdateHasErrors();
		}
		protected virtual void OnErrorsChanged(DependencyPropertyChangedEventArgs e) {
			INotifyCollectionChanged oldValue = e.OldValue as INotifyCollectionChanged;
			INotifyCollectionChanged newValue = e.NewValue as INotifyCollectionChanged;
			if(oldValue != null)
				oldValue.CollectionChanged -= OnErrorsCollectionChanged;
			if(newValue != null)
				newValue.CollectionChanged += OnErrorsCollectionChanged;
			UpdateHasErrors();
		}
		protected void UpdateHasErrors() {
			HasErrors = Errors.Count != 0;
		}
		void OnErrorsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			UpdateHasErrors();
		}
	}
}
