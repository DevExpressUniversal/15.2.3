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

using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public class LoadDocumentCommand : WebRichEditLoadModelCommandBase {
		public LoadDocumentCommand(WebRichEditLoadModelCommandBase parentCommand, DocumentModel documentModel)
			: this(parentCommand, documentModel, false) { }
		public LoadDocumentCommand(WebRichEditLoadModelCommandBase parentCommand, DocumentModel documentModel, bool loadWholeContent)
			: base(parentCommand, new Hashtable(), documentModel) {
			LoadWholeContent = loadWholeContent;
		}
		protected bool LoadWholeContent { get; private set; }
		protected override void FillHashtable(Hashtable result) {
			result["documentProperties"] = new LoadDocumentPropertiesCommand(this, DocumentModel).Execute();
			result["styles"] = new LoadStylesCommand(this, DocumentModel).Execute();
			result["sections"] = new LoadSectionsCommand(this, DocumentModel).Execute();
			var listsCommand = new NumberingListCommand(this, DocumentModel);
			listsCommand.SkipTemplates = LoadWholeContent;
			result["lists"] = listsCommand.Execute();
			result["headers"] = new LoadHeadersCommand(this, DocumentModel).Execute();
			result["footers"] = new LoadFootersCommand(this, DocumentModel).Execute();
			if(LoadWholeContent) {
				int docLength = DocumentModel.MainPieceTable.DocumentEndLogPosition - DocumentLogPosition.Zero + 1;
				result["mainSubDocument"] = new LoadPieceTableCommand(this, DocumentModel.MainPieceTable, docLength).Execute();
			}
			else
				result["mainSubDocument"] = new LoadPieceTableCommand(this, DocumentModel.MainPieceTable).Execute();
			result["countIdForPeaceTables"] = DocumentModel.CountIdForPeaceTables;
		}
		public override CommandType Type { get { return CommandType.LoadDocument; } }
		protected override bool IsEnabled() { return true; }
	}
}
