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
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	public class ASPxGridDetailFramesManager : IDisposable {
		private XafApplication application;
		private IModelDetailView detailViewModel;
		private Dictionary<object, DetailFrameInfo> framesInfoDictionary = new Dictionary<object, DetailFrameInfo>();
		public ASPxGridDetailFramesManager(XafApplication application, IModelDetailView detailViewModel) {
			this.detailViewModel = detailViewModel;
			this.application = application;
		}
		public virtual Frame GetDetailFrame(GridViewDetailRowTemplateContainer container) {
			object masterObject = GetCurrentObject(container);
			DetailFrameInfo detailFrameInfo;
			if(!framesInfoDictionary.TryGetValue(masterObject, out detailFrameInfo)) {
				IObjectSpace detailViewObjectSpace = application.CreateObjectSpace();
				object masterObjectInDetailViewObjectSpace = detailViewObjectSpace.GetObject(masterObject);
				View detailView = application.CreateDetailView(detailViewObjectSpace, detailViewModel, false, masterObjectInDetailViewObjectSpace);
				Frame detailFrame = application.CreateFrame(TemplateContext.NestedFrame);
				detailFrame.SetView(detailView);
				detailFrameInfo = new DetailFrameInfo();
				detailFrameInfo.FrameIndex = GetFrameIndex(container);
				detailFrameInfo.DetailFrame = detailFrame;
				framesInfoDictionary[masterObject] = detailFrameInfo;
			}
			else {
				detailFrameInfo.DetailFrame.SetTemplate(null);
			}
			detailFrameInfo.DetailFrame.CreateTemplate();
			return detailFrameInfo.DetailFrame;
		}
		public virtual IEnumerable<DetailFrameInfo> GetDetailsFramesInfo() {
			return framesInfoDictionary.Values;
		}
		public virtual void Dispose() {
			foreach(DetailFrameInfo frameInfo in framesInfoDictionary.Values) {
				frameInfo.Dispose();
			}
			framesInfoDictionary.Clear();
		}
		protected virtual object GetCurrentObject(GridViewDetailRowTemplateContainer container) {
			Guard.ArgumentNotNull(container, "templateContainer");
			return container.Grid.GetRow(container.VisibleIndex);
		}
		protected virtual int GetFrameIndex(GridViewDetailRowTemplateContainer container) {
			Guard.ArgumentNotNull(container, "templateContainer");
			return container.VisibleIndex;
		}
	}
	public class ASPxGridDetailRowTemplate : ITemplate {
		public ASPxGridDetailFramesManager DetailFrameManager { get; private set; }
		public bool CanMinimizeTemplate { get; set; }
		public ASPxGridDetailRowTemplate(ASPxGridDetailFramesManager manager, bool minimizeTemplate) {
			this.DetailFrameManager = manager;
			CanMinimizeTemplate = minimizeTemplate;
		}
		public void InstantiateIn(Control container) {
			Frame detailsFrame = DetailFrameManager.GetDetailFrame(container as GridViewDetailRowTemplateContainer);
			HideToolbar(detailsFrame);
			if(CanMinimizeTemplate)
				MinimizeTemplate(detailsFrame);
			Control detailsTemplateControl = detailsFrame.Template as Control;
			detailsTemplateControl.Unload += detailsTemplateControl_Unload;
			container.Controls.Add(detailsTemplateControl);
		}
		private void MinimizeTemplate(Frame detailsFrame) {
			DetailView detailView = detailsFrame.View as DetailView;
			IList<ListPropertyEditor> listPropertyEditors = detailView.GetItems<ListPropertyEditor>();
			foreach(var listPropertyEditor in listPropertyEditors) {
				listPropertyEditor.ControlCreated += listPropertyEditor_ControlCreated;
			}
		}
		private void listPropertyEditor_ControlCreated(object sender, EventArgs e) {
			ListPropertyEditor editor = sender as ListPropertyEditor;
			editor.ControlCreated -= listPropertyEditor_ControlCreated;
			SetupListPropertyEditor(editor);
		}
		private void SetupListPropertyEditor(ListPropertyEditor editor) {
			IList list = editor.PropertyValue as IList;
			if(list != null) {
				if(list.Count == 0)
					(editor.Control as Control).Visible = false;
			}
			Frame nestedFrame = editor.Frame;
			HideToolbar(nestedFrame);
			ASPxGridListEditor grid = editor.ListView.Editor as ASPxGridListEditor;
			if(grid != null) {
				grid.CanSelectRows = false;
				grid.DisplayActionColumns = false;
			}
		}
		private void detailsTemplateControl_Unload(object sender, EventArgs e) {
			IViewHolder viewHolder = sender as IViewHolder;
			if(viewHolder != null) {
				viewHolder.View.BreakLinksToControls();
			}
			IDisposable control = sender as IDisposable;
			if(control != null) {
				control.Dispose();
			}
		}
		private void HideToolbar(Frame frame) {
			if(frame != null && frame.Template is ISupportActionsToolbarVisibility) {
				((ISupportActionsToolbarVisibility)frame.Template).SetVisible(false);
			}
		}
	}
}
