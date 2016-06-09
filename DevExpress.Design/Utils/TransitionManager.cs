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
using DevExpress.Utils.Animation;
namespace DevExpress.Utils.Design {
	public class TransitionCollectionEditor : DXCollectionEditorBase {
		public TransitionCollectionEditor(Type type)
			: base(type) {
		}
		protected override Type CreateCollectionItemType() {
			return typeof(Transition);
		}
		protected override Type[] CreateNewItemTypes() {
			return new Type[] { typeof(Transition) };
		}
		protected override object CreateCustomInstance(Type itemType) {
			return new Transition();
		}
	}
	public class TransitionManagerDesigner : BaseComponentDesigner {
		public TransitionManagerDesigner() { }
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new TransitionControlDesignerActionList(Component, this));
			base.RegisterActionLists(list);
		}
	}
	public class TransitionControlDesignerActionList : DesignerActionList {
		IDesigner designerCore;
		public TransitionControlDesignerActionList(IComponent component, IDesigner designer)
			: base(component) {
			designerCore = designer;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			res.Add(new DesignerActionPropertyItem("ShowWaitingIndicator", "Show Waiting Indicator"));
			res.Add(new DesignerActionPropertyItem("FrameInterval", "Frame Interval"));
			res.Add(new DesignerActionPropertyItem("FrameCount", "Frame Count"));
			res.Add(new DesignerActionMethodItem(this, "ChooseTransitions", "Choose Transitions"));
			return res;
		}
		public void ChooseTransitions() {
			EditorContextHelper.EditValue(designerCore, Component, "Transitions");
		}
		public TransitionManager Manager { get { return Component as TransitionManager; } }
		public int FrameInterval {
			get { return Manager.FrameInterval; }
			set { EditorContextHelper.SetPropertyValue(designerCore, Component, "FrameInterval", value); }
		}
		public int FrameCount {
			get { return Manager.FrameCount; }
			set { EditorContextHelper.SetPropertyValue(designerCore, Component, "FrameCount", value); }
		}
		public bool ShowWaitingIndicator {
			get { return Manager.ShowWaitingIndicator; }
			set { EditorContextHelper.SetPropertyValue(designerCore, Component, "ShowWaitingIndicator", value); }
		}
	}
}
