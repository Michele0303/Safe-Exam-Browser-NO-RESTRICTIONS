﻿/*
 * Copyright (c) 2022 ETH Zürich, Educational Development and Technology (LET)
 * 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

namespace SafeExamBrowser.UserInterface.Contracts.FileSystemDialog
{
	/// <summary>
	/// Defines the user interaction result of an <see cref="IFileSystemDialog"/>.
	/// </summary>
	public class FileSystemDialogResult
	{
		/// <summary>
		/// The full path of the item selected by the user, or <c>null</c> if the interaction was unsuccessful.
		/// </summary>
		public string FullPath { get; set; }

		/// <summary>
		/// Indicates whether the user confirmed the dialog or not.
		/// </summary>
		public bool Success { get; set; }
	}
}
