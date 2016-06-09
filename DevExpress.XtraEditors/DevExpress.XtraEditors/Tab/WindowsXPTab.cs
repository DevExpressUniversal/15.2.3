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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraTab.ViewInfo;
using DevExpress.XtraTab.Drawing;
using DevExpress.XtraTab.Buttons;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using System.Drawing.Drawing2D;
namespace DevExpress.XtraTab.ViewInfo {
	public class WindowsXPTabControlViewInfo : BaseTabControlViewInfo {
		ObjectPainter tabPageClientPainter, tabPagePainter;
		public WindowsXPTabControlViewInfo(IXtraTab tabControl) : base(tabControl) {
			this.tabPageClientPainter = this.tabPagePainter = null;
			UpdatePainters();
		}
		public override bool IsAllowPageCustomBackColor { get { return true; } }
		public ObjectPainter TabPageClientPainter { get { return tabPageClientPainter; } }
		public ObjectPainter TabPagePainter { get { return tabPagePainter; } }
		protected virtual void UpdatePainters() {
			this.tabPageClientPainter = new XPTabPageClientPainter();
			this.tabPagePainter = new XPTabPagePainter();
		}
		protected override Rectangle CalcPageClientBounds() { 
			return TabPageClientPainter.GetObjectClientRectangle(new ObjectInfoArgs(null, PageBounds, ObjectState.Normal));
		}
	}
	public class WindowsXPTabHeaderViewInfo : BaseTabHeaderViewInfo { 
		public WindowsXPTabHeaderViewInfo(BaseTabControlViewInfo viewInfo) : base(viewInfo) {
		}
		protected override BaseTabPageViewInfo CreatePage(IXtraTabPage page) {
			return new WindowsXPTabPageViewInfo(page);
		}
		protected internal override EditorButtonPainter OnHeaderButtonGetPainter(TabButtonInfo button) {
			return EditorButtonHelper.GetWindowsXPPainter();
		}
		public virtual XPTabPageObjectInfoArgs UpdatePageInfo(BaseTabRowViewInfo row, BaseTabPageViewInfo pInfo) {
			WindowsXPTabPageViewInfo xpInfo = pInfo as WindowsXPTabPageViewInfo;
			XPTabPageObjectInfoArgs xp = xpInfo.UpdateInfo();
			if(IsSideLocation) 
				xp.Location = this.IsLeftLocation ? XPTabPageLocation.Left : XPTabPageLocation.Right;
			else 
				xp.Location = this.IsTopLocation ? XPTabPageLocation.Top : XPTabPageLocation.Bottom;
			xp.SetAppearance(pInfo.PaintAppearanceClient);
			int index = row.Pages.IndexOf(pInfo);
			xp.Position = XPTabPagePosition.Default;
			if(row.Pages.Count == 1) 
				xp.Position = XPTabPagePosition.Single;
			else {
				if(index == 0) xp.Position = XPTabPagePosition.Left;
				else {
					if(index == row.Pages.Count - 1) {
						if(pInfo.Bounds.Right >= Bounds.Right - 2)
							xp.Position = XPTabPagePosition.Right;
					}
				}
			}
			return xp;
		}
		public override bool DefaultShowHeaderFocus { get { return false; } }
		protected override int CalcUpDownGrowSize() {
			return 1;
		}
		protected override int CalcBorderSideSize() {
			return 1;
		}
	}
	public class WindowsXPTabPageViewInfo : BaseTabPageViewInfo {
		XPTabPageObjectInfoArgs infoArgs;
		public WindowsXPTabPageViewInfo(IXtraTabPage page) : base(page) {
			this.infoArgs = new XPTabPageObjectInfoArgs(null, Rectangle.Empty, null, ObjectState.Normal, XPTabPageLocation.Top);
		}
		public virtual XPTabPageObjectInfoArgs UpdateInfo() {
			this.infoArgs.Bounds = this.Bounds;
			this.infoArgs.State = this.PageState;
			return InfoArgs;
		}
		protected virtual XPTabPageObjectInfoArgs InfoArgs { get { return infoArgs; } }
	}
}
namespace DevExpress.XtraTab.Drawing {
	public class WindowsXPTabPainter : BaseTabPainter {
		public WindowsXPTabPainter(IXtraTab tabControl) : base(tabControl) {
		}
		protected override void DrawTabPage(TabDrawArgs e) {
			if(!IsNeedDrawRect(e, e.ViewInfo.PageBounds)) return;
			UpdateClipRegion(e.Graphics);
			WindowsXPTabControlViewInfo vi = e.ViewInfo as WindowsXPTabControlViewInfo;
			vi.TabPageClientPainter.DrawObject(new ObjectInfoArgs(e.Cache, e.ViewInfo.PageBounds, ObjectState.Normal));
		}
		public override void DrawPageClientControl(TabDrawArgs e, BaseTabPageViewInfo pageInfo) {
			WindowsXPTabControlViewInfo vi = e.ViewInfo as WindowsXPTabControlViewInfo;
			Rectangle bounds = GetPageClientDrawBounds(e, pageInfo);
			vi.TabPageClientPainter.DrawObject(new ObjectInfoArgs(e.Cache, bounds, ObjectState.Normal));
			pageInfo.PaintAppearanceClient.DrawBackground(e.Graphics, e.Cache, e.Bounds);
		}
		protected override void DrawHeaderPage(TabDrawArgs e, BaseTabRowViewInfo row, BaseTabPageViewInfo pInfo) {
			WindowsXPTabHeaderViewInfo header = e.ViewInfo.HeaderInfo as WindowsXPTabHeaderViewInfo;
			WindowsXPTabControlViewInfo vi = e.ViewInfo as WindowsXPTabControlViewInfo;
			XPTabPageObjectInfoArgs info = header.UpdatePageInfo(row, pInfo);
			info.Cache = e.Cache;
			vi.TabPagePainter.DrawObject(info);
			DrawHeaderPageImageText(e, pInfo);
			DrawHeaderFocus(e, pInfo);
		}
		protected override void UpdateClipRegion(Graphics g) {
		}
	}
}
