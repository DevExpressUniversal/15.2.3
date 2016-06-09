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
using System.IO;
using System.Net;
using DevExpress.Office.Services.Implementation;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public class LoadInlinePicturesCommand : WebRichEditLoadModelCommandBase {
		public LoadInlinePicturesCommand(CommandManager commandManager, Hashtable parameters) 
			: base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.LoadInlinePictures; } }
		protected override bool IsEnabled() { return true; }
		protected override void FillHashtable(Hashtable result) {
			Hashtable loadedImagesInfo = new Hashtable();
			ArrayList imagesInfo = (ArrayList)Parameters["imagesInfo"];
			for(var i = 0; i < imagesInfo.Count; i++) {
				Hashtable imageInfo = (Hashtable)imagesInfo[i];
				string sourceUrl = (string)imageInfo["sourceUrl"];
				int scaleX = Convert.ToInt32(imageInfo["scaleX"]);
				int scaleY = Convert.ToInt32(imageInfo["scaleY"]);
				DocumentLogPosition logPosition = new DocumentLogPosition(Convert.ToInt32(imageInfo["position"]));
				int id = WorkSession.EmptyImageCacheID;
				int originalWidth = 1;
				int originalHeight = 1;
				OfficeImage image;
				Stream stream = null;
				WebResponse response = null;
				try {
					DataStringUriStreamProvider dataStringUriStreamProvider = new DataStringUriStreamProvider();
					stream = dataStringUriStreamProvider.GetStream(sourceUrl);
					if(stream != null)
						image = DocumentModel.CreateImage(stream);
					else {
						WebRequest request = System.Net.WebRequest.Create(sourceUrl);
						response = request.GetResponse();
						stream = new MemoryStream();
						response.GetResponseStream().CopyTo(stream);
						stream.Position = 0;
						image = DocumentModel.CreateImage(stream);
					}
					id = image.ImageCacheKey;
					originalWidth = image.SizeInTwips.Width;
					originalHeight = image.SizeInTwips.Height;
				} catch(Exception) {
				} finally {
					if(stream != null)
						stream.Close();
					if(response != null)
						response.Close();
					loadedImagesInfo.Add(imageInfo["guid"].ToString(), new Hashtable { 
						{ "imageCacheId", id },
						{ "originalWidth", originalWidth },
						{ "originalHeight", originalHeight }
					});
				}
			}
			result.Add("loadedImagesInfo", loadedImagesInfo);
		}
	}
}
