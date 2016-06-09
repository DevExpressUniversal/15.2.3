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

using System.IO;
using System.Web.UI;
using DevExpress.XtraPrinting.HtmlExport.Native;
namespace DevExpress.XtraReports.Web.Native {
	class DXHtmlTextWriterEx : DXHtmlTextWriter {
		readonly Page page;
		readonly bool shouldCompressOutput;
		public DXHtmlTextWriterEx(TextWriter writer, Page page, bool shouldCompressOutput)
			: base(writer, shouldCompressOutput ? string.Empty : "\t") {
			this.page = page;
			this.shouldCompressOutput = shouldCompressOutput;
		}
		public override string NewLine {
			get { return shouldCompressOutput ? string.Empty : base.NewLine; }
			set { base.NewLine = value; ; }
		}
		public override void WriteAttribute(string name, string value, bool fEncode) {
			base.WriteAttribute(name, GetValidValue(name, value), fEncode);
		}
		string GetValidValue(string name, string value) {
			return page != null && (name == "src" || name == "href") && value != null && value[0] == '~'
				? page.ResolveUrl(value)
				: value;
		}
	}
}
