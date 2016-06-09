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

extern alias Platform;
using DevExpress.Design.SmartTags;
using DevExpress.Images;
using DevExpress.Mvvm.Native;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Extensions.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Guard = Platform::DevExpress.Utils.Guard;
using DevExpress.Mvvm;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
namespace DevExpress.Xpf.Core.Design.SmartTags {
	public class ImageSourcePropertyLineViewModel : PropertyLineWithPopupEditorViewModel {
		public ImageSourcePropertyLineViewModel(IPropertyLineContext context, string propertyName, IPropertyLinePlatformInfo platformInfo = null) : base(context, propertyName, null, platformInfo) { }
		protected override PropertyLineWithPopupEditorPopupViewModel CreatePopup() {
			return new ImageSourcePropertyLinePopupViewModel(this);
		}
		public override void ClearValue() {
			base.ClearValue();
			(this.PopupContent as ImageSourcePropertyLinePopupViewModel).SelectedImageOriginalString = null;
		}
	}
	public class ImageSourcePropertyLinePopupViewModel : PropertyLineWithPopupEditorPopupViewModel {
		readonly ImageSourcePropertyLinePopupViewModelDataProvider dataProviderCore;
		IDesignTimeImagePickerDataProvier dataProvider;
		IDesignTimeImagePickerImageInfo selectedImage;
		string propertyName;
		string selectedImageOriginalString;
		public ImageSourcePropertyLinePopupViewModel(ImageSourcePropertyLineViewModel propertyLine)
			: base(propertyLine) {
			dataProviderCore = new ImageSourcePropertyLinePopupViewModelDataProvider(GetImages);
			PropertyName = propertyLine.PropertyName;
		}
		string GetOriginalString(object propertyValue) {
			if(propertyValue == null) return null;
			if(propertyValue is DXImageExtension)
				return ((DXImageExtension)propertyValue).Image.MakeUri().OriginalString;
			if(propertyValue is String)
				return (String)propertyValue;
			if(propertyValue is BitmapImage)
				return ((BitmapImage)propertyValue).UriSource.OriginalString;
			return null;
		}
		public IEnumerable<IDesignTimeImagePickerGroupInfo> Groups {
			get {
				yield return new DesignTimeImagePickerGroupInfo("(Local)");
				foreach(string group in ImagesAssemblyImageList.Groups.OrderBy(g => g)) {
					yield return new DesignTimeImagePickerGroupInfo(group);
				}
			}
		}
		public string SelectedImageOriginalString {
			get { return selectedImageOriginalString; }
			set { SetProperty(ref selectedImageOriginalString, value, () => SelectedImageOriginalString); }
		}
		public IDesignTimeImagePickerDataProvier DataProvider {
			get { return dataProvider; }
			set { SetProperty(ref dataProvider, value, () => DataProvider); }
		}
		public IDesignTimeImagePickerImageInfo SelectedImage {
			get { return selectedImage; }
			set { SetProperty(ref selectedImage, value, () => SelectedImage, OnSelectedImageChanged); }
		}
		public string PropertyName {
			get { return propertyName; }
			set { SetProperty(ref propertyName, value, () => PropertyName); }
		}
		void OnSelectedImageChanged() {
			IDesignTimeImagePickerImageInfo image = SelectedImage;
			if(image == null) return;
			string value = image.Uri.OriginalString;
			SelectedImageOriginalString = value;
			if(image is DesignTimeImagePickerImageInfo) {
				IDXImageInfo imageInfo = ((DesignTimeImagePickerImageInfo)image).Info;
				try {
					DXImageExtension imageExtension = ResourceImageExtension.GetDXImageExtension(imageInfo.ImageType);
					imageExtension.Image = new DXImageInfo(imageInfo);
					PropertyLine.PropertyValue = imageExtension;
				} catch(NotImplementedException) {
					PropertyLine.PropertyValue = image.Uri.OriginalString;
				}
				ProjectReferencesHelper.EnsureAssemblyReferenced(PropertyLine.SelectedItem, AssemblyInfo.SRAssemblyImagesFull, true);
			} else {
				if(PropertyLine.PropertyValueText != value)
					PropertyLine.PropertyValueText = value;
			}
		}
		protected override void OnPropertyLineIsPopupOpenChanged(object sender, EventArgs e) {
			base.OnPropertyLineIsPopupOpenChanged(sender, e);
			if(PropertyLine.IsPopupOpen) {
				SelectedImageOriginalString = GetOriginalString(PropertyLine.PropertyValue);
				DataProvider = dataProviderCore;
			} else {
				DataProvider = null;
				dataProviderCore.Reset();
			}
		}
		static readonly IDXImagesProvider imagesProvider = new DXImageServicesImp();
		IEnumerable<IDesignTimeImagePickerImageInfo> GetImages() {
			foreach(Uri uri in ImageResourceHelper.GetImageUris(PropertyLine.Context.ModelItem.Context).OrderBy(u => u.OriginalString)) {
				Uri imageUri = uri.OriginalString.StartsWith("pack:") ? uri : new Uri(string.Format("pack://application:,,,{0}", uri.OriginalString));
				yield return new DesignTimeImagePickerExternalImageInfo(imageUri);
			}
			foreach(ImagesAssemblyImageInfo image in Native.DXImageConverter.GetDistinctImages(imagesProvider)) {
				yield return new DesignTimeImagePickerImageInfo(image);
			}
		}
	}
	public class DesignTimeImagePickerExternalImageInfo : IDesignTimeImagePickerImageInfo {
		readonly Uri uri;
		public DesignTimeImagePickerExternalImageInfo(Uri uri) {
			this.uri = uri;
		}
		public string Group { get { return "(Local)"; } }
		public string Name {
			get {
				try {
					return uri.OriginalString.Split('/').LastOrDefault().Return(s => s, () => string.Empty);
				} catch {
					return string.Empty;
				}
			}
		}
		public Uri Uri { get { return uri; } }
		public ImageType? ImageType { get { return null; } }
		public ImageSize Size { get { return ImageSize.Any; } }
		public IEnumerable<string> Tags { get { return new string[] { }; } }
	}
	public class DesignTimeImagePickerImageInfo : IDesignTimeImagePickerImageInfo {
		readonly ImagesAssemblyImageInfo info;
		Uri uri;
		public DesignTimeImagePickerImageInfo(ImagesAssemblyImageInfo info) {
			this.info = info;
		}
		internal IDXImageInfo Info { get { return info; } }
		public string Group { get { return info.Group; } }
		public string Name { get { return info.Name; } }
		public Uri Uri {
			get {
				if(uri == null) {
					object folder = ImageCollectionHelper.GetImageFolderName(info.ImageType);
					uri = AssemblyHelper.GetResourceUri(typeof(ImagesAssemblyImageList).Assembly, string.Format("{0}/{1}/{2}", folder, Group, Name));
				}
				return uri;
			}
		}
		public ImageType? ImageType { get { return info.ImageType; } }
		public ImageSize Size { get { return info.Size; } }
		public IEnumerable<string> Tags { get { return info.Tags; } }
	}
	public class DesignTimeImagePickerGroupInfo : IDesignTimeImagePickerGroupInfo {
		public DesignTimeImagePickerGroupInfo(string name) {
			Name = name;
		}
		public string Name { get; private set; }
	}
	public class ImageSourcePropertyLinePopupViewModelDataProvider : BindableBase, IDesignTimeImagePickerDataProvier {
		readonly Func<IEnumerable<IDesignTimeImagePickerImageInfo>> loadImagesCallback;
		readonly Func<bool, Thread> updateAsyncMethod;
		bool loaded = false;
		IEnumerable<IDesignTimeImagePickerImageInfo> images;
		WeakEventHandler<EventArgs, EventHandler> imagesAsyncChanged;
		Dispatcher dispatcher;
		public ImageSourcePropertyLinePopupViewModelDataProvider(Func<IEnumerable<IDesignTimeImagePickerImageInfo>> loadImagesCallback) {
			dispatcher = Dispatcher.CurrentDispatcher;
			this.loadImagesCallback = loadImagesCallback;
			updateAsyncMethod = AsyncHelper.Create<bool>(UpdateAsync);
		}
		public IEnumerable<IDesignTimeImagePickerImageInfo> Images {
			get { return images; }
			private set {
				if(SetProperty(ref images, value, () => Images))
					AsyncHelper.DoWithDispatcher(dispatcher, () => imagesAsyncChanged.SafeRaise(this, EventArgs.Empty));
			}
		}
		public event EventHandler ImagesAsyncChanged { add { imagesAsyncChanged += value; } remove { imagesAsyncChanged -= value; } }
		public void Load() {
			if(loaded) return;
			loaded = true;
			updateAsyncMethod(false);
		}
		public void Reset() {
			if(!loaded) return;
			loaded = false;
			updateAsyncMethod(true);
		}
		IEnumerable<ManualResetEvent> UpdateAsync(bool reset, CancellationToken cancellationToken) {
			if(reset) {
				Images = null;
				return null;
			}
			if(cancellationToken.IsCancellationRequested) return null;
			Images = loadImagesCallback == null ? new IDesignTimeImagePickerImageInfo[] { } : loadImagesCallback();
			return null;
		}
		IEnumerable<IDesignTimeImagePickerImageInfo> IDesignTimeImagePickerDataProvier.Images { get { return Images; } }
		event EventHandler IDesignTimeImagePickerDataProvier.ImagesChanged { add { ImagesAsyncChanged += value; } remove { ImagesAsyncChanged -= value; } }
	}
}
