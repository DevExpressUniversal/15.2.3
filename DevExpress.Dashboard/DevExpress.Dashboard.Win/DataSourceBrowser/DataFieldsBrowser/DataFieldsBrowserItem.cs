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

using System;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.DashboardWin.ServiceModel;
namespace DevExpress.DashboardWin.Native {
	public enum DataFieldsBrowserGroupType {
		Text = DataFieldType.Text,
		DateTime = DataFieldType.DateTime,
		Bool = DataFieldType.Bool,
		Integer = DataFieldType.Integer,
		Float = DataFieldType.Float,
		Double = DataFieldType.Double,
		Decimal = DataFieldType.Decimal,
		Enum = DataFieldType.Enum,
		Custom = DataFieldType.Custom,		
		DataSource,
		Complex,
		List,
		Unknown,
		OlapDataSource,
		OlapMeasureFolder,
		OlapMeasure,
		OlapDimensionFolder,
		OlapDimension,
		OlapKpiFolder,
		OlapKpi,
		OlapFolder,
		OlapHierarchy,
		CalculatedFields,
		CalculatedFieldText,
		CalculatedFieldInteger,
		CalculatedFieldDecimal,
		CalculatedFieldDateTime,
		CalculatedFieldBool,
		CalculatedFieldObject,
		CalculatedFieldCorrupted,
		CalculatedFieldAggregate
	}
	public class DataFieldsBrowserItem {
		static DataFieldsBrowserGroupType GetGroupType(DataNode dataNode, IActualParametersProvider parametersProvider) {
			switch(dataNode.NodeType){
				case DataNodeType.OlapDataSource:
					return DataFieldsBrowserGroupType.OlapDataSource;
				case DataNodeType.OlapMeasureFolder:
					return DataFieldsBrowserGroupType.OlapMeasureFolder;
				case DataNodeType.OlapMeasure:
					return DataFieldsBrowserGroupType.OlapMeasure;
				case DataNodeType.OlapDimensionFolder:
					return DataFieldsBrowserGroupType.OlapDimensionFolder;
				case DataNodeType.OlapDimension:
					return DataFieldsBrowserGroupType.OlapDimension;
				case DataNodeType.OlapKpiFolder:
					return DataFieldsBrowserGroupType.OlapKpiFolder;
				case DataNodeType.OlapKpi:
					return DataFieldsBrowserGroupType.OlapKpi;
				case DataNodeType.OlapFolder:
					return DataFieldsBrowserGroupType.OlapFolder;
				case DataNodeType.OlapHierarchy:
					return DataFieldsBrowserGroupType.OlapHierarchy;
				case DataNodeType.CalculatedFields:
					return DataFieldsBrowserGroupType.CalculatedFields;
			}
			if(dataNode.NodeType == DataNodeType.CalculatedDataField) {
				CalculatedDataField calculatedField = (CalculatedDataField)dataNode;
				if(calculatedField.GetIsCorrupted(parametersProvider))
					return DataFieldsBrowserGroupType.CalculatedFieldCorrupted;
				if(calculatedField.IsAggregateCalculatedField)
					return DataFieldsBrowserGroupType.CalculatedFieldAggregate;
				return CalculatedFieldTypeToDataFieldsBrowserGroupType(calculatedField.CalculatedFieldType);
			}
			if (dataNode.ActualFieldType != DataFieldType.Unknown)
				return (DataFieldsBrowserGroupType)dataNode.ActualFieldType;
			if (dataNode.IsDataSourceNode)
				return DataFieldsBrowserGroupType.DataSource;
			if (dataNode.IsDataMemberNode)
				return dataNode.IsList ? DataFieldsBrowserGroupType.List : DataFieldsBrowserGroupType.Complex;
			return DataFieldsBrowserGroupType.Unknown;
		}
		static DataFieldsBrowserGroupType ConvertTypeToDataFieldsBrowserGroupType(Type type) {
			if(type == typeof(string))
				return DataFieldsBrowserGroupType.Text;
			if(type == typeof(bool))
				return DataFieldsBrowserGroupType.Bool;
			if(type == typeof(int))
				return DataFieldsBrowserGroupType.Integer;
			if(type == typeof(decimal))
				return DataFieldsBrowserGroupType.Decimal;
			if(type == typeof(float))
				return DataFieldsBrowserGroupType.Float;
			if(type == typeof(double))
				return DataFieldsBrowserGroupType.Double;
			return DataFieldsBrowserGroupType.Unknown;
		}
		internal static DataFieldsBrowserGroupType CalculatedFieldTypeToDataFieldsBrowserGroupType(DevExpress.DashboardCommon.CalculatedFieldType type) {
			switch(type) {
				case CalculatedFieldType.Integer:
					return DataFieldsBrowserGroupType.CalculatedFieldInteger;
				case CalculatedFieldType.Decimal:
					return DataFieldsBrowserGroupType.CalculatedFieldDecimal;
				case CalculatedFieldType.DateTime:
					return DataFieldsBrowserGroupType.CalculatedFieldDateTime;
				case CalculatedFieldType.String:
					return DataFieldsBrowserGroupType.CalculatedFieldText;
				case CalculatedFieldType.Boolean:
					return DataFieldsBrowserGroupType.CalculatedFieldBool;
				case CalculatedFieldType.Object:
				default:
					return DataFieldsBrowserGroupType.CalculatedFieldObject;
			}
		}
		internal static int GetImageIndex(DataFieldsBrowserGroupType groupType) {
			switch (groupType) {
				case DataFieldsBrowserGroupType.Enum:
				case DataFieldsBrowserGroupType.Text:
					return 6;
				case DataFieldsBrowserGroupType.DateTime:
					return 9;
				case DataFieldsBrowserGroupType.Bool:
					return 10;
				case DataFieldsBrowserGroupType.Integer:
					return 7;
				case DataFieldsBrowserGroupType.Float:
				case DataFieldsBrowserGroupType.Double:
				case DataFieldsBrowserGroupType.Decimal:
					return 8;
				case DataFieldsBrowserGroupType.Custom:
					return 11;
				case DataFieldsBrowserGroupType.DataSource:
				case DataFieldsBrowserGroupType.Complex:
					return 0;
				case DataFieldsBrowserGroupType.List:
					return 4;
				case DataFieldsBrowserGroupType.OlapDataSource:
					return 19;
				case DataFieldsBrowserGroupType.OlapFolder:
					return 20;
				case DataFieldsBrowserGroupType.OlapMeasure:
				case DataFieldsBrowserGroupType.OlapMeasureFolder:
					return 22;
				case DataFieldsBrowserGroupType.OlapDimensionFolder:
					return 23;
				case DataFieldsBrowserGroupType.OlapKpi:
				case DataFieldsBrowserGroupType.OlapKpiFolder:
					return 24;
				case DataFieldsBrowserGroupType.OlapDimension:
					return 25;
				case DataFieldsBrowserGroupType.OlapHierarchy:
					return 26;
				case DataFieldsBrowserGroupType.CalculatedFields:
				case DataFieldsBrowserGroupType.CalculatedFieldObject:
					return 3;
				case DataFieldsBrowserGroupType.CalculatedFieldBool:
					return 16;
				case DataFieldsBrowserGroupType.CalculatedFieldDateTime:
					return 15;
				case DataFieldsBrowserGroupType.CalculatedFieldDecimal:
					return 14;
				case DataFieldsBrowserGroupType.CalculatedFieldInteger:
					return 13;
				case DataFieldsBrowserGroupType.CalculatedFieldText:
					return 12;
				case DataFieldsBrowserGroupType.CalculatedFieldCorrupted:
					return 17;
				case DataFieldsBrowserGroupType.CalculatedFieldAggregate:
					return 18;
				default:
					return -1;
			} 
		}
		internal static int GetImageIndex(Type type) {
			return GetImageIndex(ConvertTypeToDataFieldsBrowserGroupType(type));
		}
		readonly DataNode dataNode;
		readonly DataFieldsBrowserGroupType groupType;
		int imageIndex;
		bool isDummyParent;
		public DataNode DataNode { get { return dataNode; } }
		public DataField DataField { get { return dataNode as DataField; } }
		public DataFieldsBrowserGroupType GroupType { get { return groupType; } }
		public int ImageIndex { get { return imageIndex; } }
		public bool IsDummyParent { get { return isDummyParent; } set { isDummyParent = value; } }
		public string DisplayName { get { return DataField != null ? DataField.Caption : dataNode.DisplayName; } }
		public string DataMember { get { return dataNode.DataMember; } }
		public DataFieldsBrowserItem(DataNode dataNode, IActualParametersProvider provider) {
			this.dataNode = dataNode;
			groupType = GetGroupType(dataNode, provider);
			imageIndex = GetImageIndex(groupType);
			isDummyParent = dataNode.IsDummyParent;
		}
		public override string ToString() {
			return DisplayName;
		}
		public void OnExpand(TreeListNode node) {
			if(groupType == DataFieldsBrowserGroupType.OlapFolder)
				imageIndex = node.ImageIndex = node.SelectImageIndex = 21;
		}
		public void OnCollapse(TreeListNode node) {
			if(groupType == DataFieldsBrowserGroupType.OlapFolder)
				imageIndex = node.ImageIndex = node.SelectImageIndex = 20;
		}
	}
}
