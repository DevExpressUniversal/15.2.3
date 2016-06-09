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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	public class MapBubbleSettings : MapItemSettingsBase {
		public static readonly DependencyProperty MarkerTypeProperty = DependencyPropertyManager.Register("MarkerType",
			typeof(MarkerType), typeof(MapBubbleSettings), new PropertyMetadata(MarkerType.Circle, UpdateItems));
		public static readonly DependencyProperty CustomMarkerTemplateProperty = DependencyPropertyManager.Register("CustomMarkerTemplate",
			typeof(DataTemplate), typeof(MapBubbleSettings), new PropertyMetadata(null, UpdateItems));
		[Category(Categories.Layout)]
		public MarkerType MarkerType {
			get { return (MarkerType)GetValue(MarkerTypeProperty); }
			set { SetValue(MarkerTypeProperty, value); }
		}
		[Category(Categories.Layout)]
		public DataTemplate CustomMarkerTemplate {
			get { return (DataTemplate)GetValue(CustomMarkerTemplateProperty); }
			set { SetValue(CustomMarkerTemplateProperty, value); }
		}
		protected override MapDependencyObject CreateObject() {
			return new MapBubbleSettings();
		}
		protected override MapItem CreateItemInstance() {
			return new MapBubble();
		}
		protected override void FillPropertiesMappings(DependencyPropertyMappingDictionary mappings) {
			base.FillPropertiesMappings(mappings);
			mappings.Add(MapBubbleSettings.MarkerTypeProperty, MapBubble.MarkerTypeProperty);
			mappings.Add(MapBubbleSettings.CustomMarkerTemplateProperty, MapBubble.CustomMarkerTemplateProperty);
		}
	}
}
