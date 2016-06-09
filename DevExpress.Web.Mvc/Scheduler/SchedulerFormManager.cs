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
	using DevExpress.Web.ASPxScheduler.Internal;
	public class MVCxAppointmentFormRenderer: AppointmentFormRenderer {
		protected internal MVCxAppointmentFormRenderer(MVCxScheduler control)
			: base(control) {
		}
		protected internal new MVCxScheduler Control { get { return (MVCxScheduler)base.Control; } }
		protected Control AppointmentTemplateControl { get { return Control.AppointmentFormTemplateControl; } }
		protected override bool IsCreateUserControl(string formUrl) {
			return AppointmentTemplateControl != null;
		}
		protected override Control CreateUserControl(string formUrl) {
			return AppointmentTemplateControl;
		}
	}
	public class MVCxAppointmentInplaceEditorRenderer: AppointmentInplaceEditorRenderer {
		protected internal MVCxAppointmentInplaceEditorRenderer(MVCxScheduler control)
			: base(control) {
		}
		protected internal new MVCxScheduler Control { get { return (MVCxScheduler)base.Control; } }
		protected Control AppointmentInplaceEditorFormTemplateControl { get { return Control.AppointmentInplaceEditorFormTemplateControl; } }
		protected override bool IsCreateUserControl(string formUrl) {
			return AppointmentInplaceEditorFormTemplateControl != null;
		}
		protected override Control CreateUserControl(string formUrl) {
			return AppointmentInplaceEditorFormTemplateControl;
		}
	}
	public class MVCxGotoDateFormRenderer: GotoDateFormRenderer {
		protected internal MVCxGotoDateFormRenderer(MVCxScheduler control)
			: base(control) {
		}
		protected internal new MVCxScheduler Control { get { return (MVCxScheduler)base.Control; } }
		protected internal Control GotoDateFormTemplateControl { get { return Control.GotoDateFormTemplateControl; } }
		protected override bool IsCreateUserControl(string formUrl) {
			return GotoDateFormTemplateControl != null;
		}
		protected override Control CreateUserControl(string formUrl) {
			return GotoDateFormTemplateControl;
		}
	}
	public class MVCxRecurrentAppointmentDeleteFormRenderer: RecurrentAppointmentDeleteFormRenderer {
		protected internal MVCxRecurrentAppointmentDeleteFormRenderer(MVCxScheduler control)
			: base(control) {
		}
		protected internal new MVCxScheduler Control { get { return (MVCxScheduler)base.Control; } }
		protected internal Control RecurrentAppointmentDeleteFormControl { get { return Control.RecurrentAppointmentDeleteFormControl; } }
		protected override bool IsCreateUserControl(string formUrl) {
			return RecurrentAppointmentDeleteFormControl != null;
		}
		protected override Control CreateUserControl(string formUrl) {
			return RecurrentAppointmentDeleteFormControl;
		}
	}
	public class MVCxRecurrentAppointmentEditFormRenderer: RecurrentAppointmentEditFormRenderer {
		protected internal MVCxRecurrentAppointmentEditFormRenderer(MVCxScheduler control)
			: base(control) {
		}
		protected internal new MVCxScheduler Control { get { return (MVCxScheduler)base.Control; } }
		protected internal Control RecurrentAppointmentEditFormTemplateContentControl { get { return Control.RecurrentAppointmentEditFormTemplateContentControl; } }
		protected override bool IsCreateUserControl(string formUrl) {
			return RecurrentAppointmentEditFormTemplateContentControl != null;
		}
		protected override Control CreateUserControl(string formUrl) {
			return RecurrentAppointmentEditFormTemplateContentControl;
		}
	}
	public class MVCxRemindersFormRenderer: RemindersFormRenderer {
		protected internal MVCxRemindersFormRenderer(MVCxScheduler control)
			: base(control) {
		}
		protected internal new MVCxScheduler Control { get { return (MVCxScheduler)base.Control; } }
		protected internal Control RemindersFormTemplateContentControl { get { return Control.RemindersFormTemplateContentControl; } }
		protected override bool IsCreateUserControl(string formUrl) {
			return RemindersFormTemplateContentControl != null;
		}
		protected override Control CreateUserControl(string formUrl) {
			return RemindersFormTemplateContentControl;
		}
	}
}
