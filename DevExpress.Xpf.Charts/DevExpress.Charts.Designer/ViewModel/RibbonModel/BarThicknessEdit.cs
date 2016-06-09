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

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
namespace DevExpress.Charts.Designer.Native {
	public class BarThicknessEditViewModel : BarButtonViewModel {
		public class Item {
			readonly string representation;
			readonly int thickness;
			readonly Visibility lineVisiblity;
			public string Representation {
				get { return representation; }
			}
			public int Thickness {
				get { return thickness; }
			}
			public Visibility LineVisibility {
				get { return lineVisiblity; }
			}
			public Item(string representation, int thickness) {
				this.representation = representation;
				this.thickness = thickness;
				this.lineVisiblity = (thickness != 0) ? Visibility.Visible : Visibility.Collapsed;
			}
		}
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(BarThicknessEditViewModel), new PropertyMetadata(ValuePropertyChanged));
		static readonly DependencyPropertyKey ItemsPropertyKey =
			DependencyProperty.RegisterReadOnly("Items", typeof(ObservableCollection<Item>), typeof(BarThicknessEditViewModel), new PropertyMetadata());
		static void ValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			BarThicknessEditViewModel model = d as BarThicknessEditViewModel;
			Item found = null;
			if (e.NewValue == null)
				return;
			int thickness = (int)e.NewValue;
			foreach (Item item in model.Items)
				if (item.Thickness == thickness)
					found = item;
			if (found == null && model.Items.Count > 0)
				found = model.Items[model.Items.Count - 1];
			model.editValue = found;
			model.OnPropertyChanged("Value");
			model.OnPropertyChanged("EditValue");
		}
		IValueConverter converter;
		object converterParameter;
		object editValue = null;
		int editWidth = 90;
		public ObservableCollection<Item> Items {
			get { return (ObservableCollection<Item>)GetValue(ItemsPropertyKey.DependencyProperty); }
			private set { SetValue(ItemsPropertyKey, value); }
		}
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
					if (Command != null)
						Command.Execute((convertedValue == null) ? editValue : convertedValue);
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
		public BarThicknessEditViewModel(ChartCommandBase command, ChartModelElement source, string editValuePath, int startThickness, int step, int count, IValueConverter converter)
			: base(command) {
			this.converter = converter ?? this as IValueConverter;
			this.converterParameter = command;
			ObservableCollection<Item> items = new ObservableCollection<Item>();
			int counter = startThickness;
			for (int i = 0; i < count; i++) {
				items.Add(new Item((counter == 0) ? ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ThicknessNone) : counter.ToString(), counter));
				counter += step;
			}
			Items = items;
			Binding binding = new Binding(editValuePath) {
				Source = source,
				Mode = BindingMode.OneWay,
				Converter = this.converter,
				ConverterParameter = this.converterParameter,
			};
			BindingOperations.SetBinding(this, ValueProperty, binding);
		}
	}
}
