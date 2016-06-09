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
using System.Drawing;
using System.ComponentModel;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils.Serializing;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.DocumentView;
#if SL
using DevExpress.Xpf.Collections;
#else
using System.Windows.Forms;
#endif
namespace DevExpress.XtraPrinting.Native {
	public class DocumentSerializationCollection : CollectionBase {
		static string GetPageName(int i) {
			return "Page" + (i + 1);
		}
		public DocumentSerializationCollection() {
		}
		public void Add(XtraObjectInfo info) {
			List.Add(info);
		}
		public void Add(ContinuousExportInfo info) {
			Add(new XtraObjectInfo("ContinuousExportInfo", info));
		}
		public virtual void Add(DocumentSerializationOptions options) {
			Add(new XtraObjectInfo("Options", options));
		}
		public void Add(Page page, int index) {
			XtraObjectInfo pageInfo = CreatePageInfo(page, index);
			Add(pageInfo);
		}
		protected XtraObjectInfo CreatePageInfo(Page page, int index) {
			return new XtraObjectInfo(GetPageName(index), page);
		}
		protected XtraObjectInfo CreatePageInfo(Page page, int index, Predicate<int> predicate) {
			return predicate(index) ? CreatePageInfo(page, index) : XtraSkipObjectInfo.SkipObjectInfoInstance;
		}
		public void AddRange(IList<Page> source, Predicate<int> predicate) {
			for(int i = 0; i < source.Count; i++) {
				XtraObjectInfo pageInfo = CreatePageInfo(source[i], i, predicate);
				Add(pageInfo);
			}
		}
	}
	public class DocumentDeserializationCollection : DocumentSerializationCollection {
		Document document;
		Predicate<int> predicate;
		public DocumentDeserializationCollection(Document document, Predicate<int> predicate) {
			this.document = document;
			this.predicate = predicate;
		}
		public override void Add(DocumentSerializationOptions options) {
			base.Add(options);
			options.PageCountChanged += new EventHandler(OnPageCountChanged);
		}
		void OnPageCountChanged(object sender, EventArgs e) {
			DocumentSerializationOptions options = (DocumentSerializationOptions)sender;
			options.PageCountChanged -= new EventHandler(OnPageCountChanged);
			CreatePages(options.PageCount);
			document.ProgressReflector.InitializeRange(options.PageCount);
		}
		void CreatePages(int pageCount) {
			object lastItem = List[Count - 1];
			List.RemoveAt(Count - 1);
			for(int i = 0; i < pageCount; i++) {
				Page page = AddPageToDocument(i, pageCount);
				XtraObjectInfo pageInfo = CreatePageInfo(page, i, predicate);
				Add(pageInfo);
			}
			List.Add(lastItem);
		}
		Page AddPageToDocument(int index, int pageCount) {
			Page page = document.PrintingSystem.CreatePage();
			document.Pages.AddPageInternal(page);
			page.OriginalIndex = index;
			page.OriginalPageCount = pageCount;
			return page;
		}
	}
}
