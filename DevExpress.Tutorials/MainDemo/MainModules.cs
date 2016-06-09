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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Helpers;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraNavBar;
using DevExpress.XtraSplashScreen;
using DevExpress.Utils.Frames;
using System.Collections.Generic;
using DevExpress.Utils.About;
using System.Linq;
using DevExpress.XtraBars.Navigation;
namespace DevExpress.DXperience.Demos {
	[AttributeUsage(AttributeTargets.Assembly)]
	public class ProductIdAttribute : Attribute {
		string productId;
		public ProductIdAttribute(string productId) {
			ProductId = productId;
		}
		public string ProductId { get { return productId; } set { productId = value; } }
	}
	public interface ITutorialForm {
		bool IsFullMode { get; }
		void HideServiceElements();
		void ShowServiceElements();
		void ShowDemoFilter();
		bool AllowDemoFilter { get; }
		bool IsDemoFilterVisible { get; }
		void ShowModule(string name);
		void ResetNavbarSelectedLink();
	}
	public class ModuleInfo {
		string fName;
		string fDescription;
		string fUri;
		string fGroup;
		string fTypeName;
		Image fIcon;
		Control fModule;
		bool wasShown;
		int priority;
		string fCodeFile, fXmlFile, fAboutFile;
		Assembly moduleAssembly;
		public ModuleInfo(string fName, string fTypeName) : this(fName, fTypeName, "") { }
		public ModuleInfo(string fName, string fTypeName, string fDescription) : this(fName, fTypeName, fDescription,  null) { }
		public ModuleInfo(string fName, string fTypeName, string fDescription, Image fIcon) : this(fName, fTypeName, fDescription, fIcon, "") { }
		public ModuleInfo(string fName, string fTypeName, string fDescription, Image fIcon, string fGroup) : this(fName, fTypeName, fDescription,  fIcon, fGroup, "", "") { }
		public ModuleInfo(string fName, string fTypeName, string fDescription, Image fIcon, string fCodeFile, string fXmlFile) : this(fName, fTypeName, fDescription,  fIcon, "", fCodeFile, fXmlFile) { }
		public ModuleInfo(string fName, string fTypeName, string fDescription, Image fIcon, string fGroup, string fCodeFile, string fXmlFile) : this(fName, fTypeName, fDescription, fIcon, fGroup, fCodeFile, fXmlFile, "") { }
		public ModuleInfo(string fName, string fTypeName, string fDescription, Image fIcon, string fGroup, string fCodeFile, string fXmlFile, string fAboutFile) : this(fName, fTypeName, fDescription, fIcon, fGroup, fCodeFile, fXmlFile, fAboutFile,""){}
		public ModuleInfo(string fName, string fTypeName, string fDescription, Image fIcon, string fGroup, string fCodeFile, string fXmlFile, string fAboutFile, string fUri) {
			this.fName = fName;
			this.fTypeName = fTypeName;
			this.fIcon = fIcon;
			this.fDescription = fDescription;
			this.fUri = fUri;
			this.fGroup = fGroup;
			this.fCodeFile = fCodeFile;
			this.fXmlFile = fXmlFile;
			this.fAboutFile = fAboutFile;
			this.fModule = null;
		}
		public ModuleInfo(ModuleInfo info)
			: this(info.fName, info.fTypeName, info.fDescription, info.fIcon, info.fGroup, info.fCodeFile, info.fXmlFile, info.fAboutFile, info.Uri) {
		}
		public ModuleInfo(ModuleInfo info, Assembly moduleAssembly)
			: this(info) {
			this.moduleAssembly = moduleAssembly;
		}
		public string Name { get { return this.fName; } }
		public Image Icon { get { return this.fIcon; } }
		public string Description { get { return this.fDescription; } }
		public string Uri { get { return this.fUri; } set { fUri = value; } }
		public string Group { get { return this.fGroup; } set { fGroup = value; } }
		public int Priority { get { return priority; } set { priority = value; } }
		public bool Created { get { return this.fModule != null; } }
		public bool WasShown { get { return wasShown; } set { wasShown = value; } }
		public string CodeFile { get { return this.fCodeFile; } }
		public string XMLFile { get { return this.fXmlFile; } }
		public string AboutFile { get { return this.fAboutFile; } }
		public string TypeName { get { return FullTypeName.Substring(FullTypeName.LastIndexOf('.') + 1); } }
		public string FullTypeName { get { return this.fTypeName; } }
		public Control TModule {
			get {
				if(Uri != string.Empty) {
					if(this.fModule == null) {
						Type fType = (moduleAssembly ?? Assembly.GetExecutingAssembly()).GetType(fTypeName, true);
						ConstructorInfo constructorInfoObj = fType.GetConstructor(new Type[]{typeof(string)});
						if(constructorInfoObj == null)
							throw new ApplicationException(fType.FullName + " doesn't have public constructor with empty parameters");
						try {
							this.fModule = constructorInfoObj.Invoke(new object[]{Uri}) as Control;
						} catch(TargetInvocationException e) {
							string message = "Module " + fType.FullName + " constructor throws an exception:";
							throw new ApplicationException(message + Environment.NewLine, e.InnerException);
						}
					} 
				}
				if(this.fModule == null) {
					Type fType = (moduleAssembly ?? Assembly.GetCallingAssembly()).GetType(fTypeName, true);
					ConstructorInfo constructorInfoObj = fType.GetConstructor(Type.EmptyTypes);
					if(constructorInfoObj == null)
						throw new ApplicationException(fType.FullName + " doesn't have public constructor with empty parameters");
					try {
						this.fModule = constructorInfoObj.Invoke(null) as Control;
					}
					catch(TargetInvocationException e) {
						string message = "Module " + fType.FullName + " constructor throws an exception:";
						throw new ApplicationException(message + Environment.NewLine, e.InnerException);
					}
				}
				return this.fModule;
			}
		}
		public void ResetModule() {
			this.fModule = null;
		}
	}
	class ModuleInfoCollection : CollectionBase {
		public ModuleInfo this[int index] {
			get {
				if (List.Count > index)
					return List[index] as ModuleInfo;
				return null;
			}
		}
		public ModuleInfo this[string fName] {
			get {
				foreach (ModuleInfo info in this)
					if (info.Name.Equals(fName))
						return info;
				return null;
			}
		}
		public void Add(ModuleInfo value) {
			if (List.IndexOf(value) < 0)
				List.Add(value);
		}
		public int IndexOf(ModuleInfo value) {
			return List.IndexOf(value);
		}
	}
	public class ModulesInfo {
		public event EventHandler CurrentModuleChanged;
		static ModulesInfo fInstance;
		ModuleInfoCollection fCollection;
		ModuleInfo fCurrentModule;
		public ModuleInfo CurrentModuleBase { 
			get { return fCurrentModule; } 
			set { 
				fCurrentModule = value;
			} 
		}
		public static void Add(string fName, string fTypeName) {
			ModulesInfo.Add(fName, fTypeName, "");
		}
		public static void Add(string fName, string fTypeName, string fDescription) {
			ModulesInfo.Add(fName, fTypeName, fDescription,  null);
		}
		public static void Add(string fName, string fTypeName, string fDescription, Image fIcon) {
			ModulesInfo.Add(fName, fTypeName, fDescription,  fIcon, "");
		}
		public static void Add(string fName, string fTypeName, string fDescription, Image fIcon, string fGroup) {
			ModuleInfo item = new ModuleInfo(fName, fTypeName, fDescription, fIcon, fGroup);
			fInstance.fCollection.Add(item);
		}
		public static void Add(string fName, string fTypeName, string fDescription,  Image fIcon, string fCodeFile, string fXmlFile) {
			ModulesInfo.Add(fName, fTypeName, fDescription,  fIcon, fCodeFile, fXmlFile, "");
		}
		public static void Add(string fName, string fTypeName, string fDescription, Image fIcon, string fCodeFile, string fXmlFile, string fAboutFile) {
			ModulesInfo.Add(fName, fTypeName, fDescription, fIcon, fCodeFile, fXmlFile, fAboutFile,"");
		}
		public static void Add(string fName, string fTypeName, string fDescription, Image fIcon, string fCodeFile, string fXmlFile, string fAboutFile,string fUri) {
			ModuleInfo item = new ModuleInfo(fName, fTypeName, fDescription, fIcon, "", fCodeFile, fXmlFile, fAboutFile, fUri);
			fInstance.fCollection.Add(item);
		}
		public static void Add(ModuleInfo info) {
			fInstance.fCollection.Add(info);
		}
		public static int Count { get { return fInstance.fCollection.Count; } }
		public static ModuleInfo GetItem(int index) { return fInstance.fCollection[index]; }
		public static ModuleInfo GetItem(string fName) { return fInstance.fCollection[fName]; }
		public static ModuleInfo GetItemByType(string fName) {
			foreach (ModuleInfo mInfo in fInstance.fCollection)
				if (fName.Equals(mInfo.TypeName) || fName.Equals(mInfo.FullTypeName))
					return mInfo;
			return null;
		}
		public static int GetItemIndex(ModuleInfo item) {
			return fInstance.fCollection.IndexOf(item);
		}
		public static ModulesInfo Instance { get { return fInstance; } }
		public static ModuleInfo CurrentModuleInfo { get { return fInstance.fCurrentModule; } }
		public static Control CurrentModule {
			get {
				if (CurrentModuleInfo != null)
					return CurrentModuleInfo.TModule;
				return null;
			}
		}
		static ModulesInfo() {
			fInstance = new ModulesInfo();
		}
		public ModulesInfo() {
			this.fCollection = new ModuleInfoCollection();
			this.fCurrentModule = null;
		}
		public static void ShowModule(Control container, string fName) {
			ModuleInfo item = ModulesInfo.GetItem(fName);
			Cursor currentCursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			if (container.Parent != null) container.Parent.SuspendLayout();
			container.SuspendLayout();
			try {
				Control oldModule = null;
				if (Instance.CurrentModuleBase != null)
					oldModule = Instance.CurrentModuleBase.TModule;
				Control fModule = item.TModule as Control;
				fModule.Bounds = container.DisplayRectangle;
				Instance.CurrentModuleBase = item;
				fModule.Visible = false;
				container.Controls.Add(fModule);
				fModule.Dock = DockStyle.Fill;
				fModule.Visible = true;
				if (oldModule != null) oldModule.Visible = false;
			}
			finally {
				container.ResumeLayout(true);
				if (container.Parent != null) container.Parent.ResumeLayout(true);
				Cursor.Current = currentCursor;
			}
			RaiseModuleChanged();
		}
		protected static void RaiseModuleChanged() {
			if (Instance.CurrentModuleChanged != null)
				Instance.CurrentModuleChanged(Instance, EventArgs.Empty);
		}
		public static void FillListBox(DevExpress.XtraEditors.ListBoxControl listBox) {
			if (listBox == null) return;
			for (int i = 0; i < Count; i++) {
				listBox.Items.Add(GetItem(i).Name);
			}
		}
		static bool NameExist(string[] names, string name) {
			foreach (string s in names)
				if (s == name) return true;
			return false;
		}
		public static void FillListBox(DevExpress.XtraEditors.ListBoxControl listBox, string[] names) {
			if (listBox == null) return;
			for (int i = 0; i < Count; i++) {
				if (!NameExist(names, GetItem(i).Name))
					listBox.Items.Add(GetItem(i).Name);
			}
		}
		public static void FillNavBar(NavBarControl navBar) {
			FillNavBar(navBar, NavBarGroupStyle.SmallIconsList);
		}
		public static void FillNavBar(NavBarControl navBar, NavBarGroupStyle groupStyle) {
			FillNavBar(navBar, groupStyle, true);
		}
		public static void FillNavBar(NavBarControl navBar, NavBarGroupStyle groupStyle, bool showOutdated) {
			FillNavBar(navBar, groupStyle, showOutdated, NavBarImage.Default);
		}
		public static void FillNavBar(NavBarControl navBar, NavBarGroupStyle groupStyle, bool showOutdated, NavBarImage groupCaptionImage) {
			if (navBar == null) return;
			navBar.BeginUpdate();
			for (int i = 0; i < Count; i++) {
				if (GetItem(i).Group == "About" || GetItem(i).Group.IndexOf("Outdated") != -1) continue;
				NavBarItem item = new NavBarItem();
				navBar.Items.Add(item);
				item.Caption = GetItem(i).Name;
				item.SmallImage = item.LargeImage = GetItem(i).Icon;
				item.Tag = GetItem(i);
				GetNavBarGroup(navBar, GetItem(i).Group, groupStyle, showOutdated, groupCaptionImage).ItemLinks.Add(new NavBarItemLink(item));
			}
			navBar.EndUpdate();
		}
		static NavBarGroup GetNavBarGroup(NavBarControl navBar, string groupName, NavBarGroupStyle groupStyle, bool showOutdated, NavBarImage groupCaptionImage) {
			foreach (NavBarGroup group in navBar.Groups)
				if (group.Caption == groupName) return group;
			NavBarGroup newGroup = navBar.Groups.Add();
			newGroup.Caption = groupName;
			newGroup.GroupStyle = groupStyle;
			newGroup.Expanded = groupName.IndexOf("Outdated") == -1;
			newGroup.GroupCaptionUseImage = groupCaptionImage;
			if (!showOutdated && !newGroup.Expanded)
				newGroup.Visible = false;
			return newGroup;
		}
		public static void FillAccordionControl(AccordionControl accordionControl) {
			FillAccordionControl(accordionControl, NavBarGroupStyle.SmallIconsList);
		}
		public static void FillAccordionControl(AccordionControl accordionControl, NavBarGroupStyle groupStyle) {
			FillAccordionControl(accordionControl, groupStyle, true);
		}
		public static void FillAccordionControl(AccordionControl accordionControl, NavBarGroupStyle groupStyle, bool showOutdated) {
			FillAccordionControl(accordionControl, groupStyle, showOutdated, NavBarImage.Default);
		}
		public static void FillAccordionControl(AccordionControl accordionControl, NavBarGroupStyle groupStyle, bool showOutdated, NavBarImage groupCaptionImage) {
			if(accordionControl == null) return;
			accordionControl.BeginUpdate();
			for(int i = 0; i < Count; i++) {
				if(GetItem(i).Group == "About" || GetItem(i).Group.IndexOf("Outdated") != -1) continue;
				AccordionControlElement item = new AccordionControlElement();
				item.Style = ElementStyle.Item;
				item.Text = GetItem(i).Name;
				item.Tag = GetItem(i);
				GetAccordionControlGroup(accordionControl, GetItem(i).Group, showOutdated).Elements.Add(item);
			}
			accordionControl.EndUpdate();
		}
		static AccordionControlElement GetAccordionControlGroup(AccordionControl accordionControl, string groupName, bool showOutdated) {
			foreach(AccordionControlElement group in accordionControl.Elements)
				if(group.Text == groupName) return group;
			AccordionControlElement newGroup = new AccordionControlElement();
			accordionControl.Elements.Add(newGroup);
			newGroup.Text = groupName;
			newGroup.Expanded = groupName.IndexOf("Outdated") == -1;
			if(!showOutdated && !newGroup.Expanded)
				newGroup.Visible = false;
			return newGroup;
		}
	}
	[Flags]
	public enum ExportFormats {
		None = 0,
		PDF = 0x01,
		EPUB = 0x02,
		XML = 0x04,
		HTML = 0x08,
		MHT = 0x10,
		DOC = 0x20,
		DOCX = 0x40,
		XLS = 0x80,
		XLSX = 0x100,
		RTF = 0x200,
		ODT = 0x400,
		Image = 0x800,
		Text = 0x1000,
		ImageEx = 0x2000,
		All = 0xFFFF
	}
	[ToolboxItem(false)]
	public class TutorialControlBase : DevExpress.Tutorials.ModuleBase {
		RibbonMenuManager manager;
		private DevExpress.Utils.Frames.ApplicationCaption fCaption = null;
		private string fName = string.Empty;
		public TutorialControlBase() {
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			this.LookAndFeel.StyleChanged += new EventHandler(LookAndFeel_StyleChanged);
		}
		internal string FullTypeName { get { return this.GetType().FullName; } }
		public virtual RibbonMenuManager RibbonMenuManager {
			get { return manager; }
			set {
				manager = value;
				if (manager != null)
					AddMenuManager(manager.Manager);
			}
		}
		void LookAndFeel_StyleChanged(object sender, EventArgs e) {
			OnStyleChanged();
		}
		[DefaultValue(false)]
		public virtual bool AutoMergeRibbon {
			get;
			set;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		protected virtual RibbonControl ChildRibbon { 
			get {
				if(!AutoMergeRibbon) return null;
				return FindRibbon(Controls);
			} 
		}
		private RibbonControl FindRibbon(ControlCollection controls) {
			RibbonControl res = controls.OfType<Control>().FirstOrDefault(x => x is RibbonControl) as RibbonControl;
			if(res != null) return res;
			foreach(Control control in controls) {
				if(control.HasChildren) {
					res = FindRibbon(control.Controls);
					if(res != null) return res;
				}
			}
			return null;
		}
		public void CreateWaitDialog() {
			SplashScreenManager.ShowForm(typeof(DevExpress.XtraWaitForm.DemoWaitForm), false, true);
			SplashScreenManager.Default.SetWaitFormCaption(DevExpress.Tutorials.Properties.Resources.WaitCaption);
			SplashScreenManager.Default.SetWaitFormDescription(DevExpress.Tutorials.Properties.Resources.WaitDescription);
		}
		public void SetWaitDialogCaption(string description) {
			if(SplashScreenManager.Default != null)
				SplashScreenManager.Default.SetWaitFormDescription(description);
		}
		protected bool start = true;
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			if(SplashScreenManager.Default != null) SplashScreenManager.CloseForm();
		}
		protected void CreateTimer() {
			if (!start) return;
			start = false;
			Timer timer = new Timer();
			timer.Interval = 500;
			timer.Tick += new EventHandler(OnTick);
			timer.Start();
		}
		void OnTick(object sender, EventArgs e) {
			((Timer)sender).Stop();
			OnTick();
		}
		protected virtual void OnTick() {
		}
		public string TutorialName {
			get { return fName; }
			set { fName = value; }
		}
		public DevExpress.Utils.Frames.ApplicationCaption Caption {
			get { return fCaption; }
			set {
				fCaption = value;
				OnSetCaption("");
			}
		}
		public virtual bool NoGap { get { return false; } }
		public virtual bool AllowPrintOptions { get { return false; } }
		public virtual bool HasActiveDemo { get { return false; } }
		public virtual void RunActiveDemo() { }
		protected virtual void OnSetCaption(string fCaption) {
			Caption.Text = string.Format("{0}", TutorialName);
		}
		protected virtual void OnSwitchStyle() { }
		protected virtual void OnStyleChanged() { }
		protected override void OnVisibleChanged(object sender, EventArgs e) {
			base.OnVisibleChanged(sender, e);
			if (this.Visible) {
				ShowBar();
				DoShow();
			}
			else {
				DoHide();
				HideBar();
			}
		}
		protected virtual void DoShow() {
			AllowExport();
			if(AutoMergeRibbon && ChildRibbon != null && ParentFormMain != null) {
				ParentFormMain.MergeRibbon(ChildRibbon);
			}
			if(ParentFormMain != null) SetExportBarItemAvailability(ParentFormMain.ShowInVisualStudio, false, false);
		}
		protected virtual void DoHide() {
			if(AutoMergeRibbon && ParentFormMain != null) {
				if(ParentFormMain.RibbonControl.MergedRibbon == ChildRibbon)
					ParentFormMain.UnMergeRibbon();
			}
		}
		protected internal virtual bool AllowAppearanceGroup { get { return true; } }
		protected internal virtual bool ShowCaption { get { return true; } }
		#region BarsInfo
		protected ArrayList BarInfos = new ArrayList();
		protected virtual BarManager Manager { get { return null; } }
		protected virtual string BarName { get { return string.Empty; } }
		Bar Bar {
			get {
				foreach (Bar bar in Manager.Bars)
					if (bar.BarName == BarName) return bar;
				return null;
			}
		}
		protected virtual void InitBarInfo() { }
		void ShowBar() {
			if (BarName == string.Empty || Manager == null) return;
			InitBars();
			Bar.Visible = true;
		}
		void HideBar() {
			if (BarName == string.Empty || Manager == null) return;
			Bar.Visible = false;
		}
		void CreateBar() {
			Bar bar = new Bar(Manager);
			bar.BarName = bar.Text = BarName;
			bar.DockStyle = BarDockStyle.Top;
			foreach (BarInfo info in BarInfos) {
				BarItem item = info.CreateItem(Manager);
				bar.AddItem(item).BeginGroup = info.BeginGroup;
			}
		}
		protected BarItem GetBarItem(int index) {
			InitBars();
			if (BarInfos.Count <= index) return null;
			return ((BarInfo)BarInfos[index]).BarItem;
		}
		protected bool GetBarItemPushed(int index) {
			InitBars();
			if (BarInfos.Count <= index) return false;
			return ((BarInfo)BarInfos[index]).Pushed;
		}
		protected void SetBarItemEnabled(int index, bool enabled) {
			InitBars();
			if (BarInfos.Count <= index) return;
			((BarInfo)BarInfos[index]).Enabled = enabled;
		}
		protected void SetBarItemChecked(int index, bool pushed) {
			InitBars();
			if (BarInfos.Count <= index) return;
			((BarInfo)BarInfos[index]).Pushed = pushed;
		}
		void InitBars() {
			if (BarInfos.Count == 0) InitBarInfo();
			if (Manager != null && Bar == null) CreateBar();
		}
		#endregion
		#region RibbonMainForm Print and Export
		public RibbonMainForm ParentFormMain {
			get {
				return this.FindForm() as RibbonMainForm;
			}
		}
		protected virtual void AllowExport() {
			EnabledPrintExportActions(false, ExportFormats.None, true);
		}
		protected internal void EnabledPrintExportActions(bool allowPrintPreview, ExportFormats formats, bool showDisabledButtons) {
			EnabledPrintExportActions(allowPrintPreview, false, formats, showDisabledButtons);
		}
		public void EnabledPrintExportActions(bool allowPrintPreview, bool allowPrint, ExportFormats formats, bool showDisabledButtons) {
			if (ParentFormMain == null)
				return;
			ParentFormMain.PrintPreviewButton.Enabled = allowPrintPreview;
			ParentFormMain.PrintButton.Visibility = GetVisibility(allowPrint);
			SetExportBarItemAvailability(ParentFormMain.ExportToPDFButton, (formats & ExportFormats.PDF) == ExportFormats.PDF, showDisabledButtons);
			SetExportBarItemAvailability(ParentFormMain.ExportToEPUBButton, (formats & ExportFormats.EPUB) == ExportFormats.EPUB, showDisabledButtons);
			SetExportBarItemAvailability(ParentFormMain.ExportToXMLButton, (formats & ExportFormats.XML) == ExportFormats.XML, showDisabledButtons);
			SetExportBarItemAvailability(ParentFormMain.ExportToHTMLButton, (formats & ExportFormats.HTML) == ExportFormats.HTML, showDisabledButtons);
			SetExportBarItemAvailability(ParentFormMain.ExportToMHTButton, (formats & ExportFormats.MHT) == ExportFormats.MHT, showDisabledButtons);
			SetExportBarItemAvailability(ParentFormMain.ExportToDOCButton, (formats & ExportFormats.DOC) == ExportFormats.DOC, showDisabledButtons);
			SetExportBarItemAvailability(ParentFormMain.ExportToDOCXButton, (formats & ExportFormats.DOCX) == ExportFormats.DOCX, showDisabledButtons);
			SetExportBarItemAvailability(ParentFormMain.ExportToXLSButton, (formats & ExportFormats.XLS) == ExportFormats.XLS, showDisabledButtons);
			SetExportBarItemAvailability(ParentFormMain.ExportToXLSXButton, (formats & ExportFormats.XLSX) == ExportFormats.XLSX, showDisabledButtons);
			SetExportBarItemAvailability(ParentFormMain.ExportToRTFButton, (formats & ExportFormats.RTF) == ExportFormats.RTF, showDisabledButtons);
			SetExportBarItemAvailability(ParentFormMain.ExportToODTButton, (formats & ExportFormats.ODT) == ExportFormats.ODT, showDisabledButtons);
			SetExportBarItemAvailability(ParentFormMain.ExportToImageButton, (formats & ExportFormats.Image) == ExportFormats.Image, showDisabledButtons);
			SetExportBarItemAvailability(ParentFormMain.ExportToImageExButton, (formats & ExportFormats.ImageEx) == ExportFormats.ImageEx, showDisabledButtons);
			SetExportBarItemAvailability(ParentFormMain.ExportToTextButton, (formats & ExportFormats.Text) == ExportFormats.Text, showDisabledButtons);
			ParentFormMain.ExportButton.Enabled = formats != ExportFormats.None;
		}
		protected internal virtual void SetExportBarItemAvailability(BarItem button, bool isEnabled, bool showDisabled) {
			button.Enabled = isEnabled;
			button.Visibility = GetVisibility(isEnabled || showDisabled);
		}
		protected internal virtual BarItemVisibility GetVisibility(bool isVisible) {
			return isVisible ? BarItemVisibility.Always : BarItemVisibility.Never;
		}
		protected internal virtual void PrintPreview() { }
		protected internal virtual void Print() { }
		protected internal virtual void ExportTo(string ext, string filter) {
			string fileName = MainFormHelper.GetFileName(string.Format("*.{0}", ext), filter);
			if (!String.IsNullOrEmpty(fileName))
				try {
					ExportToCore(fileName, ext);
					MainFormHelper.OpenExportedFile(fileName);
				}
				catch(Exception e) {
					MainFormHelper.ShowExportErrorMessage(e);
				}
		}
		protected internal virtual void ExportToCore(String filename, string ext) { }
		protected internal virtual void ExportToPDF() { }
		protected internal virtual void ExportToEPUB() { }
		protected internal virtual void ExportToXML() { }
		protected internal virtual void ExportToHTML() { }
		protected internal virtual void ExportToMHT() { }
		protected internal virtual void ExportToDOC() { }
		protected internal virtual void ExportToDOCX() { }
		protected internal virtual void ExportToXLS() { }
		protected internal virtual void ExportToXLSX() { }
		protected internal virtual void ExportToRTF() { }
		protected internal virtual void ExportToODT() { }
		protected internal virtual void ExportToImage() { }
		protected internal virtual void ExportToText() { }
		#endregion
		public virtual bool SetNewWhatsThisPadding { get { return true; } }
		public override void StartWhatsThis() {
			if(SetNewWhatsThisPadding) {
				this.Padding = new Padding(8);
				this.Refresh();
			}
		}
		public override void EndWhatsThis() {
			if(SetNewWhatsThisPadding)
				this.Padding = new Padding(0);
		}
		protected override void OnParentChanged(EventArgs e) {
			base.OnParentChanged(e);
			if (NoGap && Parent != null && Parent.Parent != null)
				Parent.Parent.Padding = new Padding(0);
		}
	}
	public class ButtonBarItem : BarButtonItem {
		public ButtonBarItem(BarManager manager, string text, ItemClickEventHandler handler) : this(manager, text, -1, handler) { }
		public ButtonBarItem(BarManager manager, string text, int imageIndex, ItemClickEventHandler handler) {
			this.Manager = manager;
			this.Caption = text;
			this.ImageIndex = imageIndex;
			this.ItemClick += handler;
		}
	}
	public class OptionBarItem : BarCheckItem {
		bool optionItem = false;
		public OptionBarItem(BarManager manager, string text, ItemClickEventHandler handler, object tag) : this(manager, text, handler, tag, true) { }
		public OptionBarItem(BarManager manager, string text, ItemClickEventHandler handler) : this(manager, text, handler, null, true) { }
		public OptionBarItem(BarManager manager, string text, ItemClickEventHandler handler, object tag, bool optionItem) {
			this.optionItem = optionItem;
			this.Manager = manager;
			this.Caption = text;
			this.Tag = tag;
			this.ItemClick += handler;
		}
		public bool IsOption { get { return this.optionItem; } }
	}
	public class CheckBarItem : OptionBarItem {
		ActiveLookAndFeelStyle style;
		public CheckBarItem(BarManager manager, string text, ItemClickEventHandler handler) : this(manager, text, handler, ActiveLookAndFeelStyle.Flat) { }
		public CheckBarItem(BarManager manager, string text, ItemClickEventHandler handler, ActiveLookAndFeelStyle style)
			: base(manager, text, handler, false) {
			this.style = style;
		}
		public ActiveLookAndFeelStyle Style { get { return style; } }
	}
	public class CheckBarItemWithStyle : CheckBarItem {
		LookAndFeelStyle lfStyle;
		public CheckBarItemWithStyle(BarManager manager, string text, ItemClickEventHandler handler, ActiveLookAndFeelStyle style, LookAndFeelStyle lfStyle)
			: base(manager, text, handler, style) {
			this.lfStyle = lfStyle;
		}
		public LookAndFeelStyle LookAndFeelStyle { get { return lfStyle; } }
	}
	public class LookAndFeelMenu : IDisposable {
		protected string about;
		DefaultLookAndFeel lookAndFeel;
		protected BarManager manager;
		protected BarSubItem miLookAndFeel, miHelp, miView;
		protected BarButtonItem miSkin;
		protected ButtonBarItem miFeatures, miRate;
		GalleryDropDown skinGallery;
		protected CheckBarItem miAllowFormSkins, miFullViewMode, miShowNavBarFilter;
		public ButtonBarItem miAboutProduct;
		public event EventHandler BeginSkinChanging;
		public event EventHandler EndSkinChanging;
		private void RaiseEndSkinChanging() {
			if (EndSkinChanging != null) EndSkinChanging(this, EventArgs.Empty);
		}
		private void RaiseBeginSkinChanging() {
			if (BeginSkinChanging != null) BeginSkinChanging(this, EventArgs.Empty);
		}
		ITutorialForm MainForm {
			get {
				return MenuForm as ITutorialForm;
			}
		}
		public Form MenuForm {
			get {
				if (this.manager != null) return this.manager.Form as Form;
				return null;
			}
		}
		public Bar MainMenu {
			get {
				if (this.manager != null) return this.manager.Bars["Main Menu"];
				return null;
			}
		}
		public LookAndFeelMenu(BarManager manager, DefaultLookAndFeel lookAndFeel, string about) {
			if (manager == null || manager.Form == null) return;
			this.about = about;
			this.lookAndFeel = lookAndFeel;
			this.manager = manager;
			this.manager.Images = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.Tutorials.MainDemo.menu.bmp", typeof(LookAndFeelMenu).Assembly, new Size(16, 16), Color.Magenta);
			this.manager.ForceInitialize();
			this.manager.BeginUpdate();
			SetupMenu();
			this.manager.EndUpdate();
		}
		protected virtual string ProductName { get { return "Product"; } }
		void SetupMenu() {
			skinGallery = new GalleryDropDown();
			SkinHelper.InitSkinGalleryDropDown(skinGallery);
			skinGallery.Manager = this.manager;
			miLookAndFeel = new BarSubItem(this.manager, "&Look and Feel");
			miAllowFormSkins = new CheckBarItem(this.manager, "Allow Form Skins", new ItemClickEventHandler(OnSwitchFormSkinStyle_Click));
			miLookAndFeel.ItemLinks.Add(miAllowFormSkins);
			miLookAndFeel.ItemLinks.Add(new CheckBarItemWithStyle(this.manager, "&Native", new ItemClickEventHandler(OnSwitchStyle_Click), ActiveLookAndFeelStyle.WindowsXP, LookAndFeelStyle.Skin));
			miLookAndFeel.ItemLinks.Add(new CheckBarItemWithStyle(this.manager, "&Flat", new ItemClickEventHandler(OnSwitchStyle_Click), ActiveLookAndFeelStyle.Flat, LookAndFeelStyle.Flat));
			miLookAndFeel.ItemLinks.Add(new CheckBarItemWithStyle(this.manager, "&Ultra Flat", new ItemClickEventHandler(OnSwitchStyle_Click), ActiveLookAndFeelStyle.UltraFlat, LookAndFeelStyle.UltraFlat));
			miLookAndFeel.ItemLinks.Add(new CheckBarItemWithStyle(this.manager, "&Style3D", new ItemClickEventHandler(OnSwitchStyle_Click), ActiveLookAndFeelStyle.Style3D, LookAndFeelStyle.Style3D));
			miLookAndFeel.ItemLinks.Add(new CheckBarItemWithStyle(this.manager, "&Office2003", new ItemClickEventHandler(OnSwitchStyle_Click), ActiveLookAndFeelStyle.Office2003, LookAndFeelStyle.Office2003));
			miSkin = new BarButtonItem(this.manager, "S&kins");
			miSkin.ButtonStyle = BarButtonStyle.DropDown;
			miSkin.DropDownControl = skinGallery;
			miSkin.ActAsDropDown = true;
			miView = new BarSubItem(this.manager, "&View");
			miHelp = new BarSubItem(this.manager, "&Help");
			miFullViewMode = new CheckBarItem(this.manager, "Full-Window Mode", new ItemClickEventHandler(OnFullViewModeClick));
			miFullViewMode.ItemShortcut = new BarShortcut(Shortcut.F11);
			miShowNavBarFilter = new CheckBarItem(this.manager, "Show Demo Filter", new ItemClickEventHandler(OnDemoFilterClick));
			miShowNavBarFilter.ItemShortcut = new BarShortcut(Shortcut.F3);
			miView.ItemLinks.Add(miFullViewMode);
			if (MainForm != null && MainForm.AllowDemoFilter) miView.ItemLinks.Add(miShowNavBarFilter);
			miLookAndFeel.Popup += new EventHandler(OnPopupLookAndFeel);
			miView.Popup += new EventHandler(OnView);
			miLookAndFeel.ItemLinks.Insert(1, miSkin).BeginGroup = true;
			miRate = new ButtonBarItem(this.manager, "&Rate this demo...", 3, new ItemClickEventHandler(biRateDemo_Click));
			miHelp.ItemLinks.Add(new ButtonBarItem(this.manager, string.Format("{0} Web Page", ProductName), 0, new ItemClickEventHandler(biProductWebPage_Click)));
			miFeatures = new ButtonBarItem(this.manager, string.Format("{0} Features", ProductName), new ItemClickEventHandler(miFeatures_Click));
			miFeatures.ItemShortcut = new BarShortcut(Shortcut.F1);
			miHelp.ItemLinks.Add(miFeatures).BeginGroup = true;
			miAboutProduct = new ButtonBarItem(this.manager, string.Format("About {0}", ProductName), new ItemClickEventHandler(miAboutProduct_Click));
			miHelp.ItemLinks.Add(miAboutProduct);
			if (this.MenuForm != null)
				this.MenuForm.Disposed += new EventHandler(form_Dispose);
			AddItems();
		}
		protected virtual void OnSwitchFormSkinStyle_Click(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			RaiseBeginSkinChanging();
			if (DevExpress.Skins.SkinManager.AllowFormSkins)
				DevExpress.Skins.SkinManager.DisableFormSkins();
			else DevExpress.Skins.SkinManager.EnableFormSkins();
			DevExpress.LookAndFeel.LookAndFeelHelper.ForceDefaultLookAndFeelChanged();
			RaiseEndSkinChanging();
		}
		public void EnabledLookFeelMenu(bool enable) {
			this.miLookAndFeel.Enabled = enable;
		}
		internal void SetTutorialsMenu() {
			miRate.Visibility = BarItemVisibility.Never;
			miFeatures.Visibility = BarItemVisibility.Never;
			miAboutProduct.ItemShortcut = new BarShortcut(Shortcut.F1);
		}
		void biRateDemo_Click(object sender, ItemClickEventArgs e) {
			Form mainForm = this.MenuForm as Form;
			if (mainForm != null) {
				RatingForm ratingForm = new RatingForm(mainForm);
				ratingForm.ShowDialog(mainForm);
			}
		}
		protected virtual void biProductWebPage_Click(object sender, ItemClickEventArgs e) {
		}
		void biMyDevExpress_Click(object sender, ItemClickEventArgs e) {
			System.Diagnostics.Process.Start(RibbonMainForm.GetStartedLink);
		}
		void biProducts_Click(object sender, ItemClickEventArgs e) {
			System.Diagnostics.Process.Start("http://www.devexpress.com/products");
		}
		void biDownloads_Click(object sender, ItemClickEventArgs e) {
			System.Diagnostics.Process.Start("http://www.devexpress.com/downloads");
		}
		void biForum_Click(object sender, ItemClickEventArgs e) {
			System.Diagnostics.Process.Start(AssemblyInfo.DXLinkGetSupport);
		}
		void biDeveloperExpress_Click(object sender, ItemClickEventArgs e) {
			System.Diagnostics.Process.Start("http://www.devexpress.com");
		}
		void biKnowledgeBase_Click(object sender, ItemClickEventArgs e) {
			System.Diagnostics.Process.Start("http://www.devexpress.com/Support/KnowledgeBase/");
		}
		protected virtual void miAboutProduct_Click(object sender, ItemClickEventArgs e) {
		}
		protected virtual void miFeatures_Click(object sender, ItemClickEventArgs e) {
			ITutorialForm frm = this.manager.Form as ITutorialForm;
			if (frm == null) return;
			frm.ShowModule(ModulesInfo.GetItem(0).Name);
			frm.ResetNavbarSelectedLink();
		}
		private void form_Dispose(object sender, EventArgs e) {
			miView.Popup -= new EventHandler(OnView);
			this.Dispose();
		}
		public virtual void Dispose() {
			if (MenuForm != null) MenuForm.Disposed -= new EventHandler(form_Dispose);
			this.lookAndFeel = null;
			this.miLookAndFeel = null;
		}
		public DefaultLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		protected virtual void AddItems() {
			if (MainMenu == null) return;
			MainMenu.ItemLinks.Add(miLookAndFeel);
			MainMenu.ItemLinks.Add(miView);
			MainMenu.ItemLinks.Add(miHelp);
			InitLookAndFeelMenu();
		}
		public void InitLookAndFeelMenu() {
			InitLookAndFeelMenu(LookAndFeel);
		}
		private bool UsingXP {
			get { return LookAndFeel.LookAndFeel.UseWindowsXPTheme && DevExpress.Utils.WXPaint.Painter.ThemesEnabled; }
		}
		protected virtual bool Mixed { get { return false; } }
		bool AvailableStyle(LookAndFeelStyle style) {
			return lookAndFeel.LookAndFeel.Style == style && !UsingXP && !Mixed;
		}
		public void InitLookAndFeelMenu(DefaultLookAndFeel lookAndFeel) {
			this.lookAndFeel = lookAndFeel;
			miLookAndFeel.Visibility = lookAndFeel != null ? BarItemVisibility.Always : BarItemVisibility.Never;
			InitLookAndFeelEnabled();
		}
		protected virtual void InitLookAndFeelEnabled() {
			if (lookAndFeel == null) return;
			foreach (BarItemLink item in miLookAndFeel.ItemLinks) {
				CheckBarItemWithStyle aItem = item.Item as CheckBarItemWithStyle;
				if (aItem != null && aItem.LookAndFeelStyle == LookAndFeelStyle.Skin)
					aItem.Enabled = DevExpress.Utils.WXPaint.Painter.ThemesEnabled;
			}
		}
		void OnView(object sender, EventArgs e) {
			miFullViewMode.Checked = MainForm != null && MainForm.IsFullMode;
			miShowNavBarFilter.Checked = MainForm.IsDemoFilterVisible;
			miShowNavBarFilter.Enabled = !MainForm.IsFullMode;
		}
		void OnFullViewModeClick(object sender, EventArgs e) {
			if (MainForm == null) return;
			if (MainForm.IsFullMode) MainForm.ShowServiceElements();
			else MainForm.HideServiceElements();
		}
		void OnDemoFilterClick(object sender, EventArgs e) {
			if (MainForm == null) return;
			MainForm.ShowDemoFilter();
		}
		void OnPopupSkinNames(object sender, EventArgs e) {
			BarSubItem items = sender as BarSubItem;
			if (items == null || LookAndFeel == null) return;
			foreach (BarItemLink item in items.ItemLinks) {
				CheckBarItem aItem = item.Item as CheckBarItem;
				if (aItem != null)
					aItem.Checked = AvailableStyle(LookAndFeelStyle.Skin) && LookAndFeel.LookAndFeel.SkinName == item.Caption;
			}
		}
		void OnPopupLookAndFeel(object sender, EventArgs e) {
			BarSubItem items = sender as BarSubItem;
			if (items == null || LookAndFeel == null) return;
			foreach (BarItemLink item in items.ItemLinks) {
				CheckBarItemWithStyle aItem = item.Item as CheckBarItemWithStyle;
				if (aItem != null) {
					if (aItem.LookAndFeelStyle == LookAndFeelStyle.Skin)
						aItem.Checked = UsingXP && !Mixed;
					else
						aItem.Checked = AvailableStyle(aItem.LookAndFeelStyle);
				}
			}
			miAllowFormSkins.Checked = DevExpress.Skins.SkinManager.AllowFormSkins;
		}
		protected void AddOptionsMenu(BarSubItem miItem, object options, ItemClickEventHandler handler) {
			AddOptionsMenu(miItem, options, handler, this.manager);
		}
		public static void AddOptionsMenu(BarSubItem miItem, object options, ItemClickEventHandler handler, BarManager manager) {
			miItem.Visibility = options != null ? BarItemVisibility.Always : BarItemVisibility.Never;
			if (options == null) return;
			ArrayList arr = DevExpress.Utils.SetOptions.GetOptionNames(options);
			for (int i = 0; i < arr.Count; i++)
				miItem.ItemLinks.Add(new OptionBarItem(manager, ResourcesKeeper.GetTitle(arr[i]), handler, arr[i]));
			InitOptionsMenu(miItem, options);
		}
		public static void InitOptionsMenu(BarSubItem miItem, object options) {
			for (int i = 0; i < miItem.ItemLinks.Count; i++) {
				OptionBarItem item = miItem.ItemLinks[i].Item as OptionBarItem;
				if (item != null)
					item.Checked = DevExpress.Utils.SetOptions.OptionValueByString(item.Tag.ToString(), options);
			}
		}
		protected void ClearOptionItems() {
			ClearOptionItems(this.manager);
		}
		public static void ClearOptionItems(BarManager manager) {
			for (int i = manager.Items.Count - 1; i >= 0; i--) {
				OptionBarItem item = manager.Items[i] as OptionBarItem;
				if (item != null && item.IsOption) item.Dispose();
			}
		}
		protected void OpenFile(string fileName) {
			if (DevExpress.XtraEditors.XtraMessageBox.Show(DevExpress.Tutorials.Properties.Resources.OpenFileQuestion, DevExpress.Tutorials.Properties.Resources.ExportCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
				try {
					System.Diagnostics.Process process = new System.Diagnostics.Process();
					process.StartInfo.FileName = fileName;
					process.StartInfo.Verb = "Open";
					process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
					process.Start();
				}
				catch {
					DevExpress.XtraEditors.XtraMessageBox.Show(DevExpress.Tutorials.Properties.Resources.ApplicationOpenWarning, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}
		protected string ShowSaveFileDialog(string title, string filter) {
			SaveFileDialog dlg = new SaveFileDialog();
			string name = Application.ProductName;
			int n = name.LastIndexOf(".") + 1;
			if (n > 0) name = name.Substring(n, name.Length - n);
			dlg.Title = string.Format(DevExpress.Tutorials.Properties.Resources.ExportTo, title);
			dlg.FileName = name;
			dlg.Filter = filter;
			if (dlg.ShowDialog() == DialogResult.OK) return dlg.FileName;
			return "";
		}
		void OnSwitchSkin(object sender, ItemClickEventArgs e) {
			OnSwitchStyle_Click(sender, e);
			if (LookAndFeel != null) {
				RaiseBeginSkinChanging();
				LookAndFeel.LookAndFeel.SetSkinStyle(((CheckBarItem)e.Item).Caption);
				RaiseEndSkinChanging();
			}
		}
		protected virtual void OnSwitchStyle_Click(object sender, ItemClickEventArgs e) {
			this.MenuForm.SuspendLayout();
			RaiseBeginSkinChanging();
			try {
				Application.DoEvents();
				CheckBarItem item = e.Item as CheckBarItem;
				if (item == null || LookAndFeel == null) return;
				bool wxp = item.Style == ActiveLookAndFeelStyle.WindowsXP;
				if (item.Style != ActiveLookAndFeelStyle.Skin)
					LookAndFeel.LookAndFeel.SetStyle((LookAndFeelStyle)item.Style, wxp, LookAndFeel.LookAndFeel.UseDefaultLookAndFeel, LookAndFeel.LookAndFeel.SkinName);
				InitLookAndFeelMenu();
				Application.DoEvents();
			}
			finally {
				RaiseEndSkinChanging();
				this.MenuForm.ResumeLayout(true);
			}
		}
		public string About {
			get { return about; }
			set { about = value; }
		}
		protected void miAbout_Click(object sender, ItemClickEventArgs e) {
			DevExpress.Utils.About.frmAbout dlg = new DevExpress.Utils.About.frmAbout(about == "" ? "DXperience by Developer Express inc." : about);
			dlg.ShowDialog();
			dlg.Dispose();
		}
	}
	public class BarInfo {
		string caption;
		ItemClickEventHandler handler;
		Image image;
		bool isCheckItem, check, fBeginGroup;
		BarInfo[] info;
		int gIndex;
		public BarInfo(string caption, ItemClickEventHandler handler, Image image, bool isCheckItem, bool check, bool beginGroup) : this(caption, handler, image, isCheckItem, check, beginGroup, null, -1) { }
		public BarInfo(string caption, ItemClickEventHandler handler, Image image, bool isCheckItem, bool check, bool beginGroup, BarInfo[] info, int gIndex) {
			this.caption = caption;
			this.handler = handler;
			this.image = image;
			this.isCheckItem = isCheckItem;
			this.check = check;
			this.fBeginGroup = beginGroup;
			this.info = info;
			this.gIndex = gIndex;
		}
		public bool BeginGroup { get { return fBeginGroup; } }
		BarItem item = null;
		public BarItem CreateItem(BarManager manager) {
			return CreateItem(manager, -1);
		}
		public BarItem CreateItem(BarManager manager, int groupIndex) {
			if (isCheckItem) {
				item = new BarCheckItem(manager, this.check);
				item.Caption = caption;
				if (groupIndex != -1) ((BarCheckItem)item).GroupIndex = groupIndex;
			}
			else item = new BarButtonItem(manager, caption);
			if (info != null) {
				BarButtonItem buttonItem = item as BarButtonItem;
				if (buttonItem != null) {
					buttonItem.ButtonStyle = BarButtonStyle.DropDown;
					PopupMenu menu = new PopupMenu(manager);
					foreach (BarInfo bInfo in info)
						menu.ItemLinks.Add(bInfo.CreateItem(manager, this.gIndex));
					buttonItem.DropDownControl = menu;
				}
			}
			item.ItemClick += handler;
			item.Glyph = image;
			item.Hint = caption;
			return item;
		}
		public bool Pushed {
			get {
				BarCheckItem checkItem = item as BarCheckItem;
				if (checkItem == null) return false;
				return checkItem.Checked;
			}
			set {
				BarCheckItem checkItem = item as BarCheckItem;
				if (checkItem == null) return;
				checkItem.Checked = value;
			}
		}
		public bool Enabled {
			get { return item.Enabled; }
			set { item.Enabled = value; }
		}
		public BarItem BarItem { get { return item; } }
	}
	public class DemoHelper {
		public static string StartModuleParameter = "/start:";
		public static string FullWindowModeParameter = "/fullwindow";
		public static string StringComposite(string str1, string str2) {
			string[] arr = str1.Split(';');
			string ret = string.Empty;
			foreach (string str in arr) {
				if (ret != string.Empty) ret += ";";
				ret += str2 + str;
			}
			return ret;
		}
		internal static string GetModuleName(string name) {
			return name.Substring(StartModuleParameter.Length, name.Length - StartModuleParameter.Length);
		}
		public static string GetLanguageString(Assembly asm) {
			return string.Format("{0}", FrameHelper.GetLanguage(asm));
		}
	}
	public class EnumHelper {
		public static T[] GetValues<T>() {
			return (T[])Enum.GetValues(typeof(T));
		}
	}
	public static class ResourcesKeeper {
		static Dictionary<object, string> titles = new Dictionary<object, string>();
		public static void RegisterTitle(object id, string value) {
			if(!titles.ContainsKey(id))
				titles.Add(id, value);
		}
		public static string GetTitle(object value) {
			if(titles.ContainsKey(value))
				return titles[value];
			else return string.Format("{0}", value);
		}
	}
	public static class EnumTitlesKeeper<T> {
		static object nullValue = new object();
		static Dictionary<object, string> titles = new Dictionary<object, string>();
		public static void RegisterTitle(object id, string value) {
			if(id == null) {
				id = nullValue;
			}
			if(!titles.ContainsKey(id)) {
				titles.Add(id, value);
			}
		}
		public static List<TI> GetItemsList<TI>() {
			List<TI> listToReturn = new List<TI>();
			if(titles != null) {
				ConstructorInfo constructor = typeof(TI).GetConstructor(new Type[] { typeof(T), typeof(string) });
				if(constructor == null) {
					constructor = typeof(TI).GetConstructor(new Type[] { typeof(object), typeof(string) });
				}
				if(constructor != null) {
					foreach(T item in EnumHelper.GetValues<T>()) {
						listToReturn.Add((TI)constructor.Invoke(new object[] { item, GetTitle(item) }));
					}
				}
			}
			return listToReturn;
		}
		public static string GetTitle(object value) {
			if(value == null) {
				value = nullValue;
			}
			string title;
			if(!titles.TryGetValue(value, out title)) {
				title = value.ToString();
			}
			return title;
		}
	}
}
