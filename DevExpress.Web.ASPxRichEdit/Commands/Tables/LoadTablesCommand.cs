﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
	public class LoadTablesCommand : WebRichEditLoadPieceTableCommandBase {
		public LoadTablesCommand(WebRichEditLoadModelCommandBase parentCommand, PieceTable pieceTable)
			: base(parentCommand, new Hashtable(), pieceTable) { }
		public override CommandType Type { get { return CommandType.FieldLists; } }
		protected override bool IsEnabled() { return true; }
		protected override void FillHashtable(Hashtable result) {
			var tablePropertiesCache = new WebTablePropertiesCache();
			var tableCellPropertiesCache = new WebTableCellPropertiesCache();
			var tableRowPropertiesCache = new WebTableRowPropertiesCache();
			List<Hashtable> tableList = new List<Hashtable>();
			foreach (var table in PieceTable.Tables.OrderBy(t => t.NestedLevel))
				tableList.Add(new WebTable(table, tablePropertiesCache, tableRowPropertiesCache, tableCellPropertiesCache).ToHashtable());
			result.Add("tables", tableList);
			result.Add("tablePropertiesCache", tablePropertiesCache.ToHashtable());
			result.Add("tableRowPropertiesCache", tableRowPropertiesCache.ToHashtable());
			result.Add("tableCellPropertiesCache", tableCellPropertiesCache.ToHashtable());
		}
	}
}
