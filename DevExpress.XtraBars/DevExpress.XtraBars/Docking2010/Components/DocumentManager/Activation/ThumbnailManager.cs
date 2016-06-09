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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Taskbar.Core;
using DevExpress.XtraBars.Docking2010.Base;
namespace DevExpress.XtraBars.Docking2010.Views {
	public interface IThumbnailManager : IDisposable {
		void UpdateThumbnails();
	}
	public interface IThumbnailManagerClient {
		IEnumerable<Control> Children { get; }
		object GetObject(Control control);
		event DocumentEventHandler TabbedThumbnailsChanged;
		event EventHandler UpdateTabbedThumbnails;		
	}
	interface IThumbnailViewClient { 
		void RedrawThumbnailChild(BaseDocument document);
	}
	class ThumbnailManagerProvider : IThumbnailManagerProvider {
		IThumbnailManagerClient client;
		public ThumbnailManagerProvider(IThumbnailManagerClient client) {
			this.client = client;
		}
		#region IThumbnailManagerProvider Members
		BaseDocument GetDocument(Control control) {
			return client.GetObject(control) as BaseDocument;
		}
		Form GetRootForm(BaseDocument document) {
			if(document == null) return null;
			if(document != null && document.IsFloating) {
				var floatForm = document.Control.FindForm();
				return (floatForm != null) && floatForm.IsHandleCreated ? floatForm : null;
			}
			DocumentManager manager = GetManager(document);
			var form = DocumentsHostContext.GetForm(manager);
			return (form != null) && form.IsHandleCreated ? form : null;
		}
		DocumentManager GetManager(BaseDocument document) {
			return (document != null) ? document.Manager : null;
		}
		void IThumbnailManagerProvider.Activate(Control control) {
			BaseDocument document = GetDocument(control);
			DocumentManager manager = GetManager(document);
			if(manager == null) return;
			manager.View.Controller.Activate(document);
		}
		void IThumbnailManagerProvider.Close(Control control) {
			BaseDocument document = GetDocument(control);
			DocumentManager manager = GetManager(document);
			if(manager == null) return;
			manager.View.Controller.Close(document);
		}
		Rectangle CalcScreenBoundsFloatDocument(BaseDocument document) {
			if(!(document.Form is FloatDocumentForm)) return document.Form.Bounds;
			Point clientScreenLocation = document.Form.PointToScreen(new Point(0, 0));
			return new Rectangle(clientScreenLocation, document.Form.ClientSize);
		}
		Rectangle IThumbnailManagerProvider.GetScreenBounds(Control control) {
			BaseDocument document = GetDocument(control);
			DocumentManager manager = GetManager(document);
			if(manager == null) return Rectangle.Empty;
			if(document.IsFloating) return CalcScreenBoundsFloatDocument(document);
			Rectangle bounds = manager.View.GetBounds(document);
			return new Rectangle(manager.ClientToScreen(bounds.Location), bounds.Size);
		}
		Form IThumbnailManagerProvider.GetRootForm(Control control) {
			BaseDocument document = GetDocument(control);
			return GetRootForm(document);
		}
		string IThumbnailManagerProvider.GetCaption(Control control) {
			BaseDocument document = GetDocument(control);
			if(document == null) return string.Empty;
			Form rootForm = GetRootForm(document);
			string rootFormCaption = rootForm == null ? null : rootForm.Text;
			return string.Format(document.Properties.ActualThumbnailCaptionFormat, rootFormCaption, document.Caption);
		}
		Icon IThumbnailManagerProvider.GetIcon(Control control) {
			BaseDocument document = GetDocument(control);
			Image image = GetDocumentImage(document);
			if(image == null) return null;
			using(Bitmap bitmap = new Bitmap(image)) {
				Icon icon = System.Drawing.Icon.FromHandle(bitmap.GetHicon());
				return icon;
			}
		}
		Image GetDocumentImage(BaseDocument document) {
			if(document == null) return null;
			if(document.Image != null)
				return document.Image;
			var manager = GetManager(document);
			if(manager == null) return null;
			return DevExpress.Utils.ImageCollection.GetImageListImage(manager.Images, document.ImageIndex);
		}
		void IThumbnailManagerProvider.Update(Control control) {
			BaseDocument document = GetDocument(control);
			if(document == null) return;
			DocumentManager manager = GetManager(document);
			if(manager == null) return;
			IThumbnailViewClient view = manager.View as IThumbnailViewClient;
			if(view == null) return;
			view.RedrawThumbnailChild(document);
		}
		#endregion
	}
	class ThumbnailManager : IThumbnailManager, ISharedRefTarget<IThumbnailManager> {
		List<Control> childControls;
		IThumbnailManagerProvider provider;
		IThumbnailManagerClient clientCore;
		DocumentManager managerCore;
		public ThumbnailManager(DocumentManager manager, IThumbnailManagerClient client) {
			this.childControls = new List<Control>();
			this.managerCore = manager;
			this.clientCore = client;
			this.provider = CreateThumbnailManagerProvider();
			Client.TabbedThumbnailsChanged += OnTabbedThumbnailsChanged;
			Client.UpdateTabbedThumbnails += OnUpdateTabbedThumbnails;
		}
		bool isDisposing;
		void IDisposable.Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				Client.TabbedThumbnailsChanged -= OnTabbedThumbnailsChanged;
				Client.UpdateTabbedThumbnails -= OnUpdateTabbedThumbnails;
				foreach(Control control in childControls)
					TaskbarHelper.RemoveThumbnailTab(control);
				childControls.Clear();
				this.clientCore = null;
				this.managerCore = null;
			}
			GC.SuppressFinalize(this);
		}
		void OnUpdateTabbedThumbnails(object sender, EventArgs e) {
			if(tabbedThumbnailsChanged > 0)
				UpdateThumbnails();
			TaskbarHelper.UpdateSettingsTabbedThumbnails();			
		}
		int tabbedThumbnailsChanged = 0;
		void OnTabbedThumbnailsChanged(object sender, DocumentEventArgs ea) {
			if(ea.Document.IsControlHandleCreated && !CanAddThumbnailTab(ea.Document.Control))
				RemoveThumbnailTab(ea.Document.Control);
			tabbedThumbnailsChanged++;
		}
		protected virtual IThumbnailManagerProvider CreateThumbnailManagerProvider() {
			return new ThumbnailManagerProvider(Client);
		}
		public DocumentManager Manager {
			get { return managerCore; }
		}
		bool ISharedRefTarget<IThumbnailManager>.CanShare(IThumbnailManager target) {
			ThumbnailManager targetThumbnailManager = target as ThumbnailManager;
			return (targetThumbnailManager != null) && Manager.IsShareActivationScope(targetThumbnailManager.Manager);
		}
		void ISharedRefTarget<IThumbnailManager>.Share(IThumbnailManager target) {
			ThumbnailManager targetThumbnailManager = target as ThumbnailManager;
			if(targetThumbnailManager != null) {
				childControls.AddRange(targetThumbnailManager.childControls.ToArray());
				targetThumbnailManager.childControls.Clear();
			}
		}
		public IThumbnailManagerClient Client {
			get { return clientCore; }
		}
		bool CanAddThumbnailTab(Control control) {
			if(provider == null) return false;
			return !childControls.Contains(control);
		}
		public void UpdateThumbnails() {
			List<Control> actualControls = new List<Control>();
			foreach(Control control in Client.Children) {
				Control form = provider.GetRootForm(control);
				if(form == null || form.Visible == false)
					continue;
				if(CanAddThumbnailTab(control))
					AddThumbnailTab(control);
				actualControls.Add(control);
			}
			Control[] children = childControls.ToArray();
			foreach(Control control in children) {
				if(actualControls.Contains(control)) continue;
				RemoveThumbnailTab(control);
			}
			tabbedThumbnailsChanged = 0;
		}
		void RemoveThumbnailTab(Control control) {
			childControls.Remove(control);
			TaskbarHelper.RemoveThumbnailTab(control);
		}
		void AddThumbnailTab(Control control) {
			TaskbarHelper.AddThumbnailTab(provider, control);
			childControls.Add(control);
		}
	}
}
