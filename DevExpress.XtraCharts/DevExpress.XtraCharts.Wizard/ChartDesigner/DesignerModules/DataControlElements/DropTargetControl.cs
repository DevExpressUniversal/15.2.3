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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Wizard;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.Native;
namespace DevExpress.XtraCharts.Designer {
	public delegate void DataDroppedEventHandler(object sender, DataDroppedEventArgs e);
	public class DataDroppedEventArgs : EventArgs {
		readonly string dataMemberName;
		public string DataMemberName { get { return dataMemberName; } }
		public DataDroppedEventArgs(string dataMemberName) {
			this.dataMemberName = dataMemberName;
		}
	}
	public class DropTargetControl : LabelControl {
		bool isHot = false;
		public DataMemberInfo DataMemberInfo { get; set; }
		public object DataSource { get; set; }
		public event DataDroppedEventHandler DataDropped;
		public DropTargetControl() : base() {
			this.AllowDrop = true;
			this.Disposed += DropTargetControl_Disposed;
		}
		void DropTargetControl_Disposed(object sender, EventArgs e) {
			this.Disposed -= DropTargetControl_Disposed;
			this.AllowDrop = false;
		}
		~DropTargetControl() {
			this.Disposed -= DropTargetControl_Disposed;
			this.AllowDrop = false;
		}
		bool CanProcessProperty(ScaleType[] allowedScaleTypes, object dataSource, string dataMember) {
			if (allowedScaleTypes.Length == 0)
				return true;
			foreach (ScaleType scaleType in allowedScaleTypes)
				if (scaleType == ScaleType.Auto || BindingHelper.CheckDataMember(null, dataSource, dataMember, scaleType))
					return true;
			return false;
		}
		DragItemState CalculateCurrentState() {
			DragItemState currentState = DragItemState.PlaceHolder;
			if (isHot)
				currentState = DragItemState.Hot;
			else if (!string.IsNullOrEmpty(DataMemberInfo.SelectedDataMember))
				currentState = DragItemState.Normal;
			return currentState;
		}
		protected override void OnMouseEnter(EventArgs e) {
			base.OnMouseEnter(e);
			if (!string.IsNullOrEmpty(DataMemberInfo.SelectedDataMember)) {
				isHot = true;
				Invalidate();
			}
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			isHot = false;
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if (!string.IsNullOrEmpty(DataMemberInfo.SelectedDataMember)) {
				DragDropEffects finalEffect = DoDragDrop(DataMemberInfo.SelectedDataMember, DragDropEffects.All);
				switch (finalEffect) {
					case DragDropEffects.None:
					case DragDropEffects.Copy:
						if (DataDropped != null)
							DataDropped(this, new DataDroppedEventArgs(string.Empty));
						break;
				}
			}
		}
		protected override void OnGiveFeedback(GiveFeedbackEventArgs gfbevent) {
			base.OnGiveFeedback(gfbevent);
			switch (gfbevent.Effect) {
				case DragDropEffects.Copy:
					gfbevent.UseDefaultCursors = false;
					Cursor.Current = ResLoaderBase.LoadColoredCursor("ChartDesigner.Images.DataControl.Apply.cur", typeof(ChartWizard));
					break;
				case DragDropEffects.Scroll:
					gfbevent.UseDefaultCursors = false;
					Cursor.Current = Cursors.No;
					break;
			}
		}
		protected override void OnDragEnter(DragEventArgs drgevent) {
			base.OnDragEnter(drgevent);
			IDataObject dataObject = drgevent.Data;
			DataMemberListNodeBase dataMemberListNode = dataObject.GetData(dataObject.GetFormats()[0]) as DataMemberListNodeBase;
			if (dataMemberListNode != null) {
				if (CanProcessProperty(DataMemberInfo.AllowedScaleTypes, dataMemberListNode.DataSource, dataMemberListNode.DataMember)) {
					drgevent.Effect = DragDropEffects.Link;
					isHot = true;
				}
				else {
					drgevent.Effect = DragDropEffects.None;
					isHot = false;
				}
			}
			else {
				string dataMember = dataObject.GetData(dataObject.GetFormats()[0]) as string;
				if (!string.IsNullOrEmpty(dataMember)) {
					if (DataMemberInfo.SelectedDataMember != dataMember)
						if (DataSource != null && CanProcessProperty(DataMemberInfo.AllowedScaleTypes, DataSource, dataMember)) {
							drgevent.Effect = DragDropEffects.Copy;
							isHot = true;
						}
						else {
							drgevent.Effect = DragDropEffects.Scroll;
							isHot = false;
						}
					else {
						drgevent.Effect = DragDropEffects.Move;
						isHot = false;
					}
				}
			}
			Invalidate();
		}
		protected override void OnDragLeave(EventArgs e) {
			base.OnDragLeave(e);
			isHot = false;
			Invalidate();
		}
		protected override void OnDragDrop(DragEventArgs drgevent) {
			base.OnDragDrop(drgevent);
			if (DataDropped != null && drgevent.Effect != DragDropEffects.Scroll) {
				IDataObject dataObject = drgevent.Data;
				DataMemberListNodeBase dataMemberListNode = dataObject.GetData(dataObject.GetFormats()[0]) as DataMemberListNodeBase;
				if (dataMemberListNode != null) {
					string dataMember = string.IsNullOrEmpty(dataMemberListNode.DataMember) ? string.Empty : dataMemberListNode.DataMember;
					DataDropped(this, new DataDroppedEventArgs(dataMember));
				}
				else {
					string dataMember = dataObject.GetData(dataObject.GetFormats()[0]) as string;
					if (!string.IsNullOrEmpty(dataMember)) {
						DataDropped(this, new DataDroppedEventArgs(dataMember));
					}
				}
			}
		}
		protected override void OnPaint(PaintEventArgs e) {
			GraphicsCache cache = new GraphicsCache(e.Graphics);
			DragItemInfoArgs dragItemArgs = new DragItemInfoArgs(cache, e.ClipRectangle, Appearance, CalculateCurrentState());
			DragItemSkinPainter painter = new DragItemSkinPainter(LookAndFeel);
			ObjectPainter.DrawObject(cache, painter, dragItemArgs);
			base.OnPaint(e);
		}
	}
	public class DragItemSkinPainter : DragAreaSkinPainterBase {
		const int HierarchyItemShift = 4;
		public DragItemSkinPainter(UserLookAndFeel lookAndFeel)
			: base(lookAndFeel, DashboardSkins.SkinDragItem) {
		}
		protected override void PrepareElementInfo(SkinElementInfo elementInfo, ObjectInfoArgs e) {
			base.PrepareElementInfo(elementInfo, e);
			DragItemInfoArgs args = e as DragItemInfoArgs;
			if (args != null)
				elementInfo.Bounds = args.CurrentBounds;
			elementInfo.ImageIndex = (int)args.ItemState;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			DragItemInfoArgs args = e as DragItemInfoArgs;
			base.DrawObject(e);
		}
	}
	public class DragAreaSkinPainterBase : DragAreaPainter {
		readonly SkinElement skinElement;
		readonly int horizontalMargins;
		protected SkinElement SkinElement { get { return skinElement; } }
		public override int HorizontalMargins { get { return horizontalMargins; } }
		public DragAreaSkinPainterBase(UserLookAndFeel lookAndFeel, string elementName) {
			Skin skin = DashboardSkins.GetSkin(lookAndFeel);
			skinElement = skin == null ? null : skin[elementName];
			if (skinElement != null) {
				SkinPaddingEdges contentMargins = skinElement.ContentMargins;
				horizontalMargins = contentMargins.Left + contentMargins.Right;
			}
		}
		protected int GetIntegerProperty(string propertyName) {
			return skinElement == null ? 0 : (int)skinElement.Properties.GetInteger(propertyName);
		}
		protected Color GetColorProperty(string propertyName) {
			return skinElement == null ? Color.Empty : (Color)skinElement.Properties.GetColor(propertyName);
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return skinElement == null ? client : SkinElementPainter.Default.CalcBoundsByClientRectangle(new SkinElementInfo(skinElement, client));
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return skinElement == null ? e.Bounds : SkinElementPainter.Default.GetObjectClientRectangle(new SkinElementInfo(skinElement, e.Bounds));
		}
		public override void DrawObject(ObjectInfoArgs e) {
			SkinElementInfo elementInfo = new SkinElementInfo(skinElement, e.Bounds);
			PrepareElementInfo(elementInfo, e);
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, elementInfo);
		}
		protected virtual void PrepareElementInfo(SkinElementInfo elementInfo, ObjectInfoArgs e) {
		}
	}
	public abstract class DragAreaPainter : StyleObjectPainter, IDragAreaPainterBase {
		public ObjectPainter ObjectPainter { get { return this; } }
		public abstract int HorizontalMargins { get; }
		protected DragAreaPainter() {
		}
	}
	public class DragItemInfoArgs : StyleObjectInfoArgs {
		readonly DragItemState itemState;
		Rectangle currentBounds;
		public DragItemState ItemState { get { return itemState; } }
		public Rectangle CurrentBounds {
			get { return currentBounds; }
			set { currentBounds = value; }
		}
		public DragItemInfoArgs(GraphicsCache cache, Rectangle bounds, AppearanceObject appearance, DragItemState itemState)
			: base(cache, bounds, appearance) {
			this.itemState = itemState;
			this.currentBounds = bounds;
		}
	}
	public enum DragItemState { Normal, Hot, Selected, DropTarget, PlaceHolder, PlaceHolderDropDestination }
	public interface IDragAreaPainterBase {
		ObjectPainter ObjectPainter { get; }
		int HorizontalMargins { get; }
	}
}
