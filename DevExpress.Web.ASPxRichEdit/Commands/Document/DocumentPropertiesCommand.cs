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

using DevExpress.Web.ASPxRichEdit.Export;
using DevExpress.XtraRichEdit.Model;
using System.Collections;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public class LoadDocumentPropertiesCommand : WebRichEditLoadModelCommandBase {
		public LoadDocumentPropertiesCommand(WebRichEditLoadModelCommandBase parentCommand, DocumentModel documentModel)
			: base(parentCommand, new Hashtable(), documentModel) { }
		public override CommandType Type { get { return CommandType.DocumentProperties; } }
		protected override bool IsEnabled() { return true; }
		protected override void FillHashtable(Hashtable result) {
			result["defaultTabWidth"] = DocumentModel.DocumentProperties.DefaultTabWidth;
			result["differentOddAndEvenPages"] = DocumentModel.DocumentProperties.DifferentOddAndEvenPages;
			result["displayBackgroundShape"] = DocumentModel.DocumentProperties.DisplayBackgroundShape;
			result["pageBackColor"] = DocumentModel.DocumentProperties.PageBackColor.ToArgb();
			result["defaultCharacterProperties"] = GetDefaultCharacterProperties();
			result["defaultParagraphProperties"] = GetDefaultParagraphProperties();
			result["defaultTableProperties"] = new WebTableProperties(DocumentModel.DefaultTableProperties).ToHashtable();
			result["defaultTableRowProperties"] = new WebTableRowProperties(DocumentModel.DefaultTableRowProperties).ToHashtable();
			result["defaultTableCellProperties"] = new WebTableCellProperties(DocumentModel.DefaultTableCellProperties).ToHashtable();
		}
		Hashtable GetDefaultCharacterProperties() {
			return new CharacterFormattingBaseExporter(DocumentModel, WorkSession.FontInfoCache).GetHashtable(DocumentModel.DefaultCharacterProperties.Info);
		}
		Hashtable GetDefaultParagraphProperties() {
			return new ParagraphFormattingBaseExporter(DocumentModel).GetHashtable(DocumentModel.DefaultParagraphProperties.Info);
		}
	}
	public class ChangeDefaultTabWidthCommand : WebRichEditUpdateModelCommandBase {
		public ChangeDefaultTabWidthCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.ChangeDefaultTabWidth; } }
		protected override bool IsEnabled() { return true; }
		protected override void PerformModifyModel() {
			int defaultTabWidth = (int)Parameters["defaultTabWidth"];
			DocumentModel.DocumentProperties.DefaultTabWidth = defaultTabWidth;
		}
	}
	public class ChangePageColorCommand : WebRichEditUpdateModelCommandBase {
		public ChangePageColorCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.ChangePageColor; } }
		protected override bool IsEnabled() { return true; }
		protected override void PerformModifyModel() {
			int color = (int)Parameters["pageColor"];
			DocumentModel.DocumentProperties.DisplayBackgroundShape = true;
			DocumentModel.DocumentProperties.PageBackColor = PropertiesHelper.GetColorFromArgb(color);
		}
	}
	public class ChangeDifferentOddAndEvenPagesCommand : WebRichEditUpdateModelCommandBase {
		public ChangeDifferentOddAndEvenPagesCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.ChangeDifferentOddAndEvenPages; } }
		protected override bool IsEnabled() { return true; }
		protected override void PerformModifyModel() {
			bool newValue = (bool)Parameters["newValue"];
			DocumentModel.DocumentProperties.DifferentOddAndEvenPages = newValue;
		}
	}
}
