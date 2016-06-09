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
using System.Collections;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
namespace DevExpress.Utils {
	public enum CriteriaOperatorType {
		Default,
		Or,
		And
	}
	public interface ISearchControl {
		bool Focus();
	}
	public interface ISearchControlClient {
		void SetSearchControl(ISearchControl searchControl);
		SearchControlProviderBase CreateSearchProvider();
		void ApplyFindFilter(SearchInfoBase searchInfo);
		bool IsAttachedToSearchControl { get; }
	}
	public interface IFilteredComponentColumns { IEnumerable Columns { get; } }
	public interface IFilteredComponentColumnsClient : ISearchControlClient, IFilteredComponentColumns { }
	#region SearchInfo
	public abstract class SearchInfoBase {
		public abstract string SearchText { get; }
	}
	public class SearchColumnsInfo : SearchInfoBase {
		readonly string searchText;
		readonly string columns;
		public SearchColumnsInfo(string searchText, string columns) {
			this.searchText = searchText;
			this.columns = columns;
		}
		public override string SearchText { get { return searchText; } }
		public string Columns { get { return columns; } }
	}
	public class SearchCriteriaInfo : SearchInfoBase {
		readonly CriteriaOperator criteriaOperator;
		readonly FindSearchParserResults result;
		public SearchCriteriaInfo(CriteriaOperator op, FindSearchParserResults result) {
			this.result = result;
			this.criteriaOperator = op;
		}
		public CriteriaOperator CriteriaOperator {
			get { return criteriaOperator; }
		}
		public FindSearchParserResults Result {
			get { return result; }
		}
		public override string SearchText {
			get { return (result == null) ? string.Empty : result.SourceText; }
		}
	}
	#endregion SearchInfo
	#region Query Parameters
	public delegate void SearchControlQueryParamsEventHandler(
		SearchControlQueryParamsEventArgs args
	);
	public class SearchControlQueryParamsEventArgs : EventArgs {
		public FilterCondition FilterCondition { get; set; }
		public string SearchText { get; set; }
	}
	public class SearchControlQueryColumnsEventArgs : SearchControlQueryParamsEventArgs {
		public IEnumerable SearchColumns { get; set; }
	}
	#endregion Query Parameters
	#region Providers
	public interface ISearchControlColumnsProvider {
		IEnumerable Columns { get; }
	}
	public interface ISearchControlCriteriaProvider : IDisposable {
		SearchInfoBase GetCriteriaInfo();
		event SearchControlQueryParamsEventHandler QueryCriteriaParams;
	}
	public abstract class SearchControlProviderBase : ISearchControlCriteriaProvider {
		SearchInfoBase ISearchControlCriteriaProvider.GetCriteriaInfo() {
			SearchControlQueryParamsEventArgs args = RaiseQueryCriteriaParams();
			if(!IsValidParams(args)) return null;
			return GetCriteriaInfoCore(args);
		}
		bool isDisposing;
		public bool IsDisposing {
			get { return isDisposing; }
		}
		void IDisposable.Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				DisposeCore();
			}
			GC.SuppressFinalize(this);
		}
		SearchControlQueryParamsEventHandler QueryCriteriaParamsCore;
		event SearchControlQueryParamsEventHandler ISearchControlCriteriaProvider.QueryCriteriaParams {
			add { QueryCriteriaParamsCore += value; }
			remove { QueryCriteriaParamsCore -= value; }
		}
		protected SearchControlQueryParamsEventArgs RaiseQueryCriteriaParams() {
			SearchControlQueryParamsEventArgs args = CreateCriteriaParamsEventArgs();
			SearchControlQueryParamsEventHandler handler = QueryCriteriaParamsCore;
			if(handler != null)
				handler(args);
			return args;
		}
		protected virtual SearchControlQueryParamsEventArgs CreateCriteriaParamsEventArgs() {
			return new SearchControlQueryParamsEventArgs();
		}
		protected virtual bool IsValidParams(SearchControlQueryParamsEventArgs args) {
			return true;
		}
		protected abstract void DisposeCore();
		protected abstract SearchInfoBase GetCriteriaInfoCore(SearchControlQueryParamsEventArgs args);
	}
	public abstract class SearchControlCriteriaProviderBase : SearchControlProviderBase {
		protected sealed override SearchInfoBase GetCriteriaInfoCore(SearchControlQueryParamsEventArgs args) {
			FindSearchParserResults result = CalcResultCore(args);
			CriteriaOperator op = CalcActiveCriteriaOperatorCore(args, result);
			return new SearchCriteriaInfo(op, result);
		}
		protected abstract CriteriaOperator CalcActiveCriteriaOperatorCore(SearchControlQueryParamsEventArgs args, FindSearchParserResults result);
		protected abstract FindSearchParserResults CalcResultCore(SearchControlQueryParamsEventArgs args);
	}
	public abstract class FilteredComponentColumnsProviderBase : SearchControlProviderBase, ISearchControlColumnsProvider {
		IFilteredComponentColumns component;
		Func<object, string> fieldNameAccessor;
		public FilteredComponentColumnsProviderBase(IFilteredComponentColumns component, Func<object, string> fieldNameAccessor) {
			this.fieldNameAccessor = fieldNameAccessor;
			this.component = component;
		}
		protected override void DisposeCore() {
			this.component = null;
		}
		protected override SearchControlQueryParamsEventArgs CreateCriteriaParamsEventArgs() {
			return new SearchControlQueryColumnsEventArgs();
		}
		protected override SearchInfoBase GetCriteriaInfoCore(SearchControlQueryParamsEventArgs args) {
			if(fieldNameAccessor == null) return new SearchColumnsInfo(args.SearchText, "*");
			SearchControlQueryColumnsEventArgs columnArgs = args as SearchControlQueryColumnsEventArgs;
			var fieldNames = new System.Collections.Generic.List<string>();
			foreach(object column in columnArgs.SearchColumns)
				fieldNames.Add(fieldNameAccessor(column));
			return new SearchColumnsInfo(args.SearchText, string.Join(";", fieldNames));
		}
		IEnumerable ISearchControlColumnsProvider.Columns { get { return component.Columns; } }
	}
	#endregion Providers    
	public interface ISupportSearchDataAdapter {
		Data.Filtering.CriteriaOperator FilterCriteria { get; set; }
		int VisibleCount { get; }
		bool AdapterEnabled { get; set; }
		int GetSourceIndex(int filteredIndex);
		int GetVisibleIndex(int index);
	}
	public class SearchDataAdapter<T> : SearchDataAdapter where T : class {
		protected override BaseDataControllerHelper CreateHelper() { 
			return new SearchDataAdapterHelper<T>(this); 
		}
	}
	public class SearchDataAdapter : DevExpress.Data.ListSourceDataController {
		public virtual object GetValueAtIndex(string fieldName, int index) {
			if(fieldName == string.Empty)
				return GetListSourceRow(index);
			return this.GetRowValue(index, fieldName);
		}
		protected override BaseDataControllerHelper CreateHelper() { 
			return new SearchDataAdapterHelper<object>(this); 
		}
		protected class SearchDataAdapterHelper<T> : ListDataControllerHelper where T : class {
			public SearchDataAdapterHelper(SearchDataAdapter adapter)
				: base(adapter) {
			}
			protected override System.ComponentModel.PropertyDescriptorCollection GetPropertyDescriptorCollection() {
				System.ComponentModel.PropertyDescriptorCollection properties = GetTypeProperties(typeof(T));
				System.Collections.Generic.List<string> reservedNames = new System.Collections.Generic.List<string>();
				System.Collections.Generic.List<System.ComponentModel.PropertyDescriptor> actualProperties =
						   new System.Collections.Generic.List<System.ComponentModel.PropertyDescriptor>();
				if(properties != null) {
					foreach(System.ComponentModel.PropertyDescriptor pd in properties) {
						if(CheckPropertyDescriptor(pd)) {
							actualProperties.Add(pd);
							reservedNames.Add(pd.DisplayName);
						}
					}
				}
				object[] attributes = typeof(T).GetCustomAttributes(typeof(SearchColumnAttribute), false);
				if(attributes != null || attributes.Length > 0) {
					foreach(SearchColumnAttribute at in attributes) {
						if(reservedNames.Contains(at.Name)) continue;
						actualProperties.Add(new SearchDataAdapterPropertyDescriptor<T>(at.Name));
					}
				}
				if(actualProperties.Count > 0) return new System.ComponentModel.PropertyDescriptorCollection(actualProperties.ToArray());
				return CreateSimplePropertyDescriptor();
			}
			protected virtual bool CheckPropertyDescriptor(System.ComponentModel.PropertyDescriptor pd) {
				if(pd == null) return false;
				foreach(Attribute at in pd.Attributes) {
					if(at.GetType() == typeof(SearchColumnAttribute))
						return true;
				}
				return false;
			}
		}
	}
	class SearchDataAdapterPropertyDescriptor<T> : System.ComponentModel.PropertyDescriptor where T : class {		
		public SearchDataAdapterPropertyDescriptor(string propertyName) : base(propertyName, null) { }		
		public override bool CanResetValue(object component) { return false; }
		public override Type ComponentType { get { return typeof(T); } }
		public override object GetValue(object component) {
			Type type = component.GetType();
			System.Reflection.PropertyInfo info = type.GetProperty(this.Name);
			if(info == null) return null;
			return info.GetValue(component, null);
		}		
		public override bool IsReadOnly { get { return true; } }
		public override Type PropertyType { get { return typeof(object); } }
		public override void ResetValue(object component) { }
		public override void SetValue(object component, object value) { }
		public override bool ShouldSerializeValue(object component) { return false; }
	}
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
	public class SearchColumnAttribute : Attribute { 
		string nameCore;
		public SearchColumnAttribute() { }
		public SearchColumnAttribute(string name) {
			nameCore = name;
		}
		public string Name { get { return nameCore; } }
	}
}
