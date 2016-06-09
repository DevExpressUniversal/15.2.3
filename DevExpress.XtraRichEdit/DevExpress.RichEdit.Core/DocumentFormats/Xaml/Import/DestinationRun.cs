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
using DevExpress.XtraRichEdit.Internal;
namespace DevExpress.XtraRichEdit.Import.Xaml {
	#region RunDestination
	public class RunDestination : InlineLeafElementDestination {
		string text;
		public RunDestination(XamlImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			this.text = reader.GetAttribute("Text");
		}
		public override bool ProcessText(XmlReader reader) {
			this.text = reader.Value;
			if (reader.XmlSpace != XmlSpace.Preserve)
				this.text = RemoveRedundantSpaces(text);
			return true;
		}
		public override void ProcessElementClose(XmlReader reader) {
			ImportText(text);
			base.ProcessElementClose(reader);
		}
		protected internal virtual void ImportText(string text) {
			if (!String.IsNullOrEmpty(text))
				Importer.PieceTable.InsertTextCore(Importer.Position, text);
		}
		protected internal virtual string RemoveRedundantSpaces(string text) {
			text = text.Trim();
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
	#endregion
}
