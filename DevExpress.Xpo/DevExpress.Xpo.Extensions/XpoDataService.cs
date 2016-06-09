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
using System.Data.Services;
using System.Data.Services.Providers;
using DevExpress.Xpo.Helpers;
using System.Collections.Generic;
using System.Linq.Expressions;
using DevExpress.Xpo.Metadata;
using Microsoft.Data.Edm.Csdl;
using Microsoft.Data.Edm;
using Microsoft.Data.Edm.Library.Values;
using Microsoft.Data.Edm.Library;
using System.Linq;
using Microsoft.Data.Edm.Library.Annotations;
using Microsoft.Data.Edm.Annotations;
using Microsoft.Data.Edm.Validation;
using Microsoft.Data.Edm.Library.Expressions;
using DevExpress.Xpo.Extensions.Properties;
namespace DevExpress.Xpo {
	public abstract class XpoDataServiceV3 : DataService<XpoContext>, IServiceProvider {
		[ThreadStatic]
		static Dictionary<XpoDataServiceV3, UnitOfWork> sessionCache;
		static Dictionary<XpoDataServiceV3, UnitOfWork> SessionCache {
			get {
				if(sessionCache == null) {
					sessionCache = new Dictionary<XpoDataServiceV3, UnitOfWork>();
				}
				return sessionCache;
			}
		}
		readonly static Dictionary<XpoContext, IEdmModel> modelCache = new Dictionary<XpoContext, IEdmModel>();
		readonly XpoContext context;
		public object Token { get; private set; }
		public XpoContext Context {
			get { return context; }
		}
		public XpoMetadata Metadata {
			get { return context.Metadata; }
		}
		object typeWithActions;
		public XpoDataServiceV3(XpoContext context) {
			this.context = context;
			ServiceOperationFactory.PopulateServiceOperations(this, context);
		}
		public XpoDataServiceV3(XpoContext context, object typeWithActions)
			: this(context) {
			this.typeWithActions = typeWithActions;
		}
		public static Func<IEdmModel, IEnumerable<IEdmModel>> CreateAnnotationsBuilder(Func<XpoContext> getContext) {
			return (model) => {
				XpoContext context = getContext();
				IEdmModel newModel;
				lock(modelCache) {
					if(!modelCache.TryGetValue(context, out newModel)) {
						newModel = CreateAnnotatedModel(model, context);
						modelCache.Add(context, newModel);
					}
					return new IEdmModel[] { newModel };
				}
			};
		}
		private static IEdmModel CreateAnnotatedModel(IEdmModel model, XpoContext context) {
			EdmModel edmModel = new EdmModel();
			edmModel.AddElements(model.SchemaElements);
			foreach(string elementName in context.Metadata.Annotations.Keys) {
				string[] partsElementName = elementName.Split('/');
				EdmEntityType et = model.FindDeclaredType(partsElementName[0]) as EdmEntityType;
				if(et == null) continue;
				IEdmProperty property = et.Properties().AsQueryable().FirstOrDefault(i => i.Name == partsElementName[1]);
				if(property == null) continue;
				XpoAnnotation annot = context.Metadata.Annotations[elementName];
				if(annot.Size != 0) {
					EdmValueTerm termSize = new EdmValueTerm("Org.OData.Validation.V1", "Size", EdmPrimitiveTypeKind.Int32);
					EdmIntegerConstant sizeValue = new EdmIntegerConstant(annot.Size);
					EdmValueAnnotation va = new EdmValueAnnotation(property, termSize, sizeValue);
					edmModel.AddVocabularyAnnotation(va);
				}
				if(annot.ReadOnly) {
					EdmValueTerm termReadOnly = new EdmValueTerm("Org.OData.Validation.V1", "ReadOnly", EdmPrimitiveTypeKind.Boolean);
					EdmBooleanConstant readOnlyValue = new EdmBooleanConstant(true);
					EdmValueAnnotation va = new EdmValueAnnotation(property, termReadOnly, readOnlyValue);
					edmModel.AddVocabularyAnnotation(va);
				}
			}
			return edmModel;
		}
		public virtual object GetService(Type serviceType) {
			if(serviceType == typeof(IDataServiceMetadataProvider)) return Metadata;
			if(serviceType == typeof(IDataServiceQueryProvider)) return new XpoQueryProvider(this);
			if(serviceType == typeof(IDataServiceUpdateProvider2))
				return new XpoUpdateProvider(GetSession(), this);
			if(serviceType == typeof(IDataServiceStreamProvider2)) return new XpoStreamProvider(this.CurrentDataSource);
			if(serviceType == typeof(IDataServiceActionProvider)) {
				if(typeWithActions == null) typeWithActions = this;
				return new XpoActionProvider(GetSession(), this, typeWithActions);
			}
			return null;
		}
		protected override XpoContext CreateDataSource() {
			return Context;
		}
		internal UnitOfWork GetSession(bool useCache = true) {
			if(!useCache) {
				return GetSessionCore(Context.ObjectLayer);
			}
			UnitOfWork uow;
			if(!SessionCache.TryGetValue(this, out uow)) {
				uow = GetSessionCore(Context.ObjectLayer);
				if(uow == null) {
					throw new ArgumentNullException("unitOfWork");
				}
				if(uow.Dictionary != Context.ObjectLayer.Dictionary) {
					throw new InvalidOperationException(string.Format(Resources.X0BelongsToADifferentDictionary, uow));
				}
				SessionCache.Add(this, uow);
			}
			return uow;
		}
		protected virtual UnitOfWork GetSessionCore(IObjectLayer objectLayer) {
			return new UnitOfWork(objectLayer);
		}
		public T GetEntityUpdated<T>(object entity) {
			IXPSimpleObject xpObject = entity as IXPSimpleObject;
			return this.GetSession().GetObjectByKey<T>(xpObject.ClassInfo.KeyProperty.GetValue(entity));
		}
		protected override void OnStartProcessingRequest(ProcessRequestArgs args) {
			base.OnStartProcessingRequest(args);
			Token = Authenticate(args);
		}
		public virtual object Authenticate(ProcessRequestArgs args) {
			return 1;
		}
		public virtual LambdaExpression GetQueryInterceptor(Type entityType, object token) {
			if(token != null)
				return (Expression<Func<object, bool>>)((o) => true);
			else
				throw new DataServiceException(401, "Unauthorized access.");
		}
		public IQueryable<T> GetServiceQuery<T>() {
			ResourceType resType;
			if(!Context.Metadata.TryResolveResourceType(TypeSystem.ProcessTypeName(Context.NamespaceName, typeof(T)), out resType)) {
				throw new ArgumentException(string.Format(Resources.TheX0ResourceSetNotFound, typeof(T).Name));
			}
			return new XpoLinqQuery<T>(new XpoLinqQueryProvider(resType, this));
		}
		public virtual void ChangeInterceptor(Type entityType, UpdateOperations operations, object token) {
			if(token == null) throw new DataServiceException(401, "Unauthorized access.");
		}
		protected override void HandleException(HandleExceptionArgs args) {
			HandleExceptionArgs newArgs = args;
			if(args.Exception.InnerException != null && (args.Exception.InnerException is DataServiceException)) {
				newArgs.Exception = args.Exception.InnerException;
			}
			base.HandleException(newArgs);
		}
	}
}
