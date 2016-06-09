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
using System.Drawing.Printing;
using System.Drawing.Imaging;
using DevExpress.XtraPrinting;
using DevExpress.XtraGauges.Base;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System;
namespace DevExpress.XtraGauges.Core.Printing {
	public enum PrintSizeMode { None, Stretch, Zoom }
	public class GaugePrinter : IDisposable, ICloneable {
		IPrintingSystem psCore;
		ILink linkCore;
		IGaugeContainer containerCore;
		PrintSizeMode sizeModeCore;
		public GaugePrinter(IGaugeContainer gaugeContiner) {
			this.containerCore = gaugeContiner;
		}
		public PrintSizeMode SizeMode {
			get { return sizeModeCore; }
			set { sizeModeCore = value; }
		}
		protected ILink Link {
			get { return linkCore; }
		}
		protected IPrintingSystem PS {
			get { return psCore; }
		}
		protected IGaugeContainer GaugeContainer {
			get { return containerCore; }
		}
		public void Dispose() {
			Release();
		}
		void OnPrintingSystemAfterChange(object sender, DevExpress.XtraPrinting.ChangeEventArgs e) {
			if(PS == null || Link == null) return;
			switch(e.EventName) {
				case DevExpress.XtraPrinting.SR.PageSettingsChanged:
				case DevExpress.XtraPrinting.SR.AfterMarginsChange:
					((LinkBase)Link).Margins = ((PrintingSystemBase)PS).PageMargins;
					Link.CreateDocument();
					break;
			}
		}
		void SetPS(IPrintingSystem ps) {
			if(PS != null) PS.AfterChange -= OnPrintingSystemAfterChange;
			this.psCore = ps;
			if(PS != null) PS.AfterChange += OnPrintingSystemAfterChange;
		}
		Rectangle GetPrintBounds() {
			Rectangle controlBounds = GaugeContainer.Bounds;
			if(SizeMode == PrintSizeMode.None) return controlBounds;
			Rectangle pageBounds = ((PrintingSystemBase)PS).PageBounds;
			System.Drawing.Printing.Margins margins = ((PrintingSystemBase)PS).PageMargins;
			pageBounds.Width -= margins.Left + margins.Right;
			pageBounds.Height -= margins.Top + margins.Bottom;
			float pixWidth = GraphicsUnitConverter.Convert(pageBounds.Width - 1, GraphicsDpi.HundredthsOfAnInch, GraphicsDpi.UnitToDpi(GraphicsUnit.Pixel));
			float pixHeight = GraphicsUnitConverter.Convert(pageBounds.Height - 1, GraphicsDpi.HundredthsOfAnInch, GraphicsDpi.UnitToDpi(GraphicsUnit.Pixel));
			if(SizeMode == PrintSizeMode.Stretch) return new Rectangle(0, 0, (int)Math.Floor(pixWidth), (int)Math.Floor(pixHeight));
			float controlRatio = (float)controlBounds.Width / (float)controlBounds.Height;
			float pageRatio = pixWidth / pixHeight;
			float width, height;
			if(controlRatio > pageRatio) {
				width = pixWidth;
				height = pixWidth / controlRatio;
			}
			else {
				width = pixHeight * controlRatio;
				height = pixHeight;
			}
			if(SizeMode == PrintSizeMode.Zoom) return new Rectangle(0, 0, (int)Math.Floor(width), (int)Math.Floor(height));
			throw new NotImplementedException();
		}
		public virtual void Initialize(IPrintingSystem ps, ILink link) {
			SetPS(ps);
			this.linkCore = link;
		}
		public virtual void Release() {
			SetPS(null);
			this.linkCore = null;
		}
		public virtual object Clone() {
			return new GaugePrinter(GaugeContainer);
		}
		public virtual void CreateArea(string areaName, IBrickGraphics graph) {
			if(PS != null && areaName == DevExpress.XtraPrinting.SR.Detail)
				CreateDetail(graph);
		}
		public virtual void CreateDetail(IBrickGraphics graph) {
			if(GaugeContainer == null) return;
			Image image = null;
			try {
				Rectangle bounds = GetPrintBounds();
				image = GaugeContainer.GetImage(bounds.Size.Width, bounds.Size.Height);
				ImageBrick brick = (ImageBrick)PS.CreateBrick("ImageBrick");
				brick.Image = image;
				brick.Sides = BorderSide.None;
				graph.DrawBrick(brick, bounds);
				image = null;
			}
			finally {
				if(image != null) image.Dispose();
			}
		}
		public override bool Equals(object obj) {
			return obj.GetType().Equals(GetType());
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	public abstract class BasePrintableProvider : IPrintable {
		GaugePrinter printerCore;
		protected BasePrintableProvider(GaugePrinter printer) {
			printerCore = printer;
		}
		protected GaugePrinter Printer {
			get { return printerCore; }
		}
		void IBasePrintable.Initialize(IPrintingSystem ps, ILink link) {
			if(Printer != null) Printer.Initialize(ps, link);
		}
		void IBasePrintable.Finalize(IPrintingSystem ps, ILink link) {
			if(Printer != null) Printer.Release();
		}
		void IBasePrintable.CreateArea(string areaName, IBrickGraphics graph) {
			if(Printer != null) Printer.CreateArea(areaName, graph);
		}
		bool IPrintable.CreatesIntersectedBricks { get { return true; } }
		System.Windows.Forms.UserControl IPrintable.PropertyEditorControl { get { return null; } }
		bool IPrintable.HasPropertyEditor() { return false; }
		bool IPrintable.SupportsHelp() { return false; }
		void IPrintable.ShowHelp() { }
		void IPrintable.AcceptChanges() { }
		void IPrintable.RejectChanges() { }
	}
}
