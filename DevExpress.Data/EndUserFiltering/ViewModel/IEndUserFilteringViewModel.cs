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
	using DevExpress.Data.Filtering;
	using DevExpress.Utils.MVVM;
	public interface IEndUserFilteringViewModelProperties :
		IEnumerable<KeyValuePair<string, Type>> {
		IEndUserFilteringViewModelProperties GetNestedProperties(string rootPath);
	}
	public interface IEndUserFilteringViewModelPropertyValues :
		IEnumerable<IEndUserFilteringMetricViewModel> {
		IEndUserFilteringMetricViewModel this[string path] { get; }
		IEndUserFilteringViewModelPropertyValues GetNestedValues(string rootPath);
	}
	public interface IEndUserFilteringViewModel {
		void Initialize(IEndUserFilteringViewModelPropertyValues values);
	}
	public interface IEndUserFilteringViewModelProvider : IEndUserFilteringViewModelPropertyValues {
		IViewModelProvider ParentViewModelProvider { get; set; }
		object ParentViewModel { get; set; }
		Type SourceType { get; set; }
		IEnumerable<IEndUserFilteringMetricAttributes> Attributes { get; set; }
		IEndUserFilteringSettings Settings { get; }
		IEndUserFilteringViewModelProperties Properties { get; }
		IEndUserFilteringViewModelPropertyValues PropertyValues { get; }
		Type ViewModelBaseType { get; set; }
		Type ViewModelType { get; }
		object ViewModel { get; }
		bool IsViewModelTypeCreated { get; }
		bool IsViewModelCreated { get; }
		void Reset();
		CriteriaOperator FilterCriteria { get; }
		void ClearFilterCriteria();
		void RetrieveFields(Action<Type> retrieveFields, Type sourceType, IEnumerable<IEndUserFilteringMetricAttributes> attributes = null, Type viewModelBaseType = null);
	}
	public interface IEndUserFilteringMetricViewModelFactory {
		IEndUserFilteringMetricViewModel Create(IEndUserFilteringMetric metric, IMetricAttributesQuery query, IValueViewModel value, Type valueType);
	}
	public interface IEndUserFilteringMetricViewModel {
		IEndUserFilteringMetric Metric { get; }
		IMetricAttributesQuery Query { get; }
		bool HasValue { get; }
		IValueViewModel Value { get; }
		Type ValueType { get; }
		CriteriaOperator FilterCriteria { get; }
		Func<T, bool> GetWhereClause<T>();
	}
}
