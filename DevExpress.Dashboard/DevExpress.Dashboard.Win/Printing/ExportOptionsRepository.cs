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

using DevExpress.DashboardCommon.Printing;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Linq;
namespace DevExpress.DashboardWin.Native.Printing {
	public class ExportOptionsRepository {
		static readonly string[] CommonOptions = new string[] { 
			ExportOptionsWrapperKeys.Landscape,
			ExportOptionsWrapperKeys.PaperKind,
			ExportOptionsWrapperKeys.FilterStatePresentation,
			ExportOptionsWrapperKeys.ImageFormat,
			ExportOptionsWrapperKeys.ExcelFormat,
			ExportOptionsWrapperKeys.CsvValueSeparator,
			ExportOptionsWrapperKeys.ImageResolution
		};
		static readonly string[] ItemOptions = new string[] { 
			ExportOptionsWrapperKeys.ScaleMode,
			ExportOptionsWrapperKeys.ScaleFactor,
			ExportOptionsWrapperKeys.AutoFitPageCount,
			ExportOptionsWrapperKeys.AutoRotate,
			ExportOptionsWrapperKeys.AutoFitToPageSize,
			ExportOptionsWrapperKeys.ShowTitle,
			ExportOptionsWrapperKeys.Title,
			ExportOptionsWrapperKeys.Title,
			ExportOptionsWrapperKeys.PrintHeadersOnEveryPage,
			ExportOptionsWrapperKeys.ItemSizeMode,
			ExportOptionsWrapperKeys.AutoArrangeContent
		};
		static void AddOption(Dictionary<string, object> rep, string key, ExportOptionsWrapper def, ExportOptionsWrapper act) {
			object actual = act.Get(key);
			if(def.Get(key).Equals(actual))
				rep.Remove(key);
			else
				rep[key] = actual;
		}
		static void SetActualValue(Dictionary<string, object> rep, string key, ExportOptionsWrapper def, ExportOptionsWrapper act) {
			object value = rep.ContainsKey(key) ? rep[key] : def.Get(key);
			act.Set(key, value);
		}
		readonly Dictionary<string, object> commonRep = new Dictionary<string, object>();
		readonly Dictionary<string, Dictionary<string, object>> itemsRep = new Dictionary<string, Dictionary<string, object>>();
		public void Add(string name, ExtendedReportOptions def, ExtendedReportOptions act) {
			ExportOptionsWrapper defWrapper = new ExportOptionsWrapper(def);
			ExportOptionsWrapper actWrapper = new ExportOptionsWrapper(act);
			itemsRep[name] = new Dictionary<string, object>();
			foreach(string key in CommonOptions)
				AddOption(commonRep, key, defWrapper, actWrapper);
			foreach(string key in ItemOptions)
				AddOption(itemsRep[name], key, defWrapper, actWrapper);
		}
		public ExtendedReportOptions GetActualOpts(string name, ExtendedReportOptions def) {
			ExtendedReportOptions opts = ExtendedReportOptions.Empty;
			ExportOptionsWrapper defWrapper = new ExportOptionsWrapper(def);
			ExportOptionsWrapper actWrapper = new ExportOptionsWrapper(opts);
			if(!itemsRep.ContainsKey(name))
				itemsRep[name] = new Dictionary<string, object>();
			foreach(string key in CommonOptions)
				SetActualValue(commonRep, key, defWrapper, actWrapper);
			foreach(string key in ItemOptions)
				SetActualValue(itemsRep[name], key, defWrapper, actWrapper);
			return opts;
		}
	}
}
