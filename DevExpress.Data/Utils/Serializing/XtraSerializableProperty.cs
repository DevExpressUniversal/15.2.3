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
namespace DevExpress.Utils.Serializing {
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
	public class XtraSerializableProperty : Attribute {
		XtraSerializationVisibility visibility;
		XtraSerializationFlags flags = XtraSerializationFlags.None;
		bool clearCollection = false, useFindItem = false, useCreateItem = false, mergeCollection = false;
		int order;
		public XtraSerializableProperty()
			: this(XtraSerializationVisibility.Visible) {
		}
		public XtraSerializableProperty(XtraSerializationVisibility visibility, int order)
			: this(visibility) {
			this.order = order;
		}
		public XtraSerializableProperty(int order)
			: this(XtraSerializationVisibility.Visible) {
			this.order = order;
		}
		public XtraSerializableProperty(XtraSerializationVisibility visibility, bool useCreateItem, bool useFindItem, bool clearCollection, bool mergeCollection, int order, XtraSerializationFlags flags) {
			this.visibility = visibility;
			this.clearCollection = clearCollection;
			this.mergeCollection = mergeCollection;
			this.useFindItem = useFindItem;
			this.useCreateItem = useCreateItem;
			this.order = order;
			this.flags = flags;
		}
		public XtraSerializableProperty(XtraSerializationVisibility visibility, bool useCreateItem, bool useFindItem, bool clearCollection, int order, XtraSerializationFlags flags) 
			: this(visibility, useCreateItem, useFindItem, clearCollection, false, order, flags) {
		}
		public XtraSerializableProperty(XtraSerializationVisibility visibility, bool useCreateItem, bool useFindItem, bool clearCollection, int order)
			:
				this(visibility, useCreateItem, useFindItem, clearCollection, order, XtraSerializationFlags.None) { }
		public XtraSerializableProperty(XtraSerializationVisibility visibility, bool useCreateItem, bool useFindItem, bool clearCollection)
			: this(visibility, useCreateItem, useFindItem, clearCollection, 0) {
		}
		public XtraSerializableProperty(bool useCreateItem, bool useFindItem, bool clearCollection)
			: this(XtraSerializationVisibility.Collection, useCreateItem, useFindItem, clearCollection) {
		}
		public XtraSerializableProperty(XtraSerializationVisibility visibility, bool useCreateItem)
			: this(visibility, useCreateItem, false, false) {
		}
		public XtraSerializableProperty(bool useCreateItem, bool useFindItem, bool clearCollection, int order)
			: this(XtraSerializationVisibility.Collection, useCreateItem, useFindItem, clearCollection, order) {
		}
		public XtraSerializableProperty(XtraSerializationVisibility visibility) {
			this.visibility = visibility;
			this.clearCollection = false;
		}
		public XtraSerializableProperty(XtraSerializationFlags flags, int order)
			: this(flags) {
			this.order = order;
		}
		public XtraSerializableProperty(XtraSerializationFlags flags) : this(XtraSerializationVisibility.Visible, flags) { }
		public XtraSerializableProperty(XtraSerializationVisibility visibility, XtraSerializationFlags flags)
			: this(visibility, flags, 0) {
		}
		public XtraSerializableProperty(XtraSerializationVisibility visibility, XtraSerializationFlags flags, int order) {
			this.visibility = visibility;
			this.flags = flags;
			this.clearCollection = false;
			this.order = order;
		}
		public XtraSerializationFlags Flags { get { return flags; } }
		public int Order { get { return order; } }
		public bool ClearCollection { get { return clearCollection; } }
		public bool MergeCollection { get { return mergeCollection; } }
		public bool UseFindItem { get { return useFindItem; } }
		public bool UseCreateItem { get { return useCreateItem; } }
		public bool Serialize { get { return Visibility != XtraSerializationVisibility.Hidden; } }
		public bool IsCachedProperty { get { return (flags & XtraSerializationFlags.Cached) > 0; } }
		public bool DeserializeCollectionItemBeforeCallSetIndex { get { return (flags & XtraSerializationFlags.DeserializeCollectionItemBeforeCallSetIndex) > 0; } }
		public bool SupressDefaultValue { get { return (flags & XtraSerializationFlags.SuppressDefaultValue) > 0; } }
		public bool SerializeCollection {
			get {
				return Visibility == XtraSerializationVisibility.Collection || Visibility == XtraSerializationVisibility.SimpleCollection ||
					Visibility == XtraSerializationVisibility.NameCollection;
			}
		}
		public XtraSerializationVisibility Visibility { get { return visibility; } }
	}
}
