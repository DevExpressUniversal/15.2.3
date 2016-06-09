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
using System.Reflection;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.InteropServices;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Export.Web;
using System.Collections;
using DevExpress.XtraPrinting.Export.Pdf;
using DevExpress.XtraPrinting.Export;
using System.IO;
using DevExpress.XtraPrinting.Links;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Control;
namespace DevExpress.XtraPrintingLinks
{
	[Designer("DevExpress.XtraPrinting.Design.LinkDesigner," + AssemblyInfo.SRAssemblyPrintingDesign),]
	public class RichTextBoxLink : RichTextBoxLinkBase, IWinLink {
		#region windows forms specific
		ImageCollection imageCollection;
		[
		Category(NativeSR.CatHeadersFooters),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public DevExpress.Utils.Images Images {
			get { return ImageCollection.Images; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		]
		public ImageCollection ImageCollection {
			get {
				if(imageCollection == null)
					imageCollection = new ImageCollection(false);
				return imageCollection;
			}
		}
		bool ShouldSerializeImageCollection() {
			return imageCollection != null && imageCollection.Images.Count > 0;
		}
		[
		DefaultValue(null),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public ImageCollectionStreamer ImageStream {
			get {
				return imageCollection != null ? imageCollection.ImageStream : null;
			}
			set {
				ImageCollection.ImageStream = value;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public PrintingSystem PrintingSystem {
			get {
				return (PrintingSystem)PrintingSystemBase;
			}
			set {
				PrintingSystemBase = value;
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(imageCollection != null) {
					imageCollection.Dispose();
					imageCollection = null;
				}
			}
			base.Dispose(disposing);
		}
		protected override Image[] GetImageArray() {
			return ImageCollection.Images.ToArray();
		}
		public virtual void CreateDocument(PrintingSystem ps) {
			base.CreateDocument(ps);
		}
		public void Print(string printerName) {
			new LinkPrintTool(this).Print(printerName);
		}
		public void PrintDlg() {
			new LinkPrintTool(this).PrintDialog();
		}
		public void ShowPreviewDialog() {
			ShowPreviewDialog(null);
		}
		public virtual void ShowPreviewDialog(IWin32Window owner) {
			ShowPreviewDialog(owner, null);
		}
		public virtual void ShowPreviewDialog(IWin32Window owner, UserLookAndFeel lookAndFeel) {
			new LinkPrintTool(this).ShowPreviewDialog(owner, lookAndFeel);
		}
		public virtual void ShowPreview() {
			ShowPreview(null);
		}
		public virtual void ShowPreview(UserLookAndFeel lookAndFeel) {
			new LinkPrintTool(this).ShowPreview(lookAndFeel);
		}
		public void ShowRibbonPreview(UserLookAndFeel lookAndFeel) {
			new LinkPrintTool(this).ShowRibbonPreview(lookAndFeel);
		}
		public void ShowRibbonPreviewDialog(UserLookAndFeel lookAndFeel) {
			new LinkPrintTool(this).ShowRibbonPreviewDialog(lookAndFeel);
		}
		public override void HandleCommand(PrintingSystemCommand command, object[] args, IPrintControl printControl, ref bool handled) {
			new LinkCommandHandler(this, Images).HandleCommand(command, args, (PrintControl)printControl, ref handled);
		}
		public override bool CanHandleCommand(PrintingSystemCommand command, IPrintControl printControl) {
			return CanHandleCommandInternal(command, printControl);
		}
		#endregion // windows forms specific
		public RichTextBoxLink() : base() {
		}
		public RichTextBoxLink(PrintingSystem ps) : base(ps) {
		}
		internal RichTextBoxLink(PrintingSystemBase ps)
			: base(ps) {
		}
	}
}
