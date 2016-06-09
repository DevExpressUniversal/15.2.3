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
using System.Drawing;
using System.IO;
using System.Xml;
using DevExpress.Office.Utils;
using System.Text;
using DevExpress.Compatibility.System;
using DevExpress.Compatibility.System.Drawing;
using System.Diagnostics;
namespace DevExpress.Office.Crypto.Agile {
	class HmacData {
		public byte[] EncryptedKey { get; set; }
		public byte[] EncryptedValue { get; set; }
		public HmacData Clone() {
			HmacData copy = new HmacData();
			copy.EncryptedKey = (byte[])this.EncryptedKey.Clone();
			copy.EncryptedValue = (byte[])this.EncryptedValue.Clone();
			return copy;
		}
	}
	class PasswordKeyEncryptorData {
		static string rootUri = "http://schemas.microsoft.com/office/2006/keyEncryptor/password";
		public static string RootUri { get { return rootUri; } }
		public byte[] EncryptedHashInput { get; set; }
		public byte[] EncryptedHashValue { get; set; }
		public byte[] EncryptedKeyValue { get; set; }
		public byte[] SaltValue { get; set; }
		public byte[] SecretKey { get; set; }
		internal static PasswordKeyEncryptorData LoadFromXml(XmlReader reader, out PasswordKeyEncryptorInfo configOut) {
			PasswordKeyEncryptorData data = new PasswordKeyEncryptorData();
			PasswordKeyEncryptorInfo config = new PasswordKeyEncryptorInfo();
			config.CipherInfo = new CipherInfo();
			config.HashInfo = new HashInfo();
			reader.MoveToStartElement(rootUri, "encryptedKey");
			reader.ParseAttributes(12, (NameToken nameToken, String value) => {
				switch (nameToken) {
					case NameToken.spinCount:
						config.SpinCount = Int32.Parse(value);
						return true;
					case NameToken.saltSize:
						config.SaltSize = Int32.Parse(value);
						return true;
					case NameToken.blockSize:
						config.CipherInfo.BlockBits = Int32.Parse(value) * 8;
						return true;
					case NameToken.keyBits:
						config.CipherInfo.KeyBits = Int32.Parse(value);
						return true;
					case NameToken.hashSize:
						config.HashInfo.HashBits = Int32.Parse(value) * 8;
						return true;
					case NameToken.cipherAlgorithm:
						config.CipherInfo.Name = value;
						return true;
					case NameToken.cipherChaining:
						config.CipherInfo.Mode = value;
						return true;
					case NameToken.hashAlgorithm:
						config.HashInfo.Name = value;
						return true;
					case NameToken.saltValue:
						data.SaltValue = Convert.FromBase64String(value);
						return true;
					case NameToken.encryptedVerifierHashInput:
						data.EncryptedHashInput = Convert.FromBase64String(value);
						return true;
					case NameToken.encryptedVerifierHashValue:
						data.EncryptedHashValue = Convert.FromBase64String(value);
						return true;
					case NameToken.encryptedKeyValue:
						data.EncryptedKeyValue = Convert.FromBase64String(value);
						return true;
				}
				return false;
			});
			if (config.SaltSize != data.SaltValue.Length)
				throw new InvalidDataException("Invalid salt size/data found");
			if (!reader.IsEmptyElement)
				reader.MoveToNextContent();
			reader.MoveToNextContent();
			configOut = config;
			return data;
		}
		public static void WriteToXml(XmlWriter writer, PasswordKeyEncryptorInfo config, PasswordKeyEncryptorData data) {
			writer.WriteStartElement("encryptedKey", rootUri);
			writer.WriteAttributeString(NameToken.spinCount.ToString(), config.SpinCount.ToString());
			writer.WriteAttributeString(NameToken.saltSize.ToString(), config.SaltSize.ToString());
			writer.WriteAttributeString(NameToken.blockSize.ToString(), (config.CipherInfo.BlockBits / 8).ToString());
			writer.WriteAttributeString(NameToken.keyBits.ToString(), config.CipherInfo.KeyBits.ToString());
			writer.WriteAttributeString(NameToken.cipherAlgorithm.ToString(), config.CipherInfo.Name);
			writer.WriteAttributeString(NameToken.cipherChaining.ToString(), config.CipherInfo.Mode);
			writer.WriteAttributeString(NameToken.hashAlgorithm.ToString(), config.HashInfo.Name);
			writer.WriteAttributeString(NameToken.hashSize.ToString(), (config.HashInfo.HashBits / 8).ToString());
			writer.WriteAttributeString(NameToken.saltValue.ToString(), Convert.ToBase64String(data.SaltValue));
			writer.WriteAttributeString(NameToken.encryptedVerifierHashInput.ToString(), Convert.ToBase64String(data.EncryptedHashInput));
			writer.WriteAttributeString(NameToken.encryptedVerifierHashValue.ToString(), Convert.ToBase64String(data.EncryptedHashValue));
			writer.WriteAttributeString(NameToken.encryptedKeyValue.ToString(), Convert.ToBase64String(data.EncryptedKeyValue));
			writer.WriteEndElement();
		}
	}
	class EncryptionData {
		static string rootUri = "http://schemas.microsoft.com/office/2006/encryption";
		public byte[] SaltValue { get; set; }
		public HmacData HmacData { get; set; }
		public PasswordKeyEncryptorData PasswordEncryptorData { get; set; }
		public static EncryptionData LoadFromStream(Stream stream, out EncryptionInfo config) {
			return LoadFromStream(stream, true , out config);
		}
		public static EncryptionData LoadFromStream(Stream stream, bool useHmac, out EncryptionInfo config) {
			BinaryReader reader = new BinaryReader(stream);
			int flag = reader.ReadInt16();
			if (flag != 4)
				throw new InvalidOperationException("Non-agile stream");
			flag = reader.ReadInt16();
			if (flag != 4)
				throw new InvalidOperationException("Non-agile stream");
			flag = reader.ReadInt32();
			Debug.Assert(flag == 0x40);
			using (XmlReader xmlReader = XmlReader.Create(stream)) {
				return LoadFromXml(xmlReader, useHmac, out config);
			}
		}
		static EncryptionData LoadFromXml(XmlReader reader, bool useHmac, out EncryptionInfo configOut) {
			EncryptionInfo config = new EncryptionInfo();
			config.CipherInfo = new CipherInfo();
			config.HashInfo = new HashInfo();
			EncryptionData data = new EncryptionData();
			data.HmacData = new HmacData();
			reader.MoveToStartElement(rootUri, "encryption");
			reader.ParseAttributes(0, (NameToken nameToken, String value) => { return false; });
			reader.MoveToStartElement(rootUri, "keyData");
			reader.ParseAttributes(8, (NameToken nameToken, String value) => {
				switch (nameToken) {
					case NameToken.blockSize:
						config.CipherInfo.BlockBits = Int32.Parse(value) * 8;
						return true;
					case NameToken.keyBits:
						config.CipherInfo.KeyBits = Int32.Parse(value);
						return true;
					case NameToken.hashSize:
						config.HashInfo.HashBits = Int32.Parse(value) * 8;
						return true;
					case NameToken.cipherAlgorithm:
						config.CipherInfo.Name = value;
						return true;
					case NameToken.cipherChaining:
						config.CipherInfo.Mode = value;
						return true;
					case NameToken.hashAlgorithm:
						config.HashInfo.Name = value;
						return true;
					case NameToken.saltSize:
						config.SaltSize = Int32.Parse(value);
						return true;
					case NameToken.saltValue:
						data.SaltValue = Convert.FromBase64String(value);
						return true;
				}
				return false;
			});
			if (config.SaltSize != data.SaltValue.Length)
				throw new InvalidDataException("Invalid salt size/data found");
			reader.MoveToEndElement();
			if (useHmac) {
				reader.MoveToStartElement(rootUri, "dataIntegrity");
				reader.ParseAttributes(2, (NameToken nameToken, String value) => {
					switch (nameToken) {
						case NameToken.encryptedHmacKey:
							data.HmacData.EncryptedKey = Convert.FromBase64String(value);
							return true;
						case NameToken.encryptedHmacValue:
							data.HmacData.EncryptedValue = Convert.FromBase64String(value);
							return true;
					}
					return false;
				});
				reader.MoveToEndElement();
			}
			reader.MoveToStartElement(rootUri, "keyEncryptors");
			reader.ParseAttributes(0, (NameToken nameToken, String value) => { return false; });
			reader.MoveToStartElement(rootUri, "keyEncryptor");
			while (true) {
				string encryptorUri = String.Empty;
				reader.ParseAttributes(1, (NameToken nameToken, String value) => {
					if (nameToken == NameToken.uri) {
						encryptorUri = value;
						return true;
					}
					return false;
				});
				if (encryptorUri.Length == 0)
					throw new InvalidDataException("Found empty Uri");
				if (encryptorUri == PasswordKeyEncryptorData.RootUri) {
					PasswordKeyEncryptorInfo passwordKeyEncryptorConfig;
					data.PasswordEncryptorData = PasswordKeyEncryptorData.LoadFromXml(reader, out passwordKeyEncryptorConfig);
					config.PasswordKeyEncryptorInfo = passwordKeyEncryptorConfig;
				}
				else {
					string xmlNamespaceUri = reader.NamespaceURI;
					while (true) {
						reader.MoveToNextContent();
						if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "keyEncryptor" && reader.NamespaceURI == xmlNamespaceUri)
							break;
					}
				}
				Debug.Assert(reader.NodeType == XmlNodeType.EndElement);
				Debug.Assert(reader.Name == "keyEncryptor");
				reader.MoveToNextContent();
				if (reader.NodeType == XmlNodeType.EndElement)
					break;
			}
			Debug.Assert(reader.NodeType == XmlNodeType.EndElement);
			Debug.Assert(reader.Name == "keyEncryptors");
			reader.MoveToNextContent();
			Debug.Assert(reader.NodeType == XmlNodeType.EndElement);
			Debug.Assert(reader.Name == "encryption");
			configOut = config;
			return data;
		}
		public static void WriteToStream(Stream stream, EncryptionInfo config, EncryptionData data) {
			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write((short)4);
			writer.Write((short)4);
			writer.Write((int)0x40);
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Encoding = Encoding.UTF8;
			using (XmlWriter xmlWriter = XmlWriter.Create(stream, settings)) {
				WriteToXml(xmlWriter, config, data);
			}
		}
		public static void WriteToXml(XmlWriter writer, EncryptionInfo config, EncryptionData data) {
			writer.WriteStartDocument();
			writer.WriteStartElement("encryption", rootUri);
			writer.WriteStartElement("keyData", rootUri);
			writer.WriteAttributeString(NameToken.blockSize.ToString(), (config.CipherInfo.BlockBits / 8).ToString());
			writer.WriteAttributeString(NameToken.saltSize.ToString(), config.SaltSize.ToString());
			writer.WriteAttributeString(NameToken.keyBits.ToString(), config.CipherInfo.KeyBits.ToString());
			writer.WriteAttributeString(NameToken.hashSize.ToString(), (config.HashInfo.HashBits / 8).ToString());
			writer.WriteAttributeString(NameToken.cipherAlgorithm.ToString(), config.CipherInfo.Name);
			writer.WriteAttributeString(NameToken.cipherChaining.ToString(), config.CipherInfo.Mode);
			writer.WriteAttributeString(NameToken.hashAlgorithm.ToString(), config.HashInfo.Name);
			writer.WriteAttributeString(NameToken.saltValue.ToString(), Convert.ToBase64String(data.SaltValue));
			writer.WriteEndElement();
			if (data.HmacData != null) {
				writer.WriteStartElement("dataIntegrity", rootUri);
				writer.WriteAttributeString(NameToken.encryptedHmacKey.ToString(), Convert.ToBase64String(data.HmacData.EncryptedKey));
				writer.WriteAttributeString(NameToken.encryptedHmacValue.ToString(), Convert.ToBase64String(data.HmacData.EncryptedValue));
				writer.WriteEndElement();
			}
			writer.WriteStartElement("keyEncryptors", rootUri);
			{
				writer.WriteStartElement("keyEncryptor", rootUri);
				writer.WriteAttributeString(NameToken.uri.ToString(), PasswordKeyEncryptorData.RootUri);
				{
					PasswordKeyEncryptorData.WriteToXml(writer, config.PasswordKeyEncryptorInfo, data.PasswordEncryptorData);
				}
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			writer.WriteEndElement();
			writer.WriteEndDocument();
			writer.Flush();
		}
	}
	enum NameToken {
		blockSize,
		keyBits,
		hashSize,
		saltSize,
		saltValue,
		cipherAlgorithm,
		cipherChaining,
		hashAlgorithm,
		encryptedHmacKey,
		encryptedHmacValue,
		uri,
		spinCount,
		encryptedVerifierHashInput,
		encryptedVerifierHashValue,
		encryptedKeyValue,
	}
}
