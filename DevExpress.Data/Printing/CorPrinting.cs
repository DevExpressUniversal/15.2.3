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
using System.Reflection;
using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.Native;
using DevExpress.Data.Helpers;
using DevExpress.Utils.Design;
using DevExpress.Data;
using System.Collections.Generic;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Windows.Forms;
#if SL
using DevExpress.Xpf.Drawing;
using DevExpress.Xpf.Drawing.Text;
using DevExpress.XtraPrinting;
using System.Windows.Controls;
#else
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using DevExpress.Compatibility.System.Drawing.Text;
#endif
namespace DevExpress.XtraPrinting.Native {
	public interface IObject {
		string ObjectType { get; }
	}
	public class PaddingInfoConverter : StructIntConverter {
		public static readonly PaddingInfoConverter Instance = new PaddingInfoConverter();
		public override Type Type { get { return typeof(PaddingInfo); } }
		PaddingInfoConverter() {
		}
		protected override int[] GetValues(object obj) {
			PaddingInfo paddingInfo = (PaddingInfo)obj;
			return new int[] { paddingInfo.Left, paddingInfo.Right, paddingInfo.Top, paddingInfo.Bottom, (int)paddingInfo.Dpi };
		}
		protected override object CreateObject(int[] values) {
			return new PaddingInfo(values[0], values[1], values[2], values[3], values[4]);
		}
	}
#if !DXPORTABLE
	public class BrickStringFormatConverter : StructStringConverter {
		public static readonly BrickStringFormatConverter Instance = new BrickStringFormatConverter();
		public override Type Type { get { return typeof(BrickStringFormat); } }
		protected override char Delimiter { get { return ';'; } }
		BrickStringFormatConverter() {
		}
		protected override string[] GetValues(object obj) {
			BrickStringFormat format = (BrickStringFormat)obj;
			return new string[] { 
					format.Alignment.ToString(), 
					format.LineAlignment.ToString(), 
					format.FormatFlags.ToString(), 
					format.HotkeyPrefix.ToString(), 
					format.Trimming.ToString(),
					format.PrototypeKind.ToString() 
				};
		}
		protected override object CreateObject(string[] values) {
			BrickStringFormat format = new BrickStringFormat(
				(StringAlignment)Enum.Parse(typeof(StringAlignment), values[0], true),
				(StringAlignment)Enum.Parse(typeof(StringAlignment), values[1], true),
				(StringFormatFlags)Enum.Parse(typeof(StringFormatFlags), values[2], true),
				(HotkeyPrefix)Enum.Parse(typeof(HotkeyPrefix), values[3], true),
				(StringTrimming)Enum.Parse(typeof(StringTrimming), values[4], true)
				);
			format.PrototypeKind = (BrickStringFormatPrototypeKind)Enum.Parse(typeof(BrickStringFormatPrototypeKind), values[5], true);
			return format;
		}
	}
#endif
#if !SL && !DXPORTABLE
	public class LineSplitter {
		Font font;
		string text;
		StringFormat sf;
		public LineSplitter(string text, Font font, StringFormat sf) {
			this.text = text;
			this.font = font;
			this.sf = sf;
		}
		public float SplitRectangle(RectangleF rect, float position, float defaultTop, GraphicsUnit pageUnit) {
			if(string.IsNullOrEmpty(text))
				return position;
			using(Graphics gr = GraphicsHelper.CreateGraphicsFromHiResImage()) {
				gr.PageUnit = pageUnit;
				RectangleF textBounds = GetActualTextBounds(gr, rect);
				if(textBounds.IsEmpty)
					return position;
				if(position >= rect.Top && position < textBounds.Top)
					return rect.Height > position ? position : defaultTop;
				float lineHeight = font.GetHeight(GraphicsDpi.UnitToDpi(pageUnit));
				if(position >= textBounds.Top && position <= textBounds.Bottom) {
					float height = position - textBounds.Top;
					if(height <= lineHeight) {
						float topSpan = textBounds.Top - rect.Top;
						if(topSpan <= lineHeight / 2)
							return defaultTop;
					}
					height = (float)Math.Floor(height / lineHeight) * lineHeight;
					return textBounds.Top + height;
				}
				System.Diagnostics.Debug.Assert(position > textBounds.Bottom);
				if(rect.Bottom - textBounds.Bottom < lineHeight * 0.3f) {
					float height = position - textBounds.Top;
					height = (float)Math.Floor(height / lineHeight) * lineHeight;
					float newPosition = textBounds.Top + height - lineHeight;
					return newPosition < lineHeight * 0.95f ? position : newPosition;
				}
			}
			return position;
		}
		RectangleF GetCharacterRect(Graphics gr, RectangleF bounds, int characterIndex) {
			CharacterRange[] ranges = new CharacterRange[1];
			ranges[0] = new CharacterRange(characterIndex, 1);
			sf.SetMeasurableCharacterRanges(ranges);
			try {
				Region[] regions = null;
				try {
					regions = gr.MeasureCharacterRanges(text, font, bounds, sf);
					return regions[0].GetBounds(gr);
				} catch {
					return RectangleF.Empty;
				} finally {
					if(regions != null)
						foreach(Region region in regions)
							region.Dispose();
				}
			} finally {
				sf.SetMeasurableCharacterRanges(new CharacterRange[0]);
			}
		}
		RectangleF GetActualTextBounds(Graphics gr, RectangleF bounds) {
			int firstIndex = GetFirstValidSymbol();
			if(firstIndex == -1)
				return bounds;
			RectangleF firstCharacterRect = GetCharacterRect(gr, bounds, firstIndex);
			if(firstCharacterRect.IsEmpty)
				return bounds;
			for(int lastIndex = GetLastValidSymbol(); lastIndex > firstIndex; lastIndex--) {
				RectangleF lastCharacterRect = GetCharacterRect(gr, bounds, lastIndex);
				if(!lastCharacterRect.IsEmpty)
					return RectangleF.FromLTRB(bounds.Left, firstCharacterRect.Top, bounds.Right, lastCharacterRect.Bottom);
			}
			return RectangleF.FromLTRB(bounds.Left, firstCharacterRect.Top, bounds.Right, firstCharacterRect.Bottom);
		}
		int GetFirstValidSymbol() {
			for(int i = 0; i < text.Length; i++)
				if(!Char.IsControl(text[i]))
					return i;
			return -1;
		}
		int GetLastValidSymbol() {
			for(int i = text.Length - 1; i >= 0; i--)
				if(!Char.IsControl(text[i]))
					return i;
			return -1;
		}
	}
#endif
}
namespace DevExpress.XtraPrinting {
#if !SL
	public interface ILabelBrick : ITextBrick {
		float Angle { get; set; }
	}
	[Obsolete("This interface is now obsolete. You should use the IImageBrick interface instead.")]
	public interface IImageObjectBrick : IImageBrick {
		int ImageIndex { get; set; }
		object Images { get; set; }
	}
#endif
	public interface ILink {
		void CreateDocument();
		void CreateDocument(bool buildPagesInBackground);
		IPrintingSystem PrintingSystem { get; }
	}
	[System.Runtime.InteropServices.ComVisible(false)]
	public interface IBasePrintableProvider {
		object GetIPrintableImplementer();
	}
	[System.Runtime.InteropServices.ComVisible(false)]
	public interface IBasePrintable {
		void Initialize(IPrintingSystem ps, ILink link);
		void Finalize(IPrintingSystem ps, ILink link);
		void CreateArea(string areaName, IBrickGraphics graph);
	}
	[System.Runtime.InteropServices.ComVisible(false)]
	public interface IPrintable : IBasePrintable {
		UserControl PropertyEditorControl { get; }
		bool CreatesIntersectedBricks { get; }
		bool HasPropertyEditor();
		bool SupportsHelp();
		void ShowHelp();
		void AcceptChanges();
		void RejectChanges();
	}
	[System.Runtime.InteropServices.ComVisible(false)]
	public interface IPrintableEx : IPrintable {
		void OnStartActivity();
		void OnEndActivity();
	}
	[System.Runtime.InteropServices.ComVisible(false)]
	public interface IPrintHeaderFooter {
		string InnerPageHeader { get; }
		string InnerPageFooter { get; }
		string ReportHeader { get; }
		string ReportFooter { get; }
	}
}
