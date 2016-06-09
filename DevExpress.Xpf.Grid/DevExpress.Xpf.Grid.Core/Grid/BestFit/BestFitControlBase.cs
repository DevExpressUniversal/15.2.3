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
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
using System.Windows.Controls;
#if SILVERLIGHT
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
#endif
namespace DevExpress.Xpf.Grid {
	public abstract class BestFitControlBase : ContentControl {
		static readonly DependencyPropertyKey ColumnPropertyKey;
		public static readonly DependencyProperty ColumnProperty;
		static BestFitControlBase() {
			Type ownerType = typeof(BestFitControlBase);
			ColumnPropertyKey = DependencyPropertyManager.RegisterReadOnly("Column", typeof(ColumnBase), ownerType, new PropertyMetadata(null));
			ColumnProperty = ColumnPropertyKey.DependencyProperty;
		}
		public RowData RowData { get; private set; }
		protected GridColumnData CellData { get; private set; }
		protected BestFitControlBase(DataViewBase view, ColumnBase column) {
			Column = column;
			RowData = new StandaloneRowData(view.VisualDataTreeBuilder);
			RowData.SetRowDataInternal(this, RowData);
			DataContext = RowData;
			CellData = RowData.GetCellDataByColumn(Column, false);
		}
		public ColumnBase Column {
			get { return (ColumnBase)GetValue(ColumnProperty); }
			private set { this.SetValue(ColumnPropertyKey, value); }
		}
		public void Update(int rowHandle) {
			RowData.AssignFrom(rowHandle);
		}
		public void UpdateValue(object value) {
			CellData.Value = value;
		}
		public virtual void UpdateIsFocusedCell(bool isFocusedCell) { }
		protected override Size ArrangeOverride(Size arrangeBounds) {
			return new Size(0, 0);
		}
	}
	public abstract class BaseBestFitControl : BestFitControlBase {
		FrameworkElement cellContentPresenter;
		static readonly DependencyPropertyKey IsFocusedCellPropertyKey;
		public static readonly DependencyProperty IsFocusedCellProperty;
		static BaseBestFitControl() {
			Type ownerType = typeof(BaseBestFitControl);
			IsFocusedCellPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsFocusedCell", typeof(bool), ownerType, new PropertyMetadata(false));
			IsFocusedCellProperty = IsFocusedCellPropertyKey.DependencyProperty;
		}
		public bool IsFocusedCell {
			get { return (bool)GetValue(IsFocusedCellProperty); }
			private set { this.SetValue(IsFocusedCellPropertyKey, value); }
		}
		protected BaseBestFitControl(DataViewBase view, ColumnBase column) : base(view, column) { }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			cellContentPresenter = GetTemplateChild("PART_CellContentPresenter") as FrameworkElement;
			cellContentPresenter.DataContext = CellData;
			cellContentPresenter.Style = Column.ActualCellStyle;
		}
		public override void UpdateIsFocusedCell(bool isFocusedCell) {
			IsFocusedCell = isFocusedCell;
		}
	}
}
