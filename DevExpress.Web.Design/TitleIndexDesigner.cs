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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Design {
	public class ASPxTitleIndexDesigner : ASPxDataWebControlDesigner {
		private static string[] fControlTemplateNames = new string[] { "GroupHeaderTemplate", "IndexPanelItemTemplate", "ItemTemplate", "ColumnSeparatorTemplate" };
		private ASPxTitleIndex fTitleIndexControl = null;
		public ASPxTitleIndex TitleIndexControl {
			get { return fTitleIndexControl; }
		}
		public override void Initialize(IComponent component) {
			fTitleIndexControl = (ASPxTitleIndex)component;
			base.Initialize(component);			
			SetViewFlags(ViewFlags.TemplateEditing, true);
		}
		protected override void FillPropertyNameToCaptionMap(Dictionary<string, string> propertyNameToCaptionMap) {
			base.FillPropertyNameToCaptionMap(propertyNameToCaptionMap);
			propertyNameToCaptionMap.Add("Columns", "Columns");
			propertyNameToCaptionMap.Add("Items", "Items");
		}
		protected override void DataBind(ASPxDataWebControlBase dataControl) {
			ASPxTitleIndex titleIndexControl = dataControl as ASPxTitleIndex;
			if (!string.IsNullOrEmpty(titleIndexControl.DataSourceID) || (titleIndexControl.DataSource != null) || titleIndexControl.Items.Count < 1) {
				titleIndexControl.ClearItems();
				base.DataBind(titleIndexControl);
			}
		}
		protected override IEnumerable GetSampleDataSource() {
			return new TitleIndexSampleData();
		}
		protected override TemplateGroupCollection CreateTemplateGroups() {
			TemplateGroupCollection templateGroups = base.CreateTemplateGroups();
			for(int i = 0; i < fControlTemplateNames.Length; i++) {
				TemplateGroup templateGroup = new TemplateGroup(fControlTemplateNames[i]);
				TemplateDefinition templateDefinition = new TemplateDefinition(this, fControlTemplateNames[i],
					Component, fControlTemplateNames[i], GetTemplateStyle(-1, fControlTemplateNames[i]));
				templateDefinition.SupportsDataBinding = true;
				templateGroup.AddTemplateDefinition(templateDefinition);
				templateGroups.Add(templateGroup);
			}
			return templateGroups;
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new TitleIndexDesignerActionList(this);
		}
		public override void RunDesigner() {
			ShowDialog(new WrapperEditorForm(new TitleIndexCommonFormDesigner(TitleIndexControl, DesignerHost)));
		}
		protected Style GetTemplateStyle(int level, string templateName) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(TitleIndexControl.GetControlStyle());
			switch (templateName) {
				case "GroupHeaderTemplate":
					style.CopyFrom(TitleIndexControl.GetItemStyleInternal(0));
					break;
				case "ItemTemplate":
					style.CopyFrom(TitleIndexControl.GetItemStyleInternal(1));
					break;
				case "IndexPanelItemTemplate":
					style.CopyFrom(TitleIndexControl.GetIndexPanelItemStyleInternal());
					break;
				case "ColumnSeparatorTemplate":
					style.CopyFrom(TitleIndexControl.GetColumnSeparatorStyle());
					break;
			}
			return style;
		}
		protected internal override string[] GetDataBindingSchemaFields() {
			return new string[] { "Text", "NavigateUrl", "GroupValue", "Description", "Name" };
		}
		protected internal override Type GetDataBindingSchemaItemType() {
			return typeof(TitleIndexItem);
		}
	}
	public class TitleIndexDesignerActionList : ASPxWebControlDesignerActionList {
		private ASPxTitleIndexDesigner fTitleIndexDesigner;
		public byte ColumnCount {
			get { return fTitleIndexDesigner.TitleIndexControl.ColumnCount; }
			set {
				IComponent component = fTitleIndexDesigner.Component;
				TypeDescriptor.GetProperties(component)["ColumnCount"].SetValue(component, value); 
			}
		}
		public TitleIndexDesignerActionList(ASPxTitleIndexDesigner titleIndexDesigner)
			: base(titleIndexDesigner) {
			fTitleIndexDesigner = titleIndexDesigner;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = base.GetSortedActionItems();
			collection.Add(new DesignerActionPropertyItem("ColumnCount",
				StringResources.TitleIndexActionList_EditColumnCount,
				StringResources.TitleIndexActionList_EditColumnCount,
				StringResources.TitleIndexControlActionList_EditColumnCountDescription));
			return collection;
		}
	}
	public class TitleIndexCommonFormDesigner : CommonFormDesigner {
		public TitleIndexCommonFormDesigner(ASPxTitleIndex titleIndex, IServiceProvider provider) 
			: base(new FlatCollectionItemsOwner<TitleIndexItem>(titleIndex, provider, titleIndex.Items)) {
		}
		ASPxTitleIndex TitleIndex { get { return (ASPxTitleIndex)Control; } }
		protected override void CreateMainGroupItems() {
			base.CreateMainGroupItems();
			AddColumnsItem();
		}
		void AddColumnsItem() {
			var columnsOwner = new TitleIndexColumnsOwner(TitleIndex, Provider);
			var insertBefore = MainGroup.IndexOf(MainGroup.GetItemByCaption(ClientSideEventsCaption));
			MainGroup.Insert(insertBefore, CreateDesignerItem("Columns", "Columns", typeof(ItemsEditorFrame), TitleIndex, ColumnsItemImageIndex, columnsOwner));
		}
	}
	public class TitleIndexColumnsOwner : FlatCollectionItemsOwner<TitleIndexColumn> {
		public TitleIndexColumnsOwner(ASPxTitleIndex titleIndex, IServiceProvider provider) 
			: base(titleIndex, provider, titleIndex.Columns) {
		}
		protected override List<DesignEditorMenuRootItemActionType> GetToolbarActionTypes() {
			var result = base.GetToolbarActionTypes();
			result.Add(DesignEditorMenuRootItemActionType.SetItemsAmount);
			return result;
		}
		protected internal override string GetNavBarItemsGroupName() {
			return "Columns";
		}
	}
}
