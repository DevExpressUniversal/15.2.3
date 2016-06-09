#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DimensionBox = DevExpress.DashboardCommon.Native.DataItemBox<DevExpress.DashboardCommon.Dimension>;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	public class PivotItemFormatRuleLevel : IDataItemRepositoryProvider, IXmlSerializableElement {
		const string XmlRow = "Row";
		const string XmlColumn = "Column";
		readonly DimensionBox columnBox;
		readonly DimensionBox rowBox;
		IFormatRulesContext context;
		int lockUpdate;
		[
		DefaultValue(null),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.CreatableDimensionPropertyTypeConverter)
		]
		public Dimension Column {
			get { return columnBox.DataItem; }
			set {
				if(Column != value) {
					this.columnBox.DataItem = value;
					OnChanged();
				}
			}
		}		
		[
		DefaultValue(null),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.CreatableDimensionPropertyTypeConverter)
		]
		public Dimension Row {
			get { return rowBox.DataItem; }
			set {
				if(Row != value) {
					this.rowBox.DataItem = value;
					OnChanged();
				}
			}
		}
		internal IFormatRulesContext Context {
			get { return context; }
			set { context = value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public PivotItemFormatRuleLevel()
			: this(null, null, null) {
		}
		internal PivotItemFormatRuleLevel(IFormatRulesContext context)
			: this(context, null, null) {
		}
		internal PivotItemFormatRuleLevel(IFormatRulesContext context, Dimension row, Dimension column) {
			this.context = context;
			this.rowBox = new DimensionBox(this, XmlRow);
			this.rowBox.DataItem = row;
			this.rowBox.Changed += (sender, e) => OnChanged();
			this.columnBox = new DimensionBox(this, XmlColumn);
			this.columnBox.DataItem = column;
			this.columnBox.Changed += (sender, e) => OnChanged();
		}
		public void BeginUpdate() {
			lockUpdate++;
		}
		public void EndUpdate() {
			if(--lockUpdate == 0) {
				OnChanged();
			}
		}
		public void Assign(PivotItemFormatRuleLevel level) {
			BeginUpdate();
			try {
				Column = level.Column;
				Row = level.Row;
			} finally {
				EndUpdate();
			}
		}
		internal void OnEndLoading() {
			rowBox.OnEndLoading();
			columnBox.OnEndLoading();
		}
		void SaveToXml(XElement element) {
			rowBox.SaveToXml(element);
			columnBox.SaveToXml(element);
		}
		void LoadFromXml(XElement element) {
			rowBox.LoadFromXml(element);
			columnBox.LoadFromXml(element);
		}
		protected void OnChanged() {
			if(Context != null)
				Context.OnChanged(null);
		}
		#region IDataItemRepositoryProvider Members
		DataItemRepository IDataItemRepositoryProvider.DataItemRepository {
			get {
				if(Context != null)
					return Context.DataItemRepositoryProvider != null ? Context.DataItemRepositoryProvider.DataItemRepository : null; 
				return null;
			}
		}
		#endregion
		#region IXmlSerializableElement Members
		void IXmlSerializableElement.SaveToXml(XElement element) {
			this.SaveToXml(element);
		}
		void IXmlSerializableElement.LoadFromXml(XElement element) {
			this.LoadFromXml(element);
		}
		#endregion
	}
}
