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
using System.Collections;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using System.Drawing;
using DevExpress.XtraPrinting.Native.Navigation;
using System.Collections.Generic;
namespace DevExpress.XtraReports.Native.Navigation
{
	internal class NavigationManager 
	{
		#region	static 
		static IList SelectControls(ControlSelector selector, IList components) {
			ArrayList controls = new ArrayList(); 
			if(selector != null) {
				NestedComponentEnumerator enumerator = new NestedComponentEnumerator(components);
				while( enumerator.MoveNext() ) {
					if(selector.Select(enumerator.Current) && !controls.Contains(enumerator.Current))
						controls.Add(enumerator.Current);
				}
			}
			return controls;
		}
		#endregion
		Hashtable controlHT = new Hashtable();
		IList targets;
		XtraReport report;
		Document Document { get { return report.PrintingSystem.Document; }
		}
		public NavigationManager(XtraReport report) {
			this.report = report;
		}
		public void Clear() {
			controlHT.Clear();
			if(targets != null) targets.Clear();
		}
		public bool TargetsContains(XRControl control) {
			return targets != null && targets.Contains(control);
		}
		private IList GetValidNames() {
			return new ArrayList(controlHT.Keys);
		}
		public void Initialize() {
			Clear();
			IList validControls = SelectControls(new ControlNamesSelector(), report.AllBands);
			foreach(XRControl c in validControls) {
				controlHT[c.Name] = c;
			}
			CrossRefControlSelector refSelector = new CrossRefControlSelector();
			SelectControls(refSelector, report.AllBands);
			TargetControlSelector targetSelector = new TargetControlSelector(GetValidNames(), refSelector.TargetNames);
			this.targets =  SelectControls(targetSelector, report.AllBands);
		}
		public XRControl GetNavigationTarget(XRControl control) {
			return control != null ? controlHT[control.NavigateUrl] as XRControl : null;
		}
	}
	internal abstract class ControlSelector 
	{
		public bool Select(XRControl control) {
			if(CanSelect(control)) { 
				OnSelectComplete(control); 
				return true;
			}
			return false;
		}
		public abstract bool CanSelect(XRControl control);
		public virtual void OnSelectComplete(XRControl control) {
		}
	}
	internal class ControlNamesSelector : ControlSelector 
	{
		public override bool CanSelect(XRControl control) {
			return control != null && !(control is Band) && !String.IsNullOrEmpty(control.Name);
		}
	}
	internal class TargetControlSelector : ControlSelector 
	{
		IList validNames;
		IList targetNames;
		public TargetControlSelector(IList validNames, IList targetNames) {
			this.validNames = validNames != null ? validNames : new string[0];
			this.targetNames = targetNames != null ? targetNames : new string[0];
		}
		public override bool CanSelect(XRControl control) {
			return control != null && validNames.Contains(control.Name) && 
				targetNames.Contains(control.Name);
		}
	}
	internal class CrossRefControlSelector : ControlSelector 
	{
		IList targetNames = new ArrayList(); 
		public IList TargetNames { get { return targetNames; }
		}
		public override bool CanSelect(XRControl control) {
			return control != null && ((IBrickOwner)control).IsNavigationLink;
		}
		public override void OnSelectComplete(XRControl control) {
			if(control != null) {
				targetNames.Add((control.NavigateUrl));
			}
		}
	}
	internal class NavigationBuilder {
		Dictionary<IBrickOwner, BrickPagePair> navigationTargets = new Dictionary<IBrickOwner, BrickPagePair>();
		Dictionary<IBrickOwner, List<VisualBrick>> navigationLinks = new Dictionary<IBrickOwner, List<VisualBrick>>();
		Hashtable brickOwners = new Hashtable();
		NavigationManager navigationManager;
		public NavigationBuilder(NavigationManager navigationManager) {
			this.navigationManager = navigationManager;
		}
		public void SetNavigationPairs(BrickPagePairCollection bpPairs, PageList pages) {
			bpPairs.Sort(new BrickPagePairComparer(pages));
			foreach(BrickPagePair bpPair in bpPairs) {
				VisualBrick brick = bpPair.GetBrick(pages) as VisualBrick;
				if(brick == null)
					continue;
				if(brick.BrickOwner.IsNavigationLink) {
					IBrickOwner target = navigationManager.GetNavigationTarget(((XRControl)brick.BrickOwner));
					if(target != null) {
					BrickPagePair navigationPair;
					if(navigationTargets.TryGetValue(target, out navigationPair) && !brickOwners.ContainsKey(brick.BrickOwner)) {
						brick.NavigationPair = navigationPair;
					} else {
						brickOwners[brick.BrickOwner] = true;
						List<VisualBrick> links;
						if(!navigationLinks.TryGetValue(target, out links)) {
							links = new List<VisualBrick>();
							navigationLinks.Add(target, links);
						}
						links.Add(brick);
						}
					}
				}
				if(brick.BrickOwner.IsNavigationTarget) {
					List<VisualBrick> links2;
					if(navigationLinks.TryGetValue(brick.BrickOwner, out links2)) {
						foreach(VisualBrick item in links2)
							item.NavigationPair = bpPair;
						navigationLinks.Remove(brick.BrickOwner);
					}
					navigationTargets[brick.BrickOwner] = bpPair;
				}
			}
			foreach(IBrickOwner target in navigationLinks.Keys) {
				BrickPagePair navigationPair;
				if(!navigationTargets.TryGetValue(target, out navigationPair))
					continue;
				foreach(VisualBrick linkBrick in navigationLinks[target])
					linkBrick.NavigationPair = navigationPair;
			}
			navigationTargets.Clear();
			navigationLinks.Clear();
			brickOwners.Clear();
		}
	}
	internal class DocumentMapBuilder {
		readonly Document document;
		Dictionary<object, List<BookmarkNode>> childNodes = new Dictionary<object, List<BookmarkNode>>();
		Dictionary<object, BookmarkNode> nodes = new Dictionary<object, BookmarkNode>();		
		public DocumentMapBuilder(Document document) {
			this.document = document;
		}
		public void Build(BrickPagePairCollection bpPairs) {
			bpPairs.Sort(new BrickPagePairComparer(document.Pages));
			foreach(BrickPagePair brickPagePair in bpPairs) {
				VisualBrick visualBrick = brickPagePair.GetBrick(document.Pages) as VisualBrick;
				if(visualBrick == null)
					continue;
				BookmarkNode bookmarkNode = new BookmarkNode(visualBrick.BookmarkInfo.Bookmark, brickPagePair);
				nodes[visualBrick] = bookmarkNode;
				if(visualBrick.BookmarkInfo.ParentBrick == null) {
					document.BookmarkNodes.Add(bookmarkNode);
				} else {
					List<BookmarkNode> value;
					if(!childNodes.TryGetValue(visualBrick.BookmarkInfo.ParentBrick, out value)) {
						value = new List<BookmarkNode>();
						childNodes[visualBrick.BookmarkInfo.ParentBrick] = value;
					}
					value.Add(bookmarkNode);
				}
			}
			foreach(KeyValuePair<object, List<BookmarkNode>> item in childNodes) {
				BookmarkNode parentNode;
				if(nodes.TryGetValue(item.Key, out parentNode)) {
					foreach(BookmarkNode node in item.Value)
						parentNode.Nodes.Add(node);
				}
			}
			childNodes.Clear();
			nodes.Clear();
			if(((PrintingDocument)document).BookmarkDuplicateSuppress)
				SuppressDuplicateBookmarks(document.RootBookmark);
		}
		void SuppressDuplicateBookmarks(BookmarkNode rootBookmark) {
			List<BookmarkNode> newCollection = new List<BookmarkNode>();
			foreach(BookmarkNode node in rootBookmark.Nodes) {
				int index = IsNodeIncluded(newCollection, node);
				if(index == -1)
					newCollection.Add(node);
				else
				   foreach(BookmarkNode innerNode in node.Nodes)
					   newCollection[index].Nodes.Add(innerNode);
			}
			for(int i = rootBookmark.Nodes.Count - 1; i >= 0; i--) { 
				rootBookmark.Nodes.RemoveAt(i);
			} 
			foreach(BookmarkNode node in newCollection) {
				SuppressDuplicateBookmarks(node);
				rootBookmark.Nodes.Add(node);
			}
		}
		private int IsNodeIncluded(List<BookmarkNode> bookmarkCollection, BookmarkNode bookmarkNode) {
			foreach(BookmarkNode collectionNode in bookmarkCollection) {
				if(collectionNode.Text == bookmarkNode.Text)
					return bookmarkCollection.IndexOf(collectionNode);
			}
			return -1;
		}
	}
}
