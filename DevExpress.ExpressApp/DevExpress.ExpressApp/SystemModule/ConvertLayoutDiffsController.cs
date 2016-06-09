#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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

using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
namespace DevExpress.ExpressApp.SystemModule {
	internal static class ConvertLayoutDiffsController {
		private static ModelNode CreateNodesByActualNode(ModelNode actualNode, ModelNode diffLayout) {
			Stack<ModelNode> nodes = new Stack<ModelNode>();
			while (!(actualNode.Parent is IModelViewLayout)) {
				actualNode = actualNode.Parent;
				nodes.Push(actualNode);
			}
			ModelNode currentDiffNode = diffLayout;
			while(nodes.Count > 0) {
				if(currentDiffNode[nodes.Peek().Id] == null) {
					currentDiffNode = currentDiffNode.AddNode(nodes.Peek().Id, nodes.Peek().GetType());
				} else {
					currentDiffNode = currentDiffNode[nodes.Peek().Id];
					if(currentDiffNode != null && currentDiffNode.IsRemovedNode) {
						return null;
					}
				}
				nodes.Pop();
			}
			return currentDiffNode;
		}
		private static bool HasSamePath(ModelNode actualNode, ModelNode diffNode) {
			bool isActualNodeNull = actualNode == null;
			bool isDiffNodeNull = diffNode == null;
			if (actualNode is IModelDetailView && diffNode is IModelDetailView) {
				return true;
			}
			if (isActualNodeNull && isDiffNodeNull) {
				return true;
			} else {
				if (isActualNodeNull || isDiffNodeNull)
					return false;
			}
			bool result = HasSamePath(actualNode.Parent, diffNode.Parent);
			if(result) {
				result = actualNode.Id == diffNode.Id;
			}
			return result;
		}
		public static void ConvertLayoutDiffs(IList<ModelNode> nodes) {
			ModelNode generatorNode = nodes[0] as ModelNode;
			IModelViewLayout generatorLayout = (IModelViewLayout)generatorNode;
			List<Dictionary<string, ModelNode>> actualLayoutItemsList = new List<Dictionary<string, ModelNode>>();
			for(int i = 1; i < nodes.Count; i++) {
				IModelViewLayout diffLayout = nodes[i] as IModelViewLayout;
				if(diffLayout != null) {
					Dictionary<string, ModelNode> actualLayoutItems = new Dictionary<string, ModelNode>();
					foreach(ModelNode item in ModelLayoutGroupLogic.GetLayoutItems<IModelLayoutViewItem>(diffLayout)) {
						if(!item.IsNewNode) {
							actualLayoutItems[item.Id] = item;
						}
					}
					actualLayoutItemsList.Add(actualLayoutItems);
				}
			}
			foreach(ModelNode generatorItem in ModelLayoutGroupLogic.GetLayoutItems<IModelLayoutViewItem>(generatorLayout)) {
				foreach(Dictionary<string, ModelNode> actualLayoutItems in actualLayoutItemsList) {
					ModelNode actualLayoutItem = null;
					if(actualLayoutItems.TryGetValue(generatorItem.Id, out actualLayoutItem)) {
						if(!HasSamePath(generatorItem, actualLayoutItem)) {
							((IModelNode)generatorItem).Remove();
							ModelNode generatorParent = CreateNodesByActualNode(actualLayoutItem, generatorNode);
							if(generatorParent != null) {
								generatorParent.AddClonedNode(generatorItem, generatorItem.Id);
							}
						}
						break;
					}
				}
			}
		}
	}
}
