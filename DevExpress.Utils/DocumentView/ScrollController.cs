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
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting;
using DevExpress.DocumentView.Controls;
namespace DevExpress.DocumentView {
	public class ScrollController : DevExpress.Utils.Controls.OfficeScroller {
		const int ScrollRatio = 5;
		float scrollX;
		float scrollY;
		DocumentViewerBase printControl;
		DevExpress.XtraEditors.HScrollBar hScrollBar;
		DevExpress.XtraEditors.VScrollBar vScrollBar;
#if DEBUGTEST
		public DevExpress.XtraEditors.VScrollBar Test_VScrollBar {
			get { return vScrollBar; }
		}
#endif
		float Zoom {
			get { return printControl.Zoom; }
		}
		ViewManager ViewManager {
			get { return printControl.ViewManager; }
		}
		Size ScrollSize {
			get {
				return Size.Round(PSUnitConverter.DocToPixel(ViewManager.DocOffset, Zoom));
			}
		}
		IPopupForm PopupForm {
			get { return printControl.PopupForm; }
		}
		ViewControl ViewControl {
			get { return printControl.ViewControl; }
		}
		public ScrollController(DocumentViewerBase printControl, DevExpress.XtraEditors.HScrollBar hScrollBar, DevExpress.XtraEditors.VScrollBar vScrollBar) {
			this.printControl = printControl;
			this.hScrollBar = hScrollBar;
			this.vScrollBar = vScrollBar;
			hScrollBar.ValueChanged += new EventHandler(scrollBar_ValueChanged);
			hScrollBar.Scroll += new ScrollEventHandler(hScrollBar_Scroll);
			vScrollBar.ValueChanged += new EventHandler(scrollBar_ValueChanged);
			vScrollBar.Scroll += new ScrollEventHandler(vScrollBar_Scroll);
			ViewControl.MouseClick += new MouseEventHandler(ViewControl_MouseClick);
		}
		public override void Dispose() {
			if(hScrollBar != null) {
				hScrollBar.ValueChanged -= new EventHandler(scrollBar_ValueChanged);
				hScrollBar.Scroll -= new ScrollEventHandler(hScrollBar_Scroll);
				hScrollBar = null;
			}
			if(vScrollBar != null) {
				vScrollBar.ValueChanged -= new EventHandler(scrollBar_ValueChanged);
				vScrollBar.Scroll -= new ScrollEventHandler(vScrollBar_Scroll);
				vScrollBar = null;
			}
			if(ViewControl != null)
				ViewControl.MouseClick -= new MouseEventHandler(ViewControl_MouseClick);
			base.Dispose();
		}
		public void OnViewMouseWheel(MouseEventArgs e) {
			float scrollRatio = SystemInformation.MouseWheelScrollLines == -1 ?
				ScrollRatio :
				scrollRatio = (float)SystemInformation.MouseWheelScrollLines / 7;
			if(e.Delta > 0)
				scrollRatio *= -1;
			ViewManager.OffsetVertScroll(scrollRatio * scrollY);
			SetScrollBarValues();
		}
		public bool HandleKey(Keys key) {
			switch(key) {
				case Keys.Down:
					IncrementVertScroll();
					return true;
				case Keys.Up:
					DecrementVertScroll();
					return true;
				case Keys.Left:
					if(!AllowHScroll)
						return false;
					DecrementHorzScroll();
					return true;
				case Keys.Right:
					if(!AllowHScroll)
						return false;
					IncrementHorzScroll();
					return true;
			}
			return false;
		}
		public void LargeIncrementVertScroll() {
			OffsetVertScroll(ScrollRatio * scrollY);
		}
		public void LargeDecrementVertScroll() {
			OffsetVertScroll(-ScrollRatio * scrollY);
		}
		public void UpdateScrollBars() {
			try {
				allowSetScrollBarValueForCodedUI = false;
				Size viewSize = Size.Round(PSUnitConverter.DocToPixel(ViewManager.ViewSize, Zoom));
				vScrollBar.Maximum = Math.Max(ViewControl.Height, viewSize.Height);
				hScrollBar.Maximum = Math.Max(ViewControl.Width, viewSize.Width);
				hScrollBar.Minimum = 0;
				vScrollBar.Minimum = 0;
				hScrollBar.LargeChange = ViewControl.Width + 1;
				vScrollBar.LargeChange = ViewControl.Height + 1;
				hScrollBar.Enabled = hScrollBar.LargeChange <= hScrollBar.Maximum;
				vScrollBar.Enabled = vScrollBar.LargeChange <= vScrollBar.Maximum;
				vScrollBar.SmallChange = vScrollBar.LargeChange / ScrollRatio;
				hScrollBar.SmallChange = hScrollBar.LargeChange / ScrollRatio;
				SizeF size = PSUnitConverter.PixelToDoc(ViewControl.ClientSize, Zoom);
				scrollX = size.Width / ScrollRatio;
				scrollY = size.Height / ScrollRatio;
				SetScrollBarValues();
			}
			finally {
				allowSetScrollBarValueForCodedUI = true;
			}
		}
		protected override void OnVScroll(int delta) {
			ViewManager.OffsetVertScroll(delta * 50);
			SetScrollBarValues();
		}
		protected override void OnHScroll(int delta) {
			ViewManager.OffsetHorzScroll(delta * 50);
			SetScrollBarValues();
		}
		protected override bool AllowVScroll {
			get { return vScrollBar.Maximum > vScrollBar.LargeChange; }
		}
		protected override bool AllowHScroll {
			get { return hScrollBar.Maximum > hScrollBar.LargeChange; }
		}
		void SetScrollBarValues() {
			vScrollBar.Value = ScrollSize.Height;
			hScrollBar.Value = ScrollSize.Width;
		}
		bool allowSetScrollBarValueForCodedUI = true;
		void scrollBar_ValueChanged(object sender, System.EventArgs e) {
			if(allowSetScrollBarValueForCodedUI) {
				float vScrollVal = PSUnitConverter.PixelToDoc(vScrollBar.Value, Zoom);
				float hScrollVal = PSUnitConverter.PixelToDoc(hScrollBar.Value, Zoom);
				ViewManager.SetVertScroll(vScrollVal);
				ViewManager.SetHorizScroll(hScrollVal);
			}
			printControl.ViewControl.Invalidate();
		}
		void hScrollBar_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e) {
			SetHorizScroll(PSUnitConverter.PixelToDoc(e.NewValue, Zoom));
		}
		void vScrollBar_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e) {
			float scrollVal = PSUnitConverter.PixelToDoc(e.NewValue, Zoom);
			switch(e.Type) {
				case ScrollEventType.LargeIncrement:
					OffsetVertScroll(ScrollRatio * scrollY, e);
					break;
				case ScrollEventType.LargeDecrement:
					OffsetVertScroll(-ScrollRatio * scrollY, e);
					break;
				case ScrollEventType.SmallIncrement:
					OffsetVertScroll(scrollY, e);
					break;
				case ScrollEventType.SmallDecrement:
					OffsetVertScroll(-scrollY, e);
					break;
				case ScrollEventType.ThumbTrack:
					ShowScrollInfo(scrollVal);
					SetVertScroll(scrollVal);
					printControl.ViewControl.Invalidate();
					break;
				case ScrollEventType.ThumbPosition:
					SetVertScroll(scrollVal);
					ViewManager.ValidateVertScroll();
					e.NewValue = ScrollSize.Height;
					break;
				case ScrollEventType.EndScroll:
					PopupForm.HidePopup();
					break;
			}
		}
		protected virtual void SetVertScroll(float scrollVal){
			ViewManager.SetVertScroll(scrollVal);
		}
		protected virtual void SetHorizScroll(float scrollVal) {
			ViewManager.SetHorizScroll(scrollVal);
		}
		void ViewControl_MouseClick(object sender, MouseEventArgs e) {
			if(printControl.DocumentIsEmpty || e.Button != MouseButtons.Middle)
				return;
			Start(ViewControl);
		}
		void OffsetVertScroll(float dy, ScrollEventArgs e) {
			ViewManager.OffsetVertScroll(dy);
			e.NewValue = ScrollSize.Height;
		}
		void OffsetVertScroll(float dy) {
			ViewManager.OffsetVertScroll(dy);
			SetScrollBarValues();
		}
		void IncrementVertScroll() {
			OffsetVertScroll(scrollY);
		}
		void DecrementVertScroll() {
			OffsetVertScroll(-scrollY);
		}
		void OffsetHorzScroll(float dx) {
			ViewManager.OffsetHorzScroll(dx);
			SetScrollBarValues();
		}
		void IncrementHorzScroll() {
			OffsetHorzScroll(scrollX);
		}
		void DecrementHorzScroll() {
			OffsetHorzScroll(-scrollX);
		}
		protected virtual void ShowScrollInfo(float scrollVal) {			
			int pageNum = ViewManager.GetPageIndex(scrollVal) + 1;
			string pageCaption = PreviewLocalizer.GetString(PreviewStringId.ScrollingInfo_Page);
			string text = pageCaption + String.Format(": {0}", pageNum);
		   if(!PopupForm.IsPopupOwner(printControl)) {
				PopupForm.ShowText(text, Cursor.Position, MarginSide.Left, printControl);
			} else 
				PopupForm.ShowText(text);			
		}
	}
}
