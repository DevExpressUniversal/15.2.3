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
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model.NodeWrappers;
namespace DevExpress.ExpressApp.Model.DomainLogics {
	[DomainLogic(typeof(IModelBandsLayout))]
	public static class ModelBandsLayoutDomainLogic {
		public static int? Get_Index(IModelBandsLayout model) {
			return 1;
		}
	}
	[DomainLogic(typeof(IModelBand))]
	public static class ModelBandDomainLogic {
		public static string Get_Caption(IModelBandedLayoutItem model) {
			return model.Id;
		}
	}
	[DomainLogic(typeof(IModelBandedLayoutItem))]
	public static class ModelBandedLayoutItemDomainLogic {
		public static IModelList<IModelBand> Get_Bands(IModelBandedLayoutItem model) {
			return new CalculatedModelNodeList<IModelBand>(new ModelBandLayoutItemCollection(model).GetItems<IModelBand>());
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public interface IModelVirtualTreeChildrenProvider {
		IEnumerable<IModelNode> GetChildren(IModelNode parent);
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ModelVirtualTreeChildrenProvider : IModelVirtualTreeChildrenProvider {
		public IEnumerable<IModelNode> GetChildren(IModelNode parent) {
			return new ModelBandLayoutItemCollection((IModelNode)parent).GetChildren((IModelNode)parent);
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ModelVirtualTreeParentLogic : IModelVirtualTreeParentLogic {
		private void BandLayoutItemSetParent(IModelBandedLayoutItem draggedModelNode, IModelBandedLayoutItem targer) {
			if(draggedModelNode != null) {
				draggedModelNode.OwnerBand = targer as IModelBand;
			}
		}
		public bool SetParent(IModelNode draggedModelNode, IModelNode targer) {
			BandLayoutItemSetParent(draggedModelNode as IModelBandedLayoutItem, targer as IModelBandedLayoutItem);
			return true;
		}
		public IModelNode GetParent(IModelNode node) {
			IModelNode result = null;
			if(node != null) {
				if(node is IModelBandedLayoutItem) {
					result = ((IModelBandedLayoutItem)node).OwnerBand;
				}
				if(result == null && node is IModelBandedColumn) {
					IModelListView listViewModel = GetListViewModel((IModelBandedColumn)node);
					if(listViewModel != null) {
						result = listViewModel.BandsLayout;
					}
				}
				if(result == null) {
					result = node.Parent;
				}
			}
			return result;
		}
		private IModelListView GetListViewModel(IModelBandedColumn node) {
			IModelListView result = null;
			IModelNode parent = node.Parent;
			if(parent is IModelListView) {
				result = (IModelListView)parent;
			}
			else {
				while(parent != null) {
					parent = parent.Parent;
					if(parent is IModelListView) {
						result = (IModelListView)parent;
					}
				}
			}
			return result;
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ModelBandsLayoutChildrenVisibilityCalculator : IModelIsVisible {
		public bool IsVisible(IModelNode node, string propertyName) {
			return ((IModelBandsLayout)node).Enable;
		}
	}
}
