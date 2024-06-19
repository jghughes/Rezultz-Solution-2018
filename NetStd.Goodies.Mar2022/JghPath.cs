// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==
/*============================================================
**
** Class:  JghPath
** 
** <OWNER>[....]</OWNER>
**
**
** Purpose: A collection of path manipulation methods.
**
**  this class is a 99% line for line copy/paste of System.IO.Path, followed by a recursive 
**  commenting-out of all methods that can't be implemented in a portable class library effortlessly.
**
**  for the C# source code see the reference source for .Net at http://referencesource.microsoft.com/
* 
*   For a reference on path, directory and file naming conventions see https://msdn.microsoft.com/en-us/library/windows/desktop/aa365247(v=vs.85).aspx
**
===========================================================*/

using System;
using System.Text;

namespace NetStd.Goodies.Mar2022
{
	/// <summary>
	///     Provides methods for processing directory strings in an ideally
	///     cross-platform manner.Most of the methods don't do a complete
	///     full parsing (such as examining a UNC hostname), but they will
	///     handle most string operations.
	///     File names cannot contain backslash (\), slash(/), colon(:),
	///     asterisk(*), question mark(?), quote("), LessThan,
	///     GreaterThan, or pipe (|).  The first three are used as directory
	///     separators on various platforms.Asterisk and question mark are treated
	///     as wild cards.  LessThan, GreaterThan, and pipe all redirect input
	///     or output from a program to a file or some combination thereof.  Quotes
	///     are special. Also prohibited are Integer value zero, sometimes referred
	///     to as the ASCII NUL character and characters whose integer representations
	///     are in the range from 1 through 31. Everything prohibited in filenames
	///     is also prohibited in directory names. File I/O functions in the Windows API
	///     convert "/" to "\" as part of converting the name to an NT-style name, so
	///     they are interchangeable.
	///     Comprising a concatenation of one or more directory names and optionally
	///     a file name, a path may include the volume separator (:) in the second position and one or more
	///     directory separators(\ or /).
	///     We are guaranteeing that Path.SeparatorChar is the correct
	///     directory separator on all platforms, and we will support
	///     Path.AltSeparatorChar as well.To write cross platform
	///     code with minimal pain, you can use slash(/) as a directory separator in
	///     your strings.
	///     Class contains only static data, no need to serialize
	/// </summary>
	public static class JghPath
	{
		/// <summary>
		///     Changes the extension of a file path. The path parameter
		///     specifies a file path, and the extension parameter
		///     specifies a file extension (with a leading period, such as
		///     ".exe" or ".cs").
		///     The function returns a file path with the same root, directory, and base
		///     name parts as path, but with the file extension changed to
		///     the specified extension. If path is null, the function
		///     returns null. If path does not contain a file extension,
		///     the new file extension is appended to the path. If extension
		///     is null, any exsiting extension is removed from path.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="extension">The extension.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentException">throws Argument_InvalidPathChars if path contains invalid characters</exception>
		public static string ChangeExtension(string path, string extension)
		{
			if (path is not null)
			{
				CheckInvalidPathChars(path);

				var s = path;
				for (var i = path.Length; --i >= 0;)
				{
					var ch = path[i];
					if (ch == '.')
					{
						s = path.Substring(0, i);
						break;
					}
					if (ch == DirectorySeparatorChar || ch == AltDirectorySeparatorChar || ch == VolumeSeparatorChar)
						break;
				}
				if (extension is not null && path.Length != 0)
				{
					if (extension.Length == 0 || extension[0] != '.')
						s += ".";
					s += extension;
				}
				return s;
			}
			return null;
		}

		/// <summary>
		///     Gets the invalid path chars.
		///     NTFS file and folder names must be between 1 and 255 characters long and musn't have a leading space.
		///     File names cannot contain backslash (\), slash (/), colon (:),
		///     asterisk (*), question mark (?), quote ("), LessThan,
		///     GreaterThan, or pipe (|).  The first three are used as directory
		///     separators on various platforms.  Asterisk and question mark are treated
		///     as wild cards.  LessThan, GreaterThan, and pipe all redirect input
		///     or output from a program to a file or some combination thereof.  Quotes
		///     are special.
		///     For purposes here, the wildcards '?' and '*' are prohibited.
		/// </summary>
		/// <returns></returns>
		public static char[] GetInvalidPathChars()
		{
			return (char[])RealInvalidPathCharsWithWildCardsOk.Clone();
		}

		/// <summary>
		///     File names cannot contain backslash (\), slash (/), colon (:),
		///     asterisk (*), question mark (?), quote ("), LessThan,
		///     GreaterThan, or pipe (|).  The first three are used as directory
		///     separators on various platforms.  Asterisk and question mark are treated
		///     as wild cards.  LessThan, GreaterThan, and pipe all redirect input
		///     or output from a program to a file or some combination thereof.  Quotes
		///     are special.
		/// </summary>
		/// <returns></returns>
		public static char[] GetInvalidFileNameChars()
		{
			return (char[])InvalidFileNameChars.Clone();
		}

		/// <summary>
		///     Returns the extension of the given path. The returned value includes the
		///     period (".") character of the extension except when you have a terminal period when you get String.Empty, such as
		///     ".exe" or
		///     ".cpp". The returned value is null if the given path is
		///     null or if the given path does not include an extension.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns></returns>
		public static string GetExtension(string path)
		{
			if (path is null)
				return null;

			CheckInvalidPathChars(path);
			var length = path.Length;
			for (var i = length; --i >= 0;)
			{
				var ch = path[i];
				if (ch == '.')
				{
					if (i != length - 1)
						return path.Substring(i, length - i);
					return string.Empty;
				}
				if (ch == DirectorySeparatorChar || ch == AltDirectorySeparatorChar || ch == VolumeSeparatorChar)
					break;
			}
			return string.Empty;
		}

		internal static bool HasLongPathPrefix(string path)
		{
			return path.StartsWith(LongPathPrefix, StringComparison.Ordinal);
		}

		internal static string AddLongPathPrefix(string path)
		{
			if (path.StartsWith(LongPathPrefix, StringComparison.Ordinal))
				return path;

			if (path.StartsWith(UncPathPrefix, StringComparison.Ordinal))
				return path.Insert(2, UncLongPathPrefixToInsert);
			// Given \\server\share in longpath becomes \\?\UNC\server\share  => UNCLongPathPrefix + path.SubString(2); => The actual command simply reduces the operation cost.

			return LongPathPrefix + path;
		}

		internal static string RemoveLongPathPrefix(string path)
		{
			if (!path.StartsWith(LongPathPrefix, StringComparison.Ordinal))
				return path;

			if (path.StartsWith(UncLongPathPrefix, StringComparison.OrdinalIgnoreCase))
				return path.Remove(2, 6);
			// Given \\?\UNC\server\share we return \\server\share => @'\\' + path.SubString(UNCLongPathPrefix.Length) => The actual command simply reduces the operation cost.

			return path.Substring(4);
		}

		internal static StringBuilder RemoveLongPathPrefix(StringBuilder pathSb)
		{
			if (pathSb is null) return new StringBuilder();

			var path = pathSb.ToString();

			if (!path.StartsWith(LongPathPrefix, StringComparison.Ordinal))
				return pathSb;

			if (path.StartsWith(UncLongPathPrefix, StringComparison.OrdinalIgnoreCase))
				return pathSb.Remove(2, 6);
			// Given \\?\UNC\server\share we return \\server\share => @'\\' + path.SubString(UNCLongPathPrefix.Length) => The actual command simply reduces the operation cost.

			return pathSb.Remove(0, 4);
		}

		// Returns the name and extension parts of the given path. The resulting
		// string contains the characters of path that follow the last
		// backslash ("\"), slash ("/"), or colon (":") character in 
		// path. The resulting string is the entire path if path 
		// contains no backslash after removing trailing slashes, slash, or colon characters. The resulting 
		// string is null if path is null.
		//
		public static string GetFileName(string path)
		{
			if (path is not null)
			{
				CheckInvalidPathChars(path);

				var length = path.Length;
				for (var i = length; --i >= 0;)
				{
					var ch = path[i];
					if (ch == DirectorySeparatorChar || ch == AltDirectorySeparatorChar || ch == VolumeSeparatorChar)
						return path.Substring(i + 1, length - i - 1);
				}
			}
			return path;
		}

		public static string GetFileNameWithoutExtension(string path)
		{
			path = GetFileName(path);

			if (path is not null)
			{
				int i;

				if ((i = path.LastIndexOf('.')) == -1)
					return path; // No path extension found

				return path.Substring(0, i);
			}
			return null;
		}

		public static string RemoveAllExtensions(string fileNameAsString)
		{
			while (HasExtension(fileNameAsString))
				fileNameAsString = GetFileNameWithoutExtension(fileNameAsString);

			return fileNameAsString;
		}

		/// <summary>
		///     Returns the root portion of the given path. The resulting string
		///     consists of those rightmost characters of the path that constitute the
		///     root of the path. Possible patterns for the resulting string are: An
		///     empty string (a relative path on the current drive), "\" (an absolute
		///     path on the current drive), "X:" (a relative path on a given drive,
		///     where X is the drive letter), "X:\" (an absolute path on a given drive),
		///     and "\\server\share" (a UNC path for a given server and share name).
		///     The resulting string is null if path is null.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns></returns>
		//        [Pure]
		//        [ResourceExposure(ResourceScope.Machine)]
		//        [ResourceConsumption(ResourceScope.Machine)]
		//        public static string GetPathRoot(string path)
		//        {
		//            if (path is null) return null;
		//            path = NormalizePath(path, false);
		//            return path.Substring(0, GetRootLength(path));
		//        }

		//        [System.Security.SecuritySafeCritical]
		//        [ResourceExposure(ResourceScope.Machine)]
		//        [ResourceConsumption(ResourceScope.Machine)]
		//        public static string GetTempPath()
		//        {
		//#if !FEATURE_CORECLR
		//            new EnvironmentPermission(PermissionState.Unrestricted).Demand();
		//#endif
		//            StringBuilder sb = new StringBuilder(MAX_PATH);
		//            uint r = Win32Native.GetTempPath(MAX_PATH, sb);
		//            string path = sb.ToString();
		//            if (r == 0) __Error.WinIOError();
		//            path = GetFullPathInternal(path);
		//#if FEATURE_CORECLR
		//            FileSecurityState state = new FileSecurityState(FileSecurityStateAccess.Write, String.Empty, path);
		//            state.EnsureState();
		//#endif
		//            return path;
		//        }
		internal static bool IsRelative(string path)
		{
			if (path is null) throw new ArgumentNullException(nameof(path));
			if (path.Length >= 3 && path[1] == VolumeSeparatorChar && path[2] == DirectorySeparatorChar &&
				(path[0] >= 'a' && path[0] <= 'z' || path[0] >= 'A' && path[0] <= 'Z') ||
				path.Length >= 2 && path[0] == '\\' && path[1] == '\\')
				return false;
			return true;
		}

		/// <summary>
		///     Determines whether the specified path has extension.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <exception cref="System.ArgumentException">throws Argument_InvalidPathChars if path contains invalid characters</exception>
		/// <returns></returns>
		public static bool HasExtension(string path)
		{
			if (path is not null)
			{
				CheckInvalidPathChars(path);

				for (var i = path.Length; --i >= 0;)
				{
					var ch = path[i];
					if (ch == '.')
					{
						if (i != path.Length - 1)
							return true;
						return false;
					}
					if (ch == DirectorySeparatorChar || ch == AltDirectorySeparatorChar || ch == VolumeSeparatorChar)
						break;
				}
			}
			return false;
		}

		/// <summary>
		///     Returns true if path begins with a directory separator character (\, \\ or /).
		///     If the path begins with anything else, the beginning character is deemed to be a volume identifier
		///     and therefore the second character is required to be the volume separator character (:).
		/// </summary>
		/// <param name="path">The path.</param>
		/// <exception cref="System.ArgumentException">throws Argument_InvalidPathChars if path contains invalid characters</exception>
		/// <returns></returns>
		public static bool IsPathRooted(string path)
		{
			if (path is null) return false;

			CheckInvalidPathChars(path);

			var length = path.Length;

			if (length >= 1 && (path[0] == DirectorySeparatorChar || path[0] == AltDirectorySeparatorChar) ||
				length >= 2 && path[1] == VolumeSeparatorChar)
				return true;

			return false;
		}

		public static string Combine(string path1, string path2)
		{
			if (path1 is null || path2 is null)
				throw new ArgumentNullException(path1 is null ? "path1" : "path2");
			CheckInvalidPathChars(path1);
			CheckInvalidPathChars(path2);

			return CombineNoChecks(path1, path2);
		}

		public static string Combine(string path1, string path2, string path3)
		{
			if (path1 is null || path2 is null || path3 is null)
				throw new ArgumentNullException(path1 is null ? "path1" : path2 is null ? "path2" : "path3");
			CheckInvalidPathChars(path1);
			CheckInvalidPathChars(path2);
			CheckInvalidPathChars(path3);

			return CombineNoChecks(CombineNoChecks(path1, path2), path3);
		}

		public static string Combine(string path1, string path2, string path3, string path4)
		{
			if (path1 is null || path2 is null || path3 is null || path4 is null)
				throw new ArgumentNullException(path1 is null
					? "path1"
					: path2 is null
						? "path2"
						: path3 is null
							? "path3"
							: "path4");
			CheckInvalidPathChars(path1);
			CheckInvalidPathChars(path2);
			CheckInvalidPathChars(path3);
			CheckInvalidPathChars(path4);

			return CombineNoChecks(CombineNoChecks(CombineNoChecks(path1, path2), path3), path4);
		}
		public static string Combine(string path1, string path2, string path3, string path4, string path5)
		{
			if (path1 is null || path2 is null || path3 is null || path4 is null || path5 is null)
				throw new ArgumentNullException(path1 is null
						? "path1"
						: path2 is null
							? "path2"
							: path3 is null
								? "path3"
								: path4 is null
									? "path4"
									: "path5");
			CheckInvalidPathChars(path1);
			CheckInvalidPathChars(path2);
			CheckInvalidPathChars(path3);
			CheckInvalidPathChars(path4);
			CheckInvalidPathChars(path5);

			return CombineNoChecks(CombineNoChecks(CombineNoChecks(CombineNoChecks(path1, path2), path3), path4), path5);
		}

		public static string CombineNoChecks(string path1, string path2)
		{
			if (path2.Length == 0)
				return path1;

			if (path1.Length == 0)
				return path2;

			if (IsPathRooted(path2))
				return path2;

			var ch = path1[path1.Length - 1];
			if (ch != DirectorySeparatorChar && ch != AltDirectorySeparatorChar && ch != VolumeSeparatorChar)
				return path1 + DirectorySeparatorCharAsString + path2;
			return path1 + path2;
		}


		/// <summary>
		///     Checks the search pattern.
		/// </summary>
		/// <param name="searchPattern">The search pattern.</param>
		/// <exception cref="System.ArgumentException">
		///     Arg_InvalidSearchPattern
		///     or
		///     Arg_InvalidSearchPattern
		/// </exception>
		internal static void CheckSearchPattern(string searchPattern)
		{
			int index;
			while ((index = searchPattern.IndexOf("..", StringComparison.Ordinal)) != -1)
			{
				if (index + 2 == searchPattern.Length) // Terminal ".." . Files names cannot end in ".."
					throw new ArgumentException("Arg_InvalidSearchPattern");
				//throw new ArgumentException(Environment.GetResourceString("Arg_InvalidSearchPattern"));

				if (searchPattern[index + 2] == DirectorySeparatorChar
					|| searchPattern[index + 2] == AltDirectorySeparatorChar)
					throw new ArgumentException("Arg_InvalidSearchPattern");
				//throw new ArgumentException(Environment.GetResourceString("Arg_InvalidSearchPattern"));

				searchPattern = searchPattern.Substring(index + 2);
			}
		}

		/// <summary>
		///     Determines whether the specified NTFS path contains illegal characters.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="wildCardsAreInvalidToo">
		///     if set to <c>true</c> the wildcard characters "*" and "?" are deemed illegal.
		///     Otherwise they are OK.
		/// </param>
		/// <returns></returns>
		public static bool HasIllegalPathCharacters(string path, bool wildCardsAreInvalidToo = false)
		{
			if (path is null)
				return true;

			if (wildCardsAreInvalidToo)
				return path.IndexOfAny(InvalidPathCharsWithWildCardsAlsoInvalid) >= 0;

			return path.IndexOfAny(RealInvalidPathCharsWithWildCardsOk) >= 0;
		}

		/// <summary>
		///     Checks for invalid path chars and throws exception if any.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="wildCardsAreInvalidToo">
		///     if set to <c>true</c> [additionally deeems invalid the wildcard characters "*" and
		///     "?"].
		/// </param>
		/// <exception cref="System.ArgumentNullException">path is null</exception>
		/// <exception cref="System.ArgumentException">throws Argument_InvalidPathChars if path contains invalid characters</exception>
		private static void CheckInvalidPathChars(string path, bool wildCardsAreInvalidToo = false)
		{
			if (path is null)
				throw new ArgumentNullException(nameof(path));

			if (HasIllegalPathCharacters(path, wildCardsAreInvalidToo))
				throw new ArgumentException("Argument_InvalidPathChars");
			//throw new ArgumentException(Environment.GetResourceString("Argument_InvalidPathChars"));
		}

		#region constants

		private const string LongPathPrefix = @"\\?\";
		private const string UncPathPrefix = @"\\";
		private const string UncLongPathPrefixToInsert = @"?\UNC\";
		private const string UncLongPathPrefix = @"\\?\UNC\";

		private const string DirectorySeparatorCharAsString = "\\";
		// 

		/// <summary>
		///     Platform specific directory separator character.  This is backslash
		///     ('\') on Windows, slash ('/') on Unix, and colon (':') on Mac.
		/// </summary>
		public static readonly char DirectorySeparatorChar = '\\';

		/// <summary>
		///     Platform specific alternate directory separator character.
		///     This is backslash ('\') on Unix, and slash ('/') on Windows
		/// </summary>
		public static readonly char AltDirectorySeparatorChar = '/';

		/// <summary>
		///     Platform specific volume separator character.  This is colon (':')
		///     on Windows and MacOS, and slash ('/') on Unix.  This is mostly
		///     useful for parsing paths like "c:\windows" or "MacVolume:System Folder".
		/// </summary>
		private static readonly char VolumeSeparatorChar = ':';

		//// Platform specific invalid list of characters in a path.
		//// See the "Naming a File" MSDN conceptual docs for more details on
		//// what is valid in a file name (which is slightly different from what
		//// is legal in a path name).
		//// Note: This list is duplicated in CheckInvalidPathChars
		//[Obsolete("Please use GetInvalidPathChars or GetInvalidFileNameChars instead.")]
		//public static readonly char[] InvalidPathChars = { '\"', '<', '>', '|', '\0', (char)1, (char)2, (char)3, (char)4, (char)5, (char)6, (char)7, (char)8, (char)9, (char)10, (char)11, (char)12, (char)13, (char)14, (char)15, (char)16, (char)17, (char)18, (char)19, (char)20, (char)21, (char)22, (char)23, (char)24, (char)25, (char)26, (char)27, (char)28, (char)29, (char)30, (char)31 };

		// Trim trailing white spaces, tabs etc but don't be aggressive in removing everything that has UnicodeCategory of trailing space.
		// String.WhitespaceChars will trim more aggressively than what the underlying FS does (for ex, NTFS, FAT).            
		/// <summary>
		/// </summary>
		public static readonly char[] TrimEndChars =
        [
            (char) 0x9, (char) 0xA, (char) 0xB, (char) 0xC, (char) 0xD,
			(char) 0x20, (char) 0x85, (char) 0xA0
        ];

		/// <summary>
		///     Platform specific invalid list of characters in a path.
		///     See the "Naming a File" MSDN conceptual docs for more details on
		///     what is valid in a file name (which is slightly different from what
		///     is legal in a path name). Wildcards are not deemed invalid.
		///     Note: This list is duplicated in CheckInvalidPathChars
		/// </summary>
		public static readonly char[] RealInvalidPathCharsWithWildCardsOk =
        [
            '\"', '<', '>', '|', '\0', (char) 1, (char) 2, (char) 3,
			(char) 4, (char) 5, (char) 6, (char) 7, (char) 8, (char) 9, (char) 10, (char) 11, (char) 12, (char) 13,
			(char) 14, (char) 15, (char) 16, (char) 17, (char) 18, (char) 19, (char) 20, (char) 21, (char) 22,
			(char) 23,
			(char) 24, (char) 25, (char) 26, (char) 27, (char) 28, (char) 29, (char) 30, (char) 31
        ];

		/// <summary>
		///     Platform specific invalid list of characters in a path.
		///     See the "Naming a File" MSDN conceptual docs for more details on
		///     what is valid in a file name (which is slightly different from what
		///     is legal in a path name). Wildcards are deemed invalid.
		///     Note: This list is duplicated in HasIllegalPathCharacters
		/// </summary>
		public static readonly char[] InvalidPathCharsWithWildCardsAlsoInvalid =
        [
            '\"', '<', '>', '|', '\0', (char) 1,
			(char) 2, (char) 3, (char) 4, (char) 5, (char) 6, (char) 7, (char) 8, (char) 9, (char) 10, (char) 11,
			(char) 12, (char) 13, (char) 14, (char) 15, (char) 16, (char) 17, (char) 18, (char) 19, (char) 20,
			(char) 21,
			(char) 22, (char) 23, (char) 24, (char) 25, (char) 26, (char) 27, (char) 28, (char) 29, (char) 30,
			(char) 31,
			'*', '?'
        ];

		public static readonly char[] InvalidFileNameChars =
        [
            '\"', '<', '>', '|', '\0', (char) 1, (char) 2, (char) 3,
			(char) 4, (char) 5, (char) 6, (char) 7, (char) 8, (char) 9, (char) 10, (char) 11, (char) 12, (char) 13,
			(char) 14, (char) 15, (char) 16, (char) 17, (char) 18, (char) 19, (char) 20, (char) 21, (char) 22,
			(char) 23,
			(char) 24, (char) 25, (char) 26, (char) 27, (char) 28, (char) 29, (char) 30, (char) 31, ':', '*', '?', '\\',
			'/'
        ];

		public static readonly char PathSeparator = ';';

		//private static readonly char[] SBase32Char =
		//{
		//    'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h',
		//    'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p',
		//    'q', 'r', 's', 't', 'u', 'v', 'w', 'x',
		//    'y', 'z', '0', '1', '2', '3', '4', '5'
		//};


		// Make this public sometime.
		// The max total path is 260, and the max individual component length is 255. 
		// For example, D:\<256 char file name> isn't legal, even though it's under 260 chars.
		//internal static readonly int MaxPath = 260;
		//private static readonly int MaxDirectoryLength = 255;

		// Windows API definitions
		//internal const int MAX_PATH = 260;  // From WinDef.h
		//internal const int MAX_DIRECTORY_PATH = 248;   // cannot create directories greater than 248 characters

		#endregion

	}
}