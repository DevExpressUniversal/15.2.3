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
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
namespace DevExpress.XtraBars.Design {
	public class FormAssistantDesigner : BaseComponentDesigner {
		public FormAssistantDesigner() {
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new FormAssistantDesignerActionList(Component));
			base.RegisterActionLists(list);
		}
	}
	public interface IFormAssistantCommandsSupports {
		void ConvertToForm();
		void ConvertToXtraForm();
		void ConvertToRibbonForm(IComponent component);
		void ConvertToTabForm();
		IComponent AddSkins(IComponent component);
	}
	public static class FormAssistantHelper {
		public static void ResetExtraProperties(Type oldType, Type newType, object targetObj) {
			var props = GetExtraProperties(newType, oldType);
			if(props.Count() == 0) return;
			foreach(PropertyInfo propertyInfo in props) {
				DefaultValueAttribute defaultValueAttribute = GetDefaultValueAttribute(Attribute.GetCustomAttributes(propertyInfo));
				if(defaultValueAttribute != null)
					ResetPropertyValue(targetObj, propertyInfo, defaultValueAttribute.Value);
			}
		}
		static void ResetPropertyValue(object sourceObj, PropertyInfo propertyInfo, object value) {
			if(propertyInfo != null) propertyInfo.SetValue(sourceObj, value, null);
		}
		static IEnumerable<PropertyInfo> GetExtraProperties(Type sourceType, Type targetType) {
			var srcProps = GetPublicProperties(sourceType);
			var targetProps = GetPublicProperties(targetType);
			return targetProps.Except(srcProps, new PropertiesComparer());
		}
		static IEnumerable<PropertyInfo> GetPublicProperties(Type type) {
			return from property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance) select property;
		}
		static DefaultValueAttribute GetDefaultValueAttribute(Attribute[] attributes) {
			return attributes.FirstOrDefault(attribute => attribute.GetType() == typeof(DefaultValueAttribute)) as DefaultValueAttribute;
		}
		#region Properties Comparer
		class PropertiesComparer : IEqualityComparer<PropertyInfo> {
			public bool Equals(PropertyInfo x, PropertyInfo y) {
				return x.Name.Equals(y.Name);
			}
			public int GetHashCode(PropertyInfo obj) {
				return obj.Name.GetHashCode();
			}
		}
		#endregion
	}
	public class FormAssistantCommandManager : IFormAssistantCommandsSupports {
		IComponent component;
		public FormAssistantCommandManager(IComponent component) {
			this.component = component;
		}
		public void ConvertToForm() {
			OnFormTypeChanging(GetRootControlType(), typeof(Form));
			FormTypeConverter.ToType(ServiceProvider, typeof(Form));
		}
		public void ConvertToXtraForm() {
			OnFormTypeChanging(GetRootControlType(), typeof(XtraForm));
			FormTypeConverter.ToType(ServiceProvider, typeof(XtraForm));
		}
		public void ConvertToRibbonForm(IComponent component) {
			OnFormTypeChanging(GetRootControlType(), typeof(RibbonForm));
			if(!BarDesignTimeUtils.IsReferenceExists<RibbonControl>(component.Site)) {
				BarDesignTimeUtils.CreateDefaultRibbonControl(component, true);
			}
			FormTypeConverter.ToType(ServiceProvider, typeof(RibbonForm));
		}
		public void ConvertToTabForm() {
			OnFormTypeChanging(GetRootControlType(), typeof(TabForm));
			FormTypeConverter.ToType(ServiceProvider, typeof(TabForm));
		}
		public IComponent AddSkins(IComponent component) {
			DefaultLookAndFeel laf = null;
			ProjectHelper.AddReference(CurrentProject, AssemblyInfo.SRAssemblyBonusSkins);
			if(!BarDesignTimeUtils.IsReferenceExists<DefaultLookAndFeel>(component.Site)) {
				laf = new DefaultLookAndFeel();
				DesignerHost.Container.Add(laf);
				ComponentChangeSvc.OnComponentChanging(laf, null);
				ComponentChangeSvc.OnComponentChanged(laf, null, null, null);
			}
			return laf;
		}
		protected void OnFormTypeChanging(Type oldType, Type newType) {
			using(DesignerTransaction tr = DesignerHost.CreateTransaction()) {
				OnFormTypeChangedCore(oldType, newType);
				tr.Commit();
			}
		}
		protected void OnFormTypeChangedCore(Type oldType, Type newType) {
			ResetExtraProperties(oldType, newType, RootControl);
			if(typeof(TabForm).Equals(oldType)) {
				RemoveTabFormControl();
			}
			SaveChangesCore();
			RefreshDesignerCore();
		}
		protected void RemoveTabFormControl() {
			for(int i = 0; i < RootControl.Controls.Count; i++) {
				if(RootControl.Controls[i] is TabFormControl) {
					RemoveTabFormControlCore((TabFormControl)RootControl.Controls[i]);
					return;
				}
			}
		}
		protected void RemoveTabFormControlCore(TabFormControl ctrl) {
			if(ctrl.Pages.Count > 0) {
				XtraTabControl tabControl = new XtraTabControl();
				RootControl.Container.Add(tabControl);
				ctrl.SelectedPage = null;
				while(ctrl.Pages.Count > 0) {
					XtraTabPage tabPage = new XtraTabPage();
					RootControl.Container.Add(tabPage);
					tabPage.Controls.Add(ctrl.Pages[0].ContentContainer);
					ctrl.Pages[0].ContentContainer = null;
					tabControl.TabPages.Add(tabPage);
					ctrl.Pages[0].Dispose();
				}
				RootControl.Controls.Add(tabControl);
			}
			ctrl.Dispose();
		}
		protected void SaveChangesCore() {
			try {
				if(CurrentItem != null)
					CurrentItem.Save();
			}
			catch { }
		}
		protected void ResetExtraProperties(Type oldType, Type newType, object obj) {
			ComponentChangeSvc.OnComponentChanging(RootControl, null);
			try {
				FormAssistantHelper.ResetExtraProperties(oldType, newType, obj);
			}
			finally {
				ComponentChangeSvc.OnComponentChanged(RootControl, null, null, null);
			}
		}
		protected void RefreshDesignerCore() {
			Control root = RootControl;
			if(root == null) return;
			ComponentChangeSvc.OnComponentChanging(root, null);
			ComponentChangeSvc.OnComponentChanged(root, null, null, null);
		}
		protected Type GetRootControlType() {
			if(RootControl == null) return null;
			return RootControl.GetType();
		}
		protected Control RootControl {
			get { return DesignerHost.RootComponent as Control; }
		}
		IComponentChangeService componentChangeSvcCore = null;
		protected IComponentChangeService ComponentChangeSvc {
			get {
				if(componentChangeSvcCore == null) {
					componentChangeSvcCore = GetService<IComponentChangeService>();
				}
				return componentChangeSvcCore;
			}
		}
		IDesignerHost designerHostCore = null;
		protected IDesignerHost DesignerHost {
			get {
				if(designerHostCore == null) {
					designerHostCore = GetService<IDesignerHost>();
				}
				return designerHostCore;
			}
		}
		EnvDTE.ProjectItem CurrentItem {
			get { return GetService<EnvDTE.ProjectItem>(); }
		}
		EnvDTE.Project CurrentProject {
			get {
				EnvDTE.ProjectItem item = CurrentItem;
				return item.ContainingProject;
			}
		}
		protected IServiceProvider ServiceProvider {
			get { return FormAssistant.Site; }
		}
		protected T GetService<T>() where T : class {
			return ServiceProvider.GetService(typeof(T)) as T;
		}
		public FormAssistant FormAssistant { get { return this.component as FormAssistant; } }
	}
	public class FormAssistantDesignerActionList : DesignerActionList {
		IFormAssistantCommandsSupports commands;
		public FormAssistantDesignerActionList(IComponent component)
			: base(component) {
			this.commands = CreateFormAssistantCommandImpObject();
		}
		protected virtual IFormAssistantCommandsSupports CreateFormAssistantCommandImpObject() {
			return new FormAssistantCommandManager(Component);
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			res.Add(new DesignerActionHeaderItem("Actions", "Actions"));
			if(ShouldAddFormTargetCnvCommand) {
				res.Add(new DesignerActionMethodItem(this, "ToForm", "Convert to Regular Form", "Actions", "Converts to the non-skinnable Windows Form."));
			}
			if(ShouldAddXtraFormTargetCnvCommand) {
				res.Add(new DesignerActionMethodItem(this, "ToXtraForm", "Convert to Skinnable Form", "Actions", "Converts to the XtraForm, which supports title bar skinning."));
			}
			if(ShouldAddRibbonFormTargetCnvCommand) {
				res.Add(new DesignerActionMethodItem(this, "ToRibbonForm", "Convert to Ribbon Form", "Actions", "Converts to the RibbonForm and adds the Ribbon Control and Ribbon Status Bar."));
			}
			if(ShouldAddTabFormTargetCnvCommand) {
				res.Add(new DesignerActionMethodItem(this, "ToTabForm", "Convert to Tabbed Form", "Actions", "Converts to the TabForm with one tab page."));
			}
			if(ShouldAddAllowFormGlassOption) {
				res.Add(new DesignerActionPropertyItem("AllowFormGlass", "Allow Aero Glass Effect", "Actions", "Enables the frame transparency feature for the form. This feature is in effect in OS's that support the Windows Aero color scheme and have this scheme turned on."));
			}
			if(ShouldAddFormBorderEffectOption) {
				res.Add(new DesignerActionPropertyItem("FormBorderEffect", "Border Effect", "Actions", "The Default value applies a shadow effect to the form's border."));
			}
			if(ShouldAddAddSkinsCommand) {
				res.Add(new DesignerActionMethodItem(this, "AddSkins", "Select Skin...", "Helpers", "Select a skin via the DefaultLookAndFeel component, which provides centralized look-and-feel management."));
			}
			return res;
		}
		protected virtual bool ShouldAddFormTargetCnvCommand {
			get { return RootComponent.GetType() != typeof(Form); }
		}
		protected virtual bool ShouldAddXtraFormTargetCnvCommand {
			get { return RootComponent.GetType() != typeof(XtraForm); }
		}
		protected virtual bool ShouldAddRibbonFormTargetCnvCommand {
			get { return RootComponent.GetType() != typeof(RibbonForm); }
		}
		protected virtual bool ShouldAddTabFormTargetCnvCommand {
			get { return RootComponent.GetType() != typeof(TabForm); }
		}
		protected virtual bool ShouldAddAddSkinsCommand {
			get { return !ProjectHelper.IsReferenceExists(CurrentProject, AssemblyInfo.SRAssemblyBonusSkins) || !BarDesignTimeUtils.IsReferenceExists<DefaultLookAndFeel>(Component.Site); }
		}
		protected virtual bool ShouldAddAllowFormGlassOption {
			get { return RootComponent.GetType() == typeof(RibbonForm) & RibbonForm != null; }
		}
		protected virtual bool ShouldAddFormBorderEffectOption {
			get { return IsXtraForm || IsRibbonForm || IsTabForm; }
		}
		public void ToForm() {
			Commands.ConvertToForm();
			EditorContextHelperEx.RefreshSmartPanel(Component);
		}
		public void ToXtraForm() {
			Commands.ConvertToXtraForm();
			EditorContextHelperEx.RefreshSmartPanel(Component);
		}
		public void ToRibbonForm() {
			Commands.ConvertToRibbonForm(Component);
			EditorContextHelperEx.RefreshSmartPanel(Component);
		}
		public void ToTabForm() {
			Commands.ConvertToTabForm();
			EditorContextHelperEx.RefreshSmartPanel(Component);
		}
		public void AddSkins() {
			DefaultLookAndFeel laf = Commands.AddSkins(Component) as DefaultLookAndFeel;
			ActionUIService.HideUI(Component);
			if(laf != null)
				ActionUIService.ShowUI(laf);
		}
		public virtual bool AllowFormGlass {
			get { return RibbonForm.AllowFormGlass != DefaultBoolean.False; }
			set {
				DefaultBoolean valueCore = value ? DefaultBoolean.True : DefaultBoolean.False;
				EditorContextHelper.SetPropertyValue(Component.Site, RibbonForm, "AllowFormGlass", valueCore);
			}
		}
		public virtual FormBorderEffect FormBorderEffect {
			get {
				if(IsRibbonForm) return RibbonForm.FormBorderEffect;
				return XtraForm.FormBorderEffect;
			}
			set {
				var form = IsRibbonForm ? RibbonForm : XtraForm;
				EditorContextHelper.SetPropertyValue(Component.Site, form, "FormBorderEffect", value);
			}
		}
		#region Helpers
		protected RibbonForm RibbonForm {
			get { return RootComponent as RibbonForm; }
		}
		protected XtraForm XtraForm {
			get { return RootComponent as XtraForm; }
		}
		protected bool IsRibbonForm {
			get { return RootComponent.GetType() == typeof(RibbonForm); }
		}
		protected bool IsXtraForm {
			get { return RootComponent.GetType() == typeof(XtraForm); }
		}
		protected bool IsTabForm {
			get { return RootComponent.GetType() == typeof(TabForm); }
		}
		protected bool IsDefaultLookAndFeelComponentExists() {
			var objects = ReferenceService.GetReferences(typeof(DefaultLookAndFeel));
			return objects.Length > 0;
		}
		IReferenceService referenceServiceCore = null;
		IReferenceService ReferenceService {
			get {
				if(referenceServiceCore == null) {
					referenceServiceCore = GetService(typeof(IReferenceService)) as IReferenceService;
				}
				return referenceServiceCore;
			}
		}
		DesignerActionUIService sctionUIServiceCore = null;
		DesignerActionUIService ActionUIService {
			get {
				if(sctionUIServiceCore == null) {
					sctionUIServiceCore = GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;
				}
				return sctionUIServiceCore;
			}
		}
		internal EnvDTE.Project CurrentProject {
			get {
				EnvDTE.ProjectItem projectItem = GetService(typeof(EnvDTE.ProjectItem)) as EnvDTE.ProjectItem;
				return projectItem != null ? projectItem.ContainingProject : null;
			}
		}
		IDesignerHost designerHostCore = null;
		protected IDesignerHost DesignerHost {
			get {
				if(designerHostCore == null) {
					designerHostCore = GetService(typeof(IDesignerHost)) as IDesignerHost;
				}
				return designerHostCore;
			}
		}
		protected IComponent RootComponent {
			get { return DesignerHost.RootComponent; }
		}
		#endregion
		public IFormAssistantCommandsSupports Commands { get { return commands; } }
		public override bool AutoShow { get { return true; } set { base.AutoShow = value; } }
	}
}
