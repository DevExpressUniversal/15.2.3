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
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Utils;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Model {
	#region InlineCustomObjectProperties
	public class InlineCustomObjectProperties : InlineObjectProperties<InlineCustomObjectInfo> {
		public InlineCustomObjectProperties(PieceTable pieceTable)
			: base(pieceTable) {
		}
		#region Properties
		#region State
		public byte[] State {
			get { return Info.State; }
			set {
				SetPropertyValue(SetStateCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetStateCore(InlineCustomObjectInfo info, byte[] value) {
			info.State = value;
			return InlineCustomObjectChangeActionsCalculator.CalculateChangeActions(InlineCustomObjectChangeType.State);
		}
		#endregion
		#endregion
		#region Events
		#region IndexChanged
		EventHandler onIndexChanged;
		public event EventHandler IndexChanged { add { onIndexChanged += value; } remove { onIndexChanged -= value; } }
		protected internal virtual void RaiseIndexChanged() {
			if (onIndexChanged != null)
				onIndexChanged(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		protected internal override UniqueItemsCache<InlineCustomObjectInfo> GetCache(DocumentModel documentModel) {
			return documentModel.Cache.InlineCustomObjectInfoCache;
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return InlineCustomObjectChangeActionsCalculator.CalculateChangeActions(InlineCustomObjectChangeType.BatchUpdate);
		}
		protected override void OnIndexChanged() {
			base.OnIndexChanged();
			RaiseIndexChanged();
		}
	}
	#endregion
	#region InlineCustomObjectChangeType
	public enum InlineCustomObjectChangeType {
		None = 0,
		State,
		BatchUpdate
	}
	#endregion
	#region InlineCustomObjectChangeActionsCalculator
	public static class InlineCustomObjectChangeActionsCalculator {
		internal class InlineCustomObjectChangeActionsTable : Dictionary<InlineCustomObjectChangeType, DocumentModelChangeActions> {
		}
		internal static InlineCustomObjectChangeActionsTable inlineCustomObjectChangeActionsTable = CreateInlineCustomObjectChangeActionsTable();
		internal static InlineCustomObjectChangeActionsTable CreateInlineCustomObjectChangeActionsTable() {
			InlineCustomObjectChangeActionsTable table = new InlineCustomObjectChangeActionsTable();
			table.Add(InlineCustomObjectChangeType.None, DocumentModelChangeActions.None);
			table.Add(InlineCustomObjectChangeType.State, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetCaretInputPositionFormatting | DocumentModelChangeActions.ResetRuler);
			table.Add(InlineCustomObjectChangeType.BatchUpdate, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetCaretInputPositionFormatting | DocumentModelChangeActions.ResetRuler);
			return table;
		}
		public static DocumentModelChangeActions CalculateChangeActions(InlineCustomObjectChangeType change) {
			return inlineCustomObjectChangeActionsTable[change];
		}
	}
	#endregion
	#region InlineCustomObjectInfo
	public class InlineCustomObjectInfo : InlineObjectInfo, ICloneable<InlineCustomObjectInfo>, ISupportsCopyFrom<InlineCustomObjectInfo> {
		byte[] state;
		public byte[] State { get { return state; } set { state = value; } }
		public override bool Equals(object obj) {
			if (obj == null)
				return false;
			if (!base.Equals(obj))
				return false;
			InlineCustomObjectInfo info = (InlineCustomObjectInfo)obj;
			byte[] otherObjectState = info.state;
			if (Object.ReferenceEquals(state, otherObjectState))
				return true;
			if (state == null || otherObjectState == null)
				return false;
			if (state.Length != otherObjectState.Length)
				return false;
			int count = state.Length;
			for (int i = 0; i < count; i++) {
				if (state[i] != otherObjectState[i])
					return false;
			}
			return true;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public InlineCustomObjectInfo Clone() {
			InlineCustomObjectInfo result = new InlineCustomObjectInfo();
			result.CopyFrom(this);
			return result;
		}
		public void CopyFrom(InlineCustomObjectInfo info) {
			CopyFromCore(info);
			State = info.State;
		}
	}
	#endregion
	#region InlineCustomObjectInfoCache
	public class InlineCustomObjectInfoCache : UniqueItemsCache<InlineCustomObjectInfo> {
		public InlineCustomObjectInfoCache(DocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override InlineCustomObjectInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			InlineCustomObjectInfo defaultItem = new InlineCustomObjectInfo();
			defaultItem.ScaleX = 100;
			defaultItem.ScaleY = 100;
			return defaultItem;
		}
	}
	#endregion
}
