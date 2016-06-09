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
using System.IO;
using System.Globalization;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Import.Rtf {
	#region InfoDestination
	public class InfoDestination : DestinationBase {
		static KeywordTranslatorTable keywordHT = CreateKeywordTable();
		static KeywordTranslatorTable CreateKeywordTable() {
			KeywordTranslatorTable table = new KeywordTranslatorTable();
			table.Add("password", OnLegacyPasswordHash);
			table.Add("passwordhash", OnPasswordHash);
			return table;
		}
		public InfoDestination(RtfImporter importer)
			: base(importer) {
		}
		protected override KeywordTranslatorTable KeywordHT { get { return keywordHT; } }
		protected override ControlCharTranslatorTable ControlCharHT { get { return null; } }
		protected override DestinationBase CreateClone() {
			return new InfoDestination(Importer);
		}
		static void OnLegacyPasswordHash(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new LegacyPasswordHashDestination(importer);
		}
		static void OnPasswordHash(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new PasswordHashDestination(importer);
		}
	}
	#endregion
	#region LegacyPasswordHashDestination
	public class LegacyPasswordHashDestination : HexStreamDestination {
		public LegacyPasswordHashDestination(RtfImporter rtfImporter)
			: base(rtfImporter) {
		}
		protected internal override HexStreamDestination CreateEmptyClone() {
			return new LegacyPasswordHashDestination(Importer);
		}
		public override void AfterPopRtfState() {
			try {
				Value.Seek(0, SeekOrigin.Begin);
				ReadPasswordHash();
			}
			catch {
			}
		}
		protected internal virtual void ReadPasswordHash() {
			byte[] bytes = new byte[4];
			for (int i = 3; i >= 0; i--)
				bytes[i] = (byte)Value.ReadByte();
			Importer.DocumentModel.ProtectionProperties.Word2003PasswordHash = bytes;
		}
	}
	#endregion
	#region PasswordHashDestination
	public class PasswordHashDestination : HexStreamDestination {
		public PasswordHashDestination(RtfImporter rtfImporter)
			: base(rtfImporter) {
		}
		protected internal override HexStreamDestination CreateEmptyClone() {
			return new PasswordHashDestination(Importer);
		}
		public override void AfterPopRtfState() {
			try {
				Value.Seek(0, SeekOrigin.Begin);
				ReadPasswordHash();
			}
			catch {
			}
		}
		protected internal virtual void ReadPasswordHash() {
			DocumentProtectionProperties properties = Importer.DocumentModel.ProtectionProperties;
			using (BinaryReader reader = new BinaryReader(Value)) {
				int value = reader.ReadInt32();
				reader.ReadInt32();
				reader.ReadInt32();
				value = reader.ReadInt32();
				properties.HashAlgorithmType = (HashAlgorithmType)(value - 0x8000);
				properties.HashIterationCount = reader.ReadInt32();
				int passwordHashLength = reader.ReadInt32();
				int passwordPrefixLength = reader.ReadInt32();
				reader.ReadInt32();
				reader.ReadInt32();
				reader.ReadInt32();
				properties.PasswordHash = reader.ReadBytes(passwordHashLength);
				properties.PasswordPrefix = reader.ReadBytes(passwordPrefixLength);
			}
		}
	}
	#endregion
}
