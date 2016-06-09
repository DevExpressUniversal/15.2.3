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

using System.Windows.Input;
using System;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Windows;
using DevExpress.Xpf.Editors;
using System.Windows.Data;
using System.Windows.Controls;
namespace DevExpress.Xpf.SpellChecker.Native {
	#region UICommand<TControl>
	public abstract class UICommand<TControl> : ICommand {
		readonly TControl control;
		protected UICommand(TControl control) {
			this.control = control;
		}
		protected TControl Control { get { return control; } }
		EventHandler onCanExecureChanged;
		public event EventHandler CanExecuteChanged {
			add {
				onCanExecureChanged += value;
#if !SL
				CommandManager.RequerySuggested += value;
#endif
			}
			remove {
				onCanExecureChanged -= value;
#if !SL
				CommandManager.RequerySuggested -= value;
#endif
			}
		}
		protected internal void RaiseCanExecuteChanged() {
			if (onCanExecureChanged != null)
				onCanExecureChanged(this, EventArgs.Empty);
		}
		public bool CanExecute(object parameter) {
			if (this.control == null)
				return false;
			return CanExecuteCore(parameter);
		}
		public void Execute(object parameter) {
			ExecuteCore(parameter);
		}
		protected abstract bool CanExecuteCore(object parameter);
		protected abstract void ExecuteCore(object parameter);
	}
	#endregion
	#region ImageSourceExtension
	public class ImageSourceExtension : MarkupExtension {
		public ImageSourceExtension() { }
		public string Name { get; set; }
		public override object ProvideValue(IServiceProvider serviceProvider) {
			BitmapImage result = new BitmapImage();
#if !SL
			result.BeginInit();
			result.StreamSource = this.GetType().Assembly.GetManifestResourceStream("DevExpress.Xpf.SpellChecker.images." + Name);
			result.EndInit();
#else
			result.SetSource(this.GetType().Assembly.GetManifestResourceStream("DevExpress.Xpf.SpellChecker.images." + Name));
#endif
			return result;
		}
	}
	#endregion
	#region BindingHelperExtension
	public class BindingHelperExtension : MarkupExtension {
		public BindingHelperExtension() { }
		public string Path { get; set; }
		DependencyProperty BindingProperty { get; set; }
		FrameworkElement Element { get; set; }
		public override object ProvideValue(IServiceProvider serviceProvider) {
			IProvideValueTarget service = serviceProvider as IProvideValueTarget;
			BindingProperty = service.TargetProperty as DependencyProperty;
			Element = service.TargetObject as FrameworkElement;
			if (Element == null)
				return this;
			Element.Loaded += OnLoaded;
			return this;
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			if (!String.IsNullOrEmpty(Path)) {
				Binding bindingItem = new Binding(Path);
				bindingItem.Mode = BindingMode.TwoWay;
				bindingItem.RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent);
				Element.SetBinding(ListBoxEdit.SelectedItemProperty, bindingItem);  
			}
			Element.Loaded -= OnLoaded;
		}
	} 
	#endregion
	#region DoubleClickHelper
	public class DoubleClickHelper : DependencyObject {
		public static readonly DependencyProperty DoubleClickCommandProperty = CreateDoubleClickCommandProperty();
		static DependencyProperty CreateDoubleClickCommandProperty() {
			return DependencyProperty.RegisterAttached("DoubleClickCommand", typeof(ICommand), typeof(DoubleClickHelper), new PropertyMetadata(null, OnDoubleClickCommandChanged));
		}
		static void OnDoubleClickCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Control control = d as Control;
#if!SL
			control.MouseDoubleClick += OnMouseDoubleClick;
#else
			control.AddHandler(Control.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseDoubleClick), true);
#endif
		}
		static void OnMouseDoubleClick(object sender, MouseButtonEventArgs e) {
			ICommand command = GetDoubleClickCommand(sender as DependencyObject);
#if SL
			if (e.ClickCount != 2)
				return;
#endif
			if (command != null)
				command.Execute(null);
		}
		public static ICommand GetDoubleClickCommand(DependencyObject obj) {
			return (ICommand)obj.GetValue(DoubleClickCommandProperty);
		}
		public static void SetDoubleClickCommand(DependencyObject obj, ICommand command) {
			obj.SetValue(DoubleClickCommandProperty, command);
		}
	} 
	#endregion
}
