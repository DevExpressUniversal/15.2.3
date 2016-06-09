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
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Data;
namespace DevExpress.Xpf.NavBar {
	internal static class SettingsProvider {
		public static void SetPropertiesBinding(Image image, object imageFallbackSize, Control content, object settingsSource) {
			SetImagePropertiesBinding(image, imageFallbackSize, settingsSource);
			SetTextPropertiesBinding(content, settingsSource);
		}
		static void SetImagePropertiesBinding(Image image, object imageFallbackSize, object settingsSource) {
			if (image == null)
				return;
			image.SetBinding(Image.HeightProperty, new Binding() { Path = new PropertyPath("(0).Height", NavBarViewBase.ImageSettingsProperty), Source = settingsSource, FallbackValue = imageFallbackSize });
			image.SetBinding(Image.WidthProperty, new Binding() { Path = new PropertyPath("(0).Width", NavBarViewBase.ImageSettingsProperty), Source = settingsSource, FallbackValue = imageFallbackSize });
			image.SetBinding(Image.StretchProperty, new Binding() { Path = new PropertyPath("(0).Stretch", NavBarViewBase.ImageSettingsProperty), Source = settingsSource, FallbackValue = Stretch.Uniform });
			image.SetBinding(Image.StretchDirectionProperty, new Binding() { Path = new PropertyPath("(0).StretchDirection", NavBarViewBase.ImageSettingsProperty), Source = settingsSource, FallbackValue = StretchDirection.Both });
			image.SetBinding(Image.HorizontalAlignmentProperty, new Binding() { Path = new PropertyPath("(0).ImageHorizontalAlignment", NavBarViewBase.LayoutSettingsProperty), Source = settingsSource, FallbackValue = HorizontalAlignment.Left });
			image.SetBinding(Image.VerticalAlignmentProperty, new Binding() { Path = new PropertyPath("(0).ImageVerticalAlignment", NavBarViewBase.LayoutSettingsProperty), Source = settingsSource, FallbackValue = VerticalAlignment.Center });
		}
		static void SetTextPropertiesBinding(Control content, object settingsSource) {
			if (content == null)
				return;
			content.SetBinding(ContentControl.VerticalAlignmentProperty, new Binding() { Path = new PropertyPath("(0).TextVerticalAlignment", NavBarViewBase.LayoutSettingsProperty), Source = settingsSource, FallbackValue = VerticalAlignment.Center });
			content.SetBinding(ContentControl.HorizontalAlignmentProperty, new Binding() { Path = new PropertyPath("(0).TextHorizontalAlignment", NavBarViewBase.LayoutSettingsProperty), Source = settingsSource, FallbackValue = HorizontalAlignment.Stretch });
			content.SetBinding(ContentControl.VerticalContentAlignmentProperty, new Binding() { Path = new PropertyPath("(0).TextVerticalAlignment", NavBarViewBase.LayoutSettingsProperty), Source = settingsSource, FallbackValue = VerticalAlignment.Center });
			content.SetBinding(ContentControl.HorizontalContentAlignmentProperty, new Binding() { Path = new PropertyPath("(0).TextHorizontalAlignment", NavBarViewBase.LayoutSettingsProperty), Source = settingsSource, FallbackValue = HorizontalAlignment.Stretch });
		}
	}
}
