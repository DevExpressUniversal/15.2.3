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
using System.Text;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraBars;
using System.ComponentModel;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	public class LookAndFeelUtils {
		private static void SetUserLookAndFeelStyle(UserLookAndFeel lookAndFeel, bool setXPWindowsStyle) {
			if(setXPWindowsStyle) {
				lookAndFeel.SetWindowsXPStyle();
			}
			else {
				lookAndFeel.SetStyle(UserLookAndFeel.Default.Style, UserLookAndFeel.Default.UseWindowsXPTheme, UserLookAndFeel.Default.UseDefaultLookAndFeel, UserLookAndFeel.Default.SkinName);
			}
		}
		private static void SetWindowsStyle(Control parentControl, bool setXPWindowsStyle) {
			if(parentControl is ISupportLookAndFeel) {
				SetUserLookAndFeelStyle(((ISupportLookAndFeel)parentControl).LookAndFeel, setXPWindowsStyle);				
			}
			if(parentControl.Container != null) {
				foreach(Component component in parentControl.Container.Components) {
					if(component is BarManager) {
						SetUserLookAndFeelStyle((component as BarManager).GetController().LookAndFeel, setXPWindowsStyle);
					}
				}
			}
			foreach(Control control in parentControl.Controls) {
				if(control is ISupportLookAndFeel) {
					SetUserLookAndFeelStyle(((ISupportLookAndFeel)control).LookAndFeel, setXPWindowsStyle);
				}
				BarDockControl barDocControl = control as BarDockControl;
				if(barDocControl != null && barDocControl.Manager != null && barDocControl.Manager.GetController() != null
					&& (barDocControl.Manager.GetController().LookAndFeel != null)) {
					SetUserLookAndFeelStyle(((BarDockControl)control).Manager.GetController().LookAndFeel, setXPWindowsStyle);
				}
				SetWindowsStyle(control, setXPWindowsStyle);
			}
		}
		public static bool useXPStyle = false;
		public static void ApplyStyle(Control parentControl) {
			if(useXPStyle) {
				SetXPWindowsStyle(parentControl);
			}
		}
		public static void SetXPWindowsStyle(Control parentControl) {
			SetWindowsStyle(parentControl, true);
		}
	}
}
