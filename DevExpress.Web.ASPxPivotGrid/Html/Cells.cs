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
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxPivotGrid.Data;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.Web.FilterControl;
namespace DevExpress.Web.ASPxPivotGrid.HtmlControls {
	public class PivotGridHtmlDataCell : PivotGridHtmlTableCell {
		PivotGridCellItem cellItem;
		public PivotGridHtmlDataCell(PivotGridWebData data, PivotGridCellItem cellItem)
			: base(data) {
			this.cellItem = cellItem;
		}
		protected internal PivotGridCellItem CellItem { get { return cellItem; } }
		protected string GetText() { return CellItem.Text; }
		protected string GetPreparedText() {
			string cellText = GetText();
			if(cellText != null)
				cellText = cellText.Trim();
			return string.IsNullOrEmpty(cellText) ? "&nbsp;" : PivotGrid.HtmlEncode(cellText);
		}
		protected override void CreateChildControls() {
			if(Data.CellTemplate != null) {
				PivotGridCellTemplateContainer templateContainer
					= new PivotGridCellTemplateContainer(new PivotGridCellTemplateItem(CellItem, GetText()));
				Controls.Add(templateContainer);
				Data.SetupTemplateContainer(templateContainer, Data.CellTemplate);
				return;
			}
			if(CellItem.ShowKPIGraphic) {
				Image kpi = new Image();
				Data.KPIImages.GetImage(CellItem.KPIGraphic, CellItem.DataField.KPIType, CellItem.KPIValue).AssignToControl(kpi, false);
				Controls.Add(kpi);
			}
		}
#if DEBUGTEST
		public void AccessCreateChildControls() {
			CreateChildControls();
		}
#endif
		protected override void PrepareControlHierarchy() {
			if(Data.CellTemplate == null && !CellItem.ShowKPIGraphic) {
				Text = GetPreparedText();
			}
			if(!string.IsNullOrEmpty(PivotGrid.ClientSideEvents.CellClick)) {
				if(PivotGrid.IsEnabled()) Attributes.Add("onclick", ScriptHelper.GetCellClick(CellItem));
			}
			if(!string.IsNullOrEmpty(PivotGrid.ClientSideEvents.CellDblClick)) {
				if(PivotGrid.IsEnabled()) Attributes.Add("ondblclick", ScriptHelper.GetCellDblClick(CellItem));
			}
			PivotCellStyle cellStyle = new PivotCellStyle();
			Data.ApplyCellStyle(CellItem, cellStyle);
			if(CellItem.ColumnIndex == 0)
				cellStyle.BorderLeft.BorderColor = Data.GetTableStyle().Border.BorderColor;
			cellStyle.AssignToControl(this, true);
			base.PrepareControlHierarchy();
			PivotGrid.RaiseHtmlCellPrepared(this);
		}
	}
}
