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

using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Design;
namespace DevExpress.XtraBars.Navigation {
	#region NavigationFrame
	public class NavigationFrameBounds : ControlBoundsProvider {
		public override Control GetOwnerControl(IComponent component) {
			return component as NavigationFrame;
		}
	}
	public class NavigationFrameFilter : ControlFilter {
		protected override bool AllowDock { get { return true; } }
	}
	public class NavigationFrameActions : ControlActions {
		public void Pages(IComponent component) {
			EditorContextHelper.EditValue(GetDesigner(component), component, "Pages");
		}
		public void BringToFront(IComponent component) {
			(component as NavigationFrame).BringToFront();
		}
		public void SendToBack(IComponent component) {
			(component as NavigationFrame).SendToBack();
		}
		protected NavigationPageBase AddPage(NavigationFrame navigationFrame) {
			NavigationPageBase page = navigationFrame.CreateNewPage();
			navigationFrame.Pages.Add(page);
			navigationFrame.Container.Add(page);
			page.Text = page.Name;
			return page;
		}
		public void AddNavigationPage(IComponent component) {
			NavigationFrame navigationFrame = component as NavigationFrame;
			if(navigationFrame != null) {
				NavigationPageBase page = AddPage(navigationFrame);
				try {
					(navigationFrame as INavigationFrame).SelectedPage = page;
				}
				finally { }
			}
		}
		public void RemoveNavigationPage(IComponent component) {
			NavigationFrame NavigationFrame = component as NavigationFrame;
			if(NavigationFrame != null) {
				INavigationPageBase page = (NavigationFrame as INavigationFrame).SelectedPage as NavigationPageBase;
				if(page == null) return;
				int index = NavigationFrame.Pages.IndexOf(page as NavigationPageBase);
				page.Dispose();
				index = index < NavigationFrame.Pages.Count - 1 ? index : NavigationFrame.Pages.Count - 1;
				if(index >= 0)
					(NavigationFrame as INavigationFrame).SelectedPage = NavigationFrame.Pages[index];
			}
		}
	}
	#endregion
	#region NavigationPage
	public class NavigationPageActions : ControlActions {
		public void CustomHeaderButtons(IComponent component) {
			EditorContextHelper.EditValue(GetDesigner(component), component, "CustomHeaderButtons");
		}
	}
	public class NavigationPageBounds : ControlBoundsProvider {
		public override Control GetOwnerControl(IComponent component) {
			return component as NavigationPageBase;
		}
	}
	public class NavigationPageFilter : ControlFilter {
		protected override bool AllowDock { get { return false; } }
		public override bool FilterProperty(MemberDescriptor descriptor) {
			if(descriptor.Name == "AutoSize" ||
			descriptor.Name == "ScrollBarSmallChange" ||
			descriptor.Name == "AlwaysScrollActiveControlIntoView") return false;
			return base.FilterProperty(descriptor);
		}
	}
	#endregion
}
