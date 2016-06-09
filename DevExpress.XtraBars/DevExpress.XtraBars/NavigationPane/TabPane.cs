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
using System.Linq;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraEditors.ButtonPanel;
namespace DevExpress.XtraBars.Navigation {
	public interface ITabPane : INavigationPane { }
	[DXToolboxItem(true), Description("A navigation control with horizontally aligned page headers.")]
	[ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "Navigation.TabPane")]
	[DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabNavigation)]
	[Designer("DevExpress.XtraBars.Design.TabPaneDesigner, " + AssemblyInfo.SRAssemblyBarsDesign)]
	public class TabPane : NavigationPane, ITabPane {
		IBaseNavigationPageProperties pagePropertiesCore;
		public TabPane() : base() {
			pagePropertiesCore = CreateBaseNavigationPageProperties();
			PageProperties.Changed += OnPagePropertiesChanged;
		}
		void OnPagePropertiesChanged(object sender, EventArgs e) {
			LayoutChanged();
		}
		protected override Orientation GetOrientation() { 
			return Orientation.Horizontal;
		}
		protected IBaseNavigationPageProperties CreateBaseNavigationPageProperties() {
			return new BaseNavigationPageProperties();
		}
		[Category("Behavior")]
#if !SL
	[DevExpressXtraBarsLocalizedDescription("NavigationFrameSelectedPage")]
#endif
		public new TabNavigationPage SelectedPage {
			get { return SelectedPageCore as TabNavigationPage; }
			set { SelectedPageCore = value; }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("NavigationPanePageProperties")]
#endif
		[Category("Layout"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), SmartTagSearchNestedProperties]
		public new IBaseNavigationPageProperties PageProperties {
			get { return pagePropertiesCore; }
		}
		protected override void OnDockChanged(EventArgs e) {
			base.OnDockChanged(e);
			ButtonsPanel.Orientation = GetOrientation();
			if(!(this as INavigationFrame).IsRightToLeft())
				ButtonsPanel.ContentAlignment = ContentAlignment.TopLeft;
			else
				ButtonsPanel.ContentAlignment = ContentAlignment.TopRight;
		}
		protected override NavigationPaneViewInfo CreateViewInfo() {
			return new TabPaneViewInfo(this);
		}
		public override void LayoutChanged() {
			base.LayoutChanged();
			InvalidateNC();
		}
		protected override void OnPageAdded(NavigationPageBase navigationPage) {
			if(navigationPage is TabNavigationPage)
			(navigationPage  as TabNavigationPage).Properties.EnsureParentProperties(PageProperties);
			base.OnPageAdded(navigationPage);
		}
		protected override void Dispose(bool disposing) {
			if(PageProperties != null) {
				PageProperties.Changed -= OnPagePropertiesChanged;
				PageProperties.Dispose();
			}
			base.Dispose(disposing);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override NavigationPaneState State {
			get { return NavigationPaneState.Default; }
			set {}
		}
		protected override ButtonsPanel CreateButtonsPanel() {
			return new TabPaneButtonsPanel(this);
		}
		protected override void DoCollapse(NavigationPaneState oldState) {
		}
		public override ObjectPainter GetPainter() {
			return new TabPaneButtonsPanelSkinPainter(LookAndFeel);
		}
		protected override NavigationPanePainter CreatePainter() {
			return new TabPaneSkinPainter(LookAndFeel);
		}
		public override NavigationPageBase CreateNewPage() {
			return new TabNavigationPage();
		}
	}
}
