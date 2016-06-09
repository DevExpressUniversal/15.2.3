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
using DevExpress.DemoData.Helpers;
namespace DevExpress.Xpf.DemoBase.Helpers {
	class TypeTemplateSelectorPair {
		public string TypeName { get; set; }
		public DataTemplate Template { get; set; }
	}
	[ContentProperty("Templates")]
	class TypeTemplateSelector : DataTemplateSelector {
		public TypeTemplateSelector() {
			Templates = new List<TypeTemplateSelectorPair>();
		}
		public List<TypeTemplateSelectorPair> Templates { get; private set; }
		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
			string typeName = item == null ? "NULL" : item.GetType().Name;
			foreach(TypeTemplateSelectorPair pair in Templates) {
				if(pair.TypeName == typeName) return pair.Template;
			}
			return null;
		}
	}
	class ConditionTemplateSelectorPair {
		public string Value { get; set; }
		public DataTemplate Template { get; set; }
	}
	[ContentProperty("Templates")]
	class ConditionTemplateSelector : DataTemplateSelector {
		public ConditionTemplateSelector() {
			Templates = new List<ConditionTemplateSelectorPair>();
		}
		public string Condition { get; set; }
		public List<ConditionTemplateSelectorPair> Templates { get; private set; }
		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
			object value = item == null ? null : BindingPathHelper.GetValue(item, Condition);
			string valueString = value == null ? "NULL" : value.ToString();
			foreach(ConditionTemplateSelectorPair pair in Templates) {
				if(pair.Value == valueString) return pair.Template;
			}
			return null;
		}
	}
}
