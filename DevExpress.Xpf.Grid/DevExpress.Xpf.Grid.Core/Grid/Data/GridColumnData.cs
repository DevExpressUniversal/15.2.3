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

using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
using System.ComponentModel;
using DevExpress.XtraEditors.DXErrorProvider;
using System.Windows.Data;
using System;
#if SILVERLIGHT
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
#endif
namespace DevExpress.Xpf.Grid {
	public class GridColumnData : GridDataBase, ISupportVisibleIndex {
		protected ColumnsRowDataBase RowDataBase { get; private set; }
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("GridColumnDataView")]
#endif
public DataViewBase View { get { return RowDataBase.View; } }
		internal ColumnBase ColumnCore { get; private set; }
		ColumnBase column;
		public ColumnBase Column {
			get { return column; }
			internal set {
				if(column != value) {
					column = value;
					OnColumnChanged(column);
					RaisePropertyChanged("Column");
				}
			}
		}
		public GridColumnData(ColumnsRowDataBase rowData) {
			this.RowDataBase = rowData;
		}
		protected virtual void OnColumnChanged(ColumnBase newValue) {
			ColumnCore = newValue;
			if(Editor == null || ColumnCore == Editor.Column)
				UpdateValue();
			else
				updateDataOnEditorContentUpdated = true;
		}
		bool updateDataOnEditorContentUpdated;
		protected bool IsValueDirty { get { return updateDataOnEditorContentUpdated; } }
		internal void OnEditorContentUpdated() {
			if(!IsValueDirty) return;
			updateDataOnEditorContentUpdated = false;
			OnDataChanged();
		}
		protected override void OnDataChanged() {
			if(Editor == null || (View.ShouldUpdateCellData && Editor.Column == Column))
				base.OnDataChanged();
			else
				updateDataOnEditorContentUpdated = true;
		}
		protected internal override void UpdateValue() {
			base.UpdateValue();
			RaiseContentChanged();
		}
		protected override void OnContentChanged() {
			base.OnContentChanged();
			if(editor != null)
				editor.UpdateEditableValue();
		}
		#region ISupportVisibleIndex Members
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("GridColumnDataVisibleIndex")]
#endif
		public int VisibleIndex { get; internal set; }
		#endregion
		internal void UpdateCellBackgroundAppearance() {
			if(editor != null)
				editor.GridCellEditorOwner.UpdateCellBackgroundAppearance();
		}
		internal void UpdateCellForegroundAppearance() {
			if(editor != null)
				editor.GridCellEditorOwner.UpdateCellForegroundAppearance();
		}
	}
}
