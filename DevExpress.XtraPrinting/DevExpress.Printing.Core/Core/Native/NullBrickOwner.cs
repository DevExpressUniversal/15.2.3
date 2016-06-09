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

using System.Drawing;
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.XtraPrinting.HtmlExport.Controls;
using DevExpress.XtraReports;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Utils.StoredObjects;
using System;
#if SL
using Font = DevExpress.Xpf.Drawing.Font;
using System.Windows.Media;
#endif
namespace DevExpress.XtraPrinting.Native {
	public class NullBrickOwner : IBrickOwner, IStoredObject {
		public static readonly NullBrickOwner Instance = new NullBrickOwner();
		string IBrickOwner.Name { get { return string.Empty; } }
		BrickOwnerType IBrickOwner.BrickOwnerType { get { return BrickOwnerType.Control; } }
		ControlLayoutRules IBrickOwner.LayoutRules { get { return ControlLayoutRules.None; } }
		TextEditMode IBrickOwner.TextEditMode { get { return TextEditMode.Unavailable; } }
		bool IBrickOwner.IsImageEditable { get { return false; } }
		bool? IBrickOwner.HasImage { get { return null; } }
		string IBrickOwner.Text { get { return string.Empty; } }
		string IBrickOwner.ControlsUnityName { get { return string.Empty; } }
		public bool InSubreport { get { return false; } }
		public bool IsCrossbandControl { get { return false; } }
		bool IBrickOwner.LockedInDesigner { get { return false; } }
		bool IBrickOwner.IsNavigationLink { get { return false; } }
		bool IBrickOwner.IsNavigationTarget { get { return false; } }
		bool IBrickOwner.NeedCalcContainerHeight { get { return false; } }
		bool IBrickOwner.HasPageSummary { get { return false; } }
		ConvertHelper IBrickOwner.ConvertHelper { get { return ConvertHelper.Instance; } }
		BookmarkInfo IBrickOwner.EmptyBookmarkInfo { get { return BookmarkInfo.Empty; } }
		bool IBrickOwner.CanCacheImages { get { return true; } }
		object IBrickOwner.RealControl { get { return this; } }
		DevExpress.XtraReports.UI.VerticalAnchorStyles IBrickOwner.AnchorVertical { get { return DevExpress.XtraReports.UI.VerticalAnchorStyles.None; } }
		NullBrickOwner() {
		}
		void IBrickOwner.UpdateBrickBounds(VisualBrick brick) { }
		bool IBrickOwner.IsSeparableVert(bool isBrickSeparableVert) { return isBrickSeparableVert; }
		bool IBrickOwner.IsSeparableHorz(bool isBrickSeparableHorz) { return isBrickSeparableHorz; }
		Font IBrickOwner.GetActualFont() { return null; }
		Color IBrickOwner.GetActualForeColor() {
			return DXColor.Transparent;
		}
		Color IBrickOwner.GetActualBackColor() {
			return DXColor.Transparent;
		}
		Color IBrickOwner.GetActualBorderColor() {
			return DXColor.Transparent;
		}
		float IBrickOwner.GetActualBorderWidth() {
			return 0;
		}
		BorderDashStyle IBrickOwner.GetActualBorderDashStyle() {
			return BorderDashStyle.Solid;
		}
		BorderSide IBrickOwner.GetActualBorderSide() {
			return BorderSide.None;
		} 
		TextAlignment IBrickOwner.GetActualTextAlignment() { return TextAlignment.MiddleLeft; }
		void IBrickOwner.RaiseDraw(VisualBrick brick, IGraphics gr, RectangleF bounds) { }
		bool IBrickOwner.RaiseAfterPrintOnPage(VisualBrick brick, int pageNumber, int pageCount) { return true; }
		void IBrickOwner.RaiseHtmlItemCreated(VisualBrick brick, IScriptContainer scriptContainer, DXHtmlContainerControl contentCell) { }
		void IBrickOwner.RaiseSummaryCalculated(VisualBrick brick, string text, string format, object value) { }
		void IBrickOwner.AddToSummaryUpdater(VisualBrick brick, VisualBrick prototypeBrick) { }
		#region IStoredObject Members
		long id = StoredObjectExtentions.UndefinedId;
		long IStoredObject.Id {
			get { return id; }
			set { id = value; }
		}
		byte[] IStoredObject.Store(IRepositoryProvider provider) {
			throw new NotSupportedException();
		}
		void IStoredObject.Restore(IRepositoryProvider provider, byte[] store) {
			throw new NotSupportedException();
		}
		#endregion
	}
}
