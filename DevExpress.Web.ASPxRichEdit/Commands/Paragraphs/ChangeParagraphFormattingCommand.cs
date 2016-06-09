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
using System.Collections;
using System.Collections.Generic;
using DevExpress.Web.ASPxRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using System.Drawing;
using DevExpress.Office.Utils;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public class ChangeParagraphFormattingUseValueCommand : WebRichEditStateBasedCommand<IntervalCommandState> {
		public ChangeParagraphFormattingUseValueCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.ChangeParagraphPropertiesUseValue; } }
		protected override bool IsEnabled() { return true; }
		protected override void PerformModifyModelCore(IntervalCommandState stateObject) {
			var useModifier = new ParagraphPropertiesUseValueBaseModifier((ParagraphFormattingOptions.Mask)stateObject.Value);
			PieceTable.ApplyParagraphFormatting(stateObject.Position, stateObject.Length, useModifier);
		}
		protected override bool IsValidOperation() {
			return Manager.Client.DocumentCapabilitiesOptions.ParagraphFormattingAllowed;
		}
		class ParagraphPropertiesUseValueBaseModifier : ParagraphPropertyModifier<ParagraphFormattingOptions.Mask> {
			public ParagraphPropertiesUseValueBaseModifier(ParagraphFormattingOptions.Mask mask)
				: base(mask) { }
			public override ParagraphFormattingOptions.Mask GetParagraphPropertyValue(Paragraph paragraph) {
				return paragraph.ParagraphProperties.Info.UseValue;
			}
			public override void ModifyParagraph(Paragraph paragraph, ParagraphIndex paragraphIndex) {
				paragraph.ParagraphProperties.Info.UseValue = GetNewMask(paragraph.ParagraphProperties.Info.UseValue);
			}
			protected virtual ParagraphFormattingOptions.Mask GetNewMask(ParagraphFormattingOptions.Mask oldMask) {
				return NewValue;
			}
		}
	}
	public class ChangeParagraphFormattingCommand : WebRichEditPropertyStateBasedCommand<IntervalWithUseCommandState, JSONParagraphFormattingProperty> {
		public ChangeParagraphFormattingCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.ChangeParagraphProperties; } }
		protected override bool IsEnabled() { return true; }
		static Dictionary<JSONParagraphFormattingProperty, WebParagraphPropertiesModifier> modifiers = new Dictionary<JSONParagraphFormattingProperty, WebParagraphPropertiesModifier>() {
				{JSONParagraphFormattingProperty.Alignment, new WebParagraphPropertiesAlignmentModifier()},
				{JSONParagraphFormattingProperty.FirstLineIndent, new WebParagraphPropertiesFirstLineIndentModifier()},
				{JSONParagraphFormattingProperty.FirstLineIndentType, new WebParagraphPropertiesFirstLineIndentTypeModifier()},
				{JSONParagraphFormattingProperty.LeftIndent, new WebParagraphPropertiesLeftIndentModifier()},
				{JSONParagraphFormattingProperty.LineSpacing, new WebParagraphPropertiesLineSpacingModifier()},
				{JSONParagraphFormattingProperty.LineSpacingType, new WebParagraphPropertiesLineSpacingTypeModifier()},
				{JSONParagraphFormattingProperty.RightIndent, new WebParagraphPropertiesRightIndentModifier()},
				{JSONParagraphFormattingProperty.SpacingBefore, new WebParagraphPropertiesSpacingBeforeModifier()},
				{JSONParagraphFormattingProperty.SpacingAfter, new WebParagraphPropertiesSpacingAfterModifier()},
				{JSONParagraphFormattingProperty.SuppressHyphenation, new WebParagraphPropertiesSuppressHyphenationModifier()},
				{JSONParagraphFormattingProperty.SuppressLineNumbers, new WebParagraphPropertiesSuppressLineNumbersModifier()},
				{JSONParagraphFormattingProperty.ContextualSpacing, new WebParagraphPropertiesContextualSpacingModifier()},
				{JSONParagraphFormattingProperty.PageBreakBefore, new WebParagraphPropertiesPageBreakBeforeModifier()},
				{JSONParagraphFormattingProperty.BeforeAutoSpacing, new WebParagraphPropertiesBeforeAutoSpacingModifier()},
				{JSONParagraphFormattingProperty.AfterAutoSpacing, new WebParagraphPropertiesAfterAutoSpacingModifier()},
				{JSONParagraphFormattingProperty.KeepWithNext, new WebParagraphPropertiesKeepWithNextModifier()},
				{JSONParagraphFormattingProperty.KeepLinesTogether, new WebParagraphPropertiesKeepLinesTogetherModifier()},
				{JSONParagraphFormattingProperty.WidowOrphanControl, new WebParagraphPropertiesWidowOrphanControl()},
				{JSONParagraphFormattingProperty.OutlineLevel, new WebParagraphPropertiesOutlineLevelModifier()},
				{JSONParagraphFormattingProperty.BackColor, new WebParagraphPropertiesBackColorModifier()},
				{JSONParagraphFormattingProperty.LeftBorder, new WebParagraphPropertiesLeftBorderModifier()},
				{JSONParagraphFormattingProperty.RightBorder, new WebParagraphPropertiesRightBorderModifier()},
				{JSONParagraphFormattingProperty.TopBorder, new WebParagraphPropertiesTopBorderModifier()},
				{JSONParagraphFormattingProperty.BottomBorder, new WebParagraphPropertiesBottomBorderModifier()}
			};
		protected override IModelModifier<IntervalWithUseCommandState> CreateModifier(JSONParagraphFormattingProperty property) {
			var modifier = modifiers[property];
			return new IntervalParagraphPropertiesModifier(PieceTable, modifier);
		}
		protected override bool IsValidOperation() {
			return Manager.Client.DocumentCapabilitiesOptions.ParagraphFormattingAllowed;
		}
		class IntervalParagraphPropertiesModifier : IntervalWithUseModelModifier {
			public IntervalParagraphPropertiesModifier(PieceTable pieceTable, WebParagraphPropertiesModifier modifier)
				: base(pieceTable) {
					InnerModifier = modifier;
			}
			public WebParagraphPropertiesModifier InnerModifier { get; private set; }
			protected override void ModifyCore(DocumentLogPosition position, int length, object value, bool useValue) {
				if(length == 0)
					Exceptions.ThrowArgumentException("length", length);
				if(position < DocumentLogPosition.MinValue)
					Exceptions.ThrowArgumentException("position", position);
				RunInfo rinfo = PieceTable.FindRunInfo(position, length);
				for(ParagraphIndex i = rinfo.Start.ParagraphIndex; i <= rinfo.End.ParagraphIndex; i++)
					ModifyParagraph(PieceTable.Paragraphs[i], value, useValue);
			}
			public void ModifyParagraph(Paragraph paragraph, object value, bool useValue) {
				InnerModifier.ModifyParagraphProperties(paragraph.ParagraphProperties, value, useValue);
				paragraph.ResetCachedIndices(ResetFormattingCacheType.Paragraph);
			}
		}
	}
}
