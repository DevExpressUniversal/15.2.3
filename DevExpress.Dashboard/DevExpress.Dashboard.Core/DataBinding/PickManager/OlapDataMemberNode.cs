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
using System;
namespace DevExpress.DashboardCommon.Native {
	public class OlapDataMemberNode : DataMemberNode {
		readonly DataNodeType nodeType;
		static string GetDataMember(ICustomizationTreeItem treeItem, DataNodeType nodeType) {
			if(nodeType == DataNodeType.OlapFolder) {
				string result = String.Format("[{0}]", treeItem.Name);
				ICustomizationTreeItem parentItem = treeItem.Parent;
				int level = parentItem.Level;
				while(level != 0) {
					result = String.Format("[{0}].{1}", parentItem.Name, result);
					parentItem = parentItem.Parent;
					level = parentItem.Level;
				}
				result = String.Format("{0}.{1}", parentItem.Name, result);
				return result;
			}
			else {
				return treeItem.Name;
			}
		}
		public override DataNodeType NodeType { get { return nodeType; } }
		public OlapDataMemberNode(PickManager pickManager, ICustomizationTreeItem treeItem, DataNodeType nodeType)
			: base(pickManager, GetDataMember(treeItem, nodeType), treeItem.Caption, true) {
				this.nodeType = nodeType;
		}
	}
}
