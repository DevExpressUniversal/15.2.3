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
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using ResFinder = DevExpress.Printing.ResFinder;
#if !SILVERLIGHT
using System.Drawing.Design;
using DevExpress.Data;
using DevExpress.XtraReports.Native;
#endif
namespace DevExpress.XtraReports.Parameters {
#if !SILVERLIGHT
	[DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.Parameters.StaticListLookUpSettings")]
#endif
	public class StaticListLookUpSettings : LookUpSettings, IXtraSupportDeserializeCollectionItem {
		LookUpValueCollection lookUpValues = new LookUpValueCollection();
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, 0, XtraSerializationFlags.Cached),
#if !SILVERLIGHT
		Editor("DevExpress.XtraReports.Design.LookUpValuesEditor," + AssemblyInfo.SRAssemblyReportsExtensions, typeof(UITypeEditor)),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.Parameters.StaticListLookUpSettings.LookUpValues"),
		DXDisplayNameIgnore(IgnoreRecursionOnly=true)
#endif
		]
		public virtual LookUpValueCollection LookUpValues { get { return lookUpValues; } }
		protected internal override void SyncParameterType(Type type) {
			if(lookUpValues.Count != 0) {
				object value = lookUpValues[0].Value;
				if(value != null && value.GetType() != type)
					LookUpValues.Clear();
			}
		}
		#region IXtraSupportDeserializeCollectionItem Members
		object IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			if(propertyName == "LookUpValues")
				return new LookUpValue();
			return null;
		}
		void IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			if(propertyName == "LookUpValues")
				LookUpValues.Add((LookUpValue)e.Item.Value);
		}
		#endregion
		#region IDataContainer
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override object DataSource { get { return this.LookUpValues; } set { throw new NotSupportedException(); } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override object DataAdapter { get { return null; } set { throw new NotSupportedException(); } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string DataMember { get { return string.Empty; } set { throw new NotSupportedException(); } }
		#endregion
	}
}
