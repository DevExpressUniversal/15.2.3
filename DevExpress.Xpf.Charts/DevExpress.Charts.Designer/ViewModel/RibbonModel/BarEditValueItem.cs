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
	public class BarEditValueItemViewModel : BarButtonViewModel {
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(BarEditValueItemViewModel),
			new PropertyMetadata(ValuePropertyChanged));
		static void ValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			BarEditValueItemViewModel model = d as BarEditValueItemViewModel;
			if (model != null) {
				model.editValue = e.NewValue;
				model.OnPropertyChanged("Value");
				model.OnPropertyChanged("EditValue");
			}
		}
		IValueConverter converter;
		object converterParameter;
		object editValue = null;
		string nullText = string.Empty;
		int editWidth = 90;
		public object Value {
			get { return GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}
		public object EditValue {
			get { return editValue; }
			set {
				if (!object.Equals(editValue, value)) {
					editValue = value;
					OnPropertyChanged("EditValue");
					object convertedValue = null;
					if (converter != null)
						convertedValue = converter.ConvertBack(editValue, null, converterParameter, null);
					object parameter = (convertedValue == null) ? editValue : convertedValue;
					if (Command != null && Command.CanExecute(parameter))
						Command.Execute(parameter);
				}
			}
		}
		public int EditWidth {
			get { return editWidth; }
			set {
				if (editWidth != value) {
					editWidth = value;
					OnPropertyChanged("EditWidth");
				}
			}
		}
		public string NullText {
			get { return nullText; }
			set {
				if (nullText != value) {
					nullText = value;
					OnPropertyChanged("NullText");
				}
			}
		}
		public BarEditValueItemViewModel(ChartCommandBase command, ChartModelElement source, string editValuePath)
			: this(command, source, editValuePath, command as IValueConverter, null, null) { }
		public BarEditValueItemViewModel(ChartCommandBase command, ChartModelElement source, string editValuePath, object bindingFailureValue)
			: this(command, source, editValuePath, command as IValueConverter, bindingFailureValue, null) { }
		public BarEditValueItemViewModel(ChartCommandBase command, ChartModelElement source, string editValuePath, IValueConverter converter)
			: this(command, source, editValuePath, converter, null, null) { }
		public BarEditValueItemViewModel(ChartCommandBase command, ChartModelElement source, string editValuePath, IValueConverter converter, IRibbonBehavior behavior)
			: this(command, source, editValuePath, converter, null, behavior) { }
		public BarEditValueItemViewModel(ChartCommandBase command, ChartModelElement source, string editValuePath, object bindingFailureValue, IRibbonBehavior behavior)
			: this(command, source, editValuePath, null, bindingFailureValue, behavior) { }
		public BarEditValueItemViewModel(ChartCommandBase command, ChartModelElement source, string editValuePath, IValueConverter converter, object bindingFailureValue, IRibbonBehavior behavior)
			: base(command, behavior) {
			this.converter = converter ?? command as IValueConverter;
			this.converterParameter = command;
			Binding binding = new Binding(editValuePath) {
				Source = source,
				Mode = BindingMode.OneWay,
				Converter = this.converter,
				ConverterParameter = this.converterParameter,
				TargetNullValue = bindingFailureValue,
				FallbackValue = bindingFailureValue
			};
			BindingOperations.SetBinding(this, ValueProperty, binding);
		}
	}
}
