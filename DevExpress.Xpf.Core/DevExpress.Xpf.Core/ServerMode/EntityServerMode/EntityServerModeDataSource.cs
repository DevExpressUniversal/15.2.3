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
using DevExpress.Data.Linq;
using DevExpress.Xpf.Core.DataSources;
namespace DevExpress.Xpf.Core.ServerMode {
	public class EntityServerModeDataSource : ListSourceDataSourceBase, IQueryableServerModeDataSource {
		public static readonly DependencyProperty DefaultSortingProperty;
		public static readonly DependencyProperty ElementTypeProperty;
		public static readonly DependencyProperty KeyExpressionProperty;
		public static readonly DependencyProperty QueryableSourceProperty;
		public static readonly RoutedEvent ExceptionThrownEvent;
		public static readonly RoutedEvent InconsistencyDetectedEvent;
		static void OnDefaultSortingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			EntityServerModeSource data = ((EntityServerModeDataSource)d).Data as EntityServerModeSource;
			if(data != null) {
				data.DefaultSorting = (string)e.NewValue;
			}
		}
		static void OnElementTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			EntityServerModeSource data = ((EntityServerModeDataSource)d).Data as EntityServerModeSource;
			if(data != null) {
				data.ElementType = (Type)e.NewValue;
			}
		}
		static void OnKeyExpressionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			EntityServerModeSource data = ((EntityServerModeDataSource)d).Data as EntityServerModeSource;
			if(data != null) {
				data.KeyExpression = (string)e.NewValue;
			}
		}
		static void OnQueryableSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			EntityServerModeDataSource dataSource = (EntityServerModeDataSource)d;
			dataSource.ResetEntityServerModeSource();
		}
		static EntityServerModeDataSource() {
			Type ownerType = typeof(EntityServerModeDataSource);
			DefaultSortingProperty = DependencyProperty.Register("DefaultSorting", typeof(string),
				ownerType, new PropertyMetadata(String.Empty, OnDefaultSortingChanged));
			ElementTypeProperty = DependencyProperty.Register("ElementType", typeof(Type),
				ownerType, new PropertyMetadata(null, OnElementTypeChanged));
			KeyExpressionProperty = DependencyProperty.Register("KeyExpression", typeof(string),
				ownerType, new PropertyMetadata(String.Empty, OnKeyExpressionChanged));
			QueryableSourceProperty = DependencyProperty.Register("QueryableSource", typeof(IQueryable),
				ownerType, new PropertyMetadata(null, OnQueryableSourceChanged));
			ExceptionThrownEvent = EventManager.RegisterRoutedEvent("ExceptionThrown", RoutingStrategy.Direct,
				typeof(ServerModeExceptionThrownEventHandler), ownerType);
			InconsistencyDetectedEvent = EventManager.RegisterRoutedEvent("InconsistencyDetected", RoutingStrategy.Direct,
				typeof(ServerModeInconsistencyDetectedEventHandler), ownerType);
		}
		void entityServerModeSource_ExceptionThrown(object sender, DevExpress.Data.Linq.LinqServerModeExceptionThrownEventArgs e) {
			ServerModeExceptionThrownEventArgs args = new ServerModeExceptionThrownEventArgs(e.Exception);
			RaiseExceptionThrown(args);
		}
		void entityServerModeSource_InconsistencyDetected(object sender, DevExpress.Data.Linq.LinqServerModeInconsistencyDetectedEventArgs e) {
			ServerModeInconsistencyDetectedEventArgs args = new ServerModeInconsistencyDetectedEventArgs();
			RaiseInconsistencyDetected(args);
		}
		void ResetEntityServerModeSource() {
			if(Data != null) {
				EntityServerModeSource oldSource = (EntityServerModeSource)Data;
				oldSource.Dispose();
				oldSource.ExceptionThrown -= new DevExpress.Data.Linq.LinqServerModeExceptionThrownEventHandler(entityServerModeSource_ExceptionThrown);
				oldSource.InconsistencyDetected -= new DevExpress.Data.Linq.LinqServerModeInconsistencyDetectedEventHandler(entityServerModeSource_InconsistencyDetected);
			}
			if(DesignerProperties.GetIsInDesignMode(this)) {
				Data = null;
			} else {
				EntityServerModeSource newSource = new EntityServerModeSource();
				newSource.DefaultSorting = DefaultSorting;
				newSource.ElementType = ElementType;
				newSource.KeyExpression = KeyExpression;
				newSource.QueryableSource = QueryableSource;
				newSource.ExceptionThrown += new DevExpress.Data.Linq.LinqServerModeExceptionThrownEventHandler(entityServerModeSource_ExceptionThrown);
				newSource.InconsistencyDetected += new DevExpress.Data.Linq.LinqServerModeInconsistencyDetectedEventHandler(entityServerModeSource_InconsistencyDetected);
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
		protected override DataSourceStrategyBase CreateDataSourceStrategy() {
			return new QueryableServerModeDataSourceStrategy(this);
		}
		protected override string GetDesignTimeImageName() {
			return "DevExpress.Xpf.Core.Core.Images.EntityServerModeDataSource.png";
		}
		public EntityServerModeDataSource() {
			ResetEntityServerModeSource();
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
		public IQueryable QueryableSource {
			get { return (IQueryable)GetValue(QueryableSourceProperty); }
			set { SetValue(QueryableSourceProperty, value); }
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
				((EntityServerModeSource)Data).Reload();
			}
		}
	}
}
