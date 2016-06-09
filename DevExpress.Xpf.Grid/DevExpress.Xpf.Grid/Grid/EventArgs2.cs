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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using DevExpress.Core;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Data;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Data.Helpers;
using System.Collections;
#if SILVERLIGHT
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
#endif
namespace DevExpress.Xpf.Grid {	
	public class CustomColumnDisplayTextEventArgs : EventArgs {
		GridViewBase view;
		DataControllerLazyValuesContainer values;
		public GridControl Source { get { return view.Grid; } }
		public int RowHandle { get { return values.RowHandle; } }
		public int ListSourceIndex { get { return values.ListSourceIndex; } }
		public object Row { get { return Source.GetRow(RowHandle); } }
		public GridColumn Column { get; private set; }
		public object Value { get; private set; }
		public string DisplayText { get; set; }
		public bool ShowAsNullText { get; set; }
		internal void SetArgs(GridViewBase view, int? rowHandle, int? listSourceIndex, GridColumn column, object value, string displayText) {
			this.view = view;
			this.values = new DataControllerLazyValuesContainer(view, rowHandle, listSourceIndex);
			this.Column = column;
			this.Value = value;
			this.DisplayText = displayText;
			this.ShowAsNullText = false;
		}
	}
	public delegate void CustomColumnDisplayTextEventHandler(object sender, CustomColumnDisplayTextEventArgs e);
	public class CellMergeEventArgs : EventArgs {
		public GridColumn Column { get; private set; }
		public int RowHandle1 { get; private set; }
		public int RowHandle2 { get; private set; }
		public object CellValue1 { get; private set; }
		public object CellValue2 { get; private set; }
		public bool Handled { get; set; }
		public bool Merge { get; set; }
		internal void SetArgs(GridColumn column, int rowHandle1, int rowHandle2, object cellValue1, object cellValue2) {
			Column = column;
			RowHandle1 = rowHandle1;
			RowHandle2 = rowHandle2;
			CellValue1 = cellValue1;
			CellValue2 = cellValue2;
			Handled = false;
			Merge = false;
		}
	}
	public delegate void CellMergeEventHandler(object sender, CellMergeEventArgs e);
}
