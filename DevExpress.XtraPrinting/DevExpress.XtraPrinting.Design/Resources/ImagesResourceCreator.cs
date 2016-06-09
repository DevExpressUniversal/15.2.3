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
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Resources;
using System.IO;
using System.Resources.Tools;
using System.Drawing;
using System.Globalization;
using System.Drawing.Imaging;
using System.Reflection;
namespace DevExpress.XtraPrinting.Design.Resources {
	public static class ImagesResourceHelper {
		class ResXDataNodeBuilder : DevExpress.Utils.Serializers.MultiTargetBuilder {
			public ResXDataNodeBuilder(IServiceProvider provider) : base(provider) {
			}
			protected override ConstructorInfo CreateConstructor(Type delegateType) {
				return typeof(ResXDataNode).GetConstructor(new Type[] { typeof(string), typeof(object), delegateType });
			}
			public ResXDataNode CreateResXDataNode(string key, Image value) {
				if(skipMultitargetService)
					return new ResXDataNode(key, value);
				return (ResXDataNode)nodeCostructor.Invoke(new object[] { key, value, typeNameConverter });
			}
		}
		public static Dictionary<string, Image> CreateResourcesFile(IServiceProvider provider, Dictionary<string, Image> images, string name) {
			ResourcePickerService resourcePickerService = new ResourcePickerService(provider);
			ResXResourceService resxService = new ResXResourceService(provider, resourcePickerService);
			Dictionary<string, string> resXFilePaths = resourcePickerService.GetProjectResXFileNames();
			string activeProjectPath = resourcePickerService.ActiveProjectPath;
			string fullName = !string.IsNullOrEmpty(activeProjectPath) ? (activeProjectPath + name) : null;
			Hashtable existingResources = new Hashtable();
			if(!resXFilePaths.ContainsKey(name)) {
				using(Stream stream = File.Create(fullName)) { }
			} else {
				using(IResourceReader reader = resxService.GetResXResourceReader(fullName, true)) {
					foreach(DictionaryEntry entry in reader) {
						ResXDataNode node = (ResXDataNode)entry.Value;
						existingResources.Add(entry.Key, node);
					}
				}
			}
			ResXDataNodeBuilder resXDataNodeBuilder = new ResXDataNodeBuilder(provider);
			using(IResourceWriter writer = resxService.GetResXResourceWriter(fullName)) {
				foreach(DictionaryEntry entry in existingResources) {
					ResXDataNode node = (ResXDataNode)entry.Value;
					writer.AddResource(node.Name, node);
				}
				foreach(KeyValuePair<string, Image> imageInfo in images) {
					if(existingResources.ContainsKey(imageInfo.Key)) continue;
					writer.AddResource(imageInfo.Key, resXDataNodeBuilder.CreateResXDataNode(imageInfo.Key, imageInfo.Value));
				}
				writer.Generate();
			}
			if(!resXFilePaths.ContainsKey(name))
				resourcePickerService.AddResXFileToProject(fullName);
			Dictionary<string, Image> imagesFromResources = new Dictionary<string, Image>();
			foreach(KeyValuePair<string, Image> imageInfo in images) {
				imagesFromResources[imageInfo.Key] = LoadImageFromResource(provider, resourcePickerService, name, imageInfo.Key);
			}
			return imagesFromResources;
		}
		public static Image LoadImageFromResource(IServiceProvider provider, string resXName, string key) {
			return LoadImageFromResource(provider, new ResourcePickerService(provider), resXName, key);
		}
		static Image LoadImageFromResource(IServiceProvider provider, ResourcePickerService resourcePickerService, string resXName, string key) {
			string resourcesName = GetVerifiedName(resourcePickerService, Path.GetFileNameWithoutExtension(resXName));
			string imageName = GetVerifiedName(resourcePickerService, key);
			string fullName = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", new object[] { resourcesName, imageName });
			IReferenceService referenceService = provider.GetService(typeof(IReferenceService)) as IReferenceService;
			return (referenceService.GetReference(fullName) as Image);
		}
		static string GetVerifiedName(ResourcePickerService resourcePickerService, string name) {
			return StronglyTypedResourceBuilder.VerifyResourceName(name, resourcePickerService.CodeProvider);
		}
	}
}
