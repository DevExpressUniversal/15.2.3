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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using DevExpress.Data.PivotGrid;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.XtraPivotGrid.Design {
	class PivotGridFieldDesigner : BaseComponentDesigner {
		protected override bool AllowEditInherited {
			get {
				return false;
			}
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new PivotGridFieldDesignerActionList(this, Component as PivotGridField));
		}
		internal class PivotGridFieldDesignerActionList : DesignerActionList, IPivotGridViewInfoDataOwner {
			DesignerActionItemCollection collection;
			IDesigner designer;
			public PivotGridFieldDesignerActionList(IDesigner designer, PivotGridField field)
				: base(field) {
				this.designer = designer;
			}
			public override DesignerActionItemCollection GetSortedActionItems() {
				if(collection == null)
					collection = GetActionItems();
				return collection;
			}
			internal PivotGridField Field { get { return Component as PivotGridField; } }
			DesignerActionItemCollection GetActionItems() {
				DesignerActionItemCollection res = new DesignerActionItemCollection();
				res.Add(new DesignerActionHeaderItemEx(
					new GetDisplayNameDelegate(() => { return "Field: " + Field.Name; })));
				res.Add(new DesignerActionPropertyItem("Name", "Name"));
				res.Add(new DesignerActionPropertyItem("Caption", "Caption"));
				res.Add(new DesignerActionPropertyItem("FieldName", "Field Name"));
				if(Field.PivotGrid != null && string.IsNullOrEmpty(Field.PivotGrid.OLAPConnectionString)) {
					if(Field.DataType == typeof(DateTime) || Field.GroupInterval != PivotGroupInterval.Default)
						res.Add(new DesignerActionPropertyItem("GroupInterval", "Group Interval"));
					if(Field.Area == PivotArea.DataArea) {
						res.Add(new DesignerActionPropertyItem("SummaryType", "Summary Type"));
						res.Add(new DesignerActionPropertyItem("SummaryDisplayType", "Summary Display Type"));
					}
				}
				if(Field.PivotGrid != null && string.IsNullOrEmpty(Field.PivotGrid.OLAPConnectionString) && Field.Area == PivotArea.DataArea) {
					res.Add(new DesignerActionPropertyItem("FieldEdit", "Field Edit", "Editor"));
				}
				return res;
			}
			public string Caption {
				get { return Field.Caption; }
				set {
					EditorContextHelper.SetPropertyValue(designer, Field, "Caption", value);
				}
			}
			[RefreshProperties(System.ComponentModel.RefreshProperties.All)]
			public string Name {
				get { return Field.Name; }
				set {
					EditorContextHelper.SetPropertyValue(designer, Field, "Name", value);
				}
			}
			[Editor("DevExpress.XtraPivotGrid.TypeConverters.PivotColumnNameEditor, " + AssemblyInfo.SRAssemblyPivotGrid, typeof(System.Drawing.Design.UITypeEditor))]
			public string FieldName {
				get { return Field.FieldName; }
				set {
					EditorContextHelper.SetPropertyValue(designer, Field, "FieldName", value);
				}
			}
			[TypeConverter(typeof(DevExpress.XtraPivotGrid.TypeConverters.FieldEditConverter)),
			Editor("DevExpress.XtraPivotGrid.Design.FieldEditEditor, " + AssemblyInfo.SRAssemblyPivotGridDesign, typeof(System.Drawing.Design.UITypeEditor))]
			public DevExpress.XtraEditors.Repository.RepositoryItem FieldEdit {
				get { return Field.FieldEdit; }
				set { EditorContextHelper.SetPropertyValue(designer, Field, "FieldEdit", value); }
			}
			public PivotGroupInterval GroupInterval {
				get { return Field.GroupInterval; }
				set { EditorContextHelper.SetPropertyValue(designer, Field, "GroupInterval", value); }
			}
			public PivotSummaryType SummaryType {
				get { return Field.SummaryType; }
				set { EditorContextHelper.SetPropertyValue(designer, Field, "SummaryType", value); }
			}
			public PivotSummaryDisplayType SummaryDisplayType {
				get { return Field.SummaryDisplayType; }
				set { EditorContextHelper.SetPropertyValue(designer, Field, "SummaryDisplayType", value); }
			}
			[Editor(typeof(AllowedLocationsEditor), typeof(System.Drawing.Design.UITypeEditor))]
			public AllowedLocationArea AllowedAreas {
				get { return GetAreaByField(Field); }
				set { 
					EditorContextHelper.SetPropertyValue(designer, Field, "AllowedAreas", GetAllowedAreas(value)); 
					EditorContextHelper.SetPropertyValue(designer, Field.Options, "AllowHide", GetAllowHide(value));
				}
			}
			PivotGridViewInfoData IPivotGridViewInfoDataOwner.DataViewInfo {
				get { return Field.PivotGrid == null ? null : ((IPivotGridViewInfoDataOwner)Field.PivotGrid).DataViewInfo; }
			}
			AllowedLocationArea GetAreaByField(PivotGridField field) {
				AllowedLocationArea area = (AllowedLocationArea)(int)field.AllowedAreas;
				area += field.CanHide ? 16 : 0;
				return area;
			}
			PivotGridAllowedAreas GetAllowedAreas(AllowedLocationArea location) {
				PivotGridAllowedAreas areas = 0;
				if((location & AllowedLocationArea.ColumnArea) != 0)
					areas |= PivotGridAllowedAreas.ColumnArea;
				if((location & AllowedLocationArea.DataArea) != 0)
					areas |= PivotGridAllowedAreas.DataArea;
				if((location & AllowedLocationArea.FilterArea) != 0)
					areas |= PivotGridAllowedAreas.FilterArea;
				if((location & AllowedLocationArea.RowArea) != 0)
					areas |= PivotGridAllowedAreas.RowArea;
				return areas;
			}
			DefaultBoolean GetAllowHide(AllowedLocationArea location) {
				return (location & AllowedLocationArea.HiddenArea) == 0 ? DefaultBoolean.False : DefaultBoolean.True;
			}
		}
	}
	public class AllowedLocationsEditor : UITypeEditor {
		public AllowedLocationsEditor() {
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.DropDown;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			PivotGridFieldDesigner.PivotGridFieldDesignerActionList list = (PivotGridFieldDesigner.PivotGridFieldDesignerActionList)context.Instance;
			if(context != null) {
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if(edSvc != null) {
					using(AllowedLocationsControl control = new AllowedLocationsControl(edSvc, list.AllowedAreas, GetCurrentAreaByField(list.Field))) {
						edSvc.DropDownControl(control);
						value = control.Area;
					}
				}
			}
			return value;
		}
		AllowedLocationArea GetCurrentAreaByField(PivotGridField field) {
			if(!field.Visible)
				return AllowedLocationArea.HiddenArea;
			switch(field.Area) {
				case PivotArea.ColumnArea:
					return AllowedLocationArea.ColumnArea;
				case PivotArea.DataArea:
					return AllowedLocationArea.DataArea;
				case PivotArea.FilterArea:
					return AllowedLocationArea.FilterArea;
				case PivotArea.RowArea:
					return AllowedLocationArea.RowArea;
			}
			return AllowedLocationArea.All;
		}
	}
}
