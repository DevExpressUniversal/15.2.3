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
using DevExpress.XtraPrinting;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using DevExpress.XtraPrinting.Native;
using System.Drawing;
using System.Drawing.Printing;
namespace DevExpress.DocumentView {
	public class DocumentBase : IDocument {
		IList<IPage> IDocument.Pages {
			get { return Pages; }
		}
		public event EventHandler AfterBuildPages;
		public void RaiseAfterBuildPages(EventArgs e) { if (AfterBuildPages != null) AfterBuildPages(this, e); }
		public event EventHandler BeforeBuildPages;
		public void RaiseBeforeBuildPages(EventArgs e) { if (BeforeBuildPages != null) BeforeBuildPages(this, e); }
		public event ExceptionEventHandler CreateDocumentException;
		public void RaiseCreateDocumentException(ExceptionEventArgs e) { if (CreateDocumentException != null) CreateDocumentException(this, e); }
		public event EventHandler Disposed;
		public void RaiseDisposed(EventArgs e) { if (Disposed != null) Disposed(this, e); }
		public event EventHandler DocumentChanged;
		public void RaiseDocumentChanged(EventArgs e) { if (DocumentChanged != null) DocumentChanged(this, e); }
		public event EventHandler PageBackgrChanged;
		public void RaisePageBackgrChanged(EventArgs e) { if (PageBackgrChanged != null) PageBackgrChanged(this, e); }
		ServiceContainer serviceContainer = new ServiceContainer();
		PageSettingsBase pageSettings = new PageSettingsBase(PaperKind.Letter, new Margins(100, 100, 100, 100), false);
		public virtual bool IsCreating {
			get { return false; }
		}
		public virtual bool IsEmpty {
			get { return Pages.Count == 0; }
		}
		public IPageSettings PageSettings {
			get { return pageSettings; }
		}
		public PageCollectionBase Pages {
			get;
			private set;
		}
		public DocumentBase() {
			Pages = new PageCollectionBase();
		}
		protected virtual void BeginDrawPages() {
		}
		protected virtual void EndDrawPages() {
		}
		void IDocument.BeforeDrawPages(object syncObj) {
			BeginDrawPages();
		}
		void IDocument.AfterDrawPages(object syncObj, int[] pageIndices) {
			EndDrawPages();
		}
		void IServiceContainer.AddService(Type serviceType, System.ComponentModel.Design.ServiceCreatorCallback callback, bool promote) {
			serviceContainer.AddService(serviceType, callback, promote);
		}
		void IServiceContainer.AddService(Type serviceType, System.ComponentModel.Design.ServiceCreatorCallback callback) {
			serviceContainer.AddService(serviceType, callback);
		}
		void IServiceContainer.AddService(Type serviceType, object serviceInstance, bool promote) {
			serviceContainer.AddService(serviceType, serviceInstance, promote);
		}
		void IServiceContainer.AddService(Type serviceType, object serviceInstance) {
			serviceContainer.AddService(serviceType, serviceInstance);
		}
		void IServiceContainer.RemoveService(Type serviceType, bool promote) {
			serviceContainer.RemoveService(serviceType, promote);
		}
		void IServiceContainer.RemoveService(Type serviceType) {
			serviceContainer.RemoveService(serviceType);
		}
		object IServiceProvider.GetService(Type serviceType) {
			return serviceContainer.GetService(serviceType);
		}
	}
	public class PageCollectionBase : Collection<IPage> {
		Dictionary<IPage, int> indexes = new Dictionary<IPage, int>();
		protected override void InsertItem(int index, IPage item) {
			((PageBase)item).Container = this;
			base.InsertItem(index, item);
			for (int i = index; i < Count; i++)
				indexes[this[i]] = i;
		}
		protected override void SetItem(int index, IPage item) {
			((PageBase)item).Container = this;
			((PageBase)this[index]).Container = null;
			indexes.Remove(this[index]);
			base.SetItem(index, item);
			indexes[item] = index;
		}
		protected override void ClearItems() {
			foreach (PageBase item in this)
				item.Container = null;
			indexes.Clear();
			base.ClearItems();
		}
		protected override void RemoveItem(int index) {
			((PageBase)this[index]).Container = null;
			indexes.Remove(this[index]);
			base.RemoveItem(index);
			for (int i = index; i < Count; i++)
				indexes[this[i]] = i;
		}
		internal int IndexOfFast(IPage page) {
			return indexes[page];
		}
	}
	public class PageBase : IPage {
		RectangleF IPage.ApplyMargins(RectangleF pageRect) {
			return pageData.GetMarginRect(pageRect);
		}
		void IPage.Draw(Graphics gr, PointF position) {
			callback(this, gr, position);
		}
		RectangleF IPage.UsefulPageRectF {
			get { return UsefulPageRectF; }
		}
		ReadonlyPageData pageData;
		PageDrawingCallback callback;
		internal PageCollectionBase Container { get; set; }
		protected virtual RectangleF UsefulPageRectF { get { return pageData.UsefulPageRectF; } }
		public PageBase(PageDrawingCallback callback) :
			this(PaperKind.Letter, new Margins(100, 100, 100, 100), false, callback) {
		}
		public PageBase(PaperKind paperKind, Margins margins, bool landscape, PageDrawingCallback callback) :
			this(paperKind, Size.Empty, margins, landscape, callback) {
		}
		public PageBase(PaperKind paperKind, Size pageSize, Margins margins, bool landscape, PageDrawingCallback callback) {
			pageData = new ReadonlyPageData(margins, new Margins(0, 0, 0, 0), paperKind, pageSize, landscape);
			this.callback = callback;
		}
		public int Index {
			get { return Container != null ? Container.IndexOfFast(this) : -1; }
		}
		public SizeF PageSize {
			get { return pageData.PageSize; }
		}
	}
	public delegate void PageDrawingCallback(IPage page, Graphics graphics, PointF position);
	public class PageSettingsBase : IPageSettings {
		ReadonlyPageData pageData;
		public PageSettingsBase(PaperKind paperKind, Margins margins, bool landscape) {
			pageData = new ReadonlyPageData(margins, new Margins(0, 0, 0, 0), paperKind, landscape);
		}
		public int BottomMargin {
			get {
				return pageData.Margins.Bottom;
			}
			set {
				pageData.MarginsF.Bottom = MarginsF.FromHundredths(value);
			}
		}
		public int LeftMargin {
			get {
				return pageData.Margins.Left;
			}
			set {
				pageData.MarginsF.Left = MarginsF.FromHundredths(value);
			}
		}
		public int RightMargin {
			get {
				return pageData.Margins.Right;
			}
			set {
				pageData.MarginsF.Left = MarginsF.FromHundredths(value);
			}
		}
		public int TopMargin {
			get {
				return pageData.Margins.Top;
			}
			set {
				pageData.MarginsF.Top = MarginsF.FromHundredths(value);
			}
		}
		public SizeF PageSize {
			get { return pageData.PageSize; }
		}
	}
}
