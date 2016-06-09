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
using System.Text;
using System.Drawing;
using System.ComponentModel;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraPrinting.Control;
using DevExpress.XtraBars;
using DevExpress.XtraPrinting.Preview.Native;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Localization;
using DevExpress.Utils;
using System.Drawing.Printing;
namespace DevExpress.XtraPrinting.Preview {
	public class PrintPreviewRibbonPage : RibbonPage, ISupportContextSpecifier {
		#region ISupportContextSpecifier
		object contextSpecifier;
		bool ISupportContextSpecifier.HasSameContext(object contextSpecifier) {
			return this.ContextSpecifier == null || this.ContextSpecifier == contextSpecifier;
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue(null)]
		public object ContextSpecifier {
			get { return contextSpecifier; }
			set { contextSpecifier = value; }
		}
		#endregion
		public IEnumerable<PrintPreviewRibbonPageGroup> PreviewRibbonPageGroups {
			get {
				return ContextHelper.GetSameContextEnumerable<PrintPreviewRibbonPageGroup>(this.Groups, this.ContextSpecifier);
			}
		}
		public PrintPreviewRibbonPage() {
		}
		protected override RibbonPage CreatePage() {
			return new PrintPreviewRibbonPage();
		}
	}
	public enum PrintPreviewRibbonPageGroupKind { 
		Print,
		PageSetup,
		Navigation,
		Zoom,
		Background,
		Export,
		Document,
		Close,
	}
	public class PrintPreviewRibbonPageGroup : RibbonPageGroup, ControllerRibbonPageGroupKind<PrintPreviewRibbonPageGroupKind>, ISupportContextSpecifier {
		#region ISupportContextSpecifier
		object contextSpecifier;
		bool ISupportContextSpecifier.HasSameContext(object contextSpecifier) {
			return this.ContextSpecifier == null || this.ContextSpecifier == contextSpecifier;
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue(null)]
		public object ContextSpecifier {
			get { return contextSpecifier; }
			set { contextSpecifier = value; }
		}
		#endregion
		PrintPreviewRibbonPageGroupKind kind;
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public PrintPreviewRibbonPageGroupKind Kind {
			get { return kind; } 
			set { kind = value; } 
		}
	}
	[
	ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "PrintRibbonController.bmp"),
	Designer("DevExpress.XtraPrinting.Design.PrintRibbonControllerDesigner," + AssemblyInfo.SRAssemblyPrintingDesign),
	Description("Allows you to create a Print Preview form using a RibbonControl."),
	ToolboxItem(false),
	]
	public class PrintRibbonController : RibbonControllerBase, IPrintPreviewControl, ISupportContextSpecifier {
		#region ISupportContextSpecifier
		object contextSpecifier;
		bool ISupportContextSpecifier.HasSameContext(object contextSpecifier) {
			return this.ContextSpecifier == null || this.ContextSpecifier == contextSpecifier;
		}
		object ISupportContextSpecifier.ContextSpecifier {
			get { return contextSpecifier; } set { }
		}
		protected object ContextSpecifier {
			get { return contextSpecifier; }
		}
		#endregion
		PrintControl printControl;
		RibbonPreviewItemsLogic printItemsLogic;
		PrintPreviewRibbonPageGroup pageSetupGroup;
		RibbonImageCollection images = new RibbonImageCollection();
		protected RibbonPreviewItemsLogic PrintItemsLogic { get { return printItemsLogic; } }
		PrintPreviewRibbonPageGroup PageSetupGroup {
			get {
				return pageSetupGroup;
			}
			set {
				if(pageSetupGroup == value)
					return;
				if(pageSetupGroup != null)
					pageSetupGroup.CaptionButtonClick -= new RibbonPageGroupEventHandler(OnPageSetupGroupCaptionButtonClick);
				pageSetupGroup = value;
				if(pageSetupGroup != null)
					pageSetupGroup.CaptionButtonClick += new RibbonPageGroupEventHandler(OnPageSetupGroupCaptionButtonClick);
			}
		}
#if !SL
	[DevExpressXtraPrintingLocalizedDescription("PrintRibbonControllerPrintControl")]
#endif
		public virtual PrintControl PrintControl {
			get { 
				return printItemsLogic != null ? printItemsLogic.PrintControl : printControl; 
			}
			set {
				if(printItemsLogic != null) {
					printItemsLogic.PrintControl = value;
					printControl = null;
				} else
					printControl = value;
			}
		}
		RibbonBarItems Items {
			get { return RibbonControl != null ? RibbonControl.Items : null; }
		}
		RibbonPageCollection Pages {
			get { return RibbonControl != null ? RibbonControl.Pages : null; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
#if !SL
	DevExpressXtraPrintingLocalizedDescription("PrintRibbonControllerPreviewRibbonPages")
#else
	Description("")
#endif
		]
		public IEnumerable<PrintPreviewRibbonPage> PreviewRibbonPages {
			get {
				return ContextHelper.GetSameContextEnumerable<PrintPreviewRibbonPage>(Pages, this.ContextSpecifier);
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RibbonImageCollection ImageCollection {
			get { return images; }
		}
		bool ShouldSerializeImageCollection() {
			return ImageCollection.Count > 0;
		}
		void ResetImageCollection() {
			ImageCollection.Clear();
		}
		public PrintRibbonController() {
			this.contextSpecifier = this;
		}
		public PrintRibbonController(IContainer container) {
			Guard.ArgumentNotNull(container, "container");
			container.Add(this);
			this.contextSpecifier = this;
		}
		public PrintRibbonController(object contextSpecifier) {
			this.contextSpecifier = contextSpecifier;
		}
		protected internal override Dictionary<string, Image> GetImagesFromAssembly() {
			return PrintRibbonControllerConfigurator.GetImagesFromAssembly();
		}
		protected internal override void ConfigureRibbonController(RibbonControl ribbonControl, RibbonStatusBar ribbonStatusBar, Dictionary<string, Image> images) {
			(new PrintRibbonControllerConfigurator(RibbonControl, RibbonStatusBar, images)).Configure(this.ContextSpecifier);
		}
		public BarItem GetBarItemByCommand(PrintingSystemCommand command) {
			return printItemsLogic.GetBarItemByCommand(command);
		}
		public void UpdateCommands() {
			if(printItemsLogic != null)
				printItemsLogic.UpdateCommands();
		}
		protected override void Dispose(bool disposing) {
			try {
				if(disposing) {
					if(printItemsLogic != null) {
						printItemsLogic.Dispose();
						printItemsLogic = null;
					}
					if(pageSetupGroup != null) {
						pageSetupGroup.CaptionButtonClick -= new RibbonPageGroupEventHandler(OnPageSetupGroupCaptionButtonClick);
						pageSetupGroup = null;
					}
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		public override void BeginInit() { }
		public override void EndInit() {
			if(printItemsLogic == null && RibbonControl != null) {
				printItemsLogic = CreateItemsLogic();
				printItemsLogic.ImageCollection = this.ImageCollection;
				printItemsLogic.EndInit();
				PrintRibbonControllerConfigurator.LocalizeStrings(RibbonControl);
				PrintControl = printControl;
			}
			EnsureItemsState();
			PageSetupGroup = GetRibbonPageGroup(PrintPreviewRibbonPageGroupKind.PageSetup);
		}
		void EnsureItemsState() {
			BarButtonItem item = GetBarItemByCommand(DevExpress.XtraPrinting.PrintingSystemCommand.Print) as BarButtonItem;
			if(item != null && item.ButtonStyle == BarButtonStyle.Check)
				item.ButtonStyle = BarButtonStyle.Default;
		}
		protected virtual RibbonPreviewItemsLogic CreateItemsLogic() {
			return new RibbonPreviewItemsLogic(RibbonControl.Manager, this.ContextSpecifier);
		}
		void OnPageSetupGroupCaptionButtonClick(object sender, RibbonPageGroupEventArgs e) {
			printItemsLogic.ExecCommand(PrintingSystemCommand.PageSetup, null);			
		}
		internal PrintPreviewRibbonPageGroup GetRibbonPageGroup(PrintPreviewRibbonPageGroupKind kind) {
			foreach(PrintPreviewRibbonPage page in PreviewRibbonPages) {
				foreach(PrintPreviewRibbonPageGroup group in page.PreviewRibbonPageGroups) {
					if(group.Kind == kind)
						return group;
				}
			}
			return null;
		}
		protected override ICollection<IDisposable> GetObjectsToDispose() { 
			List<IDisposable> objectsToDispose = new List<IDisposable>();
			IEnumerable<IDisposable> items = ContextHelper.GetSameContextEnumerable<IDisposable>(this.Pages, this.ContextSpecifier);
			objectsToDispose.AddRange(items);
			items = ContextHelper.GetSameContextEnumerable<IDisposable>(this.Items, this.ContextSpecifier);
			objectsToDispose.AddRange(items);
			return objectsToDispose;
		}
	}
}
