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

using DevExpress.Office;
using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public class ChangeListLevelCharacterPropertiesCommand : WebRichEditPropertyStateBasedCommand<ListLevelWithUseCommandState, JSONCharacterFormattingProperty> {
		public ChangeListLevelCharacterPropertiesCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.ChangeListLevelCharacterProperties; } }
		protected override bool IsEnabled() { return true; }
		static Dictionary<JSONCharacterFormattingProperty, WebCharacterPropertiesModifier> modifiers = new Dictionary<JSONCharacterFormattingProperty, WebCharacterPropertiesModifier>() {
			{JSONCharacterFormattingProperty.FontInfoIndex, new WebCharacterPropertiesFontNameModifier()},
			{JSONCharacterFormattingProperty.FontSize, new WebCharacterPropertiesFontSizeModifier()},
			{JSONCharacterFormattingProperty.FontBold, new WebCharacterPropertiesFontBoldModifier()},
			{JSONCharacterFormattingProperty.FontItalic, new WebCharacterPropertiesFontItalicModifier()},
			{JSONCharacterFormattingProperty.FontStrikeoutType, new WebCharacterPropertiesFontStrikeoutTypeModifier()},
			{JSONCharacterFormattingProperty.FontUnderlineType, new WebCharacterPropertiesFontUnderlineTypeModifier()},
			{JSONCharacterFormattingProperty.AllCaps, new WebCharacterPropertiesAllCapsModifier()},
			{JSONCharacterFormattingProperty.StrikeoutWordsOnly, new WebCharacterPropertiesStrikeoutWordsOnlyModifier()},
			{JSONCharacterFormattingProperty.UnderlineWordsOnly, new WebCharacterPropertiesUnderlineWordsOnlyModifier()},
			{JSONCharacterFormattingProperty.ForeColor, new WebCharacterPropertiesForeColorModifier()},
			{JSONCharacterFormattingProperty.BackColor, new WebCharacterPropertiesBackColorModifier()},
			{JSONCharacterFormattingProperty.UnderlineColor, new WebCharacterPropertiesUnderlineColorModifier()},
			{JSONCharacterFormattingProperty.StrikeoutColor, new WebCharacterPropertiesStrikeoutColorModifier()},
			{JSONCharacterFormattingProperty.Script, new WebCharacterPropertiesScriptModifier()},
			{JSONCharacterFormattingProperty.Hidden, new WebCharacterPropertiesHiddenModifier()},
			{JSONCharacterFormattingProperty.NoProof, new WebCharacterPropertiesNoProofModifier()}
		};
		protected override IModelModifier<ListLevelWithUseCommandState> CreateModifier(JSONCharacterFormattingProperty property) {
			var modifier = modifiers[property];
			return new CharacterPropertiesModifier(DocumentModel, modifier);
		}
		class CharacterPropertiesModifier : ListLevelWithUseModelModifier {
			public CharacterPropertiesModifier(DocumentModel documentModel, WebCharacterPropertiesModifier modifier) : base(documentModel) {
				InnerModifier = modifier;
			}
			public WebCharacterPropertiesModifier InnerModifier { get; private set; }
			protected override void ModifyCore(bool isAbstract, int listIndex, int listLevelIndex, object value, bool useValue) {
				if(isAbstract) {
					var listLevel = DocumentModel.AbstractNumberingLists[new AbstractNumberingListIndex(listIndex)].Levels[listLevelIndex];
					InnerModifier.ModifyCharacterProperties(listLevel.CharacterProperties, value, useValue);
					listLevel.OnCharacterPropertiesChanged();
				}
				else {
					OverrideListLevel overrideListLevel = (OverrideListLevel)DocumentModel.NumberingLists[new NumberingListIndex(listIndex)].Levels[listLevelIndex];
					InnerModifier.ModifyCharacterProperties(overrideListLevel.CharacterProperties, value, useValue);
					overrideListLevel.OnCharacterPropertiesChanged();
				}
			}
		}
	}
}
