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
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using DevExpress.XtraReports.Extensions;
using DevExpress.XtraReports.Templates;
namespace DevExpress.XtraReports.Native.Templates {
	public class DXTemplateProvider : TemplateProvider, ITemplateProvider {
		#region fields & properties
		Action<Guid, byte[]> GetPreviewImageHandler;
		Guid id;
		WebClient previewImageLoader;
		#endregion
		#region static
		public static ITemplateProvider CreateReportTemplateProvider() { 
			return new DXTemplateProvider("repx", "Reports");
		}
		static void GetTemplatePreview(WebClient webClient,Guid templateID) {
			try {
				webClient.DownloadDataAsync(new Uri("http://templates.devexpress.com/preview?id=" + templateID.ToString())); 
			}
			catch {
			}
		}
		static byte[] GetTemplateIcon(Guid templateID) {
			try {
				WebClient webClient = new WebClient();
				return webClient.DownloadData(new Uri("http://templates.devexpress.com/icon?id=" + templateID.ToString()));
			} catch {
				return null;
			}
		}
		#endregion
		#region inner classes
		class AsyncHttpWebRequest {
			static string GetResourceString(WebResponse response) {
				using (Stream stream = response.GetResponseStream()) {
					using (StreamReader reader = new StreamReader(stream)) {
						return reader.ReadToEnd();
					}
				}
			}
			GetDataHandler getData;
			HttpWebRequest httpRequest;
			public void GetDataAsync(string uriString, GetDataHandler getData) {
				this.getData = getData;
				httpRequest = (HttpWebRequest)HttpWebRequest.Create(uriString);
				httpRequest.Method = "GET";
				httpRequest.BeginGetResponse(new AsyncCallback(GetResult), null);
			}
			void GetResult(IAsyncResult asyncResult) {
				ResponseInfo resultInfo = new ResponseInfo();
				try {
					HttpWebResponse response = (HttpWebResponse)httpRequest.EndGetResponse(asyncResult);
					if (response.StatusCode == HttpStatusCode.OK)
						resultInfo.Data = GetResourceString(response);
					response.Close();
				}
				catch (Exception e) {
					resultInfo.ExceptionMessage = e.Message;
				}
				getData(resultInfo);
			}
		}
		#endregion
		public DXTemplateProvider(string extension, string category)
			: base(extension, category) {
		}
		GetTemplatesHandler getTemplates;
		void GetDataAsync(ResponseInfo resultInfo) {
			if(!string.IsNullOrEmpty(resultInfo.ExceptionMessage)) {
				getTemplates(new TemplatesInfo() { ExceptionMessage = resultInfo.ExceptionMessage });
				return;
			}
			byte[] fakedImagePreviewBytes = GetFakedReportImage("fakedTemlatePreview.png");
			byte[] fakedImageIconBytes = GetFakedReportImage("fakedTemlateIcon.png");
			using(MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(resultInfo.Data))) {
				List<Template> result = (List<Template>)new DataContractJsonSerializer(typeof(List<Template>)).ReadObject(stream);
				if(result == null)
					result = new List<Template>();
				foreach(Template item in result) {
					item.PreviewBytes = fakedImagePreviewBytes;
					item.IconBytes = fakedImageIconBytes;
				}
				getTemplates(new TemplatesInfo() { Templates = result });
			}
		}
		public static byte[] GetFakedReportImage(string imageName) {
			Assembly asm = Assembly.GetExecutingAssembly();
			string[] resourceNames = asm.GetManifestResourceNames();
			foreach(string resourceName in resourceNames) {
				if(resourceName.Contains(imageName)) {
					using(Stream stream = typeof(LocalResFinder).Assembly.GetManifestResourceStream(resourceName)) {
						byte[] bytes = new byte[stream.Length];
						stream.Read(bytes, 0, bytes.Length);
						return bytes;
					}
				}
			}
			return null;
		}
		public void SendTemplate(string templateName, string description, byte[] layout, Image preview, Image icon) {
		}
		public virtual void GetTemplates(string searchString, GetTemplatesHandler getTemplates) {
			this.getTemplates = getTemplates;
			string uriString = "http://templates.devexpress.com/api/list?t=" + extension;
			if(!string.IsNullOrEmpty(searchString))
				uriString += string.Format("&q={0}", searchString);
			new AsyncHttpWebRequest().GetDataAsync(uriString, GetDataAsync);
		}
		public virtual byte[] GetTemplateLayout(Guid templateID) {
			try {
				WebClient webClient = new WebClient();
				var data = webClient.DownloadData("http://templates.devexpress.com/api/get?id=" + templateID.ToString());
				return data;
			} catch {
				return null;
			}
		}
		public void GetPreviewImageAsync(Guid templateID, Action<Guid, byte[]> callback) {
			if(previewImageLoader != null) {
				previewImageLoader.DownloadDataCompleted -= webClient_DownloadDataCompleted;
				previewImageLoader.CancelAsync();
				previewImageLoader.Dispose();
			}
			previewImageLoader = new WebClient();
			previewImageLoader.DownloadDataCompleted += new DownloadDataCompletedEventHandler(webClient_DownloadDataCompleted);
			GetPreviewImageHandler = callback;
			id = templateID;
			GetTemplatePreview(previewImageLoader, templateID);
		}
		public byte[] GetIconImage(Guid templateID) {
			return GetTemplateIcon(templateID);
		}
		void webClient_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e) {
			GetPreviewImageHandler(id, (byte[])e.Result);
		}
	}
}
