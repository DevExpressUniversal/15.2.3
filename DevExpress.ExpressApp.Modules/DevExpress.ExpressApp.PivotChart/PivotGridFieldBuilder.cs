#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.Data.PivotGrid;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.XtraPivotGrid;
namespace DevExpress.ExpressApp.PivotChart {
	internal static class NumericTypes {
		private static readonly List<Type> numericTypes = new List<Type>();
		static NumericTypes() {
			numericTypes.AddRange(new Type[] {
				typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long),
				typeof(ulong), typeof(char), typeof(float), typeof(double), typeof(decimal)});
		}
		public static bool IsNumeric(Type type, bool checkNullableUnderlyingType) {
			bool result = numericTypes.Contains(type);
			if(!result && checkNullableUnderlyingType) {
				Type nullableUnderlyingType = Nullable.GetUnderlyingType(type);
				if(nullableUnderlyingType != null) {
					result = numericTypes.Contains(nullableUnderlyingType);
				}
			}
			return result;
		}
	}
	public interface ICanApplySettingsFromModel {
		void SetModel(IModelApplication model);
		void ApplySettings();
	}
	public interface ISupportPivotGridFieldBuilder {
		PivotGridFieldBuilder FieldBuilder { get; set; }
	}
	public class PivotGridFieldBuilder : ICanApplySettingsFromModel {
		private IAnalysisControl owner;
		private IObjectSpace objectSpace;
		private IModelApplication modelApplication;
		private IModelMember GetPropertyModel(IMemberInfo memberInfo) {
			IModelMember result = null;
			if(modelApplication != null) {
				IModelClass modelClass = modelApplication.BOModel.GetClass(memberInfo.LastMember.Owner.Type);
				if(modelClass != null) {
					result = modelClass.FindOwnMember(memberInfo.LastMember.Name);
				}
			}
			return result;
		}
		private PivotGridFieldBase FindPivotGridField(string bindingPropertyName) {
			return Owner.Fields[bindingPropertyName];
		}
		private PivotGridFieldBase GetPivotGridField(string bindingPropertyName) {
			PivotGridFieldBase field = FindPivotGridField(bindingPropertyName);
			if(field == null) {
				throw new ArgumentException(string.Format("The '{0}' field is absent.", bindingPropertyName));
			}
			return field;
		}
		private string GetMemberDisplayFormat(IMemberInfo memberInfo) {
			string result = "";
			IModelMember modelMember = GetPropertyModel(memberInfo);
			if(modelMember != null) {
				result = modelMember.DisplayFormat;
			}
			else {
				ModelDefaultAttribute modelDefaultAttribute = null;
				foreach(ModelDefaultAttribute attribute in memberInfo.FindAttributes<ModelDefaultAttribute>()) {
					if(attribute.PropertyName == "DisplayFormat") {
						modelDefaultAttribute = attribute;
						break;
					}
				}
				if(modelDefaultAttribute != null) {
					result = modelDefaultAttribute.PropertyValue;
				}
			}
			return result;
		}
		private void SetPivotGridFieldDisplayFormat(PivotGridFieldBase field, string displayFormat) {
			field.CellFormat.FormatType = DevExpress.Utils.FormatType.Custom;
			field.CellFormat.FormatString = displayFormat;
			field.ValueFormat.FormatType = DevExpress.Utils.FormatType.Custom;
			field.ValueFormat.FormatString = displayFormat;
			field.GrandTotalCellFormat.FormatType = DevExpress.Utils.FormatType.Custom;
			field.GrandTotalCellFormat.FormatString = displayFormat;
		}
		protected virtual void SetupPivotGridField(PivotGridFieldBase field, Type memberType, string displayFormat) {
			Guard.ArgumentNotNull(field, "field");
			Guard.ArgumentNotNull(memberType, "memberType");
			if(memberType == typeof(DateTime)) {
				field.GroupInterval = DevExpress.XtraPivotGrid.PivotGroupInterval.Date;
			}
			SetPivotGridFieldDisplayFormat(field, displayFormat);
		}
		public PivotGridFieldBuilder(IAnalysisControl owner) {
			Guard.ArgumentNotNull(owner, "owner");
			this.owner = owner;
		}
		public virtual void SetupPivotGridField(IMemberInfo memberInfo) {
			string bindingPropertyName = GetBindingName(memberInfo);
			PivotGridFieldBase field = GetPivotGridField(bindingPropertyName);
			SetupPivotGridField(field, memberInfo.MemberType, GetMemberDisplayFormat(memberInfo));
		}
		public string GetBindingName(IMemberInfo memberInfo) {
			string result = memberInfo.Name;
			IMemberInfo displayableMember = DevExpress.Persistent.Base.ReflectionHelper.FindDisplayableMemberDescriptor(memberInfo);
			if(displayableMember != null) {
				result = displayableMember.BindingName;
			}
			return result;
		}
		public virtual PivotSummaryType GetSummaryType(IMemberInfo memberInfo) {
			PivotSummaryType result = PivotSummaryType.Count;
			if(NumericTypes.IsNumeric(memberInfo.MemberType, true)) {
				result = PivotSummaryType.Sum;
			}
			return result;
		}
		protected virtual void AddFieldCore(string caption, string bindingPropertyName, IMemberInfo propertyDescriptor) {			
			Owner.AddPivotGridField(caption, bindingPropertyName, GetSummaryType(propertyDescriptor));
			SetupPivotGridField(propertyDescriptor);
		}
		protected IAnalysisInfo GetAnalysisInfo() {
			if(Owner.DataSource != null) {
				return Owner.DataSource.AnalysisInfo;
			}
			return null;
		}
		public virtual void RebuildFields() {
			try {
				Owner.BeginUpdate();
				Owner.Fields.Clear();
				IAnalysisInfo analysisInfo = GetAnalysisInfo();
				if(analysisInfo != null) {
					String displayableProperties = "Oid";
					ITypeInfo objectTypeInfo = XafTypesInfo.Instance.FindTypeInfo(analysisInfo.DataType);
					if(objectTypeInfo != null && objectTypeInfo.KeyMember != null) {
						displayableProperties = GetBindingName(objectTypeInfo.KeyMember);
					}
					foreach(string propertyName in analysisInfo.DimensionProperties) {
						IMemberInfo propertyDescriptor = objectTypeInfo.FindMember(propertyName);
						if(propertyDescriptor != null) {
							string bindingPropertyName = GetBindingName(propertyDescriptor);
							if(objectTypeInfo.KeyMember != propertyDescriptor) {
								displayableProperties += ";" + bindingPropertyName;
							}
							AddFieldCore(CaptionHelper.GetFullMemberCaption(objectTypeInfo, propertyName), bindingPropertyName, propertyDescriptor);
						}
					}
					if(objectSpace != null) {
						objectSpace.SetDisplayableProperties(Owner.DataSource.PivotGridDataSource, displayableProperties);
					}
				}
			}
			finally {
				Owner.EndUpdate();
			}
		}
		public void SetModel(IModelApplication modelApplication) {
			this.modelApplication = modelApplication;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void SetObjectSpace(IObjectSpace objectSpace) {
			this.objectSpace = objectSpace;
		}
		protected virtual void OnSettingsApplied() {
			if(SettingsApplied != null) {
				SettingsApplied(null, EventArgs.Empty);
			}
		}
		public virtual void ApplySettings() {
			try {
				Owner.BeginUpdate();
				IAnalysisInfo analysisInfo = GetAnalysisInfo();
				if(analysisInfo != null) {
					ITypeInfo objectTypeInfo = XafTypesInfo.Instance.FindTypeInfo(analysisInfo.DataType);
					foreach(string propertyName in analysisInfo.DimensionProperties) {
						IMemberInfo memberInfo = objectTypeInfo.FindMember(propertyName);
						if(memberInfo != null) {
							PivotGridFieldBase field = FindPivotGridField(GetBindingName(memberInfo));
							if(field != null) {
								SetupPivotGridField(field, memberInfo.MemberType, GetMemberDisplayFormat(memberInfo));
								field.Caption = CaptionHelper.GetFullMemberCaption(objectTypeInfo, propertyName);
							}
						}
					}
				}
			}
			finally {
				Owner.EndUpdate();
				OnSettingsApplied();
			}
		}
		public IAnalysisControl Owner {
			get { return owner; }
		}
		public event EventHandler<EventArgs> SettingsApplied;
		#region Obsolete 15.1
		[Obsolete("Use the SettingsApplied event instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public event EventHandler<EventArgs> SettingsApllied {
			add { SettingsApplied += value; }
			remove { SettingsApplied -= value; }
		}
		#endregion
	}
}
