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
using System.Text;
using DevExpress.Web.ASPxScheduler.Drawing;
using System.Web.UI;
namespace DevExpress.Web.ASPxScheduler.Internal {
	public class ToolTipControlBlock : ASPxSchedulerControlBlock {
		ASPxSchedulerSelectionToolTtipControl selectionTooltipControl;
		ASPxSchedulerAppointmentToolTipControl appointmentTooltipControl;
		ASPxSchedulerAppointmentDragToolTipControl appointmentDragTooltipControl;
		public ToolTipControlBlock(ASPxScheduler owner)
			: base(owner) {
		}
		#region Properties
		public override string ContentControlID { get { return "toolTipBlock"; } }
		public override ASPxSchedulerChangeAction RenderActions { get { return ASPxSchedulerChangeAction.RenderCommonControls; } } 
		internal ASPxSchedulerSelectionToolTtipControl SelectionTooltipControl { get { return selectionTooltipControl; } }
		internal ASPxSchedulerAppointmentToolTipControl AppointmentTooltipControl { get { return appointmentTooltipControl; } }
		internal ASPxSchedulerAppointmentDragToolTipControl AppointmentDragTooltipControl { get { return appointmentDragTooltipControl; } }
		#endregion
		protected internal override void CreateControlHierarchyCore(System.Web.UI.Control parent) {
			this.selectionTooltipControl = CreateSchedulerSelectionToolTtipControl();
			parent.Controls.Add(SelectionTooltipControl);
			this.appointmentTooltipControl = CreateSchedulerAppointmentToolTipControl();
			parent.Controls.Add(AppointmentTooltipControl);
			this.selectionTooltipControl.ID = "selectionToolTipDiv";
			this.appointmentTooltipControl.ID = "appointmentToolTipDiv";
			this.appointmentDragTooltipControl = CreateSchedulerAppointmentDragToolTipControl();
			parent.Controls.Add(AppointmentDragTooltipControl);
			AppointmentDragTooltipControl.ID = "toolTipControl";
		}
		protected virtual ASPxSchedulerAppointmentToolTipControl CreateSchedulerAppointmentToolTipControl() {
			return new ASPxSchedulerAppointmentToolTipControl(Owner);
		}
		protected virtual ASPxSchedulerSelectionToolTtipControl CreateSchedulerSelectionToolTtipControl() {
			return new ASPxSchedulerSelectionToolTtipControl(Owner);
		}
		protected virtual ASPxSchedulerAppointmentDragToolTipControl CreateSchedulerAppointmentDragToolTipControl(){
			return new ASPxSchedulerAppointmentDragToolTipControl(Owner);
		}
		protected internal override void FinalizeCreateControlHierarchyCore(System.Web.UI.Control parent) {
		}
		public override void RenderPostbackScriptEnd(StringBuilder sb, string localVarName, string clientName) {
			base.RenderPostbackScriptEnd(sb, localVarName, clientName);
			RenderToolTipScript(sb, localVarName, AppointmentTooltipControl);
			RenderToolTipScript(sb, localVarName, AppointmentDragTooltipControl);
			RenderToolTipScript(sb, localVarName, SelectionTooltipControl);
		}
		void RenderToolTipScript(StringBuilder sb, string localVarName, ASPxSchedulerToolTipControlBase toolTip) {
			sb.AppendFormat("{0}.{1}='{2}';\n", localVarName, toolTip.GetClientName(), toolTip.ClientID);
		}
		protected internal override void PrepareControlHierarchyCore() {
			AppointmentTooltipControl.Style.Add(HtmlTextWriterStyle.Position, "absolute");
			ContentControl.Style.Add(HtmlTextWriterStyle.Position, "absolute");
			AppointmentDragTooltipControl.ControlStyle.CopyFrom(Owner.Styles.GetToolTipSquaredCornersStyle());
		}
	}
}
