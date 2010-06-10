﻿#region (c) 2010 Lokad Open Source - New BSD License 

// Copyright (c) Lokad 2010, http://www.lokad.com
// This code is released as Open Source under the terms of the New BSD Licence

#endregion

using Autofac;
using CloudBus;
using CloudBus.Build.Cloud;
using Lokad;
using Microsoft.WindowsAzure.Diagnostics;

namespace Sample_01.Worker
{
	public class WorkerRole : CloudServerHost
	{
		protected override ICloudBusHost BuildHost()
		{
			return new CloudBusBuilder()
				.Domain(d => 
					{
						d.InCurrentAssembly();
						d.WithDefaultInterfaces();
					})
				.HandleMessages(mc =>
					{
						mc.ListenTo("sample-01");
						mc.WithSingleConsumer();
					})
				.SendMessages(m => m.DefaultToQueue("sample-01"))
				.Build();
		}

		public override bool OnStart()
		{
			DiagnosticMonitor.Start("DiagnosticsConnectionString");
			WhenHostStarts += SendFirstMessage;

			return base.OnStart();
		}

		static void SendFirstMessage(ICloudBusHost host)
		{
			var sender = host.Container.Resolve<IBusSender>();
			var game = Rand.String.NextWord();
			sender.Send(new PingPongCommand(0, game));
		}
	}
}