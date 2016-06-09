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
using System.ComponentModel;
using DevExpress.Web.Internal;
using System.Collections.Generic;
namespace DevExpress.Web.ASPxScheduler.Internal {
	public class FormBlock : ASPxSchedulerControlBlock {
		SchedulerTemplateContainerBase currentFormTemplateContainer;
		SchedulerFormVisibility activeFormVisibility = SchedulerFormVisibility.None;
		public FormBlock(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public SchedulerTemplateContainerBase CurrentFormTemplateContainer {
			get { return currentFormTemplateContainer; }
			set { currentFormTemplateContainer = value; }
		}
		public override string ContentControlID { get { return "formBlock"; } }
		public override ASPxSchedulerChangeAction RenderActions { get { return ASPxSchedulerChangeAction.RenderActiveForm; } }
		#endregion
		protected virtual bool CanCreateControlHierarchy() {
			if (Owner.ActiveFormType == SchedulerFormType.Appointment && Owner.GetEditedAppointment() == null)
				return false;
			return Owner.ActiveFormType != SchedulerFormType.None;
		}
		protected internal override void CreateControlHierarchyCore(Control parent) {
			if (!CanCreateControlHierarchy())
				return;
			FormRenderer renderer = CreateFormRender(); ;
			if (renderer == null || renderer.FormContainerVisibility == SchedulerFormVisibility.None)
				return;
			this.activeFormVisibility = renderer.FormContainerVisibility;
			DataBindThroughHierarchyTerminatorControl control = new DataBindThroughHierarchyTerminatorControl();
			parent.Controls.Add(control);
			renderer.Execute(control);
		}
		protected virtual FormRenderer CreateFormRender() {
			return FormRenderer.CreateInstance(Owner.ActiveFormType, Owner);
		}
		protected internal override void FinalizeCreateControlHierarchyCore(Control parent) {
		}
		protected internal override void PrepareControlHierarchyCore() {
		}
		protected override CallbackResult CreateEmptyCallbackResult(CallbackResult originalResult) {
			if(originalResult.IsEmpty)
				return originalResult;
			return CallbackResult.CreateUsePrevResult(originalResult.ElementId, originalResult.ClientObjectId);
		}
		protected override bool IsCollapsedToZeroSize() {
			return true;
		}
		protected override bool UndoCollapsedToZeroSize() {
			return this.activeFormVisibility == SchedulerFormVisibility.FillControlArea;
		}
	}
	[ToolboxItem(false)]
	public class DataBindThroughHierarchyTerminatorControl : Control {
		public override void DataBind() {
		}
		protected override void Render(HtmlTextWriter writer) {
			RenderChildren(writer);
		}
	}
	public class InplaceEditorFormBlock : ASPxSchedulerControlBlock {
		#region Fields
		string formContainerID;
		#endregion
		public InplaceEditorFormBlock(ASPxScheduler control)
			: base(control) {
			this.formContainerID = String.Empty;
		}
		#region Properties
		public string FormContainerID { get { return formContainerID; } }
		public override string ContentControlID { get { return "inpEdtBlock"; } }
		public override ASPxSchedulerChangeAction RenderActions { get { return ASPxSchedulerChangeAction.RenderActiveForm; } }
		#endregion
		public override void RenderCommonScript(StringBuilder sb, string localVarName, string clientName) {
		}
		protected internal override void CreateControlHierarchyCore(Control parent) {
		}
		protected internal override void FinalizeCreateControlHierarchyCore(Control parent) {
		}
		protected internal override void PrepareControlHierarchyCore() {
		}
	}
}
