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
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office.Utils;
using DevExpress.Utils;
namespace DevExpress.Office.Model {
	public interface IUniqueImageId { }
	public class Crc32ImageId : IUniqueImageId {
		readonly int crc32;
		public Crc32ImageId(int crc32) {
			this.crc32 = crc32;
		}
		public override bool Equals(object obj) {
			Crc32ImageId other = obj as Crc32ImageId;
			if (object.ReferenceEquals(other, null))
				return false;
			return this.crc32 == other.crc32;
		}
		public override int GetHashCode() {
			return this.crc32;
		}
	}
	public class RtfImageId : IUniqueImageId {
		readonly int id;
		public RtfImageId(int id) {
			this.id = id;
		}
		public override bool Equals(object obj) {
			RtfImageId other = obj as RtfImageId;
			if (object.ReferenceEquals(other, null))
				return false;
			return this.id == other.id;
		}
		public override int GetHashCode() {
			return this.id;
		}
	}
	public class NativeImageId : IUniqueImageId {
		readonly int imageHashCode;
		readonly WeakReference imageRef;
		public NativeImageId(Image img) {
			Guard.ArgumentNotNull(img, "img");
			this.imageHashCode = img.GetHashCode();
			this.imageRef = new WeakReference(img);
		}
		public override int GetHashCode() {
			return this.imageHashCode;
		}
		public override bool Equals(object obj) {
			NativeImageId other = obj as NativeImageId;
			if (object.ReferenceEquals(obj, null))
				return false;
			return EqualsCore(this, other);
		}
		static bool EqualsCore(NativeImageId a, NativeImageId b) {
			if (Object.ReferenceEquals(a, b))
				return true;
			Image img1 = (Image)a.imageRef.Target;
			Image img2 = (Image)b.imageRef.Target;
			if (Object.ReferenceEquals(img1, null) && Object.ReferenceEquals(img2, null))
				return false;
			return object.ReferenceEquals(img1, img2);
		}
	}
	#region ImageCache
	public abstract class ImageCacheBase {
		readonly IDocumentModel owner;
		int nextImageId = 1;
		protected ImageCacheBase(IDocumentModel owner) {
			Guard.ArgumentNotNull(owner, "owner");
			this.owner = owner;
		}
		protected abstract void AddImage(OfficeNativeImage image, int key);
		protected abstract OfficeNativeImage GetNativeImageByKey(int key);
		protected abstract OfficeNativeImage GetNativeImageById(IUniqueImageId id);
		public void RegisterImage(OfficeNativeImage image) {
			if (image.ImageCacheKey > 0)
				return;
			OfficeImage cachedImage = GetNativeImageById(image.Id); ;
			if (cachedImage != null) {
				image.SetImageCacheKey(cachedImage.ImageCacheKey);
				return;
			}
			image.SetImageCacheKey(nextImageId);
			AddImage(image, nextImageId);
			nextImageId++;
		}
		public OfficeReferenceImage GetImageById(IUniqueImageId id) {
			OfficeNativeImage img = GetNativeImageById(id);
			if (img != null)
				return new OfficeReferenceImage(owner, img);
			return null;
		}
		public OfficeReferenceImage GetImageByKey(int imageId) {
			OfficeNativeImage image = GetNativeImageByKey(imageId);
			if (image == null)
				return null;
			return new OfficeReferenceImage(owner, image);
		}
		public OfficeReferenceImage AddImage(OfficeNativeImage image) {
			RegisterImage(image);			
			return new OfficeReferenceImage(owner, image);
		}
		public virtual void Clear() {		   
			nextImageId = 1;
		}
	}
	public class ImageCache : ImageCacheBase {
		readonly Dictionary<IUniqueImageId, WeakReference> imagesByIds = new Dictionary<IUniqueImageId, WeakReference>();
		readonly Dictionary<int, WeakReference> imagesByKeys = new Dictionary<int, WeakReference>();
		public ImageCache(IDocumentModel owner)
			: base(owner) {
		}
		protected override void AddImage(OfficeNativeImage image, int key) {
			CleanDeadItems(imagesByKeys);
			CleanDeadItems(imagesByIds);
			imagesByKeys.Add(key, new WeakReference(image));
			imagesByIds.Add(image.Id, new WeakReference(image));
		}
		protected override OfficeNativeImage GetNativeImageById(IUniqueImageId id) {
			return GetNativeImageFromDictionary(imagesByIds, id);
		}
		protected override OfficeNativeImage GetNativeImageByKey(int key) {
			return GetNativeImageFromDictionary(imagesByKeys, key);
		}
		protected OfficeNativeImage GetNativeImageFromDictionary<T>(Dictionary<T, WeakReference> dict, T id) {
			WeakReference reference;
			if (!dict.TryGetValue(id, out reference))
				return null;
			OfficeNativeImage image = (OfficeNativeImage)reference.Target;
			if (image == null) {
				dict.Remove(id);
				return null;
			}
			CleanDeadItems(dict);
			return image;
		}
		protected internal virtual void CleanDeadItems<T>(Dictionary<T, WeakReference> dictionary) {
			List<T> deadKeys = new List<T>();
			foreach (T key in dictionary.Keys) {
				WeakReference reference = dictionary[key];
				OfficeImage image = (OfficeImage)reference.Target;
				if (image == null) {
					deadKeys.Add(key);
				}
			}
			for (int i = deadKeys.Count - 1; i >= 0; i--)
				dictionary.Remove(deadKeys[i]);
		}
		public override void Clear() {
			foreach (int key in imagesByKeys.Keys) {
				WeakReference reference = imagesByKeys[key];
				OfficeImage image = (OfficeImage)reference.Target;
				if (image != null)
					image.Dispose();
			}
			imagesByKeys.Clear();
			foreach (IUniqueImageId key in imagesByIds.Keys) {
				WeakReference reference = imagesByIds[key];
				OfficeImage image = (OfficeImage)reference.Target;
				if (image != null)
					image.Dispose();
			}
			imagesByIds.Clear();
			base.Clear();
		}
	}
	#endregion
	public class PersistentImageCache : ImageCacheBase {
		readonly Dictionary<IUniqueImageId, OfficeNativeImage> imagesByIds = new Dictionary<IUniqueImageId, OfficeNativeImage>();
		readonly Dictionary<int, OfficeNativeImage> imagesByKeys = new Dictionary<int, OfficeNativeImage>();
		public PersistentImageCache(IDocumentModel owner)
			: base(owner) {
		}
		protected override void AddImage(OfficeNativeImage image, int key) {
			imagesByKeys.Add(key, image);
			imagesByIds.Add(image.Id, image);
		}
		protected override OfficeNativeImage GetNativeImageById(IUniqueImageId id) {
			OfficeNativeImage result;
			if (imagesByIds.TryGetValue(id, out result))
				return result;
			return null;
		}
		protected override OfficeNativeImage GetNativeImageByKey(int imageId) {
			OfficeNativeImage result;
			if (imagesByKeys.TryGetValue(imageId, out result))
				return result;
			else
				return null;
		}
		public override void Clear() {
			foreach (IUniqueImageId key in imagesByIds.Keys) {
				OfficeNativeImage image = imagesByIds[key];
				image.Dispose();
			}
			imagesByIds.Clear();
			foreach (int key in imagesByKeys.Keys) {
				OfficeNativeImage image = imagesByKeys[key];
				image.Dispose();
			}
			imagesByKeys.Clear();
			base.Clear();
		}
	}
}
