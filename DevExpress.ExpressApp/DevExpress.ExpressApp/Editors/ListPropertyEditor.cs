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
using System.ComponentModel;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Editors {
	public class ListPropertyEditor : PropertyEditor, IComplexViewItem, IFrameContainer {
		public const String PropertyCollectionIsNotReadOnly = "PropertyCollectionIsNotReadOnly";
		private NestedFrame frame;
		private XafApplication application;
		private IObjectSpace objectSpace;
		private ListViewProcessCurrentObjectController controller;
		private Boolean isRefreshing;
		private void InitializeFrame() {
			if(objectSpace != null) {
				if(frame == null) {
					frame = application.CreateNestedFrame(this, TemplateContext.NestedFrame);
					SetupLinkUnlinkController();
				}
				if(ListView == null) {
					ListView listView = CreateListView();
					frame.SetView(listView);
				}
				controller = Frame.GetController<ListViewProcessCurrentObjectController>();
				if(controller != null) {
					controller.CustomizeShowViewParameters -= new EventHandler<CustomizeShowViewParametersEventArgs>(controller_ProcessSelectedItemCore);
					controller.CustomizeShowViewParameters += new EventHandler<CustomizeShowViewParametersEventArgs>(controller_ProcessSelectedItemCore);
				}
				frame.SetControllersActive("PropertyEditor has ObjectSpace", objectSpace != null);
			}
			else {
				if(frame != null) {
					frame.SetControllersActive("PropertyEditor has ObjectSpace", objectSpace != null);
				}
				if(ListView != null) {
					ListView.CurrentObject = null;
				}
			}
		}
		private void controller_ProcessSelectedItemCore(Object sender, CustomizeShowViewParametersEventArgs e) {
			if(e.ShowViewParameters.CreatedView != null) {
				e.ShowViewParameters.CreatedView.AllowEdit[PropertyCollectionIsNotReadOnly] = AllowEdit;
				e.ShowViewParameters.CreatedView.AllowDelete[PropertyCollectionIsNotReadOnly] = AllowEdit;
				e.ShowViewParameters.CreatedView.AllowNew[PropertyCollectionIsNotReadOnly] = AllowEdit;
			}
		}
		private void SetupLinkUnlinkController() {
			LinkUnlinkController linkUnlinkController = frame.GetController<LinkUnlinkController>();
			if(linkUnlinkController != null) {
				linkUnlinkController.SetListPropertyEditorInfo(Model);
			}
		}
		private void ListView_ControlsCreated(Object sender, EventArgs e) {
			ListView.ControlsCreated -= new EventHandler(ListView_ControlsCreated);
			IFrameTemplate frameTemplate = frame.Template;
			if(frameTemplate is ISupportStoreSettings) {
				((ISupportStoreSettings)frameTemplate).ReloadSettings();
			}
		}
		private void UpdateEditViewAllowEdit(DetailView detailView) {
			if(detailView != null) {
				detailView.AllowEdit.SetItemValue(GetType().FullName, AllowEdit);
			}
		}
		private void listView_EditViewCreated(Object sender, DetailViewCreatedEventArgs e) {
			UpdateEditViewAllowEdit(e.View);
		}
		protected virtual ListView CreateListView() {
			ListView listView = null;
			String listViewID = "";
			if(Model != null) {
				IModelView view = Model.View;
				if(view != null) {
					listViewID = view.Id;
				}
			}
			PropertyCollectionSource collectionSource = application.CreatePropertyCollectionSource(objectSpace, ObjectType, CurrentObject, MemberInfo, listViewID);
			if(collectionSource.ObjectTypeInfo == null) {
				collectionSource.Dispose();
				if(MemberInfo.FindAttribute<ElementTypePropertyAttribute>() == null) {
					throw new Exception(String.Format(
						"Unable to recognize the list element type for the {0} member of the {1} type as a registered domain component type. So a ListView can't be created for the ListPropertyEditor, which is assigned to this member. " +
						"This error can be caused by incorrect application model settings or by an incorrect type set in the PropertyEditorTypeAttribute applied to ListPropertyEditor descendants. " +
						"To avoid this error, use the ListPropertyEditor only with members that match the above condition.",
						MemberInfo.Name, ObjectType.FullName));
				}
			}
			else {
				if(String.IsNullOrEmpty(listViewID)) {
					listViewID = application.GetListViewId(collectionSource.ObjectTypeInfo.Type);
				}
				listView = application.CreateListView(listViewID, collectionSource, false);
				listView.Caption = Caption;
				listView.IsRoot = false;
				listView.AllowEdit.SetItemValue(GetType().FullName, AllowEdit);
				listView.AllowDelete.SetItemValue(GetType().FullName, AllowEdit);
				listView.AllowNew.SetItemValue(GetType().FullName, AllowEdit);
				listView.AllowLink.SetItemValue(GetType().FullName, AllowEdit);
				listView.AllowUnlink.SetItemValue(GetType().FullName, AllowEdit);
				UpdateEditViewAllowEdit(listView.EditView);
				listView.EditViewCreated += new EventHandler<DetailViewCreatedEventArgs>(listView_EditViewCreated);
			}
			return listView;
		}
		protected override Boolean CanReadValue() {
			return (ListView != null);
		}
		protected override Boolean IsMemberSetterRequired() {
			return false;
		}
		protected override Object CreateControlCore() {
			InitializeFrame();
			frame.CreateTemplate();
			if(ListView != null) {
				if(frame.Template is ISupportStoreSettings) {
					((ISupportStoreSettings)frame.Template).SetSettings(application.GetTemplateCustomizationModel(frame.Template));
					if(ListView.IsControlCreated) {
						((ISupportStoreSettings)frame.Template).ReloadSettings();
					}
					else {
						ListView.ControlsCreated += new EventHandler(ListView_ControlsCreated);
					}
				}
				ListView.Caption = Caption;
			}
			return frame.Template;
		}
		protected override void OnAllowEditChanged() {
			base.OnAllowEditChanged();
			if(ListView != null) {
				ListView.AllowEdit.SetItemValue(GetType().FullName, AllowEdit);
				ListView.AllowNew.SetItemValue(GetType().FullName, AllowEdit);
				ListView.AllowDelete.SetItemValue(GetType().FullName, AllowEdit);
				ListView.AllowLink.SetItemValue(GetType().FullName, AllowEdit);
				ListView.AllowUnlink.SetItemValue(GetType().FullName, AllowEdit);
				UpdateEditViewAllowEdit(ListView.EditView);
			}
		}
		protected override void OnControlCreated() {
			base.OnControlCreated();
			UpdateEditorState();
		}
		protected override void ReadValueCore() {
			Tracing.Tracer.LogVerboseValue("ListView", ListView);
			if(ListView != null) {
				Tracing.Tracer.LogVerboseValue("ListView.CollectionSource", ListView.CollectionSource);
				if(ListView.CollectionSource != null) {
					Tracing.Tracer.LogVerboseValue("ListView.CollectionSource.List", ListView.CollectionSource.List);
				}
				ISupportUpdate editorUpdatable = ListView.Editor as ISupportUpdate;
				try {
					if(editorUpdatable != null) {
						editorUpdatable.BeginUpdate();
					}
					PropertyCollectionSource propertyCollectionSource = ListView.CollectionSource as PropertyCollectionSource;
					if(propertyCollectionSource != null) {
						if(propertyCollectionSource.MasterObject != CurrentObject) {
							propertyCollectionSource.MasterObject = CurrentObject;
						}
						else if(isRefreshing) {
							propertyCollectionSource.ResetCollection();
						}
					}
					ListView.Refresh();
				}
				finally {
					if(editorUpdatable != null) {
						editorUpdatable.EndUpdate();
					}
				}
			}
		}
		protected override Object GetControlValueCore() {
			if(ListView != null && ListView.CollectionSource != null) {
				return ListView.CollectionSource.List;
			}
			return null;
		}
		protected override void SaveModelCore() {
			if(frame != null) {
				frame.SaveModel();
			}
		}
		protected override void OnCurrentObjectChanged() {
			base.OnCurrentObjectChanged();
			if((ListView == null) && (frame != null)) {
				RecreateView();
			}
		}
		protected override void Dispose(Boolean disposing) {
			try {
				if(disposing) {
					if(frame != null) {
						frame.Dispose();
						frame = null;
					}
					if(controller != null) {
						controller.CustomizeShowViewParameters -= new EventHandler<CustomizeShowViewParametersEventArgs>(controller_ProcessSelectedItemCore);
						controller = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		public ListPropertyEditor(Type objectType, IModelMemberViewItem model)
			: base(objectType, model) {
		}
		public void Setup(IObjectSpace objectSpace, XafApplication application) {
			CheckIsDisposed();
			this.application = application;
			this.objectSpace = objectSpace;
		}
		public override void BreakLinksToControl(Boolean unwireEventsOnly) {
			if(!unwireEventsOnly && frame != null) {
				frame.ClearTemplate();
				if(ListView != null) {
					ListView.BreakLinksToControls();
				}
			}
			base.BreakLinksToControl(unwireEventsOnly);
		}
		public override void RefreshDataSource() {
			base.RefreshDataSource();
			if(ListView != null) {
				ListView.RefreshDataSource();
			}
		}
		public override void Refresh() {
			isRefreshing = true;
			try {
				base.Refresh();
			}
			finally {
				isRefreshing = false;
			}
		}
		public virtual void RecreateView() {
			if((frame != null) && (frame.View != null)) {
				frame.SetView(CreateListView());
			}
		}
		public override Boolean IsCaptionVisible {
			get { return false; }
		}
		public ListView ListView {
			get { return (frame != null) ? (ListView)frame.View : null; }
		}
		public Frame Frame {
			get { return frame; }
		}
		void IFrameContainer.InitializeFrame() {
			InitializeFrame();
		}
		protected internal static Boolean IsMemberListPropertyEditorCompatible(IModelMember modelMember) { 
			if(modelMember.MemberInfo != null) {
				DevExpress.ExpressApp.DC.IMemberInfo memberInfo = modelMember.MemberInfo;
				return memberInfo.ListElementTypeInfo != null ? modelMember.Application.BOModel.GetClass(memberInfo.ListElementType) != null : false;
			}
			return false;
		}
		#region Only for tests.
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void Setup(IObjectSpace objectSpace, XafApplication application, ListView listView) {
			this.application = application;
			frame = application.CreateNestedFrame(this, TemplateContext.NestedFrame);
			SetupLinkUnlinkController();
			this.objectSpace = objectSpace;
			frame.SetView(listView, null);
		}
		#endregion
	}
}
