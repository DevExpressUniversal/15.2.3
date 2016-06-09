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
using System.ComponentModel;
using DevExpress.ExpressApp;
namespace DevExpress.ExpressApp.Security {
	public class RefreshSecurityController : ObjectViewController {
		private const string ActiveKeySecurityIsNotNull = "SecurityIsNotNull";
		private const string ActiveKeySecurityUserTypeIsNotNull = "SecurityUserTypeIsNotNull";
		private bool isCurrentUserChanged;
		private void ObjectSpace_Committed(object sender, EventArgs e) {
			if(isCurrentUserChanged) {
				ReloadSecurity();
				isCurrentUserChanged = false;
			}
		}
		private void ObjectSpace_Committing(object sender, CancelEventArgs e) {
			isCurrentUserChanged = false;
			foreach(object obj in ((IObjectSpace)sender).ModifiedObjects) {
				if(IsCurrentUserObject(obj)) {
					isCurrentUserChanged = true;
				}
			}
		}
		private void View_CurrentObjectChanged(object sender, EventArgs e) {
			RefrechEventHandlers();
		}
		private void RefrechEventHandlers() {
			RemoveEventHandlers();
			if(IsCurrentUserObject(View.CurrentObject)) {
				AddEventHandlers();
			}
		}
		private bool IsCurrentUserObject(object obj) {
			if(obj == null || SecuritySystem.UserType == null) {
				return false;
			}
			return SecuritySystem.UserType.IsAssignableFrom(obj.GetType()) && !ObjectSpace.IsNewObject(obj) && object.Equals(SecuritySystem.CurrentUserId, ObjectSpace.GetKeyValue(obj));
		}
		private void AddEventHandlers() {
			View.ObjectSpace.Committing += new EventHandler<CancelEventArgs>(ObjectSpace_Committing);
			View.ObjectSpace.Committed += new EventHandler(ObjectSpace_Committed);
		}
		private void RemoveEventHandlers() {
			View.ObjectSpace.Committing -= new EventHandler<CancelEventArgs>(ObjectSpace_Committing);
			View.ObjectSpace.Committed -= new EventHandler(ObjectSpace_Committed);
		}
		internal protected virtual void ReloadSecurity() {
			if(SecuritySystem.Instance != null) {
				((ISecurityStrategyBase)SecuritySystem.Instance).ReloadPermissions();
			}
		}
		protected override void OnViewChanging(View view) {
			Active[ActiveKeySecurityIsNotNull] = (SecuritySystem.Instance != null);
			if(SecuritySystem.Instance != null) {
				Active[ActiveKeySecurityUserTypeIsNotNull] = (SecuritySystem.Instance.UserType != null);
				TargetObjectType = SecuritySystem.Instance.UserType;
			}
			base.OnViewChanging(view);
		}
		protected override void OnActivated() {
			base.OnActivated();
			View.CurrentObjectChanged += new EventHandler(View_CurrentObjectChanged);
			RefrechEventHandlers();
		}
		protected override void OnDeactivated() {
			View.CurrentObjectChanged -= new EventHandler(View_CurrentObjectChanged);
			RemoveEventHandlers();
			base.OnDeactivated();
		}
		public RefreshSecurityController() {
			TargetViewNesting = Nesting.Root;
			TypeOfView = typeof(ObjectView);
		}
	}
}
