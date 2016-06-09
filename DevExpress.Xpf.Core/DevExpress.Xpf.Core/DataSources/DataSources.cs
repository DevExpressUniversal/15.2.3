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
using System.Windows;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core.ServerMode;
using System.ComponentModel;
using System.Collections;
using DevExpress.Xpf.Core.Native;
using System.Linq;
using System.Collections.Generic;
#if SILVERLIGHT
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using DependencyPropertyChangedEventHandler = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventHandler;
using PLinqInstantFeedbackDataSource = DevExpress.Xpf.Core.ServerMode.LinqToObjectsInstantFeedbackDataSource;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Data.Browsing;
#endif
namespace DevExpress.Xpf.Core.DataSources {
#if !SL
	#region EntityFramework
	public class EntityCollectionViewSource : CollectionViewDataSourceBase {
		protected override DataSourceStrategyBase CreateDataSourceStrategy() {
			return new GenericPropertyDataSourceStrategy(this);
		}
		protected override BaseDataSourceStrategySelector CreateDataSourceStrategySelector() {
			return new EntityFrameworkStrategySelector();
		}
	}
	public class EntitySimpleDataSource : SimpleDataSource {
		protected override DataSourceStrategyBase CreateDataSourceStrategy() {
			return new GenericPropertyDataSourceStrategy(this);
		}
		protected override BaseDataSourceStrategySelector CreateDataSourceStrategySelector() {
			return new EntityFrameworkStrategySelector();
		}
	}
	public class EntityPLinqServerModeDataSource : PLinqServerModeDataSourceBase {
		protected override DataSourceStrategyBase CreateDataSourceStrategy() {
			return new GenericPropertyDataSourceStrategy(this);
		}
		protected override BaseDataSourceStrategySelector CreateDataSourceStrategySelector() {
			return new EntityFrameworkStrategySelector();
		}
	}
	public class EntityPLinqInstantFeedbackDataSource : PLinqInstantFeedbackDataSourceBase {
		protected override DataSourceStrategyBase CreateDataSourceStrategy() {
			return new GenericPropertyDataSourceStrategy(this);
		}
		protected override BaseDataSourceStrategySelector CreateDataSourceStrategySelector() {
			return new EntityFrameworkStrategySelector();
		}
	}
	#endregion
	#region Typed Data Source
	public class TypedCollectionViewSource : CollectionViewDataSourceBase, ITypedDataSource {
		public static readonly DependencyProperty AdapterTypeProperty;
		static TypedCollectionViewSource() {
			Type ownerType = typeof(TypedCollectionViewSource);
			AdapterTypeProperty = DependencyPropertyManager.Register("AdapterType", typeof(Type), ownerType,
				new FrameworkPropertyMetadata(new PropertyChangedCallback((d, e) => ((TypedCollectionViewSource)d).UpdateData())));
		}
		public Type AdapterType {
			get { return (Type)GetValue(AdapterTypeProperty); }
			set { SetValue(AdapterTypeProperty, value); }
		}
		protected override DataSourceStrategyBase CreateDataSourceStrategy() {
			return new TypedDataSourceStrategy(this);
		}
	}
	public class TypedSimpleSource : SimpleDataSource, ITypedDataSource {
		public static readonly DependencyProperty AdapterTypeProperty;
		static TypedSimpleSource() {
			Type ownerType = typeof(TypedSimpleSource);
			AdapterTypeProperty = DependencyPropertyManager.Register("AdapterType", typeof(Type), ownerType,
				new FrameworkPropertyMetadata(new PropertyChangedCallback((d, e) => ((TypedSimpleSource)d).UpdateData())));
		}
		public Type AdapterType {
			get { return (Type)GetValue(AdapterTypeProperty); }
			set { SetValue(AdapterTypeProperty, value); }
		}
		protected override DataSourceStrategyBase CreateDataSourceStrategy() {
			return new TypedDataSourceStrategy(this);
		}
	}
	#endregion
	#region Linq to SQL
	public class LinqSimpleDataSource : EnumerableDataSourceBase {
		protected override DataSourceStrategyBase CreateDataSourceStrategy() {
			return new GenericPropertyDataSourceStrategy(this);
		}
	}
	public class LinqCollectionViewDataSource : CollectionViewDataSourceBase {
		protected override DataSourceStrategyBase CreateDataSourceStrategy() {
			return new GenericPropertyDataSourceStrategy(this);
		}
	}
	public class LinqPlinqServerModeDataSource : PLinqServerModeDataSourceBase {
		protected override DataSourceStrategyBase CreateDataSourceStrategy() {
			return new GenericPropertyDataSourceStrategy(this);
		}
	}
	public class LinqPlinqInstantFeedbackDataSource : PLinqInstantFeedbackDataSourceBase {
		protected override DataSourceStrategyBase CreateDataSourceStrategy() {
			return new GenericPropertyDataSourceStrategy(this);
		}
	}
	#endregion
#endif
	#region IEnumerable
	public class IEnumerableDataSource : ItemsSourceDataSourceBase {
		private static readonly DependencyPropertyKey DataPropertyKey;
		public static readonly DependencyProperty DataProperty;
		static IEnumerableDataSource() {
			Type ownerType = typeof(IEnumerableDataSource);
			DataPropertyKey = DependencyPropertyManager.RegisterReadOnly("Data", typeof(IEnumerable), ownerType, new FrameworkPropertyMetadata());
			DataProperty = DataPropertyKey.DependencyProperty;
		}
		public IEnumerable Data {
			get { return (IEnumerable)GetValue(DataProperty); }
			protected set { this.SetValue(DataPropertyKey, value); }
		}
		protected internal override object DataCore {
			get { return Data; }
			set { Data = value as IEnumerable; }
		}
		protected override bool CanUpdateFromDesignData() {
			if(!base.CanUpdateFromDesignData())
				return false;
			return DesignData.DataObjectType != null || ItemsSource != null;
		}
		protected override object CreateDesignTimeDataSourceCore() {
			Type dataObjectType = DesignData.DataObjectType ?? DataSourceHelper.ExtractEnumerableType(ItemsSource);
			return dataObjectType != null ? new BaseGridDesignTimeDataSource(dataObjectType, DesignData.RowCount, DesignData.UseDistinctValues, DesignData.FlattenHierarchy) : null;
		}
		protected override object UpdateDataCore() {
			return ItemsSource;
		}
	}
	#endregion
}
