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
using System.ComponentModel.Design;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.ComponentModel.Design.Serialization;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Design;
using DevExpress.XtraGrid.Registrator;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Serializers;
using DevExpress.Data;
using DevExpress.Data.Helpers;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraEditors.Design;
using DevExpress.XtraEditors.Repository;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraGrid.TypeConverters;
using DevExpress.Data.Browsing.Design;
using DevExpress.XtraGrid.Views.WinExplorer;
using DevExpress.XtraGrid.EditForm;
namespace DevExpress.XtraGrid.TypeConverters {
	class GridHelper {
		public static BaseView GetBaseView(ITypeDescriptorContext context) {
			object instanceValue = DXObjectWrapper.GetInstance(context);
			if(instanceValue == null) return null;
			ColumnView view = GetView(context);
			if(view != null) return view;
			return instanceValue as BaseView;
		}
		public static ColumnView GetView(ITypeDescriptorContext context) {
			object instanceValue = DXObjectWrapper.GetInstance(context);
			if(instanceValue == null) return null;
			GridColumn column = GetColumn(context);
			if(column != null) return column.View;
			ColumnView view = instanceValue as ColumnView;
			if(view != null) return view;
			GridSummaryItem item = GetSummaryItem(context);
			if(item != null && item.Collection != null) {
				return item.Collection.View as ColumnView;
			}
			StyleFormatCondition condition = GetFormatCondition(context);
			if(condition != null && condition.Collection != null) {
				return condition.Collection.View as ColumnView;
			}
			GridFormatRule rule = GetFormatRule(context);
			if(rule != null && rule.Collection != null) return rule.Collection.View;
			return null;
		}
		public static GridColumn GetColumn(ITypeDescriptorContext context) {
			object instanceValue = DXObjectWrapper.GetInstance(context);
			if(instanceValue == null) return null;
			GridColumn column = instanceValue as GridColumn;
			if(column != null) return column;
			DevExpress.XtraGrid.Design.GridColumnDesigner.ColumnDesignerActionList columnAction = instanceValue as DevExpress.XtraGrid.Design.GridColumnDesigner.ColumnDesignerActionList;
			if(columnAction != null) return (GridColumn)columnAction.Component;
			return null;
		}
		public static GridSummaryItem GetSummaryItem(ITypeDescriptorContext context) { return GetObject<GridSummaryItem>(context);  }
		public static StyleFormatCondition GetFormatCondition(ITypeDescriptorContext context) { return GetObject<StyleFormatCondition>(context); }
		public static GridFormatRule GetFormatRule(ITypeDescriptorContext context) { return GetObject<GridFormatRule>(context); }
		public static T GetObject<T>(ITypeDescriptorContext context) where T : class {
			object instanceValue = DXObjectWrapper.GetInstance(context);
			if(instanceValue == null) return null;
			return instanceValue as T;
		}
	}
	#region FieldNameTypeConverter
	class FieldNameTypeConverter : StringConverter {
		protected virtual ColumnView GetGridView(ITypeDescriptorContext context) {
			return GridHelper.GetView(context);
		}
		internal static string[] GetFieldList(ColumnView view) {
			DataColumnInfo[] columns = GetDataColumnInfo(view);
			if(columns == null || columns.Length == 0) return null;
			string[] list = new string[columns.Length];
			for(int n = 0; n < columns.Length; n++) {
				list[n] = columns[n].Name;
			}
			return list;
		}
		protected virtual StandardValuesCollection GetFieldList(ITypeDescriptorContext context) {
			string[] list = GetFieldList(GetGridView(context));
			if(list == null) return null;
			return new StandardValuesCollection(list);
		}
		static DataColumnInfo[] GetDataColumnInfo(ColumnView view) {
			if(view == null || view.GridControl == null) return null;
			if(view == view.GridControl.MainView) {
				return new MasterDetailHelper().GetDataColumnInfo(view.GridControl.BindingContext, view.GridControl.DataSource, view.GridControl.DataMember);
			}
			DetailNodeInfo info = FindDetailNodeInfo(view);
			if(info != null && info.Columns != null) return info.Columns;
			if(info == null || info.List == null) return null;
			return new MasterDetailHelper().GetDataColumnInfo(null, info.List, null);
		}
		internal static DetailNodeInfo FindDetailNodeInfo(BaseView view) {
			if(view == null || view.GridControl == null || !view.GridControl.LevelTree.HasChildren) return null;
			GridLevelNode levelNode = view.GridControl.LevelTree.Find(view);
			if(levelNode == null) return null;
			GridLevelNode[] levelNodes = new GridLevelNode[levelNode.Level];
			for(int n = levelNode.Level - 1; n >= 0; n--) {
				levelNodes[n] = levelNode;
				levelNode = levelNode.Parent;
				if(levelNode == null || levelNode.IsRootLevel) break;
		}
			return FindDetail(levelNodes, new MasterDetailHelper().GetDetailInfo(view.GridControl.BindingContext, view.GridControl.DataSource, view.GridControl.DataMember));
		}
		static DetailNodeInfo FindDetail(GridLevelNode[] levelNodes, DetailNodeInfo[] list) {
			if(levelNodes == null || list == null || levelNodes[0] == null) return null;
			for(int n = 0; n < levelNodes.Length; n++) {
				bool found = false;
			foreach(DetailNodeInfo node in list) {
					if(node.Caption == levelNodes[n].RelationName) {
						if(n == levelNodes.Length - 1) return node;
						if(!node.HasChildren) return null;
						found = true;
						list = node.Nodes;
						break;
					}
				}
				if(!found) return null;
			}
			return null;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return GetFieldList(context);
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override bool IsValid(ITypeDescriptorContext context, object value) {
			return true;
		}
	}
	#endregion FieldNameTypeConverter
	#region GroupSummaryItemTypeConverter
	public sealed class GroupSummaryItemTypeConverter : ExpandableObjectConverter {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType == typeof(InstanceDescriptor)) {
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == null) {
				throw new ArgumentNullException("destinationType");
			}
			if (destinationType == typeof(InstanceDescriptor) && value is GridGroupSummaryItem) {
				GridGroupSummaryItem summary = (GridGroupSummaryItem)value;
				ConstructorInfo ctor;
				if(summary.Tag == null) {
					ctor = typeof(GridGroupSummaryItem).GetConstructor(new Type[] {typeof(SummaryItemType), typeof(string), typeof(GridColumn), typeof(string)});
					if(ctor != null)
						return new InstanceDescriptor(ctor, new object[] {summary.SummaryType, summary.FieldName, summary.ShowInGroupColumnFooter, summary.DisplayFormat});
				} else {
					ctor = typeof(GridGroupSummaryItem).GetConstructor(new Type[] {typeof(SummaryItemType), typeof(string), typeof(GridColumn), typeof(string), typeof(object)});
					if(ctor != null)
						return new InstanceDescriptor(ctor, new object[] {summary.SummaryType, summary.FieldName, summary.ShowInGroupColumnFooter, summary.DisplayFormat, summary.Tag});
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	#endregion
	#region ColumnReferenceConverter
	public class ColumnReferenceConverter : System.ComponentModel.TypeConverter {
		protected virtual string None { get { return DevExpress.XtraGrid.Design.Properties.Resources.NoneString; } }
		protected virtual string GetColumnName(GridColumn column) {
			IComponent comp = column as IComponent;
			if(comp.Site != null)
				return comp.Site.Name;
			if(!string.IsNullOrEmpty(column.Name)) return column.Name; 
			return "GridColumn" + column.AbsoluteIndex.ToString();
		}
		protected virtual ColumnView GetGridView(ITypeDescriptorContext context) { return GridHelper.GetView(context); }
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType.Equals(typeof(string))) {
				if(value == null) return None;
				if(value is GridColumn) {
					return GetColumnName(value as GridColumn);
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type type) {
			if(GetGridView(context) != null) {
				if(type != null && type.Equals(typeof(string))) {
					return true;
				}
			}
			return base.CanConvertFrom(context, type);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if(value == null) return null;
			if(value is string) {
				string source = value.ToString();
				if(source == None) return null;
				ColumnView view = GetGridView(context);
				if(view != null) {
					foreach(GridColumn col in view.Columns) {
						if(source == GetColumnName(col)) return col;
					}
				}
				return null;
			}
			return base.ConvertFrom(context, culture, value);
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			if(context == null || context.Instance == null) return null;
			ArrayList array = new ArrayList();
			array.Add(null);
			ColumnView columnView = GetGridView(context);
			if(columnView != null) {
				foreach(GridColumn col in columnView.Columns) {
					array.Add(col);
				}
			}
			return new StandardValuesCollection(array);
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
	}
	public class GroupSummaryColumnReferenceConverter : ColumnReferenceConverter {
		protected override string None { get { return DevExpress.XtraGrid.Design.Properties.Resources.ShowInGroupRowCaption; } }
	}
	#endregion ColumnReferenceConverter
	#region WinExplorerViewColumnReferenceConverter
	public class WinExplorerViewColumnReferenceConverter : ColumnReferenceConverter {
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			if(context == null || context.Instance == null) return null;
			ArrayList array = new ArrayList();
			array.Add(null);
			GridColumnCollection columns = GetColumns(context);
			if(columns != null) {
				foreach(GridColumn col in columns) {
					array.Add(col);
				}
			}
			return new StandardValuesCollection(array);
		}
		protected override ColumnView GetGridView(ITypeDescriptorContext context) {
			if(context == null || context.Instance == null)
				return null;
			WinExplorerViewColumns columnsSet = DXObjectWrapper.GetInstance(context) as WinExplorerViewColumns;
			return columnsSet.View;
		}
		protected virtual GridColumnCollection GetColumns(ITypeDescriptorContext context) {
			ColumnView columnView = GetGridView(context);
			return columnView != null ? columnView.Columns : null;
		}
	}
	#endregion WinExplorerViewColumnReferenceConverter
	#region ColumnEditConverter
	public sealed class ColumnEditConverter : ComponentConverter {
		public ColumnEditConverter(Type t) : base(t) { }
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return false;
		}
	}
	#endregion ColumnEditConverter
}
namespace DevExpress.XtraGrid.Design {
	public class ColumnEditEditor : EditFieldEditor {
		GridColumn GetGridColumn(ITypeDescriptorContext context) { return GridHelper.GetColumn(context); }
		ColumnView GetColumnView(ITypeDescriptorContext context) { return GridHelper.GetView(context); }
		protected override RepositoryItemCollection[] GetRepository(ITypeDescriptorContext context) {
			ColumnView view = GetColumnView(context);
			if(view != null && view.GridControl != null) {
				RepositoryItemCollection[] res = null;
				if(view.GridControl.ExternalRepository != null) {
					res = new RepositoryItemCollection[2];
					res[1] = view.GridControl.ExternalRepository.Items;
				} else {
					res = new RepositoryItemCollection[1];
				}
				res[0] = view.GridControl.RepositoryItems;
				return res;
			}
			return null;
		}
	}
	public class PaintStyleNameConverter : StringConverter {
		private static string none = "(Default)";
		protected virtual bool ShowAllStyles { get { return false; } }
		protected virtual BaseInfoRegistrator GetRegistrator(ITypeDescriptorContext context) {
			BaseView view = GridHelper.GetBaseView(context);
			if(view != null) return view.BaseInfo;
			return null;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			BaseInfoRegistrator regs = GetRegistrator(context);
			ArrayList names = new ArrayList();
			if(regs != null) {
				foreach(ViewPaintStyle paintStyle in regs.PaintStyles) {
					names.Add(paintStyle.Name);
				}
			}
			names.Insert(0, none);
			return new StandardValuesCollection(names);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType.Equals(typeof(string))) {
				if(value == null || value.ToString() == "" || value.ToString() == "Default") return none;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if(value == null) return none;
			if(value is string) {
				string source = value.ToString();
				if(source == none) return "Default";
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
	public class GridColumnNameEditor : ColumnNameEditor {
		public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if(context == null) return System.Drawing.Design.UITypeEditorEditStyle.None;
			if(IsLevelDefault(context)) return System.Drawing.Design.UITypeEditorEditStyle.None;
			return base.GetEditStyle(context);
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			BaseView view = GetView(context);
			if(IsLevelDefault(context)) return value;
			return base.EditValue(context, provider, value);
		}
		bool IsLevelDefault(ITypeDescriptorContext context) {
			return GetView(context) != null && GetView(context).IsLevelDefault;
		}
		protected override object GetDataSource(ITypeDescriptorContext context) {
			GridControl control = GetGridControl(context);
			if(control == null)
				return null;
			return GetDataSource(control);
		}
		internal static object GetDataSource(GridControl control) {
			object dataSource = control.DataSource;
			string dataMember = control.DataMember;
			DataSet ds = dataSource as DataSet;
			if(ds != null) {
				if(dataMember == string.Empty)
					dataSource = ds.Tables.Count > 0 ? ds.Tables[0] : null;
				else
					dataSource = ds.Tables[dataMember];
			}
			return dataSource;
		}
		protected BaseView GetView(ITypeDescriptorContext context) {
			BaseView res = GridHelper.GetView(context);
			if(res != null) return res;
			object instanceValue = DXObjectWrapper.GetInstance(context);
			GridColumn column = instanceValue as GridColumn;
			if(column != null && column.View != null)
				return column.View;
			GridSummaryItem item = instanceValue as GridSummaryItem;
			if(item != null && item.Collection != null && item.Collection.View != null)
				return item.Collection.View;
			ColumnView view = instanceValue as ColumnView;
			if(view != null)
				return view;
			return null;
		}
		protected GridControl GetGridControl(ITypeDescriptorContext context) {
			BaseView view = GetView(context);
			return view == null ? null : view.GridControl;
		}
	}
	public class EditFormColumnCollectionEditor : CollectionEditor {
		public EditFormColumnCollectionEditor(Type type) : base(type) {
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			EditFormColumnCollectionReadOnly newCollection = new EditFormColumnCollectionReadOnly();
			newCollection.AddRange(value as ICollection);
			newCollection.MakeReadOnly();
			base.EditValue(context, provider, newCollection);
			return value;
		}
		class EditFormColumnCollectionReadOnly : EditFormColumnCollection {
			bool isreadonly = false;
			public void MakeReadOnly() {
				isreadonly = true;
			}
			protected override bool IsReadOnly {
				get {
					return isreadonly;
				}
			}
		}
	}
}
