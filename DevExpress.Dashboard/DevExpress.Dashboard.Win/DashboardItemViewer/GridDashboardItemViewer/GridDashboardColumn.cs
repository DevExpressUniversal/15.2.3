#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Collections.Generic;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DataAccess.Native;
using DevExpress.Utils;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
namespace DevExpress.DashboardWin.Native {
	public class GridDashboardColumn : GridColumn, IGridColumn {
		public const int DefaultMinWidth = 10;
		public bool AllowGroup {
			get { return OptionsColumn.AllowGroup.ToBoolean(false); }
			set { OptionsColumn.AllowGroup = value ? DefaultBoolean.True : DefaultBoolean.False; }
		}
		public bool AllowMove {
			get { return OptionsColumn.AllowMove; }
			set { OptionsColumn.AllowMove = value; }
		}
		public bool AllowCellMerge {
			get { return OptionsColumn.AllowMerge == DefaultBoolean.True; }
			set { OptionsColumn.AllowMerge = value ? DefaultBoolean.True : DefaultBoolean.False; }
		}
		public new GridColumnFilterMode FilterMode {
			get { return base.FilterMode == ColumnFilterMode.DisplayText ? GridColumnFilterMode.DisplayText : GridColumnFilterMode.Value; }
			set { base.FilterMode = value == GridColumnFilterMode.DisplayText ? ColumnFilterMode.DisplayText : ColumnFilterMode.Value; }
		}
		public bool TextIsHidden { get; set; }
		int IGridColumn.ActualIndex { get; set; }
		double IGridColumn.Weight { get; set; }
		DevExpress.DashboardCommon.GridColumnFixedWidthType IGridColumn.WidthType { get; set; }
		double IGridColumn.FixedWidth { get; set; }
		GridColumnDisplayMode IGridColumn.DisplayMode { get; set; }
		double IGridColumn.DefaultBestCharacterCount { get; set; }
		bool IGridColumn.IgnoreDeltaIndication { get; set; }
		int IGridColumn.MaxIconStyleImageWidth { get; set; }
	}
	public class CustomDashboardColumnDisplayTextEventArgs : CustomColumnDisplayTextEventArgsBase {
		CustomColumnDisplayTextEventArgs eventArgs;
		public override string ColumnFieldName {
			get { return eventArgs.Column.FieldName; }
		}
		public override int Index {
			get { return eventArgs.ListSourceRowIndex; }
		}
		public CustomDashboardColumnDisplayTextEventArgs(CustomColumnDisplayTextEventArgs eventArgs) {
			this.eventArgs = eventArgs;
		}
		public override void SetDisplayText(string displayText) {
			eventArgs.DisplayText = displayText;
		}
	}
	public class GridDashboardColumnCollection : GridColumnCollection, IEnumerable<GridDashboardColumn> {
		readonly Locker locker = new Locker();
		public GridDashboardColumnCollection(GridDashboardView view)
			: base(view) {
		}
		public new IEnumerator<GridDashboardColumn> GetEnumerator() {
			return new GridDashboardColumnEnumerator(this);
		}
		bool IsDesignMode { get { return locker.IsLocked; } }
		protected override void OnInsert(int index, object value) {
			CheckDesignMode();
			base.OnInsert(index, value);
		}
		void CheckDesignMode() {
			if(!IsDesignMode)
				throw new InvalidOperationException(DashboardLocalizer.GetString(DashboardStringId.MessageIncorrectGridDashboardControlOperation));
		}
		internal void SetDesignModeOn() {
			locker.Lock();
		}
		internal void SetDesignModeOff() {
			locker.Unlock();
		}
		protected override void OnClear() {
			CheckDesignMode();
			base.OnClear();
		}
	}
	public class GridDashboardColumnEnumerator : IEnumerator<GridDashboardColumn> {
		GridDashboardColumnCollection columns;
		int currentIndex;
		GridDashboardColumn currentColumn;
		public GridDashboardColumnEnumerator(GridDashboardColumnCollection columns) {
			this.columns = columns;
			currentIndex = -1;
			currentColumn = default(GridDashboardColumn);
		}
		public bool MoveNext() {
			if(++currentIndex >= columns.Count)
				return false;
			else
				currentColumn = (GridDashboardColumn)columns[currentIndex];
			return true;
		}
		public void Reset() { currentIndex = -1; }
		void IDisposable.Dispose() { }
		public GridDashboardColumn Current {
			get { return currentColumn; }
		}
		object IEnumerator.Current {
			get { return Current; }
		}
	}
}
