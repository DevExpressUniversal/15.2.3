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

using System.Collections.Generic;
using System.IO;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Office.Utils;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Import.Doc {
	#region StyleSheet
	public class DocStyleSheet {
		public static DocStyleSheet FromStream(BinaryReader reader, int offset, int size) {
			DocStyleSheet result = new DocStyleSheet();
			result.Read(reader, offset, size);
			return result;
		}
		public static DocStyleSheet CreateDefault() {
			DocStyleSheet result = new DocStyleSheet();
			result.styleSheetInformation = StyleSheetInformation.CreateDefault();
			return result;
		}
		public const int ExtendedStyleDescriptionBaseInFile = 0x0012;
		#region Fields
		StyleSheetInformation styleSheetInformation;
		StyleDescriptionCollection styles;
		#endregion
		public DocStyleSheet() {
			this.styles = new StyleDescriptionCollection();
		}
		#region Properties
		public StyleSheetInformation StylesInformation {
			get { return styleSheetInformation; }
			protected internal set { styleSheetInformation = value; }
		}
		public StyleDescriptionCollection Styles { get { return styles; } }
		#endregion
		protected void Read(BinaryReader reader, int offset, int size) {
			Guard.ArgumentNotNull(reader, "reader");
			int styleSheetInformationOffset = offset + 2;
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);
			short styleSheetInformationSize = reader.ReadInt16();
			this.styleSheetInformation = StyleSheetInformation.FromStream(reader, styleSheetInformationOffset);
			reader.BaseStream.Seek(styleSheetInformationOffset + styleSheetInformationSize, SeekOrigin.Begin);
			styles.AddRange(GetStyles(reader, offset, size));
		}
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			writer.Write((ushort)0x0182);
			StylesInformation.Write(writer);
			int count = Styles.Count;
			for (int i = 0; i < count; i++) {
				if (Styles[i] != null)
					Styles[i].Write(writer, StylesInformation.ContainsStdfPost2000);
				else
					writer.Write((short)0);
			}
		}
		StyleDescriptionCollection GetStyles(BinaryReader reader, int offset, int size) {
			StyleDescriptionCollection styles = new StyleDescriptionCollection();
			while (reader.BaseStream.Position - offset < size) {
				short styleDescriptionLength = reader.ReadInt16();
				if (styleDescriptionLength > 0) {
					long currentPosition = reader.BaseStream.Position;
					StyleDescriptionBase styleDescription = StyleDescriptionFactory.CreateStyleDescription(reader, GetStyleType(reader), StylesInformation);
					styleDescription.StyleIndex = styles.Count;
					styles.Add(styleDescription);
					reader.BaseStream.Seek(currentPosition + styleDescriptionLength, SeekOrigin.Begin);
				}
				else {
					styles.Add(null);
				}
			}
			return styles;
		}
		StyleType GetStyleType(BinaryReader reader) {
			reader.BaseStream.Seek(2, SeekOrigin.Current);
			StyleType currentType = (StyleType)((reader.ReadInt16() & 0x000f) - 1); 
			reader.BaseStream.Seek(-4, SeekOrigin.Current);
			return currentType;
		}
	}
	#endregion
	#region StyleSheetInformation
	public class StyleSheetInformation {
		const int defaultFixedIndexStylesCount = 0x0f;
		public static StyleSheetInformation FromStream(BinaryReader reader, int offset) {
			StyleSheetInformation result = new StyleSheetInformation();
			result.Read(reader, offset);
			return result;
		}
		public static StyleSheetInformation CreateDefault() {
			StyleSheetInformation result = new StyleSheetInformation();
			result.FixedIndexStylesCount = defaultFixedIndexStylesCount;
			return result;
		}
		#region Fields
		const short ftcBi = 0x0000;
		const short latentStyleDescriptionSize = 0x0004;
		const uint stiNormalLatentStyleInformation = 0x00000008;
		const uint stiHeading1LatentStyleInformation = 0x00000098;
		const uint stiHeadingsDefaultLatentStyleInformation = 0x00000636;
		const uint stiTocLatenStyleInformation = 0x00000276;
		const uint stiListBulletLatentStyleInformation = 0x00000008;
		const uint stiListBullet2LatentStyleInformation = 0x00000016;
		const uint stiSubtitlelLatentStyleInformation = 0x000000b8;
		const uint stiStrongLatentStyleInformation = 0x00000168;
		const uint stiEmphasisLatentLatentStyleInformation = 0x00000148;
		const int defaultFlags = 0x0001;
		const int maxStyleIdentifierWhenSaved = 0x005b;
		const int builtInStyleNamesVersionWhenSaved = 0x0000;
		const int withoutStdfPost2000 = 0x000a;
		const int withStdfPost2000 = 0x0012;
		short stylesCount;
		short baseStyleDescriptionSize;
		short flags;
		short maxStyleIdentifier; 
		short fixedIndexStylesCount; 
		short builtInStyleNamesVersion;
		short defaultASCIIFont; 
		short defaultEastAsianFont; 
		short defaultOthersFont;
		#endregion
		public StyleSheetInformation() {
			this.baseStyleDescriptionSize = DocStyleSheet.ExtendedStyleDescriptionBaseInFile;
			this.flags = defaultFlags;
			this.maxStyleIdentifier = maxStyleIdentifierWhenSaved;
			this.builtInStyleNamesVersion = builtInStyleNamesVersionWhenSaved;
			this.defaultASCIIFont = 0x0000;
			this.defaultEastAsianFont = 0x0000;
			this.defaultOthersFont = 0x0000;
		}
		#region Properties
		public short StylesCount { get { return stylesCount; } protected internal set { stylesCount = value; } }
		public short BaseStyleDescriptionSize { get { return baseStyleDescriptionSize; } }
		public bool IsBuiltInStyleNamesStored { get { return (flags & 0x01) == 1; } }
		public short MaxStyleIdentifier { get { return maxStyleIdentifier; } }
		public short FixedIndexStylesCount {
			get { return this.fixedIndexStylesCount; }
			protected internal set { fixedIndexStylesCount = value; }
		}
		public short BuiltInStyleNameVersion { get { return builtInStyleNamesVersion; } }
		public short DefaultASCIIFont { get { return defaultASCIIFont; } protected internal set { defaultASCIIFont = value; } }
		public short DefaultEastAsianFont { get { return defaultEastAsianFont; } }
		public short DefaultOthersFont { get { return defaultOthersFont; } }
		public bool ContainsStdfPost2000 { get { return baseStyleDescriptionSize == withStdfPost2000; } }
		#endregion
		protected void Read(BinaryReader reader, int offset) {
			Guard.ArgumentNotNull(reader, "reader");
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);
			this.stylesCount = (short)reader.ReadUInt16();
			this.baseStyleDescriptionSize = (short)reader.ReadUInt16();
#if DEBUGTEST
			Debug.Assert(this.baseStyleDescriptionSize == withoutStdfPost2000 || this.baseStyleDescriptionSize == withStdfPost2000);
#endif
			this.flags = reader.ReadInt16();
			this.maxStyleIdentifier = (short)reader.ReadUInt16();
			this.fixedIndexStylesCount = (short)reader.ReadUInt16();
			this.builtInStyleNamesVersion = (short)reader.ReadUInt16();
			this.defaultASCIIFont = (short)reader.ReadUInt16();
			this.defaultEastAsianFont = (short)reader.ReadUInt16();
			this.defaultOthersFont = (short)reader.ReadUInt16();
		}
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			writer.Write(this.stylesCount);
			writer.Write(this.baseStyleDescriptionSize);
			writer.Write(this.flags);
			writer.Write(this.maxStyleIdentifier);
			writer.Write(this.fixedIndexStylesCount);
			writer.Write(this.builtInStyleNamesVersion);
			writer.Write(this.defaultASCIIFont);
			writer.Write(this.defaultEastAsianFont);
			writer.Write(this.defaultOthersFont);
			writer.Write(ftcBi);
			writer.Write(latentStyleDescriptionSize);
			writer.Write(stiNormalLatentStyleInformation);
			writer.Write(stiHeading1LatentStyleInformation);
			for (int i = 0; i < 8; i++)
				writer.Write((uint)0x0000009e);
			for (int i = 0; i < 9; i++)
				writer.Write(stiHeadingsDefaultLatentStyleInformation);
			for (int i = 0; i < 9; i++)
				writer.Write(stiTocLatenStyleInformation);
			for (int i = 0; i < 34; i++)
				writer.Write(stiHeading1LatentStyleInformation);
			writer.Write(stiListBulletLatentStyleInformation);
			writer.Write(stiHeadingsDefaultLatentStyleInformation);
			writer.Write(stiHeadingsDefaultLatentStyleInformation);
			writer.Write(stiListBullet2LatentStyleInformation);
			for (int i = 0; i < 8; i++)
				writer.Write(stiHeadingsDefaultLatentStyleInformation);
			writer.Write(stiSubtitlelLatentStyleInformation);
			for (int i = 0; i < 12; i++)
				writer.Write(stiHeadingsDefaultLatentStyleInformation);
			writer.Write(stiStrongLatentStyleInformation);
			writer.Write(stiEmphasisLatentLatentStyleInformation);
			writer.Write(stiHeadingsDefaultLatentStyleInformation);
			writer.Write(stiHeadingsDefaultLatentStyleInformation);
		}
	}
	#endregion
}
