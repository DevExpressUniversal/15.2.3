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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet.Internal;
using System.Windows.Controls;
using DevExpress.XtraSpreadsheet;
using System.Drawing;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.Xpf.Spreadsheet.Localization;
using DevExpress.Xpf.Spreadsheet.UI;
using DevExpress.Utils.Controls;
using DevExpress.Xpf.Spreadsheet.Internal;
using System.Windows.Threading;
using DevExpress.XtraSpreadsheet.Mouse;
using System.Linq;
using System.Linq.Expressions;
using PlatformIWin32Window = System.Windows.Forms.IWin32Window;
using PlatformIndependentCursor = System.Windows.Forms.Cursor;
using PlatformDialogResult = System.Windows.Forms.DialogResult;
using PlatformIndependentScrollEventArgs = System.Windows.Forms.ScrollEventArgs;
using DevExpress.Office.Internal;
using System.Windows.Controls.Primitives;
using DevExpress.XtraSpreadsheet.Model;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Input;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Spreadsheet.Internal {
	public enum SpreadsheetHitTestType { None, Worksheet, TabSelector }
	#region XpfScrollBarAdapter
	public class XpfScrollBarAdapter : IPlatformSpecificScrollBarAdapter {
		public virtual void OnScroll(ScrollBarAdapter adapter, object sender, PlatformIndependentScrollEventArgs e) {
			adapter.RaiseScroll(e);
		}
		public virtual void ApplyValuesToScrollBarCore(ScrollBarAdapter adapter) {
			DoScrollBarAction(adapter, delegate {
				IOfficeScrollbar scrollBar = adapter.ScrollBar;
				int largeChange = (int)Math.Round(adapter.Factor * adapter.LargeChange);
				scrollBar.LargeChange = largeChange;
				scrollBar.SmallChange = 1;
				scrollBar.Minimum = (int)Math.Round(adapter.Factor * adapter.Minimum);
				scrollBar.Maximum = (int)Math.Round(adapter.Factor * adapter.Maximum) - largeChange + 1;
				scrollBar.Value = (int)Math.Round(adapter.Factor * adapter.Value);
				scrollBar.Enabled = adapter.Enabled;
				IXpfOfficeScrollbar xpfScrollBar = scrollBar as IXpfOfficeScrollbar;
				if (xpfScrollBar != null) {
					xpfScrollBar.ViewportSize = largeChange;
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
				int minimum = adapter.ScrollBar.Minimum;
				if (result < minimum)
					result = minimum;
			}, true);
			return result;
		}
		public virtual int GetPageDownRawScrollBarValue(ScrollBarAdapter adapter) {
			int result = 0;
			DoScrollBarAction(adapter, delegate {
				result = (int)(adapter.ScrollBar.Value + adapter.ScrollBar.LargeChange);
				int maximum = adapter.ScrollBar.Maximum;
				if (result > maximum)
					result = maximum;
			}, true);
			return result;
		}
		public virtual PlatformIndependentScrollEventArgs CreateLastScrollEventArgs(ScrollBarAdapter adapter) {
			return new PlatformIndependentScrollEventArgs(XpfOfficeScrollbar.ConvertScrollEventType(ScrollEventType.Last), XpfOfficeScrollbar.ConvertScrollBarValue(adapter.ScrollBar.Maximum));
		}
	}
	public class SpreadsheetOfficeScrollBar : XpfOfficeScrollbar {
		public SpreadsheetOfficeScrollBar(ScrollBar scrollBar)
			: base(scrollBar) {
			SubcribeUpAndDownButtonsClick();
		}
		void SubcribeUpAndDownButtonsClick() {
			RepeatButton downButton = LayoutHelper.FindElement(ScrollBar, (e) => e.GetType() == typeof(RepeatButton) && (e as RepeatButton).Command == ScrollBar.LineDownCommand) as RepeatButton;
			RepeatButton rightButton = LayoutHelper.FindElement(ScrollBar, (e) => e.GetType() == typeof(RepeatButton) && (e as RepeatButton).Command == ScrollBar.LineRightCommand) as RepeatButton;
			if (downButton != null) downButton.Click += OnClick;
			if (rightButton != null) rightButton.Click += OnClick;
		}
		void OnClick(object sender, RoutedEventArgs e) {
			if (ScrollBar.Value == ScrollBar.Maximum) {
				double oldValue = (double)ScrollBar.GetCoerceOldValue(ScrollBar.ValueProperty);
				RaiseScroll(ConvertEventArgs(new ScrollEventArgs(ScrollEventType.SmallIncrement, ScrollBar.Value), (int)oldValue));
			}
		}
	}
	#endregion
	#region ScrollControllers
	public class XpfSpreadsheetViewVerticalScrollController : SpreadsheetViewVerticalScrollController {
		public XpfSpreadsheetViewVerticalScrollController(SpreadsheetView view) : base(view) { }
		protected internal override void ApplyNewScrollValue(int value) {
			ScrollBarAdapter.SetRawScrollBarValue(value);
		}
		int oldValue = 0;
		protected internal override void ApplyNewScrollValueToScrollEventArgs(PlatformIndependentScrollEventArgs e, int value) {
			oldValue = value;
		}
		protected internal override int CalculateScrollDelta(PlatformIndependentScrollEventArgs e) {
			long endOffset = 0;
			return endOffset == 0 ? (int)(e.NewValue - e.OldValue) : (int)(e.NewValue - oldValue);
		}
		protected internal override bool IsScrollTypeValid(PlatformIndependentScrollEventArgs e) {
			return e.Type != System.Windows.Forms.ScrollEventType.EndScroll;
		}
	}
	public class XpfSpreadsheetViewHorizontalScrollController : SpreadsheetViewHorizontalScrollController {
		public XpfSpreadsheetViewHorizontalScrollController(SpreadsheetView view) : base(view) { }
		protected internal override void ApplyNewScrollValue(int value) {
			ScrollBarAdapter.SetRawScrollBarValue(value);
		}
		protected internal override void ApplyNewScrollValueToScrollEventArgs(PlatformIndependentScrollEventArgs e, int value) { }
		protected internal override int CalculateScrollDelta(PlatformIndependentScrollEventArgs e) {
			return (int)(e.NewValue - e.OldValue);
		}
		protected internal override bool IsScrollTypeValid(PlatformIndependentScrollEventArgs e) {
			return e.Type != System.Windows.Forms.ScrollEventType.EndScroll;
		}
	}
	#endregion
}
