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
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Utils;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Commands {
	#region ChangeCharacterPropertiesCommandBase (abstract class)
	public abstract class ChangeCharacterPropertiesCommandBase : SelectionBasedPropertyChangeCommandBase {
		protected ChangeCharacterPropertiesCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected internal virtual DocumentModelChangeActions ChangeCharacterFormatting(DocumentLogPosition logPositionFrom, DocumentLogPosition logPositionTo, RunPropertyModifierBase modifier) {
			int length = logPositionTo - logPositionFrom;
			if (length <= 0) {
				ParagraphIndex paragraphIndex = ActivePieceTable.FindParagraphIndex(logPositionFrom);
				Paragraph paragraph = ActivePieceTable.Paragraphs[paragraphIndex];
				if (paragraph.LogPosition + paragraph.Length - 1 == logPositionFrom) {
					bool resetInputPosition = (DocumentModel.DeferredChanges.ChangeActions & DocumentModelChangeActions.ResetCaretInputPositionFormatting) != 0;
					ActivePieceTable.ApplyCharacterFormatting(paragraph.LogPosition + paragraph.Length - 1, 1, modifier);
					if (!resetInputPosition)
						DocumentModel.DeferredChanges.ChangeActions &= ~DocumentModelChangeActions.ResetCaretInputPositionFormatting;
				}
				ChangeInputPositionCharacterFormatting(modifier);
				return DocumentModelChangeActions.None;
			}
			ActivePieceTable.ApplyCharacterFormatting(logPositionFrom, length, modifier);
			return DocumentModelChangeActions.ResetCaretInputPositionFormatting;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentModel.DocumentCapabilities.CharacterFormatting);
			ApplyDocumentProtectionToSelectedCharacters(state);
		}
		protected internal abstract void ChangeInputPositionCharacterFormatting(RunPropertyModifierBase modifier);
	}
	#endregion
	#region ChangeCharacterFormattingCommandBase<T> (abstract class)
	public abstract class ChangeCharacterFormattingCommandBase<T> : ChangeCharacterPropertiesCommandBase {
		protected ChangeCharacterFormattingCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected internal override DocumentModelChangeActions ChangeProperty(DocumentModelPosition start, DocumentModelPosition end, ICommandUIState state) {
			RunPropertyModifier<T> modifier = CreateModifier(state);
			return ChangeCharacterFormatting(start.LogPosition, end.LogPosition, modifier);
		}
		protected internal override void ChangeInputPositionCharacterFormatting(RunPropertyModifierBase modifier) {
			RunPropertyModifier<T> typedModifier = (RunPropertyModifier<T>)modifier;
			InputPosition pos = ActiveView.CaretPosition.GetInputPosition();
			typedModifier.ModifyInputPosition(pos);
		}
		protected internal abstract RunPropertyModifier<T> CreateModifier(ICommandUIState state);
		protected internal virtual bool GetCurrentPropertyValue(out T value) {
			value = default(T);
			RunPropertyModifier<T> modifier = CreateModifier(CreateDefaultCommandUIState());
			List<SelectionItem> items = DocumentModel.Selection.Items;
			int count = items.Count;
			Debug.Assert(count > 0);
			for (int i = 0; i < count; i++) {
				SelectionItem item = items[i];
				DocumentModelPosition start = CalculateStartPosition(item, false);
				DocumentModelPosition end = CalculateEndPosition(item, false);
				int length = end.LogPosition - start.LogPosition;
				if (count == 1 && length <= 0) {
					InputPosition pos = ActiveView.CaretPosition.GetInputPosition();
					value = modifier.GetInputPositionPropertyValue(pos);
					return true;
				}
				T runValue;
				bool obtainValueResult = !ObtainRunsPropertyValue(start, length, modifier, out runValue);
				if (i == 0)
					value = runValue;
				if (obtainValueResult)
					return false;
				if (i > 0 && !runValue.Equals(value))
					return false;
			}
			return true;
		}
		protected internal virtual bool ObtainRunsPropertyValue(DocumentModelPosition start, int length, RunPropertyModifier<T> modifier, out T value) {
			return (ActivePieceTable.ObtainRunsPropertyValue(start.LogPosition, length, modifier, out value));
		}
	}
	#endregion
	#region IXtraRichEditFormatting
	public interface IXtraRichEditFormatting {
		string GetCaption(DocumentModel documentModel);
		string GetLocalizedCaption(DocumentModel documentModel);
		bool AllowSelectionExpanding { get; }
		void Apply(DocumentModel documentModel, DocumentModelPosition start, DocumentModelPosition end);
	}
	#endregion
	#region StyleFormattingBase (abstract class)
	public abstract class StyleFormattingBase : IXtraRichEditFormatting {
		#region Fields
		readonly Guid styleId;
		#endregion
		protected StyleFormattingBase(Guid styleId) {
			this.styleId = styleId;
		}
		#region Properties
		public Guid StyleId { get { return styleId; } }
		public abstract bool AllowSelectionExpanding { get; }
		#endregion
		public abstract void Apply(DocumentModel documentModel, DocumentModelPosition start, DocumentModelPosition end);
		public abstract string GetStyleName(DocumentModel documentModel);
		public abstract string GetLocalizedStyleName(DocumentModel documentModel);
		public string GetCaption(DocumentModel documentModel) {
			return GetStyleName(documentModel);
		}
		public string GetLocalizedCaption(DocumentModel documentModel) {
			return GetLocalizedStyleName(documentModel);
		}
		protected virtual int CalculateAffectedParagraphCount(DocumentModel documentModel, DocumentModelPosition start, DocumentModelPosition end) {
			if (start.ParagraphIndex != end.ParagraphIndex)
				return end.ParagraphIndex - start.ParagraphIndex + 1;
			Paragraph paragraph = documentModel.ActivePieceTable.Paragraphs[start.ParagraphIndex];
			int length = end.LogPosition - start.LogPosition;
			bool lastParagraphCharSelected = length >= paragraph.Length - 1;
			if (start.LogPosition == paragraph.LogPosition && lastParagraphCharSelected || length == 0)
				return 1;
			else
				return 0;
		}
		public override bool Equals(object obj) {
			StyleFormattingBase formatting = obj as StyleFormattingBase;
			if (formatting == null)
				return false;
			return StyleId.Equals(formatting.StyleId);
		}
		public override int GetHashCode() {
			return StyleId.GetHashCode();
		}
	}
	#endregion
	#region CharacterStyleFormatting
	public class CharacterStyleFormatting : StyleFormattingBase {
		public CharacterStyleFormatting(Guid styleId)
			: base(styleId) {
		}
		public override bool AllowSelectionExpanding { get { return true; } }
		public override void Apply(DocumentModel documentModel, DocumentModelPosition start, DocumentModelPosition end) {
			bool isAllowedChangeCharacterStyle = documentModel.DocumentCapabilities.CharacterStyleAllowed;
			int length = end.LogPosition - start.LogPosition;
			if (isAllowedChangeCharacterStyle && length > 0)
				documentModel.ActivePieceTable.ApplyCharacterStyle(start.LogPosition, length, documentModel.CharacterStyles.GetStyleIndexByName(GetStyleName(documentModel)));
		}
		public override string GetStyleName(DocumentModel documentModel) {
			return documentModel.CharacterStyles.GetStyleById(StyleId).StyleName;
		}
		public override string GetLocalizedStyleName(DocumentModel documentModel) {
			return documentModel.CharacterStyles.GetStyleById(StyleId).LocalizedStyleName;
		}
	}
	#endregion
	#region ParagraphStyleFormatting
	public class ParagraphStyleFormatting : StyleFormattingBase {
		#region Fields
		#endregion
		public ParagraphStyleFormatting(Guid styleId)
			: base(styleId) {
		}
		#region Properties
		public override bool AllowSelectionExpanding { get { return false; } }
		#endregion
		public override void Apply(DocumentModel documentModel, DocumentModelPosition start, DocumentModelPosition end) {
			bool isAllowedChangeParagraphStyle = documentModel.DocumentCapabilities.ParagraphStyleAllowed;
			if (!isAllowedChangeParagraphStyle)
				return;
			int count = CalculateAffectedParagraphCount(documentModel, start, end);
			if (count > 0) {
				DocumentModelPosition startPosition = CalculateStartPosition(start);
				DocumentModelPosition endPosition = CalculateEndPosition(end);
				ChangeProperty(documentModel,startPosition, endPosition);
			}
			else
				ApplyLinkedCharacterStyle(documentModel, start, end);
		}
		public override string GetStyleName(DocumentModel documentModel) {
			return documentModel.ParagraphStyles.GetStyleById(StyleId).StyleName;
		}
		public override string GetLocalizedStyleName(DocumentModel documentModel) {
			ParagraphStyle style = documentModel.ParagraphStyles.GetStyleById(StyleId);
			return style != null ? style.LocalizedStyleName : String.Empty;
		}
		protected internal virtual DocumentModelPosition CalculateStartPosition(DocumentModelPosition position) {
			DocumentModelPosition result = position;
			ParagraphsDocumentModelIterator iterator = new ParagraphsDocumentModelIterator(position.PieceTable);
			if (iterator.IsNewElement(result))
				return result;
			else
				return iterator.MoveBack(result);
		}
		protected internal virtual DocumentModelPosition CalculateEndPosition(DocumentModelPosition position) {
			DocumentModelPosition result = position;
			if (result.LogPosition >= position.PieceTable.DocumentEndLogPosition) {
				result = position.Clone();
				result.ParagraphIndex = position.PieceTable.Paragraphs.Last.Index + 1;
				return result;
			}
			ParagraphsDocumentModelIterator iterator = new ParagraphsDocumentModelIterator(position.PieceTable);
			if (iterator.IsNewElement(result))
				return result;
			else
				return iterator.MoveForward(result);
		}
		protected internal virtual DocumentModelChangeActions ChangeProperty(DocumentModel documentModel, DocumentModelPosition start, DocumentModelPosition end) {
			ParagraphIndex startIndex = start.ParagraphIndex;
			ParagraphIndex endIndex = end.ParagraphIndex;
			PieceTable pieceTable = start.PieceTable;
			if (startIndex == endIndex)
				endIndex++;
			for (ParagraphIndex index = startIndex; index < endIndex; index++) {
				Paragraph paragraph = pieceTable.Paragraphs[index];
				paragraph.ParagraphProperties.ResetAllUse();
				paragraph.ParagraphStyleIndex = documentModel.ParagraphStyles.GetStyleIndexById(StyleId);
				paragraph.ResetRunsCharacterFormatting();
				if (paragraph.IsInList())
					paragraph.PieceTable.DeleteNumerationFromParagraph(paragraph);
				if (paragraph.GetOwnNumberingListIndex() == NumberingListIndex.NoNumberingList) {
					paragraph.ResetNumberingListIndex(NumberingListIndex.ListIndexNotSetted);
					paragraph.ParagraphProperties.ResetUse(ParagraphFormattingOptions.Mask.UseFirstLineIndent | ParagraphFormattingOptions.Mask.UseLeftIndent);
				}
			}
			return DocumentModelChangeActions.ResetCaretInputPositionFormatting;
		}
		protected virtual void ApplyLinkedCharacterStyle(DocumentModel documentModel, DocumentModelPosition start, DocumentModelPosition end) {
			int paragraphStyleIndex =  documentModel.ParagraphStyles.GetStyleIndexById(StyleId);
			ParagraphStyle paragraphStyle = documentModel.ParagraphStyles[paragraphStyleIndex];
			CharacterStyle characterStyle = paragraphStyle.LinkedStyle;
			if (!paragraphStyle.HasLinkedStyle) {
				characterStyle = new CharacterStyle(documentModel, documentModel.CharacterStyles.DefaultItem, String.Format(XtraRichEditLocalizer.GetString(XtraRichEditStringId.LinkedCharacterStyleFormatString), paragraphStyle.StyleName));
				documentModel.CharacterStyles.Add(characterStyle);
				documentModel.StyleLinkManager.CreateLink(paragraphStyle, characterStyle);
			}
			int length = end.LogPosition - start.LogPosition;
			if (length > 0)
				documentModel.ActivePieceTable.ApplyCharacterStyle(start.LogPosition, length, documentModel.CharacterStyles.IndexOf(characterStyle));
		}
	}
	#endregion
	#region TableStyleFormatting
	public class TableStyleFormatting : StyleFormattingBase {
		public TableStyleFormatting(Guid styleId)
			: base(styleId) {
		}
		public override bool AllowSelectionExpanding { get { return true; } }
		public override void Apply(DocumentModel documentModel, DocumentModelPosition start, DocumentModelPosition end) {
			bool isAllowedChangeTableStyle = documentModel.DocumentCapabilities.TableStyleAllowed;
			int length = end.LogPosition - start.LogPosition;
			if (isAllowedChangeTableStyle)
				documentModel.ActivePieceTable.ApplyTableStyle(start.LogPosition, length, documentModel.TableStyles.GetStyleIndexById(StyleId));
		}
		public override string GetStyleName(DocumentModel documentModel) {
			return documentModel.TableStyles.GetStyleById(StyleId).StyleName;
		}
		public override string GetLocalizedStyleName(DocumentModel documentModel) {
			return documentModel.TableStyles.GetStyleById(StyleId).LocalizedStyleName;
		}
	}
	#endregion
	#region TableCellStyleFormatting
	public class TableCellStyleFormatting : StyleFormattingBase {
		public TableCellStyleFormatting(Guid styleId)
			: base(styleId) {
		}
		public override bool AllowSelectionExpanding { get { return true; } }
		public override void Apply(DocumentModel documentModel, DocumentModelPosition start, DocumentModelPosition end) {
			bool isAllowedChangeTableCellStyle = documentModel.DocumentCapabilities.TableCellStyleAllowed;
			int length = end.LogPosition - start.LogPosition;
			if (isAllowedChangeTableCellStyle)
				documentModel.ActivePieceTable.ApplyTableCellStyle(start.LogPosition, length, documentModel.TableCellStyles.GetStyleIndexById(StyleId));
		}
		public override string GetStyleName(DocumentModel documentModel) {
			return documentModel.TableCellStyles.GetStyleById(StyleId).StyleName;
		}
		public override string GetLocalizedStyleName(DocumentModel documentModel) {
			return documentModel.TableCellStyles.GetStyleById(StyleId).LocalizedStyleName;
		}
	}
	#endregion
}
