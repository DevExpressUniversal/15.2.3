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
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base.Security;
namespace DevExpress.ExpressApp.Security {
	public class LastAdminController : ViewController {
		protected virtual void CheckLastAdmin(object savingObject) {
			ISimpleUser user = savingObject as ISimpleUser;
			if(user == null || user.IsAdministrator)
				return;
			using(IObjectSpace space = Application.CreateObjectSpace(savingObject.GetType())) {
				ISimpleUser oldValue = (ISimpleUser)space.GetObject(savingObject);
				if((oldValue != null) && oldValue.IsAdministrator) {
					bool otherAdminFound = false;
					foreach(ISimpleUser otherUser in space.CreateCollection(SecuritySystem.UserType)) {
						if(otherUser.IsAdministrator && otherUser != oldValue) {
							otherAdminFound = true;
							break;
						}
					}
					if(!otherAdminFound) {
						throw new UserFriendlySecurityException(SecurityExceptionId.LastAdmin, user.UserName);
					}
				}
			}
		}
		private void ObjectSpace_ObjectSaving(object sender, ObjectManipulatingEventArgs e) {
			CheckLastAdmin(e.Object);
		}
		protected override void OnActivated() {
			base.OnActivated();
			ISimpleUser simpleUser = SecuritySystem.CurrentUser as ISimpleUser;
			if(
					 (simpleUser != null) && simpleUser.IsAdministrator && (View is ObjectView)) {
				View.ObjectSpace.ObjectSaving += new EventHandler<ObjectManipulatingEventArgs>(ObjectSpace_ObjectSaving);
			}
		}
		protected override void OnDeactivated() {
			if(View is ObjectView) {
				View.ObjectSpace.ObjectSaving -= new EventHandler<ObjectManipulatingEventArgs>(ObjectSpace_ObjectSaving);
			}
			base.OnDeactivated();
		}
	}
}
