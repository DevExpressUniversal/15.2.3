#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.AuditTrail;
namespace DevExpress.ExpressApp.AuditTrail {
	public class AuditTrailViewController : ObjectViewController {
		public AuditTrailViewController() {
			TargetViewNesting = Nesting.Root;
			TargetViewType = ViewType.DetailView;
		}
		private void SubscribeToEvents(View view) {
			view.ObjectSpace.Committed += new EventHandler(ObjectSpace_Committed);
			view.ObjectSpace.Reloaded += new EventHandler(ObjectSpace_Reloaded);
			view.CurrentObjectChanged += new EventHandler(View_CurrentObjectChanged);
		}
		private void UnsubscribeEvents() {
			if(View != null) {
				View.ObjectSpace.Committed -= new EventHandler(ObjectSpace_Committed);
				View.ObjectSpace.Reloaded -= new EventHandler(ObjectSpace_Reloaded);
				View.CurrentObjectChanged -= new EventHandler(View_CurrentObjectChanged);
			}
		}
		private void BeginAudit(ObjectView view) {
			if(view.CurrentObject != null) {
				AuditTrailService.Instance.BeginObjectsAudit(((XPObjectSpace)view.ObjectSpace).Session, view.CurrentObject);
			}
		}
		private void View_CurrentObjectChanged(object sender, EventArgs e) {
			BeginAudit(View);
		}
		private void ObjectSpace_Committed(object sender, EventArgs e) {
			BeginAudit(View);
		}
		private void ObjectSpace_Reloaded(object sender, EventArgs e) {
			BeginAudit(View);
		}
		protected override void OnActivated() {
			if((View.ObjectSpace is XPObjectSpace) && !(View.ObjectSpace is XPNestedObjectSpace)) {
				SubscribeToEvents(View);
				BeginAudit(View);
			}
			base.OnActivated();
		}
		protected override void OnDeactivated() {
			if(!(View.ObjectSpace is XPNestedObjectSpace)) {
				UnsubscribeEvents();
			}
			base.OnDeactivated();
		}
	}
}
