#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
namespace DevExpress.ExpressApp.Win.Editors {
	public class BooleanPropertyEditor : DXPropertyEditor, IHtmlFormattingSupport {
		protected override object CreateControlCore() {
			if(IsDefinedBoolImages) {
				if(IsDefinedBoolCaption) {
					return new BoolImageComboBoxEdit(Model.ImageForTrue, Model.ImageForFalse, Model.CaptionForTrue, Model.CaptionForFalse);
				}
				else {
					return new BoolImageComboBoxEdit(Model.ImageForTrue, Model.ImageForFalse);
				}
			}
			else if(IsDefinedBoolCaption) {
				return new BoolComboBoxEdit(Model.CaptionForTrue, Model.CaptionForFalse);
			}
			else {
				return new BooleanEdit(Model.Caption);
			}
		}
		protected override void OnControlCreated() {
			base.OnControlCreated();
			ApplyHtmlFormatting();
		}
		protected override RepositoryItem CreateRepositoryItem() {
			if(IsDefinedBoolImages) {
				if(IsDefinedBoolCaption) {
					return new RepositoryItemBoolImageComboBoxEdit(Model.ImageForTrue, Model.ImageForFalse, Model.CaptionForTrue, Model.CaptionForFalse);
				}
				else {
					return new RepositoryItemBoolImageComboBoxEdit(Model.ImageForTrue, Model.ImageForFalse);
				}
			}
			else if(IsDefinedBoolCaption) {
				return new RepositoryItemBoolComboBoxEdit(Model.CaptionForTrue, Model.CaptionForFalse);
			}
			else {
				return new RepositoryItemBooleanEdit();
			}
		}
		protected override void UpdateControlEnabled(bool enabled) {
			if (Control is BooleanEdit) {
				base.UpdateControlEnabled(enabled);
			}
		}
		protected override void InitializeAppearance(RepositoryItem item) {
			base.InitializeAppearance(item);
			if(item is RepositoryItemBooleanEdit) {
				item.Appearance.BackColor = Color.Transparent;
			}
		}
		protected override void SetTestTag() {
			if(IsDefinedBoolCaption) {
				Control.Tag = EasyTestTagHelper.FormatTestField(Caption);
			}
			else {
				Control.Tag = EasyTestTagHelper.FormatTestField(Control.Text);
			}
		}
		protected bool IsDefinedBoolImages {
			get { return Model != null && !string.IsNullOrEmpty(Model.ImageForFalse) && !string.IsNullOrEmpty(Model.ImageForTrue); }
		}
		protected bool IsDefinedBoolCaption {
			get { return Model != null && !string.IsNullOrEmpty(Model.CaptionForFalse) && !string.IsNullOrEmpty(Model.CaptionForTrue); }
		}
		public BooleanPropertyEditor(Type objectType, IModelMemberViewItem model)
			: base(objectType, model) {
			ControlBindingProperty = "Checked";
			CanUpdateControlEnabled = true;
		}
		public override bool IsCaptionVisible {
			get { return IsDefinedBoolCaption || IsDefinedBoolImages; }
		}
		public override string Caption {
			get { return base.Caption; }
			set {
				base.Caption = value;
				if(Control is BooleanEdit) {
					Control.Text = value;
				}
			}
		}
		#region IHtmlFormattingSupport Members
		private bool htmlFormattingEnabled;
		public void SetHtmlFormattingEnabled(bool htmlFormattingEnabled) {
			this.htmlFormattingEnabled = htmlFormattingEnabled;
			ApplyHtmlFormatting();
		}
		private void ApplyHtmlFormatting() {
			if(Control is BooleanEdit) {
				Control.Properties.AllowHtmlDraw = htmlFormattingEnabled ? DefaultBoolean.True : DefaultBoolean.False;
			}
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class BooleanEdit : CheckEdit {
		static BooleanEdit() {
			RepositoryItemBooleanEdit.Register();
		}
		public BooleanEdit() { }
		public BooleanEdit(string caption) {
			Height = WinPropertyEditor.TextControlHeight;
			Text = caption;
		}
		public override string EditorTypeName { get { return RepositoryItemBooleanEdit.EditorName; } }
	}
	public class RepositoryItemBooleanEdit : RepositoryItemCheckEdit {
		protected internal const string EditorName = "BoolEdit";
		protected internal static void Register() {
			if(!EditorRegistrationInfo.Default.Editors.Contains(EditorName)) {
				EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(BooleanEdit),
					typeof(RepositoryItemBooleanEdit), typeof(CheckEditViewInfo),
					new CheckEditPainter(), true, EditImageIndexes.CheckEdit, typeof(DevExpress.Accessibility.CheckEditAccessible)));
			}
		}
		static RepositoryItemBooleanEdit() {
			RepositoryItemBooleanEdit.Register();
		}
		public override string EditorTypeName { get { return EditorName; } }
		public RepositoryItemBooleanEdit() { }
	}
	[System.ComponentModel.ToolboxItem(false)]
	public class BoolComboBoxEdit : ImageComboBoxEdit {
		static BoolComboBoxEdit() {
			RepositoryItemBoolComboBoxEdit.Register();
		}
		protected override void OnEditValueChanged() {
			base.OnEditValueChanged();
			if(CheckedChanged != null) {
				CheckedChanged(this, EventArgs.Empty);
			}
		}
		public BoolComboBoxEdit() { }
		public BoolComboBoxEdit(string trueCaption, string falseCaption) {
			((RepositoryItemBoolComboBoxEdit)Properties).Init(trueCaption, falseCaption);
			EditValue = false;
		}
		public override string EditorTypeName {
			get { return RepositoryItemBoolComboBoxEdit.EditorName; }
		}
		public bool Checked {
			get { return (bool)base.EditValue; }
			set { base.EditValue = value; }
		}
		public event EventHandler CheckedChanged;
	}
	public class RepositoryItemBoolComboBoxEdit : RepositoryItemImageComboBox {
		protected internal const string EditorName = "BoolComboBoxEdit";
		protected internal static void Register() {
			if(!EditorRegistrationInfo.Default.Editors.Contains(EditorName)) {
				EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(BoolComboBoxEdit),
					typeof(RepositoryItemBoolComboBoxEdit), typeof(ImageComboBoxEditViewInfo),
					 new ImageComboBoxEditPainter(), true, EditImageIndexes.ImageComboBoxEdit, typeof(DevExpress.Accessibility.PopupEditAccessible)));
			}
		}
		static RepositoryItemBoolComboBoxEdit() {
			RepositoryItemBoolComboBoxEdit.Register();
		}
		public override string EditorTypeName {
			get { return EditorName; }
		}
		public void Init(string trueCaption, string falseCaption) {
			Items.Add(new ImageComboBoxItem(trueCaption, true));
			Items.Add(new ImageComboBoxItem(falseCaption, false));
		}
		public RepositoryItemBoolComboBoxEdit(string trueCaption, string falseCaption)
			: this() {
			Init(trueCaption, falseCaption);
		}
		public RepositoryItemBoolComboBoxEdit() {
			TextEditStyle = TextEditStyles.DisableTextEditor;
			ShowDropDown = ShowDropDown.SingleClick;
		}
	}
	[System.ComponentModel.ToolboxItem(false)]
	public class BoolImageComboBoxEdit : ImageComboBoxEdit {
		static BoolImageComboBoxEdit() {
			RepositoryItemBoolImageComboBoxEdit.Register();
		}
		protected override void OnEditValueChanged() {
			base.OnEditValueChanged();
			if(CheckedChanged != null) {
				CheckedChanged(this, EventArgs.Empty);
			}
		}
		public BoolImageComboBoxEdit() { }
		public BoolImageComboBoxEdit(string imageNameForTrue, string imageNameForFalse)
			: this(imageNameForTrue, imageNameForFalse, "", "") { }
		public BoolImageComboBoxEdit(string imageNameForTrue, string imageNameForFalse, string captionForTrue, string captionForFalse) {
			((RepositoryItemBoolImageComboBoxEdit)Properties).Init(imageNameForTrue, imageNameForFalse, captionForTrue, captionForFalse);
			EditValue = false;
		}
		public override string EditorTypeName {
			get { return RepositoryItemBoolImageComboBoxEdit.EditorName; }
		}
		public bool Checked {
			get { return (bool)base.EditValue; }
			set { base.EditValue = value; }
		}
		public event EventHandler CheckedChanged;
	}
	public class RepositoryItemBoolImageComboBoxEdit : RepositoryItemImageComboBox {
		protected internal const string EditorName = "BoolImageComboBoxEdit";
		protected internal static void Register() {
			if(!EditorRegistrationInfo.Default.Editors.Contains(EditorName)) {
				EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(BoolImageComboBoxEdit),
					typeof(RepositoryItemBoolImageComboBoxEdit), typeof(ImageComboBoxEditViewInfo),
					 new ImageComboBoxEditPainter(), true, EditImageIndexes.ImageComboBoxEdit, typeof(DevExpress.Accessibility.PopupEditAccessible)));
			}
		}
		static RepositoryItemBoolImageComboBoxEdit() {
			RepositoryItemBoolImageComboBoxEdit.Register();
		}
		public RepositoryItemBoolImageComboBoxEdit(string imageNameForTrue, string imageNameForFalse)
			: this(imageNameForTrue, imageNameForFalse, "", "") { }
		public RepositoryItemBoolImageComboBoxEdit(string imageNameForTrue, string imageNameForFalse, string captionForTrue, string captionForFalse)
			: this() {
			Init(imageNameForTrue, imageNameForFalse, captionForTrue, captionForFalse);
		}
		public RepositoryItemBoolImageComboBoxEdit() {
			TextEditStyle = TextEditStyles.DisableTextEditor;
			ShowDropDown = ShowDropDown.SingleClick;
		}
		public void Init(string imageNameForTrue, string imageNameForFalse) {
			Init(imageNameForTrue, imageNameForFalse, "", "");
		}
		public void Init(string imageNameForTrue, string imageNameForFalse, string captionForTrue, string captionForFalse) {
			DevExpress.ExpressApp.Utils.ImageInfo trueImageInfo = ImageLoader.Instance.GetImageInfo(imageNameForTrue);
			DevExpress.ExpressApp.Utils.ImageInfo falseImageInfo = ImageLoader.Instance.GetImageInfo(imageNameForFalse);
			int trueImageIndex = -1;
			int falseImageIndex = -1;
			ImageCollection imageCollection = new ImageCollection();
			if(!trueImageInfo.IsEmpty) {
				imageCollection.AddImage(trueImageInfo.Image);
				imageCollection.ImageSize = trueImageInfo.Image.Size;
				trueImageIndex = imageCollection.Images.Count - 1;
			}
			if(!falseImageInfo.IsEmpty) {
				imageCollection.AddImage(falseImageInfo.Image);
				imageCollection.ImageSize = falseImageInfo.Image.Size;
				falseImageIndex = imageCollection.Images.Count - 1;
			}
			SmallImages = imageCollection;
			Items.Add(new ImageComboBoxItem(captionForTrue, true, trueImageIndex));
			Items.Add(new ImageComboBoxItem(captionForFalse, false, falseImageIndex));
		}
		public override string EditorTypeName {
			get { return EditorName; }
		}
	}
}
