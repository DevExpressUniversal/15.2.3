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
using System.Xml;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Office;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	public class TextDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		ISharedStringItem sharedStringItem;
		public TextDestination(SpreadsheetMLBaseImporter importer, ISharedStringItem sharedStringItem)
			: base(importer) {
			Guard.ArgumentNotNull(sharedStringItem, "sharedStringItem");
			this.sharedStringItem = sharedStringItem;
		}
		public ISharedStringItem SharedStringItem { get { return sharedStringItem; } }
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
		}
		public override bool ProcessText(XmlReader reader) {
			string text = reader.Value;
			if (!String.IsNullOrEmpty(text))
				sharedStringItem.Content = Importer.DecodeXmlChars(text);
			return true;
		}
		static char[] trimChars = new char[] { ' ', '\r', '\n', '\t'};
		protected internal virtual string RemoveRedundantSpaces(string text) {
			text = text.Trim(trimChars);
			text = text.Replace('\n', ' ');
			int length = text.Length;
			for (; ; ) {
				text = text.Replace("  ", " ");
				if (text.Length == length)
					break;
				length = text.Length;
			}
			return text;
		}
	}
}
