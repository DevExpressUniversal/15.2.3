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

using DevExpress.Web;
using System.ComponentModel;
using System.Collections.Specialized;
namespace DevExpress.Web.ASPxPivotGrid {
	[Browsable(false), ToolboxItem(false)]
	public class ASPxPivotGridPopupMenu : ASPxPopupMenu {
		public const string ShowPrefilterID = "ShowPrefilter",
			SortAZID = "SortAZ", SortZAID = "SortZA",
			ClearSortID = "ClearSort", RefreshID = "Refresh", HideFieldID = "Hide",
			ShowFieldListID = "ShowList", HideFieldListID = "HideList";
		public const string SortGroupID = "SortGroup";
		public const string ExpandValueID = "Expand", CollapseValueID = "Collapse",
			ExpandAllID = "ExpandAll", CollapseAllID = "CollapseAll", SortByID = "SortBy_";
		ASPxPivotGrid pivotGrid;
		PivotGridPopupMenuType menuType;
		public ASPxPivotGridPopupMenu(ASPxPivotGrid pivotGrid, PivotGridPopupMenuType menuType)
			: base(pivotGrid) {
			this.pivotGrid = pivotGrid;
			this.menuType = menuType;
			this.EnableScrolling = pivotGrid.OptionsView.EnableContextMenuScrolling;
			ParentSkinOwner = PivotGrid;
		}
		protected internal ASPxPivotGrid PivotGrid { get { return pivotGrid; } }
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridPopupMenuMenuType")]
#endif
		public PivotGridPopupMenuType MenuType { get { return menuType; } }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			ItemStyle.Assign(PivotGrid.Styles.GetMenuItemStyle());
			ControlStyle.CopyFrom(PivotGrid.Styles.MenuStyle);
		}
		protected override void BeforeRender() {
			if(PivotGrid != null)
				PivotGrid.PreparePopupMenu(MenuType);
			base.BeforeRender();
		}
		protected override bool LoadPostData(NameValueCollection postCollection) {
 			bool res = base.LoadPostData(postCollection);
			foreach(MenuItem item in Items) {
				if(item.Name.StartsWith(ASPxPivotGridPopupMenu.SortByID))
					item.Checked = false;
			}
			return res;
		}
	}
}
