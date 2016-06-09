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
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.SystemModule {
	public class CustomProcessListViewSelectedItemEventArgs : HandledEventArgs {
		private SimpleActionExecuteEventArgs innerArgs;
		public CustomProcessListViewSelectedItemEventArgs(SimpleActionExecuteEventArgs innerArgs) {
			this.innerArgs = innerArgs;
		}
		public SimpleActionExecuteEventArgs InnerArgs {
			get { return innerArgs; }
		}
	}
	public class CustomizeShowViewParametersEventArgs : EventArgs {
		private ShowViewParameters showViewParameters;
		public CustomizeShowViewParametersEventArgs(ShowViewParameters showViewParameters) {
			this.showViewParameters = showViewParameters;
		}
		public ShowViewParameters ShowViewParameters {
			get { return showViewParameters; }
		}
	}
	public class ListViewProcessCurrentObjectController : ViewController {
		public const String ListViewShowObjectActionId = "ListViewShowObject";
		private SimpleAction processCurrentObjectAction;
		private void processCurrentObjectAction_OnExecute(Object sender, SimpleActionExecuteEventArgs e) {
			CustomProcessListViewSelectedItemEventArgs customProcessArgs = new CustomProcessListViewSelectedItemEventArgs(e);
			if(CustomProcessSelectedItem != null) {
				CustomProcessSelectedItem(this, customProcessArgs);
			}
			if(!customProcessArgs.Handled) {
				ProcessCurrentObject(e);
			}
			if(CustomizeShowViewParameters != null) {
				CustomizeShowViewParameters(this, new CustomizeShowViewParametersEventArgs(e.ShowViewParameters));
			}
		}
		private void View_ProcessSelectedItem(Object sender, EventArgs e) {
			if(processCurrentObjectAction.Enabled && processCurrentObjectAction.Active) {
				processCurrentObjectAction.DoExecute();
			}
		}
		protected virtual void ProcessCurrentObject(SimpleActionExecuteEventArgs e) {
			if(Application.ShowDetailViewFrom(Frame)) {
				ShowObject(((ListView)View).CurrentObject, e.ShowViewParameters, Application, Frame, View);
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			((ListView)View).ProcessSelectedItem += new EventHandler(View_ProcessSelectedItem);
		}
		protected override void OnDeactivated() {
			((ListView)View).ProcessSelectedItem -= new EventHandler(View_ProcessSelectedItem);
			base.OnDeactivated();
		}
		public ListViewProcessCurrentObjectController()
			: base() {
			TypeOfView = typeof(ListView);
			processCurrentObjectAction = new SimpleAction(this, ListViewShowObjectActionId, "ListView");
			processCurrentObjectAction.Caption = "Open";
			processCurrentObjectAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
			processCurrentObjectAction.Execute += new SimpleActionExecuteEventHandler(processCurrentObjectAction_OnExecute);
			RegisterActions(processCurrentObjectAction);
		}
		public static void ShowObject(Object obj, ShowViewParameters showViewParameters, XafApplication application, Frame sourceFrame, View sourceView) {
			if(obj == null) {
				throw new ArgumentNullException("e.CurrentObject");
			}
			IObjectSpace objectSpace = application.GetObjectSpaceToShowDetailViewFrom(sourceFrame, sourceView.ObjectSpace.GetObjectType(obj));
			Object objectInTargetObjectSpace = null;
			if(objectSpace != sourceView.ObjectSpace) {
				if(sourceView.ObjectSpace.IsNewObject(obj) && !(objectSpace is INestedObjectSpace)) {
					throw new UserFriendlyException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.AnUnsavedObjectCannotBeShown));
				}
			}
			if(application.IsDelayedDetailViewDataLoadingEnabled) {
				DetailView detailView = application.CreateDetailView(objectSpace, application.FindDetailViewId(obj, sourceView), objectSpace != sourceView.ObjectSpace);
				DetailViewCurrentObjectInitializer detailViewCurrentObjectInitializer = new DetailViewCurrentObjectInitializer(sourceView, detailView, obj);
				detailView.keyMemberValueForPendingLoading = sourceView.ObjectSpace.GetKeyValueAsString(obj);
				showViewParameters.CreatedView = detailView;
			}
			else {
				objectInTargetObjectSpace = (objectSpace != sourceView.ObjectSpace) ? objectSpace.GetObject(obj) : obj;
				showViewParameters.CreatedView = application.CreateDetailView(objectSpace, objectInTargetObjectSpace, sourceView);
			}
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ListViewProcessCurrentObjectControllerProcessCurrentObjectAction")]
#endif
		public SimpleAction ProcessCurrentObjectAction {
			get { return processCurrentObjectAction; }
		}
		public event EventHandler<CustomProcessListViewSelectedItemEventArgs> CustomProcessSelectedItem;
		public event EventHandler<CustomizeShowViewParametersEventArgs> CustomizeShowViewParameters;
	}
	public class DetailViewCurrentObjectInitializer {
		private View sourceView;
		private DetailView detailView;
		private Object obj;
		private void detailView_Activated(Object sender, EventArgs e) {
			detailView.Activated -= new EventHandler(detailView_Activated);
			if(detailView.ObjectSpace != sourceView.ObjectSpace) {
				detailView.CurrentObject = detailView.ObjectSpace.GetObject(obj);
			}
			else {
				detailView.CurrentObject = obj;
			}
		}
		public DetailViewCurrentObjectInitializer(View sourceView, DetailView detailView, Object obj) {
			this.sourceView = sourceView;
			this.detailView = detailView;
			this.obj = obj;
			detailView.Activated += new EventHandler(detailView_Activated);
		}
	}
}
