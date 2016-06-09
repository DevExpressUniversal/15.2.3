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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Utils;
using DevExpress.Data.WcfLinq;
using DevExpress.Data.WcfLinq.Helpers;
using DevExpress.Xpf.Core.DataSources;
namespace DevExpress.Xpf.Core.ServerMode {
	[DefaultEvent("GetSource")]
	public class WcfInstantFeedbackDataSource : DevExpress.Xpf.Core.DataSources.ListSourceDataSourceBase, IWcfServerModeDataSource, IDisposable {
		public static readonly DependencyProperty AreSourceRowsThreadSafeProperty;
		public static readonly DependencyProperty DefaultSortingProperty;
		public static readonly DependencyProperty FixedFilterProperty;
		public static readonly DependencyProperty KeyExpressionProperty;
		public static readonly DependencyProperty DataServiceContextProperty;
		public static readonly DependencyProperty QueryProperty;
		public static readonly DependencyProperty UseExtendedDataQueryProperty;
		public static readonly RoutedEvent GetSourceEvent;
		public static readonly RoutedEvent DismissSourceEvent;
		public static readonly DependencyProperty ServiceRootProperty;
		volatile object contextField;
		volatile IQueryable queryField;
		volatile string keyExpressionField;
		volatile bool useExtendedDataQueryField;
		volatile bool isDisposed;
		static void OnFixedFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			WcfInstantFeedbackSource data = ((WcfInstantFeedbackDataSource)d).Data as WcfInstantFeedbackSource;
			if(data != null) {
				data.FixedFilterString = (string)e.NewValue;
			}
		}
		static void OnAreSourceRowsThreadSafeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			WcfInstantFeedbackSource data = ((WcfInstantFeedbackDataSource)d).Data as WcfInstantFeedbackSource;
			if(data != null) {
				data.AreSourceRowsThreadSafe = (bool)e.NewValue;
			}
		}
		static void OnDefaultSortingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			WcfInstantFeedbackSource data = ((WcfInstantFeedbackDataSource)d).Data as WcfInstantFeedbackSource;
			if(data != null) {
				data.DefaultSorting = (string)e.NewValue;
			}
		}
		static void OnKeyExpressionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			WcfInstantFeedbackDataSource dataSource = (WcfInstantFeedbackDataSource)d;
			dataSource.keyExpressionField = dataSource.KeyExpression;
			WcfInstantFeedbackSource data = dataSource.Data as WcfInstantFeedbackSource;
			if(data != null) {
				data.KeyExpression = (string)e.NewValue;
			}
		}
		static void OnDataServiceContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			WcfInstantFeedbackDataSource dataSource = (WcfInstantFeedbackDataSource)d;
			dataSource.contextField = dataSource.DataServiceContext;
			dataSource.ResetWcfInstantFeedbackSource();
		}
		static void OnQueryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			WcfInstantFeedbackDataSource dataSource = (WcfInstantFeedbackDataSource)d;
			dataSource.queryField = dataSource.Query;
			dataSource.ResetWcfInstantFeedbackSource();
		}
		static void OnUseExtendedDataQueryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			WcfInstantFeedbackDataSource dataSource = (WcfInstantFeedbackDataSource)d;
			dataSource.useExtendedDataQueryField = dataSource.UseExtendedDataQuery;
			dataSource.ResetWcfInstantFeedbackSource();
		}
		static WcfInstantFeedbackDataSource() {
			Type ownerType = typeof(WcfInstantFeedbackDataSource);
			AreSourceRowsThreadSafeProperty = DependencyPropertyManager.Register("AreSourceRowsThreadSafe", typeof(bool),
				ownerType, new PropertyMetadata(false, OnAreSourceRowsThreadSafeChanged));
			FixedFilterProperty = DependencyPropertyManager.Register("FixedFilter", typeof(string),
				ownerType, new PropertyMetadata(String.Empty, OnFixedFilterChanged));
			DefaultSortingProperty = DependencyPropertyManager.Register("DefaultSorting", typeof(string),
				ownerType, new PropertyMetadata(String.Empty, OnDefaultSortingChanged));
			KeyExpressionProperty = DependencyPropertyManager.Register("KeyExpression", typeof(string),
				ownerType, new PropertyMetadata(String.Empty, OnKeyExpressionChanged));
			DataServiceContextProperty = DependencyPropertyManager.Register("DataServiceContext", typeof(object),
				ownerType, new PropertyMetadata(null, OnDataServiceContextChanged));
			QueryProperty = DependencyPropertyManager.Register("Query", typeof(IQueryable),
				ownerType, new PropertyMetadata(null, OnQueryChanged));
			UseExtendedDataQueryProperty = DependencyPropertyManager.Register("UseExtendedDataQuery", typeof(bool),
				ownerType, new PropertyMetadata(false, OnUseExtendedDataQueryChanged));
			GetSourceEvent = EventManager.RegisterRoutedEvent("GetSource", RoutingStrategy.Direct,
				typeof(GetWcfSourceEventHandler), ownerType);
			DismissSourceEvent = EventManager.RegisterRoutedEvent("DismissSource", RoutingStrategy.Direct,
				typeof(GetWcfSourceEventHandler), ownerType);
			ServiceRootProperty = DependencyPropertyManager.Register("ServiceRoot", typeof(Uri),
				ownerType, new PropertyMetadata((d, e) => ((WcfInstantFeedbackDataSource)d).UpdateData()));
		}
		void Data_GetSource(object sender, GetSourceEventArgs e) {
			GetWcfSourceEventArgs args = new GetWcfSourceEventArgs();
			e.Extender.CustomFetchKeys += new EventHandler<DevExpress.Data.Helpers.CustomFetchKeysEventArgs>(Extender_CustomFetchKeys);
			e.Extender.CustomGetCount += new EventHandler<DevExpress.Data.Helpers.CustomGetCountEventArgs>(Extender_CustomGetCount);
			e.Extender.CustomGetUniqueValues += new EventHandler<DevExpress.Data.Helpers.CustomGetUniqueValuesEventArgs>(Extender_CustomGetUniqueValues);
			e.Extender.CustomPrepareChildren += new EventHandler<DevExpress.Data.Helpers.CustomPrepareChildrenEventArgs>(Extender_CustomPrepareChildren);
			e.Extender.CustomPrepareTopGroupInfo += new EventHandler<DevExpress.Data.Helpers.CustomPrepareTopGroupInfoEventArgs>(Extender_CustomPrepareTopGroupInfo);
			RaiseGetSource(args);
			if(args.Handled) {
				e.AreSourceRowsThreadSafe = args.AreSourceRowsThreadSafe;
				e.KeyExpression = args.KeyExpression;
				keyExpressionField = args.KeyExpression;
				e.Query = args.Query;
				queryField = args.Query;
				e.Tag = args.Tag;
				return;
			}
			IQueryable query = queryField;
			if(query != null) {
				e.Query = query;
				return;
			}
		}
		void Data_DismissSource(object sender, GetSourceEventArgs e) {
			if(isDisposed) {
				WcfInstantFeedbackSource wcfSource = (WcfInstantFeedbackSource)sender;
				wcfSource.GetSource -= new EventHandler<GetSourceEventArgs>(Data_GetSource);
				wcfSource.DismissSource -= new EventHandler<GetSourceEventArgs>(Data_DismissSource);
			}
			GetWcfSourceEventArgs args = new GetWcfSourceEventArgs();
			e.Extender.CustomFetchKeys -= new EventHandler<DevExpress.Data.Helpers.CustomFetchKeysEventArgs>(Extender_CustomFetchKeys);
			e.Extender.CustomGetCount -= new EventHandler<DevExpress.Data.Helpers.CustomGetCountEventArgs>(Extender_CustomGetCount);
			e.Extender.CustomGetUniqueValues -= new EventHandler<DevExpress.Data.Helpers.CustomGetUniqueValuesEventArgs>(Extender_CustomGetUniqueValues);
			e.Extender.CustomPrepareChildren -= new EventHandler<DevExpress.Data.Helpers.CustomPrepareChildrenEventArgs>(Extender_CustomPrepareChildren);
			e.Extender.CustomPrepareTopGroupInfo -= new EventHandler<DevExpress.Data.Helpers.CustomPrepareTopGroupInfoEventArgs>(Extender_CustomPrepareTopGroupInfo);
			RaiseDismissSource(args);
			if(args.Handled) {
				e.AreSourceRowsThreadSafe = args.AreSourceRowsThreadSafe;
				e.KeyExpression = args.KeyExpression;
				e.Query = args.Query;
				e.Tag = args.Tag;
			}
		}
		void Extender_CustomFetchKeys(object sender, Data.Helpers.CustomFetchKeysEventArgs e) {
			ExtendedDataParametersContainer parameters = new ExtendedDataParametersContainer(
				DevExpress.Data.Filtering.CriteriaOperator.ParseList(keyExpressionField), e.Where, e.Order, e.Skip, e.Take);
			ExtendedDataResultContainer result = ExecuteCustomOperation(parameters);
			if(result != null) {
				e.Result = result.GetKeys();
				e.Handled = true;
			}
		}
		void Extender_CustomGetCount(object sender, Data.Helpers.CustomGetCountEventArgs e) {
			ExtendedDataParametersContainer parameters = new ExtendedDataParametersContainer(e.Where);
			ExtendedDataResultContainer result = ExecuteCustomOperation(parameters);
			if(result != null) {
				e.Result = result.GetCount();
				e.Handled = true;
			}
		}
		void Extender_CustomGetUniqueValues(object sender, Data.Helpers.CustomGetUniqueValuesEventArgs e) {
			ExtendedDataParametersContainer parameters = new ExtendedDataParametersContainer(e.Expression, e.MaxCount, e.Where);
			ExtendedDataResultContainer result = ExecuteCustomOperation(parameters);
			if(result != null) {
				e.Result = result.GetUniqueValues();
				e.Handled = true;
			}
		}
		void Extender_CustomPrepareChildren(object sender, Data.Helpers.CustomPrepareChildrenEventArgs e) {
			ExtendedDataParametersContainer parameters = new ExtendedDataParametersContainer(e.GroupWhere, e.GroupByDescriptor, e.Summaries);
			ExtendedDataResultContainer result = ExecuteCustomOperation(parameters);
			if(result != null) {
				e.Result = result.GetChildren();
				e.Handled = true;
			}
		}
		void Extender_CustomPrepareTopGroupInfo(object sender, Data.Helpers.CustomPrepareTopGroupInfoEventArgs e) {
			ExtendedDataParametersContainer parameters = new ExtendedDataParametersContainer(e.Where, e.Summaries);
			ExtendedDataResultContainer result = ExecuteCustomOperation(parameters);
			if(result != null) {
				e.Result = result.GetTopGroupInfo();
				e.Handled = true;
			}
		}
		ExtendedDataResultContainer ExecuteCustomOperation(ExtendedDataParametersContainer parameters) {
			if(!useExtendedDataQueryField) return null;
			if((contextField == null) || (queryField == null)) {
				throw new InvalidOperationException("DataServiceContext and Query must be specified to use extended data queries.");
			}
			string baseUri = WcfDataServiceQueryHelper.ContextGetBaseUri(contextField).AbsoluteUri;
			if(!baseUri.EndsWith("/")) baseUri += "/";
			string serializedParameters = ExtendedDataHelper.Serialize<ExtendedDataParametersContainer>(parameters);
			Uri extendedDataUri = new Uri(String.Format("{0}Get{1}{2}?{3}='{4}'", baseUri,
				GetQueryName(WcfDataServiceQueryHelper.GetRequestUri(queryField)), ExtendedDataHelper.ExtendedQuerySuffix,
				ExtendedDataHelper.ExtendedParameterName, serializedParameters));
			string serializedResult = String.Empty;
			try {
				serializedResult = WcfDataServiceQueryHelper.ContextExecute(contextField, extendedDataUri).FirstOrDefault();
			} catch(Exception ex) {
				throw new InvalidOperationException("Extended data cannot be obtained. The query parameter is too long.", ex);
			}
			return ExtendedDataHelper.Deserialize<ExtendedDataResultContainer>(serializedResult);
		}
		internal static string GetQueryName(Uri requestUri) {
			string queryName = requestUri.AbsolutePath.Split('/').Last();
			if(queryName.EndsWith("()"))
				queryName = queryName.Substring(0, queryName.Length - "()".Length);
			return queryName;
		}
		void ResetWcfInstantFeedbackSource() {
			if(Data != null) {
				WcfInstantFeedbackSource oldSource = (WcfInstantFeedbackSource)Data;
				oldSource.Dispose();
			}
			if(DesignerProperties.GetIsInDesignMode(this)) {
				Data = null;
			} else {
				if(String.IsNullOrEmpty(KeyExpression)) {
					KeyExpression = WcfDataSourceHelper.GetKeyExpressionFromQuery(queryField);
				}
				WcfInstantFeedbackSource newSource = new WcfInstantFeedbackSource();
				newSource.AreSourceRowsThreadSafe = AreSourceRowsThreadSafe;
				newSource.DefaultSorting = DefaultSorting;
				newSource.KeyExpression = KeyExpression;
				newSource.FixedFilterString = FixedFilter;
				newSource.GetSource += new EventHandler<GetSourceEventArgs>(Data_GetSource);
				newSource.DismissSource += new EventHandler<GetSourceEventArgs>(Data_DismissSource);
				Data = newSource;
			}
		}
		protected virtual void RaiseGetSource(GetWcfSourceEventArgs args) {
			args.RoutedEvent = GetSourceEvent;
			this.RaiseEvent(args);
		}
		protected virtual void RaiseDismissSource(GetWcfSourceEventArgs args) {
			args.RoutedEvent = DismissSourceEvent;
			this.RaiseEvent(args);
		}
		protected override DevExpress.Xpf.Core.DataSources.DataSourceStrategyBase CreateDataSourceStrategy() {
			return new WcfServerModeDataSourceStrategy(this);
		}
		protected override string GetDesignTimeImageName() {
			return "DevExpress.Xpf.Core.Core.Images.WcfInstantFeedbackDataSource.png";
		}
		public WcfInstantFeedbackDataSource() {
			ResetWcfInstantFeedbackSource();
			DisposeCommand = DevExpress.Mvvm.Native.DelegateCommandFactory.Create<object>(
				new Action<object>((parameter) => Dispose()),
				new Func<object, bool>((parameter) => true), false);
		}
		[System.ComponentModel.Category(DataCategory)]
		public bool AreSourceRowsThreadSafe {
			get { return (bool)GetValue(AreSourceRowsThreadSafeProperty); }
			set { SetValue(AreSourceRowsThreadSafeProperty, value); }
		}
		[System.ComponentModel.Category(DataCategory)]
		public string FixedFilter {
			get { return (string)GetValue(FixedFilterProperty); }
			set { SetValue(FixedFilterProperty, value); }
		}
		[System.ComponentModel.Category(DataCategory)]
		public string DefaultSorting {
			get { return (string)GetValue(DefaultSortingProperty); }
			set { SetValue(DefaultSortingProperty, value); }
		}
		[System.ComponentModel.Category(DataCategory)]
		public string KeyExpression {
			get { return (string)GetValue(KeyExpressionProperty); }
			set { SetValue(KeyExpressionProperty, value); }
		}
		[System.ComponentModel.Category(DataCategory)]
		public object DataServiceContext {
			get { return GetValue(DataServiceContextProperty); }
			set { SetValue(DataServiceContextProperty, value); }
		}
		[System.ComponentModel.Category(DataCategory)]
		public IQueryable Query {
			get { return (IQueryable)GetValue(QueryProperty); }
			set { SetValue(QueryProperty, value); }
		}
		[System.ComponentModel.Category(DataCategory)]
		public bool UseExtendedDataQuery {
			get { return (bool)GetValue(UseExtendedDataQueryProperty); }
			set { SetValue(UseExtendedDataQueryProperty, value); }
		}
		[System.ComponentModel.Category(DataCategory)]
		public Uri ServiceRoot {
			get { return (Uri)GetValue(ServiceRootProperty); }
			set { SetValue(ServiceRootProperty, value); }
		}
		public ICommand DisposeCommand { get; private set; }
		public event GetWcfSourceEventHandler GetSource {
			add { this.AddHandler(GetSourceEvent, value); }
			remove { this.RemoveHandler(GetSourceEvent, value); }
		}
		public event GetWcfSourceEventHandler DismissSource {
			add { this.AddHandler(DismissSourceEvent, value); }
			remove { this.RemoveHandler(DismissSourceEvent, value); }
		}
		public void Refresh() {
			if(Data != null) {
				((WcfInstantFeedbackSource)Data).Refresh();
			}
		}
		public void Dispose() {
			isDisposed = true;
			if(Data != null) {
				((WcfInstantFeedbackSource)Data).Dispose();
			}
		}
	}
	public delegate void GetWcfSourceEventHandler(object sender, GetWcfSourceEventArgs e);
	public class GetWcfSourceEventArgs : RoutedEventArgs {
		public bool AreSourceRowsThreadSafe { get; set; }
		public string KeyExpression { get; set; }
		public IQueryable Query { get; set; }
		public object Tag { get; set; }
	}
	static class WcfDataSourceHelper {
		public static string GetKeyExpressionFromQuery(object queryField) {
			if(queryField == null) 
				return String.Empty;
			Type entityType = (Type)queryField.GetType().GetProperty("ElementType").GetValue(queryField, null);
			object[] attributes = entityType.GetCustomAttributes(typeof(object), false);
			if((attributes == null) || (attributes.Length == 0))
				return string.Empty;
			object firstAttr = attributes.FirstOrDefault(a => a.GetType().Name == "DataServiceKeyAttribute");
			if(firstAttr == null)
				return string.Empty;
			ReadOnlyCollection<string> keyNames = firstAttr.GetType().GetProperty("KeyNames").GetValue(firstAttr, null) as ReadOnlyCollection<string>;
			if((keyNames == null) || (keyNames.Count == 0))
				return string.Empty;
			return keyNames[0];
		}
	}
}
