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
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Editors {
	public class DashboardViewItem : ViewItem, IComplexViewItem, IFrameContainer {
		private IModelDashboardViewItem model;
		private XafApplication application;
		private NestedFrame frame;
		private View view = null;
		private String caption;
		private void InitializeFrame() {
			if(frame == null) {
				frame = application.CreateNestedFrame(this, TemplateContext.NestedFrame);
			}
		}
		private CriteriaOperator GetCriteriaOperator() {
			CriteriaOperator result = null;
			ObjectView objectView = view as ObjectView;
			if(objectView != null && !String.IsNullOrEmpty(model.Criteria)) {
				ITypeInfo typeInfo = objectView.ObjectTypeInfo;
				Guard.ArgumentNotNull(typeInfo, "typeInfo");
				CriteriaOperator criteriaOperator = objectView.ObjectSpace.ParseCriteria(model.Criteria);
				CriteriaOperator criteriaOperatorUpgraded = LocalizedCriteriaWrapper.UpgradeOldReadOnlyParameters(typeInfo.Type, criteriaOperator);
				FilterWithObjectsProcessor criteriaProcessor = new FilterWithObjectsProcessor(objectView.ObjectSpace, typeInfo, false);
				criteriaProcessor.Process(criteriaOperatorUpgraded, FilterWithObjectsProcessorMode.StringToObject);
				EnumPropertyValueCriteriaProcessor enumParametersProcessor = new EnumPropertyValueCriteriaProcessor(typeInfo);
				enumParametersProcessor.Process(criteriaOperatorUpgraded);
				result = criteriaOperatorUpgraded;
			}
			return result;
		}
		private void ApplyModel() {
			if(frame == null) {
				return;
			}
			ISupportStoreSettings storeSettingsManager = frame.Template as ISupportStoreSettings;
			if(storeSettingsManager != null) {
				storeSettingsManager.SetSettings(application.GetTemplateCustomizationModel(frame.Template));
				storeSettingsManager.ReloadSettings();
			}
			ISupportActionsToolbarVisibility visibilityManager = frame.Template as ISupportActionsToolbarVisibility;
			if(visibilityManager != null && model.ActionsToolbarVisibility != ActionsToolbarVisibility.Default) {
				visibilityManager.SetVisible(model.ActionsToolbarVisibility == ActionsToolbarVisibility.Show ? true : false);
			}
		}
		private void View_ControlsCreated(object sender, EventArgs e) {
			View.ControlsCreated -= new EventHandler(View_ControlsCreated);
			ApplyModel();
		}
		protected override Object CreateControlCore() {
			InitializeFrame();
			if(view == null) {
				view = application.CreateView(model.View);
				CriteriaOperator criteriaOperator = GetCriteriaOperator();
				if(!ReferenceEquals(criteriaOperator, null)) {
					if(view is ListView) {
						((ListView)view).CollectionSource.Criteria["DashboardViewCriteria"] = criteriaOperator;
					}
					else if(view is ObjectView) {
						IList objects = view.ObjectSpace.GetObjects(((ObjectView)view).ObjectTypeInfo.Type, criteriaOperator);
						if(objects.Count > 0) {
							view.CurrentObject = objects[0];
						}
					}
				}
				frame.SetView(view);
			}
			frame.CreateTemplate();
			if(View.IsControlCreated) {
				ApplyModel();
			}
			else {
				View.ControlsCreated += new EventHandler(View_ControlsCreated);
			}
			return frame.Template;
		}
		protected override void SaveModelCore() {
			base.SaveModelCore();
			if(frame != null) {
				frame.SaveModel();
			}
		}
		protected override void Dispose(Boolean disposing) {
			model = null;
			application = null;
			if(frame != null) {
				frame.Dispose();
				frame = null;
				view = null;
			}
			base.Dispose(disposing);
		}
		public DashboardViewItem(IModelDashboardViewItem model, Type objectType)
			: base(objectType, (model != null) ? model.Id : "") {
			this.model = model;
			if(model != null) {
				caption = model.Caption;
			}
		}
		public override void BreakLinksToControl(Boolean unwireEventsOnly) {
			base.BreakLinksToControl(unwireEventsOnly);
			if((frame != null) && (frame.View != null)) {
				frame.View.BreakLinksToControls();
				frame.ClearTemplate();
			}
		}
		public void Setup(IObjectSpace objectSpace, XafApplication application) {
			this.application = application;
		}
		public Frame Frame {
			get { return frame; }
		}
		public View InnerView {
			get { return view; }
		}
		public IModelDashboardViewItem Model {
			get { return model; }
		}
		public override String Caption {
			get { return caption; }
			set {
				caption = value;
				if(model != null) {
					model.Caption = value;
				}
			}
		}
		void IFrameContainer.InitializeFrame() {
			InitializeFrame();
		}
	}
}
