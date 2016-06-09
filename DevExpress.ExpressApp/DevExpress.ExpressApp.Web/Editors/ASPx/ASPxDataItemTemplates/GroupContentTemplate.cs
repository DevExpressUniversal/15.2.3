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

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Model;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	public class GroupContentTemplate : ITemplate {
		#region ITemplate Members
		IDataItemTemplateInfoProvider dataItemTemplateInfoProvider;
		public GroupContentTemplate(IDataItemTemplateInfoProvider dataItemTemplateInfoProvider) {
			this.dataItemTemplateInfoProvider = dataItemTemplateInfoProvider;
		}
		public void InstantiateIn(Control container) {
			GridViewGroupRowTemplateContainer groupRowTemplateContainer = (GridViewGroupRowTemplateContainer)container;
			GridViewDataColumn column = groupRowTemplateContainer.Column;
			string caption = column.Caption;
			Control valueControl = new Control();
			IDataColumnInfo columnInfo = dataItemTemplateInfoProvider.GetColumnInfo(column) as IDataColumnInfo;
			if(column.DataItemTemplate != null && columnInfo != null && columnInfo.Model.GroupInterval == GroupInterval.None) {
				GridViewDataItemTemplateContainer fakedContainer = new GridViewDataItemTemplateContainer(groupRowTemplateContainer.Grid, 0, groupRowTemplateContainer.VisibleIndex, column);
				column.DataItemTemplate.InstantiateIn(fakedContainer);
				for(int i = 0; i < fakedContainer.Controls.Count; i++) {
					valueControl.Controls.AddAt(i, fakedContainer.Controls[i]);
				}
				fakedContainer.Controls.Clear();
				fakedContainer.Dispose();
			}
			else {
				if(groupRowTemplateContainer.KeyValue != null && groupRowTemplateContainer.KeyValue.GetType() == typeof(DateTime) && ((DateTime)groupRowTemplateContainer.KeyValue) == DateTime.MinValue) {
					valueControl.Controls.Add(new LiteralControl(string.Empty));
				}
				else {
					valueControl.Controls.Add(new LiteralControl(groupRowTemplateContainer.GroupText));
				}
			}
			string summary = groupRowTemplateContainer.SummaryText;
			groupRowTemplateContainer.Controls.Add(LayoutGroupInfo(caption, valueControl, summary));
		}
		#endregion
		public virtual Control LayoutGroupInfo(string caption, Control valueControl, string summary) {
			Table table = new Table();
			table.ID = "Tbl";
			table.Rows.Add(new TableRow());
			table.Rows[0].Cells.Add(new TableCell());
			table.Rows[0].Cells.Add(new TableCell());
			table.Rows[0].Cells.Add(new TableCell());
			table.Rows[0].Cells[0].Text = caption + ": ";
			table.Rows[0].Cells[1].Controls.Add(valueControl);
			table.Rows[0].Cells[2].Text = summary;
			return table;
		}
	}
}
