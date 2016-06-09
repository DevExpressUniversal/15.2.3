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
using System.Collections;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.DemoBase.Helpers.Internal;
using DevExpress.Xpf.DemoBase.Helpers.TextColorizer;
using DevExpress.Xpf.DemoBase.Helpers.TextColorizer.Internal;
using DevExpress.Xpf.Utils;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.DemoBase.Helpers {
	[TemplatePart(Name = "ItemsPresenter", Type = typeof(CodeViewItemsPresenter))]
	[TemplatePart(Name = "CodePresenter", Type = typeof(CodeViewPresenter))]
	[TemplatePart(Name = "TextInput", Type = typeof(TextBox))]
	class CodeViewControl : Control {
		public static readonly DependencyProperty ItemTemplateProperty =
			DependencyPropertyManager.Register("ItemTemplate", typeof(DataTemplate), typeof(CodeViewControl), new PropertyMetadata(null));
		public static readonly DependencyProperty ItemTemplateSelectorProperty =
			DependencyPropertyManager.Register("ItemTemplateSelector", typeof(DataTemplateSelector), typeof(CodeViewControl), new PropertyMetadata(null));
		public static readonly DependencyProperty ItemsSourceProperty =
			DependencyPropertyManager.Register("ItemsSource", typeof(IList), typeof(CodeViewControl), new PropertyMetadata(null));
		public static readonly DependencyProperty SelectedItemProperty =
			DependencyPropertyManager.Register("SelectedItem", typeof(object), typeof(CodeViewControl), new PropertyMetadata(null));
		public static readonly DependencyProperty SearchCommandProperty =
			DependencyPropertyManager.Register("SearchCommand", typeof(ICommand), typeof(CodeViewControl), new PropertyMetadata(null));
		public static readonly DependencyProperty TextProperty =
			DependencyPropertyManager.Register("Text", typeof(string), typeof(CodeViewControl), new PropertyMetadata(string.Empty,
				(d, e) => ((CodeViewControl)d).OnTextChanged(e)));
		public static readonly DependencyProperty MessageProperty =
			DependencyPropertyManager.Register("Message", typeof(string), typeof(CodeViewControl), new PropertyMetadata(" "));
		public static readonly DependencyProperty NotFoundMessageProperty =
			DependencyPropertyManager.Register("NotFoundMessage", typeof(string), typeof(CodeViewControl), new PropertyMetadata(" "));
		public static readonly DependencyProperty NoMoreFoundMessageProperty =
			DependencyPropertyManager.Register("NoMoreFoundMessage", typeof(string), typeof(CodeViewControl), new PropertyMetadata(" "));
		public CodeViewControl() {
			SearchCommand = DelegateCommandFactory.Create(() => DoSearch(false), () => true, false);
			DefaultStyleKey = typeof(CodeViewControl);
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
			SizeChanged += OnSizeChanged;
		}
		public DataTemplate ItemTemplate { get { return (DataTemplate)GetValue(ItemTemplateProperty); } set { SetValue(ItemTemplateProperty, value); } }
		public DataTemplateSelector ItemTemplateSelector { get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); } set { SetValue(ItemTemplateSelectorProperty, value); } }
		public IList ItemsSource { get { return (IList)GetValue(ItemsSourceProperty); } set { SetValue(ItemsSourceProperty, value); } }
		public object SelectedItem { get { return GetValue(SelectedItemProperty); } set { SetValue(SelectedItemProperty, value); } }
		public ICommand SearchCommand { get { return (ICommand)GetValue(SearchCommandProperty); } private set { SetValue(SearchCommandProperty, value); } }
		public string Text { get { return (string)GetValue(TextProperty); } private set { SetValue(TextProperty, value); } }
		public string Message { get { return (string)GetValue(MessageProperty); } private set { SetValue(MessageProperty, value); } }
		public string NotFoundMessage { get { return (string)GetValue(NotFoundMessageProperty); } set { SetValue(NotFoundMessageProperty, value); } }
		public string NoMoreFoundMessage { get { return (string)GetValue(NoMoreFoundMessageProperty); } set { SetValue(NoMoreFoundMessageProperty, value); } }
		protected CodeViewItemsPresenter ItemsPresenter { get; private set; }
		protected CodeViewPresenter CodePresenter { get; private set; }
		protected TextBox TextBox { get; private set; }
		protected virtual void OnUnloaded(object sender, RoutedEventArgs e) {
			if(Application.Current != null && Application.Current.MainWindow != null) {
				Application.Current.MainWindow.Focus();
				FocusManager.SetFocusedElement(Application.Current.MainWindow, Application.Current.MainWindow);
			}
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			Focus();
		}
		public void FocusCodePresenter() {
			if(CodePresenter != null)
				CodePresenter.Focus();
		}
		void OnSizeChanged(object sender, SizeChangedEventArgs e) {
			SizeChanged -= OnSizeChanged;
			FocusCodePresenter();
		}
		void OnItemsPresenterSelectedItemContainerContentChanged(object sender, DependencyPropertyChangedEventArgs e) {
			CodeViewItem selectedItemContainerContent = ItemsPresenter.SelectedItemContainerContent as CodeViewItem;
			if(selectedItemContainerContent == null && SelectedItem != null) return;
			if(selectedItemContainerContent == null) {
				CodePresenter.CodeText = new CodeLanguageText(CodeLanguage.Plain, string.Empty);
			} else {
				CodePresenter.SetBinding(CodeViewPresenter.CodeTextProperty, new Binding("ActualCodeText") { Source = selectedItemContainerContent, Mode = BindingMode.OneWay });
			}
		}
		void OnTextInputTextChanged(object sender, TextChangedEventArgs e) {
			Text = TextBox.Text;
		}
		void OnTextInputKeyDown(object sender, KeyEventArgs e) {
			if(e.Key == Key.Enter)
				DoSearch(false);
		}
		protected virtual void OnTextChanged(DependencyPropertyChangedEventArgs e) {
			string newValue = (string)e.NewValue;
			CodePresenter.CurrentPosition = null;
			if(!string.IsNullOrEmpty(newValue))
				DoSearch(true);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if(TextBox != null) {
				TextBox.TextChanged -= OnTextInputTextChanged;
				TextBox.KeyDown -= OnTextInputKeyDown;
			}
			if(ItemsPresenter != null)
				ItemsPresenter.SelectedItemContainerContentChanged -= OnItemsPresenterSelectedItemContainerContentChanged;
			ItemsPresenter = GetTemplateChild("ItemsPresenter") as CodeViewItemsPresenter;
			CodePresenter = (CodeViewPresenter)GetTemplateChild("CodePresenter");
			TextBox = (TextBox)GetTemplateChild("TextInput");
			if(TextBox != null) {
				TextBox.TextChanged += OnTextInputTextChanged;
				TextBox.KeyDown += OnTextInputKeyDown;
			}
			if(ItemsPresenter != null) {
				ItemsPresenter.SelectedItemContainerContentChanged += OnItemsPresenterSelectedItemContainerContentChanged;
				OnItemsPresenterSelectedItemContainerContentChanged(ItemsPresenter, new DependencyPropertyChangedEventArgs());
			}
		}
		void Search(bool searchTextChanged) {
			if(string.IsNullOrEmpty(Text)) return;
			bool previousMessageIsNotFound = Message == NotFoundMessage;
			Message = " ";
			TextPointer currentPosition = CodePresenter.CurrentPosition ?? CodePresenter.TextPresenter.ContentStart;
			SLTextRange r = RichTextHelper.FindText(CodePresenter.TextPresenter, Text, new SLTextRange(currentPosition, null), StringComparison.OrdinalIgnoreCase);
			if(r == null)
				Message = searchTextChanged || previousMessageIsNotFound ? NotFoundMessage : NoMoreFoundMessage;
			if(r == null)
				CodePresenter.HighlightedRanges.Clear();
			else if(CodePresenter.HighlightedRanges.Count == 0)
				CodePresenter.HighlightedRanges.Add(r);
			else
				CodePresenter.HighlightedRanges[0] = r;
			CodePresenter.CurrentPosition = r == null ? null : r.End;
			CodePresenter.ScrollTo(r);
			if(TextBox != null)
				TextBox.Focus();
		}
		void DoSearch(bool searchTextChanged) {
			if(DesignerProperties.GetIsInDesignMode(this)) return;
			DispatcherTimer timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(1.0) };
			timer.Tick += (s, e) => {
				timer.Stop();
				Search(searchTextChanged);
			};
			timer.Start();
		}
		internal string GetDisplayedText() {
			return CodePresenter.GetDisplayedText();
		}
		internal bool IsTemplateApplied {
			get { return CodePresenter != null; }
		}
	}
	class CodeViewControlPanel : Panel {
		public static readonly DependencyProperty CellWidthProperty =
			DependencyPropertyManager.RegisterAttached("CellWidth", typeof(GridLength), typeof(CodeViewControlPanel), new PropertyMetadata(GridLength.Auto));
		public static readonly DependencyProperty CellMaxWidthProperty =
			DependencyPropertyManager.RegisterAttached("CellMaxWidth", typeof(double), typeof(CodeViewControlPanel), new PropertyMetadata(double.PositiveInfinity));
		public static readonly DependencyProperty CellAlignmentProperty =
			DependencyPropertyManager.RegisterAttached("CellAlignment", typeof(HorizontalAlignment), typeof(CodeViewControlPanel), new PropertyMetadata(HorizontalAlignment.Stretch));
		public static readonly DependencyProperty CellActualWidthProperty =
			DependencyPropertyManager.RegisterAttached("CellActualWidth", typeof(double), typeof(CodeViewControlPanel), new PropertyMetadata(0.0));
		public static GridLength GetCellWidth(UIElement d) { return (GridLength)d.GetValue(CellWidthProperty); }
		public static void SetCellWidth(UIElement d, GridLength v) { d.SetValue(CellWidthProperty, v); }
		public static double GetCellMaxWidth(UIElement d) { return (double)d.GetValue(CellMaxWidthProperty); }
		public static void SetCellMaxWidth(UIElement d, double v) { d.SetValue(CellMaxWidthProperty, v); }
		public static HorizontalAlignment GetCellAlignment(UIElement d) { return (HorizontalAlignment)d.GetValue(CellAlignmentProperty); }
		public static void SetCellAlignment(UIElement d, HorizontalAlignment v) { d.SetValue(CellAlignmentProperty, v); }
		public static double GetCellActualWidth(UIElement d) { return (double)d.GetValue(CellActualWidthProperty); }
		public static void SetCellActualWidth(UIElement d, double v) { d.SetValue(CellActualWidthProperty, v); }
		protected override Size MeasureOverride(Size availableSize) {
			double starsSum = 0.0;
			double autoWith = 0.0;
			double maxHeight = 0.0;
			foreach(UIElement child in Children) {
				GridLength childCellWidth = GetCellWidth(child);
				if(childCellWidth.IsStar) {
					starsSum += childCellWidth.Value;
					continue;
				}
				double cellActualWidth;
				if(childCellWidth.IsAbsolute) {
					child.Measure(new Size(Math.Min(childCellWidth.Value, GetCellMaxWidth(child)), availableSize.Height));
					cellActualWidth = childCellWidth.Value;
				} else {
					child.Measure(new Size(GetCellMaxWidth(child), availableSize.Height));
					cellActualWidth = child.DesiredSize.Width;
				}
				SetCellActualWidth(child, cellActualWidth);
				autoWith += cellActualWidth;
				if(child.DesiredSize.Height > maxHeight)
					maxHeight = child.DesiredSize.Height;
			}
			double starWidth = (availableSize.Width - autoWith) / starsSum;
			if(starWidth < 0.0)
				starWidth = 0.0;
			foreach(UIElement child in Children) {
				GridLength childCellWidth = GetCellWidth(child);
				if(!childCellWidth.IsStar) continue;
				double cellActualWidth = childCellWidth.Value * starWidth;
				child.Measure(new Size(Math.Min(cellActualWidth, GetCellMaxWidth(child)), availableSize.Height));
				SetCellActualWidth(child, cellActualWidth);
				if(child.DesiredSize.Height > maxHeight)
					maxHeight = child.DesiredSize.Height;
			}
			return new Size(double.IsInfinity(availableSize.Width) ? 0.0 : availableSize.Width, maxHeight);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			double width = 0.0;
			foreach(UIElement child in Children) {
				double cellWidth = GetCellActualWidth(child);
				double childWidth = Math.Min(cellWidth, GetCellMaxWidth(child));
				double delta = cellWidth - childWidth;
				double x;
				switch(GetCellAlignment(child)) {
					case HorizontalAlignment.Center: x = delta / 2.0; break;
					case HorizontalAlignment.Right: x = delta; break;
					default: x = 0.0; break;
				}
				child.Arrange(new Rect(new Point(width + x, 0.0), new Size(childWidth, finalSize.Height)));
				width += cellWidth;
			}
			return new Size(width, finalSize.Height);
		}
	}
}
