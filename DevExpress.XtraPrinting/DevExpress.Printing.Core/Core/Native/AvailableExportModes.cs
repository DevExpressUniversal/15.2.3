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
using System.Text;
using System.Collections;
#if SILVERLIGHT
using DevExpress.Data.Browsing;
#else
using System.ComponentModel;
#endif
#if WINRT
using System.Reflection;
#endif
namespace DevExpress.XtraPrinting.Native {
	public class AvailableExportModes {
		IEnumerable<RtfExportMode> rtf;
		IEnumerable<HtmlExportMode> html;
		IEnumerable<ImageExportMode> image;
		IEnumerable<XlsExportMode> xls;
		IEnumerable<XlsxExportMode> xlsx;
		public AvailableExportModes() { }
		public AvailableExportModes(IEnumerable<RtfExportMode> rtf, IEnumerable<HtmlExportMode> html, IEnumerable<ImageExportMode> image, IEnumerable<XlsExportMode> xls, IEnumerable<XlsxExportMode> xlsx) {
			this.rtf = rtf;
			this.html = html;
			this.image = image;
			this.xls = xls;
			this.xlsx = xlsx;
		}
		public IEnumerable<RtfExportMode> Rtf {
			get {
				return rtf;
			}
			set {
				rtf = value;
			}
		}
		public IEnumerable<HtmlExportMode> Html {
			get {
				return html;
			}
			set {
				html = value;
			}
		}
		public IEnumerable<ImageExportMode> Image {
			get {
				return image;
			}
			set {
				image = value;
			}
		}
		public IEnumerable<XlsExportMode> Xls {
			get {
				return xls;
			}
			set {
				xls = value;
			}
		}
		public IEnumerable<XlsxExportMode> Xlsx {
			get {
				return xlsx;
			}
			set {
				xlsx = value;
			}
		}
#if WINRT
	public object[] GetExportModesByType(Type exportModeType) {
		foreach(PropertyInfo pd in GetType().GetTypeInfo().DeclaredProperties) {
			if(pd.PropertyType == (typeof(IEnumerable<>).MakeGenericType(exportModeType))) {
				IList exportModes = (IList)pd.GetValue(this);
				object[] result = new object[exportModes.Count];
				exportModes.CopyTo(result, 0);
				return result;
			}
		}
		return null;
	}
#else
	public object[] GetExportModesByType(Type exportModeType) {
		foreach(PropertyDescriptor pd in TypeDescriptor.GetProperties(this)) {
			if(pd.PropertyType == (typeof(IEnumerable<>).MakeGenericType(exportModeType))) {
				IList exportModes = (IList)pd.GetValue(this);
				object[] result = new object[exportModes.Count];
				exportModes.CopyTo(result, 0);
				return result;
			}
		}
		return null;
	}
#endif
	}
}
