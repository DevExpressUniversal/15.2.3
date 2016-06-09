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
using System.Reflection;
using System.Data.Services;
using System.Data.Services.Providers;
namespace DevExpress.Xpo {
	public class ActionInfo {
		readonly bool skipAvailabilityCheckForFeeds;
		readonly OperationParameterBindingKind binding;
		readonly MethodInfo actionMethod;
		readonly Func<object, object[], object> availabilityCheckHandler;
		readonly Func<object, object[], object> actionHandler;
		public MethodInfo ActionMethod {
			get { return actionMethod; }
		}
		public OperationParameterBindingKind Binding {
			get { return binding; }
		}
		public ActionInfo(MethodInfo actionMethod, OperationParameterBindingKind bindable, string availabilityMethodName)
			: this(actionMethod) {
			this.binding = bindable;
			if((this.binding == OperationParameterBindingKind.Sometimes)) {
				MethodInfo availabilityCheckMethod = GetAvailabilityMethod(availabilityMethodName);
				this.availabilityCheckHandler = MethodInvokeHelper.CompileExec(availabilityCheckMethod);
				if(availabilityCheckMethod.GetCustomAttributes(typeof(SkipCheckForFeedsAttribute), true).Length != 0)
					this.skipAvailabilityCheckForFeeds = true;
			} else {
				if(availabilityMethodName != null)
					throw new Exception("Unexpected availabilityMethodName provided.");
			}
		}
		public ActionInfo(MethodInfo serviceOperationMethod) {
			this.actionMethod = serviceOperationMethod;
			this.actionHandler = MethodInvokeHelper.CompileExec(actionMethod);
		}
		public object InvokeAction(object context, object[] parameters) {
			return actionHandler(context, parameters);
		}
		public void AssertAvailable(object context, object entity, bool inFeedContext) {
			if (entity == null)
				return;
			if (!IsAvailable(context, entity, inFeedContext))
				throw new DataServiceException(404, "Action not found.");
		}
		public bool IsAvailable(object context, object entity, bool inFeedContext) {
			switch(this.binding) {
				case OperationParameterBindingKind.Always:
				case OperationParameterBindingKind.Never:
					return true;
				default:
					if(inFeedContext && skipAvailabilityCheckForFeeds) {
						return true;
					}
					return (bool)availabilityCheckHandler(context, new object[] { entity });
			}
		}
		MethodInfo GetAvailabilityMethod(string availabilityMethodName) {
			if (availabilityMethodName == null)
				throw new Exception("If the action is conditionally available you need to provide a method to calculate availability");
			var declaringType = ActionMethod.DeclaringType;
			var method = declaringType.GetMethod(availabilityMethodName);
			if (method == null)
				throw new Exception(string.Format("Availability Method {0} was not found on type {1}", availabilityMethodName, declaringType.FullName));
			if (method.ReturnType != typeof(bool))
				throw new Exception(string.Format("AvailabilityCheck method ({0}) MUST return bool.", availabilityMethodName));
			var actionBindingParameterType = ActionMethod.GetParameters()[0].ParameterType;
			var methodParameters = method.GetParameters();
			if (methodParameters.Length != 1 || methodParameters[0].ParameterType != actionBindingParameterType)
				throw new Exception(string.Format("AvailabilityCheck method was expected to have this signature 'bool {0}({1})'", availabilityMethodName, actionBindingParameterType.FullName));
			return method;
		}
	}
}
