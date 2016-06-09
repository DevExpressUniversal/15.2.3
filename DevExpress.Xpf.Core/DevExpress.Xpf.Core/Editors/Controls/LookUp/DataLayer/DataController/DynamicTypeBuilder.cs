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
using System.Reflection.Emit;
using System.Threading;
namespace DevExpress.Xpf.Editors.Helpers {
	public static class DynamicTypeBuilder {
		static readonly object Locker = new object();
		static readonly AssemblyName AssemblyName = new AssemblyName() { Name = "XpfCoreDynamicTypes" };
		static readonly ModuleBuilder ModuleBuilder;
		static readonly Dictionary<string, Tuple<string, Type>> BuiltTypes = new Dictionary<string, Tuple<string, Type>>();
		static DynamicTypeBuilder() {
			ModuleBuilder = Thread.GetDomain().DefineDynamicAssembly(AssemblyName, AssemblyBuilderAccess.Run).DefineDynamicModule(AssemblyName.Name);
		}
		private static string GetTypeKey(Dictionary<string, Type> fields, Type baseType) {
			var fieldsKey = fields.OrderBy(v => v.Key).ThenBy(v => v.Value.Name).Aggregate(string.Empty, (current, field) => current + (field.Key + ";" + field.Value.Name + ";"));
			return baseType != null ? baseType.FullName + "+" + fieldsKey : fieldsKey;
		}
		public static Type GetDynamicType(Dictionary<string, Type> fields, Type basetype, Type[] interfaces) {
			if (fields == null)
				throw new ArgumentNullException("fields");
			if (fields.Count == 0)
				throw new ArgumentOutOfRangeException("fields", "fields must have at least 1 field definition");
			try {
				Monitor.Enter(Locker);
				string typeKey = GetTypeKey(fields, basetype);
				if (BuiltTypes.ContainsKey(typeKey))
					return BuiltTypes[typeKey].Item2;
				string typeName = "DataProxy" + BuiltTypes.Count;
				TypeBuilder typeBuilder = ModuleBuilder.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable, basetype, Type.EmptyTypes);
				var fieldInfo = GenerateProperties(typeBuilder, fields.Select(x => new DynamicProperty(x.Key, x.Value)).ToList());
				GenerateEquals(typeBuilder, fieldInfo);
				GenerateGetHashCode(typeBuilder, fieldInfo);
				BuiltTypes[typeKey] = new Tuple<string, Type>(typeName, typeBuilder.CreateType());
				return BuiltTypes[typeKey].Item2;
			}
			finally {
				Monitor.Exit(Locker);
			}
		}
		static IEnumerable<FieldInfo> GenerateProperties(TypeBuilder tb, IList<DynamicProperty> properties) {
			FieldInfo[] fields = new FieldBuilder[properties.Count];
			for (int i = 0; i < properties.Count; i++) {
				var dp = properties[i];
				FieldBuilder fb = tb.DefineField("_" + dp.Name, dp.Type, FieldAttributes.Private);
				PropertyBuilder pb = tb.DefineProperty(dp.Name, PropertyAttributes.HasDefault, dp.Type, null);
				MethodBuilder mbGet = tb.DefineMethod("get_" + dp.Name,
					MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
					dp.Type, Type.EmptyTypes);
				ILGenerator genGet = mbGet.GetILGenerator();
				genGet.Emit(OpCodes.Ldarg_0);
				genGet.Emit(OpCodes.Ldfld, fb);
				genGet.Emit(OpCodes.Ret);
				MethodBuilder mbSet = tb.DefineMethod("set_" + dp.Name,
					MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
					null, new[] { dp.Type });
				ILGenerator genSet = mbSet.GetILGenerator();
				genSet.Emit(OpCodes.Ldarg_0);
				genSet.Emit(OpCodes.Ldarg_1);
				genSet.Emit(OpCodes.Stfld, fb);
				genSet.Emit(OpCodes.Ret);
				pb.SetGetMethod(mbGet);
				pb.SetSetMethod(mbSet);
				fields[i] = fb;
			}
			return fields;
		}
		static void GenerateEquals(TypeBuilder tb, IEnumerable<FieldInfo> fields) {
			MethodBuilder mb = tb.DefineMethod("Equals",
				MethodAttributes.Public | MethodAttributes.ReuseSlot |
				MethodAttributes.Virtual | MethodAttributes.HideBySig,
				typeof(bool), new Type[] { typeof(object) });
			ILGenerator gen = mb.GetILGenerator();
			LocalBuilder other = gen.DeclareLocal(tb);
			Label next = gen.DefineLabel();
			gen.Emit(OpCodes.Ldarg_1);
			gen.Emit(OpCodes.Isinst, tb);
			gen.Emit(OpCodes.Stloc, other);
			gen.Emit(OpCodes.Ldloc, other);
			gen.Emit(OpCodes.Brtrue_S, next);
			gen.Emit(OpCodes.Ldc_I4_0);
			gen.Emit(OpCodes.Ret);
			gen.MarkLabel(next);
			foreach (FieldInfo field in fields) {
				Type ft = field.FieldType;
				Type ct = typeof(EqualityComparer<>).MakeGenericType(ft);
				next = gen.DefineLabel();
				gen.EmitCall(OpCodes.Call, ct.GetMethod("get_Default"), null);
				gen.Emit(OpCodes.Ldarg_0);
				gen.Emit(OpCodes.Ldfld, field);
				gen.Emit(OpCodes.Ldloc, other);
				gen.Emit(OpCodes.Ldfld, field);
				gen.EmitCall(OpCodes.Callvirt, ct.GetMethod("Equals", new Type[] { ft, ft }), null);
				gen.Emit(OpCodes.Brtrue_S, next);
				gen.Emit(OpCodes.Ldc_I4_0);
				gen.Emit(OpCodes.Ret);
				gen.MarkLabel(next);
			}
			gen.Emit(OpCodes.Ldc_I4_1);
			gen.Emit(OpCodes.Ret);
		}
		static void GenerateGetHashCode(TypeBuilder tb, IEnumerable<FieldInfo> fields) {
			MethodBuilder mb = tb.DefineMethod("GetHashCode",
				MethodAttributes.Public | MethodAttributes.ReuseSlot |
				MethodAttributes.Virtual | MethodAttributes.HideBySig,
				typeof(int), Type.EmptyTypes);
			ILGenerator gen = mb.GetILGenerator();
			gen.Emit(OpCodes.Ldc_I4_0);
			foreach (FieldInfo field in fields) {
				Type ft = field.FieldType;
				Type ct = typeof(EqualityComparer<>).MakeGenericType(ft);
				gen.EmitCall(OpCodes.Call, ct.GetMethod("get_Default"), null);
				gen.Emit(OpCodes.Ldarg_0);
				gen.Emit(OpCodes.Ldfld, field);
				gen.EmitCall(OpCodes.Callvirt, ct.GetMethod("GetHashCode", new Type[] { ft }), null);
				gen.Emit(OpCodes.Xor);
			}
			gen.Emit(OpCodes.Ret);
		}
	}
}
