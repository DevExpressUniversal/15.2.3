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
using DevExpress.Web.ASPxPivotGrid.Html;
namespace DevExpress.Web.ASPxPivotGrid.HtmlControls {
	public class PivotGridHtmlArea : InternalTable {
		PivotGridWebData data;
		PivotArea area;
		public PivotGridHtmlArea(PivotGridWebData data, PivotArea area) {
			this.data = data;
			this.area = area;
		}
		protected PivotGridWebData Data { get { return data; } }
		protected PivotArea Area { get { return area; } }
		protected override void CreateControlHierarchy() {
			ID = Area.ToString();
			PivotGridHtmlAreaHeaders headers = new PivotGridHtmlAreaHeaders(Data, Area);
			Controls.Add(headers);
		}
	}
	public class PivotGridHtmlAreaHeaders : InternalTableRow {
		PivotGridWebData data;
		PivotArea area;
		public PivotGridHtmlAreaHeaders(PivotGridWebData data, PivotArea area) {
			this.data = data;
			this.area = area;
		}
		public PivotGridWebData Data { get { return data; } }
		public PivotArea Area { get { return area; } }
		protected override void CreateControlHierarchy() {
			List<PivotFieldItemBase> fields = Data.GetFieldItemsByArea(Area);
			foreach(PivotFieldItemBase field in fields) {
				WebControl header = HeaderHelper.CreateHeader(field, Data, null);
				if(header == null)
					continue;
				InternalTableCell cell = new InternalTableCell();
				cell.Controls.Add(header);
				Controls.Add(cell);
			}
		}
		protected override void PrepareControlHierarchy() {
			for(int i = 0; i < Controls.Count; i++) {
				TableCell cell = Controls[i] as TableCell;
				if(cell != null) {
					RenderUtils.SetPaddings(cell, Data.GetAreaPaddings(Area, i == 0, i == Controls.Count - 1));
					if(RenderUtils.Browser.IsOpera)
						RenderUtils.SetStyleStringAttribute(cell, "border-left-style", "none"); 
				}
			}
		}
	}
	public abstract class PivotGridHtmlAreaCellContainerBase : InternalTableCell {
		PivotGridWebData data;
		PivotArea area;
		public PivotGridHtmlAreaCellContainerBase(PivotGridWebData data, PivotArea area) {
			this.data = data;
			this.area = area;
		}
		public static PivotGridHtmlAreaCellContainerBase Create(PivotGridWebData data, PivotArea area) {
			if(!data.PivotGrid.RenderHelper.GetHeadersVisible(area, true))
				return new PivotGridHtmlEmptyAreaCellContainer(data, area);
			if(area == PivotArea.DataArea && data.IsDataAreaCollapsed)
				return new PivotGridHtmlCollapsedDataAreaCellContainer(data, area);
			return new PivotGridHtmlAreaCellContainer(data, area);
		}
		protected PivotGridWebData Data { get { return data; } }
		protected ASPxPivotGrid PivotGrid { get { return Data.PivotGrid; } }
		protected ScriptHelper ScriptHelper { get { return PivotGrid.ScriptHelper; } }
		protected PivotArea Area { get { return area; } }
		protected virtual string GetID() {
			return ScriptHelper.GetHeaderTableID(Area);
		}
		protected override void CreateControlHierarchy() {
			ID = GetID();
			PivotGrid.RenderHelper.AddHeaderContextMenu(ID, null);
		}
		protected override void PrepareControlHierarchy() {
		}
	}
	public class PivotGridHtmlEmptyAreaCellContainer : PivotGridHtmlAreaCellContainerBase {
		readonly InternalTable containerTable;
		readonly TableCell containerTableCell;
		public PivotGridHtmlEmptyAreaCellContainer(PivotGridWebData data, PivotArea area) : base(data, area) {
			InternalTable table = RenderUtils.CreateTable();
			TableRow row = RenderUtils.CreateTableRow();
			TableCell cell = RenderUtils.CreateTableCell();
			row.Cells.Add(cell);
			table.Rows.Add(row);
			table.ID = ScriptHelper.GetAreaContainerID(area);
			this.containerTable = table;
			this.containerTableCell = cell;
		}
		protected override string GetID() {
			return ScriptHelper.GetAreaID(Area);
		}
		protected bool IsAreaVisible {
			get {
				return Data.OptionsView.GetShowHeaders(Area);
			}
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			Controls.Add(containerTable);
			if(IsAreaVisible) {
				if(Data.EmptyAreaTemplate != null) {
					PivotGridEmptyAreaTemplateContainer templateContainer = new PivotGridEmptyAreaTemplateContainer(Area);
					containerTableCell.Controls.Add(templateContainer);					
					Data.SetupTemplateContainer(templateContainer, Data.EmptyAreaTemplate);
				} else {
					string text = PivotGridLocalizer.GetHeadersAreaText((int)Area);
					containerTableCell.Controls.Add(RenderUtils.CreateLiteralControl(text));
				}
			}
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			Data.GetAreaStyle(Area).AssignToControl(this, true);
			Data.Styles.CreateStyleByName(PivotGridStyles.EmptyAreaStyleName).AssignToControl(containerTableCell, true);
		}
	}
	public class PivotGridHtmlAreaCellContainer : PivotGridHtmlAreaCellContainerBase {
		public PivotGridHtmlAreaCellContainer(PivotGridWebData data, PivotArea area) : base(data, area) { }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			if((Area == PivotArea.RowArea && Data.OptionsView.ShowRowHeaders) ||
				(Area == PivotArea.ColumnArea && Data.OptionsView.ShowColumnHeaders) ||
				(Area == PivotArea.DataArea && Data.OptionsView.ShowDataHeaders)) {
				Controls.Add(new PivotGridHtmlArea(Data, Area));
			}
			if(Area == PivotArea.FilterArea && Data.OptionsView.ShowFilterHeaders) {
				WebControl filterAreaContainer = RenderUtils.CreateDiv();
				filterAreaContainer.ID = "FilterAreaContainer";
				filterAreaContainer.Style.Add("overflow", "hidden");
				filterAreaContainer.Controls.Add(new PivotGridHtmlArea(Data, Area));
				Controls.Add(filterAreaContainer);
			}
			if(Area == PivotArea.ColumnArea && !Data.OptionsView.ShowColumnHeaders && Data.OptionsView.ShowDataHeaders) {
				Controls.Add(new LiteralControl("&nbsp;"));	
			}
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			Data.GetAreaStyle(Area).AssignToControl(this);
		}
	}
	public class PivotGridHtmlCollapsedDataAreaCellContainer : PivotGridHtmlAreaCellContainerBase {
		PivotGridDataAreaPopup dataAreaPopup;
		Image image;
		LiteralControl text;
		protected PivotGridDataAreaPopup DataAreaPopup { get { return dataAreaPopup; } }
		protected Image Image { get { return image; } }
		protected new LiteralControl Text { get { return text; } }
		public PivotGridHtmlCollapsedDataAreaCellContainer(PivotGridWebData data, PivotArea area) : base(data, area) { }
		protected override string GetID() {
			return ElementNames.DataHeadersPopupCell;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			this.image = RenderUtils.CreateImage();
			this.text = RenderUtils.CreateLiteralControl(PivotGridLocalizer.GetString(PivotGridStringId.PrintDesignerDataHeaders));
			Controls.Add(Image);
			Controls.Add(Text);
			if(!Data.PivotGrid.DesignMode) {
				this.dataAreaPopup = Data.PivotGrid.CreateDataAreaPopup();
				Controls.Add(dataAreaPopup);
			}
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			Data.GetEmptyAreaStyle(Area).AssignToControl(this);
			if(DataAreaPopup != null) {
				DataAreaPopup.PopupElementID = ClientID;
				DataAreaPopup.PopupVerticalAlign = PopupVerticalAlign.TopSides;
				DataAreaPopup.PopupHorizontalAlign = PopupHorizontalAlign.LeftSides;
			}
			if(Image != null) {
				PivotGrid.RenderHelper.GetDataHeadersImage().AssignToControl(Image, Data.IsDesignMode);
				PivotGrid.Styles.ApplyDataHeadersImageStyle(Image);
			}
		}
	}
}
