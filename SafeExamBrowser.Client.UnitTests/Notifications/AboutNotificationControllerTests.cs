﻿/*
 * Copyright (c) 2019 ETH Zürich, Educational Development and Technology (LET)
 * 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SafeExamBrowser.Client.Notifications;
using SafeExamBrowser.Contracts.Configuration;
using SafeExamBrowser.Contracts.UserInterface;
using SafeExamBrowser.Contracts.UserInterface.Windows;

namespace SafeExamBrowser.Client.UnitTests.Notifications
{
	[TestClass]
	public class AboutNotificationControllerTests
	{
		private Mock<AppConfig> appConfig;
		private Mock<IUserInterfaceFactory> uiFactory;

		[TestInitialize]
		public void Initialize()
		{
			appConfig = new Mock<AppConfig>();
			uiFactory = new Mock<IUserInterfaceFactory>();
		}

		[TestMethod]
		public void MustCloseWindowWhenTerminating()
		{
			var button = new NotificationButtonMock();
			var window = new Mock<IWindow>();
			var sut = new AboutNotificationController(appConfig.Object, uiFactory.Object);

			uiFactory.Setup(u => u.CreateAboutWindow(It.IsAny<AppConfig>())).Returns(window.Object);
			sut.RegisterNotification(button);
			button.Click();
			sut.Terminate();

			window.Verify(w => w.Close());
		}

		[TestMethod]
		public void MustOpenOnlyOneWindow()
		{
			var button = new NotificationButtonMock();
			var window = new Mock<IWindow>();
			var sut = new AboutNotificationController(appConfig.Object, uiFactory.Object);

			uiFactory.Setup(u => u.CreateAboutWindow(It.IsAny<AppConfig>())).Returns(window.Object);
			sut.RegisterNotification(button);
			button.Click();
			button.Click();
			button.Click();
			button.Click();
			button.Click();

			uiFactory.Verify(u => u.CreateAboutWindow(It.IsAny<AppConfig>()), Times.Once);
			window.Verify(u => u.Show(), Times.Once);
			window.Verify(u => u.BringToForeground(), Times.Exactly(4));
		}

		[TestMethod]
		public void MustSubscribeToClickEvent()
		{
			var button = new NotificationButtonMock();
			var sut = new AboutNotificationController(appConfig.Object, uiFactory.Object);

			sut.RegisterNotification(button);

			Assert.IsTrue(button.HasSubscribed);
		}
	}
}
