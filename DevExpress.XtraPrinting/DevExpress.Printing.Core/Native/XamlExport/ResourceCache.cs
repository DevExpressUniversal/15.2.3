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
using System.Collections.ObjectModel;
namespace DevExpress.XtraPrinting.XamlExport {
	public class ResourceCache {
		const string watermarkResourceName = "Watermark";
		Collection<XamlBorderStyle> borderStyles = new Collection<XamlBorderStyle>();
		Collection<XamlLineStyle> borderDashStyles = new Collection<XamlLineStyle>();
		Collection<XamlTextBlockStyle> textBlockStyles = new Collection<XamlTextBlockStyle>();
		Collection<XamlLineStyle> lineStyles = new Collection<XamlLineStyle>();
		Collection<ImageResource> imageResources = new Collection<ImageResource>();
		public IEnumerable<XamlBorderStyle> BorderStyles { get { return borderStyles; } }
		public IEnumerable<XamlLineStyle> BorderDashStyles { get { return borderDashStyles; } }
		public IEnumerable<XamlTextBlockStyle> TextBlockStyles { get { return textBlockStyles; } }
		public IEnumerable<XamlLineStyle> LineStyles { get { return lineStyles; } }
		public IEnumerable<ImageResource> ImageResources { get { return imageResources; } }
		public string RegisterBorderStyle(XamlBorderStyle style) {
			return RegisterResourceCore(style, borderStyles, GenerateBorderStyleName);
		}
		public string RegisterBorderDashStyle(XamlLineStyle style) {
			return RegisterResourceCore(style, borderDashStyles, GenerateBorderDashStyleName);
		}
		public string RegisterTextBlockStyle(XamlTextBlockStyle style) {
			return RegisterResourceCore(style, textBlockStyles, GenerateTextBlockStyleName);
		}
		public string RegisterLineStyle(XamlLineStyle style) {
			return RegisterResourceCore(style, lineStyles, GenerateLineStyleName);
		}
		public string RegisterImageResource(ImageResource resource) {
			return RegisterResourceCore(resource, imageResources, GenerateImageName);
		}
		static string RegisterResourceCore<T>(T resource, Collection<T> resourceCollection, Func<string> generateStyleName) where T: XamlResourceBase {
			string resourceName;
			if(!resourceCollection.Contains(resource)) {
				resourceName = generateStyleName();
				resource.SetName(resourceName);
				resourceCollection.Add(resource);
			} else {
				resourceName = resourceCollection[resourceCollection.IndexOf(resource)].Name;
			}
			return resourceName;
		}
		public string RegisterWatermarkImageResource(ImageResource resource) {
			imageResources.Add(resource);
			resource.SetName(watermarkResourceName);
			return watermarkResourceName;
		}
		string GenerateBorderStyleName() {
			return string.Format("BorderStyle{0}", borderStyles.Count + 1);
		}
		string GenerateBorderDashStyleName() {
			return string.Format("BorderDashStyle{0}", borderDashStyles.Count + 1);
		}
		string GenerateTextBlockStyleName() {
			return string.Format("TextBlockStyle{0}", textBlockStyles.Count + 1);
		}
		string GenerateLineStyleName() {
			return string.Format("LineStyle{0}", lineStyles.Count + 1);
		}
		string GenerateImageName() {
			return string.Format("Image{0}", imageResources.Count + 1);
		}
	}
}
