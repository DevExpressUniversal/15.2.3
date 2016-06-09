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
using System.Reflection;
using DevExpress.Utils;
namespace DevExpress.Xpf.Printing {
	public static class ServiceKnownTypeProvider {
		readonly static HashSet<Type> knownTypes = new HashSet<Type> {
#region standard types
			typeof(string),
			typeof(string[]),
			typeof(DateTime),
			typeof(DateTime[]),
			typeof(int),
			typeof(int[]),
			typeof(long),
			typeof(long[]),
			typeof(float),
			typeof(float[]),
			typeof(double),
			typeof(double[]),
			typeof(decimal),
			typeof(decimal[]),
			typeof(bool),
			typeof(bool[]),
			typeof(Guid),
			typeof(Guid[]),
			typeof(uint),
			typeof(uint[]),
			typeof(short),
			typeof(short[]),
			typeof(ushort),
			typeof(ushort[]),
			typeof(ulong),
			typeof(ulong[]),
			typeof(byte),
			typeof(byte[]),
			typeof(sbyte),
			typeof(sbyte[])
#endregion
		};
		public const string GetKnownTypesMethodName = "GetKnownTypes";
		public static IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider provider) {
			return knownTypes;
		}
		public static bool IsRegistered(Type type) {
			Guard.ArgumentNotNull(type, "type");
			return knownTypes.Contains(type);
		}
		public static void Register(Type type) {
			Guard.ArgumentNotNull(type, "type");
			knownTypes.Add(type);
		}
		public static void Register(params Type[] types) {
			Register((IEnumerable<Type>)types);
		}
		public static void Register(IEnumerable<Type> types) {
			Guard.ArgumentNotNull(types, "types");
			foreach(var type in types) {
				Register(type);
			}
		}
		public static void Register<T>() {
			Register(typeof(T));
		}
	}
}
