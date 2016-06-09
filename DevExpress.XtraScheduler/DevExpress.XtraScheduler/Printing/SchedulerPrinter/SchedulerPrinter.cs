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

using DevExpress.LookAndFeel;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Localization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraScheduler.Printing.Native {
	public class SchedulerPrinter : IDisposable, IPrintable {
		readonly SchedulerControl control;
		readonly SchedulerPrintStyle printStyle;
		SchedulerPrinterParameters printerParameters;
		IDisposable printerCore;
		public SchedulerPrinter(SchedulerControl control, SchedulerPrintStyle printStyle) {
			this.control = control;
			this.printStyle = printStyle;
		}
		protected internal SchedulerPrintStyle PrintStyle { get { return printStyle; } }
		protected internal SchedulerControl Control { get { return control; } }
		protected internal PrintingSystemBase PrintingSystemBase { get { return PrinterCore.PrintingSystemBase; } }
		public SchedulerPrinterParameters PrinterParameters { get { return printerParameters; } }
		public PageSettings PageSettings { get { return PrinterCore.PageSettings; } }
		protected ComponentPrinterBase PrinterCore {
			get {
				if (printerCore == null)
					printerCore = CreateComponentPrinter();
				return (ComponentPrinterBase)printerCore;
			}
		}
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (printerCore != null) {
					printerCore.Dispose();
					printerCore = null;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~SchedulerPrinter() {
			Dispose(false);
		}
		#endregion
		public void Initialize(IPrintingSystem ps, ILink link) {
			XtraSchedulerDebug.Assert(printerParameters == null);
			printerParameters = new SchedulerPrinterParameters(ps, link);
		}
		public void Finalize(IPrintingSystem ps, ILink link) {
			if (printerParameters != null) {
				printerParameters.Dispose();
				printerParameters = null;
			}
		}
		public void CreateArea(string areaName, IBrickGraphics graphics) {
			switch (areaName) {
				case DevExpress.XtraPrinting.SR.Detail:
					PrintDetailArea(graphics);
					break;
			}
		}
		public void AcceptChanges() {
		}
		public void RejectChanges() {
		}
		public void ShowHelp() {
		}
		public bool CreatesIntersectedBricks {
			get { return true; }
		}
		public bool SupportsHelp() {
			return false;
		}
		public bool HasPropertyEditor() {
			return false;
		}
		public System.Windows.Forms.UserControl PropertyEditorControl { get { return null; } }
		protected virtual ComponentPrinterBase CreateComponentPrinter() {
			return new ComponentPrinter(this);
		}
		protected internal virtual Form ShowPreview(UserLookAndFeel lookAndFeel) {
			if (ComponentPrinterBase.IsPrintingAvailable(false))
				return PrinterCore.ShowPreview(lookAndFeel);
			return null;
		}
		protected internal virtual void Print() {
			if (ComponentPrinterBase.IsPrintingAvailable(false))
				PrinterCore.Print();
		}
		protected internal void SetPageSettings(System.Drawing.Printing.PageSettings pageSettings) {
			PrinterCore.SetPageSettings(pageSettings);
			PrinterCore.PageSettings.PrinterSettings = pageSettings.PrinterSettings;
		}
		protected internal void CreateDocument() {
			if (ComponentPrinterBase.IsPrintingAvailable(false))
				PrinterCore.CreateDocument();
		}
		void PrintDetailArea(IBrickGraphics graphics) {
			if (control.DataStorage == null)
				return;
			Size pageSize = Size.Truncate(((BrickGraphics)graphics).ClientPageSize);
			if (pageSize.Width <= 0 || pageSize.Height <= 0)
				return;
			printerParameters.GInfo.Cache.Paint = new XBrickPaint(printerParameters, graphics);
			Rectangle pageBounds = new Rectangle(0, 0, pageSize.Width, pageSize.Height);
			IPrintableObjectViewInfo printViewInfo;
			try {
				PrintViewInfoBuilder viewInfoBuilder = PrintViewInfoBuilder.CreateInstance(printStyle, control, printerParameters.GInfo);
				printViewInfo = viewInfoBuilder.CreateViewInfo(pageBounds);
			} catch (Exception exception) {
				XtraMessageBox.Show(control.LookAndFeel, exception.Message, SchedulerLocalizer.GetString(SchedulerStringId.Msg_Warning), MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}
			GraphicsInfoArgs info = new GraphicsInfoArgs(printerParameters.GInfo.Cache, pageBounds);
			GraphicsCache cache = info.Cache;
			try {
				printViewInfo.Print(info);
			} finally {
				cache.Dispose();
			}
			ISupportClear viewInfo = printViewInfo as ISupportClear;
			if (viewInfo != null)
				viewInfo.Clear();
		}
	}
}
