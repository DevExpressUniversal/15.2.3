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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.Skins;
using DevExpress.Utils.Paint;
using DevExpress.Utils.Drawing;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.Utils.Design;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.LookAndFeel.Design;
using System.Runtime.InteropServices;
using System.Configuration;
using DevExpress.Utils.Helpers;
using DevExpress.LookAndFeel.Helpers;
namespace DevExpress.LookAndFeel {
	public enum LookAndFeelStyle { Flat, UltraFlat, Style3D, Office2003, Skin }
	public enum ActiveLookAndFeelStyle { Flat, UltraFlat, Style3D, Office2003, Skin,WindowsXP }
	[System.Runtime.InteropServices.ComVisible(false)]
	public interface ISupportLookAndFeel {
		UserLookAndFeel LookAndFeel { get; }
		bool IgnoreChildren { get; }
	}
	[System.Runtime.InteropServices.ComVisible(false)]
	public interface ILookAndFeelProvider {
		UserLookAndFeel LookAndFeel { get; }
	}
	public class LookAndFeelProperties {
		public LookAndFeelStyle Style = LookAndFeelStyle.Skin;
		public bool UseWindowsXPTheme = false, UseDefaultLookAndFeel = false, TouchUI;
		public string SkinName = string.Empty;
		public LookAndFeelProperties(UserLookAndFeel lf) {
			this.Style = lf.Style;
			this.UseWindowsXPTheme = lf.UseWindowsXPTheme;
			this.UseDefaultLookAndFeel = lf.UseDefaultLookAndFeel;
			this.SkinName = lf.SkinName;
		}
		public void SetTo(UserLookAndFeel lf) {
			lf.SetStyle(Style, UseWindowsXPTheme, UseDefaultLookAndFeel, SkinName);
		}
	}
	public enum TouchUIMode { Default, True, False }
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class UserLookAndFeel : IDisposable, ISkinProvider, ISkinProviderEx, IAppearanceDefaultFontProvider {
		[ThreadStatic]
		static UserLookAndFeel defaultLookAndFeel = null;
		static UserLookAndFeel() {
			if(Default != null) return; 
		}
		bool disposing = false;
		protected UserLookAndFeel fParentLookAndFeel;
		protected bool fUseWindowsXPTheme, fUseDefaultLookAndFeel;
		protected LookAndFeelStyle fStyle;
		BaseLookAndFeelPainters painter;
		string skinName;
		object ownerControl;
		public const string DefaultSkinName = SkinRegistrator.DefaultSkinName;
		public delegate void StyleChangedEventHandler<T>(T target, object sender, EventArgs eventArgs);
		interface IEventRecord {
			bool Equals(EventHandler target);
			void Invoke(UserLookAndFeel userLookAndFeel, EventArgs eventArgs);
			bool CanPurge { get; }
			void Dispose();
			object Target { get; }
		}
		class EventRecord<T> : IEventRecord {
			StyleChangedEventHandler<T> method;
			GCHandle handle;
			static GCHandle zeroHandle;
			[System.Security.SecuritySafeCritical]
			static EventRecord() {
				GCHandle h = GCHandle.Alloc(new Object());
				h.Free();
				zeroHandle = h;
			}
			[System.Security.SecuritySafeCritical]
			public EventRecord(EventHandler target) {
				method = (StyleChangedEventHandler<T>)Delegate.CreateDelegate(typeof(StyleChangedEventHandler<T>), null, target.Method);
				handle = GCHandle.Alloc(target.Target, GCHandleType.Weak);
			}
			~EventRecord() {
				Dispose(false);
			}
			public void Dispose() {
				Dispose(true);
			}
			[System.Security.SecuritySafeCritical]
			void Dispose(bool disposing) {
				GCHandle h = handle;
				handle = zeroHandle;
				if(h.IsAllocated)
					h.Free();
				if(disposing)
					GC.SuppressFinalize(this);
			}
			public object Target {
				[System.Security.SecuritySafeCritical]
				get {
					GCHandle h = handle;
					if(!h.IsAllocated)
						return null;
					object res = h.Target;
					return handle.IsAllocated ? res : null;
				}
			}
			public bool Equals(EventHandler target) {
				return target.Target == this.Target && method.Method == target.Method;
			}
			public void Invoke(UserLookAndFeel userLookAndFeel, EventArgs eventArgs) {
				object target = Target;
				if(target != null)
					method((T)target, userLookAndFeel, eventArgs);
			}
			public bool CanPurge {
				get { return Target == null; }
			}
		}
		class StaticEventRecord : IEventRecord {
			EventHandler method;
			public StaticEventRecord(EventHandler target) {
				method = target;
			}
			public bool Equals(EventHandler target) {
				return target.Target == null && method.Method == target.Method;
			}
			public void Invoke(UserLookAndFeel userLookAndFeel, EventArgs eventArgs) {
				method(userLookAndFeel, eventArgs);
			}
			public bool CanPurge { get { return false; } }
			public void Dispose() { }
			public object Target { get { return null; } }
		}
		List<IEventRecord> styleChanged = new List<IEventRecord>();
		protected virtual void OnFirstSubscribe() { 
		}
		protected virtual void OnLastUnsubscribe() { 
		}
		public override string ToString() {
			string res = string.Empty;
			if(UseDefaultLookAndFeel) {
				res = "UseDefault;";
			}
			if(Style != LookAndFeelStyle.Skin) {
				res += Style.ToString();
			}
			else {
				res += "Skin: " + SkinName;
			}
			return res;
		}
		IComponent GetComponent(IEventRecord rec) {
			IComponent comp = rec.Target as IComponent;
			if(comp != null)
				return comp;
			UserLookAndFeel lookAndFeel = rec.Target as UserLookAndFeel;
			if(lookAndFeel != null)
				return lookAndFeel.OwnerControl as IComponent;
			return null;
		} 
		internal bool IsDebugging {
			get {
				foreach(IEventRecord rec in this.styleChanged.ToArray()) {
					IComponent comp = GetComponent(rec);
					if(comp == null || comp.Site == null || !comp.Site.DesignMode)
						continue;
					object loader = comp.Site.GetService(typeof(IResourceService));
					if(loader == null) continue;
					if(loader.GetType().Name == "VSCodeDomDesignerLoader") {
						System.Reflection.PropertyInfo pi = loader.GetType().GetProperty("IsDebugging", BindingFlags.Instance | BindingFlags.NonPublic);
						if (pi != null) return (bool)pi.GetValue(loader, null);
					}
				}
				return false;
			}
		}
		#region Colorization
		Color maskColor = Color.Empty;
		public Color SkinMaskColor {
			get { return maskColor; }
			set {
				if(SkinMaskColor.Equals(value))
					return;
				maskColor = value;
				OnStyleChanged();
			}
		}
		void ResetSkinMaskColor() { SkinMaskColor = Color.Empty; }
		bool ShouldSerializeSkinMaskColor() { return SkinMaskColor != Color.Empty; }
		Color maskColor2 = Color.Empty;
		public Color SkinMaskColor2 {
			get { return maskColor2; }
			set {
				if(SkinMaskColor2.Equals(value))
					return;
				maskColor2 = value;
				OnStyleChanged();
			}
		}
		void ResetSkinMaskColor2() { SkinMaskColor2 = Color.Empty; }
		bool ShouldSerializeSkinMaskColor2() { return SkinMaskColor2 != Color.Empty; }
		#endregion Colorization
		public event EventHandler StyleChanged {
			add {
				PurgeStyleChangedRecords();
				styleChanged.Add(value.Target == null ? (IEventRecord)new StaticEventRecord(value) : (IEventRecord)Activator.CreateInstance(typeof(EventRecord<>).MakeGenericType(value.Method.DeclaringType), value));
				if(styleChanged.Count == 1)
					OnFirstSubscribe();
			}
			remove {
				for(int i = styleChanged.Count - 1; i >= 0; i--) {
					IEventRecord e = styleChanged[i];
					if(e.Equals(value)) {
						styleChanged.RemoveAt(i);
						e.Dispose();
						if(styleChanged.Count == 0)
							OnLastUnsubscribe();
						return;
					}
				}
			}
		}
		void PurgeStyleChangedRecords() {
			if((styleChanged.Count & 0x0F) < 0x0F) 
				return;
			PurgeStyleChangedRecordsCore();
		}
		protected void Purge() {
			PurgeStyleChangedRecordsCore();
			if(styleChanged.Count == 0)
				OnLastUnsubscribe();
		}
		void PurgeStyleChangedRecordsCore() {
			for(int i = styleChanged.Count - 1; i >= 0; i--) {
				IEventRecord e = styleChanged[i];
				if(e.CanPurge) {
					styleChanged.RemoveAt(i);
					e.Dispose();
				}
			}
		}
		public UserLookAndFeel(object ownerControl) {
			FontBehaviorHelper.CheckFont();
			this.ownerControl = ownerControl;
			this.skinName = DefaultSkinName;
			this.fUseDefaultLookAndFeel = true;
			this.fParentLookAndFeel = null;
			this.fUseWindowsXPTheme = false;
			this.fStyle = LookAndFeelStyle.Skin;
			this.painter = null;
			if(AllowParentEvents) {
				SubscribeDefaultChanged();
			} 
		}
		public virtual void Reset() {
			SetStyle(LookAndFeelStyle.Flat, true, true, DefaultSkinName);
		}
		protected virtual void SubscribeDefaultChanged() { 
			Default.StyleChanged += new EventHandler(OnParentStyleChanged);
		}
		string ISkinProvider.SkinName {
			get {
				return ActiveSkinName;
			}
		}
		protected internal virtual bool AllowParentEvents { get { return true; } }
		public virtual void Dispose() {
			this.disposing = true;
			if(styleChanged != null) styleChanged.Clear();
			ParentLookAndFeel = null;
			if(AllowParentEvents) {
				if(Default != null && Default != this) {
					Default.StyleChanged -= new EventHandler(OnParentStyleChanged);
				}
			} 
		}
		internal void SetOwner(object ownerControl) { this.ownerControl = ownerControl; } 
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public object OwnerControl { get { return ownerControl; } }
		public virtual bool ShouldSerialize() {
			return !this.UseWindowsXPTheme || this.Style != LookAndFeelStyle.Flat || !this.UseDefaultLookAndFeel;
		}
		public virtual void Assign(UserLookAndFeel source) {
			if(source == null) return;
			if(IsEquals(source)) return;
			this.fUseDefaultLookAndFeel = source.UseDefaultLookAndFeel;
			this.fParentLookAndFeel = source.ParentLookAndFeel;
			this.fUseWindowsXPTheme = source.UseWindowsXPTheme;
			this.fStyle = source.Style;
			this.skinName = source.SkinName;
			this.maskColor = source.GetMaskColor();
			this.maskColor2 = source.GetMaskColor2();
			OnStyleChanged();
		}
		public void ResetSkinMaskColors() {
			this.maskColor2 = Color.Empty;
			this.maskColor = Color.Empty;
			OnStyleChanged();
		}
		public void SetSkinMaskColors(Color skinMaskColor, Color skinMaskColor2) {
			this.maskColor = skinMaskColor;
			this.maskColor2 = skinMaskColor2;
			OnStyleChanged();
		}
		public virtual bool IsEquals(UserLookAndFeel source) {
			if(source == null) return false;
			if(this.ParentLookAndFeel == source.ParentLookAndFeel && this.Style == source.Style && this.SkinName == source.SkinName && 
				this.UseWindowsXPTheme == source.UseWindowsXPTheme && this.UseDefaultLookAndFeel == source.UseDefaultLookAndFeel) return true;
			return false;
		}
		protected bool IsDefaultLookAndFeel { get { return Default == this || defaultLookAndFeel == null; } }
		internal static void SetDefaultLookAndFeel(UserLookAndFeelDefault lookAndFeel) { defaultLookAndFeel = lookAndFeel; }
#if !SL
	[DevExpressUtilsLocalizedDescription("UserLookAndFeelDefault")]
#endif
		public static UserLookAndFeel Default {
			get {
				if(defaultLookAndFeel == null) defaultLookAndFeel = new UserLookAndFeelDefault();
				return defaultLookAndFeel;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BaseLookAndFeelPainters Painter { 
			get { 
				if(painter == null) painter = CreatePainter();
				return painter; 
			} 
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual UserLookAndFeel ActiveLookAndFeel { get { return GetLookAndFeel(0); } }
		protected virtual UserLookAndFeel GetLookAndFeel(int level) {
			if(this == Default) return Default;
			if(UseDefaultLookAndFeel) {
				if(ParentLookAndFeel != null && level < 20) return ParentLookAndFeel.GetLookAndFeel(level + 1);
				if(Default == null) return this;
				return UserLookAndFeel.Default;
			}
			return this;
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("UserLookAndFeelActiveSkinName"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual string ActiveSkinName {
			get {
				return ActiveLookAndFeel.SkinName;
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("UserLookAndFeelActiveStyle"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual ActiveLookAndFeelStyle ActiveStyle {
			get {
				UserLookAndFeel lf = ActiveLookAndFeel;
				if(lf != this) return lf.ActiveStyle;
				if(UseWindowsXPTheme && DevExpress.Utils.WXPaint.WXPPainter.Default.ThemesEnabled) return ActiveLookAndFeelStyle.WindowsXP;
				return (ActiveLookAndFeelStyle)Style;
			}
		}
		protected virtual BaseLookAndFeelPainters CreatePainter() {
			UserLookAndFeel lf = ActiveLookAndFeel;
			if(lf != this) return lf.CreatePainter();
			return LookAndFeelPainterHelper.GetPainter(this, ActiveStyle);
		}
		public virtual void ResetParentLookAndFeel() { ParentLookAndFeel = null; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public UserLookAndFeel ParentLookAndFeel { 
			get { return fParentLookAndFeel; }
			set {
				if(ParentLookAndFeel == value) return;
				if(Default == value) return;
				if(value == this) return;
				SetParentLookAndFeel(value);
			}
		}
		string GetHashCore() {
			UserLookAndFeel active = ActiveLookAndFeel;
			var font = GetDefaultFont();
			return string.Concat(active.Style, active.UseWindowsXPTheme,"-", active.SkinName, font == null ? "" : font.Name + font.SizeInPoints.ToString());
		}
		protected internal virtual void SetControlParentLookAndFeel(UserLookAndFeel value) {
			SetParentLookAndFeelCore(value);
		}
		protected internal virtual void SetParentLookAndFeelCore(UserLookAndFeel value) {
			if(this == Default) return; 
			string hash = GetHashCore();
			if(ParentLookAndFeel != null) ParentLookAndFeel.StyleChanged -= new EventHandler(OnParentStyleChanged);
			fParentLookAndFeel = value;
			if(ParentLookAndFeel != null) ParentLookAndFeel.StyleChanged += new EventHandler(OnParentStyleChanged);
			string current = GetHashCore();
			if(hash != current || (current != lastStyleChanged && lastStyleChanged != null))
				OnStyleChanged();
			this.lastStyleChanged = current;
		}
		protected virtual void SetParentLookAndFeel(UserLookAndFeel value) {
			SetParentLookAndFeelCore(value);
		}
		bool useWindows7Border = false;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(false)]
		public bool UseWindows7Border {
			get { return useWindows7Border; }
			set {
				if(UseWindows7Border == value)
					return;
				useWindows7Border = value;
				OnStyleChanged();
			}
		}
		[RefreshProperties(RefreshProperties.All), 
#if !SL
	DevExpressUtilsLocalizedDescription("UserLookAndFeelUseWindowsXPTheme"),
#endif
 DefaultValue(false)]
		public virtual bool UseWindowsXPTheme {
			get { return fUseWindowsXPTheme; }
			set {
				if(UseWindowsXPTheme == value) return;
				fUseWindowsXPTheme = value;
				OnStyleChanged();
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("UserLookAndFeelSkinName"),
#endif
 RefreshProperties(RefreshProperties.All), DefaultValue(DefaultSkinName), TypeConverter(typeof(DevExpress.LookAndFeel.Design.SkinNameTypeConverter))]
		public virtual string SkinName {
			get { return skinName; }
			set {
				if(value == null) value = "";
				string vl = SkinManager.Default.GetValidSkinName(value);
				if(vl != value) {
					SkinManager.Default.RegisterDTUserSkins(OwnerControl as IComponent);
				}
				if(SkinName == value) return;
				skinName = value;
				OnStyleChanged();
			}
		}
		[RefreshProperties(RefreshProperties.All), 
#if !SL
	DevExpressUtilsLocalizedDescription("UserLookAndFeelUseDefaultLookAndFeel"),
#endif
 DefaultValue(true)]
		public virtual bool UseDefaultLookAndFeel {
			get { return fUseDefaultLookAndFeel; }
			set {
				if(this == Default) value = true;
				if(UseDefaultLookAndFeel == value) return;
				fUseDefaultLookAndFeel = value;
				OnStyleChanged();
			}
		}
		[RefreshProperties(RefreshProperties.All), 
#if !SL
	DevExpressUtilsLocalizedDescription("UserLookAndFeelStyle"),
#endif
 DefaultValue(LookAndFeelStyle.Skin)]
		public virtual LookAndFeelStyle Style {
			get { return fStyle; }
			set {
				if(Style == value) return;
				fStyle = value;
				OnStyleChanged();
			}
		}
		protected virtual void OnParentStyleChanged(object sender, EventArgs e) {
			OnStyleChanged();
		}
		protected bool InStyleChanged { get; set; }
		string lastStyleChanged = null;
		protected internal virtual void OnStyleChanged() {
			if(InStyleChanged)
				return;
			InStyleChanged = true;
			try {
				this.painter = null;
				if(this.disposing) return;
				this.lastStyleChanged = GetHashCore();
				Control ctrl = OwnerControl as Control;
				if(ctrl != null && ctrl.InvokeRequired) {
					ctrl.Invoke(new MethodInvoker(OnStyleChanged));
					return;
				}
				OnStyleChangeProgress(new LookAndFeelProgressEventArgs(0, styleChanged.Count));
				int progressCount = 0;
				foreach(IEventRecord rec in styleChanged.ToArray()) {
					OnStyleChangeProgress(new LookAndFeelProgressEventArgs(1, progressCount++));
					rec.Invoke(this, EventArgs.Empty);
				}
				OnStyleChangeProgress(new LookAndFeelProgressEventArgs(2, 0));
			} finally { InStyleChanged = false; }
		}
		protected virtual void OnStyleChangeProgress(LookAndFeelProgressEventArgs e) { }
		protected bool HasSubscribers { get { return styleChanged.Count > 0; } }
		public void UpdateStyleSettings() { OnStyleChanged(); }
		public void SetDefaultStyle() {
			SetStyle(LookAndFeelStyle.Skin, false, true, SkinManager.DefaultSkinName);
		}
		public void SetSkinStyle(string skinName) {
			SetStyle(LookAndFeelStyle.Skin, false, false, skinName);
		}
		[Browsable(false), Obsolete("This method is obsolete. Use the SetSkinStyle method without the touchUI parameter. To enable Touch UI, use the WindowsFormsSettings.TouchUIMode property.")]
		public void SetSkinStyle(string skinName, TouchUIMode touchUI) {
			SetStyle(LookAndFeelStyle.Skin, false, false, skinName);
		}
		[Browsable(false), Obsolete("This method is obsolete. Use the SetSkinStyle method without the touchUI parameter. To enable Touch UI, use the WindowsFormsSettings.TouchUIMode property.")]
		public void SetSkinStyle(string skinName, bool touchUI) {
			SetStyle(LookAndFeelStyle.Skin, false, false, skinName);
		}
		public void SetFlatStyle() {
			SetStyle(LookAndFeelStyle.Flat, false, false, SkinName);
		}
		public void SetOffice2003Style() {
			SetStyle(LookAndFeelStyle.Office2003, false, false, SkinName);
		}
		public void SetUltraFlatStyle() {
			SetStyle(LookAndFeelStyle.UltraFlat, false, false, SkinName);
		}
		public void SetStyle3D() {
			SetStyle(LookAndFeelStyle.Style3D, false, false, SkinName);
		}
		public void SetWindowsXPStyle() {
			SetStyle(LookAndFeelStyle.Flat, true, false, SkinName);
		}
		public void SetStyle(LookAndFeelStyle style, bool useWindowsXPTheme, bool useDefaultLookAndFeel) {
			SetStyle(style, useWindowsXPTheme, useDefaultLookAndFeel, SkinName);
		}
		public void SetStyle(LookAndFeelStyle style, bool useWindowsXPTheme, bool useDefaultLookAndFeel, string skinName) {
			skinName = SkinManager.Default.GetValidSkinName(skinName);
			if(Style == style && UseWindowsXPTheme == useWindowsXPTheme &&
				SkinName == skinName && (this == Default || UseDefaultLookAndFeel == useDefaultLookAndFeel)) return;
			this.fStyle = style;
			this.fUseWindowsXPTheme = useWindowsXPTheme;
			this.skinName = skinName;
			if(this != Default)
				this.fUseDefaultLookAndFeel = useDefaultLookAndFeel;
			OnStyleChanged();
		}
		[Browsable(false), Obsolete("This method is obsolete. Use the SetStyle method without the touchUI parameter. To enable Touch UI, use the WindowsFormsSettings.TouchUIMode property.")]
		public void SetStyle(LookAndFeelStyle style, bool useWindowsXPTheme, bool useDefaultLookAndFeel, string skinName, bool touchUI) {
			SetStyle(style, useWindowsXPTheme, useDefaultLookAndFeel, skinName);
		}
		[Browsable(false), Obsolete("This method is obsolete. Use the SetStyle method without the touchUIMode parameter. To enable Touch UI, use the WindowsFormsSettings.TouchUIMode property.")]
		public void SetStyle(LookAndFeelStyle style, bool useWindowsXPTheme, bool useDefaultLookAndFeel, string skinName, TouchUIMode touchUIMode) {
			SetStyle(style, useWindowsXPTheme, useDefaultLookAndFeel, skinName);
		}
		[Browsable(false), Obsolete("This method is obsolete. Use the SetStyle method without the touchUI and touchScaleFactor parameters. To enable Touch UI and TouchScaleFactor, use the WindowsFormsSettings.TouchUIMode and WindowsFormsSettings.TouchScaleFactor properties.")]
		public void SetStyle(LookAndFeelStyle style, bool useWindowsXPTheme, bool useDefaultLookAndFeel, string skinName, bool touchUI, float touchScaleFactor) {
			SetStyle(style, useWindowsXPTheme, useDefaultLookAndFeel, skinName);
		}
		[Browsable(false), Obsolete("This method is obsolete. Use the SetStyle method without the touchUIMode and touchScaleFactor parameters. To enable Touch UI and TouchScaleFactor, use the WindowsFormsSettings.TouchUIMode and WindowsFormsSettings.TouchScaleFactor properties.")]
		public void SetStyle(LookAndFeelStyle style, bool useWindowsXPTheme, bool useDefaultLookAndFeel, string skinName, TouchUIMode touchUIMode, float touchScaleFactor) {
			SetStyle(style, useWindowsXPTheme, useDefaultLookAndFeel, skinName);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool GetTouchUI() {
			UserLookAndFeel look = GetTouchUILookAndFeel();
			if(look != null)
				return look.GetTouchUI();
			return WindowsFormsSettings.TouchUIMode == TouchUIMode.True;
		}
		protected UserLookAndFeel GetTouchUILookAndFeel() {
			int level = 0;
			if(this == Default) return null;
			UserLookAndFeel current = this;
			while(level < 20 && current != null) {
				IFormLookAndFeel form = current as IFormLookAndFeel;
				if(form != null) {
					if(form.FormTouchUIMode != DefaultBoolean.Default)
						return current;
				}
				current = current.ParentLookAndFeel;
				level++;
			}
			return null;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual float GetTouchScaleFactor() {
			UserLookAndFeel look = GetTouchUILookAndFeel();
			if(look != null)
				return look.GetTouchScaleFactor();
			return WindowsFormsSettings.TouchScaleFactor;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual Color GetMaskColor() { 
			UserLookAndFeel res = ActiveLookAndFeel;
			if(UseDefaultLookAndFeel && res != this)
				return res.SkinMaskColor;
			return SkinMaskColor;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual Color GetMaskColor2() {
			UserLookAndFeel res = ActiveLookAndFeel;
			if(UseDefaultLookAndFeel && res != this)
				return res.SkinMaskColor2;
			return SkinMaskColor2;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsColorized {
			get {
				Color mask1 = GetMaskColor();
				Color mask2 = GetMaskColor2();
				return (mask1 != Color.Empty && mask1 != Color.Transparent) || (mask2 != Color.Empty && mask2 != Color.Transparent);
			}
		}
		#region IAppearanceDefaultFontProvider Members
		Font GetDefaultFont() {
			if(defaultFont == null) {
				var a = ActiveLookAndFeel;
				if(a != null) return a.defaultFont;
				return null;
			}
			return defaultFont;
		}
		Font defaultFont;
		Font IAppearanceDefaultFontProvider.DefaultFont {
			get {
				return GetDefaultFont();
			}
			set {
				defaultFont = value;
			}
		}
		#endregion
	}
	public class LookAndFeelPainterHelper {
		static BaseLookAndFeelPainters[] painters;
		static LookAndFeelPainterHelper() {
			painters = new BaseLookAndFeelPainters[Enum.GetValues(typeof(ActiveLookAndFeelStyle)).Length];
			painters[(int)ActiveLookAndFeelStyle.Flat] = new FlatLookAndFeelPainters(null);
			painters[(int)ActiveLookAndFeelStyle.Style3D] = new Style3DLookAndFeelPainters(null);
			painters[(int)ActiveLookAndFeelStyle.UltraFlat] = new UltraFlatLookAndFeelPainters(null);
			painters[(int)ActiveLookAndFeelStyle.Office2003] = new Office2003LookAndFeelPainters(null);
			painters[(int)ActiveLookAndFeelStyle.WindowsXP] = new WindowsXPPainters(null);
			painters[(int)ActiveLookAndFeelStyle.Skin] = new SkinLookAndFeelPainters(null);
		}
		public static BaseLookAndFeelPainters GetPainter(ActiveLookAndFeelStyle activeStyle) {
			return GetPainter(null, activeStyle);
		}
		public static BaseLookAndFeelPainters GetPainter(UserLookAndFeel owner, ActiveLookAndFeelStyle activeStyle) {
			if(activeStyle == ActiveLookAndFeelStyle.WindowsXP && !DevExpress.Utils.WXPaint.WXPPainter.Default.ThemesEnabled)
				activeStyle = ActiveLookAndFeelStyle.Flat;
			if(activeStyle == ActiveLookAndFeelStyle.Skin)
				return new SkinLookAndFeelPainters(owner);
			return painters[(int)activeStyle];
		}
	}
	public class BaseLookAndFeelPainters {
		ObjectPainter fButton;
		BorderPainter fBorder;
		ProgressBarObjectPainter fProgressBar;
		ProgressBarObjectPainter fMarqueeProgressBar; 
		SizeGripObjectPainter fSizeGrip;
		HeaderObjectPainter fHeader;
		ObjectPainter fSortedShape;
		ObjectPainter fOpenCloseButton;
		FooterPanelPainter fFooterPanel;
		FooterCellPainter fFooterCell;
		GridGroupPanelPainter groupPanel;
		IndicatorObjectPainter indicator;
		UserLookAndFeel owner;
		public BaseLookAndFeelPainters(UserLookAndFeel owner) {
			this.owner = owner;
			CreatePainters();
		}
		protected UserLookAndFeel Owner { get { return owner; } }
		protected virtual void CreatePainters() {
		}
		public virtual GridGroupPanelPainter GroupPanel {
			get {
				if(groupPanel == null)
					groupPanel = CreateGroupPanelPainter();
				return groupPanel;
			}
		}
		public virtual ProgressBarObjectPainter ProgressBar {
			get {
				if(fProgressBar == null)
					fProgressBar = CreateProgressBarPainter();
				return fProgressBar;
			}
		}
		public virtual ProgressBarObjectPainter MarqueeProgressBar {
			get {
				if(fMarqueeProgressBar == null)
					fMarqueeProgressBar = CreateMarqueeProgressBarPainter();
				return fMarqueeProgressBar;
			}
		}
		public virtual SizeGripObjectPainter SizeGrip {
			get {
				if(fSizeGrip == null)
					fSizeGrip = CreateSizeGripPainter();
				return fSizeGrip;
			}
		}
		public virtual ObjectPainter Button {
			get {
				if(fButton == null)
					fButton = CreateButtonPainter();
				return fButton;
			}
		}
		public virtual BorderPainter Border {
			get {
				if(fBorder == null)
					fBorder = CreateBorderPainter();
				return fBorder;
			}
		}
		public virtual HeaderObjectPainter Header {
			get {
				if(fHeader == null)
					fHeader = CreateHeaderPainter();
				return fHeader;
			}
		}
		public virtual ObjectPainter SortedShape {
			get {
				if(fSortedShape == null)
					fSortedShape = CreateSortedShapePainter();
				return fSortedShape;
			}
		}
		public virtual ObjectPainter OpenCloseButton {
			get {
				if(fOpenCloseButton == null)
					fOpenCloseButton = CreateOpenCloseButtonPainter();
				return fOpenCloseButton;
			}
		}
		public virtual FooterPanelPainter FooterPanel {
			get {
				if(fFooterPanel == null)
					fFooterPanel = CreateFooterPanelPainter();
				return fFooterPanel;
			}
		}
		public virtual FooterCellPainter FooterCell {
			get {
				if(fFooterCell == null)
					fFooterCell = CreateFooterCellPainter();
				return fFooterCell;
			}
		}
		public virtual IndicatorObjectPainter Indicator {
			get {
				if(indicator == null)
					indicator = CreateIndicatorPainter();
				return indicator;
			}
		}
		protected virtual GridGroupPanelPainter CreateGroupPanelPainter() { return new GridGroupPanelPainter(); }
		protected virtual IndicatorObjectPainter CreateIndicatorPainter() { return new IndicatorObjectPainter(Button); }
		protected virtual ObjectPainter CreateButtonPainter() { return new SimpleButtonObjectPainter(); }
		protected virtual BorderPainter CreateBorderPainter() { return new SimpleBorderPainter(); }
		protected virtual ProgressBarObjectPainter CreateProgressBarPainter() { return new ProgressBarObjectPainter(); }
		protected virtual ProgressBarObjectPainter CreateMarqueeProgressBarPainter() { return new MarqueeProgressBarObjectPainter(); }		
		protected virtual SizeGripObjectPainter CreateSizeGripPainter() { return new SizeGripObjectPainter(); }
		protected virtual HeaderObjectPainter CreateHeaderPainter() { return new HeaderObjectPainter(new SimpleButtonObjectPainter()); }
		protected virtual ObjectPainter CreateSortedShapePainter() { return new SortedShapeObjectPainter(); }
		protected virtual ObjectPainter CreateOpenCloseButtonPainter() { return new OpenCloseButtonObjectPainter(new SimpleButtonObjectPainter()); }
		protected virtual FooterPanelPainter CreateFooterPanelPainter() { return new FooterPanelPainter(new FlatButtonObjectPainter()); }
		protected virtual FooterCellPainter CreateFooterCellPainter() { return new FooterCellPainter(new TextFlatBorderPainter()); }
	}
	public class FlatLookAndFeelPainters : BaseLookAndFeelPainters {
		public FlatLookAndFeelPainters(UserLookAndFeel owner) : base(owner) { }
		protected override ObjectPainter CreateButtonPainter() { return new FlatButtonObjectPainter(); }
		protected override BorderPainter CreateBorderPainter() { return new TextFlatBorderPainter(); }
		protected override HeaderObjectPainter CreateHeaderPainter() { return new HeaderObjectPainter(new FlatButtonObjectPainter()); }
		protected override ObjectPainter CreateOpenCloseButtonPainter() { return new OpenCloseButtonObjectPainter(new FlatButtonObjectPainter()); }
		protected override IndicatorObjectPainter CreateIndicatorPainter() { return new FlatIndicatorObjectPainter(); }
		protected override FooterPanelPainter CreateFooterPanelPainter() { return new FooterPanelPainter(new FlatButtonObjectPainter()); }
		protected override FooterCellPainter CreateFooterCellPainter() { return new FooterCellPainter(new TextFlatBorderPainter()); }
	}
	public interface ITransparentBackgroundManager {
		Color GetForeColor(Control childControl);
		Color GetForeColor(object childObject);
	}
	public interface ITransparentBackgroundManagerEx {
		Color GetEmptyBackColor(Control childControl);
	}
	public class LookAndFeelHelper {
		public static void CheckFont(ISkinProvider provider, AppearanceDefault appearance) {
			IAppearanceDefaultFontProvider fp = provider as IAppearanceDefaultFontProvider;
			if(appearance.Font == null && fp != null && fp.DefaultFont != null) appearance.Font = fp.DefaultFont;
		}
		public static Color CheckTransparentForeColor(ISkinProvider provider, Color color, Control owner) {
			return CheckTransparentForeColor(provider, color, owner, Color.Empty);
		}
		public static Color CheckTransparentForeColor(ISkinProvider provider, Color color, Control owner, Color defColor) {
			return CheckTransparentForeColor(provider, color, owner, owner == null ? true : owner.Enabled, defColor);
		}
		public static Color CheckTransparentForeColor(ISkinProvider provider, Color color, Control owner, bool enabled) {
			return CheckTransparentForeColor(provider, color, owner, enabled, Color.Empty);
		}
		public static Color CheckTransparentForeColor(ISkinProvider provider, Color color, Control owner, bool enabled, Color defColor) {
			if(color != Color.Transparent && color != Color.Empty && color != defColor) return color;
			return GetTransparentForeColor(provider, owner, enabled);
		}
		static bool IsRootDesignModeControl(Control control) {
			XtraUserControl xc = control as XtraUserControl;
			return xc != null && xc.IsRootInDesigner;
		}
		public static Color GetTransparentForeColor(ISkinProvider provider, Control owner, bool enabled) {
			if(IsRootDesignModeControl(owner) || owner == null || owner.Parent == null) return GetSystemColorEx(provider, enabled ? SystemColors.ControlText : SystemColors.GrayText);
			ITransparentBackgroundManager manager = owner.Parent as ITransparentBackgroundManager;
			Color color;
			UserLookAndFeel ulf = provider as UserLookAndFeel;
			if((ulf == null || ulf.ActiveStyle == ActiveLookAndFeelStyle.Skin) && manager != null) color = manager.GetForeColor(owner);
			else color = owner.Parent.ForeColor;
			if(color == Color.Transparent || color == Color.Empty) return GetTransparentForeColor(provider, owner.Parent, enabled);
			return color;
		}
		public static AppearanceDefault CheckColors(ISkinProvider provider, AppearanceDefault appearance, Control owner, bool enabled) {
			CheckTransparentForeColor(provider, appearance, owner, enabled);
			CheckEmptyBackColor(provider, appearance, owner);
			return appearance;
		}
		public static AppearanceDefault CheckColors(ISkinProvider provider, AppearanceDefault appearance, Control owner) {
			return CheckColors(provider, appearance, owner, owner == null ? true : owner.Enabled);
		}
		static bool IsEmpty(Color color) {
			return color.Equals(SkinElementPainter.DefaultColor);
		}
		public static AppearanceDefault CheckEmptyBackColor(ISkinProvider provider, AppearanceDefault appearance, Control owner) {
			if(!IsEmpty(appearance.BackColor)) return appearance;
			appearance.BackColor = GetEmptyBackColor(provider, owner);
			return appearance;
		}
		public static AppearanceDefault CheckTransparentForeColor(ISkinProvider provider, AppearanceDefault appearance, Control owner, bool enabled) {
			appearance.ForeColor = CheckTransparentForeColor(provider, appearance.ForeColor, owner, enabled);
			return appearance;
		}
		public static AppearanceDefault CheckTransparentForeColor(ISkinProvider provider, AppearanceDefault appearance, Control owner) {
			return CheckTransparentForeColor(provider, appearance, owner);
		}
		public static Color CheckEmptyBackColor(ISkinProvider provider, Color color, Control owner) {
			return CheckEmptyBackColor(provider, color, owner, SkinElementPainter.DefaultColor);
		}
		public static Color CheckEmptyBackColor(ISkinProvider provider, Color color, Control owner, Color defColor) {
			if(!IsEmpty(color) && color != defColor) return color;
			return GetEmptyBackColor(provider, owner);
		}
		public static Color GetEmptyBackColor(ISkinProvider provider, Control owner) {
			if(IsRootDesignModeControl(owner) || owner == null || owner.Parent == null) 
				return GetSystemColorEx(provider, SystemColors.Control);
			Color parentColor = owner.Parent.BackColor;
			ITransparentBackgroundManagerEx bgManager = owner.Parent as ITransparentBackgroundManagerEx;
			if(bgManager != null) {
				parentColor = bgManager.GetEmptyBackColor(owner);
				return CheckEmptyBackColor(provider, parentColor, owner.Parent);
			}
			return CheckEmptyBackColor(provider, parentColor, owner.Parent);
		}
		public static Color GetTransparentForeColor(ISkinProvider provider, Control owner) {
			return GetTransparentForeColor(provider, owner, owner == null || owner.Enabled);
		}
		public static AppearanceDefault GetHighlightSearchAppearance(ISkinProvider provider, bool useRegularHighlight) {
			UserLookAndFeel ulf = provider as UserLookAndFeel;
			if(useRegularHighlight || ulf == null || ulf.ActiveStyle != ActiveLookAndFeelStyle.Skin) {
				return new AppearanceDefault(Color.Black, Color.FromArgb(255, 210, 0));
			}
			Skin skin = CommonSkins.GetSkin(provider);
			if(!skin.Colors.Contains(CommonColors.HighlightSearch)) {
				return new AppearanceDefault(Color.Black, Color.FromArgb(255, 210, 0));
			}
			Color back = skin.Colors.GetColor(CommonColors.HighlightSearch);
			Color fore = skin.Colors.GetColor(CommonColors.HighlightSearchText);
			return new AppearanceDefault(fore, back);
		}
		public static Color GetSystemColorEx(ISkinProvider provider, Color color) {
			UserLookAndFeel ulf = provider as UserLookAndFeel;
			if(!color.IsSystemColor || (ulf != null && ulf.ActiveStyle != ActiveLookAndFeelStyle.Skin)) return color;
			string name = null;
			switch(color.ToKnownColor()) {
				case KnownColor.Highlight : name = CommonColors.Highlight; break;
				case KnownColor.HighlightText : name = CommonColors.HighlightText; break;
				case KnownColor.Control : name = CommonColors.Control; break;
				case KnownColor.ControlText : name = CommonColors.ControlText; break;
				case KnownColor.GrayText : name = CommonColors.DisabledText; break;
				case KnownColor.Window : name = CommonColors.Window; break;
				case KnownColor.WindowText : name = CommonColors.WindowText; break;
				case KnownColor.Menu : name = CommonColors.Menu; break;
				case KnownColor.MenuText : name= CommonColors.MenuText; break;
				case KnownColor.Info : name = CommonColors.Info; break;
				case KnownColor.InfoText : name= CommonColors.InfoText; break;
				case KnownColor.ControlLight: name = CommonColors.ReadOnly; break;
			}
			if(name != null) return CommonSkins.GetSkin(provider).Colors.GetColor(name);
			return color;
		}
		public static Color GetSystemColor(UserLookAndFeel lookAndFeel, Color color) {
			if(lookAndFeel == null) lookAndFeel = UserLookAndFeel.Default;
			if(lookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.Skin) return color;
			return GetSystemColorEx(lookAndFeel, color);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static bool IsForcedLookAndFeelChange { get; private set; }
		public static void ForceDefaultLookAndFeelChanged() {
			IsForcedLookAndFeelChange = true;
			try {
				UserLookAndFeel.Default.OnStyleChanged();
			}
			finally {
				IsForcedLookAndFeelChange = false;
			}
		}
	}
	public class SkinLookAndFeelPainters : FlatLookAndFeelPainters {
		public SkinLookAndFeelPainters(UserLookAndFeel owner) : base(owner) { }
		protected override GridGroupPanelPainter CreateGroupPanelPainter() { return new SkinGridGroupPanelPainter(Owner); }
		protected override ObjectPainter CreateButtonPainter() { return new SkinButtonObjectPainter(Owner); }
		protected override BorderPainter CreateBorderPainter() { return new SkinTextBorderPainter(Owner); }
		protected override HeaderObjectPainter CreateHeaderPainter() { return new SkinHeaderObjectPainter(Owner); }
		protected override ObjectPainter CreateOpenCloseButtonPainter() { return new SkinOpenCloseButtonObjectPainter(Owner); }
		protected override SizeGripObjectPainter CreateSizeGripPainter() { return new SkinSizeGripObjectPainter(Owner); }
		protected override IndicatorObjectPainter CreateIndicatorPainter() { return new SkinIndicatorObjectPainter(Owner); }
		protected override FooterPanelPainter CreateFooterPanelPainter() { return new SkinFooterPanelPainter(Owner); }
		protected override FooterCellPainter CreateFooterCellPainter() { return new SkinFooterCellPainter(Owner); }
		protected override ObjectPainter CreateSortedShapePainter() { return new SkinSortedShapeObjectPainter(Owner); }
	}
	public class UltraFlatLookAndFeelPainters : BaseLookAndFeelPainters {
		public UltraFlatLookAndFeelPainters(UserLookAndFeel owner) : base(owner) { }
		protected override ObjectPainter CreateButtonPainter() { return new UltraFlatButtonObjectPainter(); }
		protected override BorderPainter CreateBorderPainter() { return new UltraFlatBorderPainter(); }
		protected override ProgressBarObjectPainter CreateProgressBarPainter() { return new ProgressBarObjectPainter(); }
		protected override HeaderObjectPainter CreateHeaderPainter() { return new UltraFlatHeaderObjectPainter(); }
		protected override ObjectPainter CreateOpenCloseButtonPainter() { return new OpenCloseButtonObjectPainter(new SimpleButtonObjectPainter()); }
		protected override IndicatorObjectPainter CreateIndicatorPainter() { return new UltraFlatIndicatorObjectPainter(); }
		protected override FooterPanelPainter CreateFooterPanelPainter() { return new FooterPanelPainter(new GridUltraFlatButtonPainter()); }
		protected override FooterCellPainter CreateFooterCellPainter() { return new FooterCellPainter(new SimpleBorderPainter()); }
	}
	public class Style3DLookAndFeelPainters : BaseLookAndFeelPainters {
		public Style3DLookAndFeelPainters(UserLookAndFeel owner) : base(owner) { }
		protected override ObjectPainter CreateButtonPainter() { return new Style3DButtonObjectPainter(); }
		protected override BorderPainter CreateBorderPainter() { return new Border3DSunkenPainter(); }
		protected override HeaderObjectPainter CreateHeaderPainter() { return new HeaderObjectPainter(new Style3DButtonObjectPainter()); }
		protected override ObjectPainter CreateSortedShapePainter() { return new FlatSortedShapeObjectPainter(); }
		protected override ObjectPainter CreateOpenCloseButtonPainter() { return new OpenCloseButtonObjectPainter(new Style3DButtonObjectPainter()); }
		protected override IndicatorObjectPainter CreateIndicatorPainter() { return new Style3DIndicatorObjectPainter(); }
		protected override FooterPanelPainter CreateFooterPanelPainter() { return new FooterPanelPainter(new Style3DButtonObjectPainter()); }
		protected override FooterCellPainter CreateFooterCellPainter() { return new FooterCellPainter(new Border3DSunkenPainter()); }
	}
	public class WindowsXPPainters : BaseLookAndFeelPainters {
		public WindowsXPPainters(UserLookAndFeel owner) : base(owner) { }
		protected override ObjectPainter CreateButtonPainter() { return new XPButtonPainter(); }
		protected override BorderPainter CreateBorderPainter() { return new WindowsXPButtonEditBorderPainter(); }
		protected override ProgressBarObjectPainter CreateProgressBarPainter() { return new WindowsXPProgressBarObjectPainter(); }
		protected override ProgressBarObjectPainter CreateMarqueeProgressBarPainter() { return new WindowsXPMarqueeProgressBarObjectPainter(); }
		protected override SizeGripObjectPainter CreateSizeGripPainter() { return new WindowsXPSizeGripObjectPainter(); }
		protected override HeaderObjectPainter CreateHeaderPainter() { return new WindowsXPHeaderObjectPainter(); }
		protected override ObjectPainter CreateSortedShapePainter() { return new WindowsXPSortedShapeObjectPainter(); }
		protected override ObjectPainter CreateOpenCloseButtonPainter() { return new WindowsXPOpenCloseButtonObjectPainter(); }
		protected override IndicatorObjectPainter CreateIndicatorPainter() { return new XPIndicatorObjectPainter(); }
		protected override FooterPanelPainter CreateFooterPanelPainter() { return new FooterPanelPainter(new GridWindowsXPButtonPainter()); }
		protected override FooterCellPainter CreateFooterCellPainter() { return new FooterCellPainter(new TextFlatBorderPainter()); }
	}
	public class Office2003LookAndFeelPainters : WindowsXPPainters {
		public Office2003LookAndFeelPainters(UserLookAndFeel owner) : base(owner) { }
		public bool AllowWindowsXP {
			get { return DevExpress.Utils.WXPaint.WXPPainter.Default.ThemesEnabled; }
		}
		protected override GridGroupPanelPainter CreateGroupPanelPainter() { return new Office2003GridGroupPanelPainter(); }
		protected override ObjectPainter CreateButtonPainter() { return new Office2003ButtonObjectPainter(); }
		protected override BorderPainter CreateBorderPainter() { return new Office2003BorderPainter(); }
		protected override HeaderObjectPainter CreateHeaderPainter() { return new Office2003HeaderObjectPainter(); }
		protected override ObjectPainter CreateOpenCloseButtonPainter() { return new Office2003OpenCloseButtonObjectPainter(); }
		protected override IndicatorObjectPainter CreateIndicatorPainter() { return new Office2003IndicatorObjectPainter(); }
		protected override ProgressBarObjectPainter CreateProgressBarPainter() { 
			return AllowWindowsXP ? new WindowsXPProgressBarObjectPainter() : new ProgressBarObjectPainter(); 
		}
		protected override ProgressBarObjectPainter CreateMarqueeProgressBarPainter() {
			return AllowWindowsXP ? new WindowsXPMarqueeProgressBarObjectPainter() : new MarqueeProgressBarObjectPainter() as ProgressBarObjectPainter;
		}
		protected override SizeGripObjectPainter CreateSizeGripPainter() { 
			return AllowWindowsXP ? new WindowsXPSizeGripObjectPainter() : new SizeGripObjectPainter(); 
		}
		protected override ObjectPainter CreateSortedShapePainter() { 
			return AllowWindowsXP ? new WindowsXPSortedShapeObjectPainter() : new SortedShapeObjectPainter(); 
		}
		protected override FooterPanelPainter CreateFooterPanelPainter() { return new FooterPanelPainter(new Office2003FooterPanelObjectPainter()); } 
		protected override FooterCellPainter CreateFooterCellPainter() { return new Office2003FooterCellPainter(); }
	}
	public class LookAndFeelProgressEventArgs : EventArgs {
		int state, progress = 0;
		public LookAndFeelProgressEventArgs(int state, int progress) {
			this.state = state;
			this.progress = progress;
		}
		public int State { get { return state; } } 
		public int Progress { get { return progress; } } 
	}
	public delegate void LookAndFeelProgressEventHandler(object sender, LookAndFeelProgressEventArgs e);
	[Designer("DevExpress.LookAndFeel.Design.DefaultLookAndFeelDesigner, " + AssemblyInfo.SRAssemblyDesignFull),
	 Description("Manipulates default look and feel settings used by all controls in an application."),
	 ToolboxTabName(AssemblyInfo.DXTabComponents),
	 ToolboxBitmap(typeof(DevExpress.Utils.ToolBoxIcons.ToolboxIconsRootNS), "DefaultLookAndFeel"),
	 DXToolboxItem(DXToolboxItemKind.Free)
	]
	public class DefaultLookAndFeel : Component {
		bool enableBonusSkins;
		public DefaultLookAndFeel(IContainer container) : this() {
			this.enableBonusSkins = false;
			if(container != null) container.Add(this);
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("DefaultLookAndFeelEnableBonusSkins"),
#endif
 Browsable(false), DefaultValue(false)]
		public bool EnableBonusSkins {
			get { return enableBonusSkins; }
			set {
				if(EnableBonusSkins == value)
					return;
				enableBonusSkins = value;
				OnEnableBonusSkinsChanged(EnableBonusSkins);
			}
		}
		protected virtual void OnEnableBonusSkinsChanged(bool enabled) {
			if(enabled && !DesignMode) DesignTimeTools.EnsureBonusSkins();
		}
		public DefaultLookAndFeel() {
			((UserLookAndFeelDefault)UserLookAndFeel.Default).userLookAndFeelModified = true;
			if(LookAndFeel.OwnerControl == null)
				LookAndFeel.SetDefaultStyle();
			if(LookAndFeel.OwnerControl == null)
				LookAndFeel.SetOwner(this);
		}
		protected override void Dispose(bool disposing) {
			LookAndFeel.SetOwner(null);
			base.Dispose(disposing);
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("DefaultLookAndFeelLookAndFeel"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public UserLookAndFeel LookAndFeel { get { return UserLookAndFeel.Default; } }
	}
}
namespace DevExpress.LookAndFeel.Helpers {
	public class EmbeddedLookAndFeel : UserLookAndFeel {
		public EmbeddedLookAndFeel() : base(null) { }
		public override void Reset() {
			this.UseWindowsXPTheme = true;
			this.UseDefaultLookAndFeel = false;
			this.Style = LookAndFeelStyle.Flat;
		}
		protected internal override bool AllowParentEvents { get { return false; } }
		public override bool UseDefaultLookAndFeel { get { return false; } }
	}
	public class SkinEmbeddedLookAndFeel : EmbeddedLookAndFeel {
		ISkinProvider provider;
		public SkinEmbeddedLookAndFeel(ISkinProvider provider) { 
			this.provider = provider;
			Reset();
		}
		public override void Reset() {
			this.UseWindowsXPTheme = false;
			this.UseDefaultLookAndFeel = false;
			this.Style = LookAndFeelStyle.Skin;
		}
		ISkinProviderEx ProviderEx { get { return this.provider as ISkinProviderEx; } }
		public override bool UseWindowsXPTheme { get { return false; } }
		public override LookAndFeelStyle Style { get { return LookAndFeelStyle.Skin; } }
		public override string ActiveSkinName { get { return provider.SkinName; } }
		public override string SkinName { get { return ActiveSkinName; } }
		public override UserLookAndFeel ActiveLookAndFeel { get { return this; } }
		public override Color GetMaskColor() {
			if(ProviderEx != null)
				return ProviderEx.GetMaskColor();
			return base.GetMaskColor();
		}
		public override Color GetMaskColor2() {
			if(ProviderEx != null)
				return ProviderEx.GetMaskColor2();
			return base.GetMaskColor2();
		}
		public override bool GetTouchUI() {
			if(ProviderEx != null)
				return ProviderEx.GetTouchUI();
			return base.GetTouchUI();
		}
		public override float GetTouchScaleFactor() {
			if(ProviderEx != null)
				return ProviderEx.GetTouchScaleFactor();
			return base.GetTouchScaleFactor();
		}
	}
	public interface IFormLookAndFeel {
		DefaultBoolean FormTouchUIMode { get; set; }
		float FormTouchScaleFactor { get; set; }
		bool IsUserControl { get; }
	}
	public class ContainerUserLookAndFeel : UserLookAndFeel, IFormLookAndFeel { 
		public ContainerUserLookAndFeel(object owner) : base(owner) {
			Container.ControlAdded += Container_ControlAdded;
		}
		public override void Dispose() {
			Container.ControlAdded -= Container_ControlAdded;
			base.Dispose();
		}
		void Container_ControlAdded(object sender, ControlEventArgs e) {
			UpdateControlLookAndFeel(Container, Container, this);
		}
		protected Control Container { get { return (Control)OwnerControl; } }
		float touchScaleFactor = 2.0f;
		[DefaultValue(2.0f)]
		public virtual float TouchScaleFactor {
			get { return touchScaleFactor; }
			set {
				if(TouchScaleFactor == value)
					return;
				touchScaleFactor = value;
				OnStyleChanged();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DefaultBoolean FormTouchUIMode { get { return TouchUIMode; } set { TouchUIMode = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		float IFormLookAndFeel.FormTouchScaleFactor { get { return TouchScaleFactor; } set { TouchScaleFactor = value; } }
		DefaultBoolean touchUIMode = DefaultBoolean.Default;
		[DefaultValue(DefaultBoolean.Default)]
		public virtual DefaultBoolean TouchUIMode {
			get { return touchUIMode; }
			set {
				if(TouchUIMode == value)
					return;
				touchUIMode = value;
				OnStyleChanged();
			}
		}
		public override bool GetTouchUI() {
			if(TouchUIMode != DefaultBoolean.Default)
				return TouchUIMode == DefaultBoolean.True;
			return base.GetTouchUI();
		}
		public override float GetTouchScaleFactor() {
			if(TouchUIMode == DefaultBoolean.Default)
				return base.GetTouchScaleFactor();
			return TouchUIMode == DefaultBoolean.True ? TouchScaleFactor : 1.0f;
		}
		delegate void ResumeInvoker(bool performLayout);
		protected internal override void OnStyleChanged() {
			if(HasSubscribers && Container.IsHandleCreated) {
				try {
					if(Container.InvokeRequired)
						Container.BeginInvoke(new MethodInvoker(Container.SuspendLayout));
					else
						Container.SuspendLayout();
					base.OnStyleChanged();
					UpdateChildrenLookAndFeel(Container, Container, this);
				}
				finally {
					if(Container.InvokeRequired)
						Container.BeginInvoke(new ResumeInvoker(Container.ResumeLayout), new object[] { true });
					else
						Container.ResumeLayout(true);
				}
			}
			else
				base.OnStyleChanged();
		}
		bool IFormLookAndFeel.IsUserControl { get { return true; } }
		internal static void UpdateControlChildrenLookAndFeel(Control owner, Control control, UserLookAndFeel lookAndFeel) {
			ISupportLookAndFeel lf = owner as ISupportLookAndFeel;
			if(lf != null && lf.LookAndFeel != null) {
				lf.LookAndFeel.SetParentLookAndFeelCore(lookAndFeel);
			}
			UpdateContainerChildrenLookAndFeel(owner, control, lookAndFeel);
		}
		internal static void UpdateContainerChildrenLookAndFeel(Control owner, Control control, UserLookAndFeel lookAndFeel) {
			if(control != owner) {
				ISupportLookAndFeel sp = control as ISupportLookAndFeel;
				if(sp != null)
					sp.LookAndFeel.SetControlParentLookAndFeel(lookAndFeel);
			}
			for(int n = control.Controls.Count - 1; n >= 0; n--) {
				UpdateContainerChildrenLookAndFeel(control, control.Controls[n], lookAndFeel);
			}
		}
		internal static void UpdateControlLookAndFeel(Control owner, Control control, UserLookAndFeel lookAndFeel) {
			UpdateChildrenLookAndFeel(owner, control, lookAndFeel);
		}
		internal static void UpdateChildrenLookAndFeel(Control owner, Control control, UserLookAndFeel lookAndFeel) {
			if(control == null) return;
			if(control != owner) {
				if(control is ILookAndFeelProvider) return;
				ISupportLookAndFeel lf = control as ISupportLookAndFeel;
				if(lf != null) {
					if(lf.LookAndFeel is IFormLookAndFeel && !((IFormLookAndFeel)lf.LookAndFeel).IsUserControl) return;
					if(lf.LookAndFeel != null) lf.LookAndFeel.SetControlParentLookAndFeel(lookAndFeel);
					if(lf.LookAndFeel is IFormLookAndFeel && ((IFormLookAndFeel)lf.LookAndFeel).IsUserControl)
						return;
					if(lf.IgnoreChildren) return;
				}
			}
			for(int n = control.Controls.Count - 1; n >= 0; n--) UpdateChildrenLookAndFeel(owner, control.Controls[n], lookAndFeel);
		}
	}
	public class FormUserLookAndFeel : ContainerUserLookAndFeel, IFormLookAndFeel {
		public FormUserLookAndFeel(Form owner) : base(owner) { }
		bool IFormLookAndFeel.IsUserControl { get { return false; } }
	}
	public class ControlContainerLookAndFeelHelper {
		public static void UpdateChildrenLookAndFeel(Control control, UserLookAndFeel lookAndFeel) {
			ISupportLookAndFeel lf = control as ISupportLookAndFeel;
			if(lf == null) return;
			ControlUserLookAndFeel clf = lf.LookAndFeel as ControlUserLookAndFeel;
			if(clf != null) {
				clf.UpdateChildrenControlsLookAndFeel(lookAndFeel);
			}
		}
		public static void UpdateChildrenLookAndFeel(Control control) {
			ISupportLookAndFeel lf = control as ISupportLookAndFeel;
			if(lf == null) return;
			ControlUserLookAndFeel clf = lf.LookAndFeel as ControlUserLookAndFeel;
			if(clf != null) {
				clf.UpdateChildrenControlsLookAndFeel();
			}
		}
		public static void UpdateControlChildrenLookAndFeel(Control control, UserLookAndFeel lookAndFeel) {
			ISupportLookAndFeel lf = control as ISupportLookAndFeel;
			if(lf == null) return;
			ContainerUserLookAndFeel.UpdateControlChildrenLookAndFeel(control, control, lookAndFeel);
		}
	}
	public class ControlUserLookAndFeel : UserLookAndFeel {
		Control owner;
		bool userParentLookAndFeel = false;
		public ControlUserLookAndFeel(Control owner) : base(owner) {
			this.owner = owner;
			this.owner.ParentChanged += new EventHandler(OnControl_ParentChanged);
			this.owner.HandleCreated += new EventHandler(OnControl_HandleCreated);
		}
		public override void Assign(UserLookAndFeel source) {
			if(!source.AllowParentEvents) {
				this.userParentLookAndFeel = true;
			}
			base.Assign(source);
		}
		public override void Dispose() {
			this.owner.ParentChanged -= new EventHandler(OnControl_ParentChanged);
			this.owner.HandleCreated -= new EventHandler(OnControl_HandleCreated);
			base.Dispose();
		}
		protected UserLookAndFeel GetParentControlLookAndFeel() {
			ISupportLookAndFeel res = Owner.FindForm() as ISupportLookAndFeel;
			if(res != null) {
				Control ctrl = Owner;
				while(ctrl != null) {
					ILookAndFeelProvider provider = ctrl as ILookAndFeelProvider;
					if(provider != null)
						return provider.LookAndFeel;
					ISupportLookAndFeel sl = ctrl as ISupportLookAndFeel;
					if(sl != null && sl.IgnoreChildren) 
						return null;
					ctrl = ctrl.Parent;
				}
			}
			return res == null ? null : res.LookAndFeel;
		}
		protected internal void UpdateParentControlLookAndFeel() {
			UserLookAndFeel lf = GetParentControlLookAndFeel();
			if(lf != null) SetControlParentLookAndFeel(lf);
		}
		protected internal void UpdateChildrenControlsLookAndFeel(UserLookAndFeel parentLookAndFeel) {
			SetControlParentLookAndFeel(parentLookAndFeel);
			UpdateChildrenControlsLookAndFeel(Owner, parentLookAndFeel);
		}
		protected internal void UpdateChildrenControlsLookAndFeel() {
			UpdateParentControlLookAndFeel();
			if(ParentLookAndFeel == null) return;
			UpdateChildrenControlsLookAndFeel(Owner, ParentLookAndFeel);
		}
		protected void UpdateChildrenControlsLookAndFeel(Control control, UserLookAndFeel parentLookAndFeel) {
			if(control != Owner) {
				ISupportLookAndFeel sp = control as ISupportLookAndFeel;
				if(sp != null) sp.LookAndFeel.SetControlParentLookAndFeel(parentLookAndFeel);
			}
			for(int n = control.Controls.Count - 1; n >= 0; n--) {
				UpdateChildrenControlsLookAndFeel(control.Controls[n], parentLookAndFeel);
			}
		}
		public override void ResetParentLookAndFeel() {
			this.userParentLookAndFeel = false;
			UserLookAndFeel lf = GetParentControlLookAndFeel();
			SetControlParentLookAndFeel(lf);
		}
		protected Control Owner { get { return owner; } }
		protected override void SetParentLookAndFeel(UserLookAndFeel value) {
			this.userParentLookAndFeel = true;
			SetParentLookAndFeelCore(value);
		}
		protected internal override void SetControlParentLookAndFeel(UserLookAndFeel value) {
			if(this.userParentLookAndFeel) return;
			base.SetControlParentLookAndFeel(value);
		}
		void OnControl_HandleCreated(object sender, EventArgs e) {
			OnControl_ParentChanged(sender, e);
		}
		void OnControl_ParentChanged(object sender, EventArgs e) {
			if(this.userParentLookAndFeel || ParentLookAndFeel != null) return;
			UpdateParentControlLookAndFeel();
		}
	}
}
namespace DevExpress.LookAndFeel.Design {
	public class AppSettings : ApplicationSettingsBase {
		public AppSettings(IComponent component) : base(component) { }
		public AppSettings(IComponent component, string test) : base(component, test) { }
		public AppSettings(string test) : base(test) { }
		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute(null)]
		public string DefaultAppSkin {
			get {
				return ((string)(this["DefaultAppSkin"]));
			}
		}
		[global::System.Configuration.DefaultSettingValueAttribute("false")]
		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		public bool RegisterUserSkins {
			get {
				return ((bool)(this["RegisterUserSkins"]));
			}
		}
		[global::System.Configuration.DefaultSettingValueAttribute("false")]
		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		public bool TouchUI {
			get {
				return ((bool)(this["TouchUI"]));
			}
		}
		[global::System.Configuration.DefaultSettingValueAttribute("1.0")]
		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		public float TouchScaleFactor {
			get {
				return ((float)(this["TouchScaleFactor"]));
			}
		}
		[global::System.Configuration.DefaultSettingValueAttribute("")]
		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		public string DefaultAppFont {
			get {
				return ((string)(this["DefaultAppFont"]));
			}
		}
		[global::System.Configuration.DefaultSettingValueAttribute("")]
		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		public string InternalSkin {
			get {
				return ((string)(this["InternalSkin"]));
			}
		}
		[global::System.Configuration.DefaultSettingValueAttribute("")]
		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		public string FontBehavior {
			get {
				return ((string)(this["FontBehavior"]));
			}
		}
	}
	public class UserLookAndFeelDefault : UserLookAndFeel {
		internal bool userLookAndFeelModified = false;
		internal static bool? enableDesignTimeFormSkin = null;
		internal static bool? designTimeUserSkin = null;
		static object defaultDesignTimeLookAndFeel = null;
		static string GetRegistryKeyValue(string keyName, string defaultValue) {
			string res = defaultValue;
			try {
				Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"Software\Developer Express");
				if(key != null) {
					res = key.GetValue(keyName, defaultValue).ToString();
					key.Close();
				}
			}
			catch {
				res = defaultValue;
			}
			return res;
		}
		internal static string DefaultDesignTimeLookAndFeel {
			get {
				if(defaultDesignTimeLookAndFeel == null) {
					defaultDesignTimeLookAndFeel = GetRegistryKeyValue("DefaultDesignTimeLookAndFeel", "Skin");
				}
				return defaultDesignTimeLookAndFeel.ToString();
			}
		}
		internal static bool DesignTimeUserSkin {
			get {
				if(designTimeUserSkin == null) {
					try {
						designTimeUserSkin = false;
						designTimeUserSkin = bool.Parse(GetRegistryKeyValue("DesignTimeUserSkin", "false"));
					}
					catch {
					}
				}
				return (bool)designTimeUserSkin;
			}
		}
		internal static bool EnableDesignTimeFormSkin {
			get {
				if(enableDesignTimeFormSkin == null) {
					try {
						enableDesignTimeFormSkin = false;
						enableDesignTimeFormSkin = bool.Parse(GetRegistryKeyValue("EnableDesignTimeFormSkin", "false"));
					}
					catch {
					}
				}
				return (bool)enableDesignTimeFormSkin;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetUserLookAndFeelModified() {
			this.userLookAndFeelModified = true;
		}
		protected override void OnFirstSubscribe() {
			Microsoft.Win32.SystemEvents.UserPreferenceChanged += new Microsoft.Win32.UserPreferenceChangedEventHandler(OnUserPreferencesChanged);
		}
		protected override void OnLastUnsubscribe() {
			Microsoft.Win32.SystemEvents.UserPreferenceChanged -= new Microsoft.Win32.UserPreferenceChangedEventHandler(OnUserPreferencesChanged);
		}
		public UserLookAndFeelDefault()
			: base(null) {
		}
		public override void Dispose() {
			base.Dispose();
			Purge();
		}
		void OnUserPreferencesChanged(object sender, Microsoft.Win32.UserPreferenceChangedEventArgs e) {
			if(e.Category != Microsoft.Win32.UserPreferenceCategory.Color) return;
			ResourceCache.DefaultCache.ClearPartial();
			DevExpress.Utils.WXPaint.WXPPainter.Default.OnThemeChanged();
			Office2003Colors.Default.Init();
			OnStyleChanged();
		}
		protected override void SubscribeDefaultChanged() {
		}
		protected internal override void OnStyleChanged() {
			this.userLookAndFeelModified = true;
			base.OnStyleChanged();
		}
		event LookAndFeelProgressEventHandler styleChangeProgress;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public event LookAndFeelProgressEventHandler StyleChangeProgress {
			add { styleChangeProgress += value; }
			remove { styleChangeProgress -= value; }
		}
		protected override void OnStyleChangeProgress(LookAndFeelProgressEventArgs e) {
			if(styleChangeProgress != null) styleChangeProgress(this, e);
		}
		public void UpdateDesignTimeLookAndFeelEx(IComponent component) {
			if(component == null || component.Site == null) return;
			UpdateDesignTimeLookAndFeel(component, component.Site.GetService(typeof(IDesignerHost)));
		}
		public bool LoadSettings(MethodInvoker registerUserSkinMethod) {
			return LoadSettings((AppSettings)System.Configuration.ApplicationSettingsBase.Synchronized(new AppSettings("")), registerUserSkinMethod);
		}
		public bool LoadSettings(AppSettings settings, MethodInvoker registerUserSkinMethod) {
			try {
				try {
					if(registerUserSkinMethod != null) registerUserSkinMethod();
				}
				catch { }
				string internalSkin = settings.InternalSkin;
				string skin = settings.DefaultAppSkin;
				bool registerUserSkins = settings.RegisterUserSkins;
				string defaultFont = settings.DefaultAppFont;
				string fontBehavior = settings.FontBehavior;
				float touchScaleFactor = settings.TouchScaleFactor;
				bool touchUI = settings.TouchUI;
				bool modified = false;
				if(!string.IsNullOrEmpty(defaultFont)) {
					modified = true;
					SetDefaultApplicationFont(defaultFont);
				}
				if(!string.IsNullOrEmpty(fontBehavior)) {
					modified = true;
					UpdateFontBehavior(fontBehavior);
				}
				if(!string.IsNullOrEmpty(skin)) {
					modified = true;
					SetDefaultDesignTimeLookAndFeel(skin, registerUserSkins);
				}
				if(touchUI) {
					modified = true;
					WindowsFormsSettings.TouchUIMode = TouchUIMode.True; 
				}
				if(touchScaleFactor != 1.0f) {
					modified = true;
					WindowsFormsSettings.TouchScaleFactor = settings.TouchScaleFactor;
				}
				return modified;
			}
			catch {
				return false;
			}
		}
		void UpdateFontBehavior(string fontBehavior) {
			WindowsFormsFontBehavior bf;
			if(!Enum.TryParse<WindowsFormsFontBehavior>(fontBehavior, out bf)) return;
			WindowsFormsSettings.FontBehavior = bf;
		}
		private void LoadInternalSkin(string internalSkin) {
		}
		protected bool ReadApplicationSettings(IComponent component) {
			if(component == null) return false;
			try {
				AppSettings settings = (AppSettings)System.Configuration.ApplicationSettingsBase.Synchronized(new AppSettings(component, ""));
				return LoadSettings(settings, null);
			}
			catch {
				return false;
			}
		}
		void SetDefaultApplicationFont(string defaultFont) {
			if(string.IsNullOrEmpty(defaultFont)) return;
			string[] split = defaultFont.Split(';');
			float fontSize = 0;
			if(split.Length < 2 || !float.TryParse(split[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out fontSize)) {
				fontSize = AppearanceObject.DefaultFont.Size;
			}
			try {
				Font f = new Font(split[0], fontSize);
				FontBehaviorHelper.ForcedFromSettingsFont = f;
				AppearanceObject.DefaultFont = f;
				AppearanceDefault.Control.Font = f;
				AppearanceObject.DefaultMenuFont = f;
			}
			catch {
			}
		}
		public void UpdateDesignTimeLookAndFeel(object designerHost) {
			UpdateDesignTimeLookAndFeel(null, designerHost);
		}
		public void UpdateDesignTimeLookAndFeel(IComponent component, object designerHost) {
			IDesignerHost host = designerHost as IDesignerHost;
			if(component == null && host != null) component = host.RootComponent;
			if(this.userLookAndFeelModified || host == null) return;
			this.userLookAndFeelModified = true;
			if(ReadApplicationSettings(component)) return;
			SetDefaultDesignTimeLookAndFeel(DefaultDesignTimeLookAndFeel, DesignTimeUserSkin);
		}
		protected void SetDefaultDesignTimeLookAndFeel(string style, bool registerUserSkins) {
			string[] split = style.Split(new char[] { '/' });
			string skinName = SkinName;
			ActiveLookAndFeelStyle lf = ActiveLookAndFeelStyle.Skin;
			try {
				lf = (ActiveLookAndFeelStyle)Enum.Parse(typeof(ActiveLookAndFeelStyle), split[0]);
			}
			catch {
			}
			if(lf == ActiveLookAndFeelStyle.Skin && split.Length > 1) skinName = split[1];
			if(lf == ActiveLookAndFeelStyle.WindowsXP) {
				if(!DevExpress.Utils.WXPaint.WXPPainter.Default.ThemesEnabled) {
					if(split.Length > 0) {
						try {
							lf = (ActiveLookAndFeelStyle)Enum.Parse(typeof(ActiveLookAndFeelStyle), split[1]);
						}
						catch {
						}
						if(lf == ActiveLookAndFeelStyle.WindowsXP) lf = ActiveLookAndFeelStyle.Skin;
					}
				}
			}
			if(ActiveStyle == lf && (lf != ActiveLookAndFeelStyle.Skin || skinName == SkinName)) return;
			if(lf == ActiveLookAndFeelStyle.WindowsXP)
				SetWindowsXPStyle();
			else {
				if(registerUserSkins) SkinManager.Default.RegisterDTUserSkins();
				SetStyle((LookAndFeelStyle)lf, false, true, skinName);
			}
		}
	}
	public class SkinNameTypeConverter : TypeConverter {
		IDesignerHost GetHost(ITypeDescriptorContext context) {
			IDesignerHost host = null;
			IComponent component = null;
			if(context == null) {
				component = UserLookAndFeel.Default.OwnerControl as IComponent;
			}
			else {
				if(context.Container != null) {
					host = context.Container as IDesignerHost;
					if(host != null)
						return host;
					component = context.Container as IComponent;
				}
				else {
					return context.GetService(typeof(IDesignerHost)) as IDesignerHost;
				}
			}
			if(component != null && component.Site != null) {
				host = component.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
			}
			return host;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) { return true; }
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) { return true; }
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			SkinManager.Default.RegisterDTUserSkins(GetHost(context));
			ArrayList list = new ArrayList();
			foreach(SkinContainer skin in SkinManager.Default.GetRuntimeSkins()) {
				list.Add(skin.SkinName);
			}
			return new StandardValuesCollection(list);
		}
	}
}
