#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.XtraPivotGrid.Customization;
using DevExpress.XtraPivotGrid.Data;
using System.Text.RegularExpressions;
namespace DevExpress.DashboardCommon.Native {
	public static class OlapDataNodesCreator {
		const string MeasuresFolderName = "[Measures]";
		const string KpisFolderName = "[KPIs]";
		static string GetNames(string dataMember) {
			return dataMember.Split('.')[0];
		}
		static DataNode CreateFolderNode(PickManager pickManager, ICustomizationTreeItem treeItem) {
			string firstFolderName = GetNames(treeItem.Name);
			if(IsMeasureFolder(firstFolderName)) {
				return new OlapDataMemberNode(pickManager, treeItem, DataNodeType.OlapMeasureFolder);
			}
			if(IsKpiFolder(firstFolderName)) {
				return null;
			}
			if(IsDimensionFolder(treeItem)) {
				return new OlapDataMemberNode(pickManager, treeItem, DataNodeType.OlapDimensionFolder);
			}
			return new OlapDataMemberNode(pickManager, treeItem, DataNodeType.OlapFolder);
		}
		static DataNode CreateDataFieldNode(PickManager pickManager, ICustomizationTreeItem treeItem) {
			string firstFolderName = GetNames(treeItem.Field.FieldName);
			if(!IsMeasureFolder(firstFolderName)) {
				if(IsHierarcyField(treeItem)) {
					return new OlapHierarchyDataField(pickManager, treeItem);
				}
				return new OlapDataField(pickManager, treeItem, DataNodeType.OlapDimension);
			}
			if(IsKpiMeasure(treeItem)) {
				return new OlapDataField(pickManager, treeItem, DataNodeType.OlapKpi);
			} else {
				return new OlapDataField(
					pickManager, treeItem, DataNodeType.OlapMeasure,
					GetNumericFormat(((IPivotOLAPDataSource)pickManager.DataSource.GetOlapDataSource()).GetMeasureServerDefinedFormatString(treeItem.Field.FieldName))
				);
			}
		}
		static bool IsHierarcyField(ICustomizationTreeItem treeItem) {
			return treeItem.Field.Group != null;
		}
		static bool IsKpiMeasure(ICustomizationTreeItem treeItem) {
			return treeItem.Field.KPIType != XtraPivotGrid.PivotKPIType.None;
		}
		internal static bool IsMeasureFolder(string firstName) {
			return firstName == MeasuresFolderName;
		}
		static bool IsKpiFolder(string firstName) {
			return firstName == KpisFolderName;
		}
		static bool IsDimensionFolder(ICustomizationTreeItem treeItem) {
			return treeItem.Level == 0;
		}
		public static DataNode CreateOlapNode(PickManager pickManager, ICustomizationTreeItem treeItem) {
			return treeItem.Field == null ? CreateFolderNode(pickManager, treeItem) : CreateDataFieldNode(pickManager, treeItem);
		}
		public static DataItemNumericFormat GetNumericFormat(string mdxFormat) {
			if(string.IsNullOrEmpty(mdxFormat))
				return null;
			mdxFormat = mdxFormat.ToLower().Trim();
			if(mdxFormat == "percent")
				return new DataItemNumericFormat(null) { FormatType = DataItemNumericFormatType.Percent };
			if(mdxFormat == "standard")
				return new DataItemNumericFormat(null) { FormatType = DataItemNumericFormatType.Number, IncludeGroupSeparator = true };
			if(mdxFormat == "currency")
				return new DataItemNumericFormat(null) { FormatType = DataItemNumericFormatType.Currency, IncludeGroupSeparator = true };
			if(mdxFormat.Contains(";"))
				mdxFormat = mdxFormat.Split(';')[0];
			if(string.IsNullOrEmpty(mdxFormat))
				return null;
			DataItemNumericFormatType formatType = DataItemNumericFormatType.Number;
			string currency = null;
			if(mdxFormat.EndsWith("%"))
				formatType = DataItemNumericFormatType.Percent;
			if(mdxFormat.StartsWith("$") || mdxFormat.EndsWith("$")) {
				formatType = DataItemNumericFormatType.Currency;
				currency = "en-US";
			}
			mdxFormat = Regex.Replace(mdxFormat, "[^\\., #0;\\(\\)]", "").Trim();
			if(string.IsNullOrEmpty(mdxFormat))
				return null;
			int precision = 2;
			string[] precs = mdxFormat.Split('.');
			if(precs.Length == 2)
				precision = precs[1].Length;
			else
				precision = 0;
			bool includeGroupSeparator = precs[0].Contains(",") || precs[0].Contains(" ");
			return new DataItemNumericFormat(null) { FormatType = formatType, Precision = precision, CurrencyCultureName = currency, IncludeGroupSeparator = includeGroupSeparator };
		}
	}
}
