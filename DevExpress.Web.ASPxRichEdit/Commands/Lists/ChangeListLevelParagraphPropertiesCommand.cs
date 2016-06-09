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

using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public class ChangeListLevelParagraphPropertiesCommand : WebRichEditPropertyStateBasedCommand<ListLevelWithUseCommandState, JSONParagraphFormattingProperty> {
		public ChangeListLevelParagraphPropertiesCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.ChangeListLevelParagraphProperties; } }
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
		protected override IModelModifier<ListLevelWithUseCommandState> CreateModifier(JSONParagraphFormattingProperty property) {
			var modifier = modifiers[property];
			return new ListLevelParagraphPropertiesModifier(DocumentModel, modifier);
		}
		class ListLevelParagraphPropertiesModifier : ListLevelWithUseModelModifier {
			public ListLevelParagraphPropertiesModifier(DocumentModel documentModel, WebParagraphPropertiesModifier modifier) : base(documentModel) {
				InnerModifier = modifier;
			}
			public WebParagraphPropertiesModifier InnerModifier { get; private set; }
			protected override void ModifyCore(bool isAbstract, int listIndex, int listLevelIndex, object value, bool useValue) {
				if(isAbstract) {
					var listLevel = DocumentModel.AbstractNumberingLists[new AbstractNumberingListIndex(listIndex)].Levels[listLevelIndex];
					InnerModifier.ModifyParagraphProperties(listLevel.ParagraphProperties, value, useValue);
					listLevel.OnParagraphPropertiesChanged();
				}
				else {
					var overrideListLevel = (OverrideListLevel)DocumentModel.NumberingLists[new NumberingListIndex(listIndex)].Levels[listLevelIndex];
					InnerModifier.ModifyParagraphProperties(overrideListLevel.ParagraphProperties, value, useValue);
					overrideListLevel.OnParagraphPropertiesChanged();
				}
			}
		}
	}
}
