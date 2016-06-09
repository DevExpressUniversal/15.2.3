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
using DevExpress.Web;
using DevExpress.XtraScheduler.Drawing;
using System;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxScheduler.Forms.Internal;
namespace DevExpress.Web.ASPxScheduler.Internal {
	public class MessageBoxBlock : ASPxSchedulerControlBlock {
		const string MessageBoxId = "messageBoxPopup";
		public MessageBoxBlock(ASPxScheduler scheduler) : base(scheduler) {
		}
		public override ASPxSchedulerChangeAction RenderActions { get { return ASPxSchedulerChangeAction.Any; } }
		public override string ContentControlID { get { return "messageBoxBlock"; } }
		ASPxSchedulerPopupForm MessageBoxPopup { get; set; }
		public override void RenderPostbackScriptEnd(System.Text.StringBuilder sb, string localVarName, string clientName) {
			base.RenderPostbackScriptEnd(sb, localVarName, clientName);
			sb.AppendFormat("{0}.{1} = ASPx.GetControlCollection().Get('{2}');\n", localVarName, MessageBoxId, MessageBoxPopup.ClientID);
		}
		protected internal override void CreateControlHierarchyCore(Control parent) {
			MessageBoxPopup = new ASPxSchedulerPopupForm(Owner);
			MessageBoxPopup.ID = MessageBoxId;
			MessageBoxPopup.Modal = true;
			MessageBoxPopup.PopupAction = PopupAction.None;
			MessageBoxPopup.PopupHorizontalAlign = PopupHorizontalAlign.NotSet;
			MessageBoxPopup.PopupVerticalAlign = PopupVerticalAlign.NotSet;
			MessageBoxPopup.CloseAction = CloseAction.CloseButton;
			MessageBoxPopup.AutoUpdatePosition = true;
			MessageBoxPopup.AllowDragging = true;
			MessageBoxPopup.EnableViewState = false;
			MessageBoxPopup.EnableHierarchyRecreation = true;
			MessageBoxPopup.EnableClientSideAPI = true;
			parent.Controls.Add(MessageBoxPopup);
			Control control = CreateMessageBoxControl();
			if (control == null)
				return;
			SchedulerFormTemplateContainer container = new SchedulerMessageBoxTemplateContainer(Owner);
			SchedulerUserControl.PrepareUserControl(control, container, MessageBoxId);
			MessageBoxPopup.Controls.Add(container);
		}
		protected virtual Control CreateMessageBoxControl() {
			string templateUrl = Owner.OptionsForms.MessageBoxTemplateUrl;
			if (!String.IsNullOrEmpty(templateUrl))
			   return Owner.Page.LoadControl(templateUrl);
			return new SchedulerMessageBox();
		}
		protected internal override void PrepareControlHierarchyCore() {
			PopupFormStyles styles = Owner.Styles.PopupForm;
			MessageBoxPopup.ControlStyle.CopyFrom(styles.ControlStyle);
			MessageBoxPopup.CloseButtonStyle.CopyFrom(styles.CloseButton);
			MessageBoxPopup.HeaderStyle.CopyFrom(styles.Header);
			MessageBoxPopup.ContentStyle.CopyFrom(styles.Content);
			MessageBoxPopup.ModalBackgroundStyle.CopyFrom(styles.ModalBackground);
			MessageBoxPopup.CloseButtonImage.CopyFrom(Owner.Images.FormCloseButton);
		}
		protected internal override void FinalizeCreateControlHierarchyCore(Control parent) {
		}
	}
	public class SchedulerMessageBoxTemplateContainer : SchedulerFormTemplateContainer {
		public SchedulerMessageBoxTemplateContainer(ASPxScheduler scheduler) : base(scheduler) {
		}
		public override string CancelHandler {
			get { return String.Empty; }
		}
		public override string CancelScript {
			get { return String.Empty; }
		}
		protected override EventArgs CreateCommandEventArgs(object source, EventArgs e) {
			return e;
		}
	}
}
