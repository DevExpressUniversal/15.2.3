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
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
using System.ComponentModel.Design;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.Design;
namespace DevExpress.Web.ASPxScheduler.Design {
	public class ASPxSchedulerRelatedControlDesigner : ASPxWebControlDesigner {
		protected override string GetBaseProperty() {
			return "MasterControlID";
		}
		protected override void OnDesignerLoadComplete(object sender, EventArgs e) {
			RecreateControlHierarchy();
			UpdateDesignTimeHtml();
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			base.RegisterActionLists(list);
			list.Add(new ASPxSchedulerRelatedControlDesignerActionList(this));
		}
		public override bool IsThemableControl() {
			return false;
		}
		public override bool HasClientSideEvents() {
			return false;
		}
		public override void ShowAbout() {
			ASPxSchedulerAboutDialogHelper.ShowAbout(Component.GetType(), Component.Site);
		}
	}
	public class ASPxSchedulerRelatedControlDesignerActionList : DesignerActionList {
		public ASPxSchedulerRelatedControlDesignerActionList(ASPxSchedulerRelatedControlDesigner designer)
			: base(designer.Component) {
		}
		ASPxSchedulerRelatedControl RelatedControl { get { return (ASPxSchedulerRelatedControl)this.Component; } }
		[
		IDReferenceProperty(typeof(ASPxScheduler)),
		TypeConverter("DevExpress.Web.ASPxScheduler.Design.ASPxSchedulerIDConverter, " + AssemblyInfo.SRAssemblySchedulerWebDesignFull)
		]
		public string MasterControlID {
			get { return RelatedControl.MasterControlID; }
			set {
				ControlDesigner.InvokeTransactedChange(RelatedControl, new TransactedChangeCallback(SetMasterControlIDCallback), value, ASPxSchedulerDesignSR.SetMasterControlIDTransaction);
			}
		}
		private bool SetMasterControlIDCallback(object context) {
			PropertyDescriptor descriptor = TypeDescriptor.GetProperties(RelatedControl)[ASPxSchedulerDesignSR.MasterControlID];
			descriptor.SetValue(RelatedControl, context);
			return true;
		}
	}
}
