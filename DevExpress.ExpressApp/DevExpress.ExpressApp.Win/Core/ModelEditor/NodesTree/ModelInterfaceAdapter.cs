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

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.Controls;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.ExpressApp.Model.NodeWrappers;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.ExpressApp.Win.Model;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	[ToolboxItem(false)]
	public class ModelInterfaceAdapter : NodeObjectAdapter {
		protected FastModelEditorHelper modelEditorHelper = new FastModelEditorHelper();
		public virtual IEnumerable GetSortedNodes(IEnumerable sourceNodes) {
			List<ModelNode> nodes = new List<ModelNode>();
			foreach(ModelNode node in sourceNodes) {
				nodes.Add(node);
			}
			nodes.Sort(new ModelTreeListNodeComparer(modelEditorHelper));
			return nodes;
		}
		public override IEnumerable GetChildren(object nodeObject) {
			if(nodeObject == null) { return null; }
			IEnumerable sourceNodes = GetChildrenCore(nodeObject);
			return GetSortedNodes(sourceNodes);
		}
		protected virtual ModelNode[] GetChildrenCore(object nodeObject) {
			if(ModelTreeListNode.IsModelVirtualTreeNode((IModelNode)nodeObject)) {
				List<ModelNode> result = new List<ModelNode>();
				ModelVirtualTreeGetChildrenAttribute[] attributes = AttributeHelper.GetAttributesConsideringInterfaces<ModelVirtualTreeGetChildrenAttribute>(nodeObject.GetType(), true);
				if(attributes != null && attributes.Length > 0) {
					Type logicType = attributes[0].ChildrenProviderType;
					if(typeof(IModelVirtualTreeChildrenProvider).IsAssignableFrom(logicType)) {
						IModelVirtualTreeChildrenProvider logic = (IModelVirtualTreeChildrenProvider)Activator.CreateInstance(logicType);
						IEnumerable<IModelNode> innerResult = logic.GetChildren((IModelNode)nodeObject);
						if(innerResult != null) {
							foreach(IModelBandedLayoutItem item in innerResult) {
								result.Add((ModelNode)item);
							}
						}
					}
				}
				return result.ToArray();
			}
			else {
					List<ModelNode> result = new List<ModelNode>(ModelEditorHelper.GetChildNodes((ModelNode)nodeObject));
					return GetFilterdChildren(result);
			}
		}
		private ModelNode[] GetFilterdChildren(List<ModelNode> children) {
			List<ModelNode> result = new List<ModelNode>();
			foreach(ModelNode node in children) {
				ModelEditorBrowsableAttribute[] attributes = AttributeHelper.GetAttributesConsideringInterfaces<ModelEditorBrowsableAttribute>(node.GetType(), true);
				bool canVisible = true;
				if(attributes != null && attributes.Length > 0) {
					canVisible = false;
					foreach(ModelEditorBrowsableAttribute atr in attributes) {
						canVisible = atr.Visible;
						if(canVisible) {
							break;
						}
					}
				}
				if(canVisible) {
					result.Add(node);
				}
			}
			return result.ToArray();
		}
		public override string GetDisplayPropertyValue(object nodeObject) {
			if(nodeObject == null) { return null; }
			ModelNode modelNode = (ModelNode)nodeObject;
			return modelEditorHelper.GetModelNodeDisplayValue(modelNode);
		}
		public override Image GetImage(object nodeObject, out string imageName) {
			ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(((ModelNode)nodeObject).GetType());
			imageName = null;
			foreach(ITypeInfo interfaceTypeInfo in typeInfo.ImplementedInterfaces) {
				ImageNameAttribute imageNameAttribute = interfaceTypeInfo.FindAttribute<ImageNameAttribute>();
				if(imageNameAttribute != null) {
					imageName = imageNameAttribute.ImageName;
					return ImageLoader.Instance.GetImageInfo(imageName).Image;
				}
			}
			imageName = "ModelEditor_Default";
			return ImageLoader.Instance.GetImageInfo(imageName).Image;
		}
		public override object GetParent(object nodeObject) {
			if(nodeObject == null) { return null; }
			return ((ModelNode)nodeObject).Parent;
		}
		public override bool HasChildren(object nodeObject) {
			if(nodeObject == null) { return false; }
			return GetChildrenCore(nodeObject).Length > 0;
		}
		public override bool IsRoot(object nodeObject) {
			return true;
		}
		public override string DisplayPropertyName {
			get { return "Name"; }	
		}
		public virtual string GetModelNodeDisplayValue_Cached(ModelNode modelNode) {
			return modelEditorHelper.GetModelNodeDisplayValue(modelNode);
		}
	}
}
