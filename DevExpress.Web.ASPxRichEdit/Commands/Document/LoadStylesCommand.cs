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
using System;
using System.Collections;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Localization;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public class LoadStylesCommand : WebRichEditLoadModelCommandBase {
		public LoadStylesCommand(WebRichEditLoadModelCommandBase parentCommand, DocumentModel documentModel)
			: base(parentCommand, new Hashtable(), documentModel) { }
		public override CommandType Type { get { return CommandType.Styles; } }
		protected override bool IsEnabled() { return true; }
		protected override void FillHashtable(Hashtable result) {
			LoadCharacterStylesCommand characterStylesCommand = new LoadCharacterStylesCommand(this, new Hashtable());
			result.Add("characterStyles", characterStylesCommand.Execute());
			LoadParagraphStylesCommand paragraphStylecCommand = new LoadParagraphStylesCommand(this, new Hashtable());
			result.Add("paragraphStyles", paragraphStylecCommand.Execute());
			LoadTableStylesCommand tableStylesCommand = new LoadTableStylesCommand(this, new Hashtable());
			result.Add("tableStyles", tableStylesCommand.Execute());
			LoadTableCellStylesCommand tableCellStylesCommand = new LoadTableCellStylesCommand(this, new Hashtable());
			result.Add("tableCellStyles", tableCellStylesCommand.Execute());
			LoadNumberingListStylesCommand numberingListStylesCommand = new LoadNumberingListStylesCommand(this, new Hashtable());
			result.Add("numberingListStyles", numberingListStylesCommand.Execute());
		}
	}
	public class LoadCharacterStylesCommand : WebRichEditLoadModelCommandBase {
		public LoadCharacterStylesCommand(WebRichEditLoadModelCommandBase parentCommand, Hashtable parameters) 
			: base(parentCommand, parameters) { }
		public override CommandType Type { get { return CommandType.Styles; } }
		protected override bool IsEnabled() { return true; }
		protected override void FillHashtable(Hashtable result) {
			WebCharacterStyleCollection webStyles = new WebCharacterStyleCollection();
			CharacterFormattingBaseExporter propertiesExporter = new CharacterFormattingBaseExporter(DocumentModel, WorkSession.FontInfoCache);
			foreach (var modelStyle in DocumentModel.CharacterStyles) {
				WebCharacterStyle style = new WebCharacterStyle(modelStyle);
				webStyles.Add(style);
				propertiesExporter.RegisterItem(style.CharacterPropertiesCacheIndex, modelStyle.CharacterProperties.Info);
			}
			result.Add("characterStyles", webStyles.ToHashtableCollection());
			result.Add("characterPropertiesCache", propertiesExporter.Cache);
		}
	}
	public class LoadNumberingListStylesCommand : WebRichEditLoadModelCommandBase {
		public LoadNumberingListStylesCommand(WebRichEditLoadModelCommandBase parentCommand, Hashtable parameters) 
			: base(parentCommand, parameters) { }
		public override CommandType Type { get { return CommandType.Styles; } }
		protected override bool IsEnabled() { return true; }
		protected override void FillHashtable(Hashtable result) {
			WebNumberingListStyleCollection numberingListStyles = new WebNumberingListStyleCollection();
			foreach (var modelStyle in DocumentModel.NumberingListStyles) {
				WebNumberingListStyle style = new WebNumberingListStyle(modelStyle);
				numberingListStyles.Add(style);
			}
			result.Add("numberingListStyles", numberingListStyles.ToHashtableCollection());
		}
	}
}
