﻿/*
 * Copyright (c) 2019 ETH Zürich, Educational Development and Technology (LET)
 * 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using SafeExamBrowser.Contracts.Configuration;
using SafeExamBrowser.Contracts.Configuration.Settings;
using SafeExamBrowser.Contracts.Logging;
using SafeExamBrowser.Contracts.UserInterface.Browser;
using SafeExamBrowser.Contracts.UserInterface.Taskbar;
using SafeExamBrowser.Contracts.UserInterface.Windows;

namespace SafeExamBrowser.Contracts.UserInterface
{
	/// <summary>
	/// The factory for user interface elements which cannot be instantiated at the composition root. IMPORTANT: To allow for decoupling
	/// from the particular user interface framework in use, all dynamically generated user interface elements must be generated by this
	/// factory.
	/// </summary>
	public interface IUserInterfaceFactory
	{
		/// <summary>
		/// Creates a new about window displaying information about the currently running application version.
		/// </summary>
		IWindow CreateAboutWindow(AppConfig appConfig);

		/// <summary>
		/// Creates a taskbar button, initialized with the given application information.
		/// </summary>
		IApplicationButton CreateApplicationButton(IApplicationInfo info);

		/// <summary>
		/// Creates a new browser window loaded with the given browser control and settings.
		/// </summary>
		IBrowserWindow CreateBrowserWindow(IBrowserControl control, BrowserSettings settings);

		/// <summary>
		/// Creates a system control which allows to change the keyboard layout of the computer.
		/// </summary>
		ISystemKeyboardLayoutControl CreateKeyboardLayoutControl();

		/// <summary>
		/// Creates a new log window which runs on its own thread.
		/// </summary>
		IWindow CreateLogWindow(ILogger logger);

		/// <summary>
		/// Creates a taskbar notification, initialized with the given notification information.
		/// </summary>
		INotificationButton CreateNotification(INotificationInfo info);

		/// <summary>
		/// Creates a password dialog with the given message and title.
		/// </summary>
		IPasswordDialog CreatePasswordDialog(string message, string title);

		/// <summary>
		/// Creates a system control displaying the power supply status of the computer.
		/// </summary>
		ISystemPowerSupplyControl CreatePowerSupplyControl();

		/// <summary>
		/// Creates a new runtime window which runs on its own thread.
		/// </summary>
		/// <returns></returns>
		IRuntimeWindow CreateRuntimeWindow(AppConfig appConfig);

		/// <summary>
		/// Creates a new splash screen which runs on its own thread.
		/// </summary>
		ISplashScreen CreateSplashScreen(AppConfig appConfig = null);

		/// <summary>
		/// Creates a system control which allows to change the wireless network connection of the computer.
		/// </summary>
		ISystemWirelessNetworkControl CreateWirelessNetworkControl();
	}
}
