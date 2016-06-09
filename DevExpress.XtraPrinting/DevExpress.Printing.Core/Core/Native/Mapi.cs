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
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;
namespace DevExpress.XtraPrinting.Native {
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public class MapiMessage {
		public int reserved;
		public string subject;
		public string noteText;
		public string messageType;
		public string dateReceived;
		public string conversationID;
		public int flags;
		public IntPtr originator;
		public int recipientCount;
		public IntPtr recipients;
		public int fileCount;
		public IntPtr files;
	}
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public class MapiRecipDesc {
		public int reserved;
		public int recipientClass;
		public string name;
		public string address;
		public int eIDSize;
		public IntPtr entryID;
	}
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public class MapiFileDesc {
		public int reserved;
		public int flags;
		public int position;
		public string pathName;
		public IntPtr fileName;
		public IntPtr fileType;
	}
	[System.Security.SecuritySafeCritical]
	public class MAPI {
		#region static
		const int MAPI_LOGON_UI = 0x00000001;
		const int MAPI_DIALOG = 0x8;
		const int MAPI_USER_ABORT = 1;
		const int MAPI_E_LOGON_FAILURE = 3;
		[DllImport("MAPI32.DLL", CharSet = CharSet.Ansi)]
		private static extern int MAPISendMail(IntPtr session, IntPtr uiParam, MapiMessage message, int flags, int reserved);
		[DllImport("MAPI32.DLL", CharSet = CharSet.Ansi)]
		private static extern int MAPILogon(IntPtr hwnd, string profileName, string password, int flags, int reserved, ref IntPtr session);
		[DllImport("MAPI32.DLL", CharSet = CharSet.Ansi)]
		private static extern int MAPILogoff(IntPtr session, IntPtr hwnd, int flags, int reserved);
		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		private static extern uint GetShortPathName(string lpszLongPath, StringBuilder lpszShortPath, uint cchBuffer);
		static IntPtr OffsetPtr(IntPtr ptr, Type structureType, int offset) {
			return (IntPtr)((long)ptr + offset * Marshal.SizeOf(structureType));
		}
		#endregion
		string[] files;
		string subject = String.Empty;
		string body = String.Empty;
		IntPtr handle = IntPtr.Zero;
		IntPtr session = IntPtr.Zero;
		int error;
		List<IntPtr> stringList = new List<IntPtr>();
		RecipientCollection recipients = new RecipientCollection();
		public static bool UseLogon;
		public MAPI(IntPtr handle, string[] files, string mailSubject, string mailBody, RecipientCollection recipients) {
			this.files = files;
			this.subject = mailSubject;
			this.body = mailBody;
			this.recipients = recipients;
			if(UseLogon) {
				SendUsingLogon(handle);
				return;
			}
			MapiMessage msg = CreateMessage();
			error = MAPISendMail(session, handle, msg, MAPI_DIALOG, 0);
			if(error != 0 && error != MAPI_USER_ABORT)
				MAPISendMail(session, handle, msg, MAPI_DIALOG | MAPI_LOGON_UI, 0);
			DisposeMessage(msg);
		}
		void SendUsingLogon(IntPtr handle) {
			if(Logon(handle)) {
				MapiMessage msg = CreateMessage();
				error = MAPISendMail(session, handle, msg, MAPI_DIALOG, 0);
				Logoff();
				DisposeMessage(msg);
			}
		}
		MapiMessage CreateMessage() {
			MapiMessage msg = new MapiMessage();
			msg.subject = subject;
			msg.noteText = body;
			if(files.Length > 0) {
				msg.fileCount = files.Length;
				msg.files = GetFilesDesc();
			}
			if(recipients.Count > 0) {
				msg.recipientCount = recipients.Count;
				msg.recipients = GetRecipDesc();
			}
			return msg;
		}
		IntPtr GetFilesDesc() {
			IntPtr ptra = AllocMemory(typeof(MapiFileDesc), files.Length);
			for(int i = 0; i < files.Length; i++) {
				MapiFileDesc fileDesc = new MapiFileDesc();
				fileDesc.position = -1;
				fileDesc.fileName = StringToDefaultEncodingBytes(Path.GetFileName(files[i]));
				fileDesc.pathName = GetShortPathName(files[i]);
				Marshal.StructureToPtr(fileDesc, OffsetPtr(ptra, typeof(MapiFileDesc), i), false);
			}
			return ptra;
		}
		IntPtr StringToDefaultEncodingBytes(string str) {
			return MarshalStringBytes(System.Text.Encoding.Default.GetBytes(str));
		}
		IntPtr MarshalStringBytes(byte[] array) {
			int size = Marshal.SizeOf(array[0]) * (array.Length + 1);
			IntPtr ptr = Marshal.AllocHGlobal(size);
			stringList.Add(ptr);
			Marshal.Copy(array, 0, ptr, array.Length);
			Marshal.WriteByte(ptr, size - 1, 0);
			return ptr;
		}
		string GetShortPathName(string path) {
			StringBuilder sb = new StringBuilder();
			uint count = GetShortPathName(path, sb, 0);
			if(count == 0)
				return "";
			sb.Capacity = (int)count;
			GetShortPathName(path, sb, count);
			return sb.ToString();
		}
		IntPtr GetRecipDesc() {
			IntPtr ptra = AllocMemory(typeof(MapiRecipDesc), recipients.Count);
			Int64 ptr = (Int64)ptra;
			foreach(Recipient item in recipients) {
				MapiRecipDesc recipDesc = new MapiRecipDesc();
				recipDesc.reserved = 0;
				recipDesc.recipientClass = (int)item.FieldType;
				recipDesc.name = item.ContactName;
				recipDesc.address = item.Address;
				recipDesc.eIDSize = 0;
				recipDesc.entryID = IntPtr.Zero;
				Marshal.StructureToPtr(recipDesc, (IntPtr)ptr, false);
				ptr += Marshal.SizeOf(typeof(MapiRecipDesc));
			}
			return ptra;
		}
		IntPtr AllocMemory(Type structureType, int count) {
			return Marshal.AllocHGlobal(Marshal.SizeOf(structureType) * count);
		}
		void Logoff() {
			if(session != IntPtr.Zero) {
				error = MAPILogoff(session, handle, 0, 0);
				session = IntPtr.Zero;
			}
		}
		bool Logon(IntPtr hwnd) {
			this.handle = hwnd;
			error = MAPILogon(hwnd, null, null, 0, 0, ref session);
			if(error != 0)
				error = MAPILogon(hwnd, null, null, MAPI_LOGON_UI, 0, ref session);
			return error == 0;
		}
		void DisposeMessage(MapiMessage msg) {
			FreeMemory(msg.files, typeof(MapiFileDesc), files.Length);
			FreeMemory(msg.recipients, typeof(MapiRecipDesc), 1);
			msg = null;
			foreach(IntPtr ptr in stringList)
				Marshal.FreeHGlobal(ptr);
			stringList.Clear();
		}
		void FreeMemory(IntPtr ptr, Type structureType, int count) {
			if(ptr != IntPtr.Zero) {
				for(int i = 0; i < count; i++) {
					Marshal.DestroyStructure(OffsetPtr(ptr, structureType, i), structureType);
				}
				Marshal.FreeHGlobal(ptr);
			}
		}
	}
}
