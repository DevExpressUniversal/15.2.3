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

using DevExpress.XtraPrinting.Export;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.Remoting;
using System.Text;
using DevExpress.Utils.Serializing;
using System.Runtime.InteropServices;
#if SL
using System.Windows.Printing;
using DevExpress.Xpf.Drawing.Printing;
#else
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Windows.Forms;
using DevExpress.XtraPrinting.Export.Rtf;
#endif
namespace DevExpress.XtraPrinting {
	public class ShouldSerializeEventArgs : EventArgs {
		public string PropertyName {
			get;
			private set;
		}
		public bool? ShouldSerialize {
			get;
			set;
		}
		public ShouldSerializeEventArgs(string propertyName) {
			PropertyName = propertyName;
		}
	}
}
namespace DevExpress.XtraPrinting {
	public delegate void PagePaintEventHandler(object sender, PagePaintEventArgs e);
	public delegate void PageEventHandler(object sender, PageEventArgs e);
	public delegate void PrintDocumentEventHandler(object sender, PrintDocumentEventArgs e);
	public delegate void MarginsChangeEventHandler(object sender, MarginsChangeEventArgs e);
	public delegate void EmptySpaceEventHandler(object sender, EmptySpaceEventArgs e);
	public delegate void PrintProgressEventHandler(object sender, PrintProgressEventArgs e);
	public delegate void ExceptionEventHandler(object sender, ExceptionEventArgs args);
	public delegate void BrickEventHandlerBase(object sender, BrickEventArgsBase e);
	public delegate void PageInsertCompleteEventHandler(object sender, PageInsertCompleteEventArgs e);
	public delegate void BrickResolveEventHandler(object sender, BrickResolveEventArgs args);
	public delegate void XlsxDocumentCreatedEventHandler(object sender, XlsxDocumentCreatedEventArgs e);
	public delegate void XlSheetCreatedEventHandler(object sender, XlSheetCreatedEventArgs e);
	public class BrickResolveEventArgs : ResolveEventArgs {
		public Brick Brick { get; set; }
		public BrickResolveEventArgs(string name)
			: base(name) {
		}
	}
	public class PageInsertCompleteEventArgs : EventArgs {
		int pageIndex;
		public int PageIndex {
			get {
				return pageIndex;
			}
		}
		public PageInsertCompleteEventArgs(int pageIndex) {
			this.pageIndex = pageIndex;
		}
	}
	public class BrickEventArgsBase : EventArgs {
		private Brick brick;
		public Brick Brick {
			get { return brick; }
		}
		public BrickEventArgsBase(Brick brick)
			: base() {
			this.brick = brick;
		}
	}
	public class ExceptionEventArgs : EventArgs {
		Exception exception;
		bool handled;
		public Exception Exception { get { return exception; } }
		public bool Handled { get { return handled; } set { handled = value; } }
		public ExceptionEventArgs(Exception exception) {
			this.exception = exception;
		}
	}
	public class PrintProgressEventArgs : EventArgs {
		QueryPageSettingsEventArgs source;
		int pageIndex;
		public int PageIndex { get { return pageIndex; } }
		public PageSettings PageSettings {
			get {
				return source.PageSettings;
			}
			set {
				source.PageSettings = value;
			}
		}
		public PrintAction PrintAction {
			get {
				return source.PrintAction;
			}
		}
		internal PrintProgressEventArgs(QueryPageSettingsEventArgs source, int pageIndex)
			: base() {
			this.source = source;
			this.pageIndex = pageIndex;
		}
	}
	public class EmptySpaceEventArgs : EventArgs {
		public PSPage Page { get; private set; }
		public float EmptySpaceOffset { get; private set; }
		public float EmptySpace { get; private set; }
		public DocumentBand SpaceDocumentBand { get; private set; }
		public EmptySpaceEventArgs(PSPage page, DocumentBand spaceDocumentBand, float emptySpace, float emptySpaceOffset)
			: base() {
			Page = page;
			EmptySpace = emptySpace;
			SpaceDocumentBand = spaceDocumentBand;
			EmptySpaceOffset = emptySpaceOffset;
		}
	}
	public class MarginsChangeEventArgs : EventArgs {
		private MarginSide side = MarginSide.None;
		private float value;
		public MarginSide Side {
			get { return side; }
		}
		public float Value {
			get { return value; }
			set { this.value = value; }
		}
		internal MarginsChangeEventArgs(MarginSide side, float value)
			: base() {
			this.value = value;
			this.side = side;
		}
	}
	public class PageEventArgs : EventArgs {
		private Page page;
		IList<DocumentBand> documentBands;
		public Page Page { get { return page; } }
		public IList<DocumentBand> DocumentBands { get { return documentBands; } }
		internal PageEventArgs(Page page, IList<DocumentBand> documentBands)
			: base() {
			this.page = page;
			this.documentBands = documentBands;
		}
		internal PageEventArgs(List<DocumentBand> documentBands) : this(null, documentBands) {
		}
		internal PageEventArgs(Page page) : this(page, null) { }
	}
	public class PagePaintEventArgs : EventArgs {
		Page page;
		IGraphics graphics;
		RectangleF pageRectangle;
		public Page Page { get { return page; } }
		public RectangleF PageRectangle { get { return pageRectangle; } }
		public IGraphics Graphics { get { return graphics; } }
		internal PagePaintEventArgs(Page page, IGraphics graphics, RectangleF pageRectangle) {
			this.page = page;
			this.graphics = graphics;
			this.pageRectangle = pageRectangle;
		}
	}
	public class PrintDocumentEventArgs : EventArgs {
		private PrintDocument printDocument;
		public PrintDocument PrintDocument {
			get { return printDocument; }
		}
		internal PrintDocumentEventArgs(PrintDocument printDocument)
			: base() {
			this.printDocument = printDocument;
		}
	}
	public class ProgressEventArgs : EventArgs {
		int position;
		int maximum;
		public int Position { get { return position; } }
		public int Maximum { get { return maximum; } }
		public ProgressEventArgs(int position, int maximum) {
			this.position = position;
			this.maximum = maximum;
		}
	}
	public class XlsxDocumentCreatedEventArgs : EventArgs {
		string[] sheetNames;
		public string[] SheetNames { get { return sheetNames; } }
		public XlsxDocumentCreatedEventArgs(string[] sheetNames) {
			this.sheetNames = sheetNames;
		}
	}
	public class XlSheetCreatedEventArgs : EventArgs {
		string sheetName;
		public string SheetName {
			get { return sheetName; }
			set { sheetName = value; }
		}
		public int Index { get; private set; }
		public XlSheetCreatedEventArgs(int index, string sheetName) {
			Index = index;
			this.sheetName = sheetName;
		}
	}
}
