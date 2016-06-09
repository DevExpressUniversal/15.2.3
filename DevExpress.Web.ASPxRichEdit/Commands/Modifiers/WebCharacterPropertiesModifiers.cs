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
using DevExpress.Web.ASPxRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public abstract class WebCharacterPropertiesModifier {
		public void ModifyCharacterProperties(CharacterProperties properties, object newValue, bool newUse) {
			var baseInfo = properties.GetInfoForModification();
			var info = baseInfo.GetInfoForModification();
			var options = baseInfo.GetOptionsForModification();
			ApplyNewValue(info, newValue);
			ApplyNewUseValue(options, newUse);
			baseInfo.ReplaceInfo(info, options);
			properties.ReplaceInfo(baseInfo, CharacterFormattingChangeActionsCalculator.CalculateChangeActions(CharacterFormattingChangeType));
		}
		protected abstract CharacterFormattingChangeType CharacterFormattingChangeType { get; }
		public abstract CharacterFormattingOptions.Mask Mask { get; }
		public CharacterFormattingOptions.Mask GetNewMask(CharacterFormattingOptions.Mask oldMask, bool useValue) {
			if(useValue)
				return oldMask | Mask;
			else
				return oldMask & ~Mask;
		}
		public abstract void ApplyNewValue(CharacterFormattingInfo info, object newValue);
		public void ApplyNewUseValue(CharacterFormattingOptions options, bool useValue) {
			options.Value = GetNewMask(options.Value, useValue);
		}
		public abstract RunPropertyModifierBase CreateRunPropertyModifier(object newValue);
	}
	public abstract class WebCharacterPropertiesModifier<T> : WebCharacterPropertiesModifier {
		protected abstract void ModifyCharacterFormatting(CharacterFormattingInfo info, T value);
		protected virtual T GetNewValue(object value) { return (T)value; }
		public override void ApplyNewValue(CharacterFormattingInfo info, object newValue) {
			T value = GetNewValue(newValue);
			ModifyCharacterFormatting(info, value);
		}
		public override RunPropertyModifierBase CreateRunPropertyModifier(object newValue) {
			return CreateModifier(GetNewValue(newValue));
		}
		protected abstract RunPropertyModifier<T> CreateModifier(T newValue);
	}
	public abstract class WebCharacterPropertiesColorModifier : WebCharacterPropertiesModifier<Color> {
		protected override Color GetNewValue(object obj) {
			return Color.FromArgb((int)obj);
		}
	}
	public class WebCharacterPropertiesFontNameModifier : WebCharacterPropertiesModifier<string> {
		public override CharacterFormattingOptions.Mask Mask { get { return CharacterFormattingOptions.Mask.UseFontName; } }
		protected override RunPropertyModifier<string> CreateModifier(string newValue) {
			return new RunFontNamePropertyModifier(newValue);
		}
		protected override void ModifyCharacterFormatting(CharacterFormattingInfo info, string value) {
			info.FontName = value;
		}
		protected override CharacterFormattingChangeType CharacterFormattingChangeType {
			get { return XtraRichEdit.Model.CharacterFormattingChangeType.FontName; }
		}
	}
	public class WebCharacterPropertiesFontSizeModifier : WebCharacterPropertiesModifier<int> {
		public override CharacterFormattingOptions.Mask Mask { get { return CharacterFormattingOptions.Mask.UseDoubleFontSize; } }
		protected override RunPropertyModifier<int> CreateModifier(int newValue) {
			return new RunDoubleFontSizePropertyModifier(newValue);
		}
		protected override int GetNewValue(object obj) {
			return (int)(Convert.ToSingle(obj) * 2);
		}
		protected override void ModifyCharacterFormatting(CharacterFormattingInfo info, int value) {
			info.DoubleFontSize = value;
		}
		protected override CharacterFormattingChangeType CharacterFormattingChangeType {
			get { return XtraRichEdit.Model.CharacterFormattingChangeType.FontSize; }
		}
	}
	public class WebCharacterPropertiesFontBoldModifier : WebCharacterPropertiesModifier<bool> {
		public override CharacterFormattingOptions.Mask Mask { get { return CharacterFormattingOptions.Mask.UseFontBold; } }
		protected override RunPropertyModifier<bool> CreateModifier(bool newValue) {
			return new RunFontBoldModifier(newValue);
		}
		protected override void ModifyCharacterFormatting(CharacterFormattingInfo info, bool value) {
			info.FontBold = value;
		}
		protected override CharacterFormattingChangeType CharacterFormattingChangeType {
			get { return XtraRichEdit.Model.CharacterFormattingChangeType.FontBold; }
		}
	}
	public class WebCharacterPropertiesFontItalicModifier : WebCharacterPropertiesModifier<bool> {
		public override CharacterFormattingOptions.Mask Mask { get { return CharacterFormattingOptions.Mask.UseFontItalic; } }
		protected override RunPropertyModifier<bool> CreateModifier(bool newValue) {
			return new RunFontItalicModifier(newValue);
		}
		protected override void ModifyCharacterFormatting(CharacterFormattingInfo info, bool value) {
			info.FontItalic = value; ;
		}
		protected override CharacterFormattingChangeType CharacterFormattingChangeType {
			get { return XtraRichEdit.Model.CharacterFormattingChangeType.FontItalic; }
		}
	}
	public class WebCharacterPropertiesAllCapsModifier : WebCharacterPropertiesModifier<bool> {
		public override CharacterFormattingOptions.Mask Mask { get { return CharacterFormattingOptions.Mask.UseAllCaps; } }
		protected override RunPropertyModifier<bool> CreateModifier(bool newValue) {
			return new RunAllCapsModifier(newValue);
		}
		protected override void ModifyCharacterFormatting(CharacterFormattingInfo info, bool value) {
			info.AllCaps = value;
		}
		protected override CharacterFormattingChangeType CharacterFormattingChangeType {
			get { return XtraRichEdit.Model.CharacterFormattingChangeType.AllCaps; }
		}
	}
	public class WebCharacterPropertiesStrikeoutWordsOnlyModifier : WebCharacterPropertiesModifier<bool> {
		public override CharacterFormattingOptions.Mask Mask { get { return CharacterFormattingOptions.Mask.UseStrikeoutWordsOnly; } }
		protected override RunPropertyModifier<bool> CreateModifier(bool newValue) {
			return new RunStrikeoutWordsOnlyModifier(newValue);
		}
		protected override void ModifyCharacterFormatting(CharacterFormattingInfo info, bool value) {
			info.StrikeoutWordsOnly = value;
		}
		protected override CharacterFormattingChangeType CharacterFormattingChangeType {
			get { return XtraRichEdit.Model.CharacterFormattingChangeType.StrikeoutWordsOnly; }
		}
	}
	public class WebCharacterPropertiesUnderlineWordsOnlyModifier : WebCharacterPropertiesModifier<bool> {
		public override CharacterFormattingOptions.Mask Mask { get { return CharacterFormattingOptions.Mask.UseUnderlineWordsOnly; } }
		protected override RunPropertyModifier<bool> CreateModifier(bool newValue) {
			return new RunUnderlineWordsOnlyModifier(newValue);
		}
		protected override void ModifyCharacterFormatting(CharacterFormattingInfo info, bool value) {
			info.UnderlineWordsOnly = value;
		}
		protected override CharacterFormattingChangeType CharacterFormattingChangeType {
			get { return XtraRichEdit.Model.CharacterFormattingChangeType.UnderlineWordsOnly; }
		}
	}
	public class WebCharacterPropertiesFontStrikeoutTypeModifier : WebCharacterPropertiesModifier<StrikeoutType> {
		public override CharacterFormattingOptions.Mask Mask { get { return CharacterFormattingOptions.Mask.UseFontStrikeoutType; } }
		protected override RunPropertyModifier<StrikeoutType> CreateModifier(StrikeoutType newValue) {
			return new RunFontStrikeoutTypeModifier(newValue);
		}
		protected override void ModifyCharacterFormatting(CharacterFormattingInfo info, StrikeoutType value) {
			info.FontStrikeoutType = value;
		}
		protected override CharacterFormattingChangeType CharacterFormattingChangeType {
			get { return XtraRichEdit.Model.CharacterFormattingChangeType.FontStrikeoutType; }
		}
	}
	public class WebCharacterPropertiesFontUnderlineTypeModifier : WebCharacterPropertiesModifier<UnderlineType> {
		public override CharacterFormattingOptions.Mask Mask { get { return CharacterFormattingOptions.Mask.UseFontUnderlineType; } }
		protected override RunPropertyModifier<UnderlineType> CreateModifier(UnderlineType newValue) {
			return new RunFontUnderlineTypeModifier(newValue);
		}
		protected override void ModifyCharacterFormatting(CharacterFormattingInfo info, UnderlineType value) {
			info.FontUnderlineType = value;
		}
		protected override CharacterFormattingChangeType CharacterFormattingChangeType {
			get { return XtraRichEdit.Model.CharacterFormattingChangeType.FontUnderlineType; }
		}
	}
	public class WebCharacterPropertiesForeColorModifier : WebCharacterPropertiesColorModifier {
		public override CharacterFormattingOptions.Mask Mask { get { return CharacterFormattingOptions.Mask.UseForeColor; } }
		protected override RunPropertyModifier<Color> CreateModifier(Color newValue) {
			return new RunForeColorModifier(newValue);
		}
		protected override void ModifyCharacterFormatting(CharacterFormattingInfo info, Color value) {
			info.ForeColor = value;
		}
		protected override CharacterFormattingChangeType CharacterFormattingChangeType {
			get { return XtraRichEdit.Model.CharacterFormattingChangeType.ForeColor; }
		}
	}
	public class WebCharacterPropertiesBackColorModifier : WebCharacterPropertiesColorModifier {
		public override CharacterFormattingOptions.Mask Mask { get { return CharacterFormattingOptions.Mask.UseBackColor; } }
		protected override RunPropertyModifier<Color> CreateModifier(Color newValue) {
			return new RunBackColorModifier(newValue);
		}
		protected override void ModifyCharacterFormatting(CharacterFormattingInfo info, Color value) {
			info.BackColor = value;
		}
		protected override CharacterFormattingChangeType CharacterFormattingChangeType {
			get { return XtraRichEdit.Model.CharacterFormattingChangeType.BackColor; }
		}
	}
	public class WebCharacterPropertiesUnderlineColorModifier : WebCharacterPropertiesColorModifier {
		public override CharacterFormattingOptions.Mask Mask { get { return CharacterFormattingOptions.Mask.UseUnderlineColor; } }
		protected override RunPropertyModifier<Color> CreateModifier(Color newValue) {
			return new RunUnderlineColorModifier(newValue);
		}
		protected override void ModifyCharacterFormatting(CharacterFormattingInfo info, Color value) {
			info.UnderlineColor = value;
		}
		protected override CharacterFormattingChangeType CharacterFormattingChangeType {
			get { return XtraRichEdit.Model.CharacterFormattingChangeType.UnderlineColor; }
		}
	}
	public class WebCharacterPropertiesStrikeoutColorModifier : WebCharacterPropertiesColorModifier {
		public override CharacterFormattingOptions.Mask Mask { get { return CharacterFormattingOptions.Mask.UseStrikeoutColor; } }
		protected override RunPropertyModifier<Color> CreateModifier(Color newValue) {
			return new RunStrikeoutColorModifier(newValue);
		}
		protected override void ModifyCharacterFormatting(CharacterFormattingInfo info, Color value) {
			info.StrikeoutColor = value;
		}
		protected override CharacterFormattingChangeType CharacterFormattingChangeType {
			get { return XtraRichEdit.Model.CharacterFormattingChangeType.StrikeoutColor; }
		}
	}
	public class WebCharacterPropertiesScriptModifier : WebCharacterPropertiesModifier<CharacterFormattingScript> {
		public override CharacterFormattingOptions.Mask Mask { get { return CharacterFormattingOptions.Mask.UseScript; } }
		protected override RunPropertyModifier<CharacterFormattingScript> CreateModifier(CharacterFormattingScript newValue) {
			return new RunScriptModifier(newValue);
		}
		protected override void ModifyCharacterFormatting(CharacterFormattingInfo info, CharacterFormattingScript value) {
			info.Script = value;
		}
		protected override CharacterFormattingChangeType CharacterFormattingChangeType {
			get { return XtraRichEdit.Model.CharacterFormattingChangeType.Script; }
		}
	}
	public class WebCharacterPropertiesHiddenModifier : WebCharacterPropertiesModifier<bool> {
		public override CharacterFormattingOptions.Mask Mask { get { return CharacterFormattingOptions.Mask.UseHidden; } }
		protected override RunPropertyModifier<bool> CreateModifier(bool newValue) {
			return new RunHiddenModifier(newValue);
		}
		protected override void ModifyCharacterFormatting(CharacterFormattingInfo info, bool value) {
			info.Hidden = value;
		}
		protected override CharacterFormattingChangeType CharacterFormattingChangeType {
			get { return XtraRichEdit.Model.CharacterFormattingChangeType.Hidden; }
		}
	}
	public class WebCharacterPropertiesNoProofModifier : WebCharacterPropertiesModifier<bool> {
		public override CharacterFormattingOptions.Mask Mask { get { return CharacterFormattingOptions.Mask.UseNoProof; } }
		protected override RunPropertyModifier<bool> CreateModifier(bool newValue) {
			return new RunNoProofModifier(newValue);
		}
		protected override void ModifyCharacterFormatting(CharacterFormattingInfo info, bool value) {
			info.NoProof = value;
		}
		protected override CharacterFormattingChangeType CharacterFormattingChangeType {
			get { return XtraRichEdit.Model.CharacterFormattingChangeType.NoProof; }
		}
	}
}
