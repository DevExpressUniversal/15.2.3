#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.IO;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Design {
	public class NavigationTabsActionContainerDesigner : ContainerControlDesigner {
		public NavigationTabsActionContainerDesigner()
			: base() {
		}
		NavigationTabsActionContainer container;
		public override void Initialize(IComponent component) {
			container = ((NavigationTabsActionContainer)component);
			base.Initialize(component);
		}
		public override string GetDesignTimeHtml() {
			StringWriter textWriter = new StringWriter();
			HtmlTextWriter writer = new HtmlTextWriter(textWriter);
			ASPxMenuBase menu = (ASPxMenuBase)container.PageControl.TabPages[0].Controls[0].Controls[0];
			menu.Border.BorderStyle = BorderStyle.None;
			menu.BackColor = System.Drawing.Color.Transparent;
			menu.LinkStyle.Font.Underline = true;
			menu.LinkStyle.Color = System.Drawing.Color.Blue;
			container.Controls[0].RenderControl(writer);
			return textWriter.ToString();
		}
		public override string GetDesignTimeHtml(DesignerRegionCollection regions) {
			container.CreateDesignTimeContent();
			return GetDesignTimeHtml();
		}
	}
}
