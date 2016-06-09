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
using DevExpress.Data;
using DevExpress.Data.PivotGrid;
using DevExpress.Utils;
using CoreXtraPivotGrid = DevExpress.XtraPivotGrid;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.XtraPivotGrid;
namespace DevExpress.Xpf.PivotGrid.Internal {
	public interface IDesignTimeAdorner {
		void PerformDragDrop();
		void PerformChangeSortOrder(PivotGridField field);
		void PerformChangeUnboundExpression(PivotGridField field);
		bool IsDesignTime { get; }
		Mvvm.UI.Native.ViewGenerator.Model.IModelItem GetPivotGridModelItem();
	}
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class PivotGridHelper {
		public static void SetDesignTimeAdorner(PivotGridControl pivot, IDesignTimeAdorner adorner) {
			if(pivot != null)
				pivot.DesignTimeAdorner = adorner;
		}
		public static IDesignTimeAdorner GetDesignTimeAdorner(PivotGridControl pivot) {
			if(pivot != null)
				return pivot.DesignTimeAdorner;
			return null;
		}
		public static string GetFormatConditionPredefinedNamesProperty(FormatConditionBase format) {
			return format.Info.OwnerPredefinedFormatsPropertyName;
		}
	}
}
