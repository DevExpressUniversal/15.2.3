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
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using DevExpress.Utils;
using DevExpress.XtraReports.Configuration;
using DevExpress.XtraReports.Templates;
namespace DevExpress.XtraReports.Extensions {
	public abstract class ReportTemplateExtension : ITemplateProvider {
		const string defaultConfigurationSectionName = "devExpress/xtraReports";
		static ITemplateProvider registeredExtension;
		IEnumerable<Template> templates;
		public static void RegisterExtensionGlobal(ITemplateProvider extension) {
			registeredExtension = extension;
		}
		internal static ITemplateProvider GetTemplateProvider() {
			var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			XtraReportsConfigurationSection section = config.GetSection(defaultConfigurationSectionName) as XtraReportsConfigurationSection;
			if(section != null) {
				List<string> directories = new List<string>();
				foreach(DirectoryElement directory in section.ReportGallery) {
					string path = directory.Path;
					if(string.IsNullOrEmpty(path) || !Directory.Exists(path))
						throw new DirectoryNotFoundException(path);
					directories.Add(path);
				}
				return new ReportTemplateDirectoryExtension(directories.ToArray());
			} else{
				return registeredExtension ?? null;
			}				
		}
		public abstract IEnumerable<Template> GetTemplates();
		public void ResetTemplates() {
			if(templates != null) {
				foreach(Template template in templates)
					if(template is IDisposable)
						((IDisposable)template).Dispose();
			}
			templates = null;
		}
		IEnumerable<Template> Templates {
			get {
				if(templates == null) {
					var enumerable = GetTemplates();
					templates = enumerable.ToArray();
				}
				return templates;
			}
		}
		#region ITemplateProvider Members
		void ITemplateProvider.GetTemplates(string searchString, GetTemplatesHandler getTemplates) {
			TemplatesInfo templatesInfo = new TemplatesInfo() {
				Templates = new List<Template>(Templates.Where(t=>t.Name.ToLower().Contains(searchString.ToLower()))),
				ExceptionMessage = null
			};
			getTemplates(templatesInfo);
		}
		byte[] ITemplateProvider.GetTemplateLayout(Guid templateID) {
			return Templates.First(x => x.ID == templateID).LayoutBytes;
		}
		void ITemplateProvider.SendTemplate(string templateName, string description, byte[] layout, Image preview, Image icon) {
			throw new NotImplementedException();
		}
		public byte[] GetIconImage(Guid templateID) {
			return Templates.First(x => x.ID == templateID).IconBytes;
		}
		public void GetPreviewImageAsync(Guid templateID, Action<Guid, byte[]> callback) {
			callback(templateID, Templates.First(x => x.ID == templateID).PreviewBytes);
		}
		#endregion
	}
	public class ReportTemplateDirectoryExtension : ReportTemplateExtension {
		string[] templatesPaths;
		public ReportTemplateDirectoryExtension(string[] templatesPaths) {
			this.templatesPaths = templatesPaths;
		}
		public override IEnumerable<Template> GetTemplates() {
			List<Template> templates = new List<Template>();
			List<string> fileNames = new List<string>();
			ArrayHelper.ForEach(templatesPaths, x => fileNames.AddRange(Directory.GetFiles(x)));
			foreach(string fileName in fileNames) {
				Stream stream = new MemoryStream(File.ReadAllBytes(fileName));
				try {
					Template template = Template.CreateTemplateFromArchive(stream);
					templates.Add(template);
				} catch {
				} finally {
					stream.Dispose();
				}
			}
			return templates;
		}
	}
	public interface ITemplateProvider {
		void GetTemplates(string searchString, GetTemplatesHandler getTemplates);
		byte[] GetTemplateLayout(Guid templateID);
		void GetPreviewImageAsync(Guid templateID, Action<Guid, byte[]> callback);
		byte[] GetIconImage(Guid templateID);
		void SendTemplate(string templateName, string description, byte[] layout, Image preview, Image icon);
	}
	public class ResponseInfo {
		public string Data { get; set; }
		public string ExceptionMessage { get; set; }
	}
	public class TemplatesInfo {
		public List<Template> Templates { get; set; }
		public string ExceptionMessage { get; set; }
	}
	public delegate void GetTemplatesHandler(TemplatesInfo templatesInfo);
	public delegate void GetDataHandler(ResponseInfo responseInfo);
}
