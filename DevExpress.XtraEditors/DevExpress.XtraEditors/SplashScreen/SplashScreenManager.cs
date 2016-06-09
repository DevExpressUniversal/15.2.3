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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Skins.Info;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ToolboxIcons;
using DevExpress.XtraSplashForm;
using DevExpress.XtraSplashScreen.Utils;
using DevExpress.XtraWaitForm;
namespace DevExpress.XtraSplashScreen {
	public enum ParentType { Form, UserControl }
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class SplashFormProperties {
		Image image;
		WeakReference parentFormCore, parentFormInternal, parentControl;
		bool useFadeIn, useFadeOut, parentFormSavedState;
		bool allowGlowEffect = false;
		int pendingTime, closingDelay;
		IntPtr parentHandle;
		ICustomImagePainter painter;
		public SplashFormProperties() : this(null, null, true, true) { }
		public SplashFormProperties(Form parentForm, bool useFadeIn, bool useFadeOut) : this(parentForm, null, useFadeIn, useFadeOut) { }
		public SplashFormProperties(Form parentForm, bool useFadeIn, bool useFadeOut, bool allowGlowEffect) : this(parentForm, null, useFadeIn, useFadeOut, allowGlowEffect) { }
		public SplashFormProperties(Form parentForm, bool useFadeIn, bool useFadeOut, int pendingTime) : this(parentForm, useFadeIn, useFadeOut, pendingTime, null) { }
		public SplashFormProperties(Form parentForm, bool useFadeIn, bool useFadeOut, int pendingTime, Form parentFormInternal)
			: this(parentForm, null, useFadeIn, useFadeOut, null, pendingTime, parentFormInternal) {
		}
		public SplashFormProperties(Form parentForm, bool useFadeIn, bool useFadeOut, int pendingTime, Form parentFormInternal, bool allowGlowEffect)
			: this(parentForm, null, useFadeIn, useFadeOut, null, pendingTime, parentFormInternal, allowGlowEffect) {
		}
		public SplashFormProperties(Form parentForm, Image image, bool useFadeIn, bool useFadeOut) : this(parentForm, image, useFadeIn, useFadeOut, null, 0, null) { }
		public SplashFormProperties(Form parentForm, Image image, bool useFadeIn, bool useFadeOut, bool allowGlowEffect) : this(parentForm, image, useFadeIn, useFadeOut, null, 0, null, allowGlowEffect) { }
		public SplashFormProperties(Form parentForm, Image image, bool useFadeIn, bool useFadeOut, ICustomImagePainter painter, int pendingTime)
			: this(parentForm, image, useFadeIn, useFadeOut, painter, pendingTime, null) {
		}
		public SplashFormProperties(Form parentForm, Image image, bool useFadeIn, bool useFadeOut, ICustomImagePainter painter, int pendingTime, Form parentFormInternal)
			: this(parentForm, image, useFadeIn, useFadeOut, painter, pendingTime, parentFormInternal, false) { }
		public SplashFormProperties(Form parentForm, Image image, bool useFadeIn, bool useFadeOut, ICustomImagePainter painter, int pendingTime, Form parentFormInternal, bool allowGlowEffect)
			: this(parentForm, image, useFadeIn, useFadeOut, painter, pendingTime, parentFormInternal, allowGlowEffect, DefaultClosingDelay) {
		}
		public SplashFormProperties(Form parentForm, Image image, bool useFadeIn, bool useFadeOut, ICustomImagePainter painter, int pendingTime, Form parentFormInternal, bool allowGlowEffect, int closingDelay) {
			SetParentForm(parentForm);
			this.parentFormInternal = new WeakReference(parentFormInternal);
			this.image = image;
			this.useFadeIn = useFadeIn;
			this.useFadeOut = useFadeOut;
			this.painter = painter;
			this.pendingTime = pendingTime;
			this.closingDelay = closingDelay;
			this.allowGlowEffect = allowGlowEffect;
		}
		internal void SetParentForm(Form parentForm) {
			this.parentFormCore = new WeakReference(parentForm);
			if(parentForm == null) parentFormSavedState = false;
			else parentFormSavedState = parentForm.Enabled;
		}
		internal void SetParentControl(UserControl parentControl) {
			this.parentControl = new WeakReference(parentControl);
		}
		internal void CheckParentForm() {
			if(ParentForm != null || this.parentControl == null) return;
			UserControl control = this.parentControl.Target as UserControl;
			if(control != null) {
				Form form = control.FindForm();
				if(form != null) {
					SetParentForm(form);
				}
			}
		}
		internal void InitHandle() {
			Form form = ParentForm;
			if(form == null || !form.IsHandleCreated || form.InvokeRequired)
				return;
			this.parentHandle = form.Handle;
		}
		protected internal Form ParentFormInternal {
			get { return parentFormInternal.Target as Form; }
		}
		public SplashFormProperties Clone() {
			return this.MemberwiseClone() as SplashFormProperties;
		}
		[DefaultValue(true)]
		public bool UseFadeInEffect {
			get { return this.useFadeIn; }
			set { this.useFadeIn = value; }
		}
		[DefaultValue(false)]
		public bool AllowGlowEffect {
			get { return this.allowGlowEffect; }
			set { this.allowGlowEffect = value; }
		}
		[DefaultValue(true)]
		public bool UseFadeOutEffect {
			get { return this.useFadeOut; }
			set { this.useFadeOut = value; }
		}
		const int DefaultClosingDelay = 0;
		[DefaultValue(DefaultClosingDelay)]
		public int ClosingDelay {
			get { return this.closingDelay; }
			set {
				if(value < 0) value = 0;
				if(ClosingDelay == value)
					return;
				this.closingDelay = value;
			}
		}
		[Browsable(false)]
		public Form ParentForm {
			get { return parentFormCore.Target as Form; }
		}
		protected internal bool ParentFormSavedState {
			get { return this.parentFormSavedState; }
		}
		[Browsable(false)]
		public Image Image {
			get { return this.image; }
			set { this.image = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ICustomImagePainter CustomImagePainter {
			get { return this.painter; }
			set { this.painter = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public IntPtr ParentHandle {
			get { return parentHandle; }
		}
		protected internal int PendingTime { get { return pendingTime; } }
	}
	[TypeConverter("DevExpress.XtraSplashScreen.Design.SplashScreenTypeConverter, " + AssemblyInfo.SRAssemblyEditorsDesign)]
	public class TypeInfo {
		Mode modeCore;
		string typeNameCore;
		public TypeInfo(string typeName, Mode mode) {
			Mode = mode;
			TypeName = typeName;
		}
		public Mode Mode {
			get { return this.modeCore; }
			private set { this.modeCore = value; }
		}
		public string TypeName {
			get { return this.typeNameCore; }
			private set { this.typeNameCore = value; }
		}
		public string GetShortName() {
			int ptPos = TypeName.LastIndexOf('.');
			return ptPos != -1 ? TypeName.Substring(++ptPos) : TypeName;
		}
		public static TypeInfo FromType(Type type) {
			if(type == null)
				return null;
			Mode mode = Mode.SplashScreen;
			if(typeof(WaitForm).IsAssignableFrom(type))
				mode = Mode.WaitForm;
			return new TypeInfo(type.FullName, mode);
		}
		#region Overrides
		public override bool Equals(object obj) {
			TypeInfo sample = obj as TypeInfo;
			if(sample == null)
				return false;
			return string.Equals(TypeName, sample.TypeName, StringComparison.Ordinal);
		}
		public override string ToString() {
			return GetShortName();
		}
		public override int GetHashCode() {
			return TypeName.GetHashCode();
		}
		#endregion
	}
	public interface ICustomImagePainter {
		void Draw(GraphicsCache cache, Rectangle bounds);
	}
	public enum Mode { SplashScreen, WaitForm, Layer }
	public enum SplashFormStartPosition { Default, Manual }
	public enum ParentFormState { Locked, Unlocked }
	[Designer("DevExpress.XtraSplashScreen.Design.SplashScreenManagerDesigner, " + AssemblyInfo.SRAssemblyEditorsDesignFull)]
	[DesignerSerializer("DevExpress.XtraSplashScreen.Design.SplashScreenManagerSerializer, " + AssemblyInfo.SRAssemblyEditorsDesignFull, "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")]
	[DesignerCategory("Component")]
	[DXToolboxItem(DXToolboxItemKind.Regular)]
	[ToolboxTabName(AssemblyInfo.DXTabNavigation)]
	[ToolboxBitmap(typeof(ToolboxIconsRootNS), "SplashScreenManager")]
	[Description("Manages the creation and display of splash forms.")]
	public class SplashScreenManager : Component, DevExpress.Utils.MVVM.Services.ISplashScreenServiceProvider {
		Type typeCore;
		ParentFormState parentFormDesiredState;
		ThreadManagerBase threadManagerCore;
		static SplashScreenManager() {
			SkinName = null;
			ApartmentState = null;
			ActivateParentOnSplashFormClosing = ActivateParentOnWaitFormClosing = true;
		}
		public SplashScreenManager() : this(null, null, true, true) { }
		public SplashScreenManager(Form parentForm, Type splashFormType, bool useFadeIn, bool useFadeOut)
			: this(splashFormType, new SplashFormProperties(parentForm, useFadeIn, useFadeOut)) {
		}
		public SplashScreenManager(Form parentForm, Type splashFormType, bool useFadeIn, bool useFadeOut, int closingDelay)
			: this(splashFormType, new SplashFormProperties(parentForm, null, useFadeIn, useFadeOut, null, 0, null, false, closingDelay)) {
		}
		public SplashScreenManager(Form parentForm, Type splashFormType, bool useFadeIn, bool useFadeOut, bool allowGlowEffect)
			: this(splashFormType, new SplashFormProperties(parentForm, useFadeIn, useFadeOut, allowGlowEffect)) {
		}
		public SplashScreenManager(UserControl parentControl, Type splashFormType, bool useFadeIn, bool useFadeOut, ParentType type)
			: this(parentControl.FindForm(), splashFormType, useFadeIn, useFadeOut) {
			this.stateInfoCore.SetParentControl(parentControl);
		}
		public SplashScreenManager(UserControl parentControl, Type splashFormType, bool useFadeIn, bool useFadeOut, Type type)
			: this(parentControl, splashFormType, useFadeIn, useFadeOut, GetParentType(type)) {
		}
		public SplashScreenManager(UserControl parentControl, Type splashFormType, bool useFadeIn, bool useFadeOut, ParentType type, bool allowGlowEffect)
			: this(parentControl.FindForm(), splashFormType, useFadeIn, useFadeOut, allowGlowEffect) {
			this.stateInfoCore.SetParentControl(parentControl);
		}
		public SplashScreenManager(UserControl parentControl, Type splashFormType, bool useFadeIn, bool useFadeOut, Type type, bool allowGlowEffect)
			: this(parentControl, splashFormType, useFadeIn, useFadeOut, GetParentType(type), allowGlowEffect) {
		}
		public SplashScreenManager(UserControl parentControl, Type splashFormType, bool useFadeIn, bool useFadeOut, SplashFormStartPosition startPos, Point loc, ParentType type)
			: this(parentControl.FindForm(), splashFormType, useFadeIn, useFadeOut, startPos, loc, 0, null) {
			this.stateInfoCore.SetParentControl(parentControl);
		}
		public SplashScreenManager(UserControl parentControl, Type splashFormType, bool useFadeIn, bool useFadeOut, SplashFormStartPosition startPos, Point loc, Type type)
			: this(parentControl, splashFormType, useFadeIn, useFadeOut, startPos, loc, GetParentType(type)) {
		}
		public SplashScreenManager(UserControl parentControl, Type splashFormType, bool useFadeIn, bool useFadeOut, SplashFormStartPosition startPos, Point loc, ParentType type, bool allowGlowEffect)
			: this(parentControl.FindForm(), splashFormType, useFadeIn, useFadeOut, startPos, loc, 0, null, allowGlowEffect) {
			this.stateInfoCore.SetParentControl(parentControl);
		}
		public SplashScreenManager(UserControl parentControl, Type splashFormType, bool useFadeIn, bool useFadeOut, SplashFormStartPosition startPos, Point loc, Type type, bool allowGlowEffect)
			: this(parentControl, splashFormType, useFadeIn, useFadeOut, startPos, loc, GetParentType(type), allowGlowEffect) {
		}
		public SplashScreenManager(Form parentForm, Type splashFormType, bool useFadeIn, bool useFadeOut, SplashFormStartPosition startPos, Point loc)
			: this(parentForm, splashFormType, useFadeIn, useFadeOut, startPos, loc, 0, null) {
		}
		public SplashScreenManager(Form parentForm, Type splashFormType, bool useFadeIn, bool useFadeOut, SplashFormStartPosition startPos, Point loc, bool allowGlowEffect)
			: this(parentForm, splashFormType, useFadeIn, useFadeOut, startPos, loc, 0, null, allowGlowEffect) {
		}
		public SplashScreenManager(Form parentForm, Type splashFormType, bool useFadeIn, bool useFadeOut, SplashFormStartPosition startPos, Point loc, int pendingTime)
			: this(parentForm, splashFormType, useFadeIn, useFadeOut, startPos, loc, pendingTime, null) {
		}
		internal SplashScreenManager(Form parentForm, Type splashFormType, bool useFadeIn, bool useFadeOut, SplashFormStartPosition startPos, Point loc, int pendingTime, Form parentFormInternal)
			: this(parentForm, splashFormType, useFadeIn, useFadeOut, startPos, loc, pendingTime, parentFormInternal, ParentFormState.Unlocked) {
		}
		internal SplashScreenManager(Form parentForm, Type splashFormType, bool useFadeIn, bool useFadeOut, SplashFormStartPosition startPos, Point loc, int pendingTime, Form parentFormInternal, bool allowGlowEffect)
			: this(parentForm, splashFormType, useFadeIn, useFadeOut, startPos, loc, pendingTime, parentFormInternal, ParentFormState.Unlocked, allowGlowEffect) {
		}
		internal SplashScreenManager(Form parentForm, Type splashFormType, bool useFadeIn, bool useFadeOut, SplashFormStartPosition startPos, Point loc, int pendingTime, Form parentFormInternal, ParentFormState parentFormDesiredState)
			: this(splashFormType, startPos, loc, new SplashFormProperties(parentForm, useFadeIn, useFadeOut, pendingTime, parentFormInternal), parentFormDesiredState) {
		}
		internal SplashScreenManager(Form parentForm, Type splashFormType, bool useFadeIn, bool useFadeOut, SplashFormStartPosition startPos, Point loc, int pendingTime, Form parentFormInternal, ParentFormState parentFormDesiredState, bool allowGlowEffect)
			: this(splashFormType, startPos, loc, new SplashFormProperties(parentForm, useFadeIn, useFadeOut, pendingTime, parentFormInternal, allowGlowEffect), parentFormDesiredState) {
		}
		public SplashScreenManager(Type splashFormType, SplashFormProperties info)
			: this(splashFormType, SplashFormStartPosition.Default, Point.Empty, info) {
		}
		public SplashScreenManager(Type splashFormType, SplashFormStartPosition startPos, Point loc, SplashFormProperties info)
			: this(splashFormType, startPos, loc, info, ParentFormState.Unlocked) {
		}
		public SplashScreenManager(Type splashFormType, SplashFormStartPosition startPos, Point loc, SplashFormProperties info, ParentFormState parentFormDesiredState) {
			this.parentFormDesiredState = parentFormDesiredState;
			this.typeCore = splashFormType;
			this.stateInfoCore = info.Clone();
			this.typeInfoCore = TypeInfo.FromType(splashFormType);
			this.SplashFormStartPosition = startPos;
			this.SplashFormLocation = loc;
			if(!DesignTimeTools.IsDesignMode && IsNeedAutoDisplaying) ShowCore();
		}
		internal Func<SplashFormBase> CreateFormFunction;
		internal SplashScreenManager(Form parentForm, Type splashFormType, bool useFadeIn, bool useFadeOut, SplashFormStartPosition startPos, Point loc, int pendingTime, Form parentFormInternal, ParentFormState parentFormDesiredState, bool allowGlowEffect, Func<SplashFormBase> createFormFunction)
			: this(splashFormType, startPos, loc, new SplashFormProperties(parentForm, useFadeIn, useFadeOut, pendingTime, parentFormInternal, allowGlowEffect), parentFormDesiredState) {
			this.CreateFormFunction = createFormFunction;
		}
		void RemoveCreateFormFunction() {
			CreateFormFunction = null;
		}
		public void ShowWaitForm() {
			CheckParentForm();
			ShowCore();
		}
		protected void CheckParentForm() {
			SplashFormProperties properties = Properties;
			if(properties != null) properties.CheckParentForm();
		}
		public void CloseWaitForm() {
			CloseCore();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int ClosingDelay { 
			get { return Properties.ClosingDelay; }
			set { Properties.ClosingDelay = value; }
		}
		protected internal bool FormInPendingStateCore {
			get { return ThreadManager != null ? ThreadManager.InPendingMode : false; }
		}
		internal bool IsManagerActive() { return isManagerActive; }
		bool isManagerActive = false;
		internal void ShowCore() {
			if(!IsReadyForDisplaying)
				return;
			if(IsManagerActive())
				throw new InvalidOperationException("Splash Form already shown");
			Form parentForm = Properties.ParentForm;
			if(parentForm != null) {
				DisableParentIfRequired();
				if(IsNeedAutoDisplaying) SubscribeOwnerEvents(parentForm);
			}
			if(Properties.ParentFormInternal != null)
				SubscribeOnLoadInternal(Properties.ParentFormInternal);
			if(parentForm != null) {
				SubscribeOnDispose(parentForm);
			}
			Properties.InitHandle();
			ThreadManager = CreateThreadManager();
			ThreadManager.Start();
			this.isManagerActive = true;
		}
		internal void CloseCore() {
			CloseCore(0, null);
		}
		internal void CloseCore(int closingDelay, Form parent) {
			CloseCore(true, closingDelay, parent);
		}
		internal void CloseCore(bool throwExceptionIfAlreadyClosed, int closingDelay, Form parent) {
			if(!IsReadyForDisplaying)
				return;
			if(!IsManagerActive())
				throw new InvalidOperationException("Splash Form is not created");
			if(this.parentFormDesiredState == ParentFormState.Locked) Application.DoEvents();
			this.isManagerActive = this.IsSplashFormVisible = false;
			if(Properties != null) {
				UnsubscribeOwnerEvents(Properties.ParentForm);
				UnsubscribeOnLoadInternal(Properties.ParentFormInternal);
				UnsubscribeOnDispose(Properties.ParentForm);
			}
			ThreadManager.Destroy(closingDelay, parent, throwExceptionIfAlreadyClosed);
			EnableParentIfRequired();
		}
		internal void OnBeforeShow() {
		}
		internal void OnClosed() {
		}
		protected void DisableParentIfRequired() {
			if(this.parentFormDesiredState == ParentFormState.Locked) DisableParent();
		}
		protected void EnableParentIfRequired() {
			if(this.parentFormDesiredState == ParentFormState.Locked) EnableParent();
		}
		protected void DisableParent() {
			Form realOwner = GetRealOwner();
			if(realOwner != null) realOwner.Enabled = false;
		}
		protected void EnableParent() {
			SplashFormProperties properties = Properties;
			if(properties == null) return;
			Form realOwner = GetRealOwner();
			if(realOwner != null) realOwner.Enabled = properties.ParentFormSavedState;
		}
		protected void DoSafe(Action action) {
			Form owner = GetRealOwner();
			if(owner == null) return;
			if(owner.InvokeRequired) {
				if(owner.IsHandleCreated) {
					try { owner.BeginInvoke(action); }
					catch { }
				}
			}
			else {
				action();
			}
		}
		protected Form GetRealOwner() {
			SplashFormProperties properties = Properties;
			if(properties == null) return null;
			Form form = properties.ParentForm;
			if(form != null && form.IsMdiChild) {
				return form.MdiParent;
			}
			return form;
		}
		protected virtual void SubscribeOwnerEvents(Form parentForm) {
			if(parentForm == null) return;
			parentForm.Load += OnOwnerFormLoad;
			parentForm.HandleCreated += OnOwnerFormHandleCreated;
		}
		protected virtual void UnsubscribeOwnerEvents(Form parentForm) {
			if(parentForm == null) return;
			parentForm.Load -= OnOwnerFormLoad;
			parentForm.HandleCreated -= OnOwnerFormHandleCreated;
		}
		protected virtual void SubscribeOnLoadInternal(Form parentFormInternal) {
			if(parentFormInternal != null)
				parentFormInternal.Load += ParentFormInternal_Load;
		}
		protected virtual void SubscribeOnDispose(Form parentForm) {
			if(parentForm != null) {
				parentForm.Disposed -= ParentForm_Disposed;
				parentForm.Disposed += ParentForm_Disposed;
			}
		}
		protected virtual void UnsubscribeOnLoadInternal(Form parentFormInternal) {
			if(parentFormInternal != null)
				parentFormInternal.Load -= ParentFormInternal_Load;
		}
		protected virtual void UnsubscribeOnDispose(Form parentForm) {
			if(parentForm != null)
				parentForm.Disposed -= ParentForm_Disposed;
		}
		protected virtual ThreadManagerBase CreateThreadManager() {
			return ThreadManagerBase.Create(this);
		}
		public void SendCommand(Enum cmd, object arg) {
			if(!IsManagerActive())
				throw new InvalidOperationException("Splash Form is not created");
			if(FormInPendingStateCore) {
				ThreadManager.SendCommand(cmd, arg);
			}
			SplashFormBase form = ThreadManager.SplashBase;
			if(form != null && form.IsHandleCreated) form.Invoke((ScreenAction<Enum, object>)form.ProcessCommand, cmd, arg);
		}
		public void Invalidate() {
			if(!IsManagerActive())
				throw new InvalidOperationException("Splash Form is not created");
			SplashFormBase form = ThreadManager.SplashBase;
			if(form != null && form.IsHandleCreated) form.Invoke((ScreenAction)form.Invalidate);
		}
		public void SetWaitFormCaption(string caption) {
			if(!IsManagerActive())
				throw new InvalidOperationException("Splash Form is not created");
			if(!IsWaitFormUsage)
				throw new InvalidOperationException("This command is only applicable to WaitForms");
			if(FormInPendingStateCore) {
				ThreadManager.SetWaitFormCaption(caption);
			}
			WaitForm form = (WaitForm)ThreadManager.Form;
			if(form != null && form.IsHandleCreated) form.BeginInvoke((ScreenAction<string>)form.SetCaption, caption);
		}
		public void SetWaitFormDescription(string description) {
			if(!IsManagerActive())
				throw new InvalidOperationException("Splash Form is not created");
			if(!IsWaitFormUsage)
				throw new InvalidOperationException("This command is only applicable to WaitForms");
			if(FormInPendingStateCore) {
				ThreadManager.SetWaitFormDescription(description);
			}
			WaitForm form = (WaitForm)ThreadManager.Form;
			if(form != null && form.IsHandleCreated) form.BeginInvoke((ScreenAction<string>)form.SetDescription, description);
		}
		public void WaitForSplashFormClose() {
			ThreadManagerBase threadManager = ThreadManager;
			if(threadManager != null && threadManager.Thread != null) {
				threadManager.Join();
			}
		}
		internal ThreadManagerBase ThreadManager {
			get { return this.threadManagerCore; }
			set { this.threadManagerCore = value; }
		}
		TypeInfo typeInfoCore;
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("SplashScreenManagerActiveSplashFormTypeInfo"),
#endif
 Category("SplashForms"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TypeInfo ActiveSplashFormTypeInfo {
			get { return this.typeInfoCore; }
			set { this.typeInfoCore = value; }
		}
		SplashFormProperties stateInfoCore;
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("SplashScreenManagerProperties"),
#endif
 DXCategory(CategoryName.Behavior), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SplashFormProperties Properties {
			get { return this.stateInfoCore; }
			set { this.stateInfoCore = value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("SplashScreenManagerSplashFormLocation"),
#endif
 Category("Layout"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Point SplashFormLocation {
			get;
			set;
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("SplashScreenManagerSplashFormStartPosition"),
#endif
 DefaultValue(SplashFormStartPosition.Default), Category("Layout"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SplashFormStartPosition SplashFormStartPosition {
			get;
			set;
		}
		internal Mode Mode {
			get {
				if(SplashFormType == null)
					throw new InvalidOperationException("SplashFormType is not assigned");
				if(typeof(WaitForm).IsAssignableFrom(SplashFormType))
					return Mode.WaitForm;
				if(typeof(SplashScreenLayer).IsAssignableFrom(SplashFormType))
					return Mode.Layer;
				return Mode.SplashScreen;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsSplashFormVisible { get; internal set; }
		void OnOwnerFormLoad(object sender, EventArgs e) {
			Form parentForm = Properties.ParentForm;
			if(parentForm == null) return;
			UnsubscribeOwnerEvents(parentForm);
			parentForm.BeginInvoke((ScreenAction)delegate {
				SplashFormProperties properties = Properties;
				if(properties != null && properties.ClosingDelay > 0) {
					CloseCore(properties.ClosingDelay, sender as Form);
				}
				else CloseCore();
			});
		}
		void OnOwnerFormHandleCreated(object sender, EventArgs e) {
			Form parentForm = sender as Form;
			if(parentForm != null)
				parentForm.HandleCreated -= OnOwnerFormHandleCreated;
			ThreadManagerBase threadManager = ThreadManager;
			if(threadManager != null) threadManager.OnOwnerFormHandleCreated();
		}
		void ParentFormInternal_Load(object sender, EventArgs e) {
			UnsubscribeOnLoadInternal(Properties.ParentFormInternal);
			SplashFormBase form = ThreadManager.SplashBase;
			if(form == null || !form.IsHandleCreated) return;
			try {
				form.BeginInvoke(new MethodInvoker(form.OnParentInternalLoad));
			}
			catch { }
		}
		void ParentForm_Disposed(object sender, EventArgs e) {
			UnsubscribeOnDispose(Properties.ParentForm);
			Dispose();
		}
		protected internal virtual bool IsInnerManager { get { return false; } }
		internal Type SplashFormType { get { return this.typeCore; } set { this.typeCore = value; } }
		protected internal virtual bool IsReadyForDisplaying { get { return SplashFormType != null; } }
		protected internal virtual bool IsNeedAutoDisplaying { get { return !IsWaitFormUsage; } }
		internal bool IsWaitFormUsage { get { return typeof(WaitForm).IsAssignableFrom(SplashFormType); } }
		bool isDisposed = false;
		protected override void Dispose(bool disposing) {
			if(!this.isDisposed && disposing) {
				if(Properties != null && Properties.ParentForm != null)
					UnsubscribeOnDispose(Properties.ParentForm);
				if(Properties != null)
					Properties = null;
				if(ActiveSplashFormTypeInfo != null)
					ActiveSplashFormTypeInfo = null;
				if(SplashFormType != null)
					SplashFormType = null;
				if(ThreadManager != null) {
					ThreadManager.Dispose();
					ThreadManager = null;
				}
				this.isDisposed = true;
			}
			base.Dispose(disposing);
		}
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("SplashScreenManagerDefault")]
#endif
		public static SplashScreenManager Default { get; protected set; }
		public static void ShowForm(Type splashFormType) {
			ShowForm((Form)null, splashFormType, true, true);
		}
		public static void ShowForm(Form parentForm, Type splashFormType) {
			ShowForm(parentForm, splashFormType, true, true);
		}
		public static void ShowForm(Type splashFormType, bool useFadeIn, bool useFadeOut) {
			ShowForm((Form)null, splashFormType, useFadeIn, useFadeOut);
		}
		public static void ShowForm(Form parentForm, Type splashFormType, bool useFadeIn, bool useFadeOut) {
			ShowForm(parentForm, splashFormType, useFadeIn, useFadeOut, ParentFormState.Unlocked);
		}
		#region For Demos
		public static void ShowDefaultSplashScreen(string productText = null, string demoText = null) {
			ShowDefaultSplashScreen(null, true, true, productText, demoText);
		}
		public static void ShowDefaultProgressSplashScreen(string status) {
			ShowForm(typeof(DemoProgressSplashScreen));
			SplashScreenManager defaultManager = Default;
			if(defaultManager != null) Default.SendCommand(DemoProgressSplashScreen.CommandId.SetLabel, status);
		}
		public static void SetDefaultProgressSplashScreenValue(bool throwExceptionIfNotOpened, int value) {
			SplashScreenManager defaultManager = Default;
			if(defaultManager == null) {
				if(throwExceptionIfNotOpened) {
					throw new InvalidOperationException("Splash Form is not displayed");
				}
				return;
			}
			defaultManager.SendCommand(DemoProgressSplashScreen.CommandId.SetProgressValue, value);
		}
		public static void ShowDefaultSplashScreen(Form parentForm, bool useFadeIn, bool useFadeOut, string productText = null, string demoText = null) {
			ShowForm(parentForm, typeof(DemoSplashScreen), useFadeIn, useFadeOut);
			SplashScreenManager defaultManager = Default;
			if(!string.IsNullOrEmpty(productText)) {
				defaultManager.SendCommand(DemoSplashScreenBase.SplashScreenCommand.UpdateLabelProductText, productText);
			}
			if(!string.IsNullOrEmpty(demoText)) {
				defaultManager.SendCommand(DemoSplashScreenBase.SplashScreenCommand.UpdateLabelDemoText, demoText);
			}
		}
		public static void SetDefaultSplashScreenStatus(bool throwExceptionIfNotOpened, string status) {
			SplashScreenManager defaultManager = Default;
			if(defaultManager == null) {
				if(throwExceptionIfNotOpened) {
					throw new InvalidOperationException("Splash Form is not displayed");
				}
				return;
			}
			defaultManager.SendCommand(DemoSplashScreenBase.SplashScreenCommand.UpdateLabel, status);
		}
		public static void ShowDefaultWaitForm(string caption = null, string description = null) {
			ShowDefaultWaitForm(null, true, true, caption, description);
		}
		public static void ShowDefaultWaitForm(Form parentForm, bool useFadeIn, bool useFadeOut, string caption = null, string description = null) {
			ShowDefaultWaitForm(parentForm, useFadeIn, useFadeOut, false, 0, caption, description);
		}
		public static void ShowDefaultWaitForm(Form parentForm, bool useFadeIn, bool useFadeOut, bool throwExceptionIfAlreadyOpened, int pendingTime, string caption = null, string description = null) {
			ShowForm(parentForm, typeof(DemoWaitForm), useFadeIn, useFadeOut, throwExceptionIfAlreadyOpened, pendingTime);
			SplashScreenManager defaultManager = Default;
			if(!string.IsNullOrEmpty(caption)) {
				defaultManager.SetWaitFormCaption(caption);
			}
			if(!string.IsNullOrEmpty(description)) {
				defaultManager.SetWaitFormDescription(description);
			}
		}
		public static void CloseDefaultSplashScreen() { CloseForm(); }
		public static void CloseDefaultWaitForm() { CloseForm(); }
		#endregion
		public static void ShowForm(Form parentForm, Type splashFormType, bool useFadeIn, bool useFadeOut, ParentFormState parentFormDesiredState) {
			ShowForm(parentForm, splashFormType, useFadeIn, useFadeOut, true, parentFormDesiredState);
		}
		public static void ShowForm(UserControl parentControl, Type splashFormType, bool useFadeIn, bool useFadeOut) {
			ShowForm(parentControl, splashFormType, useFadeIn, useFadeOut, ParentFormState.Unlocked);
		}
		public static void ShowForm(UserControl parentControl, Type splashFormType, bool useFadeIn, bool useFadeOut, ParentFormState parentFormDesiredState) {
			if(!typeof(WaitForm).IsAssignableFrom(splashFormType)) throw new InvalidOperationException("You should use WaitForm only in this case");
			ShowForm(parentControl.FindForm(), splashFormType, useFadeIn, useFadeOut, parentFormDesiredState);
		}
		public static void ShowForm(UserControl parentControl, Type splashFormType, bool useFadeIn, bool useFadeOut, SplashFormStartPosition startPos, Point loc) {
			ShowForm(parentControl, splashFormType, useFadeIn, useFadeOut, startPos, loc, ParentFormState.Unlocked);
		}
		public static void ShowForm(UserControl parentControl, Type splashFormType, bool useFadeIn, bool useFadeOut, SplashFormStartPosition startPos, Point loc, ParentFormState parentFormDesiredState) {
			if(!typeof(WaitForm).IsAssignableFrom(splashFormType)) throw new InvalidOperationException("You should use WaitForm only in this case");
			ShowForm(parentControl.FindForm(), splashFormType, useFadeIn, useFadeOut, startPos, loc, parentFormDesiredState);
		}
		public static void ShowForm(Form parentForm, Type splashFormType, bool useFadeIn, bool useFadeOut, SplashFormStartPosition startPos, Point loc) {
			ShowForm(parentForm, splashFormType, useFadeIn, useFadeOut, startPos, loc, ParentFormState.Unlocked);
		}
		public static void ShowForm(Form parentForm, Type splashFormType, bool useFadeIn, bool useFadeOut, SplashFormStartPosition startPos, Point loc, ParentFormState parentFormDesiredState) {
			ShowForm(parentForm, splashFormType, useFadeIn, useFadeOut, true, startPos, loc, parentFormDesiredState);
		}
		public static void ShowForm(Form parentForm, Type splashFormType, bool useFadeIn, bool useFadeOut, bool throwExceptionIfAlreadyOpened) {
			ShowForm(parentForm, splashFormType, useFadeIn, useFadeOut, throwExceptionIfAlreadyOpened, ParentFormState.Unlocked);
		}
		public static void ShowForm(Form parentForm, Type splashFormType, bool useFadeIn, bool useFadeOut, bool throwExceptionIfAlreadyOpened, bool allowGlowEffect) {
			ShowForm(parentForm, splashFormType, useFadeIn, useFadeOut, throwExceptionIfAlreadyOpened, SplashFormStartPosition.Default, Point.Empty, allowGlowEffect);
		}
		public static void ShowForm(Form parentForm, Type splashFormType, bool useFadeIn, bool useFadeOut, bool throwExceptionIfAlreadyOpened, ParentFormState parentFormDesiredState) {
			ShowForm(parentForm, splashFormType, useFadeIn, useFadeOut, throwExceptionIfAlreadyOpened, SplashFormStartPosition.Default, Point.Empty, parentFormDesiredState);
		}
		public static void ShowForm(Form parentForm, Type splashFormType, bool useFadeIn, bool useFadeOut, bool throwExceptionIfAlreadyOpened, int pendingTime) {
			ShowForm(parentForm, splashFormType, useFadeIn, useFadeOut, throwExceptionIfAlreadyOpened, SplashFormStartPosition.Default, Point.Empty, pendingTime);
		}
		public static void ShowForm(Form parentForm, Type splashFormType, bool useFadeIn, bool useFadeOut, bool throwExceptionIfAlreadyOpened, SplashFormStartPosition startPos, Point loc) {
			ShowForm(parentForm, splashFormType, useFadeIn, useFadeOut, throwExceptionIfAlreadyOpened, startPos, loc, ParentFormState.Unlocked);
		}
		public static void ShowForm(Form parentForm, Type splashFormType, bool useFadeIn, bool useFadeOut, bool throwExceptionIfAlreadyOpened, SplashFormStartPosition startPos, Point loc, bool allowGlowEffect) {
			ShowForm(parentForm, splashFormType, useFadeIn, useFadeOut, throwExceptionIfAlreadyOpened, startPos, loc, 0, false, ParentFormState.Unlocked, allowGlowEffect);
		}
		public static void ShowForm(Form parentForm, Type splashFormType, bool useFadeIn, bool useFadeOut, bool throwExceptionIfAlreadyOpened, SplashFormStartPosition startPos, Point loc, ParentFormState parentFormDesiredState) {
			ShowForm(parentForm, splashFormType, useFadeIn, useFadeOut, throwExceptionIfAlreadyOpened, startPos, loc, 0, parentFormDesiredState);
		}
		public static void ShowForm(Form parentForm, Type splashFormType, bool useFadeIn, bool useFadeOut, bool throwExceptionIfAlreadyOpened, SplashFormStartPosition startPos, Point loc, int pendingTime) {
			ShowForm(parentForm, splashFormType, useFadeIn, useFadeOut, throwExceptionIfAlreadyOpened, startPos, loc, pendingTime, ParentFormState.Unlocked);
		}
		public static void ShowForm(Form parentForm, Type splashFormType, bool useFadeIn, bool useFadeOut, bool throwExceptionIfAlreadyOpened, SplashFormStartPosition startPos, Point loc, int pendingTime, ParentFormState parentFormDesiredState) {
			ShowForm(parentForm, splashFormType, useFadeIn, useFadeOut, throwExceptionIfAlreadyOpened, startPos, loc, pendingTime, false, parentFormDesiredState);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void ShowForm(Form parentForm, Type splashFormType, bool useFadeIn, bool useFadeOut, bool throwExceptionIfAlreadyOpened, SplashFormStartPosition startPos, Point loc, int pendingTime, bool allowUseInDT) {
			ShowForm(parentForm, splashFormType, useFadeIn, useFadeOut, throwExceptionIfAlreadyOpened, startPos, loc, pendingTime, allowUseInDT, ParentFormState.Unlocked);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void ShowForm(Form parentForm, Type splashFormType, bool useFadeIn, bool useFadeOut, bool throwExceptionIfAlreadyOpened, SplashFormStartPosition startPos, Point loc, int pendingTime, bool allowUseInDT, ParentFormState parentFormDesiredState) {
			ShowForm(parentForm, splashFormType, useFadeIn, useFadeOut, throwExceptionIfAlreadyOpened, startPos, loc, pendingTime, allowUseInDT, parentFormDesiredState, false);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void ShowForm(Form parentForm, Type splashFormType, bool useFadeIn, bool useFadeOut, bool throwExceptionIfAlreadyOpened, SplashFormStartPosition startPos, Point loc, int pendingTime, bool allowUseInDT, ParentFormState parentFormDesiredState, bool allowGlowEffect) {
			ShowForm(parentForm, splashFormType, useFadeIn, useFadeOut, throwExceptionIfAlreadyOpened, startPos, loc, pendingTime, allowUseInDT, parentFormDesiredState, allowGlowEffect, null);
		}
		static void ShowForm(Form parentForm, Type splashFormType, bool useFadeIn, bool useFadeOut, bool throwExceptionIfAlreadyOpened, SplashFormStartPosition startPos, Point loc, int pendingTime, bool allowUseInDT, ParentFormState parentFormDesiredState, bool allowGlowEffect, Func<SplashFormBase> createFormFunction) {
			if(DesignTimeTools.IsDesignMode && !allowUseInDT)
				return;
			if(Default != null) {
				if(!throwExceptionIfAlreadyOpened) return;
				throw new InvalidOperationException("Splash Form has already been displayed");
			}
			Default = new InnerSplashScreenManager(GetPublicParent(parentForm, splashFormType), splashFormType, useFadeIn, useFadeOut, startPos, loc, pendingTime, parentForm, parentFormDesiredState, allowGlowEffect, createFormFunction);
			if(Default.IsWaitFormUsage) {
				Default.CheckParentForm();
				Default.ShowCore();
			}
		}
		static void ShowForm(UserControl parentControl, Type splashFormType, bool useFadeIn, bool useFadeOut, SplashFormStartPosition startPos, Point loc, ParentFormState parentFormDesiredState, Func<SplashFormBase> createFormFunc) {
			if(!typeof(WaitForm).IsAssignableFrom(splashFormType)) throw new InvalidOperationException("You should use WaitForm only in this case");
			ShowForm(parentControl.FindForm(), splashFormType, useFadeIn, useFadeOut, true, startPos, loc, 0, false, parentFormDesiredState, true, createFormFunc);
		}
		static Form GetPublicParent(Form parentForm, Type splashFormType) {
			return SplashScreenManagerHelper.IsSplashScreen(splashFormType) ? null : parentForm;
		}
		public static bool FormInPendingState {
			get {
				SplashScreenManager manager = Default;
				return manager != null ? manager.FormInPendingStateCore : false;
			}
		}
		public static void CloseForm() {
			CloseForm(true);
		}
		public static void CloseForm(bool throwExceptionIfAlreadyClosed) {
			CloseForm(throwExceptionIfAlreadyClosed, 0, null);
		}
		public static void CloseForm(bool throwExceptionIfAlreadyClosed, bool waitForSplashFormClose) {
			CloseForm(throwExceptionIfAlreadyClosed, 0, null, waitForSplashFormClose);
		}
		public static void CloseForm(bool throwExceptionIfAlreadyClosed, int closingDelay, Form parent) {
			CloseForm(throwExceptionIfAlreadyClosed, closingDelay, parent, false);
		}
		public static void CloseForm(bool throwExceptionIfAlreadyClosed, int closingDelay, Form parent, bool waitForSplashFormClose) {
			CloseForm(throwExceptionIfAlreadyClosed, closingDelay, parent, waitForSplashFormClose, false);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void CloseForm(bool throwExceptionIfAlreadyClosed, int closingDelay, Form parent, bool waitForSplashFormClose, bool allowUseInDT) {
			if(DesignTimeTools.IsDesignMode && !allowUseInDT)
				return;
			if(Default == null) {
				if(!throwExceptionIfAlreadyClosed) return;
				throw new InvalidOperationException("Splash Form is not displayed");
			}
			Default.CloseCore(throwExceptionIfAlreadyClosed, closingDelay, parent);
			if(waitForSplashFormClose)
				Default.WaitForSplashFormClose();
			Default.RemoveCreateFormFunction();
			Default = null;
		}
		public static void ShowImage(Image image) {
			ShowImage(image, true, true);
		}
		public static void ShowImage(Image image, bool useFadeIn) {
			ShowImage(image, useFadeIn, true);
		}
		public static void ShowImage(Image image, bool useFadeIn, bool useFadeOut) {
			ShowImage(image, useFadeIn, useFadeOut, null);
		}
		public static void ShowImage(Image image, bool useFadeIn, bool useFadeOut, SplashFormStartPosition startPos, Point loc) {
			ShowImage(image, useFadeIn, useFadeOut, startPos, loc, null);
		}
		public static void ShowImage(Image image, bool useFadeIn, bool useFadeOut, ICustomImagePainter painter) {
			ShowImage(image, useFadeIn, useFadeOut, SplashFormStartPosition.Default, Point.Empty, painter);
		}
		public static void ShowImage(Image image, bool useFadeIn, bool useFadeOut, SplashFormStartPosition startPos, Point loc, ICustomImagePainter painter) {
			ShowImage(image, useFadeIn, useFadeOut, startPos, loc, painter, 0);
		}
		public static void ShowImage(Image image, bool useFadeIn, bool useFadeOut, SplashFormStartPosition startPos, Point loc, ICustomImagePainter painter, int pendingTime) {
			ShowImage(image, useFadeIn, useFadeOut, startPos, loc, painter, pendingTime, false);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void ShowImage(Image image, bool useFadeIn, bool useFadeOut, SplashFormStartPosition startPos, Point loc, ICustomImagePainter painter, int pendingTime, bool allowUseInDT) {
			if(DesignTimeTools.IsDesignMode && !allowUseInDT)
				return;
			if(Default != null)
				throw new InvalidOperationException("Image has already been displayed");
			SplashFormProperties stateInfo = new SplashFormProperties(null, image, useFadeIn, useFadeOut, painter, pendingTime);
			Default = new InnerSplashScreenManager(typeof(SplashScreenLayer), startPos, loc, stateInfo);
		}
		public static void HideImage() {
			HideImage(0, null);
		}
		public static void HideImage(int closingDelay, Form parent) {
			CloseForm(true, closingDelay, parent);
		}
		static UserSkinsStorage skinStorage = new UserSkinsStorage();
		public static void RegisterUserSkin(SkinBlobXmlCreator creator) {
			skinStorage.Skins.Add(creator);
		}
		public static void RegisterUserSkins(Assembly asm) {
			skinStorage.Asms.Add(asm);
		}
		internal static UserSkinsStorage UserSkinsStorage { get { return skinStorage; } }
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("SplashScreenManagerActivateParentOnSplashFormClosing")]
#endif
		public static bool ActivateParentOnSplashFormClosing { get; set; }
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("SplashScreenManagerActivateParentOnWaitFormClosing")]
#endif
		public static bool ActivateParentOnWaitFormClosing { get; set; }
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("SplashScreenManagerApartmentState")]
#endif
		public static System.Threading.ApartmentState? ApartmentState { get; set; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public static string SkinName { get; set; }
		internal static ParentType GetParentType(Type type) {
			if(type == typeof(UserControl)) return ParentType.UserControl;
			if(type == typeof(Form)) return ParentType.Form;
			throw new ArgumentException("type");
		}
		void MVVMServiceSendCommand(Enum command, object value, bool throwExceptionIfNotOpened) {
			SplashScreenManager defaultManager = Default;
			if(defaultManager == null) {
				if(throwExceptionIfNotOpened) {
					throw new InvalidOperationException("Splash Form is not displayed");
				}
				return;
			}
			defaultManager.SendCommand(command, value);
		}
		void DevExpress.Utils.MVVM.Services.ISplashScreenServiceProvider.HideSplashScreen() {
			SplashScreenManager.CloseForm();
		}
		bool DevExpress.Utils.MVVM.Services.ISplashScreenServiceProvider.IsSplashScreenActive {
			get { return !(Default == null); }
		}
		void DevExpress.Utils.MVVM.Services.ISplashScreenServiceProvider.SetSplashScreenProgress(double progress, double maxProgress) {
			MVVMServiceSendCommand(DemoProgressSplashScreen.CommandId.SetMaxProgressValue, (int)maxProgress, true);
			MVVMServiceSendCommand(DemoProgressSplashScreen.CommandId.SetProgressValue, (int)progress, true);
		}
		void DevExpress.Utils.MVVM.Services.ISplashScreenServiceProvider.SetSplashScreenState(object state) {
			MVVMServiceSendCommand(DemoProgressSplashScreen.CommandId.SetLabel, state, true);
		}
		void DevExpress.Utils.MVVM.Services.ISplashScreenServiceProvider.ShowSplashScreen(Func<Control> createFormFunction) {
			if(createFormFunction == null) 
				SplashScreenManager.ShowForm(this.SplashFormType ?? typeof(DemoProgressSplashScreen));
			else {
				SplashScreenManager.ShowForm(
					this.Properties.ParentForm,
					typeof(WaitForm),
					this.Properties.UseFadeInEffect,
					this.Properties.UseFadeOutEffect,
					true,
					this.SplashFormStartPosition,
					this.SplashFormLocation,
					this.Properties.PendingTime,
					false,
					this.parentFormDesiredState,
					this.Properties.AllowGlowEffect,
					() => CreateForm(createFormFunction));
			}
		}
		SplashFormBase CreateForm(Func<Control> createContolFunction) {
			if(createContolFunction == null) return null;
			Control control = createContolFunction();
			SplashFormBase form = control as SplashFormBase;
			if(form != null) return form;
			SplashFormBase form2 = new SplashFormBase();
			form2.Controls.Add(control);
			return form2;
		}
	}
	[ToolboxItem(false)]
	class InnerSplashScreenManager : SplashScreenManager {
		public InnerSplashScreenManager(Type type, SplashFormStartPosition startPos, Point loc, SplashFormProperties stateInfo)
			: base(type, startPos, loc, stateInfo) {
		}
		public InnerSplashScreenManager(Form form, Type splashFormType, bool useFadeIn, bool useFadeOut, SplashFormStartPosition startPos, Point loc, int pendingTime, Form parentForm, ParentFormState parentFormDesiredState, bool allowGlowEffect)
			: base(form, splashFormType, useFadeIn, useFadeOut, startPos, loc, pendingTime, parentForm, parentFormDesiredState, allowGlowEffect) {
		}
		public InnerSplashScreenManager(Form form, Type splashFormType, bool useFadeIn, bool useFadeOut, SplashFormStartPosition startPos, Point loc, int pendingTime, Form parentForm, ParentFormState parentFormDesiredState, bool allowGlowEffect, Func<SplashFormBase> createFormFunction)
			: base(form, splashFormType, useFadeIn, useFadeOut, startPos, loc, pendingTime, parentForm, parentFormDesiredState, allowGlowEffect, createFormFunction) {
		}
		protected internal override bool IsInnerManager { get { return true; } }
	}
	class SplashScreenManagerHelper {
		public static bool IsSplashScreen(Type type) {
			if(type == null)
				return false;
			return typeof(SplashScreen).IsAssignableFrom(type);
		}
	}
}
namespace DevExpress.XtraSplashScreen.Utils {
	class UserSkinsStorage {
		AssemblyStorage asmsCore;
		SkinCreatorStorage skinsCore;
		public UserSkinsStorage() {
			this.asmsCore = new AssemblyStorage();
			this.skinsCore = new SkinCreatorStorage();
		}
		public AssemblyStorage Asms { get { return this.asmsCore; } }
		public SkinCreatorStorage Skins { get { return this.skinsCore; } }
		public void Clear() {
			this.asmsCore.Clear();
			this.skinsCore.Clear();
		}
	}
	class SkinCreatorStorage : Collection<SkinBlobXmlCreator> {
		public SkinBlobXmlCreator this[string skinName] {
			get {
				foreach(SkinBlobXmlCreator item in this) {
					if(string.Equals(item.SkinName, skinName, StringComparison.Ordinal))
						return item;
				}
				return null;
			}
		}
	}
	class AssemblyStorage : Collection<Assembly> {
	}
	class SkinHelper {
		public static void RegisterUserSkins(string skinName) {
			RegisterAssemliesCore();
			RegisterSkinCore(skinName);
		}
		protected static void RegisterAssemliesCore() {
			if(SplashScreenManager.UserSkinsStorage.Asms.Count == 0) return;
			foreach(Assembly asm in SplashScreenManager.UserSkinsStorage.Asms)
				DevExpress.Skins.SkinManager.Default.RegisterAssembly(asm);
		}
		protected static void RegisterSkinCore(string skinName) {
			SkinBlobXmlCreator skinCreator = SplashScreenManager.UserSkinsStorage.Skins[skinName];
			if(skinCreator != null) SkinManager.Default.RegisterSkin(skinCreator);
		}
	}
}
