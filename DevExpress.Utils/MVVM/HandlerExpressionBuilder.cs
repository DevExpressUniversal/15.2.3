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

namespace DevExpress.Utils.MVVM {
	using System;
	using System.Linq.Expressions;
	using System.Reflection;
	class HandlerKey {
		readonly int hashCode;
		readonly Type target;
		readonly Type sourceType;
		public HandlerKey(Type target, Type sourceType) {
			this.target = target;
			this.sourceType = sourceType;
			this.hashCode = target.GetHashCode() ^ sourceType.GetHashCode();
		}
		public override bool Equals(object obj) {
			HandlerKey key = obj as HandlerKey;
			if(object.ReferenceEquals(null, key)) return false;
			return target == key.target && sourceType == key.sourceType;
		}
		public override int GetHashCode() {
			return hashCode;
		}
	}
	class HandlerExpressionBuilder {
		Type handlerType;
		Action<object, object> addEvent;
		Action<object, object> removeEvent;
		public HandlerExpressionBuilder(EventInfo eventInfo) {
			this.handlerType = eventInfo.EventHandlerType;
			var handler = Expression.Parameter(typeof(object));
			var source = Expression.Parameter(typeof(object));
			var typedHandler = Expression.TypeAs(handler, handlerType);
			var typedSource = Expression.TypeAs(source, eventInfo.DeclaringType);
			var addEventMethod = eventInfo.GetAddMethod(true);
			this.addEvent = GetEventMethod(handler, source, typedHandler, typedSource, addEventMethod);
			var removeEventMethod = eventInfo.GetRemoveMethod(true);
			this.removeEvent = GetEventMethod(handler, source, typedHandler, typedSource, removeEventMethod);
		}
		public void Subscribe(object source, Delegate handlerDelegate) {
			addEvent(source, handlerDelegate);
		}
		public void Unsubscribe(object source, Delegate handlerDelegate) {
			removeEvent(source, handlerDelegate);
		}
		public Delegate GetHandler(MethodCallExpression triggerExpression) {
			return GetHandler(handlerType, triggerExpression, GetEventHandlerParameters(handlerType));
		}
		public Delegate GetHandler(MethodCallExpression beforeExpression, MethodCallExpression triggerExpression, MethodCallExpression afterExpression) {
			var handlerParameters = GetEventHandlerParameters(handlerType);
			if(handlerParameters.Length > 0) {
				var args = handlerParameters[handlerParameters.Length - 1];
				var beforeExpressionWithArgs = Expression<Action<object>>.Call(beforeExpression.Object, beforeExpression.Method, args);
				return Expression.Lambda(handlerType,
							Expression.Block(
								beforeExpressionWithArgs,
								triggerExpression,
								afterExpression),
							handlerParameters
						).Compile();
			}
			return GetHandler(handlerType, triggerExpression, handlerParameters);
		}
		static Delegate GetHandler(Type handlerType, MethodCallExpression triggerExpression, ParameterExpression[] handlerParameters) {
			return Expression.Lambda(handlerType,
				triggerExpression,
				handlerParameters
			).Compile();
		}
		static Action<object, object> GetEventMethod(ParameterExpression handler, ParameterExpression source, UnaryExpression typedHandler, UnaryExpression typedSource, MethodInfo eventMethod) {
			return Expression.Lambda<Action<object, object>>(
					Expression.Call(typedSource, eventMethod, typedHandler),
					source, handler
				).Compile();
		}
		static ParameterExpression[] GetEventHandlerParameters(Type handlerType) {
			var invokeParameters = handlerType.GetMethod("Invoke").GetParameters();
			var parameters = new ParameterExpression[invokeParameters.Length];
			for(int i = 0; i < parameters.Length; i++) {
				var p = invokeParameters[i];
				parameters[i] = Expression.Parameter(p.ParameterType, p.Name);
			}
			return parameters;
		}
	}
}
