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

using System.Collections.Generic;
using System.Windows;
using System.Collections;
using System;
#if !SL
using DevExpress.Xpf.Utils;
#else
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Data.Browsing;
#endif
namespace DevExpress.Xpf.Grid {
	public class DragDropViewInfo : DependencyObject {
		public static readonly DependencyProperty DraggingRowsProperty;
		static readonly DependencyPropertyKey DraggingRowsPropertyKey;
		public static readonly DependencyProperty DropTargetTypeProperty;
		public static readonly DependencyProperty DropTargetRowProperty;
		public static readonly DependencyProperty GroupInfoProperty;		
		public static readonly DependencyProperty FirstDraggingObjectProperty;
		static DragDropViewInfo() {
			Type ownerType = typeof(DragDropViewInfo);
			DraggingRowsPropertyKey = DependencyPropertyManager.RegisterReadOnly("DraggingRows", typeof(IList), ownerType, new UIPropertyMetadata(null));
			DraggingRowsProperty = DraggingRowsPropertyKey.DependencyProperty;
			DropTargetTypeProperty = DependencyPropertyManager.Register("DropTargetType", typeof(DropTargetType), ownerType, new UIPropertyMetadata(DropTargetType.None));
			DropTargetRowProperty = DependencyPropertyManager.Register("DropTargetRow", typeof(object), ownerType, new UIPropertyMetadata(null));
			GroupInfoProperty = DependencyPropertyManager.Register("GroupInfo", typeof(IList<GroupInfo>), ownerType, new UIPropertyMetadata(null));
			FirstDraggingObjectProperty = DependencyPropertyManager.Register("FirstDraggingObject", typeof(object), ownerType, new UIPropertyMetadata(null));
		}
		public IList DraggingRows {
			get { return (IList)GetValue(DraggingRowsProperty); }
			internal set { this.SetValue(DraggingRowsPropertyKey, value); }
		}
		public DropTargetType DropTargetType {
			get { return (DropTargetType)GetValue(DropTargetTypeProperty); }
			set { this.SetValue(DropTargetTypeProperty, value); }
		}
		public object DropTargetRow {
			get { return GetValue(DropTargetRowProperty); }
			set { this.SetValue(DropTargetRowProperty, value); }
		}
		public IList<GroupInfo> GroupInfo {
			get { return (IList<GroupInfo>)GetValue(GroupInfoProperty); }
			set { this.SetValue(GroupInfoProperty, value); }
		}
		public object FirstDraggingObject {
			get { return GetValue(FirstDraggingObjectProperty); }
			set { this.SetValue(FirstDraggingObjectProperty, value); }
		}
	}
	public class GroupInfo {
		public object Value { get; set; }
		public string FieldName { get; set; }
	}
}
