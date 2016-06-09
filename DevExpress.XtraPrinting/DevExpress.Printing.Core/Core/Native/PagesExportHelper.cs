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
using System.IO;
using DevExpress.Utils;
namespace DevExpress.XtraPrinting.Native {
	class PagesExportHelper {
		PrintingSystemBase ps;
		PageByPageExportOptionsBase options;
		int[] pageIndices;
		public int PageCount {
			get {
				return pageIndices.Length;
			}
		}
		public PagesExportHelper(PrintingSystemBase ps, PageByPageExportOptionsBase options) {
			this.ps = ps;
			this.options = options;
			pageIndices = options.GetPageIndices(ps.PageCount);
		}
		public string[] Execute(string filePath, Action1<Stream> export) {
			List<string> files = new List<string>();
			for(int i = 0; i < pageIndices.Length; i++) {
				int pageIndex = pageIndices[i] + 1;
				options.PageRange = pageIndex.ToString();
				string indexFilePath = GetFilePathWithPageIndex(filePath, options.PageRange, ps.PageCount);
				files.Add(indexFilePath);
				FileExportHelper.Do(indexFilePath, export);				
			}
			return files.ToArray();
		}
		static string GetFilePathWithPageIndex(string fileName, string pageIndex, int maxPageIndex) {
			string extension = Path.GetExtension(fileName);
			string fileNameWithoutExtension = fileName.Substring(0, fileName.Length - extension.Length);
			return GetStringWithPageIndex(fileNameWithoutExtension, pageIndex, maxPageIndex) + extension;
		}
		public static string GetStringWithPageIndex(string str, string pageIndex, int maxPageIndex) {
			int zeroCount = (int)Math.Ceiling(Math.Log10(maxPageIndex));
			return string.Format("{0}{1:" + new string('0', zeroCount) + "}", str, pageIndex);
		}
		public string[] Execute(int progressRange, string filePath, Action1<Stream> calback) {
			try {
				ps.ProgressReflector.InitializeRange(progressRange);
				return Execute(filePath, calback);
			} finally {
				ps.ProgressReflector.MaximizeRange();
			}
		}
	}
}
