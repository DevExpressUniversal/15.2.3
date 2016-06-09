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

using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
namespace DevExpress.ExpressApp.SystemModule {
	public class ListViewControllerBase : ViewController<ListView> {
		protected override void OnActivated() {
			SubscribeToListEditorEventCore();
			View.EditorChanging += new EventHandler(View_EditorChanging);
			View.EditorChanged += new EventHandler(View_EditorChanged);
			base.OnActivated();
		}
		protected override void OnDeactivated() {
			View.EditorChanging -= new EventHandler(View_EditorChanging);
			View.EditorChanged -= new EventHandler(View_EditorChanged);
			UnsubscribeToListEditorEventCore();
			base.OnDeactivated();
		}
		private void View_EditorChanged(object sender, EventArgs e) {
			SubscribeToListEditorEventCore();
		}
		private void View_EditorChanging(object sender, EventArgs e) {
			UnsubscribeToListEditorEventCore();
		}
		private void SubscribeToListEditorEventCore() {
			if(View.Editor != null) {
				UnsubscribeToListEditorEvent();
				SubscribeToListEditorEvent();
			}
		}
		private void UnsubscribeToListEditorEventCore() {
			UnsubscribeToListEditorEvent();
		}
		protected bool CanRead(object targetObject, string memberName, CollectionSourceBase collectionSource) {
			IObjectSpace objectSpace = collectionSource.ObjectSpace;
			if(SecuritySystem.Instance is IRequestSecurity && objectSpace != null && collectionSource.ObjectTypeInfo.IsPersistent && !objectSpace.IsNewObject(targetObject)) {
				return CanReadCore(targetObject, objectSpace.GetObjectHandle(targetObject), memberName);
			}
			else {
				Type objectType = objectSpace.GetObjectType(targetObject);
				return DataManipulationRight.CanRead(objectType, memberName, targetObject, collectionSource, objectSpace);
			}
		}
		protected virtual bool CanReadCore(object targetObject, string objectHandle, string memberName) {
			return false;
		}
		protected bool HasMember(object targetObject, string memberName, CollectionSourceBase collectionSource) {
			if((collectionSource.DataAccessMode == CollectionSourceDataAccessMode.DataView) && (targetObject is XafDataViewRecord)) {
				return ((XafDataViewRecord)targetObject).ContainsMember(memberName);
			}
			else {
				IMemberInfo memberInfo = collectionSource.ObjectTypeInfo.FindMember(memberName);
				return memberInfo != null && memberInfo.Owner.Type.IsAssignableFrom(targetObject.GetType());
			}
		}
		protected bool GetPermissionRequestsResult(object targetObject, string objectHandle, string memberName, List<string> visibleMembers, CollectionSourceBase collectionSource) {   
			if(CustomGetPermissionRequestsResult != null) {
				GetPermissionRequestsResultEventArgs args = new GetPermissionRequestsResultEventArgs(collectionSource.ObjectSpace.GetObjectType(targetObject), memberName, targetObject, collectionSource, collectionSource.ObjectSpace);
				CustomGetPermissionRequestsResult(this, args);
				if(args.Handled) {
					return args.Result;
				}
			}
			Type objType = collectionSource.ObjectSpace.GetObjectType(targetObject);		  
			return DataManipulationRight.HasPermissionTo(objType, visibleMembers[visibleMembers.IndexOf(memberName)], targetObject, collectionSource.ObjectSpace, SecurityOperations.Read);		   
		}
		protected virtual void SubscribeToListEditorEvent() {
		}
		protected virtual void UnsubscribeToListEditorEvent() {
		}
		public event EventHandler<GetPermissionRequestsResultEventArgs> CustomGetPermissionRequestsResult;
	}
	public class GetPermissionRequestsResultEventArgs : HandledEventArgs {
		public GetPermissionRequestsResultEventArgs(Type objectType, String memberName, Object targetObject, CollectionSourceBase collectionSource, IObjectSpace objectSpace) {
			TargetObject = targetObject;
			ObjectType = objectType;
			MemberName = memberName;
			ObjectSpace = objectSpace;
			CollectionSource = collectionSource;
		}
		public CollectionSourceBase CollectionSource;
		public object TargetObject;
		public Type ObjectType;
		public string MemberName;
		public IObjectSpace ObjectSpace;
		public bool Result { get; set; }
	}
}
