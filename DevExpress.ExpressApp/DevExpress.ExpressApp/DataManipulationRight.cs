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
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp {
	public static class DataManipulationRight {
		public static Type GetTargetObjectType(Type type, object targetObject) {
			if(targetObject != null) {
				return targetObject.GetType();
			}
			return type;
		}
		private static bool IsNewObject(IObjectSpace objectSpace, object targetObject) {
			return objectSpace.IsNewObject(targetObject);
		}
		public static string GetTargetObjectHandle(IObjectSpace objectSpace, object targetObject) {
			if(targetObject != null && objectSpace != null) {
				ITypeInfo targetObjectType = objectSpace.TypesInfo.FindTypeInfo(targetObject.GetType());
				if(targetObjectType == null || !targetObjectType.IsPersistent || IsNewObject(objectSpace, targetObject)) {
					return null;
				}
				return objectSpace.GetObjectHandle(targetObject);
			}
			return null;
		}
		private static IBindingList GetBindingList(View view) {
			ListView listView = view as ListView;
			if((listView != null) && (listView.CollectionSource != null) && (listView.CollectionSource.List) != null) {
				return listView.CollectionSource.List as IBindingList;
			}
			return null;
		}
		public static Boolean IsAddToCollectionAllowed(View view, Type type, out String diagnosticInfo) {
			diagnosticInfo = "";
			ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(type);
			if(view is ListView) {
				if(!((ListView)view).ObjectTypeInfo.IsAssignableFrom(typeInfo)) {
					diagnosticInfo = ((ListView)view).ObjectTypeInfo.Name + " is not assignable from " + typeInfo.Name;
					return false;
				}
				else {
					ListView listView = (ListView)view;
					Boolean collectionSourceAllowAdd = false;
					if(listView.CollectionSource != null) {
						collectionSourceAllowAdd = listView.CollectionSource.GetAllowAdd(out diagnosticInfo);
					}
					else {
						diagnosticInfo = listView.Id + " CollectionSource is null";
					}
					return collectionSourceAllowAdd;
				}
			}
			return true;
		}
		public static Boolean IsRemoveFromCollectionAllowed(View view, out String diagnosticInfo) {
			if(view is ListView) {
				ListView listView = (ListView)view;
				if(listView.CollectionSource != null) {
					return listView.CollectionSource.GetAllowRemove(out diagnosticInfo);
				}
			}
			diagnosticInfo = "";
			return true;
		}
		public static Boolean CanInstantiate(Type objectType, IObjectSpace objectSpace) {
			if(objectSpace != null) {
				return objectSpace.CanInstantiate(objectType);
			}
			return XafTypesInfo.Instance.CanInstantiate(objectType);
		}		
		public static Boolean HasPermissionTo(Type objectType, String memberName, Object targetObject, IObjectSpace objectSpace, string operation) {
			Type targetObjectType = objectType;
			if(!(targetObject is XafDataViewRecord)) {
				targetObjectType = DataManipulationRight.GetTargetObjectType(objectType, targetObject);
			}
			PermissionRequest request = new PermissionRequest(objectSpace, targetObjectType, operation, targetObject, memberName);
			return SecuritySystem.IsGranted(request);
		}
		public static Boolean HasPermissionTo(Type objectType, ObjectAccess objectAccessType, String memberName, Object targetObject, CollectionSourceBase collectionSource) {
			SecurityContextList securityContexts = new SecurityContextList();
			if(targetObject != null) {
				securityContexts.TargetObjectContext = new TargetObjectContext(targetObject);
			}
			if(!String.IsNullOrEmpty(memberName)) {
				securityContexts.TargetMemberContext = new TargetMemberContext(memberName);
			}
			PropertyCollectionSource propertyCollectionSource = collectionSource as PropertyCollectionSource;
			if(propertyCollectionSource != null) {
				securityContexts.CollectionPropertyContext = new CollectionPropertyContext(propertyCollectionSource.MemberInfo.Owner.Type, propertyCollectionSource.MasterObject, propertyCollectionSource.MemberInfo.Name);
			}
			if(!HasPermissionTo(objectType, objectAccessType, securityContexts)) {
				return false;
			}
			if(!String.IsNullOrEmpty(memberName)) {
				if((objectAccessType == ObjectAccess.Read) || (objectAccessType == ObjectAccess.Write)) {
					if(!ObjectAccessComparerBase.CurrentComparer.IsMemberReadGranted(objectType, memberName, securityContexts)) {
						return false;
					}
					if((objectAccessType == ObjectAccess.Write) && (targetObject != null)) {
						ITypeInfo targetObjectType = XafTypesInfo.Instance.FindTypeInfo(targetObject.GetType());
						if(ObjectAccessComparerBase.CurrentComparer.IsMemberModificationDenied(targetObject, targetObjectType.FindMember(memberName))) {
							return false;
						}
					}
				}
				else {
					throw new ArgumentException(objectAccessType.ToString(), "objectAccessType");
				}
			}
			return true;
		}
		public static Boolean HasPermissionTo(Type objectType, ObjectAccess objectAccessType, SecurityContextList contexts) {
			return SecuritySystem.IsGranted(new ObjectAccessPermission(objectType, objectAccessType, contexts));
		}
		public static Boolean CanCreate(View view, Type objectType, CollectionSourceBase collectionSource, out String diagnosticInfo) {
			ITypeInfo objTypeInfo = XafTypesInfo.Instance.FindTypeInfo(objectType);
			return CanCreate(view, objTypeInfo, collectionSource, out diagnosticInfo);
		}
		public static Boolean CanCreate(ListView listView, out String diagnosticInfo) {
			return CanCreate(listView, listView.ObjectTypeInfo, listView.CollectionSource, out diagnosticInfo);
		}
		public static Boolean CanCreate(View view, ITypeInfo objectTypeInfo, CollectionSourceBase collectionSource, out String diagnosticInfo) {
			diagnosticInfo = "";
			if(view is ObjectView) {
				if(!view.AllowNew) {
					diagnosticInfo = "Can not create a new object because " + view.Id + ".AllowNew = False";
					return false;
				}
				if(!((ObjectView)view).ObjectTypeInfo.IsAssignableFrom(objectTypeInfo)) {
					diagnosticInfo = ((ObjectView)view).ObjectTypeInfo.Name + " is not assignable from " + objectTypeInfo.Name;
					return false;
				}
				if(!IsAddToCollectionAllowed(view, objectTypeInfo.Type, out diagnosticInfo)) {
					return false;
				}
			}
			IObjectSpace objectSpace = (view != null) ? view.ObjectSpace : null;
			if(objectSpace == null) {
				objectSpace = (collectionSource != null) ? collectionSource.ObjectSpace : null;
			} 
			bool isGrantedBySecurity = (SecuritySystem.Instance is IRequestSecurity) ?
				HasPermissionTo(objectTypeInfo.Type, null, null, objectSpace, SecurityOperations.Create) : HasPermissionTo(objectTypeInfo.Type, ObjectAccess.Create, null, null, collectionSource);
			if(!CanInstantiate(objectTypeInfo.Type, (view != null) ? view.ObjectSpace : null) 
				|| !isGrantedBySecurity) {
				diagnosticInfo = objectTypeInfo.Name + " cannot be instantiated";
				return false;
			}
			return true;
		}
		public static Boolean CanEdit(Type objectType, String memberName, Object targetObject, CollectionSourceBase collectionSource, IObjectSpace objectSpace) {
			return (SecuritySystem.Instance is IRequestSecurity) ?
				HasPermissionTo(objectType, memberName, targetObject, objectSpace, SecurityOperations.Write) : HasPermissionTo(objectType, ObjectAccess.Write, memberName, targetObject, collectionSource);
		}
		public static Boolean CanEdit(View view, Object targetObject, CollectionSourceBase collectionSource) {
			Guard.ArgumentNotNull(view, "view");
			return
				(view is ObjectView)
				&&
				view.AllowEdit
				&& CanEdit(((ObjectView)view).ObjectTypeInfo.Type, null, targetObject, collectionSource, view.ObjectSpace);
		}
		public static Boolean CanRead(Type objectType, String propertyName, Object targetObject, CollectionSourceBase collectionSource, IObjectSpace objectSpace) {
			return (SecuritySystem.Instance is IRequestSecurity) ?
				HasPermissionTo(objectType, propertyName, targetObject, objectSpace, SecurityOperations.Read) : HasPermissionTo(objectType, ObjectAccess.Read, propertyName, targetObject, collectionSource);
		}
		public static Boolean CanNavigate(Type objectType, Object targetObject, IObjectSpace objectSpace) {
			return (SecuritySystem.Instance is IRequestSecurity) ?
				HasPermissionTo(objectType, null, targetObject, objectSpace, SecurityOperations.Navigate) : HasPermissionTo(objectType, ObjectAccess.Navigate, "", targetObject, null);
		}
		public static Boolean CanDelete(Type type, Object targetObject, CollectionSourceBase collectionSourceBase, IObjectSpace objectSpace) {
			return (SecuritySystem.Instance is IRequestSecurity) ?
				HasPermissionTo(type, null, targetObject, objectSpace, SecurityOperations.Delete) : HasPermissionTo(type, ObjectAccess.Delete, null, targetObject, collectionSourceBase);
		}
	}
	public class PermissionRequestToCustomIsGranted : IPermissionRequest {
		public PermissionRequestToCustomIsGranted(Type objectType, String memberName, Object targetObject, IObjectSpace objectSpace, string operation) {
			ObjectType = objectType;
			MemberName = memberName;
			TargetObject = targetObject;
			ObjectSpace = objectSpace;
			Operation = operation;
		}
		public object GetHashObject() {
			throw new NotImplementedException();
		}
		public Type ObjectType { get; set; } 
		public String MemberName { get; set; }
		public Object TargetObject { get; set; } 
		public IObjectSpace ObjectSpace { get; set; }
		public string Operation { get; set; }
	}
}
