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
using DevExpress.Utils.Serializing.Helpers;
namespace DevExpress.Utils.Serializing {
	public class XtraEventArgs : EventArgs {
		XtraPropertyInfo info;
		public XtraEventArgs(XtraPropertyInfo info) {
			this.info = info;
		}
		public XtraPropertyInfo Info { get { return info; } }
	}
	public class XtraItemEventArgs : EventArgs {
		OptionsLayoutBase options;
		object owner, collection;
		XtraPropertyInfo item;
		object rootObject;
		int index;
		public XtraItemEventArgs(object owner, object collection, XtraPropertyInfo item) : this(null, owner, collection, item) { }
		public XtraItemEventArgs(object rootObject, object owner, object collection, XtraPropertyInfo item) : this(rootObject, owner, collection, item, OptionsLayoutBase.FullLayout) { }
		public XtraItemEventArgs(object rootObject, object owner, object collection, XtraPropertyInfo item, OptionsLayoutBase options) : this(rootObject, owner, collection, item, options, -1) { }
		public XtraItemEventArgs(object rootObject, object owner, object collection, XtraPropertyInfo item, OptionsLayoutBase options, int index) {
			this.options = options;
			this.owner = owner;
			this.collection = collection;
			this.item = item;
			this.rootObject = rootObject;
			this.index = index;
		}
		public OptionsLayoutBase Options { get { return options; } }
		public object Owner { get { return owner; } }
		public object Collection { get { return collection; } }
		public object RootObject { get { return rootObject; } }
		public XtraPropertyInfo Item { get { return item; } }
		public int Index { get { return index; } }
	}
	public class XtraSetItemIndexEventArgs : XtraItemEventArgs {
		int newIndex;
		public XtraSetItemIndexEventArgs(object rootObject, object owner, object collection, XtraPropertyInfo item, int newIndex)
			: base(rootObject, owner, collection, item) {
			this.newIndex = newIndex;
		}
		public virtual int NewIndex { get { return newIndex; } }
	}
	public class XtraNewItemEventArgs : XtraItemEventArgs {
		public XtraNewItemEventArgs(object rootObject, object owner, object collection, XtraPropertyInfo item) : 
			base(rootObject, owner, collection, item) { }
		public bool NewItem { get; set; }
	}
	public class XtraOldItemEventArgs : XtraItemEventArgs { 
		public XtraOldItemEventArgs(object rootObject, object owner, object collection, XtraPropertyInfo item) : 
			base(rootObject, owner, collection, item) { }
		public bool OldItem { get; set; }
	}
}
