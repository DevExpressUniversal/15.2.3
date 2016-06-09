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

namespace DevExpress.Utils.Filtering.Internal {
	using System;
	using System.Reflection;
	static class MethodInfoHelper {
		internal static MethodInfo GetMethodInfo(Type sourceType, string methodName) {
			return GetMethodInfo(sourceType, methodName, Type.EmptyTypes);
		}
		internal static MethodInfo GetMethodInfo(Type sourceType, string methodName, Type[] types,
			BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) {
			return GetMemberInfo(sourceType, (type) => type.GetMethod(methodName, flags, null, types, null));
		}
		static TMemberInfo GetMemberInfo<TMemberInfo>(Type sourceType, Func<Type, TMemberInfo> getMember)
			where TMemberInfo : MemberInfo {
			Type[] types = GetTypes(sourceType, sourceType.GetInterfaces());
			for(int i = 0; i < types.Length; i++) {
				var memberInfo = getMember(types[i]);
				if(memberInfo != null)
					return memberInfo;
			}
			return null;
		}
		static Type[] GetTypes(Type sourceType, Type[] interfaces) {
			Type[] types = new Type[interfaces.Length + 1];
			Array.Copy(interfaces, types, interfaces.Length);
			types[interfaces.Length] = sourceType;
			return types;
		}
	}
}
