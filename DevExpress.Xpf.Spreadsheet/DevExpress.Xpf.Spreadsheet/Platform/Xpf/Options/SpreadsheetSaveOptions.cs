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
using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet;
namespace DevExpress.Xpf.Spreadsheet {
	#region SpreadsheetSaveOptions
	public class SpreadsheetSaveOptions : DependencyObject {
		#region Fields
		public static readonly DependencyProperty DefaultFileNameProperty;
		public static readonly DependencyProperty CurrentFileNameProperty;
		public static readonly DependencyProperty DefaultFormatProperty;
		public static readonly DependencyProperty CurrentFormatProperty;
		WorkbookSaveOptions source;
		#endregion
		static SpreadsheetSaveOptions() {
			Type ownerType = typeof(SpreadsheetSaveOptions);
			DefaultFileNameProperty = DependencyProperty.Register("DefaultFileName", typeof(string), ownerType,
				new FrameworkPropertyMetadata(String.Empty));
			CurrentFileNameProperty = DependencyProperty.Register("CurrentFileName", typeof(string), ownerType,
				new FrameworkPropertyMetadata(String.Empty));
			DefaultFormatProperty = DependencyProperty.Register("DefaultFormat", typeof(DocumentFormat), ownerType,
				new FrameworkPropertyMetadata(DocumentFormat.OpenXml));
			CurrentFormatProperty = DependencyProperty.Register("CurrentFormat", typeof(DocumentFormat), ownerType,
				new FrameworkPropertyMetadata(DocumentFormat.Undefined));
		}
		#region Properties
		public string DefaultFileName {
			get { return (string)GetValue(DefaultFileNameProperty); }
			set { SetValue(DefaultFileNameProperty, value); }
		}
		public string CurrentFileName {
			get { return (string)GetValue(CurrentFileNameProperty); }
			set { SetValue(CurrentFileNameProperty, value); }
		}
		public DocumentFormat DefaultFormat {
			get { return (DocumentFormat)GetValue(DefaultFormatProperty); }
			set { SetValue(DefaultFormatProperty, value); }
		}
		public DocumentFormat CurrentFormat {
			get { return (DocumentFormat)GetValue(CurrentFormatProperty); }
			set { SetValue(CurrentFormatProperty, value); }
		}
		#endregion
		internal void SetSource(WorkbookSaveOptions source) {
			Guard.ArgumentNotNull(source, "source");
			this.source = source;
			UpdateSourceProperties();
			BindProperties();
		}
		void UpdateSourceProperties() {
			if (DefaultFileName != (string)GetDefaultValue(DefaultFileNameProperty))
				source.DefaultFileName = DefaultFileName;
			if (CurrentFileName != (string)GetDefaultValue(CurrentFileNameProperty))
				source.CurrentFileName = CurrentFileName;
			if (DefaultFormat != (DocumentFormat)GetDefaultValue(DefaultFormatProperty))
				source.DefaultFormat = DefaultFormat;
			if (CurrentFormat != (DocumentFormat)GetDefaultValue(CurrentFormatProperty))
				source.CurrentFormat = CurrentFormat;
		}
		object GetDefaultValue(DependencyProperty property) {
			return property.DefaultMetadata.DefaultValue;
		}
		void BindProperties() {
			BindDefaultFileNameProperty();
			BindCurrentFileNameProperty();
			BindDefaultFormatProperty();
			BindCurrentFormatProperty();
		}
		void BindDefaultFileNameProperty() {
			Binding bind = new Binding("DefaultFileName") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, DefaultFileNameProperty, bind);
		}
		void BindCurrentFileNameProperty() {
			Binding bind = new Binding("CurrentFileName") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, CurrentFileNameProperty, bind);
		}
		void BindDefaultFormatProperty() {
			Binding bind = new Binding("DefaultFormat") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, DefaultFormatProperty, bind);
		}
		void BindCurrentFormatProperty() {
			Binding bind = new Binding("CurrentFormat") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, CurrentFormatProperty, bind);
		}
		public void Reset() {
			if (source != null)
				source.Reset();
		}
	}
	#endregion
}
