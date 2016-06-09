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

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Design;
using DevExpress.XtraNavBar.ViewInfo;
namespace DevExpress.XtraNavBar {
	public class NavBarGroupDesignTimeBoundsProvider : ISmartTagClientBoundsProvider {
		public Rectangle GetBounds(IComponent component) {
			NavBarGroup group = (NavBarGroup)component;
			NavBarControl navbar = GetOwnerControl(component) as NavBarControl;
			foreach(NavGroupInfoArgs groupArgs in navbar.GetViewInfo().Groups) {
				if(groupArgs.Group == group)
					return groupArgs.Bounds;
			}
			return Rectangle.Empty;
		}
		public Control GetOwnerControl(IComponent component) {
			NavBarGroup group = (NavBarGroup)component;
			return group.NavBar;
		}
	}
	public class NavBarItemDesignTimeBoundsProvider : ISmartTagClientBoundsProvider {
		public Rectangle GetBounds(IComponent component) {
			NavBarControl navbar = GetOwnerControl(component) as NavBarControl;
			NavBarHitInfo hitInfo = navbar.CalcHitInfo(navbar.PointToClient(Control.MousePosition));
			if(hitInfo.HitTest != NavBarHitTest.Link && hitInfo.HitTest != NavBarHitTest.LinkCaption && hitInfo.HitTest != NavBarHitTest.LinkImage)
				return Rectangle.Empty;
			NavBarItemLink link = hitInfo.Link;
			foreach(NavGroupInfoArgs group in navbar.GetViewInfo().Groups) {
				foreach(NavLinkInfoArgs linkInfoArg in group.Links) {
					if(linkInfoArg.Link == link) return linkInfoArg.Bounds;
				}
			}
			return Rectangle.Empty;
		}
		public Control GetOwnerControl(IComponent component) {
			NavBarItem item = (NavBarItem)component;
			return item.NavBar;
		}
	}
}
