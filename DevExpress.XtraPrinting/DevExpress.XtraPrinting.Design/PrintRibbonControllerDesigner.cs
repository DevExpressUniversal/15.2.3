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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraPrinting.Preview.Native;
using System.Drawing;
using DevExpress.XtraPrinting.Design.Resources;
namespace DevExpress.XtraPrinting.Design {
	public abstract class RibbonControllerDesignerBase : DevExpress.Utils.Design.BaseComponentDesignerSimple {
		protected static Dictionary<string, Image> CreateImagesResource(Dictionary<string, Image> images, string resourceFileName, IServiceProvider provider) {
			Dictionary<string, Image> resourceImages = images;
			try {
				resourceImages = ImagesResourceHelper.CreateResourcesFile(provider, images, resourceFileName);
			} catch(Exception e) {
				System.Diagnostics.Debug.Assert(false, e.Message);
			}
			return resourceImages;
		}
		protected IDesignerHost Host { get { return (IDesignerHost)GetService(typeof(IDesignerHost)); } }
		protected RibbonControllerBase Controller { get { return (RibbonControllerBase)Component; } }
		protected RibbonControl RibbonControl { get { return DesignHelpers.FindComponent(Host.Container, typeof(RibbonControl)) as RibbonControl; } }
		protected RibbonStatusBar RibbonStatusBar { get { return DesignHelpers.FindComponent(Host.Container, typeof(RibbonStatusBar)) as RibbonStatusBar; } }
		public override DesignerVerbCollection Verbs {
			get {
				DesignerVerbCollection verbs = new DesignerVerbCollection();
				verbs.Add(new DesignerVerb("Update", UpdateVerbEventHandler));
				DevExpress.Utils.Design.DXSmartTagsHelper.CreateDefaultVerbs(this, verbs);
				return verbs;
			}
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			RibbonControl ribbonControl = GetRibbonControl();
			RibbonStatusBar ribbonStatusBar = GetComponent(typeof(RibbonStatusBar)) as RibbonStatusBar;
			System.Diagnostics.Debug.Assert(ribbonControl != null && ribbonStatusBar != null);
			RibbonControllerHelper.Initialize(Controller, ribbonControl, ribbonStatusBar, GetImagesForInitialize());
			AssignMainControl();
			BarsReferencesHelper.AddBarsReferences(this);
		}
		protected Component GetComponent(Type type) {
			Component component = DesignHelpers.FindComponent(Host.Container, type) as Component;
			if(component == null)
				component = DesignHelpers.CreateComponent(Host, type) as Component;
			return component;
		}
		RibbonControl GetRibbonControl() {
			RibbonControl component = DesignHelpers.FindComponent(Host.Container, typeof(RibbonControl)) as RibbonControl;
			if(component == null) {
				component = DesignHelpers.CreateComponent(Host, typeof(RibbonControl)) as RibbonControl;
				ArrayList pagesList = new ArrayList(component.Pages);
				component.Pages.Clear();
				foreach(RibbonPage page in pagesList)
					page.Dispose();
			}
			return component;
		}
		protected abstract Dictionary<string, Image> GetImagesForInitialize();
		protected abstract void AssignMainControl();
		protected abstract void UpdateVerbEventHandler(object sender, EventArgs e);
	}
	public class PrintRibbonControllerDesigner : RibbonControllerDesignerBase {
		#region inner classes
		class SaveOpenPrintRibbonControllerDesignUpdater : PrintRibbonControllerConfigurator {
			static PrintingSystemCommand[] commands = new PrintingSystemCommand[] { PrintingSystemCommand.Save, PrintingSystemCommand.Open };
			public static bool GetUpdateNeeded(RibbonBarManager manager) {
				return PrintBarManagerUpdaterBase.GetUpdateNeeded(manager, commands);
			}
			public SaveOpenPrintRibbonControllerDesignUpdater(RibbonControl ribbonControl, RibbonStatusBar ribbonStatusBar)
				: base(ribbonControl, ribbonStatusBar, CreateImagesResource(CreateImages(commands), ResourceFileName, ribbonControl.Site)) {
			}
			protected override void CreateItems() {
				foreach(RibbonPage page in ribbonControl.Pages) {
					if(page is PrintPreviewRibbonPage && ((ISupportContextSpecifier)page).HasSameContext(this.ContextSpecifier))
						previewPage = page;
				}
				if(previewPage != null) {
					AddBarItem(PrintingSystemCommand.Open);
					AddBarItem(PrintingSystemCommand.Save);
				}
			}
			protected override void CreatePageGroups() {
				CreateDocumentGroup();
			}
		}
		class ParametersPrintRibbonControllerDesignUpdater : PrintRibbonControllerConfigurator {
			static PrintingSystemCommand[] commands = new PrintingSystemCommand[] { PrintingSystemCommand.Parameters };
			public static bool GetUpdateNeeded(RibbonBarManager manager) {
				return PrintBarManagerUpdaterBase.GetUpdateNeeded(manager, commands);
			}
			public ParametersPrintRibbonControllerDesignUpdater(RibbonControl ribbonControl, RibbonStatusBar ribbonStatusBar)
				: base(ribbonControl, ribbonStatusBar, CreateImagesResource(CreateImages(commands), ResourceFileName, ribbonControl.Site)) {
			}
			protected override void CreateItems() {
				foreach(RibbonPage page in ribbonControl.Pages) {
					if(page is PrintPreviewRibbonPage && ((ISupportContextSpecifier)page).HasSameContext(this.ContextSpecifier))
						previewPage = page;
				}
				if(previewPage != null) {
					AddBarItem(PrintingSystemCommand.Parameters);
				}
			}
			protected override void CreatePageGroups() {
				foreach(RibbonPageGroup item in previewPage.Groups) {
					PrintPreviewRibbonPageGroup group = item as PrintPreviewRibbonPageGroup;
					if(group != null && group.Kind == PrintPreviewRibbonPageGroupKind.Print) {
						AddLink(group, PrintingSystemCommand.Parameters);
					}
				}
			}
		}
		class ThumbnailsPrintRibbonControllerDesignUpdater : PrintRibbonControllerConfigurator {
			static PrintingSystemCommand[] commands = new PrintingSystemCommand[] { PrintingSystemCommand.Thumbnails };
			public static bool GetUpdateNeeded(RibbonBarManager manager) {
				return PrintBarManagerUpdaterBase.GetUpdateNeeded(manager, commands);
			}
			public ThumbnailsPrintRibbonControllerDesignUpdater(RibbonControl ribbonControl, RibbonStatusBar ribbonStatusBar)
				: base(ribbonControl, ribbonStatusBar, CreateImagesResource(CreateImages(commands), ResourceFileName, ribbonControl.Site)) {
			}
			protected override void CreateItems() {
				foreach(RibbonPage page in ribbonControl.Pages) {
					if(page is PrintPreviewRibbonPage && ((ISupportContextSpecifier)page).HasSameContext(this.ContextSpecifier))
						previewPage = page;
				}
				if(previewPage != null) {
					AddBarItem(PrintingSystemCommand.Thumbnails);
				}
			}
			protected override void CreatePageGroups() {
				foreach(RibbonPageGroup item in previewPage.Groups) {
					PrintPreviewRibbonPageGroup group = item as PrintPreviewRibbonPageGroup;
					if(group != null && group.Kind == PrintPreviewRibbonPageGroupKind.Navigation) {
						AddLink(group, PrintingSystemCommand.Thumbnails);
					}
				}
			}
		}
		class StatusPanelRibonUpdater : PrintRibbonControllerConfigurator {
			public StatusPanelRibonUpdater(RibbonControl ribbonControl, RibbonStatusBar ribbonStatusBar)
				: base(ribbonControl, ribbonStatusBar, new Dictionary<string, Image>()) {
			}
			protected override void CreateItems() {
				if (ribbonControl.StatusBar == null)
					return;
				StatusPanelUpdater.ClearStatusItems(ribbonControl.StatusBar.ItemLinks);
				ribbonControl.TransparentEditors = true;
				this.AddStatusPanelItems();
			}
			protected override void CreatePageGroups() {
			}
		}
		#endregion
		public const string ResourceFileName = "PrintRibbonControllerResources.resx";
		public static void UpdatePrintRibbonController(PrintRibbonController printRibbonController, RibbonControl ribbonControl, RibbonStatusBar ribbonStatusBar) {
			bool configured = false;
			if(SaveOpenPrintRibbonControllerDesignUpdater.GetUpdateNeeded(ribbonControl.Manager)) {
				configured = true;
				PrintRibbonControllerConfigurator updater = new SaveOpenPrintRibbonControllerDesignUpdater(ribbonControl, ribbonStatusBar);
				updater.Configure(((ISupportContextSpecifier)printRibbonController).ContextSpecifier);
			}
			if(ParametersPrintRibbonControllerDesignUpdater.GetUpdateNeeded(ribbonControl.Manager)) {
				configured = true;
				PrintRibbonControllerConfigurator updater = new ParametersPrintRibbonControllerDesignUpdater(ribbonControl, ribbonStatusBar);
				updater.Configure(((ISupportContextSpecifier)printRibbonController).ContextSpecifier);
			}
			if(ThumbnailsPrintRibbonControllerDesignUpdater.GetUpdateNeeded(ribbonControl.Manager)) {
				configured = true;
				PrintRibbonControllerConfigurator updater = new ThumbnailsPrintRibbonControllerDesignUpdater(ribbonControl, ribbonStatusBar);
				updater.Configure(((ISupportContextSpecifier)printRibbonController).ContextSpecifier);
			}
			if(StatusPanelUpdater.ShouldUpdate(ribbonControl.Manager, new StatusPanelID[] { StatusPanelID.PageOfPages, StatusPanelID.Progress, StatusPanelID.ZoomFactorText })) {
				configured = true;
				PrintRibbonControllerConfigurator updater = new StatusPanelRibonUpdater(ribbonControl, ribbonStatusBar);
				updater.Configure(((ISupportContextSpecifier)printRibbonController).ContextSpecifier);
			}
			if(configured) {
				PrintRibbonControllerConfigurator.LocalizeStrings(ribbonControl);
				printRibbonController.UpdateCommands();
			}
		}
		static Dictionary<string, Image> CreateImages(PrintingSystemCommand[] commands) {
			Dictionary<string, Image> images = new Dictionary<string, Image>();
			foreach(PrintingSystemCommand command in commands) {
				images.Add("RibbonPrintPreview_" + command.ToString(), PrintRibbonControllerConfigurator.GetImageFromResource(command.ToString()));
				images.Add("RibbonPrintPreview_" + command.ToString() + "Large", PrintRibbonControllerConfigurator.GetImageFromResource(command.ToString() + "Large"));
			}
			return images;
		}
		PrintRibbonController PrintRibbonController {
			get { return (PrintRibbonController)this.Component; }
		}
		IEnumerable<PrintPreviewRibbonPageGroup> PreviewPageGroups {
			get {
				foreach(PrintPreviewRibbonPage page in PrintRibbonController.PreviewRibbonPages)
					foreach(PrintPreviewRibbonPageGroup group in page.PreviewRibbonPageGroups)
						yield return (PrintPreviewRibbonPageGroup)group;
			}
		}
		bool NeedUpdateGlyphs() {
			if(!PreviewPageGroups.GetEnumerator().MoveNext())
				return false;
			foreach(PrintPreviewRibbonPageGroup group in PreviewPageGroups)
				if(group.Glyph != null)
					return false;
			return true;
		}
		protected override Dictionary<string, Image> GetImagesForInitialize() {
			return CreateImagesResource(PrintRibbonControllerConfigurator.GetImagesFromAssembly(), ResourceFileName, Component.Site);
		}
		protected override void AssignMainControl() {
			PrintBarManagerDesigner.SetPrintControl(PrintRibbonController, Host);
		}
		protected override void UpdateVerbEventHandler(object sender, EventArgs e) {
			if(NeedUpdateGlyphs()) {
				foreach(PrintPreviewRibbonPageGroup group in PreviewPageGroups) {
					string imageName = PrintRibbonControllerConfigurator.GetGroupImageName((group as PrintPreviewRibbonPageGroup).Kind);
					TypeDescriptor.GetProperties(group)["Glyph"].SetValue(group, ImagesResourceHelper.LoadImageFromResource(Component.Site, ResourceFileName, imageName));
				}
			}
			UpdatePrintRibbonController(PrintRibbonController, RibbonControl, RibbonStatusBar);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				DestroyComponent(typeof(RibbonStatusBar));
				DestroyComponent(typeof(RibbonControl));
			}
			base.Dispose(disposing);
		}
		void DestroyComponent(Type componentType) {
			IComponent component = DevExpress.XtraPrinting.Native.DesignHelpers.FindComponent(Host.Container, componentType) as IComponent;
			if(component != null)
				Host.DestroyComponent(component);
		}
	}
}
