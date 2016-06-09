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

using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Model;
using System.Collections;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public class LoadControlOptionsCommand : WebRichEditLoadModelCommandBase {
		private ASPxRichEditSettings settings;
		public LoadControlOptionsCommand(WebRichEditLoadModelCommandBase parentCommand, Hashtable parameters, ASPxRichEditSettings settings)
			: base(parentCommand, parameters) {
			this.settings = settings;
		}
		public override CommandType Type { get { return CommandType.ControlOptions; } }
		protected override bool IsEnabled() { return true; }
		protected override void FillHashtable(Hashtable result) {
			RegisterDocumentCapability(result, "copy", settings.Behavior.Copy);
			RegisterDocumentCapability(result, "createNew", settings.Behavior.CreateNew);
			RegisterDocumentCapability(result, "cut", settings.Behavior.Cut);
			RegisterDocumentCapability(result, "drag", settings.Behavior.Drag);
			RegisterDocumentCapability(result, "drop", settings.Behavior.Drop);
			RegisterDocumentCapability(result, "open", settings.Behavior.Open);
			RegisterDocumentCapability(result, "paste", settings.Behavior.Paste);
			RegisterDocumentCapability(result, "printing", settings.Behavior.Printing);
			RegisterDocumentCapability(result, "save", settings.Behavior.Save);
			RegisterDocumentCapability(result, "saveAs", settings.Behavior.SaveAs);
			RegisterDocumentCapability(result, "fullScreen", settings.Behavior.FullScreen);
			RegisterDocumentCapability(result, "bookmarks", settings.DocumentCapabilities.Bookmarks);
			RegisterDocumentCapability(result, "characterFormatting", settings.DocumentCapabilities.CharacterFormatting);
			RegisterDocumentCapability(result, "characterStyle", settings.DocumentCapabilities.CharacterStyle);
			RegisterDocumentCapability(result, "fields", settings.DocumentCapabilities.Fields);
			RegisterDocumentCapability(result, "hyperlinks", settings.DocumentCapabilities.Hyperlinks);
			RegisterDocumentCapability(result, "inlinePictures", settings.DocumentCapabilities.InlinePictures);
			RegisterDocumentCapability(result, "paragraphFormatting", settings.DocumentCapabilities.ParagraphFormatting);
			RegisterDocumentCapability(result, "paragraphs", settings.DocumentCapabilities.Paragraphs);
			RegisterDocumentCapability(result, "paragraphStyle", settings.DocumentCapabilities.ParagraphStyle);
			RegisterDocumentCapability(result, "paragraphTabs", settings.DocumentCapabilities.ParagraphTabs);
			RegisterDocumentCapability(result, "sections", settings.DocumentCapabilities.Sections);
			RegisterDocumentCapability(result, "tabSymbol", settings.DocumentCapabilities.TabSymbol);
			RegisterDocumentCapability(result, "undo", settings.DocumentCapabilities.Undo);
			RegisterDocumentCapability(result, "numberingBulleted", settings.DocumentCapabilities.Numbering.Bulleted);
			RegisterDocumentCapability(result, "numberingMultiLevel", settings.DocumentCapabilities.Numbering.MultiLevel);
			RegisterDocumentCapability(result, "numberingSimple", settings.DocumentCapabilities.Numbering.Simple);
			RegisterDocumentCapability(result, "headersFooters", settings.DocumentCapabilities.HeadersFooters);
			RegisterDocumentCapability(result, "tables", settings.DocumentCapabilities.Tables);
			RegisterDocumentCapability(result, "tableStyle", settings.DocumentCapabilities.TableStyle);
			if (settings.Behavior.TabMarker != "\t")
				result["tabMarker"] = settings.Behavior.TabMarker;
			if(settings.Behavior.PageBreakInsertMode != XtraRichEdit.PageBreakInsertMode.NewLine)
				result["pageBreakInsertMode"] = (int)settings.Behavior.PageBreakInsertMode;
		}
		protected void RegisterDocumentCapability(Hashtable result, string name, DocumentCapability capability) {
			if(capability != DocumentCapability.Default)
				result[name] = (int)capability;
		}
	}
}
