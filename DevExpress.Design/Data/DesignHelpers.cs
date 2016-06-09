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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.Win32;
using DevExpress.Utils.About;
using System.Reflection;
namespace DevExpress.Utils.Design {
	public enum ThemeTool {
		ASPxThemeBuilder,
		ASPxThemeDeployer
	}
	public static class SystemDesignSRHelper {
		private const string
			NoDataSource = "DataSourceIDChromeConverter_NoDataSource",
			NewDataSource = "DataSourceIDChromeConverter_NewDataSource";
		private static string GetStringFromDesignSR(string name) {
			System.Resources.ResourceManager resources =
				new System.Resources.ResourceManager("System.Design", typeof(ControlDesigner).Assembly);
			return resources.GetString(name);
		}
		public static string GetDataControlNoDataSource() {
			return GetStringFromDesignSR(NoDataSource);
		}
		public static string GetDataControlNewDataSource() {
			return GetStringFromDesignSR(NewDataSource);
		}
	}
	public class DXDesignerActionList : DesignerActionList {
		List<DesignerActionItem> items;
		public DXDesignerActionList(IComponent component, List<DesignerActionItem> items)
			: base(component) {
			this.items = items;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			foreach(DesignerActionItem item in items) {
				res.Add(item);
			}
			return res;
		}
	}
	public class DXSmartTagsHelper {
		static string VSVersion = string.Empty;
		public static void CreateDefaultLinks(ComponentDesigner designer, DesignerActionListCollection collection) {
			List<DesignerActionItem> items = new List<DesignerActionItem>();
			DXDesignerActionList list = new DXDesignerActionList(designer.Component, items);
			DesignerActionItem support = CreateSupportLink(designer, list);
			if(support != null) items.Add(support);
			DesignerActionItem docs = CreateDocumentationLink(designer, list);
			if(docs != null) items.Add(docs);
			if(items.Count > 0) collection.Insert(collection.Count, list);
		}
		public static void CreateDefaultVerbs(ComponentDesigner designer, DesignerVerbCollection verbs) {
			DesignerVerb verb = CreateSupportVerb(designer);
			if(verb != null) verbs.Insert(verbs.Count, verb);
		}
		public static DesignerVerb CreateSupportVerb(ComponentDesigner designer) {
			bool allowSearch = IsAllowPassComponentAsSearchText(designer.Component);
			string searchText = allowSearch ? designer.Component.GetType().Name : string.Empty;
			string componentGuid = GetComponentGuid(designer.Component);
			if(componentGuid == null) return null;
			return new DXGetSupportVerb(componentGuid, searchText, GetVSVersion());
		}
		static DesignerActionItem CreateDocumentationLink(ComponentDesigner designer, DXDesignerActionList list) {
			try {
				if(designer.Component == null) return null;
				IServiceProvider provider = designer.Component.Site;
				IHelpService helpService = (provider == null ? null : provider.GetService(typeof(IHelpService))) as IHelpService;
				if(helpService == null) return null;
				if(designer.Component == null || helpService == null) return null;
				object[] attrs = designer.Component.GetType().GetCustomAttributes(typeof(DXDocumentationProviderAttribute), true);
				if(attrs != null && attrs.Length > 0) {
					return new DXInvokeDocumentationActionItem(list, attrs[0] as DXDocumentationProviderAttribute, helpService);
				}
			}
			catch {
			}
			return null;
		}
		static DesignerActionItem CreateSupportLink(ComponentDesigner designer, DesignerActionList list) {
			try {
				bool allowSearch = IsAllowPassComponentAsSearchText(designer.Component);
				string searchText = allowSearch ? designer.Component.GetType().Name : string.Empty;
				string guid = GetComponentGuid(designer.Component);
				if(!string.IsNullOrEmpty(guid))
					return new DXGetSupportActionItem(list, guid, searchText, GetVSVersion());
			} catch {
			}
			return null;
		}
		public static DesignerActionItem CreateDemoCenterLink(ComponentDesigner designer, DesignerActionList list) {
			try {
				if(string.IsNullOrEmpty(GetDemoCenterExe())) return null;
				string component = designer.Component.GetType().FullName;
				string assemblyName = designer.Component.GetType().Assembly.GetName().Name;
				string product = GetProductName(assemblyName, component);
				if(string.IsNullOrEmpty(product)) return null;
				return new DXRunDemoCenterActionItem(list, product);
			}
			catch {
				return null;
			}
		}
		static string GetProductName(string assemblyName, string component) {
			return assemblyName;
		}
		static string GetComponentAssembly(IComponent component) {
			if(component == null) return string.Empty;
			string componentName = component.GetType().FullName.ToLowerInvariant();
			string assemblyName = component.GetType().Assembly.GetName().Name.ToLowerInvariant();
			if(componentName.Contains("aspxeditors"))
				assemblyName = AssemblyInfo.SRAssemblyEditorsWeb.ToLowerInvariant();
			if(componentName.Contains("aspxgridview"))
				assemblyName = AssemblyInfo.SRAssemblyASPxGridView.ToLowerInvariant();
			return assemblyName;
		}
		static string GetComponentGuid(IComponent component) {
			Guid guid;
			if(AssemblyGuids.TryGetValue(GetComponentAssembly(component), out guid))
				return guid.ToString();
			return null;
		}
		static bool IsAllowPassComponentAsSearchText(IComponent component) {
			string assemblyName = GetComponentAssembly(component);
			string type = component == null ? string.Empty : component.GetType().Name;
			if(assemblyName == AssemblyInfo.SRAssemblyEditors.ToLowerInvariant()) return true;
			if(assemblyName == AssemblyInfo.SRAssemblyUtils.ToLowerInvariant()) return true;
			if(assemblyName == AssemblyInfo.SRAssemblyXpo.ToLowerInvariant()) return true;
			if(assemblyName == AssemblyInfo.SRAssemblyWeb.ToLowerInvariant()) return true;
			if(assemblyName == AssemblyInfo.SRAssemblyEditorsWeb.ToLowerInvariant()) return true;
			if(assemblyName == AssemblyInfo.SRAssemblyVertGrid.ToLowerInvariant()) return true;
			if(assemblyName == AssemblyInfo.SRAssemblyBars.ToLowerInvariant()) return type != "BarManager";
			if(assemblyName == AssemblyInfo.SRAssemblyGrid.ToLowerInvariant()) return type == "GridLookUpEdit";
			if(assemblyName == AssemblyInfo.SRAssemblyCharts.ToLowerInvariant()) return true;
			if(assemblyName == AssemblyInfo.SRAssemblyChartsCore.ToLowerInvariant()) return true;
			if(assemblyName == AssemblyInfo.SRAssemblyChartsUI.ToLowerInvariant()) return true;
			if(assemblyName == AssemblyInfo.SRAssemblyChartsWeb.ToLowerInvariant()) return true;
			if(assemblyName == AssemblyInfo.SRAssemblyPrintingCore.ToLowerInvariant()) return true;
			if(assemblyName == AssemblyInfo.SRAssemblyPrinting.ToLowerInvariant()) return true;
			if(assemblyName == AssemblyInfo.SRAssemblyReports.ToLowerInvariant()) return true;
			if(assemblyName == AssemblyInfo.SRAssemblyReportsExtensions.ToLowerInvariant()) return true;
			if(assemblyName == AssemblyInfo.SRAssemblyReportsWeb.ToLowerInvariant()) return true;
			if(assemblyName == AssemblyInfo.SRAssemblyScheduler.ToLowerInvariant()) return true;
			if(assemblyName == AssemblyInfo.SRAssemblyTreeList.ToLowerInvariant()) return type == "TreeListLookUpEdit";
			if(assemblyName == AssemblyInfo.SRAssemblyDataAccess.ToLowerInvariant()) return true;
			if(assemblyName == AssemblyInfo.SRAssemblyDashboardWeb.ToLowerInvariant()) return true;
			return false;
		}
		static string GetVSVersion() {
			try {
				if(string.IsNullOrEmpty(VSVersion)) {
					string version = GetVSVersionCore();
					if(version.StartsWith("8."))
						VSVersion = "2005";
					if(version.StartsWith("9."))
						VSVersion = "2008";
					if(version.StartsWith("10."))
						VSVersion = "2010";
				}
				return VSVersion;
			}
			catch {
			}
			return string.Empty;
		}
		static string GetVSVersionCore() {
			Process currentProcess = Process.GetCurrentProcess();
			if(currentProcess == null)
				return String.Empty;
			ProcessModule mainModule = currentProcess.MainModule;
			if(mainModule == null)
				return String.Empty;
			FileVersionInfo versionInfo = mainModule.FileVersionInfo;
			if(versionInfo == null)
				return String.Empty;
			return versionInfo.ProductMajorPart + "." + versionInfo.ProductMinorPart;
		}
		internal class DXRunDemoCenterActionItem : DesignerActionMethodItem {
			string product;
			public DXRunDemoCenterActionItem(DesignerActionList list, string product)
				: base(list, "Invoke", "") {
				this.product = product;
			}
			public override string DisplayName { get { return "Run Demo"; } }
			public override void Invoke() {
				ExecuteProcess(GetDemoCenterExe(), this.product);
			}
			public override string Category { get { return "Information"; } }
		}
		internal class DXInvokeDocumentationActionItem : DesignerActionMethodItem {
			DXDocumentationProviderAttribute attribute;
			IHelpService helpService;
			public DXInvokeDocumentationActionItem(DesignerActionList list, DXDocumentationProviderAttribute attribute, IHelpService helpService)
				: base(list, "Invoke", string.Empty) {
				this.attribute = attribute;
				this.helpService = helpService;
			}
			public override string DisplayName {
				get {
					return string.Format("Go to {0}", this.attribute.Description);
				}
			}
			public override void Invoke() {
				string url = attribute.GetUrl();
				if(url.StartsWith("http://"))
					ExecuteProcess(url, string.Empty);
				else
					helpService.ShowHelpFromUrl(url);
			}
			public override string Category { get { return "Information"; } }
		}
		internal class DXGetSupportActionItem : DesignerActionMethodItem {
			internal const string queryFormat = "http://www.devexpress.com/Support/Center/SmartTag.aspx?searchtext={1}&pid={0}&ide={2}&version=" + AssemblyInfo.Version;
			internal const string queryEmptyProductFormat = "http://www.devexpress.com/sc";
			string[] queryParameters;
			public DXGetSupportActionItem(DesignerActionList list, params string[] queryParameters)
				: base(list, "Invoke", "") {
				this.queryParameters = queryParameters;
				Parse(this.queryParameters);
			}
			internal static void Parse(string[] queryParameters) {
				if(queryParameters == null) return;
				for(int n = 0; n < queryParameters.Length; n++) {
					queryParameters[n] = Uri.EscapeDataString(queryParameters[n]);
				}
			}
			public override bool IncludeAsDesignerVerb { get { return true; } }
			static string GetPropertyText(Type type, string name) {
				PropertyInfo pi = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Static);
				if(pi != null) {
					return string.Format("{0}", pi.GetValue(null, new object[] { }));
				}
				return string.Empty;
			}
			public override string DisplayName { 
				get {
					return GetLearnMoreOnlineString; 
				} 
			}
			internal static string GetLearnMoreOnlineString {
				get {
					string ret = "Learn More Online";
					Type type = Type.GetType("DevExpress.Design.Properties.Resources");
					if(type != null) {
						string localizationText = GetPropertyText(type, "LearnMoreOnline");
						if(!string.IsNullOrEmpty(localizationText))
							ret = localizationText;
					}
					return ret; 
				} 
			}
			public override void Invoke() {
				Invoke(queryParameters);
			}
			public override string Category { get { return "Information"; } }
			internal static void Invoke(string[] queryParameters) {
				string url = queryEmptyProductFormat;
				if(queryParameters != null && !string.IsNullOrEmpty(queryParameters[0])) {
					url = string.Format(queryFormat, queryParameters);
				}
				ExecuteProcess(url, string.Empty);
			}
		}
		internal class DXGetSupportVerb : DesignerVerb {
			string[] queryParameters;
			public DXGetSupportVerb(params string[] queryParameters)
				: base(DXGetSupportActionItem.GetLearnMoreOnlineString, null) {
				this.queryParameters = queryParameters;
				DXGetSupportActionItem.Parse(this.queryParameters);
			}
			public override void Invoke() {
				DXGetSupportActionItem.Invoke(this.queryParameters);
			}
		}
		static internal void ExecuteProcess(string name, string arguments) {
			System.Diagnostics.Process process = new System.Diagnostics.Process();
			process.StartInfo.FileName = name;
			process.StartInfo.Arguments = arguments;
			process.StartInfo.Verb = "Open";
			process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
			process.Start();
		}
		public static string GetDemoCenterExe() {
			return "DemoCenter.exe";
		}
		static Dictionary<string, Guid> assemblyGuids;
		static Dictionary<string, Guid> AssemblyGuids {
			get {
				if(assemblyGuids == null) assemblyGuids = GetAssemblyGuids();
				return assemblyGuids;
			}
		}
		static Dictionary<string, Guid> GetAssemblyGuids() {
			Dictionary<string, Guid> assemblyGuids = new Dictionary<string, Guid>();
			assemblyGuids.Add(AssemblyInfo.SRAssemblyEditors.ToLowerInvariant(), new Guid("7EBED197-57D3-46D3-B72E-EF56ED375262"));
			assemblyGuids.Add(AssemblyInfo.SRAssemblyUtils.ToLowerInvariant(), new Guid("7EBED197-57D3-46D3-B72E-EF56ED375262"));
			assemblyGuids.Add(AssemblyInfo.SRAssemblyGrid.ToLowerInvariant(), new Guid("A0256F31-A938-4360-BDF4-284E4B6EBCE3"));
			assemblyGuids.Add(AssemblyInfo.SRAssemblyLayoutControl.ToLowerInvariant(), new Guid("36A4479E-34EF-48D2-A985-D5EFF5C72EB2"));
			assemblyGuids.Add(AssemblyInfo.SRAssemblyBars.ToLowerInvariant(), new Guid("F17DA424-22E3-4633-910D-F24B4217D522"));
			assemblyGuids.Add(AssemblyInfo.SRAssemblyCharts.ToLowerInvariant(), new Guid("A23EED29-267A-4B8B-9D92-C5DCAE4F1E3B"));
			assemblyGuids.Add(AssemblyInfo.SRAssemblyChartsCore.ToLowerInvariant(), new Guid("A23EED29-267A-4B8B-9D92-C5DCAE4F1E3B"));
			assemblyGuids.Add(AssemblyInfo.SRAssemblyChartsUI.ToLowerInvariant(), new Guid("A23EED29-267A-4B8B-9D92-C5DCAE4F1E3B"));
			assemblyGuids.Add(AssemblyInfo.SRAssemblyChartsWeb.ToLowerInvariant(), new Guid("A23EED29-267A-4B8B-9D92-C5DCAE4F1E3B"));
			assemblyGuids.Add(AssemblyInfo.SRAssemblyGaugesCore.ToLowerInvariant(), new Guid("8C5E9752-2BD6-4056-91DA-970B89C08EF2"));
			assemblyGuids.Add(AssemblyInfo.SRAssemblyGaugesWin.ToLowerInvariant(), new Guid("8C5E9752-2BD6-4056-91DA-970B89C08EF2"));
			assemblyGuids.Add(AssemblyInfo.SRAssemblyNavBar.ToLowerInvariant(), new Guid("A120EC56-2302-4CE4-82EC-D9E2B38B7894"));
			assemblyGuids.Add(AssemblyInfo.SRAssemblyPivotGrid.ToLowerInvariant(), new Guid("EC082237-5FC1-461C-B8CF-5BE52FCD7FBB"));
			assemblyGuids.Add(AssemblyInfo.SRAssemblyPivotGridCore.ToLowerInvariant(), new Guid("EC082237-5FC1-461C-B8CF-5BE52FCD7FBB"));
			#region reporting
			assemblyGuids.Add(AssemblyInfo.SRAssemblyPrintingCore.ToLowerInvariant(), new Guid("2CDE1D48-4563-4824-9516-4BC5EA14164F"));
			assemblyGuids.Add(AssemblyInfo.SRAssemblyPrinting.ToLowerInvariant(), new Guid("2CDE1D48-4563-4824-9516-4BC5EA14164F"));
			assemblyGuids.Add(AssemblyInfo.SRAssemblyReports.ToLowerInvariant(), new Guid("E9F1506A-471C-4B72-82DF-151EFFCA6B0D"));
			assemblyGuids.Add(AssemblyInfo.SRAssemblyReportsExtensions.ToLowerInvariant(), new Guid("E9F1506A-471C-4B72-82DF-151EFFCA6B0D"));
			assemblyGuids.Add(AssemblyInfo.SRAssemblyReportsWeb.ToLowerInvariant(), new Guid("E9F1506A-471C-4B72-82DF-151EFFCA6B0D"));
			#endregion
			assemblyGuids.Add(AssemblyInfo.SRAssemblyScheduler.ToLowerInvariant(), new Guid("895BAB8A-5F43-4EC6-9205-3683F7F9F25D"));
			assemblyGuids.Add(AssemblyInfo.SRAssemblySchedulerExtensions.ToLowerInvariant(), new Guid("895BAB8A-5F43-4EC6-9205-3683F7F9F25D"));
			assemblyGuids.Add(AssemblyInfo.SRAssemblyRichEdit.ToLowerInvariant(), new Guid("203F88E5-FC49-4C34-90BD-13D351B18F05"));
			assemblyGuids.Add(AssemblyInfo.SRAssemblyRichEditExtensions.ToLowerInvariant(), new Guid("203F88E5-FC49-4C34-90BD-13D351B18F05"));
			assemblyGuids.Add(AssemblyInfo.SRAssemblySnap.ToLowerInvariant(), new Guid("F8E8E2F3-BB36-4268-AD2A-D97766D562F2"));
			assemblyGuids.Add(AssemblyInfo.SRAssemblySnapExtensions.ToLowerInvariant(), new Guid("F8E8E2F3-BB36-4268-AD2A-D97766D562F2"));
			assemblyGuids.Add(AssemblyInfo.SRAssemblySpellChecker.ToLowerInvariant(), new Guid("C86DB017-8D63-4E75-9633-BBCA1BD185C9"));
			assemblyGuids.Add(AssemblyInfo.SRAssemblyTreeList.ToLowerInvariant(), new Guid("27DFFD60-9D0E-4AEA-8280-7D55C93105E0"));
			assemblyGuids.Add(AssemblyInfo.SRAssemblyVertGrid.ToLowerInvariant(), new Guid("34EFF45C-95C0-4E09-B50A-013261A16730"));
			assemblyGuids.Add(AssemblyInfo.SRAssemblyXpo.ToLowerInvariant(), new Guid("9EDA7BE2-8F23-467B-BB4E-9D546DB79C87"));
			assemblyGuids.Add(AssemblyInfo.SRAssemblyWizard.ToLowerInvariant(), new Guid("E41E08A0-ED77-40EE-B124-C4FDEECB7821"));
			assemblyGuids.Add(AssemblyInfo.SRAssemblyWeb.ToLowerInvariant(), new Guid("725F3678-7B8C-476A-9BAE-D68DF69F01ED"));
			assemblyGuids.Add(AssemblyInfo.SRAssemblyASPxPivotGrid.ToLowerInvariant(), new Guid("48B469A5-CD05-48AA-B992-ED28E84DD7D7"));
			assemblyGuids.Add(AssemblyInfo.SRAssemblyTreeListWeb.ToLowerInvariant(), new Guid("A6A6028E-E0CD-418E-A4C9-31E297BB0D69"));
			assemblyGuids.Add(AssemblyInfo.SRAssemblySchedulerWeb.ToLowerInvariant(), new Guid("F8AAD72B-2600-4FE2-9047-07C820FADD48"));
			assemblyGuids.Add(AssemblyInfo.SRAssemblySpellCheckerWeb.ToLowerInvariant(), new Guid("C0D297F4-D920-452B-89C3-F348B4F4DEB4"));
			assemblyGuids.Add(AssemblyInfo.SRAssemblyHtmlEditorWeb.ToLowerInvariant(), new Guid("3A1FFE09-446F-4B93-9C49-3730B33E839E"));
			assemblyGuids.Add(AssemblyInfo.SRAssemblyASPxGauges.ToLowerInvariant(), new Guid("44915B4E-49EE-439b-9B48-A1BBC487CBAE"));
			assemblyGuids.Add(AssemblyInfo.SRAssemblyXtraPdfViewer.ToLowerInvariant(), new Guid("F2217727-B639-11E2-BED6-64700200865D"));
			assemblyGuids.Add(AssemblyInfo.SRAssemblyDataAccess.ToLowerInvariant(), new Guid("669A70D5-44CF-4C86-AED7-09D39571B704"));
			assemblyGuids.Add(AssemblyInfo.SRAssemblyDashboardWeb.ToLowerInvariant(), new Guid("A4441FEF-2D61-4937-A678-F5613C9A44CD"));
			return assemblyGuids;
		}
	}
	public class BaseParentControlDesignerSimple : ParentControlDesigner, ISmartTagGlyphObserver {
		public override System.Windows.Forms.Design.Behavior.GlyphCollection GetGlyphs(System.Windows.Forms.Design.Behavior.GlyphSelectionType selectionType) {
			System.Windows.Forms.Design.Behavior.GlyphCollection res = base.GetGlyphs(selectionType);
			if(CanUseComponentSmartTags) {
				SmartTagInfo ti = ComponentSmartTagProvider.UpdateGlyphs(res);
				if(ti != null) {
					OnComponentSmartTagChangedCore(ti.OwnerControl, ti.GlyphBounds);
				}
			}
			return res;
		}
		#region Component Smart Tags
		protected virtual bool CanUseComponentSmartTags {
			get { return false; }
		}
		ISmartTagProvider componentSmartTagProviderCore = null;
		protected ISmartTagProvider ComponentSmartTagProvider {
			get {
				if(componentSmartTagProviderCore == null) {
					componentSmartTagProviderCore = CreateComponentSmartTagProviderCore(Component.Site);
				}
				return componentSmartTagProviderCore;
			}
		}
		protected virtual ISmartTagProvider CreateComponentSmartTagProviderCore(IServiceProvider serviceProvider) {
			return new ControlSmartTagProviderBase(serviceProvider);
		}
		#endregion
		#region ISmartTagGlyphObserver
		void ISmartTagGlyphObserver.OnComponentSmartTagChanged(Control owner, System.Drawing.Rectangle glyphBounds) {
			OnComponentSmartTagChangedCore(owner, glyphBounds);
		}
		#endregion
		protected virtual void OnComponentSmartTagChangedCore(Control owner, System.Drawing.Rectangle glyphBounds) {
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(ComponentSmartTagProvider != null) {
					ComponentSmartTagProvider.Dispose();
				}
				this.componentSmartTagProviderCore = null;
			}
			base.Dispose(disposing);
		}
	}
	public class BaseControlDesignerSimple : ControlDesigner, ISmartTagGlyphObserver {
		DesignerActionListCollection actionLists;
		public override DesignerActionListCollection ActionLists {
			get {
#if DXWhidbey
				if(UseVerbsAsActionList) return base.ActionLists;
#endif
				if(actionLists == null || AlwaysCreateActionLists) actionLists = CreateActionLists();
				return actionLists;
			}
		}
		public override void InitializeNewComponent(System.Collections.IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
		}
		protected virtual DesignerActionListCollection CreateActionLists() {
			DesignerActionListCollection list = new DesignerActionListCollection();
			RegisterActionLists(list);
			return list;
		}
		protected virtual void RegisterActionLists(DesignerActionListCollection list) {
			RegisterAboutAction(list);
		}
		protected virtual void RegisterAboutAction(DesignerActionListCollection list) {
			DXAboutActionList about = GetAboutAction();
			if(about != null) {
				list.Insert(list.Count, about);
			}
			DXSmartTagsHelper.CreateDefaultLinks(this, list);
		}
		protected virtual DXAboutActionList GetAboutAction() { return null; }
		protected virtual void ResetActionLists() {
			this.actionLists = null;
		}
		protected virtual bool UseVerbsAsActionList { get { return false; } }
		protected virtual bool AlwaysCreateActionLists { get { return false; } }
		public override System.Windows.Forms.Design.Behavior.GlyphCollection GetGlyphs(System.Windows.Forms.Design.Behavior.GlyphSelectionType selectionType) {
			System.Windows.Forms.Design.Behavior.GlyphCollection res = base.GetGlyphs(selectionType);
			if(CanUseComponentSmartTags) {
				SmartTagInfo ti = ComponentSmartTagProvider.UpdateGlyphs(res);
				if(ti != null) {
					OnComponentSmartTagChangedCore(ti.OwnerControl, ti.GlyphBounds);
				}
			}
			return res;
		}		
		public bool IsUseComponentSmartTags { get { return CanUseComponentSmartTags; } }
		#region Component Smart Tags
		protected virtual bool CanUseComponentSmartTags {
			get { return false; }
		}
		ISmartTagProvider componentSmartTagProviderCore = null;
		protected ISmartTagProvider ComponentSmartTagProvider {
			get {
				if(componentSmartTagProviderCore == null) {
					componentSmartTagProviderCore = CreateComponentSmartTagProviderCore(Component.Site);
				}
				return componentSmartTagProviderCore;
			}
		}
		protected virtual ISmartTagProvider CreateComponentSmartTagProviderCore(IServiceProvider serviceProvider) {
			return new ControlSmartTagProviderBase(serviceProvider);
		}
		#endregion
		#region ISmartTagGlyphObserver
		void ISmartTagGlyphObserver.OnComponentSmartTagChanged(Control owner, System.Drawing.Rectangle glyphBounds) {
			OnComponentSmartTagChangedCore(owner, glyphBounds);
		}
		#endregion
		protected virtual void OnComponentSmartTagChangedCore(Control owner, System.Drawing.Rectangle glyphBounds) {
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(ComponentSmartTagProvider != null) {
					ComponentSmartTagProvider.Dispose();
				}
				this.componentSmartTagProviderCore = null;
			}
			base.Dispose(disposing);
		}
	}
	public class BaseComponentDesignerSimple : ComponentDesigner, ISmartTagGlyphObserver {
		DesignerActionListCollection actionLists;
		public override DesignerActionListCollection ActionLists {
			get {
#if DXWhidbey
				if(this.UseVerbsAsActionList) return base.ActionLists;
#endif
				if((this.actionLists == null) || this.AlwaysCreateActionLists)
					this.actionLists = this.CreateActionLists();
				return this.actionLists;
			}
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			if(this.CanUseComponentSmartTags) {
				this.ComponentSmartTagProvider.SelectionChanged += new SelectionChangedEventHandler(this.ComponentSmartTagSelectionChanged);
			}
		}
		protected virtual void OnComponentSmartTagChangedCore(Control owner, System.Drawing.Rectangle glyphBounds) {
		}
		protected virtual IComponentSmartTagProvider CreateComponentSmartTagProviderCore(IServiceProvider serviceProvider) {
			return new ComponentSmartTagProviderBase(serviceProvider);
		}
		void ISmartTagGlyphObserver.OnComponentSmartTagChanged(Control owner, System.Drawing.Rectangle glyphBounds) {
			this.OnComponentSmartTagChangedCore(owner, glyphBounds);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(ComponentSmartTagProvider != null) {
					ComponentSmartTagProvider.SelectionChanged -= ComponentSmartTagSelectionChanged;
					ComponentSmartTagProvider.Dispose();
				}
				this.componentSmartTagProviderCore = null;
			}
		}
		private void ComponentSmartTagSelectionChanged(object sender, SelectionChangedEventArgs args) {
			if(args.SelectionState == SelectionState.Select) {
				SmartTagInfo ti = this.ComponentSmartTagProvider.UpdateGlyphs(null);
				if(ti != null) {
					this.OnComponentSmartTagChangedCore(ti.OwnerControl, ti.GlyphBounds);
				}
			}
			else {
				this.ComponentSmartTagProvider.RemoveGlyph();
			}
		}		
		protected virtual bool AlwaysCreateActionLists { get { return false; } }
		protected virtual bool CanUseComponentSmartTags { get { return false; } }
		protected IComponentSmartTagProvider ComponentSmartTagProvider {
			get {
				if(this.componentSmartTagProviderCore == null) {
					this.componentSmartTagProviderCore = this.CreateComponentSmartTagProviderCore(base.Component.Site);
				}
				return this.componentSmartTagProviderCore;
			}
		}
		protected virtual bool UseVerbsAsActionList { get { return false; } }
		private IComponentSmartTagProvider componentSmartTagProviderCore;
		public override void InitializeNewComponent(System.Collections.IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
		}
		protected virtual DesignerActionListCollection CreateActionLists() {
			DesignerActionListCollection list = new DesignerActionListCollection();
			RegisterActionLists(list);
			return list;
		}
		protected virtual void RegisterActionLists(DesignerActionListCollection list) {
			RegisterAboutAction(list);
		}
		protected virtual void RegisterAboutAction(DesignerActionListCollection list) {
			DXAboutActionList about = GetAboutAction();
			if(about != null) {
				list.Insert(list.Count, about);
			}
			DXSmartTagsHelper.CreateDefaultLinks(this, list);
		}
		protected virtual DXAboutActionList GetAboutAction() { return null; }
	}
	public class DXAboutActionList : DesignerActionList {
		MethodInvoker aboutMethod;
		public DXAboutActionList(IComponent component, MethodInvoker aboutMethod)
			: base(component) {
			this.aboutMethod = aboutMethod;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			res.Add(new DesignerActionMethodItem(this, "About", "About", "Information"));
			return res;
		}
		public virtual void About() {
			if(this.aboutMethod != null) this.aboutMethod();
		}
	}
}
