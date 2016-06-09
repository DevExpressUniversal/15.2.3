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
using DevExpress.XtraPivotGrid;
using System.Xml;
using System.Linq;
using DevExpress.PivotGrid.QueryMode;
namespace DevExpress.PivotGrid.OLAP {
	class XmlMemberReaderStorage {
		AxisColumnsProviderBase axisColumnsProvider;
		Dictionary<string, XmlMemberReaderBase> cached = new Dictionary<string, XmlMemberReaderBase>();
		public XmlMemberReaderStorage(AxisColumnsProviderBase axisColumnsProvider) {
			this.axisColumnsProvider = axisColumnsProvider;
		}
		public XmlMemberReaderBase Get(string hierarchyName, XmlReader reader, bool isColumn) {
			XmlMemberReaderBase readerBase;
			if(cached.TryGetValue(hierarchyName, out readerBase))
				return readerBase;
			readerBase = GetMemberReader(hierarchyName, reader, isColumn);
			cached.Add(hierarchyName, readerBase);
			return readerBase;
		}
		XmlMemberReaderBase GetMemberReader(string hierarchyName, XmlReader reader, bool isColumn) {
			if(hierarchyName == OLAPDataSourceQueryBase.MeasuresStringName) {
				return new MeasuresMemberReader(axisColumnsProvider, reader);
			}
			OLAPMetadataColumn column = axisColumnsProvider.GetMetadataByHierarchy(hierarchyName);
			if(column != null) {
				if(column.ParentColumn == null && column.ChildColumn == null)
					return new XmlColumnMemberReader(reader, column, axisColumnsProvider.GetColumn(column.UniqueName));
				else {
					OLAPCubeColumn cubeColumn = axisColumnsProvider.GetColumn(column.UniqueName);
					return new XmlHierarchyMemberReader(reader, column.GetColumnHierarchy().Cast<OLAPMetadataColumn>().ToList(), cubeColumn == null ? null : cubeColumn.GetColumnHierarchy());
				}
			} else
				throw new ArgumentException("unknown hirarchy" + hierarchyName);
		}
	}
	abstract class XmlMemberReaderBase : XmlReaderParserBase {
		public XmlMemberReaderBase(XmlReader reader)
			: base(reader) {
		}
		public abstract OLAPMember Read();
	}
	class MeasuresMemberReader : XmlMemberReaderBase {
		readonly Dictionary<string, OLAPMember> members = new Dictionary<string, OLAPMember>();
		readonly AxisColumnsProviderBase axisColumnsProvider;
		OLAPMember member;
		public MeasuresMemberReader(AxisColumnsProviderBase axisColumnsProvider, XmlReader reader)
			: base(reader) {
			this.axisColumnsProvider = axisColumnsProvider;
			foreach(OLAPCubeColumn column in axisColumnsProvider.EnumerateMeasures()) {
				if(!column.IsMeasure)
					continue;
				members.Add(column.UniqueName, new OLAPMember(column.Metadata, column.UniqueName, null));
			}
		}
		public override OLAPMember Read() {
			while(reader.IsStartElement()) {
				if(reader.Name == OlapProperty.UName) {
					string mName = reader.ReadElementContentAsString();
					if(!members.TryGetValue(mName, out member)) {
						member = new OLAPMember(axisColumnsProvider.ResolveMeasure(mName), mName, null);
						members.Add(mName, member);
					}
					while(reader.IsStartElement())
						reader.Skip();
					return member;
				} else
					reader.Skip();
			}
			return member;
		}
	}
	class XmlMemberReader : XmlMemberReaderBase, IOLAPMemberSource {
		protected bool valueReaded = false;
		protected bool captionReaded = false;
		string value;
		string caption;
		protected string uName;
		protected OLAPMember member = null;
		public XmlMemberReader(XmlReader reader)
			: base(reader) {
		}
		public override OLAPMember Read() {
			member = null;
			captionReaded = false;
			valueReaded = false;
			return member;
		}
		protected void ReadMemberValue() {
			if(HasNillAttribute()) {
				value = null;
				reader.Skip();
			} else
				if(reader.IsEmptyElement) {
					value = string.Empty;
					reader.Skip();
				} else {
					reader.ReadStartElement();
					value = reader.ReadContentAsString();
					reader.ReadEndElement();
				}
			valueReaded = true;
		}
		protected void ReadMemberCaption() {
			if(HasNillAttribute()) {
				caption = null;
				reader.Skip();
			} else
				if(reader.IsEmptyElement) {
					caption = string.Empty;
					reader.Skip();
				} else {
					reader.ReadStartElement();
					caption = reader.ReadContentAsString();
					reader.ReadEndElement();
				}
			captionReaded = true;
		}
		string IOLAPMemberSource.UniqueName {
			get { return uName; }
		}
		string IOLAPMemberSource.Caption {
			get { return caption; }
		}
		object IOLAPMemberSource.GetValue(Type dataType) {
			if(valueReaded && value != null)
				return XsdTypeConverter.ConvertTo(value, dataType);
			else
				return null;
		}
		protected virtual void MergeProperties(ref OLAPMemberProperties props) {
			if(props != null) {
				if(member.autoProperties == null)
					member.autoProperties = props;
				else {
					foreach(KeyValuePair<string, OLAPMemberProperty> pair in props.GetDictionaryEnumerator())
						member.autoProperties[pair.Key] = pair.Value;
					props = member.autoProperties;
				}
			} else
				props = member.autoProperties;
		}
	}
	class XmlHierarchyMemberReader : XmlMemberReader {
		private List<OLAPMetadataColumn> hierarchy;
		OLAPMemberProperties properties;
		Dictionary<string, string> stringProps = new Dictionary<string, string>();
		string lName;
		bool lNameReaded;
		bool uNameReaded;
		Dictionary<string, OLAPPropertyDescriptor> auto = new Dictionary<string, OLAPPropertyDescriptor>();
		Dictionary<string, OLAPPropertyDescriptor> stringAuto = new Dictionary<string, OLAPPropertyDescriptor>();
		Dictionary<string, OLAPMetadataColumn> columnCache = new Dictionary<string, OLAPMetadataColumn>();
		Dictionary<string, Dictionary<string, OLAPPropertyDescriptor>> stringByColumn = new Dictionary<string, Dictionary<string, OLAPPropertyDescriptor>>(); 
		public XmlHierarchyMemberReader(XmlReader reader, List<OLAPMetadataColumn> metaHierarchy, List<OLAPCubeColumn> olapColumns)
			: base(reader) {
			this.hierarchy = metaHierarchy;
			foreach(OLAPMetadataColumn meta in metaHierarchy)
				columnCache.Add(meta.UniqueName, meta);
			if(olapColumns != null)
				foreach(OLAPCubeColumn column in olapColumns) {
					Dictionary<string, OLAPPropertyDescriptor> strColumnProps = new Dictionary<string, OLAPPropertyDescriptor>();
					stringByColumn.Add(column.UniqueName, strColumnProps);
					foreach(OLAPPropertyDescriptor property in column.AutoProperties) {
						if(property.IsUserDefined)
							auto.Add(property.XmlName, property);
						else {
							stringAuto[property.XmlName] = property;
							strColumnProps.Add(property.XmlName, property);
						}
					}
				}
		}
		public override OLAPMember Read() {
			base.Read();
			lNameReaded = false;
			uNameReaded = false;
			properties = null;
			stringProps.Clear();
			ReadWaitingAny();
			return member;
		}
		void ReadWaitingAny() {
			while(reader.IsStartElement()) {
				string tag = reader.Name;
				switch(tag) {
					case OlapProperty.UName:
						uName = reader.ReadElementContentAsString();
						uNameReaded = true;
						if(TryCreateMember()) {
							ReadMemberNormal();
							return;
						}
						break;
					case OlapProperty.LName:
						lName = reader.ReadElementContentAsString();
						lNameReaded = true;
						if(TryCreateMember()) {
							ReadMemberNormal();
							return;
						}
						break;
					case OlapProperty.CaptionProperty:
						ReadMemberCaption();
						if(TryCreateMember()) {
							ReadMemberNormal();
							return;
						}
						break;
					case OlapProperty.MEMBERVALUE:
						ReadMemberValue();
						if(TryCreateMember()) {
							ReadMemberNormal();
							return;
						}
						break;
					default:
						ReadMemberProperty(tag);
						break;
				}
			}
			if(member == null) {
				OLAPMetadataColumn column;
				if(!columnCache.TryGetValue(lName, out column))
					column = hierarchy[0];
				member = column.CreateMemberWithoutProps(this);
				MergeProperties(ref properties);
			}
		}
		protected override void MergeProperties(ref OLAPMemberProperties props) {
			base.MergeProperties(ref props);
			if(stringProps.Count > 0)
				foreach(KeyValuePair<string, string> pair in stringProps) {
					OLAPPropertyDescriptor desc = stringAuto[pair.Key];
					if(!props.ContainsKey(desc.Name))
						props.Add(pair.Key, OLAPMemberProperty.Create(desc, reader.ReadElementContentAsString()));
					else
						reader.Skip();
				}
		}
		bool TryCreateMember() {
			if(!lNameReaded || !uNameReaded)
				return false;
			OLAPMetadataColumn column;
			if(!columnCache.TryGetValue(lName, out column))
				column = hierarchy[0];
			member = column[uName];
			if(member != null)
				return true;
			if(!captionReaded || !valueReaded)
				return false;
			member = column.CreateMemberWithoutProps(this);
			return true;
		}
		void ReadMemberNormal() {
			MergeProperties(ref properties);
			while(reader.IsStartElement()) {
				ReadMemberProperty(reader.Name);
			}
			if(member.autoProperties == null)
				member.autoProperties = properties;
		}
		void ReadMemberProperty(string tag) {
			if(reader.IsEmptyElement) {
				reader.Skip();
			} else {
				OLAPPropertyDescriptor desc;
				if(HasNillAttribute() || reader.IsEmptyElement) {
					reader.Skip();
				} else {
					if(!auto.TryGetValue(tag, out desc)) {
						if(!stringAuto.TryGetValue(tag, out desc)) {
							reader.Skip();
						} else {
							if(member == null)
								stringProps.Add(tag, reader.ReadElementContentAsString());
							else {
								desc = stringByColumn[member.Column.UniqueName][tag];
								if(properties == null)
									properties = new OLAPMemberProperties();
								if(!properties.ContainsKey(desc.Name))
									properties.Add(desc.Name, OLAPMemberProperty.Create(desc, reader.ReadElementContentAsString()));
								else
									reader.Skip();
							}
						}
					} else {
						if(properties == null)
							properties = new OLAPMemberProperties();
						if(!properties.ContainsKey(desc.Name))
							properties.Add(desc.Name, OLAPMemberProperty.Create(desc, reader.ReadElementContentAsString()));
						else
							reader.Skip();
					}
				}
			}
		}
	}
	class XmlColumnMemberReader : XmlMemberReader {
		Dictionary<string, OLAPPropertyDescriptor> auto = new Dictionary<string, OLAPPropertyDescriptor>();
		OLAPMetadataColumn column;
		OLAPMemberProperties properties;
		public XmlColumnMemberReader(XmlReader reader, OLAPMetadataColumn metadataColumn, OLAPCubeColumn olapColumn)
			: base(reader) {
			if(olapColumn != null)
				foreach(OLAPPropertyDescriptor desc in olapColumn.AutoProperties)
					auto.Add(desc.XmlName, desc);
			this.column = metadataColumn;
		}
		public override OLAPMember Read() {
			base.Read();
			properties = null;
			ReadWaitingAny();
			return member;
		}
		void ReadWaitingAny() {
			while(reader.IsStartElement()) {
				string tag = reader.Name;
				switch(tag) {
					case OlapProperty.UName:
						uName = reader.ReadElementContentAsString();
						UNameFounded();
						return;
					case OlapProperty.CaptionProperty:
						ReadMemberCaption();
						break;
					case OlapProperty.MEMBERVALUE:
						ReadMemberValue();
						break;
					default:
						ReadMemberProperty(tag);
						break;
				}
			}
		}
		void UNameFounded() {
			member = column[uName];
			if(member != null || TryCreateMember()) {
				ReadMemberNormal();
				return;
			} else {
				while(reader.IsStartElement()) {
					string tag = reader.Name;
					switch(tag) {
						case OlapProperty.CaptionProperty:
							ReadMemberCaption();
							if(TryCreateMember()) {
								ReadMemberNormal();
								return;
							}
							break;
						case OlapProperty.MEMBERVALUE:
							ReadMemberValue();
							if(TryCreateMember()) {
								ReadMemberNormal();
								return;
							}
							break;
						default:
							ReadMemberProperty(tag);
							break;
					}
				}
				if(member == null)
					member = column.CreateMemberWithoutProps(this);
				MergeProperties(ref properties);
			}
		}
		void ReadMemberNormal() {
			MergeProperties(ref properties);
			while(reader.IsStartElement()) {
				ReadMemberProperty(reader.Name);
			}
			if(member.autoProperties == null)
				member.autoProperties = properties;
		}
		void ReadMemberProperty(string tag) {
			if(reader.IsEmptyElement) {
				reader.Skip();
			} else {
				OLAPPropertyDescriptor desc;
				if(!auto.TryGetValue(tag, out desc) || HasNillAttribute() || reader.IsEmptyElement)
					reader.Skip();
				else {
					if(properties == null)
						properties = new OLAPMemberProperties();
					if(!properties.ContainsKey(desc.Name))
						properties.Add(desc.Name, OLAPMemberProperty.Create(desc, reader.ReadElementContentAsString()));
					else
						reader.Skip();
				}
			}
		}
		bool TryCreateMember() {
			if(!captionReaded || !valueReaded)
				return false;
			member = column.CreateMemberWithoutProps(this);
			return true;
		}
	}
}
