#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Linq;
using System.Text;
namespace DevExpress.DashboardCommon.Native {
	public class DataItemContainerActualContent {
		List<Dimension> dimensions = new List<Dimension>();
		List<Measure> measures = new List<Measure>();
		bool isDelta;
		Measure deltaActualValue;
		Measure deltaTargetValue;
		bool isSparkline;
		Measure sparklineValue;
		DeltaOptions deltaOptions;
		public List<Dimension> Dimensions { get { return dimensions; } }
		public List<Measure> Measures { get { return measures; } }
		public bool IsDelta { get { return isDelta; } set { isDelta = value; } }
		public DeltaOptions DeltaOptions { get { return deltaOptions; } set { deltaOptions = value; } }
		public bool IsSparkline { get { return isSparkline; } set { isSparkline = value; } }
		public Measure DeltaActualValue { get { return deltaActualValue; } set { deltaActualValue = value; } }
		public Measure DeltaTargetValue { get { return deltaTargetValue; } set { deltaTargetValue = value; } }
		public Measure SparklineValue { get { return sparklineValue; } set { sparklineValue = value; } }
	}
}
