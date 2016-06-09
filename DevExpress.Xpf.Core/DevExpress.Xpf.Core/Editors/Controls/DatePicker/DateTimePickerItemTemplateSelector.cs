﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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

using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.Editors {
	public class DateTimePickerItemTemplateSelector : DataTemplateSelector {
		readonly IDictionary<DateTimePart, DataTemplate> cache;
		const string DateTimePickerKey = "ItemTemplate";
		public DateTimePickerItemTemplateSelector() {
			cache = new Dictionary<DateTimePart, DataTemplate>();
		}
		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
			var picker = container as DateTimePickerSelector;
			var data = item as DateTimePickerData;
			if (picker == null || data == null)
				return base.SelectTemplate(item, container);
			DataTemplate dataTemplate;
			if (cache.TryGetValue(data.DateTimePart, out dataTemplate))
				return dataTemplate;
			string key = data.DateTimePart + DateTimePickerKey;
			dataTemplate = (DataTemplate)picker.FindResource(new DateTimePickerThemeKeyExtension() { ResourceKey = (DateTimePickerThemeKeys)Enum.Parse(typeof(DateTimePickerThemeKeys), key) });
			cache.Add(data.DateTimePart, dataTemplate);
			return dataTemplate;
		}
	}
}
