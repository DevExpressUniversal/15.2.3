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
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Xml;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using DevExpress.Skins.Info;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Win.Hook;
using DevExpress.Utils.Drawing.Helpers;
using System.Collections.Generic;
using DevExpress.Data.Utils;
using DevExpress.Utils.Colors;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing.Animation;
using System.Security;
using DevExpress.XtraEditors;
using DevExpress.Utils.Svg;
namespace DevExpress.Skins {
	public static class SkinLoader {
		readonly static string SkinAssemblyFormat = "DevExpress.{0}" + AssemblyInfo.VSuffix;
		readonly static string UserSkinsNamespace = "DevExpress.UserSkins.";
		readonly static string BonusSkins = "BonusSkins";
		readonly static string OfficeSkins = "OfficeSkins";
		public enum DevExpressSkins { Bonus, Office };
		public static bool RegisterSkins(DevExpressSkins skins) {
			string skinsName = (skins == DevExpressSkins.Bonus) ? BonusSkins : OfficeSkins;
			string asmName = String.Format(SkinAssemblyFormat, skinsName);
			Assembly skinAssembly = GetAssembly(asmName);
			if(skinAssembly == null) {
				skinAssembly = AppDomain.CurrentDomain.Load(new AssemblyName(asmName));
			}
			return InvokeRegister(skinAssembly, UserSkinsNamespace + skinsName);
		}
		static bool InvokeRegister(Assembly skinAssembly, string typeName) {
			Type skinsType = skinAssembly.GetType(typeName);
			if(skinsType != null) {
				MethodInfo mInfo = skinsType.GetMethod("Register");
				if(mInfo != null) {
					mInfo.Invoke(null, new object[] { });
					return true;
				}
			}
			return false;
		}
		static Assembly GetAssembly(string asmName) {
			return Array.Find(
				AppDomain.CurrentDomain.GetAssemblies(),
				delegate(Assembly assembly) {
					return assembly.GetName().Name == asmName;
				}
			);
		}
	}
	public interface ISkinProvider {
		string SkinName { get; }
	}
	public interface ISkinProviderEx : ISkinProvider {
		bool GetTouchUI();
		float GetTouchScaleFactor();
		Color GetMaskColor();
		Color GetMaskColor2();
	}
	public enum SkinProductId { NavBar, Common, Editors, Tab, Grid, Bars, MetroUI, Docking, VGrid, Scheduler, NavPane, Form, Reports, Ribbon, Printing, 
		RichEdit, Chart, Gauges, Dashboard, Sparkline, Spreadsheet, PdfViewer, Map, NavigationPane, AccordionControl } 
	public class SkinContainer {
		bool loaded;
		Hashtable products;
		string skinName;
		SkinCreator creator;
		internal bool embedded;
		public SkinContainer(string skinName) {
			this.skinName = skinName;
			this.loaded = true;
		}
		public SkinContainer(SkinCreator creator) {
			this.creator = creator;
			this.skinName = creator.SkinName;
		}
		public SkinContainer Clone(string skinName) {
			return Clone(skinName, true);
		}
		public SkinContainer Clone(string skinName, bool register) {
			SkinContainer container = null;
			if(!register || Creator == null) {
				if(register) SkinManager.Default.Skins.Remove(skinName);
				container = new SkinContainer(skinName);
				foreach(SkinProductId product in Enum.GetValues(typeof(SkinProductId))) {
					Skin skin = GetSkin(product);
					if(skin != null) {
						SkinBuilder builder = new SkinBuilder(skinName);
						builder.skin = skin.Clone(skinName) as Skin;
						container.RegisterSkin(product, builder);
						if(register) SkinManager.Default.Skins.Add(container);
					}
				}
				return container;
			}
			container = new SkinContainer(Creator.Clone(skinName));
			if(SkinManager.Default.Skins[skinName] == null)
				SkinManager.Default.Skins.Add(container);
			container.Load();
			return container;
		}
		public bool IsEmbedded { get { return embedded; } }
		public IDictionaryEnumerator GetDictionaryEnumerator() { 
			foreach(DictionaryEntry entry in Products) {
				GetSkin(entry.Key);
			}
			return Products.GetEnumerator();
		}
		public Skin[] GetSkins() {
			ArrayList list = new ArrayList();
			foreach(SkinProductId product in Enum.GetValues(typeof(SkinProductId))) {
				Skin skin = GetSkin(product);
				if(skin != null) list.Add(skin);
			}
			return list.ToArray(typeof(Skin)) as Skin[];
		}
		protected internal SkinCreator Creator { get { return creator; } }
		public bool Loaded { get { return loaded; } }
		protected void SetLoaded() { this.loaded = true; } 
		public string SkinName { 
			get { return skinName; }
		}
		protected internal void SetSkinName(string name) {
			this.skinName = name;
			if(Loaded) {
				foreach(Skin skin in GetSkins()) skin.SetSkinName(name);
			}
		}
		class ProductsKeyComparer : IEqualityComparer {
			bool IEqualityComparer.Equals(object x, object y) {
				return x.Equals(y);
			}
			int IEqualityComparer.GetHashCode(object obj) {
				if(obj is SkinProductId) {
					return (int)(SkinProductId)obj;
				} else
					return obj.GetHashCode();
			}
		}
		protected internal Hashtable Products {
			get {
				if(products == null) products = new Hashtable(new ProductsKeyComparer());
				return products;
			}
		}
		public Skin GetSkin(object productId) {
			Load();
			if(this.products == null) return null;
			return Products[productId] as Skin;
		}
		public Skin CommonSkin {
			get {
				return GetSkin(SkinProductId.Common);
			}
		}
		public Skin RegisterSkin(object productId, SkinBuilder builder) {
			if(productId == null || builder == null || builder.Skin.Name != SkinName) return null;
			Load();
			Products[productId] = builder.Skin;
			builder.Skin.SetContainer(this);
			return builder.Skin;
		}
		internal void SetCreator(SkinCreator creator) {
			this.loaded = false;
			this.creator = creator;
		}
		public virtual void Load() {
			if(Loaded) return;
			SetLoaded();
			if(Creator == null) return;
			Creator.Load();
		}
		public string Template { get; set; }
	}
	public class SkinContainerCollection : CollectionBase {
		Hashtable nameHash;
		protected Hashtable NameHash { 
			get {
				if(nameHash == null) nameHash = new Hashtable();
				return nameHash;
			}
		}
		protected internal void Add(SkinContainer skinContainer) {
			if(this[skinContainer.SkinName] != null) return;
			List.Add(skinContainer);
		}
		protected internal void Remove(string skinName) {
			SkinContainer container = this[skinName];
			if(container != null) List.Remove(container);
		}
		public void Remove(SkinContainer container) {
			if(container == null || container.IsEmbedded) return;
			List.Remove(container);
		}
		public SkinContainer this[int index] { get { return (SkinContainer)List[index]; } }
		public SkinContainer this[string name] { get { return Count == 0 ? null : (SkinContainer)NameHash[name]; } }
		protected override void OnInsertComplete(int position, object item) {
			SkinContainer sc = (SkinContainer)item;
			NameHash[sc.SkinName] = sc;
		}
		protected override void OnRemoveComplete(int position, object item) {
			SkinContainer sc = (SkinContainer)item;
			NameHash[sc.SkinName] = null;
		}
		protected override void OnClear() {
			NameHash.Clear();
			InnerList.Clear();
		}
	}
	public class SkinRegistrator {
		public static string DesignTimeSkinName { get { return DevExpress.Utils.Design.WindowsFormsDesignTimeSettings.DesignTimeSkinName; } }
		public const string DefaultSkinName = "DevExpress Style";
		public const string DefaultDemoSkin = "DevExpress Style";
		public static void Register() {
			SkinManager.Default.RegisterEmbeddedSkin(new SkinBlobXmlCreator("DevExpress Style", "DevExpress.Utils.SkinData."));
			SkinManager.Default.RegisterEmbeddedSkin(new SkinBlobXmlCreator("DevExpress Dark Style", "DevExpress.Utils.SkinData."));
			SkinManager.Default.RegisterEmbeddedSkin(new SkinBlobXmlCreator("VS2010", "DevExpress.Utils.SkinData."));
			SkinManager.Default.RegisterEmbeddedSkin(new SkinBlobXmlCreator("Seven Classic", "DevExpress.Utils.SkinData."));
			SkinManager.Default.RegisterEmbeddedSkin(new SkinBlobXmlCreator("Office 2010 Blue", "DevExpress.Utils.SkinData."));
			SkinManager.Default.RegisterEmbeddedSkin(new SkinBlobXmlCreator("Office 2010 Black", "DevExpress.Utils.SkinData."));
			SkinManager.Default.RegisterEmbeddedSkin(new SkinBlobXmlCreator("Office 2010 Silver", "DevExpress.Utils.SkinData."));
			SkinManager.Default.RegisterEmbeddedSkin(new SkinBlobXmlCreator("Office 2013", "DevExpress.Utils.SkinData."));
			SkinManager.Default.RegisterEmbeddedSkin(new SkinBlobXmlCreator("Office 2013 Dark Gray", "DevExpress.Utils.SkinData."));
			SkinManager.Default.RegisterEmbeddedSkin(new SkinBlobXmlCreator("Office 2013 Light Gray", "DevExpress.Utils.SkinData."));
			SkinManager.Default.RegisterEmbeddedSkin(new SkinBlobXmlCreator("Visual Studio 2013 Blue", "DevExpress.Utils.SkinData."));
			SkinManager.Default.RegisterEmbeddedSkin(new SkinBlobXmlCreator("Visual Studio 2013 Light", "DevExpress.Utils.SkinData."));
			SkinManager.Default.RegisterEmbeddedSkin(new SkinBlobXmlCreator("Visual Studio 2013 Dark", "DevExpress.Utils.SkinData."));
			SkinManager.Default.RegisterEmbeddedSkin(new SkinBlobXmlCreator("Office 2016 Colorful", "DevExpress.Utils.SkinData."));
			SkinManager.Default.RegisterEmbeddedSkin(new SkinBlobXmlCreator("Office 2016 Dark", "DevExpress.Utils.SkinData."));
		}
	}
	public class SkinManagerHelper {
		public static void RenameContainer(string oldName, string newName) {
			SkinContainer container = SkinManager.Default.Skins[oldName];
			if(container == null || container.IsEmbedded) return;
			SkinManager.Default.Skins.Remove(container);
			container.SetSkinName(newName);
			SkinManager.Default.Skins.Add(container);
		}
	}
	internal class SkinManagerHookController : IHookController {
		public SkinManagerHookController() {
		}
		#region IHookController Members
		IntPtr IHookController.OwnerHandle {
			get { return IntPtr.Zero; }
		}
		bool IHookController.InternalPreFilterMessage(int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam) {
			if(Msg == MSG.WM_PAINT) {
				Control = wnd;
			}
			return false;
		}
		bool IHookController.InternalPostFilterMessage(int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam) {
			return true;
		}
		#endregion
		public Control Control { get; private set; }
	}
	public class SkinManager {
		public static readonly string DefaultSkinName = SkinRegistrator.DefaultSkinName;
		static bool allowFormSkins, allowArrowDragIndicators = true, allowWindowGhosting = false;
#if !SL
	[DevExpressUtilsLocalizedDescription("SkinManagerAllowFormSkins")]
#endif
		public static bool AllowFormSkins { get { return allowFormSkins; } }
		public static bool AllowWindowGhosting { get { return allowWindowGhosting; } set { allowWindowGhosting = value; } }
		public static void EnableFormSkinsIfNotVista() {
			if(DevExpress.Utils.Drawing.Helpers.NativeVista.IsVista) return;
			EnableFormSkins();
		}
		public static void SetDPIAware() {
			NativeVista.SetProcessDPIAware();
		}
		public static void EnableFormSkins() {
			allowFormSkins = true;
			if(!AllowWindowGhosting) DevExpress.Utils.Drawing.Helpers.NativeVista.DisableWindowGhosting();
		}
		public static void DisableFormSkins() {
			allowFormSkins = false;
		}
		static bool allowMdiFormSkins;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static bool AllowMdiFormSkins { get { return allowMdiFormSkins; } }
		public static void EnableMdiFormSkins() {
			allowMdiFormSkins = true;
		}
		public static bool AllowArrowDragIndicators {
			get { return allowArrowDragIndicators; }
			set { allowArrowDragIndicators = value; }
		}
		public static void DisableMdiFormSkins() {
			allowMdiFormSkins = false;
		}
		public static Point InvalidPoint = new Point(-10000, -10000);
		static Point hitPoint = InvalidPoint;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static Point HitPoint {
			get { return hitPoint; }
			set {
				hitPoint = value;
				OnHitPointChanged();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static List<SkinElement> HitElements { get; set; }
		private static void OnHitPointChanged() {
			if(HitElements == null)
				HitElements = new List<SkinElement>();
			HitElements.Clear();
		}
		static SkinManagerHookController Hook { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void EnableSkinHitTesting() {
			if(Hook == null) {
				Hook = new SkinManagerHookController();
			}
			HookManager.DefaultManager.AddController(Hook);
			IsSkinHitTestingEnabled = true;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void DisableSkinHitTesting() {
			HookManager.DefaultManager.RemoveController(Hook);
			IsSkinHitTestingEnabled = false;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static bool IsSkinHitTestingEnabled { get; private set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static Control CurrentPaintControl { get { return Hook.Control; } }
		internal static void SetDefaultSkinManager(SkinManager manager) { defaultManager = manager; }
		[ThreadStatic]
		static SkinManager defaultManager;
#if !SL
	[DevExpressUtilsLocalizedDescription("SkinManagerDefault")]
#endif
		public static SkinManager Default {
			get {
				if(defaultManager == null) {
					defaultManager = new SkinManager();
					SkinRegistrator.Register();
				}
				return defaultManager;
			}
		}
		SkinContainerCollection skins;
		public SkinManager() { 	
		}
		public bool RegisterAssembly(Assembly asm) {
			if(!IsSkinAssembly(asm)) return false;
			bool res = false;
			if(asm.GetName().Name == "DevExpress.BonusSkins" + AssemblyInfo.VSuffix) {
				Type type = asm.GetType("DevExpress.UserSkins.BonusSkins");
				if(type != null) {
					MethodInfo mi = type.GetMethod("Register", BindingFlags.Public | BindingFlags.Static);
					if(mi != null) mi.Invoke(null, null);
					return true;
				}
			}
			else if(asm.GetName().Name == "DevExpress.TouchSkins" + AssemblyInfo.VSuffix) {
				Type type = asm.GetType("DevExpress.UserSkins.TouchSkins");
				if(type != null) {
					MethodInfo mi = type.GetMethod("Register", BindingFlags.Public | BindingFlags.Static);
					if(mi != null) mi.Invoke(null, null);
					return true;
				}
			}
			res |= RegisterBySkinInfo(asm, "SkinInfo", false);
			res |= RegisterBySkinInfo(asm, "SkinInfoBlob", true);
			return res;
		}
		internal bool RegisterBySkinInfo(Assembly asm, string resourceName, bool isBlob) {
			bool res = false;
			string resManager = ValidateName(asm, resourceName);
			ResourceManager rm = new ResourceManager(resManager, asm);
			if(rm == null) return false;
			for(int n = 1; ; n++) {
				object infoRes = null;
				try {
					infoRes = rm.GetObject(resourceName + n.ToString());
				if(infoRes == null) break;
				}
				catch {
					break;
				}
				string[] info = infoRes as string[];
				if(info == null) info = infoRes.ToString().Split(';');
				if(info == null) break;
				if(isBlob) {
					RegisterSkin(new SkinBlobXmlCreator(info[0], info[1], asm, null));
				}
				else {
					RegisterSkin(new SkinEmbeddedXmlCreator(info[0], info[1], asm));
				}
				res = true;
			}
			return res;
		}
		string ValidateName(Assembly asm, string resourceName) {
			try {
				string[] names = asm.GetManifestResourceNames();
				string value = names.FirstOrDefault(q=>q.EndsWith(resourceName + ".resources"));
				if(!string.IsNullOrEmpty(value)) return value.Replace(".resources", "");
			}
			catch {
			}
			return resourceName;
		}
		internal bool IsSkinAssembly(Assembly asm) {
			return Attribute.IsDefined(asm, typeof(BonusSkinsAssemblyAttribute)) || 
#if DEBUG
				Attribute.IsDefined(asm, typeof(SkinsAssemblyAttribute)) ||
#endif
				Attribute.IsDefined(asm, typeof(TouchSkinsAssemblyAttribute)) || Compare(asm.GetName().GetPublicKeyToken(), SkinAssemblyHelper.PublicKeyToken);
		}
		bool registrationProcessed;
		internal void RegisterDTUserSkins(IComponent comp) {
			if(comp == null || comp.Site == null || !comp.Site.DesignMode) return;
			RegisterDTUserSkins(comp.Site.GetService(typeof(IDesignerHost)) as IDesignerHost);
		}
		internal void RegisterDTUserSkins(IDesignerHost host) {
			if(host == null) return;
			if(host != null && !this.registrationProcessed) {
				try {
					ITypeResolutionService tr = (ITypeResolutionService)host.GetService(typeof(ITypeResolutionService));
					if(tr != null)
						tr.GetType(host.RootComponentClassName);
					else 
						host.GetType(host.RootComponentClassName);
				}
				catch {
				}
			}
			RegisterDTUserSkins();
		}
		bool Compare(byte[] b1, byte[] b2) {
			if(b1 == null || b2 == null) return false;
			if(b1.Length != b2.Length) return false;
			for(int n = 0; n < b1.Length; n++) {
				if(b1[n] != b2[n]) return false;
			}
			return true;
		}
		[Obsolete("You should use the RegisterAssembly method instead of that")]
		public bool RegisterSkinAssembly(Assembly asm) {
			object[] obj = asm.GetCustomAttributes(typeof(SkinAssemblyAttribute), true);
			if(obj == null || obj.Length == 0) return false;
			if(!IsSkinAssembly(asm)) return false;
			Type[] types = asm.GetTypes();
			foreach(Type type in types) {
				MethodInfo[] ms = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
				foreach(MethodInfo mi in ms) {
					mi.Invoke(null, null);
				}
			}
			return true;
		}
		public bool RegisterSkins(Assembly asm) {
			object[] obj = asm.GetCustomAttributes(typeof(SkinAssemblyAttribute), true);
			if(obj == null || obj.Length == 0) return false;
			if(!IsSkinAssembly(asm)) return false;
			Type[] types = asm.GetTypes();
			foreach(Type type in types) {
				MethodInfo[] ms = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
				foreach(MethodInfo mi in ms) {
					mi.Invoke(null, null);
				}
			}
			return true;
		}
		internal void RegisterDTUserSkins() {
			if(this.registrationProcessed) return;
			this.registrationProcessed = true;
			try {
				Type type = Type.GetType("DevExpress.UserSkins.BonusSkins");
				Type type2 = Type.GetType("DevExpress.UserSkins.TouchSkins");
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				if(assemblies == null || assemblies.Length == 0) return;
				foreach(Assembly asm in assemblies) {
					RegisterAssembly(asm);
					try {
						if(asm.Location.Contains("ProjectAssemblies")) {
							AssemblyName[] asmName = asm.GetReferencedAssemblies();
							foreach (AssemblyName refAsm in asmName) {
								RegisterAssembly(Assembly.Load(refAsm));
							}
						}
					}
					catch { }
				}
			} catch {
			}
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("SkinManagerSkins")]
#endif
		public SkinContainerCollection Skins {
			get {
				if(skins == null) skins = new SkinContainerCollection();
				return skins;
			}
		}
		public IEnumerable<SkinContainer> GetRuntimeSkins() {
			foreach(SkinContainer skin in Skins) {
				if(!string.Equals(skin.SkinName, WindowsFormsDesignTimeSettings.DesignTimeSkinName, StringComparison.Ordinal))
					yield return skin;
			}
		}
		public string GetValidSkinName(string skinName) {
			if(skinName == null || Skins[skinName] == null) return DefaultSkinName;
			return skinName;
		}
		static object[] skinProductIds = new object[] { SkinProductId.NavBar, SkinProductId.Common, SkinProductId.Editors,
			SkinProductId.Tab, SkinProductId.Grid, SkinProductId.Bars, SkinProductId.MetroUI, SkinProductId.Docking,
			SkinProductId.VGrid, SkinProductId.Scheduler, SkinProductId.NavPane, SkinProductId.Form, SkinProductId.Reports,
			SkinProductId.Ribbon, SkinProductId.Printing, SkinProductId.RichEdit, SkinProductId.Chart, SkinProductId.Gauges,
			SkinProductId.Dashboard, SkinProductId.Sparkline, SkinProductId.Spreadsheet, SkinProductId.PdfViewer, SkinProductId.Map,
			SkinProductId.NavigationPane, SkinProductId.AccordionControl };
		public Skin GetSkin(SkinProductId productId, ISkinProvider provider) {
			return GetSkin(skinProductIds[(int)productId], provider);
		}
		public Skin GetSkin(object productId, ISkinProvider provider) {
			if(provider == null) provider = UserLookAndFeel.Default;
			Skin skin = GetSkin(productId, provider.SkinName);
			ISkinProviderEx providerEx = provider as ISkinProviderEx;
			if(providerEx != null && skin != null) {
				skin.TouchScaleFactor = providerEx.GetTouchUI()? providerEx.GetTouchScaleFactor() : 1.0f;
				Color mc = providerEx.GetMaskColor();
				Color mc2 = providerEx.GetMaskColor2();
				skin.MaskColor = mc == Color.Empty ? skin.BaseColor : mc;
				skin.MaskColor2 = mc2 == Color.Empty ? skin.BaseColor2 : mc2;
			}
			return skin;
		}
		public Skin GetSkin(object productId) { return GetSkin(productId, UserLookAndFeel.Default); }
		public Skin GetSkin(object productId, string skinName) {
			SkinContainer container = Skins[skinName];
			if(container == null) container = Skins[DefaultSkinName];
			if(container.GetSkin(productId) == null) {
				container = string.IsNullOrEmpty(container.Template)? Skins[DefaultSkinName]: Skins[container.Template];
				if(container == null)
					container = Skins[DefaultSkinName];
			}
			if(container != null) return container.GetSkin(productId);
			return null;
		}
		internal void RegisterEmbeddedSkin(SkinCreator creator) {
			RegisterSkinCore(creator, true);
		}
		public Skin RegisterSkin(object productId, SkinBuilder skinBuilder) {
			return RegisterSkin(productId, skinBuilder, DefaultSkinName);
		}
		public Skin RegisterSkin(object productId, SkinBuilder skinBuilder, string template) {
			if(productId == null || skinBuilder == null) return null;
			SkinContainer container = Skins[skinBuilder.Skin.Name];
			if(container == null) {
				container = new SkinContainer(skinBuilder.Skin.Name);
				Skins.Add(container);
			}
			container.Template = template;
			return container.RegisterSkin(productId, skinBuilder);
		}
		public void RegisterSkin(SkinCreator creator) {
			RegisterSkinCore(creator, false);
		}
		[System.Security.SecuritySafeCritical]
		protected void RegisterSkinCore(SkinCreator creator, bool embedded) {
			if(creator == null) return;
			SkinContainer container = Skins[creator.SkinName];
			if(container == null) {
				container = new SkinContainer(creator);
				Skins.Add(container);
			} else 
				container.SetCreator(creator);
			container.embedded = embedded;
		}
		public static SkinElement GetSkinElement(SkinProductId productId, ISkinProvider provider, string elementName) {
			return Default.GetSkin(productId, provider)[elementName];
		}
	}
	public abstract class SkinCreator {
		string skinName;
		bool loaded;
		public SkinCreator(string skinName) {
			this.skinName = skinName;
		}
		protected void SetLoaded() { this.loaded = true; }
		public abstract void Load();
		public abstract SkinCreator Clone(string skinName); 
		public bool Loaded { get { return loaded; } }
		public string SkinName { get { return skinName; } set { skinName = value; } }
	}
	[AttributeUsage(AttributeTargets.Assembly)]
	public class SkinAssemblyAttribute : Attribute {
		public SkinAssemblyAttribute() { }
	}
	[AttributeUsage(AttributeTargets.Assembly)]
	public class BonusSkinsAssemblyAttribute : Attribute {
		public BonusSkinsAssemblyAttribute() { }
	}
	[AttributeUsage(AttributeTargets.Assembly)]
	public class SkinsAssemblyAttribute : Attribute {
		public SkinsAssemblyAttribute() { }
	}
	[AttributeUsage(AttributeTargets.Assembly)]
	public class TouchSkinsAssemblyAttribute : Attribute {
		public TouchSkinsAssemblyAttribute() { }
	}
	public enum SkinImageStretch { Tile, NoResize, Stretch };
	public enum SkinImageLayout { Horizontal, Vertical }
	public enum SkinOffsetKind { Default, Center, Near, Far }
	public class SkinOffsetTouch : SkinOffset {
		public SkinOffsetTouch(SkinOffset offset) {
			OwnerOffset = offset;
			ScaleFactor = 1.0f;
		}
		protected SkinOffset OwnerOffset { get; set; }
		public override Point Offset {
			get { return new Point((int)(OwnerOffset.Offset.X * ScaleFactor), (int)(OwnerOffset.Offset.Y * ScaleFactor)); }
			set { base.Offset = value; }
		}
		public float ScaleFactor { get; set; }
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class SkinOffset : ICloneable {
		Point offset = Point.Empty;
		SkinOffsetKind kind = SkinOffsetKind.Default;
		public SkinOffset(SkinOffsetKind kind, int x, int y) {
			this.kind = kind;
			this.offset = new Point(x, y);
		}
		public SkinOffset() { }
		public virtual object Clone() {
			return MemberwiseClone();
		}
		[Browsable(false)]
		public bool IsCenter { get { return Kind == SkinOffsetKind.Center; } }
		[RefreshProperties(RefreshProperties.All)]
		public SkinOffsetKind Kind { get { return kind; } set { kind = value; } }
		[RefreshProperties(RefreshProperties.All)]
		public virtual Point Offset { get { return offset; } set { offset = value; } }
		public Rectangle GetBounds(Rectangle dest, Size size, SkinOffsetKind defaultOffset) {
			return RectangleHelper.GetBounds(dest, size, this, defaultOffset);
		}
		[Browsable(false)]
		public bool IsEmpty { get { return Kind == SkinOffsetKind.Default && Offset.IsEmpty; } }
		public override string ToString() {
			if(IsEmpty) return string.Empty;
			return string.Format("({0}, {1}", Kind, Offset);
		}
	}
	public class SkinGlyph : SkinImage {
		public SkinGlyph(Image image, string imageName) : base(image, imageName) { }
		public SkinGlyph(string imageName) : base(imageName) { }
		public SkinGlyph(string imageName, SkinPaddingEdges sizingMargins) : base(imageName, sizingMargins) { }
		public SkinGlyph(string imageName, SkinPaddingEdges sizingMargins, Color transparentColor) : base(imageName, sizingMargins, transparentColor) { }
		public SkinGlyph(string imageName, Color transparentColor) : base(imageName, null, transparentColor) { }
		public SkinGlyph(Image image, SkinPaddingEdges sizingMargins, Color transparentColor) : base(image, sizingMargins, transparentColor) { }
		public SkinGlyph(Image image, SkinPaddingEdges sizingMargins) : base(image, sizingMargins) { }
		public SkinGlyph(Image image) : this(image, new SkinPaddingEdges(0)) { }
		public SkinGlyph(Image image, Color transparentColor) : this(image, new SkinPaddingEdges(0), transparentColor) { }
		public override SkinImageStretch Stretch { get { return SkinImageStretch.NoResize; } set { } }
		public override SkinImage New(Image image) { return new SkinGlyph(image); }
	}
	public interface ISkinImageProvider {
		Image GetImage();
		bool CheckReload();
		void UpdateProviderInfo(string newPath);
		void Clear();
		ISkinImageProvider Copy();
		bool Equals(ISkinImageProvider provider);
	}
	public interface ISkinImageLoader {
		Image Load(SkinImage image, string imageName);
	}
	public enum FlipType { Default, HorizontalFlip, VerticalFlip}
	public enum SkinImageColorizationMode { Default, None, Color1, Color2 }
	public class SkinImage : ICloneable {
		Image image;
		Stream rawData;
		SkinImageStretch stretch = SkinImageStretch.Stretch;
		SkinImageLayout layout = SkinImageLayout.Horizontal;
		SkinPaddingEdges sizingMargins;
		ImageCollection images;
		int imageCount = 1;
		SkinImageColorizationMode[] imageColorizationModes;
		string imageName = "";
		Color transparentColor;
		internal ISkinImageLoader imageLoader;
		ISkinImageProvider imageProvider;
		bool useOwnImage = false;
		public SkinImage(Image image, string imageName) : this((Image)null) { 
			this.imageName = imageName;
			this.image = image;
		}
		public SkinImage(string imageName) : this(imageName, null) { }
		public SkinImage(string imageName, SkinPaddingEdges sizingMargins) : this(imageName, sizingMargins, Color.Empty) { }
		public SkinImage(string imageName, SkinPaddingEdges sizingMargins, Color transparentColor) {
			SetImage(imageName, transparentColor);
			this.sizingMargins = sizingMargins == null ? new SkinPaddingEdges() : sizingMargins;
		}
		public SkinImage(Image image, SkinPaddingEdges sizingMargins) : this(image, sizingMargins, Color.Empty) { }
		public SkinImage(Image image, SkinPaddingEdges sizingMargins, Color transparentColor) {
			SetImage(image, transparentColor);
			this.sizingMargins = sizingMargins == null ? new SkinPaddingEdges() : sizingMargins;
		}
		public SkinImageColorizationMode[] ImageColorizationModes { get { return imageColorizationModes; } }
		public void SetImageProvider(ISkinImageProvider provider) {
			if (this.imageProvider != provider && this.imageProvider != null && !this.imageProvider.Equals(provider)) {
				this.imageProvider.Clear();
			} 
			this.imageProvider = provider;
		}
		internal void SetRawData(Stream data) {
			this.rawData = data;
		}
		protected internal Skin Owner { get { return OwnerInfo == null? null: OwnerInfo.Owner; } }
		public bool UseSmartColorization { get; set; }
		protected bool CanUseSmartColorization { get { return Skin.UseSmartColorizationForImages || UseSmartColorization; } }
		[Browsable(false)]
		public ISkinImageProvider ImageProvider { get { return imageProvider; } }
		public SkinImage(Image image) : this(image, new SkinPaddingEdges(0)) { }
		public SkinPaddingEdges SizingMargins { get { return sizingMargins; } set { sizingMargins = value; } }
		public int ImageCount { 
			get { return imageCount; } 
			set { 
				imageCount = value;
				this.imageColorizationModes = new SkinImageColorizationMode[ImageCount];
			} 
		}
		public SkinImageLayout Layout { get { return layout; } set { layout = value; } }
		public virtual SkinImageStretch Stretch { get { return stretch; } set { stretch = value; } }
		[Browsable(false)]
		public bool UseOwnImage { get { return useOwnImage; } set { useOwnImage = value; } }
		public Image GetImage(bool rightToLeft) {
			return GetImage(rightToLeft, FlipType.Default);
		}
		public Image GetImage(bool rightToLeft, FlipType rtlFlipType) {
			if(rightToLeft) return GetRTLImage(rtlFlipType);
			return Image;
		}
		public SkinPaddingEdges GetSizingMargins(bool rightToLeft) {
			if(!rightToLeft) return SizingMargins;
			var res = SizingMargins;
			res = new SkinPaddingEdges(res.Right, res.Top, res.Left, res.Bottom);
			return res;
		}
		public SkinImageState SaveImage() { 
			SkinImageState state = new SkinImageState();
			state.Image = Image;
			state.ColoredImages = ColoredImages;
			coloredImages = null;
			return state;
		}
		public void RestoreImage(SkinImageState state) {
			SetImage(state.Image, Color.Empty);
			coloredImages = state.ColoredImages;
		}
		[Browsable(false)]
		public Image Image { 
			get {
				Image img = ImageCore;
				if(IsColorized)
					return GetColoredImage(img);
				return img; 
			}
			set {
				if(image == value)
					return;
				image = value;
				this.images = null;
				ClearColoredImages();
				if(RTLImage != null) {
					RTLImage.Dispose();
					RTLImage = null;
			}
		}
		}
		internal Image RTLImage {
			get;
			set;
		}
		Image imageProviderImage;
		[Browsable(false)]
		public Image ImageCore {
			get {
				if(!UseOwnImage && ImageProvider != null) {
					Image res = ImageProvider.GetImage();
					if (res != imageProviderImage)
						this.images = null;
					imageProviderImage = res;
					return imageProviderImage; 
				}
				if(rawData != null) {
					SetImage(CreateImageFromStream(rawData), TransparentColor);
					rawData = null;
				}
				return image;  
			}
		}
		public Image GetRTLImage() {
			return GetRTLImage(FlipType.Default);
		}
		public Image GetRTLImage(FlipType type) {
			if(RTLImage != null) return RTLImage;
			RTLImage = GetRTLImage(ImageCore, type);
			return RTLImage;
		}
		Image GetRTLImage(Image image, FlipType type) {
			if(image == null) return null;
			Image i = image.Clone() as Image;
			if(type == FlipType.Default || type == FlipType.HorizontalFlip)
				i.RotateFlip(RotateFlipType.RotateNoneFlipX);
			else
				i.RotateFlip(RotateFlipType.RotateNoneFlipY);
			return i;
		}
		Color PrevMaskColor { get; set; }
		Color PrevMaskColor2 { get; set; }
		private System.Drawing.Image GetColoredImage(Image image) {
			if(image == null)
				return null;
			if(ColoredImage != null)
				return ColoredImage;
			PrevMaskColor = MaskColor;
			PrevMaskColor2 = MaskColor2;
			if(XtraAnimator.GetImageFrameCount(image) > 1) {
				ColoredImage = (Image)image.Clone();
				return ColoredImage;
			}
			List<Image> coloredImages = GetColoredImages(image);
			ColoredImage = new Bitmap(image.Width, image.Height);
			((Bitmap)ColoredImage).SetResolution(image.HorizontalResolution, image.VerticalResolution);
			using(Graphics g = Graphics.FromImage(ColoredImage)) {
				for(int i = 0; i < coloredImages.Count; i++) {
					Point pt = Layout == SkinImageLayout.Vertical ? new Point(0, i * coloredImages[i].Height) : new Point(i * coloredImages[i].Width, 0);
					g.DrawImage(coloredImages[i], pt);
				}
			}
			return ColoredImage; 
		}
		protected List<Image> GetColoredImages(Image image) {
			ImageCollection images = GetImagesCore(image);
			List<Image> coloredImages = new List<Image>();
			for(int i = 0; i < images.Images.Count; i++) {
				SkinColorizationMode mode = Owner.CommonSkin.ColorizationMode;
				if(mode == SkinColorizationMode.ColorAndHue)
					mode = GetActualColorizationMode(i) == SkinImageColorizationMode.Color1 ? SkinColorizationMode.Color2 : SkinColorizationMode.Hue2;
				Color actualMaskColor = GetActualMaskColor(i);
				if(UseSmartColorization)
					coloredImages.Add(SkinImageColorizer.GetSmartColorizedImage(images.Images[i], actualMaskColor));
				else
					coloredImages.Add(SkinImageColorizer.GetColoredImage(images.Images[i], actualMaskColor, mode));
			}
			return coloredImages;
		}
		protected SkinImageColorizationMode GetActualColorizationMode(int imageIndex) {
			if(
				ImageColorizationModes != null && ImageColorizationModes.Length > imageIndex 
				&& ImageColorizationModes[imageIndex] != SkinImageColorizationMode.Default)
				return ImageColorizationModes[imageIndex];
			bool useColor2 = OwnerInfo.UseColor2;
			bool useColor1 = OwnerInfo.UseColor;
			return OwnerInfo.UseColor2 ? SkinImageColorizationMode.Color2 : SkinImageColorizationMode.Color1;
		}
		protected virtual Color GetActualMaskColor(int imageIndex) {
			if(imageIndex >= ImageCount)
				return ActualMaskColor;
			if(ImageColorizationModes[imageIndex] == SkinImageColorizationMode.Default)
				return ActualMaskColor;
			if(ImageColorizationModes[imageIndex] == SkinImageColorizationMode.None)
				return Color.Empty;
			if(ImageColorizationModes[imageIndex] == SkinImageColorizationMode.Color1)
				return MaskColor;
			return MaskColor2;
		}
		protected internal bool suppressColorization;
		protected bool IsColorized { 
			get {
				if(Owner == null || OwnerInfo == null || suppressColorization)
					return false;
				if(OwnerInfo.Properties.ContainsProperty(Skin.OptUseColor) && !OwnerInfo.UseColor && !OwnerInfo.UseColor2)
					return false;
				return Owner.HasMaskColor || Owner.HasMaskColor2;
			} 
		}
		protected internal SkinBuilderElementInfo OwnerInfo { get; set; }
		public Color MaskColor {
			get { return Owner == null? Color.Empty: Owner.MaskColor; }
		}
		public Color MaskColor2 {
			get { return Owner == null ? Color.Empty : Owner.MaskColor2; }
		}
		public Color ActualMaskColor {
			get {
				if(OwnerInfo == null)
					return MaskColor;
				return OwnerInfo.UseColor2 ? MaskColor2 : MaskColor;
			}
		}
		public class ImageRef {
			public Image Image { get; set; }
			public long RefCount { get; set; }
		}
		Dictionary<Color, ImageRef> coloredImages;
		protected internal Dictionary<Color, ImageRef> ColoredImages {
			get {
				if(coloredImages == null)
					coloredImages = new Dictionary<Color, ImageRef>();
				return coloredImages;
			}
		}
		Color GetMinRefImageKey() {
			KeyValuePair<Color, ImageRef> min = new KeyValuePair<Color, ImageRef>(Color.Empty, new ImageRef() { Image = null, RefCount = long.MaxValue });
			foreach(KeyValuePair<Color, ImageRef> pair in ColoredImages) {
				if(min.Value.RefCount > pair.Value.RefCount) {
					min = pair;
				}
			}
			return min.Key;
		}
		protected internal Image ColoredImage {
			get {  
				ImageRef res = null;
				if(ColoredImages.TryGetValue(ActualMaskColor, out res)) {
					res.RefCount++;
					return res.Image;
				}
				return null;
			}
			set {
				if(!ColoredImages.ContainsKey(ActualMaskColor)) {
					if(ColoredImages.Count > 9) {
						ColoredImages.Remove(GetMinRefImageKey());
					}
					ColoredImages.Add(ActualMaskColor, new ImageRef() { Image = value });
				}
				else 
					ColoredImages[ActualMaskColor].Image = value; 
			}
		}
		protected virtual System.Drawing.Image GenerateImage(Image src, Color color) {
			return SkinImageColorizer.GetColoredImage(src, color, Owner.CommonSkin.ColorizationMode);
		}
		Image CreateImageFromStream(Stream ms) {
			Bitmap b = (Bitmap)ImageTool.ImageFromStream(ms);
			if(object.Equals(b.RawFormat, ImageFormat.Gif)) return b;
			Bitmap bmp = new Bitmap(b.Width, b.Height, (b.PixelFormat & PixelFormat.Alpha) != 0 ? PixelFormat.Format32bppArgb : PixelFormat.Format24bppRgb);
			bmp.SetResolution(b.HorizontalResolution, b.VerticalResolution);
			Graphics g = Graphics.FromImage(bmp);
			g.CompositingMode = CompositingMode.SourceCopy;
			g.DrawImageUnscaled(b, Point.Empty);
			b.Dispose();
			g.Dispose();
			return bmp;
		}
		[Browsable(false), DefaultValue("")]
		public string ImageName {
			get { return imageName; }
			set {
				if(value == null) value = "";
				if(ImageName == value) return;
				if(value == "") Image = null;
				else
					SetImage(GetImage(value), TransparentColor);
				this.imageName = value;
			}
		}
		public void SetImage(Image image, string imageName) {
			if(imageName == null) imageName = "";
			this.imageName = imageName;
			this.image = image;
			if(this.image != null) SetImage(this.image, TransparentColor);
		}
		bool ShouldSerializeTransparentColor() { return TransparentColor != Color.Empty; } 
		public Color TransparentColor { 
			get { return transparentColor; }
			set {
				if(TransparentColor == value) return;
				transparentColor = value;
				SetImage(Image, TransparentColor);
			}
		}
		public void SetImageNameCore(string imageName) {
			this.imageName = imageName;
		}
		public void SetImage(string imageName, Color transparentColor) {
			this.imageName = imageName;
			SetImage(GetImage(imageName), transparentColor);
		}
		protected Image GetImage(string imageName) {
			if(imageLoader == null) return Image.FromFile(imageName);
			return imageLoader.Load(this, imageName);
		}
		public void SetImage(Image image, Color transparentColor) {
			if(image != null)
				this.rawData = null;
			Image = image;
			if(transparentColor != Color.Empty) {
				Bitmap bmp = image as Bitmap;
				if(bmp != null) bmp.MakeTransparent(transparentColor);
			}
			this.transparentColor = transparentColor;
			this.imageColorizationModes = new SkinImageColorizationMode[ImageCount];
		}
		protected virtual ImageCollection GetImagesCore(Image img) {
			if(this.images != null || img == null) return this.images;
			this.images = new ImageCollection();
			int count = (ImageCount > 0 ? ImageCount : 1);
			if(Layout == SkinImageLayout.Horizontal) {
				this.images.ImageSize = new Size(img.Width / count, img.Height);
				this.images.AddImageStrip(ImageCore);
			}
			else {
				this.images.ImageSize = new Size(img.Width, img.Height / count);
				this.images.AddImageStripVertical(ImageCore);
			}
			return images;
		}
		public ImageCollection GetImages() {
			return GetImagesCore(Image);
		}
		internal Size GetImageSize() {
			if(Image == null || ImageCount == 0) return Size.Empty;
			Size size = Image.Size;
			if(ImageCount == 1) return size;
			if(Layout == SkinImageLayout.Horizontal) 
				size.Width /= ImageCount;
			else 
				size.Height /= ImageCount;
			return size;
		}
		public Rectangle GetImageBounds(int index) {
			if(Image == null || index < 0 || index >= ImageCount) return Rectangle.Empty;
			Size size = GetImageSize();
			Rectangle res = new Rectangle(Point.Empty, size);
			if(ImageCount == 1) return res;
			if(Layout == SkinImageLayout.Horizontal) 
				res.X = size.Width * index;
			else
				res.Y = size.Height * index;
			return res;
		}
		public virtual object Clone() { 
			return Copy(); 
		}
		public virtual SkinImage New(Image image) {
			return new SkinImage(image);
		}
		public SkinImage Copy() {
			SkinImage result = MemberwiseClone() as SkinImage;
			if(ImageProvider != null)
				result.SetImageProvider(ImageProvider.Copy());
			result.ColoredImage = null;
			return result;
		}
		public bool CheckReload() {
			if(ImageProvider != null) return ImageProvider.CheckReload();
			if(Image == null || ImageName == "" || imageLoader == null) return false;
			Image img = null;
			try {
				img = imageLoader.Load(this, ImageName);
				SetImage(img, TransparentColor);
			} catch { }
			return img != null && rawData != null;
		}
		internal void ClearColoredImages() {
			foreach(ImageRef img in ColoredImages.Values) {
				if(img.Image != null)
					img.Image.Dispose();
			}
			ColoredImages.Clear();
		}
	}
	public class SkinColor : ICloneable {
		Color backColor, backColor2, foreColor, solidImageCenterColor, solidImageCenterColor2;
		bool fontBold;
		float fontSize;
		LinearGradientMode gradientMode;
		LinearGradientMode solidImageCenterGradientMode;
		Skin owner;
		public SkinColor() {
			this.solidImageCenterGradientMode = this.gradientMode = LinearGradientMode.Horizontal;
		}
		object ICloneable.Clone() { return MemberwiseClone(); }
		[Browsable(false)]
		public Skin Owner { get { return owner; } }
		protected internal void SetOwner(Skin owner) { this.owner = owner; }
		public Color GetBackColor() {
			return GetColor(BackColor);
		}
		public Color GetBackColor2() {
			return GetColor(BackColor2);
		}
		public Color GetForeColor() {
			return GetColor(ForeColor);
		}
		public Color GetSolidImageCenterColor() {
			return GetColor(SolidImageCenterColor);
		}
		public Color GetSolidImageCenterColor2() {
			return GetColor(SolidImageCenterColor2);
		}
		protected Color GetColor(Color clr) {
			if(clr == Color.Empty) return clr;
			if(Owner != null) {
				if(clr.IsSystemColor) 
					return Owner.GetSystemColor(clr);
				return Owner.ConvertColorByBaseColor(clr);
			}
			return clr;
		}
		[DefaultValue(false)]
		public bool FontBold { get { return fontBold; } set { fontBold = value; } }
		[DefaultValue(0f)]
		public float FontSize { get { return fontSize; } set { fontSize = value; } }
		[DefaultValue(LinearGradientMode.Horizontal)]
		public LinearGradientMode GradientMode { get { return gradientMode; } set { gradientMode = value; } }
		[DefaultValue(LinearGradientMode.Horizontal)]
		public LinearGradientMode SolidImageCenterGradientMode { get { return solidImageCenterGradientMode; } set { solidImageCenterGradientMode = value; } }
		bool ShouldSerializeBackColor() { return BackColor != Color.Empty; }
		public Color BackColor { get { return backColor; } set { backColor = value; } }
		bool ShouldSerializeBackColor2() { return BackColor2 != Color.Empty; }
		public Color BackColor2 { get { return backColor2; } set { backColor2 = value; } }
		bool ShouldSerializeForeColor() { return ForeColor != Color.Empty; }
		public Color ForeColor { get { return foreColor; } set { foreColor = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsEmpty {
			get {
				return BackColor.IsEmpty && ForeColor.IsEmpty && BackColor2.IsEmpty && SolidImageCenterColor.IsEmpty && SolidImageCenterColor2.IsEmpty && FontSize == 0;
			}
		}
		bool ShouldSerializeSolidImageCenterColor() { return SolidImageCenterColor != Color.Empty; }
		public Color SolidImageCenterColor {
			get { return solidImageCenterColor; }
			set { solidImageCenterColor = value; }
		}
		bool ShouldSerializeSolidImageCenterColor2() { return SolidImageCenterColor2 != Color.Empty; }
		public Color SolidImageCenterColor2 {
			get { return solidImageCenterColor2; }
			set { solidImageCenterColor2 = value; }
		}
	}
	public class SkinBorder : ICloneable {
		Color left, right, top, bottom;
		SkinPaddingEdges thin;
		Skin owner;
		public SkinBorder() {
			this.thin = new SkinPaddingEdges();
			left = right = top = bottom = Color.Empty;
		}
		public virtual object Clone() { return MemberwiseClone(); }
		[Browsable(false)]
		public Skin Owner { get { return owner; } }
		protected internal void SetOwner(Skin owner) { this.owner = owner; }
		[Browsable(false)]
		public bool IsEmpty {
			get { 
				return 
				  All.IsEmpty && Top.IsEmpty && Bottom.IsEmpty &&
					Left.IsEmpty && Right.IsEmpty &&
					  Thin.Left == 0 && Thin.Top == 0 && Thin.Right == 0 && Thin.Bottom == 0; 
			}
		}
		public void SetBorderColor(Color color) {
			left = right = top = bottom = color;
		}
		public Color All { 
			get { 
				return Left == Right && Left == Top && Left == Bottom ? GetLeft() : Color.Empty; 
			}
			set {
				Left = Right = Top = Bottom = value;
			}
		}
		public SkinPaddingEdges Thin { get { return thin; } set { thin = value; } }
		public Color Left { get { return left; } set { left = value; } }
		public Color Right { get { return right; } set { right = value; } }
		public Color Top { get { return top; } set { top = value; } }
		public Color Bottom { get { return bottom; } set { bottom = value; } }
		public Color GetLeft() { return GetColor(Left); }
		public Color GetRight() { return GetColor(Right); }
		public Color GetTop() { return GetColor(Top); }
		public Color GetBottom() { return GetColor(Bottom); }
		protected Color GetColor(Color clr) {
			if(clr == Color.Empty) return clr;
			if(Owner != null) {
				if(clr.IsSystemColor)
					return Owner.GetSystemColor(clr);
				return Owner.ConvertColorByBaseColor(clr);
			}
			return clr;
		}
		bool ShouldSerializeLeft() { return Left != Color.Empty; }
		bool ShouldSerializeRight() { return Right != Color.Empty; }
		bool ShouldSerializeTop() { return Top != Color.Empty; }
		bool ShouldSerializeBottom() { return Bottom != Color.Empty; }
	}
	public class SkinElement {
		SkinBuilderElementInfo info;
		string elementName;
		Skin owner;
		Dictionary<float, SkinBuilderElementInfo> scaledInfo;
		SvgPaletteDictionary svgPalettes;
		public SkinElement(Skin skin, string elementName, SkinBuilderElementInfo info) {
			this.scaledInfo = new Dictionary<float, SkinBuilderElementInfo>();
			this.svgPalettes = new SvgPaletteDictionary();
			this.owner = skin;
			this.elementName = elementName;
			this.info = info == null ? new SkinBuilderElementInfo() : info;
			this.info.OwnerElement = this;
			UpdateOwner();
		}
		void CopyScaledInfo(Dictionary<float, SkinBuilderElementInfo> scaledInfos) {
			foreach(SkinBuilderElementInfo ei in scaledInfos.Values) {
				SkinBuilderElementInfo newInfo = ei.Copy();
				newInfo.OriginalInfo = Info;
				ScaledInfo.Add(ei.ScaleFactor, newInfo);
			}
		}
		public SkinElement Copy(Skin newSkin) {
			SkinBuilderElementInfo info = Info.Copy();
			SkinElement element = new SkinElement(newSkin, elementName, info);
			element.CopyScaledInfo(ScaledInfo);
			element.SvgPalettes.Clone(SvgPalettes);
			element.IsCustomElement = IsCustomElement;
			return element;
		}
		public SkinElement Copy(Skin newSkin, string newName) {
			SkinBuilderElementInfo info = Info.Copy();
			SkinElement element = new SkinElement(newSkin, newName, info);
			element.CopyScaledInfo(ScaledInfo);
			element.SvgPalettes.Clone(SvgPalettes);
			element.IsCustomElement = IsCustomElement;
			return element;
		}
		bool? ForceAllowScaleImages { get; set; }
		protected bool AllowScaleImages {
			get {
				if(ForceAllowScaleImages.HasValue)
					return ForceAllowScaleImages.Value;
				return Info.Properties.GetBoolean(Skin.OptScaleImage);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Dictionary<float, SkinBuilderElementInfo> ScaledInfo {
			get { return scaledInfo; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public SvgPaletteDictionary SvgPalettes {
			get { return svgPalettes; }
		}
		bool isCustomElement;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsCustomElement { get { return isCustomElement; } set { isCustomElement = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public SkinElement Original { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void Assign(SkinElement sourceElement) {
			this.info = sourceElement.Info.Copy();
		}
		protected virtual void UpdateOwner() {
			Info.Owner = Owner;
		}
		SkinBuilderElementInfo actualInfo;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public SkinBuilderElementInfo ActualInfo {
			get {
				if(actualInfo == null || actualInfo.ScaleFactor != ActualScaleFactor)
					actualInfo = GetActualInfo();
				return actualInfo; 
			}
			set { actualInfo = value; }
		}
		protected virtual SkinBuilderElementInfo GetActualInfo() {
			if(ActualScaleFactor == 1.0f)
				return this.info;
			if(!ScaledInfo.ContainsKey(ActualScaleFactor)) {
				ScaledInfo.Add(ActualScaleFactor, GenerateInfo(ActualScaleFactor));
			}
			return ScaledInfo[ActualScaleFactor];
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void RefreshScaledInfo(float scaleFactor) {
			SkinBuilderElementInfo info = ScaledInfo.Values.FirstOrDefault((i) => i.ScaleFactor == scaleFactor);
			if(info == null)
				return;
			ScaledInfo.Remove(info.ScaleFactor);
			if((info.ImageCore != null && info.ImageCore.Image != null) || (info.GlyphCore != null && info.GlyphCore.Image != null)) {
				ForceAllowScaleImages = true;
				ScaledInfo.Add(scaleFactor, GenerateInfo(scaleFactor));
				ForceAllowScaleImages = null;
			}
		}
		protected internal virtual SkinBuilderElementInfo GenerateInfo(float scaleFactor) {
			SkinBuilderElementInfo optimal = null;
			foreach(SkinBuilderElementInfo info in ScaledInfo.Values) { 
				if(info.ScaleFactor > scaleFactor)
					continue;
				if (optimal == null || info.ScaleFactor > optimal.ScaleFactor)
					optimal = info;
			}
			if(optimal == null)
				optimal = Info;
			SkinBuilderElementInfo scaledInfo = optimal.GenerateInfo(scaleFactor, AllowScaleImages);
			scaledInfo.Owner = Owner;
			scaledInfo.OwnerElement = this;
			return scaledInfo;
		}
		protected virtual float DpiScaleFactor { get { return Owner == null? 1.0f: Owner.DpiScaleFactor; } }
		protected internal virtual float ActualScaleFactor {
			get { return DpiScaleFactor; }
		}
		[Browsable(false)]
		public Skin Owner { get { return owner; } }
		[Browsable(false)]
		public string ElementName { get { return elementName; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public SkinBuilderElementInfo Info { get { return info; } }
		public SkinOffset Offset { 
			get { 
				return ActualInfo.AllowTouch && Owner.TouchScaleFactor != 1.0f ? ActualInfo.OffsetTouch: ActualInfo.Offset; 
			} 
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public SkinOffset OffsetCore {
			get { return ActualInfo.Offset; }
		}
		[Browsable(false)]
		public bool HasImage {
			get { return ActualInfo.HasImage; }
		}
		[Browsable(false)]
		public Image GetActualImage() {
			return ActualInfo.GetActualImage();
		}
		[Browsable(false)]
		public void SetActualImage(Image image, bool useOwnImage) {
			ActualInfo.SetActualImage(image, useOwnImage);
		}
		[Browsable(false)]
		public SkinImage Image { get { return ActualInfo.Image; } }
		[Browsable(false)]
		public SkinGlyph Glyph { get { return ActualInfo.Glyph; } }
		[Browsable(false)]
		public SkinImage ImageCore { get { return ActualInfo.ImageCore; } }
		[Browsable(false)]
		public SkinGlyph GlyphCore { get { return ActualInfo.GlyphCore; } }
		[Browsable(false)]
		public SkinBorder Border { get { return ActualInfo.Border; } }
		[Browsable(false)]
		public SkinColor Color { get { return ActualInfo.Color; } }
		public SkinSize Size { 
			get { 
				return ActualInfo.AllowTouch && Owner.TouchScaleFactor != 1.0f? ActualInfo.SizeTouch: SizeCore; 
			} 
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public SkinSize SizeCore {
			get { return ActualInfo.Size; }
			set { ActualInfo.Size = value; }
		}
		[Browsable(false)]
		public SkinProperties Properties { get { return ActualInfo.Properties; } }
		public SkinPaddingEdges ContentMargins { 
			get {
				if(Owner.TouchScaleFactor == 1.0f)
					return ContentMarginsCore;
				return ActualInfo.AllowTouch && Owner.TouchScaleFactor != 1.0f? ActualInfo.TouchContentMargins: ContentMarginsCore; 
			} 
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public SkinPaddingEdges ContentMarginsCore {
			get { return ActualInfo.ContentMargins; }
			set { ActualInfo.ContentMargins = value; }
		}
		public AppearanceDefault GetAppearanceDefault() { return GetAppearanceDefault(null); }
		public AppearanceDefault GetAppearanceDefault(ISkinProvider provider) {
			AppearanceDefault appDefault = new AppearanceDefault();
			Apply(appDefault, provider);
			return appDefault;
		}
		[Browsable(false)]
		public bool IsRequireTransparentBackground {
			get {
				if(Image != null && Image.Image != null) return true;
				if(Color.GetBackColor2() != Color.GetBackColor() && !Color.GetBackColor2().IsEmpty) return true;
				return false;
			}
		}
		public bool HasForeColor(ObjectState state) {
			Color res = GetForeColor(state);
			return !res.IsEmpty;
		}
		public Color GetForeColor(ObjectState state) {
			if((state & ObjectState.Pressed) != 0) {
				Color pressed = Properties.GetColor("ForeColorPressed");
				if(!pressed.IsEmpty) return pressed;
			}
			if((state & ObjectState.Hot) != 0) {
				Color hot = Properties.GetColor("ForeColorHot");
				if(!hot.IsEmpty) return hot;
			}
			if((state & ObjectState.Disabled) != 0) {
				Color disabled = Properties.GetColor("ForeColorDisabled");
				if(!disabled.IsEmpty) return disabled;
			}
			if((state & ObjectState.Selected) != 0) {
				Color selected = Properties.GetColor("ForeColorSelected");
				if(!selected.IsEmpty) return selected;
			}
			return Properties.GetColor("ForeColor");
		}
		public Color GetForeColor(AppearanceObject appearance, ObjectState state) {
			Color res = GetForeColor(state);
			if(res.IsEmpty) return appearance.ForeColor;
			return res;
		}
		public AppearanceObject GetForeColorAppearance(AppearanceObject appearance, ObjectState state) {
			Color clr = GetForeColor(state);
			if(clr.IsEmpty) return appearance;
			AppearanceObject res = (AppearanceObject)appearance.Clone();
			res.ForeColor = clr;
			return res;
		}
		public AppearanceDefault Apply(AppearanceDefault appDefault) {
			return Apply(appDefault, null);
		}
		public AppearanceDefault Apply(AppearanceDefault appDefault, ISkinProvider provider) {
			ApplyForeColorAndFont(appDefault, provider);
			if(Color.GetBackColor() != System.Drawing.Color.Empty) {
				appDefault.BackColor = Color.GetBackColor();
				appDefault.BackColor2 = Color.GetBackColor2();
				appDefault.GradientMode = Color.GradientMode;
			}
			if(Border.All != System.Drawing.Color.Empty)
				appDefault.BorderColor = Border.All;
			if(Border.GetLeft() != System.Drawing.Color.Empty) appDefault.BorderColor = Border.GetLeft();
			return appDefault;
		}
		public Font GetFont(Font defaultFont, FontStyle style) { return GetFont(defaultFont, style, null); }
		public Font GetFont(Font defaultFont, FontStyle style, ISkinProvider provider) {
			if(defaultFont == null) defaultFont = GetDefaultFont(provider);
			return ResourceCache.DefaultCache.GetFont(defaultFont, GetFontSize(defaultFont), style);
		}
		public Font GetDefaultFont(ISkinProvider provider) {
			IAppearanceDefaultFontProvider fp = provider as IAppearanceDefaultFontProvider;
			if(fp != null) {
				Font f = fp.DefaultFont;
				return f ?? AppearanceObject.DefaultFont;
			}
			return AppearanceObject.DefaultFont;
		}
		public Font GetFont(Font defaultFont) { return GetFont(defaultFont, null); }
		public Font GetFont(Font defaultFont, ISkinProvider provider) {
			if(defaultFont == null) defaultFont = GetDefaultFont(provider);
			if(Color.FontBold) return GetFont(defaultFont, FontStyle.Bold);
			float size = GetFontSize(defaultFont);
			if(size == defaultFont.Size) return defaultFont;
			return ResourceCache.DefaultCache.GetFont(defaultFont, GetFontSize(defaultFont), FontStyle.Regular);
		}
		public float GetFontSize() {
			float fontSize = Owner == null || Owner.CommonSkin == null ? 0f : Owner.CommonSkin.Properties.GetFloat(CommonSkins.SkinDefaultFontSize);
			if(Color.FontSize > 0) fontSize = Color.FontSize;
			return fontSize;
		}
		public float GetFontSize(Font defaultFont) {
			float fontSize = Owner == null || Owner.CommonSkin == null ? 0f : Owner.CommonSkin.Properties.GetFloat(CommonSkins.SkinDefaultFontSize);
			if(Color.FontSize > 0) fontSize = Color.FontSize;
			int fontDelta = Properties.GetInteger("FontDelta");
			if(defaultFont == null) return fontSize;
			return (fontSize == 0 ? defaultFont.Size : fontSize) + fontDelta;
		}
		public AppearanceDefault ApplyFontSize(AppearanceDefault appDefault) { return ApplyFontSize(appDefault, null); }
		public AppearanceDefault ApplyFontSize(AppearanceDefault appDefault, ISkinProvider provider) {
			int fontDelta = Properties.GetInteger("FontDelta");
			float fontSize = GetFontSize();
			if(fontDelta != 0 || fontSize > 0) {
				if(fontDelta != 0) appDefault.FontSizeDelta = fontDelta;
				if(fontSize > 0) appDefault.FontSizeDelta = (int)(fontSize - (((int)AppearanceObject.DefaultFont.SizeInPoints) - 9));
			}
			IAppearanceDefaultFontProvider fp = provider as IAppearanceDefaultFontProvider;
			if(fp != null && fp.DefaultFont != null && appDefault.Font == null) appDefault.Font = fp.DefaultFont;
			return appDefault;
		}
		public AppearanceDefault ApplyForeColorAndFont(AppearanceDefault appDefault) {
			return ApplyForeColorAndFont(appDefault, null);
		}
		public AppearanceDefault ApplyForeColorAndFont(AppearanceDefault appDefault, ISkinProvider provider) {
			if(Color.GetForeColor() != System.Drawing.Color.Empty) 
				appDefault.ForeColor = Color.GetForeColor();
			if(Color.FontBold) {
				appDefault.FontStyleDelta = FontStyle.Bold;
			}
			return ApplyFontSize(appDefault, provider);
		}
		public override string ToString() { return ElementName; }
		[Browsable(false)]
		public bool UseColor { get { return ActualInfo.UseColor; } }
		[Browsable(false)]
		public bool UseColor2 { get { return ActualInfo.UseColor2; } }
		public void ClearColoredImages() {
			if(Image != null)
				Image.ClearColoredImages();
			if(Glyph != null)
				Glyph.ClearColoredImages();
		}
	}
	public interface IDpiProvider {
		float DpiScaleFactor { get; }
	}
	public class DpiProvider : IDpiProvider {
		static DpiProvider defaultProvider;
		public static DpiProvider Default {
			get {
				if(defaultProvider == null)
					defaultProvider = new DpiProvider();
				return defaultProvider;
			}
			set {
				defaultProvider = value;
			}
		}
		float dpiScale = 0.0f;
		public float DpiScaleFactor {
			get {
				if(dpiScale == 0.0f)
					dpiScale = GetDpiScale();
				return dpiScale;
			}
		}
		public enum DeviceCap {
			VERTRES = 10,
			DESKTOPVERTRES = 117,
		}
		protected virtual float GetDpiScale() {
			float res = 1.0f;
			using(Control control  = new Control()) {
				Graphics g = control.CreateGraphics();
				res = g.DpiY / 96.0f;
				g.Dispose();
			}
			if(res == 1.0f) {
				res = ScaleUtils.GetScaleFactor().Height;
			}
			if(res == 1.0f)
				res = SystemInformation.IconSize.Width / 32.0f;
			return res;
		}
		public virtual Size ScaleSkinGlyph(Size size) {
			return size;
		}
	}
	public enum SkinColorizationMode { 
		Default,
		Hue,
		Hue2,
		ColorAndHue,
		Color,
		Color2,
		SmartBlending
	}
	public class SkinElements : IEnumerable {
		public SkinElements(Skin skin) {
			Skin = skin;
		}
		Hashtable elements;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Hashtable Elements {
			get {
				if(elements == null)
					elements = new Hashtable();
				return elements;
			}
		}
		public Skin Skin { get; private set; }
		public SkinElement this[string name] { 
			get { 
				SkinElement elem = (SkinElement)Elements[name];
				if(elem != null)
					return elem;
				if(Skin.Container == null || string.IsNullOrEmpty(Skin.Container.Template))
					return null;
				if(Skin.Container.SkinName == TemplateName)
					return null;
				Skin skin = SkinManager.Default.GetSkin(Skin.Name, GetSkinProvider(TemplateName));
				if(skin == null || skin == Skin)
					return null;
				return (SkinElement)skin.Elements[name];
			}
			set { 
				Elements[name] = value;
			}
		}
		protected string TemplateName {
			get { return string.IsNullOrEmpty(Skin.Container.Template) ? SkinManager.DefaultSkinName : Skin.Container.Template; }
		}
		public ICollection Values { get { return Elements.Values; } }
		public void Add(object key, object value) {
			Elements.Add(key, value);
		}
		public void Remove(object key) {
			Elements.Remove(key);
		}
		class SkinProvider : ISkinProvider {
			public string Name { get; set; }
			string ISkinProvider.SkinName {
				get { return Name; }
			}
		}
		SkinProvider provider = new SkinProvider();
		private ISkinProvider GetSkinProvider(string name) {
			provider.Name = name;
			return provider;
		}
		class SkinElementsEnumerator : IEnumerator {
			public SkinElementsEnumerator(SkinElements elements) {
				Elements = elements;
				Enumerator = Elements.Elements.GetEnumerator();
			}
			SkinElements Elements { get; set; }
			IEnumerator Enumerator { get; set; }
			object IEnumerator.Current {
				get { return Enumerator.Current; }
			}
			bool IEnumerator.MoveNext() {
				return Enumerator.MoveNext();
			}
			void IEnumerator.Reset() {
				Enumerator.Reset();
			}
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return new SkinElementsEnumerator(this);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public SkinElement GetElement(string elementName) {
			return (SkinElement)Elements[elementName];
		}
	}
	public class Skin {
		public static bool UseSmartColorizationForImages;
		public static Color SmartColorizationMaskColor;
		public static string OptUseColor = "UseColor";
		public static string OptUseColor2 = "UseColor2";
		public static string OptAllowTouch = "AllowTouch";
		public static string OptMaskColor = "MaskColor";
		public static string OptMaskColor2 = "MaskColor2";
		public static string OptBaseColor = "BaseColor";
		public static string OptBaseColor2 = "BaseColor2";
		public static string OptColorizationMode = "ColorizationMode";
		public static string OptScaleImage = "ScaleImage";
		public static string OptScaleOffset = "ScaleOffset";
		SkinProperties properties;
		SkinColors colors;
		SkinElements elements;
		SkinContainer container;
		string name;
		string cloned;
		IDpiProvider dpiProvider;
		SvgPaletteDictionary svgPalettes;
		public Skin(string name) {
			this.name = name;
			this.properties = new SkinProperties();
			this.properties.SetOwner(this);
			this.colors = new SkinColors(this);
			TouchScaleFactor = 1.0f;
			svgPalettes = new SvgPaletteDictionary();
		}
		public void ClearColoredImages() {
			foreach(SkinElement elem in Elements.Values) {
				elem.ClearColoredImages();
			}
		}
		public SkinColorizationMode ColorizationMode {
			get {
				if(UseSmartColorizationForImages)
					return SkinColorizationMode.SmartBlending;
				if(Properties.ContainsProperty(Skin.OptColorizationMode))
					return (SkinColorizationMode)Properties.GetInteger(Skin.OptColorizationMode);
				return SkinColorizationMode.Default;
			}
			set {
				Properties.SetProperty(Skin.OptColorizationMode, (int)value);
				ClearColoredImages();
			}
		}
		protected internal void UpdateColorizationMode(SkinColorizationMode mode){
			Properties.SetProperty(Skin.OptColorizationMode, (int)mode);
		}
		public IDpiProvider DpiProvider {
			get {
				if(dpiProvider == null) {
					dpiProvider = DevExpress.Skins.DpiProvider.Default;
					OnDpiProviderChanged();
				}
				return dpiProvider;  
			}
			set {
				if(DpiProvider == value)
					return;
				dpiProvider = value;
				OnDpiProviderChanged();
			}
		}
		public float DpiScaleFactor {
			get {
				if(!WindowsFormsSettings.AllowDpiScale)
					return 1.0f;
				return DpiProvider.DpiScaleFactor; 
			}
		}
		protected internal float TouchScaleFactor { get; set; }
		protected virtual void OnDpiProviderChanged() {
		}
		public string Cloned {
			get { return cloned; }
			set {
				cloned = value;
			}
		}
		public virtual object Clone(string skinName) {
			Skin res = new Skin(skinName);
			res.Properties.Clone(Properties);
			res.Colors.Clone(Colors);
			res.SvgPalettes.Clone(SvgPalettes);
			foreach(SkinElement element in GetElements()) {
				SkinElement newElement = element.Copy(res);
				res.AddElement(newElement);
			}
			return res;
		}
		public void AddElement(SkinElement newElement) {
			newElement.Info.Color.SetOwner(this);
			newElement.Info.Properties.SetOwner(this);
			Elements.Add(newElement.ElementName, newElement);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void RemoveElement(string elementName) {
			Elements.Remove(elementName);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void RemoveElement(SkinElement element) {
			Elements.Remove(element.ElementName);
		}
		public SkinProductId GetProductId() {
			if(Container == null) return SkinProductId.Common;
			foreach(SkinProductId product in Enum.GetValues(typeof(SkinProductId))) {
				if(Container.GetSkin(product) == this) return product;
			}
			return SkinProductId.Common;
		}
		public SkinContainer Container { get { return container; } }
		protected internal void SetContainer(SkinContainer container) { this.container = container; }
		public Skin CommonSkin {
			get {
				if(Container == null) return null;
				return Container.CommonSkin;
			}
		}
		protected internal Color GetSystemColorCore(Color color, bool always) {
			Skin common = CommonSkin;
			if(common == null || (common == this && !always) || !color.IsKnownColor) return ConvertColorByBaseColor(color);
			KnownColor known = color.ToKnownColor();
			return common.Colors.GetColor(CommonColors.GetSystemColorName(known));
		}
		public Color TranslateColor(Color color) {
			Skin common = CommonSkin;
			if(common == null || !color.IsKnownColor) return color;
			KnownColor known = color.ToKnownColor();
			string res = CommonColors.GetSystemColorName(known, false);
			if(res == null) return color;
			return common.Colors.GetColor(res);
		}
		public Color GetSystemColor(Color color) {
			return GetSystemColorCore(color, true);
		}
		public SvgPaletteDictionary SvgPalettes { get { return svgPalettes; } }
		public SkinColors Colors { get { return colors; } }
		public SkinProperties Properties { get { return properties; } }
		public string Name { get { return name; } }
		public SkinElement this[string name] { get { return Elements[name] as SkinElement; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public SkinElements Elements {
			get {
				if(elements == null) elements = new SkinElements(this);
				return elements;
			}
		}
		internal void SetSkinName(string name) { this.name = name; }
		public ICollection GetElements() {
			if(elements == null) return (new Hashtable()).Values;
			return Elements.Values;
		}
		protected internal SkinElement Add(string elementName, SkinBuilderElementInfo elementInfo) {
			SkinElement element = Elements.GetElement(elementName);
			if(element == null) {
				element = new SkinElement(this, elementName, elementInfo);
				Elements[elementName] = element;
			}
			return element;
		}
		internal bool HasMaskColor {
			get { return MaskColor != Color.Empty && MaskColor != Color.Transparent; }
		}
		internal bool HasMaskColor2 {
			get { return MaskColor2 != Color.Empty && MaskColor2 != Color.Transparent; }
		}
		public virtual Color ConvertColorByBaseColor(Color color) {
			if(!HasMaskColor || IsSpecialColorValue(color) || ColorizationMode == SkinColorizationMode.SmartBlending)
				return color;
			return SkinImageColorizer.ConvertColor(color, MaskColor, ColorizationMode);
		}
		protected virtual bool IsSpecialColorValue(Color color) {
			return color.Equals(Color.FromArgb(255, 1, 2, 3)) || color.Equals(Color.Empty) || color.Equals(Color.Transparent);
		}
		public Color BaseColor {
			get {
				object res = Colors.CustomProperties[Skin.OptBaseColor];
				if(res != null)
					return (Color)res;
				return Color.Empty;
			}
			set {
				Colors.SetProperty(Skin.OptBaseColor, value);
			}
		}
		public Color BaseColor2 {
			get {
				object res = Colors.CustomProperties[Skin.OptBaseColor2];
				if(res != null)
					return (Color)res;
				return Color.Empty;
			}
			set {
				Colors.SetProperty(Skin.OptBaseColor2, value);
			}
		}
		public Color MaskColor {
			get {
				if(UseSmartColorizationForImages)
					return SmartColorizationMaskColor;
				object res = Colors.CustomProperties[Skin.OptMaskColor];
				if(res != null)
					return (Color)res;
				return Color.Empty;
			}
			set {
				Colors.SetProperty(Skin.OptMaskColor, value);
			}
		}
		public Color MaskColor2 {
			get {
				object res = Colors.CustomProperties[Skin.OptMaskColor2];
				if(res != null)
					return (Color)res;
				return Color.Empty;
			}
			set {
				Colors.SetProperty(Skin.OptMaskColor2, value);
			}
		}
	}
	public class SkinBuilder {
		internal Skin skin;
		public SkinBuilder(string name) { 
			this.skin = new Skin(name);
		}
		public Skin Skin { get { return skin; } }
		public SkinProperties Properties { get { return Skin.Properties; } }
		public SkinElement CreateElement(string elementName, SkinBuilderElementInfo elementInfo) {
			SkinElement element = Skin.Add(elementName, elementInfo);
			return element;
		}
	}
	public class SkinRestrictedColors : SkinCustomProperties {
		protected internal override void SetProperty(object name, object val) {
			CustomProperties[name] = val;	
		}
	}
	public class SkinColors : SkinCustomProperties {
		Skin owner;
		public SkinColors(Skin owner) {
			this.owner = owner;
		}
		SkinRestrictedColors restrictedColors;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public SkinRestrictedColors RestrictedColors {
			get {
				if(restrictedColors == null)
					restrictedColors = new SkinRestrictedColors();
				return restrictedColors;
			}
		}
		[Browsable(false)]
		public Skin Owner { get { return owner; } }
		public Color GetColor(object[] names, Color defaultColor) {
			if(this.customPropertiesHash == null)
				return defaultColor;
			object name = null;
			for(int i = 0; i < names.Length; i++) {
				if(customPropertiesHash[names[i]] == null) continue;
				name = names[i];
				break;
			}
			if(name == null)
				return defaultColor;
			return GetColorCore(name, customPropertiesHash[name], defaultColor);
		}
		public Color GetColor(object name, object defaultName, Color defaultColor) {
			if(this.customPropertiesHash == null)
				return defaultColor;
			bool useName = customPropertiesHash[name] != null;
			return GetColorCore(useName? name: defaultName, customPropertiesHash[name] ?? customPropertiesHash[defaultName], defaultColor);
		}
		public Color GetColor(object name, Color defaultColor) {
			if(this.customPropertiesHash == null)
				return defaultColor;
			return GetColorCore(name, customPropertiesHash[name], defaultColor);
		}
		Color GetColorCore(object name, object obj, Color defaultColor) {
			bool isRestricted = RestrictedColors.CustomProperties.Contains(name);
			if(obj == null)
				return isRestricted? defaultColor: Owner.ConvertColorByBaseColor(defaultColor);
			Color clr = (Color)obj;
			if(clr.IsSystemColor) return Owner.GetSystemColorCore(clr, false);
			return isRestricted? clr: Owner.ConvertColorByBaseColor(clr);
		}
		public Color GetColor(object name) {
			return GetColor(name, Color.Black);
		}
		public Color this[object name] {
			get { return GetColor(name); }
			set { CustomProperties[name] = value; }
		}
		public bool Contains(string name) { return CustomProperties.ContainsKey(name); }
		protected internal override void SetProperty(object name, object val) {
			CustomProperties[name] = val;
		}
	}
	public class SkinProperties : SkinCustomProperties {
		Skin owner;
		public SkinProperties() { 
			AllowScaleProperties = false; 
		}
		public object this[object name] {
			get {
				if(ActualCustomProperties == null)
					return null;
				object res = ActualCustomProperties[name];
				if(res != null)
					return res;
				res = CustomProperties[name];
				if(res != null)
					return res;
				if(OriginalProperties != null)
					return OriginalProperties[name];
				return null;
			}
			set {
				ActualCustomProperties[name] = value;
			}
		}
		protected Hashtable ActualCustomProperties {
			get {
				Hashtable actualProperties = null;
				if(Owner == null || Owner.DpiScaleFactor == 1.0f || !AllowScaleProperties)
					actualProperties = CustomProperties;
				else
					CustomPropertiesList.TryGetValue(Owner.DpiScaleFactor, out actualProperties);
				if(actualProperties == null) {
					if(this.customPropertiesHash == null) return null;
					actualProperties = GenerateProperties(CustomProperties, Owner.DpiScaleFactor);
					CustomPropertiesList.Add(Owner.DpiScaleFactor, actualProperties);
				}
				return actualProperties;
			}
		}
		private Hashtable GenerateProperties(Hashtable prop, float scale) {
			Hashtable table = new Hashtable();
			foreach(object key in prop.Keys) {
				string skey = key.ToString().ToLower();
				if(skey.Contains("color") || skey.Contains("alpha") || skey.Contains("opacity") || skey == "FontDelta")
					continue;
				object value = prop[key];
				if(value is int) {
					table.Add(key, (int)(((int)value) * scale));
				}
			}
			return table;
		}
		Dictionary<float, Hashtable> customPropertiesList;
		protected internal Dictionary<float, Hashtable> CustomPropertiesList {
			get {
				if(customPropertiesList == null)
					customPropertiesList = new Dictionary<float, Hashtable>();
				return customPropertiesList;
			}
		}
		public virtual bool ContainsProperty(object key) {
			if(CustomProperties.ContainsKey(key))
				return true;
			if(OriginalProperties != null)
				return OriginalProperties.CustomProperties.ContainsKey(key);
			return false;
		}
		public Skin Owner { get { return owner; } }
		internal void SetOwner(Skin skin) { this.owner = skin; }
		public SkinPaddingEdges GetPadding(string name, SkinPaddingEdges defaultPadding) {
			object obj = this[name];
			if(obj == null) return defaultPadding;
			return (SkinPaddingEdges)this[name];
		}
		protected Color ConvertColor(Color color) {
			if(SuppressConvertColor)
				return color;
			return Owner == null? color: Owner.ConvertColorByBaseColor(color);
		}
		bool SuppressConvertColor { get; set; }
		public Color GetColor(string name, Color defaultColor) {
			object obj = this[name];
			if(obj == null) return ConvertColor(defaultColor);
			Color res = (Color)obj;
			if(Owner != null && res.IsSystemColor) return Owner.GetSystemColor(res);
			return ConvertColor(res);
		}
		public Color GetColor(string name) {
			return GetColor(name, Color.Empty);
		}
		protected internal Color GetColorCore(string name) {
			try {
				SuppressConvertColor = true;
				return GetColor(name, Color.Empty); }
			finally {
				SuppressConvertColor = false;
			}
		}
		public bool GetBoolean(string name, bool defaultValue) {
			object obj = this[name];
			if(obj == null) return defaultValue;
			return (bool)obj;
		}
		public bool GetBoolean(string name) { return GetBoolean(name, false); }
		public int GetInteger(string name, int defaultValue) {
			object obj = this[name];
			if (obj == null) return defaultValue;
			return (int)obj;
		}
		public int GetInteger(string name) {
			return GetInteger(name, 0);
		}
		public float GetFloat(string name) {
			object obj = this[name];
			if(obj == null) return 0;
			return (float)obj;
		}
		protected internal override void SetProperty(object name, object val) { 
			this[name] = val;
		}
		protected internal SkinProperties OriginalProperties { get; set; }
		public void RemoveProperty(object name) {
			if(OriginalProperties != null)
				OriginalProperties.CustomProperties.Remove(name);
			CustomProperties.Remove(name);
		}
		public bool AllowScaleProperties { get; set; }
	}
	public abstract class SkinCustomProperties {
		protected Hashtable customPropertiesHash;
		protected internal Hashtable CustomProperties { 
			get { 
				if(this.customPropertiesHash == null) this.customPropertiesHash = new Hashtable();
				return customPropertiesHash; 
			} 
		}
		protected internal abstract void SetProperty(object name, object val);
		public void Clone(SkinCustomProperties source) {
			this.customPropertiesHash = null;
			if(source.customPropertiesHash == null) return;
			foreach(DictionaryEntry entry in source.customPropertiesHash) {
				CustomProperties.Add(entry.Key, entry.Value);
			}
		}
	}
	public class SkinBuilderElementInfo {
		SkinImage image;
		SkinGlyph glyph;
		SkinSize size;
		SkinColor color;
		SkinPaddingEdges contentMargins;
		SkinBorder border;
		SkinOffset offset;
		SkinProperties properties;
		public SkinBuilderElementInfo() {
			this.offset = new SkinOffset();
			this.size = new SkinSize();
			this.properties = new SkinProperties() { AllowScaleProperties = false };
			this.contentMargins = new SkinPaddingEdges();
			this.color = new SkinColor();
			this.border = new SkinBorder();
			this.ScaleFactor = 1.0f;
		}
		public SkinBuilderElementInfo Copy() {
			SkinBuilderElementInfo res = (SkinBuilderElementInfo)MemberwiseClone();
			res.Color = (SkinColor)((ICloneable)this.Color).Clone();
			if(GlyphCore != null) {
				res.glyph = Glyph.Copy() as SkinGlyph;
				res.glyph.OwnerInfo = res;
			}
			if(ImageCore != null) {
				res.image = ImageCore.Copy();
				res.image.OwnerInfo = res;
			}
			res.properties = new SkinProperties();
			res.properties.Clone(Properties);
			res.ScaleFactor = ScaleFactor;
			res.OriginalInfo = null;
			res.Owner = Owner;
			return res;
		}
		public SkinProperties Properties { get { return properties; } }
		public SkinOffset Offset { get { return offset; } set { offset = value; } }
		public SkinImage ImageCore { get { return image; } }
		public SkinGlyph GlyphCore { get { return glyph; } }
		[Browsable(false)]
		public bool HasImage {
			get { return (image != null) || (originalInfo != null && originalInfo.HasImage); }
		}
		[Browsable(false)]
		public Image GetActualImage() {
			return (Image ?? originalInfo.Image).ImageCore;
		}
		public void SetActualImage(Image image, bool useOwnImage) {
			(Image ?? originalInfo.Image).UseOwnImage = useOwnImage;
			(Image ?? originalInfo.Image).suppressColorization = useOwnImage;
			(Image ?? originalInfo.Image).SetImage(image , System.Drawing.Color.Empty);
		}
		public SkinImage Image { 
			get {
				if(image != null)
					return image;
				if(OriginalInfo != null)
					return OriginalInfo.Image;
				return image; 
			} 
			set { 
				image = value;
				if(Image != null) {
					Image.OwnerInfo = this;
				}
				OnSkinImageChanged();
			} 
		}
		private void OnSkinImageChanged() {
			if(ScaleFactor == 1.0 || 
				this.image == null || 
				this.image.Image == null || 
				OriginalInfo == null || OriginalInfo.Image == null || 
				OriginalInfo.Image.Stretch != SkinImageStretch.NoResize ||
				Offset.Offset != OriginalInfo.Offset.Offset)
			return;
			Offset.Offset = ScalePoint(OriginalInfo.Offset.Offset, ScaleFactor);
		}
		public SkinGlyph Glyph { 
			get {
				if(glyph != null)
					return glyph;
				if(OriginalInfo != null)
					return OriginalInfo.Glyph;
				return glyph; 
			} 
			set { 
				glyph = value;
				if(Glyph != null) {
					Glyph.OwnerInfo = this;
				}
			} 
		}
		public SkinSize Size { 
			get { return size; } 
			set { 
				size = value;
				sizeTouch = null;
			} 
		}
		SkinSizeTouch sizeTouch = null;
		public SkinSizeTouch SizeTouch {
			get {
				if(sizeTouch == null)
					sizeTouch = new SkinSizeTouch(Size);
				sizeTouch.ScaleFactor = TouchScaleFactorCore;
				return sizeTouch;
			}
			set {
				sizeTouch = value;
			}
		}
		SkinOffsetTouch offsetTouch = null;
		protected internal SkinOffsetTouch OffsetTouch {
			get {
				if(offsetTouch == null)
					offsetTouch = new SkinOffsetTouch(Offset);
				offsetTouch.ScaleFactor = TouchScaleFactorCore;
				return offsetTouch;
			}
			set {
				offsetTouch = value;
			}
		}
		protected internal bool AllowTouch {
			get { return Properties.GetBoolean(Skin.OptAllowTouch); }
		}
		protected internal virtual float TouchScaleFactorCore { 
			get {
				if(!AllowTouch)
					return 1.0f;
				return Owner == null ? 1.0f : Owner.TouchScaleFactor; 
			} 
		}
		SkinPaddingEdges touchContentMargins = new SkinPaddingEdges();
		public SkinPaddingEdges TouchContentMargins {
			get {
				if(!touchContentMargins.IsEmpty)
					return touchContentMargins.Scale(TouchScaleFactorCore);
				return ContentMargins.Scale(TouchScaleFactorCore); 
			}
			set {
				touchContentMargins = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public SkinPaddingEdges TouchContentMarginsCore {
			get { return touchContentMargins; }
			set { touchContentMargins = value; }
		}
		public SkinPaddingEdges ContentMargins { 
			get { return contentMargins; } 
			set { contentMargins = value; } 
		}
		public SkinColor Color { 
			get {
				if(OriginalInfo != null)
					return OriginalInfo.Color;
				return color;
			} 
			set { color = value; } 
		}
		public SkinBorder Border { get { return border; } set { border = value; } }
		SkinBuilderElementInfo originalInfo;
		protected internal virtual SkinBuilderElementInfo OriginalInfo {
			get { return originalInfo; }
			set {
				if(OriginalInfo == value)
					return;
				originalInfo = value;
				OnOriginalInfoChanged();
			}
		}
		protected virtual void OnOriginalInfoChanged() {
			if(Properties != null && OriginalInfo != null)
				Properties.OriginalProperties = OriginalInfo.Properties;
		}
		public float ScaleFactor { get; set; }
		public bool ShouldSerialize { 
			get {
				if(OriginalInfo == null)
					return true;
				if(ShouldSerializeContentMargins)
					return true;
				if(ShouldSerializeGlyph)
					return true;
				if(ShouldSerializeImage)
					return true;
				if(ShouldSerializeProperties)
					return true;
				if(ShouldSerializeSize)
					return true;
				if(ShouldSerializeOffset)
					return true;
				return false;
			}
		}
		public bool ShouldSerializeContentMargins {
			get { return OriginalInfo == null || !OriginalInfo.ContentMargins.Scale(ScaleFactor).Equals(ContentMargins); }
		}
		public bool ShouldSerializeGlyph {
			get { return OriginalInfo == null || Glyph != OriginalInfo.Glyph; }
		}
		public bool ShouldSerializeImage {
			get { return OriginalInfo == null || Image != OriginalInfo.Image; }
		}
		public bool ShouldSerializeProperties {
			get { return Properties.CustomProperties.Count > 0; }
		}
		public bool ShouldSerializeSize {
			get {
				if(OriginalInfo == null)
					return true;
				if(Size.AllowHGrow != OriginalInfo.Size.AllowHGrow)
					return true;
				if(Size.AllowVGrow != OriginalInfo.Size.AllowVGrow)
					return true;
				if(!Size.MinSize.Equals(OriginalInfo.ScaleSize(OriginalInfo.Size.MinSize, ScaleFactor)))
					return true;
				return false;
			}
		}
		public  bool ShouldSerializeOffset { 
			get { 
				if(OriginalInfo == null)
					return true;
				return Offset.Kind != OriginalInfo.Offset.Kind || 
					Offset.Offset != ScalePoint(OriginalInfo.Offset.Offset, ScaleFactor);
			}
		}
		internal Point ScalePoint(Point point, float scaleFactor) {
			return new Point((int)(point.X * scaleFactor), (int)(point.Y * scaleFactor));
		}
		internal Size ScaleSize(Size size, float scaleFactor) {
			return new Size((int)(size.Width * scaleFactor + 0.5f), (int)(size.Height * scaleFactor + 0.5f));
		}
		Skin owner;
		public Skin Owner {
			get {
				if(owner != null)
					return owner;
				if(OriginalInfo != null)
					return OriginalInfo.Owner;
				return null; 
			}
			set {
				if(Owner == value)
					return;
				owner = value;
				UpdateOwner();
			}
		}
		protected internal SkinElement OwnerElement { get; set; }
		protected virtual void UpdateOwner() {
			Color.SetOwner(Owner);
			Border.SetOwner(Owner);
			Properties.SetOwner(Owner);
			if(Image != null) {
				Image.OwnerInfo = this;
			}
			if(Glyph != null) {
				Glyph.OwnerInfo = this;
			}
		}
		public bool UseColor {
			get {
				if(Owner == null || !Owner.HasMaskColor)
					return false;
				if(!Properties.ContainsProperty(Skin.OptUseColor))
					return Owner.HasMaskColor;
				return Properties.GetBoolean(Skin.OptUseColor);
			}
		}
		public bool UseColor2 {
			get {
				if(Owner == null)
					return false;
				if(!Properties.ContainsProperty(Skin.OptUseColor2))
					return false;
				return Properties.GetBoolean(Skin.OptUseColor2);
			}
		}
		protected bool AllowScaleOffset {
			get {
				if(Image != null && Image.Image != null && Image.Stretch == SkinImageStretch.NoResize)
					return false;
				if(!Properties.CustomProperties.ContainsKey(Skin.OptScaleOffset))
					return true;
				return Properties.GetBoolean(Skin.OptScaleOffset);
			}
		}
		public SkinBuilderElementInfo GenerateInfo(float scaleFactor, bool allowScaleImages) {
			SkinBuilderElementInfo scaledInfo = new SkinBuilderElementInfo();
			scaledInfo.ScaleFactor = scaleFactor;
			scaledInfo.OriginalInfo = this;
			scaledInfo.ContentMargins = ContentMargins.Scale(scaleFactor / ScaleFactor);
			scaledInfo.Border = (SkinBorder)Border.Clone();
			scaledInfo.Size.AllowHGrow = Size.AllowHGrow;
			scaledInfo.Size.AllowVGrow = Size.AllowVGrow;
			scaledInfo.Size.MinSize = ScaleSize(Size.MinSize, scaleFactor / ScaleFactor);
			if(!AllowScaleOffset)
				scaledInfo.Offset = new SkinOffset(Offset.Kind, Offset.Offset.X, Offset.Offset.Y);
			else 
				scaledInfo.Offset = new SkinOffset(Offset.Kind, (int)(Offset.Offset.X * scaleFactor / ScaleFactor + 0.5f), (int)(Offset.Offset.Y * scaleFactor / ScaleFactor + 0.5f));
			if(allowScaleImages) {
				scaledInfo.Image = DpiImageHelper.CopySkinImage(Image, scaleFactor / ScaleFactor);
				scaledInfo.Glyph = DpiImageHelper.CopySkinGlyph(Glyph, scaleFactor / ScaleFactor);
			}
			foreach(object key in Properties.CustomProperties.Keys) {
				object value = Properties.CustomProperties[key];
				if(value is int) {
					string skey = key.ToString().ToLower();
					if(skey.Contains("alpha") || skey.Contains("color") || skey.Contains("opacity"))
						continue;
					scaledInfo.Properties.CustomProperties.Add(key, (int)(((int)value) * scaleFactor));
				}
			}
			return scaledInfo;
		}
	}
	public static class DpiImageHelper {
		static Image ResizeImageNoAntialias(Image img, int width, int height) {
			Bitmap bmp = new Bitmap(img, width, height);
			((Bitmap)bmp).SetResolution(img.HorizontalResolution, img.VerticalResolution);
			for(int i = 0; i < bmp.Width; i++) {
				for(int j = 0; j < bmp.Height; j++) {
					bmp.SetPixel(i, j, ((Bitmap)img).GetPixel(i * img.Width / bmp.Width, j * img.Height / bmp.Height));
				}
			}
			return bmp;
		}
		static Image ResizeImageAntialias(Image img, int width, int height) {
			Image res = new Bitmap(img, width, height);
			((Bitmap)res).SetResolution(img.HorizontalResolution, img.VerticalResolution);
			using(Graphics g = Graphics.FromImage(img)) {
				g.Clear(Color.Transparent);
				g.DrawImage(img, new Rectangle(0, 0, width, height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel);
			}
			return res;
		}
		static Image ResizeImage(Image img, int width, int height) {
			if(img.Width > width || img.Height > height)
				return ResizeImageAntialias(img, width, height);
			return ResizeImageNoAntialias(img, width, height);
		}
		static SkinImage CopySkinImageCore(SkinImage res, SkinImage image, float scaleFactor) {
			res.SetImageProvider(null);
			if(image.Image == null)
				return res;
			ImageCollection images = image.GetImages();
			List<Image> scaledImages = new List<Image>();
			int imageWidth = 0, imageHeight = 0;
			int width = 0, height = 0;
			foreach(Image img in images.Images) {
				width = (int)(img.Width * scaleFactor + 0.5f);
				height = (int)(img.Height * scaleFactor + 0.5f);
				if(image.Layout == SkinImageLayout.Horizontal) {
					imageWidth += width;
					imageHeight = height;
				}
				else {
					imageWidth = width;
					imageHeight += height;
				}
				scaledImages.Add(ResizeImage(img, width, height));
			}
			Image scaledImage = new Bitmap(imageWidth, imageHeight);
			((Bitmap)scaledImage).SetResolution(image.Image.HorizontalResolution, image.Image.VerticalResolution);
			using(Graphics g = Graphics.FromImage(scaledImage)) {
				for(int i = 0; i < scaledImages.Count; i++) {
					Point pt = image.Layout == SkinImageLayout.Vertical ? new Point(0, i * height) : new Point(i * width, 0);
					g.DrawImage(scaledImages[i], pt);
					scaledImages[i].Dispose();
				}
			}
			res.SetImage(scaledImage, "");
			return res;
		}
		public static SkinImage CopySkinImage(SkinImage image, float scaleFactor) {
			if(image == null)
				return null;
			SkinImage res = (SkinImage)image.Clone();
			res.SizingMargins = image.SizingMargins.Scale(scaleFactor);
			CopySkinImageCore(res, image, scaleFactor);
			return res;
		}
		public static SkinGlyph CopySkinGlyph(SkinGlyph glyph, float scaleFactor) {
			if(glyph == null)
				return null;
			SkinGlyph res = (SkinGlyph)glyph.Clone();
			res.SizingMargins = glyph.SizingMargins.Scale(scaleFactor);
			CopySkinImageCore(res, glyph, scaleFactor);
			return res;
		}
	}
	public class SkinSizeTouch : SkinSize {
		public SkinSizeTouch(SkinSize size) {
			Size = size;
			ScaleFactor = 1.0f;
			AllowScale = true;
		}
		public float ScaleFactor { get; set; }
		SkinSize Size { get; set; }
		Size? sizeTouch;
		public override Size MinSize {
			get {
				if(sizeTouch.HasValue)
					return ScaleSize(sizeTouch.Value, ScaleFactor);
				return Size != null? ScaleSize(Size.MinSize, ScaleFactor): base.MinSize; 
			}
			set { sizeTouch = value; }
		}
		public bool HasCustomSize { get { return sizeTouch.HasValue; } }
		internal Size ScaleSize(Size size, float scaleFactor) {
			if(!AllowScale)
				return size;
			return new Size((int)(size.Width * scaleFactor), (int)(size.Height * scaleFactor));
		}
		public override bool AllowHGrow {
			get { return Size.AllowHGrow; }
			set { Size.AllowHGrow = value; }
		}
		public override bool AllowVGrow {
			get { return Size.AllowVGrow; }
			set { Size.AllowVGrow = value; }
		}
		public bool AllowScale { get; set; }
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class SkinSize : ICloneable {
		Size minSize;
		internal static Size DefaultSize = new Size(10, 10);
		bool allowHGrow, allowVGrow;
		public SkinSize() : this(DefaultSize, true, true) { }
		public SkinSize(Size minSize) : this(minSize, true, true) { }
		public SkinSize(Size minSize, bool allowGrow) : this(minSize, allowGrow, allowGrow) { }
		public SkinSize(Size minSize, bool allowHGrow, bool allowVGrow) {
			this.minSize = minSize;
			this.allowHGrow = allowHGrow;
			this.allowVGrow = allowVGrow;
		}
		public virtual object Clone() {
			return MemberwiseClone();
		}
		internal bool ShouldSerialize() { return ShouldSerializeMinSize() || !AllowHGrow || !AllowVGrow; }
		bool ShouldSerializeMinSize() { return MinSize != DefaultSize; } 
		[RefreshProperties(RefreshProperties.All)]
		public virtual Size MinSize { 
			get { return minSize; } 
			set { 
				if(value.Width < 0) value.Width = 0;
				if(value.Height < 0) value.Height = 0;
				minSize = value; 
			} 
		}
		[DefaultValue(true)]
		public virtual bool AllowHGrow { get { return allowHGrow; } set { allowHGrow = value; } }
		[DefaultValue(true)]
		public virtual bool AllowVGrow { get { return allowVGrow; } set { allowVGrow = value; } }
		public override string ToString() { 
			if(ShouldSerializeMinSize()) return string.Format("({0})", MinSize);
			return string.Empty; 
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class SkinPaddingEdges : ICloneable {
		bool all = true;
		int top, right, left, bottom;
		public SkinPaddingEdges() { }
		public SkinPaddingEdges(int all) {
			this.all = true;
			this.top = all;
		}
		public SkinPaddingEdges(int left, int top, int right, int bottom) {
			this.left = left;
			this.top = top;
			this.right = right;
			this.bottom = bottom;
			this.all = false;
			OnChanged();
		}
		public SkinPaddingEdges GetRightToLeft(bool rightToLeft) {
			return GetRightToLeft(rightToLeft, FlipType.Default);
		}
		public SkinPaddingEdges GetRightToLeft(bool rightToLeft, FlipType rtlFlipType) {
			return rightToLeft ? RightToLeft(rtlFlipType) : this;
		}
		public SkinPaddingEdges RightToLeft() {
			return RightToLeft(FlipType.Default);
		}
		public SkinPaddingEdges RightToLeft(FlipType rtlFlipType) {
			if(rtlFlipType == FlipType.VerticalFlip)
				return Top == Bottom ? this : new SkinPaddingEdges(Left, Bottom, Right, Top);
			else
				return Left == Right ? this : new SkinPaddingEdges(Right, Top, Left, Bottom);
		}
		protected virtual void OnChanged() {
			if(left == top && left == right && left == bottom) {
				if(top == 0) all = false;
				else all = true;
			}
		}
		public Padding ToPadding() {
			if(IsEmpty) return Padding.Empty;
			return new Padding(Left, Top, Right, Bottom);
		}
		public Padding ToPaddingVertical() {
			if(IsEmpty) return Padding.Empty;
			return new Padding(0, Top, 0, Bottom);
		}
		public Padding ToPaddingHorizontal() {
			if(IsEmpty) return Padding.Empty;
			return new Padding(Left, 0, Right, 0);
		}
		[Browsable(false)]
		public bool IsEmpty { get { return left == 0 && top == 0 && right == 0 && bottom == 0; } }
		[RefreshProperties(RefreshProperties.All), DefaultValue(0)]
		public int All {
			get { return all ? top : 0; }
			set {
				if (all != true || top != value) {
					all = true;
					top = left = right = bottom = value;
					OnChanged();
				}
			}
		}
		[RefreshProperties(RefreshProperties.All), DefaultValue(0)]
		public int Bottom {
			get { return all ? top : bottom; }
			set {
				if (all || bottom != value) {
					all = false;
					bottom = value;
					OnChanged();
				}
			}
		}
		[RefreshProperties(RefreshProperties.All), DefaultValue(0)]
		public int Top {
			get { return top; }
			set {
				if (all || top != value) {
					all = false;
					top = value;
					OnChanged();
				}
			}
		}
		[RefreshProperties(RefreshProperties.All), DefaultValue(0)]
		public int Left {
			get { return all ? top : left; }
			set {
				if (all || left != value) {
					all = false;
					left = value;
					OnChanged();
				}
			}
		}
		[RefreshProperties(RefreshProperties.All), DefaultValue(0)]
		public int Right {
			get { return all ? top : right; }
			set {
				if (all || right != value) {
					all = false;
					right = value;
					OnChanged();
				}
			}
		}
		public int Width { get { return all ? top * 2: left + right; } }
		public int Height { get { return all ? top * 2 : top + bottom; } }
		public override string ToString() {	
			if(IsEmpty) return string.Empty;
			if(All != 0) return string.Format("(All={0})", All);
			string text = GetText("", "Left", Left);
			text = GetText(text, "Top", Top);
			text = GetText(text, "Right", Right);
			text = GetText(text, "Bottom", Bottom);
			return string.Format("({0})", text); 
		}
		string GetText(string currentText, string name, int val) {
			if(val == 0) return currentText;
			if(currentText.Length > 0) currentText += ", ";
			return string.Format("{0}{1}={2}", currentText, name, val);
		}
		public override int GetHashCode() { return base.GetHashCode();	}
		public override bool Equals(object obj) {
			SkinPaddingEdges edges = obj as SkinPaddingEdges;
			if(edges == null) return false;
			return edges.all == all && edges.top == top && edges.left == left && 
				edges.bottom == bottom && 
				edges.right == right;
		}
		public void Scale(float dx, float dy) {
			top = (int)((float)top * dy);
			left = (int)((float)left * dx);
			right = (int)((float)right * dx);
			bottom = (int)((float)bottom * dy);
		}
		public SkinPaddingEdges Scale(float scale) {
			return new SkinPaddingEdges((int)(Left * scale + 0.5f), (int)(Top * scale + 0.5f), (int)(Right * scale + 0.5f), (int)(Bottom * scale + 0.5f));
		}
		bool ShouldSerializeAll() {	return all && top != 0;}
		bool ShouldSerializeBottom() { return !all && bottom != 0;}
		bool ShouldSerializeLeft() { return !all && left != 0; }
		bool ShouldSerializeRight() { return !all && right != 0; }
		bool ShouldSerializeTop() {	return !all && top != 0; }
		void ResetAll() { All = 0;}
		void ResetBottom() { Bottom = 0;}
		void ResetLeft() { Left = 0; }
		void ResetRight() { Right = 0; }
		void ResetTop() { Top = 0; }
		public object Clone() {
			SkinPaddingEdges edges = new SkinPaddingEdges();
			edges.all = all;
			edges.top = top;
			edges.right = right;
			edges.bottom = bottom;
			edges.left = left;
			return edges;
		}
		public Rectangle Deflate(Rectangle bounds) {
			if(IsEmpty) return bounds;
			Rectangle res = bounds;
			if(res.Width < Left + Right) {
				if(res.Width < Left) res.X += res.Width;
				res.Width = 0;
			} else {
				res.X += Left; res.Width -= (Left + Right);
			}
			if(res.Height < Top + Bottom) {
				if(res.Height < Top) res.Y += res.Height;
				res.Height = 0;
			} else {
				res.Y += Top; res.Height -= (Top + Bottom);
			}
			return res;
		}
		public Rectangle Inflate(Rectangle bounds) {
			bounds.X -= Left; bounds.Width += Left + Right;
			bounds.Y -= Top; bounds.Height += Top + Bottom;
			return bounds;
		}
	}
	public static class RectangleHelper {
		public static Rectangle GetCenterBounds(Rectangle dest, Size size) {
			if(dest.Width < size.Width) size.Width = dest.Width;
			if(dest.Height < size.Height) size.Height = dest.Height;
			return new Rectangle(dest.X + (dest.Width - size.Width) / 2, dest.Y + (dest.Height - size.Height) / 2, size.Width, size.Height);
		}
		public static Size Deflate(Size source, Padding padding) {
			source.Width -= padding.Horizontal;
			source.Height -= padding.Vertical;
			return source;
		}
		public static Size Inflate(Size source, Padding padding) {
			source.Width += padding.Horizontal;
			source.Height += padding.Vertical;
			return source;
		}
		public static Rectangle Deflate(Rectangle source, Padding padding) {
			source.X += padding.Left;
			source.Y += padding.Top;
			source.Width -= padding.Horizontal;
			source.Height -= padding.Vertical;
			return source;
		}
		public static Rectangle Inflate(Rectangle source, Padding padding) {
			source.X -= padding.Left;
			source.Y -= padding.Top;
			source.Width += padding.Horizontal;
			source.Height += padding.Vertical;
			return source;
		}
		public static Rectangle GetBounds(Rectangle dest, Size size, SkinOffset offset, SkinOffsetKind defaultOffset) {
			SkinOffsetKind kind = offset.Kind;
			if(kind == SkinOffsetKind.Default) kind = defaultOffset;
			Rectangle res = dest;
			if(dest.Width < size.Width) size.Width = dest.Width;
			if(dest.Height < size.Height) size.Height = dest.Height;
			switch(kind) {
				case SkinOffsetKind.Near : 
					res.Location = new Point(dest.X, dest.Y + (dest.Height - size.Height) / 2);
					break;
				case SkinOffsetKind.Far : 
					res.Location = new Point(dest.Right - size.Width, dest.Y + (dest.Height - size.Height) / 2);
					break;
				default:
					res.Location = new Point(dest.X + (dest.Width - size.Width) / 2, dest.Y + (dest.Height - size.Height) / 2);
					break;
			}
			res.Size = size;
			res.Offset(offset.Offset.X, offset.Offset.Y);
			return res;
		}
		public static bool AllowScale { get { return DevExpress.XtraEditors.WindowsFormsSettings.GetAllowAutoScale(); } }
		public static Size ScaleSize(Size size, SizeF scaleFactor) {
			if(!AllowScale) return size;
			return new Size(ScaleHorizontal(size.Width, scaleFactor.Width), ScaleVertical(size.Height, scaleFactor.Height));
		}
		public static Rectangle ScaleRectangle(Rectangle bounds, SizeF scaleFactor) {
			if(!AllowScale) return bounds;
			return new Rectangle(bounds.Location, ScaleSize(bounds.Size, scaleFactor));
		}
		public static int ScaleVertical(int height, float scaleFactor) {
			if(height == 0 || height == int.MaxValue) return height;
			if(!AllowScale) return height;
			return (int)Math.Round(scaleFactor * height, MidpointRounding.ToEven);
		}
		public static int ScaleHorizontal(int width, float scaleFactor) {
			if(width == 0 || width == int.MaxValue) return width;
			if(!AllowScale) return width;
			return (int)Math.Round(scaleFactor * width, MidpointRounding.ToEven);
		}
		public static Size DeScaleSize(Size size, SizeF scaleFactor) {
			if(!AllowScale) return size;
			return new Size(DeScaleHorizontal(size.Width, scaleFactor.Width), DeScaleVertical(size.Height, scaleFactor.Height));
		}
		public static Rectangle DeScaleRectangle(Rectangle bounds, SizeF scaleFactor) {
			if(!AllowScale) return bounds;
			return new Rectangle(bounds.Location, DeScaleSize(bounds.Size, scaleFactor));
		}
		public static int DeScaleVertical(int height, float scaleFactor) {
			if(height == 0 || height == int.MaxValue || scaleFactor == 0) return height;
			if(!AllowScale) return height;
			return (int)Math.Round(height / scaleFactor, MidpointRounding.ToEven);
		}
		public static int DeScaleHorizontal(int width, float scaleFactor) {
			if(width == 0 || width == int.MaxValue || scaleFactor == 0) return width;
			if(!AllowScale) return width;
			return (int)Math.Round(width / scaleFactor, MidpointRounding.ToEven);
		}
		public static bool IntersectsWith(Rectangle rect, params Rectangle[] samples) {
			for(int i = 0; i < samples.Length; i++) {
				if(rect.IntersectsWith(samples[i])) return true;
			}
			return false;
		}
	}
	public interface ISkinInfoProvider {
		SkinInfo GetSkinInfo(Point point);
	}
	public class SkinInfo {
		Point location;
		Rectangle elementBounds;
		object hitInfo;
		SkinProductId productId;
		SkinElementInfo sampleInfo;
		SkinElementPainter samplePainter;
		string elementName;
		public SkinInfo() {
			this.productId = SkinProductId.Editors;
			this.elementName = string.Empty;
		}
		public SkinElementInfo SampleInfo { get { return sampleInfo; } set { sampleInfo = value; } }
		public SkinElementPainter SamplePainter { get { return samplePainter; } set { samplePainter = value; } }
		public Rectangle ElementBounds { get { return elementBounds; } set { elementBounds = value; } }
		public Point Location { get { return location; } set { location = value; } }
		public object HitInfo { get { return hitInfo; } set { hitInfo = value; } }
		public SkinProductId ProductId { get { return productId; } set { productId = value; } }
		public string ElementName { get { return elementName; } set { elementName = value; } }
	}
	public static class SkinImageColorizer {
		public static int CalcChannelValue(float newVal, float baseVal, float val) {
			if(val < baseVal) return (int)((newVal - newVal / (baseVal * baseVal) * (val - baseVal) * (val - baseVal)) * 255.0f);
			return (int)((newVal + (1 - newVal) / ((1 - baseVal) * (1 - baseVal)) * (val - baseVal) * (val - baseVal)) * 255.0f);
		}
		internal class ColorF {
			public static ColorF FromColor(Color c) {
				ColorF cf = new ColorF();
				cf.AssignColor(c);
				return cf;
			}
			public void AssignColor(Color c) {
				R = c.R / 255.0;
				G = c.G / 255.0;
				B = c.B / 255.0;
			}
			double[] channels = new double[3];
			public double R { get { return channels[0]; } set { channels[0] = value; } }
			public double G { get { return channels[1]; } set { channels[1] = value; } }
			public double B { get { return channels[2]; } set { channels[2] = value; } }
			public double[] Channels { get { return channels; } }
			public double MinChannelValue {
				get {
					return Channels[MinChannel];
				}
			}
			public double MaxChannelValue {
				get {
					return Channels[MaxChannel];
				}
			}
			public double MidChannelValue {
				get {
					return Channels[MidChannel];
				}
			}
			public int MaxChannel {
				get {
					if(Channels[0] > Channels[1]) {
						return Channels[0] > Channels[2] ? 0 : 2;
					}
					return Channels[1] > Channels[2] ? 1 : 2;
				}
			}
			public int MinChannel {
				get {
					if(Channels[0] <= Channels[1]) {
						return Channels[0] <= Channels[2] ? 0 : 2;
					}
					return Channels[1] <= Channels[2] ? 1 : 2;
				}
			}
			public int MidChannel {
				get {
					return Math.Max(3 - MinChannel - MaxChannel, 0);   
				}
			}
			public void AssignColor(ColorF c) {
				R = c.R;
				G = c.G;
				B = c.B;
			}
			public Color ToColor() {
				return Color.FromArgb(255, (int)(R * 255), (int)(G * 255), (int)(B * 255));
			}
		}
		static double Lum(ColorF c) {
			return (0.3 * c.R + 0.59 * c.G + 0.11 * c.B);
		}
		static ColorF SetLum(ColorF c, double l) {
			double d = l - Lum(c);
			ColorF res = new ColorF();
			res.AssignColor(c);
			res.R += d;
			res.G += d;
			res.B += d;
			return ClipColor(res);
		}
		static ColorF ClipColor(ColorF c) {
			double l = Lum(c);
			double min = c.MinChannelValue;
			double max = c.MaxChannelValue;
			ColorF res = new ColorF();
			res.AssignColor(c);
			if(min < 0.0) {
				res.R = l + (c.R - l) * l / (l - min);
				res.G = l + (c.G - l) * l / (l - min);
				res.B = l + (c.B - l) * l / (l - min);
			}
			if(max > 1.0) {
				res.R = l + (c.R - l) * (1 - l) / (max - l);
				res.G = l + (c.G - l) * (1 - l) / (max - l);
				res.B = l + (c.B - l) * (1 - l) / (max - l);
			}
			return res;
		}
		static double Sat(ColorF c) {
			return c.MaxChannelValue - c.MinChannelValue;
		}
		static ColorF SetSat(ColorF c, double s) {
			double cmin = c.MinChannelValue;
			double cmax = c.MaxChannelValue;
			double cmid = c.MidChannelValue;
			int cMinIndex = c.MinChannel;
			int cMaxIndex = c.MaxChannel;
			int cMidIndex = c.MidChannel;
			ColorF res = new ColorF();
			res.AssignColor(c);
			if(cmax > cmin) {
				res.Channels[cMidIndex] = (cmid - cmin) * s / (cmax - cmin);
				res.Channels[cMaxIndex] = s;
			}
			else {
				res.Channels[cMidIndex] = c.Channels[cMaxIndex] = 0.0;
			}
			res.Channels[cMinIndex] = 0.0;
			return res;
		}
		public static Color ConvertColorByHue(Color color, Color maskColor) {
			ColorF baseF = ColorF.FromColor(color);
			ColorF maskF = ColorF.FromColor(maskColor);
			ColorF res = SetLum(SetSat(maskF, Sat(baseF)), Lum(baseF));
			Color cres = res.ToColor();
			Color res2 = Color.FromArgb(color.A, cres);
			HueSatBright hsb1 = new HueSatBright(cres);
			HueSatBright hsb2 = new HueSatBright(maskColor);
			double resBrightness = hsb1.Brightness * hsb2.Brightness;
			double resSaturation = hsb1.Saturation * hsb2.Saturation;
			HueSatBright hsb = new HueSatBright(hsb1.Hue, resSaturation, resBrightness);
			return Color.FromArgb(color.A, hsb.AsRGB);
		}
		public static Color ConvertColor(Color color, Color maskColor, SkinColorizationMode colorizationMode) {
			switch(colorizationMode) {
				case SkinColorizationMode.Default:
				case SkinColorizationMode.Hue:
				case SkinColorizationMode.Hue2: return ConvertColorByHue(color, maskColor);
				case SkinColorizationMode.SmartBlending: return ColorsSmartBlending(color, maskColor);
				default: return ConvertColorByColor(color, maskColor);
			}
		}
		public static Color MultiplyColors(Color color, Color maskColor) {
			return Color.FromArgb(MultiplyArgbColors(color.ToArgb(), maskColor.ToArgb()));
		}
		static int MultiplyArgbColors(int color, int maskColor) {
			byte alpha = (byte)((color >> 0x18) & 0xffL);
			double r = (double)((color >> 0x10) & 0xffL);
			double g = (double)((color >> 0x08) & 0xffL);
			double b = (double)(color & 0xffL);
			double rc = (double)((maskColor >> 0x10) & 0xffL);
			double gc = (double)((maskColor >> 0x08) & 0xffL);
			double bc = (double)(maskColor & 0xffL);
			byte red = (byte)ConvertChannel(rc, r);
			byte green = (byte)ConvertChannel(gc, g);
			byte blue = (byte)ConvertChannel(bc, b);
			return (int)(((uint)((((red << 0x10) | (green << 8)) | blue) | (alpha << 0x18))) & 0xffffffffL);
		}
		static int ConvertChannel(double color, double value) {
			return (int)(color * value) / 255;
		}
		public static bool CheckDarkSkinImage(Image img) {
			Bitmap bitmap = img as Bitmap;
			ulong darkPixelCount = 0;
			ulong lightPixelCount = 0;
			int selectionWidth = 1;
			int hPadding = bitmap.Width / 4;
			int vPadding = bitmap.Height / 4;
			int dy = vPadding;
			int yBound = vPadding + selectionWidth;
			int xBound = bitmap.Width - hPadding;
			bool transform = false;
			int transformCount = 0;
			for(int x = hPadding ; x < xBound; x++) {
				for(int y = dy; y < yBound; y++) {
					int absX = Math.Abs(x);
					int absY = Math.Abs(y);
					Color pixel = transform ? bitmap.GetPixel(absY, absX) : bitmap.GetPixel(absX, absY);
					if(CheckDarkColor(pixel))
						darkPixelCount++;
					else
						lightPixelCount++;
				}
				if(x == xBound - 1) {
					dy = x;
					if(transformCount == 0) {
						xBound = bitmap.Height - vPadding;
						yBound = bitmap.Width - hPadding;
						x = -vPadding;
					}
					if(transformCount == 1) {
						xBound = -hPadding;
						yBound = bitmap.Height - vPadding;
						x = - (bitmap.Width - hPadding);
					}
					if(transformCount == 2) {
						xBound = -vPadding;
						yBound = -hPadding;
						x = -(bitmap.Height - vPadding);
					}
					if(transformCount > 2)
						break;
					transform = !transform;
					transformCount++;
				}
			}
			return darkPixelCount > lightPixelCount;
		}
		public static bool CheckDarkColor(Color color) {
			int argb = color.ToArgb();
			double r = (double)((argb >> 0x10) & 0xffL);
			double g = (double)((argb >> 0x08) & 0xffL);
			double b = (double)(argb & 0xffL);
			return r < 120 & g < 120 & b < 120;
		}
		public static Color ConvertColorByColor(Color color, Color maskColor) {
			ColorF baseF = ColorF.FromColor(color);
			ColorF maskF = ColorF.FromColor(maskColor);
			ColorF res = SetLum(maskF, Lum(baseF));
			HueSatBright hsb = new HueSatBright(res.ToColor());
			return Color.FromArgb(color.A, hsb.AsRGB);
		}
		static int ConvertColorByColor(int color, int maskColor) {
			return ConvertColorByColor(Color.FromArgb(color), Color.FromArgb(maskColor)).ToArgb();
		}
		public static Image GetSmartColorizedImage(Image image, Color color) {
			if(image == null) return null;
			Image sourceImage = image;
			int w = sourceImage.Width; int h = sourceImage.Height;
			Bitmap bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb);
			bmp.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
			using(Graphics g = Graphics.FromImage(bmp)) {
				g.DrawImageUnscaled(sourceImage, 0, 0);
			}
			BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, bmp.PixelFormat);
			int argbColor = color.ToArgb();
			try {
				int argb; int offset = 0;
				for(int i = 0; i < w * h; i++) {
					argb = NativeMethods.ReadInt32(bmpData.Scan0, offset);
					argb = ColorsSmartBlending(argb, argbColor);
					NativeMethods.WriteInt32(bmpData.Scan0, offset, argb);
					offset += sizeof(int);
				}
			}
			finally { bmp.UnlockBits(bmpData); }
			return bmp;
		}
		public static int ColorsSmartBlending(int argb, int maskArgb) {
			return CheckDarkColor(Color.FromArgb(argb)) ? ConvertColorByColor(argb, maskArgb) : MultiplyArgbColors(argb, maskArgb);
		}
		public static Color ColorsSmartBlending(Color color, Color maskColor) {
			return Color.FromArgb(ColorsSmartBlending(color.ToArgb(), maskColor.ToArgb()));
		}
		public static Image GetColoredImage(Image image, Color color, SkinColorizationMode colorizationMode) {
			if(color == Color.Empty || color == Color.Transparent)
				return image;
			Bitmap img = image as Bitmap;
			if(img == null)
				return image;
			if(img.PixelFormat != PixelFormat.Format32bppArgb && img.PixelFormat != PixelFormat.Format24bppRgb) {
				Bitmap temp = new Bitmap(img.Width, img.Height, PixelFormat.Format32bppArgb);
				temp.SetResolution(img.HorizontalResolution, img.VerticalResolution);
				using(Graphics g = Graphics.FromImage(temp)) {
					g.Clear(Color.Transparent);
					g.DrawImage(img, Point.Empty);
				}
				img = temp;
			}
			else
				img = (Bitmap)img.Clone();
			Color c = Color.Empty;
			for(int row = 0; row < img.Height; row++) {
				for(int col = 0; col < img.Width; col++) {
					c = img.GetPixel(col, row);
					img.SetPixel(col, row, ConvertColor(c, color, colorizationMode));
				}
			}
			return img;
		}
	}
	public class SkinProxy : MarshalByRefObject {
		public event SkinHitTestElementChangedEventHandler SkinHitTestElementChanged;
		public void EnableRemoteSkinHitTesting() {
			SkinManager.EnableSkinHitTesting();
			SkinHitTestMessageFilter filter = new SkinHitTestMessageFilter();
			filter.SkinHitTestElementChanged += OnSkinHitTestElementChanged;
			Application.AddMessageFilter(filter);
		}
		void OnSkinHitTestElementChanged(SkinHitTestEventArgs e) {
			if (SkinHitTestElementChanged != null)
				SkinHitTestElementChanged(e);
		}
		[SecurityCritical]
		public override object InitializeLifetimeService() {
			return null;
		}
	}
	public delegate void SkinHitTestElementChangedEventHandler(SkinHitTestEventArgs e);
	[Serializable]
	public class SkinHitTestEventArgs : EventArgs {
		public SkinHitTestEventArgs() { }
		public List<SkinHitTestInfo> HitElements { get; set; }
	}
	[Serializable]
	public class SkinHitTestInfo {
		public SkinHitTestInfo() { }
		public string ElementName { get; set; }
		public SkinProductId ProductId { get; set; }
		public override string ToString() {
			return ElementName;
		}
	}
	public class SkinHitTestMessageFilter : IMessageFilter {
		public SkinHitTestMessageFilter() {
		}
		#region IMessageFilter Members
		public event SkinHitTestElementChangedEventHandler SkinHitTestElementChanged;
		bool IMessageFilter.PreFilterMessage(ref Message m) {
			if (m.Msg == DevExpress.Utils.Drawing.Helpers.MSG.WM_LBUTTONDOWN && Control.ModifierKeys == Keys.Control) {
				SkinManager.HitPoint = Control.MousePosition;
				Control control = Control.FromHandle(m.HWnd);
				if (control != null)
					ForcePaintControl(control);
				SkinHitTestEventArgs args = new SkinHitTestEventArgs();
				args.HitElements = SkinManager.HitElements.Select(el => new SkinHitTestInfo() { ElementName = el.ElementName, ProductId = el.Owner.GetProductId() }).ToList();
				OnSkinHitTestElementChanged(args);
				SkinManager.HitPoint = SkinManager.InvalidPoint;
			}
			return false;
		}
		#endregion
		void ForcePaintControl(Control ctrl) {
			ctrl.Invalidate();
			ctrl.Update();
			foreach (Control child in ctrl.Controls) {
				ForcePaintControl(child);
			}
		}
		void OnSkinHitTestElementChanged(SkinHitTestEventArgs e) {
			if (SkinHitTestElementChanged != null)
				SkinHitTestElementChanged(e);
		}
	}
	public class SkinElementCustomColorizer : IDisposable {
		Color savedMaskColor;
		SkinColorizationMode savedColorizationMode;
		public SkinElementCustomColorizer(SkinElementInfo info, Color color) {
			ElementInfo = info;
			if(CanUseCustomColorizer) {
				savedMaskColor = ElementInfo.Element.Owner.MaskColor;
				savedColorizationMode = ElementInfo.Element.Owner.ColorizationMode;
				ElementInfo.Element.Image.UseSmartColorization = true;
				ElementInfo.Element.Owner.MaskColor = color;
				ElementInfo.Element.Owner.UpdateColorizationMode(SkinColorizationMode.SmartBlending);
			}
		}
		public SkinElementCustomColorizer(Color maskColor) {
			Skin.UseSmartColorizationForImages = true;
			Skin.SmartColorizationMaskColor = maskColor;
		}
		bool CanUseCustomColorizer {
			get { return ElementInfo != null && ElementInfo.Element != null && ElementInfo.Element.Owner != null && ElementInfo.Element.Image != null; }
		}
		SkinElementInfo ElementInfo { get; set; }
		public void Dispose() {
			if(CanUseCustomColorizer) {
				ElementInfo.Element.Image.UseSmartColorization = false;
				ElementInfo.Element.Owner.MaskColor = savedMaskColor;
				ElementInfo.Element.Owner.UpdateColorizationMode(savedColorizationMode);
			}
			else {
				Skin.UseSmartColorizationForImages = false;
				Skin.SmartColorizationMaskColor = Color.Empty;
			} 
		}
	}
	public class SkinImageState { 
		public Dictionary<Color, DevExpress.Skins.SkinImage.ImageRef> ColoredImages{ get; set; }
		public Image Image { get; set; }
	}
}
namespace DevExpress.UserSkins {
	public class OfficeSkins {
		[Obsolete("Use the BonusSkins class instead."), EditorBrowsable(EditorBrowsableState.Never)]
		public static void Register() { }
	}
}
