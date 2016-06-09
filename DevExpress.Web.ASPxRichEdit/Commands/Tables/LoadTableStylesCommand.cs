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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public class LoadTableStylesCommand : WebRichEditLoadModelCommandBase {
		public LoadTableStylesCommand(WebRichEditLoadModelCommandBase parentCommand, Hashtable parameters)
			: base(parentCommand, parameters) { }
		public override CommandType Type { get { return CommandType.Styles; } }
		protected override bool IsEnabled() { return true; }
		protected override void FillHashtable(Hashtable result) {
			List<Hashtable> tableStylesList = new List<Hashtable>();
			var tablePropertiesCache = new WebTablePropertiesCache();
			var tableRowPropertiesCache = new WebTableRowPropertiesCache();
			var tableCellPropertiesCache = new WebTableCellPropertiesCache();
			CharacterFormattingBaseExporter characterExporter = new CharacterFormattingBaseExporter(DocumentModel, WorkSession.FontInfoCache);
			ParagraphFormattingBaseExporter paragraphExporter = new ParagraphFormattingBaseExporter(DocumentModel);
			foreach (var modelStyle in DocumentModel.TableStyles)
				tableStylesList.Add(TableStyleExporter.ToHashtable(modelStyle, tablePropertiesCache, tableRowPropertiesCache, tableCellPropertiesCache, characterExporter, paragraphExporter));
			result.Add("tableStyles", tableStylesList);
			result.Add("characterPropertiesCache", characterExporter.Cache);
			result.Add("paragraphPropertiesCache", paragraphExporter.Cache);
			result.Add("tablePropertiesCache", tablePropertiesCache.ToHashtable());
			result.Add("tableRowPropertiesCache", tableRowPropertiesCache.ToHashtable());
			result.Add("tableCellPropertiesCache", tableCellPropertiesCache.ToHashtable());
		}
	}
}
