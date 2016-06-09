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
using System.Runtime.InteropServices;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.Office.Model;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office.DrawingML;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.Office.Drawing {
	#region GradientStopInfo (struct)
	public struct GradientStopInfo {
		#region Fields
		Color color;
		int position;
		#endregion
		#region Properties
		public Color Color { get { return color; } set { color = value; } }
		public int Position { get { return position; } set { position = value; } }
		#endregion
		public DrawingGradientStop CreateStop(IDocumentModel documentModel) {
			return DrawingGradientStop.Create(documentModel, color, position);
		}
	}
	#endregion
	#region DrawingGradientStop
	public class DrawingGradientStop : ISupportsCopyFrom<DrawingGradientStop> {
		#region Static Members
		internal static DrawingGradientStop Create(IDocumentModel documentModel, Color color, int position) {
			DrawingGradientStop result = new DrawingGradientStop(documentModel);
			result.AssignProperties(color, position);
			return result;
		}
		#endregion
		#region Fields
		readonly IDocumentModel documentModel;
		readonly InvalidateProxy innerParent;
		DrawingColor color;
		int position;
		#endregion
		public DrawingGradientStop(IDocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.innerParent = new InvalidateProxy();
			this.documentModel = documentModel;
			this.color = new DrawingColor(documentModel) { Parent = this.innerParent };
			this.position = 0;
		}
		#region Properties
		public ISupportsInvalidate Parent { get { return innerParent.Target; } set { innerParent.Target = value; } }
		public DrawingColor Color { get { return color; } }
		#region Position
		public int Position {
			get { return position; }
			set {
				ValueChecker.CheckValue(value, 0, DrawingValueConstants.ThousandthOfPercentage, "Position");
				if(position == value)
					return;
				SetPosition(value);
			}
		}
		void SetPosition(int value) {
			GradientStopPositionPropertyChangedHistoryItem historyItem = new GradientStopPositionPropertyChangedHistoryItem(documentModel.MainPart, this, position, value);
			documentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetPositionCore(int value) {
			this.position = value;
			this.innerParent.Invalidate();
		}
		#endregion
		#endregion
		void AssignProperties(Color color, int position) {
			this.color = DrawingColor.Create(documentModel, color);
			SetPositionCore(position);
		}
		#region ISupportsCopyFrom<DrawingGradientStop> Members
		public void CopyFrom(DrawingGradientStop value) {
			Guard.ArgumentNotNull(value, "value");
			this.Color.CopyFrom(value.Color);
			this.Position = value.Position;
		}
		#endregion
		public override bool Equals(object obj) {
			DrawingGradientStop other = obj as DrawingGradientStop;
			if(other == null)
				return false;
			return this.position == other.position && this.color.Equals(other.color);
		}
		public override int GetHashCode() {
			return this.position ^ this.color.GetHashCode();
		}
	}
	#endregion
	#region GradientStopCollection
	public class GradientStopCollection : UndoableCollection<DrawingGradientStop> {
		readonly InvalidateProxy innerParent;
		public GradientStopCollection(IDocumentModel documentModel)
			: base(documentModel.MainPart) {
			this.innerParent = new InvalidateProxy();
		}
		protected internal ISupportsInvalidate Parent { get { return innerParent.Target; } set { innerParent.Target = value; } }
		public override int AddCore(DrawingGradientStop item) {
			int result = base.AddCore(item);
			item.Parent = this.innerParent;
			this.innerParent.Invalidate();
			return result;
		}
		protected internal override void InsertCore(int index, DrawingGradientStop item) {
			base.InsertCore(index, item);
			item.Parent = this.innerParent;
			this.innerParent.Invalidate();
		}
		public override void RemoveAtCore(int index) {
			DrawingGradientStop item = this[index];
			item.Parent = null;
			base.RemoveAtCore(index);
			this.innerParent.Invalidate();
		}
		public override void ClearCore() {
			if (Count == 0)
				return;
			foreach (DrawingGradientStop item in this)
				item.Parent = null;
			base.ClearCore();
			this.innerParent.Invalidate();
		}
		public override void AddRangeCore(IEnumerable<DrawingGradientStop> collection) {
			foreach (DrawingGradientStop item in collection)
				item.Parent = this.innerParent;
			base.AddRangeCore(collection);
			this.innerParent.Invalidate();
		}
		internal void AddStopsFromInfoes(IList<GradientStopInfo> infoes) {
			int count = infoes.Count;
			for (int i = 0; i < count; i++) 
				InnerList.Add(infoes[i].CreateStop(DocumentModel));
		}
	}
	#endregion
	#region GradientType
	public enum GradientType {
		Linear = 0,
		Rectangle,
		Circle,
		Shape
	}
	#endregion
	#region DrawingGradientFillInfo
	public class DrawingGradientFillInfo : ICloneable<DrawingGradientFillInfo>, ISupportsCopyFrom<DrawingGradientFillInfo>, ISupportsSizeOf {
		#region Static Members
		internal static DrawingGradientFillInfo Create(GradientType type) {
			DrawingGradientFillInfo result = new DrawingGradientFillInfo();
			result.GradientType = type;
			return result;
		}
		#endregion
		#region Fields
		const int offsetGradientFlip = 2;
		const int offsetAngle = 6;
		const uint maskGradientType = 0x0003;
		const uint maskGradientFlip = 0x000c;
		const uint maskRotateWithShape = 0x0010;
		const uint maskScaled = 0x0020;
		const uint maskAngle = 0x7fffffc0;
		uint packedValues = 0x00000000;
		#endregion
		#region Properties
		public GradientType GradientType {
			get { return (GradientType)PackedValues.GetIntBitValue(this.packedValues, maskGradientType); }
			set { PackedValues.SetIntBitValue(ref this.packedValues, maskGradientType, (int)value); }
		}
		public TileFlipType Flip {
			get { return (TileFlipType)PackedValues.GetIntBitValue(this.packedValues, maskGradientFlip, offsetGradientFlip); }
			set { PackedValues.SetIntBitValue(ref this.packedValues, maskGradientFlip, offsetGradientFlip, (int)value); }
		}
		public bool RotateWithShape {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskRotateWithShape); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskRotateWithShape, value); }
		}
		public bool Scaled {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskScaled); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskScaled, value); }
		}
		public int Angle {
			get { return PackedValues.GetIntBitValue(this.packedValues, maskAngle, offsetAngle); }
			set {
				ValueChecker.CheckValue(value, 0, DrawingValueConstants.MaxPositiveFixedAngle, "Angle");
				PackedValues.SetIntBitValue(ref this.packedValues, maskAngle, offsetAngle, value); 
			}
		}
		#endregion
		#region ICloneable<DrawingGradientFillInfo> Members
		public DrawingGradientFillInfo Clone() {
			DrawingGradientFillInfo result = new DrawingGradientFillInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<DrawingGradientFillInfo> Members
		public void CopyFrom(DrawingGradientFillInfo value) {
			Guard.ArgumentNotNull(value, "value");
			this.packedValues = value.packedValues;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		public override bool Equals(object obj) {
			DrawingGradientFillInfo other = obj as DrawingGradientFillInfo;
			if(other == null)
				return false;
			return this.packedValues == other.packedValues;
		}
		public override int GetHashCode() {
			return (int)this.packedValues;
		}
	}
	#endregion
	#region DrawingGradientFillInfoCache
	public class DrawingGradientFillInfoCache : UniqueItemsCache<DrawingGradientFillInfo> {
		public DrawingGradientFillInfoCache(IDocumentModelUnitConverter converter)
			: base(converter) {
		}
		protected override DrawingGradientFillInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new DrawingGradientFillInfo();
		}
	}
	#endregion
	#region DrawingGradientFill
	public class DrawingGradientFill : DrawingUndoableIndexBasedObject<DrawingGradientFillInfo>, ISupportsCopyFrom<DrawingGradientFill>, IDrawingFill, IUnderlineFill {
		#region Static Members
		public static DrawingGradientFill Create(IDocumentModel documentModel, GradientType type, IList<GradientStopInfo> stopInfoes) {
			DrawingGradientFill result = new DrawingGradientFill(documentModel);
			result.AssignInfo(DrawingGradientFillInfo.Create(type));
			result.GradientStops.AddStopsFromInfoes(stopInfoes);
			return result;
		}
		#endregion
		#region Fields
		GradientStopCollection gradientStops;
		RectangleOffset tileRect;
		RectangleOffset fillRect;
		#endregion
		public DrawingGradientFill(IDocumentModel documentModel)
			: base(documentModel.MainPart) {
			this.gradientStops = new GradientStopCollection(documentModel) { Parent = InnerParent };
			this.tileRect = RectangleOffset.Empty;
			this.fillRect = RectangleOffset.Empty;
		}
		#region Properties
		public GradientStopCollection GradientStops { get { return gradientStops; } }
		#region GradientType
		public GradientType GradientType {
			get { return Info.GradientType; }
			set {
				if(GradientType == value)
					return;
				SetPropertyValue(SetGradientTypeCore, value);
			}
		}
		DocumentModelChangeActions SetGradientTypeCore(DrawingGradientFillInfo info, GradientType value) {
			info.GradientType = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Flip
		public TileFlipType Flip {
			get { return Info.Flip; }
			set {
				if(Flip == value)
					return;
				SetPropertyValue(SetFlipCore, value);
			}
		}
		DocumentModelChangeActions SetFlipCore(DrawingGradientFillInfo info, TileFlipType value) {
			info.Flip = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region RotateWithShape
		public bool RotateWithShape {
			get { return Info.RotateWithShape; }
			set {
				if(RotateWithShape == value)
					return;
				SetPropertyValue(SetRotateWithShapeCore, value);
			}
		}
		DocumentModelChangeActions SetRotateWithShapeCore(DrawingGradientFillInfo info, bool value) {
			info.RotateWithShape = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Scaled
		public bool Scaled {
			get { return Info.Scaled; }
			set {
				if(Scaled == value)
					return;
				SetPropertyValue(SetScaledCore, value);
			}
		}
		DocumentModelChangeActions SetScaledCore(DrawingGradientFillInfo info, bool value) {
			info.Scaled = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Angle
		public int Angle {
			get { return Info.Angle; }
			set {
				if(Angle == value)
					return;
				SetPropertyValue(SetAngleCore, value);
			}
		}
		DocumentModelChangeActions SetAngleCore(DrawingGradientFillInfo info, int value) {
			info.Angle = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region TileRect
		public RectangleOffset TileRect {
			get { return tileRect; }
			set {
				if(tileRect.Equals(value))
					return;
				SetTileRect(value);
			}
		}
		void SetTileRect(RectangleOffset value) {
			GradientTileRectPropertyChangedHistoryItem historyItem = new GradientTileRectPropertyChangedHistoryItem(DocumentModel.MainPart, this, tileRect, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetTileRectCore(RectangleOffset value) {
			this.tileRect = value;
			InvalidateParent();
		}
		#endregion
		#region FillRect
		public RectangleOffset FillRect {
			get { return fillRect; }
			set {
				if(fillRect.Equals(value))
					return;
				SetFillRect(value);
			}
		}
		void SetFillRect(RectangleOffset value) {
			GradientFillRectPropertyChangedHistoryItem historyItem = new GradientFillRectPropertyChangedHistoryItem(DocumentModel.MainPart, this, fillRect, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetFillRectCore(RectangleOffset value) {
			this.fillRect = value;
			InvalidateParent();
		}
		#endregion
		#endregion
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected internal override UniqueItemsCache<DrawingGradientFillInfo> GetCache(IDocumentModel documentModel) {
			return DrawingCache.DrawingGradientFillInfoCache;
		}
		#region ISupportsCopyFrom<DrawingGradientFill> Members
		public void CopyFrom(DrawingGradientFill value) {
			Guard.ArgumentNotNull(value, "value");
			base.CopyFrom(value);
			TileRect = value.TileRect;
			FillRect = value.FillRect;
			GradientStops.Clear();
			for(int i = 0; i < value.GradientStops.Count; i++) {
				DrawingGradientStop source = value.GradientStops[i];
				DrawingGradientStop item = new DrawingGradientStop(DocumentModel);
				item.CopyFrom(source);
				GradientStops.Add(item);
			}
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			DrawingGradientFill other = obj as DrawingGradientFill;
			if (other == null)
				return false;
			if (other.FillType != DrawingFillType.Gradient)
				return false;
			DrawingGradientFill fill = other;
			if (!Info.Equals(fill.Info))
				return false;
			if (!FillRect.Equals(fill.FillRect))
				return false;
			if (!TileRect.Equals(fill.TileRect))
				return false;
			if (GradientStops.Count != fill.GradientStops.Count)
				return false;
			for (int i = 0; i < GradientStops.Count; i++) {
				if (!GradientStops[i].Equals(fill.GradientStops[i]))
					return false;
			}
			return true;
		}
		public override int GetHashCode() {
			return Info.GetHashCode() ^ gradientStops.GetHashCode() ^ tileRect.GetHashCode() ^ fillRect.GetHashCode();
		}
		#endregion
		#region IDrawingFill Members
		public DrawingFillType FillType { get { return DrawingFillType.Gradient; } }
		public IDrawingFill CloneTo(IDocumentModel documentModel) {
			DrawingGradientFill result = new DrawingGradientFill(documentModel);
			result.CopyFrom(this);
			return result;
		}
		public void Visit(IDrawingFillVisitor visitor) {
			visitor.Visit(this);
		}
		#endregion
		#region IUnderlineFill Members
		DrawingUnderlineFillType IUnderlineFill.Type { get { return DrawingUnderlineFillType.Fill; } }
		IUnderlineFill IUnderlineFill.CloneTo(IDocumentModel documentModel) {
			return CloneTo(documentModel) as IUnderlineFill;
		}
		#endregion
	}
	#endregion
}
