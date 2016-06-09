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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.Utils.Base;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraBars.Docking2010.Views.Tabbed;
namespace DevExpress.ExpressApp.Win {
	public interface IModelOptionsTabbedMdiLayout {
		[Browsable(false)]
		string DocumentManagerState { get; set; }
		[DefaultValue(true)]
		bool RestoreTabbedMdiLayout { get; set; }
	}
	[DomainLogic(typeof(IModelOptionsTabbedMdiLayout))]
	public class ModelOptionsTabbedMdiLayoutDomainLogic {
		public static void BeforeSet_RestoreTabbedMdiLayout(IModelOptionsTabbedMdiLayout node, object value) {
			if(!((bool)value)) {
				node.DocumentManagerState = null;
			}
		}
	}
	[DomainLogic(typeof(IBaseDocumentSettings)), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public static class BaseDocumentSettingsDomainLogic {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static Size? Get_FloatSize(IBaseDocumentSettings node) {
			int width;
			int height;
			if(int.TryParse(((IModelFormState)node).Width, out width) && int.TryParse(((IModelFormState)node).Height, out height)) {
				return new Size(width, height);
			}
			return null;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void Set_FloatSize(IBaseDocumentSettings node, Size? value) {
			if(value != null) {
				((IModelFormState)node).Width = value.Value.Width.ToString();
				((IModelFormState)node).Height = value.Value.Height.ToString();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static Point? Get_FloatLocation(IBaseDocumentSettings node) {
			int x;
			int y;
			if(int.TryParse(((IModelFormState)node).X, out x) && int.TryParse(((IModelFormState)node).Y, out y)) {
				return new Point(x, y);
			}
			return null;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void Set_FloatLocation(IBaseDocumentSettings node, Point? value) {
			if(value != null) {
				((IModelFormState)node).X = value.Value.X.ToString();
				((IModelFormState)node).Y = value.Value.Y.ToString();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static IDictionaryEnumerator GetProperties(IBaseDocumentSettings node) {
			Hashtable hashtable = new Hashtable();
			hashtable.Add("FloatSize", node.FloatSize);
			hashtable.Add("FloatLocation", node.FloatLocation);
			return hashtable.GetEnumerator();
		}
	}
	public class MdiShowViewStrategy : WinShowViewStrategyBase {
		private int modalCount = 0;
		private List<WinWindow> delayedToShow = new List<WinWindow>();
		private MdiMode mdiMode;
		private DocumentManagerStateRestorer documentManagerStateRestorer;
		private void Form_Activated(object sender, EventArgs e) {
			Form form = (Form)sender;
			if(form.MdiParent != null) {
				WinWindow window = FindWindowByForm(form.MdiParent);
				ShowNavigationItemController controller = window.GetController<ShowNavigationItemController>();
				if(controller != null) {
					WinWindow childWindow = FindWindowByForm((Form)sender);
					if(childWindow != null) {
						controller.UpdateSelectedItem(childWindow.View);
					}
				}
			}
		}
		private void window_SynchronizeTemplateInfo(object sender, CancelEventArgs e) {
			e.Cancel = false;
		}
		private void childWindow_SynchronizeTemplateInfo(object sender, CancelEventArgs e) {
			WinWindow window = (WinWindow)sender;
			e.Cancel = window.Form.MdiParent != null && mdiMode == MdiMode.Tabbed;
		}
		private void OnCustomRestoreViewLayout(CustomRestoreViewLayoutEventArgs args) {
			if(CustomRestoreViewLayout != null) {
				CustomRestoreViewLayout(this, args);
			}
		}
		private void OnCustomSaveViewLayout(CustomSaveViewLayoutEventArgs args) {
			if(CustomSaveViewLayout != null) {
				CustomSaveViewLayout(this, args);
			}
		}
		private void MdiShowViewStrategy_CustomDocumentsHostWindow(object sender, CustomDocumentsHostWindowEventArgs e) {
			OnCustomDocumentsHostWindow(e);
		}
		private ViewShortcut GetViewShortcutByDocument(BaseDocument document) {
			return ViewShortcut.FromString(documentManagerStateRestorer.FindDocumentSerializedControl(document));
		}
		protected virtual bool CanDeserialize(DocumentControlDescription description) {
			ViewShortcut viewShortcut = ViewShortcut.FromString(description.SerializedControl);
			IModelView modelView = Application.FindModelView(viewShortcut.ViewId);
			if(modelView == null) {
				return false;
			}
			if(modelView is IModelDetailView) {
				if(!((IModelDetailView)modelView).ModelClass.TypeInfo.IsPersistent) {
					return false;
				}
				Type objectType = viewShortcut.ObjectClass ?? ((IModelDetailView)modelView).ModelClass.TypeInfo.Type;
				if(objectType == null) {
					return false;
				}
				using(IObjectSpace objectSpace = Application.CreateObjectSpace(objectType)) {
					object objectKey = string.IsNullOrEmpty(viewShortcut.ObjectKey) ? null : objectSpace.GetObjectKey(objectType, viewShortcut.ObjectKey);
					return objectSpace.GetObjectByKey(objectType, objectKey) != null;
				}
			}
			if(modelView is IModelListView) {
				return ((IModelListView)modelView).ModelClass.TypeInfo.IsPersistent;
			}
			return true;
		}
		private void SaveDocumentLayout() {
			IModelOptionsTabbedMdiLayout mdiLayoutOptions = (IModelOptionsTabbedMdiLayout)Application.Model.Options;
			if(mdiLayoutOptions.RestoreTabbedMdiLayout && UIType == UIType.TabbedMDI && MainWindow != null) {
				BaseView documentManagerView = ((IDocumentsHostWindow)MainWindow.Form).DocumentManager.View;
				CustomSaveViewLayoutEventArgs args = new CustomSaveViewLayoutEventArgs(documentManagerView);
				OnCustomSaveViewLayout(args);
				if(!args.Handled) {
					DocumentManagerState state = documentManagerStateRestorer.SerializeDocumentManagerState(documentManagerView, SerializeDocumentControl);
					using(MemoryStream stream = new MemoryStream()) {
						new XmlSerializer(typeof(DocumentManagerState)).Serialize(stream, state);
						mdiLayoutOptions.DocumentManagerState = Convert.ToBase64String(stream.GetBuffer());
					}
				}
			}
		}
		private void RestoreViewLayout(WinWindow mainWindow) {
			IModelOptionsTabbedMdiLayout mdiLayoutOptions = (IModelOptionsTabbedMdiLayout)Application.Model.Options;
			if(mdiLayoutOptions.RestoreTabbedMdiLayout && UIType == UIType.TabbedMDI) {
				BaseView view = ((IDocumentsHostWindow)mainWindow.Form).DocumentManager.View;
				CustomRestoreViewLayoutEventArgs customRestoreViewLayoutEventArgs = new CustomRestoreViewLayoutEventArgs(view);
				OnCustomRestoreViewLayout(customRestoreViewLayoutEventArgs);
				if(!customRestoreViewLayoutEventArgs.Handled && !string.IsNullOrEmpty(mdiLayoutOptions.DocumentManagerState)) {
					DocumentManagerState documentManagerState = null;
					using(MemoryStream stream = new MemoryStream(Convert.FromBase64String(mdiLayoutOptions.DocumentManagerState))) {
						documentManagerState = (DocumentManagerState)new XmlSerializer(typeof(DocumentManagerState)).Deserialize(stream);
					}
					if(documentManagerState != null) {
						documentManagerStateRestorer.DeserializeDocumentManagerState(view, documentManagerState, DeserializeControl, CanDeserialize);
					}
				}
			}
		}
		private DocumentControlDescription SerializeDocumentControl(Control control) {
			WinWindow window = FindWindowByForm(control as Form);
			if(window != null && window.View != null) {
				bool canSerialize = true;
				if(window.View is DetailView) {
					canSerialize = window.View.ObjectTypeInfo != null && window.View.ObjectTypeInfo.IsPersistent
						&& !window.View.ObjectSpace.IsNewObject(window.View.CurrentObject)
						&& !window.View.ObjectSpace.IsDeletedObject(window.View.CurrentObject);
				}
				else if(window.View is ListView) {
					canSerialize = window.View.ObjectTypeInfo != null && window.View.ObjectTypeInfo.IsPersistent;
				}
				if(canSerialize) {
					return new DocumentControlDescription(window.View.CreateShortcut().ToString(), window.View.Model.ImageName);
				}
			}
			return null;
		}
		protected override WinWindow CreateInspectorWindow(ShowViewParameters parameters, ShowViewSource showViewSource) {
			WinWindow window = base.CreateInspectorWindow(parameters, showViewSource);
			if(window.Template != null && window.View != null) {
				IModelTemplateWin modelTemplate = Application.GetTemplateCustomizationModel(window.Template) as IModelTemplateWin;
				if(modelTemplate != null) {
					IPropertiesProvider propertiesProvider = modelTemplate.FormStates[window.View.Id] as IPropertiesProvider;
					if(propertiesProvider != null) {
						DocumentSettings.Attach(window.Form, propertiesProvider);
					}
				}
			}
			return window;
		}
		protected override WinWindow FindWindowByView(View view) {
			WinWindow result = base.FindWindowByView(view);
			if(result == null) {
				IDocumentsHostWindow documentsHostWindow = WinWindow.LastActiveExplorer as IDocumentsHostWindow;
				if(documentsHostWindow != null) {
					BaseView documentManagerView = documentsHostWindow.DocumentManager.View;
					ViewShortcut viewShortcut = view.CreateShortcut();
					foreach(BaseDocument document in documentManagerView.Documents) {
						if(GetViewShortcutByDocument(document) == viewShortcut) {
							documentManagerView.Controller.Activate(document);
							result = FindWindowByForm(document.Form);
							break;
						}
					}
				}
			}
			return result;
		}
		protected override bool IsCompatibleWindow(WinWindow existingWindow, View view, ViewShortcut viewShortcut) {
			return (existingWindow.Form.MdiParent == null || existingWindow.Form.MdiParent == WinWindow.LastActiveExplorer) &&
				base.IsCompatibleWindow(existingWindow, view, viewShortcut);
		}
		protected override WindowStyle CalculateWindowStyle(ShowViewParameters parameters, ShowViewSource showViewSource) {
			NewWindowTarget newWindowTarget = CalculateNewWindowTarget(parameters, showViewSource);
			if(parameters.CreatedView is ListView && parameters.Context != TemplateContext.PopupWindow &&
				(parameters.TargetWindow == TargetWindow.NewWindow || parameters.TargetWindow == TargetWindow.Current && showViewSource.SourceFrame is WinWindow && IsExplorer((WinWindow)showViewSource.SourceFrame)) &&
				newWindowTarget == NewWindowTarget.Separate
				) {
				return WindowStyle.Explorer;
			}
			else {
				return WindowStyle.Inspector;
			}
		}
		protected override NewWindowTarget CalculateNewWindowTarget(ShowViewParameters parameters, ShowViewSource showViewSource) {
			NewWindowTarget result = NewWindowTarget.Separate;
			if(parameters.Context == TemplateContext.Undefined || parameters.Context == TemplateContext.View) {
				result = base.CalculateNewWindowTarget(parameters, showViewSource);
				if(result == NewWindowTarget.Default &&
					Application.Model != null &&
					Application.Model.Options is IModelOptionsWin) {
					result = ((IModelOptionsWin)Application.Model.Options).MdiDefaultNewWindowTarget;
				}
				if(WinWindow.LastActiveExplorer != null && WinWindow.LastActiveExplorer.MdiChildren.Length == 0) {
					result = NewWindowTarget.MdiChild;
				}
			}
			return result;
		}
		protected override void ShowViewInModalWindow(ShowViewParameters parameters, ShowViewSource showViewSource) {
			modalCount++;
			try {
				base.ShowViewInModalWindow(parameters, showViewSource);
			}
			finally {
				modalCount--;
			}
		}
		protected override void ShowViewFromCommonView(ShowViewParameters parameters, ShowViewSource showViewSource) {
			WinWindow existWindow = FindWindowByView(parameters.CreatedView);
			if(existWindow != null && !IsNewWindowForced(parameters, showViewSource)) {
				if(parameters.CreatedView != existWindow.View) {
					parameters.CreatedView.Dispose();
					parameters.CreatedView = existWindow.View;
				}
				existWindow.Show();
			}
			else {
				ShowViewInNewWindow(parameters, showViewSource);
			}
		}
		protected override void ShowViewCore(ShowViewParameters parameters, ShowViewSource showViewSource) {
			if(modalCount > 0 && parameters.TargetWindow == TargetWindow.Default) { 
				parameters.TargetWindow = TargetWindow.NewModalWindow;
			}
			base.ShowViewCore(parameters, showViewSource);
		}
		protected override void ShowViewFromLookupView(ShowViewParameters parameters, ShowViewSource showViewSource) {
			ShowViewInModalWindow(parameters, showViewSource);
		}
		protected override void ShowViewFromNestedView(ShowViewParameters parameters, ShowViewSource showViewSource) {
			parameters.CreatedView.IsRoot = !IsSameObjectSpace(parameters, showViewSource);
			if(parameters.CreatedView.ObjectSpace is INestedObjectSpace) {
				ShowViewInModalWindow(parameters, showViewSource);
			}
			else {
				ShowViewInInspector(parameters, showViewSource);
			}
		}
		protected override WinWindow ShowViewInExplorerCore(ShowViewParameters parameters, ShowViewSource showViewSource) {
			WinWindow inspector = CreateInspectorWindow(parameters, showViewSource);
			((IXafDocumentsHostWindow)WinWindow.LastActiveExplorer).DocumentManager.View.AddFloatingDocumentsHost(inspector.Form);
			return inspector;
		}
		protected override WinWindow ShowViewInInspector(ShowViewParameters parameters, ShowViewSource showViewSource) {
			WinWindow window = CreateInspectorWindow(parameters, showViewSource);
			if(WinWindow.LastActiveExplorer != null && !IsExplorer(window) && parameters.Context != TemplateContext.PopupWindow) {
				if(CalculateNewWindowTarget(parameters, showViewSource) == NewWindowTarget.Separate) {
					((IXafDocumentsHostWindow)WinWindow.LastActiveExplorer).DocumentManager.View.AddFloatDocument(window.Form);
				}
				else {
					window.Form.MdiParent = WinWindow.LastActiveExplorer;
				}
			}
			ShowWindow(window);
			return window;
		}
		protected override void AfterAddWindow(WinWindow window) {
			if(IsExplorer(window)) {
				window.TemplateModelSaving += new EventHandler<CancelEventArgs>(window_SynchronizeTemplateInfo);
				IXafDocumentsHostWindow documentsHostWindow = (IXafDocumentsHostWindow)window.Template;
				documentsHostWindow.UIType = UIType;
				documentsHostWindow.DocumentManager.View.CustomDocumentsHostWindow += new CustomDocumentsHostWindowEventHandler(MdiShowViewStrategy_CustomDocumentsHostWindow);
			}
			else if(window.Context != TemplateContext.PopupWindow) {
				window.Form.Activated += new EventHandler(Form_Activated);
				window.TemplateModelSaving += new EventHandler<CancelEventArgs>(childWindow_SynchronizeTemplateInfo);
			}
		}
		protected virtual void OnCustomDocumentsHostWindow(CustomDocumentsHostWindowEventArgs e) {
			WinWindow targetWindow = FindWindowByForm(e.Document.Form);
			View floatedView = targetWindow.View;
			if(floatedView is ListView) {
				e.Constructor = delegate {
					ShowViewParameters parameters = new ShowViewParameters(floatedView);
					WinWindow explorer = CreateExplorerWindow(parameters);
					explorer.Form.WindowState = FormWindowState.Normal;
					return explorer.Form as IDocumentsHostWindow;
				};
			}
			else {
				targetWindow.Form.WindowState = FormWindowState.Normal;
				e.Cancel = true;
			}
		}
		protected override void OnWindowClosing(WinWindow window, CancelEventArgs e) {
			if(!e.Cancel && !internalCloseMainWindow && IsExplorer(window) && Explorers.Count == 1) {
				SaveDocumentLayout();
			}
			base.OnWindowClosing(window, e);
		}
		protected override void ShowWindow(WinWindow window) {
			if(!window.IsMain && MainWindow == null) {
				delayedToShow.Add(window);
			}
			else {
				base.ShowWindow(window);
			}
		}
		protected override void ShowViewInCurrentWindow(ShowViewParameters parameters, ShowViewSource showViewSource) {
			if(showViewSource.SourceFrame is WinWindow && IsExplorer((WinWindow)showViewSource.SourceFrame)) {
				ShowViewInNewWindow(parameters, showViewSource);
			}
			else {
				showViewSource.SourceFrame.SetView(parameters.CreatedView, showViewSource.SourceFrame);
			}
		}
		protected override void OnWindowClosed(WinWindow window) {
			if(IsExplorer(window)) {
				IXafDocumentsHostWindow documentsHostWindow = (IXafDocumentsHostWindow)window.Template;
				documentsHostWindow.DocumentManager.View.CustomDocumentsHostWindow -= new CustomDocumentsHostWindowEventHandler(MdiShowViewStrategy_CustomDocumentsHostWindow);
			}
			window.Form.Activated -= new EventHandler(Form_Activated);
			DocumentSettings.Detach(window.Form);
			base.OnWindowClosed(window);
		}
		protected override List<WinWindow> GetChildWindows(WinWindow winWindow) {
			if(IsLastExplorer(winWindow)) {
				Form mainForm = winWindow.Form;
				List<WinWindow> result = new List<WinWindow>();
				foreach(WinWindow inspector in Inspectors) {
					bool isMdiChild = inspector.Form.MdiParent == mainForm;
					if(!isMdiChild) { 
						result.Add(inspector);
					}
				}
				return result;
			}
			if(IsExplorer(winWindow)) {
				return new List<WinWindow>();
			}
			return base.GetChildWindows(winWindow);
		}
		private bool IsLastExplorer(WinWindow winWindow) {
			return Explorers.Count == 1 && Explorers[0] == winWindow;
		}
		protected override UIType GetUIType() {
			switch(MdiMode) {
				case MdiMode.Standard:
					return UIType.StandardMDI;
				case MdiMode.Tabbed:
					return UIType.TabbedMDI;
				default:
					throw new InvalidOperationException("The MdiMode property has invalid value.");
			}
		}
		protected override WinWindow ShowStartupWindowCore() {
			delayedToShow.Clear();
			WinWindow result;
			try {
				result = base.ShowStartupWindowCore();
			}
			finally {
				System.Windows.Forms.Application.DoEvents();
				foreach(WinWindow child in delayedToShow) {
					ShowWindow(child);
				}
			}
			return result;
		}
		protected override void OnStartupWindowLoad(WinWindow startupWindow) {
			RestoreViewLayout(startupWindow);
			base.OnStartupWindowLoad(startupWindow);
		}
		protected Control DeserializeControl(string serializedControl) {
			Control result = null;
			ViewShortcut shortcut = ViewShortcut.FromString(serializedControl);
			if(shortcut != ViewShortcut.Empty) {
				View view = Application.ProcessShortcut(shortcut);
				if(view != null) {
					result = CreateWindow(new ShowViewParameters(view), new ShowViewSource(Explorers[0], null), false).Form;
				}
			}
			return result;
		}
		public MdiShowViewStrategy(XafApplication application, MdiMode mdiMode)
			: base(application) {
			this.mdiMode = mdiMode;
			documentManagerStateRestorer = new DocumentManagerStateRestorer();
		}
		public MdiShowViewStrategy(XafApplication application) : this(application, MdiMode.Tabbed) { }
		public WinWindow GetActiveExplorer() {
			if(IsExplorer(Form.ActiveForm)) {
				return FindWindowByForm(Form.ActiveForm);
			}
			return null;
		}
		public WinWindow GetActiveInspector(WinWindow explorer) {
			if(!IsExplorer(explorer)) {
				throw new ArgumentException("Passed window is not an explorer");
			}
			return FindWindowByForm((explorer.Form).ActiveMdiChild);
		}
		public WinWindow GetActiveInspector() {
			if(Form.ActiveForm != null) {
				WinWindow window = FindWindowByForm(Form.ActiveForm);
				if(window != null) {
					if(!IsExplorer(window)) {
						return window;
					}
					else {
						return GetActiveInspector(window);
					}
				}
			}
			return null;
		}
		public override bool CloseAllWindows() {
			SaveDocumentLayout();
			return base.CloseAllWindows();
		}
		public override void Dispose() {
			base.Dispose();
			if(documentManagerStateRestorer != null) {
				documentManagerStateRestorer.Dispose();
				documentManagerStateRestorer = null;
			}
			WinWindow.LastActiveExplorer = null;
		}
		public MdiMode MdiMode {
			get { return mdiMode; }
		}
		public override bool SupportViewNavigationHistory {
			get { return false; }
		}
		public event EventHandler<CustomRestoreViewLayoutEventArgs> CustomRestoreViewLayout;
		public event EventHandler<CustomSaveViewLayoutEventArgs> CustomSaveViewLayout;
	}
}
