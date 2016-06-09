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
using System.Windows.Data;
namespace DevExpress.Charts.Designer.Native {
	public class BarCheckButtonViewModel : BarButtonViewModel {
		public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool), typeof(BarCheckButtonViewModel), new PropertyMetadata(IsCheckedPropertyChanged));
		static void IsCheckedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			BarCheckButtonViewModel model = d as BarCheckButtonViewModel;
			if (model != null) {
				model.isChecked = (bool)e.NewValue;
				model.OnPropertyChanged("IsChecked");
				model.OnPropertyChanged("Checked");
			}
		}
		bool isChecked;
		public bool Checked {
			get { return isChecked; }
			set {
				if (isChecked != value) {
					isChecked = value;
					OnPropertyChanged("Checked");
					if (Command != null)
						Command.Execute(isChecked);
				}
			}
		}
		public bool IsChecked {
			get { return (bool)GetValue(IsCheckedProperty); }
			set { SetValue(IsCheckedProperty, value); }
		}
		public BarCheckButtonViewModel(ChartCommandBase command, ChartModelElement source, string editValuePath, IValueConverter converter, IRibbonBehavior ribbonBehavior)
			: base(command, ribbonBehavior) {
			Binding binding = new Binding(editValuePath) { Source = source, Mode = BindingMode.OneWay, Converter = converter, ConverterParameter = command };
			BindingOperations.SetBinding(this, IsCheckedProperty, binding);
		}
		public BarCheckButtonViewModel(ChartCommandBase command, ChartModelElement source, string editValuePath, IValueConverter converter)
			: this(command, source, editValuePath, converter, null) {
		}
	}
}
