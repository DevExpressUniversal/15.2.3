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
using System.Collections.Generic;
using System.Data.Common;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.ScriptRecorder {
	public class ScriptRecorderControllerBase : Controller {
		private List<IScriptRecorderControlListener> listeners;
		private SimpleAction saveScriptInShowScriptView;
		private SimpleAction startRecord;
		private SimpleAction pauseRecord;
		private SimpleAction showScript;
		private SimpleAction saveScript;
		public const string PauseAndStartRecordFlag = "PauseAndStartRecord";
		private const string DebuggerIsAttachedFlag = "DebuggerIsAttached";
		private const string ScriptRecorderEnabledFlag = "ScriptRecorderEnabled";
		private const string ShowScriptViewFlag = "ShowScriptView";
		private const string ShowScript_ExecuteFlag = "ShowScript_Execute";
		private const string FrameIsRoot = "FrameIsRoot";
		private const string pauseImageName = "Action_PauseRecording";
		private const string startImageName = "Action_ResumeRecording";
		public const string ActionContainerName = "EasyTestRecorder";
		private static bool? scriptRecorderEnabled;
		public ScriptRecorderControllerBase() {
			pauseRecord = new SimpleAction();
			pauseRecord.ImageName = pauseImageName;
			pauseRecord.Id = "PauseRecording";
			pauseRecord.Caption = "Pause Recording";
			pauseRecord.Category = ActionContainerName;
			pauseRecord.Execute += new SimpleActionExecuteEventHandler(PauseAndStartRecord_Execute);
			startRecord = new SimpleAction();
			startRecord.ImageName = startImageName;
			startRecord.Id = "ResumeRecording";
			startRecord.Caption = "Resume Recording";
			startRecord.Category = ActionContainerName;
			startRecord.Execute += new SimpleActionExecuteEventHandler(PauseAndStartRecord_Execute);
			showScript = new SimpleAction();
			showScript.Id = "ShowScript";
			showScript.ImageName = "Action_ShowScript";
			showScript.Caption = "Show Script";
			showScript.Category = ActionContainerName;
			showScript.Execute += new SimpleActionExecuteEventHandler(ShowScript_Execute);
			saveScript = new SimpleAction();
			saveScript.Id = "SaveScript";
			saveScript.ImageName = "Action_SaveScript";
			saveScript.Caption = "Save Script";
			saveScript.Category = ActionContainerName;
			saveScript.Execute += new SimpleActionExecuteEventHandler(saveScript_Execute);
			saveScriptInShowScriptView = new SimpleAction(this, "SaveScriptForDialogWindow", DialogController.DialogActionContainerName);
			saveScriptInShowScriptView.ImageName = "Action_SaveScript";
			saveScriptInShowScriptView.Caption = "Save Script";
			saveScriptInShowScriptView.Active.SetItemValue("ShowScriptView", false);
			saveScriptInShowScriptView.Execute += new SimpleActionExecuteEventHandler(SaveScriptInShowScriptView_Execute);
			ActionBase[] actions = new ActionBase[] { pauseRecord, startRecord, showScript, saveScript };
			RegisterActions(actions);
		}
		private void PauseAndStartRecord_Execute(object sender, SimpleActionExecuteEventArgs e) {
			bool currentValue = false;
			if(WriteLocked.Contains(PauseAndStartRecordFlag)) {
				currentValue = WriteLocked[PauseAndStartRecordFlag];
			}
			WriteLocked.SetItemValue(PauseAndStartRecordFlag, !currentValue);
			UpdateActionState();
		}
		private void ShowScript_Execute(object sender, SimpleActionExecuteEventArgs e) {
			Script script = new Script();
			script.ScriptLog = Logger.Instance.Script.ScriptLog;
			e.ShowViewParameters.CreatedView = Application.CreateDetailView(new NonPersistentObjectSpace(Application.TypesInfo), script, null);
			e.ShowViewParameters.CreatedView.AllowEdit.SetItemValue("EditScript", false);
			e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
			e.ShowViewParameters.Context = TemplateContext.PopupWindow;
			DialogController dialogController = Application.CreateController<DialogController>();
			dialogController.CancelAction.Active.SetItemValue("ShowScript", false);
			e.ShowViewParameters.Controllers.Add(dialogController);
		}
		private void saveScript_Execute(object sender, SimpleActionExecuteEventArgs e) {
			SaveScript(Logger.Instance.Script.ScriptLog);
		}
		private void SaveScriptInShowScriptView_Execute(object sender, SimpleActionExecuteEventArgs e) {
			SaveScript(((Script)e.CurrentObject).ScriptLog);
		}
		private void LockWrite_ResultValueChanged(object sender, DevExpress.ExpressApp.Utils.BoolValueChangedEventArgs e) {
			UpdateActionState();
			ListenersSetActive();
		}
		private void ListenersSetActive() {
			if(listeners != null) {
				foreach(IScriptRecorderControlListener listener in listeners) {
					listener.SetActive(WriteLocked);
				}
			}
		}
		private bool CurrentViewShowScriptView {
			get {
				return (Frame != null) && (Frame.View is ObjectView) &&
					(((ObjectView)Frame.View).ObjectTypeInfo != null) && ((ObjectView)Frame.View).ObjectTypeInfo.Type.IsAssignableFrom(typeof(Script));
			}
		}
		private ScriptRecorderActionsListenerBase CreateActionsListenerCore() {
			ScriptRecorderActionsListenerBase result = null;
			if(Frame.View != null) {
				result = CreateActionsListener(CollectAllActions);
			}
			else {
				if(Frame.Application != null) {
					IModelApplicationNavigationItems rootNavigationItems = Frame.Application.Model.Application as IModelApplicationNavigationItems;
					if(rootNavigationItems != null) {
						IModelRootNavigationItems navigationItems = ((IModelApplicationNavigationItems)Frame.Application.Model.Application).NavigationItems;
						if(navigationItems.StartupNavigationItem == null) {
							ShowNavigationItemController navigationController = Frame.GetController<ShowNavigationItemController>();
							if(navigationController != null) {
								result = CreateActionsListener(new List<ActionBase>(navigationController.Actions));
							}
						}
					}
				}
			}
			return result;
		}
		public static BoolList WriteLocked {
			get {
				IValueManager<BoolList> manager = ValueManager.GetValueManager<BoolList>("ScriptRecorder_isWriteLockedBoolList");
				if(manager.Value == null) {
					manager.Value = new BoolList(false, BoolListOperatorType.Or);
				}
				return manager.Value;
			}
		}
		protected virtual void SaveScript(string script) { }
		protected void Frame_ViewChanged(object sender, EventArgs e) {
			SetActive();
			UpdateActionState();
			DisposeListeners();
			CreateListeners();
		}
		protected void UpdateActionState() {
			saveScriptInShowScriptView.Active.SetItemValue(ShowScriptViewFlag, CurrentViewShowScriptView);
			pauseRecord.Active.SetItemValue("IsWriteLocked", !WriteLocked);
			startRecord.Active.SetItemValue("IsWriteLocked", WriteLocked);
		}
		protected override void OnActivated() {
			base.OnActivated();
			Frame.ViewChanged += new EventHandler<ViewChangedEventArgs>(Frame_ViewChanged);
			WriteLocked.ResultValueChanged += new EventHandler<BoolValueChangedEventArgs>(LockWrite_ResultValueChanged);
			SetActive();
			CreateListeners();
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			Frame.ViewChanged -= new EventHandler<ViewChangedEventArgs>(Frame_ViewChanged);
			if(WriteLocked != null) { 
				WriteLocked.ResultValueChanged -= new EventHandler<BoolValueChangedEventArgs>(LockWrite_ResultValueChanged);
			}
			DisposeListeners();
			listeners = null;
		}
		protected virtual void SetActive() {
			Active.BeginUpdate();
			if(PropertyEditorListenersFactory == null) {
				Active.SetItemValue("PropertyEditorListenersFactoryIsNotImplemented", false);
			}
			if(ScriptRecorderEnabled == null) {
				Active.SetItemValue(DebuggerIsAttachedFlag, System.Diagnostics.Debugger.IsAttached);
			}
			else {
				if(Active.Contains(DebuggerIsAttachedFlag)) {
					Active.RemoveItem(DebuggerIsAttachedFlag);
				}
				Active.SetItemValue(ScriptRecorderEnabledFlag, ScriptRecorderEnabled.Value);
			}
			Active.EndUpdate();
			UpdateActionState();
		}
		protected void DisposeListeners() {
			if(listeners != null) {
				foreach(IDisposable disposedObject in listeners) {
					disposedObject.Dispose();
				}
			}
			listeners = new List<IScriptRecorderControlListener>();
		}
		protected virtual List<ActionBase> CollectAllActions {
			get {
				List<ActionBase> result = new List<ActionBase>();
				foreach(Controller controller in Frame.Controllers) {
					if(!(controller is ScriptRecorderControllerBase)) {
						result.AddRange(controller.Actions);
					}
				}
				return result;
			}
		}
		protected void CreateListeners() {
			if(IsControllerActive) {
				Logger.Instance.SetHeader(ApplicationName, DataBaseName);
				ControlNameHelper.Instance.RegisterFrame(Frame);
				listeners = new List<IScriptRecorderControlListener>();
				ScriptRecorderActionsListenerBase actionsListenerBase = CreateActionsListenerCore();
				if(actionsListenerBase != null) {
					listeners.Add(actionsListenerBase);
				}
				if(Frame.View is DetailView) {
					DetailView dv = (DetailView)Frame.View;
					listeners.AddRange(PropertyEditorListenersFactory.CreatePropertyEditorListeners(dv.GetItems<PropertyEditor>()));
				}
				if(Frame.View is ListView) {
					ListView listView = (ListView)Frame.View;
					if(listView.Model.MasterDetailMode == MasterDetailMode.ListViewAndDetailView) {
						listeners.Add(new ListViewListener(listView));
					}
				}
				ListenersSetActive();
			}
		}
		protected virtual string ApplicationName {
			get { return "Application_name"; }
		}
		protected virtual string DataBaseName {
			get {
				string result = "";
				if(Frame.Application != null) {
					if(Frame.Application.Connection != null && !string.IsNullOrEmpty(Frame.Application.Connection.Database)) {
						result = Frame.Application.Connection.Database;
					}
					if(string.IsNullOrEmpty(result)) {
						result = GetDataBaseNameFromConnectionString(Frame.Application.ConnectionString);
					}
				}
				return result;
			}
		}
		private string GetDataBaseNameFromConnectionString(string connectionString) {
			if(!string.IsNullOrEmpty(connectionString)) {
				if(connectionString.Contains("://")) {
					return connectionString;
				}
				DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
				builder.ConnectionString = connectionString;
				object dataBaseName;
				if(builder.TryGetValue("Initial Catalog", out dataBaseName)) {
					return dataBaseName.ToString();
				}
			}
			return "";
		}
		protected virtual ScriptRecorderActionsListenerBase CreateActionsListener(List<ActionBase> actions) {
			return new ScriptRecorderActionsListenerBase(actions);
		}
		protected virtual PropertyEditorListenersFactoryBase PropertyEditorListenersFactory {
			get { return null; }
		}
		public bool IsControllerActive {
			get {
				return Active && !CurrentViewShowScriptView;
			}
		}
		public SimpleAction PauseRecordAction {
			get { return pauseRecord; }
		}
		public SimpleAction StartRecordAction {
			get { return startRecord; }
		}
		public SimpleAction SaveScriptAction {
			get { return saveScript; }
		}
		public SimpleAction ShowScriptAction {
			get { return showScript; }
		}
		public SimpleAction SaveScriptInShowScriptViewAction {
			get { return saveScriptInShowScriptView; }
		}
		public static bool? ScriptRecorderEnabled {
			get { return scriptRecorderEnabled; }
			set { scriptRecorderEnabled = value; }
		}
		public List<IScriptRecorderControlListener> Listeners {
			get { return listeners; }
		}
#if DebugTest
		public string DebugTest_GetDataBaseNameFromConnectionString(string connectionString) {
			return GetDataBaseNameFromConnectionString(connectionString);
		}
#endif
	}
}
