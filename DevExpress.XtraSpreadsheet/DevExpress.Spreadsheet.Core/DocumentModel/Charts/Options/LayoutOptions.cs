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
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Compatibility.System;
namespace DevExpress.XtraSpreadsheet.Model {
	#region LayoutMode
	public enum LayoutMode {
		Auto = 0,
		Factor = 1,
		Edge = 2,
	}
	#endregion
	#region LayoutTarget
	public enum LayoutTarget {
		Inner = 0,
		Outer = 1,
	}
	#endregion
	#region ManualLayout
	public class LayoutOptions : ISupportsCopyFrom<LayoutOptions> {
		#region Fields
		public static readonly ManualLayoutPosition AutoPosition = new ManualLayoutPosition(0, LayoutMode.Auto);
		readonly IChart parent;
		ManualLayoutPosition left;
		ManualLayoutPosition top;
		ManualLayoutPosition width;
		ManualLayoutPosition height;
		LayoutTarget target;
		#endregion
		public LayoutOptions(IChart parent) {
			this.parent = parent;
			target = LayoutTarget.Outer;
			SetAutoCore();
		}
		#region Properties
		protected internal IChart Parent { get { return parent; } }
		protected internal DocumentModel DocumentModel { get { return parent.DocumentModel; } }
		public bool Auto {
			get { return left.Mode == LayoutMode.Auto && top.Mode == LayoutMode.Auto && width.Mode == LayoutMode.Auto && height.Mode == LayoutMode.Auto; }
		}
		public ManualLayoutPosition Left {
			get { return left; }
			set {
				if(left == value)
					return;
				SetLeft(value);
			}
		}
		public ManualLayoutPosition Top {
			get { return top; }
			set {
				if(top == value)
					return;
				SetTop(value);
			}
		}
		public ManualLayoutPosition Width {
			get { return width; }
			set {
				if(width == value)
					return;
				SetWidth(value);
			}
		}
		public ManualLayoutPosition Height {
			get { return height; }
			set {
				if(height == value)
					return;
				SetHeight(value);
			}
		}
		public LayoutTarget Target {
			get { return target; }
			set {
				if(target == value)
					return;
				SetTarget(value);
			}
		}
		#endregion
		#region Setters
		public void SetAuto() {
			DocumentModel.History.BeginTransaction();
			try {
				Left = AutoPosition;
				Top = AutoPosition;
				Width = AutoPosition;
				Height = AutoPosition;
			}
			finally {
				DocumentModel.History.EndTransaction();
			}
		}
		public void SetAutoCore() {
			left = AutoPosition;
			top = AutoPosition;
			width = AutoPosition;
			height = AutoPosition;
		}
		public void SetLeft(ManualLayoutPosition value) {
			ManualLayoutLeftPropertyChangedHistoryItem historyItem = new ManualLayoutLeftPropertyChangedHistoryItem(DocumentModel, this, left, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		public void SetLeftCore(ManualLayoutPosition value) {
			left = value;
			Parent.Invalidate();
		}
		public void SetTop(ManualLayoutPosition value) {
			ManualLayoutTopPropertyChangedHistoryItem historyItem = new ManualLayoutTopPropertyChangedHistoryItem(DocumentModel, this, top, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		public void SetTopCore(ManualLayoutPosition value) {
			top = value;
			Parent.Invalidate();
		}
		public void SetWidth(ManualLayoutPosition value) {
			ManualLayoutWidthPropertyChangedHistoryItem historyItem = new ManualLayoutWidthPropertyChangedHistoryItem(DocumentModel, this, width, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		public void SetWidthCore(ManualLayoutPosition value) {
			width = value;
			Parent.Invalidate();
		}
		public void SetHeight(ManualLayoutPosition value) {
			ManualLayoutHeightPropertyChangedHistoryItem historyItem = new ManualLayoutHeightPropertyChangedHistoryItem(DocumentModel, this, height, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		public void SetHeightCore(ManualLayoutPosition value) {
			height = value;
			Parent.Invalidate();
		}
		public void SetTarget(LayoutTarget value) {
			ManualLayoutTargetPropertyChangedHistoryItem historyItem = new ManualLayoutTargetPropertyChangedHistoryItem(DocumentModel, this, target, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		public void SetTargetCore(LayoutTarget value) {
			target = value;
			Parent.Invalidate();
		}
		#endregion
		#region ISupportsCopyFrom<ManualLayout> Members
		public void CopyFrom(LayoutOptions value) {
			Top = value.Top;
			Left = value.Left;
			Width = value.Width;
			Height = value.Height;
			Target = value.Target;
		}
		#endregion
	}
	#endregion
	#region ManualLayoutPosition
	[Serializable, StructLayout(LayoutKind.Sequential), ComVisible(false)]
	public struct ManualLayoutPosition {
		double value;
		LayoutMode mode;
		public ManualLayoutPosition(double value, LayoutMode mode) {
			this.value = value;
			this.mode = mode;
		}
		public double Value { get { return value; } }
		public LayoutMode Mode { get { return mode; } }
		public static bool operator ==(ManualLayoutPosition value, ManualLayoutPosition other) {
			return AreEqual(value, other);
		}
		public static bool operator !=(ManualLayoutPosition value, ManualLayoutPosition other) {
			return !AreEqual(value, other);
		}
		static bool AreEqual(ManualLayoutPosition value, ManualLayoutPosition other) {
			return value.Value == other.value && value.mode == other.mode;
		}
		public override bool Equals(object obj) {
			if(obj is ManualLayoutPosition) {
				ManualLayoutPosition other = (ManualLayoutPosition)obj;
				return value == other.value && mode == other.mode;
			}
			else
				return false;
		}
		public override int GetHashCode() {
			return value.GetHashCode() ^ (int)mode;
		}
	}
	#endregion
}
