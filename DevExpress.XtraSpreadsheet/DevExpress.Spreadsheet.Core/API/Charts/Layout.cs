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
using System.ComponentModel;
using DevExpress.Office;
namespace DevExpress.Spreadsheet.Charts {
	public enum LayoutMode {
		Auto = DevExpress.XtraSpreadsheet.Model.LayoutMode.Auto,
		Edge = DevExpress.XtraSpreadsheet.Model.LayoutMode.Edge,
		Factor = DevExpress.XtraSpreadsheet.Model.LayoutMode.Factor
	}
	public enum LayoutTarget {
		Inner = DevExpress.XtraSpreadsheet.Model.LayoutTarget.Inner,
		Outer = DevExpress.XtraSpreadsheet.Model.LayoutTarget.Outer
	}
	public interface LayoutPosition {
		LayoutMode Mode { get; set; }
		double Value { get; set; }
		void SetPosition(LayoutMode mode, double value);
	}
	public interface LayoutOptions {
		bool Auto { get; }
		LayoutPosition Left { get; }
		LayoutPosition Top { get; }
		LayoutPosition Width { get; }
		LayoutPosition Height { get; }
		LayoutTarget Target { get; set; }
		void ResetToAuto();
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	using DevExpress.Spreadsheet.Charts;
	using DevExpress.Utils;
#if DXPORTABLE
	public
#else
	internal
#endif
		enum NativeLayoutPositionType {
		Left,
		Top,
		Width,
		Height
	}
	partial class NativeLayoutPosition : NativeObjectBase, LayoutPosition {
		readonly Model.LayoutOptions modelLayout;
		readonly NativeLayoutPositionType positionType;
		public NativeLayoutPosition(Model.LayoutOptions modelLayout, NativeLayoutPositionType positionType) {
			this.modelLayout = modelLayout;
			this.positionType = positionType;
		}
#region LayoutPosition Members
		public LayoutMode Mode {
			get {
				CheckValid();
				switch (positionType) {
					case NativeLayoutPositionType.Left:
						return (LayoutMode)this.modelLayout.Left.Mode;
					case NativeLayoutPositionType.Top:
						return (LayoutMode)this.modelLayout.Top.Mode;
					case NativeLayoutPositionType.Width:
						return (LayoutMode)this.modelLayout.Width.Mode;
					case NativeLayoutPositionType.Height:
						return (LayoutMode)this.modelLayout.Height.Mode;
				}
				return LayoutMode.Auto;
			}
			set {
				SetPosition(value, Value);
			}
		}
		public double Value {
			get {
				CheckValid();
				switch (positionType) {
					case NativeLayoutPositionType.Left:
						return this.modelLayout.Left.Value;
					case NativeLayoutPositionType.Top:
						return this.modelLayout.Top.Value;
					case NativeLayoutPositionType.Width:
						return this.modelLayout.Width.Value;
					case NativeLayoutPositionType.Height:
						return this.modelLayout.Height.Value;
				}
				return 0.0;
			}
			set {
				SetPosition(Mode, value);
			}
		}
		public void SetPosition(LayoutMode mode, double value) {
			CheckValid();
			switch(positionType) {
				case NativeLayoutPositionType.Left:
					this.modelLayout.Left = new Model.ManualLayoutPosition(value, (Model.LayoutMode)mode);
					break;
				case NativeLayoutPositionType.Top:
					this.modelLayout.Top = new Model.ManualLayoutPosition(value, (Model.LayoutMode)mode);
					break;
				case NativeLayoutPositionType.Width:
					this.modelLayout.Width = new Model.ManualLayoutPosition(value, (Model.LayoutMode)mode);
					break;
				case NativeLayoutPositionType.Height:
					this.modelLayout.Height = new Model.ManualLayoutPosition(value, (Model.LayoutMode)mode);
					break;
			}
		}
#endregion
	}
	partial class NativeLayoutOptions : NativeObjectBase, LayoutOptions {
		readonly Model.LayoutOptions modelLayout;
		NativeLayoutPosition left = null;
		NativeLayoutPosition top = null;
		NativeLayoutPosition width = null;
		NativeLayoutPosition height = null;
		public NativeLayoutOptions(Model.LayoutOptions modelLayout) {
			this.modelLayout = modelLayout;
		}
#region LayoutOptions Members
		public bool Auto {
			get {
				CheckValid();
				return this.modelLayout.Auto; 
			}
		}
		public LayoutPosition Left {
			get {
				CheckValid();
				if (this.left == null)
					this.left = new NativeLayoutPosition(this.modelLayout, NativeLayoutPositionType.Left);
				return this.left; 
			}
		}
		public LayoutPosition Top {
			get {
				CheckValid();
				if (this.top == null)
					this.top = new NativeLayoutPosition(this.modelLayout, NativeLayoutPositionType.Top);
				return this.top;
			}
		}
		public LayoutPosition Width {
			get {
				CheckValid();
				if (this.width == null)
					this.width = new NativeLayoutPosition(this.modelLayout, NativeLayoutPositionType.Width);
				return this.width;
			}
		}
		public LayoutPosition Height {
			get {
				CheckValid();
				if (this.height == null)
					this.height = new NativeLayoutPosition(this.modelLayout, NativeLayoutPositionType.Height);
				return this.height;
			}
		}
		public LayoutTarget Target {
			get {
				CheckValid();
				return (LayoutTarget)this.modelLayout.Target;
			}
			set {
				CheckValid();
				this.modelLayout.Target = (Model.LayoutTarget)value;
			}
		}
		public void ResetToAuto() {
			CheckValid();
			this.modelLayout.SetAuto();
		}
#endregion
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			if (this.left != null)
				this.left.IsValid = value;
			if (this.top != null)
				this.top.IsValid = value;
			if (this.width != null)
				this.width.IsValid = value;
			if (this.height != null)
				this.height.IsValid = value;
		}
	}
}
