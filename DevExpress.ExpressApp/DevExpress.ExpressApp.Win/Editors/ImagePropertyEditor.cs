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
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.Persistent.Base;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.ExpressApp.Win.Editors {
	public class DoubleMouseClickProcessor {
		private PictureEdit pictureEdit;
		private void pictureEdit_DoubleClick(object sender, EventArgs e) {
			if(!pictureEdit.Properties.ReadOnly) {
				pictureEdit.LoadImage();
			}
		}
		public DoubleMouseClickProcessor(PictureEdit pictureEdit) {
			this.pictureEdit = pictureEdit;
			pictureEdit.DoubleClick += new EventHandler(pictureEdit_DoubleClick);
		}
		public void Dispose() {
			pictureEdit.DoubleClick -= new EventHandler(pictureEdit_DoubleClick);
			pictureEdit = null;
		}
	}
	[ToolboxItem(false)]
	public class XafPictureEdit : PictureEdit {
		private DoubleMouseClickProcessor clickProcessor;
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(clickProcessor != null) {
					clickProcessor.Dispose();
					clickProcessor = null;
				}
			}
			base.Dispose(disposing);
		}
		public XafPictureEdit()
			: base() {
			clickProcessor = new DoubleMouseClickProcessor(this);
		}
	}
	[ToolboxItem(false)]
	public class XafImageEdit : ImageEdit {
		private DoubleMouseClickProcessor clickProcessor;
		private void pictureEdit_Disposed(object sender, EventArgs e) {
			((PictureEdit)sender).Disposed -= new EventHandler(pictureEdit_Disposed);
			if(clickProcessor != null) {
				clickProcessor.Dispose();
				clickProcessor = null;
			}
		}
		protected override void DoShowPopup() {
			base.DoShowPopup();
			if(clickProcessor == null) {
				PictureEdit pictureEdit = PopupForm.Controls[2] as PictureEdit;
				if(pictureEdit != null) {
					clickProcessor = new DoubleMouseClickProcessor(pictureEdit);
					pictureEdit.Disposed += new EventHandler(pictureEdit_Disposed);
				}
			}
		}
		public XafImageEdit() : base() { }
	}
	public class ImagePropertyEditor : DXPropertyEditor {
		protected override Core.BindingHelper CreateBindingHelper() {
			if(mediaDataAttribute != null) {
				IMemberInfo mediaObjData = ObjectTypeInfo.FindMember(propertyName + "." + mediaDataAttribute.MediaDataBindingProperty);
				return new BindingHelper(Control, ControlBindingProperty, mediaObjData, ImmediatePostData);
			}
			else {
				return base.CreateBindingHelper();
			}
		}
		public static int BorderWidthInFixedSize = 2;
		protected override object CreateControlCore() {
			ImageEditorMode detailViewMode = Model.ImageEditorMode; 
			switch(detailViewMode) {
				case ImageEditorMode.DropDownPictureEdit:
				case ImageEditorMode.PopupPictureEdit: {
						return new XafImageEdit();
					}
				case ImageEditorMode.PictureEdit: {
						XafPictureEdit result = new XafPictureEdit();
						Size fixedSize = new Size(0, 0);
						int fixedHeight = Model.ImageEditorFixedHeight; 
						if(fixedHeight > 0) {
							fixedSize = new Size(fixedSize.Width, fixedHeight + 2 * BorderWidthInFixedSize);
						}
						int fixedWidth = Model.ImageEditorFixedWidth; 
						if(fixedWidth > 0) {
							fixedSize = new Size(fixedWidth + 2 * BorderWidthInFixedSize, fixedSize.Height);
						}
						result.MaximumSize = fixedSize;
						result.MinimumSize = fixedSize;
						return result;
					}
				default:
					throw new ArgumentException(detailViewMode.ToString(), "detailViewMode");
			}
		}
		protected override RepositoryItem CreateRepositoryItem() {
			ImageEditorMode listViewMode = Model.ImageEditorMode; 
			switch(listViewMode) {
				case ImageEditorMode.DropDownPictureEdit:
				case ImageEditorMode.PopupPictureEdit: {
						return new RepositoryItemImageEdit();
					}
				case ImageEditorMode.PictureEdit: {
						RepositoryItemPictureEdit result = new RepositoryItemPictureEdit();
						return result;
					}
				default:
					throw new ArgumentException(listViewMode.ToString(), "listViewMode");
			}
		}
		MediaDataObjectAttribute mediaDataAttribute = null;
		protected override void SetupRepositoryItem(RepositoryItem item) {
			mediaDataAttribute = MemberInfo.MemberTypeInfo.FindAttribute<MediaDataObjectAttribute>(false);
			if(MemberInfo.MemberType != typeof(byte[]) && mediaDataAttribute == null && !typeof(System.Drawing.Image).IsAssignableFrom(MemberInfo.MemberType)) {
				throw new InvalidOperationException("Incorrect image format.");
			}
			base.SetupRepositoryItem(item);
			PictureSizeMode pictureSizeMode = Model.ImageSizeMode == ImageSizeMode.Zoom ? PictureSizeMode.Zoom : PictureSizeMode.Clip;
			PictureStoreMode pictureStoreMode = MemberInfo.MemberType == typeof(byte[]) || mediaDataAttribute != null ? PictureStoreMode.ByteArray : PictureStoreMode.Image;
			if(item is RepositoryItemImageEdit) {
				RepositoryItemImageEdit repositoryItemImageEdit = (RepositoryItemImageEdit)item;
				repositoryItemImageEdit.SizeMode = pictureSizeMode;
				repositoryItemImageEdit.PictureStoreMode = pictureStoreMode;
			}
			else if(item is RepositoryItemPictureEdit) {
				RepositoryItemPictureEdit repositoryItemPictureEdit = (RepositoryItemPictureEdit)item;
				repositoryItemPictureEdit.SizeMode = pictureSizeMode;
				repositoryItemPictureEdit.CustomHeight = Model.ImageEditorCustomHeight;
				repositoryItemPictureEdit.PictureStoreMode = pictureStoreMode;
			}
		}
		public ImagePropertyEditor(Type objectType, IModelMemberViewItem model)
			: base(objectType, model) {
			ControlBindingProperty = "EditValue";
		}
	}
}
