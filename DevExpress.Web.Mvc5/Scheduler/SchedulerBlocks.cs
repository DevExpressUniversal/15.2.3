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

using System.Collections.Generic;
namespace DevExpress.Web.Mvc.Internal {
	using DevExpress.Web.ASPxScheduler.Internal;
	using DevExpress.Web.ASPxScheduler;
	using DevExpress.Web.Internal;
	using DevExpress.Web;
	public class MVCxBlockRelatedControlDefaultImplementation: BlockRelatedControlDefaultImplementation {
		IMasterControl masterControl;
		public MVCxBlockRelatedControlDefaultImplementation(ASPxWebControl control, IControlBlock block)
			: base(control, block) {
			this.masterControl = control as IMasterControl;
		}
		protected internal override IMasterControl LookupMasterControl() {
			return this.masterControl;
		}
	}
	public class MVCxResourceNavigatorBlock: ResourceNavigatorBlock {
		public MVCxResourceNavigatorBlock(MVCxScheduler owner)
			: base(owner) {
		}
		protected override ASPxResourceNavigator CreateResourceNavigator() {
			return new MVCxResourceNavigator((MVCxScheduler)Owner);
		}
	}
	public class MVCxViewSelectorBlock: ViewSelectorBlock {
		public MVCxViewSelectorBlock(MVCxScheduler owner)
			: base(owner) {
		}
		protected override ASPxViewSelector CreateViewSelector() {
			return new MVCxViewSelector((MVCxScheduler)Owner);
		}
	}
	public class MVCxViewNavigatorBlock: ViewNavigatorBlock {
		public MVCxViewNavigatorBlock(MVCxScheduler owner)
			: base(owner) {
		}
		protected override ASPxViewNavigator CreateViewNavigator() {
			return new MVCxViewNavigator((MVCxScheduler)Owner);
		}
	}
	public class MVCxViewVisibleIntervalBlock: ViewVisibleIntervalBlock {
		public MVCxViewVisibleIntervalBlock(MVCxScheduler owner)
			: base(owner) {
		}
		protected override ASPxViewVisibleInterval CreateViewVisibleInterval() {
			return new MVCxViewVisibleInterval((MVCxScheduler)Owner);
		}
	}
	public class MVCxSchedulerStatusInfoManagerBlock: SchedulerStatusInfoManagerBlock {
		public MVCxSchedulerStatusInfoManagerBlock(MVCxScheduler owner)
			: base(owner) {
		}
		protected override ASPxSchedulerStatusInfoManager CreateSchedulerStatusInfoManager() {
			return new MVCxSchedulerStatusInfoManager((MVCxScheduler)Owner);
		}
	}
	public class MVCxViewMenuBlock: ViewMenuBlock {
		public MVCxViewMenuBlock(MVCxScheduler owner)
			: base(owner) {
		}
		protected override BlockRelatedControlDefaultImplementation CreateDefaultRelatedControlImplementation() {
			return new MVCxBlockRelatedControlDefaultImplementation(Owner, this);
		}
	}
	public class MVCxFormBlock: FormBlock {
		public MVCxFormBlock(MVCxScheduler owner)
			: base(owner) {
		}
		public new MVCxScheduler Owner { get { return (MVCxScheduler)base.Owner; } }
		protected override FormRenderer CreateFormRender() {
			switch (Owner.ActiveFormType) {
				case SchedulerFormType.Appointment:
					return new MVCxAppointmentFormRenderer(Owner);
				case SchedulerFormType.AppointmentInplace:
					return new MVCxAppointmentInplaceEditorRenderer(Owner);
				case SchedulerFormType.GotoDate:
					return new MVCxGotoDateFormRenderer(Owner);
				case SchedulerFormType.RecurrentAppointmentDelete:
					return new MVCxRecurrentAppointmentDeleteFormRenderer(Owner);
				case SchedulerFormType.RecurrentAppointmentEdit:
					return new MVCxRecurrentAppointmentEditFormRenderer(Owner);
				case SchedulerFormType.Reminder:
					return new MVCxRemindersFormRenderer(Owner);
			}
			return FormRenderer.CreateInstance(Owner.ActiveFormType, Owner);
		}
	}
	public class MVCxSchedulerControlScriptBlock: ASPxSchedulerControlScriptBlock {
		public MVCxSchedulerControlScriptBlock(MVCxScheduler control, IScriptBlockOwner scriptBuilderOwner)
			: base(control, scriptBuilderOwner) {
		}
		public new MVCxScheduler Owner { get { return (MVCxScheduler)base.Owner; } }
		public override void RenderCommonScript(System.Text.StringBuilder sb, string localVarName, string clientName) {
			base.RenderCommonScript(sb, localVarName, clientName);
			var recurrenceFormName = GetApptRecurrenceFormName();
			if (!string.IsNullOrEmpty(recurrenceFormName))
				sb.Append(localVarName + ".recurrenceFormName=\"" + recurrenceFormName + "\";\n");
			var names = GetEditorsNamesForAppointmentForm();
			if (names != null)
				sb.Append(localVarName + ".editableAppoinmentEditors=eval(\"" + HtmlConvertor.ToJSON(names) + "\");\n");
			sb.Append(localVarName + ".initClientTimeZoneId=\"" + Owner.OptionsBehavior.ClientTimeZoneId + "\";\n");
			if(!string.IsNullOrEmpty(Owner.DateNavigatorId))
				sb.Append(localVarName + ".dateNavigatorId=\"" + Owner.DateNavigatorId + "\";\n");
		}
		string GetApptRecurrenceFormName() {
			var appointmentFormContainer = Owner.FormBlock.CurrentFormTemplateContainer as AppointmentFormTemplateContainer;
			if (Owner.ActiveFormType != SchedulerFormType.Appointment || appointmentFormContainer == null)
				return null;
			var recurrenceFormName = Owner.OptionsForms.RecurrenceFormName;
			if (string.IsNullOrEmpty(recurrenceFormName)) {
				var appointmentForm = appointmentFormContainer.FindControl("AppointmentRecurrenceForm1");
				recurrenceFormName = appointmentForm != null ? appointmentForm.ClientID : null;
			}
			return recurrenceFormName;
		}
		IEnumerable<string> GetEditorsNamesForAppointmentForm() {
			if (!IsRenderTemplateFormControl())
				return null;
			List<string> result = new List<string>();
			if (Owner.Storage.EnableReminders) {
				result.Add("HasReminder");
				result.AddRange(SchedulerReminderInfoHelper.GetReminderEditMemberNames());
			}
			foreach (var mapping in Owner.Storage.Appointments.ActualMappings) {
				result.Add(mapping.Member);
			}
			return result;
		}
		bool IsRenderTemplateFormControl() {
			switch (Owner.ActiveFormType) {
				case SchedulerFormType.Appointment:
					return Owner.AppointmentFormTemplateControl != null;
				case SchedulerFormType.AppointmentInplace:
					return Owner.AppointmentInplaceEditorFormTemplateControl != null;
			}
			return false;
		}
	}
	public class MVCxSchedulerMessageBoxBlock : MessageBoxBlock {
		public MVCxSchedulerMessageBoxBlock(MVCxScheduler owner)
			: base(owner) {
		}
		protected override BlockRelatedControlDefaultImplementation CreateDefaultRelatedControlImplementation() {
			return new MVCxBlockRelatedControlDefaultImplementation(Owner, this);
		}
	}
}
