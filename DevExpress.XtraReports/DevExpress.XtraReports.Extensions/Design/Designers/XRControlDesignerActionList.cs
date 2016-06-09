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
using System.Diagnostics;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Data.Design;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.BarCode;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Native.Data;
using DevExpress.XtraReports.Localization;
using System.Collections.Generic;
using DevExpress.Data.Browsing.Design;
using DevExpress.XtraReports.Design.Commands;
namespace DevExpress.XtraReports.Design {
	#region base classes
	public abstract class ComponentDesignerActionList : DesignerActionList {
		#region static
		protected static DesignerActionPropertyItem CreatePropertyItem(string memberName, string displayName) {
			return new DesignerActionPropertyItem(memberName, displayName, "");
		}
		protected PropertyDescriptor GetPropertyDescriptor(string name, object editedObject) {
			return TypeDescriptor.GetProperties(editedObject)[name];
		}
		public static PropertyDescriptor FilterProperty(DesignerActionList actionList, string name, PropertyDescriptor property) {
			IPropertyFilterService service = actionList.GetService(typeof(IPropertyFilterService)) as IPropertyFilterService;
			if(service != null && property != null) {
				Dictionary<string, PropertyDescriptor> properties = new Dictionary<string, PropertyDescriptor>();
				properties.Add(name, property);
				service.PreFilterProperties(properties, actionList.Component);
				PropertyDescriptor value;
				property = properties.TryGetValue(name, out value) ? value : null;
			}
			return PropertyIsBrowsable(property) ? property : null;
		}
		static bool PropertyIsBrowsable(PropertyDescriptor property) {
			if(property == null)
				return false;
			BrowsableAttribute attr = property.Attributes[typeof(BrowsableAttribute)] as BrowsableAttribute;
			return attr == null || attr.Browsable;
		}
		#endregion
		public ComponentDesignerActionList(IComponent component)
			: base(component) {
		}
		protected abstract object EditedObject { get; }
		protected void AddPropertyItem(DesignerActionItemCollection actionItems, string name, string reflectName, object editedObject) {
			PropertyDescriptor property = GetPropertyDescriptor(name, editedObject);
			property = FilterProperty(this, name, property);
			if(property != null)
				actionItems.Add(CreatePropertyItem(reflectName, property.DisplayName));
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection actionItems = new DesignerActionItemCollection();
			FillActionItemCollection(actionItems);
			return actionItems;
		}
		protected virtual void FillActionItemCollection(DesignerActionItemCollection actionItems) {
		}
		protected void AddPropertyItem(DesignerActionItemCollection actionItems, string name, string reflectName) {
			AddPropertyItem(actionItems, name, reflectName, Component);
		}
		protected void SetPropertyValue(string name, object val) {
			try {
				SetPropertyValue(EditedObject, name, val);
			} catch(Exception ex) {
				System.Windows.Forms.Design.IUIService uiService = GetService(typeof(System.Windows.Forms.Design.IUIService)) as System.Windows.Forms.Design.IUIService;
				if(uiService != null)
					uiService.ShowError(ex);
				else
					throw;
			}
		}
		protected bool PropertyIsBrowsable(string name) {
			PropertyDescriptor property = XRAccessor.GetPropertyDescriptor(this.Component, name);
			return PropertyIsBrowsable(property);
		}
		protected abstract void SetPropertyValue(object component, string name, object val);
	}
	public class XRComponentDesignerActionList : ComponentDesignerActionList {
		#region static
		static protected DesignerActionPropertyItem CreatePropertyItem(string memberName, ReportStringId id) {
			return CreatePropertyItem(memberName, ReportLocalizer.GetString(id));
		}
		#endregion
		protected XRComponentDesigner designer;
		protected XRControl XRControl { get { return (XRControl)Component; } }
		protected override sealed object EditedObject { get { return Component; } }
		public XRComponentDesignerActionList(XRComponentDesigner designer)
			: base(designer.Component) {
			this.designer = designer;
		}
		protected sealed override void SetPropertyValue(object component, string name, object val) {
			if(component is IComponent && ((IComponent)component).Site != null) {
				PropertyDescriptor property = TypeDescriptor.GetProperties(component)[name];
				if(property != null)
					property.SetValue(component, val);
			} else
				DevExpress.Utils.Design.EditorContextHelper.SetPropertyValue(designer, component, name, val);
			if(!Object.ReferenceEquals(Component, component))
				DevExpress.Utils.Design.EditorContextHelper.FireChanged(designer, Component);
		}
		protected bool CanExecCommand(CommandID command) {
			IMenuCommandService menuCommandService = GetService(typeof(IMenuCommandService)) as IMenuCommandService;
			if(menuCommandService != null) {
				MenuCommand menuCommand = menuCommandService.FindCommand(command);
				return menuCommand != null && menuCommand.Enabled;
			}
			return false;
		}
		protected void ExecCommand(CommandID command) {
			IMenuCommandService menuCommandService = GetService(typeof(IMenuCommandService)) as IMenuCommandService;
			if(menuCommandService != null)
				menuCommandService.GlobalInvoke(command);
		}
	}
	public class XRControlBaseDesignerActionList : XRComponentDesignerActionList {
		protected XRControlDesigner ControlDesigner { get { return (XRControlDesigner)designer; } }
		public XRControlBaseDesignerActionList(XRControlDesigner designer)
			: base(designer) {
		}
	}
	public abstract class XRControlTextDesignerActionListBase : XRControlBaseDesignerActionList {
		[
		Editor("DevExpress.XtraReports.Design.FormatStringEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		RefreshProperties(RefreshProperties.All),
		]
		public string FormatString {
			get {
				if(ControlDesigner.IsInplaceEditingMode) {
					MailMergeFieldInfo mailMergeFieldInfo = InplaceEditorHelper.GetMailMergeFieldInfoFromInplaceEditor(XRControl);
					return mailMergeFieldInfo != null ? mailMergeFieldInfo.FormatString : string.Empty;
				}
				string formatString = GetFormatString();
				if(!string.IsNullOrEmpty(formatString))
					return formatString;
				MailMergeFieldInfoCollection columns = EmbeddedFieldsHelper.GetEmbeddedFieldInfos(XRControl);
				return columns.Count == 1 ? columns[0].FormatString : String.Empty;
			}
			set {
				StringHelper.ValidateFormatString(value);
				ControlDesigner.SetBindingFormatString(BindingPropertyName, value);
			}
		}
		protected new XRTextControlDesigner ControlDesigner { get { return (XRTextControlDesigner)designer; } }
		protected new XRFieldEmbeddableControl XRControl { get { return Component as XRFieldEmbeddableControl; } }
		protected abstract string BindingPropertyName { get; }
		protected virtual bool ShowFormatStringInNotInplaceMode { get { return true; } }
		bool HasDataBindingsProperty {
			get {
				PropertyDescriptor property = TypeDescriptor.GetProperties(XRControl)[XRComponentPropertyNames.DataBindings];
				return property != null && property.SerializationVisibility != DesignerSerializationVisibility.Hidden;
			}
		}
		protected XRControlTextDesignerActionListBase(XRControlDesigner designer)
			: base(designer) {
		}
		protected virtual string GetFormatString() {
			XRBinding xrBinding = ((XRControl)Component).DataBindings[BindingPropertyName];
			if(xrBinding != null)
				return xrBinding.FormatString;
			return string.Empty;
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			MailMergeFieldInfoCollection columnNames = EmbeddedFieldsHelper.GetEmbeddedFieldInfos(ControlDesigner.XRControl);
			if(HasDataBindingsProperty && (ControlDesigner.IsInplaceEditingMode || XRControl.DataBindings[BindingPropertyName] != null || columnNames.Count <= 1))
				actionItems.Add(CreatePropertyItem(BindingPropertyName + "Binding", ReportStringId.STag_Name_DataBinding));
			if(InplaceEditorHelper.GetMailMergeFieldInfoFromInplaceEditor(XRControl) != null ||
				(ShowFormatStringInNotInplaceMode && !ControlDesigner.IsInplaceEditingMode &&
				(columnNames.Count == 1 || HasDataBindingsProperty && columnNames.Count == 0)))
				actionItems.Add(CreatePropertyItem("FormatString", ReportStringId.STag_Name_FormatString));
		}
	}
	public class XRControlTextDesignerActionList : XRControlTextDesignerActionListBase {
		[
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(DevExpress.XtraReports.Design.TextPropertyTypeConverter)),
		]
		public string Text {
			get { return ((XRControl)Component).Text; }
			set { SetPropertyValue("Text", value); }
		}
		[
		RefreshProperties(RefreshProperties.All),
		]
		public DesignBinding TextBinding {
			get {
				return ControlDesigner.GetDesignBinding("Text");
			}
			set {
				ControlDesigner.SetBinding("Text", value);
			}
		}
		protected override string BindingPropertyName { get { return "Text"; } }
		public XRControlTextDesignerActionList(XRControlDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			if(!ControlDesigner.IsInplaceEditingMode)
				AddPropertyItem(actionItems, "Text", "Text");
			base.FillActionItemCollection(actionItems);
		}
	}
	public class XRControlStyleDesignerActionList : XRControlBaseDesignerActionList {
		public Color ForeColor {
			get { return ((XRControl)Component).ForeColor; }
			set { SetPropertyValue("ForeColor", value); }
		}
		public Color BackColor {
			get { return ((XRControl)Component).BackColor; }
			set { SetPropertyValue("BackColor", value); }
		}
		[Editor(typeof(DevExpress.XtraReports.Design.XRFontEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public Font Font {
			get { return ((XRControl)Component).Font; }
			set { SetPropertyValue("Font", value); }
		}
		public XRControlStyleDesignerActionList(XRControlDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			XRTextControlDesigner textDesigner = designer as XRTextControlDesigner;
			if(textDesigner != null && !textDesigner.IsInplaceEditingMode) {
				AddPropertyItem(actionItems, "ForeColor", "ForeColor");
				AddPropertyItem(actionItems, "BackColor", "BackColor");
				AddPropertyItem(actionItems, "Font", "Font");
			}
		}
	}
	public class XRControlAnchorVerticalDesignerActionList : XRControlBaseDesignerActionList {
		public VerticalAnchorStyles AnchorVertical {
			get { return ((XRControl)Component).AnchorVertical; }
			set { SetPropertyValue("AnchorVertical", value); }
		}
		public XRControlAnchorVerticalDesignerActionList(XRControlDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			AddPropertyItem(actionItems, "AnchorVertical", "AnchorVertical");
		}
	}
	public class XRControlBookmarkDesignerActionList : XRControlBaseDesignerActionList {
		public string Bookmark {
			get { return ((XRControl)Component).Bookmark; }
			set { SetPropertyValue("Bookmark", value); }
		}
		[
		TypeConverter(typeof(DevExpress.XtraReports.Design.XRControlReferenceConverter)),
		Editor("DevExpress.XtraReports.Design.ParentBookmarkEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		]
		public XRControl BookmarkParent {
			get { return ((XRControl)Component).BookmarkParent; }
			set { SetPropertyValue("BookmarkParent", value); }
		}
		public XRControlBookmarkDesignerActionList(XRControlDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			AddPropertyItem(actionItems, "Bookmark", "Bookmark");
			AddPropertyItem(actionItems, "BookmarkParent", "BookmarkParent");
		}
	}
	#endregion
	#region XRLine
	public class XRLineDesignerActionList : XRControlBaseDesignerActionList {
		public Color ForeColor {
			get { return ((XRLine)Component).ForeColor; }
			set { SetPropertyValue("ForeColor", value); }
		}
		public int LineWidth {
			get { return ((XRLine)Component).LineWidth; }
			set { SetPropertyValue("LineWidth", value); }
		}
		public LineDirection LineDirection {
			get { return ((XRLine)Component).LineDirection; }
			set { SetPropertyValue("LineDirection", value); }
		}
		[TypeConverter(typeof(DevExpress.Utils.Design.DashStyleTypeConverter))]
		public System.Drawing.Drawing2D.DashStyle LineStyle {
			get { return ((XRLine)Component).LineStyle; }
			set { SetPropertyValue("LineStyle", value); }
		}
		public XRLineDesignerActionList(XRLineDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			AddPropertyItem(actionItems, "ForeColor", "ForeColor");
			AddPropertyItem(actionItems, "LineDirection", "LineDirection");
			AddPropertyItem(actionItems, "LineStyle", "LineStyle");
			AddPropertyItem(actionItems, "LineWidth", "LineWidth");
		}
	}
	#endregion
	#region XRLabel
	public class XRLabelDesignerActionList1 : XRControlTextDesignerActionList {
		XRLabel Label { get { return (XRLabel)Component; } }
		[
		RefreshProperties(RefreshProperties.All),
		Editor(typeof(DevExpress.XtraReports.Design.XRSummaryUIEditor), typeof(System.Drawing.Design.UITypeEditor))
		]
		public XRSummary Summary {
			get { return Label.Summary; }
			set { SetPropertyValue("Summary", value); }
		}
		public XRLabelDesignerActionList1(XRTextControlDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			XRTextControlDesigner textDesigner = designer as XRTextControlDesigner;
			if(textDesigner != null && !textDesigner.IsInplaceEditingMode) {
				AddPropertyItem(actionItems, "Summary", "Summary");
			}
		}
		protected override string GetFormatString() {
			if(Label.HasSummary)
				return Label.Summary.FormatString;
			return base.GetFormatString();
		}
	}
	public class XRLabelDesignerActionList2 : XRControlBaseDesignerActionList {
		public Single Angle {
			get { return ((XRLabel)Component).Angle; }
			set { SetPropertyValue("Angle", value); }
		}
		public XRLabelDesignerActionList2(XRTextControlDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			XRTextControlDesigner textDesigner = designer as XRTextControlDesigner;
			if(textDesigner != null && !textDesigner.IsInplaceEditingMode) {
				AddPropertyItem(actionItems, "Angle", "Angle");
			}
		}
	}
	public class XRLabelDesignerActionList3 : XRControlBaseDesignerActionList {
		public bool AutoWidth {
			get { return ((XRLabel)Component).AutoWidth; }
			set { SetPropertyValue("AutoWidth", value); }
		}
		public bool Shrink {
			get { return ((XRLabel)Component).CanShrink; }
			set { SetPropertyValue("CanShrink", value); }
		}
		public bool Grow {
			get { return ((XRLabel)Component).CanGrow; }
			set { SetPropertyValue("CanGrow", value); }
		}
		public bool Multiline {
			get { return ((XRLabel)Component).Multiline; }
			set { SetPropertyValue("Multiline", value); }
		}
		public bool WordWrap {
			get { return ((XRLabel)Component).WordWrap; }
			set { SetPropertyValue("WordWrap", value); }
		}
		public XRLabelDesignerActionList3(XRTextControlDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			XRTextControlDesigner textDesigner = designer as XRTextControlDesigner;
			if(textDesigner != null && !textDesigner.IsInplaceEditingMode) {
				if(textDesigner is XRLabelDesigner)
					AddPropertyItem(actionItems, "AutoWidth", "AutoWidth");
				AddPropertyItem(actionItems, "CanGrow", "Grow");
				AddPropertyItem(actionItems, "CanShrink", "Shrink");
				AddPropertyItem(actionItems, "Multiline", "Multiline");
				AddPropertyItem(actionItems, "WordWrap", "WordWrap");
			}
		}
	}
	#endregion
	#region XRBarCode
	public class XRBarCodeDesignerActionList1 : XRControlBaseDesignerActionList {
		[RefreshProperties(RefreshProperties.All)]
		public BarCodeGeneratorBase Symbology {
			get { return ((XRBarCode)Component).Symbology; }
			set { SetPropertyValue("Symbology", value); }
		}
		public float Module {
			get { return ((XRBarCode)Component).Module; }
			set { SetPropertyValue("Module", value); }
		}
		public bool AutoModule {
			get { return ((XRBarCode)Component).AutoModule; }
			set { SetPropertyValue("AutoModule", value); }
		}
		public XRBarCodeDesignerActionList1(XRBarCodeDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			AddPropertyItem(actionItems, "Symbology", "Symbology");
			AddPropertyItem(actionItems, "Module", "Module");
			AddPropertyItem(actionItems, "AutoModule", "AutoModule");
		}
	}
	public class XRBarCodeDesignerActionList2 : XRControlBaseDesignerActionList {
		public BarCodeOrientation BarCodeOrientation {
			get { return ((XRBarCode)Component).BarCodeOrientation; }
			set { SetPropertyValue("BarCodeOrientation", value); }
		}
		public XRBarCodeDesignerActionList2(XRBarCodeDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			AddPropertyItem(actionItems, "BarCodeOrientation", "BarCodeOrientation");
		}
	}
	public class XRBarCodeDesignerActionList3 : XRControlBaseDesignerActionList {
		public bool ShowText {
			get { return ((XRBarCode)Component).ShowText; }
			set { SetPropertyValue("ShowText", value); }
		}
		public XRBarCodeDesignerActionList3(XRBarCodeDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			AddPropertyItem(actionItems, "ShowText", "ShowText");
		}
	}
	#endregion
	#region XRZipCode
	public class XRZipCodeDesignerActionList : XRControlTextDesignerActionList {
		public int SegmentWidth {
			get { return ((XRZipCode)Component).SegmentWidth; }
			set { SetPropertyValue("SegmentWidth", value); }
		}
		public XRZipCodeDesignerActionList(XRZipCodeDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			AddPropertyItem(actionItems, "Text", "Text");
			actionItems.Add(CreatePropertyItem("TextBinding", ReportStringId.STag_Name_DataBinding));
			AddPropertyItem(actionItems, "SegmentWidth", "SegmentWidth");
		}
	}
	#endregion
	#region XRCheckBox
	public class XRCheckBoxDesignerActionList : XRControlBaseDesignerActionList {
		[TypeConverter(typeof(DevExpress.Utils.Design.CheckStateTypeConverter))]
		public CheckState CheckState {
			get { return ((XRCheckBox)Component).CheckState; }
			set { SetPropertyValue("CheckState", value); }
		}
		public DesignBinding CheckedBinding {
			get { return ControlDesigner.GetDesignBinding("CheckState"); }
			set { ControlDesigner.SetBinding("CheckState", value); }
		}
		public XRCheckBoxDesignerActionList(XRCheckBoxDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			AddPropertyItem(actionItems, "CheckState", "CheckState");
			actionItems.Add(CreatePropertyItem("CheckedBinding", ReportStringId.STag_Name_DataBinding));
		}
	}
	#endregion
	#region XRPictureBox
	public class XRPictureBoxDesignerActionList1 : XRControlBaseDesignerActionList {
		[
		Editor(typeof(DevExpress.XtraReports.Design.ImageEditor), typeof(System.Drawing.Design.UITypeEditor)),
		]
		public Image Image {
			get { return ((XRPictureBox)Component).Image; }
			set { SetPropertyValue("Image", value); }
		}
		public DesignBinding ImageBinding {
			get { return ControlDesigner.GetDesignBinding("Image"); }
			set { ControlDesigner.SetBinding("Image", value); }
		}
		public XRPictureBoxDesignerActionList1(XRPictureBoxDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			AddPropertyItem(actionItems, "Image", "Image");
			actionItems.Add(CreatePropertyItem("ImageBinding", ReportStringId.STag_Name_DataBinding));
		}
	}
	public class XRPictureBoxDesignerActionList2 : XRControlBaseDesignerActionList {
		[Editor(typeof(Design.ImageFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string ImageUrl {
			get { return ((XRPictureBox)Component).ImageUrl; }
			set { SetPropertyValue("ImageUrl", value); }
		}
		public DesignBinding ImageUrlBinding {
			get { return ControlDesigner.GetDesignBinding("ImageUrl"); }
			set { ControlDesigner.SetBinding("ImageUrl", value); }
		}
		public XRPictureBoxDesignerActionList2(XRPictureBoxDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			AddPropertyItem(actionItems, "ImageUrl", "ImageUrl");
			actionItems.Add(CreatePropertyItem("ImageUrlBinding", ReportStringId.STag_Name_DataBinding));
		}
	}
	public class XRPictureBoxDesignerActionList3 : XRControlBaseDesignerActionList, IXRPictureBoxDesignerActionList3 {
		[TypeConverter(typeof(ImageSizingTypeConverter))]
		[RefreshProperties(RefreshProperties.All)]
		public ImageSizeMode Sizing {
			get { return ((XRPictureBox)Component).Sizing; }
			set { SetPropertyValue("Sizing", value); }
		}
		[TypeConverter(typeof(ImageAlignmentTypeConverter))]
		[Editor("DevExpress.XtraReports.Design.ImageAlignmentEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor))]
		public ImageAlignment ImageAlignment {
			get { return ((XRPictureBox)Component).ImageAlignment; }
			set { SetPropertyValue("ImageAlignment", value); }
		}
		public XRPictureBoxDesignerActionList3(XRPictureBoxDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			AddPropertyItem(actionItems, "Sizing", "Sizing");
			AddPropertyItem(actionItems, "ImageAlignment", "ImageAlignment");
		}
	}
	#endregion
	#region XRRichText
	public class XRRichTextBoxBaseDesignerActionList : XRControlTextDesignerActionListBase {
		[
		RefreshProperties(RefreshProperties.All),
		]
		public DesignBinding RtfBinding {
			get {
				return ControlDesigner.GetDesignBinding("Rtf");
			}
			set {
				ControlDesigner.SetBinding("Rtf", value);
			}
		}
		protected override string BindingPropertyName { get { return "Rtf"; } }
		protected override bool ShowFormatStringInNotInplaceMode { get { return false; } }
		public XRRichTextBoxBaseDesignerActionList(XRControlDesigner designer)
			: base(designer) {
		}
	}
	public class XRRichTextDesignerActionList3 : XRControlBaseDesignerActionList {
		public bool Shrink {
			get { return ((XRRichText)Component).CanShrink; }
			set { SetPropertyValue("CanShrink", value); }
		}
		public bool Grow {
			get { return ((XRRichText)Component).CanGrow; }
			set { SetPropertyValue("CanGrow", value); }
		}
		public XRRichTextDesignerActionList3(XRTextControlDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			XRTextControlDesigner textDesigner = designer as XRTextControlDesigner;
			if(textDesigner != null && !textDesigner.IsInplaceEditingMode) {
				AddPropertyItem(actionItems, "CanGrow", "Grow");
				AddPropertyItem(actionItems, "CanShrink", "Shrink");
			}
		}
	}
	#endregion
	#region Subreport
	public class XRSubReportDesignerActionList : XRControlBaseDesignerActionList {
		[
		Editor("DevExpress.XtraReports.Design.ReportUrlEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		]
		public string ReportSourceUrl {
			get { return ((XRSubreport)Component).ReportSourceUrl; }
			set { SetPropertyValue("ReportSourceUrl", value); }
		}
		[
		Editor("DevExpress.XtraReports.Design.ReportSourceEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		]
		public XtraReport ReportSource {
			get { return ((SubreportBase)Component).ReportSource; }
			set { SetPropertyValue("ReportSource", value); }
		}
		public XRSubReportDesignerActionList(XRControlDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			AddPropertyItem(actionItems, "ReportSource", "ReportSource");
			AddPropertyItem(actionItems, "ReportSourceUrl", "ReportSourceUrl");
		}
	}
	#endregion
	#region XRPageInfo
	public class XRPageInfoDesignerActionList : XRControlBaseDesignerActionList {
		public PageInfo PageInfo {
			get { return ((XRPageInfo)Component).PageInfo; }
			set { SetPropertyValue("PageInfo", value); }
		}
		public int PageNumber {
			get { return ((XRPageInfo)Component).StartPageNumber; }
			set { SetPropertyValue("StartPageNumber", value); }
		}
		[Editor("DevExpress.XtraReports.Design.FormatStringEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string Format {
			get { return ((XRPageInfo)Component).Format; }
			set { SetPropertyValue("Format", value); }
		}
		[TypeConverter(typeof(DevExpress.XtraReports.Design.RunningBandTypeConverter))]
		public Band RunningBand {
			get { return ((XRPageInfo)Component).RunningBand; }
			set { SetPropertyValue("RunningBand", value); }
		}
		public XRPageInfoDesignerActionList(XRControlDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			AddPropertyItem(actionItems, "PageInfo", "PageInfo");
			AddPropertyItem(actionItems, "StartPageNumber", "PageNumber");
			AddPropertyItem(actionItems, "Format", "Format");
			AddPropertyItem(actionItems, "RunningBand", "RunningBand");
		}
	}
	#endregion
	#region bands
	public class DetailBandDesignerActionList : BandDesignerActionList1 {
		public bool KeepTogetherWithDetailReports {
			get { return ((DetailBand)Component).KeepTogetherWithDetailReports; }
			set { SetPropertyValue("KeepTogetherWithDetailReports", value); }
		}
		public DetailBandDesignerActionList(BandDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			if(XRControl.Report.Bands[BandKind.DetailReport] != null)
				AddPropertyItem(actionItems, "KeepTogetherWithDetailReports", "KeepTogetherWithDetailReports");
		}
	}
	public class BandDesignerActionList1 : XRComponentDesignerActionList {
		public bool KeepTogether {
			get { return ((Band)Component).KeepTogether; }
			set { SetPropertyValue("KeepTogether", value); }
		}
		public PageBreak PageBreak {
			get { return ((Band)Component).PageBreak; }
			set { SetPropertyValue("PageBreak", value); }
		}
		public BandDesignerActionList1(BandDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			AddPropertyItem(actionItems, "PageBreak", "PageBreak");
			AddPropertyItem(actionItems, "KeepTogether", "KeepTogether");
		}
	}
	public class GroupBandDesignerActionList : XRComponentDesignerActionList {
		public bool KeepTogether {
			get { return ((Band)Component).KeepTogether; }
			set { SetPropertyValue("KeepTogether", value); }
		}
		public bool RepeatEveryPage {
			get { return ((GroupBand)Component).RepeatEveryPage; }
			set { SetPropertyValue("RepeatEveryPage", value); }
		}
		public PageBreak PageBreak {
			get { return ((Band)Component).PageBreak; }
			set { SetPropertyValue("PageBreak", value); }
		}
		public GroupBandDesignerActionList(BandDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			AddPropertyItem(actionItems, "PageBreak", "PageBreak");
			AddPropertyItem(actionItems, "KeepTogether", "KeepTogether");
			AddPropertyItem(actionItems, "RepeatEveryPage", "RepeatEveryPage");
		}
	}
	public class GroupFooterBandDesignerActionList1 : XRComponentDesignerActionList {
		public GroupFooterUnion GroupUnion {
			get { return ((GroupFooterBand)Component).GroupUnion; }
			set { SetPropertyValue("GroupUnion", value); }
		}
		public GroupFooterBandDesignerActionList1(BandDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			AddPropertyItem(actionItems, "GroupUnion", "GroupUnion");
		}
	}
	public class GroupFooterBandDesignerActionList2 : GroupBandDesignerActionList {
		public bool PrintAtBottom {
			get { return ((GroupFooterBand)Component).PrintAtBottom; }
			set { SetPropertyValue("PrintAtBottom", value); }
		}
		public GroupFooterBandDesignerActionList2(BandDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "PrintAtBottom", "PrintAtBottom");
		}
	}
	public class ReportFooterBandDesignerActionList : XRComponentDesignerActionList {
		public bool KeepTogether {
			get { return ((Band)Component).KeepTogether; }
			set { SetPropertyValue("KeepTogether", value); }
		}
		public bool PrintAtBottom {
			get { return ((ReportFooterBand)Component).PrintAtBottom; }
			set { SetPropertyValue("PrintAtBottom", value); }
		}
		public ReportFooterBandDesignerActionList(BandDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			AddPropertyItem(actionItems, "KeepTogether", "KeepTogether");
			AddPropertyItem(actionItems, "PrintAtBottom", "PrintAtBottom");
		}
	}
	public class GroupHeaderBandDesignerActionList : XRComponentDesignerActionList {
		public GroupUnion GroupUnion {
			get { return ((GroupHeaderBand)Component).GroupUnion; }
			set { SetPropertyValue("GroupUnion", value); }
		}
		[Editor(typeof(GroupFieldCollectionEditor), typeof(UITypeEditor))]
		public GroupFieldCollection GroupFields {
			get { return ((GroupHeaderBand)Component).GroupFields; }
		}
		public int Level {
			get { return ((GroupHeaderBand)Component).Level; }
			set { SetPropertyValue("Level", value); }
		}
		[Editor(typeof(XRGroupSortingSummaryUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public XRGroupSortingSummary SortingSummary {
			get { return ((GroupHeaderBand)Component).SortingSummary; }
			set { SetPropertyValue("SortingSummary", value); }
		}
		public GroupHeaderBandDesignerActionList(BandDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			AddPropertyItem(actionItems, "GroupFields", "GroupFields");
			AddPropertyItem(actionItems, "GroupUnion", "GroupUnion");
			AddPropertyItem(actionItems, "Level", "Level");
			AddPropertyItem(actionItems, "SortingSummary", "SortingSummary");
		}
	}
	public class DetailBandDesignerActionList1 : XRComponentDesignerActionList {
		public GroupFieldCollection SortFields {
			get { return ((DetailBand)Component).SortFields; }
		}
		public DetailBandDesignerActionList1(BandDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "SortFields", "SortFields");
		}
	}
	public class DetailBandDesignerActionList2 : XRComponentDesignerActionList {
		DevExpress.XtraReports.UI.MultiColumn MultiColumn { get { return ((DetailBand)Component).MultiColumn; } }
		public MultiColumnMode ColumnMode {
			get { return MultiColumn.Mode; }
			set { SetPropertyValue(MultiColumn, "Mode", value); }
		}
		[RefreshProperties(RefreshProperties.All)]
		public int ColumnCount {
			get { return MultiColumn.ColumnCount; }
			set { SetPropertyValue(MultiColumn, "ColumnCount", value); }
		}
		[RefreshProperties(RefreshProperties.All)]
		public float ColumnWidth {
			get { return MultiColumn.ColumnWidth; }
			set { SetPropertyValue(MultiColumn, "ColumnWidth", value); }
		}
		public float ColumnSpacing {
			get { return MultiColumn.ColumnSpacing; }
			set { SetPropertyValue(MultiColumn, "ColumnSpacing", value); }
		}
		public ColumnLayout Layout {
			get { return MultiColumn.Layout; }
			set { SetPropertyValue(MultiColumn, "Layout", value); }
		}
		public DetailBandDesignerActionList2(BandDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			if(!PropertyIsBrowsable("MultiColumn")) return; 
			actionItems.Add(CreatePropertyItem("ColumnMode", ReportStringId.STag_Name_ColumnMode));
			actionItems.Add(CreatePropertyItem("Layout", ReportStringId.STag_Name_ColumnLayout));
			actionItems.Add(CreatePropertyItem("ColumnCount", ReportStringId.STag_Name_ColumnCount));
			actionItems.Add(CreatePropertyItem("ColumnWidth", ReportStringId.STag_Name_ColumnWidth));
			actionItems.Add(CreatePropertyItem("ColumnSpacing", ReportStringId.STag_Name_ColumnSpacing));
		}
	}
	public class ReportBaseDesignerActionList2 : XRComponentDesignerActionList {
		ReportPrintOptions ReportPrintOptions {
			get { return ((XtraReportBase)Component).ReportPrintOptions; }
		}
		public int DetailCountAtDesignTime {
			get { return ReportPrintOptions.DetailCountAtDesignTime; }
			set { SetPropertyValue(ReportPrintOptions, "DetailCountAtDesignTime", value); }
		}
		public ReportBaseDesignerActionList2(XRComponentDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			AddPropertyItem(actionItems, "DetailCountAtDesignTime", "DetailCountAtDesignTime", ReportPrintOptions);
		}
	}
	public class ReportBaseDesignerActionList3 : XRComponentDesignerActionList {
		public PageBreak PageBreak {
			get { return ((Band)Component).PageBreak; }
			set { SetPropertyValue("PageBreak", value); }
		}
		public ReportBaseDesignerActionList3(XRComponentDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			AddPropertyItem(actionItems, "PageBreak", "PageBreak");
		}
	}
	public class RootReportDesignerActionList1 : XRComponentDesignerActionList {
		public ReportUnit ReportUnit {
			get { return ((XtraReport)Component).ReportUnit; }
			set { SetPropertyValue("ReportUnit", value); }
		}
		public RootReportDesignerActionList1(XRComponentDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			AddPropertyItem(actionItems, "ReportUnit", "ReportUnit");
		}
	}
	public class RootReportDesignerActionList2 : XRFormattingControlDesignerActionList {
		[Editor(typeof(DevExpress.XtraReports.Design.XRWatermarkEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public XRWatermark Watermark { get { return ((XtraReport)Component).Watermark; } }
		[Editor(typeof(DevExpress.XtraReports.Design.XRControlStylesEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public XRControlStyleSheet StyleSheet { get { return ((XtraReport)Component).StyleSheet; } }
		public FormattingRuleSheet FormattingRuleSheet { get { return ((XtraReport)Component).FormattingRuleSheet; } }
		public RootReportDesignerActionList2(XRComponentDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "FormattingRuleSheet", "FormattingRuleSheet");
			AddPropertyItem(actionItems, "StyleSheet", "StyleSheet");
			AddPropertyItem(actionItems, "Watermark", "Watermark");
		}
	}
	public class DataContainerDesignerActionList : XRComponentDesignerActionList {
		IDataContainer DataContainer { get { return (IDataContainer)Component; } }
		[
		TypeConverter(typeof(DataMemberTypeConverter)),
		Editor(typeof(DataContainerDataMemberEditor), typeof(UITypeEditor))
		]
		public virtual string DataMember {
			get { return DataContainer.DataMember; }
			set { SetPropertyValue("DataMember", value); }
		}
		[
		Editor(typeof(DataSourceEditor), typeof(UITypeEditor)),
		TypeConverter(typeof(DataSourceConverter)),
		RefreshProperties(RefreshProperties.All),
		]
		public object DataSource {
			get { return DataContainer.DataSource; }
			set { SetPropertyValue("DataSource", value); }
		}
		[
		Editor(typeof(DataAdapterEditor), typeof(UITypeEditor)),
		TypeConverterAttribute(typeof(DataAdapterConverter))
		]
		public object DataAdapter {
			get { return DataContainer.DataAdapter; }
			set { SetPropertyValue("DataAdapter", value); }
		}
		public DataContainerDesignerActionList(XRComponentDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			AddPropertyItem(actionItems, "DataSource", "DataSource");
			AddPropertyItem(actionItems, "DataMember", "DataMember");
			AddPropertyItem(actionItems, "DataAdapter", "DataAdapter");
		}
	}
	public class ReportBaseDesignerActionList : DataContainerDesignerActionList {
		XtraReportBase Report { get { return (XtraReportBase)Component; } }
		[
		TypeConverter(typeof(DataMemberTypeConverter)),
		Editor(typeof(ReportDataMemberEditor), typeof(UITypeEditor))
		]
		public override string DataMember {
			get { return base.DataMember; }
			set { base.DataMember = value; }
		}
		[
		TypeConverter(typeof(TextPropertyTypeConverter)),
		Editor(typeof(FilterStringEditor), typeof(UITypeEditor)),
		]
		public string FilterString {
			get { return Report.FilterString; }
			set { SetPropertyValue("FilterString", value); }
		}
		public ReportBaseDesignerActionList(XRComponentDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "FilterString", "FilterString");
		}
	}
	#endregion
	#region AllSmartTagControls
	public class XRFormattingControlDesignerActionList : XRComponentDesignerActionList {
		[Editor(typeof(DevExpress.XtraReports.Design.FormattingRulesEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public FormattingRuleCollection FormattingRules {
			get { return ((XRControl)Component).FormattingRules; }
			set { SetPropertyValue("FormattingRules", value); }
		}
		public XRFormattingControlDesignerActionList(XRComponentDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			XRComponentDesigner componentDesigner = designer as XRComponentDesigner;
			if(componentDesigner != null)
				AddPropertyItem(actionItems, "FormattingRules", "FormattingRules");
		}
	}
	#endregion
}
