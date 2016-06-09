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
using System.Windows.Controls;
using DevExpress.XtraSpreadsheet.Forms;
using System.Windows;
using System.Windows.Input;
namespace DevExpress.Xpf.Spreadsheet.Forms {
	public partial class DocumentPropertiesControl : UserControl {
		readonly DocumentPropertiesViewModel viewModel;
		public DocumentPropertiesControl(DocumentPropertiesViewModel viewModel) {
			InitializeComponent();
			this.viewModel = viewModel;
			DataContext = viewModel;
		}
		void btnAdd_Click(object sender, RoutedEventArgs e) {
			if (viewModel != null)
				viewModel.AddProperty();
			edtName.Focus();
		}
		void btnModify_Click(object sender, RoutedEventArgs e) {
			if (viewModel != null)
				viewModel.ModifyProperty();
			edtName.Focus();
		}
		void btnDelete_Click(object sender, RoutedEventArgs e) {
			if (viewModel != null)
				viewModel.DeleteProperty();
			edtName.Focus();
		}
		void lstPredefinedProperties_SelectedIndexChanged(object sender, RoutedEventArgs e) {
			if (viewModel != null)
				viewModel.ApplyPredefinedProperty(lstPredefinedProperties.SelectedIndex);
			edtName.SelectAll();
			edtName.Focus();
		}
		void grid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			if (viewModel != null)
				viewModel.PropertyIndex = grid.SelectedIndex;
		}
		void ListViewItem_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			ListViewItem item = sender as ListViewItem;
			if (item != null) {
				if (viewModel != null)
					viewModel.PropertyIndex = grid.SelectedIndex;
			}
		}
	}
}
