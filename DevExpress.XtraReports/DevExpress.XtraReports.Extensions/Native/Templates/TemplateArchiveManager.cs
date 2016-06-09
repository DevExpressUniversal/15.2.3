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
using System.IO;
using System.Drawing;
using DevExpress.Utils.Zip;
using System.Xml;
using DevExpress.Utils;
using DevExpress.XtraReports.Templates;
namespace DevExpress.XtraReports.Native.Templates {
	public class TemplateArchiveManager : ITemplateArchiveManager {
		string category; 
		string extension;
		internal TemplateArchiveManager(string extension, string category) {
			this.extension = extension;
			this.category = category;
		}
		public TemplateArchiveManager()
			: this("repcx", string.Empty) {
		}
		public byte[] CreateArchive(string templateName, string description, string author, byte[] layout, Image preview, Image icon) {
			string layoutFile = Path.ChangeExtension(templateName, extension);
			String XML = @"<?xml version='1.0' encoding='utf-8' ?>" +
				string.Format(@"<Manifest><Template file='{0}' contentType='application/octet-stream' category='{1}' author='{2}'>{3}</Template></Manifest>",
				layoutFile, category, author,description);
			using(MemoryStream stream = new MemoryStream()) {
				using (InternalZipArchive zipArchive = new InternalZipArchive(stream)) {
					DateTime now = DateTime.Now;
					zipArchive.Add("manifest.xml", now, XML);
					zipArchive.Add(layoutFile, now, System.Text.Encoding.UTF8.GetString(layout, 0, layout.Length));
					zipArchive.Add("icon.png", now, FormTemplateHelper.ImageToArray(FormTemplateHelper.CreateImage(icon, new Size(96, 96), Color.White)));
					zipArchive.Add("preview.png", now, FormTemplateHelper.ImageToArray(FormTemplateHelper.CreateImage(preview, new Size(346, 386), Color.White)));
				}
				return stream.ToArray();
			}
		}
		[Obsolete("Use the DevExpress.XtraReports.Template.CreateTemplateFromArchive method instead")]
		public Template GetTemplatesFromArchive(Stream stream) {
			return Template.CreateTemplateFromArchive(stream);
		}
	}
}
