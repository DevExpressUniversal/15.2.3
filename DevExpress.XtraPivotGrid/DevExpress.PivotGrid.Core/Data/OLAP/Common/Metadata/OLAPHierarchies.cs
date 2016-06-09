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

using System.Collections.Generic;
using System.IO;
using DevExpress.XtraPivotGrid;
namespace DevExpress.PivotGrid.OLAP {
	public class OLAPHierarchies : Dictionary<string, OLAPHierarchy> {
		public void Add(OLAPHierarchy hierarchy) {
			if(ContainsKey(hierarchy.UniqueName)) return;
			Add(hierarchy.UniqueName, hierarchy);
		}
		public void SaveToStream(BinaryWriter writer) {
			writer.Write(Count);
			foreach(OLAPHierarchy hierarchy in Values)
				hierarchy.SaveToStream(writer);
		}
		public void RestoreFromStream(BinaryReader reader) {
			int hierarchiesCount = reader.ReadInt32();
			for(int i = 0; i < hierarchiesCount; i++) {
				OLAPHierarchy hierarchy = new OLAPHierarchy();
				hierarchy.RestoreFromStream(reader);
				Add(hierarchy);
			}
		}
	}
	public class OLAPHierarchy {
		public const string MeasuresHierarchyUniqueName = "[Measures]";
		public const string KPIsHierarchyUniqueName = "[KPIs]";
		public static bool GetIsMeasure(PivotGridFieldBase field) {
			return GetIsMeasure(field.FieldName);
		}
		static bool GetIsMeasure(string fieldName) {
			return fieldName.StartsWith(MeasuresHierarchyUniqueName);
		}
		string uniqueName, name, caption, dimension, displayFolder;
		short structure;
		short origin = 0;
		public string UniqueName { get { return uniqueName; } }
		public bool IsMeasure { get { return GetIsMeasure(UniqueName); } }
		public string Name { get { return name; } }
		public string Caption { get { return caption; } internal set { caption = value; } }
		public short Structure { get { return structure; } }
		public short Origin { get { return origin; } }
		public string Dimension {
			get {
				if(string.IsNullOrEmpty(dimension)) {
					string[] parts = uniqueName.Split('.');
					dimension = parts[0];
				}
				return dimension;
			}
		}
		public string DisplayFolder { get { return displayFolder; } }
		public bool IsUserDefined { get { return (origin & 0x1) == 1; } }
		public OLAPHierarchy() { }
		public OLAPHierarchy(string uniqueName, string name) : this(uniqueName, name, string.Empty, string.Empty) { }
		public OLAPHierarchy(string uniqueName, string name, string caption) : this(uniqueName, name, caption, string.Empty) { }
		public OLAPHierarchy(string uniqueName, string name, string caption, string displayFolder, short structure, short origin) 
			: this(uniqueName, name, caption, displayFolder) {
				this.structure = structure;
				this.origin = origin;
		}
		public OLAPHierarchy(string uniqueName, string name, string caption, string displayFolder) {
			this.uniqueName = uniqueName;
			this.name = name;
			this.caption = caption ?? string.Empty;
			this.displayFolder = displayFolder ?? string.Empty;
		}
		public virtual void SaveToStream(BinaryWriter writer) {
			writer.Write(uniqueName);
			writer.Write(name);
			writer.Write(caption);
			writer.Write(displayFolder);
			writer.Write(structure);
			writer.Write(origin);
		}
		public virtual void RestoreFromStream(BinaryReader reader) {
			uniqueName = reader.ReadString();
			name = reader.ReadString();
			caption = reader.ReadString();
			displayFolder = reader.ReadString();
			structure = reader.ReadInt16();
			origin = reader.ReadInt16();
		}
		public override string ToString() {
			return UniqueName;
		}
		public override bool Equals(object obj) {
			OLAPHierarchy b = obj as OLAPHierarchy;
			if(b != null) 
				return UniqueName == b.uniqueName && Caption == b.Caption && Name == b.Name;
			return base.Equals(obj);
		}
		public override int GetHashCode() {
			return !string.IsNullOrEmpty(UniqueName) ? UniqueName.GetHashCode() : 0;
		}
	}
}
