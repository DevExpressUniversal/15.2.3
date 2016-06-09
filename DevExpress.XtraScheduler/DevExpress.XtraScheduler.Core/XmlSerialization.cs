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
using System.Globalization;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Drawing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils;
using System.Collections.Generic;
#if SL
using System.Windows.Media;
using System.Windows.Controls;
#endif
namespace DevExpress.XtraScheduler {
	[Obsolete("You should directly use the appropriate classes from the DevExpress.XtraScheduler.Xml namespace (e.g. AppointmentXmlPersistenceHelper).", false), EditorBrowsable(EditorBrowsableState.Never)]
	public interface IXmlPersistable {
		string ToXml();
		void FromXml(string val);
	}
}
namespace DevExpress.XtraScheduler.Xml {
	using DevExpress.Utils.Serializing;
	#region PersistentObjectXmlLoader (abstract class)
	public abstract class PersistentObjectXmlLoader : ObjectXmlLoader {
		protected PersistentObjectXmlLoader(XmlNode root)
			: base(root) {
		}
		protected virtual void CustomFieldsFromXml(IPersistentObject obj, MappingCollection mappings) {
			obj.BeginUpdate();
			try {
				int count = obj.CustomFields.Count;
				for (int i = 0; i < count; i++) {
					CustomField field = obj.CustomFields.GetFieldByIndex(i);
					MappingBase mapping = mappings[field.Name];
					field.Value = ReadAttributeValue(mapping.Member, mapping.Type);
				}
			} finally {
				obj.CancelUpdate();
			}
		}
	}
	#endregion
	public abstract class SchedulerXmlPersistenceHelper : XmlPersistenceHelper {
		protected internal virtual void AddCustomFieldsToContext(XmlContext context, CustomFieldCollection customFields, MappingCollection mappings) {
			if (customFields == null)
				return;
			foreach (CustomField field in customFields) {
				MappingBase mapping = mappings[field.Name];
				context.Attributes.Add(new ObjectContextAttribute(mapping.Member, field.Value, null));
			}
		}
	}
	public class SchedulerObjectConverterImplementation : ObjectConverterImplementation {
		public string ObjectToBase64String(object val) {
			return SerialzeWithBinaryFormatter(val);
		}
	}
	public class SchedulerObjectContextAttribute : ObjectContextAttribute {
		public SchedulerObjectContextAttribute(string name, object val, object defaultValue)
			: base(name, val, defaultValue) {
		}
		public override string ValueToString() {
			return SchedulerCompatibility.Base64XmlObjectSerialization ? new SchedulerObjectConverterImplementation().ObjectToBase64String(Value) : base.ValueToString();
		}
	}
}
