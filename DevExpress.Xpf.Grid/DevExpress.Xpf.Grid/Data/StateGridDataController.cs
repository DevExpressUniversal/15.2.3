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

using DevExpress.Data;
using DevExpress.Xpf.Core.Native;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using DevExpress.Data.Selection;
using DevExpress.Xpf.Grid;
using System.ComponentModel;
using DevExpress.Xpf.Core.Mvvm.UI.ViewGenerator.Metadata;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native.ViewGenerator;
using DevExpress.Data.Helpers;
namespace DevExpress.Xpf.Data {
	public class StateGridDataController : DXGridDataController {
		readonly IDataProviderOwner owner;
		public StateGridDataController(IDataProviderOwner owner) {
			this.owner = owner;
#if SL
			editingValuesKeeper = new ControllerRowValuesKeeper(this);
#endif
		}
		protected override SelectionController CreateSelectionController() { return new RowStateController(this); }
		public override DevExpress.Data.Helpers.ListSourceRowsKeeper CreateControllerRowsKeeper() {
			return new RowStateKeeper(this, CreateSelectionKeeper());
		}
#if !SILVERLIGHT
		protected override Dispatcher Dispatcher { get { return owner.Dispatcher; } }
#endif
#if SL
		ControllerRowValuesKeeper editingValuesKeeper;
#endif
		public override void BeginCurrentRowEdit() {
#if SL
			if(!IsCurrentRowEditing && !IsIEditableObject) editingValuesKeeper.SaveValues();
#endif
			base.BeginCurrentRowEdit();
		}
		public override void CancelCurrentRowEdit() {
#if SL
			if(!IsIEditableObject && IsCurrentRowEditing) editingValuesKeeper.RestoreValues();
#endif
			base.CancelCurrentRowEdit();
			owner.RaiseCurrentRowCanceled(new ControllerRowEventArgs(CurrentControllerRow, GetRow(CurrentControllerRow)));
		}
#if SL
		bool IsIEditableObject { get { return CurrentControllerRowObject is IEditableObject; } }
#endif
		protected override CustomSummaryEventArgs CreateCustomSummaryEventArgs() {
			return new GridCustomSummaryEventArgs(owner as GridControl);
		}
		protected override CustomSummaryExistEventArgs CreateCustomSummaryExistEventArgs(GroupRowInfo groupRow, object item) {
			return new GridCustomSummaryExistEventArgs(owner as GridControl, groupRow, item);
		}
		protected override void OnCurrentControllerRowChanging(int oldControllerRow, int newControllerRow) {
			owner.OnCurrentIndexChanging(newControllerRow);
			base.OnCurrentControllerRowChanging(oldControllerRow, newControllerRow);
		}
		protected override void OnBindingListChanged(ListChangedEventArgs e) {
			bool isItemPropertyChanged = e.ListChangedType == ListChangedType.ItemChanged && e.OldIndex != -1;
			if(isItemPropertyChanged && (owner.AllowLiveDataShaping.HasValue && !owner.AllowLiveDataShaping.Value)) return;
			base.OnBindingListChanged(e);
		}
#if !SL
		protected override TypeConverter GetActualTypeConverter(TypeConverter converter, PropertyDescriptor property) {
			return DataColumnAttributesExtensions.GetActualTypeConverter(property, owner.ItemType, converter);
		}
		protected override void OnItemMoved(ListChangedEventArgs e, DataControllerChangedItemCollection changedItems) {
			base.OnItemMoved(e, changedItems);
			if(e.OldIndex >= 0 && e.NewIndex >= 0 && SortInfo.Count > 0) {
				changedItems.AddItem(GetControllerRow(e.OldIndex), NotifyChangeType.ItemChanged, null);
				changedItems.AddItem(GetControllerRow(e.NewIndex), NotifyChangeType.ItemChanged, null);
			}
		}
#endif
#if DEBUGTEST
		public static int DisposeCallCountForTest { get; set; }
		public override void Dispose() {
			DisposeCallCountForTest++;
			base.Dispose();
		}
		public static int EndUpdateCountForTest { get; set; }
		public override void EndUpdate() {
			EndUpdateCountForTest++;
			base.EndUpdate();
		}
		public static int CalcTotalSummaryItemForTest { get; set; }
		protected override void CalcTotalSummaryItem(SummaryItem summary) {
			CalcTotalSummaryItemForTest++;
			base.CalcTotalSummaryItem(summary);
		}
#endif
	}
}
