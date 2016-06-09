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
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Import.Rtf {
	#region StringValueDestination (abstract class)
	public abstract class StringValueDestination : DestinationBase {
		string value = String.Empty;
		static ControlCharTranslatorTable controlCharHT = CreateControlCharTable();
		static KeywordTranslatorTable keywordHT = CreateKeywordTable();
		static ControlCharTranslatorTable CreateControlCharTable() {
			ControlCharTranslatorTable table = new ControlCharTranslatorTable();
			table.Add('\'', OnSwitchToHexChar);
			table.Add('\\', OnEscapedChar);
			return table;
		}
		internal static KeywordTranslatorTable CreateKeywordTable() {
			KeywordTranslatorTable table = new KeywordTranslatorTable();
			table.Add("u", OnUnicodeKeyword);
			return table;
		}
		protected override ControlCharTranslatorTable ControlCharHT { get { return controlCharHT; } }
		protected override KeywordTranslatorTable KeywordHT { get { return keywordHT; } }
		public virtual string Value {
			get {
				return value.TrimEnd(';');				
			}
		}
		protected StringValueDestination(RtfImporter rtfImporter)
			: base(rtfImporter) {
		}
		protected override void ProcessCharCore(char ch) {
			value += ch;
		}
		protected override DestinationBase CreateClone() {
			StringValueDestination clone = CreateEmptyClone();
			clone.value = value;
			return clone;
		}
		protected internal abstract StringValueDestination CreateEmptyClone();
	}
	#endregion
}
