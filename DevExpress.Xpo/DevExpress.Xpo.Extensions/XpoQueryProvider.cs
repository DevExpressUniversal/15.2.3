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
using System.Collections.Generic;
using System.Data.Services.Providers;
using System.Reflection;
using System.Linq;
using System.Data.Services;
using DevExpress.Xpo.Metadata;
using System.Web;
using System.Linq.Expressions;
namespace DevExpress.Xpo.Helpers {
	internal class XpoQueryProvider : IDataServiceQueryProvider {
		XpoContext dataSource;
		XpoDataServiceV3 dataservice;
		public XpoQueryProvider(XpoDataServiceV3 dataservice) {
			this.dataservice = dataservice;
		}
		#region IDataServiceQueryProvider Members
		public object CurrentDataSource {
			get { return dataSource; }
			set {
				if (dataSource != null) {
					throw new InvalidOperationException(DevExpress.Xpo.Extensions.Properties.Resources.CurrentDataSourceShouldOnlyBeSetOnce);
				}
				dataSource = (XpoContext)value;
			}
		}
		public object GetOpenPropertyValue(object target, string propertyName) {
			throw new NotSupportedException(DevExpress.Xpo.Extensions.Properties.Resources.OpenTypesAreNotYetSupported);
		}
		public IEnumerable<KeyValuePair<string, object>> GetOpenPropertyValues(object target) {
			throw new NotSupportedException(DevExpress.Xpo.Extensions.Properties.Resources.OpenTypesAreNotYetSupported);
		}
		public object GetPropertyValue(object target, ResourceProperty resourceProperty) {
			return ODataHelpers.GetPropertyValue(dataSource.Metadata, target, resourceProperty);
		}
		public System.Linq.IQueryable GetQueryRootForResourceSet(ResourceSet resourceSet) {
			MethodInfo getTypedQueryRootForResourceSetMethod =
				typeof(XpoQueryProvider).GetMethod(
					"GetTypedQueryRootForResourceSet",
					BindingFlags.NonPublic | BindingFlags.Instance);
			return (IQueryable)getTypedQueryRootForResourceSetMethod.MakeGenericMethod(resourceSet.ResourceType.InstanceType).Invoke(this, new object[] { resourceSet });
		}
		private System.Linq.IQueryable GetTypedQueryRootForResourceSet<TElement>(ResourceSet resourceSet) {
			IQueryable query = dataSource.GetResourceSetEntities(resourceSet, dataservice);
			LambdaExpression filter = dataservice.GetQueryInterceptor(resourceSet.ResourceType.InstanceType, dataservice.Token);
			if(filter.Parameters.Count != 1 && !filter.Parameters[0].Type.IsAssignableFrom(resourceSet.ResourceType.InstanceType)) {
				throw new InvalidOperationException(string.Format("Unexpected lambda expression type. Expected: Expression<Func<{0}, bool>>", resourceSet.ResourceType.InstanceType.Name));
			}
			Type delegateType = typeof(Func<,>).MakeGenericType(new Type[] { resourceSet.ResourceType.InstanceType, typeof(bool) });
			ParameterExpression parameter = Expression.Parameter(resourceSet.ResourceType.InstanceType, filter.Parameters[0].Name);
			Expression lambda = Expression.Lambda(delegateType, filter.Body, parameter);
			Expression expression = Expression.Call(typeof(Queryable), "Where", new Type[] { resourceSet.ResourceType.InstanceType }, query.Expression, lambda);
			XpoLinqQueryProvider provider = query.Provider as XpoLinqQueryProvider;
			return provider.CreateQuery(resourceSet.ResourceType.InstanceType, expression);
		}
		public ResourceType GetResourceType(object target) {
			ResourceType resType;
			Type objectType = target.GetType();
			if (!dataSource.Metadata.TryResolveResourceType(TypeSystem.ProcessTypeName(dataSource.NamespaceName, objectType), out resType)) {
				throw new NotSupportedException(String.Format(DevExpress.Xpo.Extensions.Properties.Resources.UnknownResourceType, objectType));
			}
			return resType;
		}
		public object InvokeServiceOperation(ServiceOperation serviceOperation, object[] parameters) {
			object result = null;
			if(serviceOperation.CustomState is MethodInvokeHelper) {
				MethodInvokeHelper helper = (MethodInvokeHelper)serviceOperation.CustomState;
				result = helper.Exec(dataservice, parameters);
			}
			if (serviceOperation.CustomState is Func<object[], object>) {
				Func<object[], object> operation = (Func<object[], object>)serviceOperation.CustomState;
				result = operation(parameters);
			}
			return result;
		}
		public bool IsNullPropagationRequired {
			get { return true; }
		}
		#endregion
	}
}
