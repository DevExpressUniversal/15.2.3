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
	using System.Collections.Generic;
	using System.Linq.Expressions;
	partial class MetricAttributes {
		internal static bool IsEnumChoice(Type type) {
			if(TypeHelper.IsNullable(type))
				return IsEnumChoice(Nullable.GetUnderlyingType(type));
			return type.IsEnum;
		}
		delegate IMetricAttributes EnumChoiceMetricCtor(Type enumDataType, LookupUIEditorType editorType, bool? useFlags, bool? useSelectAll, string selectAllName, string nullName, string[] members);
		static IDictionary<Type, EnumChoiceMetricCtor> enumChoiceInitializers = new Dictionary<Type, EnumChoiceMetricCtor>();
		internal static IMetricAttributes CreateEnumChoice(Type type, Type enumDataType, LookupUIEditorType editorType, bool? useFlags, bool? useSelectAll, string selectAllName, string nullName, string[] members) {
			EnumChoiceMetricCtor initializer;
			if(!enumChoiceInitializers.TryGetValue(type, out initializer)) {
				bool isNullableType = TypeHelper.IsNullable(type);
				var underlyingType = isNullableType ? Nullable.GetUnderlyingType(type) : type;
				var aType = typeof(EnumChoiceMetricAttributes<>).MakeGenericType(underlyingType);
				var pEnumDataType = Expression.Parameter(typeof(Type), "enumDataType");
				var pEditorType = Expression.Parameter(typeof(LookupUIEditorType), "editorType");
				var pUseFlags = Expression.Parameter(typeof(bool?), "useFlags");
				var pUseSelectAll = Expression.Parameter(typeof(bool?), "useSelectAll");
				var pSelectAllName = Expression.Parameter(typeof(string), "selectAllName");
				var pNullName = Expression.Parameter(typeof(string), "nullName");
				var pMembers = Expression.Parameter(typeof(string[]), "members");
				var ctorExpression = Expression.New(
							aType.GetConstructor(new Type[] { typeof(Type), typeof(LookupUIEditorType), typeof(bool?), typeof(bool?), typeof(string), typeof(string), typeof(string[]) }),
							pEnumDataType,
							pEditorType,
							pUseFlags,
							pUseSelectAll,
							pSelectAllName,
							pNullName,
							pMembers);
				initializer = Expression.Lambda<EnumChoiceMetricCtor>(
					ctorExpression, pEnumDataType, pEditorType, pUseFlags, pUseSelectAll, pSelectAllName, pNullName, pMembers).Compile();
				enumChoiceInitializers.Add(type, initializer);
			}
			return initializer(enumDataType, editorType, useFlags, useSelectAll, selectAllName, nullName, members);
		}
		static bool GetUseFlags(bool? useFlags, Type enumType) {
			return useFlags.HasValue ? useFlags.Value && EnumHelper.IsFlags(enumType) : EnumHelper.IsFlags(enumType);
		}
		class EnumChoiceMetricAttributes<T> : BaseLookupMetricAttributes, IEnumChoiceMetricAttributes<T>
			where T : struct {
			public EnumChoiceMetricAttributes(Type enumDataType, LookupUIEditorType editorType, bool? useFlags, bool? useSelectAll, string selectAllName, string nullName, string[] members)
				: base(selectAllName, nullName, editorType, members) {
				EnumType = Nullable.GetUnderlyingType(enumDataType) ?? enumDataType;
				UseFlags = GetUseFlags(useFlags, EnumType);
				UseSelectAll = useSelectAll.GetValueOrDefault();
			}
			public Type EnumType {
				get;
				private set;
			}
			public bool UseFlags {
				get;
				private set;
			}
			public bool UseSelectAll {
				get;
				private set;
			}
		}
	}
}
