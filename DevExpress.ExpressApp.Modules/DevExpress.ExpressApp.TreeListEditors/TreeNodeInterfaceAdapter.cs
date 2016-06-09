#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Text;
using DevExpress.ExpressApp.Controls;
using System.Collections;
using DevExpress.Persistent.Base.General;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.TreeListEditors {
	public class TreeNodeInterfaceAdapter : NodeObjectAdapter {
		private IList collection;
		private bool holdRootValue = false;
		private void OnCollectionChanged() {
			if(CollectionChanged != null) {
				CollectionChanged(this, EventArgs.Empty);
			}
		}
		public override bool HasChildren(object nodeObject) {
			return nodeObject != null ? ((ITreeNode)nodeObject).Children.Count > 0 : false;
		}
		public override object GetParent(object nodeObject) {
			return nodeObject != null ? ((ITreeNode)nodeObject).Parent : null;
		}
		public override IEnumerable GetChildren(object nodeObject) {
			return nodeObject != null ? ((ITreeNode)nodeObject).Children : null;
		}
		public override System.Drawing.Image GetImage(object nodeObject, out string imageName) {
			if(nodeObject is ITreeNodeImageProvider) {
				return ((ITreeNodeImageProvider)nodeObject).GetImage(out imageName);
			}
			return base.GetImage(nodeObject, out imageName);
		}
		public override string GetDisplayPropertyValue(object nodeObject) {
			return nodeObject != null ? ((ITreeNode)nodeObject).Name : string.Empty;
		}
		public override string DisplayPropertyName {
			get { return "Name"; }	
		}
		public override bool IsRoot(object nodeObject) {
			return base.IsRoot(nodeObject) || (!holdRootValue && collection != null && !collection.Contains(GetParent(nodeObject)));
		}
		public IEnumerable GetRootNodes() {
			if(collection == null) {
				throw new InvalidOperationException("Assign the 'Collection' property before using the 'GetRootNodes()' method.");
			}
			return Enumerator.Filter(collection, new Predicate<object>(IsRoot));
		}
		public bool HoldRootValue {
			get { return holdRootValue; }
			set {
				if(holdRootValue != value) {
					holdRootValue = value;
					OnChanged();
				}
			}
		}
		public IList Collection {
			get { return collection; }
			set {
				if(collection != value) {
					collection = value;
					OnCollectionChanged();
				}
			}
		}
		public event EventHandler<EventArgs> CollectionChanged;
	}
}
