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
using System.ComponentModel;
using System.Globalization;
using DevExpress.Utils.Serializing;
using DevExpress.WebUtils;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.Utils {
#if !SL && !DXPORTABLE
	[
	TypeConverter(typeof(DevExpress.Utils.Design.EnumTypeConverter)),
	DevExpress.Utils.Design.ResourceFinder(typeof(DevExpress.Data.ResFinder))
	]
#endif
	public enum FormatType { 
		None, 
		Numeric, 
		DateTime,
		Custom
	}
	public interface IComponentLoading {
		bool IsLoading { get; }
	}
#if !SL
	[TypeConverter(typeof(ExpandableObjectConverter))]
#endif
	public class FormatInfo : ViewStatePersisterCore, IXtraSerializable {
		static bool alwaysUseThreadFormat = false;
#if !SL
	[DevExpressDataLocalizedDescription("FormatInfoAlwaysUseThreadFormat")]
#endif
		public static bool AlwaysUseThreadFormat { get { return alwaysUseThreadFormat; } set { alwaysUseThreadFormat = value; } }
		static FormatInfo fEmpty;
		public event EventHandler Changed;
		int fLockParse;
		IComponentLoading componentLoading;
		bool shouldModifyFormatString;
		protected FormatType fFormatType;
		protected string fFormatString;
		IFormatProvider _format;
		bool deserializing;
		public FormatInfo(IComponentLoading componentLoading, IViewBagOwner bagOwner, string objectPath) : base(bagOwner, objectPath) {
			this.componentLoading = componentLoading;
			this.fLockParse = 0;
			Reset();
		}
		public FormatInfo(IViewBagOwner bagOwner, string objectPath) : this(null, bagOwner, objectPath) { }
		public FormatInfo(IComponentLoading componentLoading) : this(componentLoading, null, string.Empty) {}
		public FormatInfo() : this(null) {}
		static FormatInfo() {
			fEmpty = new FormatInfo();
		}
		[Browsable(false)]
		public virtual bool IsEmpty { get { return this.IsEquals(Empty); } }
#if !SL
	[DevExpressDataLocalizedDescription("FormatInfoEmpty")]
#endif
		public static FormatInfo Empty { get { return fEmpty; } }
		public virtual bool ShouldSerialize() { return !IsEmpty; }
		public virtual void LockParse() {
			++ fLockParse;
		}
		public virtual void UnlockParse() {
			-- fLockParse;
		}
		protected bool IsDeserializing { get { return deserializing; } }
		protected virtual bool IsLoading { get { return fLockParse != 0 || IsComponentLoading || IsDeserializing; } }
		protected IComponentLoading ComponentLoading { get { return componentLoading; } }
		protected bool IsComponentLoading { get { return ComponentLoading != null ? ComponentLoading.IsLoading : false; } }
		public virtual void Reset() {
			this._format = null;
			this.fFormatType = FormatType.None;
			this.fFormatString = "";
			this.shouldModifyFormatString = true;
			OnChanged();
		}
		protected void CheckFormatString() {
			this.shouldModifyFormatString = (FormatString.IndexOf('{') == -1);
		}
		public virtual bool IsEquals(FormatInfo info) {
			if(info == null) return false;
			return info.Format == this.Format &&
				this.FormatType == info.FormatType && info.FormatString == this.FormatString;
		}
		[Browsable(false), DefaultValue(null), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), NotifyParentProperty(true)]
		public virtual IFormatProvider Format { 
			get { 
				if(AlwaysUseThreadFormat) {
					switch(FormatType) {
						case FormatType.DateTime : return DateTimeFormatInfo.CurrentInfo;
						case FormatType.Numeric : return NumberFormatInfo.CurrentInfo;
					}
				}
				return _format; 
			}
			set {
				if(Format == value) return;
				_format = value;
				OnChanged();
			}
		}
		protected virtual void ResetFormatString() { FormatString = ""; }
		protected virtual bool ShouldSerializeFormatString() { return FormatString != ""; }
		[
#if !SL
	DevExpressDataLocalizedDescription("FormatInfoFormatString"),
#endif
#if !SL
		DXDisplayName(typeof(DevExpress.Data.ResFinder), "DevExpress.Utils.FormatInfo.FormatString"),
#endif
		NotifyParentProperty(true), XtraSerializableProperty()
		]
		public virtual string FormatString { 
			get { return (string)GetViewBagProperty("FormatString", fFormatString); }
			set {
				if(value == null) value = "";
				if(FormatString == value) return;
				if(!IsLoading) {
					TestFormatString(value);
				}
				fFormatString = value;
				SetViewBagProperty("FormatString", "", fFormatString);
				CheckFormatString();
				OnChanged();
			}
		}
		protected virtual void ResetFormatType() { FormatType = Utils.FormatType.None; }
		protected virtual bool ShouldSerializeFormatType() { return FormatType != Utils.FormatType.None; }
		[
#if !SL
	DevExpressDataLocalizedDescription("FormatInfoFormatType"),
#endif
#if !SL
		DXDisplayName(typeof(DevExpress.Data.ResFinder), "DevExpress.Utils.FormatInfo.FormatType"),
#endif
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public virtual FormatType FormatType {
			get { return (FormatType)GetViewBagProperty("FormatType", fFormatType); }
			set {
				if(FormatType == value) return;
				fFormatType = value;
				SetViewBagProperty("FormatType", FormatType.None, fFormatType);
				Parse();
				OnChanged();
			}
		}
		public virtual void Assign(FormatInfo info) {
			this._format = info.Format;
			this.fFormatString = info.FormatString;
			this.fFormatType = info.FormatType;
			this.shouldModifyFormatString = info.shouldModifyFormatString;
			OnChanged();
		}
		public virtual void Parse() {
			switch(FormatType) {
				case FormatType.DateTime :
					this._format = DateTimeFormatInfo.CurrentInfo;
					if(!IsLoading) this.fFormatString = "d";
					break;
				case FormatType.Numeric:
					this._format = NumberFormatInfo.CurrentInfo;
					if(!IsLoading) this.fFormatString = "";
					break;
				case FormatType.Custom:
					break;
				default:
					this._format = null;
					this.fFormatString = "";
					break;
			}
			if(!IsLoading) {
				try {
					TestFormatString(FormatString);
				}
				catch {
					this.fFormatString = "";
				}
			}
			CheckFormatString();
		}
		protected virtual void TestFormatString(string format) {
			if(Format == null || FormatType == FormatType.Custom) return;
			object testObject = 1;
			if(Format is DateTimeFormatInfo) {
				if(format == "d")
					return;
				testObject = DateTime.Now;
			}
			IFormattable frm = testObject as IFormattable;
			if(frm == null) return;
			try {
				frm.ToString(format, Format);
			}
			catch(Exception e) {
#if !SL && !DXPORTABLE
				throw new WarningException(e.Message);
#else
				throw new Exception(e.Message);
#endif
			}
		}
		public virtual string GetDisplayText(object val) {
			return GetDisplayTextCore(val, Format, FormatString);
		}
		protected string GetFormatString(string formatString) {
			if(shouldModifyFormatString) {
				if(string.IsNullOrEmpty(formatString))
					formatString = "{0}";
				else
					formatString = "{0:" + FormatString + "}";
			}
			return formatString;
		}
		public string GetFormatString() { return GetFormatString(FormatString); }
		protected virtual string GetDisplayTextCore(object val, IFormatProvider format, string formatString) {
			if(val == null)
				return string.Empty;
			formatString = GetFormatString(formatString);
			if(format != null) {
				try {
					return string.Format(format, formatString, val);
				} catch {
					return val.ToString();
				}
			} else {
				if(fFormatType == FormatType.Custom)
					try {
						return string.Format(formatString, val);
					}
					catch {
						return GetStringValue(val);
					}
				else
					return GetStringValue(val);
			}
		}
		string GetStringValue(object val) {
			try {
				return val.ToString();
			}
			catch {
				return String.Empty;
			}
		}
		protected virtual void OnChanged() {
			SetViewBagProperty("FormatType", FormatType.None, fFormatType);
			SetViewBagProperty("FormatString", string.Empty, fFormatString);
			if(Changed != null) Changed(this, EventArgs.Empty);
		}
		#region IXtraSerializable Members		
		public void OnStartSerializing() {
		}
		public void OnEndSerializing() {
		}
		public void OnStartDeserializing(LayoutAllowEventArgs e) {
			deserializing = true;
		}
		public void OnEndDeserializing(string restoredVersion) {
			deserializing = false;
		}
		#endregion
		public override string ToString() {
			string result = string.Empty;
			if(FormatType != FormatType.None)
				result = FormatType.ToString();
			if(!string.IsNullOrEmpty(FormatString)) 
				result += ((string.IsNullOrEmpty(result) ? string.Empty : " ") + "\"" + FormatString + "\"");
			return result;
		}
	}
}
