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
using System.Collections;
using System.Collections.Generic;
using System.Data.Services.Providers;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Helpers;
using System.Reflection;
using System.Data.Services;
using DevExpress.Xpo.Metadata.Helpers;
namespace DevExpress.Xpo {
	public class XpoUpdateProvider : IDataServiceUpdateProvider2 {
		readonly XpoContext dataContext;
		readonly XpoDataServiceV3 dataService;
		readonly UnitOfWork session;
		readonly List<IDataServiceInvokable> actionsToInvoke = new List<IDataServiceInvokable>();
		readonly ObjectSet objectsInProcess = new ObjectSet();
		public XpoUpdateProvider(UnitOfWork session, XpoDataServiceV3 dataService) {
			this.dataService = dataService;
			this.dataContext = dataService.Context;
			this.session = session;
		}
		protected XpoContext DataContext {
			get { return this.dataContext; }
		}
		#region IUpdatable Members
		public void AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded) {
			dataService.ChangeInterceptor(targetResource.GetType(), UpdateOperations.Change, dataService.Token);
			IXPSimpleObject xpObject = FixSession(targetResource) as IXPSimpleObject;
			XPMemberInfo mi = xpObject.ClassInfo.GetMember(propertyName);
			IList list = mi.GetValue(xpObject) as IList;
			object resourceToBeAddedFixed = FixSession(resourceToBeAdded);
			list.Add(resourceToBeAddedFixed);
			AddObjectToProcessList(xpObject);
			AddObjectToProcessList(resourceToBeAddedFixed);
		}
		public void RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved) {
			dataService.ChangeInterceptor(targetResource.GetType(), UpdateOperations.Change, dataService.Token);
			IXPSimpleObject xpObject = FixSession(targetResource) as IXPSimpleObject;
			XPMemberInfo mi = xpObject.ClassInfo.GetMember(propertyName);
			IList list = mi.GetValue(xpObject) as IList;
			object resourceToBeRemovedFixed = FixSession(resourceToBeRemoved);
			list.Remove(resourceToBeRemovedFixed);
			AddObjectToProcessList(xpObject);
			AddObjectToProcessList(resourceToBeRemovedFixed);
		}
		public void ClearChanges() {
			objectsInProcess.Clear();
			session.DropChanges();			
		}
		public object CreateResource(string containerName, string fullTypeName) {			
			ResourceType resourceType;
			string resourceTypeName;
			if (!string.IsNullOrEmpty(fullTypeName)) {
				resourceTypeName = TypeSystem.ProcessTypeName(DataContext.NamespaceName, fullTypeName);
			} else {
				resourceTypeName = containerName;				
			}
			if (!dataContext.Metadata.TryResolveResourceType(resourceTypeName, out resourceType)) {
				throw new ArgumentException(String.Format(DevExpress.Xpo.Extensions.Properties.Resources.UnknownResourceType, fullTypeName));
			}
			dataService.ChangeInterceptor(resourceType.InstanceType, UpdateOperations.Add, dataService.Token);
			object newObject = null;
			if(containerName != null) {
				if(resourceType.ResourceTypeKind != ResourceTypeKind.EntityType) {
					throw new ArgumentException(String.Format(DevExpress.Xpo.Extensions.Properties.Resources.TheSpecifiedResourceTypeIsNotAnEntityType, fullTypeName));
				}
				newObject = resourceType.GetAnnotation().ClassInfo.CreateNewObject(session);
			} else {
				if(resourceType.ResourceTypeKind != ResourceTypeKind.ComplexType) {
					throw new ArgumentException(String.Format(DevExpress.Xpo.Extensions.Properties.Resources.TheSpecifiedResourceTypeIsNotAComplexType, fullTypeName));
				}
				Type typeToCreate = resourceType.InstanceType.GetGenericArguments()[0];
				ConstructorInfo constructorWithSession = typeToCreate.GetConstructor(new Type[] { typeof(Session) });
				if(constructorWithSession == null)
					newObject = Activator.CreateInstance(typeToCreate);
				else
					newObject = constructorWithSession.Invoke(new object[] { session });
			}
			AddObjectToProcessList(newObject);
			return newObject;
		}
		public void DeleteResource(object targetResource) {
			dataService.ChangeInterceptor(targetResource.GetType(), UpdateOperations.Delete, dataService.Token);
			object resourceToDelete = FixSession(targetResource);
			AddObjectToProcessList(resourceToDelete);
			session.Delete(resourceToDelete);
		}
		public object GetResource(System.Linq.IQueryable query, string fullTypeName) {
			object resource = null;
			foreach (object r in query) {
				if (resource != null) {
					throw new ArgumentException(String.Format(DevExpress.Xpo.Extensions.Properties.Resources.TheQueryMustReferToASingleResource, query.ToString()));
				}
				resource = r;
			}
			if(resource == null) throw new ArgumentException(String.Format(DevExpress.Xpo.Extensions.Properties.Resources.TheQueryMustReferToASingleResource, query.ToString()));
			if (fullTypeName != null) {
				string typeName = TypeSystem.ProcessTypeName(dataContext.NamespaceName, fullTypeName);
				ResourceType resourceType;
				if (!this.dataContext.Metadata.TryResolveResourceType(typeName, out resourceType)) {
					throw new ArgumentException(String.Format(DevExpress.Xpo.Extensions.Properties.Resources.UnknownResourceType, fullTypeName));
				}
				if (resource.GetType() != resourceType.InstanceType) {
					throw new System.ArgumentException(String.Format(DevExpress.Xpo.Extensions.Properties.Resources.DifferentTypesException, fullTypeName, resource.GetType().FullName));
				}
			}
			return resource;
		}
		public object GetValue(object targetResource, string propertyName) {
			return ODataHelpers.GetPropertyValue(dataContext.Metadata, targetResource, propertyName);
		}
		public object ResetResource(object resource) {
			dataService.ChangeInterceptor(resource.GetType(), UpdateOperations.Change, dataService.Token);
			IXPSimpleObject xpObject = FixSession(resource) as IXPSimpleObject;
			ResourceType resType;
			if (dataContext.Metadata.TryResolveResourceTypeByType(xpObject.ClassInfo.ClassType, out resType)) {
				XPMemberInfo mi;
				foreach (var resourceProperty in resType.Properties) {
					if ((resourceProperty.Kind & ResourcePropertyKind.Key) != ResourcePropertyKind.Key) {
						mi = xpObject.ClassInfo.GetMember(resourceProperty.Name);
						mi.SetValue(resource, null);
					}
				}
			}
			AddObjectToProcessList(resource);
			return resource;
		}
		public object ResolveResource(object resource) {
			return resource;
		}
		public void SaveChanges() {
			foreach (IDataServiceInvokable current in this.actionsToInvoke) {
				current.Invoke();
			}			
			ArrayList objectsToSave = new ArrayList(session.GetObjectsToSave());
			foreach (object savingObject in objectsToSave) {
				if (!objectsInProcess.Contains(savingObject) && !(savingObject is IntermediateObject)) session.RemoveFromSaveList(savingObject);
			}
			session.CommitChanges();
		}
		public void SetReference(object targetResource, string propertyName, object propertyValue) {
			SetValue(targetResource, propertyName, FixSession(propertyValue));
		}
		public void SetValue(object targetResource, string propertyName, object propertyValue) {
			dataService.ChangeInterceptor(targetResource.GetType(), UpdateOperations.Change, dataService.Token);
			IXPSimpleObject xpObject = FixSession(targetResource) as IXPSimpleObject;
			if (xpObject != null) {
				XPMemberInfo mi = xpObject.ClassInfo.GetMember(propertyName);
				if (mi.IsReadOnly) 
					return;
				object currentValue = ODataHelpers.GetPropertyValue(dataContext.Metadata, xpObject, propertyName);
				if (!object.Equals(currentValue, propertyValue)) {					
					AddObjectToProcessList(xpObject);
					mi.SetValue(xpObject, mi.Converter != null ? mi.Converter.ConvertFromStorageType(propertyValue) : propertyValue);
				}
				currentValue = null;
				return;
			}
			StructWrapperInfo structWrapper;
			if (dataContext.Metadata.TryResolveStructWrapper(targetResource.GetType(), out structWrapper)) {
				structWrapper.SetValue(targetResource, propertyName, propertyValue);
			}
		}
		#endregion
		#region IDataServiceUpdateProvider Members
		public void SetConcurrencyValues(object resourceCookie, bool? checkForEquality, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, object>> concurrencyValues) {
			throw new NotImplementedException();
		}
		#endregion
		#region IDataServiceUpdateProvider2 Members
		public void ScheduleInvokable(IDataServiceInvokable invokable) {
			if (invokable != null) actionsToInvoke.Add(invokable);
		}
		#endregion
		object FixSession(object targetObject) {
			IXPSimpleObject simpleObject = targetObject as IXPSimpleObject;
			if (simpleObject == null || simpleObject.Session == session) return targetObject;
			return session.GetObjectByKey(simpleObject.ClassInfo, simpleObject.ClassInfo.KeyProperty.GetValue(simpleObject));
		}
		void AddObjectToProcessList(object obj) {
			if(!objectsInProcess.Contains(obj)) objectsInProcess.Add(obj);
		}
	}
}
