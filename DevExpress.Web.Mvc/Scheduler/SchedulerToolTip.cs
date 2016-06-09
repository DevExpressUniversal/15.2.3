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
namespace DevExpress.Web.Mvc.Internal {
	using DevExpress.Web.ASPxScheduler.Drawing;
	using DevExpress.Web.ASPxScheduler.Internal;
	public enum MVCxSchedulerToolTipType { Appointment, AppointmentDrag, Selection }
	public class MVCxToolTipControlBlock : ToolTipControlBlock {
		public MVCxToolTipControlBlock(MVCxScheduler owner)
			: base(owner) {
		}
		protected override ASPxSchedulerAppointmentToolTipControl CreateSchedulerAppointmentToolTipControl() {
			return new MVCxSchedulerAppointmentToolTipControl(Owner);
		}
		protected override ASPxSchedulerSelectionToolTtipControl CreateSchedulerSelectionToolTtipControl() {
			return new MVCxSchedulerSelectionToolTipControl(Owner);
		}
		protected override ASPxSchedulerAppointmentDragToolTipControl CreateSchedulerAppointmentDragToolTipControl() {
			return new MVCxSchedulerAppointmentDragToolTipControl(Owner);
		}
	}
	public class MVCxSchedulerAppointmentToolTipControl : ASPxSchedulerAppointmentToolTipControl {
		protected internal static new string ToolTipScriptResourceName { get { return ASPxSchedulerAppointmentToolTipControl.ToolTipScriptResourceName; } }
		public MVCxSchedulerAppointmentToolTipControl(ASPxScheduler.ASPxScheduler control)
			: base(control) {
		}
		protected internal new MVCxScheduler Control { get { return (MVCxScheduler)base.Control; } }
		protected Control AppointmentToolTipTemplateControl { get { return Control.AppointmentToolTipTemplateControl; } }
		protected override bool IsCreateUserControl() {
			return AppointmentToolTipTemplateControl != null;
		}
		protected override Control CreateUserControl(string formUrl) {
			return AppointmentToolTipTemplateControl;
		}
		protected override bool GetStemVisibility(TooltipTemplateContainer tooltipTemplateContainer) { 
			return IsCreateUserControl() ? false : base.GetStemVisibility(tooltipTemplateContainer); 
		}
		protected override string GetTemplateContentClientInstanceName(TooltipTemplateContainer templateContainer) {
			return IsCreateUserControl() ? string.Format("new MVCxClientSchedulerTemplateToolTip({0})", (int)MVCxSchedulerToolTipType.Appointment) : base.GetTemplateContentClientInstanceName(templateContainer);
		}
	}
	public class MVCxSchedulerAppointmentDragToolTipControl : ASPxSchedulerAppointmentDragToolTipControl {
		public MVCxSchedulerAppointmentDragToolTipControl(ASPxScheduler.ASPxScheduler control)
			: base(control) {
		}
		protected internal new MVCxScheduler Control { get { return (MVCxScheduler)base.Control; } }
		protected Control AppointmentDragToolTipTemplateControl { get { return Control.AppointmentDragToolTipTemplateControl; } }
		protected override bool IsCreateUserControl() {
			return AppointmentDragToolTipTemplateControl != null;
		}
		protected override Control CreateUserControl(string formUrl) {
			return AppointmentDragToolTipTemplateControl;
		}
		protected override bool GetStemVisibility(TooltipTemplateContainer tooltipTemplateContainer) {
			return IsCreateUserControl() ? true : base.GetStemVisibility(tooltipTemplateContainer);
		}
		protected override string GetTemplateContentClientInstanceName(TooltipTemplateContainer templateContainer) {
			return IsCreateUserControl() ? "new MVCxClientSchedulerAppointmentDragTemplateToolTip()" : base.GetTemplateContentClientInstanceName(templateContainer);
		}
	}
	public class MVCxSchedulerSelectionToolTipControl : ASPxSchedulerSelectionToolTtipControl {
		public MVCxSchedulerSelectionToolTipControl(ASPxScheduler.ASPxScheduler control)
			: base(control) {
		}
		protected internal new MVCxScheduler Control { get { return (MVCxScheduler)base.Control; } }
		protected Control SelectionToolTipTemplateControl { get { return Control.SelectionToolTipTemplateControl; } }
		protected override bool IsCreateUserControl() {
			return SelectionToolTipTemplateControl != null;
		}
		protected override Control CreateUserControl(string formUrl) {
			return SelectionToolTipTemplateControl;
		}
		protected override bool GetStemVisibility(TooltipTemplateContainer tooltipTemplateContainer) {
			return IsCreateUserControl() ? false : base.GetStemVisibility(tooltipTemplateContainer);
		}
		protected override string GetTemplateContentClientInstanceName(TooltipTemplateContainer templateContainer) {
			return IsCreateUserControl() ? string.Format("new MVCxClientSchedulerTemplateToolTip({0})", (int)MVCxSchedulerToolTipType.Selection) : base.GetTemplateContentClientInstanceName(templateContainer);
		}
	}
}
