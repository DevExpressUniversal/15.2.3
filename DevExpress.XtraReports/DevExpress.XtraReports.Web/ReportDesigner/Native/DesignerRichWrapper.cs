#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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

using DevExpress.XtraPrinting.Native.WebClientUIControl;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Web.ReportDesigner.Native.DataContracts;
using System;
using System.ComponentModel;
using System.IO;
namespace DevExpress.XtraReports.Web.ReportDesigner.Native {
	[ToolboxItem(false)]
	public class DesignerRichWrapper : XRRichText {
		public static XRRichText createFromRequest(RichEditorRequest request) {
			var richText = new DesignerRichWrapper();
			var xmlLayout = JsonConverter.JsonToXml(request.Layout, false);
			using(var stream = new MemoryStream(xmlLayout)) {
				richText.LoadFromXml(stream);
			}
			switch(request.PropertyName) {
				case "rtf":
					richText.Rtf = request.Rtf;
					break;
				case "text":
					richText.Text = request.Text;
					break;
				case "base64rtf":
					richText.Rtf = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(request.Base64Rtf));
					break;
				case "font":
					richText.OnFontChanged();
					break;;
				case "foreColor":
					richText.OnForeColorChanged();
					break; ;
				default:
					break;
			}
			return richText;
		}
		public DesignerRichWrapper()
			: base() {
			this._report = new XtraReport();
		}
		public override XtraReport RootReport {
			get { return this._report; }
		}
		XtraReport _report;
	}
}
