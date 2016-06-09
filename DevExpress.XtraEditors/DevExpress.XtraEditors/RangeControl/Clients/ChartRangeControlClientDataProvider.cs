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
using System.ComponentModel;
using System.Drawing.Design;
using DevExpress.ChartRangeControlClient.Core;
using DevExpress.Sparkline.Core;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors.Design;
namespace DevExpress.XtraEditors {
	[
	TypeConverter(typeof(ExpandableObjectConverter))
	]
	public class ChartRangeControlClientDataProvider : IDisposable {
		#region Nested Classes: ChartRangeControlClientDataProvider, InternalBindingSourceDelegate
		class InternalClientDataProvider : IClientDataProvider {
			readonly ChartRangeControlClientDataProvider provider;
			DataChangedDelegate dataChanged;
			public InternalClientDataProvider(ChartRangeControlClientDataProvider provider) {
				this.provider = provider;
			}
			#region IClientDataProvider
			IList<IClientSeries> IClientDataProvider.Series {
				get { return provider.Series; }
			}
			void IClientDataProvider.SetDataChangedDelegate(DataChangedDelegate dataChangedDelegate) {
				this.dataChanged = dataChangedDelegate;
			}
			#endregion
			public void RaiseDataChanged() {
				if (dataChanged != null)
					dataChanged(this);
			}
		}
		class InternalBindingSourceDelegate : IBindingSourceDelegate {
			readonly ChartRangeControlClientDataProvider provider;
			public InternalBindingSourceDelegate(ChartRangeControlClientDataProvider provider) {
				this.provider = provider;
			}
			#region IBindingSourceDelegate
			void IBindingSourceDelegate.AdjustSeries(object dataSourceValue, BindingSourceClientSeries series, int seriesCounter) {
				provider.BindingDelegateAdjustSeries(dataSourceValue, series, seriesCounter);
			}
			void IBindingSourceDelegate.BindingChanged() {
				provider.BindingDelegateBindingChanged();
			}
			#endregion
		}
		#endregion
		const string fakeArgumentDataMember = "Argument";
		const string fakeValueDataMember = "Value";
		const string fakeSeriesDataMember = "Series";
		readonly BindingSource bindingSource;
		readonly InternalClientDataProvider internalDataProvider;
		readonly InternalBindingSourceDelegate internalSourceDelegate;
		readonly ChartRangeControlClientBase client;
		ChartRangeControlClientView templateView;
		object dataSource = null;
		string argumentDataMember = null;
		string valueDataMember = null;
		string seriesDataMember = null;
		object fakeDataSource = null;
		IList<IClientSeries> Series {
			get { return bindingSource.GetSeries(); }
		}
		object FakeDataSource {
			get {
				if (fakeDataSource == null)
					fakeDataSource = client.CreateFakeData();
				return fakeDataSource;
			}
		}
		bool ShouldApplyFakeData {
			get { return client.IsDesignMode && (DataSource == null); }
		}
		internal BindingSource BindingSource {
			get { return bindingSource; }
		}
		internal IClientDataProvider ClientDataProvider {
			get { return internalDataProvider; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ChartRangeControlClientDataProviderDataSource"),
#endif
		XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.ChartRangeControlClient.ClientDataSourceProvider.DataSource"),
		DefaultValue(null),
		AttributeProvider(typeof(IListSource))
		]
		public object DataSource {
			get { return dataSource; }
			set {
				if (dataSource != value) {
					dataSource = value;
					UpdateBindingSource(true);
				}
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ChartRangeControlClientDataProviderSeriesDataMember"),
#endif
		XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.ChartRangeControlClient.ClientDataSourceProvider.SeriesDataMember"),
		DefaultValue(null),
		Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design", typeof(UITypeEditor))
		]
		public string SeriesDataMember {
			get { return seriesDataMember; }
			set {
				if (seriesDataMember != value) {
					seriesDataMember = value;
					UpdateBindingSource(true);
				}
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ChartRangeControlClientDataProviderArgumentDataMember"),
#endif
		XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.ChartRangeControlClient.ClientDataSourceProvider.ArgumentDataMember"),
		DefaultValue(null),
		Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design", typeof(UITypeEditor))
		]
		public string ArgumentDataMember {
			get { return argumentDataMember; }
			set {
				if (argumentDataMember != value) {
					argumentDataMember = value;
					UpdateBindingSource(true);
				}
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ChartRangeControlClientDataProviderValueDataMember"),
#endif
		XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.ChartRangeControlClient.ClientDataSourceProvider.ValueDataMember"),
		DefaultValue(null),
		Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design", typeof(UITypeEditor))
		]
		public string ValueDataMember {
			get { return valueDataMember; }
			set {
				if (valueDataMember != value) {
					valueDataMember = value;
					UpdateBindingSource(true);
				}
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ChartRangeControlClientDataProviderTemplateView"),
#endif
		XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.ChartRangeControlClient.TemplateView"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Editor(typeof(RangeControlClientViewEditor), typeof(UITypeEditor)),
		RefreshProperties(RefreshProperties.All)
		]
		public ChartRangeControlClientView TemplateView {
			get { return templateView; }
			set {
				if (templateView != value) {
					SubscribeViewChanged(null);
					templateView = value;
					SubscribeViewChanged(ViewChanged);
					ViewChanged();
				}
			}
		}
		internal ChartRangeControlClientDataProvider(ChartRangeControlClientBase client, SparklineScaleType scaleType) {
			this.client = client;
			this.templateView = new LineChartRangeControlClientView();
			this.internalDataProvider = new InternalClientDataProvider(this);
			this.internalSourceDelegate = new InternalBindingSourceDelegate(this);
			this.bindingSource = new BindingSource(scaleType, internalSourceDelegate);
			SubscribeViewChanged(ViewChanged);
		}
		#region IDisposable
		public void Dispose() {
			bindingSource.Dispose();
			SubscribeViewChanged(null);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeTemplateView() {
			return (templateView == null) || (templateView.GetType() != typeof(LineChartRangeControlClientView)) || templateView.ShouldSerialize();
		}
		void ResetTemplateView() {
			TemplateView = new LineChartRangeControlClientView();
		}
		#endregion
		void SubscribeViewChanged(ChartRangeControlClientView.ChangedDelegate changedDelegate) {
			if (templateView != null)
				templateView.Changed = changedDelegate;
		}
		void ViewChanged() {
			RefreshData();
		}
		void BindingDelegateBindingChanged() {
			internalDataProvider.RaiseDataChanged();	
		}
		void BindingDelegateAdjustSeries(object dataSourceValue, BindingSourceClientSeries series, int seriesCounter) {
			RaiseCustomizeSeries(dataSourceValue, series, seriesCounter);
			if ((series.View == null) && (templateView != null))
				series.View = templateView.SparklineView;
		}
		void RaiseCustomizeSeries(object dataSourceValue, BindingSourceClientSeries series, int seriesCounter) {
			ChartRangeControlClientView clone = TemplateView.Clone();
			clone.ApplyPalette(client.CurrentPalette, seriesCounter);
			ClientDataSourceProviderCustomizeSeriesEventArgs eventArgs = new ClientDataSourceProviderCustomizeSeriesEventArgs(clone, dataSourceValue);
			client.RaiseCustomizeSeries(eventArgs);
			if (eventArgs.View != null)
				series.View = eventArgs.View.SparklineView;
		}
		void ApplyFakeDataSettings() {
			bindingSource.ItemsSource = FakeDataSource;
			bindingSource.ArgumentDataMember = fakeArgumentDataMember;
			bindingSource.ValueDataMember = fakeValueDataMember;
			bindingSource.SeriesDataMember = fakeSeriesDataMember;
		}
		void ApplyRealDataSettings() {
			bindingSource.ItemsSource = dataSource;
			bindingSource.ArgumentDataMember = argumentDataMember;
			bindingSource.ValueDataMember = valueDataMember;
			bindingSource.SeriesDataMember = seriesDataMember;
		}
		internal void UpdateFakeData() {
			if (ShouldApplyFakeData) {
				fakeDataSource = null;
				bindingSource.ItemsSource = FakeDataSource;
			}
		}
		internal void UpdateBindingSource(bool invalidateRangeControl) {
			if (ShouldApplyFakeData)
				ApplyFakeDataSettings();
			else
				ApplyRealDataSettings();
			if (invalidateRangeControl)
				client.InvalidateRangeControl(true);
		}
		public void RefreshData() {
			bindingSource.RefreshData();
			client.InvalidateRangeControl(false);
		}
		public override string ToString() {
			return "(" + this.GetType().Name + ")";
		}
	}
	public sealed class ClientDataSourceProviderCustomizeSeriesEventArgs : EventArgs {
		readonly object dataSourceValue;
		ChartRangeControlClientView view;
		public ChartRangeControlClientView View {
			get { return view; }
			set { view = value; }
		}
		public object DataSourceValue {
			get { return dataSourceValue; }
		}
		internal ClientDataSourceProviderCustomizeSeriesEventArgs(ChartRangeControlClientView view, object dataSourceValue) : base() {
			this.view = view;
			this.dataSourceValue = dataSourceValue;
		}
	}
}
