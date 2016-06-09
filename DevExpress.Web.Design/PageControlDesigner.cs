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
using System.ComponentModel.Design;
using System.Text;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Design {
	public class ASPxPageControlDesigner: ASPxTabControlDesignerBase {
		ASPxPageControl PageControl { get; set; }
		public override void Initialize(IComponent component) {
			PageControl = (ASPxPageControl)component;
			base.Initialize(component);
		}
		protected override void FillPropertyNameToCaptionMap(Dictionary<string, string> propertyNameToCaptionMap) {
			base.FillPropertyNameToCaptionMap(propertyNameToCaptionMap);
			propertyNameToCaptionMap.Add("TabPages", "TabPages");
		}
		protected override string GetBaseProperty() {
			return "TabPages";
		}
		public override void RunDesigner() {
			ShowDialog(new WrapperEditorForm(new PageControlCommonDesigner(PageControl, DesignerHost)));
		}
		public override string GetEditTabsAction() {
			return StringResources.PageControlActionList_EditTabs;
		}
		public override string GetEditTabsActionDescription() {
			return StringResources.PageControlActionList_EditTabsDescription;
		}
		protected override void AddDesignerRegions(DesignerRegionCollection regions) {
			base.AddDesignerRegions(regions);
			int regionCount = regions.Count;
			for (int i = 0; i < PageControl.TabPages.GetVisibleTabPageCount(); i++) {
				TabPage tabPage = PageControl.TabPages.GetVisibleTabPage(i);
				regions.Add(new PageControlEditableRegion(this, "Edit " + this.GetRegionName(tabPage), tabPage));
				WebControl pageContentItemControl = ViewControl.FindControl(TabControlBase.GetContentDivID(tabPage)) as WebControl;
				pageContentItemControl.Attributes[DesignerRegion.DesignerRegionAttributeName] = (regionCount + i).ToString();
			}
		}
		public override string GetEditableDesignerRegionContent(EditableDesignerRegion region) {
			PageControlEditableRegion pageControlRegion = region as PageControlEditableRegion;
			if(pageControlRegion == null)
				throw new ArgumentNullException(StringResources.InvalidRegion);
			return GetEditableDesignerRegionContent(pageControlRegion.TabPage.ContentControl.Controls);
		}
		public override void SetEditableDesignerRegionContent(EditableDesignerRegion region, string content) {
			PageControlEditableRegion pageControlRegion = region as PageControlEditableRegion;
			if(pageControlRegion == null)
				throw new ArgumentNullException(StringResources.InvalidRegion);
			SetEditableDesignerRegionContent(pageControlRegion.TabPage.ContentControl.Controls, content);
		}
		protected internal override string[] GetDataBindingSchemaFields() {
			return new string[] { "Enabled", "Text", "ToolTip" };
		}
		protected internal override Type GetDataBindingSchemaItemType() {
			return typeof(TabPage);
		}
	}
	public class PageControlEditableRegion: EditableDesignerRegion {
		private TabPage fTabPage = null;
		public TabPage TabPage {
			get { return fTabPage; }
		}
		public PageControlEditableRegion(ASPxTabControlDesignerBase designer, string name, TabPage tabPage)
			: base(designer, name, false) {
			fTabPage = tabPage;
		}
	}
	public class PageControlCommonDesigner : CommonFormDesigner {
		public PageControlCommonDesigner(ASPxPageControl pageControl, IServiceProvider provider)
			: base(new PageControlTabsOwner(pageControl, provider)) {
			ItemsImageIndex = TabPagesImageIndex;
		}
	}
	public class PageControlTabsOwner : FlatCollectionItemsOwner<TabPage> {
		public PageControlTabsOwner(ASPxPageControl pageControl, IServiceProvider provider)
			: base(pageControl, provider, pageControl.TabPages, "Tab Pages") {
		}
	}
}
