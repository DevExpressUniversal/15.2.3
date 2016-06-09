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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
namespace DevExpress.Utils {
	public class ComponentLocator {
		static System.Collections.Generic.IList<Func<Control, Component>> findRoutines;
		public static void RegisterFindRoutine(Func<Control, Component> routine) {
			if(findRoutines == null)
				findRoutines = new System.Collections.Generic.List<Func<Control, Component>>();
			findRoutines.Add(routine);
		}
		public static IEnumerable<Component> FindComponents(Control container) {
			if(findRoutines == null) yield break;
			for(int i = 0; i < findRoutines.Count; i++) {
				Component component = findRoutines[i](container);
				if(component != null)
					yield return component;
			}
		}
	}
	class ComponentTreeNode {
		IComponent componentCore;
		public ComponentTreeNode() {
			OnCreate();
		}
		public ComponentTreeNode(ComponentTreeNode parent) : this() {
			Parent = parent;
			if(Parent != null) {
				Key = Parent.Key;
				ExtendedKey = Parent.ExtendedKey;
				Parent.Nodes.Add(this);
			}
		}
		void OnCreate() {
			Nodes =  new List<ComponentTreeNode>();
		}
		public string Key { get; protected set; }
		public string ExtendedKey { get; protected set; }
		public ComponentTreeNode Parent { get; protected set; }
		public IComponent Component {
			get { return componentCore; }
			set {
				componentCore = value;
				UpdateKey();
			}
		}
		void UpdateKey() {
			var xtraSerializableChildren = componentCore as IXtraSerializableChildren;
			var control = componentCore as Control;
			Key = componentCore.GetType().Name + "_" + Key;
			ExtendedKey = componentCore.GetType().Name + "_" + ExtendedKey;
			if(xtraSerializableChildren != null) {
				Key = xtraSerializableChildren.Name + "_" + Key;
				ExtendedKey = xtraSerializableChildren.Name + "_" + ExtendedKey;
			}
			if(control != null) {
				ExtendedKey = control.Name + "_" + ExtendedKey;
			}
		}
		public IList<ComponentTreeNode> Nodes { get; protected set; }
	}
}
