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
using DevExpress.XtraPrinting.Native;
using System.Collections.Specialized;
using System.ComponentModel;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Windows.Forms;
using DevExpress.Compatibility.System;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using System.Collections;
using DevExpress.Data.Mask;
using System.Collections.Generic;
using DevExpress.Compatibility.System.Collections.Specialized;
using System.Windows.Forms;
using DevExpress.XtraPrinting.Export;
using DevExpress.XtraPrinting.BrickExporters;
using DevExpress.XtraPrinting.Helpers;
#if DXPORTABLE
namespace DevExpress.XtraPrinting {
	public abstract class BrickBase : IXtraSupportShouldSerialize, IXtraSupportCreateContentPropertyValue, IXtraSupportDeserializeCollectionItem, ICloneable {
#else
using DevExpress.Utils.StoredObjects;
namespace DevExpress.XtraPrinting {
	[BrickExporter(typeof(BrickBaseExporter))]
	public abstract class BrickBase : StoredObjectBase, IXtraSupportShouldSerialize, IXtraSupportCreateContentPropertyValue, IXtraSupportDeserializeCollectionItem, ICloneable {
		internal long StoredId { 
			get { return ((IStoredObject)this).Id; }
		}
		protected override void StoreValues(System.IO.BinaryWriter writer, IRepositoryProvider provider) {
			writer.WriteRectangleF(fRect);
			writer.Write(flags.Data);
		}
		protected override void RestoreValues(System.IO.BinaryReader reader, IRepositoryProvider provider) {
			fRect = reader.ReadRectangleF();
			flags = new BitVector32(reader.ReadInt32());
		}
#endif //DXPORTABLE
#region static
		static readonly BitVector32.Section ModifierSection = BitVector32.CreateSection(ReportSpecificEnumHelper.GetEnumMaxValue(typeof(BrickModifier)));
		protected static readonly int bitCanPublish =  BitVector32Helper.CreateMask(ModifierSection);
		protected static readonly BitVector32.Section XlsExportNativeFormatSection = BitVector32Helper.CreateSection(ReportSpecificEnumHelper.GetEnumMaxValue(typeof(DefaultBoolean)), bitCanPublish);
		protected static readonly BitVector32.Section SeparabilitySection = BitVector32.CreateSection(ReportSpecificEnumHelper.GetEnumMaxValue(typeof(Separability)), XlsExportNativeFormatSection);
		protected static readonly BitVector32.Section AlignmentSection = BitVector32.CreateSection(ReportSpecificEnumHelper.GetEnumMaxValue(typeof(BrickAlignment)), SeparabilitySection);
		protected static readonly BitVector32.Section LineAlignmentSection = BitVector32.CreateSection(ReportSpecificEnumHelper.GetEnumMaxValue(typeof(BrickAlignment)), AlignmentSection);
		protected static readonly BitVector32.Section ImageAlignmentSection = BitVector32.CreateSection(ReportSpecificEnumHelper.GetEnumMaxValue(typeof(ImageAlignment)), SeparabilitySection);
  		protected static readonly int bitCanGrow = BitVector32Helper.CreateMask(LineAlignmentSection);
		protected static readonly int bitCanShrink = BitVector32.CreateMask(bitCanGrow);
		protected static readonly int bitCanOverflow = BitVector32.CreateMask(bitCanShrink);
		protected static readonly int bitNoClip = BitVector32.CreateMask(bitCanOverflow);
		protected static readonly int bitMerged = BitVector32.CreateMask(bitNoClip);
		protected static readonly int bitUseTextAsDefaultHint = BitVector32.CreateMask(bitMerged);
		protected static readonly int bitIsVisible = BitVector32.CreateMask(bitUseTextAsDefaultHint);
		protected static readonly int bitCanMultiColumn = BitVector32.CreateMask(bitIsVisible);
		protected static readonly int bitCanAddToPage = BitVector32.CreateMask(bitCanMultiColumn);
#endregion
		RectangleF fRect = RectangleF.Empty;
		protected BitVector32 flags = new BitVector32();
		internal virtual IList InnerBrickList {
			get { return new BrickBase[] { }; }
		}
		internal virtual PointF InnerBrickListOffset { get { return PointF.Empty; } }
		[
		DefaultValue(false),
		XtraSerializableProperty,
		]
		public virtual bool NoClip { 
			get { return flags[bitNoClip]; } 
			set { flags[bitNoClip] = value; }
		}
		[
		Obsolete("Use the CanPublish property instead."),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty,
		]
		public bool Printed {
			get { return CanPublish; }
			set { CanPublish = value; }
		}
		[
		DefaultValue(true),
		XtraSerializableProperty,
		]
		public bool CanPublish {
			get { return flags[bitCanPublish]; }
			set { flags[bitCanPublish] = value; }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("BrickBaseModifier"),
#endif
		XtraSerializableProperty]
		public BrickModifier Modifier {
			get { return (BrickModifier)flags[ModifierSection]; }
			set { flags[ModifierSection] = (int)value; }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("BrickBaseRect"),
#endif
		XtraSerializableProperty]
		public virtual RectangleF Rect {
			get { return fRect; }
			set { InitialRect = value; }
		}
		protected internal virtual RectangleF InitialRect { 
			get { return this.fRect; } 
			set { SetBoundsCore(value.X, value.Y, value.Width, value.Height, BoundsSpecified.All); } 
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("BrickBaseLocation")]
#endif
		public PointF Location {
			get { return Rect.Location; }
			set { SetBoundsCore(value.X, value.Y, 0, 0, BoundsSpecified.Location); }
		}
		internal float X {
			get { return Rect.X; }
			set { SetBoundsCore(value, 0, 0, 0, BoundsSpecified.X); }
		}
		internal float Y {
			get { return Rect.Y; }
			set { SetBoundsCore(0, value, 0, 0, BoundsSpecified.Y); }
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("BrickBaseSize")]
#endif
		public SizeF Size {
			get { return Rect.Size; }
			set { fRect.Size = value; }
		}
		internal float Width {
			get { return Rect.Width; }
			set { SetBoundsCore(0, 0, value, 0, BoundsSpecified.Width); }
		}
		internal float Height {
			get { return Rect.Height; }
			set { SetBoundsCore(0, 0, 0, value, BoundsSpecified.Height); }
		}
		internal bool IsPageBrick { 
			get { return this is IPageBrick; } 
		}
		protected BrickBase() {
			Modifier = BrickModifier.None;
			CanPublish = true;
		}
		internal BrickBase(RectangleF rect)
			: this() {
			this.fRect = rect;
		}
		internal BrickBase(BrickBase brickBase) {
			this.fRect = brickBase.Rect;
			CanPublish = true;
		}
		internal IEnumerable<BrickBase> AllBricks() {
			foreach(BrickBase item in InnerBrickList) {
				foreach(BrickBase item2 in item.AllBricks())
					yield return item2;
				yield return item;
			}
		}
		internal void SetModifierRecursive(BrickModifier value) {
			Modifier = value;
			foreach(BrickBase item in AllBricks())
				item.Modifier = value;
		}
		internal bool HasModifier(params BrickModifier[] modifiers) {
			return Array.IndexOf<BrickModifier>(modifiers, Modifier) >= 0;
		}
		internal void SetBounds(RectangleF bounds, float dpi) {
			InitialRect = GraphicsUnitConverter.Convert(bounds, dpi, GraphicsDpi.Document);
		}
		protected internal virtual RectangleF GetViewRectangle() {
			return fRect;
		}
		internal protected virtual bool AfterPrintOnPage(IList<int> indices, int pageIndex, int pageCount, Action<BrickBase> callback) {
			IList bricks = InnerBrickList;
			List<IDisposable> disposeList = null;
			for(int i = 0; i < bricks.Count; i++) {
				try {
					indices.Add(i);
					BrickBase brick = (BrickBase)bricks[i];
					if(brick.AfterPrintOnPage(indices, pageIndex, pageCount, callback) || bricks.IsFixedSize) {
						callback(brick);
						continue;
					}
					if(brick is IDisposable) {
						if(disposeList == null)
							disposeList = new List<IDisposable>();
						disposeList.Add((IDisposable)brick);
					}
					bricks.RemoveAt(i);
					i--;
				} finally {
					indices.RemoveAt(indices.Count - 1);
				}
			}
			if(disposeList != null)
				foreach(IDisposable item in disposeList)
					item.Dispose();
			return true;
		}
		bool IXtraSupportShouldSerialize.ShouldSerialize(string propertyName) {
			return ShouldSerializeCore(propertyName);
		}
		protected virtual bool ShouldSerializeCore(string propertyName) {
			if(propertyName == PrintingSystemSerializationNames.Modifier)
				return IsPageBrick || HasModifier(BrickModifier.MarginalFooter, BrickModifier.MarginalHeader);
			if(propertyName == "Printed")
				return false;
			return true;
		}
		object IXtraSupportCreateContentPropertyValue.Create(XtraItemEventArgs e) {
			return CreateContentPropertyValue(e);
		}
		protected virtual object CreateContentPropertyValue(XtraItemEventArgs e) {
			return null;
		}
		void IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			SetIndexCollectionItemCore(propertyName, e);
		}
		object IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			return CreateCollectionItemCore(propertyName, e);
		}
		protected virtual void SetIndexCollectionItemCore(string propertyName, XtraSetItemIndexEventArgs e) { 
		}
		protected virtual object CreateCollectionItemCore(string propertyName, XtraItemEventArgs e) {
			return null;
		}
		protected virtual void SetBoundsCore(float x, float y, float width, float height, BoundsSpecified specified) {
			if((specified & BoundsSpecified.X) > 0)
				fRect.X = x;
			if((specified & BoundsSpecified.Y) > 0)
				fRect.Y = y;
			if((specified & BoundsSpecified.Width) > 0)
				fRect.Width = width;
			if((specified & BoundsSpecified.Height) > 0)
				fRect.Height = height;
		}
#region ICloneable Members
		public virtual object Clone() {
			throw new NotImplementedException();
		}
#endregion
	}
}
