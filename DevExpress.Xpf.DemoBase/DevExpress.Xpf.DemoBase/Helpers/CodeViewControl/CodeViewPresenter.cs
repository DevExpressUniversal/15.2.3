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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using DevExpress.Xpf.DemoBase.Helpers.TextColorizer;
using DevExpress.Xpf.DemoBase.Helpers.TextColorizer.Internal;
using System.Windows;
using DevExpress.Xpf.Utils;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Windows.Media;
namespace DevExpress.Xpf.DemoBase.Helpers.Internal {
	[TemplatePart(Name = "TextPresenter", Type = typeof(RichTextPresenter))]
	[TemplatePart(Name = "HighlightedRangesPreseneter", Type = typeof(Canvas))]
	[TemplatePart(Name = "ScrollViewer", Type = typeof(ScrollViewer))]
	public class CodeViewPresenter : Control {
		public static readonly DependencyProperty HighlightedRangesProperty =
			DependencyPropertyManager.Register("HighlightedRanges", typeof(ObservableCollection<SLTextRange>), typeof(CodeViewPresenter), new PropertyMetadata(null,
				(d, e) => ((CodeViewPresenter)d).OnHighlightedRangesChanged(e)));
		public static readonly DependencyProperty HighlightedRangeRectangleProperty =
			DependencyPropertyManager.Register("HighlightedRangeRectangle", typeof(DataTemplate), typeof(CodeViewPresenter), new PropertyMetadata(null));
		public static readonly DependencyProperty CodeTextProperty =
			DependencyPropertyManager.Register("CodeText", typeof(CodeLanguageText), typeof(CodeViewPresenter), new PropertyMetadata(null,
				(d, e) => ((CodeViewPresenter)d).OnCodeTextChanged(e)));
		public static readonly DependencyProperty CurrentPositionProperty =
			DependencyPropertyManager.Register("CurrentPosition", typeof(TextPointer), typeof(CodeViewPresenter), new PropertyMetadata(null));
		Dictionary<SLTextRange, Rectangle> highlights = new Dictionary<SLTextRange, Rectangle>();
		public CodeViewPresenter() {
			HighlightedRanges = new ObservableCollection<SLTextRange>();
			this.AddHandler(RichTextPresenter.KeyDownEvent, (KeyEventHandler)OnKeyDown, true);
			DefaultStyleKey = typeof(CodeViewPresenter);
			FlowDirection = System.Windows.FlowDirection.LeftToRight;
		}
		internal ObservableCollection<SLTextRange> HighlightedRanges { get { return (ObservableCollection<SLTextRange>)GetValue(HighlightedRangesProperty); } set { SetValue(HighlightedRangesProperty, value); } }
		public DataTemplate HighlightedRangeRectangle { get { return (DataTemplate)GetValue(HighlightedRangeRectangleProperty); } set { SetValue(HighlightedRangeRectangleProperty, value); } }
		public CodeLanguageText CodeText { get { return (CodeLanguageText)GetValue(CodeTextProperty); } set { SetValue(CodeTextProperty, value); } }
		public TextPointer CurrentPosition { get { return (TextPointer)GetValue(CurrentPositionProperty); } set { SetValue(CurrentPositionProperty, value); } }
		internal IRichTextPresenter TextPresenter { get; private set; }
		protected Canvas HighlightedRangesPreseneter { get; private set; }
		protected ScrollViewer ScrollViewer { get; private set; }
		internal void ScrollTo(SLTextRange range) {
			if(range == null || ScrollViewer == null) return;
			Rect rect = GetRangeRect(range);
			double horizontalOffsetMax = rect.Left;
			double horizontalOffsetMin = rect.Right - ScrollViewer.ViewportWidth;
			if(horizontalOffsetMin > horizontalOffsetMax)
				horizontalOffsetMin = rect.Left - ScrollViewer.ViewportWidth;
			double verticalOffsetMax = rect.Top;
			double verticalOffsetMin = rect.Bottom - ScrollViewer.ViewportHeight;
			if(verticalOffsetMin > verticalOffsetMax)
				verticalOffsetMin = rect.Top - ScrollViewer.ViewportHeight;
			if(ScrollViewer.HorizontalOffset < horizontalOffsetMin || ScrollViewer.HorizontalOffset > horizontalOffsetMax)
				ScrollViewer.ScrollToHorizontalOffset(horizontalOffsetMin);
			if(ScrollViewer.VerticalOffset < verticalOffsetMin || ScrollViewer.VerticalOffset > verticalOffsetMax)
				ScrollViewer.ScrollToVerticalOffset((verticalOffsetMin + verticalOffsetMax) / 2.0);
		}
		protected virtual void OnHighlightedRangesChanged(DependencyPropertyChangedEventArgs e) {
			ObservableCollection<SLTextRange> oldValue = (ObservableCollection<SLTextRange>)e.OldValue;
			ObservableCollection<SLTextRange> newValue = (ObservableCollection<SLTextRange>)e.NewValue;
			if(oldValue != null)
				oldValue.CollectionChanged -= OnHighlightedRangesCollectionChanged;
			if(newValue != null)
				newValue.CollectionChanged += OnHighlightedRangesCollectionChanged;
			RedrawHighlightedRanges();
		}
		void OnHighlightedRangesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			switch(e.Action) {
				case NotifyCollectionChangedAction.Reset:
					RedrawHighlightedRanges();
					break;
				case NotifyCollectionChangedAction.Add:
					AddHighlightedRanges(e.NewItems);
					break;
				case NotifyCollectionChangedAction.Remove:
					RemoveHighlightedRanges(e.OldItems);
					break;
				case NotifyCollectionChangedAction.Replace:
					RemoveHighlightedRanges(e.OldItems);
					AddHighlightedRanges(e.NewItems);
					break;
			}
		}
		protected virtual void RedrawHighlightedRanges() {
			if(HighlightedRangesPreseneter == null) return;
			highlights.Clear();
			HighlightedRangesPreseneter.Children.Clear();
			AddHighlightedRanges(HighlightedRanges);
		}
		protected virtual void AddHighlightedRanges(IList list) {
			if(list == null || HighlightedRangesPreseneter == null) return;
			foreach(SLTextRange range in list) {
				Rectangle highlight = CreateHighlightRectangle(GetRangeRect(range));
				highlights.Add(range, highlight);
				HighlightedRangesPreseneter.Children.Add(highlight);
			}
		}
		protected virtual void RemoveHighlightedRanges(IList list) {
			if(list == null || HighlightedRangesPreseneter == null) return;
			foreach(SLTextRange range in list) {
				Rectangle highlight = highlights[range];
				highlights.Remove(range);
				HighlightedRangesPreseneter.Children.Remove(highlight);
			}
		}
		protected virtual Rectangle CreateHighlightRectangle(Rect bounds) {
			Rectangle rectangle = HighlightedRangeRectangle == null ? null : HighlightedRangeRectangle.LoadContent() as Rectangle;
			if(rectangle == null)
				rectangle = new Rectangle();
			rectangle.Width = bounds.Width - rectangle.Margin.Left - rectangle.Margin.Right;
			rectangle.Height = bounds.Height - rectangle.Margin.Top - rectangle.Margin.Bottom;
			Canvas.SetLeft(rectangle, bounds.Left);
			Canvas.SetTop(rectangle, bounds.Top);
			return rectangle;
		}
		internal virtual Rect GetRangeRect(SLTextRange range) {
			Rect s = range.Start.GetCharacterRect(LogicalDirection.Forward);
			s.Union(range.End.GetCharacterRect(LogicalDirection.Backward));
			return s;
		}
		protected virtual void OnCodeTextChanged(DependencyPropertyChangedEventArgs e) {
			UpdateTextPresenter();
		}
		protected virtual void UpdateTextPresenter() {
			if(CodeText == null || TextPresenter == null) return;
			CurrentPosition = null;
			HighlightedRanges.Clear();
			double width = RichTextHelper.SetText(TextPresenter, CodeText.Text == null ? null : CodeText.Text(), CodeText.Language);
			TextPresenter.TextWidthMaxSet(width);
		}
		void OnKeyDown(object sender, KeyEventArgs e) {
			const double scrollDiff = 14.0; 
			switch(e.Key) {
				case Key.Up:
					ScrollViewer.ScrollToVerticalOffset(ScrollViewer.VerticalOffset - scrollDiff);
					break;
				case Key.Down:
					ScrollViewer.ScrollToVerticalOffset(ScrollViewer.VerticalOffset + scrollDiff);
					break;
				case Key.Left:
					ScrollViewer.ScrollToHorizontalOffset(ScrollViewer.HorizontalOffset - scrollDiff);
					break;
				case Key.Right:
					ScrollViewer.ScrollToHorizontalOffset(ScrollViewer.HorizontalOffset + scrollDiff);
					break;
				case Key.Home:
					ScrollViewer.ScrollToVerticalOffset(0.0);
					break;
				case Key.End:
					ScrollViewer.ScrollToVerticalOffset(ScrollViewer.ExtentHeight);
					break;
				case Key.PageUp:
					ScrollViewer.ScrollToVerticalOffset(ScrollViewer.VerticalOffset - ScrollViewer.ActualHeight);
					break;
				case Key.PageDown:
					ScrollViewer.ScrollToVerticalOffset(ScrollViewer.VerticalOffset + ScrollViewer.ActualHeight);
					break;
			}
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			TextPresenter = (RichTextPresenter)GetTemplateChild("TextPresenter");
			HighlightedRangesPreseneter = (Canvas)GetTemplateChild("HighlightedRangesPreseneter");
			ScrollViewer = (ScrollViewer)GetTemplateChild("ScrollViewer");
			UpdateTextPresenter();
		}
		protected override Size ArrangeOverride(Size size) {
			size = base.ArrangeOverride(size);
			Clip = new RectangleGeometry() { Rect = new Rect(new Point(), size) };
			return size;
		}
		internal string GetDisplayedText() {
			return RichTextHelper.GetText(TextPresenter);
		}
	}
}
