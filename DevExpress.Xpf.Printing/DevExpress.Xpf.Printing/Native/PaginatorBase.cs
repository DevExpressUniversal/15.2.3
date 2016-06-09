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

using System.Windows;
#if SL
using DocumentPageAlias = System.Windows.FrameworkElement;
#else
using System;
using System.Windows.Documents;
using DocumentPageAlias = System.Windows.Documents.DocumentPage;
#endif
namespace DevExpress.Xpf.Printing.Native {
	public abstract partial class PaginatorBase : DocumentPaginator {
		#region Properties
#if !SL
		public override bool IsPageCountValid {
			get { return true; }
		}
		public override Size PageSize {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		public override IDocumentPaginatorSource Source {
			get { return this; }
		}
#endif
		#endregion
		#region Methods
		protected abstract FrameworkElement GetPageContent(int pageNumber);
		public override DocumentPageAlias GetPage(int pageNumber) {
#if SL
			return GetPageContent(pageNumber);
#else
			FrameworkElement content = GetPageContent(pageNumber);
			Size pageSize = new Size(content.Width, content.Height);
			content.Arrange(new Rect(pageSize));
			DocumentPage documentPage = new DocumentPage(content, pageSize, new Rect(pageSize), new Rect(pageSize));
			return documentPage;
#endif
		}
		#endregion
	}
#if !SL
	public abstract partial class PaginatorBase : IDocumentPaginatorSource {
		public DocumentPaginator DocumentPaginator {
			get { return this; }
		}
	}
#endif
}
