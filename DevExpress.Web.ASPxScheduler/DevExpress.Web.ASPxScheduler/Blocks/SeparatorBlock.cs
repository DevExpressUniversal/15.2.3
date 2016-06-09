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

using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
using DevExpress.XtraScheduler;
namespace DevExpress.Web.ASPxScheduler.Internal {
	public class SeparatorBlock : ASPxSchedulerControlBlock {
		TableCell separatorCell;
		public SeparatorBlock(ASPxScheduler owner) : base(owner) { 
		}
		public override ASPxSchedulerChangeAction RenderActions { get { return ASPxSchedulerChangeAction.RenderView; } }
		public override string ContentControlID { get { return "viewSeparatorControl"; } }
		protected internal override void CreateControlHierarchyCore(Control parent) {
			bool isSeparatorCellRequired = Owner.ActiveViewType == SchedulerViewType.Day || Owner.ActiveViewType == SchedulerViewType.WorkWeek || Owner.ActiveViewType == SchedulerViewType.FullWeek;
			if(!isSeparatorCellRequired)
				return;
			InternalTable layoutTable = RenderUtils.CreateTable();
			TableRow bottomBorderRow = RenderUtils.CreateTableRow();
			parent.Controls.Add(layoutTable);
			layoutTable.Rows.Add(bottomBorderRow);
			this.separatorCell = RenderUtils.CreateTableCell();
			bottomBorderRow.Cells.Add(separatorCell);
		}
		protected internal override void PrepareControlHierarchyCore() {
			if(this.separatorCell == null)
				return;
			StylesHelper stylesHelper = StylesHelper.Create(Owner.ActiveView, Owner.ViewInfo, Owner.Styles);
			HeaderStyle dayHeaderStyle = stylesHelper.GetDayHeaderStyle(null);
			dayHeaderStyle.AssignToControl(this.separatorCell);
			RenderUtils.SetStyleStringAttribute(this.separatorCell, "height", "0");
			RenderUtils.SetStyleStringAttribute(this.separatorCell, "border-top-width", "1px");
			RenderUtils.SetStyleStringAttribute(this.separatorCell, "border-bottom-width", "0px");
			RenderUtils.SetStyleStringAttribute(this.separatorCell, "padding", "0px");
		}
		protected internal override void FinalizeCreateControlHierarchyCore(Control parent) {
		}
	}
}
