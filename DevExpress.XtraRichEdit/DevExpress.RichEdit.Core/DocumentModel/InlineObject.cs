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
using System.Collections.Generic;
using DevExpress.Office;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Model {
	#region InlineObjectInfo
	public class InlineObjectInfo : ISupportsSizeOf {
		float scaleX;
		float scaleY;
		#region Properties
		public float ScaleX { get { return scaleX; } set { scaleX = value; } }
		public float ScaleY { get { return scaleY; } set { scaleY = value; } }
		#endregion
		public override bool Equals(object obj) {
			InlineObjectInfo info = (InlineObjectInfo)obj;
			return this.ScaleX == info.ScaleX && this.ScaleY == info.ScaleY;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		protected internal void CopyFromCore(InlineObjectInfo info) {
			ScaleX = info.ScaleX;
			ScaleY = info.ScaleY;
		}
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
	}
	#endregion
	#region InlineObjectProperties<T> (abstract class)
	public abstract class InlineObjectProperties<T> : RichEditIndexBasedObject<T> where T : InlineObjectInfo, ICloneable<T>, ISupportsCopyFrom<T> {
		protected InlineObjectProperties(PieceTable pieceTable)
			: base(pieceTable) {
		}
		#region Properties
		#region ScaleX
		public float ScaleX {
			get { return Info.ScaleX; }
			set {
				if(value <= 0)
					Guard.ArgumentPositive((int)value, "ScaleX");
				SetPropertyValue(SetScaleXCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetScaleXCore(T info, float value) {
			info.ScaleX = value;
			return InlineObjectChangeActionsCalculator.CalculateChangeActions(InlineObjectChangeType.ScaleX);
		}
		#endregion
		#region ScaleY
		public float ScaleY {
			get { return Info.ScaleY; }
			set {
				if (value <= 0)
					Guard.ArgumentPositive((int)value, "ScaleY");
				SetPropertyValue(SetScaleYCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetScaleYCore(T info, float value) {
			info.ScaleY = value;
			return InlineObjectChangeActionsCalculator.CalculateChangeActions(InlineObjectChangeType.ScaleY);
		}
		#endregion
		#endregion
	}
	#endregion
	#region InlineObjectChangeType
	public enum InlineObjectChangeType {
		None = 0,
		ScaleX,
		ScaleY,
		BatchUpdate,
		Sizing
	}
	#endregion
	#region InlineObjectChangeActionsCalculator
	public static class InlineObjectChangeActionsCalculator {
		internal class InlineObjectChangeActionsTable : Dictionary<InlineObjectChangeType, DocumentModelChangeActions> {
		}
		internal static InlineObjectChangeActionsTable inlineObjectChangeActionsTable = CreateInlineObjectChangeActionsTable();
		internal static InlineObjectChangeActionsTable CreateInlineObjectChangeActionsTable() {
			InlineObjectChangeActionsTable table = new InlineObjectChangeActionsTable();
			table.Add(InlineObjectChangeType.None, DocumentModelChangeActions.None);
			table.Add(InlineObjectChangeType.ScaleX, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetCaretInputPositionFormatting | DocumentModelChangeActions.ResetRuler);
			table.Add(InlineObjectChangeType.ScaleY, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetCaretInputPositionFormatting | DocumentModelChangeActions.ResetRuler);
			table.Add(InlineObjectChangeType.BatchUpdate, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetCaretInputPositionFormatting | DocumentModelChangeActions.ResetRuler);
			table.Add(InlineObjectChangeType.Sizing, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetCaretInputPositionFormatting | DocumentModelChangeActions.ResetRuler);
			return table;
		}
		public static DocumentModelChangeActions CalculateChangeActions(InlineObjectChangeType change) {
			return inlineObjectChangeActionsTable[change];
		}
	}
	#endregion
}
