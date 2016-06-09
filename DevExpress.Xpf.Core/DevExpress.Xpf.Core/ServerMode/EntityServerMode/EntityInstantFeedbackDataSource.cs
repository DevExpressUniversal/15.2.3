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
using System.Windows.Input;
using DevExpress.Data.Linq;
using DevExpress.Xpf.Core.DataSources;
namespace DevExpress.Xpf.Core.ServerMode {
	[DefaultEvent("GetQueryable")]
	public class EntityInstantFeedbackDataSource : ListSourceDataSourceBase, IQueryableServerModeDataSource, IDisposable {
		public static readonly DependencyProperty AreSourceRowsThreadSafeProperty;
		public static readonly DependencyProperty DefaultSortingProperty;
		public static readonly DependencyProperty KeyExpressionProperty;
		public static readonly DependencyProperty QueryableSourceProperty;
		public static readonly RoutedEvent GetQueryableEvent;
		public static readonly RoutedEvent DismissQueryableEvent;
		volatile IQueryable queryableSourceField;
		volatile bool isDisposed;
		static void OnAreSourceRowsThreadSafeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			EntityInstantFeedbackSource data = ((EntityInstantFeedbackDataSource)d).Data as EntityInstantFeedbackSource;
			if(data != null) {
				data.AreSourceRowsThreadSafe = (bool)e.NewValue;
			}
		}
		static void OnDefaultSortingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			EntityInstantFeedbackSource data = ((EntityInstantFeedbackDataSource)d).Data as EntityInstantFeedbackSource;
			if(data != null) {
				data.DefaultSorting = (string)e.NewValue;
			}
		}
		static void OnKeyExpressionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			EntityInstantFeedbackSource data = ((EntityInstantFeedbackDataSource)d).Data as EntityInstantFeedbackSource;
			if(data != null) {
				data.KeyExpression = (string)e.NewValue;
			}
		}
		static void OnQueryableSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			EntityInstantFeedbackDataSource dataSource = (EntityInstantFeedbackDataSource)d;
			dataSource.queryableSourceField = dataSource.QueryableSource;
			dataSource.ResetEntityInstantFeedbackSource();
		}
		static EntityInstantFeedbackDataSource() {
			Type ownerType = typeof(EntityInstantFeedbackDataSource);
			AreSourceRowsThreadSafeProperty = DependencyProperty.Register("AreSourceRowsThreadSafe", typeof(bool),
				ownerType, new PropertyMetadata(false, OnAreSourceRowsThreadSafeChanged));
			DefaultSortingProperty = DependencyProperty.Register("DefaultSorting", typeof(string),
				ownerType, new PropertyMetadata(String.Empty, OnDefaultSortingChanged));
			KeyExpressionProperty = DependencyProperty.Register("KeyExpression", typeof(string),
				ownerType, new PropertyMetadata(String.Empty, OnKeyExpressionChanged));
			QueryableSourceProperty = DependencyProperty.Register("QueryableSource", typeof(IQueryable),
				ownerType, new PropertyMetadata(null, OnQueryableSourceChanged));
			GetQueryableEvent = EventManager.RegisterRoutedEvent("GetQueryable", RoutingStrategy.Direct,
				typeof(GetQueryableEventHandler), ownerType);
			DismissQueryableEvent = EventManager.RegisterRoutedEvent("DismissQueryable", RoutingStrategy.Direct,
				typeof(GetQueryableEventHandler), ownerType);
		}
		void Data_GetQueryable(object sender, DevExpress.Data.Linq.GetQueryableEventArgs e) {
			GetQueryableEventArgs args = new GetQueryableEventArgs();
			RaiseGetQueryable(args);
			if(args.Handled) {
				e.AreSourceRowsThreadSafe = args.AreSourceRowsThreadSafe;
				e.KeyExpression = args.KeyExpression;
				e.QueryableSource = args.QueryableSource;
				e.Tag = args.Tag;
				return;
			}
			IQueryable queryableSource = queryableSourceField;
			if(queryableSource != null) {
				e.QueryableSource = queryableSource;
				return;
			}
		}
		void Data_DismissQueryable(object sender, DevExpress.Data.Linq.GetQueryableEventArgs e) {
			if(isDisposed) {
				EntityInstantFeedbackSource entitySource = (EntityInstantFeedbackSource)sender;
				entitySource.GetQueryable -= new EventHandler<DevExpress.Data.Linq.GetQueryableEventArgs>(Data_GetQueryable);
				entitySource.DismissQueryable -= new EventHandler<DevExpress.Data.Linq.GetQueryableEventArgs>(Data_DismissQueryable);
			}
			GetQueryableEventArgs args = new GetQueryableEventArgs();
			RaiseDismissQueryable(args);
			if(args.Handled) {
				e.AreSourceRowsThreadSafe = args.AreSourceRowsThreadSafe;
				e.KeyExpression = args.KeyExpression;
				e.QueryableSource = args.QueryableSource;
				e.Tag = args.Tag;
			}
		}
		void ResetEntityInstantFeedbackSource() {
			if(Data != null) {
				EntityInstantFeedbackSource oldSource = (EntityInstantFeedbackSource)Data;
				oldSource.Dispose();
			}
			if(DesignerProperties.GetIsInDesignMode(this)) {
				Data = null;
			} else {
				EntityInstantFeedbackSource newSource = new EntityInstantFeedbackSource();
				newSource.AreSourceRowsThreadSafe = AreSourceRowsThreadSafe;
				newSource.DefaultSorting = DefaultSorting;
				newSource.KeyExpression = KeyExpression;
				newSource.GetQueryable += new EventHandler<DevExpress.Data.Linq.GetQueryableEventArgs>(Data_GetQueryable);
				newSource.DismissQueryable += new EventHandler<DevExpress.Data.Linq.GetQueryableEventArgs>(Data_DismissQueryable);
				Data = newSource;
			}
		}
		protected virtual void RaiseGetQueryable(GetQueryableEventArgs args) {
			args.RoutedEvent = GetQueryableEvent;
			this.RaiseEvent(args);
		}
		protected virtual void RaiseDismissQueryable(GetQueryableEventArgs args) {
			args.RoutedEvent = DismissQueryableEvent;
			this.RaiseEvent(args);
		}
		protected override DataSourceStrategyBase CreateDataSourceStrategy() {
			return new QueryableServerModeDataSourceStrategy(this);
		}
		protected override string GetDesignTimeImageName() {
			return "DevExpress.Xpf.Core.Core.Images.EntityInstantFeedbackDataSource.png";
		}
		public EntityInstantFeedbackDataSource() {
			ResetEntityInstantFeedbackSource();
			DisposeCommand = DevExpress.Mvvm.Native.DelegateCommandFactory.Create<object>(
				new Action<object>((parameter) => Dispose()),
				new Func<object, bool>((parameter) => true), false);
		}
		[Category(DataCategory)]
		public bool AreSourceRowsThreadSafe {
			get { return (bool)GetValue(AreSourceRowsThreadSafeProperty); }
			set { SetValue(AreSourceRowsThreadSafeProperty, value); }
		}
		[Category(DataCategory)]
		public string DefaultSorting {
			get { return (string)GetValue(DefaultSortingProperty); }
			set { SetValue(DefaultSortingProperty, value); }
		}
		[Category(DataCategory)]
		public string KeyExpression {
			get { return (string)GetValue(KeyExpressionProperty); }
			set { SetValue(KeyExpressionProperty, value); }
		}
		[Category(DataCategory)]
		public IQueryable QueryableSource {
			get { return (IQueryable)GetValue(QueryableSourceProperty); }
			set { SetValue(QueryableSourceProperty, value); }
		}
		public ICommand DisposeCommand { get; private set; }
		public event GetQueryableEventHandler GetQueryable {
			add { this.AddHandler(GetQueryableEvent, value); }
			remove { this.RemoveHandler(GetQueryableEvent, value); }
		}
		public event GetQueryableEventHandler DismissQueryable {
			add { this.AddHandler(DismissQueryableEvent, value); }
			remove { this.RemoveHandler(DismissQueryableEvent, value); }
		}
		public void Refresh() {
			if(Data != null) {
				((EntityInstantFeedbackSource)Data).Refresh();
			}
		}
		public void Dispose() {
			isDisposed = true;
			if(Data != null) {
				((EntityInstantFeedbackSource)Data).Dispose();
			}
		}
	}
}
