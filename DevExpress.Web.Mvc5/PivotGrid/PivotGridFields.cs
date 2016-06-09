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

using DevExpress.Web.ASPxPivotGrid;
using DevExpress.Web.Mvc.Internal;
using DevExpress.XtraPivotGrid.Data;
using System;
using System.ComponentModel;
namespace DevExpress.Web.Mvc {
	public class MVCxPivotGridFieldCollection: PivotGridFieldCollection {
		public MVCxPivotGridFieldCollection()
			: base(null) {
		}
		protected internal MVCxPivotGridFieldCollection(PivotGridMvcData data)
			: base(data) {
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new PivotGridData Data { get {return base.Data; } }
		public void Add(Action<MVCxPivotGridField> method) {
			method(Add());
		}
		public new MVCxPivotGridField Add() {
			return (MVCxPivotGridField)base.Add();
		}
		public new MVCxPivotGridField Add(string fieldName, XtraPivotGrid.PivotArea area) {
			return (MVCxPivotGridField)base.Add(fieldName, area);
		}
		protected override void AddCore(XtraPivotGrid.PivotGridFieldBase field) {
			List.Add(field);
		}
		protected override XtraPivotGrid.PivotGridFieldBase CreateField(string fieldName, XtraPivotGrid.PivotArea area) {
			MVCxPivotGridField field = new MVCxPivotGridField(fieldName, area);
			if (string.IsNullOrEmpty(field.Name))
				field.ID = GenerateName(field.FieldName);
			return field;
		}
	}
	public class MVCxPivotGridField: PivotGridField {
		public MVCxPivotGridField()
			: base() {
		}
		public MVCxPivotGridField(string fieldName, XtraPivotGrid.PivotArea area)
			: base(fieldName, area) {
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new PivotGridData Data { get { return base.Data; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool IsLoading { get { return false; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool IsDesignTime { get { return base.IsDesignTime; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool SelectedAtDesignTime { get { return base.SelectedAtDesignTime; } set { base.SelectedAtDesignTime = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new XtraPivotGrid.PivotGridGroup Group { get { return base.Group; } }
		protected internal string HeaderTemplateContent { get; set; }
		protected internal Action<PivotGridHeaderTemplateContainer> HeaderTemplateContentMethod { get; set; }
		protected internal string ValueTemplateContent { get; set; }
		protected internal Action<PivotGridFieldValueTemplateContainer> ValueTemplateContentMethod { get; set; }
		public void SetHeaderTemplateContent(Action<PivotGridHeaderTemplateContainer> contentMethod) {
			HeaderTemplateContentMethod = contentMethod;
		}
		public void SetHeaderTemplateContent(string content) {
			HeaderTemplateContent = content;
		}
		public void SetValueTemplateContent(Action<PivotGridFieldValueTemplateContainer> contentMethod) {
			ValueTemplateContentMethod = contentMethod;
		}
		public void SetValueTemplateContent(string content) {
			ValueTemplateContent = content;
		}
		protected override bool IsDataDeserializing {
			get { return Data == null ? true : base.IsDataDeserializing; } 
		}
	}
	public class MVCxPivotGridWebGroupCollection: PivotGridWebGroupCollection {
		public MVCxPivotGridWebGroupCollection()
			: base(null) {
		}
		public new MVCxPivotGridWebGroup this[int index] { get { return (MVCxPivotGridWebGroup)base[index]; } }
		protected override XtraPivotGrid.PivotGridGroup CreateGroup() {
			return new MVCxPivotGridWebGroup();
		}
	}
	public class MVCxPivotGridWebGroup: PivotGridWebGroup {
		public MVCxPivotGridWebGroup()
			: base() {
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool IsFilterAllowed { get { return base.IsFilterAllowed; } }
	}
}
