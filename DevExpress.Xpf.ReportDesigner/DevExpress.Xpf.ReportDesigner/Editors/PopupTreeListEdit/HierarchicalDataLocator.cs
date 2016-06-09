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
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Internal;
using DevExpress.Xpf.Reports.UserDesigner.Native;
using DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtensions;
using DevExpress.XtraReports.Parameters;
namespace DevExpress.Xpf.Reports.UserDesigner.Editors {
	public class HierarchicalDataLocator {
		readonly IEnumerable<object> data;
		readonly string valueProperty;
		readonly string childNodesProperty;
		readonly string expandNodeProperty;
		readonly IHierarchicalPathProvider pathProvider;
		[ThreadStatic]
		static ReflectionHelper reflectionHelper;
		static ReflectionHelper ReflectionHelper {
			get {
				return reflectionHelper ?? (reflectionHelper = new ReflectionHelper());
			}
		}
		public HierarchicalDataLocator(IEnumerable<object> data, string valueProperty, string childNodesProperty, string expandNodeProperty, IHierarchicalPathProvider pathProvider) {
			this.data = data;
			this.valueProperty = valueProperty;
			this.childNodesProperty = childNodesProperty;
			this.expandNodeProperty = expandNodeProperty;
			this.pathProvider = pathProvider;
		}
		public object FindItemByValue(object value) {
			if(value == null || data == null)
				return null;
			object node = null;
			IEnumerable<object> currentLevel = data;
			IEnumerable<HierarchicalPath> pathProgression = pathProvider.GetItemPath(value).GetProgression();
			foreach(var currentPath in pathProgression) {
				node = currentLevel.Where(x => currentPath.Equals(GetItemPath(x))).SingleOrDefault();
				if(node == null)
					break;
				if(currentPath != pathProgression.Last()) {
					currentLevel = GetChildNodes(node);
				}
			}
			return node;
		}
		HierarchicalPath GetItemPath(object node) {
			object itemValue = ReflectionHelper.GetPropertyValueByPath(node, valueProperty);
			return pathProvider.GetItemPath(itemValue);
		}
		IEnumerable<object> GetChildNodes(object node) {
			if(!string.IsNullOrEmpty(expandNodeProperty)) {
				ReflectionHelper.SetPropertyValue(node, expandNodeProperty, true);
			}
			return (IEnumerable<object>)ReflectionHelper.GetPropertyValueByPath(node, childNodesProperty);
		}
	}
}
