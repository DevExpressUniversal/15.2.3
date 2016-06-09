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

using DevExpress.XtraRichEdit.Design;
using System.ComponentModel.Design;
using DevExpress.XtraBars.Design;
using DevExpress.Snap.Extensions;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraRichEdit.UI;
using DevExpress.XtraCharts.UI;
using DevExpress.Snap.Extensions.UI;
namespace DevExpress.Snap.Design {
	public class SnapBarControllerDesigner : RichEditBarControllerDesigner {
		IComponentChangeService changeService;
		SnapBarController SnapBarController { get { return Component as SnapBarController; } }
		IComponentChangeService ChangeService { get { return changeService; } }
		public override void Initialize(System.ComponentModel.IComponent component) {
			base.Initialize(component);
			IDesignerHost designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
			if (designerHost == null)
				return;
			SnapDockManager snapDockManager = DesignHelpers.FindComponent(designerHost.Container, typeof(SnapDockManager)) as SnapDockManager;
			if (snapDockManager != null)
				SnapBarController.SnapDockManager = snapDockManager;
			RibbonControl ribbonControl = DesignHelpers.FindComponent(designerHost.Container, typeof(RibbonControl)) as RibbonControl;
			if (ribbonControl != null)
				SnapBarController.RibbonControl = ribbonControl;
			this.changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if (changeService != null)
				changeService.ComponentAdded += new ComponentEventHandler(ComponentAdded);
		}
		void ComponentAdded(object sender, ComponentEventArgs e) {
			if (ChangeService == null)
				return;
			if (e.Component is TableToolsRibbonPageCategory ||
				e.Component is FloatingPictureToolsRibbonPageCategory ||
				e.Component is HeaderFooterToolsRibbonPageCategory ||
				e.Component is ChartRibbonPageCategory ||
				e.Component is DataToolsRibbonPageCategory 
				)
				AddContextRibbonPageCategory((RibbonPageCategory)e.Component);
			if(e.Component is RibbonControl && !object.ReferenceEquals(SnapBarController, null))
				SnapBarController.RibbonControl = ((RibbonControl)e.Component);
		}
		void AddContextRibbonPageCategory(RibbonPageCategory category) {
			if (SnapBarController == null)
				return;
			ChangeService.OnComponentChanging(SnapBarController, null);
			ContextRibbonPageCategoryItem contextCategory = new ContextRibbonPageCategoryItem(category);
			SnapBarController.ContextPageCategories.Add(contextCategory);
			ChangeService.OnComponentChanged(SnapBarController, null, null, null);
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (disposing && ChangeService != null)
				ChangeService.ComponentAdded -= ComponentAdded;
		}
	}
}
