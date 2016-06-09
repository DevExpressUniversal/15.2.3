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

using DevExpress.DataAccess.Excel;
using DevExpress.Xpf.Core.DataSources;
using DevExpress.Xpf.Core.Native;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Resources;
namespace DevExpress.Xpf.Core.DataSources {
	public class ExcelItemsSource : DataSourceWithDataBase {
		public static readonly DependencyProperty FileNameProperty;
		public static readonly DependencyProperty ColumnsProperty;
		public static readonly DependencyProperty UseFirstRowAsHeaderProperty;
		public static readonly DependencyProperty SkipEmptyRowsProperty;
		public static readonly DependencyProperty SkipHiddenRowsProperty;
		public static readonly DependencyProperty SkipHiddenColumnsProperty;
		public static readonly DependencyProperty PasswordProperty;
		public static readonly DependencyProperty WorksheetNameProperty;
		public static readonly DependencyProperty CellRangeProperty;
		public static readonly DependencyProperty StreamProperty;
		public static readonly DependencyProperty StreamDocumentFormatProperty;
		public static readonly DependencyProperty FileUriProperty;
		static ExcelItemsSource() {
			Type ownerclass = typeof(ExcelItemsSource);
			FileNameProperty = DependencyProperty.Register("FileName", typeof(string), ownerclass, new PropertyMetadata(String.Empty, (d, e) => ((ExcelItemsSource)d).OnSettingsChanged()));
			ColumnsProperty = DependencyProperty.Register("Columns", typeof(ExcelSourceColumnsCollection), ownerclass, new PropertyMetadata(null, (d, e) => ((ExcelItemsSource)d).OnColumnsCollectionChanged((ExcelSourceColumnsCollection)e.OldValue, (ExcelSourceColumnsCollection)e.NewValue)));
			UseFirstRowAsHeaderProperty = DependencyProperty.Register("UseFirstRowAsHeader", typeof(bool), ownerclass, new PropertyMetadata(true, (d, e) => ((ExcelItemsSource)d).OnSettingsChanged()));
			SkipEmptyRowsProperty = DependencyProperty.Register("SkipEmptyRows", typeof(bool), ownerclass, new PropertyMetadata(true, (d, e) => ((ExcelItemsSource)d).OnSettingsChanged()));
			SkipHiddenRowsProperty = DependencyProperty.Register("SkipHiddenRows", typeof(bool), ownerclass, new PropertyMetadata(true, (d, e) => ((ExcelItemsSource)d).OnSettingsChanged()));
			SkipHiddenColumnsProperty = DependencyProperty.Register("SkipHiddenColumns", typeof(bool), ownerclass, new PropertyMetadata(true, (d, e) => ((ExcelItemsSource)d).OnSettingsChanged()));
			PasswordProperty = DependencyProperty.Register("Password", typeof(string), ownerclass, new PropertyMetadata(String.Empty, (d, e) => ((ExcelItemsSource)d).OnSettingsChanged()));
			WorksheetNameProperty = DependencyProperty.Register("WorksheetName", typeof(string), ownerclass, new PropertyMetadata(null, (d, e) => ((ExcelItemsSource)d).OnSettingsChanged()));
			CellRangeProperty = DependencyProperty.Register("CellRange", typeof(string), ownerclass, new PropertyMetadata(null, (d, e) => ((ExcelItemsSource)d).OnSettingsChanged()));
			StreamProperty = DependencyProperty.Register("Stream", typeof(Stream), ownerclass, new PropertyMetadata(null, (d, e) => ((ExcelItemsSource)d).OnSettingsChanged()));
			StreamDocumentFormatProperty = DependencyProperty.Register("StreamDocumentFormat", typeof(ExcelDocumentFormat), ownerclass, new PropertyMetadata(ExcelDocumentFormat.Xls, (d, e) => ((ExcelItemsSource)d).OnSettingsChanged()));
			FileUriProperty = DependencyProperty.Register("FileUri", typeof(Uri), ownerclass, new PropertyMetadata(null, (d, e) => ((ExcelItemsSource)d).OnSettingsChanged()));
		}
		void OnColumnsCollectionChanged(ExcelSourceColumnsCollection oldValue, ExcelSourceColumnsCollection newValue) {
			if(oldValue != null)
				oldValue.CollectionChanged -= Columns_CollectionChanged;
			if(newValue != null)
				newValue.CollectionChanged += Columns_CollectionChanged;
		}
		public ExcelItemsSource() {
			Columns = new ExcelSourceColumnsCollection();
		}
		void Columns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			OnSettingsChanged();
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.ExcelItemsSource,FileName")]
		public string FileName {
			get { return (string)GetValue(FileNameProperty); }
			set { SetValue(FileNameProperty, value); }
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.ExcelItemsSource,Columns")]
		public ExcelSourceColumnsCollection Columns {
			get { return (ExcelSourceColumnsCollection)GetValue(ColumnsProperty); }
			set { SetValue(ColumnsProperty, value); }
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.ExcelItemsSource,UseFirstRowAsHeader")]
		public bool UseFirstRowAsHeader {
			get { return (bool)GetValue(UseFirstRowAsHeaderProperty); }
			set { SetValue(UseFirstRowAsHeaderProperty, value); }
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.ExcelItemsSource,SkipEmptyRows")]
		public bool SkipEmptyRows {
			get { return (bool)GetValue(SkipEmptyRowsProperty); }
			set { SetValue(SkipEmptyRowsProperty, value); }
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.ExcelItemsSource,SkipHiddenRows")]
		public bool SkipHiddenRows {
			get { return (bool)GetValue(SkipHiddenRowsProperty); }
			set { SetValue(SkipHiddenRowsProperty, value); }
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.ExcelItemsSource,SkipHiddenColumns")]
		public bool SkipHiddenColumns {
			get { return (bool)GetValue(SkipHiddenColumnsProperty); }
			set { SetValue(SkipHiddenColumnsProperty, value); }
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.ExcelItemsSource,Password")]
		public string Password {
			get { return (string)GetValue(PasswordProperty); }
			set { SetValue(PasswordProperty, value); }
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.ExcelItemsSource,WorksheetName")]
		public string WorksheetName {
			get { return (string)GetValue(WorksheetNameProperty); }
			set { SetValue(WorksheetNameProperty, value); }
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.ExcelItemsSource,CellRange")]
		public string CellRange {
			get { return (string)GetValue(CellRangeProperty); }
			set { SetValue(CellRangeProperty, value); }
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.ExcelItemsSource,Stream")]
		public Stream Stream {
			get { return (Stream)GetValue(StreamProperty); }
			set { SetValue(StreamProperty, value); }
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.ExcelItemsSource,StreamDocumentFormat")]
		public ExcelDocumentFormat StreamDocumentFormat {
			get { return (ExcelDocumentFormat)GetValue(StreamDocumentFormatProperty); }
			set { SetValue(StreamDocumentFormatProperty, value); }
		}
		[Category(DataCategory), DXDescription("DevExpress.Xpf.Core.DataSources.ExcelItemsSource,FileUri")]
		public Uri FileUri {
			get { return (Uri)GetValue(FileUriProperty); }
			set { SetValue(FileUriProperty, value); }
		}
		protected override object CreateDesignTimeDataSourceCore() {
			if(String.IsNullOrWhiteSpace(FileName) || Columns == null || (Columns.Count == 0))
				return null;
			return new DesignTimeDataSource(Columns.Select(column => new DesignTimePropertyInfo(column.Name, column.ColumnType, true)), DesignData.RowCount, DesignData.UseDistinctValues);
		}
		public override void RefreshDataSource() {
			if(Data != null) {
				IDisposable oldSource = Data as IDisposable;
				if(oldSource != null)
					oldSource.Dispose();
				Data = null;
			}
			if(DesignerProperties.GetIsInDesignMode(this)) {
				UpdateData();
				return;
			}
			Data = CreateDataSource();
		}
		object CreateDataSource() {
			ExcelDataSource newSource = new ExcelDataSource();
			newSource.StreamDocumentFormat = StreamDocumentFormat;
			if(Stream == null) {
				if(FileUri == null) newSource.FileName = FileName;
				else {
					StreamResourceInfo sri = Application.GetResourceStream(FileUri);
					if(sri != null) {
						newSource.Stream = sri.Stream;
					}
				}
			}
			else {
				newSource.Stream = Stream;
			}
			ExcelSourceOptions sourceOptions = new ExcelSourceOptions();
			sourceOptions.UseFirstRowAsHeader = UseFirstRowAsHeader;
			sourceOptions.SkipEmptyRows = SkipEmptyRows;
			sourceOptions.SkipHiddenRows = SkipHiddenRows;
			sourceOptions.SkipHiddenColumns = SkipHiddenColumns;
			sourceOptions.Password = Password;
			ExcelWorksheetSettings settings = new ExcelWorksheetSettings();
			settings.WorksheetName = WorksheetName;
			settings.CellRange = CellRange;
			newSource.Schema.AddRange(Columns.ToFieldInfoArray());
			sourceOptions.ImportSettings = settings;
			newSource.SourceOptions = sourceOptions;
			newSource.Fill();
			return newSource;
		}
	}
	public class ExcelSourceColumnsCollection : ObservableCollection<ExcelColumn> {
		protected override void InsertItem(int index, ExcelColumn item) {
			base.InsertItem(index, item);
			item.PropertyChanged += ItemPropertyChanged;
		}
		protected override void RemoveItem(int index) {
			this[index].PropertyChanged -= ItemPropertyChanged;
			base.RemoveItem(index);
		}
		void ItemPropertyChanged(object sender, PropertyChangedEventArgs e) {
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
		public FieldInfo[] ToFieldInfoArray() {
			return this.Select(excelColumn => excelColumn.ToFieldInfo()).ToArray<FieldInfo>();
		}
	}
	public class ExcelColumn : INotifyPropertyChanged {
		string name = String.Empty;
		bool isSelected = true;
		Type columnType;
		public string Name {
			get { return name; }
			set {
				if(name == value)
					return;
				name = value;
				OnPropertyChanged("Name");
			}
		}
		public bool IsSelected {
			get { return isSelected; }
			set {
				if(isSelected == value)
					return;
				isSelected = value;
				OnPropertyChanged("IsSelected");
			}
		}
		public Type ColumnType {
			get { return columnType; }
			set {
				if(columnType == value)
					return;
				columnType = value;
				OnPropertyChanged("ColumnType");
			}
		}
		public FieldInfo ToFieldInfo() {
			return new FieldInfo() { Name = this.Name, Selected = this.isSelected, Type = this.ColumnType };
		}
		void OnPropertyChanged(string name) {
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(name));
		}
		public event PropertyChangedEventHandler PropertyChanged;
	}
}
