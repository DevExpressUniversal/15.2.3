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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
namespace DevExpress.Entity.ProjectModel {   
	public class ResourceOptions: IResourceOptions {
		public ResourceOptions(bool processEmebedResources, params string[] externalPaths) {
			ProcessEmbededResources = processEmebedResources;
			AddExternalPaths(externalPaths);
		}
		public void AddExternalPaths(params string[] externalPaths) {
			if(externalPaths == null)
				return;
			if(ExternalPaths == null)
				ExternalPaths = new List<string>();
			ExternalPaths.AddRange(externalPaths);
		}
		public List<string> ExternalPaths { get; private set; }
		public bool ProcessEmbededResources { get; set; }
		public static ResourceOptions DefaultOptions { get { return new ResourceOptions(true, null); } }
	}
	class EdmxResources {
		Dictionary<string, EdmxResource> csdlEdmxResources;
		Dictionary<string, EdmxResource> ssdlEdmxResources;
		ResourceOptions options;
		public EdmxResources(Assembly asm)
			: this(asm, null) {
		}
		public EdmxResources(Assembly asm, ResourceOptions options) {
			if(options == null)
				this.options = ResourceOptions.DefaultOptions;
			else
				this.options = options;
			Init(asm);
		}
		public EdmxResource GetEdmxResource(string typeName) {
			if(String.IsNullOrEmpty(typeName) || csdlEdmxResources == null || !csdlEdmxResources.ContainsKey(typeName))
				return null;
			return csdlEdmxResources[typeName];
		}
		static MemoryStream GetMemoryStream(Stream stream) {
			if(stream == null)
				return null;
			MemoryStream result = new MemoryStream();
			stream.CopyTo(result);
			result.Seek(0, SeekOrigin.Begin);
			return result;
		}
		void AddMslResourceByContainersNames(Stream stream) {
			stream = GetMemoryStream(stream);
			if(stream == null)
				return;
			string csdlContainerName;
			string ssdlContainerName;
			EdmxResource.GetContainerNamesFromMsl(stream, out csdlContainerName, out ssdlContainerName);
			if (String.IsNullOrEmpty(csdlContainerName) || String.IsNullOrEmpty(ssdlContainerName))
				return;
			if((csdlEdmxResources != null && csdlEdmxResources.ContainsKey(csdlContainerName))
				|| (ssdlEdmxResources != null && ssdlEdmxResources.ContainsKey(ssdlContainerName)))
				return;
			EdmxResource resource = new EdmxResource(csdlContainerName, ssdlContainerName);
			resource.AddMslContainerStream(stream);
			if(csdlEdmxResources == null)
				csdlEdmxResources = new Dictionary<string, EdmxResource>();
			if(ssdlEdmxResources == null)
				ssdlEdmxResources = new Dictionary<string, EdmxResource>();
			csdlEdmxResources.Add(csdlContainerName, resource);
			ssdlEdmxResources.Add(ssdlContainerName, resource);
		}
		void AddCsdlResourceByContainer(Stream stream) {
			AddResourceByContainerName(csdlEdmxResources, stream, (r, s) => r.AddCsdlContainerStream(s) );
		}
		void AddSsdlResourceByContainer(Stream stream) {
			AddResourceByContainerName(ssdlEdmxResources, stream, (r, s) => r.AddSsdlContainerStream(s) );
		} 
		void AddResourceByContainerName(Dictionary<string, EdmxResource> resources, Stream stream, Action<EdmxResource, Stream> addStream) {		   
			if(resources == null)
				return;
			stream = GetMemoryStream(stream);
			if(stream == null)
				return;
			string containerName = EdmxResource.GetEntityContainerName(stream);
			if(String.IsNullOrEmpty(containerName) || resources == null || !resources.ContainsKey(containerName))
				return;
			EdmxResource resource = resources[containerName];
			if(resource == null)
				return;
			addStream(resource, stream);
		}
		void Init(Assembly asm) {
			InitResources(asm, EdmxResource.MslExtension, AddMslResourceByContainersNames);
			InitResources(asm, EdmxResource.CsdlExtension, AddCsdlResourceByContainer);
			InitResources(asm, EdmxResource.SsdlExtension, AddSsdlResourceByContainer);
		}
		void InitResources(Assembly asm, string extension, Action<Stream> addResource) {
			if(asm == null)
				return;
			InitEmbededResources(asm, extension, addResource);
			InitExternalResources(extension, addResource);
		}
		void InitExternalResources(string extension, Action<Stream> addResource)
		{
			if(String.IsNullOrEmpty(extension) || addResource == null || options.ExternalPaths == null)
				return;
			foreach(string path in options.ExternalPaths)
				InitExternalResources(path, extension, addResource);
		}
		void InitExternalResources(string directoryPath, string extension, Action<Stream> addResource) {
			if(String.IsNullOrEmpty(directoryPath) || !Directory.Exists(directoryPath))
				return;
			try {
				string[] filePaths = Directory.GetFiles(directoryPath, "*" + extension, SearchOption.TopDirectoryOnly);
				if(filePaths == null || filePaths.Length == 0)
					return;
				foreach(string filePath in filePaths) {
					if(String.IsNullOrEmpty(filePath))
						continue;
					byte[] bytes = File.ReadAllBytes(filePath);
					if(bytes == null)
						continue;
					addResource(new MemoryStream(bytes));
				}
			}
			catch {
			}
		}
		void InitEmbededResources(Assembly asm, string extension, Action<Stream> addResource) {
			if(asm == null || !options.ProcessEmbededResources)
				return;
			try {
				string[] resourceNames = asm.GetManifestResourceNames();
				if(resourceNames == null)
					return;
				foreach(string resourceName in resourceNames) {
					if(Path.GetExtension(resourceName) != extension)
						continue;
					addResource(asm.GetManifestResourceStream(resourceName));
				}
			}
			catch {
			}
		}
	}
}
