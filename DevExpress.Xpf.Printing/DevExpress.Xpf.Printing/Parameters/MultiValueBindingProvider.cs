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
using System.Windows;
using System.Windows.Data;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Editors;
using DevExpress.Mvvm.Native;
using DevExpress.XtraReports.Parameters;
using System.Linq;
namespace DevExpress.Xpf.Printing.Parameters {
	public class MutliValueBindingProvider : Behavior<ComboBoxEdit>, IValueConverter {
		public static readonly DependencyProperty SourceTypeProperty =
			DependencyProperty.Register("SourceType", typeof(Type), typeof(MutliValueBindingProvider), new PropertyMetadata(null));
		public Type SourceType {
			get { return (Type)GetValue(SourceTypeProperty); }
			set { SetValue(SourceTypeProperty, value); }
		}
		protected override void OnAttached() {
			base.OnAttached();
			Binding binding = new Binding("Value") {
				Mode = BindingMode.TwoWay,
				Converter = this,
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
				ValidatesOnDataErrors = true
			};
			BindingOperations.SetBinding(AssociatedObject, ComboBoxEdit.EditValueProperty, binding);
		}
		protected override void OnDetaching() {
			base.OnDetaching();
			BindingOperations.ClearBinding(AssociatedObject, ComboBoxEdit.EditValueProperty);
		}
		#region IValueConverter
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var enumerable = (value as List<object>).Do(x=> x.Distinct());
			var result = new List<object>();
			enumerable.Do(e => e.ForEach(x => {
				if (x.GetType() == SourceType)
					result.Add(x);
				else {
					var convertedValue = ParameterHelper.ConvertFrom(x, SourceType, null);
					if (convertedValue != null)
						result.Add(convertedValue);
				}
			}));
			return result;
		}
		#endregion
	}
	public class ShowPopupBehavior : Behavior<ComboBoxEdit> {
		protected override void OnAttached() {
			base.OnAttached();
			AssociatedObject.IsKeyboardFocusWithinChanged += OnKeyboardFocusWithinChanged;
		}
		protected override void OnDetaching() {
			AssociatedObject.IsKeyboardFocusWithinChanged -= OnKeyboardFocusWithinChanged;
			base.OnDetaching();
		}
		void OnKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e) {
			if ((bool)e.NewValue) {
				Dispatcher.BeginInvoke(new Action(() => {
					AssociatedObject.IsPopupOpen = true;
				}));
			}
		}
	}
}
