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
using System.Drawing;
namespace DevExpress.XtraBars.Docking2010.DragEngine {
	public static class ElementHelper {
		public static IUIElement GetRoot(IUIElement element) {
			if(element == null) return null;
			if(element.Scope == null) return element;
			IUIElement root = element;
			while(root.Scope != null) {
				root = root.Scope;
			}
			return root;
		}
		public static ILayoutElement GetRoot(ILayoutElement element) {
			if(element == null) return null;
			if(element.Parent == null) return element;
			ILayoutElement root = element;
			while(root.Parent != null) {
				root = root.Parent;
			}
			return root;
		}
		public static Rectangle GetScreenRect(ILayoutElementHost host) {
			ILayoutElement rootElement = host.LayoutRoot;
			return new Rectangle(host.ClientToScreen(rootElement.Location), rootElement.Size);
		}
		public static Rectangle GetScreenRect(ILayoutElementHost host, ILayoutElement element) {
			return new Rectangle(host.ClientToScreen(element.Location), element.Size);
		}
	}
}
