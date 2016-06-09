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

using System.Collections.Generic;
using System.Drawing;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public interface IDocumentGroupInfo : IContentContainerInfo {
		DocumentGroup Group { get; }
		bool TryGetValue(Document document, out IDocumentInfo info);
		void Register(Document document);
		void Unregister(Document document);
	}
	public interface IDocumentGroupInfo<T> : IDocumentGroupInfo
		where T : DocumentGroup {
		new T Group { get; }
	}
	abstract class DocumentGroupInfo<T> : BaseContentContainerInfo, IDocumentGroupInfo<T>
		where T : DocumentGroup {
		IDictionary<Document, IDocumentInfo> documentInfosCore;
		public DocumentGroupInfo(WindowsUIView view, T group)
			: base(view, group) {
			documentInfosCore = new Dictionary<Document, IDocumentInfo>();
		}
		public T Group { 
			get { return Container as T; } 
		}
		DocumentGroup IDocumentGroupInfo.Group {
			get { return Container as DocumentGroup; }
		}
		public void Register(Document document) {
			IDocumentInfo documentInfo;
			if(!DocumentInfos.TryGetValue(document, out documentInfo)) {
				documentInfo = CreateDocumentInfo(document);
				documentInfo.SetParentInfo(this);
				OnBeforeDocumentInfoRegistered(documentInfo);
				LayoutHelper.Register(this, documentInfo);
				DocumentInfos.Add(document, documentInfo);
			}
		}
		public void Unregister(Document document) {
			IDocumentInfo documentInfo;
			if(DocumentInfos.TryGetValue(document, out documentInfo)) {
				DocumentInfos.Remove(document);
				OnBeforeDocumentInfoUnRegistered(documentInfo);
				LayoutHelper.Unregister(this, documentInfo);
				documentInfo.SetParentInfo(null);
				Ref.Dispose(ref documentInfo);
			}
		}
		protected virtual void OnBeforeDocumentInfoRegistered(IDocumentInfo info) { }
		protected virtual void OnBeforeDocumentInfoUnRegistered(IDocumentInfo info) { }
		protected virtual IDocumentInfo CreateDocumentInfo(Document document) {
			return new DocumentInfo(Owner, document);
		}
		protected IDictionary<Document, IDocumentInfo> DocumentInfos {
			get { return documentInfosCore; }
		}
		public bool TryGetValue(Document document, out IDocumentInfo info) {
			return DocumentInfos.TryGetValue(document, out info);
		}
		protected override void OnShown() {
			if(Owner.Manager != null && DocumentInfos.Count > 0)
				Owner.Manager.InvokePatchActiveChildren();
		}
	}
}
