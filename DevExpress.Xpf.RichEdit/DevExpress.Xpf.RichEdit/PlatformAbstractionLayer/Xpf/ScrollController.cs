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
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using DevExpress.Office.Internal;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.RichEdit;
using DevExpress.Xpf.RichEdit.UI;
#if SL
using PlatformIndependentScrollEventHandler = System.Windows.Controls.Primitives.ScrollEventHandler;
using PlatformIndependentScrollEventArgs = System.Windows.Controls.Primitives.ScrollEventArgs;
using PlatformIndependentScrollEventType = System.Windows.Controls.Primitives.ScrollEventType;
#else
using PlatformIndependentScrollEventHandler = System.Windows.Forms.ScrollEventHandler;
using PlatformIndependentScrollEventArgs = System.Windows.Forms.ScrollEventArgs;
using PlatformIndependentScrollEventType = System.Windows.Forms.ScrollEventType;
#endif
namespace DevExpress.XtraRichEdit.Internal.PrintLayout {
	#region XpfScrollBarAdapter
	public class XpfScrollBarAdapter : IPlatformSpecificScrollBarAdapter {
		public virtual void OnScroll(ScrollBarAdapter adapter, object sender, PlatformIndependentScrollEventArgs e) {
			adapter.RaiseScroll(e);
		}
		public virtual void ApplyValuesToScrollBarCore(ScrollBarAdapter adapter) {
			DoScrollBarAction(adapter, delegate {
				IOfficeScrollbar scrollBar = adapter.ScrollBar;
				scrollBar.Minimum = (int)Math.Round(adapter.Factor * adapter.Minimum);
				scrollBar.Maximum = (int)Math.Round(adapter.Factor * adapter.Maximum) - 1 - (int)Math.Round(adapter.Factor * adapter.LargeChange);
				scrollBar.SmallChange = 1;
				scrollBar.LargeChange = (int)Math.Round(adapter.Factor * adapter.LargeChange);
				scrollBar.Value = (int)Math.Round(adapter.Factor * adapter.Value);
				scrollBar.Enabled = adapter.Enabled;
				IXpfOfficeScrollbar xpfScrollBar = scrollBar as IXpfOfficeScrollbar;
				if (xpfScrollBar != null) {
					xpfScrollBar.ViewportSize = scrollBar.LargeChange;
					xpfScrollBar.Visibility = adapter.Maximum > 2 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
				}
			}, false);
		}
		protected void DoScrollBarAction(ScrollBarAdapter adapter, Action a, bool waitForCompletion) {
			IXpfOfficeScrollbar xpfScrollBar = adapter.ScrollBar as IXpfOfficeScrollbar;
			if (xpfScrollBar == null) {
				a();
				return;
			}
			Dispatcher dispatcher = xpfScrollBar.Dispatcher;
			if (!dispatcher.CheckAccess()) {
				if (waitForCompletion) {
					System.Threading.AutoResetEvent wait = new System.Threading.AutoResetEvent(false);
					Action action = delegate { a(); wait.Set(); };
					dispatcher.BeginInvoke(action, new object[0]);
					wait.WaitOne();
				}
				else
					dispatcher.BeginInvoke(a, new object[0]);
			}
			else
				a();
		}
		public int GetRawScrollBarValue(ScrollBarAdapter adapter) {
			double result = 0;
			DoScrollBarAction(adapter, delegate { result = adapter.ScrollBar.Value; }, true);
			return (int)result;
		}
		public bool SetRawScrollBarValue(ScrollBarAdapter adapter, int value) {
			if (adapter.GetRawScrollBarValue() != value) {
				DoScrollBarAction(adapter, delegate { adapter.ScrollBar.Value = value; }, false);
				adapter.Value = (long)Math.Round(value / adapter.Factor);
				return true;
			}
			else {
				long newValue = (long)Math.Round(value / adapter.Factor);
				bool result = newValue != adapter.Value;
				adapter.Value = newValue;
				return result;
			}
		}
		public virtual int GetPageUpRawScrollBarValue(ScrollBarAdapter adapter) {
			int result = 0;
			DoScrollBarAction(adapter, delegate {
				result = (int)(adapter.ScrollBar.Value - adapter.ScrollBar.LargeChange);
				if (result < adapter.ScrollBar.Minimum)
					result = (int)adapter.ScrollBar.Minimum;
			}, true);
			return result;
		}
		public virtual int GetPageDownRawScrollBarValue(ScrollBarAdapter adapter) {
			int result = 0;
			DoScrollBarAction(adapter, delegate {
				result = (int)(adapter.ScrollBar.Value + adapter.ScrollBar.LargeChange);
				if (result > adapter.ScrollBar.Maximum)
					result = (int)adapter.ScrollBar.Maximum;
			}, true);
			return result;
		}
		public virtual PlatformIndependentScrollEventArgs CreateLastScrollEventArgs(ScrollBarAdapter adapter) {
			return new PlatformIndependentScrollEventArgs(XpfOfficeScrollbar.ConvertScrollEventType(ScrollEventType.Last), XpfOfficeScrollbar.ConvertScrollBarValue(adapter.ScrollBar.Maximum));
		}
	}
	#endregion
	#region XpfRichEditViewVerticalScrollController
	public class XpfRichEditViewVerticalScrollController : RichEditViewVerticalScrollController {
		const double PageNumberPopupOffset = 20;
		const double ScrollTimerInterval = 100;
		Popup pageNumberPopup;
		DispatcherTimer scrollTimer;
		public XpfRichEditViewVerticalScrollController(RichEditView view)
			: base(view) {
		}
		protected virtual void OnPageNumberWindowSizeChanged(object sender, SizeChangedEventArgs e) {
			IXpfOfficeScrollbar scrollBar = ScrollBar as IXpfOfficeScrollbar;
			if (scrollBar == null)
				return;
			PageNumberWindow pageNumberWindow = (PageNumberWindow)pageNumberPopup.Child;
			RichEditControl control = (RichEditControl)View.Control;
			Point p = scrollBar.TransformToVisual((FrameworkElement)LayoutHelper.GetTopLevelVisual(control)).Transform(new Point());
			pageNumberPopup.HorizontalOffset = p.X - PageNumberPopupOffset - pageNumberWindow.ActualWidth;
			pageNumberPopup.VerticalOffset = p.Y + PageNumberPopupOffset;
		}
		void CreateScrollTimer() {
			scrollTimer = new DispatcherTimer();
			scrollTimer.Interval = TimeSpan.FromMilliseconds(ScrollTimerInterval);
			scrollTimer.Tick += new EventHandler(OnScrollTimerTick);
			scrollTimer.Start();
		}
		void DestroyScrollTimer() {
			if (scrollTimer == null)
				return;
			scrollTimer.Stop();
			scrollTimer.Tick -= new EventHandler(OnScrollTimerTick);
			scrollTimer = null;
		}
		string GetPageNumberWindowText(int pageNumber) {
			return String.Format("Page: {0}", pageNumber);
		}
		void HidePageNumber() {
			if (pageNumberPopup == null)
				return;
			pageNumberPopup.IsOpen = false;
			pageNumberPopup = null;
			this.deferredPageIndex = -1;
		}
		int PageCount {
			get {
				PageBasedRichEditView view = View as PageBasedRichEditView;
				if (view != null)
					return view.PageCount;
				else
					return 1;
			}
		}
		bool IsLargeDocument() {
			return PageCount > 20;
		}
		void OnScrollTimerTick(object sender, EventArgs e) {
			if (deferredPageIndex < 0)
				return;
			(new ScrollToPageCommand(View.Control) { PageIndex = this.deferredPageIndex }).Execute();
		}
		int deferredPageIndex = -1;
		void ShowPageNumber(int pageNumber) {
			this.deferredPageIndex = pageNumber - 1;
			if (pageNumberPopup != null) {
				((PageNumberWindow)pageNumberPopup.Child).Text = GetPageNumberWindowText(pageNumber);
				return;
			}
			pageNumberPopup = new Popup();
			PageNumberWindow pageNumberWindow = new PageNumberWindow() { Text = GetPageNumberWindowText(pageNumber) };
			pageNumberWindow.SizeChanged += new SizeChangedEventHandler(OnPageNumberWindowSizeChanged);
			pageNumberPopup.Child = pageNumberWindow;
			pageNumberPopup.IsOpen = true;
		}
		PlatformIndependentScrollEventType GetScrollEventType(PlatformIndependentScrollEventArgs e) {
#if !SL
			return e.Type;
#else
			return e.ScrollEventType;
#endif
		}
		protected internal override bool UpdatePageNumberOnScroll(PlatformIndependentScrollEventArgs e) {
			if (GetScrollEventType(e) == PlatformIndependentScrollEventType.ThumbTrack) {
				if (IsLargeDocument()) {
					DestroyScrollTimer();
					CreateScrollTimer();
					ShowPageNumber((int)(e.NewValue * (this.PageCount - 1) / (ScrollBar.Maximum - ScrollBar.Minimum) + 1));
					return true;
				}
				ShowPageNumber(View.PageViewInfos[0].Page.PageIndex + 1);
			}
			if (GetScrollEventType(e) == PlatformIndependentScrollEventType.EndScroll) {
				if (IsLargeDocument()) {
					OnScrollTimerTick(scrollTimer, new EventArgs());
					DestroyScrollTimer();
				}
				HidePageNumber();
				return true;
			}
			return false;
		}
		protected internal override bool IsScrollTypeValid(PlatformIndependentScrollEventArgs e) {
			return true;
		}
		int oldValue = 0;
		protected internal override int CalculateScrollDelta(PlatformIndependentScrollEventArgs e) {
			return (int)(e.NewValue - oldValue);
		}
		protected internal override void ApplyNewScrollValue(int value) {
			ScrollByTopInvisibleHeightDelta(value - ScrollBarAdapter.Value);
		}
		protected internal override void ApplyNewScrollValueToScrollEventArgs(PlatformIndependentScrollEventArgs e, int value) {
			oldValue = value;
		}
	}
	#endregion
	#region XpfRichEditViewHorizontalScrollController
	public class XpfRichEditViewHorizontalScrollController : RichEditViewHorizontalScrollController {
		public XpfRichEditViewHorizontalScrollController(RichEditView view)
			: base(view) {
		}
		protected internal override void OnScrollCore(PlatformIndependentScrollEventArgs e) {
			ScrollBarAdapter.SetRawScrollBarValue((int)e.NewValue);
		}
	}
	#endregion
}
