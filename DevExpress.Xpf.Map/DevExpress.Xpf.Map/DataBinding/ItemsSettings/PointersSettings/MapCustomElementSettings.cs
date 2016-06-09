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
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	public class MapCustomElementSettings : MapItemSettings {
		public static readonly DependencyProperty TemplateProperty = DependencyPropertyManager.Register("Template",
			typeof(ControlTemplate), typeof(MapCustomElementSettings), new PropertyMetadata(MapCustomElement.GetDefaultTemplate(), UpdateItems));
		public static readonly DependencyProperty ContentTemplateProperty = DependencyPropertyManager.Register("ContentTemplate",
		   typeof(DataTemplate), typeof(MapCustomElementSettings), new PropertyMetadata(null, ContentTemplateChanged));
		public static readonly DependencyProperty ContentTemplateSelectorProperty = DependencyPropertyManager.Register("ContentTemplateSelector",
		   typeof(DataTemplateSelector), typeof(MapCustomElementSettings), new PropertyMetadata(null, ContentTemplateChanged));
		static void ContentTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapCustomElementSettings settings = d as MapCustomElementSettings;
			if(settings != null)
				settings.ApplyItemsContentTemplate();
		}
		[Category(Categories.Presentation)]
		public DataTemplate ContentTemplate {
			get { return (DataTemplate)GetValue(ContentTemplateProperty); }
			set { SetValue(ContentTemplateProperty, value); }
		}
		[Category(Categories.Presentation)]
		public ControlTemplate Template {
			get { return (ControlTemplate)GetValue(TemplateProperty); }
			set { SetValue(TemplateProperty, value); }
		}
		[Category(Categories.Data)]
		public DataTemplateSelector ContentTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty); }
			set { SetValue(ContentTemplateSelectorProperty, value); }
		}
		void ApplyItemsContentTemplate() {
			if(Listener != null)
				Listener.OnSourceChanged();
		}
		protected override MapDependencyObject CreateObject() {
			return new MapCustomElementSettings();
		}
		protected override MapItem CreateItemInstance() {
			return new MapCustomElement();
		}
		protected internal override void ApplySource(MapItem mapItem, object source) {
			MapCustomElement customElement = (MapCustomElement)mapItem;
			customElement.Content = source;
			if(ContentTemplateSelector != null)
				customElement.ContentTemplate = ContentTemplateSelector.SelectTemplate(source, mapItem);
			else
				customElement.ContentTemplate = ContentTemplate;
		}
		protected override void FillPropertiesMappings(DependencyPropertyMappingDictionary mappings) {
			base.FillPropertiesMappings(mappings);
			mappings.Add(MapCustomElementSettings.TemplateProperty, MapCustomElement.TemplateProperty);
		}
	}
}
