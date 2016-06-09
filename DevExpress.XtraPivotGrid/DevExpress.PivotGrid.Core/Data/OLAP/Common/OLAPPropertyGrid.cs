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

using DevExpress.Compatibility.System.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
namespace DevExpress.XtraPivotGrid.Design {
	[ToolboxItem(false)]
	public class OLAPPropertyGrid : PropertyGrid {
		ToolStripButton[] viewTabButtonsCache;
		PropertyTab[] viewTabsCache;
		public void AddTabs() {
			PropertyTabs.AddTabType(typeof(BasicTab));
			PropertyTabs.AddTabType(typeof(AdvancedTab));
		}
		ToolStripButton[] ViewTabButtons {
			get {
				if(viewTabButtonsCache == null) {
					FieldInfo fInfo = typeof(PropertyGrid).GetField("viewTabButtons", BindingFlags.Instance | BindingFlags.NonPublic);
					viewTabButtonsCache = (ToolStripButton[])fInfo.GetValue(this);
				}
				return viewTabButtonsCache;
			}
		}
		PropertyTab[] ViewTabs {
			get {
				if(viewTabsCache == null) {
					FieldInfo fInfo = typeof(PropertyGrid).GetField("viewTabs", BindingFlags.Instance | BindingFlags.NonPublic);
					viewTabsCache = (PropertyTab[])fInfo.GetValue(this);
				}
				return viewTabsCache;
			}
		}
		public new Type SelectedTab {
			get {
				if(base.SelectedTab == null) return null;
				return base.SelectedTab.GetType();
			}
			set {
				int tabIndex = FindTabIndex(value);
				if(tabIndex < 0) return;
				MethodInfo mInfo = typeof(PropertyGrid).GetMethod("SelectViewTabButtonDefault", BindingFlags.Instance | BindingFlags.NonPublic);
				mInfo.Invoke(this, new object[] { ViewTabButtons[tabIndex] });
				Refresh();
			}
		}
		protected override PropertyTab CreatePropertyTab(Type tabType) {
			viewTabButtonsCache = null;
			viewTabsCache = null;
			return (PropertyTab)Activator.CreateInstance(tabType);
		}
		int FindTabIndex(Type type) {
			for(int i = 0; i < ViewTabs.Length; i++) {
				if(ViewTabs[i].GetType() == type)
					return i;
			}
			return -1;
		}
	}
	public class BasicTab : PropertyTab {
		Bitmap bitmap;
		public override PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes) {
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(component.GetType(), new Attribute[] { new DevExpress.XtraPivotGrid.Data.BasicAttribute() });
			return properties.Sort(DevExpress.XtraPivotGrid.Data.OLAPConnectionStringBuilder.PropertiesOrder);
		}
		public override string TabName {
			get { return "Basic"; }
		}
		public override Bitmap Bitmap {
			get {
				if(bitmap == null)
					bitmap = new Bitmap(16, 16);
				return bitmap;
			}
		}
	}
	public class AdvancedTab : PropertyTab {
		Bitmap bitmap;
		public override PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes) {
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(component, attributes);
			return properties.Sort(DevExpress.XtraPivotGrid.Data.OLAPConnectionStringBuilder.PropertiesOrder);
		}
		public override string TabName {
			get { return "Advanced"; }
		}
		public override Bitmap Bitmap {
			get {
				if(bitmap == null)
					bitmap = new Bitmap(16, 16);
				return bitmap;
			}
		}
	}
}
