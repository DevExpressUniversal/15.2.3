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
using DevExpress.Internal;
namespace DevExpress.XtraPivotGrid.Data {
	public class FieldChangingEventArgs : EventArgs {
		readonly PivotGridFieldBase field;
		readonly PivotArea newArea;
		readonly int newAreaIndex;
		bool cancel;
		public FieldChangingEventArgs(PivotGridFieldBase field, PivotArea newArea, int newAreaIndex) {
			this.field = field;
			this.newArea = newArea;
			this.newAreaIndex = newAreaIndex;
			this.cancel = false;
		}
		public PivotGridFieldBase Field { get { return field; } }
		public PivotArea NewArea { get { return newArea; } }
		public int NewAreaIndex { get { return newAreaIndex; } }
		public bool Cancel { get { return cancel; } set { cancel = value; } }
	} 
	public class FieldChangedEventArgs : EventArgs {
		readonly PivotGridFieldBase field;
		public FieldChangedEventArgs(PivotGridFieldBase field) {
			this.field = field;
		}
		public PivotGridFieldBase Field {
			get { return field; }
		}
	}
	public class FieldSizeChangedEventArgs : FieldChangedEventArgs {
		readonly bool isWidthChanged, isHeightChanged;
		public FieldSizeChangedEventArgs(PivotGridFieldBase field, bool isWidthChanged, bool isHeightChanged) 
			: base(field) {
			this.isWidthChanged = isWidthChanged;
			this.isHeightChanged = isHeightChanged;
		}
		public bool IsWidthChanged {
			get { return isWidthChanged; }
		}
		public bool IsHeightChanged {
			get { return isHeightChanged; }
		}
	}
	public delegate void FieldSizeChangedEventHandler(object sender, FieldSizeChangedEventArgs e);
	public delegate void FieldChangingEventHandler(object sender, FieldChangingEventArgs e);
	public delegate void LayoutChangedEventHandler(object sender, EventArgs e);
	public delegate void ItemsEmptyEventHandler(object sender, EventArgs e);
}
