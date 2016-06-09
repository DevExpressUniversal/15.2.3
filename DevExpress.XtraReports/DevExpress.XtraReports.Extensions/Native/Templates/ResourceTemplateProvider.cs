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
using System.IO;
using System.Linq;
using System.Reflection;
using DevExpress.XtraReports.Extensions;
using DevExpress.XtraReports.Templates;
namespace DevExpress.XtraReports.Native.Templates {
	public class ResourceTemplateProvider : TemplateProvider, ITemplateProvider {
		List<Template> templateList;
		List<Template> TemplateList {
			get {
				if(templateList == null) {
					templateList = new List<Template>();
					FillTemplates(templateList);
					templateList.Sort((x, y) => Comparer.DefaultInvariant.Compare(x.Name, y.Name));
				}
				return templateList;
			}
		}
		public ResourceTemplateProvider(string extension, string category)
			: base(extension, category) {
		}
		public static ITemplateProvider CreateReportTemplateProvider() {
			return new ResourceTemplateProvider("repx", "Reports");
		}
		public void UpdateTemplateImage(Template item) {
			return;
		}
		public byte[] GetTemplateLayout(Guid templateID) {
			Template selectedTemplate = GetTemplate(templateID);
			return selectedTemplate != null ? selectedTemplate.LayoutBytes : null;
		}
		Template GetTemplate(Guid templateID) {
			return TemplateList.Where(item => item.ID == templateID).SingleOrDefault();
		}
		void FillTemplates(List<Template> templateList) {
			Assembly asm = typeof(LocalResFinder).Assembly;
			string[] resourceNames = asm.GetManifestResourceNames();
			foreach(string resourceName in resourceNames) {
				if(resourceName.StartsWith(typeof(LocalResFinder).Namespace + ".Native.Templates.Resources.ResourceTemplates")) {
					using(Stream stream = asm.GetManifestResourceStream(resourceName)) {
						Template template = Template.CreateTemplateFromArchive(stream);
						templateList.Add(template);
					}
				}
			}
		}
		public void GetTemplates(string searchString, GetTemplatesHandler getTemplates) {
			List<Template> templates = new List<Template>();
			foreach(Template item in TemplateList) {
				if(string.IsNullOrEmpty(searchString) || item.Name.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0)
					templates.Add(item);
			}
			TemplatesInfo templatesInfo = new TemplatesInfo();
			templatesInfo.ExceptionMessage = null;
			templatesInfo.Templates = templates;
			getTemplates(templatesInfo);
		}		
		public void SendTemplate(string templateName, string description, byte[] layout, System.Drawing.Image preview, System.Drawing.Image icon) {
			string fileName = Path.ChangeExtension(Path.GetTempFileName(), ".zip");
			System.IO.File.WriteAllBytes(fileName, templateArchiveManager.CreateArchive(templateName, description,string.Empty, layout, preview, icon));
		}
		public byte[] GetIconImage(Guid templateID) {
			Template selectedTemplate = GetTemplate(templateID);
			return selectedTemplate != null ? selectedTemplate.IconBytes : null;
		}
		public void GetPreviewImageAsync(Guid templateID, Action<Guid, byte[]> callback) {
			return;
		}
	}
}
