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
using System.ComponentModel;
using System.Diagnostics;
using DevExpress.Mvvm;
using System.Windows.Input;
using System.Collections;
using DevExpress.Utils.Design;
using DevExpress.Design.Mvvm;
using DevExpress.Mvvm.UI.Native.ViewGenerator;
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.ViewModelData {
	public class CommandInfo {
		public string CommandPropertyName { get; set; }
		public string Caption { get; set; }
		public string ParameterPropertyName { get; set; }
		public bool HasGlyphs() {
			return !string.IsNullOrEmpty(LargeGlyph) && !string.IsNullOrEmpty(SmallGlyph);
		}
		public bool HasParameter() {
			return !string.IsNullOrEmpty(ParameterPropertyName);
		}
		string smallGlyphUri;
		public string SmallGlyph { get { return GetGlyph(smallGlyphUri); } }
		string largeGlyphUri;
		public string LargeGlyph { get { return GetGlyph(largeGlyphUri); }  }
		public void SetSmallGlyphUri(string uri) {
			smallGlyphUri = uri;
		}
		public void SetLargeGlyphUri(string uri) {
			largeGlyphUri = uri;
		}
		static string GetGlyph(string uri) {
			if(uri != null && uri.StartsWith(ViewModelMetadataSource.ImagesImagePrefix)) {
				string imageName = uri.Split('/').Last();
				string suffix;
				switch(Images.ImageCollectionHelper.GetImageType(uri)) {
					case ImageType.GrayScaled: suffix = "Grayscale"; break;
					case ImageType.Office2013: suffix = "Office2013"; break;
					default: suffix = string.Empty; break;
				}
				return string.Format("{{dx:DXImage{0} Image={1}}}", suffix, imageName);
			}
			return uri;
		}
	}
}
