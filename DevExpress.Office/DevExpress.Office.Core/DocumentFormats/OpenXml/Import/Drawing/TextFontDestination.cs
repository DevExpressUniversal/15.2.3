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
using DevExpress.Office.Utils;
using DevExpress.Office;
using DevExpress.Office.Drawing;
namespace DevExpress.Office.Import.OpenXml {
	public class DrawingTextFontDestination : LeafElementDestination<DestinationAndXmlBasedImporter> {
		readonly DrawingTextFont textFont;
		public DrawingTextFontDestination(DestinationAndXmlBasedImporter importer, DrawingTextFont textFont)
			: base(importer) {
			Guard.ArgumentNotNull(textFont, "textFont");
			this.textFont = textFont;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Importer.DocumentModel.BeginUpdate();
			string typeface = Importer.ReadAttribute(reader, "typeface");
			if (!String.IsNullOrEmpty(typeface))
				textFont.Typeface = typeface;
			string panose = Importer.ReadAttribute(reader, "panose");
			if (!String.IsNullOrEmpty(panose) && panose.Length != 20)
				Importer.ThrowInvalidFile();
			if (!String.IsNullOrEmpty(panose))
				textFont.Panose = panose;
			int pitchFamily = Importer.GetIntegerValue(reader, "pitchFamily", DrawingTextFont.DefaultPitchFamily);
			if (Math.Abs(pitchFamily) > byte.MaxValue)
				Importer.ThrowInvalidFile();
			textFont.PitchFamily = (byte)pitchFamily;
			int charset = Importer.GetIntegerValue(reader, "charset", DrawingTextFont.DefaultCharset);
			if (Math.Abs(charset) > byte.MaxValue)
				Importer.ThrowInvalidFile();
			textFont.Charset = (byte)charset;
		}
		public override void ProcessElementClose(XmlReader reader) {
			Importer.DocumentModel.EndUpdate();
		}
	}
}
