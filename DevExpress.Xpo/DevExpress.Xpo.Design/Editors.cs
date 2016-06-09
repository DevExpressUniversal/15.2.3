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
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Globalization;
using DevExpress.Xpo.Metadata;
using System.CodeDom;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.Helpers;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTab;
using DevExpress.Xpo.Metadata.Helpers;
using System.Collections.Generic;
using DevExpress.Xpo.DB;
using System.Text;
namespace DevExpress.Xpo.Design {
	public class XPCollectionSerializer : CodeDomSerializer {
		void CheckType(IDesignerSerializationManager manager, CodeTypeOfExpression expr) {
			string str = expr.Type.BaseType;
			Type type = manager.GetType(str);
			if (type == null) {
				manager.ReportError(new System.Runtime.Serialization.SerializationException(string.Format("Could not find type '{0}'. Please make sure that the assembly that contains this type is referenced. If this type is a part of your development project, make sure that the project has been successfully built.", str)));
			}
		}
		public override object Deserialize(IDesignerSerializationManager manager, object codeDomObject) {
			CodeDomSerializer baseSerializer = (CodeDomSerializer)manager.GetSerializer(typeof(XPCollection).BaseType, typeof(CodeDomSerializer));
			object o = baseSerializer.Deserialize(manager, codeDomObject);
			if (codeDomObject is CodeStatementCollection) {
				if (o is XPCollection) {
					foreach (CodeStatement cs in (CodeStatementCollection)codeDomObject) {
						if (cs is CodeAssignStatement) {
							if (((CodeAssignStatement)cs).Left is CodePropertyReferenceExpression &&
								((CodePropertyReferenceExpression)((CodeAssignStatement)cs).Left).PropertyName == "ObjectClassInfo") {
								CheckType(manager, (CodeTypeOfExpression)((CodeMethodInvokeExpression)((CodeAssignStatement)cs).Right).Parameters[0]);
								((ISupportInitialize)o).BeginInit();
								try {
									((XPCollection)o).ObjectClassInfo = (XPClassInfo)DeserializeExpression(manager, null, ((CodeAssignStatement)cs).Right);
								} finally {
									((ISupportInitialize)o).EndInit();
								}
								break;
							}
							if (((CodeAssignStatement)cs).Left is CodePropertyReferenceExpression &&
								((CodePropertyReferenceExpression)((CodeAssignStatement)cs).Left).PropertyName == "ObjectType") {
								CheckType(manager, (CodeTypeOfExpression)((CodeAssignStatement)cs).Right);
							}
						}
					}
				}
			}
			return o;
		}
		public override object Serialize(IDesignerSerializationManager manager, object value) {
			CodeDomSerializer baseSerializer =
				(CodeDomSerializer)manager.GetSerializer(typeof(XPCollection).BaseType, typeof(CodeDomSerializer));
			return baseSerializer.Serialize(manager, value);
		}
	}
	public class XPViewSerializer : CodeDomSerializer {
		void CheckType(IDesignerSerializationManager manager, CodeTypeOfExpression expr) {
			string str = expr.Type.BaseType;
			Type type = manager.GetType(str);
			if (type == null) {
				manager.ReportError(new System.Runtime.Serialization.SerializationException(string.Format("Could not find type '{0}'. Please make sure that the assembly that contains this type is referenced. If this type is a part of your development project, make sure that the project has been successfully built.", str)));
			}
		}
		public override object Deserialize(IDesignerSerializationManager manager, object codeDomObject) {
			CodeDomSerializer baseSerializer = (CodeDomSerializer)manager.GetSerializer(typeof(XPView).BaseType, typeof(CodeDomSerializer));
			object o = baseSerializer.Deserialize(manager, codeDomObject);
			if (codeDomObject is CodeStatementCollection) {
				foreach (CodeStatement cs in (CodeStatementCollection)codeDomObject) {
					if (cs is CodeAssignStatement) {
						if (((CodeAssignStatement)cs).Left is CodePropertyReferenceExpression &&
							((CodePropertyReferenceExpression)((CodeAssignStatement)cs).Left).PropertyName == "ObjectClassInfo") {
							CheckType(manager, (CodeTypeOfExpression)((CodeMethodInvokeExpression)((CodeAssignStatement)cs).Right).Parameters[0]);
							((ISupportInitialize)o).BeginInit();
							try {
								((XPView)o).ObjectClassInfo = (XPClassInfo)DeserializeExpression(manager, null, ((CodeAssignStatement)cs).Right);
							} finally {
								((ISupportInitialize)o).EndInit();
							}
							break;
						}
						if (((CodeAssignStatement)cs).Left is CodePropertyReferenceExpression &&
							((CodePropertyReferenceExpression)((CodeAssignStatement)cs).Left).PropertyName == "ObjectType") {
							CheckType(manager, (CodeTypeOfExpression)((CodeAssignStatement)cs).Right);
						}
					}
				}
			}
			return o;
		}
		public override object Serialize(IDesignerSerializationManager manager, object value) {
			CodeDomSerializer baseSerializer =
				(CodeDomSerializer)manager.GetSerializer(typeof(XPView).BaseType, typeof(CodeDomSerializer));
			return baseSerializer.Serialize(manager, value);
		}
	}
	public abstract class DesignUtilities {
		public static DialogResult ReportError(IServiceProvider provider, string message) {
			IUIService service = (IUIService)provider.GetService(typeof(IUIService));
			if (service != null) {
				return MessageBox.Show(service.GetDialogOwnerWindow(), message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
			}
			return MessageBox.Show(null, message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
		}
	}
	public class SessionReferenceConverter : System.ComponentModel.ReferenceConverter {
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object val) {
			if (val is string) {
				if ((string)val == "(default)")
					return null;
			}
			return base.ConvertFrom(context, culture, val);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType == typeof(string)) {
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object val, Type destType) {
			if (destType == typeof(string)) {
				if (val == null || val is DevExpress.Xpo.Helpers.DefaultSession)
					return "(default)";
			}
			return base.ConvertTo(context, culture, val, destType);
		}
		public SessionReferenceConverter() : base(typeof(Session)) { }
	}
	abstract class CriteriaEditor : UITypeEditor {
		Rectangle location;
		class CriteriaEditorForm : XtraForm {
			FilterControl filter;
			MemoEdit memo;
			XtraTabControl tab;
			XtraTabPage simple;
			XtraTabPage advanced;
			public CriteriaEditorForm() {
				this.FormBorderStyle = FormBorderStyle.Sizable;
				this.ShowIcon = false;
				this.MinimizeBox = false;
				this.MinimumSize = new Size(200, 200);
				this.StartPosition = FormStartPosition.CenterParent;
				this.ShowInTaskbar = false;
				tab = new XtraTabControl();
				tab.Parent = this;
				tab.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
				tab.Size = this.ClientRectangle.Size;
				tab.Height -= 40;
				simple = tab.TabPages.Add("&Simple");
				filter = new FilterControl();
				filter.Parent = simple;
				filter.Dock = DockStyle.Fill;
				advanced = tab.TabPages.Add("&Advanced");
				memo = new MemoEdit();
				memo.Parent = advanced;
				memo.Dock = DockStyle.Fill;
				memo.Properties.Validating += new CancelEventHandler(Properties_Validating);
				tab.SelectedTabPage = advanced;
				((IXtraTab)tab).ViewInfo.SelectedPageChanging += new DevExpress.XtraTab.ViewInfo.ViewInfoTabPageChangingEventHandler(ViewInfo_SelectedPageChanging);
				tab.SelectedPageChanged += new TabPageChangedEventHandler(tab_SelectedPageChanged);
				SimpleButton cancel = new SimpleButton();
				cancel.Parent = this;
				cancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
				cancel.DialogResult = DialogResult.Cancel;
				cancel.Top = tab.Bottom + 5;
				cancel.Left = this.ClientSize.Width - 10 - cancel.Width;
				cancel.Text = "&Cancel";
				cancel.CausesValidation = false;
				this.CancelButton = cancel;
				SimpleButton ok = new SimpleButton();
				ok.Parent = this;
				ok.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
				ok.DialogResult = DialogResult.OK;
				ok.Top = tab.Bottom + 5;
				ok.Left = cancel.Left - 5 - ok.Width;
				ok.Text = "&Ok";
			}
			void Properties_Validating(object sender, CancelEventArgs e) {
				try {
					CriteriaOperator.Parse(memo.Text);
				} catch (Exception exc) {
					memo.ErrorText = exc.Message;
					e.Cancel = true;
				}
			}
			void ViewInfo_SelectedPageChanging(object sender, DevExpress.XtraTab.ViewInfo.ViewInfoTabPageChangingEventArgs e) {
				if (e.Page == simple) {
					try {
						if (memo.Text == String.Empty)
							return;
						if (!CriteriaToTreeProcessor.IsConvertibleOperator(CriteriaOperator.Parse(memo.Text)))
							e.Cancel = true;
					} catch (Exception exc) {
						memo.ErrorText = exc.Message;
						e.Cancel = true;
					}
				}
			}
			void tab_SelectedPageChanged(object sender, TabPageChangedEventArgs e) {
				if (e.Page == simple) {
					filter.FilterCriteria = CriteriaOperator.Parse(memo.Text);
				} else {
					memo.Text = ReferenceEquals(filter.FilterCriteria, null) ? String.Empty : filter.FilterCriteria.ToString();
				}
			}
			public CriteriaOperator Criteria {
				get {
					if (tab.SelectedTabPage == advanced)
						return CriteriaOperator.Parse(memo.Text);
					else
						return filter.FilterCriteria;
				}
				set {
					memo.Text = Object.ReferenceEquals(value, null) ? String.Empty : value.ToString();
					tab.SelectedTabPage = simple;
				}
			}
			public void SetColumns(FilterColumnCollection cols) {
				filter.SetFilterColumnsCollection(cols);
			}
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
		protected FilterColumn CreateColumn(PropertyDescriptor pd) {
			FilterColumnClauseClass columnClass;
			RepositoryItem editor = GetEditor(pd.PropertyType, out columnClass);
			return new UnboundFilterColumn(pd.DisplayName == null || pd.DisplayName == String.Empty ? pd.Name : pd.DisplayName, pd.Name, pd.PropertyType, editor, columnClass);
		}
		protected FilterColumn CreateColumn(XPMemberInfo mi) {
			FilterColumnClauseClass columnClass;
			RepositoryItem editor = GetEditor(mi.MemberType, out columnClass);
			return new UnboundFilterColumn(mi.DisplayName == null || mi.DisplayName == String.Empty ? mi.Name : mi.DisplayName, mi.Name, mi.MemberType, editor, columnClass);
		}
		protected static bool IsGoodProperty(XPMemberInfo mi) {
			MemberDesignTimeVisibilityAttribute visibility = (MemberDesignTimeVisibilityAttribute)mi.FindAttributeInfo(typeof(MemberDesignTimeVisibilityAttribute));
			if (visibility != null && visibility.IsVisible)
				return true;
			if (!mi.IsPublic)
				return false;
			if (typeof(IBindingList).IsAssignableFrom(mi.MemberType) && !mi.IsCollection)
				return false;
			if (mi.MemberType.IsArray)
				return false;
			if (mi.IsCollection)
				return false;
			if (!mi.IsVisibleInDesignTime)
				return false;
			if (mi is DevExpress.Xpo.Metadata.Helpers.ServiceField)
				return false;
			return true;
		}
		protected abstract FilterColumnCollection GetColumns(object instance);
		class EnumValues {
			Enum value;
			public EnumValues(Enum value) {
				this.value = value;
			}
			public long Value {
				get {
					return ((IConvertible)value).ToInt64(null);
				}
			}
			public string Name {
				get {
					return Enum.GetName(value.GetType(), value);
				}
			}
		}
		RepositoryItem GetEditor(Type type, out FilterColumnClauseClass columnClass) {
			if (type.IsEnum) {
				columnClass = FilterColumnClauseClass.Lookup;
				RepositoryItemLookUpEdit item = new RepositoryItemLookUpEdit();
				Array enumVals = Enum.GetValues(type);
				EnumValues[] vals = new EnumValues[enumVals.Length];
				for (int i = 0; i < enumVals.Length; i++)
					vals[i] = new EnumValues((Enum)Enum.GetValues(type).GetValue(i));
				item.DataSource = vals;
				item.DisplayMember = "Name";
				item.ValueMember = "Value";
				item.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
				return item;
			}
			columnClass = FilterColumnClauseClass.Generic;
			switch (Type.GetTypeCode(type)) {
				case TypeCode.Boolean:
					return new RepositoryItemCheckEdit();
				case TypeCode.UInt64:
				case TypeCode.UInt32:
				case TypeCode.UInt16:
				case TypeCode.Int64:
				case TypeCode.Int32:
				case TypeCode.Int16:
					return new RepositoryItemSpinEdit();
				case TypeCode.DateTime:
					columnClass = FilterColumnClauseClass.DateTime;
					return new RepositoryItemDateEdit();
				case TypeCode.String:
					columnClass = FilterColumnClauseClass.String;
					return new RepositoryItemTextEdit();
				case TypeCode.Object:
					columnClass = FilterColumnClauseClass.Blob;
					return new RepositoryItemTextEdit();
			}
			return new RepositoryItemTextEdit();
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if (context != null && provider != null && context.PropertyDescriptor != null) {
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null) {
					using (CriteriaEditorForm form = new CriteriaEditorForm()) {
						if (location != Rectangle.Empty) {
							form.StartPosition = FormStartPosition.Manual;
							form.Bounds = location;
						}
						form.SetColumns(GetColumns(context.Instance));
						form.Criteria = (CriteriaOperator)value;
						if (edSvc.ShowDialog(form) == DialogResult.OK && !CriteriaOperator.Equals(value, form.Criteria)) {
							context.OnComponentChanged();
							location = form.Bounds;
							return form.Criteria;
						}
						location = form.Bounds;
					}
				}
			}
			return value;
		}
	}
	class XPCollectionCriteriaEditor : CriteriaEditor {
		protected override FilterColumnCollection GetColumns(object instance) {
			FilterColumnCollection cols = new FilterColumnCollection();
			Hashtable names = new Hashtable();
			ITypedList list = instance as ITypedList;
			if (list == null) {
				IListSource ls = instance as IListSource;
				if (ls != null)
					list = ls.GetList() as ITypedList;
			}
			if (list != null) {
				foreach (PropertyDescriptor pd in list.GetItemProperties(null)) {
					if (!names.ContainsKey(pd.Name) && pd.PropertyType == typeof(XPCollection)) {
						cols.Add(CreateColumn(pd));
						names.Add(pd.Name, null);
					}
				}
				IXPClassInfoProvider col = instance as IXPClassInfoProvider;
				if (col != null && col.ClassInfo != null) {
					foreach (XPMemberInfo mi in col.ClassInfo.Members) {
						if (!names.ContainsKey(mi.Name) && IsGoodProperty(mi)) {
							cols.Add(CreateColumn(mi));
							names.Add(mi.Name, null);
						}
					}
				}
			}
			return cols;
		}
	}
	class XPViewCriteriaEditor : CriteriaEditor {
		protected override FilterColumnCollection GetColumns(object instance) {
			FilterColumnCollection cols = new FilterColumnCollection();
			IXPClassInfoProvider col = instance as IXPClassInfoProvider;
			Hashtable names = new Hashtable();
			if (col != null && col.ClassInfo != null) {
				foreach (XPMemberInfo mi in col.ClassInfo.Members) {
					if (!names.ContainsKey(mi.Name) && IsGoodProperty(mi)) {
						cols.Add(CreateColumn(mi));
						names.Add(mi.Name, null);
					}
				}
			}
			return cols;
		}
	}
	class XPDataViewCriteriaEditor : CriteriaEditor {
		protected override FilterColumnCollection GetColumns(object instance) {
			FilterColumnCollection cols = new FilterColumnCollection();
			XPDataView col = instance as XPDataView;
			Hashtable names = new Hashtable();
			if (col != null) {
				foreach (PropertyDescriptor prop in ((ITypedList)col).GetItemProperties(null)) {
					if (!names.ContainsKey(prop.Name)) {
						cols.Add(CreateColumn(prop));
						names.Add(prop.Name, null);
					}
				}
			}
			return cols;
		}
	}
	class XPInstantFeedbackSourceCriteriaEditor : CriteriaEditor {
		protected override FilterColumnCollection GetColumns(object instance) {
			FilterColumnCollection cols = new FilterColumnCollection();
			XPInstantFeedbackSource col = instance as XPInstantFeedbackSource;
			Hashtable names = new Hashtable();
			if (col != null) {
				ITypedList typedList = (((IListSource)col).GetList()) as ITypedList;
				if (typedList != null) {
					foreach (PropertyDescriptor prop in typedList.GetItemProperties(null)) {
						if (!names.ContainsKey(prop.Name)) {
							cols.Add(CreateColumn(prop));
							names.Add(prop.Name, null);
						}
					}
				}
			}
			return cols;
		}
	}
	class CriteriaConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if (sourceType == typeof(string)) {
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object val) {
			if (val is string) {
				return CriteriaOperator.Parse((string)val);
			}
			return base.ConvertFrom(context, culture, val);
		}
	}
	public class ObjectClassInfoTypeConverter : System.ComponentModel.ReferenceConverter {
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object val) {
			if (val is string) {
				if ((string)val == "(none)")
					return null;
				try {
					XPDictionary dictionary = ((IXPDictionaryProvider)context.Instance).Dictionary;
					if (dictionary != null)
						return dictionary.QueryClassInfo("", (string)val);
				} catch (Exception e) {
					IUIService s = (IUIService)context.GetService(typeof(IUIService));
					if (s != null)
						s.ShowError(e);
				}
			}
			return base.ConvertFrom(context, culture, val);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType == typeof(string)) {
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object val, Type destType) {
			if (destType == typeof(string)) {
				if (val == null)
					return "(none)";
				if (val is XPClassInfo)
					return ((XPClassInfo)val).FullName;
			}
			return base.ConvertTo(context, culture, val, destType);
		}
		public ObjectClassInfoTypeConverter() : base(typeof(XPClassInfo)) { }
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			SortedList list = new SortedList();
			list.Add("(none)", null);
			try {
				XPDictionary dictionary = ((IXPDictionaryProvider)context.Instance).Dictionary;
				if (dictionary != null) {
					foreach (XPClassInfo obj in dictionary.Classes) {
						if (obj.IsVisibleInDesignTime)
							list.Add(obj.FullName, obj);
					}
				}
			} catch (Exception e) {
				IUIService s = (IUIService)context.GetService(typeof(IUIService));
				if (s != null)
					s.ShowError(e);
			}
			return new StandardValuesCollection(list.Values);
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return false;
		}
	}
	public class FlagsEnumEditor : UITypeEditor {
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if (context != null && context.Instance != null) {
				return UITypeEditorEditStyle.DropDown;
			}
			return base.GetEditStyle(context);
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object obj) {
			try {
				if (context != null && provider != null && context.PropertyDescriptor != null) {
					IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
					if (edSvc != null && context.OnComponentChanging()) {
						CheckedListBox list = new CheckedListBox();
						foreach (object s in Enum.GetValues(context.PropertyDescriptor.PropertyType)) {
							if ((int)s != 0)
								list.Items.Add(s, (obj != null && ((int)obj & (int)s) != 0)
									? CheckState.Checked : CheckState.Unchecked);
						}
						edSvc.DropDownControl(list);
						int newValue = 0;
						foreach (object o in list.CheckedItems) {
							newValue |= (int)o;
						}
						if (obj == null || (int)obj != newValue) {
							Type t = context.PropertyDescriptor.PropertyType;
							obj = Activator.CreateInstance(t);
							t.GetFields()[0].SetValue(obj, newValue);
							context.OnComponentChanged();
						}
					}
				}
			} catch (Exception e) {
				DesignUtilities.ReportError(provider, e.Message);
			}
			return obj;
		}
	}
	class SortingCollectionEditor : CollectionEditor {
		public class Entry : IXPClassInfoProvider {
			string property;
			SortingDirection direction;
			XPClassInfo ci;
			public Entry() {
			}
			public Entry(XPClassInfo ci, string val, SortingDirection direction) {
				property = val;				
				this.ci = ci;
				this.direction = direction;
			}
			[Browsable(false)]
			public XPClassInfo ClassInfo {
				get {
					return ci;
				}
			}
			[DefaultValue(SortingDirection.Ascending)]
			public SortingDirection Direction {
				get {
					return direction;
				}
				set {
					direction = value;
				}
			}
			[Browsable(false)]
			public string Name {
				get {
					return Property == String.Empty || Property == null ? "empty" : Property;
				}
			}
			[Category()]
			[Editor(typeof(PropertyNameEditor), typeof(UITypeEditor))]
			public string Property {
				get {
					return property;
				}
				set {
					property = value;
				}
			}
			[Browsable(false)]
			public XPDictionary Dictionary {
				get { return ci.Dictionary; }
			}
		}
		public SortingCollectionEditor()
			: base(typeof(object[])) {
		}
		protected override Type CreateCollectionItemType() {
			return typeof(Entry);
		}
		protected override object CreateInstance(Type itemType) {
			return new Entry(ci, null, SortingDirection.Ascending);
		}
		protected override bool CanSelectMultipleInstances() {
			return false;
		}		
		XPClassInfo ci;
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			try {
				ci = ((IXPClassInfoProvider)context.Instance).ClassInfo;
				return base.EditValue(context, provider, value);
			} finally {
				ci = null;
			}
		}
		protected override object[] GetItems(object editValue) {
			List<object> entries = new List<object>();
			foreach (SortProperty p in (SortingCollection)editValue)
				entries.Add(new Entry(ci, p.PropertyName, p.Direction));
			return entries.ToArray();
		}
		protected override object SetItems(object editValue, object[] value) {
			SortingCollection values = new SortingCollection();
			foreach (Entry e in value) {
				values.Add(new SortProperty(e.Property, e.Direction));
			}
			return values;
		}
	}
	class XPViewSortingCollectionEditor : CollectionEditor {
		public class Entry : ITypedList {
			string property;
			SortingDirection direction;
			ITypedList ci;
			public Entry() {
			}
			public Entry(ITypedList ci, string val, SortingDirection direction) {
				property = val;
				this.ci = ci;
				this.direction = direction;
			}
			[DefaultValue(SortingDirection.Ascending)]
			public SortingDirection Direction {
				get {
					return direction;
				}
				set {
					direction = value;
				}
			}
			[Browsable(false)]
			public string Name {
				get {
					return Property == String.Empty || Property == null ? "empty" : Property;
				}
			}
			[Category()]
			[Editor(typeof(XPViewPropertyNameEditor), typeof(UITypeEditor))]
			public string Property {
				get {
					return property;
				}
				set {
					property = value;
				}
			}
			public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
				return ci.GetItemProperties(listAccessors);
			}
			public string GetListName(PropertyDescriptor[] listAccessors) {
				return ci.GetListName(listAccessors);
			}
		}
		public XPViewSortingCollectionEditor()
			: base(typeof(object[])) {
		}
		protected override Type CreateCollectionItemType() {
			return typeof(Entry);
		}
		protected override object CreateInstance(Type itemType) {
			return new Entry(ci, null, SortingDirection.Ascending);
		}
		protected override bool CanSelectMultipleInstances() {
			return false;
		}		
		ITypedList ci;
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			try {
				ci = ((ITypedList)context.Instance);
				return base.EditValue(context, provider, value);
			} finally {
				ci = null;
			}
		}
		protected override object[] GetItems(object editValue) {
			List<object> entries = new List<object>();
			foreach (SortProperty p in (SortingCollection)editValue)
				entries.Add(new Entry(ci, p.PropertyName, p.Direction));
			return entries.ToArray();
		}
		protected override object SetItems(object editValue, object[] value) {
			SortingCollection values = new SortingCollection();
			foreach (Entry e in value) {
				values.Add(new SortProperty(e.Property, e.Direction));
			}
			return values;
		}
	}
	class DefaultSortingCollectionEditor : CollectionEditor {
		public class Entry : IXPClassInfoProvider {
			string property;
			SortingDirection direction;
			XPClassInfo ci;
			public Entry() {
			}
			public Entry(XPClassInfo ci, string val, SortingDirection direction) {
				property = val;
				this.ci = ci;
				this.direction = direction;
			}
			[Browsable(false)]
			public XPClassInfo ClassInfo {
				get { return ci; }
			}
			[DefaultValue(SortingDirection.Ascending)]
			public SortingDirection Direction {
				get { return direction; }
				set { direction = value; }
			}
			[Browsable(false)]
			public string Name {
				get {
					return Property == String.Empty || Property == null ? "empty" : Property;
				}
			}
			[Category()]
			[Editor(typeof(PropertyNameEditor), typeof(UITypeEditor))]
			public string Property {
				get { return property; }
				set { property = value; }
			}
			[Browsable(false)]
			public XPDictionary Dictionary {
				get { return ci.Dictionary; }
			}
		}
		public DefaultSortingCollectionEditor()
			: base(typeof(object[])) {
		}
		protected override Type CreateCollectionItemType() {
			return typeof(Entry);
		}
		protected override object CreateInstance(Type itemType) {
			return new Entry(ci, null, SortingDirection.Ascending);
		}
		protected override bool CanSelectMultipleInstances() {
			return false;
		}
		protected override string GetDisplayText(object value) {
			Entry entry = value as Entry;			
			string displayText = entry.Name;
			if (entry.Direction == SortingDirection.Descending) displayText += " DESC";
			return displayText;
		}
		protected XPClassInfo ci;
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			try {
				ci = ((IXPClassInfoProvider)context.Instance).ClassInfo;
				return base.EditValue(context, provider, value);
			} finally {
				ci = null;
			}
		}
		protected override object[] GetItems(object editValue) {
			string[] values = ((string)editValue).Split(';');
			object[] entries = new Entry[values.Length];
			if (values != null && values.Length != 0) {
				for (int i = 0; i < values.Length; i++) {
					string sortingProperty = values[i].Trim();
					SortingDirection direction = SortingDirection.Ascending;
					if (sortingProperty.EndsWith(" DESC", StringComparison.InvariantCultureIgnoreCase)) {
						sortingProperty = sortingProperty.Substring(0, sortingProperty.Length - 5).TrimEnd();
						direction = SortingDirection.Descending;
					} else if (sortingProperty.EndsWith(" ASC", StringComparison.InvariantCultureIgnoreCase)) {
						sortingProperty = sortingProperty.Substring(0, sortingProperty.Length - 4).TrimEnd();
					}
					entries[i] = new Entry(ci, sortingProperty, direction);
				}
			}
			return entries;
		}
		protected override object SetItems(object editValue, object[] value) {
			StringBuilder sb = new StringBuilder();
			bool firstEntry = true;
			foreach (Entry e in value) {
				if (!firstEntry) sb.Append("; ");
				sb.AppendFormat("{0}{1}", e.Property, (e.Direction == SortingDirection.Descending) ? " DESC" : string.Empty);
				firstEntry = false;
			}
			return sb.ToString(); ;
		}
	}
	class DisplayablePropertiesEditor : CollectionEditor {
		public class Entry : IXPClassInfoProvider {
			string property;
			XPClassInfo ci;
			public Entry() {
			}
			public Entry(XPClassInfo ci, string val) {
				property = val;
				this.ci = ci;
			}
			[Browsable(false)]
			public XPClassInfo ClassInfo {
				get {
					return ci;
				}
			}
			[Browsable(false)]
			public string Name {
				get {
					return PropertyName == String.Empty || PropertyName == null ? "empty" : PropertyName;
				}
			}
			[Category()]
			[Editor(typeof(PropertyNameEditor), typeof(UITypeEditor))]
			public string PropertyName {
				get {
					return property;
				}
				set {
					property = value;
				}
			}
			#region IXPDictionaryProvider Members
			[Browsable(false)]
			public XPDictionary Dictionary {
				get { return ci.Dictionary; }
			}
			#endregion
		}
		public DisplayablePropertiesEditor()
			: base(typeof(object[])) {
		}
		protected override Type CreateCollectionItemType() {
			return typeof(Entry);
		}
		protected override object CreateInstance(Type itemType) {
			return new Entry(ci, String.Empty);
		}
		protected override bool CanSelectMultipleInstances() {
			return false;
		}
		XPClassInfo ci;
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			try {
				ci = ((IXPClassInfoProvider)context.Instance).ClassInfo;
				return base.EditValue(context, provider, value);
			} finally {
				ci = null;
			}
		}
		protected override object[] GetItems(object editValue) {
			string[] values = ((string)editValue).Split(';');
			object[] entries = new Entry[values.Length];
			for (int i = 0; i < values.Length; i++) {
				entries[i] = new Entry(ci, values[i]);
			}
			return entries;
		}
		protected override object SetItems(object editValue, object[] value) {
			string[] values = new string[value.Length];
			for (int i = 0; i < values.Length; i++) {
				if (((Entry)value[i]).PropertyName != null && ((Entry)value[i]).PropertyName != String.Empty)
					values[i] = ((Entry)value[i]).PropertyName;
			}
			return string.Join(";", values);
		}
	}
	public class XPViewPropertyNameEditor : UITypeEditor {
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.DropDown;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object obj) {
			ITypedList ci = context.Instance as ITypedList;
			if (context != null && ci != null && provider != null) {
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null) {
					PropertyPicker picker = new PropertyPicker();
					OperandProperty op = CriteriaOperator.Parse((string)obj) as OperandProperty;
					picker.Show(edSvc, ci, (object)op != null ? op.PropertyName : null, true);
					if (String.IsNullOrEmpty(picker.SelectedItemPath))
						return obj;
					string s = new OperandProperty(picker.SelectedItemPath).ToString();
					if ((string)obj != s)
						return s;
				}
			}
			return obj;
		}
	}
	public class PropertyNameEditor : UITypeEditor {
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.DropDown;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object obj) {
			IXPClassInfoProvider ci = context.Instance as IXPClassInfoProvider;
			if (context != null && ci != null && provider != null) {
				XPClassInfo owner = ci.ClassInfo;
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (owner != null && edSvc != null) {
					PropertyPicker picker = new PropertyPicker();
					picker.Show(edSvc, owner, (string)obj, true);
					string s = picker.SelectedItemPath;
					if ((string)obj != s)
						return s;
				}
			}
			return obj;
		}
	}
	public class SessionOwnerDesigner : DevExpress.Utils.Design.BaseComponentDesigner {
		IComponentChangeService componentChangeService;
		ComponentEventHandler componentRemovedHandler;
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			componentChangeService = (IComponentChangeService)this.GetService(typeof(IComponentChangeService));
			if (componentChangeService != null) {
				componentRemovedHandler = new ComponentEventHandler(OnComponentRemoved);
				componentChangeService.ComponentRemoved += componentRemovedHandler;
			}
		}
		protected override void Dispose(bool disposing) {
			if (disposing && componentRemovedHandler != null) {
				componentChangeService.ComponentRemoved -= componentRemovedHandler;
				componentRemovedHandler = null;
				componentChangeService = null;
			}
			base.Dispose(disposing);
		}
		void OnComponentRemoved(object sender, ComponentEventArgs e) {
			if ((Component != null) && ((e.Component != null) && (e.Component is Session))) {
				ISessionProvider component = (ISessionProvider)Component;
				Session session = (Session)e.Component;
				if (component.Session == session) {
					TypeDescriptor.GetProperties(component)["Session"].SetValue(component, null);
				}
			}
		}
	}
	public class XPDataViewTypeConverter : TypeConverter {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(string);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == typeof(string)) {
				return value.ToString();
			}
			throw new NotSupportedException();
		}
	}
	public class SimpleEnumAndTypeEditor : UITypeEditor {
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if (context != null && context.Instance != null) {
				return UITypeEditorEditStyle.DropDown;
			}
			return base.GetEditStyle(context);
		}
		static Type GetTypeFromTypeCode(TypeCode value) {
			switch (value) {
				case TypeCode.Boolean:
					return typeof(bool);
				case TypeCode.Byte:
					return typeof(byte);
				case TypeCode.Char:
					return typeof(char);
				case TypeCode.DateTime:
					return typeof(DateTime);
				case TypeCode.Decimal:
					return typeof(decimal);
				case TypeCode.Double:
					return typeof(double);
				case TypeCode.Int16:
					return typeof(Int16);
				case TypeCode.Int32:
					return typeof(Int32);
				case TypeCode.Int64:
					return typeof(Int64);
				case TypeCode.SByte:
					return typeof(sbyte);
				case TypeCode.Single:
					return typeof(Single);
				case TypeCode.String:
					return typeof(string);
				case TypeCode.UInt16:
					return typeof(UInt16);
				case TypeCode.UInt32:
					return typeof(UInt32);
				case TypeCode.UInt64:
					return typeof(UInt64);
				default:
					return typeof(object);
			}
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object obj) {
			try {
				if (context != null && provider != null && context.PropertyDescriptor != null) {
					IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
					if (edSvc != null && context.OnComponentChanging()) {
						ListBox list = new SimpleEnumAndTypeEditorListBox(edSvc);
						list.BorderStyle = BorderStyle.None;
						int selectedIndex = -1;
						if (context.PropertyDescriptor.PropertyType == typeof(Type)) {
							object currentTypeCode = Type.GetTypeCode((Type)obj);
							foreach (object s in Enum.GetValues(typeof(TypeCode))) {
								if ((int)s != 0) {
									int index = list.Items.Add(s);
									if (object.Equals(currentTypeCode, s)) {
										selectedIndex = index;
									}
								}
							}
						} else {
							foreach (object s in Enum.GetValues(context.PropertyDescriptor.PropertyType)) {
								if ((int)s != 0) {
									int index = list.Items.Add(s);
									if (object.Equals(obj, s)) {
										selectedIndex = index;
									}
								}
							}
						}
						if (selectedIndex >= 0) {
							list.SelectedIndex = selectedIndex;
						}
						edSvc.DropDownControl(list);
						int newValue = list.SelectedItem == null ? 0 : (int)list.SelectedItem;
						if (context.PropertyDescriptor.PropertyType == typeof(Type)) {
							object newType = GetTypeFromTypeCode((TypeCode)newValue);
							if (obj == null || !object.Equals(obj, newType)) {
								obj = newType;
								context.OnComponentChanged();
							}
						} else
							if (obj == null || (int)obj != newValue) {
								Type t = context.PropertyDescriptor.PropertyType;
								obj = Activator.CreateInstance(t);
								t.GetFields()[0].SetValue(obj, newValue);
								context.OnComponentChanged();
							}
					}
				}
			} catch (Exception e) {
				DesignUtilities.ReportError(provider, e.Message);
			}
			return obj;
		}
		public class SimpleEnumAndTypeEditorListBox : ListBox {
			IWindowsFormsEditorService edSvc;
			public SimpleEnumAndTypeEditorListBox(IWindowsFormsEditorService edSvc) {
				this.edSvc = edSvc;
			}
			protected override void OnDoubleClick(EventArgs e) {
				edSvc.CloseDropDown();
			}
		}
	}
	public class XPViewExpressionEditor : DevExpress.XtraEditors.Design.ExpressionEditorBase {
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			object result = base.EditValue(context, provider, value);
			if (result is CriteriaOperator) return result;
			string resultString = result as string;
			if (string.IsNullOrEmpty(resultString)) return null;
			return CriteriaOperator.Parse(resultString);
		}
	}
}
