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
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Xml;
using DevExpress.Data.Browsing;
using DevExpress.Data.Browsing.Design;
using DevExpress.Web;
using DevExpress.XtraPrinting.Native.WebClientUIControl.DataContracts;
using DevExpress.XtraReports.Native.Data;
namespace DevExpress.XtraCharts.Web.Designer.Native {
	[DataContract]
	public class JsonResultWrapper<T> {
		[DataMember(Name = "success")]
		public bool Success { get; set; }
		[DataMember(Name = "error")]
		public string Error { get; set; }
		[DataMember(Name = "result")]
		public T Result { get; set; }
	}
	public class RenderedChart {
		public string Image { get; set; }
		public int[] Indexes { get; set; }
	}
	public class ChartHandler : IHttpModuleSubscriber {
		public static Dictionary<string, object> dataSources = new Dictionary<string, object>();
		public void ProcessRequest() {
			var context = HttpContext.Current;
			HttpRequest request = context.Request;
			HttpResponse response = context.Response;
			response.Clear();
			if (request["actionKey"] == "chart") {
				string json = HttpUtility.UrlDecode(request["arg"]);
				byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
				ChartLayout chartLayout = new ChartLayout();
				DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ChartLayout));
				using (XmlDictionaryReader reader = JsonReaderWriterFactory.CreateJsonReader(jsonBytes, 0, jsonBytes.Length, Encoding.UTF8, XmlDictionaryReaderQuotas.Max, null)) {
					chartLayout = (ChartLayout)serializer.ReadObject(reader, false);
					if (chartLayout.Chart != "{}") {
						using (PreviewChartContainer chartContainer = PreviewChartContainer.CreateFromContract(chartLayout))
						using (Bitmap bitmap = chartContainer.CreateBitmap())
						using (MemoryStream bitmapStream = new MemoryStream()) {
							bitmap.Save(bitmapStream, ImageFormat.Png);
							string imageBase64 = Convert.ToBase64String(bitmapStream.GetBuffer(), 0, (int)bitmapStream.Length);
							int[] indexes = chartContainer.GetIndexesOfIncompatibleViews();
							JsonResultWrapper<RenderedChart> result = new JsonResultWrapper<RenderedChart>() {
								Success = true,
								Result = new RenderedChart() {
									Image = imageBase64,
									Indexes = indexes
								}
							};
							DataContractJsonSerializer serializerResult = new DataContractJsonSerializer(typeof(JsonResultWrapper<RenderedChart>));
							using (MemoryStream stream = new MemoryStream()) {
								serializerResult.WriteObject(stream, result);
								string jsonResult = Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length);
								response.StatusCode = (int)HttpStatusCode.OK;
								response.Write(jsonResult);
								response.ContentType = "application/json";
								response.End();
							}
						}
					}
				}
			}
			else if (request["actionKey"] == "fieldList") {
				string json = HttpUtility.UrlDecode(request["arg"]);
				byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
				DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(FieldListRequest));
				using (XmlDictionaryReader reader = JsonReaderWriterFactory.CreateJsonReader(jsonBytes, 0, jsonBytes.Length, Encoding.UTF8, XmlDictionaryReaderQuotas.Max, null)) {
					var fieldListRequest = (FieldListRequest)serializer.ReadObject(reader, false);
					object dataSource;
					if(fieldListRequest.Id != null && ChartHandler.dataSources.TryGetValue(fieldListRequest.Id, out dataSource)) {
						var dataContextService = new DataContextServiceBase();
						var dataContextOptions = new DataContextOptions(true, false);
						GetPropertiesEventArgs args = null;
						using (var provider = new DataSortedPropertiesProvider(dataContextService.CreateDataContext(dataContextOptions), dataContextService)) {
							provider.GetListItemProperties(dataSource, fieldListRequest.DataMember, (_, e) => args = e);
							var properties = args.Properties
							.Select(x => new FieldListNode {
								Name = x.Name,
								DisplayName = x.DisplayName,
								IsList = x.IsListType || x.IsComplex,
								Specifics = x.Specifics.ToString()
							})
							.ToArray();
							JsonResultWrapper<FieldListNode[]> result = new JsonResultWrapper<FieldListNode[]>() {
								Success = true,
								Result = properties
							};
							DataContractJsonSerializer serializerResult = new DataContractJsonSerializer(typeof(JsonResultWrapper<FieldListNode[]>));
							using (MemoryStream stream = new MemoryStream()) {
								serializerResult.WriteObject(stream, result);
								string jsonResult = Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length);
								response.StatusCode = (int)HttpStatusCode.OK;
								response.Write(jsonResult);
								response.ContentType = "application/json";
								response.End();
							}
						}
					}
				}
			}
		}
		public bool RequestRecipient(HttpRequest request, RequestEvent requestEvent) {
			return request["actionKey"] == "chart" || request["actionKey"] == "fieldList";
		}
	}
}
