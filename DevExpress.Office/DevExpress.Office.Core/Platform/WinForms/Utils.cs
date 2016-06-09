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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Text;
using DevExpress.Office.Layout;
using DevExpress.Office.PInvoke;
using DevExpress.Office.Utils;
using DevExpress.Data.Helpers;
using DevExpress.Office.Model;
namespace DevExpress.Office.Utils {
	#region OfficeClipboard
	public static class OfficeClipboard {
		public static bool ContainsData(string format) {
			try {
				return Clipboard.ContainsData(format);
			}
			catch {
				return false;
			}
		}
		public static IDataObject GetDataObject() {
			try {
				return Clipboard.GetDataObject();
			}
			catch {
				return null;
			}
		}
		public static void Clear() {
			try {
				Clipboard.Clear();
			}
			catch {
			}
		}
		public static void SetDataObject(object data, bool copy) {
			try {
				Clipboard.SetDataObject(data, copy, 10, 100);
			}
			catch {
			}
		}
	}
	#endregion
	#region OfficeDataFormats
	public static class OfficeDataFormats {
		public static readonly string Rtf = DataFormats.Rtf;
		public static readonly string UnicodeText = DataFormats.UnicodeText;
		public static readonly string OemText = DataFormats.OemText;
		public static readonly string Text = DataFormats.Text;
		public static readonly string FileDrop = DataFormats.FileDrop;
		public static readonly string Bitmap = DataFormats.Bitmap;
		public static readonly string EnhancedMetafile = DataFormats.EnhancedMetafile;
		public static readonly string Html = DataFormats.Html;
		public static readonly string SilverlightXaml = "com.microsoft.xaml.silverlight";
		public static readonly string SuppressStoreImageSize = "SuppressStoreImageSize";
		public static readonly string XMLSpreadsheet = "XML Spreadsheet";
		public static readonly string MsSourceUrl = "msSourceUrl";
	}
	#endregion
}
