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
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.LookAndFeel;
using System.Collections;
using System.Collections.Generic;
using DevExpress.Utils.Design;
namespace DevExpress.XtraLayout {
	public class HiddenItemsListDesigner : BaseCustomizationFormControlDesigner {
		public HiddenItemsListDesigner() { }
	}
	public class LayoutTreeViewDesigner : BaseCustomizationFormControlDesigner {
		public LayoutTreeViewDesigner() { }
	}
	public class CustomizationPropertyGridDesigner : BaseCustomizationFormControlDesigner {
		public CustomizationPropertyGridDesigner() { }
	}
	public class ButtonsPanelDesigner : BaseCustomizationFormControlDesigner {
		public ButtonsPanelDesigner() { }
	}
	public class BaseCustomizationFormControlDesigner : DevExpress.Utils.Design.BaseControlDesigner {
		protected Hashtable allowedProps = new Hashtable();
		public BaseCustomizationFormControlDesigner() {
			allowedProps.Add("Size", "Size");
			allowedProps.Add("Location", "Location");
			allowedProps.Add("Name", "Name");
			allowedProps.Add("X", "X");
			allowedProps.Add("Y", "Y");
			allowedProps.Add("Width", "Width");
			allowedProps.Add("Height", "Height");
			allowedProps.Add("Role", "Role");
			allowedProps.Add("ShowHiddenItemsInTreeView", "ShowHiddenItemsInTreeView");
			allowedProps.Add("Modifiers", "Modifiers");
			allowedProps.Add("Dock", "Dock");
			allowedProps.Add("Anchor", "Anchor");
			allowedProps.Add("Padding", "Padding");
			allowedProps.Add("Margin", "Margin");
		}
		protected override void PostFilterProperties(IDictionary properties) {
			base.PostFilterProperties(properties);
			DevExpress.Utils.Design.DXPropertyDescriptor.ConvertDescriptors(properties, null);
			object[] keys = new object[properties.Count];
			int n = 0;
			foreach(object key in properties.Keys) {
				keys[n++] = key;
			}
			foreach(object key in keys) {
				PropertyDescriptor desc = properties[key] as PropertyDescriptor;
				if(desc != null && !allowedProps.Contains(desc.Name)) properties.Remove(key);
			}
		}
	}
}
