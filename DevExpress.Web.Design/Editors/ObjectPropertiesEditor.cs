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
using System.ComponentModel;
using System.ComponentModel.Design;
namespace DevExpress.Web.Design {
	public class ObjectPropertiesEditor : ObjectSelectorEditor {
		public ObjectPropertiesEditor()
			: base(true) {
		}
		protected override void FillTreeWithData(ObjectSelectorEditor.Selector selector, ITypeDescriptorContext context, IServiceProvider provider) {
			ASPxObjectContainer objectContainer = (ASPxObjectContainer)context.Instance;
			selector.Clear();
			foreach(ObjectType type in ASPxObjectContainer.ObjectPropertiesTypes.Keys) {
				selector.AddNode(type.ToString(),
					objectContainer.CreateObjectProperties(ASPxObjectContainer.ObjectPropertiesTypes[type]), null);
			}
			selector.Sort();
			foreach(System.Windows.Forms.TreeNode node in selector.Nodes) {
				if(node.Text == objectContainer.ObjectType.ToString()) {
					selector.SelectedNode = node;
					break;
				}
			}
			selector.Select();
			selector.FullRowSelect = true;
			selector.ShowLines = false;
			selector.ShowRootLines = false;
			selector.ShowPlusMinus = false;
			selector.Scrollable = false;
			selector.Width = 100;
			selector.ItemHeight = 20;
			selector.ClientSize = new System.Drawing.Size(selector.ClientSize.Width, selector.GetNodeCount(false) * selector.ItemHeight);
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(context == null || !(context.Instance is ASPxObjectContainer))
				return null;
			ASPxObjectContainer objectContainer = (ASPxObjectContainer)context.Instance;
			ObjectProperties objectProperties = base.EditValue(context, provider, value) as ObjectProperties;
			if(objectProperties != null) {
				objectContainer.ObjectType = objectProperties.ObjectType;
				if(objectContainer.ObjectType == ObjectType.Auto) {
					objectProperties = null;
					objectContainer.ResetObjectProperties();
				}
			}
			return objectProperties;
		}
		public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return System.Drawing.Design.UITypeEditorEditStyle.DropDown;
		}
	}
}
