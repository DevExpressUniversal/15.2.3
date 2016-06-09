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
using System.Linq;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Xpf.Core.Internal;
using DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtensions;
namespace DevExpress.Xpf.Reports.UserDesigner.Editors {
	public class PopupTreeListEditItemsProvider {
		readonly IPopupTreeListEditItemsProviderOwner owner;
		[ThreadStatic]
		static ReflectionHelper reflectionHelper;
		static ReflectionHelper ReflectionHelper {
			get {
				return reflectionHelper ?? (reflectionHelper = new ReflectionHelper());
			}
		}
		public PopupTreeListEditItemsProvider(IPopupTreeListEditItemsProviderOwner owner) {
			Guard.ArgumentNotNull(owner, "owner");
			this.owner = owner;
		}
		public object GetItemByValue(object value) {
			var locator = new HierarchicalDataLocator((IEnumerable<object>)owner.ItemsSource, owner.ValueMember, owner.ChildNodesPath, null, owner.HierarchicalPathProvider);
			var item = locator.FindItemByValue(value);
			return item;
		}
		public object GetDisplayValue(object item) {
			if(item == null)
				return null;
			if(string.IsNullOrEmpty(owner.DisplayMember))
				return item;
			return ReflectionHelper.GetPropertyValue(item, owner.DisplayMember);
		}
		public object GetValue(object item) {
			if(item == null)
				return null;
			if(string.IsNullOrEmpty(owner.ValueMember))
				return item;
			return ReflectionHelper.GetPropertyValue(item, owner.ValueMember);
		}
	}
}
