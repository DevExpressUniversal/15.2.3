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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.Forms;
using DevExpress.XtraBars.InternalItems;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraBars.Accessible;
namespace DevExpress.XtraBars.Controls {
	public class CustomBarControl : CustomLinksControl {
		Bar bar;
		internal CustomBarControl(BarManager manager, Bar bar)
			: base(manager, null) {
			this.bar = bar;
			UpdateVisibleLinks();
			ControlLinks.UpdateLinkedObject(Bar);
		}
		protected override void OnEnabledChanged(EventArgs e) {
			base.OnEnabledChanged(e);
			UpdateViewInfo();
			Invalidate();
		}
		protected override void RemoveGhostLinks() {
			Bar.MakeVisibleList();
		}
		protected override DevExpress.Accessibility.BaseAccessible CreateAccessibleInstance() {
			return new DevExpress.XtraBars.Accessible.ToolbarAccessible(Bar);
		}
		protected override void OnRightToLeftChanged(EventArgs e) {
		}
		protected override bool HasGhostLinks() {
			if(base.HasGhostLinks())
				return true;
			foreach(BarItemLink link in ViewInfo.ReallyVisibleLinks) {
				if(link.Manager == null || link.Item == null)
					return true;
			}
			return false;
		}
		protected internal override void CheckDirty() {
			if(Bar != null) {
				if(Bar.IsLinksDirty) Bar.MakeVisibleList();
				if(Bar.IsVisualDirty) {
					if(ViewInfo != null) ViewInfo.ClearReady();
					Bar.RemoveVisualDirty();
				}
			}
			base.CheckDirty();
		}
		public override void LayoutChanged() {
			if(Destroying || LockLayout != 0) return;
			if(Bar != null) Bar.SetVisualDirty();
			Invalidate();
		}
		protected override bool CanHideLink(BarItemLink link) {
			if(Bar == null || Bar.IsMerged) return false;
			if(link.Item != null && link.Item.Manager != Manager) {
				if(Manager.IsHideItemsFromMergedManager()) return true;
			}
			return false;
		}
		internal override BarItemLinkReadOnlyCollection GetLinks() { return VisibleLinks; }
		public override Color BackColor {
			get {
				if(Bar != null && !Bar.OptionsBar.DrawBorder)
					return Color.Transparent;
				return base.BackColor;
			}
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			if(Bar != null && !Bar.Visible && Manager.IsDesignMode)
				e.Graphics.FillRectangle(DevExpress.Utils.Drawing.ResourceCache.DefaultCache.GetSolidBrush(Color.FromArgb(50, Color.Gray)), ClientRectangle);
		}
		protected override bool IsAllowHash { get { return true; } }
		protected override LinksNavigation CreateLinksNavigator() {
			return new HorizontalLinksNavigation(this);
		}
		protected internal override void ExcludeRarelyItemFromVisible(BarItemLinkReadOnlyCollection links) {
			if(Bar == null || Bar.IsStatusBar || Manager.MostRecentItemsPercent == 100) {
				for(int n = links.Count - 1; n >= 0; n--) {
					BarItemLink link = (BarItemLink)links[n];
					if(link.Item is BarQBarCustomizationItem) continue;
					if(link.Item is BarDesignTimeItem) continue;
					links.RemoveItemAt(n);
					break;
				}
				return;
			}
			base.ExcludeRarelyItemFromVisible(links);
		}
		public override bool ShouldCloseMenuOnClick(MouseInfoArgs e, Control child) {
			if(Destroying) return true;
			if(e.MouseUp) return false;
			BarItemLink link = GetLinkByPoint(e.ScreenPoint);
			if(link == null) return true;
			if(Manager.SelectionInfo.HighlightedLink == null || VisibleLinks.Contains(Manager.SelectionInfo.HighlightedLink)) return false;
			foreach(IPopup popup in Manager.SelectionInfo.OpenedPopups) {
				if(popup.OwnerLink == link) return false;
			}
			return true;
		}
		public virtual Bar Bar { get { return bar; } }
		public virtual bool IsRequireMouseDown(MouseEventArgs e) {
			BarItemLink link = null;
			if(e.Button == MouseButtons.Left || (Manager.IsCustomizing && e.Button == MouseButtons.Right))
				link = GetLinkByPoint(Cursor.Position);
			return link != null;
		}
		protected internal override void UpdateVisibleLinks() {
			base.UpdateVisibleLinks();
			UpdateBarVisibleLinks();
		}
		protected virtual void UpdateBarVisibleLinks() {
			if(Bar == null) return;
			AddBarVisibleLinks(VisibleLinks, Bar.VisibleLinks);
			BarItemLink designLink = ControlLinks[CustomLinksControl.ControlLink.DesignTimeLink];
			if(designLink != null && Manager.Helper.CustomizationManager.CanAddDesignTimeLink(Bar)) {
				VisibleLinks.AddItem(designLink);
			}
			if(Bar.IsMainMenu) {
				if(Manager.Helper.MdiHelper.IsShowSystemMenu) {
					Manager.Helper.MdiHelper.AddSystemLinks(bar, VisibleLinks);
				}
				else {
					if(Bar.IsMainMenu && Manager.ShowCloseButton) {
						VisibleLinks.AddItem(ControlLinks[CustomLinksControl.ControlLink.CloseLink]);
					}
				}
			}
			SetLinksOwner(this);
		}
		public override bool IsMultiLine { get { return Bar != null && Bar.OptionsBar.MultiLine; } }
		public override bool IsVertical {
			get {
				if(Bar == null) return false;
				if(Bar.StandaloneBarDockControl != null) return Bar.StandaloneBarDockControl.IsVertical; 
				return (Bar.DockStyle == BarDockStyle.Left || Bar.DockStyle == BarDockStyle.Right);
			}
		}
		public virtual new BarControlViewInfo ViewInfo { get { return base.ViewInfo as BarControlViewInfo; } }
		public override BarItemLinkReadOnlyCollection GetVisibleLinks() {
			return ViewInfo == null ? VisibleLinks : ViewInfo.ReallyVisibleLinks;
		}
		protected virtual void ToggleBarState() {
			if(Bar.OptionsBar.BarState == BarState.Expanded)
				Bar.OptionsBar.BarState = BarState.Collapsed;
			else
				Bar.OptionsBar.BarState = BarState.Expanded;
		}
		protected override void OnLocationChanged(EventArgs e) {
			UpdateVisualEffects(DevExpress.Utils.VisualEffects.UpdateAction.BeginUpdate);
			base.OnLocationChanged(e);
			UpdateVisualEffects(DevExpress.Utils.VisualEffects.UpdateAction.EndUpdate);
		}
		Cursor prevCursor = null;
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if(ViewInfo.DragBorder.Contains(e.Location)) {
				if(prevCursor == null) {
				prevCursor = Cursor;
				Cursor = Cursors.SizeAll;
			}
			}
			else {
				if(prevCursor != null)
					Cursor = prevCursor;
				prevCursor = null;
			}
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			Cursor = null;
			prevCursor = null;
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			if(ee.Handled) {
				base.OnMouseDown(ee);
				return;
			}
			CheckDirty();
			BarItemLink link = GetLinkByPoint(Control.MousePosition);
			if(e.Clicks == 2) {
				if(!Manager.IsDesignMode) {
					if(link == null) {
						if(Bar.DockStyle == BarDockStyle.None) {
							Bar.SwitchDockStyle();
						}
						else {
							ToggleBarState();
						}
						ee.Handled = true;
						return;
					}
				}
			}
			else {
				if(e.Button == MouseButtons.Left) {
					if(Bar != null && (link == null || link.Item is BarEmptyItem || link.Item is BarQBarCustomizationItem) && Manager.IsDesignMode)
						Manager.Helper.CustomizationManager.SelectObject(Bar);
					if(Bar != null && link == null && Bar.CanMove) {
						Manager.SelectionInfo.CloseAllPopups();
						Bar.StartMoving(this);
						ee.Handled = true;
						return;
					}
				}
			}
			base.OnMouseDown(ee);
		}
		protected internal virtual void ResetChildrenAccessible() {
			if(HasAccessible)
				((ToolbarAccessible)DXAccessible).ResetChildrenAccessible();
		}
	}
	[ToolboxItem(false)]
	public class DockedBarControl : CustomBarControl {
		BarQBarCustomizationItemLink quickCustomizationLink;
		public DockedBarControl(BarManager barManager, Bar bar)
			: base(barManager, bar) {
			this.quickCustomizationLink = null;
			CausesValidation = false;
		}
		protected override bool HasInvalidLinks {
			get {
				foreach(BarControlRowViewInfo rowInfo in ((DockedBarControlViewInfo)ViewInfo).Rows) {
					foreach(BarLinkViewInfo linkInfo in rowInfo.Links) {
						if(linkInfo.Link == null || linkInfo.Link.Item == null || linkInfo.Link.Manager == null)
							return true;
					}
				}
				return false;
			}
		}
		public override Color BackColor { get { return ViewInfo != null && ViewInfo.DrawTransparent ? Color.Transparent : base.BackColor; } }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(CanDispose) {
					RemoveQuickCustomizationLink();
				}
			}
			base.Dispose(disposing);
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
		}
		protected internal override void CheckDirty() {
			if(DockControl != null && Bar.IsVisualDirty) {
				DockControl.OnDockableChanged(Bar);
			}
			base.CheckDirty();
		}
		protected BarDockControl DockControl { get { return Bar == null ? null : Bar.DockControl; } }
		public override bool RotateWhenVertical { get { return IsVertical && Bar != null && Bar.OptionsBar.RotateWhenVertical; } }
		public override bool IsCanSpringLinks { get { return !IsMultiLine && Bar != null; } }
		public BarQBarCustomizationItemLink AddQuickCustomizationLink() {
			if(Bar == null) return null;
			if(QuickCustomizationLink != null) return QuickCustomizationLink;
			if(quickCustomizationLink == null) quickCustomizationLink = Manager.InternalItems.QuickCustomizationItem.CreateLink(VisibleLinks, Bar) as BarQBarCustomizationItemLink;
			quickCustomizationLink.linkedObject = Bar;
			VisibleLinks.AddItem(quickCustomizationLink);
			if(ViewInfo != null) ViewInfo.ReallyVisibleLinks.AddItem(quickCustomizationLink);
			if(quickCustomizationLink.SubControl != null && !Bar.OptionsBar.UseWholeRow && Manager.DrawParameters.Constants.AllowLinkShadows) {
				quickCustomizationLink.SubControl.Form.CreateShadowsInternal();
				quickCustomizationLink.SubControl.Form.LayoutChanged();
			}
			return quickCustomizationLink;
		}
		public void RemoveQuickCustomizationLink() {
			if(QuickCustomizationLink == null) return;
			VisibleLinks.RemoveItem(QuickCustomizationLink);
			if(this.quickCustomizationLink != null) {
				this.quickCustomizationLink.Dispose();
			}
			this.quickCustomizationLink = null;
		}
		public BarQBarCustomizationItemLink QuickCustomizationLink {
			get {
				if(VisibleLinks.Count > 0) {
					BarItemLink link = VisibleLinks[VisibleLinks.Count - 1];
					if(link is BarQBarCustomizationItemLink) return link as BarQBarCustomizationItemLink;
				}
				return null;
			}
		}
		const int WM_NCHITTEST = 0x0084, HTBOTTOMRIGHT = 17, WM_SYSCOMMAND = 0x0112, SC_SIZE = 0xF000, SC_SIZE2 = 0xF008;
		protected override void WndProc(ref Message msg) {
			if(Manager != null && !Manager.IsDestroying) {
				if(msg.Msg == WM_NCHITTEST && IsSizeableBar) {
					Rectangle r = Bar.BarControl.ViewInfo.SizeGrip;
					Point p = WinAPIHelper.GetPoint(msg.LParam);
					if(!r.IsEmpty && r.Contains(PointToClient(p))) {
						msg.Result = new IntPtr(HTBOTTOMRIGHT);
						return;
					}
				}
				if(msg.Msg == WM_SYSCOMMAND) {
					int wp = WinAPIHelper.GetInt(msg.WParam) & 0xFFF0;
					if((wp == SC_SIZE || wp == SC_SIZE2) && IsSizeableBar) {
						BarNativeMethods.SendMessage(this.FindForm().Handle, msg.Msg, msg.WParam, msg.LParam);
						msg.Result = new IntPtr(1);
						return;
					}
				}
				if(msg.Msg == DevExpress.Utils.Drawing.Helpers.MSG.WM_MOUSEACTIVATE) {
					Form topLevelForm = Manager.GetForm();
					if(topLevelForm != null && msg.WParam == topLevelForm.Handle) {
						var controller = Manager.GetController();
						if(controller != null) {
							var link = GetLinkByPoint(Control.MousePosition, true);
							if(link != null && link.Enabled) {
								if(controller.NotifyBarMouseActivateClients(Manager, ref msg))
									return;
							}
						}
					}
				}
			}
			base.WndProc(ref msg);
		}
		bool IsSizeableBar { get { return Bar != null && Bar.IsStatusBar && Bar.BarControl != null && Bar.BarControl.ViewInfo != null; } }
	}
	[ToolboxItem(false)]
	public class FloatingBarControl : CustomBarControl, IFormContainedControl {
		ControlForm form;
		protected internal FloatingBarControl(BarManager barManager, Bar bar)
			: base(barManager, bar) {
			this.form = null;
			this.Visible = true;
		}
		public override Color BackColor {
			get {
				return Color.Transparent;
			}
		}
		protected internal override void CheckDirty() {
			bool isDirty = Bar.IsVisualDirty;
			if(isDirty) {
				if(ViewInfo != null) ViewInfo.ClearHash();
			}
			base.CheckDirty();
			if(isDirty) Form.LayoutChanged();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(!CanDispose) {
					Form.Visible = false;
					return;
				}
				this.Controls.Remove(this);
				Form form = this.Form;
				if(Form != null && !Form.Disposing) form.Dispose();
				this.form = null;
			}
			base.Dispose(disposing);
		}
		public virtual void UpdateZ() {
			FloatingBarControlForm fb = Form as FloatingBarControlForm;
			if(fb != null) {
				fb.UpdateZ();
			}
		}
		public virtual ControlForm Form { get { return form; } set { this.form = value; } }
		public virtual Point FloatLocation { get { return Bar == null ? Point.Empty : Bar.FloatLocation; } }
		protected override void OnSchemeChanged() {
			if(Form != null) Form.UpdateScheme();
			base.OnSchemeChanged();
		}
		Size IFormContainedControl.CalcSize(int width, int maxHeight) {
			if(width == -1) {
				if(Bar != null) width = Bar.GetFloatWidht();
			}
			return CalcSize(width);
		}
		Size IFormContainedControl.FormMimimumSize { get { return Size.Empty; } }
		void IFormContainedControl.SetParentForm(ControlForm form) { this.Form = form; }
		void IFormContainedControl.CalcViewInfo() {
			CheckDirty();
		}
		public override void DoSetVisible(bool newVisible) {
			if(newVisible) {
				Form.Visible = true;
			}
			else
				Form.Visible = newVisible;
			UpdateZ();
		}
	}
}
