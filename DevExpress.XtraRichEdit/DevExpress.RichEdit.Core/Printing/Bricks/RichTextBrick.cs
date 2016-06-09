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
using System.Collections;
using System.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.RichText;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraPrinting.Export.Rtf;
using DevExpress.XtraPrinting.BrickExporters;
using DevExpress.Office.Drawing;
using DevExpress.XtraRichEdit;
using System.ComponentModel;
using DevExpress.Utils.StoredObjects;
namespace DevExpress.XtraPrinting {
	[BrickExporter(typeof(RichTextBrickExporter))]
	public class RichTextBrick : RichTextBrickBase, IRichTextBrick {
		static readonly AttachedProperty<int> pageCountProp = AttachedPropertyBase.Register<int>(DOCVARIABLES.PageCount, typeof(Brick));
		static readonly AttachedProperty<int> pageNumberProp = AttachedPropertyBase.Register<int>(DOCVARIABLES.PageNumber, typeof(Brick));
		public const int InfiniteHeight = 100000;
		string rtfText = string.Empty;
		int IRichTextBrick.InfiniteHeight {
			get { return RichTextBrick.InfiniteHeight; }
		}
		protected internal override bool ShouldApplyPaddingInternal { get { return true; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichTextBrickNoClip")]
#endif
		public override bool NoClip { get { return true; } set { } }
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichTextBrickRtfText"),
#endif
		XtraSerializableProperty]
		public string RtfText
		{
			get { return rtfText; }
			set
			{
				if (rtfText == value)
					return;
				rtfText = ValidateRtf(value, DocumentModel);
				SetInternalRtf(rtfText);
			}
		}
		public override string Text {
			set {
				RecreateDocManagerAndPrinter();
				XtraRichTextEditHelper.ImportPlainTextToDocManager(value, DocumentModel);
				XtraRichTextEditHelper.SetContentFont(DocumentModel, Style.Font);
				rtfText = XtraRichTextEditHelper.GetRtfFromDocManager(DocumentModel);
			}
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichTextBrickBaseFont")]
#endif
		public Font BaseFont {
			get {
				if (DocumentModel != null) {
					CharacterProperties characterProperties = DocumentModel.CharacterStyles[0].CharacterProperties;
					int fontIndex = DocumentModel.FontCache.CalcFontIndex(characterProperties.FontName, characterProperties.DoubleFontSize, characterProperties.FontBold, characterProperties.FontItalic, characterProperties.Script, false, false);
					GdiPlusFontInfo fontInfo = (GdiPlusFontInfo)DocumentModel.FontCache[fontIndex];
					return fontInfo.Font;
				}
				else
					return Style.Font;
			}
			set {
				if (value == null)
					return;
				CharacterProperties characterProperties = DocumentModel.CharacterStyles[0].CharacterProperties;
				CharacterPropertiesFontAssignmentHelper.AssignFont(characterProperties, value);
			}
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichTextBrickBrickType")]
#endif
		public override string BrickType { get { return BrickTypes.RichText; } }
		IList IRichTextBrick.GetChildBricks() {
			PrepareDocumentPrinter(null);
			return InnerBricks;
		}
		protected override DocumentModel DocumentModel {
			get {
				return base.DocumentModel;
			}
			set {
				if(base.DocumentModel != null)
					base.DocumentModel.CalculateDocumentVariable -= OnCalculateDocumentVariable;
				base.DocumentModel = value;
				if(base.DocumentModel != null) {
					base.DocumentModel.CalculateDocumentVariable += OnCalculateDocumentVariable;
				}
			}
		}
		#region tests
#if DEBUGTEST
		public GraphicsDocumentPrinter GraphicsDocumentPrinter { get { return Printer; } }
#endif
		#endregion
		public RichTextBrick(IBrickOwner brickOwner) : base(brickOwner) {
			RtfText = RtfTags.WrapTextInRtf(string.Empty);
		}
		public RichTextBrick()
			: this(NullBrickOwner.Instance) {
		}
		protected RichTextBrick(RichTextBrick brick)
			: base(brick) {
			RtfText = brick.rtfText;
		}
		public override object Clone() {
			return new RichTextBrick(this);
		}
		void OnCalculateDocumentVariable(object sender, XtraRichEdit.CalculateDocumentVariableEventArgs e) {
			IPrintingSystemContext context = DocumentModel.GetService(typeof(IPrintingSystemContext)) as IPrintingSystemContext;
			if(e.VariableName == DOCVARIABLES.PageNumber) {
				int value = context != null ? GetPageNumber(context) : 9999;
				SetVariable(e, pageNumberProp, value);
			} else if(e.VariableName == DOCVARIABLES.PageCount) {
				int value = context != null ? context.PrintingSystem.PageCount : 9999;
				SetVariable(e, pageCountProp, value);
			} else if(e.VariableName == DOCVARIABLES.UserName) {
				e.Value = GetFormattedValue(e.Arguments, PrintingSystemBase.UserName);
				e.Handled = true;
			} else if(e.VariableName == DOCVARIABLES.Date) {
				e.Value = GetFormattedValue(e.Arguments, DateTimeHelper.Now);
				e.Handled = true;
			}
		}
		static object GetFormattedValue(ArgumentCollection arguments, object value) {
			string format;
			return TryGetFromat(arguments, out format) ? string.Format(format, value) : value;
		}
		static bool TryGetFromat(ArgumentCollection arguments, out string value) {
			if(arguments.Count > 0 && arguments[0].Value is string) {
				value = (string)arguments[0].Value;
				return true;
			}
			value = string.Empty;
			return false;
		}
		void SetVariable<T>(XtraRichEdit.CalculateDocumentVariableEventArgs e, AttachedProperty<T> prop, T value) {
			SetAttachedValue(prop, value, default(T));
			e.Value = GetFormattedValue(e.Arguments, value);
			e.Handled = true;
		}
		public void SetDocManagerAndPrinter(DocumentModel documentModel, GraphicsDocumentPrinter printer) {
			DisposeDocManagerAndPrinter();
			IsOwnDocumentModelAndPrinter = false;
			this.DocumentModel = documentModel;
			this.Printer = printer;
		}
		SizeF CalcFormatSize() {
			RectangleF clientRect = CalcClientRect();
			clientRect.Height = InfiniteHeight;
			return clientRect.Size;
		}
		public RectangleF CalcClientRect() {
			return CalcClientRectUsingPaddingAndBorders(InitialRect);
		}
		protected internal override void Scale(double scaleFactor) {
			PrepareDocumentPrinter(null);
			base.Scale(scaleFactor);
		}
		internal protected override void PrepareDocumentPrinter(IPrintingSystemContext context) {
			base.PrepareDocumentPrinter(context);
			int value;
			bool needUpdate = false;
			if(context != null && TryGetAttachedValue(pageNumberProp, out value) && !Equals(value, GetPageNumber(context)))
				needUpdate = true;
			if(context != null && TryGetAttachedValue(pageCountProp, out value) && !Equals(value, context.PrintingSystem.PageCount))
				needUpdate = true;
			if(needUpdate) {
				DocumentModel.AddService(typeof(IPrintingSystemContext), context);
				try {
					DocumentModel.UpdateFields(UpdateFieldOperationType.Normal);
				} finally {
					DocumentModel.RemoveService(typeof(IPrintingSystemContext));
				}
				Printer.Format(CalcFormatSize());
				IsFormatted = true;
			} else if(!IsFormatted) {
				Printer.Format(CalcFormatSize());
				IsFormatted = true;
			}
		}
		static int GetPageNumber(IPrintingSystemContext context) { 
			return context.DrawingPage != null ? context.DrawingPage.Index + 1 : 1;
		}
	}
	public class RichTextBrickExporter : RichTextBrickBaseExporter {
	}
}
namespace DevExpress.XtraPrinting.Native {
	public static class DOCVARIABLES {
		public const string
			PageNumber = "PAGE",
			PageCount = "NUMPAGES",
			UserName = "USER",
			Date = "DATE";
	}
}
