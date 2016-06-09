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

using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo.Metadata;
using System;
using System.Data.Services.Providers;
using System.Data.Services;
using System.Reflection;
using DevExpress.Xpo.Helpers;
using System.ServiceModel.Web;
using DevExpress.Xpo;
namespace DevExpress.Xpo {
	public class XpoContext {
		readonly string containerName;
		readonly string namespaceName;
		readonly IObjectLayer objectLayer;
		XpoMetadata metadata;		
		readonly Dictionary<string, IQueryable> resourceSetQueriesDict = new Dictionary<string, IQueryable>();
		readonly Dictionary<Type, XPQueryCreatorBase> xpQueryCreatorDict = new Dictionary<Type, XPQueryCreatorBase>();
		readonly Dictionary<Type, XPQueryExpandHelperBase> xpQueryExpandHelperDict = new Dictionary<Type, XPQueryExpandHelperBase>();
		readonly Dictionary<Type, XPQueryIQueryableToIEnumerableConverterBase> xpQueryQToEConverterDict = new Dictionary<Type, XPQueryIQueryableToIEnumerableConverterBase>();
		readonly static Dictionary<XPMemberInfo, ValueConverter> xpConverterHelperDict = new Dictionary<XPMemberInfo, ValueConverter>();		
		readonly Dictionary<string, ServiceOperation> serviceOperations;
		public IDataLayer DataLayer {
			get {
				IObjectLayerEx ole = objectLayer as IObjectLayerEx;
				return ole == null ? null : ole.DataLayer;
			}
		}
		public IObjectLayer ObjectLayer {
			get {
				return objectLayer;
			}
		}
		public XpoMetadata Metadata {
			get {
				if (metadata == null) {
					metadata = new XpoMetadata(this, containerName, namespaceName);
					metadata.SetReadOnly();
				}
				return metadata;
			}
		}
		public string NamespaceName {
			get { return namespaceName; }
		}
		public virtual ServiceOperation[] ServiceOperations { get { return this.serviceOperations.Values.ToArray(); } }
		public XpoContext(string containerName, string namespaceName, IObjectLayer objectLayer) {
			this.containerName = containerName;
			this.namespaceName = namespaceName;
			this.objectLayer = objectLayer;			
			this.serviceOperations = new Dictionary<string, ServiceOperation>();
		}
		public XpoContext(string containerName, string namespaceName, IDataLayer dataLayer)
			: this(containerName, namespaceName, new SimpleObjectLayer(dataLayer)) {
		}
		public IQueryable GetResourceSetEntities(ResourceSet resourceSet, XpoDataServiceV3 dataService) {
			lock (resourceSetQueriesDict) {
				IQueryable rootQuery;
				if (!resourceSetQueriesDict.TryGetValue(resourceSet.Name, out rootQuery)) {
					ResourceType resType = resourceSet.ResourceType as ResourceType;
					Type queryType = typeof(XpoLinqQuery<>).MakeGenericType(resType.InstanceType);
					ConstructorInfo ci = queryType.GetConstructor(new Type[] { typeof(XpoLinqQueryProvider)});
					rootQuery = (IQueryable)ci.Invoke(new object[] { new XpoLinqQueryProvider(resType, dataService) });
					resourceSetQueriesDict.Add(resourceSet.Name, rootQuery);
				}
				return rootQuery;
			}	  
		}		
		public XPQueryCreatorBase GetXPQueryCreator(Type type) {
			lock (xpQueryCreatorDict) {
				XPQueryCreatorBase queryCreator;
				if (!this.xpQueryCreatorDict.TryGetValue(type, out queryCreator)) {
					Type creatorType = typeof(XPQueryCreator<>).MakeGenericType(type);
					queryCreator = (XPQueryCreatorBase)Activator.CreateInstance(creatorType, this);
					xpQueryCreatorDict.Add(type, queryCreator);
				}
				return queryCreator;
			}
		}
		public XPQueryExpandHelperBase GetXPQueryExpandHelper(Type type) {
			if (!type.IsGenericType || !typeof(IExpandedResult).IsAssignableFrom(type.GetGenericTypeDefinition()))
				return null;
			lock (xpQueryExpandHelperDict) {
				XPQueryExpandHelperBase expandHelper;
				if (!xpQueryExpandHelperDict.TryGetValue(type, out expandHelper)) {
					Type helperType = typeof(XPQueryExpandHelper<,>).MakeGenericType(type, type.GetGenericArguments()[0]);
					expandHelper = (XPQueryExpandHelperBase)Activator.CreateInstance(helperType, this);
					xpQueryExpandHelperDict.Add(type, expandHelper);
				}
				return expandHelper;
			}
		}
		public XPQueryIQueryableToIEnumerableConverterBase GetXPQueryQToEConverter(Type type) {
			lock (xpQueryQToEConverterDict) {
				XPQueryIQueryableToIEnumerableConverterBase qToEConverter;
				if (!xpQueryQToEConverterDict.TryGetValue(type, out qToEConverter)) {
					Type helperType = typeof(XPQueryIQueryableToIEnumerableConverter<>).MakeGenericType(type);
					qToEConverter = (XPQueryIQueryableToIEnumerableConverterBase)Activator.CreateInstance(helperType);
					xpQueryQToEConverterDict.Add(type, qToEConverter);
				}
				return qToEConverter;
			}
		}
		internal bool HidePropertyInternal(XPClassInfo cInfo, XPMemberInfo mInfo) {
			if (mInfo.FindAttributeInfo(typeof(HiddenAttribute)) != null) return true;
			return HideProperty(cInfo.ClassType, mInfo.Name);
		}
		public virtual bool HideProperty(Type classType, string propertyName) {
			return false;
		}
		internal bool HideResourceSetInternal(XPClassInfo cInfo) {			
			if (cInfo.FindAttributeInfo(typeof(HiddenAttribute)) != null) return true;
			return HideResourceType(cInfo.ClassType);
		}
		public virtual bool HideResourceType(Type resourceType) {
			return false;
		}
		public virtual string SetStreamContentType(Type classType, string propertyName) {
			return string.Empty;
		}
		public virtual NamedStreamInfo SetNamedStreamData(Type classType, string propertyName) {
			return null;
		}
		public virtual bool ShowLargePropertyAsNamedStream(Type classType, string propertyName) {
			return true;
		}		
		public static ValueConverter CallConverter(XPMemberInfo memberInfo) {
			if (memberInfo.Converter == null)
				return null;
			lock (xpConverterHelperDict) {
				ValueConverter converter;
				if (xpConverterHelperDict.TryGetValue(memberInfo, out converter))
					return converter;
				Type converterType = memberInfo.Converter.GetType();
				if(converterType == typeof(DevExpress.Xpo.Metadata.Helpers.EnumsConverter)) {
					converter = new DevExpress.Xpo.Metadata.Helpers.EnumsConverter(memberInfo.MemberType);
				} else {
					ConstructorInfo converterConstructor = converterType.GetConstructor(new Type[0]);
					if(converterConstructor == null)
						throw new InvalidOperationException("ValueConverter without default constructor not supported");
					converter = (ValueConverter)converterConstructor.Invoke(new object[0]);
				}
				xpConverterHelperDict.Add(memberInfo, converter);
				return converter;
			}
		}
		public void AddServiceOperation(string serviceOperationName, Type resultType, Type[] parametersType, Func<object[], object> function) {
			lock (serviceOperations) {
				if (this.serviceOperations.ContainsKey(serviceOperationName)) return;
				ServiceOperationResultKind resultKind = OperationHelper.GetOperationResultKind(resultType, Metadata);
				ResourceType returnType = OperationHelper.GetServiceOperationReturnType(resultType, Metadata);
				ResourceSet resourceSet = OperationHelper.GetResourceSet(returnType, Metadata);
				List<ServiceOperationParameter> parameters = new List<ServiceOperationParameter>();
				int parameterNumber = 0;
				foreach (Type parameterType in parametersType) {
					string parameterName = string.Format("parameter{0}", parameterNumber++);
					parameters.Add(new ServiceOperationParameter(parameterName, OperationHelper.GetParameterResourceType(parameterType, Metadata)));
				}
				ServiceOperation so = new ServiceOperation(serviceOperationName, resultKind, returnType, resourceSet, "GET", parameters);
				AddServiceOperation(so, function);
			}
		}
		public void AddServiceOperation(ServiceOperation serviceOperation, Func<object[], object> function) {
			lock (serviceOperations) {
				if (this.serviceOperations.ContainsKey(serviceOperation.Name)) return;
				serviceOperation.CustomState = function;
				AddServiceOperationToMetadata(serviceOperation);
			}
		}
		public void AddServiceOperation(ServiceOperation serviceOperation) {
			lock (serviceOperations) {
				if (this.serviceOperations.ContainsKey(serviceOperation.Name)) return;
				AddServiceOperationToMetadata(serviceOperation);
			}
		}
		void AddServiceOperationToMetadata(ServiceOperation serviceOperation) {
			this.serviceOperations.Add(serviceOperation.Name, serviceOperation);
			serviceOperation.SetReadOnly();
		}
		public bool TryResolveServiceOperation(string name, out ServiceOperation serviceOperation) {			
			lock (serviceOperations) {
				ServiceOperation operation;
				bool result = this.serviceOperations.TryGetValue(name, out operation);
				serviceOperation = result ? operation : null;
				return result;
			}
		}
	}
}
