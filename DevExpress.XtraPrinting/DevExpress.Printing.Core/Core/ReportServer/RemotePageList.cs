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
using DevExpress.DocumentView;
using DevExpress.ReportServer.Printing.Services;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.ReportServer.Printing {
	class RemotePageList : PageList, IDisposable {
		#region fields and properties
		RemoteInnerPageList RemoteInnerPageList { get { return (RemoteInnerPageList)InnerList; } }
		#endregion
		#region ctor
		public RemotePageList(RemoteDocument document, RemoteInnerPageList list)
			: base(document, list) {
			list.PageCountChanged += innerList_PageCountChanged;
		}
		#endregion
		#region methods
		protected override void OnClear() {
			base.OnClear();
			InnerList.Clear();
		}
		public void SetCount(int pageCount) {
			if(RemoteInnerPageList.Count != pageCount)
				RemoteInnerPageList.SetCount(pageCount);
		}
		void innerList_PageCountChanged(object sender, EventArgs e) {
			RaiseDocumentChanged();
		}
		public void Dispose() {
			RemoteInnerPageList.Dispose();
		}
		#endregion
		internal void ReplaceCachedPage(int i, Page page) {
			page.Owner = this;
			RemoteInnerPageList.ReplaceCachedPage(i, page);
		}
	}
}
