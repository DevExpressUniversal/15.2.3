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
using System.ComponentModel.Design;
using System.Windows.Forms;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors;
namespace DevExpress.XtraSplashScreen.Design {
	public class SplashScreenManagerDesigner : BaseComponentDesignerSimple {
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			ScreensStorage = new SplashScreensInfoStorage(VSServiceHelper.GetDTEProject(this.Component));
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			Manager.Properties.ClosingDelay = 500;
		}
		internal SplashScreensInfoStorage ScreensStorage { get; private set; }
		protected override DesignerActionListCollection CreateActionLists() {
			DesignerActionListCollection result = base.CreateActionLists();
			result.Insert(0, new SplashScreenManagerActionList(this.Component, ScreensStorage));
			return result;
		}
		protected SplashScreenManager Manager { get { return Component as SplashScreenManager; } }
		protected override void Dispose(bool disposing) {
			if(disposing)
				ScreensStorage.Dispose();
			base.Dispose(disposing);
		}
	}
	class SplashScreenManagerActionList : DesignerActionList {
		SplashScreenManager component;
		public SplashScreenManagerActionList(IComponent component, SplashScreensInfoStorage screensStorage) : base(component) {
			this.component = (SplashScreenManager)component;
			ScreensStorage = screensStorage;
		}
		SplashScreensInfoStorage ScreensStorage { get; set; }
		public void AddSplashScreen() {
			AddItemTemplateCore(Mode.SplashScreen);
		}
		public void AddWaitForm() {
			AddItemTemplateCore(Mode.WaitForm);
		}
		void AddItemTemplateCore(Mode mode) {
			Cursor temp = Cursor.Current;
			try {
				Cursor.Current = Cursors.WaitCursor;
				DesignTimeHelperBase designHelper = DesignTimeHelperBase.Create(mode);
				designHelper.AddItemTemplate(Component.Site);
			}
			catch(TemplateNotFoundException) {
				XtraMessageBox.Show(GetWarningText(mode), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			finally {
				VSServiceHelper.RefreshSmartTag(this.component);
				Cursor.Current = temp;
			}
		}
		string GetWarningText(Mode mode) {
			string formName = (mode == Mode.SplashScreen ? "SplashScreen" : "WaitForm");
			return string.Format(DevExpress.XtraEditors.Design.Properties.Resources.Warning1 + Environment.NewLine + DevExpress.XtraEditors.Design.Properties.Resources.Warning2, formName);
		}
		public bool UseFadeInEffect {
			get { return this.component.Properties.UseFadeInEffect; }
			set {
				SplashFormProperties state = this.component.Properties.Clone();
				state.UseFadeInEffect = value;
				DesignTimeHelper.GetPropertyDescriptor(this.component, "Properties").SetValue(this.component, state);
			}
		}
		public bool UseFadeOutEffect {
			get { return this.component.Properties.UseFadeOutEffect; }
			set {
				SplashFormProperties state = this.component.Properties.Clone();
				state.UseFadeOutEffect = value;
				DesignTimeHelper.GetPropertyDescriptor(this.component, "Properties").SetValue(this.component, state);
			}
		}
		public bool AllowGlowEffect {
			get { return this.component.Properties.AllowGlowEffect;  }
			set {
				SplashFormProperties state = this.component.Properties.Clone();
				state.AllowGlowEffect = value;
				DesignTimeHelper.GetPropertyDescriptor(this.component, "Properties").SetValue(this.component, state);
			}
		}
		public int ClosingDelay {
			get { return this.component.Properties.ClosingDelay; }
			set {
				SplashFormProperties state = this.component.Properties.Clone();
				state.ClosingDelay = value;
				DesignTimeHelper.GetPropertyDescriptor(this.component, "Properties").SetValue(this.component, state);
			}
		}
		public TypeInfo SplashFormType {
			get { return this.component.ActiveSplashFormTypeInfo; }
			set {
				DesignTimeHelper.GetPropertyDescriptor(this.component, "ActiveSplashFormTypeInfo").SetValue(this.component, value);
				EditorContextHelperEx.RefreshSmartPanel(Component);
			}
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection result = new DesignerActionItemCollection();
			result.Add(new DesignerActionMethodItem(this, "AddSplashScreen", DevExpress.XtraEditors.Design.Properties.Resources.AddSplashScreenCaption));
			result.Add(new DesignerActionMethodItem(this, "AddWaitForm", DevExpress.XtraEditors.Design.Properties.Resources.AddWaitFormCaption));
			ScreensStorage.Refresh();
			if(IsShortMenuMode) {
				this.autoSelectMode = true;
				return result;
			}
			if(IsNeedAutoSelect) {
				TypeInfo ti = GetTypeInfo(ScreensStorage);
				if(ti != null) SplashFormType = ti;
			}
			this.autoSelectMode = false;
			result.Add(new DesignerActionPropertyItem("SplashFormType",  DevExpress.XtraEditors.Design.Properties.Resources.ActiveSplashFormCaption));
			result.Add(new DesignerActionHeaderItem(DevExpress.XtraEditors.Design.Properties.Resources.ViewOptionsCaption, "View Options"));
			result.Add(new DesignerActionPropertyItem("UseFadeInEffect", DevExpress.XtraEditors.Design.Properties.Resources.UseFadeInEffectCaption, "View Options"));
			result.Add(new DesignerActionPropertyItem("UseFadeOutEffect", DevExpress.XtraEditors.Design.Properties.Resources.UseFadeOutEffectCaption, "View Options"));
			if(SplashFormType != null && SplashFormType.Mode == Mode.SplashScreen) {
				result.Add(new DesignerActionPropertyItem("ClosingDelay", DevExpress.XtraEditors.Design.Properties.Resources.ClosingDelayCaption, "View Options"));
			}
			result.Add(new DesignerActionPropertyItem("AllowGlowEffect", DevExpress.XtraEditors.Design.Properties.Resources.AllowGlowEffectCaption, "View Options"));
			return result;
		}
		bool autoSelectMode = false;
		bool IsNeedAutoSelect {
			get {
				if(SplashFormType == null)
					return this.autoSelectMode;
				return !ScreensStorage.ContainsWithShortName(SplashFormType);
			}
		}
		TypeInfo GetTypeInfo(SplashScreensInfoStorage storage) {
			if(!VSServiceHelper.IsUserControlRootComponent(Component))
				return storage[0];
			foreach(TypeInfo ti in storage.Items) {
				if(ti.Mode == Mode.WaitForm) return ti;
			}
			return null;
		}
		protected virtual bool ShouldDisplayAddSplashScreenSmartTagItem {
			get { return !VSServiceHelper.IsUserControlRootComponent(Component); }
		}
		protected virtual bool ShouldDisplayAddWaitFormSmartTagItem { get { return true; } }
		public override bool AutoShow { get { return true; } }
		bool IsShortMenuMode { get { return ScreensStorage.ItemsCount == 0; } }
	}
}
