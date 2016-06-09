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
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Internal {
	public class RenderHelper {
		ASPxMenuBase menu;
		public RenderHelper(ASPxMenuBase menu) { 
			this.menu = menu;
		}
		public ASPxMenuBase Menu { get { return menu; } }
		public WebControl CreateScrollArea(WebControl parent, MenuItem item) {
			WebControl area = RenderUtils.CreateDiv();
			parent.Controls.Add(area);
			return area;
		}
		public DivButtonControl CreateScrollUpButton(WebControl parent, MenuItem item) {
			DivButtonControl button = new DivButtonControl();
			parent.Controls.Add(button);
			return button;
		}
		public DivButtonControl CreateScrollDownButton(WebControl parent, MenuItem item) {
			DivButtonControl button = new DivButtonControl();
			parent.Controls.Add(button);
			return button;
		}
		public void PrepareScrollArea(WebControl area, MenuItem item) {
			Menu.GetScrollAreaStyle(item).AssignToControl(area);
		}
		public void PrepareScrollUpButton(DivButtonControl button, MenuItem item) {
			button.ButtonImage = Menu.GetScrollUpButtonImageProperties(item);
			button.ButtonStyle = Menu.GetScrollUpButtonStyle(item);
			button.ButtonPaddings = Menu.GetScrollUpButtonContentPaddings(item);
			button.ButtonImageID = Menu.GetScrollUpButtonImageID(item);
			RenderUtils.SetVisibility(button, false, true);
		}
		public void PrepareScrollDownButton(DivButtonControl button, MenuItem item) {
			button.ButtonImage = Menu.GetScrollDownButtonImageProperties(item);
			button.ButtonStyle = Menu.GetScrollDownButtonStyle(item);
			button.ButtonPaddings = Menu.GetScrollDownButtonContentPaddings(item);
			button.ButtonImageID = Menu.GetScrollDownButtonImageID(item);
			RenderUtils.SetVisibility(button, false, true);
		}
		public void PrepareTableElement(WebControl table) { 
			RenderUtils.ApplyCellPaddingAndSpacing(table);
		}
	}
}
