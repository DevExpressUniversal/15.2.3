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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using DevExpress.Xpf.Charts;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.Charts.Designer.Native {
	public class BarPopupDataBrowserEditViewModel : BarEditValueItemViewModel {
		public static readonly DependencyProperty DataSourceProperty = DependencyProperty.Register("DataSource", typeof(object), typeof(BarPopupDataBrowserEditViewModel),
			new PropertyMetadata(ValuePropertyChanged));
		public static readonly DependencyProperty ScaleTypeProperty = DependencyProperty.Register("ScaleType", typeof(ScaleType), typeof(BarPopupDataBrowserEditViewModel),
			new PropertyMetadata(ValuePropertyChanged));
		static void ValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			BarPopupDataBrowserEditViewModel model = d as BarPopupDataBrowserEditViewModel;
			if (model != null)
				model.UpdatePopupTree();
		}
		List<DataBrowserTreeNode> itemsSource;
		bool isValueDatamember;
		public List<DataBrowserTreeNode> ItemsSource { 
			get { return itemsSource; } 
		}
		public object DataSource {
			get { return GetValue(DataSourceProperty); }
			set { SetValue(DataSourceProperty, value); }
		}
		public ScaleType ScaleType {
			get { return (ScaleType)GetValue(ScaleTypeProperty); }
			set { SetValue(ScaleTypeProperty, value); }
		}
		public BarPopupDataBrowserEditViewModel(ChartCommandBase command, ChartModelElement source, string editValuePath, IValueConverter converter, bool isValueDatamember)
			: this(command, source, editValuePath, converter, null, isValueDatamember) {
		}
		public BarPopupDataBrowserEditViewModel(ChartCommandBase command, ChartModelElement source, string editValuePath, IValueConverter converter, IRibbonBehavior behavior, bool isValueDatamember)
			: base(command, source, editValuePath, converter, behavior) {
			this.isValueDatamember = isValueDatamember;
			Binding dataSourceBinding = new Binding(editValuePath) { Source = source, Converter = new DataSourceConverter() };
			BindingOperations.SetBinding(this, DataSourceProperty, dataSourceBinding);
			Binding scaleTypeBinding = new Binding(editValuePath) { Source = source, Converter = new ScaleTypeConverter(), ConverterParameter = command };
			BindingOperations.SetBinding(this, ScaleTypeProperty, scaleTypeBinding);
		}
		void UpdatePopupTree() {
			ScaleType[] allowedScaleTypes;
			if (ScaleType == ScaleType.Auto)
				allowedScaleTypes = isValueDatamember
					? new ScaleType[] { ScaleType.Numerical, ScaleType.DateTime }
					: new ScaleType[] { ScaleType.Qualitative, ScaleType.Numerical, ScaleType.DateTime };
			else
				allowedScaleTypes = new ScaleType[] { ScaleType };
			DataBrowserPickManager pickManager = new DataBrowserPickManager(allowedScaleTypes);
			List<DataBrowserTreeNode> nodes = new List<DataBrowserTreeNode>();
			Collection<Pair<object, string>> dataSources = new Collection<Pair<object, string>>();
			dataSources.Add(new Pair<object, string>(DataSource, ""));
			pickManager.FillContent(nodes, dataSources, true);
			itemsSource = nodes;
		}
	}
}
