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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Design;
using DevExpress.Utils.Filtering;
using DevExpress.Utils.Filtering.Internal;
using DevExpress.XtraEditors.Filtering.Popup;
using DevExpress.XtraEditors.Filtering.Repository;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.XtraEditors.Filtering {
	[ToolboxItem(false)]
	public class FilterUIEditorPopupContainerEdit : PopupContainerEdit {
		public FilterUIEditorPopupContainerEdit() { }
		protected override void Dispose(bool disposing) {
			if(uiEditorContainerCore != null)
				uiEditorContainerCore.Dispose();
			base.Dispose(disposing);
		}
		public override string EditorTypeName {
			get { return "FilterUIEditorPopupContainerEdit"; }
		}
		[DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagSearchNestedProperties]
		public new RepositoryItemFilterUIEditorPopupContainerEdit Properties {
			get { return base.Properties as RepositoryItemFilterUIEditorPopupContainerEdit; }
		}
		protected FilterUIEditorPopupContainerForm PopupContainerForm {
			get { return base.PopupForm as FilterUIEditorPopupContainerForm; }
		}
		protected override void CreateRepositoryItem() {
			this.fProperties = new RepositoryItemFilterUIEditorPopupContainerEdit();
			this.fProperties.SetOwnerEdit(this);
		}
		protected override PopupBaseForm CreatePopupForm() {
			return new FilterUIEditorPopupContainerForm(this);
		}
		protected virtual PopupContainerControl CreatePopupControl() {
			return new PopupContainerControl();
		}
		protected virtual FilterUIEditorTemplateContainer CreateFilterUIEditorContainer() {
			return new FilterUIEditorTemplateContainer(Properties);
		}
		FilterUIEditorTemplateContainer uiEditorContainerCore;
		protected FilterUIEditorTemplateContainer UIEditorContainer {
			get {
				if(uiEditorContainerCore == null && IsHandleCreated)
					uiEditorContainerCore = CreateFilterUIEditorContainer();
				return uiEditorContainerCore;
			}
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			if(EditValue is IValueViewModel)
				UIEditorContainer.ValueViewModel = EditValue as IValueViewModel;
		}
		protected override void OnEditValueChanged() {
			base.OnEditValueChanged();
			if(IsHandleCreated)
				UIEditorContainer.ValueViewModel = EditValue as IValueViewModel;
		}
		protected override bool CanShowPopup {
			get { return base.CanShowPopup && IsHandleCreated && EditValue is IValueViewModel; }
		}
		protected override void DoShowPopup() {
			CheckPopupControl();
			base.DoShowPopup();
		}
		void CheckPopupControl() {
			if(Properties.PopupControl == null) {
				this.Properties.PopupControl = CreatePopupControl();
				this.Properties.PopupControl.Controls.Add(UIEditorContainer.Template);
				Size size = Properties.GetDesiredPopupFormSize(true);
				if(size.Width <= 0)
					Properties.PopupControl.Width = Math.Max(Properties.PopupControl.Width, this.Width);
				if(size.Height <= 0)
					Properties.PopupControl.Height = GetPopupHeight();
			}
		}
		int GetPopupHeight() {
			return Math.Max(Height, ((IXtraResizableControl)UIEditorContainer.Template).MinSize.Height);
		}
	}
}
namespace DevExpress.XtraEditors.Filtering.Popup {
	[ToolboxItem(false)]
	public class FilterUIEditorPopupContainerForm : PopupContainerForm {
		public FilterUIEditorPopupContainerForm(FilterUIEditorPopupContainerEdit ownerEdit)
			: base(ownerEdit) {
		}
		new RepositoryItemFilterUIEditorPopupContainerEdit Properties {
			get {
				FilterUIEditorPopupContainerEdit edit = base.OwnerEdit as FilterUIEditorPopupContainerEdit;
				if(edit == null) return null;
				return edit.Properties;
			}
		}
	}
}
namespace DevExpress.XtraEditors.Filtering.Repository {
	using System.ComponentModel.DataAnnotations;
	using DevExpress.XtraEditors.Controls;
	using DevExpress.XtraEditors.Drawing;
	using DevExpress.XtraEditors.Registrator;
	[ToolboxItem(false)]
	public class RepositoryItemFilterUIEditorPopupContainerEdit : RepositoryItemPopupContainerEdit {
		static RepositoryItemFilterUIEditorPopupContainerEdit() {
			RegisterFilterUIEditorPopupContainerEdit();
		}
		internal static void RegisterFilterUIEditorPopupContainerEdit() {
			EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo("FilterUIEditorPopupContainerEdit",
				typeof(FilterUIEditorPopupContainerEdit),
				typeof(RepositoryItemFilterUIEditorPopupContainerEdit),
				typeof(DevExpress.XtraEditors.ViewInfo.PopupContainerEditViewInfo),
				new ButtonEditPainter(),
				true, null, typeof(DevExpress.Accessibility.ComboBoxEditAccessible)));
		}
		public RepositoryItemFilterUIEditorPopupContainerEdit() {
			fTextEditStyle = TextEditStyles.DisableTextEditor;
		}
		public RepositoryItemFilterUIEditorPopupContainerEdit(RangeUIEditorType editorType, DataType? dataType)
			: this() {
			RangeUIEditorType = editorType;
			DataType = dataType;
		}
		public RepositoryItemFilterUIEditorPopupContainerEdit(DateTimeRangeUIEditorType editorType, DataType? dataType)
			: this() {
			DateTimeRangeUIEditorType = editorType;
			DataType = dataType;
		}
		public RepositoryItemFilterUIEditorPopupContainerEdit(LookupUIEditorType editorType, bool useFlags)
			: this() {
			LookupUIEditorType = editorType;
			UseFlags = useFlags;
		}
		public RepositoryItemFilterUIEditorPopupContainerEdit(BooleanUIEditorType editorType)
			: this() {
			BooleanUIEditorType = editorType;
		}
		public RepositoryItemFilterUIEditorPopupContainerEdit(LookupUIEditorType editorType, Type enumType, bool useFlags)
			: this(editorType, useFlags) {
			EnumType = enumType;
		}
		[DefaultValue(null), Category("EditorType")]
		public RangeUIEditorType? RangeUIEditorType { get; set; }
		[DefaultValue(null), Category("EditorType")]
		public DateTimeRangeUIEditorType? DateTimeRangeUIEditorType { get; set; }
		[DefaultValue(null), Category("Data")]
		public DataType? DataType { get; set; }
		[DefaultValue(null), Category("EditorType")]
		public LookupUIEditorType? LookupUIEditorType { get; set; }
		[DefaultValue(null), Category("EditorType")]
		public BooleanUIEditorType? BooleanUIEditorType { get; set; }
		[DefaultValue(null), Category("Data")]
		public Type EnumType { get; set; }
		[DefaultValue(false), Category("Data")]
		public bool UseFlags { get; set; }
		public override void Assign(DevExpress.XtraEditors.Repository.RepositoryItem item) {
			RepositoryItemFilterUIEditorPopupContainerEdit source = item as RepositoryItemFilterUIEditorPopupContainerEdit;
			this.RangeUIEditorType = source.RangeUIEditorType;
			this.DateTimeRangeUIEditorType = source.DateTimeRangeUIEditorType;
			this.DataType = source.DataType;
			this.LookupUIEditorType = source.LookupUIEditorType;
			this.BooleanUIEditorType = source.BooleanUIEditorType;
			this.EnumType = source.EnumType;
			this.UseFlags = source.UseFlags;
			base.Assign(item);
		}
		public override string EditorTypeName {
			get { return "FilterUIEditorPopupContainerEdit"; }
		}
		protected internal override bool IsReadOnlyAllowsDropDown {
			get { return AllowDropDownWhenReadOnly != DefaultBoolean.False; }
		}
		protected new FilterUIEditorPopupContainerEdit OwnerEdit {
			get { return base.OwnerEdit as FilterUIEditorPopupContainerEdit; }
		}
		protected internal override bool UseMaskBox {
			get { return false; }
		}
		[DXCategory(CategoryName.Behavior),  DefaultValue(TextEditStyles.DisableTextEditor)]
		public override TextEditStyles TextEditStyle {
			get { return base.TextEditStyle; }
			set {
				if(value == TextEditStyles.Standard)
					value = TextEditStyles.DisableTextEditor;
				base.TextEditStyle = value;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override PopupContainerControl PopupControl {
			get { return base.PopupControl; }
			set { base.PopupControl = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DevExpress.XtraEditors.Mask.MaskProperties Mask {
			get { return base.Mask; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(true)]
		public override SimpleContextItemCollectionOptions ContextButtonOptions {
			get { return base.ContextButtonOptions; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof(ExpandableObjectConverter)), Browsable(true)]
		public override ContextItemCollection ContextButtons {
			get { return base.ContextButtons; }
		}
		public override string GetDisplayText(FormatInfo format, object editValue) {
			IFilterValueViewModel valueViewModel = editValue as IFilterValueViewModel;
			if(valueViewModel != null) {
				string formatString = format.FormatString;
				if(string.IsNullOrEmpty(formatString) && AnnotationAttributes.HasDisplayFormatAttribute) 
					formatString = AnnotationAttributes.DataFormatString;
			}
			return base.GetDisplayText(format, editValue);
		}
	}
}
