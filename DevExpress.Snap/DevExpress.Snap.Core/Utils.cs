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

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.Data.Browsing;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Core.Native.Data;
using System.Collections;
using DevExpress.Data.Helpers;
using System.Data;
using DevExpress.XtraPrinting.Native;
using System.Reflection;
namespace DevExpress.Snap.Core {
	public static class SnapDataHelper {
		public static SNDataInfo[] GetDataInfo(IDataObject data) {
			return !data.GetDataPresent(SnapDataFormats.SNDataInfo) ? null : (SNDataInfo[])data.GetData(SnapDataFormats.SNDataInfo);
		}
	}
	public static class DataRelationHelper {
		public static List<string> GetRelatedDataMembers(object dataSource, string dataMember) {
			string[] dataMemberParts = dataMember.Split('.');
			List<PropertyDescriptor> listAccessors = GetListAccessors(dataSource, dataMemberParts);
			if (listAccessors.Count == 0)
				return null;
			List<string> result = new List<string>();
			Dictionary<string, string[]> relationsMap = CalculateRelations(dataSource, listAccessors.ToArray());
			if (relationsMap.Count == 0)
				return null;
			int count = dataMemberParts.Length;
			for (int index = 0; index < count; index++) {
				string part = dataMemberParts[index];
				string[] columns;
				if (!relationsMap.TryGetValue(part, out columns))
					continue;
				string prefix = String.Join(".", dataMemberParts, 0, index);
				if (!String.IsNullOrEmpty(prefix))
					prefix += ".";
				int columnCount = columns.Length;
				for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
					result.Add(prefix + columns[columnIndex]);
			}
			return result;
		}
		static List<PropertyDescriptor> GetListAccessors(object dataSource, string[] relations) {
			PropertyDescriptorCollection properties = ListBindingHelper.GetListItemProperties(dataSource);
			List<PropertyDescriptor> listAccessors = new List<PropertyDescriptor>();
			for (int i = 0; i < relations.Length; i++) {
				PropertyDescriptor propertyDescriptor = properties.Find(relations[i], true);
				if (propertyDescriptor != null)
					listAccessors.Add(propertyDescriptor);
				else
					break;
				properties = ListBindingHelper.GetListItemProperties(dataSource, new PropertyDescriptor[] { propertyDescriptor });
				object value = GetPropertyValue(dataSource, propertyDescriptor);
				if (value == null)
					break;
				dataSource = value;
			}
			return listAccessors;
		}
		static Dictionary<string, string[]> CalculateRelations(object dataSource, PropertyDescriptor[] listAccessors) {
			Dictionary<string, string[]> result = new Dictionary<string, string[]>();
			object source = dataSource;
			int length = listAccessors.Length;
			for (int i = 0; i < length; i++) {
				PropertyDescriptor propertyDescriptor = listAccessors[i];
				object value = GetPropertyValue(source, propertyDescriptor);
				if (value == null)
					continue;
				DataView dataView = value as DataView;
				if (dataView != null) {
					DataRelation dataRelation = dataView.Table.ParentRelations[propertyDescriptor.DisplayName];
					if (dataRelation != null)
						PopulateRelationTable(result, dataRelation);
				}
				source = value;
			}
			return result;
		}
		static object GetPropertyValue(object source, PropertyDescriptor propertyDescriptor) {
			IList list = MasterDetailHelper.GetDataSource(source, String.Empty);
			if (list != null && list.Count > 0)
				return propertyDescriptor.GetValue(list[0]);
			return null;
		}
		static void PopulateRelationTable(Dictionary<string, string[]> result, DataRelation dataRelation) {
			DataColumn[] dataColumns = dataRelation.ParentColumns;
			List<string> columnNames = new List<string>();
			int count = dataColumns.Length;
			for (int i = 0; i < count; i++)
				columnNames.Add(dataColumns[i].ColumnName);
			result.Add(dataRelation.RelationName, columnNames.ToArray());
		}
	}
}
