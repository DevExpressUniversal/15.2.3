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
using System.Windows.Controls;
using System.Windows;
namespace DevExpress.Xpf.Core.Native {
	public class ActualTemplateSelectorWrapper : DataTemplateSelector {
		public static ActualTemplateSelectorWrapper Combine(ActualTemplateSelectorWrapper source, DataTemplateSelector selector, DataTemplate template) {
			if((selector == null && template == null) || source == null)
				return source;
			return new ActualTemplateSelectorWrapper(selector ?? source.Selector, template ?? source.Template);
		}
		DataTemplateSelector selector;
		DataTemplate template;
		public DataTemplate Template { get { return template; } }
		public DataTemplateSelector Selector { get { return selector; } }
		public ActualTemplateSelectorWrapper(DataTemplateSelector selector, DataTemplate template) {
			this.selector = selector;
			this.template = template;
		}
		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
			if(selector == null)
				return template;
			DataTemplate selectedTemplate = selector.SelectTemplate(item, container);
			return selectedTemplate != null ? selectedTemplate : template;
		}
	}
	public static class ListHelper {
		public static bool AreEqual<T>(IList<T> list1, IList<T> list2) {
			if(list1.Count != list2.Count)
				return false;
			for(int i = 0; i < list1.Count; i++) {
				if(!object.ReferenceEquals(list1[i], list2[i]))
					return false;
			}
			return true;
		}
		public static T Find<T>(IList<T> list, Predicate<T> match) {
			if(match == null) {
				throw new ArgumentNullException("match");
			}
			for(int i = 0; i < list.Count; i++) {
				if(match(list[i])) {
					return list[i];
				}
			}
			return default(T);
		}
	}
}
