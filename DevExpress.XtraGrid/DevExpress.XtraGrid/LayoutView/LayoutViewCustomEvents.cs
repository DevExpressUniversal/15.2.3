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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Card;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Layout.ViewInfo;
namespace DevExpress.XtraGrid.Views.Layout.Events {
	public class LayoutViewCardCaptionImageEventArgs : CardCaptionImageEventArgs {
		GroupElementLocation captionImageLocationCore  = GroupElementLocation.Default;
		bool captionImageVisibleCore = true;
		public LayoutViewCardCaptionImageEventArgs(int rowHandle, object list, int imageIndex, Image image, bool imageVisible)
			: base(rowHandle, list) {
			ImageIndex = imageIndex;
			Image = image;
			CaptionImageVisible = imageVisible;
		}
		public GroupElementLocation CaptionImageLocation {
			get { return captionImageLocationCore; }
			set { captionImageLocationCore = value; }
		}
		public bool CaptionImageVisible {
			get { return captionImageVisibleCore; }
			set { captionImageVisibleCore = value; }
		}
	}
	public class LayoutViewFieldCaptionImageEventArgs : CardCaptionImageEventArgs {
		GridColumn columnCore = null;
		ContentAlignment imageAlignmentCore = ContentAlignment.MiddleCenter;
		int imageToTextDistanceCore = 0;
		public LayoutViewFieldCaptionImageEventArgs(int rowHandle, GridColumn column, object list)
			: base(rowHandle, list) {
			columnCore = column;
		}
		public GridColumn Column {
			get { return columnCore; }
		}
		public ContentAlignment ImageAlignment {
			get { return imageAlignmentCore; }
			set { imageAlignmentCore = value; }
		}
		public int ImageToTextDistance {
			get { return imageToTextDistanceCore; }
			set { imageToTextDistanceCore = value; }
		}
	}
	public class LayoutViewCustomRowCellEditEventArgs : CustomRowCellEditEventArgs {
		public LayoutViewCustomRowCellEditEventArgs(int rowHandle, GridColumn column, RepositoryItem repositoryItem)
			: base(rowHandle, column, repositoryItem) {
		}
	}
	public class LayoutViewCustomCardLayoutEventArgs : EventArgs {
		int rowHandleCore;
		LayoutViewCardDifferences differencesCore;
		public int RowHandle { get { return rowHandleCore; } }
		public LayoutViewCardDifferences CardDifferences { get { return differencesCore; } }
		public LayoutViewCustomCardLayoutEventArgs(int rowHandle, LayoutViewCardDifferences differences) {
			this.rowHandleCore = rowHandle;
			this.differencesCore = differences;
		}
	}
	public class LayoutViewVisibleRecordIndexChangedEventArgs : EventArgs {
		int visibleRecordIndexCore;
		int prevVisibleRecordIndexCore;
		public int VisibleRecordIndex { get { return visibleRecordIndexCore; } }
		public int PrevVisibleRecordIndex { get { return prevVisibleRecordIndexCore; } }
		public LayoutViewVisibleRecordIndexChangedEventArgs(int visibleRecordIndex, int prevVisibleRecordIndex) {
			this.visibleRecordIndexCore = visibleRecordIndex;
			this.prevVisibleRecordIndexCore = prevVisibleRecordIndex;
		}
	}
	public abstract class LayoutViewCustomRowStyleEventArgs : RowEventArgs { 
		AppearanceObject appearanceCore;
		GridRowCellState stateCore;
		protected LayoutViewCustomRowStyleEventArgs(int rowHandle, GridRowCellState state, AppearanceObject appearance)
			: base(rowHandle) {
			this.appearanceCore = appearance;
			this.stateCore = state;
		}
		public GridRowCellState State { 
			get { return stateCore; } 
		}
		public AppearanceObject Appearance { 
			get { return appearanceCore; } 
		}
		internal void SetAppearance(AppearanceObject appearance) {
			this.appearanceCore = appearance;
		}
		public void CombineAppearance(AppearanceObject appearance) {
			if(appearance == null || Appearance == appearance) return;
			AppearanceHelper.Combine(Appearance, appearance, Appearance);
		}
	}
	public abstract class LayoutViewCustomRowCellStyleEventArgs : LayoutViewCustomRowStyleEventArgs{
		GridColumn columnCore;
		protected LayoutViewCustomRowCellStyleEventArgs(int rowHandle, GridColumn column, GridRowCellState state, AppearanceObject appearance)
			: base(rowHandle, state, appearance) {
			columnCore = column;
		}
		public GridColumn Column {
			get { return columnCore; }
		}
	}
	public class LayoutViewCustomDrawCardCaptionEventArgs : GroupCaptionCustomDrawEventArgs {
		int rowHandleCore;
		public LayoutViewCustomDrawCardCaptionEventArgs(int rowHandle, GroupCaptionCustomDrawEventArgs args)
			: base(args.Cache, args.Painter,args.Info) {
			rowHandleCore = rowHandle;
		}
		public int RowHandle { get { return rowHandleCore; } }
		public AppearanceObject Appearance { get { return Info.AppearanceCaption; } }
		public string CardCaption { get { return Info.Caption; } set { Info.Caption = value; } }
	}
	public class LayoutViewCustomSeparatorStyleEventArgs : EventArgs {
		AppearanceObject appearanceCore;
		int widthCore;
		bool isRowCore;
		public LayoutViewCustomSeparatorStyleEventArgs(AppearanceObject appearance, int width, bool isRow) {
			this.appearanceCore = appearance;
			this.widthCore = width;
			this.isRowCore = isRow;
		}
		public bool IsRowSeparator {
			get { return isRowCore; }
		}
		public int Width {
			get { return widthCore; }
			set { widthCore = Math.Max(0, value); }
		}
		public AppearanceObject Appearance {
			get { return appearanceCore; }
		}
		internal void SetAppearance(AppearanceObject appearance) {
			this.appearanceCore = appearance;
		}
		public void CombineAppearance(AppearanceObject appearance) {
			if(appearance == null || Appearance == appearance) return;
			AppearanceHelper.Combine(Appearance, appearance, Appearance);
		}
	}
	public class LayoutViewCardStyleEventArgs : LayoutViewCustomRowStyleEventArgs {
		public LayoutViewCardStyleEventArgs(int rowHandle, GridRowCellState state, AppearanceObject appearance)
			: base(rowHandle, state, appearance) {
		}
	}
	public class LayoutViewFieldCaptionStyleEventArgs : LayoutViewCustomRowCellStyleEventArgs {
		public LayoutViewFieldCaptionStyleEventArgs(int rowHandle, GridColumn column, GridRowCellState state, AppearanceObject appearance)
			: base(rowHandle, column, state, appearance) {
		}
	}
	public class LayoutViewFieldValueStyleEventArgs : LayoutViewCustomRowCellStyleEventArgs {
		public LayoutViewFieldValueStyleEventArgs(int rowHandle, GridColumn column, GridRowCellState state, AppearanceObject appearance)
			: base(rowHandle, column, state, appearance) {
		}
	}
	public class LayoutViewFieldEditingValueStyleEventArgs : LayoutViewCustomRowCellStyleEventArgs {
		public LayoutViewFieldEditingValueStyleEventArgs(int rowHandle, GridColumn column, GridRowCellState state, AppearanceObject appearance)
			: base(rowHandle, column, state, appearance) {
		}
	}
	public delegate void LayoutViewCustomDrawCardCaptionEventHandler(object sender, LayoutViewCustomDrawCardCaptionEventArgs e);
	public delegate void LayoutViewCardStyleEventHandler(object sender, LayoutViewCardStyleEventArgs e);
	public delegate void LayoutViewFieldCaptionStyleEventHandler(object sender, LayoutViewFieldCaptionStyleEventArgs e);
	public delegate void LayoutViewFieldValueStyleEventHandler(object sender, LayoutViewFieldValueStyleEventArgs e);
	public delegate void LayoutViewFieldEditingValueStyleEventHandler(object sender, LayoutViewFieldEditingValueStyleEventArgs e);
	public delegate void LayoutViewCustomSeparatorStyleEventHandler(object sender, LayoutViewCustomSeparatorStyleEventArgs e);
	public delegate void LayoutViewCustomRowCellEditEventHandler(object sender, LayoutViewCustomRowCellEditEventArgs e);
	public delegate void LayoutViewCardCaptionImageEventHandler(object sender, LayoutViewCardCaptionImageEventArgs e);
	public delegate void LayoutViewFieldCaptionImageEventHandler(object sender, LayoutViewFieldCaptionImageEventArgs e);
	public delegate void LayoutViewCustomCardLayoutEventHandler(object sender, LayoutViewCustomCardLayoutEventArgs e);
	public delegate void LayoutViewVisibleRecordIndexChangedEventHandler(object sender, LayoutViewVisibleRecordIndexChangedEventArgs e);
	public delegate void CardClickEventHandler(object sender, CardClickEventArgs e);
	public delegate void FieldValueClickEventHandler(object sender, FieldValueClickEventArgs e);
	public class CardClickEventArgs : DXMouseEventArgs {
		int rowHandle;
		LayoutViewHitInfo hitInfo;
		LayoutViewCard card;
		internal CardClickEventArgs(DXMouseEventArgs e, LayoutViewHitInfo hitInfo)
			: this(e, hitInfo.RowHandle, hitInfo.HitCard) {
			this.hitInfo = hitInfo;
		}
		public CardClickEventArgs(DXMouseEventArgs e, int rowHandle, LayoutViewCard card)
			: base(e) {
			this.rowHandle = rowHandle;
			this.card = card;
		}
		public int RowHandle { get { return rowHandle; } }
		public LayoutViewCard Card { get { return card; } }
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public LayoutViewHitInfo HitInfo { get { return hitInfo; } set { hitInfo = value; } }
	}
	public class FieldValueClickEventArgs : CardClickEventArgs {
		LayoutViewColumn column;
		internal FieldValueClickEventArgs(DXMouseEventArgs e, LayoutViewHitInfo hitInfo)
			: this(e, hitInfo.RowHandle, hitInfo.HitCard, (LayoutViewColumn)hitInfo.Column) {
		}
		public FieldValueClickEventArgs(DXMouseEventArgs e, int rowHandle, LayoutViewCard card, LayoutViewColumn column)
			: base(e, rowHandle, card) {
			this.column = column;
		}
		public LayoutViewColumn Column { get { return column; } }
		public object FieldValue { get { return ColumnView.GetRowCellValueCore(RowHandle, Column); } }
	}
}
