﻿//  <Telegram.Bot.Framework>
//  Copyright (C) <2022 - 2024>  <Azumo-Lab> see <https://github.com/Azumo-Lab/Telegram.Bot.Framework/>
//
//  This file is part of <Telegram.Bot.Framework>: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <https://www.gnu.org/licenses/>.

using Telegram.Bot.Framework.Abstracts.Bots;
using Telegram.Bot.Framework.Interfaces;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Telegram.Bot.Framework
{
    /// <summary>
    /// 这个是一个机器人接口： <see cref="ITelegramBot"/> 接口的实现类
    /// </summary>
    internal class TelegramBot : ITelegramBot, IUpdateHandler, IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IServiceProvider ServiceProvider;

        /// <summary>
        /// 
        /// </summary>
        private readonly ILogger __log;

        /// <summary>
        /// 
        /// </summary>
        private bool __IsEnd;

        /// <summary>
        /// 项目的启动LOGO
        /// </summary>
        private static readonly string LOGO =
@"
████████╗███████╗██╗     ███████╗ ██████╗ ██████╗  █████╗ ███╗   ███╗   ██████╗  ██████╗ ████████╗
╚══██╔══╝██╔════╝██║     ██╔════╝██╔════╝ ██╔══██╗██╔══██╗████╗ ████║   ██╔══██╗██╔═══██╗╚══██╔══╝
   ██║   █████╗  ██║     █████╗  ██║  ███╗██████╔╝███████║██╔████╔██║   ██████╔╝██║   ██║   ██║   
   ██║   ██╔══╝  ██║     ██╔══╝  ██║   ██║██╔══██╗██╔══██║██║╚██╔╝██║   ██╔══██╗██║   ██║   ██║   
   ██║   ███████╗███████╗███████╗╚██████╔╝██║  ██║██║  ██║██║ ╚═╝ ██║██╗██████╔╝╚██████╔╝   ██║██╗
   ╚═╝   ╚══════╝╚══════╝╚══════╝ ╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═╝╚═╝     ╚═╝╚═╝╚═════╝  ╚═════╝    ╚═╝╚═╝
                                                                                                  
███████╗██████╗  █████╗ ███╗   ███╗███████╗██╗    ██╗ ██████╗ ██████╗ ██╗  ██╗                    
██╔════╝██╔══██╗██╔══██╗████╗ ████║██╔════╝██║    ██║██╔═══██╗██╔══██╗██║ ██╔╝                    
█████╗  ██████╔╝███████║██╔████╔██║█████╗  ██║ █╗ ██║██║   ██║██████╔╝█████╔╝                     
██╔══╝  ██╔══██╗██╔══██║██║╚██╔╝██║██╔══╝  ██║███╗██║██║   ██║██╔══██╗██╔═██╗                     
██║     ██║  ██║██║  ██║██║ ╚═╝ ██║███████╗╚███╔███╔╝╚██████╔╝██║  ██║██║  ██╗                    
╚═╝     ╚═╝  ╚═╝╚═╝  ╚═╝╚═╝     ╚═╝╚══════╝ ╚══╝╚══╝  ╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═╝                    
";

        private readonly IUpdateHandler __UpdateHandler;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="ServiceProvider"></param>
        public TelegramBot(IServiceProvider ServiceProvider)
        {
            this.ServiceProvider = ServiceProvider;

            __log = this.ServiceProvider.GetService<ILogger<TelegramBot>>();
            __UpdateHandler = this.ServiceProvider.GetService<IUpdateHandler>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="exception"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            await __UpdateHandler.HandlePollingErrorAsync(botClient, exception, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="update"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            await __UpdateHandler.HandleUpdateAsync(botClient, update, cancellationToken);
        }

        /// <summary>
        /// 开始执行
        /// </summary>
        /// <returns></returns>
        public async Task StartAsync()
        {
            __log.LogInformation(LOGO);
            __log.LogInformation("启动中...");

            try
            {
                ITelegramBotClient telegramBot = ServiceProvider.GetService<ITelegramBotClient>();
                if (telegramBot == null)
                    throw new NullReferenceException(nameof(telegramBot));

                List<IStartExec> startExecs = ServiceProvider.GetServices<IStartExec>().ToList();
                foreach (IStartExec execs in startExecs)
                    await execs.Exec(telegramBot, ServiceProvider);

                telegramBot.StartReceiving(this);

                User user = await telegramBot.GetMeAsync();
                __log.LogInformation(message: $"用户 @{user.Username} 正在运行中...");

                while (!__IsEnd)
                {
                    if (__IsEnd)
                        await telegramBot.CloseAsync();
                    await Task.Delay(500);
                }
            }
            catch (Exception ex)
            {
                __log.LogError("程序启动发生致命错误");
                __log.LogError(ex.ToString());
            }
        }

        /// <summary>
        /// 停止执行
        /// </summary>
        /// <returns></returns>
        public Task StopAsync()
        {
            __IsEnd = true;
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            StopAsync().Wait();
            (ServiceProvider as IDisposable)?.Dispose();
        }
    }
}
