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

using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public abstract class LoadHeadersFootersCommand<TObject, TIndex> : WebRichEditLoadModelCommandBase
		where TObject : SectionHeaderFooterBase
		where TIndex : struct, IConvertToInt<TIndex> {
		public LoadHeadersFootersCommand(WebRichEditLoadModelCommandBase parentCommand, DocumentModel documentModel)
			: base(parentCommand, new Hashtable(), documentModel) { }
		protected override bool IsEnabled() { return true; }
		protected override void FillHashtable(Hashtable result) {
			var arrayList = new ArrayList();
			foreach (var item in GetContainer()) {
				var itemHt = new Hashtable();
				itemHt["subDocumentId"] = LoadAdditionalPieceTableOnClient(item.PieceTable);
				itemHt["type"] = (int)item.Type;
				arrayList.Add(itemHt);
			}
			result["cache"] = arrayList;
		}
		protected abstract HeaderFooterCollectionBase<TObject, TIndex> GetContainer();
	}
	public class LoadHeadersCommand : LoadHeadersFootersCommand<SectionHeader, HeaderIndex> {
		public LoadHeadersCommand(WebRichEditLoadModelCommandBase parentCommand, DocumentModel documentModel)
			: base(parentCommand, documentModel) { }
		public override CommandType Type { get { return CommandType.LoadHeaders; } }
		protected override HeaderFooterCollectionBase<SectionHeader, HeaderIndex> GetContainer() {
			return DocumentModel.Headers;
		}
	}
	public class LoadFootersCommand : LoadHeadersFootersCommand<SectionFooter, FooterIndex> {
		public LoadFootersCommand(WebRichEditLoadModelCommandBase parentCommand, DocumentModel documentModel)
			: base(parentCommand, documentModel) { }
		public override CommandType Type { get { return CommandType.LoadFooters; } }
		protected override HeaderFooterCollectionBase<SectionFooter, FooterIndex> GetContainer() {
			return DocumentModel.Footers;
		}
	}
}
