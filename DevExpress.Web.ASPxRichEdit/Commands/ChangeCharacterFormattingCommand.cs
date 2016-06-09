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
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Office.Utils;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public class ChangeCharacterFormattingUseValueCommand : WebRichEditStateBasedCommand<IntervalCommandState> {
		public ChangeCharacterFormattingUseValueCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.ChangeCharacterPropertiesUseValue; } }
		protected override bool IsEnabled() { return true; }
		protected override void PerformModifyModelCore(IntervalCommandState stateObject) {
			var useModifier = new RunCharacterPropertiesUseValueBaseModifier((CharacterFormattingOptions.Mask)stateObject.Value);
			PieceTable.ApplyCharacterFormatting(stateObject.Position, stateObject.Length, useModifier);
		}
		protected override bool IsValidOperation() {
			return Manager.Client.DocumentCapabilitiesOptions.CharacterFormattingAllowed;
		}
	}
	public class ChangeCharacterFormattingCommand : WebRichEditPropertyStateBasedCommand<IntervalWithUseCommandState, JSONCharacterFormattingProperty> {
		public ChangeCharacterFormattingCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.ChangeCharacterProperties; } }
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
		protected override IModelModifier<IntervalWithUseCommandState> CreateModifier(JSONCharacterFormattingProperty property) {
			var modifier = modifiers[property];
			return new CharacterPropertiesModifier(PieceTable, modifier);
		}
		protected override bool IsValidOperation() {
			return Manager.Client.DocumentCapabilitiesOptions.CharacterFormattingAllowed;
		}
		class CharacterPropertiesModifier: IntervalWithUseModelModifier {
			public CharacterPropertiesModifier(PieceTable pieceTable, WebCharacterPropertiesModifier modifier) : base(pieceTable) {
				InnerModifier = modifier;
			}
			public WebCharacterPropertiesModifier InnerModifier { get; private set; }
			protected override void ModifyCore(DocumentLogPosition position, int length, object value, bool useValue) {
				if(length == 0)
					Exceptions.ThrowArgumentException("length", length);
				if(position < DocumentLogPosition.MinValue)
					Exceptions.ThrowArgumentException("position", position);
				PieceTable.ApplyCharacterFormatting(position, length, InnerModifier.CreateRunPropertyModifier(value));
				PieceTable.ApplyCharacterFormatting(position, length, new RunCharacterPropertiesUseValueModifier(InnerModifier.Mask, useValue));
			}
		}
	}
	class RunCharacterPropertiesUseValueModifier : RunCharacterPropertiesUseValueBaseModifier {
		bool useValue;
		public RunCharacterPropertiesUseValueModifier(CharacterFormattingOptions.Mask mask, bool useValue)
			: base(mask) {
			this.useValue = useValue;
		}
		protected override CharacterFormattingOptions.Mask GetNewMask(CharacterFormattingOptions.Mask oldMask) {
			if(this.useValue)
				return oldMask | NewValue;
			else
				return oldMask & ~NewValue;
		}
	}
	class RunCharacterPropertiesUseValueBaseModifier : RunPropertyModifier<CharacterFormattingOptions.Mask> {
		public RunCharacterPropertiesUseValueBaseModifier(CharacterFormattingOptions.Mask mask)
			: base(mask) { }
		public override CharacterFormattingOptions.Mask GetInputPositionPropertyValue(InputPosition pos) {
			return pos.CharacterFormatting.UseValue;
		}
		public override CharacterFormattingOptions.Mask GetRunPropertyValue(TextRunBase run) {
			return run.CharacterProperties.Info.Options.Value;
		}
		public override void ModifyInputPositionCore(InputPosition pos) {
			pos.CharacterFormatting.UseValue = GetNewMask(pos.CharacterFormatting.UseValue);
		}
		public override void ModifyTextRun(TextRunBase run, RunIndex runIndex) {
			run.CharacterProperties.Info.Options.Value = GetNewMask(run.CharacterProperties.Info.Options.Value);
		}
		protected virtual CharacterFormattingOptions.Mask GetNewMask(CharacterFormattingOptions.Mask oldMask) {
			return NewValue;
		}
	}
}
