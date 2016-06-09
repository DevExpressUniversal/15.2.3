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

using DevExpress.Utils.Design;
using DevExpress.XtraEditors;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.XtraBars {
	public class TabFormPageDesignTimeBoundsProvider : ISmartTagClientBoundsProvider {
		public Rectangle GetBounds(IComponent component) {
			TabFormPage page = component as TabFormPage;
			if(page == null) return Rectangle.Empty;
			return page.Owner.ViewInfo.GetPageInfo(page).Bounds;
		}
		public Control GetOwnerControl(IComponent component) {
			TabFormPage page = (TabFormPage)component;
			return page.Owner;
		}
	}
	public class TabFormControlDesignTimeManager : BaseDesignTimeManager {
		public TabFormControlDesignTimeManager(TabFormControlBase control)
			: base(control, null) {
		}
		public override void InitSelectionService() {
			base.InitSelectionService();
			if(SelectionService != null) {
				SelectionService.SelectionChanging += OnSelectionChanging;
			}
		}
		void OnSelectionChanging(object sender, EventArgs e) {
			ISelectionService serv = sender as ISelectionService;
			XtraScrollableControl ctrl = serv.PrimarySelection as XtraScrollableControl;
			if(ctrl != null && ctrl.Parent is TabFormControl) {
				serv.SetSelectedComponents(new IComponent[] { ctrl.Parent }, SelectionTypes.Auto);
			}
		}
		public TabFormControl TabFormControl { get { return Owner as TabFormControl; } }
		public override ISite Site { get { return TabFormControl == null ? null : TabFormControl.Site; } }
		public override void InvalidateComponent(object component) {
			TabFormControl.Invalidate(false);
		}
		protected override void OnDesignTimeSelectionChanged(object component) {
			TabFormPage page = component as TabFormPage;
			if(page != null && TabFormControl.Equals(page.Owner)) InvalidateComponent(page);
		}
		public void AddPage(TabFormPage page) {
			if(DesignerHost == null) return;
			DesignerHost.Container.Add(page);
		}
		public override void Dispose() {
			if(SelectionService != null) {
				SelectionService.SelectionChanging -= OnSelectionChanging;
			}
			base.Dispose();
		}
	}
}
