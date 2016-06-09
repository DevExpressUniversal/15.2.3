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
using DevExpress.XtraRichEdit.Native;
using System.IO;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Import.Doc;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Export.Doc {
	public class DocCharacterPropertiesActions : ICharacterPropertiesActions, IDisposable {
		#region Fields
		BinaryWriter writer;
		CharacterProperties properties;
		int fontNameIndex;
		bool useDefaultFontSize;
		DocPictureBulletInformation pictureBulletInformation;
		#endregion
		#region Constructors
		protected DocCharacterPropertiesActions(MemoryStream output, int fontNameIndex) {
			Guard.ArgumentNotNull(output, "output");
			this.writer = new BinaryWriter(output);
			this.fontNameIndex = fontNameIndex;
		}
		public DocCharacterPropertiesActions(MemoryStream output, CharacterProperties properties, int fontNameIndex)
			: this(output, fontNameIndex) {
			this.properties = properties;
		}
		#endregion
		#region Properties
		protected CharacterProperties CharacterProperties { get { return properties; } set { properties = value; } }
		DocumentModel DocumentModel { get { return properties != null ? properties.DocumentModel : null; } }
		protected DocPictureBulletInformation PictureBulletInformation { get { return pictureBulletInformation; } set { pictureBulletInformation = value; } }
		#endregion
		public void CreateInlinePicturePropertiesModifiers(int characterStyleIndex, int dataStreamOffset) {
			CreateCharacterPropertiesModifiers(characterStyleIndex, DocStyleIndexes.DefaultParagraphStyleIndex, true);
			PictureOffsetAction(dataStreamOffset);
		}
		public void CreateCharacterPropertiesModifiers() {
			CreateCharacterPropertiesModifiers(DocStyleIndexes.DefaultCharacterStyleIndex, DocStyleIndexes.DefaultParagraphStyleIndex, false);
		}
		public void CreateCharacterPropertiesModifiers(int characterStyleIndex, int paragraphStyleIndex, bool specialSymbol) {
			SetUseDefaultFontSize(characterStyleIndex, paragraphStyleIndex);
			if (this.properties != null)
				CharacterPropertiesHelper.ForEach(this);
			if (specialSymbol)
				SpecialSymbolAction();
			PictureBulletInformationAction();
		}
		protected virtual void SetUseDefaultFontSize(int characterStyleIndex, int paragraphStyleIndex) {
			if (characterStyleIndex != DocStyleIndexes.DefaultCharacterStyleIndex)
				WriteCharacterStyleIndex(characterStyleIndex);
			else
				useDefaultFontSize = paragraphStyleIndex == DocStyleIndexes.DefaultParagraphStyleIndex;
		}
		#region ICharacterPropertiesActions Members
		public void AllCapsAction() {
			if (!CharacterProperties.UseAllCaps)
				return;
			DocCommandAllCaps command = new DocCommandAllCaps();
			command.Value = CharacterProperties.Info.AllCaps;
			command.Write(writer);
		}
		public void BackColorAction() {
			if (!CharacterProperties.UseBackColor || DXColor.IsTransparentOrEmpty(CharacterProperties.BackColor))
				return;
			DocCommandBackColor command = new DocCommandBackColor();
			command.ShadingDescriptor.BackgroundColor = CharacterProperties.Info.BackColor;
			command.Write(writer);
		}
#if THEMES_EDIT
		public void BackColorInfoAction() { 
		}
		public void ShadingAction() { 
		}
		public void FontInfoAction() { 
		}
		public void ForeColorInfoAction() { 
		}
		public void StrikeoutColorInfoAction() {
		}
		public void UnderlineColorInfoAction() { 
		}
#endif
		public void FontBoldAction() {
			if (!CharacterProperties.UseFontBold)
				return;
			DocCommandBold command = new DocCommandBold();
			command.Value = CharacterProperties.Info.FontBold;
			command.Write(writer);
		}
		public void FontItalicAction() {
			if (!CharacterProperties.UseFontItalic)
				return;
			DocCommandItalic command = new DocCommandItalic();
			command.Value = CharacterProperties.Info.FontItalic;
			command.Write(writer);
		}
		public void FontNameAction() {
			if (!CharacterProperties.UseFontName)
				return;
			DocCommandFontName fontNameCommand = new DocCommandFontName();
			fontNameCommand.Value = this.fontNameIndex;
			fontNameCommand.Write(writer);
			DocCommandEastAsianFontName eastAsianFontNameCommand = new DocCommandEastAsianFontName();
			eastAsianFontNameCommand.Value = this.fontNameIndex;
			eastAsianFontNameCommand.Write(writer);
			DocCommandNonASCIIFontName nonASCIIFontNameCommand = new DocCommandNonASCIIFontName();
			nonASCIIFontNameCommand.Value = fontNameIndex;
			nonASCIIFontNameCommand.Write(writer);
		}
		public void FontSizeAction() {
		}
		public void DoubleFontSizeAction() {
			bool useFontSize = CharacterProperties.UseDoubleFontSize;
			if (!useFontSize && !useDefaultFontSize)
				return;
			DocCommandFontSize command = new DocCommandFontSize();
			command.DoubleFontSize = (useFontSize) ? CharacterProperties.Info.DoubleFontSize : DocumentModel.ParagraphStyles.DefaultItem.CharacterProperties.DoubleFontSize;
			command.Write(writer);
		}
		public void FontStrikeoutTypeAction() {
			if (!CharacterProperties.UseFontStrikeoutType)
				return;
			DocCommandStrike commandStrike = new DocCommandStrike();
			DocCommandDoubleStrike commandDoubleStrike = new DocCommandDoubleStrike();
			if (CharacterProperties.Info.FontStrikeoutType == StrikeoutType.Single) {
				commandStrike.Value = true;
				commandStrike.Write(writer);
				commandDoubleStrike.Value = false;
				commandDoubleStrike.Write(writer);
			}
			else if (CharacterProperties.Info.FontStrikeoutType == StrikeoutType.Double) {
				commandStrike.Value = false;
				commandStrike.Write(writer);
				commandDoubleStrike.Value = true;
				commandDoubleStrike.Write(writer);
			}
			else {
				commandStrike.Value = false;
				commandStrike.Write(writer);
				commandDoubleStrike.Value = false;
				commandDoubleStrike.Write(writer);
			}
		}
		public void FontUnderlineTypeAction() {
			if (!CharacterProperties.UseFontUnderlineType)
				return;
			UnderlineAction();
		}
		public void ForeColorAction() {
			if (!CharacterProperties.UseForeColor)
				return;
			DocCommandForeColor command = new DocCommandForeColor();
			command.Color = CharacterProperties.Info.ForeColor;
			command.Write(writer);
		}
		public void HiddenAction() {
			if (!CharacterProperties.UseHidden)
				return;
			DocCommandHidden command = new DocCommandHidden();
			command.Value = CharacterProperties.Info.Hidden;
			command.Write(writer);
		}
		public void ScriptAction() {
			if (!CharacterProperties.UseScript)
				return;
			DocCommandScript command = new DocCommandScript();
			command.Script = CharacterProperties.Info.Script;
			command.Write(writer);
		}
		public void LangInfoAction() { 
		}
		public void NoProofAction() {
		}
		public void StrikeoutColorAction() {
		}
		public void StrikeoutWordsOnlyAction() {
		}
		public void UnderlineColorAction() {
			if (!CharacterProperties.UseUnderlineColor)
				return;
			DocCommandUnderlineColor command = new DocCommandUnderlineColor();
			command.Color = CharacterProperties.Info.UnderlineColor;
			command.Write(writer);
		}
		public void UnderlineWordsOnlyAction() {
			if (!CharacterProperties.UseUnderlineWordsOnly || !CharacterProperties.UseFontUnderlineType || CharacterProperties.FontUnderlineType != UnderlineType.Single)
				return;
			UnderlineAction();
		}
		void UnderlineAction() {
			DocCommandUnderline command = new DocCommandUnderline();
			command.FontUnderlineType = CharacterProperties.Info.FontUnderlineType;
			command.UnderlyneWordsOnly = CharacterProperties.Info.UnderlineWordsOnly;
			command.Write(writer);
		}
		void WriteCharacterStyleIndex(int characterStyleIndex) {
			DocCommandChangeCharacterStyle command = new DocCommandChangeCharacterStyle();
			command.Value = characterStyleIndex;
			command.Write(writer);
		}
		#endregion
		void PictureBulletInformationAction() {
			if (this.pictureBulletInformation == null)
				return;
			DocCommandPictureBulletCharacterPosition characterPositionCommand = new DocCommandPictureBulletCharacterPosition();
			characterPositionCommand.Value = this.pictureBulletInformation.PictureCharacterPosition;
			characterPositionCommand.Write(this.writer);
			DocCommandPictureBulletProperties bulletProperties = new DocCommandPictureBulletProperties();
			bulletProperties.DefaultPicture = this.pictureBulletInformation.DefaultPicture;
			bulletProperties.PictureBullet = this.pictureBulletInformation.PictureBullet;
			bulletProperties.SuppressBulletResize = this.pictureBulletInformation.SuppressBulletResize;
			bulletProperties.Write(this.writer);
		}
		void PictureOffsetAction(int dataStreamOffset) {
			DocCommandPictureOffset pictureOffsetCommand = new DocCommandPictureOffset();
			pictureOffsetCommand.Value = dataStreamOffset;
			pictureOffsetCommand.Write(this.writer);
		}
		void SpecialSymbolAction() {
			DocCommandSpecial command = new DocCommandSpecial();
			command.Value = true;
			command.Write(writer);
		}
		#region IDisposable Members
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				IDisposable resource = writer;
				if (resource != null) {
					resource.Dispose();
					resource = null;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
	}
	public class DocListCharacterPropertiesActions : DocCharacterPropertiesActions {
		public DocListCharacterPropertiesActions(MemoryStream output, ListLevel listLevel, int fontNameIndex)
			: base(output, fontNameIndex) {
			CharacterProperties = listLevel.CharacterProperties;
			PictureBulletInformation = new DocPictureBulletInformation();
			PictureBulletInformation.SuppressBulletResize = listLevel.ListLevelProperties.SuppressBulletResize;
		}
		protected override void SetUseDefaultFontSize(int characterStyleIndex, int paragraphStyleIndex) {
		}
	}
}
