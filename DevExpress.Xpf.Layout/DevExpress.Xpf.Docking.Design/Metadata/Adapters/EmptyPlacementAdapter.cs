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

using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DevExpress.Xpf.Core.Design;
using DevExpress.Xpf.Docking.Base;
using DevExpress.Xpf.Design;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Services;
using System.Windows.Controls;
using Microsoft.Windows.Design.Policies;
using DevExpress.Xpf.Core;
using System.Windows.Controls.Primitives;
using Microsoft.Windows.Design;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
namespace DevExpress.Xpf.Docking.Design {
	public class EmptyPlacementAdapter : PlacementAdapter {
		public override bool CanSetPosition(PlacementIntent intent, RelativePosition position) {
			return false;
		}
		public override RelativeValueCollection GetPlacement(ModelItem item, params RelativePosition[] positions) {
			return new RelativeValueCollection();
		}
		public override Rect GetPlacementBoundary(ModelItem item, PlacementIntent intent, params RelativeValue[] positions) {
			return new Rect();
		}
		public override Rect GetPlacementBoundary(ModelItem item) {
			return new Rect();
		}
		public override void SetPlacements(ModelItem item, PlacementIntent intent, RelativeValueCollection placement) {
		}
		public override void SetPlacements(ModelItem item, PlacementIntent intent, params RelativeValue[] positions) {
		}
	}
}
