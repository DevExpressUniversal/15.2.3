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
using System.ComponentModel.Design;
using System.ComponentModel;
namespace DevExpress.Utils.Design {
	public class AdornerElementCollectionEditor : DXCollectionEditorBase {
		public AdornerElementCollectionEditor(Type type)
			: base(type) {
		}
		protected override Type CreateCollectionItemType() {
			return typeof(DevExpress.Utils.VisualEffects.Badge);
		}
		protected override Type[] CreateNewItemTypes() {
			return new Type[] {
				typeof(DevExpress.Utils.VisualEffects.Badge)
			};
		}
	}
	public class AdornerUIManagerDesigner : BaseComponentDesigner {
		DevExpress.Utils.VisualEffects.AdornerUIManager Manager { get { return this.Component as DevExpress.Utils.VisualEffects.AdornerUIManager; } }
		public AdornerUIManagerDesigner() { }
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new AdornerUIManagerActionList(Component, this));
			base.RegisterActionLists(list);
		}
		protected override void OnInitializeNew(System.Collections.IDictionary defaultValues) {
			base.OnInitializeNew(defaultValues);
			if(Manager != null) {
				System.Windows.Forms.ContainerControl container = this.ParentComponent as System.Windows.Forms.ContainerControl;
				Manager.Owner = container;
			}
		}
	}
	public class AdornerUIManagerActionList : DesignerActionList {
		IDesigner designerCore;
		public AdornerUIManagerActionList(IComponent component, IDesigner designer)
			: base(component) {
			designerCore = designer;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			res.Add(new DesignerActionPropertyItem("Owner", "Owner"));
			res.Add(new DesignerActionMethodItem(this, "ChooseElements", "Choose Elements"));
			return res;
		}
		public void ChooseElements() {
			EditorContextHelper.EditValue(designerCore, Component, "Elements");
		}
		public System.Windows.Forms.ContainerControl Owner {
			get {
				DevExpress.Utils.VisualEffects.AdornerUIManager manager = Component as DevExpress.Utils.VisualEffects.AdornerUIManager;
				return manager.Owner;
			}
			set { EditorContextHelper.SetPropertyValue(designerCore, Component, "Owner", value); }
		}
	}
}
