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
using System.IO;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.DC;
using System.Collections;
using DevExpress.Data.Filtering;
using System.Drawing;
using System.Collections.Specialized;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	public enum ImagePropertyEditorStorageMode { Image, ByteArray, MediaData }
	public class ASPxImagePropertyEditor : WebPropertyEditor, IComplexViewItem, ITestableEx {
		protected override void WriteValueCore() {
			object propertyValue = PropertyValue;
			if(storageMode == ImagePropertyEditorStorageMode.MediaData) {
				IMemberInfo bindingMemberInfo = MemberInfo.MemberTypeInfo.FindMember(mediaDataAttribute.MediaDataBindingProperty);
				if(bindingMemberInfo != null) {
					bindingMemberInfo.SetValue(propertyValue, ControlValue);
				}
			}
			else {
				base.WriteValueCore();
			}
		}
		internal const string LocalizationGroupName = "ASPxImagePropertyEditor";	
		private static List<ImageFormat> imagePossibleFormats = new List<ImageFormat>();
		static ASPxImagePropertyEditor() {
			imagePossibleFormats.Add(ImageFormat.Gif);
			imagePossibleFormats.Add(ImageFormat.Jpeg);
			imagePossibleFormats.Add(ImageFormat.Png);
		}
		private IObjectSpace objectSpace;
		private ImagePropertyEditorStorageMode storageMode;
		private byte[] uploadedImage;
		private void PictureHolder_ImageChanged(object sender, ImageChangedEventArgs e) {
			if(storageMode == ImagePropertyEditorStorageMode.ByteArray || storageMode == ImagePropertyEditorStorageMode.MediaData) {
				uploadedImage = e.ImageBytes;
			}
			if(!inReadValue) {
				EditValueChangedHandler(sender, e);
				if(!ImmediatePostData) {
					SetPictureHolderValue(PictureHolder);
				}
			}
		}
		private int GetCustomImageHeight() {
			int fixedHeight = 0;
			if(Model is IModelColumn) {
				fixedHeight = Model.ImageEditorCustomHeight; 
			}
			if(Model is IModelViewItem) {
				fixedHeight = Model.ImageEditorFixedHeight; 
			}
			return fixedHeight;
		}
		private int GetCustomImageWidth() {
			return Model.ImageEditorFixedWidth; 
		}
		MediaDataObjectAttribute mediaDataAttribute = null;
		private WebControl CreateImageControl(bool readOnly) {
			mediaDataAttribute = MemberInfo.MemberTypeInfo.FindAttribute<MediaDataObjectAttribute>(false);
			if(MemberInfo.MemberType != typeof(byte[]) && mediaDataAttribute == null && !typeof(System.Drawing.Image).IsAssignableFrom(MemberInfo.MemberType)) {
				throw new InvalidOperationException("Incorrect image format.");
			}
			storageMode = mediaDataAttribute != null ? ImagePropertyEditorStorageMode.MediaData : MemberInfo.MemberType == typeof(byte[]) ? ImagePropertyEditorStorageMode.ByteArray : ImagePropertyEditorStorageMode.Image;
			ImageEditorMode viewMode = Model.ImageEditorMode;
			CancelClickEventPropagation = Model is IModelColumn && viewMode != ImageEditorMode.PictureEdit;
			IPictureHolder pictureHolder;
			switch(viewMode) {
				case ImageEditorMode.PictureEdit:
					ImageEdit imageEdit = new ImageEdit(readOnly, storageMode, ImmediatePostData);
					imageEdit.CustomImageHeight = GetCustomImageHeight();
					imageEdit.CustomImageWidth = GetCustomImageWidth();
					imageEdit.ImageSizeMode = Model.ImageSizeMode;
					pictureHolder = imageEdit;
					break;
				case ImageEditorMode.DropDownPictureEdit:
					pictureHolder = new DropDownImageEdit(readOnly, false, storageMode, ImmediatePostData);
					break;
				case ImageEditorMode.PopupPictureEdit:
					pictureHolder = new DropDownImageEdit(readOnly, true, storageMode, ImmediatePostData);
					break;
				default:
					throw new ArgumentException(viewMode.ToString(), "viewMode");
			}
			pictureHolder.ImageChanged += new EventHandler<ImageChangedEventArgs>(PictureHolder_ImageChanged);
			return (WebControl)pictureHolder;
		}
		private void SetPictureHolderValue(IPictureHolder pictureHolder) {
			bool visibleOnRender = false;
			if(storageMode == ImagePropertyEditorStorageMode.MediaData || storageMode == ImagePropertyEditorStorageMode.ByteArray) {
				string imageUrl = ImageProcessorsHelper.GetImageUrl(objectSpace, CurrentObject, PropertyName, MemberInfo);
				if(ImageProcessorsHelper.IsMemberModified(CurrentObject, PropertyName, objectSpace) && !MemberInfo.IsDelayed) {
					visibleOnRender = PropertyValue != null && Model.ImageEditorMode == ImageEditorMode.PictureEdit;
				}
				pictureHolder.SetControlImageUrl(imageUrl, visibleOnRender);
			}
			else {
				pictureHolder.Image = (System.Drawing.Image)PropertyValue;
			}
		}
		private void ClearCache() {
			uploadedImage = null;
		}
		private void objectSpace_Refreshing(object sender, CancelEventArgs e) {
			if(storageMode == ImagePropertyEditorStorageMode.MediaData || storageMode == ImagePropertyEditorStorageMode.ByteArray) {
				ClearCache();
			}
		}
		private IPictureHolder PictureHolder {
			get {
				return ((IPictureHolder)Editor);
			}
		}
		protected override WebControl CreateEditModeControlCore() {
			return CreateImageControl(false);
		}
		protected override WebControl CreateViewModeControlCore() {
			return CreateImageControl(true);
		}
		protected override object GetControlValueCore() {
			object result;
			if(storageMode == ImagePropertyEditorStorageMode.MediaData || storageMode == ImagePropertyEditorStorageMode.ByteArray) {
				result = uploadedImage;
			}
			else {
				result = PictureHolder.Image;
			}
			return result;
		}
		protected override void ReadEditModeValueCore() {
			SetPictureHolderValue(PictureHolder);
		}
		protected override void ReadViewModeValueCore() {
			SetPictureHolderValue((IPictureHolder)InplaceViewModeEditor);
		}
		protected override void Dispose(bool disposing) {
			if(storageMode == ImagePropertyEditorStorageMode.MediaData || storageMode == ImagePropertyEditorStorageMode.ByteArray) {
				ClearCache();
			}
			objectSpace.Refreshing -= new EventHandler<CancelEventArgs>(objectSpace_Refreshing);
			base.Dispose(disposing);
		}
		public override void BreakLinksToControl(bool unwireEventsOnly) {
			if(Editor != null) {
				PictureHolder.ImageChanged -= new EventHandler<ImageChangedEventArgs>(PictureHolder_ImageChanged);
				Editor.Dispose();
			}
			base.BreakLinksToControl(unwireEventsOnly);
		}
		public override bool SupportInlineEdit {
			get {
				return false;
			}
		}
		public ASPxImagePropertyEditor(Type objectType, IModelMemberViewItem model)
			: base(objectType, model) {
			skipEditModeDataBind = true;
		}
		public void Setup(IObjectSpace objectSpace, XafApplication application) {
			if(objectSpace != null) {
				objectSpace.Refreshing += new EventHandler<CancelEventArgs>(objectSpace_Refreshing);
			}
			this.objectSpace = objectSpace;
		}
		#region ITestableEx Members
		public Type RegisterControlType {
			get { return this.GetType(); }
		}
		#endregion
#if DebugTest
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public const string DebugTest_LocalizationGroupName = LocalizationGroupName;
		public ImagePropertyEditorStorageMode StorageMode { get { return storageMode; } }
#endif
	}
	public interface IPictureHolder {
		[Bindable(true)]
		void SetControlImageUrl(string imageId, bool visibleOnRender);
		System.Drawing.Image Image { get; set; }
		event EventHandler<ImageChangedEventArgs> ImageChanged;
	}
}
