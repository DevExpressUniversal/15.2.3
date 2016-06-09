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
using System.ComponentModel;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Popup;
namespace DevExpress.XtraEditors.Repository {
	public class RepositoryItemBlobBaseEdit : RepositoryItemPopupBase {
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoletePropertiesText)]
		public new RepositoryItemBlobBaseEdit Properties { get { return this; } }
		object fImages;
		bool fShowIcon, fPopupSizeable;
		public RepositoryItemBlobBaseEdit() {
			this.fTextEditStyle = TextEditStyles.DisableTextEditor;
			this.fImages = null;
			this.fPopupSizeable = true;
			this.fShowIcon = true;
		}
		protected internal override bool IsReadOnlyAllowsDropDown {
			get {
				return AllowDropDownWhenReadOnly != DefaultBoolean.False;
			}
		}
		protected internal override bool UseMaskBox { get { return false; } }
		public override void Assign(RepositoryItem item) {
			RepositoryItemBlobBaseEdit source = item as RepositoryItemBlobBaseEdit;
			BeginUpdate(); 
			try {
				base.Assign(item);
				if(source == null) return;
				this.fImages = source.Images;
				this.fPopupSizeable = source.PopupSizeable;
				this.fShowIcon = source.ShowIcon;
			}
			finally {
				EndUpdate();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DevExpress.XtraEditors.Mask.MaskProperties Mask {
			get { return base.Mask; }
		}
		protected internal virtual object DefaultImages { get { return null; } }
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBlobBaseEditTextEditStyle"),
#endif
 DefaultValue(TextEditStyles.DisableTextEditor)]
		public override TextEditStyles TextEditStyle {
			get { return base.TextEditStyle; }
			set {
				if(value == TextEditStyles.Standard) value = TextEditStyles.DisableTextEditor;
				base.TextEditStyle = value;
			}
		}
		[Obsolete(ObsoleteText.SRObsoletePopupStartSize), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Size PopupStartSize {
			get { return PopupFormSize; }
			set { PopupFormSize = value; }
		}
		[Browsable(false)]
		public override string EditorTypeName { get { return "BlobBaseEdit"; } }
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBlobBaseEditImages"),
#endif
 DefaultValue(null),
		TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))
		]
		public object Images {
			get { return fImages; }
			set { 
				if(Images == value) return;
				fImages = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBlobBaseEditShowIcon"),
#endif
 DefaultValue(true)]
		public bool ShowIcon {
			get { return fShowIcon; }
			set {
				if(ShowIcon == value) return;
				fShowIcon = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBlobBaseEditPopupSizeable"),
#endif
 DefaultValue(true)]
		public virtual bool PopupSizeable {
			get { return fPopupSizeable; }
			set {
				if(PopupSizeable == value) return;
				fPopupSizeable = value;
				OnPropertiesChanged();
			}
		}
		[Browsable(false)]
		public override HorzAlignment DefaultAlignment { get { return HorzAlignment.Near; } }
		protected override bool NeededKeysPopupContains(Keys key) {
			if(key == (Keys.Enter | Keys.Control))
				return true;
			return base.NeededKeysPopupContains(key);
		}
		public override bool IsNonSortableEditor { get { return true; } }
	}
}
namespace DevExpress.XtraEditors {
	[ToolboxItem(false), 
		Designer("DevExpress.XtraEditors.Design.BlobEditDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign)]
	public abstract class BlobBaseEdit : PopupBaseEdit {
		public BlobBaseEdit() {
		}
		[Browsable(false)]
		public override string EditorTypeName { get { return "BlobBaseEdit"; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BlobBaseEditProperties"),
#endif
 DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemBlobBaseEdit Properties { get { return base.Properties as RepositoryItemBlobBaseEdit; } }
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	public class BlobBaseEditViewInfo : PopupBaseEditViewInfo {
		public BlobBaseEditViewInfo(RepositoryItem item) : base(item) {
		}
		public new RepositoryItemBlobBaseEdit Item { get { return base.Item as RepositoryItemBlobBaseEdit; } }
		public override object Images {
			get { return Item.Images != null ? Item.Images : Item.DefaultImages; } 
		}
		public override int ImageIndex { 
			get { 
				int res = IsDataEmpty ? 1 : 0;
				if(ImageCollection.GetImageListImageCount(Images) == 1) res = 0;
				return res;
			} 
		}
		protected virtual bool IsDataEmpty {
			get { return EditValue == null || EditValue == DBNull.Value; }
		}
		public override TextGlyphDrawModeEnum GlyphDrawMode {
			get {
				if(!Item.ShowIcon || ImageSize.IsEmpty) return TextGlyphDrawModeEnum.Text;
				return TextGlyphDrawModeEnum.Glyph;
			}
		}
		public override HorzAlignment GlyphAlignment { get { return HorzAlignment.Center; } }
		protected override void CalcBestFitTextSize(Graphics g) {
			CalcTextSize(g, false);
		}
		protected override void CalcContentRect(Rectangle bounds) {
			base.CalcContentRect(bounds);
			if(GlyphDrawMode == TextGlyphDrawModeEnum.Glyph) this.fMaskBoxRect = Rectangle.Empty;
		}
		protected override void CalcShowHint() { 
			this.toolTipInfoCalculated = true;
			ShowTextToolTip = false;
		}
	}
}
namespace DevExpress.XtraEditors.Drawing {
	public class BlobBaseEditPainter : ButtonEditPainter {
	}
}
namespace DevExpress.XtraEditors.Popup {
	public class BlobBasePopupForm : CustomBlobPopupForm {
		int lockControlUpdate = 0;
		public BlobBasePopupForm(BlobBaseEdit ownerEdit) : base(ownerEdit) {
			this.AllowSizing = OwnerEdit.Properties.PopupSizeable;
		}
		public virtual void BeginControlUpdate() {
			lockControlUpdate ++;
		}
		public virtual void EndControlUpdate() {
			lockControlUpdate --;
		}
		protected virtual bool IsControlUpdateLocked { get { return lockControlUpdate != 0; } }
		protected override void SetupButtons() {
			this.fShowOkButton = true;
			this.fCloseButtonStyle = BlobCloseButtonStyle.Caption;
		}
		[Browsable(false)]
		public new BlobBaseEdit OwnerEdit { get { return base.OwnerEdit as BlobBaseEdit; } }
		public override void ShowPopupForm() {
			OkButton.Enabled = false;
			base.ShowPopupForm();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Size ContentSize { get { return ViewInfo.ContentRect.Size; } }
	}
}
