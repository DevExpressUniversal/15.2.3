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
using System.Collections;
using System.Collections.Generic;
using DevExpress.Data.Browsing.Design;
namespace DevExpress.DashboardCommon.Native {
	public enum DataNodeType {
		Unknown,
		DataSource,
		DataMember,
		DataField,
		CalculatedFields,
		CalculatedDataField,
		OlapDataSource,
		OlapMeasureFolder,
		OlapMeasure,
		OlapDimensionFolder,
		OlapDimension,
		OlapKpiFolder,
		OlapKpi,
		OlapFolder,
		OlapHierarchy,
	}
	public class DataNode : INode {
		readonly PickManager pickManager;
		readonly List<DataNode> childNodes = new List<DataNode>();
		public object Parent { get { return null; } }
		public IList ChildNodes { get { return childNodes; } }
		public bool IsDummyNode { get { return !IsDataMemberNode && !IsDataSourceNode; } }
		public bool IsDummyParent { get { return pickManager.AreContainDummyNode(childNodes); } }
		public bool IsEmpty { get { return false; } }
		public virtual string DataMember { get { return string.Empty; } }
		public virtual string DisplayName { get { return string.Empty; } set { } }
		public virtual bool IsList { get { return false; } }
		public virtual bool IsComplex { get { return false; } }
		public virtual bool IsDataSourceNode { get { return false; } }
		public virtual bool IsDataMemberNode { get { return false; } }
		public virtual bool IsDataFieldNode { get { return false; } }
		public virtual DataFieldType ActualFieldType { get { return DataFieldType.Unknown; } }
		public virtual DataNodeType NodeType { get { return DataNodeType.Unknown; } }
		protected PickManager PickManager { get { return pickManager; } }
		protected IDashboardDataSource DataSource { get { return pickManager.DataSource; } }
		public DataNode(PickManager pickManager) {
			this.pickManager = pickManager;
		}
		protected virtual void FillDataFields(List<DataField> dataFields) {
			foreach(DataNode node in childNodes)
				node.FillDataFields(dataFields);
		}
		public void Expand(EventHandler callback) {
			pickManager.ExpandNode(this);
			if(callback != null)
				callback(this, EventArgs.Empty);
		}
		public bool HasDataSource(object dataSource) {
			throw new NotSupportedException();
		}
		public IList<DataField> GetAllDataFields() {
			List<DataField> dataFields = new List<DataField>();
			FillDataFields(dataFields);
			return dataFields;
		}
		public DataNode FindNode(string dataMember) {
			if(dataMember == DataMember)
				return this;
			foreach(DataNode node in childNodes) {
				DataNode foundNode = node.FindNode(dataMember);
				if(foundNode != null)
					return foundNode;
			}
			return null;
		}
		public DataNode FindNodeDeep(string dataMember) {
			return pickManager.FindNode(dataMember);
		}
#if DEBUGTEST && !DXPORTABLE // TODO dnxcore
		void FillInfoList(List<DevExpress.DashboardCommon.Tests.DataNodeInfo> infoList) {
			infoList.Add(CreateInfo());
			foreach(DataNode node in childNodes)
				node.FillInfoList(infoList);
		}
		public DevExpress.DashboardCommon.Tests.DataNodeInfo CreateInfo() {
			return new DevExpress.DashboardCommon.Tests.DataNodeInfo {
				IsDataSourceNode = IsDataSourceNode,
				IsDataMemberNode = IsDataMemberNode,
				IsListNode = IsList,
				IsDataFieldNode = IsDataFieldNode,
				IsDummyNode = IsDummyNode,
				DataMember = DataMember,
				DisplayName = DisplayName
			};
		}
		public IList<DevExpress.DashboardCommon.Tests.DataNodeInfo> CreateInfoList() {
			List<DevExpress.DashboardCommon.Tests.DataNodeInfo> infoList = new List<DevExpress.DashboardCommon.Tests.DataNodeInfo>();
			FillInfoList(infoList);
			return infoList;
		}
#endif
	}
}
