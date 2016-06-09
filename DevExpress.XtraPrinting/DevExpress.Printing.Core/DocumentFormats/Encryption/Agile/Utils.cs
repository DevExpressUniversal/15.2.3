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

using DevExpress.Office.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Xml;
using System.Linq;
using System.Diagnostics;
namespace DevExpress.Office.Crypto.Agile {
	public static class Utils {
		internal static void MoveToNextContent(this XmlReader reader) {
			if (!reader.Read())
				throw new InvalidDataException();
			reader.MoveToContent();
		}
		internal static void MoveToStartElement(this XmlReader reader) {
			reader.MoveToNextContent();
			if (reader.NodeType != XmlNodeType.Element)
				throw new InvalidDataException();
		}
		internal static void MoveToStartElement(this XmlReader reader, string namespaceUri, string localName) {
			reader.MoveToStartElement();
			reader.CheckNode(namespaceUri, localName);
		}
		internal static void CheckNode(this XmlReader reader, string namespaceUri, string localName) {
			if (reader.LocalName != localName)
				throw new InvalidDataException();
			if (reader.NamespaceURI != namespaceUri)
				throw new InvalidDataException();
		}
		internal static void MoveToEndElement(this XmlReader reader) {
			if (!reader.IsEmptyElement) {
				reader.MoveToNextContent();
				if (reader.NodeType != XmlNodeType.EndElement)
					throw new InvalidDataException();
			}
		}
		internal static void ParseAttributes(this XmlReader reader, int attributeMax, Func<NameToken, String, bool> action) {
			int attributeCount = 0;
			if (reader.MoveToFirstAttribute()) {
				do {
					if (reader.IsXmlnsAttribute())
						continue;
					NameToken nameToken;
					if (!Enum.TryParse(reader.LocalName, out nameToken))
						throw new InvalidDataException();
					if (reader.NamespaceURI.Length != 0)
						throw new InvalidDataException();
					if (!action(nameToken, reader.Value))
						throw new InvalidDataException();
					attributeCount++;
				} while (reader.MoveToNextAttribute());
			}
			if (attributeCount != attributeMax)
				throw new InvalidDataException();
			reader.MoveToElement();
		}
		internal static bool IsXmlnsAttribute(this XmlReader reader) {
			if (reader.NodeType != XmlNodeType.Attribute)
				return false;
			if (reader.Prefix == "xmlns")
				return true;
			if (reader.Prefix.Length == 0 && reader.LocalName == "xmlns")
				return true;
			return false;
		}
		public static ICryptoTransform GetCryptoTransform(this ICipherProvider cipher, int key1, int key2, bool isEncryption) {
			if (key2 == 0) {
				return cipher.GetCryptoTransform(BitConverter.GetBytes(key1), isEncryption);
			}
			else {
				byte[] blockKey = new byte[8];
				Buffer.BlockCopy(BitConverter.GetBytes(key2), 0, blockKey, 0, 4);
				Buffer.BlockCopy(BitConverter.GetBytes(key1), 0, blockKey, 4, 4);
				return cipher.GetCryptoTransform(blockKey, isEncryption);
			}
		}
		public static ICryptoTransform GetEncryptor(this ICipherProvider cipher, int key1, int key2) {
			return cipher.GetCryptoTransform(key1, key2, true);
		}
		public static ICryptoTransform GetEncryptor(this ICipherProvider cipher, byte[] blockKey) {
			return cipher.GetCryptoTransform(blockKey, true);
		}
		public static ICryptoTransform GetDecryptor(this ICipherProvider cipher, int key1, int key2) {
			return cipher.GetCryptoTransform(key1, key2, false);
		}
		public static ICryptoTransform GetDecryptor(this ICipherProvider cipher, byte[] blockKey) {
			return cipher.GetCryptoTransform(blockKey, false);
		}
		public static void TransformInPlace(this ICryptoTransform transform, byte[] buffer, int offset, int count) {
			if (count == 0)
				return;
			Debug.Assert((count % transform.InputBlockSize) == 0);
			Debug.Assert(transform.InputBlockSize == transform.OutputBlockSize);
			int outputCount = transform.TransformBlock(buffer, offset, count, buffer, offset);
			if (count != outputCount)
				throw new InvalidDataException();
		}
		public static bool EqualBytes(this byte[] arr1, byte[] arr2) {
			if (arr1.Length != arr2.Length)
				return false;
			for (int i = 0; i < arr1.Length; i++) {
				if (arr1[i] != arr2[i])
					return false;
			}
			return true;
		}
		public static byte[] CloneToFit(this byte[] input, int size) {
			return input.CloneToFit(size, 0x36);
		}
		public static byte[] CloneToFit(this byte[] input, int size, byte pad) {
			byte[] output = new byte[size];
			Buffer.BlockCopy(input, 0, output, 0, Math.Min(input.Length, output.Length));
			for (int i = input.Length; i < output.Length; i++)
				output[i] = pad;
			return output;
		}
		public static int RoundUp(int value, int round) {
			if (round == 0)
				return round;
			return ((value + round - 1) / round) * round;
		}
	}
}
