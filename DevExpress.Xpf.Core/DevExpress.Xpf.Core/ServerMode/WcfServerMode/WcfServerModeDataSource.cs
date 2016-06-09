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
using System.ComponentModel;
using System.Linq;
using System.Windows;
using DevExpress.Data;
using DevExpress.Data.WcfLinq;
using DevExpress.Xpf.Core.DataSources;
using DevExpress.Data.WcfLinq.Helpers;
namespace DevExpress.Xpf.Core.ServerMode {
	public class WcfServerModeDataSource : DevExpress.Xpf.Core.DataSources.ListSourceDataSourceBase, IWcfServerModeDataSource {
		public static readonly DependencyProperty DefaultSortingProperty;
		public static readonly DependencyProperty FixedFilterProperty;
		public static readonly DependencyProperty ElementTypeProperty;
		public static readonly DependencyProperty KeyExpressionProperty;
		public static readonly DependencyProperty DataServiceContextProperty;
		public static readonly DependencyProperty QueryProperty;
		public static readonly DependencyProperty ServiceRootProperty;
		public static readonly DependencyProperty UseExtendedDataQueryProperty;
		public static readonly RoutedEvent ExceptionThrownEvent;
		public static readonly RoutedEvent InconsistencyDetectedEvent;
		static void OnFixedFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			WcfServerModeSource data = ((WcfServerModeDataSource)d).Data as WcfServerModeSource;
			if(data != null) {
				data.FixedFilterString = (string)e.NewValue;
			}
		}
		static void OnDefaultSortingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			WcfServerModeSource data = ((WcfServerModeDataSource)d).Data as WcfServerModeSource;
			if(data != null) {
				data.DefaultSorting = (string)e.NewValue;
			}
		}
		static void OnElementTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			WcfServerModeSource data = ((WcfServerModeDataSource)d).Data as WcfServerModeSource;
			if(data != null) {
				data.ElementType = (Type)e.NewValue;
			}
		}
		static void OnKeyExpressionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			WcfServerModeSource data = ((WcfServerModeDataSource)d).Data as WcfServerModeSource;
			if(data != null) {
				data.KeyExpression = (string)e.NewValue;
			}
		}
		static void OnDataServiceContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			WcfServerModeDataSource dataSource = (WcfServerModeDataSource)d;
			dataSource.ResetWcfServerModeSource();
		}
		static void OnQueryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			WcfServerModeDataSource dataSource = (WcfServerModeDataSource)d;
			dataSource.ResetWcfServerModeSource();
		}
		static void OnUseExtendedDataQueryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			WcfServerModeDataSource dataSource = (WcfServerModeDataSource)d;
			dataSource.ResetWcfServerModeSource();
		}
		static WcfServerModeDataSource() {
			Type ownerType = typeof(WcfServerModeDataSource);
			FixedFilterProperty = DependencyProperty.Register("FixedFilter", typeof(string),
				ownerType, new PropertyMetadata(String.Empty, OnFixedFilterChanged));
			DefaultSortingProperty = DependencyProperty.Register("DefaultSorting", typeof(string),
				ownerType, new PropertyMetadata(String.Empty, OnDefaultSortingChanged));
			ElementTypeProperty = DependencyProperty.Register("ElementType", typeof(Type),
				ownerType, new PropertyMetadata(null, OnElementTypeChanged));
			KeyExpressionProperty = DependencyProperty.Register("KeyExpression", typeof(string),
				ownerType, new PropertyMetadata(String.Empty, OnKeyExpressionChanged));
			DataServiceContextProperty = DependencyProperty.Register("DataServiceContext", typeof(object),
				ownerType, new PropertyMetadata(null, OnDataServiceContextChanged));
			QueryProperty = DependencyProperty.Register("Query", typeof(IQueryable),
				ownerType, new PropertyMetadata(null, OnQueryChanged));
			ServiceRootProperty = DependencyProperty.Register("ServiceRoot", typeof(Uri),
				ownerType, new PropertyMetadata((d, e) => ((WcfServerModeDataSource)d).UpdateData()));
			UseExtendedDataQueryProperty = DependencyProperty.Register("UseExtendedDataQuery", typeof(bool),
				ownerType, new PropertyMetadata(false, OnUseExtendedDataQueryChanged));
			ExceptionThrownEvent = EventManager.RegisterRoutedEvent("ExceptionThrown", RoutingStrategy.Direct,
				typeof(ServerModeExceptionThrownEventHandler), ownerType);
			InconsistencyDetectedEvent = EventManager.RegisterRoutedEvent("InconsistencyDetected", RoutingStrategy.Direct,
				typeof(ServerModeInconsistencyDetectedEventHandler), ownerType);
		}
		void Extender_CustomFetchKeys(object sender, Data.Helpers.CustomFetchKeysEventArgs e) {
			ExtendedDataParametersContainer parameters = new ExtendedDataParametersContainer(
				DevExpress.Data.Filtering.CriteriaOperator.ParseList(KeyExpression), e.Where, e.Order, e.Skip, e.Take);
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
		void wcfServerModeSource_ExceptionThrown(object sender, DevExpress.Data.ServerModeExceptionThrownEventArgs e) {
			ServerModeExceptionThrownEventArgs args = new ServerModeExceptionThrownEventArgs(e.Exception);
			RaiseExceptionThrown(args);
		}
		void wcfServerModeSource_InconsistencyDetected(object sender, DevExpress.Data.ServerModeInconsistencyDetectedEventArgs e) {
			ServerModeInconsistencyDetectedEventArgs args = new ServerModeInconsistencyDetectedEventArgs();
			RaiseInconsistencyDetected(args);
		}
		ExtendedDataResultContainer ExecuteCustomOperation(ExtendedDataParametersContainer parameters) {
			if((DataServiceContext == null) || (Query == null) || !UseExtendedDataQuery) return null;
			string serializedParameters = ExtendedDataHelper.Serialize<ExtendedDataParametersContainer>(parameters);
			Uri extendedDataUri = new Uri(WcfDataServiceQueryHelper.ContextGetBaseUri(DataServiceContext), String.Format("Get{0}{1}?{2}='{3}'",
				WcfInstantFeedbackDataSource.GetQueryName(WcfDataServiceQueryHelper.GetRequestUri(Query)), ExtendedDataHelper.ExtendedQuerySuffix,
				ExtendedDataHelper.ExtendedParameterName, serializedParameters));
			string serializedResult = String.Empty;
			try {
				serializedResult = WcfDataServiceQueryHelper.ContextExecute(DataServiceContext, extendedDataUri).FirstOrDefault();
			} catch(Exception ex) {
				throw new InvalidOperationException("Extended data cannot be obtained. The query parameter is too long.", ex);
			}
			return ExtendedDataHelper.Deserialize<ExtendedDataResultContainer>(serializedResult);
		}
		void ResetWcfServerModeSource() {
			if(Data != null) {
				WcfServerModeSource oldSource = (WcfServerModeSource)Data;
				oldSource.Dispose();
				oldSource.Extender.CustomFetchKeys -= new EventHandler<DevExpress.Data.Helpers.CustomFetchKeysEventArgs>(Extender_CustomFetchKeys);
				oldSource.Extender.CustomGetCount -= new EventHandler<DevExpress.Data.Helpers.CustomGetCountEventArgs>(Extender_CustomGetCount);
				oldSource.Extender.CustomGetUniqueValues -= new EventHandler<DevExpress.Data.Helpers.CustomGetUniqueValuesEventArgs>(Extender_CustomGetUniqueValues);
				oldSource.Extender.CustomPrepareChildren -= new EventHandler<DevExpress.Data.Helpers.CustomPrepareChildrenEventArgs>(Extender_CustomPrepareChildren);
				oldSource.Extender.CustomPrepareTopGroupInfo -= new EventHandler<DevExpress.Data.Helpers.CustomPrepareTopGroupInfoEventArgs>(Extender_CustomPrepareTopGroupInfo);
				oldSource.ExceptionThrown -= new EventHandler<DevExpress.Data.ServerModeExceptionThrownEventArgs>(wcfServerModeSource_ExceptionThrown);
				oldSource.InconsistencyDetected -= new EventHandler<DevExpress.Data.ServerModeInconsistencyDetectedEventArgs>(wcfServerModeSource_InconsistencyDetected);
			}
			if(DesignerProperties.GetIsInDesignMode(this)) {
				Data = null;
			} else {
				if(String.IsNullOrEmpty(KeyExpression))
					KeyExpression = WcfDataSourceHelper.GetKeyExpressionFromQuery(Query);
				WcfServerModeSource newSource = new WcfServerModeSource();
				newSource.DefaultSorting = DefaultSorting;
				newSource.ElementType = ElementType;
				newSource.KeyExpression = KeyExpression;
				newSource.Query = Query;
				newSource.Extender.CustomFetchKeys += new EventHandler<DevExpress.Data.Helpers.CustomFetchKeysEventArgs>(Extender_CustomFetchKeys);
				newSource.Extender.CustomGetCount += new EventHandler<DevExpress.Data.Helpers.CustomGetCountEventArgs>(Extender_CustomGetCount);
				newSource.Extender.CustomGetUniqueValues += new EventHandler<DevExpress.Data.Helpers.CustomGetUniqueValuesEventArgs>(Extender_CustomGetUniqueValues);
				newSource.Extender.CustomPrepareChildren += new EventHandler<DevExpress.Data.Helpers.CustomPrepareChildrenEventArgs>(Extender_CustomPrepareChildren);
				newSource.Extender.CustomPrepareTopGroupInfo += new EventHandler<DevExpress.Data.Helpers.CustomPrepareTopGroupInfoEventArgs>(Extender_CustomPrepareTopGroupInfo);
				newSource.ExceptionThrown += new EventHandler<DevExpress.Data.ServerModeExceptionThrownEventArgs>(wcfServerModeSource_ExceptionThrown);
				newSource.InconsistencyDetected += new EventHandler<DevExpress.Data.ServerModeInconsistencyDetectedEventArgs>(wcfServerModeSource_InconsistencyDetected);
				Data = newSource;
			}
		}
		protected virtual void RaiseExceptionThrown(ServerModeExceptionThrownEventArgs args) {
			args.RoutedEvent = ExceptionThrownEvent;
			this.RaiseEvent(args);
		}
		protected virtual void RaiseInconsistencyDetected(ServerModeInconsistencyDetectedEventArgs args) {
			args.RoutedEvent = InconsistencyDetectedEvent;
			this.RaiseEvent(args);
		}
		protected override DevExpress.Xpf.Core.DataSources.DataSourceStrategyBase CreateDataSourceStrategy() {
			return new WcfServerModeDataSourceStrategy(this);
		}
		protected override string GetDesignTimeImageName() {
			return "DevExpress.Xpf.Core.Core.Images.WcfServerModeDataSource.png";
		}
		public WcfServerModeDataSource() {
			ResetWcfServerModeSource();
		}
		[Category(DataCategory)]
		public string FixedFilter {
			get { return (string)GetValue(FixedFilterProperty); }
			set { SetValue(FixedFilterProperty, value); }
		}
		[Category(DataCategory)]
		public string DefaultSorting {
			get { return (string)GetValue(DefaultSortingProperty); }
			set { SetValue(DefaultSortingProperty, value); }
		}
		[Category(DataCategory)]
		public Type ElementType {
			get { return (Type)GetValue(ElementTypeProperty); }
			set { SetValue(ElementTypeProperty, value); }
		}
		[Category(DataCategory)]
		public string KeyExpression {
			get { return (string)GetValue(KeyExpressionProperty); }
			set { SetValue(KeyExpressionProperty, value); }
		}
		[Category(DataCategory)]
		public object DataServiceContext {
			get { return (object)GetValue(DataServiceContextProperty); }
			set { SetValue(DataServiceContextProperty, value); }
		}
		[Category(DataCategory)]
		public IQueryable Query {
			get { return (IQueryable)GetValue(QueryProperty); }
			set { SetValue(QueryProperty, value); }
		}
		[Category(DataCategory)]
		public bool UseExtendedDataQuery {
			get { return (bool)GetValue(UseExtendedDataQueryProperty); }
			set { SetValue(UseExtendedDataQueryProperty, value); }
		}
		[Category(DataCategory)]
		public Uri ServiceRoot {
			get { return (Uri)GetValue(ServiceRootProperty); }
			set { SetValue(ServiceRootProperty, value); }
		}
		public event GetEnumerableEventHandler ExceptionThrown {
			add { this.AddHandler(ExceptionThrownEvent, value); }
			remove { this.RemoveHandler(ExceptionThrownEvent, value); }
		}
		public event RoutedEventHandler InconsistencyDetected {
			add { this.AddHandler(InconsistencyDetectedEvent, value); }
			remove { this.RemoveHandler(InconsistencyDetectedEvent, value); }
		}
		public void Reload() {
			if(Data != null) {
				((WcfServerModeSource)Data).Reload();
			}
		}
	}
}
