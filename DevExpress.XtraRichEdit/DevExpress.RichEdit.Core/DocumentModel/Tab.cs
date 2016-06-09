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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
namespace DevExpress.XtraRichEdit.Model {
	#region ITabPropertiesContainer
	public interface ITabPropertiesContainer {
		DocumentModel DocumentModel { get; }
		IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateTabPropertiesChangedHistoryItem();
		void OnTabPropertiesChanged();
	}
	#endregion
	#region TabProperties
	public class TabProperties : RichEditIndexBasedObject<TabFormattingInfo> {
		readonly IParagraphPropertiesContainer owner;
		public TabProperties(IParagraphPropertiesContainer owner)
			: base(GetPieceTable(owner)) {
			this.owner = owner;
		}
		static PieceTable GetPieceTable(IParagraphPropertiesContainer owner) {
			Guard.ArgumentNotNull(owner, "owner");
			return owner.PieceTable;
		}
		public TabInfo this[int index] {
			get { return Info[index]; }
			set {
				Guard.ArgumentNotNull(value, "value");
				if (Info[index] == value)
					return;
				if (SuppressInsertTabs())
					return;
				SetTabCore(index, value);
			}
		}
		protected internal override UniqueItemsCache<TabFormattingInfo> GetCache(DocumentModel documentModel) {
			return documentModel.Cache.TabFormattingInfoCache;
		}
		void SetTabCore(int index, TabInfo value) {
			TabFormattingInfo newInfo = GetInfoForModification();
			newInfo[index] = value;
			ReplaceInfo(newInfo, GetBatchUpdateChangeActions());
		}
		public TabFormattingInfo GetTabs() {
			return Info.Clone();
		}
		public void SetTabs(TabFormattingInfo tabs) {
			if (SuppressInsertTabs())
				return;
			if (Info.Count == 0 && tabs.Count == 0)
				return;
			TabFormattingInfo newTabs = tabs.Clone();
			ReplaceInfo(newTabs, GetBatchUpdateChangeActions());
		}
		private bool SuppressInsertTabs() {
			return !DocumentModel.DocumentCapabilities.ParagraphTabsAllowed;
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.RaiseContentChanged | DocumentModelChangeActions.ForceResetHorizontalRuler;
		}
		protected internal override IObtainAffectedRangeListener GetObtainAffectedRangeListener() {
			return owner as IObtainAffectedRangeListener;
		}
	}
	#endregion
}
