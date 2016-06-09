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
using DevExpress.Office.Internal;
using DevExpress.XtraRichEdit;
namespace DevExpress.XtraRichEdit.Internal.PrintLayout {
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
	public class WinFormsRichEditViewVerticalScrollController : RichEditViewVerticalScrollController {
		public WinFormsRichEditViewVerticalScrollController(RichEditView view)
			: base(view) {
		}
		protected internal override bool UpdatePageNumberOnScroll(ScrollEventArgs e) {
			return false;
		}
		protected internal override bool IsScrollTypeValid(ScrollEventArgs e) {
			return e.Type != ScrollEventType.EndScroll;
		}
		protected internal override int CalculateScrollDelta(ScrollEventArgs e) {
			return (int)(e.NewValue - ScrollBarAdapter.GetRawScrollBarValue());
		}
		protected internal override void ApplyNewScrollValue(int value) {
			ScrollBarAdapter.SetRawScrollBarValue(value);
		}
		protected internal override void ApplyNewScrollValueToScrollEventArgs(ScrollEventArgs e, int value) {
			e.NewValue = value;
		}
	}
	public class WinFormsRichEditViewHorizontalScrollController : RichEditViewHorizontalScrollController {
		public WinFormsRichEditViewHorizontalScrollController(RichEditView view)
			: base(view) {
		}
		protected internal override void OnScrollCore(ScrollEventArgs e) {
			ScrollBarAdapter.Value = (long)e.NewValue;
		}
	}
}
