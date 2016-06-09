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
namespace DevExpress.Office.History {
	#region DrawingHistoryItem (abstract class)
	public abstract class DrawingHistoryItem<TOwner, TValue> : HistoryItem {
		#region Fields
		int index;
		TOwner owner;
		TValue oldValue;
		TValue newValue;
		#endregion
		protected DrawingHistoryItem(IDocumentModelPart documentModelPart, TOwner owner, TValue oldValue, TValue newValue)
			: base(documentModelPart) {
			this.owner = owner;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		protected DrawingHistoryItem(IDocumentModelPart documentModelPart, TOwner owner, int index, TValue oldValue, TValue newValue)
			: this(documentModelPart, owner, oldValue, newValue) {
			this.index = index;
		}
		#region Properties
		protected int Index { get { return index; } }
		protected TOwner Owner { get { return owner; } }
		protected TValue OldValue { get { return oldValue; } }
		protected TValue NewValue { get { return newValue; } }
		#endregion
	}
	#endregion
}
