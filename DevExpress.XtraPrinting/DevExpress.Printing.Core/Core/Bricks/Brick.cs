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

using DevExpress.XtraPrinting.Native;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.Export;
using DevExpress.Utils;
using DevExpress.XtraPrinting.BrickExporters;
#if DXPORTABLE
namespace DevExpress.XtraPrinting {
	public class Brick : BrickBase, IBaseBrick, IDisposable, ITableCell, IEnumerable {
#else
using DevExpress.Utils.StoredObjects;
namespace DevExpress.XtraPrinting {
	[BrickExporterAttribute(typeof(BrickExporter))]
	public class Brick : BrickBase, IBaseBrick, IDisposable, ITableCell, IEnumerable {
		protected override void StoreValues(System.IO.BinaryWriter writer, IRepositoryProvider provider) {
			base.StoreValues(writer, provider);
			if(fBrickData == null) {
				writer.Write(0);
				return;
			}
			writer.Write(fBrickData.Length);
			foreach(AttachedPropertyValue item in fBrickData) {
				AttachedPropertyBase prop = AttachedPropertyBase.GetProperty(item.PropertyIndex);
				IStoreAgent agent;
				if(!StoreAgentRepository.TryGetAgent(prop.PropertyType, out agent))
					throw new InvalidOperationException();
				writer.Write(item.PropertyIndex);
				agent.StoreObject(provider, item.Value, writer);
			}
		}
		protected override void RestoreValues(System.IO.BinaryReader reader, IRepositoryProvider provider) {
			base.RestoreValues(reader, provider);
			int length = reader.ReadInt32();
			if(length == 0) return;
			ITypeProvider typeProvider = provider.GetService(typeof(ITypeProvider)) as ITypeProvider;
			fBrickData = new AttachedPropertyValue[length];
			for(int i = 0; i < length; i++) {
				Int16 propertyIndex = reader.ReadInt16();
				AttachedPropertyBase prop = AttachedPropertyBase.GetProperty(propertyIndex);
				IStoreAgent agent;
				if(!StoreAgentRepository.TryGetAgent(prop.PropertyType, out agent))
					throw new InvalidOperationException();
				object value = agent.RestoreObject(provider, reader);
				fBrickData[i] = new AttachedPropertyValue(propertyIndex) { Value = value };
			}
		}
#endif //DXPORTABLE
		const string ValuePropertyName = "Value";
		protected PointF pageBuilderOffset = PointF.Empty;
		internal PointF PageBuilderOffset {
			get { return pageBuilderOffset; }
			set { pageBuilderOffset = value; }
		}
		AttachedPropertyValue[] fBrickData;
#if DEBUGTEST
		internal AttachedPropertyValue[] BrickDataCore {
			get { return fBrickData; }
		}
#endif
		internal override IList InnerBrickList {
			get { return Bricks; }
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected internal virtual bool AllowHitTest { get { return true; } }
#region // Obsolete
		[
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("Use the SetAttachedValue(AttachedProperty prop, object value) method instead")
		]
		public void SetAttachedValue(string name, object value) {
			throw new NotSupportedException();
		}
		[
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("Use the TryGetAttachedValue(AttachedProperty prop, out object value) method instead")
		]
		public bool TryGetAttachedValue(string name, out object value) {
			throw new NotSupportedException();
		}
		[
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("Use the SafeGetAttachedValue(AttachedProperty prop, object defaultValue) method instead")
		]
		public T SafeGetAttachedValue<T>(string propertyName) {
			throw new NotSupportedException();
		}
		[
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("Use the SafeGetAttachedValue(AttachedProperty prop, object defaultValue) method instead")
		]
		public object SafeGetAttachedValue(string propertyName, object defaultValue) {
			throw new NotSupportedException();
		}
#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetAttachedValue<T>(AttachedProperty<T> prop, T value) {
			if(fBrickData == null) {
				fBrickData = new AttachedPropertyValue[] { new AttachedPropertyValue(prop.Index) { Value = value } };
				return;
			}
			int index = GetDataIndex(prop.Index);
			if(index >= 0) {
				fBrickData[index].Value = value;
				return;
			}
			AttachedPropertyValue[] newData = new AttachedPropertyValue[fBrickData.Length + 1];
			Array.Copy(fBrickData, newData, fBrickData.Length);
			newData[newData.Length - 1] = new AttachedPropertyValue(prop.Index) { Value = value };
			fBrickData = newData;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool TryGetAttachedValue<T>(AttachedProperty<T> prop, out T value) {
			int index = GetDataIndex(prop.Index);
			if(index >= 0) {
				value = GetDataValue<T>(index);
				return true;
			}
			value = default(T);
			return false;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public T SafeGetAttachedValue<T>(AttachedProperty<T> prop, T defaultValue) {
			int index = GetDataIndex(prop.Index);
			return index >= 0 ? GetDataValue<T>(index) : defaultValue;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool TryExtractAttachedValue<T>(AttachedProperty<T> prop, out T value) {
			int index = GetDataIndex(prop.Index);
			if(index >= 0) {
				value = GetDataValue<T>(index);
				RemoveAttachedValue(index);
				return true;
			}
			value = default(T);
			return false;
		}
		void RemoveAttachedValue(int index) {
			if(fBrickData.Length == 1) {
				System.Diagnostics.Debug.Assert(index == 0);
				fBrickData = null;
				return;
			}
			AttachedPropertyValue[] newData = new AttachedPropertyValue[fBrickData.Length - 1];
			Array.Copy(fBrickData, 0, newData, 0, index);
			Array.Copy(fBrickData, index + 1, newData, index, fBrickData.Length - index - 1);
		}
		protected void SetAttachedValue<T>(AttachedProperty<T> prop, T value, T defaultValue) {
			if(!object.Equals(value, defaultValue)) {
				SetAttachedValue(prop, value);
				return;
			}
			if(fBrickData != null && fBrickData.Length > 0) {
				int index = GetDataIndex(prop.Index);
				if(index >= 0)
					RemoveAttachedValue(index);
			}
		}
		protected T GetValue<T>(AttachedProperty<T> prop, T defaultValue) {
			int index = GetDataIndex(prop.Index);
			return index >= 0 ? GetDataValue<T>(index) : defaultValue;
		}
		protected T GetDataValue<T>(int index) {
			return (T)fBrickData[index].Value;
		}
		protected int GetDataIndex(int propIndex) {
			if(fBrickData != null)
				return Array.FindIndex<AttachedPropertyValue>(fBrickData, item => propIndex == item.PropertyIndex);
			return -1;
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("BrickPrintingSystem")]
#endif
		public virtual PrintingSystemBase PrintingSystem { get { return null; } set { } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("BrickUrl"),
#endif
		XtraSerializableProperty,
		DefaultValue(""),
		]
		public string Url {
			get { return GetValue(BrickAttachedProperties.Url, string.Empty); }
			set { SetAttachedValue(BrickAttachedProperties.Url, value ?? string.Empty, string.Empty); }
		}
		internal string GetActualUrl() {
			string value = Url;
			return !string.Equals(Url, SR.BrickEmptyUrl, StringComparison.OrdinalIgnoreCase) ? value : string.Empty;
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("BrickHint"),
#endif
		XtraSerializableProperty,
		DefaultValue(""),
		]
		public virtual string Hint {
			get { return GetValue(BrickAttachedProperties.Hint, string.Empty); }
			set { SetAttachedValue(BrickAttachedProperties.Hint, value ?? string.Empty, string.Empty); }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("BrickID"),
#endif
		XtraSerializableProperty,
		DefaultValue(""),
		]
		public string ID {
			get { return GetValue(BrickAttachedProperties.Id, string.Empty); }
			set { SetAttachedValue(BrickAttachedProperties.Id, value ?? string.Empty, string.Empty); }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("BrickValue"),
#endif
		XtraSerializableProperty,
		DefaultValue(""),
		]
		public object Value {
			get { return GetValue(BrickAttachedProperties.Value, string.Empty); }
			set { SetAttachedValue(BrickAttachedProperties.Value, value, string.Empty); }
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("BrickRect")]
#endif
		public override RectangleF Rect {
			get {
				return RectF.Offset(base.Rect, pageBuilderOffset.X, pageBuilderOffset.Y);
			}
			set {
				System.Diagnostics.Debug.Assert(!IsInitialized, "Brick rectangle has been already set");
				if (!IsInitialized)
					base.Rect = RectF.Offset(value, -pageBuilderOffset.X, -pageBuilderOffset.Y);
			}
		}
		internal virtual RectangleF DocumentBandRect {
			get { return this.InitialRect; } 
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("BrickSeparableHorz")]
#endif
		public virtual bool SeparableHorz {
			get { return (Separability & (Separability.Horz | Separability.HorzForced)) > 0; }
			set {
				if (CanBeSeparabled) {
					SetSeparability(Separability.Horz, value);
				}
			}
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("BrickSeparableVert")]
#endif
		public virtual bool SeparableVert {
			get { return (Separability & (Separability.Vert | Separability.VertForced)) > 0; }
			set {
				if (CanBeSeparabled) {
					SetSeparability(Separability.Vert, value);
				}
			}
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("BrickSeparable")]
#endif
		public virtual bool Separable {
			get { return (Separability & (Separability.VertHorz | Separability.VertHorzForced)) > 0; }
			set {
				if (CanBeSeparabled)
					SetSeparability(Separability.VertHorz, value);
			}
		}
		protected internal bool IsInitialized {
			get { return PrintingSystem != null; }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("BrickParent"),
#endif
		Obsolete(),
		]
		public DocumentBand Parent {
			get { return null; }
			set {}
		}
		[XtraSerializableProperty, DefaultValue(true) ]
		public bool IsVisible {
			get {
				return flags[bitIsVisible];
			}
			set {
				flags[bitIsVisible] = value;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual BrickCollectionBase Bricks { get { return EmptyBrickCollection.Instance; } }
		bool ITableCell.ShouldApplyPadding { get { return ShouldApplyPaddingInternal; } }
		string ITableCell.FormatString { get { return null; } }
		string ITableCell.XlsxFormatString { get { return null; } }
		object ITableCell.TextValue { get { return null; } }
		DefaultBoolean ITableCell.XlsExportNativeFormat { get { return DefaultBoolean.Default; } }
		string ITableCell.Url { get { return GetActualUrl(); } }
		protected internal virtual bool ShouldApplyPaddingInternal { get { return false; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("BrickBrickType"),
#endif
XtraSerializableProperty]
		public virtual string BrickType { get { return ExceptionHelper.ThrowInvalidOperationException<string>(); } }
		protected BrickAlignment AlignmentCore { get { return (BrickAlignment)flags[AlignmentSection]; } set { flags[AlignmentSection] = (int)value; } }
		protected BrickAlignment LineAlignmentCore { get { return (BrickAlignment)flags[LineAlignmentSection]; } set { flags[LineAlignmentSection] = (int)value; } }
		protected ImageAlignment ImageAlignmentCore { get { return (ImageAlignment)flags[ImageAlignmentSection]; } set { flags[ImageAlignmentSection] = (int)value; } }
		Separability Separability { get { return (Separability)flags[SeparabilitySection]; } set { flags[SeparabilitySection] = (int)value; } }
		internal bool IsSeparable { get { return Separability != Separability.None; } }
		bool CanBeSeparabled { get { return (Modifier & (BrickModifier.ReportHeader | BrickModifier.ReportFooter | BrickModifier.Detail | BrickModifier.DetailHeader | BrickModifier.DetailFooter)) > 0; } }
		internal bool CanOverflow { get { return flags[bitCanOverflow]; } set { flags[bitCanOverflow] = value; } }
		internal bool CanMultiColumn { get { return flags[bitCanMultiColumn]; } set { flags[bitCanMultiColumn] = value; } }
		internal bool CanAddToPage { get { return flags[bitCanAddToPage]; } set { flags[bitCanAddToPage] = value; } }
		public Brick()
			: base() {
			InitializeBrickProperties();
		}
		internal Brick(Brick brick) : base(brick) {
			AssignBrickData(brick.fBrickData);
			InitializeBrickProperties();
		}
		void AssignBrickData(AttachedPropertyValue[] source) {
			if(source == null || source.Length == 0)
				return;
			fBrickData = new AttachedPropertyValue[source.Length];
			Array.Copy(source, fBrickData, source.Length);
		}
		void InitializeBrickProperties() {
			IsVisible = true;
			AlignmentCore = BrickAlignment.None;
			LineAlignmentCore = BrickAlignment.None;
			CanAddToPage = true;
			CanMultiColumn = true;
		}
		public virtual void Dispose() {
			fBrickData = null;
		}
		internal void Offset(PointF pos) {
			base.Location = new PointF(base.X + pos.X, base.Y + pos.Y);
		}
		protected internal virtual void Scale(double scaleFactor) {
			base.Rect = MathMethods.Scale(base.Rect, scaleFactor);
		}
		internal virtual Brick GetRealBrick() {
			return this;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Initialize(PrintingSystemBase ps, RectangleF rect) {
			Initialize(ps, rect, true);
		}
		internal void Initialize(PrintingSystemBase ps, RectangleF rect, bool cacheStyle) {
			this.InitialRect = rect;
			PrintingSystem = ps;
			OnSetPrintingSystem(cacheStyle);
		}
		Hashtable IBrick.GetProperties() {
			Hashtable ht = new Hashtable();
			Accessor.GetProperties(this, ht);
			return ht;
		}
		void IBrick.SetProperties(Hashtable properties) {
			Accessor.SetProperties(this, properties);
		}
		void IBrick.SetProperties(object[,] properties) {
			Accessor.SetProperties(this, properties);
		}
		protected virtual void OnSetPrintingSystem(bool cacheStyle) {
		}
		public virtual float ValidatePageBottom(RectangleF pageBounds, bool enforceSplitNonSeparable, RectangleF rect, IPrintingSystemContext context) {
			if(SeparableVert || (pageBounds.Height < rect.Height && enforceSplitNonSeparable))
				return ValidatePageBottomCore(pageBounds, enforceSplitNonSeparable, rect, context);
			return rect.Top;
		}
		protected internal virtual float ValidatePageBottomCore(RectangleF pageBounds, bool enforceSplitNonSeparable, RectangleF rect, IPrintingSystemContext context) {
			return ValidatePageBottomInternal(pageBounds.Bottom, rect, context);
		}
		protected virtual float ValidatePageBottomInternal(float pageBottom, RectangleF rect, IPrintingSystemContext context) {
			return rect.Top;
		}
#if DEBUGTEST
		public float Test_ValidatePageBottomInternal(float pageBottom, RectangleF rect, IPrintingSystemContext context) {
			return ValidatePageBottomInternal(pageBottom, rect, context);
		}
#endif
		public virtual float ValidatePageRight(float pageRight, RectangleF rect) {
			return rect.Left;
		}
		protected internal override RectangleF GetViewRectangle() {
			return Rect;
		}
		protected internal virtual void OnAfterMerge() {
		}
		protected internal virtual void PerformLayout(IPrintingSystemContext context) {
			if(IsPageBrick)
				InitialRect = PageCustomBrickHelper.AlignRect(this, PageCustomBrickHelper.GetPageData(context.DrawingPage, context.PrintingSystem));
		}
		void SetSeparability(Separability flag, bool val) {
			Separability = val ? flag : Separability.None;
		}
		public virtual IEnumerator GetEnumerator() {
			return this.Bricks.GetEnumerator();
		}
		protected override bool ShouldSerializeCore(string propertyName) {
			if(propertyName == ValuePropertyName) {
				if(Value is string || (Value != null && Value.GetType().IsValueType))
					return base.ShouldSerializeCore(propertyName);
				return false;
			}
			return base.ShouldSerializeCore(propertyName);
		}
	}
}
