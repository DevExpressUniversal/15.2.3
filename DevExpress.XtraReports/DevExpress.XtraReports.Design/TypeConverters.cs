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
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraBars;
using DevExpress.XtraNavBar;
using DevExpress.XtraNavBar.ViewInfo;
using System.ComponentModel;
using System.Collections;
using System.Reflection;
namespace DevExpress.XtraReports.Design
{
	public class NavBarViewNamesConverter : TypeConverter {
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			if(context == null || context.Instance == null) return null;
			ToolBoxDockPanel panel = context.Instance as ToolBoxDockPanel;
			if(panel == null) return null;
			XRDesignToolBox toolBox = GetXRDesignToolBox(panel);
			if(toolBox == null) return null;
			PopulateDesignTimeViews(toolBox);
			ArrayList list = new ArrayList();
			list.Add(NavBarControl.DefaultPaintStyleName);
			foreach(BaseViewInfoRegistrator info in toolBox.AvailableNavBarViews) {
				list.Add(info.ViewName);
			}
			return new StandardValuesCollection(list);
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return context != null && context.Instance != null;
		}
		private static void PopulateDesignTimeViews(NavBarControl navBar) {
			MethodInfo mi = typeof(NavBarControl).GetMethod("PopulateDesignTimeViews", BindingFlags.NonPublic | BindingFlags.Instance);
			if(mi != null) mi.Invoke(navBar, null);
		}
		XRDesignToolBox GetXRDesignToolBox(ToolBoxDockPanel panel) {
			if(panel.ControlContainer.Controls.Count > 0)
				return panel.ControlContainer.Controls[0] as XRDesignToolBox;
			return null;
		}
	}
}
