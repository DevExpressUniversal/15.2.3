#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Templates;
namespace DevExpress.ExpressApp.Editors {
	public class DetailPropertyEditor : PropertyEditor, IComplexViewItem, IFrameContainer, ISupportViewEditMode {
		private IObjectSpace objectSpace;
		private XafApplication application;
		private NestedFrame frame;
		private ViewEditMode viewEditMode = ViewEditMode.View;
		private void InitializeFrame() {
			if(objectSpace != null) {
				if(frame == null) {
					frame = application.CreateNestedFrame(this, TemplateContext.NestedFrame);
				}
			}
			if(frame != null) {
				frame.SetControllersActive("PropertyEditor has ObjectSpace", objectSpace != null);
			}
		}
		private void SubscribeToEvents(ViewItem detailViewItem) {
			if(detailViewItem is PropertyEditor) {
				((PropertyEditor)detailViewItem).ControlValueChanged += new EventHandler(Editor_ControlValueChanged);
			}
		}
		private void UnsubscribeFromEvents(ViewItem detailViewItem) {
			if(detailViewItem is PropertyEditor) {
				((PropertyEditor)detailViewItem).ControlValueChanged -= new EventHandler(Editor_ControlValueChanged);
			}
		}
		private void SetDetailView(DetailView view) {
			if(DetailView != null) {
				DetailView.ItemsChanged -= new EventHandler<ViewItemsChangedEventArgs>(view_ItemsChanged);
				foreach(ViewItem detailViewItem in DetailView.Items) {
					UnsubscribeFromEvents(detailViewItem);
				}
			}
			if(view != null) {
				view.IsRoot = false;
				view.ItemsChanged += new EventHandler<ViewItemsChangedEventArgs>(view_ItemsChanged);
				foreach(ViewItem detailViewItem in view.Items) {
					SubscribeToEvents(detailViewItem);
				}
				view.AllowEdit.SetItemValue(GetType().FullName, AllowEdit);
				view.ViewEditMode = ViewEditMode;
			}
			frame.SetView(view, null);
		}
		private void UpdateViewEditMode() {
			if(DetailView != null) {
				DetailView.ViewEditMode = viewEditMode;
			}
		}
		private void view_ItemsChanged(object sender, ViewItemsChangedEventArgs e) {
			if(e.ChangedType == ViewItemsChangedType.Added) {
				SubscribeToEvents(e.Item);
			}
			else {
				UnsubscribeFromEvents(e.Item);
			}
		}
		private void Editor_ControlValueChanged(object sender, EventArgs e) {
			OnControlValueChanged();
		}		
		private Boolean NeedChangeDetailView(String newDetailViewId) {
			return ((DetailView == null) || (DetailView.Id != newDetailViewId));
		}
		private void DetailView_ControlsCreated(Object sender, EventArgs e) {
			DetailView.ControlsCreated -= new EventHandler(DetailView_ControlsCreated);
			IFrameTemplate frameTemplate = frame.Template;
			if(frameTemplate is ISupportStoreSettings) {
				((ISupportStoreSettings)frameTemplate).ReloadSettings();
			}
		}
		protected override object GetControlValueCore() {
			if(DetailView != null) {
				return DetailView.CurrentObject;
			}
			return null;
		}
		protected override object CreateControlCore() {
			InitializeFrame();
			frame.CreateTemplate();
			IFrameTemplate result = frame.Template;
			ReadValueCore();
			if(result is ISupportStoreSettings) {
				((ISupportStoreSettings)result).SetSettings(application.GetTemplateCustomizationModel(result));
				if(DetailView != null) {
					if(DetailView.IsControlCreated) {
						((ISupportStoreSettings)frame.Template).ReloadSettings();
					}
					else {
						DetailView.ControlsCreated += new EventHandler(DetailView_ControlsCreated);
					}
				}
			}			
			return result;
		}
		protected override void SaveModelCore() {
			base.SaveModelCore();
			if(frame != null) {
				frame.SaveModel();
			}
		}
		protected override void ReadValueCore() {
			Object propertyValue = PropertyValue;
			IModelDetailView modelDetailView = Model.View as IModelDetailView;
			if(modelDetailView != null) {
				if(NeedChangeDetailView(modelDetailView.Id)) {
					SetDetailView(application.CreateDetailView(objectSpace, modelDetailView, false));
				}
			}
			else {
				String viewId = "";
				if(propertyValue != null) {
					viewId = application.GetDetailViewId(propertyValue.GetType());
				}
				else {
					viewId = application.FindDetailViewId(Model.ModelMember.Type);
				}
				if(NeedChangeDetailView(viewId)) {
					DetailView detailView = null;
					if(!String.IsNullOrEmpty(viewId)) {
						detailView = application.CreateDetailView(objectSpace, viewId, false);
					}
					SetDetailView(detailView);
				}
			}
			if(DetailView != null) {
				DetailView.CurrentObject = propertyValue;
			}
		}
		protected override void Dispose(bool disposing) {
			try {
				if(disposing) {
					application = null;
					objectSpace = null;
					if(frame != null) {
						if(DetailView != null) {
							DetailView.ItemsChanged -= new EventHandler<ViewItemsChangedEventArgs>(view_ItemsChanged);
						}
						frame.Dispose();
						frame = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected override bool IsMemberSetterRequired() {
			return false;
		}
		protected override void OnAllowEditChanged() {
			base.OnAllowEditChanged();
			if(DetailView != null) {
				DetailView.AllowEdit.SetItemValue(GetType().FullName, AllowEdit);
			}
		}
		public DetailPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }
		public override void BreakLinksToControl(bool unwireEventsOnly) {
			if((frame != null) && (frame.View != null)) {
				frame.ClearTemplate();
				frame.View.BreakLinksToControls();
			}
			base.BreakLinksToControl(unwireEventsOnly);
		}
		public void Setup(IObjectSpace objectSpace, XafApplication application) {
			this.objectSpace = objectSpace;
			this.application = application;
		}
		public override void RefreshDataSource() {
			base.RefreshDataSource();
			if(DetailView != null) {
				DetailView.RefreshDataSource();
			}
		}
		public Frame Frame {
			get { return frame; }
		}
		public DetailView DetailView {
			get { return (frame != null) ? (DetailView)frame.View : null; }
		}
		public ViewEditMode ViewEditMode {
			get { return viewEditMode; }
			set {
				viewEditMode = value;
				UpdateViewEditMode();
			}
		}
		public override bool IsCaptionVisible {
			get { return false; }
		}
		void IFrameContainer.InitializeFrame() {
			InitializeFrame();
		}
	}
}
