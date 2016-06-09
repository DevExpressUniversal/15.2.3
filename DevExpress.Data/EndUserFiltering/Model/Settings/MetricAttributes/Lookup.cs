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
		internal static bool IsLookup(Type type) {
			if(type == typeof(string)) return true;
			if(TypeHelper.IsNullable(type))
				return IsLookup(Nullable.GetUnderlyingType(type));
			return (type == typeof(Guid)) || (type == typeof(IntPtr));
		}
		internal static bool IsLookupMember(IMetadataStorage metadataStorage, Type type, string path) {
			if(TypeHelper.IsNullable(type))
				return IsLookupMember(metadataStorage, Nullable.GetUnderlyingType(type), path);
			return IsLookupMember(metadataStorage, path) &&
				(type == typeof(int)) || (type == typeof(uint)) ||
				(type == typeof(long)) || (type == typeof(ulong));
		}
		static bool IsLookupMember(IMetadataStorage metadataStorage, string path) {
			return metadataStorage.@Get(storage => GetIsKey(storage, path)) || IsIdProperty(path);
		}
		static bool IsIdProperty(string path) {
			return path.ToLowerInvariant().EndsWith("id");
		}
		static bool GetIsKey(IMetadataStorage metadataStorage, string path) {
			Data.Utils.AnnotationAttributes attributes;
			return metadataStorage.TryGetValue(path, out attributes) && attributes.IsKey;
		}
		delegate IMetricAttributes LookupMetricCtor(object dataSource, string valueMember, string displayMember,
			int? top, int? maxCount, LookupUIEditorType editorType,
			bool? useFlags, bool? useSelectAll, string selectAllName, string nullName, string[] members);
		static IDictionary<Type, LookupMetricCtor> lookupInitializers = new Dictionary<Type, LookupMetricCtor>();
		internal static IMetricAttributes CreateLookup(Type type, object dataSource, string valueMember, string displayMember, int? top, int? maxCount, LookupUIEditorType editorType, bool? useFlags, bool? useSelectAll, string selectAllName, string nullName, string[] members) {
			LookupMetricCtor initializer;
			if(!lookupInitializers.TryGetValue(type, out initializer)) {
				var mType = typeof(LookupMetricAttributes<>).MakeGenericType(type);
				var pDataSource = Expression.Parameter(typeof(object), "dataSource");
				var pValueMember = Expression.Parameter(typeof(string), "valueMember");
				var pDisplayMember = Expression.Parameter(typeof(string), "displayMember");
				var pTop = Expression.Parameter(typeof(int?), "top");
				var pMaxCount = Expression.Parameter(typeof(int?), "maxCount");
				var pMembers = Expression.Parameter(typeof(string[]), "members");
				var pEditorType = Expression.Parameter(typeof(LookupUIEditorType), "editorType");
				var pUseFlags = Expression.Parameter(typeof(bool?), "useFlags");
				var pUseSelectAll = Expression.Parameter(typeof(bool?), "useSelectAll");
				var pSelectAllName = Expression.Parameter(typeof(string), "selectAllName");
				var pNullName = Expression.Parameter(typeof(string), "nullName");
				var ctorExpression = Expression.New(
							mType.GetConstructor(new Type[] { typeof(object), typeof(string), typeof(string), typeof(int?), typeof(int?), typeof(LookupUIEditorType), typeof(bool?), typeof(bool?), typeof(string), typeof(string), typeof(string[]) }),
							pDataSource,
							pValueMember,
							pDisplayMember,
							pTop,
							pMaxCount,
							pEditorType,
							pUseFlags,
							pUseSelectAll,
							pSelectAllName,
							pNullName,
							pMembers);
				initializer = Expression.Lambda<LookupMetricCtor>(
					ctorExpression, pDataSource, pValueMember, pDisplayMember, pTop, pMaxCount, pEditorType, pUseFlags, pUseSelectAll, pSelectAllName, pNullName, pMembers).Compile();
				lookupInitializers.Add(type, initializer);
			}
			return initializer(dataSource, valueMember, displayMember, top, maxCount, editorType, useFlags, useSelectAll, selectAllName, nullName, members);
		}
		class BaseLookupMetricAttributes : MetricAttributes {
			readonly string selectAllName;
			readonly string nullName;
			protected BaseLookupMetricAttributes(string selectAllName, string nullName, LookupUIEditorType editorType, string[] members)
				: base(members) {
				this.selectAllName = selectAllName;
				this.nullName = nullName;
				EditorType = editorType;
			}
			public LookupUIEditorType EditorType {
				get;
				private set;
			}
			public string SelectAllName {
				get { return selectAllName ?? FilteringLocalizer.GetString(FilteringLocalizerStringId.SelectAllName); }
			}
			public string NullName {
				get { return nullName ?? FilteringLocalizer.GetString(FilteringLocalizerStringId.NullName); }
			}
		}
		class LookupMetricAttributes<T> : BaseLookupMetricAttributes, ILookupMetricAttributes<T> {
			MemberLookupValuesBox<T> lookupValues;
			MemberNullableValueBox<int> top;
			MemberNullableValueBox<int> maxCount;
			public LookupMetricAttributes(
				object dataSource, string valueMember, string displayMember,
				int? top, int? maxCount, LookupUIEditorType editorType,
				bool? useFlags, bool? useSelectAll, string selectAllName, string nullName,
				string[] members)
				: base(selectAllName, nullName, editorType, members) {
				this.lookupValues = new MemberLookupValuesBox<T>(dataSource, valueMember, displayMember, 0, this, () => DataSource);
				this.top = new MemberNullableValueBox<int>(top, 1, this, () => Top);
				this.maxCount = new MemberNullableValueBox<int>(maxCount, 2, this, () => MaxCount);
				UseFlags = useFlags.GetValueOrDefault(true);
				UseSelectAll = useSelectAll.GetValueOrDefault(true);
			}
			public object DataSource {
				get { return lookupValues.Value; }
			}
			public string ValueMember {
				get { return lookupValues.ValueMember; }
			}
			public string DisplayMember {
				get { return lookupValues.DisplayMember; }
			}
			public int? Top {
				get { return GetTop(top.Value); }
			}
			public int? MaxCount {
				get { return GetMaxCount(top.Value, maxCount.Value); }
			}
			public bool UseFlags {
				get;
				private set;
			}
			public bool UseSelectAll {
				get;
				private set;
			}
			#region static
			static int? GetTop(int? top) {
				if(top.HasValue && top.Value <= 0)
					return null;
				return top;
			}
			static int? GetMaxCount(int? top, int? maxCount) {
				if(!maxCount.HasValue || maxCount.Value <= 0)
					return null;
				return !top.HasValue ? maxCount : new Nullable<int>(Math.Max(maxCount.Value, top.Value));
			}
			#endregion
		}
	}
}
