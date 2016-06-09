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
using System.Windows;
using System.Collections;
using DevExpress.Xpf.Grid;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Data;
using DevExpress.Xpf.Utils;
using System.Collections.Generic;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid.Native;
#if SILVERLIGHT
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
#endif
namespace DevExpress.Xpf.Grid {
	public class GridDataIterator : DataIteratorBase {
		GridViewBase View { get { return (GridViewBase)viewBase; } }
		GridControl Grid { get { return View.Grid; } }
		public GridDataIterator(GridViewBase view) 
			: base(view) {
		}
		protected internal override bool GetHasTop(DataNodeContainer nodeContainer) {
			return base.GetHasTop(nodeContainer) || GetHasTopCore(nodeContainer);
		}
		protected virtual bool GetHasTopCore(DataNodeContainer nodeContainer) {
			return GetActualRowLevel(nodeContainer.StartScrollIndex - 1) < nodeContainer.GroupLevel;
		}
		protected internal override bool GetHasBottomCore(DataNodeContainer nodeContainer, int lastVisibleIndex) {
			return GetActualRowLevel(lastVisibleIndex + 1) < nodeContainer.GroupLevel;
		}
		int GetActualRowLevel(int scrollIndex) {
			object visibleIndex = Grid.DataProviderBase.GetVisibleIndexByScrollIndex(scrollIndex);
			return (visibleIndex is GroupSummaryRowKey) ? ((GroupSummaryRowKey)visibleIndex).Level : Grid.GetRowLevelByVisibleIndex((int)visibleIndex);
		}
		GroupNode GetGroupNode(int rowHandle) {
			DataRowNode node = null;
			if(Grid.View.Nodes.TryGetValue(rowHandle, out node)) 
				return node as GroupNode;
			else 
				return null;
		}
		protected internal override RowNode GetRowNodeForCurrentLevel(DataNodeContainer nodeContainer, int index, int startVisibleIndex, ref bool shouldBreak) {
			object visibleIndex = Grid.DataProviderBase.GetVisibleIndexByScrollIndex(index);
			GroupSummaryRowKey groupSummaryRowKey = visibleIndex as GroupSummaryRowKey;
			if(groupSummaryRowKey != null) {
				if(nodeContainer.IsGroupRowsContainer && nodeContainer.GroupLevel == groupSummaryRowKey.Level && (nodeContainer.treeBuilder.View.AllowFixedGroupsCore ? nodeContainer.StartScrollIndex != index : true)) {
					RowNode summaryNode = GetSummaryNodeForCurrentNode(nodeContainer, groupSummaryRowKey.RowHandle, index);
					if(summaryNode != null) {
						GroupNode node = GetGroupNode(groupSummaryRowKey.RowHandle.Value);
						if(node != null)
							node.summaryNode = summaryNode;
						return summaryNode;
					}
				}
				else if(groupSummaryRowKey.Level < nodeContainer.GroupLevel) {
					shouldBreak = true;
				}
				else if(nodeContainer.StartScrollIndex == index) {
					return GetGroupRowNode(nodeContainer.treeBuilder, nodeContainer.StartScrollIndex + (nodeContainer.treeBuilder.View.AllowFixedGroupsCore ? 1 : 0), nodeContainer.treeBuilder.View.AllowFixedGroupsCore, DataIteratorBase.CreateValuesContainer(nodeContainer.treeBuilder, nodeContainer.parentVisibleIndex), (index == nodeContainer.StartScrollIndex) ? nodeContainer.DetailStartScrollIndex : 0);
				}
				return null;
			}
			DataControllerValuesContainer info = CreateValuesContainer(nodeContainer.treeBuilder, (int)visibleIndex);
			if(info.RowHandle.Value == DataControlBase.InvalidRowHandle)
				return null;
			if((info.RowHandle.Value < 0) && !nodeContainer.IsGroupRowsContainer && info.RowHandle.Value != DataControlBase.NewItemRowHandle) {
				shouldBreak = true;
				return null;
			}
			int actualLevel = View.DataControl.DataProviderBase.GetActualRowLevel(info.RowHandle.Value, info.Level);
			if(actualLevel > nodeContainer.GroupLevel && index == nodeContainer.StartScrollIndex)
				return GetGroupRowNode(nodeContainer.treeBuilder, nodeContainer.StartScrollIndex + (View.AllowFixedGroupsCore ? 1 : 0), View.AllowFixedGroupsCore, DataIteratorBase.CreateValuesContainer(nodeContainer.treeBuilder, nodeContainer.parentVisibleIndex), startVisibleIndex);
			if(nodeContainer.GroupLevel == actualLevel)
				return CreateRowNode(nodeContainer, info, startVisibleIndex, index);
			if(actualLevel < nodeContainer.GroupLevel)
				shouldBreak = true;
			return null;
		}
		RowNode CreateRowNode(DataNodeContainer nodeContainer, DataControllerValuesContainer info, int startVisibleIndex, int globalIndex) {
			return DataTreeBuilder.CreateRowElement<RowNode>(
				nodeContainer.IsGroupRowsContainer && View.DataControl.IsGroupRowHandleCore(info.RowHandle.Value),
				() => GetGroupRowNode(nodeContainer.treeBuilder,  globalIndex + 1, true, info, startVisibleIndex),
				() => GetRowNode(nodeContainer.treeBuilder, startVisibleIndex, info)
			);
		}
		protected internal override bool IsGroupRowsContainer(DataNodeContainer nodeContainer) {
			return nodeContainer.GroupLevel < View.Grid.ActualGroupCount;
		}
		GroupNode GetGroupRowNode(DataTreeBuilder treeBuilder, int startVisibleIndex, bool isGroupRowVisible, DataControllerValuesContainer controllerValues, int detailStartVisibleIndex) {
			GroupNode groupNode = (GroupNode)treeBuilder.GetRowNode(values => new GroupNode(treeBuilder, values), controllerValues);
			groupNode.summaryNode = null;
			groupNode.NodesContainer.DetailStartScrollIndex = detailStartVisibleIndex;
			groupNode.UpdateExpandInfo(startVisibleIndex, isGroupRowVisible);
			return groupNode;
		}
		protected internal override RowNode GetSummaryNodeForCurrentNode(DataNodeContainer nodeContainer, RowHandle rowHandle, int index) {
			if(View.ShowGroupSummaryFooter) {
				GroupSummaryRowNode rowNode = (GroupSummaryRowNode)nodeContainer.treeBuilder.GetGroupSummaryRowNode(rowHandle.Value);
				if(rowNode != null)
					return rowNode;
				return CreateGroupSummaryNode(nodeContainer, rowHandle);
			}
			return null;
		}
		protected virtual RowNode CreateGroupSummaryNode(DataNodeContainer nodeContainer, RowHandle rowHandle) {
			GroupSummaryRowNode rowNode = new GroupSummaryRowNode(nodeContainer.treeBuilder, CreateValuesContainer(nodeContainer.treeBuilder, rowHandle));
			nodeContainer.treeBuilder.AddGroupSummaryRowNode(rowHandle.Value, rowNode);
			return rowNode;
		}
		protected internal override int GetRowParentIndex(DataNodeContainer nodeContainer, int visibleIndex, int level) {
			object visibleIndexCore = Grid.DataProviderBase.GetVisibleIndexByScrollIndex(visibleIndex);
			if(visibleIndexCore == null)
				return base.GetRowParentIndex(nodeContainer, visibleIndex, level);
			GroupSummaryRowKey groupSummaryRowKey = visibleIndexCore as GroupSummaryRowKey;
			if(groupSummaryRowKey != null)
				return base.GetRowParentIndex(nodeContainer, nodeContainer.treeBuilder.View.DataProviderBase.GetRowVisibleIndexByHandle(groupSummaryRowKey.RowHandle.Value), level);
			return base.GetRowParentIndex(nodeContainer, (int)visibleIndexCore, level);
		}
	}
}
