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
using System.Windows.Data;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Data.Mask;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using System.Windows.Controls;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Utils;
using System.Windows.Markup;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DevExpress.Xpf.Collections;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
#else
using DevExpress.Xpf.Utils;
#endif
namespace DevExpress.Xpf.Grid {
	public class DXDetailPresenter : DXContentPresenter {
		public static readonly DependencyProperty IsContentVisibleProperty =
			DependencyPropertyManager.Register("IsContentVisible", typeof(bool), typeof(DXDetailPresenter), new PropertyMetadata(false, IsContentVisiblePropertyChanged));
		public static readonly DependencyProperty IsDetailVisibleProperty =
			DependencyPropertyManager.RegisterAttached("IsDetailVisible", typeof(bool), typeof(DXDetailPresenter), new PropertyMetadata(false));
		public static bool GetIsDetailVisible(DependencyObject obj) {
			return (bool)obj.GetValue(IsDetailVisibleProperty);
		}
		public static void SetIsDetailVisible(DependencyObject obj, bool value) {
			obj.SetValue(IsDetailVisibleProperty, value);
		}
		public bool IsContentVisible {
			get { return (bool)GetValue(IsContentVisibleProperty); }
			set { SetValue(IsContentVisibleProperty, value); }
		}
		public DataTemplate DetailTemplate {
			get { return (DataTemplate)GetValue(DetailTemplateProperty); }
			set { SetValue(DetailTemplateProperty, value); }
		}
		public static readonly DependencyProperty DetailTemplateProperty =
			DependencyPropertyManager.Register("DetailTemplate", typeof(DataTemplate), typeof(DXDetailPresenter), new PropertyMetadata(null));
		public object DetailContent {
			get { return (object)GetValue(DetailContentProperty); }
			set { SetValue(DetailContentProperty, value); }
		}
		public static readonly DependencyProperty DetailContentProperty =
			DependencyPropertyManager.Register("DetailContent", typeof(object), typeof(DXDetailPresenter), new PropertyMetadata(null));
		static void IsContentVisiblePropertyChanged(DependencyObject dObject, DependencyPropertyChangedEventArgs e) {
			((DXDetailPresenter)dObject).OnIsContentVisibleChanged();
		}
		void OnIsContentVisibleChanged() {
			if(IsContentVisible) {
				Content = new ContentPresenter() {
					Content = DetailContent,
				ContentTemplate = DetailTemplate
			};
				Visibility = Visibility.Visible;
			}
			else {
				Visibility = Visibility.Collapsed;
				Content = null;
			}
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			OnIsContentVisibleChanged();
		}
		public DXDetailPresenter() {
		}
	}
	public static class MasterDetailHelper {
		const string DataRowTemplateXAML =
			 @"<DataTemplate " +
			  "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
			  "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' " +
			  "xmlns:dx='http://schemas.devexpress.com/winfx/2008/xaml/core' " +
			  "xmlns:dxg='http://schemas.devexpress.com/winfx/2008/xaml/grid'> " +
			  "<Grid>" +
			  "<Grid.Resources>" +
			  "<dx:BoolToVisibilityConverter x:Key='BoolToVisibilityConverter' />" +
			  "</Grid.Resources>" +
			  "<Grid.RowDefinitions> <RowDefinition/> <RowDefinition/> </Grid.RowDefinitions> " +
			  "<ContentPresenter x:Name='defaultRowPresenter' Content='{Binding}' ContentTemplate='{Binding View.DefaultDataRowTemplate}'/> " +
			  "<Grid Grid.Row='1' Visibility='{Binding Path=DataContext.RowState.(dxg:DXDetailPresenter.IsDetailVisible), RelativeSource={RelativeSource TemplatedParent}, Converter={StaticResource BoolToVisibilityConverter}}'><Grid.ColumnDefinitions><ColumnDefinition Width='23'/><ColumnDefinition Width='Auto'/><ColumnDefinition/></Grid.ColumnDefinitions>" +
			  "<dxg:DXDetailPresenter Grid.Column='2' IsContentVisible='{Binding Path=DataContext.RowState.(dxg:DXDetailPresenter.IsDetailVisible), RelativeSource={RelativeSource TemplatedParent}}' DetailTemplate='{Binding Path=DataContext.View.(dxg:MasterDetailHelper.Detail), RelativeSource={RelativeSource TemplatedParent}}' DetailContent='{Binding DataContext, RelativeSource={RelativeSource TemplatedParent}}'/> " +
			  "<dxg:RowsDelimiter Grid.Column='2' Margin='0,0,0,-1' Height='1' VerticalAlignment='Top'/>" +
			  "<dxg:FixedDelimiter Grid.Column='1' Width='{Binding View.FixedLineWidth}'/>" +
			  "</Grid></Grid> </DataTemplate>";
		public static readonly DependencyProperty DetailProperty;
		static void DetailPropertyChanged(DependencyObject dObject, DependencyPropertyChangedEventArgs e) {
			TableView view = (TableView)dObject;
#if SL
			view.DataRowTemplate = (DataTemplate)XamlReader.Load(DataRowTemplateXAML);
#else
			view.DataRowTemplate = (DataTemplate)XamlReader.Parse(DataRowTemplateXAML);
#endif
		}
		public static DataTemplate GetDetail(DependencyObject obj) {
			return (DataTemplate)obj.GetValue(DetailProperty);
		}
		public static void SetDetail(DependencyObject obj, DataTemplate value) {
			obj.SetValue(DetailProperty, value);
		}
		static MasterDetailHelper() {
			DetailProperty = DependencyPropertyManager.RegisterAttached("Detail", typeof(DataTemplate), typeof(MasterDetailHelper), new PropertyMetadata(null, DetailPropertyChanged));
		}
	}
	public class ExpandPreviewCommand : ICommand {
		public bool CanExecute(object parameter) {
			return true;
		}
		public event EventHandler CanExecuteChanged;
		public void RaiseCanExecuteChanged() {
			if(CanExecuteChanged != null) {
				CanExecuteChanged(this, new EventArgs());
			}
		}
		public void Execute(object parameter) {
			RowData data = parameter as RowData;
			bool value = (bool)data.RowState.GetValue(DXDetailPresenter.IsDetailVisibleProperty);
			data.RowState.SetValue(DXDetailPresenter.IsDetailVisibleProperty, !value);
		}
	}
	public class GridExpandColumn : GridColumn {
		const string DisplayTemplateXAML =
			 @"<DataTemplate " +
				"xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
				"xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' " +
				"xmlns:dxg='http://schemas.devexpress.com/winfx/2008/xaml/grid'> " +
				"<Grid> <Grid.Resources> <dxg:ExpandPreviewCommand x:Key='ExpandPreview'/> </Grid.Resources> " +
				"<dxg:GridDetailExpandButton IsChecked='{Binding Path=DataContext.RowData.RowState.(dxg:DXDetailPresenter.IsDetailVisible), RelativeSource={RelativeSource TemplatedParent}}' " +
				"Command='{StaticResource ExpandPreview}' CommandParameter='{Binding DataContext.RowData, RelativeSource={RelativeSource TemplatedParent}}'> " +
				"</dxg:GridDetailExpandButton> " +
				"</Grid> " +
			"</DataTemplate>";
		public GridExpandColumn() {
			this.AllowAutoFilter = false;
			this.UnboundType = DevExpress.Data.UnboundColumnType.Object;
			this.Fixed = FixedStyle.Left;
			this.Width = 23;
			this.FixedWidth = true;
			this.AllowEditing = DefaultBoolean.False;
			this.AllowMoving = DefaultBoolean.False;
			this.AllowSorting = DefaultBoolean.False;
			this.AllowColumnFiltering = DefaultBoolean.False;
			this.AllowResizing = DefaultBoolean.False;
			this.AllowBestFit = DefaultBoolean.False;
#if SL
			this.CellTemplate = (DataTemplate)XamlReader.Load(DisplayTemplateXAML);
#else
			this.CellTemplate = (DataTemplate)XamlReader.Parse(DisplayTemplateXAML);
#endif
		}
	}
}
