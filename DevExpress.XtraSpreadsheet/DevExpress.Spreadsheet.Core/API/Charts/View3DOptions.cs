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
	public interface SurfaceOptions : ShapeFormat {
		int Thickness { get; set; }
	}
	public interface View3DOptions {
		int DepthPercent { get; set; }
		int HeightPercent { get; set; }
		int Perspective { get; set; }
		bool RightAngleAxes { get; set; }
		bool AutoHeight { get; set; }
		int XRotation { get; set; }
		int YRotation { get; set; }
		SurfaceOptions Floor { get; }
		SurfaceOptions BackWall { get; }
		SurfaceOptions SideWall { get; }
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	using DevExpress.Spreadsheet.Charts;
	using DevExpress.Office.API.Internal;
	#region NativeSurfaceOptions
	partial class NativeSurfaceOptions : NativeShapeFormat, SurfaceOptions {
		readonly Model.SurfaceOptions modelOptions;
		public NativeSurfaceOptions(Model.SurfaceOptions modelOptions, NativeWorkbook nativeWorkbook)
			: base(modelOptions.ShapeProperties, nativeWorkbook) {
			this.modelOptions = modelOptions;
		}
		#region SurfaceOptions Members
		public int Thickness {
			get {
				CheckValid();
				return modelOptions.Thickness;
			}
			set {
				CheckValid();
				modelOptions.Thickness = value;
			}
		}
		#endregion
	}
	#endregion
	#region NativeView3DOptions
	partial class NativeView3DOptions : NativeObjectBase, View3DOptions {
		#region Fields
		readonly Model.Chart modelChart;
		NativeWorkbook nativeWorkbook;
		NativeSurfaceOptions floor;
		NativeSurfaceOptions backWall;
		NativeSurfaceOptions sideWall;
		#endregion
		public NativeView3DOptions(Model.Chart modelChart, NativeWorkbook nativeWorkbook)
			: base() {
			this.modelChart = modelChart;
			this.nativeWorkbook = nativeWorkbook;
		}
		#region View3DOptions Members
		public int DepthPercent {
			get {
				CheckValid();
				return modelChart.View3D.DepthPercent;
			}
			set {
				CheckValid();
				modelChart.View3D.DepthPercent = value;
			}
		}
		public int HeightPercent {
			get {
				CheckValid();
				return modelChart.View3D.HeightPercent;
			}
			set {
				CheckValid();
				modelChart.View3D.HeightPercent = value;
			}
		}
		public int Perspective {
			get {
				CheckValid();
				return modelChart.View3D.Perspective;
			}
			set {
				CheckValid();
				modelChart.View3D.Perspective = value;
			}
		}
		public bool RightAngleAxes {
			get {
				CheckValid();
				return modelChart.View3D.RightAngleAxes;
			}
			set {
				CheckValid();
				modelChart.View3D.RightAngleAxes = value;
			}
		}
		public bool AutoHeight {
			get {
				CheckValid();
				return modelChart.View3D.AutoHeight;
			}
			set {
				CheckValid();
				modelChart.View3D.AutoHeight = value;
			}
		}
		public int XRotation {
			get {
				CheckValid();
				return modelChart.View3D.XRotation;
			}
			set {
				CheckValid();
				modelChart.View3D.XRotation = value;
			}
		}
		public int YRotation {
			get {
				CheckValid();
				return modelChart.View3D.YRotation;
			}
			set {
				CheckValid();
				modelChart.View3D.YRotation = value;
			}
		}
		public SurfaceOptions Floor {
			get {
				CheckValid();
				if (floor == null)
					floor = new NativeSurfaceOptions(modelChart.Floor, nativeWorkbook);
				return floor; 
			}
		}
		public SurfaceOptions BackWall {
			get {
				CheckValid();
				if (backWall == null)
					backWall = new NativeSurfaceOptions(modelChart.BackWall, nativeWorkbook);
				return backWall;
			}
		}
		public SurfaceOptions SideWall {
			get {
				CheckValid();
				if (sideWall == null)
					sideWall = new NativeSurfaceOptions(modelChart.SideWall, nativeWorkbook);
				return sideWall;
			}
		}
		#endregion
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			if (floor != null)
				floor.IsValid = value;
			if (backWall != null)
				backWall.IsValid = value;
			if (sideWall != null)
				sideWall.IsValid = value;
		}
	}
	#endregion
}
