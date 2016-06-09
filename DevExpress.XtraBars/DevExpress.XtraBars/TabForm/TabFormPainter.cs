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

using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Skins.XtraForm;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Text;
using System;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.XtraBars {
	public class TabFormPainter : FormPainter {
		public TabFormPainter(Control owner, ISkinProvider provider)
			: base(owner, provider) {
		}
		public TabForm TabForm { get { return Owner as TabForm; } }
		protected override void DrawCaption(GraphicsCache cache, bool doubleBuffer) { }
		public void DrawCaption(GraphicsCache cache) {
			base.DrawCaption(cache, false);
		}
		public override void DrawIcon(GraphicsCache cache) {
			if(TabForm.TabFormControl.ShowTabsInTitleBar == ShowTabsInTitleBar.True) return;
			if(TabForm.TabFormControl.IsRightToLeft) {
				Rectangle iconBounds = IconBounds;
				IconBounds = new Rectangle(-IconBounds.Right, IconBounds.Y, IconBounds.Width, IconBounds.Height);
				base.DrawIcon(cache);
				IconBounds = iconBounds;
				return;
			}
			base.DrawIcon(cache);
		}
		protected override void DrawText(GraphicsCache cache) {
			if(TabForm.TabFormControl.ShowTabsInTitleBar == ShowTabsInTitleBar.True) return;
			if(TabForm.TabFormControl.IsRightToLeft) {
				Rectangle textBounds = TextBounds;
				TextBounds = new Rectangle(-GetTextRight(cache.Graphics), TextBounds.Y, TextBounds.Width, TextBounds.Height);
				base.DrawText(cache);
				TextBounds = textBounds;
				return;
			}
			base.DrawText(cache);
		}
		protected override Rectangle CalcButtonsBounds(Rectangle captionClient) {
			captionClient = CorrectButtonsPosition(captionClient);
			return base.CalcButtonsBounds(captionClient);
		}
		protected Rectangle CorrectButtonsPosition(Rectangle captionClient) {
			UserLookAndFeel lookAndFeel = GetActiveLookAndFeel();
			if(lookAndFeel == null) return captionClient;
			string name = lookAndFeel.SkinName;
			if(name.Contains("Office 2010") || name.Equals("Caramel") || name.Equals("The Asphalt World") ||
				name.Contains("Metropolis") || name.Equals("Valentine") || name.Equals("Glass Oceans"))
					captionClient.Width--;
			if(string.Equals(name, "London Liquid Sky")) captionClient.Width -= 2;
			return captionClient;
		}
		protected UserLookAndFeel GetActiveLookAndFeel() {
			if(XtraFormOwner.LookAndFeel == null)
				return null;
			return XtraFormOwner.LookAndFeel.ActiveLookAndFeel;
		}
		public override SkinPaddingEdges Margins {
			get {
				SkinPaddingEdges margins = base.Margins;
				margins.Top = 0;
				return margins;
			}
		}
		int CaptionHeight {
			get {
				if(TabForm == null || TabForm.TabFormControl == null)
					return 0;
				return TabForm.TabFormControl.ViewInfo.GetCaptionHeight();
			}
		}
		internal int GetTextRight(Graphics g) {
			int textWidth = CalcBestTextSize(g, Text, TextBounds).Width;
			return TextBounds.X + textWidth;
		}
		internal int GetButtonsLeft() {
			return Buttons.ButtonsBounds.X;
		}
		internal AppearanceObject GetCaptionAppearance() {
			return GetTextAppearance();
		}
		internal Size CalcBestTextSize(Graphics g, string text, Rectangle textBounds) {
			AppearanceObject appearance = GetTextAppearance();
			if(AllowHtmlDraw)
				return StringPainter.Default.Calculate(g, appearance, null, text, textBounds, null, HtmlContext).Bounds.Size;
			return appearance.CalcTextSizeInt(g, text, 0);
		}
		protected override Rectangle CaptionBounds {
			get {
				if(IsWindowMinimized) return new Rectangle(0, 0, FormBounds.Width, Math.Min(CaptionHeight, FormBounds.Height));
				return new Rectangle(0, 0, FormBounds.Width, CaptionHeight);
			}
		}
		protected int GetContentMarginTop() {
			if(!TabForm.IsMaximized)
				return 0;
			return ZoomedInvisibleHeight;
		}
		protected internal int ZoomedInvisibleHeight {
			get {
				Screen sc = Screen.FromRectangle(Owner.Bounds);
				return sc.WorkingArea.Y - Owner.Bounds.Y;
			}
		}
		protected override bool ShouldResetTopMargin { get { return true; } }
		int GetInt(IntPtr ptr) {
			return (IntPtr.Size == 8) ? (int)(ptr.ToInt64() & 0xFFFFFFFFL) : ptr.ToInt32();
		}
		bool IsCaptionButtonHitTest(ref Message msg) {
			int res = GetInt(msg.Result);
			return res == NativeMethods.HT.HTCLOSE || res == NativeMethods.HT.HTMAXBUTTON || res == NativeMethods.HT.HTMINBUTTON;
		}
		protected void NCHitTest(ref Message msg) {
			base.DoWndProc(ref msg);
			Point p = PointToFormBounds(new Point(GetInt(msg.LParam)));
			if(IsCaptionButtonHitTest(ref msg)) {
				if(NativeVista.IsWindows7) {
					int buttonBottom = SystemInformation.CaptionButtonSize.Height;
					if(XtraFormOwner.WindowState == FormWindowState.Maximized && XtraFormOwner.Bounds.Top < 0)
						buttonBottom -= XtraFormOwner.Bounds.Top;
					if(p.Y >= buttonBottom) {
						msg.Result = new IntPtr(NativeMethods.HT.HTCAPTION);
					}
				}
			}
			switch(GetInt(msg.Result)) {
				case NativeMethods.HT.HTCAPTION:
				case NativeMethods.HT.HTSYSMENU:
				case NativeMethods.HT.HTCLIENT:
					break;
				default: return;
			}
			msg.Result = new IntPtr(WMNCHitTestResize(p, GetInt(msg.Result)));
			if(GetInt(msg.Result) == NativeMethods.HT.HTCLIENT) {
				if(CaptionBounds.Contains(p)) {
					if(IconBounds.Contains(p))
						msg.Result = new IntPtr(NativeMethods.HT.HTMENU);
					else
						msg.Result = new IntPtr(NativeMethods.HT.HTCAPTION);
				}
			}
		}
		public override bool DoWndProc(ref Message msg) {
			switch(msg.Msg) {
				case MSG.WM_NCCALCSIZE:
					NCCalcSize(ref msg);
					return true;
				case MSG.WM_NCHITTEST:
					NCHitTest(ref msg);
					return true;
			}
			bool res = base.DoWndProc(ref msg);
			switch(msg.Msg) {
				case MSG.WM_NCMOUSEMOVE:
				case MSG.WM_NCMOUSELEAVE:
				case MSG.WM_NCLBUTTONDOWN:
				case MSG.WM_NCLBUTTONUP:
					InvalidateCaption();
					break;
			}
			return res;
		}
		protected override Rectangle GetCaptionClient(Rectangle captionBounds) {
			captionBounds.Height = GetXtraFormCaptionHeight();
			captionBounds.Y = GetContentMarginTop();
			return base.GetCaptionClient(captionBounds);
		}
		internal Rectangle GetCaptionClientBounds() {
			Rectangle rect = CaptionBounds;
			rect.Height = GetXtraFormCaptionHeight();
			return rect;
		}
		internal int GetXtraFormCaptionHeight() {
			return base.GetCaptionHeight();
		}
		protected override void DrawFrameCore(GraphicsCache cache, SkinElementInfo info, FrameKind kind) {
			base.DrawFrameCore(cache, info, kind);
			Rectangle formBounds = FormBounds;
			DrawCaptionBackground(cache, formBounds);
		}
		protected virtual void DrawCaptionBackground(GraphicsCache cache, Rectangle formBounds) {
			Rectangle rect = new Rectangle(0, 0, formBounds.Width, CaptionHeight);
			SkinElementInfo siCaption = new SkinElementInfo(GetSkinCaption(), rect);
			siCaption.ImageIndex = IsWindowActive ? 0 : 1;
			if(TabForm.TabFormControl != null) {
				siCaption.CorrectImageFormRTL = TabForm.TabFormControl.IsRightToLeft;
			}
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, siCaption);
		}
		protected internal SkinPaddingEdges GetZoomedMarginsCore() {
			return GetZoomedMargins();
		}
		protected override void OnWindowActiveChanged() {
			base.OnWindowActiveChanged();
			InvalidateCaption();
		}
		protected virtual void InvalidateCaption() {
			if(TabForm != null && TabForm.TabFormControl != null)
				TabForm.TabFormControl.Invalidate();
		}
	}
}
