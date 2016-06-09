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
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Model.History {
	#region ChangeParentStyleHistoryItem<T>
	public class ChangeParentStyleHistoryItem<T> : RichEditHistoryItem where T : StyleBase<T> {
		#region Fields
		readonly StyleBase<T> style;
		readonly T oldParentStyle;
		readonly T newParentStyle;
		#endregion
		public ChangeParentStyleHistoryItem(StyleBase<T> style, T oldParentStyle, T newParentStyle)
			: base(style.DocumentModel.MainPieceTable) {
			this.style = style;
			this.oldParentStyle = oldParentStyle;
			this.newParentStyle = newParentStyle;
		}
		#region Properties
		public T OldParentStyle { get { return oldParentStyle; } }
		public T NewParentStyle { get { return newParentStyle; } }
		public StyleBase<T> Style { get { return style; } }
		#endregion
		protected override void UndoCore() {
			Style.SetParentStyleCore(OldParentStyle);
		}
		protected override void RedoCore() {
			Style.SetParentStyleCore(NewParentStyle);
		}
	}
	#endregion
	#region ChangeStyleNameHistoryItem<T>
	public class ChangeStyleNameHistoryItem<T> : RichEditHistoryItem where T : StyleBase<T> {
		#region Fields
		readonly StyleBase<T> style;
		readonly string oldStyleName;
		readonly string newStyleName;
		#endregion
		public ChangeStyleNameHistoryItem(StyleBase<T> style, string oldStyleName, string newStyleName)
			: base(style.DocumentModel.MainPieceTable) {
			this.style = style;
			this.oldStyleName = oldStyleName;
			this.newStyleName = newStyleName;
		}
		#region Properties
		public string OldStyleName { get { return oldStyleName; } }
		public string NewStyleName { get { return newStyleName; } }
		public StyleBase<T> Style { get { return style; } }
		#endregion
		protected override void UndoCore() {
			Style.SetStyleNameCore(OldStyleName);
		}
		protected override void RedoCore() {
			Style.SetStyleNameCore(NewStyleName);
		}
	}
	#endregion
	#region StyleLinkHistoryItemBase (abstract class)
	public abstract class StyleLinkHistoryItemBase : RichEditHistoryItem {
		#region Fields
		readonly CharacterStyle characterStyle;
		readonly ParagraphStyle paragraphStyle;
		#endregion
		protected StyleLinkHistoryItemBase(ParagraphStyle paragraphStyle, CharacterStyle characterStyle)
			: base(paragraphStyle.DocumentModel.MainPieceTable) {
			Guard.ArgumentNotNull(paragraphStyle, "paragraphStyle");
			Guard.ArgumentNotNull(characterStyle, "characterStyle");
			this.characterStyle = characterStyle;
			this.paragraphStyle = paragraphStyle;
		}
		#region Properties
		public CharacterStyle CharacterStyle { get { return characterStyle; } }
		public ParagraphStyle ParagraphStyle { get { return paragraphStyle; } }
		#endregion
	}
	#endregion
	#region CreateStyleLinkHistoryItem
	public class CreateStyleLinkHistoryItem : StyleLinkHistoryItemBase {
		public CreateStyleLinkHistoryItem(ParagraphStyle paragraphStyle, CharacterStyle characterStyle)
			: base(paragraphStyle, characterStyle) {
		}
		protected override void UndoCore() {
			DocumentModel.StyleLinkManager.DeleteLinkCore(ParagraphStyle, CharacterStyle);
		}
		protected override void RedoCore() {
			DocumentModel.StyleLinkManager.CreateLinkCore(ParagraphStyle, CharacterStyle);
		}
	}
	#endregion
	#region DeleteStyleLinkHistoryItem
	public class DeleteStyleLinkHistoryItem : StyleLinkHistoryItemBase {
		public DeleteStyleLinkHistoryItem(ParagraphStyle paragraphStyle, CharacterStyle characterStyle)
			: base(paragraphStyle, characterStyle) {
		}
		protected override void UndoCore() {
			DocumentModel.StyleLinkManager.CreateLinkCore(ParagraphStyle, CharacterStyle);
		}
		protected override void RedoCore() {
			DocumentModel.StyleLinkManager.DeleteLinkCore(ParagraphStyle, CharacterStyle);
		}
	}
	#endregion
	#region StyleHistoryItem<T> (abstract class)
	public abstract class StyleHistoryItem<T> : RichEditHistoryItem where T : StyleBase<T> {
		readonly T style;
		protected StyleHistoryItem(T style)
			: base(style.DocumentModel.MainPieceTable) {
			Guard.ArgumentNotNull(style, "style");
			this.style = style;
		}
		public T Style { get { return style; } }
	}
	#endregion
	#region DeleteStyleHistoryItem<T>
	public class DeleteStyleHistoryItem<T> : StyleHistoryItem<T> where T : StyleBase<T> {
		readonly StyleCollectionBase<T> owner;
		public DeleteStyleHistoryItem(StyleCollectionBase<T> owner, T style)
			: base(style) {
			Guard.ArgumentNotNull(owner, "owner");
			this.owner = owner;
		}
		public StyleCollectionBase<T> Owner { get { return owner; } }
		protected override void UndoCore() {
			Style.DeletedCore = false;
		}
		protected override void RedoCore() {
			Owner.DeleteCore(Style);
		}
	}
	#endregion
	#region AddStyleHistoryItem<T>
	public class AddStyleHistoryItem<T> : StyleHistoryItem<T> where T : StyleBase<T> {
		readonly StyleCollectionBase<T> owner;
		bool oldDeleted;
		public AddStyleHistoryItem(StyleCollectionBase<T> owner, T style)
			: base(style) {
			Guard.ArgumentNotNull(owner, "owner");
			this.owner = owner;
		}
		public StyleCollectionBase<T> Owner { get { return owner; } }
		bool OldDeleted { get { return oldDeleted; } set { oldDeleted = value; } }
		protected override void UndoCore() {
			Debug.Assert(Owner.Count > 0);
			Debug.Assert(Owner[Owner.Count - 1] == Style);
			owner.RemoveLastStyle();
			Style.DeletedCore = OldDeleted;
		}
		protected override void RedoCore() {
			OldDeleted = Style.Deleted;
			Owner.AddCore(Style);
			Style.DeletedCore = false;
		}
	}
	#endregion
	#region AddDeletedStyleHistoryItem<T>
	public class AddDeletedStyleHistoryItem<T> : StyleHistoryItem<T> where T : StyleBase<T> {
		readonly StyleCollectionBase<T> owner;
		public AddDeletedStyleHistoryItem(StyleCollectionBase<T> owner, T style)
			: base(style) {
			this.owner = owner;
		}
		protected override void UndoCore() {
			owner.DeleteCore(Style);
		}
		protected override void RedoCore() {
			owner.AddDeletedStyleCore(Style);
		}
	}
	#endregion
	#region ParagraphStyleChangeAutoUpdatePropertyHistoryItem
	public class ParagraphStyleChangeAutoUpdatePropertyHistoryItem : RichEditHistoryItem {
		readonly ParagraphStyle style;
		public ParagraphStyleChangeAutoUpdatePropertyHistoryItem(ParagraphStyle style)
			: base(style.DocumentModel.MainPieceTable) {
			this.style = style;
		}
		public ParagraphStyle Style { get { return style; } }
		protected override void UndoCore() {
			Style.SetAutoUpdateCore(!Style.AutoUpdate);
		}
		protected override void RedoCore() {
			Style.SetAutoUpdateCore(!Style.AutoUpdate);
		}
	}
	#endregion
	#region ParagraphStyleChangeNextParagraphStylePropertyHistoryItem
	public class ParagraphStyleChangeNextParagraphStylePropertyHistoryItem : RichEditHistoryItem {
		readonly ParagraphStyle style;
		readonly ParagraphStyle oldValue;
		readonly ParagraphStyle newValue;
		public ParagraphStyleChangeNextParagraphStylePropertyHistoryItem(ParagraphStyle style, ParagraphStyle oldValue, ParagraphStyle newValue)
			: base(style.DocumentModel.MainPieceTable) {
			this.style = style;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		public ParagraphStyle Style { get { return style; } }
		protected override void UndoCore() {
			Style.SetNextParagraphStyleCore(oldValue);
		}
		protected override void RedoCore() {
			Style.SetNextParagraphStyleCore(newValue);
		}
	}
	#endregion
}
