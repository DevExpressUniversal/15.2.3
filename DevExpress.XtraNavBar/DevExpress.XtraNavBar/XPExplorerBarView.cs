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
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraNavBar;
using System.Collections;
using DevExpress.Utils.Text;
namespace DevExpress.XtraNavBar.ViewInfo {
	public class XPExplorerBarNavBarViewInfo : ExplorerBarNavBarViewInfo {
		public XPExplorerBarNavBarViewInfo(NavBarControl navBar) : base(navBar) {
		}
		protected override void DrawBackgroundCore(GraphicsCache e, CustomDrawObjectEventArgs custom, Rectangle r) {
			XPObjectPainter backPainter = new XPObjectPainter("explorerbar", 1, 1);
			StyleObjectInfoArgs args = new StyleObjectInfoArgs(e, r, custom.Appearance, ObjectState.Normal);
			backPainter.DrawObject(args);
		}
	}
	public class XPExplorerBarNavLinkPainter : ExplorerBarNavLinkPainter {
		public XPExplorerBarNavLinkPainter(NavBarControl navBar) : base(navBar) {
		}
		protected override void DrawLinkImageBorder(ObjectInfoArgs e, Rectangle r) {
		}
		protected override Brush GetLinkCaptionBrush(ObjectInfoArgs e) { 
			NavBarItemLink link = GetLink(e);
			return ExplorerBarColorHelper.CalcTextBrush(e, Color.Empty, link.Enabled);
		}
	}
	public class XPExplorerBarNavGroupClientPainter : ExplorerBarNavGroupClientPainter {
		public XPExplorerBarNavGroupClientPainter(BaseNavGroupPainter groupPainter) : base(groupPainter) { }
		protected new XPExplorerBarNavGroupPainter GroupPainter { get { return base.GroupPainter as XPExplorerBarNavGroupPainter; } }
		protected override void DrawBorder(NavGroupClientInfoArgs e) { }
		protected override void DrawBackgroundCore(NavGroupClientInfoArgs e, CustomDrawObjectEventArgs custom) {
			XPObjectPainter backPainter = new XPObjectPainter("explorerbar", 5, 1);
			ObjectInfoArgs args = GroupPainter.CreateXPArgs(e);
			args.Bounds = e.ClientBounds;
			backPainter.DrawObject(args);
		}
		protected internal override int CalcGroupClientHeightByClientSize(Size clientSize) {
			return clientSize.Height;
		}
		protected override void CalcClientBounds(NavGroupClientInfoArgs e) {
			e.ClientBounds = e.Bounds;
		}
	}
	public class XPExplorerBarNavGroupPainter : ExplorerBarNavGroupPainter {
		XPObjectPainter groupCaptionPainter;
		public XPExplorerBarNavGroupPainter(NavBarControl navBar) : base(navBar) {
			groupCaptionPainter = new XPObjectPainter("explorerbar", 8, 2);
		}
		protected override ObjectPainter CreateOpenCloseButtonPainter() { return new XPExplorerBarOpenCloseButtonObjectPainter(); }
		protected override BaseNavGroupClientPainter CreateGroupClientPainter() { 
			return new XPExplorerBarNavGroupClientPainter(this); 
		}
		public virtual StyleObjectInfoArgs CreateXPArgs(ObjectInfoArgs e) {
			StyleObjectInfoArgs args = new StyleObjectInfoArgs(e.Cache, e.Bounds, GetGroupAppearance(e), e.State);
			return args;
		}
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			Rectangle r = base.CalcObjectBounds(e);
			Rectangle rNew = groupCaptionPainter.CalcObjectMinBounds(CreateXPArgs(e));
			r.Height = Math.Max(r.Height, rNew.Height) + 2;
			return r;
		}
		protected override void DrawGroupCaptionCore(NavGroupInfoArgs e, CustomDrawNavBarElementEventArgs custom) {
			ObjectInfoArgs xpArgs = CreateXPArgs(e);
			groupCaptionPainter.DrawObject(xpArgs);
			DrawGroupImage(e, custom);
			Brush brush = ExplorerBarColorHelper.CalcTextBrush(e, Color.Empty, true);
			DrawGroupButton(e);
			if (e.Group.GetAllowHtmlString()) {
				StringPainter.Default.UpdateLocation(e.StringInfo, e.CaptionBounds.Location);
				StringPainter.Default.DrawString(e.Cache, e.StringInfo);
			} else
				custom.Appearance.DrawString(e.Cache, custom.Caption, e.CaptionBounds, brush);
		}
	}
	public class XPExplorerBarOpenCloseButtonObjectPainter : ExplorerBarOpenCloseButtonObjectPainter {
		XPObjectPainter buttonPainter;
		bool drawSpecial;
		public XPExplorerBarOpenCloseButtonObjectPainter() {
			drawSpecial = false;
			buttonPainter = new XPObjectPainter("explorerbar", 7, -1);
		}
		protected int CalcPart(ObjectInfoArgs e) {
			bool expanded = ((ExplorerBarOpenCloseButtonInfoArgs)e).Expanded;
			if(drawSpecial) {
				return (expanded ?  10 : 11);
			}
			return (expanded ? 6 : 7);
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			buttonPainter.Part = CalcPart(e);
			return buttonPainter.CalcObjectMinBounds(e);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			buttonPainter.Part = CalcPart(e);
			buttonPainter.DrawObject(e);
		}
	}
}
