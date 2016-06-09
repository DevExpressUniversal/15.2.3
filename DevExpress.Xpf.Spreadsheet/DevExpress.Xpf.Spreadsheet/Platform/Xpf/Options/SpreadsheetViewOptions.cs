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
using DevExpress.Utils;
using System.Windows.Data;
using InnerSpreadsheetViewOptions = DevExpress.XtraSpreadsheet.SpreadsheetViewOptions;
namespace DevExpress.Xpf.Spreadsheet {
	#region SpreadsheetViewOptions
	public class SpreadsheetViewOptions : DependencyObject {
		#region Fields
		public static readonly DependencyProperty ShowColumnHeadersProperty;
		public static readonly DependencyProperty ShowRowHeadersProperty;
		public static readonly DependencyProperty ColumnHeaderHeightProperty;
		public static readonly DependencyProperty RowHeaderWidthProperty;
		InnerSpreadsheetViewOptions source;
		#endregion
		static SpreadsheetViewOptions() {
			Type ownerType = typeof(SpreadsheetViewOptions);
			ShowColumnHeadersProperty = DependencyProperty.Register("ShowColumnHeaders", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true));
			ShowRowHeadersProperty = DependencyProperty.Register("ShowRowHeaders", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true));
			ColumnHeaderHeightProperty = DependencyProperty.Register("ColumnHeaderHeight", typeof(int), ownerType,
				new FrameworkPropertyMetadata(0));
			RowHeaderWidthProperty = DependencyProperty.Register("RowHeaderWidth", typeof(int), ownerType,
				new FrameworkPropertyMetadata(0));
		}
		#region Properties
		public bool ShowColumnHeaders {
			get { return (bool)GetValue(ShowColumnHeadersProperty); }
			set { SetValue(ShowColumnHeadersProperty, value); }
		}
		public bool ShowRowHeaders {
			get { return (bool)GetValue(ShowRowHeadersProperty); }
			set { SetValue(ShowRowHeadersProperty, value); }
		}
		public int ColumnHeaderHeight {
			get { return (int)GetValue(ColumnHeaderHeightProperty); }
			set { SetValue(ColumnHeaderHeightProperty, value); }
		}
		public int RowHeaderWidth {
			get { return (int)GetValue(RowHeaderWidthProperty); }
			set { SetValue(RowHeaderWidthProperty, value); }
		}
		#endregion
		internal void SetSource(InnerSpreadsheetViewOptions source) {
			Guard.ArgumentNotNull(source, "source");
			this.source = source;
			UpdateSourceProperties();
			BindProperties();
		}
		void UpdateSourceProperties() {
			if (ShowColumnHeaders != (bool)GetDefaultValue(ShowColumnHeadersProperty))
				source.ShowColumnHeaders = ShowColumnHeaders;
			if (ShowRowHeaders != (bool)GetDefaultValue(ShowRowHeadersProperty))
				source.ShowRowHeaders = ShowRowHeaders;
			if (ColumnHeaderHeight != (int)GetDefaultValue(ColumnHeaderHeightProperty))
				source.ColumnHeaderHeight = ColumnHeaderHeight;
			if (RowHeaderWidth != (int)GetDefaultValue(RowHeaderWidthProperty))
				source.RowHeaderWidth = RowHeaderWidth;
		}
		object GetDefaultValue(DependencyProperty property) {
			return property.DefaultMetadata.DefaultValue;
		}
		void BindProperties() {
			BindShowColumnHeadersProperty();
			BindShowRowHeadersProperty();
			BindColumnHeaderHeightProperty();
			BindRowHeaderWidthProperty();
		}
		void BindShowColumnHeadersProperty() {
			Binding bind = new Binding("ShowColumnHeaders") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, ShowColumnHeadersProperty, bind);
		}
		void BindShowRowHeadersProperty() {
			Binding bind = new Binding("ShowRowHeaders") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, ShowRowHeadersProperty, bind);
		}
		void BindColumnHeaderHeightProperty() {
			Binding bind = new Binding("ColumnHeaderHeight") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, ColumnHeaderHeightProperty, bind);
		}
		void BindRowHeaderWidthProperty() {
			Binding bind = new Binding("RowHeaderWidth") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, RowHeaderWidthProperty, bind);
		}
		public void Reset() {
			if (source != null)
				source.Reset();
		}
	}
	#endregion
}
