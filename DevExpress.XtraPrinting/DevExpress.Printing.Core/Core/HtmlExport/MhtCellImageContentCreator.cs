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
using System.Text;
using DevExpress.XtraPrinting.HtmlExport.Controls;
using System.Drawing;
namespace DevExpress.XtraPrinting.Export.Web {
	class MhtCellImageContentCreator : HtmlCellImageContentCreator {
		IScriptContainer scriptContainer;
		public MhtCellImageContentCreator(IImageRepository imageRepository, IScriptContainer scriptContainer)
			: base(imageRepository) {
			this.scriptContainer = scriptContainer;
		}
		protected override void ProcessImage(DXWebControlBase imgContainer, DXWebControlBase imageControl, Image image) {
			imgContainer.Controls.Add(imageControl);
			DXHtmlImage htmlImage = imageControl as DXHtmlImage;
			if(htmlImage == null || !IsPng(image))
				return;
			htmlImage.Style.Add("filter", "expression(fixPng(this))");
			string s =
				@"function fixPng(element) {" +
				  @"if(/MSIE (5\.5|6).+Win/.test(navigator.userAgent)) {" +
					@"if(element.tagName=='IMG' && /\.png$/.test(element.src)) {" +
						"var src = partlyEscape(element.src);" +
						"element.src = '" + GetBlankGifSrc() + "';" +
						"element.style.filter = \"progid:DXImageTransform.Microsoft.AlphaImageLoader(src='\" + src + \"',sizingMethod='scale')\";" +
					"}" +
				  "}" +
				"}" +
				@"function partlyEscape(s) {" +
					@"var parts = s.split('!');" +
					@"var arr = parts[0].split(/[\\\/]/);" +
					@"for(var i = 3; i < arr.length; i++)" +
						@"arr[i] = escape(arr[i]);" +
					@"return arr.join('/') + '!' + parts[1];" +
				"}";
			if(!scriptContainer.IsClientScriptBlockRegistered("fixPng"))
				scriptContainer.RegisterClientScriptBlock("fixPng", s);
		}
		protected override bool ValidateImageSrc(Image image, ref string imageSrc) {
			imageSrc = imageRepository.GetImageSource(image, false);
			return !string.IsNullOrEmpty(imageSrc);
		}
	}
}
