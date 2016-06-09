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
namespace DevExpress.XtraPrintingLinks {
	public enum RichTextPrintFormat {
		ClientPageSize,
		RichTextBoxSize,
		Custom
	};
	[DefaultProperty("RichTextBox")]
	public class RichTextBoxLinkBase : LinkBase {
		#region static
		static readonly Size DefaultCustomSize = new Size(1000, 1000);
		#endregion
		RichTextBox richTextBox;
		RichTextPrintFormat printFormat = RichTextPrintFormat.ClientPageSize;
		bool infiniteHeight;
		Size customSize = DefaultCustomSize;
		#region properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichTextBoxLinkBasePrintableObjectType")]
#endif
		public override Type PrintableObjectType {
			get { return typeof(RichTextBox); }
		}
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichTextBoxLinkBaseRichTextBox"),
#endif
		Category(NativeSR.CatPrinting),
		DefaultValue(null),
		]
		public RichTextBox RichTextBox {
			get { return richTextBox; }
			set { richTextBox = value; }
		}
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichTextBoxLinkBasePrintFormat"),
#endif
		Category(NativeSR.CatPrintOptions),
		DefaultValue(RichTextPrintFormat.ClientPageSize),
		]
		public RichTextPrintFormat PrintFormat {
			get { return printFormat; }
			set {
				if (!Enum.IsDefined(typeof(RichTextPrintFormat), value)) {
					throw new InvalidEnumArgumentException("streamType", (int)value, typeof(RichTextPrintFormat));
				}
				printFormat = value;
			}
		}
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichTextBoxLinkBaseInfiniteFormatHeight"),
#endif
		Category(NativeSR.CatPrintOptions),
		DefaultValue(false),
		]
		public bool InfiniteFormatHeight {
			get { return infiniteHeight; }
			set { infiniteHeight = value; }
		}
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichTextBoxLinkBaseCustomFormatSize"),
#endif
		Category(NativeSR.CatPrintOptions),
		]
		public Size CustomFormatSize {
			get { return customSize; }
			set {
				if (value.Width <= 0 || value.Height <= 0)
					throw new ArgumentException("customSize");
				customSize = value;
			}
		}
		#endregion
		public RichTextBoxLinkBase()
			: base() {
		}
		public RichTextBoxLinkBase(PrintingSystemBase ps)
			: base(ps) {
		}
		public override void SetDataObject(object data) {
			RichTextBox rich = data as RichTextBox;
			if (rich != null)
				richTextBox = rich;
		}
		public override void AddSubreport(PointF offset) {
			if (richTextBox != null)
				base.AddSubreport(offset);
		}
		protected override void BeforeCreate() {
			if(RichTextBox == null)
				throw new NullReferenceException("The RichTextBox property value must not be null");
			base.BeforeCreate();
			ps.Graph.PageUnit = GraphicsUnit.Document;
			PrintingSystemCommand[] commands = new PrintingSystemCommand[] {
				PrintingSystemCommand.ExportXls, 
				PrintingSystemCommand.ExportXlsx, 
				PrintingSystemCommand.ExportTxt, 
				PrintingSystemCommand.ExportCsv, 
				PrintingSystemCommand.ExportHtm, 
				PrintingSystemCommand.ExportMht,
				PrintingSystemCommand.ExportRtf,
				PrintingSystemCommand.SendXls, 
				PrintingSystemCommand.SendXlsx, 
				PrintingSystemCommand.SendTxt, 
				PrintingSystemCommand.SendCsv, 
				PrintingSystemCommand.SendMht, 
				PrintingSystemCommand.SendRtf,
			};
			foreach (PrintingSystemCommand command in commands)
				((IPrintingSystem)this.PrintingSystemBase).SetCommandVisibility(command, true);
		}
		protected override void CreateDetail(BrickGraphics gr) {
			if (richTextBox == null) return;
			using (MemoryStream stream = new MemoryStream()) {
				PSNativeMethods.ForceCreateHandle(richTextBox);
				richTextBox.SaveFile(stream, System.Windows.Forms.RichTextBoxStreamType.RichText);
				stream.Position = 0;
				gr.DefaultBrickStyle = BrickStyle.CreateDefault();
				FormattedRichTextBrick brick = new FormattedRichTextBrick();
				brick.Init(stream, GetFormatSize());
				gr.DrawBrick(brick);
			}
		}
#if DEBUG
		public
#endif
 SizeF GetFormatSize() {
			SizeF formatSize = SizeF.Empty;
			switch (printFormat) {
				case RichTextPrintFormat.ClientPageSize:
					formatSize = PrintingSystemBase.Graph.ClientPageSize;
					break;
				case RichTextPrintFormat.Custom:
					formatSize = GraphicsUnitConverter.Convert(customSize, GraphicsDpi.Pixel, GraphicsDpi.Document);
					break;
				case RichTextPrintFormat.RichTextBoxSize:
					formatSize = new SizeF(GraphicsUnitConverter.Convert(richTextBox.ClientSize, GraphicsDpi.Pixel, GraphicsDpi.Document));
					break;
			}
			if (infiniteHeight)
				formatSize.Height = 100000;
			return formatSize;
		}
		bool ShouldSerializeCustomFormatSize() {
			return customSize != DefaultCustomSize;
		}
	}
}
