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
	public class LinqServerModeDataSource : ListSourceDataSourceBase, IQueryableServerModeDataSource {
		public static readonly DependencyProperty DefaultSortingProperty;
		public static readonly DependencyProperty ElementTypeProperty;
		public static readonly DependencyProperty KeyExpressionProperty;
		public static readonly DependencyProperty QueryableSourceProperty;
		public static readonly RoutedEvent ExceptionThrownEvent;
		public static readonly RoutedEvent InconsistencyDetectedEvent;
		static void OnDefaultSortingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			LinqServerModeSource data = ((LinqServerModeDataSource)d).Data as LinqServerModeSource;
			if(data != null) {
				data.DefaultSorting = (string)e.NewValue;
			}
		}
		static void OnElementTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			LinqServerModeSource data = ((LinqServerModeDataSource)d).Data as LinqServerModeSource;
			if(data != null) {
				data.ElementType = (Type)e.NewValue;
			}
		}
		static void OnKeyExpressionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			LinqServerModeSource data = ((LinqServerModeDataSource)d).Data as LinqServerModeSource;
			if(data != null) {
				data.KeyExpression = (string)e.NewValue;
			}
		}
		static void OnQueryableSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			LinqServerModeDataSource dataSource = (LinqServerModeDataSource)d;
			dataSource.ResetLinqServerModeSource();
		}
		static LinqServerModeDataSource() {
			Type ownerType = typeof(LinqServerModeDataSource);
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
		void linqServerModeSource_ExceptionThrown(object sender, DevExpress.Data.Linq.LinqServerModeExceptionThrownEventArgs e) {
			ServerModeExceptionThrownEventArgs args = new ServerModeExceptionThrownEventArgs(e.Exception);
			RaiseExceptionThrown(args);
		}
		void linqServerModeSource_InconsistencyDetected(object sender, DevExpress.Data.Linq.LinqServerModeInconsistencyDetectedEventArgs e) {
			ServerModeInconsistencyDetectedEventArgs args = new ServerModeInconsistencyDetectedEventArgs();
			RaiseInconsistencyDetected(args);
		}
		void ResetLinqServerModeSource() {
			if(Data != null) {
				LinqServerModeSource oldSource = (LinqServerModeSource)Data;
				oldSource.Dispose();
				oldSource.ExceptionThrown -= new DevExpress.Data.Linq.LinqServerModeExceptionThrownEventHandler(linqServerModeSource_ExceptionThrown);
				oldSource.InconsistencyDetected -= new DevExpress.Data.Linq.LinqServerModeInconsistencyDetectedEventHandler(linqServerModeSource_InconsistencyDetected);
			}
			if(DesignerProperties.GetIsInDesignMode(this)) {
				Data = null;
			} else {
				LinqServerModeSource newSource = new LinqServerModeSource();
				newSource.DefaultSorting = DefaultSorting;
				newSource.ElementType = ElementType;
				newSource.KeyExpression = KeyExpression;
				newSource.QueryableSource = QueryableSource;
				newSource.ExceptionThrown += new DevExpress.Data.Linq.LinqServerModeExceptionThrownEventHandler(linqServerModeSource_ExceptionThrown);
				newSource.InconsistencyDetected += new DevExpress.Data.Linq.LinqServerModeInconsistencyDetectedEventHandler(linqServerModeSource_InconsistencyDetected);
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
			return "DevExpress.Xpf.Core.Core.Images.LinqServerModeDataSource.png";
		}
		public LinqServerModeDataSource() {
			ResetLinqServerModeSource();
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
				((LinqServerModeSource)Data).Reload();
			}
		}
	}
	public delegate void ServerModeExceptionThrownEventHandler(object sender, ServerModeExceptionThrownEventArgs e);
	public delegate void ServerModeInconsistencyDetectedEventHandler(object sender, ServerModeInconsistencyDetectedEventArgs e);
	public class ServerModeExceptionThrownEventArgs : RoutedEventArgs {
		Exception exception;
		public ServerModeExceptionThrownEventArgs(Exception exception) {
			this.exception = exception;
		}
		public Exception Exception { get { return exception; } }
	}
	public class ServerModeInconsistencyDetectedEventArgs : RoutedEventArgs { }
}
