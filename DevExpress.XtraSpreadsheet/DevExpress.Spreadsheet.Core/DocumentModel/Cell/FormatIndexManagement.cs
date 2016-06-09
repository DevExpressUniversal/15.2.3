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
using System.Diagnostics;
using DevExpress.Office;
using DevExpress.Office.History;
namespace DevExpress.XtraSpreadsheet.Model {
	#region Cell : IIndexBasedObject<DocumentModelChangeActions>
	partial class Cell {
		public short FormatIndex {
			get {
				return (short)(packedFormat & formatIndexMask);
			}
			set {
				packedFormat &= ~formatIndexMask;
				packedFormat |= (uint)value & formatIndexMask;
			}
		}
		int OriginalFormatIndex { get { return FormatIndex < 0 ? BatchUpdateHelper.OriginalFormatIndex : FormatIndex; } }
		public CellFormat FormatInfo { get { return BatchUpdateHelper != null ? (CellFormat)BatchUpdateHelper.DeferredNotifications : FormatInfoCore; } }
		CellFormat FormatInfoCore { get { return (CellFormat)DocumentModel.Cache.CellFormatCache[FormatIndex]; } }
		public void SetCellFormatIndex(int value) {
			this.FormatIndex = (short)value;
			sheet.WebRanges.AddChangedCellPosition(this);
		}
		#region Format index management
		protected internal UniqueItemsCache<FormatBase> GetCache(IDocumentModel documentModel) {
			return Workbook.Cache.CellFormatCache;
		}
		protected internal virtual int GetInfoIndex(FormatBase value) {
			return GetCache(DocumentModel).GetItemIndex(value);
		}
		protected internal delegate DocumentModelChangeActions SetPropertyValueDelegate<U>(FormatBase info, U newValue);
		protected internal virtual void SetPropertyValue<U>(SetPropertyValueDelegate<U> setter, U newValue) {
			DocumentModel.BeginUpdate();
			try {
				FormatBase info = GetInfoForModification();
				DocumentModelChangeActions changeActions = setter(info, newValue);
				ReplaceInfo(info, changeActions);
			} finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal virtual FormatBase GetInfoForModification() {
			if (IsUpdateLocked)
				return BatchUpdateHelper.DeferredNotifications;
			else
				return FormatInfo.Clone();
		}
		protected internal virtual bool ReplaceInfo(FormatBase newValue, DocumentModelChangeActions changeActions) {
			if (IsUpdateLocked) {
				return false;
			}
			int newIndex = GetInfoIndex(newValue);
			return ChangeFormatIndex(newIndex, changeActions);
		}
		public bool ChangeFormatIndex(int newIndex, DocumentModelChangeActions changeActions) {
			IDocumentModel documentModel = DocumentModel;
			Debug.Assert(newIndex < GetCache(documentModel).Count);
			if (newIndex != OriginalFormatIndex) {
				ChangeIndexCore(newIndex, changeActions);
				return true;
			}
			else
				return false;
		}
		protected internal virtual void ChangeIndexCore(int newIndex, DocumentModelChangeActions changeActions) {
			IDocumentModel documentModel = DocumentModel;
			documentModel.BeginUpdate();
			try {
				OnBeginAssign();
				try {
					IndexChangedHistoryItemCore<DocumentModelChangeActions> item = CreateIndexChangedHistoryItem();
					Debug.Assert(item != null);
					item.OldIndex = OriginalFormatIndex;
					item.NewIndex = newIndex;
					item.ChangeActions = changeActions;
					documentModel.History.Add(item);
					item.Execute();
				}
				finally {
					OnEndAssign();
				}
			}
			finally {
				documentModel.EndUpdate();
			}
		}
		protected virtual IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateIndexChangedHistoryItem() {
			return new IndexChangedHistoryItem<DocumentModelChangeActions>(Workbook, this);
		}
		protected internal virtual void NotifyFakeAssign() {
			if (IsUpdateLocked)
				BatchUpdateHelper.FakeAssignDetected = true;
			else {
				DocumentModel.BeginUpdate();
				try {
					OnBeginAssign();
					try {
					}
					finally {
						OnEndAssign();
					}
				}
				finally {
					DocumentModel.EndUpdate();
				}
			}
		}
		#endregion
		protected internal virtual void OnBeginAssign() {
		}
		protected internal virtual void OnEndAssign() {
		}
		#region IIndexBasedObject<DocumentModelChangeActions> Members
		IDocumentModelPart IIndexBasedObject<DocumentModelChangeActions>.DocumentModelPart { get { return Worksheet; } }
		int IIndexBasedObject<DocumentModelChangeActions>.GetIndex() {
			return OriginalFormatIndex;
		}
		void IIndexBasedObject<DocumentModelChangeActions>.SetIndex(int index, DocumentModelChangeActions changeActions) {
			this.FormatIndex = (short)index;
			sheet.WebRanges.AddChangedCellPosition(this);
		}
		#endregion
		public void CellCopyFormatSameDocumentModels(ICell source) {
			Debug.Assert(!this.IsUpdateLocked);
			Debug.Assert(Object.ReferenceEquals(this.DocumentModel, source.DocumentModel));
			if (this.FormatIndex != source.FormatIndex)
				ChangeFormatIndex(source.FormatIndex, GetBatchUpdateChangeActions());
			else
				NotifyFakeAssign();
		}
	}
	#endregion
}
