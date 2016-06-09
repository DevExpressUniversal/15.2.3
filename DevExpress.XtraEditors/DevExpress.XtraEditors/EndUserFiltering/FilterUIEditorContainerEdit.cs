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
using System.Windows.Forms;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Design;
using DevExpress.Utils.Filtering;
using DevExpress.Utils.Filtering.Internal;
using DevExpress.XtraEditors.Filtering.Repository;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.XtraEditors.Filtering {
	[ToolboxItem(false)]
	public class FilterUIEditorContainerEdit : BaseEdit {
		public FilterUIEditorContainerEdit() { }
		protected override void Dispose(bool disposing) {
			if(uiEditorContainerCore != null)
				uiEditorContainerCore.Dispose();
			base.Dispose(disposing);
		}
		protected override void CreateRepositoryItem() {
			this.fProperties = new RepositoryItemFilterUIEditorContainerEdit();
			this.fProperties.SetOwnerEdit(this);
		}
		public override string EditorTypeName {
			get { return "FilterUIEditorContainerEdit"; }
		}
		[DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagSearchNestedProperties]
		public new RepositoryItemFilterUIEditorContainerEdit Properties {
			get { return base.Properties as RepositoryItemFilterUIEditorContainerEdit; }
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
		[Browsable(false)]
		public override bool IsEditorActive {
			get {
				if(!this.Enabled)
					return false;
				IContainerControl container = GetContainerControl();
				if(container == null) return EditorContainsFocus;
				return container.ActiveControl == this || Contains(container.ActiveControl);
			}
		}
		protected internal override Control InnerControl {
			get { return UIEditorContainer.Template; }
		}
		protected override void OnGotFocus(EventArgs e) {
			base.OnGotFocus(e);
			Control c = UIEditorContainer.Template;
			if(c != null)
				if(ContainsFocus && !c.ContainsFocus) c.Focus();
		}
		internal Size GetTemplateSize() {
			return GetTemplateMinSize();
		}
		protected override bool SizeableIsCaptionVisible {
			get { return false; }
		}
		protected override Size CalcSizeableMinSize() {
			return GetTemplateMinSize();
		}
		protected override Size CalcSizeableMaxSize() {
			return GetTemplateMaxSize();
		}
		Size GetTemplateMinSize() {
			if(UIEditorContainer == null || !(UIEditorContainer.Template is IXtraResizableControl))
				return Size.Empty;
			return ((IXtraResizableControl)UIEditorContainer.Template).MinSize;
		}
		Size GetTemplateMaxSize() {
			if(UIEditorContainer == null || !(UIEditorContainer.Template is IXtraResizableControl))
				return Size.Empty;
			return ((IXtraResizableControl)UIEditorContainer.Template).MaxSize;
		}
		internal void SizeableChanged() {
			RaiseSizeableChanged();
			LayoutChanged();
		}
	}
}
namespace DevExpress.XtraEditors.Filtering.Repository {
	using System.ComponentModel.DataAnnotations;
	using System.Drawing;
	using DevExpress.Utils;
	using DevExpress.XtraEditors.Controls;
	using DevExpress.XtraEditors.Drawing;
	using DevExpress.XtraEditors.Registrator;
	using DevExpress.XtraEditors.ViewInfo;
	[ToolboxItem(false)]
	public class RepositoryItemFilterUIEditorContainerEdit : RepositoryItem {
		static RepositoryItemFilterUIEditorContainerEdit() {
			RegisterFilterUIEditorContainerEdit();
		}
		internal static void RegisterFilterUIEditorContainerEdit() {
			EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo("FilterUIEditorContainerEdit",
				typeof(FilterUIEditorContainerEdit),
				typeof(RepositoryItemFilterUIEditorContainerEdit),
				typeof(RepositoryItemFilterUIEditorContainerEditViewInfo),
				new RepositoryItemFilterUIEditorContainerEditPainter(),
				true, null, typeof(DevExpress.Accessibility.BaseEditAccessible)));
		}
		public RepositoryItemFilterUIEditorContainerEdit() {
			this.fBorderStyle = BorderStyles.NoBorder;
		}
		public RepositoryItemFilterUIEditorContainerEdit(RangeUIEditorType editorType, DataType? dataType)
			: this() {
			RangeUIEditorType = editorType;
			DataType = dataType;
		}
		public RepositoryItemFilterUIEditorContainerEdit(DateTimeRangeUIEditorType editorType, DataType? dataType)
			: this() {
			DateTimeRangeUIEditorType = editorType;
			DataType = dataType;
		}
		public RepositoryItemFilterUIEditorContainerEdit(LookupUIEditorType editorType, bool useFlags)
			: this() {
			LookupUIEditorType = editorType;
			UseFlags = useFlags;
		}
		public RepositoryItemFilterUIEditorContainerEdit(BooleanUIEditorType editorType)
			: this() {
			BooleanUIEditorType = editorType;
		}
		public RepositoryItemFilterUIEditorContainerEdit(LookupUIEditorType editorType, Type enumType, bool useFlags)
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
			RepositoryItemFilterUIEditorContainerEdit source = item as RepositoryItemFilterUIEditorContainerEdit;
			if(source != null) {
				this.RangeUIEditorType = source.RangeUIEditorType;
				this.DateTimeRangeUIEditorType = source.DateTimeRangeUIEditorType;
				this.DataType = source.DataType;
				this.LookupUIEditorType = source.LookupUIEditorType;
				this.BooleanUIEditorType = source.BooleanUIEditorType;
				this.EnumType = source.EnumType;
				this.UseFlags = source.UseFlags;
			}
			this.fBorderStyle = item.BorderStyle;
			base.Assign(item);
		}
		[DXCategory(CategoryName.Appearance),  DefaultValue(BorderStyles.NoBorder)]
		public override BorderStyles BorderStyle {
			get { return base.BorderStyle; }
			set { base.BorderStyle = value; }
		}
		public override string EditorTypeName {
			get { return "FilterUIEditorContainerEdit"; }
		}
		protected new FilterUIEditorContainerEdit OwnerEdit {
			get { return base.OwnerEdit as FilterUIEditorContainerEdit; }
		}
	}
	public class RepositoryItemFilterUIEditorContainerEditViewInfo : BaseEditViewInfo {
		public RepositoryItemFilterUIEditorContainerEditViewInfo(RepositoryItem item)
			: base(item) {
		}
		public override bool AllowDrawFocusRect {
			get { return false; }
			set { }
		}
		protected internal override bool AllowDrawParentBackground {
			get { return true; }
		}
		public override AppearanceDefault DefaultAppearance {
			get {
				if(OwnerEdit != null && OwnerEdit.InplaceType == InplaceType.Standalone && OwnerEdit.Parent != null) {
					AppearanceDefault res = base.DefaultAppearance.Clone() as AppearanceDefault;
					res.BackColor = Color.Transparent;
					return res;
				}
				return base.DefaultAppearance;
			}
		}
		protected override Size CalcContentSize(Graphics g) {
			Size res = base.CalcContentSize(g);
			if(OwnerEdit == null) return res;
			Size editSize = OwnerEdit.GetTemplateSize();
			res.Height = Math.Max(editSize.Height, res.Height);
			res.Width = Math.Max(editSize.Width, res.Width);
			return res;
		}
		public new FilterUIEditorContainerEdit OwnerEdit {
			get { return base.OwnerEdit as FilterUIEditorContainerEdit; }
		}
		public new RepositoryItemFilterUIEditorContainerEdit Item {
			get { return base.Item as RepositoryItemFilterUIEditorContainerEdit; }
		}
	}
	public class RepositoryItemFilterUIEditorContainerEditPainter : BaseEditPainter {
	}
}
