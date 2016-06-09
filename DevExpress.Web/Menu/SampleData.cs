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

using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Internal {
	public class MenuHierarchicalSampleData : HierarchicalSampleData {
		public MenuHierarchicalSampleData(int depth, string path)
			: base(depth, path) {
		}
		protected override void CreateSampleNodes(int depth, string path) {
			if(depth == 0) { 
				List.Add(new MenuHierarchicalSampleDataNode(string.Format(StringResources.MenuHierarchicalSampleData_SampleRootItem, 1), depth, path));
				List.Add(new MenuHierarchicalSampleDataNode(string.Format(StringResources.MenuHierarchicalSampleData_SampleRootItem, 2), depth, path));
				List.Add(new MenuHierarchicalSampleDataNode(string.Format(StringResources.MenuHierarchicalSampleData_SampleRootItem, 3), depth, path));
				List.Add(new MenuHierarchicalSampleDataNode(string.Format(StringResources.MenuHierarchicalSampleData_SampleRootItem, 4), depth, path));
			}
			else if(depth == 1) { 
				List.Add(new MenuHierarchicalSampleDataNode(string.Format(StringResources.MenuHierarchicalSampleData_SampleItem, 1), depth, path));
				List.Add(new MenuHierarchicalSampleDataNode(string.Format(StringResources.MenuHierarchicalSampleData_SampleItem, 2), depth, path));
				List.Add(new MenuHierarchicalSampleDataNode(string.Format(StringResources.MenuHierarchicalSampleData_SampleItem, 3), depth, path));
			}
		}
	}
	public class MenuHierarchicalSampleDataNode : HierarchicalSampleDataNode {
		public MenuHierarchicalSampleDataNode(string text, int depth, string path)
			: base(text, depth, path) {
		}
		protected override IHierarchicalEnumerable CreateChildren(int depth, string path) {
			return new MenuHierarchicalSampleData(depth, path);
		}
	}
}
