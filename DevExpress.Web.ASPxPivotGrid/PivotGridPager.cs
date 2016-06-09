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
using System.ComponentModel;
using DevExpress.Web;
using DevExpress.Web.ASPxPivotGrid.Data;
using DevExpress.Web.ASPxPivotGrid.HtmlControls;
using DevExpress.Web.ASPxPivotGrid.Html;
namespace DevExpress.Web.ASPxPivotGrid {
	[ToolboxItem(false)]
	public class ASPxPivotGridPager : ASPxPagerBase {
		ASPxPivotGrid pivotGrid;
		PivotGridHtmlTable htmlTable;
		protected PivotGridHtmlTable HtmlTable { get { return htmlTable; } }
		protected ASPxPivotGrid PivotGrid { get { return pivotGrid; } }
		protected PivotGridWebData Data { get { return PivotGrid.Data; } }
		protected ScriptHelper ScriptHelper { get { return PivotGrid.ScriptHelper; } }
		public ASPxPivotGridPager(PivotGridHtmlTable htmlTable, ASPxPivotGrid pivotGrid)
			: base(pivotGrid) {
			this.htmlTable = htmlTable;
			this.pivotGrid = pivotGrid;
			EnableViewState = false;
			ParentSkinOwner = PivotGrid;
		}
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridPagerPageIndex")]
#endif
		public override int PageIndex { get { return PivotGrid.OptionsPager.PageIndex; } }
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridPagerItemCount")]
#endif
		public override int ItemCount { get { return PivotGrid.Data.VisualItems.GetLastLevelUnpagedItemCount(false); } }
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridPagerItemsPerPage")]
#endif
		public override int ItemsPerPage { get { return PivotGrid.OptionsPager.RowsPerPage; } }
#if !SL
	[DevExpressWebASPxPivotGridLocalizedDescription("ASPxPivotGridPagerPageCount")]
#endif
		public override int PageCount { get { return PivotGrid.Data.VisualItems.GetPageCount(false); } }
		protected override bool RequireInlineLayout { get { return true; } }
		protected override int ItemCountInSummaryText { get { return PivotGrid.Data.VisualItems.GetLastLevelUnpagedItemCountWithoutGrandTotals(false); } }
		protected override string GetItemElementOnClick(string id) {
			if(ScriptHelper != null)
				return ScriptHelper.GetPagerOnClick(id);
			return base.GetItemElementOnClick(id);
		}
		protected override string GetPageSizeChangedHandler() {
			if(ScriptHelper != null)
				return ScriptHelper.GetPagerOnPageSizeChange();
			return base.GetPageSizeChangedHandler();
		}
		internal new PagerSettingsEx PagerSettings { get { return base.PagerSettings; } }
		protected override void PrepareControlHierarchy() {
			if(Page != null) 
				ApplyStyleSheetSkin(Page);
			Styles.Assign(PivotGrid.StylesPager);
			base.PrepareControlHierarchy();
		}
	}
}
