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

using System.Drawing;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils;
using DevExpress.XtraPrinting.HtmlExport.Controls;
using DevExpress.XtraPrinting.HtmlExport.Native;
using DevExpress.XtraPrinting.HtmlExport;
namespace DevExpress.XtraPrinting.Export.Web {
	public class HtmlHelper {
		public const string WatermarkMouseDownScript = "if(!event || !event.target) { return; } event.target.style.display = 'none'; var lowerDiv = document.elementFromPoint(event.clientX, event.clientY); event.target.style.display = 'block'; if(!lowerDiv) { return; } var newEvent = document.createEvent('MouseEvent'); newEvent.initMouseEvent('mousedown', true, true, window, 0, event.screenX, event.screenY, event.clientX, event.clientY, false, false, false, false, 0, null); lowerDiv.dispatchEvent(newEvent);";
		public static string GetHtmlUrl(string url) {
			return "#" + url;
		}
		public static void SetStyleSize(DXCssStyleCollection style, Size size) {
			SetStyleWidth(style, size.Width);
			SetStyleHeight(style, size.Height);
		}
		public static void SetStyleWidth(DXCssStyleCollection style, int width) {
			style.Add("width", HtmlConvert.ToHtml(width));
		}
		public static void SetStyleHeight(DXCssStyleCollection style, int height) {
			style.Add("height", HtmlConvert.ToHtml(height));
		}
#region image
		public static ClipControl SetClip(DXHtmlControl parent, Point offset, Size clipSize, Size originalSize) {
			ClipControl clipControl = SetClip(parent, offset, clipSize);
			clipControl.SetOriginalSize(originalSize);
			return clipControl;
		}
		public static ClipControl SetClip(DXHtmlControl parent, Point offset, Size outerControlSize) {
			ClipControl clipControl = parent.Controls.Count == 1 ? parent.Controls[0] as ClipControl : null;
			if(clipControl != null) {
				clipControl.SetClipSize(outerControlSize);
				clipControl.AddOffset(offset);
			} else {
				clipControl = new ClipControl(parent, offset, outerControlSize);
				for(int i = parent.Controls.Count - 1; i >= 0; i--)
					clipControl.InnerContainer.Controls.AddAt(0, parent.Controls[i]);
				parent.Controls.Add(clipControl);
			}
			return clipControl;
		}
#endregion
	}
	public class HtmlCellTextContentCreator {
		readonly DXHtmlContainerControl cell;
		public HtmlCellTextContentCreator(DXHtmlContainerControl cell) {
			Guard.ArgumentNotNull(cell, "cell");
			this.cell = cell;
		}
		public void CreateContent(string text, BrickStyle style, RectangleF bounds, Measurer measurer) {
			float dpi = GraphicsDpi.UnitToDpiI(GraphicsUnit.Pixel);
			text = HotkeyPrefixHelper.PreprocessHotkeyPrefixesInString(text, style);
			RectangleF clientRectangle = style.DeflateBorderWidth(bounds, dpi);
			clientRectangle.Size = style.Padding.Deflate(clientRectangle.Size, dpi);
			SetCellContent(text, style.StringFormat.Value, style.TextAlignment, style.Font, clientRectangle, measurer);
		}
		void SetCellContent(string text, StringFormat stringFormat, TextAlignment textAlignment, Font font, RectangleF bounds, Measurer measurer) {
			HtmlStringCreator htmlStringCreator = CreateHtmlStringCreator(textAlignment, stringFormat, bounds);
			htmlStringCreator.AddHtmlControls(cell, text, measurer, font);
		}
		static HtmlStringCreator CreateHtmlStringCreator(TextAlignment textAlignment, StringFormat stringFormat, RectangleF bounds) {
			if (IsJustified(textAlignment))
				return new JustifyHtmlStringCreator(stringFormat, bounds.Width, bounds.Height);
			else
				return new HtmlStringCreator(stringFormat, bounds.Width, bounds.Height);
		}
		static bool IsJustified(TextAlignment textAlignment) {
			return textAlignment == TextAlignment.TopJustify || textAlignment == TextAlignment.MiddleJustify || textAlignment == TextAlignment.BottomJustify;
		}
	}
	[System.ComponentModel.ToolboxItem(false)]
	public class ClipControl : DXWebControlBase {
		Point offset;
		Size clipSize;
		Size originalSize;
		DXHtmlControl parent;
		public string InnerControlCSSClass;
		DXWebControlBase innerContainer;
		public DXWebControlBase InnerContainer {
			get { return innerContainer; }
		}
		public ClipControl(DXHtmlControl parent, Point offset, Size clipSize) {
			this.offset = offset;
			this.clipSize = clipSize;
			this.parent = parent;
			originalSize = Size.Empty;
			innerContainer = new DXWebControlBase();
		}
		public void AddOffset(Point additionalOffset) {
			offset.Offset(additionalOffset);
		}
		public void SetClipSize(Size size) {
			clipSize = size;
		}
		public void SetOriginalSize(Size size) {
			originalSize = size;
		}
		public override void RenderControl(DXHtmlTextWriter writer) {
			CreateChildControls();
			base.Render(writer);
		}
		protected internal override void CreateChildControls() {
			base.CreateChildControls();
			DXHtmlGenericControl outerDiv = new DXHtmlGenericControl(DXHtmlTextWriterTag.Div);
			outerDiv.Style.Add("overflow", "hidden");
			HtmlHelper.SetStyleSize(outerDiv.Style, clipSize);
			this.Controls.Add(outerDiv);
			DXHtmlControl innerDiv = outerDiv;
			if(offset != Point.Empty || originalSize != Size.Empty) {
				if(innerContainer.Controls.Count == 1 && innerContainer.Controls[0] is DXHtmlImage)
					ApplyStyle(innerContainer.Controls[0] as DXHtmlImage);
				else {
					innerDiv = new DXHtmlGenericControl(DXHtmlTextWriterTag.Div);
					ApplyStyle(innerDiv);
					CopyParentStyle(innerDiv);
					if(originalSize != Size.Empty)
						HtmlHelper.SetStyleSize(innerDiv.Style, originalSize);
					innerDiv.Style.Add("overflow", "hidden");
					outerDiv.Controls.Add(innerDiv);
				}
			}
			innerDiv.Controls.Add(innerContainer);
		}
		void ApplyStyle(DXHtmlControl control) {
			if(offset.Y != 0)
				control.Style.Add("margin-top", HtmlConvert.ToHtml(offset.Y));
			if(offset.X != 0)
				control.Style.Add("margin-left", HtmlConvert.ToHtml(offset.X));
		}
		void CopyParentStyle(DXHtmlControl control) {
			if(!string.IsNullOrEmpty(InnerControlCSSClass)) {
				control.Attributes["class"] = InnerControlCSSClass;
				control.Style.Add("text-align", parent.Style["text-align"]);
				control.Style.Add("vertical-align", parent.Style["vertical-align"]);
				control.Style.Add("line-height", parent.Style["line-height"]);
			}
		}
	}
}
