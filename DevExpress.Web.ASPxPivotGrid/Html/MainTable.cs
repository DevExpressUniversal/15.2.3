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
using DevExpress.Web.ASPxPivotGrid.Data;
using DevExpress.Web.Internal;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Utils;
using DevExpress.Web.ASPxPivotGrid.Html;
using DevExpress.Web.ASPxPivotGrid.Internal;
namespace DevExpress.Web.ASPxPivotGrid.HtmlControls {
	public class PivotGridHtmlTable : PivotInternalTable, ISupportsCallbackResult {
		PivotGridPagerContainer topPagerContainer;
		PivotGridPagerContainer bottomPagerContainer;
		PivotWebFilterControlPopupRow prefilterPanel;
		InternalTableCell prefilterPanelContainer;
		public PivotGridHtmlTable(ASPxPivotGrid pivotGrid)
			: base(pivotGrid) {
		}
		public IEnumerable<ISupportsCallbackResult> GetPartialCallbackControls() {
			List<ISupportsCallbackResult> list = new List<ISupportsCallbackResult>();
			if(topPagerContainer != null)
				list.Add(topPagerContainer);
			if(bottomPagerContainer != null)
				list.Add(bottomPagerContainer);
			list.Add(this);
			return list;
		}
		protected PivotWebVisualItems VisualItems { get { return Data.VisualItems; } }
		public ASPxPivotGridPager TopPager { get { return topPagerContainer != null ? topPagerContainer.Pager : null; } }
		protected PivotWebFilterControlPopupRow PrefilterPanel { get { return prefilterPanel; } }
		protected InternalTableCell PrefilterPanelContainer { get { return prefilterPanelContainer; } }
		protected internal bool HasPagerCore {
			get {
				return PivotGrid.OptionsPager.HasPager(false) &&
					(PivotGrid.OptionsPager.AlwaysShowPager ||
					VisualItems.RowCount < VisualItems.GetLastLevelUnpagedItemCount(false) ||
					PivotGrid.OptionsPager.PageIndex == -1 ||
					Data.OptionsPager.IsPageSizeVisible());
			}
		}
		protected bool HasTopPager {
			get {
				return HasPagerCore &&
					(PivotGrid.OptionsPager.Position == PagerPosition.Top ||
						PivotGrid.OptionsPager.Position == PagerPosition.TopAndBottom);
			}
		}
		protected bool HasBottomPager {
			get {
				return HasPagerCore && 
					(PivotGrid.OptionsPager.Position == PagerPosition.Bottom ||
						PivotGrid.OptionsPager.Position == PagerPosition.TopAndBottom);
			}
		}
		protected override void CreateControlHierarchy() {
			Data.EnsureFieldCollections();
			CreatePager(true);
			CreateFilterHeaders();
			CreatePivotTableCell();
			CreatePager(false);
			CreatePrefilterPanel();
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderUtils.SetStringAttribute(this, "summary", PivotGrid.SummaryText);
			if(RenderUtils.Browser.Platform.IsMSTouchUI && PivotGrid.GetCanDragHeaders()){
				RenderUtils.AppendDefaultDXClassName(this, PivotGridStyles.MSTouchDraggableMarkerCssClassName);
			}
			PreparePrefilterPanel();
		}
		InternalTable pivotTable;
		WebControl pivotTableContainer;
		void CreatePivotTableCell() {
			InternalTableCell cell = new InternalTableCell();
			cell.ID = ElementNames.PivotTableCell;
			WebControl container = RenderUtils.CreateDiv();
			container.ClientIDMode = System.Web.UI.ClientIDMode.Predictable;
			container.ID = ElementNames.PivotTableContainerDiv;
			PivotGrid.Styles.CreateStyleCopyByName<AppearanceStyle>(PivotGridStyles.PivotTableContainerDivName).AssignToControl(container);
			this.pivotTable = CreatePivotTable();
			if(!PivotGrid.DesignMode && PivotGrid.RenderHelper.UseDynamicRender && !PivotGrid.IsCallback)
				pivotTable.Style[HtmlTextWriterStyle.Display] = "none";
			this.pivotTableContainer = container;
			pivotTable.ID = ElementNames.PivotTable;
			container.Controls.Add(pivotTable);
			cell.Controls.Add(container);
			WebControl dummyContainer = RenderUtils.CreateDiv();
			dummyContainer.Width = Unit.Pixel(0);
			dummyContainer.Height = Unit.Pixel(0);
			dummyContainer.Style[HtmlTextWriterStyle.Overflow] = "hidden";
			container.Style[HtmlTextWriterStyle.Overflow] = "hidden";
			dummyContainer.ClientIDMode = System.Web.UI.ClientIDMode.Predictable;
			dummyContainer.ID = ElementNames.PivotTableContainerDiv + "_Dummy";
			cell.Controls.Add(dummyContainer);
			InternalTableRow row = new InternalTableRow();
			row.Cells.Add(cell);
			Rows.Add(row);
		}
		InternalTable CreatePivotTable() {
			return new PivotTableHtmlTable(PivotGrid);
		}
		void PreparePrefilterPanel() {
			if(PrefilterPanelContainer == null)
				return;
			Data.Styles.GetPrefilterPanelContainerStyle().AssignToControl(PrefilterPanelContainer);
		}
		void CreatePager(bool isTopPager) {
			InternalTableRow row = new InternalTableRow();
			Rows.Add(row);
			PivotGridPagerContainer cell = new PivotGridPagerContainer(this, PivotGrid, isTopPager);
			row.Controls.Add(cell);
			if(isTopPager)
				topPagerContainer = cell;
			else
				bottomPagerContainer = cell;
		}
		void CreatePrefilterPanel() {
			if(Data.Prefilter.IsEmpty || !Data.OptionsCustomization.AllowPrefilter) return;
			InternalTableRow row = new InternalTableRow();
			Rows.Add(row);
			this.prefilterPanelContainer = new InternalTableCell();
			row.Cells.Add(PrefilterPanelContainer);
			this.prefilterPanel = new PivotWebFilterControlPopupRow(PivotGrid);
			PrefilterPanelContainer.Controls.Add(PrefilterPanel);
		}
		void CreateFilterHeaders() {
			if(!Data.OptionsView.ShowFilterHeaders) return;
			InternalTableRow row = new InternalTableRow();
			Rows.Add(row);
			AddHeader(row, PivotArea.FilterArea, 1);
		}
		CallbackResult ISupportsCallbackResult.CalcCallbackResult() {
			return new CallbackResult {
				ClientObjectId = PivotGrid.ClientID,
				ElementId = pivotTableContainer.ClientID,
				InnerHtml = RenderUtils.GetControlChildrenRenderResult(pivotTableContainer) + PivotGrid.GetCallbackClientObjectScript(),
				Parameters = "pivotTable"
			};
		}
	}
	public class PivotGridHtmlTableCell : InternalTableCell {
		PivotGridWebData data;
		public PivotGridHtmlTableCell(PivotGridWebData data) {
			this.data = data;
		}
		protected PivotGridWebData Data { get { return data; } }
		protected ASPxPivotGrid PivotGrid { get { return Data.PivotGrid; } }
		protected ScriptHelper ScriptHelper { get { return PivotGrid.ScriptHelper; } }
	}	
	public static class HeaderHelper {
		public static WebControl CreateHeader(PivotFieldItemBase field, PivotGridWebData data, ISupportsFieldsCustomization control) {
			if(field.Group == null) {
				return new PivotGridHtmlHeaderContent(data, field, DefaultBoolean.Default, control);
			} else {
				if(field.InnerGroupIndex == 0)
					return control != null ? new PivotGridHtmlSolidGroupHeader(data, field.Group, control) : new PivotGridHtmlGroupHeader(data, field.Group);
			}
			return null;
		}
	}
}
