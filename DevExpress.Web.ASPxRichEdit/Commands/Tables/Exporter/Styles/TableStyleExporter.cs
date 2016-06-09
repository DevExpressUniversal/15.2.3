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
namespace DevExpress.Web.ASPxRichEdit.Export {
	public enum JSONTableStyleProperty {
		Name = 0,
		Deleted = 1,
		Hidden = 2,
		Semihidden = 3,
		IsDefault = 4,
		ParentName = 5,
		BaseConditionalStyle = 6,
		ConditionalStyles = 7,
		LocalizedName = 8
	}
	public static class TableStyleExporter {
		public static Hashtable ToHashtable(TableStyle from, WebTablePropertiesCache tablePropertiesCache, WebTableRowPropertiesCache tableRowProperiesCache, WebTableCellPropertiesCache tableCellPropertiesCache, CharacterFormattingBaseExporter characterExporter, ParagraphFormattingBaseExporter paragraphExporter) {
			Hashtable result = new Hashtable();
			FillHashtable(result, from, tablePropertiesCache, tableRowProperiesCache, tableCellPropertiesCache, characterExporter, paragraphExporter);
			return result;
		}
		static void FillHashtable(Hashtable result, TableStyle from, WebTablePropertiesCache tablePropertiesCache, WebTableRowPropertiesCache tableRowProperiesCache, WebTableCellPropertiesCache tableCellPropertiesCache, CharacterFormattingBaseExporter characterExporter, ParagraphFormattingBaseExporter paragraphExporter) {
			result.Add((int)JSONTableStyleProperty.Name, from.StyleName);
			result.Add((int)JSONTableStyleProperty.Deleted, from.Deleted);
			result.Add((int)JSONTableStyleProperty.Hidden, from.Hidden);
			result.Add((int)JSONTableStyleProperty.Semihidden, from.Semihidden);
			result.Add((int)JSONTableStyleProperty.IsDefault, from.StyleName == "TableGrid");
			result.Add((int)JSONTableStyleProperty.LocalizedName, from.LocalizedStyleName);
			result.Add((int)JSONTableStyleProperty.ParentName, from.Parent == null ? null : from.Parent.StyleName);
			result.Add((int)JSONTableStyleProperty.BaseConditionalStyle, TableBaseConditionalStyleExporter.ToHashtable(from, tablePropertiesCache, tableRowProperiesCache, tableCellPropertiesCache, characterExporter, paragraphExporter));
			result.Add((int)JSONTableStyleProperty.ConditionalStyles, TableBaseConditionalStylesExporter.ToHashtable(from.ConditionalStyleProperties, tablePropertiesCache, tableRowProperiesCache, tableCellPropertiesCache, characterExporter, paragraphExporter));
		}
	}
}
