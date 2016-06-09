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
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
namespace DevExpress.XtraRichEdit.Model {
	#region CharacterStyle
	public class CharacterStyle : StyleBase<CharacterStyle>, ICharacterPropertiesContainer {
		readonly CharacterProperties characterProperties;
		public CharacterStyle(DocumentModel documentModel)
			: this(documentModel, null) {
		}
		public CharacterStyle(DocumentModel documentModel, CharacterStyle parent)
			: this(documentModel, parent, String.Empty) {
		}
		public CharacterStyle(DocumentModel documentModel, CharacterStyle parent, string styleName)
			: base(documentModel, parent, styleName) {
			this.characterProperties = CreateCharacterProperties();
			SubscribeCharacterPropertiesEvents();
		}
		#region Properties
		public override StyleType Type { get { return StyleType.CharacterStyle; } }
		public CharacterProperties CharacterProperties { get { return characterProperties; } }
		public ParagraphStyle LinkedStyle { get { return DocumentModel.StyleLinkManager.GetLinkedStyle(this); } }
		public bool HasLinkedStyle { get { return DocumentModel.StyleLinkManager.HasLinkedStyle(this); } }
		#endregion
		protected internal virtual CharacterProperties CreateCharacterProperties() {
			return new CharacterProperties(this);
		}
		public virtual CharacterProperties GetCharacterProperties(CharacterFormattingOptions.Mask mask) {
			if (Deleted)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedStyleError);
			if (CharacterProperties.Info.Options.GetVal(mask))
				return CharacterProperties;
			return Parent != null ? Parent.GetCharacterProperties(mask) : null;
		}
		public virtual MergedCharacterProperties GetMergedCharacterProperties() {
			if (Deleted)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedStyleError);
			CharacterPropertiesMerger merger = new CharacterPropertiesMerger(CharacterProperties);
			if (Parent != null)
				merger.Merge(Parent.GetMergedCharacterProperties());
			return merger.MergedProperties;
		}
		public virtual MergedCharacterProperties GetMergedWithDefaultCharacterProperties() {
			if (Deleted)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedStyleError);
			CharacterPropertiesMerger merger = new CharacterPropertiesMerger(GetMergedCharacterProperties());
			merger.Merge(DocumentModel.DefaultCharacterProperties);
			return merger.MergedProperties;
		}
		#region ICharacterPropertiesContainer Members
		IndexChangedHistoryItemCore<DocumentModelChangeActions> ICharacterPropertiesContainer.CreateCharacterPropertiesChangedHistoryItem() {
			return new IndexChangedHistoryItem<DocumentModelChangeActions>(DocumentModel.MainPieceTable, CharacterProperties);
		}
		void ICharacterPropertiesContainer.OnCharacterPropertiesChanged() {
			OnCharacterPropertiesChangedCore();
		}
		#endregion
		protected override void MergePropertiesWithParent() {
			if (Parent == null)
				Exceptions.ThrowInternalException();
			CharacterProperties.Merge(Parent.CharacterProperties);
		}
		protected virtual void OnCharacterPropertiesChangedCore() {
			BeginUpdate();
			try {
				NotifyStyleChanged();
				if (HasLinkedStyle)
					LinkedStyle.CharacterProperties.CopyFrom(CharacterProperties);
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void SubscribeCharacterPropertiesEvents() {
			CharacterProperties.ObtainAffectedRange += new ObtainAffectedRangeEventHandler(OnCharacterPropertiesObtainAffectedRange);
		}
		protected internal virtual void OnCharacterPropertiesObtainAffectedRange(object sender, ObtainAffectedRangeEventArgs e) {
			e.Start = RunIndex.Zero;
			e.End = RunIndex.MaxValue;
		}
		protected internal virtual CharacterStyle CopyFrom(DocumentModel targetModel) {
			CharacterStyle style = new CharacterStyle(targetModel);
			CopyTo(style);
			return style;
		}
		protected internal void CopyTo(CharacterStyle style) {
			style.CharacterProperties.CopyFrom(this.characterProperties.Info);
			style.StyleName = this.StyleName;
			if (Parent != null)
				style.Parent = style.DocumentModel.CharacterStyles[this.Parent.Copy(style.DocumentModel)];
			CharacterProperties.ApplyPropertiesDiff(style.CharacterProperties, style.GetMergedWithDefaultCharacterProperties().Info, this.GetMergedWithDefaultCharacterProperties().Info);
		}
		protected internal override void CopyProperties(CharacterStyle source) {
			CharacterProperties.CopyFrom(source.CharacterProperties.Info);
		}
		public override int Copy(DocumentModel targetModel) {
			CharacterStyleCollection targetStyles = targetModel.CharacterStyles;
			for (int i = 0; i < targetStyles.Count; i++) {
				if (this.StyleName == targetStyles[i].StyleName)
					return i;
			}
			return targetStyles.AddNewStyle(CopyFrom(targetModel));
		}
		public override void ResetCachedIndices(ResetFormattingCacheType resetFormattingCacheType) {
		}
	}
	#endregion
	#region CharacterStyleCollection
	public class CharacterStyleCollection : StyleCollectionBase<CharacterStyle> {
		public const int EmptyCharacterStyleIndex = 0;
		public static readonly string DefaultCharacterStyleName = "Default Paragraph Font";
		public static readonly string LineNumberingStyleName = "Line Number";
		public static readonly string HyperlinkStyleName = "Hyperlink";
		public CharacterStyleCollection(DocumentModel documentModel)
			: base(documentModel) {
			Items.Add(CreateLineNumberingStyle());
			Items.Add(CreateHyperlinkStyle());
		}
		protected internal override CharacterStyle CreateDefaultItem() {
			CharacterStyle style = new CharacterStyle(DocumentModel, null, DefaultCharacterStyleName);
			style.SemihiddenCore = true;
			return style;
		}
		protected internal virtual CharacterStyle CreateHyperlinkStyle() {
			CharacterStyle style = new CharacterStyle(DocumentModel, null, HyperlinkStyleName);
			style.CharacterProperties.BeginInit();
			style.CharacterProperties.ForeColor = DXColor.Blue;
			style.CharacterProperties.FontUnderlineType = UnderlineType.Single;
			style.CharacterProperties.EndInit();
			return style;
		}
		protected internal virtual CharacterStyle CreateLineNumberingStyle() {
			CharacterStyle style = new CharacterStyle(DocumentModel, this[EmptyCharacterStyleIndex], LineNumberingStyleName);
			style.SemihiddenCore = true;
			return style;
		}
		protected override void NotifyPieceTableStyleDeleting(PieceTable pieceTable, CharacterStyle style) {
			TextRunCollection runs = pieceTable.Runs;
			RunIndex count = new RunIndex(runs.Count);
			for (RunIndex i = RunIndex.Zero; i < count; i++) {
				if(runs[i].CharacterStyle == style)
					runs[i].CharacterStyleIndex = this.DefaultItemIndex;
			}
		}
	}
	#endregion
}
