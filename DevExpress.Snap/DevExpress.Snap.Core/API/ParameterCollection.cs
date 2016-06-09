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
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections;
using DevExpress.Data;
using DevExpress.XtraEditors.Filtering;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Snap.Localization;
using DevExpress.Snap.Core.Native;
namespace DevExpress.Snap.Core.API {
	[Editor("DevExpress.Snap.Extensions.Native.ParameterCollectionEditor, " + AssemblyInfo.SRAssemblySnapExtensions, typeof(UITypeEditor))]
	public class ParameterCollection : BindingList<Parameter>, IList<IParameter>, IList<IFilterParameter> {
		public Parameter this[string parameterName] {
			get { return GetByName(parameterName); }
		}
		internal Parameter GetByName(string parameterName) {
			foreach (Parameter parameter in this)
				if (parameter.Name == parameterName)
					return parameter;
			return null;
		}
		public void AddRange(Parameter[] parameters) {
			AddRangeCore((IList)parameters, false);
		}
		public void AddRangeByValue(IList parameters) {
			AddRangeCore(parameters, true);
		}
		protected internal void AddRangeCore(IList parameters, bool clone) {
			foreach(Parameter parameter in parameters) {
				Parameter newParameter = clone ? parameter.Clone() : parameter;
				this.Add(newParameter);
			}
		}
		#region IEnumerable<IParameter> Members
		IEnumerator<IParameter> IEnumerable<IParameter>.GetEnumerator() {
			foreach(Parameter item in base.Items) {
				yield return item;
			}
		}
		#endregion
		#region IEnumerable<IFilterParameter> Members
		IEnumerator<IFilterParameter> IEnumerable<IFilterParameter>.GetEnumerator() {
			foreach(Parameter item in base.Items) {
				yield return item;
			}
		}
		#endregion
		#region IList<IParameter> Members
		int IList<IParameter>.IndexOf(IParameter item) {
			return IndexOf((Parameter)item);
		}
		void IList<IParameter>.Insert(int index, IParameter item) {
			Insert(index, (Parameter)item);
		}
		void IList<IParameter>.RemoveAt(int index) {
			RemoveAt(index);
		}
		IParameter IList<IParameter>.this[int index] {
			get {
				return this[index];
			}
			set {
				this[index] = (Parameter)value;
			}
		}
		#endregion
		#region ICollection<IParameter> Members
		void ICollection<IParameter>.Add(IParameter item) {
			Add((Parameter)item);
		}
		void ICollection<IParameter>.Clear() {
			Clear();
		}
		bool ICollection<IParameter>.Contains(IParameter item) {
			return Contains((Parameter)item);
		}
		void ICollection<IParameter>.CopyTo(IParameter[] array, int arrayIndex) {
			CopyToCore(array, arrayIndex);
		}
		int ICollection<IParameter>.Count {
			get { return Count; }
		}
		bool ICollection<IParameter>.IsReadOnly {
			get { return ((IList)this).IsReadOnly; }
		}
		bool ICollection<IParameter>.Remove(IParameter item) {
			return Remove((Parameter)item);
		}
		#endregion
		#region IList<IFilterParameter> Members
		int IList<IFilterParameter>.IndexOf(IFilterParameter item) {
			return IndexOf((Parameter)item);
		}
		void IList<IFilterParameter>.Insert(int index, IFilterParameter item) {
			Insert(index, (Parameter)item);
		}
		IFilterParameter IList<IFilterParameter>.this[int index] {
			get {
				return this[index];
			}
			set {
				this[index] = (Parameter)value;
			}
		}
		#endregion
		#region ICollection<IFilterParameter> Members
		void ICollection<IFilterParameter>.Add(IFilterParameter item) {
			Add((Parameter)item);
		}
		void ICollection<IFilterParameter>.Clear() {
			Clear();
		}
		bool ICollection<IFilterParameter>.Contains(IFilterParameter item) {
			return Contains((Parameter)item);
		}
		void ICollection<IFilterParameter>.CopyTo(IFilterParameter[] array, int arrayIndex) {
			CopyToCore(array, arrayIndex);
		}
		void CopyToCore(IFilterParameter[] array, int arrayIndex) {
			Parameter[] parameters = new Parameter[array.Length];
			CopyTo(parameters, arrayIndex);
			parameters.CopyTo(array, arrayIndex);
		}
		int ICollection<IFilterParameter>.Count {
			get { return Count; }
		}
		bool ICollection<IFilterParameter>.IsReadOnly {
			get { return ((IList)this).IsReadOnly; }
		}
		bool ICollection<IFilterParameter>.Remove(IFilterParameter item) {
			return Remove((Parameter)item);
		}
		#endregion
		internal IParameterService GetParameterService() {
			return new ParameterService(this);
		}
	}
	public class ParameterService : IParameterService {
		readonly ParameterCollection parameters;
		public ParameterService(ParameterCollection parameters) {
			this.parameters = parameters;
		}
		public string AddParameterString { get { return SnapLocalizer.GetString(SnapStringId.ParameterService_AddParameter); } }
		public string CreateParameterString { get { return SnapLocalizer.GetString(SnapStringId.ParameterService_CreateParameter); } }
		public bool CanCreateParameters { get { return true; } }
		public IEnumerable<IParameter> Parameters { get { return this.parameters; } }
		public void AddParameter(IParameter parameter) {
			this.parameters.Add((Parameter)parameter);
		}
		public IParameter CreateParameter(Type type) {
			return new Parameter { Type = type, Name = NameHelper.GetParameterName(this.parameters) };
		}
	}
}
