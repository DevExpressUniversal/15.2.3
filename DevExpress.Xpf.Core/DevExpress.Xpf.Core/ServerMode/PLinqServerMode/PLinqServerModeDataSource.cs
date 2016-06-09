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
using DevExpress.Data.PLinq;
using DevExpress.Xpf.Core.DataSources;
using DevExpress.Data.PLinq.Helpers;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Core.ServerMode {
	public class PLinqServerModeDataSource : PLinqDataSourceBase {
		public static readonly DependencyProperty DefaultSortingProperty;
		public static readonly DependencyProperty ElementTypeProperty;
		public static readonly DependencyProperty ListSourceProperty;
		static void OnDefaultSortingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PLinqServerModeSource data = ((PLinqServerModeDataSource)d).Data as PLinqServerModeSource;
			if(data != null) {
				data.DefaultSorting = (string)e.NewValue;
			}
		}
		static void OnElementTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PLinqServerModeDataSource instance = (PLinqServerModeDataSource)d;
			PLinqServerModeSource data = instance.Data as PLinqServerModeSource;
			if(data != null)
				data.ElementType = (Type)e.NewValue;
			if(DesignerProperties.GetIsInDesignMode(instance))
				instance.UpdateData();
		}
		static void OnListSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PLinqServerModeDataSource dataSource = (PLinqServerModeDataSource)d;
			dataSource.UpdateData();
		}
		static PLinqServerModeDataSource() {
			Type ownerType = typeof(PLinqServerModeDataSource);
			DefaultSortingProperty = DependencyProperty.Register("DefaultSorting", typeof(string),
				ownerType, new PropertyMetadata(String.Empty, OnDefaultSortingChanged));
			ElementTypeProperty = DependencyProperty.Register("ElementType", typeof(Type),
				ownerType, new PropertyMetadata(null, OnElementTypeChanged));
			ListSourceProperty = DependencyProperty.Register("ListSource", typeof(IListSource),
				ownerType, new PropertyMetadata(null, OnListSourceChanged));
		}
		void ResetPLinqServerModeSource() {
			if(Data != null) {
				PLinqServerModeSource oldSource = (PLinqServerModeSource)Data;
				oldSource.Dispose();
			}
			if(DesignerProperties.GetIsInDesignMode(this)) {
				Data = null;
			} else {
				PLinqServerModeSource newSource = new PLinqServerModeSource();
				newSource.DefaultSorting = DefaultSorting;
				newSource.ElementType = ElementType;
				if(ItemsSource != null) {
					newSource.Source = ItemsSource;
				}
				if(ListSource != null) {
					newSource.Source = ListSource.GetList();
				}
				Data = newSource;
			}
		}
		protected override Type GetDataObjectType() {
			IEnumerable source = ListSource != null ? ListSource.GetList() : ItemsSource;
			return ElementType ?? DataSourceHelper.ExtractEnumerableType(source);
		}
		protected override object UpdateDataCore() {
			ResetPLinqServerModeSource();
			return Data;
		}
		protected override string GetDesignTimeImageName() {
			return "DevExpress.Xpf.Core.Core.Images.PLinqServerModeDataSource.png";
		}
		public PLinqServerModeDataSource() {
			ResetPLinqServerModeSource();
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
		public IListSource ListSource {
			get { return (IListSource)GetValue(ListSourceProperty); }
			set { SetValue(ListSourceProperty, value); }
		}
		public void Reload() {
			if(Data != null) {
				((PLinqServerModeSource)Data).Reload();
			}
		}
	}
}
