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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
#if !SL
using System.Windows.Media;
#endif
namespace DevExpress.Xpf.Printing.Native {
	public class SimpleScrollInfo : ScrollInfoBase {
		IPreviewModel model;
		#region Constructors
		public SimpleScrollInfo(FrameworkElement scrollablePageView, IPreviewModel model, Thickness pageMargin)
			: base(scrollablePageView, model, pageMargin) {
				this.model = model;
		}
		#endregion
		#region Properties
		protected override double ScrollablePageViewHeight {
			get {
				if(model == null || model.PageCount == 0)
					return 0;
				return PageViewHeight + PageMargin.Top + PageMargin.Bottom;
			}
		}
		public override double ExtentHeight {
			get {
				if(model == null || model.PageCount == 0)
					return 0;
				return Math.Max(ScrollablePageViewHeight, ViewportHeight);
			}
		}
		#endregion
		#region Methods
#if SL
		public override Rect MakeVisible(UIElement element, Rect rectangle) {
			return MakeVisibleCore(element, rectangle);
		}
#else
		public override Rect MakeVisible(Visual visual, Rect rectangle) {
			return MakeVisibleCore((UIElement)visual, rectangle);
		}
#endif
		public override double GetTransformX() {
			return ScrollablePageViewWidth > ViewportWidth ? -HorizontalOffset : 0;
		}
		public override double GetTransformY() {
			return ScrollablePageViewHeight > ViewportHeight ? -VerticalOffset : 0;
		}
		public override void SetCurrentPageIndex() {
		}
		protected override double GetVerticalScrollOffset(ScrollMode scrollMode, ScrollDirection scrollDirection) {
			double scrollStep = GetStep(scrollMode);
			if(scrollDirection == ScrollDirection.Down)
				return VerticalOffset + scrollStep;
			return VerticalOffset - scrollStep;
		}
		Rect MakeVisibleCore(UIElement element, Rect rectangle) {
			if(model.PageContent == null || !model.PageContent.IsInVisualTree())
				return Rect.Empty;
			Rect zoomedRect = LayoutHelper.GetRelativeElementRect(element, model.PageContent);
			double zoomFactor = model.Zoom / 100;
			zoomedRect = new Rect(zoomedRect.X * zoomFactor, zoomedRect.Y * zoomFactor, zoomedRect.Width * zoomFactor, zoomedRect.Height * zoomFactor);
			if(IsVisible(zoomedRect))
				return zoomedRect;
			if(ScrollablePageViewHeight > ViewportHeight) {
				double elementVerticalOffset = 0d;
				if(zoomedRect.Height < ViewportHeight)
					elementVerticalOffset = Math.Max(0d, zoomedRect.Top + (0.5 * zoomedRect.Height) - (0.5 * ViewportHeight) + PageMargin.Top);
				else
					elementVerticalOffset = zoomedRect.Top;
				SetVerticalOffset(elementVerticalOffset);
			}
			if(ScrollablePageViewWidth > ViewportWidth) {
				double elementHorizontalOffset = 0d;
				if(zoomedRect.Width < ViewportWidth)
					elementHorizontalOffset = Math.Max(0d, zoomedRect.Left + (0.5 * zoomedRect.Width) - (0.5 * ViewportWidth) + PageMargin.Left);
				else
					elementHorizontalOffset = zoomedRect.Left;
				SetHorizontalOffset(elementHorizontalOffset);
			}
			return zoomedRect;
		}
		bool IsVisible(Rect rectangle) {
			if(ScrollablePageViewHeight < ViewportHeight && ScrollablePageViewWidth < ViewportWidth)
				return true;
			Rect viewRect = new Rect(HorizontalOffset, VerticalOffset, ViewportWidth, ViewportHeight);
			return viewRect.Contains(rectangle.TopLeft()) && viewRect.Contains(rectangle.BottomRight());
		}
		#endregion
	}
}
