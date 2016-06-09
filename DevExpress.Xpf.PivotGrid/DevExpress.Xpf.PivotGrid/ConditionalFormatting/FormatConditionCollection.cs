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
using System.Linq;
using System.Windows;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
using DevExpress.Xpf.GridData;
using DevExpress.Xpf.PivotGrid.Internal;
namespace DevExpress.Xpf.PivotGrid {
	public class FormatConditionCollection : ObservableCollectionCore<FormatConditionBase> {
		ConditionalFormatSummaryInfo[] summaries;
		internal ConditionalFormatSummaryInfo[] Summaries { get { return summaries ?? (summaries = CollectSummaries()); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public IFormatConditionCollectionOwner Owner { get; private set; }
		public FormatConditionCollection(IFormatConditionCollectionOwner owner) {
			this.Owner = owner;
		}
		Dictionary<string, IList<FormatConditionBase>> cache = new Dictionary<string, IList<FormatConditionBase>>();
		internal IList<FormatConditionBase> this[string fieldName] {
			get {
				if (Count == 0)
					return null;
				return cache.GetOrAdd(fieldName, () => GetItemsByFieldName(fieldName));
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IList<FormatConditionBaseInfo> GetInfoByFieldName(string fieldName) {
			IList<FormatConditionBase> conditions = this[fieldName];
			return conditions == null ? new List<FormatConditionBaseInfo>() : conditions.Select(x => x.Info).ToList();
		}
		internal IList<FormatConditionBaseInfo> GetInfoByFieldName(string fieldName, CellsAreaItem ValueItem) {
			if(fieldName == string.Empty)
				fieldName = null;
			string rowName = ValueItem.Item.RowField == null ? null : ValueItem.Item.RowField.Name;
			string columnName = ValueItem.Item.ColumnField == null ? null : ValueItem.Item.ColumnField.Name;
			var list = this.Where(x => x.GetApplyToFieldName() == fieldName && x.IsValid && (!x.ApplyToSpecificLevel || x.RowName == rowName && x.ColumnName == columnName)).ToArray();
			return list.Length > 0 ? list.Select(x => x.Info).ToList() : null;
		}
		IList<FormatConditionBase> GetItemsByFieldName(string fieldName) {
			if (fieldName == string.Empty)
				fieldName = null;
			var list = this.Where(x => x.GetApplyToFieldName() == fieldName && x.IsValid).ToArray();
			return list.Length > 0 ? list : null;
		}
		protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			base.OnCollectionChanged(e);
			if (IsLockUpdate)
				return;
			OnChanged(FormatConditionChangeType.All);
			Owner.SyncFormatCondtitionCollectionWithDetails(e);
		}
		void OnChanged(FormatConditionChangeType changeType) {
			cache.Clear();
			if (changeType.HasFlag(FormatConditionChangeType.Summary))
				summaries = null;
			Owner.OnFormatConditionCollectionChanged(changeType);
		}
		internal void OnItemPropertyChanged(FormatConditionBase item, DependencyPropertyChangedEventArgs e, FormatConditionChangeType changeType) {
			if (IsLockUpdate)
				return;
			OnChanged(changeType);
			Owner.SyncFormatCondtitionPropertyWithDetails(item, e);
		}
		protected override void InsertItem(int index, FormatConditionBase item) {
			base.InsertItem(index, item);
			SetOwner(item);
		}
		protected override void RemoveItem(int index) {
			NullOwner(this[index]);
			base.RemoveItem(index);
		}
		protected override void ClearItems() {
			foreach (var item in this) {
				NullOwner(item);
			}
			base.ClearItems();
		}
		protected override void SetItem(int index, FormatConditionBase item) {
			NullOwner(this[index]);
			base.SetItem(index, item);
			SetOwner(item);
		}
		void SetOwner(FormatConditionBase item) {
			if (item.Owner != null && item.Owner != this)
				throw new InvalidOperationException(typeof(FormatCondition).Name + " object cannot be added to more than one " + typeof(FormatConditionCollection).Name + ".");
			item.Owner = this;
		}
		void NullOwner(FormatConditionBase item) {
			item.Owner = null;
		}
		ConditionalFormatSummaryInfo[] CollectSummaries() {
			return this.Where(x => x.IsValid).SelectMany(x => x.CreateSummaryItems()).Distinct().ToArray();
		}
		internal IEnumerable<IColumnInfo> GetUnboundColumns() {
			return this.Where(x => x.IsValid).SelectMany(x => x.GetUnboundColumnInfo());
		}
	}
}
