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
using System.Windows.Forms;
using DevExpress.XtraBars;
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.XtraPrinting.Preview.Native;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Control;
using System.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraTab;
using DevExpress.XtraEditors.Controls;
using System.Reflection;
using System.Collections;
using DevExpress.Utils.Serializing;
using DevExpress.Utils;
namespace DevExpress.XtraPrinting.Preview
{
	[
	Designer("DevExpress.XtraPrinting.Design.PrintBarManagerDesigner, " + AssemblyInfo.SRAssemblyPrintingDesign, typeof(IDesigner)),
	ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "PrintBarManager.bmp"),
	Description("Allows you to create bars within a Print Preview form."),
	ToolboxItem(false),
	]
	public class PrintBarManager : DevExpress.XtraBars.BarManager, DevExpress.XtraPrinting.Control.IPrintPreviewControl {
		Bar previewBar;
		DevExpress.Utils.ImageCollection imageCollection;
		PreviewItemsLogicBase printItemsLogic;
		const int multiplePagesIndex = 0;
		const int colorPopupIndex = 1;
		const int scaleIndex = 2;
		const int lastIndex = 2;
		PopupControlContainer[] popupControlContainers = new PopupControlContainer[lastIndex+1] { null, null, null};
		#region properties
		internal DevExpress.Utils.ImageCollection ImageCollection {
			get { return base.Images as DevExpress.Utils.ImageCollection; }
		}
		[
#if !SL
	DevExpressXtraPrintingLocalizedDescription("PrintBarManagerPreviewBar"),
#endif
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public Bar PreviewBar{ get { return previewBar; } set { previewBar = value; }
		}
		[
#if !SL
	DevExpressXtraPrintingLocalizedDescription("PrintBarManagerImages"),
#endif
		Category(NativeSR.CatPrinting),
		DefaultValue(null), 
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public new DevExpress.Utils.Images Images { 
			get {
				return (base.Images as DevExpress.Utils.ImageCollection).Images;
			} 
		}
		[
		DefaultValue(null), 
		Browsable(false), 
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public DevExpress.Utils.ImageCollectionStreamer ImageStream {
			get {
				return (base.Images as DevExpress.Utils.ImageCollection).ImageStream;
			}
			set {
				(base.Images as DevExpress.Utils.ImageCollection).ImageStream = value;
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public new object LargeImages { get { return base.LargeImages; } set { base.LargeImages = value; }
		} 
		[
#if !SL
	DevExpressXtraPrintingLocalizedDescription("PrintBarManagerPrintControl"),
#endif
		Category(NativeSR.CatPrinting),
		DefaultValue(null),
		]
		public virtual PrintControl PrintControl { 
			get { return printItemsLogic.PrintControl; } 
			set {
				printItemsLogic.PrintControl = value;
			} 
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Obsolete("This property is now obsolete and isn't used at all."),
		]
		public MultiplePagesControlContainer MultiplePagesControlContainer {
			get { return (MultiplePagesControlContainer)popupControlContainers[multiplePagesIndex]; }
			set { popupControlContainers[multiplePagesIndex] = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Obsolete("This property is now obsolete and isn't used at all."),
		]
		public ColorPopupControlContainer ColorPopupControlContainer {
			get { return (ColorPopupControlContainer)popupControlContainers[colorPopupIndex]; }
			set { popupControlContainers[colorPopupIndex] = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Obsolete("This property is now obsolete and isn't used at all."),
		]
		public ScaleControlContainer ScaleControlContainer {
			get { return (ScaleControlContainer)popupControlContainers[scaleIndex]; }
			set { popupControlContainers[scaleIndex] = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Obsolete("This property is now obsolete and isn't used at all."),
		]
		public ZoomBarEditItem ZoomItem { get { return printItemsLogic.ZoomItem; } set { } }
#if DEBUGTEST
		internal PrintPreviewStaticItem PanelZoomFactor { get { return printItemsLogic.PanelZoomFactor; } }
#endif
		#endregion properties
		public PrintBarManager() {
			InitializeComponent();
		}
		public PrintBarManager(IContainer container)
			: base(container) {
			InitializeComponent();
		}
		void InitializeComponent() {
			imageCollection = ImageCollectionHelper.CreateImageCollectionFromResources("DevExpress.XtraPrinting.Images.PrintPreviewBar.png", System.Reflection.Assembly.GetExecutingAssembly());
			base.Images = imageCollection;
			printItemsLogic = new PrintPreviewBarItemsLogic(this);
		}
		public void Initialize(PrintControl printControl) {
			ClearBarsAndItems();
			PrintControl = printControl;
			SetDefaultBars();
		}
		internal void SetBarItemLocked(BarItem item, bool locked) {
			printItemsLogic.SetItemLocked(item, locked);
		}
		public BarItem GetBarItemByCommand(PrintingSystemCommand command) {
			return printItemsLogic.GetBarItemByCommand(command);
		}
		internal void EnableCommand(PrintingSystemCommand command, bool enabled) {
			printItemsLogic.CommandSet.EnableCommand(enabled, command);
			printItemsLogic.UpdateCommands();
		}
		public void UpdateCommands() {
			printItemsLogic.UpdateCommands();
		}
		void SetDefaultBars() {
			PrintBarManagerConfigurator configurator = new PrintBarManagerConfigurator(this);
			configurator.Configure();
			UpdateCommands();
			printItemsLogic.UpdatePreviewStatusPanels();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(imageCollection != null) {
					imageCollection.Dispose();
					imageCollection = null;
				}
				previewBar = null;
				printItemsLogic.Dispose();
			}
			base.Dispose(disposing);
		}
		protected override void RestoreLayoutCore(XtraSerializer serializer, object path) {
			bool previousLayoutActual = !(serializer is RegistryXtraSerializer) || BarManagerConfigurator.CheckLayoutVersion(PrintBarManagerConfigurator.LayoutVersionKey, PrintBarManagerConfigurator.LayoutVersionName, PrintBarManagerConfigurator.LayoutVersionValue);
			if(previousLayoutActual)
				base.RestoreLayoutCore(serializer, path);
			UpdateCommands();
		}
		protected override bool SaveLayoutCore(XtraSerializer serializer, object path) {
			if(serializer is RegistryXtraSerializer)
				BarManagerConfigurator.SaveLayoutVersion(PrintBarManagerConfigurator.LayoutVersionKey, PrintBarManagerConfigurator.LayoutVersionName, PrintBarManagerConfigurator.LayoutVersionValue);
			return base.SaveLayoutCore(serializer, path);
		}
		void DisposeSerializedControlContainers() {
			for(int i = 0; i < popupControlContainers.Length; i++) {
				PopupControlContainer container = popupControlContainers[i];
				if(container != null) {
					container.Dispose();
					container = null;
				}
			}
		}
		void ClearBarsAndItems() {
			Bars.Clear();
			Items.Clear();
		}
		protected override void OnEndInit() {
			base.OnEndInit();
			DisposeSerializedControlContainers();
			printItemsLogic.EndInit();
		}
	}
}
