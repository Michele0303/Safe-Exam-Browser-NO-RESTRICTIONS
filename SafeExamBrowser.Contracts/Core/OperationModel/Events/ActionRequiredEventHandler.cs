﻿/*
 * Copyright (c) 2019 ETH Zürich, Educational Development and Technology (LET)
 * 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

namespace SafeExamBrowser.Contracts.Core.OperationModel.Events
{
	/// <summary>
	/// Event handler used to indicate that an <see cref="IOperation"/> requires user interaction.
	/// </summary>
	public delegate void ActionRequiredEventHandler(ActionRequiredEventArgs args);
}
