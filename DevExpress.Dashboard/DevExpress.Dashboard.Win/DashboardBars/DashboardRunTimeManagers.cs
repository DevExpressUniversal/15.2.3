#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Design;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.DashboardWin.Bars;
using DevExpress.DashboardWin.Commands;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.DashboardWin.Native {
	public class DashboardRibbonGenerationManager : RibbonGenerationManager<DashboardDesigner, DashboardCommandId> {
		protected override DashboardCommandId EmptyCommandId { get { return DashboardCommandId.None; } }
		public DashboardRibbonGenerationManager(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<DashboardDesigner, DashboardCommandId> barController)
			: base(creator, container, barController) {
		}
		protected override void AddItemToBarItemGroup(CommandBasedRibbonPageGroup pageGroup, BarItem item) {
			base.AddItemToBarItemGroup(pageGroup, item);
			DashboardBarButtonItem dashboardBarButtonItem = item as DashboardBarButtonItem;
			if(dashboardBarButtonItem != null && dashboardBarButtonItem.AddToQuickAccess)
				AddItemLink(RibbonControl.Toolbar.ItemLinks, item);
		}
		protected override CommandBasedRibbonPageGroup FindCommandRibbonPageGroupInPages(RibbonPageCollection pages) {
			 DashboardCommandBarCreator dashboardBarCreator = BarCreator as DashboardCommandBarCreator;
			 if (dashboardBarCreator != null && dashboardBarCreator.ShouldCreateNewRibbonPageGroupInstance)
				 if (!pages.Category.GetType().IsAssignableFrom(BarCreator.SupportedRibbonPageCategoryType))
					 return null;
			int pagesCount = pages.Count;
			for (int i = 0; i < pagesCount; i++) {
				CommandBasedRibbonPageGroup result = FindCommandRibbonPageGroupInPage(pages[i]);
				if (result != null)
					return result;
			}
			return null;
		}
		protected override CommandBasedRibbonPage FindCommandRibbonPage() {			
			if (RibbonControl == null)
				return null;
			DashboardCommandBarCreator dashboardBarCreator = BarCreator as DashboardCommandBarCreator;
			if (dashboardBarCreator != null && dashboardBarCreator.ShouldCreateNewRibbonPageInstance)
				return null;
			CommandBasedRibbonPage page = FindCommandRibbonPage(RibbonControl.Pages);
			if (page != null)
				return page;
			foreach (RibbonPageCategory category in RibbonControl.PageCategories) {
				if (BarCreator.SupportedRibbonPageCategoryType.IsAssignableFrom(category.GetType())) {
					page = FindCommandRibbonPage(category.Pages);
					if (page != null)
						return page;
				}
			}
			return null;
		}
	}
	public class DashboardBarGenerationManager : BarGenerationManager<DashboardDesigner, DashboardCommandId> {
		public DashboardBarGenerationManager(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<DashboardDesigner, DashboardCommandId> barController)
			: base(creator, container, barController) {
		}
	}
}
