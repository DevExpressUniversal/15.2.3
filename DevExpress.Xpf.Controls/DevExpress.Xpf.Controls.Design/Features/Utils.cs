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

#if SILVERLIGHT
extern alias SL;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Policies;
using Microsoft.Windows.Design.Services;
using DevExpress.Xpf.Controls.Design.Features;
#if SILVERLIGHT
using SL::DevExpress.Xpf.Core.Design;
using SL::DevExpress.Xpf.Controls;
using SL::DevExpress.Xpf.Controls.Internal;
using SL::DevExpress.Xpf.WindowsUI;
using AssemblyInfo = SL::AssemblyInfo;
#else
using System.ComponentModel;
using DevExpress.Xpf.WindowsUI.Navigation;
using DevExpress.Xpf.Core.Design;
using DevExpress.Xpf.WindowsUI;
using Microsoft.Windows.Design.PropertyEditing;
using DevExpress.Xpf.Navigation;
#endif
namespace DevExpress.Xpf.Controls.Design {
	internal class PropertyUtil {
		internal static void InvalidateProperty(ModelItem item, PropertyIdentifier propertyIdentifier) {
			item.Context.Services.GetRequiredService<ValueTranslationService>().InvalidateProperty(item, propertyIdentifier);
		}
	}
	internal static class DesignerState {
		class DesignTimeStateDictionary : Dictionary<object, object> { }
		class DesignerStateDictionary : Dictionary<ModelItem, DesignTimeStateDictionary> { }
		public static void ClearDesigneTimeProperty<T>(this ModelItem item, DesigneTimeProperty<T> property) {
			DesignerStateDictionary dictionary = item.Context.Services.GetRequiredService<DesignerStateDictionary>();
			if(dictionary == null) return;
			DesignTimeStateDictionary table;
			if(dictionary.TryGetValue(item, out table))
				table.Remove(property);
			if(table != null && table.Count == 0)
				dictionary.Remove(item);
		}
		public static T GetDesignTimeProperty<T>(this ModelItem item, DesigneTimeProperty<T> property) {
			return GetDesignTimeProperty<T>(item, property, null);
		}
		public static T GetDesignTimeProperty<T>(this ModelItem item, DesigneTimeProperty<T> property, Func<ModelItem, T> factory) {
			DesignerStateDictionary dictionary = item.Context.Services.GetService<DesignerStateDictionary>();
			if(dictionary == null) {
				dictionary = new DesignerStateDictionary();
				item.Context.Services.Publish<DesignerStateDictionary>(dictionary);
			}
			DesignTimeStateDictionary table;
			if(dictionary.TryGetValue(item, out table)) {
				object value;
				if(table.TryGetValue(property, out value)) {
					return (T)value;
				}
				else if(factory != null) {
					T v = factory(item);
					table[property] = v;
					return v;
				}
			}
			return default(T);
		}
		public static void SetDesignTimeProperty<T>(this ModelItem item, DesigneTimeProperty<T> property, T value) {
			DesignerStateDictionary dictionary = item.Context.Services.GetService<DesignerStateDictionary>();
			if(dictionary == null) {
				dictionary = new DesignerStateDictionary();
				item.Context.Services.Publish<DesignerStateDictionary>(dictionary);
			}
			DesignTimeStateDictionary table;
			if(!dictionary.TryGetValue(item, out table)) {
				table = new DesignTimeStateDictionary();
				dictionary[item] = table;
			}
			table[property] = value;
		}
	}
	internal class DesigneTimeProperty<T> {
		string Name { get; set; }
		internal DesigneTimeProperty(string name) {
			Name = name;
		}
		public override string ToString() {
			return Name;
		}
	}
}
