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
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Utils.Design;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.Design.Commands;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraReports.Localization;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DevExpress.XtraEditors;
using DevExpress.LookAndFeel;
using DevExpress.XtraPrinting;
using DevExpress.Data.Utils;
using DevExpress.Utils;
using System.Linq;
using DevExpress.XtraPrinting.Design;
using DevExpress.LookAndFeel.DesignService;
namespace DevExpress.XtraReports.UserDesigner.Native {
	interface INestedServiceProvider : IServiceProvider {
	}
	class TryCatchHelper {
		IWin32Window window;
		UserLookAndFeel lookAndFeel;
		public TryCatchHelper(UserLookAndFeel lookAndFeel, IWin32Window window) {
			this.window = window;
			this.lookAndFeel = lookAndFeel;
		}
		public void ExecuteAction(Action action) {
			try {
				action();
			} catch(Exception ex) {
				DevExpress.XtraPrinting.Tracer.TraceError(NativeSR.TraceSource, ex);
				NotificationService.ShowException<XtraReport>(lookAndFeel, window, ex);
			}
		}
	}
	public class EnvironmentService : IEnvironmentService {
		bool IEnvironmentService.IsEndUserDesigner { get { return true; } }
	}
	public class XRComponentChangeService : IComponentChangeService {
		public void OnComponentChanged(object component, MemberDescriptor member, object oldValue, object newValue) {
			if(ComponentChanged != null) {
				try {
					ComponentChanged(this, new ComponentChangedEventArgs(component, member, oldValue, newValue));
				} catch(Exception) { }
			}
		}
		public void OnComponentChanging(object component, MemberDescriptor member) {
			if(ComponentChanging != null)
				ComponentChanging(this, new ComponentChangingEventArgs(component, member));
		}
		public void OnComponentAdded(IComponent component) {
			if(ComponentAdded != null)
				ComponentAdded(this, new ComponentEventArgs(component));
		}
		public void OnComponentAdding(IComponent component) {
			if(ComponentAdding != null)
				ComponentAdding(this, new ComponentEventArgs(component));
		}
		public void OnComponentRemoved(IComponent component) {
			if(ComponentRemoved != null)
				ComponentRemoved(this, new ComponentEventArgs(component));
		}
		public void OnComponentRemoving(IComponent component) {
			if(ComponentRemoving != null)
				ComponentRemoving(this, new ComponentEventArgs(component));
		}
		public void OnComponentRename(object component, string oldName, string newName) {
			if(ComponentRename != null)
				ComponentRename(this, new ComponentRenameEventArgs(component, oldName, newName));
		}
		public XRComponentChangeService() {
		}
		#region IComponentChangeService implementation
		public event ComponentEventHandler ComponentAdded;
		public event ComponentEventHandler ComponentAdding;
		public event ComponentChangedEventHandler ComponentChanged;
		public event ComponentChangingEventHandler ComponentChanging;
		public event ComponentEventHandler ComponentRemoved;
		public event ComponentEventHandler ComponentRemoving;
		public event ComponentRenameEventHandler ComponentRename;
		#endregion
	}
	public class XRDesignerEventService : IDesignerEventService {
		IDesignerHost fActiveDesigner;
		ArrayList fDesigners = new ArrayList();
		public IDesignerHost ActiveDesigner {
			get { return fActiveDesigner; }
		}
		public DesignerCollection Designers {
			get { return new DesignerCollection(fDesigners); }
		}
		public void AddDesigner(IDesignerHost host) {
			this.fDesigners.Add(host);
			if(fDesigners.Count == 1)
				SetActiveDesigner(host);
			OnDesignerCreated(new DesignerEventArgs(host));
		}
		public void RemoveDesigner(IDesignerHost host) {
			fDesigners.Remove(host);
			if(fActiveDesigner == host) {
				if(fDesigners.Count <= 0) {
					SetActiveDesigner(null);
				} else {
					this.SetActiveDesigner((IDesignerHost)this.fDesigners[fDesigners.Count - 1]);
				}
			}
			((IContainer)host).Dispose();
			OnDesignerDisposed(new DesignerEventArgs(host));
		}
		public void SetActiveDesigner(IDesignerHost host) {
			if(fActiveDesigner != host) {
				IDesignerHost oldDesigner = fActiveDesigner;
				fActiveDesigner = host;
				FireSelectionChanged();
				OnActiveDesignerChanged(new ActiveDesignerEventArgs(oldDesigner, host));
			}
		}
		public void FireSelectionChanged() {
			if(SelectionChanged != null)
				SelectionChanged(this, EventArgs.Empty);
		}
		protected virtual void OnDesignerCreated(DesignerEventArgs e) {
			if(DesignerCreated != null)
				DesignerCreated(this, e);
		}
		protected virtual void OnActiveDesignerChanged(ActiveDesignerEventArgs e) {
			if(ActiveDesignerChanged != null)
				ActiveDesignerChanged(this, e);
		}
		protected virtual void OnDesignerDisposed(DesignerEventArgs e) {
			if(DesignerDisposed != null)
				DesignerDisposed(this, e);
		}
		public event EventHandler SelectionChanged;
		public event DesignerEventHandler DesignerCreated;
		public event DesignerEventHandler DesignerDisposed;
		public event ActiveDesignerEventHandler ActiveDesignerChanged;
	}
	public class XRDesignerSerializationService : IDesignerSerializationService {
		IServiceProvider serviceProvider;
		public XRDesignerSerializationService(IServiceProvider serviceProvider) {
			this.serviceProvider = serviceProvider;
		}
		#region IDesignerSerializationService Members
		public System.Collections.ICollection Deserialize(object serializationData) {
			SerializationStore serializationStore = serializationData as SerializationStore;
			if(serializationStore != null) {
				ComponentSerializationService componentSerializationService = serviceProvider.GetService(typeof(ComponentSerializationService)) as ComponentSerializationService;
				IDesignerHost designerHost = (IDesignerHost)serviceProvider.GetService(typeof(IDesignerHost));
				if(designerHost != null)
					return componentSerializationService.Deserialize(serializationStore, designerHost.Container);
				else
					return componentSerializationService.Deserialize(serializationStore);
			}
			return new object[0];
		}
		public object Serialize(System.Collections.ICollection objects) {
			ComponentSerializationService componentSerializationService = serviceProvider.GetService(typeof(ComponentSerializationService)) as ComponentSerializationService;
			using(SerializationStore serializationStore = componentSerializationService.CreateStore()) {
				foreach(object obj in objects)
					componentSerializationService.Serialize(serializationStore, obj);
				return serializationStore;
			}
		}
		#endregion
	}
	public class XRExtenderService : IExtenderListService, IExtenderProviderService {
		ArrayList fExtenderProviders = new ArrayList();
		public ArrayList ExtenderProviders {
			get { return fExtenderProviders; }
		}
		public XRExtenderService() {
		}
		#region System.ComponentModel.Design.IExtenderListService interface implementation
		public IExtenderProvider[] GetExtenderProviders() {
			return (IExtenderProvider[])fExtenderProviders.ToArray(typeof(IExtenderProvider));
		}
		#endregion
		#region System.ComponentModel.Design.IExtenderProviderService interface implementation
		public void RemoveExtenderProvider(System.ComponentModel.IExtenderProvider provider) {
			fExtenderProviders.Remove(provider);
		}
		public void AddExtenderProvider(System.ComponentModel.IExtenderProvider provider) {
			fExtenderProviders.Add(provider);
		}
		#endregion
	}
#if DEBUGTEST
	public
#endif
	class XRMenuCommandService : XRMenuCommandServiceBase, IDisposable {
		Control panel;
		IDesignerHost host;
		FieldDropMenu fieldDropMenu;
		SelectionMenu selectionMenu, reportExplorerMenu;
		ArrayList verbs = new ArrayList();
		public override DesignerVerbCollection Verbs {
			get {
				DesignerVerbCollection verbCollection = CreateDesignerVerbCollection();
				verbCollection.AddRange((DesignerVerb[])verbs.ToArray(typeof(DesignerVerb)));
				return verbCollection;
			}
		}
		public XRMenuCommandService(IDesignerHost host, Control panel) {
			this.host = host;
			this.panel = panel;
		}
		public override void AddVerb(DesignerVerb verb) {
			Debug.Assert(verb != null);
			this.verbs.Add(verb);
		}
		public override void RemoveVerb(DesignerVerb verb) {
			Debug.Assert(verb != null);
			verbs.Remove(verb);
		}
		public override bool GlobalInvoke(CommandID commandID, object[] args) {
			ReportCommandService reportCommandService = host.GetService(typeof(ReportCommandService)) as ReportCommandService;
			if(reportCommandService != null && reportCommandService.HandleCommand(CommandIDReportCommandConverter.GetReportCommand(commandID), args)) {
				return true;
			}
			return base.GlobalInvoke(commandID, args);
		}
		public override MenuCommand FindCommand(CommandID commandID) {
			MenuCommand menuCommand = base.FindCommand(commandID);
			if(menuCommand != null)
				return menuCommand;
			foreach(DesignerVerb designerVerb in Verbs) {
				if(Object.Equals(designerVerb.CommandID, commandID)) {
					return designerVerb;
				}
			}
			return null;
		}
		public override void ShowContextMenu(CommandID commandID, int x, int y) {
			if(commandID == MenuCommandServiceCommands.SelectionMenu) {
				if(selectionMenu == null)
					selectionMenu = new SelectionMenu(host, MenuKind.Selection);
				ShowContextMenu(selectionMenu, x, y);
			} else if(commandID == MenuCommandServiceCommands.ReportExplorerMenu) {
				if(reportExplorerMenu == null)
					reportExplorerMenu = new SelectionMenu(host, MenuKind.ReportExplorer);
				ShowContextMenu(reportExplorerMenu, x, y);
			} else if(commandID == MenuCommandServiceCommands.FieldDropMenu) {
				if(fieldDropMenu == null)
					fieldDropMenu = new FieldDropMenu(host);
				ShowContextMenu(fieldDropMenu, x, y);
			}
		}
		public override void ShowContextMenu(XtraContextMenuBase menu, int x, int y) {
			if(menu != null) {
				menu.Show(panel, new Point(x, y), host);
			}
		}
		public DesignerVerbCollection CreateDesignerVerbCollection() {
			DesignerVerbCollection designerVerbCollection = new DesignerVerbCollection();
			ISelectionService selectionService = (ISelectionService)host.GetService(typeof(ISelectionService));
			if(selectionService != null && selectionService.SelectionCount == 1) {
				IComponent selectedComponent = selectionService.PrimarySelection as Component;
				if(selectedComponent != null) {
					IDesigner designer = host.GetDesigner((IComponent)selectedComponent);
					if(designer != null) {
						designerVerbCollection.AddRange(designer.Verbs);
					}
				}
				if(selectedComponent == host.RootComponent) {
					designerVerbCollection.AddRange((DesignerVerb[])this.verbs.ToArray(typeof(DesignerVerb)));
				}
			}
			return designerVerbCollection;
		}
		public void Dispose() {
			if(selectionMenu != null)
				selectionMenu = null;
			if(reportExplorerMenu != null)
				reportExplorerMenu = null;
			if(fieldDropMenu != null)
				fieldDropMenu = null;
		}
#if DEBUGTEST
		public SelectionMenu Test_GetSelectionMenu() {
			return selectionMenu;
		}
#endif
	}
	public class XRSelectionService : ISelectionService, IDisposable {
		#region static
		static bool IsArrayEqual(IList arr1, IList arr2) {
			if(arr1.Count != arr2.Count)
				return false;
			for(int i = 0; i < arr1.Count; i++) {
				if(!arr1[i].Equals(arr2[i]))
					return false;
			}
			return true;
		}
		#endregion
		IDesignerHost fHost;
		ArrayList fSelectedComponents = new ArrayList();
		bool selectionChanged;
		public object PrimarySelection {
			get {
				if(fSelectedComponents.Count > 0) {
					return fSelectedComponents[0];
				}
				return null;
			}
		}
		public int SelectionCount {
			get { return fSelectedComponents.Count; }
		}
		public XRSelectionService(IDesignerHost host) {
			Debug.Assert(host != null);
			this.fHost = host;
			((IComponentChangeService)host.GetService(typeof(IComponentChangeService))).ComponentRemoved += new ComponentEventHandler(ComponentRemovedHandler);
			fHost.TransactionClosed += new DesignerTransactionCloseEventHandler(OnTransactionClosed);
			fHost.LoadComplete += new EventHandler(OnLoadComplete);
		}
		#region Event methods
		public event EventHandler SelectionChanging;
		public event EventHandler SelectionChanged;
		protected virtual void OnSelectionChanging(EventArgs e) {
			if(SelectionChanging != null)
				SelectionChanging(this, e);
		}
		protected virtual void OnSelectionChanged(EventArgs e) {
			if(SelectionChanged != null)
				SelectionChanged(this, e);
		}
		#endregion
		void OnLoadComplete(object sender, EventArgs e) {
			FireSelectionEvents();
		}
		void OnTransactionClosed(object sender, DesignerTransactionCloseEventArgs e) {
			FireSelectionEvents();
		}
		void FireSelectionEvents() {
			if(selectionChanged) {
				FireSelectionChanging();
				FireSelectionChange();
			}
		}
		void ComponentRemovedHandler(object sender, ComponentEventArgs e) {
			if(!fSelectedComponents.Contains(e.Component))
				return;
			FireSelectionChanging();
			fSelectedComponents.Remove(e.Component);
			if(fSelectedComponents.Count == 0 && fHost.RootComponent != null)
				AddSelectedComponent(fHost.RootComponent);
			FireSelectionChange();
		}
		void AddSelectedComponent(object component) {
			if(component is XRChart)
				fSelectedComponents.Insert(0, component);
			else
				fSelectedComponents.Add(component);
		}
		void FireSelectionChanging() {
			if(DesignMethods.IsDesignerInTransaction(fHost))
				return;
			OnSelectionChanging(EventArgs.Empty);
		}
		void FireSelectionChange() {
			selectionChanged = DesignMethods.IsDesignerInTransaction(fHost);
			if(DesignMethods.IsDesignerInTransaction(fHost))
				return;
			OnSelectionChanged(EventArgs.Empty);
			XRDesignerEventService designerEventService = fHost.GetService(typeof(IDesignerEventService)) as XRDesignerEventService;
			if(designerEventService != null)
				designerEventService.FireSelectionChanged();
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected void Dispose(bool disposing) {
			if(disposing) {
				((IComponentChangeService)fHost.GetService(typeof(IComponentChangeService))).ComponentRemoved -= new ComponentEventHandler(ComponentRemovedHandler);
				fHost.TransactionClosed -= new DesignerTransactionCloseEventHandler(OnTransactionClosed);
				fHost.LoadComplete -= new EventHandler(OnLoadComplete);
			}
		}
		~XRSelectionService() {
			Dispose(false);
		}
		public bool GetComponentSelected(object component) {
			return fSelectedComponents.Contains(component);
		}
		public ICollection GetSelectedComponents() {
			return fSelectedComponents.ToArray();
		}
		public void SetSelectedComponents(ICollection components, SelectionTypes selectionType) {
			if(IsArrayEqual(fSelectedComponents, new ArrayList(components)))
				return;
			FireSelectionChanging();
			if(components == null || components.Count == 0) {
				fSelectedComponents.Clear();
				FireSelectionChange();
				return;
			}
			bool controlPressed = (Control.ModifierKeys & Keys.Control) == Keys.Control;
			bool shiftPressed = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
			switch(selectionType) {
				case SelectionTypes.Replace:
					ReplaceSelection(components);
					break;
				default:
					if(components.Count == 1 && (controlPressed || shiftPressed)) {
						ToggleSelection(components);
					} else if(controlPressed) {
						AddSelection(components);
					} else if(shiftPressed) {
						ReplaceSelection(components);
					} else {
						NormalSelection(components);
					}
					break;
			}
			fSelectedComponents.TrimToSize();
			FireSelectionChange();
		}
		public void SetSelectedComponents(ICollection components) {
			SetSelectedComponents(components, SelectionTypes.Replace);
		}
		#region SetSelection helper methods
		void ToggleSelection(ICollection components) {
			foreach(object component in components) {
				if(component == null)
					continue;
				if(GetComponentSelected(component))
					fSelectedComponents.Remove(component);
				else
					AddSelectedComponent(component);
			}
		}
		void AddSelection(ICollection components) {
			foreach(object component in components)
				if(component != null && !GetComponentSelected(component))
					AddSelectedComponent(component);
		}
		void ReplaceSelection(ICollection components) {
			fSelectedComponents.Clear();
			AddSelection(components);
		}
		void NormalSelection(ICollection components) {
			if(components.Count == 1) {
				object componentToAdd = null;
				foreach(object component in components)
					if(component != null)
						componentToAdd = component;
				if(componentToAdd != null && GetComponentSelected(componentToAdd)) {
					fSelectedComponents.Remove(componentToAdd);
					fSelectedComponents.Insert(0, componentToAdd);
					return;
				}
			}
			ReplaceSelection(components);
		}
		#endregion
	}
	public delegate void ToolboxEventHandler(object sender, ToolboxEventArgs e);
	public class ToolboxEventArgs : EventArgs {
		ToolboxItem item = null;
		string category = null;
		IDesignerHost host = null;
		public ToolboxEventArgs(ToolboxItem item, string category, IDesignerHost host) {
			this.item = item;
			this.category = category;
			this.host = host;
		}
		public ToolboxItem Item {
			get { return item; }
		}
		public string Category {
			get { return category; }
		}
		public IDesignerHost Host {
			get { return host; }
		}
	}
	public class XRToolboxService : IToolboxService, IDisposable {
		public static ToolboxItem[][] GroupItemsBySubCategory(ToolboxItemCollection items, IDesignerHost designerHost) {
			return GroupItemsBySubCategory(EnumerateItems(items, designerHost));
		}
		static ToolboxItem[][] GroupItemsBySubCategory(IEnumerable<Tuple<ToolboxItem, int, int>> items) {
			SortedDictionary<int, List<KeyValuePair<int, ToolboxItem>>> categorizedItems = new SortedDictionary<int, List<KeyValuePair<int, ToolboxItem>>>();
			foreach(Tuple<ToolboxItem, int, int> item in items) {
				int group = item.Item2;
				int position = item.Item3;
				List<KeyValuePair<int, ToolboxItem>> list;
				if(!categorizedItems.TryGetValue(group, out list)) {
					list = new List<KeyValuePair<int, ToolboxItem>>();
					categorizedItems.Add(group, list);
				}
				list.Add(new KeyValuePair<int, ToolboxItem>(position, item.Item1));
			}
			ToolboxItem[][] result = new ToolboxItem[categorizedItems.Count][];
			int i = 0;
			foreach(KeyValuePair<int, List<KeyValuePair<int, ToolboxItem>>> kvp in categorizedItems) {
				List<KeyValuePair<int, ToolboxItem>> list = kvp.Value;
				list.Sort((x, y) => { return Comparer<int>.Default.Compare(x.Key, y.Key); });
				result[i] = list.ConvertAll(pair => { return pair.Value; }).ToArray();
				i++;
			}
			return result;
		}
		static IEnumerable<Tuple<ToolboxItem, int, int>> EnumerateItems(ToolboxItemCollection items, IDesignerHost designerHost) {
			foreach(ToolboxItem item in items) {
				Type type = item.GetType(designerHost);
				XRToolboxSubcategoryAttribute attr = GetAttribute(type, typeof(XRToolboxSubcategoryAttribute)) as XRToolboxSubcategoryAttribute;
				int group = attr != null ? attr.Subcategory : 0;
				int position = attr != null ? attr.Position : 0;
				yield return Tuple.Create(item, group, position);
			}
		}
#if DEBUGTEST
		public static ToolboxItem[][] Test_GroupItemsBySubCategory(IEnumerable<Tuple<ToolboxItem, int, int>> items) {
			return GroupItemsBySubCategory(items);
		}
#endif
		static object GetAttribute(Type type, Type attributeType) {
			object[] attributes = type.GetCustomAttributes(attributeType, true);
			return attributes.Length > 0 ? attributes[0] : null;
		}
		public static Type GetType(ToolboxItem item) {
			string typeName = item.TypeName;
			string assemblyName = item.AssemblyName.FullName;
			return Type.GetType(String.Format("{0}, {1}", typeName, assemblyName));
		}
		#region internal classes
		class EmptyDesignerHost : IDesignerHost {
			static EmptyDesignerHost instance = new EmptyDesignerHost();
			internal static EmptyDesignerHost Instance {
				get { return instance; }
			}
			public EmptyDesignerHost() {
				IgnoreFunction();
			}
			private void IgnoreFunction() {
				bool ignore = Activated == null
					&& Deactivated == LoadComplete
					&& TransactionClosed == TransactionClosing
					&& TransactionOpened == TransactionOpening;
				ignore = !ignore;
			}
			#region IDesignerHost Members
			public void Activate() {
			}
			public event EventHandler Activated;
			public IContainer Container {
				get { return null; }
			}
			public IComponent CreateComponent(Type componentClass, string name) {
				return null;
			}
			public IComponent CreateComponent(Type componentClass) {
				return null;
			}
			public DesignerTransaction CreateTransaction(string description) {
				return null;
			}
			public DesignerTransaction CreateTransaction() {
				return null;
			}
			public event EventHandler Deactivated;
			public void DestroyComponent(IComponent component) {
			}
			public IDesigner GetDesigner(IComponent component) {
				return null;
			}
			public Type GetType(string typeName) {
				return null;
			}
			public bool InTransaction {
				get { return false; }
			}
			public event EventHandler LoadComplete;
			public bool Loading {
				get { return false; }
			}
			public IComponent RootComponent {
				get { return null; }
			}
			public string RootComponentClassName {
				get { return null; }
			}
			public event DesignerTransactionCloseEventHandler TransactionClosed;
			public event DesignerTransactionCloseEventHandler TransactionClosing;
			public string TransactionDescription {
				get { return null; }
			}
			public event EventHandler TransactionOpened;
			public event EventHandler TransactionOpening;
			#endregion
			#region IServiceContainer Members
			public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) {
			}
			public void AddService(Type serviceType, ServiceCreatorCallback callback) {
			}
			public void AddService(Type serviceType, object serviceInstance, bool promote) {
			}
			public void AddService(Type serviceType, object serviceInstance) {
			}
			public void RemoveService(Type serviceType, bool promote) {
			}
			public void RemoveService(Type serviceType) {
			}
			#endregion
			#region IServiceProvider Members
			public object GetService(Type serviceType) {
				return null;
			}
			#endregion
		}
		[StructLayout(LayoutKind.Sequential)]
		struct IconInfo {
			public bool fIcon;
			public int xHotspot;
			public int yHotspot;
			public IntPtr hbmMask;
			public IntPtr hbmColor;
		}
		#endregion
		static readonly Type[] formats = new Type[] { typeof(LocalizableToolboxItem), typeof(ToolboxItem) };
		Dictionary<string, List<ToolboxItem>> toolboxByCategory = new Dictionary<string, List<ToolboxItem>>();
		Dictionary<IDesignerHost, List<ToolboxItem>> toolboxByHost = new Dictionary<IDesignerHost, List<ToolboxItem>>();
		List<ToolboxItem> toolboxItems = new List<ToolboxItem>();
		Dictionary<Type, Image> typeToImage = new Dictionary<Type, Image>();
		Dictionary<Type, Image> typeToLargeImage = new Dictionary<Type, Image>();
		IDictionary creators = new HybridDictionary();
		IDictionary creatorsByHost = new ListDictionary();
		string selectedCategory;
		ToolboxItem selectedItem;
		Cursor toolboxItemCursor;
		string toolboxItemName;
		bool disposed;
		public virtual string DefaultCategoryName {
			get { return ReportLocalizer.GetString(ReportStringId.UD_XtraReportsToolboxCategoryName); }
		}
		public XRToolboxService() {
			List<ToolboxItem> list = new List<ToolboxItem>();
			toolboxByHost.Add(EmptyDesignerHost.Instance, list);
			AddToolboxItems();
		}
		internal Image GetImage(ToolboxItem toolboxItem) {
			Type type = GetType(toolboxItem);
			Image image = GetImageByType(type);
			if(image == null) {
				try {
					image = DesignImageHelper.Get256ColorBitmap(type);
				} catch {
					image = toolboxItem.Bitmap;
				}
			}
			AddToolBoxImage(type, image);
			return image;
		}
		protected virtual void AddToolboxItems() {
			foreach(ToolboxItem toolboxItem in ToolboxHelper.XRLocalizableToolboxItems)
				AddToolboxItem(toolboxItem, ReportLocalizer.GetString(ReportStringId.UD_XtraReportsToolboxCategoryName));
		}
		public CategoryNameCollection CategoryNames {
			get {
				string[] names = new string[toolboxByCategory.Count];
				toolboxByCategory.Keys.CopyTo(names, 0);
				return new CategoryNameCollection(names);
			}
		}
		public string SelectedCategory {
			get { return selectedCategory; }
			set {
				if(value != selectedCategory) {
					OnSelectedCategoryChanging();
					selectedCategory = value;
					OnSelectedCategoryChanged();
				}
			}
		}
		public void AddCreator(ToolboxItemCreatorCallback creator, string format) {
			AddCreator(creator, format, null);
		}
		public void AddCreator(ToolboxItemCreatorCallback creator, string format, IDesignerHost host) {
			if(host == null) {
				creators.Add(format, creator);
			} else {
				IDictionary creatorsDict = (IDictionary)creatorsByHost[host];
				if(creatorsDict == null) {
					creatorsDict = new HybridDictionary();
					creatorsByHost.Add(host, creatorsDict);
				}
				creatorsDict[format] = creator;
			}
		}
		private void AddToolboxItemByHost(ToolboxItem toolboxItem, IDesignerHost host) {
			if(host != null) {
				List<ToolboxItem> list = null;
				if(!toolboxByHost.TryGetValue(host, out list)) {
					list = new List<ToolboxItem>();
					toolboxByHost.Add(host, list);
				}
				list.Add(toolboxItem);
			} else {
				List<ToolboxItem> list = null;
				if(toolboxByHost.TryGetValue(EmptyDesignerHost.Instance, out list))
					list.Add(toolboxItem);
			}
		}
		private void AddToolboxItemByCategory(ToolboxItem toolboxItem, string category) {
			if(category != null) {
				List<ToolboxItem> list = null;
				if(!toolboxByCategory.TryGetValue(category, out list)) {
					list = new List<ToolboxItem>();
					toolboxByCategory.Add(category, list);
				}
				list.Add(toolboxItem);
			} else {
				List<ToolboxItem> list = null;
				if(toolboxByCategory.TryGetValue(DefaultCategoryName, out list))
					list.Add(toolboxItem);
			}
		}
		void AddItemToToolbox(ToolboxItem toolboxItem, string category, IDesignerHost host) {
			if(!CanAddToolBoxItem(toolboxItem))
				return;
			toolboxItems.Add(toolboxItem);
			AddToolboxItemByHost(toolboxItem, host);
			AddToolboxItemByCategory(toolboxItem, category);
			OnToolboxItemAdded(toolboxItem, category, host);
		}
		public void AddToolBoxImage(Type type, Image toolboxImage) {
			if(typeToImage.ContainsKey(type))
				typeToImage.Remove(type);
			typeToImage.Add(type, toolboxImage);
		}
		public void AddToolBoxImage(Type type, Type sourceType) {
			Image image = null;
			if(typeToImage.TryGetValue(sourceType, out image))
				AddToolBoxImage(type, image);
		}
		public void AddToolBoxLargeImage(Type type, Image toolboxImage) {
			if(typeToLargeImage.ContainsKey(type))
				typeToLargeImage.Remove(type);
			typeToLargeImage.Add(type, toolboxImage);
		}
		public void AddToolBoxLargeImage(Type type, Type sourceType) {
			Image image = null;
			if(typeToLargeImage.TryGetValue(sourceType, out image))
				AddToolBoxLargeImage(type, image);
		}
		protected internal virtual bool CanAddToolBoxItem(ToolboxItem toolboxItem) {
			return true;
		}
		public void AddLinkedToolboxItem(ToolboxItem toolboxItem, string category, IDesignerHost host) {
			AddItemToToolbox(toolboxItem, category, host);
		}
		public void AddLinkedToolboxItem(ToolboxItem toolboxItem, IDesignerHost host) {
			this.AddLinkedToolboxItem(toolboxItem, null, host);
		}
		public void AddToolboxItem(ToolboxItem toolboxItem) {
			this.AddItemToToolbox(toolboxItem, null, null);
		}
		public void AddToolboxItem(ToolboxItem toolboxItem, string category) {
			this.AddItemToToolbox(toolboxItem, category, null);
		}
		public ToolboxItem DeserializeToolboxItem(object serializedObject) {
			if(serializedObject is System.Windows.Forms.IDataObject) {
				Type format = GetDataFormat((IDataObject)serializedObject);
				if(format != null)
					return (ToolboxItem)((IDataObject)serializedObject).GetData(format);
			}
			return null;
		}
		public ToolboxItem DeserializeToolboxItem(object serializedObject, IDesignerHost host) {
			ToolboxItem item = DeserializeToolboxItem(serializedObject);
			return GetToolboxItem(host, item);
		}
		public ToolboxItem GetSelectedToolboxItem() {
			return selectedItem;
		}
		public ToolboxItem GetSelectedToolboxItem(IDesignerHost host) {
			return GetToolboxItem(host, selectedItem);
		}
		ToolboxItem GetToolboxItem(IDesignerHost host, ToolboxItem item) {
			if(item == null || host == null)
				return null;
			List<ToolboxItem> list = null;
			if(toolboxByHost.TryGetValue(host, out list) && list.Contains(item))
				return item;
			toolboxByHost.TryGetValue(EmptyDesignerHost.Instance, out list);
			if(list != null && list.Contains(item))
				return item;
			return null;
		}
		public ToolboxItemCollection GetToolboxItems() {
			return new ToolboxItemCollection(toolboxItems.ToArray());
		}
		public ToolboxItemCollection GetToolboxItems(string category) {
			if(category == null)
				category = DefaultCategoryName;
			List<ToolboxItem> items = null;
			if(!toolboxByCategory.TryGetValue(category, out items))
				items = new List<ToolboxItem>(0);
			return new ToolboxItemCollection(items.ToArray());
		}
		public ToolboxItemCollection GetToolboxItems(string category, IDesignerHost host) {
			if(category == null)
				category = DefaultCategoryName;
			List<ToolboxItem> hList = null;
			if(host == null)
				toolboxByHost.TryGetValue(EmptyDesignerHost.Instance, out hList);
			else
				toolboxByHost.TryGetValue(host, out hList);
			List<ToolboxItem> cList = null;
			toolboxByCategory.TryGetValue(category, out cList);
			List<ToolboxItem> list = new List<ToolboxItem>();
			foreach(ToolboxItem item in hList)
				if(cList.Contains(item))
					list.Add(item);
			return new ToolboxItemCollection(toolboxItems.ToArray());
		}
		public ToolboxItemCollection GetToolboxItems(IDesignerHost host) {
			List<ToolboxItem> items = null;
			if(!toolboxByHost.TryGetValue(host, out items))
				items = new List<ToolboxItem>(0);
			return new ToolboxItemCollection(items.ToArray());
		}
		string GetPointerImagePath(Type attribyteType) {
			string size = attribyteType == typeof(ToolboxBitmap24Attribute) ? "24x24" : "32x32";
			return "Images.Toolbox" + size + ".Pointer.png";
		}
		Image GetImageByAttribute(Type type, Type attribyteType) {
			ToolboxBitmapAttributeBase attr = TypeDescriptor.GetAttributes(type)[attribyteType] as ToolboxBitmapAttributeBase;
			return attr == null ? null : (Bitmap)ImageTool.ImageFromStream(attr.GetStream());
		}
		public Image GetImageBase(Type type, Dictionary<Type, Image> dictionary, Type attributeType) {
			Image image = null;
			if(!dictionary.TryGetValue(type, out image)){
				if(typeof(object) == type) {
					image = ResourceImageHelper.CreateBitmapFromResources(GetPointerImagePath(attributeType), typeof(LocalResFinder));
				} else {
					image = GetImageByAttribute(type, attributeType);
				}
				if(image != null) {
					dictionary[type] = image;
				}
			}
			return image;
		}
		public Image GetImageByType(Type type) {
			return GetImageBase(type, typeToImage, typeof(ToolboxBitmap24Attribute));
		}
		public Image GetLargeImage(ToolboxItem item) {
			return GetLargeImage(GetType(item));
		}
		public Image GetLargeImage(Type type) {
			Image image = GetImageBase(type, typeToImage, typeof(ToolboxBitmap32Attribute));
			if(image == null) {
				try {
					image = DesignImageHelper.Get256ColorBitmap(type);
				} catch {
					image = GetImageByType(type);
				}
			}
			AddToolBoxLargeImage(type, image);
			return image;
		}
		public Image GetSmallImage(Type type) {
			Image image;
			try {
				image = ResourceImageHelper.CreateBitmapFromResources("Images.Pointer.png", typeof(LocalResFinder));
			} catch {
				image = new Bitmap(16, 16);
			}
			return image;
		}
		public Image GetSmallImage(ToolboxItem item) {
			Image image;
			try {
				image = item.Bitmap;
			} catch {
				image = new Bitmap(16, 16);
			}
			return image;
		}
		public bool IsSupported(object serializedObject, ICollection filterAttributes) {
			return false;
		}
		public bool IsSupported(object serializedObject, IDesignerHost host) {
			return false;
		}
		public bool IsToolboxItem(object serializedObject) {
			return GetDataFormat(serializedObject as System.Windows.Forms.IDataObject) != null;
		}
		static Type GetDataFormat(System.Windows.Forms.IDataObject dataObject) {
			if(dataObject != null) {
				foreach(Type format in formats)
					if(dataObject.GetDataPresent(format))
						return format;
			}
			return null;
		}
		public bool IsToolboxItem(object serializedObject, IDesignerHost host) {
			return DeserializeToolboxItem(serializedObject, host) != null;
		}
		public void Refresh() {
		}
		public void RemoveCreator(string format) {
		}
		public void RemoveCreator(string format, IDesignerHost host) {
		}
		public void RemoveToolboxItem(ToolboxItem toolboxItem) {
			RemoveToolboxItem(toolboxItem, String.Empty);
		}
		public void RemoveToolboxItem(ToolboxItem toolboxItem, string category) {
			if(String.IsNullOrEmpty(category)) {
				CategoryNameCollection nameCollection = CategoryNames;
				foreach(string name in nameCollection)
					if(GetToolboxItems(name).Contains(toolboxItem)) {
						category = name;
						break;
					}
			}
			if(!toolboxByCategory.ContainsKey(category) || !toolboxByCategory[category].Contains(toolboxItem))
				return;
			toolboxItems.Remove(toolboxItem);
			toolboxByCategory[category].Remove(toolboxItem);
			if(toolboxByCategory[category].Count == 0)
				toolboxByCategory.Remove(category);
			OnToolboxItemRemoved(toolboxItem, category, null);
		}
		public void SelectedToolboxItemUsed() {
			OnSelectedItemUsed();
		}
		public object SerializeToolboxItem(ToolboxItem toolboxItem) {
			return null;
		}
		public bool SetCursor() {
			if(selectedItem == null || selectedItem.DisplayName == ReportLocalizer.GetString(ReportStringId.UD_XtraReportsPointerItemCaption)) {
				return false;
			}
			if(!Comparer.Equals(Cursor.Current, toolboxItemCursor) || selectedItem.DisplayName != toolboxItemName) {
				if(toolboxItemCursor != null) {
					toolboxItemCursor.Dispose();
				}
				toolboxItemCursor = CreateCrossCursorWithBitmap(selectedItem.Bitmap);
				toolboxItemName = selectedItem.DisplayName;
				Cursor.Current = toolboxItemCursor;
			}
			return true;
		}
		static Cursor CreateCrossCursorWithBitmap(Bitmap toolBoxItemBitmap) {
			IconInfo iconInfo = new IconInfo();
			try {
				const int crossLineLength = 12;
				Point hotSpot = new Point(crossLineLength / 2 - 1, crossLineLength / 2 - 1);
				Bitmap bitmap = new Bitmap(toolBoxItemBitmap.Size.Width + crossLineLength, toolBoxItemBitmap.Size.Height + crossLineLength);
				for(int i = 0; i < crossLineLength - 1; i++) {
					bitmap.SetPixel(hotSpot.X, i, Color.Black);
					bitmap.SetPixel(i, hotSpot.Y, Color.Black);
				}
				for(int i = 0; i <= toolBoxItemBitmap.Size.Width - 1; i++) {
					for(int j = 0; j <= toolBoxItemBitmap.Size.Height - 1; j++) {
						bitmap.SetPixel(i + crossLineLength, j + crossLineLength, toolBoxItemBitmap.GetPixel(i, j));
					}
				}
				GetIconInfo(bitmap.GetHicon(), ref iconInfo);
				iconInfo.xHotspot = hotSpot.X;
				iconInfo.yHotspot = hotSpot.Y;
				iconInfo.fIcon = false;
				return new Cursor(CreateIconIndirect(ref iconInfo));
			} finally {
				if(iconInfo.hbmColor != IntPtr.Zero) DeleteObject(iconInfo.hbmColor);
				if(iconInfo.hbmMask != IntPtr.Zero) DeleteObject(iconInfo.hbmMask);
			}
		}
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);
		[DllImport("user32.dll")]
		static extern IntPtr CreateIconIndirect(ref IconInfo icon);
		[DllImport("gdi32.dll")]
		static extern IntPtr DeleteObject(IntPtr hDc);
		public void SetSelectedToolboxItem(ToolboxItem toolboxItem) {
			if(toolboxItem != selectedItem) {
				OnSelectedItemChanging();
				selectedItem = toolboxItem;
				OnSelectedItemChanged();
			}
		}
		protected virtual void OnSelectedCategoryChanging() {
			if(SelectedCategoryChanging != null)
				SelectedCategoryChanging(this, EventArgs.Empty);
		}
		protected virtual void OnSelectedItemChanged() {
			if(SelectedItemChanged != null)
				SelectedItemChanged(this, EventArgs.Empty);
		}
		protected virtual void OnSelectedItemChanging() {
			if(SelectedCategoryChanging != null)
				SelectedItemChanging(this, EventArgs.Empty);
		}
		protected virtual void OnSelectedCategoryChanged() {
			if(SelectedCategoryChanged != null)
				SelectedCategoryChanged(this, EventArgs.Empty);
		}
		protected virtual void OnSelectedItemUsed() {
			if(SelectedItemUsed != null)
				SelectedItemUsed(this, EventArgs.Empty);
		}
		protected virtual void OnToolboxItemAdded(ToolboxItem item, string category, IDesignerHost host) {
			if(ToolboxItemAdded != null)
				ToolboxItemAdded(this, new ToolboxEventArgs(item, category, host));
		}
		protected virtual void OnToolboxItemRemoved(ToolboxItem item, string category, IDesignerHost host) {
			if(ToolboxItemRemoved != null)
				ToolboxItemRemoved(this, new ToolboxEventArgs(item, category, host));
		}
		public event EventHandler SelectedCategoryChanging;
		public event EventHandler SelectedCategoryChanged;
		public event EventHandler SelectedItemChanging;
		public event EventHandler SelectedItemChanged;
		public event EventHandler SelectedItemUsed;
		public event ToolboxEventHandler ToolboxItemAdded;
		public event ToolboxEventHandler ToolboxItemRemoved;
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		void Dispose(bool disposing) {
			if(!this.disposed) {
				if(disposing) {
					toolboxItemCursor.Dispose();
				}
				this.disposed = true;
			}
		}
	}
	public class XRTypeDescriptorFilterService : ITypeDescriptorFilterService {
		#region static
		static IDesignerFilter GetDesignerFilter(IComponent component) {
			ISite site = component.Site;
			if(site == null) {
				return null;
			}
			IDesignerHost host = (IDesignerHost)site.GetService(typeof(IDesignerHost));
			return host.GetDesigner(component) as IDesignerFilter;
		}
		#endregion
		#region System.ComponentModel.Design.ITypeDescriptorFilterService interface implementation
		public bool FilterProperties(System.ComponentModel.IComponent component, System.Collections.IDictionary properties) {
			IDesignerFilter designerFilter = GetDesignerFilter(component);
			if(designerFilter != null) {
				designerFilter.PreFilterProperties(properties);
				designerFilter.PostFilterProperties(properties);
			}
			return true;
		}
		public bool FilterEvents(System.ComponentModel.IComponent component, System.Collections.IDictionary events) {
			IDesignerFilter designerFilter = GetDesignerFilter(component);
			if(designerFilter != null) {
				designerFilter.PreFilterEvents(events);
				designerFilter.PostFilterEvents(events);
			}
			return true;
		}
		public bool FilterAttributes(System.ComponentModel.IComponent component, System.Collections.IDictionary attributes) {
			IDesignerFilter designerFilter = GetDesignerFilter(component);
			if(designerFilter != null) {
				designerFilter.PreFilterAttributes(attributes);
				designerFilter.PostFilterAttributes(attributes);
			}
			return true;
		}
		#endregion
	}
	public class UIService : IUIService, IDisposable {
		Control control;
		IDictionary styles = new Hashtable();
		IServiceProvider servProvider;
		Form RootForm { get { return FindNonMdiClientForm(control); } }
		public UIService(Control control, IServiceProvider servProvider) {
			this.control = control;
			this.servProvider = servProvider;
		}
		public DialogResult ShowDialog(Form form) {
			return form.ShowDialog(RootForm);
		}
		public IWin32Window GetDialogOwnerWindow() {
			return RootForm;
		}
		public bool CanShowComponentEditor(object component) {
			return false;
		}
		public void SetUIDirty() {
		}
		public bool ShowComponentEditor(object component, IWin32Window parent) {
			throw new NotSupportedException();
		}
		public void ShowError(Exception ex, string message) {
			ShowError(new Exception(message, ex));
		}
		public void ShowError(Exception ex) {
			DevExpress.XtraPrinting.Tracer.TraceError(NativeSR.TraceSource, ex);
			NotificationService.ShowException<XtraReport>(LookAndFeelProviderHelper.GetLookAndFeel(servProvider), RootForm, ex);
		}
		public void ShowError(string message) {
			ShowError(new Exception(message));
		}
		public DialogResult ShowMessage(string message, string caption, MessageBoxButtons buttons) {
			return NotificationService.ShowMessage<XtraReport>(LookAndFeelProviderHelper.GetLookAndFeel(servProvider), RootForm, message, caption, buttons, MessageBoxIcon.None);
		}
		public void ShowMessage(string message, string caption) {
			ShowMessage(message, caption, MessageBoxButtons.OK);
		}
		public void ShowMessage(string message) {
			ShowMessage(message, string.Empty);
		}
		public bool ShowToolWindow(Guid toolWindow) {
			throw new NotSupportedException();
		}
		public IDictionary Styles {
			get { return styles; }
		}
		public void Dispose() {
			control = null;
		}
		static Form FindNonMdiClientForm(Control control) {
			Form form = control.FindForm();
			if(form != null)
				return form.MdiParent ?? form;
			return null;
		}
	}
}
