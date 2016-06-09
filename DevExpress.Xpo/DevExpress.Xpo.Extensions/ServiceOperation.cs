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
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using System.ServiceModel.Web;
using System.Reflection;
using System.Data.Services.Providers;
using System.Linq.Expressions;
namespace DevExpress.Xpo {
	public class ServiceOperationFactory {
		public static void PopulateServiceOperations(XpoDataServiceV3 dataservice, XpoContext context) {
			var serviceoperationInfosGetMethod = dataservice.GetType()
				.GetMethods(BindingFlags.Public | BindingFlags.Instance)
				.Select(m => new {
					Method = m,
					Attribute = Attribute.GetCustomAttribute(m, typeof(WebGetAttribute)) as WebGetAttribute
				})
				.Where(u => u.Attribute != null)
				.Select(u => u.Method)
				.ToArray();
			foreach (var soInfo in serviceoperationInfosGetMethod) {
				ServiceOperation serviceOperation = CreateServiceOperation(soInfo, context.Metadata, "GET");
				context.AddServiceOperation(serviceOperation);
			}
			var ServiceoperationInfosPostMethod = dataservice.GetType()
				.GetMethods(BindingFlags.Public | BindingFlags.Instance)
				.Select(m => new {
					Method = m,
					Attribute = Attribute.GetCustomAttribute(m, typeof(WebInvokeAttribute)) as WebInvokeAttribute
				})
				.Where(u => u.Attribute != null && u.Attribute.Method == "POST")
				.Select(u => u.Method)
				.ToArray();
			foreach (var soInfo in ServiceoperationInfosPostMethod) {
				ServiceOperation serviceOperation = CreateServiceOperation(soInfo, context.Metadata, "POST");
				context.AddServiceOperation(serviceOperation);
			}
		}
		public static ServiceOperation CreateServiceOperation(MethodInfo mi, IDataServiceMetadataProvider metadata, string method) {
			string operationName = mi.Name;
			ServiceOperationResultKind operationResultKind = OperationHelper.GetOperationResultKind(mi.ReturnType, metadata);
			ResourceType returnType = OperationHelper.GetServiceOperationReturnType(mi.ReturnType, metadata);
			ResourceSet resourceSet = OperationHelper.GetResourceSet(returnType, metadata);
			var parameters = GetParameters(mi, metadata);
			ServiceOperation serviceOperation = new ServiceOperation(
				operationName,
				operationResultKind,
				returnType,
				resourceSet,
				method,
				parameters
			);
			serviceOperation.CustomState = new MethodInvokeHelper(mi);
			return serviceOperation;
		}
		static IEnumerable<ServiceOperationParameter> GetParameters(MethodInfo method, IDataServiceMetadataProvider _metadata) {
			IEnumerable<ParameterInfo> parameters = method.GetParameters();
			foreach (var parameter in parameters) {
				yield return new ServiceOperationParameter(
					parameter.Name,
					OperationHelper.GetParameterResourceType(parameter.ParameterType, _metadata)
				);
			}
		}
	}
	public class MethodInvokeHelper {
		public readonly MethodInfo Method;
		public readonly Func<object, object[], object> Exec;
		public MethodInvokeHelper(MethodInfo mi) {
			Method = mi;
			Exec = CompileExec(mi);
		}
		public static Func<object, object[], object> CompileExec(MethodInfo mi) {
			ParameterExpression paramApi = Expression.Parameter(typeof(object), "api");
			ParameterExpression paramArray = Expression.Parameter(typeof(object[]), "ar");
			ParameterInfo[] methodParameters = mi.GetParameters();
			Expression[] paramExpressions = new Expression[methodParameters.Length];
			for(int i = 0; i < methodParameters.Length; i++) {
				ParameterInfo pi = methodParameters[i];
				Expression paramExpression = Expression.ArrayIndex(paramArray, Expression.Constant(i));
				if(pi.ParameterType != typeof(object)) {
					paramExpressions[i] = Expression.Convert(paramExpression, pi.ParameterType);
				} else {
					paramExpressions[i] = paramExpression;
				}
			}
			MethodCallExpression call = Expression.Call(Expression.Convert(paramApi, mi.DeclaringType), mi, paramExpressions);
			Expression callExpression = call;
			if(call.Type == typeof(void)) {
				callExpression = Expression.Block(call, Expression.Constant(null, typeof(object)));
			} else {
				if(call.Type != typeof(object)) {
					callExpression = Expression.Convert(call, typeof(object));
				}
			}
			Expression<Func<object, object[], object>> lambda = Expression.Lambda<Func<object, object[], object>>(callExpression, paramApi, paramArray);
			Func<object, object[], object> exec = lambda.Compile();
			return exec;
		}
	}
}
