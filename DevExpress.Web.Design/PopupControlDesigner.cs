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
using System.Text;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Design {
	public class ASPxPopupControlDesigner : ASPxDataWebControlDesigner {
		private static string fWindowTemplateCaption = "Window[{0}]";
		private static string[] fWindowTemplateNames = new string[] { "HeaderTemplate", "HeaderContentTemplate", 
			"ContentTemplate", "FooterTemplate", "FooterContentTemplate" };
		private static string[] fControlTemplateNames = new string[] { "HeaderTemplate", "HeaderContentTemplate", 
			"FooterTemplate", "FooterContentTemplate", "WindowHeaderTemplate", "WindowHeaderContentTemplate", 
			"WindowContentTemplate", "WindowFooterTemplate", "WindowFooterContentTemplate" };
		private ASPxPopupControl fPopupControl = null;
		private int fViewWindow = 0;
		public ASPxPopupControl PopupControl {
			get { return fPopupControl; }
		}
		public int ViewWindow {
			get { return fViewWindow; }
			set {
				fViewWindow = value;
				TypeDescriptor.Refresh(Component);
				UpdateDesignTimeHtml();
			}
		}
		protected internal bool AllowDragging {
			get { return PopupControl.AllowDragging; }
			set {
				PopupControl.AllowDragging = value;
				PropertyChanged("AllowDragging");
			}
		}
		protected internal CloseAction CloseAction {
			get { return PopupControl.CloseAction; }
			set {
				PopupControl.CloseAction = value;
				PropertyChanged("CloseAction");
			}
		}
		protected internal bool CloseOnEscape {
			get { return PopupControl.CloseOnEscape; }
			set {
				PopupControl.CloseOnEscape = value;
				PropertyChanged("CloseOnEscape");
			}
		}
		protected internal string PopupElementID {
			get { return PopupControl.PopupElementID; }
			set {
				PopupControl.PopupElementID = value;
				PropertyChanged("PopupElementID");
			}
		}
		protected internal bool ShowOnPageLoad {
			get { return PopupControl.ShowOnPageLoad; }
			set {
				PopupControl.ShowOnPageLoad = value;
				PropertyChanged("ShowOnPageLoad");
			}
		}
		protected internal bool Modal {
			get { return PopupControl.Modal; }
			set {
				PopupControl.Modal = value;
				PropertyChanged("Modal");
			}
		}
		protected internal bool ShowHeader {
			get { return PopupControl.ShowHeader; }
			set {
				PopupControl.ShowHeader = value;
				PropertyChanged("ShowHeader");
			}
		}
		public override void Initialize(IComponent component) {
			fPopupControl = (ASPxPopupControl)component;
			base.Initialize(component);
			SetViewFlags(ViewFlags.TemplateEditing, true);
		}
		protected override void FillPropertyNameToCaptionMap(Dictionary<string, string> propertyNameToCaptionMap) {
			base.FillPropertyNameToCaptionMap(propertyNameToCaptionMap);
			propertyNameToCaptionMap.Add("Windows", "Windows");
		}
		protected override string GetBaseProperty() {
			return "Windows";
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new PopupControlDesignerActionList(this);
		}
		public override void RunDesigner() {
			ShowDialog(new WrapperEditorForm(new PopupControlCommonFormDesigner(PopupControl, DesignerHost)));
		}
		protected override void DataBind(ASPxDataWebControlBase dataControl) {
			ASPxPopupControl popupControl = dataControl as ASPxPopupControl;
			if (!string.IsNullOrEmpty(popupControl.DataSourceID) || (popupControl.DataSource != null)) {
				popupControl.WindowsInternal.Clear();
				base.DataBind(popupControl);
			}
		}
		protected override TemplateGroupCollection CreateTemplateGroups() {
			TemplateGroupCollection templateGroups = base.CreateTemplateGroups();
			PopupWindowCollection windows = PopupControl.WindowsInternal;
			for(int i = 0; i < windows.Count; i++) {
				PopupWindow window = windows[i];
				TemplateGroup templateGroup = new TemplateGroup(string.Format(fWindowTemplateCaption, i));
				for(int j = 0; j < fWindowTemplateNames.Length; j++) {
					TemplateDefinition templateDefinition = new TemplateDefinition(this,
						fWindowTemplateNames[j], window, fWindowTemplateNames[j],
						GetTemplateStyle(window, fWindowTemplateNames[j]));
					templateDefinition.SupportsDataBinding = true;
					templateGroup.AddTemplateDefinition(templateDefinition);
				}
				templateGroups.Add(templateGroup);
			}
			for(int i = 0; i < fControlTemplateNames.Length; i++) {
				TemplateGroup templateGroup = new TemplateGroup(fControlTemplateNames[i]);
				TemplateDefinition templateDefinition = new TemplateDefinition(this, fControlTemplateNames[i],
					Component, fControlTemplateNames[i],
					GetTemplateStyle(null, fControlTemplateNames[i]));
				templateDefinition.SupportsDataBinding = true;
				templateGroup.AddTemplateDefinition(templateDefinition);
				templateGroups.Add(templateGroup);
			}
			return templateGroups;
		}
		private Style GetTemplateStyle(PopupWindow window, string templateName) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(fPopupControl.GetControlStyle());
			switch (templateName) {
				case "HeaderTemplate":
				case "HeaderContentTemplate":
					style.CopyFrom(fPopupControl.GetHeaderStyle(window));
					break;
				case "ContentTemplate":
					style.CopyFrom(fPopupControl.GetContentStyle(window));
					break;
				case "FooterTemplate":
				case "FooterContentTemplate":
					style.CopyFrom(fPopupControl.GetFooterStyle(window));
					break;
			}
			return style;
		}
		protected override string GetDesignTimeHtmlInternal() {
			((IControlDesignerAccessor)base.ViewControl).UserData["ViewWindow"] = ViewWindow;
			((IControlDesignerAccessor)base.ViewControl).GetDesignModeState();
			return base.GetDesignTimeHtmlInternal();
		}
		protected override void AddDesignerRegions(DesignerRegionCollection regions) {
			base.AddDesignerRegions(regions);
			int regionCount = regions.Count;
			for (int i = 0; i < fPopupControl.WindowsInternal.Count; i++) {
				PopupWindow window = fPopupControl.WindowsInternal[i];
				regions.Add(new PopupControlEditableRegion(this, "Edit " + GetRegionName(window), window));
				((ASPxPopupControl)ViewControl).WindowsInternal[i].ContentControl.Attributes[DesignerRegion.DesignerRegionAttributeName] = (regionCount + i).ToString();
			}
			if (fPopupControl.HasDefaultWindowInternal()) {
				PopupWindow window = fPopupControl.DefaultWindow;
				regions.Add(new PopupControlEditableRegion(this, "Edit " + GetRegionName(window), window));
				((ASPxPopupControl)ViewControl).DefaultWindow.ContentControl.Attributes[DesignerRegion.DesignerRegionAttributeName] = (regionCount).ToString();
			}
		}
		public override string GetEditableDesignerRegionContent(EditableDesignerRegion region) {
			PopupControlEditableRegion popupControlRegion = region as PopupControlEditableRegion;
			if(popupControlRegion == null)
				throw new ArgumentNullException(StringResources.InvalidRegion);
			return GetEditableDesignerRegionContent(popupControlRegion.Window.ContentControl.Controls);
		}
		public override void SetEditableDesignerRegionContent(EditableDesignerRegion region, string content) {
			PopupControlEditableRegion popupControlRegion = region as PopupControlEditableRegion;
			if(popupControlRegion == null)
				throw new ArgumentNullException(StringResources.InvalidRegion);
			SetEditableDesignerRegionContent(popupControlRegion.Window.ContentControl.Controls, content);
		}
		protected string GetRegionName(PopupWindow window) {
			if (window.Text != "")
				return window.Text;
			else if (window.Name != "")
				return window.Name;
			else
				return ("[Window (" + window.Index + ")]");
		}
		protected internal override string[] GetDataBindingSchemaFields() {
			return new string[] { "Enabled", "FooterNavigateUrl", "FooterText", "HeaderNavigateUrl", 
				"HeaderText", "Target", "Text", "ToolTip"};
		}
		protected internal override Type GetDataBindingSchemaItemType() {
			return typeof(PopupWindow);
		}
	}
	public class PopupControlDesignerActionList: ASPxWebControlDesignerActionList {
		private ASPxPopupControlDesigner fPopupControlDesigner = null;
		[TypeConverter(typeof(ViewWindowTypeConverter))]
		public int ViewWindow {
			get { return fPopupControlDesigner.ViewWindow; }
			set { fPopupControlDesigner.ViewWindow = value; }
		}
		public int ViewWindowCount {
			get { return fPopupControlDesigner.PopupControl.WindowsInternal.Count; }
		}
		public bool AllowDragging { 
			get { return fPopupControlDesigner.AllowDragging; }
			set { fPopupControlDesigner.AllowDragging = value; }
		}
		public CloseAction CloseAction {
			get { return fPopupControlDesigner.CloseAction; }
			set { fPopupControlDesigner.CloseAction = value; }
		}
		public bool CloseOnEscape {
			get { return fPopupControlDesigner.CloseOnEscape; }
			set { fPopupControlDesigner.CloseOnEscape = value; }
		}
		public string PopupElementID {
			get { return fPopupControlDesigner.PopupElementID; }
			set { fPopupControlDesigner.PopupElementID = value; }
		}
		public bool ShowOnPageLoad {
			get { return fPopupControlDesigner.ShowOnPageLoad; }
			set { fPopupControlDesigner.ShowOnPageLoad = value; }
		}
		public bool Modal {
			get { return fPopupControlDesigner.Modal; }
			set { fPopupControlDesigner.Modal = value; }
		}
		public bool ShowHeader {
			get { return fPopupControlDesigner.ShowHeader; }
			set { fPopupControlDesigner.ShowHeader = value; }
		}
		public PopupControlDesignerActionList(ASPxPopupControlDesigner popupControlDesigner)
			: base(popupControlDesigner) {
			fPopupControlDesigner = popupControlDesigner;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = base.GetSortedActionItems();
			if(ViewWindowCount > 1) {
				collection.Insert(0, new DesignerActionPropertyItem("ViewWindow",
					StringResources.PopupControlActionList_View,
					StringResources.ActionList_MiscCategory,
					StringResources.PopupControlActionList_ViewDescription));
			}
			collection.Add(new DesignerActionPropertyItem("PopupElementID",
				StringResources.PopupControlActionList_PopupElementID,
				StringResources.ActionList_MiscCategory,
				StringResources.PopupControlActionList_PopupElementIDDescription));
			collection.Add(new DesignerActionPropertyItem("Modal",
				StringResources.PopupControlActionList_Modal,
				StringResources.ActionList_MiscCategory,
				StringResources.PopupControlActionList_ModalDescription));
			collection.Add(new DesignerActionPropertyItem("AllowDragging",
				StringResources.PopupControlActionList_AllowDragging,
				StringResources.ActionList_MiscCategory,
				StringResources.PopupControlActionList_AllowDraggingDescription));
			collection.Add(new DesignerActionPropertyItem("ShowHeader",
				StringResources.PopupControlActionList_ShowHeader,
				StringResources.ActionList_MiscCategory,
				StringResources.PopupControlActionList_ShowHeaderDescription));
			collection.Add(new DesignerActionPropertyItem("ShowOnPageLoad",
				StringResources.PopupControlActionList_ShowOnPageLoad,
				StringResources.ActionList_MiscCategory,
				StringResources.PopupControlActionList_ShowOnPageLoadDescription));
			collection.Add(new DesignerActionPropertyItem("CloseAction",
				StringResources.PopupControlActionList_CloseAction,
				StringResources.ActionList_MiscCategory,
				StringResources.PopupControlActionList_CloseActionDescription));
			collection.Add(new DesignerActionPropertyItem("CloseOnEscape",
				StringResources.PopupControlActionList_CloseOnEscape,
				StringResources.ActionList_MiscCategory,
				StringResources.PopupControlActionList_CloseOnEscapeDescription));
			return collection;
		}
	}
	public class ViewWindowTypeConverter: TypeConverter {
		protected const string All = "All";
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return true;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			int viewWindowCount = (context.Instance as PopupControlDesignerActionList).ViewWindowCount;
			List<string> objects = new List<string>();
			for (int i = 0; i < viewWindowCount; i++)
				objects.Add(i.ToString());
			objects.Add(All);
			return new StandardValuesCollection(objects.ToArray());
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if (sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType == typeof(string))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			if (value is string) {
				int viewWindow = -1;
				if (int.TryParse((string)value, out viewWindow))
					return viewWindow;
				return -1;
			}
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if (destinationType == typeof(string)) {
				if (value is int) {
					int viewWindowCount = (context.Instance as PopupControlDesignerActionList).ViewWindowCount;
					int viewWindow = (int)value;
					if (0 <= viewWindow && viewWindow < viewWindowCount)
						return viewWindow.ToString();
					return All;
				}
				return value.ToString();
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public class PopupControlEditableRegion: EditableDesignerRegion {
		private PopupWindow fWindow = null;
		public PopupWindow Window {
			get { return fWindow; }
		}
		public PopupControlEditableRegion(ASPxPopupControlDesigner designer, string name, PopupWindow window)
			: base(designer, name, false) {
			fWindow = window;
		}
	}
	public class PopupControlCommonFormDesigner : CommonFormDesigner {
		public PopupControlCommonFormDesigner(ASPxPopupControl popupControl, IServiceProvider provider)
			: base(new FlatCollectionItemsOwner<PopupWindow>(popupControl, provider, popupControl.Windows, "Windows")) {
			ItemsImageIndex = WindowsImageIndex;
		}
	}
}
