#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardCommon.Native;
using System;
using DevExpress.Office.Utils;
using System.IO;
using System.Reflection;
using DevExpress.Utils;
namespace DevExpress.DashboardCommon.ViewModel {
	public class DashboardImageViewModel {
		const string DefaultImage = "DefaultImage";
		const string CorruptedImage = "ImageCorrupted";
		const string ImagePngMimeType = "image/png";
		public string Url { get; set; }
		public string SourceBase64String { get; set; }
		public string MimeType { get; set; }
		public bool IsInternalImage { get; set; }
		public DashboardImageViewModel(DashboardImage image) {
			if(image.Data != null) {
				try {
					SetData(image.Data);
					using(Stream stream = new MemoryStream(image.Data))
						MimeType = OfficeImage.GetContentType(OfficeImage.CreateImage(stream).RawFormat);
				}
				catch {
					SetDefaultImage(CorruptedImage);
				}
			}
			else if(!string.IsNullOrEmpty(image.Url))
				Url = image.Url;
			else
				SetDefaultImage(DefaultImage);
		}
		void SetData(byte[] data) {
			SourceBase64String = Convert.ToBase64String(data);
		}
		void SetDefaultImage(string dashboardImageName) {
			using(Stream stream = GetType().GetAssembly().GetManifestResourceStream(string.Format("DevExpress.DashboardCommon.Images.{0}.png", dashboardImageName))) {
				using(MemoryStream memoryStream = new MemoryStream()) {
					int bytesNumber;
					byte[] buffer = new byte[16 * 1024];
					while((bytesNumber = stream.Read(buffer, 0, buffer.Length)) > 0)
						memoryStream.Write(buffer, 0, bytesNumber);
					SetData(memoryStream.ToArray());
				}
			}
			MimeType = ImagePngMimeType;
			IsInternalImage = true;
		}
	}
}
