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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.Design.TypePickEditor;
using DevExpress.Design.VSIntegration;
using DevExpress.XtraTreeList.Nodes;
namespace DevExpress.XtraPrinting.Design {
	public class TypePickerTreeViewVS : TypePickerTreeView {
		Attribute attr;
		string attrValue;
		public TypePickerTreeViewVS(Type type, Attribute attr, string attrValue)
			: base(type, "None") {
			this.attr = attr;
			this.attrValue = attrValue;
		}
		public override void Start(ITypeDescriptorContext context, IServiceProvider provider, object currentValue) {
			base.Start(context, provider, currentValue);
			IDesignerHost designerHost = (IDesignerHost)provider.GetService(typeof(IDesignerHost));
			FillNodeInternal(designerHost);
		}
		protected virtual void FillNodeInternal(IDesignerHost designerHost) {
			var node = new ProjectObjectsPickerNode("Project Objects", Nodes);
			((IList)Nodes).Add(node);
			FillNodes(node.Nodes, designerHost);
		}
		protected virtual void FillNodes(TreeListNodes nodes, IDesignerHost designerHost) {
			DTEServiceBase dteService = new DTEServiceBase(designerHost);
			if(dteService == null) return;
			string[] items = dteService.GetClassesInfo(type, attr, attrValue);
			foreach(string item in items) {
				try {
					Type itemType = designerHost.GetType(item);
					PickerNode pickerNode = new TypePickerNode(itemType.Name, itemType, nodes);
					((IList)nodes).Add(pickerNode);
				} catch {
				}
			}
		}
	}
}
