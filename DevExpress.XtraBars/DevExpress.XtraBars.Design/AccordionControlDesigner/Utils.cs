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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars.Navigation;
using DevExpress.Utils.Design;
using System.Reflection;
namespace DevExpress.XtraBars.Navigation.Frames {
	public class AccordionDragInfo {
		AccordionControlElement dragElement;
		AccordionControlElement[] dragElements;
		public AccordionDragInfo(AccordionControlElement[] elements) {
			this.dragElements = elements;
			this.dragElement = null;
		}
		public AccordionDragInfo(TreeNode[] nodes, AccordionControlElementCollection nodesCollection) {
			this.dragElement = null;
			this.dragElements = new AccordionControlElement[nodes.Length];
			for(int i = 0; i < nodes.Length; i++)
				this.dragElements[i] = nodes[i].Tag as AccordionControlElement;
		}
		public AccordionControlElement[] DragElements {
			get {
				return dragElements;
			}
		}
		public AccordionControlElement Element { get { return dragElement; } }
		public bool IsElementDragging { get { return Element != null; } }
	}
	public class AccordionDesignerTreeView : DXTreeView {
		protected override bool UseThemes { get { return true; } }
	}
	internal class SkinHelper {
		public static bool InitSkins(IServiceProvider sp) {
			Type type = ProjectHelper.TryLoadType(sp, Info.TypeName);
			if(type == null) return false;
			MethodInfo mi = type.GetMethod(Info.MethodName, BindingFlags.Public | BindingFlags.Static);
			if(mi == null) return false;
			mi.Invoke(null, null);
			return true;
		}
		static SkinInitTypeInfo Info { get { return new SkinInitTypeInfo("DevExpress.UserSkins.BonusSkins", "Register"); } }
	}
	internal class SkinInitTypeInfo {
		public SkinInitTypeInfo(string typeName, string methodName) {
			this.TypeName = typeName;
			this.MethodName = methodName;
		}
		public string TypeName { get; private set; }
		public string MethodName { get; private set; }
	}
}
