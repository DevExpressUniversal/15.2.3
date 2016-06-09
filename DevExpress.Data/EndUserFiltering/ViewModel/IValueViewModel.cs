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
	public interface IValueViewModel {
		bool IsModified { get; }
		void Initialize(IEndUserFilteringMetricViewModel metricViewModel);
		void Release();
		void Reset();
		event EventHandler Changed;
	}
	public interface IFilterValueViewModel {
		DevExpress.Data.Filtering.CriteriaOperator CreateFilterCriteria();
	}
	public interface ISimpleValueViewModel<T> : IValueViewModel
		where T : struct {
		bool AllowNull { get; }
		T? Value { get; set; }
	}
	public interface IBooleanValueViewModel : ISimpleValueViewModel<bool> {
		string DefaultName { get; }
		string TrueName { get; }
		string FalseName { get; }
		bool? DefaultValue { get; }
	}
	public interface IRangeValueViewModel<T> : IValueViewModel
		where T : struct {
		bool AllowNull { get; }
		T? Average { get; }
		T? Minimum { get; }
		T? Maximum { get; }
		T? FromValue { get; set; }
		T? ToValue { get; set; }
		string FromName { get; }
		string ToName { get; }
	}
	public interface IBaseCollectionValueViewModel : IValueViewModel {
		bool UseFlags { get; }
		bool UseSelectAll { get; }
		string SelectAllName { get; }
		string NullName { get; }
	}
	public interface IEnumValueViewModel : IBaseCollectionValueViewModel {
		Type EnumType { get; }
	}
	public interface IEnumValueViewModel<T> : IEnumValueViewModel, ISimpleValueViewModel<T>
		where T : struct {
		IEnumerable<T> Values { get; set; }
	}
	public interface ICollectionValueViewModel : IBaseCollectionValueViewModel {
		object DataSource { get; }
		string ValueMember { get; }
		string DisplayMember { get; }
		bool IsLoadMoreAvailable { get; }
		void LoadMore();
		bool IsLoadFewerAvailable { get; }
		void LoadFewer();
		IEnumerable<KeyValuePair<object, string>> LookupDataSource { get; }
	}
	public interface ICollectionValueViewModel<T> : ICollectionValueViewModel {
		IEnumerable<T> Values { get; set; }
		int? Top { get; }
		int? MaxCount { get; }
	}
}
