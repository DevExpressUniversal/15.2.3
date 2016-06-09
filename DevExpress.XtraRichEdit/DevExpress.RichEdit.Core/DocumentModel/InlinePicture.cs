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
using DevExpress.Office.History;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraRichEdit.Model {
	#region IInlinePicturePropertiesContainer
	public interface IInlinePicturePropertiesContainer {
		PieceTable PieceTable { get; }
		IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateInlinePicturePropertiesChangedHistoryItem();
		void OnInlineCharacterPropertiesChanged();
	}
	#endregion
	#region InlinePictureProperties
	public class InlinePictureProperties : InlineObjectProperties<InlinePictureInfo> {
		readonly IInlinePicturePropertiesContainer owner;
		public InlinePictureProperties(IInlinePicturePropertiesContainer owner)
			: base(owner.PieceTable) {
			this.owner = owner;
		}
		protected internal IInlinePicturePropertiesContainer Owner { get { return owner; } }
		#region Sizing
		public ImageSizeMode Sizing {
			get { return Info.Sizing; }
			set { SetPropertyValue(SetSizingCore, value); }
		}
		protected internal virtual DocumentModelChangeActions SetSizingCore(InlinePictureInfo info, ImageSizeMode value) {
			info.Sizing = value;
			return InlineObjectChangeActionsCalculator.CalculateChangeActions(InlineObjectChangeType.Sizing);
		}
		#endregion
		#region ResizingShadowDisplayMode
		public ResizingShadowDisplayMode ResizingShadowDisplayMode { 
			get { return Info.ResizingShadowDisplayMode; }
			set { SetPropertyValue(SetResizingShadowDisplayModeCore, value); }
		}
		protected internal virtual DocumentModelChangeActions SetResizingShadowDisplayModeCore(InlinePictureInfo info, ResizingShadowDisplayMode value) {
			info.ResizingShadowDisplayMode = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region LockAspectRatio
		public bool LockAspectRatio {
			get { return Info.LockAspectRatio; }
			set { SetPropertyValue(SetLockAspectRatioCore, value); }
		}
		protected internal virtual DocumentModelChangeActions SetLockAspectRatioCore(InlinePictureInfo info, bool value) {
			info.LockAspectRatio = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region PseudoInline
		public bool PseudoInline {
			get { return Info.PseudoInline; }
			set { SetPropertyValue(SetPseudoInlineCore, value); }
		}
		protected internal virtual DocumentModelChangeActions SetPseudoInlineCore(InlinePictureInfo info, bool value) {
			info.PseudoInline = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		protected override IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateIndexChangedHistoryItem() {
			return owner.CreateInlinePicturePropertiesChangedHistoryItem();
		}
		protected internal override UniqueItemsCache<InlinePictureInfo> GetCache(DocumentModel documentModel) {
			return documentModel.Cache.InlinePictureInfoCache;
		}
		protected override void OnIndexChanged() {
			Owner.OnInlineCharacterPropertiesChanged();
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return InlinePictureChangeActionsCalculator.CalculateChangeActions(InlinePictureChangeType.BatchUpdate);
		}
	}
	#endregion
	#region InlinePictureChangeType
	public enum InlinePictureChangeType {
		None = 0,
		BatchUpdate
	}
	#endregion
	#region InlinePictureChangeActionsCalculator
	public static class InlinePictureChangeActionsCalculator {
		internal class InlinePictureChangeActionsTable : Dictionary<InlinePictureChangeType, DocumentModelChangeActions> {
		}
		internal static InlinePictureChangeActionsTable inlinePictureChangeActionsTable = CreateInlinePictureChangeActionsTable();
		internal static InlinePictureChangeActionsTable CreateInlinePictureChangeActionsTable() {
			InlinePictureChangeActionsTable table = new InlinePictureChangeActionsTable();
			table.Add(InlinePictureChangeType.None, DocumentModelChangeActions.None);
			table.Add(InlinePictureChangeType.BatchUpdate, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetCaretInputPositionFormatting | DocumentModelChangeActions.ResetRuler);
			return table;
		}
		public static DocumentModelChangeActions CalculateChangeActions(InlinePictureChangeType change) {
			return inlinePictureChangeActionsTable[change];
		}
	}
	#endregion
	#region ResizingShadowDisplayMode
	public enum ResizingShadowDisplayMode {
		Content,
		WireFrame
	}
	#endregion
	#region JSONInlineObjectProperty
	public enum JSONInlineObjectProperty {
		Scales = 0,
		LockAspectRatio = 1
	}
	#endregion
	#region InlinePictureInfo
	public class InlinePictureInfo : InlineObjectInfo, ICloneable<InlinePictureInfo>, ISupportsCopyFrom<InlinePictureInfo> {
		ImageSizeMode sizing = ImageSizeMode.StretchImage;
		ResizingShadowDisplayMode resizingShadowDisplayMode = ResizingShadowDisplayMode.Content;
		bool lockAspectRatio = true;
		bool pseudoInline;
		public ImageSizeMode Sizing { get { return sizing; } set { sizing = value; } }
		public ResizingShadowDisplayMode ResizingShadowDisplayMode { get { return resizingShadowDisplayMode; } set { resizingShadowDisplayMode = value; } }
		public bool LockAspectRatio { get { return lockAspectRatio; } set { lockAspectRatio = value; } }
		public bool PseudoInline { get { return pseudoInline; } set { pseudoInline = value; } }
		public override bool Equals(object obj) {
			InlinePictureInfo info = obj as InlinePictureInfo;
			if (info == null)
				return false;
			return base.Equals(obj) && 
				info.Sizing == Sizing &&
				info.ResizingShadowDisplayMode == ResizingShadowDisplayMode &&
				info.LockAspectRatio == LockAspectRatio &&
				info.PseudoInline == PseudoInline;
		}
		public override int GetHashCode() {
			return (int)ScaleX | (((int)ScaleY) << 12) | ((int)Sizing << 24) | ((int)ResizingShadowDisplayMode << 28) | (LockAspectRatio.GetHashCode() << 29) | (PseudoInline.GetHashCode() << 30);
		}
		public InlinePictureInfo Clone() {
			InlinePictureInfo result = new InlinePictureInfo();
			result.CopyFrom(this);
			return result;
		}
		public void CopyFrom(InlinePictureInfo info) {
			CopyFromCore(info);
			Sizing = info.Sizing;
			ResizingShadowDisplayMode = info.ResizingShadowDisplayMode;
			LockAspectRatio = info.LockAspectRatio;
			PseudoInline = info.PseudoInline;
		}
	}
	#endregion
	#region InlinePictureInfoCache
	public class InlinePictureInfoCache : UniqueItemsCache<InlinePictureInfo> {
		public InlinePictureInfoCache(DocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override InlinePictureInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			InlinePictureInfo defaultItem = new InlinePictureInfo();
			defaultItem.ScaleX = 100;
			defaultItem.ScaleY = 100;
			return defaultItem;
		}
	}
	#endregion
}
