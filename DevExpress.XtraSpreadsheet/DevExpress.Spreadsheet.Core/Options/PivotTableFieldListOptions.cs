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
using System.Runtime.InteropServices;
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
using System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraSpreadsheet {
	#region SpreadsheetPivotTableFieldListOptions
	[ComVisible(true)]
	public class SpreadsheetPivotTableFieldListOptions : SpreadsheetNotificationOptions {
		#region Fields
		internal static Point defaultStartLocation = new Point(0, 0);
		internal static Size defaultStartSize = new Size(0, 0);
		SpreadsheetPivotTableFieldListStartPosition startPosition;
		Point startLocation;
		Size startSize;
		#endregion
		public SpreadsheetPivotTableFieldListOptions()
			: base() {
		}
		#region Properties
		#region StartPosition
		[ DefaultValue(SpreadsheetPivotTableFieldListStartPosition.CenterSpreadsheetControl), NotifyParentProperty(true)]
		public SpreadsheetPivotTableFieldListStartPosition StartPosition {
			get { return startPosition; }
			set {
				if (startPosition == value)
					return;
				SpreadsheetPivotTableFieldListStartPosition oldValue = this.startPosition;
				this.startPosition = value;
				OnChanged("StartPosition", oldValue, value);
			}
		}
		#endregion
		#region StartLocation
		[ NotifyParentProperty(true)]
		public Point StartLocation {
			get { return startLocation; }
			set {
				if (this.startLocation == value)
					return;
				Point oldValue = this.startLocation;
				this.startLocation = value;
				OnChanged("StartLocation", oldValue, value);
			}
		}
		protected internal virtual bool ShouldSerializeStartLocation() { return StartLocation != defaultStartLocation; }
		protected internal virtual void ResetStartLocation() { StartLocation = defaultStartLocation; }
		#endregion
		#region StartSize
		[ NotifyParentProperty(true)]
		public Size StartSize {
			get { return startSize; }
			set {
				if (this.startSize == value)
					return;
				Size oldValue = this.startSize;
				this.startSize = value;
				OnChanged("StartSize", oldValue, value);
			}
		}
		protected internal virtual bool ShouldSerializeStartSize() { return StartSize != defaultStartSize; }
		protected internal virtual void ResetStartSize() { StartSize = defaultStartSize; }
		#endregion
		#endregion
		protected internal override void ResetCore() {
			this.startPosition = SpreadsheetPivotTableFieldListStartPosition.CenterSpreadsheetControl;
			ResetStartLocation();
			ResetStartSize();
		}
		protected internal void CopyFrom(SpreadsheetPivotTableFieldListOptions value) {
			this.startPosition = value.StartPosition;
			this.startLocation = value.StartLocation;
			this.startSize = value.StartSize;
		}
	}
	#endregion
	#region SpreadsheetPivotTableFieldListStartPosition
	public enum SpreadsheetPivotTableFieldListStartPosition {
		ManualScreen,
		ManualSpreadsheetControl,
		CenterScreen,
		CenterSpreadsheetControl
	}
	#endregion
}
