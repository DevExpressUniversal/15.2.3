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
using System.Linq;
using System.Text;
using DevExpress.Office.Model;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public class ASPxInternalRichEditDocumentServer : InternalRichEditDocumentServer {
		public ASPxInternalRichEditDocumentServer()
			: base(RichEditDocumentFormatsDependecies.CreateDocumentFormatsDependecies()) {
		}
		protected internal override InnerRichEditDocumentServer CreateInnerServer(XtraRichEdit.Model.DocumentModel documentModel) {
			if (documentModel == null)
				return new ASPxInnerRichEditDocumentServer(this);
			else
				return new ASPxInnerRichEditDocumentServer(this, documentModel);
		}
	}
	public class ASPxInnerRichEditDocumentServer : InnerRichEditDocumentServer {
		public ASPxInnerRichEditDocumentServer(IInnerRichEditDocumentServerOwner owner) : base(owner) {
		}
		public ASPxInnerRichEditDocumentServer(IInnerRichEditDocumentServerOwner owner, DocumentModel documentModel)
			: base(owner, documentModel) {
		}
		protected override DocumentModel CreateDocumentModelCore() {
			return new ASPxDocumentModel(RichEditDocumentFormatsDependecies.CreateDocumentFormatsDependecies());
		}
	}
	public class ASPxDocumentModel : DocumentModel {
		public ASPxDocumentModel(DocumentFormatsDependencies dependencies)
			: base(dependencies) {
		}
		protected internal ASPxDocumentModel(bool addDefaultsList, bool changeDefaultTableStyle, DocumentFormatsDependencies dependencies)
			: base(addDefaultsList, changeDefaultTableStyle, dependencies) {
		}
		public override DocumentModel CreateNew() {
			return new ASPxDocumentModel(DocumentFormatsDependencies);
		}
		public override DocumentModel CreateNew(bool addDefaultLists, bool changeDefaultTableStyle) {
			return new ASPxDocumentModel(addDefaultLists, changeDefaultTableStyle, DocumentFormatsDependencies);
		}
		protected override ImageCacheBase CreateImageCache() {
			return new PersistentImageCache(this);
		}
	}
}
