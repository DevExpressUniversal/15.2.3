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
using System.Web.UI;
using System.Web.UI.Design;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Customization;
namespace DevExpress.Web.ASPxPivotGrid.Design {
	public class PivotGridControlDesigner : ASPxDataWebControlDesigner, IDataSourceViewSchemaAccessor, 
			IDataSourceDesigner {
		protected ASPxPivotGrid Grid {
			get { return Component as ASPxPivotGrid; }
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			SetViewFlags(ViewFlags.TemplateEditing, true);
		}
		protected override void FillPropertyNameToCaptionMap(Dictionary<string, string> propertyNameToCaptionMap) {
			base.FillPropertyNameToCaptionMap(propertyNameToCaptionMap);
			propertyNameToCaptionMap.Add("Fields", "Fields");
		}
		protected override string GetDesignTimeHtmlInternal() {
			Grid.Data.DataSourceViewSchemaAccessor = this;
			TypeDescriptor.Refresh(Component);
			return base.GetDesignTimeHtmlInternal();
		}
		protected override int SampleRowCount {
			get {
				return 10;
			}
		}
		static string[] controlTemplateNames = new string[] { "HeaderTemplate", "EmptyAreaTemplate", "FieldValueTemplate", "CellTemplate" };
		static string fieldTemplateName = "Fields[{0}].{1}";
		static string[] fieldTemplateNames = new string[] { "HeaderTemplate", "ValueTemplate" };
		protected override TemplateGroupCollection CreateTemplateGroups() {
			TemplateGroupCollection templateGroups = base.CreateTemplateGroups();
			for(int i = 0; i < Grid.Fields.Count; i++) {
				PivotGridField field = Grid.Fields[i];
				TemplateGroup templateGroup = new TemplateGroup(string.Format(fieldTemplateName, i, field.ToString()));
				for(int j = 0; j < fieldTemplateNames.Length; j++) {
					TemplateDefinition templateDefinition = new TemplateDefinition(this, fieldTemplateNames[j], field, fieldTemplateNames[j], this.GetTemplateStyle());
					templateDefinition.SupportsDataBinding = true;
					templateGroup.AddTemplateDefinition(templateDefinition);
				}
				templateGroups.Add(templateGroup);
			}
			for(int i = 0; i < controlTemplateNames.Length; i++) {
				TemplateGroup templateGroup = new TemplateGroup(controlTemplateNames[i]);
				TemplateDefinition templateDefinition = new TemplateDefinition(this, controlTemplateNames[i], base.Component, controlTemplateNames[i], this.GetTemplateStyle());
				templateDefinition.SupportsDataBinding = true;
				templateGroup.AddTemplateDefinition(templateDefinition);
				templateGroups.Add(templateGroup);
			}
			return templateGroups;
		}
		System.Web.UI.WebControls.Style GetTemplateStyle() {
			return Grid.ControlStyle;
		}
		public string[] GetFieldList() {
			return Grid.Data.GetFieldList();
		}
		public override void RunDesigner() {
			ShowDialog(new WrapperEditorForm(new PivotGridCommonFormDesigner(Grid, DesignerHost)));
		}
		protected override DataWebControlActionListBase CreateDataActionList() {
			return new PivotGridDataWebControlActionList(this);
		}
		public override void ShowAbout() {
			PivotGridAboutDialogHelper.ShowAbout(Component.Site);
		}
		object IDataSourceViewSchemaAccessor.DataSourceViewSchema {
			get {
				if(DesignerView == null) return null;
				if(DesignerView.Schema == null && DataSourceDesigner != null) {
					DataSourceDesigner.RefreshSchema(true);
				}
				return DesignerView.Schema;
			}
			set { ;}
		}
		protected override void PreFilterProperties(IDictionary properties) {
			base.PreFilterProperties(properties);
			PerformPrefilterProperty(properties, "DataSourceID", typeof(PivotGridDataSourceIDConverter));
			foreach(object propName in new ArrayList(properties.Keys)) {
				PropertyDescriptor prop = (PropertyDescriptor)properties[propName];
				if(prop.Attributes[typeof(ObsoleteAttribute)] as ObsoleteAttribute != null) {
					properties[propName] = TypeDescriptor.CreateProperty(typeof(ASPxPivotGrid), prop, new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden));
				}
			}
		}
		public override void EnsureControlReferences() {
			base.EnsureControlReferences();
			EnsureReferences(
				AssemblyInfo.SRAssemblyPivotGridCore,
				AssemblyInfo.SRAssemblyASPxPivotGrid
			);
		}
		protected internal override string[] GetDataBindingSchemaFields() {
			return new string[] { PivotChartDescriptor.ArgumentsColumn, PivotChartDescriptor.SeriesColumn, 
				PivotChartDescriptor.ValuesColumn };
		}
		protected internal override Type GetDataBindingSchemaItemType() {
			return typeof(PivotChartDesignerItem);
		}
		#region IDataSourceDesigner Members
		bool IDataSourceDesigner.CanConfigure {
			get { return false; }
		}
		bool IDataSourceDesigner.CanRefreshSchema {
			get { return false; }
		}
		void IDataSourceDesigner.Configure() { }
		event EventHandler IDataSourceDesigner.DataSourceChanged {
			add { ; }
			remove { ; }
		}
		DesignerDataSourceView IDataSourceDesigner.GetView(string viewName) {
			if(string.IsNullOrEmpty(viewName))
				return new PivotChartDesignerDataSourceView(this, string.Empty);
			return null;
		}
		string[] IDataSourceDesigner.GetViewNames() {
			return new string[] { string.Empty };
		}
		void IDataSourceDesigner.RefreshSchema(bool preferSilent) { }
		void IDataSourceDesigner.ResumeDataSourceEvents() { }
		event EventHandler IDataSourceDesigner.SchemaRefreshed {
			add { ; }
			remove { ; }
		}
		void IDataSourceDesigner.SuppressDataSourceEvents() { }
		#endregion
	}
	public class PivotChartDesignerDataSourceView : DesignerDataSourceView {
		IDataSourceViewSchema schema;
		PivotGridControlDesigner designer;
		public PivotChartDesignerDataSourceView(PivotGridControlDesigner owner, string viewName)
			: base(owner, viewName) {
			this.designer = owner;
		}
		public override IDataSourceViewSchema Schema {
			get {
				if(schema == null)
					schema = new DataBindingSchema(this.designer);
				return schema;
			}
		}
	}
	[TypeDescriptionProvider(typeof(DevExpress.Web.ASPxPivotGrid.Design.PivotChartDesignerItem.PivotChartDesignerItemDescriptionProvider))]
	public class PivotChartDesignerItem : CustomTypeDescriptor {
		class PivotChartDesignerItemDescriptionProvider : TypeDescriptionProvider {
			public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance) {
				if(objectType == typeof(PivotChartDesignerItem))
					return new PivotChartDesignerItem();
				return base.GetTypeDescriptor(objectType, instance);
			}
		}
		public override PropertyDescriptorCollection GetProperties() {
			PropertyDescriptorCollection chartProps = new PropertyDescriptorCollection(null);
			chartProps.Add(new PivotChartDescriptorDesign(PivotChartDescriptorType.Argument));
			chartProps.Add(new PivotChartDescriptorDesign(PivotChartDescriptorType.Series)); 
			chartProps.Add(new PivotChartDescriptorDesign(PivotChartDescriptorType.Value));
			return chartProps;
		}
		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes) {
			if(attributes == null || attributes.Length == 0)
				return GetProperties();
			return base.GetProperties(attributes);
		}
	}
	public class PivotChartDescriptorDesign : PivotChartDescriptorBase {
		public PivotChartDescriptorDesign(PivotChartDescriptorType type)
			: base(type) {
		}
		public override Type PropertyType {
			get {
				switch(Type) {
					case PivotChartDescriptorType.Argument:
					case PivotChartDescriptorType.Series:
						return typeof(string);
					case PivotChartDescriptorType.Value:
						return typeof(decimal);
				}
				return base.PropertyType;
			}
		}
	}
	public class PivotGridDataWebControlActionList : DataWebControlActionList {
		PivotGridControlDesigner designer;
		protected new PivotGridControlDesigner Designer { get { return designer; } }
		[Editor(typeof(DevExpress.Web.ASPxPivotGrid.Design.OLAPConnectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string OLAPConnectionString {
			get { return ((ASPxPivotGrid)Designer.Component).OLAPConnectionString; }
			set {
				ControlDesigner.InvokeTransactedChange(Designer.Component, new TransactedChangeCallback(SetOLAPConnectionStringCallback),
					value, "");
			}
		}
		[TypeConverter(typeof(PivotGridDataSourceIDConverter))]
		public override string DataSourceID {
			get { return base.DataSourceID; }
			set { base.DataSourceID = value; }
		}
		public PivotGridDataWebControlActionList(PivotGridControlDesigner designer) 
			: base(designer, designer.DataSourceDesigner) {
			this.designer = designer;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection result = base.GetSortedActionItems();
			result.Add(DesignUtils.CreateDesignerPropertyItem(Designer, "OLAPConnectionString", Designer.Component, StringResources.DataControl_ConfigureOLAPDataVerb));
			return result;
		}
		private bool SetOLAPConnectionStringCallback(object context) {
			((ASPxPivotGrid)Designer.Component).OLAPConnectionString = (string)context;
			return true;
		}
	}
	public class PivotCustomizationControlDesigner : ASPxWebControlDesigner {
		public PivotCustomizationControlDesigner() : base() { }
		public override void Initialize(IComponent component) {
			base.Initialize(component);
		}
		public override void ShowAbout() {
			PivotGridAboutDialogHelper.ShowASPxPivotCustomizationControlAbout(Component.Site);
		}
		public override bool HasClientSideEvents() {
			return false;
		}
		public override bool IsThemableControl() {
			return false;
		}
		public override void EnsureControlReferences() {
			base.EnsureControlReferences();
			EnsureReferences(
				AssemblyInfo.SRAssemblyPivotGridCore,
				AssemblyInfo.SRAssemblyASPxPivotGrid
			);
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new PivotCustomizationControlActionList(this);
		}
		public override bool HasCommonDesigner() {
			return false;
		}
	}
	public class PivotCustomizationControlActionList : ASPxWebControlDesignerActionList {
		public PivotCustomizationControlActionList(PivotCustomizationControlDesigner designer)
			: base(designer) { }
		protected ASPxPivotCustomizationControl CustomizationControl {
			get { return (ASPxPivotCustomizationControl)Designer.Component; }
		}
		[
		IDReferenceProperty(typeof(ASPxPivotGrid)), TypeConverter(typeof(PivotGridIDConverter))
		]
		public string ASPxPivotGridID {
			get { return CustomizationControl.ASPxPivotGridID; }
			set {
				CustomizationControl.ASPxPivotGridID = value;
				CustomizationControl.PropertyChanged("ASPxPivotGridID");
			}
		}
		public CustomizationFormLayout Layout {
			get { return CustomizationControl.Layout; }
			set {
				CustomizationControl.Layout = value;
				CustomizationControl.PropertyChanged("Layout");
			}
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = base.GetSortedActionItems();
			collection.Add(new DesignerActionPropertyItem("ASPxPivotGridID", "ASPxPivotGrid ID"));
			collection.Add(new DesignerActionPropertyItem("Layout", "Form Layout"));
			return collection;
		}
	}
	public class PivotGridCommonFormDesigner : CommonFormDesigner {
		public PivotGridCommonFormDesigner(ASPxPivotGrid pivot, IServiceProvider provider) :
			base(new PivotGridItemsOwner(pivot, provider)) {
		}
		protected override Type DefaultItemsFrameType { get { return typeof(PivotGridItemsEditorFrame); } }
	}
}
