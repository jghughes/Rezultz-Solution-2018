using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jgh.SymbolsStringsConstants.Mar2022;

//using Jgh.SymbolsStringsConstants.Mar2022;

// ReSharper disable UnusedVariable

namespace NetStd.Goodies.Mar2022
{
	public static class JghFilePathValidator
	{
		private const string Locus2 = nameof(JghFilePathValidator);
		private const string Locus3 = "[NetStd.Goodies.Mar2022]";


        public const int SeasonProfileIdLowerLimit = 100;
        public const int SeasonProfileIdUpperLimit = 999;




        #region SeasonProfile

        public static bool IsSuperficiallyValidXmlFileNameOfSeasonProfile(string candidateMetadataIdAsString)
        {
            var numOfChars = candidateMetadataIdAsString.Length;

            if (numOfChars > 3 || numOfChars < 3)
                return false;

            var xx = JghConvert.TryConvertToInt32(candidateMetadataIdAsString, out var resultingInteger,
                out _);

            if (xx == false) return false;

            var yy = XmlFileNameOfSeasonProfileIsWithinValidRange(resultingInteger);

            return yy;
        }

        public static bool XmlFileNameOfSeasonProfileIsWithinValidRange(int candidateMetadataId)
        {
            return candidateMetadataId is >= SeasonProfileIdLowerLimit and <= SeasonProfileIdUpperLimit;
        }

        #endregion

		#region Azure storage

		/// <summary>
		///     Checks storage account name for improper format and content
		/// </summary>
		/// <param name="nameAsString">name</param>
		/// <param name="errorDescription">explanation of error</param>
		/// <returns>true if name is proper</returns>
		public static bool IsValidAccountName(string nameAsString, out string errorDescription)
		{
			const string failure = "Unable to determine if name is a valid Microsoft Azure storage account name.";
			const string locus = "[IsValidAccountName]";

			errorDescription = string.Empty;

			try
			{
				if (string.IsNullOrEmpty(nameAsString))
				{
					errorDescription = "Account name is empty. A name of 3 - 24 characters must be provided.";
					return false;
				}

				if (nameAsString[0] == ' ')
				{
					errorDescription = "Account name begins with a space. A leading space is forbidden.";
					return false;
				}

				if ((nameAsString.Length > 24) | (nameAsString.Length < 3))
				{
					errorDescription =
						$"Account name has {nameAsString.Length} characters whereas the permitted range is 3 - 24.";
					return false;
				}

				var nameAsCharArray = nameAsString.ToCharArray();

				if (nameAsCharArray.Any(char.IsUpper))
				{
					var count = nameAsCharArray.Count(char.IsUpper);

					var sb = new StringBuilder();
					foreach (var upperCaseChar in nameAsCharArray.Where(char.IsUpper))
						sb.Append(upperCaseChar);

					errorDescription =
						$"Account name has {count} upper case characters which is forbidden. The offending characters are <{sb}>.";
					return false;
				}

				var forbiddenCharacters = GetInvalidAzureAccountNameChars(nameAsString);
				if (forbiddenCharacters.Any())
				{
					var sb = new StringBuilder();
					foreach (var forbiddenCharacter in forbiddenCharacters)
						sb.Append(forbiddenCharacter);
					errorDescription =
						$"Account name contains {forbiddenCharacters.Count} prohibited characters. The offending characters are <{sb}>. Only letters and digits are permitted.";
					return false;
				}

				//if (!IsValidUriString(nameAsString, out var dummy, out var errorMessage))
				//{
				//    errorDescription =
				//        $"Container name contains one or more non-compliant characters. <{nameAsString}> Name must comply with international standards including RFC 2396 and RFC 2732 regarding Url format and content.";
				//    return false;
				//}

				return true;
			}
			catch (Exception ex)
			{
				errorDescription = JghString.ConcatAsSentences(failure, ex.Message, locus, null);
				return false;
			}
		}

		/// <summary>
		///     Checks Azure container name for improper format and content
		/// </summary>
		/// <param name="nameAsString">name</param>
		/// <param name="errorDescription">explanation of error</param>
		/// <returns>true if name is proper</returns>
		public static bool IsValidContainerName(string nameAsString, out string errorDescription)
		{
			const string failure = "Unable to determine if name is a valid Microsoft Azure storage container name.";
			const string locus = "[IsValidContainerName]";

			errorDescription = string.Empty;

			try
			{
				if (string.IsNullOrEmpty(nameAsString))
				{
					errorDescription = "Container name is empty. A name of 3 - 63 characters must be provided.";
					return false;
				}

				if (nameAsString[0] == ' ')
				{
					errorDescription = "Container name begins with a space. A leading space is forbidden.";
					return false;
				}


				if ((nameAsString.Length > 63) | (nameAsString.Length < 3))
				{
					errorDescription =
						$"Container name has {nameAsString.Length} characters whereas the permitted range is 3 - 63.";
					return false;
				}

				var forbiddenCharacters = GetInvalidAzureContainerNameChars(nameAsString);
				if (forbiddenCharacters.Any())
				{
					var sb = new StringBuilder();
					foreach (var forbiddenCharacter in forbiddenCharacters)
						sb.Append(forbiddenCharacter);
					errorDescription =
						$"Container name contains {forbiddenCharacters.Count} prohibited characters. The offending characters in <{nameAsString}> are <{sb}>. Only letters, digits and dashes are allowed. ";
					return false;
				}

				var nameAsCharArray = nameAsString.ToCharArray();

				if (nameAsCharArray.Any(char.IsUpper))
				{
					var count = nameAsCharArray.Count(char.IsUpper);

					var sb = new StringBuilder();
					foreach (var upperCaseChar in nameAsCharArray.Where(char.IsUpper))
						sb.Append(upperCaseChar);

					//explanation = string.Format("Container name has {0} upper case characters. The offending characters are <{1}>. Only lower case is permitted.", count, sb.ToString());
					errorDescription =
						$"Container name contains {count} upper case characters. <{nameAsString}>. Only lower case is permitted.";
					return false;
				}


				if (nameAsCharArray.First() == '-')
				{
					errorDescription = "Container name begins with a dash. A leading dash is forbidden.";
					return false;
				}


				if (nameAsCharArray.Last() == '-')
				{
					errorDescription = "Container name ends with a dash. A trailing dash is forbidden.";
					return false;
				}

				if (nameAsCharArray.Length >= 2)
				{
					var containsSequentialDashes = false;

					for (var i = 0; i < nameAsCharArray.Length - 1; i++)
						if ((nameAsCharArray[i] == '-') & (nameAsCharArray[i + 1] == '-'))
						{
							containsSequentialDashes = true;
							break;
						}

					if (containsSequentialDashes)
					{
						errorDescription =
							"Container name has two or more sequential dashes. Sequential dashes are forbidden.";
						return false;
					}
				}

				//if (!IsValidUriString(nameAsString, out var dummy, out var errorMessage))
				//{
				//    errorDescription =
				//        $"Container name is problematic. <{nameAsString}> Name must comply with international standards including RFC 2396 and RFC 2732 regarding Url format and content.";
				//    return false;
				//}

				return true;
			}
			catch (Exception ex)
			{
				errorDescription = JghString.ConcatAsSentences(failure, ex.Message, locus, null);
				return false;
			}
		}

		/// <summary>
		///     Checks blob name for improper format and content.
		///     Name must not contain spaces or be longer than 255 characters.
		/// </summary>
		/// <param name="nameAsString">name</param>
		/// <param name="errorDescription">explanation of error</param>
		/// <returns>true if name is proper</returns>
		public static bool IsValidBlobName(string nameAsString, out string errorDescription)
		{
			const string failure = "Unable to determine if name is a valid Microsoft Azure storage blob name.";
			const string locus = "[IsValidBlobName]";

			errorDescription = string.Empty;

			try
			{
				if (string.IsNullOrEmpty(nameAsString))
				{
					errorDescription = "Blob name is empty. Name must be provided.";
					return false;
				}

				if (nameAsString.ToCharArray().Any(z => z == ' '))
				{
					var count = nameAsString.ToCharArray().Count(z => z == ' ');
					errorDescription =
						$"Blob name has {count} spaces. Spaces are forbidden. (This is to ensure that the blob can be addressed using a URL in applications.) ";
					return false;
				}

				if (nameAsString.Length > 255)
				{
					errorDescription =
						$"Blob name has {nameAsString.Length} characters, whereas the permitted range is 1 - 255.";
					return false;
				}

				return true;
			}
			catch (Exception ex)
			{
				errorDescription = JghString.ConcatAsSentences(failure, ex.Message, locus, null);
				return false;
			}
		}

		/// <summary>
		///     Eliminates spaces, if any, with a specified replacement character.
		/// </summary>
		/// <param name="replacementChar">The replacement character.</param>
		/// <param name="provisionalName">The provisional name</param>
		/// <returns></returns>
		public static string EliminateSpaces(char replacementChar,
			string provisionalName)
		{
			var characters = provisionalName.ToCharArray();

			for (var i = 0; i < characters.Length; i++)
				if (characters[i] == ' ')
					characters[i] = replacementChar;

			var sb = new StringBuilder();

			var j = 1;
			foreach (var character in characters)
			{
				if (j < 256)
					sb.Append(character);
				j++;
			}

			var answer = sb.ToString();

			return answer;
		}

		/// <summary>
		///     Checks connection parameters for improper format and content
		/// </summary>
		/// <param name="accountName">account name</param>
		/// <param name="containerName">container name</param>
		/// <param name="blobName">blob name</param>
		/// <param name="errorDescription">error message</param>
		/// <returns>true if particulars are proper</returns>
		public static bool IsValidBlobLocationSpecification(string accountName, string containerName, string blobName,
			out string errorDescription)
		{
			errorDescription = "Improper or invalid cloud storage particulars.";

			var accountNameIsValid = IsValidAccountName(accountName, out var accountNameMsg);

			var containerNameIsValid = IsValidContainerName(containerName, out var containerNameMsg);

			var blobNameIsValid = IsValidBlobName(blobName, out var blobNameMsg);

			if (accountNameIsValid && containerNameIsValid && blobNameIsValid)
				return true;

			var sb = new StringBuilder();

			sb.Append(errorDescription);

			if (!accountNameIsValid)
			{
				sb.Append(" ");
				sb.Append(accountNameMsg);
			}

			if (!containerNameIsValid)
			{
				sb.Append(" ");
				sb.Append(containerNameMsg);
			}

			if (!blobNameIsValid)
			{
				sb.Append(" ");
				sb.Append(blobNameMsg);
			}

			errorDescription = sb.ToString();

			return false;
		}
		/// <summary>
		///     Checks connection parameters for improper format and content
		/// </summary>
		/// <param name="accountName">account name</param>
		/// <param name="containerName">container name</param>
		/// <param name="errorDescription">error message</param>
		/// <returns>true if particulars are proper</returns>
		public static bool IsValidContainerLocationSpecification(string accountName, string containerName, 
			out string errorDescription)
		{
			errorDescription = "Improper or invalid cloud storage particulars.";

			var accountNameIsValid = IsValidAccountName(accountName, out var accountNameMsg);

			var containerNameIsValid = IsValidContainerName(containerName, out var containerNameMsg);

			if (accountNameIsValid && containerNameIsValid)
				return true;

			var sb = new StringBuilder();

			sb.Append(errorDescription);

			if (!accountNameIsValid)
			{
				sb.Append(" ");
				sb.Append(accountNameMsg);
			}

			if (!containerNameIsValid)
			{
				sb.Append(" ");
				sb.Append(containerNameMsg);
			}

			errorDescription = sb.ToString();

			return false;
		}

		//public static bool ThrowArgumentExceptionIfStorageParticularsAreInvalid(string accountName, string containerName, string blobName)
		//{
		//	ThrowArgumentExceptionIfStorageParticularsAreInvalid(accountName, containerName, blobName, string.Empty);

		//	return true;
		//}

		public static bool ThrowArgumentExceptionIfStorageParticularsAreInvalid(string accountName, string containerName, string blobName, string prefixMessage)
		{
			// WebException message commonly includes mention of 403 (forbidden) or 400 (bad request) or 404 (not found)
			// very important that the strings 403, or 400, or 404 be included in the message for downstream error handling

			var errorDescription = "One or more Azure storage particulars rejected.";
			//var errorDescription = "Disallowed Azure storage particulars. Http status: 400 (bad request)";

			var accountNameIsValid = IsValidAccountName(accountName, out var accountNameMsg);

			var containerNameIsValid = IsValidContainerName(containerName, out var containerNameMsg);

			var blobNameIsValid = IsValidBlobName(blobName, out var blobNameMsg);

			if (accountNameIsValid && containerNameIsValid && blobNameIsValid)
				return true;

			var sb = new StringBuilder();

			if (!string.IsNullOrWhiteSpace(prefixMessage))
			{
				sb.Append(prefixMessage);
			}

			if (!string.IsNullOrWhiteSpace(errorDescription))
			{
				sb.Append(" ");
				sb.Append(errorDescription);
			}

			if (!accountNameIsValid)
			{
				sb.Append(" ");
				sb.Append(accountNameMsg);
			}

			if (!containerNameIsValid)
			{
				sb.Append(" ");
				sb.Append(containerNameMsg);
			}

			if (!blobNameIsValid)
			{
				sb.Append(" ");
				sb.Append(blobNameMsg);
			}


			errorDescription = sb.ToString();

			throw new ArgumentException(errorDescription);
		}

		/// <summary>
		///     Composes valid connection string for Azure storage account.
		///		Throws informative ArgumentException exception if unable to create a valid answer based on the provided parameters.
		///		Throws ArgumentNullException exception for null parameters.
		/// </summary>
		/// <param name="defaultEndpointsProtocol">http or https</param>
		/// <param name="accountName">valid name</param>
		/// <param name="accountKey">valid name</param>
		/// <returns>
		///     Valid connection string
		/// </returns>
		/// <exception cref="System.ArgumentNullException" />
		/// <exception cref="ArgumentException">the account name is badly formatted</exception>
		public static string MakeAzureAccountConnectionString(string defaultEndpointsProtocol, string accountName, string accountKey)
		{
			// Step 1. null checks

			if (defaultEndpointsProtocol == null)
				throw new ArgumentNullException(nameof(defaultEndpointsProtocol));

			if (accountName == null)
				throw new ArgumentNullException(nameof(accountName));

			if (accountKey == null)
				throw new ArgumentNullException(nameof(accountKey));

			// Step 2. validate account name

			if (!IsValidBlobLocationSpecification(accountName, "dummy", "dummy",
				out var errorMessage))
				throw new ArgumentException(errorMessage);

			// Step 3. validate transmisssion protocol

			if (!((defaultEndpointsProtocol == "http") | (defaultEndpointsProtocol == "https")))
				throw new ArgumentException(
					$@"Transmission protocol specified was '{defaultEndpointsProtocol}'. This is invalid. It must be http for regular communications or https for secure communications.");

			// Step 4. compose answer

			var answer =
				$"DefaultEndpointsProtocol={defaultEndpointsProtocol};AccountName={accountName};AccountKey={accountKey}";

			return answer;
		}

		/// <summary>
		///     Composes valid Uri for connection to public Azure blob storage, with http prefix
		/// </summary>
		/// <param name="accountName">valid name</param>
		/// <param name="containerName">valid name</param>
		/// <param name="blobName">valid name</param>
		/// <returns>Uri</returns>
		/// <exception cref="ArgumentException">
		///     any one argument is invalid or arguments collectively fail to
		///     concatenate into a valid uri
		/// </exception>
		public static Uri MakeAzureBlobUriHttp(string accountName, string containerName, string blobName)
		{
			var errorMessage = string.Empty;

			try
			{
				if (!IsValidBlobLocationSpecification(accountName, containerName, blobName, out errorMessage))
					throw new ArgumentException(errorMessage);

				var requestUriString = MakeAzureBlobStorageHttpRequestUriString(accountName, containerName, blobName);

				var answer = new Uri(requestUriString);

				return answer;
			}

			catch (Exception)
			{
				throw new ArgumentException(errorMessage);
			}
		}

		/// <summary>
		///     Composes valid Uri for connection to Azure blob storage, with https prefix.
		///     Account name, container name and blob name must comply with requirements of both Azure and Rezultz.
		///     Rezultz imposes additional requirements, like for example that the blob name can't contain spaces.
		/// </summary>
		/// <param name="accountName">valid name</param>
		/// <param name="containerName">valid name</param>
		/// <param name="blobName">valid name</param>
		/// <returns>Uri</returns>
		/// <exception cref="ArgumentException">
		///     any one argument is invalid or arguments collectively fail to
		///     concatenate into a valid uri
		/// </exception>
		public static Uri MakeAzureAndRezultzCompliantBlobUriHttps(string accountName, string containerName,
			string blobName)
		{
			var errorMessage = string.Empty;

			try
			{
				if (!IsValidBlobLocationSpecification(accountName, containerName, blobName, out errorMessage))
					throw new ArgumentException(errorMessage);

				var requestUriString = MakeAzureBlobStorageHttpRequestUriString(accountName, containerName, blobName);

				var answer = new Uri(requestUriString);

				return answer;
			}

			catch (Exception)
			{
				throw new ArgumentException(errorMessage);
			}
		}

		/// <summary>
		///     Composes valid Uri for connection to Azure blob storage, with https prefix.
		///     Account name and container name must comply with requirements of Azure and Rezultz.
		///     Blob name can be anything.
		/// </summary>
		/// <param name="accountName">valid name</param>
		/// <param name="containerName">valid name</param>
		/// <param name="blobName">valid name</param>
		/// <returns>Uri</returns>
		/// <exception cref="ArgumentException">
		///     any one argument is invalid or arguments collectively fail to
		///     concatenate into a valid uri
		/// </exception>
		public static Uri MakeAzureCompliantBlobUriHttps(string accountName, string containerName, string blobName)
		{
			var errorMessage = string.Empty;

			try
			{
				var requestUriString = MakeAzureBlobStorageHttpRequestUriString(accountName, containerName, blobName);

				if (!IsValidUriString(requestUriString, out var answer, out errorMessage))
					throw new ArgumentException(errorMessage);

				if (!IsValidBlobLocationSpecification(accountName, containerName, "dummy", out errorMessage))
					throw new ArgumentException(errorMessage);

				return answer;
			}

			catch (Exception)
			{
				throw new ArgumentException(errorMessage);
			}
		}

		#endregion

		#region NTFS

		/// <summary>
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
		/// <param name="inputChars">file or folder name.</param>
		/// <returns>true if name is proper</returns>
		public static bool IsValidNtfsFileOrFolderName(string inputChars)
		{
			return IsValidNtfsFileOrFolderName(inputChars, out _);
		}

		/// <summary>
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
		/// <param name="inputChars">file or folder name.</param>
		/// <param name="errorDescription">explanation of error</param>
		/// <returns>true if name is proper</returns>
		public static bool IsValidNtfsFileOrFolderName(string inputChars, out string errorDescription)
		{
			const string failure = "Unable to determine the validity of a NTFS file or folder name.";
			const string locus = "[IsValidNtfsFileOrFolderName]";

			errorDescription = string.Empty;

			try
			{
				if (string.IsNullOrEmpty(inputChars))
				{
					errorDescription = "Name is empty.";
					return false;
				}

				if (inputChars[0] == ' ')
				{
					errorDescription =
						"Name begins with a space. Leading spaces are forbidden.";
					return false;
				}

				if (inputChars.Length > 255)
				{
					errorDescription =
						$"Name contains {inputChars.Length} characters. More than 255 characters are forbidden.";
					return false;
				}


				foreach (var inputChar in inputChars.ToArray())
				{
					if (!IsForbiddenTypeOfNtfsFileOrFolderCharacter(inputChar, out var problematicCharacterMsg))
						continue;

					errorDescription =
						$"Name contains at least one character which is prohibited in file and folder names. Here is the first such character. <{problematicCharacterMsg}>";

					return false;
				}
			}
			catch (Exception ex)
			{
				errorDescription = JghString.ConcatAsSentences(failure, ex.Message, locus, null);
				return true;
			}

			return true;
		}

		/// <summary>
		///     NTFS paths must be less than 260 characters long and musn't have a leading space.
		///     Everything prohibited in filenames is also prohibited in directory paths
		///     with the exception of the volume separator (:) and directory separator (\ or /)
		///     which are permitted.
		///     File names cannot contain backslash (\), slash (/), colon (:),
		///     asterisk (*), question mark (?), quote ("), LessThan,
		///     GreaterThan, or pipe (|).  The first three are used as directory
		///     separators on various platforms.  Asterisk and question mark are treated
		///     as wild cards.  LessThan, GreaterThan, and pipe all redirect input
		///     or output from a program to a file or some combination thereof.  Quotes
		///     are special.
		///     If wildCardsAreInvalid is set to true then the wildcard characters "*" and "?" are deemed invalid. Otherwise they
		///     are OK.
		/// </summary>
		/// <param name="inputChars">path</param>
		/// <param name="wildCardsAreInvalid">
		///     if set to <c>true</c> then the wildcard characters "*" and "?" are also deemed
		///     invalid. Otherwise they are OK.
		/// </param>
		/// <returns>true if path is proper</returns>
		public static bool IsValidNtfsDirectoryPath(string inputChars, bool wildCardsAreInvalid)
		{
			return IsValidNtfsDirectoryPath(inputChars, wildCardsAreInvalid, out _);
		}

		/// <summary>
		///     NTFS paths must be less than 260 characters long and musn't have a leading space.
		///     Everything prohibited in filenames is prohibited in directory paths
		///     with the exception of the volume separator (:) and directory separator (\ or /)
		///     which are permitted.
		///     File names cannot contain backslash (\), slash (/), colon (:),
		///     asterisk (*), question mark (?), quote ("), LessThan,
		///     GreaterThan, or pipe (|).  The first three are used as directory
		///     separators on various platforms.  Asterisk and question mark are treated
		///     as wild cards.  LessThan, GreaterThan, and pipe all redirect input
		///     or output from a program to a file or some combination thereof.  Quotes
		///     are special.
		///     If wildCardsAreInvalid is set to true then the wildcard characters "*" and "?" are deemed invalid. Otherwise they
		///     are OK.
		/// </summary>
		/// <param name="inputChars">path</param>
		/// <param name="wildCardsAreInvalid">
		///     if set to <c>true</c> then the wildcard characters "*" and "?" are also deemed
		///     invalid. Otherwise they are OK.
		/// </param>
		/// <param name="errorDescription">explanation of error</param>
		/// <returns>true if path is proper</returns>
		public static bool IsValidNtfsDirectoryPath(string inputChars, bool wildCardsAreInvalid, out string errorDescription)
		{
			const string failure = "Unable to determine the validity of a NTFS path.";
			const string locus = "[IsValidNtfsDirectoryPath]";

			errorDescription = string.Empty;

			try
			{
				if (string.IsNullOrEmpty(inputChars))
				{
					errorDescription = "Path is empty.";
					return false;
				}

				if (inputChars[0] == ' ')
				{
					errorDescription =
						"Path begins with a space. Leading spaces are forbidden.";
					return false;
				}

				if (inputChars.Length > 260)
				{
					errorDescription =
						$"Path contains {inputChars.Length} characters. More than 260 characters are forbidden.";
					return false;
				}

				foreach (var inputChar in inputChars.ToArray())
				{
					if (
						!IsForbiddenTypeOfNtfsDirectoryPathCharacter(inputChar, wildCardsAreInvalid,
							out var problematicCharacterMsg)) continue;

					errorDescription =
						$"Path contains at least one character which is prohibited. Here is the first such character. {problematicCharacterMsg}";

					return false;
				}
			}
			catch (Exception ex)
			{
				errorDescription = JghString.ConcatAsSentences(failure, ex.Message, locus, null);
				return true;
			}

			return true;
		}

        public static string MakeSimpleRezultzNtfsFileNameWithTimestampPrefix(string suffix)
        {
            var timestamp = DateTime.Now.ToString(JghDateTime.SortablePattern);

            var candidate = $"{timestamp}+{suffix}";

            var hopefullyValidFileName = AttemptToMakeValidNtfsFileOrFolderNameByReplacingInvalidCharacters('-', candidate);

            return hopefullyValidFileName;
        }

        /// <summary>
		///     Attempts to make a valid NTFS file or folder name by replacing invalid characters, if any, with a specified
		///     replacement character.
		/// </summary>
		/// <param name="replacementChar">The replacement character substituted for each invalid character</param>
		/// <param name="provisionalName">The provisional file or folder name</param>
		/// <returns></returns>
		public static string AttemptToMakeValidNtfsFileOrFolderNameByReplacingInvalidCharacters(char replacementChar, string provisionalName)
		{
			var characters = provisionalName.ToCharArray();

			for (var i = 0; i < characters.Length; i++)
				if (IsForbiddenTypeOfNtfsFileOrFolderCharacter(characters[i], out _))
					characters[i] = replacementChar;

			var sb = new StringBuilder();
			foreach (var character in characters)
				sb.Append(character);
			var answer = sb.ToString();

			return answer;
		}

		//public static string TryMakeValidNtfsFileName(string draftFileName)
		//{
		//	var timestamp = DateTime.Now.ToString(JghDateTime.Iso8601Pattern);


		//	var hopefullyValidFileName = AttemptToMakeValidNtfsFileOrFolderNameByReplacingInvalidCharacters('-',
		//		draftFileName);

		//	if (!IsValidNtfsFileOrFolderName(hopefullyValidFileName,
		//		out _))
		//		return JghString.ConcatWithSeparator("-",
		//			ExportImportDataSerialiserNames.SuffixForFileNamesDenotingDataExportedByRezultz,
		//			timestamp);

		//	return hopefullyValidFileName;
		//}

		/// <summary>
		///     Used as a predicate to test if a character is forbidden in a NTFS file or folder name.
		///     File names cannot contain backslash (\), slash (/), colon (:),
		///     asterisk (*), question mark (?), quote ("), LessThan,
		///     GreaterThan, or pipe (|).  The first three are used as directory
		///     separators on various platforms.  Asterisk and question mark are treated
		///     as wild cards.  LessThan, GreaterThan, and pipe all redirect input
		///     or output from a program to a file or some combination thereof.  Quotes
		///     are special.
		/// </summary>
		/// <param name="inputChar">candidate character</param>
		/// <param name="errorDescription">friendly decription of forbidden character</param>
		/// <returns>true if the character is forbidden</returns>
		public static bool IsForbiddenTypeOfNtfsFileOrFolderCharacter(char inputChar, out string errorDescription)
		{
			const string failure = "Unable to determine the acceptability of a character.";
			const string locus = "[IsForbiddenTypeOfNtfsFileOrFolderCharacter]";

			errorDescription = string.Empty;

			try
			{
				if (JghPath.InvalidFileNameChars.Contains(inputChar))
				{
					errorDescription = $"Prohibited character is {inputChar}.";

					return true;
				}
			}
			catch (Exception ex)
			{
				errorDescription = JghString.ConcatAsSentences(failure, ex.Message, locus, null);
				return true;
			}

			return false;
		}

		/// <summary>
		///     Everything prohibited in filenames is also prohibited in directory paths
		///     with the exception of the volume separator (:) and directory separator (\ or /)
		///     which are permitted.
		///     File names cannot contain backslash (\), slash (/), colon (:),
		///     asterisk (*), question mark (?), quote ("), LessThan,
		///     GreaterThan, or pipe (|).  The first three are used as directory
		///     separators on various platforms.  Asterisk and question mark are treated
		///     as wild cards.  LessThan, GreaterThan, and pipe all redirect input
		///     or output from a program to a file or some combination thereof.  Quotes
		///     are special.
		///     If wildCardsAreInvalid is set to true then the wildcard characters "*" and "?" are deemed invalid. Otherwise they
		///     are OK.
		/// </summary>
		/// <param name="inputChar">candidate character</param>
		/// <param name="wildCardsAreInvalidToo">
		///     if set to <c>true</c> then the wildcard characters "*" and "?" are invalid.
		///     Otherwise they are OK.
		/// </param>
		/// <param name="errorDescription">friendly decription of forbidden character</param>
		/// <returns>true if the character is forbidden</returns>
		public static bool IsForbiddenTypeOfNtfsDirectoryPathCharacter(char inputChar, bool wildCardsAreInvalidToo, out string errorDescription)
		{
			const string failure = "Unable to determine the acceptability of a character.";
			const string locus = "[IsForbiddenTypeOfNtfsDirectoryPathCharacter]";

			errorDescription = string.Empty;

			try
			{
				var invalidCharacters = wildCardsAreInvalidToo
					? JghPath.InvalidPathCharsWithWildCardsAlsoInvalid
					: JghPath.RealInvalidPathCharsWithWildCardsOk;

				if (invalidCharacters.Contains(inputChar))
				{
					errorDescription = $"Prohibited character is {inputChar}.";

					return true;
				}
			}
			catch (Exception ex)
			{
				errorDescription = JghString.ConcatAsSentences(failure, ex.Message, locus, null);
				return true;
			}

			return false;
		}

		public static string RemoveAllExtensions(string fileNameAsString)
		{
			var sb = new StringBuilder();

			foreach (var character in fileNameAsString.ToCharArray())
			{
				if (character == '.')
					break;

				sb.Append(character);
			}
			var answer = sb.ToString();

			return answer;
		}

		public static string AddDotTxtExtension(string fileNameAsString)
		{
			return string.Concat(fileNameAsString, ".", "txt");
		}

		public static string AddDotXmlExtension(string fileNameAsString)
		{
			return string.Concat(fileNameAsString, ".", StandardFileTypeSuffix.Xml);
		}

		public static string AddDotXmlDotGzExtension(string fileNameAsString)
		{
			return string.Concat(AddDotXmlExtension(fileNameAsString), ".", StandardFileTypeSuffix.Gz);
		}

		#endregion

		#region web uri stuff

		/// <summary>
		///     Uri strings must be correctly formed to be usable
		/// </summary>
		/// <param name="candidateString">uri string candidate. The candidate must be a fully fledged Uri with an unambiguous, self-evident format, not merely a fragment of a Uri</param>
		/// <param name="resultantUri">resultant uri if successful, or null if not</param>
		/// <param name="errorDescription">explanation of problem, which may unavoidably be somewhat inscrutable unfortunately</param>
		/// <returns>true if valid</returns>
		public static bool IsValidUriString(string candidateString, out Uri resultantUri, out string errorDescription)
		{
			const string failure = "Unable to determine if url is validly properly formatted.";
			const string locus = "[IsValidUriString]";

			errorDescription = string.Empty;

			resultantUri = null; // default

			try
			{
				resultantUri = new Uri(candidateString);

				//if (!System.Uri.IsWellFormedUriString(candidateString, UriKind.RelativeOrAbsolute))
				//{
				//    errorDescription =
				//        "Microsoft System.Uri warning. A section of the address provided is not permitted over the internet. There is a problem with what it contains, or with what is missing.";
				//    return false;
				//}

				//if (!Uri.TryCreate(candidateString, UriKind.RelativeOrAbsolute, out var dummyUri))
				//{
				//    errorDescription =
				//        "Microsoft System.Uri warning. A section of the address provided is not permitted over the internet. System Uri parser threw an exception when it tried to create a valid uri out of the address.";
				//    return false;
				//}

				// if we've got this far, yipee

				//resultantUri = dummyUri;

				return true;
			}
			catch (ArgumentNullException ex)
			{
				errorDescription = JghString.ConcatAsSentences($"Failure. Candidate uri string is inadequate or improper. Candidate is <{candidateString}>.", ex.Message);
				return false;
			}
			catch (UriFormatException exx)
			{
				errorDescription = JghString.ConcatAsSentences($"Failure. Candidate uri string is inadequate or improper. Candidate is <{candidateString}>.", exx.Message);
				return false;
			}
			catch (Exception exxx)
			{
				errorDescription = JghString.ConcatAsSentences(failure, exxx.Message, locus, Locus2, Locus3);
				return false;
			}
		}

		/// <summary>
		///     uri strings must be well formed to be usable
		/// </summary>
		/// <param name="candidateString">uri string candidate</param>
		/// <returns>string if string is proper or "http://default" if not</returns>
		public static string CreateValidUriString(string candidateString)
		{
			const string failure = "Unable to create valid uri";
			const string locus = "[CreateValidUriString]";

			if (string.IsNullOrWhiteSpace(candidateString))
				return "http://default";

			try
			{
				return IsValidUriString(candidateString, out var resultantUri, out var dummyErrorMessage)
					? candidateString
					: "http://default";
			}
			catch (Exception ex)
			{
				throw new Exception(JghString.ConcatAsSentences(failure, locus, Locus2, Locus3), ex);
			}
		}

		#endregion

		#region helpers

		private static string MakeAzureBlobStorageHttpRequestUriString(string accountName, string containerName,
			string blobName)
		{
			var requestUriString = $"https://{accountName}.blob.core.windows.net/{containerName}/{blobName}";
			//var requestUriString = $"http://{accountName}.blob.core.windows.net/{containerName}/{blobName}";

			return requestUriString;
		}

		/// <summary>
		///     Used to extract and concatenate any forbidden characters in an Azure account name.
		///     Only letters and digits are permitted.
		/// </summary>
		/// <param name="inputChars">account name</param>
		/// <returns>non-empty list or empty list if there are no forbidden characters.</returns>
		private static List<char> GetInvalidAzureAccountNameChars(string inputChars)
		{
			var answer = new List<char>();

			try
			{
				answer.AddRange(inputChars.ToCharArray().Where(inputChar => !char.IsLetterOrDigit(inputChar)));
			}
			catch (Exception)
			{
				return new List<char>();
			}

			return answer;
		}

		/// <summary>
		///     Used to extract and concatenate any forbidden characters in an Azure container name.
		///     Only letters, digits and dashes are allowed.
		/// </summary>
		/// <param name="inputChars">container name</param>
		/// <returns>non-empty list or empty list if there are no forbidden characters.</returns>
		private static List<char> GetInvalidAzureContainerNameChars(string inputChars)
		{
			var answer = new List<char>();

			try
			{
				answer.AddRange(
					inputChars.ToCharArray()
						.Where(inputChar => !(char.IsLetterOrDigit(inputChar) | (inputChar == '-'))));
			}
			catch (Exception)
			{
				return new List<char>();
			}

			return answer;
		}

		#endregion

    }
}