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
	public enum JSONTableConditionalStyleProperty {
		TablePropertiesIndex = 0,
		TableRowPropertiesIndex = 1,
		TableCellPropertiesIndex = 2,
		MaskedParagraphPropertiesCacheIndex = 3,
		MaskedCharacterPropertiesCacheIndex = 4,
		Tabs = 5
	}
	public static class TableBaseConditionalStyleExporter {
		public static Hashtable ToHashtable(TableStyle from, WebTablePropertiesCache tablePropertiesCache, WebTableRowPropertiesCache tableRowProperiesCache, WebTableCellPropertiesCache tableCellPropertiesCache, CharacterFormattingBaseExporter characterExporter, ParagraphFormattingBaseExporter paragraphExporter) {
			Hashtable result = new Hashtable();
			result.Add((int)JSONTableConditionalStyleProperty.TablePropertiesIndex, tablePropertiesCache.AddItem(new WebTableProperties(from.TableProperties)));
			result.Add((int)JSONTableConditionalStyleProperty.TableRowPropertiesIndex, tableRowProperiesCache.AddItem(new WebTableRowProperties(from.TableRowProperties)));
			result.Add((int)JSONTableConditionalStyleProperty.TableCellPropertiesIndex, tableCellPropertiesCache.AddItem(new WebTableCellProperties(from.TableCellProperties)));
			result.Add((int)JSONTableConditionalStyleProperty.MaskedParagraphPropertiesCacheIndex, from.ParagraphProperties.Index);
			paragraphExporter.RegisterItem(from.ParagraphProperties.Index, from.ParagraphProperties.Info);
			result.Add((int)JSONTableConditionalStyleProperty.MaskedCharacterPropertiesCacheIndex, from.CharacterProperties.Index);
			characterExporter.RegisterItem(from.CharacterProperties.Index, from.CharacterProperties.Info);
			result.Add((int)JSONTableConditionalStyleProperty.Tabs, TabPropertiesExporter.ToHashtableList(from.Tabs));
			return result;
		}
	}
	public static class TableConditionalStyleExporter {
		public static Hashtable ToHashtable(TableConditionalStyle from, WebTablePropertiesCache tablePropertiesCache, WebTableRowPropertiesCache tableRowProperiesCache, WebTableCellPropertiesCache tableCellPropertiesCache, CharacterFormattingBaseExporter characterExporter, ParagraphFormattingBaseExporter paragraphExporter) {
			Hashtable result = new Hashtable();
			result.Add((int)JSONTableConditionalStyleProperty.TablePropertiesIndex, tablePropertiesCache.AddItem(new WebTableProperties(from.TableProperties)));
			result.Add((int)JSONTableConditionalStyleProperty.TableRowPropertiesIndex, tableRowProperiesCache.AddItem(new WebTableRowProperties(from.TableRowProperties)));
			result.Add((int)JSONTableConditionalStyleProperty.TableCellPropertiesIndex, tableCellPropertiesCache.AddItem(new WebTableCellProperties(from.TableCellProperties)));
			result.Add((int)JSONTableConditionalStyleProperty.MaskedParagraphPropertiesCacheIndex, from.ParagraphProperties.Index);
			paragraphExporter.RegisterItem(from.ParagraphProperties.Index, from.ParagraphProperties.Info);
			result.Add((int)JSONTableConditionalStyleProperty.MaskedCharacterPropertiesCacheIndex, from.CharacterProperties.Index);
			characterExporter.RegisterItem(from.CharacterProperties.Index, from.CharacterProperties.Info);
			result.Add((int)JSONTableConditionalStyleProperty.Tabs, TabPropertiesExporter.ToHashtableList(from.Tabs));
			return result;
		}
	}
}
