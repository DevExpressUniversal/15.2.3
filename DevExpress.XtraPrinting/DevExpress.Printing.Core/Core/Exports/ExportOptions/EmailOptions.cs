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
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Localization;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using System.Collections.ObjectModel;
using System.Linq;
#if !SL
using System.Windows.Forms;
#endif
namespace DevExpress.XtraPrinting {
	[TypeConverter(typeof(LocalizableObjectConverter)),
	DXDisplayName(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.EmailOptions")]
	public class EmailOptions : IXtraSupportShouldSerialize, IXtraSupportDeserializeCollectionItem {
		const string defaultAddressPrefix = "SMTP:";
		string recipientName = string.Empty;
		string recipientAddress = string.Empty;
		string recipientAddressPrefix = defaultAddressPrefix;
		string subject = string.Empty;
		string body = string.Empty;
		RecipientCollection recipients = new RecipientCollection();
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("EmailOptionsRecipientName"),
#endif
		DXDisplayName(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.EmailOptions.RecipientName"),
		DefaultValue(""),
		XtraSerializableProperty,
		]
		public string RecipientName {
			get { return recipientName; }
			set { recipientName = value; }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("EmailOptionsRecipientAddress"),
#endif
		DXDisplayName(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.EmailOptions.RecipientAddress"),
		DefaultValue(""),
		XtraSerializableProperty,
		]
		public string RecipientAddress {
			get { return recipientAddress; }
			set { recipientAddress = value; }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("EmailOptionsRecipientAddressPrefix"),
#endif
		DXDisplayName(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.EmailOptions.RecipientAddressPrefix"),
		DefaultValue(defaultAddressPrefix),
		XtraSerializableProperty,
		]
		public string RecipientAddressPrefix {
			get { return recipientAddressPrefix; }
			set { recipientAddressPrefix = value; }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("EmailOptionsSubject"),
#endif
		DXDisplayName(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.EmailOptions.Subject"),
		DefaultValue(""),
		XtraSerializableProperty,
		]
		public string Subject {
			get { return subject; }
			set { subject = value; }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("EmailOptionsBody"),
#endif
		DXDisplayName(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.EmailOptions.Body"),
		DefaultValue(""),
		XtraSerializableProperty,
		]
		public string Body {
			get { return body; }
			set { body = value; }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("EmailOptionsAdditionalRecipients"),
#endif
		DXDisplayName(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.EmailOptions.AdditionalRecipients"),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, -1, XtraSerializationFlags.Cached),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		]
		public RecipientCollection AdditionalRecipients {
			get { return recipients; }
		}
		bool ShouldSerializeAdditionalRecipients() {
			return recipients.Count > 0;
		}
		public EmailOptions() {
		}
		public void Assign(EmailOptions source) {
			AdditionalRecipients.CopyFrom(source.AdditionalRecipients);
			recipientAddressPrefix = source.recipientAddressPrefix;
			subject = source.Subject;
			body = source.Body;
			recipientName = source.RecipientName;
			recipientAddress = source.RecipientAddress;
		}
		public void AddRecipient(Recipient recipient) {
			if(!recipients.Contains(recipient))
				recipients.Add(recipient);
		}
		public void InsertRecipient(int index, Recipient recipient) {
			if(!recipients.Contains(recipient))
				recipients.Insert(index, recipient);
		}
		internal void AddMainRecipient() {
			Recipient mainRecipient = new Recipient(recipientAddress, recipientName, recipientAddressPrefix, RecipientFieldType.TO);
			mainRecipient.CorrectData();
			InsertRecipient(0, mainRecipient);
		}
		internal void CorrectRecipientData() {
			foreach(Recipient recipient in recipients.ToList())
				if(recipient.EmptyNameAndAddress)
					recipients.Remove(recipient);
				else
					recipient.CorrectData();
		}
		internal bool ShouldSerialize() {
			return recipientName != "" || recipientAddress != "" || recipientAddressPrefix != defaultAddressPrefix || subject != "" || body != "" || ShouldSerializeAdditionalRecipients();
		}
		#region IXtraSupportDeserializeCollectionItem Members
		object IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			if(propertyName == "AdditionalRecipients")
				return new Recipient();
			return null;
		}
		void IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			if(propertyName == "AdditionalRecipients")
				this.AdditionalRecipients.Add((Recipient)e.Item.Value);
		}
		#endregion
		bool IXtraSupportShouldSerialize.ShouldSerialize(string propertyName) {
			switch(propertyName) {
				case "AdditionalRecipients":
					return ShouldSerializeAdditionalRecipients();
			}
			return true;
		}
	}
	[ToolboxItem(false)]
	[DesignTimeVisibleAttribute(false)]
	public class Recipient {
		#region Fields & Properties
		const RecipientFieldType defaultFieldType = RecipientFieldType.TO;
		const string defaultAddressPrefix = "SMTP:";
		string address, contactName;
		string prefix = defaultAddressPrefix;
		RecipientFieldType type = defaultFieldType;
		[
		DXDisplayName(typeof(DevExpress.Printing.ResFinder), "DevExpress.XtraReports.Recipients.Recipient.ContactName"),
		Category("Data"),
		XtraSerializableProperty,
		]
		public string ContactName {
			get { return contactName; }
			set { contactName = value; }
		}
		[
		DXDisplayName(typeof(DevExpress.Printing.ResFinder), "DevExpress.XtraReports.Recipients.Recipient.Address"),
		Category("Data"),
		XtraSerializableProperty,
		]
		public string Address {
			get { return address; }
			set { address = value; }
		}
		[
		DXDisplayName(typeof(DevExpress.Printing.ResFinder), "DevExpress.XtraReports.Recipients.Recipient.Prefix"),
		DefaultValue(defaultAddressPrefix),
		Category("Data"),
		XtraSerializableProperty,
		]
		public string Prefix {
			get { return prefix; }
			set { prefix = value; }
		}
		[
		DXDisplayName(typeof(DevExpress.Printing.ResFinder), "DevExpress.XtraReports.Recipients.Recipient.FieldType"),
		XtraSerializableProperty,
		Category("Data"),
		DefaultValue(defaultFieldType),
		]
		public RecipientFieldType FieldType {
			get { return type; }
			set { type = value; }
		}
		#endregion
		#region Constructors
		public Recipient()
			: this("") {
		}
		public Recipient(string address)
			: this(address, "") {
		}
		public Recipient(string address, RecipientFieldType type)
			: this(address, "", defaultAddressPrefix, type) {
		}
		public Recipient(string address, string recipientName)
			: this(address, recipientName, defaultAddressPrefix) {
		}
		public Recipient(string address, string recipientName, RecipientFieldType type)
			: this(address, recipientName, defaultAddressPrefix, type) {
		}
		public Recipient(string address, string recipientName, string prefix)
			: this(address, recipientName, prefix, RecipientFieldType.TO) {
		}
		public Recipient(string address, string recipientName, string prefix, RecipientFieldType type) {
			this.address = address;
			this.contactName = recipientName;
			this.prefix = prefix;
			this.type = type;
		}
		#endregion
		#region Methods
		public override string ToString() {
			return EmptyNameAndAddress ? "Recipient" : string.Format("{0}{1}", EmptyName ? "" : contactName + " ", EmptyAddress ? "" : "<" + address + ">");
		}
		internal void CorrectData() {
			if(!String.IsNullOrEmpty(address) && !HasRecipientPrefix)
				address = prefix + address;
			if(String.IsNullOrEmpty(contactName))
				contactName = HasRecipientPrefix ? address.Substring(prefix.Length) : address;
		}
		internal bool EmptyNameAndAddress { get { return EmptyName && EmptyAddress; } }
		bool EmptyName { get { return string.IsNullOrEmpty(contactName); } }
		bool EmptyAddress { get { return string.IsNullOrEmpty(address); } }
		bool HasRecipientPrefix {
			get { return address.IndexOf(prefix) == 0; }
		}
		#endregion
	}
	[ListBindable(BindableSupport.No),
	TypeConverter(typeof(DevExpress.Utils.Design.CollectionTypeConverter))]
	public class RecipientCollection : Collection<Recipient> {
		public RecipientCollection() {
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Recipient this[string recipientAddress] {
			get { return GetByAddress(recipientAddress); }
		}
		internal Recipient GetByAddress(string recipientAddress) {
			return Items.FirstOrDefault(recipient => recipient.Address == recipientAddress);
		}
		public void AddRange(Recipient[] items) {
			foreach(Recipient item in items)
				if(!item.EmptyNameAndAddress)
					this.Add(item);
		}
		internal void CopyFrom(RecipientCollection source) {
			this.Clear();
			this.AddRange(source.ToArray());
		}
		protected Recipient[] ToArray() {
			Recipient[] items = new Recipient[this.Count];
			this.CopyTo(items, 0);
			return items;
		}
	}
}
