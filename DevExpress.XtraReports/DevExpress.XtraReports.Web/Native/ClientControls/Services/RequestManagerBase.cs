#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.ServiceModel;
namespace DevExpress.XtraReports.Web.Native.ClientControls.Services {
	public class RequestManagerBase<TController> : IRequestManager {
		delegate object ControllerGenericFunc(TController requestController, string argumentJson);
		delegate HttpActionResultBase ControllerFunc(TController requestController, string argumentJson);
		internal const string
			ActionKey = "actionKey",
			ArgKey = "arg";
		protected static ILoggingService Logger {
			get { return DefaultLoggingService.Instance; }
		}
		readonly Dictionary<string, ControllerFunc> actions = new Dictionary<string, ControllerFunc>();
		readonly TController requestController;
		public RequestManagerBase(TController requestController) {
			this.requestController = requestController;
			FillActionsbyReflection(actions, requestController.GetType());
		}
		public HttpActionResultBase ProcessRequest(NameValueCollection query) {
			try {
				return ProcessRequestCore(query);
			} catch(Exception e) {
				var targetInvocationException = e as TargetInvocationException;
				if(targetInvocationException != null) {
					e = targetInvocationException.InnerException;
				}
				Logger.Error(GetType().Name + ".ProcessRequest: " + e);
				return new ErrorHttpActionResult();
			}
		}
		HttpActionResultBase ProcessRequestCore(NameValueCollection query) {
			string action = query[ActionKey];
			ControllerFunc controllerFunc;
			if(string.IsNullOrEmpty(action) || !actions.TryGetValue(action, out controllerFunc)) {
				return new ErrorHttpActionResult();
			}
			string argument = query[ArgKey];
			var decodedArgument = Uri.UnescapeDataString(argument);
			return controllerFunc(requestController, decodedArgument);
		}
		static void FillActionsbyReflection(Dictionary<string, ControllerFunc> actions, Type controllerType) {
			var initActionsCount = actions.Count;
			MethodInfo[] methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
			foreach(MethodInfo method in methods) {
				var attributes = (WebHttpActionAttribute[])method.GetCustomAttributes(typeof(WebHttpActionAttribute), true);
				if(attributes.Length == 0) {
					continue;
				}
				WebHttpActionAttribute attribute = attributes[0];
				var func = GenerateControllerFunc(controllerType, method, attribute);
				actions.Add(attribute.Name, func);
			}
			if(initActionsCount == actions.Count) {
				Debug.Fail("Cannot find actions in specified controller");
			}
		}
		static ControllerFunc GenerateControllerFunc(Type controllerType, MethodInfo method, WebHttpActionAttribute attribute) {
			ParameterInfo[] parameters = method.GetParameters();
			if(parameters.Length != 1) {
				throw new InvalidOperationException("Cannot process actions with other than single argument.");
			}
			ParameterInfo parameter = parameters[0];
			bool isWebApi = attribute is WebApiHttpActionAttribute
				|| (method.ReturnType.IsGenericType && typeof(JsonHttpActionResult<>).IsAssignableFrom(method.ReturnType.GetGenericTypeDefinition()));
			var inputControllerInstance = Expression.Parameter(typeof(TController), "controller");
			var inputJsonParameter = Expression.Parameter(typeof(string), "json");
			Expression call = Expression.Convert(inputControllerInstance, controllerType);
			if(parameter.ParameterType == typeof(string)) {
				call = Expression.Call(call, method, inputJsonParameter);
			} else {
				var readContractMethodName = GetMethodName(() => ActionHelper.Read<object>(""));
				var readContractMethod = typeof(ActionHelper).GetMethod(readContractMethodName, BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string) }, null).MakeGenericMethod(parameter.ParameterType);
				var deserialize = Expression.Call(readContractMethod, inputJsonParameter);
				call = Expression.Call(call, method, deserialize);
			}
			if(!typeof(HttpActionResultBase).IsAssignableFrom(method.ReturnType)) {
				call = WrapWithHttpActionResult(call, controllerType.FullName, method, attribute, isWebApi);
			}
			var lambda = Expression.Lambda<ControllerFunc>(call, inputControllerInstance, inputJsonParameter);
			ControllerFunc func = lambda.Compile();
			if(isWebApi) {
				func = WrapJsonExceptions(controllerType.FullName, method.Name, attribute.Name, func);
			}
			return func;
		}
		static Expression WrapWithHttpActionResult(Expression call, string controllerTypeName, MethodInfo method, WebHttpActionAttribute attribute, bool isWebApi) {
			if(isWebApi) {
				if(method.ReturnType == typeof(void)) {
					var jsonResultCtorInfo = typeof(JsonHttpActionResult<object>).GetConstructor(new[] { typeof(object) });
					var returnJsonResult = Expression.New(jsonResultCtorInfo, Expression.Constant(null));
					return Expression.Block(call, returnJsonResult);
				} else if(!method.ReturnType.IsGenericType || !typeof(JsonHttpActionResult<>).IsAssignableFrom(method.ReturnType.GetGenericTypeDefinition())) {
					var jsonResultCtorInfo = typeof(JsonHttpActionResult<>).MakeGenericType(method.ReturnType).GetConstructor(new[] { method.ReturnType });
					return Expression.New(jsonResultCtorInfo, call);
				}
			} else if(method.ReturnType == typeof(byte[])) {
				var binaryResultCtorInfo = typeof(BinaryHttpActionResult).GetConstructor(new[] { typeof(byte[]), typeof(string) });
				return Expression.New(binaryResultCtorInfo, call, Expression.Constant(attribute.ContentType, typeof(string)));
			}
			throw new NotSupportedException(string.Format("Cannot use method {0}.{1} as a controller action", controllerTypeName, method.Name));
		}
		static ControllerFunc WrapJsonExceptions(string controllerTypeName, string methodName, string webActionName, ControllerFunc invoker) {
			return (x, a) => WrapWithTryCatchAction(controllerTypeName, methodName, webActionName, invoker, x, a);
		}
		static HttpActionResultBase WrapWithTryCatchAction(string controllerTypeName, string methodName, string webActionName, ControllerFunc invoker, TController controller, string json) {
			try {
				return invoker(controller, json);
			} catch(Exception e) {
				var targetInvocationException = e as TargetInvocationException;
				if(targetInvocationException != null) {
					e = targetInvocationException.InnerException;
				}
				var prefix = controllerTypeName + "." + methodName + " (" + webActionName + "): ";
				Logger.Error(prefix + e);
				return e is FaultException
					? (HttpActionResultBase)JsonHttpActionResult.CreateFromError(e.Message)
					: JsonHttpActionResult.CreateFromError("Internal Server Error");
			}
		}
		static string GetMethodName(Expression<Action> expression) {
			var body = (MethodCallExpression)expression.Body;
			return body.Method.Name;
		}
	}
}
