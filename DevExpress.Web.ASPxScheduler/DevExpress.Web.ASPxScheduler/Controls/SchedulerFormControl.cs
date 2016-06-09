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
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxScheduler.Internal;
using DevExpress.Web.Internal;
using DevExpress.XtraScheduler;
using System.Text;
namespace DevExpress.Web.ASPxScheduler {
	public static class InplaceEditorHiddenFieldId {
		public const string StartDate = "STRTD";
		public const string EndDate = "ENDD";
		public const string ResourceId = "RESID";
	}
	public abstract class SchedulerFormControl : ASPxSchedulerFormWithClientScriptSupportBase {
		protected SchedulerFormControl() {
			ID = SchedulerIdHelper.FormTemplateControlId;
		}
		protected override void OnLoad(EventArgs e) {
			EnsureChildControls();
			PrepareChildControls();
			base.OnLoad(e);
		}
		internal void InternalPrepareChildControls() {
			PrepareChildControls();
		}
		protected virtual void PrepareChildControls() {
			ASPxScheduler control = ObatinOwner();
			if (control == null)
				return;
			ApplyEditorsParentStyles(control);
			ApplyButtonsParentStyles(control);
		}		
		protected override void BindToOwner(StringBuilder sb, string instanceName) {
			ASPxScheduler scheduler = ObatinOwner();
			string schedulerInstanceName = GetControlClientName(scheduler);
			sb.AppendFormat("{0}.formOwnerId = '{1}';\n", instanceName, schedulerInstanceName);
		}
		internal void InternalApplyEditorsParentStyles(ASPxScheduler control) {
			ApplyEditorsParentStyles(control);
		}
		protected virtual void ApplyEditorsParentStyles(ASPxScheduler control) {
			ASPxEditBase[] edits = GetChildEditors();
			foreach (ASPxEditBase edit in edits) {
				if (edit == null)
					continue;
				edit.ParentSkinOwner = control;
				edit.ParentStyles = control.Styles.FormEditors;
				edit.ParentImages = control.Images.FormEditors;
			}
		}
		protected virtual void ApplyButtonsParentStyles(ASPxScheduler control) {
			ASPxButton[] buttons = GetChildButtons();
			foreach (ASPxButton button in buttons) {
				if (button == null)
					continue;
				button.ParentSkinOwner = control;
				button.ControlStyle.CopyFrom(control.Styles.FormButton);
				button.ParentStyles = control.Styles.Buttons;
			}
		}
		protected override Control[] GetAllChildren() {
			List<Control> result = new List<Control>();
			result.AddRange(GetChildEditors());
			result.AddRange(GetChildButtons());
			result.AddRange(GetChildControls());
			return result.ToArray();
		}
		protected ASPxScheduler ObatinOwner() {
			SchedulerFormTemplateContainer container = Parent as SchedulerFormTemplateContainer;
			if (container == null)
				return null;
			return container.Control;
		}
		protected abstract ASPxEditBase[] GetChildEditors();
		protected abstract ASPxButton[] GetChildButtons();		
	}
	public abstract class InplaceEditorBaseFormControl : SchedulerFormControl {
		HiddenField startDate; 
		HiddenField endDate;
		HiddenField resourceId;
		protected override void EnsureChildControls() {
			base.EnsureChildControls();
		}
		public override void DataBind() {
			EnsureChildControls();
			base.DataBind();
		}
		protected override void CreateChildControls() {
			base.CreateChildControls();
			this.startDate = RenderUtils.CreateHiddenField(InplaceEditorHiddenFieldId.StartDate);
			this.endDate = RenderUtils.CreateHiddenField(InplaceEditorHiddenFieldId.EndDate);
			this.resourceId = RenderUtils.CreateHiddenField(InplaceEditorHiddenFieldId.ResourceId);
			Controls.Add(StartDate);
			Controls.Add(EndDate);
			Controls.Add(ResourceId);
		}
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			AppointmentInplaceEditorTemplateContainer container = Parent as AppointmentInplaceEditorTemplateContainer;
			if (container == null)
				return;
			Appointment appointment = container.Appointment;
			StartDate.Value = appointment.Start.ToString(CultureInfo.InvariantCulture);
			EndDate.Value = appointment.End.ToString(CultureInfo.InvariantCulture);
			string[] resourceIds = SchedulerIdHelper.GenerateResourceIds(appointment.ResourceIds);
			ResourceId.Value = String.Join(",", resourceIds);
		}
		public HiddenField StartDate { get { return startDate; } }
		public HiddenField EndDate { get { return endDate; } }
		public HiddenField ResourceId { get { return resourceId; } }
	}
	public abstract class SchedulerUserControl: UserControl, IDialogFormElementRequiresLoad {
		protected SchedulerUserControl()
			: base() {
		}
		public override string ClientID {
			get { return IsMvcRender() ? ClientIDHelper.GenerateClientID(this, base.ClientID) : base.ClientID; }
		}
		protected internal ISkinOwner ParentSkinOwner { get; set; }
		protected internal static void PrepareUserControl(Control userControl, Control parent) {
			PrepareUserControl(userControl, parent, null);
		}
		protected internal static void PrepareUserControl(Control userControl, Control parent, string id) {
			if (IsMvcRender())
				PerformOnInit(userControl);
			userControl.ID = id;
			parent.Controls.Add(userControl);
			if (IsMvcRender() && !string.IsNullOrEmpty(HttpRuntime.AppDomainAppVirtualPath))
				PerformOnLoad(userControl);
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			ASPxScheduler scheduler = (ParentSkinOwner as ASPxScheduler) ?? GetSchedulerByNamingContainer();
			PrepareControls(scheduler);
		}
		protected virtual void PrepareControls(ASPxScheduler scheduler) {
		}
		ASPxScheduler GetSchedulerByNamingContainer() {
			Control control = NamingContainer;
			while(control != null) {
				ASPxScheduler scheduler = control as ASPxScheduler;
				if(scheduler != null)
					return scheduler;
				control = control.NamingContainer;
			}
			return null;
		}
		static void PerformOnInit(Control control) {
			IDialogFormElementRequiresLoad element = control as IDialogFormElementRequiresLoad;
			if (element != null) element.ForceInit();
		}
		static void PerformOnLoad(Control control) {
			IDialogFormElementRequiresLoad element = control as IDialogFormElementRequiresLoad;
			if (element != null) element.ForceLoad();
			foreach (Control childControl in control.Controls)
				PerformOnLoad(childControl);
		}
		protected static bool IsMvcRender() {
			return MvcUtils.RenderMode != MvcRenderMode.None;
		}
		#region IDialogFormElementRequiresLoad Members
		void IDialogFormElementRequiresLoad.ForceInit() {
			FrameworkInitialize();
		}
		void IDialogFormElementRequiresLoad.ForceLoad() {
			OnLoad(EventArgs.Empty);
		}
		#endregion
	}
}
