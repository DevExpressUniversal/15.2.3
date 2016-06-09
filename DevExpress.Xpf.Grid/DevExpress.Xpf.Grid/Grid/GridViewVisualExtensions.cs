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
using DevExpress.Xpf.Grid.Native;
using System.Windows.Markup;
using System.Windows.Data;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Grid {
	public abstract class DataViewVisualInitializerBase : INotifyCurrentViewChanged {
		void INotifyCurrentViewChanged.OnCurrentViewChanged(DependencyObject d) {
			DataViewBase view = DataControlBase.GetCurrentView(d);
			if(view == null)
				return;
			Execute(view, (FrameworkElement)d);
		}
		protected abstract void Execute(DataViewBase view, FrameworkElement element);
	}
	public class HeadersPanelInitializer : DataViewVisualInitializerBase {
		protected override void Execute(DataViewBase view, FrameworkElement element) { view.HeadersPanel = element; }
	}
	public class BandsContainerInitializer : DataViewVisualInitializerBase {
		protected override void Execute(DataViewBase view, FrameworkElement element) { view.RootBandsContainer = element; }
	}
	public class NewItemRowInitializer : DataViewVisualInitializerBase {
		protected override void Execute(DataViewBase view, FrameworkElement element) {
			TableView tableView = view as TableView;
			NewItemRowControl newItemRowControl = element as NewItemRowControl;
			if(tableView == null || newItemRowControl == null)
				return;
			newItemRowControl.SetBinding(FrameworkElement.VisibilityProperty, new Binding(TableView.NewItemRowPositionProperty.GetName()) { Source = view, Converter = new NewItemRowPositionToVisibilityConverter() });
		}
	}
	public class VerticalScrollBarBlendCustomizationExtension : DependencyObject, INotifyCurrentViewChanged {
		public static readonly DependencyProperty ElementVisibilityProperty = DependencyProperty.Register("ElementVisibility", typeof(Visibility), typeof(VerticalScrollBarBlendCustomizationExtension), new PropertyMetadata(Visibility.Visible, (d, e) => (d as VerticalScrollBarBlendCustomizationExtension).UpdateVerticalScrollbarWidth()));
		public Visibility ElementVisibility {
			get { return (Visibility)GetValue(ElementVisibilityProperty); }
			set { SetValue(ElementVisibilityProperty, value); }
		}
		FrameworkElement Element { get; set; }
		void INotifyCurrentViewChanged.OnCurrentViewChanged(DependencyObject d) {
			Element = d as FrameworkElement;
			Element.SizeChanged += delegate {
				UpdateVerticalScrollbarWidth();
			};
			BindingOperations.SetBinding(this, ElementVisibilityProperty, new Binding("Visibility") { Source = d });
			UpdateVerticalScrollbarWidth();
		}
		void UpdateVerticalScrollbarWidth() {
			DataViewBase view = GridControl.GetCurrentView(Element) as DataViewBase;
			if(view == null)
				return;
			view.ViewBehavior.SetVerticalScrollBarWidth(ColumnsLayoutParametersValidator.GetVerticalScrollBarWidth(Element.Visibility == Visibility.Visible ? Element.ActualWidth : 0));
		}
	}
	public class GroupPanelInitializer : DataViewVisualInitializerBase {
		protected override void Execute(DataViewBase view, FrameworkElement element) {
			GridViewBase gridView = view as GridViewBase;
			if(gridView == null)
				return;
			gridView.GroupPanel = element;
			GroupPanelControl groupPanelControl = element as GroupPanelControl;
			if(element != null) {
				element.SetBinding(GroupPanelControl.IsGroupedProperty, new Binding(GridViewBase.IsGroupPanelTextVisibleProperty.GetName()) { Source = gridView, Converter = new NegationConverterExtension() });
				element.SetBinding(GroupPanelControl.VisibilityProperty, new Binding(GridViewBase.IsGroupPanelVisibleProperty.GetName()) { Source = gridView, Converter = new BoolToVisibilityConverter() });
			}
		}
	}
	public class ScrollContentPresenterInitializer : DataViewVisualInitializerBase {
		protected override void Execute(DataViewBase view, FrameworkElement element) { ((DataViewBase)view).ScrollContentPresenter = element; }
	}
	public class BestFitControlDecoratorInitializer : DataViewVisualInitializerBase {
		protected override void Execute(DataViewBase view, FrameworkElement element) { ((ITableView)view).TableViewBehavior.BestFitControlDecorator = (Decorator)element; }
	}
#if SL
	public class PopupContainerDecoratorInitializer : DataViewVisualInitializerBase {
		protected override void Execute(DataViewBase view, FrameworkElement element) { ((IPopupContainer)view).PopupContainer = (Decorator)element; }
	}
#endif
	public class NewItemRowPositionToVisibilityConverter : MarkupExtension, IValueConverter {
		object IValueConverter.Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			NewItemRowPosition position = (NewItemRowPosition)value;
			return position == NewItemRowPosition.Top ? Visibility.Visible : Visibility.Collapsed;
		}
		object IValueConverter.ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new System.NotImplementedException();
		}
		public override object ProvideValue(System.IServiceProvider serviceProvider) {
			return this;
		}
	}
	public class ValidationErrorToIsEnabledConverter : IValueConverter {
		object IValueConverter.Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(value == null)
				return true;
			bool hasCellError = value is RowValidationError && ((RowValidationError)value).IsCellError;
			return !hasCellError;
		}
		object IValueConverter.ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new System.NotImplementedException();
		}
	}
	public class SelectionStateToVisibilityConverter : MarkupExtension, IValueConverter {
		public SelectionState Value { get; set; }
		public bool Invert { get; set; }
		public override object ProvideValue(System.IServiceProvider serviceProvider) {
			return this;
		}
		object IValueConverter.Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			SelectionState mode = (SelectionState)value;
			bool result = mode == Value;
			if(Invert) result = !result;
			return result ? Visibility.Visible : Visibility.Collapsed;
		}
		object IValueConverter.ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new System.NotImplementedException();
		}
	}
	public class GroupLevelToMarginConverter : IValueConverter {
		public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			Thickness thickness = (Thickness)parameter;
			int level = (int)value;
			return new Thickness(thickness.Left * level, thickness.Top, thickness.Right, thickness.Bottom);
		}
		public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new System.NotImplementedException();
		}
	}
	public class SelectionStateToBooleanConverter : MarkupExtension, IValueConverter {
		public override object ProvideValue(System.IServiceProvider serviceProvider) {
			return this;
		}
		public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return (SelectionState)value == SelectionState.None ? false : true;
		}
		public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new System.NotImplementedException();
		}
	}
}
