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
using System.Data;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils.Design;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGrid;
using DevExpress.Data.Filtering.Helpers;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
namespace DevExpress.XtraGrid {
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(DevExpress.Utils.ResFinder), DXDisplayNameAttribute.DefaultResourceFile)
	]
	public enum FormatConditionEnum { None, Equal, NotEqual, Between, NotBetween, Less, Greater, 
		GreaterOrEqual, LessOrEqual, Expression };
}
namespace DevExpress.XtraEditors {
	[ListBindable(false)]
	public abstract class FormatConditionCollectionBase : CollectionBase {
		int lockUpdate;
		public event CollectionChangeEventHandler CollectionChanged;
		public FormatConditionCollectionBase() {
			this.lockUpdate = 0;
		}
		protected virtual void ResetEvaluators() {
			foreach(StyleFormatConditionBase item in this) {
				item.ResetEvaluator();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public abstract bool IsLoading { get ; }
		public abstract int CompareValues(object val1, object val2);
		protected virtual object CreateItem() { return new StyleFormatConditionBase(); }
		protected virtual void AddRange(StyleFormatConditionBase[] conditions) {
			BeginUpdate();
			try {
				foreach(StyleFormatConditionBase item in conditions) {
					Add(item);
				}
			}
			finally {
				EndUpdate();
			}
		}
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("FormatConditionCollectionBaseItem")]
#endif
		public StyleFormatConditionBase this[int index] { get { return List[index] as StyleFormatConditionBase; } }
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("FormatConditionCollectionBaseItem")]
#endif
		public virtual StyleFormatConditionBase this[object tag] {
			get {
				foreach(StyleFormatConditionBase item in this) {
					if(item.Tag == tag) return item;
				}
				return null;
			}
		}
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("FormatConditionCollectionBaseItem")]
#endif
		public virtual StyleFormatConditionBase this[string name] {
			get {
				foreach(StyleFormatConditionBase item in this) {
					if(item.Name == name) return item;
				}
				return null;
			}
		}
		protected virtual void AssignItem(object item, object source) {
			(item as StyleFormatConditionBase).Assign(source as StyleFormatConditionBase);
		}
		public virtual void Assign(FormatConditionCollectionBase source) {
			BeginUpdate();
			try {
				Clear();
				foreach(object obj in source) {
					object item = CreateItem();
					Add(item as StyleFormatConditionBase);
					AssignItem(item, obj);
				}
			}
			finally {
				EndUpdate();
			}
		}
		public virtual void Add(StyleFormatConditionBase condition) {
			if(Contains(condition)) return;
			List.Add(condition);
		}
		protected bool IsLockUpdate { get { return lockUpdate != 0; } }
		protected override void OnClear() {
			InnerList.Clear();
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
		}
		public virtual void BeginUpdate() {
			lockUpdate++;
		}
		public virtual void EndUpdate() {
			if(--lockUpdate == 0) {
				OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
			}
		}
		public virtual void Remove(StyleFormatConditionBase condition) {
			if(!Contains(condition)) return;
			List.Remove(condition);
		}
		public virtual bool Contains(StyleFormatConditionBase condition) { return List.Contains(condition);	}
		public virtual int IndexOf(StyleFormatConditionBase condition) { return List.IndexOf(condition);	}
		protected override void OnRemoveComplete(int index, object item) {
			base.OnRemoveComplete(index, item);
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, item));
		}
		protected override void OnInsertComplete(int index, object item) {
			base.OnInsertComplete(index, item);
			(item as StyleFormatConditionBase).fCollection = this;
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, item));
		}
		protected internal virtual void OnCollectionChanged(CollectionChangeEventArgs e) {
			if(lockUpdate != 0) return;
			if(CollectionChanged != null)
				CollectionChanged(this, e);
		}
	}
	[TypeConverter(typeof(DevExpress.Utils.Design.UniversalTypeConverterEx))]
	public class StyleFormatConditionBase : IAppearanceOwner, IDataColumnInfo {
		object tag = null, value1 = null, value2 = null;
		FormatConditionEnum condition = FormatConditionEnum.None;
		object column = null;
		bool loaded = false;
		bool enabled = true;
		string expression = string.Empty;
		AppearanceObjectEx appearance;
		AppearanceObjectEx appearanceDescription;
		protected internal FormatConditionCollectionBase fCollection;
		string name = "";
		public StyleFormatConditionBase() : this(FormatConditionEnum.None, null, (AppearanceObject)null, null, null, null) {}
		public StyleFormatConditionBase(FormatConditionEnum condition, object tag, AppearanceDefault appearanceDefault, object val1, object val2,
			object column) : this(condition, tag, (AppearanceObject)null, val1, val2, column) {
			if(appearanceDefault != null) {
				this.appearance.BeginUpdate();
				this.appearance.Assign(appearanceDefault);
				this.appearance.CancelUpdate();
				this.appearanceDescription.BeginUpdate();
				this.appearanceDescription.Assign(appearanceDefault);
				this.appearanceDescription.CancelUpdate();
		}
		}
		public StyleFormatConditionBase(FormatConditionEnum condition, object tag, AppearanceObject appearanceDefault, object val1, object val2,
			object column) {
			fCollection = null;
			this.tag = tag;
			this.condition = condition;
			this.value1 = val1;
			this.value2 = val2;
			this.column = column;
			this.appearance = new AppearanceObjectEx(this);
			this.appearanceDescription = new AppearanceObjectEx(this);
			this.appearance.Changed += new EventHandler(OnAppearanceChanged);
			this.appearanceDescription.Changed += new EventHandler(OnAppearanceChanged);
			if(appearanceDefault != null) {
				this.appearance.BeginUpdate();
				this.appearance.Assign(appearanceDefault);
				this.appearance.CancelUpdate();
				this.appearanceDescription.BeginUpdate();
				this.appearanceDescription.Assign(appearanceDefault);
				this.appearanceDescription.CancelUpdate();
			}
		}
		[DefaultValue(true), XtraSerializableProperty]
		public bool Enabled {
			get { return enabled; }
			set {
				if(Enabled == value) return;
				enabled = value;
				ItemChanged();
			}
		}
		[DefaultValue(""), XtraSerializableProperty]
		public string Name {
			get { return name; }
			set {
				if(value == null) value = string.Empty;
				name = value; 
			}
		}
		protected void SetLoaded(bool loaded) {
			this.loaded = loaded;
		}
		bool IAppearanceOwner.IsLoading { get { return !loaded && (Collection == null || Collection.IsLoading); } }
		void OnAppearanceChanged(object sender, EventArgs e) {
			ItemChanged();
		}
		[Browsable(false)]
		public FormatConditionCollectionBase Collection {
			get { return fCollection; } 
		}
		bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		bool ShouldSerializeAppearanceDescription() { return AppearanceDescription.ShouldSerialize(); }
		void ResetAppearance() { 
			Appearance.Reset();
			AppearanceDescription.Reset();
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("StyleFormatConditionBaseAppearance"),
#endif
		DXDisplayName(typeof(DevExpress.Utils.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.StyleFormatConditionBase.Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)
		]
		public AppearanceObjectEx Appearance {
			get { return appearance; }
		}
		public AppearanceObjectEx AppearanceDescription
		{
			get { return appearanceDescription; }
		}
		protected object Column {
			get { return column; }
			set {
				if(Column == value) return;
				column = value;
				ItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("StyleFormatConditionBaseCondition"),
#endif
		DXDisplayName(typeof(DevExpress.Utils.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.StyleFormatConditionBase.Condition"),
		DefaultValue(FormatConditionEnum.None),
		XtraSerializableProperty()
		]
		public FormatConditionEnum Condition {
			get { return condition; }
			set {
				if(value == Condition) return;
				condition = value;
				ItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("StyleFormatConditionBaseValue1"),
#endif
		DXDisplayName(typeof(DevExpress.Utils.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.StyleFormatConditionBase.Value1"),
		XtraSerializableProperty(),
		DefaultValue(null), 
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))
		]
		public object Value1 {
			get { return value1; }
			set { 
				if(Value1 == value) return;
				value1 = value;
				ItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("StyleFormatConditionBaseValue2"),
#endif
		DXDisplayName(typeof(DevExpress.Utils.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.StyleFormatConditionBase.Value2"),
		XtraSerializableProperty(), DefaultValue(null), 
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))
		]
		public object Value2 {
			get { return value2; }
			set { 
				if(Value2 == value) return;
				value2 = value;
				ItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("StyleFormatConditionBaseTag"),
#endif
		DXDisplayName(typeof(DevExpress.Utils.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.StyleFormatConditionBase.Tag"),
		DefaultValue(null),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))
		]
		public virtual object Tag {
			get { return tag; }
			set {
				if(Tag == value) return;
				tag = value;
				ItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("StyleFormatConditionBaseExpression"),
#endif
		DXDisplayName(typeof(DevExpress.Utils.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.StyleFormatConditionBase.Expression"),
		DefaultValue(""),
		Editor(typeof(DevExpress.XtraEditors.Design.ExpressionEditorBase), typeof(System.Drawing.Design.UITypeEditor)), XtraSerializableProperty()
		]
		public string Expression {
			get { return expression; }
			set {
				if(value == null) value = string.Empty;
				if(Expression == value) return;
				this.expression = value;
				this.evaluatorCreated = false;
				ItemChanged();
			}
		}
		[Browsable(false)]
		public virtual bool IsValid {
			get { return Condition != FormatConditionEnum.None;}
		}
		protected object CheckNullValue(object val) {
			if(val == DBNull.Value) return null;
			return val;
		}
		[Obsolete()]
		public bool CheckValue(object column, object val) {
			return CheckValue(column, val, DataController.InvalidRow);
		}
		public virtual bool CheckValue(object column, object val, object row) {
			if(!Enabled) return false;
			if(!IsValid || (Column != null && column != Column)) return false;
			if(BaseEdit.IsNotLoadedValue(val)) return false;
			if(condition == FormatConditionEnum.Expression) return IsFit(val, row);
			val = CheckNullValue(val);
			object value1 = Value1, value2 = Value2;
			try {
				if(val != null) {
					Type valType = val.GetType();
					if(value1 != null && !valType.Equals(value1.GetType())) {
						value1 = Convert.ChangeType(value1, valType);
					}
				}
				int res1 = Collection.CompareValues(val, value1);
				switch(condition) {
					case FormatConditionEnum.Equal :	  return res1 == 0;
					case FormatConditionEnum.NotEqual :  return res1 != 0;
					case FormatConditionEnum.Less :   return res1 < 0;
					case FormatConditionEnum.Greater : return res1 > 0;
					case FormatConditionEnum.GreaterOrEqual : return res1 >= 0;
					case FormatConditionEnum.LessOrEqual : return res1 <= 0;
					case FormatConditionEnum.Between :	  
					case FormatConditionEnum.NotBetween :
						if(val != null) {
							value2 = Convert.ChangeType(value2, val.GetType());
						}
						int res2 = Comparer.Default.Compare(val, value2);
						if(condition == FormatConditionEnum.Between)
							return res1 > 0 && res2 < 0;
						return res1 <= 0 || res2 >= 0;
				}
			}
			catch {
				return false;
			}
			return true;
		}
		bool evaluatorCreated = false;
		ExpressionEvaluator evaluator;
		bool IsFit(object val, object row) {
			if(!evaluatorCreated) {
				this.evaluatorCreated = true;
				this.evaluator = CreateEvaluator();
			}
			if(evaluator == null) return false;
			return IsFitCore(evaluator, val, row);
		}
		protected virtual bool IsFitCore(ExpressionEvaluator evaluator, object val, object row) {
			return false;
		}
		protected virtual DataControllerBase Controller { get { return null; } }
		protected virtual ExpressionEvaluator CreateEvaluator() {
			if(Controller != null) {
				Exception e;
				return Controller.CreateExpressionEvaluator(CriteriaOperator.TryParse(Expression), true, out e);
			}
			return null;
		}
		internal void ResetEvaluator() {
			this.evaluator = null;
			this.evaluatorCreated = false;
		}
		public virtual void Assign(StyleFormatConditionBase source) {
			this.condition = source.Condition;
			this.value1 = source.Value1;
			this.value2 = source.Value2;
			this.enabled = source.Enabled;
			this.name = source.Name;
			AssignColumn(source.Column);
			this.tag = source.tag;
			this.expression = source.Expression;
			this.evaluatorCreated = false;
			this.Appearance.AssignInternal(source.Appearance);
			this.AppearanceDescription.AssignInternal(source.AppearanceDescription);
			ItemChanged();
		}
		public override string ToString() {
			if(!string.IsNullOrEmpty(Name)) return Name;
			return Tag == null ? string.Format("'{0}'", Condition.ToString()) : Tag.ToString();
		}
		protected virtual void AssignColumn(object column) {
			this.column = column;
		}
		protected virtual void ItemChanged() {
			if(Collection != null)
				Collection.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, this));
		}
		#region IDataColumnInfo Members
		string IDataColumnInfo.Caption {
			get {
				return ToString();
			}
		}
		DataControllerBase IDataColumnInfo.Controller { get { return Controller; } }
		List<IDataColumnInfo> IDataColumnInfo.Columns { get { return GetColumns(); } }
		protected virtual List<IDataColumnInfo> GetColumns() {
			return new List<IDataColumnInfo>();
		}
		string IDataColumnInfo.FieldName { get { return string.Empty; } }
		Type IDataColumnInfo.FieldType { get { return typeof(object); } }
		string IDataColumnInfo.Name { get { return string.Empty; } }
		string IDataColumnInfo.UnboundExpression { get { return Expression; } }
		#endregion
	}
}
