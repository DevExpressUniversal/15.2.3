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
using System.Globalization;
namespace DevExpress.Utils.StructuredStorage.Internal {
	#region AbstractDirectoryEntry (abstract class)
	[CLSCompliant(false)]
	public abstract class AbstractDirectoryEntry {
		#region Fields
		UInt32 sid;
		string path;
		string name;
		DirectoryEntryType type;
		DirectoryEntryColor color;
		UInt32 leftSiblingSid;
		UInt32 rightSiblingSid;
		UInt32 childSiblingSid;
		Guid clsId;
		UInt32 userFlags;
		UInt32 startSector;
		UInt64 streamLength;
		#endregion
		protected AbstractDirectoryEntry()
			: this(0x0) { }
		protected AbstractDirectoryEntry(UInt32 sid) {
			this.sid = sid;
		}
		#region Properties
		public UInt32 Sid { get { return sid; } protected internal set { sid = value; } }
		public string Path { get { return path + Name; } }
		protected internal string InnerPath { get { return path; } set { path = value; } }
		public string Name {
			get { return Mask(name); }
			protected set {
				name = value;
				if (name.Length >= 32)
					ThrowInvalidValueInDirectoryEntryException("Name.Length >= 32");
			}
		}
		protected internal string InnerName { get { return name; } }
		public UInt16 LengthOfName {
			get {
				if (name.Length == 0)
					return 0;
				return (UInt16)((name.Length + 1) * 2);
			}
		}
		public DirectoryEntryType Type {
			get { return type; }
			protected set {
				if (value < DirectoryEntryType.MinValue || value > DirectoryEntryType.MaxValue)
					ThrowInvalidValueInDirectoryEntryException("Type");
				type = value;
			}
		}
		public DirectoryEntryColor Color {
			get { return color; }
			protected internal set {
				if (value < DirectoryEntryColor.MinValue || value > DirectoryEntryColor.MaxValue)
					ThrowInvalidValueInDirectoryEntryException("Color"); 
				color = value;
			}
		}
		public UInt32 LeftSiblingSid { get { return leftSiblingSid; } protected internal set { leftSiblingSid = value; } }
		public UInt32 RightSiblingSid { get { return rightSiblingSid; } protected internal set { rightSiblingSid = value; } }
		public UInt32 ChildSiblingSid { get { return childSiblingSid; } protected set { childSiblingSid = value; } }
		public Guid ClsId { get { return clsId; } protected set { clsId = value; } }
		public UInt32 UserFlags { get { return userFlags; } protected set { userFlags = value; } }
		public UInt32 StartSector { get { return startSector; } protected set { startSector = value; } }
		public UInt64 StreamLength { get { return streamLength; } protected set { streamLength = value; } }
		#endregion
		static readonly char[] CharsToMask = { '%', '\\' };
		internal static string Mask(string text) {
			string result = text;
			foreach (char character in CharsToMask)
				result = result.Replace(new String(character, 1), String.Format(CultureInfo.InvariantCulture, "%{0:X4}", (UInt32)character));
			return result;
		}
		void ThrowInvalidValueInDirectoryEntryException(string name) {
			throw new Exception("The value for '" + name + "' is invalid.");
		}
	}
	#endregion
}
