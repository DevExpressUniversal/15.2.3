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
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxPivotGrid.Data;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxPivotGrid.HtmlControls {
	public class PivotGridPagerContainer : InternalTableCell, ISupportsCallbackResult {
		PivotGridHtmlTable htmlTable;
		ASPxPivotGrid pivotGrid;
		ASPxPivotGridPager pager;
		WebControl container;
		bool isTopPager;
		public PivotGridPagerContainer(PivotGridHtmlTable htmlTable, ASPxPivotGrid pivotGrid, bool isTopPager) {
			this.htmlTable = htmlTable;
			this.pivotGrid = pivotGrid;
			this.isTopPager = isTopPager;
		}
		protected PivotGridHtmlTable HtmlTable { get { return htmlTable; } }
		protected ASPxPivotGrid PivotGrid { get { return pivotGrid; } }
		protected PivotGridWebData Data { get { return PivotGrid.Data; } }
		public ASPxPivotGridPager Pager { get { return pager; } }
		protected bool IsTopPager { get { return isTopPager; } }
		PagerPosition Position { get { return isTopPager ? PagerPosition.Top : PagerPosition.Bottom; } }
		bool HasPager {
			get {
				return htmlTable.HasPagerCore &&
					(pivotGrid.OptionsPager.Position == PagerPosition.TopAndBottom || PivotGrid.OptionsPager.Position == Position);
			}
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			string id = IsTopPager ? "TopPager" : "BottomPager";
			ID = "Container" + id;
			if(HasPager) {
				container = RenderUtils.CreateDiv();
				pager = new ASPxPivotGridPager(HtmlTable, PivotGrid);
				pager.ID = id;
				Pager.Width = PivotGrid.OptionsPager.PageSizeItemSettings.Visible ? Unit.Percentage(100) : Unit.Empty;
				Controls.Add(container);
				container.Controls.Add(Pager);
			}
		}
		protected override void PrepareControlHierarchy() {
			if(HasPager && Pager != null) {
				Pager.PagerSettings.Assign(PivotGrid.OptionsPager);
				Data.GetPagerStyle(IsTopPager).AssignToControl(container);
			}
			base.PrepareControlHierarchy();
		}
		CallbackResult ISupportsCallbackResult.CalcCallbackResult() {
			PrepareControlHierarchy();
			return new CallbackResult {
				ClientObjectId = PivotGrid.ClientID,
				ElementId = ClientID,
				InnerHtml = RenderUtils.GetControlChildrenRenderResult(this),
				Parameters = "Pager"
			};
		}
	}
}
