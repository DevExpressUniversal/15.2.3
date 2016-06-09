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
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils.Design;
using System.Collections.Generic;
namespace DevExpress.XtraScheduler {
	[TypeConverter(typeof(UniversalTypeConverterEx))]
	public class SchedulerColorSchemaBase : ICloneable, IBatchUpdateable, IBatchUpdateHandler, ISupportObjectChanged {
		int defaultCell = 0;
		int defaultCellBorder = 0;
		int defaultCellBorderDark = 0;
		int defaultCellLight = 0;
		int defaultCellLightBorder = 0;
		int defaultCellLightBorderDark = 0;
		int baseColorValue = 0;
		int cellColorValue = 0;
		int cellBorderColorValue = 0;
		int cellBorderDarkColorValue = 0;
		int cellLightColorValue = 0;
		int cellLightBorderColorValue = 0;
		int cellLightBorderDarkColorValue = 0;
		BatchUpdateHelper batchUpdateHelper;
		bool deferredRaiseChanged;
		EventHandler onChanged;
		public SchedulerColorSchemaBase() {
			Initialize();
		}
		public SchedulerColorSchemaBase(int baseColorValue) {
			Initialize();
			SetBaseColor(baseColorValue, new ColorSchemaTransformDefault());
		}
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsUpdateLocked { get { return batchUpdateHelper.IsUpdateLocked; } }
		protected internal int DefaultCell { get { return defaultCell; } set { defaultCell = value; } }
		protected internal int DefaultCellBorder { get { return defaultCellBorder; } set { defaultCellBorder = value; } }
		protected internal int DefaultCellBorderDark { get { return defaultCellBorderDark; } set { defaultCellBorderDark = value; } }
		protected internal int DefaultCellLight { get { return defaultCellLight; } set { defaultCellLight = value; } }
		protected internal int DefaultCellLightBorder { get { return defaultCellLightBorder; } set { defaultCellLightBorder = value; } }
		protected internal int DefaultCellLightBorderDark { get { return defaultCellLightBorderDark; } set { defaultCellLightBorderDark = value; } }
		protected internal int BaseColorValue { get { return baseColorValue; } }
		protected internal int CellColorValue {
			get { return cellColorValue; }
			set {
				if (cellColorValue == value)
					return;
				cellColorValue = value;
				OnChanged();
			}
		}
		protected internal int CellBorderColorValue {
			get { return cellBorderColorValue; }
			set {
				if (cellBorderColorValue == value)
					return;
				cellBorderColorValue = value;
				OnChanged();
			}
		}
		protected internal int CellBorderDarkColorValue {
			get { return cellBorderDarkColorValue; }
			set {
				if (cellBorderDarkColorValue == value)
					return;
				cellBorderDarkColorValue = value;
				OnChanged();
			}
		}
		protected internal int CellLightColorValue {
			get { return cellLightColorValue; }
			set {
				if (cellLightColorValue == value)
					return;
				cellLightColorValue = value;
				OnChanged();
			}
		}
		protected internal int CellLightBorderColorValue {
			get { return cellLightBorderColorValue; }
			set {
				if (cellLightBorderColorValue == value)
					return;
				cellLightBorderColorValue = value;
				OnChanged();
			}
		}
		protected internal int CellLightBorderDarkColorValue {
			get { return cellLightBorderDarkColorValue; }
			set {
				if (cellLightBorderDarkColorValue == value)
					return;
				cellLightBorderDarkColorValue = value;
				OnChanged();
			}
		}
		private void Initialize() {
			batchUpdateHelper = new BatchUpdateHelper(this);
		}
		protected internal void SetBaseColor(int colorValue, ColorSchemaTransformBase transform) {
			this.baseColorValue = colorValue;
			ResetAll(transform);
		}
		protected internal virtual void ResetAll(ColorSchemaTransformBase transform) {
			BeginUpdate();
			try {
				ResetCell(transform);
				ResetCellBorder(transform);
				ResetCellBorderDark(transform);
				ResetCellLight(transform);
				ResetCellLightBorder(transform);
				ResetCellLightBorderDark(transform);
			} finally {
				EndUpdate();
			}
		}
		internal void ResetCell(ColorSchemaTransformBase transform) {
			CellColorValue = (baseColorValue != 0) ? TransformColorValue(baseColorValue, transform.Cell) : defaultCell;
		}
		internal void ResetCellBorder(ColorSchemaTransformBase transform) {
			CellBorderColorValue = (baseColorValue != 0) ? TransformColorValue(baseColorValue, transform.CellBorder) : defaultCellBorder;
		}
		internal void ResetCellBorderDark(ColorSchemaTransformBase transform) {
			CellBorderDarkColorValue = (baseColorValue != 0) ? TransformColorValue(baseColorValue, transform.CellBorderDark) : defaultCellBorderDark;
		}
		internal void ResetCellLight(ColorSchemaTransformBase transform) {
			cellLightColorValue = (baseColorValue != 0) ? TransformColorValue(baseColorValue, transform.CellLight) : defaultCellLight;
		}
		internal void ResetCellLightBorder(ColorSchemaTransformBase transform) {
			cellLightBorderColorValue = (baseColorValue != 0) ? TransformColorValue(baseColorValue, transform.CellLightBorder) : defaultCellLightBorder;
		}
		internal void ResetCellLightBorderDark(ColorSchemaTransformBase transform) {
			cellLightBorderDarkColorValue = (baseColorValue != 0) ? TransformColorValue(baseColorValue, transform.CellLightBorderDark) : defaultCellLightBorderDark;
		}
		protected virtual SchedulerColorSchemaBase CreateSchemaInstance() {
			return new SchedulerColorSchemaBase();
		}
		protected virtual SchedulerColorSchemaBase CloneCore() {
			SchedulerColorSchemaBase clone = CreateSchemaInstance();
			clone.BeginUpdate();
			try {
				clone.defaultCell = defaultCell;
				clone.defaultCellBorder = defaultCellBorder;
				clone.defaultCellBorderDark = defaultCellBorderDark;
				clone.defaultCellLight = defaultCellLight;
				clone.defaultCellLightBorder = defaultCellLightBorder;
				clone.defaultCellLightBorderDark = defaultCellLightBorderDark;
				clone.baseColorValue = baseColorValue;
				clone.cellColorValue = cellColorValue;
				clone.cellBorderColorValue = cellBorderColorValue;
				clone.cellBorderDarkColorValue = cellBorderDarkColorValue;
				clone.cellLightColorValue = cellLightColorValue;
				clone.cellLightBorderColorValue = cellLightBorderColorValue;
				clone.cellLightBorderDarkColorValue = cellLightBorderDarkColorValue;
			} finally {
				clone.EndUpdate();
			}
			return clone;
		}
		public object Clone() {
			return CloneCore();
		}
		public void BeginUpdate() {
			batchUpdateHelper.BeginUpdate();
		}
		public void EndUpdate() {
			batchUpdateHelper.EndUpdate();
		}
		public void CancelUpdate() {
			batchUpdateHelper.CancelUpdate();
		}
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
			deferredRaiseChanged = false;
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			if (deferredRaiseChanged)
				RaiseChanged();
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
		}
		private int TransformColorValue(int baseColorValue, float light) {
			Color baseColor = Color.FromArgb(baseColorValue);
			return Color.FromArgb((int)(baseColor.R * light), (int)(baseColor.G * light), (int)(baseColor.B * light)).ToArgb();
		}
		protected virtual void OnChanged() {
			if (IsUpdateLocked)
				deferredRaiseChanged = true;
			else
				RaiseChanged();
		}
		protected virtual void RaiseChanged() {
			if (onChanged != null)
				onChanged(this, EventArgs.Empty);
		}
		public override bool Equals(object obj) {
			SchedulerColorSchemaBase schema = obj as SchedulerColorSchemaBase;
			if (schema != null) {
				return schema.baseColorValue == baseColorValue && schema.cellColorValue == cellColorValue &&
					schema.cellBorderColorValue == cellBorderColorValue && schema.cellBorderDarkColorValue == cellBorderDarkColorValue &&
					schema.cellLightColorValue == cellLightColorValue && schema.cellLightBorderColorValue == cellLightBorderColorValue &&
					schema.cellLightBorderDarkColorValue == cellLightBorderDarkColorValue;
			}
			return false;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override string ToString() {
			return "ColorSchemaBase";
		}
		public event EventHandler Changed {
			add { onChanged += value; }
			remove { onChanged -= value; }
		}
	}
	public interface ISchedulerColorSchemaCollection<out T> : IEnumerable<T>, IAssignableCollection where T : SchedulerColorSchemaBase {
		T this[int index] { get; }
		int Count { get; }
		int Add(object value);
		void RemoveAt(int index);
		T GetSchema(int index);
		T GetSchema(int colorValue, int index);
		void LoadDefaults();
		bool HasDefaultContent();
	}
	[TypeConverter(typeof(UniversalCollectionTypeConverter))]
	[ListBindable(BindableSupport.No), AutoFormatUrlPropertyClass]
	public class SchedulerColorSchemaCollectionBase<T> : NotificationCollection<T>, ISchedulerColorSchemaCollection<T> where T : SchedulerColorSchemaBase, new() {
		T default1;
		T default2;
		T default3;
		T default4;
		T default5;
		T default6;
		T default7;
		T default8;
		T default9;
		T default10;
		T default11;
		T default12;
		public SchedulerColorSchemaCollectionBase() {
			InitSchemas();
		}
		public T DefaultSchema1 { get { return (T)default1.Clone(); } }
		public T DefaultSchema2 { get { return (T)default2.Clone(); } }
		public T DefaultSchema3 { get { return (T)default3.Clone(); } }
		public T DefaultSchema4 { get { return (T)default4.Clone(); } }
		public T DefaultSchema5 { get { return (T)default5.Clone(); } }
		public T DefaultSchema6 { get { return (T)default6.Clone(); } }
		public T DefaultSchema7 { get { return (T)default7.Clone(); } }
		public T DefaultSchema8 { get { return (T)default8.Clone(); } }
		public T DefaultSchema9 { get { return (T)default9.Clone(); } }
		public T DefaultSchema10 { get { return (T)default10.Clone(); } }
		public T DefaultSchema11 { get { return (T)default11.Clone(); } }
		public T DefaultSchema12 { get { return (T)default12.Clone(); } }
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		protected virtual void CreateDefaultSchemas() {
			default1 = CreateSchemaInstance(new int[] { Color.FromArgb(0xFF, 0xF4, 0xBC).ToArgb(), Color.FromArgb(0xF3, 0xE4, 0xB1).ToArgb(), Color.FromArgb(0xEA, 0xD0, 0x98).ToArgb(), Color.FromArgb(0xFF, 0xFF, 0xD5).ToArgb(), Color.FromArgb(0xFF, 0xEF, 0xC7).ToArgb(), Color.FromArgb(0xF6, 0xDB, 0xA2).ToArgb() });
			default2 = CreateSchemaInstance(new int[] { SystemColors.Control.ToArgb(), SystemColors.ControlDark.ToArgb(), SystemColors.ControlDark.ToArgb(), SystemColors.Window.ToArgb(), SystemColors.ControlDark.ToArgb(), SystemColors.ControlDark.ToArgb() });
			default3 = CreateSchemaInstance(new int[] { Color.FromArgb(0xB3, 0xD4, 0x97).ToArgb(), Color.FromArgb(0xA8, 0xCB, 0x8A).ToArgb(), Color.FromArgb(0x8C, 0xB4, 0x68).ToArgb(), Color.FromArgb(0xD5, 0xEC, 0xBC).ToArgb(), Color.FromArgb(0xCD, 0xE4, 0xB4).ToArgb(), Color.FromArgb(0xBA, 0xD1, 0xA2).ToArgb() });
			default4 = CreateSchemaInstance(new int[] { Color.FromArgb(0x8B, 0x9E, 0xBF).ToArgb(), Color.FromArgb(0x80, 0x93, 0xB5).ToArgb(), Color.FromArgb(0x61, 0x74, 0x98).ToArgb(), Color.FromArgb(0xCF, 0xD8, 0xE6).ToArgb(), Color.FromArgb(0xC1, 0xC9, 0xDB).ToArgb(), Color.FromArgb(0xA1, 0xAF, 0xCC).ToArgb() });
			default5 = CreateSchemaInstance(new int[] { Color.FromArgb(0xBE, 0x86, 0xA1).ToArgb(), Color.FromArgb(0xB4, 0x7C, 0x95).ToArgb(), Color.FromArgb(0x9C, 0x65, 0x7A).ToArgb(), Color.FromArgb(0xE3, 0xCB, 0xD6).ToArgb(), Color.FromArgb(0xDA, 0xBD, 0xC7).ToArgb(), Color.FromArgb(0xC5, 0xA3, 0xAB).ToArgb() });
			default6 = CreateSchemaInstance(new int[] { Color.FromArgb(0x89, 0xB1, 0xA7).ToArgb(), Color.FromArgb(0x7B, 0xA8, 0x9C).ToArgb(), Color.FromArgb(0x54, 0x8E, 0x80).ToArgb(), Color.FromArgb(0xC1, 0xD6, 0xD1).ToArgb(), Color.FromArgb(0xAE, 0xCA, 0xC3).ToArgb(), Color.FromArgb(0x91, 0xB6, 0xAD).ToArgb() });
			default7 = CreateSchemaInstance(new int[] { Color.FromArgb(0xF7, 0xB4, 0x7F).ToArgb(), Color.FromArgb(0xEB, 0xA7, 0x71).ToArgb(), Color.FromArgb(0xCA, 0x83, 0x47).ToArgb(), Color.FromArgb(0xFA, 0xD0, 0xAE).ToArgb(), Color.FromArgb(0xEE, 0xC4, 0xA3).ToArgb(), Color.FromArgb(0xE1, 0xA6, 0x76).ToArgb() });
			default8 = CreateSchemaInstance(new int[] { Color.FromArgb(0xDD, 0x8C, 0x8E).ToArgb(), Color.FromArgb(0xD2, 0x81, 0x83).ToArgb(), Color.FromArgb(0xB3, 0x64, 0x65).ToArgb(), Color.FromArgb(0xEF, 0xC8, 0xC9).ToArgb(), Color.FromArgb(0xE9, 0xBB, 0xBD).ToArgb(), Color.FromArgb(0xDE, 0xA4, 0xA6).ToArgb() });
			default9 = CreateSchemaInstance(new int[] { Color.FromArgb(0x89, 0x96, 0x84).ToArgb(), Color.FromArgb(0x81, 0x8A, 0x7A).ToArgb(), Color.FromArgb(0x66, 0x64, 0x59).ToArgb(), Color.FromArgb(0xD0, 0xD8, 0xCB).ToArgb(), Color.FromArgb(0xC4, 0xCF, 0xBF).ToArgb(), Color.FromArgb(0xAC, 0xB5, 0xA9).ToArgb() });
			default10 = CreateSchemaInstance(new int[] { Color.FromArgb(0x00, 0xC7, 0xC8).ToArgb(), Color.FromArgb(0x00, 0xBA, 0xBB).ToArgb(), Color.FromArgb(0x00, 0x97, 0x99).ToArgb(), Color.FromArgb(0xA8, 0xEC, 0xEC).ToArgb(), Color.FromArgb(0x90, 0xE2, 0xE3).ToArgb(), Color.FromArgb(0x54, 0xCB, 0xCC).ToArgb() });
			default11 = CreateSchemaInstance(new int[] { Color.FromArgb(0xA8, 0x94, 0xCF).ToArgb(), Color.FromArgb(0x9B, 0x88, 0xC2).ToArgb(), Color.FromArgb(0x76, 0x63, 0x9B).ToArgb(), Color.FromArgb(0xDD, 0xD5, 0xEC).ToArgb(), Color.FromArgb(0xD2, 0xC7, 0xE6).ToArgb(), Color.FromArgb(0xB9, 0xA9, 0xD8).ToArgb() });
			default12 = CreateSchemaInstance(new int[] { Color.FromArgb(0xCC, 0xCC, 0xCC).ToArgb(), Color.FromArgb(0xBD, 0xBD, 0xBD).ToArgb(), Color.FromArgb(0x79, 0x79, 0x79).ToArgb(), Color.FromArgb(0xE6, 0xE6, 0xE6).ToArgb(), Color.FromArgb(0xCC, 0xCC, 0xCC).ToArgb(), Color.FromArgb(0xB1, 0xB1, 0xB1).ToArgb() });
		}
		protected override T GetItem(int index) {
			return (index >= 0 && index < Count) ? base.GetItem(index) : DefaultSchema1;
		}
		protected virtual void InitSchemas() {
			LoadDefaults();
		}
		protected internal virtual T CreateSchemaInstance(int[] colorValues) {
			T schema = new T();
			schema.BeginUpdate();
			try {
				schema.DefaultCell = colorValues[0];
				schema.DefaultCellBorder = colorValues[1];
				schema.DefaultCellBorderDark = colorValues[2];
				schema.DefaultCellLight = colorValues[3];
				schema.DefaultCellLightBorder = colorValues[4];
				schema.DefaultCellLightBorderDark = colorValues[5];
				schema.CellColorValue = schema.DefaultCell;
				schema.CellBorderColorValue = schema.DefaultCellBorder;
				schema.CellBorderDarkColorValue = schema.DefaultCellBorderDark;
				schema.CellLightColorValue = schema.DefaultCellLight;
				schema.CellLightBorderColorValue = schema.DefaultCellLightBorder;
				schema.CellLightBorderDarkColorValue = schema.DefaultCellLightBorderDark;
			} finally {
				schema.EndUpdate();
			}
			return schema;
		}
		protected internal virtual T[] CreateDefaultContent() {
			CreateDefaultSchemas();
			return new T[] { DefaultSchema1, DefaultSchema2, DefaultSchema3, DefaultSchema4, DefaultSchema5, DefaultSchema6, DefaultSchema7, DefaultSchema8, DefaultSchema9, DefaultSchema10, DefaultSchema11, DefaultSchema12 };
		}
		public bool HasDefaultContent() {
			T[] defaultContent = CreateDefaultContent();
			if (defaultContent.Length == 0)
				return false;
			int count = Count;
			if (count != defaultContent.Length)
				return false;
			for (int i = 0; i < count; i++) {
				if (!GetItem(i).Equals(defaultContent[i]))
					return false;
			}
			return true;
		}
		public T GetSchema(int index) {
			return GetItem(index % Math.Max(1, Count));
		}
		T ISchedulerColorSchemaCollection<T>.GetSchema(int colorValue, int index) {
			return GetSchema(colorValue, index);
		}
		protected virtual T GetSchema(int colorValue, int index) {
			if (colorValue != 0) {
				T schema = new T();
				schema.SetBaseColor(colorValue, new ColorSchemaTransformDefault());
				return schema;
			}
			return (T)GetSchema(index).Clone();
		}
		public virtual void LoadDefaults() {
			BeginUpdate();
			try {
				Clear();
				AddRange(CreateDefaultContent());
			} finally {
				EndUpdate();
			}
		}
		public void Assign(IAssignableCollection sourceCollection) {
			SchedulerColorSchemaCollectionBase<T> source = sourceCollection as SchedulerColorSchemaCollectionBase<T>;
			if (source == null)
				return;
			this.Clear();
			int count = source.Count;
			for (int i = 0; i < count; i++)
				this.Add(source[i].Clone());
		}
		public virtual int Add(object value) {
			return base.Add((T)value);
		}
	}
}
namespace DevExpress.XtraScheduler.Native {
	public abstract class ColorSchemaTransformBase {
		public abstract float Cell { get; }
		public abstract float CellBorder { get; }
		public abstract float CellBorderDark { get; }
		public abstract float CellLight { get; }
		public abstract float CellLightBorder { get; }
		public abstract float CellLightBorderDark { get; }
	}
	public class ColorSchemaTransformDefault : ColorSchemaTransformBase {
		public override float Cell { get { return 0.9f; } }
		public override float CellBorder { get { return 0.8f; } }
		public override float CellBorderDark { get { return 0.65f; } }
		public override float CellLight { get { return 1.0f; } }
		public override float CellLightBorder { get { return 0.85f; } }
		public override float CellLightBorderDark { get { return 0.7f; } }
	}
	public class ColorSchemaTransformSkin : ColorSchemaTransformBase {
		private float cell = 0.97f;
		private float cellBorder = 0.92f;
		private float cellBorderDark = 0.88f;
		private float cellLight = 1.0f;
		private float cellLightBorder = 0.95f;
		private float cellLightBorderDark = 0.91f;
		public override float Cell { get { return cell; } }
		public override float CellBorder { get { return cellBorder; } }
		public override float CellBorderDark { get { return cellBorderDark; } }
		public override float CellLight { get { return cellLight; } }
		public override float CellLightBorder { get { return cellLightBorder; } }
		public override float CellLightBorderDark { get { return cellLightBorderDark; } }
		protected virtual float ConvertPercentToRatio(int percent) {
			if (percent < 0)
				return 0;
			return percent / 100f;
		}
		public void SetCellPercent(int percent) {
			this.cell = ConvertPercentToRatio(percent);
		}
		public void SetCellBorderPercent(int percent) {
			this.cellBorder = ConvertPercentToRatio(percent);
		}
		public void SetCellBorderDarkPercent(int percent) {
			this.cellBorderDark = ConvertPercentToRatio(percent);
		}
		public void SetCellLightPercent(int percent) {
			this.cellLight = ConvertPercentToRatio(percent);
		}
		public void SetCellLightBorderPercent(int percent) {
			this.cellLightBorder = ConvertPercentToRatio(percent);
		}
		public void SetCellLightBorderDarkPercent(int percent) {
			this.cellLightBorderDark = ConvertPercentToRatio(percent);
		}
	}
}
