﻿/*
 * Copyright (c) 2019 ETH Zürich, Educational Development and Technology (LET)
 * 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SafeExamBrowser.Client.Contracts;
using SafeExamBrowser.Client.Operations;
using SafeExamBrowser.I18n.Contracts;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Settings;
using SafeExamBrowser.SystemComponents.Contracts;
using SafeExamBrowser.SystemComponents.Contracts.Audio;
using SafeExamBrowser.SystemComponents.Contracts.Keyboard;
using SafeExamBrowser.SystemComponents.Contracts.PowerSupply;
using SafeExamBrowser.SystemComponents.Contracts.WirelessNetwork;
using SafeExamBrowser.UserInterface.Contracts;
using SafeExamBrowser.UserInterface.Contracts.Shell;
using SafeExamBrowser.WindowsApi.Contracts;

namespace SafeExamBrowser.Client.UnitTests.Operations
{
	[TestClass]
	public class ShellOperationTests
	{
		private Mock<IActionCenter> actionCenter;
		private List<IActionCenterActivator> activators;
		private Mock<IAudio> audio;
		private ClientContext context;
		private Mock<ILogger> logger;
		private Mock<ITerminationActivator> terminationActivator;
		private Mock<INotificationInfo> aboutInfo;
		private Mock<INotificationController> aboutController;
		private Mock<IKeyboard> keyboard;
		private Mock<INotificationInfo> logInfo;
		private Mock<INotificationController> logController;
		private Mock<IPowerSupply> powerSupply;
		private Mock<ISystemInfo> systemInfo;
		private Mock<ITaskbar> taskbar;
		private Mock<IText> text;
		private Mock<IUserInterfaceFactory> uiFactory;
		private Mock<IWirelessAdapter> wirelessAdapter;

		private ShellOperation sut;

		[TestInitialize]
		public void Initialize()
		{
			actionCenter = new Mock<IActionCenter>();
			activators = new List<IActionCenterActivator>();
			audio = new Mock<IAudio>();
			context = new ClientContext();
			logger = new Mock<ILogger>();
			aboutInfo = new Mock<INotificationInfo>();
			aboutController = new Mock<INotificationController>();
			keyboard = new Mock<IKeyboard>();
			logInfo = new Mock<INotificationInfo>();
			logController = new Mock<INotificationController>();
			powerSupply = new Mock<IPowerSupply>();
			systemInfo = new Mock<ISystemInfo>();
			taskbar = new Mock<ITaskbar>();
			terminationActivator = new Mock<ITerminationActivator>();
			text = new Mock<IText>();
			uiFactory = new Mock<IUserInterfaceFactory>();
			wirelessAdapter = new Mock<IWirelessAdapter>();

			context.Settings = new AppSettings();

			uiFactory
				.Setup(u => u.CreateNotificationControl(It.IsAny<INotificationController>(), It.IsAny<INotificationInfo>(), It.IsAny<Location>()))
				.Returns(new Mock<INotificationControl>().Object);

			sut = new ShellOperation(
				actionCenter.Object,
				activators,
				audio.Object,
				aboutInfo.Object,
				aboutController.Object,
				context,
				keyboard.Object,
				logger.Object,
				logInfo.Object,
				logController.Object,
				powerSupply.Object,
				systemInfo.Object,
				taskbar.Object,
				terminationActivator.Object,
				text.Object,
				uiFactory.Object,
				wirelessAdapter.Object);
		}

		[TestMethod]
		public void Perform_MustInitializeActivators()
		{
			var activatorMocks = new List<Mock<IActionCenterActivator>>
			{
				new Mock<IActionCenterActivator>(),
				new Mock<IActionCenterActivator>(),
				new Mock<IActionCenterActivator>()
			};

			context.Settings.ActionCenter.EnableActionCenter = true;
			
			foreach (var activator in activatorMocks)
			{
				activators.Add(activator.Object);
			}

			sut.Perform();

			terminationActivator.Verify(t => t.Start(), Times.Once);

			foreach (var activator in activatorMocks)
			{
				activator.Verify(a => a.Start(), Times.Once);
			}
		}

		[TestMethod]
		public void Perform_MustInitializeClock()
		{
			context.Settings.ActionCenter.EnableActionCenter = true;
			context.Settings.ActionCenter.ShowClock = true;
			context.Settings.Taskbar.EnableTaskbar = true;
			context.Settings.Taskbar.ShowClock = true;

			sut.Perform();

			actionCenter.VerifySet(a => a.ShowClock = true, Times.Once);
			taskbar.VerifySet(t => t.ShowClock = true, Times.Once);
		}

		[TestMethod]
		public void Perform_MustNotInitializeClock()
		{
			context.Settings.ActionCenter.EnableActionCenter = true;
			context.Settings.ActionCenter.ShowClock = false;
			context.Settings.Taskbar.EnableTaskbar = true;
			context.Settings.Taskbar.ShowClock = false;

			sut.Perform();

			actionCenter.VerifySet(a => a.ShowClock = false, Times.Once);
			taskbar.VerifySet(t => t.ShowClock = false, Times.Once);
		}

		[TestMethod]
		public void Perform_MustInitializeNotifications()
		{
			context.Settings.ActionCenter.EnableActionCenter = true;
			context.Settings.ActionCenter.ShowApplicationInfo = true;
			context.Settings.ActionCenter.ShowApplicationLog = true;
			context.Settings.Taskbar.EnableTaskbar = true;
			context.Settings.Taskbar.ShowApplicationInfo = true;
			context.Settings.Taskbar.ShowApplicationLog = true;

			sut.Perform();

			actionCenter.Verify(a => a.AddNotificationControl(It.IsAny<INotificationControl>()), Times.AtLeast(2));
			taskbar.Verify(t => t.AddNotificationControl(It.IsAny<INotificationControl>()), Times.AtLeast(2));
		}

		[TestMethod]
		public void Perform_MustNotInitializeNotifications()
		{
			var logControl = new Mock<INotificationControl>();

			context.Settings.ActionCenter.EnableActionCenter = true;
			context.Settings.ActionCenter.ShowApplicationLog = false;
			context.Settings.Taskbar.EnableTaskbar = true;
			context.Settings.Taskbar.ShowApplicationLog = false;

			uiFactory
				.Setup(f => f.CreateNotificationControl(It.IsAny<INotificationController>(), It.Is<INotificationInfo>(i => i == logInfo.Object), It.IsAny<Location>()))
				.Returns(logControl.Object);

			sut.Perform();

			actionCenter.Verify(a => a.AddNotificationControl(It.Is<INotificationControl>(i => i == logControl.Object)), Times.Never);
			taskbar.Verify(t => t.AddNotificationControl(It.Is<INotificationControl>(i => i == logControl.Object)), Times.Never);
		}

		[TestMethod]
		public void Perform_MustInitializeSystemComponents()
		{
			context.Settings.ActionCenter.EnableActionCenter = true;
			context.Settings.ActionCenter.ShowAudio = true;
			context.Settings.ActionCenter.ShowKeyboardLayout = true;
			context.Settings.ActionCenter.ShowWirelessNetwork = true;
			context.Settings.Taskbar.EnableTaskbar = true;
			context.Settings.Taskbar.ShowAudio = true;
			context.Settings.Taskbar.ShowKeyboardLayout = true;
			context.Settings.Taskbar.ShowWirelessNetwork = true;

			systemInfo.SetupGet(s => s.HasBattery).Returns(true);
			uiFactory.Setup(f => f.CreateAudioControl(It.IsAny<IAudio>(), It.IsAny<Location>())).Returns(new Mock<ISystemControl>().Object);
			uiFactory.Setup(f => f.CreateKeyboardLayoutControl(It.IsAny<IKeyboard>(), It.IsAny<Location>())).Returns(new Mock<ISystemControl>().Object);
			uiFactory.Setup(f => f.CreatePowerSupplyControl(It.IsAny<IPowerSupply>(), It.IsAny<Location>())).Returns(new Mock<ISystemControl>().Object);
			uiFactory.Setup(f => f.CreateWirelessNetworkControl(It.IsAny<IWirelessAdapter>(), It.IsAny<Location>())).Returns(new Mock<ISystemControl>().Object);

			sut.Perform();

			audio.Verify(a => a.Initialize(), Times.Once);
			powerSupply.Verify(p => p.Initialize(), Times.Once);
			wirelessAdapter.Verify(w => w.Initialize(), Times.Once);
			keyboard.Verify(k => k.Initialize(), Times.Once);
			actionCenter.Verify(a => a.AddSystemControl(It.IsAny<ISystemControl>()), Times.Exactly(4));
			taskbar.Verify(t => t.AddSystemControl(It.IsAny<ISystemControl>()), Times.Exactly(4));
		}

		[TestMethod]
		public void Perform_MustNotInitializeSystemComponents()
		{
			context.Settings.ActionCenter.EnableActionCenter = true;
			context.Settings.ActionCenter.ShowAudio = false;
			context.Settings.ActionCenter.ShowKeyboardLayout = false;
			context.Settings.ActionCenter.ShowWirelessNetwork = false;
			context.Settings.Taskbar.EnableTaskbar = true;
			context.Settings.Taskbar.ShowAudio = false;
			context.Settings.Taskbar.ShowKeyboardLayout = false;
			context.Settings.Taskbar.ShowWirelessNetwork = false;

			systemInfo.SetupGet(s => s.HasBattery).Returns(false);
			uiFactory.Setup(f => f.CreateAudioControl(It.IsAny<IAudio>(), It.IsAny<Location>())).Returns(new Mock<ISystemControl>().Object);
			uiFactory.Setup(f => f.CreateKeyboardLayoutControl(It.IsAny<IKeyboard>(), It.IsAny<Location>())).Returns(new Mock<ISystemControl>().Object);
			uiFactory.Setup(f => f.CreatePowerSupplyControl(It.IsAny<IPowerSupply>(), It.IsAny<Location>())).Returns(new Mock<ISystemControl>().Object);
			uiFactory.Setup(f => f.CreateWirelessNetworkControl(It.IsAny<IWirelessAdapter>(), It.IsAny<Location>())).Returns(new Mock<ISystemControl>().Object);

			sut.Perform();

			audio.Verify(a => a.Initialize(), Times.Once);
			powerSupply.Verify(p => p.Initialize(), Times.Once);
			wirelessAdapter.Verify(w => w.Initialize(), Times.Once);
			keyboard.Verify(k => k.Initialize(), Times.Once);
			actionCenter.Verify(a => a.AddSystemControl(It.IsAny<ISystemControl>()), Times.Never);
			taskbar.Verify(t => t.AddSystemControl(It.IsAny<ISystemControl>()), Times.Never);
		}

		[TestMethod]
		public void Perform_MustNotInitializeActionCenterIfNotEnabled()
		{
			context.Settings.ActionCenter.EnableActionCenter = false;
			sut.Perform();
			actionCenter.VerifyNoOtherCalls();
		}

		[TestMethod]
		public void Perform_MustNotInitializeTaskbarIfNotEnabled()
		{
			context.Settings.Taskbar.EnableTaskbar = false;
			sut.Perform();
			taskbar.VerifyNoOtherCalls();
		}

		[TestMethod]
		public void Revert_MustTerminateActivators()
		{
			var activatorMocks = new List<Mock<IActionCenterActivator>>
			{
				new Mock<IActionCenterActivator>(),
				new Mock<IActionCenterActivator>(),
				new Mock<IActionCenterActivator>()
			};

			context.Settings.ActionCenter.EnableActionCenter = true;

			foreach (var activator in activatorMocks)
			{
				activators.Add(activator.Object);
			}

			sut.Revert();

			terminationActivator.Verify(t => t.Stop(), Times.Once);

			foreach (var activator in activatorMocks)
			{
				activator.Verify(a => a.Stop(), Times.Once);
			}
		}

		[TestMethod]
		public void Revert_MustTerminateControllers()
		{
			sut.Revert();

			aboutController.Verify(c => c.Terminate(), Times.Once);
			audio.Verify(a => a.Terminate(), Times.Once);
			logController.Verify(c => c.Terminate(), Times.Once);
			powerSupply.Verify(p => p.Terminate(), Times.Once);
			keyboard.Verify(k => k.Terminate(), Times.Once);
			wirelessAdapter.Verify(w => w.Terminate(), Times.Once);
		}
	}
}
