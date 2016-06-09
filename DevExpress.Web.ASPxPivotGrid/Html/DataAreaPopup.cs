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
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxPivotGrid.Data;
using DevExpress.Web.ASPxPivotGrid.Html;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.Web.ASPxPivotGrid.HtmlControls {
	public class PivotGridDataAreaPopup : DevExpress.Web.ASPxPopupControl {
		ASPxPivotGrid pivotGrid;
		Table maintable;
		protected ASPxPivotGrid PivotGrid { get { return pivotGrid; } }
		protected PivotGridWebData Data { get { return PivotGrid.Data; } }
		protected internal Table MainTable { get { return maintable; } }
		public PivotGridDataAreaPopup(ASPxPivotGrid pivotGrid) {
			this.pivotGrid = pivotGrid;
			ParentSkinOwner = PivotGrid;			
			ShowHeader = false;
			ShowFooter = false;
			PopupAnimationType = AnimationType.Fade;
			AllowDragging = false;
			AllowResize = false;
			CloseAction = RenderUtils.Browser.Platform.IsTouchUI ? CloseAction.OuterMouseClick : CloseAction.None;
			PopupAction = RenderUtils.Browser.Platform.IsTouchUI ? PopupAction.LeftMouseClick : PopupAction.MouseOver;
			AppearAfter = 0;
			DisappearAfter = 0;
			PopupHorizontalAlign = PopupHorizontalAlign.LeftSides;
			PopupVerticalAlign = PopupVerticalAlign.TopSides;
			PopupHorizontalOffset = 2;
			PopupVerticalOffset = 2;
			ShowOnPageLoad = false;
			ShowShadow = true;
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.maintable = null;
		}
		protected override void CreateControlHierarchy() {
			ID = ElementNames.DataHeadersPopup;
			this.maintable = RenderUtils.CreateTable();
			CreateHeaders();
			Controls.Clear();
			Controls.Add(MainTable);
			base.CreateControlHierarchy();
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(MainTable != null) {
				MainTable.GridLines = GridLines.None;
				MainTable.Style.Add(HtmlTextWriterStyle.BorderCollapse, "collapse");
				MainTable.CellPadding = 0;
				MainTable.CellSpacing = 0;
				MainTable.Width = Unit.Pixel(1);
				foreach(TableRow row in MainTable.Rows) {
					for(int i = 0; i < row.Cells.Count; i++)
						Data.GetAreaPaddings(PivotArea.DataArea, i == 0, i == row.Cells.Count - 1).AssignToControl(row.Cells[i]);
				}
			}
			Width = Unit.Pixel(1);
			ControlStyle.CopyFrom(PivotGrid.Styles.CustomizationFieldsStyle);
			ContentStyle.CopyFrom(PivotGrid.Styles.GetCustomizationFieldsContentStyle());
		}
		void CreateHeaders() {
			TableRow row = RenderUtils.CreateTableRow();
			List<PivotFieldItemBase> fields = Data.GetFieldItemsByArea(PivotArea.DataArea);
			int fieldCounter = 0;
			foreach(PivotFieldItemBase field in fields) {
				WebControl header = HeaderHelper.CreateHeader(field, Data, null);
				if(header == null)
					continue;
				TableCell cell = RenderUtils.CreateTableCell();
				cell.Controls.Add(header);
				row.Cells.Add(cell);
				if(CountPerLine > 0 && (++fieldCounter % CountPerLine == 0) && fieldCounter < fields.Count) {
					MainTable.Rows.Add(row);
					row = RenderUtils.CreateTableRow();
				}
			}
			MainTable.Rows.Add(row);
		}
		protected int CountPerLine { get { return pivotGrid.OptionsView.DataHeadersPopupMaxColumnCount; } }
		protected override Paddings GetContentPaddings(PopupWindow window) {
			return new Paddings(0);
		}
	}
}
