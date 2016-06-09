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

using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
namespace DevExpress.Xpf.Docking.Design {
	class LayoutGroupDesignModeValueProvider : DesignModeValueProvider {
		public static readonly DesigneTimeProperty<int> DesignTimeSelectedTabIndexProperty = new DesigneTimeProperty<int>("DesignTimeSelectedTabIndex");
		public static readonly PropertyIdentifier SelectedTabIndexPropertyIdentifier = new PropertyIdentifier(typeof(LayoutGroup), "SelectedTabIndex");
		public static void SetDesignTimeSelectedTabIndex(ModelItem item, int index) {
			item.SetDesignTimeProperty(LayoutGroupDesignModeValueProvider.DesignTimeSelectedTabIndexProperty, index);
			PropertyUtil.InvalidateProperty(item, SelectedTabIndexPropertyIdentifier);
		}
		public static int GetDesignTimeSelectedTabIndex(ModelItem item) {
			return item.GetDesignTimeProperty(DesignTimeSelectedTabIndexProperty);
		}
		public LayoutGroupDesignModeValueProvider() {
			Properties.Add(SelectedTabIndexPropertyIdentifier);
		}
		public override object TranslatePropertyValue(ModelItem item, PropertyIdentifier identifier, object value) {
			if(identifier == SelectedTabIndexPropertyIdentifier) {
				return GetDesignTimeSelectedTabIndex(item);
			}
			return base.TranslatePropertyValue(item, identifier, value);
		}
	}
}
