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
using System.Collections.Generic;
using DevExpress.ExpressApp.Actions;
using DevExpress.Utils;
namespace DevExpress.ExpressApp.Templates.ActionControls.Binding {
	public class ActionBindingFactory {
		sealed class FactoryEntry {
			private string name;
			private Func<ActionBase, IActionControl, bool> canCreateFunction;
			private Func<ActionBase, IActionControl, ActionBinding> createFunction;
			public FactoryEntry(string name, Func<ActionBase, IActionControl, bool> canCreateFunction, Func<ActionBase, IActionControl, ActionBinding> createFunction) {
				this.name = name;
				this.canCreateFunction = canCreateFunction;
				this.createFunction = createFunction;
			}
			public string Name {
				get { return name; }
			}
			public bool CanCreate(ActionBase action, IActionControl actionControl) {
				return canCreateFunction(action, actionControl);
			}
			public ActionBinding Create(ActionBase action, IActionControl actionControl) {
				return createFunction(action, actionControl);
			}
		}
		static ActionBindingFactory instance;
		static ActionBindingFactory() {
			Instance = new ActionBindingFactory();
		}
		public static ActionBindingFactory Instance {
			get { return instance; }
			set {
				Guard.ArgumentNotNull(value, "value");
				instance = value;
			}
		}
		private List<FactoryEntry> entries = new List<FactoryEntry>();
		public void Register(string name, Func<ActionBase, IActionControl, bool> canCreateFunction, Func<ActionBase, IActionControl, ActionBinding> createFunction) {
			Guard.ArgumentNotNull(name, "name");
			Guard.ArgumentNotNull(canCreateFunction, "canCreateFunction");
			Guard.ArgumentNotNull(createFunction, "createFunction");
			for(int i = entries.Count - 1; i >= 0; i--) {
				if(entries[i].Name == name) {
					entries.RemoveAt(i);
				}
			}
			entries.Add(new FactoryEntry(name, canCreateFunction, createFunction));
		}
		public ActionBinding Create(ActionBase action, IActionControl actionControl) {
			Guard.ArgumentNotNull(action, "action");
			Guard.ArgumentNotNull(actionControl, "actionControl");
			List<FactoryEntry> suitableEntries = entries.FindAll(entry => entry.CanCreate(action, actionControl));
			if(suitableEntries.Count == 0 || suitableEntries.Count > 1) {
				string message = string.Format("Cannot create binding between the '{0}' Action and the '{1}' Action Control because there is no suitable binding type for this pair, or there are multiple suitable bindings.", action, actionControl);
				throw new InvalidOperationException(message);
			}
			return suitableEntries[0].Create(action, actionControl);
		}
	}
}
