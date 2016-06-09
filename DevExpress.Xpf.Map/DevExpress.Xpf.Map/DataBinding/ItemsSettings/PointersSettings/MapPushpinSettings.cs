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

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	public class MapPushpinSettings : MapItemSettings {
		public static readonly DependencyProperty TemplateProperty = DependencyPropertyManager.Register("Template",
			typeof(ControlTemplate), typeof(MapPushpinSettings), new PropertyMetadata(MapPushpin.GetDefaultTemplate(), UpdateItems));
		public static readonly DependencyProperty MarkerTemplateProperty = DependencyPropertyManager.Register("MarkerTemplate",
			typeof(DataTemplate), typeof(MapPushpinSettings), new PropertyMetadata(MapPushpin.GetDefaultMarkerTemplate(), UpdateItems));
		public static readonly DependencyProperty LocationChangedAnimationProperty = DependencyPropertyManager.Register("LocationChangedAnimation",
			typeof(PushpinLocationAnimation), typeof(MapPushpinSettings), new PropertyMetadata(null, UpdateItems));
		public static readonly DependencyProperty TraceDepthProperty = DependencyPropertyManager.Register("TraceDepth",
			typeof(int), typeof(MapPushpinSettings), new PropertyMetadata(0, UpdateItems));
		public static readonly DependencyProperty TraceStrokeProperty = DependencyPropertyManager.Register("TraceStroke",
			typeof(Brush), typeof(MapPushpinSettings), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0xFF, 0xFF)), UpdateItems));
		public static readonly DependencyProperty TraceStrokeStyleProperty = DependencyPropertyManager.Register("TraceStrokeStyle",
			typeof(StrokeStyle), typeof(MapPushpinSettings), new PropertyMetadata(null, UpdateItems));
		public static readonly DependencyProperty BrushProperty = DependencyPropertyManager.Register("Brush",
			typeof(Brush), typeof(MapPushpinSettings), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x8A, 0xFB, 0xFF)), UpdateItems));
		public static readonly DependencyProperty TextBrushProperty = DependencyPropertyManager.Register("TextBrush",
			typeof(Brush), typeof(MapPushpinSettings), new PropertyMetadata(new SolidColorBrush(Colors.Black), UpdateItems));
		[Category(Categories.Presentation)]
		public ControlTemplate Template {
			get { return (ControlTemplate)GetValue(TemplateProperty); }
			set { SetValue(TemplateProperty, value); }
		}
		[Category(Categories.Behavior)]
		public PushpinLocationAnimation LocationChangedAnimation {
			get { return (PushpinLocationAnimation)GetValue(LocationChangedAnimationProperty); }
			set { SetValue(LocationChangedAnimationProperty, value); }
		}
		[Category(Categories.Appearance)]
		public int TraceDepth {
			get { return (int)GetValue(TraceDepthProperty); }
			set { SetValue(TraceDepthProperty, value); }
		}
		[Category(Categories.Appearance)]
		public StrokeStyle TraceStrokeStyle {
			get { return (StrokeStyle)GetValue(TraceStrokeStyleProperty); }
			set { SetValue(TraceStrokeStyleProperty, value); }
		}
		[Category(Categories.Appearance)]
		public Brush TraceStroke {
			get { return (Brush)GetValue(TraceStrokeProperty); }
			set { SetValue(TraceStrokeProperty, value); }
		}
		[Category(Categories.Appearance)]
		public DataTemplate MarkerTemplate {
			get { return (DataTemplate)GetValue(MarkerTemplateProperty); }
			set { SetValue(MarkerTemplateProperty, value); }
		}
		[Category(Categories.Appearance)]
		public Brush Brush {
			get { return (Brush)GetValue(BrushProperty); }
			set { SetValue(BrushProperty, value); }
		}
		[Category(Categories.Appearance)]
		public Brush TextBrush {
			get { return (Brush)GetValue(TextBrushProperty); }
			set { SetValue(TextBrushProperty, value); }
		}
		protected override MapDependencyObject CreateObject() {
			return new MapPushpinSettings();
		}
		protected override MapItem CreateItemInstance() {
			return new MapPushpin();
		}
		protected override void FillPropertiesMappings(DependencyPropertyMappingDictionary mappings) {
			base.FillPropertiesMappings(mappings);
			mappings.Add(MapPushpinSettings.TemplateProperty, MapPushpin.TemplateProperty);
			mappings.Add(MapPushpinSettings.MarkerTemplateProperty, MapPushpin.MarkerTemplateProperty);
			mappings.Add(MapPushpinSettings.LocationChangedAnimationProperty, MapPushpin.LocationChangedAnimationProperty);
			mappings.Add(MapPushpinSettings.TraceDepthProperty, MapPushpin.TraceDepthProperty);
			mappings.Add(MapPushpinSettings.TraceStrokeProperty, MapPushpin.TraceStrokeProperty);
			mappings.Add(MapPushpinSettings.TraceStrokeStyleProperty, MapPushpin.TraceStrokeStyleProperty);
			mappings.Add(MapPushpinSettings.BrushProperty, MapPushpin.BrushProperty);
			mappings.Add(MapPushpinSettings.TextBrushProperty, MapPushpin.TextBrushProperty);
		}
	}
}
