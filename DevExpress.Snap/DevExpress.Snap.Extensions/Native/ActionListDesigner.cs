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
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Design;
namespace DevExpress.Snap.Extensions.Native {
	public class ActionListDesignerBase : IComponentDesigner {
		ActionListCollection actionLists;
		readonly DesignerVerbCollection verbs;
		ComponentImplementation component;
		[System.Security.SecuritySafeCritical]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
		public ActionListDesignerBase() {
			this.verbs = new DesignerVerbCollection();
		}
		public ComponentImplementation Component { get { return component; } }
		public virtual void Initialize(ComponentImplementation component) {
			this.component = component;
		}
		public virtual ActionListCollection ActionLists {
			get {
				if (actionLists == null)
					actionLists = CreateActionLists();
				return actionLists;
			}
		}
		protected virtual ActionListCollection CreateActionLists() {
			ActionListCollection result = new ActionListCollection();
			RegisterActionLists(result);
			return result;
		}
		protected virtual void RegisterActionLists(ActionListCollection list) {
		}
		IDesignerActionListCollection IComponentDesigner.ActionLists {
			get { return ActionLists; }
		}
		public DesignerVerbCollection Verbs {
			get { return verbs; }
		}
	}
	public class ActionListDesigner : ActionListDesignerBase {
		readonly Type[] actionListTypes;
		public ActionListDesigner() {
		}
		public ActionListDesigner(Type[] actionListTypes) {
			this.actionListTypes = actionListTypes;
		}
		protected override void RegisterActionLists(ActionListCollection list) {
			foreach (Type actionListType in actionListTypes) {
				IDesignerActionList actionList = CreateActionList(actionListType);
				if (actionList != null)
					list.Add(actionList);
			}
		}
		protected virtual IDesignerActionList CreateActionList(Type actionListType) {
			return (IDesignerActionList)Activator.CreateInstance(actionListType, Component.CustomObject);
		}
	}
}
