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

using DevExpress.Web.ASPxPivotGrid.Data;
using DevExpress.Web.Internal;
using DevExpress.XtraPivotGrid;
using System;
using System.Web.UI;
namespace DevExpress.Web.ASPxPivotGrid.HtmlControls {
	public class PivotInternalTable : InternalTable {
		protected const int DefaultMaxColumnSpan = 999;
		readonly ASPxPivotGrid pivotGrid;
		protected ASPxPivotGrid PivotGrid { get { return pivotGrid; } }
		protected PivotGridWebData Data { get { return PivotGrid.Data; } }
		public PivotInternalTable(ASPxPivotGrid pivotGrid) {
			this.pivotGrid = pivotGrid;
		}
		public PivotTableColGroup AddColGroup(string id) {
			PivotTableColGroup colGroup = new PivotTableColGroup(this) { Id = id };
			ColGroups.Add(colGroup);
			return colGroup;
		}
		protected void AddHeader(InternalTableRow row, PivotArea area, int colSpan) {
			PivotGridHtmlAreaCellContainerBase cell = PivotGridHtmlAreaCellContainerBase.Create(Data, area);
			cell.ColumnSpan = Math.Min(DefaultMaxColumnSpan, colSpan);
			row.Controls.Add(cell);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			this.CellPadding = 0;
			this.CellSpacing = 0;
			Data.GetTableStyle().AssignToControl(this);
			RenderUtils.SetStyleStringAttribute(this, "border-collapse", "separate");
		}
		protected InternalTableRow[] CreateRows(int count) {
			return CreateRows(this, count);
		}
		protected InternalTableRow[] CreateRows(InternalTable table, int count) {
			InternalTableRow[] rows = new InternalTableRow[count];
			for(int i = 0; i < rows.Length; i++) {
				rows[i] = new InternalTableRow();
				table.Rows.Add(rows[i]);
			}
			return rows;
		}
	}
	public class PivotTableColGroup : InternalTableColGroup {
		readonly Control owner;
		string id;
		public PivotTableColGroup(Control owner) {
			this.owner = owner;
		}
		public string Id {
			get { return id; }
			set { id = value; }
		}
		protected override void Render(HtmlTextWriter writer) {
			writer.AddAttribute(HtmlTextWriterAttribute.Id, String.Format("{0}_{1}", owner.ClientID, id));
			base.Render(writer);
		}
	}
}
