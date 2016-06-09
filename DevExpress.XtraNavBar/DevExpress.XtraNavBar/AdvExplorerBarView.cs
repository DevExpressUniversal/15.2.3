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
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraNavBar;
using DevExpress.Utils.Drawing;
using System.Collections;
using DevExpress.Utils.Text;
namespace DevExpress.XtraNavBar.ViewInfo {
	public class AdvExplorerBarNavBarViewInfo : ExplorerBarNavBarViewInfo {
		public AdvExplorerBarNavBarViewInfo(NavBarControl navBar) : base(navBar) {
		}
		protected override void DrawBackgroundCore(GraphicsCache e, CustomDrawObjectEventArgs custom, Rectangle r) {
			custom.Appearance.FillRectangle(e, r);
		}
	}
	public class AdvExplorerBarNavLinkPainter : ExplorerBarNavLinkPainter {
		public AdvExplorerBarNavLinkPainter(NavBarControl navBar) : base(navBar) {
		}
		protected override void DrawLinkImageBorder(ObjectInfoArgs e, Rectangle r) {
		}
		protected override Brush GetLinkCaptionBrush(ObjectInfoArgs e) { 
			NavBarItemLink link = GetLink(e);
			AppearanceObject appearance = GetLinkAppearance(e);
			return ExplorerBarColorHelper.CalcTextBrush(e, appearance.GetForeColor(), link.Enabled);
		}
	}
	public class AdvExplorerBarNavGroupClientPainter : ExplorerBarNavGroupClientPainter {
		public AdvExplorerBarNavGroupClientPainter(BaseNavGroupPainter groupPainter) : base(groupPainter) { }
		protected override void DrawBorder(NavGroupClientInfoArgs e) {
			DrawBorder(e, SystemBrushes.Window);
		}
	}
	public class AdvExplorerBarNavGroupPainter : ExplorerBarNavGroupPainter {
		public AdvExplorerBarNavGroupPainter(NavBarControl navBar) : base(navBar) {	}
		protected override ObjectPainter CreateOpenCloseButtonPainter() { return new AdvExplorerBarOpenCloseButtonObjectPainter(); }
		protected override BaseNavGroupClientPainter CreateGroupClientPainter() { 
			return new AdvExplorerBarNavGroupClientPainter(this); 
		}
		protected virtual StyleObjectInfoArgs CreateXPArgs(ObjectInfoArgs e) {
			StyleObjectInfoArgs args = new StyleObjectInfoArgs(e.Cache, e.Bounds, GetGroupAppearance(e), e.State);
			return args;
		}
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			Rectangle r = base.CalcObjectBounds(e);
			r.Height += 2;
			return r;
		}
		protected virtual void FillGroupCaption(CustomDrawNavBarElementEventArgs custom) {
			Rectangle r = custom.ObjectInfo.Bounds;
			Region reg = new Region(new Rectangle(r.X, r.Y + 2, r.Width, r.Height - 2));
			reg.Union(new Rectangle(r.X + 2, r.Y, r.Width - 4, 1));
			reg.Union(new Rectangle(r.X + 1, r.Y + 1, r.Width - 2, 1));
			Brush brush = custom.Appearance.GetBackBrush(custom.ObjectInfo.Cache, r);
			custom.Graphics.FillRegion(brush, reg);
			reg.Dispose();
		}
		protected override void DrawGroupCaptionCore(NavGroupInfoArgs e, CustomDrawNavBarElementEventArgs custom) {
			FillGroupCaption(custom);
			DrawGroupImage(e, custom);
			Brush brush = ExplorerBarColorHelper.CalcTextBrush(e, custom.Appearance.GetForeColor(), true);
			DrawGroupButton(e);
			if (e.Group.GetAllowHtmlString()) {
				StringPainter.Default.UpdateLocation(e.StringInfo, e.CaptionBounds.Location);
				StringPainter.Default.DrawString(e.Cache, e.StringInfo);
			} else
				custom.Appearance.DrawString(e.Cache, custom.Caption, e.CaptionBounds, brush);
		}
		protected override Rectangle CalcGroupCaptionButtonBounds(ObjectInfoArgs e) {
			Rectangle r = base.CalcGroupCaptionButtonBounds(e);
			if(r.IsEmpty) return r;
			r.Y -= 2;
			return r;
		}
	}
}
