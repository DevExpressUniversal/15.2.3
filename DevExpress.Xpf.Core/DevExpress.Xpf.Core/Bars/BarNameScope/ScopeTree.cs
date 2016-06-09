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

using DevExpress.Mvvm.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Xpf.Bars.Native {
	#region scopeTree    
	public interface IEventHandlerListSupport {
		void AddHandler(object key, Delegate value);
		void RemoveHandler(object key, Delegate value);
	}
	[Flags]
	public enum ScopeSearchSettings {
		Ancestors = 0x1,
		Local = 0x2,
		Descendants = 0x4,
	}
	public sealed class ScopeTree {
		ScopeTree() { }
		internal static ScopeTreeNode Attach(BarNameScope scope) {
			var tree = ElementRegistratorService.ParentsAndSelf(scope).FirstOrDefault(x => x.ScopeTree != null).With(x => x.ScopeTree).With(x=>x.Tree) ?? new ScopeTree();
			return tree.DoAttach(scope);
		}
		internal static void Detach(BarNameScope scope) {
			if (scope == null || scope.ScopeTree == null)
				return;
			scope.ScopeTree.Tree.DoDetach(scope);			
		}		
		ScopeTreeNode DoAttach(BarNameScope scope) { return new ScopeTreeNode(this, scope); }
		void DoDetach(BarNameScope scope) { }		
		public static IEnumerable<BarNameScope> Ancestors(BarNameScope scope, bool includeSelf = false) {
			if (scope == null)
				yield break;
			if (!includeSelf)
				scope = scope.Parent;
			while (scope != null) {
				yield return scope;
				scope = scope.Parent;
			}
		}
		public static IEnumerable<BarNameScope> Descendants(BarNameScope scope, bool includeSelf = false) {			
			if (scope == null) yield break;
			if (includeSelf)
				yield return scope;
			foreach (var element in scope.Children) {
				yield return element;
				foreach (var descendant in Descendants(element))
					yield return descendant;
			}
		}		
		public IEnumerable<BarNameScope> CreateSearchList(BarNameScope scope, ScopeSearchSettings searchDirection) {
			if (scope == null)
				yield break;
			var first = searchDirection.HasFlag(ScopeSearchSettings.Ancestors) ? Ancestors(scope) : Enumerable.Empty<BarNameScope>();
			var second = searchDirection.HasFlag(ScopeSearchSettings.Descendants) ? Descendants(scope) : Enumerable.Empty<BarNameScope>();
			var center = scope;			
			foreach (var element in first)
				yield return element;
			if (searchDirection.HasFlag(ScopeSearchSettings.Local))
				yield return scope;
			foreach (var element in second)
				yield return element;
		}
		public static int GetLevel(BarNameScope currentNode, BarNameScope secondNode, ScopeSearchSettings condition) {
			if (Equals(condition, ScopeSearchSettings.Local))
				return Equals(currentNode, secondNode) ? 0 : -1;
			int ancLevel = -1;
			int desLevel = -1;
			bool returnAny = condition.HasFlag(ScopeSearchSettings.Ancestors | ScopeSearchSettings.Descendants);
			if (condition.HasFlag(ScopeSearchSettings.Ancestors)) {
				ancLevel = GetAncestorLevel(currentNode, secondNode);
				if (!returnAny)
					return ancLevel;
			}
			if (condition.HasFlag(ScopeSearchSettings.Descendants)) {
				desLevel = GetDescendantLevel(currentNode, secondNode);
				if (!returnAny)
					return desLevel;
			}
			if (returnAny) {
				var result = ancLevel == -1 ? desLevel : ancLevel;
				if ((result == 0) && !condition.HasFlag(ScopeSearchSettings.Local))
					return -1;
				return result;
			}
			return -1;
		}
		public static int GetAncestorLevel(BarNameScope currentNode, BarNameScope ancestor) {
			int result = 0;
			foreach (var scope in Ancestors(currentNode, true)) {
				if (Equals(scope, ancestor))
					return result;
				result++;
			}
			return -1;
		}
		public static int GetDescendantLevel(BarNameScope currentNode, BarNameScope descendant) {
			return GetAncestorLevel(descendant, currentNode);
		}
		public IEnumerable<BarNameScope> Find(BarNameScope scope, Func<BarNameScope, bool> predicate = null, ScopeSearchSettings searchDirection = ScopeSearchSettings.Local) {
			return CreateSearchList(scope, searchDirection).Where(predicate ?? new Func<BarNameScope, bool>(ns => true));
		}
	}
	public sealed class ScopeTreeNode {
		readonly ScopeTree tree;
		readonly BarNameScope target;
		internal ScopeTree Tree { get { return tree; } }
		internal ScopeTreeNode(ScopeTree tree, BarNameScope target) {
			this.tree = tree;
			this.target = target;
		}
		public IEnumerable<BarNameScope> Ancestors() {
			return ScopeTree.Ancestors(target);
		}
		public IEnumerable<BarNameScope> Descendants() {
			return ScopeTree.Descendants(target);
		}
		public IEnumerable<BarNameScope> Find(Func<BarNameScope, bool> predicate = null, ScopeSearchSettings searchDirection = ScopeSearchSettings.Local) {
			return tree.Find(target, predicate, searchDirection);
		}
	}
	#endregion
}
