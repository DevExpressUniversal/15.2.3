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
using System.Collections.Generic;
using System.Diagnostics;
using DevExpress.Utils;
using DevExpress.Office.Internal;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Model;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Windows.Forms;
namespace DevExpress.XtraSpreadsheet.Internal {
	#region VerticalScrollBarAdapter
	public class VerticalScrollBarAdapter : ScrollBarAdapter {
		public VerticalScrollBarAdapter(IOfficeScrollbar scrollBar, IPlatformSpecificScrollBarAdapter adapter)
			: base(scrollBar, adapter) {
		}
		protected override bool DeferredScrollBarUpdate { get { return false; } }
		protected override bool Synchronized { get { return true; } set { } }
	}
	#endregion
	#region HorizontalScrollBarAdapter
	public class HorizontalScrollBarAdapter : ScrollBarAdapter {
		public HorizontalScrollBarAdapter(IOfficeScrollbar scrollBar, IPlatformSpecificScrollBarAdapter adapter)
			: base(scrollBar, adapter) {
		}
		protected override bool DeferredScrollBarUpdate { get { return false; } }
		protected override bool Synchronized { get { return true; } set { } }
	}
	#endregion
	#region SpreadsheetViewScrollControllerBase (abstract class)
	public abstract partial class SpreadsheetViewScrollControllerBase : OfficeScrollControllerBase {
		#region Fields
		readonly SpreadsheetView view;
		#endregion
		protected SpreadsheetViewScrollControllerBase(SpreadsheetView view) {
			Guard.ArgumentNotNull(view, "view");
			this.view = view;
			Initialize();
		}
		#region Properties
		public SpreadsheetView View { get { return view; } }
		#endregion
		public override void UpdateScrollBar() {
			if (ScrollBar == null)
				return;
			if (!Object.ReferenceEquals(View, View.Control.InnerControl.ActiveView))
				return;
			Debug.Assert(Object.ReferenceEquals(View, View.Control.InnerControl.ActiveView));
			base.UpdateScrollBar();
		}
	}
	#endregion
	#region SpreadsheetViewVerticalScrollController (abstract class)
	public abstract class SpreadsheetViewVerticalScrollController : SpreadsheetViewScrollControllerBase {
		protected SpreadsheetViewVerticalScrollController(SpreadsheetView view)
			: base(view) {
		}
		public override IOfficeScrollbar ScrollBar {
			get {
				if (View.Control == null)
					return null;
				if (View.Control.InnerControl == null)
					return null;
				return View.Control.InnerControl.VerticalScrollBar;
			}
		}
		protected override ScrollBarAdapter CreateScrollBarAdapter() {
			return new VerticalScrollBarAdapter(ScrollBar, View.Control.CreatePlatformSpecificScrollBarAdapter());
		}
		public long GetTopInvisibleHeight() {
			return ScrollBarAdapter.Value;
		}
		protected internal abstract bool IsScrollTypeValid(ScrollEventArgs e);
		protected override void OnScroll(object sender, ScrollEventArgs e) {
			if (!IsScrollTypeValid(e))
				return;
			int delta = CalculateScrollDelta(e);
			int value = (int)e.NewValue;
#if !SL
			if (delta == 0 && e.Type == ScrollEventType.SmallIncrement) {
				delta = 1;
				value = e.OldValue + 1;
			}
#endif
			if (delta != 0) {
				ApplyNewScrollValue(value);
				View.OnVerticalScroll(delta);
				ApplyNewScrollValueToScrollEventArgs(e, value);
			}
		}
		protected internal abstract int CalculateScrollDelta(ScrollEventArgs e);
		protected internal abstract void ApplyNewScrollValue(int value);
		protected internal abstract void ApplyNewScrollValueToScrollEventArgs(ScrollEventArgs e, int value);
		protected internal virtual int ScrollLineDown() {
			return ScrollLineUpDown(1);
		}
		protected internal virtual int ScrollLineUpDown(int rowOffset) {
			Worksheet sheet = View.Control.InnerControl.DocumentModel.ActiveSheet;
			ScrollBarAdapter.BeginUpdate();
			try {
				if (ScrollBarAdapter.Value + rowOffset + ScrollBarAdapter.LargeChange > ScrollBarAdapter.Maximum)
					ScrollBarAdapter.Maximum = Math.Min(ScrollBarAdapter.Maximum + rowOffset, sheet.MaxRowCount - 1);
				ScrollBarAdapter.Value += rowOffset;
			}
			finally {
				ScrollBarAdapter.EndUpdate();
			}
			return ScrollBarAdapter.GetRawScrollBarValue();
		}
		protected override void UpdateScrollBarAdapter() {
			DocumentLayout documentLayout = View.Control.InnerControl.InnerDocumentLayout;
			if (documentLayout == null)
				return;
			Worksheet sheet = documentLayout.DocumentModel.ActiveSheet;
			ModelWorksheetView sheetView = sheet.ActiveView;
			ScrollBarAdapter.BeginUpdate();
			try {
				ScrollBarAdapter.Minimum = 0;
				ScrollBarAdapter.Maximum = documentLayout.ScrollInfo.MaximumRow;
				ScrollBarAdapter.LargeChange = documentLayout.ScrollInfo.LargeChangeRow;
				ScrollBarAdapter.Value = documentLayout.ScrollInfo.ScrollRowIndex;
			}
			finally {
				ScrollBarAdapter.EndUpdate();
			}
		}
	}
	#endregion
	#region SpreadsheetViewHorizontalScrollController (abstract class)
	public abstract class SpreadsheetViewHorizontalScrollController : SpreadsheetViewScrollControllerBase {
		protected SpreadsheetViewHorizontalScrollController(SpreadsheetView view)
			: base(view) {
		}
		public override IOfficeScrollbar ScrollBar {
			get {
				if (View.Control == null)
					return null;
				if (View.Control.InnerControl == null)
					return null;
				return View.Control.InnerControl.HorizontalScrollBar;
			}
		}
		protected override ScrollBarAdapter CreateScrollBarAdapter() {
			return new HorizontalScrollBarAdapter(ScrollBar, View.Control.CreatePlatformSpecificScrollBarAdapter());
		}
		protected internal abstract bool IsScrollTypeValid(ScrollEventArgs e);
		protected override void OnScroll(object sender, ScrollEventArgs e) {
			if (!IsScrollTypeValid(e))
				return;
			int delta = CalculateScrollDelta(e);
			int value = (int)e.NewValue;
#if !SL
			if (delta == 0 && e.Type == ScrollEventType.SmallIncrement) {
				delta = 1;
				value = e.OldValue + 1;
			}
#endif
			if (delta != 0) {
				ApplyNewScrollValue(value);
				View.OnHorizontalScroll(delta);
				ApplyNewScrollValueToScrollEventArgs(e, value);
			}
		}		
		protected internal abstract int CalculateScrollDelta(ScrollEventArgs e);
		protected internal abstract void ApplyNewScrollValue(int value);
		protected internal abstract void ApplyNewScrollValueToScrollEventArgs(ScrollEventArgs e, int value);
		protected override void UpdateScrollBarAdapter() {
			DocumentLayout documentLayout = View.Control.InnerControl.InnerDocumentLayout;
			if (documentLayout == null)
				return;
			Worksheet sheet = documentLayout.DocumentModel.ActiveSheet;
			ModelWorksheetView sheetView = sheet.ActiveView;
			ScrollBarAdapter.BeginUpdate();
			try {
				ScrollBarAdapter.Minimum = 0;
				ScrollBarAdapter.Maximum = documentLayout.ScrollInfo.MaximumColumn;
				ScrollBarAdapter.LargeChange = documentLayout.ScrollInfo.LargeChangeColumn;
				ScrollBarAdapter.Value = documentLayout.ScrollInfo.ScrollColumnIndex;
			}
			finally {
				ScrollBarAdapter.EndUpdate();
			}
		}
		protected internal virtual int ScrollLineLeftRight(int columnOffset) {
			Worksheet sheet = View.Control.InnerControl.DocumentModel.ActiveSheet;
			ScrollBarAdapter.BeginUpdate();
			try {
				if (ScrollBarAdapter.Value + columnOffset + ScrollBarAdapter.LargeChange > ScrollBarAdapter.Maximum)
					ScrollBarAdapter.Maximum = Math.Min(ScrollBarAdapter.Maximum + columnOffset, sheet.MaxColumnCount - 1);
				ScrollBarAdapter.Value += columnOffset;
			}
			finally {
				ScrollBarAdapter.EndUpdate();
			}
			return ScrollBarAdapter.GetRawScrollBarValue();
		}
	}
	#endregion
}
