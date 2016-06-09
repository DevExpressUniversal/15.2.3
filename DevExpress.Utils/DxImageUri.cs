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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Design;
namespace DevExpress.Utils {
	interface IDxImageUriProvider {
		DxImageUri ImageUri { get; }
	}
	public interface IDXImageUriClient {
		bool SupportsLookAndFeel { get; }
		bool SupportsGlyphSkinning { get; }
		UserLookAndFeel LookAndFeel { get; }
		void SetGlyphSkinningValue(bool value);
		bool IsDesignMode { get; }
	}
	[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
	public class ImageUriPreferencesAttribute : Attribute {
		ImageSize preferredImageSize;
		public ImageUriPreferencesAttribute(ImageSize preferredImageSize) {
			this.preferredImageSize = preferredImageSize;
		}
		public ImageSize PreferredImageSize { get { return preferredImageSize; } }
	}
	[Editor("DevExpress.Utils.Design.DxImageUriEditor, " + AssemblyInfo.SRAssemblyDesignFull, typeof(UITypeEditor))]
	public class DxImageUri : ICloneable, IDisposable {
		string uri;
		DxImageUriParser parser;
		DxImageUriData uriData;
		IDXImageUriClient client;
		public DxImageUri() : this(string.Empty) { }
		public DxImageUri(string uri)
			: this(uri, new DxImageUriParser(uri)) {
		}
		public DxImageUri(string uri, DxImageUriParser parser) {
			this.uri = uri;
			this.parser = parser;
			this.uriData = new DxImageUriData();
		}
		[DefaultValue("")]
		public string Uri {
			get { return uri; }
			set {
				if(Uri == value) return;
				uri = value;
				OnChanged();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool IsInitialized {
			get {
				if(string.IsNullOrEmpty(Uri)) return false;
				return true;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void SetClient(IDXImageUriClient newClient) {
			if(object.ReferenceEquals(Client, newClient)) return;
			UnsubscribeClientEvents();
			this.client = newClient;
			SubscribeClientEvents();
			OnChanged();
		}
		protected void UnsubscribeClientEvents() {
			UserLookAndFeel laf = GetLookAndFeelObj();
			if(laf != null) {
				laf.StyleChanged -= OnClientStyleChanged;
			}
		}
		protected void SubscribeClientEvents() {
			UserLookAndFeel laf = GetLookAndFeelObj();
			if(laf != null) {
				laf.StyleChanged += OnClientStyleChanged;
			}
		}
		protected virtual void CheckItemAppearance() {
			UserLookAndFeel laf = GetLookAndFeelObj();
			if(laf == null) return;
			Parser.SetDesiredAppearance(laf);
			if(AllowSetGlyphSkinningValue()) SetClientGlyphSkinning(laf);
		}
		protected bool AllowSetGlyphSkinningValue() {
			if(Client == null || Client.IsDesignMode || !Client.SupportsGlyphSkinning) return false;
			return IsInitialized;
		}
		protected UserLookAndFeel GetLookAndFeelObj() {
			if(Client == null || !Client.SupportsLookAndFeel) return null;
			return Client.LookAndFeel;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public IDXImageUriClient Client { get { return client; } }
		public ImageSize GetPreferredImageSize() {
			if(Client == null) return ImageSize.Any;
			ImageUriPreferencesAttribute attr = (ImageUriPreferencesAttribute)Attribute.GetCustomAttribute(Client.GetType(), typeof(ImageUriPreferencesAttribute));
			return attr != null ? attr.PreferredImageSize : ImageSize.Any;
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool IsMatch(string val) {
			return Uri.Equals(val);
		}
		protected DxImageUriParser Parser { get { return parser; } }
		protected void OnClientStyleChanged(object sender, EventArgs e) {
			UserLookAndFeel laf = (UserLookAndFeel)sender;
			if(this.isDisposed) {
				laf.StyleChanged -= OnClientStyleChanged;
				return;
			}
			if(IsInitialized) OnChanged();
		}
		protected virtual void SetClientGlyphSkinning(UserLookAndFeel lookAndFeel) {
			SkinAppearanceType appearanceType = DxImageUriUtils.GetAppearanceType(lookAndFeel);
			Client.SetGlyphSkinningValue(GetGlyphSkinningValue(appearanceType));
		}
		protected virtual bool GetGlyphSkinningValue(SkinAppearanceType appearanceType) {
			return appearanceType == SkinAppearanceType.Gray;
		}
		public virtual Image GetLargeImage() {
			if(ImageType == Design.ImageType.Svg)
				return GetSvgImage(new Size(32, 32), null);
			if(HasDefaultImage && ImageSize != Design.ImageSize.Size16x16)
				return uriData.DefaultImage;
			if(!HasLargeImage) return null;
			return uriData.LargeImage;
		}
		public virtual Image GetImage() {
			if(ImageType == Design.ImageType.Svg)
				return GetSvgImage(new Size(16, 16), null);
			if(HasDefaultImage && ImageSize != Design.ImageSize.Size32x32)
				return uriData.DefaultImage;
			if(!HasImage) return null;
			return uriData.Image;
		}
		[Browsable(false)]
		public virtual bool HasImage {
			get { return (this.uriData.HasImage || HasSvgImage) && ImageSize != Design.ImageSize.Size32x32; }
		}
		public virtual Image GetSvgImage(Size imageSize, ISvgPaletteProvider paletteProvider) {
			return DxImageAssemblyUtil.ImageProvider.GetSvgImage(uriData.ImageId, paletteProvider, imageSize.Width, imageSize.Height);
		}
		[Browsable(false)]
		public virtual bool HasLargeImage {
			get { return (this.uriData.HasLargeImage || HasSvgImage) && ImageSize != Design.ImageSize.Size16x16; }
		}
		[Browsable(false)]
		public virtual string ImageId {
			get { return this.uriData.ImageId; }
		}
		[Browsable(false)]
		public virtual ImageSize ImageSize {
			get { return this.uriData.ImageSize; }
		}
		[Browsable(false)]
		public virtual ImageType ImageType {
			get { return this.uriData.ImageType; }
		}
		[Browsable(false)]
		public virtual bool HasSvgImage {
			get { return this.uriData.HasSvgImage; }
		}
		public virtual Image GetDefaultImage() {
			return this.uriData.DefaultImage;
		}
		[Browsable(false)]
		public virtual bool HasDefaultImage {
			get { return this.uriData.HasDefaultImage; }
		}
		protected virtual void OnChanged() {
			Debug.Assert(Parser != null);
			CheckItemAppearance();
			Parser.SetUri(Uri);
			this.uriData.Assign(Parser.DoParse());
			RaiseChanged();
		}
		protected virtual void RaiseChanged() {
			if(Changed != null) Changed(this, EventArgs.Empty);
		}
		public event EventHandler Changed;
		public static implicit operator DxImageUri(string uri) {
			return new DxImageUri(uri);
		}
		public static implicit operator string(DxImageUri uri) {
			return uri.Uri;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool ShouldSerialize() {
			if(string.IsNullOrEmpty(Uri)) return false;
			return true;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void Reset() {
			Uri = string.Empty;
		}
		protected virtual void Assign(DxImageUri other) {
			this.uri = other.uri;
			this.parser = other.parser;
			this.uriData = other.uriData.Clone();
			this.client = other.client;
		}
		public override bool Equals(object obj) {
			return (obj is DxImageUri) && Equals((DxImageUri)obj);
		}
		protected virtual bool Equals(DxImageUri other) {
			if(Uri == null) return other.Uri == null;
			return Uri.Equals(other.Uri);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override string ToString() {
			return OptionsHelper.GetObjectText(this);
		}
		#region ICloneable
		object ICloneable.Clone() {
			return Clone();
		}
		public DxImageUri Clone() {
			DxImageUri result = new DxImageUri();
			result.Assign(this);
			return result;
		}
		#endregion
		#region Disposing
		public void Dispose() {
			Dispose(true);
		}
		bool isDisposed = false;
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				UnsubscribeClientEvents();
				if(this.uriData != null) {
					this.uriData.Dispose();
				}
			}
			this.uri = null;
			this.client = null;
			this.parser = null;
			this.uriData = null;
			this.isDisposed = true;
		}
		#endregion
	}
	public class DxImageUriData : ICloneable, IDisposable {
		public DxImageUriData()
			: this(null, null) {
		}
		public DxImageUriData(Image smallImage, Image largeImage) {
			ImageId = string.Empty;
			ImageSize = ImageSize.Any;
			Image = smallImage;
			LargeImage = largeImage;
			ImageType = ImageType.Default;
		}
		public string ImageId { get; set; }
		public ImageSize ImageSize { get; set; }
		public ImageType ImageType { get; set; }
		public Size SvgImageSize { get; set; }
		public Image Image { get; set; }
		public bool HasImage {
			get { return Image != null; }
		}
		public Image LargeImage { get; set; }
		public bool HasLargeImage {
			get { return LargeImage != null; }
		}
		public Image DefaultImage { get; set; }
		public bool HasDefaultImage {
			get { return DefaultImage != null; }
		}
		public bool HasSvgImage { get; set; }
		protected internal virtual void Assign(DxImageUriData other) {
			ImageId = other.ImageId;
			ImageSize = other.ImageSize;
			ImageType = other.ImageType;
			SvgImageSize = other.SvgImageSize;
			if(other.Image != null) {
				this.Image = (Image)other.Image.Clone();
			}
			if(other.DefaultImage != null) {
				this.DefaultImage = (Image)other.DefaultImage.Clone();
			}
			if(other.LargeImage != null) {
				this.LargeImage = (Image)other.LargeImage.Clone();
			}
			HasSvgImage = other.HasSvgImage;
		}
		#region ICloneable
		object ICloneable.Clone() {
			return Clone();
		}
		public DxImageUriData Clone() {
			DxImageUriData result = new DxImageUriData();
			result.Assign(this);
			return result;
		}
		#endregion
		#region Disposing
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				if(Image != null) {
					Image.Dispose();
				}
				if(LargeImage != null) {
					LargeImage.Dispose();
				}
			}
			Image = null;
			LargeImage = null;
		}
		#endregion
	}
	public class DxImageUriParser {
		static DxImageUriData NullResult = new DxImageUriData();
		string uri;
		SkinAppearanceType appearance;
		public DxImageUriParser(string uri) {
			this.uri = uri;
			this.appearance = SkinAppearanceType.Default;
		}
		public DxImageUriData DoParse() {
			if(string.IsNullOrEmpty(this.uri)) return NullResult;
			DxImageUriData result = ParseUri();
			ImageType imageType = GetImageType();
			if(result.ImageType != ImageType.Default && result.ImageSize == ImageSize.Any) {
				imageType = result.ImageType;
			}
			result.Image = GetImage(result.ImageId, ImageSize.Size16x16, imageType);
			result.LargeImage = GetImage(result.ImageId, ImageSize.Size32x32, imageType);
			if(result.ImageType != ImageType.Default && (result.ImageSize != ImageSize.Any))
				result.DefaultImage = GetImage(result.ImageId, result.ImageSize, result.ImageType);
			if(result.ImageType == ImageType.Svg)
				result.HasSvgImage = GetImage(result.ImageId, ImageSize.Size32x32, ImageType.Svg) != null;
			return result;
		}
		DxImageUriData ParseUri() {
			DxImageUriData result = new DxImageUriData();
			var parts = GetUriParts();
			result.ImageId = parts[0];
			ImageSize imageSize = ImageSize.Any;
			ImageType imageType = ImageType.Default;
			for(int i = 1; i < parts.Length; i++) {
				if(Enum.TryParse<ImageSize>(parts[i], true, out imageSize))
					result.ImageSize = imageSize;
				if(Enum.TryParse<ImageType>(parts[i], true, out imageType)) {
					result.ImageType = imageType;
					if(imageType == ImageType.Svg) {
						result.SvgImageSize =  GetSvgImageSize(parts);
					}
				}
			}
			return result;
		}
		protected virtual  Size GetSvgImageSize(string[] parts) {
			string sizeString = parts.FirstOrDefault(x => x.Contains("Size"));
			if(!string.IsNullOrEmpty(sizeString)) {
				sizeString = sizeString.Replace("Size", string.Empty);
				var numbers = sizeString.Split('x');
				if(numbers.Count() == 2) {
					int width = int.Parse(numbers[0]);
					int height = int.Parse(numbers[1]);
					return new Size(width, height);
				}
			}
			return Size.Empty;
		}
		public void SetUri(string uri) {
			this.uri = uri;
		}
		public void SetDesiredAppearance(UserLookAndFeel lookAndFeel) {
			this.appearance = DxImageUriUtils.GetAppearanceType(lookAndFeel);
		}
		protected ImageType GetImageType() {
			return DxImageUriUtils.GetImageType(this.appearance);
		}
		protected string[] GetUriParts() { return this.uri.Split(';', ','); }
		#region Format URI
		public static string FormatUri(string imageId, ImageSize imageSize, ImageType imageType = ImageType.Default, Size svgImageSize = new Size()) {
			StringBuilder builder = new StringBuilder();
			builder.Append(imageId);
			if(imageSize != ImageSize.Any) {
				builder.Append(";");
				builder.AppendFormat(imageSize.ToString());
			}
			if(imageType != ImageType.Default) {
				if(imageType == ImageType.Svg && svgImageSize != Size.Empty) {
					builder.Append(";");
					builder.AppendFormat(string.Format("Size{0}x{1}", svgImageSize.Width, svgImageSize.Height));
				}
				builder.Append(";");
				builder.AppendFormat(imageType.ToString());
			}
			return builder.ToString();
		}
		#endregion
		protected Image GetImage(string imageId, ImageSize size, ImageType type) {
			return DxImageAssemblyUtil.ImageProvider.GetImage(imageId, size, type);
		}
	}
	public static class DxImageUriUtils {
		public static SkinAppearanceType GetAppearanceType(UserLookAndFeel lookAndFeel) {
			return SkinCollectionHelper.GetSkinAppearanceType(lookAndFeel.ActiveSkinName);
		}
		public static ImageType GetImageType(UserLookAndFeel lookAndFeel) {
			SkinAppearanceType appearanceType = GetAppearanceType(lookAndFeel);
			return GetImageType(appearanceType);
		}
		public static ImageType GetImageType(SkinAppearanceType appearanceType) {
			ImageType imageType = ImageType.Colored;
			switch(appearanceType) {
				case SkinAppearanceType.Default:
					imageType = ImageType.Colored;
					break;
				case SkinAppearanceType.Office2013:
					imageType = ImageType.Office2013;
					break;
				case SkinAppearanceType.Gray:
					imageType = ImageType.GrayScaled;
					break;
			}
			return imageType;
		}
	}
	public class DxImageAssemblyUtil {
		Assembly imageAsm;
		IDXImagesProvider imageProvider;
		protected DxImageAssemblyUtil() {
			this.imageAsm = LoadImageAssembly();
			this.imageProvider = LoadImageProvider(this.imageAsm);
		}
		protected virtual Assembly LoadImageAssembly() {
			return Assembly.Load(AssemblyInfo.SRAssemblyImagesFull);
		}
		protected virtual IDXImagesProvider LoadImageProvider(Assembly imageAsm) {
			IDXImagesProvider provider = AssemblyServiceClassAttribute.CreateDXImagesProvider(imageAsm);
			if(provider == null) {
				throw new CannotGetDxImageProviderException();
			}
			return provider;
		}
		static DxImageAssemblyUtil instance = null;
		static DxImageAssemblyUtil Instance {
			get {
				if(instance == null) instance = new DxImageAssemblyUtil();
				return instance;
			}
		}
		public static Assembly ImageAssembly {
			get { return Instance.imageAsm; }
		}
		public static IDXImagesProviderEx ImageProvider {
			get { return Instance.imageProvider as IDXImagesProviderEx; }
		}
	}
	public class CannotGetDxImageProviderException : Exception {
		public CannotGetDxImageProviderException()
			: base() {
		}
		public CannotGetDxImageProviderException(string message)
			: base(message) {
		}
	}
}
