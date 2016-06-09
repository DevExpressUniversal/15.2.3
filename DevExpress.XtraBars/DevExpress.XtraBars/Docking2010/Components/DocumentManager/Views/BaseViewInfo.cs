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
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.Docking2010.Base;
namespace DevExpress.XtraBars.Docking2010.Views {
	public abstract class BaseViewInfo {
		BaseView viewCore;
		IDockingAdornerInfo emptyViewAdornerInfoCore;
		IDictionary<BaseDocument, IFloatDocumentInfo> floatDocumentInfosCore;
		IDictionary<BaseDocument, IFloatPanelInfo> floatPanelInfosCore;
		public BaseView View {
			get { return viewCore; }
		}
		public BaseViewInfo(BaseView view) {
			AssertionException.IsNotNull(view);
			viewCore = view;
			paintAppearanceCore = new DevExpress.Utils.FrozenAppearance();
			emptyViewAdornerInfoCore = CreateEmptyViewAdornerInfo();
			floatDocumentInfosCore = new Dictionary<BaseDocument, IFloatDocumentInfo>();
			floatPanelInfosCore = new Dictionary<BaseDocument, IFloatPanelInfo>();
		}
		public IDockingAdornerInfo EmptyViewAdornerInfo {
			get { return UseEmptyViewAdorner() ? emptyViewAdornerInfoCore : null; }
		}
		protected virtual bool UseEmptyViewAdorner() {
			return View.IsEmpty;
		}
		protected internal IDictionary<BaseDocument, IFloatDocumentInfo> FloatDocumentInfos {
			get { return floatDocumentInfosCore; }
		}
		protected internal IDictionary<BaseDocument, IFloatPanelInfo> FloatPanelInfos {
			get { return floatPanelInfosCore; }
		}
		public IEnumerable<IFloatDocumentInfo> GetFloatDocumentInfos() {
			return FloatDocumentInfos.Values;
		}
		protected abstract IDockingAdornerInfo CreateEmptyViewAdornerInfo();
		protected virtual IFloatDocumentInfo CreateFloatDocumentInfo(BaseDocument document) {
			return new FloatDocumentInfo(View, document);
		}
		protected virtual IFloatPanelInfo CreateFloatPanelInfo(BaseDocument document) {
			return new FloatPanelInfo(View, document);
		}
		public void RegisterFloatDocumentInfo(BaseDocument document) {
			IFloatDocumentInfo info = CreateFloatDocumentInfo(document);
			FloatDocumentInfos.Add(document, info);
		}
		public void UnregisterFloatDocumentInfo(BaseDocument document) {
			IFloatDocumentInfo info;
			if(FloatDocumentInfos.TryGetValue(document, out info)) {
				FloatDocumentInfos.Remove(document);
				Ref.Dispose(ref info);
			}
		}
		public void RegisterFloatPanelInfo(BaseDocument document) {
			IFloatPanelInfo info = CreateFloatPanelInfo(document);
			FloatPanelInfos.Add(document, info);
		}
		public void UnregisterFloatPanelInfo(BaseDocument document) {
			IFloatPanelInfo info;
			if(FloatPanelInfos.TryGetValue(document, out info)) {
				FloatPanelInfos.Remove(document);
				Ref.Dispose(ref info);
			}
		}
		bool isReadyCore;
		public bool IsReady {
			get { return isReadyCore; }
		}
		Rectangle boundsCore;
		public Rectangle Bounds {
			get { return boundsCore; }
		}
		Rectangle[] visibleMdiChildrenCore;
		public Rectangle[] VisibleMdiChildren {
			get { return visibleMdiChildrenCore; }
		}
		DevExpress.Utils.AppearanceObject paintAppearanceCore;
		public DevExpress.Utils.AppearanceObject PaintAppearance {
			get { return paintAppearanceCore; }
		}
		bool appearancesDirty = true;
		public void SetAppearanceDirty() {
			appearancesDirty = true;
		}
		public void UpdateAppearances() {
			if(appearancesDirty) {
				UpdateAppearancesCore();
				appearancesDirty = false;
			}
		}
		protected virtual void UpdateAppearancesCore() {
			DevExpress.Utils.AppearanceHelper.Combine(PaintAppearance,
				View.Appearance, (View.Appearances != null) ? View.Appearances.View : null);
			if(PaintAppearance.BackColor.IsEmpty)
				PaintAppearance.BackColor = View.GetBackColor();
		}
		public void SetDirty() {
			isReadyCore = false;
		}
		public void Calc(Graphics g, Rectangle bounds) {
			if(IsReady) return;
			ClearCore();
			boundsCore = bounds;
			visibleMdiChildrenCore = CalculateCore(g, bounds);
			isReadyCore = true;
		}
		protected virtual void ClearCore() { }
		protected abstract Rectangle[] CalculateCore(Graphics g, Rectangle bounds);
		protected internal abstract Point GetFloatLocation(BaseDocument document);
		protected internal virtual Rectangle GetFloatingBounds(Rectangle bounds) { return bounds; }
		protected internal virtual Rectangle GetBounds(BaseDocument document) { return Bounds; }
		protected internal virtual void OnMouseDown(MouseEventArgs e) { }
		protected internal virtual void OnMouseMove(MouseEventArgs e) { }
		protected internal virtual void OnMouseUp(MouseEventArgs e) { }
	}
	public abstract class BaseElementInfo : IBaseElementInfo {
		Rectangle boundsCore;
		bool isDisposingCore;
		bool isVisibleCore;
		BaseView ownerCore;
		public BaseElementInfo(BaseView owner) {
			ownerCore = owner;
		}
		public void Dispose() {
			if(!isDisposingCore) {
				isDisposingCore = true;
				ResetStyleCore();
				OnDispose();
				ownerCore = null;
			}
			GC.SuppressFinalize(this);
		}
		public Rectangle Bounds {
			get { return boundsCore; }
			protected set { boundsCore = value; }
		}
		public BaseView Owner {
			get { return ownerCore; }
		}
		public bool IsDisposing {
			get { return isDisposingCore; }
		}
		public bool IsVisible {
			get { return isVisibleCore; }
		}
		public abstract Type GetUIElementKey();
		#region IBaseElementInfo Members
		void IBaseElementInfo.Calc(Graphics g, Rectangle bounds) {
			if(IsDisposing) return;
			if(boundsCore.IsEmpty && boundsCore != bounds)
				OnShown();
			boundsCore = bounds;
			CalcCore(g, bounds);
			isVisibleCore = CalcIsVisible();
		}
		void IBaseElementInfo.Draw(GraphicsCache cache) {
			if(IsDisposing) return;
			if(IsVisible && !Bounds.IsEmpty)
				DrawCore(cache);
		}
		void IBaseElementInfo.UpdateStyle() {
			ResetStyleCore();
			UpdateStyleCore();
		}
		void IBaseElementInfo.ResetStyle() {
			ResetStyleCore();
		}
		#endregion IBaseElementInfo Members
		protected virtual void OnDispose() { }
		protected virtual void OnShown() { }
		protected virtual void CalcCore(Graphics g, Rectangle bounds) { }
		protected virtual bool CalcIsVisible() { return true; }
		protected virtual void DrawCore(GraphicsCache cache) { }
		protected virtual void UpdateStyleCore() { }
		protected virtual void ResetStyleCore() { }
	}
}
