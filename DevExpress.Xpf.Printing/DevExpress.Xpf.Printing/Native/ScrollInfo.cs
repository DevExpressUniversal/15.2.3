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
using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Printing.Native {
	class ScrollInfo : ScrollInfoBase {
		#region Fields
		const double epsilon = 1E-12;
		readonly Thickness scrollingPlay = new Thickness(0, 100, 0, 100);
		bool changingCurrentPageIndex;
		readonly IDocumentPreviewModel model;
		Thickness ScrollingPlay {
			get {
				return ViewportHeight < PageViewHeightWithMargins ? scrollingPlay :
					new Thickness();
			}
		}
		#endregion
		#region Properties
		protected override double ScrollablePageViewHeight {
			get {
				if(model == null || model.PageCount == 0)
					return 0;
				return PageViewHeightWithMargins + ScrollingPlay.Top + ScrollingPlay.Bottom;
			}
		}
		double PageViewHeightWithMargins {
			get { return PageViewHeight + PageMargin.Top + PageMargin.Bottom; }
		}
		double ScrollableWidth {
			get { return Math.Max(0, ExtentWidth - ViewportWidth); }
		}
		double ScrollableHeight {
			get { return Math.Max(0, ExtentHeight - ViewportHeight); }
		}
		double HorizontalRelativityFactor {
			get { return (ScrollablePageViewWidth - ViewportWidth) / ScrollableWidth; }
		}
		double VerticalRelativityFactor {
			get {
				double scrollablePageHeight = ScrollableHeight / model.PageCount;
				return (ScrollablePageViewHeight - ViewportHeight) / scrollablePageHeight;
			}
		}
		#endregion
		#region Constructors
		public ScrollInfo(FrameworkElement scrollablePageView, IDocumentPreviewModel model, Thickness pageMargin)
			: base(scrollablePageView, model, pageMargin) {
			this.model = model;
		}
		#endregion
		#region IScrollInfo Members
		public override double ExtentHeight {
			get {
				if(model == null || model.PageCount == 0)
					return 0;
				return Math.Max(ScrollablePageViewHeight, ViewportHeight) * model.PageCount;
			}
		}
#if SL
		public override Rect MakeVisible(UIElement element, Rect rectangle) {
			return MakeVisibleCore(element, rectangle);
		}
#else
		public override Rect MakeVisible(Visual visual, Rect rectangle) {
			return MakeVisibleCore((UIElement)visual, rectangle);
		}
#endif
		#endregion
		#region Methods
		public override double GetTransformX() {
			return ScrollablePageViewWidth > ViewportWidth ? -GetLogicalHorizontalOffset(HorizontalOffset) : 0;
		}
		public override double GetTransformY() {
			return ScrollablePageViewHeight - ScrollingPlay.Top - ScrollingPlay.Bottom > ViewportHeight ? -GetLogicalVerticalOffset(VerticalOffset) : 0;
		}
		double GetLogicalHorizontalOffset(double offset) {
			return offset * HorizontalRelativityFactor;
		}
		double GetLogicalVerticalOffset(double offset) {
			double scrollablePageHeight = ScrollableHeight / model.PageCount;
			int pageIndex = GetVisiblePageIndex(offset);
			double pageOffset = offset - scrollablePageHeight * pageIndex;
			double realPageVerticalOffset = pageOffset * VerticalRelativityFactor;
			realPageVerticalOffset = Math.Max(0, realPageVerticalOffset - ScrollingPlay.Top);
			return Math.Min(realPageVerticalOffset, PageViewHeight + PageMargin.Top + PageMargin.Bottom - ViewportHeight);
		}
		int GetVisiblePageIndex(double offset) {
			return Math.Min(GetRealPageIndex(offset), model.PageCount - 1);
		}
		int GetRealPageIndex(double offset) {
			double scrollablePageHeight = ScrollableHeight / model.PageCount;
			return (int)Math.Floor(offset / scrollablePageHeight + epsilon);
		}
		public override void SetCurrentPageIndex() {
			try {
				changingCurrentPageIndex = true;
				VerticalOffset = GetPageOffset(model.CurrentPageIndex) + ScrollablePageViewLocalVerticalOffset;
				if (VerticalOffset < 0) VerticalOffset = 0;
				model.CurrentPageIndex = GetVisiblePageIndex(VerticalOffset);
				UpdateScrollablePageViewLocalVerticalOffset();
			} finally {
				changingCurrentPageIndex = false;
			}
		}
		protected override double GetVerticalScrollOffset(ScrollMode scrollMode, ScrollDirection scrollDirection) {
			double scrollStep = GetStep(scrollMode);
			if(ScrollablePageViewHeight > ViewportHeight) {
				int index = GetRealPageIndex(VerticalOffset);
				double scrollablePageHeight = ScrollableHeight / model.PageCount;
				double pageOffset = VerticalOffset - scrollablePageHeight * index;
				if(scrollDirection == ScrollDirection.Down) {
					if(VerticalOffset == (index + 1) * scrollablePageHeight - 1)
						return (index + 1) * scrollablePageHeight;
					if(pageOffset + scrollStep > scrollablePageHeight)
						return (index + 1) * scrollablePageHeight - 1;
					return VerticalOffset + scrollStep;
				}
				return VerticalOffset == index * scrollablePageHeight ? index * scrollablePageHeight - 1 : pageOffset - scrollStep < 0 ? index * scrollablePageHeight : VerticalOffset - scrollStep;
			}
			return scrollDirection == ScrollDirection.Down ? VerticalOffset + scrollStep : VerticalOffset - scrollStep;
		}
		Rect MakeVisibleCore(UIElement element, Rect rectangle) {
			if(model.PageContent == null || !model.PageContent.IsInVisualTree())
				return Rect.Empty;
			Rect rect = LayoutHelper.GetRelativeElementRect((UIElement)element, model.PageContent);
			double zoomFactor = model.Zoom / 100;
			rect = new Rect(rect.X * zoomFactor, rect.Y * zoomFactor, rect.Width * zoomFactor, rect.Height * zoomFactor);
			if(ScrollablePageViewHeight > ViewportHeight) {
				if(rect.Height < ViewportHeight) {
					double realPageVerticalOffset = rect.Top + 0.5 * rect.Height - 0.5 * ViewportHeight + ScrollingPlay.Top + PageMargin.Top;
					double verticalOffset = realPageVerticalOffset / VerticalRelativityFactor;
					verticalOffset = Math.Min(Math.Max(0, verticalOffset), ScrollableHeight / model.PageCount - 1);
					SetVerticalOffset(verticalOffset + (ScrollableHeight / model.PageCount * model.CurrentPageIndex));
				} else {
					SetVerticalOffset(rect.Top + (ScrollableHeight / model.PageCount * model.CurrentPageIndex));
				}
			}
			if(ScrollablePageViewWidth > ViewportWidth) {
				if(rect.Width < ViewportWidth) {
					double realPageHorizontalOffset = rect.Left + 0.5 * rect.Width - 0.5 * ViewportWidth + PageMargin.Left;
					double horizontalOffset = realPageHorizontalOffset / HorizontalRelativityFactor;
					SetHorizontalOffset(horizontalOffset);
				} else
					SetHorizontalOffset(rect.Left);
			}
			return rect;
		}
		protected override void model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
			base.model_PropertyChanged(sender, e);
			if(e.PropertyName == "PageCount" && model.PageCount == 0) {
				UpdateScrollablePageViewLocalVerticalOffset();
			} else if(e.PropertyName == "CurrentPageIndex") {
				if(!changingCurrentPageIndex && model.CurrentPageIndex >= 0 && model.PageCount > 0) {
					VerticalOffset = GetPageOffset(model.CurrentPageIndex);
					scrollablePageView.InvalidateArrange();
				}
			}
		}
		double GetPageOffset(int pageIndex) {
			return ScrollableHeight / model.PageCount * pageIndex;
		}
		public override void SetVerticalOffset(double offset) {
			base.SetVerticalOffset(
				double.IsPositiveInfinity(offset) && model.PageCount > 0 ? GetPageOffset(model.PageCount - 1) :
				double.IsNegativeInfinity(offset) ? 0 : 
				offset);
		}
		#endregion
	}
}
