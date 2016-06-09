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
	public enum JSONTableRowProperty {
		Height = 0,
		CellSpacing = 1,
		Header = 2,
		HideCellMark = 3,
		CantSplit = 4,
		TableRowAlignment = 5,
		UseValue = 6,
		GridAfter = 7,
		GridBefore = 8,
		WidthAfter = 9,
		WidthBefore = 10
	}
	public class WebTableRowProperties : IHashtableProvider {
		WidthUnit cellSpacing;
		bool header;
		bool hideCellMark;
		bool cantSplit;
		TableRowAlignment tableRowAlignment;
		TableRowPropertiesOptions.Mask useValue;
		public WebTableRowProperties(TableRowProperties properties) {
			cellSpacing = properties.CellSpacing;
			header = properties.Header;
			hideCellMark = properties.HideCellMark;
			cantSplit = properties.CantSplit;
			tableRowAlignment = properties.TableRowAlignment;
			useValue = properties.UseValue;
		}
		public void FillHashtable(Hashtable result) {
			result.Add((int)JSONTableRowProperty.CellSpacing, WidthUnitExporter.ToHashtable(cellSpacing));
			result.Add((int)JSONTableRowProperty.Header, header);
			result.Add((int)JSONTableRowProperty.HideCellMark, hideCellMark);
			result.Add((int)JSONTableRowProperty.CantSplit, cantSplit);
			result.Add((int)JSONTableRowProperty.TableRowAlignment, (int)tableRowAlignment);
			result.Add((int)JSONTableRowProperty.UseValue, (int)useValue);
		}
		public Hashtable ToHashtable() {
			var result = new Hashtable();
			FillHashtable(result);
			return result;
		}
		public override int GetHashCode() {
			return (cantSplit ? 1 : 0) ^
				WebTableProperties.GetWidthUnitHashCode(cellSpacing) ^
				(header ? 1 : 0) ^
				(hideCellMark ? 1 : 0) ^
				(int)tableRowAlignment ^
				(int)useValue;
		}
		public override bool Equals(object obj) {
			var prop = obj as WebTableRowProperties;
			if(prop == null)
				return false;
			return prop.cantSplit == cantSplit &&
				WebTableProperties.EqualsWidthUnit(prop.cellSpacing, cellSpacing) &&
				prop.header == header &&
				prop.hideCellMark == hideCellMark &&
				prop.tableRowAlignment == tableRowAlignment &&
				prop.useValue == useValue;
		}
	}
	public class WebTableRowPropertiesCache : WebModelPropertiesCacheBase<WebTableRowProperties> {
	}
	public static class TableRowPropertiesExporter {
		public static void ConvertFromHashtable(Hashtable source, TableRowProperties props) {
			WidthUnitExporter.FromHashtable((Hashtable)source[((int)JSONTableRowProperty.CellSpacing).ToString()], props.CellSpacing);
			props.Header = (bool)source[((int)JSONTableRowProperty.Header).ToString()];
			props.HideCellMark = (bool)source[((int)JSONTableRowProperty.HideCellMark).ToString()];
			props.CantSplit = (bool)source[((int)JSONTableRowProperty.CantSplit).ToString()];
			props.TableRowAlignment = (TableRowAlignment)source[((int)JSONTableRowProperty.TableRowAlignment).ToString()];
			props.UseValue = (TableRowPropertiesOptions.Mask)source[((int)JSONTableRowProperty.UseValue).ToString()];
		}
	}
}
