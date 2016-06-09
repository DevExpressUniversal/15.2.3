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
	using System.Collections.Generic;
	using System.Reflection;
	using System.Reflection.Emit;
	using DevExpress.Utils.MVVM.Internal;
	static class EventInfoHelper {
		internal static EventInfo GetEventInfo<T>(Action<T, Action> subscribe) {
			var reader = new ILReader(subscribe.Method);
			var subscribeEventPattern = GetSubscribeEventPattern();
			if(subscribeEventPattern.Match(reader)) {
				var handlerCtor = subscribeEventPattern.Captures[1].Operand as ConstructorInfo;
				Type handlerType = handlerCtor.DeclaringType;
				var handlerMethod = subscribeEventPattern.Captures[0].Operand as MethodInfo;
				var addEventMethod = subscribeEventPattern.Captures[2].Operand as MethodInfo;
				var addEventMethodParameter = addEventMethod.GetParameters()[0];
				if(addEventMethodParameter.ParameterType == handlerType && AreParametersEquals(handlerMethod, handlerType))
					return addEventMethod.DeclaringType.GetEvent(addEventMethod.Name.Substring("add_".Length));
			}
			return null;
		}
		static bool AreParametersEquals(MethodInfo mInfo, Type handlerType) {
			return AreEquals(mInfo.GetParameters(), handlerType.GetMethod("Invoke").GetParameters());
		}
		static bool AreEquals(ParameterInfo[] pInfos1, ParameterInfo[] pInfos2) {
			if(pInfos1.Length != pInfos2.Length) return false;
			for(int i = 0; i < pInfos1.Length; i++) {
				if(pInfos1[i].ParameterType == pInfos2[i].ParameterType)
					continue;
				if(!pInfos2[i].ParameterType.IsAssignableFrom(pInfos1[i].ParameterType))
					return false;
			}
			return true;
		}
		internal static ILReader.Pattern GetSubscribeEventPattern() {
			return new ILReader.Pattern(
				i => i.OpCode == OpCodes.Ldftn,
				i => i.OpCode == OpCodes.Newobj && IsDelegate(i.Operand as ConstructorInfo),
				i => i.OpCode == OpCodes.Callvirt && IsAddEvent(i.Operand as MethodInfo));
		}
		static bool IsDelegate(ConstructorInfo cInfo) {
			return (cInfo != null) && typeof(Delegate).IsAssignableFrom(cInfo.DeclaringType);
		}
		static bool IsAddEvent(MethodInfo mInfo) {
			return (mInfo != null) && mInfo.IsSpecialName && mInfo.Name.StartsWith("add_");
		}
		static IDictionary<HandlerKey, HandlerExpressionBuilder> handlersCache = new Dictionary<HandlerKey, HandlerExpressionBuilder>();
		internal static HandlerExpressionBuilder GetBuilder(Type type, EventInfo eventInfo) {
			HandlerKey key = new HandlerKey(type, eventInfo.DeclaringType);
			HandlerExpressionBuilder builder;
			if(!handlersCache.TryGetValue(key, out builder)) {
				builder = new HandlerExpressionBuilder(eventInfo);
				handlersCache.Add(key, builder);
			}
			return builder;
		}
		internal static bool TryGetBuilder(Type type, EventInfo eventInfo, out HandlerExpressionBuilder builder) {
			return handlersCache.TryGetValue(new HandlerKey(type, eventInfo.DeclaringType), out builder);
		}
	}
}
