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
using System.Collections;
using System.Collections.Generic;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardCommon.ViewModel {
	public class PointLabelViewModel {
		bool showPointLabels;
		bool showForZeroValues;
		PointLabelPosition position;
		PointLabelOrientation orientation;
		PointLabelContentType content;
		ScatterPointLabelContentType scatterContent;
		PointLabelOverlappingMode overlappingMode;
		public bool ShowPointLabels {
			get { return showPointLabels; }
			set { showPointLabels = value; }
		}
		public bool ShowForZeroValues {
			get { return showForZeroValues; }
			set { showForZeroValues = value; }
		}
		public PointLabelPosition Position {
			get { return position; }
			set { position = value; }
		}
		public PointLabelOrientation Orientation {
			get { return orientation; }
			set { orientation = value; }
		}
		public PointLabelContentType Content {
			get { return content; }
			set { content = value; }
		}
		public ScatterPointLabelContentType ScatterContent {
			get { return scatterContent; }
			set { scatterContent = value; }
		}
		public PointLabelOverlappingMode OverlappingMode {
			get { return overlappingMode; }
			set { overlappingMode = value; }
		}
	}
}
