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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
namespace DevExpress.Xpf.Core.Native {
	public class TemplatesDictionary : Dictionary<string, DataTemplate> { }
	[ContentProperty("Templates")]
	public class TypeTemplateSelector : DataTemplateSelector {
		public bool FindDescendants { get; set; }
		public TypeTemplateSelector() {
			Templates = new TemplatesDictionary();
		}
		public TemplatesDictionary Templates { get; set; }
		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
			DataTemplate template;
			if(item == null) {
				return Templates.TryGetValue("NULL", out template) ? null : template;
			}
			Type itemType = item.GetType();
			do {
				if(Templates.TryGetValue(itemType.Name, out template))
					return template;
				itemType = FindDescendants ? itemType.BaseType : null;
			} while(itemType != null);
			return null;
		}
	}
#if !SL
	public class StylesDictionary : Dictionary<string, Style> { }
	[ContentProperty("Styles")]
	public class TypeStyleSelector : StyleSelector {
		public TypeStyleSelector() {
			Styles = new StylesDictionary();
		}
		public StylesDictionary Styles { get; set; }
		public override Style SelectStyle(object item, DependencyObject container) {
			string typeName = item == null ? "NULL" : item.GetType().Name;
			Style style;
			if(!Styles.TryGetValue(typeName, out style))
				style = null;
			return style;
		}
	}
#endif
}
