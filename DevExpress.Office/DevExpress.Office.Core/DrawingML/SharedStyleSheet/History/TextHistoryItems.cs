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
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.Office.Model;
using System.Globalization;
using DevExpress.Office.DrawingML;
namespace DevExpress.Office.Drawing {
	#region DrawingTextRunTextPropertyHistoryItem
	public class DrawingTextRunTextPropertyChangedHistoryItem : DrawingHistoryItem<DrawingTextRunStringBase, string> {
		public DrawingTextRunTextPropertyChangedHistoryItem(IDocumentModelPart documentModelPart, DrawingTextRunStringBase owner, string oldValue, string newValue)
			: base(documentModelPart, owner, oldValue, newValue) {
		}
		protected override void UndoCore() {
			Owner.SetTextCore(OldValue);
		}
		protected override void RedoCore() {
			Owner.SetTextCore(NewValue);
		}
	}
	#endregion
	#region DrawingTexFieldIdPropertyHistoryItem
	public class DrawingTextFieldIdPropertyChangedHistoryItem : DrawingHistoryItem<DrawingTextField, Guid> {
		public DrawingTextFieldIdPropertyChangedHistoryItem(IDocumentModelPart documentModelPart, DrawingTextField owner, Guid oldValue, Guid newValue)
			: base(documentModelPart, owner, oldValue, newValue) {
		}
		protected override void UndoCore() {
			Owner.SetFieldIdCore(OldValue);
		}
		protected override void RedoCore() {
			Owner.SetFieldIdCore(NewValue);
		}
	}
	#endregion
	#region DrawingTexFieldTypePropertyHistoryItem
	public class DrawingTextFieldTypePropertyChangedHistoryItem : DrawingHistoryItem<DrawingTextField, string> {
		public DrawingTextFieldTypePropertyChangedHistoryItem(IDocumentModelPart documentModelPart, DrawingTextField owner, string oldValue, string newValue)
			: base(documentModelPart, owner, oldValue, newValue) {
		}
		protected override void UndoCore() {
			Owner.SetFieldTypeCore(OldValue);
		}
		protected override void RedoCore() {
			Owner.SetFieldTypeCore(NewValue);
		}
	}
	#endregion
	#region DrawingTextFontStringChangedHistoryItem
	public class DrawingTextFontStringChangedHistoryItem : DrawingHistoryItem<DrawingTextFont, string> {
		public DrawingTextFontStringChangedHistoryItem(DrawingTextFont owner, int index, string oldValue, string newValue)
			: base(owner.DocumentModel.MainPart, owner, index, oldValue, newValue) {
		}
		protected override void UndoCore() {
			Owner.SetStringCore(Index, OldValue);
		}
		protected override void RedoCore() {
			Owner.SetStringCore(Index, NewValue);
		}
	}
	#endregion
	#region DrawingTextFontByteChangedHistoryItem
	public class DrawingTextFontByteChangedHistoryItem : DrawingHistoryItem<DrawingTextFont, byte> {
		public DrawingTextFontByteChangedHistoryItem(DrawingTextFont owner, int index, byte oldValue, byte newValue)
			: base(owner.DocumentModel.MainPart, owner, index, oldValue, newValue) {
		}
		protected override void UndoCore() {
			Owner.SetByteCore(Index, OldValue);
		}
		protected override void RedoCore() {
			Owner.SetByteCore(Index, NewValue);
		}
	}
	#endregion
	#region DrawingTextParagraphPropertiesHistoryItem
	public abstract class DrawingTextParagraphPropertiesHistoryItem : IndexChangedHistoryItemCore<DocumentModelChangeActions> {
		readonly DrawingTextParagraphProperties obj;
		static IDocumentModelPart GetModelPart(DrawingTextParagraphProperties obj) {
			Guard.ArgumentNotNull(obj, "obj");
			return obj.DocumentModel.MainPart;
		}
		protected DrawingTextParagraphPropertiesHistoryItem(DrawingTextParagraphProperties obj)
			: base(GetModelPart(obj)) {
			this.obj = obj;
		}
		protected DrawingTextParagraphProperties Object { get { return obj; } }
		public override IIndexBasedObject<DocumentModelChangeActions> GetObject() {
			return null;
		}
	}
	#endregion
	#region DrawingTextParagraphInfoIndexChangeHistoryItem
	public class DrawingTextParagraphInfoIndexChangeHistoryItem : DrawingTextParagraphPropertiesHistoryItem {
		public DrawingTextParagraphInfoIndexChangeHistoryItem(DrawingTextParagraphProperties obj)
			: base(obj) {
		}
		protected override void UndoCore() {
			Object.SetIndexCore(DrawingTextParagraphProperties.TextParagraphInfoIndexAccessor, OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			Object.SetIndexCore(DrawingTextParagraphProperties.TextParagraphInfoIndexAccessor, NewIndex, ChangeActions);
		}
	}
	#endregion
	#region DrawingTextSpacingInfoIndexChangeHistoryItem
	public class DrawingTextSpacingInfoIndexChangeHistoryItem : DrawingTextParagraphPropertiesHistoryItem {
		readonly int index;
		public DrawingTextSpacingInfoIndexChangeHistoryItem(DrawingTextParagraphProperties obj, int index)
			: base(obj) {
			this.index = index;
		}
		protected override void UndoCore() {
			Object.SetIndexCore((DrawingTextSpacingInfoIndexAccessor)DrawingTextParagraphProperties.TextParagraphPropertiesIndexAccessors[index], OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			Object.SetIndexCore((DrawingTextSpacingInfoIndexAccessor)DrawingTextParagraphProperties.TextParagraphPropertiesIndexAccessors[index], NewIndex, ChangeActions);
		}
	}
	#endregion
	#region DrawingTextParagraphPropertiesBulletChangedHistoryItem
	public class DrawingTextParagraphPropertiesBulletChangedHistoryItem : DrawingHistoryItem<DrawingTextParagraphProperties, IDrawingBullet> {
		DrawingBulletType type;
		public DrawingTextParagraphPropertiesBulletChangedHistoryItem(DrawingTextParagraphProperties owner, DrawingBulletType type, IDrawingBullet oldValue, IDrawingBullet newValue)
			: base(owner.DocumentModel.MainPart, owner, oldValue, newValue) {
			this.type = type;
		}
		protected override void UndoCore() {
			Owner.SetBulletCore(type, OldValue);
		}
		protected override void RedoCore() {
			Owner.SetBulletCore(type, NewValue);
		}
	}
	#endregion
	#region DrawingTextInsetChangeHistoryItem
	public class DrawingTextInsetChangeHistoryItem : DrawingHistoryItem<DrawingTextInset, int> {
		public DrawingTextInsetChangeHistoryItem(DrawingTextInset owner, int index, int oldValue, int newValue)
			: base(owner.DocumentModel.MainPart, owner, index, oldValue, newValue) {
		}
		protected override void UndoCore() {
			Owner.SetInsetCore(Index, OldValue);
		}
		protected override void RedoCore() {
			Owner.SetInsetCore(Index, NewValue);
		}
	}
	#endregion
	#region DrawingTextInsetHasValuesChangeHistoryItem
	public class DrawingTextInsetHasValuesChangeHistoryItem : DrawingHistoryItem<DrawingTextInset, bool> {
		public DrawingTextInsetHasValuesChangeHistoryItem(DrawingTextInset owner, int index, bool oldValue, bool newValue)
			: base(owner.DocumentModel.MainPart, owner, index, oldValue, newValue) {
		}
		protected override void UndoCore() {
			Owner.SetInsetHasValuesCore(Index, OldValue);
		}
		protected override void RedoCore() {
			Owner.SetInsetHasValuesCore(Index, NewValue);
		}
	}
	#endregion
	#region DrawingTextBodyPropertiesText3DChangedHistoryItem
	public class DrawingTextBodyPropertiesText3DChangedHistoryItem : DrawingHistoryItem<DrawingTextBodyProperties, IDrawingText3D> {
		public DrawingTextBodyPropertiesText3DChangedHistoryItem(DrawingTextBodyProperties owner, IDrawingText3D oldValue, IDrawingText3D newValue)
			: base(owner.DocumentModel.MainPart, owner, oldValue, newValue) {
		}
		protected override void UndoCore() {
			Owner.SetText3DCore(OldValue);
		}
		protected override void RedoCore() {
			Owner.SetText3DCore(NewValue);
		}
	}
	#endregion
	#region DrawingTextBodyPropertiesAutoFitChangedHistoryItem
	public class DrawingTextBodyPropertiesAutoFitChangedHistoryItem : DrawingHistoryItem<DrawingTextBodyProperties, IDrawingTextAutoFit> {
		public DrawingTextBodyPropertiesAutoFitChangedHistoryItem(DrawingTextBodyProperties owner, IDrawingTextAutoFit oldValue, IDrawingTextAutoFit newValue)
			: base(owner.DocumentModel.MainPart, owner, oldValue, newValue) {
		}
		protected override void UndoCore() {
			Owner.SetAutoFitCore(OldValue);
		}
		protected override void RedoCore() {
			Owner.SetAutoFitCore(NewValue);
		}
	}
	#endregion
	#region DrawingStrokeUnderlineChangedHistoryItem
	public class DrawingStrokeUnderlineChangedHistoryItem : DrawingHistoryItem<DrawingTextCharacterProperties, IStrokeUnderline> {
		public DrawingStrokeUnderlineChangedHistoryItem(DrawingTextCharacterProperties owner, IStrokeUnderline oldValue, IStrokeUnderline newValue)
			: base(owner.DocumentModel.MainPart, owner, oldValue, newValue) {
		}
		protected override void UndoCore() {
			Owner.SetStrokeUnderlineCore(OldValue);
		}
		protected override void RedoCore() {
			Owner.SetStrokeUnderlineCore(NewValue);
		}
	}
	#endregion
	#region DrawingUnderlineFillChangedHistoryItem
	public class DrawingUnderlineFillChangedHistoryItem : DrawingHistoryItem<DrawingTextCharacterProperties, IUnderlineFill> {
		public DrawingUnderlineFillChangedHistoryItem(DrawingTextCharacterProperties owner, IUnderlineFill oldValue, IUnderlineFill newValue)
			: base(owner.DocumentModel.MainPart, owner, oldValue, newValue) {
		}
		protected override void UndoCore() {
			Owner.SetUnderlineFillCore(OldValue);
		}
		protected override void RedoCore() {
			Owner.SetUnderlineFillCore(NewValue);
		}
	}
	#endregion
	#region DrawingLanguageChangedHistoryItem
	public class DrawingLanguageChangedHistoryItem : DrawingHistoryItem<DrawingTextCharacterProperties, CultureInfo> {
		Action<CultureInfo> action;
		public DrawingLanguageChangedHistoryItem(DrawingTextCharacterProperties owner, Action<CultureInfo> action, CultureInfo oldValue, CultureInfo newValue)
			: base(owner.DocumentModel.MainPart, owner, oldValue, newValue) {
			this.action = action;
		}
		protected override void UndoCore() {
			action(OldValue);
		}
		protected override void RedoCore() {
			action(NewValue);
		}
	}
	#endregion
	#region DrawingTextCharacterPropertiesBookmarkChangedHistoryItem
	public class DrawingTextCharacterPropertiesBookmarkChangedHistoryItem : DrawingHistoryItem<DrawingTextCharacterProperties, string> {
		public DrawingTextCharacterPropertiesBookmarkChangedHistoryItem(DrawingTextCharacterProperties owner, string oldValue, string newValue)
			: base(owner.DocumentModelPart, owner, oldValue, newValue) {
		}
		protected override void UndoCore() {
			Owner.SetBookmarkCore(OldValue);
		}
		protected override void RedoCore() {
			Owner.SetBookmarkCore(NewValue);
		}
	}
	#endregion
	#region DrawingTextParagraphOptionsChangedHistoryItem
	public class DrawingTextParagraphOptionsChangedHistoryItem : DrawingHistoryItem<DrawingTextParagraph, bool> {
		public DrawingTextParagraphOptionsChangedHistoryItem(DrawingTextParagraph owner, int index, bool oldValue, bool newValue)
			: base(owner.DocumentModel.MainPart, owner, index, oldValue, newValue) {
		}
		protected override void UndoCore() {
			Owner.SetOptionsCore(Index, OldValue);
		}
		protected override void RedoCore() {
			Owner.SetOptionsCore(Index, NewValue);
		}
	}
	#endregion
}
