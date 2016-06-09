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

using System.Collections;
using System.Web.UI;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Internal {
	public class HierarchicalSampleData : IHierarchicalEnumerable, IEnumerable {
		private ArrayList fList = new ArrayList();
		public ArrayList List {
			get { return fList; }
		}
		public HierarchicalSampleData(int depth, string path) {
			CreateSampleNodes(depth, path);
		}
		public IEnumerator GetEnumerator() {
			return fList.GetEnumerator();
		}
		public IHierarchyData GetHierarchyData(object enumeratedItem) {
			return (IHierarchyData)enumeratedItem;
		}
		protected virtual void CreateSampleNodes(int depth, string path) {
			if(depth == 0) {
				List.Add(new HierarchicalSampleDataNode(StringResources.HierarchicalSampleData_SampleRoot, depth, path));
			}
			else if(depth == 1) {
				List.Add(new HierarchicalSampleDataNode(string.Format(StringResources.HierarchicalSampleData_SampleParent, 1), depth, path));
				List.Add(new HierarchicalSampleDataNode(string.Format(StringResources.HierarchicalSampleData_SampleParent, 2), depth, path));
			}
			else if(depth == 2) {
				List.Add(new HierarchicalSampleDataNode(string.Format(StringResources.HierarchicalSampleData_SampleLeaf, 1), depth, path));
				List.Add(new HierarchicalSampleDataNode(string.Format(StringResources.HierarchicalSampleData_SampleLeaf, 2), depth, path));
			}
		}
	}
	public class HierarchicalSampleDataNode : IHierarchyData {
		private int fDepth = 0;
		private string fPath = "";
		private string fText = "";
		public int Depth {
			get { return fDepth; }
		}
		public virtual string NavigateUrl {
			get { return fPath; }
		}
		public virtual string Text {
			get { return fText; }
		}
		public HierarchicalSampleDataNode(string text, int depth, string path) {
			fText = text;
			fDepth = depth;
			fPath = path + '\\' + text;
		}
		public override string ToString() {
			return fText;
		}
		bool IHierarchyData.HasChildren {
			get { return (fDepth < 2); }
		}
		object IHierarchyData.Item {
			get { return this; }
		}
		string IHierarchyData.Path {
			get { return fPath; }
		}
		string IHierarchyData.Type {
			get { return StringResources.HierarchicalSampleData_SampleData; }
		}
		IHierarchicalEnumerable IHierarchyData.GetChildren() {
			return CreateChildren(fDepth + 1, fPath);
		}
		IHierarchyData IHierarchyData.GetParent() {
			return null;
		}
		protected virtual IHierarchicalEnumerable CreateChildren(int depth, string path) {
			return new HierarchicalSampleData(depth, path);
		}
	}
}
