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
using DevExpress.Office;
using DevExpress.Office.DrawingML;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.Office.Model;
namespace DevExpress.Office.Drawing {	
	#region DrawingBlipHistoryItems
	#region DrawingBlipEmbeddedChangeHistoryItem
	public class DrawingBlipEmbeddedChangeHistoryItem : DrawingHistoryItem<DrawingBlip, bool> {
		public DrawingBlipEmbeddedChangeHistoryItem(DrawingBlip owner, bool oldValue, bool newValue)
			: base(owner.DocumentModel.MainPart, owner, oldValue, newValue) {
		}
		protected override void UndoCore() {
			Owner.SetEmbeddedCore(OldValue);
		}
		protected override void RedoCore() {
			Owner.SetEmbeddedCore(NewValue);
		}
	}
	#endregion
	#region DrawingBlipLinkChangeHistoryItem
	public class DrawingBlipLinkChangeHistoryItem : DrawingHistoryItem<DrawingBlip, string> {
		public DrawingBlipLinkChangeHistoryItem(DrawingBlip owner, string oldValue, string newValue)
			: base(owner.DocumentModel.MainPart, owner, oldValue, newValue) {
		}
		protected override void UndoCore() {
			Owner.SetLinkCore(OldValue);
		}
		protected override void RedoCore() {
			Owner.SetLinkCore(NewValue);
		}
	}
	#endregion
	#region DrawingBlipCompressionStateHistoryItem
	public class DrawingBlipCompressionStateHistoryItem : DrawingHistoryItem<DrawingBlip, CompressionState> {
		public DrawingBlipCompressionStateHistoryItem(DrawingBlip owner, CompressionState oldValue, CompressionState newValue)
			: base(owner.DocumentModel.MainPart, owner, oldValue, newValue) {
		}
		protected override void UndoCore() {
			Owner.SetCompressionStateCore(OldValue);
		}
		protected override void RedoCore() {
			Owner.SetCompressionStateCore(NewValue);
		}
	}
	#endregion
	#region DrawingBlipImageChangeHistoryItem
	public class DrawingBlipImageChangeHistoryItem : DrawingHistoryItem<DrawingBlip, OfficeImage> {
		public DrawingBlipImageChangeHistoryItem(DrawingBlip owner, OfficeImage oldValue, OfficeImage newValue)
			: base(owner.DocumentModel.MainPart, owner, oldValue, newValue) {
		}
		protected override void UndoCore() {
			Owner.SetImageCore(OldValue);
		}
		protected override void RedoCore() {
			Owner.SetImageCore(NewValue);
		}
	}
	#endregion
	#endregion
	#region DrawingBlipFillHistoryItems
	public abstract class RectangleOffsetHistoryItem : HistoryItem {
		#region Fields
		RectangleOffset oldValue;
		RectangleOffset newValue;
		DrawingBlipFill blipFill;
		#endregion
		protected RectangleOffsetHistoryItem(DrawingBlipFill blipFill, RectangleOffset oldValue, RectangleOffset newValue)
			: base(blipFill.DocumentModel.MainPart) {
			this.blipFill = blipFill;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		#region Properties
		public DrawingBlipFill BlipFill { get { return blipFill; } }
		public RectangleOffset OldValue { get { return oldValue; } }
		public RectangleOffset NewValue { get { return newValue; } }
		#endregion
	}
	public class SourceRectangleHistoryItem : RectangleOffsetHistoryItem {
		public SourceRectangleHistoryItem(DrawingBlipFill blipFill, RectangleOffset oldValue, RectangleOffset newValue)
			: base(blipFill, oldValue, newValue) {
		}
		protected override void UndoCore() {
			BlipFill.SetSourceRectangleCore(OldValue);
		}
		protected override void RedoCore() {
			BlipFill.SetSourceRectangleCore(NewValue);
		}
	}
	public class FillRectangleHistoryItem : RectangleOffsetHistoryItem {
		public FillRectangleHistoryItem(DrawingBlipFill blipFill, RectangleOffset oldValue, RectangleOffset newValue)
			: base(blipFill, oldValue, newValue) {
		}
		protected override void UndoCore() {
			BlipFill.SetFillRectangleCore(OldValue);
		}
		protected override void RedoCore() {
			BlipFill.SetFillRectangleCore(NewValue);
		}
	}
	#region DrawingBlipFillHistoryItem
	public abstract class DrawingBlipFillHistoryItem : IndexChangedHistoryItemCore<DocumentModelChangeActions> {
		readonly DrawingBlipFill obj;
		static IDocumentModelPart GetModelPart(DrawingBlipFill obj) {
			Guard.ArgumentNotNull(obj, "obj");
			return obj.DocumentModel.MainPart;
		}
		protected DrawingBlipFillHistoryItem(DrawingBlipFill obj)
			: base(GetModelPart(obj)) {
			this.obj = obj;
		}
		protected DrawingBlipFill Object { get { return obj; } }
		public override IIndexBasedObject<DocumentModelChangeActions> GetObject() {
			return null;
		}
	}
	#endregion
	#region DrawingBlipFillInfoIndexChangeHistoryItem
	public class DrawingBlipFillInfoIndexChangeHistoryItem : DrawingBlipFillHistoryItem {
		public DrawingBlipFillInfoIndexChangeHistoryItem(DrawingBlipFill obj)
			: base(obj) {
		}
		protected override void UndoCore() {
			Object.SetIndexCore(DrawingBlipFill.FillInfoIndexAccessor, OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			Object.SetIndexCore(DrawingBlipFill.FillInfoIndexAccessor, NewIndex, ChangeActions);
		}
	}
	#endregion
	#region DrawingBlipTileInfoIndexChangeHistoryItem
	public class DrawingBlipTileInfoIndexChangeHistoryItem : DrawingBlipFillHistoryItem {
		public DrawingBlipTileInfoIndexChangeHistoryItem(DrawingBlipFill obj)
			: base(obj) {
		}
		protected override void UndoCore() {
			Object.SetIndexCore(DrawingBlipFill.TileInfoIndexAccessor, OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			Object.SetIndexCore(DrawingBlipFill.TileInfoIndexAccessor, NewIndex, ChangeActions);
		}
	}
	#endregion
	#endregion
	#region PatternTypePropertyChangedHistoryItem
	public class PatternTypePropertyChangedHistoryItem : DrawingHistoryItem<DrawingPatternFill, DrawingPatternType> {
		public PatternTypePropertyChangedHistoryItem(IDocumentModelPart documentModelPart, DrawingPatternFill owner, DrawingPatternType oldValue, DrawingPatternType newValue)
			: base(documentModelPart, owner, oldValue, newValue) {
		}
		protected override void UndoCore() {
			Owner.SetPatternTypeCore(OldValue);
		}
		protected override void RedoCore() {
			Owner.SetPatternTypeCore(NewValue);
		}
	}
	#endregion
	#region GradientStopPositionPropertyChangedHistoryItem
	public class GradientStopPositionPropertyChangedHistoryItem : DrawingHistoryItem<DrawingGradientStop, int> {
		public GradientStopPositionPropertyChangedHistoryItem(IDocumentModelPart documentModelPart, DrawingGradientStop owner, int oldValue, int newValue)
			: base(documentModelPart, owner, oldValue, newValue) {
		}
		protected override void UndoCore() {
			Owner.SetPositionCore(OldValue);
		}
		protected override void RedoCore() {
			Owner.SetPositionCore(NewValue);
		}
	}
	#endregion
	#region GradientTileRectPropertyChangedHistoryItem
	public class GradientTileRectPropertyChangedHistoryItem : DrawingHistoryItem<DrawingGradientFill, RectangleOffset> {
		public GradientTileRectPropertyChangedHistoryItem(IDocumentModelPart documentModelPart, DrawingGradientFill owner, RectangleOffset oldValue, RectangleOffset newValue)
			: base(documentModelPart, owner, oldValue, newValue) {
		}
		protected override void UndoCore() {
			Owner.SetTileRectCore(OldValue);
		}
		protected override void RedoCore() {
			Owner.SetTileRectCore(NewValue);
		}
	}
	#endregion
	#region GradientFillRectPropertyChangedHistoryItem
	public class GradientFillRectPropertyChangedHistoryItem : DrawingHistoryItem<DrawingGradientFill, RectangleOffset> {
		public GradientFillRectPropertyChangedHistoryItem(IDocumentModelPart documentModelPart, DrawingGradientFill owner, RectangleOffset oldValue, RectangleOffset newValue)
			: base(documentModelPart, owner, oldValue, newValue) {
		}
		protected override void UndoCore() {
			Owner.SetFillRectCore(OldValue);
		}
		protected override void RedoCore() {
			Owner.SetFillRectCore(NewValue);
		}
	}
	#endregion
	#region DrawingFillPropertyChangedHistoryItem
	public class DrawingFillPropertyChangedHistoryItem : DrawingHistoryItem<IFillOwner, IDrawingFill> {
		public DrawingFillPropertyChangedHistoryItem(IDocumentModelPart documentModelPart, IFillOwner owner, IDrawingFill oldValue, IDrawingFill newValue)
			: base(documentModelPart, owner, oldValue, newValue) {
		}
		protected override void UndoCore() {
			Owner.SetDrawingFillCore(OldValue);
		}
		protected override void RedoCore() {
			Owner.SetDrawingFillCore(NewValue);
		}
	}
	#endregion
	#region Scene3DVectorChangeHistoryItem
	public class Scene3DVectorCoordinateChangeHistoryItem : DrawingHistoryItem<Scene3DVector, long> {
		public Scene3DVectorCoordinateChangeHistoryItem(Scene3DVector owner, int index, long oldValue, long newValue)
			: base(owner.DocumentModel.MainPart, owner, index, oldValue, newValue) {
		}
		protected override void UndoCore() {
		   Owner.SetCoordinateCore(Index, OldValue);
		}
		protected override void RedoCore() {
		   Owner.SetCoordinateCore(Index, NewValue);
		}
	}
	#endregion
	#region Scene3DPropertiesHistoryItems
	#region Scene3DPropertiesHistoryItem (abstract class)
	public abstract class Scene3DPropertiesHistoryItem : IndexChangedHistoryItemCore<DocumentModelChangeActions> {
		readonly Scene3DProperties obj;
		static IDocumentModelPart GetModelPart(Scene3DProperties obj) {
			Guard.ArgumentNotNull(obj, "obj");
			return obj.DocumentModel.MainPart;
		}
		protected Scene3DPropertiesHistoryItem(Scene3DProperties obj)
			: base(GetModelPart(obj)) {
			this.obj = obj;
		}
		protected Scene3DProperties Object { get { return obj; } }
		public override IIndexBasedObject<DocumentModelChangeActions> GetObject() {
			return null;
		}
	}
	#endregion
	#region Scene3DPropertiesInfoIndexChangeHistoryItem
	public class Scene3DPropertiesInfoIndexChangeHistoryItem : Scene3DPropertiesHistoryItem {
		public Scene3DPropertiesInfoIndexChangeHistoryItem(Scene3DProperties obj)
			: base(obj) {
		}
		protected override void UndoCore() {
			Object.SetIndexCore(Scene3DProperties.InfoIndexAccessor, OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			Object.SetIndexCore(Scene3DProperties.InfoIndexAccessor, NewIndex, ChangeActions);
		}
	}
	#endregion
	#region Scene3DCameraRotationInfoIndexChangeHistoryItem
	public class Scene3DCameraRotationInfoIndexChangeHistoryItem : Scene3DPropertiesHistoryItem {
		public Scene3DCameraRotationInfoIndexChangeHistoryItem(Scene3DProperties obj)
			: base(obj) {
		}
		protected override void UndoCore() {
			Object.SetIndexCore(Scene3DProperties.CameraRotationInfoIndexAccessor, OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			Object.SetIndexCore(Scene3DProperties.CameraRotationInfoIndexAccessor, NewIndex, ChangeActions);
		}
	}
	#endregion
	#region Scene3DLightRigRotationInfoIndexChangeHistoryItem
	public class Scene3DLightRigRotationInfoIndexChangeHistoryItem : Scene3DPropertiesHistoryItem {
		public Scene3DLightRigRotationInfoIndexChangeHistoryItem(Scene3DProperties obj)
			: base(obj) {
		}
		protected override void UndoCore() {
			Object.SetIndexCore(Scene3DProperties.LightRigRotationInfoIndexAccessor, OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			Object.SetIndexCore(Scene3DProperties.LightRigRotationInfoIndexAccessor, NewIndex, ChangeActions);
		}
	}
	#endregion
	#endregion
	#region Shape3DPropertiesCoordinateChangeHistoryItem
	public class Shape3DPropertiesCoordinateChangeHistoryItem<TPreset> : DrawingHistoryItem<Shape3DPropertiesBase<TPreset>, long> {
		public Shape3DPropertiesCoordinateChangeHistoryItem(Shape3DPropertiesBase<TPreset> owner, int index, long oldValue, long newValue)
			: base(owner.DocumentModel.MainPart, owner, index, oldValue, newValue) {
		}
		protected override void UndoCore() {
			Owner.SetCoordinateCore(Index, OldValue);
		}
		protected override void RedoCore() {
			Owner.SetCoordinateCore(Index, NewValue);
		}
	}
	#endregion
	#region Shape3DPropertiesPresetChangeHistoryItem
	public class Shape3DPropertiesPresetChangeHistoryItem<TPreset> : DrawingHistoryItem<Shape3DPropertiesBase<TPreset>, TPreset> {
		public Shape3DPropertiesPresetChangeHistoryItem(Shape3DPropertiesBase<TPreset> owner, TPreset oldValue, TPreset newValue)
			: base(owner.DocumentModel.MainPart, owner, oldValue, newValue) {
		}
		protected override void UndoCore() {
			Owner.SetPresetCore(OldValue);
		}
		protected override void RedoCore() {
			Owner.SetPresetCore(NewValue);
		}
	}
	#endregion
	#region DrawingContainerEffectNameChangedHistoryItem
	public class DrawingContainerEffectNameChangedHistoryItem : DrawingHistoryItem<ContainerEffect, string> {
		public DrawingContainerEffectNameChangedHistoryItem(IDocumentModelPart documentModelPart, ContainerEffect owner, string oldValue, string newValue)
			: base(documentModelPart, owner, oldValue, newValue) {
		}
		protected override void UndoCore() {
			Owner.SetNameCore(OldValue);
		}
		protected override void RedoCore() {
			Owner.SetNameCore(NewValue);
		}
	}
	#endregion
	#region DrawingContainerEffectTypeChangedHistoryItem
	public class DrawingContainerEffectTypeChangedHistoryItem : DrawingHistoryItem<ContainerEffect, DrawingEffectContainerType> {
		public DrawingContainerEffectTypeChangedHistoryItem(IDocumentModelPart documentModelPart, ContainerEffect owner, DrawingEffectContainerType oldValue, DrawingEffectContainerType newValue)
			: base(documentModelPart, owner, oldValue, newValue) {
		}
		protected override void UndoCore() {
			Owner.SetTypeCore(OldValue);
		}
		protected override void RedoCore() {
			Owner.SetTypeCore(NewValue);
		}
	}
	#endregion
	#region DrawingContainerEffectHasEffectsListChangedHistoryItem
	public class DrawingContainerEffectHasEffectsListChangedHistoryItem : DrawingHistoryItem<ContainerEffect, bool> {
		public DrawingContainerEffectHasEffectsListChangedHistoryItem(IDocumentModelPart documentModelPart, ContainerEffect owner, bool oldValue, bool newValue)
			: base(documentModelPart, owner, oldValue, newValue) {
		}
		protected override void UndoCore() {
			Owner.SetHasEffectsListCore(OldValue);
		}
		protected override void RedoCore() {
			Owner.SetHasEffectsListCore(NewValue);
		}
	}
	#endregion
}
