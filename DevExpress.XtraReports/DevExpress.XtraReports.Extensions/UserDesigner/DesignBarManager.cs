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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.XtraBars;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UserDesigner.Native;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils;
using DevExpress.XtraReports.Design;
using DevExpress.XtraBars.Utils;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Design.Commands;
namespace DevExpress.XtraReports.UserDesigner {
	[
#if !DEBUG
#endif // DEBUG
Designer("DevExpress.XtraReports.Design.XRDesignBarManagerDesigner, " + AssemblyInfo.SRAssemblyReportsDesignFull, typeof(IDesigner)),
	ToolboxBitmap(typeof(LocalResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "XRDesignBarManager.bmp"),
	Description("Allows you to embed predefined bars when creating an End-User Report Designer."),
	ToolboxItem(false),
	ProvideProperty("ToolboxType", typeof(Bar)),
	]
	public class XRDesignBarManager : DevExpress.XtraBars.BarManager, IDesignControl, IExtenderProvider, IDesignPanelListener, IWeakServiceProvider {
		DevExpress.Utils.ImageCollection imageCollection;
		Bar formattingToolbar;
		Bar layoutToolbar;
		Bar toolbar;
		XRDesignBarMangerLogic designItemsLogic;
		StringCollection updates = new StringCollection();
		Dictionary<Bar, ToolboxType> toolboxTypeDictionary = new Dictionary<Bar, ToolboxType>();
		BarInfoCollection barInfos = new BarInfoCollection();
		#region ExtenderProvider
		[
		Browsable(false), 
		EditorBrowsable(EditorBrowsableState.Never), 
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public BarInfoCollection BarInfos {
			get {
				if(!this.IsLoading)
					SynchBarInfos();
				return barInfos;
			}
		}
		void SynchBarInfos() {
			barInfos.Clear();
			foreach(Bar bar in Bars) { 
				ToolboxType toolboxType = GetToolboxType(bar);
				if(toolboxType != ToolboxType.None)
					barInfos.Add(new BarInfo(bar, toolboxType));
			}
		}
		bool IExtenderProvider.CanExtend(object extendee) {
			Bar bar = extendee as Bar;
			return bar != null && bar.GetType() == typeof(Bar) && this.Bars.Contains(bar);
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		SRCategory(DevExpress.XtraReports.Localization.ReportStringId.CatUserDesigner),
		DefaultValue(ToolboxType.None),
		DisplayName("ToolboxType")
		]
		public ToolboxType GetToolboxType(Bar bar) {
			ToolboxType result;
			return toolboxTypeDictionary.TryGetValue(bar, out result) ? result : ToolboxType.None;
		}
		public void SetToolboxType(Bar bar, ToolboxType value) {
			if(bar == null)
				return;
			toolboxTypeDictionary[bar] = value;
		}
		internal static void ApplyCategory(Bar bar, string category) {
			bar.Text = category;
		}
		#endregion
		#region properties
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public StringCollection Updates {
			get {
				return updates;
			}
		}
		[
#if !SL
	DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignBarManagerFontNameEdit"),
#endif
		Browsable(false),
		]
		public XtraBars.BarEditItem FontNameEdit {
			get { return designItemsLogic.FontNameEdit; }
			set { designItemsLogic.FontNameEdit = value; }
		}
		[Browsable(false),]
		public RepositoryItemComboBox FontNameBox {
			get { return designItemsLogic.FontNameBox; } 
			set { designItemsLogic.FontNameBox = value; } 
		}
		[Browsable(false),]
		public XtraBars.BarEditItem FontSizeEdit {
			get { return designItemsLogic.FontSizeEdit; }
			set { designItemsLogic.FontSizeEdit = value; }
		}
		[Browsable(false),]
		public RepositoryItemComboBox FontSizeBox {
			get { return designItemsLogic.FontSizeBox; }
			set { designItemsLogic.FontSizeBox = value; }
		}
		[Browsable(false),]
		public Bar FormattingToolbar { get{ return this.formattingToolbar; } set {this.formattingToolbar = value;}
		}
		[Browsable(false),]
		public Bar LayoutToolbar { get{ return this.layoutToolbar; } set {this.layoutToolbar = value;}
		}
		[Browsable(false),]
		public Bar Toolbar { get{ return this.toolbar; } set {this.toolbar = value;}
		}
		[Browsable(false),]
		public BarStaticItem HintStaticItem {
			get { return designItemsLogic.HintStaticItem; }
			set { designItemsLogic.HintStaticItem = value; }
		}
		[
#if !SL
	DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignBarManagerImages"),
#endif
		SRCategory(DevExpress.XtraReports.Localization.ReportStringId.CatUserDesigner),
		DefaultValue(null),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public new DevExpress.Utils.Images Images {
			get { return (base.Images as DevExpress.Utils.ImageCollection).Images; }
		}
		[
		DefaultValue(null), 
		Browsable(false), 
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public DevExpress.Utils.ImageCollectionStreamer ImageStream {
			get { return (base.Images as DevExpress.Utils.ImageCollection).ImageStream;	}
			set { (base.Images as DevExpress.Utils.ImageCollection).ImageStream = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public new object LargeImages { get { return base.LargeImages; } set { base.LargeImages  = value;} 
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public XRZoomBarEditItem ZoomItem {
			get { return designItemsLogic.ZoomItem; }
			set { designItemsLogic.ZoomItem = value; }
		}
		[
#if !SL
	DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignBarManagerXRDesignPanel"),
#endif
		SRCategory(DevExpress.XtraReports.Localization.ReportStringId.CatUserDesigner),
		DefaultValue(null),
		]
		public XRDesignPanel XRDesignPanel {
			get {
				if(designItemsLogic != null)
					return designItemsLogic.XRDesignPanel;
				return null;
			}
			set { designItemsLogic.XRDesignPanel = value; }
		}
		internal DevExpress.Utils.ImageCollection ImageCollection {
			get { return imageCollection; }
		}
		#endregion
		public XRDesignBarManager(): base() {
			InitializeComponent();
		}
		public XRDesignBarManager(IContainer container) : base(container) {
			InitializeComponent();
		}
		void InitializeComponent() {
			CreateImageList();
			designItemsLogic = new XRDesignBarMangerLogic(this, this);
		}
		public override void EndInit() {
			foreach(BarInfo item in barInfos) {
				SetToolboxType(item.Bar, item.ToolboxType);
				if(string.IsNullOrEmpty(item.BarName))
					continue;
				string category;
				if(ToolBoxBarsConfigurator.TryGetCategory(item.BarName, out category))
					ApplyCategory(item.Bar, category);
			}
			designItemsLogic.EndInit();
			base.EndInit();
		}
		public BarItem[] GetBarItemsByReportCommand(ReportCommand command) {
			return designItemsLogic.GetBarItemsByReportCommand(command);
		}
		#region obsolete
		[
		Obsolete("The RegisterCommandHandler method is obsolete now. Use the XRDesignMdiController.AddCommandHandler method instead."),
		]
		public void RegisterCommandHandler(ICommandHandler handler) {
		}
		[
		Obsolete("The UnregisterCommandHandler method is obsolete now. Use the XRDesignMdiController.RemoveCommandHandler method instead."),
		]
		public void UnregisterCommandHandler(ICommandHandler handler) {
		}
		[
		Obsolete("The GetBarItemByCommand method is obsolete now. Use the GetBarItemsByReportCommand method instead."),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public CommandBarItem GetBarItemByCommand(CommandID commandID) {
			return null;
		}
		[
		Obsolete("The SetCommandVisibility method is obsolete now. Use the XRDesignMdiController.SetCommandVisibility method instead."),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public void SetCommandVisibility(CommandID commandID, BarItemVisibility visibility) {
		}
		[
		Obsolete("The SetCommandVisibility method is obsolete now. Use the XRDesignMdiController.SetCommandVisibility method instead."),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public void SetCommandVisibility(CommandID[] commands, BarItemVisibility visibility) {
		}
		#endregion
		public virtual void Initialize(XRDesignPanel designPanel) {
			ClearBarsAndItems();
			designItemsLogic.ClearContent();
			XRDesignPanel = designPanel;
			SetDefaultBars();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(imageCollection != null) {
					imageCollection.Dispose();
					imageCollection = null;
				}
				if(designItemsLogic != null) {
					designItemsLogic.Dispose();
					designItemsLogic = null;
				}
			}
			base.Dispose(disposing);
		}
		protected virtual void SetDefaultBars() {
			BarManagerConfigurator.Configure(
				new DesignBarManagerConfigurator(this),
				new SaveAllBarItemDesignBarManagerConfigurator(this),
				new MdiDesignBarManagerConfigurator(this),
				new CloseBarItemDesignBarManagerConfigurator(this),
				new ZoomDesignBarManagerConfigurator(this)
			);
		}
		protected override void RestoreLayoutCore(XtraSerializer serializer, object path) {
			Dictionary<Bar, TypeDescriptionProvider> providers = GetToolboxBarProviders();
			if(providers.Count > 0) {
				foreach(KeyValuePair<Bar, TypeDescriptionProvider> item in providers)
					TypeDescriptor.AddProvider(item.Value, item.Key);
				RestoreLayoutCore2(serializer, path);
				foreach(KeyValuePair<Bar, TypeDescriptionProvider> item in providers)
					TypeDescriptor.RemoveProvider(item.Value, item.Key);
			} else
				RestoreLayoutCore2(serializer, path);
		}
		void RestoreLayoutCore2(XtraSerializer serializer, object path) {
			bool previousLayoutActual = !(serializer is RegistryXtraSerializer) || BarManagerConfigurator.CheckLayoutVersion(DesignBarManagerConfigurator.LayoutVersionKey, DesignBarManagerConfigurator.LayoutVersionName, DesignBarManagerConfigurator.LayoutVersionValue);
			if(previousLayoutActual)
				base.RestoreLayoutCore(serializer, path);
		}
		protected override bool SaveLayoutCore(XtraSerializer serializer, object path) {
			Dictionary<Bar, TypeDescriptionProvider> providers = GetToolboxBarProviders();
			if(providers.Count > 0) {
				foreach(KeyValuePair<Bar, TypeDescriptionProvider> item in providers)
					TypeDescriptor.AddProvider(item.Value, item.Key);
				try {
					return SaveLayoutCore2(serializer, path);
				} finally {
					foreach(KeyValuePair<Bar, TypeDescriptionProvider> item in providers)
						TypeDescriptor.RemoveProvider(item.Value, item.Key);
				}
			} else
				return SaveLayoutCore2(serializer, path);
		}
		Dictionary<Bar, TypeDescriptionProvider> GetToolboxBarProviders() {
			Dictionary<Bar, TypeDescriptionProvider> result = new Dictionary<Bar, TypeDescriptionProvider>();
			foreach(Bar bar in Bars)
				if(bar.BarName.StartsWith(XRDesignBarManagerBarNames.Toolbox))
					result.Add(bar, new NonserializedBarProvider(bar));
			return result;
		}
		bool SaveLayoutCore2(XtraSerializer serializer, object path) {
			if(serializer is RegistryXtraSerializer)
				BarManagerConfigurator.SaveLayoutVersion(DesignBarManagerConfigurator.LayoutVersionKey, DesignBarManagerConfigurator.LayoutVersionName, DesignBarManagerConfigurator.LayoutVersionValue);
			this.designItemsLogic.ClearToolboxBarItems();
			return base.SaveLayoutCore(serializer, path);
		}
		protected override void FillAdditionalBarItemInfoCollection(DevExpress.XtraBars.Styles.BarItemInfoCollection coll) {
			base.FillAdditionalBarItemInfoCollection(coll);
			coll.Add(new DevExpress.XtraBars.Styles.BarItemInfo("CommandBarItem", "CommandButton", -1, typeof(CommandBarItem), typeof(BarButtonItemLink), typeof(DevExpress.XtraBars.ViewInfo.BarButtonLinkViewInfo), PaintStyle.CreateButtonItemLinkPainter(), true, false));
		}
		protected override BarManagerHelpers CreateHelpers() {
			return new XRBarManagerHelpers(this);
		}
		void ClearBarsAndItems() {
			Bars.Clear();
			Items.Clear();
			RepositoryItems.Clear();
		}
		private void CreateImageList() {
			imageCollection = ImageCollectionHelper.CreateVoidImageCollection();
			base.Images = imageCollection;
			imageCollection.Images.AddRange(XRBitmaps.GetFormattingToolBarIcons().Images);
			imageCollection.Images.AddRange(XRBitmaps.GetMainToolBarIcons().Images);
			imageCollection.Images.AddRange(XRBitmaps.GetLayoutToolBarIcons().Images);
		}
		internal object XtraFindBarsItem(XtraItemEventArgs e) {
			DevExpress.Utils.Serializing.Helpers.XtraPropertyInfo pi = e.Item.ChildProperties["BarName"];
			if(pi == null) pi = e.Item.ChildProperties["Name"];
			if(pi == null) return null;
			Bar bar = null;
			if(pi.Value != null) bar = Bars[pi.Value.ToString()]; else return null;
			if(bar == null) {
				return new Bar(this, pi.Value.ToString());
			}
			return bar;
		}
		IServiceProvider serviceProvider;
		IServiceProvider IWeakServiceProvider.ServiceProvider {
			get { return serviceProvider; }
			set { 
				serviceProvider = value;
				if(designItemsLogic != null && this.Site == null)
					this.designItemsLogic.UpdateBarItems();
			}
		}
		object IServiceProvider.GetService(Type serviceType) {
			return serviceProvider != null ? serviceProvider.GetService(serviceType) : null;
		}
	}
	public enum ToolboxType { None, Standard, Custom }
	public class BarInfo {
		Bar bar;
		ToolboxType toolboxType;
		public Bar Bar { get { return bar; } set { bar = value; } }
		public ToolboxType ToolboxType { get { return toolboxType; } set { toolboxType = value; } }
		internal string BarName { get { return bar != null ? bar.BarName : string.Empty; } }
		public BarInfo() { 
		}
		public BarInfo(Bar bar, ToolboxType toolboxType) {
			this.bar = bar;
			this.toolboxType = toolboxType;
		}
	}
	public class BarInfoCollection : Collection<BarInfo> {
		public void AddRange(BarInfo[] items) {
			foreach(BarInfo item in items)
				this.Add(item);
		}
	}
	public class DesignBar : Bar {
		[XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public override string Text { get { return base.Text; } set { base.Text = value; }
		}
	}
	internal class XRBarManagerHelpers : BarManagerHelpers {
		public XRBarManagerHelpers(BarManager manager)
			: base(manager) {
		}
		protected override BarLinkDragManager CreateLinkDragManager() {
			return new XRBarLinkDragManager(Manager);
		}
	}
	internal class XRBarLinkDragManager : BarLinkDragManager {
		public XRBarLinkDragManager(BarManager manager)
			: base(manager) {
			UseDefaultCursors = true;
		}
	}
}
