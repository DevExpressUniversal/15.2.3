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
using System.Linq;
using System.Collections.Generic;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Preview;
namespace DevExpress.XtraPrinting.Native {
	public class DocumentHelper : IDisposable {
		int pageIndex;
		protected PrintingDocument document;
		protected PageBuildEngine pageBuildEngine;
		public DocumentHelper(PrintingDocument document, PageBuildEngine pageBuildEngine) {
			this.document = document;
			this.pageBuildEngine = pageBuildEngine;
		}
		protected void BuildPagesCore() {
			document.Pages.Clear();
			document.PrintingSystem.PerformIfNotNull<GroupingManager>(groupingManager => groupingManager.Clear());
			pageBuildEngine.BuildPages(document.Root);
		}
		public virtual void BuildPages() {
			BuildPagesCore();
			pageBuildEngine.AfterBuildPages();
			document.AfterBuild();
		}
		public virtual void AddPage(PSPage page) {
			AddPageCore(page);
		}
		protected void AddPageCore(PSPage page) {
			document.Pages.AddPageInternal(page);
			document.PrintingSystem.OnAfterPagePrint(new PageEventArgs(page, null));
			GroupingManager groupingManager = document.PrintingSystem.GetService<GroupingManager>();
			if(groupingManager == null) return;
			if(page.X == 0)
				pageIndex = groupingManager.TryBuildPageGroups(page.ID, page.Index) ? page.Index : -1;
			else if(pageIndex >= 0) 
				groupingManager.UpdatePageGroups(pageIndex);
			groupingManager.PageBands.Remove(page.ID);
		}
		public virtual void Dispose() {
			if(pageBuildEngine != null)
				pageBuildEngine.Abort();
		}
		public void StopPageBuilding() {
			if(pageBuildEngine != null)
				pageBuildEngine.Stop();
		}
		public virtual int PageCount {
			get {
				return document.Pages.Count;
			}
		}
	}
	public class BackgroundDocumentHelper : DocumentHelper {
		List<PSPage> pageBuffer = new List<PSPage>();
		bool isDisposed;
		BackgroundPageBuildEngine PageBuildEngine {
			get { return (BackgroundPageBuildEngine)base.pageBuildEngine; }
		}
		public BackgroundDocumentHelper(PrintingDocument document, BackgroundPageBuildEngine pageBuildEngine)
			: base(document, pageBuildEngine) {
			this.PageBuildEngine.AfterBuild += AfterBuildProc;
		}
		public override void Dispose() {
			if(PageBuildEngine != null)
				PageBuildEngine.AfterBuild -= AfterBuildProc;
			base.Dispose();
			if(!isDisposed)
				isDisposed = true;
		}
		public override void BuildPages() {
			BuildPagesCore();
		}
		void AfterBuildProc() {
			if(!isDisposed && document.IsCreating) {
				UpdatePages();
				PageBuildEngine.AfterBuildPages();
				document.AfterBuild();
			}
		}
		public override void AddPage(PSPage page) {
			lock(document) {
				pageBuffer.Add(page);
			}
			ContentChangedProc();
		}
		void ContentChangedProc() {
			if(isDisposed)
				return;
			int pageCount = document.Pages.Count;
			lock(document) {
				if(pageBuffer.Count < PageBuildEngine.GetBufferSize(pageCount))
					return;
				UpdatePages();
			}
			if(pageCount != document.Pages.Count)
				document.OnContentChanged();
		}
		void UpdatePages() {
			for(int i = 0; i < pageBuffer.Count; i++)
				AddPageCore(pageBuffer[i]);
			pageBuffer.Clear();
		}
		public override int PageCount {
			get {
				return document.Pages.Count + pageBuffer.Count;
			}
		}
	}
}
