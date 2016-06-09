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
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Xml.Linq;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Design;
using DevExpress.Data;
using DevExpress.Data.Utils;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.ObjectBinding;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraReports;
namespace DevExpress.DataAccess.ObjectBinding {
#if !DXPORTABLE
	[Designer("DevExpress.DataAccess.Design.VSObjectDataSourceDesigner," + AssemblyInfo.SRAssemblyDataAccessDesign, typeof(IDesigner))]
	[XRDesigner("DevExpress.DataAccess.UI.Design.XRObjectDataSourceDesigner," + AssemblyInfo.SRAssemblyDataAccessUI, typeof(IDesigner))]
	[ToolboxBitmap(typeof(ResFinder), "Bitmaps256.ObjectDataSource.bmp")]
#endif
	[ToolboxItem(false)]
	public class ObjectDataSource : DataComponentBase, ITypedList, IList, IListAdapter, IDataComponent, ISupportInitialize {
		readonly ParameterList parameters;
		ObjectConstructorInfo constructor;
		ResultTypedList result;
		int updateCnt;
		object dataSource;
		string dataMember;
		public ObjectDataSource(IContainer container) : this() {
			Guard.ArgumentNotNull(container, "container");
			container.Add(this);
		}
		public ObjectDataSource() {
			parameters = new ParameterList();
		}
		[Editor("DevExpress.DataAccess.UI.Native.ObjectBinding.DataSourceEditor," + AssemblyInfo.SRAssemblyDataAccessUI, typeof(UITypeEditor))]
		[DXDisplayName(typeof(ResFinder), "DevExpress.DataAccess.ObjectBinding.ObjectDataSource.DataSource")]
#if !SL
	[DevExpressDataAccessLocalizedDescription("ObjectDataSourceDataSource")]
#endif
		[Category("Data")]
		public object DataSource {
			get { return dataSource; }
			set {
				if(value == dataSource)
					return;
				BeginUpdate();
				dataSource = value;
				EndUpdate();
			}
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public Type DataSourceType {
			get {
				return DataSource != null ? DataSource.GetType() : null;
			}
			set {
				DataSource = value;
			}
		}
		[DefaultValue(null)]
		[Editor("DevExpress.DataAccess.UI.Native.ObjectBinding.DataMemberEditor," + AssemblyInfo.SRAssemblyDataAccessUI, typeof(UITypeEditor))]
		[DXDisplayName(typeof(ResFinder), "DevExpress.DataAccess.ObjectBinding.ObjectDataSource.DataMember")]
#if !SL
	[DevExpressDataAccessLocalizedDescription("ObjectDataSourceDataMember")]
#endif
		[Category("Data")]
		public string DataMember {
			get { return dataMember; }
			set {
				if(value == dataMember)
					return;
				BeginUpdate();
				dataMember = value;
				EndUpdate();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Editor("DevExpress.DataAccess.UI.Native.ObjectBinding.ParametersEditor," + AssemblyInfo.SRAssemblyDataAccessUI, typeof(UITypeEditor))]
		[DXDisplayName(typeof(ResFinder), "DevExpress.DataAccess.ObjectBinding.ObjectDataSource.Parameters")]
#if !SL
	[DevExpressDataAccessLocalizedDescription("ObjectDataSourceParameters")]
#endif
		[Category("Data")]
		public ParameterList Parameters { get { return parameters; } }
		[DefaultValue(null)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Editor("DevExpress.DataAccess.UI.Native.ObjectBinding.CtorParametersEditor," + AssemblyInfo.SRAssemblyDataAccessUI, typeof(UITypeEditor))]
		[DXDisplayName(typeof(ResFinder), "DevExpress.DataAccess.ObjectBinding.ObjectDataSource.Constructor")]
#if !SL
	[DevExpressDataAccessLocalizedDescription("ObjectDataSourceConstructor")]
#endif
		[Category("Data")]
		public ObjectConstructorInfo Constructor {
			get { return constructor; }
			set {
				if(ObjectConstructorInfo.EqualityComparer.Equals(constructor, value))
					return;
				BeginUpdate();
				constructor = value;
				EndUpdate();
			}
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[XtraSerializableProperty]
		public string Base64 {
			get { return Base64Helper.Encode(SaveToXml()); }
			set {
				BeginUpdate();
				try { LoadFromXml(Base64Helper.Decode(value)); }
				finally { EndUpdate(); }
			}
		}
		internal ResultTypedList Result {
			get {
				if(result == null)
					Fill(null, true);
				return result;
			}
		}
		protected override IEnumerable<IParameter> AllParameters {
			get { return Constructor != null && Constructor.Parameters != null ? Parameters.Union(Constructor.Parameters) : Parameters; }
		}
		public void Fill() {
			Fill(null);
		}
		public void Invalidate() {
			result = null;
			UpdateSchema();
		}
		public void BeginUpdate() { updateCnt++; }
		public void EndUpdate() {
			if(updateCnt <= 0)
				throw new InvalidOperationException("The object data source has not yet been started updating.");
			if(--updateCnt == 0)
				UpdateSchema();
		}
		#region ShouldSerialize...
		protected virtual bool ShouldSerializeDataSourceType() {
			return !ShouldSerializeDataSource();
		}
		bool ShouldSerializeDataSource() {
			return DataSource is Type;
		}
		#endregion
		public override XElement SaveToXml() { return ObjectDataSourceSerializer.SaveToXml(this, ExtensionsProvider); }
		public override void LoadFromXml(XElement element) { ObjectDataSourceSerializer.LoadFromXml(this, element, ExtensionsProvider); }
		protected override void Fill(IEnumerable<IParameter> sourceParameters) {
			Fill(sourceParameters, false);
		}
		internal void Fill(IEnumerable<IParameter> sourceParameters, bool schemaOnly) {
			if(DataSource == null) {
				result = new ResultTypedList(Name, PropertyDescriptorCollection.Empty);
				return;
			}
			IList<IParameter> sourceParametersList = sourceParameters == null
				? new IParameter[0]
				: sourceParameters as IList<IParameter> ?? sourceParameters.ToList();
			ParameterList dataMemberParameters = (schemaOnly)
				? ObjectDataSourceFillHelper.EvaluateParametersForSchema(Parameters)
				: ObjectDataSourceFillHelper.EvaluateParametersForResult(Parameters, sourceParametersList);
			ParameterList ctorParameters = (Constructor != null)
				? (schemaOnly)
					 ? ObjectDataSourceFillHelper.EvaluateParametersForSchema(Constructor.Parameters)
					 : ObjectDataSourceFillHelper.EvaluateParametersForResult(Constructor.Parameters, sourceParametersList)
				: null;
			ObjectDataSourceFillHelper.FindPropertiesHelper helper = new ObjectDataSourceFillHelper.FindPropertiesHelper(DataSource, DataMember, dataMemberParameters, (Constructor == null), ctorParameters);
			PropertyDescriptorCollection pdc = helper.PDC;
			object instance = helper.Instance;
			object data = helper.Data;
			if(schemaOnly) {
				ITypedList typedList = (string.IsNullOrEmpty(DataMember) ? instance : data) as ITypedList;
				result = typedList != null
					? new ResultTypedList(Name, typedList, new object[0])
					: new ResultTypedList(Name, pdc);
			}
			else {
				instance = instance ?? (Constructor == null
					? ObjectDataSourceFillHelper.CreateInstanceForResult(DataSource)
					: ObjectDataSourceFillHelper.CreateInstanceForResult(DataSource, ctorParameters));
				data = data ?? ((string.IsNullOrEmpty(DataMember)
					? instance
					: ObjectDataSourceFillHelper.BrowseForResult(instance, DataMember, dataMemberParameters)));
				result = ObjectDataSourceFillHelper.CreateTypedList(data, Name, pdc);
			}
			OnAfterFill(sourceParametersList);
		}
		internal Type ResolveType() { return DataSource as Type ?? DataSource.GetType(); }
		void UpdateSchema() {
			try { Fill(null, true); }
			catch(Exception ex) {
				result = new ResultTypedList(Name, PropertyDescriptorCollection.Empty);
				if(ex is NoDefaultConstructorException)
					throw;
			}
		}
		protected virtual void OnAfterFill(IEnumerable<IParameter> parameters) {
		}
		#region Overrides of DataComponentBase
		protected override string GetDataMember() { return null; }
		#endregion
		#region Implementation of IListAdapter
		void IListAdapter.FillList(IServiceProvider servProvider) {
			IParameterSupplierBase parameterSupplier = servProvider != null ? servProvider.GetService<IParameterSupplierBase>() : null;
			IEnumerable<IParameter> sourceParameters = parameterSupplier != null ? parameterSupplier.GetIParameters() : null;
			Fill(sourceParameters);
		}
		bool IListAdapter.IsFilled { get { return result != null; } }
		#endregion
		#region Implementation of IDataComponent
		string IDataComponent.DataMember { get { return GetDataMember(); } }
		#endregion
		#region Implementation of ISupportInitialize
		public virtual void BeginInit() {
			if(updateCnt > 0)
				throw new InvalidOperationException("The object data source is already being updated.");
			BeginUpdate();
		}
		public virtual void EndInit() {
			if(updateCnt > 1)
				throw new InvalidOperationException("The object data source is still being updated.");
			EndUpdate();
		}
		#endregion
		#region Implementation of ITypedList
		public string GetListName(PropertyDescriptor[] listAccessors) {
			if(DataSource == null)
				return string.Empty;
			return Result.GetListName(listAccessors);
		}
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
			if(DataSource == null)
				return PropertyDescriptorCollection.Empty;
			return Result.GetItemProperties(listAccessors);
		}
		#endregion
		#region Implementation of IEnumerable
		IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable)Result).GetEnumerator(); }
		#endregion
		#region Implementation of ICollection
		void ICollection.CopyTo(Array array, int index) { ((ICollection)Result).CopyTo(array, index); }
		int ICollection.Count { get { return Result.Count; } }
		object ICollection.SyncRoot { get { return Result.SyncRoot; } }
		bool ICollection.IsSynchronized { get { return Result.IsSynchronized; } }
		#endregion
		#region Implementation of IList
		int IList.Add(object value) { return ((IList)Result).Add(value); }
		bool IList.Contains(object value) { return ((IList)Result).Contains(value); }
		void IList.Clear() { ((IList)Result).Clear(); }
		int IList.IndexOf(object value) { return ((IList)Result).IndexOf(value); }
		void IList.Insert(int index, object value) { ((IList)Result).Insert(index, value); }
		void IList.Remove(object value) { ((IList)Result).Remove(value); }
		void IList.RemoveAt(int index) { ((IList)Result).RemoveAt(index); }
		object IList.this[int index] { get { return Result[index]; } set { Result[index] = value; } }
		bool IList.IsReadOnly {
			get { return result.IsReadOnly; } }
		bool IList.IsFixedSize {
			get { return result.IsFixedSize; } }
		#endregion
	}
}
