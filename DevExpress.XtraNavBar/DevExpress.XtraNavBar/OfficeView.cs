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
namespace DevExpress.XtraNavBar.ViewInfo {
	public class Office1NavBarViewInfo : NavBarViewInfo {
		public Office1NavBarViewInfo(NavBarControl navBar) : base(navBar) {
		}
		protected override int CalcGroupVertIndent(NavGroupInfoArgs ia) {
			return 1;
		}
		protected override BorderPainter CreateDefaultBorderPainter() {
			return new Office1BorderPainter(); 
		}
	}
	public class Office1NavGroupPainter : FlatNavGroupPainter {
		public Office1NavGroupPainter(NavBarControl navBar) : base(navBar) {
		}
		protected override ObjectPainter CreateButtonPainter() {
			return new Office1FlatButtonObjectPainter();
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			Rectangle r = base.CalcObjectMinBounds(e);
			r.Height += 4;
			return r;
		}
	}
	public class Office1NavLinkPainter : BaseNavLinkPainter  {
		public Office1NavLinkPainter(NavBarControl navBar) : base(navBar)  {
		}
		protected override int CalcLargeVertIndent(ObjectInfoArgs e) {
			return 6;
		}
	}
	public class Office2NavLinkPainter : Office1NavLinkPainter {
		protected ButtonObjectPainter buttonPainter;
		public Office2NavLinkPainter(NavBarControl navBar) : base(navBar)  {
			buttonPainter = new UltraFlatButtonObjectPainter();
		}
		protected override void DrawLinkImageBorder(ObjectInfoArgs e, Rectangle r) {
			NavLinkInfoArgs li = e as NavLinkInfoArgs;
			if(IsDrawLinkHotOrPressed(e)) {
				ObjectInfoArgs ia = new StyleObjectInfoArgs(e.Cache, li.ImageRectangle, GetLinkAppearance(e), e.State);
				buttonPainter.DrawObject(ia);
			}
		}
		public override ObjectState CalcLinkState(NavBarItemLink link, ObjectState state) {
			if((state & ObjectState.Selected) != 0) {
				if(NavBar.PressedLink == link) return ObjectState.Pressed;
				return ObjectState.Pressed;
			}
			return base.CalcLinkState(link, state);
		}
	}
	public class Office3NavBarViewInfo : Office1NavBarViewInfo {
		public Office3NavBarViewInfo(NavBarControl navBar) : base(navBar) {
		}
		protected override NavBarHitTest[] CreateValidHotTracks() {
			return new NavBarHitTest[] {
					NavBarHitTest.UpButton, NavBarHitTest.DownButton,
					NavBarHitTest.GroupCaption, NavBarHitTest.GroupCaptionButton,
					NavBarHitTest.LinkCaption, NavBarHitTest.LinkImage, NavBarHitTest.Link };
		}
		protected override NavBarHitTest[] CreateValidPressedInfo() {
			return new NavBarHitTest[] {
					NavBarHitTest.UpButton, NavBarHitTest.DownButton,
					NavBarHitTest.GroupCaption, NavBarHitTest.GroupCaptionButton,
					NavBarHitTest.LinkCaption, NavBarHitTest.LinkImage, NavBarHitTest.Link };
		}
		protected override NavBarState[] CreateValidPressedStateInfo() {
			return new NavBarState[] {
					NavBarState.UpButtonPressed, NavBarState.DownButtonPressed,
					NavBarState.GroupPressed, NavBarState.GroupPressed,
					NavBarState.LinkPressed, NavBarState.LinkPressed, NavBarState.LinkPressed };
		}
	}
	public class Office3NavLinkPainter : Office2NavLinkPainter  {
		public Office3NavLinkPainter(NavBarControl navBar) : base(navBar)  {
		}
		protected override void DrawLinkImageBorder(ObjectInfoArgs e, Rectangle r) {
			NavLinkInfoArgs li = e as NavLinkInfoArgs;
			if(IsDrawLinkHotOrPressed(e)) {
				StyleObjectInfoArgs ia = new StyleObjectInfoArgs(e.Cache, li.Bounds, GetLinkAppearance(e), e.State);
				buttonPainter.DrawObject(ia);
			}
		}
		protected override int CalcLargeVertIndent(ObjectInfoArgs e) {
			return 10;
		}
	}
}
