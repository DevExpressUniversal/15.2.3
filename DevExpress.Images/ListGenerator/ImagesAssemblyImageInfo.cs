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
using System.Linq;
using System.Text;
#if !IMAGELISTGENERATOR
using DevExpress.Utils.Design;
#else
using DevExpress.Internal;
using DevExpress.Utils.Design;
#endif
namespace DevExpress.Images {
	public static class ImageCollectionHelper {
		static Dictionary<ImageType, string> folders = new Dictionary<ImageType, string>() {
			{ImageType.Colored, "Images"},
			{ImageType.GrayScaled, "GrayScaleImages"},
			{ImageType.Office2013, "Office2013"},
			{ImageType.DevAV, "DevAV"}
		};
		internal static ImageType[] IncompleteFolderKeys = new ImageType[] { ImageType.DevAV };
		public static bool IsIncompleteFolder(string item) {
			return false;			
		}
		static IList<string> folderList = null;
		static IList<string> IncompleteFolderList {
			get {
				if(folderList == null) {
					folderList = folders.Where(element => IncompleteFolderKeys.Contains(element.Key)) .Select(element => element.Value).ToList();
				}
				return folderList;
			}
		}
		public static string GetImageFolderName(ImageType imageType) {
			return folders[imageType];
		}
		public static ImageType? GetImageTypeByFolderName(string folderName) {
			var pair = folders.FirstOrDefault(x => x.Value == folderName);
			return pair.Value != folderName ? (ImageType?)null : pair.Key;
		}
		public static ImageType GetImageType(string key) {
			var pair = folders.FirstOrDefault(x => x.Key != ImageType.Colored && key.IndexOf(x.Value, StringComparison.OrdinalIgnoreCase) >= 0);
			return pair.Value == null || key.IndexOf(pair.Value, StringComparison.OrdinalIgnoreCase) < 0 ? ImageType.Colored : pair.Key;
		}
		internal static int ImagesCountForName { get { return folders.Keys.Except(IncompleteFolderKeys).Count() * 2; } }
		public static readonly string ResourceName = AssemblyInfo.SRAssemblyImages + ".g.resources";
	}
	public class ImagesAssemblyImageInfo
#if !IMAGELISTGENERATOR
		: IDXImageInfo 
		#endif
		{
		internal static Dictionary<string, List<ImagesAssemblyImageInfo>> BuildCache(IEnumerable<ImagesAssemblyImageInfo> images, bool keysInLowerCase = true) {
			var tempDictionary = new Dictionary<string, List<ImagesAssemblyImageInfo>>();
			foreach(var image in images) {
				if(ImageCollectionHelper.IncompleteFolderKeys.Contains(image.ImageType))
					continue;
				if(!IsBrowsable(image.MakeUriShort().ToLowerInvariant()))
					continue;
				int index = image.Name.IndexOf("_");
					if(index < 0 || !(image.Name.EndsWith("_16x16.png") || image.Name.EndsWith("_32x32.png")) || image.Name.Contains("-") || image.Name.Contains("%")) {
						LogInconsistentImage(image);
						continue;
					}
				if(image.Name.IndexOf("_", index + 1) >= 0)
					continue;
				string key = image.Name.Substring(0, image.Name.IndexOf("_"));
				if(keysInLowerCase)
					key = key.ToLowerInvariant();
				List<ImagesAssemblyImageInfo> list;
				if(!tempDictionary.TryGetValue(key, out list)) {
					tempDictionary[key] = (list = new List<ImagesAssemblyImageInfo>());
				}
				list.Add(image);
			}
			Dictionary<string, List<ImagesAssemblyImageInfo>> result = new Dictionary<string, List<ImagesAssemblyImageInfo>>();
			foreach(var item in tempDictionary) {
				bool isConsistentGroups = true;
				foreach(var image in item.Value) {
					if(image.Group != item.Value[0].Group)
						isConsistentGroups = false;
				}
				if(!isConsistentGroups) {
					LogInconsistentImage(item.Value.ToArray());
					continue;
				}
				if(item.Value.Count == ImageCollectionHelper.ImagesCountForName)
					result[item.Key] = item.Value;
			}
			return result;
		}
		internal static bool IsBrowsable(string key) {
			return !HiddenKeysStorage.Default.Contains(key);
		}
		static void LogInconsistentImage(params ImagesAssemblyImageInfo[] images) {
#if DEBUGTEST
#endif
		}
#if !SL
		internal const string PackPrefix = "pack://application:,,,";
#else
			internal const string PackPrefix = "";
#endif
		public ImagesAssemblyImageInfo(string group, ImageType imageType, string name, ImageSize size, string[] tags) {
			Group = group;
			ImageType = imageType;
			Name = name;
			Size = size;
			Tags = tags;
		}
		public string Group { get; private set; }
		public ImageType ImageType { get; private set; }
		public string Name { get; private set; }
		public ImageSize Size { get; private set; }
		public string[] Tags { get; private set; }
		public string MakeUri() {
			return string.Format(PackPrefix + "/{0};component/{1}", AssemblyInfo.SRAssemblyImages, MakeUriShort());
		}
		internal string MakeUriShort() {
			return string.Format("{1}/{2}/{3}", AssemblyInfo.SRAssemblyImages, ImageCollectionHelper.GetImageFolderName(ImageType), Group, Name);
		}
	}
	public class HiddenKeysStorage {
		Dictionary<string, bool> storage;
		protected HiddenKeysStorage() {
			this.storage = new Dictionary<string, bool>();
		}
		static HiddenKeysStorage instanceCore = null;
		public static HiddenKeysStorage Default {
			get {
				if(instanceCore == null) {
					instanceCore = new HiddenKeysStorage();
					Initialize(instanceCore);
				}
				return instanceCore;
			}
		}
		static void Initialize(HiddenKeysStorage instance) {
			instance.Storage.Add("grayscaleimages/edit/remove_16x16.png", true);
			instance.Storage.Add("grayscaleimages/edit/remove_32x32.png", true);
			instance.Storage.Add("grayscaleimages/edit/new_16x16.png", true);
			instance.Storage.Add("grayscaleimages/edit/new_32x32.png", true);
			instance.Storage.Add("grayscaleimages/actions/delete_16x16.png", true);
			instance.Storage.Add("grayscaleimages/actions/delete_32x32.png", true);
			instance.Storage.Add("grayscaleimages/actions/openfile_16x16.png", true);
			instance.Storage.Add("grayscaleimages/actions/openfile_32x32.png", true);
		}
		public bool Contains(string key) {
			return Storage.ContainsKey(key);
		}
		Dictionary<string, bool> Storage { get { return storage; } }
	}
}
