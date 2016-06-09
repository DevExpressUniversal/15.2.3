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

using DevExpress.Utils.Design;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils.Themes;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Policies;
using Microsoft.Windows.Design.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Threading;
using Task = System.Threading.Tasks.Task;
using System.ServiceModel;
using DevExpress.MarkupUtils.Design;
using DevExpress.Xpf.Core.Design.Utils;
using System.CodeDom;
using Microsoft.Windows.Design;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Core.Design.SmartTags {
	[UsesItemPolicy(typeof(DesignTimeThemeSelectorAdornerPolicy))]
	public class DesignTimeThemeSelectorAdornerProvider : AdornerProvider {
		DesignerView View {
			get { return view; }
			set {
				if(view == value)
					return;
				var oldValue = view;
				view = value;
				OnViewChanged(oldValue);
			}
		}
		protected override void Activate(ModelItem item) {
			base.Activate(item);
			View = DesignerView.FromContext(Context);
		}
		protected override void Deactivate() {
			View = null;
			base.Deactivate();
		}
		public override bool IsToolSupported(Tool tool) {
			return true;
		}
		void OnViewChanged(DesignerView oldValue) {
			if(oldValue != null)
				DesignTimeThemeSelectorService.UnregisterView(oldValue);
			if(View != null)
				DesignTimeThemeSelectorService.RegisterView(View);
		}
		DesignerView view;
	}
	public static class DesignTimeThemeSelectorService {
		const string AppConfigFileName = "App.config";
		const string EventHandlerName = "OnAppStartup_UpdateThemeName";
		const string UpdateApplicationThemeNameMethodName = "UpdateApplicationThemeName";
		static readonly Type appThemeHelperType = typeof(ApplicationThemeHelper);
		static Theme currentTheme;
		static DesignTimeThemeSelectorService() {
			EventManager.RegisterClassHandler(typeof(FrameworkElement), FrameworkElement.LoadedEvent, new RoutedEventHandler(OnFrameworkElementLoadedCallback));
			Views = new List<WeakReference>();
		}
		public static Theme CurrentTheme {
			get { return currentTheme; }
			set {
				if (currentTheme == value)
					return;
				var oldValue = currentTheme;
				currentTheme = value;
				OnCurrentThemeChanged(oldValue);
			}
		}
		static List<WeakReference> Views { get; set; }
		public static void RegisterView(DesignerView view) {
			if(FrameworkElementSmartTagAdorner.VSVersion <= 10)
				return;
			ApplyThemeCore(view);
			if(GetViewReference(view) != null)
				return;
			Views.Add(new WeakReference(view));
			LoadThemeNameAsync(view);
		}
		public static void UnregisterView(DesignerView view) {
			WeakReference reference = GetViewReference(view);
			if(reference != null)
				Views.Remove(reference);
		}
		static void LoadThemeNameAsync(DesignerView view) {
			Task.Factory.StartNew<string>(LoadThemeNameFromConfig, view).ContinueWith(x => {
				if (x.IsFaulted) {
#if DEBUG
					if (x.Exception != null)
						System.Diagnostics.Debug.WriteLine(x.Exception.Message);
#endif
				} else {
					string themeName = x.Result;
					CurrentTheme = string.IsNullOrEmpty(themeName) ? null : Theme.FindTheme(themeName);
				}
			}, new CancellationToken(), TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
		}
		static void ApplyTheme() {
			Views = Views.Where(view => view.IsAlive).ToList();
			Views.ForEach(reference => ApplyThemeCore((DesignerView)reference.Target));
		}
		static WeakReference GetViewReference(DesignerView view) {
			return Views.Where(reference => reference.IsAlive).FirstOrDefault(reference => reference.Target == view);
		}
		static void ApplyThemeCore(DesignerView view) {
			if (view == null || !view.IsLoaded || view.RootView == null || !view.IsInVisualTree())
				return;
			SetTheme(view);
		}
		static void OnFrameworkElementLoadedCallback(object sender, System.Windows.RoutedEventArgs e) {
			if (sender is UserControl)
				CheckIsPreviewAndSetTheme((UserControl)sender);
			UpdateTheme(sender as FrameworkElement);
		}
		static void CheckIsPreviewAndSetTheme(UserControl sender) {
			string typeName = sender.GetType().Name;
			if (string.Compare(typeName, "XamlPreviewUserControl", StringComparison.Ordinal) != 0)
				return;
			var contentPresenter = DevExpress.Xpf.Core.Native.LayoutHelper.FindElement(sender, (elem) => elem is ContentPresenter) as ContentPresenter;
			var content = contentPresenter == null ? null : contentPresenter.Content as FrameworkElement;
			if (content != null && string.IsNullOrEmpty(ThemeManager.GetThemeName(content))) {
				SetTheme(content);
			}
		}
		static void UpdateTheme(FrameworkElement frameworkElement) {
			var designerView = DesignerView.GetDesignerView(frameworkElement);
			if (designerView == null || AdornerProperties.GetModel(frameworkElement) != null)
				return;
			ApplyThemeCore(designerView);
		}
		static void SaveThemeNameAsync(Theme previousTheme, DesignerView view) {
			Task.Factory.StartNew(() => {
				SaveThemeNameToConfig(CurrentTheme, view);
			}).ContinueWith(x => {
				if(x.IsFaulted) {
					currentTheme = previousTheme;
#if DEBUG
					if(x.Exception != null)
						System.Diagnostics.Debug.WriteLine(x.Exception.Message);
#endif
				}
				ApplyTheme();
			}, new CancellationToken(), TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
			AddHandler(view.Context);
		}
		static void AddHandler(EditingContext context) {
			ModelItem appXaml = GetAppXamlModelItem(context);
			if(appXaml == null)
				return;
			using(var scope = appXaml.BeginEdit()) {
				var eventService = appXaml.Context.Services.GetService<EventBindingService>();
				var startupEvent = appXaml.Events["Startup"];
				if(startupEvent.Handlers.Count == 0) {
					startupEvent.Handlers.Add(EventHandlerName);
					CreateAppStartupEventHandlerIfNotExist(eventService, startupEvent);
				} else if(!isUpdateThemeInvoke && !startupEvent.Handlers.Contains(EventHandlerName) && !IsUpdateThemeNameCalled(appXaml.Context, startupEvent.Handlers.ElementAt(0))) {
					var statements = new CodeStatementCollection();
					statements.Add(CreateUpdateAppThemeNameExpression());
					eventService.AppendStatements(startupEvent, startupEvent.Handlers.ElementAt(0), statements);
					isUpdateThemeInvoke = true;
				}
				scope.Complete();
			}
		}
		static bool isUpdateThemeInvoke = false;
		static bool IsUpdateThemeNameCalled(EditingContext editingContext, string handlerMethod) {
			var helper = CodeHelper.Create(editingContext);
			return helper != null && helper.IsMethodInvokedInHandler(handlerMethod, appThemeHelperType.Name, UpdateApplicationThemeNameMethodName);
		}
		static ModelItem GetAppXamlModelItem(EditingContext context) {
			if(context == null)
				return null;
			var externalResourceService = context.Services.GetService<ExternalResourceService>();
			try {
				if(externalResourceService.ApplicationModel != null) {
					var appXamlModelItem = externalResourceService.ApplicationModel;
					return appXamlModelItem.ModelItem;
				}
			} catch {
				return externalResourceService.ApplicationModel.ModelItem;
			}
			return null;
		}
		static void CreateAppStartupEventHandlerIfNotExist(EventBindingService eventService, ModelEvent modelEvent) {
			if(eventService.IsExistingMethodName(modelEvent, EventHandlerName))
				return;
			if(eventService.CreateMethod(modelEvent, EventHandlerName)) {
				CodeExpressionStatement expressionStatement = new CodeExpressionStatement(CreateUpdateAppThemeNameExpression());
				eventService.AppendStatements(modelEvent, EventHandlerName, new CodeStatementCollection(new CodeStatement[] { expressionStatement }));
			}
		}
		static CodeMethodInvokeExpression CreateUpdateAppThemeNameExpression() {
			return new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(appThemeHelperType), UpdateApplicationThemeNameMethodName));
		}
		static string GetExistingFileAppConfigFilePath(DesignerView view) {
			ModelService modelService = view.Context.Services.GetService<ModelService>();
			object sceneViewModel = modelService.GetPrivateField("viewModel");
			object projectContext = sceneViewModel.GetPublicProperty("ProjectContext");
			object projectRoot = projectContext.GetPublicProperty("ProjectRoot");
			string appConfigPath = Path.Combine(projectRoot.ToString(), AppConfigFileName);
			object projectItem = projectContext.InvokePublicMethod("FindItem", typeof(string), appConfigPath);
			return projectItem != null ? appConfigPath : null;
		}
		static string GetCombinedFilePath(DesignerView view, string filePath) {
			ModelService modelService = view.Context.Services.GetService<ModelService>();
			object sceneViewModel = modelService.GetPrivateField("viewModel");
			object projectContext = sceneViewModel.GetPublicProperty("ProjectContext");
			object projectRoot = projectContext.GetPublicProperty("ProjectRoot");
			return Path.Combine(projectRoot.ToString(), AppConfigFileName);
		}
		static string LoadThemeNameFromConfig(object view) {
			var designerView = view as DesignerView;
			string appConfigPath = GetExistingFileAppConfigFilePath(designerView);
			if(appConfigPath == null)
				return string.Empty;
			return ConfigurationHelper.GetApplicationThemeNameFromConfig(appConfigPath);
		}
		static void SaveThemeNameToConfig(Theme theme, DesignerView view) {
			string themeName = theme == null ? string.Empty : theme.Name;
			if(string.Equals(themeName, LoadThemeNameFromConfig(view), StringComparison.OrdinalIgnoreCase))
				return;
			if (theme != null)
				DesignDteHelper.AddAssemblyToReferences(theme.Assembly.GetName().Name);
			string appFilePath = GetExistingFileAppConfigFilePath(view);
			if(String.IsNullOrEmpty(appFilePath)) {
				appFilePath = GetCombinedFilePath(view, AppConfigFileName);
				if(String.IsNullOrEmpty(appFilePath))
					return;
				ConfigurationHelper.SaveApplicationThemeNameToConfig(appFilePath, themeName);
				DesignDteHelper.AddFileToActiveProject(appFilePath);
			} else if(DesignDteHelper.CheckoutFile(appFilePath))
				ConfigurationHelper.SaveApplicationThemeNameToConfig(appFilePath, themeName);
		}
		static void OnCurrentThemeChanged(Theme oldValue) {
			SaveThemeNameAsync(CurrentTheme, Views.Where(view => view.IsAlive).Select(view => (DesignerView)view.Target).FirstOrDefault());
			ApplyTheme();
		}
		static void SetTheme(FrameworkElement elem) {
			string themeName = CurrentTheme.Return(theme => theme.Name, () => NoneThemeName);
			try {
				ThemeManager.SetThemeName(elem, themeName);
			} catch {
				ClearTheme(elem);
			}
		}
		static void ClearTheme(FrameworkElement elem) {
			ThemeManager.SetThemeName(elem, NoneThemeName);
		}
		const string NoneThemeName = "None";
	}
	public class DesignTimeThemeSelectorAdornerPolicy : SelectionPolicy {
		protected override IEnumerable<ModelItem> GetPolicyItems(Selection selection) {
			DesignerView.FromContext(Context).Do(view => DesignTimeThemeSelectorService.RegisterView(view));
			return base.GetPolicyItems(selection);
		}
	}
	abstract class CodeHelper {
		const string csharpSuffix = "cs";
		const string vbSuffix = "vb";
		public static CodeHelper Create(EditingContext context) {
			if(context == null)
				throw new ArgumentNullException("context");
			string filePath = context.ToString();
			string path = string.Concat(filePath, ".", csharpSuffix);
			if(File.Exists(path))
				return new CSharpCodeHelper(path);
			else {
				path = string.Concat(filePath, ".", vbSuffix);
				if(File.Exists(path))
					return new VBCodeHelper(path);
			}
			return null;
		}
		protected string Path { get; private set; }
		public CodeHelper(string path) {
			Path = path;
		}
		public bool IsMethodInvokedInHandler(string handlerMethod, string className, string methodName) {
			using(var sr = File.OpenText(Path)) {
				string code = GetMethodCode(handlerMethod, sr);
				return code.Contains(className) && code.Contains(methodName);
			}
		}
		protected abstract string GetMethodCode(string handlerMethod, StreamReader sr);
		protected abstract bool ContainsMethodInvoke(string line, string handlerMethod);
	}
	class CSharpCodeHelper : CodeHelper {
		public CSharpCodeHelper(string path) : base(path) { }
		protected override bool ContainsMethodInvoke(string line, string handlerMethod) {
			return line.Contains("void") && line.Contains(handlerMethod);
		}
		protected override string GetMethodCode(string handlerMethod, StreamReader sr) {
			string line = string.Empty;
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			do {
				line = sr.ReadLine();
			} while(!sr.EndOfStream && !ContainsMethodInvoke(line, handlerMethod));
			int idx = 0;
			if(line.Contains("{"))
				idx++;
			do {
				line = sr.ReadLine();
				sb.AppendLine(line);
				if(line.Contains("{"))
					idx++;
				if(line.Contains("}"))
					idx--;
			} while(!sr.EndOfStream && idx > 0);
			return sb.ToString();
		}
	}
	class VBCodeHelper : CodeHelper {
		public VBCodeHelper(string path) : base(path) { }
		protected override bool ContainsMethodInvoke(string line, string handlerMethod) {
			return line.Contains("Sub") && line.Contains(handlerMethod);
		}
		protected override string GetMethodCode(string handlerMethod, StreamReader sr) {
			string line = string.Empty;
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			do {
				line = sr.ReadLine();
			} while(!sr.EndOfStream && !ContainsMethodInvoke(line, handlerMethod));
			do {
				line = sr.ReadLine();
				sb.AppendLine(line);
			} while(!sr.EndOfStream && !line.Contains("End Sub"));
			return sb.ToString();
		}
	}
	public static class DynamicObjectHelper {
		public static object GetPrivateField(this object o, string fieldName) {
			return o.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(o);
		}
		public static object GetPublicProperty(this object o, string propertyName) {
			return o.GetType().GetProperty(propertyName).GetValue(o, null);
		}
		public static object InvokePublicMethod(this object o, string methodName, Type type, params object[] parameters) {
			return o.GetType().GetMethod(methodName, new Type[] { type }).Invoke(o, parameters);
		}
		public static object InvokePublicMethod(this object o, string methodName, params object[] parameters) {
			return o.GetType().GetMethod(methodName).Invoke(o, parameters);
		}
		public static object InvokePublicMethod(this object o, string methodName, Type[] types, params object[] parameters) {
			if(types == null)
				return o.GetType().GetMethod(methodName).Invoke(o, parameters);
			else
				return o.GetType().GetMethod(methodName, types).Invoke(o, parameters);
		}
	}
}
