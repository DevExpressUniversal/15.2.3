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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraLayout.Utils;
namespace DevExpress.XtraLayout {
	#region DefinitionBase
	public abstract class DefinitionBase {
		protected SizeType sizeTypeCore;
		protected internal LayoutGroup owner;
		protected internal double realSize;
		double percentCore = 0;
		internal double Percent {
			get { return sizeTypeCore == SizeType.Percent ? percentCore : 0; }
			set { percentCore = value; }
		}
	}
	public class RowDefinition :DefinitionBase {
		public RowDefinition() {}
		public RowDefinition(LayoutGroup owner) {
			this.owner = owner;
			this.SizeType = SizeType.AutoSize;
			owner.ShouldRecreateTableLayoutManager = true;
		}
		public RowDefinition(LayoutGroup owner, double size, SizeType sizingType) {
			this.SizeType = sizingType;
			this.owner = owner;
			Height = size;
			owner.ShouldRecreateTableLayoutManager = true;
		}
		[XtraSerializableProperty()]
		public double Height {
			get {
				if(SizeType == SizeType.Percent) return Percent;
				return base.realSize;
			}
			set {
				if(value > 0) {
					base.realSize = value;
					if(SizeType == SizeType.Percent) Percent = value;
				}
				if(owner != null)owner.UpdateTableLayoutCore();
			}
		}
		[XtraSerializableProperty()]
		public SizeType SizeType {
			get { return base.sizeTypeCore; }
			set {
				base.sizeTypeCore = value;
				if(value == System.Windows.Forms.SizeType.Percent && Percent == 0) Percent = realSize;
			}
		}
		internal void SetHeightWithoutCorrection(int height) {
			if(SizeType != SizeType.Absolute) base.realSize = height;
		}
	}
	public class ColumnDefinition :DefinitionBase {
		public ColumnDefinition() { }
		public ColumnDefinition(LayoutGroup owner) {
			this.owner = owner;
			SizeType = SizeType.AutoSize;
			owner.ShouldRecreateTableLayoutManager = true;
		}
		public ColumnDefinition(LayoutGroup owner, double size, SizeType sizingType) {
			this.SizeType = sizingType;
			this.owner = owner;
			Width = size;
			owner.ShouldRecreateTableLayoutManager = true;
		}
		[XtraSerializableProperty()]
		public double Width {
			get {
				if(SizeType == SizeType.Percent) return Percent;
				return base.realSize; 
			}
			set {
				if(value > 0) {
					base.realSize = value;
					if(SizeType == SizeType.Percent) Percent = value;
				}
				if(owner != null) owner.UpdateTableLayoutCore();
			}
		}
		[XtraSerializableProperty()]
		public SizeType SizeType {
			get { return base.sizeTypeCore; }
			set { base.sizeTypeCore = value; }
		}
		internal void SetWidthWithoutCorrection(int width) {
			if(SizeType != SizeType.Absolute) base.realSize = width;
		}
	}
	#endregion
	#region DefinitionBaseCollection
	[ListBindable(BindableSupport.No)]
	public abstract class DefinitionBaseCollection :CollectionBase, IEnumerable<DefinitionBase> {
		protected internal LayoutGroup ownerGroup;
		public DefinitionBaseCollection(LayoutGroup owner) {
			ownerGroup = owner;
		}
		protected internal DefinitionBaseCollection(DefinitionBase[] defenitionBaseArray, LayoutGroup ownerGroup)
			: this(ownerGroup) {
			AddRange(defenitionBaseArray);
		}
		public DefinitionBaseCollection(DefinitionBaseCollection source, LayoutGroup ownerGroup)
			: this(ownerGroup) {
			if(source != null)
				AddRange(System.Linq.Enumerable.ToArray(source));
		}
		public LayoutGroup Owner {
			get { return ownerGroup; }
		}
		protected DefinitionBase this[int index] {
			get { return InnerList[index] as DefinitionBase; }
		}
		public int IndexOf(DefinitionBase item) {
			return InnerList.IndexOf(item);
		}
		int updateCounter = 0;
		public void BeginUpdate() {
			updateCounter++;
		}
		public virtual void AddRange(DefinitionBase[] items) {
			if(Owner != null) Owner.BeginInit();
			for(int i = 0; i < items.Length; i++) {
				Add(items[i]);
			}
			if(Owner != null) Owner.EndInit();
		}
		public void EndUpdate() {
			updateCounter--;
			if(updateCounter == 0) Owner.UpdateTableLayoutCore();
		}
		public List<DefinitionBase> ConvertToTypedList() {
			return new List<DefinitionBase>(this);
		}
		public bool Contains(DefinitionBase item) {
			return InnerList.Contains(item);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public new void Clear() { base.Clear(); }
		protected virtual void Add(DefinitionBase definitionBase) {
			List.Add(definitionBase);
		}
		protected internal void Insert(int index, DefinitionBase definitionBase) { List.Insert(index, definitionBase); }
		protected virtual internal void Remove(DefinitionBase definitionBase) {
			List.Remove(definitionBase);
		}
		protected override void OnRemoveComplete(int position, object definitionBase) {
			if(updateCounter == 0) ownerGroup.UpdateTableLayoutCore();
		}
		protected override void OnInsertComplete(int position, object definitionBase) {
			if(updateCounter == 0) ownerGroup.UpdateTableLayoutCore();
		}
		protected virtual void OnChanged(CollectionChangeEventArgs e) {
			if(updateCounter == 0) ownerGroup.UpdateTableLayoutCore();
		}
		IEnumerator<DefinitionBase> IEnumerable<DefinitionBase>.GetEnumerator() {
			foreach(DefinitionBase definitionBase in InnerList)
				yield return definitionBase;
		}
		public void Reverse() {
			InnerList.Reverse();
		}
		public void Reverse(int index, int count) {
			InnerList.Reverse(index, count);
		}
	}
	[ListBindable(BindableSupport.No), Editor("DevExpress.XtraLayout.Design.RowDefinitionsEditor, " + AssemblyInfo.SRAssemblyLayoutControlDesign, typeof(System.Drawing.Design.UITypeEditor))]
	public class RowDefinitions :DefinitionBaseCollection, IEnumerable<RowDefinition> {
		public RowDefinitions(LayoutGroup owner) : base(owner) { }
		protected internal RowDefinitions(RowDefinition[] rowDefinitionArray, LayoutGroup ownerGroup) : base(rowDefinitionArray.Cast<DefinitionBase>().ToArray(), ownerGroup) { }
		public RowDefinitions(DefinitionBaseCollection source, LayoutGroup ownerGroup) : base(source, ownerGroup) { }
		public virtual void Add(RowDefinition rowDefinition) {
			if(rowDefinition.owner == null) rowDefinition.owner = Owner;
			base.Add(rowDefinition);
		}
		IEnumerator<RowDefinition> IEnumerable<RowDefinition>.GetEnumerator() {
			foreach(RowDefinition rowDefinition in InnerList)
				yield return rowDefinition;
		}
		public new virtual RowDefinition this[int index] {
			get { return InnerList[index] as RowDefinition; }
		}
		public void AddRange(RowDefinition[] items) {
			BeginUpdate();
			base.AddRange(items);
			EndUpdate();
		}
		internal void InternalAdd(RowDefinition rowDefinition) {
			InnerList.Add(rowDefinition);
			ownerGroup.ShouldRecreateTableLayoutManager = true;
		}
	}
	[ListBindable(BindableSupport.No), Editor("DevExpress.XtraLayout.Design.ColumnDefinitionsEditor, " + AssemblyInfo.SRAssemblyLayoutControlDesign, typeof(System.Drawing.Design.UITypeEditor))]
	public class ColumnDefinitions :DefinitionBaseCollection, IEnumerable<ColumnDefinition> {
		public ColumnDefinitions(LayoutGroup owner) : base(owner) { }
		protected internal ColumnDefinitions(ColumnDefinition[] columnDefenitionArray, LayoutGroup ownerGroup) : base(columnDefenitionArray.Cast<DefinitionBase>().ToArray(), ownerGroup) { }
		public ColumnDefinitions(ColumnDefinitions source, LayoutGroup ownerGroup) : base(source, ownerGroup) { }
		public virtual void Add(ColumnDefinition columnDefinition) {
			if(columnDefinition.owner == null) columnDefinition.owner = Owner;
			base.Add(columnDefinition);
		}
		public void AddRange(ColumnDefinition[] items) {
			BeginUpdate();
			base.AddRange(items);
			EndUpdate();
		}
		IEnumerator<ColumnDefinition> IEnumerable<ColumnDefinition>.GetEnumerator() {
			foreach(ColumnDefinition columnDefinition in InnerList)
				yield return columnDefinition;
		}
		public new virtual ColumnDefinition this[int index] {
			get { return InnerList[index] as ColumnDefinition; }
		}
		internal void InternalAdd(ColumnDefinition columnTableLayout) {
			InnerList.Add(columnTableLayout);
			ownerGroup.ShouldRecreateTableLayoutManager = true;
		}
	}
	#endregion
	public class OptionsTableLayoutGroup :BaseLayoutOptions {
		public OptionsTableLayoutGroup(ILayoutControl owner,LayoutGroup ownerGroup)
			: base(owner) {
				rowDefinitionsCore = new RowDefinitions(ownerGroup);
				columnDefinitionsCore = new ColumnDefinitions(ownerGroup);
		}
		readonly RowDefinitions rowDefinitionsCore;
		readonly ColumnDefinitions columnDefinitionsCore;
		internal LayoutGroup groupOwner { get { return rowDefinitionsCore.ownerGroup; } }
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsTableLayoutGroupRowCount"),
#endif
 Browsable(false)]
		public int RowCount {
			get {
				return rowDefinitionsCore.Count;
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("OptionsTableLayoutGroupColumnCount"),
#endif
 Browsable(false)]
		public int ColumnCount {
			get {
				return columnDefinitionsCore.Count;
			}
		}
		const int constDefaultLength = 20;
		int autoSizeDefaultDefinitionLengthCore = constDefaultLength;
		[DefaultValue(constDefaultLength)]
		public int AutoSizeDefaultDefinitionLength {
			get { return autoSizeDefaultDefinitionLengthCore; }
			set {
				if(value <= 0) return;
				autoSizeDefaultDefinitionLengthCore = value;
				if(groupOwner != null) groupOwner.UpdateTableLayoutCore();
			}
		}
		bool ShouldSerializeRowDefinitions() { return true; }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true)]
		public RowDefinitions RowDefinitions {
			get {
				return rowDefinitionsCore;
			}
		}
		bool ShouldSerializeColumnDefinitions() { return true; }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true)]
		public ColumnDefinitions ColumnDefinitions {
			get {
				return columnDefinitionsCore;
			}
		}
		internal object XtraCreateColumnDefinitionsItem(XtraItemEventArgs e) {
			XtraPropertyInfo infoType = e.Item.ChildProperties["Width"];
			int Width = Convert.ToInt32(infoType.Value);
			infoType = e.Item.ChildProperties["SizeType"];
			SizeType sizeType = GetSizeType(infoType);
			ColumnDefinition definition = new ColumnDefinition();
			if(sizeType == SizeType.Percent) {
				definition.Percent = Width;
			}
			(e.Collection as ColumnDefinitions).Add(definition);
			return definition;
		}
		internal object XtraCreateRowDefinitionsItem(XtraItemEventArgs e) {
			XtraPropertyInfo infoType = e.Item.ChildProperties["Height"];
			int Height = Convert.ToInt32(infoType.Value);
			infoType = e.Item.ChildProperties["SizeType"];
			SizeType sizeType = GetSizeType(infoType);
			RowDefinition definition = new RowDefinition();
			if(sizeType == SizeType.Percent) {
				definition.Percent = Height;
			}
			(e.Collection as RowDefinitions).Add(definition);
			return definition;
		}
		private SizeType GetSizeType(XtraPropertyInfo infoType) {
			string str = (string)infoType.Value;
			switch(str) {
				case "Percent":
					return SizeType.Percent;
				case "Absolute":
					return SizeType.Absolute;
				case "AutoSize" :
					return SizeType.AutoSize;
				default:
					return SizeType.AutoSize;
			}
		}
		internal int GetFreeHeightForPercent(Size ClientSize, LayoutGroupItemCollection Items) {
			int result = ClientSize.Height;
			for(int i = 0; i < RowDefinitions.Count; i++)
				if(RowDefinitions[i].SizeType != SizeType.Percent) {
					result -= Items[i].Height;
				}
			return result;
		}
		internal int GetFreeWidthForPercent(Size ClientSize, LayoutGroupItemCollection Items) {
			int result = ClientSize.Width;
			for(int i = 0; i < ColumnDefinitions.Count; i++)
				if(ColumnDefinitions[i].SizeType != SizeType.Percent) {
					result -= Items[i].Width;
				}
			return result;
		}
		public RowDefinition AddRow() {
		   RowDefinition row = new RowDefinition(groupOwner);
		   rowDefinitionsCore.Add(row);
		   return row;
		}
		public ColumnDefinition AddColumn() {
			ColumnDefinition column = new ColumnDefinition(groupOwner);
			columnDefinitionsCore.Add(column);
			return column;
		}
		public RowDefinition GetRow(int index) {
			return rowDefinitionsCore[index];
		}
		public ColumnDefinition GetColumn(int index) {
			return columnDefinitionsCore[index];
		}
		public void Add(RowDefinition rowDefenition) {
			rowDefinitionsCore.Add(rowDefenition);
		}
		public void Add(ColumnDefinition columnDefinition) {
			columnDefinitionsCore.Add(columnDefinition);
		}
		public void Remove(RowDefinition rowDefenition) {
			rowDefinitionsCore.Remove(rowDefenition);
		}
		public void Remove(ColumnDefinition columnDefinition) {
			columnDefinitionsCore.Remove(columnDefinition);
		}
	}
#if DEBUGTEST
	[System.Diagnostics.DebuggerDisplay("Row: {rowIndex}, Column: {columnIndex}, RowSpan: {rowSpan}, ColumnSpan: {columnSpan}")]
#endif
	public class OptionsTableLayoutItem :BaseOptions {
		public OptionsTableLayoutItem(BaseLayoutItem ownerItem) { this.ownerItem = ownerItem; }
		readonly BaseLayoutItem ownerItem;
		BaseLayoutItem Owner { get { return ownerItem; } }
		LayoutGroup Parent { get { return Owner.Parent; } }
		ILayoutControl OwnerILayoutControl { get { return Owner.Owner; } }
		int rowIndex = 0, rowSpan = 1, columnIndex = 0, columnSpan = 1;
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true), DefaultValue(0)]
		[XtraSerializableProperty()]
		public virtual int RowIndex {
			get { return rowIndex; }
			set {
				if(Parent != null && !CanSetIndexAndSpan(Parent, value + rowSpan,LayoutType.Horizontal)) return;
				rowIndex = value;
				if(Parent != null && OwnerILayoutControl != null) {
					OwnerILayoutControl.ShouldResize = true;
					Parent.Update();
				}
			}
		}
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true), DefaultValue(1)]
		[XtraSerializableProperty()]
		public virtual int RowSpan {
			get { return rowSpan; }
			set {
				if(Parent != null && !CanSetIndexAndSpan(Parent, value + rowIndex, LayoutType.Horizontal)) return;
				if(value < 1 || value > 50) return;
				rowSpan = value;
				if(Parent != null && OwnerILayoutControl != null) {
					OwnerILayoutControl.ShouldResize = true;
					Parent.Update();
				}
			}
		}
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true), DefaultValue(0)]
		[XtraSerializableProperty()]
		public virtual int ColumnIndex {
			get { return columnIndex; }
			set {
				if(Parent != null && !CanSetIndexAndSpan(Parent, value + columnSpan, LayoutType.Vertical)) return;
				columnIndex = value;
				if(Parent != null && OwnerILayoutControl != null) {
					OwnerILayoutControl.ShouldResize = true;
					Parent.Update();
				}
			}
		}
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true), DefaultValue(1)]
		[XtraSerializableProperty()]
		public virtual int ColumnSpan {
			get { return columnSpan; }
			set {
				if(Parent != null && !CanSetIndexAndSpan(Parent, value + columnIndex, LayoutType.Vertical)) return;
				if(value < 1 || value > 50) return;
				columnSpan = value;
				if(Parent != null && OwnerILayoutControl != null) {
					OwnerILayoutControl.ShouldResize = true;
					Parent.Update();
				}
			}
		}
		internal bool CanSetIndexAndSpan(LayoutGroup Parent, int value, LayoutType lType) {
			if(Parent.LayoutMode != LayoutMode.Table) return false;
			if((lType == LayoutType.Horizontal ? Parent.OptionsTableLayoutGroup.RowCount : Parent.OptionsTableLayoutGroup.ColumnCount) < value) return false;
			return true;
		}																										
		internal void SetRowColumnIndex(int row, int column) {
			rowIndex = row;
			columnIndex = column;
		}
		internal Rectangle GetRectangleFromRowColumn() {
			return new Rectangle(columnIndex, rowIndex, columnSpan, rowSpan);
		}
		internal void SetRowColumnSpan(int rowSpan, int columnSpan) {
			this.rowSpan = rowSpan;
			this.columnSpan = columnSpan;
		}
	}
}
