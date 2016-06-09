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
using DevExpress.Web.ASPxSpreadsheet.Internal;
using DevExpress.Web.Design;
namespace DevExpress.Web.ASPxSpreadsheet.Design {
	public class ASPxSpreadsheetDesigner : ASPxWebControlDesigner {
		ASPxSpreadsheet spreadsheet;
		internal ASPxSpreadsheet Spreadsheet {
			get { return this.spreadsheet; }
		}
		public override void Initialize(IComponent component) {
			this.spreadsheet = (ASPxSpreadsheet)component;
			base.Initialize(component);
		}
		protected override bool IsControlRequireHttpHandlerRegistration() {
			return true;
		}
		public override void EnsureControlReferences() {
			base.EnsureControlReferences();
			EnsureReferences(AssemblyInfo.SRAssemblyOfficeCore,
				AssemblyInfo.SRAssemblySpreadsheetCore
				);
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new SpreadsheetDesignerActionList(this);
		}
		public override void RunDesigner() {
			ShowDialog(new WrapperEditorForm(new SpreadsheetCommonFormDesigner(Spreadsheet, DesignerHost)));
		}
		protected override void FillPropertyNameToCaptionMap(Dictionary<string, string> propertyNameToCaptionMap) {
			base.FillPropertyNameToCaptionMap(propertyNameToCaptionMap);
			propertyNameToCaptionMap.Add("RibbonTabs", "Ribbon Tabs");
		}
		protected override void OnBeforeControlHierarchyCompleted() {
			LoadDefaultRibbonTabs();
		}
		private void LoadDefaultRibbonTabs() {
			ASPxSpreadsheet editor = (ASPxSpreadsheet)ViewControl;
			if(editor.RibbonTabs.IsEmpty && editor.RibbonMode == SpreadsheetRibbonMode.Ribbon)
				editor.RibbonTabs.CreateDefaultRibbonTabs();
		}
		public SpreadsheetRibbonMode RibbonMode {
			get { return Spreadsheet.RibbonMode; }
			set {
				Spreadsheet.RibbonMode = value;
				PropertyChanged("RibbonMode");
			}
		}
		public bool ShowFormulaBar {
			get { return Spreadsheet.ShowFormulaBar; }
			set {
				Spreadsheet.ShowFormulaBar = value;
				PropertyChanged("ShowFormulaBar");
			}
		}
		public override void ShowAbout() {
			SpreadsheetAboutDialogHelper.ShowAbout(Component.Site);
		}
	}
	public class SpreadsheetDesignerActionList : ASPxWebControlDesignerActionList {
		private ASPxSpreadsheetDesigner designer = null;
		public SpreadsheetDesignerActionList(ASPxSpreadsheetDesigner designer)
			: base(designer) {
			this.designer = designer;
		}
		public new ASPxSpreadsheetDesigner Designer {
			get { return this.designer; }
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = base.GetSortedActionItems();
			if(RibbonMode == SpreadsheetRibbonMode.ExternalRibbon) {
				if(!string.IsNullOrEmpty(AssociatedRibbonID)) {
					collection.Insert(0, new DesignerActionMethodItem(this, "CreateDefaultRibbonTabs",
												   "Create Default Ribbon Tabs",
												   "Ribbon",
												   "Create Default Ribbon Tabs for External Ribbon", true));
				}
				collection.Insert(0, new DesignerActionPropertyItem("AssociatedRibbonID", "Associated Ribbon ID", "Ribbon", string.Empty));
			}
			collection.Insert(0, new DesignerActionPropertyItem("ShowFormulaBar", "Show Formula Bar", "FormulaBar", string.Empty));
			collection.Insert(0, new DesignerActionPropertyItem("RibbonMode", "Ribbon Mode", "Ribbon", string.Empty));
			return collection;
		}
		protected virtual void CreateDefaultRibbonTabs() {
			if(RibbonMode == SpreadsheetRibbonMode.ExternalRibbon && !string.IsNullOrEmpty(Designer.Spreadsheet.AssociatedRibbonID)) {
				SpreadsheetDefaultRibbon defaultRibbon = new SpreadsheetDefaultRibbon(Designer.Spreadsheet);
				RibbonDesignerHelper.AddTabCollectionToRibbonControl(Designer.Spreadsheet.AssociatedRibbonID, defaultRibbon.DefaultRibbonTabs, defaultRibbon.DefaultRibbonContextTabCategories, Designer.Spreadsheet);
			}
		}
		[TypeConverterAttribute(typeof(RibbonControlIDConverter))]
		public virtual string AssociatedRibbonID {
			get { return Designer.Spreadsheet.AssociatedRibbonID; }
			set {
				IComponent component = Designer.Component;
				TypeDescriptor.GetProperties(component)["AssociatedRibbonID"].SetValue(component, value);
			}
		}
		public virtual SpreadsheetRibbonMode RibbonMode {
			get { return Designer.RibbonMode; }
			set { Designer.RibbonMode = value; }
		}
		public virtual bool ShowFormulaBar {
			get { return Designer.ShowFormulaBar; }
			set { Designer.ShowFormulaBar = value; }
		}
	}
	public class SpreadsheetCommonFormDesigner : CommonFormDesigner {
		ItemsEditorOwner itemsOwner;
		ASPxSpreadsheet spreadsheet = null;
		public SpreadsheetCommonFormDesigner(ASPxSpreadsheet spreadsheet, IServiceProvider provider)
			: base(spreadsheet, provider) {
			ItemsImageIndex = RibbonItemsImageIndex;
		}
		ASPxSpreadsheet Spreadsheet {
			get {
				if(spreadsheet == null)
					spreadsheet = (ASPxSpreadsheet)Control;
				return spreadsheet;
			}
		}
		protected override Type DefaultItemsFrameType { get { return typeof(SpreadsheetRibbonItemsEditorForm); } }
		private bool CanCreateRibbonTabsViaDesigner {
			get {
				return Spreadsheet.RibbonMode != SpreadsheetRibbonMode.None && Spreadsheet.RibbonMode != SpreadsheetRibbonMode.ExternalRibbon;
			}
		}
		public override ItemsEditorOwner ItemsOwner {
			get {
				if(itemsOwner == null && CanCreateRibbonTabsViaDesigner)
					itemsOwner = new SpreadsheetRibbonItemsOwner(Spreadsheet, Provider, Spreadsheet.RibbonTabs);
				return itemsOwner;
			}
		}
		protected override void CreateMainGroupItems() {
			base.CreateItemsItem();
			if(CanCreateRibbonTabsViaDesigner)
				CreateTabCategoriesItems();
			AddDocumentSelectorItem();
			base.CreateClientSideEventsItem();
		}
		void AddDocumentSelectorItem() {
			MainGroup.Add(CreateDesignerItem("Document Selector", "Document Selector", typeof(SpreadsheetDocumentSelectorFrame), Spreadsheet, 0, null)); 
		}
		void CreateTabCategoriesItems() {
			MainGroup.Add(CreateDesignerItem(
				new SpreadsheetRibbonContextTabCategoriesOwner(Spreadsheet, Provider, Spreadsheet.RibbonContextTabCategories),
				typeof(SpreadsheetContextTabsEditorForm),
				RibbonItemsImageIndex));
		}
	}
}
