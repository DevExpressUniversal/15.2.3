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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.XtraReports.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Native.Printing;
using DevExpress.XtraPrintingLinks;
using DevExpress.XtraReports.Localization;
using System.Drawing;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils.Serializing;
using DevExpress.XtraReports.Native.Presenters;
using DevExpress.Utils;
using DevExpress.XtraReports.UserDesigner;
namespace DevExpress.XtraReports.UI {
	[
	ToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabReportControls),
	ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "PrintableComponentContainer.bmp"),
	ToolboxItemFilter(DevExpress.XtraReports.Design.AttributeSR.SchedulerToolboxItemFilter, ToolboxItemFilterType.Prevent),
	XRDesigner("DevExpress.XtraReports.Design.PrintableComponentContainerDesigner," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	Designer("DevExpress.XtraReports.Design._PrintableComponentContainerDesigner," + AssemblyInfo.SRAssemblyReportsDesignFull),
	DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.PrintableComponentContainer"),
	XRToolboxSubcategoryAttribute(1, 4),
	]
	public class PrintableComponentContainer : WinControlContainer {
		#region inner classes
		class StyleReferenceComparer : IEqualityComparer<BrickStyle> {
			public bool Equals(BrickStyle x, BrickStyle y) {
				return x == y;
			}
			public int GetHashCode(BrickStyle obj) {
				return obj.GetHashCode();
			}
		}
		#endregion
		#region hidden properties
		[ Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden) ]
		public override WinControlPrintMode PrintMode { get { return base.PrintMode; } set { base.PrintMode = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override WinControlDrawMethod DrawMethod { get { return base.DrawMethod; } set { base.DrawMethod = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override WinControlImageType ImageType { get { return base.ImageType; } set { base.ImageType = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool SyncBounds { get { return false; } set {} }
		#endregion
		object printableComponent;
		IList<IDisposable> container = new List<IDisposable>();
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false)
		]
		public SizeF PrintableContentOffset { get; set; }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		XtraSerializableProperty(XtraSerializationVisibility.Visible),
		EditorBrowsable(EditorBrowsableState.Always),
		Browsable(true),
		]
		public override PaddingInfo Padding {
			get { return base.Padding; }
			set { base.Padding = value; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		XtraSerializableProperty(XtraSerializationVisibility.Visible),
		EditorBrowsable(EditorBrowsableState.Always),
		Browsable(true),
		]
		public override Color BackColor {
			get { return base.BackColor; }
			set { base.BackColor = value; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		XtraSerializableProperty(XtraSerializationVisibility.Visible),
		EditorBrowsable(EditorBrowsableState.Always),
		Browsable(true),
		]
		public override Color BorderColor {
			get { return base.BorderColor; }
			set { base.BorderColor = value; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		XtraSerializableProperty(XtraSerializationVisibility.Visible),
		EditorBrowsable(EditorBrowsableState.Always),
		Browsable(true),
		]
		public override float BorderWidth {
			get { return base.BorderWidth; }
			set { base.BorderWidth = value; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		XtraSerializableProperty(XtraSerializationVisibility.Visible),
		EditorBrowsable(EditorBrowsableState.Always),
		Browsable(true),
		]
		public override BorderDashStyle BorderDashStyle {
			get { return base.BorderDashStyle; }
			set { base.BorderDashStyle = value; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		XtraSerializableProperty(XtraSerializationVisibility.Visible),
		EditorBrowsable(EditorBrowsableState.Always),
		Browsable(true),
		]
		public override BorderSide Borders {
			get { return base.Borders; }
			set { base.Borders = value; }
		}
		[
		SRCategory(ReportStringId.CatBehavior),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.PrintableComponentContainer.ClipContent"),
		DefaultValue(false),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		]
		public bool ClipContent { get; set; }
		[
		SRCategory(ReportStringId.CatBehavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.PrintableComponentContainer.WindowControlOptions"),
		]
		public WindowControlOptions WindowControlOptions {
			get { return options; }
		}
		bool ShouldSerializeWindowControlOptions() {
			return options.ShouldSerialize();
		}
		[Browsable(false), DefaultValue(null)]
		public object PrintableComponent {
			get {
				return printableComponent;
			}
			set {
				if(printableComponent != value) {
					printableComponent = value;
					ClearLink();
					base.WinControl = null;
					RaisePrintableComponentChanged(EventArgs.Empty);
				}
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Control WinControl {
			get {
				if(base.WinControl == null && printableComponent is Control)
					base.WinControl = (Control)printableComponent;
				return base.WinControl;
			}
			set { base.WinControl = value; }
		}
		protected internal override int DefaultWidth {
			get { return DefaultSizes.ComponentContainer.Width; }
		}
		protected internal override int DefaultHeight {
			get { return DefaultSizes.ComponentContainer.Height; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event EventHandler WinControlChanged {
			add { }
			remove { }
		}
		static readonly object PrintableComponentChangedEvent = new object();
		public virtual event EventHandler PrintableComponentChanged {
			add { Events.AddHandler(PrintableComponentChangedEvent, value); }
			remove { Events.RemoveHandler(PrintableComponentChangedEvent, value); }
		}
		void RaisePrintableComponentChanged(EventArgs e) {
			EventHandler handler = (EventHandler)Events[PrintableComponentChangedEvent];
			if(handler != null) handler(this, e);
		}
		public PrintableComponentContainer() {
			ClipContent = false;
			PrintableContentOffset = SizeF.Empty;
		}
		protected override ILink2 CreateLink() {
			if(printableComponent is ILink2)
				return (ILink2)printableComponent;
			if(printableComponent is IPrintable && WinControl == null) {
				PrintableComponentLinkBase link = new PrintableComponentLinkBase();
				link.Component = (IPrintable)printableComponent;
				return link;
			}
			return base.CreateLink();
		}
		protected override VisualBrick CreateBrick(VisualBrick[] childrenBricks) {
			if(PrintAsBricks && ClipContent) {
				PrintingSystemBase ps = Link.PrintingSystem as PrintingSystemBase;
				if((ps == null || ps.IsDisposed) && Link is LinkBase) {
					ps = new PrintingSystemBase();
					container.Add(ps);
					((LinkBase)Link).PrintingSystemBase = ps;
				}
				ps.RemoveService(typeof(MergeBrickHelper), true);
				Link.PaperKind = System.Drawing.Printing.PaperKind.Custom;
				Link.Margins = new System.Drawing.Printing.Margins(0, 0, 0, 0);
				Link.MinMargins = new System.Drawing.Printing.Margins(0, 0, 0, 0);
				Link.CustomPaperSize = new Size(int.MaxValue/4, int.MaxValue/4);
				Link.CreateDocument();
				PanelBrick panelBrick = new FakedPanelBrick(this);
				panelBrick.SeparableVert = !KeepTogether;
				if(ps.Pages.Count > 0) {
					RectangleF bounds = XRConvert.Convert(new RectangleF(PointF.Empty, SizeF), Dpi, GraphicsDpi.Document);
					SizeF offset = XRConvert.Convert(PrintableContentOffset, Dpi, GraphicsDpi.Document);
					Dictionary<BrickStyle, BrickStyle> styleMap = new Dictionary<BrickStyle, BrickStyle>(new StyleReferenceComparer());
					foreach(Brick brick in ((PSPage)ps.Pages[0]).Bricks) {
						RectangleF brickRect = RectHelper.OffsetRectF(brick.Rect, offset.Width, offset.Height);
						if(bounds.IntersectsWith(brickRect)) {
							BrickStyle outStyle;
							BrickStyle styleBefore = ((VisualBrick)brick).Style;
							if(styleMap.TryGetValue(styleBefore, out outStyle)) {
								((VisualBrick)brick).Style = outStyle;
							} else {
								brick.Initialize(RootReport.PrintingSystem, Rectangle.Empty, true);
								BrickStyle styleAfter = ((VisualBrick)brick).Style;
								if(styleBefore != styleAfter) {
									styleMap.Add(styleBefore, styleAfter);
								}
							}
							brick.PageBuilderOffset = PointF.Empty;
							brick.SetBounds(brickRect, GraphicsDpi.Document);
							brick.SeparableVert = !KeepTogether;
							panelBrick.Bricks.Add(brick);
						}
					}
				}
				return panelBrick;
			}
			return base.CreateBrick(childrenBricks);
		}
		protected override void BeforeReportPrint() {
			base.BeforeReportPrint();
			DisposeComponents();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				DisposeComponents();
			}
			base.Dispose(disposing);
		}
		void DisposeComponents() {
			foreach(IDisposable item in container)
				item.Dispose();
			container.Clear();
		}
		protected override void CollectAssociatedComponents(DesignItemList components) {
		}
	}
}
