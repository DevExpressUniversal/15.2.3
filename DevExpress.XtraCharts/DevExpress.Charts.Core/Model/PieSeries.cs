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

using DevExpress.Charts.Native;
using System;
using System.Collections.Generic;
namespace DevExpress.Charts.Model {
	public enum PieExplodeMode {
		None,
		All,
		UsePoints
	}
	public enum PieSweepDirection {
		Clockwise = PointsSweepDirection.Clockwise,
		Counterclockwise = PointsSweepDirection.Counterclockwise,
	}
	public abstract class PieSeriesBase : SeriesModel {
		const double DefaultExplodedDistancePercentage = 10.0;
		readonly List<int> explodedPointsIndexes = new List<int>();
		double explodedDistancePercentage = DefaultExplodedDistancePercentage;
		PieExplodeMode explodeMode = PieExplodeMode.None;
		int rotationAngle = 0;
		int depthPercent = 0;
		PieSweepDirection sweepDirection = PieSweepDirection.Counterclockwise;
		public int RotationAngle {
			get { return rotationAngle; }
			set {
				if(rotationAngle != value) {
					rotationAngle = value;
					NotifyParent(this, "RotationAngle", value);
				}
			}
		}
		public double ExplodedDistancePercentage {
			get { return explodedDistancePercentage; }
			set {
				if(explodedDistancePercentage != value) {
					explodedDistancePercentage = value;
					NotifyParent(this, "ExplodedDistancePercentage", value);
				}
			}
		}
		public PieExplodeMode ExplodeMode {
			get { return explodeMode; }
			set {
				if(explodeMode != value) {
					explodeMode = value;
					NotifyParent(this, "ExplodeMode", value);
				}
			}
		}
		public List<int> ExplodedPointsIndexes {
			get { return explodedPointsIndexes; }
		}
		public int DepthPercent {
			get { return depthPercent; }
			set {
				if(depthPercent != value) {
					depthPercent = value;
					NotifyParent(this, "DepthPercent", value);
				}
			}
		}
		public PieSweepDirection SweepDirection {
			get { return sweepDirection; }
			set {
				if (sweepDirection != value) {
					sweepDirection = value;
					NotifyParent(this, "SweepDirection", value);
				}
			}
		}
	}
	public class PieSeries : PieSeriesBase {
	}
	public class DonutSeries : PieSeriesBase {
		const int DefaultHoleRadiusPercent = 60;
		int holeRadiusPercent = DefaultHoleRadiusPercent;
		public int HoleRadiusPercent {
			get { return holeRadiusPercent; }
			set {
				if(holeRadiusPercent != value) {
					holeRadiusPercent = value;
					NotifyParent(this, "HoleRadiusPercent", value);
				}
			}
		}
	}
	public class NestedDonutSeries : DonutSeries {
		const double defaultInnerIndent = 0.0;
		double innerIndent = defaultInnerIndent;
		public double InnerIndent {
			get { return innerIndent; }
			set {
				if (innerIndent != value) {
					innerIndent = value;
					NotifyParent(this, "InnerIndent", value);
				}
			}
		}
	}
}
