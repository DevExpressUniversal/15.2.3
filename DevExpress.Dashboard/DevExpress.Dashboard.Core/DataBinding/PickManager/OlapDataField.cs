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
using System.Collections.Generic;
using DevExpress.XtraPivotGrid.Data;
using System;
namespace DevExpress.DashboardCommon.Native {
	public class OlapDataField : DataField {
		readonly DataNodeType nodeType;
		readonly DataFieldType olapFieldType;
		readonly DataItemNumericFormat defaultNumericFormat;
		public override DataNodeType NodeType { get { return nodeType; } }
		public DataItemNumericFormat DefaultNumericFormat { get { return defaultNumericFormat; } }
		public DataFieldType OlapFieldType { get { return olapFieldType; } }
		public OlapDataField(PickManager pickManager, ICustomizationTreeItem treeItem, DataNodeType nodeType) : this(pickManager, treeItem, nodeType, null) { }
		public OlapDataField(PickManager pickManager, ICustomizationTreeItem treeItem, DataNodeType nodeType, DataItemNumericFormat defaultNumericFormat)
			: base(pickManager, treeItem == null ? null : treeItem.Field.FieldName, treeItem == null ? null : treeItem.Caption,
					(nodeType == DataNodeType.OlapHierarchy || nodeType == DataNodeType.OlapDimension) ? typeof(string) : treeItem.Field.FieldType) {
			this.nodeType = nodeType;
				this.olapFieldType = treeItem == null || treeItem.Field == null ? DataFieldType.Text : DataBindingHelper.GetDataFieldType(treeItem.Field.FieldType);
			this.defaultNumericFormat = defaultNumericFormat;
		}
		public OlapDataField(Type type, DataNodeType nodeType) : base(null, String.Empty, String.Empty, type){
			this.nodeType = nodeType;
		}
	}
	public class OlapHierarchyDataField : OlapDataField {
		List<string> groupDataMembers;
		List<string> groupCaptions;
		int groupIndex;
		public List<string> GroupDataMembers { get { return groupDataMembers; } }
		public List<string> GroupCaptions { get { return groupCaptions; } }
		public int GroupIndex { get { return groupIndex; } }
		public OlapHierarchyDataField(PickManager pickManager, ICustomizationTreeItem treeItem)
			: base(pickManager, treeItem, DataNodeType.OlapHierarchy) {
			groupDataMembers = new List<string>();
			groupCaptions = new List<string>();
			foreach(PivotFieldItemBase field in treeItem.Field.Group.Fields) {
				groupDataMembers.Add(field.FieldName);
				groupCaptions.Add(field.Caption);
			}
			groupIndex = treeItem.Field.Group.Index;
		}
		public OlapHierarchyDataField(Type type) : base(type, DataNodeType.OlapHierarchy) {
		}
		public OlapHierarchyDataField(Type type, int groupIndex, List<string> groupDataMembers)
			: this(type) {
			this.groupIndex = groupIndex;
			this.groupDataMembers = groupDataMembers;
		}
	}
	public class OlapHierarchyDataFieldItem : OlapDataField {
		OlapHierarchyDataField field;
		int index;
		public override string DataMember {
			get { return field.GroupDataMembers[index]; }
		}
		public override string DisplayName {
			get { return field.GroupCaptions[index]; }
			set { base.DisplayName = value; }
		}
		public OlapHierarchyDataFieldItem(PickManager pickManager, OlapHierarchyDataField field, int index) : base(pickManager, null, DataNodeType.OlapDimension) {
			this.field = field;
			this.index = index;
		}
	}
}
