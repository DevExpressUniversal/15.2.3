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
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraMap.Native;
namespace DevExpress.XtraMap {
	public enum FieldValueType { Integer = 1, Decimal = 2, DateTime = 3, String = 4, Boolean = 5, Object = 6 };
	public class MapItemAttributeMappingCollection : MapItemMappingBaseCollection<MapItemAttributeMapping> {
		internal MapItemAttributeMappingCollection(LayerDataManager dataManager) : base(dataManager) {
		}
	}
	public class MapItemAttributeMapping : IOwnedElement, ILayerDataManagerProvider {
		object owner;
		string member = string.Empty;
		string name = string.Empty;
		FieldValueType valueType = FieldValueType.Object;
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemAttributeMappingName"),
#endif
		DefaultValue(""),
		NotifyParentProperty(true)]
		public string Name {
			get { return name; }
			set {
				if(name == value)
					return;
				name = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemAttributeMappingMember"),
#endif
		DefaultValue(""), NotifyParentProperty(true),
		TypeConverter("DevExpress.XtraMap.Design.MapColumnNameConverter," + "DevExpress.XtraMap" + AssemblyInfo.VSuffixDesign)]
		public string Member {
			get { return member; }
			set {
				if(member == value)
					return;
				member = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemAttributeMappingValueType"),
#endif
		DefaultValue(FieldValueType.Object), NotifyParentProperty(true)]
		public FieldValueType ValueType {
			get { return valueType; }
			set {
				if(ValueType == value)
					return;
				valueType = value;
				OnChanged();
			}
		}
#if !SL
	[DevExpressXtraMapLocalizedDescription("MapItemAttributeMappingType")]
#endif
		public Type Type { get { return GetType(ValueType); } }
		public MapItemAttributeMapping() {
		}
		public MapItemAttributeMapping(string name, string member) {
			this.name = name;
			this.member = member;
		}
		public MapItemAttributeMapping(string name, string member, FieldValueType valueType) {
			this.name = name;
			this.member = member;
			this.valueType = valueType;
		}
		#region IOwnedElement implementation
		object IOwnedElement.Owner {
			get { return owner; }
			set {
				if(owner == value)
					return;
				owner = value;
			}
		}
		#endregion
		protected virtual Type GetType(FieldValueType valueType) {
			return new UnboundColumnInfo(Name, (UnboundColumnType)valueType, false).DataType;
		}
		protected virtual void OnChanged() {
			LayerDataManager dataManager = owner as LayerDataManager;
			if(dataManager != null) {
				dataManager.OnMappingsChanged();
			}
		}
		#region ILayerDataManagerProvider Members
		LayerDataManager ILayerDataManagerProvider.DataManager {
			get { return owner as LayerDataManager; }
		}
		#endregion
	}
}
