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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Policies;
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
using DevExpress.Xpf.Core.Design.Services;
using DevExpress.Xpf.Core.Design;
using DevExpress.Xpf.WindowsUI;
using Microsoft.Windows.Design.PropertyEditing;
using DevExpress.Xpf.Navigation;
#endif
namespace DevExpress.Xpf.Controls.Design {
	class PageViewDesignModeValueProvider : DesignModeValueProvider {
		public static readonly DesigneTimeProperty<int?> DesignTimeSelectedIndexProperty = new DesigneTimeProperty<int?>("DesignTimeSelectedIndex");
		public static readonly PropertyIdentifier SelectedIndexPropertyIdentifier = new PropertyIdentifier(typeof(PageView), "SelectedIndex");
		public static void SetDesignTimeSelectedIndex(ModelItem item, int? SelectedIndex) {
			item.SetDesignTimeProperty(PageViewDesignModeValueProvider.DesignTimeSelectedIndexProperty, SelectedIndex);
			PropertyUtil.InvalidateProperty(item, SelectedIndexPropertyIdentifier);
		}
		public static int? GetDesignTimeSelectedIndex(ModelItem item) {
			return item.GetDesignTimeProperty(DesignTimeSelectedIndexProperty);
		}
		public PageViewDesignModeValueProvider() {
			Properties.Add(SelectedIndexPropertyIdentifier);
		}
		public override object TranslatePropertyValue(ModelItem item, PropertyIdentifier identifier, object value) {
			if(identifier == SelectedIndexPropertyIdentifier) {
				int? isSelected = GetDesignTimeSelectedIndex(item);
				if(isSelected.HasValue) return isSelected.Value;
			}
			return base.TranslatePropertyValue(item, identifier, value);
		}
	}
}
