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
	public interface IValueTypeResolver {
		Type GetValueViewModelType(Type metricTypeDefinition, Type valueType);
		Type GetValueBoxType(Type metricTypeDefinition, Type valueType);
	}
	sealed class DefaultValueTypeResolver : IValueTypeResolver {
		internal static readonly DefaultValueTypeResolver Instance = new DefaultValueTypeResolver();
		DefaultValueTypeResolver() { }
		readonly static IDictionary<Type, Type> viewModelTypesMapping = new Dictionary<Type, Type>() {
				{ typeof(IRangeMetricAttributes<>), typeof(IRangeValueViewModel<>) },
				{ typeof(ILookupMetricAttributes<>), typeof(ICollectionValueViewModel<>) },
				{ typeof(IChoiceMetricAttributes<>), typeof(ISimpleValueViewModel<>) },
				{ typeof(IEnumChoiceMetricAttributes<>), typeof(IEnumValueViewModel<>) },
		};
		Type IValueTypeResolver.GetValueViewModelType(Type metricTypeDefinition, Type valueType) {
			return GetType(metricTypeDefinition, valueType, viewModelTypesMapping);
		}
		readonly static IDictionary<Type, Type> valueBoxTypesMapping = new Dictionary<Type, Type>() {
				{ typeof(IRangeMetricAttributes<>), typeof(RangeValueBox<>) },
				{ typeof(ILookupMetricAttributes<>), typeof(CollectionValueBox<>) },
				{ typeof(IChoiceMetricAttributes<>), typeof(BooleanValueBox<>) },
				{ typeof(IEnumChoiceMetricAttributes<>), typeof(EnumValueBox<>) },
		};
		Type IValueTypeResolver.GetValueBoxType(Type metricTypeDefinition, Type valueType) {
			return GetType(metricTypeDefinition, valueType, valueBoxTypesMapping);
		}
		static Type GetType(Type typeDefinition, Type valueType, IDictionary<Type, Type> mapping) {
			Type type;
			if(mapping.TryGetValue(typeDefinition, out type)) {
				valueType = TypeHelper.IsNullable(valueType) ? Nullable.GetUnderlyingType(valueType) : valueType;
				return type.MakeGenericType(valueType);
			}
			throw new NotSupportedException(typeDefinition.ToString());
		}
	}
}
