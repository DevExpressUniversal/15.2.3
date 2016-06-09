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
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using DevExpress.Data.PLinq;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core.DataSources;
using DevExpress.Data.PLinq.Helpers;
using DevExpress.Xpf.Core.Native;
using System.Linq;
#if SL
using DevExpress.Data.Browsing;
using DevExpress.Xpf.Core.WPFCompatibility;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using PLinqInstantFeedbackDataSource = DevExpress.Xpf.Core.ServerMode.LinqToObjectsInstantFeedbackDataSource;
#endif
namespace DevExpress.Xpf.Core.ServerMode {
	[DefaultEvent("GetEnumerable")]
	public class
#if !SL
		PLinqInstantFeedbackDataSource
#else
		LinqToObjectsInstantFeedbackDataSource
#endif
	 	: PLinqDataSourceBase, IDisposable {
		public static readonly DependencyProperty DefaultSortingProperty;
		public static readonly DependencyProperty ListSourceProperty;
		public static readonly RoutedEvent GetEnumerableEvent;
		public static readonly RoutedEvent DismissEnumerableEvent;
		volatile IEnumerable itemsSourceField;
		volatile IListSource listSourceField;
		volatile bool isDisposed;
		static void OnDefaultSortingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PLinqInstantFeedbackSource data = ((PLinqInstantFeedbackDataSource)d).Data as PLinqInstantFeedbackSource;
			if(data != null) {
				data.DefaultSorting = (string)e.NewValue;
			}
		}
		static void OnListSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PLinqInstantFeedbackDataSource dataSource = (PLinqInstantFeedbackDataSource)d;
			dataSource.UpdateData();
		}
#if !SL
		static PLinqInstantFeedbackDataSource() {
#else
		static LinqToObjectsInstantFeedbackDataSource() {
#endif
			Type ownerType = typeof(PLinqInstantFeedbackDataSource);
			DefaultSortingProperty = DependencyPropertyManager.Register("DefaultSorting", typeof(string),
				ownerType, new PropertyMetadata(String.Empty, OnDefaultSortingChanged));
			ListSourceProperty = DependencyPropertyManager.Register("ListSource", typeof(IListSource),
				ownerType, new PropertyMetadata(null, OnListSourceChanged));
			GetEnumerableEvent = EventManager.RegisterRoutedEvent("GetEnumerable", RoutingStrategy.Direct,
				typeof(GetEnumerableEventHandler), ownerType);
			DismissEnumerableEvent = EventManager.RegisterRoutedEvent("DismissEnumerable", RoutingStrategy.Direct,
				typeof(GetEnumerableEventHandler), ownerType);
		}
		void Data_GetEnumerable(object sender, DevExpress.Data.PLinq.GetEnumerableEventArgs e) {
			GetEnumerableEventArgs args = new GetEnumerableEventArgs();
			RaiseGetEnumerable(args);
			if(args.Handled) {
				e.Source = args.ItemsSource;
				e.Tag = args.Tag;
				return;
			}
			IEnumerable itemsSource = itemsSourceField;
			if(itemsSource != null) {
				e.Source = itemsSource;
				return;
			}
			IListSource listSource = listSourceField;
			if(listSource != null) {
				e.Source = listSource.GetList();
				return;
			}
			e.Source = Enumerable.Empty<object>();
		}
		void Data_DismissEnumerable(object sender, DevExpress.Data.PLinq.GetEnumerableEventArgs e) {
			if(isDisposed) {
				PLinqInstantFeedbackSource pLinqSource = (PLinqInstantFeedbackSource)sender;
				pLinqSource.GetEnumerable -= new EventHandler<DevExpress.Data.PLinq.GetEnumerableEventArgs>(Data_GetEnumerable);
				pLinqSource.DismissEnumerable -= new EventHandler<DevExpress.Data.PLinq.GetEnumerableEventArgs>(Data_DismissEnumerable);
			}
			GetEnumerableEventArgs args = new GetEnumerableEventArgs();
			RaiseDismissEnumerable(args);
			if(args.Handled) {
				e.Source = args.ItemsSource;
				e.Tag = args.Tag;
			}
		}
		internal void ResetPLinqInstantFeedbackSource() {
			if(Data != null) {
				PLinqInstantFeedbackSource oldSource = (PLinqInstantFeedbackSource)Data;
				oldSource.Dispose();
			}
			if(DesignerProperties.GetIsInDesignMode(this)) {
				Data = null;
			} else {
				PLinqInstantFeedbackSource newSource = new PLinqInstantFeedbackSource();
				newSource.DefaultSorting = DefaultSorting;
				newSource.GetEnumerable += new EventHandler<DevExpress.Data.PLinq.GetEnumerableEventArgs>(Data_GetEnumerable);
				newSource.DismissEnumerable += new EventHandler<DevExpress.Data.PLinq.GetEnumerableEventArgs>(Data_DismissEnumerable);
				Data = newSource;
			}
		}
		protected virtual void RaiseGetEnumerable(GetEnumerableEventArgs args) {
			args.RoutedEvent = GetEnumerableEvent;
			this.RaiseEvent(args);
		}
		protected virtual void RaiseDismissEnumerable(GetEnumerableEventArgs args) {
			args.RoutedEvent = DismissEnumerableEvent;
			this.RaiseEvent(args);
		}
		protected override Type GetDataObjectType() {
			if(DesignData.DataObjectType != null)
				return DesignData.DataObjectType;
			IEnumerable source = ListSource != null ? ListSource.GetList() : ItemsSource;
			return source != null ? DataSourceHelper.ExtractEnumerableType(source) : null;
		}
		protected override object UpdateDataCore() {
			this.listSourceField = ListSource;
			this.itemsSourceField = ItemsSource;
			ResetPLinqInstantFeedbackSource();
			return Data;
		}
		protected override string GetDesignTimeImageName() {
			return "DevExpress.Xpf.Core.Core.Images.PLinqInstantFeedbackDataSource.png";
		}
#if !SL
		public PLinqInstantFeedbackDataSource() {
#else
		public LinqToObjectsInstantFeedbackDataSource() {
#endif
			ResetPLinqInstantFeedbackSource();
			DisposeCommand = DevExpress.Mvvm.Native.DelegateCommandFactory.Create<object>(
				new Action<object>((parameter) => Dispose()),
				new Func<object, bool>((parameter) => true), false);
		}
		[System.ComponentModel.Category(DataCategory)]
		public string DefaultSorting {
			get { return (string)GetValue(DefaultSortingProperty); }
			set { SetValue(DefaultSortingProperty, value); }
		}
		[System.ComponentModel.Category(DataCategory)]
		public IListSource ListSource {
			get { return (IListSource)GetValue(ListSourceProperty); }
			set { SetValue(ListSourceProperty, value); }
		}
		public ICommand DisposeCommand { get; private set; }
		public event GetEnumerableEventHandler GetEnumerable {
			add { this.AddHandler(GetEnumerableEvent, value); }
			remove { this.RemoveHandler(GetEnumerableEvent, value); }
		}
		public event GetEnumerableEventHandler DismissEnumerable {
			add { this.AddHandler(DismissEnumerableEvent, value); }
			remove { this.RemoveHandler(DismissEnumerableEvent, value); }
		}
		public void Refresh() {
			if(Data != null) {
				((PLinqInstantFeedbackSource)Data).Refresh();
			}
		}
		public void Dispose() {
			isDisposed = true;
			if(Data != null) {
				((PLinqInstantFeedbackSource)Data).Dispose();
			}
		}
	}
	public delegate void GetEnumerableEventHandler(object sender, GetEnumerableEventArgs e);
	public class GetEnumerableEventArgs : RoutedEventArgs {
		public IEnumerable ItemsSource { get; set; }
		public object Tag { get; set; }
	}
}
