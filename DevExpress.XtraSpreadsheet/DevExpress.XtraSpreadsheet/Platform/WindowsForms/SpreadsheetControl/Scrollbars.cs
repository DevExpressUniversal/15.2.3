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
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.Office.Internal;
using DevExpress.XtraSpreadsheet;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.Office.Utils;
namespace DevExpress.XtraSpreadsheet {
	public partial class SpreadsheetControl {
		DevExpress.XtraEditors.VScrollBar verticalScrollbar;
		DevExpress.XtraEditors.HScrollBar horizontalScrollbar;
		SplitContainerControl horizontalSplitContainer;
		internal DevExpress.XtraEditors.VScrollBar VerticalScrollBar { get { return verticalScrollbar; } }
		internal DevExpress.XtraEditors.HScrollBar HorizontalScrollBar { get { return horizontalScrollbar; } }
		internal SplitContainerControl HorizontalSplitContainer { get { return horizontalSplitContainer; } }
		IOfficeScrollbar IInnerSpreadsheetControlOwner.CreateVerticalScrollBar() {
			return this.CreateVerticalScrollBar();
		}
		IOfficeScrollbar IInnerSpreadsheetControlOwner.CreateHorizontalScrollBar() {
			return this.CreateHorizontalScrollBar();
		}
		protected internal virtual IOfficeScrollbar CreateVerticalScrollBar() {
			this.verticalScrollbar = new DevExpress.XtraEditors.VScrollBar();
			this.Controls.Add(verticalScrollbar);
			return CreateScrollBarCore(verticalScrollbar);
		}
		protected bool IsTouchModeHorizontalScrollBar { get { return horizontalScrollbar != null && ScrollBarBase.GetUIMode(horizontalScrollbar) == ScrollUIMode.Touch && horizontalScrollbar.IsOverlapScrollBar; } }
		protected internal virtual IOfficeScrollbar CreateHorizontalScrollBar() {
			CreateSplitContainer();
			this.horizontalScrollbar = new DevExpress.XtraEditors.HScrollBar();
			ScrollBarBase.ApplyUIMode(this.horizontalScrollbar);
			if (IsTouchModeHorizontalScrollBar) {
				this.Controls.Add(horizontalScrollbar);
			}
			else {
				this.horizontalScrollbar.Dock = DockStyle.Fill;
				horizontalSplitContainer.Panel2.Controls.Add(horizontalScrollbar);
			}
			this.Controls.Add(horizontalSplitContainer);
			return CreateScrollBarCore(horizontalScrollbar);
		}
		protected internal void CreateSplitContainer() {
			horizontalSplitContainer = new SplitContainerControl();
			horizontalSplitContainer.FixedPanel = SplitFixedPanel.None;
			horizontalSplitContainer.LookAndFeel.ParentLookAndFeel = this.LookAndFeel;
		}
		protected internal virtual IOfficeScrollbar CreateScrollBarCore(ScrollBarBase scrollBar) {
			scrollBar.Cursor = Cursors.Default;
			scrollBar.LookAndFeel.ParentLookAndFeel = this.lookAndFeel;
			ScrollBarBase.ApplyUIMode(scrollBar);
			return new DevExpress.XtraSpreadsheet.Internal.WinFormsOfficeScrollbar(scrollBar);
		}
		void DisposeScrollbars() {
			if (verticalScrollbar != null) {
				verticalScrollbar.Dispose();
				verticalScrollbar = null;
			}
			if (horizontalScrollbar != null) {
				horizontalScrollbar.Dispose();
				horizontalScrollbar = null;
			}
		}
		protected internal void UpdateVerticalScrollbarVisibility() {
			verticalScrollbar.SetVisibility(CalculateVerticalScrollbarVisibility());
		}
		protected internal void UpdateHorizontalScrollbarAndTabSelectorVisibility() {
			bool isHorizontalScrollbarVisible = CalculateHorizontalScrollbarVisibility();
			bool isTabSelectorVisible = CalculateTabSelectorVisibility();
			horizontalScrollbar.SetVisibility(isHorizontalScrollbarVisible);
			bool isDesktopHorizontalScrollbarVisible = isHorizontalScrollbarVisible && !IsTouchModeHorizontalScrollBar;
			bool isSplitContainerVisible = isDesktopHorizontalScrollbarVisible || isTabSelectorVisible;
			horizontalSplitContainer.Visible = isSplitContainerVisible;
			if (!isSplitContainerVisible)
				return;
			horizontalSplitContainer.PanelVisibility = CalculateContainerPanelsVisibility(isDesktopHorizontalScrollbarVisible, isTabSelectorVisible);
		}
		SplitPanelVisibility CalculateContainerPanelsVisibility(bool isHorizontalScrollbarVisible, bool isTabSelectorVisible) {
			if (isHorizontalScrollbarVisible && isTabSelectorVisible)
				return SplitPanelVisibility.Both;
			if (isTabSelectorVisible)
				return SplitPanelVisibility.Panel1;
			if (isHorizontalScrollbarVisible)
				return SplitPanelVisibility.Panel2;
			Exceptions.ThrowInternalException();
			return SplitPanelVisibility.Both;
		}
		protected internal virtual bool CalculateVerticalScrollbarVisibility() {
			SpreadsheetScrollbarVisibility visibility = Options.VerticalScrollbar.Visibility;
			switch (visibility) {
				default:
				case SpreadsheetScrollbarVisibility.Auto:
					return true;
				case SpreadsheetScrollbarVisibility.Visible:
					return true;
				case SpreadsheetScrollbarVisibility.Hidden:
					return false;
			}
		}
		protected internal virtual bool CalculateHorizontalScrollbarVisibility() {
			SpreadsheetScrollbarVisibility visibility = Options.HorizontalScrollbar.Visibility;
			switch (visibility) {
				default:
				case SpreadsheetScrollbarVisibility.Auto:
					return true;
				case SpreadsheetScrollbarVisibility.Visible:
					return true;
				case SpreadsheetScrollbarVisibility.Hidden:
					return false;
			}
		}
		protected internal virtual int CalculateVerticalScrollbarWidth() {
			bool isScrollBarVisible = CalculateVerticalScrollbarVisibility();
			if (isScrollBarVisible)
				return this.VerticalScrollBar.GetDefaultVerticalScrollBarWidth();
			return 0;
		}
		protected internal virtual int CalculateHorizontalScrollbarHeight() {
			bool isScrollBarVisible = CalculateHorizontalScrollbarVisibility();
			if (isScrollBarVisible)
				return CalculateHorizontalScrollbarHeightCore();
			return 0;
		}
		protected internal int CalculateHorizontalSplitContainerHeight() {
			bool isDesktopHorizontalScrollbarVisible = CalculateHorizontalScrollbarVisibility() && !IsTouchModeHorizontalScrollBar;
			bool isTabSelectorVisible = CalculateTabSelectorVisibility();
			if (isDesktopHorizontalScrollbarVisible || isTabSelectorVisible)
				return CalculateHorizontalScrollbarHeightCore();
			else
				return 0;
		}
		protected internal int CalculateHorizontalScrollbarHeightCore() {
			return this.HorizontalScrollBar.GetDefaultHorizontalScrollBarHeight();
		}
		IPlatformSpecificScrollBarAdapter ISpreadsheetControl.CreatePlatformSpecificScrollBarAdapter() {
			return new WinFormsScrollBarAdapter();
		}
		SpreadsheetViewVerticalScrollController ISpreadsheetControl.CreateSpreadsheetViewVerticalScrollController(SpreadsheetView view) {
			return new WinFormsSpreadsheetViewVerticalScrollController(view);
		}
		SpreadsheetViewHorizontalScrollController ISpreadsheetControl.CreateSpreadsheetViewHorizontalScrollController(SpreadsheetView view) {
			return new WinFormsSpreadsheetViewHorizontalScrollController(view);
		}
	}
}
namespace DevExpress.XtraSpreadsheet.Internal {
	public class WinFormsOfficeScrollbar : IOfficeScrollbar {
		readonly ScrollBarBase scrollbar;
		public WinFormsOfficeScrollbar(ScrollBarBase scrollbar) {
			Guard.ArgumentNotNull(scrollbar, "scrollbar");
			this.scrollbar = scrollbar;
		}
		public ScrollBarBase ScrollBar { get { return scrollbar; } }
		#region IOfficeScrollbar Members
		public int Value { get { return ScrollBar.Value; } set { ScrollBar.Value = value; } }
		public int Minimum { get { return ScrollBar.Minimum; } set { ScrollBar.Minimum = value; } }
		public int Maximum { get { return ScrollBar.Maximum; } set { ScrollBar.Maximum = value; } }
		public int LargeChange { get { return ScrollBar.LargeChange; } set { ScrollBar.LargeChange = value; } }
		public int SmallChange { get { return ScrollBar.SmallChange; } set { ScrollBar.SmallChange = value; } }
		public bool Enabled { get { return ScrollBar.Enabled; } set { ScrollBar.Enabled = value; } }
		public event ScrollEventHandler Scroll { add { ScrollBar.Scroll += value; } remove { ScrollBar.Scroll -= value; } }
		public void BeginUpdate() {
			ScrollBar.BeginUpdate();
		}
		public void EndUpdate() {
			ScrollBar.EndUpdate();
		}
		#endregion
	}
	public class WinFormsScrollBarAdapter : IPlatformSpecificScrollBarAdapter {
		public virtual void OnScroll(ScrollBarAdapter adapter, object sender, ScrollEventArgs e) {
			int delta = ((int)e.NewValue) - adapter.GetRawScrollBarValue();
			if (adapter.EnsureSynchronizedCore()) {
				ScrollEventArgs args = new ScrollEventArgs(e.Type, adapter.GetRawScrollBarValue(), adapter.GetRawScrollBarValue() + delta, e.ScrollOrientation);
				adapter.RaiseScroll(args);
				e.NewValue = args.NewValue;
			}
			else
				adapter.RaiseScroll(e);
		}
		public virtual void ApplyValuesToScrollBarCore(ScrollBarAdapter adapter) {
			if (adapter.Maximum > (long)int.MaxValue)
				adapter.Factor = 1.0 / (1 + (adapter.Maximum / (long)int.MaxValue));
			else
				adapter.Factor = 1.0;
			adapter.ScrollBar.BeginUpdate();
			try {
				adapter.ScrollBar.Minimum = (int)Math.Round(adapter.Factor * adapter.Minimum);
				adapter.ScrollBar.Maximum = (int)Math.Round(adapter.Factor * adapter.Maximum);
				adapter.ScrollBar.LargeChange = (int)Math.Round(adapter.Factor * adapter.LargeChange);
				adapter.ScrollBar.Value = (int)Math.Round(adapter.Factor * adapter.Value);
				adapter.ScrollBar.Enabled = adapter.Enabled;
			}
			finally {
				adapter.ScrollBar.EndUpdate();
			}
		}
		public int GetRawScrollBarValue(ScrollBarAdapter adapter) {
			return adapter.ScrollBar.Value;
		}
		public bool SetRawScrollBarValue(ScrollBarAdapter adapter, int value) {
			if (adapter.ScrollBar.Value != value) {
				adapter.ScrollBar.Value = value;
				adapter.Value = (long)Math.Round(value / adapter.Factor);
				return true;
			}
			else
				return false;
		}
		public virtual int GetPageUpRawScrollBarValue(ScrollBarAdapter adapter) {
			return Math.Max(adapter.ScrollBar.Minimum, adapter.ScrollBar.Value - adapter.ScrollBar.LargeChange);
		}
		public virtual int GetPageDownRawScrollBarValue(ScrollBarAdapter adapter) {
			return Math.Min(adapter.ScrollBar.Maximum - adapter.ScrollBar.LargeChange + 1, adapter.ScrollBar.Value + adapter.ScrollBar.LargeChange);
		}
		public virtual ScrollEventArgs CreateLastScrollEventArgs(ScrollBarAdapter adapter) {
			return new ScrollEventArgs(ScrollEventType.Last, adapter.ScrollBar.Maximum - adapter.ScrollBar.LargeChange + 1);
		}
	}
	public class WinFormsSpreadsheetViewVerticalScrollController : SpreadsheetViewVerticalScrollController {
		public WinFormsSpreadsheetViewVerticalScrollController(SpreadsheetView view)
			: base(view) {
		}
		protected override bool IsScrollTypeValid(ScrollEventArgs e) {
			return e.Type != ScrollEventType.EndScroll;
		}
		protected override int CalculateScrollDelta(ScrollEventArgs e) {
			return (int)(e.NewValue - ScrollBarAdapter.GetRawScrollBarValue());
		}
		protected override void ApplyNewScrollValue(int value) {
			ScrollBarAdapter.SetRawScrollBarValue(value);
		}
		protected override void ApplyNewScrollValueToScrollEventArgs(ScrollEventArgs e, int value) {
			e.NewValue = value;
		}
	}
	public class WinFormsSpreadsheetViewHorizontalScrollController : SpreadsheetViewHorizontalScrollController {
		public WinFormsSpreadsheetViewHorizontalScrollController(SpreadsheetView view)
			: base(view) {
		}
		protected override bool IsScrollTypeValid(ScrollEventArgs e) {
			return e.Type != ScrollEventType.EndScroll;
		}
		protected override int CalculateScrollDelta(ScrollEventArgs e) {
			return (int)(e.NewValue - ScrollBarAdapter.GetRawScrollBarValue());
		}
		protected override void ApplyNewScrollValue(int value) {
			ScrollBarAdapter.SetRawScrollBarValue(value);
		}
		protected override void ApplyNewScrollValueToScrollEventArgs(ScrollEventArgs e, int value) {
			e.NewValue = value;
		}
	}
}
