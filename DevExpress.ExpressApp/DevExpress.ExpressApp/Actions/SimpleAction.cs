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
using System.ComponentModel;
using System.Drawing;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Actions {
	public class SimpleActionExecuteEventArgs : ActionBaseEventArgs {
		private object currentObject;
		private ArrayList selectedObjects;
		public SimpleActionExecuteEventArgs(ActionBase action, ISelectionContext context)
			: base(action) {
			if(context != null) {
				currentObject = context.CurrentObject;
				selectedObjects = new ArrayList();
				if(context.SelectedObjects != null) {
					selectedObjects.AddRange(context.SelectedObjects);
				}
			}
			else {
				selectedObjects = new ArrayList();
			}
		}
		public IList SelectedObjects {
			get { return selectedObjects; }
		}
		public object CurrentObject {
			get { return currentObject; }
		}
	}
	public delegate void SimpleActionExecuteEventHandler(Object sender, SimpleActionExecuteEventArgs e);
	[DXToolboxItem(true)]
	[ToolboxBitmap(typeof(XafApplication), "Resources.Actions.Action_SimpleAction.bmp")]
	[DevExpress.Utils.ToolboxTabName(XafAssemblyInfo.DXTabXafActions)]
	public class SimpleAction : ActionBase {
		public SimpleAction() : this(null) { }
		public SimpleAction(IContainer container) : base(container) { }
		public SimpleAction(Controller owner, string id, string category) : base(owner, id, category) { }
		public SimpleAction(Controller owner, string id, PredefinedCategory category)
			: this(owner, id, category.ToString()) { }
		public SimpleAction(Controller owner, string id, string category, SimpleActionExecuteEventHandler execute)
			: this(owner, id, category) {
			Execute += execute;
		}
		public bool DoExecute() {
			return ExecuteCore(Execute, new SimpleActionExecuteEventArgs(this, SelectionContext));
		}
		protected internal override void RaiseExecute(ActionBaseEventArgs eventArgs) {
			Execute(this, (SimpleActionExecuteEventArgs)eventArgs);
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("SimpleActionExecute"),
#endif
 Category("Action")]
		public event SimpleActionExecuteEventHandler Execute;
	}
}
