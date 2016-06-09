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
using System.Reflection;
using DevExpress.Utils;
namespace DevExpress.DataAccess.Native.ObjectBinding {
	public static class TypeNamesHelper {
		static readonly Dictionary<Type, string> aliases =
			new Dictionary<Type, string>() {
				{typeof(byte), "byte"},
				{typeof(sbyte), "sbyte"},
				{typeof(short), "short"},
				{typeof(ushort), "ushort"},
				{typeof(int), "int"},
				{typeof(uint), "uint"},
				{typeof(long), "long"},
				{typeof(ulong), "ulong"},
				{typeof(float), "float"},
				{typeof(double), "double"},
				{typeof(decimal), "decimal"},
				{typeof(object), "object"},
				{typeof(bool), "bool"},
				{typeof(char), "char"},
				{typeof(string), "string"},
				{typeof(void), "void"}
			};
		public static string ShortName(Type type) { return NameCore(type, t => GetAliasForType(t) ?? t.Name); }
		public static string LongName(Type type) { return NameCore(type, t => GetAliasForType(t) ?? t.FullName); }
		static string GetAliasForType(Type type) {
			string alias;
			return aliases.TryGetValue(type, out alias) ? alias : null;
		}
		static string NameCore(Type type, Func<Type, string> nameFunc) {
			if(!type.IsGenericType())
				return nameFunc(type);
			Type definition = type.GetGenericTypeDefinition();
#if DXPORTABLE
			Type[] arguments = type.GetTypeInfo().GenericTypeArguments;
#else
			Type[] arguments = type.GetGenericArguments();
#endif
			if (definition == typeof(Nullable<>))
				return NameCore(arguments[0], nameFunc) + "?";
			return string.Format("{0}<{1}>", nameFunc(definition).Split('`')[0],
				string.Join(", ", arguments.Select(typeArg => NameCore(typeArg, nameFunc))));
		}
	}
}
