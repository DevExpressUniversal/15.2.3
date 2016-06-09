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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.XtraDiagram.Utils {
	public class DiagramUtils {
		public static RibbonControl FindRibbon(DiagramControl diagram) {
			Form parentForm = diagram.FindForm();
			if(parentForm != null) {
				return FindControl(parentForm, typeof(RibbonControl)) as RibbonControl;
			}
			return null;
		}
		public static Control FindControl(Control control, Type t) {
			if(control != null) {
				foreach(Control c in control.Controls) {
					if(t.IsAssignableFrom(c.GetType())) return c;
				}
			}
			return null;
		}
		public static RibbonControl CreateRibbon() {
			return new RibbonControl();
		}
		public static void SetupRibbon(RibbonControl ribbon) {
			ribbon.BeginInit();
			try {
				ribbon.RibbonStyle = RibbonControlStyle.Office2013;
				ribbon.ToolbarLocation = RibbonQuickAccessToolbarLocation.Above;
				ribbon.Dock = DockStyle.Top;
				if(ribbon.ApplicationButtonDropDownControl == null) ribbon.ApplicationButtonDropDownControl = CreateApplicationMenu(ribbon);
			}
			finally {
				ribbon.EndInit();
			}
		}
		static ApplicationMenu CreateApplicationMenu(RibbonControl ribbon) {
			ApplicationMenu appMenu = new ApplicationMenu();
			appMenu.MenuDrawMode = MenuDrawMode.LargeImagesTextDescription;
			appMenu.MinWidth = 350;
			appMenu.Ribbon = ribbon;
			AddToContainer(ribbon, appMenu);
			return appMenu;
		}
		static void AddToContainer(RibbonControl ribbon, Component component) {
			ISite site = ribbon.Site;
			if(site != null) {
				IContainer c = site.Container;
				if(c != null)
					c.Add(component);
			}
		}
		public static RibbonPage FindPage(RibbonControl ribbonControl, Type pageType) {
			RibbonPage page = FindPageInternal(ribbonControl.Pages, pageType);
			if(page != null)
				return page;
			foreach(RibbonPageCategory pageCategory in ribbonControl.PageCategories) {
				page = FindPageInternal(pageCategory.Pages, pageType);
				if(page != null)
					return page;
			}
			return null;
		}
		static RibbonPage FindPageInternal(RibbonPageCollection ribbonPages, Type pageType) {
			foreach(RibbonPage page in ribbonPages)
				if(page.GetType() == pageType)
					return page;
			return null;
		}
		public static string GenerateDiagramItemName(DiagramControl diagram, DiagramItem item) {
			if(diagram.Site == null) return string.Empty;
			string itemName = string.Empty;
			INameCreationService svc = diagram.Site.GetService(typeof(INameCreationService)) as INameCreationService;
			if(svc != null) {
				itemName = svc.CreateName(diagram.Container, item.GetType());
			}
			return itemName;
		}
	}
}
