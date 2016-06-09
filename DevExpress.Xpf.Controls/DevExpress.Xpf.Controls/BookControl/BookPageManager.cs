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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Utils;
namespace DevExpress.Xpf.Controls.Internal {
	public class BookPage {
		#region Property
		public double BaseForeShadowWidth { get { return BaseForeShadow.GetWidth(); } }
		public double OverlayForeShadowWidth { get { return OverlayForeShadow.GetWidth(); } }
		public double BackShadowWidth { get { return BackShadow.GetWidth(); } }
		public Rect PageRect { get { return new Rect(Grid.GetLeft(), Grid.GetTop(), Grid.GetWidth(), Grid.GetHeight()); } }
		protected internal Dictionary<BookTemplateElementType, BookTemplateElement> Elements { get; set; }
		protected internal BookTemplateElement Grid { get { return Elements[BookTemplateElementType.Grid]; } }
		protected internal BookTemplateElement Content { get { return Elements[BookTemplateElementType.Content]; } }
		protected internal BookTemplateElement BaseForeShadow { get { return Elements[BookTemplateElementType.BaseForeShadow]; } }
		protected internal BookTemplateElement OverlayForeShadow { get { return Elements[BookTemplateElementType.OverlayForeShadow]; } }
		protected internal BookTemplateElement BackShadow { get { return Elements[BookTemplateElementType.BackShadow]; } }
		#endregion
		public BookPage(Book book, BookPageLayout layout) {
			Elements = new Dictionary<BookTemplateElementType, BookTemplateElement>();
			Array elementsArray = EnumExtensions.GetValues(typeof(BookTemplateElementType));
			foreach(BookTemplateElementType element in elementsArray) {
				BookTemplateElement templateElement = book.GetTemplateElement(element, layout);
				Elements.Add(element, templateElement);
			}
		}
		public void SetShadowHeight(double value) {
			BaseForeShadow.SetHeight(value);
			OverlayForeShadow.SetHeight(value);
			BackShadow.SetHeight(value);
		}
		public void SetShadowTop(double value) {
			BaseForeShadow.SetTop(value);
			OverlayForeShadow.SetTop(value);
			BackShadow.SetTop(value);
		}
		public void SetContent(object value) { Content.SetContent(value); }
		public void SetTemplate(DataTemplate value) { Content.SetTemplate(value); }
		public void SetHorizontalContentAlignment(HorizontalAlignment value) { Content.SetHorizontalContentAlignment(value); }
		public void SetVerticalContentAlignment(VerticalAlignment value) { Content.SetVerticalContentAlignment(value); }
		public void SetVisible(bool value) {
			foreach(BookTemplateElement element in Elements.Values)
				element.SetVisible(value);
		}
		public void SetZIndex(int value) { Grid.SetZIndex(value); }
		public void SetColumn(int value) { Grid.SetColumn(value); }
		public void SetClip(Geometry value) { Grid.SetClip(value); }
		public void SetTransform(BookTemplateElementType element, Transform transform) { Elements[element].SetTransform(transform); }
	}
	public class BookPageManager {
		#region Property
		public double BaseForeShadowWidth { get { return Pages[BookPageLayout.NextEven].BaseForeShadowWidth; } }
		public double OverlayForeShadowWidth { get { return Pages[BookPageLayout.NextOdd].OverlayForeShadowWidth; } }
		public double BackShadowWidth { get { return Pages[BookPageLayout.NextOdd].BackShadowWidth; } }
		public Rect OddPageRect { get { return OddPage.PageRect; } }
		public Rect EvenPageRect { get { return EvenPage.PageRect; } }
		protected internal Book Book { get; set; }
		protected internal Dictionary<BookPageLayout, BookPage> Pages { get; set; }
		protected internal int BasePageIndex {
			get {
				int index = Book.PageIndex + FirstPageIndex;
				if(index < FirstPageIndex)
					index = FirstPageIndex;
				if(index > LastPageIndex)
					index = LastPageIndex;
				return index % 2 != 0 ? index : index - 1;
			}
		}
		protected internal IList BookDataSource { get { return Book.DataSource; } }
		protected internal DataTemplate Template { get { return Book.PageTemplate; } }
		protected internal int PageCount { get { return BookDataSource == null ? 0 : BookDataSource.Count; } }
		protected internal bool IsNextPage { get { return PageIndex(BookPageLayout.NextOdd) != -1; } }
		protected internal bool IsPrevPage { get { return PageIndex(BookPageLayout.PrevEven) != -1; } }
		protected internal BookViewState State { get { return Book.EventHandler.ViewState; } }
		protected internal bool IsPrevView { get { return State == BookViewState.Prev; } }
		protected internal bool IsNextView { get { return State == BookViewState.Next; } }
		protected internal BookPage OddPage { get { return Pages[BookPageLayout.Odd]; } }
		protected internal BookPage EvenPage { get { return Pages[BookPageLayout.Even]; } }
		protected internal int FirstPageIndex { get { return Book.FirstPage == PageType.Odd ? 1 : 0; } }
		protected internal int LastPageIndex { get { return FirstPageIndex + PageCount - 1; } }
		#endregion
		public BookPageManager(Book book) { Book = book; }
		public bool CanChangePagesViewStateTo(BookViewState desiredState) {
			bool nextOk = (desiredState == BookViewState.Next) && IsNextPage;
			bool prevOk = (desiredState == BookViewState.Prev) && IsPrevPage;
			return nextOk || prevOk;
		}
		public void OnApplyTemplate() {
			InitializePagesDictionary();
			InitializePagesProperties();
		}
		public void UpdateAllProperties() {
			if(Pages == null)
				return;
			UpdateDragPoint();
			foreach(BookPageLayout layout in Pages.Keys) {
				UpdateContentProperty(layout);
				UpdateTemplateProperty(layout);
				UpdateVisibilityProperty(layout);
				UpdateClipProperty(layout);
				UpdateTransformProperties(layout);
			}
		}
		protected internal virtual void UpdateDragPoint() {
			if(Book.BookWidth != 0 && Book.BookHeight != 0 && Book.EventHandler.IsPartialTurn) {
				Point endPoint = Book.EventHandler.ViewState == BookViewState.Next ?
					Book.Animator.PartialTurnForwardEndPoint : Book.Animator.PartialTurnBackwardEndPoint;
				Book.DragTracker.SetDragPoint(endPoint);
			}
		}
		protected internal void InitializePagesDictionary() {
			Pages = new Dictionary<BookPageLayout, BookPage>();
			Array layoutsArray = EnumExtensions.GetValues(typeof(BookPageLayout));
			foreach(BookPageLayout layout in layoutsArray) {
				BookPage page = new BookPage(Book, layout);
				Pages.Add(layout, page);
			}
		}
		protected internal void InitializePagesProperties() {
			BookGeometryParams geometryParams = Book.GeometryParams;
			foreach(BookPageLayout layout in Pages.Keys) {
				BookPage page = Pages[layout];
				page.SetTemplate(Template);
				page.SetHorizontalContentAlignment(HorizontalAlignment.Stretch);
				page.SetVerticalContentAlignment(VerticalAlignment.Stretch);
				page.SetZIndex(ZIndex(layout));
				page.SetColumn(GetPageColumn(layout));
				page.SetShadowHeight(geometryParams.ShadowHeight);
				page.SetShadowTop(geometryParams.ShadowTop);
			}
		}
		protected virtual int GetPageColumn(BookPageLayout layout) { return (int)layout % 2; }
		protected internal void ClearContentProperty(BookPageLayout layout) { Pages[layout].SetContent(null); }
		protected internal void ClearTemplateProperty(BookPageLayout layout) { Pages[layout].SetTemplate(null); }
		protected internal void UpdateContentProperty(BookPageLayout layout) { Pages[layout].SetContent(PageContent(layout)); }
		protected internal void UpdateTemplateProperty(BookPageLayout layout) { Pages[layout].SetTemplate(PageTemplate(layout)); }
		protected internal void UpdateVisibilityProperty(BookPageLayout layout) { Pages[layout].SetVisible(IsVisible(layout)); }
		protected internal void UpdateClipProperty(BookPageLayout layout) {
			Geometry geometry = BookClipBuilder.CreateClip(Book, layout);
			Pages[layout].SetClip(geometry);
		}
		protected internal void UpdateTransformProperties(BookPageLayout layout) {
			Array elements = EnumExtensions.GetValues(typeof(BookTemplateElementType));
			foreach(BookTemplateElementType element in elements) {
				if(!IsTransform(layout, element))
					continue;
				Transform transform = CreateTransform(Book, layout, element);
				Pages[layout].SetTransform(element, transform);
			}
		}
		public Transform CreateTransform(Book book, BookPageLayout layout, BookTemplateElementType element) {
			BookTransformBuilder builder = CreateTransformBuilder(book, layout, element);
			RotateTransform rotation = new RotateTransform() { CenterX = builder.RotationX, CenterY = builder.RotationY, Angle = builder.RotationAngle };
			TranslateTransform translation = new TranslateTransform() { X = builder.TranslationX, Y = builder.TranslationY };
			TransformGroup transforms = new TransformGroup();
			transforms.Children.Add(rotation);
			transforms.Children.Add(translation);
			return transforms;
		}
		protected internal BookTransformBuilder CreateTransformBuilder(Book book, BookPageLayout layout, BookTemplateElementType element) {
			switch(element) {
			case BookTemplateElementType.Grid:
				return CreateGridTransformBuilder(book, layout, element);
			case BookTemplateElementType.BaseForeShadow:
				return new BookBaseForeShadowTransformBuilder(book, layout, element);
			case BookTemplateElementType.OverlayForeShadow:
				return new BookOverlayForeShadowTransformBuilder(book, layout, element);
			case BookTemplateElementType.BackShadow:
				return new BookBackShadowTransformBuilder(book, layout, element);
			default:
				return new BookTransformBuilder(book, layout, element);
			}
		}
		public virtual BookGridTransformBuilder CreateGridTransformBuilder(Book book, BookPageLayout layout, BookTemplateElementType element) {
			return new BookGridTransformBuilder(book, layout, element);
		}
		protected internal int PageIndex(BookPageLayout layout) {
			int index = BasePageIndex + (int)layout - 2;
			return IsPageIndexInRange(index) ? index : -1;
		}
		protected internal object PageContent(BookPageLayout layout) {
			int index = PageIndex(layout);
			return IsPageIndexInRange(index) ? BookDataSource[index - FirstPageIndex] : null;
		}
		protected internal DataTemplate PageTemplate(BookPageLayout layout) { return IsLayoutPageIndexInRange(layout) ? Template : null; }
		protected internal bool IsLayoutPageIndexInRange(BookPageLayout layout) { return IsPageIndexInRange(PageIndex(layout)); }
		protected internal bool IsPageIndexInRange(int index) { return index >= FirstPageIndex && index <= LastPageIndex; }
		protected internal virtual bool IsVisible(BookPageLayout layout) {
			bool always = layout == BookPageLayout.Even || layout == BookPageLayout.Odd;
			bool prev = IsPrevView && (layout == BookPageLayout.PrevEven || layout == BookPageLayout.PrevOdd);
			bool next = IsNextView && (layout == BookPageLayout.NextEven || layout == BookPageLayout.NextOdd);
			return always | prev | next;
		}
		protected internal bool IsTransform(BookPageLayout layout, BookTemplateElementType element) {
			bool prev = IsPrevView && (layout == BookPageLayout.PrevEven);
			bool next = IsNextView && (layout == BookPageLayout.NextOdd);
			bool baseForeShadow = (IsPrevView || IsNextView) && element == BookTemplateElementType.BaseForeShadow;
			return prev || next || baseForeShadow;
		}
		protected internal static int ZIndex(BookPageLayout layout) {
			switch(layout) {
				case BookPageLayout.PrevOdd:
				case BookPageLayout.NextEven:
					return -1;
				case BookPageLayout.PrevEven:
				case BookPageLayout.NextOdd:
					return 1;
				default:
					return 0;
			}
		}
	}
}
