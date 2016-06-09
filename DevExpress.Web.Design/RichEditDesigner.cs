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
using DevExpress.Web.ASPxRichEdit.Internal;
using DevExpress.Web.Design;
namespace DevExpress.Web.ASPxRichEdit.Design {
	public class ASPxRichEditDesigner : ASPxDataWebControlDesigner {
		ASPxRichEdit richedit;
		internal ASPxRichEdit RichEdit {
			get { return this.richedit; }
		}
		public override void Initialize(IComponent component) {
			this.richedit = (ASPxRichEdit)component;
			base.Initialize(component);
		}
		protected override bool IsControlRequireHttpHandlerRegistration() {
			return true;
		}
		public override void EnsureControlReferences() {
			base.EnsureControlReferences();
			EnsureReferences(AssemblyInfo.SRAssemblyOfficeCore,
				AssemblyInfo.SRAssemblyRichEditCore
				);
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new RichEditDesignerActionList(this);
		}
		public override void RunDesigner() {
			ShowDialog(new WrapperEditorForm(new RichEditCommonFormDesigner(RichEdit, DesignerHost)));
		}
		protected override void FillPropertyNameToCaptionMap(Dictionary<string, string> propertyNameToCaptionMap) {
			base.FillPropertyNameToCaptionMap(propertyNameToCaptionMap);
			propertyNameToCaptionMap.Add("RibbonTabs", "Ribbon Tabs");
		}
		protected override void OnBeforeControlHierarchyCompleted() {
			LoadDefaultRibbonTabs();
		}
		private void LoadDefaultRibbonTabs() {
			ASPxRichEdit editor = (ASPxRichEdit)ViewControl;
			if(editor.RibbonTabs.IsEmpty && editor.RibbonMode != RichEditRibbonMode.None && editor.RibbonMode != RichEditRibbonMode.ExternalRibbon)
				editor.RibbonTabs.CreateDefaultRibbonTabs();
		}
		public RichEditRibbonMode RibbonMode {
			get { return RichEdit.RibbonMode; }
			set {
				RichEdit.RibbonMode = value;
				PropertyChanged("RibbonMode");
			}
		}
		public bool ShowStatusBar {
			get { return RichEdit.ShowStatusBar; }
			set {
				RichEdit.ShowStatusBar = value;
				PropertyChanged("ShowStatusBar");
			}
		}
		public override void ShowAbout() {
			RichEditAboutDialogHelper.ShowAbout(Component.Site);
		}
	}
	public class RichEditDesignerActionList : ASPxWebControlDesignerActionList {
		private ASPxRichEditDesigner designer = null;
		public RichEditDesignerActionList(ASPxRichEditDesigner designer)
			: base(designer) {
			this.designer = designer;
		}
		public new ASPxRichEditDesigner Designer {
			get { return this.designer; }
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = base.GetSortedActionItems();
			collection.Add(new DesignerActionPropertyItem("RibbonMode", "Ribbon Mode", "Ribbon", string.Empty));
			if(RibbonMode == RichEditRibbonMode.ExternalRibbon) {
				collection.Add(new DesignerActionPropertyItem("AssociatedRibbonID", "Associated Ribbon ID", "Ribbon", string.Empty));
				if(!string.IsNullOrEmpty(AssociatedRibbonID)) {
					collection.Add(new DesignerActionMethodItem(this, "CreateDefaultRibbonTabs",
												   "Create Default Ribbon Tabs",
												   "Ribbon",
												   "Create Default Ribbon Tabs for External Ribbon", true));
				}				
			}			
			collection.Add(new DesignerActionPropertyItem("ShowStatusBar", "Show Status Bar", "StatusBar", string.Empty));
			return collection;
		}
		protected virtual void CreateDefaultRibbonTabs() {
			if(RibbonMode == RichEditRibbonMode.ExternalRibbon && !string.IsNullOrEmpty(Designer.RichEdit.AssociatedRibbonID)) {
				RichEditDefaultRibbon ribbon = new RichEditDefaultRibbon(Designer.RichEdit);
				RibbonDesignerHelper.AddTabCollectionToRibbonControl(Designer.RichEdit.AssociatedRibbonID, ribbon.DefaultRibbonTabs, ribbon.DefaultRibbonContextTabCategories, Designer.RichEdit);
			}
		}
		[TypeConverterAttribute(typeof(RibbonControlIDConverter))]
		public virtual string AssociatedRibbonID {
			get { return Designer.RichEdit.AssociatedRibbonID; }
			set {
				IComponent component = Designer.Component;
				TypeDescriptor.GetProperties(component)["AssociatedRibbonID"].SetValue(component, value);
			}
		}
		public virtual RichEditRibbonMode RibbonMode {
			get { return Designer.RibbonMode; }
			set { Designer.RibbonMode = value; }
		}
		public virtual bool ShowStatusBar {
			get { return Designer.ShowStatusBar; }
			set { Designer.ShowStatusBar = value; }
		}
	}
	public class RichEditCommonFormDesigner : CommonFormDesigner {
		ItemsEditorOwner itemsOwner;
		public RichEditCommonFormDesigner(ASPxRichEdit richedit, IServiceProvider provider)
			: base(richedit, provider) {
			ItemsImageIndex = RibbonItemsImageIndex;
			RichEdit = (ASPxRichEdit)Control;
		}
		public override ItemsEditorOwner ItemsOwner {
			get {
				if(itemsOwner == null && RichEdit.RibbonMode != RichEditRibbonMode.None && RichEdit.RibbonMode != RichEditRibbonMode.ExternalRibbon)
					itemsOwner = new RichEditRibbonItemsOwner(RichEdit, Provider, RichEdit.RibbonTabs);
				return itemsOwner;
			}
		}
		protected override Type DefaultItemsFrameType { get { return typeof(RichEditRibbonItemsEditorForm); } }
		ASPxRichEdit RichEdit { get; set; }
		protected override void CreateMainGroupItems() {
			base.CreateItemsItem();
			if(RichEdit.RibbonMode != RichEditRibbonMode.None && RichEdit.RibbonMode != RichEditRibbonMode.ExternalRibbon)
				CreateTabCategoriesItems();
			AddDocumentSelectorItem();
			base.CreateClientSideEventsItem();
		}
		void AddDocumentSelectorItem() {
			MainGroup.Add(CreateDesignerItem("Document Selector", "Document Selector", typeof(RichEidtDocumentSelectorFrame), RichEdit, 0, null)); 
		}
		void CreateTabCategoriesItems() {
			MainGroup.Add(CreateDesignerItem(
				new RichEditRibbonContextTabItemsOwner(RichEdit, Provider, RichEdit.RibbonContextTabCategories),
				typeof(RichEditContextTabsItemsEditorForm),
				RibbonItemsImageIndex));
		}
	}
}
