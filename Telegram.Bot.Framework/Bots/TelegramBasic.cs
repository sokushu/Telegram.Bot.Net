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

using Azumo.Reflection;
using System.Reflection;
using System.Runtime.CompilerServices;
using Telegram.Bot.Framework.Abstracts;
using Telegram.Bot.Framework.Abstracts.Attributes;
using Telegram.Bot.Framework.Abstracts.Bots;
using Telegram.Bot.Framework.Abstracts.Controllers;
using Telegram.Bot.Framework.InternalInterface;

namespace Telegram.Bot.Framework.Bots
{
    /// <summary>
    /// 进行框架运行所必须依赖的设置工作
    /// </summary>
    /// <remarks>
    /// 进行基础服务的设置和处理
    /// </remarks>
    internal class TelegramBasic : ITelegramPartCreator
    {
        /// <summary>
        /// 创建时服务
        /// </summary>
        /// <param name="services"></param>
        public void AddBuildService(IServiceCollection services)
        {

        }

        /// <summary>
        /// 运行时服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="builderService"></param>
        public void Build(IServiceCollection services, IServiceProvider builderService)
        {
            Action<ILoggingBuilder> _LogAction = builderService.GetService<Action<ILoggingBuilder>>();
            // 添加Log
            _ = services.AddLogging(option =>
            {
                _ = option.AddSimpleConsole();
                _LogAction?.Invoke(option);
            });
            // 添加 ITelegramBot
            _ = services.AddSingleton<ITelegramBot, TelegramBot>();
        }
    }

    internal class TelegramInstall : ITelegramPartCreator
    {
        public TelegramInstall()
        {

        }
        public void AddBuildService(IServiceCollection services)
        {
            _ = services.AddSingleton<IControllerParamMaker, ControllerParamMaker>();
            AzReflection<ITelegramService> reflection = AzReflection<ITelegramService>.Create();
            foreach (Type item in reflection.FindAllSubclass())
            {
                _ = services.AddSingleton(typeof(ITelegramService), item);
            }
        }

        public void Build(IServiceCollection services, IServiceProvider builderService)
        {
            _ = services.ScanService();

            ControllerManager controllerManager = new();

            AzReflection<TelegramController> azReflection = AzReflection<TelegramController>.Create();
            foreach (Type Controller in azReflection.FindAllSubclass())
            {
                MethodInfo[] methodInfos = Controller.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (MethodInfo Method in methodInfos)
                {
                    if (Attribute.IsDefined(Method, typeof(BotCommandAttribute)))
                    {
                        BotCommandAttribute? botCommandAttribute = Attribute.GetCustomAttribute(Method, typeof(BotCommandAttribute)) as BotCommandAttribute;
                        controllerManager.InternalCommands.Add(new BotCommand
                        {
                            BotCommandName = botCommandAttribute?.BotCommandName == null && botCommandAttribute?.MessageType == null ? $"/{Method.Name.ToLower()}" : botCommandAttribute?.BotCommandName ?? string.Empty,
                            Description = botCommandAttribute?.Description ?? string.Empty,
                            Controller = Controller,
                            Func = (telegram, objs) =>
                            {
                                return Method.Invoke(telegram, objs) is Task task ? task : Task.CompletedTask;
                            },
                            MethodInfo = Method,
                            ControllerParams = Method.GetParameters().Select(p =>
                            {
                                IControllerParamMaker controllerParamMaker = builderService.GetService<IControllerParamMaker>()!;
                                if (Attribute.GetCustomAttribute(p, typeof(ParamAttribute)) is ParamAttribute paramAttribute && paramAttribute.ControllerParamSenderType != null)
                                {
                                    IControllerParamSender controllerParamSender = (IControllerParamSender)ActivatorUtilities.CreateInstance(builderService, paramAttribute.ControllerParamSenderType, []);
                                    return controllerParamMaker.Make(p.ParameterType, controllerParamSender);
                                }
                                return controllerParamMaker.Make(p.ParameterType, null!);
                            }).ToList(),
                            MessageType = botCommandAttribute?.MessageType ?? Types.Enums.MessageType.Unknown
                        });
                    }
                }
            }

            foreach (BotCommand item in controllerManager.InternalCommands)
            {
                RuntimeHelpers.PrepareDelegate(item.Func);
                RuntimeHelpers.PrepareMethod(item.MethodInfo.MethodHandle);
            }

            _ = services.AddSingleton<IControllerManager>(controllerManager);

            foreach (ITelegramService service in builderService.GetServices<ITelegramService>())
            {
                service.AddServices(services);
            }
        }
    }

    public static partial class TelegramBuilderExtensionMethods
    {
        /// <summary>
        /// 添加基础的服务
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        internal static ITelegramBotBuilder AddBasic(this ITelegramBotBuilder builder)
        {
            return builder
                .AddTelegramPartCreator(new TelegramInstall())
                .AddTelegramPartCreator(new TelegramBasic());
        }
    }
}
