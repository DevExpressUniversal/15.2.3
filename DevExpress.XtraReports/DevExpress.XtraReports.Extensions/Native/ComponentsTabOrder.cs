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
using System.Text;
using DevExpress.XtraReports.UI;
using System.Collections;
namespace DevExpress.XtraReports.Native {
	public static class ComponentsTabOrder {
		static ArrayList MakeOrderedComponentsList(XRControlCollection collection) {
			ArrayList components = new ArrayList(collection);
			components.Sort(new DevExpress.XtraReports.Design.ReportExplorerController.ComponentComparer());
			return components;
		}
		static XRControl FindNextSibling(XRControl control) {
			XRControl parent = control.Parent;
			if(parent == null)
				return control as XRControl;
			ArrayList components = MakeOrderedComponentsList(parent.Controls);
			int i = components.IndexOf(control) + 1;
			return i < components.Count ? (components[i] as XRControl) : FindNextSibling(parent);
		}
		static XRControl FindLast(XRControl control) {
			if(control == null)
				return null;
			if(control.Controls.Count <= 0)
				return control;
			ArrayList components = MakeOrderedComponentsList(control.Controls);
			return FindLast(components[components.Count - 1] as XRControl);
		}
		static XRControl FindPrevSibling(XRControl control) {
			XRControl parent = control.Parent;
			if(parent == null)
				return FindLast(control);
			ArrayList components = MakeOrderedComponentsList(parent.Controls);
			int i = components.IndexOf(control) - 1;
			return i >= 0 ? FindLast(components[i] as XRControl) : parent;
		}
		public static XRControl GetNextComponent(XRControl control) {
			if(control == null)
				return null;
			if(control.Controls.Count > 0) {
				ArrayList components = MakeOrderedComponentsList(control.Controls);
				return components[0] as XRControl;
			}
			return FindNextSibling(control);
		}
		public static XRControl GetPrevComponent(XRControl control) {
			return (control != null) ? FindPrevSibling(control) : null;
		}
	}
}
