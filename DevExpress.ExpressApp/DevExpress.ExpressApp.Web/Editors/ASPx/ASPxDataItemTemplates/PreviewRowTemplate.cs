#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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

using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	public class PreviewRowTemplate : ITemplate {
		#region ITemplate Members
		public void InstantiateIn(Control container) {
			GridViewPreviewRowTemplateContainer templateContainer = (GridViewPreviewRowTemplateContainer)container;
			ASPxGridView grid = templateContainer.Grid;
			int visibleIndex = templateContainer.VisibleIndex;
			GridViewDataColumn column = (GridViewDataColumn)grid.Columns[grid.PreviewFieldName];
			object obj = grid.GetRow(visibleIndex);
			GridViewDataItemTemplateContainer dataItemTemplateContainer = new GridViewDataItemTemplateContainer(grid, obj, visibleIndex, column);
			ITemplate dataItemColumnTemplate = column.DataItemTemplate;
			if(dataItemColumnTemplate != null) {
				dataItemColumnTemplate.InstantiateIn(dataItemTemplateContainer);
				templateContainer.Controls.Add(dataItemTemplateContainer);
			}
			else {
				templateContainer.Controls.Add(CreateLiteralControl(templateContainer.Text));
			}
		}
		protected virtual Control CreateLiteralControl(string previewText) {
			Table table = RenderHelper.CreateTable();
			table.Rows.Add(new TableRow());
			table.Rows[0].Cells.Add(new TableCell());
			table.Rows[0].Cells[0].Controls.Add(new LiteralControl(previewText));
			return table;
		}
		#endregion
	}
}
