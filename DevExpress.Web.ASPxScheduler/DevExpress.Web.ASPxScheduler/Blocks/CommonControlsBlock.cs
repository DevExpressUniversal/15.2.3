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
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxScheduler.Drawing;
using DevExpress.Web.ASPxScheduler.Rendering;
using DevExpress.Web;
namespace DevExpress.Web.ASPxScheduler.Internal {
	public class CommonControlsBlock : ASPxSchedulerControlBlock {
		#region Fields
		WebControl selectionDiv;
		ASPxSelectedAppointmentAdorner selectedAppointmentAdorner;
		WebControl topResizeControlDiv;
		WebControl bottomResizeControlDiv;
		WebControl leftResizeControlDiv;
		WebControl rightResizeControlDiv;
		#endregion
		public CommonControlsBlock(ASPxScheduler control) : base(control) {
		}
		#region Properties
		public override string ContentControlID { get { return "commonControlsBlock"; } }
		public override ASPxSchedulerChangeAction RenderActions { get { return ASPxSchedulerChangeAction.RenderCommonControls; } }
		internal WebControl SelectionDiv { get { return selectionDiv; } }
		internal ASPxSelectedAppointmentAdorner SelectedAppointmentAdorner { get { return selectedAppointmentAdorner; } }
		internal WebControl TopResizeControlDiv { get { return topResizeControlDiv; } }
		internal WebControl BottomResizeControlDiv { get { return bottomResizeControlDiv; } }
		internal WebControl LeftResizeControlDiv { get { return leftResizeControlDiv; } }
		internal WebControl RightResizeControlDiv { get { return rightResizeControlDiv; } }
		#endregion
		protected internal override void CreateControlHierarchyCore(Control parent) {
			this.selectionDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			selectionDiv.ID = "selectionDiv";
			parent.Controls.Add(selectionDiv);
			selectedAppointmentAdorner = new ASPxSelectedAppointmentAdorner(Owner);
			parent.Controls.Add(selectedAppointmentAdorner);
			this.topResizeControlDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			parent.Controls.Add(TopResizeControlDiv);
			this.leftResizeControlDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			parent.Controls.Add(LeftResizeControlDiv);
			this.rightResizeControlDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			parent.Controls.Add(RightResizeControlDiv);
			this.bottomResizeControlDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			parent.Controls.Add(BottomResizeControlDiv);
		}
		protected internal override void FinalizeCreateControlHierarchyCore(Control parent) {
		}
		public override void RenderCommonScript(StringBuilder sb, string localVarName, string clientName) {
			base.RenderCommonScript(sb, localVarName, clientName);
			sb.AppendFormat("{0}.AddContextMenuEvent(\"{1}\",\"{2}\");\n", localVarName, SelectionDiv.ClientID, ASPxSchedulerScripts.GetShowViewMenuFunction(Owner.ClientID));
			this.selectedAppointmentAdorner.RenderCommonScript(sb, localVarName);
		}
		protected internal override void PrepareControlHierarchyCore() {
			TopResizeControlDiv.ID = "topResizeControlDiv";
			LeftResizeControlDiv.ID = "leftResizeControlDiv";
			BottomResizeControlDiv.ID = "bottomResizeControlDiv";
			RightResizeControlDiv.ID = "rightResizeControlDiv";
			RenderUtils.AppendDefaultDXClassName(TopResizeControlDiv, "dxscNSR");
			RenderUtils.AppendDefaultDXClassName(BottomResizeControlDiv, "dxscNSR");
			RenderUtils.AppendDefaultDXClassName(LeftResizeControlDiv, "dxscEWR");
			RenderUtils.AppendDefaultDXClassName(RightResizeControlDiv, "dxscEWR");
			RenderUtils.AppendDefaultDXClassName(SelectionDiv, "dxscSel");
			StylesHelper helper = StylesHelper.Create(Owner.ActiveView, Owner.ViewInfo, Owner.Styles);
			SelectionDiv.BackColor = helper.GetSelectionColor();
			RenderUtils.SetOpacity(LeftResizeControlDiv, 1);
			RenderUtils.SetOpacity(RightResizeControlDiv, 1);
			RenderUtils.SetOpacity(TopResizeControlDiv, 1);
			RenderUtils.SetOpacity(BottomResizeControlDiv, 1);			
			ContentControl.Style.Add(HtmlTextWriterStyle.Position, "relative");
		}
	}
}
